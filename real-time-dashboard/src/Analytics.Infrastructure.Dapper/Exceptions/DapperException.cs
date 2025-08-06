// Copyright (c) DigiOutsource. All rights reserved.

using System.Runtime.Serialization;

namespace Analytics.Infrastructure.Dapper.Exceptions
{
    [Serializable]
    public sealed class DapperException : Exception
    {
        public DapperException()
        {
        }

        public DapperException(string message)
            : base(message)
        {
        }

        public DapperException(string message, Exception inner)
            : base(message, inner)
        {
        }

        [Obsolete("Obsolete")]
        private DapperException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}