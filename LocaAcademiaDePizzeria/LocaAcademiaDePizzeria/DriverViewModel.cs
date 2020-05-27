using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace LocaAcademiaDePizzeria
{
    public class DriverViewModel : Driver
    {
        public Image img;

        public double carryCapacityBar()
        {
            return 100 * (double)actDeliveries / (double)maxDeliveries;
        }

        public double carryRatio()
        {
            return 100 * (double)maxDeliveries / (double)MAX_CARRY;
        }

        public double moneyRatio()
        {
            return 100 * (double)money / (double)MAX_MONEY;
        }

        public double happinessRatio()
        {
            return 100 * (double)happiness / (double)MAX_HAPPINESS;
        }

        public double speedRatio()
        {
            return 100 * (double)speed / (double)MAX_SPEED;
        }

        public string speedRatiostring()
        {
            return (speed + " km/h");
        }

        public string happinessRatiostring()
        {
            return (happiness + " :)");
        }

        public string carryRatiostring()
        {
            return (maxDeliveries + " pizzas");
        }

        public string moneyRatiostring()
        {
            return (money + " €/h");
        }

        public DriverViewModel(Driver model)
        {
            img = new Image();
            string s = System.IO.Directory.GetCurrentDirectory() + "\\" + model.imgSource;
            img.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(s));
            img.Width = 50;
            img.Height = 50;
            actDeliveries = model.actDeliveries;
            maxDeliveries = model.maxDeliveries;
            imgSource = model.imgSource;
            name = model.name;
            speed = model.speed;
            happiness = model.happiness;
            money = model.money;
            carryBar = (int)(100 * (double)actDeliveries / (double)maxDeliveries);
            routeColor = model.routeColor;
        }
    }
}