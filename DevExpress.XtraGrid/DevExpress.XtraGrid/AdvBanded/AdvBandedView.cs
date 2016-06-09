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
using System.Data;
using System.Collections;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraGrid.Drawing;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.BandedGrid.ViewInfo;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Printing;
using DevExpress.Utils.Serializing;
using System.Text;
using DevExpress.XtraEditors;
namespace DevExpress.XtraGrid.Views.BandedGrid {
	[Designer("DevExpress.XtraGrid.Design.BandedGridViewDesignTimeDesigner, " + AssemblyInfo.SRAssemblyGridDesign, typeof(System.ComponentModel.Design.IDesigner))]
	public class AdvBandedGridView : BandedGridView {
		public AdvBandedGridView(GridControl ownerGrid) : this() {
			SetGridControl(ownerGrid);
		}
		public AdvBandedGridView() { }
		protected override BaseViewInfo CreateNullViewInfo() { return new NullAdvBandedGridViewInfo(this); }
		protected override BaseViewPrintInfo CreatePrintInfoInstance(PrintInfoArgs args) {
			return new AdvBandedGridViewPrintInfo(args);
		}
		protected override ColumnViewOptionsView CreateOptionsView() { return new AdvBandedGridOptionsView(); }
		bool ShouldSerializeOptionsView() { return OptionsView.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("AdvBandedGridViewOptionsView"),
#endif
 DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue), XtraSerializablePropertyId(LayoutIdOptionsView)]
		public new AdvBandedGridOptionsView OptionsView {
			get { return base.OptionsView as AdvBandedGridOptionsView; }
		}
		protected override ColumnViewOptionsSelection CreateOptionsSelection() { return new AdvBandedGridOptionsSelection(); }
		bool ShouldSerializeOptionsSelection() { return OptionsSelection.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("AdvBandedGridViewOptionsSelection"),
#endif
 DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public new AdvBandedGridOptionsSelection OptionsSelection {
			get { return base.OptionsSelection as AdvBandedGridOptionsSelection; }
		}
		protected override GridOptionsNavigation CreateOptionsNavigation() { return new AdvBandedGridOptionsNavigation(); }
		bool ShouldSerializeOptionsNavigation() { return OptionsNavigation.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("AdvBandedGridViewOptionsNavigation"),
#endif
 DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public new AdvBandedGridOptionsNavigation OptionsNavigation {
			get { return base.OptionsNavigation as AdvBandedGridOptionsNavigation; }
		}
		protected internal override bool AllowPartialGroups { get { return false; } }
		protected override bool IsSupportRightToLeft {
			get {
				return true;
			}
		}
		protected internal override int GetColumnVisibleIndex(BandedGridColumn column) {
			if(column == null || column.OwnerBand == null) return -1;
			GridBandRowCollection rows = GetBandRows(column.OwnerBand, true);
			GridBandRow row = rows.FindRow(column);
			if(row == null) return -1;
			return row.Columns.VisibleIndexOf(column);
		}
		protected internal override bool IsAlignGroupRowSummariesUnderColumns { get { return false; } }
		protected internal override void SetColumnVisibleIndex(BandedGridColumn column, int colVIndex) {
			if(colVIndex < 0) {
				column.Visible = false;
				return;
			}
			if(column.OwnerBand == null) return;
			GridBandRowCollection rows = GetBandRows(column.OwnerBand, true);
			GridBandRow row = rows.FindRow(column);
			if(row == null) return;
			int curIndex = row.Columns.VisibleIndexOf(column);
			if(curIndex == colVIndex && column.Visible) return;
			BeginUpdate();
			try {
				column.Visible = true;
				row.Columns.MoveToVisibleIndex(column, colVIndex);
				column.OwnerBand.Columns.Assign(rows);
			}
			finally {
				EndUpdate();
			}
		}
		public void SetColumnWidth(BandedGridColumn column, int width) {
			if(width < column.MinWidth) width = column.MinWidth;
			if(!column.Visible || column.OwnerBand == null) {
				column.Width = width;
				return;
			}
			if(column.Width == width) return;
			SetColumnWidthCore(column, width);
		}
		void SetColumnWidthCore(BandedGridColumn col, int newWidth) {
			int delta = newWidth - col.Width;
			Hashtable hash = SaveWidth(col.OwnerBand);
			col.OwnerBand.Width += delta;
			BeginUpdate();
			if(hash[col] != null) hash[col] = newWidth;
			ApplyWidth(hash);
			EndUpdate();
		}
		void ApplyWidth(Hashtable hash) {
			foreach(DictionaryEntry entry in hash) { ((GridColumn)entry.Key).Width = (int)entry.Value; }
		}
		Hashtable SaveWidth(GridBand band) {
			Hashtable res = new Hashtable();
			foreach(GridColumn col in band.Columns) {
				res[col] = col.Width;
			}
			return res;
		}
		public virtual void SetColumnPosition(BandedGridColumn column, int rowIndex, int colIndex) {
			if(column.OwnerBand == null) return;
			bool anyChanges = false;
			BeginUpdate();
			try {
				LockRaiseColumnPositionChanged();
				if(!column.Visible) {
					anyChanges = true;
					column.Visible = true;
				}
				GridBandRowCollection rows = GetBandRows(column.OwnerBand, true);
				if(column.RowIndex != rowIndex || colIndex == -1) {
					GridBandRow currentRow = rows.FindRow(column);
					if(currentRow != null) currentRow.Columns.RemoveCore(column);
					if(rowIndex >= 0 && rowIndex < rows.Count && colIndex != -1) {
						GridBandRow destRow = rows[rowIndex];
						column.SetRowIndexCore(rowIndex);
						destRow.Columns.MoveToVisibleIndex(column, colIndex);
					} else {
						GridBandRow destRow = new GridBandRow();;
						if(rowIndex < rows.Count) {
							rows.Insert(rowIndex, destRow);
							column.SetRowIndexCore(rowIndex);
						}
						else
							column.SetRowIndexCore(rows.Add(destRow));
						destRow.Columns.AddCore(column);
					}
					anyChanges = true;
					column.OwnerBand.Columns.Assign(rows);
					ApplyBandRowValues(column.OwnerBand, rows);
				} else {
					GridBandRow row = rows.FindRow(column);
					if(row == null) return;
					if(colIndex < 0) colIndex = 0;
					if(colIndex > row.Columns.Count) colIndex = row.Columns.Count;
					int curIndex = row.Columns.IndexOf(column);
					if(colIndex != curIndex) {
						anyChanges = true;
						row.Columns.MoveToVisibleIndex(column, colIndex);
						column.OwnerBand.Columns.Assign(rows);
					}
				}
			}
			finally {
				UnlockRaiseColumnPositionChanged();
				if(anyChanges) {
					EndUpdate();
					RaiseColumnPositionChanged(column);
				}
				else
					CancelUpdate();
			}
		}
		protected internal override int CalcBandColIndex(BandedGridColumn column) { 
			if(IsLoading || column == null) return -1;
			if(column.OwnerBand == null) return -1;
			GridBandRowCollection rows = GetBandRows(column.OwnerBand);
			if(rows == null) return -1;
			GridBandRow row = rows.FindRow(column);
			if(row == null) return -1;
			return row.Columns.IndexOf(column);
		}
		protected internal override GridBandRowCollection GetBandRows(GridBand band, bool includeNonVisible) {
			GridBandRowCollection rows = new GridBandRowCollection();
			int vCount = includeNonVisible ? band.Columns.Count : band.Columns.VisibleColumnCount;
			int addCount = 0;
			if(vCount == 0) return rows;
			for(int n = 0;; n++) {
				GridBandRow row = new GridBandRow();
				while(true) {
					for(int k = 0; k < band.Columns.Count; k++) {
						BandedGridColumn col = band.Columns[k];
						if(!col.Visible && !includeNonVisible) continue; 
						int rowIndex = col.RowIndex;
						if(rowIndex < 0) rowIndex = 0;
						if(rowIndex < n) continue;
						if(rowIndex == n) {
							row.Columns.AddCore(col);
							addCount ++;
						}
					}
					if(addCount == vCount) break;
					if(row.Columns.Count == 0) n++;
					else break;
				}
				if(row.Columns.Count == 0) break;
				rows.Add(row);
			}
			return rows;
		}
		protected override string ViewName { get { return "AdvBandedGridView"; } }
		protected new AdvBandedGridViewInfo ViewInfo { get { return base.ViewInfo as AdvBandedGridViewInfo; } }
		protected internal override void OnColumnRowIndexChanged(BandedGridColumn column) {
			if(IsLoading || IsDeserializing) return;
			UpdateBandedColumnIndexes(column);
			OnPropertiesChanged();
		}
		protected override bool GetDataRowText(StringBuilder sb, int rowHandle) {
			for(int n = 0; n < VisibleColumns.Count; n++) {
				sb.Append(rowHandle == InvalidRowHandle ?  VisibleColumns[n].GetTextCaption(): GetRowCellDisplayText(rowHandle, VisibleColumns[n]));
				if(n < VisibleColumns.Count - 1) sb.Append(columnSeparator);
			}
			return true;
		}
		protected internal override void UpdateBandColumnsRowValues(GridBand band) {
			if(IsLockUpdate || IsLoading || ViewDisposing || IsDeserializing) return;
			ApplyBandRowValues(band, GetBandRows(band, true));
			band.Columns.SortCore(new BandedColumnRowIndexComparer());
		}
		protected virtual void UpdateBandedColumnIndexes(BandedGridColumn column) {
			if(column != null) {
				if(column.OwnerBand == null) return;
				UpdateBandColumnsRowValues(column.OwnerBand);
			}
			RefreshVisibleColumnsList();
		}
		protected override void CreateVisibleListOnBands(GridBand[] bands) {
			for(int n = 0; n < bands.Length; n++) {
				GridBand band = bands[n];
				if(band.Columns.Count == 0) continue;
				band.Columns.ResetIndexes();
				BandedGridColumn[] cols = new BandedGridColumn[band.Columns.Count];
				ICollection collection = band.Columns;
				collection.CopyTo(cols, 0);
				Array.Sort(cols, new BandedColumnRowIndexComparer());
				foreach(BandedGridColumn column in cols) {
					if(column.Visible) VisibleColumnsCore.Show(column, VisibleColumnsCore.Count);
					else VisibleColumnsCore.Hide(column);
				}
				if(GridControl == null) return;
				GridBandRowCollection rows = GetBandRows(band, false);
				ApplyBandRowValues(band, rows);
			}
		}
		protected class BandedColumnRowIndexComparer : IComparer {
			int IComparer.Compare(object a, object b) {
				BandedGridColumn c1 = (BandedGridColumn)a, c2 = (BandedGridColumn)b;
				if(c1 == c2) return 0;
				if(c1.RowIndex == c2.RowIndex) return Comparer.Default.Compare(c1.InternalColIndexCore, c2.InternalColIndexCore);
				return Comparer.Default.Compare(c1.RowIndex, c2.RowIndex);
			}
		}
		public override DevExpress.XtraGrid.Export.BaseExportLink CreateExportLink(DevExpress.XtraExport.IExportProvider provider) {
			return new DevExpress.XtraGrid.Export.AdvBandedViewExportLink(this, provider);
		}
		protected internal override bool AllowAddNewSummaryItemMenu {
			get {
				return OptionsMenu.ShowAddNewSummaryItem == DefaultBoolean.True;
			}
		}
	}
}
