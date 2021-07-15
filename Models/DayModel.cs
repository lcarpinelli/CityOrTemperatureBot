using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DialogBot.Models
{
    public class DayModel
    {
            [JsonProperty("id")]
            public int? Id { get; set; }
            [JsonProperty("data")]
            public DateTime? Date { get; set; }
            [JsonProperty("stato")]
            public string? CountryName { get; set; }
            [JsonProperty("ricoverati_con_sintomi")]
            public int? RicoveratiConSintomi { get; set; }
            [JsonProperty("terapia_intensiva")]
            public int? IntensiveCare { get; set; }
            [JsonProperty("totale_ospedalizzati")]
            public int? TotaleOspedalizzati { get; set; }
            [JsonProperty("isolamento_domiciliare")]
            public int? IsolamentoDomiciliare { get; set; }
            [JsonProperty("totale_positivi")]
            public int? TotalPositives { get; set; }
            [JsonProperty("variazione_totale_positivi")]
            public int? VariazioneTotalePositivi { get; set; }
            [JsonProperty("nuovi_positivi")]
            public int? NewPositives { get; set; }
            [JsonProperty("dimessi_guariti")]
            public int? DimessiGuariti { get; set; }
            [JsonProperty("deceduti")]
            public int? Deaths { get; set; }
            [JsonProperty("totale_casi")]
            public int? TotalCases { get; set; }
            [JsonProperty("tamponi")]
            public int? Tests { get; set; }
            [JsonProperty("casi_testati")]
            public double? CasesTested { get; set; }
        
    }
}
