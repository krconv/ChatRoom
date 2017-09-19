using ChatRoom.Properties;
using ChatRoom.Utilities;
using System.Net.Sockets;
using System.Windows;
using System.Linq;

namespace ChatRoom
{
    /// <summary>
    /// Manages the connection between this client and the server.
    /// </summary>
    class Server : RemoteSource
    {

        /// <summary>
        /// Creates a new handler to handle the server connected on the given socket.
        /// </summary>
        /// <param name="accepted">An accepted connection that is connected to a server.</param>
        /// <param name="handler">The handler that will handle all message interactions.</param>
        public Server(TcpClient accepted, ClientMessageHandler handler)
            : base(accepted, handler)
        {
            Message recieved = RecieveMessage();
            User.Me = new User(recieved.Recipient.Id);
            if (recieved.Body.Equals(Message.ChooseInstruction))
                ChooseNickname();
        }

        /// <summary>
        /// Enter a conversation with the server to choose a nickname and user ID.
        /// </summary>
        /// <param name="another">If the user should be told to "Try Again".</param>
        private void ChooseNickname(bool another = false)
        {
            if (!another)
                MainWindow.Instance.ShowDialog(Resources.ChooseNickname, (n) => SubmitNickname(n));
            else
                MainWindow.Instance.ShowDialog(Resources.ChooseNicknameAgain, (n) => SubmitNickname(n));

        }

        /// <summary>
        /// Submits the given nickname to the server to use, 
        /// and prompts the user to choose again if the server denies it.
        /// If the server accepts the username, server messages will start being listened for.
        /// </summary>
        /// <param name="nickname">Nickname to submit.</param>
        private void SubmitNickname(string nickname)
        {
            SendMessage(new Message(User.Server, User.Me, Message.MessageType.Reply, nickname));
            Message recieved = null;
            try
            {
                recieved = RecieveMessage();
            }
            catch (InvalidMessageException) { }

            if (recieved == null || recieved.Body.Equals(Message.ChooseInstruction))
            {
                // need to pick another username
                ChooseNickname(true);
                return;
            }
            else
            {
                User.Me.SetNickname(nickname);
                handler.RecieveMessage(recieved);
                active = true;
                thread.Start();
                return;
            }
        }

        /// <summary>
        /// Continuously listen for the server to send a message, then pass the message onto the message handler.
        /// </summary>
        protected override void Listen()
        {
            while (active)
            {
                try
                {
                    Message message = RecieveMessage();
                    handler.RecieveMessage(message);
                }
                catch (InvalidMessageException)
                {
                    // ignore an invalid message and listening
                }
                catch (RemoteSourceDisconnectedException)
                {
                    Disconnect();
                    return;
                }
            }
        }

        /// <summary>
        /// Frees up resources used to connect to the server and alert the message handler that it's been disconnected.
        /// </summary>
        public override void Disconnect()
        {
            base.Disconnect();
            if (MainWindow.Instance != null)
            {
                MainWindow.Instance.ConnectedStatus.Text = Resources.StatusNotConnected;
                Application.Current.Dispatcher.Invoke(
                    () => MainWindow.Instance.ShowDialog(Resources.ServerDisconnected, Application.Current.MainWindow.Close));
            }
        }
    }
}
