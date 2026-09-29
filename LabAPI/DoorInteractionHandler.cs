using Interactables.Interobjects.DoorUtils;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Console;
using LabApi.Features.Enums;
using LabApi.Features.Wrappers;

namespace KeycardInventoryBypass.LabAPI;

internal sealed class DoorInteractionHandler
{
    private readonly Plugin _plugin;

    public DoorInteractionHandler(Plugin plugin) => _plugin = plugin;

    public void OnInteractingDoor(PlayerInteractingDoorEventArgs ev)
    {
        // IsAllowed cancels the whole interaction; CanOpen is the door access result.
        if (!ev.IsAllowed || ev.CanOpen || ev.Player is null || ev.Door is null)
            return;

        if (ev.Door.IsLocked)
            return;

        DoorPermissionFlags required = ev.Door.Permissions & ~DoorPermissionFlags.ScpOverride;
        if (ev.Door.DoorName is DoorName.EzGateA or DoorName.EzGateB)
            required = DoorPermissionFlags.ExitGates;

        if (required == DoorPermissionFlags.None)
            return;

        foreach (Item item in ev.Player.Items)
        {
            if (item is not KeycardItem keycard)
                continue;

            if ((keycard.Permissions & required) != required)
                continue;

            ev.CanOpen = true;
            Logger.Debug($"Inventory keycard {keycard.Type} opened {ev.Door.DoorName} for {ev.Player.Nickname}.", _plugin.Config.Debug);
            return;
        }

        Logger.Debug($"No inventory keycard can open {ev.Door.DoorName} for {ev.Player.Nickname}.", _plugin.Config.Debug);
    }
}
