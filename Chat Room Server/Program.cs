using System;
using System.Configuration;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatRoom.Utilities;
using ChatRoom.Server.Properties;

namespace ChatRoom.Server
{
    /// <summary>
    /// The Chat Room Server program. This initializes the server.
    /// </summary>
    class Program
    {
        /// <summary>
        /// The listener that will listen for new connections.
        /// </summary>
        private static Listener listener;
        /// <summary>
        /// The handler that will handle all message interaction for the chat room.
        /// </summary>
        private static ServerMessageHandler handler;

        /// <summary>
        /// Create and start the chat room server.
        /// </summary>
        /// <param name="args">Unused.</param>
        static void Main(string[] args)
        {
            Logger.Log(Resources.StartingServer);
            handler = new ServerMessageHandler();
            listener = new Listener(Utilities.ConfigurationManager.GetServerPort(), handler);
            listener.Start();
        }
    }
}
