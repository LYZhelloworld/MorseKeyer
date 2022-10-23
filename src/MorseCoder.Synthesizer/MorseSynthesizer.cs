// <copyright file="MorseSynthesizer.cs" company="Helloworld">
// Copyright (c) Helloworld. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MorseCoder.Synthesizer
{
    using MorseCoder.Synthesizer.MorseSignalGenerator;
    using NAudio.Wave;
    using NAudio.Wave.SampleProviders;

    /// <summary>
    /// The synthesizer of morse code sound.
    /// </summary>
    public class MorseSynthesizer : IDisposable
    {
        /// <summary>
        /// The background noise generator.
        /// </summary>
        private readonly SignalGenerator noiseGenerator;

        /// <summary>
        /// The signal generator.
        /// </summary>
        private ISampleProvider? signalGenerator;

        /// <summary>
        /// A value indicating whether this instance is disposed.
        /// </summary>
        private bool disposedValue;

        /// <summary>
        /// The <see cref="WaveOutEvent"/> of background noise.
        /// </summary>
        private WaveOutEvent? noiseWaveOutEvent;

        /// <summary>
        /// The <see cref="WaveOutEvent"/> of signal.
        /// </summary>
        private WaveOutEvent? signalWaveOutEvent;

        /// <inheritdoc cref="SignalGain"/>
        private double signalGain = 0.1;

        /// <inheritdoc cref="NoiseGain"/>
        private double noiseGain = 0.01;

        /// <summary>
        /// Initializes a new instance of the <see cref="MorseSynthesizer"/> class.
        /// </summary>
        public MorseSynthesizer()
        {
            this.noiseGenerator = new SignalGenerator()
            {
                Gain = this.SignalGain,
                Type = SignalGeneratorType.White,
            };
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="MorseSynthesizer"/> class.
        /// </summary>
        ~MorseSynthesizer()
        {
            this.Dispose(disposing: false);
        }

        /// <summary>
        /// Gets or sets the gain of signal.
        /// </summary>
        /// <remarks>The value should be between 0 and 1.</remarks>
        public double SignalGain
        {
            get => this.signalGain;
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The value should be between 0 and 1.");
                }

                this.signalGain = value;
            }
        }

        /// <summary>
        /// Gets or sets the gain of noise.
        /// </summary>
        /// <remarks>The value should be between 0 and 1.</remarks>
        public double NoiseGain
        {
            get => this.noiseGain;
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The value should be between 0 and 1.");
                }

                this.noiseGain = value;
                this.noiseGenerator.Gain = value;
            }
        }

        /// <summary>
        /// Gets or sets the signal frequency in Hz.
        /// </summary>
        public int Frequency { get; set; } = 550;

        /// <summary>
        /// Gets or sets WPM.
        /// </summary>
        public int Wpm { get; set; } = 25;

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Starts monitoring the frequency.
        /// </summary>
        public void Start()
        {
            if (this.disposedValue)
            {
                throw new ObjectDisposedException(nameof(MorseSynthesizer));
            }

            this.noiseWaveOutEvent = new WaveOutEvent();
            this.noiseWaveOutEvent.Init(new SignalGenerator()
            {
                Gain = this.NoiseGain,
                Type = SignalGeneratorType.White,
            });
            this.noiseWaveOutEvent.Play();
        }

        /// <summary>
        /// Plays morse code asynchronously.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <returns>A <see cref="Task"/> indicating the asynchronous operation of playing morse code.</returns>
        /// <exception cref="InvalidCharacterException">Thrown when message contains invalid characters.</exception>
        public async Task PlayAsync(string message)
        {
            if (this.disposedValue)
            {
                throw new ObjectDisposedException(nameof(MorseSynthesizer));
            }

            this.CreateSignalWaveOutEvent(message);
            this.signalWaveOutEvent?.Play();
            await Task.Run(() =>
            {
                while (this.signalWaveOutEvent?.PlaybackState == PlaybackState.Playing)
                {
                }
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Stops playing morse code.
        /// </summary>
        public void StopPlaying()
        {
            if (this.disposedValue)
            {
                throw new ObjectDisposedException(nameof(MorseSynthesizer));
            }

            this.signalWaveOutEvent?.Stop();
        }

        /// <summary>
        /// Stops monitoring the frequency.
        /// </summary>
        public void Stop()
        {
            if (this.disposedValue)
            {
                throw new ObjectDisposedException(nameof(MorseSynthesizer));
            }

            this.signalWaveOutEvent?.Stop();
            this.noiseWaveOutEvent?.Stop();
        }

        /// <summary>
        /// Provides a mechanism for releasing unmanaged resources.
        /// </summary>
        /// <param name="disposing">A value indicating whether to dispose managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    // Release managed resources.
                    this.Stop();
                    this.signalWaveOutEvent?.Dispose();
                    this.noiseWaveOutEvent?.Dispose();
                }

                // Release unmanaged resources (nothing here).
                this.disposedValue = true;
            }
        }

        /// <summary>
        /// Creates a new signal <see cref="WaveOutEvent"/>.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <exception cref="InvalidCharacterException">Thrown when message contains invalid characters.</exception>
        private void CreateSignalWaveOutEvent(string message)
        {
            if (this.disposedValue)
            {
                throw new ObjectDisposedException(nameof(MorseSynthesizer));
            }

            this.signalWaveOutEvent?.Stop();
            this.signalWaveOutEvent = null;

            this.signalGenerator = null;
            this.signalGenerator = new MorseGenerator(message, new()
            {
                Gain = this.SignalGain,
                Frequency = this.Frequency,
                Wpm = this.Wpm,
            });

            this.signalWaveOutEvent = new WaveOutEvent();
            this.signalWaveOutEvent.Init(this.signalGenerator);
        }
    }
}