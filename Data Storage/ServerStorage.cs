using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;

namespace Announcement_Bot_Core
{
    public class ServerStorage
    {
        private static List<Server> servers;

        static ServerStorage()
        {
            // If there is a save, load the save. Otherwise, create a new one
            if (Data.SaveExists())
            {
#pragma warning disable CS8604 // Possible null reference argument.
                servers = Data.LoadGuilds().ToList();
#pragma warning restore CS8604 // Possible null reference argument.
            }
            else
            {
                servers = new List<Server>();
                SaveGuilds();
            }
        }

        public static void SaveGuilds()
        {
            // Save the guilds to the GuildData file
            Data.SaveGuilds(servers);
        }

        public static Server GetGuild(SocketGuild guild)
        {
            // Return a guild from the guilds list
            return GetOrCreateAccount(guild.Id);
        }

        private static Server GetOrCreateAccount(ulong id)
        {
            // If the guild exists, return it. Otherwise, create a new one and return it instead
            var result = from a in servers
                         where a.ID == id
                         select a;

            var account = result.FirstOrDefault();
            if (account == null) account = CreateGuildAccount(id);
            return account;
        }

        private static Server CreateGuildAccount(ulong id)
        {
            // Create a new guild, add it to the server list and save it
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            var newGuild = new Server()
            {
                ID = id,
                Modlog = false,
                Modlogid = 0,
                name = Program._client.GetGuild(id).Name,
                prefix = Program.prefix,
                ModLogReasons = null
            };
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            servers.Add(newGuild);
            SaveGuilds();
            return newGuild;
        }
    }
}
