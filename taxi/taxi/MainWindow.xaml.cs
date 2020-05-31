using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using System.Device.Location;

namespace taxi
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<MapObject> objs = new List<MapObject>();
        List<Car> cars = new List<Car>();

        List<PointLatLng> pts = new List<PointLatLng>();

        //Car car = null;
        Human human = null;
        Location loc = null;
        public GMapMarker carMarker = null;
        public GMapMarker humMarker = null;
        public GMapMarker locMarker = null;

        //GMapMarker dst;

        public MainWindow()
        {
            InitializeComponent();
        }


        private void MapLoaded(object sender, RoutedEventArgs e)
        {
            // настройка доступа к данным
            GMaps.Instance.Mode = AccessMode.ServerAndCache;

            // установка провайдера карт
            Map.MapProvider = OpenStreetMapProvider.Instance;

            // установка зума карты
            Map.MinZoom = 2;
            Map.MaxZoom = 17;
            Map.Zoom = 15;
            // установка фокуса карты
            Map.Position = new PointLatLng(55.012823, 82.950359);

            // настройка взаимодействия с картой
            Map.MouseWheelZoomType = MouseWheelZoomType.MousePositionAndCenter;
            Map.CanDragMap = true;
            Map.DragButton = MouseButton.Left;

        }
        private void Map_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (searchDist.IsChecked == true)
            {
                PointLatLng point = Map.FromLocalToLatLng((int)e.GetPosition(Map).X, (int)e.GetPosition(Map).Y);

                //pts.Add(point);

                sorting(objs, point);

                objectList.Items.Clear();

                foreach (MapObject cm in objs)
                {
                    objectList.Items.Add(cm.getTitle() + " " + Math.Round(cm.getDistance(point)).ToString());
                }
            }
            else
            {

                PointLatLng point = Map.FromLocalToLatLng((int)e.GetPosition(Map).X, (int)e.GetPosition(Map).Y);

                pts.Add(point);

                if (objType.SelectedIndex > -1)
                {
                    if (objType.SelectedIndex == 0)
                    {
                        //if (car == null)

                        Car car = new Car(objTitle.Text, point, Map);
                        objs.Add(car);
                        cars.Add(car);
                        carMarker = car.getMarker();
                    }

                    if (objType.SelectedIndex == 1)
                    {
                        if (human == null)
                        {
                            human = new Human(objTitle.Text, point);
                            objs.Add(human);
                            humMarker = human.getMarker();

                            human.setPosition(point);
                            humMarker.Position = point;

                        }

                    }

                    if (objType.SelectedIndex == 2)
                    {
                        if ((human != null) && (loc == null))
                        {
                            human.moveTo(point);


                            loc = new Location(objTitle.Text, point);
                            objs.Add(loc);
                            locMarker = loc.getMarker();

                        }
                    }

                    if (objType.SelectedIndex == 3)
                    {
                        Area a = new Area(objTitle.Text, pts);
                        objs.Add(a);
                        Map.Markers.Add(a.getMarker());
                    }

                    Map.Markers.Clear();

                    foreach (MapObject cm in objs)
                    {
                        Map.Markers.Add(cm.getMarker());
                    }

                    //Map.Markers.Add(dst);

                    pts.Clear();
                }
            }
        }

        public List<MapObject> sorting(List<MapObject> l, PointLatLng p)
        {
            double distance1;
            double distance2;

            for (int n = 0; n < l.Count; n++)
            {
                int min_i = n;

                for (int j = n + 1; j < l.Count; j++)
                {
                    distance1 = l[j].getDistance(p);
                    distance2 = l[min_i].getDistance(p);

                    if (distance2 > distance1)
                    {
                        min_i = j;
                    }
                }

                MapObject t = l[n];
                l[n] = l[min_i];
                l[min_i] = t;
            }

            return l;
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            objectList.Items.Clear();

            int i = 0;
            PointLatLng p = Map.Position;

            foreach (MapObject cm in objs)
            {
                if (searchTitle.Text == cm.getTitle())
                {
                    i += 1;
                    p = cm.getFocus();

                }
            }

            if (i == 1)
            {
                Map.Position = p;

                sorting(objs, p);

                objectList.Items.Clear();

                foreach (MapObject cm in objs)
                {
                    if (cm.getDistance(p) != 0)
                        objectList.Items.Add(cm.getTitle() + " " + Math.Round(cm.getDistance(p)).ToString());
                }
            }

            if (i > 1)
            {
                foreach (MapObject cm in objs)
                {
                    if (searchTitle.Text == cm.getTitle())
                    {
                        objectList.Items.Add(cm.getTitle());
                    }
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (objectList.SelectedIndex > -1)
            {
                PointLatLng p = objs[objectList.SelectedIndex + 1].getFocus();

                sorting(objs, p);

                objectList.Items.Clear();

                foreach (MapObject cm in objs)
                {
                    if (cm.getDistance(p) != 0)
                        objectList.Items.Add(cm.getTitle() + " " + Math.Round(cm.getDistance(p)).ToString());
                }

                Map.Position = p;
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            double distance1;
            double distance2;

            for (int n = 0; n < cars.Count; n++)
            {
                int min_i = n;

                for (int j = n + 1; j < cars.Count; j++)
                {
                    distance1 = cars[j].getDistance(human.getFocus());
                    distance2 = cars[min_i].getDistance(human.getFocus());

                    if (distance2 > distance1)
                    {
                        min_i = j;
                    }
                }

                Car t = cars[n];
                cars[n] = cars[min_i];
                cars[min_i] = t;
            }

            //sorting(cr, h.getFocus());

            cars[0].Arrived += human.CarArrived;
            human.passSeated += cars[0].passSeated;

            Map.Markers.Add(cars[0].moveTo(human.getFocus()));

            cars[0].Follow += Car_Follow;
        }


        private void Car_Follow(object sender, EventArgs e)
        {
            Car car = (Car)sender;
            car.Arrived -= human.CarArrived;

            Map.Position = car.getFocus();
        }

    }

}
