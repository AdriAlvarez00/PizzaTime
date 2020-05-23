using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI;

namespace LocaAcademiaDePizzeria
{
    public class Driver : INotifyPropertyChanged
    {
        public int maxDeliveries { get; set; }

        public int actDeliveries { get; set; }

        public string name { get; set; }

        public string imgSource { get; set; }

        public int money { get; set; }
        public int speed { get; set; }
        public int happiness { get; set; }

        private int _carryBar;

        public Color routeColor;

        public int carryBar
        {
            get { return _carryBar; }
            set
            {
                _carryBar = value;
                NotifyPropertyChanged("carryBar");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        public const int MAX_MONEY = 20;
        public const int MAX_SPEED = 55;
        public const int MAX_CARRY = 10;
        public const int MAX_HAPPINESS = 150;

        public Driver()
        {
        }
    }

    public class DriverModel
    {
        public static List<Driver> DriverList = new List<Driver>()
        {
            new Driver()
            {
                maxDeliveries=4,
                actDeliveries = 1,
                name = "Vegan Jorge",
                imgSource = "Assets//Samples//8.jpg",
                money=9,
                speed = 30,
                happiness = 100,
                routeColor = Colors.IndianRed,
            },
            new Driver()
            {
                maxDeliveries=6,
                actDeliveries = 4,
                name = "Influencer",
                imgSource = "Assets//Samples//17.jpg",
                money=6,
                speed = 20,
                happiness = 80,
                routeColor = Colors.BlueViolet,
            },
            new Driver()
            {
                maxDeliveries=12,
                actDeliveries = 0,
                name = "El manitas",
                imgSource = "Assets//Samples//7.jpg",
                money=7,
                speed = 40,
                happiness = 135,
                routeColor = Colors.GreenYellow,
            }
        };

        public static IList<Driver> GetAllDrivers()
        {
            return DriverList;
        }
    }
}