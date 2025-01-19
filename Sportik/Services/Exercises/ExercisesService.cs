using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Models;

namespace Sportik.Services.Exercises
{
    internal sealed class ExercisesService : IExercisesService
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
    }
}
