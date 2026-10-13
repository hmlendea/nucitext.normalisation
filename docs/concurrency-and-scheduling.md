# Concurrency and scheduling

## Overview

`NuciText.Normaliser` is a stateless, pure function with no internal scheduling or concurrency concerns. All concurrency is external to the library.

## Thread safety

### Implementation characteristics

| Property | Value |
|----------|-------|
| **Instance fields** | None |
| **Static fields** | None |
| **Local variables** | Method-scoped only |
| **External dependencies** | `System.Text.RegularExpressions.Regex` (static, thread-safe) |
| **Mutable state** | None |

### Concurrency safety guarantees

1. **Reentrant** — Same thread can call `NormaliseSentence` recursively
2. **Thread-safe** — Multiple threads can call simultaneously
3. **No race conditions** — No shared mutable state
4. **No deadlocks** — No locks or synchronization primitives

### Test coverage

No explicit concurrency tests in current test suite. The stateless nature makes this safe, but we can add:

```csharp
[Test]
public void ConcurrentAccess_ThreadSafety()
{
    var normaliser = new NuciTextNormaliser();
    var results = new List<string>();
    var barrier = new Barrier(10);

    Parallel.For(0, 10, i =>
    {
        barrier.SignalAndWait();
        results.Add(normaliser.NormaliseSentence("test input"));
    });

    // All results should be identical
    Assert.That(results, Is.All.EqualTo("Test input."));
}
```

## Scheduling

### No internal scheduling

- No timers, timeouts, or delayed execution
- No background threads
- No async/await
- No callbacks or event loops

### External scheduling

Consumers may schedule calls:

```csharp
// Parallel processing
var results = inputs.AsParallel().Select(input =>
    normaliser.NormaliseSentence(input)).ToList();

// Thread pool
ThreadPool.QueueUserWorkItem(_ =>
    normaliser.NormaliseSentence(input));

// Task parallel library
Task.Run(() => normaliser.NormaliseSentence(input));
```

## Performance under concurrency

### Characteristics

| Metric | Value |
|--------|-------|
| **CPU usage** | Linear with concurrent calls |
| **Memory pressure** | Each call allocates ~5-15 strings |
| **GC pressure** | Low (short-lived, gen 0) |
| **Cache locality** | Good (small, sequential strings) |

### Scaling

- **Ideal scaling** — Linear scaling up to CPU cores
- **No contention** — No shared resources
- **No bottlenecks** — Each call independent

## Error handling under concurrency

### Exception propagation

```csharp
// If a call throws, exception propagates to caller
// No shared state to corrupt
// No recovery needed
```

### Memory allocation

- **High allocation** — Each call allocates new strings
- **GC impact** — Short-lived strings collected quickly
- **Mitigation** — Reuse normaliser instance across threads

## Integration with async/await

### Synchronous library in async context

```csharp
// Direct call (no await)
var result = normaliser.NormaliseSentence(input);

// With Task.Run for offloading
var result = await Task.Run(() => normaliser.NormaliseSentence(input));
```

### Thread pool usage

```csharp
// Queue work item
ThreadPool.QueueUserWorkItem(_ =>
{
    var result = normaliser.NormaliseSentence(input);
    // Process result
});
```

## Testing concurrency

### Current test suite

No concurrency tests. The library is simple enough that concurrency safety is guaranteed by:

1. **Statelessness** — No instance or static fields
2. **Immutable inputs** — `string` is immutable
3. **Immutable outputs** — Returns new `string`
4. **Thread-safe BCL** — `Regex` is thread-safe

### Recommended concurrency tests

