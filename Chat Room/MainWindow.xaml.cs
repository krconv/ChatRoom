
using System;
using System.Linq;
using System.Windows;
using ChatRoom.Utilities;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace ChatRoom
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// <author>Kodey Converse (krconverse@wpi.edu)</author>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The message handler for the current chat session.
        /// </summary>
        private MessageHandler handler;
        /// <summary>
        /// An event handler to react to the window being closed by the user.
        /// </summary>
        private EventHandler closed;
        /// <summary>
        /// An singleton instance of this class so that the dialog window can be used.
        /// </summary>
        public static MainWindow Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Initialize the Chat Room Client.
        /// </summary>
        public MainWindow()
        {
            Instance = this;
            InitializeComponent();
            ConnectedStatus.Text = Properties.Resources.StatusNotConnected;
            MessageText.MaxLength = Message.MaxBodySize;

            // make sure that when the program is closed, we close it correctly
            closed = new EventHandler((s, e) => WindowClosed(s, e));
            Closed += closed;

            try
            {
                // set up the chat room
                handler = new ClientMessageHandler();
                ChatHistory.DataContext = handler;
                ConnectedUsers.DataContext = handler;
            }
            catch (Exception ex)
            {
                // if anything uncaught happens, warn the user with a dialog and then close the program
                ShowDialog(String.Format(Properties.Resources.ErrorMessage, ex.Message), Close);
            }
        }

        /// <summary>
        /// Called when the window is closed. 
        /// This will close the handler and all client-server connetions.
        /// </summary>
        /// <param name="sender">Unused.</param>
        /// <param name="e">Unused.</param>
        private void WindowClosed(object sender, EventArgs e)
        {
            Instance = null;
            if (handler != null)
                handler.Stop();
        }
        /// <summary>
        /// Sends the current message to the server when the user clicks [Send].
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (SendMessage(MessageText.Text))
                MessageText.Text = String.Empty;
        }
        /// <summary>
        /// Sends the current message to the server when the user presses Enter.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MessageText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return
                && !Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift))
            {
                SendButton_Click(sender, e);
            }
        }

        /// <summary>
        /// Sends the user's message to the chat room.
        /// </summary>
        /// <param name="text">Text that should be in the body of the message.</param>
        /// <returns>True if the message was sent, false otherwise.</returns>
        private bool SendMessage(string text)
        {
            bool sent = true;
            if (String.IsNullOrWhiteSpace(ToTextBox.Text))
                handler.SendMessage(new Message(User.All, User.Me, Message.MessageType.ChatMessage, text));
            else
            {
                User user = handler.Users.FirstOrDefault((u) => u.Nickname.Equals(ToTextBox.Text));
                if (user != null)
                    handler.SendMessage(new Message(user, User.Me, Message.MessageType.ChatMessage, text));
                else
                    sent = false;
            }
            return sent;
        }

        #region Dialog Box
        /// <summary>
        /// Save the call back of whatever called the dialog window last.
        /// </summary>
        private object callback;
        /// <summary>
        /// Determines whether the current dialog box should capture user input.
        /// </summary>
        private bool needInput;

        /// <summary>
        /// Shows a dialog to the user that has an Ok button.
        /// </summary>
        /// <param name="prompt">The message to prompt the user with.</param>
        /// <param name="callback">The action to take once the user has clicked ok.</param>
        public void ShowDialog(string prompt, Action callback)
        {
            this.callback = callback;
            needInput = false;
            PromptBlock.Text = prompt;
            InputTextBox.Visibility = Visibility.Collapsed;
            Dialog.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Shows a dialog to the user that has a Ok button and a 
        /// text box to enter a response in.
        /// </summary>
        /// <param name="prompt">The message to prompt the user with.</param>
        /// <param name="callback">The action to take once the user has submitted an answer.</param>
        public void ShowDialog(string prompt, Action<string> callback)
        {
            this.callback = callback;
            needInput = true;
            PromptBlock.Text = prompt;
            InputTextBox.Visibility = Visibility.Visible;
            InputTextBox.Text = String.Empty;
            Dialog.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Closes the currently open dialog.
        /// </summary>
        public void CloseDialog()
        {
            Dialog.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Sends the answer to the prompt to the callback function when the user presses Enter.
        /// </summary>
        /// <param name="sender">Unused.</param>
        /// <param name="e">The key event arguments.</param>
        private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                OkButton_Click(sender, e);
            }
        }

        /// <summary>
        /// Sends the answer to the prompt to the callback function when the user presses [Ok].
        /// </summary>
        /// <param name="sender">Unused.</param>
        /// <param name="e">Unused.</param>
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (needInput)
            {
                if (!String.IsNullOrWhiteSpace(InputTextBox.Text))
                {
                    CloseDialog();
                    ((Action<string>)callback)(InputTextBox.Text);
                }
            }
            else
            {
                CloseDialog();
                ((Action)callback)();
            }
        }
        #endregion
    }
}
