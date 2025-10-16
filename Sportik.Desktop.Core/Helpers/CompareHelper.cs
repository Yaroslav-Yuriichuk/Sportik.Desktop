using System;
using Sportik.Desktop.Core.Models;

namespace Sportik.Desktop.Core.Helpers
{
    public static class CompareHelper
    {
        public static bool EqualById(Exercise first, Exercise second)
        {
            return first.Id == second.Id;
        }

        public static bool EqualById(Exercise exercise, Guid id)
        {
            return exercise.Id == id;
        }
    }
}
