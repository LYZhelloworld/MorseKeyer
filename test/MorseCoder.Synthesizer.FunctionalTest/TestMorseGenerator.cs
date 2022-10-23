// <copyright file="TestMorseGenerator.cs" company="Helloworld">
// Copyright (c) Helloworld. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MorseCoder.SignalGenerator.FunctionalTest
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NAudio.Wave;

    /// <summary>
    /// Tests <see cref="MorseGenerator"/>.
    /// </summary>
    [TestClass]
    [Ignore]
    public class TestMorseGenerator
    {
        /// <summary>
        /// Tests morse code sound generation.
        /// </summary>
        /// <param name="message">The message to test.</param>
        [DataTestMethod]
        [DataRow("CQ CQ CQ DE BA1ZZZ BA1ZZZ BA1ZZZ PSE K")]
        [DataRow("BA1ZZZ DE BA1YYY BA1YYY BA1YYY UR RST 599 5NN MY RST HW? K")]
        [DataRow("BA1ZZZ DE BA1YYY THX FER UR QSO ES RST RPRT 73 <SK> E E")]
        public void TestSampleMessages(string message)
        {
            using var signalWaveOutEvent = new WaveOutEvent();
            signalWaveOutEvent.Init(new MorseGenerator(message, new()
            {
                Wpm = 20,
                Frequency = 700,
                Gain = 0.5,
            }));

            signalWaveOutEvent.Play();
            Task.Run(() =>
            {
                while (signalWaveOutEvent.PlaybackState == PlaybackState.Playing)
                {
                }
            }).Wait();
        }
    }
}