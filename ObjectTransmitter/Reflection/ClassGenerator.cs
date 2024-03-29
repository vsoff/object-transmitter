﻿using ObjectTransmitter.Collectors;
using ObjectTransmitter.Extensions;
using ObjectTransmitter.Reflection.Models;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace ObjectTransmitter.Reflection
{
    internal static class ClassGenerator
    {
        public static Type GenerateTransmitter(TypeDescription typeDescription)
        { 
            if (typeDescription == null) throw new ArgumentNullException(nameof(typeDescription));
            TypeValidator.ThrowIfTypeInvalid(typeDescription.Type);

            var moduleBuilder = CreateModuleBuilder();
            TypeBuilder typeBuilder = CreateTypeBuilder(moduleBuilder, typeDescription);

            // Implement interface.
            typeBuilder.AddInterfaceImplementation(typeDescription.Type);
            foreach (var propertyDescription in typeDescription.Properties)
            {
                AddProperty(typeBuilder, propertyDescription);
            }
            
            // Create type.
            Type generatedType = typeBuilder.CreateTypeInfo();
            return generatedType;
        }

        private static TypeBuilder CreateTypeBuilder(ModuleBuilder moduleBuilder, TypeDescription typeDescription)
        {
            var generatedName = GetGeneratedName(typeDescription);
            return moduleBuilder.DefineType(generatedName, TypeAttributes.Public, typeof(Transmitter));
        }

        private static string GetGeneratedName(TypeDescription typeDescription)
        {
            var baseName = typeDescription.Type.Name.Substring(1);
            return $"{baseName}_{Guid.NewGuid():N}";
        }

        private static ModuleBuilder CreateModuleBuilder()
        {
            var generatedAssemblyName = $"{nameof(ObjectTransmitter)}.Generated";
            AssemblyName assemblyName = new AssemblyName(generatedAssemblyName);
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name);

            return moduleBuilder;
        }

        private static void AddProperty(TypeBuilder typeBuilder, PropertyDescription propertyDescription)
        {
            FieldBuilder fieldBuilder = typeBuilder.DefineField($"_{propertyDescription.Name}", propertyDescription.Type, FieldAttributes.Private);

            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(propertyDescription.Name, PropertyAttributes.HasDefault, propertyDescription.Type, null);
            MethodAttributes methodAttributes = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Final;

            // Configure getter.
            MethodBuilder getMethodBuilder = typeBuilder.DefineMethod($"get_{propertyDescription.Name}", methodAttributes, propertyDescription.Type, Type.EmptyTypes);
            ILGenerator getIlGenerator = getMethodBuilder.GetILGenerator();
            getIlGenerator.Emit(OpCodes.Ldarg_0);
            getIlGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
            getIlGenerator.Emit(OpCodes.Ret);
            propertyBuilder.SetGetMethod(getMethodBuilder);

            // Configure setter.
            MethodBuilder setMethodBuilder = typeBuilder.DefineMethod($"set_{propertyDescription.Name}", methodAttributes, null, new Type[] { propertyDescription.Type });
            ILGenerator setIlGenerator = setMethodBuilder.GetILGenerator();
            setIlGenerator.Emit(OpCodes.Ldarg_0);
            setIlGenerator.Emit(OpCodes.Ldarg_1);
            setIlGenerator.Emit(OpCodes.Stfld, fieldBuilder);
            AppendSetterMethodAdditionalCall(setIlGenerator, propertyDescription);
            setIlGenerator.Emit(OpCodes.Ret);
            propertyBuilder.SetSetMethod(setMethodBuilder);
        }

        private static void AppendSetterMethodAdditionalCall(ILGenerator setIlGenerator, PropertyDescription propertyDescription)
        {
            var saveChangeMethod = typeof(Transmitter).GetMethod(Transmitter.SaveChangeMethodName, BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(propertyDescription.Type);
            setIlGenerator.Emit(OpCodes.Ldarg_0);
            setIlGenerator.Emit(OpCodes.Ldc_I4, propertyDescription.PropertyId);
            setIlGenerator.Emit(OpCodes.Ldarg_1);
            setIlGenerator.Emit(OpCodes.Callvirt, saveChangeMethod);
        }
    }
}
