using Shared.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Result.User
{
    public class MyRequestResult : BaseResult
    {
        public List<RequestBase> Requests { get; set; } = new List<RequestBase>();
    }
}
