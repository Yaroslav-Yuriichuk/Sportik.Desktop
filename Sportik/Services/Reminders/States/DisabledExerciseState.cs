using System;
using Microsoft.Extensions.DependencyInjection;
using Sportik.Helpers;
using Sportik.Services.Events;
using Sportik.Services.Settings;

namespace Sportik.Services.Reminders.States
{
    internal class DisabledExerciseState : ExerciseState
    {
        private IEventsService EventsService => App.ServiceProvider.GetService<IEventsService>();

        public override ExerciseStateKind Kind => ExerciseStateKind.Disabled;

        public DisabledExerciseState(ExerciseStatesContext context) : base(context) { }

        public override void Enter()
        {
            EventsService.Event += EventsService_Event;
        }

        public override void Exit()
        {
            EventsService.Event -= EventsService_Event;
        }

        private void EventsService_Event(EventArgs args)
        {
            if (args is ExerciseIsEnabledChangedEventArgs changedEventArgs)
            {
                if (CompareHelper.EqualById(Context.Exercise, changedEventArgs.Exercise) && changedEventArgs.IsEnabled)
                {
                    Context.Switch(Context.WaitingExerciseState);
                }
            }
        }
    }
}
