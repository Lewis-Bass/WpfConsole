using System.ComponentModel;

namespace Themes.Enumerations
{
    public class ThemeEnums
    {
        public enum ETheme
        {
            [DescriptionAttribute("Light")]
            Light,
            [DescriptionAttribute("Medium Light")]
            MediumLight,
            [DescriptionAttribute("Medium Dark")]
            MediumDark,
            [DescriptionAttribute("Dark")]
            Dark,
            //[DescriptionAttribute("Color Blind")]
            //ColorBlind,
        }
    }
}
