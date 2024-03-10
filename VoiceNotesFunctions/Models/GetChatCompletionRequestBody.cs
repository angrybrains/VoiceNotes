using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenAiHelper.Models
{
    internal class GetChatCompletionRequestBody
    {
        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "apiKey")]
        public string ApiKey { get; set; }
    }
}
