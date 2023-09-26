using Exiled.API.Features;
using System;
using Exiled.API.Enums;
using Player = Exiled.Events.Handlers.Player;
using Scp914H = Exiled.Events.Handlers.Scp914;
using Scp079 = Exiled.Events.Handlers.Scp079;

using AFKReplace.EventHandler;
using Exiled.API.Interfaces;
using Exiled.Loader;


namespace AFKReplace
{

    public class AFKReplace : Plugin<Config>
    {
        public override PluginPriority Priority { get; } = PluginPriority.Medium;
        public override string Author { get; } = "Based from @Thomasjosif, \"\"\"Improved\"\"\" by @DatPanDat";
        public override string Name { get; } = "AFKReplace";
        public override string Prefix { get; } = "AFKReplace";
        public override Version Version { get; } = new Version(1, 0, 0);
        public override Version RequiredExiledVersion { get; } = new Version(8, 2, 1);

        public static AFKReplace Instance;
        public EventHandlers ev;

        public override void OnEnabled()
        {
            Instance = this;

            ev = new EventHandlers();

            foreach (IPlugin<IConfig> plugin in Loader.Plugins)
            {
                switch (plugin.Name)
                {
                    case "CiSpy" when plugin.Config.IsEnabled:
                        Log.Debug("CiSpy detected, enabling compatibility.");
                        API.API.CiSpyRole.Init(plugin.Assembly);
                        break;
                }
            }

            Player.Verified += ev.OnPlayerVerified;
            Player.ChangingRole += ev.OnSetClass;
            Player.Shooting += ev.OnPlayerShoot;
            Player.InteractingDoor += ev.OnDoorInteract;
            Scp914H.Activating += ev.On914Activate;
            Scp914H.ChangingKnobSetting += ev.On914Change;
            Player.InteractingLocker += ev.OnLockerInteract;
            Player.DroppedItem += ev.OnDropItem;
            Player.DroppedAmmo += ev.OnDropAmmo;
            Scp079.GainingExperience += ev.OnSCP079Exp;
        }

        public override void OnDisabled()
        {
            Instance = null;

            Player.Verified -= ev.OnPlayerVerified;
            Player.ChangingRole -= ev.OnSetClass;
            Player.Shooting -= ev.OnPlayerShoot;
            Player.InteractingDoor -= ev.OnDoorInteract;
            Scp914H.Activating -= ev.On914Activate;
            Scp914H.ChangingKnobSetting -= ev.On914Change;
            Player.InteractingLocker -= ev.OnLockerInteract;
            Player.DroppedItem -= ev.OnDropItem;
            Player.DroppedAmmo -= ev.OnDropAmmo;
            Scp079.GainingExperience -= ev.OnSCP079Exp;

            ev = null;
        }

    }
}
