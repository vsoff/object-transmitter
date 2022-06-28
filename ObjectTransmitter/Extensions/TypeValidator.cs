using ObjectTransmitter.Exceptions;
using System;
using System.Linq;

namespace ObjectTransmitter.Extensions
{
    internal static class TypeValidator
    {
        public static void ThrowIfTypeInvalid<TInterface>() => ThrowIfTypeInvalid(typeof(TInterface));

        public static void ThrowIfTypeInvalid(Type interfaceType)
        {
            if (!interfaceType.IsInterface)
                throw new ObjectTransmitterException("Type should be interface");

            if (interfaceType.GetFields().Any())
                throw new ObjectTransmitterException("Interface shouldn't has fields");

            // TODO
            //if (interfaceType.GetMethods().Any())
            //    throw new ObjectTransmitterException("Interface shouldn't has methods");

            foreach (var propertyInfo in interfaceType.GetProperties())
            {
                if (!propertyInfo.GetGetMethod().IsPublic || !propertyInfo.GetSetMethod().IsPublic)
                    throw new ObjectTransmitterException("Interface properties should has public get and set methods");
            }
        }
    }
}
