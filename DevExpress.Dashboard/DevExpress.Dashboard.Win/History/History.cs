#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
namespace DevExpress.DashboardWin.Native {
	public interface IHistoryItem {
		string Caption { get; }
		void Undo(DashboardDesigner designer);
		void Redo(DashboardDesigner designer);
	}
	public class History {
		const int forceModifiedIndex = -2;
		readonly List<IHistoryItem> items = new List<IHistoryItem>();
		readonly DashboardDesigner designer;
		int currentIndex = -1;
		int unmodifiedIndex = -1;
		public IEnumerable<IHistoryItem> UndoItems { 
			get { 
				for (int i = currentIndex; i >= 0; i--)
					yield return items[i];
			} 
		}
		public IEnumerable<IHistoryItem> RedoItems { 
			get { 
				int itemsCount = items.Count;
				for (int i = currentIndex + 1; i < itemsCount; i++)
					yield return items[i];
			} 
		}
		public bool IsModified { 
			get { return currentIndex != unmodifiedIndex; } 
			set {
				if (value != IsModified) 
					unmodifiedIndex = value ? forceModifiedIndex : currentIndex;
			}
		}
		public bool CanUndo { get { return currentIndex >= 0; } }
		public bool CanRedo { 
			get { 
				int count = items.Count;
				return count > 0 && currentIndex < count - 1; 
			} 
		}
		public event EventHandler Changed;
		public History(DashboardDesigner designer) {
			this.designer = designer;
		}
		public void Undo() {
			if (CanUndo)
				items[currentIndex--].Undo(designer);
		}
		public void Undo(IHistoryItem historyItem) {
			historyItem.Undo(designer);
		}
		public void Redo() {
			if (CanRedo)
				items[++currentIndex].Redo(designer);
		}
		public void Redo(IHistoryItem historyItem) {
			historyItem.Redo(designer);
		}
		public void RedoAndAdd(IHistoryItem item) {
			item.Redo(designer);
			Add(item);
		}
		public void Add(IHistoryItem item) {
			int index = currentIndex + 1;
			int count = items.Count;
			if (index < count)
				items.RemoveRange(index, count - index);
			if (unmodifiedIndex > currentIndex)
				unmodifiedIndex = forceModifiedIndex;
			items.Add(item);
			currentIndex++;
			RaiseChanged();
		}
		public void Clear() {
			items.Clear();
			currentIndex = -1;
			unmodifiedIndex = -1;
			RaiseChanged();
		}
		void RaiseChanged() {
			if (Changed != null)
				Changed(this, EventArgs.Empty);
		}
	}
}
