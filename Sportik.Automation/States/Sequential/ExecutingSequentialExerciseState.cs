using System;
using Sportik.Automation.Helpers;
using System.Collections.Generic;
using System.Linq;
using Sportik.Automation.Models;
using Sportik.Automation.Services;
using Sportik.Core.Events;
using Sportik.Core.Helpers;
using Sportik.Core.Models;
using Sportik.Core.Models.Settings;
using Sportik.Core.Services.Interfaces;
using Sportik.Core.Timers;
using Sportik.Notification.Events;

namespace Sportik.Automation.States.Sequential
{
    internal class ExecutingSequentialExerciseState : SequentialExerciseState
    {
        private readonly IEventsService _eventsService;
        private readonly IExerciseTimersService _exerciseTimersService;
        private readonly Func<IExerciseSettingsService> _exerciseSettingsServiceFactory;

        public override States.SequentialExerciseState ExerciseState => States.SequentialExerciseState.Executing;

        public ExecutingSequentialExerciseState(SequentialExercisesStatesContext context, IEventsService eventsService, IExerciseTimersService exerciseTimersService,
            Func<IExerciseSettingsService> exerciseSettingsServiceFactory) : base(context)
        {
            _eventsService = eventsService;
            _exerciseTimersService = exerciseTimersService;
            _exerciseSettingsServiceFactory = exerciseSettingsServiceFactory;
        }

        public override void Enter()
        {
            IExerciseSettingsService exerciseSettingsService = _exerciseSettingsServiceFactory();

            _eventsService.AddListener<ExerciseIsEnabledChangedEventArgs>(EventsService_Event);
            _eventsService.AddListener<ExerciseExecutionTimeChangedEventArgs>(EventsService_Event);
            _eventsService.AddListener<ExerciseStatisticsDeltaAddedEventArgs>(EventsService_Event);
            _eventsService.AddListener<ReminderNotificationDismissedEventArgs>(EventsService_Event);

            ITimer timer = _exerciseTimersService.GetTimer(Context.Exercise, ReminderMode.Sequential);

            timer.Loop = false;
            timer.Interval = exerciseSettingsService.GetExerciseSettings(Context.Exercise).ExecutionTime;

            timer.Elapsed += Timer_Elapsed;

            if (!timer.IsRunning)
            {
                timer.Start();
            }
        }

        public override void Exit()
        {
            _eventsService.RemoveListener<ExerciseIsEnabledChangedEventArgs>(EventsService_Event);
            _eventsService.RemoveListener<ExerciseExecutionTimeChangedEventArgs>(EventsService_Event);
            _eventsService.RemoveListener<ExerciseStatisticsDeltaAddedEventArgs>(EventsService_Event);
            _eventsService.RemoveListener<ReminderNotificationDismissedEventArgs>(EventsService_Event);

            ITimer timer = _exerciseTimersService.GetTimer(Context.Exercise, ReminderMode.Parallel);

            timer.Elapsed -= Timer_Elapsed;

            if (timer.IsRunning)
            {
                timer.Stop();
            }
        }

        private void EventsService_Event(ExerciseIsEnabledChangedEventArgs args)
        {
            if (CompareHelper.EqualById(Context.Exercise, args.Exercise) && !args.IsEnabled)
            {
                IExerciseSettingsService exerciseSettingsService = _exerciseSettingsServiceFactory();

                IEnumerable<ExerciseSettings> exerciseSettings = Context.Exercises.Select(e => exerciseSettingsService.GetExerciseSettings(e));
                Exercise nextExercise = ExercisesSequenceHelper.GetNextEnabledExercise(Context.Exercises, Context.Exercise, exerciseSettings);

                Context.Switch(Context.DisabledExerciseState);
                
                if (nextExercise != null)
                {
                    SequentialExercisesStatesContext nextExerciseContext = Context.GetContext(nextExercise);
                    nextExerciseContext.Switch(nextExerciseContext.WaitingExerciseState);
                }
            }
        }

        private void EventsService_Event(ExerciseExecutionTimeChangedEventArgs args)
        {
            if (CompareHelper.EqualById(Context.Exercise, args.Exercise))
            {
                ITimer timer = _exerciseTimersService.GetTimer(Context.Exercise, ReminderMode.Sequential);
                timer.Interval = args.ExecutionTime;
            }
        }

        private void EventsService_Event(ExerciseStatisticsDeltaAddedEventArgs args)
        {
            if (CompareHelper.EqualById(Context.Exercise, args.Exercise))
            {
                IExerciseSettingsService exerciseSettingsService = _exerciseSettingsServiceFactory();

                IEnumerable<ExerciseSettings> exerciseSettings = Context.Exercises.Select(e => exerciseSettingsService.GetExerciseSettings(e));
                Exercise nextExercise = ExercisesSequenceHelper.GetNextEnabledExercise(Context.Exercises, Context.Exercise, exerciseSettings);

                if (nextExercise != null)
                {
                    Context.Switch(Context.QueuedExerciseState);

                    SequentialExercisesStatesContext nextExerciseContext = Context.GetContext(nextExercise);
                    nextExerciseContext.Switch(nextExerciseContext.WaitingExerciseState);
                }
                else
                {
                    Context.Switch(Context.WaitingExerciseState);
                }
            }
        }

        private void EventsService_Event(ReminderNotificationDismissedEventArgs args)
        {
            if (CompareHelper.EqualById(Context.Exercise, args.Exercise))
            {
                IExerciseSettingsService exerciseSettingsService = _exerciseSettingsServiceFactory();

                IEnumerable<ExerciseSettings> exerciseSettings = Context.Exercises.Select(e => exerciseSettingsService.GetExerciseSettings(e));
                Exercise nextExercise = ExercisesSequenceHelper.GetNextEnabledExercise(Context.Exercises, Context.Exercise, exerciseSettings);

                if (nextExercise != null)
                {
                    Context.Switch(Context.QueuedExerciseState);

                    SequentialExercisesStatesContext nextExerciseContext = Context.GetContext(nextExercise);
                    nextExerciseContext.Switch(nextExerciseContext.WaitingExerciseState);
                }
                else
                {
                    Context.Switch(Context.WaitingExerciseState);
                }
            }
        }

        private void Timer_Elapsed(object sender, EventArgs args)
        {
            IExerciseSettingsService exerciseSettingsService = _exerciseSettingsServiceFactory();

            IEnumerable<ExerciseSettings> exerciseSettings = Context.Exercises.Select(e => exerciseSettingsService.GetExerciseSettings(e));
            Exercise nextExercise = ExercisesSequenceHelper.GetNextEnabledExercise(Context.Exercises, Context.Exercise, exerciseSettings);

            if (nextExercise != null)
            {
                Context.Switch(Context.QueuedExerciseState);

                SequentialExercisesStatesContext nextExerciseContext = Context.GetContext(nextExercise);
                nextExerciseContext.Switch(nextExerciseContext.WaitingExerciseState);
            }
            else
            {
                Context.Switch(Context.WaitingExerciseState);
            }
        }
    }
}
