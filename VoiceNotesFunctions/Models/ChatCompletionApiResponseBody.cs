using Newtonsoft.Json;

namespace OpenAiHelper.Models
{
    internal class ChatCompletionApiResponseBody
    {
        [JsonProperty(PropertyName = "choices")]
        public ChoiceModel[] Choices { get; set; }

        [JsonProperty(PropertyName = "created")]
        public string Created { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "model")]
        public string Model { get; set; }

        [JsonProperty(PropertyName = "object")]
        public string ApiObject { get; set; }

        [JsonProperty(PropertyName = "usage")]
        public ApiUsageModel Usage { get; set; }


        public string GetBestChoice()
        {
            string bestChoice = "";
            double bestChoiceLogProbs = 0.0;
            foreach (ChoiceModel choice in Choices)
            {
                if (choice.LogProbs > bestChoiceLogProbs)
                {
                    bestChoice = choice.Message.Content;
                    bestChoiceLogProbs = choice.LogProbs;
                }
            }
            return bestChoice;
        }
    }
}
