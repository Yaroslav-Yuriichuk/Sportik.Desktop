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
            _eventsService.Event += EventsService_Event;

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
            _eventsService.Event -= EventsService_Event;

            ITimer timer = _exerciseTimersService.GetTimer(Context.Exercise);

            timer.Elapsed -= Timer_Elapsed;

            if (timer.IsRunning)
            {
                timer.Stop();
            }
        }

        private void EventsService_Event(EventArgs args)
        {
            if (args is ExerciseIsEnabledChangedEventArgs isEnabledChangedEventArgs)
            {
                if (CompareHelper.EqualById(Context.Exercise, isEnabledChangedEventArgs.Exercise) && !isEnabledChangedEventArgs.IsEnabled)
                {
                    Context.Switch(Context.DisabledExerciseState);
                }
            }

            if (args is ExerciseExecutionTimeChangedEventArgs executionTimeChangedEventArgs)
            {
                if (CompareHelper.EqualById(Context.Exercise, executionTimeChangedEventArgs.Exercise))
                {
                    ITimer timer = _exerciseTimersService.GetTimer(Context.Exercise);
                    timer.Interval = executionTimeChangedEventArgs.ExecutionTime;
                }
            }

            if (args is ExerciseStatisticsDeltaAddedEventArgs statisticsDeltaAddedEventArgs)
            {
                if (CompareHelper.EqualById(Context.Exercise, statisticsDeltaAddedEventArgs.Exercise))
                {
                    Context.Switch(Context.WaitingExerciseState);
                }
            }
        }

        private void Timer_Elapsed(object sender, EventArgs e)
        {
            Context.Switch(Context.WaitingExerciseState);
        }
    }
}
