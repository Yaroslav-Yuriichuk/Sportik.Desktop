using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Core.Models;
using Sportik.Core.Repositories.Interfaces;
using Sportik.Core.Services.Interfaces;

namespace Sportik.Data.Services.Implementations
{
    public sealed class ExercisesService : IExercisesService
    {
        private readonly IExercisesRepository _exercisesRepository;

        public ExercisesService(IExercisesRepository exercisesRepository)
        {
            _exercisesRepository = exercisesRepository;
        }

        public IEnumerable<Exercise> GetAllExercises()
        {
            return _exercisesRepository.GetAll();
        }

        public async Task<IEnumerable<Exercise>> GetAllExercisesAsync(CancellationToken cancellationToken = default)
        {
            return await _exercisesRepository.GetAllAsync(cancellationToken);
        }

        public IEnumerable<Exercise> GetExercises(IEnumerable<int> exercisesIds)
        {
            return exercisesIds.Select(id => _exercisesRepository.GetById(id));
        }
    }
}
