using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;

namespace DaylightSavings;

internal static class ModConfig
{
    internal static ConfigEntry<bool> Enabled;
    internal static ConfigEntry<float> ProximityMeters;
    internal static ConfigEntry<bool> PauseFiresWhenNoPlayerNearby;
    internal static ConfigEntry<bool> PauseLightsWhenNoPlayerNearby;
    internal static ConfigEntry<bool> PauseLightsInUsableDaylight;
    internal static ConfigEntry<float> SealedCoverThreshold;
    internal static ConfigEntry<string> ExtraFirePrefabs;
    internal static ConfigEntry<string> ExtraLightPrefabs;
    internal static ConfigEntry<string> ExcludePrefabs;

    internal static void Bind(ConfigFile config)
    {
        Enabled = config.Bind(
            "General",
            "Enabled",
            true,
            "Enable DaylightSavings. When false, fires and lights behave as vanilla.");

        ProximityMeters = config.Bind(
            "General",
            "ProximityMeters",
            10f,
            new ConfigDescription(
                "Player range in meters for fires and for lights that are not in usable daylight.",
                new AcceptableValueRange<float>(1f, 50f)));

        PauseFiresWhenNoPlayerNearby = config.Bind(
            "Fires",
            "PauseFiresWhenNoPlayerNearby",
            true,
            "Pause campfires, hearths, and bonfires when no player is within ProximityMeters.");

        PauseLightsWhenNoPlayerNearby = config.Bind(
            "Lights",
            "PauseLightsWhenNoPlayerNearby",
            true,
            "Pause torches and other lights when no player is within ProximityMeters, including at night.");

        PauseLightsInUsableDaylight = config.Bind(
            "Lights",
            "PauseLightsInUsableDaylight",
            true,
            "Pause lights in windowed or open spaces during daylight, even if a player is standing there.");

        SealedCoverThreshold = config.Bind(
            "Lights",
            "SealedCoverThreshold",
            0.8f,
            new ConfigDescription(
                "Cover fraction that counts as a sealed pit (vanilla shelter is 0.8). Windowed halls stay below this.",
                new AcceptableValueRange<float>(0.4f, 1f)));

        ExtraFirePrefabs = config.Bind(
            "Classification",
            "ExtraFirePrefabs",
            "",
            "Comma-separated prefab names treated as fires (proximity only). Example: MyMod_Campfire");

        ExtraLightPrefabs = config.Bind(
            "Classification",
            "ExtraLightPrefabs",
            "",
            "Comma-separated prefab names treated as lights (proximity + daylight).");

        ExcludePrefabs = config.Bind(
            "Classification",
            "ExcludePrefabs",
            "",
            "Comma-separated prefab names DaylightSavings never pauses.");

        ExtraFirePrefabs.SettingChanged += (_, _) => FireplaceClassifier.Invalidate();
        ExtraLightPrefabs.SettingChanged += (_, _) => FireplaceClassifier.Invalidate();
        ExcludePrefabs.SettingChanged += (_, _) => FireplaceClassifier.Invalidate();
    }

    internal static IReadOnlyList<string> ParseNames(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return Array.Empty<string>();
        }

        return raw
            .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim())
            .Where(s => s.Length > 0)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}
