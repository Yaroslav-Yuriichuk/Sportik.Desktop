using System;
using Sportik.Automation.States;
using Sportik.Core.Events;
using Sportik.Core.Helpers;
using Sportik.Core.Services.Interfaces;

namespace Sportik.UWP.Services.Reminders.States
{
    internal class DisabledExerciseState : ExerciseState
    {
        private readonly IEventsService _eventsService;

        public override ExerciseStateKind Kind => ExerciseStateKind.Disabled;

        public DisabledExerciseState(ExerciseStatesContext context, IEventsService eventsService) : base(context)
        {
            _eventsService = eventsService;
        }

        public override void Enter()
        {
            _eventsService.Event += EventsService_Event;
        }

        public override void Exit()
        {
            _eventsService.Event -= EventsService_Event;
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
