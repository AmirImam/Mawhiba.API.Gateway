using System;
using System.Collections.Generic;

namespace Mawhiba.API.Gateway.Models;

public partial class UserFeedback
{
    public Guid Id { get; set; }

    public long? UserId { get; set; }

    public int? RatingValue { get; set; }

    public string? Feedback { get; set; }

    public int? ServiceId { get; set; }

    public string? MoreInfo { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? Ostype { get; set; }

    public string? AppVersion { get; set; }

    public string? DeviceName { get; set; }

    public string? DeviceModel { get; set; }

    public int? ContentServiceId { get; set; }
}
