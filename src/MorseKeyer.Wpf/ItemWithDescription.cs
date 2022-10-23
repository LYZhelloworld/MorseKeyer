// <copyright file="ItemWithDescription.cs" company="Helloworld">
// Copyright (c) Helloworld. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MorseKeyer.Wpf
{
    /// <summary>
    /// Item for combo box with description.
    /// </summary>
    internal record ItemWithDescription
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemWithDescription"/> class.
        /// </summary>
        public ItemWithDescription()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemWithDescription"/> class.
        /// </summary>
        /// <param name="value">The value of the item.</param>
        /// <param name="description">The description of the item.</param>
        public ItemWithDescription(string value, string description)
        {
            this.Value = value;
            this.Description = description;
        }

        /// <summary>
        /// Gets or sets the value of the item.
        /// </summary>
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the item.
        /// </summary>
        public string Description { get; set; } = string.Empty;
    }
}
