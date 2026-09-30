using System;
using Exiled.API.Features;

namespace KeycardInventoryBypass;

public sealed class Plugin : Plugin<Config>
{
    public override string Name => "KeycardInventoryBypass";
    public override string Author => "Codex";
    public override Version Version { get; } = new Version(1, 1, 1);
    public override Version RequiredExiledVersion { get; } = new Version(9, 6, 0);

    private EventHandler? _handler;

    public override void OnEnabled()
    {
        base.OnEnabled();
        _handler = new EventHandler(this);
        _handler.Register();
        Log.Info($"{Name} enabled.");
    }

    public override void OnDisabled()
    {
        _handler?.Unregister();
        _handler = null;
        Log.Info($"{Name} disabled.");
        base.OnDisabled();
    }
}
