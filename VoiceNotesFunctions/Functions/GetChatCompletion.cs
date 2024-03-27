using System;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenAiHelper.Models;

namespace OpenAiHelper
{
    /// <summary>
    /// Implements the OpenAI Chat API for Completion
    /// https://platform.openai.com/docs/guides/text-generation
    /// </summary>
    public class GetChatCompletion
    {
        private readonly ILogger<GetChatCompletion> _logger;

        public GetChatCompletion(ILogger<GetChatCompletion> log)
        {
            _logger = log;
        }

        [FunctionName("GetChatCompletion")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "chat-completion" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("Entering GetChatCompletion v.23.1225");
            string requestBody;
            try
            {
                requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<GetChatCompletionRequestBody>(requestBody);
                if (data != null)
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("https://api.openai.com/v1/");
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", data.ApiKey);
                        //client.DefaultRequestHeaders.Add("Content-Type", "application/json");

                        ChatCompletionApiRequestModel chatCompletionRequest = new ChatCompletionApiRequestModel();
                        chatCompletionRequest.Model = "gpt-3.5-turbo";
                        chatCompletionRequest.AddMessage(new Gpt35Model { Role = "system", Content = "You are a helpful assistant" });
                        chatCompletionRequest.AddMessage(new Gpt35Model { Role = "system", Content = "Summarize the text in bullets. Be as descriptive as possible without exceeding 1000 words" });
                        chatCompletionRequest.AddMessage(new Gpt35Model { Role = "user", Content = "Summarize the following text:" });
                        chatCompletionRequest.AddMessage(new Gpt35Model { Role = "user", Content = data.Text });

                        string json = JsonConvert.SerializeObject(chatCompletionRequest);
                        using var content = new StringContent(json);
                        //using var content = new StringContent(JsonConvert.SerializeObject(chatCompletionRequest));
                        var response = await client.PostAsync("chat/completions", content);

                        var result = JObject.Parse(await response.Content.ReadAsStringAsync());
                        string resultJson = result.ToString();
                        Console.WriteLine(resultJson);
                        _logger.LogInformation(resultJson);

                        var chatCompletionResponse = JsonConvert.DeserializeObject<ChatCompletionApiResponseBody>(resultJson);
                        OkObjectResult returnResponse = new OkObjectResult(chatCompletionResponse.GetBestChoice());
                        //returnResponse.ContentTypes.Add("application/json");
                        return returnResponse;
                    }
                }
                else
                {
                    string err = "request does not conform to schema";
                    _logger.LogWarning(err);
                    return new BadRequestObjectResult(err);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _logger.LogError(ex.Message);
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}

