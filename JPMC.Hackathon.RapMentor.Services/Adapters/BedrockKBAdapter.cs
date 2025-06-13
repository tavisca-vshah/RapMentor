using Amazon;
using Amazon.BedrockAgentRuntime;
using Amazon.BedrockAgentRuntime.Model;
using Amazon.BedrockRuntime;
using Amazon.BedrockRuntime.Model;
using JPMC.Hackathon.RapMentor.Contract.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace JPMC.Hackathon.RapMentor.Services.Adapters
{
    public class BedrockKBAdapter
    {
        static string modelArn = "anthropic.claude-3-sonnet-20240229-v1:0";
        static string knowledgeBaseId = "UUUDGIZAZM";
        public static async Task<string> GetRagQnAAsync(string prompt)
        {
            var text = @"
                You are an AI Q&A bot with RAG capabilities. Provide accurate, professional responses using company data first, then supplement with external sources when needed.

                **Response Priority:**
                1. Use company data as primary source
                2. Supplement with external sources only if company data is insufficient  
                3. Clearly cite all external references

                **Guidelines:**
                - Professional, clear, and concise tone
                - Structure with bullet points/headings when helpful
                - Provide only verified information if not then return probable result
                - State 'Based on available information' if uncertain
                - Include brief explanations for technical topics

                Question:" + prompt;

            
            return await RagAsync(text);
        }

        public static async Task<string> GetRagGoalAsync(GoalRequest request)
        {
            var text = @"
                You are an AI Q&A assistant with Retrieval-Augmented Generation (RAG) capabilities. Your task is to provide personalized, role-specific skill improvement recommendations for employees. Use internal company documents as the primary source, especially those categorized by role, team, and skill development. Supplement with verified external sources only if internal data is insufficient.
                **Guidelines:**
                - Professional, clear, and concise tone
                - Structure with bullet points/headings when helpful
                - Provide only verified information if not then return probable result
                - State 'Based on available information' if uncertain
                - Include brief explanations for technical topics
                - Give role information and team information also
                - Give more details as possible 

                Employee Details:
                   " +
                   JsonSerializer.Serialize(request)
                   +
                @"
                Please suggest actionable steps or learning paths tailored for the employee's role.";

            return await RagAsync(text);
        }


        public static async Task<string> GenerateMessageAsync(string context, QnAPrompt question)
        {

            var messages = new List<Prompt>();

            var sysPrompt = @"
                You are an AI Q&A bot with RAG capabilities. Provide accurate, professional responses using company data first, then supplement with external sources when needed.

                **Response Priority:**
                1. Use company data as primary source
                2. Supplement with external sources only if company data is insufficient  
                3. Clearly cite all external references

                **Guidelines:**
                - Professional, clear, and concise tone
                - Structure with bullet points/headings when helpful
                - Provide only verified information if not then return probable result
                - State 'Based on available information' if uncertain
                - Include brief explanations for technical topics
                - Use proper spacing, line breaks, and indentation to improve readability.
                - Add new lines between paragraphs, sections, and code blocks to avoid clutter.
                - Ensure code blocks or examples are clearly formatted and indented.
                - Avoid long paragraphs; keep sentences and sections concise.

                **Example:**
                User: 'What is ID Everywhere Authentication System?'
                Response: 'ID Everywhere Authentication provides secure identity verification across platforms, featuring multi-factor authentication (MFA), single sign-on (SSO), federated identity management, and adaptive security protocols.'
                ";

            question.Prompts.Add(new Prompt { role = "Assistant", content = context });
            int promptsSize = question.Prompts.Count;
            string que = question.Prompts[promptsSize - 2].content;
            // Define the user message.
            var userMessage = $"{sysPrompt} + \n\"\"\"\n Use the context below if relevant. Context:\n\"\"\"\n{JsonSerializer.Serialize(question)}\n\"\"\"\n\n Question: {que}";
            
            messages.Add(new Prompt { role = "user", content = userMessage });
            //Format the request payload using the model's native structure.
            var nativeRequest = JsonSerializer.Serialize(new
            {
                anthropic_version = "bedrock-2023-05-31",
                max_tokens = 512,
                temperature = 0.5,
                messages = messages
            });

            // Create a request with the model ID and the model's native request payload.
            var request = new InvokeModelRequest()
            {
                ModelId = modelArn,
                Body = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(nativeRequest)),
                ContentType = "application/json"
            };

            return await InvokeModelAsync(request);

        }

        public static async Task<List<string>> GetCourseHeadings(CourseHeadersRequest courseHeadersRequest)
        {
            var messages = new List<Prompt> { new Prompt
            {
                role = "user",
                content = @"
                    You are a course architect and learning experience designer. Your task is to generate a comma-separated list of high-level topic headings for a course based on the provided context.
 
                    Design the course using the following structure:
                    1. CourseName/ProductName Overview[only if we can write content about that product]
                    2. Technologies Used
                    3. Services or Tools Used
                    4. System Architecture
                    5. Core Functional Modules (based on the course prompt and skills)
                    6. Additional Modules (based on the 'Additional Modules' input)
 
                    Ensure the course is appropriate for the specified level and duration. The output should be a single, logically ordered, comma-separated list of topic headings.
 
                    Example:
                    For a course on 'Learn Web Development' with skills 'HTML, CSS', and additional modules 'Responsive Design, SEO Basics', the output might be:
                    Course Overview, Technologies Used, Services or Tools Used, System Architecture, HTML Basics, CSS Fundamentals, Building Your First Web Page, Responsive Design, SEO Basics
 
                    Respond with only the topic headings, separated by commas. Do not include any explanations or formatting.
                    
                    query: " + JsonSerializer.Serialize(courseHeadersRequest)
            }
            };

            var nativeRequest = JsonSerializer.Serialize(new
            {
                anthropic_version = "bedrock-2023-05-31",
                max_tokens = 512,
                temperature = 0.5,
                messages = messages
            });

            // Create a request with the model ID and the model's native request payload.
            var request = new InvokeModelRequest()
            {
                ModelId = modelArn,
                Body = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(nativeRequest)),
                ContentType = "application/json"
            };

            var res = await InvokeModelAsync(request);
            return res.Split(",").ToList();

        }

        public static async Task<string> Getheadercontent(HeaderContentRequest courseHeadersRequest)
        {
            var messages = new List<Prompt> { new Prompt
            {
                role = "user",
                content = $@"
                You are a course content generator. Your task is to create detailed, beginner-friendly content for a specific course topic, based on the overall course context.
 
                Guidelines:
                - Content specific to header provided in query
                - Write in a clear, instructional tone suitable for the intended audience.
                - Use markdown formatting (## for headings, bullet points, numbered lists, etc.).
                - Include relevant examples, use cases, or scenarios to enhance understanding.
                - If applicable, include step-by-step explanations, frameworks, or best practices.
                - Ensure the content aligns with the course's skills, level, and duration.
                - Adapt the tone and depth based on whether the topic is technical, business-oriented, or soft-skill related.
                - Keep the total content under 700 words or 4,000 characters to ensure readability and UI compatibility.
 
                Do not include introductory or closing remarks unless the topic itself requires it.
                
               
                Query: " + JsonSerializer.Serialize(courseHeadersRequest)
            }
            };

            var nativeRequest = JsonSerializer.Serialize(new
            {
                anthropic_version = "bedrock-2023-05-31",
                max_tokens = 512,
                temperature = 0.5,
                messages = messages
            });

            // Create a request with the model ID and the model's native request payload.
            var request = new InvokeModelRequest()
            {
                ModelId = modelArn,
                Body = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(nativeRequest)),
                ContentType = "application/json"
            };

            return await InvokeModelAsync(request);
        }

        public static async Task<string> GetCourseSummerization(CourseHeadersRequest courseHeadersRequest)
        {
            var messages = new List<Prompt> { new Prompt
            {
                role = "user",
                content = $@"
                You are a professional course content writer. Based on the following input, generate a compelling and informative course description suitable for a course catalog or website. The description should be clear, engaging, and tailored to the target audience.
 
                {JsonSerializer.Serialize(courseHeadersRequest)}
 
                Write a paragraph that introduces the course, highlights the skills learners will gain, specifies the target audience based on the level, mentions the duration, and includes the value of the additional modules. Keep the tone professional and informative." 
            }
            };

            var nativeRequest = JsonSerializer.Serialize(new
            {
                anthropic_version = "bedrock-2023-05-31",
                max_tokens = 512,
                temperature = 0.5,
                messages = messages
            });

            // Create a request with the model ID and the model's native request payload.
            var request = new InvokeModelRequest()
            {
                ModelId = modelArn,
                Body = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(nativeRequest)),
                ContentType = "application/json"
            };

            return await InvokeModelAsync(request);
        }

        private static async Task<string> InvokeModelAsync(InvokeModelRequest request)
        {
            try
            {
                var client = new AmazonBedrockRuntimeClient(RegionEndpoint.USEast1);
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

        private static async Task<string> RagAsync(string text)
        {
            try
            {
                var client = new AmazonBedrockAgentRuntimeClient(RegionEndpoint.USEast1);
                var request = new RetrieveAndGenerateRequest
                {
                    Input = new RetrieveAndGenerateInput
                    {
                        Text = text,
                    },
                    RetrieveAndGenerateConfiguration = new RetrieveAndGenerateConfiguration
                    {
                        KnowledgeBaseConfiguration = new KnowledgeBaseRetrieveAndGenerateConfiguration
                        {
                            KnowledgeBaseId = knowledgeBaseId,
                            ModelArn = modelArn
                        },
                        Type = RetrieveAndGenerateType.KNOWLEDGE_BASE,

                    }
                };

                var response = await client.RetrieveAndGenerateAsync(request);

                return response.Output.Text;
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


    }

}
