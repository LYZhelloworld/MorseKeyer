// <copyright file="TestDeviceNameHelper.cs" company="Helloworld">
// Copyright (c) Helloworld. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MorseKeyer.Wpf.FunctionalTest.Helpers
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using MorseKeyer.Wpf.Helpers;
    using NAudio.Wave;

    /// <summary>
    /// Tests <see cref="DeviceNameHelper"/>.
    /// </summary>
    [TestClass]
    [TestCategory("FunctionalTest")]
    [Ignore]
    public class TestDeviceNameHelper
    {
        /// <summary>
        /// Tests <see cref="DeviceNameHelper.GetOutputDevices"/>.
        /// </summary>
        [TestMethod]
        public void TestGetOutputDevices()
        {
            var outputDevices = DeviceNameHelper.GetOutputDevices();
            outputDevices.Should().NotBeNull();
            Enumerable.Range(0, WaveOut.DeviceCount).ToList().ForEach(i =>
            {
                WaveOutCapabilities capabilities = WaveOut.GetCapabilities(i);
                Console.WriteLine($"{i}: {capabilities.ProductName} - {outputDevices.FirstOrDefault(d => d.Key == i).Value}");
            });
        }
    }
}
