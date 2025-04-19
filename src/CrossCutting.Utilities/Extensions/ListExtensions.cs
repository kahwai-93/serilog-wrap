using System.ComponentModel;
using System.Data;
using System.Reflection;

namespace Common.Utilities.Extensions
{
    public static class ListExtensions
    {
        public static DataTable ToDataTable<T>(this List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                var attrs = prop.GetCustomAttribute<DescriptionAttribute>();

                var propType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                if (attrs != null)
                {
                    dataTable.Columns.Add(attrs.Description, propType);
                }
                else
                {
                    dataTable.Columns.Add(prop.Name, propType);
                }
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    var currentProp = Props[i];
                    var value = currentProp.GetValue(item, null);
                    if (value == null)
                    {
                        if (currentProp.PropertyType == typeof(decimal) || currentProp.PropertyType == typeof(decimal?))
                        {
                            value = 0; 
                        }
                        else
                        {
                            value = "-"; 
                        }
                    }

                    values[i] = value;
                }
                dataTable.Rows.Add(values);
            }

            return dataTable;
        }

    }
}
