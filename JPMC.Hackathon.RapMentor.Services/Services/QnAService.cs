using JPMC.Hackathon.RapMentor.Contract.Interfaces;
using JPMC.Hackathon.RapMentor.Contract.Models;
using JPMC.Hackathon.RapMentor.Services.Adapters;
using System.Linq;
using System.Threading.Tasks;

namespace JPMC.Hackathon.RapMentor.Services.Services
{
    public class QnAService : IQnAService
    {
        public async Task<string> GetRagQnAAsync(QnAPrompt input)
        {
            var res =  await BedrockKBAdapter.GetRagQnAAsync(input.Prompts.Last().Content);
            return await BedrockKBAdapter.GenerateMessageAsync(res, input);
        }
       
    }
}
