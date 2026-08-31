using Forno.Contracts;

namespace Forno.Validation;

public static class SubscribeValidator
{
    public static Result Validate(string? email)
    {
        if (!EmailRules.IsValid(email))
        {
            return Result.Fail("email", "Zadajte platný e-mail.");
        }

        return Result.Ok();
    }
}
