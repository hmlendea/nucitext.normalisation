# Change guide

## How to modify common areas safely

## 1. Modifying the normalisation pipeline

### Adding a new stage

1. **Edit** `NuciText.Normalisation/NuciTextNormaliser.cs`
2. Add new stage **after** existing stages (before terminal period check)
3. **Add test cases** to `NuciTextNormaliserTests.cs`
4. **Update** `docs/flows/normalise-sentence.md` with new stage
5. **Update** `docs/behaviour/normalise-sentence.md` with new behaviour
6. **Update** `docs/data-model.md` transformation table
7. **Update** `docs/invariants.md` if new invariant introduced
8. **Bump version** if behaviour changes for existing inputs

### Removing a stage

1. **Edit** `NuciTextNormaliser.cs` — remove stage
2. **Remove or update** affected test cases
3. **Update** all documentation files referencing the stage
4. **Bump major version** (breaking change)

### Modifying a regex pattern

1. **Edit** the specific `Regex.Replace` or `Regex.IsMatch` call
2. **Add test cases** for the modified behaviour
3. **Verify** existing test cases still pass
4. **Update** `docs/data-model.md` pattern documentation
5. **Update** `docs/flows/normalise-sentence.md` if flow changes

### Example: Adding HTML entity decoding

```csharp
// After stage 2 (trim), before stage 3 (collapse spaces)
result = System.Net.WebUtility.HtmlDecode(result);
```

**Impact:**
- New dependency on `System.Net.WebUtility` (BCL)
- New test cases needed
- Documentation update required

## 2. Modifying the interface

### Adding a method

1. **Edit** `INuciTextNormaliser.cs` — add method signature
2. **Implement** in `NuciTextNormaliser.cs`
3. **Add tests** for new method
4. **Update** `docs/components/inuci-text-normaliser.md`
5. **Update** `docs/components/nuci-text-normaliser.md`
6. **Update** `docs/api-reference/INDEX.md`
7. **Bump major version** (breaking change)

### Removing a method

1. **Edit** `INuciTextNormaliser.cs` — remove method
2. **Remove implementation** from `NuciTextNormaliser.cs`
3. **Remove tests** for removed method
4. **Update** all documentation
5. **Bump major version** (breaking change)

### Changing return type

1. **Edit** interface and implementation
2. **Update all tests**
3. **Update all documentation**
4. **Bump major version** (breaking change)

## 3. Changing the target framework

### Upgrading (e.g., net10.0 → net11.0)

1. **Edit** both `.csproj` files: `<TargetFramework>net11.0</TargetFramework>`
2. **Verify** no API changes affect implementation
3. **Update** `docs/repository-structure.md`
4. **Update** `docs/build-and-deployment.md`
5. **Update** `docs/dependencies.md`
6. **Update** CI workflow `.github/workflows/dotnet.yml`
7. **Bump version** if breaking

### Downgrading (e.g., net10.0 → net8.0)

1. **Edit** both `.csproj` files
2. **Verify** all APIs used are available in target
3. **Update** all documentation
4. **Update** CI workflow
5. **Bump version** if breaking

## 4. Adding tests

### New test case for existing method

1. **Edit** `NuciTextNormaliserTests.cs`
2. Add `[TestCase(input, expected)]` to existing test method
3. Run `dotnet test` to verify

### New test method

```csharp
[Test]
[TestCase("input", "expected")]
public void Given{Context}_When{Action}_Then{Outcome}(string input, string expected)
    => Assert.That(normaliser.NormaliseSentence(input), Is.EqualTo(expected));
```

### New test class

1. Create `{NewType}Tests.cs`
2. Follow naming conventions
3. Add to test project

## 5. Updating package metadata

### Version bump

1. **Edit** `NuciText.Normalisation/NuciText.Normalisation.csproj`
2. Update `<Version>` element
3. **Update** `docs/build-and-deployment.md` version table
4. **Update** `docs/api-reference/INDEX.md` version table
5. Commit and tag

### License change

1. **Edit** `NuciText.Normalisation.csproj` — `<PackageLicenseExpression>`
2. **Replace** `LICENSE` file content
3. **Update** `docs/repository-overview.md`
4. **Update** `docs/build-and-deployment.md`
5. **Legal review required**

## 6. CI/CD changes

### Adding a workflow step

1. **Edit** `.github/workflows/dotnet.yml`
2. Add step after existing steps
3. Test locally: `dotnet <command>`
4. **Update** `docs/build-and-deployment.md`

### Adding a new workflow

1. Create `.github/workflows/<name>.yml`
2. Follow naming conventions
3. **Update** `docs/build-and-deployment.md`

## 7. Documentation changes

### Adding a new doc file

1. Create file in appropriate `docs/` subdirectory
2. Add entry to `docs/INDEX.md` catalogue
3. Link from related documents

### Updating existing docs

1. Edit the specific file
2. Update cross-references if structure changes
3. Update `docs/INDEX.md` if file list changes

## 8. Dependency changes

### Adding a NuGet dependency

1. **Edit** `NuciText.Normalisation.csproj`
2. Add `<PackageReference>`
3. `dotnet restore`
4. **Update** `docs/dependencies.md`
5. **Update** `docs/build-and-deployment.md`

### Removing a dependency

1. **Edit** `.csproj`
2. Remove `<PackageReference>`
3. Remove `using` statements
4. **Update** documentation

## Safety checklist

Before committing changes:

- [ ] All existing tests pass (`dotnet test`)
- [ ] New tests added for new/modified behaviour
- [ ] Documentation updated for changed behaviour
- [ ] Version bumped if breaking change
- [ ] CI workflow updated if build/test process changed
- [ ] No new warnings (`dotnet build`)
- [ ] Null/whitespace guard still returns `null`
- [ ] Idempotence preserved
- [ ] Thread safety preserved (no new state)
- [ ] No new exceptions thrown

## Rollback procedure

1. Identify the problematic commit
2. `git revert <commit-hash>`
3. Verify tests pass
4. If version was bumped, revert version too
5. Update documentation if needed

## Testing locally

```bash
# Full build and test
dotnet build
dotnet test

# Specific test
dotnet test --filter "FullyQualifiedName~TestName"

# Verbose output
dotnet test --verbosity normal

# No build (incremental)
dotnet test --no-build
```