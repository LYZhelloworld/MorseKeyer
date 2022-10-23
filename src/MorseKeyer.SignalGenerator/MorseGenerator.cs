// <copyright file="MorseGenerator.cs" company="Helloworld">
// Copyright (c) Helloworld. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MorseKeyer.SignalGenerator
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using NAudio.Wave;
    using NAudio.Wave.SampleProviders;

    /// <summary>
    /// The signal generator of Morse code.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MorseGenerator : ISampleProvider
    {
        /// <summary>
        /// The signal generator.
        /// </summary>
        private readonly ISampleProvider signalGenerator;

        /// <summary>
        /// Initializes a new instance of the <see cref="MorseGenerator"/> class.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <exception cref="InvalidCharacterException">Thrown when message contains invalid characters.</exception>
        public MorseGenerator(string message)
            : this(message, new MorseGeneratorConfig())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MorseGenerator"/> class.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="config">The config of the generator.</param>
        /// <exception cref="InvalidCharacterException">Thrown when message contains invalid characters.</exception>
        public MorseGenerator(string message, MorseGeneratorConfig config)
        {
            config = config ?? throw new ArgumentNullException(nameof(config));

            if (config.Gain < 0 || config.Gain > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(config.Gain), config.Gain, "The value should be between 0 and 1.");
            }

            int unitTime = GetUnitTime(config.Wpm);

            var code = MorseConverter.Convert(message);
            if (code.Length == 0)
            {
                this.signalGenerator = new SignalGenerator().Take(TimeSpan.Zero);
                return;
            }

            this.signalGenerator = code.Split(' ')
                .Select(word => word.Split('/')
                    .Select(letters => letters.ToCharArray()
                        .Select(c => c switch
                        {
                            '.' => new SignalGenerator()
                            {
                                Gain = config.Gain,
                                Frequency = config.Frequency,
                                Type = SignalGeneratorType.Sin,
                            }.Take(TimeSpan.FromMilliseconds(unitTime)),
                            '-' => new SignalGenerator()
                            {
                                Gain = config.Gain,
                                Frequency = config.Frequency,
                                Type = SignalGeneratorType.Sin,
                            }.Take(TimeSpan.FromMilliseconds(3 * unitTime)),
                            _ => null, // This should not happen.
                        })
                        .Where(x => x != null)
                        .Select(x => x!)
                        .Aggregate((a, b) => a.FollowedBy(TimeSpan.FromMilliseconds(unitTime), b)))
                    .Aggregate((a, b) => a.FollowedBy(TimeSpan.FromMilliseconds(3 * unitTime), b)))
                .Aggregate((a, b) => a.FollowedBy(TimeSpan.FromMilliseconds(7 * unitTime), b));
        }

        /// <inheritdoc/>
        [CLSCompliant(false)]
        public WaveFormat WaveFormat => this.signalGenerator.WaveFormat;

        /// <inheritdoc/>
        public int Read(float[] buffer, int offset, int count)
        {
            return this.signalGenerator.Read(buffer, offset, count);
        }

        /// <summary>
        /// Gets the unit time. Unit time is the time duration (in ms) of a dot.
        /// </summary>
        /// <param name="wpm">The number of words per minute.</param>
        /// <remarks>
        /// <para>Time duration of:</para>
        /// <list type="bullet">
        /// <item><term>Dot</term><description>1 unit</description></item>
        /// <item><term>Dash</term><description>3 units</description></item>
        /// <item><term>Space between morse codes</term><description>1 unit</description></item>
        /// <item><term>Space between letters</term><description>3 units</description></item>
        /// <item><term>Space between words</term><description>7 units</description></item>
        /// </list>
        /// </remarks>
        private static int GetUnitTime(int wpm)
        {
            return 1200 / wpm;
        }
    }
}
