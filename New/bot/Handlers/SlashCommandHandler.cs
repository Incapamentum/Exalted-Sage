using Discord.WebSocket;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bot.Handlers
{
    /// <summary>
    ///     Handler class that executes the specific slash command that was invoked
    /// </summary>
    public class SlashCommandHandler
    {
        public async Task SlashCommandExecute(SocketSlashCommand command)
        {
            switch (command.Data.Name)
            {
                case "register-message":
                    await RegisterMessageAsync(command);
                    break;
            }            
        }

        // Have this add the message ID to the watch list within the DB
        private async Task RegisterMessageAsync(SocketSlashCommand command)
        {
            await command.RespondAsync($"Message options: {command.Data.Options.First().Value}");
        }
    }
}
