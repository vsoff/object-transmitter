using ObjectTransmitter.Collectors;
using ObjectTransmitter.Exceptions;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace ObjectTransmitter.Reflection
{
    internal static class ClassGenerator
    {
        public static TInterface GenerateClass<TInterface>()
        {
            var interfaceType = typeof(TInterface);
            if (!interfaceType.IsInterface)
                throw new ObjectTransmitterException("Type should be interface");

            var moduleBuilder = CreateModuleBuilder();
            var generatedClassName = GetGeneratedName<TInterface>();
            TypeBuilder typeBuilder = moduleBuilder.DefineType(generatedClassName, TypeAttributes.Public, typeof(Transmitter)); // TODO: FIX. Here is strong base type.

            // Implement interface.
            typeBuilder.AddInterfaceImplementation(typeof(TInterface));
            foreach (var propertyInfo in typeof(TInterface).GetProperties())
            {
                AddProperty(typeBuilder, propertyInfo.Name, propertyInfo.PropertyType);
            }
            
            // Create type.
            Type generatedType = typeBuilder.CreateTypeInfo();

            // Create instance.
            var instance = (TInterface)Activator.CreateInstance(generatedType);
            return instance;
        }

        private static string GetGeneratedName<TInterface>()
        {
            var baseName = typeof(TInterface).Name.Substring(1);
            return $"{baseName}_Generated_{Guid.NewGuid():N}";
        }

        private static ModuleBuilder CreateModuleBuilder()
        {
            var generatedAssemblyName = $"{nameof(ObjectTransmitter)}.Generated";
            AssemblyName assemblyName = new AssemblyName(generatedAssemblyName);
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name);

            return moduleBuilder;
        }

        private static void AddProperty(TypeBuilder typeBuilder, string propertyName, Type propertyType)
        {
            FieldBuilder fieldBuilder = typeBuilder.DefineField($"_{propertyName}", propertyType, FieldAttributes.Private);

            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);
            MethodAttributes methodAttributes = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Final;

            // Configure getter.
            MethodBuilder getMethodBuilder = typeBuilder.DefineMethod($"get_{propertyName}", methodAttributes, propertyType, Type.EmptyTypes);

            ILGenerator getIlGenerator = getMethodBuilder.GetILGenerator();
            getIlGenerator.Emit(OpCodes.Ldarg_0);
            getIlGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
            getIlGenerator.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getMethodBuilder);

            // Configure setter.
            MethodBuilder setMethodBuilder = typeBuilder.DefineMethod($"set_{propertyName}", methodAttributes, null, new Type[] { propertyType });

            ILGenerator setIlGenerator = setMethodBuilder.GetILGenerator();
            setIlGenerator.Emit(OpCodes.Ldarg_0);
            setIlGenerator.Emit(OpCodes.Ldarg_1);
            setIlGenerator.Emit(OpCodes.Stfld, fieldBuilder);
            AddSetterTransmitterSaveMethod(setIlGenerator, propertyType);
            setIlGenerator.Emit(OpCodes.Ret);

            propertyBuilder.SetSetMethod(setMethodBuilder);
        }

        private static void AddSetterTransmitterSaveMethod(ILGenerator setIlGenerator, Type propertyType)
        {
            var saveChangeMethod = typeof(Transmitter).GetMethod("SaveChange", BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(propertyType); // TODO: FIX. Here is strong base type.
            setIlGenerator.Emit(OpCodes.Ldarg_0);
            setIlGenerator.Emit(OpCodes.Ldc_I4, 1337); // TODO: Property id.
            setIlGenerator.Emit(OpCodes.Ldarg_1);
            setIlGenerator.Emit(OpCodes.Callvirt, saveChangeMethod);
        }

        private static void AddMethod(TypeBuilder typeBuilder)
        {
            // Define a method that accepts an integer argument and returns
            // the product of that integer and the private field m_number. This
            // time, the array of parameter types is created on the fly.
            MethodBuilder meth = typeBuilder.DefineMethod(
                "MyMethod",
                MethodAttributes.Public,
                typeof(int),
                new Type[] { typeof(int) });

            ILGenerator methIL = meth.GetILGenerator();
            // To retrieve the private instance field, load the instance it
            // belongs to (argument zero). After loading the field, load the
            // argument one and then multiply. Return from the method with
            // the return value (the product of the two numbers) on the
            // execution stack.
            methIL.Emit(OpCodes.Ldarg_0);
            //methIL.Emit(OpCodes.Ldfld, fbNumber);
            methIL.Emit(OpCodes.Ldarg_1);
            methIL.Emit(OpCodes.Mul);
            methIL.Emit(OpCodes.Ret);
        }
    }
}
