using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Sportik.Desktop.Core.Events;
using Sportik.Desktop.Core.Models.Statistics;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.UI.ViewModels.Statistics
{
    internal sealed class ExerciseStatisticsViewModel : ViewModel, IDisposable
    {
        private string _name;

        public string Name
        {
            get => _name;
            set => SetField(ref _name, value);
        }

        private int _sets;

        public int Sets
        {
            get => _sets;
            set => SetField(ref _sets, value);
        }

        private int _repetitions;

        public int Repetitions
        {
            get => _repetitions;
            set => SetField(ref _repetitions, value);
        }

        private readonly Guid _exerciseId;
        private readonly DateTime _date;
        private HashSet<Guid> _setIds;

        private IEventsService EventsService => App.ServiceProvider.GetRequiredService<IEventsService>();

        public ExerciseStatisticsViewModel(ExerciseStatistics exerciseStatistics, DateTime date)
        {
            _exerciseId = exerciseStatistics.Exercise.Id;
            _date = date;
            _setIds = exerciseStatistics.Sets.Select(set => set.Id).ToHashSet();

            Name = exerciseStatistics.Exercise.Name;
            Sets = exerciseStatistics.Sets.Count;
            Repetitions = exerciseStatistics.Sets.Aggregate(0, (sum, set) => sum + set.Repetitions);

            EventsService.AddListener<ExerciseSetAddedEventArgs>(EventsService_Event);
        }

        public void Dispose()
        {
            EventsService.RemoveListener<ExerciseSetAddedEventArgs>(EventsService_Event);
        }

        private void EventsService_Event(ExerciseSetAddedEventArgs args)
        {
            if (args.Set.ExerciseId != _exerciseId)
            {
                return;
            }

            TimeSpan offset = TimeZoneInfo.Local.GetUtcOffset(DateTimeOffset.UtcNow);
            DateTime date = args.Set.LoggedAt.ToOffset(offset).Date;

            if (date == _date && _setIds.Add(args.Set.Id))
            {
                Sets++;
                Repetitions += args.Set.Repetitions;
            }
        }
    }
}
