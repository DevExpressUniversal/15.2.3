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

using DevExpress.Office;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Drawing;
using DevExpress.Utils;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.Export.Xl {
	#region XlFontSchemeStyles
	public enum XlFontSchemeStyles {
		None,
		Minor,
		Major
	}
	#endregion
	#region XlUnderlineType
	public enum XlUnderlineType {
		None = 0x00,
		Single = 0x01,
		Double = 0x02,
		SingleAccounting = 0x21,
		DoubleAccounting = 0x22
	}
	#endregion
	#region XlScriptType
	public enum XlScriptType {
		Baseline = 0,
		Superscript = 1,
		Subscript = 2
	}
	#endregion
	#region XlFontBase
	public abstract class XlFontBase {
		#region Fields
		const uint MaskBold = 0x00000001;
		const uint MaskCondense = 0x00000002;
		const uint MaskExtend = 0x00000004;
		const uint MaskItalic = 0x00000008;
		const uint MaskOutline = 0x00000010;
		const uint MaskShadow = 0x00000020;
		const uint MaskStrikeThrough = 0x00000040;
		const uint MaskUnderline = 0x00001F80; 
		const uint MaskScript = 0x00006000; 
		const uint MaskSchemeStyle = 0x00018000; 
		uint packedValues;
		int charset;
		string name;
		double size;
		#endregion
		#region Properties
		public bool Bold { get { return GetBooleanValue(MaskBold); } set { SetBooleanValue(MaskBold, value); } }
		public bool Condense { get { return GetBooleanValue(MaskCondense); } set { SetBooleanValue(MaskCondense, value); } }
		public bool Extend { get { return GetBooleanValue(MaskExtend); } set { SetBooleanValue(MaskExtend, value); } }
		public bool Italic { get { return GetBooleanValue(MaskItalic); } set { SetBooleanValue(MaskItalic, value); } }
		public bool Outline { get { return GetBooleanValue(MaskOutline); } set { SetBooleanValue(MaskOutline, value); } }
		public bool Shadow { get { return GetBooleanValue(MaskShadow); } set { SetBooleanValue(MaskShadow, value); } }
		public bool StrikeThrough { get { return GetBooleanValue(MaskStrikeThrough); } set { SetBooleanValue(MaskStrikeThrough, value); } }
		public int Charset { get { return charset; } set { charset = value; } }
		public string Name { get { return name; } set { name = value; } }
		public double Size { get { return size; } set { size = value; } }
		public XlFontSchemeStyles SchemeStyle {
			get { return (XlFontSchemeStyles)((packedValues & MaskSchemeStyle) >> 15); }
			set {
				packedValues &= ~MaskSchemeStyle;
				packedValues |= ((uint)value << 15) & MaskSchemeStyle;
			}
		}
		public XlScriptType Script {
			get { return (XlScriptType)((packedValues & MaskScript) >> 13); }
			set {
				packedValues &= ~MaskScript;
				packedValues |= ((uint)value << 13) & MaskScript;
			}
		}
		public XlUnderlineType Underline {
			get { return (XlUnderlineType)((packedValues & MaskUnderline) >> 7); }
			set {
				packedValues &= ~MaskUnderline;
				packedValues |= ((uint)value << 7) & MaskUnderline;
			}
		}
		#endregion
		public virtual void CopyFrom(XlFontBase value) {
			this.packedValues = value.packedValues;
			this.Charset = value.Charset;
			this.Name = value.Name;
			this.Size = value.Size;
		}
		public override bool Equals(object obj) {
			XlFontBase other = obj as XlFontBase;
			if(other == null)
				return false;
			return
				this.packedValues == other.packedValues &&
				this.Charset == other.Charset &&
				this.Name == other.Name &&
				this.Size == other.Size;
		}
		public override int GetHashCode() {
			return packedValues.GetHashCode() ^ charset ^ size.GetHashCode() ^ (name == null ? 0 : name.GetHashCode());
		}
		#region GetBooleanValue/SetBooleanValue helpers
		void SetBooleanValue(uint mask, bool bitVal) {
			if (bitVal)
				packedValues |= mask;
			else
				packedValues &= ~mask;
		}
		bool GetBooleanValue(uint mask) {
			return (packedValues & mask) != 0;
		}
		#endregion
	}
	#endregion
	#region XlFontFamily
	public enum XlFontFamily {
		Auto = 0,
		Roman = 1,
		Swiss = 2,
		Modern = 3,
		Script = 4,
		Decorative = 5
	}
	#endregion
	#region XlFont
	public class XlFont : XlFontBase, ISupportsCopyFrom<XlFont> {
		XlColor color = XlColor.FromTheme(XlThemeColor.Dark1, 0.0);
		public XlFontFamily FontFamily { get; set; }
		public XlColor Color {
			get { return color; }
			set { color = value ?? XlColor.Empty; }
		}
		public XlFont() {
			Name = "Calibri";
			Size = 11;
			FontFamily = XlFontFamily.Swiss;
			SchemeStyle = XlFontSchemeStyles.Minor;
		}
		public void CopyFrom(XlFont value) {
			Guard.ArgumentNotNull(value, "value");
			base.CopyFrom(value);
			this.FontFamily = value.FontFamily;
			this.Color = value.Color;
		}
		public override int GetHashCode() {
			return base.GetHashCode() ^ FontFamily.GetHashCode() ^ Color.GetHashCode();
		}
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			XlFont other = obj as XlFont;
			if (other == null)
				return false;
			return FontFamily == other.FontFamily && color.Equals(other.Color);
		}
		public XlFont Clone() {
			XlFont result = new XlFont();
			result.CopyFrom(this);
			return result;
		}
		public static XlFont BodyFont() {
			XlFont result = new XlFont();
			result.Name = "Calibri";
			result.Size = 11;
			result.FontFamily = XlFontFamily.Swiss;
			result.SchemeStyle = XlFontSchemeStyles.Minor;
			result.Color = XlColor.FromTheme(XlThemeColor.Dark1, 0.0);
			return result;
		}
		public static XlFont HeadingsFont() {
			XlFont result = new XlFont();
			result.Name = "Cambria";
			result.Size = 11;
			result.FontFamily = XlFontFamily.Roman;
			result.SchemeStyle = XlFontSchemeStyles.Major;
			result.Color = XlColor.FromTheme(XlThemeColor.Dark1, 0.0);
			return result;
		}
		public static XlFont CustomFont(string fontName) {
			return CustomFont(fontName, 11, XlColor.FromTheme(XlThemeColor.Dark1, 0.0));
		}
		public static XlFont CustomFont(string fontName, double size) {
			return CustomFont(fontName, size, XlColor.FromTheme(XlThemeColor.Dark1, 0.0));
		}
		public static XlFont CustomFont(string fontName, double size, XlColor color) {
			XlFont result = new XlFont();
			result.Name = fontName;
			result.Size = size;
			result.Color = color;
			result.SchemeStyle = XlFontSchemeStyles.None;
			return result;
		}
	}
	#endregion
	#region XlPatternType
	public enum XlPatternType {
		None = 0,
		Solid = 1,
		MediumGray = 2,
		DarkGray = 3,
		LightGray = 4,
		DarkHorizontal = 5,
		DarkVertical = 6,
		DarkDown = 7,
		DarkUp = 8,
		DarkGrid = 9,
		DarkTrellis = 10,
		LightHorizontal = 11,
		LightVertical = 12,
		LightDown = 13,
		LightUp = 14,
		LightGrid = 15,
		LightTrellis = 16,
		Gray125 = 17,
		Gray0625 = 18
	}
	#endregion
	#region XlFill
	public class XlFill : ISupportsCopyFrom<XlFill> {
		XlColor foreColor = XlColor.Empty;
		XlColor backColor = XlColor.Empty;
		#region Properties
		public XlPatternType PatternType { get; set; }
		public XlColor ForeColor {
			get { return foreColor; }
			set { foreColor = value ?? XlColor.Empty; }
		}
		public XlColor BackColor {
			get { return backColor; }
			set { backColor = value ?? XlColor.Empty; }
		}
		#endregion
		public override bool Equals(object obj) {
			XlFill fill = obj as XlFill;
			if (fill == null)
				return false;
			return
				PatternType == fill.PatternType &&
				ForeColor.Equals(fill.ForeColor) &&
				BackColor.Equals(fill.BackColor);
		}
		public override int GetHashCode() {
			return (int)PatternType ^ ForeColor.GetHashCode() ^ BackColor.GetHashCode();
		}
		public XlFill Clone() {
			XlFill result = new XlFill();
			result.CopyFrom(this);
			return result;
		}
		#region ISupportsCopyFrom<XlFill> Members
		public void CopyFrom(XlFill value) {
			Guard.ArgumentNotNull(value, "value");
			PatternType = value.PatternType;
			ForeColor = value.ForeColor;
			BackColor = value.BackColor;
		}
		#endregion
		public static XlFill NoFill() {
			return new XlFill();
		}
		public static XlFill SolidFill(XlColor color) {
			XlFill result = new XlFill();
			result.PatternType = XlPatternType.Solid;
			result.ForeColor = color;
			return result;
		}
		public static XlFill PatternFill(XlPatternType patternType) {
			XlFill result = new XlFill();
			result.PatternType = patternType;
			return result;
		}
		public static XlFill PatternFill(XlPatternType patternType, XlColor backColor, XlColor patternColor) {
			XlFill result = new XlFill();
			result.PatternType = patternType;
			result.ForeColor = patternColor;
			result.BackColor = backColor;
			return result;
		}
	}
	#endregion
	#region XlHorizontalAlignment
	public enum XlHorizontalAlignment {
		General = 0,
		Left = 1,
		Center = 2,
		Right = 3,
		Fill = 4,
		Justify = 5,
		CenterContinuous = 6,
		Distributed = 7
	}
	#endregion
	#region XlVerticalAlignment
	public enum XlVerticalAlignment {
		Top = 0,
		Center = 1,
		Bottom = 2,
		Justify = 3,
		Distributed = 4
	}
	#endregion
	#region XlReadingOrder
	public enum XlReadingOrder {
		Context = 0,
		LeftToRight = 1,
		RightToLeft = 2
	}
	#endregion
	#region XlCellAlignment
	public class XlCellAlignment : ISupportsCopyFrom<XlCellAlignment> {
		#region Fields
		const uint MaskHorizontalAlignment = 0x0000007; 
		const uint MaskVerticalAlignment = 0x00000038; 
		const uint MaskWrapText = 0x0000040;
		const uint MaskJustifyLastLine = 0x0000080;
		const uint MaskShrinkToFit = 0x0000100;
		const uint MaskReadingOrder = 0x00000600; 
		uint packedValues = 0x00000010;
		int textRotation;
		byte indent;
		int relativeIndent;
		#endregion
		#region Properties
		public bool WrapText { 
			get { return GetBooleanValue(MaskWrapText); } 
			set { SetBooleanValue(MaskWrapText, value); } 
		}
		public bool JustifyLastLine { 
			get { return GetBooleanValue(MaskJustifyLastLine); } 
			set { SetBooleanValue(MaskJustifyLastLine, value); } 
		}
		public bool ShrinkToFit { 
			get { return GetBooleanValue(MaskShrinkToFit); } 
			set { SetBooleanValue(MaskShrinkToFit, value); } 
		}
		public int TextRotation { 
			get { return textRotation; } 
			set {
				CheckTextRotation(value);
				textRotation = value; 
			} 
		}
		public byte Indent { 
			get { return indent; } 
			set {
				if(value > 250)
					throw new ArgumentOutOfRangeException("Indent out of range 0...250!");
				indent = value; 
			} 
		}
		public int RelativeIndent { 
			get { return relativeIndent; } 
			set {
				CheckRelativeIndent(value);
				relativeIndent = value; 
			} 
		}
		public XlHorizontalAlignment HorizontalAlignment {
			get { return (XlHorizontalAlignment)(packedValues & MaskHorizontalAlignment); }
			set {
				packedValues &= ~MaskHorizontalAlignment;
				packedValues |= (uint)value & MaskHorizontalAlignment;
			}
		}
		public XlVerticalAlignment VerticalAlignment {
			get { return (XlVerticalAlignment)((packedValues & MaskVerticalAlignment) >> 3); }
			set {
				packedValues &= ~MaskVerticalAlignment;
				packedValues |= ((uint)value << 3) & MaskVerticalAlignment;
			}
		}
		public XlReadingOrder ReadingOrder {
			get { return (XlReadingOrder)((packedValues & MaskReadingOrder) >> 9); }
			set {
				packedValues &= ~MaskReadingOrder;
				packedValues |= ((uint)value << 9) & MaskReadingOrder;
			}
		}
		#endregion
		public XlCellAlignment Clone() {
			XlCellAlignment result = new XlCellAlignment();
			result.CopyFrom(this);
			return result;
		}
		public void CopyFrom(XlCellAlignment value) {
			Guard.ArgumentNotNull(value, "value");
			this.packedValues = value.packedValues;
			this.textRotation = value.textRotation;
			this.indent = value.indent;
			this.relativeIndent = value.relativeIndent;
		}
		public override bool Equals(object obj) {
			XlCellAlignment other = obj as XlCellAlignment;
			if(other == null)
				return false;
			return
				this.packedValues == other.packedValues &&
				this.textRotation == other.textRotation &&
				this.indent == other.indent &&
				this.relativeIndent == other.relativeIndent;
		}
		public override int GetHashCode() {
			return packedValues.GetHashCode() ^ textRotation ^ indent ^ relativeIndent;
		}
		#region GetBooleanValue/SetBooleanValue helpers
		void SetBooleanValue(uint mask, bool bitVal) {
			if (bitVal)
				packedValues |= mask;
			else
				packedValues &= ~mask;
		}
		bool GetBooleanValue(uint mask) {
			return (packedValues & mask) != 0;
		}
		#endregion
		protected virtual void CheckTextRotation(int value) {
			if((value < 0 || value > 180) && (value != 255))
				throw new ArgumentException("Text rotation must be 0...180 degrees or 255 for vertical text.");
		}
		protected virtual void CheckRelativeIndent(int value) {
			if((value < -15 || value > 15) && (value != 255))
				throw new ArgumentException("Relative indent must be from -15 to 15 or 255.");
		}
		protected virtual void CheckIndent(byte value) {
			if(value > 250)
				throw new ArgumentOutOfRangeException("Indent out of range 0...250!");
		}
		public static XlCellAlignment FromHV(XlHorizontalAlignment horizontalAlignment, XlVerticalAlignment verticalAlignment) {
			XlCellAlignment result = new XlCellAlignment();
			result.HorizontalAlignment = horizontalAlignment;
			result.VerticalAlignment = verticalAlignment;
			return result;
		}
	}
	#endregion
	#region XlBorderLineStyle
	public enum XlBorderLineStyle {
		None = 0,
		Thin = 1,
		Medium = 2,
		Dashed = 3,
		Dotted = 4,
		Thick = 5,
		Double = 6,
		Hair = 7,
		MediumDashed = 8,
		DashDot = 9,
		MediumDashDot = 10,
		DashDotDot = 11,
		MediumDashDotDot = 12,
		SlantDashDot = 13,
	}
	#endregion
	#region XlBorderBase
	public abstract class XlBordersBase {
		#region Fields
		const uint MaskLeftLineStyle = 0x0000000F;
		const uint MaskRightLineStyle = 0x000000F0;
		const uint MaskTopLineStyle = 0x00000F00;
		const uint MaskBottomLineStyle = 0x0000F000;
		const uint MaskDiagonalLineStyle = 0x000F0000;
		const uint MaskHorizontalLineStyle = 0x00F00000;
		const uint MaskVerticalLineStyle = 0x0F000000;
		const uint MaskDiagonalUp = 0x10000000;
		const uint MaskDiagonalDown = 0x20000000;
		const uint MaskOutline = 0x40000000;
		uint packedValues = 0x40000000;
		#endregion
		#region Properties
		public XlBorderLineStyle LeftLineStyle { get { return GetBorderLineStyleValue(MaskLeftLineStyle, 0); } set { SetBorderLineStyleValue(MaskLeftLineStyle, 0, value); } }
		public XlBorderLineStyle RightLineStyle { get { return GetBorderLineStyleValue(MaskRightLineStyle, 4); } set { SetBorderLineStyleValue(MaskRightLineStyle, 4, value); } }
		public XlBorderLineStyle TopLineStyle { get { return GetBorderLineStyleValue(MaskTopLineStyle, 8); } set { SetBorderLineStyleValue(MaskTopLineStyle, 8, value); } }
		public XlBorderLineStyle BottomLineStyle { get { return GetBorderLineStyleValue(MaskBottomLineStyle, 12); } set { SetBorderLineStyleValue(MaskBottomLineStyle, 12, value); } }
		public XlBorderLineStyle DiagonalLineStyle { get { return GetBorderLineStyleValue(MaskDiagonalLineStyle, 16); } set { SetBorderLineStyleValue(MaskDiagonalLineStyle, 16, value); } }
		public XlBorderLineStyle HorizontalLineStyle { get { return GetBorderLineStyleValue(MaskHorizontalLineStyle, 20); } set { SetBorderLineStyleValue(MaskHorizontalLineStyle, 20, value); } }
		public XlBorderLineStyle VerticalLineStyle { get { return GetBorderLineStyleValue(MaskVerticalLineStyle, 24); } set { SetBorderLineStyleValue(MaskVerticalLineStyle, 24, value); } }
		public XlBorderLineStyle DiagonalUpLineStyle { get { return GetBorderDiagonalUpLineStyle(); } set { SetBorderDiagonalUpLineStyle(value); } }
		public XlBorderLineStyle DiagonalDownLineStyle { get { return GetBorderDiagonalDownLineStyle(); } set { SetBorderDiagonalDownLineStyle(value); } }
		public bool DiagonalUp { get { return GetBooleanValue(MaskDiagonalUp); } set { SetBooleanValue(MaskDiagonalUp, value); } }
		public bool DiagonalDown { get { return GetBooleanValue(MaskDiagonalDown); } set { SetBooleanValue(MaskDiagonalDown, value); } }
		public bool Outline { get { return GetBooleanValue(MaskOutline); } set { SetBooleanValue(MaskOutline, value); } }
		#endregion
		#region GetBooleanValue/SetBooleanValue helpers
		void SetBooleanValue(uint mask, bool bitVal) {
			if (bitVal)
				packedValues |= mask;
			else
				packedValues &= ~mask;
		}
		bool GetBooleanValue(uint mask) {
			return (packedValues & mask) != 0;
		}
		#endregion
		#region GetBorderLineStyleValue/SetBorderLineStyleValue helpers
		void SetBorderLineStyleValue(uint mask, int bits, XlBorderLineStyle value) {
			packedValues &= ~mask;
			packedValues |= ((uint)value << bits) & mask;
		}
		XlBorderLineStyle GetBorderLineStyleValue(uint mask, int bits) {
			return (XlBorderLineStyle)((packedValues & mask) >> bits);
		}
		#endregion
		XlBorderLineStyle GetBorderDiagonalUpLineStyle() {
			return DiagonalUp ? DiagonalLineStyle : XlBorderLineStyle.None;
		}
		XlBorderLineStyle GetBorderDiagonalDownLineStyle() {
			return DiagonalDown ? DiagonalLineStyle : XlBorderLineStyle.None;
		}
		void SetBorderDiagonalUpLineStyle(XlBorderLineStyle lineStyle) {
			DiagonalUp = lineStyle != XlBorderLineStyle.None;
			DiagonalLineStyle = lineStyle;
		}
		void SetBorderDiagonalDownLineStyle(XlBorderLineStyle lineStyle) {
			DiagonalDown = lineStyle != XlBorderLineStyle.None;
			DiagonalLineStyle = lineStyle;
		}
		public override bool Equals(object obj) {
			XlBordersBase border = obj as XlBordersBase;
			if (border == null)
				return false;
			return this.packedValues == border.packedValues;
		}
		public override int GetHashCode() {
			return (int)packedValues;
		}
		public void CopyFrom(XlBordersBase value) {
			this.packedValues = value.packedValues;
		}
		protected bool HasBorderLines { get { return (packedValues & 0x0FFFFFFF) != 0; } }
	}
	#endregion
	#region XlBorder
	public class XlBorder : XlBordersBase, ISupportsCopyFrom<XlBorder> {
		#region Fields
		XlColor leftColor = XlColor.Empty;
		XlColor rightColor = XlColor.Empty;
		XlColor topColor = XlColor.Empty;
		XlColor bottomColor = XlColor.Empty;
		XlColor diagonalColor = XlColor.Empty;
		XlColor horizontalColor = XlColor.Empty;
		XlColor verticalColor = XlColor.Empty;
		#endregion
		#region Properties
		public XlColor LeftColor {
			get { return leftColor; }
			set { leftColor = value ?? XlColor.Empty; }
		}
		public XlColor RightColor { 
			get { return rightColor; }
			set { rightColor = value ?? XlColor.Empty; }
		}
		public XlColor TopColor {
			get { return topColor; }
			set { topColor = value ?? XlColor.Empty; }
		}
		public XlColor BottomColor {
			get { return bottomColor; }
			set { bottomColor = value ?? XlColor.Empty; }
		}
		public XlColor DiagonalColor {
			get { return diagonalColor; }
			set { diagonalColor = value ?? XlColor.Empty; }
		}
		public XlColor HorizontalColor {
			get { return horizontalColor; }
			set { horizontalColor = value ?? XlColor.Empty; }
		}
		public XlColor VerticalColor {
			get { return verticalColor; }
			set { verticalColor = value ?? XlColor.Empty; }
		}
		#endregion
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			XlBorder border = obj as XlBorder;
			if (border == null)
				return false;
			if(HasBorderLines) {
				if(LeftLineStyle != XlBorderLineStyle.None && !LeftColor.Equals(border.LeftColor))
					return false;
				if(RightLineStyle != XlBorderLineStyle.None && !RightColor.Equals(border.RightColor))
					return false;
				if(TopLineStyle != XlBorderLineStyle.None && !TopColor.Equals(border.TopColor))
					return false;
				if(BottomLineStyle != XlBorderLineStyle.None && !BottomColor.Equals(border.BottomColor))
					return false;
				if(DiagonalLineStyle != XlBorderLineStyle.None && !DiagonalColor.Equals(border.DiagonalColor))
					return false;
				if(HorizontalLineStyle != XlBorderLineStyle.None && !HorizontalColor.Equals(border.HorizontalColor))
					return false;
				if(VerticalLineStyle != XlBorderLineStyle.None && !VerticalColor.Equals(border.VerticalColor))
					return false;
			}
			return true;
		}
		public override int GetHashCode() {
			int result = base.GetHashCode();
			if(HasBorderLines) {
				if(LeftLineStyle != XlBorderLineStyle.None)
					result = result ^ LeftColor.GetHashCode();
				if(RightLineStyle != XlBorderLineStyle.None)
					result = result ^ RightColor.GetHashCode();
				if(TopLineStyle != XlBorderLineStyle.None)
					result = result ^ TopColor.GetHashCode();
				if(BottomLineStyle != XlBorderLineStyle.None)
					result = result ^ BottomColor.GetHashCode();
				if(DiagonalLineStyle != XlBorderLineStyle.None)
					result = result ^ DiagonalColor.GetHashCode();
				if(HorizontalLineStyle != XlBorderLineStyle.None)
					result = result ^ HorizontalColor.GetHashCode();
				if(VerticalLineStyle != XlBorderLineStyle.None)
					result = result ^ VerticalColor.GetHashCode();
			}
			return result;
		}
		public XlBorder Clone() {
			XlBorder result = new XlBorder();
			result.CopyFrom(this);
			return result;
		}
		public void CopyFrom(XlBorder other) {
			Guard.ArgumentNotNull(other, "other");
			base.CopyFrom(other);
			LeftColor = other.LeftColor;
			RightColor = other.RightColor;
			TopColor = other.TopColor;
			BottomColor = other.BottomColor;
			DiagonalColor = other.DiagonalColor;
			HorizontalColor = other.HorizontalColor;
			VerticalColor = other.VerticalColor;
		}
		public static XlBorder NoBorders() {
			return new XlBorder();
		}
		public static XlBorder OutlineBorders(XlColor color) {
			return OutlineBorders(color, XlBorderLineStyle.Thin);
		}
		public static XlBorder OutlineBorders(XlColor color, XlBorderLineStyle lineStyle) {
			XlBorder result = new XlBorder();
			result.TopColor = color;
			result.BottomColor = color;
			result.LeftColor = color;
			result.RightColor = color;
			result.TopLineStyle = lineStyle;
			result.BottomLineStyle = lineStyle;
			result.LeftLineStyle = lineStyle;
			result.RightLineStyle = lineStyle;
			return result;
		}
		public static XlBorder InsideBorders(XlColor color) {
			return InsideBorders(color, XlBorderLineStyle.Thin);
		}
		public static XlBorder InsideBorders(XlColor color, XlBorderLineStyle lineStyle) {
			XlBorder result = new XlBorder();
			result.HorizontalColor = color;
			result.VerticalColor = color;
			result.HorizontalLineStyle = lineStyle;
			result.VerticalLineStyle = lineStyle;
			return result;
		}
		public static XlBorder AllBorders(XlColor color) {
			return AllBorders(color, XlBorderLineStyle.Thin);
		}
		public static XlBorder AllBorders(XlColor color, XlBorderLineStyle lineStyle) {
			XlBorder result = OutlineBorders(color, lineStyle);
			result.HorizontalColor = color;
			result.VerticalColor = color;
			result.HorizontalLineStyle = lineStyle;
			result.VerticalLineStyle = lineStyle;
			return result;
		}
	}
	#endregion
	#region XlNumberFormat
	public class XlNumberFormat {
		#region Statics
		static readonly XlNumberFormat general = new XlNumberFormat(0, string.Empty);
		static readonly XlNumberFormat number = new XlNumberFormat(1, "0");
		static readonly XlNumberFormat number2 = new XlNumberFormat(2, "0.00");
		static readonly XlNumberFormat numberWithSeparator = new XlNumberFormat(3, "#,##0");
		static readonly XlNumberFormat numberWithSeparator2 = new XlNumberFormat(4, "#,##0.00");
		static readonly XlNumberFormat percentage = new XlNumberFormat(9, "0%");
		static readonly XlNumberFormat percentage2 = new XlNumberFormat(10, "0.00%");
		static readonly XlNumberFormat scientific = new XlNumberFormat(11, "0.00E+00");
		static readonly XlNumberFormat fraction = new XlNumberFormat(12, "# ?/?");
		static readonly XlNumberFormat fraction2 = new XlNumberFormat(13, "# ??/??");
		static readonly XlNumberFormat shortDate = new XlNumberFormat(14, "mm-dd-yy");
		static readonly XlNumberFormat longDate = new XlNumberFormat(15, "d-mmm-yy");
		static readonly XlNumberFormat dayMonth = new XlNumberFormat(16, "d-mmm");
		static readonly XlNumberFormat monthYear = new XlNumberFormat(17, "mmm-yy");
		static readonly XlNumberFormat shortTime12 = new XlNumberFormat(18, "h:mm AM/PM");
		static readonly XlNumberFormat longTime12 = new XlNumberFormat(19, "h:mm:ss AM/PM");
		static readonly XlNumberFormat shortTime24 = new XlNumberFormat(20, "h:mm");
		static readonly XlNumberFormat longTime24 = new XlNumberFormat(21, "h:mm:ss");
		static readonly XlNumberFormat shortDateTime = new XlNumberFormat(22, "m/d/yy h:mm");
		static readonly XlNumberFormat negativeParentheses = new XlNumberFormat(37, "#,##0;(#,##0)");
		static readonly XlNumberFormat negativeParenthesesRed = new XlNumberFormat(38, "#,##0;[Red](#,##0)");
		static readonly XlNumberFormat negativeParentheses2 = new XlNumberFormat(39, "#,##0.00;(#,##0.00)");
		static readonly XlNumberFormat negativeParenthesesRed2 = new XlNumberFormat(40, "#,##0.00;[Red](#,##0.00)");
		static readonly XlNumberFormat minuteSeconds = new XlNumberFormat(45, "mm:ss");
		static readonly XlNumberFormat timeSpan = new XlNumberFormat(46, "[h]:mm:ss");
		static readonly XlNumberFormat minuteSecondsMs = new XlNumberFormat(47, "mm:ss.0");
		static readonly XlNumberFormat scientific1 = new XlNumberFormat(48, "##0.0E+0");
		static readonly XlNumberFormat text = new XlNumberFormat(49, "@");
		static readonly Dictionary<string, XlNumberFormat> predefinedFormats = CreatePredefinedFormats();
		static Dictionary<string, XlNumberFormat> CreatePredefinedFormats() {
			Dictionary<string, XlNumberFormat> result = new Dictionary<string, XlNumberFormat>();
			result.Add("0", number);
			result.Add("0.00", number2);
			result.Add("#,##0", numberWithSeparator);
			result.Add("#,##0.00", numberWithSeparator2);
			result.Add("0%", percentage);
			result.Add("0.00%", percentage2);
			result.Add("0.00E+00", scientific);
			result.Add("# ?/?", fraction);
			result.Add("# ??/??", fraction2);
			result.Add("mm-dd-yy", shortDate);
			result.Add("d-mmm-yy", longDate);
			result.Add("d-mmm", dayMonth);
			result.Add("mmm-yy", monthYear);
			result.Add("h:mm AM/PM", shortTime12);
			result.Add("h:mm:ss AM/PM", longTime12);
			result.Add("h:mm", shortTime24);
			result.Add("h:mm:ss", longTime24);
			result.Add("m/d/yy h:mm", shortDateTime);
			result.Add("#,##0;(#,##0)", negativeParentheses);
			result.Add("#,##0;[Red](#,##0)", negativeParenthesesRed);
			result.Add("#,##0.00;(#,##0.00)", negativeParentheses2);
			result.Add("#,##0.00;[Red](#,##0.00)", negativeParenthesesRed2);
			result.Add("mm:ss", minuteSeconds);
			result.Add("[h]:mm:ss", timeSpan);
			result.Add("mm:ss.0", minuteSecondsMs);
			result.Add("##0.0E+0", scientific1);
			result.Add("@", text);
			return result;
		}
		static readonly Dictionary<int, XlNumberFormat> predefinedFormatIds = CreatePredefinedFormatIds();
		static Dictionary<int, XlNumberFormat> CreatePredefinedFormatIds() {
			Dictionary<int, XlNumberFormat> result = new Dictionary<int, XlNumberFormat>();
			result.Add(0, general);
			result.Add(1, number);
			result.Add(2, number2);
			result.Add(3, numberWithSeparator);
			result.Add(4, numberWithSeparator2);
			result.Add(9, percentage);
			result.Add(10, percentage2);
			result.Add(11, scientific);
			result.Add(12, fraction);
			result.Add(13, fraction2);
			result.Add(14, shortDate);
			result.Add(15, longDate);
			result.Add(16, dayMonth);
			result.Add(17, monthYear);
			result.Add(18, shortTime12);
			result.Add(19, longTime12);
			result.Add(20, shortTime24);
			result.Add(21, longTime24);
			result.Add(22, shortDateTime);
			result.Add(37, negativeParentheses);
			result.Add(38, negativeParenthesesRed);
			result.Add(39, negativeParentheses2);
			result.Add(40, negativeParenthesesRed2);
			result.Add(45, minuteSeconds);
			result.Add(46, timeSpan);
			result.Add(47, minuteSecondsMs);
			result.Add(48, scientific1);
			result.Add(49, text);
			return result;
		}
		#endregion
		#region Properties
		public int FormatId { get; private set; }
		public string FormatCode { get; private set; }
		public bool IsDateTime { get; private set; }
		public static XlNumberFormat General { get { return general; } }
		public static XlNumberFormat Number { get { return number; } }
		public static XlNumberFormat Number2 { get { return number2; } }
		public static XlNumberFormat NumberWithThousandSeparator { get { return numberWithSeparator; } }
		public static XlNumberFormat NumberWithThousandSeparator2 { get { return numberWithSeparator2; } }
		public static XlNumberFormat Percentage { get { return percentage; } }
		public static XlNumberFormat Percentage2 { get { return percentage2; } }
		public static XlNumberFormat Scientific { get { return scientific; } }
		public static XlNumberFormat Fraction { get { return fraction; } }
		public static XlNumberFormat Fraction2 { get { return fraction2; } }
		public static XlNumberFormat ShortDate { get { return shortDate; } }
		public static XlNumberFormat LongDate { get { return longDate; } }
		public static XlNumberFormat DayMonth { get { return dayMonth; } }
		public static XlNumberFormat MonthYear { get { return monthYear; } }
		public static XlNumberFormat ShortTime12 { get { return shortTime12; } }
		public static XlNumberFormat LongTime12 { get { return longTime12; } }
		public static XlNumberFormat ShortTime24 { get { return shortTime24; } }
		public static XlNumberFormat LongTime24 { get { return longTime24; } }
		public static XlNumberFormat ShortDateTime { get { return shortDateTime; } }
		public static XlNumberFormat NegativeParentheses { get { return negativeParentheses; } }
		public static XlNumberFormat NegativeParenthesesRed { get { return negativeParenthesesRed; } }
		public static XlNumberFormat NegativeParentheses2 { get { return negativeParentheses2; } }
		public static XlNumberFormat NegativeParenthesesRed2 { get { return negativeParenthesesRed2; } }
		public static XlNumberFormat MinuteSeconds { get { return minuteSeconds; } }
		public static XlNumberFormat Span { get { return timeSpan; } }
		public static XlNumberFormat MinuteSecondsMs { get { return minuteSecondsMs; } }
		public static XlNumberFormat Scientific1 { get { return scientific1; } }
		public static XlNumberFormat Text { get { return text; } }
		#endregion
		protected XlNumberFormat(int formatId, string formatCode) {
			FormatId = formatId;
			FormatCode = formatCode;
			IsDateTime = IsDateTimeFormat(formatCode);
		}
		#region Implicit conversion
		public static implicit operator XlNumberFormat(string value) {
			if(string.IsNullOrEmpty(value))
				return general;
			XlNumberFormat result;
			if(!predefinedFormats.TryGetValue(value, out result))
				result = new XlNumberFormat(-1, value);
			return result;
		}
		#endregion
		internal static XlNumberFormat FromId(int id) {
			XlNumberFormat result;
			if(!predefinedFormatIds.TryGetValue(id, out result))
				result = null;
			return result;
		}
		internal string GetLocalizedFormatCode(CultureInfo culture) {
			if(string.IsNullOrEmpty(FormatCode))
				return string.Empty;
			StringBuilder sb = new StringBuilder();
			List<string> parts = SplitFormatCode(FormatCode);
			XlExportNumberFormatConverter numberFormatConverter = new XlExportNumberFormatConverter();
			for(int i = 0; i < parts.Count; i++) {
				if(i > 0)
					sb.Append(";");
				string part = parts[i];
				if(IsDateTimeFormat(part))
					sb.Append(numberFormatConverter.GetLocalDateFormatString(part, culture));
				else
					sb.Append(numberFormatConverter.GetLocalFormatString(part, culture));
			}
			return sb.ToString();
		}
		internal static List<string> SplitFormatCode(string formatCode) {
			List<string> result = new List<string>();
			bool hasQuotationMark = false;
			bool hasBackSlash = false;
			StringBuilder sb = new StringBuilder();
			for(int i = 0; i < formatCode.Length; i++) {
				char ch = formatCode[i];
				if(ch == '"') {
					if(hasBackSlash)
						hasBackSlash = false;
					else
						hasQuotationMark = !hasQuotationMark;
				}
				else if(ch == '\\') {
					if(!hasQuotationMark)
						hasBackSlash = !hasBackSlash;
				}
				else if(ch == ';') {
					if(hasBackSlash)
						hasBackSlash = false;
					else if(!hasQuotationMark) {
						result.Add(sb.ToString());
						sb.Clear();
						continue;
					}
				}
				else if(hasBackSlash)
					hasBackSlash = false;
				sb.Append(ch);
			}
			result.Add(sb.ToString());
			return result;
		}
		bool IsDateTimeFormat(string formatString) {
			if(string.IsNullOrEmpty(formatString))
				return false;
			bool quoted = false;
			bool escaped = false;
			bool braced = false;
			bool isDatePart = true;
			bool result = false;
			int count = formatString.Length;
			for(int i = 0; i < count; i++) {
				char currentChar = formatString[i];
				if(quoted) {
					if(currentChar == '\"')
						quoted = false;
				}
				else if(braced) {
					if(currentChar == ']')
						braced = false;
				}
				else {
					if(escaped) {
						escaped = false;
					}
					else if(currentChar == '\\') {
						escaped = true;
					}
					else if(currentChar == '[') {
						if((i + 1) < count && formatString[i + 1] != 'h' && formatString[i + 1] != 'm' && formatString[i + 1] != 's')
							braced = true;
					}
					else if(currentChar == '\"') {
						quoted = true;
					}
					else if(currentChar == '/') {
						if(!((i + 1) < count && formatString[i + 1] == 'P'))
							isDatePart = true;
					}
					else if(currentChar == ':') {
						isDatePart = false;
					}
					else if(currentChar == 'd') {
						result |= true;
						isDatePart = true;
					}
					else if(currentChar == 'y') {
						result |= true;
						isDatePart = true;
					}
					else if(currentChar == 'M') {
						if(!((i > 0 && formatString[i - 1] == 'A') || (i > 0 && formatString[i - 1] == 'P'))) {
							result |= true;
							isDatePart = true;
						}
					}
					else if(currentChar == 'm') {
						if(isDatePart && (((i + 1) < count && formatString[i + 1] == ':') || ((i + 2) < count && formatString[i + 2] == ':')))
							isDatePart = false;
						result |= true;
					}
					else if(currentChar == 'h' || currentChar == 'H') {
						result |= true;
						isDatePart = false;
					}
					else if(currentChar == 's') {
						result |= true;
						isDatePart = false;
					}
					else {
						isDatePart = true;
					}
				}
			}
			return result;
		}
	}
	#endregion
	#region XlFormatting
	public abstract class XlFormatting {
		public XlFont Font { get; set; }
		public XlFill Fill { get; set; }
		public XlCellAlignment Alignment { get; set; }
		public string NetFormatString { get; set; }
		public bool IsDateTimeFormatString { get; set; }
		public XlNumberFormat NumberFormat { get; set; }
		public XlBorder Border { get; set; }
		public static T CopyObject<T>(T other) where T : class, ISupportsCopyFrom<T>, new() {
			if (other == null)
				return default(T);
			T obj = new T();
			obj.CopyFrom(other);
			return obj;
		}
	}
	#endregion
	#region XlCellFormatting
	public class XlCellFormatting : XlFormatting, ISupportsCopyFrom<XlCellFormatting> {
		#region Predefined formatting
		public static XlCellFormatting Bad {
			get {
				XlCellFormatting result = new XlCellFormatting();
				result.Font = new XlFont();
				result.Font.Color = DXColor.FromArgb(255, 156, 0, 6);
				result.Fill = new XlFill();
				result.Fill.PatternType = XlPatternType.Solid;
				result.Fill.ForeColor = DXColor.FromArgb(255, 255, 199, 206);
				return result;
			}
		}
		public static XlCellFormatting Good {
			get {
				XlCellFormatting result = new XlCellFormatting();
				result.Font = new XlFont();
				result.Font.Color = DXColor.FromArgb(255, 0, 97, 0);
				result.Fill = new XlFill();
				result.Fill.PatternType = XlPatternType.Solid;
				result.Fill.ForeColor = DXColor.FromArgb(255, 198, 239, 206);
				return result;
			}
		}
		public static XlCellFormatting Neutral {
			get {
				XlCellFormatting result = new XlCellFormatting();
				result.Font = new XlFont();
				result.Font.Color = DXColor.FromArgb(255, 156, 101, 0);
				result.Fill = new XlFill();
				result.Fill.PatternType = XlPatternType.Solid;
				result.Fill.ForeColor = DXColor.FromArgb(255, 255, 235, 156);
				return result;
			}
		}
		public static XlCellFormatting Calculation {
			get {
				XlCellFormatting result = new XlCellFormatting();
				result.Font = new XlFont();
				result.Font.Color = DXColor.FromArgb(255, 250, 125, 0);
				result.Font.Bold = true;
				result.Fill = new XlFill();
				result.Fill.PatternType = XlPatternType.Solid;
				result.Fill.ForeColor = DXColor.FromArgb(255, 242, 242, 242);
				result.Border = new XlBorder();
				result.Border.LeftColor = DXColor.FromArgb(255, 127, 127, 127);
				result.Border.LeftLineStyle = XlBorderLineStyle.Thin;
				result.Border.RightColor = DXColor.FromArgb(255, 127, 127, 127);
				result.Border.RightLineStyle = XlBorderLineStyle.Thin;
				result.Border.TopColor = DXColor.FromArgb(255, 127, 127, 127);
				result.Border.TopLineStyle = XlBorderLineStyle.Thin;
				result.Border.BottomColor = DXColor.FromArgb(255, 127, 127, 127);
				result.Border.BottomLineStyle = XlBorderLineStyle.Thin;
				return result;
			}
		}
		public static XlCellFormatting CheckCell {
			get {
				XlCellFormatting result = new XlCellFormatting();
				result.Font = XlFont.BodyFont();
				result.Font.Color = XlColor.FromTheme(XlThemeColor.Light1, 0.0);
				result.Font.Bold = true;
				result.Fill = XlFill.SolidFill(DXColor.FromArgb(0xa5, 0xa5, 0xa5));
				result.Border = XlBorder.OutlineBorders(DXColor.FromArgb(0x3f, 0x3f, 0x3f), XlBorderLineStyle.Double);
				return result;
			}
		}
		public static XlCellFormatting Explanatory {
			get {
				XlCellFormatting result = new XlCellFormatting();
				result.Font = new XlFont();
				result.Font.Color = DXColor.FromArgb(255, 127, 127, 127);
				result.Font.Italic = true;
				return result;
			}
		}
		public static XlCellFormatting Input {
			get {
				XlCellFormatting result = new XlCellFormatting();
				result.Font = new XlFont();
				result.Font.Color = DXColor.FromArgb(255, 63, 63, 118);
				result.Fill = new XlFill();
				result.Fill.PatternType = XlPatternType.Solid;
				result.Fill.ForeColor = DXColor.FromArgb(255, 255, 204, 153);
				result.Border = new XlBorder();
				result.Border.LeftColor = DXColor.FromArgb(255, 127, 127, 127);
				result.Border.LeftLineStyle = XlBorderLineStyle.Thin;
				result.Border.RightColor = DXColor.FromArgb(255, 127, 127, 127);
				result.Border.RightLineStyle = XlBorderLineStyle.Thin;
				result.Border.TopColor = DXColor.FromArgb(255, 127, 127, 127);
				result.Border.TopLineStyle = XlBorderLineStyle.Thin;
				result.Border.BottomColor = DXColor.FromArgb(255, 127, 127, 127);
				result.Border.BottomLineStyle = XlBorderLineStyle.Thin;
				return result;
			}
		}
		public static XlCellFormatting LinkedCell {
			get {
				XlCellFormatting result = new XlCellFormatting();
				result.Font = new XlFont();
				result.Font.Color = DXColor.FromArgb(255, 250, 125, 0);
				result.Border = new XlBorder();
				result.Border.BottomColor = DXColor.FromArgb(255, 255, 128, 1);
				result.Border.BottomLineStyle = XlBorderLineStyle.Double;
				return result;
			}
		}
		public static XlCellFormatting Note {
			get {
				XlCellFormatting result = new XlCellFormatting();
				result.Fill = new XlFill();
				result.Fill.PatternType = XlPatternType.Solid;
				result.Fill.ForeColor = DXColor.FromArgb(255, 255, 255, 204);
				result.Border = new XlBorder();
				result.Border.LeftColor = DXColor.FromArgb(255, 178, 178, 178);
				result.Border.LeftLineStyle = XlBorderLineStyle.Thin;
				result.Border.RightColor = DXColor.FromArgb(255, 178, 178, 178);
				result.Border.RightLineStyle = XlBorderLineStyle.Thin;
				result.Border.TopColor = DXColor.FromArgb(255, 178, 178, 178);
				result.Border.TopLineStyle = XlBorderLineStyle.Thin;
				result.Border.BottomColor = DXColor.FromArgb(255, 178, 178, 178);
				result.Border.BottomLineStyle = XlBorderLineStyle.Thin;
				return result;
			}
		}
		public static XlCellFormatting Output {
			get {
				XlCellFormatting result = new XlCellFormatting();
				result.Font = new XlFont();
				result.Font.Color = DXColor.FromArgb(255, 63, 63, 63);
				result.Fill = new XlFill();
				result.Fill.PatternType = XlPatternType.Solid;
				result.Fill.ForeColor = DXColor.FromArgb(255, 242, 242, 242);
				result.Border = new XlBorder();
				result.Border.LeftColor = DXColor.FromArgb(255, 63, 63, 63);
				result.Border.LeftLineStyle = XlBorderLineStyle.Thin;
				result.Border.RightColor = DXColor.FromArgb(255, 63, 63, 63);
				result.Border.RightLineStyle = XlBorderLineStyle.Thin;
				result.Border.TopColor = DXColor.FromArgb(255, 63, 63, 63);
				result.Border.TopLineStyle = XlBorderLineStyle.Thin;
				result.Border.BottomColor = DXColor.FromArgb(255, 63, 63, 63);
				result.Border.BottomLineStyle = XlBorderLineStyle.Thin;
				return result;
			}
		}
		public static XlCellFormatting WarningText {
			get {
				XlCellFormatting result = new XlCellFormatting();
				result.Font = new XlFont();
				result.Font.Color = DXColor.Red;
				return result;
			}
		}
		public static XlCellFormatting Hyperlink {
			get {
				XlCellFormatting result = new XlCellFormatting();
				result.Font = new XlFont();
				result.Font.Color = DXColor.Blue;
				result.Font.Underline = XlUnderlineType.Single;
				return result;
			}
		}
		public static XlCellFormatting Heading1 {
			get {
				XlCellFormatting result = new XlCellFormatting();
				result.Font = XlFont.BodyFont();
				result.Font.Color = XlColor.FromTheme(XlThemeColor.Dark2, 0.0);
				result.Font.Size = 15;
				result.Font.Bold = true;
				result.Border = new XlBorder();
				result.Border.BottomColor = XlColor.FromTheme(XlThemeColor.Accent1, 0.0);
				result.Border.BottomLineStyle = XlBorderLineStyle.Thick;
				return result;
			}
		}
		public static XlCellFormatting Heading2 {
			get {
				XlCellFormatting result = new XlCellFormatting();
				result.Font = XlFont.BodyFont();
				result.Font.Color = XlColor.FromTheme(XlThemeColor.Dark2, 0.0);
				result.Font.Size = 13;
				result.Font.Bold = true;
				result.Border = new XlBorder();
				result.Border.BottomColor = XlColor.FromTheme(XlThemeColor.Accent1, 0.5);
				result.Border.BottomLineStyle = XlBorderLineStyle.Thick;
				return result;
			}
		}
		public static XlCellFormatting Heading3 {
			get {
				XlCellFormatting result = new XlCellFormatting();
				result.Font = XlFont.BodyFont();
				result.Font.Color = XlColor.FromTheme(XlThemeColor.Dark2, 0.0);
				result.Font.Bold = true;
				result.Border = new XlBorder();
				result.Border.BottomColor = XlColor.FromTheme(XlThemeColor.Accent1, 0.4);
				result.Border.BottomLineStyle = XlBorderLineStyle.Medium;
				return result;
			}
		}
		public static XlCellFormatting Heading4 {
			get {
				XlCellFormatting result = new XlCellFormatting();
				result.Font = XlFont.BodyFont();
				result.Font.Color = XlColor.FromTheme(XlThemeColor.Dark2, 0.0);
				result.Font.Bold = true;
				return result;
			}
		}
		public static XlCellFormatting Title {
			get {
				XlCellFormatting result = new XlCellFormatting();
				result.Font = XlFont.HeadingsFont();
				result.Font.Color = XlColor.FromTheme(XlThemeColor.Dark2, 0.0);
				result.Font.Bold = true;
				result.Font.Size = 18;
				return result;
			}
		}
		public static XlCellFormatting Total {
			get {
				XlCellFormatting result = new XlCellFormatting();
				result.Font = XlFont.BodyFont();
				result.Font.Color = XlColor.FromTheme(XlThemeColor.Dark1, 0.0);
				result.Font.Bold = true;
				result.Border = new XlBorder();
				result.Border.TopColor = XlColor.FromTheme(XlThemeColor.Accent1, 0.0);
				result.Border.TopLineStyle = XlBorderLineStyle.Thin;
				result.Border.BottomColor = XlColor.FromTheme(XlThemeColor.Accent1, 0.0);
				result.Border.BottomLineStyle = XlBorderLineStyle.Double;
				return result;
			}
		}
		public static XlCellFormatting Themed(XlThemeColor themeColor, double tint) {
			if(themeColor != XlThemeColor.Accent1 && themeColor != XlThemeColor.Accent2 && themeColor != XlThemeColor.Accent3 &&
				themeColor != XlThemeColor.Accent4 && themeColor != XlThemeColor.Accent5 && themeColor != XlThemeColor.Accent6)
				throw new ArgumentException("themeColor: accent color required");
			XlCellFormatting result = new XlCellFormatting();
			result.Font = XlFont.BodyFont();
			result.Font.Color = XlColor.FromTheme((tint >= 0.5) ? XlThemeColor.Dark1 : XlThemeColor.Light1, 0.0);
			result.Fill = XlFill.SolidFill(XlColor.FromTheme(themeColor, tint));
			return result;
		}
		#endregion
		public XlCellFormatting Clone() {
			XlCellFormatting result = new XlCellFormatting();
			result.CopyFrom(this);
			return result;
		}
		public void CopyFrom(XlCellFormatting other) {
			if(other == null) {
				Font = null;
				Fill = null;
				Alignment = null;
				NetFormatString = null;
				IsDateTimeFormatString = false;
				NumberFormat = null;
				Border = null;
			}
			else {
				this.Font = CopyObject(other.Font);
				this.Fill = CopyObject(other.Fill);
				this.Alignment = CopyObject(other.Alignment);
				this.Border = CopyObject(other.Border);
				this.NetFormatString = other.NetFormatString;
				this.IsDateTimeFormatString = other.IsDateTimeFormatString;
				this.NumberFormat = other.NumberFormat;
			}
		}
		public XlDifferentialFormatting ToDifferentialFormatting() {
			XlDifferentialFormatting result = new XlDifferentialFormatting();
			result.Font = CopyObject(this.Font);
			if (this.Fill != null) {
				if (this.Fill.PatternType == XlPatternType.Solid) {
					result.Fill = new XlFill();
					result.Fill.PatternType = XlPatternType.Solid;
					result.Fill.BackColor = this.Fill.ForeColor;
				}
				else
					result.Fill = CopyObject(this.Fill);
			}
			result.Alignment = CopyObject(this.Alignment);
			result.Border = CopyObject(this.Border);
			result.NetFormatString = this.NetFormatString;
			result.IsDateTimeFormatString = this.IsDateTimeFormatString;
			result.NumberFormat = this.NumberFormat;
			return result;
		}
		public void MergeWith(XlCellFormatting other) {
			if(other == null)
				return;
			if(other.Font != null)
				this.Font = CopyObject(other.Font);
			if(other.Fill != null)
				this.Fill = CopyObject(other.Fill);
			if(other.Alignment != null)
				this.Alignment = CopyObject(other.Alignment);
			if (other.Border != null)
				this.Border = CopyObject(other.Border);
			if(other.NetFormatString != null) {
				this.NetFormatString = other.NetFormatString;
				this.IsDateTimeFormatString = other.IsDateTimeFormatString;
			}
			if(other.NumberFormat != null)
				this.NumberFormat = other.NumberFormat;
		}
		public static XlCellFormatting Merge(XlCellFormatting target, XlCellFormatting source) {
			if(target == null)
				return XlFormatting.CopyObject(source);
			target.MergeWith(source);
			return target;
		}
		public static bool Equals(XlCellFormatting first, XlCellFormatting second) {
			if(object.ReferenceEquals(first, second))
				return true;
			if(first != null && second != null) {
				if(!EqualObjects(first.Fill, second.Fill))
					return false;
				if(!EqualObjects(first.Font, second.Font))
					return false;
				if(!EqualObjects(first.Border, second.Border))
					return false;
				if(!EqualObjects(first.Alignment, second.Alignment))
					return false;
				if(!EqualObjects(first.NumberFormat, second.NumberFormat))
					return false;
				if ((!string.IsNullOrEmpty(first.NetFormatString) || !string.IsNullOrEmpty(second.NetFormatString)) && 
					!string.Equals(first.NetFormatString, second.NetFormatString, StringComparison.Ordinal))
					return false;
				return first.IsDateTimeFormatString == second.IsDateTimeFormatString;
			}
			return false;
		}
		internal static bool EqualFonts(XlCellFormatting first, XlCellFormatting second) {
			if(object.ReferenceEquals(first, second))
				return true;
			if(first != null && second != null)
				return EqualObjects(first.Font, second.Font);
			return false;
		}
		static bool EqualObjects<T>(T first, T second) where T : class {
			if(object.ReferenceEquals(first, second))
				return true;
			if(first == null)
				return false;
			return first.Equals(second);
		}
		#region Implicit conversions
		public static implicit operator XlCellFormatting(XlCellAlignment alignment) {
			XlCellFormatting result = new XlCellFormatting();
			result.Alignment = alignment;
			return result;
		}
		public static implicit operator XlCellFormatting(XlBorder border) {
			XlCellFormatting result = new XlCellFormatting();
			result.Border = border;
			return result;
		}
		public static implicit operator XlCellFormatting(XlFill fill) {
			XlCellFormatting result = new XlCellFormatting();
			result.Fill = fill;
			return result;
		}
		public static implicit operator XlCellFormatting(XlFont font) {
			XlCellFormatting result = new XlCellFormatting();
			result.Font = font;
			return result;
		}
		public static implicit operator XlCellFormatting(XlNumberFormat numberFormat) {
			XlCellFormatting result = new XlCellFormatting();
			result.NumberFormat = numberFormat;
			return result;
		}
		#endregion
		public static XlCellFormatting FromNetFormat(string formatString, bool isDateTimeFormat) {
			XlCellFormatting result = new XlCellFormatting();
			result.NetFormatString = formatString;
			result.IsDateTimeFormatString = isDateTimeFormat;
			return result;
		}
	}
	#endregion
	#region XlDifferentialFormatting
	public class XlDifferentialFormatting : XlFormatting, ISupportsCopyFrom<XlDifferentialFormatting> {
		public void CopyFrom(XlDifferentialFormatting other) {
			if(other == null) {
				Font = null;
				Fill = null;
				Alignment = null;
				NetFormatString = null;
				IsDateTimeFormatString = false;
				Border = null;
				NumberFormat = null;
			}
			else {
				this.Font = CopyObject(other.Font);
				this.Fill = CopyObject(other.Fill);
				this.Alignment = CopyObject(other.Alignment);
				this.Border = CopyObject(other.Border);
				this.NetFormatString = other.NetFormatString;
				this.IsDateTimeFormatString = other.IsDateTimeFormatString;
				this.NumberFormat = other.NumberFormat;
			}
		}
		#region Implicit conversions
		public static implicit operator XlDifferentialFormatting(XlCellFormatting formatting) {
			return formatting.ToDifferentialFormatting();
		}
		public static implicit operator XlDifferentialFormatting(XlCellAlignment alignment) {
			XlCellFormatting result = new XlCellFormatting();
			result.Alignment = alignment;
			return result.ToDifferentialFormatting();
		}
		public static implicit operator XlDifferentialFormatting(XlBorder border) {
			XlCellFormatting result = new XlCellFormatting();
			result.Border = border;
			return result.ToDifferentialFormatting();
		}
		public static implicit operator XlDifferentialFormatting(XlFill fill) {
			XlCellFormatting result = new XlCellFormatting();
			result.Fill = fill;
			return result.ToDifferentialFormatting();
		}
		public static implicit operator XlDifferentialFormatting(XlFont font) {
			XlCellFormatting result = new XlCellFormatting();
			result.Font = font;
			return result.ToDifferentialFormatting();
		}
		public static implicit operator XlDifferentialFormatting(XlNumberFormat numberFormat) {
			XlCellFormatting result = new XlCellFormatting();
			result.NumberFormat = numberFormat;
			return result.ToDifferentialFormatting();
		}
		#endregion
	}
	#endregion
}
