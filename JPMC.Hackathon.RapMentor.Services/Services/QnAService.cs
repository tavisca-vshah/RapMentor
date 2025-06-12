using JPMC.Hackathon.RapMentor.Contract.Interfaces;
using JPMC.Hackathon.RapMentor.Contract.Models;
using JPMC.Hackathon.RapMentor.Services.Adapters;

namespace JPMC.Hackathon.RapMentor.Services.Services
{
    public class QnAService : IQnAService
    {
        public async Task<string> GetRagQnAAsync(QnAPrompt input)
        {
            return await BedrockKBAdapter.GetRagQnAAsync(input);
        }
    }
}
