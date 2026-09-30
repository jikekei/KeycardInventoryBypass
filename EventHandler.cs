using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using Interactables.Interobjects.DoorUtils;
using KeycardInventoryBypass.Shared;

namespace KeycardInventoryBypass;

internal sealed class EventHandler
{
    private readonly Plugin _plugin;
    private readonly InventoryDoorAccess _access = new();

    public EventHandler(Plugin plugin) => _plugin = plugin;

    public void Register()
    {
        Exiled.Events.Handlers.Player.InteractingDoor += OnInteractingDoor;
        _access.Register();
    }

    public void Unregister()
    {
        Exiled.Events.Handlers.Player.InteractingDoor -= OnInteractingDoor;
        _access.Unregister();
    }

    private void OnInteractingDoor(InteractingDoorEventArgs ev)
    {
        if (ev.Player is null)
            return;
        _access.BeginInteraction(ev.Player.ReferenceHub);

        if (ev.IsAllowed || !ev.CanInteract || ev.Door is null || ev.Door.IsLocked)
            return;

        DoorPermissionFlags required = ev.Door.RequiredPermissions & ~DoorPermissionFlags.ScpOverride;
        if (ev.Door.Type is DoorType.GateA or DoorType.GateB)
            required = DoorPermissionFlags.ExitGates;
        if (required == DoorPermissionFlags.None)
            return;

        ev.IsAllowed = _access.TryAuthorize(ev.Player.ReferenceHub, ev.Door.Base, required,
            ev.Player.Items.OfType<Keycard>().Select(card => card.Base));
        if (_plugin.Config.Debug)
            Log.Debug($"[KeycardInventoryBypass] Inventory access: allowed={ev.IsAllowed} door={ev.Door.Type}");
    }
}
