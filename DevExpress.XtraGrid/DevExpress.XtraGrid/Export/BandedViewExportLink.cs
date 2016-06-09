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
using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.BandedGrid.ViewInfo;
using DevExpress.XtraExport;
using DevExpress.Data;
	namespace DevExpress.XtraGrid.Export {
#pragma warning disable 618
		[Obsolete("Use the BaseView.ExportTo* methods")]
		public class BandedViewExportLink : GridViewExportLink {
#pragma warning restore 618
		protected int bandStyle;
		protected int bandPanelEmptySpaceStyle;
		protected int headerPanelEmptySpaceStyle;
		protected int footerPanelEmptySpaceStyle;
		protected BandExportInfos bandInfos;
		public BandedViewExportLink(BaseView view, IExportProvider provider):
			base(view, provider) {			
		}
		public new BandedViewAppearances ExportAppearance { 
			get { return base.ExportAppearance as BandedViewAppearances; }
		}
		protected override BaseAppearanceCollection CreateAppearanceCollectionInstance() {
			return new BandedViewAppearances(View); 
		}
		private void SetBorders11(int col, int row, ExportCacheCellStyle style) {
			style.LeftBorder.Width = 1;
			style.TopBorder.Width = 1;
			style.RightBorder.Width = 1;
			style.BottomBorder.Width = 1;
			fProvider.SetCellStyle(col, row, style);
		}
		private void SetBorders1N(int col, int row, int height, ExportCacheCellStyle style) {
			style.LeftBorder.Width = 1;
			style.RightBorder.Width = 1;
			for(int i = 1; i < height - 1; i++)
				fProvider.SetCellStyle(col, row + i, style);
			style.TopBorder.Width = 1;
			fProvider.SetCellStyle(col, row, style);
			style.TopBorder.Width = 0;
			style.BottomBorder.Width = 1;
			fProvider.SetCellStyle(col, row + height - 1, style);
		}
		private void SetBordersN1(int col, int row, int width, ExportCacheCellStyle style) {
			style.TopBorder.Width = 1;
			style.BottomBorder.Width = 1;
			for(int i = 1; i < width - 1; i++)
				fProvider.SetCellStyle(col + i, row, style);
			style.LeftBorder.Width = 1;
			fProvider.SetCellStyle(col, row, style);
			style.LeftBorder.Width = 0;
			style.RightBorder.Width = 1;
			fProvider.SetCellStyle(col + width - 1, row, style);
		}
		private void SetBordersNN(int col, int row, int width, int height, ExportCacheCellStyle style) {
			style.TopBorder.Width = 1;
			for(int i = 1; i < width - 1; i++)
				fProvider.SetCellStyle(col + i, row, style);
			style.RightBorder.Width = 1;
			fProvider.SetCellStyle(col + width - 1, row, style);
			style.TopBorder.Width = 0;
			for(int i = 1; i < height - 1; i++)
				fProvider.SetCellStyle(col + width - 1, row + i, style);
			style.BottomBorder.Width = 1;
			fProvider.SetCellStyle(col + width - 1, row + height - 1, style);
			style.RightBorder.Width = 0;
			for(int i = 1; i < width - 1; i++)
				fProvider.SetCellStyle(col + i, row + height - 1, style);
			style.LeftBorder.Width = 1;
			fProvider.SetCellStyle(col, row + height - 1, style);
			style.BottomBorder.Width = 0;
			for(int i = 1; i < height - 1; i++)
				fProvider.SetCellStyle(col, row + i, style);
			style.TopBorder.Width = 1;
			fProvider.SetCellStyle(col, row, style);
		}
		protected int GetBandsTotalWidth() {
			int result = bandInfos.TotalWidth;
			if(result <= 0)
				result = fCacheWidth;
			return result;
		}
		protected int GetBandsTotalHeight() {
			int result = bandInfos.TotalHeight;
			if(result <= 0)
				result = 1;
			return result;
		}
		protected bool IsMostLeftBand(BandExportInfo info) {
			return info.Col == 0;
		}
		protected void FillBandPanelEmptySpace(int startRow) {
			for(int i = 0; i < GetBandsTotalHeight(); i++)
				for(int j = 0; j < fCacheWidth; j++)
					fProvider.SetCellStyle(j, startRow + i, bandPanelEmptySpaceStyle);
		}
		protected void FillHeaderPanelEmptySpace(int startRow) {
			for(int i = 0; i < GetHeaderHeight(); i++)
				for(int j = 0; j < fCacheWidth; j++)
					fProvider.SetCellStyle(j, startRow + i, headerPanelEmptySpaceStyle);
		}
		protected void FillFooterPanelEmptySpace(int startRow, int colOffset) {
			for(int i = 0; i < GetFooterHeight(); i++)
				for(int j = colOffset; j < fCacheWidth; j++)
					fProvider.SetCellStyle(j, startRow + i, footerPanelEmptySpaceStyle);
		}
		protected void SetBorders(int col, int row, int width, int height, int styleIndex) {
			ExportCacheCellStyle style = fProvider.GetStyle(styleIndex);
			style.LeftBorder.Width = 0;
			style.RightBorder.Width = 0;
			style.TopBorder.Width = 0;
			style.BottomBorder.Width = 0;
			if(width == 1 && height == 1)
				SetBorders11(col, row, style);
			else if(width > 1 && height == 1)
				SetBordersN1(col, row, width, style);
			else if(width == 1 && height > 1)
				SetBorders1N(col, row, height, style);
			else
				SetBordersNN(col, row, width, height, style);
		}
		protected virtual void LoadBands(ref int row) {
			FillBandPanelEmptySpace(row);
			for(int i = 0; i < bandInfos.Count; i++)
				LoadBand(bandInfos[i], row);
			row += GetBandsTotalHeight();
		}
		protected virtual void LoadBand(BandExportInfo info, int startRow) {
			int row = info.Row + startRow;
			fProvider.SetCellStyleAndUnion(info.Col, row, info.Width, info.Height, bandStyle);
			fProvider.SetCellString(info.Col, row, info.Band.GetTextCaption());
			for(int i = 0; i < info.Children.Length; i++)
				LoadBand(info.Children[i], startRow);			
		}
		protected virtual void LoadHeaderContent(BandExportInfo info, int startRow) {
			if(!info.HasChildren) {
				int row = startRow;
				for(int i = 0; i < info.ColumnInfos.Count; i++) {
					for(int j = 0; j < info.ColumnInfos[i].Count; j++) {
						fProvider.SetCellString(
							info.Col + info.ColumnInfos[i][j].HeaderCol, 
							row, 
							info.ColumnInfos[i][j].Column.GetTextCaption());						
						fProvider.SetCellStyleAndUnion(
							info.Col + info.ColumnInfos[i][j].HeaderCol, 
							row, 
							info.ColumnInfos[i][j].HeaderUnionWidth, 
							info.ColumnInfos[i][j].Column.RowCount, 
							fHeaderStyle);
					}
					row += info.ColumnInfos[i].MaxRowCount;
				}
			} else
				for(int i = 0; i < info.Children.Length; i++)
					LoadHeaderContent(info.Children[i], startRow);
		}		
		protected virtual void LoadFooterContent(BandExportInfo info, int startRow) {
			if(!info.HasChildren) {
				int row = startRow;
				for(int i = 0; i < info.ColumnInfos.Count; i++) {
					for(int j = 0; j < info.ColumnInfos[i].Count; j++) {
						if(info.ColumnInfos[i][j].Column.SummaryItem.SummaryType == SummaryItemType.None)
							continue;
						if(ExportCellsAsDisplayText) {
							fProvider.SetCellString(
								info.Col + info.ColumnInfos[i][j].HeaderCol, 
								row, 
								info.ColumnInfos[i][j].Column.SummaryText);
						} else {
							fProvider.SetCellData(
								info.Col + info.ColumnInfos[i][j].HeaderCol, 
								row,
								info.ColumnInfos[i][j].Column.SummaryItem.SummaryValue);
						}
						fProvider.SetCellStyleAndUnion(
							info.Col + info.ColumnInfos[i][j].HeaderCol, 
							row, 
							info.ColumnInfos[i][j].HeaderUnionWidth, 
							info.ColumnInfos[i][j].Column.RowCount, 
							fFooterStyle);
					}
					row += info.ColumnInfos[i].MaxRowCount;
				}
			} else
				for(int i = 0; i < info.Children.Length; i++)
					LoadFooterContent(info.Children[i], startRow);
		}
		protected virtual void LoadGroupFooterData(BandExportInfo info, int row, int handle, int level) {
			if(!info.HasChildren) {
				int row_ = row;
				int col;
				int width;
				for(int i = 0; i < info.ColumnInfos.Count; i++) {
					for(int j = 0; j < info.ColumnInfos[i].Count; j++) {
					string groupFooterText = View.GetRowFooterCellText(handle, info.ColumnInfos[i][j].Column);
					object groupFooterValue = View.GetRowSummaryItem(handle, info.ColumnInfos[i][j].Column).Value;
					if(groupFooterText != "" && groupFooterValue != null) {
						col = info.Col + info.ColumnInfos[i][j].Col;
						width = info.ColumnInfos[i][j].UnionWidth;
		 				if(ExportCellsAsDisplayText)
							fProvider.SetCellString(col, row_, groupFooterText);
						else
							fProvider.SetCellData(col, row_, groupFooterValue);
						fProvider.SetCellStyleAndUnion(
							col, 
							row_, 
							width, 
							info.ColumnInfos[i][j].Column.RowCount, 
							fFooterStyle);						
						}
					}
					row_ += info.ColumnInfos[i].MaxRowCount;
				}
			} else
				for(int i = 0; i < info.Children.Length; i++)
					LoadGroupFooterData(info.Children[i], row, handle, level);
		}
		protected virtual void LoadRowData(BandExportInfo info, int level, int row, int handle) {
			if(!info.HasChildren) {
				int bandCol = info.Col;
				int bandWidth = info.Width;
				if(IsMostLeftBand(info)) {
					if(info.Band.Columns.VisibleColumnCount > 0) {
						bandCol += info.ColumnInfos[0][0].Col;
						bandWidth -= info.ColumnInfos[0][0].Col;
					} else {
						bandCol += View.SortInfo.GroupCount;
						bandWidth -= View.SortInfo.GroupCount;
					}
				}
				if(info.ColumnInfos.Count > 0) {
					SetBorders(bandCol, row, bandWidth, GetRowHeight(handle), fProvider.RegisterStyle(GetDataRowStyle(handle)));
					int row_ = row;
					for(int i = 0; i < info.ColumnInfos.Count; i++) {
						for(int j = 0; j < info.ColumnInfos[i].Count; j++) {
							fProvider.SetCellStyleAndUnion(
								info.Col + info.ColumnInfos[i][j].Col, 
								row_, 
								info.ColumnInfos[i][j].UnionWidth, 
								info.ColumnInfos[i][j].Column.RowCount,
								GetDataRowCellStyle(handle, info.ColumnInfos[i][j].Column));
							if(ExportCellsAsDisplayText)
								fProvider.SetCellString(info.Col + info.ColumnInfos[i][j].Col, 
									row_, 
									View.GetRowCellDisplayText(handle, info.ColumnInfos[i][j].Column));
							else
								fProvider.SetCellData(info.Col + info.ColumnInfos[i][j].Col, 
									row_, 
									View.GetRowCellValue(handle, info.ColumnInfos[i][j].Column));
						}
						row_ += info.ColumnInfos[i].MaxRowCount;
					}
				} else
					fProvider.SetCellStyleAndUnion(
						bandCol, row, 
						bandWidth, GetRowHeight(handle), 
						GetDataRowStyle(handle));
			} else
				for(int i = 0; i < info.Children.Length; i++)
					LoadRowData(info.Children[i], level, row, handle);
		}
		protected virtual void CalcBandInfos() {
			bandInfos = new BandExportInfos(View, IsMasterViewMode && fExportDetails);
			bandInfos.Calculate();
		}
		protected new BandedGridViewInfo ViewInfo { get { return base.ViewInfo as BandedGridViewInfo; } }
		protected virtual ExportCacheCellStyle GetBandStyle() {
			return GridStyleToExportStyle(ExportAppearance.BandPanel, null, null);
		}
		protected virtual ExportCacheCellStyle GetBandPanelEmptySpaceStyle() {
			ExportCacheCellStyle result = GridStyleToExportStyle(ExportAppearance.BandPanelBackground, null, null);
			result.TopBorder.Width = 0;
			result.BottomBorder.Width = 0;
			result.LeftBorder.Width = 0;
			result.RightBorder.Width = 0;
			return result;
		}
		protected virtual ExportCacheCellStyle GetHeaderPanelEmptySpaceStyle() {
			ExportCacheCellStyle result = GridStyleToExportStyle(ExportAppearance.HeaderPanelBackground, null, null);
			result.TopBorder.Width = 0;
			result.BottomBorder.Width = 0;
			result.LeftBorder.Width = 0;
			result.RightBorder.Width = 0;
			return result;
		}
		protected virtual ExportCacheCellStyle GetFooterPanelEmptySpaceStyle() {
			ExportCacheCellStyle result = fProvider.GetStyle(fHeaderStyle);
			result.TopBorder.Width = 0;
			result.BottomBorder.Width = 0;
			result.LeftBorder.Width = 0;
			result.RightBorder.Width = 0;
			return result;
		}
		protected override void LoadHeaderContent(ref int row) {
			FillHeaderPanelEmptySpace(row);
			for(int i = 0; i < bandInfos.Count; i++)
				LoadHeaderContent(bandInfos[i], row);
			row += GetHeaderHeight();
		}
		protected override void LoadFooterContent(ref int row) {
			FillFooterPanelEmptySpace(row, 0);
			SetBorders(0, row, GetBandsTotalWidth(), GetFooterHeight(), fFooterStyle);
			for(int i = 0; i < bandInfos.Count; i++)
				LoadFooterContent(bandInfos[i], row);
			row += GetFooterHeight();
		}
		protected override void LoadGroupFooterData(ref int row, int handle, int level) {
			for(int i = 1; i < GetFooterHeight(); i++)
				ModifyGroupSeparatorsStyle(level - 1, row + i);
			FillFooterPanelEmptySpace(row, level);
			SetBorders(level, row, GetBandsTotalWidth() - level, GetFooterHeight(), fFooterStyle);
			for(int i = 0; i < bandInfos.Count; i++)
				LoadGroupFooterData(bandInfos[i], row, handle, level);
			row += GetFooterHeight();
		}
		protected override void LoadRowData(int level, int row, int handle) {
			for(int i = 1; i < GetRowHeight(handle); i++) 
				ModifyGroupSeparatorsStyle(level - 1, row + i);
			if(isMasterView && fExportDetails) {
				ExportCacheCellStyle style = GetDataRowStyle(handle);
				if(View.IsShowDetailButtons)
					for(int i = 1; i < GetRowHeight(handle); i++)	
						ModifyMasterSeparatorStyle(level - 1, row + i, style);
			}
			for(int i = 0; i < bandInfos.Count; i++)	
				LoadRowData(bandInfos[i], level, row, handle);
		}
		protected override void RegisterStyles() {
			base.RegisterStyles();
			bandStyle = fProvider.RegisterStyle(GetBandStyle());
			bandPanelEmptySpaceStyle = fProvider.RegisterStyle(GetBandPanelEmptySpaceStyle());
			headerPanelEmptySpaceStyle = fProvider.RegisterStyle(GetHeaderPanelEmptySpaceStyle());
			footerPanelEmptySpaceStyle = fProvider.RegisterStyle(GetFooterPanelEmptySpaceStyle());
		}
		protected override int LoadCache(int startRow) {
			int row = startRow;
			if(View.OptionsView.ShowBands)
				LoadBands(ref row);
			return base.LoadCache(row);
		}
		protected override int CalcCacheHeight() {
			int result = base.CalcCacheHeight();
			if(View.OptionsView.ShowBands)
				result += GetBandsTotalHeight();
			return result;
		}
		protected override int CalcCacheWidth() {
			int result = 0;
			for(int i = 0; i < bandInfos.Count; i++)
				result += bandInfos[i].Width;
			if(IsMasterViewMode)
				isMasterView = true;
			if(result <= 0)
				result = View.SortInfo.GroupCount + 1;
			return result;
		}
		protected override int GetHeaderHeight() {
			int result = bandInfos.RecordTotalHeight;
			if(result <= 0)
				result = 1;
			return result;
		}
		protected override int GetRowHeight(int handle) {
			int result;
			if(View.IsGroupRow(handle))
				result = 1;
			else
				result = bandInfos.RecordTotalHeight;
			if(result <= 0)
				result = 1;
			return result;
		}
		protected override int GetFooterHeight() {
			int result = bandInfos.RecordTotalHeight;
			if(result <= 0)
				result = 1;
			return result;
		}
		protected override void SetColumnsWidth() {
		}
		protected override void DoExport() {
			CalcBandInfos();
			base.DoExport();
		}
		protected override bool TestView() {
			return fView is BandedGridView;
		}
		public new BandedGridView View {
			get {
				return (BandedGridView)fView;
			}
		}
	}
	#region Export Information 
	public class BandExportInfo {
		private BandExportInfo[] children;
		private ColumnExportInfos columnInfos;
		private int col;
		private int row;
		private int width;
		private int height;
		private int heightWithChildren;
		private bool hasColumns;
		private GridBand band;
		public BandExportInfo(GridBand band) {
			this.band = band;
		}
		protected void CalcWidth() {
			int width = columnInfos.TotalWidth;
			int chWidth = 0;
			hasColumns = width > 0;
			for(int i = 0; i < children.Length; i++) {
				children[i].CalcWidth();
				chWidth += children[i].Width;
				hasColumns |= children[i].HasColumns;
			}
			this.width = Math.Max(width, chWidth);
			if(this.width == 0)
				this.width = 1;
		}
		protected void CalcHeight() {
			height = band.RowCount;
			int chMaxHeight = 0;
			for(int i = 0; i < children.Length; i++) {
				children[i].CalcHeight();
				chMaxHeight = Math.Max(chMaxHeight, children[i].HeightWithChildren);
			}
			heightWithChildren = height + chMaxHeight;
		}
		public void CreateChildren() {
			children = new BandExportInfo[band.Children.VisibleBandCount];
			int k = 0;
			for(int i = 0; i < band.Children.Count; i++) {
				if(!band.Children[i].Visible)
					continue;
				children[k] = new BandExportInfo(band.Children[i]);
				children[k].CreateChildren();
				k++;
			}
		}
		public void CreateVisibleColumns() {
			columnInfos = new ColumnExportInfos(this);
			columnInfos.Calculate();
			for(int i = 0; i < children.Length; i++)
				children[i].CreateVisibleColumns();
		}
		public void Calculate() {
			CalcWidth();
			CalcHeight();
		}
		public void CorrectWidth(int value_, bool isMasterView, bool isShowDetailButtons) {
			width += value_;
			columnInfos.CorrectWidth(value_, isMasterView, isShowDetailButtons);
			if(children.Length > 0)
				children[0].CorrectWidth(value_, isMasterView, isShowDetailButtons);
		}
		public void CorrectHeight(int totalHeight) {
			if(!HasChildren) {
				if(Band.AutoFillDown)
					height = totalHeight - row;
				return;
			}
			for(int i = 0; i < children.Length; i++)
				children[i].CorrectHeight(totalHeight);
		}
		public void CalcCol(int startCol) {
			int inc = 0;
			col = startCol;
			for(int i = 0; i < children.Length; i++) {
				children[i].CalcCol(startCol + inc);
				inc += children[i].Width;
			}
		}
		public void CalcRow(int startRow) {
			row = startRow;
			for(int i = 0; i < children.Length; i++)
				children[i].CalcRow(row + height);
		}
		public void CalcRecordTotalHeight(ref int totalHeight) {
			if(!HasChildren) {
				if(totalHeight < columnInfos.TotalHeight)
					totalHeight = columnInfos.TotalHeight;
			} else
				for(int i = 0; i < children.Length; i++)
					children[i].CalcRecordTotalHeight(ref totalHeight);
		}
		public int Col {
			get {
				return col;
			}
		}
		public int Row {
			get {
				return row;
			}
		}
		public int Width {
			get {
				return width;
			}
		}
		public int Height { 
			get {
				return height;
			}
		}
		public int HeightWithChildren {
			get {
				return heightWithChildren;
			}
		}
		public BandExportInfo[] Children {
			get {
				return children;
			}
		}
		public ColumnExportInfos ColumnInfos {
			get {
				return columnInfos;
			}
		}
		public GridBand Band {
			get {
				return band;
			}
		}
		public bool HasChildren {
			get {
				return children.Length > 0;
			}
		}
		public bool HasColumns {
			get {
				return hasColumns;
			}
		}
	}
	public class BandExportInfos {
		private BandExportInfo[] infos;
		private BandedGridView view;
		private int totalWidth;
		private int totalHeight;
		private int recordTotalHeight;
		private bool fExportDetails;
		public BandExportInfos(BandedGridView view, bool exportDetails) {
			this.fExportDetails = exportDetails;
			this.view = view;
			infos = new BandExportInfo[view.Bands.VisibleBandCount];
			int k = 0;
			for(int i = 0; i < view.Bands.Count; i++) {
				if(!view.Bands[i].Visible)
					continue;
				infos[k] = new BandExportInfo(view.Bands[i]);
				infos[k].CreateChildren();
				infos[k].CreateVisibleColumns();
				k++;
			}
		}
		protected void CalcRecordTotalHeight() {
			for(int i = 0; i < infos.Length; i++)
				infos[i].CalcRecordTotalHeight(ref recordTotalHeight);
		}
		public void Calculate() {
			totalHeight = 0;
			for(int i = 0; i < infos.Length; i++) {
				infos[i].Calculate();
				totalHeight = Math.Max(totalHeight, infos[i].HeightWithChildren);
			}
			if(infos.Length > 0) {
				int corr = view.SortInfo.GroupCount;
				if(view.DataController.IsSupportMasterDetail && fExportDetails)
					corr++;
				infos[0].CorrectWidth(corr, fExportDetails, view.IsShowDetailButtons);
			}
			int col = 0;
			totalWidth = 0;
			for(int i = 0; i < infos.Length; i++) {
				infos[i].CalcCol(col);
				infos[i].CalcRow(0);
				infos[i].CorrectHeight(totalHeight);
				col += infos[i].Width;
				totalWidth += infos[i].Width;
			}			
			CalcRecordTotalHeight();
		}
		public BandExportInfo this[int index] {
			get {
				return infos[index];
			}
		}
		public int Count {
			get {
				return infos.Length;
			}
		}
		public int TotalWidth {
			get {
				return totalWidth;
			}
		}
		public int TotalHeight {
			get {
				return totalHeight;
			}
		}
		public int RecordTotalHeight {
			get {
				return recordTotalHeight;
			}
		}
	}
	public class ColumnExportInfo {
		public BandedGridColumn fColumn;
		public ColumnExportInfo(BandedGridColumn column) {
			this.fColumn = column;
		}
		public void Equalize() {
			HeaderUnionWidth = UnionWidth;
			HeaderCol = Col;
		}
		public int AccWidth;
		public int UnionWidth;
		public int Col;
		public int HeaderUnionWidth;
		public int HeaderCol;
		public BandedGridColumn Column {
			get {
				return fColumn;
			}
		}
	}
	public class ColumnExportInfoList {
		private ArrayList list;
		private int maxRowCount;
		public ColumnExportInfoList() {
			list = new ArrayList();
		}
		public int Add(ColumnExportInfo info) {
			return list.Add(info);
		}
		public ColumnExportInfo this[int index] {
			get {
				return list[index] as ColumnExportInfo;
			}
			set {
				list[index] = value;
			}
		}
		public int Count {
			get {
				return list.Count;
			}
		}
		public int MaxRowCount {
			get {
				return maxRowCount;
			}
			set {
				if(value > 0)
					maxRowCount = value;
			}
		}
	}
	public class ColumnExportInfos {
		private ArrayList infos;
		private int totalHeight;
		private int totalWidth;
		public ColumnExportInfos(BandExportInfo ownerBandInfo) {
			infos = new ArrayList();
			for(int i = 0; i < ownerBandInfo.Band.Columns.Count; i++) {
				BandedGridColumn column = ownerBandInfo.Band.Columns[i];
				if(column.VisibleIndex >= 0) {
					SetInfos(column.RowIndex, column.ColIndex, new ColumnExportInfo(column));
				}
			}
			PurgeInfos();
		}
		private bool HasNotNullColumnInfo(int index) {
			for(int i = 0; i < this[index].Count; i++)
				if(this[index][i] != null)
					return true;
			return false;
		}
		private void PurgeInfos() {
			ArrayList newInfos = new ArrayList();
			for(int i = 0; i < Count; i++) {
				if(HasNotNullColumnInfo(i)) {
					newInfos.Add(new ColumnExportInfoList());
					for(int j = 0; j < this[i].Count; j++) {
						if(this[i][j] != null)
							((ColumnExportInfoList)newInfos[newInfos.Count - 1]).Add(this[i][j]);
					}
				}
			}
			infos = newInfos;
		}
		private void AllocInfos(int rowIndex, int colIndex) {
			int count = 0; 
			if(rowIndex >= Count) {
				count = rowIndex - Count + 1;
				for(int i = 0; i < count; i++)
					infos.Add(new ColumnExportInfoList());
			}
			if(colIndex >= this[rowIndex].Count) {
				count = colIndex - this[rowIndex].Count + 1;
				for(int i = 0; i < count; i++)
					this[rowIndex].Add(null);
			}
		}
		private void SetInfos(int rowIndex, int colIndex, ColumnExportInfo info) {
			AllocInfos(rowIndex, colIndex);
			this[rowIndex][colIndex] = info;
		}
		protected void EqualizeHorzParams() {
			for(int i = 0; i < Count; i++)
				for(int j = 0; j < this[i].Count; j++)
					if(this[i][j] != null)
						this[i][j].Equalize();
		}
		protected void CalcAccWidth() {
			ColumnExportInfo prevInfo;
			for(int i = 0; i < Count; i++) {
				prevInfo = null;
				for(int j = 0; j < this[i].Count; j++) {
					ColumnExportInfo info = this[i][j];
					if(info != null) {
						info.AccWidth = info.Column.VisibleWidth;
						if(prevInfo != null)
							info.AccWidth += prevInfo.AccWidth;
						prevInfo = info;
					}
				}
			}
		}
		protected void CalcTotalWidth() {
			totalWidth = 0;
			if(Count > 0)
				for(int i = 0; i < this[0].Count; i++) 				
					totalWidth += this[0][i].UnionWidth;
		}
		protected void CalcCol() {
			for(int i = 0; i < Count; i++) {
				for(int j = 0; j < this[i].Count; j++) {
					if(this[i][j] != null) {
						if(j == 0)
							this[i][j].Col = 0;
						else 
							this[i][j].Col = this[i][j - 1].Col + this[i][j - 1].UnionWidth;
					}
				}
			}
		}
		protected void CalcHorzParams() {			
			CalcAccWidth();
			int[] indeces = new int[Count];
			int minWidth;
			bool finish;
			while(true) {
				finish = true;
				minWidth = int.MaxValue;
				for(int i = 0; i < indeces.Length; i++) {
					if(indeces[i] >= this[i].Count)
						continue;
					if(this[i][indeces[i]] == null)
						continue;
					finish = false;
					this[i][indeces[i]].UnionWidth++;
					if(minWidth > this[i][indeces[i]].AccWidth)
						minWidth = this[i][indeces[i]].AccWidth;
				}
				if(finish) 
					break;
				for(int i = 0; i < indeces.Length; i++) {
					if(indeces[i] >= this[i].Count)
						continue;
					if(this[i][indeces[i]] == null)
						continue;
					if(this[i][indeces[i]].AccWidth == minWidth)
						indeces[i]++;
				}
			}
			CalcCol();
			CalcTotalWidth();
		}
		protected void CalcVertParams() {
			totalHeight = 0;
			for(int i = 0; i < Count; i++) {
				this[i].MaxRowCount = 0;
				for(int j = 0; j < this[i].Count; j++) {
					if(this[i][j] != null) {
						if(this[i].MaxRowCount < this[i][j].Column.RowCount)	
							this[i].MaxRowCount = this[i][j].Column.RowCount;
					}				
				}
				totalHeight += this[i].MaxRowCount;
			}
		}
		public void Calculate() {			
			CalcHorzParams();
			CalcVertParams();
			EqualizeHorzParams();
		}
		public void CorrectWidth(int value_, bool isMasterView, bool isShowDetailButtons) {
			for(int i = 0; i < Count; i++) {
				if(this[i].Count > 0) {
					if(this[i][0] != null) {
						if(isMasterView && !isShowDetailButtons) {
							this[i][0].Col += value_- 1;
							this[i][0].UnionWidth++;
						} else 
							this[i][0].Col += value_;
						this[i][0].HeaderUnionWidth += value_;
					}
					for(int j = 1; j < this[i].Count; j++)
						if(this[i][j] != null) {
							this[i][j].Col += value_;
							this[i][j].HeaderCol += value_;
						}
				}
			}
		}
		public BandExportInfo OwnerBandInfo {
			get {
				return OwnerBandInfo;
			}
		}
		public int TotalHeight {
			get {
				return totalHeight;
			}
		}
		public int TotalWidth {
			get {
				return totalWidth;
			}
		}
		public ColumnExportInfoList this[int index] {
			get {
				return infos[index] as ColumnExportInfoList;
			}
		}
		public int Count {
			get {
				return infos.Count;
			}
		}
	}
	#endregion
}
