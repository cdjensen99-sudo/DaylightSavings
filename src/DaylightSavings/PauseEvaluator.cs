using System.Collections.Generic;
using UnityEngine;

namespace DaylightSavings;

internal enum PauseReason
{
    None,
    NoPlayerNearby,
    UsableDaylight,
}

internal static class PauseEvaluator
{
    private struct CacheEntry
    {
        public float Time;
        public PauseReason Reason;
    }

    private static readonly Dictionary<int, CacheEntry> Cache = new Dictionary<int, CacheEntry>();

    private static int SolidRayMask;

    internal static bool ShouldPause(Fireplace fireplace)
    {
        return Evaluate(fireplace) != PauseReason.None;
    }

    internal static PauseReason Evaluate(Fireplace fireplace)
    {
        if (fireplace == null || ModConfig.Enabled == null || !ModConfig.Enabled.Value)
        {
            return PauseReason.None;
        }

        if (fireplace.m_infiniteFuel || AlwaysBurnStore.IsAlwaysBurn(fireplace))
        {
            return PauseReason.None;
        }

        ZNetView nview = FireplaceAccess.GetNview(fireplace);
        if (nview == null || !nview.IsValid())
        {
            return PauseReason.None;
        }

        int id = fireplace.GetInstanceID();
        float now = Time.time;
        if (Cache.TryGetValue(id, out CacheEntry cached) && now - cached.Time < ModConstants.EvaluationCacheSeconds)
        {
            return cached.Reason;
        }

        PauseReason reason = EvaluateUncached(fireplace);
        Cache[id] = new CacheEntry { Time = now, Reason = reason };
        return reason;
    }

    internal static void StampLastTime(Fireplace fireplace)
    {
        ZDO zdo = FireplaceAccess.GetZdo(fireplace);
        if (zdo == null || ZNet.instance == null)
        {
            return;
        }

        zdo.Set(ZDOVars.s_lastTime, ZNet.instance.GetTime().Ticks);
    }

    internal static bool LastUpdateIsStale(Fireplace fireplace, float maxSeconds)
    {
        ZDO zdo = FireplaceAccess.GetZdo(fireplace);
        if (zdo == null || ZNet.instance == null)
        {
            return false;
        }

        long lastTicks = zdo.GetLong(ZDOVars.s_lastTime, 0L);
        if (lastTicks <= 0)
        {
            return false;
        }

        double elapsed = (ZNet.instance.GetTime() - new System.DateTime(lastTicks)).TotalSeconds;
        return elapsed > maxSeconds;
    }

    private static PauseReason EvaluateUncached(Fireplace fireplace)
    {
        FireplaceKind kind = FireplaceClassifier.Classify(fireplace);
        if (kind == FireplaceKind.Ignored)
        {
            return PauseReason.None;
        }

        Vector3 pos = fireplace.transform.position;
        bool playerNearby = Player.IsPlayerInRange(pos, ModConfig.ProximityMeters.Value);

        if (kind == FireplaceKind.Fire)
        {
            if (ModConfig.PauseFiresWhenNoPlayerNearby.Value && !playerNearby)
            {
                return PauseReason.NoPlayerNearby;
            }

            return PauseReason.None;
        }

        if (ModConfig.PauseLightsWhenNoPlayerNearby.Value && !playerNearby)
        {
            return PauseReason.NoPlayerNearby;
        }

        if (ModConfig.PauseLightsInUsableDaylight.Value && HasUsableDaylight(fireplace))
        {
            return PauseReason.UsableDaylight;
        }

        return PauseReason.None;
    }

    private static bool HasUsableDaylight(Fireplace fireplace)
    {
        if (EnvMan.instance == null || !EnvMan.IsDaylight())
        {
            return false;
        }

        Vector3 coverPoint = fireplace.transform.position + Vector3.up * fireplace.m_coverCheckOffset;
        Cover.GetCoverForPoint(coverPoint, out float coverPercentage, out bool _);

        bool sealedVolume = coverPercentage >= ModConfig.SealedCoverThreshold.Value;
        bool sunReaches = SunReachesSpace(fireplace);

        // Windowed halls: cover drops through openings and/or sun reaches a sample in the room.
        // Sealed pits: high cover and no sun — not usable daylight.
        return !sealedVolume || sunReaches;
    }

    private static bool SunReachesSpace(Fireplace fireplace)
    {
        Light sun = EnvMan.instance != null ? EnvMan.instance.m_dirLight : null;
        if (sun == null || !sun.isActiveAndEnabled || sun.intensity < ModConstants.MinSunIntensity)
        {
            return false;
        }

        EnsureRayMask();
        Vector3 toSun = -sun.transform.forward;
        Transform t = fireplace.transform;
        Vector3 origin = t.position + Vector3.up * ModConstants.SunSampleHeight;
        float inset = ModConstants.SunSampleInward;

        Vector3[] samples =
        {
            origin,
            origin + t.forward * inset,
            origin - t.forward * inset,
            origin + t.right * inset,
            origin - t.right * inset,
        };

        for (int i = 0; i < samples.Length; i++)
        {
            if (SunReachesPoint(samples[i], toSun))
            {
                return true;
            }
        }

        return false;
    }

    private static bool SunReachesPoint(Vector3 point, Vector3 toSun)
    {
        Vector3 start = point + toSun * 0.2f;
        // A hit means something sits between the sample and the sun.
        return !Physics.Raycast(start, toSun, ModConstants.SunRayDistance, SolidRayMask, QueryTriggerInteraction.Ignore);
    }

    private static void EnsureRayMask()
    {
        if (SolidRayMask == 0)
        {
            SolidRayMask = LayerMask.GetMask("Default", "static_solid", "Default_small", "piece", "terrain");
        }
    }
}
