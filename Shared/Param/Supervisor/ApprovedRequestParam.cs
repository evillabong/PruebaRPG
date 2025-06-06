using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Param.Supervisor
{
    public class ApprovedRequestParam : BaseParam
    {
        public int RequestId { get; set; }
        public int Status { get; set; }
        public string? Comment { get; set; }
    }
}
