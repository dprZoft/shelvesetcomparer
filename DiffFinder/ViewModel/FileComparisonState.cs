// <copyright file="FileComparisonState.cs" company="https://github.com/rajeevboobna/CompareShelvesets">Copyright https://github.com/rajeevboobna/CompareShelvesets. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.</copyright>

namespace DiffFinder
{
    /// <summary>
    /// The comparison outcome of a file listed in the comparison grid.
    /// </summary>
    public enum FileComparisonState
    {
        /// <summary>
        /// The file exists in both shelvesets with the same content.
        /// </summary>
        Matching,

        /// <summary>
        /// The file exists in both shelvesets with different content.
        /// </summary>
        Different,

        /// <summary>
        /// The file has no corresponding file in the other shelveset.
        /// </summary>
        MissingInOtherShelveset
    }
}
