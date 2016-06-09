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
using System.Linq;
using System.Collections.Generic;
using DevExpress.Utils.Design;
using DevExpress.Design.Mvvm.Wizards.UI;
using System.Threading;
using System.ComponentModel.Design;
using DevExpress.Xpf.Core.Design.Wizards.Mvvm.TextTemplating;
namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm {
	class RebuildTask :SimpleWizardTask {
		readonly IServiceContainer serviceContainer;
		private List<ProjectItem> winListToOpenAndReSave;
		public RebuildTask(IServiceContainer serviceContainer) {
			this.serviceContainer = serviceContainer;
			this.Action = () => { this.ShowConfirmationDialog(); };
		}
		public RebuildTask(IServiceContainer serviceContainer, List<ProjectItem> winListToOpenAndReSave) : this(serviceContainer) {
			this.winListToOpenAndReSave = winListToOpenAndReSave;
		}
		void ShowConfirmationDialog() {
			Project currentProject = DTEHelper.GetActiveProject();
			if(currentProject == null)
				return;
			IWizardTaskManager wizardTaskManager = (IWizardTaskManager)this.serviceContainer.GetService(typeof(IWizardTaskManager));
			if(DevExpress.Design.UI.MessageDialog.Show(wizardTaskManager.MainWindow, SR_Mvvm.RebuildQuestion, System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.Yes) {
				wizardTaskManager.Add(new SimpleWizardTask(() => {
					SynchronizationContext.Current.Post(state => {
						try {
							if(winListToOpenAndReSave != null && winListToOpenAndReSave.Count != 0) {
								ReSaveDesignerFilesForWin(currentProject, wizardTaskManager);
							} else BuildHelper.BuildProjects(currentProject.FullName);
						} catch {
						}
					}, null);
				}));
				wizardTaskManager.CloseMainWindow();
			}
		}
		void ReSaveDesignerFilesForWin(Project currentProject, IWizardTaskManager wizardTaskManager) {
			ProgressDialog progressDialog = new ProgressDialog();
			progressDialog.Topmost = true;
			progressDialog.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
			progressDialog.ViewModel.Description = string.Format("Start rebuild {0}", currentProject.Name);
			progressDialog.Show();
			BuildHelper.BuildProjects(currentProject.FullName);
			try {
				ReSaveDesignerFilesForWinCore(progressDialog);
			} finally {
				currentProject.DTE.Documents.SaveAll();
				progressDialog.ViewModel.Description = string.Format("End rebuild {0}", currentProject.Name);
				BuildHelper.BuildProjects(currentProject.FullName);
				progressDialog.Close();
			}
		}
		void ReSaveDesignerFilesForWinCore(TextTemplating.ProgressDialog progressDialog) {
			double count = winListToOpenAndReSave.Count;
			int counter = 0;
			foreach(ProjectItem projectItem in winListToOpenAndReSave) {
				progressDialog.ViewModel.Description = projectItem.Name;
				progressDialog.ViewModel.Progress = counter / count;
				Window window = projectItem.Open("{7651A702-06E5-11D1-8EBD-00A0C90F26EA}");
				window.Document.Activate();
				IDesignerHost host = window.Object as IDesignerHost;
				IComponentChangeService changeService = host.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
				changeService.OnComponentChanging(host.RootComponent, null);
				changeService.OnComponentChanged(host.RootComponent, null, null, null);
				window.Document.Save();
				window.Document.Close();
				counter++;
			}
		}
	}
}
