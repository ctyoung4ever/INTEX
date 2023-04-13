using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace INTEX.Models.ViewModels
{
    public class BurialViewModel
    {
        public List<Burialmain> Burials { get; set; }
        public PageInfo PageInfo { get; set; }
        public Burialmain BurialMain { get; internal set; }
        public List<Textile>? Textile { get; internal set; }
        public List<BurialmainTextile> TextileList { get; internal set; }
        public Bodyanalysischart? BodyAnalysis { get; internal set; }
        public List<Photodata>? Photos { get; internal set; }
        public C14 C14 { get; internal set; }
        public PhotodataTextile Photodata { get; set; }
        public BurialmainTextile Burialmain { get; set; }
        public long photoid { get; set; }
        public long burialid { get; set; }
    }
}
