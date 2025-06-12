using JPMC.Hackathon.RapMentor.Contract.Models;

namespace JPMC.Hackathon.RapMentor.Contract.Interfaces
{
    public interface IQnAService
    {
        Task<string> GetAsync(string input);
    }
}
