using System;
using Sportik.UWP.Core;
using Sportik.Core.Events;
using Sportik.Core.Services.Interfaces;
using Sportik.Core.Helpers;
using Sportik.Automation.Services;
using Sportik.Automation.States;

namespace Sportik.UWP.Services.Reminders.States
{
    internal sealed class ExecutingExerciseState : ExerciseState
    {
        private readonly IEventsService _eventsService;
        private readonly IExerciseTimersService _exerciseTimersService;
        private readonly IExerciseSettingsService _exerciseSettingsService;

        public override ExerciseStateKind Kind => ExerciseStateKind.Executing;

        public ExecutingExerciseState(ExerciseStatesContext context, IEventsService eventsService,
            IExerciseTimersService exerciseTimersService, IExerciseSettingsService exerciseSettingsService) : base(context)
        {
            _eventsService = eventsService;
            _exerciseTimersService = exerciseTimersService;
            _exerciseSettingsService = exerciseSettingsService;
        }

        public override void Enter()
        {
            _eventsService.AddListener<ExerciseIsEnabledChangedEventArgs>(EventsService_Event);
            _eventsService.AddListener<ExerciseExecutionTimeChangedEventArgs>(EventsService_Event);
            _eventsService.AddListener<ExerciseStatisticsDeltaAddedEventArgs>(EventsService_Event);

            ITimer timer = _exerciseTimersService.GetTimer(Context.Exercise);

            timer.Loop = false;
            timer.Interval = _exerciseSettingsService.GetExerciseSettings(Context.Exercise).ExecutionTime;

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

            ITimer timer = _exerciseTimersService.GetTimer(Context.Exercise);

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
                ITimer timer = _exerciseTimersService.GetTimer(Context.Exercise);
                timer.Interval = args.ExecutionTime;
            }
        }

        private void EventsService_Event(ExerciseStatisticsDeltaAddedEventArgs args)
        {
            if (CompareHelper.EqualById(Context.Exercise, args.Exercise))
            {
                Context.Switch(Context.WaitingExerciseState);
            }
        }

        private void Timer_Elapsed(object sender, EventArgs e)
        {
            Context.Switch(Context.WaitingExerciseState);
        }
    }
}
