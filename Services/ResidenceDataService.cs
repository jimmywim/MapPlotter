using MapPlotter.Data;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MapPlotter.Services
{
    internal class ResidenceDataService
    {
        private string filename;
        private readonly ResidencesContext residencesContext;

        public ResidenceDataService(string fileName, ResidencesContext context)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            filename = fileName;
            residencesContext = context;
        }

        public async Task ImportResidences()
        {
            int residencesAdded = 0;
            int residencesUpdated = 0;

            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                await excelPackage.LoadAsync(filename);
                var worksheet = excelPackage.Workbook.Worksheets.First();

                var dataTable = worksheet.Cells["A:K"].ToDataTable(c =>
                {
                    c.DataTableName = "Residences";
                    c.ColumnNameParsingStrategy = OfficeOpenXml.Export.ToDataTable.NameParsingStrategy.RemoveSpace;

                    c.Mappings.Add(1, "Address", typeof(string), true);
                    c.Mappings.Add(2, "Number", typeof(string), true);
                    c.Mappings.Add(3, "VRNumber", typeof(string), true);
                    c.Mappings.Add(4, "Description", typeof(string), true);
                    c.Mappings.Add(5, "Proprietor", typeof(string), true);
                    c.Mappings.Add(6, "Tenant", typeof(string), true);
                    c.Mappings.Add(7, "Occupier", typeof(string), true);
                });

                foreach (DataRow row in dataTable.Rows)
                {
                    var pkey = row["PrimaryKey"] as string;
                    if (string.IsNullOrEmpty(pkey)) continue;

                    var residence = residencesContext.Residences.FirstOrDefault(r => r.PrimaryKey == pkey);
                    if (residence != null)
                    {
                        // update record
                        residencesAdded++;

                        residence.Address = row["Address"] as string;
                        residence.Number = row["Number"] as string;
                        residence.Vrnumber = row["VRNumber"] as string;
                        residence.Vrdescription = row["Description"] as string;
                        residence.Vrproprietor = row["Proprietor"] as string;
                        residence.Vrtenant = row["Tenant"] as string;
                        residence.Vroccupier = row["Occupier"] as string;
                    }
                    else
                    {
                        // add record
                        residencesUpdated++;

                        var newResidence = new Residence
                        {
                            PrimaryKey = pkey,
                            Address = row["Address"] as string,
                            Number = row["Number"] as string,
                            Vrnumber = row["VRNumber"] as string,
                            Vrdescription = row["Description"] as string,
                            Vrproprietor = row["Proprietor"] as string,
                            Vrtenant = row["Tenant"] as string,
                            Vroccupier = row["Occupier"] as string
                        };

                        await residencesContext.Residences.AddAsync(newResidence);
                    }
                }

                await residencesContext.SaveChangesAsync();
            }
        }
    }
}
