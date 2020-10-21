﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace BlizzTrackVT.Services
{
    public class NavigationService
    {
        public Dictionary<string, List<BNetLib.Models.Summary>> Create(List<BNetLib.Models.Summary> items)
        {
            var p = BNetLib.Helpers.GameName.Prefix.Keys.OrderBy(x => x);

            var res = new Dictionary<string, List<BNetLib.Models.Summary>>();

            foreach (var prefix in p)
            {
                var i = new List<BNetLib.Models.Summary>();

                switch (prefix)
                {
                    case "agent":
                    case "bts":
                    case "catalogs":
                        continue;
                    case "bna":
                        i.AddRange(items.Where(x => x.Product == "bna"));
                        i.AddRange(items.Where(x => x.Product == "catalogs"));
                        i.AddRange(items.Where(x => x.Product == "agent"));
                        i.AddRange(items.Where(x => x.Product == "bts"));
                        res.Add(prefix, i);

                        continue;
                }

                foreach (var item in items)
                {
                    if (item.Product.ToLower().Replace("_", "").StartsWith(prefix, StringComparison.CurrentCultureIgnoreCase))
                    {
                        if(item.Product.StartsWith("wow_classic") && prefix == "wow") continue;
                        i.Add(item);
                    }
                }

                if(i.Count > 0)
                    res.Add(prefix, i);
            }

            return res;
        }
    }
}
