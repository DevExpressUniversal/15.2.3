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
using System.Linq;
using System.Windows;
using System.Collections.Generic;
namespace DevExpress.Design.Mvvm.Wizards.UI {
	public class TaskManager : ITaskManager {
		bool started;
		ITask activeTask;
		List<ITask> tasks = new List<ITask>();
		public virtual void Start() {
			if(started || tasks.Count == 0)
				return;
			RunTasks();
		}
		protected void RunTasks() {
			ITask task = GetNext();
			while(task != null) {
				DoTask(task);
				task = GetNext();
			}
		}
		protected bool AllTasksCompleted {
			get {
				if (tasks.Count == 0)
					return true;
				if (activeTask == null)
					return false;					
				int index = tasks.IndexOf(activeTask);
				return index >= 0 && index == tasks.Count - 1;
			}
		}
		ITask GetNext() {
			if(tasks.Count == 0)
				return null;
			if(activeTask == null)
				return tasks[0];
			int index = tasks.IndexOf(activeTask);
			if(index < 0)
				index = 0;
			else index++;
			if(index < tasks.Count)
				return tasks[index];
			return null;
		}
		protected virtual void DoTask(ITask task) {
			if(task == null)
				return;
			started = true;
			activeTask = task;
			IWizardTask wizardTask = activeTask as IWizardTask;
			if(wizardTask != null) {
				wizardTask.Invoke();
				return;
			}
			IWizardUITask uiTask = activeTask as IWizardUITask;
			if(uiTask != null) {
				bool? result = ShowDialog(uiTask.GetDialog());
				uiTask.DialogClosed(result);
				return;
			}
		}
		protected virtual bool? ShowDialog(Window dialog){
			if(dialog != null)
				return dialog.ShowDialog();
			return null;
		}
		public void Add(ITask task) {
			if (task != null && !tasks.Contains(task))
				tasks.Add(task);
		}
		public ITask ActiveTask {
			get { return activeTask; }
		}
		public IEnumerable<ITask> Tasks {
			get { return tasks; }
		}
		public virtual void Remove(ITask task) {
			if(task == null || ActiveTask == task || tasks == null)
				return;
			if(tasks.Contains(task))
				tasks.Remove(task);
		}
		public virtual void RemoveLastTasks() {
			if(ActiveTask == null || tasks == null)
				return;
			List<ITask> toRemove = new List<ITask>();
			int index = tasks.IndexOf(ActiveTask);
			if(index < 0)
				return;
			index++;
			int count = tasks.Count - index;
			if(index >= tasks.Count || count <= 0)
				return;
			tasks.RemoveRange(index, count);
		}
		public bool IsStarted { get { return this.started; } }
	}
}
