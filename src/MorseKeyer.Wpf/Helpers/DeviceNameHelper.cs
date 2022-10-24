// <copyright file="DeviceNameHelper.cs" company="Helloworld">
// Copyright (c) Helloworld. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MorseKeyer.Wpf.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using NAudio.CoreAudioApi;
    using NAudio.Wave;

    /// <summary>
    /// A helper class to get the list of output devices.
    /// </summary>
    internal static class DeviceNameHelper
    {
        /// <summary>
        /// Gets a list of output devices.
        /// </summary>
        /// <returns>The list of output devices.</returns>
        public static IEnumerable<KeyValuePair<int, string>> GetOutputDevices()
        {
            var friendlyNames = GetOutputDevicesFromMMDevice();

            var deviceIdList = Enumerable.Range(0, WaveOut.DeviceCount);
            return deviceIdList.Select(i =>
            {
                WaveOutCapabilities capabilities = WaveOut.GetCapabilities(i);
                /*
                 * From the doc of NAudio:
                 * "Also note that the ProductName retured is limited to 32 characters, resulting in it often being truncated.
                 * This is a limitation of the underlying Windows API and there is unfortunately no easy way to fix it in NAudio."
                 *
                 * Further investigation shows that the productName will be truncated with only 31 characters.
                 *
                 * See also: https://github.com/naudio/NAudio/blob/master/Docs/EnumerateOutputDevices.md
                 */
                var productName = capabilities.ProductName;

                // Use friendly names if it exists in the list. Otherwise, use productName.
                return KeyValuePair.Create(i, friendlyNames.Where(friendlyName => friendlyName.StartsWith(productName, System.StringComparison.Ordinal)).FirstOrDefault() ?? productName);
            });
        }

        /// <summary>
        /// Gets friendly names of output devices from <see cref="MMDeviceEnumerator"/>.
        /// </summary>
        /// <returns>The list of friendly names of output devices.</returns>
        private static IEnumerable<string> GetOutputDevicesFromMMDevice()
        {
            using var enumerator = new MMDeviceEnumerator();
            var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
            return devices.Select(d => d.FriendlyName);
        }
    }
}
