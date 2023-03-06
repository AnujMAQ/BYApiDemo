using BYApiDemo.Models;
using Microsoft.AnalysisServices.AdomdClient;
using Microsoft.AnalysisServices.Tabular;
using Microsoft.PowerBI.Api;
using System;
using System.Threading.Tasks;
using Hierarchy = Microsoft.AnalysisServices.Tabular.Hierarchy;
using JsonSerializer = Microsoft.AnalysisServices.Tabular.JsonSerializer;

namespace BYApiDemo.Services
{
    public class DatasetService : IDatasetService
    {
        private readonly IPowerBIClientService _powerBIClientService;
        private readonly GitHubService _gitHubService;

        public DatasetService(IPowerBIClientService powerBIClientService)
        {
            _powerBIClientService = powerBIClientService;
            _gitHubService = new GitHubService();
        }

        public async Task CreateColumn(ColumnData columnData)
        {
            var client = await _powerBIClientService.CreatePowerBIClient();
            string filter = $"id eq '{columnData.WorkspaceId}'";
            var workspace = client.Groups.GetGroups(filter);
            var dataset = client.Datasets.GetDatasetInGroup(new Guid(columnData.WorkspaceId), columnData.DatasetId);

            string connectionString = $"DataSource=powerbi://api.powerbi.com/v1.0/myorg/{workspace.Value[0].Name};Password={Constants.accessToken}";
            using (Server server = new Server())
            {
                server.Connect(connectionString);
                Database database = server.Databases.GetByName(dataset.Name);
                Model model = database.Model;

                if (model != null)
                {
                    if (!columnData.IsCalculated)
                    {
                        var column = new DataColumn()
                        {
                            Name = columnData.NewName,
                            DataType = columnData.Datatype,
                            IsHidden = columnData.IsHidden,
                            FormatString = columnData.FormatString,
                        };
                        model.Tables[columnData.TableName].Columns.Add(column);
                        //model.SaveChanges();
                    }
                    else if (columnData.IsCalculated)
                    {
                        CalculatedColumn column = new CalculatedColumn()
                        {
                            Name = columnData.NewName,
                            DataType = columnData.Datatype,
                            Expression = columnData.Expression,
                            IsHidden = columnData.IsHidden,
                            FormatString = columnData.FormatString,
                        };
                        model.Tables[columnData.TableName].Columns.Add(column);
                        //model.SaveChanges();
                    }
                    var datasetStringified = JsonSerializer.SerializeDatabase(database);
                    var newJsonString = "{\r\n \"createOrReplace\": {\r\n \"object\": {\r\n \"database\": \"" + database.Name + "\"}, \r\n \"database\": " + datasetStringified.ToString() + "\r\n }\r\n }";
                    await _gitHubService.InsertOrUpdate("model.xmla", newJsonString, "AnujMAQ/BYDemo", $"Inserted column {columnData.NewName} to table {columnData.TableName}");
                }
                server.Disconnect();
            }
        }

        public async Task UpdateColumn(ColumnData columnData)
        {
            var client = await _powerBIClientService.CreatePowerBIClient();
            string filter = $"id eq '{columnData.WorkspaceId}'";
            var workspace = client.Groups.GetGroups(filter);
            var dataset = client.Datasets.GetDatasetInGroup(new Guid(columnData.WorkspaceId), columnData.DatasetId);

            string connectionString = $"DataSource=powerbi://api.powerbi.com/v1.0/myorg/{workspace.Value[0].Name};Password={Constants.accessToken}";
            using (Server server = new Server())
            {
                server.Connect(connectionString);
                Database database = server.Databases.GetByName(dataset.Name);
                Model model = database.Model;

                if (model != null)
                {
                    model.Tables[columnData.TableName].Columns[columnData.OldName].DataType = columnData.Datatype;
                    if (columnData.IsCalculated)
                    {
                        (model.Tables[columnData.TableName].Columns[columnData.OldName] as CalculatedColumn).Expression = columnData.Expression;
                    }
                    if (columnData.OldName != columnData.NewName)
                    {
                        model.Tables[columnData.TableName].Columns[columnData.OldName].RequestRename(columnData.NewName);
                    }
                    //model.SaveChanges();
                    var datasetStringified = JsonSerializer.SerializeDatabase(database);
                    var newJsonString = "{\r\n \"createOrReplace\": {\r\n \"object\": {\r\n \"database\": \"" + database.Name + "\"}, \r\n \"database\": " + datasetStringified.ToString() + "\r\n }\r\n }";
                    await _gitHubService.InsertOrUpdate("model.xmla", newJsonString, "AnujMAQ/BYDemo", $"Updated column {columnData.NewName} in table {columnData.TableName}");
                }
                server.Disconnect();
            }
        }

