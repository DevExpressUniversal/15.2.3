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
using System.ComponentModel.Design;
using System.IO;
using System.Reflection;
using DevExpress.ExpressApp.Design.Core;
using DevExpress.Persistent.Base;
using EnvDTE;
namespace DevExpress.ExpressApp.Design {
	public class UpdateModelCommandHelper {		
		static public void ExecModelUpdater(ModelUpdater modelUpdater, ProjectWrapper projectWrapper, OutputWindowPane outputWindow) {
			try {
				outputWindow.OutputString(string.Format("------- Updating model differences: Project: {0}", projectWrapper.FullName) + Environment.NewLine);
				string assemblyName = null;
				if(projectWrapper.Project != null) {
					try {
						assemblyName = projectWrapper.Project.Properties.Item("AssemblyName").Value as string;
					}
					catch { }
				}
				modelUpdater.UpdateModel(projectWrapper.TargetPath,
					projectWrapper.BinariesDir,
					projectWrapper.ProjectPath,
					projectWrapper.DiffsNameTemplate, projectWrapper.DeviceSpecificDiffsNameTemplates, assemblyName, projectWrapper.FullName);
				outputWindow.OutputString("Update complete" + Environment.NewLine + Environment.NewLine);
				if(modelUpdater.HasUnusableNodes) {
					outputWindow.OutputString("Warning:" + Environment.NewLine);
					outputWindow.OutputString("\tThe updated model differences have unusable nodes. See:" + Environment.NewLine);
					string[] unusableDiffsFileNames = Directory.GetFiles(projectWrapper.ProjectPath, modelUpdater.UnusableNodesFileNameTemplate + "*.xml");
					foreach(string fileName in unusableDiffsFileNames) {
						outputWindow.OutputTaskItemString("\t\t" + fileName, vsTaskPriority.vsTaskPriorityHigh, "User", vsTaskIcon.vsTaskIconUser, fileName, 1, "", true);
						outputWindow.OutputString(Environment.NewLine);
					}
				}
			}
			catch(Exception e) {
				Tracing.Tracer.LogError(e);
				outputWindow.OutputString("Error:" + Environment.NewLine);
				outputWindow.OutputString(e.Message + Environment.NewLine);
				outputWindow.OutputString("Update failed" + Environment.NewLine + Environment.NewLine);
			}
		}
	}
	public class UpdateModelCommand : VSCommand {
		public UpdateModelCommand(IServiceProvider serviceProvider) : base(serviceProvider) { }
		private string updateModelCommandName = "UpdateModel";
		private string updateModelToolName = "Model Updater";
		private ModelUpdater modelUpdater;		
		private void UpdateModel(ProjectWrapper projectWrapper) {
			OutputWindowPane outputWindow = GetOutputWindowPane("Updating");
			outputWindow.Clear();
			AppDomain domain = null;
			try {
				string dllPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
				AppDomainSetup setup = new AppDomainSetup();
				setup.ApplicationBase = dllPath;
				ReflectionHelper.AddResolvePath(Path.GetDirectoryName(GetType().Assembly.Location));
				AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
				domain = AppDomain.CreateDomain("Updater" + Guid.NewGuid().ToString(), null, setup);
				modelUpdater = (ModelUpdater)domain.CreateInstanceAndUnwrap
					(GetType().Assembly.GetName().Name, typeof(ModelUpdater).FullName);
				modelUpdater.CrossDomainBridge = new CrossDomainBridge(serviceProvider);
				UpdateModelCommandHelper.ExecModelUpdater(modelUpdater, projectWrapper, outputWindow);
				AppDomain.CurrentDomain.UnhandledException -= new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
			}
			finally {
				modelUpdater = null;
				if(domain != null)
					AppDomain.Unload(domain);
			}
		}
		protected override EnvDTE.vsCommandStatus InternalQueryStatus(EnvDTE.vsCommandStatus status) {
			Project pr = FindSelectedProject();
			if(pr != null && ProjectWrapper.IsExpressAppProjectFastCheck(pr) && ProjectWrapper.Create(pr).ContainsModelDiffs) {
				return (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
			}
			else {
				return status;
			}
		}
		public override string CommandName {
			get { return updateModelCommandName; }
		}
		public override string CommandToolName {
			get { return updateModelToolName; }
		}
		public override void InternalExec() {
			ProjectWrapper projectWrapper = ProjectWrapper.Create(FindSelectedProject());
			if(!projectWrapper.Build()) {
				ShowErrorMessage("Project build failed. Unable to edit model.");
			}
			else {
				UpdateModel(projectWrapper);				
			}
		}
		#region IOleCommandTarget Members
		#endregion
		public override System.ComponentModel.Design.CommandID CommandID {
			get { return new CommandID(ConstantList.guidMenuAndCommandsCmdSet, CommandIds.cmdidUpdateModelCommand); }
		}
	}
}
