using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace The_Wanderers_Helper.Modules
{
    [Name("Fun")]
    public class FunModule : ModuleBase<SocketCommandContext>
    {
        private readonly IHttpClientFactory _httpService;

        public FunModule(IHttpClientFactory httpservice)
        {
            _httpService = httpservice;
        }

        #region Quiz

        [Command("quiz")]
        [Alias("q", "trivia")]
        [Summary("Answer a trivia question")]
        public async Task Quiz()
        {
            using (var http = _httpService.CreateClient())
            {
                var quizRequest = await http.GetAsync("https://opentdb.com/api.php?amount=1&type=multiple");
                string response = await quizRequest.Content.ReadAsStringAsync();
                Result result = JsonConvert.DeserializeObject<Response>(response).Results.First();

                List<Tuple<bool, string>> answers = result.IncorrectAnswers.Select(answer => new Tuple<bool, string>(false, answer)).ToList();
                answers.Insert(new Random().Next(answers.Count), new Tuple<bool, string>(true, result.CorrectAnswer));

                var quizEmbed = new EmbedBuilder()
                    .WithAuthor(Context.Client.CurrentUser)
                    .WithTitle(CleanText(result.Question))
                    .WithColor(new Color(78, 91, 245))
                    .WithFooter("You have 20 seconds to answer")
                    .AddField("Category", result.Category, true)
                    .AddField("Difficulty", result.Difficulty, true);

                var component = new ComponentBuilder();
                for (int i = 0; i < answers.Count; i++)
                {
                    var button = new ButtonBuilder()
                    {
                        Label = CleanText(answers[i].Item2),
                        CustomId = $"quiz_{i}_{(answers[i].Item1 ? "correct" : "wrong")}",
                        Style = (ButtonStyle)(i + 1),
                    };
                    component.WithButton(button);
                }

                var msg = await Context.Channel.SendMessageAsync(component: component.Build(), embed: quizEmbed.Build());

                await Task.Delay(20000);
                var existingMsg = await msg.Channel.GetMessageAsync(msg?.Id ?? 0);
                if (existingMsg != null)
                {
                    await msg.DeleteAsync();
                    await Context.Channel.SendMessageAsync("Nobody managed to answer in time");
                }
            }
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

        [Command("ask")]
        [Alias("8ball")]
        [Summary("Ask me a question")]
        public async Task Ask([Remainder][Summary("Your question")] string question)
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

            await Context.Message.ReplyAsync($"{answers[new Random().Next(answers.Length)]}");
        }

        #endregion

        #region Cookie

        [RequireUserPermission(GuildPermission.Administrator, Group = "Permission")]
        [RequireOwner(Group = "Permission")]
        [Command("cookie")]
        [Alias("biscuit")]
        [Summary("Give someone a cookie")]
        public async Task Cookie([Summary("user")] IGuildUser user)
        {
            //var user = await CustomUserTypereader.GetUserFromString(mention, Context.Guild);

            if (user == Context.User)
                await Context.Channel.SendMessageAsync($"Are you really giving yourself a cookie, {Context.User.Mention}?");
            else
                await Context.Channel.SendMessageAsync($"{user.Mention}, {Context.User.Mention} gave you a cookie :cookie:. Enjoy!");
        }

        #endregion

        #region Hug

        [RequireUserPermission(GuildPermission.Administrator, Group = "Permission")]
        [RequireOwner(Group = "Permission")]
        [Command("hug")]
        [Summary("Give someone a hug")]
        public async Task Hug([Summary("user")] string mention)
        {
            using (var http = _httpService.CreateClient())
            {
                var user = await CustomUserTypereader.GetUserFromString(mention, Context.Guild);

                if (user != null)
                {
                    string text = Context.User == user
                      ? $"{Context.User.Mention} gave themselves a hug"
                      : $"{user.Mention}, {Context.User.Mention} gave you a hug";

                    using (var stream = await http.GetStreamAsync("https://media.giphy.com/media/RPyUPymjO4YJa/giphy.gif"))
                    {
                        await Context.Channel.SendFileAsync(stream, "giphy.gif", text: text);
                    }
                }
            }
        }

        #endregion

        #region Dice

        [Command("dice")]
        [Summary("Roll a dice with <number> sides")]
        public async Task Dice([Summary("number")] int number)
        {
            await Context.Message.ReplyAsync($"You rolled **{new Random().Next(number + 1)}**");
        }

        #endregion

        public class CustomUserTypereader
        {
            public static async Task<IUser> GetUserFromString(string s, IGuild server)
            {
                //if (s.IndexOf('@') == -1 || s.Replace("<", "").Replace(">", "").Length != s.Length - 2)
                //    throw new System.Exception("Not a valid user mention.");

                string idStr = s.Replace("<", "").Replace(">", "").Replace("@!", "");

                try
                {
                    ulong id = ulong.Parse(idStr);
                    return await server.GetUserAsync(id);
                }
                catch
                {
                    return null;
                    //throw new System.Exception("Could not parse User ID. Are you sure the user is still on the server?");
                }
            }
        }
    }
}
