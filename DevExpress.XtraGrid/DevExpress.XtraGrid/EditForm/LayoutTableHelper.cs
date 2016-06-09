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
using System.Linq;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
namespace DevExpress.XtraGrid.EditForm.Layout {
	public enum GridViewEditFormLayoutItemType { Caption, Editor, Empty }
	public class GridViewEditFormLayoutItem {
		public EditFormColumn Column { get; set; }
		public GridViewEditFormLayoutItemType Type { get; set; }
		public int ColIndex { get; set; }
		public int ColSpan { get; set; }
		public int RowSpan { get; set; }
		public EditFormColumnCaptionLocation CaptionLocation { get; set; }
		public double WidthPercent { get; set; }
		public override string ToString() {
			return string.Format("{0}: {1} {2},{3}", Type, Column == null ? "null" : Column.FieldName, ColSpan, RowSpan);
		}
	}
	public class GridViewEditFormLayout {
		protected GridViewEditFormLayout(EditFormOwner grid) {
			Owner = grid;
			Columns = new List<EditFormColumn>();
			Rectangles = new Dictionary<Rectangle, EditFormColumn>();
			Layout = new List<List<GridViewEditFormLayoutItem>>();
			MaxCellCountInLayoutColumn = new List<int>(Enumerable.Range(0, ColCount).Select(i => 1));
			Build();
		}
		public static List<List<GridViewEditFormLayoutItem>> CreateLayout(EditFormOwner grid) {
			return new GridViewEditFormLayout(grid).Layout;
		}
		protected List<EditFormColumn> Columns { get; private set; }
		protected EditFormOwner Owner { get; private set; }
		protected Dictionary<Rectangle, EditFormColumn> Rectangles { get; private set; }
		protected List<List<GridViewEditFormLayoutItem>> Layout { get; private set; }
		protected List<int> MaxCellCountInLayoutColumn { get; private set; }
		protected int ColCount { get { return Owner.EditFormColumnCount; } }
		protected virtual void Build() {
			BuildColumns();
			BuildRectangles();
			if(Columns.Count == 0) return;
			BuildLayout();
		}
		protected virtual void BuildColumns() {
			Columns.AddRange(
				Owner.Columns.Where(c => IsEditable(c)).OrderBy(c => c, new EditColumnComparer())
			);
		}
		protected virtual void BuildRectangles() {
			var x = 0;
			var y = 0;
			foreach(var column in Columns) {
				var rect = GetNextEmptyRect(ref x, ref y, column);
				Rectangles[rect] = column;
				if(rect.Width == 1 && GetCaptionLocation(column) == EditFormColumnCaptionLocation.Near)
					MaxCellCountInLayoutColumn[x] = 2;
			}
		}
		protected virtual Rectangle GetNextEmptyRect(ref int x, ref int y, EditFormColumn column) {
			int width = GetColSpan(column), height = GetRowSpan(column);
			Rectangle rect = Rectangle.Empty;
			bool startNewRow = column.StartNewRow;
			while(true) {
				if(x >= ColCount || startNewRow) {
					startNewRow = false;
					x = 0;
					y++;
					continue;
				}
				rect = new Rectangle(x, y, width, height);
				if(!Rectangles.Keys.Any(r => r.IntersectsWith(rect)) && rect.Right <= ColCount)
					break;
				x++;
			}
			return rect;
		}
		public void BuildLayout() {
			var right = ColCount;
			var bottom = Rectangles.Keys.Max(r => r.Bottom);
			for(var j = 0; j < bottom; j++) {
				var row = new List<GridViewEditFormLayoutItem>();
				Layout.Add(row);
				var emptyItemSpan = 0;
				for(var i = 0; i < right; i++) {
					var rect = Rectangles.Keys.FirstOrDefault(r => r.Contains(i, j));
					if(rect == Rectangle.Empty) {
						emptyItemSpan += MaxCellCountInLayoutColumn[i];
						continue;
					}
					if(emptyItemSpan > 0) {
						CreateEmptyItem(row, emptyItemSpan);
						emptyItemSpan = 0;
					}
					var column = Rectangles[rect];
					if(!Columns.Contains(column))
						continue;
					Columns.Remove(column);
					var location = GetCaptionLocation(column);
					int colIndex = MaxCellCountInLayoutColumn.Take(rect.X).Sum();
					if(location == EditFormColumnCaptionLocation.Near)
						CreateCaptionItem(row, column, colIndex++);
					var editorCellColSpan = MaxCellCountInLayoutColumn.Skip(rect.X).Take(rect.Width).Sum();
					if(location == EditFormColumnCaptionLocation.Near)
						editorCellColSpan--;
					CreateEditorItem(row, column, editorCellColSpan, rect.Height, location, colIndex);
				}
				if(emptyItemSpan > 0)
					CreateEmptyItem(row, emptyItemSpan);
			}
		}
		void CreateEmptyItem(List<GridViewEditFormLayoutItem> row, int colSpan) {
			row.Add(new GridViewEditFormLayoutItem() {
				Type = GridViewEditFormLayoutItemType.Empty,
				ColSpan = colSpan,
				RowSpan = 1
			});
		}
		void CreateCaptionItem(List<GridViewEditFormLayoutItem> row, EditFormColumn column, int colIndex) {
			row.Add(new GridViewEditFormLayoutItem() {
				Type = GridViewEditFormLayoutItemType.Caption,
				ColIndex = colIndex,
				Column = column,
				ColSpan = 1,
				RowSpan = 1
			});
		}
		void CreateEditorItem(List<GridViewEditFormLayoutItem> row, EditFormColumn column, int colSpan, int rowSpan, EditFormColumnCaptionLocation location, int colIndex) {
			row.Add(new GridViewEditFormLayoutItem() {
				Type = GridViewEditFormLayoutItemType.Editor,
				ColIndex = colIndex,
				Column = column,
				ColSpan = colSpan,
				RowSpan = rowSpan,
				CaptionLocation = location,
				WidthPercent = 100d * GetColSpan(column) / ColCount
			});
		}
		protected bool IsEditable(EditFormColumn column) {
			if(column.Visible == DefaultBoolean.False)
				return false;
			if(column.Visible == DefaultBoolean.True)
				return true;
			return column.ColumnVisible;
		}
		protected int GetColSpan(EditFormColumn column) {
			int span = Math.Min(column.UseEditorColRowSpan ? GetDefaultEditorColumnSpan(column) : column.ColumnSpan, ColCount);
			return span > 0 ? span : 1;
		}
		protected int GetRowSpan(EditFormColumn column) {
			int span = column.UseEditorColRowSpan ? GetDefaultEditorRowSpan(column) : column.RowSpan;
			return span > 0 ? span : 1;
		}
		protected virtual int GetDefaultEditorColumnSpan(EditFormColumn column) {
			if(column.RepositoryItem is RepositoryItemMemoEdit) {
				return Owner.EditFormColumnCount;
			}
			if(column.RepositoryItem is RepositoryItemPictureEdit) return 2;
			return 1;
		}
		protected virtual int GetDefaultEditorRowSpan(EditFormColumn column) {
			if(column.RepositoryItem is RepositoryItemMemoEdit || column.RepositoryItem is RepositoryItemPictureEdit) return 3;
			return 1;
		}
		protected EditFormColumnCaptionLocation GetCaptionLocation(EditFormColumn column) {
			var location = column.CaptionLocation;
			if(location != EditFormColumnCaptionLocation.Default)
				return location;
			return GetRowSpan(column) > 1 ? EditFormColumnCaptionLocation.Top : EditFormColumnCaptionLocation.Near;
		}
		class EditColumnComparer : IComparer<EditFormColumn> {
			public int Compare(EditFormColumn c1, EditFormColumn c2) {
				if(c1 == c2)
					return 0;
				var v1 = c1.VisibleIndex < 0 ? c1.VisibleIndex : c1.VisibleIndex;
				var v2 = c2.VisibleIndex < 0 ? c2.VisibleIndex : c2.VisibleIndex;
				if(v1 != v2)
					return Comparer.Default.Compare(v1, v2);
				if(c1.VisibleIndex == c2.VisibleIndex)
					return Comparer.Default.Compare(c1.GlobalVisibleIndex, c2.GlobalVisibleIndex);
				if(c1.VisibleIndex < 0)
					return 1;
				if(c2.VisibleIndex < 0)
					return -1;
				return Comparer.Default.Compare(c1.VisibleIndex, c2.VisibleIndex);
			}
		}
	}
}
