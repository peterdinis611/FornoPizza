# Forno

Blazor e‑shop pre pizzeriu 800° Forno. Katalóg, košík, objednávka, Stripe a RabbitMQ lístky do kuchyne.

```bash
cd forno
dotnet run
```

Otvorte `http://localhost:5036`.

## RabbitMQ

Lokálny broker:

```bash
docker compose up -d
```

Management UI: `http://localhost:15672` (guest / guest).

V `appsettings.Development.json` je `RabbitMq:Enabled: true`. Produkčne nastavte:

```bash
dotnet user-secrets set "RabbitMq:Enabled" "true"
dotnet user-secrets set "RabbitMq:Host" "localhost"
dotnet user-secrets set "RabbitMq:UserName" "guest"
dotnet user-secrets set "RabbitMq:Password" "guest"
```

**Flow:** po prijatí objednávky (bez Stripe) alebo po zaplatení (Stripe) ide JSON lístok do exchange `forno.orders` → fronta `forno.kitchen`. Appka má aj built-in kitchen consumer (loguje lístky).

## Stripe

1. V [Stripe Dashboard](https://dashboard.stripe.com/test/apikeys) skopírujte test kľúče.
2. Nastavte user secrets:

```bash
dotnet user-secrets set "Stripe:SecretKey" "sk_test_..."
dotnet user-secrets set "Stripe:PublishableKey" "pk_test_..."
dotnet user-secrets set "Stripe:WebhookSecret" "whsec_..."
```

3. Lokálny webhook (voliteľné):

```bash
stripe listen --forward-to localhost:5036/api/stripe/webhook
```

## Čo je hotové

- pec (úvod), menu, detail pizze, košík
- checkout s výdajom / rozvozom, chipmi alergií
- Stripe Checkout (objednávka → platba → potvrdenie)
- RabbitMQ kitchen tickets (`order.placed` / `order.paid`)
- admin pece (`/admin`) — objednávky, stavy, list dňa, menu
- list dňa z DB (`/admin/day`)

## Admin

Otvorte `http://localhost:5036/admin`.

Voliteľný PIN:

```bash
dotnet user-secrets set "Admin:Pin" "tajne"
```

Stavy objednávky: čaká platbu → zaplatená/prijatá → v peci → hotová → vydaná (alebo zrušená).

Ďalšie veci (účty, notifikácie) neskôr.
