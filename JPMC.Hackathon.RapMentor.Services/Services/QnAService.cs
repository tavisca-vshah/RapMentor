using JPMC.Hackathon.RapMentor.Contract.Interfaces;
using JPMC.Hackathon.RapMentor.Services.Adapters;

namespace JPMC.Hackathon.RapMentor.Services.Services
{
    public class QnAService : IQnAService
    {
        public async Task<string> GetAsync(string input)
        {
            return await BedrockKBAdapter.GetAsync(input);
        }
    }
}
