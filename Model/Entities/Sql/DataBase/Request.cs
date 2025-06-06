using System;
using System.Collections.Generic;

namespace Model.Entities.Sql.DataBase;

public partial class Request
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string? Description { get; set; }

    public decimal? Amount { get; set; }

    public DateTime? AwaitedAt { get; set; }

    public int Status { get; set; }

    public string? Comment { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ResponsedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
