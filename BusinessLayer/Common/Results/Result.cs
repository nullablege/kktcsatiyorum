using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Common.Constants;
using FluentValidation.Results;

namespace BusinessLayer.Common.Results
{
    public class Result
    {
        public bool IsSuccess { get; init; }
        public Error? Error { get; init; }
        public List<ValidationError> ValidationErrors { get; init; } = new();

        public static Result Success()
        {
            return new Result { IsSuccess = true };
        }

        public static Result Fail(ErrorType type, string code, string message)
        {
            return new Result { 
                                IsSuccess = false, 
                                Error = new Error(type, code, message)
            };
        }
            

        public static Result ValidationFail(IEnumerable<ValidationError> errors)
        {
            return new Result {
                                IsSuccess = false,
                                Error = new Error(ErrorType.Validation, ErrorCodes.Common.ValidationError, "Validation failed"),
                                ValidationErrors = errors.ToList()
                               };
        }
        public static Result FromValidation(ValidationResult validationResult)
        {
            var errors = validationResult.Errors
                .Select(e => new ValidationError(e.PropertyName, e.ErrorMessage))
                .ToList();

            return ValidationFail(errors);
        }

    }
}
