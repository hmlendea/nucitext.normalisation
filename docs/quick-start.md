# Quick start

## Installation

### .NET CLI

```bash
dotnet add package NuciText.Normalisation
```

### Package Manager Console

```powershell
Install-Package NuciText.Normalisation
```

### PackageReference (csproj)

```xml
<PackageReference Include="NuciText.Normalisation" Version="1.0.1" />
```

## Basic usage

```csharp
using NuciText.Normalisation;

// Create normaliser (stateless, thread-safe, reusable)
INuciTextNormaliser normaliser = new NuciTextNormaliser();

// Normalise a sentence
string input = "   hello   world!   ";
string result = normaliser.NormaliseSentence(input);
// result = "Hello world!"

// Handle null/whitespace
string? nullable = normaliser.NormaliseSentence(null);      // null
string? empty = normaliser.NormaliseSentence("");           // null
string? whitespace = normaliser.NormaliseSentence("   ");   // null

// Safe usage with null-coalescing
string safe = normaliser.NormaliseSentence(userInput) ?? "";
```

## Dependency injection

```csharp
// Program.cs / Startup.cs
services.AddSingleton<INuciTextNormaliser, NuciTextNormaliser>();

// Consumer
public class MyService
{
    private readonly INuciTextNormaliser _normaliser;

    public MyService(INuciTextNormaliser normaliser)
    {
        _normaliser = normaliser;
    }

    public string Process(string input) => _normaliser.NormaliseSentence(input) ?? "";
}
```

## Expected transformations

| Input | Output |
|-------|--------|
| `"hello world"` | `"Hello world."` |
| `"  multiple   spaces  "` | `"Multiple spaces."` |
| `"no period"` | `"No period."` |
| `"already. good!"` | `"Already. Good!"` |
| `"space before comma , fixed"` | `"Space before comma, fixed."` |
| `"no space after comma,added"` | `"No space after comma, added."` |
| `"version 1.2.3 preserved"` | `"Version 1.2.3 preserved."` |

## Null handling

```csharp
// Returns null for null/whitespace
string? result = normaliser.NormaliseSentence(null); // null

// Use null-coalescing for non-null string
string neverNull = normaliser.NormaliseSentence(input) ?? "";

// Or check explicitly
if (normaliser.NormaliseSentence(input) is { } normalised)
{
    // use normalised
}
```

## Thread safety

The normaliser is stateless and thread-safe. A single instance can be shared across threads:

```csharp
// Singleton registration is safe
services.AddSingleton<INuciTextNormaliser, NuciTextNormaliser>();

// Concurrent calls are safe
Parallel.ForEach(inputs, input => 
{
    var result = normaliser.NormaliseSentence(input);
});
```

## Requirements

- .NET 10.0 or compatible runtime
- No additional dependencies

## Next steps

- Read [behaviour/normalise-sentence.md](./behaviour/normalise-sentence.md) for detailed behaviour
- Read [flows/normalise-sentence.md](./flows/normalise-sentence.md) for execution flow
- Read [components/nuci-text-normaliser.md](./components/nuci-text-normaliser.md) for implementation details
- Run tests: `dotnet test`