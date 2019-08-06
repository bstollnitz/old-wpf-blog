using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace ZagStudio.Helpers
{
	public static class TreeHelper
	{
		public static IEnumerable<T> Descendants<T>(this DependencyObject element) where T : DependencyObject
		{
			if (element != null)
			{
				for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
				{
					DependencyObject child = VisualTreeHelper.GetChild(element, i);
					T result = child as T;
					if (result != null)
					{
						yield return result;
					}

					foreach (T descendant in Descendants<T>(child))
					{
						yield return descendant;
					}
				}
			}
		}

		public static IEnumerable<T> DescendantsAndSelf<T>(this DependencyObject element) where T : DependencyObject
		{
			T result = element as T;
			if (result != null)
			{
				yield return result;
			}

			foreach (T descendant in Descendants<T>(element))
			{
				yield return descendant;
			}
		}

		public static IEnumerable<T> Ancestors<T>(this DependencyObject element) where T : DependencyObject
		{
			while (element != null)
			{
				element = VisualTreeHelper.GetParent(element);
				T result = element as T;
				if (result != null)
				{
					yield return result;
				}
			}
		}

		public static IEnumerable<T> AncestorsAndSelf<T>(this DependencyObject element) where T : DependencyObject
		{
			T result = element as T;
			if (result != null)
			{
				yield return result;
			}

			foreach (T ancestor in Ancestors<T>(element))
			{
				yield return ancestor;
			}
		}
	}
}
