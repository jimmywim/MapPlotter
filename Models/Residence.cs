using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapPlotter.Models
{
    public class Residence
    {
        public string? PrimaryKey { get; set; }
        public string? Address { get; set; }
        public string? Number { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public string? Notes { get; set; }
        public string? Description { get; set; }
        public string? Proprietor { get; set; }
        public string? Tenant { get; set; }
        public string? Occupier { get; set; }

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

        public bool HasGeo => !string.IsNullOrEmpty(Latitude) && !string.IsNullOrEmpty(Longitude);

        public string IconName => HasGeo ? "MapPin" : "Question";

        public bool IsDirty { get; set; }

        public bool IsOwnerOccupier => Occupier == "The Proprietor";

        public Residence Clone()
        {
            Residence clone = MemberwiseClone() as Residence;
            return clone;
        }
    }
}
