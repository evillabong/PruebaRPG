using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Entities.Sql.DataBase
{
    public partial class DatabaseContext 
    {
        public virtual DbSet<EmptyEntity> Empty{ get; set; }
    }
}
