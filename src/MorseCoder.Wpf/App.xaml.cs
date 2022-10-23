// <copyright file="App.xaml.cs" company="Helloworld">
// Copyright (c) Helloworld. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MorseCoder.Wpf
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for App.xaml.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// The event handler for the <see cref="Application.Startup"/> event.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void ApplicationStartup(object sender, StartupEventArgs e)
        {
            var window = new MainWindow
            {
                DataContext = new MainViewModel(),
            };
            window.Show();
        }
    }
}
