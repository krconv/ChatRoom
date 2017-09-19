using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Windows.Data;
using ChatRoom.Properties;
using ChatRoom.Utilities;
using System.Collections.Generic;

namespace ChatRoom
{
    /// <summary>
    /// The main message handler for the client. This handles all messages ingoing and outgoing,
    /// and keeps track of information about the current chat session.
    /// </summary>
    /// <author>Kodey Converse (krconverse@wpi.edu)</author>
    class ClientMessageHandler : MessageHandler
    {
        /// <summary>
        /// The server currently connected to.
        /// </summary>
        public Server Server
        {
            get;
            private set;
        }

        /// <summary>
        /// The recently sent messages in the chat.
        /// </summary>
        public ObservableCollection<Message> MessageHistory
        {
            get;
            private set;
        }

        /// <summary>
        /// A lock object for being about to asyncronously update UI bound sources.
        /// </summary>
        private static object _lock = new object();


        /// <summary>
        /// Creates a new empty chat room.
        /// </summary>
        public ClientMessageHandler()
        {
            Users = new ObservableCollection<User>();
            BindingOperations.EnableCollectionSynchronization(Users, _lock);
            MessageHistory = new ObservableCollection<Message>();
            BindingOperations.EnableCollectionSynchronization(MessageHistory, _lock);

            // prompt the user for a server to connect to
            MainWindow.Instance.ShowDialog(Resources.EnterAServerHost, (h) => ConnectToServer(h, ConfigurationManager.GetServerPort()));
        }

        /// <summary>
        /// Connect to a chat server on the given host. If the server can't be connected to, 
        /// prompt the user for a server to connect to.
        /// </summary>
        /// <param name="host">Host to connect to.</param>
        /// <param name="port">Port that the server is running on.</param>
        private void ConnectToServer(string host, int port)
        {
            TcpClient server = new TcpClient();
            try
            {
                server.Connect(host, port);
                Server = new Server(server, this);
                MainWindow.Instance.ConnectedStatus.Text = String.Format(Resources.StatusConnectedUnfmt, host);
            }
            catch (Exception ex)
            {
                if (ex is SocketException || ex is ObjectDisposedException)
                {
                    // if the given host failed, warn the user and prompt again
                    MainWindow.Instance.ShowDialog(String.Format(Resources.ErrorMessage, ex.Message),
                        () => MainWindow.Instance.ShowDialog(Resources.EnterAServerHost, (h) => ConnectToServer(h, ConfigurationManager.GetServerPort())));
                }
            }
        }

        /// <summary>
        /// Sends a message.
        /// </summary>
        /// <param name="message">The message to send.</param>
        public override void SendMessage(Message message)
        {
            Server.SendMessage(message);
        }

        /// <summary>
        /// Processes the given inbound message, adding it to chat history or performing an action.
        /// </summary>
        /// <param name="message">Message to process.</param>
        public override void RecieveMessage(Message message)
        {
            if (message != null)
                if (message.Sender == User.Server)
                { // only process a message if it's from the server
                    if (message.Type == Message.MessageType.Update)
                    {
                        // recieved an update to the connected user list
                        UpdateMessage update = (message as UpdateMessage);
                        if (update.ExtendedType == UpdateMessage.UpdateType.Joined
                            || update.ExtendedType == UpdateMessage.UpdateType.Present)
                        {
                            Users.Add(update.User);
                        }
                        else if (update.ExtendedType == UpdateMessage.UpdateType.Left)
                        {
                            Users.Remove(update.User);
                        }
                        MessageHistory.Add(update);
                    }
                }
                else
                    if (message.Type == Message.MessageType.ChatMessage)
                    MessageHistory.Add(message);
        }

        /// <summary>
        /// Stop this client from processing anything else and disconnect from the server.
        /// </summary>
        public override void Stop()
        {
            if (Server != null)
                Server.Disconnect();
        }
    }
}
