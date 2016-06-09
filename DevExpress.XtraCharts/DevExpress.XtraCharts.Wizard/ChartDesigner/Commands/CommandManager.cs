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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
namespace DevExpress.XtraCharts.Designer.Native {
	public abstract class ChartCommandBase {
		readonly CommandManager commandManager;
		protected internal virtual bool CanDisposeOldValue { get { return false; } }
		protected internal virtual bool CanDisposeNewValue { get { return false; } }
		protected internal virtual bool CanUpdatePointsGrid { get { return false; } }
		internal CommandManager CommandManager { get { return commandManager; } }
		public ChartCommandBase(CommandManager commandManager) {
			this.commandManager = commandManager;
		}
		protected virtual object CreatePropertiesCache(ChartElement chartElement) {
			return null;
		}
		protected virtual void RestoreProperties(ChartElement chartElement, object properties) {
		}
		public abstract bool CanExecute(object parameter);
		public abstract HistoryItem ExecuteInternal(object parameter);
		public abstract void UndoInternal(HistoryItem historyItem);
		public abstract void RedoInternal(HistoryItem historyItem);
		public void Execute(object parameter) {
			if (CanExecute(parameter)) {
				HistoryItem historyItem = ExecuteInternal(parameter);
				commandManager.AddHistoryItem(historyItem);
			}
		}
		public void Undo(HistoryItem historyItem) {
			UndoInternal(historyItem);
		}
		public void Redo(HistoryItem historyItem) {
			RedoInternal(historyItem);
		}
	}
	public abstract class DesignerDisposableElement : IDisposable {
		bool isDisposed = false;
		~DesignerDisposableElement() {
			Dispose(false);
		}
		protected virtual void Dispose(bool disposing) {
		}
		public void Dispose() {
			if (!isDisposed) {
				Dispose(true);
				isDisposed = true;
				GC.SuppressFinalize(this);
			}
		}
	}
	public class HistoryDisposingStrategy {
		static readonly HistoryDisposingStrategy executedItemsStrategy = new HistoryDisposingStrategy(true, false);
		static readonly HistoryDisposingStrategy undoedItemsStrategy = new HistoryDisposingStrategy(false, true);
		public static HistoryDisposingStrategy ExecutedItemsStrategy { get { return executedItemsStrategy; } }
		public static HistoryDisposingStrategy UndoedItemsStrategy { get { return undoedItemsStrategy; } }
		readonly bool canDisposeOldValue;
		readonly bool canDisposeNewValue;
		public bool CanDisposeOldValue { get { return canDisposeOldValue; } }
		public bool CanDisposeNewValue { get { return canDisposeNewValue; } }
		HistoryDisposingStrategy(bool canDisposeOldValue, bool canDisposeNewValue) {
			this.canDisposeOldValue = canDisposeOldValue;
			this.canDisposeNewValue = canDisposeNewValue;
		}
	}
	public interface IHistoryItem : IDisposable {
		object ObjectToSelect { get; }
		void Undo();
		void Redo();
	}
	public class HistoryItem : DesignerDisposableElement, IHistoryItem {
		readonly ChartCommandBase command;
		readonly object oldValue;
		readonly object newValue;
		readonly object parameter;
		public ChartCommandBase Command {
			get { return command; }
		}
		public object OldValue {
			get { return oldValue; }
		}
		public object NewValue {
			get { return newValue; }
		}
		public object Parameter { get { return parameter; } }
		public object TargetObject { get; set; }
		public object ObjectToSelect { get; set; }
		public HistoryItem(ChartCommandBase command, object oldValue, object newValue, object parameter) {
			this.command = command;
			this.oldValue = oldValue;
			this.newValue = newValue;
			this.parameter = parameter;
		}
		#region IHistoryItem implementation
		object IHistoryItem.ObjectToSelect { get { return ObjectToSelect; } }
		void IHistoryItem.Undo() {
			Command.Undo(this);
		}
		void IHistoryItem.Redo() {
			Command.Redo(this);
		}
		#endregion
		void DisposeValueCore(object value) {
			if (value is IDisposable)
				((IDisposable)value).Dispose();
		}
		void DisposeValue(object value, bool canDispose) {
			if (!canDispose || value is string)
				return;
			DisposeValueCore(value);
			IEnumerable valueEnumerable = value as IEnumerable;
			if (valueEnumerable != null)
				foreach (object valueItem in valueEnumerable) {
					DisposeValueCore(valueItem);
				}
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				DisposeValue(oldValue, command.CommandManager.CanDisposeOldValue(this));
				DisposeValue(newValue, command.CommandManager.CanDisposeNewValue(this));
			}
			base.Dispose(disposing);
		}
		public override string ToString() {
			return "HistoryItem (" + command.GetType().Name + ")";
		}
	}
	public class CompositeHistoryItem : DesignerDisposableElement, IHistoryItem {
		List<IHistoryItem> historyItems = new List<IHistoryItem>();
		internal List<IHistoryItem> HistoryItems { get { return historyItems; } }
		IEnumerable<IHistoryItem> UndoHistoryItems {
			get {
				for (int i = historyItems.Count - 1; i >= 0; i--)
					yield return historyItems[i];
			}
		}
		public object ObjectToSelect { get; set; }
		#region IHistoryItem
		object IHistoryItem.ObjectToSelect { get { return ObjectToSelect; } }
		void IHistoryItem.Undo() {
			foreach (IHistoryItem item in UndoHistoryItems)
				item.Undo();
		}
		void IHistoryItem.Redo() {
			foreach (IHistoryItem item in HistoryItems)
				item.Redo();
		}
		#endregion
		protected override void Dispose(bool disposing) {
			if (disposing) {
				foreach (IHistoryItem item in HistoryItems)
					item.Dispose();
			}
			base.Dispose(disposing);
		}
		public override string ToString() {
			StringBuilder builder = new StringBuilder("CompositeHistoryItem (");
			bool started = false;
			foreach (var item in historyItems) {
				if (started)
					builder.Append(",");
				builder.Append(item.ToString());
				started = true;
			}
			builder.Append(")");
			return builder.ToString();
		}
	}
	public class CommandManager : DesignerDisposableElement, IDisposable {
		const int defaultQueuePosition = -1;
		readonly SetObjectPropertyCommand setObjectPropertyCommand;
		readonly List<IHistoryItem> commands = new List<IHistoryItem>();
		readonly HashSet<IHistoryItem> lockedCommands = new HashSet<IHistoryItem>();
		int queuePosition = defaultQueuePosition;
		CompositeHistoryItem transaction = null;
		HistoryDisposingStrategy disposingStrategy;
		public bool CanUndo { get { return commands.Count > 0 && queuePosition > defaultQueuePosition; } }
		public bool CanRedo { get { return commands.Count > 0 && queuePosition < (commands.Count - 1); } }
		public SetObjectPropertyCommand SetPropertyCommand { get { return setObjectPropertyCommand; } }
		public event CommandExecutedEventHandler CommandExecuted;
		public CommandManager() {
			setObjectPropertyCommand = new SetObjectPropertyCommand(this);
			disposingStrategy = HistoryDisposingStrategy.UndoedItemsStrategy;
		}
		void OnCommandExecuted(IHistoryItem item) {
			if (CommandExecuted == null)
				return;
			HistoryItem historyItem = item as HistoryItem;
			if (historyItem != null)
				CommandExecuted(new CommandExecutedEventArgs(item.ObjectToSelect, historyItem.Parameter, historyItem));
			else {
				object objectToSelect = item == null ? null : item.ObjectToSelect;
				CommandExecuted(new CommandExecutedEventArgs(objectToSelect));
			}
		}
		void PushItem(IHistoryItem info) {
			if (commands.Count > queuePosition + 1) {
				int queueCount = commands.Count;
				for (int i = queueCount - 1; i > queuePosition; i--) {
					IHistoryItem item = commands[i];
					commands.RemoveAt(i);
					item.Dispose();
				}
			}
			commands.Add(info);
			lockedCommands.Clear();
			queuePosition++;
		}
		void PushToTransaction(IHistoryItem info) {
			transaction.HistoryItems.Add(info);
		}
		IHistoryItem GetUndoItem() {
			IHistoryItem undoInfo = null;
			if (CanUndo) {
				undoInfo = commands[queuePosition];
				queuePosition--;
				if (queuePosition < defaultQueuePosition)
					queuePosition = defaultQueuePosition;
			}
			return undoInfo;
		}
		IHistoryItem GetRedoItem() {
			IHistoryItem redoInfo = null;
			if (queuePosition < commands.Count - 1) {
				queuePosition++;
				redoInfo = commands[queuePosition];
			}
			return redoInfo;
		}
		bool CanDisposeValue(HistoryItem historyItem, object value) {
			return !lockedCommands.Contains(historyItem) && (value is ChartElement || value is Image || value is Font);
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				disposingStrategy = HistoryDisposingStrategy.ExecutedItemsStrategy;
				for (int i = 0; i <= queuePosition; i++) {
					commands[i].Dispose();
				}
				disposingStrategy = HistoryDisposingStrategy.UndoedItemsStrategy;
				for (int i = queuePosition + 1; i < commands.Count; i++) {
					commands[i].Dispose();
				}
			}
			base.Dispose(disposing);
		}
		public bool CanDisposeOldValue(HistoryItem historyItem) {
			return disposingStrategy.CanDisposeOldValue && historyItem.Command.CanDisposeOldValue && CanDisposeValue(historyItem, historyItem.OldValue);
		}
		public bool CanDisposeNewValue(HistoryItem historyItem) {
			return disposingStrategy.CanDisposeNewValue && historyItem.Command.CanDisposeNewValue && CanDisposeValue(historyItem, historyItem.NewValue);
		}
		public void AddHistoryItem(HistoryItem item) {
			if (item != null) {
				lockedCommands.Add(item);
				if (transaction != null)
					PushToTransaction(item);
				else
					PushItem(item);
			}
			OnCommandExecuted(item);
		}
		public void Undo() {
			IHistoryItem item = GetUndoItem();
			if (item != null) {
				item.Undo();
				OnCommandExecuted(item);
			}
		}
		public void Redo() {
			IHistoryItem item = GetRedoItem();
			if (item != null) {
				item.Redo();
				OnCommandExecuted(item);
			}
		}
		public void BeginTransaction() {
			transaction = new CompositeHistoryItem();
		}
		public void CommitTransaction() {
			if (transaction.HistoryItems.Count > 0)
				PushItem(transaction);
			transaction = null;
		}
		public void CancelTransaction() {
			transaction = null;
			lockedCommands.Clear();
			Undo();
		}
	}
	public class CommandExecutedEventArgs : EventArgs {
		readonly object objectToSelect;
		readonly object commandParameter;
		readonly HistoryItem historyItem;
		public object ObjectToSelect { get { return objectToSelect; } }
		public object CommandParameter { get { return commandParameter; } }
		public HistoryItem HistoryItem { get { return historyItem; } }
		public CommandExecutedEventArgs(object objectToSelect) {
			this.objectToSelect = objectToSelect;
		}
		public CommandExecutedEventArgs(object objectToSelect, object commandParameter) {
			this.objectToSelect = objectToSelect;
			this.commandParameter = commandParameter;
		}
		public CommandExecutedEventArgs(object objectToSelect, object commandParameter, HistoryItem historyItem) {
			this.objectToSelect = objectToSelect;
			this.commandParameter = commandParameter;
			this.historyItem = historyItem;
		}
	}
	public delegate void CommandExecutedEventHandler(CommandExecutedEventArgs e);
}
