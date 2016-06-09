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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Frames;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Layout.Events;
using DevExpress.XtraGrid.Views.Layout.ViewInfo;
using DevExpress.XtraGrid.Views.Printing;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Printing;
using DevExpress.XtraPrinting;
namespace DevExpress.XtraGrid.Views.Layout.Printing {
	public class LayoutViewPrintInfo : ColumnViewPrintInfo {
		public LayoutViewPrintInfo(PrintInfoArgs args)
			: base(args) {
		}
		protected override bool PrintSelectedRowsOnly {
			get { return View.OptionsPrint.PrintSelectedCardsOnly; }
		}
		protected override void MakeRowList() {
			if(PrintSelectedRowsOnly) MakeSelectedRowList();
			else base.MakeRowList();
		}
		protected override BaseViewAppearanceCollection CreatePrintAppearance() {
			return new LayoutViewPrintAppearances(View);
		}
		public new LayoutViewPrintAppearances AppearancePrint {
			get { return base.AppearancePrint as LayoutViewPrintAppearances; }
		}
		public new LayoutView View {
			get { return base.View as LayoutView; }
		}
		protected new LayoutViewInfo ViewViewInfo {
			get { return base.ViewViewInfo as LayoutViewInfo; }
		}
		protected override bool AllowPrintFilterInfo {
			get { return View.OptionsPrint.PrintFilterInfo; }
		}
		int cardsHeight;
		public override void PrintRows(IBrickGraphics graph) {
			View.ViewInfo.GInfo.AddGraphics(null);
			try {
				PreparePrintRows();
				for(int n = 0; n < rowCount; n++) {
					if(CancelPending) break;
					PrintOneCardInRow(graph, n);
				}
				AfterPrintRows(graph);
			}
			finally { View.ViewInfo.GInfo.ReleaseGraphics(); }
		}
		protected int CalcColumnCardsCountByWidth(int gridRowCount, int width) {
			LayoutViewCard card = ViewViewInfo.InitializeCard(0);
			int n = Math.Max(width / card.Width, 1);
			return (n >= gridRowCount) ? gridRowCount : gridRowCount / n;
		}
		protected int CalcRowCardsCountByWidth(int gridRowCount, int width) {
			LayoutViewCard card = ViewViewInfo.InitializeCard(0);
			return Math.Max(width / card.Width, 1);
		}
		int iVerticalOffset;
		int iHorizontalOffset;
		int maxCardHeight;
		int maxCardWidth;
		int iNumCardsInRow;
		int iNumCardsInColumn;
		LayoutViewPrintMode mode;
		bool showCaptions;
		void PreparePrintRows() {
			if(!ViewViewInfo.IsReady) ViewViewInfo.CalcConstants();
			MakeRowList();
			rowCount = Rows.Count;
			cardsHeight = Indent;
			iVerticalOffset = Indent;
			iHorizontalOffset = Indent;
			maxCardHeight = 0;
			maxCardWidth = 0;
			iNumCardsInRow = rowCount / Math.Max(View.OptionsPrint.MaxCardRows, 1);
			iNumCardsInColumn = rowCount / Math.Max(View.OptionsPrint.MaxCardColumns, 1);
			PrintWrapper.BeginPrint();
			if(rowCount % Math.Max(View.OptionsPrint.MaxCardRows, 1) > 0) iNumCardsInRow++;
			if(rowCount % Math.Max(View.OptionsPrint.MaxCardColumns, 1) > 0) iNumCardsInColumn++;
			mode = View.OptionsPrint.PrintMode;
			if(mode == LayoutViewPrintMode.Default) mode = PrintModeHelper.FromViewMode(View.OptionsView.ViewMode);
			if(rowCount > 0) {
				if(mode == LayoutViewPrintMode.Default) {
					mode = LayoutViewPrintMode.MultiColumn;
					iNumCardsInColumn = CalcColumnCardsCountByWidth(rowCount, MaximumWidth);
				}
				else if(mode == LayoutViewPrintMode.MultiColumn && View.OptionsPrint.MaxCardColumns == 0) {
					iNumCardsInColumn = CalcColumnCardsCountByWidth(rowCount, MaximumWidth);
				}
				else if(mode == LayoutViewPrintMode.MultiRow && View.OptionsPrint.MaxCardRows == 0) {
					iNumCardsInRow = CalcRowCardsCountByWidth(rowCount, MaximumWidth);
				}
			}
			showCaptions = View.OptionsView.ShowCardCaption;
			PreparePrintAppearances();
		}
		void AfterPrintRows(IBrickGraphics graph) {
			View.OptionsView.ShowCardCaption = showCaptions;
			Y = cardsHeight;
			PrintFilterInfo(graph);
			PrintWrapper.EndPrint();
			CleanupPrintAppearances();
		}
		int rowCount = -1;
		int currentPrintedRow = -1;
		public override void PrintNextRow(XtraPrinting.IBrickGraphics graph) {
			if(rowCount < 0) {
				PreparePrintRows();
				currentPrintedRow = 0;
			}
			Y = 0;
			cardsHeight = iVerticalOffset = 0;
			PrintRowCore(graph, currentPrintedRow);
			currentPrintedRow += CardsPerRow();
			if(currentPrintedRow == rowCount)
				AfterPrintRows(graph);
		}
		protected int CardsPerRow() {
			return (iNumCardsInRow == rowCount ? 1 : iNumCardsInRow);
		}
		public override int GetDetailCount() {
			if(rowCount < 0) { PreparePrintRows(); currentPrintedRow = 0; }
			return rowCount / CardsPerRow() + rowCount % CardsPerRow();
		}
		void PrintRowCore(IBrickGraphics graph, int n) {
			for(int i = 0; i < CardsPerRow(); i++)
				PrintOneCardInRow(graph, n + i);
		}
		void PrintOneCardInRow(IBrickGraphics graph, int n) {
			LayoutViewCard card = ViewViewInfo.InitializeCard((int)Rows[n]);
			card.ViewInfo.Offset = card.Location = Point.Empty;
			card.UpdateChildrenToBeRefactored();
			ViewViewInfo.UpdateBeforePaint(card);
			double actualCardHeight = PrintLayoutCard(card, graph, new PointF(iHorizontalOffset, iVerticalOffset));
			int dh = Math.Max(card.Height, (int)Math.Ceiling(actualCardHeight));
			cardsHeight = Math.Max(cardsHeight, iVerticalOffset + dh);
			maxCardHeight = Math.Max(Math.Max(maxCardHeight, card.Height), (int)actualCardHeight);
			maxCardWidth = Math.Max(maxCardWidth, card.Width);
			switch(mode) {
				case LayoutViewPrintMode.Column:
					iVerticalOffset += dh;
					break;
				case LayoutViewPrintMode.Row:
					iHorizontalOffset += card.Width;
					break;
				case LayoutViewPrintMode.MultiColumn:
					if((n + 1) % iNumCardsInColumn == 0) {
						iHorizontalOffset += maxCardWidth;
						iVerticalOffset = Indent;
						maxCardWidth = 0;
					}
					else iVerticalOffset += maxCardHeight;
					break;
				case LayoutViewPrintMode.MultiRow:
					if((n + 1) % iNumCardsInRow == 0) {
						iVerticalOffset += maxCardHeight;
						iHorizontalOffset = Indent;
						maxCardHeight = 0;
					}
					else iHorizontalOffset += card.Width;
					break;
			}
		}
		BaseViewAppearanceCollection appearance;
		BaseViewPainter painter;
		Registrator.ViewPaintStyle paintStyle;
		protected void PreparePrintAppearances() {
			View.BeginUpdate();
			painter = View.Painter;
			paintStyle = View.ReplacePaint(new PrintLayoutViewPaintStyle(), new PrintLayoutViewPainter(View));
			ViewViewInfo.SetPaintAppearanceDirty();
			FrozenLayoutViewAppearances printAppearance = new FrozenLayoutViewAppearances(View.Appearance);
			if(View.OptionsPrint.UsePrintStyles) {
				printAppearance.Assign(AppearancePrint);
				printAppearance.CardCaption.Assign(AppearancePrint.CardCaption);
				printAppearance.FocusedCardCaption.Assign(AppearancePrint.CardCaption);
				printAppearance.SelectedCardCaption.Assign(AppearancePrint.CardCaption);
				printAppearance.HideSelectionCardCaption.Assign(AppearancePrint.CardCaption);
			}
			else {
				printAppearance.CardCaption.Assign(View.PaintAppearance.CardCaption);
				printAppearance.FocusedCardCaption.Assign(View.PaintAppearance.CardCaption);
				printAppearance.SelectedCardCaption.Assign(View.PaintAppearance.CardCaption);
				printAppearance.HideSelectionCardCaption.Assign(View.PaintAppearance.CardCaption);
			}
			printAppearance.ViewBackground.BackColor = printAppearance.ViewBackground.BackColor2 = Color.White;
			appearance = View.ReplaceAppearance(printAppearance);
			View.OptionsView.ShowCardCaption = View.OptionsPrint.PrintCardCaption;
			ViewViewInfo.SetPaintAppearanceDirty();
			View.CancelUpdate();
		}
		protected void CleanupPrintAppearances() {
			View.BeginUpdate();
			FrozenLayoutViewAppearances fAppearance = View.ReplaceAppearance(appearance) as FrozenLayoutViewAppearances;
			if(fAppearance != null)
				fAppearance.Dispose();
			Registrator.ViewPaintStyle printPaintStyle = View.ReplacePaint(paintStyle, painter);
			if(printPaintStyle != null)
				printPaintStyle.Dispose();
			View.ResetCardsCache();
			ViewViewInfo.SetPaintAppearanceDirty();
			View.EndUpdate();
		}
		protected double PrintLayoutCard(LayoutViewCard card, IBrickGraphics graph, PointF cardPos) {
			LayoutCardPrinter cardPrinter = CreateCardPrinter();
			cardPrinter.Initialize(PS, PrintWrapper.Link, card.ViewInfo);
			SetDefaultBrickStyle(graph, Bricks["Card"]);
			RectangleF rect = card.ViewInfo.BorderInfo.Bounds;
			rect.Offset(cardPos);
			UsePanelBrickDisableDrawing = true;
			IPanelBrick cardBrick = DrawBrick(graph, "PanelBrick", rect) as IPanelBrick;
			cardPrinter.PrintGroupEx(card, cardPos, graph);
			float bottomContent = 0;
			foreach(DevExpress.XtraGrid.Views.Layout.Printing.LayoutCardPrinter.BrickPrintInfo pbi in cardPrinter.PrintInfo) {
				pbi.Brick.Rect = new RectangleF(new PointF(pbi.BrickBounds.Left - cardPos.X, pbi.BrickBounds.Top - cardPos.Y), pbi.BrickBounds.Size);
				cardBrick.Bricks.Add(pbi.Brick);
				if(pbi.Brick.Rect.Bottom > bottomContent) bottomContent = pbi.Brick.Rect.Bottom;
			}
			if(cardBrick.Rect.Height < bottomContent) cardBrick.Rect = new RectangleF(cardBrick.Rect.Location, new SizeF(cardBrick.Rect.Width, bottomContent));
			graph.DrawBrick(cardBrick, cardBrick.Rect);
			UsePanelBrickDisableDrawing = false;
			return bottomContent;
		}
		protected virtual LayoutCardPrinter CreateCardPrinter() {
			return new LayoutCardPrinter(View);
		}
		public override void Initialize() {
			base.Initialize();
			Bricks.Add("Card", AppearancePrint.Card, BorderSide.All, AppearancePrint.CardCaption.BackColor, 2);
			Bricks.Add("CardCaption", AppearancePrint.CardCaption);
			Bricks.Add("FieldCaption", AppearancePrint.FieldCaption);
			Bricks.Add("FieldValue", AppearancePrint.FieldValue);
			CreateInfo();
		}
		protected override void UpdateAppearances() {
			base.UpdateAppearances();
			if(!View.OptionsPrint.UsePrintStyles) {
				AppearanceDefaultInfo[] info = View.BaseInfo.GetDefaultPrintAppearance();
				AppearancePrint.Combine(ViewViewInfo.PaintAppearance, info);
				if(View.IsSkinned) {
					AppearancePrint.CardCaption.Assign(Find("CardCaption", info));
					AppearancePrint.FilterPanel.Assign(Find("FilterPanel", info));
				}
			}
		}
		protected virtual void CreateInfo() {
			if(Graph == null) return;
			View.RefreshVisibleColumnsList();
		}
		#region Internal Classes
		class FrozenLayoutViewAppearances : LayoutViewAppearances {
			public FrozenLayoutViewAppearances(LayoutViewAppearances appearances)
				: base(null) {
				AssignInternal(appearances);
			}
			public override bool IsLoading { get { return true; } }
			protected override void Subscribe(AppearanceObject appearance) { }
			protected override void Unsubscribe(AppearanceObject appearance) { }
		}
		class PrintLayoutViewPaintStyle : Registrator.LayoutViewPaintStyle { }
		class PrintLayoutViewPainter : Drawing.LayoutViewPainter {
			public PrintLayoutViewPainter(LayoutView view)
				: base(view) {
			}
			protected override Drawing.LayoutViewElementsPainter CreateElementsPainter(BaseView view) {
				return new Drawing.LayoutViewElementsPainter(view);
			}
		}
		#endregion Internal Classes
	}
	public class LayoutCardPrinter : LayoutControlPrinter {
		public class BrickPrintInfo {
			public BrickPrintInfo(IVisualBrick brick, RectangleF rect) {
				brickBounsCore = rect;
				brickCore = brick;
			}
			private RectangleF brickBounsCore;
			public RectangleF BrickBounds {
				get { return brickBounsCore; }
				set { brickBounsCore = value; }
			}
			private IVisualBrick brickCore;
			public IVisualBrick Brick {
				get { return brickCore; }
				set { brickCore = value; }
			}
		}
		LayoutView viewCore;
		public LayoutCardPrinter(LayoutView ownerView)
			: base(null) {
			viewCore = ownerView;
			printinfoCore = new List<BrickPrintInfo>();
		}
		protected LayoutView View {
			get { return viewCore; }
		}
		private List<BrickPrintInfo> printinfoCore;
		public List<BrickPrintInfo> PrintInfo {
			get { return printinfoCore; }
		}
		protected override void DrawBrick(IVisualBrick brick, RectangleF rect) {
			rect = new RectangleF(rect.X + globalX, rect.Y + globalY, rect.Width, rect.Height);
			PrintInfo.Add(new BrickPrintInfo(brick, rect));
		}
		protected override void SelectTabPage(TabbedGroup tGroup, LayoutGroup tabPage) {
			base.SelectTabPage(tGroup, tabPage);
			UpdateChildren(tGroup, true);
		}
		protected override void ProcessGroupCaption(LayoutGroup group, ref string text, ref AppearanceObject textAppearance) {
			LayoutViewCard card = group as LayoutViewCard;
			if(card != null) {
				GroupCaptionCustomDrawEventArgs e = new GroupCaptionCustomDrawEventArgs(
					View.ViewInfo.GInfo.Cache, null, group.ViewInfo.BorderInfo);
				LayoutViewCustomDrawCardCaptionEventArgs ea = new LayoutViewCustomDrawCardCaptionEventArgs(
					card.RowHandle, e);
				View.RaiseCustomDrawCardCaption(ea);
				text = ea.Info.Caption;
				textAppearance = ea.Info.AppearanceCaption;
			}
			else base.ProcessGroupCaption(group, ref text, ref textAppearance);
		}
		protected override void PrintControlArea(LayoutControlItem citem) {
			LayoutViewField field = citem as LayoutViewField;
			if(field != null) {
				LayoutViewFieldInfo fieldInfo = field.ViewInfo as LayoutViewFieldInfo;
				BaseEditViewInfo editorInfo = fieldInfo.RepositoryItemViewInfo;
				PrintCellHelperInfo pcinfo = new PrintCellHelperInfo(Color.Empty, ps,
						editorInfo.EditValue,
						editorInfo.PaintAppearance,
						editorInfo.DisplayText,
						fieldInfo.ClientAreaRelativeToControl, graph
					);
				IVisualBrick valueBrick = field.RepositoryItem.GetBrick(pcinfo);
				valueBrick.Sides = BorderSide.None;
				DrawBrick(valueBrick, fieldInfo.ClientAreaRelativeToControl);
				return;
			}
			base.PrintControlArea(citem);
		}
		protected override RectangleF GetGroupCaptionRect(LayoutGroup group, RectangleF captionRect) {
			if(group is LayoutViewCard) {
				captionRect = (RectangleF)group.ViewInfo.BorderInfo.CaptionBounds;
			}
			return captionRect;
		}
	}
}
