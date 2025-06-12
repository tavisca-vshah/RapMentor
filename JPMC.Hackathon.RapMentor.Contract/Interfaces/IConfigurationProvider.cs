using System.Threading.Tasks;

namespace JPMC.Hackathon.RapMentor.Contract.Interfaces
{
    public interface IConfigurationProvider
    {
        Task<string> GetConfiguration(string key);
    }
}
