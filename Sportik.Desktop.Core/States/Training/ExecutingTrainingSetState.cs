using Sportik.Desktop.Core.Events;
using Sportik.Desktop.Core.Helpers;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Training;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.Core.States.Training
{
    internal sealed class ExecutingTrainingSetState : TrainingSetStateBase
    {
        private readonly IEventsService _eventsService;

        public override TrainingSetState SetState => TrainingSetState.Executing;

        public ExecutingTrainingSetState(TrainingSetStatesContext context, IEventsService eventsService)
            : base(context)
        {
            _eventsService = eventsService;
        }

        protected override void HandleEnter()
        {
            _eventsService.AddListener<TrainingSetCompleteRequestedEventArgs>(EventsService_Event);
        }

        protected override void HandleExit()
        {
            _eventsService.RemoveListener<TrainingSetCompleteRequestedEventArgs>(EventsService_Event);
        }

        private void EventsService_Event(TrainingSetCompleteRequestedEventArgs args)
        {
            if (args.SetId == Context.TrainingSet.Id)
            {
                TrainingSet nextTrainingSet = TrainingSetsSequenceHelper.GetNextTrainingSet(
                    Context.TrainingSets, Context.TrainingSet);

                Context.Switch(Context.CompletedState);

                if (nextTrainingSet != null)
                {
                    TrainingSetStatesContext nextTrainingSetContext = Context.GetContext(nextTrainingSet.Id);
                    nextTrainingSetContext.Switch(nextTrainingSetContext.ExecutingState);
                }
            }
        }
    }
}
