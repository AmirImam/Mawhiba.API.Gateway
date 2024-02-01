using System;
using System.Collections.Generic;

namespace Mawhiba.API.Gateway.Models;

public partial class ServicesContent
{
    public int Id { get; set; }

    public int ServiceId { get; set; }

    public int? ContentTypeId { get; set; }

    public string Contents { get; set; } = null!;

    public DateTime LastUpdated { get; set; }

    public virtual ContentType? ContentType { get; set; }

    public virtual Service Service { get; set; } = null!;
}
