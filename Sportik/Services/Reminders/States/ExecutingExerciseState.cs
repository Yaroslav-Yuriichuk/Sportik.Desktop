using Microsoft.Extensions.DependencyInjection;
using Sportik.Core;
using Sportik.Helpers;
using Sportik.Services.Events;
using Sportik.Services.Settings;
using System;
using Sportik.Services.Statistics;

namespace Sportik.Services.Reminders.States
{
    internal sealed class ExecutingExerciseState : ExerciseState
    {
        private IEventsService EventsService => App.ServiceProvider.GetService<IEventsService>();
        private IExerciseTimersService ExerciseTimersService => App.ServiceProvider.GetService<IExerciseTimersService>();
        private IExerciseSettingsService ExerciseSettingsService => App.ServiceProvider.GetService<IExerciseSettingsService>();

        public override ExerciseStateKind Kind => ExerciseStateKind.Executing;

        public ExecutingExerciseState(ExerciseStatesContext context) : base(context) { }

        public override void Enter()
        {
            EventsService.Event += EventsService_Event;

            ITimer timer = ExerciseTimersService.GetTimer(Context.Exercise);

            timer.Loop = false;
            timer.Interval = ExerciseSettingsService.GetExerciseSettings(Context.Exercise).ExecutionTime;

            timer.Elapsed += Timer_Elapsed;

            if (!timer.IsRunning)
            {
                timer.Start();
            }
        }

        public override void Exit()
        {
            EventsService.Event -= EventsService_Event;

            ITimer timer = ExerciseTimersService.GetTimer(Context.Exercise);

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
                    ITimer timer = ExerciseTimersService.GetTimer(Context.Exercise);
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
