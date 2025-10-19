using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Infrastructure.DTOs;

namespace Sportik.Desktop.Infrastructure.Mappers
{
    internal static class ExerciseMapper
    {
        public static Exercise ToDomain(ExerciseDto dto, bool isEnabled)
        {
            return new Exercise(
                id: dto.Id,
                name: dto.Name,
                settings: ExerciseSettingsMapper.ToDomain(dto.Settings, isEnabled));
        }
    }
}