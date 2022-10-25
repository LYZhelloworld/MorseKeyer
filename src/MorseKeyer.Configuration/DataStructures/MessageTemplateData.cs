// <copyright file="MessageTemplateData.cs" company="Helloworld">
// Copyright (c) Helloworld. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MorseKeyer.Configuration.DataStructures
{
    /// <summary>
    /// The template of a message, used on the buttons to set the message to send.
    /// </summary>
    public record MessageTemplateData
    {
        /// <summary>
        /// The placeholder for my callsign.
        /// </summary>
        public const string MyCallsignPlaceholder = "{TX}";

        /// <summary>
        /// The placeholder for their callsign.
        /// </summary>
        public const string TheirCallsignPlaceholder = "{RX}";

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageTemplateData"/> class.
        /// </summary>
        public MessageTemplateData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageTemplateData"/> class.
        /// </summary>
        /// <param name="displayName">The name to display.</param>
        /// <param name="message">The template of the message.</param>
        /// <param name="isAppend">A value indicating whether the message is supposed to be appended to the end of the message.</param>
        public MessageTemplateData(string displayName, string message, bool isAppend = false)
        {
            this.DisplayName = displayName;
            this.Message = message;
            this.IsAppend = isAppend;
        }

        /// <summary>
        /// Gets or sets the name to display.
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the template of the message.
        /// The placeholder <c>{TX}</c> will be replaced by the value of "My callsign" and <c>{RX}</c> will be replaced by the value of "Their callsign".
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the message is supposed to be appended to the end of the message.
        /// <see langword="true"/> is for message to be appended; <see langword="false"/> is for replacement.
        /// </summary>
        public bool IsAppend { get; set; }

        /// <summary>
        /// Gets a value indicating whether "My callsign" needs to be non-empty.
        /// </summary>
        public bool RequireMyCallsign { get => this.Message.Contains(MyCallsignPlaceholder, StringComparison.OrdinalIgnoreCase); }

        /// <summary>
        /// Gets a value indicating whether "Their callsign" needs to be non-empty.
        /// </summary>
        public bool RequireTheirCallsign { get => this.Message.Contains(TheirCallsignPlaceholder, StringComparison.OrdinalIgnoreCase); }
    }
}
