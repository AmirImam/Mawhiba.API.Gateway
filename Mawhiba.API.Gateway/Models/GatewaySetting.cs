using System;
using System.Collections.Generic;

namespace Mawhiba.API.Gateway.Models;

public partial class GatewaySetting
{
    public Guid Id { get; set; }

    public int ServiceId { get; set; }

    public string? SettingJson { get; set; }
}
