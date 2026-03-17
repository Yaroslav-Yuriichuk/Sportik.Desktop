using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Common;
using Sportik.Desktop.Core.Events;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Settings;
using Sportik.Desktop.Core.Repositories.Interfaces;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.Core.Services.Implementations
{
    internal sealed class ExercisesService : IExercisesService
    {
        private readonly IExercisesRepository _exercisesRepository;
        private readonly IEventsService _eventsService;

        public ExercisesService(Func<DataSource, IExercisesRepository> exercisesRepositoryFactory, IEventsService eventsService)
        {
            _exercisesRepository = exercisesRepositoryFactory(DataSource.Remote);
            _eventsService = eventsService;
        }

        public async Task<OperationResult<IEnumerable<Exercise>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                IEnumerable<Exercise> exercises = await _exercisesRepository.GetAllAsync(cancellationToken);
                return OperationResult<IEnumerable<Exercise>>.Success(exercises);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception)
            {
                return OperationResult<IEnumerable<Exercise>>.Failure(new[] { "Failed to retrieve exercises." });
            }
        }

        public async Task<OperationResult<Exercise>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            try
            {
                Exercise exercise = await _exercisesRepository.GetByIdAsync(id, cancellationToken);
                return OperationResult<Exercise>.Success(exercise);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception)
            {
                return OperationResult<Exercise>.Failure(new[] { "Failed to retrieve exercise." });
            }
        }

        public async Task<OperationResult<IEnumerable<Exercise>>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
        {
            try
            {
                IEnumerable<Exercise> exercises = await _exercisesRepository.GetByIdsAsync(ids, cancellationToken);
                return OperationResult<IEnumerable<Exercise>>.Success(exercises);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception)
            {
                return OperationResult<IEnumerable<Exercise>>.Failure(new[] { "Failed to retrieve exercises." });
            }
        }

        public async Task<OperationResult<Exercise>> AddAsync(string name, ExerciseSettings settings, CancellationToken cancellationToken = default)
        {
            try
            {
                Exercise exercise = await _exercisesRepository.AddAsync(name, settings, cancellationToken);
                _eventsService.RaiseEvent(new ExerciseCreatedEventArgs(exercise));

                return OperationResult<Exercise>.Success(exercise);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception)
            {
                return OperationResult<Exercise>.Failure(new[] { "Failed to add exercise." });
            }
        }
    }
}
