using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Common;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Automation;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.Core.States.App
{
    internal sealed class OfflineAppState : AppState
    {
        private readonly IReminderService _reminderService;
        private readonly IRuntimeCacheService _runtimeCacheService;
        private readonly IPersistentCacheService _persistentCacheService;
        private readonly Func<IExercisesService> _exercisesServiceFactory;

        public override ApplicationState ApplicationState => ApplicationState.Offline;

        public OfflineAppState(AppStatesContext context, IReminderService reminderService,
            IRuntimeCacheService runtimeCacheService, IPersistentCacheService persistentCacheService,
            Func<IExercisesService> exercisesServiceFactory) : base(context)
        {
            _reminderService = reminderService;
            _runtimeCacheService = runtimeCacheService;
            _persistentCacheService = persistentCacheService;
            _exercisesServiceFactory = exercisesServiceFactory;
        }

        protected override void HandleEnter()
        {
            _runtimeCacheService.Set(new AppModeCache
            {
                IsOffline = true,
            });

            ReminderMode reminderMode = _reminderService.Mode;
            bool toStartReminders = true;

            if (_persistentCacheService.TryGet(out ReminderCache cache))
            {
                toStartReminders = cache.IsActive;
            }

            if (toStartReminders)
            {
                _reminderService.Start(reminderMode);
            }

            IExercisesService exercisesService = _exercisesServiceFactory();

            Task.Run(async () =>
            {
                OperationResult<IEnumerable<Exercise>> result = await exercisesService.GetAllAsync(ActiveCancellationToken);

                if (!result.Succeeded)
                {
                    return;
                }

                foreach (Exercise exercise in result.Value)
                {
                    _reminderService.AddExercise(exercise.Id);
                }
            });
        }

        protected override void HandleExit()
        {
            _runtimeCacheService.Remove<AppModeCache>();

            ReminderCache reminderCache = new ReminderCache
            {
                IsActive = _reminderService.IsRunning,
                Mode = _reminderService.Mode,
            };

            _persistentCacheService.Set(reminderCache);

            _reminderService.Stop();
        }
    }
}