using Microsoft.PowerBI.Api;
using System.Threading.Tasks;

namespace BYApiDemo.Services
{
    public interface IPowerBIClientService
    {
        Task<PowerBIClient> CreatePowerBIClient();
    }
}
