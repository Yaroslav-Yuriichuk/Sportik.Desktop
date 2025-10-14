using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Events;
using Sportik.Desktop.Core.Helpers;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Settings;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.Core.States.Sequential
{
    internal sealed class DisabledSequentialExerciseState : SequentialExerciseState
    {
        private readonly IEventsService _eventsService;
        private readonly Func<IExerciseSettingsService> _exerciseSettingsServiceFactory;

        public override States.SequentialExerciseState ExerciseState => States.SequentialExerciseState.Disabled;

        public DisabledSequentialExerciseState(SequentialExercisesStatesContext context, IEventsService eventsService,
            Func<IExerciseSettingsService> exerciseSettingsServiceFactory) : base(context)
        {
            _eventsService = eventsService;
            _exerciseSettingsServiceFactory = exerciseSettingsServiceFactory;
        }

        protected override void HandleEnter()
        {
            _eventsService.AddListener<ExerciseIsEnabledChangedEventArgs>(EventsService_Event);
        }

        protected override void HandleExit()
        {
            _eventsService.RemoveListener<ExerciseIsEnabledChangedEventArgs>(EventsService_Event);
        }

        private void EventsService_Event(ExerciseIsEnabledChangedEventArgs args)
        {
            if (!CompareHelper.EqualById(Context.Exercise, args.Exercise) || !args.IsEnabled)
            {
                return;
            }

            IExerciseSettingsService exerciseSettingsService = _exerciseSettingsServiceFactory();

            Task.Run(async () =>
            {
                IEnumerable<ExerciseSettings> exerciseSettings = await Task.WhenAll(
                    Context.Exercises.Select(async e =>
                        await exerciseSettingsService.GetExerciseSettingsAsync(e, ActiveCancellationToken)));

                Exercise otherExercise = ExercisesSequenceHelper.GetAnyOtherEnabledExercise(Context.Exercises, Context.Exercise, exerciseSettings);

                Context.Switch(otherExercise == null
                    ? Context.WaitingBeforeForceExecutionExerciseState
                    : Context.QueuedExerciseState);
            });
        }
    }
}
