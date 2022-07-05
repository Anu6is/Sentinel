using Disqord.Webhook;
using Microsoft.AspNetCore.Authentication;
using Sentinel;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebhookClientFactory();
builder.Services.AddTransient<DiscordMessage>();

builder.Services.AddAuthentication("SentrySignatureAuthentication")
                .AddScheme<AuthenticationSchemeOptions, SentrySignatureAuthenticationHandler>("SentrySignatureAuthentication", null);

builder.Services.AddAuthorization();

builder.Services.AddControllers().AddJsonOptions(options => 
{
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    options.JsonSerializerOptions.NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString;
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.MapMethods("/", new[] { "HEAD" }, () => Results.Ok());

app.MapGet("/status", () => "Sentinel Online!");

app.MapPost("/", async (SentryRequest request, DiscordMessage message) =>
{
    await message.Create(request).SendAsync();
  
    return Results.Ok();
}).WithName("NewIssue")
  .RequireAuthorization();

app.Run();