```csharp
[Test]
public void Stateless_ThreadSafety()
{
    var normaliser = new NuciTextNormaliser();
    var barrier = new Barrier(5);
    var results = new string[5];

    Parallel.For(0, 5, i =>
    {
        barrier.SignalAndWait();
        results[i] = normaliser.NormaliseSentence("test");
    });

    // All results should be identical
    Assert.That(results, Is.All.EqualTo("Test."));
}

[Test]
public void Reentrant_ThreadSafety()
{
    var normaliser = new NuciTextNormaliser();
    var results = new List<string>();

    Parallel.For(0, 10, i =>
    {
        // Each thread calls multiple times
        for (int j = 0; j < 10; j++)
        {
            results.Add(normaliser.NormaliseSentence("input"));
        }
    });

    // All results should be identical
    Assert.That(results, Is.All.EqualTo("Input."));
}
```

## Deadlock analysis

### No deadlock risk

- No locks acquired
- No wait conditions
- No shared resources
- No blocking I/O

### External deadlock possibilities

```csharp
// If consumer introduces synchronization
// that calls back into NormaliseSentence
// while holding a lock
```

**Risk:** Low — library has no callbacks or events.

## Memory model

### Allocation pattern

```
Input string (stack/reference)
    │
    ▼
Trim() ──────────────────▶ New string (heap)
    │
    ▼
while (Contains) ───────▶ N × Replace() ──────▶ N strings (heap)
    │                          │
    ▼                          ▼
Regex operations ──────────▶ New strings + MatchCollection (heap)
    │
    ▼
Return string (heap)
```

### GC pressure

- **Allocation rate:** ~5-15 strings per call
- **Lifetime:** Short (returned to caller)
- **Generation:** Mostly Gen 0
- **Impact:** Minimal for typical usage

### Memory leaks

**None expected.**
- No caches
- No static collections
- No finalizers
- No unmanaged resources

## Resource limits

### CPU

- **No limits** — CPU usage proportional to input size
- **No throttling** — Pure computation

### Memory

- **Per call:** ~1KB for typical sentence
- **No limits** — Caller responsible for input size

### Time

- **No timeouts** — Method completes when input processed
- **Deterministic** — Same input, same time

## Performance recommendations

### For high-throughput scenarios

```csharp
// Reuse normaliser instance
var normaliser = new NuciTextNormaliser();

// Use parallel processing for batch inputs
var results = inputs.AsParallel().Select(input =>
    normaliser.NormaliseSentence(input)).ToList();
```

### For memory-constrained environments

```csharp
// Input size limits
if (input.Length > MaxInputSize)
    throw new ArgumentException("Input too large");
```

### For deterministic timing

```csharp
// No async operations needed
var result = normaliser.NormaliseSentence(input);
```

## Monitoring

### No built-in monitoring

- No counters
- No timers
- No metrics
- No logging

### External monitoring

```csharp
// Wrap for observability
public class MonitoringNormaliser : INuciTextNormaliser
{
    private readonly INuciTextNormaliser _inner;
    private readonly IMetrics _metrics;

    public string NormaliseSentence(string input)
    {
        _metrics.Counter("normalisation_calls_total");
        _metrics.Histogram("normalisation_duration", () => Stopwatch.StartNew());

        var result = _inner.NormaliseSentence(input);

        _metrics.Counter("normalisation_success_total");
        return result;
    }
}
```

## Security considerations

### Concurrency security

- **No shared state** — No race conditions
- **No secrets** — No sensitive data in library
- **No side effects** — Pure function
- **No external calls** — No network, file, or process I/O

### Thread safety in consumer code

```csharp
// Safe: stateless library
var normaliser = new NuciTextNormaliser();
Parallel.ForEach(inputs, input =>
{
    var result = normaliser.NormaliseSentence(input);
});

// Unsafe: if consumer introduces shared state
// that is not thread-safe
```

## Summary

`NuciText.Normaliser` is inherently thread-safe due to:

1. **Stateless design** — No instance or static fields
2. **Immutable data** — `string` inputs and outputs
3. **Thread-safe BCL** — `Regex` is thread-safe
4. **No side effects** — Pure function

No special concurrency handling is required in the library itself. Consumers can safely use the normaliser in multi-threaded environments.