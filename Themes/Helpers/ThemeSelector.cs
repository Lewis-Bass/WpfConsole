using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Themes.Enumerations;
using static Themes.Enumerations.ThemeEnums;

namespace Themes.Helpers
{

    public class ThemeSelector : DependencyObject
    {
        #region Constants
        public const string LIGHT_COLOR_THEME_URI = "/Themes;component/Themes/LightColorTheme.xaml";
        public const string MEDIUMLIGHT_COLOR_THEME_URI = "/Themes;component/Themes/MediumLightColorTheme.xaml";
        public const string MEDIUMDARK_COLOR_THEME_URI = "/Themes;component/Themes/MediumDarkColorTheme.xaml";
        public const string DARK_COLOR_THEME_URI = "/Themes;component/Themes/DarkColorTheme.xaml";
        //public const string COLORBLIND_COLOR_THEME_URI = "/Themes;component/Themes/ColorBlindColorTheme.xaml";
        public const string DEFAULT_STYLE_URI = "/Themes;component/Themes/DefaultStyles.xaml";
        #endregion Constants
        public static void ApplyTheme(Uri dictionaryUri)
        {
            var targetElement = Application.Current;
            Uri styleUri = new Uri(DEFAULT_STYLE_URI, UriKind.Relative);
            try
            {
                // find if the target element already has a theme applied
                List<ResourceDictionary> existingDictionaries =
                    (from dictionary in targetElement.Resources.MergedDictionaries.OfType<ResourceDictionary>()
                     select dictionary).ToList();

                // remove the existing dictionaries 
                foreach (ResourceDictionary thDictionary in existingDictionaries)
                {
                    targetElement.Resources.MergedDictionaries.Remove(thDictionary);
                }

                if (dictionaryUri != null)
                {
                    // add the new dictionary to the collection of merged dictionaries of the target object, needs to be added to the end to overwrite the other items
                    targetElement.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = dictionaryUri });
                }
                targetElement.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = styleUri });
            }
            finally { }
        }

        public static string ThemeEnumToURIString(ETheme theme)
        {
            switch (theme)
            {
                //case ETheme.ColorBlind:
                //    return COLORBLIND_COLOR_THEME_URI;
                case ETheme.Dark:
                    return DARK_COLOR_THEME_URI;
                case ETheme.Light:
                    return LIGHT_COLOR_THEME_URI;
                case ETheme.MediumDark:
                    return MEDIUMDARK_COLOR_THEME_URI;
                case ETheme.MediumLight:
                    return MEDIUMLIGHT_COLOR_THEME_URI;
                default:
                    return MEDIUMLIGHT_COLOR_THEME_URI;
            }
        }
    }

    //public class ThemeSelector : DependencyObject
    //{

    //    public static readonly DependencyProperty CurrentThemeDictionaryProperty =
    //        DependencyProperty.RegisterAttached("CurrentThemeDictionary", typeof(Uri),
    //        typeof(ThemeSelector),
    //        new UIPropertyMetadata(null, CurrentThemeDictionaryChanged));

    //    public static Uri GetCurrentThemeDictionary(DependencyObject obj)
    //    {
    //        return (Uri)obj.GetValue(CurrentThemeDictionaryProperty);
    //    }

    //    public static void SetCurrentThemeDictionary(DependencyObject obj, Uri value)
    //    {
    //        obj.SetValue(CurrentThemeDictionaryProperty, value);
    //    }

    //    private static void CurrentThemeDictionaryChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    //    {
    //        if (obj is FrameworkElement) // works only on FrameworkElement objects
    //        {
    //            ApplyTheme(obj as FrameworkElement, GetCurrentThemeDictionary(obj));
    //        }
    //    }

    //    private static void ApplyTheme(FrameworkElement targetElement, Uri dictionaryUri)
    //    {
    //        if (targetElement == null) return;

    //        try
    //        {
    //            ThemeResourceDictionary themeDictionary = null;
    //            if (dictionaryUri != null)
    //            {
    //                themeDictionary = new ThemeResourceDictionary();
    //                themeDictionary.Source = dictionaryUri;

    //                // add the new dictionary to the collection of merged dictionaries of the target object
    //                targetElement.Resources.MergedDictionaries.Insert(0, themeDictionary);
    //            }

    //            // find if the target element already has a theme applied
    //            List<ThemeResourceDictionary> existingDictionaries =
    //                (from dictionary in targetElement.Resources.MergedDictionaries.OfType<ThemeResourceDictionary>()
    //                 select dictionary).ToList();

    //            // remove the existing dictionaries 
    //            foreach (ThemeResourceDictionary thDictionary in existingDictionaries)
    //            {
    //                if (themeDictionary == thDictionary) continue;  // don't remove the newly added dictionary
    //                targetElement.Resources.MergedDictionaries.Remove(thDictionary);
    //            }
    //        }
    //        finally { }
    //    }
    //}


}
