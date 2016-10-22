using System;

namespace MultiRun.Editor
{
    /// <summary>
    /// Thrown when the path to a file does not exist
    /// </summary>
    public class InvalidPathException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the MultiRun.Editor.InvalidPathException class.
        /// </summary>
        public InvalidPathException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the MultiRun.Editor.InvalidPathException class with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public InvalidPathException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MultiRun.Editor.InvalidPathException class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="inner">The exception that is the cause of the current exception.</param>
        public InvalidPathException(string message, Exception inner) : base(message, inner)
        {
        }
    }

    /// <summary>
    /// Thrown when a file is not a valid MultiRun file
    /// </summary>
    public class InvalidFileException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the MultiRun.Editor.InvalidFileException class.
        /// </summary>
        public InvalidFileException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the MultiRun.Editor.InvalidFileException class with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public InvalidFileException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MultiRun.Editor.InvalidFileException class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="inner">The exception that is the cause of the current exception.</param>
        public InvalidFileException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}