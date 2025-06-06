using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Result
{
    public class LoginResult : BaseResult
    {
        public string? Jti { get; set; }
        public string? Token { get; set; } 
        public DateTime? ExpireAt { get; set; }
    };

}
