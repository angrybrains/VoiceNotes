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
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace OpenAiHelper
{
    /// <summary>
    /// Implements the OpenAI Audio API for Transcription
    /// https://platform.openai.com/docs/guides/speech-to-text
    /// </summary>
    public class GetAudioTranscription
    {
        private readonly ILogger<GetAudioTranscription> _logger;

        public GetAudioTranscription(ILogger<GetAudioTranscription> log)
        {
            _logger = log;
        }

        [FunctionName("GetAudioTranscription")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "audio-transcription" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        //[OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("Entering GetTranscription v.23.1225");

            string requestBody;
            try
            {
                requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<GetAudioTranscriptionRequestBody>(requestBody);
                if (data != null)
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("https://api.openai.com/v1/");
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", data.ApiKey);

                        using var content = new MultipartFormDataContent();
                        HttpContent audioContent = new ByteArrayContent(Convert.FromBase64String(data.Audio));  //Audio is base64 encoded in the Logic App
                        //HttpContent audioContent = new ByteArrayContent(System.Text.Encoding.UTF8.GetBytes(data.Audio));
                        //HttpContent audioContent = new ByteArrayContent(File.ReadAllBytes(audioFile));
                        audioContent.Headers.ContentType = MediaTypeHeaderValue.Parse("audio/mp3"); //optional
                        content.Add(audioContent, "file", data.Name);          // supported formats: flac, mp3, mp4, mpeg, mpga, m4a, ogg, wav, or webm
                        content.Add(new StringContent("whisper-1"), "model");
                        content.Add(new StringContent("en"), "language");       //optional - default=en   ISO 639-1
                        content.Add(new StringContent("make it funny"), "prompt"); //optional - text to guide the model or continue a previous audio segment.
                        content.Add(new StringContent("json"), "response_format"); //optional - json/text/srt/verbose_json,vtt, default=json
                        content.Add(new StringContent("1"), "temperature");     //optional - 0~1, default=0. lower number is more deterministic, higher value is more random. 
                        var response = await client.PostAsync("audio/translations", content);

                        var result = JObject.Parse(await response.Content.ReadAsStringAsync());
                        string resultJson = result.ToString();
                        Console.WriteLine(resultJson);
                        _logger.LogInformation(resultJson);

                        var transcriptionResponse = JsonConvert.DeserializeObject<AudioTranscriptionApiResponseBody>(resultJson);
                        OkObjectResult returnResponse = new OkObjectResult(transcriptionResponse.Text);
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

