using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenAiHelper.Models
{
    internal class ChatCompletionApiRequestModel
    {
        private List<Gpt35Model> messages { get; set; }

        [JsonProperty(PropertyName = "model")]
        public string Model { get; set; }

        [JsonProperty(PropertyName = "messages")]
        public Gpt35Model[] Messages
        {
            get
            { return messages.ToArray(); }
        }

        public ChatCompletionApiRequestModel()
        {
            Model = "gpt-3.5-turbo";
            messages = new List<Gpt35Model>();
        }

        public void AddMessage(Gpt35Model message)
        {
            messages.Add(message);
        }
    }
}
