using System.Collections.Generic;
using System.Linq;

namespace Sportik.Backend.Domain.Common
{
    public class OperationResult
    {
        public bool Succeeded { get; private set; }

        public List<string> Errors { get; private set; } = new List<string>();

        public static OperationResult Success()
        {
            return new OperationResult { Succeeded = true };
        }

        public static OperationResult Failure(IEnumerable<string> errors)
        {
            return new OperationResult { Succeeded = false, Errors = errors.ToList() };
        }
    }

    public class OperationResult<T>
    {
        public bool Succeeded { get; private set; }

        public T Value { get; private set; }

        public List<string> Errors { get; private set; } = new List<string>();

        public static OperationResult<T> Success(T value)
        {
            return new OperationResult<T> { Succeeded = true, Value = value };
        }

        public static OperationResult<T> Failure(IEnumerable<string> errors)
        {
            return new OperationResult<T> { Succeeded = false, Errors = errors.ToList() };
        }
    }
}
