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
using System.Runtime.InteropServices;
using DevExpress.Office;
using DevExpress.Office.Drawing;
using DevExpress.Utils;
using DevExpress.Export.Xl;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Office.Model;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model {
	#region Cell : IRunFontInfo
	partial class Cell {
		ushort fontKeyIndex = 0;
		protected internal FontInfoKey FontKey { get { return DocumentModel.Cache.FontKeyCache[fontKeyIndex]; } }
		public IRunFontInfo Font { get { return this; } }
		public IActualRunFontInfo ActualFont { get { return this; } }  
		public virtual IActualRunFontInfo InnerActualFont { get { return FormatInfo.ActualFont; } }
		public short ActualFontInfoIndex {
			get {
				return (short)(((packedFormat & actualFontInfoIndexMask) >> actualFontInfoIndexOffset) - 1);
			}
			set {
				packedFormat &= ~actualFontInfoIndexMask;
				packedFormat |= ((uint)(value + 1) << actualFontInfoIndexOffset) & actualFontInfoIndexMask;
			}
		}
		#region IRunFontInfo Members
		#region IRunFontInfo.Name
		string IRunFontInfo.Name {
			get { return FormatInfo.Font.Name; }
			set {
				SetFontPropertyValue(SetFontName, value);
			}
		}
		DocumentModelChangeActions SetFontName(FormatBase info, string value) {
			info.Font.Name = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.Color
		Color IRunFontInfo.Color {
			get { return FormatInfo.Font.Color; }
			set {
				SetFontPropertyValue(SetFontColor, value);
			}
		}
		DocumentModelChangeActions SetFontColor(FormatBase info, Color value) {
			info.Font.Color = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.Bold
		bool IRunFontInfo.Bold {
			get { return FormatInfo.Font.Bold; }
			set {
				SetFontPropertyValue(SetFontBold, value);
			}
		}
		DocumentModelChangeActions SetFontBold(FormatBase info, bool value) {
			info.Font.Bold = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.Condense
		bool IRunFontInfo.Condense {
			get { return FormatInfo.Font.Condense; }
			set {
				SetFontPropertyValue(SetFontCondense, value);
			}
		}
		DocumentModelChangeActions SetFontCondense(FormatBase info, bool value) {
			info.Font.Condense = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.Extend
		bool IRunFontInfo.Extend {
			get { return FormatInfo.Font.Extend; }
			set {
				SetFontPropertyValue(SetFontExtend, value);
			}
		}
		DocumentModelChangeActions SetFontExtend(FormatBase info, bool value) {
			info.Font.Extend = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.Italic
		bool IRunFontInfo.Italic {
			get { return FormatInfo.Font.Italic; }
			set {
				SetFontPropertyValue(SetFontItalic, value);
			}
		}
		DocumentModelChangeActions SetFontItalic(FormatBase info, bool value) {
			info.Font.Italic = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.Outline
		bool IRunFontInfo.Outline {
			get { return FormatInfo.Font.Outline; }
			set {
				SetFontPropertyValue(SetFontOutline, value);
			}
		}
		DocumentModelChangeActions SetFontOutline(FormatBase info, bool value) {
			info.Font.Outline = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.Shadow
		bool IRunFontInfo.Shadow {
			get { return FormatInfo.Font.Shadow; }
			set {
				SetFontPropertyValue(SetFontShadow, value);
			}
		}
		DocumentModelChangeActions SetFontShadow(FormatBase info, bool value) {
			info.Font.Shadow = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.StrikeThrough
		bool IRunFontInfo.StrikeThrough {
			get { return FormatInfo.Font.StrikeThrough; }
			set {
				SetFontPropertyValue(SetFontStrikeThrough, value);
			}
		}
		DocumentModelChangeActions SetFontStrikeThrough(FormatBase info, bool value) {
			info.Font.StrikeThrough = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.Charset
		int IRunFontInfo.Charset {
			get { return FormatInfo.Font.Charset; }
			set {
				SetFontPropertyValue(SetFontCharset, value);
			}
		}
		DocumentModelChangeActions SetFontCharset(FormatBase info, int value) {
			info.Font.Charset = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.FontFamily
		int IRunFontInfo.FontFamily {
			get { return FormatInfo.Font.FontFamily; }
			set {
				SetFontPropertyValue(SetFontFamily, value);
			}
		}
		DocumentModelChangeActions SetFontFamily(FormatBase info, int value) {
			info.Font.FontFamily = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.Size
		double IRunFontInfo.Size {
			get { return FormatInfo.Font.Size; }
			set {
				SetFontPropertyValue(SetFontSize, value);
			}
		}
		DocumentModelChangeActions SetFontSize(FormatBase info, double value) {
			info.Font.Size = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.SchemeStyle
		XlFontSchemeStyles IRunFontInfo.SchemeStyle {
			get { return FormatInfo.Font.SchemeStyle; }
			set {
				SetFontPropertyValue(SetFontSchemeStyle, value);
			}
		}
		DocumentModelChangeActions SetFontSchemeStyle(FormatBase info, XlFontSchemeStyles value) {
			info.Font.SchemeStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.Script
		XlScriptType IRunFontInfo.Script {
			get { return FormatInfo.Font.Script; }
			set {
				SetFontPropertyValue(SetFontScript, value);
			}
		}
		DocumentModelChangeActions SetFontScript(FormatBase info, XlScriptType value) {
			info.Font.Script = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.Underline
		XlUnderlineType IRunFontInfo.Underline {
			get { return FormatInfo.Font.Underline; }
			set {
				SetFontPropertyValue(SetFontUnderline, value);
			}
		}
		DocumentModelChangeActions SetFontUnderline(FormatBase info, XlUnderlineType value) {
			info.Font.Underline = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		protected internal virtual void SetFontPropertyValue<U>(SetPropertyValueDelegate<U> setter, U newValue) {
			DocumentModel.BeginUpdate();
			try {
				FormatBase info = GetInfoForModification();
				info.SetActualFont(ActualFont);
				DocumentModelChangeActions changeActions = setter(info, newValue);
				ReplaceInfo(info, changeActions);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		#endregion
		#region IActualRunFontInfo Members
		string IActualRunFontInfo.Name { get { return GetActualFontName(ActualApplyInfo.ApplyFont); } }
		Color IActualRunFontInfo.Color {
			get {
				if (conditionalFormatAccumulator != null) {
					conditionalFormatAccumulator.Update(this);
					if (conditionalFormatAccumulator.ColorModified)
						return conditionalFormatAccumulator.Color;
				}
				return GetRgbColor(GetActualFontColorIndex(ActualApplyInfo.ApplyFont));
			}
		}
		int IActualRunFontInfo.ColorIndex { get { return GetActualFontColorIndex(ActualApplyInfo.ApplyFont); } }
		bool IActualRunFontInfo.Bold {
			get {
				if (conditionalFormatAccumulator != null) {
					conditionalFormatAccumulator.Update(this);
					if (conditionalFormatAccumulator.BoldAndItalicModified)
						return conditionalFormatAccumulator.Bold;
				}
				return GetActualFontBold(ActualApplyInfo.ApplyFont);
			}
		}
		bool IActualRunFontInfo.Condense { get { return GetActualFontCondense(ActualApplyInfo.ApplyFont); } }
		bool IActualRunFontInfo.Extend { get { return GetActualFontExtend(ActualApplyInfo.ApplyFont); } }
		bool IActualRunFontInfo.Italic {
			get {
				if (conditionalFormatAccumulator != null) {
					conditionalFormatAccumulator.Update(this);
					if (conditionalFormatAccumulator.BoldAndItalicModified)
						return conditionalFormatAccumulator.Italic;
				}
				return GetActualFontItalic(ActualApplyInfo.ApplyFont);
			}
		}
		bool IActualRunFontInfo.Outline { get { return GetActualFontOutline(ActualApplyInfo.ApplyFont); } }
		bool IActualRunFontInfo.Shadow { get { return GetActualFontShadow(ActualApplyInfo.ApplyFont); } }
		bool IActualRunFontInfo.StrikeThrough {
			get {
				if (conditionalFormatAccumulator != null) {
					conditionalFormatAccumulator.Update(this);
					if (conditionalFormatAccumulator.StrikeThroughModified)
						return conditionalFormatAccumulator.StrikeThrough;
				}
				return GetActualFontStrikeThrough(ActualApplyInfo.ApplyFont);
			}
		}
		int IActualRunFontInfo.Charset { get { return GetActualFontCharset(ActualApplyInfo.ApplyFont); } }
		int IActualRunFontInfo.FontFamily { get { return GetActualFontFamily(ActualApplyInfo.ApplyFont); } }
		double IActualRunFontInfo.Size { get { return GetActualFontSize(ActualApplyInfo.ApplyFont); } }
		XlFontSchemeStyles IActualRunFontInfo.SchemeStyle { get { return GetActualFontSchemeStyle(ActualApplyInfo.ApplyFont); } }
		XlScriptType IActualRunFontInfo.Script { get { return GetActualFontScript(ActualApplyInfo.ApplyFont); } }
		XlUnderlineType IActualRunFontInfo.Underline {
			get {
				if (conditionalFormatAccumulator != null) {
					conditionalFormatAccumulator.Update(this);
					if (conditionalFormatAccumulator.UnderlineModified)
						return conditionalFormatAccumulator.Underline;
				}
				return GetActualFontUnderline(ActualApplyInfo.ApplyFont);
			}
		}
		FontInfo IActualRunFontInfo.GetFontInfo() {
			return GetActualFontInfo();
		}
		#endregion
		#region GetActualFontValue
		protected T GetActualFontValue<T>(T cellFormatActualValue, bool actualApplyFont, DifferentialFormatPropertyDescriptor propertyDescriptor) {
			return GetActualFormatValue(cellFormatActualValue, actualApplyFont, propertyDescriptor);
		}
		protected virtual string GetActualFontName(bool actualApplyFont) {
			return GetActualFontValue(InnerActualFont.Name, actualApplyFont, DifferentialFormatPropertyDescriptor.FontName);
		}
		protected virtual int GetActualFontColorIndex(bool actualApplyFont) {
			int colorIndex = InnerActualFont.ColorIndex;
			return GetActualFontValue(colorIndex, actualApplyFont && GetColorModelInfo(colorIndex).ColorType != ColorType.Auto, DifferentialFormatPropertyDescriptor.FontColorIndex);
		}
		protected virtual bool GetActualFontBold(bool actualApplyFont) {
			return GetActualFontValue(InnerActualFont.Bold, actualApplyFont, DifferentialFormatPropertyDescriptor.FontBold);
		}
		protected virtual bool GetActualFontCondense(bool actualApplyFont) {
			return GetActualFontValue(InnerActualFont.Condense, actualApplyFont, DifferentialFormatPropertyDescriptor.FontCondense);
		}
		protected virtual bool GetActualFontExtend(bool actualApplyFont) {
			return GetActualFontValue(InnerActualFont.Extend, actualApplyFont, DifferentialFormatPropertyDescriptor.FontExtend);
		}
		protected virtual bool GetActualFontItalic(bool actualApplyFont) {
			return GetActualFontValue(InnerActualFont.Italic, actualApplyFont, DifferentialFormatPropertyDescriptor.FontItalic);
		}
		protected virtual bool GetActualFontOutline(bool actualApplyFont) {
			return GetActualFontValue(InnerActualFont.Outline, actualApplyFont, DifferentialFormatPropertyDescriptor.FontOutline);
		}
		protected virtual bool GetActualFontShadow(bool actualApplyFont) {
			return GetActualFontValue(InnerActualFont.Shadow, actualApplyFont, DifferentialFormatPropertyDescriptor.FontShadow);
		}
		protected virtual bool GetActualFontStrikeThrough(bool actualApplyFont) {
			return GetActualFontValue(InnerActualFont.StrikeThrough, actualApplyFont, DifferentialFormatPropertyDescriptor.FontStrikeThrough);
		}
		protected virtual int GetActualFontCharset(bool actualApplyFont) {
			return GetActualFontValue(InnerActualFont.Charset, actualApplyFont, DifferentialFormatPropertyDescriptor.FontCharset);
		}
		protected virtual int GetActualFontFamily(bool actualApplyFont) {
			return GetActualFontValue(InnerActualFont.FontFamily, actualApplyFont, DifferentialFormatPropertyDescriptor.FontFamily);
		}
		protected virtual double GetActualFontSize(bool actualApplyFont) {
			return GetActualFontValue(InnerActualFont.Size, actualApplyFont, DifferentialFormatPropertyDescriptor.FontSize);
		}
		protected virtual XlFontSchemeStyles GetActualFontSchemeStyle(bool actualApplyFont) {
			return GetActualFontValue(InnerActualFont.SchemeStyle, actualApplyFont, DifferentialFormatPropertyDescriptor.FontSchemeStyle);
		}
		protected virtual XlScriptType GetActualFontScript(bool actualApplyFont) {
			return GetActualFontValue(InnerActualFont.Script, actualApplyFont, DifferentialFormatPropertyDescriptor.FontScript);
		}
		protected virtual XlUnderlineType GetActualFontUnderline(bool actualApplyFont) {
			return GetActualFontValue(InnerActualFont.Underline, actualApplyFont, DifferentialFormatPropertyDescriptor.FontUnderline);
		}
		protected virtual FontInfo GetActualFontInfo() {
			FontInfoKey fontInfoKey = GetActualFontInfoKey();
			int index;
			if (conditionalFormatAccumulator != null) {
				conditionalFormatAccumulator.Update(this);
				if (conditionalFormatAccumulator.FontModified) {
					if (conditionalFormatAccumulator.BoldAndItalicModified) {
						fontInfoKey.Bold = conditionalFormatAccumulator.Bold;
						fontInfoKey.Italic = conditionalFormatAccumulator.Italic;
					}
					if (conditionalFormatAccumulator.StrikeThroughModified)
						fontInfoKey.StrikeThrough = conditionalFormatAccumulator.StrikeThrough;
					if (conditionalFormatAccumulator.UnderlineModified)
						fontInfoKey.Underline = conditionalFormatAccumulator.Underline;
					index = RunFontInfo.GetFontInfoIndex(DocumentModel.FontCache, fontInfoKey.FontName, fontInfoKey.FontSize, fontInfoKey.Bold, fontInfoKey.Italic, fontInfoKey.Script, fontInfoKey.StrikeThrough, fontInfoKey.Underline);
					fontKeyIndex = (ushort)DocumentModel.Cache.FontKeyCache.AddItem(fontInfoKey);
					return DocumentModel.FontCache[index];
				}
			}
			if (ActualFontInfoIndex >= 0 && TransactionVersion == Workbook.TransactionVersion)
				return DocumentModel.FontCache[ActualFontInfoIndex];
			this.TransactionVersion = Workbook.TransactionVersion;
			index = RunFontInfo.GetFontInfoIndex(DocumentModel.FontCache, fontInfoKey.FontName, fontInfoKey.FontSize, fontInfoKey.Bold, fontInfoKey.Italic, fontInfoKey.Script, fontInfoKey.StrikeThrough, fontInfoKey.Underline);
			fontKeyIndex = (ushort)DocumentModel.Cache.FontKeyCache.AddItem(fontInfoKey);
			if (index >= 0 && index <= short.MaxValue)
				this.ActualFontInfoIndex = (short)index;
			else
				this.ActualFontInfoIndex = -1;
			return DocumentModel.FontCache[index];
		}
		FontInfoKey GetActualFontInfoKey() {
			IActualRunFontInfo innerActualFont = InnerActualFont;
			bool actualApplyFont = ActualApplyInfo.ApplyFont;
			if (actualApplyFont)
				return FontInfoKey.FromActualFont(innerActualFont);
			CellPosition cellPosition = Position;
			ITableBase table = Worksheet.TryGetTableBase(cellPosition);
			if (table == null)
				return FontInfoKey.FromActualFont(innerActualFont);
			return new FontInfoKey(
				TableStyleFormatBuilderFactory.PropertyBuilder.Build(table, DifferentialFormatPropertyDescriptor.FontName, cellPosition, innerActualFont.Name),
				TableStyleFormatBuilderFactory.PropertyBuilder.Build(table, DifferentialFormatPropertyDescriptor.FontSize, cellPosition, innerActualFont.Size),
				TableStyleFormatBuilderFactory.PropertyBuilder.Build(table, DifferentialFormatPropertyDescriptor.FontBold, cellPosition, innerActualFont.Bold),
				TableStyleFormatBuilderFactory.PropertyBuilder.Build(table, DifferentialFormatPropertyDescriptor.FontItalic, cellPosition, innerActualFont.Italic),
				TableStyleFormatBuilderFactory.PropertyBuilder.Build(table, DifferentialFormatPropertyDescriptor.FontStrikeThrough, cellPosition, innerActualFont.StrikeThrough),
				TableStyleFormatBuilderFactory.PropertyBuilder.Build(table, DifferentialFormatPropertyDescriptor.FontUnderline, cellPosition, innerActualFont.Underline),
				TableStyleFormatBuilderFactory.PropertyBuilder.Build(table, DifferentialFormatPropertyDescriptor.FontScript, cellPosition, innerActualFont.Script)
			);
		}
		#endregion
		public static Color GetTextColor(IDocumentModelSkinColorProvider skinColorProvider, IActualRunFontInfo font) {
			Color color = font.Color;
			if (DXColor.IsTransparentOrEmpty(color))
				return skinColorProvider.SkinForeColor;
			else
				return color;
		}
		public static Color GetTextColor(IDocumentModelSkinColorProvider skinColorProvider, IActualRunFontInfo font, NumberFormatResult formatResult) {
			if (!DXColor.IsTransparentOrEmpty(formatResult.Color))
				return formatResult.Color;
			else
				return GetTextColor(skinColorProvider, font);
		}
		public static Color GetTextColor(IDocumentModelSkinColorProvider skinColorProvider, IActualRunFontInfo font, NumberFormatResult formatResult, bool isHyperlink) {
			if (isHyperlink)
				return DXColor.Blue;
			else
				return GetTextColor(skinColorProvider, font, formatResult);
		}
	}
	#endregion
	#region FontInfoKey
	public class FontInfoKey : ICloneable<FontInfoKey>, ISupportsCopyFrom<FontInfoKey>, ISupportsSizeOf {
		#region Fields
		const uint maskBold = 0x0001;
		const uint maskItalic = 0x0002;
		const uint maskStrikeThrough = 0x0004;
		const uint maskScript = 0x0030;
		const uint maskUnderline = 0x3f00;
		uint packedValues;
		string fontName;
		double fontSize;
		#endregion
		public static FontInfoKey FromActualFont(IActualRunFontInfo actualFont) {
			return new FontInfoKey(actualFont.Name, actualFont.Size, actualFont.Bold, actualFont.Italic, actualFont.StrikeThrough, actualFont.Underline, actualFont.Script);
		}
		FontInfoKey() {
		}
		public FontInfoKey(string fontName, double fontSize, bool bold, bool italic, bool strikeThrough, XlUnderlineType underline, XlScriptType script) {
			FontName = fontName;
			FontSize = fontSize;
			Bold = bold;
			Italic = italic;
			StrikeThrough = strikeThrough;
			Underline = underline;
			Script = script;
		}
		#region Properties
		public string FontName {
			get { return fontName; }
			private set {
				Guard.ArgumentIsNotNullOrEmpty(value, "FontName");
				fontName = value;
			}
		}
		public double FontSize {
			get { return fontSize; }
			private set {
				Guard.ArgumentPositive(value, "FontSize");
				fontSize = value;
			}
		}
		public bool Bold {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskBold); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskBold, value); }
		}
		public bool Italic {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskItalic); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskItalic, value); }
		}
		public bool StrikeThrough {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskStrikeThrough); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskStrikeThrough, value); }
		}
		public XlScriptType Script {
			get { return (XlScriptType)PackedValues.GetIntBitValue(this.packedValues, maskScript, 4); }
			private set { PackedValues.SetIntBitValue(ref this.packedValues, maskScript, 4, (int)value); }
		}
		public XlUnderlineType Underline {
			get { return (XlUnderlineType)PackedValues.GetIntBitValue(this.packedValues, maskUnderline, 8); }
			set { PackedValues.SetIntBitValue(ref this.packedValues, maskUnderline, 8, (int)value); }
		}
		#endregion
		public override bool Equals(object obj) {
			FontInfoKey other = obj as FontInfoKey;
			if (other == null)
				return false;
			return packedValues == other.packedValues && fontSize.Equals(other.fontSize) && string.Equals(fontName, other.fontName);
		}
		public override int GetHashCode() {
			return (int)packedValues ^ fontName.GetHashCode() ^ fontSize.GetHashCode();
		}
		#region ICloneable<FontInfoKey> Members
		public FontInfoKey Clone() {
			FontInfoKey result = new FontInfoKey();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<FontInfoKey> Members
		public void CopyFrom(FontInfoKey value) {
			Guard.ArgumentNotNull(value, "value");
			this.packedValues = value.packedValues;
			this.fontName = value.fontName;
			this.fontSize = value.fontSize;
		}
		#endregion
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return DXMarshal.SizeOf(GetType());
		}
		#endregion
	}
	#endregion
	#region FontKeyCache
	public class FontKeyCache : UniqueItemsCache<FontInfoKey> {
		public FontKeyCache(IDocumentModelUnitConverter converter)
			: base(converter) {
		}
		protected override FontInfoKey CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return new FontInfoKey("Calibri", 11.0, false, false, false, XlUnderlineType.None, XlScriptType.Baseline);
		}
	}
	#endregion
}
