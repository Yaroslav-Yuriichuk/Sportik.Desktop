using System;
using Microsoft.Extensions.DependencyInjection;
using Sportik.Core;
using Sportik.Models;
using Sportik.Services.Events;

namespace Sportik.Services.Reminders.States
{
    internal class ExerciseStatesContext : IStatesContext<ExerciseState>, IDisposable
    {
        public Exercise Exercise { get; }

        public ExerciseState DisabledExerciseState { get; private set; }

        public ExerciseState WaitingExerciseState { get; private set; }

        public ExerciseState ExecutingExerciseState { get; private set; }

        public ExerciseState CurrentState { get; private set; }

        private IEventsService EventsService => App.ServiceProvider.GetService<IEventsService>();

        public ExerciseStatesContext(Exercise exercise)
        {
            DisabledExerciseState = new DisabledExerciseState(this);
            WaitingExerciseState = new WaitingExerciseState(this);
            ExecutingExerciseState = new ExecutingExerciseState(this);

            Exercise = exercise;

            ExerciseState state = Exercise.ExerciseSettings.IsEnabled
                ? WaitingExerciseState
                : DisabledExerciseState;

            Switch(state);
        }

        public void Dispose()
        {
            Switch(null);
        }

        public void Switch(ExerciseState state)
        {
            ExerciseStateKind previousState = CurrentState?.Kind ?? ExerciseStateKind.Unknown;
            ExerciseStateKind currentState = state?.Kind ?? ExerciseStateKind.Unknown;

            CurrentState?.Exit();
            CurrentState = state;
            CurrentState?.Enter();

            EventsService.RaiseEvent(new ExerciseStateChangedEventArgs(Exercise, previousState, currentState));
        }
    }
}
