using HarmonyLib;
using UnityEngine;

namespace DaylightSavings.Patches;

[HarmonyPatch(typeof(Fireplace))]
internal static class FireplacePatches
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(Fireplace.IsBurning))]
    [HarmonyPriority(Priority.Last)]
    private static void IsBurningPostfix(Fireplace __instance, ref bool __result)
    {
        if (!__result)
        {
            return;
        }

        if (PauseEvaluator.ShouldPause(__instance))
        {
            __result = false;
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch("UpdateFireplace")]
    private static void UpdateFireplacePrefix(Fireplace __instance)
    {
        ZNetView nview = FireplaceAccess.GetNview(__instance);
        if (nview == null || !nview.IsValid() || !nview.IsOwner())
        {
            return;
        }

        if (PauseEvaluator.ShouldPause(__instance))
        {
            PauseEvaluator.StampLastTime(__instance);
            return;
        }

        if (PauseEvaluator.LastUpdateIsStale(__instance, ModConstants.CatchUpMaxSeconds))
        {
            PauseEvaluator.StampLastTime(__instance);
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(Fireplace.CanBeRemoved))]
    private static void CanBeRemovedPostfix(Fireplace __instance, ref bool __result)
    {
        if (!__result)
        {
            return;
        }

        if (PauseEvaluator.ShouldPause(__instance) && FireplaceAccess.HasFuel(__instance))
        {
            __result = false;
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(Fireplace.Interact))]
    private static bool InteractPrefix(Fireplace __instance, Humanoid user, bool hold, bool alt, ref bool __result)
    {
        if (ModConfig.Enabled == null || !ModConfig.Enabled.Value || hold || !alt)
        {
            return true;
        }

        ZNetView nview = FireplaceAccess.GetNview(__instance);
        if (nview == null || !nview.IsValid())
        {
            return true;
        }

        if (FireplaceClassifier.Classify(__instance) == FireplaceKind.Ignored)
        {
            return true;
        }

        bool alwaysBurn = AlwaysBurnStore.ToggleAlwaysBurn(__instance);
        if (user != null)
        {
            string message = alwaysBurn
                ? "Daylight Savings: Always burn"
                : "Daylight Savings: Managed";
            user.Message(MessageHud.MessageType.Center, message);
        }

        __result = true;
        return false;
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(Fireplace.GetHoverText))]
    private static void GetHoverTextPostfix(Fireplace __instance, ref string __result)
    {
        if (ModConfig.Enabled == null || !ModConfig.Enabled.Value || string.IsNullOrEmpty(__result))
        {
            return;
        }

        if (FireplaceClassifier.Classify(__instance) == FireplaceKind.Ignored)
        {
            return;
        }

        bool alwaysBurn = AlwaysBurnStore.IsAlwaysBurn(__instance);
        PauseReason reason = alwaysBurn ? PauseReason.None : PauseEvaluator.Evaluate(__instance);

        string pauseLine = reason switch
        {
            PauseReason.NoPlayerNearby => "\n<color=grey>Paused (no one nearby)</color>",
            PauseReason.UsableDaylight => "\n<color=grey>Paused (daylight)</color>",
            _ => string.Empty,
        };

        string modeLine = alwaysBurn
            ? "\n<color=orange>Daylight Savings: Always burn</color>"
            : "\n<color=orange>Daylight Savings: Managed</color>";

        string altHint = alwaysBurn
            ? "\n[<color=yellow><b>$KEY_AltPlace + $KEY_Use</b></color>] Use Daylight Savings"
            : "\n[<color=yellow><b>$KEY_AltPlace + $KEY_Use</b></color>] Always burn";

        __result += Localization.instance.Localize(pauseLine + modeLine + altHint);
    }
}
