namespace Sprint.Helpers
{
    using System.ComponentModel;

    internal static class Helpers
    {    
        internal static T? ToNullable<T>(object target) where T : struct
        {
            var result = new T?();
            var s = target != null ? target.ToString() : string.Empty;

            if (!string.IsNullOrEmpty(s) && s.Trim().Length > 0)
            {
                var conv = TypeDescriptor.GetConverter(typeof(T));
                var convertFrom = conv.ConvertFrom(s);
                if (convertFrom != null) result = (T)convertFrom;
            }
            return result;
        }
    }
}