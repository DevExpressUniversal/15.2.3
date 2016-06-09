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
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using DevExpress.Design.Mvvm.Wizards;
using DevExpress.Design.Mvvm.Wizards.UI;
using DevExpress.Utils.Design;
using DevExpress.Utils.Format;
using DevExpress.Xpf.Core.Design.Wizards.Mvvm.TextTemplating;
namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm {
	class CompoundMultiTask : SimpleWizardTask {
		List<Tuple<IWizardTask, string>> tasks = new List<Tuple<IWizardTask, string>>();
		public CompoundMultiTask(IServiceContainer serviceContainer) {
			ServiceContainer = serviceContainer;
			this.Action = () => DoTasks();			
		}
		IWizardTaskManager WizardTaskManager { get { return (IWizardTaskManager)ServiceContainer.GetService(typeof(IWizardTaskManager)); } }
		void DoTasks() {
			if(tasks.Count <= 0)
				return;
			tasks.Sort(delegate(Tuple<IWizardTask, string> x, Tuple<IWizardTask, string> y) {
				if(x == null && y == null)
					return 0;
				if(x == null)
					return -1;
				if(y == null)
					return 1;
				AddLicXFileTask addLicXFileTask = x.Item1 as AddLicXFileTask;
				if (addLicXFileTask != null)
					return 1;
				addLicXFileTask = y.Item1 as AddLicXFileTask;
				if (addLicXFileTask != null)
					return -1;
				AddAssemblyReferenceTask taskx = x.Item1 as AddAssemblyReferenceTask;
				AddAssemblyReferenceTask tasky = y.Item1 as AddAssemblyReferenceTask;
				if(taskx == null && tasky == null)
					return 0;
				if(taskx == null)
					return 1;
				if(tasky == null)
					return -1;
				return 0;
			});
			ProgressDialog progressDialog = new ProgressDialog();
			bool showed = false;
			bool isEnabledBak = WizardTaskManager.MainWindow != null && WizardTaskManager.MainWindow.IsEnabled;
			try {
				if (WizardTaskManager.MainWindow != null) {
					progressDialog.Owner = WizardTaskManager.MainWindow;
					WizardTaskManager.MainWindow.IsEnabled = false;
				}
				double count = tasks.Count;
				int counter = 0;
				try {
					RunningDocumentTableService.Subscribe(DTEHelper.GetCurrentDTE());
				}
				catch {
					RunningDocumentTableService.Unsubscribe();
				}
				foreach(Tuple<IWizardTask, string> item in tasks) {
					progressDialog.ViewModel.Description = item.Item2;
					if(!showed) {
						progressDialog.Show();
						showed = true;
					}
					try {
						item.Item1.Invoke();
					}
					catch(Exception ex) {
						Log.SendException(ex);
					}
					counter++;
					progressDialog.ViewModel.Progress = counter / count;
					DispatcherHelper.UpdateLayoutAndDoEvents(progressDialog);
				}
			}				
			catch(Exception ex) {
				Log.SendException(ex);
			}
			finally {
				RunningDocumentTableService.Unsubscribe();
				if(showed && progressDialog != null)
					progressDialog.Close();
				if (WizardTaskManager.MainWindow != null)
					WizardTaskManager.MainWindow.IsEnabled = isEnabledBak;
			}
		}
		public bool Contains(IWizardTask task) {
			return tasks.Any(tpl => tpl.Item1 == task);
		}
		public void Add(IWizardTask task, string description) {
			if(task != null && !this.Contains(task))
				tasks.Add(new Tuple<IWizardTask, string>(task, description));
		}
		public void Remove(IWizardTask task) {
			Tuple<IWizardTask, string> toRemove = tasks.FirstOrDefault(tpl => tpl.Item1 == task);
			if(toRemove != null)
				tasks.Remove(toRemove);
		}
		public IEnumerable<IWizardTask> Tasks {
			get {
				return tasks.Select<Tuple<IWizardTask, string>, IWizardTask>(tlp => tlp.Item1);
			}
		}
		public IServiceContainer ServiceContainer { get; private set; }
	}
}
