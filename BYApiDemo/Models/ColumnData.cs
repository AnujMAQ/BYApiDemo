using Microsoft.AnalysisServices.Tabular;
using Newtonsoft.Json;

namespace BYApiDemo.Models
{
    public class ColumnData: BaseClass
    {
        public string OldName { get; set; }
        public string NewName { get; set; }
        public DataType Datatype { get; set; }
        public string FormatString { get; set; }
        public string TableName { get; set; }
        public bool IsHidden { get; set; }
        public bool IsCalculated { get; set; }
        public string Expression { get; set; }

        [JsonConstructor]
        public ColumnData(string workspaceId, string datasetId, string oldName, string newName, DataType datatype, string formatString, string tableName, bool isHidden, bool isCalculated, string expression): base(workspaceId, datasetId)
        {
            OldName = oldName;
            NewName = newName;
            Datatype = datatype;
            FormatString = formatString;
            TableName = tableName;
            IsHidden = isHidden;
            IsCalculated = isCalculated;
            Expression = expression;
        }
    }
}
