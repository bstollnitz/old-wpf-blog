using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.ComponentModel;
using System.Threading;
using System.Collections;
using System.Diagnostics;


namespace ChangesMultithreading
{
	public partial class Window1 : Window
	{
		Thread workerThread1;
		Thread workerThread2;
		BeginInvokeOC<Place> beginInvokePlaces;
		InvokeOC<Place> invokePlaces;
		ObservableCollection<Place> throwPlaces;
		object lockObject;

		public Window1()
		{
			InitializeComponent();
			lockObject = new object();
		}

		// Avalon will throw if it receives a collection change notification
		// from a collection that was changed by a different thread.
		private void Throw_Click(object sender, RoutedEventArgs e)
		{
			throwPlaces = new ObservableCollection<Place>();
			AddPlaces(throwPlaces);
			lb.ItemsSource = throwPlaces;
			lb.DisplayMemberPath = "Name";
			workerThread1 = new Thread(new ThreadStart(CrashMe));
			workerThread1.Start();
		}

		void CrashMe()
		{
			throwPlaces.RemoveAt(0);
		}

		// An attempt to solve the problem: use the UI thread's dispatcher
		// to delegate collection changes to the UI thread. The exception
		// is gone and it seems to work at first sight.
		private void DelegateUIThread_Click(object sender, RoutedEventArgs e)
		{
			beginInvokePlaces = new BeginInvokeOC<Place>(lb.Dispatcher);
			AddPlaces(beginInvokePlaces);
			lb.ItemsSource = beginInvokePlaces;
			lb.DisplayMemberPath = "Name";
			workerThread1 = new Thread(new ThreadStart(DontCrashMe));
			workerThread1.Start();
		}

		void DontCrashMe()
		{
			beginInvokePlaces.RemoveAt(0);
		}

		// A scenario that shows how the previous attempt can cause your
		// application to be in a bad state.
		private void DelegateUIThreadNotWorking_Click(object sender, RoutedEventArgs e)
		{
			beginInvokePlaces = new BeginInvokeOC<Place>(lb.Dispatcher);
			AddPlaces(beginInvokePlaces);
			lb.ItemsSource = beginInvokePlaces;
			lb.DisplayMemberPath = "Name";
			workerThread1 = new Thread(new ThreadStart(DelegateUIThreadNotWorking_Thread1));
			workerThread1.Start();
			workerThread2 = new Thread(new ThreadStart(DelegateUIThreadNotWorking_Thread2));
			workerThread2.Start();
		}

		void DelegateUIThreadNotWorking_Thread1()
		{
			int count = beginInvokePlaces.Count;
			Thread.Sleep(500); // do a bunch of work (or be really unlucky to be interrupted by another thread here)
			Place newPlace = beginInvokePlaces[count - 1];
		}

		void DelegateUIThreadNotWorking_Thread2()
		{
			Thread.Sleep(100); // do a little work
			beginInvokePlaces.RemoveAt(0);
		}

		// If you get all your locks right, this solution won't get you in
		// a bad state, but it has a few other disadvantages and unknowns.
		private void LockingOperations_Click(object sender, RoutedEventArgs e)
		{
			invokePlaces = new InvokeOC<Place>(lb.Dispatcher);
			AddPlaces(invokePlaces);
			lb.ItemsSource = invokePlaces;
			lb.DisplayMemberPath = "Name";
			workerThread1 = new Thread(new ThreadStart(LockingOperations_Thread1));
			workerThread1.Start();
			workerThread2 = new Thread(new ThreadStart(LockingOperations_Thread2));
			workerThread2.Start();
		}

		void LockingOperations_Thread1()
		{
			lock (lockObject)
			{
				int count = invokePlaces.Count;
				Thread.Sleep(500); // do a bunch of work 
				Place newPlace = invokePlaces[count - 1];
			}
		}

		void LockingOperations_Thread2()
		{
			lock (lockObject)
			{
				Thread.Sleep(100); // do a little work
				invokePlaces.RemoveAt(0);
			}
		}

		private void AddPlaces(ObservableCollection<Place> places)
		{
			places.Add(new Place("Seattle", "WA"));
			places.Add(new Place("Redmond", "WA"));
			places.Add(new Place("Bellevue", "WA"));
			places.Add(new Place("Kirkland", "WA"));
			places.Add(new Place("Portland", "OR"));
			places.Add(new Place("San Francisco", "CA"));
			places.Add(new Place("Los Angeles", "CA"));
			places.Add(new Place("San Diego", "CA"));
			places.Add(new Place("San Jose", "CA"));
			places.Add(new Place("Santa Ana", "CA"));
			places.Add(new Place("Bellingham", "WA"));
		}
	}

