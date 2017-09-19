using System;
using System.Linq;
using System.Collections.Generic;

namespace ChatRoom.Utilities
{
    /// <summary>
    /// The center of the chat room, which manages all messages and remote 
    /// connections, along with storeing information about the currently chat room.
    /// </summary>
    /// <author>Kodey Converse (krconverse@wpi.edu)</author>
    public abstract class MessageHandler
    {

        /// <summary>
        /// A list of the connected users.
        /// </summary>
        public IList<User> Users
        {
            get;
            protected set;
        }

        /// <summary>
        /// Adds a user to the chat room.
        /// </summary>
        /// <param name="user">The user to join.</param>
        public virtual void AddUser(User user)
        {
            Users.Add(user);
        }

        /// <summary>
        /// Removes a user from the chat room.
        /// </summary>
        /// <param name="user">The user to leave.</param>
        public virtual void RemoveUser(User user)
        {
            Users.Remove(user);
        }

        /// <summary>
        /// Retrieves the currently connected user with the given ID.
        /// </summary>
        /// <param name="id">User ID of the user to retrieve.</param>
        /// <returns>The currently connected user with the given ID, 
        /// or an <see cref="UnknownUser"/> if the user couldn't be found.
        /// </returns>
        public User GetUserByID(int id)
        {
            User result = null;
            if (id == User.All.Id)
                result = User.All;
            else if (id == User.Server.Id)
                result = User.Server;
            else
                result = Users.FirstOrDefault((u) => u.Id == id);

            if (result == null)
                result = new UnknownUser(id);

            return result;
        }

        /// <summary>
        /// Sends the given message.
        /// </summary>
        /// <param name="message">Message to send.</param>
        public abstract void SendMessage(Message message);

        /// <summary>
        /// Recieves and processes the given message.
        /// </summary>
        /// <param name="message">Message to recieve and process.</param>
        public abstract void RecieveMessage(Message message);

        /// <summary>
        /// Closes all open connections and stops the chat room.
        /// </summary>
        public abstract void Stop();
    }
}
