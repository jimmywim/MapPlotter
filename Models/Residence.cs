using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapPlotter.Data;

namespace MapPlotter.Data
{
    public partial class Residence
    {
        [NotMapped]
        public string? Proprietor { get; set; }

        [NotMapped]
        public string? Tenant => Vrtenant;

        [NotMapped]
        public string? Occupier => Vroccupier;

        [NotMapped]
        public string Name
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(Address);

                if (!string.IsNullOrEmpty(Number))
                {
                    sb.Append($", {Number}");
                }

                return sb.ToString();
            }
        }

        [NotMapped]
        public bool HasGeo => ResidenceLocations != null && ResidenceLocations.Count > 0;

        [NotMapped]
        public ResidenceLocation Location => ResidenceLocations.FirstOrDefault();

        [NotMapped]
        public string IconName => HasGeo ? "MapPin" : "Question";

        [NotMapped]
        public bool IsOwnerOccupier => Occupier == "The Proprietor";

        public Residence Clone()
        {
            Residence clone = MemberwiseClone() as Residence;
            return clone;
        }
    }
}
