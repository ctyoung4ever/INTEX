using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace INTEX.Models.ViewModels
{
    public class AskViewModel
    {
        public long MaxBurialmainId { get; set; }
        public long MaxTextileId { get; set; }
        public long MaxPhotodataId { get; set; }
        public BurialmainTextile BurialmainTextile { get; set; }
        public PhotodataTextile PhotodataTextile { get; set; }
    }

}
