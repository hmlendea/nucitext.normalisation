# Security Policy

This document describes the security policy for `NuciText.Normalisation`, a stateless .NET text normalisation library with no external dependencies, no I/O, and no configuration. The policy covers vulnerability reporting, supported versions, and disclosure expectations.

## Table of Contents

- [Supported Versions](#supported-versions)
- [Reporting a Vulnerability](#reporting-a-vulnerability)
- [Scope](#scope)
- [Disclosure Policy](#disclosure-policy)
- [Safe Harbour](#safe-harbour)
- [Recognition](#recognition)

## Supported Versions

Use this table to indicate which project versions currently receive security maintenance.

| Version | Distribution Channel | Supported |
|---------|--------------------|-----------|
| Latest version | NuGet.org | ✅ |
| Latest version | GitHub Releases | ✅ |
| Preceding versions | Any distribution channel | ❌ |

Only the latest released version on NuGet.org and GitHub Releases receives security updates. Preceding versions are not maintained.

## Reporting a Vulnerability

Please do not disclose suspected vulnerabilities publicly before maintainers have had an opportunity to validate and remediate them.

To report a vulnerability:
- [GitHub Security Advisories](https://github.com/hmlendea/nucitext.normalisation/security/advisories)
- Contact the maintainers directly via the repository

## Scope

The subsequent report categories are in scope for this repository:
- ReDoS (Regular Expression Denial of Service) in the normalisation pipeline
- Memory exhaustion via pathological input
- Supply chain compromise of the NuGet package or GitHub Actions workflow
- Build integrity issues in the CI pipeline

The subsequent categories are out of scope unless explicitly stated to the contrary:
- Vulnerabilities in the .NET runtime or BCL (report to Microsoft)
- Vulnerabilities in consumer applications using this library
- Denial of service via consumer misuse (e.g., unbounded input length)
- Issues in test dependencies (NUnit, Moq) not included in the NuGet package

## Disclosure Policy

This project follows coordinated disclosure:
1. Vulnerabilities are investigated privately.
2. A remediation plan is prepared and validated.
3. Public disclosure is published after a fix, mitigation, or agreed risk decision is available.
4. Credit is attributed in accordance with reporter preference and project policy.

## Safe Harbour

If your research is conducted in good faith, confined to authorised scope, and disclosed responsibly, the maintainers will not pursue action for policy-compliant activity.

## Recognition

We appreciate responsible disclosure. Reporters who desire public attribution may be acknowledged in release notes, advisories, or a dedicated acknowledgements section.