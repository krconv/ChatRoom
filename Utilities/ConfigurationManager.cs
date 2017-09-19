using ChatRoom.Utilities.Properties;
using System;
using System.Collections.Specialized;
using System.Configuration;

namespace ChatRoom.Utilities
{
    /// <summary>
    /// Contains global configuration defaults.
    /// </summary>
    /// <author>Kodey Converse (krconverse@wpi.edu)</author>
    public class ConfigurationManager
    {
        /// <summary>
        /// Retrieves the default port that the chat server will run on.
        /// </summary>
        /// <returns>The default port that the chat server will run on.</returns>
        public static int GetServerPort()
        {
            return Properties.Settings.Default.port;
        }
    }
}
