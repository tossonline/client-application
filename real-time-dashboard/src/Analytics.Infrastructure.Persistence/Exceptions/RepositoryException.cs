// Copyright (c) DigiOutsource. All rights reserved.

using System.Runtime.Serialization;

namespace Analytics.Infrastructure.Persistence.Exceptions
{
    [Serializable]
    public sealed class RepositoryException : Exception
    {
        public RepositoryException()
        {
        }

        public RepositoryException(string message)
            : base(message)
        {
        }

        public RepositoryException(string message, Exception inner)
            : base(message, inner)
        {
        }

        [Obsolete("Obsolete")]
        private RepositoryException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}