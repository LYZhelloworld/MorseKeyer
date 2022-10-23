// <copyright file="TemplateSettingsDialog.xaml.cs" company="Helloworld">
// Copyright (c) Helloworld. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MorseKeyer.Wpf
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for TemplateSettingsDialog.xaml.
    /// </summary>
    public partial class TemplateSettingsDialog : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateSettingsDialog"/> class.
        /// </summary>
        public TemplateSettingsDialog()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets the data context.
        /// </summary>
        public TemplateSettingsDialogViewModel ViewModel => (TemplateSettingsDialogViewModel)this.DataContext;
    }
}
