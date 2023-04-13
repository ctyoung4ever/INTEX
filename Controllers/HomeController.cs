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
using System.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;
using LinqKit;

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
        [HttpGet]
        public IActionResult BurialList(int pageNum = 1)
        {
            int pageSize = 50;

            var area = MummyContext.Burialmain.Select(x => x.Area).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToList();
            var burialnumber = MummyContext.Burialmain.Select(x => x.Burialnumber).Distinct().Where(x => !string.IsNullOrEmpty(x)).ToList();
            var depth = MummyContext.Burialmain.Select(x => x.Depth).Distinct().ToList();
            var headdirection = MummyContext.Burialmain.Select(x => x.Headdirection).Distinct().ToList();
            var ageatdeath = MummyContext.Burialmain.Select(x => x.Ageatdeath).Distinct().ToList();
            var length = MummyContext.Burialmain.Select(x => x.Length).Distinct().ToList();
            var sex = MummyContext.Burialmain.Select(x => x.Sex).Distinct().ToList();
            var haircolor = MummyContext.Burialmain.Select(x => x.Haircolor).Distinct().ToList();


            var x = new BurialViewModel
            {

                FilterForm = new FilterForm
                {
                    Burials = MummyContext.Burialmain.ToList(),
                    Areas = area, //= MummyContext.Burialmain.Select(x => x.Area).Distinct()
                    Burialnumbers = burialnumber,
                    Depths = depth,
                    Headdirections = headdirection,
                    Ageatdeaths = ageatdeath,
                    Lengths = length,
                    Sexs = sex,
                    Haircolors = haircolor,

                },

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

        [HttpPost]
        public IActionResult BurialList(IFormCollection form, string area, string burialnumber, string depth, string headdirection, string ageatdeath, string length, string sex, string haircolor, int pageNum = 1)
        {
            //Emma test
            var query = MummyContext.Burialmain.AsEnumerable().AsQueryable();
            var filters = new List<Expression<Func<Burialmain, bool>>>();
            Expression<Func<Burialmain, bool>> combinedFilters = null;

            foreach (string key in form.Keys)
            {
                string value = form[key];
                if (!string.IsNullOrEmpty(value) && value != "" && value != "Select an option")
                {
                    switch (key)
                    {
                        case "area":
                            filters.Add(x => x.Area == value);
                            break;
                        case "burialnumber":
                            filters.Add(x => x.Burialnumber == value);
                            break;
                        case "depth":
                            filters.Add(x => x.Depth == value);
                            break;
                        case "headdirection":
                            filters.Add(x => x.Headdirection == value);
                            break;
                        case "ageatdeath":
                            filters.Add(x => x.Ageatdeath == value);
                            break;
                        case "length":
                            filters.Add(x => x.Length == value);
                            break;
                        case "sex":
                            filters.Add(x => x.Sex == value);
                            break;
                        case "haircolor":
                            filters.Add(x => x.Haircolor == value);
                            break;
                        default:
                            break;
                    }
                }
            }

            if (filters.Count > 0)
            {
                //combinedFilters = filters.Aggregate<Expression<Func<Burialmain, bool>>, Expression<Func<Burialmain, bool>>>(null, (expr1, expr2) => expr1 == null ? expr2 : Expression.Lambda<Func<Burialmain, bool>>(Expression.And(expr1.Body, expr2.Body), expr1.Parameters));

                combinedFilters = filters.Aggregate((expr1, expr2) => expr1.And(expr2));
            }

            if (combinedFilters != null)
            {
                query = query.Where(combinedFilters);
                //Console.Write("Query: " + query);
            }



            ViewBag.test = area;

            //bool filtered = true;
            //bool areaFiltered = false;

            //if((area == null) && (burialnumber == null) && (depth == null) && (headdirection == null) && (ageatdeath == null) && (length == null) && (sex == null) && (haircolor == null))
            //{
            //    filtered = false;
            //}

            int pageSize = 50;

            var areas = MummyContext.Burialmain.Select(x => x.Area).Distinct().ToList();
            var burialnumbers = MummyContext.Burialmain.Select(x => x.Burialnumber).Distinct().ToList();
            var depths = MummyContext.Burialmain.Select(x => x.Depth).Distinct().ToList();
            var headdirections = MummyContext.Burialmain.Select(x => x.Headdirection).Distinct().ToList();
            var ageatdeaths = MummyContext.Burialmain.Select(x => x.Ageatdeath).Distinct().ToList();
            var lengths = MummyContext.Burialmain.Select(x => x.Length).Distinct().ToList();
            var sexs = MummyContext.Burialmain.Select(x => x.Sex).Distinct().ToList();
            var haircolors = MummyContext.Burialmain.Select(x => x.Haircolor).Distinct().ToList();

            //var filteredBurials = MummyContext.Burialmain;

            //if (areaFiltered)
            //{
            //    var afilteredBurials = filteredBurials.Where(x => x.Area == area);
            //}


            var x = new BurialViewModel
            {

                FilterForm = new FilterForm
                {
                    Burials = MummyContext.Burialmain.ToList(),
                    Areas = areas, //= MummyContext.Burialmain.Select(x => x.Area).Distinct()
                    Burialnumbers = burialnumbers,
                    Depths = depths,
                    Headdirections = headdirections,
                    Ageatdeaths = ageatdeaths,
                    Lengths = lengths,
                    Sexs = sexs,
                    Haircolors = haircolors,

                },

                Burials = query
                    .OrderBy(x => x.Id)
                    .Skip((pageNum - 1) * pageSize)
                    .Take(pageSize)
                    .ToList(),

                //Burials = MummyContext.Burialmain.ToList(),
                    //.FromSqlRaw("SELECT * from )

                    //.Include(x => x.BurialmainTextile)
                    //.Where(x => x.Area == area)
                    //.Where(x => x.Burialnumber == burialnumber)
                    //.Where(x => x.Depth == depth)
                    //.Where(x => x.Headdirection == headdirection)
                    //.Where(x => x.Ageatdeath == ageatdeath)
                    //.Where(x => x.Length == length)
                    //.Where(x => x.Sex == sex)
                    //.Where(x => x.Haircolor == haircolor)
                    //.OrderBy(x => x.Id)
                    //.Skip((pageNum - 1) * pageSize)
                    //.Take(pageSize)
                    //.ToList(),

                PageInfo = new PageInfo
                {
                    TotalNumBurials = query.Count(),
                    BurialsPerPage = pageSize,
                    CurrentPage = pageNum
                }
            };

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
            var burialMain = MummyContext.Burialmain.SingleOrDefault(x => x.Id == id);
            var burialMainTextiles = MummyContext.BurialmainTextile.Where(bt => bt.MainBurialmainid == id).ToList();
            var textileIds = burialMainTextiles.Select(bt => bt.MainTextileid);
            var textiles = MummyContext.Textile.Where(t => textileIds.Contains(t.Id)).ToList();
            var bodyanalysis = MummyContext.Bodyanalysischart.SingleOrDefault(x => x.Id == id);
            
            var photoids = MummyContext.PhotodataTextile.Where(t => textileIds.Contains(t.MainTextileid));
            var newids = photoids.Select(bt => bt.MainPhotodataid);
            var photos = MummyContext.Photodata.Where(t => newids.Contains(t.Id)).ToList();
            
            //photodata connects to photodata_textile which connects to textile and burialmain
            var viewModel = new BurialViewModel
            {
                BurialMain = burialMain,
                TextileList = burialMainTextiles,
                Textile = textiles,
                BodyAnalysis = bodyanalysis,
                Photos = photos
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
    }
}
