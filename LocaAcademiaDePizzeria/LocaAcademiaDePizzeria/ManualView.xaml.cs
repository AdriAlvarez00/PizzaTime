using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Diagnostics;
using Windows.Media.Playback;
using Windows.Services.Maps;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using static LocaAcademiaDePizzeria.PlanningView;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace LocaAcademiaDePizzeria
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class ManualView : Page
    {

        public ObservableCollection<Ability> abilities { get; } = new ObservableCollection<Ability>();

        public PointerPoint firstPoint = null;

        public int maxJoystickDistance = 60;

        public int timerSpeed = 10;

        public double opacityChange = 0.5;

        public DateTime dateTimer;

        public Geopoint[] requests;

        public MapRouteView[] routeViews;

        public bool[] isRouteVisible;

        public MediaPlayer mediaPlayer;

        public ManualView()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //mapa
            BasicGeoposition soriaPosition;
            soriaPosition.Latitude = 41.764609;
            soriaPosition.Longitude = -2.472443;
            soriaPosition.Altitude = 2000;
            mapaSoria.Center = new Geopoint(soriaPosition);
            mapaSoria.ZoomLevel = 15;

            //timer
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler<object>(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, timerSpeed);
            dispatcherTimer.Start();
            DateTime d = new DateTime();
            d.Subtract(d);
            dateTimer = d.AddMinutes(1170);

            // Carga la lista de ModelView a partir de la lista de Modelo
            if (abilities != null) foreach (Ability ability in AbilityModel.GetAllAbilities()) abilities.Add(ability);

            PlanningViewParameters p = e.Parameter as PlanningViewParameters;
            mediaPlayer = p.mediaPlayer;
            requests = p.requests;
            routeViews = p.routeViews;
            isRouteVisible = p.isRouteVisible;
            CreateBikes();
        }     

        private void CreateBikes()
        {
            List<Color> colors = new List<Color>();

            //create as many bikers as selected halfway to one of their deliveries
            for(int i = 0; i< 5; i++)
            {
                if(isRouteVisible[i] && !colors.Contains(routeViews[i].RouteColor)){
                    colors.Add(routeViews[i].RouteColor);
                    BasicGeoposition bikerPos = routeViews[i].Route.Path.Positions[routeViews[i].Route.Path.Positions.Count / 2];
                    createImage("/Assets/ManualView/Bike.png", new Geopoint(bikerPos));
                }
            }

            for (int i = 0; i< 5; i++)
            {
                //requests
                createImage("/Assets/ManualView/Request.png", requests[i]);

                if(isRouteVisible[i]) mapaSoria.Routes.Add(routeViews[i]);
            }
        }

        private void createImage(string path, Geopoint pos)
        {
            Image Img = new Image();
            Img.Source = new BitmapImage(new Uri(this.BaseUri, path));
            Img.Width = 35;
            Img.Height = 35;
            mapaSoria.Children.Add(Img);
            MapControl.SetLocation(Img, pos);
            MapControl.SetNormalizedAnchorPoint(Img, new Point(0.5, 0.5));
        }

        private void StreetCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            firstPoint = e.GetCurrentPoint(StreetCanvas);
            Canvas.SetLeft(JoystickBorder, firstPoint.Position.X - JoystickBorder.ActualHeight/2);
            Canvas.SetTop(JoystickBorder, firstPoint.Position.Y - JoystickBorder.ActualHeight/2);
            Canvas.SetLeft(Joystick, firstPoint.Position.X - Joystick.ActualWidth/2);
            Canvas.SetTop(Joystick, firstPoint.Position.Y - Joystick.ActualHeight/2);
            JoystickBorder.Visibility = Visibility.Visible;
            Joystick.Visibility = Visibility.Visible;
        }

        private void StreetCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (firstPoint != null)
            {
                PointerPoint newPoint = e.GetCurrentPoint(StreetCanvas);
                double xDiff = newPoint.Position.X - firstPoint.Position.X;
                double yDiff = newPoint.Position.Y - firstPoint.Position.Y;
                if (Math.Pow(xDiff, 2) + Math.Pow(yDiff, 2) < Math.Pow(maxJoystickDistance, 2))
                {
                    Canvas.SetLeft(Joystick, newPoint.Position.X - Joystick.ActualWidth / 2);
                    Canvas.SetTop(Joystick, newPoint.Position.Y - Joystick.ActualHeight / 2);
                }
                else
                {
                    double angle = Math.Atan2(yDiff, xDiff);
                    Canvas.SetLeft(Joystick, firstPoint.Position.X + Math.Cos(angle) * maxJoystickDistance - Joystick.ActualWidth / 2);
                    Canvas.SetTop(Joystick, firstPoint.Position.Y + Math.Sin(angle) * maxJoystickDistance - Joystick.ActualHeight / 2);
                }
            }
        }

        private void StreetCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            JoystickBorder.Visibility = Visibility.Collapsed;
            Joystick.Visibility = Visibility.Collapsed;
            firstPoint = null;
        }  

        private void dispatcherTimer_Tick(object sender, object e)
        {
            dateTimer = dateTimer.AddSeconds(1);
            Timer.Text = dateTimer.Hour.ToString("D2") + ":" + dateTimer.Minute.ToString("D2") + ":" + dateTimer.Second.ToString("D2");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button b = e.OriginalSource as Button;
            Image img = b.Content as Image;
            img.Opacity = opacityChange; 
        }
    }
}
