using System.Text.Json.Serialization;

namespace MauiSample.Models
{
    public interface IGenericBase
    {
        [JsonPropertyName("ID")]
        public Guid? Id { get; set; }

        [JsonPropertyName("Expression-ObjectID")]
        public Guid? ExpressionObjectId { get; set; }

        [JsonIgnore]
        public string Columns { get; set; }

        [JsonIgnore]
        public string ClassBaseName { get; set; }

        [JsonIgnore]
        public string ClassTypeName { get; set; }
    }
}
