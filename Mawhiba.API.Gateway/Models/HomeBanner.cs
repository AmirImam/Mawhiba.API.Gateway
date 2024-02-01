using System;
using System.Collections.Generic;

namespace Mawhiba.API.Gateway.Models;

public partial class HomeBanner
{
    public int Id { get; set; }

    public string? BannerUrl { get; set; }

    public string? BannerHoverTitle { get; set; }

    public string? BannerHoverText { get; set; }

    public string? BannerClickPath { get; set; }

    public int? BannerRank { get; set; }

    public DateTime? PublishedFrom { get; set; }

    public DateTime? PublishedTo { get; set; }
}
