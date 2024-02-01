using System;
using System.Collections.Generic;

namespace Mawhiba.API.Gateway.Models;

public partial class Service
{
    public int Id { get; set; }

    public string ServiceName { get; set; } = null!;

    public string? ServiceDescription { get; set; }

    public string? ServiceUrl { get; set; }

    public string? ServiceApplicationUrl { get; set; }

    public string? ServiceHomeImageUrl { get; set; }

    public bool? IsPublished { get; set; }

    public int? ServiceOrder { get; set; }

    public bool? IsInHomePage { get; set; }

    public virtual ICollection<ServicesAppresource> ServicesAppresources { get; set; } = new List<ServicesAppresource>();

    public virtual ICollection<ServicesContent> ServicesContents { get; set; } = new List<ServicesContent>();

    public virtual ICollection<ServicesFeature> ServicesFeatures { get; set; } = new List<ServicesFeature>();
}
