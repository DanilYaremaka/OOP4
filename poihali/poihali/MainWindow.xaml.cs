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
using System.Threading;

namespace poihali
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<MapObject> objs = new List<MapObject>();

        Human human = null;
        Car car = null;
        public GMapMarker carMarker = null;
        public GMapMarker humanMarker = null;

        GMapMarker dst;



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
            PointLatLng point = Map.FromLocalToLatLng((int)e.GetPosition(Map).X, (int)e.GetPosition(Map).Y);

            if (objType.SelectedIndex > -1)
            {
                if (objType.SelectedIndex == 0)
                {
                    if (car == null)
                    {
                        car = new Car("Mater", point);
                        objs.Add(car);
                        carMarker = car.getMarker();

                        if (human != null)
                        {
                            car.Arrived += human.CarArrived;
                            human.passSeated += car.passSeated;
                        }

                    }
                    else
                    {
                        car.setPosition(point);
                        carMarker.Position = point;
                    }
                }
                if (objType.SelectedIndex == 1)
                {
                    if (human == null)
                    {
                        human = new Human("Papich", point);
                        objs.Add(human);
                        humanMarker = human.getMarker();

                        if (car!=null)
                        {
                            car.Arrived += human.CarArrived;
                            human.passSeated += car.passSeated;
                        }
                    }
                    else
                    {
                        human.setPosition(point);
                        humanMarker.Position = point;
                    }
                }
                if (objType.SelectedIndex == 2)
                {
                    if (human != null)
                    {
                        human.moveTo(point);

                        dst = new GMapMarker(point)
                        {
                            Shape = new Image
                            {
                                Width = 48, // ширина маркера
                                Height = 48, // высота маркера
                                ToolTip = "pivo", // всплывающая подсказка
                                Source = new BitmapImage(new Uri("pack://application:,,,/pictures/pivo.png")) // картинка
                            }
                        };
                    }
                }

                Map.Markers.Clear();

                foreach (MapObject cm in objs)
                {
                    Map.Markers.Add(cm.getMarker());
                }
                Map.Markers.Add(dst);
            }
        }

        private void Go_Click(object sender, RoutedEventArgs e)
        {
            Map.Markers.Add(car.moveTo(human.getFocus()));
            car.Follow += Car_Follow;
        }

        private void Car_Follow(object sender, EventArgs e)
        {
            Car car = (Car)sender;
            car.Arrived -= human.CarArrived;

            Map.Position = car.getFocus();
        }
    } 
}
