// <copyright file="ConfigData.cs" company="Helloworld">
// Copyright (c) Helloworld. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MorseKeyer.Configuration.DataStructures
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The configuration class.
    /// </summary>
    public record ConfigData
    {
        /// <summary>
        /// The default value of message templates.
        /// </summary>
        private static readonly MessageTemplateData[] DefaultMessageTemplates = new MessageTemplateData[8]
        {
            new("CQ", "CQ CQ CQ DE {TX} {TX} {TX} K"),
            new("QRZ?", "QRZ? DE {TX} K"),
            new("RST", "{RX} DE {TX} UR RST 599 5NN"),
            new("{TX}", "{TX}", true),
            new("{RX}", "{RX}", true),
            new("73", "73", true),
            new("?", "?", true),
            new("NIL", "NIL", true),
        };

        private string myCallsign = string.Empty;

        private IEnumerable<MessageTemplateData> messageTemplates = DefaultMessageTemplates;

        private double gain = 0.5;

        private int frequency = 700;

        private int wpm = 30;

        /// <summary>
        /// Gets or sets my callsign.
        /// </summary>
        public string MyCallsign
        {
            get => this.myCallsign;
            set => this.myCallsign = value?.ToUpperInvariant() ?? string.Empty;
        }

        /// <summary>
        /// Gets or sets the message templates.
        /// </summary>
        /// <remarks>The property will patch user-defined configs with default values if the number of templates is not enough.</remarks>
        public IEnumerable<MessageTemplateData> MessageTemplates
        {
            get => this.messageTemplates;
            set => this.messageTemplates = DefaultMessageTemplates.Select((x, i) => i < value.Count() ? value.ElementAt(i) : x);
        }

        /// <summary>
        /// Gets or sets the gain of the signal.
        /// </summary>
        public double Gain { get => this.gain; set => this.gain = Limit(value, 0, 1); }

        /// <summary>
        /// Gets or sets the frequency of the signal.
        /// </summary>
        public int Frequency { get => this.frequency; set => this.frequency = Limit(value, 300, 900); }

        /// <summary>
        /// Gets or sets the words-per-minute value.
        /// </summary>
        public int Wpm { get => this.wpm; set => this.wpm = Limit(value, 5, 50); }

        /// <summary>
        /// Limits a value so that it is within the range from <paramref name="lowerBound"/> to <paramref name="upperBound"/>, inclusively.
        /// </summary>
        /// <param name="value">The value to test.</param>
        /// <param name="lowerBound">The lower bound.</param>
        /// <param name="upperBound">The upper bound.</param>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <returns>The value within the range.</returns>
        private static T Limit<T>(T value, T lowerBound, T upperBound)
            where T : IComparable
        {
            if (value.CompareTo(lowerBound) < 0)
            {
                return lowerBound;
            }

            if (value.CompareTo(upperBound) > 0)
            {
                return upperBound;
            }

            return value;
        }
    }
}
