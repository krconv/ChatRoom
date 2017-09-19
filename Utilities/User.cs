using ChatRoom.Utilities.Properties;
using System;
using System.Text;

namespace ChatRoom.Utilities
{
    /// <summary>
    /// A user in the chat room.
    /// </summary>
    /// <author>Kodey Converse (krconverse@wpi.edu)</author>
    public class User
    {
        /// <summary>
        /// The maximum size of a nickname.
        /// </summary>
        public const int MaxNicknameLength = Message.MaxBodySize - sizeof(int);

        /// <summary>
        /// The user that represents the server entity.
        /// </summary>
        public static User Server = new User(2, Resources.ServerNickname);

        /// <summary>
        /// The user that represents the all connected users.
        /// </summary>
        public static User All = new User(1, Resources.AllNickname);

        /// <summary>
        /// The user representing the current entity.
        /// </summary>
        public static User Me;

        /// <summary>
        /// The ID that uniquely represents this user.
        /// </summary>
        public int Id
        {
            get;
            private set;
        }

        /// <summary>
        /// The nickname that represents this user.
        /// </summary>
        public string Nickname
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates a new uniquely identifiable user.
        /// </summary>
        /// <param name="id">The user ID that the user is assigned.</param>
        /// <param name="nickname">The nickname that the user is assigned.</param>
        public User(int id, string nickname = null)
        {
            Id = id;
            if (!String.IsNullOrWhiteSpace(nickname))
                Nickname = nickname;
        }

        /// <summary>
        /// Creates a user from a string produced by <see cref="User.ToRaw()"/>.
        /// </summary>
        /// <param name="raw">User string to parse a user from.</param>
        public User(string raw)
            : this(Int32.Parse(raw.Split('\0')[0]), raw.Split('\0').Length > 1 ? raw.Split('\0')[1] : null)
        { }

        /// <summary>
        /// Sets the user's nickname.
        /// </summary>
        /// <param name="nickname">The new nickname for the user.</param>
        public void SetNickname(string nickname)
        {
            Nickname = nickname;
        }

        /// <summary>
        /// Converts the user into a raw string.
        /// </summary>
        /// <returns>A raw string representing the user.</returns>
        public string ToRaw()
        {
            return String.Format("{0}{2}{1}", Id, Nickname, '\0');
        }

        /// <summary>
        /// A readable format that represents a user.
        /// </summary>
        /// <returns>This user represented as a string.</returns>
        public override string ToString()
        {
            return String.Format(Resources.UserUnfmt, Nickname, Id);
        }
    }
}
