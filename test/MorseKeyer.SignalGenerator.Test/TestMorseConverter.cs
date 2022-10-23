// <copyright file="TestMorseConverter.cs" company="Helloworld">
// Copyright (c) Helloworld. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MorseKeyer.SignalGenerator.Test
{
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests <see cref="MorseConverter"/>.
    /// </summary>
    [TestClass]
    public class TestMorseConverter
    {
        /// <summary>
        /// Tests <see cref="MorseConverter.Convert(string)"/>.
        /// </summary>
        /// <param name="message">The plain text message for testing.</param>
        /// <param name="expected">The expected result.</param>
        [DataTestMethod]
        [DataRow("CQ CQ CQ", "-.-./--.- -.-./--.- -.-./--.-")]
        [DataRow("<SK>", "...-.-")]
        public void TestConvert(string message, string expected)
        {
            var morseCode = MorseConverter.Convert(message);
            morseCode.Should().Be(expected);
        }

        /// <summary>
        /// Tests <see cref="MorseConverter.Convert(string)"/> with exception thrown.
        /// </summary>
        /// <param name="message">The plain text message for testing.</param>
        [DataTestMethod]
        [DataRow("\0")]
        public void TestConvertWithException(string message)
        {
            var action = () => MorseConverter.Convert(message);
            action.Should().Throw<InvalidCharacterException>();
        }
    }
}
