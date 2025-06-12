using Amazon.Lambda.AspNetCoreServer;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace JPMC.Hackathon.RapMentor
{
    /// <summary>
    /// This class extends from APIGatewayProxyFunction which contains the method FunctionHandlerAsync which is the 
    /// actual Lambda function entry point. The Lambda handler field should be set to
    /// 
    /// JPMC.Hackathon.RapMentor::JPMC.Hackathon.RapMentor.LambdaEntryPoint::FunctionHandlerAsync
    /// </summary>
    public class LambdaEntryPoint : APIGatewayProxyFunction
    {
        protected override void Init(IWebHostBuilder builder)
        {
            builder.UseContentRoot(Directory.GetCurrentDirectory())
                   .UseStartup<Startup>()
                   .UseLambdaServer();
        }
    }
}