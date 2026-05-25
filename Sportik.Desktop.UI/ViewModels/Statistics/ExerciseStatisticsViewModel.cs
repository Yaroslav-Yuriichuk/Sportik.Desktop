using System;
using System.Collections.Generic;
using System.Linq;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Statistics;

namespace Sportik.Desktop.UI.ViewModels.Statistics
{
    internal sealed class ExerciseStatisticsViewModel : ViewModel, IDisposable
    {
        private string _name;

        public string Name
        {
            get => _name;
            private set => SetField(ref _name, value);
        }

        private int _sets;

        public int Sets
        {
            get => _sets;
            private set => SetField(ref _sets, value);
        }

        private int _repetitions;

        public int Repetitions
        {
            get => _repetitions;
            private set => SetField(ref _repetitions, value);
        }

        public Guid ExerciseId { get; }

        private readonly HashSet<Guid> _setIds;

        public ExerciseStatisticsViewModel(ExerciseStatistics exerciseStatistics)
        {
            ExerciseId = exerciseStatistics.Exercise.Id;
            _setIds = exerciseStatistics.Sets.Select(set => set.Id).ToHashSet();

            Name = exerciseStatistics.Exercise.Name;
            Sets = exerciseStatistics.Sets.Count;
            Repetitions = exerciseStatistics.Sets.Aggregate(0, (sum, set) => sum + set.Repetitions);
        }

        public void Dispose() { }

        public void AddSet(ExerciseSet set)
        {
            if (set.ExerciseId != ExerciseId)
            {
                return;
            }

            if (_setIds.Add(set.Id))
            {
                Sets++;
                Repetitions += set.Repetitions;
            }
        }
    }
}
