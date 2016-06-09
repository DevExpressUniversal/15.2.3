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
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.PivotGrid.Printing {
	public class PrintCellSizeProvider : CellSizeProvider {
		PivotGridPrinterBase printer;
		public PrintCellSizeProvider(PivotGridData data, PivotVisualItemsBase visualItems, PivotGridPrinterBase printer)
			: base(data, visualItems) {
			this.printer = printer;
		}
		protected override int GetFieldValueHeight(int lineCount, PivotGridValueType valueType, PivotFieldItemBase field) {
			return MeasureAppearance(printer.GetValueAppearance(valueType, field), GetMeasureCaption(field != null ? (field.IsColumnOrRow ? (field.IsColumn ? field.ColumnValueLineCount : field.RowValueLineCount) : lineCount) : 1), printer).Height + 6;
		}
		protected internal override int CalculateHeaderHeight(PivotFieldItemBase field) {
			return MeasureAppearance(printer.GetFieldAppearance(field), field == null ? GetMeasureCaption(1) : field.HeaderDisplayText, printer).Height + 6;
		}
		protected internal override int CalculateHeaderWidth(PivotFieldItemBase field) {
			return MeasureAppearance(printer.GetFieldAppearance(field), field.HeaderDisplayText, printer).Width + 11; 
		}
		protected internal override int CalculateHeaderWidthOffset(PivotFieldItemBase field) {
			return 0; 
		}
		static public Size MeasureAppearance(IPivotPrintAppearance appearance, string text, PivotGridPrinterBase printer) {
			SizeF sizeF = MeasureString(printer, text, appearance.Font, Int32.MaxValue, appearance.StringFormat, printer.GetPageUnit());
			RectangleF rect = new RectangleF(PointF.Empty, sizeF);
			Size size = Size.Truncate(rect.Size);
			size.Height++;
			size.Width++;
			return size;
		}
		static SizeF MeasureString(PivotGridPrinterBase printer, string text, Font font, int width, StringFormat stringFormat, GraphicsUnit pageUint) {
			if(printer.Graph == null)
				return DevExpress.XtraPrinting.Native.Measurement.MeasureString(text, font, width, stringFormat, pageUint);
			return Measure(printer, pageUint, 
				brickGraphics => brickGraphics.MeasureString(text, font, width, stringFormat)
			);
		}
		static public SizeF MeasureString(PivotGridPrinterBase printer, string text, Font font, GraphicsUnit pageUint) {
			if(printer.Graph == null)
				return DevExpress.XtraPrinting.Native.Measurement.MeasureString(text, font, pageUint);
			return Measure(printer, pageUint,
				brickGraphics => brickGraphics.MeasureString(text, font)
			);
		}
		static SizeF Measure(PivotGridPrinterBase printer, GraphicsUnit graphicsUnit, Func<DevExpress.XtraPrinting.BrickGraphics,SizeF> measure) {
			DevExpress.XtraPrinting.BrickGraphics brickGraphics = (DevExpress.XtraPrinting.BrickGraphics)printer.Graph;
			GraphicsUnit originalUnit = brickGraphics.PageUnit;
			brickGraphics.PageUnit = graphicsUnit;
			try {
				return measure(brickGraphics);
			} finally {
				brickGraphics.PageUnit = originalUnit;
			}
		}
	}
}
