using MediCom.Komunikacia.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MediCom.Komunikacia.Services
{
    public class OpenAiChatService
    {
        private const string Endpoint = "https://api.openai.com/v1/chat/completions";
        private readonly HttpClient _httpClient;
        private readonly JavaScriptSerializer _serializer = new JavaScriptSerializer();

        public OpenAiChatService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<string> SendMessageAsync(IReadOnlyCollection<ChatMessage> conversation)
        {
            var apiKey = ConfigurationManager.AppSettings["OpenAiApiKey"];
            var model = ConfigurationManager.AppSettings["OpenAiModel"] ?? "gpt-4o-mini";

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return "Doplň OpenAI API kľúč do App.config pod OpenAiApiKey, aby aplikácia vedela odpovedať.";
            }

            var payload = new
            {
                model,
                messages = BuildMessages(conversation)
            };

            using (var request = new HttpRequestMessage(HttpMethod.Post, Endpoint))
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
                request.Content = new StringContent(_serializer.Serialize(payload), Encoding.UTF8, "application/json");

                using (var response = await _httpClient.SendAsync(request).ConfigureAwait(false))
                {
                    var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    if (!response.IsSuccessStatusCode)
                    {
                        return "Nepodarilo sa kontaktovať OpenAI API: " + content;
                    }

                    var chatResponse = _serializer.Deserialize<ChatCompletionResponse>(content);
                    if (chatResponse?.Choices == null || chatResponse.Choices.Length == 0)
                    {
                        return "Model nevrátil žiadnu odpoveď.";
                    }

                    return chatResponse.Choices[0].Message?.Content?.Trim() ?? "Model vrátil prázdnu odpoveď.";
                }
            }
        }

        private static object[] BuildMessages(IReadOnlyCollection<ChatMessage> conversation)
        {
            var messages = new List<object>
            {
                new
                {
                    role = "system",
                    content = "Si užitočný asistent v desktopovej chat aplikácii podobnej WhatsAppu. Odpovedaj stručne, priateľsky a po slovensky, ak používateľ píše po slovensky."
                }
            };

            foreach (var item in conversation)
            {
                messages.Add(new
                {
                    role = item.IsUser ? "user" : "assistant",
                    content = item.Text
                });
            }

            return messages.ToArray();
        }

        private class ChatCompletionResponse
        {
            public Choice[] Choices { get; set; }
        }

        private class Choice
        {
            public ResponseMessage Message { get; set; }
        }

        private class ResponseMessage
        {
            public string Content { get; set; }
        }
    }
}
