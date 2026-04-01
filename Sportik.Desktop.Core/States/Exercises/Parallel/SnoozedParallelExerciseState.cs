using System;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Common;
using Sportik.Desktop.Core.Common.Timers;
using Sportik.Desktop.Core.Constants;
using Sportik.Desktop.Core.Events;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Automation;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.Core.States.Exercises.Parallel
{
    internal sealed class SnoozedParallelExerciseState : ParallelExerciseState
    {
        private readonly IEventsService _eventsService;
        private readonly IExerciseTimersService _exerciseTimersService;
        private readonly Func<IExercisesService> _exercisesServiceFactory;
        private readonly Func<INotificationService> _notificationServiceFactory;

        public override Exercises.ParallelExerciseState ExerciseState => Exercises.ParallelExerciseState.Snoozed;

        public SnoozedParallelExerciseState(ParallelExerciseStatesContext context, IEventsService eventsService,
            IExerciseTimersService exerciseTimersService, Func<IExercisesService> exercisesServiceFactory,
            Func<INotificationService> notificationServiceFactory) : base(context)
        {
            _eventsService = eventsService;
            _exerciseTimersService = exerciseTimersService;
            _exercisesServiceFactory = exercisesServiceFactory;
            _notificationServiceFactory = notificationServiceFactory;
        }

        protected override void HandleEnter()
        {
            _eventsService.AddListener<ExerciseIsEnabledChangedEventArgs>(EventsService_Event);
            _eventsService.AddListener<ExerciseForceExecutionRequestedEventArgs>(EventsService_Event);

            ITimer timer = _exerciseTimersService.GetTimer(Context.ExerciseId, ReminderMode.Parallel);

            timer.Loop = false;
            timer.Interval = AutomationConstants.SnoozingTime;

            timer.Elapsed += Timer_Elapsed;

            if (!timer.IsRunning)
            {
                timer.Start();
            }
        }

        protected override void HandleExit()
        {
            _eventsService.RemoveListener<ExerciseIsEnabledChangedEventArgs>(EventsService_Event);
            _eventsService.RemoveListener<ExerciseForceExecutionRequestedEventArgs>(EventsService_Event);

            ITimer timer = _exerciseTimersService.GetTimer(Context.ExerciseId, ReminderMode.Parallel);

            timer.Elapsed -= Timer_Elapsed;

            if (timer.IsRunning)
            {
                timer.Stop();
            }
        }

        private void EventsService_Event(ExerciseIsEnabledChangedEventArgs args)
        {
            if (args.ExerciseId == Context.ExerciseId && !args.IsEnabled)
            {
                Context.Switch(Context.DisabledExerciseState);
            }
        }

        private void EventsService_Event(ExerciseForceExecutionRequestedEventArgs args)
        {
            if (args.ExerciseId == Context.ExerciseId)
            {
                Context.Switch(Context.ExecutingExerciseState);
            }
        }

        private void Timer_Elapsed(object sender, EventArgs args)
        {
            IExercisesService exercisesService = _exercisesServiceFactory();
            INotificationService notificationService = _notificationServiceFactory();

            Task.Run(async () =>
            {
                OperationResult<Exercise> result = await exercisesService.GetByIdAsync(Context.ExerciseId, ActiveCancellationToken);

                if (!result.Succeeded)
                {
                    // TODO: Handle error.
                    return;
                }

                Exercise exercise = result.Value;

                notificationService.ShowReminder(Context.ExerciseId, new ReminderNotification
                {
                    Title = $"{exercise.Name} reminder!",
                    Texts = new []
                    {
                        $"You have {exercise.Settings.ExecutionTime.TotalMinutes} minutes to complete {exercise.Name.ToLower()} exercise.",
                        $"Target repetitions: {exercise.Settings.TargetRepetitions}.",
                    },
                    ExpirationTime = exercise.Settings.ExecutionTime,
                });

                Context.Switch(Context.ExecutingExerciseState);
            });
        }
    }
}