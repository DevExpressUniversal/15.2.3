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
using DevExpress.Data.Details;
using DevExpress.Utils;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Drawing;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Scrolling;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraPrinting;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraGrid.Views.Printing;
using DevExpress.XtraEditors.Repository;
using System.Collections.Generic;
using DevExpress.XtraPrinting.NativeBricks;
using DevExpress.XtraGrid.Printing;
using DevExpress.Utils.Text;
using DevExpress.Data.Summary;
using DevExpress.XtraExport.Helpers;
using DevExpress.Export.Xl;
using System.Linq;
using DevExpress.Sparkline;
namespace DevExpress.XtraGrid.Views.Printing {
	public class PrintInfoArgs {
		public BaseView View;
		public int Indent;
		public int Y;
		public IBrickGraphics Graph;
		public ViewPrintWrapper PrintWrapper;
		public PrintInfoArgs(BaseView view) : this(view, null, null, 0) { }
		public PrintInfoArgs(BaseView view, ViewPrintWrapper printWrapper, IBrickGraphics graph, int y) {
			this.PrintWrapper = printWrapper;
			this.Indent = printWrapper == null ? 0 : printWrapper.PrintIndent;
			this.View = view;
			this.Y = y;
			this.Graph = graph;
		}
	}
	public class BrickCache {
		Hashtable bricks;
		public BrickCache() {
			this.bricks = new Hashtable();
		}
		public BrickStyle this[string name] { get { return Bricks[name] as BrickStyle; } }
		protected Hashtable Bricks { get { return bricks; } }
		public void Add(string name, AppearanceObject appearance, BorderSide borderSide, Color borderColor, int borderWidth) {
			BrickStyle brick = Create(appearance, borderSide, borderColor, borderWidth);
			Bricks[name] = brick;
		}
		public void Add(string name, AppearanceObject appearance) {
			Add(name, appearance, BorderSide.None, Color.Transparent, 1);
		}
		public void Clear() {
			Bricks.Clear();
		}
		public BrickStyle Create(AppearanceObject appearance, BorderSide borderSide, Color borderColor, int borderWidth) {
			if(borderColor == Color.Empty) borderColor = Color.Gray;
			BrickStringFormat format = new BrickStringFormat(appearance.GetStringFormat());
			format.PrototypeKind = BrickStringFormatPrototypeKind.GenericTypographic;
			format.Value.FormatFlags |= StringFormatFlags.LineLimit;
			BrickStyle brick = new BrickStyle(borderSide, borderWidth, borderColor, appearance.GetBackColor(),
				appearance.GetForeColor(), appearance.GetFont(), format);
			brick.TextAlignment = TextAlignmentFromTextOptions(appearance.TextOptions);
			return brick;
		}
		TextAlignment TextAlignmentFromTextOptions(TextOptions to) {
			return DevExpress.XtraPrinting.Native.TextAlignmentConverter.ToTextAlignment(to.HAlignment, to.VAlignment);
		}
	}
	public class CancelPrintRowEventArgs :PrintRowEventArgs {
		bool cancelCore = false;
		public CancelPrintRowEventArgs(IPrintingSystem ips, Graphics g, IBrickGraphics graph, int rowHandle, int level, int x, int y) : base(ips, g, graph, rowHandle, level, x, y) { }
		public CancelPrintRowEventArgs(IPrintingSystem ips, Graphics g, IBrickGraphics graph, int rowHandle, int level, int x, int y, bool hasRowFooter) : base(ips, g, graph, rowHandle, level, x, y, hasRowFooter) { }
		public bool Cancel {
			get { return cancelCore; }
			set { cancelCore = value; }
		}
	}
	public class PrintRowEventArgs :EventArgs {
		Graphics gCore;
		IBrickGraphics graphCore;
		IPrintingSystem psCore;
		int rowHandleCore;
		int levelCore;
		int xCore, yCore;
		bool hasFooter;
		public PrintRowEventArgs(IPrintingSystem ips, Graphics g, IBrickGraphics graph, int rowHandle, int level, int x, int y, bool hasRowFooter)
			: this(ips, g, graph, rowHandle, level, x, y) {
			hasFooter = hasRowFooter;
		}
		public PrintRowEventArgs(IPrintingSystem ips, Graphics g, IBrickGraphics graph, int rowHandle, int level, int x, int y) {
			gCore = g;
			graphCore = graph;
			rowHandleCore = rowHandle;
			levelCore = level;
			yCore = y;
			xCore = x;
			psCore = ips;
		}
		public int X { get { return xCore; } set { xCore = value; } }
		public int Y { get { return yCore; } set { yCore = value; } }
		public int Level { get { return levelCore; } }
		public int RowHandle { get { return rowHandleCore; } }
		public Graphics Graphics { get { return gCore; } }
		public IBrickGraphics BrickGraphics { get { return graphCore; } }
		public IPrintingSystem PS { get { return psCore; } }
		public bool HasFooter { get { return hasFooter; } }
	}
	public class GridViewPrintInfo :ColumnViewPrintInfo {
		List<PrintColumnInfo> columns;
		protected int headerRowHeight;
		int prevLevel = -1, prevRowHandle, currentRowHeight,
			groupFooterHeight, footerPanelHeight;
		GridColumnsInfo columnsInfo;
		protected int fMaxRowWidth;
		GridRowInfo currentRowInfo;
		public GridViewPrintInfo(PrintInfoArgs args)
			: base(args) {
			this.columnsInfo = new GridColumnsInfo();
			this.columns = new List<PrintColumnInfo>();
			this.currentRowHeight = 0;
			this.fMaxRowWidth = 0;
			this.currentRowInfo = null;
		}
		protected override BaseViewAppearanceCollection CreatePrintAppearance() { return new GridViewPrintAppearances(View); }
		public new GridViewPrintAppearances AppearancePrint { get { return base.AppearancePrint as GridViewPrintAppearances; } }
		protected override bool PrintSelectedRowsOnly { get { return View.OptionsPrint.PrintSelectedRowsOnly; } }
		public GridColumnsInfo ColumnsInfo { get { return columnsInfo; } }
		public List<PrintColumnInfo> Columns { get { return columns; } }
		protected new GridViewInfo ViewViewInfo { get { return base.ViewViewInfo as GridViewInfo; } }
		public new GridView View { get { return base.View as GridView; } }
		protected int CurrentRowHeight { get { return currentRowHeight; } }
		protected virtual int HeaderY { get { return 0; } }
		public override void PrintViewHeader(IBrickGraphics graph) {
			int width = 0;
			foreach(PrintColumnInfo col in Columns) if(width < col.Bounds.Right) width = col.Bounds.Right;
			PrintViewHeaderCore(graph, width);
		}
		public override void PrintHeader(DevExpress.XtraPrinting.IBrickGraphics graph) {
			if(!View.OptionsPrint.PrintHeader) return;
			Point indent = new Point(Indent, HeaderY);
			Rectangle r = Rectangle.Empty;
			bool usePrintStyles = View.OptionsPrint.UsePrintStyles;
			SetDefaultBrickStyle(graph, Bricks["HeaderPanel"]);
			foreach(PrintColumnInfo col in Columns) {
				if(!usePrintStyles) {
					AppearanceObject temp = new AppearanceObject();
					AppearanceHelper.Combine(temp, new AppearanceObject[] { col.Column.AppearanceHeader, View.Appearance.HeaderPanel, AppearancePrint.HeaderPanel });
					SetDefaultBrickStyle(graph, Bricks.Create(temp, BorderSide.All, temp.BorderColor, 1));
				}
				r = col.Bounds;
				r.Offset(indent);
				string caption = col.Column.GetTextCaptionForPrinting();
				if(!col.Column.OptionsColumn.ShowCaption) caption = string.Empty;
				ITextBrick itb = DrawTextBrick(graph, caption, r);
				if(caption.Contains(Environment.NewLine)) itb.Style.StringFormat = BrickStringFormat.Create(itb.Style.TextAlignment, true);
				if(AppearancePrint.HeaderPanel.TextOptions.WordWrap == WordWrap.NoWrap && View.OptionsPrint.UsePrintStyles) {
					using(Graphics g = this.View.GridControl.CreateGraphics()) {
						SizeF s = g.MeasureString(itb.Text, itb.Font, 1000, itb.StringFormat.Value);
						if(s.Width + 5 >= r.Width) {
							itb.Text = "";
							itb.TextValue = "";
						}
					}
				}
			}
		}
		int rowCount = -1;
		int currentPrintedRow = -1;
		Graphics g = null;
		public override void PrintNextRow(DevExpress.XtraPrinting.IBrickGraphics graph) {
			if(CancelPending) return;
			if(rowCount < 0) { PreparePrintRows(out rowCount, out g); currentPrintedRow = 0; }
			Y = 0;
			PrintRowCore(graph, g, currentPrintedRow);
			currentPrintedRow++;
			if(currentPrintedRow == rowCount) {
				AfterPrintRows(g);
				PrintFooterPanel(graph);
				PrintFilterInfo(graph);
			}
		}
		public override int GetDetailCount() {
			if(rowCount < 0) { PreparePrintRows(out rowCount, out g); currentPrintedRow = 0; }
			return rowCount;
		}
		public override void PrintRows(DevExpress.XtraPrinting.IBrickGraphics graph) {
			try {
				PreparePrintRows(out rowCount, out g);
				for(int n = 0; n < rowCount; n++) {
					if(CancelPending) break;
					PrintRowCore(graph, g, n);
				}
			} finally {
				AfterPrintRows(g);
			}
			if(CancelPending) return;
			PrintFooterPanel(graph);
			PrintFilterInfo(graph);
		}
		private void AfterPrintRows(Graphics g) {
			ViewViewInfo.GInfo.ReleaseGraphics();
			ViewViewInfo.GInfo.Cache.Paint = DevExpress.Utils.Paint.XPaint.Graphics;
			PrintWrapper.EndPrint();
			g.Dispose();
		}
		BorderSide CalcBrickBorders() {
			BorderSide border = BorderSide.None;
			if(View.OptionsPrint.PrintHorzLines) border |= BorderSide.Top | BorderSide.Bottom;
			if(View.OptionsPrint.PrintVertLines) border |= BorderSide.Left | BorderSide.Right;
			return border;
		}
		void PreparePrintRows(out int rowCount, out Graphics g) {
			MakeRowList();
			if(Rows.Count > 0) Rows.Add(GridControl.InvalidRowHandle);
			rowCount = Rows.Count;
			g = null;
			PrintWrapper.BeginPrint();
			g = View.GridControl.CreateGraphics();
			ViewViewInfo.GInfo.AddGraphics(g);
			ViewViewInfo.GInfo.Cache.Paint = new DevExpress.Utils.Paint.XPrintPaint();
			printMergedInfoTable = new Dictionary<GridColumn, PrintMergeInfo>();
		}
		int printedRows = 0;
		void PrintRowCore(DevExpress.XtraPrinting.IBrickGraphics graph, Graphics g, int n) {
			if(rowCount > 0) {
				ReportProgress(Math.Min(100, (int)(((n + 1) / (float)rowCount) * 100f)));
			}
			int rh = (int)Rows[n];
			if(RaiseBeforePrintRow(g, graph, rh, View.GetRowLevel(rh), Indent, Y)) return;
			if(++printedRows % 10 == 0 && !AllowProcessMergedInfo) {
				BrickGraphics graph2 = (BrickGraphics)graph;
				graph2.Modifier = BrickModifier.None;
				graph2.Modifier = BrickModifier.Detail;
				Y = 0;
			}
			PrintRow(g, graph, rh, View.GetRowLevel(rh));
		}
		protected bool RaiseBeforePrintRow(Graphics g, IBrickGraphics graph, int rowHandle, int level, int x, int y) {
			CancelPrintRowEventArgs prea = new CancelPrintRowEventArgs(PS, g, graph, rowHandle, level, x, y, RowHasFooter(level));
			View.RaiseBeforePrintRow(prea);
			this.Y = prea.Y;
			this.Indent = prea.X;
			return prea.Cancel;
		}
		protected void RaiseAfterPrintRow(Graphics g, IBrickGraphics graph, int rowHandle, int level, int x, int y, bool hasRowFooter) {
			CancelPrintRowEventArgs prea = new CancelPrintRowEventArgs(PS, g, graph, rowHandle, level, x, y, hasRowFooter);
			View.RaiseAfterPrintRow(prea);
			this.Y = prea.Y;
			this.Indent = prea.X;
		}
		bool? isSameCellEditor = null;
		bool IsSameCellEditor {
			get {
				if(isSameCellEditor == null) isSameCellEditor = View.IsSameColumnEditor;
				return isSameCellEditor.Value;
			}
		}
		bool IsAllowCacheDataRow = true;
		GridDataRowInfo cachedDataRow = null;
		GridRowInfo CreateRowInfo(int rowHandle, int level) {
			if(rowHandle >= 0 && IsAllowCacheDataRow) {
				if(cachedDataRow != null) {
					cachedDataRow.RowHandle = rowHandle;
					return cachedDataRow;
				}
			}
			GridRowInfo res = ViewViewInfo.CreateRowInfo(new GridRow(rowHandle, 0, level, 0, null, false));
			if(rowHandle >= 0 && IsAllowCacheDataRow) {
				cachedDataRow = res as GridDataRowInfo;
				CreateRowCellInfo(cachedDataRow);
			}
			return res;
		}
		void CreateRowCellInfo(GridDataRowInfo dataRow) {
			foreach(PrintColumnInfo colInfo in Columns) {
				GridCellInfo cell = new GridCellInfo();
				cell.ColumnInfo = View.ViewInfo.CreateColumnInfo(null);
				cell.ColumnInfo.Column = colInfo.Column;
				cell.RowInfo = dataRow;
				UpdateCellEditorInfo(cell, dataRow.RowHandle);
				dataRow.Cells.Add(cell);
			}
		}
		void UpdateCellEditorInfo(GridDataRowInfo dataRow) {
			for(int n = 0; n < dataRow.Cells.Count; n++) {
				UpdateCellEditorInfo(dataRow.Cells[n], dataRow.RowHandle);
			}
		}
		void UpdateCellEditorInfo(GridCellInfo cell, int rowHandle) {
			RepositoryItem item = View.GetRowCellRepositoryItem(rowHandle, cell.Column);
			if(cell.Editor == null || cell.Editor != item) {
				cell.Editor = item;
				cell.ViewInfo = null;
			}
			if(cell.ViewInfo == null) {
				cell.ViewInfo = item.CreateViewInfo();
				if(!cell.Column.DisplayFormat.IsEmpty) {
					cell.ViewInfo.Format = cell.Column.DisplayFormat;
				}
				cell.ViewInfo.Editable = View.Editable && cell.Column.OptionsColumn.AllowEdit;
			}
		}
		protected virtual void PrintRow(Graphics g, IBrickGraphics graph, int rowHandle, int level) {
			if(prevLevel != -1) {
				bool hasRowFooter = RowHasFooter(level);
				if(hasRowFooter) PrintRowFooter(prevRowHandle, rowHandle);
				RaiseAfterPrintRow(g, graph, prevRowHandle, View.GetRowLevel(prevRowHandle), Indent, Y, hasRowFooter);
			}
			if(rowHandle == GridControl.InvalidRowHandle) return;
			bool isGroup = View.IsGroupRow(rowHandle);
			currentRowHeight = CalcRowHeight(g, rowHandle, isGroup);
			currentRowInfo = CreateRowInfo(rowHandle, level);
			prevLevel = level;
			if(isGroup) {
				printMergedInfoTable.Clear();
				PrintGroupRow(rowHandle, level);
			} else {
				PrintDataRow(rowHandle, level);
				PrintMasterDataRow(rowHandle, level);
			}
			prevRowHandle = rowHandle;
		}
		private bool RowHasFooter(int level) {
			bool hasRowFooter = prevLevel > level ||
				(View.OptionsView.GroupFooterShowMode == GroupFooterShowMode.VisibleAlways &&
				prevLevel == level && View.IsGroupRow(prevRowHandle));
			return hasRowFooter;
		}
		object dataRowHeight = null, groupRowHeight;
		protected int CalcDefaultRowHeight(bool dataRow) {
			if(dataRow) {
				if(dataRowHeight == null) {
					AppearanceObject[] apps = new AppearanceObject[] { AppearancePrint.Row, AppearancePrint.OddRow, AppearancePrint.EvenRow };
					dataRowHeight = ViewViewInfo.CalcMinEditorHeight(apps);
				}
				return (int)dataRowHeight;
			}
			if(groupRowHeight == null) {
				groupRowHeight = AppearancePrint.GroupRow.CalcDefaultTextSize(g).Height;
			}
			return (int)groupRowHeight;
		}
		protected virtual int CalcRowHeight(Graphics g, int rowHandle, bool isGroup) {
			int lineHeight = View.OptionsPrint.PrintHorzLines ? 2 : 0;
			if(isGroup) {
				return ViewViewInfo.CalcRowHeight(g, rowHandle, CalcDefaultRowHeight(false) + 2 + lineHeight, -1, false, ColumnsInfo);
			}
			return ViewViewInfo.CalcRowHeight(g, rowHandle, CalcDefaultRowHeight(true), -1, false, ColumnsInfo) + lineHeight;
		}
		protected virtual void PrintMasterDataRow(int rowHandle, int level) {
			if(!View.OptionsPrint.PrintDetails) return;
			if(!View.IsMasterRow(rowHandle)) return;
			bool expanded = View.GetMasterRowExpanded(rowHandle);
			bool printAll = View.OptionsPrint.ExpandAllDetails;
			if(!expanded && !printAll) return;
			if(expanded) {
				PrintDetail(View.GetVisibleDetailView(rowHandle), level);
				return;
			}
			GridDetailInfo[] detailInfos = View.GetDetailInfo(rowHandle, true);
			for(int n = 0; detailInfos != null && n < detailInfos.Length; n++) {
				if(!View.CanExpandMasterRowEx(rowHandle, detailInfos[n].RelationIndex)) continue;
				BaseView defaultView;
				string levelName;
				BaseView newGV = View.CreateLevelDefault(rowHandle, detailInfos[n].RelationIndex, out defaultView, out levelName);
				if(newGV.IsSupportPrinting) {
					newGV.BeginUpdate();
					MasterRowInfo mi = new MasterRowInfo(View.DataController, rowHandle, null);
					DetailInfo di = mi.CreateDetail(View.DataController.GetDetailList(rowHandle, detailInfos[n].RelationIndex), detailInfos[n].RelationIndex);
					View.ApplyLevelDefaults(newGV, defaultView, di, levelName);
					newGV.ParentView = null;
					newGV.ParentView = View;
					PrintDetail(newGV, level);
				}
				newGV.ParentView = null;
				newGV.Dispose();
			}
		}
		protected virtual void PrintDetail(BaseView view, int rowLevel) {
			if(!view.IsSupportPrinting) return;
			PrintWrapper.PrintDetail(view, rowLevel, ViewViewInfo.LevelIndent, View.DetailVerticalIndent);
			Y = View.DetailVerticalIndent;
		}
		protected virtual int RowLineCount { get { return 1; } }
		protected virtual void PrintDataRow(int rowHandle, int level) {
			GridDataRowInfo dataRow = this.currentRowInfo as GridDataRowInfo;
			if(dataRow.Cells.Count == 0) {
				CreateRowCellInfo(dataRow);
			} else {
				if(!IsSameCellEditor) UpdateCellEditorInfo(dataRow);
			}
			ViewViewInfo.UpdateRowData(currentRowInfo, false, true);
			ViewViewInfo.UpdateBeforePaint(this.currentRowInfo);
			GridRowCellState rowState = ViewViewInfo.CalcRowState(dataRow);
			if(View.OptionsPrint.UsePrintStyles)
				dataRow.BaseAppearance = ((rowState & GridRowCellState.Even) != 0) ? AppearancePrint.EvenRow : AppearancePrint.OddRow;
			Point indent = new Point(Indent, Y);
			int lastRight = 0;
			for(int n = 0; n < Columns.Count; n++) {
				PrintColumnInfo colInfo = Columns[n] as PrintColumnInfo;
				dataRow.Cells[n].State = rowState & ~(GridRowCellState.Focused | GridRowCellState.Selected | GridRowCellState.FocusedCell);
				lastRight = PreparePrintRowCell(colInfo, indent, rowHandle, dataRow.Cells[n], level, lastRight);
			}
			Y += RowLineCount * this.currentRowHeight;
			PrintDataRowPreview(rowHandle, level);
		}
		protected virtual int PreparePrintRowCell(PrintColumnInfo colInfo, Point indent, int rowHandle, GridCellInfo cell, int level, int lastRight) {
			Rectangle r = colInfo.Bounds;
			r.Offset(indent);
			r.Height = this.CurrentRowHeight;
			int levelIndent = level * ViewViewInfo.LevelIndent;
			if(colInfo.Column.VisibleIndex == 0) {
				r.X += levelIndent;
				r.Width -= levelIndent;
			}
			if(r.X < indent.X + levelIndent) {
				int delta = indent.X + levelIndent - r.X;
				r.X += delta;
				r.Width -= delta;
			}
			if(r.Width > 0 && r.Right > indent.X + levelIndent) {
				PrintRowCell(rowHandle, cell, r);
			}
			return r.Right;
		}
		protected virtual void PrintDataRowPreview(int rowHandle, int level) {
			if(!View.OptionsPrint.PrintPreview) return;
			GridDataRowInfo dataRow = this.currentRowInfo as GridDataRowInfo;
			if(dataRow.PreviewText == null || dataRow.PreviewText.Length == 0) return;
			RectangleF r = Rectangle.Empty;
			r.Y = Y;
			r.X = Indent + level * ViewViewInfo.LevelIndent;
			r.Width = this.fMaxRowWidth - r.X;
			GInfo.AddGraphics(null);
			try {
				r.Height = (int)AppearancePrint.Preview.CalcTextSize(GInfo.Cache, dataRow.PreviewText, (int)r.Width - 2 - 3 - ViewViewInfo.PreviewIndent).Height + 3 + 2;
			} finally {
				GInfo.ReleaseGraphics();
			}
			Y += (int)r.Height;
			SetDefaultBrickStyle(Graph, Bricks["Preview"]);
			IPanelBrick brick = (IPanelBrick)DrawBrick(Graph, "PanelBrick", r);
			IList col = brick.Bricks;
			UsePanelBrickDisableDrawing = true;
			RectangleF panelRect = r;
			if(View.OptionsPrint.SplitCellPreviewAcrossPages) brick.Separable = true;
			r.Inflate(-1, -1);
			r.X += ViewViewInfo.PreviewIndent;
			r.Width -= ViewViewInfo.PreviewIndent;
			r.X -= panelRect.X;
			r.Y -= panelRect.Y;
			SetDefaultBrickStyle(Graph, Bricks["PreviewText"]);
			ITextBrick textBrick = DrawTextBrick(Graph, dataRow.PreviewText, r, true);
			col.Add(textBrick);
			if(View.OptionsPrint.SplitCellPreviewAcrossPages) textBrick.Separable = true;
			UsePanelBrickDisableDrawing = false;
		}
		CellMergeEventArgs mergeArgs = new CellMergeEventArgs(0, 0, null);
		protected int RaiseMergeEvent(int row1, int row2, GridColumn column) {
			GridColumnInfoArgs columnInfo = ColumnsInfo[column];
			mergeArgs.Setup(row1, row2, columnInfo == null ? null : columnInfo.Column);
			View.RaiseCellMerge(mergeArgs);
			return this.mergeArgs.Handled ? (this.mergeArgs.Merge ? 0 : -1) : 3;
		}
		Dictionary<GridColumn, PrintMergeInfo> printMergedInfoTable;
		int mergeValueProvider = 0;
		class PrintMergeInfo {
			public PrintMergeInfo(Brick brick, int rowHandle) {
				brickCore = brick;
				rowHandleCore = rowHandle;
			}
			private Brick brickCore;
			public Brick Brick {
				get { return brickCore; }
				set { brickCore = value; }
			}
			private int rowHandleCore;
			public int RowHandle {
				get { return rowHandleCore; }
				set { rowHandleCore = value; }
			}
		}
		protected object GetTextValue(VisualBrick brick) {
			PanelBrick pb = brick as PanelBrick;
			IRichTextBrick irtb = brick as IRichTextBrick;
			XECheckBoxBrick xectb = brick as XECheckBoxBrick;
			if(xectb != null) return xectb.Value;
			if(irtb != null) return irtb.RtfText;
			if(pb != null) return pb.Value; else return brick.TextValue;
		}
		protected void UpdateMergedStatus(GridCellInfo cell, VisualBrick brick) {
			if(printMergedInfoTable.ContainsKey(cell.Column)) {
				VisualBrick savedBrick = (VisualBrick)printMergedInfoTable[cell.Column].Brick;
				bool canMerge = object.Equals(GetTextValue(brick), GetTextValue(savedBrick)) && (savedBrick.Rect.Height < View.OptionsPrint.MaxMergedCellHeight || View.OptionsPrint.MaxMergedCellHeight <= 0) && cell.Column.GetAllowMerge();
				if(cell.Column.GetAllowMerge()) {
					int eventCanMerge = RaiseMergeEvent(printMergedInfoTable[cell.Column].RowHandle, cell.RowHandle, cell.Column);
					if(eventCanMerge < 0) canMerge = false;
					if(eventCanMerge == 0) canMerge = true;
				}
				if(canMerge) {
					object result = null;
					savedBrick.TryGetAttachedValue(DevExpress.XtraPrinting.Native.BrickAttachedProperties.MergeValue, out result);
					if(result == null) {
						mergeValueProvider++;
						savedBrick.SetAttachedValue(DevExpress.XtraPrinting.Native.BrickAttachedProperties.MergeValue, mergeValueProvider);
						brick.SetAttachedValue(DevExpress.XtraPrinting.Native.BrickAttachedProperties.MergeValue, mergeValueProvider);
					} else {
						brick.SetAttachedValue(DevExpress.XtraPrinting.Native.BrickAttachedProperties.MergeValue, result);
					}
				} else {
					printMergedInfoTable[cell.Column] = new PrintMergeInfo(brick, cell.RowHandle);
				}
			} else {
				printMergedInfoTable.Add(cell.Column, new PrintMergeInfo(brick, cell.RowHandle));
			}
		}
		protected virtual void PrintRowCell(int rowHandle, GridCellInfo cell, Rectangle r) {
			ViewViewInfo.UpdateCellAppearanceCore(cell);
			string displayText = View.GetRowCellDisplayTextCore(rowHandle, cell.Column, cell.ViewInfo, cell.CellValue, false);
			if(cell.ViewInfo.AllowHtmlString) {
				displayText = StringPainter.Default.RemoveFormat(displayText, true);
			}
			PrintCellHelperInfo info = new PrintCellHelperInfo(new Point(cell.Column == null ? -1 : cell.Column.AbsoluteIndex, rowHandle),
				LineColor,
				PS,
				cell.CellValue,
				cell.Appearance,
				displayText,
				r,
				Graph,
				View.GetRowCellDefaultAlignment(rowHandle, cell.Column, cell.Appearance.HAlignment),
				View.OptionsPrint.PrintHorzLines,
				View.OptionsPrint.PrintVertLines,
				cell.ColumnInfo.Column.DisplayFormat.FormatString,
				CalcBrickBorders()
				);
			IVisualBrick brick = cell.Editor.GetBrick(info);
			if(AllowProcessMergedInfo) {
				brick.Rect = r;
				UpdateMergedStatus(cell, (VisualBrick)brick);
			}
			Graph.DrawBrick(brick, r);
		}
		protected virtual void PrintRowCellOld(int rowHandle, GridCellInfo cell, Rectangle r) {
			ViewViewInfo.UpdateCellAppearance(cell);
			DevExpress.XtraEditors.Repository.RepositoryItem ritem = View.GetRowCellRepositoryItem(rowHandle, cell.ColumnInfo.Column);
			object cellvalue = View.GetRowCellValue(rowHandle, cell.ColumnInfo.Column);
			PrintCellHelperInfo info = new PrintCellHelperInfo(
				LineColor,
				PS,
				cellvalue,
				cell.Appearance,
				View.GetRowCellDisplayText(rowHandle, cell.ColumnInfo.Column),
				r,
				Graph,
				View.GetRowCellDefaultAlignment(rowHandle, cell.Column, cell.Appearance.HAlignment),
				View.OptionsPrint.PrintHorzLines,
				View.OptionsPrint.PrintVertLines,
				cell.ColumnInfo.Column.DisplayFormat.FormatString
				);
			IVisualBrick brick = ritem.GetBrick(info);
			if(AllowProcessMergedInfo) {
				brick.Rect = r;
				UpdateMergedStatus(cell, (VisualBrick)brick);
			}
			brick.Sides = CalcBrickBorders();
			Graph.DrawBrick(brick, r);
		}
		protected virtual bool AllowProcessMergedInfo {
			get { return View.OptionsView.AllowCellMerge; }
		}
		protected virtual void PrintGroupRow(int rowHandle, int level) {
			Rectangle r = Rectangle.Empty;
			r.X = Indent + level * ViewViewInfo.LevelIndent;
			r.Width = this.fMaxRowWidth - r.Left;
			r.Y = Y;
			r.Height = currentRowHeight;
			if(View.OptionsPrint.UsePrintStyles) SetDefaultBrickStyle(Graph, Bricks["GroupRow"]);
			else {
				ViewViewInfo.UpdateBeforePaint(this.currentRowInfo);
				AppearanceObject rowAppearance = currentRowInfo.Appearance;
				BrickStyle tempStyle = new BrickStyle(Bricks["GroupRow"].Sides, Bricks["GroupRow"].BorderWidth, rowAppearance.BorderColor, rowAppearance.BackColor, rowAppearance.ForeColor, rowAppearance.Font, Bricks["GroupRow"].StringFormat);
				tempStyle.TextAlignment = DevExpress.XtraPrinting.Native.TextAlignmentConverter.ToTextAlignment(rowAppearance.TextOptions.HAlignment, rowAppearance.TextOptions.VAlignment);
				SetDefaultBrickStyle(Graph, tempStyle);
			}
			ITextBrick textBrick = DrawTextBrick(Graph, View.GetGroupRowDisplayText(rowHandle), r, true);
			textBrick.Text = textBrick.Text.Replace(Environment.NewLine, "");
			textBrick.TextValue = View.GetGroupRowPrintValue(rowHandle);
			Y += r.Height;
		}
		protected virtual void PrintRowFooter(int prevRowHandle, int rowHandle) {
			if(!View.OptionsPrint.PrintGroupFooter) return;
			bool isExpanded = View.OptionsPrint.ExpandAllGroups ? true : View.GetRowExpanded(prevRowHandle);
			int fcount = ViewViewInfo.GetRowFooterCount(prevRowHandle, rowHandle, isExpanded);
			if(fcount == 0) return;
			bool showCurrentFooter = ViewViewInfo.IsShowCurrentRowFooter(prevRowHandle, rowHandle);
			int level = View.GetRowLevel(prevRowHandle);
			if(!View.IsGroupRow(prevRowHandle) || !showCurrentFooter) prevRowHandle = View.GetParentRowHandle(prevRowHandle);
			if(!showCurrentFooter)
				level--;
			PrintRowFooterCore(prevRowHandle, level, fcount);
		}
		protected virtual void PrintRowFooterCore(int prevRowHandle, int level, int fcount) {
			int x = 0;
			Rectangle r = Rectangle.Empty;
			for(int n = 0; n < fcount; n++) {
				int parentHandle = View.GetParentRowHandle(prevRowHandle);
				if(View.OptionsPrint.ExpandAllGroups || View.GetRowExpanded(prevRowHandle) || View.OptionsView.GroupFooterShowMode == GroupFooterShowMode.VisibleAlways) {
					r.X = Indent + level * ViewViewInfo.LevelIndent;
					r.Width = this.fMaxRowWidth - r.Left;
					r.Y = Y;
					r.Height = this.groupFooterHeight * RowLineCount + 2;
					SetDefaultBrickStyle(Graph, Bricks["GroupFooter"]);
					IPanelBrick brick = (IPanelBrick)DrawBrick(Graph, "PanelBrick", r);
					brick.Sides = CalcBrickBorders();
					IList col = brick.Bricks;
					UsePanelBrickDisableDrawing = true;
					ITextBrick textBrick;
					Rectangle panelRect = r;
					brick.SeparableHorz = true;
					foreach(PrintColumnInfo colInfo in Columns) {
						if(View.IsShowRowFooterCell(prevRowHandle, colInfo.Column)) {
							x = colInfo.Bounds.X + Indent;
							if(x < panelRect.X) {
								r.Width = colInfo.Bounds.Width - (panelRect.X - x);
								r.X = panelRect.X;
							} else {
								r.X = x;
								r.Width = colInfo.Bounds.Width;
							}
							r.Y = colInfo.RowIndex * this.groupFooterHeight + 1 + Y;
							r.Height = colInfo.RowCount * this.groupFooterHeight;
							r = new Rectangle(r.X - panelRect.X, r.Y - panelRect.Y, r.Width, r.Height);
							string text = StringPainter.Default.RemoveFormat(View.GetRowFooterCellText(prevRowHandle, colInfo.Column), true);
							textBrick = DrawTextBrick(Graph, text, r);
							DictionaryEntry de = View.GetRowSummaryItem(prevRowHandle, colInfo.Column);
							GridGroupSummaryItem gsi = de.Key as GridGroupSummaryItem;
							if(gsi != null) {
								textBrick.TextValue = de.Value;
								textBrick.TextValueFormatString = gsi.DisplayFormat;
							}
							textBrick.Sides = CalcBrickBorders();
							col.Add(textBrick);
						}
					}
					UsePanelBrickDisableDrawing = false;
					Hashtable hash1 = View.GetGroupSummaryValues(prevRowHandle);
					Hashtable hash2 = View.GetGroupSummaryValues(View.GetParentRowHandle(prevRowHandle));
					Y += this.groupFooterHeight * RowLineCount + 2;
				}
				prevRowHandle = View.GetParentRowHandle(prevRowHandle);
				level--;
			}
		}
		protected override void MakeRowList() {
			if(PrintSelectedRowsOnly) {
				MakeSelectedRowList();
				return;
			}
			VisibleIndexCollection indexes = new VisibleIndexCollection(View.DataController, View.DataController.GroupInfo);
			indexes.BuildVisibleIndexes(View.DataController.VisibleListSourceRowCount, true, View.OptionsPrint.ExpandAllGroups);
			Rows.AddRange(indexes);
		}
		public override void PrintFooter(DevExpress.XtraPrinting.IBrickGraphics graph) {
		}
		const int FooterCellIndent = 0;
		public virtual void PrintFooterPanel(DevExpress.XtraPrinting.IBrickGraphics graph) {
			if(!View.OptionsPrint.PrintFooter) return;
			int maxFooterCount = 0;
			foreach(PrintColumnInfo colInfo in Columns) {
				if(!colInfo.Column.HasSummary) continue;
				maxFooterCount = Math.Max(colInfo.Column.Summary.ActiveCount, maxFooterCount);
			}
			if(maxFooterCount == 0) return;
			Rectangle r = Rectangle.Empty;
			int footerHeight = this.footerPanelHeight * RowLineCount * maxFooterCount + FooterCellIndent * (maxFooterCount) + RowLineCount * FooterCellIndent;
			r.Y = Y;
			r.X = Indent;
			r.Height = footerHeight;
			r.Width = this.fMaxRowWidth;
			SetDefaultBrickStyle(graph, Bricks["FooterPanel"]);
			IPanelBrick brick = (IPanelBrick)DrawBrick(graph, "PanelBrick", r);
			ITextBrick textBrick;
			brick.SeparableHorz = true;
			IList col = brick.Bricks;
			UsePanelBrickDisableDrawing = true;
			BrickStyle cellStyle = Bricks["FooterPanel"].Clone() as BrickStyle;
			cellStyle.Padding = new PaddingInfo(2, 2, 0, 0);
			graph.DefaultBrickStyle = cellStyle;
			foreach(PrintColumnInfo colInfo in Columns) {
				if(!colInfo.Column.HasSummary) continue;
				for(int n = 0; n < colInfo.Column.Summary.ActiveCount; n++) {
					r.X = colInfo.Bounds.X + Indent;
					r.Y = colInfo.RowIndex * this.footerPanelHeight * maxFooterCount + FooterCellIndent * (n + 1) + this.footerPanelHeight * n +
						colInfo.RowIndex * (FooterCellIndent * maxFooterCount);
					r.Width = colInfo.Bounds.Width;
					r.Height = this.footerPanelHeight * colInfo.RowCount;
					r.X -= Indent;
					GridSummaryItem item = colInfo.Column.Summary.GetActiveItem(n);
					string text = item.GetDisplayText(item.SummaryValue, false);
					text = StringPainter.Default.RemoveFormat(text, true);
					textBrick = DrawTextBrick(graph, text, r);
					textBrick.TextValue = item.SummaryValue;
					textBrick.TextValueFormatString = item.DisplayFormat;
					col.Add(textBrick);
				}
			}
			Y += footerHeight;
			UsePanelBrickDisableDrawing = false;
		}
		Hashtable widthCache = new Hashtable();
		protected Hashtable WidthCache { get { return widthCache; } }
		protected virtual void UpdateWidthCache(bool push) {
			if(push) WidthCache.Clear();
			foreach(GridColumn column in View.VisibleColumns) {
				UpdateWidthCache(column, push);
			}
		}
		protected virtual void UpdateWidthCache(object obj, bool push) {
			GridColumn column = obj as GridColumn;
			if(column == null) return;
			if(push) {
				WidthCache[column] = new Point(column.Width, column.VisibleWidth);
			} else {
				if(!WidthCache.ContainsKey(column)) return;
				Point p = (Point)WidthCache[column];
				column.width = p.X;
				column.visibleWidth = p.Y;
			}
		}
		protected virtual void CreateInfo() {
			if(Graph == null) return;
			View.RefreshVisibleColumnsList();
			View.ViewInfo.RecalcColumnWidthes();
			this.groupFooterHeight = CalcStyleHeight(AppearancePrint.GroupFooter) + 4;
			this.footerPanelHeight = Math.Max(View.FooterPanelHeight, CalcStyleHeight(AppearancePrint.FooterPanel) + 4);
			UpdateWidthCache(true);
			try {
				CalcAutoWidth(Graph);
				CreateColumns();
			} finally {
				UpdateWidthCache(false);
			}
		}
		protected void CalcAutoWidth(IBrickGraphics graph) {
			if(!View.OptionsPrint.AutoWidth) return;
			ViewViewInfo.RecalcColumnWidthes(new GridAutoWidthCalculatorArgs(null, true, MaximumWidth - 1) { VisibleColumns = GetPrintColumnCollection() });
		}
		protected virtual void CreateColumns() {
			this.headerRowHeight = Math.Max(CalcStyleHeight(AppearancePrint.HeaderPanel), View.ColumnPanelRowHeight) + 4;
			int row = 0;
			this.columnsInfo = ViewViewInfo.CreateVisibleColumnsInfo(ref row);
			CreatePrintColumnCollection();
		}
		protected virtual List<GridColumn> GetPrintColumnCollection() {
			List<GridColumn> res = new List<GridColumn>();
			foreach(GridColumn col in View.VisibleColumns) {
				if(View.CheckboxSelectorColumn == col && !View.IsShowCheckboxSelectorInPrintExport) continue;
				if(col.OptionsColumn.Printable == DefaultBoolean.False) continue;
				res.Add(col);
			}
			return res;
		}
		protected virtual void CreatePrintColumnCollection() {
			this.fMaxRowWidth = 0;
			foreach(GridColumn col in GetPrintColumnCollection()) {
				PrintColumnInfo colInfo = new PrintColumnInfo();
				colInfo.Bounds = new Rectangle(this.fMaxRowWidth, 0, col.VisibleWidth, headerRowHeight);
				colInfo.Column = col;
				Columns.Add(colInfo);
				this.fMaxRowWidth += colInfo.Bounds.Width;
			}
		}
		protected override int MaxWidth { get { return fMaxRowWidth; } }
		protected override bool AllowPrintFilterInfo { get { return View.OptionsPrint.PrintFilterInfo; } }
		protected override Color LineColor { get { return AppearancePrint.Lines.BackColor; } }
		public override void Initialize() {
			base.Initialize();
			Bricks.Add("GroupRow", AppearancePrint.GroupRow, BorderSide.All, LineColor, 1);
			Bricks.Add("GroupFooter", AppearancePrint.GroupFooter, BorderSide.All, LineColor, 1);
			Bricks.Add("FooterPanel", AppearancePrint.FooterPanel, BorderSide.All, AppearancePrint.FooterPanel.BorderColor, 1);
			Bricks.Add("HeaderPanel", AppearancePrint.HeaderPanel, BorderSide.All, AppearancePrint.HeaderPanel.BorderColor, 1);
			BorderSide cell = BorderSide.None;
			if(View.OptionsPrint.PrintHorzLines) cell |= BorderSide.Bottom | BorderSide.Top;
			if(View.OptionsPrint.PrintVertLines) cell |= BorderSide.Left | BorderSide.Right;
			Bricks.Add("Preview", AppearancePrint.Preview, cell, LineColor, 1);
			Bricks.Add("PreviewText", AppearancePrint.Preview);
			ViewViewInfo.CalcConstants();
			CreateInfo();
		}
		protected override void UpdateAppearances() {
			base.UpdateAppearances();
			if(!View.OptionsPrint.UsePrintStyles) {
				AppearanceDefaultInfo[] info = View.BaseInfo.GetDefaultPrintAppearance();
				AppearancePrint.Combine(ViewViewInfo.PaintAppearance, info);
				if(View.IsSkinned) {
					AppearancePrint.HeaderPanel.Assign(Find("HeaderPanel", info));
					AppearancePrint.FilterPanel.Assign(Find("FilterPanel", info));
					AppearancePrint.GroupFooter.Assign(Find("GroupFooter", info));
					AppearancePrint.GroupRow.Assign(Find("GroupRow", info));
					AppearancePrint.FooterPanel.Assign(Find("FooterPanel", info));
				} else {
					AppearancePrint.Lines.BackColor = AppearancePrint.Lines.ForeColor = ViewViewInfo.PaintAppearance.HorzLine.BackColor;
				}
				if(!View.OptionsView.EnableAppearanceEvenRow) AppearancePrint.EvenRow.Assign(AppearancePrint.Row);
				if(!View.OptionsView.EnableAppearanceOddRow) AppearancePrint.OddRow.Assign(AppearancePrint.Row);
			} else {
				if(!View.OptionsPrint.EnableAppearanceEvenRow) AppearancePrint.EvenRow.Assign(AppearancePrint.Row);
				if(!View.OptionsPrint.EnableAppearanceOddRow) AppearancePrint.OddRow.Assign(AppearancePrint.Row);
			}
			AppearanceHelper.Combine(AppearancePrint.EvenRow, new AppearanceObject[] { AppearancePrint.EvenRow.Clone() as AppearanceObject, AppearancePrint.Row });
			AppearanceHelper.Combine(AppearancePrint.OddRow, new AppearanceObject[] { AppearancePrint.OddRow.Clone() as AppearanceObject, AppearancePrint.Row });
		}
	}
	public class PrintColumnInfo {
		public GridColumn Column;
		public Rectangle Bounds;
		public int RowCount, RowIndex;
		public PrintColumnInfo(GridColumn column) {
			this.Column = column;
			this.Bounds = Rectangle.Empty;
			this.RowCount = 1;
			this.RowIndex = 0;
		}
		public PrintColumnInfo()
			: this(null) {
		}
	}
	class SummaryItemExportImplementer :ISummaryItemEx {
		GridView gridView;
		GridSummaryItem gridSummaryItem;
		public SummaryItemExportImplementer(GridSummaryItem item, string columnCaption, GridView gridView) {
			this.gridView = gridView;
			this.gridSummaryItem = item;
			ShowInColumnFooterName = columnCaption;
			DisplayFormat = gridSummaryItem.DisplayFormat;
			FieldName = gridSummaryItem.FieldName;
			SummaryType = gridSummaryItem.SummaryType;
			SummaryValue = gridSummaryItem.SummaryValue;
		}
		public string DisplayFormat { get; set; }
		public string FieldName { get; private set; }
		public SummaryItemType SummaryType { get; private set; }
		public object SummaryValue { get; private set; }
		public string ShowInColumnFooterName { get; private set; }
		public object GetSummaryValueByGroupId(int groupId) {
			return gridView.GetGroupSummaryValue(groupId, (GridGroupSummaryItem)gridSummaryItem);
		}
	}
	public class ExcelFormattingConverter {
		const string DefaultFontName = "Tahoma";
		const float DefaultFontSize = 8.25f;
		const string DefaultXlFontName = "Calibri";
		const float DefaultXlFontSize = 11;
		public static XlDifferentialFormatting ConvertConditionAppearance(AppearanceObject appearance) {
			XlDifferentialFormatting result = new XlDifferentialFormatting();
			result.Fill = new XlFill() {
				BackColor = appearance.GetBackColor() == Color.Empty ? Color.White : appearance.GetBackColor(),
				ForeColor = appearance.GetForeColor(),
				PatternType = XlPatternType.Solid,
			};
			result.Font = Convert(appearance.GetFont());
			return result;
		}
		public static XlFont Convert(Font font) {
			XlFont result = new XlFont();
			result.Bold = font.Bold;
			result.StrikeThrough = font.Strikeout;
			result.Underline = font.Underline ? XlUnderlineType.Single : XlUnderlineType.None;
			result.Italic = font.Italic;
			result.FontFamily = XlFontFamily.Swiss;
			result.Name = GetFontName(font);
			result.Size = GetFontSize(font);
			GetShemeStyle(result);
			return result;
		}
		static void GetShemeStyle(XlFont font) {
			font.SchemeStyle = font.Name == DefaultXlFontName ? XlFontSchemeStyles.Minor : XlFontSchemeStyles.None;
		}
		static float GetFontSize(Font font) {
			return Math.Abs(font.Size - DefaultFontSize) < 0.01 ? DefaultXlFontSize : font.Size;
		}
		static string GetFontName(Font font) {
			return font.Name == DefaultFontName ? DefaultXlFontName : font.Name;
		}
		static XlVerticalAlignment Convert(VertAlignment valgm) {
			switch(valgm) {
				case VertAlignment.Top:
					return XlVerticalAlignment.Top;
				case VertAlignment.Center:
					return XlVerticalAlignment.Center;
				case VertAlignment.Bottom:
					return XlVerticalAlignment.Bottom;
			}
			return XlVerticalAlignment.Center;
		}
		static XlHorizontalAlignment Convert(HorzAlignment halgm) {
			switch(halgm) {
				case HorzAlignment.Center:
					return XlHorizontalAlignment.Center;
				case HorzAlignment.Near:
					return XlHorizontalAlignment.Left;
				case HorzAlignment.Far:
					return XlHorizontalAlignment.Right;
				default:
					return XlHorizontalAlignment.General;
			}
		}
		public static XlCellFormatting Convert(AppearanceObject appearance) {
			XlCellFormatting result = new XlCellFormatting();
			GetFill(appearance, result);
			result.Alignment = new XlCellAlignment {
				WrapText = appearance.TextOptions.WordWrap.HasFlag(WordWrap.Wrap),
				VerticalAlignment = Convert(appearance.TextOptions.VAlignment),
				HorizontalAlignment = Convert(appearance.TextOptions.HAlignment)
			};
			GetBorders(appearance, result);
			result.Font = Convert(appearance.Font);
			result.Font.Color = GetFontColor(appearance);
			return result;
		}
		static void GetBorders(AppearanceObject appearance, XlCellFormatting result) {
			Color borderColor = appearance.GetBorderColor();
			if(!Equals(borderColor, Color.Empty)) {
				result.Border = new XlBorder {
					BottomLineStyle = XlBorderLineStyle.Thin,
					BottomColor = borderColor,
					LeftLineStyle = XlBorderLineStyle.Thin,
					LeftColor = borderColor,
					RightLineStyle = XlBorderLineStyle.Thin,
					RightColor = borderColor,
				};
			}
		}
		static void GetFill(AppearanceObject appearance, XlCellFormatting result) {
			var backColor = appearance.GetBackColor();
			if(!Equals(backColor, Color.Empty)) {
				result.Fill = new XlFill {
					BackColor = appearance.GetBackColor(),
					ForeColor = appearance.GetBackColor(),
					PatternType = XlPatternType.Solid
				};
			}
		}
		static XlColor GetFontColor(AppearanceObject appearance) {
			XlColor color = appearance.GetForeColor();
			return Equals(color, XlColor.Empty) ? XlColor.FromTheme(XlThemeColor.Dark1, 0.0) : color;
		}
		static Color GetBorderColor(AppearanceObject appearance) {
			Color color = appearance.GetBorderColor();
			return color == Color.Empty
				? Color.LightGray
				: color;
		}
		public static FormatConditions ConvertFromCondition(FormatConditionEnum item) { return (FormatConditions)(int)item; }
		public static FormatConditionObject Convert(StyleFormatCondition condition) {
			FormatConditionObject cond = new FormatConditionObject();
			cond.Appearance = ConvertConditionAppearance(condition.Appearance);
			cond.Condition = ConvertFromCondition(condition.Condition);
			cond.ApplyToRow = condition.ApplyToRow;
			cond.ColumnName = condition.ColumnName;
			cond.Value1 = condition.Value1;
			cond.Value2 = condition.Value2;
			cond.Expression = condition.Expression;
			return cond;
		}
	}
	public class UnboundInfoWrapper :IUnboundInfo {
		GridColumn column;
		public UnboundInfoWrapper(GridColumn column) {
			this.column = column;
		}
		public string UnboundExpression {
			get { return column.UnboundExpression; }
		}
		public UnboundColumnType UnboundType {
			get { return column.UnboundType; }
		}
	}
	public class ClipboardGridViewImplementor<TCol, TRow> :GridViewImplementer<TCol, TRow>, IClipboardGridView<TCol, TRow>
		where TRow : DataRowImplementer
		where TCol : ColumnImplementer {
		internal bool ShowProgress;
		public ClipboardGridViewImplementor(GridView view) : base(view, ExportTarget.Xls) { }
		public IEnumerable<TCol> GetSelectedColumns() {
			if(viewCore.SelectedRowsCount < 1) {
				return new List<TCol>();
			}
			if(viewCore.OptionsSelection.MultiSelectMode != GridMultiSelectMode.CellSelect || !viewCore.IsMultiSelect)
				return GetColumns(viewCore.Columns, false).OrderBy(e => e.VisibleIndex);
			List<TCol> allColumns = GetColumns(viewCore.Columns, false);
			HashSet<TCol> result = new HashSet<TCol>();
			IEnumerable selectedRows = GetRowsCore();
			foreach(int handle in selectedRows) {
				if(viewCore.IsGroupRow(handle))
					return allColumns.Where(e => e.IsVisible).OrderBy(e => e.VisibleIndex);
				foreach(TCol col in allColumns) {
					if(viewCore.IsCellSelected(handle, col.GridColumn))
						result.Add(col);
				}
				if(result.Count == allColumns.Count)
					return result.OrderBy(e => e.VisibleIndex);
			}
			return result.OrderBy(e => e.VisibleIndex);
		}
		protected override IEnumerable GetRowsCore() {
			ArrayList rows = new ArrayList();
			if(!viewCore.IsMultiSelect) {
				if(viewCore.IsValidRowHandle(viewCore.FocusedRowHandle)) rows.Add(viewCore.FocusedRowHandle);
				return rows;
			}
			if(viewCore.SelectedRowsCount == 0) return rows;
			int[] rowsa = viewCore.DataController.Selection.GetNormalizedSelectedRowsEx();
			rows.AddRange(rowsa);
			return rows;
		}
		public IEnumerable<TRow> GetSelectedRows() {
			IEnumerable rows = null;
			rows = GetRowsCore();
			HashSet<int> _rows = new HashSet<int>(rows.Cast<int>());
			Stack<GroupRowImplementer> rowsStack = new Stack<GroupRowImplementer>();
			int prevRowLevel = 0;
			List<TRow> result = new List<TRow>();
			foreach(int handle in rows) {
				int currentRowLevel = viewCore.GetRowLevel(handle);
				if(prevRowLevel >= currentRowLevel && rowsStack.Count > 0) rowsStack.Pop();
				prevRowLevel = currentRowLevel;
				if(viewCore.IsGroupRow(handle)) {
					int parent = viewCore.DataController.GetParentRowHandle(handle);
					if(parent != GridControl.InvalidRowHandle && !_rows.Add(parent))
						continue;
					var gr = GetGroupRowImplementer(handle);
					if(rowsStack.Count > 0) rowsStack.Peek().AddChild(gr);
					else result.Add(gr as TRow);
					rowsStack.Push(gr);
				} else {
					int parent = viewCore.DataController.GetParentRowHandle(handle);
					if(parent != GridControl.InvalidRowHandle && !_rows.Add(parent))
						continue;
					var dr = new DataRowImplementer(viewCore, handle, viewCore.GetDataSourceRowIndex(handle));
					if(rowsStack.Count > 0) rowsStack.Peek().AddChild(dr);
					else result.Add((TRow)dr);
				}
			}
			return result;
		}
		bool isAnyParentCollapsed(int handle) {
			int parent = viewCore.GetParentRowHandle(handle);
			if(parent == GridDataController.InvalidRow) return false;
			while(parent != GridDataController.InvalidRow) {
				if(!viewCore.GetRowExpanded(parent)) return true;
				parent = viewCore.GetParentRowHandle(parent);
			}
			return false;
		}
		public string GetRowCellDisplayText(TRow row, string columnName) {
			if(viewCore.IsCellSelect && !viewCore.IsCellSelected(row.RowHandle, viewCore.Columns[columnName])) {
				if(viewCore.OptionsClipboard.CopyCollapsedData == DefaultBoolean.True && isAnyParentCollapsed(row.RowHandle)) {
					return viewCore.GetRowCellDisplayText(row.LogicalPosition, columnName);
				}
				return string.Empty;
			}
			return viewCore.GetRowCellDisplayText(row.LogicalPosition, columnName);
		}
		public override object GetRowCellValue(TRow row, TCol col) {
			if(viewCore.IsCellSelect && !viewCore.IsCellSelected(row.RowHandle, col.GridColumn)) {
				if(viewCore.OptionsClipboard.CopyCollapsedData == DefaultBoolean.True && isAnyParentCollapsed(row.RowHandle)) {
					return base.GetRowCellValue(row, col);
				}
				return string.Empty;
			}
			return base.GetRowCellValue(row, col);
		}
		public XlCellFormatting GetCellAppearance(TRow row, TCol col) {
			int rowHandle = row == null ? Int32.MinValue : row.LogicalPosition;
			if(viewCore.ViewInfo == null) return ExcelFormattingConverter.Convert(AppearanceObject.EmptyAppearance);
			if(col == null) {
				return ExcelFormattingConverter.Convert(viewCore.ViewInfo.PaintAppearance.GroupRow);
			}
			if(rowHandle == Int32.MinValue) {
				return ExcelFormattingConverter.Convert(viewCore.ViewInfo.PaintAppearance.HeaderPanel);
			}
			GridDataRowInfo infoRow = new GridDataRowInfo(viewCore.ViewInfo, rowHandle, viewCore.GetRowLevel(rowHandle));
			GridColumnInfoArgs infoColumn = new GridColumnInfoArgs(col.GridColumn);
			if(infoRow == null || infoColumn == null)
				return ExcelFormattingConverter.Convert(AppearanceObject.EmptyAppearance);
			GridCellInfo infoCell = viewCore.ViewInfo.CreateCellInfo(infoRow, infoColumn);
			if(infoCell == null)
				return ExcelFormattingConverter.Convert(AppearanceObject.EmptyAppearance);
			infoCell.State = (rowHandle % 2 == 0) ? GridRowCellState.Odd : GridRowCellState.Even;
			viewCore.ViewInfo.UpdateCellAppearanceCore(infoCell);
			return ExcelFormattingConverter.Convert(infoCell.Appearance);
		}
		public bool CanCopyToClipboard() {
			return true;
		}
		protected override GroupRowImplementer GetGroupRowImplementer(int handle) {
			return new ClipboardGroupRowImplementer(viewCore, handle);
		}
		public int GetSelectedCellsCount() {
			int count = 0;
			if(viewCore.IsMultiSelect) {
				count += viewCore.DataController.Selection.GetSelectionInfo().SelectedCount;
				if(viewCore.OptionsClipboard.CopyCollapsedData == DefaultBoolean.True && viewCore.DataController.Selection.GetSelectionInfo().SelectedGroupRows > 0) {
					int[] handles = viewCore.DataController.Selection.GetSelectedRows();
					foreach(int handle in handles) {
						if(IsGroupRow(handle)) {
							count += viewCore.GetChildRowCount(handle);
						}
					}
				}
				count *= GetSelectedColumns().Count();
			} else {
				count = GetSelectedColumns().Count();
			}
			return count;
		}
		public bool UseHierarchyIndent(TRow row, TCol col) {
			return false;
		}
		public void ProgressBarCallBack(int progress) {
			if(!ShowProgress) return;
			viewCore.ProgressWindow.SetProgress(progress);
		}
	}
	public class SparklineInfo : ISparklineInfo {
		RepositoryItemSparklineEdit edit;
		public SparklineInfo(RepositoryItemSparklineEdit edit){
			this.edit = edit;
		}
		public ColumnSortOrder PointSortOrder { get { return edit.PointSortOrder; } }
		public XlSparklineType SparklineType{
			get {
				switch(edit.View.Type){
					case Sparkline.SparklineViewType.Line:
						return XlSparklineType.Line;
					case Sparkline.SparklineViewType.Bar:
						return XlSparklineType.Column;
					case Sparkline.SparklineViewType.WinLoss:
						return XlSparklineType.WinLoss;
					default:
						return XlSparklineType.Line;
				}
			}
		}
		public Color ColorSeries { get { return edit.View.ActualColor; } }
		public Color ColorNegative { get { return edit.View.ActualNegativePointColor; } }
		public Color ColorMarker{
			get {
				var lineView = edit.View as LineSparklineView;
				if(lineView != null) return lineView.MarkerColor;
				return Color.Empty;
			}
		}
		public Color ColorFirst { get { return edit.View.ActualStartPointColor; } }
		public Color ColorLast { get { return edit.View.ActualEndPointColor; } }
		public Color ColorHigh { get { return edit.View.ActualMaxPointColor; } }
		public Color ColorLow { get { return edit.View.ActualMinPointColor; } }
		public double LineWeight{
			get {
				var lineView = edit.View as LineSparklineView;
				if (lineView != null) return lineView.LineWidth;
				return 1;
			}
		}
		public bool HighlightNegative { get { return edit.View.HighlightMinPoint; } }
		public bool HighlightFirst { get { return edit.View.HighlightStartPoint; } }
		public bool HighlightLast { get { return edit.View.HighlightEndPoint; } }
		public bool HighlightHighest { get { return edit.View.HighlightMaxPoint; } }
		public bool HighlightLowest { get { return edit.View.HighlightMinPoint; } }
		public bool DisplayMarkers{
			get {
				var lineView = edit.View as LineSparklineView;
				if (lineView != null) return lineView.ShowMarkers;
				return false;
			}
		}
	}
	internal class AdditionalSheetInfoWrapper : IAdditionalSheetInfo {
		public string Name { get { return "Additional Data"; } }
		public XlSheetVisibleState VisibleState { get { return XlSheetVisibleState.Hidden; } }
	}
	internal class GridViewAppearancePrintImplementer : IGridViewAppearancePrint {
		GridView view;
		public GridViewAppearancePrintImplementer(GridView view) {
			this.view = view;
		}
		XlCellFormatting IGridViewAppearancePrint.AppearanceEvenRow { get { return ExcelFormattingConverter.Convert(view.AppearancePrint.EvenRow); } }
		XlCellFormatting IGridViewAppearancePrint.AppearanceOddRow { get { return ExcelFormattingConverter.Convert(view.AppearancePrint.OddRow); } }
		XlCellFormatting IGridViewAppearancePrint.AppearanceGroupRow { get { return ExcelFormattingConverter.Convert(view.AppearancePrint.GroupRow); } }
		XlCellFormatting IGridViewAppearancePrint.AppearanceFooterPanel { get { return ExcelFormattingConverter.Convert(view.AppearancePrint.FooterPanel); } }
		XlCellFormatting IGridViewAppearancePrint.AppearanceGroupFooter { get { return ExcelFormattingConverter.Convert(view.AppearancePrint.GroupFooter); } }
		XlCellFormatting IGridViewAppearancePrint.AppearanceRow { get { return ExcelFormattingConverter.Convert(view.AppearancePrint.Row); } }
		XlCellFormatting IGridViewAppearancePrint.AppearanceHeader { get { return ExcelFormattingConverter.Convert(view.AppearancePrint.HeaderPanel); } }
	}
	public class GridViewImplementer<TCol, TRow> : IGridView<TCol, TRow> where TRow : DataRowImplementer where TCol : ColumnImplementer {
		protected GridView viewCore;
		ExportTarget exportTargetCore;
		public GridViewImplementer(GridView view, ExportTarget exportTarget){
			viewCore = view;
			exportTargetCore = exportTarget;
		}
		public string GetRowCellHyperlink(TRow row, TCol col){
			return null;
		}
		public bool GetAllowMerge(TCol col){
			return viewCore.GetColumnAllowMerge(col.GridColumn);
		}
		CellMergeEventArgs mergeArgs = new CellMergeEventArgs(0, 0, null);
		public int RaiseMergeEvent(int row1, int row2, TCol col){
			mergeArgs.Setup(row1, row2, col.GridColumn);
			viewCore.RaiseCellMerge(mergeArgs);
			return this.mergeArgs.Handled ? (this.mergeArgs.Merge ? 0 : -1) : 3;
		}
		XlCellFormatting _grappearance;
		public XlCellFormatting AppearanceGroupRow{
			get{
				if(_grappearance == null) _grappearance = ExcelFormattingConverter.Convert(viewCore.Appearance.GroupRow);
				return _grappearance;
			}
		}
		public bool IsCancelPending { get { return viewCore.ProgressWindowCancelPending; } }
		public IEnumerable<IFormatRuleBase> FormatRulesCollection{
			get{
				List<IFormatRuleBase> resultLst = new List<IFormatRuleBase>();
				foreach(GridFormatRule item in viewCore.FormatRules)
					resultLst.Add(new FormatRuleBaseImplementer(item));
				foreach(StyleFormatCondition condition in viewCore.FormatConditions)
					resultLst.Add(new FormatRuleBaseImplementer(viewCore.ConvertFormatConditionToFormatRule(condition)));
				return resultLst;
			}
		}
		public IEnumerable<FormatConditionObject> FormatConditionsCollection{
			get{
				List<FormatConditionObject> result = new List<FormatConditionObject>();
				foreach(StyleFormatCondition item in viewCore.FormatConditions){
					FormatConditionObject fco = ExcelFormattingConverter.Convert(item);
					if(item.Column != null && item.Condition != FormatConditionEnum.Expression){
						fco.Value1 = FormatCellValue(fco.Value1, -1, item.Column, item.Column.ColumnEdit);
						fco.Value2 = FormatCellValue(fco.Value2, -1, item.Column, item.Column.ColumnEdit);
					}
					result.Add(fco);
				}
				return result;
			}
		}
		public bool ReadOnly { get { return viewCore.OptionsBehavior.Editable; } }
		public bool ShowGroupFooter{
			get{
				switch(viewCore.OptionsView.GroupFooterShowMode){
					case GroupFooterShowMode.Hidden:
						return false;
					case GroupFooterShowMode.VisibleAlways:
						return true;
					case GroupFooterShowMode.VisibleIfExpanded:
						return true; 
					default:
						return false;
				}
			}
		}
		public bool ShowFooter { get { return viewCore.OptionsView.ShowFooter; } }
		public bool ShowGroupedColumns { get { return viewCore.OptionsView.ShowGroupedColumns; } }
		public IAdditionalSheetInfo AdditionalSheetInfo { get { return new AdditionalSheetInfoWrapper(); } }
		public int GetRowLevel(int lPosRow){
			return viewCore.GetRowLevel(lPosRow);
		}
		long IGridView<TCol, TRow>.RowCount { get { return viewCore.DataController.ListSourceRowCount + viewCore.DataController.GroupRowCount; } }
		public int RowHeight { get { return viewCore.RowHeight; } }
		public int FixedRowsCount { get { return 1; } }
		public bool IsGroupRow(int lPosRow){
			return viewCore.IsGroupRow(lPosRow);
		}
		public List<TCol> GetColumns(IEnumerable<GridColumn> collection, bool condition){
			var res = new List<TCol>();
			int gridColumnIndex = 0;
			foreach(GridColumn gridColumn in collection){
				if(CanAddColumn(gridColumn) || condition) res.Add((TCol) new ColumnImplementer(gridColumn, gridColumnIndex++));
			}
			return res;
		}
		bool CanAddColumn(GridColumn gridColumn){
			bool canExportColumn = viewCore.OptionsView.ShowGroupedColumns || gridColumn.GroupIndex == -1;
			if(gridColumn.Name == GridView.CheckBoxSelectorColumnName && !viewCore.OptionsSelection.ShowCheckBoxSelectorInPrintExport.ToBoolean(true)) return false;
			return gridColumn.OptionsColumn.Printable != DefaultBoolean.False && canExportColumn;
		}
		public IEnumerable<TCol> GetAllColumns(){
			return GetColumns(viewCore.VisibleColumns, false);
		}
		public IEnumerable<TCol> GetGroupedColumns(){
			return GetColumns(viewCore.GroupedColumns, true);
		}
		public IEnumerable<ISummaryItemEx> GridGroupSummaryItemCollection{
			get{
				var summaryCollection = new List<ISummaryItemEx>();
				GetGroupSummaryItems(summaryCollection, false);
				return summaryCollection;
			}
		}
		void GetGroupSummaryItems(List<ISummaryItemEx> summaryCollection, bool headerSummary){
			foreach(GridGroupSummaryItem item in viewCore.GroupSummary){
				string shownInColumnName = item.ShowInGroupColumnFooter != null ? item.ShowInGroupColumnFooter.FieldName : string.Empty;
				var addeditem = new SummaryItemExportImplementer(item, shownInColumnName, viewCore);
				if (shownInColumnName != string.Empty && !headerSummary){
					summaryCollection.Add(addeditem);
				}else if(string.IsNullOrEmpty(shownInColumnName) && headerSummary) summaryCollection.Add(addeditem);
			}
		}
		public IEnumerable<ISummaryItemEx> GridTotalSummaryItemCollection{
			get{
				List<ISummaryItemEx> result = new List<ISummaryItemEx>();
				foreach(GridColumn col in viewCore.Columns)
					result.AddRange(CreateColumnTotalSummaryCollection(col.Summary, col.FieldName));
				return result;
			}
		}
		public IEnumerable<ISummaryItemEx> GroupHeaderSummaryItems{
			get{
				var summaryCollection = new List<ISummaryItemEx>();
				GetGroupSummaryItems(summaryCollection, true);
				return summaryCollection;
			}
		}
		public IEnumerable<ISummaryItemEx> FixedSummaryItems { get { return new List<ISummaryItemEx>(); } }
		private IEnumerable<ISummaryItemEx> CreateColumnTotalSummaryCollection(GridColumnSummaryItemCollection gridColumnSummaryItemCollection,string columnCaption) {
			foreach(GridSummaryItem item in gridColumnSummaryItemCollection){
				yield return new SummaryItemExportImplementer(item, columnCaption, viewCore);
			}
		}
		public IEnumerable<TRow> GetAllRows() {
			IEnumerable rows = null;
			rows = GetRowsCore();
			Stack<GroupRowImplementer> rowsStack = new Stack<GroupRowImplementer>();
			int prevRowLevel = 0;
			List<TRow> result = new List<TRow>();
			foreach(int handle in rows) {
				int currentRowLevel = viewCore.GetRowLevel(handle);
				if(prevRowLevel > currentRowLevel && rowsStack.Count > 0) rowsStack.Pop();
				prevRowLevel = currentRowLevel;
				if(viewCore.IsGroupRow(handle)) {
					var gr = GetGroupRowImplementer(handle);
					if(rowsStack.Count > 0) rowsStack.Peek().AddChild(gr);
					else result.Add(gr as TRow);
					rowsStack.Push(gr);
				} else {
					var dr = new DataRowImplementer(viewCore, handle, viewCore.GetDataSourceRowIndex(handle));
					if(rowsStack.Count > 0) rowsStack.Peek().AddChild(dr);
					else result.Add((TRow)dr);
				}
			}
			return result;
		}
		protected virtual GroupRowImplementer GetGroupRowImplementer(int handle) {
			return new GroupRowImplementer(viewCore, handle);
		}
		protected virtual IEnumerable GetRowsCore() {
			IEnumerable rows;
			if(viewCore.OptionsPrint.PrintSelectedRowsOnly) {
				rows = ColumnViewPrintInfo.MakeSelectedRowListCore(viewCore);
			} else {
				var indexes = new VisibleIndexCollection(viewCore.DataController, viewCore.DataController.GroupInfo);
				indexes.BuildVisibleIndexes(viewCore.DataController.VisibleListSourceRowCount, true, true);
				rows = indexes;
			}
			return rows;
		}
		public virtual object GetRowCellValue(TRow row, TCol col) {
			object rawValue = viewCore.GetRowCellValue(row.LogicalPosition, ((ColumnImplementer)col).GridColumn);
			ColumnImplementer ci = col as ColumnImplementer;
			if(ci == null) return rawValue;
			if(ci.GridColumn == null) return rawValue;
			var ce = viewCore.GetRowCellRepositoryItem(row.LogicalPosition, ci.GridColumn); 
			rawValue = FormatCellValue(rawValue, row.LogicalPosition, ci.GridColumn, ce);
			return rawValue;
		}
		public FormatSettings GetRowCellFormatting(TRow row, TCol col){
			return null;
		}
		public XlCellFormatting AppearanceEvenRow { get { return ExcelFormattingConverter.Convert(viewCore.Appearance.EvenRow); } }
		public XlCellFormatting AppearanceOddRow { get { return ExcelFormattingConverter.Convert(viewCore.Appearance.OddRow); } }
		public XlCellFormatting AppearanceGroupFooter { get { return ExcelFormattingConverter.Convert(viewCore.Appearance.GroupFooter); } }
		public XlCellFormatting AppearanceHeader { get { return ExcelFormattingConverter.Convert(viewCore.Appearance.HeaderPanel); } }
		public XlCellFormatting AppearanceFooter { get { return ExcelFormattingConverter.Convert(viewCore.Appearance.FooterPanel); } }
		public XlCellFormatting AppearanceRow { get { return ExcelFormattingConverter.Convert(viewCore.Appearance.Row); } }
		public IGridViewAppearancePrint AppearancePrint { get { return new GridViewAppearancePrintImplementer(this.viewCore); } }
		private object FormatCellValue(object rawValue, int rowHandle, GridColumn column, RepositoryItem ri) {
			if(column == null) return rawValue;
			if(ri == null) return rawValue;
			return ri.GetEditValueForExportByOptions(rawValue, rowHandle, column, rawValue, (rowHandleE, columnE, rawValueE) => { return viewCore.GetRowCellDisplayText((int)rowHandleE, (GridColumn)columnE, rawValueE); }, exportTargetCore);
		}
		public string GetGroupRowHeader(int lPosRow) {
			return viewCore.GetGroupRowDisplayText(lPosRow);
		}
		public string GetViewCaption {
			get{
				if(viewCore.OptionsView.ShowViewCaption)
					return viewCore.GetViewCaption();
				return string.Empty;
			}
		}
		public string FilterString{
			get { return viewCore.ActiveFilterString; }
		}
	}
	public class ClipboardGroupRowImplementer : GroupRowImplementer, IClipboardGroupRow<DataRowImplementer> {
		public ClipboardGroupRowImplementer(GridView gridView, int handle)
			: base(gridView, handle) {
			int childCount = gridViewCore.GetChildRowCount(handle);
			for(int index = 0; index < childCount; index++) {
				int childHandle = gridViewCore.GetChildRowHandle(handle, index);
				int dataSourceRowIndex = gridViewCore.GetDataSourceRowIndex(childHandle);
					if(gridViewCore.IsGroupRow(childHandle)) children.Add(new ClipboardGroupRowImplementer(gridView, childHandle));
					else children.Add(new DataRowImplementer(gridView, childHandle, dataSourceRowIndex));
			}
		}
		bool isAnyParentCollapsed(int handle) {
			int parent = gridViewCore.GetParentRowHandle(handle);
			if(parent == GridDataController.InvalidRow) return false;
			while(parent != GridDataController.InvalidRow) {
				if(!gridViewCore.GetRowExpanded(parent)) return true;
				parent = gridViewCore.GetParentRowHandle(parent);
			}
			return false;
		}
		public IEnumerable<DataRowImplementer> GetSelectedRows() {
			List<DataRowImplementer> selected = new List<DataRowImplementer>();
			foreach(DataRowImplementer row in children)
				if(gridViewCore.IsRowSelected(row.RowHandle))
					selected.Add(row);
			if(selected.Count > 0) 
				return selected;
			if(gridViewCore.OptionsClipboard.CopyCollapsedData == DefaultBoolean.True && isAnyParentCollapsed(RowHandle))
				return children;
			return selected;
		}
		public override IEnumerable<DataRowImplementer> GetAllRows() {
			List<DataRowImplementer> selected = new List<DataRowImplementer>();
			foreach(DataRowImplementer row in children)
				if(gridViewCore.IsRowSelected(row.RowHandle))
					selected.Add(row);
			if(selected.Count > 0)
				return selected;
			return base.GetAllRows();
		}
		public bool IsTreeListGroupRow() {
			return false;
		}
		public override bool IsCollapsed {
			get {
				if(gridViewCore.OptionsClipboard.CopyCollapsedData == DefaultBoolean.True) return !gridViewCore.GetRowExpanded(rowCore);
				else return base.IsCollapsed;
			}
		}
	}
	public class GroupRowImplementer : DataRowImplementer, IGroupRow<DataRowImplementer> {
		protected List<DataRowImplementer> children;
		public GroupRowImplementer(GridView gridView, int handle)
			: base(gridView, handle, -1) {
			children = new List<DataRowImplementer>();
		}
		public virtual bool IsCollapsed {
			get{
				if(gridViewCore.OptionsPrint.ExpandAllGroups) return false;
				return !gridViewCore.GetRowExpanded(rowCore);
			}
		}
		public void AddChild(DataRowImplementer dataRow) {
			children.Add(dataRow);
		}
		public virtual IEnumerable<DataRowImplementer> GetAllRows() {
			return children;
		}
		public int RowHeight { get { return gridViewCore.RowHeight; } }
		FormatSettings IRowBase.FormatSettings { get { return null; } }
		int IRowBase.DataSourceRowIndex{
			get { return gridViewCore.GetDataSourceRowIndex(rowCore); }
		}
		public override bool IsGroupRow { get { return true; } }
		public string GetGroupRowHeader() { return gridViewCore.GetGroupRowDisplayText(rowCore); }
	}
	public class DataRowImplementer : IRowBase {
		protected GridView gridViewCore;
		protected int rowCore, dataSourceRowIndexCore;
		public DataRowImplementer(GridView gridView, int rowHandle, int dataSourceRowIndex) {
			gridViewCore = gridView;
			rowCore = rowHandle;
			dataSourceRowIndexCore = dataSourceRowIndex;
		}
		public int LogicalPosition { get { return rowCore; } }
		public int RowHandle { get { return rowCore; } }
		public int DataSourceRowIndex { get { return dataSourceRowIndexCore; } }
		public int GetRowLevel() {
			return gridViewCore.GetRowLevel(rowCore);
		}
		public virtual bool IsGroupRow { get { return false; } }
		public bool IsDataAreaRow { get { return true; } }
		public FormatSettings FormatSettings { get { return null; } }
	}
	public class ColumnImplementer : IColumn{
		GridColumn columnCore;
		int indexCore;
		public ColumnImplementer(GridColumn column, int index){
			columnCore = column;
			indexCore = index;
		}
		public IEnumerable<IColumn> GetAllColumns(){
			return null;
		}
		public int GetColumnGroupLevel(){
			return 0;
		}
		public ISparklineInfo SparklineInfo { get { return GetSparklineInfo(); } }
		ISparklineInfo GetSparklineInfo(){
			var sparklineEdit = columnCore.ColumnEdit as RepositoryItemSparklineEdit;
			if(sparklineEdit != null){
				return new SparklineInfo(sparklineEdit);
			}
			return null;
		}
		public ColumnSortOrder SortOrder{
			get { return columnCore.SortOrder; }
		}
		public IUnboundInfo UnboundInfo{
			get { return new UnboundInfoWrapper(this.GridColumn); }
		}
		public bool IsGroupColumn { get { return false; } }
		public bool IsCollapsed { get { return false; } }
		public GridColumn GridColumn { get { return columnCore; } }
		public string Header { get { return columnCore.GetTextCaption(); } }
		public string Name { get { return columnCore.Name; } }
		public string FieldName { get { return columnCore.FieldName; } }
		public string GetGroupColumnHeader(){
			return null;
		}
		public override int GetHashCode() { return columnCore.GetHashCode(); }
		public int Width { get { return columnCore.VisibleWidth; } }
		public int VisibleIndex { get { return columnCore.VisibleIndex; } }
		public int GroupIndex { get { return columnCore.GroupIndex; } }
		public bool IsVisible { get { return columnCore.Visible; } }
		public bool HasGroupIndex { get { return columnCore.GroupIndex != -1; } }
		public Type ColumnType { get { return columnCore.ColumnType; } }
		string GetFormatString(){
			var ce = columnCore.ColumnEdit as RepositoryItemTextEdit;
			if(ce != null)
				if(ce.Mask.UseMaskAsDisplayFormat && ce.Mask.EditMask != "N00") return ce.Mask.EditMask;
			string format = columnCore.DisplayFormat.FormatString;
			if(!string.IsNullOrEmpty(format)) return format;
			if(ce != null){
				string formatRepository = ce.DisplayFormat.FormatString;
				if(!string.IsNullOrEmpty(formatRepository)) return formatRepository;
			}
			return format;
		}
		public string HyperlinkEditorCaption {
			get{
				if(columnCore.ColumnEdit is RepositoryItemHyperLinkEdit){
					var ce = columnCore.ColumnEdit as RepositoryItemHyperLinkEdit;
					return ce.Caption;
				}
				return string.Empty;
			}
		}
		public string HyperlinkTextFormatString{
			get { return string.Empty; }
		}
		public int LogicalPosition { get { return indexCore; } }
		public override bool Equals(object obj) {
			ColumnImplementer castedObj = obj as ColumnImplementer;
			return castedObj.columnCore == columnCore;
		}
		public FormatSettings FormatSettings{
			get{
				return new FormatSettings { FormatType = columnCore.DisplayFormat.FormatType, FormatString = GetFormatString(), ActualDataType = columnCore.ColumnType };
			}
		}
		public ColumnEditTypes ColEditType {
			get {
				if(columnCore.ColumnEdit is RepositoryItemComboBox) return ColumnEditTypes.Lookup;
				if(columnCore.ColumnEdit is RepositoryItemLookUpEdit) return ColumnEditTypes.Lookup;
				if(columnCore.ColumnEdit is RepositoryItemProgressBar) return ColumnEditTypes.ProgressBar;
				if(columnCore.ColumnEdit is RepositoryItemPictureEdit
				|| columnCore.ColumnEdit is RepositoryItemImageEdit) return ColumnEditTypes.Image;
				if(columnCore.ColumnEdit is RepositoryItemHyperLinkEdit) return ColumnEditTypes.Hyperlink;
				if(columnCore.ColumnEdit is RepositoryItemSparklineEdit) return ColumnEditTypes.Sparkline;
				return ColumnEditTypes.Text;
			}
		}
		public IEnumerable<object> DataValidationItems {
			get {
				List<object> result = new List<object>();
				var ce = columnCore.View.GetRowCellRepositoryItem(GridControl.InvalidRowHandle, columnCore); 
				if(ce == null) return null;
				if(ce is RepositoryItemImageComboBox) {
					var riic = ce as RepositoryItemImageComboBox;
					foreach(XtraEditors.Controls.ImageComboBoxItem item in riic.Items) {
						if(item.Description!=string.Empty) result.Add(item.ToString());
					}
					return result.Count==0 ? null : result;
				}
				if(ce is RepositoryItemComboBox && !(ce is RepositoryItemImageComboBox)) {
					RepositoryItemComboBox ric = ce as RepositoryItemComboBox;
					foreach(var item in ric.Items)
						result.Add(ric.GetDisplayText(ric.DisplayFormat, item));
					return result;
				}
				if(ce is RepositoryItemGridLookUpEdit) {
					RepositoryItemGridLookUpEdit rigle = ce as RepositoryItemGridLookUpEdit;
					for(int i = 0; rigle.GetDisplayValue(i) != null; i++) {
						result.Add(rigle.GetDisplayValue(i));
					}
					return result;
				}
				if(ce is RepositoryItemLookUpEdit) {
					RepositoryItemLookUpEdit rile = ce as RepositoryItemLookUpEdit;
					for(int i = 0; rile.GetDataSourceValue(rile.ValueMember, i) != null; i++)
						result.Add(rile.GetDataSourceValue(rile.DisplayMember, i).ToString());
					return result;
				}
				return null;
			}
		}
		public XlCellFormatting Appearance { get { return ExcelFormattingConverter.Convert(columnCore.AppearanceCell); } }
		public XlCellFormatting AppearanceHeader {
			get { return ExcelFormattingConverter.Convert(columnCore.AppearanceHeader); }
		}
		public bool IsFixedLeft {
			get {
				if(columnCore.Fixed == FixedStyle.Left) return true;
				BandedGrid.BandedGridColumn bgc = columnCore as BandedGrid.BandedGridColumn;
				if(bgc != null && bgc.OwnerBand != null && bgc.OwnerBand.Fixed == FixedStyle.Left) return true;
				return false;
			}
		}
	}
	public class FormatRuleBaseImplementer : IFormatRuleBase {
		GridFormatRule rule;
		public FormatRuleBaseImplementer(GridFormatRule rule) {
			this.rule = rule;
		}
		public bool StopIfTrue { get { return rule.StopIfTrue; } }
		public bool ApplyToRow { get { return rule.ApplyToRow; } }
		public IColumn ColumnApplyTo{
			get{
				if(rule.ColumnApplyTo != null)
					return new ColumnImplementer(rule.ColumnApplyTo, rule.ColumnApplyTo.VisibleIndex);
				return null;
			}
		}
		public string ColumnName { get { return rule.ColumnName; } }
		public IColumn Column { get {
				if(rule.Column == null) return null;
			return new ColumnImplementer(rule.Column, rule.Column.VisibleIndex); }
		}
		public bool Enabled { get { return rule.Enabled; } }
		public string Name { get { return rule.Name; } }
		public IFormatConditionRuleBase Rule { get { return rule.Rule as IFormatConditionRuleBase; } }
		public object Tag {
			get { return rule.Tag; }
			set { rule.Tag = value; }
		}
	}
	public class GridViewFactoryImplementer : IGridViewFactory<ColumnImplementer, DataRowImplementer>
	{
		GridView viewCore;
		public GridViewFactoryImplementer(GridView view) {
			viewCore = view;
		}
		string IGridViewFactory<ColumnImplementer, DataRowImplementer>.GetDataMember()
		{
			if (viewCore.GridControl != null) return viewCore.GridControl.DataMember;
			else return null;
		}
		object IGridViewFactory<ColumnImplementer, DataRowImplementer>.GetDataSource()
		{
			if (viewCore.GridControl != null) return viewCore.GridControl.DataSource;
			else return null;
		}
		void IGridViewFactory<ColumnImplementer, DataRowImplementer>.ReleaseIViewImplementerInstance(IGridView<ColumnImplementer, DataRowImplementer> instance) { }
		IGridView<ColumnImplementer, DataRowImplementer> IGridViewFactory<ColumnImplementer, DataRowImplementer>.GetIViewImplementerInstance()
		{
			return new GridViewImplementer<ColumnImplementer, DataRowImplementer>(viewCore, XtraPrinting.ExportTarget.Xlsx);
		}
	}
}
