// <copyright file="MorseConverter.cs" company="Helloworld">
// Copyright (c) Helloworld. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MorseCoder.Synthesizer.MorseSignalGenerator
{
    using System;

    /// <summary>
    /// The converter from letters to morse dots and dashes.
    /// </summary>
    internal static class MorseConverter
    {
        /// <summary>
        /// The separator between letters.
        /// </summary>
        private const char LetterSeparator = '/';

        /// <summary>
        /// The separator between words.
        /// </summary>
        private const char WordSeparator = ' ';

        /// <summary>
        /// The sign for starts of prosigns.
        /// </summary>
        private const char ProsignStart = '<';

        /// <summary>
        /// The sign for ends of prosigns.
        /// </summary>
        private const char ProsignEnd = '>';

        /// <summary>
        /// The conversion table between letters and morse codes.
        /// </summary>
        private static readonly Dictionary<char, string> LetterToMorseCode = new()
        {
            { 'A', ".-" },
            { 'B', "-..." },
            { 'C', "-.-." },
            { 'D', "-.." },
            { 'E', "." },
            { 'F', "..-." },
            { 'G', "--." },
            { 'H', "...." },
            { 'I', ".." },
            { 'J', ".---" },
            { 'K', "-.-" },
            { 'L', ".-.." },
            { 'M', "--" },
            { 'N', "-." },
            { 'O', "---" },
            { 'P', ".--." },
            { 'Q', "--.-" },
            { 'R', ".-." },
            { 'S', "..." },
            { 'T', "-" },
            { 'U', "..-" },
            { 'V', "...-" },
            { 'W', ".--" },
            { 'X', "-..-" },
            { 'Y', "-.--" },
            { 'Z', "--.." },
            { '0', "-----" },
            { '1', ".----" },
            { '2', "..---" },
            { '3', "...--" },
            { '4', "....-" },
            { '5', "....." },
            { '6', "-...." },
            { '7', "--..." },
            { '8', "---.." },
            { '9', "----." },
            { '.', ".-.-.-" },
            { ',', "--..--" },
            { '?', "..--.." },
            { '\'', ".----." },
            { '!', "-.-.--" },
            { '/', "-..-." },
            { '(', "-.--." },
            { ')', "-.--.-" },
            { '&', ".-..." },
            { ':', "---..." },
            { ';', "-.-.-." },
            { '=', "-...-" },
            { '+', "-.-.-" },
            { '-', "-....-" },
            { '_', "..--.-" },
            { '"', ".-..-." },
            { '$', "...-..-" },
            { '@', ".--.-." },
        };

        /// <summary>
        /// Converts message to morse codes.
        /// </summary>
        /// <param name="message">The plain text message.</param>
        /// <returns>Morse codes.</returns>
        /// <remarks>
        /// <para>Letters will be separated by <see cref="LetterSeparator"/>.</para>
        /// <para>Words will be separated by <see cref="WordSeparator"/>.</para>
        /// <para>Especially, prosigns will have no separators.</para>
        /// </remarks>
        /// <exception cref="InvalidCharacterException">Thrown when message contains invalid characters.</exception>
        public static string Convert(string message)
        {
            message = message.ToUpperInvariant();

            try
            {
                var wordsInMorseCode = message.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(word =>
                {
                    if (word.StartsWith(ProsignStart) && word.EndsWith(ProsignEnd))
                    {
                        // Prosigns are connected directly.
                        return word.TrimStart(ProsignStart).TrimEnd(ProsignEnd).ToCharArray().Select(c => LetterToMorseCode[c]).Aggregate((a, b) => a + b);
                    }

                    return string.Join(LetterSeparator, word.ToCharArray().Select(c => LetterToMorseCode[c]));
                });
                return string.Join(WordSeparator, wordsInMorseCode);
            }
            catch (KeyNotFoundException e)
            {
                throw new InvalidCharacterException(message, e);
            }
        }
    }
}
