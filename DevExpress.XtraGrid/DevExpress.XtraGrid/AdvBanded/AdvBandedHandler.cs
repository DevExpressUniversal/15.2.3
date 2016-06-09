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
using DevExpress.XtraGrid.Dragging;
using DevExpress.XtraGrid.Drawing;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid.Handler;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.BandedGrid.Handler;
using DevExpress.XtraGrid.Views.BandedGrid.ViewInfo;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
namespace DevExpress.XtraGrid.Views.BandedGrid.Handler {
	public class AdvBandedViewRowNavigationHelper {
		AdvBandedGridView view;
		public AdvBandedViewRowNavigationHelper(AdvBandedGridView view) {
			this.view = view;
		}
		public AdvBandedGridView View { get { return view; } }
		BandedGridColumn FindColumn(GridBandRow row, int prevColIndex) {
			if(row.Columns.Count == 0) return null;
			if(prevColIndex >= row.Columns.Count) prevColIndex = row.Columns.Count - 1;
			if(prevColIndex < 0) prevColIndex = 0;
			BandedGridColumn col = row.Columns[prevColIndex];
			if(col.OptionsColumn.AllowFocus) return col;
			int delta = 9999;
			col = null;
			for(int n = 0; n < row.Columns.Count; n++) {
				if(row.Columns[n].OptionsColumn.AllowFocus) {
					if(Math.Abs(prevColIndex - n) < delta) {
						delta = Math.Abs(prevColIndex - n);
						col = row.Columns[n];
					}
				}
			}
			return col;
		}
		void UpdateBandRows(GridBandRowCollection bandRows) {
			for(int n = bandRows.Count - 1; n >= 0; n--) {
				GridBandRow row = bandRows[n];
				int count = row.CanFocusedColumnCount;
				if(count == 0)
					bandRows.RemoveAt(n);
				else {
					for(int c = row.Columns.Count - 1; c >= 0; c--) {
						if(!row.Columns[c].OptionsColumn.AllowFocus) row.Columns.RemoveAtCore(c);
					}
				}
			}
		}
		protected ArrayList CreateColumnsArray(GridBandCollection collection, ArrayList rows) {
			if(rows == null) rows = new ArrayList();
			foreach(GridBand band in collection) {
				if(!band.Visible) continue;
				if(band.HasChildren) {
					CreateColumnsArray(band.Children, rows);
					continue;
				}
				if(band.Columns.Count > 0) {
					GridBandRowCollection brows = View.GetBandRows(band, false);
					int zeroFit = rows.Count > 0 ? ((ArrayList)rows[0]).Count : 0;
					while(brows.Count > rows.Count) {
						ArrayList list = new ArrayList();
						for(int n = 0; n < zeroFit; n++) list.Add(null);
						rows.Add(list);
					}
					int maxCCount = 0;
					for(int n = 0; n < brows.Count; n++) {
						GridBandRow brow = brows[n];
						ArrayList list = rows[n] as ArrayList;
						foreach(GridColumn col in brow.Columns) {
							if(col.OptionsColumn.AllowFocus) list.Add(col);
						}
					}
					for(int n = 0; n < rows.Count; n++) {
						ArrayList list = rows[n] as ArrayList;
						maxCCount = Math.Max(list.Count, maxCCount);
					}
					for(int n = 0; n < rows.Count; n++) {
						ArrayList list = rows[n] as ArrayList;
						while(list.Count < maxCCount) list.Add(null);
					}
				}
			}
			return rows;
		}
		public virtual bool DoVertNavigation(KeyEventArgs e, KeyEventHandler baseKeyDown) {
			if(!View.OptionsNavigation.UseAdvVertNavigation) return false;
			int delta = 0;
			switch(e.KeyData) {
				case Keys.Down: delta = 1; break;
				case Keys.Up: delta = -1; break;
			}
			GridBandRowCollection bandRows;
			GridBandRow row;
			if(delta != 0) {
				BandedGridColumn fc = View.FocusedColumn as BandedGridColumn;
				bandRows = View.GetBandRows(fc.OwnerBand);
				UpdateBandRows(bandRows);
				row = bandRows.FindRow(fc);
				int rowIndex = bandRows.IndexOf(row);
				int prevColIndex = 0;
				if(rowIndex != -1) rowIndex += delta;
				if(row != null && fc != null) prevColIndex = row.Columns.IndexOf(fc);
				if(rowIndex != -1 && rowIndex < bandRows.Count) {
					GridBandRow newRow = bandRows[rowIndex];
					if(newRow.Columns.Count > 0) {
						View.FocusedColumn = FindColumn(newRow, prevColIndex);
						return true;
					}
				}
				int savedRow = View.FocusedRowHandle;
				baseKeyDown(this, e);
				if(savedRow != View.FocusedRowHandle) {
					bandRows = View.GetBandRows(fc.OwnerBand);
					UpdateBandRows(bandRows);
					if(bandRows.Count > 0) {
						row = bandRows[delta > 0 ? 0 : bandRows.Count - 1];
						if(row.Columns.Count > 0) {
							GridColumn col = FindColumn(row, prevColIndex);
							if(col != null) View.FocusedColumn = col;
						}
					}
				}
				return true;
			}
			return false;
		}
		public virtual bool DoHorzNavigation(KeyEventArgs e) {
			if(!View.OptionsNavigation.UseAdvHorzNavigation) return false;
			int delta = 0;
			switch(e.KeyData) {
				case Keys.Right: delta = 1; break;
				case Keys.Left: delta = -1; break;
			}
			if(delta == 0) return false;
			BandedGridColumn fc = View.FocusedColumn as BandedGridColumn;
			ArrayList rows = CreateColumnsArray(View.Bands, null);
			ArrayList currentRow = null;
			int rowIndex = 0;
			for(int n = 0; n < rows.Count; n++) {
				currentRow = rows[n] as ArrayList;
				if(currentRow.Contains(fc)) {
					rowIndex = n;
					break;
				}
				currentRow = null;
			}
			if(currentRow == null) return false;
			int curIndex = currentRow.IndexOf(fc);
			BandedGridColumn newColumn = null;
			for(int l = 0; l < 2; l++) {
				int start, realDelta;
				if(l == 0) {
					start = curIndex + delta;
					realDelta = delta;
				}
				else {
					start = delta < 0 ? currentRow.Count - 1 : 0;
					realDelta = -delta;
				}
				for(int n = start; realDelta > 0 ? n < currentRow.Count : n >= 0; n += realDelta) {
					object res = currentRow[n];
					if(res != null) {
						newColumn = res as BandedGridColumn;
						break;
					}
					else {
						for(int r = rowIndex - 1; r >= 0; r--) {
							BandedGridColumn col = ((ArrayList)rows[r])[n] as BandedGridColumn;
							if(col != null && col.OwnerBand != fc.OwnerBand) {
								newColumn = col;
								break;
							}
						}
					}
				}
				if(newColumn != null) break;
			}
			if(newColumn != null) {
				View.FocusedColumn = newColumn;
				return true;
			}
			return false;
		}
		public void ProcessKeyDown(KeyEventArgs e, KeyEventHandler baseKeyDown) {
			if(View.FocusedColumn != null) {
				if(e.KeyData == Keys.Up || e.KeyData == Keys.Down) {
					if(DoVertNavigation(e, baseKeyDown)) return;
				}
				if(e.KeyData == Keys.Left || e.KeyData == Keys.Right) {
					if(DoHorzNavigation(e)) return;
				}
			}
			baseKeyDown(this, e);
		}
	}
	public class BandedGridRegularRowNavigator : GridRegularRowNavigator {
		public BandedGridRegularRowNavigator(BandedGridHandler handler) : base(handler) { }
		protected new AdvBandedGridHandler Handler { get { return base.Handler as AdvBandedGridHandler; } }
		public override void OnKeyDown(KeyEventArgs e) {
			Handler.Helper.ProcessKeyDown(e, new KeyEventHandler(OnBaseKeyDown));
		}
		void OnBaseKeyDown(object sender, KeyEventArgs e) { base.OnKeyDown(e);  }
	}
	public class BandedGridNewRowNavigator : GridNewRowNavigator {
		public BandedGridNewRowNavigator(BandedGridHandler handler) : base(handler) { }
		protected new AdvBandedGridHandler Handler { get { return base.Handler as AdvBandedGridHandler; } }
		public override void OnKeyDown(KeyEventArgs e) {
			Handler.Helper.ProcessKeyDown(e, new KeyEventHandler(OnBaseKeyDown));
		}
		void OnBaseKeyDown(object sender, KeyEventArgs e) { base.OnKeyDown(e); }
	}
	public class BandedGridTopNewRowNavigator : GridTopNewRowNavigator {
		public BandedGridTopNewRowNavigator(BandedGridHandler handler) : base(handler) { }
		protected new AdvBandedGridHandler Handler { get { return base.Handler as AdvBandedGridHandler; } }
		public override void OnKeyDown(KeyEventArgs e) {
			Handler.Helper.ProcessKeyDown(e, new KeyEventHandler(OnBaseKeyDown));
		}
		void OnBaseKeyDown(object sender, KeyEventArgs e) { base.OnKeyDown(e); }
	}
	public class BandedGridFilterRowNavigator : GridFilterRowNavigator {
		public BandedGridFilterRowNavigator(BandedGridHandler handler) : base(handler) { }
		protected new AdvBandedGridHandler Handler { get { return base.Handler as AdvBandedGridHandler; } }
		public override void OnKeyDown(KeyEventArgs e) {
			Handler.Helper.ProcessKeyDown(e, new KeyEventHandler(OnBaseKeyDown));
		}
		void OnBaseKeyDown(object sender, KeyEventArgs e) { base.OnKeyDown(e); }
	}
	public class AdvBandedGridHandler : BandedGridHandler {
		AdvBandedViewRowNavigationHelper helper;
		public AdvBandedGridHandler(AdvBandedGridView gridView) : base(gridView) { }
		public new AdvBandedGridView View { get { return base.View as AdvBandedGridView; } }
		public new AdvBandedGridViewInfo ViewInfo { get { return base.ViewInfo as AdvBandedGridViewInfo; } }
		protected override GridRowNavigator CreateRowNavigator() { return new BandedGridRegularRowNavigator(this); }
		protected override GridRowNavigator CreateFilterRowNavigator() { return new BandedGridFilterRowNavigator(this); }
		protected override GridRowNavigator CreateNewRowNavigator() { return new BandedGridNewRowNavigator(this); }
		protected override GridRowNavigator CreateTopNewRowNavigator() { return new BandedGridTopNewRowNavigator(this); }
		public AdvBandedViewRowNavigationHelper Helper {
			get {
				if(helper == null) helper = new AdvBandedViewRowNavigationHelper(View);
				return helper;
			}
		}
		protected override GridDragManager CreateDragManager() { return new AdvBandedGridDragManager(View); }
	}
}
