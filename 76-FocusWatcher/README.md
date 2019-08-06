# How to debug keyboard focus

When it comes to interacting with a graphical user interface, touch and mouse input typically come to mind first, and get the lion's share of a developer's attention.  However, there are plenty of users who prefer to type or must type, and therefore it's important to make sure applications work well with the keyboard.  Fixing keyboard focus issues is the first step, whether you are making your application accessible to blind users or making it easier for everyone to navigate.

Here are some problems we frequently encounter when building applications:
• No initial focus.  When a new page, window, dialog, or popup appears, one of its controls should have keyboard focus.
• Loss of focus.  When an application removes, hides, or disables the control that currently has keyboard focus, it should set focus to another control.  Otherwise, keyboard users can't do anything until they press Tab, at which point focus returns to the start of the page.
• Incorrect tab order.  The order in which controls get keyboard focus should make logical sense, corresponding to the order in which items are read and used.

Discovering and solving these issues can be tricky.  It's often difficult to see precisely which control has focus, and of course, switching between a running app and the debugger has an impact on keyboard focus.  While tools like <a href="http://snoopwpf.codeplex.com/">Snoop</a> and <a href="http://xamlspy.com/">XAML Spy</a> help with many issues in XAML applications, we like to solve keyboard focus problems in Windows Store apps using our very own home-grown FocusWatcher class.

FocusWatcher helps in two ways: it draws a red outline around the control that has focus, and it displays the type, name, and content of the focused control in a text overlay.  These visual aids are updated continuously to track the focused control.  In the screenshot below, we're running FocusWatcher on the <a href="http://code.msdn.microsoft.com/windowsapps/Getting-started-with-C-and-41e15af5">Windows SDK blog reader sample</a> and you can see that keyboard focus is on a ListViewItem whose content is a FeedItem.

<img src="http://www.zagstudio.com/blogfiles/76/76FocusWatcher.png" class="postImage" />

Here's how to use FocusWatcher to solve issues in your own Windows Store application:
1. Download <a href="http://www.zagstudio.com/blogfiles/76/FocusWatcher.zip">FocusWatcher.zip</a>.
2. Add FocusWatcher.cs to your project.
3. In App.xaml.cs, within the app's OnLaunched method, just after calling Window.Current.Activate(), add a call to ZagStudio.Helpers.FocusWatcher.Start().
4. Build and run.

Your code should look something like this:

	protected override void OnLaunched(LaunchActivatedEventArgs args)
	{
		...
		Window.Current.Activate();
		ZagStudio.Helpers.FocusWatcher.Start();
		…
	}

You can also start and stop FocusWatcher whenever you want, in case you need to sort out a keyboard focus problem in just one part of your UI.  We hope this tool helps you make great keyboard-accessible apps!

