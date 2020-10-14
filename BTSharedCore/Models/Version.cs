using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;

namespace BTSharedCore.Models
{
    public class Version : Base
    {
        public List<BNetLib.Models.Version> Value { get; set; }

        public int Seqn { get; set; }

        public string Product { get; set; }
    }
}
