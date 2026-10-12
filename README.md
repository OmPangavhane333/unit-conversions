# Unit Conversion Web API (.NET 9)

A clean, extensible RESTful Web API built with **ASP.NET Core (.NET 9)** that converts numerical values between different units of measurement across **Length**, **Temperature**, **Weight / Mass**, **Volume**, **Area**, **Speed**, and **Time**.

---

## Project Structure

The solution uses a single, well-organized ASP.NET Core Web API project (`UnitConversion.Api`) with a clear separation of concerns by folder:

```text
├── UnitConversion.sln
├── README.md
└── UnitConversion.Api/
    ├── Controllers/
    │   └── UnitConversionController.cs   # Single REST API controller exposing conversion & discovery endpoints
    ├── DTOs/
    │   ├── ConvertRequestDto.cs          # Input payload for conversion requests
    │   ├── ConvertResponseDto.cs         # Output payload for completed conversions
    │   └── CategoryAndUnitDtos.cs        # DTOs for listing categories and units
    ├── Entities/
    │   ├── UnitCategory.cs               # Measurement category entity & base unit reference
    │   └── UnitDefinition.cs             # Unit definition entity with conversion factors/offsets
    ├── Interfaces/
    │   ├── IUnitCatalogService.cs        # Contract for retrieving categories and resolving units
    │   └── IUnitConversionService.cs     # Contract for conversion business logic
    ├── Models/
    │   ├── ConversionResult.cs           # Internal service result model
    │   └── ApiErrorResponse.cs           # Standardized API error model
    ├── Services/
    │   ├── UnitCatalogService.cs         # In-memory store and O(1) lookup for units and aliases
    │   └── UnitConversionService.cs      # Core conversion and validation logic
    └── Program.cs                        # Dependency Injection (DI) setup and middleware pipeline
```

---

## Supported Categories & Units

| Category | Canonical Base Unit | Supported Units & Examples of Accepted Aliases |
| :--- | :--- | :--- |
| **Length** (`length`) | `meter` (`m`) | `meter` (`m`, `meters`), `kilometer` (`km`), `centimeter` (`cm`), `millimeter` (`mm`), `mile` (`mi`), `yard` (`yd`), `foot` (`ft`, `feet`), `inch` (`in`, `inches`), `nautical-mile` (`nmi`) |
| **Temperature** (`temperature`) | `kelvin` (`K`) | `celsius` (`c`, `°C`, `celcius`), `fahrenheit` (`f`, `°F`), `kelvin` (`k`, `K`), `rankine` (`r`, `°R`) |
| **Weight / Mass** (`mass` / `weight`) | `kilogram` (`kg`) | `kilogram` (`kg`, `kgs`), `gram` (`g`, `grams`), `milligram` (`mg`), `metric-ton` (`t`, `tonne`), `pound` (`lb`, `lbs`, `pounds`), `ounce` (`oz`, `ounces`), `stone` (`st`) |
| **Volume** (`volume`) | `liter` (`L`) | `liter` (`l`, `L`, `liters`), `milliliter` (`ml`), `cubic-meter` (`m3`), `us-gallon` (`gal`, `gallon`), `us-quart` (`qt`), `us-pint` (`pt`), `us-cup` (`cup`), `us-fluid-ounce` (`fl oz`) |
| **Area** (`area`) | `square-meter` (`m²`) | `square-meter` (`m2`, `sqm`), `square-kilometer` (`km2`), `square-foot` (`ft2`, `sqft`), `acre` (`ac`), `hectare` (`ha`) |
| **Speed** (`speed`) | `meter-per-second` (`m/s`) | `meter-per-second` (`m/s`, `mps`), `kilometer-per-hour` (`km/h`, `kph`), `mile-per-hour` (`mph`), `knot` (`kn`, `kt`) |
| **Time** (`time`) | `second` (`s`) | `second` (`s`, `sec`), `millisecond` (`ms`), `minute` (`min`), `hour` (`h`, `hr`), `day` (`d`, `days`) |

---

## Prerequisites & Running Locally

### Prerequisites
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) (`9.0.x`)

### 1. Build the Solution
From the root directory of the repository:
```bash
dotnet build
```

