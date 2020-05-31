using GMap.NET;
using GMap.NET.WindowsPresentation;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;


namespace taxi
{
    public class Human : MapObject
    {
        private PointLatLng point;
        private PointLatLng destination;
        public GMapMarker humMarker;

        public event EventHandler passSeated;

        public Human(string title, PointLatLng point) : base(title)
        {
            this.point = point;
        }

        public void setPosition(PointLatLng point)
        {
            this.point = point;
        }

        public PointLatLng getDestination()
        {
            return destination;
        }

        public void moveTo(PointLatLng dest)
        {
            destination = dest;
        }

        public override double getDistance(PointLatLng point)
        {
            GeoCoordinate p1 = new GeoCoordinate(point.Lat, point.Lng);
            GeoCoordinate p2 = new GeoCoordinate(this.point.Lat, this.point.Lng);

            return p1.GetDistanceTo(p2);
        }

        public override PointLatLng getFocus()
        {
            return point;
        }

        public override GMapMarker getMarker()
        {
            GMapMarker marker = new GMapMarker(point)
            {
                Shape = new Image
                {
                    Width = 32, // ширина маркера
                    Height = 32, // высота маркера
                    Margin = new System.Windows.Thickness(-20, -20, 0, 0),
                    ToolTip = this.getTitle(), // всплывающая подсказка
                    Source = new BitmapImage(new Uri("pack://application:,,,/pictures/papich.png")) // картинка
                }
            };

            humMarker = marker;

            return marker;
        }

        public void CarArrived(object sender, EventArgs e)
        {
            passSeated?.Invoke(this, EventArgs.Empty);
        }
    }
}