	public class BeginInvokeOC<T> : ObservableCollection<T>
	{
		private Dispatcher dispatcherUIThread;

		private delegate void SetItemCallback(int index, T item);
		private delegate void RemoveItemCallback(int index);
		private delegate void ClearItemsCallback();
		private delegate void InsertItemCallback(int index, T item);
		private delegate void MoveItemCallback(int oldIndex, int newIndex);

		public BeginInvokeOC(Dispatcher dispatcher)
		{
			this.dispatcherUIThread = dispatcher;
		}

		protected override void SetItem(int index, T item)
		{
			if (dispatcherUIThread.CheckAccess())
			{
				base.SetItem(index, item);
			}
			else
			{
				dispatcherUIThread.BeginInvoke(DispatcherPriority.Send,
					new SetItemCallback(SetItem), index, new object[] { item });
			}
		}

		protected override void RemoveItem(int index)
		{
			if (dispatcherUIThread.CheckAccess())
			{
				base.RemoveItem(index);
			}
			else
			{
				dispatcherUIThread.BeginInvoke(DispatcherPriority.Send,
					new RemoveItemCallback(RemoveItem), index);
			}
		}

		protected override void ClearItems()
		{
			if (dispatcherUIThread.CheckAccess())
			{
				base.ClearItems();
			}
			else
			{
				dispatcherUIThread.BeginInvoke(DispatcherPriority.Send,
					new ClearItemsCallback(ClearItems));
			}
		}

		protected override void InsertItem(int index, T item)
		{
			if (dispatcherUIThread.CheckAccess())
			{
				base.InsertItem(index, item);
			}
			else
			{
				dispatcherUIThread.BeginInvoke(DispatcherPriority.Send,
					new InsertItemCallback(InsertItem), index, new object[] { item });
			}
		}

		protected override void MoveItem(int oldIndex, int newIndex)
		{
			if (dispatcherUIThread.CheckAccess())
			{
				base.MoveItem(oldIndex, newIndex);
			}
			else
			{
				dispatcherUIThread.BeginInvoke(DispatcherPriority.Send,
					new MoveItemCallback(MoveItem), oldIndex, new object[] { newIndex });
			}
		}
	}

	public class InvokeOC<T> : ObservableCollection<T>
	{
		private Dispatcher dispatcherUIThread;

		private delegate void SetItemCallback(int index, T item);
		private delegate void RemoveItemCallback(int index);
		private delegate void ClearItemsCallback();
		private delegate void InsertItemCallback(int index, T item);
		private delegate void MoveItemCallback(int oldIndex, int newIndex);

		public InvokeOC(Dispatcher dispatcher)
		{
			this.dispatcherUIThread = dispatcher;
		}

		protected override void SetItem(int index, T item)
		{
			if (dispatcherUIThread.CheckAccess())
			{
				base.SetItem(index, item);
			}
			else
			{
				dispatcherUIThread.Invoke(DispatcherPriority.Send,
					new SetItemCallback(SetItem), index, new object[] { item });
			}
		}

		protected override void RemoveItem(int index)
		{
			if (dispatcherUIThread.CheckAccess())
			{
				base.RemoveItem(index);
			}
			else
			{
				dispatcherUIThread.Invoke(DispatcherPriority.Send,
					new RemoveItemCallback(RemoveItem), index);
			}
		}

		protected override void ClearItems()
		{
			if (dispatcherUIThread.CheckAccess())
			{
				base.ClearItems();
			}
			else
			{
				dispatcherUIThread.Invoke(DispatcherPriority.Send,
					new ClearItemsCallback(ClearItems));
			}
		}

		protected override void InsertItem(int index, T item)
		{
			if (dispatcherUIThread.CheckAccess())
			{
				base.InsertItem(index, item);
			}
			else
			{
				dispatcherUIThread.Invoke(DispatcherPriority.Send,
					new InsertItemCallback(InsertItem), index, new object[] { item });
			}
		}

		protected override void MoveItem(int oldIndex, int newIndex)
		{
			if (dispatcherUIThread.CheckAccess())
			{
				base.MoveItem(oldIndex, newIndex);
			}
			else
			{
				dispatcherUIThread.Invoke(DispatcherPriority.Send,
					new MoveItemCallback(MoveItem), oldIndex, new object[] { newIndex });
			}
		}
	}

	public class Place : INotifyPropertyChanged
	{
		private string name;

		private string state;

		public string Name
		{
			get { return name; }
			set
			{
				name = value;
				OnPropertyChanged("Name");
			}
		}

		public string State
		{
			get { return state; }
			set
			{
				state = value;
				OnPropertyChanged("State");
			}
		}

		public Place(string name, string state)
		{
			this.name = name;
			this.state = state;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}