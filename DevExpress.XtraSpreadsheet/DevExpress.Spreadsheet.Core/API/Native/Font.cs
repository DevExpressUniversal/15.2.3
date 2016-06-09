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
#else
using System.Windows.Media;
#endif
namespace DevExpress.Spreadsheet {
	#region SpreadsheetFont
	public interface SpreadsheetFont {
		SpreadsheetFontStyle FontStyle { get; set; }
		bool Bold { get; set; }
		bool Italic { get; set; }
		string Name { get; set; }
		ScriptType Script { get; set; }
		double Size { get; set; }
		UnderlineType UnderlineType { get; set; }
		bool Strikethrough { get; set; }
		Color Color { get; set; }
	}
	#endregion
	#region ScriptType
	public enum ScriptType {
		None = XlScriptType.Baseline,
		Superscript = XlScriptType.Superscript,
		Subscript = XlScriptType.Subscript
	}
	#endregion
	#region UnderlineType
	public enum UnderlineType {
		None = XlUnderlineType.None,
		Single = XlUnderlineType.Single,
		Double = XlUnderlineType.Double,
		SingleAccounting = XlUnderlineType.SingleAccounting,
		DoubleAccounting = XlUnderlineType.DoubleAccounting
	}
	#endregion
	#region FontStyle
	public enum SpreadsheetFontStyle {
		Regular = 0,
		Italic = 1,
		Bold = 2,
		BoldItalic = 3
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Spreadsheet;
	#region NativeFont
	partial class NativeFont : SpreadsheetFont {
		IBatchUpdateable owner;
		Model.IRunFontInfo fontInfo;
		public NativeFont(IBatchUpdateable owner, Model.IRunFontInfo fontInfo) {
			this.owner = owner;
			this.fontInfo = fontInfo;
		}
		Model.IRunFontInfo FontInfo { get { return fontInfo; } }
		#region FontStyle
		public SpreadsheetFontStyle FontStyle {
			get {
				if (FontInfo.Bold && FontInfo.Italic)
					return DevExpress.Spreadsheet.SpreadsheetFontStyle.BoldItalic;
				if (FontInfo.Bold && !FontInfo.Italic)
					return DevExpress.Spreadsheet.SpreadsheetFontStyle.Bold;
				if (!FontInfo.Bold && FontInfo.Italic)
					return DevExpress.Spreadsheet.SpreadsheetFontStyle.Italic;
				return DevExpress.Spreadsheet.SpreadsheetFontStyle.Regular;
			}
			set {
				owner.BeginUpdate();
				try {
					switch (value) {
						case DevExpress.Spreadsheet.SpreadsheetFontStyle.Bold:
							FontInfo.Bold = true;
							FontInfo.Italic = false;
							break;
						case DevExpress.Spreadsheet.SpreadsheetFontStyle.BoldItalic:
							FontInfo.Bold = true;
							FontInfo.Italic = true;
							break;
						case DevExpress.Spreadsheet.SpreadsheetFontStyle.Italic:
							FontInfo.Bold = false;
							FontInfo.Italic = true;
							break;
						default:
							FontInfo.Bold = false;
							FontInfo.Italic = false;
							break;
					}
				}
				finally {
					owner.EndUpdate();
				}
			}
		}
		#endregion
		public bool Bold { get { return FontInfo.Bold; } set { FontInfo.Bold = value; } }
		public bool Italic { get { return FontInfo.Italic; } set { FontInfo.Italic = value; } }
		#region Name
		public string Name { get { return FontInfo.Name; } set { FontInfo.Name = value; } }
		#endregion
		#region Outline
		public bool Outline {
			get {
				return FontInfo.Outline;
			}
			set {
				FontInfo.Outline = value;
			}
		}
		#endregion
		#region Script
		public ScriptType Script {
			get {
				return (ScriptType)FontInfo.Script;
			}
			set {
				FontInfo.Script = (XlScriptType)value;
			}
		}
		#endregion
		#region Size
		public double Size {
			get {
				return FontInfo.Size;
			}
			set {
				FontInfo.Size = value;
			}
		}
		#endregion
		#region UnderlineType
		public UnderlineType UnderlineType {
			get {
				return (UnderlineType)FontInfo.Underline;
			}
			set {
				FontInfo.Underline = (XlUnderlineType)(value);
			}
		}
		#endregion
		#region StrikeThrough
		public bool Strikethrough {
			get {
				return FontInfo.StrikeThrough;
			}
			set {
				FontInfo.StrikeThrough = value;
			}
		}
		#endregion
		#region Color
		public Color Color {
			get {
				return FontInfo.Color;
			}
			set {
				FontInfo.Color = value;
			}
		}
		#endregion
	}
	#endregion
	#region ActualNativeFont
	partial class NativeActualFont : SpreadsheetFont {
		readonly IFormatBaseAccessor ownerFormatAccessor;
		public NativeActualFont(IFormatBaseAccessor formatAccessor) {
			this.ownerFormatAccessor = formatAccessor;
		}
		Model.IFormatBaseBatchUpdateable ReadOnlyOwnerFormat { get { return ownerFormatAccessor.ReadOnlyFormat; } }
		Model.IFormatBaseBatchUpdateable ReadWriteOwnerFormat { get { return ownerFormatAccessor.ReadWriteFormat; } }
		Model.IRunFontInfo ReadWriteFontInfo { get { return ReadWriteOwnerFormat.Font; } }
		Model.IActualRunFontInfo ReadOnlyActualFontInfo { get { return ReadOnlyOwnerFormat.ActualFont; } }
		#region FontStyle
		public SpreadsheetFontStyle FontStyle {
			get {
				if (ReadOnlyActualFontInfo.Bold && ReadOnlyActualFontInfo.Italic)
					return DevExpress.Spreadsheet.SpreadsheetFontStyle.BoldItalic;
				if (ReadOnlyActualFontInfo.Bold && !ReadOnlyActualFontInfo.Italic)
					return DevExpress.Spreadsheet.SpreadsheetFontStyle.Bold;
				if (!ReadOnlyActualFontInfo.Bold && ReadOnlyActualFontInfo.Italic)
					return DevExpress.Spreadsheet.SpreadsheetFontStyle.Italic;
				return DevExpress.Spreadsheet.SpreadsheetFontStyle.Regular;
			}
			set {
				ReadWriteOwnerFormat.BeginUpdate();
				try {
					switch (value) {
						case DevExpress.Spreadsheet.SpreadsheetFontStyle.Bold:
							ReadWriteFontInfo.Bold = true;
							ReadWriteFontInfo.Italic = false;
							break;
						case DevExpress.Spreadsheet.SpreadsheetFontStyle.BoldItalic:
							ReadWriteFontInfo.Bold = true;
							ReadWriteFontInfo.Italic = true;
							break;
						case DevExpress.Spreadsheet.SpreadsheetFontStyle.Italic:
							ReadWriteFontInfo.Bold = false;
							ReadWriteFontInfo.Italic = true;
							break;
						default:
							ReadWriteFontInfo.Bold = false;
							ReadWriteFontInfo.Italic = false;
							break;
					}
				}
				finally {
					ReadWriteOwnerFormat.EndUpdate();
				}
			}
		}
		#endregion
		#region Bold
		public bool Bold {
			get {
				return ReadOnlyActualFontInfo.Bold;
			}
			set {
				ReadWriteFontInfo.Bold = value;
			}
		}
		#endregion
		#region Italic
		public bool Italic {
			get {
				return ReadOnlyActualFontInfo.Italic;
			}
			set {
				ReadWriteFontInfo.Italic = value;
			}
		}
		#endregion
		#region Name
		public string Name { get { return ReadOnlyActualFontInfo.Name; } set { ReadWriteFontInfo.Name = value; } }
		#endregion
		#region Outline
		public bool Outline {
			get {
				return ReadOnlyActualFontInfo.Outline;
			}
			set {
				ReadWriteFontInfo.Outline = value;
			}
		}
		#endregion
		#region Script
		public ScriptType Script {
			get {
				return (ScriptType)ReadOnlyActualFontInfo.Script;
			}
			set {
				ReadWriteFontInfo.Script = (XlScriptType)value;
			}
		}
		#endregion
		#region Size
		public double Size {
			get {
				return ReadOnlyActualFontInfo.Size;
			}
			set {
				ReadWriteFontInfo.Size = value;
			}
		}
		#endregion
		#region UnderlineType
		public UnderlineType UnderlineType {
			get {
				return (UnderlineType)ReadOnlyActualFontInfo.Underline;
			}
			set {
				ReadWriteFontInfo.Underline = (XlUnderlineType)(value);
			}
		}
		#endregion
		#region StrikeThrough
		public bool Strikethrough {
			get {
				return ReadOnlyActualFontInfo.StrikeThrough;
			}
			set {
				ReadWriteFontInfo.StrikeThrough = value;
			}
		}
		#endregion
		#region Color
		public Color Color {
			get {
				return ReadOnlyActualFontInfo.Color;
			}
			set {
				ReadWriteFontInfo.Color = value;
			}
		}
		#endregion
	}
	#endregion
	#region FontStyleConverter
	public static class FontStyleConverter {
		public static bool IsBold(SpreadsheetFontStyle fontStyle) {
			return fontStyle == SpreadsheetFontStyle.Bold || fontStyle == SpreadsheetFontStyle.BoldItalic;
		}
		public static SpreadsheetFontStyle SetBold(SpreadsheetFontStyle fontStyle, bool newBoldValue) {
			int packedValue = (int)fontStyle;
			return (SpreadsheetFontStyle)SetValue(packedValue, 2, newBoldValue);
		}
		public static bool IsItalic(SpreadsheetFontStyle fontStyle) {
			return fontStyle == SpreadsheetFontStyle.Italic || fontStyle == SpreadsheetFontStyle.BoldItalic;
		}
		public static SpreadsheetFontStyle SetItalic(SpreadsheetFontStyle fontStyle, bool newItalicValue) {
			int packedValue = (int)fontStyle;
			return (SpreadsheetFontStyle)SetValue(packedValue, 1, newItalicValue);
		}
		static int SetValue(int packedValue, int mask, bool flag) {
			if (flag) { return packedValue |= mask; } else { return packedValue &= ~mask; }
		}
	}
	#endregion
}
