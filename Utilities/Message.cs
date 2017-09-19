using ChatRoom.Utilities.Properties;
using System;
using System.ComponentModel;
using System.Text;

namespace ChatRoom.Utilities
{
    /// <summary>
    /// Form of communication between the clients and the server. All communication is done through a message.
    /// </summary>
    /// <author>Kodey Converse (krconverse@wpi.edu)</author>
    public class Message
    {
        /// <summary>
        /// Describes the type of the message.
        /// </summary>
        public enum MessageType
        {
            /// <summary>
            /// A message that instructs the client to do something.
            /// </summary>
            Instruction = 0,
            /// <summary>
            /// A reply to an instruction.
            /// </summary>
            Reply = 1,
            /// <summary>
            /// A message that updates the user on connected users.
            /// </summary>
            Update = 2,
            /// <summary>
            /// A message that is part of the chat.
            /// </summary>
            ChatMessage = 3,
        };

        /// <summary>
        /// Combined with the <see cref="MessageType.Instruction"/> type, this is an 
        /// instruction to choose a nickname.
        /// </summary>
        public const string ChooseInstruction = "CHOOSE";

        /// <summary>
        /// The recipient that this message is addressed to.
        /// </summary>
        public User Recipient
        {
            get;
            private set;
        }

        /// <summary>
        /// The sender that sent this message.
        /// </summary>
        public User Sender
        {
            get;
            private set;
        }

        /// <summary>
        /// The type of the message.
        /// </summary>
        public MessageType Type
        {
            get;
            private set;
        }

        /// <summary>
        /// The contents of the message.
        /// </summary>
        public string Body
        {
            get;
            protected set;
        }

        /// <summary>
        /// The formatted body of the message.
        /// </summary>
        public string FormattedBody
        {
            get
            {
                if (Recipient != User.All)
                    return String.Format(Resources.PrivateMessageBodyUnfmt, Recipient.Nickname, Body);
                else
                    return String.Format(Resources.PublicMessageBodyUnfmt, Body);
            }
        }

        /// <summary>
        /// Whether the message is a private message or not.
        /// </summary>
        public bool IsPrivateMessage
        {
            get
            {
                return Recipient != User.All;
            }
        }

        /// <summary>
        /// The minimum size of a valid message in bytes.
        /// </summary>
        private const int MinMessageSize = sizeof(int) * 3;

        /// <summary>
        /// The maximum size of a valid message in bytes.
        /// </summary>
        public const int MaxMessageSize = 76;

        /// <summary>
        /// The maximum size of a valid message's body.
        /// </summary>
        public const int MaxBodySize = MaxMessageSize - (sizeof(int) * 3);

        /// <summary>
        /// Creates a new message.
        /// </summary>
        /// <param name="recipient">The intended recipient of this message.</param>
        /// <param name="sender">The sender of this message.</param>
        /// <param name="type">The type of this message.</param>
        /// <param name="body">The content of this message.</param>
        public Message(User recipient, User sender, MessageType type, string body)
        {
            Recipient = recipient;
            Sender = sender;
            Type = type;
            Body = body;
        }

        /// <summary>
        /// Creates a new message.
        /// </summary>
        /// <param name="data">The array of bytes representing a message.</param>
        /// <param name="handler">The handler to use to get user information.</param>
        public Message(byte[] data, MessageHandler handler)
        {
            if (data == null)
                throw new InvalidMessageException("No data was provided.");
            else if (data.Length < MinMessageSize)
                throw new InvalidMessageException("The message is too short to be a valid message!");
            else if (data.Length > MaxMessageSize)
                throw new InvalidMessageException("The message is too long to be a valid message!");

            Recipient = handler.GetUserByID(BitConverter.ToInt32(data, 0));
            Sender = handler.GetUserByID(BitConverter.ToInt32(data, sizeof(int)));
            Type = (MessageType)Enum.Parse(typeof(MessageType), BitConverter.ToInt32(data, sizeof(int) * 2).ToString());
            Body = Encoding.UTF8.GetString(data, sizeof(int) * 3, data.Length - sizeof(int) * 3).Trim('\0');
        }

        /// <summary>
        /// Converts this message to its raw form to be used for sending.
        /// </summary>
        /// <returns>The raw bytes of this message.</returns>
        public virtual byte[] ToByteArray()
        {
            byte[] result = new byte[MaxMessageSize];
            Buffer.BlockCopy(BitConverter.GetBytes(Recipient.Id), 0, result, 0, sizeof(int));
            Buffer.BlockCopy(BitConverter.GetBytes(Sender.Id), 0, result, sizeof(int), sizeof(int));
            Buffer.BlockCopy(BitConverter.GetBytes((int)Type), 0, result, sizeof(int) * 2, sizeof(int));
            Encoding.UTF8.GetBytes(Body, 0, Body.Length, result, sizeof(int) * 3);

            return result;
        }
    }
}