using System;
using System.Runtime.Serialization;

namespace ContosoAdsCommon
{
    /// <summary>
    /// Contoso Business Exception class
    /// </summary>
    public class ContosoBusinessException : Exception, ISerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContosoBusinessException"/> class.
        /// </summary>
        public ContosoBusinessException() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContosoBusinessException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ContosoBusinessException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContosoBusinessException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ContosoBusinessException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
