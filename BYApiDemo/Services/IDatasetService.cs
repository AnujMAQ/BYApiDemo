using BYApiDemo.Models;
using System.Threading.Tasks;

namespace BYApiDemo.Services
{
    public interface IDatasetService
    {
        Task CreateColumn(ColumnData columnData);
        Task UpdateColumn(ColumnData columnData);
        Task DeleteColumn(ColumnData columnData);
        Task CreateMeasure(MeasureData measureData);
        Task UpdateMeasure(MeasureData measureData);
        Task DeleteMeasure(MeasureData measureData);
        Task AddOrUpdateHeirarchy(HierarchyData hierarchyData);
        Task PublishDataset(string repository);
    }
}
