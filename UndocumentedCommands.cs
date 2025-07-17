using System.Diagnostics;
using System.Runtime.InteropServices; // ADDED
using System.Reflection; // ADDED
using Announcement_Bot_Core;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.IO; // ADDED
using System.Threading.Tasks;

namespace Announcement_Bot_Core
{
    public class UndocumentedCommands : ModuleBase<SocketCommandContext>
    {
        Process currentProcess = Process.GetCurrentProcess();

        [Command("botreport")]
        private async Task BotReportCmd()
        {
            Console.WriteLine("Bot Report Command Ran");
            if (Context.User.Id != Context.Guild.OwnerId && Context.User.Id != 296820164823875585)
            {
                await ReplyAsync("Only the server owner and bot owner/current dev can use this command.");
                return;
            }

            try
            {
                int guildCount = 0;
                int roleCount = 0;
                int textChannels = 0;
                int voiceChannels = 0;
                int totalChannels = 0;
                int userCount = 0;
                int dndCount = 0;
                int invCount = 0;
                int idlCount = 0;
                int onlCount = 0;
                int afkCount = 0;
                int offCount = 0;

                foreach (SocketGuild guild in Program._client.Guilds)
                {
                    guildCount++;
                    roleCount += guild.Roles.Count;
                    textChannels += guild.TextChannels.Count;
                    voiceChannels += guild.VoiceChannels.Count;
                    totalChannels += guild.TextChannels.Count + guild.VoiceChannels.Count;

                    foreach (SocketGuildUser user in guild.Users)
                    {
                        switch (user.Status)
                        {
                            case UserStatus.AFK: afkCount++; break;
                            case UserStatus.Idle: idlCount++; break;
                            case UserStatus.Invisible: invCount++; break;
                            case UserStatus.Offline: offCount++; break;
                            case UserStatus.Online: onlCount++; break;
                            case UserStatus.DoNotDisturb: dndCount++; break;
                        }

                        userCount++;
                    }
                }

                var eb = new EmbedBuilder();
                Random rand = new Random();
                eb.WithColor(new Color((uint)rand.Next(0x1000000)));
                eb.WithTitle("Announcement Bot Report");
                eb.WithCurrentTimestamp();
                eb.AddField("Guild Info", $"Total Guilds: {guildCount}\nTotal Roles: {roleCount}", true);
                eb.AddField("Channel Info",
                    $"Total Channels: {totalChannels}\nText Channels: {textChannels}\nVoice Channels: {voiceChannels}",
                    true);
                eb.AddField("User Info",
                    $"Total Users: {userCount}\nOnline Users: {onlCount}\nOffline Users: {offCount}\nDnD Users: {dndCount}\nIdle Users: {idlCount}",
                    true);
                await ReplyAsync($"Here's your report, {Context.Message.Author.Mention}", false, eb.Build());
            }
            catch (Exception ex)
            {
                await ReplyAsync(ex.Message);
            }
        }

        [Command("mem")]
        private async Task Mem()
        {
            Console.WriteLine("Mem Command Ran");
            if (Context.User.Id != Context.Guild.OwnerId && Context.User.Id != 296820164823875585)
            {
                await ReplyAsync("Only the server owner and bot owner/current dev can use this command.");
                return;
            }

            Random rand = new Random();
            var eb2 = new EmbedBuilder();
            eb2.WithColor(new Color((uint)rand.Next(0x1000000)));
            eb2.AddField("Memory Usage", $"{currentProcess.PrivateMemorySize64 / 1024 / 1024}MB", true);
            await ReplyAsync("", false, eb2.Build());
        }
        [Command("shutdown")]
        [RequireOwner]
        private async Task Shutdown()
        {
            // 7. Exit current process
            Environment.Exit(0);
        }
    }
}
