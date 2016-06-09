#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
{                                                                   }
{       Copyright (c) 2000-2015 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2015 Developer Express Inc.

using System;
using EnvDTE;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.VisualStudio.Shell.Interop;
using DevExpress.Design.Mvvm.Wizards;
using DevExpress.Utils.Design;
namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm {
	static class BuildHelper {		
		static UIHierarchy solutionExplorer;
		static DTE dte;
		static Window solutionExplorerWindow;		
		static void GetAllProjectsHierarhies(UIHierarchyItems items, List<UIHierarchyItem> result, List<string> projects) {
			if(items == null || items.Count == 0 || result == null)
				return;
			foreach(UIHierarchyItem item in items) {
				EnvDTE.Project project = item.Object as EnvDTE.Project;
				if(project == null) {
					EnvDTE.ProjectItem projectItem = item.Object as EnvDTE.ProjectItem;
					if(projectItem != null)
						project = projectItem.SubProject;
				}
				if(project == null)
					continue;
				if(!string.IsNullOrEmpty(project.FullName) && File.Exists(project.FullName)) {
					if(projects.Contains(project.FullName))
						result.Add(item);
				}
				else
					GetAllProjectsHierarhies(item.UIHierarchyItems, result, projects);
			}
		}
		static IVsWindowFrame GetFrame(Window window) {
			if(window == null)
				return null;
			try {
				Guid lSlot = new Guid(window.ObjectKind);
				IVsWindowFrame frame;
				VsUIShell.FindToolWindow((uint)__VSFINDTOOLWIN.FTW_fFrameOnly, ref lSlot, out frame);
				return frame;
			}
			catch(Exception ex) {
				Log.SendException(ex);
			}
			return null;
		}
		public static IVsUIShell VsUIShell {
			get { return DTEHelper.Query<SVsUIShell, IVsUIShell>(); }
		}
		static List<IVsWindowFrame> GetVisibleFrames() {
			List<IVsWindowFrame> result = new List<IVsWindowFrame>();
			if(Dte.MainWindow.Collection == null && Dte.MainWindow.Collection.Count == 0)
				return result;
			for(int i = 1; i <= Dte.MainWindow.Collection.Count; i++) {
				try {
					Window window = Dte.MainWindow.Collection.Item(i);
					if(window == null)
						continue;
					IVsWindowFrame frame = GetFrame(window);
					if(frame == null)
						continue;
					int onScreen;
					if(frame.IsOnScreen(out onScreen) != 0 || onScreen != 1)
						continue;
					result.Add(frame);
				}
				catch(Exception ex) {
					Log.SendException(ex);
				}
			}
			return result;
		}
		static bool BuildProjects(List<string> projects) {
			if(projects == null || SolutionExplorer.UIHierarchyItems.Count == 0)
				return false;
			Array selected = (Array)SolutionExplorer.SelectedItems;
			UIHierarchyItem solutionItem = SolutionExplorer.UIHierarchyItems.Item(1);
			Window activeWindow = DTEHelper.GetCurrentDTE().ActiveWindow;
			List<IVsWindowFrame> visibleFrames = GetVisibleFrames();
			bool visible = SolutionExplorerWindow.Visible;
			SolutionExplorerWindow.Activate();
			try {
				List<UIHierarchyItem> hierarhyItems = new List<UIHierarchyItem>();
				GetAllProjectsHierarhies(solutionItem.UIHierarchyItems, hierarhyItems, projects);
				bool first = true;
				foreach(UIHierarchyItem item in hierarhyItems) {
					item.Select(first ? vsUISelectionType.vsUISelectionTypeSelect : vsUISelectionType.vsUISelectionTypeToggle);
					first = false;
				}
				return Build();
			}
			catch {
				return false;
			}
			finally {
				if(!visible)
					SolutionExplorerWindow.Visible = false;
				foreach(IVsWindowFrame frame in visibleFrames)
					frame.Show();
				if(activeWindow != null)
					activeWindow.Activate();
				RestoreSelection(selected);
			}
		}
		static void RestoreSelection(Array selected) {
			bool first = true;
			foreach(UIHierarchyItem item in selected) {
				if(first)
					item.Select(vsUISelectionType.vsUISelectionTypeSelect);
				else
					item.Select(vsUISelectionType.vsUISelectionTypeToggle);
				first = false;
			}
		}
		static bool Build() {
			SolutionBuild solBuild = Dte.Solution.SolutionBuild;
			if(!BuildFromCommand())
				solBuild.Build(true);
			switch(solBuild.BuildState) {
				case vsBuildState.vsBuildStateNotStarted:
				case vsBuildState.vsBuildStateInProgress:
					return false;
				case vsBuildState.vsBuildStateDone:
					return (solBuild.LastBuildInfo == 0);
			}
			return false;
		}
		static bool BuildFromCommand() {
			Command command = Dte.Commands.Item("{5EFC7975-14BC-11CF-9B2B-00AA00573819}", 0x376); 
			if(!command.IsAvailable)
				return false;
			object customIn = null;
			object customOut = null;
			Dte.Commands.Raise(command.Guid, command.ID, ref customIn, ref customOut);
			SolutionBuild solutionBuild = Dte.Solution.SolutionBuild;
			while(solutionBuild.BuildState == vsBuildState.vsBuildStateInProgress)
				Application.DoEvents();
			return true;
		}
		static DTE Dte {
			get {
				if(dte != null)
					return dte;
				dte = DTEHelper.GetCurrentDTE();
				return dte;
			}
		}
		static UIHierarchy SolutionExplorer {
			get {
				if(solutionExplorer != null)
					return solutionExplorer;
				if(SolutionExplorerWindow == null)
					return null;
				solutionExplorer = SolutionExplorerWindow.Object as UIHierarchy;
				return solutionExplorer;
			}
		}
		static Window SolutionExplorerWindow {
			get {
				if(solutionExplorerWindow != null)
					return solutionExplorerWindow;
				if(Dte == null)
					return null;
				solutionExplorerWindow = Dte.Windows.Item("{3AE79031-E1BC-11D0-8F78-00A0C9110057}");
				return solutionExplorerWindow;
			}
		}
		public static bool BuildProjects(params string[] projectNames) {
			if(projectNames == null || projectNames.Length == 0)
				return false;
			List<string> references = new List<string>();
			foreach(string projectName in projectNames)
				references.Add(projectName);
			return BuildProjects(references);
		}
	}
}
