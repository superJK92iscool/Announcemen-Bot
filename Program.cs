
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Microsoft.Extensions.Configuration;
namespace Announcement_Bot_Core
{
    class Program
    {

        public static string ver = "4.0.3";
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public static DiscordSocketClient _client;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private CommandService? _commands;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public static IServiceProvider _services;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public static string prefix = "a!";
        // Main Task for memory fix
        
        // Runbot task
        public async Task RunBot()
        {
            var config = new DiscordSocketConfig()
            {
                // Other config options can be presented here.
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
            };
            _client = new DiscordSocketClient(config); // Define _client
            _commands = new CommandService(); // Define _commands

            _services = new ServiceCollection() // Define _services
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();

            // Load token from config or env var
            string botToken = LoadToken();
            if (string.IsNullOrWhiteSpace(botToken))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Bot token not found. Set it via appsettings.json or DISCORD_TOKEN env variable.");
                return;
            }

            _client.Log += Log; // Set up logging
            System.Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("   ___                                                            __         ___        __");
            Console.WriteLine("  / _ |  ___   ___  ___  __ __  ___  ____ ___   __ _  ___   ___  / /_       / _ ) ___  / /_");
            Console.WriteLine(" / __ | / _ \\ / _ \\/ _ \\/ // / / _ \\/ __// -_) /  ' \\/ -_) / _ \\/ __/      / _  |/ _ \\/ __/");
            Console.WriteLine("/_/ |_|/_//_//_//_/\\___/\\_,_/ /_//_/\\__/ \\__/ /_/_/_/\\__/ /_//_/\\__/      /____/ \\___/\\__/");
            System.Console.ForegroundColor = ConsoleColor.Green;
            System.Console.WriteLine(" ");

            System.Console.WriteLine("Starting");
            System.Console.Write("Version: ");
            System.Console.Write(ver);
            System.Console.WriteLine(" ");
            System.Console.Write("Prefix: ");
            System.Console.Write(prefix);
            System.Console.WriteLine(" ");
            System.Console.Write("Framework: .Net Core " + Environment.Version.ToString());
            System.Console.WriteLine(" ");

            await RegisterCommandsAsync(); // Call registercommands

            await _client.LoginAsync(TokenType.Bot, botToken); // Log into the bot user
            await _client.StartAsync(); // Start the bot user
            await Task.Delay(-1); // Delay for -1 to keep the console window opened

        }
        static void Main(string[] args)
        {

            // Must be inside a method
            AppContext.SetSwitch("System.Runtime.EnableLargeArraySupport", true);

            // Example large array allocation that would otherwise fail
            try
            {
                //byte[] bigArray = new byte[2_500_000_000]; // ~2.5 GB
                System.Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Allocation succeeded.");
            }
            catch (OutOfMemoryException ex)

            {
                System.Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Allocation failed: " + ex.Message);
            }
            new Program().RunBot().GetAwaiter().GetResult();
        }
        private async Task RegisterCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync; // Messagerecieved
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), null); // Set up the command handler
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }

        private Task Log(LogMessage arg) // Logging
        {
            System.Console.WriteLine(arg.Message); // Write the log to console

            return Task.CompletedTask; // Return with completedtask
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage; // Create a variable with the message as SocketUserMessage
            if (message is null || message.Author.IsBot) return; // Checks if the message is empty or sent by a bot
            int argumentPos = 0; // Sets the argpos to 0 (the start of the message)
            if (message.HasStringPrefix(prefix, ref argumentPos) || message.HasMentionPrefix(_client.CurrentUser, ref argumentPos)) // If the message has the prefix at the start or starts with someone mentioning the bot
            {
                var context = new SocketCommandContext(_client, message); // Create a variable called context
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                var result = await _commands.ExecuteAsync(context, argumentPos, _services); // Create a veriable called result
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            }
        }
        private string LoadToken()
        {
            // 1. Try JSON config
            try
            {
                var configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.Development.json", optional: true)
                    .AddJsonFile("appsettings.json", optional: true)
                    .AddEnvironmentVariables()
                    .Build();

                string tokenFromConfig = configuration["Discord:Token"];
                if (!string.IsNullOrWhiteSpace(tokenFromConfig))
                    return tokenFromConfig;
            }
            catch { }

            // 2. Try environment variable
            return Environment.GetEnvironmentVariable("DISCORD_TOKEN");
        }
    }
}

