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

        public void SetResult(int resultCode, string? message = null)
        {
            ResultCode = resultCode;
            Message = message;
        }   
        public void SetResult(Enum resultCode, string? message = null)
        {
            ResultCode = Convert.ToInt32(resultCode);
            Message = message;
        }
    }
}
