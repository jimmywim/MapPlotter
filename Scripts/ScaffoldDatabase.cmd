cd ..
dotnet ef dbcontext scaffold "DataSource=Data.db" Microsoft.EntityFrameworkCore.Sqlite --context ResidencesContext --output-dir Data --namespace MapPlotter.Data --force