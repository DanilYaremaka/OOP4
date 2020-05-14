using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace poihali
{
    public class Car : MapObject
    {
        private PointLatLng point;
        private Route route;
        private Human pass;
        GMapMarker carMarker;
        Thread newThread;

        // событие прибытия
        public event EventHandler Arrived;
        public event EventHandler Follow;


        public Car(string title, PointLatLng point) : base(title)
        {
            this.point = point;
            pass = null;
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
                    Width = 80, // ширина маркера
                    Height = 80, // высота маркера
                    ToolTip =  "Mater", // всплывающая подсказка
                    Source = new BitmapImage(new Uri("pack://application:,,,/pictures/mater.png")) // картинка
                }
            };

            carMarker = marker;

            return marker;
        }

        public GMapMarker moveTo(PointLatLng dest)
        {
            // провайдер навигации
            RoutingProvider routingProvider = GMapProviders.OpenStreetMap;
            
            // определение маршрута
            MapRoute route = routingProvider.GetRoute(
             point, // начальная точка маршрута
             dest, // конечная точка маршрута
             false, // поиск по шоссе (false - включен)
             false, // режим пешехода (false - выключен)
             (int)15);

            // получение точек маршрута
            List<PointLatLng> routePoints = route.Points;

            this.route = new Route("", routePoints);

            newThread = new Thread(new ThreadStart(MoveByRoute));
            newThread.Start();

            return this.route.getMarker();
        }

        private void MoveByRoute()
        {
            // последовательный перебор точек маршрута
            foreach (var point in route.getPoints())
            {
                // делегат, возвращающий управление в главный поток
                Application.Current.Dispatcher.Invoke(delegate {
                    // изменение позиции маркера
                    carMarker.Position = point;
                    this.point = point;

                    if(pass!=null)
                    {
                        pass.setPosition(point);
                        pass.humanMarker.Position = point;
                        Follow?.Invoke(this, null);
                    }
                });
                // задержка 500 мс
                Thread.Sleep(500);
            }

            // отправка события о прибытии после достижения последней точки маршрута
            //if (pass == null)
            //{
                newThread.Abort();
                Arrived?.Invoke(this, null);
           /* }
            else
            {
                pass = null;
                newThread.Abort();
            }*/
        }

        public void passSeated(object sender, EventArgs e)
        {
            pass = (Human)sender;

            Application.Current.Dispatcher.Invoke(delegate {
                moveTo(pass.getDestination()); 
            } );
        }

    }
}
