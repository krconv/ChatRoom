using ChatRoom.Utilities.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatRoom.Utilities
{
    /// <summary>
    /// A message that is specifically for updating a client 
    /// with information about who is in the chat room.
    /// </summary>
    /// <author>Kodey Converse (krconverse@wpi.edu)</author>
    public class UpdateMessage : Message
    {
        /// <summary>
        /// A type representing what kind of update has occured.
        /// </summary>
        public enum UpdateType
        {
            /// <summary>
            /// A update where the user was already in the chat room.
            /// </summary>
            Present = 0,
            /// <summary>
            /// An update where the user just joined the chat room.
            /// </summary>
            Joined = 1,
            /// <summary>
            /// An update where the user just left the chat room.
            /// </summary>
            Left = 2
        }

        /// <summary>
        /// The type of this update.
        /// </summary>
        public UpdateType ExtendedType
        {
            get;
            private set;
        }

        /// <summary>
        /// The user that this update applies to.
        /// </summary>
        public User User
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates a message that can convey information about a user joining/leaving the chat.
        /// </summary>
        /// <param name="type">Type of this update.</param>
        /// <param name="user">User that this update applies to.</param>
        public UpdateMessage(UpdateType type, User user)
            : base(Utilities.User.All, Utilities.User.Server, MessageType.Update, GetUpdateDescription(type, user))
        {
            ExtendedType = type;
            User = user;
        }

        /// <summary>
        /// Creates a new update message.
        /// </summary>
        /// <param name="data">The array of bytes representing an update message.</param>
        /// <param name="handler">The handler to use to get user information.</param>
        public UpdateMessage(byte[] data, MessageHandler handler)
            : base(data, handler)
        {
            ExtendedType = (UpdateType)Enum.Parse(typeof(UpdateType), BitConverter.ToInt32(data, sizeof(int) * 3).ToString());
            User about = new User(Encoding.UTF8.GetString(data.Skip(sizeof(int) * 4).ToArray()).Trim('\0'));
            if (!(handler.GetUserByID(about.Id) is UnknownUser))
            {
                User = handler.GetUserByID(about.Id);
                User.SetNickname(about.Nickname);
            }
            else
            {
                User = about;
            }
            Body = GetUpdateDescription(ExtendedType, User);
        }

        /// <summary>
        /// Retrieves a formatted message representing the given update.
        /// </summary>
        /// <param name="type">The action that caused this update.</param>
        /// <param name="user">The user this update applies to.</param>
        /// <returns>A formated message, or empty if none represent this update.</returns>
        private static string GetUpdateDescription(UpdateType type, User user)
        {
            if (type == UpdateType.Joined)
                return String.Format(Resources.UserJoinedUnfmt, user.Nickname);
            else if (type == UpdateType.Left)
                return String.Format(Resources.UserLeftUnfmt, user.Nickname);
            else if (type == UpdateType.Present)
                return String.Format(Resources.UserIsPresentUnfmt, user.Nickname);
            else
                return String.Empty;
        }

        /// <summary>
        /// Converts this into a raw byte array for transport.
        /// </summary>
        /// <returns>A raw byte array representing this message.</returns>
        public override byte[] ToByteArray()
        {
            byte[] result = new byte[Message.MaxMessageSize];
            Buffer.BlockCopy(base.ToByteArray(), 0, result, 0, sizeof(int) * 3);
            Buffer.BlockCopy(BitConverter.GetBytes((int)ExtendedType), 0, result, sizeof(int) * 3, sizeof(int));
            byte[] rawUser = Encoding.UTF8.GetBytes(User.ToRaw());
            Buffer.BlockCopy(rawUser, 0, result, sizeof(int) * 4, rawUser.Length);
            return result;
        }

    }
}
