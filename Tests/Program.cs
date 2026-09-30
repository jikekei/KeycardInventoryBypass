using InventorySystem.Items.Keycards;
using Interactables.Interobjects.DoorUtils;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;

static class Program
{
    static void Check(bool value, string message) { if (!value) throw new Exception(message); }
    static int Main()
    {
        int count = 0;
        try {
            foreach (bool lab in new[] {false, true}) {
                Run(lab, "success consumes exactly once", f => { Check(f.Interact(), "pass must open"); Check(f.Pass.Uses == 0, "pre-event must not consume"); f.Complete(true); f.Complete(true); Check(f.Pass.Uses == 1, "consume exactly once"); Check(!f.Interact(), "spent pass cannot grant access again"); }); count++;
                Run(lab, "backpack pass cannot close", f => { f.Door.TargetState = true; Check(!f.Interact(), "close must fail"); f.Complete(false); Check(f.Pass.Uses == 0, "failed close must not consume"); }); count++;
                Run(lab, "held pass cannot close", f => { f.Hub.inventory.CurInstance = f.Pass; f.Door.TargetState = true; Check(!f.Interact(), "held pass must not bypass native rejection"); }); count++;
                Run(lab, "held success stays with game callback", f => { f.Hub.inventory.CurInstance = f.Pass; Check(f.Interact(alreadyAllowed:true), "native allow remains"); f.Pass.PermissionsUsedCallback!(f.Door,true); f.Complete(true); Check(f.Pass.Uses == 1,"must not double consume held card"); }); count++;
                Run(lab, "reusable card preferred", f => { f.Add(new KeycardItem {ItemSerial=2}); Check(f.Interact(),"regular card must open"); f.Complete(true); Check(f.Pass.Uses == 0,"do not waste pass"); }); count++;
                Run(lab, "reusable card can close", f => { f.Add(new KeycardItem {ItemSerial=2}); f.Door.TargetState=true; Check(f.Interact(),"regular card may close"); f.Complete(true); Check(f.Pass.Uses == 0,"pass must not be used for close"); }); count++;
                Run(lab, "later denial does not consume", f => { Check(f.Interact(),"initial grant"); f.Complete(false); Check(f.Pass.Uses==0,"later denial must not consume"); }); count++;
                Run(lab, "cancelled interaction rejected", f => { Check(!f.Interact(cancelled:true),"cancel stays cancelled"); f.Complete(false); Check(f.Pass.Uses==0,"no consumption"); }); count++;
                Run(lab, "locked door rejected", f => { f.Locked=true; Check(!f.Interact(),"locked gate must not bypass"); f.Complete(false); Check(f.Pass.Uses==0,"no consumption"); }); count++;
                Run(lab, "removed card not consumed", f => { Check(f.Interact(),"initial grant"); f.Hub.inventory.UserInventory.Items.Clear(); f.Complete(true); Check(f.Pass.Uses==0,"removed card callback must not run"); }); count++;
                Run(lab, "other player isolated", f => { Check(f.Interact(),"initial grant"); PlayerEvents.Complete(new(new(new ReferenceHub()),new(f.Door),true)); Check(f.Pass.Uses==0,"wrong player must not consume"); f.Complete(true); Check(f.Pass.Uses==1,"correct player consumes"); }); count++;
                Run(lab, "new interaction clears cancelled pending use", f => { Check(f.Interact(),"initial grant"); f.Door.TargetState=true; Check(!f.Interact(),"second attempt rejected"); f.Complete(true); Check(f.Pass.Uses==0,"stale cancelled attempt must not consume"); }); count++;
                Run(lab, "disconnect clears pending use", f => { Check(f.Interact(),"initial grant"); PlayerEvents.Leave(new(f.LabPlayer)); f.Complete(true); Check(f.Pass.Uses==0,"disconnect must clear pending use"); }); count++;
                Run(lab, "disable clears pending use", f => { Check(f.Interact(),"initial grant"); f.Dispose(); f.Complete(true); Check(f.Pass.Uses==0,"disabled plugin must clear pending use"); }); count++;
            }
            Console.WriteLine($"Passed {count} regressions against both production adapters and shared policy.");
            return 0;
        } catch(Exception ex) { Console.Error.WriteLine(ex); return 1; }
    }
    static void Run(bool lab, string label, Action<Fixture> test) { using var f = new Fixture(lab); test(f); Console.WriteLine($"PASS {(lab ? "LabAPI" : "EXILED")}: {label}"); }
    sealed class Fixture : IDisposable
    {
        readonly bool lab;
        readonly KeycardInventoryBypass.EventHandler? exiledHandler;
        readonly KeycardInventoryBypass.LabAPI.DoorInteractionHandler? labHandler;
        public readonly ReferenceHub Hub = new();
        public readonly DoorVariant Door = new();
        public readonly SingleUseKeycardItem Pass = new() {ItemSerial=1};
        public readonly LabApi.Features.Wrappers.Player LabPlayer;
        readonly Exiled.API.Features.Player exiledPlayer;
        public bool Locked;
        public Fixture(bool useLab) {
            lab=useLab; LabPlayer=new(Hub); exiledPlayer=new(Hub); Add(Pass);
            if(lab) {labHandler=new(new());labHandler.Register();} else {exiledHandler=new(new());exiledHandler.Register();}
        }
        public void Add(KeycardItem card) {Hub.inventory.UserInventory.Items.Add(card.ItemSerial,card);LabPlayer.Items.Add(new LabApi.Features.Wrappers.KeycardItem(card));exiledPlayer.Items.Add(new Exiled.API.Features.Items.Keycard(card));}
        public bool Interact(bool cancelled=false, bool alreadyAllowed=false) {
            if(lab) {var ev=new PlayerInteractingDoorEventArgs(LabPlayer,new(Door){IsLocked=Locked}) {IsAllowed=!cancelled,CanOpen=alreadyAllowed};labHandler!.OnInteractingDoor(ev);return ev.CanOpen;}
            var ex=new Exiled.Events.EventArgs.Player.InteractingDoorEventArgs(exiledPlayer,new(Door){IsLocked=Locked}) {CanInteract=!cancelled,IsAllowed=alreadyAllowed};Exiled.Events.Handlers.Player.Interact(ex);return ex.IsAllowed;
        }
        public void Complete(bool allowed) => PlayerEvents.Complete(new(LabPlayer,new(Door),allowed));
        public void Dispose() {labHandler?.Unregister();exiledHandler?.Unregister();}
    }
}
