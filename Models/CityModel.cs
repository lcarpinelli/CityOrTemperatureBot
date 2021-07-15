using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DialogBot.Models
{
    public class CityModel
    {
        public string Final { get; set; }
        public string Name { get; set; }
        public string Request { get; set; }
        public string Choice { get; set; }
        public string City { get; set; }
        public string Temp { get; set; }
        public string Lon { get; set; }
        public string Lat { get; set; }
        public string Id { get; set; }
        public bool IsValid { get; set; }
        public HttpResponseMessage Response { get; set; }

    }
}
