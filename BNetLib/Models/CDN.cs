using System;
using System.Collections.Generic;
using System.Text;

namespace BNetLib.Models
{
    public class CDN
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Hosts { get; set; }
        public string Servers { get; set; }
        public string ConfigPath { get; set; }
    }
}
