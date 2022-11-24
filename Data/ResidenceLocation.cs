using System;
using System.Collections.Generic;

namespace MapPlotter.Data;

public partial class ResidenceLocation
{
    public long Id { get; set; }

    public string ResidenceId { get; set; } = null!;

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public string? LocationNotes { get; set; }

    public virtual Residence Residence { get; set; } = null!;
}
