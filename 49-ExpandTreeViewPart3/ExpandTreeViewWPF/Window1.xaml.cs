using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ExpandTreeViewWPF
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Expands all TreeViewItems.
        /// Every time an item is expanded we need to do a non-blocking wait to give WPF a chance to create the
        /// next level of containers. We need to make sure all dispatcher operations at priority Background have
        /// been completed, so we wait for the priority that comes right after Background: ContextIdle.
        /// </summary>
        /// <param name="sender">The Button clicked.</param>
        /// <param name="e">Parameters associated to the Button click.</param>
        private void ExpandAll(object sender, RoutedEventArgs e)
        {
            ApplyActionToAllTreeViewItems(itemsControl =>
                {
                    itemsControl.IsExpanded = true;
                    DispatcherHelper.WaitForPriority(DispatcherPriority.ContextIdle);
                }, 
                treeView);
        }

        /// <summary>
        /// Collapses all TreeViewItems.
        /// There is no need to give back control to WPF (by calling WaitForPriority) in this
        /// case because the TreeViewItems are all created.
        /// Also, any tree traversing order will work. Since we're not giving control back to 
        /// WPF as we collapse the TreeViewItems, WPF does not have a chance to clean them up. So, 
        /// even if we start closing them starting at the top of the tree, the children containers
        /// will be availabe.
        /// </summary>
        /// <param name="sender">The Button clicked.</param>
        /// <param name="e">Parameters associated to the Button click.</param>
        private void CollapseAll(object sender, RoutedEventArgs e)
        {
            ApplyActionToAllTreeViewItems(itemsControl => itemsControl.IsExpanded = false, treeView);
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

        /// <summary>
        /// Helper method that executes an action for every container in the hierarchy that starts with 
        /// the ItemsControl passed as a parameter. I'm passing an ItemsControl so that this method can be
        /// called with both TreeView and TreeViewItem as parameters.
        /// </summary>
        /// <param name="itemAction">Action to be executed for every item.</param>
        /// <param name="itemsControl">ItemsControl (TreeView or TreeViewItem) at the top of the hierarchy.</param>
        private void ApplyActionToAllTreeViewItems(Action<TreeViewItem> itemAction, ItemsControl itemsControl)
        {
            Stack<ItemsControl> itemsControlStack = new Stack<ItemsControl>();
            itemsControlStack.Push(itemsControl);

            while (itemsControlStack.Count != 0)
            {
                ItemsControl currentItem = itemsControlStack.Pop() as ItemsControl;
                TreeViewItem currentTreeViewItem = currentItem as TreeViewItem;
                if (currentTreeViewItem != null)
                {
                    itemAction(currentTreeViewItem);
                }
                if (currentItem != null) // this handles the scenario where some TreeViewItems are already collapsed
                {
                    foreach (object dataItem in currentItem.Items)
                    {
                        ItemsControl childElement = (ItemsControl)currentItem.ItemContainerGenerator.ContainerFromItem(dataItem);
                        itemsControlStack.Push(childElement);
                    }
                }
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
            ArrayList treeOfLifeCollection = (ArrayList)this.Resources["treeOfLife"];
            Taxonomy elementToExpand = ((Taxonomy)treeOfLifeCollection[2]).Subclasses[3].Subclasses[0].Subclasses[0].Subclasses[0];

            foreach (Taxonomy firstLevelDataItem in treeView.Items)
            {
                Collection<Taxonomy> superclasses = GetSuperclasses(firstLevelDataItem, elementToExpand);
                if (superclasses != null)
                {
                    // Expand superclasses
                    TreeViewItem parentTreeViewItem = (TreeViewItem)treeView.ItemContainerGenerator.ContainerFromItem(firstLevelDataItem);
                    parentTreeViewItem.IsExpanded = true;
                    DispatcherHelper.WaitForPriority(DispatcherPriority.Background);

                    foreach (Taxonomy superclassToExpand in superclasses.Skip(1))
                    {
                        TreeViewItem treeViewItemToExpand = (TreeViewItem)parentTreeViewItem.ItemContainerGenerator.ContainerFromItem(superclassToExpand);
                        treeViewItemToExpand.IsExpanded = true;
                        DispatcherHelper.WaitForPriority(DispatcherPriority.Background);
                        parentTreeViewItem = treeViewItemToExpand;
                    }

                    // Select node
                    TreeViewItem treeViewItemToSelect = (TreeViewItem)parentTreeViewItem.ItemContainerGenerator.ContainerFromItem(elementToExpand);
                    treeViewItemToSelect.IsSelected = true;
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
    }
}
