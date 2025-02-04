using Sportik.Core.Models;

namespace Sportik.Core.Helpers
{
    public static class CompareHelper
    {
        public static bool EqualById(Exercise first, Exercise second)
        {
            return first.Id == second.Id;
        }
    }
}
