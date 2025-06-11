using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace Dress_Up.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatbotController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ChatbotController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("AskAssistant")]
        public async Task<IActionResult> AskAssistant([FromBody] string message)
        {
            var apiKey = _configuration["OpenAI:ApiKey"];
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var requestBody = new
            {
                model = "llama3-70b-8192",
                messages = new object[]
                {
                    new { role = "system", content = "Ești un consultant de modă virtual. Răspunde scurt, prietenos și pe înțelesul tuturor in maxim 30 de cuvinte" },
                    new { role = "user", content = message }
                },
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://api.groq.com/openai/v1/chat/completions", content);
            var responseString = await response.Content.ReadAsStringAsync();


            if (response.IsSuccessStatusCode)
            {
                dynamic jsonResponse = JsonConvert.DeserializeObject(responseString);
                string assistantReply = jsonResponse.choices[0].message.content;

                return Ok(assistantReply);
            }
            else
            {
                return BadRequest($"Eroare la OpenAI API: {response.StatusCode} - {responseString}");
            }
        }
    }
}
