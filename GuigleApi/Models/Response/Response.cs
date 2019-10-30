using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GuigleApi.Models.Response
{
    [JsonObject]
    public class Response<T> : IResponse<T>
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("error_message")]
        public string ErrorMessage { get; set; }

        [JsonProperty("candidates")]
        public List<T> Candidates { get; set; }

        [JsonProperty("results")]
        public List<T> Results { get; set; }

        [JsonProperty("result")]
        public T Result { get; set; }

        [JsonProperty("html_attributions")]
        public List<string> HtmlAttributions { get; set; }

        [JsonProperty("next_page_token")]
        public string NextPageToken { get; set; }

        public static async Task<HttpRequestException> ResponseError(HttpResponseMessage response)
        {
            return new HttpRequestException($"Error parsing Response<{nameof(T)}>. Request status code {response.StatusCode}. More details: {await response.Content?.ReadAsStringAsync()}");
        }
    }
}
