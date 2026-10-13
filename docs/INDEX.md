# Documentation index

## Repository summary

`NuciText.Normalisation` is a small, dependency-free .NET library that provides text normalisation for single-sentence input. It exposes a single interface, `INuciTextNormaliser`, with one method, `NormaliseSentence`, which trims whitespace, collapses repeated spaces, fixes spacing around punctuation, capitalises sentence starts, and appends a terminal period when one is missing. The library targets .NET 10 and is distributed as a NuGet package under the GPL-3.0-or-later license.

## Root document map

- [README.md](../README.md) — installation, usage, and badge links
- [LICENSE](../LICENSE) — GPL-3.0-or-later license text
- [.github/FUNDING.yml](../.github/FUNDING.yml) — funding configuration
- [.github/workflows/dotnet.yml](../.github/workflows/dotnet.yml) — CI build and test workflow

No root `ARCHITECTURE.md`, `SECURITY.md`, `ROADMAP.md`, or `PRIVACY.md` exists in this repository.

## Documentation catalogue

### Core documentation

| File | Description |
|------|-------------|
| [repository-overview.md](./repository-overview.md) | Repository purpose, scope, and entry points |
| [repository-structure.md](./repository-structure.md) | Source tree layout and module organisation |
| [architecture.md](./architecture.md) | High-level architecture and component relationships |
| [data-model.md](./data-model.md) | Domain entities and data transformations |
| [configuration.md](./configuration.md) | Configuration schema, sources, and precedence |
| [dependencies.md](./dependencies.md) | External and internal dependencies |
| [design-decisions.md](./design-decisions.md) | Key architectural and design choices with rationale |
| [invariants.md](./invariants.md) | System-wide invariants and contracts |
| [testing.md](./testing.md) | Test strategy, organisation, and coverage |
| [build-and-deployment.md](./build-and-deployment.md) | Build pipeline, CI, and deployment |
| [quick-start.md](./quick-start.md) | Getting started guide for newcomers |
| [change-guide.md](./change-guide.md) | How to modify common areas safely |
| [troubleshooting.md](./troubleshooting.md) | Common issues and solutions |
| [faq.md](./faq.md) | Frequently asked questions |
| [ambiguities-and-open-questions.md](./ambiguities-and-open-questions.md) | Unresolved items and known gaps |
| [documentation-maintenance.md](./documentation-maintenance.md) | How to keep docs current and ownership |

### Behaviour and flows

| File | Description |
|------|-------------|
| [behaviour/normalise-sentence.md](./behaviour/normalise-sentence.md) | User-facing behaviour of the normalisation operation |
| [flows/normalise-sentence.md](./flows/normalise-sentence.md) | End-to-end execution flow of `NormaliseSentence` |

### Components

| File | Description |
|------|-------------|
| [components/nuci-text-normaliser.md](./components/nuci-text-normaliser.md) | Deep dive into the `NuciTextNormaliser` implementation |
| [components/inuci-text-normaliser.md](./components/inuci-text-normaliser.md) | Deep dive into the `INuciTextNormaliser` interface |

### API reference

| File | Description |
|------|-------------|
| [api-reference/INDEX.md](./api-reference/INDEX.md) | API reference index and endpoint summary |

## Navigation aids

- **Start here** — read [repository-overview.md](./repository-overview.md) for purpose and scope, then [quick-start.md](./quick-start.md) for usage.
- **Deep dive** — read [components/nuci-text-normaliser.md](./components/nuci-text-normaliser.md) for the implementation, then [flows/normalise-sentence.md](./flows/normalise-sentence.md) for the execution flow.
- **Flows** — read [flows/normalise-sentence.md](./flows/normalise-sentence.md) for the step-by-step normalisation pipeline.
- **Tests** — read [testing.md](./testing.md) for test strategy and [components/nuci-text-normaliser.md](./components/nuci-text-normaliser.md) for test-to-implementation mapping.

## Maintenance metadata

- **Last reviewed:** 2026-10-13
- **Coverage status:** Complete — all source files and tests documented
