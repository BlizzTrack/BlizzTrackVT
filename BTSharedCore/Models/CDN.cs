using System.Collections.Generic;

namespace BTSharedCore.Models
{
    public class CDN : Base
    {
        public List<BNetLib.Models.CDN> Value { get; set; }

        public int Seqn { get; set; }

        public string Product { get; set; }
    }
}
