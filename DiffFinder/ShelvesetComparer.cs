﻿// <copyright file="ShelvesetComparer.cs" company="https://github.com/rajeevboobna/CompareShelvesets">Copyright https://github.com/rajeevboobna/CompareShelvesets. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.</copyright>

using Microsoft.TeamFoundation.Controls;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;

namespace DiffFinder
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class ShelvesetComparer
    {
        /// <summary>
        /// Command ID for Result window
        /// </summary>
        public const int ShelvesetComparerResuldId = 0x0100;

        /// <summary>
        /// Command ID for TeamExplorer compare view
        /// </summary>
        public const int ShelvesetComparerTeamExplorerViewId = 0x0200;

        /// <summary>
        /// Dte Command name for Result window, keep in sync with vsct Button names (removing special characters and whitespaces)
        /// </summary>
        public const string ShelvesetComparerResuldIdDteCommandName = "Team.DiffFinderResults";

        /// <summary>
        /// Dte Command name for Result window, keep in sync with vsct Button names (removing special characters and whitespaces)
        /// </summary>
        public const string ShelvesetComparerTeamExplorerViewIdDteCommandName = "Team.DiffFinderSelect";

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("b8e98565-7b6d-4d64-b51d-97fe5e56c5ec");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShelvesetComparer"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private ShelvesetComparer(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            this.package = package;
            
            TraceOutput("Initializing Package ..");

            OleMenuCommandService commandService = this.ServiceProvider.GetService<IMenuCommandService, OleMenuCommandService>();
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, ShelvesetComparerResuldId);
                var menuItem = new MenuCommand(this.ShelvesetComparerResuldIdMenuItemCallback, menuCommandID);
                commandService.AddCommand(menuItem);

                menuCommandID = new CommandID(CommandSet, ShelvesetComparerTeamExplorerViewId);
                menuItem = new MenuCommand(this.ShelvesetComparerTeamExplorerViewIdMenuItemCallback, menuCommandID);
                commandService.AddCommand(menuItem);
            }

            TraceOutput("Package initialized.");
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static ShelvesetComparer Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new ShelvesetComparer(package);
        }

        /// <summary>
        /// Open and show ShelvesetComparer result window.
        /// </summary>
        public void ShowComparisonWindow()
        {
            ToolWindowPane window = package.FindToolWindow(typeof(ShelvesetComparerToolWindow), 0, true);
            if ((null == window) || (null == window.Frame))
            {
                throw new NotSupportedException(Resources.CanNotCreateWindow);
            }

            IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
            windowFrame.Show();
        }

        /// <summary>
        /// Menu item callback for "Team - Results view" item.
        /// 
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void ShelvesetComparerResuldIdMenuItemCallback(object sender, EventArgs e)
        {
            this.ShowComparisonWindow();
        }

        /// <summary>
        /// Menu item callback for "Team - Select view" item.
        /// 
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void ShelvesetComparerTeamExplorerViewIdMenuItemCallback(object sender, EventArgs e)
        {
            NavigateToShelvestComparerPage();
        }

        /// <summary>
        /// Open ShelvesetComparer select page in TeamExplorer
        /// </summary>
        public void NavigateToShelvestComparerPage()
        {
            var teamExplorer = ServiceProvider.GetService<ITeamExplorer>();
            teamExplorer.NavigateToShelvesetComparer();
        }

        /// <summary>
        /// Write trace to output (only if trace is enabled)
        /// </summary>
        public void TraceOutput(string text)
        {
#if TRACE
            OutputPaneWriteLine(text);
#endif
        }

        /// <summary>
        /// Write to own output pane in output window with optional DateTime prefix and activate pane afterwards.
        /// </summary>
        public void OutputPaneWriteLine(string text, bool prefixDateTime = true)
        {
            OutputPaneWriteLine(this.ServiceProvider, text, prefixDateTime);
        }

        /// <summary>
        /// Write text with optional DateTime prefix to own output pane (create if not existing) and activate pane afterwards.
        /// </summary>
        public static void OutputPaneWriteLine(IServiceProvider serviceProvider, string text, bool prefixDateTime = true)
        {
            var outWindow = serviceProvider.GetService<SVsOutputWindow, IVsOutputWindow>();
            if (outWindow == null)
            {
                return;
            }

            // randomly generated GUID to identify the "Shelveset Comparer" output window pane
            const string c_ExtensionOutputWindowGuid = "{38BFBA25-8AB3-4F8E-B992-930E403AA281}";
            Guid paneGuid = new Guid(c_ExtensionOutputWindowGuid);
            IVsOutputWindowPane generalPane = null;
            outWindow.GetPane(ref paneGuid, out generalPane);
            if (generalPane == null)
            {
                // the pane doesn't already exist
                outWindow.CreatePane(ref paneGuid, Resources.ToolWindowTitle, Convert.ToInt32(true), Convert.ToInt32(true));
                outWindow.GetPane(ref paneGuid, out generalPane);
            }

            if (prefixDateTime)
            {
                text = $"{DateTime.Now:G} {text}";
            }
            generalPane.OutputString(text + Environment.NewLine);
            generalPane.Activate();
        }
    }
}
