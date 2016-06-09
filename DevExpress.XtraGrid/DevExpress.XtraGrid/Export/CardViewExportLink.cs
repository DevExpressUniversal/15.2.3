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
using DevExpress.XtraExport;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Card;
using DevExpress.XtraGrid.Views.Card.ViewInfo;
using DevExpress.XtraGrid.Columns;
using DevExpress.Data;
namespace DevExpress.XtraGrid.Export {
	public class CardViewExportLink : BaseExportLink	{
		protected int fCacheHeight;
		protected int fCacheWidth;
		protected int fCardRows = 3;
		int realCardRows = -1;
		int cardColumns = 0;
		int rowCount;
		ArrayList rows;
		protected int fEmptySpaceStyle;
		protected int fCardCaptionStyle;
		protected int fColumnCaptionStyle;
		protected int fRowContentStyle;
		protected int fFirstColumnCaptionStyle;
		protected int fFirstRowContentStyle;
		protected int fLastColumnCaptionStyle;
		protected int fLastRowContentStyle;
		protected int fLeftSeparatorStyle;
		protected int fRightSeparatorStyle;
		public CardViewExportLink(BaseView view, IExportProvider provider): 
			base(view, provider) {
			rows = new ArrayList();
		}
		protected Color GetCardBorderColor() {			
			return ExportAppearance.Card.BorderColor;
		}
		protected CardViewInfo ViewInfo { get { return View.ViewInfo as CardViewInfo; } }
		public new CardViewAppearances ExportAppearance { 
			get { return base.ExportAppearance as CardViewAppearances; }
		}
		protected override void UpdateAppearanceScheme(BaseAppearanceCollection coll) {
			if(View.PaintStyle== null) return;
			CardViewAppearances res = coll as CardViewAppearances;
			if(View.PaintStyle.IsSkin) {
				res.Combine(res, View.BaseInfo.PaintStyles["Flat"].GetAppearanceDefaultInfo(View));
			}
		}
		protected override BaseAppearanceCollection CreateAppearanceCollectionInstance() {
			return new CardViewAppearances(View); 
		}
		protected Color GetCardSeparatorColor() {
			return ExportAppearance.SeparatorLine.BackColor;
		}
		protected ExportCacheCellStyle GetEmptySpaceStyle() {
			return GridStyleToExportStyle(ExportAppearance.EmptySpace, null, null);
		}
		protected ExportCacheCellStyle GetCardCaptionStyle() {
			ExportCacheCellStyle result = 
				GridStyleToExportStyle(ExportAppearance.CardCaption, null, null);
			result.LeftBorder.Width = 1;
			result.TopBorder.Width = 1;
			result.RightBorder.Width = 1;
			result.BottomBorder.Width = 1;
			result.LeftBorder.Color_ = GetCardBorderColor();
			result.TopBorder.Color_ = GetCardBorderColor();
			result.RightBorder.Color_ = GetCardBorderColor();
			result.BottomBorder.Color_ = GetCardBorderColor();
			return result;
		}
		protected ExportCacheCellStyle GetColumnCaptionStyle() {
			ExportCacheCellStyle result = 
				GridStyleToExportStyle(ExportAppearance.FieldCaption, null, null);
			result.LeftBorder.Width = 1;
			result.LeftBorder.Color_ = GetCardBorderColor();
			return result;
		}
		protected ExportCacheCellStyle GetRowContentStyle_() {
			ExportCacheCellStyle result = 
				GridStyleToExportStyle(ExportAppearance.FieldValue, null, null);
			result.RightBorder.Width = 1;
			result.LeftBorder.Width = 1;
			result.TopBorder.Width = 1;
			result.BottomBorder.Width = 1;
			return result;
		}
		protected ExportCacheCellStyle GetRowContentStyle() {
			ExportCacheCellStyle result = GetRowContentStyle_();
			result.LeftBorder.Color_ = Provider.GetStyle(fColumnCaptionStyle).BkColor;
			result.TopBorder.Color_ = Provider.GetStyle(fColumnCaptionStyle).BkColor;
			result.BottomBorder.Color_ = Provider.GetStyle(fColumnCaptionStyle).BkColor;
			result.RightBorder.Color_ = GetCardBorderColor();
			return result;
		}
		protected ExportCacheCellStyle GetFirstColumnCaptionStyle() {
			ExportCacheCellStyle result = GetColumnCaptionStyle();
			result.TopBorder.Width = 1;
			result.TopBorder.Color_ = GetCardBorderColor();
			return result;
		}
		protected ExportCacheCellStyle GetFirstRowContentStyle() {
			ExportCacheCellStyle result = GetRowContentStyle_();
			result.TopBorder.Color_ = GetCardBorderColor();
			result.LeftBorder.Color_ = Provider.GetStyle(fColumnCaptionStyle).BkColor;
			result.BottomBorder.Color_ = Provider.GetStyle(fColumnCaptionStyle).BkColor;
			result.RightBorder.Color_ = GetCardBorderColor();
			return result;
		}
		protected ExportCacheCellStyle GetLastColumnCaptionStyle() {
			ExportCacheCellStyle result = GetColumnCaptionStyle();
			result.BottomBorder.Width = 1;
			result.BottomBorder.Color_ = GetCardBorderColor();
			return result;
		}
		protected ExportCacheCellStyle GetLastRowContentStyle() {
			ExportCacheCellStyle result = GetRowContentStyle_();
			result.LeftBorder.Color_ = Provider.GetStyle(fColumnCaptionStyle).BkColor;
			result.TopBorder.Color_ = Provider.GetStyle(fColumnCaptionStyle).BkColor;
			result.RightBorder.Color_ = GetCardBorderColor();
			result.BottomBorder.Color_ = GetCardBorderColor();
			return result;
		}
		protected ExportCacheCellStyle GetLeftSeparatorStyle() {
			ExportCacheCellStyle result = GetEmptySpaceStyle();
			result.RightBorder.Width = 1;
			result.RightBorder.Color_ = GetCardSeparatorColor();
			return result;
		}
		protected ExportCacheCellStyle GetRightSeparatorStyle() {
			ExportCacheCellStyle result = GetEmptySpaceStyle();
			result.LeftBorder.Width = 1;
			result.LeftBorder.Color_ = GetCardSeparatorColor();
			return result;
		}
		private void RegisterStyles() {
			fEmptySpaceStyle = Provider.RegisterStyle(GetEmptySpaceStyle());
			fCardCaptionStyle = Provider.RegisterStyle(GetCardCaptionStyle());
			fColumnCaptionStyle = Provider.RegisterStyle(GetColumnCaptionStyle());
			fRowContentStyle = Provider.RegisterStyle(GetRowContentStyle());
			fFirstColumnCaptionStyle = Provider.RegisterStyle(GetFirstColumnCaptionStyle());
			fFirstRowContentStyle = Provider.RegisterStyle(GetFirstRowContentStyle());
			fLastColumnCaptionStyle = Provider.RegisterStyle(GetLastColumnCaptionStyle());
			fLastRowContentStyle = Provider.RegisterStyle(GetLastRowContentStyle());
			fLeftSeparatorStyle = Provider.RegisterStyle(GetLeftSeparatorStyle());
			fRightSeparatorStyle  = Provider.RegisterStyle(GetRightSeparatorStyle());
		}
		protected void FillEmptySpace() {
			for(int i = 0; i < fCacheWidth; i++)
				for(int j = 0; j < fCacheHeight; j++)
					Provider.SetCellStyle(i, j, fEmptySpaceStyle);
		}
		protected bool IsShowCardCaption() {
			return View.OptionsView.ShowCardCaption;
		}
		protected bool IsShowLines() {
			return View.OptionsView.ShowLines;
		}
		protected void CalcCacheBounds() {
			if(fExportAll)
				MakeRowList();
			else
				MakeSelectedRowList();
			rowCount = Math.Min(rows.Count, View.RowCount);
			CalculateRealCardRows();
			CalculateCardColumns();			
			CalculateCacheHeight();
			CalculateCacheWidth();		
		}
		protected void CalculateCacheHeight() {
			fCacheHeight = 0;
			fCacheHeight += (View.VisibleColumns.Count + 1) * realCardRows + 1;
			if(IsShowCardCaption())
				fCacheHeight += realCardRows;
		}
		protected void CalculateCacheWidth() {
			if(IsShowLines())
				fCacheWidth = cardColumns * 4;
			else
				fCacheWidth = cardColumns * 3 + 1;
		}
		protected void CalculateRealCardRows() {
			if(fCardRows > rowCount)
				realCardRows = rowCount;
			else
				realCardRows = fCardRows;
		}
		protected void CalculateCardColumns() {
			if(realCardRows > 0) {
				cardColumns = rowCount / realCardRows; 
				int remainder = rowCount % realCardRows;
				if(remainder > 0)
					cardColumns++;
			}					
		}
		protected void SetCardWidth() {
			if(IsShowLines()) {
				for(int i = 1; i < fCacheWidth - 2; i += 4)
					Provider.SetColumnWidth(i, View.CardWidth / 2);
				for(int i = 2; i < fCacheWidth - 1; i += 4)
					Provider.SetColumnWidth(i, View.CardWidth / 2);
				for(int i = 0; i < fCacheWidth; i += 4) {
					Provider.SetColumnWidth(i, View.CardInterval);
					if(i + 3 < fCacheWidth)
						Provider.SetColumnWidth(i + 3, View.CardInterval);
				}
			} else {
				for(int i = 1; i < fCacheWidth - 2; i += 3)
					Provider.SetColumnWidth(i, View.CardWidth / 2);			
				for(int i = 2; i < fCacheWidth - 1; i += 3)
					Provider.SetColumnWidth(i, View.CardWidth / 2);
				for(int i = 0; i < fCacheWidth; i += 3) 
					Provider.SetColumnWidth(i, View.CardInterval);
			}
		}
		protected void MakeRowList() {
			rows.Clear();
			VisibleIndexCollection indexes = new VisibleIndexCollection(View.DataController, View.DataController.GroupInfo);
			indexes.BuildVisibleIndexes(View.DataController.VisibleListSourceRowCount, true, false);
			rows.AddRange(indexes);
		}
		protected void MakeSelectedRowList() {
			rows.Clear();
			if(View.IsMultiSelect)
				rows.AddRange(View.CardSelection.GetNormalizedSelectedRows());
			else
				rows.Add(View.FocusedRowHandle);
		}
		protected void NextCard(ref int leftColumn, ref int topRow) {
			int topRow_ = topRow;
			int leftColumn_ = leftColumn;
			topRow_ += View.VisibleColumns.Count + 1;
			if(IsShowCardCaption())
				topRow_++;
			if(topRow_ < fCacheHeight - 1) {
				topRow = topRow_;
			} else {
				if(IsShowLines())
					leftColumn_ += 4;
				else
					leftColumn_ += 3;
				if(leftColumn_ < fCacheWidth - 1) {
					topRow = 1;
					leftColumn = leftColumn_;
				}
			}
		}
		protected bool IsMostRightCard(int leftColumn) {
			return (leftColumn + 4) > fCacheWidth;
		}
		protected bool IsMostBottomCard(int topRow) {
			int finalRow = topRow + View.VisibleColumns.Count + 1;
			if(IsShowCardCaption())
				finalRow++;
			return finalRow > (fCacheHeight - 1);
		}
		protected virtual void LoadCache() {
			OnProgress(0, ExportPhase.Link);
			int leftColumn = 1;
			int topRow = 1;
			for(int i = 0; i < rows.Count; i++) {
				if(IsShowCardCaption())
					LoadCardCaptionContent((int)rows[i], leftColumn, topRow);
				LoadCardContent((int)rows[i], leftColumn, topRow);
				if(IsShowLines())
					if(!IsMostRightCard(leftColumn))
						LoadCardSeparator(leftColumn, topRow);
				NextCard(ref leftColumn, ref topRow);
				if(rows.Count > 1)
					OnProgress(i * 100 / (rows.Count - 1), ExportPhase.Link);
			}
			OnProgress(100, ExportPhase.Link);
		}
		protected virtual void LoadCardCaptionContent(int handle, int leftColumn, int topRow) {
			Provider.SetCellStyleAndUnion(leftColumn, topRow, 2, 1, fCardCaptionStyle);
			Provider.SetCellString(leftColumn, topRow, View.GetCardCaption(handle));
		}
		protected virtual void LoadCardContent(int handle, int leftColumn, int topRow) {
			if(IsShowCardCaption())
				topRow++;
			for(int i = 0; i < View.VisibleColumns.Count; i++) {
				Provider.SetCellString(leftColumn, topRow, 
					View.GetVisibleColumn(i).GetTextCaption());
				if(ExportCellsAsDisplayText)
					Provider.SetCellString(leftColumn + 1, topRow, View.GetRowCellDisplayText(handle, View.GetVisibleColumn(i)));
				else
					Provider.SetCellData(leftColumn + 1, topRow, View.GetRowCellValue(handle, View.GetVisibleColumn(i)));
				if(i == View.VisibleColumns.Count - 1) {
					Provider.SetCellStyle(leftColumn, topRow, fLastColumnCaptionStyle);
					Provider.SetCellStyle(leftColumn + 1, topRow, fLastRowContentStyle);
				} else if(i == 0) {
					if(IsShowCardCaption()) {
						Provider.SetCellStyle(leftColumn, topRow, fColumnCaptionStyle);
						Provider.SetCellStyle(leftColumn + 1, topRow, fRowContentStyle);
					} else {
						Provider.SetCellStyle(leftColumn, topRow, fFirstColumnCaptionStyle);
						Provider.SetCellStyle(leftColumn + 1, topRow, fFirstRowContentStyle);
					}
				}
				else {
					Provider.SetCellStyle(leftColumn, topRow, fColumnCaptionStyle);
					Provider.SetCellStyle(leftColumn + 1, topRow, fRowContentStyle);
				}
				topRow++;
			}
		}
		protected virtual void LoadCardSeparator(int leftColumn, int topRow) {
			int finalRow = topRow + View.VisibleColumns.Count;
			if(IsShowCardCaption())
				finalRow++;
			for(int i = topRow; i < finalRow; i++) {
				Provider.SetCellStyle(leftColumn + 2, i, fLeftSeparatorStyle);
				Provider.SetCellStyle(leftColumn + 3, i, fRightSeparatorStyle);
			}
			if(!IsMostBottomCard(topRow)) {
				Provider.SetCellStyle(leftColumn + 2, finalRow, fLeftSeparatorStyle);
				Provider.SetCellStyle(leftColumn + 3, finalRow, fRightSeparatorStyle);
			}
		}
		protected override int GetCacheWidth() {
			return fCacheWidth;
		}
		protected override int GetCacheHeight() {
			return fCacheHeight;
		}
		protected override ExportCacheCellStyle GetDefaultStyle() {
			ExportCacheCellStyle result = Provider.GetDefaultStyle();
			return result;
		}
		protected override bool TestView() {
			return fView is CardView;
		}
		protected override void DoExport() {
			if(View == null) return;
			CalcCacheBounds();
			if(fCacheHeight > 0 && fCacheWidth > 0) {
				Provider.SetRange(fCacheWidth, fCacheHeight, false);
				SetCardWidth();
				RegisterStyles();
				FillEmptySpace();
				LoadCache();
			}					
		}
		public override void Copy(BaseExportLink link) {
			base.Copy(link);
			if(link is CardViewExportLink) 
				fCardRows = ((CardViewExportLink)link).CardRows;
		}
		public new CardView View {
			get {
				return (CardView)fView;
			}
		}
		public int CardRows {
			get {
				return fCardRows;
			}
			set {
				if(value > 0)
					fCardRows = value;
			}
		}
	}
}
