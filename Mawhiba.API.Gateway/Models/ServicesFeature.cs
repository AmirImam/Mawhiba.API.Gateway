using System;
using System.Collections.Generic;

namespace Mawhiba.API.Gateway.Models;

public partial class ServicesFeature
{
    public int Id { get; set; }

    public int ServiceId { get; set; }

    public string FeatureName { get; set; } = null!;

    public string FeatureText { get; set; } = null!;

    public string FeatureTextEn { get; set; } = null!;

    public bool FreatureVisible { get; set; }

    public virtual Service Service { get; set; } = null!;
}
