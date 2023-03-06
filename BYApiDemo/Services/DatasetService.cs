using BYApiDemo.Models;
using Microsoft.AnalysisServices.AdomdClient;
using Microsoft.AnalysisServices.Tabular;
using Microsoft.PowerBI.Api;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public async Task CreateColumn(List<ColumnData> columnData)
        {
            var client = await _powerBIClientService.CreatePowerBIClient();
            string filter = $"id eq '{columnData[0].WorkspaceId}'";
            var workspace = client.Groups.GetGroups(filter);
            var dataset = client.Datasets.GetDatasetInGroup(new Guid(columnData[0].WorkspaceId), columnData[0].DatasetId);

            string connectionString = $"DataSource=powerbi://api.powerbi.com/v1.0/myorg/{workspace.Value[0].Name};Password={Constants.accessToken}";
            using (Server server = new Server())
            {
                server.Connect(connectionString);
                Database database = server.Databases.GetByName(dataset.Name);
                Model model = database.Model;
                foreach (var data in columnData)
                {
                    if (model != null)
                    {
                        if (!data.IsCalculated)
                        {
                            var column = new DataColumn()
                            {
                                Name = data.NewName,
                                DataType = data.Datatype,
                                IsHidden = data.IsHidden,
                                FormatString = data.FormatString,
                            };
                            model.Tables[data.TableName].Columns.Add(column);
                            //model.SaveChanges();
                        }
                        else if (data.IsCalculated)
                        {
                            CalculatedColumn column = new CalculatedColumn()
                            {
                                Name = data.NewName,
                                DataType = data.Datatype,
                                Expression = data.Expression,
                                IsHidden = data.IsHidden,
                                FormatString = data.FormatString,
                            };
                            model.Tables[data.TableName].Columns.Add(column);

                        }

                    }
                }
                var datasetStringified = JsonSerializer.SerializeDatabase(database);
                var newJsonString = "{\r\n \"createOrReplace\": {\r\n \"object\": {\r\n \"database\": \"" + database.Name + "\"}, \r\n \"database\": " + datasetStringified.ToString() + "\r\n }\r\n }";
                await _gitHubService.InsertOrUpdate("model.xmla", newJsonString, "AnujMAQ/BYDemo", $"Inserted column {columnData[0].NewName}");
                //model.SaveChanges();
                server.Disconnect();
            }
        }

        public async Task UpdateColumn(List<ColumnData> columnData)
        {
            var client = await _powerBIClientService.CreatePowerBIClient();
            string filter = $"id eq '{columnData[0].WorkspaceId}'";
            var workspace = client.Groups.GetGroups(filter);
            var dataset = client.Datasets.GetDatasetInGroup(new Guid(columnData[0].WorkspaceId), columnData[0].DatasetId);

            string connectionString = $"DataSource=powerbi://api.powerbi.com/v1.0/myorg/{workspace.Value[0].Name};Password={Constants.accessToken}";
            using (Server server = new Server())
            {
                server.Connect(connectionString);
                Database database = server.Databases.GetByName(dataset.Name);
                Model model = database.Model;
                foreach (var data in columnData)
                {
                    if (model != null)
                    {
                        model.Tables[data.TableName].Columns[data.OldName].DataType = data.Datatype;
                        if (data.IsCalculated)
                        {
                            (model.Tables[data.TableName].Columns[data.OldName] as CalculatedColumn).Expression = data.Expression;
                        }
                        if (data.OldName != data.NewName)
                        {
                            model.Tables[data.TableName].Columns[data.OldName].RequestRename(data.NewName);
                        }

                    }
                }
                model.SaveChanges();
                var datasetStringified = JsonSerializer.SerializeDatabase(database);
                var newJsonString = "{\r\n \"createOrReplace\": {\r\n \"object\": {\r\n \"database\": \"" + database.Name + "\"}, \r\n \"database\": " + datasetStringified.ToString() + "\r\n }\r\n }";
                await _gitHubService.InsertOrUpdate("model.xmla", newJsonString, "AnujMAQ/BYDemo", $"Updated column");
                server.Disconnect();
            }
        }

        public async Task DeleteColumn(List<ColumnData> columnData)
        {
            var client = await _powerBIClientService.CreatePowerBIClient();
            string filter = $"id eq '{columnData[0].WorkspaceId}'";
            var workspace = client.Groups.GetGroups(filter);
            var dataset = client.Datasets.GetDatasetInGroup(new Guid(columnData[0].WorkspaceId), columnData[0].DatasetId);

            string connectionString = $"DataSource=powerbi://api.powerbi.com/v1.0/myorg/{workspace.Value[0].Name};Password={Constants.accessToken}";
            using (Server server = new Server())
            {
                server.Connect(connectionString);
                Database database = server.Databases.GetByName(dataset.Name);
                Model model = database.Model;
                foreach (var data in columnData)
                {
                    if (model != null)
                    {
                        model.Tables[data.TableName].Columns.Remove(data.OldName);

                    }
                }
                var datasetStringified = JsonSerializer.SerializeDatabase(database);
                var newJsonString = "{\r\n \"createOrReplace\": {\r\n \"object\": {\r\n \"database\": \"" + database.Name + "\"}, \r\n \"database\": " + datasetStringified.ToString() + "\r\n }\r\n }";
                await _gitHubService.InsertOrUpdate("model.xmla", newJsonString, "AnujMAQ/BYDemo", $"Deleted column {columnData[0].OldName}");
                model.SaveChanges();
                server.Disconnect();
            }
        }

        public async Task CreateMeasure(List<MeasureData> measureData)
        {
            var client = await _powerBIClientService.CreatePowerBIClient();
            string filter = $"id eq '{measureData[0].WorkspaceId}'";
            var workspace = client.Groups.GetGroups(filter);
            var dataset = client.Datasets.GetDatasetInGroup(new Guid(measureData[0].WorkspaceId), measureData[0].DatasetId);

            // Connect to workspace using XMLA endpoint
            string connectionString = $"DataSource=powerbi://api.powerbi.com/v1.0/myorg/{workspace.Value[0].Name};Password={Constants.accessToken}";
            using (Server server = new Server())
            {
                server.Connect(connectionString);
                Database database = server.Databases.GetByName(dataset.Name);
                Model model = database.Model;
                var table = model.Tables.Find(measureData[0].TableName);
                foreach (var data in measureData)
                {
                    if (table != null)
                    {
                        var measure = new Microsoft.AnalysisServices.Tabular.Measure()
                        {
                            Name = data.OldMeasureName,
                            Expression = data.Expression,
                            FormatString = data.FormatString,
                        };
                        table.Measures.Add(measure);
                    }
                }
                    model.SaveChanges();
                    var datasetStringified = JsonSerializer.SerializeDatabase(database);
                    var newJsonString = "{\r\n \"createOrReplace\": {\r\n \"object\": {\r\n \"database\": \"" + database.Name + "\"}, \r\n \"database\": " + datasetStringified.ToString() + "\r\n }\r\n }";
                    await _gitHubService.InsertOrUpdate("model.xmla", newJsonString, "AnujMAQ/BYDemo", $"Inserted {measureData[0].NewMeasureName} measure");
                server.Disconnect();
            }
                
        }
        

        public async Task UpdateMeasure(List<MeasureData> measureData)
        {
            var client = await _powerBIClientService.CreatePowerBIClient();
            string filter = $"id eq '{measureData[0].WorkspaceId}'";
            var workspace = client.Groups.GetGroups(filter);
            var dataset = client.Datasets.GetDatasetInGroup(new Guid(measureData[0].WorkspaceId), measureData[0].DatasetId);

            // Connect to workspace using XMLA endpoint
            string connectionString = $"DataSource=powerbi://api.powerbi.com/v1.0/myorg/{workspace.Value[0].Name};Password={Constants.accessToken}";
            using (Server server = new Server())
            {
                server.Connect(connectionString);
                Database database = server.Databases.GetByName(dataset.Name);
                Model model = database.Model;
                var table = model.Tables.Find(measureData[0].TableName);
                foreach(var data in measureData)
                {
                    if (table != null)
                    {
                        var measure = table.Measures.Find(measureData[0].OldMeasureName);
                        if (measure != null)
                        {
                            if (!data.NewMeasureName.Equals(measure.Name))
                            {
                                measure.Name = data.NewMeasureName;
                            }
                            measure.Expression = data.Expression;
                        }

                    }
                    
                }
                model.SaveChanges();
                var datasetStringified = JsonSerializer.SerializeDatabase(database);
                var newJsonString = "{\r\n \"createOrReplace\": {\r\n \"object\": {\r\n \"database\": \"" + database.Name + "\"}, \r\n \"database\": " + datasetStringified.ToString() + "\r\n }\r\n }";
                await _gitHubService.InsertOrUpdate("model.xmla", newJsonString, "AnujMAQ/BYDemo", $"Updated measure {measureData[0].NewMeasureName} in table {measureData[0].TableName}");
                server.Disconnect();
            }
        }

        public async Task DeleteMeasure(List<MeasureData> measureData)
        {
            var client = await _powerBIClientService.CreatePowerBIClient();
            string filter = $"id eq '{measureData[0].WorkspaceId}'";
            var workspace = client.Groups.GetGroups(filter);
            var dataset = client.Datasets.GetDatasetInGroup(new Guid(measureData[0].WorkspaceId), measureData[0].DatasetId);

            // Connect to workspace using XMLA endpoint
            string connectionString = $"DataSource=powerbi://api.powerbi.com/v1.0/myorg/{workspace.Value[0].Name};Password={Constants.accessToken}";
            using (Server server = new Server())
            {
                server.Connect(connectionString);
                Database database = server.Databases.GetByName(dataset.Name);
                Model model = database.Model;
                var table = model.Tables.Find(measureData[0].TableName);
                foreach (var data in measureData)
                {
                    if (table != null)
                    {
                        var measure = table.Measures.Find(data.OldMeasureName);
                        if (measure != null)
                        {
                            table.Measures.Remove(measure.Name);
                        }
                        //model.SaveChanges();
                        
                    }
                }
                var datasetStringified = JsonSerializer.SerializeDatabase(database);
                var newJsonString = "{\r\n \"createOrReplace\": {\r\n \"object\": {\r\n \"database\": \"" + database.Name + "\"}, \r\n \"database\": " + datasetStringified.ToString() + "\r\n }\r\n }";
                await _gitHubService.InsertOrUpdate("model.xmla", newJsonString, "AnujMAQ/BYDemo", $"Deleted measure {measureData[0].NewMeasureName} from table {measureData[0].TableName}");
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

        public async Task DeleteTable(List<TableData> tableData)
        {
            var client = await _powerBIClientService.CreatePowerBIClient();
            string filter = $"id eq '{tableData[0].WorkspaceId}'";
            var workspace = client.Groups.GetGroups(filter);
            var dataset = client.Datasets.GetDatasetInGroup(new Guid(tableData[0].WorkspaceId), tableData[0].DatasetId);

            string connectionString = $"DataSource=powerbi://api.powerbi.com/v1.0/myorg/{workspace.Value[0].Name};Password={Constants.accessToken}";
            using (Server server = new Server())
            {
                try
                {
                    server.Connect(connectionString);
                    Database database = server.Databases.GetByName(dataset.Name);
                    Model model = database.Model;
                    foreach (var table in tableData)
                    {
                        if (!table.NewName.ToLower().Contains("date"))
                        {
                            model.Tables.Remove(table.NewName);

                        }
                    }
                    model.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

            }
        }

        public async Task CreateTable(TableData tableData)
        {

        }
        public async Task UpdateAllTables(List<ColumnData> columnData)
        {
            var client = await _powerBIClientService.CreatePowerBIClient();
            string filter = $"id eq '{columnData[0].WorkspaceId}'";
            var workspace = client.Groups.GetGroups(filter);
            var dataset = client.Datasets.GetDatasetInGroup(new Guid(columnData[0].WorkspaceId), columnData[0].DatasetId);

            string connectionString = $"DataSource=powerbi://api.powerbi.com/v1.0/myorg/{workspace.Value[0].Name};Password={Constants.accessToken}";
            using (Server server = new Server())
            {
                server.Connect(connectionString);
                Database database = server.Databases.GetByName(dataset.Name);
                Model model = database.Model;
                foreach (var table in model.Tables)
                {
                    foreach (var data in columnData)
                    {
                        if (!table.Name.ToLower().Contains("date"))
                        {
                            var column = new DataColumn()
                            {
                                Name = data.NewName,
                                DataType = data.Datatype,
                                IsHidden = data.IsHidden,
                                FormatString = data.FormatString
                            };
                            table.Columns.Add(column);
                        }
                    }
                }
                model.SaveChanges();
            }

        }
        public async Task PublishDataset(string repository)
        {
            var fileResponse = await _gitHubService.GetFile("model.xmla", repository);
            if (!string.IsNullOrEmpty(fileResponse))
            {
                //var fileMetadata = JsonConvert.DeserializeObject<FileMetadata>(fileResponse);
                //byte[] data = Convert.FromBase64String(fileMetadata.content);
                string commandText = fileResponse;

                using (AdomdConnection connection = new AdomdConnection($"Data Source=powerbi://api.powerbi.com/v1.0/myorg/Blue%20Yonder%20POC;Provider=MSOLAP.4;Password={Constants.accessToken}"))
                {
                    using (AdomdCommand cmd = connection.CreateCommand())
                    {
                        connection.Open();
                        cmd.CommandText = commandText;
                        cmd.ExecuteNonQuery();
                        connection.Close();
                    }
                }
            }
        }
    }
}
