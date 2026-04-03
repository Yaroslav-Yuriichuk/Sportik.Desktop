using System;
using System.Collections.Generic;
using System.Linq;

namespace Sportik.Desktop.Core.Constants
{
    public static class AutomationConstants
    {
        public static readonly IEnumerable<TimeSpan> TimesBetweenSets = Enumerable
            .Range(1, 120)
            .Select(x => TimeSpan.FromMinutes(x))
            .ToArray();

        public static readonly IEnumerable<int> TargetRepetitions = Enumerable
            .Range(1, 100)
            .ToArray();

        public static readonly IEnumerable<TimeSpan> ExecutionTimes = Enumerable
            .Range(1, 10)
            .Select(x => TimeSpan.FromMinutes(x))
            .ToArray();

        public static readonly TimeSpan TimeBeforeForceExecution = TimeSpan.FromSeconds(15);

        public static readonly TimeSpan SnoozingTime = TimeSpan.FromMinutes(1);
    }
}
