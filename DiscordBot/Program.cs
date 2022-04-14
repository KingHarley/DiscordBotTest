using Discord;
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

            using var r = new StreamReader(@"C:\Token\KingHarleyTestBot.json");

            var token = JsonConvert.DeserializeObject<Token>(r.ReadToEnd());
            if (token == null)
                throw new System.Exception("Could not deserialize KingHarleyTestBot.json");

            await _client.LoginAsync(TokenType.Bot, token.token);
            await _client.StartAsync();

            await Task.Delay(-1);

        }

        private Task LogMessage(LogMessage message)
        {
            Console.WriteLine(message.ToString());
            return Task.CompletedTask;
        }

    }
}


