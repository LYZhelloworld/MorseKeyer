// <copyright file="AssemblyInfo.cs" company="Helloworld">
// Copyright (c) Helloworld. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Windows;

[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, // where theme specific resource dictionaries are located (used if a resource is not found in the page, or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly) // where the generic resource dictionary is located (used if a resource is not found in the page, app, or any theme specific resource dictionaries)
]
[assembly: CLSCompliant(true)]
[assembly: ExcludeFromCodeCoverage]
[assembly: InternalsVisibleTo("MorseKeyer.Wpf.FunctionalTest")]
