using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Bot.Events
{
    public class InteractionEvents
    {
        public InteractionEvents()
        {

        }

        public async Task InteractionCreated(SocketInteraction arg)
        {
            if (arg.Type == Discord.InteractionType.MessageComponent)
            {
                var parsedArg = (SocketMessageComponent)arg;

                switch (parsedArg.Data.CustomId)
                {
                    case string s when s.StartsWith("quiz"):
                        await ProcessQuiz(parsedArg);
                        break;
                    default:
                        break;
                }
            }
        }

        public async Task ProcessQuiz(SocketMessageComponent clickedButton)
        {
            bool correct = clickedButton.Data.CustomId.EndsWith("correct");
            var actionRow = clickedButton.Message.Components.First(comp => comp.Components.Any(button => button.CustomId == clickedButton.Data.CustomId));
            string answer = actionRow.Components.FirstOrDefault(button => button.CustomId == clickedButton.Data.CustomId).Label;
            string text = correct
                ? $"`{answer}` is correct! Congratulations {clickedButton.User.Mention}"
                : $"`{answer}` was not the correct answer, {clickedButton.User.Mention}";

            await clickedButton.Message.DeleteAsync();
            await clickedButton.Channel.SendMessageAsync(text);
        }
    }
}
