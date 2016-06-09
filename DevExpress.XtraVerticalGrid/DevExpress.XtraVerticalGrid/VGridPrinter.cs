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
using System.Text;
using System.Drawing;
using DevExpress.XtraPrinting;
using DevExpress.XtraVerticalGrid.ViewInfo;
using DevExpress.XtraEditors;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraVerticalGrid.Events;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraVerticalGrid.Rows;
using System.Collections.Generic;
namespace DevExpress.XtraVerticalGrid {
	public class BasePrintInfo : RepositoryItem {
		RowViewInfoReadOnlyCollection rowsViewInfo;
		public BasePrintInfo(BaseViewInfo viewInfo) {
			ViewInfo = viewInfo;
		}
		protected int X { get; set; }
		protected int Y { get; set; }
		protected int CurrentRowHeight { get; set; }
		protected IPrintingSystem PrintingSystem { get { return ViewInfo.Grid.PrintingSystem; } }
		protected IBrickGraphics PGraphics { get { return ViewInfo.Grid.curentPGraph; } }
		protected VGridControlBase Grid { get { return ViewInfo.Grid; } }
		protected BaseViewInfo ViewInfo { get; private set; }
		protected RowViewInfoReadOnlyCollection RowsViewInfo {
			get {
				if(rowsViewInfo == null)
					rowsViewInfo = CreateRowViewInfos();
				return rowsViewInfo;
			}
		}
		BorderSide FirstCellBorderSideMask { get { return BorderSide.Left | BorderSide.Top | BorderSide.Bottom; } }
		BorderSide LastCellBorderSideMask { get { return BorderSide.Top | BorderSide.Right | BorderSide.Bottom; } }
		BorderSide MiddleCellBorderSideMask { get { return BorderSide.Top | BorderSide.Bottom; } }
		bool PrintLineSeparator { get; set; }
		public virtual void PrintHeader(IBrickGraphics graph) { }
		public void PrintRows() {
			Y = 0;
			foreach(BaseRowViewInfo ri in RowsViewInfo)
				PrintRow(ri);
		}
		public virtual void PrintFooter(IBrickGraphics graph) { }
		BorderSide GetBorderSideMask(RowCellInfo cellInfo) {
			if(IsCellInMultiEditorRow(cellInfo)) {
				if(IsLastCell(cellInfo)) {
					return LastCellBorderSideMask;
				}
				if(IsFirstCell(cellInfo)) {
					return FirstCellBorderSideMask;
				}
				return MiddleCellBorderSideMask;
			}
			return BorderSide.All;
		}
		bool IsCellInMultiEditorRow(RowCellInfo cellInfo) {
			return 1 < cellInfo.Row.RowPropertiesCount;
		}
		bool IsFirstCell(RowCellInfo cellInfo) {
			return cellInfo.RowCellIndex == 0;
		}
		bool IsLastCell(RowCellInfo cellInfo) {
			return cellInfo.RowCellIndex == cellInfo.Row.RowPropertiesCount - 1;
		}
		Rectangle IncreaseSize(Rectangle rect) {
			return new Rectangle(rect.Location, new Size(rect.Width + 1, rect.Height + ViewInfo.RC.HorzLineWidth));
		}
		void PrintRow(BaseRowViewInfo rowInfo) {
			CurrentRowHeight = rowInfo.HeaderInfo.HeaderRect.Height;
			X = rowInfo.Row.Level == 0 ? 0 : rowInfo.HeaderInfo.IndentBounds.Width - ViewInfo.RowIndentWidth;
			PrintRowHeaderCore(rowInfo);
			MultiEditorRowViewInfo meri = rowInfo as MultiEditorRowViewInfo;
			for(int i = 0; i < rowInfo.ValuesInfo.Count; i++) {
				PrintRowValueCell(rowInfo.ValuesInfo[i]);
				PrintSeparator(meri, rowInfo.ValuesInfo[i], false);
			}
			Y += CurrentRowHeight + ViewInfo.RC.HorzLineWidth;
		}
		internal void PrintRowValueCell(RowValueInfo rowValueInfo) {
			PrintRowValueCellCore(rowValueInfo);
		}
		protected virtual void PrintSeparator(MultiEditorRowViewInfo rowInfo, RowCellInfo cellInfo, bool isHeader) {
			if(rowInfo == null ||
				rowInfo.SeparatorRects.Count <= cellInfo.RowCellIndex ||
				IsLastCell(cellInfo))
				return;
			if(rowInfo.HeaderInfo.SeparatorInfo.SeparatorKind != SeparatorKind.String && ViewInfo.Grid.OptionsView.ShowVertLines) {
				PrintLineSeparator = true;
				return;
			}
			Rectangle separatorInfoRectangle = (Rectangle)rowInfo.SeparatorRects[cellInfo.RowCellIndex];
			Rectangle separatorRectangle = new Rectangle(X, Y, separatorInfoRectangle.Width, separatorInfoRectangle.Height);
			CustomDrawSeparatorEventArgs eventArgs = new CustomDrawSeparatorEventArgs(null,
				separatorRectangle,
				cellInfo.Style.Clone() as AppearanceObject,
				(MultiEditorRow)rowInfo.Row);
			eventArgs.Init(rowInfo.HeaderInfo.SeparatorInfo, cellInfo.RowCellIndex, isHeader);
			PrintSeparatorCore(eventArgs);
		}
		protected virtual void PrintRowValueCellCore(RowValueInfo rowValueInfo) {
			BaseEditViewInfo editViewInfo = rowValueInfo.EditorViewInfo;
			Rectangle valueRectangle = IncreaseSize(rowValueInfo.Bounds);
			Rectangle bounds = new Rectangle(X, Y, valueRectangle.Width, valueRectangle.Height);
			X += valueRectangle.Width;
			PrintCellHelperInfo info = new PrintCellHelperInfo(
				Color.Black,
				PrintingSystem,
				editViewInfo.EditValue,
				editViewInfo.Appearance,
				editViewInfo.DisplayText,
				bounds,
				PGraphics,
				editViewInfo.Appearance.HAlignment,
				Grid.OptionsView.ShowHorzLines,
				Grid.OptionsView.ShowVertLines
			);
			IVisualBrick brick = rowValueInfo.Item.GetBrick(info);
			brick.Sides &= GetBorderSideMask(rowValueInfo);
			AddLineSeparator(brick);
			brick.BorderStyle = BrickBorderStyle.Center;
			PGraphics.DrawBrick(brick, info.Rectangle);
		}
		void AddLineSeparator(IVisualBrick brick) {
			if(!PrintLineSeparator)
				return;
			PrintLineSeparator = false;
			brick.Sides |= BorderSide.Left;
		}
		protected void PrintRowHeaderCore(BaseRowViewInfo rowInfo) {
			BaseRowHeaderInfo rowHeader = rowInfo.HeaderInfo;
			for(int i = 0; i < rowHeader.CaptionsInfo.Count; i++) {
				RowCaptionInfo captionInfo = (RowCaptionInfo)rowHeader.CaptionsInfo[i];
				PrintRowHeaderCell(captionInfo);
				PrintSeparator(rowInfo as MultiEditorRowViewInfo, captionInfo, true);
			}
		}
		protected void PrintRowHeaderCellCore(CustomDrawRowHeaderCellEventArgs e, RowCaptionInfo captionInfo) {
			Rectangle bounds = IncreaseSize(e.Bounds);
			if(Grid.IsCategoryRow(captionInfo.Row)) {
				bounds.Width += (ViewInfo.RC.VertLineWidth == 0 ? 1 : 0) * (ViewInfo.VisibleValuesCount - 1) -
					(Grid.RecordsInterval == 0 ? 0 : Grid.RecordsInterval + ViewInfo.RC.VertLineWidth) * (ViewInfo.VisibleValuesCount - 1);
			}
			PrintCellHelperInfo info = new PrintCellHelperInfo(
				Color.Black,
				PrintingSystem,
				e.Caption,
				e.Appearance,
				e.Caption,
				bounds,
				PGraphics,
				e.Appearance.HAlignment,
				Grid.OptionsView.ShowHorzLines,
				Grid.OptionsView.ShowVertLines,
				string.Empty);
			IImageBrick imageBrick = GetImageBrick(e, info);
			bool imageExists = imageBrick != null;
			IPanelBrick pBrick = CreatePanelBrick(info, !imageExists, CreateBrickStyle(info, string.Empty));
			pBrick.Sides &= GetBorderSideMask(captionInfo);
			AddLineSeparator(pBrick);
			if(imageExists) {
				pBrick.Bricks.Add(imageBrick);
			} else {
				pBrick.Style.ForeColor = e.Appearance.ForeColor;
				pBrick.Style.Font = e.Appearance.Font;
			}
			ITextBrick textBrick = GetTextBrick(e, info);
			pBrick.Bricks.Add(textBrick);
			PGraphics.DrawBrick(pBrick, info.Rectangle);
		}
		ITextBrick GetTextBrick(CustomDrawRowHeaderCellEventArgs e, PrintCellHelperInfo info) {
			ITextBrick textBrick = CreateTextBrick(info);
			SetCommonBrickProperties(textBrick, info);
			textBrick.Sides = DevExpress.XtraPrinting.BorderSide.None;
			textBrick.Padding = new PaddingInfo(2, 1, 0, 0);
			textBrick.VertAlignment = VertAlignment.Center;
			textBrick.Rect = new Rectangle(e.CaptionRect.X - info.Rectangle.X + 1, e.CaptionRect.Y - info.Rectangle.Y + 2, e.CaptionRect.Width, e.CaptionRect.Height - 2);
			return textBrick;
		}
		IImageBrick GetImageBrick(CustomDrawRowHeaderCellEventArgs e, PrintCellHelperInfo info) {
			if(e.ImageIndex < 0 || e.ImageRect.Size == Size.Empty) return null;
			using(BitmapGraphicsHelper gHelper = new BitmapGraphicsHelper(e.ImageRect.Width, e.ImageRect.Height)) {
				IImageBrick imageBrick = CreateImageBrick(info, CreateBrickStyle(info, ""));
				DrawImage(new GraphicsCache(gHelper.Graphics), e.ImageIndex, new Rectangle(0, 0, gHelper.Bitmap.Width, gHelper.Bitmap.Height));
				imageBrick.Image = gHelper.MemSafeBitmap;
				imageBrick.Rect = new Rectangle(e.ImageRect.X - info.Rectangle.X + 1, e.ImageRect.Y - info.Rectangle.Y + 1, e.ImageRect.Width - 1, e.ImageRect.Height - 1);
				imageBrick.Sides = BorderSide.None;
				imageBrick.Padding = PaddingInfo.Empty;
				return imageBrick;
			}
		}
		protected virtual void DrawImage(GraphicsCache cache, int imageIndex, Rectangle imageRect) {
			if(imageIndex != -1) 
				ImageCollection.DrawImageListImage(cache, ViewInfo.Grid.ImageList, imageIndex, imageRect);
		}
		protected void PrintRowHeaderCell(RowCaptionInfo captionInfo) {
			Rectangle captionRectangle = new Rectangle(
				X,
				Y,
				captionInfo.CaptionRect.Width + (Grid.IsCategoryRow(captionInfo.Row) && captionInfo.Row.Level == 0 ? ViewInfo.RC.VertLineWidth : 0),
				captionInfo.CaptionRect.Height);
			X += captionInfo.CaptionRect.Width + 1;
			Point offsetPoint = GetOffset(captionRectangle.Location, captionInfo.CaptionRect.Location);
			CustomDrawRowHeaderCellEventArgs e = new CustomDrawRowHeaderCellEventArgs(null, captionRectangle, captionInfo.Style);
			e.Init(captionInfo);
			e.Offset(offsetPoint);
			PrintRowHeaderCellCore(e, captionInfo);
		}
		Point GetOffset(Point p0, Point p1) {
			return new Point(p0.X - p1.X, p0.Y - p1.Y);
		}
		void PrintSeparatorCore(CustomDrawSeparatorEventArgs e) {
			if(e.SeparatorKind != SeparatorKind.String)
				return;
			Rectangle bounds = new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 1, e.Bounds.Height + 1);
			X += bounds.Width;
			PrintCellHelperInfo info = new PrintCellHelperInfo(
				Color.Black,
				PrintingSystem,
				e.SeparatorString,
				e.Appearance,
				e.SeparatorString,
				bounds,
				PGraphics);
			info.Appearance.TextOptions.WordWrap = WordWrap.NoWrap;
			ITextBrick textBrick = CreateTextBrick(info);
			SetCommonBrickProperties(textBrick, info);
			textBrick.Sides = ViewInfo.Grid.OptionsView.ShowHorzLines ? BorderSide.Top | BorderSide.Bottom : BorderSide.None;
			textBrick.VertAlignment = VertAlignment.Center;
			textBrick.HorzAlignment = HorzAlignment.Center;
			textBrick.Padding = new PaddingInfo(1, 0, 0, 0);
			PGraphics.DrawBrick(textBrick, info.Rectangle);
		}
		RowViewInfoReadOnlyCollection CreateRowViewInfos() {
			RowViewInfoReadOnlyCollection rowViewInfos = new RowViewInfoReadOnlyCollection();
			rowViewInfos.Comparer = new RowViewInfoComparerAdapter((IComparer<BaseRow>)Grid.VisibleRowsComparer);
			foreach(BaseRowViewInfo rowViewInfo in ViewInfo.RowsViewInfo){
				rowViewInfos.Add(rowViewInfo);
			}
			rowViewInfos.Sort();
			return rowViewInfos;
		}
	}
	public class SingleRecordPrintInfo : BasePrintInfo {
		public SingleRecordPrintInfo(BaseViewInfo viewInfo) : base(viewInfo) { }
	}
	public class MultiRecordPrintInfo : BasePrintInfo {
		public MultiRecordPrintInfo(BaseViewInfo viewInfo) : base(viewInfo) { }
	}
	public class BandsPrintInfo : BasePrintInfo {
		public BandsPrintInfo(BaseViewInfo viewInfo) : base(viewInfo) { }
	}
	public class VGridPrinterBase {
		VGridControlBase ownerCore;
		BasePrintInfo printInfoCore;
		public VGridPrinterBase(VGridControlBase owner) {
			ownerCore = owner;
		}
		protected virtual void CreatePrintInfo(IPrintingSystem ps, IBrickGraphics graph) {
			BaseRow row = ownerCore.FocusedRow;
			ownerCore.FocusedRow = null;
			printInfoCore = ownerCore.CreatePrintViewInfo(graph).CreatePrintInfo();
			ownerCore.FocusedRow = row;
		}
		protected BasePrintInfo PrintInfo { get { return printInfoCore; } }
		protected internal virtual void CreatePrintArea(IPrintingSystem ps, string areaName, IBrickGraphics graph) {
			if(areaName == "DetailHeader") {
				CreatePrintInfo(ps, graph);
			}
			if(PrintInfo == null) return;
			switch(areaName) {
				case "DetailHeader": PrintInfo.PrintHeader(graph);
					break;
				case "Detail": PrintInfo.PrintRows();
					break;
				case "DetailFooter": PrintInfo.PrintFooter(graph);
					break;
			}
		}
	}
}
