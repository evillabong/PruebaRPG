using Shared.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Result.Supervisor
{
    public class PendingRequestResult : BaseResult
    {
        public List<RequestBase> PendingRequest { get; set; } = [];
    }
}
