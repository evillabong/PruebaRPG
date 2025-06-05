using System;
using System.Collections.Generic;

namespace Model.Entities.Sql.DataBase;

public partial class Audit
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? Action { get; set; }

    public string? Detail { get; set; }
}
