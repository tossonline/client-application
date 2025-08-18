using System;
using Analytics.Domain.Abstractions;
using System.Collections.Generic;
using System.Linq;

namespace Analytics.Domain.Entities.Common
{
    /// <summary>
    /// Value object representing an event type
    /// </summary>
    public class EventType : IEquatable<EventType>
    {
        private static readonly HashSet<string> ValidEventTypes = new()
        {
            "visit",
            "registration",
            "deposit"
        };

        /// <summary>
        /// Gets the event type value
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Private constructor for value object
        /// </summary>
        /// <param name="value">The event type value</param>
        private EventType(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Creates a new EventType value object
        /// </summary>
        /// <param name="value">The event type value</param>
        /// <returns>A new EventType instance</returns>
        /// <exception cref="ArgumentException">Thrown when value is not a valid event type</exception>
        public static EventType Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Event type cannot be null or empty", nameof(value));

            if (!ValidEventTypes.Contains(value.ToLowerInvariant()))
                throw new ArgumentException($"Invalid event type. Must be one of: {string.Join(", ", ValidEventTypes)}", nameof(value));

            return new EventType(value.ToLowerInvariant());
        }

        /// <summary>
        /// Creates a visit event type
        /// </summary>
        public static EventType Visit => Create("visit");

        /// <summary>
        /// Creates a registration event type
        /// </summary>
        public static EventType Registration => Create("registration");

        /// <summary>
        /// Creates a deposit event type
        /// </summary>
        public static EventType Deposit => Create("deposit");

        /// <summary>
        /// Determines if the event type is valid
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <returns>True if valid, false otherwise</returns>
        public static bool IsValid(string value)
        {
            return !string.IsNullOrWhiteSpace(value) && ValidEventTypes.Contains(value.ToLowerInvariant());
        }

        /// <summary>
        /// Gets all valid event types
        /// </summary>
        public static IReadOnlyCollection<string> ValidTypes => ValidEventTypes.ToList().AsReadOnly();

        /// <summary>
        /// Implicit conversion to string
        /// </summary>
        public static implicit operator string(EventType eventType) => eventType.Value;

        /// <summary>
        /// Explicit conversion from string
        /// </summary>
        public static explicit operator EventType(string value) => Create(value);

        /// <summary>
        /// Equality comparison
        /// </summary>
        public bool Equals(EventType? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Value == other.Value;
        }

        /// <summary>
        /// Override equals
        /// </summary>
        public override bool Equals(object? obj)
        {
            return Equals(obj as EventType);
        }

        /// <summary>
        /// Get hash code
        /// </summary>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        /// <summary>
        /// To string
        /// </summary>
        public override string ToString()
        {
            return Value;
        }

        /// <summary>
        /// Equality operator
        /// </summary>
        public static bool operator ==(EventType? left, EventType? right)
        {
            return EqualityComparer<EventType>.Default.Equals(left, right);
        }

        /// <summary>
        /// Inequality operator
        /// </summary>
        public static bool operator !=(EventType? left, EventType? right)
        {
            return !(left == right);
        }
    }
}
