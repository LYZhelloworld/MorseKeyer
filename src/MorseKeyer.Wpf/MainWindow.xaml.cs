// <copyright file="MainWindow.xaml.cs" company="Helloworld">
// Copyright (c) Helloworld. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MorseKeyer.Wpf
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private MainViewModel ViewModel => (MainViewModel)this.DataContext;

        /// <summary>
        /// Appends new message to the end of the message box.
        /// </summary>
        /// <param name="newMessage">The message to add.</param>
        /// <param name="requireMyCallsign">A value that indicates whether "My callsign" needs to be non-empty.</param>
        /// <param name="requireTheirCallsign">A value that indicates whether "Their callsign" needs to be non-empty.</param>
        private void AppendMessage(string newMessage, bool requireMyCallsign = false, bool requireTheirCallsign = false)
        {
            if (requireMyCallsign && string.IsNullOrEmpty(this.ViewModel.MyCallsign))
            {
                this.MyCallsignTextBox.Focus();
                return;
            }

            if (requireTheirCallsign && string.IsNullOrEmpty(this.ViewModel.TheirCallsign))
            {
                this.TheirCallsignTextBox.Focus();
                return;
            }

            if (this.ViewModel.Message.EndsWith(" ", System.StringComparison.InvariantCulture))
            {
                this.ViewModel.Message += newMessage;
            }
            else
            {
                this.ViewModel.Message += " " + newMessage;
            }

            // Put caret at the end.
            this.MessageTextBox.SelectionStart = this.MessageTextBox.Text.Length;
            this.MessageTextBox.SelectionLength = 0;
        }

        /// <summary>
        /// Sets the text of the message box.
        /// </summary>
        /// <param name="message">The message to set.</param>
        /// <param name="requireMyCallsign">A value that indicates whether "My callsign" needs to be non-empty.</param>
        /// <param name="requireTheirCallsign">A value that indicates whether "Their callsign" needs to be non-empty.</param>
        private void SetMessage(string message, bool requireMyCallsign = false, bool requireTheirCallsign = false)
        {
            if (requireMyCallsign && string.IsNullOrEmpty(this.ViewModel.MyCallsign))
            {
                this.MyCallsignTextBox.Focus();
                return;
            }

            if (requireTheirCallsign && string.IsNullOrEmpty(this.ViewModel.TheirCallsign))
            {
                this.TheirCallsignTextBox.Focus();
                return;
            }

            this.ViewModel.Message = message;
            this.MessageTextBox.Focus();

            // Put caret at the end.
            this.MessageTextBox.SelectionStart = this.MessageTextBox.Text.Length;
            this.MessageTextBox.SelectionLength = 0;
        }

        private void CQTemplateButton_Click(object sender, RoutedEventArgs e)
        {
            this.SetMessage($"CQ CQ CQ DE {this.ViewModel.MyCallsign} {this.ViewModel.MyCallsign} {this.ViewModel.MyCallsign} K", requireMyCallsign: true);
        }

        private void QRZTemplateButton_Click(object sender, RoutedEventArgs e)
        {
            this.SetMessage($"QRZ? DE {this.ViewModel.MyCallsign} K", requireMyCallsign: true);
        }

        private void RstTemplateButton_Click(object sender, RoutedEventArgs e)
        {
            this.SetMessage($"{this.ViewModel.TheirCallsign} DE {this.ViewModel.MyCallsign} UR RST 599 5NN", requireMyCallsign: true, requireTheirCallsign: true);
        }

        private void MyCallsignTemplateButton_Click(object sender, RoutedEventArgs e)
        {
            this.AppendMessage(this.ViewModel.MyCallsign, requireMyCallsign: true);
        }

        private void TheirCallsignTemplateButton_Click(object sender, RoutedEventArgs e)
        {
            this.AppendMessage(this.ViewModel.TheirCallsign, requireTheirCallsign: true);
        }

        private void SeventyThreeTemplateButton_Click(object sender, RoutedEventArgs e)
        {
            this.AppendMessage("73");
        }

        private void SayAgainTemplateButton_Click(object sender, RoutedEventArgs e)
        {
            this.AppendMessage("?");
        }

        private void NILTemplateButton_Click(object sender, RoutedEventArgs e)
        {
            this.AppendMessage("NIL");
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                if (comboBox.SelectedItem is ItemWithDescription item)
                {
                    this.AppendMessage(item.Value);
                    this.MessageTextBox.Focus();
                }

                comboBox.SelectedIndex = -1;
            }
        }

        private void MessageTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MessageBox.Show(this.MessageTextBox.Text); // Debug
                this.ViewModel.Message = string.Empty;
                this.MessageTextBox.Focus();
            }
        }
    }
}
