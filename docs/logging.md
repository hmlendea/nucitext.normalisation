# Logging

## Overview

`NuciText.Normalisation` has **no built-in logging**. The library is a pure function with no side effects, no external dependencies, and no observability infrastructure.

## Logging framework

| Property | Value |
|----------|-------|
| Framework | None |
| Configuration | None |
| Sinks | None |
| Levels | None |

## Log levels

Not applicable — no logging.

## Structured fields

Not applicable — no logging.

## Correlation

Not applicable — no logging.

## Sinks/destinations

Not applicable — no logging.

## Redaction/masking

Not applicable — no logging.

## Retention

Not applicable — no logging.

## Sampling

Not applicable — no logging.

## Performance considerations

| Aspect | Impact |
|--------|--------|
| Logging overhead | Zero (no logging) |
| Allocation | Zero (no logging) |
| Latency | Zero (no logging) |

## Error logging

Not applicable — no logging. The method never throws exceptions for string inputs.

## Audit logging

Not applicable — no logging.

## Configuration

Not applicable — no logging.

## Adding logging (consumer side)

Consumers can add logging by wrapping the normaliser:

```csharp
using Microsoft.Extensions.Logging;
using NuciText.Normalisation;

public class LoggingNormaliser : INuciTextNormaliser
{
    private readonly INuciTextNormaliser _inner;
    private readonly ILogger<LoggingNormaliser> _logger;

    public LoggingNormaliser(INuciTextNormaliser inner, ILogger<LoggingNormaliser> logger)
    {
        _inner = inner;
        _logger = logger;
    }

    public string NormaliseSentence(string input)
    {
        _logger.LogDebug("Normalising sentence: {Input}", input);

        var result = _inner.NormaliseSentence(input);

        _logger.LogDebug("Normalised result: {Result}", result);

        return result;
    }
}

// Registration
services.AddSingleton<INuciTextNormaliser, NuciTextNormaliser>();
services.Decorate<INuciTextNormaliser, LoggingNormaliser>();
```

### Structured logging example

```csharp
public class StructuredLoggingNormaliser : INuciTextNormaliser
{
    private readonly INuciTextNormaliser _inner;
    private readonly ILogger _logger;

    public string NormaliseSentence(string input)
    {
        var stopwatch = Stopwatch.StartNew();

        _logger.LogInformation("Normalisation started", new
        {
            Operation = "NormaliseSentence",
            InputLength = input?.Length ?? 0,
            InputPreview = input?.Length > 50 ? input[..50] + "..." : input
        });

        try
        {
            var result = _inner.NormaliseSentence(input);

            _logger.LogInformation("Normalisation completed", new
            {
                Operation = "NormaliseSentence",
                DurationMs = stopwatch.ElapsedMilliseconds,
                OutputLength = result?.Length ?? 0,
                WasNull = result == null
            });

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Normalisation failed", new
            {
                Operation = "NormaliseSentence",
                DurationMs = stopwatch.ElapsedMilliseconds
            });
            throw;
        }
    }
}
```

## Recommended logging for consumers

| Scenario | Recommended level | Fields |
|----------|-------------------|--------|
| Normal operation | Debug | Input, Output, Duration |
| Null input | Debug | Input type |
| Performance monitoring | Information | Duration, Input/Output length |
| Errors (if any) | Error | Exception, Input, Duration |

## Integration with logging frameworks

### Serilog

```csharp
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

var normaliser = new LoggingNormaliser(new NuciTextNormaliser(), Log.Logger);
```

### NLog

```xml
<!-- nlog.config -->
<targets>
  <target name="console" type="Console" layout="${message} ${exception:format=tostring}" />
</targets>
<rules>
  <logger name="*" minlevel="Debug" writeTo="console" />
</rules>
```

### Microsoft.Extensions.Logging

```csharp
using Microsoft.Extensions.Logging;

var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
var logger = loggerFactory.CreateLogger<LoggingNormaliser>();
var normaliser = new LoggingNormaliser(new NuciTextNormaliser(), logger);
```

## Security considerations

### No sensitive data in library

The library processes only the input string provided by the caller. No secrets, credentials, or personal data are handled internally.

### Consumer responsibility

If consumers log input/output, they must:
- Redact sensitive data before logging
- Comply with data protection regulations
- Avoid logging PII, tokens, passwords

### Example redaction

```csharp
public string RedactForLogging(string input)
{
    if (string.IsNullOrEmpty(input)) return input;

    // Redact potential sensitive patterns
    return Regex.Replace(input, @"\b\d{4}[-\s]?\d{4}[-\s]?\d{4}[-\s]?\d{4}\b", "[CARD]")
                .Replace(Regex.Replace(input, @"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b", "[EMAIL]"));
}
```

## Summary

| Aspect | Status |
|--------|--------|
| Built-in logging | None |
| Framework | N/A |
| Configuration | N/A |
| Extensibility | Consumer wraps |
| Performance impact | Zero |
| Security impact | None (no logging) |

The library intentionally avoids logging to remain lightweight, dependency-free, and secure. Consumers have full control over observability.