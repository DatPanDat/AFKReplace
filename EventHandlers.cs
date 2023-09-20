using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp079;
using Exiled.Events.EventArgs.Scp914;

namespace AFKReplace.EventHandler
{
    public class EventHandlers
    {
        public AFKReplace plugin;

        public void OnPlayerVerified(VerifiedEventArgs ev)
        {
            // Add a component to the player to check AFK status.
            AFKComponent afkComponent = ev.Player.GameObject.gameObject.AddComponent<AFKComponent>();
            afkComponent.plugin = plugin;
        }

        public void OnSetClass(ChangingRoleEventArgs ev)
        {
            try
            {
                if (ev.Player == null) return;
                AFKComponent afkComponent = ev.Player.GameObject.gameObject.GetComponent<AFKComponent>();

                if (afkComponent != null)
                {
                    if (true)
                        if (ev.Player.IPAddress == "127.0.0.1") //127.0.0.1 is sometimes used for "Pets" which causes issues
                            afkComponent.disabled = true;
                }


            }
            catch (System.Exception e)
            {
                Log.Error($"AFK: ERROR In OnSetClass(): {e}");
            }
        }

        /*
		 * The following events are only here as additional AFK checks for some very basic player interactions
		 * I can add more interactions, but this seems good for now.
		 */
        public void OnDoorInteract(InteractingDoorEventArgs ev)
        {
            try
            {
                ResetAFKTime(ev.Player);
            }
            catch (System.Exception e)
            {
                Log.Error($"AFK: ERROR In OnDoorInteract(): {e}");
            }
        }

        public void OnPlayerShoot(ShootingEventArgs ev)
        {
            try
            {
                ResetAFKTime(ev.Player);
            }
            catch (System.Exception e)
            {
                Log.Error($"AFK: ERROR In ResetAFKTime(): {e}");
            }
        }
        public void On914Activate(ActivatingEventArgs ev)
        {
            try
            {
                ResetAFKTime(ev.Player);
            }
            catch (System.Exception e)
            {
                Log.Error($"AFK: ERROR In On914Activate(): {e}");
            }
        }
        public void On914Change(ChangingKnobSettingEventArgs ev)
        {
            try
            {
                ResetAFKTime(ev.Player);
            }
            catch (System.Exception e)
            {
                Log.Error($"AFK: ERROR In OnLockerInteract(): {e}");
            }
        }

        public void OnLockerInteract(InteractingLockerEventArgs ev)
        {
            try
            {
                ResetAFKTime(ev.Player);
            }
            catch (System.Exception e)
            {
                Log.Error($"AFK: ERROR In OnLockerInteract(): {e}");
            }
        }
        public void OnDropItem(DroppedItemEventArgs ev)
        {
            try
            {
                ResetAFKTime(ev.Player);
            }
            catch (System.Exception e)
            {
                Log.Error($"AFK: ERROR In OnDropItem(): {e}");
            }
        }
        public void OnDropAmmo(DroppedAmmoEventArgs ev)
        {
            try
            {
                ResetAFKTime(ev.Player);
            }
            catch (System.Exception e)
            {
                Log.Error($"AFK: ERROR In OnDropAmmo(): {e}");
            }
        }

        public void OnSCP079Exp(GainingExperienceEventArgs ev)
        {
            try
            {
                ResetAFKTime(ev.Player);
            }
            catch (System.Exception e)
            {
                Log.Error($"AFK: ERROR In OnSCP079Exp(): {e}");
            }
        }

        /// <summary>
        /// Reset the AFK time of a player.
        /// </summary>
        /// <param name="player"></param>
        public void ResetAFKTime(Player player)
        {
            try
            {
                if (player == null) return;

                AFKComponent afkComponent = player.GameObject.gameObject.GetComponent<AFKComponent>();

                if (afkComponent != null)
                    afkComponent.AFKTime = 0;

            }
            catch (System.Exception e)
            {
                Log.Error($"AFK: ERROR In ResetAFKTime(): {e}");
            }
        }
    }
}
