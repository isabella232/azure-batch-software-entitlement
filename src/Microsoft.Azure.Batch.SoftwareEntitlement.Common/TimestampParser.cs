using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Microsoft.Azure.Batch.SoftwareEntitlement.Common
{
    /// <summary>
    /// Utility class for conversion of dates from strings
    /// </summary>
    /// <remarks>Wraps our specific conventions and knows about logging for diagnostics.</remarks>
    public class TimestampParser
    {
        // The expected format for timestamps
        public const string ExpectedFormat = "yyyy-MM-ddTHH:mm";

        /// <summary>
        /// Try to parse a string into a DateTimeOffset
        /// </summary>
        /// <param name="value">The string value to parse.</param>
        /// <param name="name">Name to use for the value if there is an error.</param>
        /// <returns>A succesfully parsed <see cref="DateTimeOffset"/> or errors detailing what
        /// went wrong.</returns>
        [SuppressMessage(
            "Performance",
            "CA1822:Mark members as static",
            Justification = "This method should not be static.")]
        public Result<DateTimeOffset, ErrorSet> TryParse(string value, string name)
        {
            if (DateTimeOffset.TryParseExact(
                value, ExpectedFormat, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var result))
            {
                // Correctly parsed in the expected format
                return result;
            }

            if (DateTimeOffset.TryParse(
                value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out result))
            {
                // Correctly parsed with detected format
                return result;
            }

            if (DateTimeOffset.TryParse(value, out result))
            {
                // Correctly parsed with detected format
                return result;
            }

            return ErrorSet.Create(
                $"Unable to parse {name} timestamp '{value}' (expected format is '{ExpectedFormat}')");
        }
    }
}
