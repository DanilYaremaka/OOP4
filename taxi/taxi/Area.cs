using GMap.NET;
using GMap.NET.WindowsPresentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Device.Location;

namespace taxi
{
    public class Area : MapObject
    {
        private List<PointLatLng> points;

        public Area(string title, List<PointLatLng> points) : base(title)
        {
            this.points = new List<PointLatLng>();

            foreach (PointLatLng p in points)
            {
                this.points.Add(p);
            }
        }

        public override double getDistance(PointLatLng point)
        {
            GeoCoordinate p1 = new GeoCoordinate(point.Lat, point.Lng);
            GeoCoordinate p2 = new GeoCoordinate(this.points[0].Lat, this.points[0].Lng);

            return p1.GetDistanceTo(p2);
        }

        public override PointLatLng getFocus()
        {
            return points[0];
        }

        public override GMapMarker getMarker()
        {
            GMapMarker marker = new GMapPolygon(points)
            {
                Shape = new Path
                {
                    Stroke = Brushes.Black, // стиль обводки
                    Fill = Brushes.Violet, // стиль заливки
                    Opacity = 0.7 // прозрачность
                }
            };

            return marker;
        }
    }
}
