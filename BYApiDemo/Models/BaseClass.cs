namespace BYApiDemo.Models
{
    public class BaseClass
    {
        public string WorkspaceId { get; set; }
        public string DatasetId { get; set; }

        public BaseClass(string workspaceId, string datasetId)
        {
            WorkspaceId = workspaceId;
            DatasetId = datasetId;
        }
    }
}
