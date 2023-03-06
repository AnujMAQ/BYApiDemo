using Microsoft.AnalysisServices.Tabular;

namespace BYApiDemo.Models
{
    public class MeasureData: BaseClass
    {
        public string OldMeasureName { get; set; }
        public string NewMeasureName { get; set; }
        public string Expression { get; set; }
        public bool IsHidden { get; set; }
        public DataType Datatype { get; set; }
        public string Description { get; set; }
        public string FormatString { get; set; }
        public string TableName { get; set; }

        public MeasureData(string workspaceId, string datasetId, string oldMeasureName, string newMeasureName, string expression, bool isHidden, DataType datatype, string description, string formatString, string tableName): base(workspaceId, datasetId)
        {
            OldMeasureName = oldMeasureName;
            NewMeasureName = newMeasureName;
            Expression = expression;
            IsHidden = isHidden;
            Datatype = datatype;
            Description = description;
            FormatString = formatString;
            TableName = tableName;
        }
    }
}
