using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Common.Results
{
    public enum ErrorType
    {
        Validation = 1,
        NotFound = 2,
        Conflict = 3,
        Forbidden = 4,
        Unauthorized = 5,
        Failure = 6
    }
    public sealed record Error(ErrorType Type, string Code, string Message);

}
