using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectTransmitter.Collectors
{
    public class Transmitter
    {
        internal const string SaveChangeMethodName = nameof(SaveChange);

        protected void SaveChange<T>(int propertyId, T newValue)
        {

        }
    }
}
