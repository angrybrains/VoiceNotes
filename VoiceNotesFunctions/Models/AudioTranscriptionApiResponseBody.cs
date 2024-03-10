using Newtonsoft.Json;

namespace OpenAiHelper.Models
{
    internal class AudioTranscriptionApiResponseBody
    {
        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }
    }
}
