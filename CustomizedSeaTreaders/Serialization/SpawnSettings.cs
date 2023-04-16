using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizedSeaTreaders.Serialization
{
    public class SpawnSettings
    {
        public class SpawnConfig
        {
            public float Weight { get; set; } = 1.0f;
        }

        public float Chance { get; set; } = 1.0f;
        public Dictionary<string, SpawnConfig> Spawns = new Dictionary<string, SpawnConfig>();
    }
}
