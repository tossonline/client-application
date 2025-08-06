// Copyright (c) DigiOutsource. All rights reserved.

using System.Runtime.Serialization;

namespace Analytics.Infrastructure.Persistence.Exceptions
{
    [Serializable]
    public sealed class UnitOfWorkException : Exception
    {
        public UnitOfWorkException()
        {
        }

        public UnitOfWorkException(string message)
            : base(message)
        {
        }

        public UnitOfWorkException(string message, Exception inner)
            : base(message, inner)
        {
        }

        [Obsolete("Obsolete")]
        private UnitOfWorkException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}