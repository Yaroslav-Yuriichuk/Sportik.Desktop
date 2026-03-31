using System;
using System.Collections.Generic;
using System.Linq;
using Sportik.Desktop.Core.Events;
using Sportik.Desktop.Core.Models.Training;
using Sportik.Desktop.Core.Services.Interfaces;
using Sportik.Desktop.Core.States.Training;

namespace Sportik.Desktop.Core.Services.Implementations
{
    internal sealed class TrainingService : ITrainingService
    {
        private readonly IEventsService _eventsService;

        private readonly List<TrainingSetStatesContext> _contexts = new List<TrainingSetStatesContext>();
        private List<TrainingSet> _trainingSets = new List<TrainingSet>();

        public event Action<TrainingRunningStateChangedEventArgs> RunningStateChanged;

        public bool IsRunning => _contexts.Count > 0;

        public IEnumerable<TrainingSet> Sets => _trainingSets;

        public TrainingService(IEventsService eventsService)
        {
            _eventsService = eventsService;
        }

        public void Start(IEnumerable<TrainingSet> trainingSets)
        {
            if (IsRunning)
            {
                return;
            }

            _trainingSets = trainingSets.ToList();

            if (_trainingSets.Count == 0)
            {
                return;
            }

            foreach (TrainingSet trainingSet in _trainingSets)
            {
                TrainingSetStatesContext context = new TrainingSetStatesContext(
                    trainingSet,
                    _trainingSets,
                    _eventsService,
                    GetContext,
                    HandleCompletion);

                _contexts.Add(context);
            }

            RunningStateChanged?.Invoke(new TrainingRunningStateChangedEventArgs(true));
        }

        public void Stop()
        {
            if (!IsRunning)
            {
                return;
            }

            foreach (TrainingSetStatesContext context in _contexts)
            {
                context.Dispose();
            }

            _contexts.Clear();
            _trainingSets.Clear();

            RunningStateChanged?.Invoke(new TrainingRunningStateChangedEventArgs(false, TrainingStopReason.Cancelled));
        }

        public TrainingSetState GetSetState(Guid setId)
        {
            TrainingSetStatesContext context = _contexts.FirstOrDefault(c => c.TrainingSet.Id == setId);
            return context?.CurrentState?.SetState ?? TrainingSetState.Unknown;
        }

        private TrainingSetStatesContext GetContext(Guid setId)
        {
            return _contexts.FirstOrDefault(c => c.TrainingSet.Id == setId);
        }

        private void HandleCompletion(Guid setId)
        {
            if (!IsRunning)
            {
                return;
            }

            if (_contexts.All(c => c.CurrentState.SetState == TrainingSetState.Completed))
            {
                foreach (TrainingSetStatesContext context in _contexts)
                {
                    context.Dispose();
                }

                _contexts.Clear();
                _trainingSets.Clear();

                RunningStateChanged?.Invoke(new TrainingRunningStateChangedEventArgs(false, TrainingStopReason.Completed));
            }
        }
    }
}
