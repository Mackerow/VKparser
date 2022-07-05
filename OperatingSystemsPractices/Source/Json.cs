using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OperatingSystemsPractices.Source
{
    public static class Json
    {
        public static JsonSerializerOptions GetOptions()
        {
            return new JsonSerializerOptions()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };
        }
    }
}
