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
using System.Numerics;
using System.Windows.Media;

namespace taxi
{
    public class Car : MapObject
    {
        private PointLatLng point;
        private Route route;
        private Human pass;


        GMapMarker carMarker;
        GMapControl gMap = null;

        List<PointLatLng> epoints = new List<PointLatLng>();

        // событие прибытия
        public event EventHandler Arrived;
        public event EventHandler Follow;

        public Car(string title, PointLatLng point, GMapControl map) : base(title)
        {
            this.point = point;
            pass = null;

            gMap = map;
        }

        public void setPosition(PointLatLng point)
        {
            this.point = point;
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
                    Width = 64, // ширина маркера
                    Height = 64, // высота маркера
                    ToolTip = this.getTitle(), // всплывающая подсказка
                    Margin = new System.Windows.Thickness(-20,-20,0,0),
                    Source = new BitmapImage(new Uri("pack://application:,,,/pictures/bort.png")) // картинка
                }
            };

            marker.ZIndex = 100;
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

            double dist = 0;
            double minDist = 0.00001;

            List<PointLatLng> points = this.route.getPoints();
            epoints.Clear();
            

            for (int i = 0; i < points.Count-1; i++)
            {
                dist = Vector2.Distance(new Vector2((float)points[i].Lat, (float)points[i].Lng), new Vector2((float)points[i+1].Lat, (float)points[i+1].Lng));
                if (dist > minDist)
                {
                    double aPoints = dist / minDist;
                    for (int j = 0; j < aPoints; j++)
                    {
                        Vector2 t = Vector2.Lerp(new Vector2((float)points[i].Lat, (float)points[i].Lng), new Vector2((float)points[i + 1].Lat, (float)points[i + 1].Lng), (float)(j / aPoints));

                        epoints.Add(new PointLatLng(t.X, t.Y));
                    }
                }
            }
       
            Thread newThread = new Thread(new ThreadStart(MoveByRoute));
            newThread.Start();
           
            return this.route.getMarker();
        }

        private void MoveByRoute()
        {
            double cAngle = 0;
            // последовательный перебор точек маршрута
            for(int i =0; i < epoints.Count; i++)
            {
                var point = epoints[i];
                // делегат, возвращающий управление в главный поток
                Application.Current.Dispatcher.Invoke(delegate {

                    if (i < epoints.Count - 10)
                    {
                        var nextPoint = epoints[i + 10];

                        double latDiff = nextPoint.Lat - point.Lat;
                        double lngDiff = nextPoint.Lng - point.Lng;
                        // вычисление угла направления движения
                        // latDiff и lngDiff - катеты прямоугольного треугольника
                        double angle = Math.Atan2(lngDiff, latDiff) * 180.0 / Math.PI;

                        // установка угла поворота маркера

                        if (Math.Abs(angle - cAngle) > 7) //|| (a - angle < 0))
                        {
                            cAngle = angle;
                            carMarker.Shape.RenderTransform = new RotateTransform( angle,20,20 );
                        }
                    }
                    // изменение позиции маркера
                    carMarker.Position = point;
                    this.point = point;

                    if (pass != null)
                    {
                        pass.setPosition(point); 
                        pass.humMarker.Position = point;
                        pass.humMarker.Shape.RenderTransform = new RotateTransform (cAngle,20,20);
                        Follow?.Invoke(this, null);

                    }
                });
                // задержка 5 мс
                Thread.Sleep(5);
            }

            // отправка события о прибытии после достижения последней точки маршрута
            if (pass == null)
            {
                Arrived?.Invoke(this, null);
            }
            else
            {
                MessageBox.Show("Ну всё, выходи!");
                pass = null;
            }
        }

        public void passSeated(object sender, EventArgs e)
        {
            MessageBox.Show("Все на борт!");

            pass = (Human)sender;

            Application.Current.Dispatcher.Invoke(delegate {
                gMap.Markers.Add(moveTo(pass.getDestination()));
                //moveTo(pass.getDestination());
            }
            );
        }
    }
}
