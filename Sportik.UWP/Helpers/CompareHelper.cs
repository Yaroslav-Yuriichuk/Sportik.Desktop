using Sportik.UWP.Models;

namespace Sportik.UWP.Helpers
{
    internal static class CompareHelper
    {
        public static bool EqualById(Exercise first, Exercise second)
        {
            return first.Id == second.Id;
        }
    }
}
