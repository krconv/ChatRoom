using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatRoom.Utilities
{
    /// <summary>
    /// Represents the situation in which a message could not be reconstructed from a byte array.
    /// </summary>
    /// <author>Kodey Converse (krconverse@wpi.edu)</author>
    [Serializable]
    public class InvalidMessageException : Exception
    {
        /// <summary>
        /// Creates a new exception in which a message could not be reconstructed from a byte array.
        /// </summary>
        public InvalidMessageException()
            : base()
        { }

        /// <summary>
        /// Creates a new exception in which a message could not be reconstructed from a byte array and the situation is exceptional due to the given message.
        /// </summary>
        /// <param name="message">Message explaining why the situation is exceptional.</param>
        public InvalidMessageException(string message)
            : base(message)
        { }

        /// <summary>
        /// Creates a new exception in which a message could not be reconstructed from a byte array caused by the given exception,
        /// and the situation is exceptional due to the given message.
        /// </summary>
        /// <param name="message">Message explaining why the situation is exceptional.</param>
        /// <param name="innerException">The exception that caused this exception.</param>
        public InvalidMessageException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
