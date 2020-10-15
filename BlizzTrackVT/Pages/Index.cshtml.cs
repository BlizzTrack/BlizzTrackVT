using System.Threading.Tasks;
using BTSharedCore.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace BlizzTrackVT.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly Summary _summary;

        public BTSharedCore.Models.Summary Summary;
        public BTSharedCore.Models.Summary PreviousSummary;

        public IndexModel(ILogger<IndexModel> logger, Summary summary)
        {
            _logger = logger;
            _summary = summary;
        }

        public async Task OnGetAsync()
        {
            Summary = await _summary.Latest();
            PreviousSummary = await _summary.Previous(string.Empty, Summary.Seqn);
        }
    }
}
