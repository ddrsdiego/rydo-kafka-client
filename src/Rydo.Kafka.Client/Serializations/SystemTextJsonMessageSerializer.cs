namespace Rydo.Kafka.Client.Serializations
{
    using System.Text.Encodings.Web;
    using System.Text.Json;

    public static class SystemTextJsonMessageSerializer
    {
        public static readonly JsonSerializerOptions?  Options;
        
        static SystemTextJsonMessageSerializer()
        {
            Options = new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                ReadCommentHandling = JsonCommentHandling.Skip,
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            };
        }
    }
}