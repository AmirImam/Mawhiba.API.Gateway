using System;
using System.Collections.Generic;

namespace Mawhiba.API.Gateway.Models;

public partial class SignalRconnection
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string ConnectionId { get; set; } = null!;
}
