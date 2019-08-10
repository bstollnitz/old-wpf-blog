using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace TemplatingItems
{
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }
    }

    public class PlaceTemplateSelector : DataTemplateSelector
    {
        private DataTemplate washingtonTemplate;

        public DataTemplate WashingtonTemplate
        {
            get { return washingtonTemplate; }
            set { washingtonTemplate = value; }
        }

        private DataTemplate notWashingtonTemplate;

        public DataTemplate NotWashingtonTemplate
        {
            get { return notWashingtonTemplate; }
            set { notWashingtonTemplate = value; }
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            Place place = (Place)item;
            
            if (place.State == "WA")
            {
                return washingtonTemplate;
            }
            else
            {
                return notWashingtonTemplate;
            }
        }
    }

    public class GreekGodsAndHeros : ObservableCollection<object>
    {
        public GreekGodsAndHeros()
        {
            Add(new GreekGod("Aphrodite", "Goddess of love, beauty and fertility"));
            Add(new GreekGod("Apollo", "God of prophesy, music and healing"));
            Add(new GreekGod("Ares", "God of war"));
            Add(new GreekGod("Artemis", "Virgin goddess of the hunt"));
            Add(new GreekGod("Athena", "Goddess of crafts and the domestic arts"));
            Add(new GreekHero("Bellerophon"));
            Add(new GreekGod("Demeter", "Goddess of agriculture"));
            Add(new GreekGod("Dionysus", "God of wine"));
            Add(new GreekGod("Hephaestus", "God of fire and crafts"));
            Add(new GreekGod("Hera", "Goddess of marriage"));
            Add(new GreekHero("Hercules"));
            Add(new GreekGod("Hermes", "Messenger of the gods and guide of dead souls to the Underworld"));
            Add(new GreekHero("Jason"));
            Add(new GreekHero("Odysseus"));
            Add(new GreekHero("Perseus"));
            Add(new GreekGod("Poseidon", "God of the sea, earthquakes and horses"));
            Add(new GreekHero("Theseus"));
            Add(new GreekGod("Zeus", "The supreme god of the Olympians"));
        }
    }

    public class GreekGod
    {
        private string godName;

        public string GodName
        {
            get { return godName; }
            set { godName = value; }
        }

        private string godDescription;

        public string GodDescription
        {
            get { return godDescription; }
            set { godDescription = value; }
        }

        public GreekGod(string godName, string godDescription)
        {
            this.GodName = godName;
            this.GodDescription = godDescription;
        }
    }

    public class GreekHero 
    {
        private string heroName;

        public string HeroName
        {
            get { return heroName; }
            set { heroName = value; }
        }

        public GreekHero(string heroName)
        {
            this.HeroName = heroName;
        }
    }

    public class Places : ObservableCollection<Place>
    {
        public Places()
        {
            Add(new Place("Bellevue", "WA"));
            Add(new Place("Bellingham", "WA"));
            Add(new Place("Kirkland", "WA"));
            Add(new Place("Los Angeles", "CA"));
            Add(new Place("Portland", "OR"));
            Add(new Place("Redmond", "WA"));
            Add(new Place("San Diego", "CA"));
            Add(new Place("San Francisco", "CA"));
            Add(new Place("San Jose", "CA"));
            Add(new Place("Santa Ana", "CA"));
            Add(new Place("Seattle", "WA"));
        }
    }

    public class Place
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string state;

        public string State
        {
            get { return state; }
            set { state = value; }
        }

        public Place(string name, string state)
        {
            this.name = name;
            this.state = state;
        }
    }
}