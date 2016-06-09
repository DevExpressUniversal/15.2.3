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
using System.Web.UI.WebControls;
using DevExpress.Utils;
namespace DevExpress.Web.Rendering {
	public enum GridViewEditFormLayoutItemType { Caption, Editor, Empty }
	public class GridViewEditFormLayoutItem {
		public GridViewDataColumn Column { get; set; }
		public GridViewEditFormLayoutItemType Type { get; set; }
		public int ColSpan { get; set; }
		public int RowSpan { get; set; }
		public ASPxColumnCaptionLocation CaptionLocation { get; set; }
		public Unit Width { get; set; }
	}
	public class GridViewEditFormLayout {
		protected GridViewEditFormLayout(ASPxGridView grid) {
			Grid = grid;
			Columns = new List<GridViewDataColumn>();
			Rectangles = new Dictionary<Rectangle, GridViewDataColumn>();
			Layout = new List<List<GridViewEditFormLayoutItem>>();
			MaxCellCountInLayoutColumn = new List<int>(Enumerable.Range(0, ColCount).Select(i => 1));
			Build();
		}
		public static List<List<GridViewEditFormLayoutItem>> CreateLayout(ASPxGridView grid) {
			return new GridViewEditFormLayout(grid).Layout;
		}
		protected List<GridViewDataColumn> Columns { get; private set; }
		protected ASPxGridView Grid { get; private set; }
		protected Dictionary<Rectangle, GridViewDataColumn> Rectangles { get; private set; }
		protected List<List<GridViewEditFormLayoutItem>> Layout { get; private set; }
		protected List<int> MaxCellCountInLayoutColumn { get; private set; }
		protected int ColCount { get { return Grid.SettingsEditing.EditFormColumnCount; } }
		protected virtual void Build() {
			BuildColumns();
			BuildRectangles();
			BuildLayout();
		}
		protected virtual void BuildColumns() {
			Columns.AddRange(
				Grid.DataColumns.Where(c => IsEditable(c)).OrderBy(c => c, new EditColumnComparer())
			);
		}
		protected virtual void BuildRectangles() {
			var x = 0;
			var y = 0;
			foreach(var column in Columns) {
				var rect = GetNextEmptyRect(ref x, ref y, GetColSpan(column), GetRowSpan(column));
				Rectangles[rect] = column;
				if(rect.Width == 1 && GetCaptionLocation(column) == ASPxColumnCaptionLocation.Near)
					MaxCellCountInLayoutColumn[x] = 2;
			}
		}
		protected virtual Rectangle GetNextEmptyRect(ref int x, ref int y, int width, int height) {
			Rectangle rect = Rectangle.Empty;
			while(true) {
				if(x >= ColCount) {
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
			if(Columns.Count == 0) return;
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
					if(location == ASPxColumnCaptionLocation.Near)
						CreateCaptionItem(row, column);
					var editorCellColSpan = MaxCellCountInLayoutColumn.Skip(rect.X).Take(rect.Width).Sum();
					if(location == ASPxColumnCaptionLocation.Near)
						editorCellColSpan--;
					CreateEditorItem(row, column, editorCellColSpan, rect.Height, location);
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
		void CreateCaptionItem(List<GridViewEditFormLayoutItem> row, GridViewDataColumn column) {
			row.Add(new GridViewEditFormLayoutItem() {
				Type = GridViewEditFormLayoutItemType.Caption,
				Column = column,
				ColSpan = 1,
				RowSpan = 1
			});
		}
		void CreateEditorItem(List<GridViewEditFormLayoutItem> row, GridViewDataColumn column, int colSpan, int rowSpan, ASPxColumnCaptionLocation location) {
			row.Add(new GridViewEditFormLayoutItem() {
				Type = GridViewEditFormLayoutItemType.Editor,
				Column = column,
				ColSpan = colSpan,
				RowSpan = rowSpan,
				CaptionLocation = location,
				Width = Unit.Percentage(100.0 * GetColSpan(column) / ColCount)
			});
		}
		protected bool IsEditable(GridViewDataColumn column) {
			if(column.EditFormSettings.Visible == DefaultBoolean.False)
				return false;
			if(column.EditFormSettings.Visible == DefaultBoolean.True)
				return true;
			return column.Visible;
		}
		protected int GetColSpan(GridViewDataColumn column) {
			int span = Math.Min(column.EditFormSettings.ColumnSpan, ColCount);
			return span > 0 ? span : 1;
		}
		protected int GetRowSpan(GridViewDataColumn column) {
			int span = column.EditFormSettings.RowSpan;
			return span > 0 ? span : 1;
		}
		protected ASPxColumnCaptionLocation GetCaptionLocation(GridViewDataColumn column) {
			var location = column.EditFormSettings.CaptionLocation;
			if(location != ASPxColumnCaptionLocation.Default)
				return location;
			return GetRowSpan(column) > 1 ? ASPxColumnCaptionLocation.Top : ASPxColumnCaptionLocation.Near;
		}
		class EditColumnComparer : IComparer<GridViewDataColumn> {
			public int Compare(GridViewDataColumn c1, GridViewDataColumn c2) {
				if(c1 == c2)
					return 0;
				var v1 = c1.EditFormSettings.VisibleIndex < 0 ? c1.VisibleIndex : c1.EditFormSettings.VisibleIndex;
				var v2 = c2.EditFormSettings.VisibleIndex < 0 ? c2.VisibleIndex : c2.EditFormSettings.VisibleIndex;
				if(v1 != v2)
					return Comparer.Default.Compare(v1, v2);
				if(c1.EditFormSettings.VisibleIndex == c2.EditFormSettings.VisibleIndex)
					return Comparer.Default.Compare(c1.Grid.GetColumnGlobalIndex(c1), c2.Grid.GetColumnGlobalIndex(c2));
				if(c1.EditFormSettings.VisibleIndex < 0)
					return 1;
				if(c2.EditFormSettings.VisibleIndex < 0)
					return -1;
				return Comparer.Default.Compare(c1.EditFormSettings.VisibleIndex, c2.EditFormSettings.VisibleIndex);
			}
		}
	}
}
