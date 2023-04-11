using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace INTEX.Models
{
    public partial class C14
    {
        public long Id { get; set; }
        public int? Sample { get; set; }
        public string Squarenorthsouth { get; set; }
        public string Northsouth { get; set; }
        public string Squareeastwest { get; set; }
        public string Eastwest { get; set; }
        public string Area { get; set; }
        public int? Burialnumber { get; set; }
        public string Description { get; set; }
        public int? Agebp { get; set; }
        public string CalenderdateBcAd { get; set; }
        public int? CalendardateStart { get; set; }
        public int? CalendardateEnd { get; set; }
    }
}
