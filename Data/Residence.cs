using System;
using System.Collections.Generic;

namespace MapPlotter.Data;

public partial class Residence
{
    public string PrimaryKey { get; set; } = null!;

    public string? Address { get; set; }

    public string? Number { get; set; }

    public string? Vrnumber { get; set; }

    public string? Vrdescription { get; set; }

    public string? Vrproprietor { get; set; }

    public string? Vrtenant { get; set; }

    public string? Vroccupier { get; set; }

    public string? VrnotRated { get; set; }

    public long? VrannualValue { get; set; }

    public long? VrannualValued { get; set; }

    public long? VrannualValues { get; set; }

    public long? VrfeuDuty { get; set; }

    public long? VrfeuDutyd { get; set; }

    public long? VrfeuDutys { get; set; }

    public long? VryearlyRent { get; set; }

    public long? VryearlyRentd { get; set; }

    public long? VryearlyRents { get; set; }

    public virtual ICollection<ResidenceLocation> ResidenceLocations { get; } = new List<ResidenceLocation>();
}
