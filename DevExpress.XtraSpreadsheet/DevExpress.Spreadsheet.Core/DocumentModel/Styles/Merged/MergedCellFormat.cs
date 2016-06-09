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
using DevExpress.Export.Xl;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
using DevExpress.Office.Model;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model {
	#region MergedCellFormat
	public class MergedCellFormat {
		#region Fields
		MergedFormatStringInfo formatString;
		MergedAlignmentInfo alignment;
		MergedFontInfo font;
		MergedBorderInfo border;
		MergedFillInfo fill;
		MergedCellProtectionInfo protection;
		CellPosition activeCellPosition;
		#endregion
		public MergedCellFormat() {
			this.formatString = new MergedFormatStringInfo();
			this.alignment = new MergedAlignmentInfo();
			this.font = new MergedFontInfo();
			this.border = new MergedBorderInfo();
			this.fill = new MergedFillInfo();
			this.protection = new MergedCellProtectionInfo();
		}
		public MergedCellFormat(ICellFormat format) {
			Guard.ArgumentNotNull(format, "format");
			this.formatString = new MergedFormatStringInfo(format);
			this.alignment = new MergedAlignmentInfo(format.ActualAlignment);
			this.font = new MergedFontInfo(format.ActualFont);
			this.border = new MergedBorderInfo(format.ActualBorder);
			this.fill = new MergedFillInfo(format.ActualFill);
			this.protection = new MergedCellProtectionInfo(format.ActualProtection);
		}
		#region Properties
		public CellPosition ActiveCellPosition { get { return activeCellPosition; } set { activeCellPosition = value; } }
		public MergedFormatStringInfo FormatString { get { return formatString; } }
		public MergedAlignmentInfo Alignment { get { return alignment; } }
		public MergedFontInfo Font { get { return font; } }
		public MergedBorderInfo Border { get { return border; } }
		public MergedFillInfo Fill { get { return fill; } }
		public MergedCellProtectionInfo Protection { get { return protection; } }
		#endregion
	}
	#endregion
	#region MergedFormatStringInfo
	public class MergedFormatStringInfo : IFormatStringAccessor {
		public MergedFormatStringInfo() {
		}
		public MergedFormatStringInfo(IFormatStringAccessor formatString) {
			Guard.ArgumentNotNull(formatString, "formatString");
			CopyFrom(formatString);
		}
		#region Properties
		public string FormatString { get; set; }
		#endregion
		protected internal void CopyFrom(IFormatStringAccessor owner) {
			this.FormatString = owner.FormatString;
		}
	}
	#endregion
	#region MergedAlignmentInfo
	public class MergedAlignmentInfo : ICellAlignmentInfo {
		public MergedAlignmentInfo() {
		}
		public MergedAlignmentInfo(IActualCellAlignmentInfo alignment) {
			Guard.ArgumentNotNull(alignment, "alignment");
			CopyFrom(alignment);
		}
		#region Properties
		public bool? WrapText { get; set; }
		public bool? JustifyLastLine { get; set; }
		public bool? ShrinkToFit { get; set; }
		public int? TextRotation { get; set; }
		public byte? Indent { get; set; }
		public int? RelativeIndent { get; set; }
		public XlHorizontalAlignment? Horizontal { get; set; }
		public XlVerticalAlignment? Vertical { get; set; }
		public XlReadingOrder? ReadingOrder { get; set; }
		bool ICellAlignmentInfo.WrapText { get { return WrapText.Value; } set { WrapText = value; } }
		bool ICellAlignmentInfo.JustifyLastLine { get { return JustifyLastLine.Value; } set { JustifyLastLine = value; } }
		bool ICellAlignmentInfo.ShrinkToFit { get { return ShrinkToFit.Value; } set { ShrinkToFit = value; } }
		int ICellAlignmentInfo.TextRotation { get { return TextRotation.Value; } set { TextRotation = value; } }
		byte ICellAlignmentInfo.Indent { get { return Indent.Value; } set { Indent = value; } }
		int ICellAlignmentInfo.RelativeIndent { get { return RelativeIndent.Value; } set { RelativeIndent = value; } }
		XlHorizontalAlignment ICellAlignmentInfo.Horizontal { get { return Horizontal.Value; } set { Horizontal = value; } }
		XlVerticalAlignment ICellAlignmentInfo.Vertical { get { return Vertical.Value; } set { Vertical = value; } }
		XlReadingOrder ICellAlignmentInfo.ReadingOrder { get { return ReadingOrder.Value; } set { ReadingOrder = value; } }
		#endregion
		protected internal void CopyFrom(IActualCellAlignmentInfo owner) {
			this.WrapText = owner.WrapText;
			this.JustifyLastLine = owner.JustifyLastLine;
			this.ShrinkToFit = owner.ShrinkToFit;
			this.TextRotation = owner.TextRotation;
			this.Indent = owner.Indent;
			this.RelativeIndent = owner.RelativeIndent;
			this.Horizontal = owner.Horizontal;
			this.Vertical = owner.Vertical;
			this.ReadingOrder = owner.ReadingOrder;
		}
	}
	#endregion
	#region MergedFontInfo
	public class MergedFontInfo : IRunFontInfo {
		public MergedFontInfo() {
		}
		public MergedFontInfo(IActualRunFontInfo font) {
			Guard.ArgumentNotNull(font, "font");
			CopyFrom(font);
		}
		#region Fields
		public string Name { get; set; }
		public Color? Color { get; set; }
		public bool? Bold { get; set; }
		public bool? Condense { get; set; }
		public bool? Extend { get; set; }
		public bool? Italic { get; set; }
		public bool? Outline { get; set; }
		public bool? Shadow { get; set; }
		public bool? StrikeThrough { get; set; }
		public int? Charset { get; set; }
		public int? FontFamily { get; set; }
		public double? Size { get; set; }
		public XlFontSchemeStyles? SchemeStyle { get; set; }
		public XlScriptType? Script { get; set; }
		public XlUnderlineType? Underline { get; set; }
		Color IRunFontInfo.Color { get { return Color.Value; } set { Color = value; } }
		bool IRunFontInfo.Bold { get { return Bold.Value; } set { Bold = value; } }
		bool IRunFontInfo.Condense { get { return Condense.Value; } set { Condense = value; } }
		bool IRunFontInfo.Extend { get { return Extend.Value; } set { Extend = value; } }
		bool IRunFontInfo.Italic { get { return Italic.Value; } set { Italic = value; } }
		bool IRunFontInfo.Outline { get { return Outline.Value; } set { Outline = value; } }
		bool IRunFontInfo.Shadow { get { return Shadow.Value; } set { Shadow = value; } }
		bool IRunFontInfo.StrikeThrough { get { return StrikeThrough.Value; } set { StrikeThrough = value; } }
		int IRunFontInfo.Charset { get { return Charset.Value; } set { Charset = value; } }
		int IRunFontInfo.FontFamily { get { return FontFamily.Value; } set { FontFamily = value; } }
		double IRunFontInfo.Size { get { return Size.Value; } set { Size = value; } }
		XlFontSchemeStyles IRunFontInfo.SchemeStyle { get { return SchemeStyle.Value; } set { SchemeStyle = value; } }
		XlScriptType IRunFontInfo.Script { get { return Script.Value; } set { Script = value; } }
		XlUnderlineType IRunFontInfo.Underline { get { return Underline.Value; } set { Underline = value; } }
		#endregion
		protected internal void CopyFrom(IActualRunFontInfo owner) {
			this.Name = owner.Name;
			this.Color = owner.Color;
			this.Bold = owner.Bold;
			this.Condense = owner.Condense;
			this.Extend = owner.Extend;
			this.Italic = owner.Italic;
			this.Outline = owner.Outline;
			this.Shadow = owner.Shadow;
			this.StrikeThrough = owner.StrikeThrough;
			this.Charset = owner.Charset;
			this.FontFamily = owner.FontFamily;
			this.Size = owner.Size;
			this.SchemeStyle = owner.SchemeStyle;
			this.Script = owner.Script;
			this.Underline = owner.Underline;
		}
	}
	#endregion
	#region MergedBorderOptionsInfo
	public class MergedBorderOptionsInfo {
		#region Properties
		protected internal bool? ApplyLeftBorder { get; set; }
		protected internal bool? ApplyRightBorder { get; set; }
		protected internal bool? ApplyTopBorder { get; set; }
		protected internal bool? ApplyBottomBorder { get; set; }
		protected internal bool? ApplyVerticalBorder { get; set; }
		protected internal bool? ApplyHorizontalBorder { get; set; }
		protected internal bool? ApplyDiagonalUpBorder { get; set; }
		protected internal bool? ApplyDiagonalDownBorder { get; set; }
		#endregion
	}
	#endregion
	#region MergedBorderInfo
	public class MergedBorderInfo : IBorderInfo {
		#region Static Members
		public static MergedBorderInfo CreateTopBorder(XlBorderLineStyle style, Color color) {
			MergedBorderInfo result = new MergedBorderInfo();
			result.SetTopBorderLine(style, color);
			return result;
		}
		public static MergedBorderInfo CreateBottomBorder(XlBorderLineStyle style, Color color) {
			MergedBorderInfo result = new MergedBorderInfo();
			result.SetBottomBorderLine(style, color);
			return result;
		}
		public static MergedBorderInfo CreateLeftBorder(XlBorderLineStyle style, Color color) {
			MergedBorderInfo result = new MergedBorderInfo();
			result.SetLeftBorderLine(style, color);
			return result;
		}
		public static MergedBorderInfo CreateRightBorder(XlBorderLineStyle style, Color color) {
			MergedBorderInfo result = new MergedBorderInfo();
			result.SetRightBorderLine(style, color);
			return result;
		}
		public static MergedBorderInfo CreateNoBorders() {
			MergedBorderInfo result = CreateAllBorders(XlBorderLineStyle.None, DXColor.Empty);
			result.SetDiagonalUpBorderLine(XlBorderLineStyle.None, DXColor.Empty);
			result.SetDiagonalDownBorderLine(XlBorderLineStyle.None, DXColor.Empty);
			return result;
		}
		public static MergedBorderInfo CreateAllBorders(XlBorderLineStyle style, Color color) {
			MergedBorderInfo result = new MergedBorderInfo();
			result.SetOutlineBorders(style, color);
			result.SetInsideBorders(style, color);
			return result;
		}
		public static MergedBorderInfo CreateInsideBorders(XlBorderLineStyle style, Color color) {
			MergedBorderInfo result = new MergedBorderInfo();
			result.SetInsideBorders(style, color);
			return result;
		}
		public static MergedBorderInfo CreateOutlineBorders(XlBorderLineStyle style, Color color) {
			MergedBorderInfo result = new MergedBorderInfo();
			result.SetOutlineBorders(style, color);
			return result;
		}
		public static MergedBorderInfo CreateThickBoxBorder(Color color) {
			MergedBorderInfo result = new MergedBorderInfo();
			result.SetOutlineBorders(XlBorderLineStyle.Medium, color);
			return result;
		}
		public static MergedBorderInfo CreateBottomDoubleBorder(Color color) {
			MergedBorderInfo result = new MergedBorderInfo();
			result.SetBottomBorderLine(XlBorderLineStyle.Double, color);
			return result;
		}
		public static MergedBorderInfo CreateThickBottomBorder(Color color) {
			MergedBorderInfo result = new MergedBorderInfo();
			result.SetBottomBorderLine(XlBorderLineStyle.Medium, color);
			return result;
		}
		public static MergedBorderInfo CreateTopAndBottomBorder(Color color) {
			MergedBorderInfo result = new MergedBorderInfo();
			result.SetTopBorderLine(XlBorderLineStyle.Thin, color);
			result.SetBottomBorderLine(XlBorderLineStyle.Thin, color);
			return result;
		}
		public static MergedBorderInfo CreateTopAndThickBottomBorder(Color color) {
			MergedBorderInfo result = new MergedBorderInfo();
			result.SetTopBorderLine(XlBorderLineStyle.Thin, color);
			result.SetBottomBorderLine(XlBorderLineStyle.Medium, color);
			return result;
		}
		public static MergedBorderInfo CreateTopAndDoubleBottomBorder(Color color) {
			MergedBorderInfo result = new MergedBorderInfo();
			result.SetTopBorderLine(XlBorderLineStyle.Thin, color);
			result.SetBottomBorderLine(XlBorderLineStyle.Double, color);
			return result;
		}
		public static MergedBorderInfo CreateVerticalBorder(XlBorderLineStyle style, Color color) {
			MergedBorderInfo result = new MergedBorderInfo();
			result.SetVerticalBorderLine(style, color);
			return result;
		}
		public static MergedBorderInfo CreateHorizontalBorder(XlBorderLineStyle style, Color color) {
			MergedBorderInfo result = new MergedBorderInfo();
			result.SetHorizontalBorderLine(style, color);
			return result;
		}
		#endregion
		public MergedBorderInfo() {
		}
		public MergedBorderInfo(IActualBorderInfo border) {
			Guard.ArgumentNotNull(border, "border");
			CopyFrom(border);
		}
		#region Properties
		public XlBorderLineStyle? LeftLineStyle { get; set; }
		public XlBorderLineStyle? RightLineStyle { get; set; }
		public XlBorderLineStyle? TopLineStyle { get; set; }
		public XlBorderLineStyle? BottomLineStyle { get; set; }
		public XlBorderLineStyle? DiagonalUpLineStyle { get; set; }
		public XlBorderLineStyle? DiagonalDownLineStyle { get; set; }
		public XlBorderLineStyle? HorizontalLineStyle { get; set; }
		public XlBorderLineStyle? VerticalLineStyle { get; set; }
		public bool? Outline { get; set; }
		public Color? LeftColor { get; set; }
		public Color? RightColor { get; set; }
		public Color? TopColor { get; set; }
		public Color? BottomColor { get; set; }
		public Color? DiagonalColor { get; set; }
		public Color? HorizontalColor { get; set; }
		public Color? VerticalColor { get; set; }
		public int? LeftColorIndex { get; set; }
		public int? RightColorIndex { get; set; }
		public int? TopColorIndex { get; set; }
		public int? BottomColorIndex { get; set; }
		public int? DiagonalColorIndex { get; set; }
		public int? HorizontalColorIndex { get; set; }
		public int? VerticalColorIndex { get; set; }
		XlBorderLineStyle IBorderInfo.LeftLineStyle { get { return LeftLineStyle.Value; } set { LeftLineStyle = value; } }
		XlBorderLineStyle IBorderInfo.RightLineStyle { get { return RightLineStyle.Value; } set { RightLineStyle = value; } }
		XlBorderLineStyle IBorderInfo.TopLineStyle { get { return TopLineStyle.Value; } set { TopLineStyle = value; } }
		XlBorderLineStyle IBorderInfo.BottomLineStyle { get { return BottomLineStyle.Value; } set { BottomLineStyle = value; } }
		XlBorderLineStyle IBorderInfo.DiagonalUpLineStyle { get { return DiagonalUpLineStyle.Value; } set { DiagonalUpLineStyle = value; } }
		XlBorderLineStyle IBorderInfo.DiagonalDownLineStyle { get { return DiagonalDownLineStyle.Value; } set { DiagonalDownLineStyle = value; } }
		XlBorderLineStyle IBorderInfo.HorizontalLineStyle { get { return HorizontalLineStyle.Value; } set { HorizontalLineStyle = value; } }
		XlBorderLineStyle IBorderInfo.VerticalLineStyle { get { return VerticalLineStyle.Value; } set { VerticalLineStyle = value; } }
		bool IBorderInfo.Outline { get { return Outline.Value; } set { Outline = value; } }
		Color IBorderInfo.LeftColor { get { return LeftColor.Value; } set { LeftColor = value; } }
		Color IBorderInfo.RightColor { get { return RightColor.Value; } set { RightColor = value; } }
		Color IBorderInfo.TopColor { get { return TopColor.Value; } set { TopColor = value; } }
		Color IBorderInfo.BottomColor { get { return BottomColor.Value; } set { BottomColor = value; } }
		Color IBorderInfo.DiagonalColor { get { return DiagonalColor.Value; } set { DiagonalColor = value; } }
		Color IBorderInfo.HorizontalColor { get { return HorizontalColor.Value; } set { HorizontalColor = value; } }
		Color IBorderInfo.VerticalColor { get { return VerticalColor.Value; } set { VerticalColor = value; } }
		int IBorderInfo.LeftColorIndex { get { return LeftColorIndex.Value; } set { LeftColorIndex = value; } }
		int IBorderInfo.RightColorIndex { get { return RightColorIndex.Value; } set { RightColorIndex = value; } }
		int IBorderInfo.TopColorIndex { get { return TopColorIndex.Value; } set { TopColorIndex = value; } }
		int IBorderInfo.BottomColorIndex { get { return BottomColorIndex.Value; } set { BottomColorIndex = value; } }
		int IBorderInfo.DiagonalColorIndex { get { return DiagonalColorIndex.Value; } set { DiagonalColorIndex = value; } }
		int IBorderInfo.HorizontalColorIndex { get { return HorizontalColorIndex.Value; } set { HorizontalColorIndex = value; } }
		int IBorderInfo.VerticalColorIndex { get { return VerticalColorIndex.Value; } set { VerticalColorIndex = value; } }
		protected internal bool HasLeftBorder { get { return HasBorder(LeftLineStyle, LeftColor, LeftColorIndex); } }
		protected internal bool HasRightBorder { get { return HasBorder(RightLineStyle, RightColor, RightColorIndex); } }
		protected internal bool HasTopBorder { get { return HasBorder(TopLineStyle, TopColor, TopColorIndex); } }
		protected internal bool HasBottomBorder { get { return HasBorder(BottomLineStyle, BottomColor, BottomColorIndex); } }
		protected internal bool HasVerticalBorder { get { return HasBorder(VerticalLineStyle, VerticalColor, VerticalColorIndex); } }
		protected internal bool HasHorizontalBorder { get { return HasBorder(HorizontalLineStyle, HorizontalColor, HorizontalColorIndex); } }
		protected internal bool HasDiagonalUpBorder { get { return HasBorder(DiagonalUpLineStyle, DiagonalColor, DiagonalColorIndex); } }
		protected internal bool HasDiagonalDownBorder { get { return HasBorder(DiagonalDownLineStyle, DiagonalColor, DiagonalColorIndex); } }
		protected internal bool HasLeftBorderLine { get { return HasLine(LeftLineStyle, LeftColor); } }
		protected internal bool HasRightBorderLine { get { return HasLine(RightLineStyle, RightColor); } }
		protected internal bool HasTopBorderLine { get { return HasLine(TopLineStyle, TopColor); } }
		protected internal bool HasBottomBorderLine { get { return HasLine(BottomLineStyle, BottomColor); } }
		protected internal bool HasVerticalBorderLine { get { return HasLine(VerticalLineStyle, VerticalColor); } }
		protected internal bool HasHorizontalBorderLine { get { return HasLine(HorizontalLineStyle, HorizontalColor); } }
		protected internal bool HasDiagonalUpBorderLine { get { return HasLine(DiagonalUpLineStyle, DiagonalColor); } }
		protected internal bool HasDiagonalDownBorderLine { get { return HasLine(DiagonalDownLineStyle, DiagonalColor); } }
		protected internal bool HasOutlineBorders { get { return HasLeftBorder || HasRightBorder || HasTopBorder || HasBottomBorder; } }
		protected internal bool HasInsideBorders { get { return HasVerticalBorder || HasHorizontalBorder; } }
		protected internal bool HasDiagonalBorders { get { return HasDiagonalUpBorder || HasDiagonalDownBorder; } }
		protected internal bool HasOutlineOrDiagonalBorders { get { return HasOutlineBorders || HasDiagonalBorders; } }
		protected internal bool HasVerticalRangeBorders { get { return HasOutlineOrDiagonalBorders || HasHorizontalBorder; } }
		protected internal bool HasHorizontalRangeBorders { get { return HasOutlineOrDiagonalBorders || HasVerticalBorder; } }
		protected internal bool HasBorders { get { return HasOutlineBorders || HasInsideBorders || HasDiagonalBorders; } }
		protected internal bool HasBorderLines { 
			get {
				return
					HasLeftBorderLine || HasRightBorderLine || HasTopBorderLine || HasBottomBorderLine ||
					HasVerticalBorderLine || HasHorizontalBorderLine || HasDiagonalUpBorderLine || HasDiagonalDownBorderLine;
			} 
		}
		#endregion
		#region SetColorIndexes
		public void SetColorIndexes(DocumentModel workbook) {
			SetColorIndex(workbook, LeftLineStyle, LeftColor, SetLeftColorIndex);
			SetColorIndex(workbook, RightLineStyle, RightColor, SetRightColorIndex);
			SetColorIndex(workbook, TopLineStyle, TopColor, SetTopColorIndex);
			SetColorIndex(workbook, BottomLineStyle, BottomColor, SetBottomColorIndex);
			SetColorIndex(workbook, VerticalLineStyle, VerticalColor, SetVerticalColorIndex);
			SetColorIndex(workbook, HorizontalLineStyle, HorizontalColor, SetHorizontalColorIndex);
			SetDiagonalColorIndex(workbook);
		}
		void SetColorIndex(DocumentModel workbook, XlBorderLineStyle? style, Color? color, Action<int?> setter) {
			if (IsVisibleStyle(style)) {
				ColorModelInfo colorInfo = GetColorModelInfo(color);
				setter(ColorModelInfo.GetColorIndex(workbook.Cache.ColorModelInfoCache, colorInfo));
			} else if (style.HasValue)
				setter(ColorModelInfoCache.DefaultItemIndex);
		}
		bool IsVisibleStyle(XlBorderLineStyle? style) {
			return style.HasValue && style.Value != XlBorderLineStyle.None;
		}
		bool IsHasDiagonalStyle(XlBorderLineStyle? style) {
			return style.HasValue;
		}
		void SetDiagonalColorIndex(DocumentModel workbook) {
			if (IsHasDiagonalStyle(DiagonalUpLineStyle))
				SetColorIndex(workbook, DiagonalUpLineStyle, DiagonalColor, SetDiagonalColorIndex);
			if (IsHasDiagonalStyle(DiagonalDownLineStyle))
				SetColorIndex(workbook, DiagonalDownLineStyle, DiagonalColor, SetDiagonalColorIndex);
		}
		ColorModelInfo GetColorModelInfo(Color? color) {
			if (color.HasValue) {
				Color value = color.Value;
				return DXColor.IsTransparentOrEmpty(value) ? ColorModelInfo.CreateAutomatic() : ColorModelInfo.Create(value);
			}
			return ColorModelInfo.CreateAutomatic();
		}
		#endregion
		bool HasLine(XlBorderLineStyle? style, Color? color) {
			return style.HasValue && color.HasValue;
		}
		bool HasBorder(XlBorderLineStyle? style, Color? color, int? colorIndex) {
			return HasLine(style, color) && colorIndex.HasValue;
		}
		#region SetProperties
		void SetLeftBorderLine(XlBorderLineStyle? style, Color? color) {
			LeftLineStyle = style;
			LeftColor = color;
		}
		void SetRightBorderLine(XlBorderLineStyle? style, Color? color) {
			RightLineStyle = style;
			RightColor = color;
		}
		void SetTopBorderLine(XlBorderLineStyle? style, Color? color) {
			TopLineStyle = style;
			TopColor = color;
		}
		void SetBottomBorderLine(XlBorderLineStyle? style, Color? color) {
			BottomLineStyle = style;
			BottomColor = color;
		}
		void SetVerticalBorderLine(XlBorderLineStyle? style, Color? color) {
			VerticalLineStyle = style;
			VerticalColor = color;
		}
		void SetHorizontalBorderLine(XlBorderLineStyle? style, Color? color) {
			HorizontalLineStyle = style;
			HorizontalColor = color;
		}
		void SetDiagonalUpBorderLine(XlBorderLineStyle? style, Color? color) {
			DiagonalUpLineStyle = style;
			DiagonalColor = color;
		}
		void SetDiagonalDownBorderLine(XlBorderLineStyle? style, Color? color) {
			DiagonalDownLineStyle = style;
			DiagonalColor = color;
		}
		void SetLeftColorIndex(int? colorIndex) {
			LeftColorIndex = colorIndex;
		}
		void SetRightColorIndex(int? colorIndex) {
			RightColorIndex = colorIndex;
		}
		void SetTopColorIndex(int? colorIndex) {
			TopColorIndex = colorIndex;
		}
		void SetBottomColorIndex(int? colorIndex) {
			BottomColorIndex = colorIndex;
		}
		void SetVerticalColorIndex(int? colorIndex) {
			VerticalColorIndex = colorIndex;
		}
		void SetHorizontalColorIndex(int? colorIndex) {
			HorizontalColorIndex = colorIndex;
		}
		void SetDiagonalColorIndex(int? colorIndex) {
			DiagonalColorIndex = colorIndex;
		}
		void SetOutlineBorders(XlBorderLineStyle? style, Color? color) {
			SetLeftBorderLine(style, color);
			SetRightBorderLine(style, color);
			SetTopBorderLine(style, color);
			SetBottomBorderLine(style, color);
		}
		void SetInsideBorders(XlBorderLineStyle? style, Color? color) {
			SetHorizontalBorderLine(style, color);
			SetVerticalBorderLine(style, color);
		}
		#endregion
		protected internal MergedBorderInfo GetColumnInfo(CellRange columnRange, CellRange sourceRange, bool entireAllRowsSelected) {
			bool hasOutlineFirst = columnRange.LeftColumnIndex == sourceRange.LeftColumnIndex;
			bool hasOutlineLast = columnRange.RightColumnIndex == sourceRange.RightColumnIndex && !entireAllRowsSelected;
			return GetColumnInfoCore(hasOutlineFirst, hasOutlineLast);
		}
		MergedBorderInfo GetColumnInfoCore(bool hasOutlineFirst, bool hasOutlineLast) {
			MergedBorderInfo result = Clone(hasOutlineFirst, hasOutlineLast, HasTopBorderLine, HasBottomBorderLine);
			if (HasVerticalBorderLine) {
				if (!hasOutlineFirst)
					result.SetLeftBorderLine(VerticalLineStyle, VerticalColor);
				if (!hasOutlineLast)
					result.SetRightBorderLine(VerticalLineStyle, VerticalColor);
			}
			if (HasHorizontalBorderLine) {
				if (!HasTopBorderLine)
					result.SetTopBorderLine(HorizontalLineStyle, HorizontalColor);
				if (!HasBottomBorderLine)
					result.SetBottomBorderLine(HorizontalLineStyle, HorizontalColor);
			} else 
				result.SetBottomBorderLine(null, null);
			return result;
		}
		protected internal MergedBorderInfo GetRowInfo(CellRange rowRange, CellRange sourceRange) {
			bool hasOutlineFirst = rowRange.TopRowIndex == sourceRange.TopRowIndex;
			bool hasOutlineLast = rowRange.BottomRowIndex == sourceRange.BottomRowIndex;
			return GetRowInfoCore(hasOutlineFirst, hasOutlineLast);
		}
		MergedBorderInfo GetRowInfoCore(bool hasOutlineFirst, bool hasOutlineLast) {
			MergedBorderInfo result = Clone(HasLeftBorderLine, HasRightBorderLine, hasOutlineFirst, hasOutlineLast);
			if (HasVerticalBorderLine) {
				if (!HasLeftBorderLine)
					result.SetLeftBorderLine(VerticalLineStyle, VerticalColor);
				if (!HasRightBorderLine)
					result.SetRightBorderLine(VerticalLineStyle, VerticalColor);
			} else 
				result.SetRightBorderLine(null, null);
			if (HasHorizontalBorder) {
				if (!hasOutlineFirst)
					result.SetTopBorderLine(HorizontalLineStyle, HorizontalColor);
				if (!hasOutlineLast)
					result.SetBottomBorderLine(HorizontalLineStyle, HorizontalColor);
			}
			return result;
		}
		MergedBorderInfo Clone(bool hasOutlineLeft, bool hasOutlineRight, bool hasOutlineTop, bool hasOutlineBottom) {
			MergedBorderInfo result = new MergedBorderInfo();
			if (hasOutlineLeft)
				result.SetLeftBorderLine(LeftLineStyle, LeftColor);
			if (hasOutlineRight)
				result.SetRightBorderLine(RightLineStyle, RightColor);
			if (hasOutlineTop)
				result.SetTopBorderLine(TopLineStyle, TopColor);
			if (hasOutlineBottom)
				result.SetBottomBorderLine(BottomLineStyle, BottomColor);
			result.SetHorizontalBorderLine(HorizontalLineStyle, HorizontalColor);
			result.SetVerticalBorderLine(VerticalLineStyle, VerticalColor);
			result.SetDiagonalUpBorderLine(DiagonalUpLineStyle, DiagonalColor);
			result.SetDiagonalDownBorderLine(DiagonalDownLineStyle, DiagonalColor);
			return result;
		}
		protected internal MergedBorderInfo GetFixedInfo(MergedBorderOptionsInfo options) {
			MergedBorderInfo result = new MergedBorderInfo();
			SetBorderLine(options.ApplyLeftBorder, LeftLineStyle, LeftColor, result.SetLeftBorderLine);
			SetBorderLine(options.ApplyRightBorder, RightLineStyle, RightColor, result.SetRightBorderLine);
			SetBorderLine(options.ApplyTopBorder, TopLineStyle, TopColor, result.SetTopBorderLine);
			SetBorderLine(options.ApplyBottomBorder, BottomLineStyle, BottomColor, result.SetBottomBorderLine);
			SetBorderLine(options.ApplyVerticalBorder, VerticalLineStyle, VerticalColor, result.SetVerticalBorderLine);
			SetBorderLine(options.ApplyHorizontalBorder, HorizontalLineStyle, HorizontalColor, result.SetHorizontalBorderLine);
			SetDiagonalBorderLine(options.ApplyDiagonalUpBorder, options.ApplyDiagonalDownBorder, result);
			return result;
		}
		void SetDiagonalBorderLine(bool? applyDiagonalUp, bool? applyDiagonalDown, MergedBorderInfo info) {
			if (HasBorderLine(applyDiagonalUp, DiagonalUpLineStyle) && HasBorderLine(applyDiagonalDown, DiagonalDownLineStyle)) {
				info.SetDiagonalUpBorderLine(DiagonalUpLineStyle, DiagonalColor);
				info.SetDiagonalDownBorderLine(DiagonalDownLineStyle, DiagonalColor);
			}
			else if (HasBorderLine(applyDiagonalUp, DiagonalUpLineStyle)) 
				info.SetDiagonalUpBorderLine(DiagonalUpLineStyle, DiagonalColor);
			else if (HasBorderLine(applyDiagonalDown, DiagonalDownLineStyle)) 
				info.SetDiagonalDownBorderLine(DiagonalDownLineStyle, DiagonalColor);
		}
		bool HasSpecialCase(XlBorderLineStyle? style) {
			return style == SpecialBorderLineStyle.InsideComplexBorder || style == SpecialBorderLineStyle.OutsideComplexBorder;
		}
		void SetBorderLine(bool? applyBorder, XlBorderLineStyle? style, Color? color, Action<XlBorderLineStyle?, Color?> setter) {
			if (HasBorderLine(applyBorder, style))
				setter(style, color);
		}
		bool HasBorderLine(bool? applyBorder, XlBorderLineStyle? style) {
			return applyBorder.HasValue && !HasSpecialCase(style);
		}
		#region CopyFromActualFormat
		protected internal void CopyFrom(IActualBorderInfo owner) {
			this.Outline = owner.Outline;
			CopyBorder(owner.LeftLineStyle, owner.LeftColor, owner.LeftColorIndex, SetLeftBorderLine, SetLeftColorIndex);
			CopyBorder(owner.RightLineStyle, owner.RightColor, owner.RightColorIndex, SetRightBorderLine, SetRightColorIndex);
			CopyBorder(owner.TopLineStyle, owner.TopColor, owner.TopColorIndex, SetTopBorderLine, SetTopColorIndex);
			CopyBorder(owner.BottomLineStyle, owner.BottomColor, owner.BottomColorIndex, SetBottomBorderLine, SetBottomColorIndex);
			CopyBorder(owner.HorizontalLineStyle, owner.HorizontalColor, owner.HorizontalColorIndex, SetHorizontalBorderLine, SetHorizontalColorIndex);
			CopyBorder(owner.VerticalLineStyle, owner.VerticalColor, owner.VerticalColorIndex, SetVerticalBorderLine, SetVerticalColorIndex);
			CopyBorder(owner.DiagonalUpLineStyle, owner.DiagonalColor, owner.DiagonalColorIndex, SetDiagonalUpBorderLine, SetDiagonalColorIndex);
			CopyBorder(owner.DiagonalDownLineStyle, owner.DiagonalColor, owner.DiagonalColorIndex, SetDiagonalDownBorderLine, SetDiagonalColorIndex);
		}
		void CopyBorder(XlBorderLineStyle? style, Color? color, int? colorIndex, Action<XlBorderLineStyle?, Color?> setBorderLine, Action<int?> setColorIndex) {
			setBorderLine(style, color);
			setColorIndex(colorIndex);
		}
		#endregion
	}
	#endregion
	#region MergedFillInfo
	public class MergedFillInfo : IFillInfo {
		#region Fields
		MergedGradientFill gradientFill;
		#endregion
		public MergedFillInfo() {
			this.gradientFill = new MergedGradientFill();
		}
		public MergedFillInfo(IActualFillInfo fill)
			: this() {
			CopyFrom(fill);
		}
		#region Properties
		public XlPatternType? PatternType { get; set; }
		public Color? ForeColor { get; set; }
		public Color? BackColor { get; set; }
		public ModelFillType? FillType { get; set; }
		public MergedGradientFill GradientFill { get { return gradientFill; } }
		XlPatternType IFillInfo.PatternType { get { return PatternType.Value; } set { PatternType = value; } }
		Color IFillInfo.ForeColor { get { return ForeColor.Value; } set { ForeColor = value; } }
		Color IFillInfo.BackColor { get { return BackColor.Value; } set { BackColor = value; } }
		ModelFillType IFillInfo.FillType { get { return FillType.Value; } set { FillType = value; } }
		IGradientFillInfo IFillInfo.GradientFill { get { return GradientFill; } }
		#endregion
		void IFillInfo.Clear() {
			PatternType = null;
			ForeColor = null;
			BackColor = null;
			FillType = null;
			gradientFill = new MergedGradientFill();
		}
		protected internal void CopyFrom(IActualFillInfo owner) {
			this.gradientFill.CopyFrom(owner.GradientFill);
			this.PatternType = owner.PatternType;
			this.ForeColor = owner.ForeColor;
			this.BackColor = owner.BackColor;
			this.FillType = owner.FillType;
		}
	}
	#endregion
	#region MergedGradientFill
	public class MergedGradientFill : IGradientFillInfo {
		#region Fields
		MergedConvergence convergence;
		#endregion
		public MergedGradientFill() {
			this.convergence = new MergedConvergence();
		}
		public MergedGradientFill(IActualGradientFillInfo gradientFill)
			: this() {
			CopyFrom(gradientFill);
		}
		#region Properties
		public ModelGradientFillType? Type { get; set; }
		public double? Degree { get; set; }
		public MergedConvergence Convergence { get { return convergence; } }
		ModelGradientFillType IGradientFillInfo.Type { get { return Type.Value; } set { Type = value; } }
		double IGradientFillInfo.Degree { get { return Degree.Value; } set { Degree = value; } }
		IGradientStopCollection IGradientFillInfo.GradientStops { get { return null; } }
		IConvergenceInfo IGradientFillInfo.Convergence { get { return Convergence; } }
		#endregion
		protected internal void CopyFrom(IActualGradientFillInfo gradientFill) {
			this.convergence.CopyFrom(gradientFill.Convergence);
			this.Type = gradientFill.Type;
			this.Degree = gradientFill.Degree;
		}
	}
	#endregion
	#region MergedConvergence
	public class MergedConvergence : IConvergenceInfo {
		public MergedConvergence() {
		}
		public MergedConvergence(IActualConvergenceInfo convergence) {
			Guard.ArgumentNotNull(convergence, "convergence");
			CopyFrom(convergence);
		}
		#region Properties
		public float? Left { get; set; }
		public float? Right { get; set; }
		public float? Top { get; set; }
		public float? Bottom { get; set; }
		float IConvergenceInfo.Left { get { return Left.Value; } set { Left = value; } }
		float IConvergenceInfo.Right { get { return Right.Value; } set { Right = value; } }
		float IConvergenceInfo.Top { get { return Top.Value; } set { Top = value; } }
		float IConvergenceInfo.Bottom { get { return Bottom.Value; } set { Bottom = value; } }
		#endregion
		protected internal void CopyFrom(IActualConvergenceInfo owner) {
			this.Left = owner.Left;
			this.Right = owner.Right;
			this.Top = owner.Top;
			this.Bottom = owner.Bottom;
		}
	}
	#endregion
	#region MergedCellProtectionInfo
	public class MergedCellProtectionInfo : ICellProtectionInfo {
		public MergedCellProtectionInfo() {
		}
		public MergedCellProtectionInfo(IActualCellProtectionInfo protection) {
			CopyFrom(protection);
		}
		#region Properties
		public bool? Locked { get; set; }
		public bool? Hidden { get; set; }
		bool ICellProtectionInfo.Locked { get { return Locked.Value; } set { Locked = value; } }
		bool ICellProtectionInfo.Hidden { get { return Hidden.Value; } set { Hidden = value; } }
		#endregion
		protected internal void CopyFrom(IActualCellProtectionInfo owner) {
			this.Locked = owner.Locked;
			this.Hidden = owner.Hidden;
		}
	}
	#endregion
}
