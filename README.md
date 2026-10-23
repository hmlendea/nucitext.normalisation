[![Donate](https://img.shields.io/badge/-%E2%99%A5%20Donate-%23ff69b4)](https://hmlendea.go.ro/funding)
[![Latest Release](https://img.shields.io/github/v/release/hmlendea/nucitext.normalisation)](https://github.com/hmlendea/nucitext.normalisation/releases/latest)
[![Build Status](https://github.com/hmlendea/nucitext.normalisation/actions/workflows/dotnet.yml/badge.svg)](https://github.com/hmlendea/nucitext.normalisation/actions/workflows/dotnet.yml)
[![NuGet](https://img.shields.io/nuget/v/NuciText.Normalisation)](https://nuget.org/packages/NuciText.Normalisation)
[![License](https://img.shields.io/github/license/hmlendea/nucitext.normalisation)](https://github.com/hmlendea/nucitext.normalisation/blob/master/LICENSE)

# NuciText.Normalisation

A .NET library for normalising text by trimming whitespace, collapsing repeated spaces, ensuring proper spacing around punctuation, capitalising the first letter of each sentence, and appending a period if necessary.

## 📑 Table of Contents

- [✨ Capabilities](#-capabilities)
- [🚀 Usage](#-usage)
- [📦 Installation](#-installation)
- [🛠️ Development](#-development)
- [⚙️ GitHub Actions](#-github-actions)
- [🗂️ Project Structure](#-project-structure)
- [🏗️ Architecture](#-architecture)
- [🗺️ Roadmap](#-roadmap)
- [📚 Documentation](#-documentation)
- [🤝 Contributing](#-contributing)
- [🔒 Security](#-security)
- [💝 Project Engagement](#-project-engagement)
- [📄 License](#-license)

## ✨ Capabilities

- Trims leading and trailing whitespace
- Collapses repeated spaces into single spaces
- Removes spaces before punctuation marks (.,!?;:)
- Ensures single space after in-sentence punctuation marks (,!?;:)
- Capitalises the first letter of each sentence
- Appends a period if the text lacks ending punctuation

## 🚀 Usage

```csharp
using NuciText.Normalisation;

INuciTextNormaliser normaliser = new NuciTextNormaliser();
string result = normaliser.NormaliseSentence("   hello   world!   ");
// Result: "Hello world!"
```

## 📦 Installation

[![Obtain it from NuGet](https://raw.githubusercontent.com/hmlendea/readme-assets/master/badges/stores/nuget.png)](https://nuget.org/packages/NuciText.Normalisation)

### Package Manager Installation

```bash
dotnet add package NuciText.Normalisation
```

Or, via the `Package Manager Console`:
```powershell
Install-Package NuciText.Normalisation
```

## 🛠️ Development

### Requirements

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

### Setup

```bash
git clone https://github.com/hmlendea/nucitext.normalisation.git
cd nucitext.normalisation
dotnet restore
```

### Build

```bash
dotnet build --no-restore
```

### Test

```bash
dotnet test --no-build --verbosity normal
```

### Continuous Integration

The project uses GitHub Actions for continuous integration. The workflow runs on every push and pull request to the `master` branch, executing restore, build, and test steps.

## ⚙️ GitHub Actions

| Workflow | Purpose | What it does |
|----------|---------|--------------|
| [ dotnet.yml ](https://github.com/hmlendea/nucitext.normalisation/blob/master/.github/workflows/dotnet.yml) | CI validation | Restores dependencies, builds the solution, and runs all tests on Ubuntu with .NET 10.0 |

## 🗂️ Project Structure

### Projects and Packages

| Project | Type | Purpose |
|---------|------|---------|
| `NuciText.Normalisation/NuciText.Normalisation.csproj` | Library | Core text normalisation library |
| `NuciText.Normalisation.UnitTests/NuciText.Normalisation.UnitTests.csproj` | Unit Tests | NUnit test suite for the normaliser |

### Directories

| Directory | Purpose |
|-----------|---------|
| `docs/` | Comprehensive documentation covering API reference, behaviour, components, and flows |
| `.github/workflows/` | GitHub Actions CI/CD workflows |

## 🏗️ Architecture

NuciText.Normalisation follows a simple interface-based design with a sealed stateless implementation. The `INuciTextNormaliser` interface defines a single `NormaliseSentence(string)` method. The `NuciTextNormaliser` class implements a 7-stage deterministic pipeline: null guard, trim, collapse spaces, regex punctuation spacing, capitalisation, and terminal period.

See the [architecture documentation](./ARCHITECTURE.md) for the system context, principal components, runtime flows, ownership boundaries, dependencies, constraints, and extension points.

## 🗺️ Roadmap

See [ROADMAP.md](./ROADMAP.md) for planned work and forthcoming changes.

## 📚 Documentation

| Resource | Description |
|----------|-------------|
| [API Reference](./docs/api-reference/) | Complete API documentation |
| [Behaviour](./docs/behaviour/) | Normalisation behaviour specifications |
| [Components](./docs/components/) | Component architecture documentation |
| [Flows](./docs/flows/) | Runtime flow documentation |

## 📜 Code of Conduct

See the [Code of Conduct](./CODE_OF_CONDUCT.md) for the standards of conduct expected in all project spaces.

## 🤝 Contributing

You are welcome to submit any suggestion, feedback, or modification to this project.

When doing so, please:
- Maintain cross-platform compatibility
- Preserve the existing public contract unless a breaking change is intentional
- Submit focused pull requests that conform to the existing code style
- Maintain your branch synchronised with `master`
- Revise the documentation when functionality changes
- Properly test all modifications, including edge cases and error conditions
- Add tests for additional or modified functionality
- Raise a new [issue](https://github.com/hmlendea/nucitext.normalisation/issues) for problems or suggestions

See the [contributing guidelines](./CONTRIBUTING.md) for details on how to report issues, suggest enhancements, and submit changes.

## 🔒 Security

NuciText.Normalisation has no external dependencies beyond the .NET runtime and processes only in-memory string data. It does not perform I/O, network calls, or execute user code.

For information on reporting security vulnerabilities, see [SECURITY.md](./SECURITY.md).

## 💝 Project Engagement

Discovered a problem or have a suggestion? [Open an issue](https://github.com/hmlendea/nucitext.normalisation/issues)!

If you find this project useful, consider [funding it](https://hmlendea.go.ro/funding) or starring ⭐️ it on GitHub!

[![Donate](https://raw.githubusercontent.com/hmlendea/readme-assets/master/donate_generic.png)](https://hmlendea.go.ro/funding)

## 📄 License

This project is being distributed under the `GNU General Public License v3.0 or later`.
See [LICENSE](./LICENSE) for further information.
