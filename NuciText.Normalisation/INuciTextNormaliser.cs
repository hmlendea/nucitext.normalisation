namespace NuciText.Normalisation
{
    /// <summary>
    /// Interface for normalising text.
    /// </summary>
    public interface INuciTextNormaliser
    {
        /// <summary>
        /// Normalises the input sentence by trimming whitespace, removing repeated spaces, ensuring proper spacing around punctuation, capitalising the first letter of each sentence, and appending a period if necessary.
        /// </summary>
        /// <param name="input">The input sentence to normalise.</param>
        /// <returns>The normalised sentence.</returns>
        string NormaliseSentence(string input);
    }
}
