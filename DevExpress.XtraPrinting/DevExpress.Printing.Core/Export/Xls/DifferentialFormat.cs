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
using System.Globalization;
using System.IO;
using System.Text;
using DevExpress.Utils;
using DevExpress.Export.Xl;
namespace DevExpress.XtraExport.Xls {
	#region XlsDxfNFlags
	public class XlsDxfNFlags {
		#region Fields
		const uint MaskAlignmentHorizontalNinch = 0x00000001;
		const uint MaskAlignmentVerticalNinch = 0x00000002;
		const uint MaskAlignmentWrapTextNinch = 0x00000004;
		const uint MaskAlignmentTextRotationNinch = 0x00000008;
		const uint MaskAlignmentJustifyLastLineNinch = 0x00000010;
		const uint MaskAlignmentIndentNinch = 0x00000020;
		const uint MaskAlignmentShrinkToFitNinch = 0x00000040;
		const uint MaskMergeCellNinch = 0x00000080; 
		const uint MaskProtectionLockedNinch = 0x00000100;
		const uint MaskProtectionHiddenNinch = 0x00000200;
		const uint MaskBorderLeftNinch = 0x00000400;
		const uint MaskBorderRightNinch = 0x00000800;
		const uint MaskBorderTopNinch = 0x00001000;
		const uint MaskBorderBottomNinch = 0x00002000;
		const uint MaskBorderDiagonalDownNinch = 0x00004000;
		const uint MaskBorderDiagonalUpNinch = 0x00008000;
		const uint MaskFillPatternTypeNinch = 0x00010000;
		const uint MaskFillForegroundColorNinch = 0x00020000;
		const uint MaskFillBackgroundColorNinch = 0x00040000;
		const uint MaskNumberFormatNinch = 0x00080000;
		const uint MaskFontNinch = 0x00100000;
		const uint MaskIncludeNumberFormat = 0x02000000;
		const uint MaskIncludeFont = 0x04000000;
		const uint MaskIncludeAlignment = 0x08000000;
		const uint MaskIncludeBorder = 0x10000000;
		const uint MaskIncludeFill = 0x20000000;
		const uint MaskIncludeProtection = 0x40000000;
		const uint MaskAlignmentReadingOrderNinch = 0x80000000;
		const ushort MaskUserDefinedNumberFormat = 0x0001;
		const ushort MaskNewBorder = 0x0004;
		const ushort MaskAlignmentReadingOrderZeroInited = 0x8000;
		uint firstPackedValues = 0x801FFFFF;
		ushort secondPackedValues;
		#endregion
		#region Properties
		public bool AlignmentHorizontalNinch {
			get { return GetBooleanValue(MaskAlignmentHorizontalNinch); }
			set { SetBooleanValue(MaskAlignmentHorizontalNinch, value); }
		}
		public bool AlignmentVerticalNinch {
			get { return GetBooleanValue(MaskAlignmentVerticalNinch); }
			set { SetBooleanValue(MaskAlignmentVerticalNinch, value); }
		}
		public bool AlignmentWrapTextNinch {
			get { return GetBooleanValue(MaskAlignmentWrapTextNinch); }
			set { SetBooleanValue(MaskAlignmentWrapTextNinch, value); }
		}
		public bool AlignmentTextRotationNinch {
			get { return GetBooleanValue(MaskAlignmentTextRotationNinch); }
			set { SetBooleanValue(MaskAlignmentTextRotationNinch, value); }
		}
		public bool AlignmentJustifyLastLineNinch {
			get { return GetBooleanValue(MaskAlignmentJustifyLastLineNinch); }
			set { SetBooleanValue(MaskAlignmentJustifyLastLineNinch, value); }
		}
		public bool AlignmentIndentNinch {
			get { return GetBooleanValue(MaskAlignmentIndentNinch); }
			set { SetBooleanValue(MaskAlignmentIndentNinch, value); }
		}
		public bool AlignmentShrinkToFitNinch {
			get { return GetBooleanValue(MaskAlignmentShrinkToFitNinch); }
			set { SetBooleanValue(MaskAlignmentShrinkToFitNinch, value); }
		}
		public bool AlignmentReadingOrderNinch {
			get { return GetBooleanValue(MaskAlignmentReadingOrderNinch); }
			set { SetBooleanValue(MaskAlignmentReadingOrderNinch, value); }
		}
		public bool ProtectionLockedNinch {
			get { return GetBooleanValue(MaskProtectionLockedNinch); }
			set { SetBooleanValue(MaskProtectionLockedNinch, value); }
		}
		public bool ProtectionHiddenNinch {
			get { return GetBooleanValue(MaskProtectionHiddenNinch); }
			set { SetBooleanValue(MaskProtectionHiddenNinch, value); }
		}
		public bool BorderLeftNinch {
			get { return GetBooleanValue(MaskBorderLeftNinch); }
			set { SetBooleanValue(MaskBorderLeftNinch, value); }
		}
		public bool BorderRightNinch {
			get { return GetBooleanValue(MaskBorderRightNinch); }
			set { SetBooleanValue(MaskBorderRightNinch, value); }
		}
		public bool BorderTopNinch {
			get { return GetBooleanValue(MaskBorderTopNinch); }
			set { SetBooleanValue(MaskBorderTopNinch, value); }
		}
		public bool BorderBottomNinch {
			get { return GetBooleanValue(MaskBorderBottomNinch); }
			set { SetBooleanValue(MaskBorderBottomNinch, value); }
		}
		public bool BorderDiagonalDownNinch {
			get { return GetBooleanValue(MaskBorderDiagonalDownNinch); }
			set { SetBooleanValue(MaskBorderDiagonalDownNinch, value); }
		}
		public bool BorderDiagonalUpNinch {
			get { return GetBooleanValue(MaskBorderDiagonalUpNinch); }
			set { SetBooleanValue(MaskBorderDiagonalUpNinch, value); }
		}
		public bool FillPatternTypeNinch {
			get { return GetBooleanValue(MaskFillPatternTypeNinch); }
			set { SetBooleanValue(MaskFillPatternTypeNinch, value); }
		}
		public bool FillForegroundColorNinch {
			get { return GetBooleanValue(MaskFillForegroundColorNinch); }
			set { SetBooleanValue(MaskFillForegroundColorNinch, value); }
		}
		public bool FillBackgroundColorNinch {
			get { return GetBooleanValue(MaskFillBackgroundColorNinch); }
			set { SetBooleanValue(MaskFillBackgroundColorNinch, value); }
		}
		public bool NumberFormatNinch {
			get { return GetBooleanValue(MaskNumberFormatNinch); }
			set { SetBooleanValue(MaskNumberFormatNinch, value); }
		}
		public bool FontNinch {
			get { return GetBooleanValue(MaskFontNinch); }
			set { SetBooleanValue(MaskFontNinch, value); }
		}
		public bool IncludeNumberFormat {
			get { return GetBooleanValue(MaskIncludeNumberFormat); }
			set { SetBooleanValue(MaskIncludeNumberFormat, value); }
		}
		public bool IncludeFont {
			get { return GetBooleanValue(MaskIncludeFont); }
			set { SetBooleanValue(MaskIncludeFont, value); }
		}
		public bool IncludeAlignment {
			get { return GetBooleanValue(MaskIncludeAlignment); }
			set { SetBooleanValue(MaskIncludeAlignment, value); }
		}
		public bool IncludeBorder {
			get { return GetBooleanValue(MaskIncludeBorder); }
			set { SetBooleanValue(MaskIncludeBorder, value); }
		}
		public bool IncludeFill {
			get { return GetBooleanValue(MaskIncludeFill); }
			set { SetBooleanValue(MaskIncludeFill, value); }
		}
		public bool IncludeProtection {
			get { return GetBooleanValue(MaskIncludeProtection); }
			set { SetBooleanValue(MaskIncludeProtection, value); }
		}
		public bool UserDefinedNumberFormat {
			get { return GetBooleanValue(MaskUserDefinedNumberFormat); }
			set { SetBooleanValue(MaskUserDefinedNumberFormat, value); }
		}
		public bool NewBorder {
			get { return GetBooleanValue(MaskNewBorder); }
			set { SetBooleanValue(MaskNewBorder, value); }
		}
		public bool AlignmentReadingOrderZeroInited {
			get { return GetBooleanValue(MaskAlignmentReadingOrderZeroInited); }
			set { SetBooleanValue(MaskAlignmentReadingOrderZeroInited, value); }
		}
		#endregion
		#region GetBooleanValue/SetBooleanValue helpers
		void SetBooleanValue(uint mask, bool bitVal) {
			if(bitVal)
				firstPackedValues |= mask;
			else
				firstPackedValues &= ~mask;
		}
		bool GetBooleanValue(uint mask) {
			return (firstPackedValues & mask) != 0;
		}
		void SetBooleanValue(ushort mask, bool bitVal) {
			if(bitVal)
				secondPackedValues |= mask;
			else
				secondPackedValues &= (ushort)(~mask);
		}
		bool GetBooleanValue(ushort mask) {
			return (secondPackedValues & mask) != 0;
		}
		#endregion
		public void Read(BinaryReader reader) {
			firstPackedValues = reader.ReadUInt32();
			secondPackedValues = reader.ReadUInt16();
		}
		public void Write(BinaryWriter writer) {
			XlsChunkWriter chunkWriter = writer as XlsChunkWriter;
			if(chunkWriter != null)
				chunkWriter.BeginRecord(6);
			writer.Write(firstPackedValues);
			writer.Write(secondPackedValues);
		}
	}
	#endregion
	#region XlsDxfNum (abstract)
	public abstract class XlsDxfNum {
		public abstract bool IsCustom { get; }
		public abstract int NumberFormatId { get; set; }
		public abstract string NumberFormatCode { get; set; }
		public abstract void Read(BinaryReader reader);
		public abstract void Write(BinaryWriter writer);
		public virtual int GetSize() {
			return 2;
		}
	}
	#endregion
	#region XlsDxfNumIFmt
	public class XlsDxfNumIFmt : XlsDxfNum {
		public override bool IsCustom { get { return false; } }
		public override int NumberFormatId { get; set; }
		public override string NumberFormatCode { get { return string.Empty; } set { } }
		public override void Read(BinaryReader reader) {
			reader.ReadByte();
			NumberFormatId = reader.ReadByte();
		}
		public override void Write(BinaryWriter writer) {
			XlsChunkWriter chunkWriter = writer as XlsChunkWriter;
			if(chunkWriter != null)
				chunkWriter.BeginRecord(GetSize());
			writer.Write((byte)0);
			writer.Write((byte)NumberFormatId);
		}
	}
	#endregion
	#region XlsDxfNumUser
	public class XlsDxfNumUser : XlsDxfNum {
		XLUnicodeString numberFormatCode = new XLUnicodeString();
		public override bool IsCustom { get { return true; } }
		public override int NumberFormatId { get { return 0; } set { } }
		public override string NumberFormatCode { 
			get { return numberFormatCode.Value; }
			set { numberFormatCode.Value = value; } 
		}
		public override void Read(BinaryReader reader) {
			reader.ReadUInt16();
			this.numberFormatCode = XLUnicodeString.FromStream(reader);
		}
		public override void Write(BinaryWriter writer) {
			int size = GetSize();
			XlsChunkWriter chunkWriter = writer as XlsChunkWriter;
			if(chunkWriter != null)
				chunkWriter.BeginRecord(size);
			writer.Write((ushort)size);
			this.numberFormatCode.Write(writer);
		}
		public override int GetSize() {
			return base.GetSize() + numberFormatCode.Length;
		}
	}
	#endregion
	#region XlsDxfFont
	public class XlsDxfFont {
		#region Fields
		public const int DefaultIntValue = -1;
		public const short Size = 118;
		const int maxFontNameLength = 63;
		const int normalFontWeightValue = 0x0190;
		const int boldFontWeightValue = 0x02BC;
		XLUnicodeStringNoCch fontName = new XLUnicodeStringNoCch();
		int fontSize = DefaultIntValue;
		int fontWeight = DefaultIntValue;
		int fontScript = DefaultIntValue;
		int fontUnderline = DefaultIntValue;
		int fontFamily;
		int fontCharset;
		int fontColorIndex = DefaultIntValue;
		int firstPosition = DefaultIntValue;
		int charactersCount = DefaultIntValue;
		bool isDefaultFont = true;
		bool fontItalicNinch = true;
		bool fontStrikeThroughNinch = true;
		bool fontScriptNinch = true;
		bool fontUnderlineNinch = true;
		bool fontBoldNinch = true;
		#endregion
		#region Properties
		public string FontName {
			get { return fontName.Value; }
			set {
				XLUnicodeStringNoCch newFontName = new XLUnicodeStringNoCch();
				newFontName.Value = value;
				if(newFontName.Length > maxFontNameLength)
					throw new ArgumentException("String value too long");
				fontName = newFontName;
			}
		}
		public int FontSize { get { return fontSize; } set { fontSize = value; } }
		public int FontFamily { get { return fontFamily; } set { fontFamily = value; } }
		public int FontCharset { get { return fontCharset; } set { fontCharset = value; } }
		public int FontColorIndex { get { return fontColorIndex; } set { fontColorIndex = value; } }
		public int FirstPosition { get { return firstPosition; } set { firstPosition = value; } }
		public int CharactersCount {
			get { return charactersCount; }
			set {
				if(FirstPosition == DefaultIntValue && value != DefaultIntValue)
					throw new ArgumentException("Invalid number of characters");
				charactersCount = value;
			}
		}
		public bool IsDefaultFont { get { return isDefaultFont; } set { isDefaultFont = value; } }
		public bool FontItalicNinch { get { return fontItalicNinch; } set { fontItalicNinch = value; } }
		public bool FontStrikeThroughNinch { get { return fontStrikeThroughNinch; } set { fontStrikeThroughNinch = value; } }
		public bool FontScriptNinch { get { return fontScriptNinch; } set { fontScriptNinch = value; } }
		public bool FontUnderlineNinch { get { return fontUnderlineNinch; } set { fontUnderlineNinch = value; } }
		public bool FontBoldNinch { get { return fontBoldNinch; } set { fontBoldNinch = value; } }
		public bool? FontBold {
			get {
				if(fontWeight == DefaultIntValue)
					return null;
				return fontWeight == boldFontWeightValue;
			}
			set {
				if(value.HasValue)
					fontWeight = value.Value ? boldFontWeightValue : normalFontWeightValue;
				else
					fontWeight = DefaultIntValue;
			}
		}
		public XlScriptType? FontScript {
			get {
				if(fontScript == DefaultIntValue)
					return null;
				return (XlScriptType)fontScript;
			}
			set {
				if(value.HasValue)
					fontScript = (int)value.Value;
				else
					fontScript = DefaultIntValue;
			}
		}
		public XlUnderlineType? FontUnderline {
			get {
				if(fontUnderline == DefaultIntValue)
					return null;
				return (XlUnderlineType)fontUnderline;
			}
			set {
				if(value.HasValue)
					fontUnderline = (int)value.Value;
				else
					fontUnderline = DefaultIntValue;
			}
		}
		public bool FontItalic { get; set; }
		public bool FontStrikeThrough { get; set; }
		#endregion
		public void Read(BinaryReader reader) {
			ReadFontName(reader);
			ReadStxpStructure(reader);
			FontColorIndex = reader.ReadInt32();
			reader.ReadInt32();
			ReadTsNinchStructure(reader);
			FontScriptNinch = reader.ReadInt32() != 0;
			FontUnderlineNinch = reader.ReadInt32() != 0;
			FontBoldNinch = reader.ReadInt32() != 0;
			reader.ReadInt32();
			FirstPosition = reader.ReadInt32();
			CharactersCount = reader.ReadInt32();
			IsDefaultFont = reader.ReadInt16() == 0;
		}
		void ReadFontName(BinaryReader reader) {
			int charCount = reader.ReadByte();
			int countBytes = 63;
			if(charCount > 0) {
				this.fontName = XLUnicodeStringNoCch.FromStream(reader, charCount);
				countBytes -= this.fontName.Length;
			}
			reader.ReadBytes(countBytes);
		}
		void ReadStxpStructure(BinaryReader reader) {
			FontSize = reader.ReadInt32();
			ReadTsStructure(reader);
			fontWeight = reader.ReadInt16();
			fontScript = reader.ReadInt16();
			fontUnderline = reader.ReadSByte();
			FontFamily = reader.ReadByte();
			fontCharset = reader.ReadByte();
			reader.ReadByte();
		}
		void ReadTsStructure(BinaryReader reader) {
			uint bitwiseField = reader.ReadUInt32();
			FontItalic = Convert.ToBoolean(bitwiseField & 0x00000002);
			FontStrikeThrough = Convert.ToBoolean(bitwiseField & 0x00000080);
		}
		void ReadTsNinchStructure(BinaryReader reader) {
			uint bitwiseField = reader.ReadUInt32();
			FontItalicNinch = Convert.ToBoolean(bitwiseField & 0x00000002);
			FontStrikeThroughNinch = Convert.ToBoolean(bitwiseField & 0x00000080);
		}
		public void Write(BinaryWriter writer) {
			XlsChunkWriter chunkWriter = writer as XlsChunkWriter;
			if(chunkWriter != null)
				chunkWriter.BeginRecord(118);
			WriteFontName(writer);
			WriteStxpStructure(writer);
			writer.Write((uint)FontColorIndex);
			writer.Write((uint)0);
			WriteTsStructure(writer, FontItalicNinch, FontStrikeThroughNinch);
			writer.Write(FontScriptNinch ? 1 : 0);
			writer.Write(FontUnderlineNinch ? 1 : 0);
			writer.Write(FontBoldNinch ? 1 : 0);
			writer.Write((uint)0);
			writer.Write((uint)FirstPosition);
			writer.Write((uint)CharactersCount);
			writer.Write((ushort)(IsDefaultFont ? 0 : 1));
		}
		void WriteFontName(BinaryWriter writer) {
			int countBytes = 63;
			if(!String.IsNullOrEmpty(this.fontName.Value)) {
				writer.Write((byte)this.fontName.Value.Length);
				this.fontName.Write(writer);
				countBytes -= this.fontName.Length;
			}
			else
				writer.Write((byte)0);
			if(countBytes > 0)
				writer.Write(new byte[countBytes]);
		}
		void WriteStxpStructure(BinaryWriter writer) {
			writer.Write(FontSize);
			WriteTsStructure(writer, FontItalic, FontStrikeThrough);
			writer.Write((ushort)fontWeight);
			writer.Write((ushort)fontScript);
			writer.Write((byte)fontUnderline);
			writer.Write((byte)FontFamily);
			writer.Write((byte)FontCharset);
			writer.Write((byte)0);
		}
		void WriteTsStructure(BinaryWriter writer, bool fontItalic, bool fontStrikeThrough) {
			uint bitwiseField = 0;
			if(fontItalic)
				bitwiseField |= 0x00000002;
			if(fontStrikeThrough)
				bitwiseField |= 0x00000080;
			writer.Write(bitwiseField);
		}
	}
	#endregion
	#region XlsDxfAlign
	public class XlsDxfAlign {
		#region Fields
		const uint MaskHorizontalAlignment = 0x00000007;
		const uint MaskWrapText = 0x00000008;
		const uint MaskVerticalAlignment = 0x00000070;
		const uint MaskJustifyLastLine = 0x00000080;
		const uint MaskTextRotation = 0x0000FF00;
		const uint MaskIndent = 0x000F0000;
		const uint MaskShrinkToFit = 0x00100000;
		const uint MaskMergeCell = 0x00200000;
		const uint MaskReadingOrder = 0x00C00000;
		public const short Size = 8;
		public const int DefaultRelativeIndent = 255;
		uint packedValues;
		int relativeIndent = DefaultRelativeIndent;
		#endregion
		#region Properties
		public XlHorizontalAlignment HorizontalAlignment {
			get { return (XlHorizontalAlignment)(packedValues & MaskHorizontalAlignment); }
			set {
				packedValues &= ~MaskHorizontalAlignment;
				packedValues |= (uint)value & MaskHorizontalAlignment;
			}
		}
		public XlVerticalAlignment VerticalAlignment {
			get { return (XlVerticalAlignment)((packedValues & MaskVerticalAlignment) >> 4); }
			set {
				packedValues &= ~MaskVerticalAlignment;
				packedValues |= ((uint)value << 4) & MaskVerticalAlignment;
			}
		}
		public int TextRotation {
			get { return (int)((packedValues & MaskTextRotation) >> 8); }
			set {
				packedValues &= ~MaskTextRotation;
				packedValues |= ((uint)value << 8) & MaskTextRotation;
			}
		}
		public byte Indent {
			get { return (byte)((packedValues & MaskIndent) >> 16); }
			set {
				packedValues &= ~MaskIndent;
				packedValues |= ((uint)value << 16) & MaskIndent;
			}
		}
		public XlReadingOrder ReadingOrder {
			get { return (XlReadingOrder)((packedValues & MaskReadingOrder) >> 22); }
			set {
				packedValues &= ~MaskReadingOrder;
				packedValues |= ((uint)value << 22) & MaskReadingOrder;
			}
		}
		public bool WrapText { get { return GetBooleanValue(MaskWrapText); } set { SetBooleanValue(MaskWrapText, value); } }
		public bool JustifyLastLine { get { return GetBooleanValue(MaskJustifyLastLine); } set { SetBooleanValue(MaskJustifyLastLine, value); } }
		public bool ShrinkToFit { get { return GetBooleanValue(MaskShrinkToFit); } set { SetBooleanValue(MaskShrinkToFit, value); } }
		public bool MergeCell { get { return GetBooleanValue(MaskMergeCell); } set { SetBooleanValue(MaskMergeCell, value); } }
		public int RelativeIndent { get { return relativeIndent; } set { relativeIndent = value; } }
		#endregion
		public void Read(BinaryReader reader) {
			packedValues = reader.ReadUInt32();
			RelativeIndent = reader.ReadInt32();
		}
		public void Write(BinaryWriter writer) {
			XlsChunkWriter chunkWriter = writer as XlsChunkWriter;
			if(chunkWriter != null)
				chunkWriter.BeginRecord(8);
			writer.Write(packedValues);
			writer.Write((int)RelativeIndent);
		}
		#region GetBooleanValue/SetBooleanValue helpers
		void SetBooleanValue(uint mask, bool bitVal) {
			if(bitVal)
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
	#region XlsDxfBorder
	public class XlsDxfBorder {
		static Dictionary<uint, int> maskToPositionTranslationTable = CreateMaskToPositionTranslationTable();
		static Dictionary<uint, int> CreateMaskToPositionTranslationTable() {
			Dictionary<uint, int> result = new Dictionary<uint, int>();
			result.Add(MaskLeftLineStyle, LeftLineStylePosition);
			result.Add(MaskRightLineStyle, RightLineStylePosition);
			result.Add(MaskTopLineStyle, TopLineStylePosition);
			result.Add(MaskBottomLineStyle, BottomLineStylePosition);
			result.Add(MaskLeftColorIndex, LeftColorIndexPosition);
			result.Add(MaskRightColorIndex, RightColorIndexPosition);
			result.Add(MaskTopColorIndex, TopColorIndexPosition);
			result.Add(MaskBottomColorIndex, BottomColorIndexPosition);
			result.Add(MaskDiagonalColorIndex, DiagonalColorIndexPosition);
			result.Add(MaskDiagonalLineStyle, DiagonalLineStylePosition);
			return result;
		}
		#region Fields
		#region FirstPackedValuesMasks
		const uint MaskLeftLineStyle = 0x0000000F; 
		const uint MaskRightLineStyle = 0x000000F0; 
		const uint MaskTopLineStyle = 0x00000F00; 
		const uint MaskBottomLineStyle = 0x0000F000; 
		const uint MaskLeftColorIndex = 0x007F0000; 
		const uint MaskRightColorIndex = 0x3F800000; 
		const uint MaskDiagonalDown = 0x40000000;
		const uint MaskDiagonalUp = 0x80000000;
		#endregion
		#region SecondPackedValuesMasks
		const uint MaskTopColorIndex = 0x0000007F; 
		const uint MaskBottomColorIndex = 0x00003F80; 
		const uint MaskDiagonalColorIndex = 0x001FC000; 
		const uint MaskDiagonalLineStyle = 0x01E00000; 
		#endregion
		#region FirstPackedValuesPosition
		const int LeftLineStylePosition = 0;
		const int RightLineStylePosition = 4;
		const int TopLineStylePosition = 8;
		const int BottomLineStylePosition = 12;
		const int LeftColorIndexPosition = 16;
		const int RightColorIndexPosition = 23;
		#endregion
		#region SecondPackedValuesMasks
		const int TopColorIndexPosition = 0;
		const int BottomColorIndexPosition = 7;
		const int DiagonalColorIndexPosition = 14;
		const int DiagonalLineStylePosition = 21;
		#endregion
		public const short Size = 8;
		uint firstPackedValues = 0x20400000;
		uint secondPackedValues = 0x0002040;
		#endregion
		#region Properties
		public XlBorderLineStyle LeftLineStyle {
			get { return GetBorderLineStyle(firstPackedValues, MaskLeftLineStyle, LeftLineStylePosition); }
			set { firstPackedValues = SetBorderLineStyle(firstPackedValues, MaskLeftLineStyle, value); }
		}
		public XlBorderLineStyle RightLineStyle {
			get { return GetBorderLineStyle(firstPackedValues, MaskRightLineStyle, RightLineStylePosition); }
			set { firstPackedValues = SetBorderLineStyle(firstPackedValues, MaskRightLineStyle, value); }
		}
		public XlBorderLineStyle TopLineStyle {
			get { return GetBorderLineStyle(firstPackedValues, MaskTopLineStyle, TopLineStylePosition); }
			set { firstPackedValues = SetBorderLineStyle(firstPackedValues, MaskTopLineStyle, value); }
		}
		public XlBorderLineStyle BottomLineStyle {
			get { return GetBorderLineStyle(firstPackedValues, MaskBottomLineStyle, BottomLineStylePosition); }
			set { firstPackedValues = SetBorderLineStyle(firstPackedValues, MaskBottomLineStyle, value); }
		}
		public int LeftColorIndex {
			get { return GetBorderColorIndex(firstPackedValues, MaskLeftColorIndex, LeftColorIndexPosition); }
			set { firstPackedValues = SetBorderColorIndex(firstPackedValues, MaskLeftColorIndex, value); }
		}
		public int RightColorIndex {
			get { return GetBorderColorIndex(firstPackedValues, MaskRightColorIndex, RightColorIndexPosition); }
			set { firstPackedValues = SetBorderColorIndex(firstPackedValues, MaskRightColorIndex, value); }
		}
		public bool DiagonalDown { get { return GetBooleanValue(MaskDiagonalDown); } set { SetBooleanValue(MaskDiagonalDown, value); } }
		public bool DiagonalUp { get { return GetBooleanValue(MaskDiagonalUp); } set { SetBooleanValue(MaskDiagonalUp, value); } }
		public int TopColorIndex {
			get { return GetBorderColorIndex(secondPackedValues, MaskTopColorIndex, TopColorIndexPosition); }
			set { secondPackedValues = SetBorderColorIndex(secondPackedValues, MaskTopColorIndex, value); }
		}
		public int BottomColorIndex {
			get { return GetBorderColorIndex(secondPackedValues, MaskBottomColorIndex, BottomColorIndexPosition); }
			set { secondPackedValues = SetBorderColorIndex(secondPackedValues, MaskBottomColorIndex, value); }
		}
		public int DiagonalColorIndex {
			get { return GetBorderColorIndex(secondPackedValues, MaskDiagonalColorIndex, DiagonalColorIndexPosition); }
			set { secondPackedValues = SetBorderColorIndex(secondPackedValues, MaskDiagonalColorIndex, value); }
		}
		public XlBorderLineStyle DiagonalLineStyle {
			get { return GetBorderLineStyle(secondPackedValues, MaskDiagonalLineStyle, DiagonalLineStylePosition); }
			set { secondPackedValues = SetBorderLineStyle(secondPackedValues, MaskDiagonalLineStyle, value); }
		}
		#endregion
		public void Read(BinaryReader reader) {
			firstPackedValues = reader.ReadUInt32();
			secondPackedValues = reader.ReadUInt32();
		}
		public void Write(BinaryWriter writer) {
			XlsChunkWriter chunkWriter = writer as XlsChunkWriter;
			if(chunkWriter != null)
				chunkWriter.BeginRecord(8);
			writer.Write(firstPackedValues);
			writer.Write(secondPackedValues);
		}
		#region GetUIntValue/SetUIntValue helpers
		uint SetUIntValue(uint packedValues, uint mask, int position, uint value) {
			packedValues &= ~mask;
			packedValues |= GetPackedValue(mask, position, value);
			return packedValues;
		}
		uint GetPackedValue(uint mask, int position, uint value) {
			return (value << position) & mask;
		}
		uint GetUIntValue(uint packedValues, uint mask, int position) {
			return ((packedValues & mask) >> position);
		}
		#endregion
		#region GetBorderLineStyle/SetBorderLineStyle helpers
		uint SetBorderLineStyle(uint packedValues, uint mask, XlBorderLineStyle value) {
			int position = maskToPositionTranslationTable[mask];
			return SetUIntValue(packedValues, mask, position, (uint)value);
		}
		XlBorderLineStyle GetBorderLineStyle(uint packedValues, uint mask, int position) {
			return (XlBorderLineStyle)GetUIntValue(packedValues, mask, position);
		}
		#endregion
		#region GetBorderColorIndex/SetBorderColorIndex helpers
		uint SetBorderColorIndex(uint packedValues, uint mask, int value) {
			int position = maskToPositionTranslationTable[mask];
			return SetUIntValue(packedValues, mask, position, (uint)value);
		}
		int GetBorderColorIndex(uint packedValues, uint mask, int position) {
			return (int)GetUIntValue(packedValues, mask, position);
		}
		#endregion
		#region GetBooleanValue/SetBooleanValue helpers
		void SetBooleanValue(uint mask, bool bitVal) {
			if(bitVal)
				firstPackedValues |= mask;
			else
				firstPackedValues &= ~mask;
		}
		bool GetBooleanValue(uint mask) {
			return (firstPackedValues & mask) != 0;
		}
		#endregion
	}
	#endregion
	#region XlsDxfFill
	public class XlsDxfFill {
		#region Fields
		const uint MaskPatternType = 0x0000FC00; 
		const uint MaskForeColorIndex = 0x007F0000; 
		const uint MaskBackColorIndex = 0x3F800000; 
		public const short Size = 4;
		uint packedValues;
		#endregion
		#region Properties
		public XlPatternType PatternType {
			get { return GetPatternType(); }
			set { SetPatternType(value); }
		}
		public int BackColorIndex {
			get { return GetColorIndex(MaskBackColorIndex, 23); }
			set { SetColorIndex(MaskBackColorIndex, 23, value); }
		}
		public int ForeColorIndex {
			get { return GetColorIndex(MaskForeColorIndex, 16); }
			set { SetColorIndex(MaskForeColorIndex, 16, value); }
		}
		#endregion
		public void Read(BinaryReader reader) {
			packedValues = reader.ReadUInt32();
		}
		public void Write(BinaryWriter writer) {
			XlsChunkWriter chunkWriter = writer as XlsChunkWriter;
			if(chunkWriter != null)
				chunkWriter.BeginRecord(4);
			writer.Write(packedValues);
		}
		#region GetUIntValue/SetUIntValue helpers
		void SetUIntValue(uint mask, int position, uint value) {
			packedValues &= ~mask;
			packedValues |= GetPackedValue(mask, position, value);
		}
		uint GetPackedValue(uint mask, int position, uint value) {
			return (value << position) & mask;
		}
		uint GetUIntValue(uint mask, int position) {
			return ((packedValues & mask) >> position);
		}
		#endregion
		#region GetPatternType/SetPatternType helpers
		void SetPatternType(XlPatternType value) {
			SetUIntValue(MaskPatternType, 10, (uint)value);
		}
		XlPatternType GetPatternType() {
			return (XlPatternType)GetUIntValue(MaskPatternType, 10);
		}
		#endregion
		#region GetColorIndex/SetColorIndex helpers
		void SetColorIndex(uint mask, int position, int value) {
			SetUIntValue(mask, position, (uint)value);
		}
		int GetColorIndex(uint mask, int position) {
			return (int)GetUIntValue(mask, position);
		}
		#endregion
	}
	#endregion
	#region XlsDxfProtection
	public class XlsDxfProtection {
		#region Fields
		const ushort MaskLocked = 0x0001;
		const ushort MaskHidden = 0x0002;
		public const short Size = 2;
		ushort packedValues;
		#endregion
		#region Properties
		public bool Locked { get { return GetBooleanValue(MaskLocked); } set { SetBooleanValue(MaskLocked, value); } }
		public bool Hidden { get { return GetBooleanValue(MaskHidden); } set { SetBooleanValue(MaskHidden, value); } }
		#endregion
		public void Read(BinaryReader reader) {
			packedValues = reader.ReadUInt16();
		}
		public void Write(BinaryWriter writer) {
			XlsChunkWriter chunkWriter = writer as XlsChunkWriter;
			if(chunkWriter != null)
				chunkWriter.BeginRecord(2);
			writer.Write(packedValues);
		}
		#region GetBooleanValue/SetBooleanValue helpers
		void SetBooleanValue(ushort mask, bool bitVal) {
			if(bitVal)
				packedValues |= mask;
			else
				packedValues &= (ushort)(~mask);
		}
		bool GetBooleanValue(ushort mask) {
			return (packedValues & mask) != 0;
		}
		#endregion
	}
	#endregion
	#region XlsDxfN
	public class XlsDxfN {
		#region Fields
		const int fixedPartSize = 6;
		readonly XlsDxfNFlags flagsInfo = new XlsDxfNFlags();
		readonly XlsDxfFont fontInfo = new XlsDxfFont();
		readonly XlsDxfAlign alignmentInfo = new XlsDxfAlign();
		readonly XlsDxfBorder borderInfo = new XlsDxfBorder();
		readonly XlsDxfFill fillInfo = new XlsDxfFill();
		readonly XlsDxfProtection protectionInfo = new XlsDxfProtection();
		XlsDxfNum numberFormatInfo = new XlsDxfNumIFmt();
		#endregion
		#region Properties
		public XlsDxfNFlags FlagsInfo { get { return flagsInfo; } }
		public XlsDxfNum NumberFormatInfo { get { return numberFormatInfo; } set { numberFormatInfo = value; } }
		public XlsDxfFont FontInfo { get { return fontInfo; } }
		public XlsDxfAlign AlignmentInfo { get { return alignmentInfo; } }
		public XlsDxfBorder BorderInfo { get { return borderInfo; } }
		public XlsDxfFill FillInfo { get { return fillInfo; } }
		public XlsDxfProtection ProtectionInfo { get { return protectionInfo; } }
		#endregion
		public virtual void Read(BinaryReader reader) {
			FlagsInfo.Read(reader);
			if(FlagsInfo.IncludeNumberFormat)
				NumberFormatInfo = ReadDxfNum(reader, FlagsInfo.UserDefinedNumberFormat);
			if(FlagsInfo.IncludeFont)
				FontInfo.Read(reader);
			if(FlagsInfo.IncludeAlignment)
				AlignmentInfo.Read(reader);
			if(FlagsInfo.IncludeBorder)
				BorderInfo.Read(reader);
			if(FlagsInfo.IncludeFill)
				FillInfo.Read(reader);
			if(FlagsInfo.IncludeProtection)
				ProtectionInfo.Read(reader);
		}
		public virtual void Write(BinaryWriter writer) {
			XlsChunkWriter chunkWriter = writer as XlsChunkWriter;
			if(chunkWriter != null)
				chunkWriter.BeginRecord(GetSize());
			FlagsInfo.Write(writer);
			if(FlagsInfo.IncludeNumberFormat)
				NumberFormatInfo.Write(writer);
			if(FlagsInfo.IncludeFont)
				FontInfo.Write(writer);
			if(FlagsInfo.IncludeAlignment)
				AlignmentInfo.Write(writer);
			if(FlagsInfo.IncludeBorder)
				BorderInfo.Write(writer);
			if(FlagsInfo.IncludeFill)
				FillInfo.Write(writer);
			if(FlagsInfo.IncludeProtection)
				ProtectionInfo.Write(writer);
		}
		public virtual short GetSize() {
			int result = fixedPartSize;
			if(FlagsInfo.IncludeNumberFormat)
				result += NumberFormatInfo.GetSize();
			if(FlagsInfo.IncludeFont)
				result += XlsDxfFont.Size;
			if(FlagsInfo.IncludeAlignment)
				result += XlsDxfAlign.Size;
			if(FlagsInfo.IncludeBorder)
				result += XlsDxfBorder.Size;
			if(FlagsInfo.IncludeFill)
				result += XlsDxfFill.Size;
			if(FlagsInfo.IncludeProtection)
				result += XlsDxfProtection.Size;
			return (short)result;
		}
		XlsDxfNum ReadDxfNum(BinaryReader reader, bool isCustom) {
			XlsDxfNum result = CreateDxfNum(isCustom);
			result.Read(reader);
			return result;
		}
		protected XlsDxfNum CreateDxfNum(bool isCustom) {
			if(isCustom)
				return new XlsDxfNumUser();
			return new XlsDxfNumIFmt();
		}
		internal virtual void SetIsEmpty(bool value) {
		}
		internal virtual void AddExtProperty(XfPropBase prop) {
		}
	}
	#endregion
	#region XlsDxfN12
	public class XlsDxfN12 : XlsDxfN {
		readonly XfProperties extProperties = new XfProperties();
		public bool IsEmpty { get; set; }
		public XfProperties ExtProperties { get { return extProperties; } }
		public override void Read(BinaryReader reader) {
		}
		public override void Write(BinaryWriter writer) {
			if(IsEmpty) {
				writer.Write((int)0); 
				writer.Write((ushort)0); 
			}
			else {
				int cbDxf = GetSize() - 4;
				writer.Write(cbDxf);
				base.Write(writer);
				if(extProperties.Count > 0) {
					writer.Write((ushort)0x0000); 
					writer.Write((ushort)0xffff); 
					extProperties.Write(writer);
				}
			}
		}
		public override short GetSize() {
			if(IsEmpty)
				return 6;
			int result = base.GetSize() + 4;
			if(extProperties.Count > 0)
				result += extProperties.GetSize() + 4;
			return (short)result;
		}
		internal override void SetIsEmpty(bool value) {
			IsEmpty = value;
		}
		internal override void AddExtProperty(XfPropBase prop) {
			this.extProperties.Add(prop);
		}
	}
	#endregion
}
