using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML.OnnxRuntime.Tensors;



namespace INTEX.Models

{
    public class APIData
    {
        public float squarenorthsouth { get; set; }
        public float depth { get; set; }
        public float southtohead { get; set; }
        public float squareeastwest { get; set; }
        public float westtohead { get; set; }
        public float westtofeet { get; set; }
        public float southtofeet { get; set; }
        public float eastwest_W { get; set; }
        public float wrapping_H { get; set; }
        public float wrapping_W { get; set; }
        public float area_NNW { get; set; }
        public float area_NW { get; set; }
        public float area_SE { get; set; }
        public float area_SW { get; set; }
        public float ageatdeath_C { get; set; }
        public float ageatdeath_I { get; set; }

        public Tensor<float> AsTensor()
        {
            float[] data = new float[]
            {
                squarenorthsouth, depth, southtohead, squareeastwest, westtohead, 
                westtofeet, southtofeet, eastwest_W, wrapping_H, wrapping_W, 
                area_NNW, area_NW, area_SE, area_SW, ageatdeath_C, ageatdeath_I
                };
            int[] dimensions = new int[] { 1, 16 };
            return new DenseTensor<float>(data, dimensions);
        }
    }
}