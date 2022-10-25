// <copyright file="MainViewModel.cs" company="Helloworld">
// Copyright (c) Helloworld. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MorseKeyer.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using MorseKeyer.Configuration;
    using MorseKeyer.Configuration.DataStructures;
    using MorseKeyer.Resources;

    /// <summary>
    /// The view model for <see cref="MainWindow"/>.
    /// </summary>
    internal class MainViewModel : INotifyPropertyChanged
    {
        private static readonly IEnumerable<KeyValuePair<string, string>> ProsignsValue = new List<KeyValuePair<string, string>>
        {
            new("R", ProsignStrings.R),
            new("K", ProsignStrings.K),
            new("<AR>", ProsignStrings.AR),
            new("<AS>", ProsignStrings.AS),
            new("<VE>", ProsignStrings.VE),
            new("?", ProsignStrings.QuestionMark),
            new("<HH>", ProsignStrings.HH),
            new("<BT>", ProsignStrings.BT),
            new("<KA>", ProsignStrings.KA),
            new("73", ProsignStrings.SeventyThree),
            new("<KN>", ProsignStrings.KN),
            new("<SK>", ProsignStrings.SK),
            new("<SOS>", ProsignStrings.SOS),
            new("<BK>", ProsignStrings.BK),
        }.OrderBy(x => x.Value);

        private static readonly IEnumerable<KeyValuePair<string, string>> QCodesValue = new List<KeyValuePair<string, string>>
        {
            new("QRA", QCodeStrings.QRA),
            new("QRL", QCodeStrings.QRL),
            new("QRM", QCodeStrings.QRM),
            new("QRN", QCodeStrings.QRN),
            new("QRP", QCodeStrings.QRP),
            new("QRS", QCodeStrings.QRS),
            new("QRT", QCodeStrings.QRT),
            new("QRU", QCodeStrings.QRU),
            new("QRV", QCodeStrings.QRV),
            new("QRZ", QCodeStrings.QRZ),
            new("QSL", QCodeStrings.QSL),
            new("QSO", QCodeStrings.QSO),
            new("QSP", QCodeStrings.QSP),
            new("QSY", QCodeStrings.QSY),
            new("QTH", QCodeStrings.QTH),
        };

        private readonly Config config = new();

        private ObservableCollection<MessageTemplateData> messageTemplates;

        private string message = string.Empty;

        private string myCallsign;

        private string theirCallsign = string.Empty;

        private double gain;

        private int frequency;

        private int wpm;

        private bool isSending;

        private ObservableCollection<KeyValuePair<int, string>> outputDevices = new();

        private bool isSecondaryOutputDeviceEnabled;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        public MainViewModel()
        {
            var configData = this.config.Load();
            this.messageTemplates = new(configData.MessageTemplates);
            this.myCallsign = configData.MyCallsign;
            this.gain = configData.Gain;
            this.frequency = configData.Frequency;
            this.wpm = configData.Wpm;
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets the prosigns used in morse code.
        /// </summary>
        public static IEnumerable<KeyValuePair<string, string>> Prosigns => ProsignsValue;

        /// <summary>
        /// Gets the Q-codes used in morse code.
        /// </summary>
        public static IEnumerable<KeyValuePair<string, string>> QCodes => QCodesValue;

        /// <summary>
        /// Gets the configuration manager.
        /// </summary>
        public Config Config => this.config;

        /// <summary>
        /// Gets or sets the message to send.
        /// </summary>
        public string Message
        {
            get => this.message;
            set => this.SetProperty(ref this.message, value.ToUpperInvariant());
        }

        /// <summary>
        /// Gets or sets the message templates.
        /// </summary>
        public ObservableCollection<MessageTemplateData> MessageTemplates
        {
            get => this.messageTemplates;
            set => this.SetProperty(ref this.messageTemplates, value);
        }

        /// <summary>
        /// Gets or sets my callsign.
        /// </summary>
        public string MyCallsign
        {
            get => this.myCallsign;
            set => this.SetProperty(ref this.myCallsign, value.ToUpperInvariant());
        }

        /// <summary>
        /// Gets or sets their callsign.
        /// </summary>
        public string TheirCallsign
        {
            get => this.theirCallsign;
            set => this.SetProperty(ref this.theirCallsign, value.ToUpperInvariant());
        }

        /// <summary>
        /// Gets or sets the gain of the signal.
        /// </summary>
        public double Gain
        {
            get => this.gain;
            set => this.SetProperty(ref this.gain, value);
        }

        /// <summary>
        /// Gets or sets the text of the gain of the signal.
        /// </summary>
        public string GainText
        {
            get => this.Gain.ToString(CultureInfo.InvariantCulture);
            set
            {
                if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var gain))
                {
                    this.Gain = gain;
                }
            }
        }

        /// <summary>
        /// Gets or sets the frequency of the signal.
        /// </summary>
        public int Frequency
        {
            get => this.frequency;
            set => this.SetProperty(ref this.frequency, value);
        }

        /// <summary>
        /// Gets or sets the frequency text of the signal.
        /// </summary>
        public string FrequencyText
        {
            get => this.Frequency.ToString(CultureInfo.InvariantCulture);
            set
            {
                if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var frequency))
                {
                    this.Frequency = frequency;
                }
            }
        }

        /// <summary>
        /// Gets or sets the words-per-minute value.
        /// </summary>
        public int Wpm
        {
            get => this.wpm;
            set => this.SetProperty(ref this.wpm, value);
        }

        /// <summary>
        /// Gets or sets the words-per-minute text.
        /// </summary>
        public string WpmText
        {
            get => this.Wpm.ToString(CultureInfo.InvariantCulture);
            set
            {
                if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var wpm))
                {
                    this.Wpm = wpm;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether there is a message being sent currently.
        /// </summary>
        public bool IsSending
        {
            get => this.isSending;
            set
            {
                if (this.SetProperty(ref this.isSending, value))
                {
                    this.OnPropertyChanged(nameof(this.IsMessageControlsEnabled));
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the controls related to message is enabled.
        /// </summary>
        public bool IsMessageControlsEnabled
        {
            get => !this.isSending;
        }

        /// <summary>
        /// Gets or sets output devices.
        /// </summary>
        public IEnumerable<KeyValuePair<int, string>> OutputDevices
        {
            get => this.outputDevices;
            set => this.SetProperty(ref this.outputDevices, new ObservableCollection<KeyValuePair<int, string>>(value));
        }

        /// <summary>
        /// Gets or sets a value indicating whether the secondary output device is enabled.
        /// </summary>
        public bool IsSecondaryOutputDeviceEnabled
        {
            get => this.isSecondaryOutputDeviceEnabled;
            set => this.SetProperty(ref this.isSecondaryOutputDeviceEnabled, value);
        }

        /// <summary>
        /// Saves config to config file.
        /// </summary>
        public void SaveConfig()
        {
            var configData = new ConfigData
            {
                MessageTemplates = this.messageTemplates.ToArray(),
                MyCallsign = this.myCallsign,
                Gain = this.gain,
                Frequency = this.frequency,
                Wpm = this.wpm,
            };
            this.config.Save(configData);
        }

        /// <summary>
        /// Sets a property.
        /// </summary>
        /// <typeparam name="T">The type of the field.</typeparam>
        /// <param name="field">The field.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns><see langword="true"/> if the operation was successful; <see langword="false"/> otherwise.</returns>
        protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = "")
        {
            if (!Equals(field, newValue))
            {
                field = newValue;
                this.OnPropertyChanged(propertyName);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Invokes <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            this.PropertyChanged?.Invoke(this, new(propertyName));
        }
    }
}
