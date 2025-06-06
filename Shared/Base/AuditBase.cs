using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Base
{
    public class AuditBase
    {
        public int Id { get; set; }

        public int? UserId { get; set; }
        public string? Username { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string? Action { get; set; }

        public string? Detail { get; set; }
    }
}
