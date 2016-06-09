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
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Drawing;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Scrolling;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Card;
using DevExpress.XtraGrid.Views.Card.ViewInfo;
using DevExpress.XtraPrinting;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraGrid.Views.Printing;
namespace DevExpress.XtraGrid.Views.Printing {
	public class CardViewPrintInfo : ColumnViewPrintInfo {
		const int cardInterval = 13, cardYInterval = 13;
		int cardCaptionHeight, cardWidth, cardColumnsCount, cardFieldCaptionWidth;
		public CardViewPrintInfo(PrintInfoArgs args) : base(args) { }
		protected override BaseViewAppearanceCollection CreatePrintAppearance() { return new CardViewPrintAppearances(View);	}
		public new CardViewPrintAppearances AppearancePrint { get { return base.AppearancePrint as CardViewPrintAppearances; } }
		public new CardView View { get { return base.View as CardView; } }
		protected new CardViewInfo ViewViewInfo { get { return base.ViewViewInfo as CardViewInfo; } }
		protected override bool PrintSelectedRowsOnly { get { return View.OptionsPrint.PrintSelectedCardsOnly; } }
		int rowCount = -1;
		int currentPrintedRow = -1;
		public override void PrintNextRow(DevExpress.XtraPrinting.IBrickGraphics graph) {
			if(rowCount < 0) { PreparePrintRows(out rowCount); currentPrintedRow = 0; }
			Y = 0;
			PrintRowCore(graph, currentPrintedRow);
			currentPrintedRow += cardColumnsCount;
			if(currentPrintedRow == rowCount) {
				AfterPrintRows(graph);
			}
		}
		public override int GetDetailCount() {
			if(rowCount < 0) { PreparePrintRows(out rowCount); currentPrintedRow = 0; }
			return rowCount / cardColumnsCount + rowCount % cardColumnsCount;
		}
		int offset;
		int currentCardIndex, maxHeight;
		bool yReady;
		public override void PrintRows(DevExpress.XtraPrinting.IBrickGraphics graph) {
			PreparePrintRows(out rowCount);
			for(int n = 0; n < rowCount; n++) {
				if(CancelPending) break;
				PrintOneCardInRow(graph, n);
			}
			AfterPrintRows(graph);
		}
		private void AfterPrintRows(DevExpress.XtraPrinting.IBrickGraphics graph) {
			if(!yReady) Y += maxHeight + cardYInterval;
			PrintFilterInfo(graph);
			PrintWrapper.EndPrint();
		}
		private void PrintRowCore(DevExpress.XtraPrinting.IBrickGraphics graph, int n) {
			for(int i = 0; i < cardColumnsCount; i++) {
				PrintOneCardInRow(graph, n + i);
			}
		}
		private void PrintOneCardInRow(DevExpress.XtraPrinting.IBrickGraphics graph, int n) {
			yReady = false;
			int rh = (int)Rows[n];
			maxHeight = Math.Max(PrintCard(graph, rh, currentCardIndex, offset), maxHeight);
			offset += cardWidth + cardInterval;
			currentCardIndex++;
			if(currentCardIndex >= cardColumnsCount) {
				Y += maxHeight + cardYInterval;
				offset = Indent;
				currentCardIndex = 0;
				yReady = true;
				maxHeight = 0;
			}
		}
		protected override void MakeRowList() {
			if(PrintSelectedRowsOnly) { MakeSelectedRowList(); return; }
			base.MakeRowList();
		}
		void PreparePrintRows(out int rowCount) {
			offset = Indent;
			currentCardIndex = 0;
			maxHeight = 0;
			yReady = false;
			if(!ViewViewInfo.IsReady) ViewViewInfo.CalcConstants();
			MakeRowList();
			rowCount = Rows.Count;
			PrintWrapper.BeginPrint();
		}
		CardInfo CreateCardInfo(int rowHandle, int offset) {
			CardInfo ci = new CardInfo(ViewViewInfo);
			ci.RowHandle = rowHandle;
			ci.Bounds = new Rectangle(offset, Y, this.cardWidth, 100);
			ci.ClientBounds = Rectangle.Inflate(ci.Bounds, -1, -1);
			if(View.OptionsPrint.PrintCardCaption) {
				Rectangle caption = Rectangle.Inflate(ci.Bounds, -1, -1);
				caption.Height = this.cardCaptionHeight;
				ci.ClientBounds.Y = caption.Bottom;
				ci.CaptionInfo.Bounds = caption;
				ci.CaptionInfo.CardCaption = View.GetCardCaption(rowHandle);
			}
			return ci;
		}
		protected virtual int PrintCard(IBrickGraphics graph, int rowHandle, int cardIndex, int offset) {
			bool canScroll = false;
			CardInfo ci = CreateCardInfo(rowHandle, offset);
			CardCaptionCustomDrawEventArgs cs = new CardCaptionCustomDrawEventArgs(ViewViewInfo.GInfo.Cache, ci, ViewViewInfo.PaintAppearance.CardCaption);
			View.RaiseCustomDrawCardCaption(cs);
			int height = ViewViewInfo.CalcCardFieldsInfo(ci, ci.ClientBounds.Top, 9999, ref canScroll);
			ci.ClientBounds.Height = height;
			ci.Bounds.Height = (ci.ClientBounds.Bottom - ci.Bounds.Top) + 1;
			ViewViewInfo.UpdateBeforePaint(ci);
			SetDefaultBrickStyle(graph, Bricks["Card"]);
			IPanelBrick brick = (IPanelBrick)DrawBrick(graph, "PanelBrick", ci.Bounds);
			IList col = brick.Bricks;
			UsePanelBrickDisableDrawing = true;
			SetDefaultBrickStyle(graph, Bricks["CardCaption"]);
			Rectangle textRectangle = new Rectangle(new Point(ci.CaptionInfo.Bounds.X - ci.Bounds.X, ci.CaptionInfo.Bounds.Y - ci.Bounds.Y), ci.CaptionInfo.Bounds.Size);
			ITextBrick textBrick = DrawTextBrick(graph, ci.CaptionInfo.CardCaption,  textRectangle, false);
			col.Add(textBrick);
			foreach(CardFieldInfo fi in ci.Fields) {
				ViewViewInfo.UpdateFieldAppearance(fi);
				ViewViewInfo.RequestFieldEditViewInfo(fi);
				if(fi.CaptionBounds.Width > 0) {
					SetDefaultBrickStyle(graph, Bricks["FieldCaption"]);
					fi.CaptionBounds.Inflate(0, 1);
					fi.CaptionBounds.X--; fi.CaptionBounds.Width++;
					Rectangle rectangle = new Rectangle(new Point(fi.CaptionBounds.X - ci.Bounds.X, fi.CaptionBounds.Y - ci.Bounds.Y), fi.CaptionBounds.Size);
					textBrick = DrawTextBrick(graph, fi.FieldCaption,  rectangle, false);
					col.Add(textBrick);
				}
				AppearanceObject valueAppearance = fi.PaintAppearanceValue;
				if(View.OptionsPrint.UsePrintStyles) valueAppearance = AppearancePrint.FieldValue;
				IVisualBrick visualBrick = fi.Editor.GetBrick(
					new PrintCellHelperInfo(
					LineColor,
					PS,
					fi.EditViewInfo.EditValue, valueAppearance,
					fi.EditViewInfo.DisplayText,
					fi.ValueBounds,
					Graph,
					View.GetRowCellDefaultAlignment(rowHandle, fi.Column, fi.PaintAppearanceValue.HAlignment),
					false,
					false
					));
				visualBrick.Rect = new RectangleF(new Point(fi.ValueBounds.X - ci.Bounds.X, fi.ValueBounds.Y - ci.Bounds.Y), fi.ValueBounds.Size);
				col.Add(visualBrick);
			}
			ci.Dispose();
			UsePanelBrickDisableDrawing = false;
			return ci.Bounds.Height;
		}
		protected virtual void CreateInfo() {
			if(Graph == null) return;
			View.RefreshVisibleColumnsList();
			cardCaptionHeight = CalcStyleHeight(AppearancePrint.CardCaption) + 4;
			cardFieldCaptionWidth = ViewViewInfo.CalcCardFieldCaptionWidth(AppearancePrint.CardCaption);
			int maxWidth = MaximumWidth;
			cardColumnsCount = 1;
			if(View.PrintMaximumCardColumns == -1) {
				if(!View.OptionsPrint.AutoHorzWidth)
					cardColumnsCount = -1;
			} else {
				cardColumnsCount = View.PrintMaximumCardColumns;
			}
			cardWidth = View.CardWidth;
			if(cardWidth > maxWidth) cardWidth = maxWidth;
			if(!View.OptionsPrint.AutoHorzWidth) {
				int width = 0, maxCards = 0;
				for(int n = 0;; n++) {
					width += cardWidth;
					if(width > maxWidth) {
						maxCards = n;
						break;
					}
					width += cardInterval;
				}
				if(maxCards < 1) maxCards = 1;
				if(cardColumnsCount == -1 || maxCards < cardColumnsCount) 
					cardColumnsCount = maxCards;
			} else {
				if(cardColumnsCount < 2) {
					cardColumnsCount = 1;
					cardWidth = maxWidth;
				} else {
					cardWidth = (maxWidth / cardColumnsCount) - cardInterval;
				}
			}
		}
		protected override Color LineColor { get { return AppearancePrint.Card.BorderColor == Color.Empty ? Color.Gray : AppearancePrint.Card.BorderColor; } }
		protected override bool AllowPrintFilterInfo { get { return View.OptionsPrint.PrintFilterInfo; } }
		public override void Initialize() {
			base.Initialize();
			Bricks.Add("Card", AppearancePrint.Card, BorderSide.All, AppearancePrint.Card.BorderColor, 1);
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
	}
}
