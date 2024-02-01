using System;
using System.Collections.Generic;

namespace Mawhiba.API.Gateway.Models;

public partial class ContentType
{
    public int Id { get; set; }

    public string ContentTypeName { get; set; } = null!;

    public virtual ICollection<ServicesContent> ServicesContents { get; set; } = new List<ServicesContent>();
}
