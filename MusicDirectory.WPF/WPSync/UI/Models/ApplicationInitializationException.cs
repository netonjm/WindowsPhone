namespace Microsoft.WPSync.UI
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class ApplicationInitializationException : Exception
    {
        public ApplicationInitializationException()
        {
        }

        public ApplicationInitializationException(string type) : base(type)
        {
        }

        protected ApplicationInitializationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ApplicationInitializationException(string type, Exception exception) : base(type, exception)
        {
        }
    }
}

