using System.Collections.Generic;
using System.Threading.Tasks;
using BlizzTrackVT.Models;
using BTSharedCore.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using BGDL = BTSharedCore.Data.BGDL;
using CDN = BTSharedCore.Data.CDN;
using Version = BNetLib.Models.Version;

namespace BlizzTrackVT.Pages
{
    public class ViewGameModel : PageModel
    {
        private readonly ILogger<ViewGameModel> _logger;
        private readonly Versions _versions;
        private readonly CDN _cdn;
        private readonly BGDL _bgdl;

        [BindProperty(SupportsGet = true)]
        public string Product { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? Seqn { get; set; }

        [BindProperty(SupportsGet = true)]
        public string File { get; set; }

        public GenericHistoryModel<BTSharedCore.Models.Version> Versions { get; set; } = new GenericHistoryModel<BTSharedCore.Models.Version>();
        public GenericHistoryModel<BTSharedCore.Models.BGDL> BGDL { get; set; } = new GenericHistoryModel<BTSharedCore.Models.BGDL>(); 
        public BTSharedCore.Models.CDN CDN { get; set; }

        public ViewGameModel(ILogger<ViewGameModel> logger, Versions versions, CDN cdn, BGDL bgdl)
        {
            _logger = logger;
            _versions = versions;
            _cdn = cdn;
            _bgdl = bgdl;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            switch (File.ToLower())
            {
                case "cdn":
                    if (Seqn != null)
                    {
                        CDN = await _cdn.Get(Product, Seqn.Value);
                    }
                    else
                    {
                        CDN = await _cdn.Latest(Product);
                    }

                    if (CDN == null) return NotFound();
                    break;
                case "versions":
                    Versions.Latest = await _versions.Latest(Product);

                    if (Seqn != null)
                    {
                        Versions.Current = await _versions.Get(Product, Seqn.Value);
                    }
                    else
                    {
                        Versions.Current = await _versions.Latest(Product);
                    }

                    if (Versions.Current == null) return NotFound();
                    Versions.Previous = await _versions.Previous(Product, Versions.Current.Seqn) ??
                                        new BTSharedCore.Models.Version()
                                        {
                                            Value = new List<Version>(),
                                            Seqn = 0
                                        };
                    break;
                case "bgdl":
                    BGDL.Latest = await _bgdl.Latest(Product);
                    if (Seqn != null)
                    {

                        BGDL.Current = await _bgdl.Get(Product, Seqn.Value);
                    }
                    else
                    {
                        BGDL.Current = await _bgdl.Latest(Product);
                    }

                    if (BGDL.Current == null) return NotFound();
                    BGDL.Previous = await _bgdl.Previous(Product, BGDL.Current.Seqn);
                    break;
                default:
                    return NotFound();
            }

            return Page();
        }
    }
}
