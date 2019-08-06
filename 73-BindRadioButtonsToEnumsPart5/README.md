# Binding RadioButtons to an Enum – Part V

Our last few blog posts presented four different solutions to bind a list of RadioButtons to an enumeration. Today we will discuss some of the pros and cons of each of these solutions. Hopefully this analysis will help you decide which solution is right for your scenario.

## Solution 1 - Use a ListBox to track selection and style its ListBoxItems to look like RadioButtons

It's a bit of work to re-style a ListBox and its ListBoxItems to look just right, and the resulting styles are quite verbose. However, many apps already use custom styling for their RadioButtons, so it's not hard to style the ListBoxItems as well. In these situations, this option is attractive. 

## Solution 2 - Add a Boolean-valued property to the view model for each enumeration value, and bind directly to it

This is the simplest solution to read and understand. If you're collaborating with junior developers this is a good option for you. We would only recommend using this approach if you have very few enum values and don't anticipate adding more. Since this approach requires the addition of a Boolean-valued property to the view model for each enum value, the view model code can get hard to navigate when there are lots of enum values.

## Solution 3 - Use a converter to translate between enumeration values and Boolean values

This solution is also simple to understand, since converters are a core concept in binding that most developers are familiar with. Compared to the previous approach, this one has the advantage that it scales nicely to lots of enum values. Also, we like the fact that the converter that translates between enumeration values and Booleans is generic and reusable. 

## Solution 4 - Create a BindableEnum class that wraps the original enum, and bind to an indexer property in this class

This is the most elegant solution, in our opinion. If you're working on a project by yourself or with experienced developers, this is a good option for you. Developers who are unfamiliar with the code, on the other hand, may find the BindableEnum approach confusing.

The table below summarizes the advantages and disadvantages of each of the four options in terms of their view model, XAML markup, and overall legibility.

<table border="1">
<tr>
<td>Approach</td>
<td>View model</td>
<td>XAML</td>
</tr>
<td>1</td>
<td>
&#150; requires a list of available enum values
</td>
<td>
&#43; XAML on the page is very concise (just a ListBox)
&#150; not obvious that ListBox is actually displaying radio buttons&#150; requires lengthy styles for ListBox and ListBoxItem
</td>
</tr>
<tr>
<td>2</td>
<td>
&#150; littered with extra bool properties
</td>
<td>
&#43; easy to read
</td>
</tr>
<tr>
<td>3</td>
<td>
&#43; concise 
</td>
<td>
&#43; converters are well understood
&#150; requires an extra value converter (but it's reusable code)
&#150; bindings are lengthy
</td>
</tr>
<tr>
<td>4</td>
<td>
&#43; concise
&#150; requires use of BindableEnum's Value property and ValueChanged event
</td>
<td>
&#43; relatively concise 
&#150; non-standard path in bindings
</td>
</tr>
</table>
