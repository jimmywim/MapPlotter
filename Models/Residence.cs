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
    }
}
