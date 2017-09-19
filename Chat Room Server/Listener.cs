using System;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using ChatRoom.Utilities;
using ChatRoom.Server.Properties;
using System.Threading.Tasks;

namespace ChatRoom.Server
{
    /// <summary>
    /// A process that will listen for new client's to connect, 
    /// and will create a new <seealso cref="Client"/> for each new connection.
    /// </summary>
    /// <author>Kodey Converse (krconverse@wpi.edu)</author>
    class Listener
    {
        /// <summary>
        /// The port that the client listener will listen on.
        /// </summary>
        private readonly int port;
        /// <summary>
        /// Boolean indicating whether the listener is currently listening for connections.
        /// </summary>
        private bool listening = false;
        /// <summary>
        /// The thread that the listener will be listening on (when it is listening).
        /// </summary>
        private Thread thread;
        /// <summary>
        /// The tcp listener that will be listening for connections.
        /// </summary>
        private TcpListener listener;
        /// <summary>
        /// The handler to send new client connections to.
        /// </summary>
        private ServerMessageHandler handler;

        /// <summary>
        /// Creates a new listener for new connections to the server.
        /// </summary>
        /// <param name="port">Port that the listener will listen on.</param>
        /// <param name="handler">The handler that this will send new connections to.</param>
        public Listener(int port, ServerMessageHandler handler)
        {
            this.port = port;
            this.handler = handler;
        }

        /// <summary>
        /// Tells the client listener to start listening for new connections.
        /// </summary>
        public void Start()
        {
            // set up the socket for the listener to use
            try
            {
                listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
                // bind to an IP address
                Logger.Log(Resources.BindSuccessfulUnfmt, IPAddress.Any, port);
            }
            catch (SocketException ex)
            {
                Logger.Log(Resources.BindFailedUnfmt, IPAddress.Any, port, ex.Message);
                throw;
            }

            // start to listen asyncronously
            listening = true;
            thread = new Thread(new ThreadStart(Listen));
            thread.Start();
        }

        /// <summary>
        /// Listen for new connections. This will continue to loop and 
        /// handle connections until <see cref="listening"/> is falsified.
        /// </summary>
        private void Listen()
        {
            Logger.Log(Resources.Listening);
            while (listening)
            {
                TcpClient accepted = listener.AcceptTcpClient();
                Logger.Log(Resources.NewConnectionUnfmt, accepted.Client.RemoteEndPoint);
                new Task(() => handler.AddClient(accepted)).Start();
            }
        }
    }
}