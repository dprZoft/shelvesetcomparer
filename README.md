# ShelvesetComparer

A visual studio extension that allow users to compere contents of two shelvesets.

If the same file path cannot be found, the relative path is checked to allow comparison of shelvesets targeting different branches, e.g. `$/BranchA/path/to/file1` with `$/BranchB/path/to/file1`.

This fork extends the existing **[hamidshahid / shelvesetcomparer](https://github.com/hamidshahid/shelvesetcomparer)** Visual Studio extension.

Color coding used in file comparison:

* **black**: no difference / both files are identical
* **red**: differences / both files differ
* **blue**: file exists only in one of the compared containers (Shelveset or Pending Changes)

## VisualStudio Marketplace

* DiffFinder (extends ShelvesetComparer functionality to compare with Pending Changes)
  * [DiffFinder for VS2022](https://marketplace.visualstudio.com/items?itemName=dprZoft.DiffFinder-VS2022)
  * [DiffFinder for VS2019](https://marketplace.visualstudio.com/items?itemName=dprZoft.DiffFinder-VS2019)
* ShelvesetComparer:
  * [ShelvesetComparer for VS2022 (see releases)](https://github.com/dprZoft/shelvesetcomparer/releases)
  * [ShelvesetComparer for VS2019](https://marketplace.visualstudio.com/items?itemName=dprZoft.ShelvesetComparer-VS2019)
* Previous versions:
  * [DiffFinder (VS2017)](https://marketplace.visualstudio.com/items?itemName=RajeevBoobna.DiffFinder)
  * [ShelvesetComparer (VS2017)](https://marketplace.visualstudio.com/items?itemName=HamidShahid.ShelvesetComparer-19329)

## Examples

1. Open Shelveset selection from TeamExplorer:
![TeamExplorer Diff Finder Button](/ShelvesetComparer/Resources/PreviewImage.png)

2. Select two Shelvesets for comparison:
<!--![TeamExplorer Diff Finder Button](/ShelvesetComparer/Resources/Example_SelectCompareShelvesets.png)-->

3. File comparison view comparing two Shelvesets:

   * Shelvesets with same file paths (same branch):
   <!--![TeamExplorer Diff Finder Button](/ShelvesetComparer/Resources/Example_CompareSameBranch.png)-->

   * Shelvesets with different paths (algorithm tries to find the best match (most common path parts)):
      * differerent branches but same relative path:
      <!--![TeamExplorer Diff Finder Button](/ShelvesetComparer/Resources/Example_CompareDifferentBranches.png)-->

      * different branches and different paths:
      <!--![File comparison: different branches and folders -> tries to find best match](/ShelvesetComparer/Resources/Example_CompareDifferentBranchesAndFolders.png)-->

## Branches and tags

* DiffFinder (remote / difffinder: https://github.com/rajeevboobna/shelvesetcomparer)
  * `main/master`: equivalent to latest releases/ branch
  * `releases/`*: release branches for corresponding VS version
  * Release tags: `DiffFinder-vN.N.N.N`
* ShelvesetComparer (remote / upstream: https://github.com/hamidshahid/shelvesetcomparer)
  * `SC/`*: ShelvesetComparer branches with same logic as for DiffFinder
  * Release tags: `vN.N.N.N`
