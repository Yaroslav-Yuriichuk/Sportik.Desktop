using System;
using System.Collections.Generic;
using System.Linq;

namespace Sportik.Desktop.UI.Constants
{
    public static class DashboardConstants
    {
        public static readonly IEnumerable<int> Years = Enumerable
            .Range(2000, DateTime.Now.Year - 2000 + 1)
            .Reverse()
            .ToArray();
    }
}