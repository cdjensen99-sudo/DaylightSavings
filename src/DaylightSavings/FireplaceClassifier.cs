using System;
using System.Collections.Generic;

namespace DaylightSavings;

internal enum FireplaceKind
{
    Ignored,
    Fire,
    Light,
}

internal static class FireplaceClassifier
{
    private static readonly HashSet<string> DefaultFires = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "fire_pit",
        "fire_pit_iron",
        "bonfire",
        "hearth",
    };

    private static readonly HashSet<string> DefaultLights = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "piece_walltorch",
        "piece_groundtorch",
        "piece_groundtorch_wood",
        "piece_groundtorch_green",
        "piece_groundtorch_blue",
        "piece_groundtorch_mist",
        "piece_brazierfloor01",
        "piece_brazierfloor02",
        "piece_brazierceiling01",
        "piece_jackoturnip",
        "piece_Lavalantern",
    };

    private static readonly Dictionary<string, FireplaceKind> Cache =
        new Dictionary<string, FireplaceKind>(StringComparer.OrdinalIgnoreCase);

    internal static void Invalidate()
    {
        Cache.Clear();
    }

    internal static FireplaceKind Classify(Fireplace fireplace)
    {
        if (fireplace == null)
        {
            return FireplaceKind.Ignored;
        }

        string prefab = GetPrefabName(fireplace);
        if (string.IsNullOrEmpty(prefab))
        {
            return FireplaceKind.Ignored;
        }

        if (Cache.TryGetValue(prefab, out FireplaceKind cached))
        {
            return cached;
        }

        FireplaceKind kind = ClassifyPrefab(prefab, fireplace);
        Cache[prefab] = kind;
        return kind;
    }

    internal static string GetPrefabName(Fireplace fireplace)
    {
        if (fireplace == null || fireplace.gameObject == null)
        {
            return string.Empty;
        }

        try
        {
            return Utils.GetPrefabName(fireplace.gameObject);
        }
        catch
        {
            return Utils.GetPrefabName(fireplace.gameObject.name);
        }
    }

    private static FireplaceKind ClassifyPrefab(string prefab, Fireplace fireplace)
    {
        foreach (string excluded in ModConfig.ParseNames(ModConfig.ExcludePrefabs.Value))
        {
            if (string.Equals(excluded, prefab, StringComparison.OrdinalIgnoreCase))
            {
                return FireplaceKind.Ignored;
            }
        }

        foreach (string extraFire in ModConfig.ParseNames(ModConfig.ExtraFirePrefabs.Value))
        {
            if (string.Equals(extraFire, prefab, StringComparison.OrdinalIgnoreCase))
            {
                return FireplaceKind.Fire;
            }
        }

        foreach (string extraLight in ModConfig.ParseNames(ModConfig.ExtraLightPrefabs.Value))
        {
            if (string.Equals(extraLight, prefab, StringComparison.OrdinalIgnoreCase))
            {
                return FireplaceKind.Light;
            }
        }

        if (DefaultFires.Contains(prefab))
        {
            return FireplaceKind.Fire;
        }

        if (DefaultLights.Contains(prefab))
        {
            return FireplaceKind.Light;
        }

        return IsWoodFuel(fireplace) ? FireplaceKind.Fire : FireplaceKind.Light;
    }

    private static bool IsWoodFuel(Fireplace fireplace)
    {
        ItemDrop fuel = fireplace.m_fuelItem;
        if (fuel == null || fuel.m_itemData?.m_shared == null)
        {
            return false;
        }

        string name = fuel.m_itemData.m_shared.m_name;
        return string.Equals(name, "$item_wood", StringComparison.OrdinalIgnoreCase);
    }
}
