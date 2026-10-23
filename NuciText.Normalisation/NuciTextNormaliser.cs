using System;
using System.Text.RegularExpressions;

namespace NuciText.Normalisation
{
    /// <summary>
    /// Implements the INuciTextNormaliser interface to provide functionality for normalising text.
    /// </summary>
    public sealed class NuciTextNormaliser : INuciTextNormaliser
    {
        private static readonly Regex PunctuationSpacingRegex = new(@"\s+([.,!?;:])", RegexOptions.Compiled);
        private static readonly Regex InSentencePunctuationSpacingRegex = new(@"([,!?;:])([^\s\d])", RegexOptions.Compiled);
        private static readonly Regex SentenceCapitalisationRegex = new(@"(^\s*[a-z])|([.!?]\s+[a-z])", RegexOptions.Compiled);
        private static readonly Regex TerminalPunctuationRegex = new(@"[.!?]$", RegexOptions.Compiled);

        /// <summary>
        /// Normalises the input sentence by trimming whitespace, removing repeated spaces, ensuring proper spacing around punctuation, capitalising the first letter of each sentence, and appending a period if necessary.
        /// </summary>
        /// <param name="input">The input sentence to normalise.</param>
        /// <returns>The normalised sentence.</returns>
        public string NormaliseSentence(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }

            string result = input!.Trim();

            result = RemoveRepeatedSpaces(result);
            result = PunctuationSpacingRegex.Replace(result, "$1");
            result = InSentencePunctuationSpacingRegex.Replace(result, "$1 $2");
            result = SentenceCapitalisationRegex.Replace(result, match => match.Value.ToUpper());

            if (!TerminalPunctuationRegex.IsMatch(result))
            {
                result += ".";
            }

            return result;
        }

        private static string RemoveRepeatedSpaces(string value)
        {
            while (value.Contains("  ", StringComparison.Ordinal))
            {
                value = value.Replace("  ", " ", StringComparison.Ordinal);
            }

            return value;
        }
    }
}
