using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Types
{
    public enum ResultType
    {
        Success = 0,
        Error = 1,
        SessionFail = 98,
        InternalError = 99,
        ContextFail = 100,
        UnknowError = 101,
        ClientError = 102,
        Unauthorized = 103
    }
}
