using System;
using Microsoft.Extensions.DependencyInjection;
using Sportik.Automation.Events;
using Sportik.Automation.Services;
using Sportik.Automation.States;
using Sportik.Core.Models;
using Sportik.Core.Services.Interfaces;
using Sportik.Core.StateMachine;
using Sportik.Notification.Services;

namespace Sportik.UWP.Services.Reminders.States
{
    internal class ExerciseStatesContext : IStatesContext<ExerciseState>, IDisposable
    {
        private readonly IEventsService _eventsService;
        private readonly IExerciseTimersService _exerciseTimersService;
        private readonly Func<IExerciseSettingsService> _exerciseSettingsServiceFactory;
        private readonly Func<INotificationService> _notificationServiceFactory;

        public Exercise Exercise { get; }

        public ExerciseState DisabledExerciseState { get; private set; }

        public ExerciseState WaitingExerciseState { get; private set; }

        public ExerciseState ExecutingExerciseState { get; private set; }

        public ExerciseState CurrentState { get; private set; }

        public ExerciseStatesContext(Exercise exercise, IEventsService eventsService, IExerciseTimersService exerciseTimersService,
            Func<IExerciseSettingsService> exerciseSettingsServiceFactory, Func<INotificationService> notificationServiceFactory)
        {
            _eventsService = eventsService;
            _exerciseTimersService = exerciseTimersService;
            _exerciseSettingsServiceFactory = exerciseSettingsServiceFactory;
            _notificationServiceFactory = notificationServiceFactory;

            DisabledExerciseState = new DisabledExerciseState(this, _eventsService);
            WaitingExerciseState = new WaitingExerciseState(this, _eventsService, _exerciseTimersService, _exerciseSettingsServiceFactory, _notificationServiceFactory);
            ExecutingExerciseState = new ExecutingExerciseState(this, _eventsService, _exerciseTimersService, _exerciseSettingsServiceFactory);

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

            _eventsService.RaiseEvent(new ExerciseStateChangedEventArgs(Exercise, previousState, currentState));
        }
    }
}
