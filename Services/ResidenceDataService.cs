//using MapPlotter.Models;
//using OfficeOpenXml;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.IO;
//using System.Linq;
//using System.Threading.Tasks;

//namespace MapPlotter.Services
//{
//    internal class ResidenceDataService
//    {
//        private string filename;

//        public ResidenceDataService(string fileName)
//        {
//            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
//            filename = fileName;
//        }

//        public async Task<List<Residence>> LoadResidences()
//        {
//            var residences = new List<Residence>();

//            using (ExcelPackage excelPackage = new ExcelPackage())
//            {
//                await excelPackage.LoadAsync(filename);
//                var worksheet = excelPackage.Workbook.Worksheets.First();

//                var dataTable = worksheet.Cells["A:K"].ToDataTable(c =>
//                {
//                    c.DataTableName = "Residences";
//                    c.ColumnNameParsingStrategy = OfficeOpenXml.Export.ToDataTable.NameParsingStrategy.RemoveSpace;

//                    c.Mappings.Add(1, "Address", typeof(string), true);
//                    c.Mappings.Add(2, "Number", typeof(string), true);
//                    c.Mappings.Add(3, "Latitude", typeof(string), true);
//                    c.Mappings.Add(4, "Longitude", typeof(string), true);
//                    c.Mappings.Add(5, "Notes", typeof(string), true);
//                    c.Mappings.Add(6, "VRNumber", typeof(string), true);
//                    c.Mappings.Add(7, "Description", typeof(string), true);
//                    c.Mappings.Add(8, "Proprietor", typeof(string), true);
//                    c.Mappings.Add(9, "Tenant", typeof(string), true);
//                    c.Mappings.Add(10, "Occupier", typeof(string), true);
//                });

//                foreach (DataRow row in dataTable.Rows)
//                {
//                    Residence residence = new Residence
//                    {
//                        PrimaryKey = row["PrimaryKey"] as string,
//                        Address = row["Address"] as string,
//                        Latitude = row["Latitude"] as string,
//                        Longitude = row["Longitude"] as string,
//                        Notes = row["Notes"] as string,
//                        Number = row["Number"] as string,
//                        Description = row["Description"] as string,
//                        Proprietor = row["Proprietor"] as string,
//                        Tenant = row["Tenant"] as string,
//                        Occupier = row["Occupier"] as string
//                    };

//                    residences.Add(residence);
//                }
//            }

//            return residences;
//        }

//        public async Task SaveResidences(List<Residence> residences)
//        {
//            using (ExcelPackage excelPackage = new ExcelPackage(new FileInfo(filename)))
//            {
//                var worksheet = excelPackage.Workbook.Worksheets.First();
//                var range = worksheet.Cells["A1:F700"];
//                var start = range.Start;
//                var end = range.End;

//                for (int row = start.Row; row <= end.Row; row++)
//                {
//                    // Row by row...
//                    var primaryKey = worksheet.Cells[row, 1].Text;
//                    var residence = residences.FirstOrDefault(r => r.PrimaryKey == primaryKey);

//                    if (residence != null)
//                    {
//                        worksheet.Cells[row, 4].Value = residence.Latitude;
//                        worksheet.Cells[row, 5].Value = residence.Longitude;
//                        worksheet.Cells[row, 6].Value = residence.Notes;
//                    }
//                }

//                await excelPackage.SaveAsync();
//            }
//        }
//    }
//}
