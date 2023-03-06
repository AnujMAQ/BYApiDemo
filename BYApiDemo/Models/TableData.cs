namespace BYApiDemo.Models
{
    public class TableData: BaseClass
    {
        public string NewName { get; set; }
        public string OldName { get; set; }

        public TableData(string workspaceId, string datasetId, string newName, string oldName): base(workspaceId, datasetId)
        {
            NewName = newName;
            OldName = oldName;
        }
    }
}
