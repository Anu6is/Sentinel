using System.Text.Json.Serialization;

namespace Sentinel;
public class SentryRequest
{
    public string Action { get; set; }
    public Installation Installation { get; set; }
    public Data Data { get; set; }
    public Actor Actor { get; set; }
}

public class Installation
{
    public string Uuid { get; set; }
}

public class Data
{
    public Event Event { get; set; }
    public string Triggered_Rule { get; set; }
}

public class Event
{
    public int project { get; set; }
    public string event_id { get; set; }
    public string release { get; set; }
    public string dist { get; set; }
    public string platform { get; set; }
    public string message { get; set; }
    public DateTime datetime { get; set; }
    public string[][] tags { get; set; }
    public Contexts contexts { get; set; }
    public string environment { get; set; }
    public Error[] errors { get; set; }
    public Exception exception { get; set; }
    [JsonPropertyName("extra")]
    public Dictionary<string, object> extras { get; set; }
    public string key_id { get; set; }
    public string level { get; set; }
    public Logentry logentry { get; set; }
    public string logger { get; set; }
    public Metadata metadata { get; set; }
    public float timestamp { get; set; }
    public string title { get; set; }
    public string type { get; set; }
    public string version { get; set; }
    public string transaction { get; set; }
    public User user { get; set; }
    public string url { get; set; }
    public string web_url { get; set; }
    public string issue_url { get; set; }
    public string issue_id { get; set; }
}

public class Contexts
{
    public App app { get; set; }
    public Device device { get; set; }
    public Os os { get; set; }
    public Runtime runtime { get; set; }
}

public class App
{
    public DateTime app_start_time { get; set; }
    public string type { get; set; }
}

public class Device
{
    public DateTime boot_time { get; set; }
    public string timezone { get; set; }
    public string timezone_display_name { get; set; }
    public string type { get; set; }
}

public class Os
{
    public string name { get; set; }
    public string version { get; set; }
    public string build { get; set; }
    public string raw_description { get; set; }
    public string type { get; set; }
}

public class Runtime
{
    public string name { get; set; }
    public string version { get; set; }
    public string raw_description { get; set; }
    public string identifier { get; set; }
    public string type { get; set; }
}

public class Logentry
{
    public string message { get; set; }
    public string formatted { get; set; }
}

public class Metadata
{
    public string title { get; set; }
}

public class Error
{
    public string type { get; set; }
    public string name { get; set; }
    public string reason { get; set; }
}

public class Actor
{
    public string type { get; set; }
    public string id { get; set; }
    public string name { get; set; }
}

public class Exception
{
    [JsonPropertyName("values")]
    public ExceptionValue[] values { get; set; }
}

public class ExceptionValue
{
    public string type { get; set; }
    public string value { get; set; }
}

public class User
{
    public string id { get; set; }
    public string username { get; set; }
    public string email { get; set; }
    public string ip_address { get; set; }
    public string subscription { get; set; }
}