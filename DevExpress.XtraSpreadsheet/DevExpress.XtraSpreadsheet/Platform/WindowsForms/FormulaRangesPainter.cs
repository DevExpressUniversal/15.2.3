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
using DevExpress.Utils;
using System.Drawing.Drawing2D;
using DevExpress.Utils.Drawing;
using DevExpress.Office.Layout;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.XtraSpreadsheet.Internal;
namespace DevExpress.XtraSpreadsheet.Drawing {
	#region FormulaRangesPainter
	public class FormulaRangesPainter {
		#region Fields
		readonly DocumentModel documentModel;
		const int cornerBorderWidthInPixels = 1;
		const int cornerWidthInPixels = InnerCellInplaceEditor.CornerFormulaRangeResizWidthInPixels + cornerBorderWidthInPixels * 2;
		GraphicsCache cache;
		#endregion
		public FormulaRangesPainter(DocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.documentModel = documentModel;
		}
		#region Properties
		public DocumentLayoutUnitConverter UnitConverter { get { return documentModel.LayoutUnitConverter; } }
		#endregion
		public void Draw(GraphicsCache cache, Page page, FormulaReferencedRanges referencedRanges) {
			this.cache = cache;
			int count = referencedRanges.Count;
			for (int i = 0; i < count; i++)
				DrawFormulaRange(page, referencedRanges[i]);
		}
		void DrawFormulaRange(Page page, FormulaReferencedRange referencedRange) {
			DrawFormulaRange(page, referencedRange.CellRange, referencedRange.Color, referencedRange.IsReadOnly);
		}
		void DrawFormulaRange(Page page, CellRangeBase range, Color color, bool isReadOnly) {
			CellUnion unionRange = range as CellUnion;
			if (unionRange != null) {
				foreach (CellRangeBase subRange in unionRange.InnerCellRanges)
					DrawFormulaRange(page, subRange, color, isReadOnly);
				return;
			}
			CellRange simpleRange = range as CellRange;
			if (simpleRange != null)
				DrawFormulaRangeCore(page, simpleRange, color, isReadOnly);
		}
		void DrawFormulaRangeCore(Page page, CellRange range, Color color, bool isReadOnly) {
			if (range.Worksheet != page.Sheet)
				return;
			Rectangle bounds = page.CalculateRangeBounds(range);
			if (bounds == Rectangle.Empty)
				return;
			Color fillColor = DXColor.FromArgb(16, color.R, color.G, color.B);
			cache.FillRectangle(fillColor, bounds);
			float twoPixels = UnitConverter.PixelsToLayoutUnitsF(2, DocumentModel.Dpi);
			using (Pen whitePen = new Pen(DXColor.White, twoPixels)) {
				whitePen.Alignment = PenAlignment.Inset;
				cache.DrawRectangle(whitePen, bounds);
			}
			using (Pen colorPen = new Pen(color, twoPixels)) {
				cache.DrawRectangle(colorPen, bounds);
			}
			if (isReadOnly)
				return;
			DrawFormulaRangeCorner(CalculateTopLeftCorner(bounds), color);
			DrawFormulaRangeCorner(CalculateTopRightCorner(bounds), color);
			DrawFormulaRangeCorner(CalculateBottomRightCorner(bounds), color);
			DrawFormulaRangeCorner(CalculateBottomLeftCorner(bounds), color);
		}
		void DrawFormulaRangeCorner(Rectangle bounds, Color color) {
			cache.FillRectangle(DXColor.White, bounds);
			int borderWidth = (int)Math.Round(UnitConverter.PixelsToLayoutUnitsF(cornerBorderWidthInPixels, DocumentModel.Dpi));
			bounds.Inflate(-borderWidth, -borderWidth);
			cache.FillRectangle(color, bounds);
		}
		Rectangle CalculateTopLeftCorner(Rectangle bounds) {
			return CalculateCorner(new Point(bounds.Left, bounds.Top), -4, -4);
		}
		Rectangle CalculateTopRightCorner(Rectangle bounds) {
			return CalculateCorner(new Point(bounds.Right, bounds.Top), -5, -4);
		}
		Rectangle CalculateBottomRightCorner(Rectangle bounds) {
			return CalculateCorner(new Point(bounds.Right, bounds.Bottom), -5, -5);
		}
		Rectangle CalculateBottomLeftCorner(Rectangle bounds) {
			return CalculateCorner(new Point(bounds.Left, bounds.Bottom), -4, -5);
		}
		Rectangle CalculateCorner(Point point, int horizontalOffsetInPixels, int verticalOffsetInPixels) {
			int totalAreaPixelWidth = (int)Math.Round(UnitConverter.PixelsToLayoutUnitsF(cornerWidthInPixels, DocumentModel.Dpi));
			int horizontalOffset = (int)Math.Round(UnitConverter.PixelsToLayoutUnitsF(horizontalOffsetInPixels, DocumentModel.Dpi));
			int verticalOffset = (int)Math.Round(UnitConverter.PixelsToLayoutUnitsF(verticalOffsetInPixels, DocumentModel.Dpi));
			return new Rectangle(point.X + horizontalOffset, point.Y + verticalOffset, totalAreaPixelWidth, totalAreaPixelWidth);
		}
	}
	#endregion
}
