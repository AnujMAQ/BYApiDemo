namespace BYApiDemo.Models
{
    public class GitHubData
    {
    }

    public class Payload
    {
        public string message { get; set; }
        public Committer committer { get; set; }
        public string content { get; set; }
        public string? sha { get; set; }
        public Payload(string message, Committer committer, string content, string? sha = "")
        {
            this.message = message;
            this.committer = committer;
            this.content = content;
            this.sha = sha;
        }
    }

    public class Committer
    {
        public string name { get; set; }
        public string email { get; set; }
        public Committer(string name, string email)
        {
            this.name = name;
            this.email = email;
        }
    }

    public class FileMetadata
    {
        public string name { get; set; }
        public string sha { get; set; }
        public string content { get; set; }
        public FileMetadata(string name, string sha, string content)
        {
            this.name = name;
            this.sha = sha;
            this.content = content;
        }
    }
}
