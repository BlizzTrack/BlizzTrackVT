using System;
using System.Collections.Generic;
using System.Linq;
using BNetLib.Helpers;
using BNetLib.Models;

namespace BlizzTrackVT.Services
{
    public class NavigationService
    {
        public Dictionary<string, List<Summary>> Create(List<Summary> items)
        {
            var p = GameName.Prefix.Keys.OrderBy(x => x).ToList();

            var res = new Dictionary<string, List<Summary>>();

            var itemsAdded = new List<Summary>();

            foreach (var prefix in p)
            {
                var i = new List<Summary>();

                switch (prefix)
                {
                    case "agent":
                    case "bts":
                    case "catalogs":
                        continue;
                    case "bna":

                        var its = items.Where(x => x.Product == "bna" || x.Product == "catalogs" || x.Product == "agent" || x.Product == "bts");
                        var collection = its as Summary[] ?? its.ToArray();
                        i.AddRange(collection);
                        itemsAdded.AddRange(collection);

                        res.Add(prefix, i);

                        continue;
                }

                foreach (var item in items.Where(item => item.Product.ToLower().Replace("_", "")
                    .StartsWith(prefix, StringComparison.CurrentCultureIgnoreCase)))
                {
                    if (item.Product.StartsWith("wow_classic") && prefix == "wow")
                    {
                        itemsAdded.Add(item);
                        continue;
                    }

                    itemsAdded.Add(item);
                    i.Add(item);
                }

                if (i.Count > 0)
                    res.Add(prefix, i);
            }

            foreach (var item in itemsAdded)
            {
                items.Remove(item);
            }

            if (items.Count <= 0) 
                return res;

            res.Add("Unknown", items.ToList());

            return res;
        }
    }
}
