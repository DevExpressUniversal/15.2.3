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
using DevExpress.XtraPrinting.Export;
using DevExpress.XtraPrinting.Export.Pdf;
using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraPrinting.BrickExporters {
	public class BrickExporter : BrickBaseExporter {
		protected ITableCell TableCell { get { return Brick as ITableCell; } }
		protected string Url { get { return TableCell != null ? TableCell.Url : string.Empty; } }
		public override void Draw(IGraphics gr, RectangleF rect, RectangleF parentRect) {
			if(gr is GdiGraphics)
				Draw(((GdiGraphics)gr).Graphics, rect);
			else if(gr is PdfGraphics)
				DrawPdf((PdfGraphics)gr, rect);
		}
		protected internal override void DrawWarningRect(IGraphics gr, RectangleF r, string message) {
			base.DrawWarningRect(gr, r, message);
			((Brick)Brick).Hint = String.Format("{0} {1}", PreviewLocalizer.GetString(PreviewStringId.Msg_CustomDrawWarning), message);
		}
		public virtual void Draw(Graphics gr, RectangleF rect) {
		}
		internal protected virtual void DrawPdf(PdfGraphics gr, RectangleF rect) {
		}
		public void FillRtfTableCell(IRtfExportProvider exportProvider) {
			FillRtfTableCellInternal(exportProvider);
		}
		public void FillXlsTableCell(IXlsExportProvider exportProvider) {
			FillXlsTableCellInternal(exportProvider);
		}
		public void FillHtmlTableCell(IHtmlExportProvider exportProvider) {
			FillHtmlTableCellInternal(exportProvider);
		}
		public void FillTextTableCell(ITableExportProvider exportProvider, bool shouldSplitText) {
			FillTextTableCellInternal(exportProvider, shouldSplitText);
		}
		protected internal virtual void FillHtmlTableCellInternal(IHtmlExportProvider exportProvider) {
		}
		protected internal virtual void FillRtfTableCellInternal(IRtfExportProvider exportProvider) {
		}
		protected internal virtual void FillXlsTableCellInternal(IXlsExportProvider exportProvider) {
		}
		protected internal virtual void FillTextTableCellInternal(ITableExportProvider exportProvider, bool shouldSplitText) {
			exportProvider.SetCellText(string.Empty, null);
		}
		protected internal virtual BrickViewData[] GetHtmlData(ExportContext htmlExportContext, RectangleF bounds, RectangleF clipBounds) {
			return GetExportData(htmlExportContext, bounds, clipBounds);
		}
		protected internal virtual BrickViewData[] GetXlsData(ExportContext xlsExportContext, RectangleF rect) {
			return GetExportData(xlsExportContext, rect, rect);
		}
		protected internal virtual BrickViewData[] GetRtfData(ExportContext rtfExportContext, RectangleF rect, RectangleF clipRect) {
			return GetExportData(rtfExportContext, rect, clipRect);
		}
		protected internal virtual BrickViewData[] GetTextData(ExportContext exportContext, RectangleF rect) {
			return GetExportData(exportContext, rect, rect);
		}
		protected internal virtual BrickViewData[] GetExportData(ExportContext exportContext, RectangleF rect, RectangleF clipRect) {
			return exportContext.CreateBrickViewDataArray(null, rect, TableCell);
		}
		internal override void ProcessLayout(PageLayoutBuilder layoutBuilder, PointF pos, RectangleF clipRect) {
			RectangleF brickBounds = ((Brick)Brick).GetViewRectangle();
			brickBounds.Offset(pos);
			ProcessLayoutCore(layoutBuilder, brickBounds, RectangleF.Intersect(clipRect, brickBounds));
		}
		protected void ProcessLayoutCore(PageLayoutBuilder layoutBuilder, RectangleF rect, RectangleF clipRect) {
			var realBrick = ((Brick)Brick).GetRealBrick();
			RectangleF realBrickBounds = GraphicsUnitConverter.Convert(rect, GraphicsDpi.Document, GraphicsDpi.DeviceIndependentPixel);
			RectangleF realBrickClipBounds = layoutBuilder.GetCorrectClipRect(GraphicsUnitConverter.Convert(clipRect, GraphicsDpi.Document, GraphicsDpi.DeviceIndependentPixel));
			var brickViewDataCollection = layoutBuilder.GetData(realBrick, realBrickBounds, realBrickClipBounds);
			foreach(var brickViewData in brickViewDataCollection) {
				System.Diagnostics.Debug.Assert(brickViewData is PageBrickViewData);
				brickViewData.ApplyClipping(GraphicsUnitConverter.Round(realBrickClipBounds), !(layoutBuilder.ExportContext is RtfExportContext || layoutBuilder.ExportContext is HtmlExportContext));
			}
			layoutBuilder.AddData(brickViewDataCollection);
		}
	}
}
