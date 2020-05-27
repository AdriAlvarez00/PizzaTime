using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace LocaAcademiaDePizzeria
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class TitleScreen : Page
    {

        public MediaPlayer mediaPlayer;

        public TitleScreen()
        {
            this.InitializeComponent();

            //Sound
            ElementSoundPlayer.State = ElementSoundPlayerState.On;

            mediaPlayer = new MediaPlayer();
            mediaPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/Sounds/Italian_Spirit.mp3"));
            mediaPlayer.Volume = 0.5;
            mediaPlayer.Play();
        }

        private void NewGameClickEvent(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainMenu), e);
        }

        private void NewGamePointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Button button = sender as Button;

            Image img = new Image();
            img.Source = new BitmapImage(new Uri(this.BaseUri, "/Assets/NewGameSelected.png"));

            button.Content = img;
        }

        private void NewGamePointerExited(object sender, PointerRoutedEventArgs e)
        {
            Button button = sender as Button;

            Image img = new Image();
            img.Source = new BitmapImage(new Uri(this.BaseUri, "/Assets/NewGameButtonUnselected.png"));

            button.Content = img;
        }

        private void LoadGamePointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Button button = sender as Button;

            Image img = new Image();
            img.Source = new BitmapImage(new Uri(this.BaseUri, "/Assets/LoadGameSelected.png"));

            button.Content = img;
        }

        private void LoadGamePointerExited(object sender, PointerRoutedEventArgs e)
        {
            Button button = sender as Button;

            Image img = new Image();
            img.Source = new BitmapImage(new Uri(this.BaseUri, "/Assets/LoadGameUnselected.png"));

            button.Content = img;
        }

        private void ExitGamePointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Button button = sender as Button;

            Image img = new Image();
            img.Source = new BitmapImage(new Uri(this.BaseUri, "/Assets/ExitGameSelected.png"));

            button.Content = img;
        }

        private void ExitGamePointerExited(object sender, PointerRoutedEventArgs e)
        {
            Button button = sender as Button;

            Image img = new Image();
            img.Source = new BitmapImage(new Uri(this.BaseUri, "/Assets/ExitGameUnselected.png"));

            button.Content = img;
        }

        private void ExitGameClickEvent(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }
    }
}
