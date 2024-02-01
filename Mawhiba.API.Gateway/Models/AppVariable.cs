using System;
using System.Collections.Generic;

namespace Mawhiba.API.Gateway.Models;

public partial class AppVariable
{
    public int Id { get; set; }

    public string VariableKey { get; set; } = null!;

    public string VariableValue { get; set; } = null!;
}
