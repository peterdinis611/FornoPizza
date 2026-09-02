using Forno.Contracts;
using Forno.Domain;

namespace Forno.Validation;

public static class CheckoutValidator
{
    public static IReadOnlyList<FieldError> Validate(PlaceOrderRequest request)
    {
        var errors = new List<FieldError>();

        if (!NameRules.IsValid(request.Name))
        {
            errors.Add(new FieldError("name", "Zadajte celé meno (iba písmená, 2–80 znakov)."));
        }

        if (!EmailRules.IsValid(request.Email))
        {
            errors.Add(new FieldError("email", "Zadajte platný e-mail, napr. meno@email.sk."));
        }

        if (!PhoneRules.IsValid(request.Phone))
        {
            errors.Add(new FieldError("phone", "Zadajte slovenský mobil, napr. 0905 123 456."));
        }

        if (!Enum.IsDefined(request.Fulfillment))
        {
            errors.Add(new FieldError("fulfillment", "Vyberte výdaj pri peci alebo rozvoz."));
        }

        var isDelivery = request.Fulfillment == FulfillmentMode.Delivery;
        if (isDelivery)
        {
            if (!AddressRules.IsValid(request.Address))
            {
                errors.Add(new FieldError(
                    "address",
                    "Zadajte adresu s ulicou a číslom domu, napr. Hlavná 12."));
            }
        }
        else if (request.Fulfillment == FulfillmentMode.Pickup)
        {
            // pickup address is set server-side; ignore client garbage
        }

        var note = (request.Note ?? "").Trim();
        if (note.Length > OvenLimits.NoteMax)
        {
            errors.Add(new FieldError("note", $"Poznámka môže mať najviac {OvenLimits.NoteMax} znakov."));
        }

        if (request.Lines is null || request.Lines.Count == 0)
        {
            errors.Add(new FieldError("cart", "Košík je prázdny."));
            return errors;
        }

        if (request.Lines.Count > OvenLimits.CartLinesMax)
        {
            errors.Add(new FieldError("cart", $"V košíku môže byť najviac {OvenLimits.CartLinesMax} riadkov."));
        }

        foreach (var line in request.Lines)
        {
            if (line.Quantity < OvenLimits.QtyMin || line.Quantity > OvenLimits.QtyMax)
            {
                errors.Add(new FieldError(
                    "cart",
                    $"Každý list musí mať {OvenLimits.QtyMin}–{OvenLimits.QtyMax} ks."));
                break;
            }

            if (line.ExtraIds is not null && line.ExtraIds.Count > OvenLimits.ExtraMax)
            {
                errors.Add(new FieldError(
                    "cart",
                    $"Na jeden list najviac {OvenLimits.ExtraMax} príloh."));
                break;
            }
        }

        var total = CartRules.Total(request.Lines);
        if (total <= 0)
        {
            errors.Add(new FieldError("cart", "Suma objednávky nie je platná."));
        }

        if (isDelivery && total < OvenCommerce.ShipMinimum)
        {
            var gap = OvenCommerce.ShipMinimum - total;
            errors.Add(new FieldError(
                "fulfillment",
                $"Rozvoz je od {OvenCommerce.ShipMinimum:N0} €. Ešte {gap:N2} € alebo výdaj pri peci."));
        }

        return errors;
    }
}
