using Disqord;
using Disqord.Webhook;

namespace Sentinel;

public class DiscordMessage
{
    private readonly IWebhookClient _client;
    private readonly LocalWebhookMessage _message;

    public DiscordMessage(IWebhookClientFactory factory)
    {
        _client = factory.CreateClient(Environment.GetEnvironmentVariable("DiscordWebhookURL"));
        _message = new LocalWebhookMessage();
    }

    public DiscordMessage Create(SentryRequest request)
    {
        var embed = new LocalEmbed()
            .WithAuthor("Sentinel", url: request.Data.Event.web_url)
            .WithTitle((request.Data.Event.title == "" ? request.Data.Event.metadata.title : request.Data.Event.title).Truncate(256))
            .WithUrl(request.Data.Event.web_url)
            .WithDescription(request.Data.Event.exception == null 
                ? $"```{request.Data.Event.logentry.formatted.Truncate(4090)}```" 
                : $"```{string.Join(Environment.NewLine, request.Data.Event.exception.values.Select(ex => $"{ex.type}: {ex.value}")).Truncate(4090)}```")
            .WithFooter($"Event ID: {request.Data.Event.event_id}")
            .WithTimestamp(request.Data.Event.datetime)
            .WithColor(request.Data.Event.level.ToColor());

        embed.AddField("Environment", request.Data.Event.environment, true);
        embed.AddField("Release", request.Data.Event.release, true);
        
        if (!string.IsNullOrWhiteSpace(request.Data.Event.transaction)) embed.AddField("Transaction", request.Data.Event.transaction, true);
        
        if (request.Data.Event.user != null) embed.AddField("User", request.Data.Event.user.username);
        if (request.Data.Event.tags != null) embed.AddField("Tags", string.Join(Environment.NewLine, request.Data.Event.tags.Select(tag => $"**{tag[0]}:** {tag[1]}")).Truncate(1204));
        if (request.Data.Event.extras != null) embed.AddField("Extras", string.Join(Environment.NewLine, request.Data.Event.extras.Select(extra => $"**{extra.Key}:** {extra.Value}")).Truncate(1024));
        if (request.Data.Event.errors != null) embed.AddField("Errors", string.Join(Environment.NewLine, request.Data.Event.errors.Select(err => $"{err.name} {err.type} {err.reason}")).Truncate(1024));

        _message.AddEmbed(embed);

        return this;
    }

    public async Task SendAsync() => await _client.ExecuteAsync(_message);
}

public static class StringExtensions
{
    public static string Truncate(this string original, int length)
    {
        if (original == null) return string.Empty;
        if (length == 0) return original;

        return original[..Math.Min(length, original.Length)];
    }

    public static Color ToColor(this string level) => level.ToLower() switch
    {
        "error" => Color.OrangeRed,
        "fatal" => Color.DarkRed,
        "info" => Color.SlateBlue,
        "warning" => Color.Orange,
        _ => Color.SlateGray
    };
}