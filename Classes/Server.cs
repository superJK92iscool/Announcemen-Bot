using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Announcement_Bot_Core
{
    [Serializable]
    public class Server
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string name { get; set; }
        public string prefix { get; set; }
        public ulong ID { get; set; }
        public bool Modlog { get; set; }
        public ulong Modlogid { get; set; }
        public List<string> ModLogReasons { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    }
}
