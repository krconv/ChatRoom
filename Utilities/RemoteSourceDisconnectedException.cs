using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatRoom.Utilities
{
    /// <summary>
    /// Represents the situation where the remote source was disconnected and therefore exceptional actions are required.
    /// </summary>
    /// <author>Kodey Converse (krconverse@wpi.edu)</author>
    [Serializable]
    public class RemoteSourceDisconnectedException : Exception
    {
        /// <summary>
        /// Creates a new exception in which a remote source disconnected where an active client connection was needed.
        /// </summary>
        public RemoteSourceDisconnectedException() :
            base()
        { }

        /// <summary>
        /// Creates a new exception in which a remote source disconnected, and the situation is exceptional due to the given message.
        /// </summary>
        /// <param name="message">Message explaining why the situation is exceptional.</param>
        public RemoteSourceDisconnectedException(string message)
            : base(message)
        { }

        /// <summary>
        /// Creates a new exception in which the given exception caused this exception, and the situation is exceptional due to the given message.
        /// </summary>
        /// <param name="message">Message explaining why the situation is exceptional.</param>
        /// <param name="innerException">The exception that caused this exception.</param>
        public RemoteSourceDisconnectedException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
