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
        /// Creates <see cref="WaveOutEvent"/> with given <see cref="ISampleProvider"/>.
        /// </summary>
        /// <param name="provider">The <see cref="ISampleProvider"/> to create <see cref="WaveOutEvent"/> with.</param>
        /// <param name="deviceNumber">The device number. If the number is invalid, <c>-1</c> (default device) will be used.</param>
        /// <param name="playbackStoppedEventHandler">The event handler triggered after the playback stops.</param>
        /// <returns>A new <see cref="WaveOutEvent"/> object.</returns>
        private static WaveOutEvent CreateWaveOutEvent(ISampleProvider provider, int deviceNumber, EventHandler<StoppedEventArgs> playbackStoppedEventHandler)
        {
            if (deviceNumber >= WaveOut.DeviceCount)
            {
                deviceNumber = -1;
            }

            var waveOutEvent = new WaveOutEvent()
            {
                DeviceNumber = deviceNumber,
            };

            waveOutEvent.Init(provider);
            waveOutEvent.PlaybackStopped += playbackStoppedEventHandler;
            return waveOutEvent;
        }

        /// <summary>
        /// Creates a new <see cref="MorseGenerator"/>.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <returns>A new <see cref="MorseGenerator"/> object.</returns>
        /// <exception cref="InvalidCharacterException">Thrown when message contains invalid characters.</exception>
        private ISampleProvider CreateMorseGenerator(string message)
        {
            return new MorseGenerator(message, new()
            {
                Gain = this.ViewModel.Gain,
                Frequency = this.ViewModel.Frequency,
                Wpm = this.ViewModel.Wpm,
            });
        }

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

            if (string.IsNullOrEmpty(this.ViewModel.Message) || this.ViewModel.Message.EndsWith(" ", StringComparison.InvariantCulture))
            {
                this.ViewModel.Message += message;
            }
            else
            {
                this.ViewModel.Message += " " + message;
            }

            this.MessageTextBox.Focus();

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
                this.waveOutEvent?.Stop();
                this.waveOutEvent = null;

                this.secondaryWaveOutEvent?.Stop();
                this.secondaryWaveOutEvent = null;

                this.waveOutEvent = CreateWaveOutEvent(this.CreateMorseGenerator(message), this.OutputDeviceComboBox.SelectedItem is KeyValuePair<int, string> item ? item.Key : -1, (sender, e) =>
                {
                    this.waveOutEvent = null;
                    this.Dispatcher.Invoke(() =>
                    {
                        this.ViewModel.IsSending = false;

                        if (!this.isCancelled)
                        {
                            this.ViewModel.Message = string.Empty;
                        }

                        this.MessageTextBox.Focus();
                    });
                });

                if (this.ViewModel.IsSecondaryOutputDeviceEnabled)
                {
                    this.secondaryWaveOutEvent = CreateWaveOutEvent(this.CreateMorseGenerator(message), this.SecondaryOutputDeviceComboBox.SelectedItem is KeyValuePair<int, string> secondaryItem ? secondaryItem.Key : -1, (sender, e) =>
                    {
                        this.secondaryWaveOutEvent = null;
                    });
                }

                this.isCancelled = false;
                this.waveOutEvent.Play();
                this.secondaryWaveOutEvent?.Play();
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
