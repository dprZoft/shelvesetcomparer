using System.Text;
using System.Text.RegularExpressions;

namespace DiffFinder
{
    /// <summary>
    /// Builds the command line arguments passed to the external difference tool configured in Visual Studio.
    /// </summary>
    public static class DiffToolArgumentBuilder
    {
        /// <summary>
        /// Matches the diff tool argument placeholders %1 through %9
        /// </summary>
        private static readonly Regex Placeholders = new Regex("%[1-9]", RegexOptions.Compiled);

        /// <summary>
        /// Substitutes the diff tool argument placeholders, quoting each substituted value so that paths
        /// and labels containing spaces are passed to the tool as a single argument.
        /// %1: Original file, %2: Modified file, %3: Base file, %4: Merged file, %5: Diff command-line options,
        /// %6: Original file label, %7: Modified file label, %8, %9: Base file and merged file label.
        /// Only %1, %2, %6 and %7 are substituted, the remaining placeholders are left untouched.
        /// A placeholder the template already wraps in quotes is substituted without adding another pair.
        /// </summary>
        /// <param name="argumentsTemplate">The arguments template configured for the tool</param>
        /// <param name="firstFileName">The path of the original file</param>
        /// <param name="secondFileName">The path of the modified file</param>
        /// <param name="firstDisplayName">The label of the original file</param>
        /// <param name="secondDisplayName">The label of the modified file</param>
        /// <returns>The arguments to pass to the tool</returns>
        public static string Build(string argumentsTemplate, string firstFileName, string secondFileName, string firstDisplayName, string secondDisplayName)
        {
            if (string.IsNullOrEmpty(argumentsTemplate))
            {
                return argumentsTemplate;
            }

            return Placeholders.Replace(argumentsTemplate, match =>
            {
                string value;
                switch (match.Value)
                {
                    case "%1":
                        value = firstFileName;
                        break;
                    case "%2":
                        value = secondFileName;
                        break;
                    case "%6":
                        value = firstDisplayName;
                        break;
                    case "%7":
                        value = secondDisplayName;
                        break;
                    default:
                        // the placeholder is not supported, leave it as it is
                        return match.Value;
                }

                value = Escape(value);

                // if the template already wraps the placeholder in quotes there is no need to add another pair
                var precededByQuote = match.Index > 0 && argumentsTemplate[match.Index - 1] == '"';
                var followedByQuote = match.Index + match.Length < argumentsTemplate.Length && argumentsTemplate[match.Index + match.Length] == '"';

                return precededByQuote && followedByQuote ? value : "\"" + value + "\"";
            });
        }

        /// <summary>
        /// Escapes a value so that it survives being wrapped in double quotes, following the rules the
        /// Windows command line parser applies: a double quote is escaped with a backslash, and every
        /// backslash preceding a double quote, including the closing one, is doubled.
        /// </summary>
        /// <param name="value">The value to escape</param>
        /// <returns>The escaped value</returns>
        private static string Escape(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            var escaped = new StringBuilder(value.Length);
            var backslashes = 0;
            foreach (var character in value)
            {
                if (character == '\\')
                {
                    backslashes++;
                    continue;
                }

                if (character == '"')
                {
                    escaped.Append('\\', (backslashes * 2) + 1);
                }
                else
                {
                    escaped.Append('\\', backslashes);
                }

                backslashes = 0;
                escaped.Append(character);
            }

            // the trailing backslashes precede the closing quote, so they have to be doubled as well
            escaped.Append('\\', backslashes * 2);

            return escaped.ToString();
        }
    }
}
