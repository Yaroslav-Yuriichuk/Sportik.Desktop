using System;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Events;
using Sportik.Desktop.Core.Models.Settings;
using Sportik.Desktop.Core.Repositories.Interfaces;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.Core.Services.Implementations
{
    internal sealed class ExerciseSettingsService : IExerciseSettingsService
    {
        private readonly IExerciseSettingsRepository _exerciseSettingsRepository;
        private readonly IEventsService _eventsService;

        public ExerciseSettingsService(IExerciseSettingsRepository exerciseSettingsRepository, IEventsService eventsService)
        {
            _exerciseSettingsRepository = exerciseSettingsRepository;
            _eventsService = eventsService;
        }

        public async Task<ExerciseSettings> UpdateAsync(ExerciseSettingsDelta exerciseSettingsDelta, Guid exerciseId,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
