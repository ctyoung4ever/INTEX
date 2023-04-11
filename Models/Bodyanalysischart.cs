using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace INTEX.Models
{
    public partial class Bodyanalysischart
    {
        public long Id { get; set; }
        public string Squarenorthsouth { get; set; }
        public string Northsouth { get; set; }
        public string Squareeastwest { get; set; }
        public string Eastwest { get; set; }
        public string Area { get; set; }
        public int? Burialnumber { get; set; }
        public string Dateofexamination { get; set; }
        public int? Perservationindex { get; set; }
        public string Haircolor { get; set; }
        public string Observations { get; set; }
        public bool? Robust { get; set; }
        public string Supraorbitalridges { get; set; }
        public string Orbitedge { get; set; }
        public string Parietalblossing { get; set; }
        public string Gonion { get; set; }
        public string Nuchalcrest { get; set; }
        public string Zygomaticcrest { get; set; }
        public string Sphenooccipitalsynchondrosis { get; set; }
        public string Lambdoidsuture { get; set; }
        public string Squamossuture { get; set; }
        public int? Toothattrition { get; set; }
        public string Tootheruption { get; set; }
        public string Tootheruptionageestimate { get; set; }
        public bool? Ventralarc { get; set; }
        public string Subpubicangle { get; set; }
        public string Sciaticnotch { get; set; }
        public string Pubicbone { get; set; }
        public string MedicalIpRamus { get; set; }
        public string Femur { get; set; }
        public string Humerus { get; set; }
        public decimal? Femurheaddiameter { get; set; }
        public decimal? Humerusheaddiameter { get; set; }
        public decimal? Femurlength { get; set; }
        public decimal? Humeruslength { get; set; }
        public decimal? Tibia { get; set; }
        public decimal? Estimatestature { get; set; }
        public string Osteophytosis { get; set; }
        public string CariesPeriodontalDisease { get; set; }
        public string Notes { get; set; }
    }
}
