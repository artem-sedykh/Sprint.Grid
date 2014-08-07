namespace Sprint.Dynamic
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Threading;
 
    internal class ClassFactory
    {
        public static readonly ClassFactory Instance = new ClassFactory();

        static ClassFactory() { }  // Trigger lazy initialization of static fields

        readonly ModuleBuilder module;
        readonly Dictionary<Signature, Type> classes;
        int classCount;
        readonly ReaderWriterLock rwLock;

        private ClassFactory()
        {
            var name = new AssemblyName("DynamicClasses");
            var assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);

#if ENABLE_LINQ_PARTIAL_TRUST

            new ReflectionPermission(PermissionState.Unrestricted).Assert();
#endif
            try
            {
                module = assembly.DefineDynamicModule("Module");
            }
            finally
            {

#if ENABLE_LINQ_PARTIAL_TRUST
                PermissionSet.RevertAssert();
#endif
            }

            classes = new Dictionary<Signature, Type>();

            rwLock = new ReaderWriterLock();
        }

        public Type GetDynamicClass(IEnumerable<DynamicProperty> properties)
        {
            rwLock.AcquireReaderLock(Timeout.Infinite);

            try
            {
                var signature = new Signature(properties);
                Type type;
                if (!classes.TryGetValue(signature, out type))
                {
                    type = CreateDynamicClass(signature.Properties);
                    classes.Add(signature, type);
                }
                return type;
            }
            finally
            {
                rwLock.ReleaseReaderLock();
            }
        }

        Type CreateDynamicClass(DynamicProperty[] properties)
        {
            var cookie = rwLock.UpgradeToWriterLock(Timeout.Infinite);

            try
            {
                var typeName = "DynamicClass" + (classCount + 1);

#if ENABLE_LINQ_PARTIAL_TRUST
                new ReflectionPermission(PermissionState.Unrestricted).Assert();
#endif
                try
                {
                    var tb = module.DefineType(typeName, TypeAttributes.Class |
                        TypeAttributes.Public, typeof(DynamicClass));
                    var fields = GenerateProperties(tb, properties);
                    GenerateEquals(tb, fields);
                    GenerateGetHashCode(tb, fields);
                    Type result = tb.CreateType();
                    classCount++;
                    return result;
                }
                finally
                {
#if ENABLE_LINQ_PARTIAL_TRUST
                    PermissionSet.RevertAssert();
#endif
                }
            }
            finally
            {
                rwLock.DowngradeFromWriterLock(ref cookie);
            }
        }

        static FieldInfo[] GenerateProperties(TypeBuilder tb, DynamicProperty[] properties)
        {
            var fields = new FieldInfo[properties.Length];
            for (var i = 0; i < properties.Length; i++)
            {
                var dp = properties[i];
                var fb = tb.DefineField("_" + dp.Name, dp.Type, FieldAttributes.Private);
                var pb = tb.DefineProperty(dp.Name, PropertyAttributes.HasDefault, dp.Type, null);
                var mbGet = tb.DefineMethod("get_" + dp.Name,
                    MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                    dp.Type, Type.EmptyTypes);
                var genGet = mbGet.GetILGenerator();
                genGet.Emit(OpCodes.Ldarg_0);
                genGet.Emit(OpCodes.Ldfld, fb);
                genGet.Emit(OpCodes.Ret);
                var mbSet = tb.DefineMethod("set_" + dp.Name,
                    MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                    null, new[] { dp.Type });
                var genSet = mbSet.GetILGenerator();
                genSet.Emit(OpCodes.Ldarg_0);
                genSet.Emit(OpCodes.Ldarg_1);
                genSet.Emit(OpCodes.Stfld, fb);
                genSet.Emit(OpCodes.Ret);
                pb.SetGetMethod(mbGet);
                pb.SetSetMethod(mbSet);
                fields[i] = fb;
            }
            return fields;
        }

        void GenerateEquals(TypeBuilder tb, IEnumerable<FieldInfo> fields)
        {
            var mb = tb.DefineMethod("Equals",
                MethodAttributes.Public | MethodAttributes.ReuseSlot |
                MethodAttributes.Virtual | MethodAttributes.HideBySig,
                typeof(bool), new[] { typeof(object) });
            var gen = mb.GetILGenerator();
            var other = gen.DeclareLocal(tb);
            var next = gen.DefineLabel();

            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Isinst, tb);
            gen.Emit(OpCodes.Stloc, other);
            gen.Emit(OpCodes.Ldloc, other);
            gen.Emit(OpCodes.Brtrue_S, next);
            gen.Emit(OpCodes.Ldc_I4_0);
            gen.Emit(OpCodes.Ret);
            gen.MarkLabel(next);

            foreach (var field in fields)
            {
                var ft = field.FieldType;
                var ct = typeof(EqualityComparer<>).MakeGenericType(ft);
                next = gen.DefineLabel();
                gen.EmitCall(OpCodes.Call, ct.GetMethod("get_Default"), null);
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldfld, field);
                gen.Emit(OpCodes.Ldloc, other);
                gen.Emit(OpCodes.Ldfld, field);
                gen.EmitCall(OpCodes.Callvirt, ct.GetMethod("Equals", new[] { ft, ft }), null);
                gen.Emit(OpCodes.Brtrue_S, next);
                gen.Emit(OpCodes.Ldc_I4_0);
                gen.Emit(OpCodes.Ret);
                gen.MarkLabel(next);
            }

            gen.Emit(OpCodes.Ldc_I4_1);
            gen.Emit(OpCodes.Ret);
        }

        private static void GenerateGetHashCode(TypeBuilder tb, IEnumerable<FieldInfo> fields)
        {
            var mb = tb.DefineMethod("GetHashCode",
                MethodAttributes.Public | MethodAttributes.ReuseSlot |
                MethodAttributes.Virtual | MethodAttributes.HideBySig,
                typeof(int), Type.EmptyTypes);

            var gen = mb.GetILGenerator();

            gen.Emit(OpCodes.Ldc_I4_0);

            foreach (var field in fields)
            {
                var ft = field.FieldType;
                var ct = typeof(EqualityComparer<>).MakeGenericType(ft);
                gen.EmitCall(OpCodes.Call, ct.GetMethod("get_Default"), null);
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldfld, field);
                gen.EmitCall(OpCodes.Callvirt, ct.GetMethod("GetHashCode", new [] { ft }), null);
                gen.Emit(OpCodes.Xor);
            }

            gen.Emit(OpCodes.Ret);
        }
    }
}
