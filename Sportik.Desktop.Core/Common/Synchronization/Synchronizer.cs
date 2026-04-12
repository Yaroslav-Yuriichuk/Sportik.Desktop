using System;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Repositories.Interfaces;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.Core.Common.Synchronization
{
    internal abstract class Synchronizer : ISynchronizer
    {
        private IExercisesRepository _remoteExercisesRepository;
        private IExercisesRepository _localExercisesRepository;
        private IExerciseSettingsRepository _remoteExerciseSettingsRepository;
        private IExerciseSettingsRepository _localExerciseSettingsRepository;
        private IExerciseStatisticsRepository _remoteExerciseStatisticsRepository;
        private IExerciseStatisticsRepository _localExerciseStatisticsRepository;
        private IEventsService _eventsService;

        protected IExercisesRepository RemoteExercisesRepository => _remoteExercisesRepository;
        protected IExercisesRepository LocalExercisesRepository => _localExercisesRepository;
        protected IExerciseSettingsRepository RemoteExerciseSettingsRepository => _remoteExerciseSettingsRepository;
        protected IExerciseSettingsRepository LocalExerciseSettingsRepository => _localExerciseSettingsRepository;
        protected IExerciseStatisticsRepository RemoteExerciseStatisticsRepository => _remoteExerciseStatisticsRepository;
        protected IExerciseStatisticsRepository LocalExerciseStatisticsRepository => _localExerciseStatisticsRepository;
        protected IEventsService EventsService => _eventsService;

        private bool _isInitialized;

        public void Initialize(
            IExercisesRepository remoteExercisesRepository,
            IExercisesRepository localExercisesRepository,
            IExerciseSettingsRepository remoteExerciseSettingsRepository,
            IExerciseSettingsRepository localExerciseSettingsRepository,
            IExerciseStatisticsRepository remoteExerciseStatisticsRepository,
            IExerciseStatisticsRepository localExerciseStatisticsRepository,
            IEventsService eventsService)
        {
            if (_isInitialized)
            {
                throw new InvalidOperationException("Synchronizer is already initialized.");
            }

            _remoteExercisesRepository = remoteExercisesRepository;
            _localExercisesRepository = localExercisesRepository;
            _remoteExerciseSettingsRepository = remoteExerciseSettingsRepository;
            _localExerciseSettingsRepository = localExerciseSettingsRepository;
            _remoteExerciseStatisticsRepository = remoteExerciseStatisticsRepository;
            _localExerciseStatisticsRepository = localExerciseStatisticsRepository;
            _eventsService = eventsService;

            _isInitialized = true;
        }

        public abstract Task SyncAsync(CancellationToken cancellationToken);
    }
}