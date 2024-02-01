using System;
using System.Collections.Generic;

namespace Mawhiba.API.Gateway.Models;

public partial class AppVersion
{
    public int Id { get; set; }

    public DateTime? VersionDate { get; set; }

    public string? VersionNumber { get; set; }

    public string? IosversionNumber { get; set; }

    public string? AndroidVersionNumber { get; set; }
}
