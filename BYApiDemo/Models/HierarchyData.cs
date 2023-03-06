using System.Collections.Generic;

namespace BYApiDemo.Models
{
    public class HierarchyData: BaseClass
    {
        public string Name { get; set; }
        public List<Level> Levels { get; set; }
        public string TableName { get; set; }

        public HierarchyData(string workspaceId, string datasetId, string name, List<Level> levels, string tableName): base(workspaceId, datasetId)
        {
            Name = name;
            Levels = levels;
            TableName = tableName;
        }
    }

    public class Level
    {
        public int Ordinal { get; set; }
        public string Name { get; set; }
        public string Column { get; set; }

        public Level(int ordinal, string name, string column)
        {
            Ordinal = ordinal;
            Name = name;
            Column = column;
        }
    }
}
