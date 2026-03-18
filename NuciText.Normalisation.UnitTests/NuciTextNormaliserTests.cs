using NUnit.Framework;

namespace NuciText.Normalisation.UnitTests
{
    public class NuciTextNormaliserTests
    {
        INuciTextNormaliser normaliser;

        [SetUp]
        public void Setup()
        {
            normaliser = new NuciTextNormaliser();
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void GivenANullOrWhitespaceString_WhenNormalisingSentence_ThenTheResultIsEmpty(string input)
            => Assert.That(normaliser.NormaliseSentence(input), Is.Null);

        [Test]
        [TestCase("   Hello   world!   ")]
        [TestCase("This is a test.  ")]
        public void GivenAValidString_WhenNormalisingSentence_ThenTheResultIsNotNull(string input)
            => Assert.That(normaliser.NormaliseSentence(input), Is.Not.Null);

        [Test]
        [TestCase("   Hello   world!   ")]
        [TestCase("This is a test.  ")]
        public void GivenAValidString_WhenNormalisingSentence_ThenTheResultIsNotEmpty(string input)
            => Assert.That(normaliser.NormaliseSentence(input), Is.Not.Empty);

        [Test]
        [TestCase(
            "   Hello   world!   ",
            "Hello world!")]
        [TestCase(
            "Should we add a period here?",
            "Should we add a period here?")]
        [TestCase(
            "Test sentence is testy",
            "Test sentence is testy.")]
        [TestCase(
            "This is a test.  ",
            "This is a test.")]
        [TestCase(
            "this one should be capitalised. yes it should!",
            "This one should be capitalised. Yes it should!")]
        [TestCase(
            "Punctuation not added after version numbers such as 1.2.3 and others.",
            "Punctuation not added after version numbers such as 1.2.3 and others.")]
        public void GivenAValidString_WhenNormalisingSentence_ThenTheResultHasBeenNormaliseSentenced(string input, string expected)
            => Assert.That(normaliser.NormaliseSentence(input), Is.EqualTo(expected));
    }
}