using GMap.NET;
using GMap.NET.WindowsPresentation;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace poihali
{
    public class Human : MapObject
    {
        private PointLatLng point;
        private PointLatLng destination;
        public GMapMarker humanMarker;

        public event EventHandler passSeated;


        public Human(string title, PointLatLng point) : base(title)
        {
            this.point = point;
        }

        public PointLatLng getDestination()
        {
            return destination;
        }

        public PointLatLng getPosition()
        {
            return point;
        }

        public void moveTo(PointLatLng dest)
        {
            destination = dest;
        }

        public void setPosition(PointLatLng point)
        {
            this.point = point;
        }

        public override double getDistance(PointLatLng point)
        {
            throw new NotImplementedException();
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
                    Width = 48, // ширина маркера
                    Height = 48, // высота маркера
                    ToolTip = "Papich", // всплывающая подсказка
                    Source = new BitmapImage(new Uri("pack://application:,,,/pictures/papich.png")) // картинка
                }
            };

            humanMarker = marker;

            return marker;
        }

        // обработчик события прибытия такси
        public void CarArrived(object sender, EventArgs e)
        {
            MessageBox.Show("Все на борт");

            passSeated?.Invoke(this, EventArgs.Empty);
        }

    }
}
