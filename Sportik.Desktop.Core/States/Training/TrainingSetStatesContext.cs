using System;
using System.Collections.Generic;
using Sportik.Desktop.Core.Common.StateMachine;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Training;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.Core.States.Training
{
    internal sealed class TrainingSetStatesContext : IStatesContext<TrainingSetStateBase>, IDisposable
    {
        private readonly IEventsService _eventsService;

        private readonly Func<Guid, TrainingSetStatesContext> _getContextCallback;
        private readonly Action<Guid> _completionCallback;

        public TrainingSet TrainingSet { get; }

        public IList<TrainingSet> TrainingSets { get; }

        public TrainingSetStateBase QueuedState { get; }

        public TrainingSetStateBase ExecutingState { get; }

        public TrainingSetStateBase CompletedState { get; }

        public TrainingSetStateBase CurrentState { get; private set; }

        public TrainingSetStatesContext(
            TrainingSet trainingSet,
            IList<TrainingSet> trainingSets,
            IEventsService eventsService,
            Func<Guid, TrainingSetStatesContext> getContextCallback,
            Action<Guid> completionCallback)
        {
            _eventsService = eventsService;

            _getContextCallback = getContextCallback;
            _completionCallback = completionCallback;

            TrainingSet = trainingSet;
            TrainingSets = trainingSets;

            QueuedState = new QueuedTrainingSetState(this);
            ExecutingState = new ExecutingTrainingSetState(this, _eventsService);
            CompletedState = new CompletedTrainingSetState(this);

            int index = trainingSets.IndexOf(trainingSet);

            if (index == 0)
            {
                Switch(ExecutingState);
            }
            else
            {
                Switch(QueuedState);
            }
        }

        public void Dispose()
        {
            Switch(null);
        }

        public void Switch(TrainingSetStateBase state)
        {
            TrainingSetState previousState = CurrentState?.SetState ?? TrainingSetState.Unknown;

            CurrentState?.Exit();
            CurrentState = state;
            CurrentState?.Enter();

            TrainingSetState currentState = state?.SetState ?? TrainingSetState.Unknown;

            _eventsService.RaiseEvent(new Events.TrainingSetStateChangedEventArgs(TrainingSet.Id, previousState, currentState));

            if (currentState == TrainingSetState.Completed)
            {
                _completionCallback?.Invoke(TrainingSet.Id);
            }
        }

        public TrainingSetStatesContext GetContext(Guid setId)
        {
            return _getContextCallback(setId);
        }
    }
}
