using HarmonyLib;

namespace DaylightSavings;

internal static class FireplaceAccess
{
    internal static readonly AccessTools.FieldRef<Fireplace, ZNetView> Nview =
        AccessTools.FieldRefAccess<Fireplace, ZNetView>("m_nview");

    internal static ZNetView GetNview(Fireplace fireplace)
    {
        if (fireplace == null)
        {
            return null;
        }

        try
        {
            ZNetView nview = Nview(fireplace);
            if (nview != null)
            {
                return nview;
            }
        }
        catch
        {
            // Fall through to GetComponent.
        }

        return fireplace.GetComponent<ZNetView>();
    }

    internal static ZDO GetZdo(Fireplace fireplace)
    {
        ZNetView nview = GetNview(fireplace);
        if (nview == null || !nview.IsValid())
        {
            return null;
        }

        return nview.GetZDO();
    }

    internal static bool HasFuel(Fireplace fireplace)
    {
        if (fireplace == null)
        {
            return false;
        }

        if (fireplace.m_infiniteFuel)
        {
            return true;
        }

        ZDO zdo = GetZdo(fireplace);
        return zdo != null && zdo.GetFloat(ZDOVars.s_fuel) > 0f;
    }
}
