using System.Text.Json.Serialization;

namespace IOrder.Communication.Response;

public class ResponseErrorDto
{
    public IList<string> Errors { get; private set; }

    [JsonConstructor]
    public ResponseErrorDto(IList<string> errors) => Errors = errors;

    public bool TokenIsExpired { get; set; }

    public ResponseErrorDto(string error) => Errors = [error];
}
