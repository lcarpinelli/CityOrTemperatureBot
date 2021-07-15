using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DialogBot.Models
{
    public class CovidModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Day { get; set; }
        public string Restart { get; set; }
        public int? Deceduti { get; set; }
        public int? Positivi { get; set; }
        public HttpResponseMessage Response { get; set; }
        public bool IsValid { get; set; }
    }
}
