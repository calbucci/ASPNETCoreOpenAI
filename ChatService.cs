using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

public interface IChatService
{
    IAsyncEnumerable<string> GetChatResponseStream(string message);
}

public class ChatService : IChatService
{
    private readonly HttpClient _client;
    private string? _previousResponseId = null;

    public ChatService(HttpClient client)
    {
        _client = client;
    }

    public async IAsyncEnumerable<string> GetChatResponseStream(string message)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/responses");

        // Get API Key from https://platform.openai.com/settings/organization/api-keys
        // Don't forget to add a credit balance (under Billing)
        var apiKey = Environment.GetEnvironmentVariable("OpenAIKey");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        var payload = new
        {
            model = "gpt-4o-2024-11-20",
            input = message,
            //stream = true,
            previous_response_id = _previousResponseId
        };
        request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        using var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

        using var stream = await response.Content.ReadAsStreamAsync();
        using var reader = new StreamReader(stream);
        var content = await reader.ReadToEndAsync();

        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            yield return "error: " + response.StatusCode;
        }


        dynamic json = JsonSerializer.Deserialize<dynamic>(content)!;

        _previousResponseId = json.id;
        string? textOutput = json.output?.content?.text;

        yield return textOutput == null ? "" : textOutput!;

        // while (!reader.EndOfStream)
        // {
        //     var line = await reader.ReadLineAsync();
        //     if (line?.StartsWith("data: ") != true)
        //         break;

        //     var data = line.Substring("data: ".Length).Trim();
        //     if (data == "[DONE]")
        //     {
        //         yield break;
        //     }
        //     yield return data;

        // }
    }

}
