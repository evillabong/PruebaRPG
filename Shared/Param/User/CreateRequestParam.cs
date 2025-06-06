using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Param.User
{
    public class CreateRequestParam : BaseParam
    {
        [Required(ErrorMessage = "La descripción es requerida")]

        public string Description { get; set; } = null!;
        [Required(ErrorMessage = "El monto es requerido")]

        public double Amount { get; set; }
        [Required(ErrorMessage = "La fecha esperada es requerida")]

        public DateTime AwaitedAt { get; set; }
        public string? Comment { get; set; }
    }
}
