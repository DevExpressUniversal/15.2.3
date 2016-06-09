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
using System.Windows;
using System.Windows.Controls;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Xpf.Spreadsheet.Extensions.Internal;
using System.Windows.Media;
using DevExpress.Office.Layout;
using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.Xpf.Spreadsheet.Internal {
	public class FormulaRangesControl : WorksheetPaintControl {
		const int cornerWidthInPixels = 8;
		protected override void OnRender(System.Windows.Media.DrawingContext dc) {
			base.OnRender(dc);
			if (Spreadsheet == null || Spreadsheet.InnerControl.InplaceEditor == null) return;
			if (!Spreadsheet.InnerControl.IsInplaceEditorActive) return;
			InnerCellInplaceEditor editor = Spreadsheet.InnerControl.InplaceEditor;
			FormulaReferencedRanges referencedRanges = editor.ReferencedRanges;
			if (referencedRanges.Count <= 0) return;
			var pages = LayoutInfo.Pages;
			foreach (DevExpress.XtraSpreadsheet.Layout.Page page in pages) {
				DrawFormulaRanges(dc, page, referencedRanges);
			}
		}
		private void DrawFormulaRanges(System.Windows.Media.DrawingContext dc, XtraSpreadsheet.Layout.Page page, FormulaReferencedRanges referencedRanges) {
			int count = referencedRanges.Count;
			for (int i = 0; i < count; i++)
				DrawFormulaRange(dc, page, referencedRanges[i]);
		}
		private void DrawFormulaRange(System.Windows.Media.DrawingContext dc, XtraSpreadsheet.Layout.Page page, FormulaReferencedRange referencedRange) {
			DrawFormulaRange(dc, page, referencedRange.CellRange, referencedRange.Color, referencedRange.IsReadOnly);
		}
		private void DrawFormulaRange(System.Windows.Media.DrawingContext dc, XtraSpreadsheet.Layout.Page page, CellRangeBase range, System.Drawing.Color color, bool isReadOnly) {
			CellUnion unionRange = range as CellUnion;
			if (unionRange != null) {
				foreach (CellRangeBase subRange in unionRange.InnerCellRanges)
					DrawFormulaRange(dc, page, subRange, color, isReadOnly);
				return;
			}
			CellRange simpleRange = range as CellRange;
			if (simpleRange != null)
				DrawFormulaRangeCore(dc, page, simpleRange, color.ToWpfColor(), isReadOnly);
		}
		DocumentLayoutUnitConverter UnitConverter { get { return LayoutInfo.DocumentModel.LayoutUnitConverter; } }
		private void DrawFormulaRangeCore(System.Windows.Media.DrawingContext dc, XtraSpreadsheet.Layout.Page page, CellRange range, Color color, bool isReadOnly) {
			if (range.Worksheet != page.Sheet)
				return;
			Rect bounds = page.CalculateRangeBounds(range).ToRect();
			if (bounds.IsEmpty) return;
			Color fillColor = Color.FromArgb(16, color.R, color.G, color.B);
			float twoPixels = UnitConverter.PixelsToLayoutUnitsF(2, DocumentModel.Dpi);
			Pen whitePen = new Pen(Brushes.White, twoPixels);
			Brush brush = new SolidColorBrush(fillColor);
			dc.DrawRectangle(brush, whitePen, bounds);
			Pen colorPen = new Pen(new SolidColorBrush(color), twoPixels);
			dc.DrawRectangle(null, colorPen, bounds);
			if (isReadOnly) return;
			DrawFormulaRangeCorner(dc, CalculateTopLeftCorner(bounds), color);
			DrawFormulaRangeCorner(dc, CalculateTopRightCorner(bounds), color);
			DrawFormulaRangeCorner(dc, CalculateBottomRightCorner(bounds), color);
			DrawFormulaRangeCorner(dc, CalculateBottomLeftCorner(bounds), color);
		}
		void DrawFormulaRangeCorner(System.Windows.Media.DrawingContext dc, Rect bounds, Color color) {
			int borderWidth = (int)Math.Round(UnitConverter.PixelsToLayoutUnitsF(1, DocumentModel.Dpi));
			bounds.Inflate(-borderWidth, -borderWidth);
			dc.DrawRectangle(new SolidColorBrush(color), null, bounds);
		}
		Rect CalculateTopLeftCorner(Rect bounds) {
			return CalculateCorner(new Point(bounds.Left, bounds.Top), -4, -4);
		}
		Rect CalculateTopRightCorner(Rect bounds) {
			return CalculateCorner(new Point(bounds.Right, bounds.Top), -5, -4);
		}
		Rect CalculateBottomRightCorner(Rect bounds) {
			return CalculateCorner(new Point(bounds.Right, bounds.Bottom), -5, -5);
		}
		Rect CalculateBottomLeftCorner(Rect bounds) {
			return CalculateCorner(new Point(bounds.Left, bounds.Bottom), -4, -5);
		}
		Rect CalculateCorner(Point point, int horizontalOffsetInPixels, int verticalOffsetInPixels) {
			int totalAreaPixelWidth = (int)Math.Round(UnitConverter.PixelsToLayoutUnitsF(cornerWidthInPixels, DocumentModel.Dpi));
			int horizontalOffset = (int)Math.Round(UnitConverter.PixelsToLayoutUnitsF(horizontalOffsetInPixels, DocumentModel.Dpi));
			int verticalOffset = (int)Math.Round(UnitConverter.PixelsToLayoutUnitsF(verticalOffsetInPixels, DocumentModel.Dpi));
			return new Rect(point.X + horizontalOffset, point.Y + verticalOffset, totalAreaPixelWidth, totalAreaPixelWidth);
		}
	}
}
