using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls.Maps;
using System.Data;
using Windows.UI.Xaml.Automation;
using Windows.Media.Playback;
using Windows.Media.Core;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace LocaAcademiaDePizzeria
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class MainMenu : Page
    {
        public Button selectedButton = null;

        public bool isPizzeriaSelected = false;

        public Geopoint selectedLocation;

        public MediaPlayer mediaPlayer;

        public class MainMenuParameters
        {
            public MediaPlayer mediaPlayer;
            public Geopoint selectedLocation;
        }

        public MainMenu()
        {
            this.InitializeComponent();

            CreatePizzerias();

            BasicGeoposition soriaPosition;
            soriaPosition.Latitude = 41.764609;
            soriaPosition.Longitude = -2.472443;
            soriaPosition.Altitude = 2000;

            mapaSoria.Center = new Geopoint(soriaPosition);
            mapaSoria.ZoomLevel = 15;

            //Sound
            ElementSoundPlayer.State = ElementSoundPlayerState.On;

            mediaPlayer = new MediaPlayer();
        }

        private void CreatePizzerias()
        {
            BasicGeoposition pizzaPos1; pizzaPos1.Latitude = 41.765633; pizzaPos1.Longitude = -2.471333; pizzaPos1.Altitude = 1050;
            BasicGeoposition pizzaPos2; pizzaPos2.Latitude = 41.769806; pizzaPos2.Longitude = -2.474726; pizzaPos2.Altitude = 1050;
            BasicGeoposition pizzaPos3; pizzaPos3.Latitude = 41.761557; pizzaPos3.Longitude = -2.468557; pizzaPos3.Altitude = 1050;
            BasicGeoposition pizzaPos4; pizzaPos4.Latitude = 41.760440; pizzaPos4.Longitude = -2.474464; pizzaPos4.Altitude = 1050;
            BasicGeoposition pizzaPos5; pizzaPos5.Latitude = 41.769015; pizzaPos5.Longitude = -2.466636; pizzaPos5.Altitude = 1050;


            Geopoint[] pizzeriaPositions = new Geopoint[5]{ new Geopoint(pizzaPos1), new Geopoint(pizzaPos2),
                new Geopoint(pizzaPos3), new Geopoint(pizzaPos4), new Geopoint(pizzaPos5) };

            foreach (Geopoint pos in pizzeriaPositions)
            {
                Button piz = new Button();
                Image pizzeriaIMG = new Image();
                pizzeriaIMG.Source = new BitmapImage(new Uri(this.BaseUri, "/Assets/PizzeriaUnselected.png"));
                piz.Style = (Style)Application.Current.Resources["InvisibleButtonStyle"];
                piz.Width = 50; piz.Height = 70; /*piz.BorderThickness = (Thickness)0;*/
                piz.Content = pizzeriaIMG;
                piz.Click += OnPizzeriaSelected;
                mapaSoria.Children.Add(piz);
                MapControl.SetLocation(piz, pos);
                MapControl.SetNormalizedAnchorPoint(piz, new Point(0.5, 0.5));
            }
        }

        private void OnButtonClicked(object sender, RoutedEventArgs e)
        {
            if (isPizzeriaSelected)
            {
                MainMenuParameters p = new MainMenuParameters();
                p.mediaPlayer = mediaPlayer;
                p.selectedLocation = selectedLocation;
                this.Frame.Navigate(typeof(PlanningView), p);
            }
        }

        private void OnPizzeriaSelected(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != selectedButton)
            {
                if (!isPizzeriaSelected) activatePizzaTimeButton();

                button.Style = (Style)Application.Current.Resources["InvisibleButtonStyle"];
                button.Width = 50; button.Height = 70; /*piz.BorderThickness = (Thickness)0;*/

                Image img = new Image();
                img.Source = new BitmapImage(new Uri(this.BaseUri, "/Assets/PizzeriaSelected.png"));
                img.Stretch = Stretch.Uniform;

                button.Content = img;

                if (selectedButton != null)
                {
                    img = new Image();
                    img.Source = new BitmapImage(new Uri(this.BaseUri, "/Assets/PizzeriaUnselected.png"));
                    img.Stretch = Stretch.Uniform;

                    selectedButton.Content = img;
                }

                selectedButton = button;

                selectedLocation = MapControl.GetLocation(button);
            }
        }

        private void activatePizzaTimeButton()
        {
            isPizzeriaSelected = true;
            Image buttonImg = PizzaTimeButton.Content as Image;
            buttonImg.Source = new BitmapImage(new Uri(this.BaseUri, "/Assets/MainMenuButtonActive.png"));
        }
    }
}
