using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Windows.Controls;

namespace ExpandTreeViewSilverlight
{
    public partial class Page : UserControl
    {
        public Page()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Expands all TreeViewItems. ExpandAll needs to be called on each top-level
        /// data item because there is no single Taxonomy data item at the root of the
        /// hierarchy.
        /// </summary>
        /// <param name="sender">The Button clicked.</param>
        /// <param name="e">Parameters associated to the Button click.</param>
        private void ExpandAll(object sender, RoutedEventArgs e)
        {
            // This iterates through the three top-level items only.
            foreach (TaxonomyViewModel item in treeView.Items)
            {
                item.ExpandAll();
            }
        }

        /// <summary>
        /// Expands one path in the tree and selects a particular TreeViewItem.
        /// </summary>
        /// <param name="sender">The Button clicked.</param>
        /// <param name="e">Parameters associated to the Button click.</param>
        private void SelectOne(object sender, RoutedEventArgs e)
        {
            ObjectCollection treeOfLifeCollection = (ObjectCollection)this.Resources["treeOfLife"];
            TaxonomyViewModel elementToExpand = (TaxonomyViewModel)((TaxonomyViewModel)treeOfLifeCollection[2]).Subclasses[3].Subclasses[0].Subclasses[0].Subclasses[0];

            // This iterates through the three top-level items only.
            foreach (TaxonomyViewModel item in treeView.Items)
            {
                if (item.ExpandSuperclasses(elementToExpand))
                {
                    elementToExpand.IsSelected = true;
                    break;
                }
            }
        }

        /// <summary>
        /// This method collapses only the top level TreeViewItems.
        /// So, if you expand several items, collapse all, and click on a top level TreeViewItem
        /// to expand it manually, the TreeView will remember which items were selected previously
        /// and will restore that state.
        /// </summary>
        /// <param name="sender">The Button clicked.</param>
        /// <param name="e">Parameters associated to the Button click.</param>
        private void CollapseTopLevel(object sender, RoutedEventArgs e)
        {
            // This iterates through the three top-level items only.
            foreach (TaxonomyViewModel item in treeView.Items)
            {
                TreeViewItem tvi = treeView.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
                tvi.IsExpanded = false;
            }
        }

        /// <summary>
        /// This method walks the entire data hierarchy and collapses all TreeViewItems.
        /// If you expand several items, collapse all, and click on a top level TreeViewItem
        /// to expand it manually, the TreeView will *NOT* remember which items were selected
        /// previously.
        /// There is currently a bug in the Toolkit TreeView that causes occasional unexpected
        /// behavior when collapsing items this way.
        /// </summary>
        /// <param name="sender">The Button clicked.</param>
        /// <param name="e">Parameters associated to the Button click.</param>
        private void CollapseAll(object sender, RoutedEventArgs e)
        {
            // This iterates through the three top-level items only.
            foreach (TaxonomyViewModel item in treeView.Items)
            {
                item.CollapseAll();
            }
        }
    }
}
