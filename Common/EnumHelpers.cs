using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace Common
{
    public class EnumHelpers
    {

        /// <summary>
        /// gets the name of the enum from the description attribute
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static List<string> GetEnumDescriptions(Type enumType)
        {
            List<string> items = new List<string>();

            Type actualEnumType = Nullable.GetUnderlyingType(enumType) ?? enumType;
            if (actualEnumType != enumType ||
                false == actualEnumType.IsEnum)
            {
                items.Add(null);
            }
            else
            {
                // otherwise we must process the list
                foreach (object item in Enum.GetValues(enumType))
                {
                    // Grab the Description Attributes
                    string itemString = item.ToString();
                    FieldInfo field = enumType.GetField(itemString);
                    object[] attribs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);

                    if (null != attribs && attribs.Length > 0)
                        itemString = ((DescriptionAttribute)attribs[0]).Description;

                    // add to the list
                    items.Add(itemString);
                }
            }

            // return the list of strings
            return items;
        }

        /// <summary>
        /// gets the enum from the description attribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="description"></param>
        /// <returns></returns>
        public static T GetValueFromDescription<T>(string description)
        {
            var type = typeof(T);
            if (!type.IsEnum) throw new InvalidOperationException();
            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field,
                    typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attribute != null)
                {
                    if (attribute.Description == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }
            throw new ArgumentException("Not found.", "description");
        }
    }
}
