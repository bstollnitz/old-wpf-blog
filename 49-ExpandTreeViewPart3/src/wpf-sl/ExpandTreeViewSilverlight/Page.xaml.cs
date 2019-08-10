using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
        /// Expands all TreeViewItems.
        /// </summary>
        /// <param name="sender">The Button clicked.</param>
        /// <param name="e">Parameters associated to the Button click.</param>
        private void ExpandAll(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < treeView.Items.Count; i++)
            {
                ExpandAllTreeViewItems((TreeViewItem)treeView.ItemContainerGenerator.ContainerFromIndex(i));
            }
        }

        /// <summary>
        /// Helper method that expands all TreeViewItems starting at the one passed as a parameter.
        /// Notice the BeginInvoke calls, where control is returned to Silverlight to give it a chance to perform the 
        /// actual expansion of TreeViewItems.
        /// </summary>
        /// <param name="currentTreeViewItem">The root TreeViewItem.</param>
        private void ExpandAllTreeViewItems(TreeViewItem currentTreeViewItem)
        {
            if (!currentTreeViewItem.IsExpanded)
            {
                currentTreeViewItem.IsExpanded = true;
                currentTreeViewItem.Dispatcher.BeginInvoke(() => ExpandAllTreeViewItems(currentTreeViewItem));
            }
            else
            {
                for (int i = 0; i < currentTreeViewItem.Items.Count; i++)
                {
                    TreeViewItem child = (TreeViewItem)currentTreeViewItem.ItemContainerGenerator.ContainerFromIndex(i);
                    ExpandAllTreeViewItems(child);
                }
            }
        }

        /// <summary>
        /// Helper method that expands all TreeViewItems in the hierarchy between the parent TreeViewItem passed 
        /// as a parameter and the data item to select, including the TVI passed and excluding the TVI that contains 
        /// the item to select. 
        /// Once all TVIs are expanded, the data item passed as a parameter is selected.
        /// Notice the BeginInvoke calls, where control is returned to Silverlight to give it a chance to perform the 
        /// actual expansion of TreeViewItems.
        /// </summary>
        /// <param name="currentTreeViewItem">The root TreeViewItem.</param>
        /// <param name="enumerator">Enumeration containing the path to expand in the tree.</param>
        /// <param name="itemToSelect">Bottom level data item to select.</param>
        private void ExpandPathAndSelectLast(TreeViewItem currentTreeViewItem, IEnumerator enumerator, object itemToSelect)
        {
            if (!currentTreeViewItem.IsExpanded)
            {
                currentTreeViewItem.IsExpanded = true;
                currentTreeViewItem.Dispatcher.BeginInvoke(() => ExpandPathAndSelectLast(currentTreeViewItem, enumerator, itemToSelect));
            }
            else if (enumerator.MoveNext())
            {
                object dataItem = enumerator.Current;
                TreeViewItem nextContainer = (TreeViewItem)currentTreeViewItem.ItemContainerGenerator.ContainerFromItem(dataItem);
                ExpandPathAndSelectLast(nextContainer, enumerator, itemToSelect);
            }
            else
            {
                TreeViewItem treeViewItemToSelect = (TreeViewItem)currentTreeViewItem.ItemContainerGenerator.ContainerFromItem(itemToSelect);
                treeViewItemToSelect.IsSelected = true;
            }
        }

        /// <summary>
        /// This method expands all TreeViewItems in the parent hierarchy of a specific data item and selects
        /// the TreeViewItem that contains that data item.
        /// It this does in two parts: first it gets the parent data hierarchy of the data item, and then
        /// it traverses that hierarchy, expands all TreeViewItems along the way, and lastly it selects the bottom
        /// most item.
        /// </summary>
        /// <param name="sender">The Button clicked.</param>
        /// <param name="e">Parameters associated to the Button click.</param>
        private void SelectOne(object sender, RoutedEventArgs e)
        {
            ObjectCollection treeOfLifeCollection = (ObjectCollection)this.Resources["treeOfLife"];
            Taxonomy elementToExpand = ((Taxonomy)treeOfLifeCollection[2]).Subclasses[3].Subclasses[0].Subclasses[0].Subclasses[0];

            foreach (Taxonomy firstLevelDataItem in treeView.Items)
            {
                Collection<Taxonomy> superclasses = GetSuperclasses(firstLevelDataItem, elementToExpand);
                if (superclasses != null)
                {
                    TreeViewItem parentTreeViewItem = (TreeViewItem)treeView.ItemContainerGenerator.ContainerFromItem(firstLevelDataItem);
                    ExpandPathAndSelectLast(parentTreeViewItem, superclasses.Skip(1).GetEnumerator(), elementToExpand);
                }
            }
        }

        /// <summary>
        /// Helper method that, given a data item to look for and a root data item, returns
        /// all data items in between, including the root and excluding the item it looks for.
        /// It returns this list of items starting at the top most superclass.
        /// </summary>
        /// <param name="currentItem">The root item to start from.</param>
        /// <param name="itemToLookFor">The bottom level item we're looking for.</param>
        /// <returns>The path between the two.</returns>
        private Collection<Taxonomy> GetSuperclasses(Taxonomy currentItem, Taxonomy itemToLookFor)
        {
            if (itemToLookFor == currentItem)
            {
                Collection<Taxonomy> results = new Collection<Taxonomy>();
                return results;
            }
            else
            {
                foreach (Taxonomy subclass in currentItem.Subclasses)
                {
                    Collection<Taxonomy> results = GetSuperclasses(subclass, itemToLookFor);
                    if (results != null)
                    {
                        results.Insert(0, currentItem);
                        return results;
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Collapses all TreeViewItems.
        /// There is no need to give back control to Silverlight in this case because the 
        /// TreeViewItems are all created.
        /// Also, any tree traversing order will work. Since we're not giving controls back to 
        /// Silverlight as we collapse the TreeViewItems, Silverlight does not have a chance to clean them up. So, 
        /// even if we start closing them starting at the top of the tree, the children containers
        /// will be availabe.
        /// </summary>
        /// <param name="sender">The Button clicked.</param>
        /// <param name="e">Parameters associated to the Button click.</param>
        private void CollapseAll(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < treeView.Items.Count; i++)
            {
                CollapseAllTreeViewItems((TreeViewItem)treeView.ItemContainerGenerator.ContainerFromIndex(i));
            }
        }

        /// <summary>
        /// Helper method that uses a stack to traverse the tree non-recursively in a depth
        /// first way and sets IsExpanded to false.
        /// </summary>
        /// <param name="rootTreeViewItem">The root TreeViewItem.</param>
        private void CollapseAllTreeViewItems(TreeViewItem rootTreeViewItem)
        {
            Stack<TreeViewItem> treeViewItemsStack = new Stack<TreeViewItem>();
            treeViewItemsStack.Push(rootTreeViewItem);
            while (treeViewItemsStack.Count != 0)
            {
                TreeViewItem current = treeViewItemsStack.Pop();
                current.IsExpanded = false;

                for (int i = 0; i < current.Items.Count; i++)
                {
                    treeViewItemsStack.Push(current.ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItem);
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
            foreach (Taxonomy item in treeView.Items)
            {
                TreeViewItem tvi = treeView.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
                tvi.IsExpanded = false;
            }
        }
    }
}
