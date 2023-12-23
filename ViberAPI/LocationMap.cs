using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using static Models.RouteSheet;

namespace ViberAPI
{
    public static class LocationMap
    {
        public static async Task GetLocationAsync(RoutePoint point)
        {
            string result = null;
            try
            {
                var format = new NumberFormatInfo() { NumberDecimalSeparator = "." };
                var uri = $"http://nominatim.openstreetmap.org/reverse?format=json&lat={point.Lat.ToString(format)}&lon={point.Lon.ToString(format)}&layer=address";
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Referer", uri);
                var response = await client.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                var responseStr = await response.Content.ReadAsStringAsync();
                var address = JsonConvert.DeserializeObject<LocarionOpenstreetmap>(responseStr);
                var ad = address.address;
                result = $"{ad.city ?? ad.village ?? ad.state + ", " + ad.district}{(ad.road == null ? "" : ", " + ad.road)}{(ad.house_number == null ? "" : ", " + ad.house_number)}";
            }
            catch (Exception) { }
            if (result != null)
                point.Point = result;
        }
    }

    public class LocarionOpenstreetmap
    {
        public string osm_type { get; set; }
        public string lat { get; set; }
        public string lon { get; set; }
        public string addresstype { get; set; }
        public string name { get; set; }
        public string display_name { get; set; }
        public Address address { get; set; }
    }

    public class Address
    {
        public string house_number { get; set; }
        public string road { get; set; }
        public string village { get; set; }
        public string city { get; set; }
        public string district { get; set; }
        public string state { get; set; }
        public string postcode { get; set; }
    }
}
