using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Bot.Modules
{
    public  class ButtonModule :  InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
    {
        [ComponentInteraction("sortrole-*")]
        public async Task Sort(string id)
        {
            await Context.Interaction.UpdateAsync(x =>
            {
                x.Content = $"you selected {id}";
                x.Components = null;
                x.Embeds = null;
            });
        }
    }
}
