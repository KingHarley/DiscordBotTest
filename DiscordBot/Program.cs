using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace DiscordBot
{
    public class Program
    {
        private DiscordSocketClient _client;

        public Program()
        {
            _client = new DiscordSocketClient();
        }

        public static Task Main(string[] args) => new Program().MainAsync();
        public async Task MainAsync()
        {
            _client.Log += LogMessage;
            _client.Ready += ClientReady;
            _client.SlashCommandExecuted += SlashCommandHandler;

            using var r = new StreamReader(@"C:\Token\KingHarleyTestBot.json");

            var token = JsonConvert.DeserializeObject<Token>(r.ReadToEnd());
            if (token == null)
                throw new System.Exception("Could not deserialize KingHarleyTestBot.json");

            await _client.LoginAsync(TokenType.Bot, token.token);
            await _client.StartAsync();

            

            await Task.Delay(-1);

        }

        private async Task ClientReady()
        {
            var slashCommand = new SlashCommandBuilder().WithName("First-Command").WithDescription("This is the first command");

            try
            {
                await _client.CreateGlobalApplicationCommandAsync(slashCommand.Build());
            }
            catch (HttpException ex)
            {
                Console.WriteLine(ex.Reason);
            }
        }
        private async Task SlashCommandHandler(SocketSlashCommand command)
        {
            await command.RespondAsync(command.CommandName);
        }

        private Task LogMessage(LogMessage message)
        {
            Console.WriteLine(message.ToString());
            return Task.CompletedTask;
        }

    }
}


