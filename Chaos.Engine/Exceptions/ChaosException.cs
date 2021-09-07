using System;
using System.Runtime.Serialization;

namespace Chaos.Engine.Exceptions
{
    [Serializable]
    public class ChaosException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public ChaosException()
        {
        }

        public ChaosException(string message) : base(message)
        {
        }

        public ChaosException(string message, Exception inner) : base(message, inner)
        {
        }

        protected ChaosException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
