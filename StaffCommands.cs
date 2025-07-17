using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Announcement_Bot_Core
{
    public class StaffCommands : ModuleBase<SocketCommandContext>
    {
        [Command("addmodlog")]
        [RequireUserPermission(GuildPermission.ManageChannels)]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        private async Task infot()
        {
            Console.WriteLine("Add Mod Log command ran");
            // Add a mod log to the guild collected from guildstorage and create the channel
            var guild = ServerStorage.GetGuild(Context.Guild);
            await ReplyAsync("Adding a moderation log to this guild...");
            IGuildChannel chnl = await Context.Guild.CreateTextChannelAsync("mod-log");
            guild.Modlog = true;
            guild.Modlogid = chnl.Id;
            ServerStorage.SaveGuilds();
            await ReplyAsync("Your guild now has a moderation log. To remove this log type " + Program.prefix + "removemodlog");
        }


        [Command("removemodlog")]
        [RequireUserPermission(GuildPermission.ManageChannels)]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireUserPermission(GuildPermission.ManageMessages)]
#pragma warning disable IDE0051 // Remove unused private members
        private async Task RemoveModLog()
#pragma warning restore IDE0051 // Remove unused private members
        {
            Console.WriteLine("Remove Mod Log command ran");
            // Remove the modlog from the guild collected from guildstorage and remove the channel
            var guild = ServerStorage.GetGuild(Context.Guild);
            var chnl = Context.Guild.GetChannel(guild.Modlogid);
            await ReplyAsync("Now removing modlog from your guild...");
            await chnl.DeleteAsync();
            guild.Modlogid = 0;
            guild.Modlog = false;
            ServerStorage.SaveGuilds();
            await ReplyAsync("Your guild no longer has a moderation log");
        }


        [Command("testmodlog")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        private async Task DBG()
        {
             Console.WriteLine("Test Mod Log command ran");
            // Variable definitions 
            var guild = ServerStorage.GetGuild(Context.Guild);
            SocketTextChannel log = (SocketTextChannel)Context.Guild.GetChannel(guild.Modlogid);

            // Notify the user
            await ReplyAsync("Sending a test message to your modlog");

            // Create the embed and send it to the modlog
            var eb = new EmbedBuilder();
            eb.WithTitle("This is a moderation log test message");
            eb.WithCurrentTimestamp();
            eb.WithColor(Discord.Color.Blue);
            eb.WithDescription("This is a test message to test your servers mod log sent from " + Context.Message.Author.Username);
            var ebb = eb.Build();
            await log.SendMessageAsync("", false, ebb);
            await ReplyAsync("The message has been sent successfully ");
        }


        [Command("kick")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        private async Task Kick(IGuildUser? user = null, [Remainder] string reason = "reason not set")
        {
             Console.WriteLine("Kick command ran");
            // If an invalid user is chosen, notify the user
            if (user == null)
                await ReplyAsync("Invalid User");
            else
            {
                // Delete the context message and send the kicked message
                await Context.Message.DeleteAsync();
                await ReplyAsync("Kicked " + user.Username + " for " + reason);

                // Get the guild and save all guilds
                var guild = ServerStorage.GetGuild(Context.Guild);
                ServerStorage.SaveGuilds();

                if (guild.Modlog)
                {
                    // Create a modlog embed and send it to the modlog
                    var eb = new EmbedBuilder();
                    eb.WithColor(Discord.Color.Teal);
                    eb.WithCurrentTimestamp();
                    eb.WithTitle("A user has been kicked!");
                    if (user.IsBot)
                    {
                        eb.WithTitle("A bot has been kicked!");
                        eb.AddField("Bot", user.Username, true);
                    }
                    else
                    {
                        eb.WithTitle("A user has been kicked!");
                        eb.AddField("User", user.Username, true);
                    }
                    eb.AddField("Moderator", Context.Message.Author.Username, true);
                    eb.AddField("Reason", reason, true);
                    eb.WithThumbnailUrl(user.GetAvatarUrl());
                    SocketTextChannel ml = (SocketTextChannel)Context.Guild.GetChannel(guild.Modlogid);
                    var ebb = eb.Build();
                    await ml.SendMessageAsync("", false, ebb);
                }
                await user.KickAsync(reason);
            }
        }


        [Command("ban")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        private async Task Ban(IGuildUser? user = null, [Remainder] string reason = "reason not set")
        {
             Console.WriteLine("Ban command ran");
            // If an invalid user is chosen, notify the user
            if (user == null)
                await ReplyAsync("Invalid User");
            else
            {
                // Delete the context message and tell the user that the ban worked
                await Context.Message.DeleteAsync();
                await ReplyAsync("Banned " + user.Username + " for " + reason);

                // Get the current guild and save all guilds
                var guild = ServerStorage.GetGuild(Context.Guild);

                ServerStorage.SaveGuilds();
                if (guild.Modlog)
                {
                    // Create the modlog embed and send it
                    var eb = new EmbedBuilder();
                    eb.WithColor(Discord.Color.Red);
                    eb.WithCurrentTimestamp();
                    if (user.IsBot)
                    {
                        eb.WithTitle("A bot has been banned!");
                        eb.AddField("Bot", user.Username, true);
                    }
                    else
                    {
                        eb.WithTitle("A user has been banned!");
                        eb.AddField("User", user.Username, true);
                    }
                    eb.AddField("Moderator", Context.Message.Author.Username, true);
                    eb.AddField("Reason", reason, inline: true);
                    eb.WithThumbnailUrl(user.GetAvatarUrl());
                    var ebb = eb.Build();
                    SocketTextChannel ml = (SocketTextChannel)Context.Guild.GetChannel(guild.Modlogid);
                    await ml.SendMessageAsync("", false, ebb);
                }
                // Ban the user
                await Context.Guild.AddBanAsync(user, 1, reason);
            }
        }


        [Command("purge", RunMode = RunMode.Async)]
        [Summary("Deletes the specified amount of messages.")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task PurgeChat(int amount = 0)
        {
             Console.WriteLine("Purge command ran");
            if (amount != 0)
            {
                IEnumerable<IMessage> messages = await Context.Channel.GetMessagesAsync(amount + 1).FlattenAsync(); // Get the group of messages to delete
                await ((ITextChannel)Context.Channel).DeleteMessagesAsync(messages); // Delete the messages stored in the messages IEnumerable
                IUserMessage m = await ReplyAsync($"Messages purged. This message will be deleted in 3 seconds"); // Notify users about the purge
                await Task.Delay(3000); // Wait for 3 seconds
                await m.DeleteAsync(); // Delete the purged notification
            }
        }
    }
}
