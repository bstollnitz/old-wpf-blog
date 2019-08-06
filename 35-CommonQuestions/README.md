# What is the difference between... ?

I had a blast at TechEd. After a few years working on WPF, it's very rewarding to see people's excitement when they discover all the features we offer. The high point of the conference for me (apart from several detours to the Belgian chocolate fountain, yum...) was <a href="http://www.interact-sw.co.uk/iangblog/">Ian Griffiths'</a> presentation on data binding. If you haven't seen Ian speak, you're missing out! His presentation was well prepared, well delivered and technically accurate. I was also very excited to see that the WPF's labs were incredibly popular, so popular in fact that half way through the conference the number of machines assigned to WPF was doubled.  

Now, back to the rainy weather... sigh...

Today I will answer a few simple questions that I get asked repeatedly. There are some concepts in WPF data binding that are similar enough that users become easily confused. If there are other concepts that you think should have made this list, please leave a comment and I'll cover them in my next post.

## What is the difference between CollectionView and CollectionViewSource?

The short answer is that CollectionView is a view and CollectionViewSource is not. 

Every time you bind an ItemsControl directly to a collection, we create a view on top of that collection and bind to the view instead. You can think of a view as a list of pointers to the source collection. Views make it possible to perform four operations on bound collections: sorting, filtering, grouping and tracking the current item. Creating an intermediate object to handle these operations may seem like unnecessary overhead, but it's important to guarantee that the original data remains intact. Imagine the problems that could occur without view objects if two different parts of the UI were bound to the same collection but with different sorting and filtering.

So, what is the type of that view object? At the very minimum, it needs to implement ICollectionView. CollectionView is the base implementation we provide for this interface, and it's also the view type we create when the source collection implements IEnumerable and nothing else. CollectionView is the base class for two other interesting view classes: BindingListCollectionView, which we create when the source collection implements IBindingList, and ListCollectionView, which is created when the source collection implements IList. I talked about views in a bit more detail in an <a href="..\21-CustomSorting">earlier post</a>.

CollectionViewSource is *not* a view. We designed this class for three reasons:

- We wanted to allow users to create a custom view and be able to tell us to use that view without the use of code (all in markup). I may show how this can be done in a future post (is this a topic of interest?).

- We wanted to allow users to do simple sorting and grouping without using code. You can see a sample with this scenario in <a href="..\14-SortingGroups">this earlier post</a>. - We wanted to have a container for all methods and properties related to view operations. I'm not sure where they lived before we had this class - possibly in BindingOperations - but I do remember realizing that users had a hard time finding them.

CollectionViewSource has a Source property that should be set to the source collection and a read-only View property that returns a handle to the view we create over that collection. If we set the Source property of a Binding to a CollectionViewSource, the binding engine is smart enough to understand that most of the time we really want to bind to the view, so it binds to its View property instead. (If this is not what you want, you can set the BindsDirectlyToSource property of Binding to true.) I believe this is the reason why people tend to think that CollectionViewSource is a view. Also, the name is probably a bit misleading.

In summary, you can think of CollectionViewSource as an intermediate class that has a pointer to the source collection and another one to the corresponding view, and that offers the advantages I mentioned above. A CollectionView is simply the base class for all view types we ship in WPF.

## What is the difference between Binding and TemplateBinding?

Binding provides much more flexibility than TemplateBinding, but it's more costly. TemplateBinding is limited to one scenario but very efficient in what it does. 

Everytime you want to bind to a property of an element from within its template, you should consider using a TemplateBinding. For example, consider the following scenario:

	<Window x:Class="CommonQuestions.Window1" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Title="CommonQuestions" Height="300" Width="300">
		<Window.Resources>
			<ControlTemplate TargetType="{x:Type Button}" x:Key="buttonTemplate">
				<Border BorderBrush="{TemplateBinding Property=Background}" BorderThickness="3" >
					<ContentPresenter Margin="10"/>
				</Border>
			</ControlTemplate>
		</Window.Resources>
		<StackPanel Margin="20">
			<Button Template="{StaticResource buttonTemplate}" HorizontalAlignment="Center" Background="SteelBlue">Hello</Button>
		</StackPanel>
	</Window>

In this case, I want the BorderBrush property of Border to be whatever color is specified in the Button's Background. This is exactly the scenario that TemplateBinding is optimized for. Instead of the TemplateBinding in this snippet, you could use the following Binding: {Binding RelativeSource={RelativeSource TemplatedParent}, Path=Background}. The resulting behavior would be the same, but this is not as efficient, and it's quite a bit more complex to write, so TemplateBinding is the preferred approach. TemplateBinding also allows you to specify a Converter and a ConverterParameter, which increases the scope of the supported scenarios quite a bit. However, a TemplateBinding can only transfer data in one direction: from the templated parent to the element with the TemplateBinding. If you need to transfer data in the opposite direction or both ways, a Binding with RelativeSource of TemplatedParent is your only option. For example, interaction with a TextBox or Slider within a template will only change a property on the templated parent if you use a two-way Binding.

In summary: if you want a one-way binding from within a template to a property of its templated parent, use TemplateBinding. For all other scenarios, or for extra flexibility in this last scenario, use Binding.

## What is the difference between ContentPresenter and ContentControl?

If you look at these two classes in <a href="http://www.red-gate.com/products/reflector/">reflector</a>, you will notice the main difference between them: ContentControl derives from Control, and ContentPresenter doesn't. 

ContentControl is a control that knows how to display content. If you've been reading my blog, you're probably familiar with ItemsControl by now, which is a control that knows how to display a collection of data. ContentControl is the equivalent to ItemsControl, but it is used to display non-collections instead. Some classic examples of controls that derive from ContentControl are Button and Label. Its most important property is the Content DependencyProperty, of type object.

ContentPresenter is an element that is useful inside the template of a ContentControl, and is used to specify where you want its content to be placed. For example, in the markup above I placed a ContentPresenter inside the Border because I want the Content of the Button ("Hello") to appear inside the Border. If you remove the ContentPresenter, you will notice that "Hello" is no longer displayed. If you add elements before or after, you will notice that “Hello" will show up in the location where the ContentPresenter is placed in the layout pass.

The ContentPresenter tag in the markup above is equivalent to the following:

	<ContentPresenter Content="{TemplateBinding Content}" ContentTemplate="{TemplateBinding ContentTemplate}" ContentTemplateSelector="{TemplateBinding ContentTemplateSelector}" Margin="10"/>

A long time ago, you had to be explicit about where the Content, ContentTemplate and ContentTemplateSelector properties came from. We decided to make this implicit because we realized this is what people want most of the time. If, for some reason, you don't want to use the Content of your ContentControl in its template, and want to use some other data instead, you can set the Content property of the ContentPresenter explicitly. For example, try replacing the ContentPresenter in the markup above with the following:

	<ContentPresenter Content="{TemplateBinding Background}" Margin="10"/>

You will notice that the Button will display "#FF4682B4" instead of "Hello", even though we set its Content property to "Hello".

In summary: ContentControl is a control that uses a template to display a single piece of content, and ContentPresenter is used to specify where the content should go in the ContentControl's template.
