using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using Interactables.Interobjects.DoorUtils;

namespace KeycardInventoryBypass;

internal sealed class EventHandler
{
    private readonly Plugin _plugin;

    public EventHandler(Plugin plugin)
    {
        _plugin = plugin;
    }

    public void Register()
    {
        Exiled.Events.Handlers.Player.InteractingDoor += OnInteractingDoor;
    }

    public void Unregister()
    {
        Exiled.Events.Handlers.Player.InteractingDoor -= OnInteractingDoor;
    }

    private void OnInteractingDoor(InteractingDoorEventArgs ev)
    {
        LogDebug($"InteractingDoor: player={ev.Player?.Nickname ?? "null"} allowed={ev.IsAllowed} canInteract={ev.CanInteract} door={(ev.Door != null ? ev.Door.Type.ToString() : "null")}");

        if (ev.IsAllowed || !ev.CanInteract)
        {
            LogDebug("Skip: already allowed or cannot interact.");
            return;
        }

        if (ev.Player is null || ev.Door is null)
        {
            LogDebug("Skip: player or door is null.");
            return;
        }

        if (ev.Door.IsLocked)
        {
            LogDebug("Skip: door is locked.");
            return;
        }

        KeycardPermissions required = ev.Door.KeycardPermissions;
        DoorPermissionFlags rawRequired = ev.Door.RequiredPermissions;
        LogDebug($"Door perms: keycard={required} rawRequired={rawRequired}");

        if (ev.Door.Type == DoorType.GateA || ev.Door.Type == DoorType.GateB)
        {
            LogDebug("Override: Gate requires ExitGates.");
            required = KeycardPermissions.ExitGates;
        }

        if (required.HasFlag(KeycardPermissions.ScpOverride))
        {
            LogDebug("Override: removing ScpOverride from keycard requirements.");
            required &= ~KeycardPermissions.ScpOverride;
        }

        if (rawRequired.HasFlag(DoorPermissionFlags.ScpOverride))
        {
            LogDebug("Override: removing ScpOverride from raw requirements.");
            rawRequired &= ~DoorPermissionFlags.ScpOverride;
        }

        if (required == KeycardPermissions.None && rawRequired != DoorPermissionFlags.None)
        {
            LogDebug("Note: KeycardPermissions is None but RequiredPermissions is not None.");
        }

        if (required == KeycardPermissions.None && rawRequired == DoorPermissionFlags.None)
        {
            LogDebug("Skip: door requires no keycard permissions.");
            return;
        }

        foreach (var item in ev.Player.Items)
        {
            LogDebug($"Inventory item: {item.Type}");

            if (item is not Keycard keycard)
                continue;

            if (required != KeycardPermissions.None && (keycard.Permissions & required) == required)
            {
                LogDebug($"Granted by keycard: {keycard.Type} perms={keycard.Permissions}");
                ev.IsAllowed = true;
                return;
            }

            if (rawRequired == DoorPermissionFlags.None)
                continue;

            DoorPermissionFlags keycardRaw = (DoorPermissionFlags)keycard.Permissions;
            LogDebug($"Keycard perms detail: type={keycard.Type} perms={keycard.Permissions} raw={keycardRaw}");
            if ((keycardRaw & rawRequired) == rawRequired)
            {
                LogDebug($"Granted by keycard (rawRequired): {keycard.Type} perms={keycard.Permissions}");
                ev.IsAllowed = true;
                return;
            }
        }

        LogDebug("Denied: no matching keycard in inventory.");
    }

    private void LogDebug(string message)
    {
        if (_plugin.Config.Debug)
            Log.Debug($"[KeycardInventoryBypass] {message}");
    }
}
