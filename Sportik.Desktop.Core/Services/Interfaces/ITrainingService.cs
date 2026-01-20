using System;
using System.Collections.Generic;
using Sportik.Desktop.Core.Events;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Training;
using Sportik.Desktop.Core.States.Training;

namespace Sportik.Desktop.Core.Services.Interfaces
{
    public interface ITrainingService
    {
        event Action<TrainingRunningStateChangedEventArgs> RunningStateChanged;

        bool IsRunning { get; }

        IEnumerable<TrainingSet> Sets { get; }

        void Start(IEnumerable<TrainingSet> trainingSets);

        void Stop();

        TrainingSetState GetSetState(Guid setId);
    }
}