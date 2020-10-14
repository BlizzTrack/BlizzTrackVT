using System.Collections.Generic;

namespace BTSharedCore.Models
{
    public class Summary : Base
    {
        public List<BNetLib.Models.Summary> Value { get; set; }

        public int Seqn { get; set; }
    }
}
