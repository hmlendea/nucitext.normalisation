# FAQ

## General questions

### What does this library do?

`NuciText.Normalisation` normalises single-sentence text by trimming whitespace, collapsing repeated spaces, fixing punctuation spacing, capitalising sentence starts, and ensuring terminal punctuation.

### What is the target framework?

.NET 10.0 (`net10.0`)

### Is it open source?

Yes, licensed under GPL-3.0-or-later. Source at https://github.com/hmlendea/nucitext.normalisation

### How do I install it?

```bash
dotnet add package NuciText.Normalisation
```

### What are the dependencies?

None. Only .NET Base Class Library (BCL). No NuGet dependencies.

## Usage questions

### How do I use it?

```csharp
using NuciText.Normalisation;

var normaliser = new NuciTextNormaliser();
string result = normaliser.NormaliseSentence("  hello   world!  ");
// result = "Hello world!"
```

### Can I use dependency injection?

Yes:
```csharp
services.AddSingleton<INuciTextNormaliser, NuciTextNormaliser>();
```

### Is it thread-safe?

Yes. The implementation is stateless and thread-safe. A single instance can be shared across threads.

### Does it support async?

No. The method is synchronous. Use `Task.Run` if you need to offload:
```csharp
var result = await Task.Run(() => normaliser.NormaliseSentence(input));
```

### What does it return for null/empty input?

Returns `null` for `null`, `""`, or whitespace-only input.

### How do I handle the null return?

```csharp
string safe = normaliser.NormaliseSentence(input) ?? "";
// or
if (normaliser.NormaliseSentence(input) is { } result) { ... }
```

## Behaviour questions

### What transformations does it apply?

1. Trim whitespace
2. Collapse repeated spaces
3. Remove spaces before punctuation (`. , ! ? ; :`)
4. Ensure space after punctuation (except before digits)
5. Capitalise first letter of each sentence
6. Append period if missing terminal punctuation

### Does it handle multiple sentences?

Yes, it capitalises the first letter after `.`, `!`, or `?` followed by whitespace. But it treats the input as a single text block.

### Does it preserve version numbers like `1.2.3`?

Yes. The space-after-punctuation rule excludes digits, so `1.2.3` remains unchanged.

### What culture does it use for capitalisation?

Current thread culture (`ToUpper()` without explicit culture). Not invariant culture.

### Is it idempotent?

Yes. `NormaliseSentence(NormaliseSentence(x)) == NormaliseSentence(x)` for all valid inputs.

## Technical questions

### Can I extend or customise it?

Not directly. The implementation is `sealed` with no virtual methods. Options:
1. Wrap/decorate the normaliser
2. Pre/post-process input/output
3. Fork the library

### Can I mock it for testing?

Yes. The interface `INuciTextNormaliser` is easily mockable with Moq or similar.

### Does it log or have telemetry?

No. Zero observability built in. Wrap it if you need logging.

### What is the performance?

- Time: O(n) typical, O(n²) worst case (space collapse)
- Allocations: ~5-15 strings per call
- Thread contention: None

### Can I target older .NET versions?

Currently only `net10.0`. The code likely works on .NET 8/9 but is not tested or published for them.

## Licensing questions

### What license?

GPL-3.0-or-later. Copyleft — derivative works must be GPL-compatible.

### Can I use it in commercial software?

Yes, if your software is also GPL-3.0-or-later compatible. Consult legal counsel.

### Can I use it in proprietary software?

Only if you comply with GPL-3.0 (source disclosure, same license). Not suitable for closed-source proprietary software.

## Contribution questions

### How do I contribute?

1. Fork the repository
2. Create a branch
3. Add tests for new behaviour
4. Implement changes
5. Update documentation
6. Submit PR

### Where do I report bugs?

GitHub Issues at https://github.com/hmlendea/nucitext.normalisation/issues

### Where is the changelog?

Not currently maintained. Check git history and release tags.

## Troubleshooting

### Build fails with "SDK not found"

Install .NET 10 SDK from https://dotnet.microsoft.com/download

### Tests fail after behaviour change

Update test cases in `NuciTextNormaliserTests.cs` to match new expected output.

### Package not found on NuGet

Check spelling: `NuciText.Normalisation` (case-insensitive). Verify version exists.

### Unexpected output for my input

Review [behaviour/normalise-sentence.md](../behaviour/normalise-sentence.md) for exact rules. Test with known cases from [testing.md](../testing.md#test-case-catalogue).

## Advanced questions

### How does the space collapse work?

Iterative `while (result.Contains("  ")) result = result.Replace("  ", " ");` — replaces double spaces until none remain.

### Why regex for punctuation?

Declarative patterns handle complex conditions (e.g., "not digit") cleanly. BCL regex is well-optimised.

### Can I disable specific stages?

No. The pipeline is fixed. Wrap the normaliser to pre/post-process.

### Is there a maximum input length?

No hard limit. Very large inputs may have performance issues due to O(n²) space collapse.

### Does it handle HTML/markdown?

No. Treats all input as plain text. HTML entities, tags, markdown syntax are not processed.

## Migration questions

### Upgrading from older version

Check release notes for breaking changes. Current version 1.0.1 has no breaking changes from 1.0.0.

### Breaking changes policy

Any change to normalisation output for same input requires major version bump.

### Deprecation policy

Not applicable — single method interface, no deprecated members.

## Support questions

### Is there commercial support?

No. Community support via GitHub Issues only.

### What is the SLA?

None. Open source, best-effort.

### Can I request features?

Yes, via GitHub Issues. But the library is intentionally minimal — new features unlikely unless widely needed.