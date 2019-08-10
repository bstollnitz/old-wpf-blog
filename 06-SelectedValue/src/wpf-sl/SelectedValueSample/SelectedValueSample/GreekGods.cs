using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.Collections.ObjectModel;

namespace SelectedValueSample
{
    public class GreekGods : ObservableCollection<GreekGod>
    {
        public GreekGods()
        {
            this.Add(new GreekGod("Aphrodite", "Goddess of love, beauty and fertility", "Venus"));
            this.Add(new GreekGod("Apollo", "God of prophesy, music and healing", "Apollo"));
            this.Add(new GreekGod("Ares", "God of war", "Mars"));
            this.Add(new GreekGod("Artemis", "Virgin goddess of the hunt", "Diana"));
            this.Add(new GreekGod("Athena", "Goddess of crafts and the domestic arts", "Athena"));
            this.Add(new GreekGod("Demeter", "Goddess of agriculture", "Ceres"));
            this.Add(new GreekGod("Dionysus", "God of wine", "Bacchus"));
            this.Add(new GreekGod("Hephaestus", "God of fire and crafts", "Vulcan"));
            this.Add(new GreekGod("Hera", "Goddess of marriage", "Juno"));
            this.Add(new GreekGod("Hermes", "Messenger of the Gods", "Mercury"));
            this.Add(new GreekGod("Poseidon", "God of the sea, earthquakes and horses", "Neptune"));
            this.Add(new GreekGod("Zeus", "Supreme God of the Olympians", "Jupiter"));
        }
    }
}
