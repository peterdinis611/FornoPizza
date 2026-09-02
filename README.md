# Forno

Blazor e‑shop pre pizzeriu 800° Forno. Katalóg, košík, objednávka a platba kartou cez Stripe.

```bash
cd forno
dotnet run
```

Otvorte `http://localhost:5036`.

## Stripe

1. V [Stripe Dashboard](https://dashboard.stripe.com/test/apikeys) skopírujte test kľúče.
2. Nastavte user secrets (odporúčané pre vývoj):

```bash
dotnet user-secrets init
dotnet user-secrets set "Stripe:SecretKey" "sk_test_..."
dotnet user-secrets set "Stripe:PublishableKey" "pk_test_..."
dotnet user-secrets set "Stripe:WebhookSecret" "whsec_..."
```

Alternatíva: premenné prostredia `Stripe__SecretKey`, `Stripe__PublishableKey`, `Stripe__WebhookSecret`.

3. Lokálny webhook (voliteľné, pre spoľahlivé potvrdenie):

```bash
stripe listen --forward-to localhost:5036/api/stripe/webhook
```

Skopírujte `whsec_...` z výstupu do `Stripe:WebhookSecret`.

## Čo je hotové

- pec (úvod), menu, detail pizze, košík
- checkout s výdajom / rozvozom, chipmi alergií
- Stripe Checkout (objednávka → platba → potvrdenie)
- list dňa z DB (`/kiln/day`)

Ďalšie veci (kuchyňa, účty, admin objednávok) neskôr.
