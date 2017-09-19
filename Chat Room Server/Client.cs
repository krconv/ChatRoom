using System;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using ChatRoom.Utilities;
using System.IO;
using ChatRoom.Server.Properties;

namespace ChatRoom.Server
{
    /// <summary>
    /// A handler which is tied to a single remote user and handles all communication between the client and server.
    /// </summary>
    /// <author>Kodey Converse (krconverse@wpi.edu)</author>
    class Client : RemoteSource
    {
        /// <summary>
        /// The user that this client is using.
        /// </summary>
        public readonly User User;

        /// <summary>
        /// Creates a new client connected on the given socket.
        /// </summary>
        /// <param name="accepted">An accepted connection that is connected to a client.</param>
        /// <param name="handler">The handler that will handle all message interactions.</param>
        public Client(TcpClient accepted, ServerMessageHandler handler)
            : base(accepted, handler)
        {
            // ask the user to pick a nickname
            int userID = handler.GenerateUserID();
            User = new User(userID);
            User.SetNickname(ChooseNickname());

            // update the client with the list of currently connected users
            UpdateClient();

            // start to listen asyncronously
            active = true;
            thread.Start();
        }

        /// <summary>
        /// Prompt the currently connected client to choose a nickname that hasn't been taken yet.
        /// </summary>
        /// <return>Returns the valid nickname that the client chooses.</return>
        /// <exception cref="RemoteSourceDisconnectedException">Thrown if the user disconnects while selecting a nickname.</exception>
        private string ChooseNickname()
        {
            string nickname = null;
            do
            {
                try
                {
                    // prompt the user again for a username
                    SendMessage(new Message(User, User.Server, Message.MessageType.Instruction, Message.ChooseInstruction));
                    nickname = RecieveMessage().Body;
                }
                catch (InvalidMessageException) { }
            } while (!String.IsNullOrWhiteSpace(nickname)
                 && (handler.Users.Any((u) => u.Nickname.Equals(nickname))
                 || nickname.Length > User.MaxNicknameLength));
            return nickname;
        }

        /// <summary>
        /// Continuously listen for the client to send a message, then pass recieved messages onto the message handler.
        /// </summary>
        protected override void Listen()
        {
            while (active)
            {
                try
                {
                    Message message = RecieveMessage();
                    // make sure user didn't forge a sender field, then process it
                    if (message.Sender == User)
                        handler.RecieveMessage(message);
                }
                catch (InvalidMessageException ex)
                {
                    Logger.Log(Resources.InvalidMessageUnfmt, User, ex.Message);
                }
                catch (RemoteSourceDisconnectedException)
                {
                    Logger.Log(Resources.UserDisconnectedUnfmt, User);
                    Disconnect();
                }
            }
        }

        /// <summary>
        /// Updates the client with a current list of connected users.
        /// </summary>
        protected virtual void UpdateClient()
        {
            foreach (User user in handler.Users)
            {
                try
                {
                    UpdateMessage update = new UpdateMessage(UpdateMessage.UpdateType.Present, user);
                    SendMessage(update);
                }
                catch (RemoteSourceDisconnectedException)
                {
                    Logger.Log(Resources.UserDisconnectedUnfmt, User.Nickname);
                    Disconnect();
                    return;
                }
            }
        }

        /// <summary>
        /// To be called when the user disconnects. This removes the user from the chat room
        /// and closes any resources used for connecting to the user.
        /// </summary>
        public override void Disconnect()
        {
            base.Disconnect();
            (handler as ServerMessageHandler).RemoveClient(this);
        }
    }
}