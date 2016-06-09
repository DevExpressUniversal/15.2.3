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
using System.Collections.Generic;
using DevExpress.Design.UI;
using System.Windows;
using DevExpress.Entity.ProjectModel;
namespace DevExpress.Design.Mvvm.Wizards.UI {
	public interface ITask { 
	}
	public interface IWizardTask : ITask {
		IAsyncResult BeginInvoke(AsyncCallback asyncCallback, object @object);
		void EndInvoke(IAsyncResult result);
		void Invoke();
	}
	public interface IWizardTask<TContext> : IWizardTask {
		TContext Context { get; }
	}
	public interface IWizardUITask : ITask {
		Window GetDialog();
		void DialogClosed(bool? dialogResult);
	}	
	public interface IWizardUITask<TContext> : IWizardUITask {
		TContext Context { get; }
	}
	public interface ITaskManager {
		void Start();
		void Add(ITask task);
		void Remove(ITask task);
		void RemoveLastTasks();
		ITask ActiveTask { get; }
		IEnumerable<ITask> Tasks { get; }
		bool IsStarted { get; }
	}
	public interface IWizardTaskManager : ITaskManager {
		Window MainWindow { get; }
		void CloseMainWindow();
	}	
	interface IStepByStepConfiguratorPagesProvider<TContext> {
		TContext Context { get; }
		IEnumerable<IStepByStepConfiguratorPageViewModel<TContext>> GetPages(IViewModelBase parentViewModel);
	}
	interface IAppLayerConstructor : IStepByStepConfiguratorPagesProvider<MvvmConstructorContext> {
		IEnumerable<ConstructorEntry> GetAvailableEntries();
		bool IsVisibleWithoutItems(ConstructorEntry entry);
		IEnumerable<IMvvmConstructorPageViewModel> GetPages(IViewModelBase parentViewModel, ConstructorEntry entry);
		IEnumerable<IHasName> GetEntryItems(ConstructorEntry entry, bool showLocalOnly);
		bool IsUnsupported(IHasName entryItem);
		IItemCreator GetEntryItemCreator(ConstructorEntry entry);
		bool CanCreateNewItemsOf(ConstructorEntry entry);
		void EntryItemSelected(ConstructorEntry entry, IHasName selectedItem);
	}
	interface IItemCreator { 
		void Create();
		string Message { get; }
		bool IsCloseDialogNeeded { get; }
	}
}
