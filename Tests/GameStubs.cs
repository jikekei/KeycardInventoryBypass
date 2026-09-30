// Minimal game/framework contracts: tests link the production handlers and policy.
public sealed class ReferenceHub { public Inventory inventory = new(); }
public sealed class Inventory { public InventorySystem.Items.Keycards.KeycardItem? CurInstance; public UserInventory UserInventory = new(); }
public sealed class UserInventory { public Dictionary<ushort, InventorySystem.Items.Keycards.KeycardItem> Items = new(); }
namespace Interactables.Interobjects.DoorUtils {
    [Flags] public enum DoorPermissionFlags { None = 0, ExitGates = 1, ScpOverride = 2 }
    public sealed class DoorVariant { public bool TargetState; }
    public delegate void PermissionUsed(DoorVariant door, bool success);
}
namespace InventorySystem.Items.Keycards {
    using Interactables.Interobjects.DoorUtils;
    public class KeycardItem {
        public ushort ItemSerial; public int Uses; public bool Destroyed;
        public PermissionUsed? PermissionsUsedCallback;
        public KeycardItem() { PermissionsUsedCallback = (_, success) => { if (success) { Uses++; if (this is SingleUseKeycardItem) Destroyed = true; } }; }
        public virtual DoorPermissionFlags GetPermissions(DoorVariant door) => DoorPermissionFlags.ExitGates;
    }
    public sealed class SingleUseKeycardItem : KeycardItem {
        public override DoorPermissionFlags GetPermissions(DoorVariant door) => Destroyed || door.TargetState ? DoorPermissionFlags.None : DoorPermissionFlags.ExitGates;
    }
}
namespace LabApi.Features.Enums { public enum DoorName { EzGateA, EzGateB, Other } }
namespace LabApi.Features.Wrappers {
    using Interactables.Interobjects.DoorUtils;
    public class Item { }
    public sealed class KeycardItem : Item { public InventorySystem.Items.Keycards.KeycardItem Base; public KeycardItem(InventorySystem.Items.Keycards.KeycardItem card) => Base = card; }
    public sealed class Player { public ReferenceHub ReferenceHub; public string Nickname = "test"; public List<Item> Items = new(); public Player(ReferenceHub hub) => ReferenceHub = hub; }
    public sealed class Door { public DoorVariant Base; public bool IsLocked; public DoorPermissionFlags Permissions = DoorPermissionFlags.ExitGates; public Enums.DoorName DoorName = Enums.DoorName.EzGateA; public Door(DoorVariant door) => Base = door; }
}
namespace LabApi.Events.Arguments.PlayerEvents {
    using LabApi.Features.Wrappers;
    public sealed class PlayerInteractingDoorEventArgs { public Player Player; public Door Door; public bool IsAllowed = true; public bool CanOpen; public PlayerInteractingDoorEventArgs(Player p, Door d) {Player=p;Door=d;} }
    public sealed class PlayerInteractedDoorEventArgs { public Player Player; public Door Door; public bool CanOpen; public PlayerInteractedDoorEventArgs(Player p, Door d, bool result) {Player=p;Door=d;CanOpen=result;} }
    public sealed class PlayerLeftEventArgs { public Player Player; public PlayerLeftEventArgs(Player p) => Player=p; }
}
namespace LabApi.Events.Handlers {
    using LabApi.Events.Arguments.PlayerEvents;
    public static class PlayerEvents {
        public static event Action<PlayerInteractedDoorEventArgs>? InteractedDoor;
        public static event Action<PlayerLeftEventArgs>? Left;
        public static void Complete(PlayerInteractedDoorEventArgs args) => InteractedDoor?.Invoke(args);
        public static void Leave(PlayerLeftEventArgs args) => Left?.Invoke(args);
    }
}
namespace LabApi.Features.Console { public static class Logger { public static void Debug(string text, bool enabled) { } } }
namespace Exiled.API.Enums { public enum DoorType { GateA, GateB, Other } }
namespace Exiled.API.Features.Items {
    public class Item { }
    public sealed class Keycard : Item { public InventorySystem.Items.Keycards.KeycardItem Base; public Keycard(InventorySystem.Items.Keycards.KeycardItem card) => Base=card; }
}
namespace Exiled.API.Features {
    using Interactables.Interobjects.DoorUtils;
    public static class Log { public static void Debug(string text) { } }
    public sealed class Player { public ReferenceHub ReferenceHub; public List<Items.Item> Items = new(); public Player(ReferenceHub hub) => ReferenceHub=hub; }
    public sealed class Door { public DoorVariant Base; public bool IsLocked; public DoorPermissionFlags RequiredPermissions=DoorPermissionFlags.ExitGates; public Enums.DoorType Type=Enums.DoorType.GateA; public Door(DoorVariant door) => Base=door; }
}
namespace Exiled.Events.EventArgs.Player {
    public sealed class InteractingDoorEventArgs { public Exiled.API.Features.Player Player; public Exiled.API.Features.Door Door; public bool IsAllowed; public bool CanInteract=true; public InteractingDoorEventArgs(Exiled.API.Features.Player p, Exiled.API.Features.Door d) {Player=p;Door=d;} }
}
namespace Exiled.Events.Handlers {
    public static class Player { public static event Action<Exiled.Events.EventArgs.Player.InteractingDoorEventArgs>? InteractingDoor; public static void Interact(Exiled.Events.EventArgs.Player.InteractingDoorEventArgs args) => InteractingDoor?.Invoke(args); }
}
namespace KeycardInventoryBypass { public sealed class Config { public bool Debug; } public sealed class Plugin { public Config Config = new(); } }
namespace KeycardInventoryBypass.LabAPI { public sealed class Config { public bool Debug; } public sealed class Plugin { public Config Config = new(); } }
