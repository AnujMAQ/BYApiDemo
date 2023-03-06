using Microsoft.PowerBI.Api;
using Microsoft.Rest;
using System;
using System.Threading.Tasks;

namespace BYApiDemo.Services
{
    public class PowerBIClientService: IPowerBIClientService
    {
        public async Task<PowerBIClient> CreatePowerBIClient()
        {
            var credentials = new TokenCredentials(Constants.accessToken, "Bearer");
            var client = new PowerBIClient(new Uri("https://api.powerbi.com"), credentials);
            return client;
        }
    }
}
