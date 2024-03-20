using Discord.Bots.Core.Models;
using Discord.Interactions;
using Newtonsoft.Json;

namespace Discord.Bots.Core.Modules;

public class FunModule(IHttpClientFactory httpClientFactory) : InteractionModuleBase<InteractionContext>
{
    private readonly IHttpClientFactory _httpService = httpClientFactory;

    #region Hug

    [SlashCommand("hug", "Give someone a hug")]
    public async Task Hug([Summary(description: "The person you wanna hug")] IUser user)
    {
        using var http = _httpService.CreateClient();
        string text = Context.User.Id == user.Id
              ? $"{Context.User.Mention} gave themselves a hug"
              : $"{user.Mention}, {Context.User.Mention} gave you a hug";

        using var stream = await http.GetStreamAsync("https://media.giphy.com/media/RPyUPymjO4YJa/giphy.gif");
        await RespondWithFileAsync(stream, "giphy.gif", text: text);
    }

    #endregion

    #region Cookie

    [SlashCommand("cookie", "Give someone a cookie")]
    public async Task Cookie([Summary(description: "The person you wanna give a cookie to")] IUser user)
    {
        string text = Context.User.Id == user.Id
              ? $"{Context.User.Mention} gave themselves a cookie :cookie:"
              : $"{user.Mention}, {Context.User.Mention} gave you a cookie :cookie:. Enjoy!";

        await RespondAsync(text);
    }

    #endregion

    #region Ask

    [SlashCommand("ask", "Ask me a question")]
    public async Task Ask(string question)
    {
        var answers = new string[]
        {
            "Yes",
            "No",
            "Maybe?",
            "If i tell you i'd need to ban you",
            "The person next to you will answer that",
            "Why would you even ask that?",
            "Uhm...",
            "Of course",
            "*Looks at you sternly*",
            "Oh, look! What a lovely weather it is today",
            "Why are you asking me? Look it up in the library",
            "If all planets align, then...maybe",
            "Ask again later, i don't have time now",
            "To quote Shakespeares Hamlet, Act 2, Scene 4, Verse 48: no",
            "I dont quite get your question, it doesnt make sense to me"
        };

        await RespondAsync($"{answers[new Random().Next(answers.Length)]}");
    }

    #endregion


    #region Dice

    [SlashCommand("dice", "Roll a dice with <number> sides")]
    public async Task Dice(int number)
    {
        await RespondAsync($"You rolled **{new Random().Next(number + 1)}**");
    }

    #endregion

    #region Quote

    [SlashCommand("quote", "Get a quote from InspiroBot")]
    public async Task Quote()
    {
        using var http = _httpService.CreateClient();
        var quoteRequest = await http.GetAsync("https://inspirobot.me/api?generate=true");
        string imageURL = await quoteRequest.Content.ReadAsStringAsync();

        using var stream = await http.GetStreamAsync(imageURL);
        await RespondWithFileAsync(stream, "quote.jpg");
    }

    #endregion

    #region Quiz

    [SlashCommand("quiz", "Answer a trivia question")]
    public async Task Quiz()
    {
        using var http = _httpService.CreateClient();
        var quizRequest = await http.GetAsync("https://opentdb.com/api.php?amount=1&type=multiple");
        var quizResponse = await quizRequest.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<Response>(quizResponse)?.Results?.First();
        if (result == null || result.IncorrectAnswers == null || string.IsNullOrEmpty(result.CorrectAnswer) || string.IsNullOrEmpty(result.Question))
        {
            await Context.Interaction.RespondAsync("Something went wrong while fetching the question");
            return;
        }

       var answers = result.IncorrectAnswers.Select<string, (bool correct, string answer)>(answer => (false, answer)).ToList();
        answers.Insert(new Random().Next(answers.Count), (true, result.CorrectAnswer));

        var quizEmbed = new EmbedBuilder()
            .WithAuthor(Context.Client.CurrentUser)
            .WithTitle(CleanText(result.Question))
            .WithColor(Color.Gold)
            .WithFooter("You have 20 seconds to answer")
            .AddField("Category", result.Category, true)
            .AddField("Difficulty", result.Difficulty, true);

        var component = new ComponentBuilder();
        for (int i = 0; i < answers.Count; i++)
        {
            var button = new ButtonBuilder()
            {
                Label = CleanText(answers[i].answer),
                CustomId = PrepareText($"quiz_{(answers[i].correct ? 1 : 0)}_{CleanText(answers[i].answer)}"),
                Style = (ButtonStyle)(i + 1),
            };
            component.WithButton(button);
        }

        await Context.Interaction.RespondAsync(components: component.Build(), embed: quizEmbed.Build());

        await Task.Delay(20000);
        var response = await Context.Interaction.GetOriginalResponseAsync();
        if (response.Embeds.Any())
        {
            await response.DeleteAsync();
        }
    }

    public static string CleanText(string text)
    {
        return text
            .Replace("&quot;", "\"")
            .Replace("&#039;", "\'")
            .Replace("&amp;", "&")
            .Replace("&ouml;", "ö");
    }

    public static string PrepareText(string text)
    {
        text = text.Replace(" ", "|");

        if (text.Length < 100) return text;

        return $"{text[..96]}...";
    }

    #endregion
}
