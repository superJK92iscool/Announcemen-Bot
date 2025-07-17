using System;
using System.Collections.Generic;
using System.IO;
using ProtoBuf;

namespace WPFApp
{
    [ProtoContract]
    public class ServerData
    {
        [ProtoMember(1)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string Name { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        // Add other properties of Server class as ProtoMembers
        // Example:
        // [ProtoMember(2)]
        // public int ServerId { get; set; }
    }
}

namespace Announcement_Bot_Core
{
    public class Data
    {
        static string path = "Data\\GuildData.bin";

        public static void SaveGuilds(IEnumerable<Server> guilds)
        {
            try
            {
                using FileStream stream = new FileStream(path, FileMode.Create);
                Serializer.Serialize(stream, guilds);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }
        }

        public static IEnumerable<Server>? LoadGuilds()
        {
            try
            {
                if (File.Exists(path))
                {
                    using (FileStream stream = new FileStream(path, FileMode.Open))
                    {
                        return Serializer.Deserialize<List<Server>>(stream);
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error: Guild Data not found.");
                    Console.ResetColor();
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return null;
            }
        }

        public static bool SaveExists()
        {
            return File.Exists(path);
        }
    }
}