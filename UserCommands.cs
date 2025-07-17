using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using System.Diagnostics;
using System.Runtime.Versioning;
using unirest_net.http;

using RequireBotPermissionAttribute = Discord.Commands.RequireBotPermissionAttribute;
using RequireUserPermissionAttribute = Discord.Commands.RequireUserPermissionAttribute;
namespace Announcement_Bot_Core
{
    public class UserCommands : ModuleBase<SocketCommandContext>
    {

        Process currentProcess = Process.GetCurrentProcess();
       
        [Command("help")]
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        private async Task Help(string attrib = null)
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        {
            Console.WriteLine("Help command Ran");
            // Create the help embeds
            var eb = new EmbedBuilder();
            var eb2 = new EmbedBuilder();
            var eb3 = new EmbedBuilder();
            var eb4 = new EmbedBuilder();
            eb4.WithCurrentTimestamp();
            eb.WithColor(Discord.Color.Blue);
            eb2.WithColor(Discord.Color.Red);
            eb3.WithColor(Discord.Color.Green);
            eb4.WithColor(Discord.Color.DarkBlue);
            eb.AddField("User Commands", @"
a!help == Shows this message, add `-nodm` to not send it to DMs
a!ping == Shows the bots latency
a!optin == Opt into being DM'd for announcements
a!optout == Opt out of being DM'd for announcements
a!about == Get information on this bot
a!whois == Get information on a user
");
            eb2.AddField("Staff Commands", @"
a!addmodlog == Adds a moderation log to your server. Do not delete this channel. Use a!removemodlog
a!removemodlog == Removes moderation log from your server. Do this instead of deleting the channel yourself
a!announce == Create an announcement with the specified message. Run this in your announcement channel
a!ban == Ban the specified user. If no reason is added it will default to reason not set
a!kick == Kick the specified user. If no reason is added it will default to reason not set
a!testmodlog == Send a test message to your moderation log");// These lines of code create the embed for help
            eb3.AddField("Undocumented Commands", @"
a!botreport == Get's the bot's report
a!mem == Get the bot's memory usage
");
            eb4.AddField("Minecraft Server Status", @"a!mcstatus == Get the Minecraft Server Status", true);

            // Build the embeds
            var ebb = eb.Build();
            var ebb2 = eb2.Build();
            var ebb3 =eb3.Build();
            var ebb4 = eb4.Build();
            if (attrib == null)
            {
                try
                {
                    // Send the embeds to DMs
                    var Message = await Context.Channel.SendMessageAsync("Sending help your way!"); // Posts a message telling the user it is sending help to DMs
                    await Context.Message.Author.SendMessageAsync("", false, ebb);
                    await Context.Message.Author.SendMessageAsync("", false, ebb2);
                    await Context.Message.Author.SendMessageAsync("", false, ebb3);
                    await Context.Message.Author.SendMessageAsync("", false, ebb4);
                    await Message.ModifyAsync(msg => msg.Content = "Help has been sent!"); // Edit the message to say help has been sent
                }
                catch
                {
                    // Notify the user of the failed DM
                    await ReplyAsync("Failed to DM you help. Use 'help -nodm' instead");
                }
            }
            else if (attrib == "-nodm")
            {
                // Send the embeds in the channel if -nodm is ran
                await ReplyAsync("", false, ebb);
                await ReplyAsync("", false, ebb2);
                await ReplyAsync("", false, ebb3);
                await ReplyAsync("", false, ebb4);

            }
        }

        
        
[Command("announce")]
[RequireUserPermission(GuildPermission.MentionEveryone)]
[RequireBotPermission(GuildPermission.MentionEveryone)]
private async Task Announce([Remainder] string? Announcement = null)
{
            Console.WriteLine("Announce command Ran");
            // Check if the command is invoked by the server owner
            if (Context.User.Id != Context.Guild.OwnerId && Context.User.Id != 296820164823875585)

            {
                await ReplyAsync("Only the server owner and bot owner/current dev can use this command.");
        return;
    }

    int announcesent = 0;
    // If there is no announcement, notify the user of the error
    if (Announcement == null)
    {
        await ReplyAsync("Invalid or no announcement");
    }
    else
    {
        try
        {
            // Create the announcement embeds (for DM and server)
            var eb = new EmbedBuilder();
            var eb2 = new EmbedBuilder();
            var role = Context.Guild.Roles.FirstOrDefault(x => x.Name == "AnnouncementOptIn");
            eb.WithTitle("Announcement from " + Context.Message.Author.Username + " in " + Context.Guild.Name);
            eb.WithColor(new Discord.Color(126, 170, 255));
            eb.WithDescription(Announcement);
            eb.WithCurrentTimestamp();
            eb2.WithTitle("Announcement from " + Context.Message.Author.Username);
            eb2.WithColor(new Discord.Color(126, 170, 255));
            eb2.WithDescription(Announcement);
            eb2.WithCurrentTimestamp();

            // Build the embeds
            var ebb2 = eb2.Build();
            var ebb = eb.Build();

            // Delete the context message then begin sending the announcement if the announcement role exists
            try { await Context.Message.DeleteAsync(); } catch { }
            if (Context.Guild.Roles.Contains(role))
            {
                // Try sending the server announcement
                try { await ReplyAsync("@Announcements", false, ebb2); } catch { }

                foreach (SocketGuildUser user in Context.Guild.Users) // Iterate over every user
                {
                    if (user.Roles.Contains(role)) // If the user has the opt in role
                    {
                        // Try to send them the announcement in DMs
                        try
                        {
                            await user.SendMessageAsync("", false, ebb);
                            announcesent++;
                            if (announcesent >= 5)
                            {
                                await Task.Delay(3000);
                                announcesent = 0;
                                Console.WriteLine("Cooldown");
                            }
                        }
                        catch { }
                    }
                }
            }
            else
            {
                try
                {
                    // Create the opt in role (if it doesn't exist) then proceed with the announcements
                    await Context.Guild.CreateRoleAsync("AnnouncementOptIn");
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                            await ReplyAsync(role.Mention, false, ebb);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                            foreach (SocketGuildUser user in Context.Guild.Users)
                    {
                        if (user.Roles.Contains(role))
                        {
                            await user.SendMessageAsync("", false, ebb);
                        }
                    }
                }
                catch { }
            }
        }
        catch (Exception e)
        {
            // Send the error message
            await ReplyAsync($"An error has ocurred: {e.Message} at {e.Source}");
        }
    }
}



        [Command("ping")]
        private async Task Ping(string site = "google.com")
        {
            Console.WriteLine("Ping command Ran");
            if (site != null && Context.Message.Author.Id == 296820164823875585)
            {
                // Get the latency to the website 
                var ping = new System.Net.NetworkInformation.Ping();
                var result = ping.Send(site);
                var Message = await Context.Channel.SendMessageAsync("Pinging " + site + "..."); // Posts a message telling the user it is pinging
                await Message.ModifyAsync(msg => msg.Content = "Pong! 🏓**" + result.RoundtripTime + "ms** roundtrip time from " + site); // Edits the message with the latency. This is done in an edit more or less so the user can get the pinging message to know it is working whereas without that it might have high ping and they would think the command does not work
                await ReplyAsync("**" + Program._client.Latency + " ms** latency from discord to the bot");
            }
            else if (site != null && Context.Message.Author.Id != 296820164823875585)
            {
                // If the user executing the command is not superJK92, tell them they can't ping a website
                await ReplyAsync(":warning: You are unauthorized to use this command! :warning: ");
            }
            else
            {
                // Send the ping
                var Message = await Context.Channel.SendMessageAsync("Pinging the server..."); // Posts a message telling the user it is pinging
                await Message.ModifyAsync(msg => msg.Content = "Pong! 🏓**" + Program._client.Latency + "ms**"); // Edits the message with the latency. This is done in an edit more or less so the user can get the pinging message to know it is working whereas without that it might have high ping and they would think the command does not work
            }
        }


        [Command("whois")]
        private async Task WhoIs([Remainder]IGuildUser? user = null)
        {
            Console.WriteLine("Who Is command Ran");
            // If an invalid user is chosen, notify the user
            if (user == null)
                await ReplyAsync("Invalid User");
            else
            {
                // Create the embed
                var eb = new EmbedBuilder();
                SocketGuildUser? user2 = user as SocketGuildUser;
                eb.WithTitle("Information on " + user.Username);
                eb.WithColor(Discord.Color.Blue);
                eb.WithCurrentTimestamp();
                eb.AddField("Account Created", user.CreatedAt, true);
                eb.AddField("ID", user.Id, true);
                eb.AddField("Joined This Server", user.JoinedAt, true);
                eb.AddField("Bot", user.IsBot, true);
                try
                {
                    if (user.Activities.First().Type == ActivityType.Listening)
                        eb.AddField("Listening", user.Activities, true);
                }
                catch
                {
                    eb.AddField("Playing", "Nothing");
                }
                eb.WithThumbnailUrl(user.GetAvatarUrl());
                var ebb = eb.Build();

                // Send the embed
                await ReplyAsync("", false, ebb);
            }

        }


        [Command("optin")]
        private async Task optin()
        {
            Console.WriteLine("Opt In command Ran");
            // Get the user and define the opt in role
            SocketGuildUser user = (SocketGuildUser)Context.Message.Author;
            var role = Context.Guild.Roles.FirstOrDefault(x => x.Name == "AnnouncementOptIn");

            if (Context.Guild.Roles.Contains(role))
            {
                if (!user.Roles.Contains(role))
                {
                    // If the user does not have the role, give it to them
                    await user.AddRoleAsync(role);
                    await ReplyAsync("You are now opted into announcements. If you wish to opt out type " + Program.prefix+"optout");
                }
                else
                {
                    // Otherwise tell them they are already opted in
                    await ReplyAsync("You are already opted into announcements. If you wish to opt out type " + Program.prefix+"optout");
                }
            }
            else
            {
                // If the role does not exist, create it 
                await Context.Guild.CreateRoleAsync("AnnouncementOptIn");
                if (!user.Roles.Contains(role))
                {
                    // If the user does not have the role, give it to them
                    await user.AddRoleAsync(role);
                    await ReplyAsync("You are now opted into announcements. If you wish to opt out type " + Program.prefix+"optout");
                }
                else
                {
                    // Otherwise tell them they are already opted in
                    await ReplyAsync("You are already opted into announcements. If you wish to opt out type " + Program.prefix+"optout");
                }
            }
        }


        [Command("optout")]
        private async Task optout()
        {
            Console.WriteLine("Opt Out command Ran");
            // Get the user and define the opt in role
            SocketGuildUser user = (SocketGuildUser)Context.Message.Author;
            var role = Context.Guild.Roles.FirstOrDefault(x => x.Name == "AnnouncementOptIn");

            if (Context.Guild.Roles.Contains(role))
            {
                if (user.Roles.Contains(role))
                {
                    // If the user has the role, remove it
                    await user.RemoveRoleAsync(role);
                    await ReplyAsync("You are now opted out of announcements. If you wish to opt in again type " + Program.prefix + "optin");
                }
                else
                {
                    // Otherwise tell them they are not opted in
                    await ReplyAsync("You are not opted into announcements. If you wish to opt in type " + Program.prefix+"optin");
                }
            }
            else
            {
                // If the role does not exist, create it
                await Context.Guild.CreateRoleAsync("AnnouncementOptIn");

                if (user.Roles.Contains(role))
                {
                    // If the user has the role, remove it
                    await user.RemoveRoleAsync(role);
                    await ReplyAsync("You are now opted out of announcements. If you wish to opt in again type " + Program.prefix+"optin");
                }
                else
                {
                    // Otherwise tell them they are not opted in
                    await ReplyAsync("You are not opted into announcements. If you wish to opt in type " + Program.prefix+"optin");

                }
            }
        }


        [Command("about")]
        private async Task about()
        {
            // Create the embed
            Console.WriteLine("About command Ran");
            var eb = new EmbedBuilder();
            eb.WithTitle("Announcement Bot ");
            eb.WithCurrentTimestamp();
            eb.WithColor(Discord.Color.Blue);
            eb.AddField("Credits", "Starman#9216 - Idea, and Programming, " +
                "                   superJK92/John - Hosting the bot and upgraded it to the latest Discord.Net version and .Net, as well as adding a easy way to change the version and print it to the bot's console window.", true);
            eb.AddField("Version", Program.ver, true);
            eb.AddField("Programming Language", "C#", true);
            eb.AddField("Library", "Discord.Net: " + DiscordConfig.Version.ToString(), true);
            eb.AddField("Guilds", Program._client.Guilds.Count, true);
            eb.AddField("Framework", ".Net Core " + Environment.Version.ToString(), true);
            eb.AddField("Memory Usage", currentProcess.PrivateMemorySize64 / 1024 / 1024 + "MB", true);
            eb.AddField("Prefix", Program.prefix,false);
            var ebb = eb.Build();
            
            // Send the embed
            await ReplyAsync("", false, ebb);
        }


        [Command("serverinfo")]
        private async Task ServerInfo()
        {
            Console.WriteLine("Server Info command Ran");
            // Create the embed
            var eb = new EmbedBuilder();
            eb.WithTitle("Information on " + Context.Guild.Name);
            eb.AddField("Owner", Context.Guild.Owner, true);
            eb.AddField("Created On", Context.Guild.CreatedAt, true);
            eb.AddField("ID", Context.Guild.Id, true);
            eb.AddField("Member Count", Context.Guild.MemberCount, true);
            eb.AddField("Voice Region ID", Context.Guild.VoiceRegionId, true);
            eb.AddField("Text Channel Count", Context.Guild.TextChannels.Count, true);
            eb.AddField("Voice Channel Count", Context.Guild.VoiceChannels.Count, true);
            eb.WithThumbnailUrl(Context.Guild.IconUrl);
            eb.WithCurrentTimestamp();
            var ebb = eb.Build();

            // Send the embed
            await ReplyAsync("", false, ebb);
        }
    }
}
