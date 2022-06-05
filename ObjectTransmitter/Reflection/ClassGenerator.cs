using ObjectTransmitter.Collectors;
using ObjectTransmitter.Exceptions;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace ObjectTransmitter.Reflection
{
    internal static class ClassGenerator
    {

        public static TInterface GenerateTransmitter<TInterface>() => GenerateClass<TInterface>(ClassType.Transmitter);
        public static TInterface GenerateRepeater<TInterface>() => GenerateClass<TInterface>(ClassType.Repeater);
        public static TInterface GenerateContract<TInterface>() => GenerateClass<TInterface>(ClassType.Contract);

        private static TInterface GenerateClass<TInterface>(ClassType classType)
        {
            var interfaceType = typeof(TInterface);
            if (!interfaceType.IsInterface)
                throw new ObjectTransmitterException("Type should be interface");

            var moduleBuilder = CreateModuleBuilder();
            TypeBuilder typeBuilder = CreateTypeBuilder<TInterface>(moduleBuilder, classType);

            // Implement interface.
            typeBuilder.AddInterfaceImplementation(interfaceType);
            foreach (var propertyInfo in interfaceType.GetProperties())
            {
                AddProperty(typeBuilder, propertyInfo.Name, propertyInfo.PropertyType, classType);
            }
            
            // Create type.
            Type generatedType = typeBuilder.CreateTypeInfo();

            // Create instance.
            var instance = (TInterface)Activator.CreateInstance(generatedType);
            return instance;
        }

        private static TypeBuilder CreateTypeBuilder<TInterface>(ModuleBuilder moduleBuilder, ClassType classType)
        {
            var generatedName = GetGeneratedName<TInterface>(classType);
            switch (classType)
            {
                case ClassType.Transmitter: return moduleBuilder.DefineType(generatedName, TypeAttributes.Public, typeof(Transmitter));
                default: throw new ArgumentOutOfRangeException(nameof(classType), $"Unknown class type: {classType}");
            }
        }

        private static string GetGeneratedName<TInterface>(ClassType classType)
        {
            var baseName = typeof(TInterface).Name.Substring(1);
            return $"{baseName}_{classType}_{Guid.NewGuid():N}";
        }

        private static ModuleBuilder CreateModuleBuilder()
        {
            var generatedAssemblyName = $"{nameof(ObjectTransmitter)}.Generated";
            AssemblyName assemblyName = new AssemblyName(generatedAssemblyName);
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name);

            return moduleBuilder;
        }

        private static void AddProperty(TypeBuilder typeBuilder, string propertyName, Type propertyType, ClassType classType)
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
            AppendSetterMethodAdditionalCall(setIlGenerator, propertyType, classType);
            setIlGenerator.Emit(OpCodes.Ret);
            propertyBuilder.SetSetMethod(setMethodBuilder);
        }

        private static void AppendSetterMethodAdditionalCall(ILGenerator setIlGenerator, Type propertyType, ClassType classType)
        {
            switch (classType)
            {
                case ClassType.Transmitter:
                    {
                        var saveChangeMethod = typeof(Transmitter).GetMethod(Transmitter.SaveChangeMethodName, BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(propertyType);
                        setIlGenerator.Emit(OpCodes.Ldarg_0);
                        setIlGenerator.Emit(OpCodes.Ldc_I4, 1337); // TODO: Property id.
                        setIlGenerator.Emit(OpCodes.Ldarg_1);
                        setIlGenerator.Emit(OpCodes.Callvirt, saveChangeMethod);
                        break;
                    }
                default: throw new ArgumentOutOfRangeException(nameof(classType), $"Unknown class type: {classType}");
            }
        }

        private enum ClassType
        {
            Contract,
            Transmitter,
            Repeater
        }
    }
}
