using System;
using System.Collections.Generic;
using System.Text;

namespace BNetLib.Models
{

    /*
        Buildconfig: "e6ddfb824dd99f033124a78480cc8751",
        Buildid: "74102",
        Cdnconfig: "658611ac3ef9a13dee3f2c698713ea21",
        Keyring: "3dabb1bf65a3a797ccc6eff63c51b509",
        Region: "cn",
        Versionsname: "1.53.0.0.74102",
        Productconfig: "a94e0b154d36a6aca656554ecff0aa19",
     */
    public class Version
    {
        public string Buildconfig { get; set; }
        public int Buildid { get; set; }
        public string Cdnconfig { get; set; }
        public string Keyring { get; set; }
        public string Region { get; set; }
        public string Versionsname { get; set; }
        public string Productconfig { get; set; }
    }
}
