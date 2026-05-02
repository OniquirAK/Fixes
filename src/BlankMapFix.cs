using SwiftlyS2.Shared.GameEventDefinitions;
using SwiftlyS2.Shared.GameEvents;
using SwiftlyS2.Shared.Misc;

namespace Fixes;

public partial class Fixes
{
    [GameEventHandler(HookMode.Pre)]
    public HookResult OnNextLevelChangedEvent(EventNextlevelChanged @event)
    {
        if (!Config.CurrentValue.EnableBlankMapFix) return HookResult.Continue;

        if (@event.NextLevel == "")
        {
            var mapName = Core.Engine.GlobalVars.MapName;

            if (Core.Engine.IsMapValid(mapName))
            {
                Core.Engine.ExecuteCommand($"changelevel {mapName}");
            }
            else
            {
                Core.Engine.ExecuteCommand($"ds_workshop_changelevel {mapName}");
            }
        }

        return HookResult.Continue;
    }
}