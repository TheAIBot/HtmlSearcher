using System;
using System.Runtime.Serialization;

namespace HtmlSearcher
{
    [Serializable]
    public class ExpectedException : Exception
    {
        public ExpectedException(string message) : base(message) { }

        protected ExpectedException(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) { }
    }

    [Serializable]
    public class ExceptionNotImplementedYet : Exception
    {
        public ExceptionNotImplementedYet() { }

        public ExceptionNotImplementedYet(string message) : base(message) { }

        protected ExceptionNotImplementedYet(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) { }
    }
}
