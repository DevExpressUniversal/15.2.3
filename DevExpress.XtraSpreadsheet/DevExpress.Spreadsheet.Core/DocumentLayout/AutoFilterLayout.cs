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
using System.Drawing;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Mouse;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Forms;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraSpreadsheet.Layout {
	#region AutoFilterLayout
	public class AutoFilterLayout : DocumentItemLayout {
		#region Fields
		protected internal static int ImageSize = 17;
		readonly HotZoneCollection hotZones;
		Page lastPage = null;
		#endregion
		public AutoFilterLayout(SpreadsheetView view)
			: base(view) {
			this.hotZones = new HotZoneCollection();
		}
		#region Properties
		public HotZoneCollection HotZones { get { return hotZones; } }
		#endregion
		public override void Update(Page page) {
			hotZones.Clear();
			UpdateLayout(page, ActiveSheet.AutoFilter);
			TableCollection tables = ActiveSheet.Tables;
			int count = tables.Count;
			for (int i = 0; i < count; i++) {
				Table table = tables[i];
				if (table.ShowAutoFilterButton)
					UpdateLayout(page, table.AutoFilter);
			}
			lastPage = page;
		}
		protected internal void UpdateLayout(Page page, AutoFilterBase filter) {
			if (!filter.Enabled)
				return;
			CellRange range = filter.Range;
			int from = range.TopLeft.Column;
			int to = range.BottomRight.Column;
			for (int i = from; i <= to; i++) {
				HotZone hotZone = CreateFilterColumnHotZone(page, filter, i - from);
				if (hotZone != null)
					hotZones.Add(hotZone);
			}
		}
		public override void Invalidate() {
			hotZones.Clear();
		}
		protected internal override HotZone CalculateHotZone(Point point, Page page) {
			if (!object.ReferenceEquals(lastPage, page) && page != null)
				Update(page);
			return HotZoneCalculator.CalculateHotZone(HotZones, point, View.ZoomFactor, LayoutUnitConverter);
		}
		HotZone CreateFilterColumnHotZone(Page page, AutoFilterBase filter, int filterColumnIndex) {
			AutoFilterColumn filterColumn = filter.FilterColumns[filterColumnIndex];
			if (!filterColumn.ShowFilterButton || filterColumn.HiddenAutoFilterButton)
				return null;
			CellRange range = filter.Range;
			int columnGridIndex = page.GridColumns.LookupItem(range.TopLeft.Column + filterColumnIndex);
			if (columnGridIndex < 0)
				return null;
			int rowGridIndex = page.GridRows.LookupItem(range.TopLeft.Row);
			if (rowGridIndex < 0)
				return null;
			AutoFilterColumnHotZone hotZone = new AutoFilterColumnHotZone(View.Control.InnerControl);
			hotZone.Filter = filter;
			hotZone.FilterColumnIndex = filterColumnIndex;
			hotZone.FilterColumn = filterColumn;
			PageGridItem column = page.GridColumns[columnGridIndex];
			PageGridItem row = page.GridRows[rowGridIndex];
			int size = Math.Min(ImageSize, Math.Min(column.Extent, row.Extent));
			hotZone.Bounds = Rectangle.FromLTRB(column.Far - size, row.Far - size, column.Far, row.Far);
			return hotZone;
		}
	}
	#endregion
	#region AutoFilterColumnHotZone
	public class AutoFilterColumnHotZone : HotZone, IFilterHotZone {
		readonly InnerSpreadsheetControl control;
		public AutoFilterColumnHotZone(InnerSpreadsheetControl control)
			: base(control) {
			this.control = control;
		}
		public override SpreadsheetCursor Cursor { get { return SpreadsheetCursors.Hand; } }
		public AutoFilterBase Filter { get; set; }
		public int FilterColumnIndex { get; set; }
		public AutoFilterColumn FilterColumn { get; set; }
		public Rectangle BoundsHotZone {
			get { return Bounds; }
			set {
				if (BoundsHotZone == value)
					return;
				Bounds = value;
			}
		}
		public SortCondition SortCondition {
			get {
				SortState sortState = Filter.SortState;
				int count = sortState.SortConditions.Count;
				for (int i = 0; i < count; i++) {
					SortCondition condition = sortState.SortConditions[i];
					if (condition.SortReference.TopLeft.Column == Filter.Range.TopLeft.Column + FilterColumnIndex)
						return condition;
				}
				return null;
			}
		}
		public override void Visit(IHotZoneVisitor visitor) {
			visitor.Visit(this);
		}
		public override void Activate(SpreadsheetMouseHandler handler, SpreadsheetHitTestResult result) {
			CellRange range = Filter.Range;
			CellPosition position = new CellPosition(range.TopLeft.Column + FilterColumnIndex, range.TopLeft.Row);
			control.DocumentModel.ActiveSheet.Selection.SetSelection(position);
			AutoFilterViewModel viewModel = new AutoFilterViewModel(control.Owner);
			control.Owner.ShowAutoFilterForm(viewModel);
		}
	}
	#endregion
}
