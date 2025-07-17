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
        private static readonly string BaseBuildPath = @"E:\Bots\AnnouncementBot\Build";
        private static readonly string TempBuildPath = Path.Combine(BaseBuildPath, "temp_build");
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

        private async Task SafeReplyAsync(string message)
        {
            const int maxLength = 1900;
            if (message.Length > maxLength)
                message = message.Substring(0, maxLength) + "\n...(truncated)";
            await ReplyAsync(message);
        }

        [Command("restart")]
        [RequireOwner]
        private async Task Restart()
        {
            await SafeReplyAsync("Starting build to temp folder...");

            // 1. Run dotnet build to temp_build folder
            var buildResult = await RunProcessAsync("dotnet", $"build -c Release -o \"{TempBuildPath}\"",
                Directory.GetParent(BaseBuildPath).FullName);
            if (!buildResult.success)
            {
                Console.WriteLine("Build Failed");
                return;
            }

            await SafeReplyAsync("Build succeeded. Preparing new run folder...");

            // 2. Create new unique run folder
            string newRunFolder = Path.Combine(BaseBuildPath, "run_" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
            Directory.CreateDirectory(newRunFolder);

            // 3. Copy build output to new run folder
            CopyDirectory(TempBuildPath, newRunFolder);

            await SafeReplyAsync("Copied build to new run folder. Starting new bot process...");

            // 4. Determine executable to run
            string exeName = GetExecutableName();

            string exePath = Path.Combine(newRunFolder, exeName);
            if (!File.Exists(exePath))
            {
                await SafeReplyAsync($"Executable not found in build output: {exePath}");
                return;
            }

            // 5. Start new bot process
            var startInfo = new ProcessStartInfo
            {
                FileName = exePath,
                WorkingDirectory = newRunFolder,
                UseShellExecute = true,
            };

            Process.Start(startInfo);

            // 6. Cleanup old run folders (except newRunFolder)
            CleanupOldRunFolders(newRunFolder);

            await SafeReplyAsync("Restarting now...");
            await Task.Delay(1000);

            // 7. Exit current process
            Environment.Exit(0);
        }

        private string GetExecutableName()
        {
            bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            return isWindows ? "Announcement Bot Core.exe" : "Announcement Bot Core";
        }

        private void CopyDirectory(string sourceDir, string targetDir)
        {
            foreach (var dirPath in Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories))
            {
                var newDirPath = dirPath.Replace(sourceDir, targetDir);
                if (!Directory.Exists(newDirPath))
                    Directory.CreateDirectory(newDirPath);
            }

            foreach (var filePath in Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories))
            {
                var newFilePath = filePath.Replace(sourceDir, targetDir);
                File.Copy(filePath, newFilePath, overwrite: true);
            }
        }

        private void CleanupOldRunFolders(string currentRunFolder)
        {
            var runFolders = Directory.GetDirectories(BaseBuildPath, "run_*");
            foreach (var folder in runFolders)
            {
                if (!folder.Equals(currentRunFolder, StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        Directory.Delete(folder, recursive: true);
                    }
                    catch
                    {
                        // Ignore failures, possibly folder is still locked
                    }
                }
            }
        }

        private async Task<(bool success, string output)> RunProcessAsync(string fileName, string args,
            string workingDir)
        {
            var psi = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = args,
                WorkingDirectory = workingDir,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            using var process = Process.Start(psi);
            string output = await process.StandardOutput.ReadToEndAsync() +
                            await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();

            return (process.ExitCode == 0, output);
        }
    }
}