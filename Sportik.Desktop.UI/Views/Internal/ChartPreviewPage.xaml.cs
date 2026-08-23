using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Controls;

namespace Sportik.Desktop.UI.Views.Internal
{
    public sealed partial class ChartPreviewPage : Page
    {
        public IReadOnlyList<ChartPreviewDataPoint> SampleData { get; } = new List<ChartPreviewDataPoint>
        {
            new ChartPreviewDataPoint { Label = "Mon", Count = 4, },
            new ChartPreviewDataPoint { Label = "Tue", Count = 7, },
            new ChartPreviewDataPoint { Label = "Wed", Count = 3, },
            new ChartPreviewDataPoint { Label = "Thu", Count = 8, },
            new ChartPreviewDataPoint { Label = "Fri", Count = 6, },
            new ChartPreviewDataPoint { Label = "Sat", Count = 2, },
            new ChartPreviewDataPoint { Label = "Sun", Count = 5, },
        };

        public int TotalSets => SampleData.Sum(x => x.Count);

        public string BestDayLabel => SampleData.OrderByDescending(x => x.Count).FirstOrDefault()?.Label ?? "-";

        public ChartPreviewPage()
        {
            this.InitializeComponent();
        }
    }

    public sealed class ChartPreviewDataPoint
    {
        public string Label { get; set; }

        public int Count { get; set; }
    }
}
