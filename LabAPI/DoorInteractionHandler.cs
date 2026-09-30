using System.Linq;
using Interactables.Interobjects.DoorUtils;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Console;
using LabApi.Features.Enums;
using LabApi.Features.Wrappers;
using KeycardInventoryBypass.Shared;

namespace KeycardInventoryBypass.LabAPI;

internal sealed class DoorInteractionHandler
{
    private readonly Plugin _plugin;
    private readonly InventoryDoorAccess _access = new();

    public DoorInteractionHandler(Plugin plugin) => _plugin = plugin;
    public void Register() => _access.Register();
    public void Unregister() => _access.Unregister();

    public void OnInteractingDoor(PlayerInteractingDoorEventArgs ev)
    {
        if (ev.Player is null)
            return;
        _access.BeginInteraction(ev.Player.ReferenceHub);
        if (!ev.IsAllowed || ev.CanOpen || ev.Door is null || ev.Door.IsLocked)
            return;

        DoorPermissionFlags required = ev.Door.Permissions & ~DoorPermissionFlags.ScpOverride;
        if (ev.Door.DoorName is DoorName.EzGateA or DoorName.EzGateB)
            required = DoorPermissionFlags.ExitGates;
        if (required == DoorPermissionFlags.None)
            return;

        ev.CanOpen = _access.TryAuthorize(ev.Player.ReferenceHub, ev.Door.Base, required,
            ev.Player.Items.OfType<KeycardItem>().Select(card => card.Base));
        Logger.Debug($"Inventory access: allowed={ev.CanOpen} door={ev.Door.DoorName} player={ev.Player.Nickname}.", _plugin.Config.Debug);
    }
}
