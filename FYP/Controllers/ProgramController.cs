using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FYPDraft.Controllers
{
    public class ProgramController : Controller
    {
        [Authorize(Roles = "Admin,Alumni,Startup")]
        public IActionResult Calendar()
        {
            return View();
        }
    }
}