using System;
using Sportik.Desktop.Core.Models;

namespace Sportik.Desktop.UI.ViewModels.Extra
{
    internal class SetViewModel : ViewModel
    {
        private Exercise _exercise;

        public Exercise Exercise
        {
            get => _exercise;
            set => SetField(ref _exercise, value);
        }

        private int _repetitions;

        public int Repetitions
        {
            get => _repetitions;
            set => SetField(ref _repetitions, value);
        }

        private DateTimeOffset _time;

        public DateTimeOffset Time
        {
            get => _time;
            set => SetField(ref _time, value);
        }

        public SetViewModel(Exercise exercise, int repetitions, DateTimeOffset time)
        {
            Exercise = exercise;
            Repetitions = repetitions;
            Time = time;
        }
    }
}
