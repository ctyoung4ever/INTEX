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


        //[Authorize(Roles = "Authorized")]
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
            var maxId = MummyContext.Burialmain.Max(x => x.Id);
            var pkid = maxId + 1;
           

            ViewBag.Pkid = pkid; // Set the Pkid property of the ViewBag to pkid.Id

            return View("CreateBurialItem", new Burialmain());
        }


        [HttpPost]
        public IActionResult CreateBurialItem(Burialmain ar)
        {


            MummyContext.Add(ar);
            MummyContext.SaveChanges();
            var maxbId = MummyContext.Burialmain.Max(x => x.Id);

            var mummies = MummyContext.Burialmain.ToList();
            var maxtextileid = MummyContext.BurialmainTextile.Max(x => x.MainTextileid);
            ViewBag.maxbId = maxbId;
            ViewBag.maxtextileid = maxtextileid + 1;
            return View("asktextile", new BurialmainTextile());
            
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
            var burialMain = MummyContext.Burialmain.SingleOrDefault(x => x.Id == id);
            var burialMainTextiles = MummyContext.BurialmainTextile.Where(bt => bt.MainBurialmainid == id).ToList();
            var textileIds = burialMainTextiles.Select(bt => bt.MainTextileid);
            var textiles = MummyContext.Textile.Where(t => textileIds.Contains(t.Id)).ToList();
            var bodyanalysis = MummyContext.Bodyanalysischart.SingleOrDefault(x => x.Id == id);
            
            var photoids = MummyContext.PhotodataTextile.Where(t => textileIds.Contains(t.MainTextileid));
            var newids = photoids.Select(bt => bt.MainPhotodataid);
            var photos = MummyContext.Photodata.Where(t => newids.Contains(t.Id)).ToList();
            var c14 = MummyContext.C14.SingleOrDefault(x => x.Id == id);
            //photodata connects to photodata_textile which connects to textile and burialmain
            var viewModel = new BurialViewModel
            {
                BurialMain = burialMain,
                TextileList = burialMainTextiles,
                Textile = textiles,
                BodyAnalysis = bodyanalysis,
                Photos = photos,
                C14 = c14
            };

            return View("detailsburialitem", viewModel);
        }

        //[Authorize(Roles ="Admin")]





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
        
        [HttpGet]
        public IActionResult asktextile()
        {


            var maxtextileid = MummyContext.BurialmainTextile.Max(x => x.MainTextileid);
            var maxbId = MummyContext.Burialmain.Max(x => x.Id);
            ViewBag.maxbId = maxbId;
            ViewBag.maxtextileid = maxtextileid + 1;
            return View("asktextile", new BurialmainTextile());
        }



        [HttpPost]
        public IActionResult asktextile(BurialmainTextile ar, BurialViewModel bvm)
        {
            if (ar.MainTextileid == 0)
            {
                ar.MainBurialmainid = bvm.Burialmain1.MainBurialmainid;
                ar.MainTextileid = bvm.Maxtextileid;
                MummyContext.Add(ar);
                MummyContext.SaveChanges();
            }
            else
            {
                MummyContext.Add(ar);
                MummyContext.SaveChanges();
            }

            var maxtextileid = MummyContext.BurialmainTextile.Max(x => x.MainTextileid);
            var maxbId = MummyContext.Burialmain.Max(x => x.Id);
            ViewBag.maxtextileid = maxtextileid;
            ViewBag.maxbId = maxbId;
            return View("CreateTextile", new Textile());
        }

        [HttpGet]
        public IActionResult CreateTextile()
        {

            var maxtextileid = MummyContext.BurialmainTextile.Max(x => x.MainTextileid);

            ViewBag.maxtextileid = maxtextileid;
            return View("asktextile", new Textile());
        }



        [HttpPost]
        public IActionResult CreateTextile(Textile ar)
        {
            MummyContext.Update(ar);
            MummyContext.SaveChanges();
            var maxtextileid = MummyContext.BurialmainTextile.Max(x => x.MainTextileid);
            var phototextid = MummyContext.PhotodataTextile.Max(x => x.MainPhotodataid);
            ViewBag.maxtextileid = maxtextileid;
            ViewBag.phototextid = phototextid + 1;

            return View("askphoto", new PhotodataTextile());


        }

        [HttpGet]
        public IActionResult askphoto()
        {

            var maxtextileid = MummyContext.Textile.Max(x => x.Id);
            var phototextid = MummyContext.PhotodataTextile.Max(x => x.MainPhotodataid);
            ViewBag.maxtextileid = maxtextileid;
            ViewBag.phototextid = phototextid + 1;

            return View("askphoto", new PhotodataTextile());
        }



        [HttpPost]
        public IActionResult askphoto(PhotodataTextile ar, BurialViewModel bvm)
        {

            if (ar.MainTextileid == 0)
            {
                ar.MainTextileid = bvm.Photodata.MainTextileid;
                ar.MainPhotodataid = bvm.Photodata.MainPhotodataid;
                MummyContext.Add(ar);
                MummyContext.SaveChanges();
            }

            else
            {
                MummyContext.Add(ar);
                MummyContext.SaveChanges();

            }

            var photoid = MummyContext.PhotodataTextile.Max(x => x.MainPhotodataid);

            ViewBag.phototextid = photoid;
            return View("CreatePhoto", new Photodata());


        }
        [HttpGet]
        public IActionResult CreatePhoto()
        {

            var maxtextileid = MummyContext.PhotodataTextile.Max(x => x.MainPhotodataid);

            ViewBag.phototextid = maxtextileid;
            return View("askphoto", new Photodata());
        }



        [HttpPost]
        public IActionResult CreatePhoto(Photodata ar)
        {
            MummyContext.Add(ar);
            MummyContext.SaveChanges();
            var maxtextileid = MummyContext.BurialmainTextile.Max(x => x.MainTextileid);
            var phototextid = MummyContext.PhotodataTextile.Max(x => x.MainPhotodataid);
            ViewBag.maxtextileid = maxtextileid;
            ViewBag.phototextid = phototextid + 1;



            var maxBurialmainId = MummyContext.Burialmain.Max(x => x.Id);
            var maxTextileId = MummyContext.BurialmainTextile.Max(x => x.MainTextileid);
            var maxPhotodataId = MummyContext.PhotodataTextile.Max(x => x.MainPhotodataid);

            // Create a new instance of the PhotoBurialViewModel and populate it with data from both models
            BurialViewModel viewModel = new BurialViewModel()
            {
                Photodata = MummyContext.PhotodataTextile.Single(x => x.MainPhotodataid == maxPhotodataId),
                Burialmain1 = MummyContext.BurialmainTextile.Single(x => x.MainTextileid == maxTextileId)
            };
            return View("ask");


        }
        //[HttpGet]
        //public IActionResult ask()
        //{
        //    var maxtextileid = MummyContext.Textile.Max(x => x.Id);
        //    var phototextid = MummyContext.PhotodataTextile.Max(x => x.MainPhotodataid);
        //    var maxbId = MummyContext.Burialmain.Max(x => x.Id);
        //    var maxtextileid2 = MummyContext.BurialmainTextile.Max(x => x.MainTextileid);

        //    ViewBag.maxtextileid = Math.Max(maxtextileid, maxtextileid2);
        //    ViewBag.phototextid = phototextid;
        //    ViewBag.maxbId = maxbId;

        //    return View("ask", new PhotodataTextile());
        //}

        //[HttpPost]
        //public IActionResult ask(PhotodataTextile ar, string photoSubmit = null, string textileSubmit = null)
        //{
        //    if (!string.IsNullOrEmpty(photoSubmit))
        //    {
        //        // Perform photo form database operation
        //        var photoid = MummyContext.PhotodataTextile.Max(x => x.MainPhotodataid);
        //        ViewBag.phototextid = photoid;
        //        return View("CreatePhoto", new Photodata());
        //    }
        //    else if (!string.IsNullOrEmpty(textileSubmit))
        //    {
        //        // Perform textile form database operation
        //        var maxtextileid = MummyContext.BurialmainTextile.Max(x => x.MainTextileid);
        //        ViewBag.maxtextileid = maxtextileid;
        //        return View("CreateTextile", new Textile());
        //    }
        //    else
        //    {
        //        // Invalid form submit button
        //        return BadRequest();
        //    }
        //}
        //[HttpGet]
        //public IActionResult Ask()
        //{
        //    var maxBurialmainId = MummyContext.Burialmain.Max(x => x.Id);
        //    var maxTextileId = MummyContext.BurialmainTextile.Max(x => x.MainTextileid);
        //    var maxPhotodataId = MummyContext.PhotodataTextile.Max(x => x.MainPhotodataid);



        //    return View();
        //}
        [HttpGet]
        public IActionResult Ask()
        {

            var maxtextileid = MummyContext.Textile.Max(x => x.Id);
            var phototextid = MummyContext.PhotodataTextile.Max(x => x.MainPhotodataid);
            var maxbId = MummyContext.Burialmain.Max(x => x.Id);
            ViewBag.maxbId = maxbId;
            ViewBag.maxtextileid = maxtextileid;
            ViewBag.phototextid = phototextid + 1;
            

            //return View("askphoto", new PhotodataTextile());

   
            


            return View(new BurialViewModel());
        }
    }
}