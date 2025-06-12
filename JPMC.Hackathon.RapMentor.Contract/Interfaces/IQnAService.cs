using JPMC.Hackathon.RapMentor.Contract.Models;
using System.Threading.Tasks;

namespace JPMC.Hackathon.RapMentor.Contract.Interfaces
{
    public interface IQnAService
    {
        Task<string> GetRagQnAAsync(QnAPrompt input);
    }
}
