using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace JinaReaderGPTs20240430.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ContentController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public ContentController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet("process-url")]
        public async Task<IActionResult> GetProcessedContent([FromQuery] string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return BadRequest("URL is required.");
            }

            // Check if URL exists
            var checkResponse = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
            if (!checkResponse.IsSuccessStatusCode)
            {
                return NotFound("URL does not exist or cannot be reached.");
            }

            // Process URL through Reader API
            var readerUrl = $"https://r.jina.ai/{url}";
            var request = new HttpRequestMessage(HttpMethod.Get, readerUrl);
            request.Headers.Add("x-no-cache", "true"); // キャッシュを回避するヘッダーを追加

            //var response = await _httpClient.GetAsync(readerUrl);
            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode(500, "Failed to retrieve content from the Reader API.");
            }

            var content = await response.Content.ReadAsStringAsync();
            return Ok(new { Content = content });
        }
    }
}