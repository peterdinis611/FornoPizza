using Forno.Domain;
using Forno.Models;

namespace Forno.Validation;

public static class CartRules
{
    public static int ClampQty(int quantity) =>
        Math.Clamp(quantity, OvenLimits.QtyMin, OvenLimits.QtyMax);

    public static bool IsQty(int quantity) =>
        quantity is >= OvenLimits.QtyMin and <= OvenLimits.QtyMax;

    public static IReadOnlyList<string> Extras(IEnumerable<string>? ids) =>
        OvenExtras.Normalize(ids);
}
