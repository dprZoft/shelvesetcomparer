using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DiffFinder.Tests
{
    /// <summary>
    /// Tests for <see cref="DiffToolArgumentBuilder"/>.
    /// </summary>
    [TestClass]
    public class DiffToolArgumentBuilderTests
    {
        private const string FirstFile = @"C:\Users\first last\AppData\Local\Temp\tmp1A2B.tmp";

        private const string SecondFile = @"C:\Users\first last\AppData\Local\Temp\tmp3C4D.tmp";

        private const string FirstLabel = "$/Project/My Folder/File.cs;Shelveset One";

        private const string SecondLabel = "$/Project/My Folder/File.cs;Shelveset Two";

        /// <summary>
        /// The regression test for https://github.com/dprZoft/shelvesetcomparer/issues/17: an unquoted
        /// template has to produce two arguments, not the four or more the tool would read as a 3-way merge.
        /// </summary>
        [TestMethod]
        public void Build_UnquotedTemplateWithSpacesInPaths_QuotesBothFiles()
        {
            var arguments = DiffToolArgumentBuilder.Build("%1 %2", FirstFile, SecondFile, FirstLabel, SecondLabel);

            Assert.AreEqual($"\"{FirstFile}\" \"{SecondFile}\"", arguments);
        }

        [TestMethod]
        public void Build_TemplateAlreadyQuoted_DoesNotAddASecondPairOfQuotes()
        {
            var arguments = DiffToolArgumentBuilder.Build("\"%1\" \"%2\"", FirstFile, SecondFile, FirstLabel, SecondLabel);

            Assert.AreEqual($"\"{FirstFile}\" \"{SecondFile}\"", arguments);
        }

        [TestMethod]
        public void Build_TemplateQuotesOnlySomePlaceholders_QuotesOnlyTheUnquotedOnes()
        {
            var arguments = DiffToolArgumentBuilder.Build("\"%1\" %2", FirstFile, SecondFile, FirstLabel, SecondLabel);

            Assert.AreEqual($"\"{FirstFile}\" \"{SecondFile}\"", arguments);
        }

        [TestMethod]
        public void Build_TemplateWithLabels_QuotesTheLabels()
        {
            var arguments = DiffToolArgumentBuilder.Build("%1 %2 /title1=%6 /title2=%7", FirstFile, SecondFile, FirstLabel, SecondLabel);

            Assert.AreEqual($"\"{FirstFile}\" \"{SecondFile}\" /title1=\"{FirstLabel}\" /title2=\"{SecondLabel}\"", arguments);
        }

        [TestMethod]
        public void Build_TemplateWithQuotedLabels_DoesNotAddASecondPairOfQuotes()
        {
            var arguments = DiffToolArgumentBuilder.Build("/title1=\"%6\" /title2=\"%7\"", FirstFile, SecondFile, FirstLabel, SecondLabel);

            Assert.AreEqual($"/title1=\"{FirstLabel}\" /title2=\"{SecondLabel}\"", arguments);
        }

        [TestMethod]
        public void Build_UnsupportedPlaceholders_AreLeftUntouched()
        {
            var arguments = DiffToolArgumentBuilder.Build("%3 %4 %5 %8 %9", FirstFile, SecondFile, FirstLabel, SecondLabel);

            Assert.AreEqual("%3 %4 %5 %8 %9", arguments);
        }

        [TestMethod]
        public void Build_ValueContainingAPlaceholder_IsNotSubstitutedAgain()
        {
            var firstFile = @"C:\Temp\a %2 b.cs";

            var arguments = DiffToolArgumentBuilder.Build("%1 %2", firstFile, SecondFile, FirstLabel, SecondLabel);

            Assert.AreEqual($"\"{firstFile}\" \"{SecondFile}\"", arguments);
        }

        [TestMethod]
        public void Build_ValueContainingAQuote_EscapesTheQuote()
        {
            var arguments = DiffToolArgumentBuilder.Build("%1", "C:\\Temp\\a\"b.cs", SecondFile, FirstLabel, SecondLabel);

            Assert.AreEqual("\"C:\\Temp\\a\\\"b.cs\"", arguments);
        }

        [TestMethod]
        public void Build_ValueEndingWithABackslash_DoublesTheTrailingBackslash()
        {
            var arguments = DiffToolArgumentBuilder.Build("%1", @"C:\Temp\my folder\", SecondFile, FirstLabel, SecondLabel);

            Assert.AreEqual("\"C:\\Temp\\my folder\\\\\"", arguments);
        }

        [TestMethod]
        public void Build_QuotedTemplateAndValueEndingWithABackslash_DoublesTheTrailingBackslash()
        {
            var arguments = DiffToolArgumentBuilder.Build("\"%1\"", @"C:\Temp\my folder\", SecondFile, FirstLabel, SecondLabel);

            Assert.AreEqual("\"C:\\Temp\\my folder\\\\\"", arguments);
        }

        [TestMethod]
        public void Build_NullValue_ProducesAnEmptyArgument()
        {
            var arguments = DiffToolArgumentBuilder.Build("%1 %2", null, SecondFile, FirstLabel, SecondLabel);

            Assert.AreEqual($"\"\" \"{SecondFile}\"", arguments);
        }

        [TestMethod]
        public void Build_NullTemplate_ReturnsNull()
        {
            var arguments = DiffToolArgumentBuilder.Build(null, FirstFile, SecondFile, FirstLabel, SecondLabel);

            Assert.IsNull(arguments);
        }

        [TestMethod]
        public void Build_EmptyTemplate_ReturnsEmpty()
        {
            var arguments = DiffToolArgumentBuilder.Build(string.Empty, FirstFile, SecondFile, FirstLabel, SecondLabel);

            Assert.AreEqual(string.Empty, arguments);
        }

        [TestMethod]
        public void Build_TemplateWithoutPlaceholders_IsLeftUntouched()
        {
            var arguments = DiffToolArgumentBuilder.Build("/e /u /wl", FirstFile, SecondFile, FirstLabel, SecondLabel);

            Assert.AreEqual("/e /u /wl", arguments);
        }
    }
}
