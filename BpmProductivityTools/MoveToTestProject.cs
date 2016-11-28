﻿//------------------------------------------------------------------------------
// <copyright file="MoveToTestProject.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace BpmProductivityTools {
	/// <summary>
	/// Command handler
	/// </summary>
	internal sealed class MoveToTestProject {
		/// <summary>
		/// Command ID.
		/// </summary>
		public const int CommandId = 0x0100;

		/// <summary>
		/// Command menu group (command set GUID).
		/// </summary>
		public static readonly Guid CommandSet = new Guid("1fce10c0-0613-49c1-a68b-5466fa58ec43");

		/// <summary>
		/// VS Package that provides this command, not null.
		/// </summary>
		private readonly Package package;

		/// <summary>
		/// Initializes a new instance of the <see cref="MoveToTestProject"/> class.
		/// Adds our command handlers for menu (commands must exist in the command table file)
		/// </summary>
		/// <param name="package">Owner package, not null.</param>
		private MoveToTestProject(Package package) {
			if (package == null) {
				throw new ArgumentNullException("package");
			}

			this.package = package;

			OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
			if (commandService != null) {
				var menuCommandID = new CommandID(CommandSet, CommandId);
				var menuItem = new OleMenuCommand(this.MoveFiles, menuCommandID);
				menuItem.BeforeQueryStatus += menuCommand_BeforeQueryStatus;
				commandService.AddCommand(menuItem);
			}
		}

		void menuCommand_BeforeQueryStatus(object sender, EventArgs e) {
			// get the menu that fired the event
			var menuCommand = sender as OleMenuCommand;
			if (menuCommand != null) {
				// start by assuming that the menu will not be shown
				menuCommand.Visible = false;
				menuCommand.Enabled = false;

				
				menuCommand.Visible = true;
				menuCommand.Enabled = true;
			}
		}
		public static bool IsSingleProjectItemSelection(out IVsHierarchy hierarchy, out uint itemid) {
			hierarchy = null;
			itemid = VSConstants.VSITEMID_NIL;
			int hr = VSConstants.S_OK;

			var monitorSelection = Package.GetGlobalService(typeof(SVsShellMonitorSelection)) as IVsMonitorSelection;
			var solution = Package.GetGlobalService(typeof(SVsSolution)) as IVsSolution;
			if (monitorSelection == null || solution == null) {
				return false;
			}

			IVsMultiItemSelect multiItemSelect = null;
			IntPtr hierarchyPtr = IntPtr.Zero;
			IntPtr selectionContainerPtr = IntPtr.Zero;

			try {
				hr = monitorSelection.GetCurrentSelection(out hierarchyPtr, out itemid, out multiItemSelect, out selectionContainerPtr);

				if (ErrorHandler.Failed(hr) || hierarchyPtr == IntPtr.Zero || itemid == VSConstants.VSITEMID_NIL) {
					// there is no selection
					return false;
				}

				// multiple items are selected
				if (multiItemSelect != null)
					return false;

				// there is a hierarchy root node selected, thus it is not a single item inside a project

				if (itemid == VSConstants.VSITEMID_ROOT)
					return false;

				hierarchy = Marshal.GetObjectForIUnknown(hierarchyPtr) as IVsHierarchy;
				if (hierarchy == null)
					return false;

				Guid guidProjectID = Guid.Empty;

				if (ErrorHandler.Failed(solution.GetGuidOfProject(hierarchy, out guidProjectID))) {
					return false; // hierarchy is not a project inside the Solution if it does not have a ProjectID Guid
				}

				// if we got this far then there is a single project item selected
				return true;
			} finally {
				if (selectionContainerPtr != IntPtr.Zero) {
					Marshal.Release(selectionContainerPtr);
				}

				if (hierarchyPtr != IntPtr.Zero) {
					Marshal.Release(hierarchyPtr);
				}
			}
		}
		/// <summary>
		/// Gets the instance of the command.
		/// </summary>
		public static MoveToTestProject Instance {
			get;
			private set;
		}

		/// <summary>
		/// Gets the service provider from the owner package.
		/// </summary>
		private IServiceProvider ServiceProvider {
			get {
				return this.package;
			}
		}

		/// <summary>
		/// Initializes the singleton instance of the command.
		/// </summary>
		/// <param name="package">Owner package, not null.</param>
		public static void Initialize(Package package) {
			Instance = new MoveToTestProject(package);
		}

		/// <summary>
		/// This function is the callback used to execute the command when the menu item is clicked.
		/// See the constructor to see how the menu item is associated with this function using
		/// OleMenuCommandService service and MenuCommand class.
		/// </summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		private void MoveFiles(object sender, EventArgs e) {
			IVsHierarchy hierarchy = null;
			uint itemid = VSConstants.VSITEMID_NIL;

			if (!IsSingleProjectItemSelection(out hierarchy, out itemid))
				return;
			// Get the file path
			string itemFullPath = null;
			((IVsProject)hierarchy).GetMkDocument(itemid, out itemFullPath);
			var transformFileInfo = new FileInfo(itemFullPath);

			string message = transformFileInfo.Name;

			// Show a message box to prove we were here
			VsShellUtilities.ShowMessageBox(
				this.ServiceProvider,
				message,
				"Hello",
				OLEMSGICON.OLEMSGICON_INFO,
				OLEMSGBUTTON.OLEMSGBUTTON_OK,
				OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
		}
	}
}
