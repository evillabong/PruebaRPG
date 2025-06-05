using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Param.User
{
    public class CreateRequestParam
    {
        public int UserId { get; set; }
        public string Description { get; set; } = null!;
        public double Amount { get; set; }
        public DateTime AwaitedAt { get; set; }
        public string? Comment { get; set; }
    }
}
