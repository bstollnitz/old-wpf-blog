using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace CustomControls
{
    public static class TreeHelper
    {
        /// <summary>
        /// Finds the first ancestor of the element passed as a parameter that has type T.
        /// </summary>
        /// <typeparam name="T">The type of the ancestor we're looking for.</typeparam>
        /// <param name="element">The element where we start our search.</param>
        /// <returns>The first ancestor of element of type T.</returns>
        public static T FindAncestor<T>(DependencyObject element) where T : class
        {
            while (element != null)
            {
                DependencyObject parent = VisualTreeHelper.GetParent(element) as DependencyObject;
                T result = parent as T;
                if (result != null)
                {
                    return result;
                }
                element = parent;
            }
            return null;
        }

        /// <summary>
        /// Finds a descendent of the element passed as a parameter that has the 
        /// name passed as a parameter.
        /// </summary>
        /// <param name="element">The element where we want to start the search.</param>
        /// <param name="name">The name of the element we're looking for.</param>
        /// <returns>An element descendent of the element passed as a parameter, with the name
        /// passed as a parameter.</returns>
        public static FrameworkElement FindDescendent(FrameworkElement element, string name)
        {
            Stack<FrameworkElement> stack = new Stack<FrameworkElement>();

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                stack.Push(VisualTreeHelper.GetChild(element, i) as FrameworkElement);
            }

            while (stack.Count > 0)
            {
                FrameworkElement poppedElement = stack.Pop();
                if (poppedElement != null && String.Equals(poppedElement.Name, name))
                {
                    return poppedElement;
                }
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(poppedElement); i++)
                {
                    stack.Push(VisualTreeHelper.GetChild(poppedElement, i) as FrameworkElement);
                }
            }

            return null;
        }

        /// <summary>
        /// Checks whether the element is added to an element tree.
        /// </summary>
        /// <param name="element">The element we want to check for.</param>
        /// <returns>True if the element is part of a tree, false otherwise.</returns>
        public static bool IsInTree(this FrameworkElement element)
        {
            var rootElement = Application.Current.RootVisual as FrameworkElement;

            while (element != null)
            {
                if (element == rootElement)
                {
                    return true;
                }
                element = VisualTreeHelper.GetParent(element) as FrameworkElement;
            }

            return false;
        }

    }
}
