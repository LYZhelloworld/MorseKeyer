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

        private IEnumerable<MessageTemplateData> messageTemplates = DefaultMessageTemplates;

        /// <summary>
        /// Gets or sets my callsign.
        /// </summary>
        public string MyCallsign { get; set; } = string.Empty;

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
        public double Gain { get; set; } = 0.5;

        /// <summary>
        /// Gets or sets the frequency of the signal.
        /// </summary>
        public int Frequency { get; set; } = 700;

        /// <summary>
        /// Gets or sets the words-per-minute value.
        /// </summary>
        public int Wpm { get; set; } = 30;
    }
}
