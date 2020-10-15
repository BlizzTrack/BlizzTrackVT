using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlizzTrackVT.Pages
{
    public class StatusCodeModel : PageModel
    {
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public void OnGet(int? statusCode = null)
        {
            StatusCode = statusCode ?? 0;
            if (statusCode == 404)
            {
                Message = "Requested page not found";
            }
        }
    }
}
