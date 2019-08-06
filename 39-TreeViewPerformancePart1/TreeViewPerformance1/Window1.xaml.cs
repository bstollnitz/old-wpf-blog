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
using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.Win32;
using System.Windows.Threading;
using System.Security.Permissions;
using System.Security;

[assembly: RegistryPermissionAttribute(SecurityAction.RequestMinimum, Read = "HKEY_CURRENT_CONFIG")]
[assembly: RegistryPermissionAttribute(SecurityAction.RequestMinimum, Read = "HKEY_CURRENT_USER")]

namespace TreeViewPerformance
{
    public delegate void SimpleDelegate(DependencyObject visual, TextBlock tb);

    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
            this.grid1.DataContext = new RegistryData1();
            this.treeView1.Loaded += new RoutedEventHandler(UpdateVisualCountTreeView1);
        }

        private void UpdateVisualCountTreeView1(object sender, EventArgs e)
        {
            UpdateVisualCount(this.treeView1, this.tbNumberOfVisuals1);
        }

        private void InvokeUpdateVisualCount(DependencyObject visual)
        {
            TextBlock tb = null;
            if (visual == this.treeView1)
            {
                tb = this.tbNumberOfVisuals1;
            }
            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new SimpleDelegate(UpdateVisualCount), visual, tb);
        }

        private static void UpdateVisualCount(DependencyObject visual, TextBlock tb)
        {
            int visualCount = GetVisualCount(visual);
            tb.Text = visualCount.ToString();
        }

        /// <summary>
        /// Counts all visuals within the visual tree.
        /// </summary>
        /// <param name="visual">It's of type DO but we know it can only be a Visual or Visual3D. </param>
        /// <returns>Number of visuals in the visual tree.</returns>
        private static int GetVisualCount(DependencyObject visual)
        {
            int visualCount = 1;
            int childCount = VisualTreeHelper.GetChildrenCount(visual);

            for (int i = 0; i < childCount; i++)
            {
                DependencyObject childVisual = VisualTreeHelper.GetChild(visual, i);
                visualCount += GetVisualCount(childVisual);
            }

            return visualCount;
        }

        #region No UI virtualization, no data virtualization
        private void ItemCollapsedOrExpanded1(object sender, RoutedEventArgs e)
        {
            this.InvokeUpdateVisualCount(this.treeView1);
            e.Handled = true;
        }
        #endregion

    }

    #region Data - No UI virtualization, no data virtualization
    public class RegistryData1
    {
        private ObservableCollection<RegistryKeyHolder1> rootKeys;
        private int dataItemsCount;

        public ObservableCollection<RegistryKeyHolder1> RootKeys
        {
            get { return rootKeys; }
        }

        public int DataItemsCount
        {
            get { return dataItemsCount; }
        }

        public RegistryData1()
        {
            this.rootKeys = new ObservableCollection<RegistryKeyHolder1>();
            this.rootKeys.Add(new RegistryKeyHolder1(Registry.CurrentUser));
            this.rootKeys.Add(new RegistryKeyHolder1(Registry.CurrentConfig));
            this.dataItemsCount = 2;
            PopulateSubKeys(this.rootKeys);
        }

        private void PopulateSubKeys(ObservableCollection<RegistryKeyHolder1> keys)
        {
            foreach (RegistryKeyHolder1 keyHolder in keys)
            {
                keyHolder.PopulateSubKeys();
                this.dataItemsCount += keyHolder.SubKeys.Count;
                // It will take forever if I get all registry keys
                if (this.dataItemsCount >= 5000)
                {
                    return;
                }
                PopulateSubKeys(keyHolder.SubKeys);
            }
        }
    }

    public class RegistryKeyHolder1
    {
        private RegistryKey key;
        private ObservableCollection<RegistryKeyHolder1> subKeys;

        public RegistryKey Key
        {
            get { return key; }
        }

        public string ShortName
        {
            get { return key.Name.Substring(key.Name.LastIndexOf('\\') + 1); }
        }

        public ObservableCollection<RegistryKeyHolder1> SubKeys
        {
            get { return subKeys; }
        }

        public RegistryKeyHolder1(RegistryKey key)
        {
            this.key = key;
            this.subKeys = new ObservableCollection<RegistryKeyHolder1>();
        }

        public void PopulateSubKeys()
        {
            try
            {
                string[] subKeyNames = this.key.GetSubKeyNames();
                for (int i = 0; i < subKeyNames.Length; i++)
                {
                    this.subKeys.Add(new RegistryKeyHolder1(this.key.OpenSubKey(subKeyNames[i])));
                }
            }
            catch (SecurityException se)
            {
                System.Console.WriteLine(se.Message);
            }
        }
    }
    #endregion

}
