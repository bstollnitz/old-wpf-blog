using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.Windows.Resources;
using System.IO;

namespace BindToXLinq
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();

            XDocument planetsDocument = InitializeDataSource("/Planets.xml");
            this.XLinqPanel2.DataContext = planetsDocument;
            this.XLinqPanel3.DataContext = planetsDocument;
            this.XLinqPanel4.DataContext = planetsDocument;
            this.XLinqPanel5.DataContext = planetsDocument;
            this.XLinqPanel6.DataContext = planetsDocument;
            this.XLinqPanel7.DataContext = planetsDocument;
            this.XLinqPanel8.DataContext = planetsDocument;

            XmlDataProvider xdp = new XmlDataProvider();
            xdp.Source = new Uri("Planets.xml", UriKind.Relative);
            XmlNamespaceMapping mapping = new XmlNamespaceMapping("mn", new Uri("http://myNamespace"));
            XmlNamespaceMappingCollection mappingCollection = new XmlNamespaceMappingCollection();
            mappingCollection.Add(mapping);
            xdp.XmlNamespaceManager = mappingCollection;
            this.XMLPanel2.DataContext = xdp;
            this.XMLPanel3.DataContext = xdp;
            this.XMLPanel4.DataContext = xdp;
            this.XMLPanel5.DataContext = xdp;
            this.XMLPanel6.DataContext = xdp;
            this.XMLPanel7.DataContext = xdp;
            this.XMLPanel8.DataContext = xdp;

            // WPF XPath=/SolarSystemPlanets/Planet[1]/Orbit, Path=OuterXml
            var query2 = planetsDocument.Root.Element("Planet").Element("Orbit");
            xLinqInCode2.Text = "planetsDocument.Root.Element(\"Planet\").Element(\"Orbit\")";
            Console.WriteLine(query2);
            // <Orbit>57,910,000 km (0.38 AU)</Orbit>

            // WPF Path=Root.Elements
            var query3 = planetsDocument.Root.Elements();
            xLinqInCode3.Text = "planetsDocument.Root.Elements()" + "\n" + "For each element: element.Attribute(\"Name\").Value";
            foreach (var element in query3)
            {
                // WPF Path=Attribute[Name].Value
                Console.WriteLine(element.Attribute("Name").Value);
            }
            //Mercury
            //Venus
            //Earth
            //Mars
            //Jupiter
            //Saturn
            //Uranus
            //Neptune
            //Pluto

            // WPF Path=Root.Descendants[Diameter]
            var query4 = planetsDocument.Root.Descendants("Diameter");
            xLinqInCode4.Text = "planetsDocument.Root.Descendants(\"Diameter\")" + "\n" + "For each element: element.Value";
            foreach (var element in query4)
            {
                // WPF Path=Value
                Console.WriteLine(element.Value);
            }
            //4,880 km
            //12,103.6 km
            //12,756.3 km
            //6,794 km
            //142,984 km (equatorial)
            //120,536 km (equatorial)
            //51,118 km (equatorial)
            //49,532 km (equatorial)
            //2274 km

            // WPF Path=Root.Element[Planet].Xml
            var query5 = planetsDocument.Root.Element("Planet").ToString(SaveOptions.DisableFormatting);
            xLinqInCode5.Text = "planetsDocument.Root.Element(\"Planet\").ToString(SaveOptions.DisableFormatting)";
            Console.WriteLine(query5);
            // <Planet Name="Mercury"><Orbit>57,910,000 km (0.38 AU)</Orbit><Diameter>4,880 km</Diameter><Mass>3.30e23 kg</Mass><Details xmlns="http://myNamespace">The small and rocky planet Mercury is the closest planet to the Sun.</Details></Planet>

            // WPF Path=Root.Element[Planet].Element[Mass].HasAttributes
            var query6 = planetsDocument.Root.Element("Planet").Element("Mass").HasAttributes;
            xLinqInCode6.Text = "planetsDocument.Root.Element(\"Planet\").Element(\"Mass\").HasAttributes";
            Console.WriteLine(query6);
            // false

            // WPF Path=Root.Element[Planet].Element[{http://myNamespace}Details].Value
            XNamespace ns = "http://myNamespace";
            var query7 = planetsDocument.Root.Element("Planet").Element(ns + "Details").Value;
            xLinqInCode7.Text = "planetsDocument.Root.Element(\"Planet\").Element(ns + \"Details\").Value";
            Console.WriteLine(query7);
            //The small and rocky planet Mercury is the closest planet to the Sun.

            // WPF Path=Root.Element[Planet].Elements
            var query8 = planetsDocument.Root.Element("Planet").Elements();
            xLinqInCode8.Text = "planetsDocument.Root.Element(\"Planet\").Elements()" + "\n" + "For each element: element.Value";
            foreach (var element in query8)
            {
                // wpf Path=Value
                Console.WriteLine(element.Value);
            }
            //57,910,000 km (0.38 AU)
            //4,880 km
            //3.30e23 kg
            //The small and rocky planet Mercury is the closest planet to the Sun.
        }

        private XDocument InitializeDataSource(string uriString)
        {
            StreamResourceInfo resourceInfo = Application.GetResourceStream(new Uri(uriString, UriKind.Relative));
            StreamReader reader = new StreamReader(resourceInfo.Stream);
            return XDocument.Load(reader);
        }
    }
}
