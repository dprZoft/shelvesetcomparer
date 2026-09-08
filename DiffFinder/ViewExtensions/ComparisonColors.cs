// <copyright file="ComparisonColors.cs" company="https://github.com/rajeevboobna/CompareShelvesets">Copyright https://github.com/rajeevboobna/CompareShelvesets. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.</copyright>

using Microsoft.VisualStudio.PlatformUI;
using System;
using System.ComponentModel;
using System.Windows.Media;

namespace DiffFinder
{
    /// <summary>
    /// Provides the text colors used by the comparison grid for each <see cref="FileComparisonState"/>, following the current Visual Studio theme.
    /// </summary>
    public sealed class ComparisonColors : INotifyPropertyChanged
    {
        /// <summary>
        /// The relative luminance below which the tool window background is considered a dark theme.
        /// </summary>
        private const double DarkThemeLuminanceThreshold = 0.2;

        /// <summary>
        /// The color used for different files on a dark theme.
        /// </summary>
        private static readonly Color DifferentFilesDarkColor = Color.FromRgb(0xFF, 0x99, 0xA4);

        /// <summary>
        /// The color used for different files on a light theme.
        /// </summary>
        private static readonly Color DifferentFilesLightColor = Color.FromRgb(0xC5, 0x0F, 0x1F);

        /// <summary>
        /// The color used for files missing in the other shelveset on a dark theme.
        /// </summary>
        private static readonly Color MissingFileDarkColor = Color.FromRgb(0x62, 0xB5, 0xFF);

        /// <summary>
        /// The color used for files missing in the other shelveset on a light theme.
        /// </summary>
        private static readonly Color MissingFileLightColor = Color.FromRgb(0x0F, 0x6C, 0xBD);

        /// <summary>
        /// The text color used for matching files when the theme colors cannot be read. Light text, paired with the dark palette.
        /// </summary>
        private static readonly Color MatchingFilesFallbackColor = Color.FromRgb(0xF1, 0xF1, 0xF1);

        /// <summary>
        /// Static Instance Variable. A Singleton instance is used so that the XAML brushes can bind to it.
        /// </summary>
        private static readonly ComparisonColors instance = new ComparisonColors();

        /// <summary>
        /// Prevents a default instance of the <see cref="ComparisonColors"/> class from being created.
        /// </summary>
        private ComparisonColors()
        {
            this.UpdateColors();

            try
            {
                VSColorTheme.ThemeChanged += this.OnThemeChanged;
            }
            catch (Exception)
            {
                // The theme service may not be reachable yet. Without the subscription the colors simply
                // stay as computed above; throwing here would poison the static initializer for the whole process.
            }
        }

        /// <summary>
        /// Notification event used by view to update itself when any property changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the single instance of the comparison colors.
        /// </summary>
        public static ComparisonColors Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// Gets the text color used when the two files match.
        /// </summary>
        public Color MatchingFilesColor { get; private set; }

        /// <summary>
        /// Gets the text color used when the two files are different.
        /// </summary>
        public Color DifferentFilesColor { get; private set; }

        /// <summary>
        /// Gets the text color used when a file has no corresponding file in the other shelveset.
        /// </summary>
        public Color MissingFileColor { get; private set; }

        /// <summary>
        /// Converts a themed GDI color into a WPF media color.
        /// </summary>
        /// <param name="color">The GDI color</param>
        /// <returns>The equivalent WPF media color</returns>
        private static Color ToMediaColor(System.Drawing.Color color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        /// <summary>
        /// Computes the WCAG relative luminance of the given color.
        /// </summary>
        /// <param name="color">The color</param>
        /// <returns>The relative luminance, between 0 for black and 1 for white</returns>
        private static double GetRelativeLuminance(Color color)
        {
            return (0.2126 * ToLinear(color.R)) + (0.7152 * ToLinear(color.G)) + (0.0722 * ToLinear(color.B));
        }

        /// <summary>
        /// Converts an sRGB channel value into its linear value as defined by WCAG.
        /// </summary>
        /// <param name="channel">The sRGB channel value, between 0 and 255</param>
        /// <returns>The linear channel value, between 0 and 1</returns>
        private static double ToLinear(byte channel)
        {
            double value = channel / 255.0;
            return value <= 0.03928 ? value / 12.92 : System.Math.Pow((value + 0.055) / 1.055, 2.4);
        }

        /// <summary>
        /// Recomputes the colors when the Visual Studio theme changes.
        /// </summary>
        /// <param name="e">The event arguments</param>
        private void OnThemeChanged(ThemeChangedEventArgs e)
        {
            this.UpdateColors();
            this.NotifyPropertyChanged("MatchingFilesColor");
            this.NotifyPropertyChanged("DifferentFilesColor");
            this.NotifyPropertyChanged("MissingFileColor");
        }

        /// <summary>
        /// Reads the current theme and picks the colors accordingly.
        /// When the theme colors cannot be read, falls back to the dark palette with a light text color instead of throwing.
        /// </summary>
        private void UpdateColors()
        {
            bool isDarkTheme;
            Color matchingFilesColor;

            try
            {
                Color background = ToMediaColor(VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowBackgroundColorKey));
                isDarkTheme = GetRelativeLuminance(background) < DarkThemeLuminanceThreshold;
                matchingFilesColor = ToMediaColor(VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowTextColorKey));
            }
            catch (Exception)
            {
                // A failure here would surface as a TypeInitializationException from the static initializer, or as an
                // unhandled exception from the ThemeChanged handler. Either way the grid would lose its colors for good.
                isDarkTheme = true;
                matchingFilesColor = MatchingFilesFallbackColor;
            }

            this.MatchingFilesColor = matchingFilesColor;
            this.DifferentFilesColor = isDarkTheme ? DifferentFilesDarkColor : DifferentFilesLightColor;
            this.MissingFileColor = isDarkTheme ? MissingFileDarkColor : MissingFileLightColor;
        }

        /// <summary>
        /// The method raise the Property Changed event for the given property
        /// </summary>
        /// <param name="propertyName">The property for which the event needs to be raised</param>
        private void NotifyPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
