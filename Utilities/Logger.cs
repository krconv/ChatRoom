using ChatRoom.Utilities.Properties;
using System;

namespace ChatRoom.Utilities
{
    /// <summary>
    /// Handles all logging.
    /// </summary>
    /// <author>Kodey Converse (krconverse@wpi.edu)</author>
    public static class Logger
    {
        /// <summary>
        /// Log a message containing information about the state of the program.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="arg">Arguments for a formatted string.</param>
        public static void Log(string message, params object[] arg)
        {
            System.Console.WriteLine(message, arg);
        }

        /// <summary>
        /// Log a message containing information about the state of the program.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void Log(string message)
        {
            Log(message, null);
        }

        /// <summary>
        /// Log a message containing information about the state of the program.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="successful">Whether the message was successful.</param>
        public static void Log(Message message, bool successful)
        {
            if (message != null)
                if (successful)
                    if (message.Recipient == User.All)
                        Log(Resources.LogMessageToAllUnfmt, message.Sender, message.Body);
                    else
                        Log(Resources.LogMessageToSpecificUnfmt, message.Sender, message.Recipient, message.Body);
                else
                    Log(Resources.LogMessageFailed, message.Sender, message.Recipient, message.Type.ToString(), message.Body);
        }
    }
}