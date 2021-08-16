using Discord;
using Discord.Commands;
using Discord_Bot.Services;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Discord_Bot.Modules
{
    [Name("Test")]
    public class TestModule : ModuleBase<SocketCommandContext>
    {
        private readonly IHttpClientFactory httpservice;

        public TestModule(IHttpClientFactory httpservice)
        {
            this.httpservice = httpservice;
        }


        [Command("test")]
        [Summary("Test")]
        public async Task Test()
        {
            var builder = new ComponentBuilder().WithButton("Hello!", "id_1", ButtonStyle.Primary);
            var builder2 = new EmbedBuilder()
            {
                Color = new Color(114, 137, 0),
                Description = "Test"
            };
            await Context.Channel.SendMessageAsync("Test buttons!", component: builder.Build(), embed: builder2.Build());
        }

        [Command("quiz")]
        [Summary("quiz")]
        public async Task Quiz()
        {
            using (var http = httpservice.CreateClient())
            {
                var quizRequest = await http.GetAsync("https://opentdb.com/api.php?amount=1&type=multiple");
                var result = JsonConvert.DeserializeObject<Root>(await quizRequest.Content.ReadAsStringAsync()).Results.First();
            }
            var client = new RestClient("https://opentdb.com/api.php?amount=1&type=multiple");
            var response = await client.ExecuteAsync<Root>(new RestRequest(Method.GET));
            var result = response.Data.Results.First();

            List<Tuple<bool, string>> answers = result.IncorrectAnswers.Select(answer => new Tuple<bool, string>(false, answer)).ToList();
            answers.Insert(new Random().Next(answers.Count), new Tuple<bool, string>(true, result.CorrectAnswer));

            var quizEmbed = new EmbedBuilder()
                .WithAuthor(Context.Client.CurrentUser)
                .WithTitle(CleanText(result.Question))
                .WithColor(new Color(78, 91, 245))
                .WithFooter("You have 20 seconds to answer")
                .AddField("Category", result.Category, true)
                .AddField("Difficulty", result.Difficulty, true);

            int i = 0;
            var buttons = answers
                .Select(answer =>
                {
                    i++;
                    return new ButtonBuilder()
                    {
                        Label = CleanText(answer.Item2),
                        CustomId = $"quiz_{i}_{(answer.Item1 ? "correct" : "wrong")}",
                        Style = (ButtonStyle)i
                    }.Build();
                }).ToList();

            var component = new ComponentBuilder()
            {
                ActionRows = new List<ActionRowBuilder> { new ActionRowBuilder().WithComponents(buttons) }
            };

            await Context.Channel.SendMessageAsync(component: component.Build(), embed: quizEmbed.Build());
        }

        public string CleanText(string text)
        {
            return text.Replace("&quot;", "\"").Replace("&#039;", "\'").Replace("&amp;", "&");
        }

        [Command("quote")]
        [Summary("quote")]
        public async Task Quote()
        {
            using (var http = httpservice.CreateClient())
            {
                var imageRequest = await http.GetAsync("https://inspirobot.me/api?generate=true");
                string imageUrl = await imageRequest.Content.ReadAsStringAsync();

                using (var stream = await http.GetStreamAsync(imageUrl))
                {
                    await Context.Channel.SendFileAsync(stream, imageUrl.Substring(imageUrl.LastIndexOf('/')));
                }
            }
        }
    }


    public class Result
    {
        [JsonPropertyName("category")]
        public string Category { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("difficulty")]
        public string Difficulty { get; set; }

        [JsonPropertyName("question")]
        public string Question { get; set; }

        [JsonPropertyName("correct_answer")]
        public string CorrectAnswer { get; set; }

        [JsonPropertyName("incorrect_answers")]
        public List<string> IncorrectAnswers { get; set; }
    }

    public class Root
    {
        [JsonPropertyName("response_code")]
        public int ResponseCode { get; set; }

        [JsonPropertyName("results")]
        public List<Result> Results { get; set; }
    }
}
