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

        private readonly Exercise _exercise;
        private readonly List<ExerciseSet> _exerciseSets;
        private readonly HashSet<Guid> _exerciseSetIds;

        public ExerciseStatisticsViewModel(ExerciseStatistics exerciseStatistics)
            : this(exerciseStatistics.Exercise, new List<ExerciseSet>(exerciseStatistics.Sets))
        {
        }

        private ExerciseStatisticsViewModel(Exercise exercise, List<ExerciseSet> sets)
        {
            _exercise = exercise;
            _exerciseSets = sets.ToList();
            _exerciseSetIds = sets.Select(set => set.Id).ToHashSet();

            ExerciseId = exercise.Id;
            Name = exercise.Name;
            Sets = sets.Count;
            Repetitions = sets.Aggregate(0, (sum, set) => sum + set.Repetitions);
        }

        public void Dispose() { }

        public void AddSet(ExerciseSet set)
        {
            if (set.ExerciseId != ExerciseId)
            {
                return;
            }

            if (_exerciseSetIds.Add(set.Id))
            {
                _exerciseSets.Add(set);

                Sets++;
                Repetitions += set.Repetitions;
            }
        }

        public ExerciseStatisticsViewModel Clone()
        {
            return new ExerciseStatisticsViewModel(_exercise, new List<ExerciseSet>(_exerciseSets));
        }

        public void Merge(ExerciseStatisticsViewModel other)
        {
            if (ExerciseId != other.ExerciseId)
            {
                return;
            }

            foreach (ExerciseSet set in other._exerciseSets)
            {
                AddSet(set);
            }
        }
    }
}
