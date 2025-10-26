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

        private DateTimeOffset _date;

        public DateTimeOffset Date
        {
            get => _date;
            set => SetField(ref _date, value);
        }

        public SetViewModel(Exercise exercise, int repetitions, DateTimeOffset date)
        {
            Exercise = exercise;
            Repetitions = repetitions;
            Date = date;
        }
    }
}
