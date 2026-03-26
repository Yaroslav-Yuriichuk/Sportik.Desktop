using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Infrastructure.DTOs.Statistics;

namespace Sportik.Desktop.Infrastructure.Mappers.Statistics
{
    internal static class SetMapper
    {
        public static ExerciseSet ToDomain(SetDto dto)
        {
            return new ExerciseSet(dto.Id, dto.Repetitions, dto.LoggedAt);
        }

        public static AddSetDto ToDto(AddExerciseSetModel addModel)
        {
            return new AddSetDto(addModel.Id, addModel.ExerciseId, addModel.Repetitions, addModel.LoggedAt);
        }
    }
}