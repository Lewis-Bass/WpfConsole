using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace WpfConsole.Converters
{

    /// <summary>
    /// Convert a boolean value to visibility
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {

        /// <summary>
        /// To use the converter follow the pattern below
        /// xmlns:converters="clr-namespace:WpfConsole.Converters"  
        /// <UserControl.Resources>
        ///     <converters:BooleanToVisibilityConverter x:Key="b2vconv"/>
        /// </UserControl.Resources>
        /// 
        /// Visibility="{Binding ShowTitleText, Converter={StaticResource b2vconv}}
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Visibility)value == Visibility.Visible;
        }
    }
}
