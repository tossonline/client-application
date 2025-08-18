using System;
using Analytics.Domain.Abstractions;
using System.Collections.Generic;

namespace Analytics.Domain.Entities.Common
{
    /// <summary>
    /// Value object representing a banner tag
    /// </summary>
    public class BannerTag : IEquatable<BannerTag>
    {
        /// <summary>
        /// Gets the banner tag value
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Gets the campaign ID extracted from the banner tag
        /// </summary>
        public string CampaignId { get; }

        /// <summary>
        /// Gets the placement extracted from the banner tag
        /// </summary>
        public string? Placement { get; }

        /// <summary>
        /// Gets the banner size extracted from the banner tag
        /// </summary>
        public string? Size { get; }

        /// <summary>
        /// Private constructor for value object
        /// </summary>
        /// <param name="value">The banner tag value</param>
        private BannerTag(string value)
        {
            Value = value;
            var parts = value.Split('-');
            CampaignId = parts.Length > 0 ? parts[0] : value;
            Placement = parts.Length > 1 ? parts[1] : null;
            Size = parts.Length > 2 ? parts[2] : null;
        }

        /// <summary>
        /// Creates a new BannerTag value object
        /// </summary>
        /// <param name="value">The banner tag value</param>
        /// <returns>A new BannerTag instance</returns>
        /// <exception cref="ArgumentException">Thrown when value is null or empty</exception>
        public static BannerTag Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Banner tag cannot be null or empty", nameof(value));

            if (value.Length > 255)
                throw new ArgumentException("Banner tag cannot exceed 255 characters", nameof(value));

            return new BannerTag(value.Trim());
        }

        /// <summary>
        /// Creates a banner tag from campaign components
        /// </summary>
        /// <param name="campaignId">The campaign identifier</param>
        /// <param name="placement">Optional placement identifier</param>
        /// <param name="size">Optional banner size</param>
        /// <returns>A new BannerTag instance</returns>
        public static BannerTag CreateFromComponents(string campaignId, string? placement = null, string? size = null)
        {
            if (string.IsNullOrWhiteSpace(campaignId))
                throw new ArgumentException("Campaign ID cannot be null or empty", nameof(campaignId));

            var components = new List<string> { campaignId };
            if (!string.IsNullOrWhiteSpace(placement))
                components.Add(placement);
            if (!string.IsNullOrWhiteSpace(size))
                components.Add(size);

            return Create(string.Join("-", components));
        }

        /// <summary>
        /// Determines if the banner tag is valid
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <returns>True if valid, false otherwise</returns>
        public static bool IsValid(string value)
        {
            return !string.IsNullOrWhiteSpace(value) && value.Length <= 255;
        }

        /// <summary>
        /// Implicit conversion to string
        /// </summary>
        public static implicit operator string(BannerTag bannerTag) => bannerTag.Value;

        /// <summary>
        /// Explicit conversion from string
        /// </summary>
        public static explicit operator BannerTag(string value) => Create(value);

        /// <summary>
        /// Equality comparison
        /// </summary>
        public bool Equals(BannerTag? other)
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
            return Equals(obj as BannerTag);
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
        public static bool operator ==(BannerTag? left, BannerTag? right)
        {
            return EqualityComparer<BannerTag>.Default.Equals(left, right);
        }

        /// <summary>
        /// Inequality operator
        /// </summary>
        public static bool operator !=(BannerTag? left, BannerTag? right)
        {
            return !(left == right);
        }
    }
}
