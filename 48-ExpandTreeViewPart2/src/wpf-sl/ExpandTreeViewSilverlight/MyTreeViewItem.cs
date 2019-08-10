using System.Windows;
using System.Windows.Data;
using Microsoft.Windows.Controls;

namespace ExpandTreeViewSilverlight
{
    public class MyTreeViewItem : TreeViewItem
    {
        protected override DependencyObject GetContainerForItemOverride()
        {
            MyTreeViewItem tvi = new MyTreeViewItem();
            Binding expandedBinding = new Binding("IsExpanded");
            expandedBinding.Mode = BindingMode.TwoWay;
            tvi.SetBinding(MyTreeViewItem.IsExpandedProperty, expandedBinding);
            Binding selectedBinding = new Binding("IsSelected");
            selectedBinding.Mode = BindingMode.TwoWay;
            tvi.SetBinding(MyTreeViewItem.IsSelectedProperty, selectedBinding);
            return tvi;
        }
    }
}
