using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Repositories.Interfaces;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.Core.Services.Implementations
{
    internal sealed class ExercisesService : IExercisesService
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
