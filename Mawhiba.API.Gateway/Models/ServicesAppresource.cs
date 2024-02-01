using System;
using System.Collections.Generic;

namespace Mawhiba.API.Gateway.Models;

public partial class ServicesAppresource
{
    public int Id { get; set; }

    public int ServiceId { get; set; }

    public byte[] FileData { get; set; } = null!;

    public string LangCode { get; set; } = null!;

    public DateTime LastModified { get; set; }

    public virtual Service Service { get; set; } = null!;
}
