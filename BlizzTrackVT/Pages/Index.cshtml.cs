using System.Collections.Generic;
using System.Threading.Tasks;
using BlizzTrackVT.Services;
using BTSharedCore.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace BlizzTrackVT.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly Summary _summary;
        private readonly NavigationService _navigationService;

        public BTSharedCore.Models.Summary Summary;
        public BTSharedCore.Models.Summary PreviousSummary;

        public Dictionary<string, List<BNetLib.Models.Summary>> Games;

        public IndexModel(ILogger<IndexModel> logger, Summary summary, NavigationService navigationService)
        {
            _logger = logger;
            _summary = summary;
            _navigationService = navigationService;
        }

        public async Task OnGetAsync()
        {
            Summary = await _summary.Latest();
            PreviousSummary = await _summary.Previous(string.Empty, Summary.Seqn);

           Games = _navigationService.Create(Summary.Value);
        }
    }
}
