# Security

## Overview

`NuciText.Normalisation` is a pure, stateless text transformation library with minimal attack surface. It has no external dependencies, no I/O, no deserialization, and no configuration.

## Security model

### Threat model

| Threat | Applicability | Mitigation |
|--------|---------------|------------|
| Injection (SQL, command, LDAP) | None | No external systems |
| Deserialization attacks | None | No deserialization |
| Path traversal | None | No file I/O |
| XXE | None | No XML processing |
| SSRF | None | No network calls |
| ReDoS | Low | Constant regex patterns, no user input in patterns |
| DoS via allocation | Low | O(n) typical, O(n²) worst case for pathological input |
| Information disclosure | None | No secrets, no logging |
| Privilege escalation | None | No elevated operations |

### Attack surface

```
Input (string)
    │
    ▼
NormaliseSentence
    │
    ├── string.IsNullOrWhiteSpace
    ├── string.Trim
    ├── string.Contains / string.Replace
    ├── Regex.Replace (constant patterns)
    ├── Regex.IsMatch (constant pattern)
    └── string concatenation
    │
    ▼
Output (string)
```

**No:**
- File system access
- Network access
- Process execution
- Reflection
- Dynamic code generation
- Unsafe code
- Native interop
- Cryptography
- Authentication/authorisation

## Input validation

### Library-level

| Check | Implementation |
|-------|----------------|
| Null input | `string.IsNullOrWhiteSpace(input)` → returns `null` |
| Empty string | Same as above |
| Whitespace | Same as above |
| Length limit | None (caller responsibility) |
| Encoding | UTF-16 (native .NET string) |

### Consumer responsibility

Consumers should:
- Validate input length before calling
- Sanitise input if from untrusted source
- Handle `null` return value

## Output safety

### Guarantees

| Property | Guarantee |
|----------|-----------|
| No null for valid input | Returns non-null string |
| No exceptions | Never throws for string input |
| No control characters | Input preserved (no sanitisation) |
| No script injection | Output is plain text |
| Encoding preserved | UTF-16 string |

### Consumer responsibility

- Output is plain text — safe for HTML/JS contexts only if properly encoded by consumer
- No HTML encoding applied — consumer must encode for web contexts

## Regex security (ReDoS)

### Patterns used

| Stage | Pattern | User-controlled? |
|-------|---------|------------------|
| 4 | `\s+([.,!?;:])` | No (constant) |
| 5 | `([,!?;:])([^\s\d])` | No (constant) |
| 6 | `(^\s*[a-z])|([.!?]\s+[a-z])` | No (constant) |
| 7 | `[.!?]$` | No (constant) |

### Analysis

- **All patterns are constants** — no user input in regex
- **No catastrophic backtracking** — patterns are simple, no nested quantifiers
- **Bounded complexity** — linear in input length
- **ReDoS risk: None** — patterns cannot be influenced by attacker

## Dependency security

### Direct dependencies

| Dependency | Type | Version | Vulnerabilities |
|------------|------|---------|-----------------|
| `System.Text.RegularExpressions` | BCL | .NET 10.0 | Managed by Microsoft |

### Transitive dependencies

None for library project.

### Test dependencies

| Dependency | Version | Purpose |
|------------|---------|---------|
| NUnit | 4.5.0 | Test framework |
| Moq | 4.20.72 | Mocking |
| Microsoft.NET.Test.Sdk | 18.3.0 | Test host |

**Note:** Test dependencies not included in NuGet package.

## Supply chain security

### Build integrity

- Built on GitHub Actions (ubuntu-latest)
- .NET SDK from official Microsoft feed
- No custom build scripts
- Deterministic build not enforced (recommended: add `<Deterministic>true</Deterministic>`)

### Package signing

- **Not signed** — no strong name, no certificate
- **Recommended:** Enable package signing for production use

### SBOM

Not generated. Recommended: add SBOM generation to CI.

## Cryptography

None used.

## Authentication/authorisation

None. Library has no concept of users, roles, or permissions.

## Data protection

### Personal data

The library does not:
- Collect personal data
- Store personal data
- Transmit personal data
- Process personal data specially

Input strings may contain personal data — consumer responsibility.

### GDPR/privacy

- No data processing by library
- No cookies, tracking, analytics
- Consumer must comply if input contains personal data

## Compliance

| Standard | Status |
|----------|--------|
| SOC 2 | Not assessed |
| ISO 27001 | Not assessed |
| PCI DSS | Not applicable |
| HIPAA | Not applicable |
| GDPR | No data processing |

## Vulnerability reporting

### Process

1. Report via GitHub Security Advisories (private)
2. Maintainer assesses and fixes
3. Coordinated disclosure
4. Patch release

### Contact

Security issues: GitHub Security Advisories at https://github.com/hmlendea/nucitext.normalisation/security/advisories

## Security testing

### Current

- No automated security scanning
- No penetration testing
- No fuzzing

### Recommended

```bash
# Dependency scanning
dotnet list package --vulnerable

# Static analysis
dotnet build /p:RunAnalyzers=true

# Fuzzing (if available)
# dotnet fuzz
```

## Hardening recommendations

### For library

1. **Enable deterministic builds:**
   ```xml
   <Deterministic>true</Deterministic>
   <ContinuousIntegrationBuild>true</ContinuousIntegrationBuild>
   ```

2. **Enable nullable reference types:**
   ```xml
   <Nullable>enable</Nullable>
   ```

3. **Add package signing:**
   ```xml
   <SignAssembly>true</SignAssembly>
   <AssemblyOriginatorKeyFile>key.snk</AssemblyOriginatorKeyFile>
   ```

4. **Generate SBOM:**
   Add to CI: `dotnet sbom`

### For consumers

1. **Validate input length:**
   ```csharp
   if (input.Length > MaxAllowedLength)
       throw new ArgumentException("Input too long");
   ```

2. **Encode output for context:**
   ```csharp
   // For HTML
   var safe = HttpUtility.HtmlEncode(normaliser.NormaliseSentence(input));

   // For JavaScript
   var safe = JavaScriptEncoder.Default.Encode(normaliser.NormaliseSentence(input));
   ```

3. **Wrap for observability:**
   ```csharp
   // Add logging, metrics, error handling
   ```

## Security checklist

- [ ] No external dependencies
- [ ] No I/O operations
- [ ] No deserialization
- [ ] Constant regex patterns
- [ ] No user input in patterns
- [ ] No secrets in code
- [ ] No logging of input
- [ ] Thread-safe (no race conditions)
- [ ] No privilege escalation
- [ ] Input validation (null/whitespace)
- [ ] Output safe for plain text contexts

## Summary

`NuciText.Normalisation` has a minimal attack surface:

- **Pure function** — no side effects
- **No dependencies** — only BCL
- **No configuration** — no injection vectors
- **Constant regex** — no ReDoS
- **No I/O** — no path traversal, SSRF, XXE
- **No auth** — no privilege escalation

Primary risk: **Consumer misuse** (not encoding output, not validating input length). Library itself is secure by design.