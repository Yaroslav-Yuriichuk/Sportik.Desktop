using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Sportik.Core;
using Sportik.Helpers;
using Sportik.Models;
using Sportik.Models.Settings;
using Sportik.Services.Notifications;
using Sportik.Services.Settings;

namespace Sportik.ViewModels.Exercises
{
    internal sealed class ExerciseViewModel : ViewModel, IDisposable
    {
        private string _name;

        public string Name
        {
            get => _name;
            set => SetField(ref _name, value);
        }

        private bool _isEnabled;

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (SetField(ref _isEnabled, value))
                {
                    _updateCts?.Cancel();
                    _updateCts = new CancellationTokenSource();

                    _ = UpdateExerciseSettingsAsync(_updateCts.Token);
                }
            }
        }

        private IExerciseSettingsService ExerciseSettingsService => App.ServiceProvider.GetService<IExerciseSettingsService>();
        private IExerciseTimersService ExerciseTimersService => App.ServiceProvider.GetService<IExerciseTimersService>();

        private readonly Exercise _exercise;

        private CancellationTokenSource _updateCts = new CancellationTokenSource();

        public ExerciseViewModel(Exercise exercise)
        {
            _exercise = exercise;

            Name = _exercise.Name;
            IsEnabled = _exercise.ExerciseSettings.IsEnabled;
        }

        public void Dispose()
        {
            _updateCts?.Cancel();
        }

        private async Task UpdateExerciseSettingsAsync(CancellationToken cancellationToken)
        {
            ExerciseSettingsDelta exerciseSettingsDelta = new ExerciseSettingsDelta()
            {
                Change = ExerciseSettingsChange.IsEnabled,
                IsEnabled = IsEnabled,
            };

            await ExerciseSettingsService.UpdateExerciseSettingsAsync(exerciseSettingsDelta, _exercise, cancellationToken);

            ITimer timer = ExerciseTimersService.GetTimer(_exercise);

            if (IsEnabled)
            {
                if (!timer.IsRunning)
                {
                    timer.Start();
                }

                if (timer.IsPaused)
                {
                    timer.Resume();
                }
            }
            else
            {
                if (timer.IsRunning)
                {
                    timer.Pause();
                }
            }
        }
    }
}
