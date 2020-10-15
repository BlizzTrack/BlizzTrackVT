using System.Collections.Generic;
using System.Threading.Tasks;
using BlizzTrackVT.Models;
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
        public GenericHistoryModel<BTSharedCore.Models.Summary> Summary { get; } = new GenericHistoryModel<BTSharedCore.Models.Summary>();

        public Dictionary<string, List<BNetLib.Models.Summary>> Games;

        public IndexModel(ILogger<IndexModel> logger, Summary summary, NavigationService navigationService)
        {
            _logger = logger;
            _summary = summary;
            _navigationService = navigationService;
        }

        public async Task OnGetAsync(int? seqn = null)
        {
            var latest = await _summary.Latest();
            Summary.Current = latest;
            Summary.Latest = latest;

            if (seqn != null)
            {
                Summary.Current = await _summary.Get(string.Empty, seqn.Value) ?? latest;
            }

            Summary.Previous = await _summary.Previous(string.Empty, Summary.Current.Seqn);

           Games = _navigationService.Create(Summary.Current.Value);
        }
    }
}
