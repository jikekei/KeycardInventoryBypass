using System.Collections.Generic;
using InventorySystem.Items.Keycards;
using Interactables.Interobjects.DoorUtils;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;

namespace KeycardInventoryBypass.Shared;

internal sealed class InventoryDoorAccess
{
    private readonly Dictionary<ReferenceHub, PendingUse> _pending = new();

    public void Register()
    {
        PlayerEvents.InteractedDoor += OnInteractedDoor;
        PlayerEvents.Left += OnLeft;
    }

    public void Unregister()
    {
        PlayerEvents.InteractedDoor -= OnInteractedDoor;
        PlayerEvents.Left -= OnLeft;
        _pending.Clear();
    }

    public void BeginInteraction(ReferenceHub player) => _pending.Remove(player);

    public bool TryAuthorize(ReferenceHub player, DoorVariant door, DoorPermissionFlags required, IEnumerable<KeycardItem> cards)
    {
        // The game already handles a held consumable, including its callback on success.
        // A fallback here could turn a denied close into success and consume the wrong card.
        if (player.inventory.CurInstance is SingleUseKeycardItem)
            return false;

        SingleUseKeycardItem? consumable = null;
        foreach (KeycardItem card in cards)
        {
            // Unlike wrapper.Permissions, this checks the target door, closing restrictions,
            // and whether a single-use card has already been used.
            DoorPermissionFlags permissions = card.GetPermissions(door) & ~DoorPermissionFlags.ScpOverride;
            if (required == DoorPermissionFlags.None || (permissions & required) != required)
                continue;

            // Prefer a reusable card; do not spend a pass if another card grants access.
            if (card is not SingleUseKeycardItem singleUse)
                return true;
            consumable ??= singleUse;
        }

        if (consumable is null)
            return false;

        _pending[player] = new PendingUse(door, consumable);
        return true;
    }

    private void OnInteractedDoor(PlayerInteractedDoorEventArgs ev)
    {
        ReferenceHub player = ev.Player.ReferenceHub;
        if (!_pending.TryGetValue(player, out PendingUse? pending) || pending is null)
            return;
        _pending.Remove(player);

        // This event follows the final permission result and actual door state change.
        // A later plugin cancellation must not spend a card during the pre-event.
        if (!ev.CanOpen || !ReferenceEquals(pending.Door, ev.Door.Base))
            return;
        if (!player.inventory.UserInventory.Items.TryGetValue(pending.Card.ItemSerial, out var owned) ||
            !ReferenceEquals(owned, pending.Card))
            return;

        pending.Card.PermissionsUsedCallback?.Invoke(pending.Door, true);
    }

    private void OnLeft(PlayerLeftEventArgs ev) => _pending.Remove(ev.Player.ReferenceHub);

    private sealed class PendingUse
    {
        public PendingUse(DoorVariant door, SingleUseKeycardItem card) { Door = door; Card = card; }
        public DoorVariant Door { get; }
        public SingleUseKeycardItem Card { get; }
    }
}
