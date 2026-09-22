namespace DaylightSavings;

internal static class AlwaysBurnStore
{
    internal static bool IsAlwaysBurn(Fireplace fireplace)
    {
        ZDO zdo = FireplaceAccess.GetZdo(fireplace);
        return zdo != null && zdo.GetBool(ModConstants.AlwaysBurnZdoKey);
    }

    internal static void SetAlwaysBurn(Fireplace fireplace, bool alwaysBurn)
    {
        ZNetView nview = FireplaceAccess.GetNview(fireplace);
        if (nview == null || !nview.IsValid())
        {
            return;
        }

        if (!nview.HasOwner())
        {
            nview.ClaimOwnership();
        }

        ZDO zdo = nview.GetZDO();
        if (zdo == null)
        {
            return;
        }

        zdo.Set(ModConstants.AlwaysBurnZdoKey, alwaysBurn);
    }

    internal static bool ToggleAlwaysBurn(Fireplace fireplace)
    {
        bool next = !IsAlwaysBurn(fireplace);
        SetAlwaysBurn(fireplace, next);
        return next;
    }
}
