using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sportik.Desktop.Infrastructure.Services.Implementations
{
    public sealed class ExercisesService : IExercisesService
    {
        private readonly IExercisesRepository _exercisesRepository;

        public ExercisesService(IExercisesRepository exercisesRepository)
        {
            _exercisesRepository = exercisesRepository;
        }

        public async Task<IEnumerable<Exercise>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _exercisesRepository.GetAllAsync(cancellationToken);
        }

        public async Task<IEnumerable<Exercise>> GetByIdsAsync(IEnumerable<int> exercisesIds, CancellationToken cancellationToken = default)
        {
            return await _exercisesRepository.GetByIdsAsync(exercisesIds, cancellationToken);
        }
    }
}
