# 🌤️ Weather Analytics Application

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat&logo=dotnet)
![React](https://img.shields.io/badge/React-19-61DAFB?style=flat&logo=react)
![TypeScript](https://img.shields.io/badge/TypeScript-5-3178C6?style=flat&logo=typescript)
![Vite](https://img.shields.io/badge/Vite-6-646CFF?style=flat&logo=vite)
![Tailwind CSS](https://img.shields.io/badge/Tailwind_CSS-4-06B6D4?style=flat&logo=tailwindcss)
![Auth0](https://img.shields.io/badge/Auth0-secured-EB5424?style=flat&logo=auth0)
![License](https://img.shields.io/badge/license-MIT-green)

A full-stack weather analytics platform that fetches live weather data for 10 cities, computes a custom **Comfort Index** score for each, and displays them ranked from most to least comfortable, with server-side caching and Auth0-based authentication, MFA, and whitelist-only access.

**Stack:** ASP.NET Core 8 (backend), React + TypeScript + Vite + Tailwind CSS (frontend), Auth0 (auth), OpenWeatherMap (weather data)

---

## 🛠️ Setup Instructions

### Prerequisites
- .NET 8 SDK
- Node.js (v18+) and npm
- An [OpenWeatherMap](https://openweathermap.org/api) API key (free tier)
- An [Auth0](https://auth0.com) account (free tier)

### ⚙️ Backend Setup

```bash
cd WeatherAnalytics.Api
dotnet restore

# Configure secrets (never committed to source control)
dotnet user-secrets init
dotnet user-secrets set "OpenWeatherMap:ApiKey" "your-openweathermap-api-key"
```

Update `appsettings.json` with your Auth0 tenant details:
```json
{
  "Auth0": {
    "Domain": "your-tenant.us.auth0.com",
    "Audience": "https://your-api-identifier"
  }
}
```

Run the backend:
```bash
dotnet run
```
Backend runs at `http://localhost:5078`.

### 🎨 Frontend Setup

```bash
cd weather-analytics-client
npm install
```

Create a `.env` file in `weather-analytics-client/`:
```
VITE_API_BASE_URL=http://localhost:5078/api
VITE_AUTH0_DOMAIN=your-tenant.us.auth0.com
VITE_AUTH0_CLIENT_ID=your-auth0-spa-client-id
VITE_AUTH0_AUDIENCE=https://your-api-identifier
```

Run the frontend:
```bash
npm run dev
```
Frontend runs at `http://localhost:5173`.

### 🔐 Auth0 Configuration Required
- Create a Single Page Application in Auth0, with Allowed Callback/Logout/Web Origin URLs set to `http://localhost:5173`
- Create an API in Auth0 with a unique Identifier (used as the Audience above)
- Disable public signups on the default database connection
- Manually create the test user (`careers@fidenz.com`)
- Enable Email as an MFA factor, set policy to "Always Required"

### ✅ Running Tests
```bash
dotnet test
```

---

## 🧮 Comfort Index Formula

The Comfort Index is a weighted composite score (0 to 100) built from six weather parameters, combining one peer-reviewed formula with several self-designed scoring functions.

| Factor | Weight | Basis |
|---|---|---|
| 🌡️ Temperature + Humidity (Thom's Discomfort Index) | 60% | Cited, Thom 1959 |
| 💨 Wind Speed | 15% | Physics-grounded (wind chill interaction) |
| 💧 Dew Point | 10% | Derived (Magnus-Tetens approximation), NOAA comfort bands |
| ☁️ Cloud Cover | 5% | Self-designed |
| 🔽 Pressure | 5% | Self-designed |
| 👁️ Visibility | 5% | Self-designed |

### 1. 🌡️ Temperature + Humidity, Thom's Discomfort Index (1959)

```
DI = T - 0.55 × (1 - 0.01×RH) × (T - 14.5)
score_DI = max(0, 100 - |DI - 21| × 6)
```

Thom's Discomfort Index is a well-established formula combining temperature and relative humidity into a single measure of perceived discomfort. A DI around 21 is generally considered the human comfort optimum; the score falls off linearly as DI moves away from that point in either direction.

**Why 60% weight:** temperature and humidity together are the dominant drivers of how "comfortable" weather feels, reflected in virtually every established comfort/heat-index model, so it earns the majority weight here.

### 2. 💨 Wind Speed, with wind-chill interaction

```
if windSpeed ≤ 5 m/s:  score_wind = 100
else:
    excess = windSpeed - 5
    penalty = excess × 8
    if temp < 15°C:
        penalty += excess × (15 - temp) × 0.5   // wind chill interaction
    score_wind = max(0, 100 - penalty)
```

Wind isn't scored in isolation, its penalty depends on temperature. Below 15°C, wind is penalized more heavily to reflect the real physiological wind-chill effect (wind makes cold weather feel significantly colder, while the same wind speed on a warm day may even feel refreshing).

**Why 15% weight:** wind is a secondary but meaningful comfort factor, and giving it a temperature-dependent penalty (rather than a flat range check) captures a real interaction effect rather than treating parameters independently.

### 3. 💧 Dew Point, derived via Magnus-Tetens approximation

OpenWeatherMap's current-weather endpoint doesn't return dew point directly, so it's derived from temperature and humidity:

```
α = ln(RH/100) + (17.27×T)/(237.3+T)
dewPoint = (237.3 × α) / (17.27 - α)

score_dew = 100                          if dewPoint ≤ 15°C
          = 100 - (dewPoint - 15) × 5    if dewPoint > 15°C (floored at 0)
```

Scoring bands follow NOAA's general dew-point comfort guidance (≤15°C comfortable, higher values increasingly "sticky" or "oppressive").

**Why 10% weight:** dew point is a more direct measure of moisture-driven discomfort than relative humidity alone, but since it's derived from the same underlying temp/humidity data already weighted heavily in the Discomfort Index, it's kept as a secondary rather than primary factor to avoid excessive double-counting of the same signal.

### 4 to 6. ☁️ Cloud Cover, 🔽 Pressure, 👁️ Visibility, self-designed ideal-band scoring

Each uses the same pattern, a scoring band around an ideal midpoint, with a linear penalty for distance outside it:

```
distance = max(0, |value - idealMid| - halfWidth)
score = max(0, 100 - distance × decayRate)
```

- **Cloud Cover** (ideal: 20 to 60%): partial cloud cover is comfortable (some shade without full overcast); weighted low (5%) since it affects ambience/glare more than physical comfort
- **Pressure** (ideal: 1010 to 1020 hPa): associated with stable, fair weather; weighted low (5%) since it's a weak, indirect proxy
- **Visibility** (ideal: 8000m or more): proxy for atmospheric clarity/air quality; weighted low (5%) as a secondary factor

These three are explicitly the weakest, least physiologically direct factors in the formula, their low weights are an honest reflection of that, rather than being padded to appear more sophisticated.

---

## 🎬 Live Extension (Screen Recording)

The wind-scoring function ships in this repository already including the wind-chill interaction described above. During the required screen recording, this interaction is added live to demonstrate the development process, starting from a simpler static-range wind score and modifying it in real time to incorporate temperature-dependent wind chill, then confirming city rankings shift accordingly for cold, windy cities.

> Screen Recording: `will be add shortly`

---

## ⚡ Caching Design

Two-tier server-side caching using `IMemoryCache`:

1. **Raw weather cache**: each city's raw OpenWeatherMap response is cached individually (keyed by city code) with a 5-minute TTL. This avoids re-fetching from the external API on every request.
2. **Processed/ranked results cache**: the final computed and sorted comfort rankings are cached separately (also 5-minute TTL). If this cache is warm, the entire ranking request short-circuits before touching per-city weather data at all, the fastest possible path.

A debug endpoint (`GET /api/cache/status`) reports the most recent HIT/MISS outcome per city code, tracked via an in-memory dictionary alongside `IMemoryCache` (since `IMemoryCache` itself doesn't expose hit/miss state to external callers).

**Trade-off:** because the ranked-results cache is checked first, the debug endpoint's per-city status only updates when the ranked cache is cold, this is expected two-tier caching behavior, not a bug, but worth understanding when testing the debug endpoint.

---

## ⚠️ Known Limitations

- **City count**: the assignment requires a minimum of 10 cities; this implementation uses exactly 10. Scaling to more cities would benefit from a queue-based or batched fetching strategy rather than a single `Task.WhenAll` burst, to stay within OpenWeatherMap's rate limits on the free tier.
- **Dew point accuracy**: the Magnus-Tetens approximation is accurate to roughly ±0.4°C, sufficient for comfort scoring, but not meteorological-grade precision.
- **Cache is in-memory and per-instance**: `IMemoryCache` doesn't persist across app restarts or scale across multiple server instances. A production deployment with multiple instances would need a distributed cache (e.g. Redis) instead.
- **No precipitation-specific penalty**: the current formula doesn't explicitly penalize active rain/storm conditions beyond their effect on humidity, cloud cover, and pressure, a city with rain but otherwise moderate temp/humidity could score reasonably well despite being actively wet.
- **Free-tier API key**: OpenWeatherMap's free tier has rate limits; heavy concurrent use (e.g. many simultaneous users triggering cache misses at once) could hit those limits.

---

## 📁 Project Structure

```
WeatherAnalytics.Api/          # ASP.NET Core backend
├── Controllers/                # API endpoints
├── Models/                     # DTOs and data models
├── Services/                   # Business logic (weather fetching, caching, comfort calculation, ranking)
└── Data/cities.json            # City list

WeatherAnalytics.Api.Tests/     # xUnit unit tests (Comfort Index calculator)

weather-analytics-client/       # React + TypeScript + Vite frontend
└── src/
    ├── auth/                   # Auth0 provider configuration
    ├── components/             # UI components
    ├── hooks/                  # Data-fetching hooks
    ├── pages/                  # Page-level components
    └── types/                  # TypeScript interfaces
```

## ✨ Bonus Features Implemented

- 🌙 Dark mode toggle
- 🔍 Frontend sorting and filtering
- 📈 24-hour temperature trend chart per city
- ✅ Unit tests for the Comfort Index calculator