using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Bot.Modules
{
    public class FunModule : InteractionModuleBase<InteractionContext>
    {
        private readonly IHttpClientFactory _httpService;

        public FunModule(IHttpClientFactory httpservice)
        {
            _httpService = httpservice;
        }


        [SlashCommand("echo", "Echo an input")]
        public async Task Echo(string input)
        {
            await RespondAsync(input);
        }

        [SlashCommand("hug", "Give someone a hug")]
        public async Task Hug(IUser user)
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
    }
}
