using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using INTEX.Models;
using Microsoft.ML.Transforms.Onnx;
using Microsoft.ML.Data;


namespace INTEX.Controllers
{
    [ApiController]
    [Route("/score")]
    public class APIController : ControllerBase
    {
        private InferenceSession _session;

        public APIController(InferenceSession session)
        {
            _session = session;
        }

        [HttpPost]
        public ActionResult Score(APIData data)
        {
            var result = _session.Run(new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("float_input", data.AsTensor())
            });


            Tensor<string> output_label = result.First().AsTensor<string>();
            var prediction = new Prediction { PredictedValue = output_label.First() };
            result.Dispose();
            return Ok(prediction);


            //Tensor<string> score = result.First().AsTensor<string>();
            //var prediction = new Prediction { PredictedValue = score.First() };
            //result.Dispose();
            //return Ok(prediction);
        }
    }
}






//using (var enumerator = result.GetEnumerator())
//{
//    if (enumerator.MoveNext())
//    {
//        var firstValue = enumerator.Current;
//        var firstValueData = firstValue.GetValue<float[]>();
//        // Use the first value data here
//    }
//}