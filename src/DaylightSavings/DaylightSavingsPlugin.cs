using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace DaylightSavings;

[BepInPlugin(ModConstants.ModGuid, ModConstants.ModName, ModConstants.ModVersion)]
public sealed class DaylightSavingsPlugin : BaseUnityPlugin
{
    internal static ManualLogSource Log { get; private set; }

    private Harmony _harmony;

    private void Awake()
    {
        Log = Logger;
        ModConfig.Bind(Config);

        _harmony = new Harmony(ModConstants.ModGuid);
        _harmony.PatchAll();

        Log.LogInfo($"{ModConstants.ModName} {ModConstants.ModVersion} loaded. Client-side: install on every joining player.");
    }

    private void OnDestroy()
    {
        _harmony?.UnpatchSelf();
    }
}
