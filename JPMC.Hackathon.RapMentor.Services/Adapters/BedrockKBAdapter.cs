using Amazon;
using Amazon.BedrockAgentRuntime;
using Amazon.BedrockAgentRuntime.Model;
using Amazon.BedrockRuntime;
using Amazon.BedrockRuntime.Model;
using JPMC.Hackathon.RapMentor.Contract.Models;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace JPMC.Hackathon.RapMentor.Services.Adapters
{
    public class BedrockKBAdapter
    {
        static string modelArn = "anthropic.claude-3-sonnet-20240229-v1:0";
        public static async Task<string> GetRagQnAAsync(QnAPrompt prompt)
        {

            string knowledgeBaseId = "R9FO1ASDHC";

            var client = new AmazonBedrockAgentRuntimeClient(RegionEndpoint.USEast1);

            try
            {
                var request = new RetrieveAndGenerateRequest
                {
                    Input = new RetrieveAndGenerateInput
                    {
                        Text = prompt.Prompts.Last().Content,
                    },
                    RetrieveAndGenerateConfiguration = new RetrieveAndGenerateConfiguration
                    {
                        KnowledgeBaseConfiguration = new KnowledgeBaseRetrieveAndGenerateConfiguration
                        {
                            KnowledgeBaseId = knowledgeBaseId,
                            ModelArn = modelArn
                        },
                        Type = RetrieveAndGenerateType.KNOWLEDGE_BASE,

                    },
                    SessionId = "c2d088d6-82af-4543-865a-67ae79ce343d"
                };

                var response = await client.RetrieveAndGenerateAsync(request);

                return await GenerateMessageAsync(response.Output.Text, prompt);
            }
            catch (AmazonBedrockAgentRuntimeException ex)
            {
                // Handle specific exceptions related to AWS SDK
                Console.WriteLine($"AWS Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Handle other potential exceptions
                Console.WriteLine($"General Error: {ex.Message}");
            }
            return "";
        }


        private static async Task<string> GenerateMessageAsync(string context, QnAPrompt question)
        {
            question.Prompts.Add(new Prompt { Role = "Assistant", Content = context });
            var client = new AmazonBedrockRuntimeClient(RegionEndpoint.USEast1);

            int promptsSize = question.Prompts.Count;
            string que = question.Prompts[promptsSize - 2].Content;
            // Define the user message.
            var userMessage = $"Use the context below if relevant. Context:\n\"\"\"\n{JsonSerializer.Serialize(question)}\n\"\"\"\n\n Question: {que}";

            //Format the request payload using the model's native structure.
            var nativeRequest = JsonSerializer.Serialize(new
            {
                anthropic_version = "bedrock-2023-05-31",
                max_tokens = 512,
                temperature = 0.5,
                messages = new[]
                    {
                            new { role = "user", content = userMessage }
                    }
            });

            // Create a request with the model ID and the model's native request payload.
            var request = new InvokeModelRequest()
            {
                ModelId = modelArn,
                Body = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(nativeRequest)),
                ContentType = "application/json"
            };

            try
            {
                // Send the request to the Bedrock Runtime and wait for the response.
                var response = await client.InvokeModelAsync(request);

                // Decode the response body.
                var modelResponse = await JsonNode.ParseAsync(response.Body);

                // Extract and print the response text.
                var responseText = modelResponse["content"]?[0]?["text"] ?? "";
                return responseText.ToString();
            }
            catch (AmazonBedrockRuntimeException e)
            {
                Console.WriteLine($"ERROR: Can't invoke '{modelArn}'. Reason: {e.Message}");
                throw;
            }

        }
    }

}
