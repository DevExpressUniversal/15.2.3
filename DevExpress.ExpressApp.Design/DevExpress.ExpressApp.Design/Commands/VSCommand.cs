#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.ExpressApp.Design.Core;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using stdole;
using DevExpress.ExpressApp.Design.Commands;
namespace DevExpress.ExpressApp.Design {
	public class CommandCreator {
		private IServiceProvider serviceProvider;
		private List<VSCommand> createdCommands = new List<VSCommand>();
		public CommandCreator(IServiceProvider serviceProvider) {
			this.serviceProvider = serviceProvider;
		}
		public bool CreateCommands() {
			VSCommand command = new DevExpress.ExpressApp.Design.UpdateModelCommand(serviceProvider);
			createdCommands.Add(command);
			command = new DevExpress.ExpressApp.Design.UpdateModelsCommand(serviceProvider);
			createdCommands.Add(command);
			command = new DevExpress.ExpressApp.Design.Commands.OpenModelEditorCommand(serviceProvider);
			createdCommands.Add(command);
			command = new DevExpress.ExpressApp.Design.Commands.MergeDifferencesCommand(serviceProvider);
			createdCommands.Add(command);
			command = new DevExpress.ExpressApp.Design.Commands.MergeUserDifferencesCommand(serviceProvider);
			createdCommands.Add(command);
			command = new DevExpress.ExpressApp.Design.Commands.EasyTestRunCommand(serviceProvider);
			createdCommands.Add(command);
			command = new DevExpress.ExpressApp.Design.Commands.EasyTestRunNextStepCommand(serviceProvider);
			createdCommands.Add(command);
			command = new DevExpress.ExpressApp.Design.Commands.EasyTestRunToCursorCommand(serviceProvider);
			createdCommands.Add(command);
			command = new DevExpress.ExpressApp.Design.Commands.EasyTestStopRunningCommand(serviceProvider);
			createdCommands.Add(command);
			return true;
		}
		public void RemoveCommands() {
			foreach(VSCommand command in createdCommands) {
				command.Dispose();
			}
			createdCommands.Clear();
		}
	}
	public abstract class VSCommand : IDisposable {
		protected _DTE dte;
		protected System.IServiceProvider serviceProvider;
		protected Boolean skipXafVersionChecking;
		private OleMenuCommand oleMenuCommand;
		private bool CheckXafVersion() {
			EasyTestManager.TraceMethodEnter(this, "CheckXafVersion");
			Object[] activeProjects = (Object[])dte.ActiveSolutionProjects;
			bool result = false;
			EasyTestManager.TraceValue("activeProjects.Length", activeProjects.Length);
			if(activeProjects.Length == 0) {
				result = CheckProjectsVersion();
			}
			else {
				result = CheckProjectVersion(activeProjects);
			}
			EasyTestManager.TraceValue("result", result);
			EasyTestManager.TraceMethodExit(this, "CheckXafVersion");
			return result;
		}
		private bool CheckProjectsVersion() {
			string version = "0";
			bool toDefault = false;
			foreach(BuildDependency buildDependancy in dte.Solution.SolutionBuild.BuildDependencies) {
				ProjectWrapper projectWrapper = null;
				try {
					projectWrapper = ProjectWrapper.Create(buildDependancy.Project);
				}
				catch(NotImplementedException) {
				}
				string postfix = projectWrapper.GetXafVersionPostfix();
				if(postfix != null) {
					toDefault = true;
					foreach(char ch in postfix) {
						if(char.IsNumber(ch)) {
							version += ch;
						}
					}
					if(Convert.ToInt32(version) < 82) { return false; }
					version = "0";
				}
			}
			return toDefault;
		}
		private bool CheckProjectVersion(object[] activeProjects) {
			string version = "0";
			foreach(object project in activeProjects) {
				ProjectWrapper projectWrapper = ProjectWrapper.Create((Project)project);
				string postfix = projectWrapper.GetXafVersionPostfix();
				if(postfix != null) {
					foreach(char ch in postfix) {
						if(char.IsNumber(ch)) {
							version += ch;
						}
					}
					break;
				}
			}
			return Convert.ToInt32(version) >= 82;
		}
		private void oleMenuCommand_BeforeQueryStatus(Object sender, EventArgs e) {
			EasyTestManager.TraceMethodEnter(this, "oleMenuCommand_BeforeQueryStatus");
			EasyTestManager.TraceValue("sender", sender.GetType());
			MenuCommand command = sender as MenuCommand;
			if(command != null) {
				EasyTestManager.TraceValue("skipXafVersionChecking", skipXafVersionChecking);
				vsCommandStatus status = QueryStatus();
				command.Visible =
					(status == (vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled))
					&&
					(skipXafVersionChecking || CheckXafVersion());
				command.Enabled = command.Visible;
			}
			EasyTestManager.TraceMethodExit(this, "oleMenuCommand_BeforeQueryStatus");
		}
		protected Project FindSelectedProject() {
			if(((object[])dte.ActiveSolutionProjects).Length > 0) {
				return (Project)((object[])dte.ActiveSolutionProjects)[0];
			}
			return null;
		}
		protected void CollectProjectsFromFolder(Project folder, List<Project> projects) {
			foreach(ProjectItem projectItem in folder.ProjectItems) {
				Project project = projectItem.Object as Project;
				if(project != null) {
					if(project.Kind == EnvDTE80.ProjectKinds.vsProjectKindSolutionFolder) {
						CollectProjectsFromFolder(project, projects);
					}
					else {
						projects.Add(project);
					}
				}
			}
		}
		protected List<Project> GetProcessedProjects() {
			List<Project> projects = new List<Project>();
			Project project = FindSelectedProject();
			if(project != null) {
				if(project.Kind == EnvDTE80.ProjectKinds.vsProjectKindSolutionFolder) {
					CollectProjectsFromFolder(project, projects);
				}
				else {
					projects.Add(project);
				}
			}
			else {
				foreach(BuildDependency buildDependancy in dte.Solution.SolutionBuild.BuildDependencies) {
					try {
						projects.Add(buildDependancy.Project);
					}
					catch(NotImplementedException) {
					}
				}
			}
			return projects;
		}
		protected Solution GetSolution() {
			return dte.Solution;			
		}
		protected void ShowErrorMessage(string message) {
			MessageBox.Show(message, CommandToolName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}
		protected void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
			ShowErrorMessage(((Exception)e.ExceptionObject).Message);
		}
		protected OutputWindowPane GetOutputWindowPane(string name) {
			EnvDTE.Window win = dte.Windows.Item(EnvDTE.Constants.vsWindowKindOutput);
			OutputWindow outputWindow = win.Object as OutputWindow;
			OutputWindowPane pane;
			try {
				pane = outputWindow.OutputWindowPanes.Item(name);
				pane.Activate();
			}
			catch {
				pane = outputWindow.OutputWindowPanes.Add(name);
			}
			return pane;
		}
		public VSCommand(IServiceProvider serviceProvider) {
			this.serviceProvider = serviceProvider;
			this.dte = (EnvDTE._DTE)serviceProvider.GetService(typeof(SDTE));
			CreateCommand();
		}
		public void CreateCommand() {
			IMenuCommandService mcs = serviceProvider.GetService(typeof(IMenuCommandService)) as IMenuCommandService;
			if(mcs != null) {
				oleMenuCommand = new OleMenuCommand(new EventHandler(delegate(object sender, EventArgs e) {
					InternalExec();
				}), CommandID);
				oleMenuCommand.BeforeQueryStatus += new EventHandler(oleMenuCommand_BeforeQueryStatus);
				mcs.AddCommand(oleMenuCommand);
			}
		}
		public abstract string CommandName {
			get;
		}
		public abstract string CommandToolName {
			get;
		}
		public abstract CommandID CommandID {
			get;
		}
		#region IVSCommand Members
		public abstract void InternalExec();
		protected abstract vsCommandStatus InternalQueryStatus(vsCommandStatus status);
		public vsCommandStatus QueryStatus() {
			return InternalQueryStatus(vsCommandStatus.vsCommandStatusInvisible);
		}
		#endregion
		public string GetModelEditorAssemblyName() {
			string assemblyName = null;
			foreach(BuildDependency buildDependancy in dte.Solution.SolutionBuild.BuildDependencies) {
				try {
					ProjectWrapper projectWrapper = ProjectWrapper.Create(buildDependancy.Project);
					assemblyName = projectWrapper.GetModelEditorAssemblyName();
					if(assemblyName != null) {
						return assemblyName;
					}
				}
				catch(NotImplementedException) {
				}
			}
			return null;
		}
		public Assembly LoadModelEditorAssembly() {
			try {
				string designCoreAssemblyName = GetModelEditorAssemblyName();
				return Assembly.Load(designCoreAssemblyName);
			}
			catch {
				return null;
			}
		}
		#region IDisposable Members
		public void Dispose() {
			ProjectWrapper.ClearCache();
			if(serviceProvider != null) {
				IMenuCommandService mcs = serviceProvider.GetService(typeof(IMenuCommandService)) as IMenuCommandService;
				if(mcs != null) {
					oleMenuCommand.BeforeQueryStatus -= new EventHandler(oleMenuCommand_BeforeQueryStatus);
					mcs.RemoveCommand(oleMenuCommand);
				}
				serviceProvider = null;
				oleMenuCommand = null;
			}
		}
		#endregion
	}
	public class PictureHelper : AxHost {
		public PictureHelper() : base("59EE46BA-677D-4d20-BF10-8D8067CB8B33") { }
		public static IPictureDisp GetIPictureDisp(Image image) {
			return (IPictureDisp)GetIPictureDispFromPicture(image);
		}
		public static Bitmap CreateMask(Bitmap src) {
			Bitmap maskBmp = new Bitmap(src.Width, src.Height, PixelFormat.Format24bppRgb);
			Color transparent = src.GetPixel(0, 0);
			for(int i = 0; i < src.Width; i++) {
				for(int j = 0; j < src.Height; j++) {
					maskBmp.SetPixel(i, j, (src.GetPixel(i, j) == transparent) ? Color.White : Color.Black);
				}
			}
			return maskBmp;
		}
	}
}
