using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Announcement_Bot_Core
{
    public class MCStatus : ModuleBase<SocketCommandContext>
    {
        private readonly int[] _ports = { 25550, 25560, 25570, 25580, 25590 };
        private const int MaxRetries = 2;

        [Command("mcstatus")]
        [Summary("Checks the status of predefined Minecraft ports.")]
        public async Task CheckPortsAsync()
        {
            Console.WriteLine($"[mcstatus] Command invoked by {Context.User.Username} in #{Context.Channel.Name} on #{Context.Guild.Name}");

            foreach (int port in _ports)
            {
                bool isOnline = false;

                for (int i = 0; i < MaxRetries; i++)
                {
                    isOnline = await IsPortOpenAsync("localhost", port);
                    Console.WriteLine($"[mcstatus] Port {port}, Attempt {i + 1}/{MaxRetries}: {(isOnline ? "Online" : "Offline")}");
                    if (isOnline) break;
                    await Task.Delay(1000);
                }

                var embed = new EmbedBuilder()
                    .WithTitle($"🧪 Checking Minecraft Server Port {port}")
                    .AddField("Status", isOnline ? "✅ Online" : "❌ Offline", true)
                    .AddField("Port", port, true)
                    .AddField("Retries", MaxRetries, true)
                    .WithColor(isOnline ? Color.Green : Color.Red)
                    .WithFooter($"Requested by {Context.User.Username} • {DateTime.Now:M/d/yy, h:mm tt}")
                    .Build();
                await ReplyAsync(embed: embed);
                await ReplyAsync($"<@{Context.User.Id}> The server on port `{port}` is {(isOnline ? "online" : "offline")}!");

                Console.WriteLine($"[mcstatus] Sent status for port {port}");
            }

            Console.WriteLine("[mcstatus] Command finished.");
        }

        private static async Task<bool> IsPortOpenAsync(string host, int port)
        {
            try
            {
                using var client = new TcpClient();
                var connectTask = client.ConnectAsync(host, port);
                var timeoutTask = Task.Delay(1000);
                var completedTask = await Task.WhenAny(connectTask, timeoutTask);
                return completedTask == connectTask && client.Connected;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[mcstatus] Error checking port {port}: {ex.Message}");
                return false;
            }
        }
    }
}
