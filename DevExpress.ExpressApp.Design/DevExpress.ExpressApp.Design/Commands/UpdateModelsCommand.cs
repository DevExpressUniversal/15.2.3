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
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.ExpressApp.Design.Core;
using DevExpress.Persistent.Base;
using EnvDTE;
namespace DevExpress.ExpressApp.Design {
	public class UpdateModelsCommand : VSCommand {
		public UpdateModelsCommand(IServiceProvider serviceProvider) : base(serviceProvider) { }
		private const string updateModelsCommandName = "UpdateModelOnAllProjects";
		private const string updateModelsToolName = "Model Updater";
		private ModelUpdater modelUpdater;
		private bool BuildSolutionFolder(_DTE dte) {
			dte.ExecuteCommand("ClassViewContextMenus.ClassViewProject.Build", "");
			while(dte.Solution.SolutionBuild.BuildState != vsBuildState.vsBuildStateDone) {
				System.Threading.Thread.Sleep(100);
				Application.DoEvents();
			}
			bool isBuildSuccess = dte.Solution.SolutionBuild.LastBuildInfo == 0;
			if(!isBuildSuccess) {
				OutputWindowPane outputWindow = GetOutputWindowPane("Updating");
				outputWindow.OutputString("Solution folder build failed. Unable to run test.");
			}
			return isBuildSuccess;
		}
		private void UpdateModels(IList<IProjectWrapper> projects) {
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
				foreach(ProjectWrapper projectWrapper in projects) {
					if(projectWrapper.ContainsModelDiffs) {
						UpdateModelCommandHelper.ExecModelUpdater(modelUpdater, projectWrapper, outputWindow);
					}
				}
				AppDomain.CurrentDomain.UnhandledException -= new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
			}
			finally {
				modelUpdater = null;
				if(domain != null)
					AppDomain.Unload(domain);
			}
		}
		protected override EnvDTE.vsCommandStatus InternalQueryStatus(EnvDTE.vsCommandStatus status) {
			foreach(Project project in GetProcessedProjects()) {
				if(ProjectWrapper.IsExpressAppProjectFastCheck(project)) {
					return (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
				}
			}
			return status;
		}
		public override void InternalExec() {
			Project project = FindSelectedProject();
			if(project != null) {
				if(!BuildSolutionFolder(dte)) {
					ShowErrorMessage("Solution build failed. Unable to update models.");
				}
				else {
					IList<IProjectWrapper> xafProjects = new List<IProjectWrapper>();
					foreach(Project processedProject in GetProcessedProjects()) {
						if(ProjectWrapper.IsExpressAppProjectFastCheck(processedProject)) {
							xafProjects.Add(ProjectWrapper.Create(processedProject));
						}
					}
					UpdateModels(xafProjects);
				}
			}
			else {
				SolutionWrapper solution = new SolutionWrapper(GetSolution());
				if(!solution.Build()) {
					ShowErrorMessage("Solution build failed. Unable to update models.");
				}
				else {
					UpdateModels(solution.XafProjects);
				}
			}
		}
		public override string CommandName {
			get { return updateModelsCommandName; }
		}
		public override string CommandToolName {
			get { return updateModelsToolName; }
		}
		public override System.ComponentModel.Design.CommandID CommandID {
			get { return new CommandID(ConstantList.guidMenuAndCommandsCmdSet, CommandIds.cmdidUpdateModelsCommand); }
		}
	}
}
