using Soenneker.Extensions.DateTimeOffsets.Months;
using Soenneker.Extensions.DateTimeOffsets.Weeks;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Soenneker.Utils.DateTimeOffsets;

/// <summary>
/// Represents the date time offsets util.
/// </summary>
public static class DateTimeOffsetsUtil
{
    /// <summary>
    /// Builds a new <see cref="System.DateTimeOffset"/> instance representing a UTC date and time, 
    /// with optional year, month, day, hour, minute, and second parameters. If any of these parameters
    /// are not provided, the current UTC date and time values are used as defaults.
    /// </summary>
    /// <remarks>
    /// The current UTC date and time is used to fill in any parameters not provided. This approach ensures that the method
    /// is efficient by avoiding unnecessary null checks.
    /// </remarks>
    /// <param name="year">The year component of the date and time. Defaults to the current UTC year if null.</param>
    /// <param name="month">The month component of the date and time. Defaults to the current UTC month if null.</param>
    /// <param name="day">The day component of the date and time. Defaults to the current UTC day if null.</param>
    /// <param name="hour">The hour component of the date and time. Defaults to the current UTC hour if null.</param>
    /// <param name="minute">The minute component of the date and time. Defaults to the current UTC minute if null.</param>
    /// <param name="second">The second component of the date and time. Defaults to the current UTC second if null.</param>
    /// <returns>A <see cref="System.DateTimeOffset"/> object set to the specified date and time in UTC.</returns>
    [Pure]
    public static DateTimeOffset CreateUtcDateTimeOffset(int? year = null, int? month = null, int? day = null, int? hour = null, int? minute = null, int? second = null)
    {
        DateTimeOffset utcNow = DateTimeOffset.UtcNow;

        year ??= utcNow.Year;
        month ??= utcNow.Month;
        day ??= utcNow.Day;
        hour ??= utcNow.Hour;
        minute ??= utcNow.Minute;
        second ??= utcNow.Second;

        return new DateTimeOffset(year.Value, month.Value, day.Value, hour.Value, minute.Value, second.Value, TimeSpan.Zero);
    }

    /// <summary>
    /// Builds a new <see cref="System.DateTimeOffset"/> instance representing a date and time 
    /// adjusted to a specific time zone, with optional year, month, day, hour, minute, and second parameters.
    /// If any parameter is not provided, the current UTC date and time values are used as defaults, 
    /// and then converted to the specified time zone.
    /// </summary>
    /// <remarks>
    /// It leverages the <see cref="CreateUtcDateTimeOffset"/> method to create a UTC <see cref="System.DateTimeOffset"/> instance, which is
    /// then adjusted to the specified time zone using the <see cref="TimeZoneInfo"/> parameter.
    /// </remarks>
    /// <param name="timeZoneInfo">The <see cref="TimeZoneInfo"/> representing the target time zone for the date and time.</param>
    /// <param name="year">The year component of the date and time. Defaults to the current UTC year if null.</param>
    /// <param name="month">The month component of the date and time. Defaults to the current UTC month if null.</param>
    /// <param name="day">The day component of the date and time. Defaults to the current UTC day if null.</param>
    /// <param name="hour">The hour component of the date and time. Defaults to the current UTC hour if null.</param>
    /// <param name="minute">The minute component of the date and time. Defaults to the current UTC minute if null.</param>
    /// <param name="second">The second component of the date and time. Defaults to the current UTC second if null.</param>
    /// <returns>A <see cref="System.DateTimeOffset"/> object set to the specified date and time, adjusted to the specified time zone.</returns>
    [Pure]
    public static DateTimeOffset CreateTzDateTimeOffset(TimeZoneInfo timeZoneInfo, int? year = null, int? month = null, int? day = null, int? hour = null, int? minute = null, int? second = null)
    {
        DateTimeOffset utcDateTimeOffset = CreateUtcDateTimeOffset(year, month, day, hour, minute, second);
        DateTime localDateTime = new DateTime(utcDateTimeOffset.Year, utcDateTimeOffset.Month, utcDateTimeOffset.Day, utcDateTimeOffset.Hour, utcDateTimeOffset.Minute, utcDateTimeOffset.Second, DateTimeKind.Unspecified);
        DateTime utcResult = TimeZoneInfo.ConvertTimeToUtc(localDateTime, timeZoneInfo);
        return new DateTimeOffset(utcResult, TimeSpan.Zero);
    }

    /// <summary>
    /// Generates a list of weekly date ranges between the specified start and end dates, based on the given time zone.
    /// </summary>
    /// <param name="startAt">The start date and time.</param>
    /// <param name="endAt">The end date and time.</param>
    /// <param name="timeZoneInfo">The time zone to consider for week calculations.</param>
    /// <returns>
    /// A list of tuples, each containing a weekly start and end date within the specified range.
    /// </returns>
    /// <remarks>
    /// The method ensures that each week starts and ends according to the specified time zone's week start.
    /// </remarks>
    [Pure]
    public static List<(DateTimeOffset startAt, DateTimeOffset endAt)> GetWeeklyDateTimeOffsetsBetween(DateTimeOffset startAt, DateTimeOffset endAt, TimeZoneInfo timeZoneInfo)
    {
        var result = new List<(DateTimeOffset startAt, DateTimeOffset endAt)>();

        DateTimeOffset startDate = startAt.ToStartOfTzWeek(timeZoneInfo);
        DateTimeOffset endDate = startDate.ToEndOfTzWeek(timeZoneInfo);

        result.Add((startDate, endDate));

        while (endDate < endAt)
        {
            startDate = startDate.AddDays(7);
            endDate = endDate.AddDays(7);

            result.Add((startDate, endDate));
        }

        return result;
    }

    /// <summary>
    /// Generates a list of monthly date ranges between the specified start and end dates, based on the given time zone.
    /// </summary>
    /// <param name="startAt">The start date and time.</param>
    /// <param name="endAt">The end date and time.</param>
    /// <param name="timeZoneInfo">The time zone to consider for month calculations.</param>
    /// <returns>
    /// A list of tuples, each containing a monthly start and end date within the specified range.
    /// </returns>
    /// <remarks>
    /// The method ensures that each month starts and ends according to the specified time zone's month start.
    /// </remarks>
    [Pure]
    public static List<(DateTimeOffset startAt, DateTimeOffset endAt)> GetMonthlyDateTimeOffsetsBetween(DateTimeOffset startAt, DateTimeOffset endAt, TimeZoneInfo timeZoneInfo)
    {
        var result = new List<(DateTimeOffset startAt, DateTimeOffset endAt)>();

        DateTimeOffset startDate = startAt.ToStartOfTzMonth(timeZoneInfo);
        DateTimeOffset endDate = startDate.ToEndOfTzMonth(timeZoneInfo);

        result.Add((startDate, endDate));

        while (endDate < endAt)
        {
            startDate = startDate.AddMonths(1);
            endDate = endDate.AddMonths(1);

            result.Add((startDate, endDate));
        }

        return result;
    }
}
