using MapPlotter.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace MapPlotter.Services
{
    internal class ResidenceDataService
    {
        private string filename;

        public ResidenceDataService(string fileName)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            filename = fileName;
        }

        public List<Residence> LoadResidences()
        {
            var residences = new List<Residence>(); 

            using (FileStream stream = File.OpenRead(filename))
            using (ExcelPackage excelPackage = new ExcelPackage(stream))
            {
                var worksheet = excelPackage.Workbook.Worksheets.First();
                var dataTable = worksheet.Cells["A:F"].ToDataTable(c =>
                {
                    c.DataTableName = "Residences";
                    c.ColumnNameParsingStrategy = OfficeOpenXml.Export.ToDataTable.NameParsingStrategy.RemoveSpace;

                    c.Mappings.Add(1, "Address", typeof(string), true);
                    c.Mappings.Add(2, "Number", typeof(string), true);
                    c.Mappings.Add(3, "Latitude", typeof(string), true);
                    c.Mappings.Add(4, "Longitude", typeof(string), true);
                    c.Mappings.Add(5, "Notes", typeof(string), true);
                });

                foreach(DataRow row in dataTable.Rows)
                {
                    Residence residence = new Residence
                    {
                        PrimaryKey = row["PrimaryKey"] as string,
                        Address = row["Address"] as string,
                        Latitude = row["Latitude"] as string,
                        Longitude = row["Longitude"] as string,
                        Notes = row["Notes"] as string,
                        Number = row["Number"] as string,
                    };

                    residences.Add(residence);
                }
            }

            return residences;
        }

        public void SaveResidences(List<Residence> residences)
        {

        }
    }
}
