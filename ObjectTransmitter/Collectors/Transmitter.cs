namespace ObjectTransmitter.Collectors
{
    public class Transmitter
    {
        internal const string SaveChangeMethodName = nameof(SaveChange);

        protected void SaveChange<T>(int propertyId, T newValue)
        {

        }
    }

    public class Repeater
    {
        internal const string PropertyChangedMethodName = nameof(PropertyChanged);

        protected void PropertyChanged<T>(int propertyId, T newValue)
        {
            
        }
    }
}
