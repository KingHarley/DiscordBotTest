using Discord;
using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace DiscordBot
{
    public class Program
    {
        const string TokenFileEnvVar = "KingHarley_Bot_File_Path";
        private readonly IServiceProvider _services;
        private readonly DiscordSocketConfig _socketConfig = new()
        {
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildMembers,
            AlwaysDownloadUsers = true
        };

        public Program()
        {
            _services = new ServiceCollection()
                .AddSingleton(_socketConfig)
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
                .AddSingleton<InteractionHandler>()
                .BuildServiceProvider();
        }

        public static void Main() => new Program().RunAsync().GetAwaiter().GetResult();
        public async Task RunAsync()
        {
            var client = _services.GetRequiredService<DiscordSocketClient>();
            client.Log += LogMessage;

            await _services.GetRequiredService<InteractionHandler>().InitializeAsync();

            var token_path = Environment.GetEnvironmentVariable(TokenFileEnvVar);
            if (token_path == null)
                throw new System.Exception($"The {TokenFileEnvVar} environment variable is not set. Make sure to set this environment variable to the absolute path of .json file containing the bot token");

            using var r = new StreamReader(token_path);

            var token_dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(r.ReadToEnd());
            if (token_dict == null)
                throw new System.Exception("Could not deserialize KingHarleyTestBot.json");

            if (!token_dict.TryGetValue("token", out var token))
                throw new System.Exception("The token is missing");

            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            await Task.Delay(-1);

        }

        private Task LogMessage(LogMessage message)
        {
            Console.WriteLine(message.ToString());
            return Task.CompletedTask;
        }

        public static bool IsDebug()
        {
#if DEBUG
            return true;
#else
                return false;
#endif
        }
    }
}


