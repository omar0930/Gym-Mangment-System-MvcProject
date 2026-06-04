using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymMangmentSystem.BLL.Common
{
    public sealed record Result(bool success, string? ErrorMessage = null, ResultStatus Status = ResultStatus.Success)
    {
        public static Result Success() => new(true);
        public static Result Failure(string errorMessage, ResultStatus status = ResultStatus.Conflict) => new(false, errorMessage, status);
        public static Result NotFound(string errorMessage = "Resource not found") => new(false, errorMessage, ResultStatus.NotFound);
        public static Result ValidationError(string errorMessage) => new(false, errorMessage, ResultStatus.ValidationError);
        public static Result Forbidden(string errorMessage = "Access denied") => new(false, errorMessage, ResultStatus.Forbidden);

    }
    public sealed record Result<T>(bool success, T? Value, string? ErrorMessage = null, ResultStatus Status = ResultStatus.Success)
    {
        public static Result<T> Success(T Value) => new(true, Value);
        public static Result<T> Failure(string errorMessage, ResultStatus status = ResultStatus.Conflict) => new(false, default, errorMessage, status);
        public static Result<T> NotFound(string errorMessage = "Resource not found") => new(false, default, errorMessage, ResultStatus.NotFound);
        public static Result<T> ValidationError(string errorMessage) => new(false, default, errorMessage, ResultStatus.ValidationError);
        public static Result<T> Forbidden(string errorMessage = "Access denied") => new(false, default, errorMessage, ResultStatus.Forbidden);

    }
    public enum ResultStatus
    {
        Success,
        NotFound,
        Conflict,
        ValidationError,
        Forbidden
    }
}
