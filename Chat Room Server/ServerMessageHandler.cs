using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Sockets;
using ChatRoom.Utilities;
using ChatRoom.Server.Properties;

namespace ChatRoom.Server
{
    /// <summary>
    /// A handler for the messaging system of the chat room. This will hold a 
    /// list of all connected clients, and send out messages to the needed clients.
    /// </summary>
    /// <author>Kodey Converse (krconverse@wpi.edu)</author>
    class ServerMessageHandler : MessageHandler
    {
        /// <summary>
        /// Random generator for generating unique user IDs.
        /// </summary>
        private static Random random = new Random(DateTime.Now.GetHashCode());

        /// <summary>
        /// A list of the the currently connected clients.
        /// </summary>
        private List<Client> clients;

        /// <summary>
        /// Creates a new empty chat room.
        /// </summary>
        public ServerMessageHandler()
        {
            Users = new List<User>();
            clients = new List<Client>();
        }

        /// <summary>
        /// Adds a client to the chat room.
        /// </summary>
        /// <param name="accepted">An accepted connection to a user.</param>
        public void AddClient(TcpClient accepted)
        {
            try
            {
                Client client = new Client(accepted, this);
                clients.Add(client);
                AddUser(client.User);
            }
            catch (Exception ex)
            {
                if (ex is RemoteSourceDisconnectedException || ex is InvalidMessageException)
                    Logger.Log(Resources.ClientInitializationFailedUnfmt, accepted.Client.RemoteEndPoint);
                else
                    Logger.Log(Resources.UnknownErrorUnfmt, accepted.Client.RemoteEndPoint);
            }
        }

        /// <summary>
        /// Adds a user to the chat room.
        /// </summary>
        /// <param name="user">The user to join.</param>
        public override void AddUser(User user)
        {
            base.AddUser(user);
            SendMessage(new UpdateMessage(UpdateMessage.UpdateType.Joined, user));
        }

        /// <summary>
        /// Removes a client from the chat room.
        /// </summary>
        /// <param name="client">The client to be removed.</param>
        public void RemoveClient(Client client)
        {
            if (clients.Contains(client))
            {
                clients.Remove(client);
                RemoveUser(client.User);
            }
        }

        /// <summary>
        /// Removes a user from the chat room. This does not remove a client's connection!
        /// </summary>
        /// <param name="user">The user to leave.</param>
        public override void RemoveUser(User user)
        {
            base.RemoveUser(user);
            SendMessage(new UpdateMessage(UpdateMessage.UpdateType.Left, user));
        }

        /// <summary>
        /// Sends a message.
        /// </summary>
        /// <param name="message">The message to send.</param>
        public override void SendMessage(Message message)
        {
            bool success = false;
            if (message.Recipient == User.All)
            { // send the message to all users
                foreach (Client client in clients)
                {
                    client.SendMessage(message);
                }
                success = true;
            }
            else
            { // send the message to a specific user
                Client client = GetClientByUser(message.Recipient);
                if (client != null)
                {
                    client.SendMessage(message);
                    if (message.Sender != User.Server)
                    {
                        client = GetClientByUser(message.Sender);
                        if (client != null)
                            client.SendMessage(message);
                    }
                    success = true;
                }
                else
                {
                    Logger.Log(Resources.UserNotFoundUnfmt, message.Recipient);
                }
            }
            Logger.Log(message, success);
        }

        /// <summary>
        /// Recieves informational messages from a client.
        /// </summary>
        /// <param name="message">Message to recieve and process.</param>
        public override void RecieveMessage(Message message)
        {
            if (message.Recipient != User.Server)
            {
                SendMessage(message);
            }
        }

        /// <summary>
        /// Generates a unique ID number that can be used to identify a user.
        /// </summary>
        /// <returns>A unique ID number.</returns>
        public int GenerateUserID()
        {
            int id = random.Next();
            // keep generating user ID until it is unique
            while (Users.Any((u) => u.Id == id)
                || id == User.Server.Id
                || id == User.All.Id)
                id = random.Next();
            return id;
        }

        /// <summary>
        /// Finds the client that is connected to the given user.
        /// </summary>
        /// <param name="user">User to look for.</param>
        /// <returns>The connected client, or null if the client couldn't be found.</returns>
        private Client GetClientByUser(User user)
        {
            return clients.FirstOrDefault((c) => c.User == user);
        }

        /// <summary>
        /// Stops the chat room and disconnects all clients.
        /// </summary>
        public override void Stop()
        {
            foreach (Client client in clients)
                client.Disconnect();
        }
    }
}