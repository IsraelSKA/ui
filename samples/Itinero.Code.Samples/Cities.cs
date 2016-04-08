// ReSharper disable All

namespace Itinero.Code.Samples
{
    public class Rootobject
    {
        public Geoname[] geonames { get; set; }
    }

    public class Geoname
    {
        public float lng { get; set; }
        public int geonameId { get; set; }
        public string countrycode { get; set; }
        public string name { get; set; }
        public string fclName { get; set; }
        public string toponymName { get; set; }
        public string fcodeName { get; set; }
        public string wikipedia { get; set; }
        public float lat { get; set; }
        public string fcl { get; set; }
        public int population { get; set; }
        public string fcode { get; set; }
    }

}