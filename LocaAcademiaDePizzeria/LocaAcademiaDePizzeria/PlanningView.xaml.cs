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
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Services.Maps;
using Windows.UI;
using Windows.Media.Playback;
using static LocaAcademiaDePizzeria.MainMenu;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace LocaAcademiaDePizzeria
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    ///
    public sealed partial class PlanningView : Page
    {
        public ObservableCollection<DriverViewModel> DriverList { get; } = new ObservableCollection<DriverViewModel>();
        public ObservableCollection<AbilityViewModel> AbilityList { get; } = new ObservableCollection<AbilityViewModel>();

        private DriverViewModel selectedDriver = null;

        public int playerMoney = 400;

        public Random rnd = new Random();

        public Geopoint[] requests = new Geopoint[5];

        public Geopoint pizzeriaPosition; //pizzería

        public MapRouteView[] routeViews = new MapRouteView[5];

        public Color[] colors = { Colors.LightGreen, Colors.LightCoral, Colors.LightGray, Colors.Aqua, Colors.White };

        public MediaPlayer mediaPlayer;

        public class PlanningViewParameters
        {
            public MediaPlayer mediaPlayer;
            public Geopoint[] requests;
        }

        public PlanningView()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Carga la lista de ModelView a partir de la lista de Modelo
            if (DriverList != null)
                foreach (Driver driver in DriverModel.GetAllDrivers())
                {
                    DriverViewModel VMitem = new DriverViewModel(driver);
                    DriverList.Add(VMitem);
                }
            // Carga la lista de ModelView a partir de la lista de Modelo
            if (AbilityList != null)
                foreach (Ability ability in AbilityModel.GetAllAbilities())
                {
                    AbilityViewModel VMitem = new AbilityViewModel(ability);
                    AbilityList.Add(VMitem);
                }
            CloseAbilties();
            CloseDrivers();

            //Centrado del mapa sobre la posición de la pizzería
            MainMenuParameters p = e.Parameter as MainMenuParameters;
            pizzeriaPosition = p.selectedLocation;
            mediaPlayer = p.mediaPlayer;
            mapaSoria.Center = pizzeriaPosition;
            mapaSoria.ZoomLevel = 15;

            //Creación de la imagen de la pizzería
            Image image = new Image();
            image.Source = new BitmapImage(new Uri("ms-appx:///Assets/Pizzeria.png"));
            image.Width = 50;
            image.Height = 50;
            mapaSoria.Children.Add(image);
            MapControl.SetLocation(image, pizzeriaPosition);
            MapControl.SetNormalizedAnchorPoint(image, new Point(0.5, 0.5));

            CreateRequests();

            base.OnNavigatedTo(e);
        }

        private async void CreateRequests()
        {
            for (int i = 0; i < 5; i++)
            {
                BasicGeoposition b;
                b.Latitude = mapaSoria.Center.Position.Latitude + rnd.Next(-10000, 10000) / 1000000.0; // números mágicos
                b.Longitude = mapaSoria.Center.Position.Longitude + rnd.Next(-20000, 20000) / 1000000.0; // números mágicos
                b.Altitude = mapaSoria.Center.Position.Altitude;
                requests[i] = new Geopoint(b);

                Image requestImg = new Image();
                requestImg.Source = new BitmapImage(new Uri(this.BaseUri, "/Assets/ManualView/Request.png"));
                Button req = new Button();
                req.Width = 50; req.Height = 70;
                req.Content = requestImg;
                req.Click += Request_Click;
                mapaSoria.Children.Add(req);
                MapControl.SetLocation(req, requests[i]);
                MapControl.SetNormalizedAnchorPoint(req, new Point(0.5, 0.5));

                //creamos las rutas sin mostrarlas
                MapRouteFinderResult routeResult = await MapRouteFinder.GetDrivingRouteAsync(pizzeriaPosition, requests[i],
                    MapRouteOptimization.Distance, MapRouteRestrictions.None);

                //Proceso de mostrar la ruta anterior en el mapa
                if (routeResult.Status == MapRouteFinderStatus.Success)
                {
                    // Inicializamos un MapRouteView
                    MapRouteView viewOfRoute = new MapRouteView(routeResult.Route);
                    viewOfRoute.RouteColor = colors[i];
                    viewOfRoute.OutlineColor = Colors.Black;

                    routeViews[i] = viewOfRoute; //guardamos pero no mostramos
                }
            }
        }

        private void OpenAbilties()
        {
            Button_OpenAbilitiesGrid.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            Grid_expandedAbility.Visibility = Windows.UI.Xaml.Visibility.Visible;
            Grid_AbilityInfo.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private void CloseAbilties()
        {
            Button_OpenAbilitiesGrid.Visibility = Windows.UI.Xaml.Visibility.Visible;
            Grid_expandedAbility.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            Grid_AbilityInfo.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void OpenDrivers()
        {
            Grid_expandedDrivers.Visibility = Windows.UI.Xaml.Visibility.Visible;
            Grid_collapsedDrivers.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            Grid_DriverInfo.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private void CloseDrivers()
        {
            Grid_expandedDrivers.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            Grid_DriverInfo.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            Grid_collapsedDrivers.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private void Button_OpenAbilitiesGrid_Click(object sender, RoutedEventArgs e)
        {
            OpenAbilties();
        }

        private void Button_CloseAbilites_Click(object sender, RoutedEventArgs e)
        {
            CloseAbilties();
        }

        private void Button_CloseDrivers_Click(object sender, RoutedEventArgs e)
        {
            CloseDrivers();
        }

        private void OpenDriversButton_Click(object sender, RoutedEventArgs e)
        {
            OpenDrivers();
        }

        private void PizzaTime_Click(object sender, RoutedEventArgs e)
        {
            PlanningViewParameters p = new PlanningViewParameters();
            p.mediaPlayer = mediaPlayer;
            p.requests = requests;
            this.Frame.Navigate(typeof(ManualView), p);
        }

        private void Request_Click(object sender, RoutedEventArgs e)
        {
            if (selectedDriver != null)
            {
                int i = mapaSoria.Children.IndexOf(e.OriginalSource as DependencyObject);
                i--; //el primer child es la pizzería
                routeViews[i].RouteColor = selectedDriver.routeColor;

                //logica asgiganr pedido
                if (!mapaSoria.Routes.Contains(routeViews[i]))
                {
                    if (selectedDriver.actDeliveries < selectedDriver.maxDeliveries)
                    {
                        selectedDriver.actDeliveries++;
                        mapaSoria.Routes.Add(routeViews[i]);
                    }
                }
                else
                {
                    selectedDriver.actDeliveries--;
                    mapaSoria.Routes.Remove(routeViews[i]);
                }


                selectedDriver.carryBar = (int)(100 * (double)selectedDriver.actDeliveries / (double)selectedDriver.maxDeliveries);
                //Geopoint g = new Geopoint(args.Location.Position);
            }
        }

        private void expandedDrivers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Grid_AbilityInfo.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            Grid_DriverInfo.Visibility = Windows.UI.Xaml.Visibility.Visible;

            selectedDriver = e.AddedItems.First() as DriverViewModel;

            if (selectedDriver != null)
            {
                driverInfoImg.Source = selectedDriver.img.Source;
                driverInfoDescription.Text = selectedDriver.name;
                driverInfoMoneyBar.Value = selectedDriver.moneyRatio();
                driverInfoMoneyTxt.Text = selectedDriver.moneyRatiostring();

                driverInfoSpeedBar.Value = selectedDriver.speedRatio();
                driverInfoSpeedTxt.Text = selectedDriver.speedRatiostring();

                driverInfoCarryBar.Value = selectedDriver.carryRatio();
                driverInfoCarryTxt.Text = selectedDriver.carryRatiostring();

                driverInfoHappinessBar.Value = selectedDriver.happinessRatio();
                driverInfoHappinessTxt.Text = selectedDriver.happinessRatiostring();
            }
        }

        private void expandedAbility_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Grid_AbilityInfo.Visibility = Windows.UI.Xaml.Visibility.Visible;
            Grid_DriverInfo.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            AbilityViewModel selected = e.AddedItems.First() as AbilityViewModel;

            abilityInfoImg.Source = selected.img.Source;
            abilityInfoDescription.Text = selected.description;
        }

        private void AbilityButton_Click(object sender, RoutedEventArgs e)
        {
            AbilityViewModel selected = (AbilityViewModel)(sender as Button).DataContext;
            if (selected.maxLevel > selected.actLevel && selected.upgradeCost <= playerMoney)
            {
                playerMoney -= selected.upgradeCost;
                moneyText.Text = playerMoney.ToString();//no funciona el binding
                selected.actLevel++;

                //el binding de las barras no se actualiza asi que
                Button p = e.OriginalSource as Button;
                ProgressBar levelBar = FindVisualChild<ProgressBar>(p.Parent);
                levelBar.Value = selected.getLevelBar();
            }
        }

        //No podemos usar x:Name porque usamos listas de templates asi que tenemos que buscar
        //https://stackoverflow.com/questions/19409947/how-to-get-the-children-of-a-uielement
        public static T FindVisualChild<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        return (T)child;
                    }

                    T childItem = FindVisualChild<T>(child);
                    if (childItem != null) return childItem;
                }
            }
            return null;
        }

        private void collapsedDrivers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedDriver = e.AddedItems.First() as DriverViewModel;
        }
    }
}