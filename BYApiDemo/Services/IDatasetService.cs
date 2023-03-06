using BYApiDemo.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BYApiDemo.Services
{
    public interface IDatasetService
    {
        Task CreateColumn(List<ColumnData> columnData);
        Task UpdateColumn(List<ColumnData> columnData);
        Task DeleteColumn(List<ColumnData> columnData);
        Task CreateMeasure(List<MeasureData> measureData);
        Task UpdateMeasure(List<MeasureData> measureData);
        Task DeleteMeasure(List<MeasureData> measureData);
        Task AddOrUpdateHeirarchy(HierarchyData hierarchyData);
        Task PublishDataset(string repository);
        Task UpdateAllTables(List<ColumnData> columnData);

        Task DeleteTable(List<TableData> tableData);
    }
}
