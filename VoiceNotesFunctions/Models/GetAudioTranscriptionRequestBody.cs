using Newtonsoft.Json;

namespace OpenAiHelper
{
    internal class GetAudioTranscriptionRequestBody
    {
        [JsonProperty(PropertyName ="name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "apiKey")]
        public string ApiKey { get; set; }

        [JsonProperty(PropertyName = "audio")]
        public string Audio { get; set; }
    }
}
