// <copyright file="MainViewModel.cs" company="Helloworld">
// Copyright (c) Helloworld. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MorseKeyer.Wpf
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using MorseKeyer.Wpf.DataStructures;

    /// <summary>
    /// The view model for <see cref="MainWindow"/>.
    /// </summary>
    internal class MainViewModel : INotifyPropertyChanged
    {
        private static readonly IEnumerable<ItemWithDescription> ProsignsValue = new List<ItemWithDescription>
        {
            new("R", "Roger"),
            new("K", "Over"),
            new("<AR>", "Newline"),
            new("<AS>", "Wait"),
            new("<VE>", "Verified"),
            new("?", "Say again"),
            new("<HH>", "Correction"),
            new("<BT>", "Break"),
            new("<KA>", "Attention"),
            new("73", "Best regards"),
            new("<KN>", "Go ahead"),
            new("<SK>", "Out"),
            new("<SOS>", "Distress signal"),
            new("<BK>", "Break in"),
        }.OrderBy(x => x.Value);

        private static readonly IEnumerable<ItemWithDescription> QCodesValue = new List<ItemWithDescription>
        {
            new("QRA", "Station name"),
            new("QRL", "Busy"),
            new("QRM", "Interference"),
            new("QRN", "Interference by station"),
            new("QRP", "Low power"),
            new("QRS", "Slow"),
            new("QRT", "End of transmission"),
            new("QRU", "Station not in use"),
            new("QRV", "Ready"),
            new("QRZ", "Calling"),
            new("QSL", "Confirmation of reception"),
            new("QSO", "Contact"),
            new("QSP", "Relay to"),
            new("QSY", "Change frequency"),
            new("QTH", "Location"),
        };

        /// <summary>
        /// The default value of message templates.
        /// </summary>
        private static readonly MessageTemplate[] DefaultMessageTemplates = new MessageTemplate[8]
        {
            new("CQ", "CQ CQ CQ DE {TX} {TX} {TX} K"),
            new("QRZ?", "QRZ? DE {TX} K"),
            new("RST", "{RX} DE {TX} UR RST 599 5NN"),
            new("<my>", "{TX}", true),
            new("<their>", "{RX}", true),
            new("73", "73", true),
            new("?", "?", true),
            new("NIL", "NIL", true),
        };

        private string message = string.Empty;
        private ObservableCollection<MessageTemplate> messageTemplates = new(DefaultMessageTemplates);
        private string myCallsign = string.Empty;
        private string theirCallsign = string.Empty;
        private double gain = 0.5;
        private int frequency = 700;
        private int wpm = 25;
        private bool isSending;

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets the prosigns used in morse code.
        /// </summary>
        public static IEnumerable<ItemWithDescription> Prosigns => ProsignsValue;

        /// <summary>
        /// Gets the Q-codes used in morse code.
        /// </summary>
        public static IEnumerable<ItemWithDescription> QCodes => QCodesValue;

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
        public ObservableCollection<MessageTemplate> MessageTemplates
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
                if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var gain)
                    && gain >= 0
                    && gain <= 1)
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
            get => this.frequency.ToString(CultureInfo.InvariantCulture);
            set
            {
                if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var frequency)
                    && frequency >= 300
                    && frequency <= 900)
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
            get => this.wpm.ToString(CultureInfo.InvariantCulture);
            set
            {
                if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var wpm)
                    && wpm >= 5
                    && wpm <= 50)
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
                this.SetProperty(ref this.isSending, value);
                this.PropertyChanged?.Invoke(this, new(nameof(this.IsMessageControlsEnabled)));
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
                this.PropertyChanged?.Invoke(this, new(propertyName));
                return true;
            }

            return false;
        }
    }
}
