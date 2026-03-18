using System.Text.RegularExpressions;

namespace NuciText.Normalisation
{
    /// <summary>
    /// Implements the INuciTextNormaliser interface to provide functionality for normalising text.
    /// </summary>
    public sealed class NuciTextNormaliser : INuciTextNormaliser
    {
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

            string result = input.Trim();

            // Remove repeated spaces
            while (result.Contains("  "))
            {
                result = result.Replace("  ", " ");
            }

            // Remove spaces before punctuation
            result = Regex.Replace(result, @"\s+([.,!?;:])", "$1");

            // Ensure space after punctuation
            result = Regex.Replace(result, @"([.,!?;:])([^\s\d])", "$1 $2");

            // Capitalise first letter of each sentence
            result = Regex.Replace(result, @"(^\s*[a-z])|([.!?]\s*[a-z])", match =>
            {
                return match.Value.ToUpper();
            });

            // Append period if no ending punctuation
            if (!Regex.IsMatch(result, @"[.!?]$"))
            {
                result += ".";
            }

            return result;
        }
    }
}
