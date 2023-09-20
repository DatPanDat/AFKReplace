using Exiled.API.Interfaces;
using PlayerRoles;
using System.Collections.Generic;
using System.ComponentModel;

namespace AFKReplace
{
    public sealed class Config : IConfig
    {
        [Description("If the plugin is enabled or not. (Make sure your disconnect_drop is set to false!)")]
        public bool IsEnabled { get; set; } = true;

        [Description("Enable debugging?")]
        public bool Debug { get; set; } = false;

        [Description("The roles to not attempt replacing/detecting as afk.")]
        public List<RoleTypeId> RestrictedRoles { get; set; } = new()
    {
        RoleTypeId.Spectator,
        RoleTypeId.Tutorial,
        RoleTypeId.Overwatch,
        RoleTypeId.Filmmaker,
    };
        [Description("Minimum required players for AFK replacing to be active.")]
        public int minPlayers { get; set; } = 2;

        [Description("How long can player not move?")]
        public int afkTime { get; private set; } = 120;

        [Description("How long to wait before player gets kicked after getting a warning for not moving?")]
        public int afkGraceTime { get; private set; } = 15;

        [Description("After how many changes to spectator for AFK should player get kicked? (Set to -1 to disable.)")]
        public int afkCountBeforeKick { get; private set; } = 2;

        [Description("The warning message for player who has been AFK before the action is taken.")]
        public string afkGraceMsg { get; private set; } = "<b><color=#ff3a3a>You will be moved to spectator in</color> <color=white>%TimeLeft% seconds</color><color=#ff3a3a> if you do not move!</b>";

        [Description("The message for player who have been moved to spectators.")]
        public string afkFSpecMsg { get; private set; } = "You were detected as AFK and has been moved to spectator!";

        [Description("The duration of the force spectator message above.")]
        public ushort afkFSpecMsgTime { get; set; } = 8;

        [Description("The kick reason if AFKBeforeKick is used.")]
        public string afkKickMsg { get; private set; } = "[Kicked by \"AFKReplace\" Plugin] You have been detected as AFK too many times!";

        [Description("The text displayed to the player after replacing.")]
        public string replacedMessage { get; set; } = "<i>You have replaced an AFK player.</i>";

        [Description("The duration of message above.")]
        public ushort replacedMessageTime { get; set; } = 5;

        [Description("The text displayed to the server if no replacement was found.")]
        public string noReplaceMessage { get; set; } = "<i>%oldPlayerName% with a role %oldRole% was afk, but no one has replaced them!</i>";

        [Description("The duration of no replacement message above.")]
        public ushort noReplaceMessageTime { get; set; } = 8;

        [Description("The death reason that will appear on the ragdoll if player was AFK and no replacement was found.")]
        public string deathReason { get; set; } = "AFK for too long.";

    }
}