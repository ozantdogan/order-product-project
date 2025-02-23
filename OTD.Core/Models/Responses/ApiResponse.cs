using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace OTD.Core.Models.Responses
{
    public class ApiResponse
    {
        public bool Success { get; set; }
        public string? ResultMessage { get; set; } = "";
    }

    public class ApiResponse<T> : ApiResponse
    {
        [JsonConstructor]
        public ApiResponse()
        {
        }

        public ApiResponse(T t)
        {
            Data = t;
        }

        public T? Data { get; set; }
    }
}