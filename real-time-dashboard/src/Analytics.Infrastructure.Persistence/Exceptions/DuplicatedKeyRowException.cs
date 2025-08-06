// Copyright (c) DigiOutsource. All rights reserved.

using System.Runtime.Serialization;

namespace Analytics.Infrastructure.Persistence.Exceptions
{
    [Serializable]
    public sealed class DuplicatedKeyRowException : Exception
    {
        public DuplicatedKeyRowException()
        {
        }

        public DuplicatedKeyRowException(string message)
            : base(message)
        {
        }

        public DuplicatedKeyRowException(string message, Exception inner)
            : base(message, inner)
        {
        }

        [Obsolete("Obsolete")]
        private DuplicatedKeyRowException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}