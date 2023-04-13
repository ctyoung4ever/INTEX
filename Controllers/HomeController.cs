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
        public IActionResult DeleteBurialItem(Burialmain viewModel, BurialmainTextile bmt, Textile t,  PhotodataTextile pdt, Photodata pd, Bodyanalysischart bac)
        {
            // Remove all textiles associated with the burial item
            var textiles = MummyContext.BurialmainTextile.Where(t => t.MainBurialmainid == viewModel.Id).ToList();
            foreach (var textile in textiles)
            {
                //remove all photos associated with each textile
                t = MummyContext.Textile.Single(x => x.Id == textile.MainTextileid);
                
                var pdts = MummyContext.PhotodataTextile.Where(x => x.MainTextileid == t.Id).ToList();
                foreach (var item in pdts)
                {
                    pd = MummyContext.Photodata.Single(x => x.Id == item.MainPhotodataid);
                    MummyContext.PhotodataTextile.Remove(item);
                    MummyContext.Photodata.Remove(pd);
                }
                MummyContext.BurialmainTextile.Remove(textile);
                MummyContext.Textile.Remove(t);
                
            }

            // Remove the body analysis chart, if it exists
            bac = MummyContext.Bodyanalysischart.SingleOrDefault(b => b.Id == viewModel.Id);
            if (bac != null)
            {
                MummyContext.Bodyanalysischart.Remove(bac);
            }

            // Remove the burial item
            MummyContext.Burialmain.Remove(viewModel);
            MummyContext.SaveChanges();

            return RedirectToAction("BurialList");
        }



        public IActionResult detailsburialitem(long id)
        {
            var burialMain = MummyContext.Burialmain.SingleOrDefault(x => x.Id == id);
            var burialMainTextiles = MummyContext.BurialmainTextile.Where(bt => bt.MainBurialmainid == id).ToList();
            var textileIds = burialMainTextiles.Select(bt => bt.MainTextileid);
            var textiles = MummyContext.Textile.Where(t => textileIds.Contains(t.Id)).OrderBy(x => x.Id).ToList();
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

        [HttpGet]
        public IActionResult EditBodyAnalysis(long burialmainid)
        {
            var bodyAnalysis = MummyContext.Bodyanalysischart.Single(x => x.Id == burialmainid);
            
            

            return View("EditBodyAnalysis", bodyAnalysis);
        }
        [HttpGet]
        public IActionResult EditPicture(long pictureid)
        {
            var Photo = MummyContext.Photodata.Single(x => x.Id == pictureid);



            return View("EditPicture", Photo);
        }
        [HttpGet]
        public IActionResult EditTextile(long id)
        {
            var Textile = MummyContext.Textile.Single(x => x.Id == id);



            return View("EditTextile", Textile);
        }
        [HttpPost]
        public IActionResult EditBodyAnalysis(Bodyanalysischart viewModel)
        {

            if (ModelState.IsValid)
            {
                MummyContext.Update(viewModel);
                MummyContext.SaveChanges();
                return RedirectToAction("BurialList");
            }
            else
            {
                return View("BurialList");
            }
        }
        [HttpPost]
        public IActionResult EditPicture(Photodata viewModel)
        {

            if (ModelState.IsValid)
            {
                MummyContext.Update(viewModel);
                MummyContext.SaveChanges();
                return RedirectToAction("BurialList");
            }
            else
            {
                return View("BurialList");
            }
        }
        [HttpPost]
        public IActionResult EditTextile(Textile tex)
        {

            if (ModelState.IsValid)
            {
                MummyContext.Textile.Update(tex);
                MummyContext.SaveChanges();
                return RedirectToAction("BurialList");
            }
            else
            {
                return Error();
            }
        }

        public IActionResult CreateBodyAnalysis(long burialmainid)
        {
            var bodyAnalysis = MummyContext.Bodyanalysischart.SingleOrDefault(x => x.Id == burialmainid);

            Bodyanalysischart viewModel = new Bodyanalysischart
            {
                Id = burialmainid
            };
            return View(viewModel);
        }
        [HttpGet]
        public IActionResult DeleteBodyAnalysis(long id)
        {
            var burialItem = MummyContext.Bodyanalysischart.Single(x => x.Id == id);



            return View(burialItem);
        }
        [HttpGet]
        public IActionResult DeletePicture(long id)
        {
            var photoid = MummyContext.Photodata.Single(x => x.Id == id);
            
            

            return View(photoid);
        }
        [HttpGet]
        public IActionResult DeleteTextile(long id)
        {
            var textileid = MummyContext.Textile.Single(x => x.Id == id);



            return View(textileid);
        }
        [HttpPost]
        public IActionResult DeletePicture(Photodata viewModel, PhotodataTextile textile)
        {

            MummyContext.Photodata.Remove(viewModel);
            textile = MummyContext.PhotodataTextile.Single(x => x.MainPhotodataid == viewModel.Id);
            MummyContext.PhotodataTextile.Remove(textile);
            MummyContext.SaveChanges();

            return RedirectToAction("BurialList");
        }
        //[HttpPost]
        //public IActionResult DeleteTextile(Textile viewModel, BurialmainTextile bmt, PhotodataTextile pdt, Photodata pd)
        //{
        //    MummyContext.Textile.Remove(viewModel);
        //    bmt = MummyContext.BurialmainTextile.Single(x => x.MainTextileid == viewModel.Id);
        //    pdt = MummyContext.PhotodataTextile.Single(x => x.MainTextileid == viewModel.Id);
        //    pd = MummyContext.Photodata.Single(x => x.Id == pdt.MainPhotodataid);
        //    MummyContext.BurialmainTextile.Remove(bmt);
        //    MummyContext.SaveChanges();

        //    return RedirectToAction("BurialList");
        //}
        [HttpPost]
        public IActionResult DeleteTextile(Textile viewModel, BurialmainTextile bmt, PhotodataTextile pdt, Photodata pd)
        {
            bmt = MummyContext.BurialmainTextile.Single(x => x.MainTextileid == viewModel.Id);
            var pdts = MummyContext.PhotodataTextile.Where(x => x.MainTextileid == viewModel.Id).ToList();
            foreach (var item in pdts)
            {
                pd = MummyContext.Photodata.Single(x => x.Id == item.MainPhotodataid);
                MummyContext.PhotodataTextile.Remove(item);
                MummyContext.Photodata.Remove(pd);
            }
            MummyContext.BurialmainTextile.Remove(bmt);
            MummyContext.Textile.Remove(viewModel);
            MummyContext.SaveChanges();

            return RedirectToAction("BurialList");
        }


        [HttpPost]
        public IActionResult CreateBodyAnalysis(Bodyanalysischart ar)
        {


            MummyContext.Add(ar);
            MummyContext.SaveChanges();
            
            return RedirectToAction("BurialList");

        }

        [HttpPost]
        public IActionResult DeleteBodyAnalysis(Bodyanalysischart viewModel)
        {

            MummyContext.Bodyanalysischart.Remove(viewModel);
            MummyContext.SaveChanges();

            return RedirectToAction("BurialList");
        }


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
        public IActionResult asktextile(BurialViewModel bvm)
        {

            if (bvm.burialid == 0)
            {
                var maxtextileid = MummyContext.BurialmainTextile.Max(x => x.MainTextileid);
                var maxbId = MummyContext.Burialmain.Max(x => x.Id);
                var maxtexxxid = MummyContext.Textile.Max(x => x.Textileid);
                ViewBag.maxtexxxid = maxtexxxid + 1;
                ViewBag.maxbId = maxbId;
                ViewBag.maxtextileid = maxtextileid + 1;
            }
            if (bvm.burialid != 0)
            {
                ViewBag.maxbId = bvm.burialid;
                ViewBag.maxbId = bvm.MaxTextileId;
                var maxtexxxid = MummyContext.Textile.Max(x => x.Textileid);
                ViewBag.maxtexxxid = maxtexxxid +1;
            }
                return View("asktextile", new BurialmainTextile());
        }



        [HttpPost]
        public IActionResult asktextile(BurialmainTextile ar, BurialViewModel bvm)
        {
            if (ar.MainTextileid == 0)
            {
                
                ar.MainBurialmainid = bvm.burialid;
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
            var maxtexxxid = MummyContext.Textile.Max(x => x.Textileid);
            ViewBag.maxtexxxid = maxtexxxid + 1;
            ViewBag.maxtextileid = maxtextileid;
            ViewBag.maxbId = maxbId;
            return View("CreateTextile", new Textile());
        }

        [HttpGet]
        public IActionResult CreateTextile()
        {

            var maxtextileid = MummyContext.BurialmainTextile.Max(x => x.MainTextileid);
            var maxtexxxid = MummyContext.Textile.Max(x => x.Textileid);
            ViewBag.maxtexxxid = maxtexxxid;
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

            // Create a new tuple containing data from both models
            var viewModelTuple = (
                MummyContext.BurialmainTextile.Single(x => x.MainTextileid == maxTextileId),
                MummyContext.PhotodataTextile.Single(x => x.MainPhotodataid == maxPhotodataId)
            );

            // Pass the tuple to the ask view
            return View("ask", viewModelTuple);
        }


        //[HttpPost]
        //public IActionResult CreatePhoto(Photodata ar)
        //{
        //    MummyContext.Add(ar);
        //    MummyContext.SaveChanges();
        //    var maxtextileid = MummyContext.BurialmainTextile.Max(x => x.MainTextileid);
        //    var phototextid = MummyContext.PhotodataTextile.Max(x => x.MainPhotodataid);
        //    ViewBag.maxtextileid = maxtextileid;
        //    ViewBag.phototextid = phototextid + 1;



        //    var maxBurialmainId = MummyContext.Burialmain.Max(x => x.Id);
        //    var maxTextileId = MummyContext.BurialmainTextile.Max(x => x.MainTextileid);
        //    var maxPhotodataId = MummyContext.PhotodataTextile.Max(x => x.MainPhotodataid);

        //    // Create a new instance of the PhotoBurialViewModel and populate it with data from both models
        //    BurialViewModel viewModel = new BurialViewModel()
        //    {
        //        Photodata = MummyContext.PhotodataTextile.Single(x => x.MainPhotodataid == maxPhotodataId),
        //        Burialmain = MummyContext.BurialmainTextile.Single(x => x.MainTextileid == maxTextileId),
        //        burialid = maxBurialmainId
        //    };
        //    return View("ask", viewModel);


    
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
            var maxbId = MummyContext.BurialmainTextile.Max(x => x.MainBurialmainid);
            ViewBag.maxbId = maxbId;
            ViewBag.maxtextileid = maxtextileid;
            ViewBag.phototextid = phototextid + 1;


            //return View("askphoto", new PhotodataTextile());




            var viewModel = new BurialmainTextile();
            var photoDataTextile = new PhotodataTextile();
            return View((viewModel, photoDataTextile));
        }
        
        [HttpPost]
        public IActionResult Ask(string formType, BurialmainTextile Burialmain, PhotodataTextile Photodata)
        {
            if (formType == "textile")
            {
                if (ModelState.IsValid)
                {
                    Burialmain.MainBurialmainid = MummyContext.BurialmainTextile.Max(x => x.MainBurialmainid);
                    Burialmain.MainTextileid = (MummyContext.BurialmainTextile.Max(x => x.MainTextileid) + 1);
                    MummyContext.BurialmainTextile.Add(Burialmain);
                    MummyContext.SaveChanges();
                    var photoid = MummyContext.BurialmainTextile.Max(x => x.MainTextileid);
                    var maxtexxxid = MummyContext.Textile.Max(x => x.Textileid);
                    ViewBag.maxtexxxid = maxtexxxid + 1;
                    ViewBag.maxtextileid = photoid;

                    return View("CreateTextile", new Textile());
                }
                else
                {
                    return View("Ask", (Burialmain, Photodata));
                }
            }
            else if (formType == "photo")
            {
                if (ModelState.IsValid)
                {
                    Photodata.MainTextileid = MummyContext.PhotodataTextile.Max(x => x.MainTextileid);
                    Photodata.MainPhotodataid = (MummyContext.PhotodataTextile.Max(x=> x.MainPhotodataid)+ 1);
                    MummyContext.PhotodataTextile.Add(Photodata);
                    MummyContext.SaveChanges();
                    var photoid = MummyContext.PhotodataTextile.Max(x => x.MainPhotodataid);

                    ViewBag.phototextid = photoid;
                    return View("CreatePhoto", new Photodata());
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return BadRequest();
            }
        }

        //public IActionResult Ask(string formType, BurialmainTextile viewModel, PhotodataTextile photoDataTextile)
        //{
        //    switch (formType)
        //    {
        //        case "textile":
        //            // Handle submit for the textile form
        //            // Example:
        //            MummyContext.BurialmainTextile.Add(viewModel);
        //            MummyContext.SaveChanges();
        //            return RedirectToAction("CreateTextile", new Textile());

        //        case "photo":
        //            // Handle submit for the photo form
        //            // Example:
        //            MummyContext.PhotodataTextile.Update(photoDataTextile);
        //            MummyContext.SaveChanges();
        //            return RedirectToAction("CreatePhoto", new PhotodataTextile());

        //        default:
        //            // Invalid form type
        //            return BadRequest();
        //    }
        //}

    }
}