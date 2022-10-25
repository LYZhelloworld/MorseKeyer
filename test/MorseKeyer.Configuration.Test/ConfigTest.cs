// <copyright file="ConfigTest.cs" company="Helloworld">
// Copyright (c) Helloworld. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MorseKeyer.Configuration.Test
{
    using System.IO.Abstractions.TestingHelpers;
    using System.Text.Json;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using MorseKeyer.Configuration.DataStructures;

    /// <summary>
    /// Tests <see cref="Config"/>.
    /// </summary>
    [TestClass]
    public class ConfigTest
    {
        private const string TestConfigFilename = "config.json";

        /// <summary>
        /// Tests <see cref="Config()"/>.
        /// </summary>
        [TestMethod]
        public void TestConstructor()
        {
            var config = new Config();
            config.ConfigData.Should().NotBeNull();
        }

        /// <summary>
        /// Tests <see cref="Config.Load(string)"/>.
        /// </summary>
        [TestMethod]
        public void TestLoad()
        {
            var fileSystem = new MockFileSystem();
            fileSystem.AddFile(TestConfigFilename, new MockFileData(@"
            {
                ""MessageTemplates"": [
                    { ""DisplayName"": ""TEST"", ""Message"": ""{TX} TEST"", ""IsAppend"": false },
                    { ""DisplayName"": ""TEST 2"", ""Message"": ""{TX} TEST"", ""IsAppend"": true },
                ],
                ""MyCallsign"": ""BA1ZZZ"",
                ""Gain"": 0.9,
                ""Frequency"": 300,
                ""Wpm"": 20
            }
            "));

            var config = new Config(fileSystem);
            config.Load(TestConfigFilename);

            config.ConfigData.MyCallsign.Should().Be("BA1ZZZ");
            config.ConfigData.Gain.Should().BeApproximately(0.9f, 1e-6f);
            config.ConfigData.Frequency.Should().Be(300);
            config.ConfigData.Wpm.Should().Be(20);

            var messageTemplates = config.ConfigData.MessageTemplates;
            messageTemplates.Should().HaveCount(8);
            messageTemplates.Should().ContainInOrder(new MessageTemplateData[8]
            {
                new("TEST", "{TX} TEST", false),
                new("TEST 2", "{TX} TEST", true),
                new("RST", "{RX} DE {TX} UR RST 599 5NN"),
                new("{TX}", "{TX}", true),
                new("{RX}", "{RX}", true),
                new("73", "73", true),
                new("?", "?", true),
                new("NIL", "NIL", true),
            });
        }

        /// <summary>
        /// Tests <see cref="Config.Load(string)"/> with non-existing config file.
        /// </summary>
        [TestMethod]
        public void TestLoadNonExistingFile()
        {
            var fileSystem = new MockFileSystem();
            var config = new Config(fileSystem);
            config.Load(TestConfigFilename);
            config.ConfigData.Should().Be(new ConfigData());
        }

        /// <summary>
        /// Tests <see cref="Config.Load(string)"/> with invalid JSON content.
        /// </summary>
        /// <param name="jsonContent">The JSON content to test.</param>
        [DataTestMethod]
        [DataRow("")]
        [DataRow("null")]
        public void TestLoadInvalidJson(string jsonContent)
        {
            var fileSystem = new MockFileSystem();
            fileSystem.AddFile(TestConfigFilename, new MockFileData(jsonContent));

            var config = new Config(fileSystem);
            config.Load(TestConfigFilename);
            config.ConfigData.Should().Be(new ConfigData());
        }

        /// <summary>
        /// Tests <see cref="Config.Save(string)"/>.
        /// </summary>
        [TestMethod]
        public void TestSave()
        {
            var fileSystem = new MockFileSystem();

            var config = new Config(fileSystem);
            config.ConfigData.MyCallsign = "BA1ZZZ";
            config.Save(TestConfigFilename);

            fileSystem.FileExists(TestConfigFilename).Should().BeTrue();
            ConfigData result = JsonSerializer.Deserialize<ConfigData>(fileSystem.File.ReadAllText(TestConfigFilename))!;
            result.Should().BeEquivalentTo(config.ConfigData);
        }

        /// <summary>
        /// Tests <see cref="Config.Save(string)"/> when it fails to save.
        /// </summary>
        [TestMethod]
        public void TestSaveFailed()
        {
            var fileSystem = new MockFileSystem();
            fileSystem.AddFile(TestConfigFilename, new MockFileData(string.Empty));
            fileSystem.File.SetAttributes(TestConfigFilename, FileAttributes.ReadOnly);

            var config = new Config(fileSystem);
            config.Save(TestConfigFilename);

            fileSystem.File.ReadAllText(TestConfigFilename).Should().BeEmpty();
        }
    }
}
