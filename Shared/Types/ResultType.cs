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
        Unauthorized = 103,
        SERVER_INTERNAL_ERROR = 104,
        SERVER_NOT_FOUND = 105,
        SERVER_UNAUTHORIZED = 106,
        SERVER_FORBIDDEN = 107,
        SERVER_BAD_REQUEST = 108,
        SERVER_FAIL = 109
    }
}
