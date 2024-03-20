using Newtonsoft.Json;

namespace Discord.Bots.Core.Models;

public sealed record Result
{
    [JsonProperty("category")]
    public string? Category { get; set; }

    [JsonProperty("type")]
    public string? Type { get; set; }

    [JsonProperty("difficulty")]
    public string? Difficulty { get; set; }

    [JsonProperty("question")]
    public string? Question { get; set; }

    [JsonProperty("correct_answer")]
    public string? CorrectAnswer { get; set; }

    [JsonProperty("incorrect_answers")]
    public List<string>? IncorrectAnswers { get; set; }
}

public sealed record Response
{
    [JsonProperty("response_code")]
    public int ResponseCode { get; set; }

    [JsonProperty("results")]
    public List<Result>? Results { get; set; }
}
