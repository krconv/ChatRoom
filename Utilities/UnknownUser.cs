using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatRoom.Utilities
{
    /// <summary>
    /// Represents an unknown user.
    /// </summary>
    /// <author>Kodey Converse (krconverse@wpi.edu)</author>
    public class UnknownUser : User
    {
        /// <summary>
        /// Creates a new unknown user with the given known ID.
        /// </summary>
        /// <param name="id">The ID of the unknown user.</param>
        public UnknownUser(int id) : base(id)
        { }
    }
}
