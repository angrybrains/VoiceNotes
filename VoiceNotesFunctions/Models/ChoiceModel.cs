using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenAiHelper.Models
{
    internal class ChoiceModel
    {
        [JsonProperty(PropertyName = "finish_reason")]
        public string FinishReason { get; set; }

        [JsonProperty(PropertyName = "index")]
        public string Index { get; set; }

        [JsonProperty(PropertyName = "message")]
        public Gpt35Model Message { get; set; }

        [JsonProperty(PropertyName = "logprobs")]
        public double LogProbs { get; set; }

    }
}
