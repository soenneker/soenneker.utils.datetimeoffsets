[![](https://img.shields.io/nuget/v/soenneker.utils.datetimeoffsets.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.utils.datetimeoffsets/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.utils.datetimeoffsets/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.utils.datetimeoffsets/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.utils.datetimeoffsets.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.utils.datetimeoffsets/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.utils.datetimeoffsets/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.utils.datetimeoffsets/actions/workflows/codeql.yml)

# Soenneker.Utils.DateTimeOffsets

Static helpers for constructing UTC `DateTimeOffset` values and splitting ranges into timezone-aligned weeks or months.

## Installation

```bash
dotnet add package Soenneker.Utils.DateTimeOffsets
```

## Construct timestamps

`CreateUtcDateTimeOffset()` builds a value with a zero UTC offset. Any omitted component is taken from one snapshot of `DateTimeOffset.UtcNow`:

```csharp
DateTimeOffset start = DateTimeOffsetsUtil.CreateUtcDateTimeOffset(
    year: 2026,
    month: 4,
    day: 15,
    hour: 9,
    minute: 30,
    second: 0);
```

`CreateTzDateTimeOffset()` interprets the components as wall-clock time in a timezone and returns the corresponding instant with a zero UTC offset:

```csharp
TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("America/Chicago");

DateTimeOffset utc = DateTimeOffsetsUtil.CreateTzDateTimeOffset(
    zone,
    year: 2026,
    month: 4,
    day: 15,
    hour: 9,
    minute: 30,
    second: 0);
```

Omitted components use the current wall-clock values in that timezone. Standard `TimeZoneInfo` rules apply: invalid local times during a daylight-saving transition throw, and ambiguous local times use the platform's normal conversion behavior.

## Calendar ranges

```csharp
List<(DateTimeOffset startAt, DateTimeOffset endAt)> weeks =
    DateTimeOffsetsUtil.GetWeeklyDateTimeOffsetsBetween(startAt, endAt, zone);

List<(DateTimeOffset startAt, DateTimeOffset endAt)> months =
    DateTimeOffsetsUtil.GetMonthlyDateTimeOffsetsBetween(startAt, endAt, zone);
```

Each method first expands `startAt` to the timezone-aligned start and end of its containing calendar period, then adds complete local-calendar periods until one contains `endAt`. Results are UTC instants, and their UTC boundary time can change across daylight-saving transitions while still representing the same local boundary.

The returned ranges are calendar buckets rather than intersections with the input range, so the first start may precede `startAt` and the final end may follow `endAt`. Both methods return at least the period containing `startAt`; validate range ordering before calling when an inverted range should be rejected.
