using Discord;
using Discord.Interactions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace The_Wanderers_Helper.Modules
{
    public class FunModule : InteractionModuleBase<InteractionContext>
    {
        private readonly IHttpClientFactory _httpService;

        public FunModule(IHttpClientFactory httpservice)
        {
            _httpService = httpservice;
        }

        #region Quiz

        [SlashCommand("quiz", "Answer a trivia question")]
        public async Task Quiz()
        {
            using (var http = _httpService.CreateClient())
            {
                var quizRequest = await http.GetAsync("https://opentdb.com/api.php?amount=1&type=multiple");
                string quizResponse = await quizRequest.Content.ReadAsStringAsync();
                Result result = JsonConvert.DeserializeObject<Response>(quizResponse).Results.First();

                List<Tuple<bool, string>> answers = result.IncorrectAnswers.Select(answer => new Tuple<bool, string>(false, answer)).ToList();
                answers.Insert(new Random().Next(answers.Count), new Tuple<bool, string>(true, result.CorrectAnswer));

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
                        Label = CleanText(answers[i].Item2),
                        CustomId = PrepareText($"quiz_{(answers[i].Item1 ? 1 : 0)}_{CleanText(answers[i].Item2)}"),
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
        }

        public string PrepareText(string text)
        {
            text = text.Replace(" ", "|");

            if (text.Length < 100) return text;

            return $"{text.Substring(0, 96)}...";
        }

        public string CleanText(string text)
        {
            return text
                .Replace("&quot;", "\"")
                .Replace("&#039;", "\'")
                .Replace("&amp;", "&")
                .Replace("&ouml;", "ö");
        }

        public class Result
        {
            [JsonProperty("category")]
            public string Category { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("difficulty")]
            public string Difficulty { get; set; }

            [JsonProperty("question")]
            public string Question { get; set; }

            [JsonProperty("correct_answer")]
            public string CorrectAnswer { get; set; }

            [JsonProperty("incorrect_answers")]
            public List<string> IncorrectAnswers { get; set; }
        }

        public class Response
        {
            [JsonProperty("response_code")]
            public int ResponseCode { get; set; }

            [JsonProperty("results")]
            public List<Result> Results { get; set; }
        }

        #endregion

        #region Ask

        [SlashCommand("ask", "Ask me a question")]
        public async Task Ask(string question)
        {
            string[] answers = new string[]
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

        #region Hug

        [SlashCommand("hug", "Give someone a hug")]
        public async Task Hug([Summary(description: "The person you wanna hug")] IUser user)
        {
            using (var http = _httpService.CreateClient())
            {

                string text = Context.User.Id == user.Id
                      ? $"{Context.User.Mention} gave themselves a hug"
                      : $"{user.Mention}, {Context.User.Mention} gave you a hug";

                using (var stream = await http.GetStreamAsync("https://media.giphy.com/media/RPyUPymjO4YJa/giphy.gif"))
                {
                    await RespondWithFileAsync(stream, "giphy.gif", text: text);
                }
            }
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
            using (var http = _httpService.CreateClient())
            {
                var quoteRequest = await http.GetAsync("https://inspirobot.me/api?generate=true");
                string imageURL = await quoteRequest.Content.ReadAsStringAsync();

                using (var stream = await http.GetStreamAsync(imageURL))
                {
                    await RespondWithFileAsync(stream, "quote.jpg");
                }
            }
        }

        #endregion
    }
}
