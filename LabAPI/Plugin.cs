using System;
using LabApi.Events.Handlers;
using LabApi.Features.Console;
using LabApi.Loader.Features.Plugins;

namespace KeycardInventoryBypass.LabAPI;

public sealed class Plugin : Plugin<Config>
{
    public override string Name => "KeycardInventoryBypass";
    public override string Description => "Allows keycards in the inventory to open doors.";
    public override string Author => "Codex";
    public override Version Version { get; } = new Version(1, 1, 1);
    public override Version RequiredApiVersion { get; } = new Version(1, 1, 7);

    private DoorInteractionHandler? _handler;

    public override void Enable()
    {
        if (!Config.IsEnabled)
            return;

        _handler = new DoorInteractionHandler(this);
        _handler.Register();
        PlayerEvents.InteractingDoor += _handler.OnInteractingDoor;
        Logger.Info($"{Name} enabled.");
    }

    public override void Disable()
    {
        if (_handler is not null)
            PlayerEvents.InteractingDoor -= _handler.OnInteractingDoor;
        _handler?.Unregister();
        _handler = null;
        Logger.Info($"{Name} disabled.");
    }
}
