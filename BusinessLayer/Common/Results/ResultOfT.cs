using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.Results;

namespace BusinessLayer.Common.Results
{
    public class Result<T> : Result
    {
        public T? Data { get; init; }

        public static Result<T> Success(T data) => new() { IsSuccess = true, Data = data };

        public static Result<T> Fail(ErrorType type, string code, string message) =>
            new() { IsSuccess = false, Error = new Error(type, code, message) };

        public static Result<T> ValidationFail(IEnumerable<ValidationError> errors) =>
            new()
            {
                IsSuccess = false,
                Error = new Error(ErrorType.Validation, "validation_error", "Validation failed"),
                ValidationErrors = errors.ToList()
            };

        public static Result<T> FromValidation(ValidationResult validationResult)
        {
            var errors = validationResult.Errors
                .Select(e => new ValidationError(e.PropertyName, e.ErrorMessage))
                .ToList();

            return ValidationFail(errors);
        }
    }
}
