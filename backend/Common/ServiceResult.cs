using System.Collections.Generic;
using System.Linq;

namespace TalentBridge.Common
{
    public class ServiceResult
    {
        public bool Succeeded { get; protected set; }
        public bool IsForbidden { get; protected set; }
        public bool IsNotFound { get; protected set; }
        public IEnumerable<string> Errors { get; protected set; }

        public static ServiceResult Success() => new ServiceResult { Succeeded = true };
        public static ServiceResult Fail(string error) => new ServiceResult { Succeeded = false, Errors = new[] { error } };
        public static ServiceResult Fail(IEnumerable<string> errors) => new ServiceResult { Succeeded = false, Errors = errors };
        public static ServiceResult Forbidden() => new ServiceResult { Succeeded = false, IsForbidden = true };
        public static ServiceResult NotFound() => new ServiceResult { Succeeded = false, IsNotFound = true };
    }

    public class ServiceResult<T> : ServiceResult
    {
        public T Data { get; private set; }

        public static ServiceResult<T> Success(T data) => new ServiceResult<T> { Succeeded = true, Data = data };
        public new static ServiceResult<T> Fail(string error) => new ServiceResult<T> { Succeeded = false, Errors = new[] { error } };
        public new static ServiceResult<T> Fail(IEnumerable<string> errors) => new ServiceResult<T> { Succeeded = false, Errors = errors };
        public new static ServiceResult<T> Forbidden() => new ServiceResult<T> { Succeeded = false, IsForbidden = true };
        public new static ServiceResult<T> NotFound() => new ServiceResult<T> { Succeeded = false, IsNotFound = true };
    }
}
