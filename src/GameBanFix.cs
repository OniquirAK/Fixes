using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SwiftlyS2.Shared.Memory;
using SwiftlyS2.Shared.Natives;

namespace Fixes;

[StructLayout(LayoutKind.Sequential)]
public struct CGcBanInformation_t
{
    public uint Reason;
    public double Unknown;
    public double Expiration;
    public uint AccountId;
}

public partial class Fixes
{
    private delegate void CheckSteamBanDelegate();

    private IUnmanagedFunction<CheckSteamBanDelegate>? _CheckSteamBanDelegate;
    private nint addressGCBanInfo;
    private bool enableSteamBanFix = false;

    public void InitGameBanFixes()
    {
        _CheckSteamBanDelegate = Core.Memory.GetUnmanagedFunctionByAddress<CheckSteamBanDelegate>(
            Core.GameData.GetSignature("CheckSteamBan")
        );

        addressGCBanInfo = core.Memory.ResolveXrefAddress(
            core.GameData.GetSignature("CCSGameRules::m_mapGcBanInformation")
        );

        enableSteamBanFix = Config.CurrentValue.EnableSteamBanFix;
        Config.OnChange((v, _) =>
        {
            enableSteamBanFix = v.EnableSteamBanFix;
        });

        _CheckSteamBanDelegate.AddHook(next =>
        {
            unsafe
            {
                return () =>
                {
                    next()();

                    if (!enableSteamBanFix) return;

                    ref var gcBanInfoMap = ref Unsafe.AsRef<CUtlMap<uint, CGcBanInformation_t, uint>>((void*)addressGCBanInfo);

                    if (gcBanInfoMap.Count > 0)
                        gcBanInfoMap.RemoveAll();
                };
            }
        });
    }
}