### 2. Run the API
```bash
dotnet run --project UnitConversion.Api
```
The API will start and listen on `http://localhost:5171` (and `https://localhost:7145`).
In the `Development` environment, the OpenAPI specification is available at:
- `http://localhost:5171/openapi/v1.json`

---

## API Endpoints & Example Usage

### 1. Convert Units via `GET`
`GET /api/unitconversion/convert?value={value}&from={from}&to={to}&precision={precision}`

**Examples:**
```bash
# Convert 10 meters to feet
curl "http://localhost:5171/api/unitconversion/convert?value=10&from=meters&to=feet&precision=4"

# Convert 100 Celsius to Fahrenheit
curl "http://localhost:5171/api/unitconversion/convert?value=100&from=celsius&to=fahrenheit"

# Convert 5 kilograms to pounds
curl "http://localhost:5171/api/unitconversion/convert?value=5&from=kg&to=lbs"
```

**Sample Response (`200 OK`):**
```json
{
  "originalValue": 10,
  "fromUnit": "meter",
  "fromSymbol": "m",
  "toUnit": "foot",
  "toSymbol": "ft",
  "category": "length",
  "convertedValue": 32.8084,
  "exactValue": 32.808398950131233595800524934,
  "precision": 4,
  "formattedResult": "10 m = 32.8084 ft"
}
```

### 2. Convert Units via `POST`
`POST /api/unitconversion/convert`

```bash
curl -X POST "http://localhost:5171/api/unitconversion/convert" \
  -H "Content-Type: application/json" \
  -d "{\"value\": 5, \"from\": \"kilograms\", \"to\": \"pounds\", \"precision\": 4}"
```

### 3. List All Categories
`GET /api/unitconversion/categories`

```bash
curl "http://localhost:5171/api/unitconversion/categories"
```

### 4. List Supported Units (Optionally Filtered by Category)
`GET /api/unitconversion/units?category={category}`

```bash
curl "http://localhost:5171/api/unitconversion/units?category=temperature"
```

---

## Key Design Decisions & Trade-offs

1. **Canonical Base-Unit Pivot Pattern ($O(N)$ vs. $O(N^2)$ Scalability):**
   - Instead of hardcoding direct conversion formulas between every pair of units (which requires $N(N - 1)$ rules and becomes unmaintainable as the system grows to hundreds of units), each category defines a single **Canonical Base Unit** (e.g., `meter` for Length, `kelvin` for Temperature, `kilogram` for Weight/Mass).
   - Every unit only defines how to convert `ToBaseUnit` and `FromBaseUnit`. Converting between any two units in a category is a two-step pivot (`Source -> BaseUnit -> Target`). Adding a new unit requires adding just **one** `UnitDefinition` entry.

2. **Purely Data-Driven Affine Formula Model (Database-Ready):**
   - Rather than using C# lambda delegates (`Func<decimal, decimal>`) that cannot be serialized to a database, `UnitDefinition` uses numeric parameters (`Factor`, `Divisor`, `PreOffset`, `PostOffset`):
     $$\text{baseValue} = \frac{(\text{value} + \text{PreOffset}) \times \text{Factor}}{\text{Divisor}} + \text{PostOffset}$$
   - This handles both simple multiplicative conversions (length, weight, volume) and offset-based scales (Celsius, Fahrenheit, Kelvin, Rankine) using a single data structure that can be stored directly in SQL tables or JSON configuration files in the future.

3. **`decimal` Arithmetic & Rational Divisors for Financial/Scientific Precision:**
   - All calculations use C# `decimal` (128-bit high-precision decimal) alongside an explicit `Divisor` property (e.g., `Factor = 5, Divisor = 9` for Fahrenheit). This prevents IEEE 754 binary floating-point rounding artifacts (such as `100 °C` converting to `212.00000000000003 °F`).

4. **Alias & Symbol Resolution + Physical Validation:**
   - Callers can pass canonical codes (`meter`), plural names (`meters`), British spellings (`metres`), common typos (`celcius`), or symbols (`m`, `°C`, `lbs`).
   - Physical constraints such as **Absolute Zero** ($0\text{ K}$ / $-273.15\text{ °C}$ / $-459.67\text{ °F}$) are validated automatically against the category's `MinimumBaseValue`.
