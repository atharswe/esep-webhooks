using System.Text;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace EsepWebhook;

public class Function
{
    static readonly HttpClient http = new HttpClient();

    public string FunctionHandler(object input, ILambdaContext context)
    {
        context.Logger.LogLine($"Received: {input}");

        var link = GetIssueLink(input);
        if (link == null)
        {
            context.Logger.LogLine("no issue in payload, skipping");
            return "ignored";
        }

        var reply = PostToSlack($"Issue Created: {link}");
        context.Logger.LogLine($"Slack said: {reply}");

        return reply;
    }


    static string? GetIssueLink(object input)
    {
        var body = JObject.Parse(input.ToString() ?? "{}");
        var url = (string?)body["issue"]?["html_url"];

        return string.IsNullOrEmpty(url) ? null : url;
    }

    static string PostToSlack(string message)
    {
        var slackUrl = Environment.GetEnvironmentVariable("SLACK_URL");
        if (string.IsNullOrEmpty(slackUrl))
            throw new InvalidOperationException("SLACK_URL is not set");

        var json = JsonConvert.SerializeObject(new { text = message });

        using var req = new HttpRequestMessage(HttpMethod.Post, slackUrl);
        req.Content = new StringContent(json, Encoding.UTF8, "application/json");

        using var res = http.Send(req);
        using var reader = new StreamReader(res.Content.ReadAsStream());

        return reader.ReadToEnd();
    }
}
