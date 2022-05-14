using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace DiscordBot
{
    public class Program
    {
        const string TokenFileEnvVar = "KingHarley_Bot_File_Path";
        private DiscordSocketClient _client;

        public Program()
        {
            _client = new DiscordSocketClient();
        }

        public static Task Main(string[] args) => new Program().MainAsync();
        public async Task MainAsync()
        {
            var token_path = Environment.GetEnvironmentVariable(TokenFileEnvVar);
            if (token_path == null)
                throw new System.Exception($"The {TokenFileEnvVar} environment variable is not set. Make sure to set this environment variable to the absolute path of .json file containing the bot token");

            _client.Log += LogMessage;
            _client.Ready += ClientReady;
            _client.SlashCommandExecuted += SlashCommandHandler;

            using var r = new StreamReader(token_path);

            var token_dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(r.ReadToEnd());
            if (token_dict == null)
                throw new System.Exception("Could not deserialize KingHarleyTestBot.json");

            if (!token_dict.TryGetValue("token", out var token))
                throw new System.Exception("The token is missing");

            await _client.LoginAsync(TokenType.Bot, token);
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


