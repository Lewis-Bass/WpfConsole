using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace WpfConsole.Converters
{
    public class MaximumWidthConverterMargin5 : IValueConverter
    {
        /// <summary>
        /// set the width of the list view column to the maximum value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ListView)
            {
                ListView listview = value as ListView;
                double width = listview.Width;
                GridView gv = listview.View as GridView;
                for (int i = 0; i < gv.Columns.Count; i++)
                {
                    if (!Double.IsNaN(gv.Columns[i].Width))
                        width -= gv.Columns[i].Width;
                }
                var width2 = width - 5;
                return width - 5; // this is to take care of margin/padding
            }
            return null;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
