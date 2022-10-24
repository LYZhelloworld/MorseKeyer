// <copyright file="ItemWithDescription.cs" company="Helloworld">
// Copyright (c) Helloworld. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MorseKeyer.Wpf.DataStructures
{
    /// <summary>
    /// Item for combo box with description.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    internal record ItemWithDescription<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemWithDescription{T}"/> class.
        /// </summary>
        /// <param name="value">The value of the item.</param>
        /// <param name="description">The description of the item.</param>
        public ItemWithDescription(T value, string description)
        {
            this.Value = value;
            this.Description = description;
        }

        /// <summary>
        /// Gets or sets the value of the item.
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// Gets or sets the description of the item.
        /// </summary>
        public string Description { get; set; } = string.Empty;
    }
}
