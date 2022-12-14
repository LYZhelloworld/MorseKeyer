// <copyright file="Config.cs" company="Helloworld">
// Copyright (c) Helloworld. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MorseKeyer.Configuration
{
    using System.IO.Abstractions;
    using System.Text.Json;
    using MorseKeyer.Configuration.DataStructures;

    /// <summary>
    /// The class to load or save configurations.
    /// </summary>
    public class Config
    {
        /// <summary>
        /// The name of the config file.
        /// </summary>
        private const string ConfigFilename = "config.json";

        private readonly IFileSystem fileSystem;

        /// <summary>
        /// Initializes a new instance of the <see cref="Config"/> class.
        /// </summary>
        public Config()
            : this(new FileSystem())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Config"/> class.
        /// </summary>
        /// <param name="fileSystem">The <see cref="IFileSystem"/> to use.</param>
        public Config(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        /// <summary>
        /// Loads config from a file.
        /// </summary>
        /// <param name="filename">The filename of the config.</param>
        /// <returns>The loaded config.</returns>
        public ConfigData Load(string filename = ConfigFilename)
        {
            if (!this.fileSystem.File.Exists(filename))
            {
                // Use default config.
                return new();
            }

            try
            {
                using Stream jsonStream = this.fileSystem.File.Open(filename, FileMode.Open, FileAccess.Read);
                return JsonSerializer.Deserialize<ConfigData>(jsonStream, new JsonSerializerOptions
                {
                    AllowTrailingCommas = true,
                    IgnoreReadOnlyProperties = true,
                }) ?? new();
            }
            catch (Exception e) when (e is JsonException || e is IOException || e is FileNotFoundException)
            {
                // Use default config.
                return new();
            }
        }

        /// <summary>
        /// Saves config to a file.
        /// </summary>
        /// <param name="configData">The config to save.</param>
        /// <param name="filename">The filename of the config.</param>
        public void Save(ConfigData configData, string filename = ConfigFilename)
        {
            try
            {
                using Stream jsonStream = this.fileSystem.File.Open(filename, FileMode.Create, FileAccess.Write);
                JsonSerializer.Serialize(jsonStream, configData, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    AllowTrailingCommas = false,
                    IgnoreReadOnlyProperties = true,
                });
            }
            catch (Exception e) when (e is UnauthorizedAccessException || e is IOException)
            {
                // Do nothing.
            }
        }
    }
}
