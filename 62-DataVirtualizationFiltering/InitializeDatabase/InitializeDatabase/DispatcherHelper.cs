using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace InitializeDatabase
{
	internal static class DispatcherHelper
	{
		/// <summary>
		/// This method allows developer to call a non-blocking wait. When calling WaitForPriority, the developer is certain
		/// that all dispatcher operations with priority higher than the one passed as a parameter will have been executed
		/// by the time the line of code that follows it is reached.
		/// Similar to VB's DoEvents.
		/// Keep in mind that this call does not guarantee that all operations at the priority passed will have been executed,
		/// only operations with priority higher than the one passed. In practice it waits for some (and sometimes all) 
		/// operations at the priority passed, but this is not guaranteed.
		/// </summary>
		/// <param name="priority">Priority below the one we want to wait for before the next line of code is executed.</param>
		internal static void WaitForPriority(DispatcherPriority priority)
		{
			DispatcherFrame frame = new DispatcherFrame();
			DispatcherOperation dispatcherOperation = Dispatcher.CurrentDispatcher.BeginInvoke(priority, new DispatcherOperationCallback(ExitFrameOperation), frame);
			Dispatcher.PushFrame(frame);
			if (dispatcherOperation.Status != DispatcherOperationStatus.Completed)
			{
				dispatcherOperation.Abort();
			}
		}

		private static object ExitFrameOperation(object obj)
		{
			((DispatcherFrame)obj).Continue = false;
			return null;
		}
	}
}
