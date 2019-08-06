using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.Web;
using System.ComponentModel;

namespace ConvertXaml
{
    public class XamlConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string originalString = value.ToString();
            StringBuilder result = new StringBuilder();
            StringBuilder block = new StringBuilder();
            string[] lines = originalString.Split('\n');
            bool inCodeMode = false;
            string image = null;
            string zip = null;
            foreach (string line in lines)
            {
                if (line.StartsWith("{Title:}"))
                {
                    continue;
                }
                else if (line.StartsWith("{Image:}"))
                {
                    image = line.Substring(8).Trim();
                    continue;
                }
                else if (line.StartsWith("{Zip:}"))
                {
                    zip = line.Substring(6).Trim();
                    continue;
                }

                bool startWithTab = line.StartsWith("\t");
                if (startWithTab && !inCodeMode)
                {
                    EncodeText(block, result);
                    result.Append("\n");
                    block = new StringBuilder();
                    inCodeMode = true;
                }
                else if (!startWithTab && inCodeMode)
                {
                    EncodeCode(block, result);
                    result.Append("\n");
                    block = new StringBuilder();
                    inCodeMode = false;
                }                
                block.Append(line);
            }
            if (inCodeMode)
            {
                EncodeCode(block, result);
            }
            else
            {
                EncodeText(block, result);
            }

            if (image != null)
            {
                result.Append("\n<img style=\"DISPLAY: block; MARGIN: 0px auto 10px; TEXT-ALIGN: center\" alt=\"\" src=\"http://www.beacosta.com/uploaded_images/");
                result.Append(image);
                result.Append("\" border=\"0\" />");
            }

            if (zip != null)
            {
                result.Append("\n<span style=\"font-family:verdana;\"><a href=\"http://www.beacosta.com/Zips/");
                result.Append(zip);
                result.Append("\">Here</a> you can find the VS project with this sample code. This works with RC1 WPF bits. \n</span>");
            }

            return result.ToString();
        }

        private void EncodeCode(StringBuilder original, StringBuilder result)
        {
            string encodedString = HttpUtility.HtmlEncode(original.ToString());
            encodedString = encodedString.Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;");
            result.Append("<span style=\"font-family:courier;color:#6666cc;font-size:85%\">");
            result.Append(encodedString);
            result.Append("\n</span>");
        }

        private void EncodeText(StringBuilder original, StringBuilder result)
        {
            result.Append("<span style=\"font-family:verdana;\">");
            result.Append(original.ToString().Trim());
            result.Append("\n</span>");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
			throw new NotImplementedException("The ConvertBack method is not implemented because this Converter should only be used in a one-way Binding.");
		}
    }
}
