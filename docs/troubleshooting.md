# Troubleshooting

## Common issues

### 1. Unexpected normalisation output

**Symptom:** Output doesn't match expectations.

**Causes:**
- Input contains non-ASCII characters
- Version number heuristic affects output
- Culture-specific capitalisation differs

**Solutions:**
- Review [behaviour/normalise-sentence.md](../behaviour/normalise-sentence.md) for exact rules
- Test with known inputs from [testing.md](../testing.md#test-case-catalogue)
- Check if version number protection is triggering (digits after punctuation)

### 2. Null return when expecting string

**Symptom:** `NormaliseSentence` returns `null` for non-null input.

**Causes:**
- Input is whitespace-only (`" "`, `"\t"`, `"\n"`)
- Input is empty string `""`

**Solutions:**
- Check `string.IsNullOrWhiteSpace(input)` before calling
- Use null-coalescing: `normaliser.NormaliseSentence(input) ?? ""`

### 3. Performance issues with large input

**Symptom:** Slow execution for very long strings.

**Causes:**
- Stage 3 (space collapse) uses O(n²) iterative replacement
- Multiple regex allocations per call

**Solutions:**
- Validate input length before calling
- Consider pre-processing to limit input size
- For very large inputs, implement custom optimisation

### 4. Culture-specific capitalisation issues

**Symptom:** Capitalisation differs from expected (e.g., Turkish `i` → `İ`).

**Causes:**
- `ToUpper()` uses current thread culture
- No invariant culture specified

**Solutions:**
- Set thread culture before calling: `Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture`
- Wrap normaliser to enforce invariant culture
- Accept as designed behaviour

### 5. Build failures

**Symptom:** `dotnet build` fails.

**Causes:**
- .NET 10 SDK not installed
- csproj syntax errors
- Missing dependencies

**Solutions:**
- Install .NET 10 SDK
- Check csproj for syntax errors
- Run `dotnet restore` first

### 6. Test failures

**Symptom:** `dotnet test` fails.

**Causes:**
- Test project not referencing library correctly
- NUnit adapter not installed
- Test cases outdated after behaviour change

**Solutions:**
- Verify project reference in test csproj
- Run `dotnet restore`
- Update test cases to match new behaviour

### 7. NuGet package issues

**Symptom:** Package not found or version conflict.

**Causes:**
- Package not published to NuGet
- Version already exists
- Feed configuration incorrect

**Solutions:**
- Check NuGet.org for package existence
- Bump version in csproj
- Verify API key and feed URL

## Debugging

### Enable detailed logging

```csharp
// Wrap normaliser for debugging
public class DebugNormaliser : INuciTextNormaliser
{
    private readonly INuciTextNormaliser _inner;
    
    public string NormaliseSentence(string input)
    {
        Console.WriteLine($"[DEBUG] Input: '{input}'");
        var result = _inner.NormaliseSentence(input);
        Console.WriteLine($"[DEBUG] Output: '{result}'");
        return result;
    }
}
```

### Inspect intermediate stages

```csharp
// Copy NormaliseSentence implementation and add logging
public string DebugNormaliseSentence(string input)
{
    if (string.IsNullOrWhiteSpace(input))
    {
        Console.WriteLine("Stage 1: Null/whitespace → null");
        return null;
    }

    string result = input.Trim();
    Console.WriteLine($"Stage 2 (Trim): '{result}'");

    while (result.Contains("  "))
    {
        result = result.Replace("  ", " ");
    }
    Console.WriteLine($"Stage 3 (Collapse): '{result}'");

    result = Regex.Replace(result, @"\s+([.,!?;:])", "$1");
    Console.WriteLine($"Stage 4 (Space before punct): '{result}'");

    result = Regex.Replace(result, @"([,!?;:])([^\s\d])", "$1 $2");
    Console.WriteLine($"Stage 5 (Space after punct): '{result}'");

    result = Regex.Replace(result, @"(^\s*[a-z])|([.!?]\s+[a-z])", m => m.Value.ToUpper());
    Console.WriteLine($"Stage 6 (Capitalise): '{result}'");

    if (!Regex.IsMatch(result, @"[.!?]$"))
    {
        result += ".";
    }
    Console.WriteLine($"Stage 7 (Terminal): '{result}'");

    return result;
}
```

### Run tests with verbose output

```bash
dotnet test --verbosity normal
dotnet test --logger "console;verbosity=detailed"
```

### Debug specific test

```bash
dotnet test --filter "FullyQualifiedName=NuciText.Normalisation.UnitTests.NuciTextNormaliserTests.GivenAValidString_WhenNormalisingSentence_ThenTheResultHasBeenNormaliseSentenced"
```

## Error messages

### No exceptions thrown

The library never throws exceptions for valid string inputs. If you see an exception:

1. **NullReferenceException** — Impossible (guard clause)
2. **ArgumentException** — Impossible (constant regex patterns)
3. **OutOfMemoryException** — System-level, not library issue
4. **StackOverflowException** — Impossible (no recursion)

## Performance troubleshooting

### High allocation rate

**Symptom:** High GC pressure.

**Analysis:**
- Each call allocates ~5-15 strings
- Regex creates MatchCollection objects
- Lambda in stage 6 creates delegates

**Mitigation:**
- Reuse normaliser instance
- Batch processing with `AsParallel()`
- Consider custom implementation for hot paths

### Slow execution

**Symptom:** High latency per call.

**Analysis:**
- Typical: <1ms for sentence-length input
- Pathological: O(n²) for many repeated spaces

**Mitigation:**
- Limit input length
- Pre-process to collapse spaces before calling
- Profile with `dotnet trace` or `dotnet counters`

## CI/CD troubleshooting

### GitHub Actions failures

**Symptom:** Workflow fails.

**Common causes:**
1. **Setup .NET fails** — Version `10.0.x` not available
2. **Restore fails** — Network/feed issues
3. **Build fails** — Compilation errors
4. **Test fails** — Test failures

**Solutions:**
- Check workflow logs for specific error
- Verify .NET 10 availability on runner
- Run locally to reproduce

### Version conflicts

**Symptom:** `dotnet nuget push` fails.

**Causes:**
- Version already exists on NuGet
- Package ID conflict
- API key invalid

**Solutions:**
- Bump version in csproj
- Check NuGet.org for existing versions
- Verify API key in secrets

## Getting help

### Resources

1. **Documentation** — Start with [quick-start.md](../quick-start.md)
2. **Behaviour spec** — [behaviour/normalise-sentence.md](../behaviour/normalise-sentence.md)
3. **Test cases** — [testing.md](../testing.md#test-case-catalogue)
4. **Source code** — `NuciTextNormaliser.cs`

### Reporting issues

1. Check existing issues on GitHub
2. Create minimal reproduction
3. Include:
   - Input string
   - Expected output
   - Actual output
   - .NET version
   - Library version

### Contributing fixes

1. Fork repository
2. Create branch
3. Add failing test case
4. Fix implementation
5. Update documentation
6. Submit PR

## Known limitations

| Limitation | Workaround |
|------------|------------|
| No locale parameter | Set thread culture before calling |
| No configuration | Wrap or fork |
| Version heuristic only | Pre-process version numbers |
| Single sentence assumption | Split paragraphs before calling |
| No async API | Use `Task.Run` for offloading |

## Version-specific issues

### Version 1.0.1

No known issues beyond documented limitations.

### Future versions

Check release notes for breaking changes.