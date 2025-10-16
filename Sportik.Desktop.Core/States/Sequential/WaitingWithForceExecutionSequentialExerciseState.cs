using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Common.Timers;
using Sportik.Desktop.Core.Events;
using Sportik.Desktop.Core.Helpers;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Automation;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.Core.States.Sequential
{
    internal sealed class WaitingWithForceExecutionSequentialExerciseState : SequentialExerciseState
    {
        private readonly IEventsService _eventsService;
        private readonly IExerciseTimersService _exerciseTimersService;
        private readonly Func<IExercisesService> _exercisesServiceFactory;
        private readonly Func<INotificationService> _notificationServiceFactory;

        public override States.SequentialExerciseState ExerciseState => States.SequentialExerciseState.WaitingWithForceExecution;

        public WaitingWithForceExecutionSequentialExerciseState(SequentialExercisesStatesContext context, IEventsService eventsService, IExerciseTimersService exerciseTimersService,
            Func<IExercisesService> exercisesServiceFactory, Func<INotificationService> notificationServiceFactory) : base(context)
        {
            _eventsService = eventsService;
            _exerciseTimersService = exerciseTimersService;
            _exercisesServiceFactory = exercisesServiceFactory;
            _notificationServiceFactory = notificationServiceFactory;
        }

        protected override void HandleEnter()
        {
            IExercisesService exercisesService = _exercisesServiceFactory();

            _eventsService.AddListener<ExerciseIsEnabledChangedEventArgs>(EventsService_Event);
            _eventsService.AddListener<ExerciseTimeBetweenSetsChangedEventArgs>(EventsService_Event);
            _eventsService.AddListener<ExerciseForceExecutionRequestedEventArgs>(EventsService_Event);

            Task.Run(async () =>
            {
                Exercise exercise = await exercisesService.GetByIdAsync(Context.ExerciseId, ActiveCancellationToken);

                ITimer timer = _exerciseTimersService.GetTimer(Context.ExerciseId, ReminderMode.Sequential);

                timer.Loop = false;
                timer.Interval = exercise.Settings.TimeBetweenSets;

                timer.Elapsed += Timer_Elapsed;

                if (timer.IsPaused)
                {
                    timer.Resume();
                }
                else
                {
                    timer.Start();
                }
            });
        }

        protected override void HandleExit()
        {
            _eventsService.RemoveListener<ExerciseIsEnabledChangedEventArgs>(EventsService_Event);
            _eventsService.RemoveListener<ExerciseTimeBetweenSetsChangedEventArgs>(EventsService_Event);
            _eventsService.RemoveListener<ExerciseForceExecutionRequestedEventArgs>(EventsService_Event);

            ITimer timer = _exerciseTimersService.GetTimer(Context.ExerciseId, ReminderMode.Sequential);

            timer.Elapsed -= Timer_Elapsed;

            if (timer.IsRunning)
            {
                timer.Pause();
            }
        }

        private void EventsService_Event(ExerciseIsEnabledChangedEventArgs args)
        {
            if (args.ExerciseId != Context.ExerciseId || args.IsEnabled)
            {
                return;
            }

            IExercisesService exercisesService = _exercisesServiceFactory();

            Task.Run(async () =>
            {
                IEnumerable<Exercise> exercises = await exercisesService.GetByIdsAsync(Context.ExerciseIds, ActiveCancellationToken);

                Exercise nextExercise = ExercisesSequenceHelper.GetNextEnabledExercise(exercises, Context.ExerciseId);

                Context.Switch(Context.DisabledExerciseState);

                if (nextExercise != null)
                {
                    SequentialExercisesStatesContext nextExerciseContext = Context.GetContext(nextExercise.Id);
                    nextExerciseContext.Switch(nextExerciseContext.WaitingBeforeForceExecutionExerciseState);
                }
            });
        }

        private void EventsService_Event(ExerciseTimeBetweenSetsChangedEventArgs args)
        {
            if (args.ExerciseId == Context.ExerciseId)
            {
                ITimer timer = _exerciseTimersService.GetTimer(Context.ExerciseId, ReminderMode.Sequential);
                timer.Interval = args.TimeBetweenSets;
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
                Exercise exercise = await exercisesService.GetByIdAsync(Context.ExerciseId, ActiveCancellationToken);

                notificationService.ShowReminder(Context.ExerciseId, new ReminderNotification
                {
                    Title = $"{exercise.Name} reminder!",
                    Texts = new[]
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
