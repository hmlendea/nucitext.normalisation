# Privacy and Personal Data

This document describes how `NuciText.Normalisation` at https://github.com/hmlendea/nucitext.normalisation handles personal data. It covers the library behaviour and verified integrations described below. Where the software is consumed as a library, the consuming application operator has separate responsibilities described below.

**Information reviewed:** 2026-10-13

## Table of Contents

- [What This Document Covers](#what-this-document-covers)
- [Data We Handle](#data-we-handle)
- [Processing and Use](#processing-and-use)
- [Storage, Retention, and Deletion](#storage-retention-and-deletion)
- [External Processing and Integrations](#external-processing-and-integrations)
- [Document Changes](#document-changes)
- [Contact](#contact)

## What This Document Covers

This document describes how `NuciText.Normalisation` at https://github.com/hmlendea/nucitext.normalisation handles personal data. It covers the library behaviour and verified integrations described below. Where the software is consumed as a library, the consuming application operator has separate responsibilities described below.

## Data We Handle

### Data Provided to the Application

- The library accepts a single `string` input parameter to `NormaliseSentence`. This string may contain personal data provided by the consuming application. The library does not request, validate, or categorise personal data.

### Data Generated or Collected by the Application

- No personal data is generated or collected automatically by the library. The library performs no logging, telemetry, crash reporting, or update checks.

### Data Received from Integrations

- No personal data is received from integrations or third parties. The library has no external integrations.

## Processing and Use

The library processes the data described above for these verified functions:
- Text normalisation (trimming, space collapsing, punctuation spacing, capitalisation, terminal period) — Input string provided by caller

## Storage, Retention, and Deletion

The library does not persist, cache, or store any data. All transformations are in-memory, pure, and deterministic. Input strings are caller-owned and not modified. Intermediate string allocations (5–15 per call) are short-lived and eligible for Gen 0 garbage collection. Output strings are returned to the caller. The library has no databases, files, logs, caches, or backups.

## External Processing and Integrations

The library has no built-in external data transfer. No external services, recipients, or integrations process or receive data from this library.

| Service or integration | Purpose | Data involved | Configuration or documentation |
|-----------------------|---------|---------------|--------------------------------|
| None | N/A | N/A | N/A |

## Document Changes

Update this document when library data flows, storage, integrations, or deployment responsibilities change. The current version is published at https://github.com/hmlendea/nucitext.normalisation/blob/master/PRIVACY.md.

## Contact

For questions about library data handling, contact the project maintainers via GitHub. For a consuming application, contact the application operator, unless the project explicitly handles the request. Include the library version and calling context; do not send passwords, access tokens, or other secrets.