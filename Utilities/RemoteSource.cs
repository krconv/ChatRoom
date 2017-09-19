using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatRoom.Utilities
{
    /// <summary>
    /// A connection to a remote source which will communicate back and forth.
    /// </summary>
    /// <author>Kodey Converse (krconverse@wpi.edu)</author>
    public abstract class RemoteSource
    {
        /// <summary>
        /// The stream that is connected to the remote source.
        /// </summary>
        protected readonly NetworkStream stream;
        /// <summary>
        /// The socket that is connected to the remote source.
        /// </summary>
        protected readonly Socket socket;
        /// <summary>
        /// The thread that this client handler is running in.
        /// </summary>
        protected readonly Thread thread;
        /// <summary>
        /// The handler that will process message interactions.
        /// </summary>
        protected readonly MessageHandler handler;
        /// <summary>
        /// A boolean determining whether this source is active.
        /// </summary>
        protected bool active = false;

        /// <summary>
        /// Creates a remote source that will use the given handler to process messages.
        /// </summary>
        /// <param name="accepted">An accepted client that is connected to a user.</param>
        /// <param name="handler">The handler that will handle all message interactions.</param>
        protected RemoteSource(TcpClient accepted, MessageHandler handler)
        {
            this.handler = handler;
            stream = accepted.GetStream();
            socket = accepted.Client;
            thread = new Thread(new ThreadStart(Listen));
        }

        /// <summary>
        /// Recieves a message from the remote source.
        /// </summary>
        /// <returns>The message recieved from the remote source.</returns>
        /// <exception cref="InvalidMessageException">Thrown if the recieved message is not of proper format.</exception>
        /// <exception cref="RemoteSourceDisconnectedException">Thrown if the remote source disconnects while trying to read from the stream.</exception>
        public Message RecieveMessage()
        {
            byte[] buffer = new byte[Message.MaxMessageSize];
            int recievedLength = 0;
            try
            {
                recievedLength = stream.Read(buffer, 0, Message.MaxMessageSize);
            }
            catch (Exception ex)
            {
                if (ex is IOException || ex is ObjectDisposedException)
                {
                    Disconnect();
                    throw new RemoteSourceDisconnectedException("Client was disconnected while trying to recieve a message.", ex);
                }
                else
                    throw;
            }
            if (recievedLength > 0)
            {
                Message recieved = new Message(buffer, handler);
                if (recieved.Type == Message.MessageType.Update)
                    recieved = new UpdateMessage(buffer, handler);
                return recieved;
            }
            else
            {
                Disconnect();
                throw new RemoteSourceDisconnectedException("Client was disconnected while trying to recieve a message.");
            }

        }

        /// <summary>
        /// Relays a message to the client.
        /// </summary>
        /// <param name="message">Message to send to the client.</param>
        /// <exception cref="RemoteSourceDisconnectedException">Thrown if the remote source disconnects while trying to write to the stream.</exception>
        public void SendMessage(Message message)
        {
            byte[] raw = message.ToByteArray();
            try
            {
                stream.Write(raw, 0, raw.Length);
                stream.Flush();
            }
            catch (Exception ex)
            {
                if (ex is IOException || ex is ObjectDisposedException)
                    throw new RemoteSourceDisconnectedException("Client was disconnected while trying to send a message", ex);
                else
                    throw;
            }
        }

        /// <summary>
        /// Continuously listen for the source to send a message, 
        /// then pass the message onto the message handler.
        /// </summary>
        protected abstract void Listen();

        /// <summary>
        /// Handles the freeing of resources held by this connection.
        /// </summary>
        public virtual void Disconnect()
        {
            active = false;
            stream.Flush();
            if (socket.Connected)
                socket.Shutdown(SocketShutdown.Both);
            stream.Dispose();
        }
    }
}
