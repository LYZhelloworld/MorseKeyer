// <copyright file="InvalidCharacterException.cs" company="Helloworld">
// Copyright (c) Helloworld. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MorseCoder.Synthesizer.MorseSignalGenerator
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Represents errors that occur when the input message for generating morse code contains invalid characters.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class InvalidCharacterException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCharacterException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public InvalidCharacterException(string message)
            : base("The message contains invalid characters. " + message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCharacterException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public InvalidCharacterException(string message, Exception innerException)
            : base("The message contains invalid characters. " + message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCharacterException"/> class.
        /// </summary>
        public InvalidCharacterException()
            : base("The message contains invalid characters.")
        {
        }
    }
}
