using System;
using System.Collections.Generic;

namespace Mawhiba.API.Gateway.Models;

public partial class ExceptionLog
{
    public Guid Id { get; set; }

    public string ExceptionText { get; set; } = null!;

    public DateTime ExceptionTime { get; set; }
}
