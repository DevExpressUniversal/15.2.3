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

extern alias Platform;
using DevExpress.Charts.Native;
using DevExpress.Xpf.Charts;
using Platform::DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using System;
using System.Security;
using System.Windows.Input;
namespace DevExpress.Charts.Designer.Native {
	public abstract class ChartCommandBase : ICommand {
		readonly WeakReference chartModel;
		protected WpfChartModel ChartModel {
			get { return (WpfChartModel)chartModel.Target; }
		}
		protected ChartControl PreviewChart {
			get { return ChartModel.Chart; }
		}
		public abstract string Caption {
			get;
		}
		public virtual string Description {
			get { return null; }
		}
		public abstract string ImageName {
			get;
		}
		public ChartCommandBase(WpfChartModel model) {
			this.chartModel = new WeakReference(model);
		}
		event EventHandler canExecuteChanged;
		event EventHandler ICommand.CanExecuteChanged {
			add { canExecuteChanged += value; }
			remove { canExecuteChanged -= value; }
		}
		public event EventHandler CommandExecuted;
		#region ICommand implementation
		bool ICommand.CanExecute(object parameter) {
			return CanExecute(parameter);
		}
		void ICommand.Execute(object parameter) {
			if (CanExecute(parameter)) {
				CommandResult result = RuntimeExecute(parameter);
				if (result != null) {
					ChartModel.CommandManager.AddHistoryItem(result.HistoryItem);
					UpdateChartModelAfterCommandExecution(result);
					RiseCommandExecutedEvent();
					ChartModel.SelectedPageName = result.SelectedPageName;
				}
			}
			else
				ChartDebug.Fail("The " + GetType().Name + " can not be executed because the CanExecute method has returned a false value.");
		}
		#endregion
		void UpdateChartModelAfterCommandExecution(CommandResult result) {
			if (result != null) {
				ChartModel.RecursivelyUpdateChildren();
				ChartModel.UpdateCommandsCanExecute();
				if (result.SelectedChartObject != null)
					ChartModel.SelectedObject = result.SelectedChartObject;
				ChartModel.UpdateModelProperties();
			}
		}
		void RiseCommandExecutedEvent() {
			if (CommandExecuted != null)
				CommandExecuted(this, new EventArgs());
		}
		protected abstract bool CanExecute(object parameter);
		protected abstract Object RuntimeRedo(HistoryItem historyItem);
		protected abstract Object RuntimeUndo(HistoryItem historyItem);
		[SecuritySafeCritical]
		public abstract void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider);
		public void RaiseCanExecuteChanged() {
			if (canExecuteChanged != null)
				canExecuteChanged(this, new EventArgs());
		}
		public void Redo(HistoryItem historyItem) {
			object selectedItem = RuntimeRedo(historyItem);
			ChartModel.RecursivelyUpdateChildren();
			ChartModel.UpdateCommandsCanExecute();
			if (selectedItem != null) {
				ChartModel.SelectedObject = selectedItem;
				ChartModel.UpdateModelProperties();
			}
		}
		public void RegisterForUpdate() {
			ChartModel.RegisterCommand(this);
		}
		public abstract void RuntimeApply(ChartControl chartControl, HistoryItem historyItem);
		public abstract CommandResult RuntimeExecute(object parameter);
		public void Undo(HistoryItem historyItem) {
			object selectedItem = RuntimeUndo(historyItem);
			ChartModel.RecursivelyUpdateChildren();
			ChartModel.UpdateCommandsCanExecute();
			if (selectedItem != null) {
				ChartModel.SelectedObject = selectedItem;
				ChartModel.UpdateModelProperties();
			}
		}
		public CommandResult RuntumeExecuteAndUpdateChartModel(object parameter) {
			CommandResult result = RuntimeExecute(parameter);
			UpdateChartModelAfterCommandExecution(result);
			return result;
		}
	}
}
