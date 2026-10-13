# Documentation maintenance

## Overview

This document describes how to keep the documentation in `docs/` current, accurate, and useful.

## Documentation structure

```
docs/
├── INDEX.md                          # Master index and navigation
├── repository-overview.md            # Repository purpose, scope, entry points
├── repository-structure.md           # Source tree layout, module organisation
├── architecture.md                   # High-level architecture
├── data-model.md                     # Domain entities, transformations
├── configuration.md                  # Configuration schema, sources, precedence
├── dependencies.md                   # External and internal dependencies
├── design-decisions.md               # Key architectural/design choices
├── invariants.md                     # System-wide invariants and contracts
├── testing.md                        # Test strategy, organisation, coverage
├── build-and-deployment.md           # Build pipeline, deployment, environments
├── quick-start.md                    # Getting started guide
├── change-guide.md                   # How to modify common areas safely
├── troubleshooting.md                # Common issues and solutions
├── faq.md                            # Frequently asked questions
├── ambiguities-and-open-questions.md # Unresolved items, TODOs, known gaps
├── concurrency-and-scheduling.md     # Threading, async, schedulers, locks
├── logging.md                        # Logging framework, levels, structured fields
├── security.md                       # Security model, threats, mitigations
├── state-and-persistence.md          # State management, stores, caches
├── integrations.md                   # External system integrations
├── api-usage-examples.md             # Example requests/responses
├── behaviour/
│   └── normalise-sentence.md         # User-facing behaviour
├── flows/
│   └── normalise-sentence.md         # End-to-end execution flow
├── components/
│   ├── nuci-text-normaliser.md       # Implementation deep dive
│   └── inuci-text-normaliser.md      # Interface deep dive
└── api-reference/
    └── INDEX.md                      # API reference index
```

## Maintenance responsibilities

### Owner

- **Primary:** Horațiu Mlendea (hmlendea)
- **Review:** Before each release

### Update triggers

| Trigger | Documentation to update |
|---------|------------------------|
| New feature added | All relevant docs + INDEX.md |
| Behaviour changed | behaviour/, flows/, components/, invariants.md, testing.md |
| API changed | api-reference/, components/, quick-start.md |
| Test added/changed | testing.md, components/ |
| Build/CI changed | build-and-deployment.md, configuration.md |
| Dependency added/removed | dependencies.md, build-and-deployment.md |
| Bug fixed | troubleshooting.md, testing.md if test added |
| Version bumped | build-and-deployment.md, api-reference/INDEX.md |
| New ambiguity discovered | ambiguities-and-open-questions.md |

## Maintenance workflow

### 1. During development

- Update docs **in the same PR** as code changes
- Treat documentation as part of the change, not afterthought
- Run `dotnet build` to verify no broken links (if using docfx or similar)

### 2. Before release

- Review all changed files for accuracy
- Update version references
- Update `INDEX.md` last reviewed date
- Verify cross-references work

### 3. Periodic review (quarterly)

- Check for outdated information
- Verify links to root documents (README.md, LICENSE, etc.)
- Review ambiguities-and-open-questions.md for resolved items
- Update FAQ with new common questions

## Documentation standards

### File naming

- **kebab-case:** `normalise-sentence.md`, not `normaliseSentence.md`
- **Descriptive:** `repository-overview.md`, not `overview.md`
- **Consistent:** Use same terms across files

### Headings

- **Sentence case:** `## Repository purpose`, not `## Repository Purpose`
- **Hierarchical:** `#`, `##`, `###` for structure
- **Unique:** No duplicate heading names within file

### Cross-references

- **Relative from docs/ root:** `[Architecture](./architecture.md)`
- **Absolute for root files:** `[README.md](../README.md)`
- **Anchor links:** `[Stage 3](#stage-3-collapse-repeated-spaces)`

### Code symbols

- **Backticks with namespace:** `` `NuciText.Normalisation.NuciTextNormaliser.NormaliseSentence` ``
- **Method signatures:** `NormaliseSentence(string): string`
- **Types:** `INuciTextNormaliser`, `NuciTextNormaliser`

### Code blocks

- **Language tags:** ```csharp, ```bash, ```yaml
- **Complete examples:** Runnable snippets where possible
- **Comments:** Explain non-obvious parts

## Content guidelines

### What to document

1. **Public API** — Every public type and member
2. **Behaviour** — What the code does, not just how
3. **Invariants** — What must always be true
4. **Decisions** — Why choices were made
5. **Gotchas** — Edge cases, limitations, surprises
6. **Procedures** — How to build, test, deploy, debug

### What not to document

1. **Implementation details** that are obvious from code
2. **Standard patterns** (DI, async/await) unless library-specific
3. **Future plans** — use ROADMAP.md at root
4. **Internal implementation** unless needed for debugging

### Tone

- **Factual** — State what is, not what should be
- **Concise** — Prefer tables and lists over prose
- **Actionable** — Include commands, examples, steps
- **Honest** — Document limitations and known issues

## Review checklist

Before merging documentation changes:

- [ ] All cross-references resolve
- [ ] Code examples compile (if applicable)
- [ ] Version numbers consistent
- [ ] No duplicate content
- [ ] INDEX.md updated if file list changed
- [ ] Root document links verified
- [ ] No broken external links
- [ ] Spelling and grammar checked

## Tools

### Link checking

```bash
# Install markdown-link-check
npm install -g markdown-link-check

# Check all docs
markdown-link-check docs/**/*.md
```

### Spell checking

```bash
# Install cspell
npm install -g cspell

# Check all docs
cspell "docs/**/*.md"
```

### Formatting

```bash
# Prettier for markdown
npx prettier --write "docs/**/*.md"
```

## Versioning

### Documentation version

Documentation version tracks library version. Update when:

- Major version: Full review and restructure
- Minor version: Add new feature docs
- Patch version: Fix typos, clarify existing

### Changelog

Not maintained separately. Use git history and GitHub releases.

## Archiving

### Obsolete documentation

- Move to `docs/archive/` with date prefix
- Update INDEX.md to remove links
- Keep for reference, not navigation

### Example

```
docs/archive/
├── 2026-10-old-architecture.md
└── 2026-09-deprecated-api.md
```

## Metrics

### Quality indicators

| Metric | Target |
|--------|--------|
| Cross-reference validity | 100% |
| External link validity | 100% |
| Coverage of public API | 100% |
| Time since last review | < 90 days |
| Open ambiguities | < 10 |

### Tracking

Review metrics quarterly. Update `INDEX.md` maintenance metadata.

## Contributing to documentation

### For contributors

1. Follow the same standards as code
2. Update in same PR as code changes
3. Ask maintainer if unsure about structure
4. Use existing files as templates

### For maintainers

1. Enforce standards in PR review
2. Update INDEX.md when files added/removed
3. Keep archive clean
4. Document decisions in design-decisions.md

## Emergency updates

### Hotfix documentation

1. Create minimal fix in `docs/`
2. Update INDEX.md if needed
3. Deploy with hotfix release
4. Schedule full review for next cycle

## Contact

- **Documentation owner:** Horațiu Mlendea
- **Issues:** GitHub Issues
- **Discussions:** GitHub Discussions