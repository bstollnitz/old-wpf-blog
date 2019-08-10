using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace SortingHierarchy
{
	public class DataSource
	{
		public ObservableCollection<State> States { get; private set; }

		public DataSource()
		{
			// WA
			City seattle = new City() { CityName = "Seattle" };
			City kirkland = new City() { CityName = "Kirkland" };
			City redmond = new City() { CityName = "Redmond" };
			City bellevue = new City() { CityName = "Bellevue" };

			City bothell = new City() { CityName = "Bothell" };
			City everett = new City() { CityName = "Everett" };
			City edmonds = new City() { CityName = "Edmonds" };

			City spokane = new City() { CityName = "Spokane" };

			County kingCounty = new County() { CountyName = "King County", Cities = { seattle, kirkland, redmond, bellevue } };
			County snohomishCounty = new County() { CountyName = "Snohomish County", Cities = { bothell, everett, edmonds } };
			County spokaneCounty = new County() { CountyName = "Spokane County", Cities = { spokane } };

			State wa = new State() { Abbreviation = "WA", Counties = { kingCounty, snohomishCounty, spokaneCounty }, StateName = "Washington" };

			// OR
			City tillamook = new City() { CityName = "Tillamook" };
			City bayCity = new City() { CityName = "Bay City" };
			City wheeler = new City() { CityName = "Wheeler" };

			City prescott = new City() { CityName = "Prescott" };
			City columbiaCity = new City() { CityName = "Columbia City" };
			City vernonia = new City() { CityName = "Vernonia" };
			City clatskanie = new City() { CityName = "Clatskanie" };

			City cannonBeach = new City() { CityName = "Cannon Beach" };
			City seaside = new City() { CityName = "Seaside" };
			City astoria = new City() { CityName = "Astoria" };

			County tillamookCounty = new County() { CountyName = "Tillamook County", Cities = { tillamook, bayCity, wheeler } };
			County columbiaCounty = new County() { CountyName = "Columbia County", Cities = { prescott, columbiaCity, vernonia, clatskanie } };
			County clatsopCounty = new County() { CountyName = "Clatsop County", Cities = { cannonBeach, seaside, astoria } };

			State or = new State() { Abbreviation = "OR", Counties = { tillamookCounty, columbiaCounty, clatsopCounty }, StateName = "Oregon" };

			// CA
			City millValley = new City() { CityName = "Mill Valley" };
			City sausalito = new City() { CityName = "Sausalito" };
			City muirBeach = new City() { CityName = "Muir Beach" };

			City lagunaHills = new City() { CityName = "Laguna Hills" };
			City irvine = new City() { CityName = "Irvine" };
			City anaheim = new City() { CityName = "Anaheim" };

			City napa = new City() { CityName = "Napa" };
			City calistoga = new City() { CityName = "Calistoga" };
			City yountville = new City() { CityName = "Yountville" };

			County marinCounty = new County() { CountyName = "Marin County", Cities = { millValley, sausalito, muirBeach } };
			County orangeCounty = new County() { CountyName = "Orange County", Cities = { lagunaHills, irvine, anaheim } };
			County napaCounty = new County() { CountyName = "Napa County", Cities = { napa, calistoga, yountville } };

			State ca = new State() { Abbreviation = "CA", Counties = { marinCounty, orangeCounty, napaCounty }, StateName = "California" };

			this.States = new ObservableCollection<State> { wa, or, ca };
		}
	}
}