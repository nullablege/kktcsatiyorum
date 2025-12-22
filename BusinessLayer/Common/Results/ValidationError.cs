using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Common.Results
{
    public sealed record ValidationError(string Field, string Message);

}
