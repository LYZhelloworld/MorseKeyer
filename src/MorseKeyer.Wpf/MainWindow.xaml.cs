// <copyright file="MainWindow.xaml.cs" company="Helloworld">
// Copyright (c) Helloworld. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MorseKeyer.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using MorseKeyer.Resources;
    using MorseKeyer.SignalGenerator;
    using MorseKeyer.Wpf.DataStructures;
    using MorseKeyer.Wpf.Helpers;
    using NAudio.Wave;

    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The command to stop playing.
        /// </summary>
        public static readonly ICommand StopPlayingCommand = new RoutedCommand();

        /// <summary>
        /// The event for playing morse code.
        /// </summary>
        private WaveOutEvent? waveOutEvent;

        /// <summary>
        /// The event for playing morse code on secondary device.
        /// </summary>
        private WaveOutEvent? secondaryWaveOutEvent;

        /// <summary>
        /// A value that indicates whether the playback event has been cancelled.
        /// When cancelling the event, the message textbox will not be cleared.
        /// </summary>
        private bool isCancelled;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            this.Loaded += (sender, e) => this.RefreshOutputDevices();
        }

        /// <summary>
        /// Gets the data context.
        /// </summary>
        private MainViewModel ViewModel => (MainViewModel)this.DataContext;

        /// <summary>
        /// Appends new message to the end of the message box.
        /// </summary>
        /// <param name="message">The message to add.</param>
        /// <param name="requireMyCallsign">A value that indicates whether "My callsign" needs to be non-empty.</param>
        /// <param name="requireTheirCallsign">A value that indicates whether "Their callsign" needs to be non-empty.</param>
        private void AppendMessage(string message, bool requireMyCallsign = false, bool requireTheirCallsign = false)
        {
            if (this.ViewModel.IsSending)
            {
                return;
            }

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

            if (this.ViewModel.Message.EndsWith(" ", StringComparison.InvariantCulture))
            {
                this.ViewModel.Message += message;
            }
            else
            {
                this.ViewModel.Message += " " + message;
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
            if (this.ViewModel.IsSending)
            {
                return;
            }

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

        /// <summary>
        /// Sends the message.
        /// </summary>
        private void SendMessage()
        {
            var message = this.ViewModel.Message;
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            this.ViewModel.IsSending = true;

            try
            {
                var provider = new MorseGenerator(message, new()
                {
                    Gain = this.ViewModel.Gain,
                    Frequency = this.ViewModel.Frequency,
                    Wpm = this.ViewModel.Wpm,
                });

                this.waveOutEvent?.Stop();
                this.secondaryWaveOutEvent?.Stop();

                var numberOfDevices = WaveOut.DeviceCount;
                var deviceNumber = this.OutputDeviceComboBox.SelectedItem is KeyValuePair<int, string> item ? item.Key : -1;
                if (deviceNumber >= numberOfDevices)
                {
                    deviceNumber = -1;
                    this.OutputDeviceComboBox.SelectedIndex = 0;
                }

                this.waveOutEvent = new()
                {
                    DeviceNumber = deviceNumber,
                };
                this.waveOutEvent.Init(provider);

                if (this.ViewModel.IsSecondaryOutputDeviceEnabled)
                {
                    var secondaryDeviceNumber = this.SecondaryOutputDeviceComboBox.SelectedItem is KeyValuePair<int, string> secondaryItem ? secondaryItem.Key : -1;
                    if (secondaryDeviceNumber >= numberOfDevices)
                    {
                        secondaryDeviceNumber = -1;
                        this.SecondaryOutputDeviceComboBox.SelectedIndex = 0;
                    }

                    this.secondaryWaveOutEvent = new()
                    {
                        DeviceNumber = secondaryDeviceNumber,
                    };
                    this.secondaryWaveOutEvent.Init(provider);
                }

                this.isCancelled = false;

                this.waveOutEvent.PlaybackStopped += (sender, e) =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        this.ViewModel.IsSending = false;

                        if (!this.isCancelled)
                        {
                            this.ViewModel.Message = string.Empty;
                        }

                        this.MessageTextBox.Focus();
                    });
                };

                this.waveOutEvent.Play();
            }
            catch (InvalidCharacterException)
            {
                MessageBox.Show(MainWindowStrings.InvalidCharacterError, MainWindowStrings.WindowTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                this.ViewModel.IsSending = false;
            }
        }

        /// <summary>
        /// Refreshes the list of output devices.
        /// </summary>
        private void RefreshOutputDevices()
        {
            this.ViewModel.OutputDevices = DeviceNameHelper.GetOutputDevices()
                .Prepend(KeyValuePair.Create(-1, MainWindowStrings.DeviceNameDefault)); // Prepend default device.

            // Use ">" here because there is another item "Default" in the list.
            if (this.OutputDeviceComboBox.SelectedIndex == -1 || this.OutputDeviceComboBox.SelectedIndex > this.ViewModel.OutputDevices.Count())
            {
                this.OutputDeviceComboBox.SelectedIndex = 0;
            }

            if (this.SecondaryOutputDeviceComboBox.SelectedIndex == -1 || this.SecondaryOutputDeviceComboBox.SelectedIndex > this.ViewModel.OutputDevices.Count())
            {
                this.SecondaryOutputDeviceComboBox.SelectedIndex = 0;
            }
        }

        private void MessageTemplateButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button
                && button.DataContext is MessageTemplate messageTemplate)
            {
                var message = messageTemplate.Message
                    .Replace(MessageTemplate.MyCallsignPlaceholder, this.ViewModel.MyCallsign, StringComparison.OrdinalIgnoreCase)
                    .Replace(MessageTemplate.TheirCallsignPlaceholder, this.ViewModel.TheirCallsign, StringComparison.OrdinalIgnoreCase);
                if (messageTemplate.IsAppend)
                {
                    this.AppendMessage(message, messageTemplate.RequireMyCallsign, messageTemplate.RequireTheirCallsign);
                }
                else
                {
                    this.SetMessage(message, messageTemplate.RequireMyCallsign, messageTemplate.RequireTheirCallsign);
                }
            }
        }

        private void MessageTemplateButton_RightClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is Button button
                && button.DataContext is MessageTemplate messageTemplate)
            {
                var templateSettingsDialog = new TemplateSettingsDialog()
                {
                    DataContext = new TemplateSettingsDialogViewModel()
                    {
                        MessageTemplate = messageTemplate,
                    },
                    Owner = this,
                };
                var result = templateSettingsDialog.ShowDialog() ?? false;

                if (result)
                {
                    button.DataContext = ((TemplateSettingsDialogViewModel)templateSettingsDialog.DataContext).MessageTemplate;
                }
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                if (comboBox.SelectedItem is KeyValuePair<string, string> item)
                {
                    this.AppendMessage(item.Key);
                }

                comboBox.SelectedIndex = -1;
            }
        }

        private void MessageTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.SendMessage();
            }
        }

        private void StopPlayingCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.isCancelled = true;
            this.waveOutEvent?.Stop();
            this.secondaryWaveOutEvent?.Stop();
        }

        private void StopPlayingCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.waveOutEvent != null;
        }

        private void OutputDeviceComboBox_DropDownOpened(object sender, EventArgs e)
        {
            this.RefreshOutputDevices();
        }
    }
}
