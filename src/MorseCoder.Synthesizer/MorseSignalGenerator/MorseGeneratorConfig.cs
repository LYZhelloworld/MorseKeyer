﻿// <copyright file="MorseGeneratorConfig.cs" company="Helloworld">
// Copyright (c) Helloworld. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MorseCoder.Synthesizer.MorseSignalGenerator
{
    /// <summary>
    /// The config when creating <see cref="MorseGenerator"/>.
    /// </summary>
    public record MorseGeneratorConfig
    {
        /// <summary>
        /// Gets or sets the gain of signal.
        /// </summary>
        public double Gain { get; set; }

        /// <summary>
        /// Gets or sets the frequency.
        /// </summary>
        public int Frequency { get; set; }

        /// <summary>
        /// Gets or sets the number of words per minute.
        /// </summary>
        public int Wpm { get; set; }
    }
}
