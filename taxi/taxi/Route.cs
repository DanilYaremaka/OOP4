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
    public class Route : MapObject
    {
        private List<PointLatLng> points;

        public Route(string title, List<PointLatLng> points) : base(title)
        {
            this.points = new List<PointLatLng>();
            
            foreach (PointLatLng p in points)
            {
                this.points.Add(p);
            }
        }

        public List<PointLatLng> getPoints()
        {
            return points;
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

            GMapMarker Marker = new GMapRoute(points)
            {
                Shape = new Path()
                {

                    Stroke = Brushes.DarkBlue, // цвет обводки
                    Fill = Brushes.DarkBlue, // цвет заливки
                    StrokeThickness = 4 // толщина обводки
                }
            };

            return Marker;
        }
    }
}
