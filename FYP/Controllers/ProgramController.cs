using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace FYPDraft.Controllers
{
    public class ProgramController : Controller
    {
        public IActionResult Calendar()
        {
            return View();
        }
    }
}