using System;
using System.Collections.Generic;

namespace Sportik.Desktop.Automation.Constants
{
    public static class AutomationConstants
    {
        public static readonly IEnumerable<TimeSpan> TimesBetweenSets = new[]
        {
            TimeSpan.FromMinutes(1),
            TimeSpan.FromMinutes(3),
            TimeSpan.FromMinutes(5),
            TimeSpan.FromMinutes(10),
            TimeSpan.FromMinutes(15),
            TimeSpan.FromMinutes(20),
            TimeSpan.FromMinutes(25),
            TimeSpan.FromMinutes(30),
            TimeSpan.FromMinutes(35),
            TimeSpan.FromMinutes(40),
            TimeSpan.FromMinutes(45),
        };

        public static readonly IEnumerable<int> TargetRepetitions = new[]
        {
            5,
            7,
            10,
            12,
            15,
            18,
            20,
            21,
            22,
            25,
            30,
            35,
            40,
            45,
            50,
        };

        public static readonly IEnumerable<TimeSpan> ExecutionTimes = new[]
        {
            TimeSpan.FromMinutes(1),
            TimeSpan.FromMinutes(2),
            TimeSpan.FromMinutes(3),
            TimeSpan.FromMinutes(4),
            TimeSpan.FromMinutes(5),
        };

        public static readonly TimeSpan TimeBeforeForceExecution = TimeSpan.FromSeconds(15);
    }
}
