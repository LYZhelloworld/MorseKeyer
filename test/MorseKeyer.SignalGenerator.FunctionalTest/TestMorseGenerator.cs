// <copyright file="TestMorseGenerator.cs" company="Helloworld">
// Copyright (c) Helloworld. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MorseKeyer.SignalGenerator.FunctionalTest
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using MorseKeyer.SignalGenerator;
    using NAudio.Wave;

    /// <summary>
    /// Tests <see cref="MorseGenerator"/>.
    /// </summary>
    [TestClass]
    [TestCategory("FunctionalTest")]
    [Ignore]
    public class TestMorseGenerator
    {
        /// <summary>
        /// Tests morse code sound generation.
        /// </summary>
        /// <param name="message">The message to test.</param>
        [DataTestMethod]
        [DataRow("CQ CQ CQ")]
        [DataRow("UR RST 599 5NN")]
        [DataRow("73 <SK> E E")]
        public void TestSampleMessages(string message)
        {
            using var signalWaveOutEvent = new WaveOutEvent();
            signalWaveOutEvent.Init(new MorseGenerator(message, new()));

            signalWaveOutEvent.Play();
            Task.Run(() =>
            {
                while (signalWaveOutEvent.PlaybackState == PlaybackState.Playing)
                {
                }
            }).Wait();

            Thread.Sleep(500);
        }
    }
}
