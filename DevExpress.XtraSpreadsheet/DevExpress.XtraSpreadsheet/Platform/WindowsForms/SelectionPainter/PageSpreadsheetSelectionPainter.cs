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
using System.IO;
using DevExpress.Skins;
using DevExpress.LookAndFeel;
using DevExpress.Office.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Office.Layout;
using DevExpress.Office.Drawing;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.Utils;
using System.Reflection;
using System.Drawing.Drawing2D;
namespace DevExpress.XtraSpreadsheet.Drawing {
	public abstract class PageSpreadsheetSelectionPainter : SpreadsheetSelectionPainter {
		#region Fields
		const int smallBorderWidthInPixels = 1;
		const int largeBorderWidthInPixels = 3;
		const int borderOffsetInPixels = -2;
		int onePixel;
		int smallBorderWidth;
		int largeBorderWidth;
		int borderOffset;
		int twoPixelsBorderWidth;
		float[] dashPattern = new float[] { 4f, 1f };
		#endregion
		protected PageSpreadsheetSelectionPainter(GraphicsCache cache)
			: base(cache) {
		}
		public int LargeBorderWidth { get { return largeBorderWidth; } }
		public int OnePixel { get { return onePixel; } }
		protected internal void Initialize(DocumentLayoutUnitConverter layoutUnitConverter) {
			smallBorderWidth = layoutUnitConverter.PixelsToLayoutUnits(smallBorderWidthInPixels, DocumentModel.Dpi);
			largeBorderWidth = layoutUnitConverter.PixelsToLayoutUnits(GetSelectionBorderWidth(), DocumentModel.Dpi);
			borderOffset = layoutUnitConverter.PixelsToLayoutUnits(borderOffsetInPixels, DocumentModel.Dpi);
			onePixel = layoutUnitConverter.PixelsToLayoutUnits(1, DocumentModel.Dpi);
			twoPixelsBorderWidth = layoutUnitConverter.PixelsToLayoutUnits(2, DocumentModel.Dpi);
		}
		protected internal virtual int GetSelectionBorderWidth() {
			return largeBorderWidthInPixels;
		}
		protected override void FillRectangle(Rectangle bounds) {
			Cache.FillRectangle(GetSelectionColor(), bounds);
		}
		protected internal virtual Color GetSelectionColor() {
			return selectionColor;
		}
		protected override void DrawRectangle(Rectangle bounds) {
			if (IsMultiSelection) {
				bounds.Inflate(borderOffset, borderOffset);
				DrawSelectionBorder(bounds, smallBorderWidth);
			}
			else
				DrawSelectionBorder(GetActualBounds(bounds), largeBorderWidth);
		}
		protected internal void DrawSelectionBorder(Rectangle bounds, int widthInPixels) {
			Cache.DrawRectangle(Cache.GetPen(GetSelectionBorderColor(), widthInPixels), bounds);
		}
		protected Rectangle GetActualBounds(Rectangle bounds) {
			if (largeBorderWidth % 2 == 0) {
				bounds.Width += onePixel;
				bounds.Height += onePixel;
			}
			return bounds;
		}
		protected override void DrawLine(Point from, Point to) {
			Exceptions.ThrowInternalException(); 
		}
		protected override void DrawDashRectangle(Rectangle bounds) {
			using (Pen pen = new Pen(GetSelectionBorderColor(), LargeBorderWidth)) {
				pen.DashPattern = dashPattern;
				Cache.DrawRectangle(pen, GetActualBounds(bounds));
			}
		}
		protected override void DrawPrintRangeRectangle(Rectangle bounds) {
			using (Pen pen = new Pen(GetPrintRangeBorderColor(), smallBorderWidth)) {
				pen.DashPattern = dashPattern;
				Cache.DrawRectangle(pen, GetActualBounds(bounds));
			}
		}
		protected override void DrawRangeResizeHotZone(Rectangle bounds) {
			bounds.Offset(onePixel, onePixel);
			Cache.FillRectangle(GetSelectionBorderColor(), bounds);
		}
		protected override int GetConvertedMailMergeRangeWidth() {
			return twoPixelsBorderWidth;
		}
		protected override void DrawColumnAutoFilterBackground(Rectangle bounds) {
			Cache.FillRectangle(Brushes.Red, bounds);
		}
		protected override void DrawPivotTableExpandCollapseHotZone(Rectangle bounds, bool isCollapsed) {
			Cache.FillRectangle(Brushes.Red, bounds);
		}
	}
}
