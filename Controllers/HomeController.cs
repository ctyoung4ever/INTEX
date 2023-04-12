using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using INTEX.Models;
using INTEX.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace INTEX.Controllers
{
    public class HomeController : Controller
    {
        private mummyContext MummyContext { get; set; }
       

        //Controller
        //public HomeController(mummyContext con)
        //{
        //    MummyContext = con;
        //}

        public HomeController(mummyContext con)
        {
            
            MummyContext = con;
        }


        public IActionResult Index()
        {
            return View();
        }


        [Authorize(Roles = "Authorized")]
        public IActionResult BurialList(int pageNum = 1)
        {
            int pageSize = 50;

            var x = new BurialViewModel
            {
                Burials = MummyContext.Burialmain
                //.Include(x => x.BurialmainTextile)
                .OrderBy(x => x.Id)
                .Skip((pageNum - 1) * pageSize)
                .Take(pageSize)
                .ToList(),

                PageInfo = new PageInfo
                {
                    TotalNumBurials = MummyContext.Burialmain.Count(),
                    BurialsPerPage = pageSize,
                    CurrentPage = pageNum
                }
            };

            //var mummies = repo.burialmains
            //    .OrderBy(x => x.Id)
            //    .Skip((pageNum - 1) * pageSize)
            //    .Take(pageSize);
            //.Include(x => x.Burialmaterials)
            //.ToList();
            return View(x);
        }
        [HttpGet]
        public IActionResult CreateBurialItem()
        {
            var mummies = MummyContext.Burialmain.ToList();
            return View("CreateBurialItem", new Burialmain());
        }

        [HttpPost]
        public IActionResult CreateBurialItem(Burialmain ar)
        {
            MummyContext.Add(ar);
            MummyContext.SaveChanges();
            return View("Index");
        }

        [HttpGet]
        public IActionResult DeleteBurialItem(long id)
        {
            var burialItem = MummyContext.Burialmain.Single(x => x.Id == id);



            return View(burialItem);
        }

        [HttpPost]
        public IActionResult DeleteBurialItem(Burialmain viewModel)
        {
            //var burialItem = MummyContext.Burialmain.Single(x => x.Id == viewModel.Id);
            MummyContext.Burialmain.Remove(viewModel);
            MummyContext.SaveChanges();
            //MummyContext.Burialmain.Remove(burialItem);
            //MummyContext.SaveChanges();
            return RedirectToAction("BurialList");
        }

        public IActionResult detailsburialitem(long id)
        {
            var application = MummyContext.Burialmain.Single(x => x.Id == id);

            return View("detailsburialitem", application);
        }
        [Authorize(Roles ="Admin")]
        public IActionResult Privacy()
        {
            return View();
        }
        [HttpGet]
        public IActionResult editBurialList(long id)
        {

            var application = MummyContext.Burialmain.Single(x => x.Id == id);
            return View("editBurialList", application);
        }
        [HttpPost]
        public IActionResult editBurialList(Burialmain viewModel)
        {

            if (ModelState.IsValid)
            {
                MummyContext.Update(viewModel);
                MummyContext.SaveChanges();
                return RedirectToAction("editBurialList");
            }
            else
            {
                return View("BurialList");
            }

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
