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

using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
namespace DevExpress.XtraReports.Native {
	public class TableModifier {
		protected XRTable table;
		public TableModifier(XRTable table) {
			this.table = table;
		}
		protected virtual void AssignRowWeight(XRTableRow row, double weight) {
			row.Weight = weight;
		}
		protected virtual void AssignTableHeight(float height) {
			table.HeightF = height;
		}
		protected virtual void AssignTableWidth(float width) {
			table.WidthF = width;
		}
		protected virtual void AssignTableLeft(float left) {
			table.LeftF = left;
		}
		protected virtual void AssignTableTop(float top) {
			table.TopF = top;
		}
		public void DeleteColumn(float lowestWidth, bool isLeftColumn) {
			AssignTableWidth(table.WidthF - lowestWidth);
			if(isLeftColumn) AssignTableLeft(table.LeftF + lowestWidth);
		}
		public void DeleteRow(XRTableRow row) {
			if(!table.Rows.Contains(row))
				throw new ArgumentException("row doesn't belong to this XRTable");
			if(row.IsSingleChild)
				DeleteSingleRow();
			else {
				AssignTableHeight(table.HeightF - row.HeightF);
				DeleteOrdinaryRow(row);
				if(!table.IsLoading)
					table.ArrangeRows();
			}
		}
		protected virtual void DeleteOrdinaryRow(XRTableRow row) {
			if(table.Rows.Contains(row))
				table.Rows.Remove(row);
		}
		protected virtual void DeleteSingleRow() {
			if(table.Parent != null && table.Parent.Controls.Contains(table))
				table.Parent.Controls.Remove(table);
		}
		public XRTableRow InsertRowByIndex(XRTableRow baseRow, int index) {
			if(baseRow != null && !table.Rows.Contains(baseRow)) throw new ArgumentException("baseRow doesn't belong to this XRTable");
			bool isLoading = table.IsLoading;
			if(!isLoading)
				table.BeginInit();
			try {
				XRTableRow row = CreateRow();
				InsertRowInTable(index, row);
				if(baseRow == null) {
					AssignRowWeight(row, WeightHelper.DefaultWeight);
				} else {
					XRTableCell[] tableCells = CloneRowCells(baseRow);
					row.SetCellRange(tableCells);
					AssignRowWeight(row, baseRow.Weight);
					if(baseRow.Index > index)
						AssignTableTop(table.TopF - baseRow.HeightF);
					AssignTableHeight(table.HeightF + baseRow.HeightF);
				}
				return row;
			} finally {
				if(!isLoading)
					table.EndInit();
			}
		}
		public XRTableCell[] InsertColumn(XRTableCell baseCell, CellInsertPosition position, bool autoExpandTable, bool inheritBaseCellAppearance) {
			if(baseCell != null && !table.ContainsCell(baseCell)) throw new ArgumentException("baseCell doesn't belong to this XRTable");
			XRTableCell[] result = null;
			int i = 0;
			bool isLoading = table.IsLoading;
			if(!isLoading)
				table.BeginInit();
			try {
				if(baseCell == null) {
					result = new XRTableCell[table.Rows.Count];
					foreach(XRTableRow row in table.Rows) {
						XRTableCell cell = new XRTableCell();
						row.InsertCell(null, cell, position, autoExpandTable, inheritBaseCellAppearance);
						result[i++] = cell;
					}
				} else {
					List<XRTableCell> cells = table.GetAlignedCells(baseCell, position);
					result = new XRTableCell[cells.Count];
					if(cells.Count == 0) return result;
					foreach(XRTableCell neighbourCell in cells) {
						XRTableCell cell = new XRTableCell();
						neighbourCell.Row.InsertCell(neighbourCell, cell, position, autoExpandTable, inheritBaseCellAppearance);
						result[i++] = cell;
					}
					if(autoExpandTable) {
						AssignTableWidth(table.WidthF + baseCell.WidthF);
						if(position == CellInsertPosition.Left)
							AssignTableLeft(table.LeftF - baseCell.WidthF);
					}
					cells.Clear();
				}
			} finally {
				if(!isLoading)
					table.EndInit();
			}
			return result;
		}
		protected virtual void InsertRowInTable(int index, XRTableRow row) {
			table.Rows.Insert(index, row);
		}
		protected virtual XRTableRow CreateRow() {
			return new XRTableRow();
		}
		protected virtual XRTableCell[] CloneRowCells(XRTableRow baseRow) {
			return baseRow.CloneCells();
		}
		public XRControl[] ConvertToControls() {
			List<XRControl> controls = CreateControls(table);
			RemoveControl(table);
			return controls.ToArray();
		}
		protected virtual void AddControl(XRControl control, XRControl newControl) {
			control.Controls.Add(newControl);
		}
		protected virtual void RemoveControl(XRControl control) {
			control.Parent.Controls.Remove(control);
		}
		protected virtual void CollectionChanging(XRControl control) { }
		protected virtual void CollectionChanged(XRControl control) { }
		List<XRControl> CreateControls(XRTable table) {
			List<XRControl> controls = new List<XRControl>();
			foreach(XRTableRow row in table.Rows) {
				foreach(XRTableCell cell in row.Cells) {
					controls.Add(cell.Controls.Count == 0 ? (XRControl)ConvertToLabel(cell) : ConvertToPanel(cell));
				}
			}
			return controls;
		}
		XRPanel ConvertToPanel(XRTableCell cell) {
			XRPanel panel = new XRPanel();
			CopyCommonProperties(cell, panel);
			AddControl(table.Parent, panel);
			CollectionChanging(cell);
			CollectionChanging(panel);
			for(int i = cell.Controls.Count - 1; i >= 0; i--) { 
				XRControl control = cell.Controls[i];
				cell.Controls.RemoveAt(i);
				panel.Controls.Add(control);
			}
			CollectionChanged(panel);
			CollectionChanged(cell);
			return panel;
		}
		XRLabel ConvertToLabel(XRTableCell cell) {
			XRLabel label = new XRLabel();
			CopyCommonProperties(cell, label);
			label.Text = cell.Text;
			label.XlsxFormatString = cell.XlsxFormatString;
			label.NullValueText = cell.NullValueText;
			label.Summary = cell.Summary;
			label.Angle = cell.Angle;
			label.CanShrink = cell.CanShrink;
			label.Multiline = cell.Multiline;
			label.ProcessDuplicatesTarget = cell.ProcessDuplicatesTarget;
			label.ProcessDuplicatesMode = cell.ProcessDuplicatesMode;
			label.ProcessNullValues = cell.ProcessNullValues;
			label.WordWrap = cell.WordWrap;
			AddControl(table.Parent, label);
			return label;
		}
		static void CopyCommonProperties(XRTableCell srcCell, XRControl dstControl) {
			dstControl.SyncDpi(srcCell.Dpi);
			dstControl.LocationF = GetLocation(srcCell);
			dstControl.SizeF = srcCell.SizeF;
			dstControl.AssignStyle(srcCell.GetEffectiveXRStyle());
			dstControl.Bookmark = srcCell.Bookmark;
			dstControl.BookmarkParent = srcCell.BookmarkParent;
			dstControl.NavigateUrl = srcCell.NavigateUrl;
			dstControl.Target = srcCell.Target;
			dstControl.Tag = srcCell.Tag;
			dstControl.KeepTogether = srcCell.KeepTogether;
			dstControl.Visible = srcCell.Visible;
			dstControl.CanGrow = srcCell.CanGrow;
			foreach(PropertyDescriptor pd in TypeDescriptor.GetProperties(dstControl, new Attribute[] { new BindableAttribute(true) })) {
				XRBinding binding = srcCell.DataBindings[pd.Name];
				if(binding != null)
					dstControl.DataBindings.Add((XRBinding)((ICloneable)binding).Clone());
			}
		}
		static PointF GetLocation(XRTableCell cell) {
			PointF location = PointF.Add(cell.LocationF, PointToSize(cell.Row.LocationF));
			location = PointF.Add(location, PointToSize(cell.Row.Table.LocationF));
			return location;
		}
		static SizeF PointToSize(PointF src) {
			return new SizeF(src.X, src.Y);
		}
	}
}
