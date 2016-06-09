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
namespace DevExpress.Xpf.Grid.EditForm {
	public class EditFormLayoutCalculator {
		int columnCountCore = 1;
		public int ColumnCount {
			get { return columnCountCore; }
			set {
				if(columnCountCore != value)
					columnCountCore = value;
			}
		}
		public virtual EditFormLayoutSettings LayoutSettings { get; protected set; }
		public virtual void SetPositions(IEnumerable<IEditFormLayoutItem> items) {
			var layoutMatrix = new LayoutMatrix(ColumnCount);
			Row currentRow = layoutMatrix.AddNewRow();
			foreach(IEditFormLayoutItem item in items) {
				if(!ValidateItem(item))
					return;
				while(!currentRow.TryAddItem(item))
					currentRow = layoutMatrix.GetNextRow(currentRow.Index);
				for(int j = 0; j < item.RowSpan - 1; j++) {
					Row row = layoutMatrix.GetNextRow(currentRow.Index + j);
					row.FillSlotRange(item);
				}
			}
			UpdateLayoutSettings(layoutMatrix);
		}
		void UpdateLayoutSettings(LayoutMatrix layoutMatrix) {
			int rowCount = layoutMatrix.Count;
			if(ColumnCount != LayoutSettings.ColumnCount || rowCount != LayoutSettings.RowCount)
				LayoutSettings = new EditFormLayoutSettings(ColumnCount, rowCount);
		}
		bool ValidateItem(IEditFormLayoutItem item) {
			return item.ColumnSpan > 0 && item.RowSpan > 0 && item.ColumnSpan <= ColumnCount;
		}
	}
	public interface IEditFormLayoutItem {
		int Column { get; set; }
		int Row { get; set; }
		int ColumnSpan { get; set; }
		int RowSpan { get; set; }
		bool StartNewRow { get; }
		EditFormLayoutItemType ItemType { get; }
	}
	public enum EditFormLayoutItemType {
		None,
		Caption,
		Editor
	}
	internal class LayoutMatrix : List<Row> {
		readonly int columnCount;
		public LayoutMatrix(int columnCount) {
			this.columnCount = columnCount;
		}
		public Row GetNextRow(int currentIndex) {
			Row row = null;
			if(currentIndex == Count - 1)
				row = AddNewRow();
			else
				row = this[currentIndex + 1];
			return row;
		}
		public Row AddNewRow() {
			var row = new Row(columnCount, Count);
			Add(row);
			return row;
		}
	}
	internal class Row {
		int columnCount;
		int rowIndex;
		const int IncorrectSlotIndex = -1;
		bool[] slots;
		public Row(int columnCount, int index) {
			this.columnCount = columnCount;
			this.rowIndex = index;
			this.slots = new bool[columnCount];
		}
		public bool TryAddItem(IEditFormLayoutItem item) {
			int slotIndex = FindIndexOfFreeSlotRange(item.ColumnSpan);
			if(slotIndex == IncorrectSlotIndex || item.StartNewRow && slotIndex != 0) {
				FillSlotRange(0, columnCount);
				return false;
			}
			item.Column = slotIndex;
			item.Row = rowIndex;
			FillSlotRange(0, item.Column + item.ColumnSpan);
			return true;
		}
		int FindIndexOfFreeSlotRange(int rangeSize) {
			int firstFreeSlotIndex = IncorrectSlotIndex;
			int buffer = 0;
			for(int i = 0; i < slots.Length; i++) {
				if(!slots[i]) {
					if(buffer == 0)
						firstFreeSlotIndex = i;
					buffer++;
				} else {
					buffer = 0;
				}
				if(buffer == rangeSize)
					return firstFreeSlotIndex;
			}
			return IncorrectSlotIndex;
		}
		public void FillSlotRange(IEditFormLayoutItem item) {
			FillSlotRange(item.Column, item.ColumnSpan);
		}
		public void FillSlotRange(int startSlotIndex, int rangeSize) {
			for(int i = startSlotIndex; i < startSlotIndex + rangeSize; i++)
				slots[i] = true;
		}
		public int Index { get { return rowIndex; } }
	}
	internal class CaptionedLayoutItem : IEditFormLayoutItem {
		readonly IEditFormLayoutItem captionCore;
		readonly IEditFormLayoutItem editorCore;
		IEditFormLayoutItem Caption { get { return captionCore; } }
		IEditFormLayoutItem Editor { get { return editorCore; } }
		public CaptionedLayoutItem(IEditFormLayoutItem caption, IEditFormLayoutItem editor) {
			captionCore = caption;
			editorCore = editor;
			ColumnSpan = editorCore.ColumnSpan;
			RowSpan = editorCore.RowSpan;
			startNewRowCore = editorCore.StartNewRow;
			Caption.ColumnSpan = 1;
			Caption.RowSpan = 1;
			Editor.ColumnSpan = Editor.ColumnSpan * 2 - 1;
			SyncColumn();
			SyncRow();
		}
		int columnCore;
		public int Column {
			get { return columnCore; }
			set {
				if(columnCore != value) {
					columnCore = value;
					SyncColumn();
				}
			}
		}
		void SyncColumn() {
			Caption.Column = Column * 2;
			Editor.Column = Caption.Column + 1;
		}
		int rowCore;
		public int Row {
			get { return rowCore; }
			set {
				if(rowCore != value) {
					rowCore = value;
					SyncRow();
				}
			}
		}
		void SyncRow() {
			Caption.Row = Row;
			Editor.Row = Row;
		}
		public int ColumnSpan { get; set; }
		public int RowSpan { get; set; }
		bool startNewRowCore;
		public bool StartNewRow { get { return startNewRowCore; } }
		public EditFormLayoutItemType ItemType { get { return EditFormLayoutItemType.None; } }
	}
}
