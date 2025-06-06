using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Base
{
    public class RequestBase
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public decimal? Amount { get; set; }

        public DateTime? AwaitedAt { get; set; }

        public int Status { get; set; }

        public string? Comment { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? ResponsedAt { get; set; }
    }
}
