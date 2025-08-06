// Copyright (c) DigiOutsource. All rights reserved.

using System.Runtime.Serialization;

namespace Analytics.Application.BackgroundJobs.Exceptions
{
    [Serializable]
    public sealed class BackgroundJobException : Exception
    {
        public BackgroundJobException(string message)
            : base(message)
        {
        }

        public BackgroundJobException(string message, Exception inner)
            : base(message, inner)
        {
        }

        [Obsolete("Obsolete")]
        private BackgroundJobException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}