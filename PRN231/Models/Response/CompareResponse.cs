using Newtonsoft.Json;

namespace PRN231.Models.Response
{
    public class CompareResponse
    {
        [JsonProperty("t")]
        public List<long> Times { get; set; }

        [JsonProperty("c")]
        public List<double> Close { get; set; }

        [JsonProperty("o")]
        public List<double> Open { get; set; }

        [JsonProperty("h")]
        public List<double> Hight { get; set; }

        [JsonProperty("l")]
        public List<double> Low { get; set; }

        [JsonProperty("v")]
        public List<int> Volume { get; set; }
    }
}
