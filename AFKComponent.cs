using System;
using System.Linq;
using UnityEngine;
using MEC;
using Exiled.API.Features;
using PlayerRoles;
using Exiled.API.Enums;
using Exiled.API.Features.Roles;
using System.Collections.Generic;
using CustomPlayerEffects;
using Exiled.API.Features.Items;

namespace AFKReplace
{
    public class AFKComponent : MonoBehaviour
    {
        public AFKReplace plugin;

        public bool disabled = false;

        Player ply;

        public Vector3 AFKLastPosition;
        public Quaternion AFKLastAngle;

        public int AFKTime = 0;
        public int AFKCount = 0;
        private float timer = 0.0f;

        // Do not change this delay. It will screw up the detection
        public float delay = 1.0f;

        // Expose replacing player for plugin support
        public Player PlayerToReplace;


        void Awake()
        {
            ply = Player.Get(gameObject);
        }

        void Update()
        {
            timer += Time.deltaTime;
            if (timer > delay)
            {
                timer = 0f;
                if (!this.disabled)
                {
                    try
                    {
                        AFKChecker();
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
            }
        }

        // Called every 1 second according to the player's Update function. This is way more efficient than the old way of doing a forloop for every player.
        // Also, since the gameObject for the player is deleted when they disconnect, we don't need to worry about cleaning any variables :) 
        private void AFKChecker()
        {
            bool isScp079 = (this.ply.Role.Type.ToString() == "Scp079");

            //Log.Info($"AFK Time: {this.AFKTime} AFK Count: {this.AFKCount}");
            if (AFKReplace.Instance.Config.RestrictedRoles.Contains(this.ply.Role) || isScp079 || Player.List.Count() <= AFKReplace.Instance.Config.minPlayers || !Round.InProgress) return;

            bool scp096TryNotToCry = false;

            // When SCP096 is in the state "TryNotToCry" he cannot move or it will cancel,
            // therefore, we don't want to AFK check 096 while he's in this state.
            if (this.ply.Role.Type.ToString() == "Scp096")
            {
                scp096TryNotToCry = this.ply.Role.As<Scp096Role>().TryNotToCryActive;
            }

            Vector3 CurrentPos = this.ply.Position;
            Quaternion CurrentAngle = this.ply.Rotation;

            if (CurrentPos != this.AFKLastPosition || CurrentAngle != this.AFKLastAngle || scp096TryNotToCry)
            {
                this.AFKLastPosition = CurrentPos;
                this.AFKLastAngle = CurrentAngle;
                this.AFKTime = 0;
                PlayerToReplace = null;
                return;
            }

            // The player hasn't moved past this point.
            this.AFKTime++;

            // If the player hasn't reached the time yet don't continue.
            if (this.AFKTime < AFKReplace.Instance.Config.afkTime) return;

            // Check if we're still in the "grace" period
            int secondsuntilspec = AFKReplace.Instance.Config.afkTime + AFKReplace.Instance.Config.afkGraceTime - this.AFKTime;
            if (secondsuntilspec > 0)
            {
                string warning = AFKReplace.Instance.Config.afkGraceMsg;
                warning = warning.Replace("%TimeLeft%", secondsuntilspec.ToString());

                this.ply.ClearBroadcasts();
                this.ply.Broadcast(1, warning);
                return;
            }

            // The player is AFK and action will be taken.
            Log.Info($"AFK: {this.ply.Nickname} ({this.ply.UserId}) was detected as AFK!");
            this.AFKTime = 0;

            // Let's make sure they are still alive before doing any replacement.
            if (this.ply.Role.Team == Team.Dead) return;

            if (true)//was a check if replace is true or not, but we replacing regardless lul
            {//also i just took these from eventhandlers tbh

                Vector3 pos = this.ply.Position;
                Quaternion rot = this.ply.Rotation;//get player pos and rotation

                IEnumerable<Item> items = this.ply.Items;//saving current inventory                
                Dictionary<ItemType, ushort> ammoAndAmount = this.ply.Ammo;//saving current ammo(is out here for when replacement not found)

                List<Player> specPlayers = new();
                Player PlayerToReplace = null;

                foreach (var player in Player.List)
                {
                    if (player.Role != RoleTypeId.Spectator)
                        continue;
                    specPlayers.Add(player);
                }

                if (specPlayers.Count == 0)
                {
                    Log.Debug("No spectators found...");
                }

                if (specPlayers.Count > 0) PlayerToReplace = specPlayers[UnityEngine.Random.Range(0, specPlayers.Count - 1)];

                if (PlayerToReplace != null)
                {
                    Log.Debug("AFK: Successfully gotten replacement...");

                    float health = this.ply.Health;
                    float ahealth = this.ply.ArtificialHealth;
                    float hshield = this.ply.HumeShield;//save current hp, ahp, hs

                    ushort ammo1 = this.ply.GetAmmo(AmmoType.Nato9);
                    ushort ammo2 = this.ply.GetAmmo(AmmoType.Nato556);
                    ushort ammo3 = this.ply.GetAmmo(AmmoType.Nato762);
                    ushort ammo4 = this.ply.GetAmmo(AmmoType.Ammo12Gauge);
                    ushort ammo5 = this.ply.GetAmmo(AmmoType.Ammo44Cal);//fuck you, wack ass ammo getting

                    IEnumerable<StatusEffectBase> effs = this.ply.ActiveEffects;//save all status effects

                    //declaring for scps
                    int Exp079 = 0;
                    float Ap079 = 0f, Vigor106 = 0f;
                    Exiled.API.Features.Camera Room079 = null;

                    if (this.ply.Role == RoleTypeId.Scp079)//Check 079 location xp and ap
                    {
                        Log.Debug("AFK: SCP-079 Detected");
                        Exp079 = this.ply.Role.As<Scp079Role>().Experience;
                        Ap079 = this.ply.Role.As<Scp079Role>().Energy;
                        Room079 = this.ply.Role.As<Scp079Role>().Camera;
                    }

                    if (this.ply.Role == RoleTypeId.Scp106)//check 106 vigor
                    {
                        Log.Debug("AFK: SCP-106 Detected");
                        Vigor106 = this.ply.Role.As<Scp106Role>().Vigor;
                    }

                    PlayerToReplace.Role.Set(this.ply.Role, RoleSpawnFlags.None);//spawn new player

                    //making afk specs
                    this.ply.ClearInventory(true);
                    this.ply.Role.Set(RoleTypeId.Spectator);
                    this.ply.Broadcast(AFKReplace.Instance.Config.afkFSpecMsgTime, AFKReplace.Instance.Config.afkFSpecMsg);

                    Timing.CallDelayed(0.3f, () =>
                    {
                        PlayerToReplace.Position = pos;
                        PlayerToReplace.Rotation = rot;//Position, rotation

                        PlayerToReplace.Health = health;
                        PlayerToReplace.ArtificialHealth = ahealth;
                        PlayerToReplace.HumeShield = hshield;//HP, AHP, HS

                        if (PlayerToReplace.Role == RoleTypeId.Scp079)//if 079, take them to correct room with right amount of xp and ap
                        {
                            PlayerToReplace.Role.As<Scp079Role>().Experience = Exp079;
                            PlayerToReplace.Role.As<Scp079Role>().Energy = Ap079;
                            PlayerToReplace.Role.As<Scp079Role>().Camera = Room079;
                        }

                        if (PlayerToReplace.Role == RoleTypeId.Scp106)//if 106, give the right amount of vigor
                        {
                            PlayerToReplace.Role.As<Scp106Role>().Vigor = Vigor106;
                        }



                        foreach (Item item in items)//Inventory giving
                        {
                            Log.Debug(item);

                            if (item is Armor == true)
                            {
                                PlayerToReplace.AddItem(item.Type);
                            }

                            else
                            {
                                PlayerToReplace.AddItem(item);
                            }
                        }

                        PlayerToReplace.SetAmmo(AmmoType.Nato9, ammo1);
                        PlayerToReplace.SetAmmo(AmmoType.Nato556, ammo2);
                        PlayerToReplace.SetAmmo(AmmoType.Nato762, ammo3);
                        PlayerToReplace.SetAmmo(AmmoType.Ammo12Gauge, ammo4);
                        PlayerToReplace.SetAmmo(AmmoType.Ammo44Cal, ammo5);//wack ass ammo giving

                        foreach (StatusEffectBase effect in effs)//Status effects giving
                        {
                            PlayerToReplace.EnableEffect(effect);
                        }

                        PlayerToReplace.Broadcast(AFKReplace.Instance.Config.replacedMessageTime, AFKReplace.Instance.Config.replacedMessage);//broadcast to the replacement
                        Log.Debug("AFK: Replacement completed successfully");


                    });

                }
                else
                {
                    Log.Debug("AFK: No replacement found...");

                    Ragdoll.CreateAndSpawn(this.ply.Role, this.ply.Nickname, AFKReplace.Instance.Config.deathReason, pos, rot);//spawn a corpse in their place
                    foreach (Item item in items) item.CreatePickup(pos, rot);//dropping the items they had
                    foreach (ItemType ammo in ammoAndAmount.Keys) Item.Create(ammo).CreatePickup(pos, rot);//dropping their ammos not working lmaoooooo
                    string FormatNonreplaceMessage = AFKReplace.Instance.Config.noReplaceMessage.Replace("%oldPlayerName%", this.ply.Nickname).Replace("%oldRole%", this.ply.Role.Type.ToString());//Format messages with dc player infos
                    Map.Broadcast(AFKReplace.Instance.Config.noReplaceMessageTime, FormatNonreplaceMessage);//broadcast to the entire server about it

                    //making afk specs, without replacement dis time
                    this.ply.ClearInventory(true);
                    this.ply.Role.Set(RoleTypeId.Spectator);
                    this.ply.Broadcast(AFKReplace.Instance.Config.afkFSpecMsgTime, AFKReplace.Instance.Config.afkFSpecMsg);
                }



                // If it's -1 we won't be kicking at all.
                if (AFKReplace.Instance.Config.afkCountBeforeKick != -1)
                {
                    // Increment AFK Count
                    this.AFKCount++;
                    if (this.AFKCount >= AFKReplace.Instance.Config.afkCountBeforeKick)
                    {
                        // Since this.AFKCount is greater than the config we're going to kick that player for being AFK too many times in one match.
                        ServerConsole.Disconnect(this.gameObject, AFKReplace.Instance.Config.afkKickMsg);
                    }
                }
            }

        }
    }
}
