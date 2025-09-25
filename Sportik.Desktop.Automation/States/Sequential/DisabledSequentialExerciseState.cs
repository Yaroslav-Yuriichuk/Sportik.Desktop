using System;
using System.Collections.Generic;
using System.Linq;
using Sportik.Desktop.Automation.Helpers;
using Sportik.Desktop.Core.Events;
using Sportik.Desktop.Core.Helpers;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Settings;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.Automation.States.Sequential
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

        public override void Enter()
        {
            _eventsService.AddListener<ExerciseIsEnabledChangedEventArgs>(EventsService_Event);
        }

        public override void Exit()
        {
            _eventsService.RemoveListener<ExerciseIsEnabledChangedEventArgs>(EventsService_Event);
        }

        private void EventsService_Event(ExerciseIsEnabledChangedEventArgs args)
        {
            if (CompareHelper.EqualById(Context.Exercise, args.Exercise) && args.IsEnabled)
            {
                IExerciseSettingsService exerciseSettingsService = _exerciseSettingsServiceFactory();

                IEnumerable<ExerciseSettings> exerciseSettings = Context.Exercises.Select(e => exerciseSettingsService.GetExerciseSettings(e));
                Exercise otherExercise = ExercisesSequenceHelper.GetAnyOtherEnabledExercise(Context.Exercises, Context.Exercise, exerciseSettings);

                if (otherExercise == null)
                {
                    Context.Switch(Context.WaitingBeforeForceExecutionExerciseState);
                }
                else
                {
                    Context.Switch(Context.QueuedExerciseState);
                }
            }
        }
    }
}
