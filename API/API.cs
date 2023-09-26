using AFKReplace.API.Features.ExternalRole;
using AFKReplace.API.Features.ExternalRole.Enums;
using Exiled.API.Features;

namespace AFKReplace.API;

public class API
{
    public static void ToggleAFKReplace(bool enable, string pluginName = null)
    {
        if (pluginName != null)
            Log.Debug($"{pluginName} is trying to toggle AFKReplace to {enable}.");
        else
            Log.Debug("A plugin is trying to toggle AFKReplace to {enable}.");
        
        AFKReplace.Instance.ev.IsDisabled = !enable;
        
        if (pluginName != null)
            Log.Debug($"{pluginName} has toggled AFKReplace to {enable}.");
        else
            Log.Debug("A plugin has toggled AFKReplace to {enable}.");
    }
    
    public static bool IsExternalRole(Player player)
    {
        if (CiSpyRole.IsRole(player))
            return true;
        
        return false;
    }
    
    public static ExternalRoleType GetExternalRole(Player player)
    {
        if (CiSpyRole.IsRole(player))
            return ExternalRoleType.CiSpy;
        
        return ExternalRoleType.None;
    }
    
    internal static readonly ExternalRoleChecker CiSpyRole = new CiSpyRole();
}