        public async Task DeleteColumn(ColumnData columnData)
        {
            var client = await _powerBIClientService.CreatePowerBIClient();
            string filter = $"id eq '{columnData.WorkspaceId}'";
            var workspace = client.Groups.GetGroups(filter);
            var dataset = client.Datasets.GetDatasetInGroup(new Guid(columnData.WorkspaceId), columnData.DatasetId);

            string connectionString = $"DataSource=powerbi://api.powerbi.com/v1.0/myorg/{workspace.Value[0].Name};Password={Constants.accessToken}";
            using (Server server = new Server())
            {
                server.Connect(connectionString);
                Database database = server.Databases.GetByName(dataset.Name);
                Model model = database.Model;

                if (model != null)
                {
                    model.Tables[columnData.TableName].Columns.Remove(columnData.OldName);
                    //model.SaveChanges();
                    var datasetStringified = JsonSerializer.SerializeDatabase(database);
                    var newJsonString = "{\r\n \"createOrReplace\": {\r\n \"object\": {\r\n \"database\": \"" + database.Name + "\"}, \r\n \"database\": " + datasetStringified.ToString() + "\r\n }\r\n }";
                    await _gitHubService.InsertOrUpdate("model.xmla", newJsonString, "AnujMAQ/BYDemo", $"Deleted column {columnData.NewName} from table {columnData.TableName}");
                }
                server.Disconnect();
            }
        }

        public async Task CreateMeasure(MeasureData measureData)
        {
            var client = await _powerBIClientService.CreatePowerBIClient();
            string filter = $"id eq '{measureData.WorkspaceId}'";
            var workspace = client.Groups.GetGroups(filter);
            var dataset = client.Datasets.GetDatasetInGroup(new Guid(measureData.WorkspaceId), measureData.DatasetId);

            // Connect to workspace using XMLA endpoint
            string connectionString = $"DataSource=powerbi://api.powerbi.com/v1.0/myorg/{workspace.Value[0].Name};Password={Constants.accessToken}";
            using (Server server = new Server())
            {
                server.Connect(connectionString);
                Database database = server.Databases.GetByName(dataset.Name);
                Model model = database.Model;
                var table = model.Tables.Find(measureData.TableName);
                if (table != null)
                {
                    var measure = new Microsoft.AnalysisServices.Tabular.Measure()
                    {
                        Name = measureData.OldMeasureName,
                        Expression = measureData.Expression,
                        FormatString = measureData.FormatString,
                    };
                    table.Measures.Add(measure);
                    //model.SaveChanges();
                    var datasetStringified = JsonSerializer.SerializeDatabase(database);
                    var newJsonString = "{\r\n \"createOrReplace\": {\r\n \"object\": {\r\n \"database\": \"" + database.Name + "\"}, \r\n \"database\": " + datasetStringified.ToString() + "\r\n }\r\n }";
                    await _gitHubService.InsertOrUpdate("model.xmla", newJsonString, "AnujMAQ/BYDemo", $"Inserted measure {measureData.NewMeasureName} to table {measureData.TableName}");
                }
                server.Disconnect();
            }
        }

        public async Task UpdateMeasure(MeasureData measureData)
        {
            var client = await _powerBIClientService.CreatePowerBIClient();
            string filter = $"id eq '{measureData.WorkspaceId}'";
            var workspace = client.Groups.GetGroups(filter);
            var dataset = client.Datasets.GetDatasetInGroup(new Guid(measureData.WorkspaceId), measureData.DatasetId);

            // Connect to workspace using XMLA endpoint
            string connectionString = $"DataSource=powerbi://api.powerbi.com/v1.0/myorg/{workspace.Value[0].Name};Password={Constants.accessToken}";
            using (Server server = new Server())
            {
                server.Connect(connectionString);
                Database database = server.Databases.GetByName(dataset.Name);
                Model model = database.Model;
                var table = model.Tables.Find(measureData.TableName);
                if (table != null)
                {
                    var measure = table.Measures.Find(measureData.OldMeasureName);
                    if (measure != null)
                    {
                        if (!measureData.NewMeasureName.Equals(measure.Name))
                        {
                            measure.Name = measureData.NewMeasureName;
                        }
                        measure.Expression = measureData.Expression;
                    }
                    //model.SaveChanges();
                    var datasetStringified = JsonSerializer.SerializeDatabase(database);
                    var newJsonString = "{\r\n \"createOrReplace\": {\r\n \"object\": {\r\n \"database\": \"" + database.Name + "\"}, \r\n \"database\": " + datasetStringified.ToString() + "\r\n }\r\n }";
                    await _gitHubService.InsertOrUpdate("model.xmla", newJsonString, "AnujMAQ/BYDemo", $"Updated measure {measureData.NewMeasureName} in table {measureData.TableName}");
                }
                server.Disconnect();
            }
        }

