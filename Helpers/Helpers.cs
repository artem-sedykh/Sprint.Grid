namespace Sprint.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Text;
    using System.Threading;
    using System.ComponentModel;

    internal static class Helpers
    {
        private static readonly AssemblyName AssemblyName = new AssemblyName { Name = "DynamicLinqTypes" };
        private static readonly ModuleBuilder ModuleBuilder;
        private static readonly Dictionary<string, Type> BuiltTypes = new Dictionary<string, Type>();

        static Helpers()
        {
            ModuleBuilder = Thread.GetDomain().DefineDynamicAssembly(AssemblyName, AssemblyBuilderAccess.Run).DefineDynamicModule(AssemblyName.Name);
        }

        private static string GetTypeKey(Dictionary<string, Type> fields)
        {
            var sb = new StringBuilder();

            foreach (var field in fields.OrderBy(x => x.Key).ThenBy(x => x.Value.Name))
                sb.AppendFormat("{0};{1};", field.Key, field.Value.Name);

            return sb.ToString();
        }

        /// <summary>
        /// Create new dynamic Type.
        /// </summary>
        /// <param name="properties">Dictionary, PropertyName/Type.</param>
        /// <returns>Created type.</returns>
        internal static Type GetDynamicType(Dictionary<string, Type> properties)
        {
            if (null == properties)
                throw new ArgumentNullException("properties");
            if (0 == properties.Count)
                throw new ArgumentOutOfRangeException("properties", @"properties must have at least 1 field definition");

            try
            {
                Monitor.Enter(BuiltTypes);

                var className = GetTypeKey(properties);

                if (BuiltTypes.ContainsKey(className))
                    return BuiltTypes[className];

                var typeBuilder = ModuleBuilder.DefineType(className.GetHashCode().ToString(CultureInfo.InvariantCulture), TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Serializable);

                const MethodAttributes getSetAttr = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

                foreach (var propery in properties)
                {
                    var fieldPropBldr = typeBuilder.DefineProperty(propery.Key, PropertyAttributes.HasDefault, propery.Value, null);

                    var fieldBldr = typeBuilder.DefineField("_" + propery.Key, propery.Value, FieldAttributes.Private);

                    var fieldGetPropMthdBldr = typeBuilder.DefineMethod("get_" + propery.Key, getSetAttr, propery.Value, Type.EmptyTypes);

                    var fieldGetIl = fieldGetPropMthdBldr.GetILGenerator();

                    fieldGetIl.Emit(OpCodes.Ldarg_0);
                    fieldGetIl.Emit(OpCodes.Ldfld, fieldBldr);
                    fieldGetIl.Emit(OpCodes.Ret);

                    var fieldSetPropMthdBldr = typeBuilder.DefineMethod("set_" + propery.Key, getSetAttr, null, new[] { propery.Value });

                    var fieldSetIl = fieldSetPropMthdBldr.GetILGenerator();

                    fieldSetIl.Emit(OpCodes.Ldarg_0);
                    fieldSetIl.Emit(OpCodes.Ldarg_1);
                    fieldSetIl.Emit(OpCodes.Stfld, fieldBldr);
                    fieldSetIl.Emit(OpCodes.Ret);

                    fieldPropBldr.SetGetMethod(fieldGetPropMthdBldr);
                    fieldPropBldr.SetSetMethod(fieldSetPropMthdBldr);

                }

                BuiltTypes[className] = typeBuilder.CreateType();

                return BuiltTypes[className];
            }
            finally
            {
                Monitor.Exit(BuiltTypes);
            }
        }

        /// <summary>
        ///  Create new dynamic Type.
        /// </summary>
        /// <param name="properties">PropertyInfo collection.</param>
        /// <returns>Created type.</returns>
        internal static Type GetDynamicType(IEnumerable<PropertyInfo> properties)
        {
            return GetDynamicType(properties.Where(p => p != null).ToDictionary(p => p.Name, p => p.PropertyType));
        }

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