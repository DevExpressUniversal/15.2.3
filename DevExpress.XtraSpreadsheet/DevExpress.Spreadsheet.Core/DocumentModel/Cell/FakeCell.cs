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
using DevExpress.Utils;
using System.Drawing;
using DevExpress.Office.Drawing;
using System.Collections.Generic;
using DevExpress.Export.Xl;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model {
	#region FakeCell
	public class FakeCell : Cell {
		public static NumberFormatResult FormatResult = CreateNumberFormatResult();
		static NumberFormatResult CreateNumberFormatResult() {
			NumberFormatResult result = new NumberFormatResult();
			result.Text = String.Empty;
			return result;
		}
		public FakeCell(CellPosition position, Worksheet sheet)
			: base(position, sheet) {
		}
		public override bool ShouldUseInLayout { get { return true; } }
		#region GetActualFormatStringValue
		protected override string GetActualFormatStringValue() {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyNumberFormat)
				return row.ActualFormatString;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyNumberFormat)
				return column.ActualFormatString;
			return base.GetActualFormatStringValue();
		}
		#endregion
		#region GetActualFormatIndexValue
		protected override int GetActualFormatIndexValue() {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyNumberFormat)
				return row.ActualFormatIndex;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyNumberFormat)
				return column.ActualFormatIndex;
			return base.GetActualFormatIndexValue();
		}
		#endregion
		#region GetActuaAlignmentValue
		protected override XlHorizontalAlignment GetActualAlignmentHorizontal() {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyAlignment)
				return row.ActualAlignment.Horizontal;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyAlignment)
				return column.ActualAlignment.Horizontal;
			return base.GetActualAlignmentHorizontal();
		}
		protected override XlVerticalAlignment GetActualAlignmentVertical() {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyAlignment)
				return row.ActualAlignment.Vertical;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyAlignment)
				return column.ActualAlignment.Vertical;
			return base.GetActualAlignmentVertical();
		}
		protected override XlReadingOrder GetActualAlignmentReadingOrder() {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyAlignment)
				return row.ActualAlignment.ReadingOrder;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyAlignment)
				return column.ActualAlignment.ReadingOrder;
			return base.GetActualAlignmentReadingOrder();
		}
		protected override byte GetActualAlignmentIndent() {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyAlignment)
				return row.ActualAlignment.Indent;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyAlignment)
				return column.ActualAlignment.Indent;
			return base.GetActualAlignmentIndent();
		}
		protected override int GetActualAlignmentRelativeIndent() {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyAlignment)
				return row.ActualAlignment.RelativeIndent;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyAlignment)
				return column.ActualAlignment.RelativeIndent;
			return base.GetActualAlignmentRelativeIndent();
		}
		protected override bool GetActualAlignmentJustifyLastLine() {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyAlignment)
				return row.ActualAlignment.JustifyLastLine;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyAlignment)
				return column.ActualAlignment.JustifyLastLine;
			return base.GetActualAlignmentJustifyLastLine();
		}
		protected override bool GetActualAlignmentShrinkToFit() {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyAlignment)
				return row.ActualAlignment.ShrinkToFit;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyAlignment)
				return column.ActualAlignment.ShrinkToFit;
			return base.GetActualAlignmentShrinkToFit();
		}
		protected override int GetActualAlignmentTextRotation() {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyAlignment)
				return row.ActualAlignment.TextRotation;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyAlignment)
				return column.ActualAlignment.TextRotation;
			return base.GetActualAlignmentTextRotation();
		}
		protected override bool GetActualAlignmentWrapText() {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyAlignment)
				return row.ActualAlignment.WrapText;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyAlignment)
				return column.ActualAlignment.WrapText;
			return base.GetActualAlignmentWrapText();
		}
		#endregion
		#region GetActuaFontValue
		protected override string GetActualFontName(bool actualApplyFont) {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyFont)
				return row.ActualFont.Name;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyFont)
				return column.ActualFont.Name;
			return base.GetActualFontName(actualApplyFont);
		}
		protected override bool GetActualFontBold(bool actualApplyFont) {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyFont)
				return row.ActualFont.Bold;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyFont)
				return column.ActualFont.Bold;
			return base.GetActualFontBold(actualApplyFont);
		}
		protected override bool GetActualFontExtend(bool actualApplyFont) {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyFont)
				return row.ActualFont.Extend;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyFont)
				return column.ActualFont.Extend;
			return base.GetActualFontExtend(actualApplyFont);
		}
		protected override bool GetActualFontItalic(bool actualApplyFont) {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyFont)
				return row.ActualFont.Italic;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyFont)
				return column.ActualFont.Italic;
			return base.GetActualFontItalic(actualApplyFont);
		}
		protected override bool GetActualFontShadow(bool actualApplyFont) {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyFont)
				return row.ActualFont.Shadow;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyFont)
				return column.ActualFont.Shadow;
			return base.GetActualFontShadow(actualApplyFont);
		}
		protected override bool GetActualFontOutline(bool actualApplyFont) {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyFont)
				return row.ActualFont.Outline;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyFont)
				return column.ActualFont.Outline;
			return base.GetActualFontOutline(actualApplyFont);
		}
		protected override bool GetActualFontStrikeThrough(bool actualApplyFont) {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyFont)
				return row.ActualFont.StrikeThrough;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyFont)
				return column.ActualFont.StrikeThrough;
			return base.GetActualFontStrikeThrough(actualApplyFont);
		}
		protected override bool GetActualFontCondense(bool actualApplyFont) {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyFont)
				return row.ActualFont.Condense;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyFont)
				return column.ActualFont.Condense;
			return base.GetActualFontCondense(actualApplyFont);
		}
		protected override int GetActualFontColorIndex(bool actualApplyFont) {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyFont)
				return row.ActualFont.ColorIndex;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyFont)
				return column.ActualFont.ColorIndex;
			return base.GetActualFontColorIndex(actualApplyFont);
		}
		protected override int GetActualFontCharset(bool actualApplyFont) {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyFont)
				return row.ActualFont.Charset;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyFont)
				return column.ActualFont.Charset;
			return base.GetActualFontCharset(actualApplyFont);
		}
		protected override int GetActualFontFamily(bool actualApplyFont) {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyFont)
				return row.ActualFont.FontFamily;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyFont)
				return column.ActualFont.FontFamily;
			return base.GetActualFontFamily(actualApplyFont);
		}
		protected override double GetActualFontSize(bool actualApplyFont) {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyFont)
				return row.ActualFont.Size;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyFont)
				return column.ActualFont.Size;
			return base.GetActualFontSize(actualApplyFont);
		}
		protected override XlScriptType GetActualFontScript(bool actualApplyFont) {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyFont)
				return row.ActualFont.Script;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyFont)
				return column.ActualFont.Script;
			return base.GetActualFontScript(actualApplyFont);
		}
		protected override XlUnderlineType GetActualFontUnderline(bool actualApplyFont) {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyFont)
				return row.ActualFont.Underline;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyFont)
				return column.ActualFont.Underline;
			return base.GetActualFontUnderline(actualApplyFont);
		}
		protected override XlFontSchemeStyles GetActualFontSchemeStyle(bool actualApplyFont) {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyFont)
				return row.ActualFont.SchemeStyle;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyFont)
				return column.ActualFont.SchemeStyle;
			return base.GetActualFontSchemeStyle(actualApplyFont);
		}
		protected override FontInfo GetActualFontInfo() {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyFont)
				return row.ActualFont.GetFontInfo();
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyFont)
				return column.ActualFont.GetFontInfo();
			return base.GetActualFontInfo();
		}
		#endregion
		#region GetActualFillValue
		protected override XlPatternType GetActualFillPatternType() {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyFill)
				return row.ActualFill.PatternType;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyFill)
				return column.ActualFill.PatternType;
			return base.GetActualFillPatternType();
		}
		protected override int GetActualFillForeColorIndex() {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyFill)
				return row.ActualFill.ForeColorIndex;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyFill)
				return column.ActualFill.ForeColorIndex;
			return base.GetActualFillForeColorIndex();
		}
		protected override int GetActualFillBackColorIndex() {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyFill)
				return row.ActualFill.BackColorIndex;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyFill)
				return column.ActualFill.BackColorIndex;
			return base.GetActualFillBackColorIndex();
		}
		protected override bool GetActualFillApplyPatternType() {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyFill)
				return true;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyFill)
				return true;
			return base.GetActualFillApplyPatternType();
		}
		protected override bool GetActualFillApplyForeColor() {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyFill)
				return row.ActualFill.ApplyForeColor;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyFill)
				return column.ActualFill.ApplyForeColor;
			return base.GetActualFillApplyForeColor();
		}
		protected override bool GetActualFillApplyBackColor() {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyFill)
				return row.ActualFill.ApplyBackColor;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyFill)
				return column.ActualFill.ApplyBackColor;
			return base.GetActualFillApplyBackColor();
		}
		protected override bool IsDifferential() {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyFill)
				return row.ActualFill.IsDifferential;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyFill)
				return column.ActualFill.IsDifferential;
			return base.IsDifferential();
		}
		protected override ModelFillType GetActualFillType() {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyFill)
				return row.ActualFill.FillType;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyFill)
				return column.ActualFill.FillType;
			return base.GetActualFillType();
		}
		protected override ModelGradientFillType GetActualFillGradientFillType() {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyFill)
				return row.ActualFill.GradientFill.Type;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyFill)
				return column.ActualFill.GradientFill.Type;
			return base.GetActualFillGradientFillType();
		}
		protected override double GetActualFillGradientFillDegree() {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyFill)
				return row.ActualFill.GradientFill.Degree;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyFill)
				return column.ActualFill.GradientFill.Degree;
			return base.GetActualFillGradientFillDegree();
		}
		protected override float GetActualFillGradientFillConvergenceLeft() {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyFill)
				return row.ActualFill.GradientFill.Convergence.Left;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyFill)
				return column.ActualFill.GradientFill.Convergence.Left;
			return base.GetActualFillGradientFillConvergenceLeft();
		}
		protected override float GetActualFillGradientFillConvergenceRight() {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyFill)
				return row.ActualFill.GradientFill.Convergence.Right;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyFill)
				return column.ActualFill.GradientFill.Convergence.Right;
			return base.GetActualFillGradientFillConvergenceRight();
		}
		protected override float GetActualFillGradientFillConvergenceTop() {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyFill)
				return row.ActualFill.GradientFill.Convergence.Top;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyFill)
				return column.ActualFill.GradientFill.Convergence.Top;
			return base.GetActualFillGradientFillConvergenceTop();
		}
		protected override float GetActualFillGradientFillConvergenceBottom() {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyFill)
				return row.ActualFill.GradientFill.Convergence.Bottom;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyFill)
				return column.ActualFill.GradientFill.Convergence.Bottom;
			return base.GetActualFillGradientFillConvergenceBottom();
		}
		protected override IActualGradientStopCollection GetActualFillGradientFillGradientStops() {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyFill)
				return row.ActualFill.GradientFill.GradientStops;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyFill)
				return column.ActualFill.GradientFill.GradientStops;
			return base.GetActualFillGradientFillGradientStops();
		}
		#endregion
		#region GetActualBorderValue
		protected override XlBorderLineStyle GetActualBorderLeftLineStyle() {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyBorder)
				return row.ActualBorder.LeftLineStyle;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyBorder)
				return column.ActualBorder.LeftLineStyle;
			return base.GetActualBorderLeftLineStyle();
		}
		protected override int GetActualBorderLeftColorIndex() {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyBorder)
				return row.ActualBorder.LeftColorIndex;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyBorder)
				return column.ActualBorder.LeftColorIndex;
			return base.GetActualBorderLeftColorIndex();
		}
		protected override XlBorderLineStyle GetActualBorderRightLineStyle() {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyBorder)
				return row.ActualBorder.RightLineStyle;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyBorder)
				return column.ActualBorder.RightLineStyle;
			return base.GetActualBorderRightLineStyle();
		}
		protected override int GetActualBorderRightColorIndex() {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyBorder)
				return row.ActualBorder.RightColorIndex;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyBorder)
				return column.ActualBorder.RightColorIndex;
			return base.GetActualBorderRightColorIndex();
		}
		protected override XlBorderLineStyle GetActualBorderTopLineStyle() {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyBorder)
				return row.ActualBorder.TopLineStyle;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyBorder)
				return column.ActualBorder.TopLineStyle;
			return base.GetActualBorderTopLineStyle();
		}
		protected override int GetActualBorderTopColorIndex() {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyBorder)
				return row.ActualBorder.TopColorIndex;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyBorder)
				return column.ActualBorder.TopColorIndex;
			return base.GetActualBorderTopColorIndex();
		}
		protected override XlBorderLineStyle GetActualBorderBottomLineStyle() {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyBorder)
				return row.ActualBorder.BottomLineStyle;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyBorder)
				return column.ActualBorder.BottomLineStyle;
			return base.GetActualBorderBottomLineStyle();
		}
		protected override int GetActualBorderBottomColorIndex() {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyBorder)
				return row.ActualBorder.BottomColorIndex;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyBorder)
				return column.ActualBorder.BottomColorIndex;
			return base.GetActualBorderBottomColorIndex();
		}
		protected override int GetActualBorderDiagonalColorIndex() {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyBorder)
				return row.ActualBorder.DiagonalColorIndex;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyBorder)
				return column.ActualBorder.DiagonalColorIndex;
			return base.GetActualBorderDiagonalColorIndex();
		}
		protected override XlBorderLineStyle GetActualBorderDiagonalUpLineStyle() {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyBorder)
				return row.ActualBorder.DiagonalUpLineStyle;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyBorder)
				return column.ActualBorder.DiagonalUpLineStyle;
			return base.GetActualBorderDiagonalUpLineStyle();
		}
		protected override XlBorderLineStyle GetActualBorderDiagonalDownLineStyle() {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyBorder)
				return row.ActualBorder.DiagonalDownLineStyle;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyBorder)
				return column.ActualBorder.DiagonalDownLineStyle;
			return base.GetActualBorderDiagonalDownLineStyle();
		}
		protected override XlBorderLineStyle GetActualBorderHorizontalLineStyle() {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyBorder)
				return row.ActualBorder.HorizontalLineStyle;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyBorder)
				return column.ActualBorder.HorizontalLineStyle;
			return base.GetActualBorderHorizontalLineStyle();
		}
		protected override XlBorderLineStyle GetActualBorderVerticalLineStyle() {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyBorder)
				return row.ActualBorder.VerticalLineStyle;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyBorder)
				return column.ActualBorder.VerticalLineStyle;
			return base.GetActualBorderVerticalLineStyle();
		}
		protected override int GetActualBorderHorizontalColorIndex() {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyBorder)
				return row.ActualBorder.HorizontalColorIndex;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyBorder)
				return column.ActualBorder.HorizontalColorIndex;
			return base.GetActualBorderHorizontalColorIndex();
		}
		protected override int GetActualBorderVerticalColorIndex() {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyBorder)
				return row.ActualBorder.VerticalColorIndex;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyBorder)
				return column.ActualBorder.VerticalColorIndex;
			return base.GetActualBorderVerticalColorIndex();
		}
		protected override bool GetActualBorderOutline() {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyBorder)
				return row.ActualBorder.Outline;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyBorder)
				return column.ActualBorder.Outline;
			return base.GetActualBorderOutline();
		}
		#endregion
		#region GetActualProtectionValue
		protected override bool GetActualProtectionLocked() {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyProtection)
				return row.ActualProtection.Locked;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyProtection)
				return column.ActualProtection.Locked;
			return base.GetActualProtectionLocked();
		}
		protected override bool GetActualProtectionHidden() {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ActualApplyInfo.ApplyProtection)
				return row.ActualProtection.Hidden;
			IColumnRange column = Worksheet.Columns.TryGetColumnRange(ColumnIndex);
			if (column != null && column.ActualApplyInfo.ApplyProtection)
				return column.ActualProtection.Hidden;
			return base.GetActualProtectionHidden();
		}
		#endregion
		#region GetActualStyle
		protected override CellStyleBase GetActualStyle() {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null)
				return row.Style;
			Column column = Worksheet.Columns.TryGetColumn(ColumnIndex);
			if (column != null)
				return column.Style;
			return base.GetActualStyle();
		}
		#endregion
		public override VariantValue GetValue() {
			return VariantValue.Empty;
		}
		public override string GetText() {
			return String.Empty;
		}
		public override NumberFormatResult GetFormatResult(NumberFormatParameters parameters) {
			return FakeCell.FormatResult;
		}
	}
	#endregion
	#region FakeCellWithColumnFormatting
	public class FakeCellWithColumnFormatting : Cell {
		readonly IColumnRange columnRange;
		public FakeCellWithColumnFormatting(IColumnRange columnRange, int columnIndex, int rowIndex)
			: base(new CellPosition(columnIndex, rowIndex, PositionType.Absolute, PositionType.Absolute), columnRange.Sheet) {
			Guard.ArgumentNotNull(columnRange, "columnRange");
			System.Diagnostics.Debug.Assert(columnIndex >= columnRange.StartIndex);
			System.Diagnostics.Debug.Assert(columnIndex <= columnRange.EndIndex);
			this.columnRange = columnRange;
		}
		IActualBorderInfo ColumnRangeActualBorder { get { return columnRange.ActualBorder; } }
		IActualFillInfo ColumnRangeActualFill { get { return columnRange.ActualFill; } }
		IActualApplyInfo ColumnRangeActualApplyInfo { get { return columnRange.ActualApplyInfo; } }
		#region GetActualBorderValue
		protected override T GetActualBorderValue<T>(T cellFormatActualValue, DifferentialFormatPropertyDescriptor propertyDescriptor) {
			return GetActualBorderValueCore<T>(cellFormatActualValue, ColumnRangeActualApplyInfo.ApplyBorder, propertyDescriptor);
		}
		protected override XlBorderLineStyle GetActualBorderLeftLineStyle() {
			return GetActualDisplayBorderValue(ColumnRangeActualBorder.LeftLineStyle, DifferentialFormatDisplayBorderDescriptor.LeftLineStyle);
		}
		protected override XlBorderLineStyle GetActualBorderRightLineStyle() {
			return GetActualDisplayBorderValue(ColumnRangeActualBorder.RightLineStyle, DifferentialFormatDisplayBorderDescriptor.RightLineStyle);
		}
		protected override XlBorderLineStyle GetActualBorderTopLineStyle() {
			return GetActualDisplayBorderValue(ColumnRangeActualBorder.TopLineStyle, DifferentialFormatDisplayBorderDescriptor.TopLineStyle);
		}
		protected override XlBorderLineStyle GetActualBorderBottomLineStyle() {
			return GetActualDisplayBorderValue(ColumnRangeActualBorder.BottomLineStyle, DifferentialFormatDisplayBorderDescriptor.BottomLineStyle);
		}
		protected override int GetActualBorderLeftColorIndex() {
			return GetActualDisplayBorderValue(ColumnRangeActualBorder.LeftColorIndex, DifferentialFormatDisplayBorderDescriptor.LeftColorIndex);
		}
		protected override int GetActualBorderRightColorIndex() {
			return GetActualDisplayBorderValue(ColumnRangeActualBorder.RightColorIndex, DifferentialFormatDisplayBorderDescriptor.RightColorIndex);
		}
		protected override int GetActualBorderTopColorIndex() {
			return GetActualDisplayBorderValue(ColumnRangeActualBorder.TopColorIndex, DifferentialFormatDisplayBorderDescriptor.TopColorIndex);
		}
		protected override int GetActualBorderBottomColorIndex() {
			return GetActualDisplayBorderValue(ColumnRangeActualBorder.BottomColorIndex, DifferentialFormatDisplayBorderDescriptor.BottomColorIndex);
		}
		protected override XlBorderLineStyle GetActualBorderDiagonalUpLineStyle() {
			return ColumnRangeActualBorder.DiagonalUpLineStyle;
		}
		protected override XlBorderLineStyle GetActualBorderDiagonalDownLineStyle() {
			return ColumnRangeActualBorder.DiagonalDownLineStyle;
		}
		protected override XlBorderLineStyle GetActualBorderHorizontalLineStyle() {
			return ColumnRangeActualBorder.HorizontalLineStyle;
		}
		protected override XlBorderLineStyle GetActualBorderVerticalLineStyle() {
			return ColumnRangeActualBorder.VerticalLineStyle;
		}
		protected override int GetActualBorderDiagonalColorIndex() {
			return ColumnRangeActualBorder.DiagonalColorIndex;
		}
		protected override int GetActualBorderHorizontalColorIndex() {
			return ColumnRangeActualBorder.HorizontalColorIndex;
		}
		protected override int GetActualBorderVerticalColorIndex() {
			return ColumnRangeActualBorder.VerticalColorIndex;
		}
		protected override bool GetActualBorderOutline() {
			return ColumnRangeActualBorder.Outline;
		}
		#endregion
		#region GetActualFillValue
		protected override T GetActualFillValue<T>(T cellFormatActualValue, DifferentialFormatPropertyDescriptor propertyDescriptor) {
			return GetActualFormatValue<T>(cellFormatActualValue, ColumnRangeActualApplyInfo.ApplyFill, propertyDescriptor);
		}
		protected override bool GetActualApplyFillValue(DifferentialFormatPropertyDescriptor propertyDescriptor) {
			return GetActualApplyFormatValue(ColumnRangeActualApplyInfo.ApplyFill, propertyDescriptor);
		}
		protected override XlPatternType GetActualFillPatternType() {
			return GetActualFillValue(ColumnRangeActualFill.PatternType, DifferentialFormatPropertyDescriptor.FillPatternType);
		}
		protected override int GetActualFillForeColorIndex() {
			return GetActualFillValue(ColumnRangeActualFill.ForeColorIndex, DifferentialFormatPropertyDescriptor.FillForeColorIndex);
		}
		protected override int GetActualFillBackColorIndex() {
			return GetActualFillValue(ColumnRangeActualFill.BackColorIndex, DifferentialFormatPropertyDescriptor.FillBackColorIndex);
		}
		protected override bool GetActualFillApplyPatternType() {
			return GetActualApplyFillValue(DifferentialFormatPropertyDescriptor.FillPatternType);
		}
		protected override bool GetActualFillApplyForeColor() {
			return GetActualApplyFillValue(DifferentialFormatPropertyDescriptor.FillForeColorIndex);
		}
		protected override bool GetActualFillApplyBackColor() {
			return GetActualApplyFillValue(DifferentialFormatPropertyDescriptor.FillBackColorIndex);
		}
		protected override ModelFillType GetActualFillType() {
			return GetActualFillValue(ColumnRangeActualFill.FillType, DifferentialFormatPropertyDescriptor.FillType);
		}
		protected override ModelGradientFillType GetActualFillGradientFillType() {
			return GetActualFillValue(ColumnRangeActualFill.GradientFill.Type, DifferentialFormatPropertyDescriptor.FillGradientFillType);
		}
		protected override double GetActualFillGradientFillDegree() {
			return GetActualFillValue(ColumnRangeActualFill.GradientFill.Degree, DifferentialFormatPropertyDescriptor.FillGradientFillDegree);
		}
		protected override IActualGradientStopCollection GetActualFillGradientFillGradientStops() {
			return GetActualFillValue(ColumnRangeActualFill.GradientFill.GradientStops, DifferentialFormatPropertyDescriptor.FillGradientFillGradientStops);
		}
		protected override float GetActualFillGradientFillConvergenceLeft() {
			return GetActualFillValue(ColumnRangeActualFill.GradientFill.Convergence.Left, DifferentialFormatPropertyDescriptor.FillGradientFillCovergenceLeft);
		}
		protected override float GetActualFillGradientFillConvergenceRight() {
			return GetActualFillValue(ColumnRangeActualFill.GradientFill.Convergence.Right, DifferentialFormatPropertyDescriptor.FillGradientFillCovergenceRight);
		}
		protected override float GetActualFillGradientFillConvergenceTop() {
			return GetActualFillValue(ColumnRangeActualFill.GradientFill.Convergence.Top, DifferentialFormatPropertyDescriptor.FillGradientFillCovergenceTop);
		}
		protected override float GetActualFillGradientFillConvergenceBottom() {
			return GetActualFillValue(ColumnRangeActualFill.GradientFill.Convergence.Bottom, DifferentialFormatPropertyDescriptor.FillGradientFillCovergenceBottom);
		}
		#endregion
		public override VariantValue GetValue() {
			return VariantValue.Empty;
		}
		public override string GetText() {
			return String.Empty;
		}
		public override NumberFormatResult GetFormatResult(NumberFormatParameters parameters) {
			return FakeCell.FormatResult;
		}
	}
	#endregion
	#region FakeCellWithRowFormatting
	public class FakeCellWithRowFormatting : Cell {
		readonly Row row;
		public FakeCellWithRowFormatting(int columnIndex, Row row)
			: base(new CellPosition(columnIndex, row.Index, PositionType.Absolute, PositionType.Absolute), (Worksheet)row.Sheet) {
			this.row = row;
		}
		IActualBorderInfo RowActualBorder { get { return row.ActualBorder; } }
		IActualFillInfo RowActualFill { get { return row.ActualFill; } }
		IActualApplyInfo RowActualApplyInfo { get { return row.ActualApplyInfo; } }
		#region GetActualBorderValue
		protected override T GetActualBorderValue<T>(T cellFormatActualValue, DifferentialFormatPropertyDescriptor propertyDescriptor) {
			return GetActualBorderValueCore<T>(cellFormatActualValue, RowActualApplyInfo.ApplyBorder, propertyDescriptor);
		}
		protected override XlBorderLineStyle GetActualBorderLeftLineStyle() {
			return GetActualDisplayBorderValue(RowActualBorder.LeftLineStyle, DifferentialFormatDisplayBorderDescriptor.LeftLineStyle);
		}
		protected override XlBorderLineStyle GetActualBorderRightLineStyle() {
			return GetActualDisplayBorderValue(RowActualBorder.RightLineStyle, DifferentialFormatDisplayBorderDescriptor.RightLineStyle);
		}
		protected override XlBorderLineStyle GetActualBorderTopLineStyle() {
			return GetActualDisplayBorderValue(RowActualBorder.TopLineStyle, DifferentialFormatDisplayBorderDescriptor.TopLineStyle);
		}
		protected override XlBorderLineStyle GetActualBorderBottomLineStyle() {
			return GetActualDisplayBorderValue(RowActualBorder.BottomLineStyle, DifferentialFormatDisplayBorderDescriptor.BottomLineStyle);
		}
		protected override int GetActualBorderLeftColorIndex() {
			return GetActualDisplayBorderValue(RowActualBorder.LeftColorIndex, DifferentialFormatDisplayBorderDescriptor.LeftColorIndex);
		}
		protected override int GetActualBorderRightColorIndex() {
			return GetActualDisplayBorderValue(RowActualBorder.RightColorIndex, DifferentialFormatDisplayBorderDescriptor.RightColorIndex);
		}
		protected override int GetActualBorderTopColorIndex() {
			return GetActualDisplayBorderValue(RowActualBorder.TopColorIndex, DifferentialFormatDisplayBorderDescriptor.TopColorIndex);
		}
		protected override int GetActualBorderBottomColorIndex() {
			return GetActualDisplayBorderValue(RowActualBorder.BottomColorIndex, DifferentialFormatDisplayBorderDescriptor.BottomColorIndex);
		}
		protected override XlBorderLineStyle GetActualBorderDiagonalUpLineStyle() {
			return RowActualBorder.DiagonalUpLineStyle;
		}
		protected override XlBorderLineStyle GetActualBorderDiagonalDownLineStyle() {
			return RowActualBorder.DiagonalDownLineStyle;
		}
		protected override XlBorderLineStyle GetActualBorderHorizontalLineStyle() {
			return RowActualBorder.HorizontalLineStyle;
		}
		protected override XlBorderLineStyle GetActualBorderVerticalLineStyle() {
			return RowActualBorder.VerticalLineStyle;
		}
		protected override int GetActualBorderDiagonalColorIndex() {
			return RowActualBorder.DiagonalColorIndex;
		}
		protected override int GetActualBorderHorizontalColorIndex() {
			return RowActualBorder.HorizontalColorIndex;
		}
		protected override int GetActualBorderVerticalColorIndex() {
			return RowActualBorder.VerticalColorIndex;
		}
		protected override bool GetActualBorderOutline() {
			return RowActualBorder.Outline;
		}
		#endregion
		#region GetActualFillValue
		protected override T GetActualFillValue<T>(T cellFormatActualValue, DifferentialFormatPropertyDescriptor propertyDescriptor) {
			return GetActualFormatValue<T>(cellFormatActualValue, RowActualApplyInfo.ApplyFill, propertyDescriptor);
		}
		protected override bool GetActualApplyFillValue(DifferentialFormatPropertyDescriptor propertyDescriptor) {
			return GetActualApplyFormatValue(RowActualApplyInfo.ApplyFill, propertyDescriptor);
		}
		protected override XlPatternType GetActualFillPatternType() {
			return GetActualFillValue(RowActualFill.PatternType, DifferentialFormatPropertyDescriptor.FillPatternType);
		}
		protected override int GetActualFillForeColorIndex() {
			return GetActualFillValue(RowActualFill.ForeColorIndex, DifferentialFormatPropertyDescriptor.FillForeColorIndex);
		}
		protected override int GetActualFillBackColorIndex() {
			return GetActualFillValue(RowActualFill.BackColorIndex, DifferentialFormatPropertyDescriptor.FillBackColorIndex);
		}
		protected override bool GetActualFillApplyPatternType() {
			return GetActualApplyFillValue(DifferentialFormatPropertyDescriptor.FillPatternType);
		}
		protected override bool GetActualFillApplyForeColor() {
			return GetActualApplyFillValue(DifferentialFormatPropertyDescriptor.FillForeColorIndex);
		}
		protected override bool GetActualFillApplyBackColor() {
			return GetActualApplyFillValue(DifferentialFormatPropertyDescriptor.FillBackColorIndex);
		}
		protected override ModelFillType GetActualFillType() {
			return GetActualFillValue(RowActualFill.FillType, DifferentialFormatPropertyDescriptor.FillType);
		}
		protected override ModelGradientFillType GetActualFillGradientFillType() {
			return GetActualFillValue(RowActualFill.GradientFill.Type, DifferentialFormatPropertyDescriptor.FillGradientFillType);
		}
		protected override double GetActualFillGradientFillDegree() {
			return GetActualFillValue(RowActualFill.GradientFill.Degree, DifferentialFormatPropertyDescriptor.FillGradientFillDegree);
		}
		protected override IActualGradientStopCollection GetActualFillGradientFillGradientStops() {
			return GetActualFillValue(RowActualFill.GradientFill.GradientStops, DifferentialFormatPropertyDescriptor.FillGradientFillGradientStops);
		}
		protected override float GetActualFillGradientFillConvergenceLeft() {
			return GetActualFillValue(RowActualFill.GradientFill.Convergence.Left, DifferentialFormatPropertyDescriptor.FillGradientFillCovergenceLeft);
		}
		protected override float GetActualFillGradientFillConvergenceRight() {
			return GetActualFillValue(RowActualFill.GradientFill.Convergence.Right, DifferentialFormatPropertyDescriptor.FillGradientFillCovergenceRight);
		}
		protected override float GetActualFillGradientFillConvergenceTop() {
			return GetActualFillValue(RowActualFill.GradientFill.Convergence.Top, DifferentialFormatPropertyDescriptor.FillGradientFillCovergenceTop);
		}
		protected override float GetActualFillGradientFillConvergenceBottom() {
			return GetActualFillValue(RowActualFill.GradientFill.Convergence.Bottom, DifferentialFormatPropertyDescriptor.FillGradientFillCovergenceBottom);
		}
		#endregion
		public override VariantValue GetValue() {
			return VariantValue.Empty;
		}
		public override string GetText() {
			return String.Empty;
		}
		public override NumberFormatResult GetFormatResult(NumberFormatParameters parameters) {
			return FakeCell.FormatResult;
		}
	}
	#endregion
	#region FakeCellWithValue
	public class FakeCellWithValue : FakeCell {
		readonly VariantValue value;
		public FakeCellWithValue(CellPosition position, Worksheet sheet, VariantValue value)
			: base(position, sheet) {
			this.value = value;
		}
		public override VariantValue GetValue() {
			return value;
		}
		public override string GetText() {
			VariantValue value = GetValue();
			if (HasError)
				return CellErrorFactory.GetErrorName(Error, Context);
			return ActualFormat.Format(value, Context).Text;
		}
		public override NumberFormatResult GetFormatResult(NumberFormatParameters parameters) {
			return GetFormatResultCore(ActualFormat, parameters);
		}
	}
	#endregion
	#region CellPositionWithActualBorderBase
	public abstract class CellPositionWithActualBorderBase : Cell {
		#region Fields
		XlBorderLineStyle emptyLineStyle;
		ActualBorderInfo info;
		#endregion
		protected CellPositionWithActualBorderBase(CellPosition position, Worksheet sheet)
			: base(position, sheet) {
			info = new ActualBorderInfo();
		}
		#region Properties
		protected internal XlBorderLineStyle EmptyLineStyle { get { return emptyLineStyle; } set { emptyLineStyle = value; } }
		protected internal ActualBorderInfo Info { get { return info; } }
		#endregion
		#region GetActualBorderValue
		protected override XlBorderLineStyle GetActualBorderLeftLineStyle() {
			return GetLineStyle(ActualCellBorderAccessor.Left);
		}
		protected override XlBorderLineStyle GetActualBorderRightLineStyle() {
			return GetLineStyle(ActualCellBorderAccessor.Right);
		}
		protected override XlBorderLineStyle GetActualBorderTopLineStyle() {
			return GetLineStyle(ActualCellBorderAccessor.Top);
		}
		protected override XlBorderLineStyle GetActualBorderBottomLineStyle() {
			return GetLineStyle(ActualCellBorderAccessor.Bottom);
		}
		protected override int GetActualBorderLeftColorIndex() {
			return GetColorIndex(ActualCellBorderAccessor.Left);
		}
		protected override int GetActualBorderRightColorIndex() {
			return GetColorIndex(ActualCellBorderAccessor.Right);
		}
		protected override int GetActualBorderTopColorIndex() {
			return GetColorIndex(ActualCellBorderAccessor.Top);
		}
		protected override int GetActualBorderBottomColorIndex() {
			return GetColorIndex(ActualCellBorderAccessor.Bottom);
		}
		#endregion
		XlBorderLineStyle GetLineStyle(ActualCellBorderAccessor accessor) {
			return info.CreateBorderElement(CreateBorderElement, accessor).LineStyle;
		}
		int GetColorIndex(ActualCellBorderAccessor accessor) {
			return info.CreateBorderElement(CreateBorderElement, accessor).ColorIndex;
		}
		protected ActualBorderElementInfo CreateBorderElementCore(ActualBorderElementInfo info, bool hasVisibleFill) {
			if (info.IsEmptyBorder) {
				if (hasVisibleFill)
					return ActualBorderElementInfo.CreateDefault(EmptyLineStyle);
			}
			return info;
		}
		protected abstract ActualBorderElementInfo CreateBorderElement(ActualCellBorderAccessor accessor);
	}
	#endregion
	#region CellWithActualBorderBase (abstract class)
	public abstract class CellWithActualBorderBase : CellPositionWithActualBorderBase {
		CellFormat overridenFormatInfo;
		protected CellWithActualBorderBase(CellPosition position, Worksheet sheet)
			: base(position, sheet) {
		}
		protected internal CellFormat OverridenFormatInfo { get { return overridenFormatInfo; } set { overridenFormatInfo = value; } }
	}
	#endregion
	#region CellWithActualBorder
	public class CellWithActualBorder : CellWithActualBorderBase {
		public CellWithActualBorder(CellPosition position, Worksheet sheet)
			: base(position, sheet) {
		}
		protected override ActualBorderElementInfo CreateBorderElement(ActualCellBorderAccessor accessor) {
			ActualBorderElementInfo actualInfo = accessor.CreateActualBorder(OverridenFormatInfo.ActualBorderInfo);
			return CreateBorderElementCore(actualInfo, OverridenFormatInfo.HasVisibleActualFill);
		}
	}
	#endregion
	#region CellWithActualTableBorder
	public class CellWithActualTableBorder : CellWithActualBorderBase {
		ITableBase table;
		public CellWithActualTableBorder(CellPosition position, Worksheet sheet)
			: base(position, sheet) {
		}
		protected internal ITableBase Table { get { return table; } set { table = value; } }
		protected override ActualBorderElementInfo CreateBorderElement(ActualCellBorderAccessor accessor) {
			ActualBorderElementInfo defaultInfo = accessor.CreateActualBorder(OverridenFormatInfo.ActualBorderInfo);
			return CreateBorderElement(accessor, defaultInfo, OverridenFormatInfo.HasVisibleActualFill);
		}
		ActualBorderElementInfo CreateBorderElement(ActualCellBorderAccessor accessor, ActualBorderElementInfo defaultInfo, bool hasVisibleFill) {
			if (defaultInfo.IsEmptyBorder) {
				ActualBorderElementInfo tableInfo = CreateTableBorderElement(accessor, defaultInfo);
				if (!hasVisibleFill)
					hasVisibleFill = GetHasVisibleTableFill();
				return CreateBorderElementCore(tableInfo, hasVisibleFill);
			}
			return defaultInfo;
		}
		ActualBorderElementInfo CreateTableBorderElement(ActualCellBorderAccessor accessor, ActualBorderElementInfo defaultInfo) {
			return TableStyleFormatBuilderFactory.DisplayBorderBuilder.Build(table, accessor.Descriptor, Position, defaultInfo);
		}
		bool GetHasVisibleTableFill() {
			return TableStyleFormatBuilderFactory.ApplyPropertyBuilder.Build(table, DifferentialFormatPropertyDescriptor.HasVisibleFill, Position);
		}
	}
	#endregion
	#region CellWithActualMergedRangeBorder
	public class CellWithActualMergedRangeBorder : CellWithActualBorderBase {
		#region Fields
		CellPosition topLeftMergedRange;
		CellPosition bottomRightMergedRange;
		CellPosition hostPosition;
		#endregion
		public CellWithActualMergedRangeBorder(CellPosition position, Worksheet sheet)
			: base(position, sheet) {
		}
		#region Properties
		protected internal CellPosition TopLeftMergedRange { get { return topLeftMergedRange; } set { topLeftMergedRange = value; } }
		protected internal CellPosition BottomRightMergedRange { get { return bottomRightMergedRange; } set { bottomRightMergedRange = value; } }
		protected internal CellPosition HostPosition { get { return hostPosition; } set { hostPosition = value; } }
		#endregion
		protected override ActualBorderElementInfo CreateBorderElement(ActualCellBorderAccessor accessor) {
			if (accessor.CheckEmptyMergedBorder(Position, topLeftMergedRange, bottomRightMergedRange))
				return ActualBorderElementInfo.CreateDefault(EmptyLineStyle);
			bool hasVisibleFill = OverridenFormatInfo.HasVisibleActualFill;
			if (!hasVisibleFill)
				hasVisibleFill = Worksheet[hostPosition].FormatInfo.HasVisibleActualFill;
			ActualBorderElementInfo actualInfo = accessor.CreateActualBorder(OverridenFormatInfo.ActualBorderInfo);
			return CreateBorderElementCore(actualInfo, hasVisibleFill);
		}
	}
	#endregion
	#region CellWithActualConditionalFormattingBorder
	public class CellWithActualConditionalFormattingBorder : CellWithActualBorderBase {
		ConditionalFormattingFormatAccumulator accumulator;
		public CellWithActualConditionalFormattingBorder(CellPosition position, Worksheet sheet)
			: base(position, sheet) {
		}
		protected internal ConditionalFormattingFormatAccumulator Accumulator { get { return accumulator; } set { accumulator = value; } }
		protected override ActualBorderElementInfo CreateBorderElement(ActualCellBorderAccessor accessor) {
			bool hasVisibleFill = accumulator.HasVisibleFill(DocumentModel) || OverridenFormatInfo.HasVisibleActualFill;
			BorderInfo borderInfo = accumulator.BorderAssigned ? DocumentModel.Cache.BorderInfoCache[accumulator.BorderIndex] : OverridenFormatInfo.ActualBorderInfo;
			ActualBorderElementInfo actualInfo = accessor.CreateActualBorder(borderInfo);
			return CreateBorderElementCore(actualInfo, hasVisibleFill);
		}
	}
	#endregion
	#region CellWithActualConditionalFormattingTableBorder
	public class CellWithActualConditionalFormattingTableBorder : CellWithActualBorderBase {
		#region Fields
		ConditionalFormattingFormatAccumulator accumulator;
		ITableBase table;
		#endregion
		public CellWithActualConditionalFormattingTableBorder(CellPosition position, Worksheet sheet)
			: base(position, sheet) {
		}
		#region Fields
		protected internal ConditionalFormattingFormatAccumulator Accumulator { get { return accumulator; } set { accumulator = value; } }
		protected internal ITableBase Table { get { return table; } set { table = value; } }
		#endregion
		protected override ActualBorderElementInfo CreateBorderElement(ActualCellBorderAccessor accessor) {
			bool hasVisibleFill = accumulator.HasVisibleFill(DocumentModel) || OverridenFormatInfo.HasVisibleActualFill || GetTableHasVisibleFill();
			ActualBorderElementInfo actualInfo;
			if (accumulator.BorderAssigned) {
				BorderInfo accumulatorInfo = DocumentModel.Cache.BorderInfoCache[accumulator.BorderIndex];
				actualInfo = accessor.CreateActualBorder(accumulatorInfo);
				return CreateBorderElementCore(actualInfo, hasVisibleFill);
			}
			actualInfo = accessor.CreateActualBorder(OverridenFormatInfo.ActualBorderInfo);
			if (actualInfo.IsEmptyBorder) {
				ActualBorderElementInfo tableInfo = CreateTableBorderElement(accessor, actualInfo);
				return CreateBorderElementCore(tableInfo, hasVisibleFill);
			}
			return actualInfo;
		}
		ActualBorderElementInfo CreateTableBorderElement(ActualCellBorderAccessor accessor, ActualBorderElementInfo defaultInfo) {
			return TableStyleFormatBuilderFactory.DisplayBorderBuilder.Build(table, accessor.Descriptor, Position, defaultInfo);
		}
		bool GetTableHasVisibleFill() {
			return TableStyleFormatBuilderFactory.ApplyPropertyBuilder.Build(table, DifferentialFormatPropertyDescriptor.HasVisibleFill, Position);
		}
	}
	#endregion
	#region CellWithActualConditionalFormattingMergedRangeBorder
	public class CellWithActualConditionalFormattingMergedRangeBorder : CellWithActualBorderBase {
		#region Fields
		CellPosition topLeftMergedRange;
		CellPosition bottomRightMergedRange;
		CellPosition hostPosition;
		ConditionalFormattingFormatAccumulator accumulator;
		#endregion
		public CellWithActualConditionalFormattingMergedRangeBorder(CellPosition position, Worksheet sheet)
			: base(position, sheet) {
		}
		#region Properties
		protected internal CellPosition TopLeftMergedRange { get { return topLeftMergedRange; } set { topLeftMergedRange = value; } }
		protected internal CellPosition BottomRightMergedRange { get { return bottomRightMergedRange; } set { bottomRightMergedRange = value; } }
		protected internal ConditionalFormattingFormatAccumulator Accumulator { get { return accumulator; } set { accumulator = value; } }
		protected internal CellPosition HostPosition { get { return hostPosition; } set { hostPosition = value; } }
		#endregion
		protected override ActualBorderElementInfo CreateBorderElement(ActualCellBorderAccessor accessor) {
			if (accessor.CheckEmptyMergedBorder(Position, TopLeftMergedRange, BottomRightMergedRange))
				return ActualBorderElementInfo.CreateDefault(EmptyLineStyle);
			bool hasVisibleFill = GetActualHasVisibleFill();
			BorderInfo borderInfo = GetActualBorderInfo();
			ActualBorderElementInfo actualInfo = accessor.CreateActualBorder(borderInfo);
			return CreateBorderElementCore(actualInfo, hasVisibleFill);
		}
		bool GetActualHasVisibleFill() {
			ICell hostCell = Worksheet[hostPosition];
			ConditionalFormattingFormatAccumulator hostAccumulator = hostCell.ConditionalFormatAccumulator;
			if (hostAccumulator != null) {
				hostAccumulator.Update(hostCell);
				if (hostAccumulator.HasVisibleFill(DocumentModel))
					return true;
			}
			return accumulator.HasVisibleFill(DocumentModel) || hostCell.FormatInfo.HasVisibleActualFill || OverridenFormatInfo.HasVisibleActualFill;
		}
		BorderInfo GetActualBorderInfo() {
			ICell hostCell = Worksheet[hostPosition];
			ConditionalFormattingFormatAccumulator hostAccumulator = hostCell.ConditionalFormatAccumulator;
			if (hostAccumulator != null) {
				hostAccumulator.Update(hostCell);
				if (hostAccumulator.BorderAssigned)
					return DocumentModel.Cache.BorderInfoCache[hostAccumulator.BorderIndex];
			}
			if (accumulator.BorderAssigned)
				return DocumentModel.Cache.BorderInfoCache[accumulator.BorderIndex];
			return OverridenFormatInfo.ActualBorderInfo;
		}
	}
	#endregion
	#region FakeCellWithActualBorderBase (abstract class)
	public abstract class FakeCellWithActualBorderBase : CellPositionWithActualBorderBase {
		IActualBorderInfo defaultBorderInfo;
		protected FakeCellWithActualBorderBase(CellPosition position, Worksheet sheet)
			: base(position, sheet) {
		}
		protected internal IActualBorderInfo DefaultBorderInfo { get { return defaultBorderInfo; } set { defaultBorderInfo = value; } }
	}
	#endregion
	#region FakeCellWithActualBorder
	public class FakeCellWithActualBorder : FakeCellWithActualBorderBase {
		public FakeCellWithActualBorder(CellPosition position, Worksheet sheet)
			: base(position, sheet) {
		}
		protected override ActualBorderElementInfo CreateBorderElement(ActualCellBorderAccessor accessor) {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ApplyStyle)
				return CreateBorderElement(accessor, row.FormatInfo);
			Column column = Worksheet.Columns.TryGetColumn(ColumnIndex);
			if (column != null)
				return CreateBorderElement(accessor, column.FormatInfo);
			return accessor.CreateActualBorder(DefaultBorderInfo);
		}
		ActualBorderElementInfo CreateBorderElement(ActualCellBorderAccessor accessor, CellFormat format) {
			ActualBorderElementInfo actualInfo = accessor.CreateActualBorder(format.ActualBorderInfo);
			return CreateBorderElementCore(actualInfo, format.HasVisibleActualFill);
		}
	}
	#endregion
	#region FakeCellWithActualTableBorder
	public class FakeCellWithActualTableBorder : FakeCellWithActualBorderBase {
		ITableBase table;
		public FakeCellWithActualTableBorder(CellPosition position, Worksheet sheet)
			: base(position, sheet) {
		}
		protected internal ITableBase Table { get { return table; } set { table = value; } }
		protected override ActualBorderElementInfo CreateBorderElement(ActualCellBorderAccessor accessor) {
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null)
				return CreateBorderElement(accessor, row.FormatInfo);
			Column column = Worksheet.Columns.TryGetColumn(ColumnIndex);
			if (column != null)
				return CreateBorderElement(accessor, column.FormatInfo);
			ActualBorderElementInfo defaultInfo = accessor.CreateActualBorder(DefaultBorderInfo);
			ActualBorderElementInfo tableInfo = CreateTableBorderElement(accessor, defaultInfo);
			bool hasVisibleFill = GetHasVisibleTableFill();
			return CreateBorderElementCore(tableInfo, hasVisibleFill);
		}
		ActualBorderElementInfo CreateBorderElement(ActualCellBorderAccessor accessor, CellFormat format) {
			ActualBorderElementInfo actualInfo = accessor.CreateActualBorder(format.ActualBorderInfo);
			return CreateBorderElement(accessor, actualInfo, format.HasVisibleActualFill);
		}
		ActualBorderElementInfo CreateBorderElement(ActualCellBorderAccessor accessor, ActualBorderElementInfo defaultInfo, bool hasVisibleFill) {
			if (defaultInfo.IsEmptyBorder) {
				ActualBorderElementInfo tableInfo = CreateTableBorderElement(accessor, defaultInfo);
				if (!hasVisibleFill)
					hasVisibleFill = GetHasVisibleTableFill();
				return CreateBorderElementCore(tableInfo, hasVisibleFill);
			}
			return defaultInfo;
		}
		ActualBorderElementInfo CreateTableBorderElement(ActualCellBorderAccessor accessor, ActualBorderElementInfo defaultInfo) {
			return TableStyleFormatBuilderFactory.DisplayBorderBuilder.Build(table, accessor.Descriptor, Position, defaultInfo);
		}
		bool GetHasVisibleTableFill() {
			return TableStyleFormatBuilderFactory.ApplyPropertyBuilder.Build(table, DifferentialFormatPropertyDescriptor.HasVisibleFill, Position);
		}
	}
	#endregion
	#region FakeCellWithActualMergedRangeBorder
	public class FakeCellWithActualMergedRangeBorder : FakeCellWithActualBorderBase {
		#region Fields
		CellPosition topLeftMergedRange;
		CellPosition bottomRightMergedRange;
		CellPosition hostPosition;
		#endregion
		public FakeCellWithActualMergedRangeBorder(CellPosition position, Worksheet sheet)
			: base(position, sheet) {
		}
		#region Properties
		protected internal CellPosition TopLeftMergedRange { get { return topLeftMergedRange; } set { topLeftMergedRange = value; } }
		protected internal CellPosition BottomRightMergedRange { get { return bottomRightMergedRange; } set { bottomRightMergedRange = value; } }
		protected internal CellPosition HostPosition { get { return hostPosition; } set { hostPosition = value; } }
		#endregion
		protected override ActualBorderElementInfo CreateBorderElement(ActualCellBorderAccessor accessor) {
			if (accessor.CheckEmptyMergedBorder(Position, TopLeftMergedRange, BottomRightMergedRange))
				return ActualBorderElementInfo.CreateDefault(EmptyLineStyle);
			Row row = Worksheet.Rows.TryGetRow(RowIndex);
			if (row != null && row.ApplyStyle)
				return CreateBorderElement(accessor, row.FormatInfo);
			Column column = Worksheet.Columns.TryGetColumn(ColumnIndex);
			if (column != null)
				return CreateBorderElement(accessor, column.FormatInfo);
			ActualBorderElementInfo defaultInfo = accessor.CreateActualBorder(DefaultBorderInfo);
			return CreateBorderElementCore(defaultInfo, GetHostCellHasVisibleFill());
		}
		ActualBorderElementInfo CreateBorderElement(ActualCellBorderAccessor accessor, CellFormat format) {
			bool hasVisibleFill = format.HasVisibleActualFill;
			if (!hasVisibleFill)
				hasVisibleFill = GetHostCellHasVisibleFill();
			ActualBorderElementInfo actualInfo = accessor.CreateActualBorder(format.ActualBorderInfo);
			return CreateBorderElementCore(actualInfo, hasVisibleFill);
		}
		bool GetHostCellHasVisibleFill() {
			ICell hostCell = Worksheet.TryGetCell(hostPosition.Column, hostPosition.Row);
			return hostCell != null ? hostCell.FormatInfo.HasVisibleActualFill : false;
		}
	}
	#endregion
	#region CellPositionWithActualBorderFactory
	public static class CellPositionWithActualBorderFactory {
		public static ICell CreateCellOverriddenBorder(ICell cell, CellRangesCachedRTree mergedCellsTree) {
			ConditionalFormattingFormatAccumulator accumulator = cell.ConditionalFormatAccumulator;
			if (accumulator != null) {
				accumulator.Update(cell);
				return CreateCellWithConditionalFormattingBorder(cell, mergedCellsTree, accumulator);
			}
			return CreateCellOverriddenBorderCore(cell, mergedCellsTree);
		}
		public static ICell GetCellOverrideBorderInsideMergedCell(ICell cell, CellRangesCachedRTree mergedCellsTree) {
			CellRange mergedRange = mergedCellsTree.Search(cell.ColumnIndex, cell.RowIndex);
			if (mergedRange == null)
				return cell;
			ConditionalFormattingFormatAccumulator accumulator = cell.ConditionalFormatAccumulator;
			if (accumulator != null) {
				accumulator.Update(cell);
				return CreateCellWithConditionalFormattingMergedRangeBorder(cell, mergedRange, accumulator, XlBorderLineStyle.None);
			}
			return CreateCellWithMergedRangeBorder(cell, mergedRange, XlBorderLineStyle.None);
		}
		public static ICell CreateFakeCellWithActualBorder(Worksheet sheet, int columnIndex, int rowIndex, CellRangesCachedRTree mergedCellsTree, IActualBorderInfo baseActualBorder) {
			CellRange mergedRange = mergedCellsTree.Search(columnIndex, rowIndex);
			CellPosition position = new CellPosition(columnIndex, rowIndex, PositionType.Absolute, PositionType.Absolute);
			if (mergedRange == null) {
				ITableBase table = sheet.TryGetTableBase(position);
				if (table != null) {
					FakeCellWithActualTableBorder tableBorder = new FakeCellWithActualTableBorder(position, sheet);
					tableBorder.EmptyLineStyle = SpecialBorderLineStyle.NoneOverrideDefaultGrid;
					tableBorder.Table = table;
					tableBorder.DefaultBorderInfo = baseActualBorder;
					return tableBorder;
				}
				FakeCellWithActualBorder rowColumnBorder = new FakeCellWithActualBorder(position, sheet);
				rowColumnBorder.EmptyLineStyle = SpecialBorderLineStyle.NoneOverrideDefaultGrid;
				rowColumnBorder.DefaultBorderInfo = baseActualBorder;
				return rowColumnBorder;
			}
			return CreateFakeCellWithMergedRangeBorder(sheet, columnIndex, rowIndex, mergedRange, SpecialBorderLineStyle.NoneOverrideDefaultGrid, baseActualBorder);
		}
		static ICell CreateFakeCellWithMergedRangeBorder(Worksheet sheet, int columnIndex, int rowIndex, CellRange mergedRange, XlBorderLineStyle emptyLineStyle, IActualBorderInfo baseActualBorder) {
			CellPosition position = new CellPosition(columnIndex, rowIndex, PositionType.Absolute, PositionType.Absolute);
			FakeCellWithActualMergedRangeBorder result = new FakeCellWithActualMergedRangeBorder(position, sheet);
			result.EmptyLineStyle = emptyLineStyle;
			result.DefaultBorderInfo = baseActualBorder;
			result.TopLeftMergedRange = sheet.CalculateVisibleTopLeftPosition(mergedRange);
			result.BottomRightMergedRange = sheet.CalculateVisibleBottomRightPosition(mergedRange);
			result.HostPosition = mergedRange.TopLeft;
			return result;
		}
		static ICell CreateCellOverriddenBorderCore(ICell cell, CellRangesCachedRTree mergedCellsTree) {
			CellRange mergedRange = mergedCellsTree.Search(cell.ColumnIndex, cell.RowIndex);
			if (mergedRange == null) {
				Worksheet sheet = cell.Worksheet;
				CellFormat formatInfo = cell.FormatInfo;
				CellPosition position = cell.Position;
				bool hasVisibleFill = formatInfo.HasVisibleActualFill;
				ITableBase table = sheet.TryGetTableBase(position);
				if (table != null) {
					CellWithActualTableBorder tableBorder = new CellWithActualTableBorder(position, sheet);
					tableBorder.EmptyLineStyle = SpecialBorderLineStyle.NoneOverrideDefaultGrid;
					tableBorder.Table = table;
					tableBorder.OverridenFormatInfo = formatInfo;
					return tableBorder;
				}
				CellWithActualBorder cellBorder = new CellWithActualBorder(position, sheet);
				cellBorder.EmptyLineStyle = SpecialBorderLineStyle.NoneOverrideDefaultGrid;
				cellBorder.OverridenFormatInfo = formatInfo;
				return cellBorder;
			}
			return CreateCellWithMergedRangeBorder(cell, mergedRange, SpecialBorderLineStyle.NoneOverrideDefaultGrid);
		}
		static ICell CreateCellWithConditionalFormattingBorder(ICell cell, CellRangesCachedRTree mergedCellsTree, ConditionalFormattingFormatAccumulator accumulator) {
			CellRange mergedRange = mergedCellsTree.Search(cell.ColumnIndex, cell.RowIndex);
			if (mergedRange == null) {
				Worksheet sheet = cell.Worksheet;
				CellFormat formatInfo = cell.FormatInfo;
				CellPosition position = cell.Position;
				bool hasVisibleFill = accumulator.HasVisibleFill(cell.DocumentModel);
				ITableBase table = sheet.TryGetTableBase(position);
				if (table != null) {
					CellWithActualConditionalFormattingTableBorder tableBorder = new CellWithActualConditionalFormattingTableBorder(position, sheet);
					tableBorder.EmptyLineStyle = SpecialBorderLineStyle.NoneOverrideDefaultGrid;
					tableBorder.Table = table;
					tableBorder.Accumulator = accumulator;
					tableBorder.OverridenFormatInfo = formatInfo;
					return tableBorder;
				}
				CellWithActualConditionalFormattingBorder cellBorder = new CellWithActualConditionalFormattingBorder(position, sheet);
				cellBorder.EmptyLineStyle = SpecialBorderLineStyle.NoneOverrideDefaultGrid;
				cellBorder.Accumulator = accumulator;
				cellBorder.OverridenFormatInfo = formatInfo;
				return cellBorder;
			}
			return CreateCellWithConditionalFormattingMergedRangeBorder(cell, mergedRange, accumulator, SpecialBorderLineStyle.NoneOverrideDefaultGrid);
		}
		static ICell CreateCellWithMergedRangeBorder(ICell cell, CellRange mergedRange, XlBorderLineStyle emptyLineStyle) {
			Worksheet sheet = cell.Worksheet;
			CellFormat formatInfo = cell.FormatInfo;
			CellPosition position = cell.Position;
			CellWithActualMergedRangeBorder result = new CellWithActualMergedRangeBorder(position, sheet);
			result.TopLeftMergedRange = sheet.CalculateVisibleTopLeftPosition(mergedRange);
			result.BottomRightMergedRange = sheet.CalculateVisibleBottomRightPosition(mergedRange);
			result.EmptyLineStyle = emptyLineStyle;
			result.OverridenFormatInfo = formatInfo;
			result.HostPosition = mergedRange.TopLeft;
			return result;
		}
		static ICell CreateCellWithConditionalFormattingMergedRangeBorder(ICell cell, CellRange mergedRange, ConditionalFormattingFormatAccumulator accumulator, XlBorderLineStyle emptyLineStyle) {
			Worksheet sheet = cell.Worksheet;
			CellWithActualConditionalFormattingMergedRangeBorder result = new CellWithActualConditionalFormattingMergedRangeBorder(cell.Position, sheet);
			result.TopLeftMergedRange = sheet.CalculateVisibleTopLeftPosition(mergedRange);
			result.BottomRightMergedRange = sheet.CalculateVisibleBottomRightPosition(mergedRange);
			result.EmptyLineStyle = emptyLineStyle;
			result.OverridenFormatInfo = cell.FormatInfo;
			result.HostPosition = mergedRange.TopLeft;
			result.Accumulator = accumulator;
			return result;
		}
	}
	#endregion
}
