using System.ComponentModel;
using System.Reflection;

namespace Common.Utilities.Extensions
{
    public static class EnumExtensions
    {
        public static T GetAttribute<T>(this Enum @enum) where T : Attribute
        {
            var type = @enum.GetType();
            var name = Enum.GetName(type, @enum);
            var field = type.GetField(name);
            var attribute = field.GetCustomAttribute<T>();

            return attribute ?? throw new ArgumentException("Enum Attribute not found.", nameof(@enum)); ;
        }

        public static string GetDescription(this Enum enumValue)
        {
            FieldInfo fi = enumValue.GetType().GetField(enumValue.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return enumValue.ToString();
        }

        public static bool IsValidEnum<TEnum>(string value) where TEnum : struct
        {
            return Enum.TryParse(value, true, out TEnum s) && Enum.IsDefined(typeof(TEnum), s);
        }
    }
}
