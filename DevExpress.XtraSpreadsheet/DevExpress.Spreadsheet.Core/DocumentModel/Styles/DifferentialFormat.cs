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

using System.Drawing;
using System.Runtime.InteropServices;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.Export.Xl;
using System.Collections.Generic;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraSpreadsheet.Model {
	#region NumberFormatIdInfo
	public struct NumberFormatIdInfo : ICloneable<NumberFormatIdInfo>, ISupportsSizeOf { 
		int id;
		public int Id { get { return id; } set { this.id = value; } }
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return DXMarshal.SizeOf(typeof(NumberFormatIdInfo));
		}
		#endregion
		#region ICloneable<CellFormatFlagsInfo> Members
		public NumberFormatIdInfo Clone() {
			NumberFormatIdInfo clone = new NumberFormatIdInfo();
			clone.id = this.id;
			return clone;
		}
		#endregion
	}
	#endregion
	#region BorderOptionsBorderAccessor (abstract class)
	public abstract class BorderOptionsBorderAccessor {
		public abstract bool GetApplyColor(BorderOptionsInfo info);
		public abstract bool GetApplyLineStyle(BorderOptionsInfo info);
		public abstract void SetApplyColor(BorderOptionsInfo info, bool value);
		public abstract void SetApplyLineStyle(BorderOptionsInfo info, bool value);
	}
	#endregion
	#region BorderOptionsLeftBorderAccessor
	public class BorderOptionsLeftBorderAccessor : BorderOptionsBorderAccessor {
		public override bool GetApplyColor(BorderOptionsInfo info) {
			return info.ApplyLeftColor;
		}
		public override bool GetApplyLineStyle(BorderOptionsInfo info) {
			return info.ApplyLeftLineStyle;
		}
		public override void SetApplyColor(BorderOptionsInfo info, bool value) {
			info.ApplyLeftColor = value;
		}
		public override void SetApplyLineStyle(BorderOptionsInfo info, bool value) {
			info.ApplyLeftLineStyle = value;
		}
	}
	#endregion
	#region BorderOptionsRightBorderAccessor
	public class BorderOptionsRightBorderAccessor : BorderOptionsBorderAccessor {
		public override bool GetApplyColor(BorderOptionsInfo info) {
			return info.ApplyRightColor;
		}
		public override bool GetApplyLineStyle(BorderOptionsInfo info) {
			return info.ApplyRightLineStyle;
		}
		public override void SetApplyColor(BorderOptionsInfo info, bool value) {
			info.ApplyRightColor = value;
		}
		public override void SetApplyLineStyle(BorderOptionsInfo info, bool value) {
			info.ApplyRightLineStyle = value;
		}
	}
	#endregion
	#region BorderOptionsTopBorderAccessor
	public class BorderOptionsTopBorderAccessor : BorderOptionsBorderAccessor {
		public override bool GetApplyColor(BorderOptionsInfo info) {
			return info.ApplyTopColor;
		}
		public override bool GetApplyLineStyle(BorderOptionsInfo info) {
			return info.ApplyTopLineStyle;
		}
		public override void SetApplyColor(BorderOptionsInfo info, bool value) {
			info.ApplyTopColor = value;
		}
		public override void SetApplyLineStyle(BorderOptionsInfo info, bool value) {
			info.ApplyTopLineStyle = value;
		}
	}
	#endregion
	#region BorderOptionsBottomBorderAccessor
	public class BorderOptionsBottomBorderAccessor : BorderOptionsBorderAccessor {
		public override bool GetApplyColor(BorderOptionsInfo info) {
			return info.ApplyBottomColor;
		}
		public override bool GetApplyLineStyle(BorderOptionsInfo info) {
			return info.ApplyBottomLineStyle;
		}
		public override void SetApplyColor(BorderOptionsInfo info, bool value) {
			info.ApplyBottomColor = value;
		}
		public override void SetApplyLineStyle(BorderOptionsInfo info, bool value) {
			info.ApplyBottomLineStyle = value;
		}
	}
	#endregion
	#region BorderOptionsVerticalBorderAccessor
	public class BorderOptionsVerticalBorderAccessor : BorderOptionsBorderAccessor {
		public override bool GetApplyColor(BorderOptionsInfo info) {
			return info.ApplyVerticalColor;
		}
		public override bool GetApplyLineStyle(BorderOptionsInfo info) {
			return info.ApplyVerticalLineStyle;
		}
		public override void SetApplyColor(BorderOptionsInfo info, bool value) {
			info.ApplyVerticalColor = value;
		}
		public override void SetApplyLineStyle(BorderOptionsInfo info, bool value) {
			info.ApplyVerticalLineStyle = value;
		}
	}
	#endregion
	#region BorderOptionsHorizontalBorderAccessor
	public class BorderOptionsHorizontalBorderAccessor : BorderOptionsBorderAccessor {
		public override bool GetApplyColor(BorderOptionsInfo info) {
			return info.ApplyHorizontalColor;
		}
		public override bool GetApplyLineStyle(BorderOptionsInfo info) {
			return info.ApplyHorizontalLineStyle;
		}
		public override void SetApplyColor(BorderOptionsInfo info, bool value) {
			info.ApplyHorizontalColor = value;
		}
		public override void SetApplyLineStyle(BorderOptionsInfo info, bool value) {
			info.ApplyHorizontalLineStyle = value;
		}
	}
	#endregion
	#region BorderOptionsDiagonalBorderAccessor
	public class BorderOptionsDiagonalBorderAccessor : BorderOptionsBorderAccessor {
		public override bool GetApplyColor(BorderOptionsInfo info) {
			return info.ApplyDiagonalColor;
		}
		public override bool GetApplyLineStyle(BorderOptionsInfo info) {
			return info.ApplyDiagonalLineStyle;
		}
		public override void SetApplyColor(BorderOptionsInfo info, bool value) {
			info.ApplyDiagonalColor = value;
		}
		public override void SetApplyLineStyle(BorderOptionsInfo info, bool value) {
			info.ApplyDiagonalLineStyle = value;
		}
	}
	#endregion
	#region BorderOptionsInfo
	public struct BorderOptionsInfo : ICloneable<BorderOptionsInfo>, ISupportsCopyFrom<BorderOptionsInfo>, ISupportsSizeOf {
		#region Static Members
		static readonly BorderOptionsBorderAccessor leftBorderAccessor = new BorderOptionsLeftBorderAccessor();
		static readonly BorderOptionsBorderAccessor rightBorderAccessor = new BorderOptionsRightBorderAccessor();
		static readonly BorderOptionsBorderAccessor topBorderAccessor = new BorderOptionsTopBorderAccessor();
		static readonly BorderOptionsBorderAccessor bottomBorderAccessor = new BorderOptionsBottomBorderAccessor();
		static readonly BorderOptionsBorderAccessor verticalBorderAccessor = new BorderOptionsVerticalBorderAccessor();
		static readonly BorderOptionsBorderAccessor horizontalBorderAccessor = new BorderOptionsHorizontalBorderAccessor();
		static readonly BorderOptionsBorderAccessor diagonalBorderAccessor = new BorderOptionsDiagonalBorderAccessor();
		public static BorderOptionsBorderAccessor LeftBorderAccessor { get { return leftBorderAccessor; } }
		public static BorderOptionsBorderAccessor RightBorderAccessor { get { return rightBorderAccessor; } }
		public static BorderOptionsBorderAccessor TopBorderAccessor { get { return topBorderAccessor; } }
		public static BorderOptionsBorderAccessor BottomBorderAccessor { get { return bottomBorderAccessor; } }
		public static BorderOptionsBorderAccessor VerticalBorderAccessor { get { return verticalBorderAccessor; } }
		public static BorderOptionsBorderAccessor HorizontalBorderAccessor { get { return horizontalBorderAccessor; } }
		public static BorderOptionsBorderAccessor DiagonalBorderAccessor { get { return diagonalBorderAccessor; } }
		public static BorderOptionsInfo Create(int value) {
			BorderOptionsInfo result = new BorderOptionsInfo();
			result.packedValues = value;
			return result;
		}
		#endregion
		#region Fields
		internal const int MaskApplyLeftLineStyle = 0x00000001;
		internal const int MaskApplyRightLineStyle = 0x00000002;
		internal const int MaskApplyTopLineStyle = 0x00000004;
		internal const int MaskApplyBottomLineStyle = 0x00000008;
		internal const int MaskApplyHorizontalLineStyle = 0x00000010;
		internal const int MaskApplyVerticalLineStyle = 0x00000020;
		internal const int MaskApplyDiagonalLineStyle = 0x00000040;
		internal const int MaskApplyLeftColor = 0x00000080;
		internal const int MaskApplyRightColor = 0x00000100;
		internal const int MaskApplyTopColor = 0x00000200;
		internal const int MaskApplyBottomColor = 0x00000400;
		internal const int MaskApplyHorizontalColor = 0x00000800;
		internal const int MaskApplyVerticalColor = 0x00001000;
		internal const int MaskApplyDiagonalColor = 0x00002000;
		internal const int MaskApplyDiagonalUp = 0x00004000;
		internal const int MaskApplyDiagonalDown = 0x00008000;
		internal const int MaskApplyOutline = 0x00010000;
		const int MaskApplyBorder = 0x0001FFFF;
		internal const int DefaultIndex = 0;
		int packedValues;
		#endregion
		#region Properties
		public int PackedValues { get { return packedValues; } set { this.packedValues = value; } }
		public bool ApplyNone { get { return !GetBooleanValue(MaskApplyBorder); } }
		public bool ApplyLeftLineStyle { get { return GetBooleanValue(MaskApplyLeftLineStyle); } set { SetBooleanValue(MaskApplyLeftLineStyle, value); } }
		public bool ApplyRightLineStyle { get { return GetBooleanValue(MaskApplyRightLineStyle); } set { SetBooleanValue(MaskApplyRightLineStyle, value); } }
		public bool ApplyTopLineStyle { get { return GetBooleanValue(MaskApplyTopLineStyle); } set { SetBooleanValue(MaskApplyTopLineStyle, value); } }
		public bool ApplyBottomLineStyle { get { return GetBooleanValue(MaskApplyBottomLineStyle); } set { SetBooleanValue(MaskApplyBottomLineStyle, value); } }
		public bool ApplyHorizontalLineStyle { get { return GetBooleanValue(MaskApplyHorizontalLineStyle); } set { SetBooleanValue(MaskApplyHorizontalLineStyle, value); } }
		public bool ApplyVerticalLineStyle { get { return GetBooleanValue(MaskApplyVerticalLineStyle); } set { SetBooleanValue(MaskApplyVerticalLineStyle, value); } }
		public bool ApplyDiagonalLineStyle { get { return GetBooleanValue(MaskApplyDiagonalLineStyle); } set { SetBooleanValue(MaskApplyDiagonalLineStyle, value); } }
		public bool ApplyLeftColor { get { return GetBooleanValue(MaskApplyLeftColor); } set { SetBooleanValue(MaskApplyLeftColor, value); } }
		public bool ApplyRightColor { get { return GetBooleanValue(MaskApplyRightColor); } set { SetBooleanValue(MaskApplyRightColor, value); } }
		public bool ApplyTopColor { get { return GetBooleanValue(MaskApplyTopColor); } set { SetBooleanValue(MaskApplyTopColor, value); } }
		public bool ApplyBottomColor { get { return GetBooleanValue(MaskApplyBottomColor); } set { SetBooleanValue(MaskApplyBottomColor, value); } }
		public bool ApplyHorizontalColor { get { return GetBooleanValue(MaskApplyHorizontalColor); } set { SetBooleanValue(MaskApplyHorizontalColor, value); } }
		public bool ApplyVerticalColor { get { return GetBooleanValue(MaskApplyVerticalColor); } set { SetBooleanValue(MaskApplyVerticalColor, value); } }
		public bool ApplyDiagonalColor { get { return GetBooleanValue(MaskApplyDiagonalColor); } set { SetBooleanValue(MaskApplyDiagonalColor, value); } }
		public bool ApplyDiagonalUp { get { return GetBooleanValue(MaskApplyDiagonalUp); } set { SetBooleanValue(MaskApplyDiagonalUp, value); } }
		public bool ApplyDiagonalDown { get { return GetBooleanValue(MaskApplyDiagonalDown); } set { SetBooleanValue(MaskApplyDiagonalDown, value); } }
		public bool ApplyOutline { get { return GetBooleanValue(MaskApplyOutline); } set { SetBooleanValue(MaskApplyOutline, value); } }
		#endregion
		#region GetBooleanValue/SetBooleanValue helpers
		void SetBooleanValue(int mask, bool bitVal) {
			if (bitVal)
				packedValues |= mask;
			else
				packedValues &= ~mask;
		}
		bool GetBooleanValue(int mask) {
			return (packedValues & mask) != 0;
		}
		#endregion
		#region SetPropertiesFromFormat
		public void SetPropertiesFromFormat(FormatBase format) {
			BorderInfoCache cache = format.DocumentModel.Cache.BorderInfoCache;
			BorderInfo info = cache[format.BorderIndex];
			BorderInfo defaultInfo = cache.DefaultItem;
			ApplyLeftLineStyle = info.LeftLineStyle != defaultInfo.LeftLineStyle;
			ApplyRightLineStyle = info.RightLineStyle != defaultInfo.RightLineStyle;
			ApplyTopLineStyle = info.TopLineStyle != defaultInfo.TopLineStyle;
			ApplyBottomLineStyle = info.BottomLineStyle != defaultInfo.BottomLineStyle;
			ApplyHorizontalLineStyle = info.HorizontalLineStyle != defaultInfo.HorizontalLineStyle;
			ApplyVerticalLineStyle = info.VerticalLineStyle != defaultInfo.VerticalLineStyle;
			ApplyDiagonalLineStyle = info.DiagonalLineStyle != defaultInfo.DiagonalLineStyle;
			ApplyLeftColor = info.LeftColorIndex != defaultInfo.LeftColorIndex;
			ApplyRightColor = info.RightColorIndex != defaultInfo.RightColorIndex;
			ApplyTopColor = info.TopColorIndex != defaultInfo.TopColorIndex;
			ApplyBottomColor = info.BottomColorIndex != defaultInfo.BottomColorIndex;
			ApplyHorizontalColor = info.HorizontalColorIndex != defaultInfo.HorizontalColorIndex;
			ApplyVerticalColor = info.VerticalColorIndex != defaultInfo.VerticalColorIndex;
			ApplyDiagonalColor = info.DiagonalColorIndex != defaultInfo.DiagonalColorIndex;
			ApplyDiagonalUp = info.DiagonalUp != defaultInfo.DiagonalUp;
			ApplyDiagonalDown = info.DiagonalDown != defaultInfo.DiagonalDown;
			ApplyOutline = info.Outline != defaultInfo.Outline;
		}
		#endregion
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return DXMarshal.SizeOf(typeof(BorderOptionsInfo));
		}
		#endregion
		#region ICloneable<BorderOptionsInfo> Members
		public BorderOptionsInfo Clone() {
			return BorderOptionsInfo.Create(this.PackedValues);
		}
		#endregion
		#region ISupportsCopyFrom<BorderOptionsInfo> Members
		public void CopyFrom(BorderOptionsInfo value) {
			this.packedValues = value.packedValues;
		}
		#endregion
	}
	#endregion
	#region MultiOptionsInfo
	public struct MultiOptionsInfo : ICloneable<MultiOptionsInfo>, ISupportsCopyFrom<MultiOptionsInfo>, ISupportsSizeOf {
		#region Static Members
		public static int DefaultIndex = 0;
		public static MultiOptionsInfo Create(int value) {
			MultiOptionsInfo result = new MultiOptionsInfo();
			result.packedValues = value;
			return result;
		}
		#endregion
		#region Fields
		#region MaskFont
		internal const int MaskApplyFontName = 0x00000001;
		internal const int MaskApplyFontColor = 0x00000002;
		internal const int MaskApplyFontBold = 0x00000004;
		internal const int MaskApplyFontCondense = 0x00000008;
		internal const int MaskApplyFontExtend = 0x00000010;
		internal const int MaskApplyFontItalic = 0x00000020;
		internal const int MaskApplyFontOutline = 0x00000040;
		internal const int MaskApplyFontShadow = 0x00000080;
		internal const int MaskApplyFontStrikeThrough = 0x00000100;
		internal const int MaskApplyFontCharset = 0x00000200;
		internal const int MaskApplyFontFamily = 0x00000400;
		internal const int MaskApplyFontSize = 0x00000800;
		internal const int MaskApplyFontSchemeStyle = 0x00001000;
		internal const int MaskApplyFontScript = 0x00002000;
		internal const int MaskApplyFontUnderline = 0x00004000;
		const int MaskApplyFont = 0x00007FFF;
		#endregion
		#region MaskFill
		internal const int MaskApplyFillPatternType = 0x00008000;
		internal const int MaskApplyFillBackColor = 0x00010000;
		internal const int MaskApplyFillForeColor = 0x00020000;
		const int MaskApplyFill = 0x00038000;
		#endregion
		#region MaskAlignment
		internal const int MaskApplyAlignmentHorizontal = 0x00040000;
		internal const int MaskApplyAlignmentVertical = 0x00080000;
		internal const int MaskApplyAlignmentWrapText = 0x00100000;
		internal const int MaskApplyAlignmentJustifyLastLine = 0x00200000;
		internal const int MaskApplyAlignmentShrinkToFit = 0x00400000;
		internal const int MaskApplyAlignmentReadingOrder = 0x00800000;
		internal const int MaskApplyAlignmentTextRotation = 0x01000000;
		internal const int MaskApplyAlignmentIndent = 0x02000000;
		internal const int MaskApplyAlignmentRelativeIndent = 0x04000000;
		const int MaskApplyAlignment = 0x07FC0000;
		#endregion
		#region MaskProtection
		internal const int MaskApplyProtectionLocked = 0x08000000;
		internal const int MaskApplyProtectionHidden = 0x10000000;
		const int MaskApplyProtection = 0x18000000;
		#endregion
		#region MaskNumberFormat
		internal const int MaskApplyNumberFormat = 0x20000000;
		#endregion
		int packedValues;
		#endregion
		#region Properties
		public int PackedValues { get { return packedValues; } set { this.packedValues = value; } }
		#region FontOptions
		public bool ApplyFontNone { get { return !GetBooleanValue(MaskApplyFont); } }
		public bool ApplyFontName { get { return GetBooleanValue(MaskApplyFontName); } set { SetBooleanValue(MaskApplyFontName, value); } }
		public bool ApplyFontColor { get { return GetBooleanValue(MaskApplyFontColor); } set { SetBooleanValue(MaskApplyFontColor, value); } }
		public bool ApplyFontBold { get { return GetBooleanValue(MaskApplyFontBold); } set { SetBooleanValue(MaskApplyFontBold, value); } }
		public bool ApplyFontCondense { get { return GetBooleanValue(MaskApplyFontCondense); } set { SetBooleanValue(MaskApplyFontCondense, value); } }
		public bool ApplyFontExtend { get { return GetBooleanValue(MaskApplyFontExtend); } set { SetBooleanValue(MaskApplyFontExtend, value); } }
		public bool ApplyFontItalic { get { return GetBooleanValue(MaskApplyFontItalic); } set { SetBooleanValue(MaskApplyFontItalic, value); } }
		public bool ApplyFontOutline { get { return GetBooleanValue(MaskApplyFontOutline); } set { SetBooleanValue(MaskApplyFontOutline, value); } }
		public bool ApplyFontShadow { get { return GetBooleanValue(MaskApplyFontShadow); } set { SetBooleanValue(MaskApplyFontShadow, value); } }
		public bool ApplyFontStrikeThrough { get { return GetBooleanValue(MaskApplyFontStrikeThrough); } set { SetBooleanValue(MaskApplyFontStrikeThrough, value); } }
		public bool ApplyFontCharset { get { return GetBooleanValue(MaskApplyFontCharset); } set { SetBooleanValue(MaskApplyFontCharset, value); } }
		public bool ApplyFontFamily { get { return GetBooleanValue(MaskApplyFontFamily); } set { SetBooleanValue(MaskApplyFontFamily, value); } }
		public bool ApplyFontSize { get { return GetBooleanValue(MaskApplyFontSize); } set { SetBooleanValue(MaskApplyFontSize, value); } }
		public bool ApplyFontSchemeStyle { get { return GetBooleanValue(MaskApplyFontSchemeStyle); } set { SetBooleanValue(MaskApplyFontSchemeStyle, value); } }
		public bool ApplyFontScript { get { return GetBooleanValue(MaskApplyFontScript); } set { SetBooleanValue(MaskApplyFontScript, value); } }
		public bool ApplyFontUnderline { get { return GetBooleanValue(MaskApplyFontUnderline); } set { SetBooleanValue(MaskApplyFontUnderline, value); } }
		#endregion
		#region FillOptions
		public bool ApplyFillNone { get { return !GetBooleanValue(MaskApplyFill); } }
		public bool ApplyFillPatternType { get { return GetBooleanValue(MaskApplyFillPatternType); } set { SetBooleanValue(MaskApplyFillPatternType, value); } }
		public bool ApplyFillBackColor { get { return GetBooleanValue(MaskApplyFillBackColor); } set { SetBooleanValue(MaskApplyFillBackColor, value); } }
		public bool ApplyFillForeColor { get { return GetBooleanValue(MaskApplyFillForeColor); } set { SetBooleanValue(MaskApplyFillForeColor, value); } }
		#endregion
		#region AlignmentOptions
		public bool ApplyAlignmentNone { get { return !GetBooleanValue(MaskApplyAlignment); } }
		public bool ApplyAlignmentHorizontal { get { return GetBooleanValue(MaskApplyAlignmentHorizontal); } set { SetBooleanValue(MaskApplyAlignmentHorizontal, value); } }
		public bool ApplyAlignmentVertical { get { return GetBooleanValue(MaskApplyAlignmentVertical); } set { SetBooleanValue(MaskApplyAlignmentVertical, value); } }
		public bool ApplyAlignmentWrapText { get { return GetBooleanValue(MaskApplyAlignmentWrapText); } set { SetBooleanValue(MaskApplyAlignmentWrapText, value); } }
		public bool ApplyAlignmentJustifyLastLine { get { return GetBooleanValue(MaskApplyAlignmentJustifyLastLine); } set { SetBooleanValue(MaskApplyAlignmentJustifyLastLine, value); } }
		public bool ApplyAlignmentShrinkToFit { get { return GetBooleanValue(MaskApplyAlignmentShrinkToFit); } set { SetBooleanValue(MaskApplyAlignmentShrinkToFit, value); } }
		public bool ApplyAlignmentTextRotation { get { return GetBooleanValue(MaskApplyAlignmentTextRotation); } set { SetBooleanValue(MaskApplyAlignmentTextRotation, value); } }
		public bool ApplyAlignmentIndent { get { return GetBooleanValue(MaskApplyAlignmentIndent); } set { SetBooleanValue(MaskApplyAlignmentIndent, value); } }
		public bool ApplyAlignmentRelativeIndent { get { return GetBooleanValue(MaskApplyAlignmentRelativeIndent); } set { SetBooleanValue(MaskApplyAlignmentRelativeIndent, value); } }
		public bool ApplyAlignmentReadingOrder { get { return GetBooleanValue(MaskApplyAlignmentReadingOrder); } set { SetBooleanValue(MaskApplyAlignmentReadingOrder, value); } }
		#endregion
		#region Protection
		public bool ApplyProtectionNone { get { return !GetBooleanValue(MaskApplyProtection); } }
		public bool ApplyProtectionLocked { get { return GetBooleanValue(MaskApplyProtectionLocked); } set { SetBooleanValue(MaskApplyProtectionLocked, value); } }
		public bool ApplyProtectionHidden { get { return GetBooleanValue(MaskApplyProtectionHidden); } set { SetBooleanValue(MaskApplyProtectionHidden, value); } }
		#endregion
		#region NumberFormat
		public bool ApplyNumberFormat { get { return GetBooleanValue(MaskApplyNumberFormat); } set { SetBooleanValue(MaskApplyNumberFormat, value); } }
		#endregion 
		#endregion
		internal void ClearFontOptions() {
			ResetPackedValues(MaskApplyFont, 0);
		}
		internal void ClearFillOptions() {
			ResetPackedValues(MaskApplyFill, 16);
		}
		#region GetBooleanValue/SetBooleanValue helpers
		void SetBooleanValue(int mask, bool bitVal) {
			if (bitVal)
				packedValues |= mask;
			else
				packedValues &= ~mask;
		}
		bool GetBooleanValue(int mask) {
			return (packedValues & mask) != 0;
		}
		#endregion
		#region ResetPackedValues
		void ResetPackedValues(uint mask, int position) {
			uint uintPackedValues = (uint)packedValues;
			uintPackedValues &= ~mask;
			uintPackedValues |= GetPackedValue(mask, position, 0);
			packedValues = (int)uintPackedValues;
		}
		uint GetPackedValue(uint mask, int position, uint value) {
			return (value << position) & mask;
		}
		#endregion
		#region SetPropertiesFromFormat
		public void SetPropertiesFromFormat(FormatBase format) {
			DocumentCache cache = format.DocumentModel.Cache;
			SetPropertiesFromNumberFormat(format.NumberFormatIndex);
			SetPropertiesFromFont(cache.FontInfoCache, format.FontIndex);
			SetPropertiesFromFill(cache.FillInfoCache, format.FillIndex);
			SetPropertiesFromAlignment(cache.CellAlignmentInfoCache, format.AlignmentIndex);
			SetPropertiesFromProtection(format);
		}
		void SetPropertiesFromNumberFormat(int formatIndex) {
			ApplyNumberFormat = formatIndex != NumberFormatCollection.DefaultItemIndex;
		}
		void SetPropertiesFromFont(RunFontInfoCache cache, int formatIndex) {
			RunFontInfo info = cache[formatIndex];
			RunFontInfo defaultInfo = cache.DefaultItem;
			ApplyFontName = info.Name != defaultInfo.Name;
			ApplyFontColor = info.ColorIndex != defaultInfo.ColorIndex;
			ApplyFontBold = info.Bold != defaultInfo.Bold;
			ApplyFontCondense = info.Condense != defaultInfo.Condense;
			ApplyFontExtend = info.Extend != defaultInfo.Extend;
			ApplyFontItalic = info.Italic != defaultInfo.Italic;
			ApplyFontOutline = info.Outline != defaultInfo.Outline;
			ApplyFontShadow = info.Shadow != defaultInfo.Shadow;
			ApplyFontStrikeThrough = info.StrikeThrough != defaultInfo.StrikeThrough;
			ApplyFontCharset = info.Charset != defaultInfo.Charset;
			ApplyFontFamily = info.FontFamily != defaultInfo.FontFamily;
			ApplyFontSize = info.Size != defaultInfo.Size;
			ApplyFontSchemeStyle = info.SchemeStyle != defaultInfo.SchemeStyle;
			ApplyFontScript = info.Script != defaultInfo.Script;
			ApplyFontUnderline = info.Underline != defaultInfo.Underline;
		}
		void SetPropertiesFromFill(FillInfoCache cache, int formatIndex) {
			FillInfo info = cache[formatIndex];
			FillInfo defaultInfo = cache.DefaultItem;
			ApplyFillPatternType = info.PatternType != defaultInfo.PatternType;
			ApplyFillBackColor = info.BackColorIndex != defaultInfo.BackColorIndex;
			ApplyFillForeColor = info.ForeColorIndex != defaultInfo.ForeColorIndex;
		}
		void SetPropertiesFromAlignment(CellAlignmentInfoCache cache, int formatIndex) {
			CellAlignmentInfo info = cache[formatIndex];
			CellAlignmentInfo defaultInfo = cache.DefaultItem;
			ApplyAlignmentHorizontal = info.HorizontalAlignment != defaultInfo.HorizontalAlignment;
			ApplyAlignmentVertical = info.VerticalAlignment != defaultInfo.VerticalAlignment;
			ApplyAlignmentWrapText = info.WrapText != defaultInfo.WrapText;
			ApplyAlignmentJustifyLastLine = info.JustifyLastLine != defaultInfo.JustifyLastLine;
			ApplyAlignmentShrinkToFit = info.ShrinkToFit != defaultInfo.ShrinkToFit;
			ApplyAlignmentReadingOrder = info.ReadingOrder != defaultInfo.ReadingOrder;
			ApplyAlignmentTextRotation = info.TextRotation != defaultInfo.TextRotation;
			ApplyAlignmentIndent = info.Indent != defaultInfo.Indent;
			ApplyAlignmentRelativeIndent = info.RelativeIndent != defaultInfo.RelativeIndent;
		}
		void SetPropertiesFromProtection(FormatBase format) {
			CellFormatFlagsInfo info = format.CellFormatFlagsInfo;
			CellFormatFlagsInfo defaultInfo = format is CellStyleFormat ? CellFormatFlagsInfo.DefaultStyle : CellFormatFlagsInfo.DefaultFormat;
			ApplyProtectionLocked = info.Locked != defaultInfo.Locked;
			ApplyProtectionHidden = info.Hidden != defaultInfo.Hidden;
		}
		#endregion
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return DXMarshal.SizeOf(typeof(MultiOptionsInfo));
		}
		#endregion
		#region ICloneable<MultiOptionsInfo> Members
		public MultiOptionsInfo Clone() {
			return MultiOptionsInfo.Create(this.PackedValues);
		}
		#endregion
		#region ISupportsCopyFrom<MultiOptionsInfo> Members
		public void CopyFrom(MultiOptionsInfo value) {
			this.packedValues = value.packedValues;
		}
		#endregion
	}
	#endregion
	#region IDifferentialFormat
	public interface IDifferentialFormat  {
		IRunFontInfo Font { get; }
		ICellAlignmentInfo Alignment { get; }
		IBorderInfo Border { get; }
		IFillInfo Fill { get; }
		ICellProtectionInfo Protection { get; }
		string FormatString { get; set; }
	}
	#endregion
	#region MultiOptionsIndexAccessor
	public class MultiOptionsIndexAccessor : IIndexAccessor<FormatBase, MultiOptionsInfo, DocumentModelChangeActions> {
		#region IIndexAccessor Members
		public int GetIndex(FormatBase owner) {
			return ((DifferentialFormat)owner).MultiOptionsIndex;
		}
		public int GetDeferredInfoIndex(FormatBase owner) {
			return GetInfoIndex(owner, GetDeferredInfo(owner.BatchUpdateHelper));
		}
		public void SetIndex(FormatBase owner, int value) {
			((DifferentialFormat)owner).AssignMultiOptionsIndex(value);
		}
		public int GetInfoIndex(FormatBase owner, MultiOptionsInfo value) {
			return value.PackedValues;
		}
		public MultiOptionsInfo GetInfo(FormatBase owner) {
			return MultiOptionsInfo.Create(((DifferentialFormat)owner).MultiOptionsIndex);
		}
		public bool IsIndexValid(FormatBase owner, int index) {
			return true;
		}
		public IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateHistoryItem(FormatBase owner) {
			return new DifferentialFormatMultiOptionsIndexChangeHistoryItem((DifferentialFormat)owner);
		}
		public MultiOptionsInfo GetDeferredInfo(MultiIndexBatchUpdateHelper helper) {
			return ((DifferentialFormatBatchUpdateHelper)helper).MultiOptionsInfo;
		}
		public void SetDeferredInfo(MultiIndexBatchUpdateHelper helper, MultiOptionsInfo info) {
			((DifferentialFormatBatchUpdateHelper)helper).MultiOptionsInfo = info.Clone();
		}
		public void InitializeDeferredInfo(FormatBase owner) {
			this.SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(owner));
		}
		public void CopyDeferredInfo(FormatBase owner, FormatBase from) {
			SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(from));
		}
		public bool ApplyDeferredChanges(FormatBase owner) {
			return owner.ReplaceInfo(this, GetDeferredInfo(owner.BatchUpdateHelper), owner.GetBatchUpdateChangeActions());
		}
		#endregion
	}
	#endregion
	#region BorderOptionsIndexAccessor
	public class BorderOptionsIndexAccessor : IIndexAccessor<FormatBase, BorderOptionsInfo, DocumentModelChangeActions> {
		#region IIndexAccessor Members
		public int GetIndex(FormatBase owner) {
			return ((DifferentialFormat)owner).BorderOptionsIndex;
		}
		public int GetDeferredInfoIndex(FormatBase owner) {
			return GetInfoIndex(owner, GetDeferredInfo(owner.BatchUpdateHelper));
		}
		public void SetIndex(FormatBase owner, int value) {
			((DifferentialFormat)owner).AssignBorderOptionsIndex(value);
		}
		public int GetInfoIndex(FormatBase owner, BorderOptionsInfo value) {
			return value.PackedValues;
		}
		public BorderOptionsInfo GetInfo(FormatBase owner) {
			return BorderOptionsInfo.Create(((DifferentialFormat)owner).BorderOptionsIndex);
		}
		public bool IsIndexValid(FormatBase owner, int index) {
			return true;
		}
		public IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateHistoryItem(FormatBase owner) {
			return new DifferentialFormatBorderOptionsIndexChangeHistoryItem(((DifferentialFormat)owner));
		}
		public BorderOptionsInfo GetDeferredInfo(MultiIndexBatchUpdateHelper helper) {
			return ((DifferentialFormatBatchUpdateHelper)helper).BorderOptionsInfo;
		}
		public void SetDeferredInfo(MultiIndexBatchUpdateHelper helper, BorderOptionsInfo info) {
			((DifferentialFormatBatchUpdateHelper)helper).BorderOptionsInfo = info.Clone();
		}
		public void InitializeDeferredInfo(FormatBase owner) {
			this.SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(owner));
		}
		public void CopyDeferredInfo(FormatBase owner, FormatBase from) {
			SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(from));
		}
		public bool ApplyDeferredChanges(FormatBase owner) {
			return owner.ReplaceInfo(this, GetDeferredInfo(owner.BatchUpdateHelper), owner.GetBatchUpdateChangeActions());
		}
		#endregion
	}
	#endregion
	#region NumberFormatIdInfoIndexAccessor
	public class NumberFormatIdInfoIndexAccessor : IIndexAccessor<FormatBase, NumberFormatIdInfo, DocumentModelChangeActions> {
		#region IIndexAccessor<FormatBase, NumberFormatIdInfo> Members
		public int GetIndex(FormatBase owner) {
			return ((DifferentialFormat)owner).NumberFormatIdInfoIndex;
		}
		public int GetDeferredInfoIndex(FormatBase owner) {
			return GetInfoIndex(owner, GetDeferredInfo(owner.BatchUpdateHelper));
		}
		public void SetIndex(FormatBase owner, int value) {
			((DifferentialFormat)owner).AssignNumberFormatIdInfoIndex(value);
		}
		public int GetInfoIndex(FormatBase owner, NumberFormatIdInfo value) {
			return value.Id;
		}
		public NumberFormatIdInfo GetInfo(FormatBase owner) {
			NumberFormatIdInfo info = new NumberFormatIdInfo();
			info.Id = GetIndex(owner);
			return info;
		}
		public bool IsIndexValid(FormatBase owner, int index) {
			return true;
		}
		public IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateHistoryItem(FormatBase owner) {
			return new DifferentialFormatNumberFormatIdInfoIndexChangeHistoryItem((DifferentialFormat)owner);
		}
		public NumberFormatIdInfo GetDeferredInfo(MultiIndexBatchUpdateHelper helper) {
			return ((DifferentialFormatBatchUpdateHelper)helper).NumberFormatIdInfo;
		}
		public void SetDeferredInfo(MultiIndexBatchUpdateHelper helper, NumberFormatIdInfo info) {
			((DifferentialFormatBatchUpdateHelper)helper).NumberFormatIdInfo = info.Clone();
		}
		public void InitializeDeferredInfo(FormatBase owner) {
			this.SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(owner));
		}
		public void CopyDeferredInfo(FormatBase owner, FormatBase from) {
			SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(from));
		}
		public bool ApplyDeferredChanges(FormatBase owner) {
			return owner.ReplaceInfo(this, GetDeferredInfo(owner.BatchUpdateHelper), owner.GetBatchUpdateChangeActions());
		}
		#endregion
	}
	#endregion
	#region DifferentialFormatBatchUpdateHelper
	public class DifferentialFormatBatchUpdateHelper: FormatBaseBatchUpdateHelper {
		MultiOptionsInfo multiOptionsInfo;
		BorderOptionsInfo borderOptionsInfo;
		NumberFormatIdInfo numberFormatIdInfo;
		public DifferentialFormatBatchUpdateHelper(IBatchUpdateHandler handler)
			: base(handler) {
		}
		public MultiOptionsInfo MultiOptionsInfo { get { return multiOptionsInfo; } set { multiOptionsInfo = value; } }
		public BorderOptionsInfo BorderOptionsInfo { get { return borderOptionsInfo; } set { borderOptionsInfo = value; } }
		public NumberFormatIdInfo NumberFormatIdInfo { get { return numberFormatIdInfo; } set { numberFormatIdInfo = value; } }
	}
	#endregion
	#region DifferentialFormatBatchInitHelper
	public class DifferentialFormatBatchInitHelper : DifferentialFormatBatchUpdateHelper {
		public DifferentialFormatBatchInitHelper(IBatchInitHandler handler)
			: base(new BatchInitAdapter(handler)) {
		}
		public IBatchInitHandler BatchInitHandler { get { return ((BatchInitAdapter)BatchUpdateHandler).BatchInitHandler; } }
	}
	#endregion      
	#region DifferentialFormat
	public class DifferentialFormat : FormatBase, IDifferentialFormat, IRunFontInfo, IFillInfo, IBorderInfo, ICellAlignmentInfo, ICellProtectionInfo, IGradientFillInfo, IConvergenceInfo {
		#region Static Members
		readonly static MultiOptionsIndexAccessor multiOptionsIndexAccessor = new MultiOptionsIndexAccessor();
		readonly static BorderOptionsIndexAccessor borderOptionsIndexAccessor = new BorderOptionsIndexAccessor();
		readonly static NumberFormatIdInfoIndexAccessor numberFormatIdInfoIndexAccessor = new NumberFormatIdInfoIndexAccessor();
		readonly static IIndexAccessorBase<FormatBase, DocumentModelChangeActions>[] indexAccessors = new IIndexAccessorBase<FormatBase, DocumentModelChangeActions>[] {
			FontIndexAccessor,
			NumberFormatIndexAccessor, 
			FillIndexAccessor,
			GradientFillIndexAccessor,
			AlignmentIndexAccessor,
			BorderIndexAccessor,
			CellFormatFlagsIndexAccessor, 
			multiOptionsIndexAccessor,
			borderOptionsIndexAccessor,
			numberFormatIdInfoIndexAccessor
		};
		public static MultiOptionsIndexAccessor MultiOptionsIndexAccessor { get { return multiOptionsIndexAccessor; } }
		public static BorderOptionsIndexAccessor BorderOptionsIndexAccessor { get { return borderOptionsIndexAccessor; } }
		public static NumberFormatIdInfoIndexAccessor NumberFormatIdInfoIndexAccessor { get { return numberFormatIdInfoIndexAccessor; } }
		#endregion
		#region Fields
		int multiOptionsIndex;
		int borderOptionsIndex;
		int numberFormatIdInfoIndex;
		#endregion
		public DifferentialFormat(DocumentModel documentModel)
			: base(documentModel) {
		}
		#region Properties
		internal int MultiOptionsIndex { get { return multiOptionsIndex; } }
		internal int BorderOptionsIndex { get { return borderOptionsIndex; } }
		internal int NumberFormatIdInfoIndex { get { return numberFormatIdInfoIndex; } }
		internal new DifferentialFormatBatchUpdateHelper BatchUpdateHelper { get { return (DifferentialFormatBatchUpdateHelper)base.BatchUpdateHelper; } }
		protected override IIndexAccessorBase<FormatBase, DocumentModelChangeActions>[] IndexAccessors { get { return indexAccessors; } }
		protected internal MultiOptionsInfo MultiOptionsInfo { get { return IsUpdateLocked ? BatchUpdateHelper.MultiOptionsInfo : MultiOptionsInfoCore; } }
		protected internal BorderOptionsInfo BorderOptionsInfo { get { return IsUpdateLocked ? BatchUpdateHelper.BorderOptionsInfo : BorderOptionsInfoCore; } }
		protected internal NumberFormatIdInfo NumberFormatIdInfo { get { return IsUpdateLocked ? BatchUpdateHelper.NumberFormatIdInfo : NumberFormatIdInfoCore; } }
		MultiOptionsInfo MultiOptionsInfoCore { get { return multiOptionsIndexAccessor.GetInfo(this); } }
		BorderOptionsInfo BorderOptionsInfoCore { get { return borderOptionsIndexAccessor.GetInfo(this); } }
		NumberFormatIdInfo NumberFormatIdInfoCore { get { return numberFormatIdInfoIndexAccessor.GetInfo(this); } }
		DifferentialFormat DefaultFormatInfo { get { return DocumentModel.Cache.CellFormatCache.DefaultDifferentialFormatItem as DifferentialFormat; } }
		protected override FillInfo DefaultFillInfo { get { return DefaultFormatInfo.FillInfo; } }
		protected override GradientFillInfo DefaultGradientFillInfo { get { return DefaultFormatInfo.GradientFillInfo; } }
		#region Font
		#region Font.Name
		string IRunFontInfo.Name {
			get { return FontInfo.Name; }
			set {
				if (FontInfo.Name == value && MultiOptionsInfo.ApplyFontName)
					return;
				SetPropertyValue(FontIndexAccessor, SetFontName, value, multiOptionsIndexAccessor, SetApplyFontName);
			}
		}
		void SetApplyFontName(ref MultiOptionsInfo info, bool value) {
			info.ApplyFontName = value;
		}
		#endregion
		#region Font.Color
		Color IRunFontInfo.Color {
			get { return GetColor(FontInfo.ColorIndex); }
			set {
				if (GetColor(FontInfo.ColorIndex) == value && MultiOptionsInfo.ApplyFontColor)
					return;
				SetPropertyValue(FontIndexAccessor, SetFontColor, value, multiOptionsIndexAccessor, SetApplyFontColor);
			}
		}
		void SetApplyFontColor(ref MultiOptionsInfo info, bool value) {
			info.ApplyFontColor = value;
		}
		#endregion
		#region Font.Bold
		bool IRunFontInfo.Bold {
			get { return FontInfo.Bold; }
			set {
				if (FontInfo.Bold == value && MultiOptionsInfo.ApplyFontBold)
					return;
				SetPropertyValue(FontIndexAccessor, SetFontBold, value, multiOptionsIndexAccessor, SetApplyFontBold);
			}
		}
		void SetApplyFontBold(ref MultiOptionsInfo info, bool value) {
			info.ApplyFontBold = value;
		}
		#endregion
		#region Font.Condense
		bool IRunFontInfo.Condense {
			get { return FontInfo.Condense; }
			set {
				if (FontInfo.Condense == value && MultiOptionsInfo.ApplyFontCondense)
					return;
				SetPropertyValue(FontIndexAccessor, SetFontCondense, value, multiOptionsIndexAccessor, SetApplyFontCondense);
			}
		}
		void SetApplyFontCondense(ref MultiOptionsInfo info, bool value) {
			info.ApplyFontCondense = value;
		}
		#endregion
		#region Font.Extend
		bool IRunFontInfo.Extend {
			get { return FontInfo.Extend; }
			set {
				if (FontInfo.Extend == value && MultiOptionsInfo.ApplyFontExtend)
					return;
				SetPropertyValue(FontIndexAccessor, SetFontExtend, value, multiOptionsIndexAccessor, SetApplyFontExtend);
			}
		}
		void SetApplyFontExtend(ref MultiOptionsInfo info, bool value) {
			info.ApplyFontExtend = value;
		}
		#endregion
		#region Font.Italic
		bool IRunFontInfo.Italic {
			get { return FontInfo.Italic; }
			set {
				if (FontInfo.Italic == value && MultiOptionsInfo.ApplyFontItalic)
					return;
				SetPropertyValue(FontIndexAccessor, SetFontItalic, value, multiOptionsIndexAccessor, SetApplyFontItalic);
			}
		}
		void SetApplyFontItalic(ref MultiOptionsInfo info, bool value) {
			info.ApplyFontItalic = value;
		}
		#endregion
		#region Font.Outline
		bool IRunFontInfo.Outline {
			get { return FontInfo.Outline; }
			set {
				if (FontInfo.Outline == value && MultiOptionsInfo.ApplyFontOutline)
					return;
				SetPropertyValue(FontIndexAccessor, SetFontOutline, value, multiOptionsIndexAccessor, SetApplyFontOutline);
			}
		}
		void SetApplyFontOutline(ref MultiOptionsInfo info, bool value) {
			info.ApplyFontOutline = value;
		}
		#endregion
		#region Font.Shadow
		bool IRunFontInfo.Shadow {
			get { return FontInfo.Shadow; }
			set {
				if (FontInfo.Shadow == value && MultiOptionsInfo.ApplyFontShadow)
					return;
				SetPropertyValue(FontIndexAccessor, SetFontShadow, value, multiOptionsIndexAccessor, SetApplyFontShadow);
			}
		}
		void SetApplyFontShadow(ref MultiOptionsInfo info, bool value) {
			info.ApplyFontShadow = value;
		}
		#endregion
		#region Font.StrikeThrough
		bool IRunFontInfo.StrikeThrough {
			get { return FontInfo.StrikeThrough; }
			set {
				if (FontInfo.StrikeThrough == value && MultiOptionsInfo.ApplyFontStrikeThrough)
					return;
				SetPropertyValue(FontIndexAccessor, SetFontStrikeThrough, value, multiOptionsIndexAccessor, SetApplyFontStrikeThrough);
			}
		}
		void SetApplyFontStrikeThrough(ref MultiOptionsInfo info, bool value) {
			info.ApplyFontStrikeThrough = value;
		}
		#endregion
		#region Font.Charset
		int IRunFontInfo.Charset {
			get { return FontInfo.Charset; }
			set {
				if (FontInfo.Charset == value && MultiOptionsInfo.ApplyFontCharset)
					return;
				SetPropertyValue(FontIndexAccessor, SetFontCharset, value, multiOptionsIndexAccessor, SetApplyFontCharset);
			}
		}
		void SetApplyFontCharset(ref MultiOptionsInfo info, bool value) {
			info.ApplyFontCharset = value;
		}
		#endregion
		#region Font.FontFamily
		int IRunFontInfo.FontFamily {
			get { return FontInfo.FontFamily; }
			set {
				if (FontInfo.FontFamily == value && MultiOptionsInfo.ApplyFontFamily)
					return;
				SetPropertyValue(FontIndexAccessor, SetFontFontFamily, value, multiOptionsIndexAccessor, SetApplyFontFamily);
			}
		}
		void SetApplyFontFamily(ref MultiOptionsInfo info, bool value) {
			info.ApplyFontFamily = value;
		}
		#endregion
		#region Font.Size
		double IRunFontInfo.Size {
			get { return FontInfo.Size; }
			set {
				if (FontInfo.Size == value && MultiOptionsInfo.ApplyFontSize)
					return;
				SetPropertyValue(FontIndexAccessor, SetFontSize, value, multiOptionsIndexAccessor, SetApplyFontSize);
			}
		}
		void SetApplyFontSize(ref MultiOptionsInfo info, bool value) {
			info.ApplyFontSize = value;
		}
		#endregion
		#region Font.FontSchemeStyles
		XlFontSchemeStyles IRunFontInfo.SchemeStyle {
			get { return FontInfo.SchemeStyle; }
			set {
				if (FontInfo.SchemeStyle == value && MultiOptionsInfo.ApplyFontSchemeStyle)
					return;
				SetPropertyValue(FontIndexAccessor, SetFontSchemeStyle, value, multiOptionsIndexAccessor, SetApplyFontSchemeStyle);
			}
		}
		void SetApplyFontSchemeStyle(ref MultiOptionsInfo info, bool value) {
			info.ApplyFontSchemeStyle = value;
		}
		#endregion
		#region Font.FontScripts
		XlScriptType IRunFontInfo.Script {
			get { return FontInfo.Script; }
			set {
				if (FontInfo.Script == value && MultiOptionsInfo.ApplyFontScript)
					return;
				SetPropertyValue(FontIndexAccessor, SetFontScript, value, multiOptionsIndexAccessor, SetApplyFontScript);
			}
		}
		void SetApplyFontScript(ref MultiOptionsInfo info, bool value) {
			info.ApplyFontScript = value;
		}
		#endregion
		#region Font.FontUnderline
		XlUnderlineType IRunFontInfo.Underline {
			get { return FontInfo.Underline; }
			set {
				if (FontInfo.Underline == value && MultiOptionsInfo.ApplyFontUnderline)
					return;
				SetPropertyValue(FontIndexAccessor, SetFontUnderline, value, multiOptionsIndexAccessor, SetApplyFontUnderline);
			}
		}
		void SetApplyFontUnderline(ref MultiOptionsInfo info, bool value) {
			info.ApplyFontUnderline = value;
		}
		#endregion
		#endregion
		#region NumberFormatId
		public int NumberFormatId {
			get { return NumberFormatIdInfo.Id; }
			set {
				if (NumberFormatIdInfo.Id == value && MultiOptionsInfo.ApplyNumberFormat)
					return;
				SetNumberFormatIdValue(value);
			}
		}
		DocumentModelChangeActions SetNumberFormatId(ref NumberFormatIdInfo info, int value) {
			info.Id = value;
			return DocumentModelChangeActions.None; 
		}
		void SetNumberFormatIdValue(int newInfoValue) {
			DocumentModel.BeginUpdate();
			try {
				SetPropertyValueForStruct(numberFormatIdInfoIndexAccessor, SetNumberFormatId, newInfoValue);
				SetPropertyValueForOptionsInfo(multiOptionsIndexAccessor, SetApplyNumberFormat, true);
			} finally {
				DocumentModel.EndUpdate();
			}
		}
		#endregion
		#region FormatString
		protected override void SetFormatString(string value) {
			if (NumberFormatInfo.FormatCode == value && MultiOptionsInfo.ApplyNumberFormat)
				return;
			SetFormatStringCore(value);
		}
		void SetApplyNumberFormat(ref MultiOptionsInfo info, bool value) {
			info.ApplyNumberFormat = value;
		}
		void SetFormatStringCore(string value) {
			DocumentModel.BeginUpdate();
			try {
				SetPropertyValueCore(NumberFormatIndexAccessor, SetFormatString, value);
				SetPropertyValueForOptionsInfo(multiOptionsIndexAccessor, SetApplyNumberFormat, true);
				SetPropertyValueForStruct(numberFormatIdInfoIndexAccessor, SetNumberFormatId, NumberFormatIndexAccessor.GetInfoIndex(this, NumberFormatInfo));
			} finally {
				DocumentModel.EndUpdate();
			}
		}
		#endregion
		#region Fill
		#region Fill.PatternType
		XlPatternType IFillInfo.PatternType {
			get { return FillInfo.PatternType; }
			set {
				if (FillInfo.PatternType == value && MultiOptionsInfo.ApplyFillPatternType)
					return;
				SetPropertyValue(FillIndexAccessor, SetFillPatternType, value, multiOptionsIndexAccessor, SetApplyFillPatternType);
			}
		}
		void SetApplyFillPatternType(ref MultiOptionsInfo info, bool value) {
			info.ApplyFillPatternType = value;
		}
		#endregion
		#region Fill.ForeColor
		Color IFillInfo.ForeColor {
			get { return GetColor(FillInfo.ForeColorIndex); }
			set {
				if (GetColor(FillInfo.ForeColorIndex) == value && MultiOptionsInfo.ApplyFillForeColor)
					return;
				SetPropertyValue(FillIndexAccessor, SetFillForeColor, value, multiOptionsIndexAccessor, SetApplyFillForeColor);
			}
		}
		void SetApplyFillForeColor(ref MultiOptionsInfo info, bool value) {
			info.ApplyFillForeColor = value;
		}
		#endregion
		#region Fill.BackColor
		Color IFillInfo.BackColor {
			get { return GetColor(FillInfo.BackColorIndex); }
			set {
				if (GetColor(FillInfo.BackColorIndex) == value && MultiOptionsInfo.ApplyFillBackColor)
					return;
				SetPropertyValue(FillIndexAccessor, SetFillBackColor, value, multiOptionsIndexAccessor, SetApplyFillBackColor);
			}
		}
		void SetApplyFillBackColor(ref MultiOptionsInfo info, bool value) {
			info.ApplyFillBackColor = value;
		}
		#endregion
		#region Fill.GradientFill
		IGradientFillInfo IFillInfo.GradientFill { get { return this; } }
		#endregion
		#region Fill.FillType
		ModelFillType IFillInfo.FillType {
			get { return CellFormatFlagsInfo.FillType; }
			set {
				if (CellFormatFlagsInfo.FillType == value)
					return;
				SetPropertyValueForStruct(CellFormatFlagsIndexAccessor, SetModelFillType, value);
			}
		}
		#endregion
		#region GradientFill
		#region GradientFill.Convergence
		IConvergenceInfo IGradientFillInfo.Convergence { get { return this; } }
		#endregion
		#region GradientFill.GradientStops
		IGradientStopCollection IGradientFillInfo.GradientStops { get { return GradientStopInfoCollection; } }
		#endregion
		#region IGradientFillInfo.Type
		ModelGradientFillType IGradientFillInfo.Type {
			get { return GradientFillInfo.Type; }
			set {
				if (GradientFillInfo.Type == value)
					return;
				SetPropertyValue(GradientFillIndexAccessor, SetGradientFillInfoType, value);
			}
		}
		#endregion
		#region IGradientFillInfo.Degree
		double IGradientFillInfo.Degree {
			get { return GradientFillInfo.Degree; }
			set {
				if (GradientFillInfo.Degree == value)
					return;
				SetPropertyValue(GradientFillIndexAccessor, SetGradientFillInfoDegree, value);
			}
		}
		#endregion
		#region IConvergenceInfo Members
		#region IConvergenceInfo.Left
		float IConvergenceInfo.Left {
			get { return GradientFillInfo.Left; }
			set {
				if (GradientFillInfo.Left == value)
					return;
				SetPropertyValue(GradientFillIndexAccessor, SetGradientFillInfoLeft, value);
			}
		}
		#endregion
		#region IConvergenceInfo.Right
		float IConvergenceInfo.Right {
			get { return GradientFillInfo.Right; }
			set {
				if (GradientFillInfo.Right == value)
					return;
				SetPropertyValue(GradientFillIndexAccessor, SetGradientFillInfoRight, value);
			}
		}
		#endregion
		#region IConvergenceInfo.Top
		float IConvergenceInfo.Top {
			get { return GradientFillInfo.Top; }
			set {
				if (GradientFillInfo.Top == value)
					return;
				SetPropertyValue(GradientFillIndexAccessor, SetGradientFillInfoTop, value);
			}
		}
		#endregion
		#region IConvergenceInfo.Bottom
		float IConvergenceInfo.Bottom {
			get { return GradientFillInfo.Bottom; }
			set {
				if (GradientFillInfo.Bottom == value)
					return;
				SetPropertyValue(GradientFillIndexAccessor, SetGradientFillInfoBottom, value);
			}
		}
		#endregion
		#endregion
		#endregion
		#endregion       
		#region Alignment
		#region Alignment.Horizontal
		XlHorizontalAlignment ICellAlignmentInfo.Horizontal {
			get { return AlignmentInfo.HorizontalAlignment; }
			set {
				if (AlignmentInfo.HorizontalAlignment == value && MultiOptionsInfo.ApplyAlignmentHorizontal)
					return;
				SetPropertyValue(AlignmentIndexAccessor, SetAlignmentHorizontal, value, multiOptionsIndexAccessor, SetApplyAlignmentHorizontal);
			}
		}
		void SetApplyAlignmentHorizontal(ref MultiOptionsInfo info, bool value) {
			info.ApplyAlignmentHorizontal = value;
		}
		#endregion
		#region Alignment.Vertical
		XlVerticalAlignment ICellAlignmentInfo.Vertical {
			get { return AlignmentInfo.VerticalAlignment; }
			set {
				if (AlignmentInfo.VerticalAlignment == value && MultiOptionsInfo.ApplyAlignmentVertical)
					return;
				SetPropertyValue(AlignmentIndexAccessor, SetAlignmentVertical, value, multiOptionsIndexAccessor, SetApplyAlignmentVertical);
			}
		}
		void SetApplyAlignmentVertical(ref MultiOptionsInfo info, bool value) {
			info.ApplyAlignmentVertical = value;
		}
		#endregion
		#region Alignment.WrapText
		bool ICellAlignmentInfo.WrapText {
			get { return AlignmentInfo.WrapText; }
			set {
				if (AlignmentInfo.WrapText == value && MultiOptionsInfo.ApplyAlignmentWrapText)
					return;
				SetPropertyValue(AlignmentIndexAccessor, SetAlignmentWrapText, value, multiOptionsIndexAccessor, SetApplyAlignmentWrapText);
			}
		}
		void SetApplyAlignmentWrapText(ref MultiOptionsInfo info, bool value) {
			info.ApplyAlignmentWrapText = value;
		}
		#endregion
		#region Alignment.JustifyLastLine
		bool ICellAlignmentInfo.JustifyLastLine {
			get { return AlignmentInfo.JustifyLastLine; }
			set {
				if (AlignmentInfo.JustifyLastLine == value && MultiOptionsInfo.ApplyAlignmentJustifyLastLine)
					return;
				SetPropertyValue(AlignmentIndexAccessor, SetAlignmentJustifyLastLine, value, multiOptionsIndexAccessor, SetApplyAlignmentJustifyLastLine);
			}
		}
		void SetApplyAlignmentJustifyLastLine(ref MultiOptionsInfo info, bool value) {
			info.ApplyAlignmentJustifyLastLine = value;
		}
		#endregion
		#region Alignment.ShrinkToFit
		bool ICellAlignmentInfo.ShrinkToFit {
			get { return AlignmentInfo.ShrinkToFit; }
			set {
				if (AlignmentInfo.ShrinkToFit == value && MultiOptionsInfo.ApplyAlignmentShrinkToFit)
					return;
				SetPropertyValue(AlignmentIndexAccessor, SetAlignmentShrinkToFit, value, multiOptionsIndexAccessor, SetApplyAlignmentShrinkToFit);
			}
		}
		void SetApplyAlignmentShrinkToFit(ref MultiOptionsInfo info, bool value) {
			info.ApplyAlignmentShrinkToFit = value;
		}
		#endregion
		#region Alignment.TextRotation
		int ICellAlignmentInfo.TextRotation {
			get { return AlignmentInfo.TextRotation; }
			set {
				if (AlignmentInfo.TextRotation == value && MultiOptionsInfo.ApplyAlignmentTextRotation)
					return;
				SetPropertyValue(AlignmentIndexAccessor, SetAlignmentTextRotation, value, multiOptionsIndexAccessor, SetApplyAlignmentTextRotation);
			}
		}
		void SetApplyAlignmentTextRotation(ref MultiOptionsInfo info, bool value) {
			info.ApplyAlignmentTextRotation = value;
		}
		#endregion
		#region Alignment.Indent
		byte ICellAlignmentInfo.Indent {
			get { return AlignmentInfo.Indent; }
			set {
				if (AlignmentInfo.Indent == value && MultiOptionsInfo.ApplyAlignmentIndent)
					return;
				SetPropertyValue(AlignmentIndexAccessor, SetAlignmentIndent, value, multiOptionsIndexAccessor, SetApplyAlignmentIndent);
			}
		}
		void SetApplyAlignmentIndent(ref MultiOptionsInfo info, bool value) {
			info.ApplyAlignmentIndent = value;
		}
		#endregion
		#region Alignment.RelativeIndent
		int ICellAlignmentInfo.RelativeIndent {
			get { return AlignmentInfo.RelativeIndent; }
			set {
				if (AlignmentInfo.RelativeIndent == value && MultiOptionsInfo.ApplyAlignmentRelativeIndent)
					return;
				SetPropertyValue(AlignmentIndexAccessor, SetAlignmentRelativeIndent, value, multiOptionsIndexAccessor, SetApplyAlignmentRelativeIndent);
			}
		}
		void SetApplyAlignmentRelativeIndent(ref MultiOptionsInfo info, bool value) {
			info.ApplyAlignmentRelativeIndent = value;
		}
		#endregion
		#region Alignment.XlReadingOrder
		XlReadingOrder ICellAlignmentInfo.ReadingOrder {
			get { return AlignmentInfo.ReadingOrder; }
			set {
				if (AlignmentInfo.ReadingOrder == value && MultiOptionsInfo.ApplyAlignmentReadingOrder)
					return;
				SetPropertyValue(AlignmentIndexAccessor, SetAlignmentReadingOrder, value, multiOptionsIndexAccessor, SetApplyAlignmentReadingOrder);
			}
		}
		void SetApplyAlignmentReadingOrder(ref MultiOptionsInfo info, bool value) {
			info.ApplyAlignmentReadingOrder = value;
		}
		#endregion
		#endregion
		#region Borders
		#region Borders.LeftLineStyle
		XlBorderLineStyle IBorderInfo.LeftLineStyle {
			get { return BorderInfo.LeftLineStyle; }
			set {
				if (BorderInfo.LeftLineStyle == value && BorderOptionsInfo.ApplyLeftLineStyle)
					return;
				SetPropertyValue(BorderIndexAccessor, SetBorderLeftLineStyle, value, borderOptionsIndexAccessor, SetApplyBorderLeftLineStyle);
			}
		}
		void SetApplyBorderLeftLineStyle(ref BorderOptionsInfo info, bool value) {
			info.ApplyLeftLineStyle = value;
		}
		#endregion
		#region Borders.RightLineStyle
		XlBorderLineStyle IBorderInfo.RightLineStyle {
			get { return BorderInfo.RightLineStyle; }
			set {
				if (BorderInfo.RightLineStyle == value && BorderOptionsInfo.ApplyRightLineStyle)
					return;
				SetPropertyValue(BorderIndexAccessor, SetBorderRightLineStyle, value, borderOptionsIndexAccessor, SetApplyBorderRightLineStyle);
			}
		}
		void SetApplyBorderRightLineStyle(ref BorderOptionsInfo info, bool value) {
			info.ApplyRightLineStyle = value;
		}
		#endregion
		#region Borders.TopLineStyle
		XlBorderLineStyle IBorderInfo.TopLineStyle {
			get { return BorderInfo.TopLineStyle; }
			set {
				if (BorderInfo.TopLineStyle == value && BorderOptionsInfo.ApplyTopLineStyle)
					return;
				SetPropertyValue(BorderIndexAccessor, SetBorderTopLineStyle, value, borderOptionsIndexAccessor, SetApplyBorderTopLineStyle);
			}
		}
		void SetApplyBorderTopLineStyle(ref BorderOptionsInfo info, bool value) {
			info.ApplyTopLineStyle = value;
		}
		#endregion
		#region Borders.BottomLineStyle
		XlBorderLineStyle IBorderInfo.BottomLineStyle {
			get { return BorderInfo.BottomLineStyle; }
			set {
				if (BorderInfo.BottomLineStyle == value && BorderOptionsInfo.ApplyBottomLineStyle)
					return;
				SetPropertyValue(BorderIndexAccessor, SetBorderBottomLineStyle, value, borderOptionsIndexAccessor, SetApplyBorderBottomLineStyle);
			}
		}
		void SetApplyBorderBottomLineStyle(ref BorderOptionsInfo info, bool value) {
			info.ApplyBottomLineStyle = value;
		}
		#endregion
		#region Borders.DiagonalUpLineStyle
		XlBorderLineStyle IBorderInfo.DiagonalUpLineStyle {
			get { return GetBorderDiagonalLineStyle(BorderInfo.DiagonalUp); }
			set {
				if (GetBorderDiagonalLineStyle(BorderInfo.DiagonalUp) == value && BorderOptionsInfo.ApplyDiagonalLineStyle)
					return;
				SetPropertyValue(BorderIndexAccessor, SetBorderDiagonalUpLineStyle, value, borderOptionsIndexAccessor, SetApplyBorderDiagonalUpLineStyle);
			}
		}
		void SetApplyBorderDiagonalUpLineStyle(ref BorderOptionsInfo info, bool value) {
			info.ApplyDiagonalLineStyle = true;
			info.ApplyDiagonalUp = true;
		}
		#endregion
		#region Borders.DiagonalDownLineStyle
		XlBorderLineStyle IBorderInfo.DiagonalDownLineStyle {
			get { return GetBorderDiagonalLineStyle(BorderInfo.DiagonalDown); }
			set {
				if (GetBorderDiagonalLineStyle(BorderInfo.DiagonalDown) == value && BorderOptionsInfo.ApplyDiagonalLineStyle)
					return;
				SetPropertyValue(BorderIndexAccessor, SetBorderDiagonalDownLineStyle, value, borderOptionsIndexAccessor, SetApplyBorderDiagonalDownLineStyle);
			}
		}
		void SetApplyBorderDiagonalDownLineStyle(ref BorderOptionsInfo info, bool value) {
			info.ApplyDiagonalLineStyle = true;
			info.ApplyDiagonalDown = true;
		}
		#endregion
		#region Borders.HorizontalLineStyle
		XlBorderLineStyle IBorderInfo.HorizontalLineStyle {
			get { return BorderInfo.HorizontalLineStyle; }
			set {
				if (BorderInfo.HorizontalLineStyle == value && BorderOptionsInfo.ApplyHorizontalLineStyle)
					return;
				SetPropertyValue(BorderIndexAccessor, SetBorderHorizontalLineStyle, value, borderOptionsIndexAccessor, SetApplyBorderHorizontalLineStyle);
			}
		}
		void SetApplyBorderHorizontalLineStyle(ref BorderOptionsInfo info, bool value) {
			info.ApplyHorizontalLineStyle = value;
		}
		#endregion
		#region Borders.VerticalLineStyle
		XlBorderLineStyle IBorderInfo.VerticalLineStyle {
			get { return BorderInfo.VerticalLineStyle; }
			set {
				if (BorderInfo.VerticalLineStyle == value && BorderOptionsInfo.ApplyVerticalLineStyle)
					return;
				SetPropertyValue(BorderIndexAccessor, SetBorderVerticalLineStyle, value, borderOptionsIndexAccessor, SetApplyBorderVerticalLineStyle);
			}
		}
		void SetApplyBorderVerticalLineStyle(ref BorderOptionsInfo info, bool value) {
			info.ApplyVerticalLineStyle = value;
		}
		#endregion
		#region Borders.Outline
		bool IBorderInfo.Outline {
			get { return BorderInfo.Outline; }
			set {
				if (BorderInfo.Outline == value && BorderOptionsInfo.ApplyOutline)
					return;
				SetPropertyValue(BorderIndexAccessor, SetBorderOutline, value, borderOptionsIndexAccessor, SetApplyBorderOutline);
			}
		}
		void SetApplyBorderOutline(ref BorderOptionsInfo info, bool value) {
			info.ApplyOutline = value;
		}
		#endregion
		#region Borders.LeftColor
		Color IBorderInfo.LeftColor {
			get { return GetColor(BorderInfo.LeftColorIndex); }
			set {
				if (GetColor(BorderInfo.LeftColorIndex) == value && BorderOptionsInfo.ApplyLeftColor)
					return;
				SetPropertyValue(BorderIndexAccessor, SetBorderLeftColorCore, value, borderOptionsIndexAccessor, SetApplyBorderLeftColor);
			}
		}
		void SetApplyBorderLeftColor(ref BorderOptionsInfo info, bool value) {
			info.ApplyLeftColor = value;
		}
		#endregion
		#region Borders.RightColor
		Color IBorderInfo.RightColor {
			get { return GetColor(BorderInfo.RightColorIndex); }
			set {
				if (GetColor(BorderInfo.RightColorIndex) == value && BorderOptionsInfo.ApplyRightColor)
					return;
				SetPropertyValue(BorderIndexAccessor, SetBorderRightColorCore, value, borderOptionsIndexAccessor, SetApplyBorderRightColor);
			}
		}
		void SetApplyBorderRightColor(ref BorderOptionsInfo info, bool value) {
			info.ApplyRightColor = value;
		}
		#endregion
		#region Borders.TopColor
		Color IBorderInfo.TopColor {
			get { return GetColor(BorderInfo.TopColorIndex); }
			set {
				if (GetColor(BorderInfo.TopColorIndex) == value && BorderOptionsInfo.ApplyTopColor)
					return;
				SetPropertyValue(BorderIndexAccessor, SetBorderTopColorCore, value, borderOptionsIndexAccessor, SetApplyBorderTopColor);
			}
		}
		void SetApplyBorderTopColor(ref BorderOptionsInfo info, bool value) {
			info.ApplyTopColor = value;
		}
		#endregion
		#region Borders.BottomColor
		Color IBorderInfo.BottomColor {
			get { return GetColor(BorderInfo.BottomColorIndex); }
			set {
				if (GetColor(BorderInfo.BottomColorIndex) == value && BorderOptionsInfo.ApplyBottomColor)
					return;
				SetPropertyValue(BorderIndexAccessor, SetBorderBottomColorCore, value, borderOptionsIndexAccessor, SetApplyBorderBottomColor);
			}
		}
		void SetApplyBorderBottomColor(ref BorderOptionsInfo info, bool value) {
			info.ApplyBottomColor = value;
		}
		#endregion
		#region Borders.DiagonalColor
		Color IBorderInfo.DiagonalColor {
			get { return GetColor(BorderInfo.DiagonalColorIndex); }
			set {
				if (GetColor(BorderInfo.DiagonalColorIndex) == value && BorderOptionsInfo.ApplyDiagonalColor)
					return;
				SetPropertyValue(BorderIndexAccessor, SetBorderDiagonalColorCore, value, borderOptionsIndexAccessor, SetApplyBorderDiagonalColor);
			}
		}
		void SetApplyBorderDiagonalColor(ref BorderOptionsInfo info, bool value) {
			info.ApplyDiagonalColor = value;
		}
		#endregion
		#region Borders.HorizontalColor
		Color IBorderInfo.HorizontalColor {
			get { return GetColor(BorderInfo.HorizontalColorIndex); }
			set {
				if (GetColor(BorderInfo.HorizontalColorIndex) == value && BorderOptionsInfo.ApplyHorizontalColor)
					return;
				SetPropertyValue(BorderIndexAccessor, SetBorderHorizontalColorCore, value, borderOptionsIndexAccessor, SetApplyBorderHorizontalColor);
			}
		}
		void SetApplyBorderHorizontalColor(ref BorderOptionsInfo info, bool value) {
			info.ApplyHorizontalColor = value;
		}
		#endregion
		#region Borders.VerticalColor
		Color IBorderInfo.VerticalColor {
			get { return GetColor(BorderInfo.VerticalColorIndex); }
			set {
				if (GetColor(BorderInfo.VerticalColorIndex) == value && BorderOptionsInfo.ApplyVerticalColor)
					return;
				SetPropertyValue(BorderIndexAccessor, SetBorderVerticalColorCore, value, borderOptionsIndexAccessor, SetApplyBorderVerticalColor);
			}
		}
		void SetApplyBorderVerticalColor(ref BorderOptionsInfo info, bool value) {
			info.ApplyVerticalColor = value;
		}
		#endregion
		#endregion
		#region Protection
		#region Protection.Locked
		bool ICellProtectionInfo.Locked {
			get { return CellFormatFlagsInfo.Locked; }
			set {
				if (CellFormatFlagsInfo.Locked == value && MultiOptionsInfo.ApplyProtectionLocked)
					return;
				BeginUpdate();
				SetPropertyValueForStruct(CellFormatFlagsIndexAccessor, SetProtectionLocked, value);
				SetPropertyValueForStruct(multiOptionsIndexAccessor, SetApplyProtectionLocked, true);
				EndUpdate();
			}
		}
		DocumentModelChangeActions SetApplyProtectionLocked(ref MultiOptionsInfo info, bool value) {
			info.ApplyProtectionLocked = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Protection.Hidden
		bool ICellProtectionInfo.Hidden {
			get { return CellFormatFlagsInfo.Hidden; }
			set {
				if (CellFormatFlagsInfo.Hidden == value && MultiOptionsInfo.ApplyProtectionHidden)
					return;
				BeginUpdate();
				SetPropertyValueForStruct(CellFormatFlagsIndexAccessor, SetProtectionHidden, value);
				SetPropertyValueForStruct(multiOptionsIndexAccessor, SetApplyProtectionHidden, true);
				EndUpdate();
			}
		}
		DocumentModelChangeActions SetApplyProtectionHidden(ref MultiOptionsInfo info, bool value) {
			info.ApplyProtectionHidden = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#endregion
		#endregion
		#region Index Management
		internal void AssignMultiOptionsIndex(int value) {
			multiOptionsIndex = value;
		}
		internal void AssignBorderOptionsIndex(int value) {
			borderOptionsIndex = value;
		}
		internal void AssignNumberFormatIdInfoIndex(int value) {
			numberFormatIdInfoIndex = value;
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045")]
		delegate void SetOptionsValueDelegate<TOptions>(ref TOptions options, bool newOptionsValue);
		void SetPropertyValue<TInfo, U, TOptions>(IIndexAccessor<FormatBase, TInfo, DocumentModelChangeActions> infoIndexHolder, MultiIndexObject<FormatBase, DocumentModelChangeActions>.SetPropertyValueDelegate<TInfo, U> infoSetter, U newInfoValue, IIndexAccessor<FormatBase, TOptions, DocumentModelChangeActions> optionsIndexHolder, SetOptionsValueDelegate<TOptions> optionsSetter)
			where TInfo : ICloneable<TInfo>, ISupportsSizeOf
			where TOptions : ICloneable<TOptions>, ISupportsSizeOf {
			DocumentModel.BeginUpdate();
			try {
				SetPropertyValueCore(infoIndexHolder, infoSetter, newInfoValue);
				SetPropertyValueForOptionsInfo(optionsIndexHolder, optionsSetter, true);
			} finally {
				DocumentModel.EndUpdate();
			}
		}
		void SetPropertyValueForOptionsInfo<TInfo>(IIndexAccessor<FormatBase, TInfo, DocumentModelChangeActions> indexHolder, SetOptionsValueDelegate<TInfo> setter, bool newValue)
			where TInfo : ICloneable<TInfo>, ISupportsSizeOf {
			TInfo info = GetInfoForModification(indexHolder);
			setter(ref info, newValue);
			ReplaceInfoForFlags(indexHolder, info, DocumentModelChangeActions.None);
		}
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			DifferentialFormat other = obj as DifferentialFormat;
			if (other == null)
				return false;
			return base.Equals(obj);
		}
		public override int GetHashCode() {
			return base.GetHashCode() ^ GetType().GetHashCode();
		}
		#endregion
		#region  MultiIndexObject Members
		public override FormatBase GetOwner() {
			return this;
		}
		protected override MultiIndexBatchUpdateHelper CreateBatchUpdateHelper() {
			return new DifferentialFormatBatchUpdateHelper(this);
		}
		protected override MultiIndexBatchUpdateHelper CreateBatchInitHelper() {
			return new DifferentialFormatBatchInitHelper(this);
		}
		#endregion
		public override FormatBase CreateEmptyClone(IDocumentModel documentModel) {
			return new DifferentialFormat((DocumentModel)documentModel);
		}
		public new DifferentialFormat Clone() {
			return (DifferentialFormat)base.Clone();
		}
		internal override void CopySimple(FormatBase item) {
			base.CopySimple(item);
			DifferentialFormat differentialFormatItem = (DifferentialFormat)item;
			this.multiOptionsIndex = differentialFormatItem.multiOptionsIndex;
			this.borderOptionsIndex = differentialFormatItem.borderOptionsIndex;
			this.numberFormatIdInfoIndex = differentialFormatItem.numberFormatIdInfoIndex;
		}
		#region ClearFormatting
		internal void RemoveBorders() {
			DocumentModelChangeActions changeActions = DocumentModelChangeActions.None;
			DocumentModel.BeginUpdate();
			try {
				ReplaceInfo(BorderIndexAccessor, DefaultFormatInfo.BorderInfo, changeActions);
				ReplaceInfoForFlags(BorderOptionsIndexAccessor, DefaultFormatInfo.BorderOptionsInfo, changeActions);
			} finally {
				DocumentModel.EndUpdate();
			}
		}
		internal void ClearFont() {
			DocumentModelChangeActions changeActions = DocumentModelChangeActions.None;
			DocumentModel.BeginUpdate();
			try {
				ReplaceInfo(FontIndexAccessor, DefaultFormatInfo.FontInfo, changeActions);
				MultiOptionsInfo options = MultiOptionsInfo.Clone();
				options.ClearFontOptions();
				ReplaceInfoForFlags(MultiOptionsIndexAccessor, options, changeActions);
			} finally {
				DocumentModel.EndUpdate();
			}
		}
		protected override bool CheckClearFill() {
			return !MultiOptionsInfo.ApplyFillNone;
		}
		protected override void ClearFillOptions() {
			MultiOptionsInfo options = MultiOptionsInfo.Clone();
			options.ClearFillOptions();
			ReplaceInfoForFlags(MultiOptionsIndexAccessor, options, DocumentModelChangeActions.None);
		}
		#endregion
		public bool HasVisibleFill {
			get {
				if (CellFormatFlagsInfo.FillType == ModelFillType.Gradient)
					return true;
				return FillInfo.HasVisible(DocumentModel, true);
			}
		}
	}
	#endregion
	#region IDifferentialFormatPropertyChanger
	public interface IDifferentialFormatPropertyChanger {
		#region FormatString
		string GetFormatString(int elementIndex);
		void SetFormatString(int elementIndex, string value);
		#endregion
		#region Font
		string GetFontName(int elementIndex);
		void SetFontName(int elementIndex, string value);
		double GetFontSize(int elementIndex);
		void SetFontSize(int elementIndex, double value);
		bool GetFontBold(int elementIndex);
		void SetFontBold(int elementIndex, bool value);
		bool GetFontItalic(int elementIndex);
		void SetFontItalic(int elementIndex, bool value);
		bool GetFontStrikeThrough(int elementIndex);
		void SetFontStrikeThrough(int elementIndex, bool value);
		bool GetFontCondense(int elementIndex);
		void SetFontCondense(int elementIndex, bool value);
		bool GetFontExtend(int elementIndex);
		void SetFontExtend(int elementIndex, bool value);
		bool GetFontOutline(int elementIndex);
		void SetFontOutline(int elementIndex, bool value);
		bool GetFontShadow(int elementIndex);
		void SetFontShadow(int elementIndex, bool value);
		int GetFontCharset(int elementIndex);
		void SetFontCharset(int elementIndex, int value);
		int GetFontFontFamily(int elementIndex);
		void SetFontFontFamily(int elementIndex, int value);
		XlUnderlineType GetFontUnderline(int elementIndex);
		void SetFontUnderline(int elementIndex, XlUnderlineType value);
		Color GetFontColor(int elementIndex);
		void SetFontColor(int elementIndex, Color value);
		XlScriptType GetFontScript(int elementIndex);
		void SetFontScript(int elementIndex, XlScriptType value);
		XlFontSchemeStyles GetFontSchemeStyle(int elementIndex);
		void SetFontSchemeStyle(int elementIndex, XlFontSchemeStyles value);
		#endregion
		#region Fill
		void ClearFill(int elementIndex);
		XlPatternType GetFillPatternType(int elementIndex);
		void SetFillPatternType(int elementIndex, XlPatternType value);
		Color GetFillForeColor(int elementIndex);
		void SetFillForeColor(int elementIndex, Color value);
		Color GetFillBackColor(int elementIndex);
		void SetFillBackColor(int elementIndex, Color value);
		ModelFillType GetFillType(int elementIndex);
		void SetFillType(int elementIndex, ModelFillType value);
		IGradientStopCollection GetGradientStops(int elementIndex);
		ModelGradientFillType GetGradientFillType(int elementIndex);
		void SetGradientFillType(int elementIndex, ModelGradientFillType value);
		double GetDegree(int elementIndex);
		void SetDegree(int elementIndex, double value);
		float GetConvergenceLeft(int elementIndex);
		void SetConvergenceLeft(int elementIndex, float value);
		float GetConvergenceRight(int elementIndex);
		void SetConvergenceRight(int elementIndex, float value);
		float GetConvergenceTop(int elementIndex);
		void SetConvergenceTop(int elementIndex, float value);
		float GetConvergenceBottom(int elementIndex);
		void SetConvergenceBottom(int elementIndex, float value);
		#endregion
		#region Border
		XlBorderLineStyle GetBorderLeftLineStyle(int elementIndex);
		void SetBorderLeftLineStyle(int elementIndex, XlBorderLineStyle value);
		Color GetBorderLeftColor(int elementIndex);
		void SetBorderLeftColor(int elementIndex, Color value);
		int GetBorderLeftColorIndex(int elementIndex);
		void SetBorderLeftColorIndex(int elementIndex, int value);
		XlBorderLineStyle GetBorderRightLineStyle(int elementIndex);
		void SetBorderRightLineStyle(int elementIndex, XlBorderLineStyle value);
		Color GetBorderRightColor(int elementIndex);
		void SetBorderRightColor(int elementIndex, Color value);
		int GetBorderRightColorIndex(int elementIndex);
		void SetBorderRightColorIndex(int elementIndex, int value);
		XlBorderLineStyle GetBorderTopLineStyle(int elementIndex);
		void SetBorderTopLineStyle(int elementIndex, XlBorderLineStyle value);
		Color GetBorderTopColor(int elementIndex);
		void SetBorderTopColor(int elementIndex, Color value);
		int GetBorderTopColorIndex(int elementIndex);
		void SetBorderTopColorIndex(int elementIndex, int value);
		XlBorderLineStyle GetBorderBottomLineStyle(int elementIndex);
		void SetBorderBottomLineStyle(int elementIndex, XlBorderLineStyle value);
		Color GetBorderBottomColor(int elementIndex);
		void SetBorderBottomColor(int elementIndex, Color value);
		int GetBorderBottomColorIndex(int elementIndex);
		void SetBorderBottomColorIndex(int elementIndex, int value);
		XlBorderLineStyle GetBorderHorizontalLineStyle(int elementIndex);
		void SetBorderHorizontalLineStyle(int elementIndex, XlBorderLineStyle value);
		Color GetBorderHorizontalColor(int elementIndex);
		void SetBorderHorizontalColor(int elementIndex, Color value);
		int GetBorderHorizontalColorIndex(int elementIndex);
		void SetBorderHorizontalColorIndex(int elementIndex, int value);
		XlBorderLineStyle GetBorderVerticalLineStyle(int elementIndex);
		void SetBorderVerticalLineStyle(int elementIndex, XlBorderLineStyle value);
		Color GetBorderVerticalColor(int elementIndex);
		void SetBorderVerticalColor(int elementIndex, Color value);
		int GetBorderVerticalColorIndex(int elementIndex);
		void SetBorderVerticalColorIndex(int elementIndex, int value);
		XlBorderLineStyle GetBorderDiagonalUpLineStyle(int elementIndex);
		void SetBorderDiagonalUpLineStyle(int elementIndex, XlBorderLineStyle value);
		Color GetBorderDiagonalColor(int elementIndex);
		void SetBorderDiagonalColor(int elementIndex, Color value);
		int GetBorderDiagonalColorIndex(int elementIndex);
		void SetBorderDiagonalColorIndex(int elementIndex, int value);
		XlBorderLineStyle GetBorderDiagonalDownLineStyle(int elementIndex);
		void SetBorderDiagonalDownLineStyle(int elementIndex, XlBorderLineStyle value);
		bool GetBorderOutline(int elementIndex);
		void SetBorderOutline(int elementIndex, bool value);
		#endregion
		#region CellAlignment
		bool GetCellAlignmentWrapText(int elementIndex);
		void SetCellAlignmentWrapText(int elementIndex, bool value);
		bool GetCellAlignmentJustifyLastLine(int elementIndex);
		void SetCellAlignmentJustifyLastLine(int elementIndex, bool value);
		bool GetCellAlignmentShrinkToFit(int elementIndex);
		void SetCellAlignmentShrinkToFit(int elementIndex, bool value);
		int GetCellAlignmentTextRotation(int elementIndex);
		void SetCellAlignmentTextRotation(int elementIndex, int value);
		byte GetCellAlignmentIndent(int elementIndex);
		void SetCellAlignmentIndent(int elementIndex, byte value);
		int GetCellAlignmentRelativeIndent(int elementIndex);
		void SetCellAlignmentRelativeIndent(int elementIndex, int value);
		XlHorizontalAlignment GetCellAlignmentHorizontal(int elementIndex);
		void SetCellAlignmentHorizontal(int elementIndex, XlHorizontalAlignment value);
		XlVerticalAlignment GetCellAlignmentVertical(int elementIndex);
		void SetCellAlignmentVertical(int elementIndex, XlVerticalAlignment value);
		XlReadingOrder GetCellAlignmentReadingOrder(int elementIndex);
		void SetCellAlignmentReadingOrder(int elementIndex, XlReadingOrder value);
		#endregion
		#region CellProtection
		bool GetCellProtectionLocked(int elementIndex);
		void SetCellProtectionLocked(int elementIndex, bool value);
		bool GetCellProtectionHidden(int elementIndex);
		void SetCellProtectionHidden(int elementIndex, bool value);
		#endregion
	}
	#endregion
	#region DifferentialFormatPropertyChangeManager
	public class DifferentialFormatPropertyChangeManager : IDifferentialFormat, IRunFontInfo, IFillInfo, IBorderInfo, ICellAlignmentInfo, ICellProtectionInfo, IGradientFillInfo, IConvergenceInfo {
		int elementIndex;
		readonly IDifferentialFormatPropertyChanger info;
		public DifferentialFormatPropertyChangeManager(IDifferentialFormatPropertyChanger info) {
			this.info = info;
		}
		#region Properties
		protected IDifferentialFormatPropertyChanger Info { get { return info; } }
		protected int ElementIndex { get { return elementIndex; } set { elementIndex = value; } }
		#region IDifferentialFormat Members
		public IRunFontInfo Font { get { return this; } }
		public IFillInfo Fill { get { return this; } }
		public IBorderInfo Border { get { return this; } }
		public ICellAlignmentInfo Alignment { get { return this; } }
		public ICellProtectionInfo Protection { get { return this; } }
		#region FormatString
		public string FormatString {
			get { return Info.GetFormatString(ElementIndex); }
			set { Info.SetFormatString(ElementIndex, value); }
		}
		#endregion
		#endregion
		#region IRunFontInfo Members
		double IRunFontInfo.Size {
			get { return Info.GetFontSize(ElementIndex); }
			set { Info.SetFontSize(ElementIndex, value); }
		}
		string IRunFontInfo.Name {
			get { return Info.GetFontName(ElementIndex); }
			set { Info.SetFontName(ElementIndex, value); }
		}
		bool IRunFontInfo.Bold {
			get { return Info.GetFontBold(ElementIndex); }
			set { Info.SetFontBold(ElementIndex, value); }
		}
		bool IRunFontInfo.Italic {
			get { return Info.GetFontItalic(ElementIndex); }
			set { Info.SetFontItalic(ElementIndex, value); }
		}
		bool IRunFontInfo.StrikeThrough {
			get { return Info.GetFontStrikeThrough(ElementIndex); }
			set { Info.SetFontStrikeThrough(ElementIndex, value); }
		}
		bool IRunFontInfo.Condense {
			get { return Info.GetFontCondense(ElementIndex); }
			set { Info.SetFontCondense(ElementIndex, value); }
		}
		bool IRunFontInfo.Extend {
			get { return Info.GetFontExtend(ElementIndex); }
			set { Info.SetFontExtend(ElementIndex, value); }
		}
		bool IRunFontInfo.Outline {
			get { return Info.GetFontOutline(ElementIndex); }
			set { Info.SetFontOutline(ElementIndex, value); }
		}
		bool IRunFontInfo.Shadow {
			get { return Info.GetFontShadow(ElementIndex); }
			set { Info.SetFontShadow(ElementIndex, value); }
		}
		int IRunFontInfo.Charset {
			get { return Info.GetFontCharset(ElementIndex); }
			set { Info.SetFontCharset(ElementIndex, value); }
		}
		int IRunFontInfo.FontFamily {
			get { return Info.GetFontFontFamily(ElementIndex); }
			set { Info.SetFontFontFamily(ElementIndex, value); }
		}
		XlUnderlineType IRunFontInfo.Underline {
			get { return Info.GetFontUnderline(ElementIndex); }
			set { Info.SetFontUnderline(ElementIndex, value); }
		}
		Color IRunFontInfo.Color {
			get { return Info.GetFontColor(ElementIndex); }
			set { Info.SetFontColor(ElementIndex, value); }
		}
		XlScriptType IRunFontInfo.Script {
			get { return Info.GetFontScript(ElementIndex); }
			set { Info.SetFontScript(ElementIndex, value); }
		}
		XlFontSchemeStyles IRunFontInfo.SchemeStyle {
			get { return Info.GetFontSchemeStyle(ElementIndex); }
			set { Info.SetFontSchemeStyle(ElementIndex, value); }
		}
		#endregion
		#region IFillInfo Members
		void IFillInfo.Clear() {
			Info.ClearFill(ElementIndex);
		}
		XlPatternType IFillInfo.PatternType {
			get { return Info.GetFillPatternType(ElementIndex); }
			set { Info.SetFillPatternType(ElementIndex, value); }
		}
		Color IFillInfo.ForeColor {
			get { return Info.GetFillForeColor(ElementIndex); }
			set { Info.SetFillForeColor(ElementIndex, value); }
		}
		Color IFillInfo.BackColor {
			get { return Info.GetFillBackColor(ElementIndex); }
			set { Info.SetFillBackColor(ElementIndex, value); }
		}
		IGradientFillInfo IFillInfo.GradientFill { get { return this; } }
		ModelFillType IFillInfo.FillType {
			get { return Info.GetFillType(ElementIndex); }
			set { Info.SetFillType(ElementIndex, value); }
		}
		IConvergenceInfo IGradientFillInfo.Convergence { get { return this; } }
		IGradientStopCollection IGradientFillInfo.GradientStops { get { return Info.GetGradientStops(ElementIndex); } }
		ModelGradientFillType IGradientFillInfo.Type {
			get { return Info.GetGradientFillType(ElementIndex); }
			set { Info.SetGradientFillType(ElementIndex, value); }
		}
		double IGradientFillInfo.Degree {
			get { return Info.GetDegree(ElementIndex); }
			set { Info.SetDegree(ElementIndex, value); }
		}
		float IConvergenceInfo.Left {
			get { return Info.GetConvergenceLeft(ElementIndex); }
			set { Info.SetConvergenceLeft(ElementIndex, value); }
		}
		float IConvergenceInfo.Right {
			get { return Info.GetConvergenceRight(ElementIndex); }
			set { Info.SetConvergenceRight(ElementIndex, value); }
		}
		float IConvergenceInfo.Top {
			get { return Info.GetConvergenceTop(ElementIndex); }
			set { Info.SetConvergenceTop(ElementIndex, value); }
		}
		float IConvergenceInfo.Bottom {
			get { return Info.GetConvergenceBottom(ElementIndex); }
			set { Info.SetConvergenceBottom(ElementIndex, value); }
		}
		#endregion
		#region IBorderInfo Members
		XlBorderLineStyle IBorderInfo.LeftLineStyle {
			get { return Info.GetBorderLeftLineStyle(ElementIndex); }
			set { Info.SetBorderLeftLineStyle(ElementIndex, value); }
		}
		XlBorderLineStyle IBorderInfo.RightLineStyle {
			get { return Info.GetBorderRightLineStyle(ElementIndex); }
			set { Info.SetBorderRightLineStyle(ElementIndex, value); }
		}
		XlBorderLineStyle IBorderInfo.TopLineStyle {
			get { return Info.GetBorderTopLineStyle(ElementIndex); }
			set { Info.SetBorderTopLineStyle(ElementIndex, value); }
		}
		XlBorderLineStyle IBorderInfo.BottomLineStyle {
			get { return Info.GetBorderBottomLineStyle(ElementIndex); }
			set { Info.SetBorderBottomLineStyle(ElementIndex, value); }
		}
		XlBorderLineStyle IBorderInfo.HorizontalLineStyle {
			get { return Info.GetBorderHorizontalLineStyle(ElementIndex); }
			set { Info.SetBorderHorizontalLineStyle(ElementIndex, value); }
		}
		XlBorderLineStyle IBorderInfo.VerticalLineStyle {
			get { return Info.GetBorderVerticalLineStyle(ElementIndex); }
			set { Info.SetBorderVerticalLineStyle(ElementIndex, value); }
		}
		XlBorderLineStyle IBorderInfo.DiagonalUpLineStyle {
			get { return Info.GetBorderDiagonalUpLineStyle(ElementIndex); }
			set { Info.SetBorderDiagonalUpLineStyle(ElementIndex, value); }
		}
		XlBorderLineStyle IBorderInfo.DiagonalDownLineStyle {
			get { return Info.GetBorderDiagonalDownLineStyle(ElementIndex); }
			set { Info.SetBorderDiagonalDownLineStyle(ElementIndex, value); }
		}
		Color IBorderInfo.LeftColor {
			get { return Info.GetBorderLeftColor(ElementIndex); }
			set { Info.SetBorderLeftColor(ElementIndex, value); }
		}
		Color IBorderInfo.RightColor {
			get { return Info.GetBorderRightColor(ElementIndex); }
			set { Info.SetBorderRightColor(ElementIndex, value); }
		}
		Color IBorderInfo.TopColor {
			get { return Info.GetBorderTopColor(ElementIndex); }
			set { Info.SetBorderTopColor(ElementIndex, value); }
		}
		Color IBorderInfo.BottomColor {
			get { return Info.GetBorderBottomColor(ElementIndex); }
			set { Info.SetBorderBottomColor(ElementIndex, value); }
		}
		Color IBorderInfo.HorizontalColor {
			get { return Info.GetBorderHorizontalColor(ElementIndex); }
			set { Info.SetBorderHorizontalColor(ElementIndex, value); }
		}
		Color IBorderInfo.VerticalColor {
			get { return Info.GetBorderVerticalColor(ElementIndex); }
			set { Info.SetBorderVerticalColor(ElementIndex, value); }
		}
		Color IBorderInfo.DiagonalColor {
			get { return Info.GetBorderDiagonalColor(ElementIndex); }
			set { Info.SetBorderDiagonalColor(ElementIndex, value); }
		}
		int IBorderInfo.LeftColorIndex {
			get { return Info.GetBorderLeftColorIndex(ElementIndex); }
			set { Info.SetBorderLeftColorIndex(ElementIndex, value); }
		}
		int IBorderInfo.RightColorIndex {
			get { return Info.GetBorderRightColorIndex(ElementIndex); }
			set { Info.SetBorderRightColorIndex(ElementIndex, value); }
		}
		int IBorderInfo.TopColorIndex {
			get { return Info.GetBorderTopColorIndex(ElementIndex); }
			set { Info.SetBorderTopColorIndex(ElementIndex, value); }
		}
		int IBorderInfo.BottomColorIndex {
			get { return Info.GetBorderBottomColorIndex(ElementIndex); }
			set { Info.SetBorderBottomColorIndex(ElementIndex, value); }
		}
		int IBorderInfo.HorizontalColorIndex {
			get { return Info.GetBorderHorizontalColorIndex(ElementIndex); }
			set { Info.SetBorderHorizontalColorIndex(ElementIndex, value); }
		}
		int IBorderInfo.VerticalColorIndex {
			get { return Info.GetBorderVerticalColorIndex(ElementIndex); }
			set { Info.SetBorderVerticalColorIndex(ElementIndex, value); }
		}
		int IBorderInfo.DiagonalColorIndex {
			get { return Info.GetBorderDiagonalColorIndex(ElementIndex); }
			set { Info.SetBorderDiagonalColorIndex(ElementIndex, value); }
		}
		bool IBorderInfo.Outline {
			get { return Info.GetBorderOutline(ElementIndex); }
			set { Info.SetBorderOutline(ElementIndex, value); }
		}
		#endregion
		#region ICellAlignmentInfo Members
		bool ICellAlignmentInfo.WrapText {
			get { return Info.GetCellAlignmentWrapText(ElementIndex); }
			set { Info.SetCellAlignmentWrapText(ElementIndex, value); }
		}
		public bool JustifyLastLine {
			get { return Info.GetCellAlignmentJustifyLastLine(ElementIndex); }
			set { Info.SetCellAlignmentJustifyLastLine(ElementIndex, value); }
		}
		public bool ShrinkToFit {
			get { return Info.GetCellAlignmentShrinkToFit(ElementIndex); }
			set { Info.SetCellAlignmentShrinkToFit(ElementIndex, value); }
		}
		public int TextRotation {
			get { return Info.GetCellAlignmentTextRotation(ElementIndex); }
			set { Info.SetCellAlignmentTextRotation(ElementIndex, value); }
		}
		public byte Indent {
			get { return Info.GetCellAlignmentIndent(ElementIndex); }
			set { Info.SetCellAlignmentIndent(ElementIndex, value); }
		}
		public int RelativeIndent {
			get { return Info.GetCellAlignmentRelativeIndent(ElementIndex); }
			set { Info.SetCellAlignmentRelativeIndent(ElementIndex, value); }
		}
		public XlHorizontalAlignment Horizontal {
			get { return Info.GetCellAlignmentHorizontal(ElementIndex); }
			set { Info.SetCellAlignmentHorizontal(ElementIndex, value); }
		}
		public XlVerticalAlignment Vertical {
			get { return Info.GetCellAlignmentVertical(ElementIndex); }
			set { Info.SetCellAlignmentVertical(ElementIndex, value); }
		}
		public XlReadingOrder ReadingOrder {
			get { return Info.GetCellAlignmentReadingOrder(ElementIndex); }
			set { Info.SetCellAlignmentReadingOrder(ElementIndex, value); }
		}
		#endregion
		#region ICellProtectionInfo Members
		bool ICellProtectionInfo.Locked {
			get { return Info.GetCellProtectionLocked(ElementIndex); }
			set { Info.SetCellProtectionLocked(ElementIndex, value); }
		}
		bool ICellProtectionInfo.Hidden {
			get { return Info.GetCellProtectionHidden(ElementIndex); }
			set { Info.SetCellProtectionHidden(ElementIndex, value); }
		}
		#endregion
		#endregion
		protected internal IDifferentialFormat GetFormatInfo(int elementIndex) {
			this.elementIndex = elementIndex;
			return this;
		}
	}
	#endregion
	#region DifferentialFormatPropertyDescriptor
	public enum DifferentialFormatPropertyDescriptor {
		NumberFormat = 0,
		NumberFormatIndex = 1,
		FontName = 3,
		FontColorIndex = 4,
		FontBold = 5,
		FontCondense = 6,
		FontExtend = 7,
		FontItalic = 8,
		FontOutline = 9,
		FontShadow = 10,
		FontStrikeThrough = 11,
		FontCharset = 12,
		FontFamily = 13,
		FontSize = 14,
		FontSchemeStyle = 15,
		FontScript = 16,
		FontUnderline = 17,
		FillPatternType = 18,
		FillBackColorIndex = 19,
		FillForeColorIndex = 20,
		FillType = 21,
		FillGradientFillType = 22,
		FillGradientFillDegree = 23,
		FillGradientFillGradientStops = 24,
		FillGradientFillCovergenceLeft = 25,
		FillGradientFillCovergenceRight = 26,
		FillGradientFillCovergenceTop = 27,
		FillGradientFillCovergenceBottom = 28,
		AlignmentHorizontal = 29,
		AlignmentVertical = 30,
		AlignmentWrapText = 31,
		AlignmentJustifyLastLine = 32,
		AlignmentShrinkToFit = 33,
		AlignmentReadingOrder = 34,
		AlignmentTextRotation = 35,
		AlignmentIndent = 36,
		AlignmentRelativeIndent = 37,
		ProtectionLocked = 38,
		ProtectionHidden = 39,
		BorderHorizontalLineStyle = 40,
		BorderVerticalLineStyle = 41,
		BorderDiagonalUpLineStyle = 42,
		BorderDiagonalDownLineStyle = 43,
		BorderHorizontalColorIndex = 44,
		BorderVerticalColorIndex = 45,
		BorderDiagonalColorIndex = 46,
		BorderOutline = 47,
		HasVisibleFill = 48,
	}
	#endregion
	#region DifferentialFormatDisplayBorderDescriptor
	public enum DifferentialFormatDisplayBorderDescriptor {
		LeftLineStyle = 49,
		RightLineStyle = 50,
		TopLineStyle = 51,
		BottomLineStyle = 52,
		LeftColorIndex = 53,
		RightColorIndex = 54,
		TopColorIndex = 55,
		BottomColorIndex = 56,
		LeftInfo = 57,
		RightInfo = 58,
		TopInfo = 59,
		BottomInfo = 60,
	}
	#endregion
	#region DifferentialFormatPropertyAccessors
	public static class DifferentialFormatPropertyAccessors {
		#region baseAccessorsTable
		static Dictionary<DifferentialFormatPropertyDescriptor, IDifferentialFormatPropertyAccessorBase> baseAccessorsTable = GetBaseAccessorsTable();
		static Dictionary<DifferentialFormatPropertyDescriptor, IDifferentialFormatPropertyAccessorBase> GetBaseAccessorsTable() {
			Dictionary<DifferentialFormatPropertyDescriptor, IDifferentialFormatPropertyAccessorBase> result = new Dictionary<DifferentialFormatPropertyDescriptor, IDifferentialFormatPropertyAccessorBase>();
			result.Add(DifferentialFormatPropertyDescriptor.NumberFormatIndex, NumberFormatIndex);
			result.Add(DifferentialFormatPropertyDescriptor.NumberFormat, NumberFormatCode);
			result.Add(DifferentialFormatPropertyDescriptor.FontName, FontName);
			result.Add(DifferentialFormatPropertyDescriptor.FontColorIndex, FontColorIndex);
			result.Add(DifferentialFormatPropertyDescriptor.FontBold, FontBold);
			result.Add(DifferentialFormatPropertyDescriptor.FontCondense, FontCondense);
			result.Add(DifferentialFormatPropertyDescriptor.FontExtend, FontExtend);
			result.Add(DifferentialFormatPropertyDescriptor.FontItalic, FontItalic);
			result.Add(DifferentialFormatPropertyDescriptor.FontOutline, FontOutline);
			result.Add(DifferentialFormatPropertyDescriptor.FontShadow, FontShadow);
			result.Add(DifferentialFormatPropertyDescriptor.FontStrikeThrough, FontStrikeThrough);
			result.Add(DifferentialFormatPropertyDescriptor.FontCharset, FontCharset);
			result.Add(DifferentialFormatPropertyDescriptor.FontFamily, FontFamily);
			result.Add(DifferentialFormatPropertyDescriptor.FontSize, FontSize);
			result.Add(DifferentialFormatPropertyDescriptor.FontSchemeStyle, FontSchemeStyle);
			result.Add(DifferentialFormatPropertyDescriptor.FontScript, FontScript);
			result.Add(DifferentialFormatPropertyDescriptor.FontUnderline, FontUnderline);
			result.Add(DifferentialFormatPropertyDescriptor.FillPatternType, FillPatternType);
			result.Add(DifferentialFormatPropertyDescriptor.FillBackColorIndex, FillBackColorIndex);
			result.Add(DifferentialFormatPropertyDescriptor.FillForeColorIndex, FillForeColorIndex);
			result.Add(DifferentialFormatPropertyDescriptor.FillType, FillType);
			result.Add(DifferentialFormatPropertyDescriptor.FillGradientFillType, FillGradientFillType);
			result.Add(DifferentialFormatPropertyDescriptor.FillGradientFillDegree, FillGradientFillDegree);
			result.Add(DifferentialFormatPropertyDescriptor.FillGradientFillGradientStops, FillGradientFillGradientStops);
			result.Add(DifferentialFormatPropertyDescriptor.FillGradientFillCovergenceLeft, FillGradientFillCovergenceLeft);
			result.Add(DifferentialFormatPropertyDescriptor.FillGradientFillCovergenceRight, FillGradientFillCovergenceRight);
			result.Add(DifferentialFormatPropertyDescriptor.FillGradientFillCovergenceTop, FillGradientFillCovergenceTop);
			result.Add(DifferentialFormatPropertyDescriptor.FillGradientFillCovergenceBottom, FillGradientFillCovergenceBottom);
			result.Add(DifferentialFormatPropertyDescriptor.AlignmentHorizontal, AlignmentHorizontal);
			result.Add(DifferentialFormatPropertyDescriptor.AlignmentVertical, AlignmentVertical);
			result.Add(DifferentialFormatPropertyDescriptor.AlignmentWrapText, AlignmentWrapText);
			result.Add(DifferentialFormatPropertyDescriptor.AlignmentJustifyLastLine, AlignmentJustifyLastLine);
			result.Add(DifferentialFormatPropertyDescriptor.AlignmentShrinkToFit, AlignmentShrinkToFit);
			result.Add(DifferentialFormatPropertyDescriptor.AlignmentReadingOrder, AlignmentReadingOrder);
			result.Add(DifferentialFormatPropertyDescriptor.AlignmentTextRotation, AlignmentTextRotation);
			result.Add(DifferentialFormatPropertyDescriptor.AlignmentIndent, AlignmentIndent);
			result.Add(DifferentialFormatPropertyDescriptor.AlignmentRelativeIndent, AlignmentRelativeIndent);
			result.Add(DifferentialFormatPropertyDescriptor.BorderHorizontalLineStyle, BorderHorizontalLineStyle);
			result.Add(DifferentialFormatPropertyDescriptor.BorderVerticalLineStyle, BorderVerticalLineStyle);
			result.Add(DifferentialFormatPropertyDescriptor.BorderDiagonalColorIndex, BorderDiagonalColorIndex);
			result.Add(DifferentialFormatPropertyDescriptor.BorderHorizontalColorIndex, BorderHorizontalColorIndex);
			result.Add(DifferentialFormatPropertyDescriptor.BorderVerticalColorIndex, BorderVerticalColorIndex);
			result.Add(DifferentialFormatPropertyDescriptor.BorderDiagonalUpLineStyle, BorderDiagonalUpLineStyle);
			result.Add(DifferentialFormatPropertyDescriptor.BorderDiagonalDownLineStyle, BorderDiagonalDownLineStyle);
			result.Add(DifferentialFormatPropertyDescriptor.BorderOutline, BorderOutline);
			result.Add(DifferentialFormatPropertyDescriptor.ProtectionLocked, ProtectionLocked);
			result.Add(DifferentialFormatPropertyDescriptor.ProtectionHidden, ProtectionHidden);
			result.Add(DifferentialFormatPropertyDescriptor.HasVisibleFill, HasVisibleFill);
			return result;
		}
		#endregion
		#region DisplayBorderAccessorsTable
		static Dictionary<DifferentialFormatDisplayBorderDescriptor, IDifferentialFormatDisplayBorderPropertyAccessorBase> displayBorderAccessorsTable = GetDisplayBordersTable();
		static Dictionary<DifferentialFormatDisplayBorderDescriptor, IDifferentialFormatDisplayBorderPropertyAccessorBase> GetDisplayBordersTable() {
			Dictionary<DifferentialFormatDisplayBorderDescriptor, IDifferentialFormatDisplayBorderPropertyAccessorBase> result = new Dictionary<DifferentialFormatDisplayBorderDescriptor, IDifferentialFormatDisplayBorderPropertyAccessorBase>();
			result.Add(DifferentialFormatDisplayBorderDescriptor.LeftLineStyle, BorderLeftLineStyle);
			result.Add(DifferentialFormatDisplayBorderDescriptor.RightLineStyle, BorderRightLineStyle);
			result.Add(DifferentialFormatDisplayBorderDescriptor.TopLineStyle, BorderTopLineStyle);
			result.Add(DifferentialFormatDisplayBorderDescriptor.BottomLineStyle, BorderBottomLineStyle);
			result.Add(DifferentialFormatDisplayBorderDescriptor.LeftColorIndex, BorderLeftColorIndex);
			result.Add(DifferentialFormatDisplayBorderDescriptor.RightColorIndex, BorderRightColorIndex);
			result.Add(DifferentialFormatDisplayBorderDescriptor.TopColorIndex, BorderTopColorIndex);
			result.Add(DifferentialFormatDisplayBorderDescriptor.BottomColorIndex, BorderBottomColorIndex);
			result.Add(DifferentialFormatDisplayBorderDescriptor.LeftInfo, BorderLeftInfo);
			result.Add(DifferentialFormatDisplayBorderDescriptor.RightInfo, BorderRightInfo);
			result.Add(DifferentialFormatDisplayBorderDescriptor.TopInfo, BorderTopInfo);
			result.Add(DifferentialFormatDisplayBorderDescriptor.BottomInfo, BorderBottomInfo);
			return result;
		}
		#endregion
		#region NumberFormatAccessors
		public static IDifferentialFormatPropertyAccessor<int> NumberFormatIndex { get { return new DifferentialFormatNumberFormatIndexAccessor(); } }
		public static IDifferentialFormatPropertyAccessor<string> NumberFormatCode { get { return new DifferentialFormatNumberFormatCodeAccessor(); } }
		#endregion
		#region FontAccessors
		public static IDifferentialFormatPropertyAccessor<string> FontName { get { return new DifferentialFormatFontNameAccessor(); } }
		public static IDifferentialFormatPropertyAccessor<int> FontColorIndex { get { return new DifferentialFormatFontColorIndexAccessor(); } }
		public static IDifferentialFormatPropertyAccessor<bool> FontBold { get { return new DifferentialFormatFontBoldAccessor(); } }
		public static IDifferentialFormatPropertyAccessor<bool> FontCondense { get { return new DifferentialFormatFontCondenseAccessor(); } }
		public static IDifferentialFormatPropertyAccessor<bool> FontExtend { get { return new DifferentialFormatFontExtendAccessor(); } }
		public static IDifferentialFormatPropertyAccessor<bool> FontItalic { get { return new DifferentialFormatFontItalicAccessor(); } }
		public static IDifferentialFormatPropertyAccessor<bool> FontOutline { get { return new DifferentialFormatFontOutlineAccessor(); } }
		public static IDifferentialFormatPropertyAccessor<bool> FontShadow { get { return new DifferentialFormatFontShadowAccessor(); } }
		public static IDifferentialFormatPropertyAccessor<bool> FontStrikeThrough { get { return new DifferentialFormatFontStrikeThroughAccessor(); } }
		public static IDifferentialFormatPropertyAccessor<int> FontCharset { get { return new DifferentialFormatFontCharsetAccessor(); } }
		public static IDifferentialFormatPropertyAccessor<int> FontFamily { get { return new DifferentialFormatFontFamilyAccessor(); } }
		public static IDifferentialFormatPropertyAccessor<double> FontSize { get { return new DifferentialFormatFontSizeAccessor(); } }
		public static IDifferentialFormatPropertyAccessor<XlFontSchemeStyles> FontSchemeStyle { get { return new DifferentialFormatFontSchemeStyleAccessor(); } }
		public static IDifferentialFormatPropertyAccessor<XlScriptType> FontScript { get { return new DifferentialFormatFontScriptAccessor(); } }
		public static IDifferentialFormatPropertyAccessor<XlUnderlineType> FontUnderline { get { return new DifferentialFormatFontUnderlineAccessor(); } }
		#endregion
		#region FillAccessors
		public static IDifferentialFormatPropertyAccessor<XlPatternType> FillPatternType { get { return new DifferentialFormatFillPatternTypeAccessor(); } }
		public static IDifferentialFormatPropertyAccessor<int> FillBackColorIndex { get { return new DifferentialFormatFillBackColorIndexAccessor(); } }
		public static IDifferentialFormatPropertyAccessor<int> FillForeColorIndex { get { return new DifferentialFormatFillForeColorIndexAccessor(); } }
		public static IDifferentialFormatPropertyAccessor<ModelFillType> FillType { get { return new DifferentialFormatFillTypeAccessor(); } }
		public static IDifferentialFormatPropertyAccessor<ModelGradientFillType> FillGradientFillType { get { return new DifferentialFormatFillGradientFillTypeAccessor(); } }
		public static IDifferentialFormatPropertyAccessor<double> FillGradientFillDegree { get { return new DifferentialFormatFillGradientFillDegreeAccessor(); } }
		public static IDifferentialFormatPropertyAccessor<GradientStopInfoCollection> FillGradientFillGradientStops { get { return new DifferentialFormatFillGradientFillGradientStopsAccessor(); } }
		public static IDifferentialFormatPropertyAccessor<float> FillGradientFillCovergenceLeft { get { return new DifferentialFormatFillGradientFillCovergenceLeftAccessor(); } }
		public static IDifferentialFormatPropertyAccessor<float> FillGradientFillCovergenceRight { get { return new DifferentialFormatFillGradientFillCovergenceRightAccessor(); } }
		public static IDifferentialFormatPropertyAccessor<float> FillGradientFillCovergenceTop { get { return new DifferentialFormatFillGradientFillCovergenceTopAccessor(); } }
		public static IDifferentialFormatPropertyAccessor<float> FillGradientFillCovergenceBottom { get { return new DifferentialFormatFillGradientFillCovergenceBottomAccessor(); } }
		public static IDifferentialFormatPropertyAccessorBase HasVisibleFill { get { return new DifferentialFormatHasVisibleFillAccessor(); } }
		#endregion
		#region AlignmentAccessors
		public static IDifferentialFormatPropertyAccessor<XlHorizontalAlignment> AlignmentHorizontal { get { return new DifferentialFormatAlignmentHorizontalAccessor(); } }
		public static IDifferentialFormatPropertyAccessor<XlVerticalAlignment> AlignmentVertical { get { return new DifferentialFormatAlignmentVerticalAccessor(); } }
		public static IDifferentialFormatPropertyAccessor<bool> AlignmentWrapText { get { return new DifferentialFormatAlignmentWrapTextAccessor(); } }
		public static IDifferentialFormatPropertyAccessor<bool> AlignmentJustifyLastLine { get { return new DifferentialFormatAlignmentJustifyLastLineAccessor(); } }
		public static IDifferentialFormatPropertyAccessor<bool> AlignmentShrinkToFit { get { return new DifferentialFormatAlignmentShrinkToFitAccessor(); } }
		public static IDifferentialFormatPropertyAccessor<XlReadingOrder> AlignmentReadingOrder { get { return new DifferentialFormatAlignmentReadingOrderAccessor(); } }
		public static IDifferentialFormatPropertyAccessor<int> AlignmentTextRotation { get { return new DifferentialFormatAlignmentTextRotationAccessor(); } }
		public static IDifferentialFormatPropertyAccessor<byte> AlignmentIndent { get { return new DifferentialFormatAlignmentIndentAccessor(); } }
		public static IDifferentialFormatPropertyAccessor<int> AlignmentRelativeIndent { get { return new DifferentialFormatAlignmentRelativeIndentAccessor(); } }
		#endregion
		#region BorderAccessors
		public static IDifferentialFormatDisplayBorderPropertyAccessor<XlBorderLineStyle> BorderLeftLineStyle { get { return new DifferentialFormatBorderLeftLineStyleAccessor(); } }
		public static IDifferentialFormatDisplayBorderPropertyAccessor<XlBorderLineStyle> BorderRightLineStyle { get { return new DifferentialFormatBorderRightLineStyleAccessor(); } }
		public static IDifferentialFormatDisplayBorderPropertyAccessor<XlBorderLineStyle> BorderTopLineStyle { get { return new DifferentialFormatBorderTopLineStyleAccessor(); } }
		public static IDifferentialFormatDisplayBorderPropertyAccessor<XlBorderLineStyle> BorderBottomLineStyle { get { return new DifferentialFormatBorderBottomLineStyleAccessor(); } }
		public static IDifferentialFormatPropertyAccessor<XlBorderLineStyle> BorderDiagonalLineStyle { get { return new DifferentialFormatBorderDiagonalLineStyleAccessor(); } }
		public static IDifferentialFormatPropertyAccessor<XlBorderLineStyle> BorderDiagonalUpLineStyle { get { return new DifferentialFormatBorderDiagonalUpLineStyleAccessor(); } }
		public static IDifferentialFormatPropertyAccessor<XlBorderLineStyle> BorderDiagonalDownLineStyle { get { return new DifferentialFormatBorderDiagonalDownLineStyleAccessor(); } }
		public static IDifferentialFormatPropertyAccessor<XlBorderLineStyle> BorderHorizontalLineStyle { get { return new DifferentialFormatBorderHorizontalLineStyleAccessor(); } }
		public static IDifferentialFormatPropertyAccessor<XlBorderLineStyle> BorderVerticalLineStyle { get { return new DifferentialFormatBorderVerticalLineStyleAccessor(); } }
		public static IDifferentialFormatDisplayBorderPropertyAccessor<int> BorderLeftColorIndex { get { return new DifferentialFormatBorderLeftColorIndexAccessor(); } }
		public static IDifferentialFormatDisplayBorderPropertyAccessor<int> BorderRightColorIndex { get { return new DifferentialFormatBorderRightColorIndexAccessor(); } }
		public static IDifferentialFormatDisplayBorderPropertyAccessor<int> BorderTopColorIndex { get { return new DifferentialFormatBorderTopColorIndexAccessor(); } }
		public static IDifferentialFormatDisplayBorderPropertyAccessor<int> BorderBottomColorIndex { get { return new DifferentialFormatBorderBottomColorIndexAccessor(); } }
		public static IDifferentialFormatPropertyAccessor<int> BorderDiagonalColorIndex { get { return new DifferentialFormatBorderDiagonalColorIndexAccessor(); } }
		public static IDifferentialFormatPropertyAccessor<int> BorderHorizontalColorIndex { get { return new DifferentialFormatBorderHorizontalColorIndexAccessor(); } }
		public static IDifferentialFormatPropertyAccessor<int> BorderVerticalColorIndex { get { return new DifferentialFormatBorderVerticalColorIndexAccessor(); } }
		public static IDifferentialFormatPropertyAccessor<bool> BorderDiagonalUp { get { return new DifferentialFormatBorderDiagonalUpAccessor(); } }
		public static IDifferentialFormatPropertyAccessor<bool> BorderDiagonalDown { get { return new DifferentialFormatBorderDiagonalDownAccessor(); } }
		public static IDifferentialFormatPropertyAccessor<bool> BorderOutline { get { return new DifferentialFormatBorderOutlineAccessor(); } }
		public static IDifferentialFormatDisplayBorderPropertyAccessor<ActualBorderElementInfo> BorderLeftInfo { get { return new DifferentialFormatActualLeftBorderInfoAccessor(); } }
		public static IDifferentialFormatDisplayBorderPropertyAccessor<ActualBorderElementInfo> BorderRightInfo { get { return new DifferentialFormatActualRightBorderInfoAccessor(); } }
		public static IDifferentialFormatDisplayBorderPropertyAccessor<ActualBorderElementInfo> BorderTopInfo { get { return new DifferentialFormatActualTopBorderInfoAccessor(); } }
		public static IDifferentialFormatDisplayBorderPropertyAccessor<ActualBorderElementInfo> BorderBottomInfo { get { return new DifferentialFormatActualBottomBorderInfoAccessor(); } }
		#endregion
		#region ProtectionAccessors
		public static IDifferentialFormatPropertyAccessor<bool> ProtectionLocked { get { return new DifferentialFormatProtectionLockedAccessor(); } }
		public static IDifferentialFormatPropertyAccessor<bool> ProtectionHidden { get { return new DifferentialFormatProtectionHiddenAccessor(); } }
		#endregion
		public static IDifferentialFormatPropertyAccessorBase GetPropertyBaseAccessor(DifferentialFormatPropertyDescriptor descriptor) {
			return baseAccessorsTable[descriptor];
		}
		public static IDifferentialFormatDisplayBorderPropertyAccessorBase GetDisplayBorderBaseAccessor(DifferentialFormatDisplayBorderDescriptor descriptor) {
			return displayBorderAccessorsTable[descriptor];
		}
	}
	#region IDifferentialFormatPropertyAccessor
	public interface IDifferentialFormatPropertyAccessorBase {
		bool GetApply(DifferentialFormat differentialFormat);
	}
	public interface IDifferentialFormatPropertyAccessor<TValue> : IDifferentialFormatPropertyAccessorBase {
		TValue GetValue(DifferentialFormat differentialFormat);
	}
	public interface IDifferentialFormatDisplayBorderPropertyAccessorBase {
		bool GetApply(BorderOptionsInfo info, bool isOutline);
	}
	public interface IDifferentialFormatDisplayBorderPropertyAccessor<TValue> : IDifferentialFormatDisplayBorderPropertyAccessorBase {
		BorderIndex BorderIndex { get; }
		TValue GetValue(BorderInfo info, bool isOutline);
		IDifferentialFormatDisplayBorderPropertyAccessor<TValue> NearbyAccessor { get; }
		CellPositionOffset NearbyOffset { get; }
	}
	#endregion
	#region DifferentialFormatNumberFormatAccessors
	public struct DifferentialFormatNumberFormatIndexAccessor : IDifferentialFormatPropertyAccessor<int> {
		public bool GetApply(DifferentialFormat differentialFormat) {
			return differentialFormat.MultiOptionsInfo.ApplyNumberFormat;
		}
		public int GetValue(DifferentialFormat differentialFormat) {
			return differentialFormat.NumberFormatIndex;
		}
	}
	public struct DifferentialFormatNumberFormatCodeAccessor : IDifferentialFormatPropertyAccessor<string> {
		public bool GetApply(DifferentialFormat differentialFormat) {
			return differentialFormat.MultiOptionsInfo.ApplyNumberFormat;
		}
		public string GetValue(DifferentialFormat differentialFormat) {
			return differentialFormat.NumberFormatInfo.FormatCode;
		}
	}
	#endregion
	#region DifferentialFormatFontAccessors
	public struct DifferentialFormatFontNameAccessor : IDifferentialFormatPropertyAccessor<string> {
		public bool GetApply(DifferentialFormat format) {
			return format.MultiOptionsInfo.ApplyFontName;
		}
		public string GetValue(DifferentialFormat format) {
			return format.FontInfo.Name;
		}
	}
	public struct DifferentialFormatFontColorIndexAccessor : IDifferentialFormatPropertyAccessor<int> {
		public bool GetApply(DifferentialFormat format) {
			return format.MultiOptionsInfo.ApplyFontColor;
		}
		public int GetValue(DifferentialFormat format) {
			return format.FontInfo.ColorIndex;
		}
	}
	public struct DifferentialFormatFontBoldAccessor : IDifferentialFormatPropertyAccessor<bool> {
		public bool GetApply(DifferentialFormat format) {
			return format.MultiOptionsInfo.ApplyFontBold;
		}
		public bool GetValue(DifferentialFormat format) {
			return format.FontInfo.Bold;
		}
	}
	public struct DifferentialFormatFontExtendAccessor : IDifferentialFormatPropertyAccessor<bool> {
		public bool GetApply(DifferentialFormat format) {
			return format.MultiOptionsInfo.ApplyFontExtend;
		}
		public bool GetValue(DifferentialFormat format) {
			return format.FontInfo.Extend;
		}
	}
	public struct DifferentialFormatFontCondenseAccessor : IDifferentialFormatPropertyAccessor<bool> {
		public bool GetApply(DifferentialFormat format) {
			return format.MultiOptionsInfo.ApplyFontCondense;
		}
		public bool GetValue(DifferentialFormat format) {
			return format.FontInfo.Condense;
		}
	}
	public struct DifferentialFormatFontItalicAccessor : IDifferentialFormatPropertyAccessor<bool> {
		public bool GetApply(DifferentialFormat format) {
			return format.MultiOptionsInfo.ApplyFontItalic;
		}
		public bool GetValue(DifferentialFormat format) {
			return format.FontInfo.Italic;
		}
	}
	public struct DifferentialFormatFontOutlineAccessor : IDifferentialFormatPropertyAccessor<bool> {
		public bool GetApply(DifferentialFormat format) {
			return format.MultiOptionsInfo.ApplyFontOutline;
		}
		public bool GetValue(DifferentialFormat format) {
			return format.FontInfo.Outline;
		}
	}
	public struct DifferentialFormatFontShadowAccessor : IDifferentialFormatPropertyAccessor<bool> {
		public bool GetApply(DifferentialFormat format) {
			return format.MultiOptionsInfo.ApplyFontShadow;
		}
		public bool GetValue(DifferentialFormat format) {
			return format.FontInfo.Shadow;
		}
	}
	public struct DifferentialFormatFontStrikeThroughAccessor : IDifferentialFormatPropertyAccessor<bool> {
		public bool GetApply(DifferentialFormat format) {
			return format.MultiOptionsInfo.ApplyFontStrikeThrough;
		}
		public bool GetValue(DifferentialFormat format) {
			return format.FontInfo.StrikeThrough;
		}
	}
	public struct DifferentialFormatFontCharsetAccessor : IDifferentialFormatPropertyAccessor<int> {
		public bool GetApply(DifferentialFormat format) {
			return format.MultiOptionsInfo.ApplyFontCharset;
		}
		public int GetValue(DifferentialFormat format) {
			return format.FontInfo.Charset;
		}
	}
	public struct DifferentialFormatFontFamilyAccessor : IDifferentialFormatPropertyAccessor<int> {
		public bool GetApply(DifferentialFormat format) {
			return format.MultiOptionsInfo.ApplyFontFamily;
		}
		public int GetValue(DifferentialFormat format) {
			return format.FontInfo.FontFamily;
		}
	}
	public struct DifferentialFormatFontSizeAccessor : IDifferentialFormatPropertyAccessor<double> {
		public bool GetApply(DifferentialFormat format) {
			return format.MultiOptionsInfo.ApplyFontSize;
		}
		public double GetValue(DifferentialFormat format) {
			return format.FontInfo.Size;
		}
	}
	public struct DifferentialFormatFontSchemeStyleAccessor : IDifferentialFormatPropertyAccessor<XlFontSchemeStyles> {
		public bool GetApply(DifferentialFormat format) {
			return format.MultiOptionsInfo.ApplyFontSchemeStyle;
		}
		public XlFontSchemeStyles GetValue(DifferentialFormat format) {
			return format.FontInfo.SchemeStyle;
		}
	}
	public struct DifferentialFormatFontScriptAccessor : IDifferentialFormatPropertyAccessor<XlScriptType> {
		public bool GetApply(DifferentialFormat format) {
			return format.MultiOptionsInfo.ApplyFontScript;
		}
		public XlScriptType GetValue(DifferentialFormat format) {
			return format.FontInfo.Script;
		}
	}
	public struct DifferentialFormatFontUnderlineAccessor : IDifferentialFormatPropertyAccessor<XlUnderlineType> {
		public bool GetApply(DifferentialFormat format) {
			return format.MultiOptionsInfo.ApplyFontUnderline;
		}
		public XlUnderlineType GetValue(DifferentialFormat format) {
			return format.FontInfo.Underline;
		}
	}
	#endregion
	#region DifferentialFormatFillAccessors
	public struct DifferentialFormatFillPatternTypeAccessor : IDifferentialFormatPropertyAccessor<XlPatternType> {
		public bool GetApply(DifferentialFormat format) {
			return format.MultiOptionsInfo.ApplyFillPatternType;
		}
		public XlPatternType GetValue(DifferentialFormat format) {
			return format.FillInfo.PatternType;
		}
	}
	public struct DifferentialFormatFillForeColorIndexAccessor : IDifferentialFormatPropertyAccessor<int> {
		public bool GetApply(DifferentialFormat format) {
			return format.MultiOptionsInfo.ApplyFillForeColor;
		}
		public int GetValue(DifferentialFormat format) {
			return format.FillInfo.ForeColorIndex;
		}
	}
	public struct DifferentialFormatFillBackColorIndexAccessor : IDifferentialFormatPropertyAccessor<int> {
		public bool GetApply(DifferentialFormat format) {
			return format.MultiOptionsInfo.ApplyFillBackColor;
		}
		public int GetValue(DifferentialFormat format) {
			return format.FillInfo.BackColorIndex;
		}
	}
	public struct DifferentialFormatFillTypeAccessor : IDifferentialFormatPropertyAccessor<ModelFillType> {
		public bool GetApply(DifferentialFormat format) {
			return format.CellFormatFlagsInfo.FillType == ModelFillType.Gradient;
		}
		public ModelFillType GetValue(DifferentialFormat format) {
			return format.CellFormatFlagsInfo.FillType;
		}
	}
	public struct DifferentialFormatFillGradientFillTypeAccessor : IDifferentialFormatPropertyAccessor<ModelGradientFillType> {
		public bool GetApply(DifferentialFormat format) {
			return format.CellFormatFlagsInfo.FillType == ModelFillType.Gradient;
		}
		public ModelGradientFillType GetValue(DifferentialFormat format) {
			return format.GradientFillInfo.Type;
		}
	}
	public struct DifferentialFormatFillGradientFillDegreeAccessor : IDifferentialFormatPropertyAccessor<double> {
		public bool GetApply(DifferentialFormat format) {
			return format.CellFormatFlagsInfo.FillType == ModelFillType.Gradient;
		}
		public double GetValue(DifferentialFormat format) {
			return format.GradientFillInfo.Degree;
		}
	}
	public struct DifferentialFormatFillGradientFillGradientStopsAccessor : IDifferentialFormatPropertyAccessor<GradientStopInfoCollection> {
		public bool GetApply(DifferentialFormat format) {
			return format.CellFormatFlagsInfo.FillType == ModelFillType.Gradient;
		}
		public GradientStopInfoCollection GetValue(DifferentialFormat format) {
			return format.GradientStopInfoCollection;
		}
	}
	public struct DifferentialFormatFillGradientFillCovergenceLeftAccessor : IDifferentialFormatPropertyAccessor<float> {
		public bool GetApply(DifferentialFormat format) {
			return format.CellFormatFlagsInfo.FillType == ModelFillType.Gradient;
		}
		public float GetValue(DifferentialFormat format) {
			return format.GradientFillInfo.Left;
		}
	}
	public struct DifferentialFormatFillGradientFillCovergenceRightAccessor : IDifferentialFormatPropertyAccessor<float> {
		public bool GetApply(DifferentialFormat format) {
			return format.CellFormatFlagsInfo.FillType == ModelFillType.Gradient;
		}
		public float GetValue(DifferentialFormat format) {
			return format.GradientFillInfo.Right;
		}
	}
	public struct DifferentialFormatFillGradientFillCovergenceTopAccessor : IDifferentialFormatPropertyAccessor<float> {
		public bool GetApply(DifferentialFormat format) {
			return format.CellFormatFlagsInfo.FillType == ModelFillType.Gradient;
		}
		public float GetValue(DifferentialFormat format) {
			return format.GradientFillInfo.Top;
		}
	}
	public struct DifferentialFormatFillGradientFillCovergenceBottomAccessor : IDifferentialFormatPropertyAccessor<float> {
		public bool GetApply(DifferentialFormat format) {
			return format.CellFormatFlagsInfo.FillType == ModelFillType.Gradient;
		}
		public float GetValue(DifferentialFormat format) {
			return format.GradientFillInfo.Bottom;
		}
	}
	public struct DifferentialFormatHasVisibleFillAccessor : IDifferentialFormatPropertyAccessorBase {
		public bool GetApply(DifferentialFormat format) {
			return format.HasVisibleFill;
		}
	}
	#endregion
	#region DifferentialFormatAlignmentAccessors
	public struct DifferentialFormatAlignmentHorizontalAccessor : IDifferentialFormatPropertyAccessor<XlHorizontalAlignment> {
		public bool GetApply(DifferentialFormat format) {
			return format.MultiOptionsInfo.ApplyAlignmentHorizontal;
		}
		public XlHorizontalAlignment GetValue(DifferentialFormat format) {
			return format.AlignmentInfo.HorizontalAlignment;
		}
	}
	public struct DifferentialFormatAlignmentVerticalAccessor : IDifferentialFormatPropertyAccessor<XlVerticalAlignment> {
		public bool GetApply(DifferentialFormat format) {
			return format.MultiOptionsInfo.ApplyAlignmentVertical;
		}
		public XlVerticalAlignment GetValue(DifferentialFormat format) {
			return format.AlignmentInfo.VerticalAlignment;
		}
	}
	public struct DifferentialFormatAlignmentIndentAccessor : IDifferentialFormatPropertyAccessor<byte> {
		public bool GetApply(DifferentialFormat format) {
			return format.MultiOptionsInfo.ApplyAlignmentIndent;
		}
		public byte GetValue(DifferentialFormat format) {
			return format.AlignmentInfo.Indent;
		}
	}
	public struct DifferentialFormatAlignmentRelativeIndentAccessor : IDifferentialFormatPropertyAccessor<int> {
		public bool GetApply(DifferentialFormat format) {
			return format.MultiOptionsInfo.ApplyAlignmentRelativeIndent;
		}
		public int GetValue(DifferentialFormat format) {
			return format.AlignmentInfo.RelativeIndent;
		}
	}
	public struct DifferentialFormatAlignmentReadingOrderAccessor : IDifferentialFormatPropertyAccessor<XlReadingOrder> {
		public bool GetApply(DifferentialFormat format) {
			return format.MultiOptionsInfo.ApplyAlignmentReadingOrder;
		}
		public XlReadingOrder GetValue(DifferentialFormat format) {
			return format.AlignmentInfo.ReadingOrder;
		}
	}
	public struct DifferentialFormatAlignmentTextRotationAccessor : IDifferentialFormatPropertyAccessor<int> {
		public bool GetApply(DifferentialFormat format) {
			return format.MultiOptionsInfo.ApplyAlignmentTextRotation;
		}
		public int GetValue(DifferentialFormat format) {
			return format.AlignmentInfo.TextRotation;
		}
	}
	public struct DifferentialFormatAlignmentWrapTextAccessor : IDifferentialFormatPropertyAccessor<bool> {
		public bool GetApply(DifferentialFormat format) {
			return format.MultiOptionsInfo.ApplyAlignmentWrapText;
		}
		public bool GetValue(DifferentialFormat format) {
			return format.AlignmentInfo.WrapText;
		}
	}
	public struct DifferentialFormatAlignmentJustifyLastLineAccessor : IDifferentialFormatPropertyAccessor<bool> {
		public bool GetApply(DifferentialFormat format) {
			return format.MultiOptionsInfo.ApplyAlignmentJustifyLastLine;
		}
		public bool GetValue(DifferentialFormat format) {
			return format.AlignmentInfo.JustifyLastLine;
		}
	}
	public struct DifferentialFormatAlignmentShrinkToFitAccessor : IDifferentialFormatPropertyAccessor<bool> {
		public bool GetApply(DifferentialFormat format) {
			return format.MultiOptionsInfo.ApplyAlignmentShrinkToFit;
		}
		public bool GetValue(DifferentialFormat format) {
			return format.AlignmentInfo.ShrinkToFit;
		}
	}
	#endregion
	#region DifferentialFormatProtectionAccessors
	public struct DifferentialFormatProtectionLockedAccessor : IDifferentialFormatPropertyAccessor<bool> {
		public bool GetApply(DifferentialFormat format) {
			return format.MultiOptionsInfo.ApplyProtectionLocked;
		}
		public bool GetValue(DifferentialFormat format) {
			return format.Protection.Locked;
		}
	}
	public struct DifferentialFormatProtectionHiddenAccessor : IDifferentialFormatPropertyAccessor<bool> {
		public bool GetApply(DifferentialFormat format) {
			return format.MultiOptionsInfo.ApplyProtectionHidden;
		}
		public bool GetValue(DifferentialFormat format) {
			return format.Protection.Hidden;
		}
	}
	#endregion
	#region DifferentialFormatBorderAccessors
	public struct DifferentialFormatBorderLeftLineStyleAccessor : IDifferentialFormatDisplayBorderPropertyAccessor<XlBorderLineStyle> {
		static CellPositionOffset nearbyOffset = new CellPositionOffset(-1, 0);
		public BorderIndex BorderIndex { get { return BorderIndex.Left; } }
		public IDifferentialFormatDisplayBorderPropertyAccessor<XlBorderLineStyle> NearbyAccessor { get { return DifferentialFormatPropertyAccessors.BorderRightLineStyle; } }
		public CellPositionOffset NearbyOffset { get { return nearbyOffset; } }
		public bool GetApply(BorderOptionsInfo info, bool isOutline) {
			return isOutline ? info.ApplyLeftLineStyle : info.ApplyVerticalLineStyle;
		}
		public XlBorderLineStyle GetValue(BorderInfo info, bool isOutline) {
			return isOutline ? info.LeftLineStyle : info.VerticalLineStyle;
		}
	}
	public struct DifferentialFormatBorderRightLineStyleAccessor : IDifferentialFormatDisplayBorderPropertyAccessor<XlBorderLineStyle> {
		static CellPositionOffset nearbyOffset = new CellPositionOffset(1, 0);
		public BorderIndex BorderIndex { get { return BorderIndex.Right; } }
		public IDifferentialFormatDisplayBorderPropertyAccessor<XlBorderLineStyle> NearbyAccessor { get { return DifferentialFormatPropertyAccessors.BorderLeftLineStyle; } }
		public CellPositionOffset NearbyOffset { get { return nearbyOffset; } }
		public bool GetApply(BorderOptionsInfo info, bool isOutline) {
			return isOutline ? info.ApplyRightLineStyle : info.ApplyVerticalLineStyle;
		}
		public XlBorderLineStyle GetValue(BorderInfo info, bool isOutline) {
			return isOutline ? info.RightLineStyle : info.VerticalLineStyle;
		}
	}
	public struct DifferentialFormatBorderTopLineStyleAccessor : IDifferentialFormatDisplayBorderPropertyAccessor<XlBorderLineStyle> {
		static CellPositionOffset nearbyOffset = new CellPositionOffset(0, -1);
		public BorderIndex BorderIndex { get { return BorderIndex.Top; } }
		public IDifferentialFormatDisplayBorderPropertyAccessor<XlBorderLineStyle> NearbyAccessor { get { return DifferentialFormatPropertyAccessors.BorderBottomLineStyle; } }
		public CellPositionOffset NearbyOffset { get { return nearbyOffset; } }
		public bool GetApply(BorderOptionsInfo info, bool isOutline) {
			return isOutline ? info.ApplyTopLineStyle : info.ApplyHorizontalLineStyle;
		}
		public XlBorderLineStyle GetValue(BorderInfo info, bool isOutline) {
			return isOutline ? info.TopLineStyle : info.HorizontalLineStyle;
		}
	}
	public struct DifferentialFormatBorderBottomLineStyleAccessor : IDifferentialFormatDisplayBorderPropertyAccessor<XlBorderLineStyle> {
		static CellPositionOffset nearbyOffset = new CellPositionOffset(0, 1);
		public BorderIndex BorderIndex { get { return BorderIndex.Bottom; } }
		public IDifferentialFormatDisplayBorderPropertyAccessor<XlBorderLineStyle> NearbyAccessor { get { return DifferentialFormatPropertyAccessors.BorderTopLineStyle; } }
		public CellPositionOffset NearbyOffset { get { return nearbyOffset; } }
		public bool GetApply(BorderOptionsInfo info, bool isOutline) {
		   return isOutline ? info.ApplyBottomLineStyle : info.ApplyHorizontalLineStyle;
		}
		public XlBorderLineStyle GetValue(BorderInfo info, bool isOutline) {
			return isOutline ? info.BottomLineStyle : info.HorizontalLineStyle;
		}
	}
	public struct DifferentialFormatBorderDiagonalLineStyleAccessor : IDifferentialFormatPropertyAccessor<XlBorderLineStyle> {
		public bool GetApply(DifferentialFormat format) {
			return format.BorderOptionsInfo.ApplyDiagonalLineStyle;
		}
		public XlBorderLineStyle GetValue(DifferentialFormat format) {
			return format.BorderInfo.DiagonalLineStyle;
		}
	}
	public struct DifferentialFormatBorderDiagonalUpLineStyleAccessor : IDifferentialFormatPropertyAccessor<XlBorderLineStyle> {
		public bool GetApply(DifferentialFormat format) {
			return format.BorderOptionsInfo.ApplyDiagonalLineStyle;
		}
		public XlBorderLineStyle GetValue(DifferentialFormat format) {
			return format.BorderInfo.DiagonalUpLineStyle;
		}
	}
	public struct DifferentialFormatBorderDiagonalDownLineStyleAccessor : IDifferentialFormatPropertyAccessor<XlBorderLineStyle> {
		public bool GetApply(DifferentialFormat format) {
			return format.BorderOptionsInfo.ApplyDiagonalLineStyle;
		}
		public XlBorderLineStyle GetValue(DifferentialFormat format) {
			return format.BorderInfo.DiagonalDownLineStyle;
		}
	}
	public struct DifferentialFormatBorderHorizontalLineStyleAccessor : IDifferentialFormatPropertyAccessor<XlBorderLineStyle> {
		public bool GetApply(DifferentialFormat format) {
			return format.BorderOptionsInfo.ApplyHorizontalLineStyle;
		}
		public XlBorderLineStyle GetValue(DifferentialFormat format) {
			return format.BorderInfo.HorizontalLineStyle;
		}
	}
	public struct DifferentialFormatBorderVerticalLineStyleAccessor : IDifferentialFormatPropertyAccessor<XlBorderLineStyle> {
		public bool GetApply(DifferentialFormat format) {
			return format.BorderOptionsInfo.ApplyVerticalLineStyle;
		}
		public XlBorderLineStyle GetValue(DifferentialFormat format) {
			return format.BorderInfo.VerticalLineStyle;
		}
	}
	public struct DifferentialFormatBorderDiagonalUpAccessor : IDifferentialFormatPropertyAccessor<bool> {
		public bool GetApply(DifferentialFormat format) {
			return format.BorderOptionsInfo.ApplyDiagonalUp;
		}
		public bool GetValue(DifferentialFormat format) {
			return format.BorderInfo.DiagonalUp;
		}
	}
	public struct DifferentialFormatBorderDiagonalDownAccessor : IDifferentialFormatPropertyAccessor<bool> {
		public bool GetApply(DifferentialFormat format) {
			return format.BorderOptionsInfo.ApplyDiagonalDown;
		}
		public bool GetValue(DifferentialFormat format) {
			return format.BorderInfo.DiagonalDown;
		}
	}
	public struct DifferentialFormatBorderOutlineAccessor : IDifferentialFormatPropertyAccessor<bool> {
		public bool GetApply(DifferentialFormat format) {
			return format.BorderOptionsInfo.ApplyOutline;
		}
		public bool GetValue(DifferentialFormat format) {
			return format.BorderInfo.Outline;
		}
	}
	public struct DifferentialFormatBorderLeftColorIndexAccessor : IDifferentialFormatDisplayBorderPropertyAccessor<int> {
		static CellPositionOffset nearbyOffset = new CellPositionOffset(-1, 0);
		public BorderIndex BorderIndex { get { return BorderIndex.Left; } }
		public IDifferentialFormatDisplayBorderPropertyAccessor<int> NearbyAccessor { get { return DifferentialFormatPropertyAccessors.BorderRightColorIndex; } }
		public CellPositionOffset NearbyOffset { get { return nearbyOffset; } }
		public bool GetApply(BorderOptionsInfo info, bool isOutline) {
			return isOutline ? info.ApplyLeftColor : info.ApplyVerticalColor;
		}
		public int GetValue(BorderInfo info, bool isOutline) {
			return isOutline ? info.LeftColorIndex : info.VerticalColorIndex;
		}
	}
	public struct DifferentialFormatBorderRightColorIndexAccessor : IDifferentialFormatDisplayBorderPropertyAccessor<int> {
		static CellPositionOffset nearbyOffset = new CellPositionOffset(1, 0);
		public BorderIndex BorderIndex { get { return BorderIndex.Right; } }
		public IDifferentialFormatDisplayBorderPropertyAccessor<int> NearbyAccessor { get { return DifferentialFormatPropertyAccessors.BorderLeftColorIndex; } }
		public CellPositionOffset NearbyOffset { get { return nearbyOffset; } }
		public bool GetApply(BorderOptionsInfo info, bool isOutline) {
			return isOutline ? info.ApplyRightColor : info.ApplyVerticalColor;
		}
		public int GetValue(BorderInfo info, bool isOutline) {
			return isOutline ? info.RightColorIndex : info.VerticalColorIndex;
		}
	}
	public struct DifferentialFormatBorderTopColorIndexAccessor : IDifferentialFormatDisplayBorderPropertyAccessor<int> {
		static CellPositionOffset nearbyOffset = new CellPositionOffset(0, -1);
		public BorderIndex BorderIndex { get { return BorderIndex.Top; } }
		public IDifferentialFormatDisplayBorderPropertyAccessor<int> NearbyAccessor { get { return DifferentialFormatPropertyAccessors.BorderBottomColorIndex; } }
		public CellPositionOffset NearbyOffset { get { return nearbyOffset; } }
		public bool GetApply(BorderOptionsInfo info, bool isOutline) {
			return isOutline ? info.ApplyTopColor : info.ApplyHorizontalColor;
		}
		public int GetValue(BorderInfo info, bool isOutline) {
			return isOutline ? info.TopColorIndex : info.HorizontalColorIndex;
		}
	}
	public struct DifferentialFormatBorderBottomColorIndexAccessor : IDifferentialFormatDisplayBorderPropertyAccessor<int> {
		static CellPositionOffset nearbyOffset = new CellPositionOffset(0, 1);
		public BorderIndex BorderIndex { get { return BorderIndex.Bottom; } }
		public IDifferentialFormatDisplayBorderPropertyAccessor<int> NearbyAccessor { get { return DifferentialFormatPropertyAccessors.BorderTopColorIndex; } }
		public CellPositionOffset NearbyOffset { get { return nearbyOffset; } }
		public bool GetApply(BorderOptionsInfo info, bool isOutline) {
			return isOutline ? info.ApplyBottomColor : info.ApplyHorizontalColor;
		}
		public int GetValue(BorderInfo info, bool isOutline) {
			return isOutline ? info.BottomColorIndex : info.HorizontalColorIndex;
		}
	}
	public struct DifferentialFormatBorderHorizontalColorIndexAccessor : IDifferentialFormatPropertyAccessor<int> {
		public bool GetApply(DifferentialFormat format) {
			return format.BorderOptionsInfo.ApplyHorizontalColor;
		}
		public int GetValue(DifferentialFormat format) {
			return format.Border.HorizontalColorIndex;
		}
	}
	public struct DifferentialFormatBorderVerticalColorIndexAccessor : IDifferentialFormatPropertyAccessor<int> {
		public bool GetApply(DifferentialFormat format) {
			return format.BorderOptionsInfo.ApplyVerticalColor;
		}
		public int GetValue(DifferentialFormat format) {
			return format.Border.VerticalColorIndex;
		}
	}
	public struct DifferentialFormatBorderDiagonalColorIndexAccessor : IDifferentialFormatPropertyAccessor<int> {
		public bool GetApply(DifferentialFormat format) {
			return format.BorderOptionsInfo.ApplyDiagonalColor;
		}
		public int GetValue(DifferentialFormat format) {
			return format.Border.DiagonalColorIndex;
		}
	}
	public struct DifferentialFormatActualLeftBorderInfoAccessor : IDifferentialFormatDisplayBorderPropertyAccessor<ActualBorderElementInfo> {
		static CellPositionOffset nearbyOffset = new CellPositionOffset(-1, 0);
		public BorderIndex BorderIndex { get { return BorderIndex.Left; } }
		public IDifferentialFormatDisplayBorderPropertyAccessor<ActualBorderElementInfo> NearbyAccessor { get { return DifferentialFormatPropertyAccessors.BorderRightInfo; } }
		public CellPositionOffset NearbyOffset { get { return nearbyOffset; } }
		public ActualBorderElementInfo GetValue(BorderInfo borderInfo, bool isOutline) {
			return new ActualBorderElementInfo(borderInfo.LeftLineStyle, borderInfo.LeftColorIndex);
		}
		public bool GetApply(BorderOptionsInfo info, bool isOutline) {
			return isOutline ? info.ApplyLeftLineStyle : info.ApplyVerticalLineStyle;
		}
	}
	public struct DifferentialFormatActualRightBorderInfoAccessor : IDifferentialFormatDisplayBorderPropertyAccessor<ActualBorderElementInfo> {
		static CellPositionOffset nearbyOffset = new CellPositionOffset(1, 0);
		public BorderIndex BorderIndex { get { return BorderIndex.Right; } }
		public IDifferentialFormatDisplayBorderPropertyAccessor<ActualBorderElementInfo> NearbyAccessor { get { return DifferentialFormatPropertyAccessors.BorderLeftInfo; } }
		public CellPositionOffset NearbyOffset { get { return nearbyOffset; } }
		public ActualBorderElementInfo GetValue(BorderInfo borderInfo, bool isOutline) {
			return new ActualBorderElementInfo(borderInfo.RightLineStyle, borderInfo.RightColorIndex);
		}
		public bool GetApply(BorderOptionsInfo info, bool isOutline) {
			return isOutline ? info.ApplyRightLineStyle : info.ApplyVerticalLineStyle;
		}
	}
	public struct DifferentialFormatActualTopBorderInfoAccessor : IDifferentialFormatDisplayBorderPropertyAccessor<ActualBorderElementInfo> {
		static CellPositionOffset nearbyOffset = new CellPositionOffset(0, -1);
		public BorderIndex BorderIndex { get { return BorderIndex.Top; } }
		public IDifferentialFormatDisplayBorderPropertyAccessor<ActualBorderElementInfo> NearbyAccessor { get { return DifferentialFormatPropertyAccessors.BorderBottomInfo; } }
		public CellPositionOffset NearbyOffset { get { return nearbyOffset; } }
		public ActualBorderElementInfo GetValue(BorderInfo borderInfo, bool isOutline) {
			return new ActualBorderElementInfo(borderInfo.TopLineStyle, borderInfo.TopColorIndex);
		}
		public bool GetApply(BorderOptionsInfo info, bool isOutline) {
			return isOutline ? info.ApplyTopLineStyle : info.ApplyHorizontalLineStyle;
		}
	}
	public struct DifferentialFormatActualBottomBorderInfoAccessor : IDifferentialFormatDisplayBorderPropertyAccessor<ActualBorderElementInfo> {
		static CellPositionOffset nearbyOffset = new CellPositionOffset(0, 1);
		public BorderIndex BorderIndex { get { return BorderIndex.Bottom; } }
		public IDifferentialFormatDisplayBorderPropertyAccessor<ActualBorderElementInfo> NearbyAccessor { get { return DifferentialFormatPropertyAccessors.BorderTopInfo; } }
		public CellPositionOffset NearbyOffset { get { return nearbyOffset; } }
		public ActualBorderElementInfo GetValue(BorderInfo borderInfo, bool isOutline) {
			return new ActualBorderElementInfo(borderInfo.BottomLineStyle, borderInfo.BottomColorIndex);
		}
		public bool GetApply(BorderOptionsInfo info, bool isOutline) {
			return isOutline ? info.ApplyBottomLineStyle : info.ApplyHorizontalLineStyle;
		}
	}
	#endregion
	#endregion
}
