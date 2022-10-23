// <copyright file="TestMorseSynthesizer.cs" company="Helloworld">
// Copyright (c) Helloworld. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MorseCoder.Synthesizer.FunctionalTest
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests <see cref="MorseSynthesizer"/>.
    /// </summary>
    [TestClass]
    [Ignore]
    public class TestMorseSynthesizer
    {
        /// <summary>
        /// Tests morse code sound generation.
        /// </summary>
        [TestMethod]
        public void TestSampleMessages()
        {
            using var synthesizer = new MorseSynthesizer();
            synthesizer.Start();

            var result = synthesizer.PlayAsync("CQ CQ CQ DE BA1ZZZ BA1ZZZ BA1ZZZ PSE K");
            result.Wait();
            Thread.Sleep(500);

            result = synthesizer.PlayAsync("BA1ZZZ DE BA1YYY BA1YYY BA1YYY UR RST 599 5NN MY RST HW? K");
            result.Wait();
            Thread.Sleep(500);

            result = synthesizer.PlayAsync("BA1ZZZ DE BA1YYY THX FER UR QSO ES RST RPRT 73 <SK> E E");
            result.Wait();
            Thread.Sleep(500);

            synthesizer.Stop();
        }
    }
}