        public async Task DeleteMeasure(MeasureData measureData)
        {
            var client = await _powerBIClientService.CreatePowerBIClient();
            string filter = $"id eq '{measureData.WorkspaceId}'";
            var workspace = client.Groups.GetGroups(filter);
            var dataset = client.Datasets.GetDatasetInGroup(new Guid(measureData.WorkspaceId), measureData.DatasetId);

            // Connect to workspace using XMLA endpoint
            string connectionString = $"DataSource=powerbi://api.powerbi.com/v1.0/myorg/{workspace.Value[0].Name};Password={Constants.accessToken}";
            using (Server server = new Server())
            {
                server.Connect(connectionString);
                Database database = server.Databases.GetByName(dataset.Name);
                Model model = database.Model;
                var table = model.Tables.Find(measureData.TableName);
                if (table != null)
                {
                    var measure = table.Measures.Find(measureData.OldMeasureName);
                    if (measure != null)
                    {
                        table.Measures.Remove(measure.Name);
                    }
                    //model.SaveChanges();
                    var datasetStringified = JsonSerializer.SerializeDatabase(database);
                    var newJsonString = "{\r\n \"createOrReplace\": {\r\n \"object\": {\r\n \"database\": \"" + database.Name + "\"}, \r\n \"database\": " + datasetStringified.ToString() + "\r\n }\r\n }";
                    await _gitHubService.InsertOrUpdate("model.xmla", newJsonString, "AnujMAQ/BYDemo", $"Deleted measure {measureData.NewMeasureName} from table {measureData.TableName}");
                }
                server.Disconnect();
            }
        }

        public async Task AddOrUpdateHeirarchy(HierarchyData hierarchyData)
        {
            var client = await _powerBIClientService.CreatePowerBIClient();
            string filter = $"id eq '{hierarchyData.WorkspaceId}'";
            var workspace = client.Groups.GetGroups(filter);
            var dataset = client.Datasets.GetDatasetInGroup(new Guid(hierarchyData.WorkspaceId), hierarchyData.DatasetId);

            // Connect to workspace using XMLA endpoint
            string connectionString = $"DataSource=powerbi://api.powerbi.com/v1.0/myorg/{workspace.Value[0].Name};Password={Constants.accessToken}";
            using (Server server = new Server())
            {
                server.Connect(connectionString);
                Model model = server.Databases.GetByName(dataset.Name).Model;
                var table = model.Tables[hierarchyData.TableName];

                //if (table.Hierarchies.ContainsName(hierarchyData.Name))
                //{
                //    // Update
                //    table.Hierarchies[hierarchyData.Name].Levels = hierarchyData.Levels.Select(item => new Level() { Name = item.Name, Ordinal = item.Ordinal, Column = table.Columns.Find(item.Column) });
                //}
                //else
                //{
                //    // Insert
                //    table.Hierarchies.Add(new Hierarchy() { Name = hierarchyData.Name, Levels = { } });
                //    //hierarchyData.Levels.Select(item => new Level() { Name = item.Name, Ordinal = item.Ordinal, Column = table.Columns.Find(item.Column) }) as LevelCollection,
                //}
            }
        }

        public async Task CreateTable(TableData tableData)
        {
            var client = await _powerBIClientService.CreatePowerBIClient();
            string filter = $"id eq '{tableData.WorkspaceId}'";
            var workspace = client.Groups.GetGroups(filter);
            var dataset = client.Datasets.GetDatasetInGroup(new Guid(tableData.WorkspaceId), tableData.DatasetId);

            // Connect to workspace using XMLA endpoint
            string connectionString = $"DataSource=powerbi://api.powerbi.com/v1.0/myorg/{workspace.Value[0].Name};Password={Constants.accessToken}";
            using (Server server = new Server())
            {
                server.Connect(connectionString);
                // table creation with tom

                server.Disconnect();
            }
        }

        public async Task PublishDataset(string repository)
        {
            var fileResponse = await _gitHubService.GetFile("model.xmla", repository);
            if (!string.IsNullOrEmpty(fileResponse))
            {
                string commandText = fileResponse;

                using (AdomdConnection connection = new AdomdConnection($"Data Source=powerbi://api.powerbi.com/v1.0/myorg/Blue%20Yonder%20POC;Provider=MSOLAP.4;Password={Constants.accessToken}"))
                {
                    using (AdomdCommand cmd = connection.CreateCommand())
                    {
                        try
                        {
                            connection.Open();
                            cmd.CommandText = commandText;
                            var response = cmd.Execute();
                            connection.Close();
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message);
                        }
                        
                    }
                }
            }
        }
    }
}
