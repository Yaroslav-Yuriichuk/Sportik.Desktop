using System.Collections.Generic;
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
    }
}
