// <copyright file="TemplateSettingsDialogViewModel.cs" company="Helloworld">
// Copyright (c) Helloworld. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MorseKeyer.Wpf
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using MorseKeyer.Configuration.DataStructures;

    /// <summary>
    /// The view model for <see cref="TemplateSettingsDialog"/>.
    /// </summary>
    public class TemplateSettingsDialogViewModel : INotifyPropertyChanged
    {
        private MessageTemplateData messageTemplate = new();

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets or sets the message template.
        /// </summary>
        public MessageTemplateData MessageTemplate
        {
            get => this.messageTemplate;
            set => this.SetProperty(ref this.messageTemplate, value);
        }

        /// <inheritdoc cref="MessageTemplateData.DisplayName"/>
        public string DisplayName
        {
            get => this.MessageTemplate.DisplayName;
            set
            {
                var tmp = this.MessageTemplate.DisplayName;
                if (this.SetProperty(ref tmp, value))
                {
                    this.messageTemplate.DisplayName = tmp;
                }
            }
        }

        /// <inheritdoc cref="MessageTemplateData.Message"/>
        public string Message
        {
            get => this.MessageTemplate.Message;
            set
            {
                var tmp = this.MessageTemplate.Message;
                if (this.SetProperty(ref tmp, value))
                {
                    this.messageTemplate.Message = tmp;
                }
            }
        }

        /// <inheritdoc cref="MessageTemplateData.IsAppend"/>
        public bool IsAppend
        {
            get => this.MessageTemplate.IsAppend;
            set
            {
                var tmp = this.MessageTemplate.IsAppend;
                if (this.SetProperty(ref tmp, value))
                {
                    this.messageTemplate.IsAppend = tmp;
                    this.PropertyChanged?.Invoke(this, new(nameof(this.IsAppendingMode)));
                    this.PropertyChanged?.Invoke(this, new(nameof(this.IsReplacingMode)));
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the template is to be added to the end of the message box.
        /// </summary>
        public bool IsAppendingMode
        {
            get
            {
                return this.IsAppend;
            }

            set
            {
                this.IsAppend = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the template is to replace the content in the message box.
        /// </summary>
        public bool IsReplacingMode
        {
            get
            {
                return !this.IsAppend;
            }

            set
            {
                this.IsAppend = !value;
            }
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
