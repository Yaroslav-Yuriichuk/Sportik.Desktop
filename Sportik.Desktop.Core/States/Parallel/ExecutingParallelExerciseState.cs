using System;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Common.Timers;
using Sportik.Desktop.Core.Events;
using Sportik.Desktop.Core.Helpers;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Automation;
using Sportik.Desktop.Core.Models.Settings;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.Core.States.Parallel
{
    internal sealed class ExecutingParallelExerciseState : ParallelExerciseState
    {
        private readonly IEventsService _eventsService;
        private readonly IExerciseTimersService _exerciseTimersService;
        private readonly Func<IExerciseSettingsService> _exerciseSettingsServiceFactory;

        public override States.ParallelExerciseState ExerciseState => States.ParallelExerciseState.Executing;

        public ExecutingParallelExerciseState(ParallelExerciseStatesContext context, IEventsService eventsService, IExerciseTimersService exerciseTimersService,
            Func<IExerciseSettingsService> exerciseSettingsServiceFactory) : base(context)
        {
            _eventsService = eventsService;
            _exerciseTimersService = exerciseTimersService;
            _exerciseSettingsServiceFactory = exerciseSettingsServiceFactory;
        }

        protected override void HandleEnter()
        {
            IExerciseSettingsService exerciseSettingsService = _exerciseSettingsServiceFactory();

            _eventsService.AddListener<ExerciseIsEnabledChangedEventArgs>(EventsService_Event);
            _eventsService.AddListener<ExerciseExecutionTimeChangedEventArgs>(EventsService_Event);
            _eventsService.AddListener<ExerciseCompleteRequestedEventArgs>(EventsService_Event);
            _eventsService.AddListener<ReminderNotificationDismissedEventArgs>(EventsService_Event);

            Task.Run(async () =>
            {
                ExerciseSettings exerciseSettings =
                    await exerciseSettingsService.GetExerciseSettingsAsync(Context.Exercise, ActiveCancellationToken);

                ITimer timer = _exerciseTimersService.GetTimer(Context.Exercise, ReminderMode.Parallel);

                timer.Loop = false;
                timer.Interval = exerciseSettings.ExecutionTime;

                timer.Elapsed += Timer_Elapsed;

                if (!timer.IsRunning)
                {
                    timer.Start();
                }
            });
        }

        protected override void HandleExit()
        {
            _eventsService.RemoveListener<ExerciseIsEnabledChangedEventArgs>(EventsService_Event);
            _eventsService.RemoveListener<ExerciseExecutionTimeChangedEventArgs>(EventsService_Event);
            _eventsService.RemoveListener<ExerciseCompleteRequestedEventArgs>(EventsService_Event);
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
                Context.Switch(Context.DisabledExerciseState);
            }
        }

        private void EventsService_Event(ExerciseExecutionTimeChangedEventArgs args)
        {
            if (CompareHelper.EqualById(Context.Exercise, args.Exercise))
            {
                ITimer timer = _exerciseTimersService.GetTimer(Context.Exercise, ReminderMode.Parallel);
                timer.Interval = args.ExecutionTime;
            }
        }

        private void EventsService_Event(ExerciseCompleteRequestedEventArgs args)
        {
            if (CompareHelper.EqualById(Context.Exercise, args.Exercise))
            {
                Context.Switch(Context.WaitingBeforeForceExecutionExerciseState);
            }
        }

        private void EventsService_Event(ReminderNotificationDismissedEventArgs args)
        {
            if (CompareHelper.EqualById(Context.Exercise, args.Exercise))
            {
                Context.Switch(Context.WaitingBeforeForceExecutionExerciseState);
            }
        }

        private void Timer_Elapsed(object sender, EventArgs args)
        {
            Context.Switch(Context.WaitingBeforeForceExecutionExerciseState);
        }
    }
}
