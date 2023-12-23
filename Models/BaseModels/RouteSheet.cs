using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class RouteSheet
    {
        public RouteSheet(string id)
        {
            Id = id;
            FromPoint = null;
            MediatePoint = new List<RoutePoint>();
            ToPoint = null;
            Distance = -1;
            Passenger = null;
            Remark = null;
        }

        public string Id { get; set; }
        public RoutePoint FromPoint { get; set; }
        public List<RoutePoint> MediatePoint { get; set; }
        public RoutePoint ToPoint { get; set; }
        public int Distance { get; set; }
        public int PassengerCode { get; set; }
        public string Passenger { get; set; }
        public string Remark { get; set; }

        public RouteState GetState()
        {
            if (FromPoint == null) return RouteState.FromPoint;
            if (ToPoint == null) return RouteState.NextPoint;
            if (Distance == -1) return RouteState.Distance;
            if (Passenger == null) return RouteState.Passenger;
            if (Remark == null) return RouteState.Remark;
            return RouteState.Ended;
        }

        public class RoutePoint
        {
            public string Point { get; set; }
            public Double Lat { get; set; }
            public Double Lon { get; set; }
            public DateTime Time { get; set; }
        }

        public enum RouteState
        {
            FromPoint,
            NextPoint,
            Distance,
            Passenger,
            Remark,
            Ended
        }
    }
}
