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
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security;
using System.Text;
using DevExpress.Xpf.Charts;
using Microsoft.Windows.Design.Model;
using Platform::DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
namespace DevExpress.Charts.Designer.Native {
	public interface IHistoryItem {
		void Undo();
		void Redo();
		void Apply(ChartControl chartControl);
		void DesigntimeApply(IModelItem chartModelItem, IDesignTimeProvider designTimeProvider);
	}
	public class ElementIndexItem {
		readonly string name;
		readonly int index;
		public string ElementName {
			get { return name; }
		}
		public int Index {
			get { return index; }
		}
		public ElementIndexItem(string elemenName, int index) {
			this.name = elemenName;
			this.index = index;
		}
	}
	public class ExecuteCommandInfo {
		readonly object parameter;
		readonly object additionalInfo;
		readonly Dictionary<string, int> pathIndexes = new Dictionary<string,int>();
		public Dictionary<string, int> IndexByNameDictionary {
			get { return pathIndexes; }
		}
		public object Parameter {
			get { return parameter; }
		}
		public bool IsValidPath {
			get { return (pathIndexes.Count != 0); }
		}
		public object AdditionalInfo{
			get { return additionalInfo; }
		}
		public ExecuteCommandInfo(object parameter) : this(parameter, null) { }
		public ExecuteCommandInfo(object parameter, params ElementIndexItem[] pathIndexes) : this(parameter, null, pathIndexes) { }
		public ExecuteCommandInfo(object parameter, object additionalInfo, params ElementIndexItem[] pathIndexes) {
			this.parameter = parameter;
			this.additionalInfo = additionalInfo;
			if (pathIndexes != null)
				foreach (var pathIndex in pathIndexes) 
					this.pathIndexes.Add(pathIndex.ElementName, pathIndex.Index);
		}
	}
	public class HistoryItem : IHistoryItem {
		readonly ChartCommandBase command;
		readonly object oldValue;
		readonly object newValue;
		readonly object targetObject;
		readonly ExecuteCommandInfo executeCommandInfo;
		public ChartCommandBase Command { 
			get { return command; } 
		}
		public object OldValue { 
			get { return oldValue; } 
		}
		public object NewValue { 
			get { return newValue; } 
		}
		public object TargetObject { 
			get { return targetObject; } 
		}
		public ExecuteCommandInfo ExecuteCommandInfo { 
			get { return executeCommandInfo; } 
		}
		public HistoryItem(ExecuteCommandInfo executeCommandInfo, ChartCommandBase command, object oldValue, object newValue)
			: this(executeCommandInfo, command, null, oldValue, newValue) { }
		public HistoryItem(ExecuteCommandInfo executeCommandInfo, ChartCommandBase command, object targetObject, object oldValue, object newValue) {
			this.command = command;
			this.targetObject = targetObject;
			this.oldValue = oldValue;
			this.newValue = newValue;
			this.executeCommandInfo = executeCommandInfo;
		}
		#region IHistoryItem implementation
		void IHistoryItem.Undo() {
			Command.Undo(this);
		}
		void IHistoryItem.Redo() {
			Command.Redo(this);
		}
		void IHistoryItem.Apply(ChartControl chartControl) {
			Command.RuntimeApply(chartControl, this);
		}
		void IHistoryItem.DesigntimeApply(IModelItem chartModelItem, IDesignTimeProvider designTimeProvider) {
			Command.DesigntimeApply(chartModelItem, this, designTimeProvider);
		}
		#endregion
		public override string ToString() {
			return "HistoryItem (" + command.GetType().Name + ")";
		}
	}
	public class CompositeHistoryItem : IHistoryItem {
		List<IHistoryItem> historyItems = new List<IHistoryItem>();
		public List<IHistoryItem> HistoryItems { get { return historyItems; } }
		public IEnumerable<IHistoryItem> UndoHistoryItems {
			get {
				for (int i = historyItems.Count - 1; i >= 0; i--)
					yield return historyItems[i];
			}
		}
		#region IHistoryItem
		void IHistoryItem.Undo() {
			foreach (IHistoryItem item in UndoHistoryItems)
				item.Undo();
		}
		void IHistoryItem.Redo() {
			foreach (IHistoryItem item in HistoryItems)
				item.Redo();
		}
		void IHistoryItem.Apply(ChartControl chartControl) {
			foreach (IHistoryItem item in HistoryItems)
				item.Apply(chartControl);
		}
		void IHistoryItem.DesigntimeApply(IModelItem chartModelItem, IDesignTimeProvider designTimeProvider) {
			foreach (IHistoryItem item in HistoryItems)
				item.DesigntimeApply(chartModelItem, designTimeProvider);
		}
		#endregion
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
	public class CommandManager : NotifyPropertyChangedObject {
		readonly ObservableCollection<IHistoryItem> commands = new ObservableCollection<IHistoryItem>();
		int queuePosition = -1;
		public ObservableCollection<IHistoryItem> Commands {
			get { return commands; }
		}
		public int QueuePosition {
			get { return queuePosition; }
			private set {
				if (queuePosition != value) {
					queuePosition = value;
					OnPropertyChanged("QueuePosition");
				}
			}
		}
		void PushItem(IHistoryItem info) {
			if (commands.Count > queuePosition + 1) {
				int queueCount = commands.Count;
				for (int i = queueCount - 1; i > queuePosition; i--)
					commands.RemoveAt(i);
			}
			commands.Add(info);
			QueuePosition++;
		}
		IHistoryItem GetUndoItem() {
			IHistoryItem undoInfo = null;
			if (commands.Count > 0 && queuePosition > -1) {
				undoInfo = commands[queuePosition];
				QueuePosition--;
				if (queuePosition < -1)
					queuePosition = -1;
			}
			return undoInfo;
		}
		IHistoryItem GetRedoItem() {
			IHistoryItem redoInfo = null;
			if (queuePosition < commands.Count - 1) {
				QueuePosition++;
				redoInfo = commands[queuePosition];
			}
			return redoInfo;
		}
		public void AddHistoryItem(IHistoryItem item) {
			PushItem(item);		   
		}
		public void Undo() {
			IHistoryItem item = GetUndoItem();
			if (item != null)
				item.Undo();
		}
		public void Redo() {
			IHistoryItem item = GetRedoItem(); 
			if(item != null)
				item.Redo();
		}
		[SecuritySafeCritical]
		public void ApplyAllCommands(ChartControl chartControl) {
			for (int i = 0; i <= QueuePosition; i++)
				commands[i].Apply(chartControl);
		}
		[SecuritySafeCritical]
		public void ApplyAllCommands(ChartDesigner designer) {
			for (int i = 0; i <= QueuePosition; i++)
				commands[i].DesigntimeApply(designer.ChartModelItem, designer.DesignTimeProvider);
		}
	}
}
