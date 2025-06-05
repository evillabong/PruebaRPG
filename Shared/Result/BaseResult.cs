using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Result
{
    public class BaseResult 
    {
        public int ResultCode { get; set; } = 0;
        public string? Message { get; set; }


    }
}
