using System.Reflection;
using Exiled.API.Features;

namespace AFKReplace.API.Features.ExternalRole;

public abstract class ExternalRoleChecker
{
    public abstract void Init(Assembly assembly);
    public abstract bool IsRole(Player player);
    public abstract void SpawnRole(Player oldPlayer, Player newPlayer);
    protected bool PluginEnabled { get; set; }
    protected Assembly Assembly { get; set; }
}