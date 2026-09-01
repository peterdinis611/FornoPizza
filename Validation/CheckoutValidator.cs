using Forno.Contracts;
using Forno.Domain;

namespace Forno.Validation;

public static class CheckoutValidator
{
    public static IReadOnlyList<FieldError> Validate(PlaceOrderRequest request)
    {
        var errors = new List<FieldError>();

        var name = InputText.Name(request.Name);
        if (name.Length < OvenLimits.NameMin || !InputText.HasLetter(name))
        {
            errors.Add(new FieldError("name", "Zadajte meno (2–80 znakov)."));
        }

        if (!PhoneRules.IsValid(request.Phone))
        {
            errors.Add(new FieldError("phone", "Zadajte slovenský telefón, napr. 0905 123 456."));
        }

        var isDelivery = request.Fulfillment == FulfillmentMode.Delivery;
        if (isDelivery)
        {
            var address = InputText.Address(request.Address);
            if (address.Length < OvenLimits.AddressMin || !InputText.HasLetter(address))
            {
                errors.Add(new FieldError("address", "Zadajte adresu doručenia."));
            }
        }

        if ((request.Note ?? "").Trim().Length > OvenLimits.NoteMax)
        {
            errors.Add(new FieldError("note", "Poznámka môže mať najviac 240 znakov."));
        }

        if (request.Lines is null || request.Lines.Count == 0)
        {
            errors.Add(new FieldError("cart", "Košík je prázdny."));
            return errors;
        }

        if (request.Lines.Any(line => line.Quantity < OvenLimits.QtyMin))
        {
            errors.Add(new FieldError("cart", "Každý list musí mať aspoň jeden kus."));
        }

        var total = CartRules.Total(request.Lines);
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
