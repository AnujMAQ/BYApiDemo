using BYApiDemo.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BYApiDemo.Services
{
    public class GitHubService
    {
        public HttpClient GetClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", "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");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.raw"));
            client.DefaultRequestHeaders.UserAgent.TryParseAdd("request");
            return client;
        }

        public async Task<string> GetFile(string fileName, string repository)
        {
            using (var client = GetClient())
            {
                var response = await client.GetAsync($"https://api.github.com/repos/{repository}/contents/{fileName}");
                if (response.StatusCode == HttpStatusCode.NotFound) return "";
                return response.Content.ReadAsStringAsync().Result;
            }
        }

        public async Task InsertOrUpdate(string fileName, string xmlaScript, string repository, string commitMessage)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(xmlaScript);
            var fileContent = Convert.ToBase64String(bytes);
            var fileResponse = await GetFile(fileName, repository);
            var payload = new Payload("Inserted new model file xmla script model.xmla", new Committer("shyam", "shyam@maqsoftware.com"), fileContent);
            var client = GetClient();
            if (string.IsNullOrEmpty(fileResponse))
            {
                //Insert
                var json = JsonConvert.SerializeObject(payload);
                var url = $"https://api.github.com/repos/{repository}/contents/{fileName}";
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PutAsync(url, content);
            }
            else
            {
                //Update
                var fileMetadata = JsonConvert.DeserializeObject<FileMetadata>(fileResponse);
                payload.message = commitMessage;
                payload.sha = fileMetadata.sha;

                var json = JsonConvert.SerializeObject(payload);
                var url = $"https://api.github.com/repos/{repository}/contents/{fileName}";
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PutAsync(url, content);
            }
        }
    }
}
