# Testing

## Test strategy

| Aspect | Approach |
|--------|----------|
| **Framework** | NUnit 4.5.0 with NUnit3TestAdapter 6.1.0 |
| **Test runner** | `dotnet test` via Microsoft.NET.Test.Sdk 18.3.0 |
| **Organisation** | Single test class per type under test |
| **Naming** | `Given{Context}_When{Action}_Then{Outcome}` |
| **Parameterisation** | `TestCase` attributes for data-driven tests |
| **Mocking** | Moq 4.20.72 available (not used) |
| **Coverage** | Not currently measured |

## Test project structure

```
NuciText.Normalisation.UnitTests/
├── NuciTextNormaliserTests.cs
└── NuciText.Normalisation.UnitTests.csproj
```

## Test class: `NuciTextNormaliserTests`

| Method | Purpose | Cases |
|--------|---------|-------|
| `Setup()` | Instantiate `NuciTextNormaliser` | — |
| `GivenANullOrWhitespaceString_WhenNormalisingSentence_ThenTheResultIsEmpty` | Verify null/whitespace → null | 3: `null`, `""`, `"   "` |
| `GivenAValidString_WhenNormalisingSentence_ThenTheResultIsNotNull` | Verify valid input → non-null | 2 |
| `GivenAValidString_WhenNormalisingSentence_ThenTheResultIsNotEmpty` | Verify valid input → non-empty | 2 |
| `GivenAValidString_WhenNormalisingSentence_ThenTheResultHasBeenNormaliseSentenced` | Verify full normalisation pipeline | 7 parameterised |

## Test case catalogue

### Null/whitespace handling

| Input | Expected | Test |
|-------|----------|------|
| `null` | `null` | `TestCase(null)` |
| `""` | `null` | `TestCase("")` |
| `"   "` | `null` | `TestCase("   ")` |

### Basic validity

| Input | Expected | Test |
|-------|----------|------|
| `"   Hello   world!   "` | Not null, not empty | 2 `TestCase` |
| `"This is a test.  "` | Not null, not empty | 2 `TestCase` |

### Full normalisation pipeline

| Input | Expected output | Pipeline stages exercised |
|-------|-----------------|---------------------------|
| `"   Hello   world!   "` | `"Hello world!"` | Trim, collapse spaces, punctuation spacing, capitalise, terminal punctuation |
| `"Should we add a period here?"` | `"Should we add a period here?"` | Already correct; terminal punctuation preserved |
| `"Test sentence is testy"` | `"Test sentence is testy."` | Append period |
| `"This is a test.  "` | `"This is a test."` | Trim, terminal punctuation |
| `"this one should be capitalised. yes it should!"` | `"This one should be capitalised. Yes it should!"` | Capitalise after `.` |
| `"Package names such as NuciText.Normalisation or nucitext.normalisation should not have spaces after punctuation."` | Same | Version number protection (no space after `.` before digit) |
| `"Punctuation not added after version numbers such as 1.2.3 and others."` | Same | Version number protection |

## Coverage analysis

### Covered behaviours

| Behaviour | Test coverage |
|-----------|---------------|
| Null input → null | ✅ 3 cases |
| Empty string → null | ✅ |
| Whitespace → null | ✅ |
| Trim leading/trailing | ✅ (implicit in all valid cases) |
| Collapse repeated spaces | ✅ (`"   Hello   world!   "`) |
| Remove space before punctuation | ⚠️ Not explicitly tested (no test case with space before comma) |
| Add space after punctuation | ✅ (`"hello,world"` not directly tested but implied) |
| Capitalise first letter | ✅ (`"this one..."` → `"This one..."`) |
| Capitalise after sentence end | ✅ (`"...capitalised. yes..."` → `"...capitalised. Yes..."`) |
| Append period if missing | ✅ (`"Test sentence is testy"` → `"...testy."`) |
| Preserve existing terminal punctuation | ✅ (`"...here?"` unchanged) |
| Version number protection | ✅ 2 cases (`NuciText.Normalisation`, `1.2.3`) |

### Gaps

| Gap | Risk | Recommendation |
|-----|------|----------------|
| Space before punctuation removal | Medium | Add test: `"hello , world"` → `"hello, world"` |
| Space after punctuation insertion | Medium | Add test: `"hello,world"` → `"hello, world"` |
| Multiple consecutive punctuation | Low | Add test: `"hello!! world"` |
| Ellipsis handling | Low | Add test: `"hello..."` |
| Non-ASCII punctuation | Low | Add test: `"hello。world"` (ideographic full stop) |
| Culture-specific capitalisation | Medium | Document or test with Turkish culture |
| Very long input performance | Low | Not a unit test concern |
| Idempotence | Low | Add test: `Normalise(Normalise(x)) == Normalise(x)` |

## Running tests

```bash
# From solution root
dotnet test

# From test project
dotnet test NuciText.Normalisation.UnitTests/NuciText.Normalisation.UnitTests.csproj

# With verbosity
dotnet test --verbosity normal

# Filter by test name
dotnet test --filter "FullyQualifiedName~GivenANullOrWhitespaceString"

# Filter by trait (if using Category)
dotnet test --filter "TestCategory=Unit"
```

## CI integration

File: `.github/workflows/dotnet.yml`

```yaml
- name: Test
  run: dotnet test --no-build --verbosity normal
```

Runs on every push and PR to `master`. Failure fails the workflow.

## Test data conventions

| Convention | Example |
|------------|---------|
| Test method names | `GivenAValidString_WhenNormalisingSentence_ThenTheResultHasBeenNormaliseSentenced` |
| TestCase parameters | Input first, expected second |
| Multi-line TestCase | Split across lines for readability |
| Assertion style | `Assert.That(actual, Is.EqualTo(expected))` |

## Extending tests

### Adding a new test case

1. Open `NuciTextNormaliserTests.cs`
2. Add `[TestCase(input, expected)]` to `GivenAValidString_WhenNormalisingSentence_ThenTheResultHasBeenNormaliseSentenced`
3. Run `dotnet test` to verify

### Adding a new test method

```csharp
[Test]
[TestCase("input", "expected")]
public void Given{Context}_When{Action}_Then{Outcome}(string input, string expected)
    => Assert.That(normaliser.NormaliseSentence(input), Is.EqualTo(expected));
```

### Adding a new test class

Create `{NewType}Tests.cs` following the same pattern.

## Test maintenance

| Task | Frequency |
|------|-----------|
| Run full suite | Every commit (CI) |
| Review coverage gaps | Quarterly or when behaviour changes |
| Update test cases | When normalisation rules change |
| Upgrade test packages | With .NET SDK upgrades |

## Debugging tests

```bash
# Run with debugger attached (VS Code)
dotnet test --no-build --logger "console;verbosity=detailed"

# Run single test
dotnet test --filter "FullyQualifiedName=NuciText.Normalisation.UnitTests.NuciTextNormaliserTests.GivenAValidString_WhenNormalisingSentence_ThenTheResultHasBeenNormaliseSentenced"
```