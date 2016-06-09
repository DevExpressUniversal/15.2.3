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
using System.IO;
using System.Text;
using DevExpress.Data.Export;
using DevExpress.Utils;
using DevExpress.Export.Xl;
using System.Drawing.Printing;
using DevExpress.Compatibility.System.Drawing.Printing;
namespace DevExpress.XtraExport.Xls {
	#region XlsSubstreamType
	public enum XlsSubstreamType {
		WorkbookGlobals = 0x0005,
		VisualBasicModule = 0x0006,
		Sheet = 0x0010,
		Chart = 0x0020,
		MacroSheet = 0x0040,
		Workspace = 0x0100
	}
	#endregion
	#region XlsFileHistory
	public static class XlsFileHistory {
		public static int Win = 0x00000001;
		public static int RISC = 0x00000002;
		public static int Beta = 0x00000004;
		public static int WinAny = 0x00000008;
		public static int MacAny = 0x00000010;
		public static int BetaAny = 0x00000020;
		public static int Unused = 0x000000c0;
		public static int RISCAny = 0x00000100;
		public static int OutOfMemoryFailure = 0x00000200;
		public static int OutOfMemoryFailureDuringRendering = 0x00000400;
		public static int FontLimit = 0x00002000;
		public static int Excel2010 = 0x00018000;
		public static int Default = XlsFileHistory.Win | XlsFileHistory.WinAny | XlsFileHistory.Unused | XlsFileHistory.Excel2010;
	}
	#endregion
	#region XlsUpdateLinksMode
	public enum XlsUpdateLinksMode {
		Prompt = 0,
		DontUpdate = 1,
		SilentlyUpdate = 2
	}
	#endregion
	#region XlsContentEmpty
	public class XlsContentEmpty : XlsContentBase {
		public override void Read(XlsReader reader, int size) {
		}
		public override void Write(BinaryWriter writer) {
		}
		public override int GetSize() {
			return 0;
		}
	}
	#endregion
	#region XlsContentBoolValue
	public class XlsContentBoolValue : XlsContentBase {
		public bool Value { get; set; }
		public override void Read(XlsReader reader, int size) {
			Value = reader.ReadInt16() != 0;
		}
		public override void Write(BinaryWriter writer) {
			writer.Write((short)(Value ? 1 : 0));
		}
		public override int GetSize() {
			return 2;
		}
	}
	#endregion
	#region XlsContentShortValue
	public class XlsContentShortValue : XlsContentBase {
		public short Value { get; set; }
		public override void Read(XlsReader reader, int size) {
			Value = reader.ReadInt16();
		}
		public override void Write(BinaryWriter writer) {
			writer.Write(Value);
		}
		public override int GetSize() {
			return 2;
		}
	}
	#endregion
	#region XlsContentDoubleValue
	public class XlsContentDoubleValue : XlsContentBase {
		public double Value { get; set; }
		public override void Read(XlsReader reader, int size) {
			Value = reader.ReadDouble();
		}
		public override void Write(BinaryWriter writer) {
			writer.Write(Value);
		}
		public override int GetSize() {
			return sizeof(double);
		}
	}
	#endregion
	#region XlsContentStringValue
	public class XlsContentStringValue : XlsContentBase {
		XLUnicodeString internalString = new XLUnicodeString();
		public string Value {
			get { return internalString.Value; }
			set { internalString.Value = value; }
		}
		public override void Read(XlsReader reader, int size) {
			if (size > 0)
				this.internalString = XLUnicodeString.FromStream(reader);
			else
				this.internalString.Value = string.Empty;
		}
		public override void Write(BinaryWriter writer) {
			if (!string.IsNullOrEmpty(this.internalString.Value))
				this.internalString.Write(writer);
		}
		public override int GetSize() {
			if (string.IsNullOrEmpty(this.internalString.Value))
				return 0;
			return this.internalString.Length;
		}
	}
	#endregion
	#region XlsContentBeginOfSubstream
	public class XlsContentBeginOfSubstream : XlsContentBase {
		#region Fields
		XlsSubstreamType substreamType;
		int fileHistoryFlags = XlsFileHistory.Default;
		#endregion
		#region Properties
		public XlsSubstreamType SubstreamType {
			get { return this.substreamType; }
			set { this.substreamType = value; }
		}
		public int FileHistoryFlags {
			get { return this.fileHistoryFlags; }
			set { this.fileHistoryFlags = value; }
		}
		#endregion
		public override void Read(XlsReader reader, int size) {
			short biffVersion = reader.ReadNotCryptedInt16();
			if (biffVersion != XlsDefs.BIFFVersion && biffVersion != XlsDefs.BIFF5Version)
				ThrowInvalidXlsFile();
			this.substreamType = (XlsSubstreamType)reader.ReadNotCryptedInt16();
			reader.Seek(4, SeekOrigin.Current); 
			if (size == 8)
				this.fileHistoryFlags = XlsFileHistory.Default;
			else {
				this.fileHistoryFlags = reader.ReadNotCryptedInt32();
				reader.Seek(4, SeekOrigin.Current);
			}
		}
		public override void Write(BinaryWriter writer) {
			writer.Write(XlsDefs.BIFFVersion);
			writer.Write((short)SubstreamType);
			writer.Write(XlsDefs.DefaultBuildIdentifier);
			writer.Write(XlsDefs.DefaultBuildYear);
			writer.Write(this.fileHistoryFlags);
			writer.Write((int)0x0606);
		}
		public override int GetSize() {
			return 0x0010;
		}
	}
	#endregion
	#region XlsContentEncoding
	public class XlsContentEncoding : XlsContentBase {
		#region Fields
		const short defaultCodePage = 0x04b0;
		Encoding value = DXEncoding.GetEncodingFromCodePage(defaultCodePage);
		#endregion
		#region Properties
		public Encoding Value {
			get { return value; }
			set {
				Guard.ArgumentNotNull(value, "Value");
				this.value = value;
			}
		}
		#endregion
		public override void Read(XlsReader reader, int size) {
			this.value = DXEncoding.GetEncodingFromCodePage(reader.ReadInt16());
		}
		public override void Write(BinaryWriter writer) {
			writer.Write((short)DXEncoding.GetEncodingCodePage(this.value));
		}
		public override int GetSize() {
			return 2;
		}
	}
	#endregion
	#region XlsContentWriteAccess
	public class XlsContentWriteAccess : XlsContentBase {
		#region Fields
		const int dataSize = 112;
		const string noUserName = "  ";
		XLUnicodeString userName = new XLUnicodeString() { Value = noUserName };
		#endregion
		#region Properties
		public string Value {
			get { return this.userName.Value; }
			set {
				this.userName.Value = string.IsNullOrEmpty(value) ? noUserName : value;
				if (this.userName.Length > dataSize)
					throw new ArgumentException("Too long string value");
			}
		}
		#endregion
		public override void Read(XlsReader reader, int size) {
			this.userName = XLUnicodeString.FromStream(reader, size);
			int bytesToSkip = size - this.userName.Length;
			if (bytesToSkip > 0)
				reader.Seek(bytesToSkip, SeekOrigin.Current);
		}
		public override void Write(BinaryWriter writer) {
			this.userName.Write(writer);
			int remineLength = dataSize - this.userName.Length;
			for (int i = 0; i < remineLength; i++)
				writer.Write((byte)0x20);
		}
		public override int GetSize() {
			return dataSize;
		}
	}
	#endregion
	#region XlsContentSheetIdTable
	public class XlsContentSheetIdTable : XlsContentBase {
		#region Fields
		List<int> sheetIdTable = new List<int>();
		#endregion
		#region Properties
		public List<int> SheetIdTable { get { return sheetIdTable; } }
		#endregion
		public override void Read(XlsReader reader, int size) {
			sheetIdTable.Clear();
			int count = size / sizeof(short);
			for (int i = 0; i < count; i++)
				sheetIdTable.Add(reader.ReadInt16());
		}
		public override void Write(BinaryWriter writer) {
			int count = sheetIdTable.Count;
			for (int i = 0; i < count; i++)
				writer.Write((short)sheetIdTable[i]);
		}
		public override int GetSize() {
			return sheetIdTable.Count * sizeof(short);
		}
	}
	#endregion
	#region XlsContentWorkbookWindow
	public class XlsContentWorkbookWindow : XlsContentBase {
		#region Fields
		int horizontalPosition = 0;
		int verticalPosition = 0;
		int widthInTwips = 1;
		int heightInTwips = 1;
		int selectedTabIndex = 0;
		int firstDisplayedTabIndex = 0;
		int selectedTabsCount = 0;
		int tabRatio = 600;
		#endregion
		#region Properties
		public int HorizontalPosition {
			get { return horizontalPosition; }
			set { horizontalPosition = value; }
		}
		public int VerticalPosition {
			get { return verticalPosition; }
			set { verticalPosition = value; }
		}
		public int WidthInTwips {
			get { return widthInTwips; }
			set { widthInTwips = ValueInRange(value, 1, short.MaxValue); }
		}
		public int HeightInTwips {
			get { return heightInTwips; }
			set { heightInTwips = ValueInRange(value, 1, short.MaxValue); }
		}
		public bool IsHidden { get; set; }
		public bool IsMinimized { get; set; }
		public bool IsVeryHidden { get; set; }
		public bool HorizontalScrollDisplayed { get; set; }
		public bool VerticalScrollDisplayed { get; set; }
		public bool SheetTabsDisplayed { get; set; }
		public bool NoAutoFilterDateGrouping { get; set; }
		public int SelectedTabIndex {
			get { return selectedTabIndex; }
			set {
				CheckValue(value, 0, ushort.MaxValue, "SelectedTabIndex");
				selectedTabIndex = value;
			}
		}
		public int FirstDisplayedTabIndex {
			get { return firstDisplayedTabIndex; }
			set {
				CheckValue(value, 0, ushort.MaxValue, "FirstDisplayedTabIndex");
				firstDisplayedTabIndex = value;
			}
		}
		public int SelectedTabsCount {
			get { return selectedTabsCount; }
			set {
				CheckValue(value, 0, ushort.MaxValue, "SelectedTabsCount");
				selectedTabsCount = value;
			}
		}
		public int TabRatio {
			get { return tabRatio; }
			set {
				CheckValue(value, 0, 1000, "TabRatio");
				tabRatio = value;
			}
		}
		#endregion
		public override void Read(XlsReader reader, int size) {
			horizontalPosition = reader.ReadInt16();
			verticalPosition = reader.ReadInt16();
			widthInTwips = ValueInRange(reader.ReadUInt16(), 1, short.MaxValue);
			heightInTwips = ValueInRange(reader.ReadUInt16(), 1, short.MaxValue);
			ushort bitwiseField = reader.ReadUInt16();
			IsHidden = (bitwiseField & 0x0001) != 0;
			IsMinimized = (bitwiseField & 0x0002) != 0;
			IsVeryHidden = (bitwiseField & 0x0004) != 0;
			HorizontalScrollDisplayed = (bitwiseField & 0x0008) != 0;
			VerticalScrollDisplayed = (bitwiseField & 0x0010) != 0;
			SheetTabsDisplayed = (bitwiseField & 0x0020) != 0;
			NoAutoFilterDateGrouping = (bitwiseField & 0x0040) != 0;
			selectedTabIndex = reader.ReadInt16();
			firstDisplayedTabIndex = reader.ReadInt16();
			selectedTabsCount = reader.ReadInt16();
			tabRatio = reader.ReadInt16();
		}
		public override void Write(BinaryWriter writer) {
			writer.Write((short)horizontalPosition);
			writer.Write((short)verticalPosition);
			writer.Write((short)widthInTwips);
			writer.Write((short)heightInTwips);
			ushort bitwiseField = 0;
			if (IsHidden)
				bitwiseField |= 0x0001;
			if (IsMinimized)
				bitwiseField |= 0x0002;
			if (IsVeryHidden)
				bitwiseField |= 0x0004;
			if (HorizontalScrollDisplayed)
				bitwiseField |= 0x0008;
			if (VerticalScrollDisplayed)
				bitwiseField |= 0x0010;
			if (SheetTabsDisplayed)
				bitwiseField |= 0x0020;
			if (NoAutoFilterDateGrouping)
				bitwiseField |= 0x0040;
			writer.Write(bitwiseField);
			writer.Write((short)selectedTabIndex);
			writer.Write((short)firstDisplayedTabIndex);
			writer.Write((short)selectedTabsCount);
			writer.Write((short)tabRatio);
		}
		public override int GetSize() {
			return 18;
		}
	}
	#endregion
	#region XlsContentWorkbookBool
	public class XlsContentWorkbookBool : XlsContentBase {
		#region Properties
		public bool NotSaveExternalLinksValues { get; set; }
		public bool HasEnvelope { get; set; }
		public bool EnvelopeVisible { get; set; }
		public bool EnvelopeInitDone { get; set; }
		public XlsUpdateLinksMode UpdateLinksMode { get; set; }
		public bool HideBordersOfUnselTables { get; set; }
		#endregion
		public override void Read(XlsReader reader, int size) {
			ushort bitwiseField = reader.ReadUInt16();
			NotSaveExternalLinksValues = (bitwiseField & 0x0001) != 0;
			HasEnvelope = (bitwiseField & 0x0004) != 0;
			EnvelopeVisible = (bitwiseField & 0x0008) != 0;
			EnvelopeInitDone = (bitwiseField & 0x0010) != 0;
			UpdateLinksMode = (XlsUpdateLinksMode)((bitwiseField & 0x0060) >> 5);
			HideBordersOfUnselTables = (bitwiseField & 0x0100) != 0;
		}
		public override void Write(BinaryWriter writer) {
			ushort bitwiseField = 0;
			if (NotSaveExternalLinksValues)
				bitwiseField |= 0x0001;
			if (HasEnvelope || EnvelopeVisible || EnvelopeInitDone)
				bitwiseField |= 0x0004;
			if (EnvelopeVisible)
				bitwiseField |= 0x0008;
			if (EnvelopeInitDone)
				bitwiseField |= 0x0010;
			bitwiseField |= (ushort)(((int)UpdateLinksMode) << 5);
			if (HideBordersOfUnselTables)
				bitwiseField |= 0x0100;
			writer.Write(bitwiseField);
		}
		public override int GetSize() {
			return 2;
		}
	}
	#endregion
	#region XlsContentFont
	public class XlsContentFont : XlsContentBase {
		#region Fields
		const double fontCoeff = 20.0;
		const short defaultNormal = 400;
		const short defaultBoldness = 700;
		const int basePartSize = 14;
		short boldness = defaultNormal;
		ShortXLUnicodeString fontName = new ShortXLUnicodeString() { ForceHighBytes = true };
		#endregion
		#region Properties
		public double Size { get; set; }
		public bool Bold {
			get { return boldness >= defaultBoldness; }
			set {
				boldness = value ? defaultBoldness : defaultNormal;
			}
		}
		public bool Italic { get; set; }
		public bool StrikeThrough { get; set; }
		public bool Outline { get; set; }
		public bool Shadow { get; set; }
		public bool Condense { get; set; }
		public bool Extend { get; set; }
		public XlScriptType Script { get; set; }
		public XlUnderlineType Underline { get; set; }
		public int FontFamily { get; set; }
		public int Charset { get; set; }
		public string FontName {
			get { return fontName.Value; }
			set {
				if (!string.IsNullOrEmpty(value) && value.Length > XlsDefs.MaxFontNameLength)
					value = value.Substring(0, XlsDefs.MaxFontNameLength);
				fontName.Value = value;
			}
		}
		public int ColorIndex { get; set; }
		public short Boldness { get { return boldness; } }
		#endregion
		public override void Read(XlsReader reader, int size) {
			Size = reader.ReadInt16() / fontCoeff;
			ushort bitwiseField = reader.ReadUInt16();
			Italic = (bitwiseField & 0x02) != 0;
			StrikeThrough = (bitwiseField & 0x08) != 0;
			Outline = (bitwiseField & 0x10) != 0;
			Shadow = (bitwiseField & 0x20) != 0;
			Condense = (bitwiseField & 0x40) != 0;
			Extend = (bitwiseField & 0x80) != 0;
			ColorIndex = reader.ReadInt16();
			boldness = reader.ReadInt16();
			Script = (XlScriptType)reader.ReadInt16();
			Underline = (XlUnderlineType)reader.ReadByte();
			FontFamily = reader.ReadByte();
			Charset = reader.ReadByte();
			reader.ReadByte(); 
			fontName = ShortXLUnicodeString.FromStream(reader);
		}
		public override void Write(BinaryWriter writer) {
			writer.Write((short)(Size * fontCoeff));
			ushort bitwiseField = 0;
			if (Italic)
				bitwiseField |= 0x02;
			if (Underline == XlUnderlineType.Single)
				bitwiseField |= 0x04;
			if (StrikeThrough)
				bitwiseField |= 0x08;
			if (Outline)
				bitwiseField |= 0x10;
			if (Shadow)
				bitwiseField |= 0x20;
			if (Condense)
				bitwiseField |= 0x40;
			if (Extend)
				bitwiseField |= 0x80;
			writer.Write(bitwiseField);
			writer.Write((short)ColorIndex);
			if (Bold)
				writer.Write(defaultBoldness);
			else
				writer.Write(defaultNormal);
			writer.Write((short)Script);
			writer.Write((byte)Underline);
			writer.Write((byte)FontFamily);
			writer.Write((byte)Charset);
			writer.Write((byte)0);
			fontName.Write(writer);
		}
		public override int GetSize() {
			return basePartSize + fontName.Length;
		}
	}
	#endregion
	#region XlsContentNumberFormat
	public class XlsContentNumberFormat : XlsContentBase {
		#region Fields
		int formatId;
		XLUnicodeString formatCode = new XLUnicodeString();
		#endregion
		#region Properties
		public int FormatId {
			get { return formatId; }
			set {
				CheckValue(value, 0, ushort.MaxValue, "FormatId");
				formatId = value;
			}
		}
		public string FormatCode {
			get { return formatCode.Value; }
			set { formatCode.Value = value; }
		}
		#endregion
		public override void Read(XlsReader reader, int size) {
			formatId = reader.ReadUInt16();
			formatCode = XLUnicodeString.FromStream(reader);
		}
		public override void Write(BinaryWriter writer) {
			writer.Write((ushort)formatId);
			formatCode.Write(writer);
		}
		public override int GetSize() {
			return formatCode.Length + 2;
		}
	}
	#endregion
	#region XlsContentXF
	public class XlsContentXF : XlsContentBase {
		#region Fields
		readonly MsoCrc32Compute crc32 = new MsoCrc32Compute();
		#endregion
		#region Properties
		public bool IsStyleFormat { get; set; }
		public int FontId { get; set; }
		public int NumberFormatId { get; set; }
		public bool IsLocked { get; set; }
		public bool IsHidden { get; set; }
		public bool QuotePrefix { get; set; }
		public int StyleId { get; set; }
		public XlHorizontalAlignment HorizontalAlignment { get; set; }
		public bool WrapText { get; set; }
		public XlVerticalAlignment VerticalAlignment { get; set; }
		public int TextRotation { get; set; }
		public byte Indent { get; set; }
		public bool ShrinkToFit { get; set; }
		public XlReadingOrder ReadingOrder { get; set; }
		public bool ApplyNumberFormat { get; set; }
		public bool ApplyFont { get; set; }
		public bool ApplyAlignment { get; set; }
		public bool ApplyBorder { get; set; }
		public bool ApplyFill { get; set; }
		public bool ApplyProtection { get; set; }
		public XlBorderLineStyle BorderLeftLineStyle { get; set; }
		public XlBorderLineStyle BorderRightLineStyle { get; set; }
		public XlBorderLineStyle BorderTopLineStyle { get; set; }
		public XlBorderLineStyle BorderBottomLineStyle { get; set; }
		public int BorderLeftColorIndex { get; set; }
		public int BorderRightColorIndex { get; set; }
		public bool BorderDiagonalDown { get; set; }
		public bool BorderDiagonalUp { get; set; }
		public int BorderTopColorIndex { get; set; }
		public int BorderBottomColorIndex { get; set; }
		public int BorderDiagonalColorIndex { get; set; }
		public XlBorderLineStyle BorderDiagonalLineStyle { get; set; }
		public bool HasExtension { get; set; }
		public XlPatternType FillPatternType { get; set; }
		public int FillForeColorIndex { get; set; }
		public int FillBackColorIndex { get; set; }
		public bool PivotButton { get; set; }
		public int CrcValue { get { return crc32.CrcValue; } set { crc32.CrcValue = value; } }
		protected MsoCrc32Compute Crc32 { get { return crc32; } }
		#endregion
		public override void Read(XlsReader reader, int size) {
			int fontId = reader.ReadInt16();
			Crc32.Add((short)fontId);
			if (fontId == XlsDefs.UnusedFontIndex)
				fontId = 0;
			else if (fontId > XlsDefs.UnusedFontIndex)
				fontId--;
			FontId = fontId;
			NumberFormatId = reader.ReadInt16();
			Crc32.Add((short)NumberFormatId);
			int bitwiseField = reader.ReadInt16();
			Crc32.Add((short)bitwiseField);
			IsStyleFormat = Convert.ToBoolean(bitwiseField & 0x0004);
			if (!IsStyleFormat) {
				QuotePrefix = Convert.ToBoolean(bitwiseField & 0x0008);
				StyleId = (bitwiseField & 0xfff0) >> 4;
			}
			IsLocked = Convert.ToBoolean(bitwiseField & 0x0001);
			IsHidden = Convert.ToBoolean(bitwiseField & 0x0002);
			bitwiseField = reader.ReadUInt16();
			Crc32.Add((short)bitwiseField);
			HorizontalAlignment = (XlHorizontalAlignment)(bitwiseField & 0x0007);
			WrapText = Convert.ToBoolean(bitwiseField & 0x0008);
			VerticalAlignment = (XlVerticalAlignment)((bitwiseField & 0x0070) >> 4);
			TextRotation = (bitwiseField & 0xff00) >> 8;
			bitwiseField = reader.ReadUInt16();
			Crc32.Add((short)bitwiseField);
			Indent = (byte)(bitwiseField & 0x000f);
			ShrinkToFit = Convert.ToBoolean(bitwiseField & 0x0010);
			ReadingOrder = (XlReadingOrder)((bitwiseField & 0x00c0) >> 6);
			if (IsStyleFormat) {
				ApplyNumberFormat = !Convert.ToBoolean(bitwiseField & 0x0400);
				ApplyFont = !Convert.ToBoolean(bitwiseField & 0x0800);
				ApplyAlignment = !Convert.ToBoolean(bitwiseField & 0x1000);
				ApplyBorder = !Convert.ToBoolean(bitwiseField & 0x2000);
				ApplyFill = !Convert.ToBoolean(bitwiseField & 0x4000);
				ApplyProtection = !Convert.ToBoolean(bitwiseField & 0x8000);
			}
			else {
				ApplyNumberFormat = Convert.ToBoolean(bitwiseField & 0x0400);
				ApplyFont = Convert.ToBoolean(bitwiseField & 0x0800);
				ApplyAlignment = Convert.ToBoolean(bitwiseField & 0x1000);
				ApplyBorder = Convert.ToBoolean(bitwiseField & 0x2000);
				ApplyFill = Convert.ToBoolean(bitwiseField & 0x4000);
				ApplyProtection = Convert.ToBoolean(bitwiseField & 0x8000);
			}
			bitwiseField = reader.ReadUInt16();
			Crc32.Add((short)bitwiseField);
			BorderLeftLineStyle = (XlBorderLineStyle)(bitwiseField & 0x000f);
			BorderRightLineStyle = (XlBorderLineStyle)((bitwiseField & 0x00f0) >> 4);
			BorderTopLineStyle = (XlBorderLineStyle)((bitwiseField & 0x0f00) >> 8);
			BorderBottomLineStyle = (XlBorderLineStyle)((bitwiseField & 0xf000) >> 12);
			bitwiseField = reader.ReadUInt16();
			Crc32.Add((short)bitwiseField);
			BorderLeftColorIndex = bitwiseField & 0x007f;
			BorderRightColorIndex = (bitwiseField & 0x3f80) >> 7;
			BorderDiagonalDown = Convert.ToBoolean(bitwiseField & 0x4000);
			BorderDiagonalUp = Convert.ToBoolean(bitwiseField & 0x8000);
			bitwiseField = reader.ReadInt32();
			Crc32.Add((int)bitwiseField);
			BorderTopColorIndex = bitwiseField & 0x0000007f;
			BorderBottomColorIndex = (bitwiseField & 0x00003f80) >> 7;
			BorderDiagonalColorIndex = (bitwiseField & 0x001fc000) >> 14;
			BorderDiagonalLineStyle = (XlBorderLineStyle)((bitwiseField & 0x1e00000) >> 21);
			HasExtension = Convert.ToBoolean(bitwiseField & 0x02000000);
			FillPatternType = (XlPatternType)((bitwiseField & 0xfc000000) >> 26);
			bitwiseField = reader.ReadUInt16();
			Crc32.Add((short)bitwiseField);
			FillForeColorIndex = bitwiseField & 0x007f;
			FillBackColorIndex = (bitwiseField & 0x3f80) >> 7;
			if (!IsStyleFormat)
				PivotButton = Convert.ToBoolean(bitwiseField & 0x4000);
		}
		public override void Write(BinaryWriter writer) {
			int fontId = FontId;
			if (fontId >= XlsDefs.UnusedFontIndex)
				fontId++;
			writer.Write((short)fontId);
			Crc32.Add((short)fontId);
			writer.Write((short)NumberFormatId);
			Crc32.Add((short)NumberFormatId);
			int bitwiseField = 0;
			if (IsLocked)
				bitwiseField |= 0x0001;
			if (IsHidden)
				bitwiseField |= 0x0002;
			if (IsStyleFormat)
				bitwiseField |= 0x0004;
			if (!IsStyleFormat) {
				if (QuotePrefix)
					bitwiseField |= 0x0008;
				bitwiseField |= (StyleId & 0x0fff) << 4;
			}
			else
				bitwiseField |= 0xfff0;
			writer.Write((ushort)bitwiseField);
			Crc32.Add((short)bitwiseField);
			bitwiseField = (int)HorizontalAlignment;
			if (WrapText)
				bitwiseField |= 0x0008;
			bitwiseField |= (int)VerticalAlignment << 4;
			bitwiseField |= (TextRotation & 0x00ff) << 8;
			writer.Write((ushort)bitwiseField);
			Crc32.Add((short)bitwiseField);
			bitwiseField = Indent & 0x0f;
			if (ShrinkToFit)
				bitwiseField |= 0x0010;
			bitwiseField |= (int)ReadingOrder << 6;
			bitwiseField = SetApplyProperties(bitwiseField);
			writer.Write((ushort)bitwiseField);
			Crc32.Add((short)bitwiseField);
			bitwiseField = (int)BorderLeftLineStyle;
			bitwiseField |= (int)BorderRightLineStyle << 4;
			bitwiseField |= (int)BorderTopLineStyle << 8;
			bitwiseField |= (int)BorderBottomLineStyle << 12;
			writer.Write((ushort)bitwiseField);
			Crc32.Add((short)bitwiseField);
			bitwiseField = BorderLeftColorIndex & 0x007f;
			bitwiseField |= (BorderRightColorIndex & 0x007f) << 7;
			if (BorderDiagonalDown)
				bitwiseField |= 0x4000;
			if (BorderDiagonalUp)
				bitwiseField |= 0x8000;
			writer.Write((ushort)bitwiseField);
			Crc32.Add((short)bitwiseField);
			bitwiseField = BorderTopColorIndex & 0x007f;
			bitwiseField |= (BorderBottomColorIndex & 0x007f) << 7;
			bitwiseField |= (BorderDiagonalColorIndex & 0x007f) << 14;
			bitwiseField |= (int)BorderDiagonalLineStyle << 21;
			if (HasExtension && !IsStyleFormat)
				bitwiseField |= 0x02000000;
			bitwiseField |= (int)FillPatternType << 26;
			writer.Write(bitwiseField);
			Crc32.Add(bitwiseField);
			bitwiseField = FillForeColorIndex & 0x007f;
			bitwiseField |= (FillBackColorIndex & 0x007f) << 7;
			if (!IsStyleFormat && PivotButton)
				bitwiseField |= 0x4000;
			writer.Write((ushort)bitwiseField);
			Crc32.Add((short)bitwiseField);
		}
		public override int GetSize() {
			return 0x14;
		}
		int SetApplyProperties(int bitwiseField) {
			if (IsStyleFormat) {
				if (!ApplyNumberFormat)
					bitwiseField |= 0x0400;
				if (!ApplyFont)
					bitwiseField |= 0x0800;
				if (!ApplyAlignment)
					bitwiseField |= 0x1000;
				if (!ApplyBorder)
					bitwiseField |= 0x2000;
				if (!ApplyFill)
					bitwiseField |= 0x4000;
				if (!ApplyProtection)
					bitwiseField |= 0x8000;
			}
			else {
				if (ApplyNumberFormat)
					bitwiseField |= 0x0400;
				if (ApplyFont)
					bitwiseField |= 0x0800;
				if (ApplyAlignment)
					bitwiseField |= 0x1000;
				if (ApplyBorder)
					bitwiseField |= 0x2000;
				if (ApplyFill)
					bitwiseField |= 0x4000;
				if (ApplyProtection)
					bitwiseField |= 0x8000;
			}
			return bitwiseField;
		}
	}
	#endregion
	#region XlsContentXFCrc
	public class XlsContentXFCrc : XlsContentBase {
		int xfCount;
		FutureRecordHeader recordHeader = new FutureRecordHeader();
		#region Properties
		public int XFCount {
			get { return xfCount; }
			set {
				CheckValue(value, 0, XlsDefs.MaxXFCount, "XFCount");
				xfCount = value;
			}
		}
		public int XFCRC { get; set; }
		public override FutureRecordHeaderBase RecordHeader { get { return recordHeader; } }
		#endregion
		public override void Read(XlsReader reader, int size) {
			recordHeader = FutureRecordHeader.FromStream(reader);
			reader.ReadUInt16(); 
			XFCount = reader.ReadUInt16();
			XFCRC = reader.ReadInt32();
		}
		public override void Write(BinaryWriter writer) {
			recordHeader.Write(writer);
			writer.Write((ushort)0); 
			writer.Write((ushort)XFCount);
			writer.Write(XFCRC);
		}
		public override int GetSize() {
			return 20;
		}
	}
	#endregion
	#region XlsContentXFExt
	public class XlsContentXFExt : XlsContentBase {
		int xfIndex;
		readonly FutureRecordHeader recordHeader = new FutureRecordHeader();
		readonly XfProperties properties = new XfProperties();
		#region Properties
		public int XFIndex {
			get { return xfIndex; }
			set {
				CheckValue(value, 0, XlsDefs.MaxXFCount, "XFIndex");
				xfIndex = value;
			}
		}
		public XfProperties Properties { get { return properties; } }
		public override FutureRecordHeaderBase RecordHeader { get { return recordHeader; } }
		#endregion
		public override void Read(XlsReader reader, int size) {
			reader.ReadBytes(size);
		}
		public override void Write(BinaryWriter writer) {
			recordHeader.Write(writer);
			writer.Write((ushort)0); 
			writer.Write((ushort)XFIndex);
			properties.Write(writer);
		}
		public override int GetSize() {
			return 16 + properties.GetSize();
		}
	}
	#endregion
	#region XlsContentStyle
	public class XlsContentStyle : XlsContentBase {
		const short basePartSize = 2;
		XLUnicodeString styleName = new XLUnicodeString();
		#region Properties
		public string StyleName {
			get { return styleName.Value; }
			set { styleName.Value = value; }
		}
		public bool IsHidden { get; set; }
		public int BuiltInId { get; set; }
		public int OutlineLevel { get; set; }
		public int StyleFormatId { get; set; }
		public bool IsBuiltIn { get; set; }
		#endregion
		public XlsContentStyle() {
			BuiltInId = Int32.MinValue;
			OutlineLevel = Int32.MinValue;
		}
		public override void Read(XlsReader reader, int size) {
			ushort bitwiseField = reader.ReadUInt16();
			StyleFormatId = bitwiseField & 0x0fff;
			IsBuiltIn = Convert.ToBoolean(bitwiseField & 0x8000);
			if (!IsBuiltIn) {
				BuiltInId = Int32.MinValue;
				OutlineLevel = Int32.MinValue;
				styleName = XLUnicodeString.FromStream(reader);
			}
			else {
				BuiltInId = reader.ReadByte();
				OutlineLevel = reader.ReadByte() + 1;
				if (BuiltInId != 0x01 && BuiltInId != 0x02)
					OutlineLevel = Int32.MinValue;
				StyleName = string.Empty;
			}
		}
		public override void Write(BinaryWriter writer) {
			int bitwiseField = StyleFormatId;
			if (IsBuiltIn)
				bitwiseField |= 0x8000;
			writer.Write((ushort)bitwiseField);
			if (!IsBuiltIn)
				styleName.Write(writer);
			else {
				writer.Write((byte)BuiltInId);
				if (BuiltInId == 0x01 || BuiltInId == 0x02)
					writer.Write((byte)(OutlineLevel - 1));
				else
					writer.Write((byte)0xff);
			}
		}
		public override int GetSize() {
			if (IsBuiltIn)
				return basePartSize + 2;
			return basePartSize + styleName.Length;
		}
	}
	#endregion
	#region XlsContentStyleExt
	public class XlsContentStyleExt : XlsContentBase {
		#region Fields
		readonly FutureRecordHeader recordHeader = new FutureRecordHeader();
		LPWideString styleName = new LPWideString();
		readonly XfProperties properties = new XfProperties();
		#endregion
		#region Properties
		public bool IsBuiltIn { get; set; }
		public bool IsHidden { get; set; }
		public bool CustomBuiltIn { get; set; }
		public XlStyleCategory Category { get; set; }
		public int BuiltInId { get; set; }
		public int OutlineLevel { get; set; }
		public string StyleName {
			get { return this.styleName.Value; }
			set { this.styleName.Value = value; }
		}
		public XfProperties Properties { get { return properties; } }
		public override FutureRecordHeaderBase RecordHeader { get { return recordHeader; } }
		#endregion
		public override void Read(XlsReader reader, int size) {
			reader.ReadBytes(size);
		}
		public override void Write(BinaryWriter writer) {
			recordHeader.Write(writer);
			byte bitwiseField = 0;
			if (IsBuiltIn)
				bitwiseField |= 0x01;
			if (IsHidden)
				bitwiseField |= 0x02;
			if (CustomBuiltIn)
				bitwiseField |= 0x04;
			writer.Write(bitwiseField);
			writer.Write((byte)Category);
			if (!IsBuiltIn) {
				writer.Write((ushort)0xffff);
			}
			else {
				writer.Write((byte)BuiltInId);
				if (BuiltInId == 0x01 || BuiltInId == 0x02)
					writer.Write((byte)(OutlineLevel - 1));
				else
					writer.Write((byte)0xff);
			}
			this.styleName.Write(writer);
			properties.Write(writer);
		}
		public override int GetSize() {
			return 16 + this.styleName.Length + this.properties.GetSize();
		}
	}
	#endregion
	#region XlsContentBoundSheet8
	public class XlsContentBoundSheet8 : XlsContentBase {
		#region Fields
		ShortXLUnicodeString name = new ShortXLUnicodeString();
		#endregion
		#region Properties
		public int StartPosition { get; set; }
		public XlSheetVisibleState VisibleState { get; set; }
		public string Name {
			get { return name.Value; }
			set { name.Value = value; }
		}
		#endregion
		public override void Read(XlsReader reader, int size) {
			reader.ReadBytes(size);
		}
		public override void Write(BinaryWriter writer) {
			writer.Write(StartPosition);
			writer.Write((ushort)VisibleState);
			this.name.Write(writer);
		}
		public override int GetSize() {
			return 6 + this.name.Length;
		}
	}
	#endregion
	#region XlsContentCountry
	public class XlsContentCountry : XlsContentBase {
		#region Properties
		public int DefaultCountryIndex { get; set; }
		public int CountryIndex { get; set; }
		#endregion
		public override void Read(XlsReader reader, int size) {
			DefaultCountryIndex = reader.ReadUInt16();
			CountryIndex = reader.ReadUInt16();
		}
		public override void Write(BinaryWriter writer) {
			writer.Write((ushort)DefaultCountryIndex);
			writer.Write((ushort)CountryIndex);
		}
		public override int GetSize() {
			return 4;
		}
	}
	#endregion
	#region XlsContentExtSST
	public class XlsContentExtSST : XlsContentBase {
		#region Fields
		int stringsInBucket = XlsDefs.MinStringsInBucket;
		readonly List<XlsSSTInfo> items = new List<XlsSSTInfo>();
		#endregion
		#region Properties
		public int StringsInBucket {
			get { return stringsInBucket; }
			set {
				Guard.ArgumentNonNegative(value, "StringsInBucket value");
				stringsInBucket = value;
			}
		}
		public List<XlsSSTInfo> Items { get { return items; } }
		#endregion
		public override void Read(XlsReader reader, int size) {
		}
		public override void Write(BinaryWriter writer) {
			writer.Write((ushort)StringsInBucket);
			foreach (XlsSSTInfo item in items) {
				writer.Write(item.StreamPosition);
				writer.Write((ushort)item.Offset);
				writer.Write((ushort)0x00); 
			}
		}
		public override int GetSize() {
			return 2 + items.Count * 8;
		}
	}
	#endregion
	#region XlsContentSupBookSelf
	public class XlsContentSupBookSelf : XlsContentBase {
		const short selfRef = 0x0401;
		public int SheetCount { get; set; }
		public override void Read(XlsReader reader, int size) {
		}
		public override void Write(BinaryWriter writer) {
			writer.Write((ushort)SheetCount);
			writer.Write(selfRef);
		}
		public override int GetSize() {
			return 4;
		}
	}
	#endregion
	#region XlsContentDefinedName
	public class XlsContentDefinedName : XlsContentBase {
		#region Fields
		const int fixedPartSize = 14;
		static readonly string[] builtInNames = new string[] { 
			"_xlnm.Consolidate_Area", 
			"_xlnm.Auto_Open", 
			"_xlnm.Auto_Close", 
			"_xlnm.Extract", 
			"_xlnm.Database", 
			"_xlnm.Criteria", 
			"_xlnm.Print_Area", 
			"_xlnm.Print_Titles", 
			"_xlnm.Recorder", 
			"_xlnm.Data_Form", 
			"_xlnm.Auto_Activate", 
			"_xlnm.Auto_Deactivate", 
			"_xlnm.Sheet_Title", 
			"_xlnm._FilterDatabase" 
		};
		int functionCategory;
		int sheetIndex;
		XLUnicodeStringNoCch name = new XLUnicodeStringNoCch();
		byte[] formulaBytes = new byte[] { 0x00, 0x00 };
		int formulaSize;
		#endregion
		#region Properties
		public bool IsHidden { get; set; }
		public bool IsXlmMacro { get; set; }
		public bool IsVbaMacro { get; set; }
		public bool IsMacro { get; set; }
		public bool CanReturnArray { get; set; }
		public bool IsBuiltIn { get; private set; }
		public int FunctionCategory {
			get { return functionCategory; }
			set {
				CheckValue(value, 0, 31, "FunctionCategory");
				functionCategory = value;
			}
		}
		public bool IsPublished { get; set; }
		public bool IsWorkbookParameter { get; set; }
		public byte Key { get; set; }
		public int SheetIndex {
			get { return sheetIndex; }
			set {
				CheckValue(value, 0, ushort.MaxValue, "SheetIndex");
				sheetIndex = value;
			}
		}
		public string Name {
			get {
				if (!IsBuiltIn)
					return name.Value;
				if (name.Value.Length == 1) {
					int typeCode = Convert.ToUInt16(name.Value[0]);
					if (typeCode >= 0 && typeCode < builtInNames.Length)
						return builtInNames[typeCode];
				}
				return "_xlnm." + name.Value;
			}
			set {
				if (!string.IsNullOrEmpty(value) && value.StartsWith("_xlnm.")) {
					int index = -1;
					for (int i = 0; i < builtInNames.Length; i++) {
						if (value == builtInNames[i]) {
							index = i;
							break;
						}
					}
					if (index != -1) {
						name.Value = Char.ToString((char)index);
						IsBuiltIn = true;
						return;
					}
				}
				name.Value = value;
				IsBuiltIn = false;
			}
		}
		public string Comment { get; set; }
		public string InternalName { get { return name.Value; } }
		public byte[] FormulaBytes {
			get { return formulaBytes; }
			set {
				if (value == null || value.Length == 0) {
					this.formulaBytes = new byte[] { 0x00, 0x00 };
					this.formulaSize = 0;
				}
				else {
					if (value.Length < 2)
						throw new ArgumentException("value must be at least 2 bytes long");
					this.formulaBytes = value;
					this.formulaSize = BitConverter.ToUInt16(formulaBytes, 0);
				}
			}
		}
		public int FormulaSize { get { return formulaSize; } }
		#endregion
		public override void Read(XlsReader reader, int size) {
			ushort bitwiseField = reader.ReadUInt16();
			IsHidden = Convert.ToBoolean(bitwiseField & 0x0001);
			IsXlmMacro = Convert.ToBoolean(bitwiseField & 0x0002);
			IsVbaMacro = Convert.ToBoolean(bitwiseField & 0x0004);
			IsMacro = Convert.ToBoolean(bitwiseField & 0x0008);
			CanReturnArray = Convert.ToBoolean(bitwiseField & 0x0010);
			IsBuiltIn = Convert.ToBoolean(bitwiseField & 0x0020);
			FunctionCategory = (bitwiseField & 0x0fc0) >> 6;
			IsPublished = Convert.ToBoolean(bitwiseField & 0x2000);
			IsWorkbookParameter = Convert.ToBoolean(bitwiseField & 0x4000);
			Key = reader.ReadByte();
			int charCount = reader.ReadByte();
			formulaSize = reader.ReadUInt16();
			reader.ReadUInt16(); 
			SheetIndex = reader.ReadUInt16();
			reader.ReadUInt32(); 
			this.name = XLUnicodeStringNoCch.FromStream(reader, charCount);
			if (formulaSize > 0) {
				int formulaBytesCount = size - GetSizeWithoutFormula();
				formulaBytes = reader.ReadBytes(formulaBytesCount);
			}
		}
		public override void Write(BinaryWriter writer) {
			ushort bitwiseField = 0;
			if (IsHidden)
				bitwiseField |= 0x0001;
			if (IsXlmMacro)
				bitwiseField |= 0x0002;
			if (IsVbaMacro)
				bitwiseField |= 0x0004;
			if (IsMacro)
				bitwiseField |= 0x0008;
			if (CanReturnArray)
				bitwiseField |= 0x0010;
			if (IsBuiltIn)
				bitwiseField |= 0x0020;
			bitwiseField |= (ushort)((FunctionCategory & 0x3f) << 6);
			if (IsPublished)
				bitwiseField |= 0x2000;
			if (IsWorkbookParameter)
				bitwiseField |= 0x4000;
			writer.Write(bitwiseField);
			writer.Write(Key);
			writer.Write((byte)this.name.Value.Length);
			writer.Write(BitConverter.ToUInt16(formulaBytes, 0));
			writer.Write((ushort)0); 
			writer.Write((ushort)SheetIndex);
			writer.Write((uint)0); 
			this.name.Write(writer);
			if (formulaBytes.Length > 2)
				writer.Write(formulaBytes, 2, formulaBytes.Length - 2);
		}
		public override int GetSize() {
			return GetSizeWithoutFormula() + formulaBytes.Length - 2;
		}
		int GetSizeWithoutFormula() {
			return fixedPartSize + name.Length;
		}
	}
	#endregion
	#region XlsXTI
	public class XlsXTI {
		public static XlsXTI FromStream(BinaryReader reader) {
			XlsXTI result = new XlsXTI();
			result.Read(reader);
			return result;
		}
		public int SupBookIndex { get; set; }
		public int FirstSheetIndex { get; set; }
		public int LastSheetIndex { get; set; }
		protected void Read(BinaryReader reader) {
			SupBookIndex = reader.ReadUInt16();
			FirstSheetIndex = reader.ReadInt16();
			LastSheetIndex = reader.ReadInt16();
		}
		public void Write(BinaryWriter writer) {
			writer.Write((ushort)SupBookIndex);
			writer.Write((short)FirstSheetIndex);
			writer.Write((short)LastSheetIndex);
		}
		public override bool Equals(object obj) {
			XlsXTI other = obj as XlsXTI;
			if (other == null)
				return false;
			return SupBookIndex == other.SupBookIndex &&
				FirstSheetIndex == other.FirstSheetIndex &&
				LastSheetIndex == other.LastSheetIndex;
		}
		public override int GetHashCode() {
			return SupBookIndex ^ FirstSheetIndex ^ LastSheetIndex;
		}
	}
	#endregion
	#region XlsContentTheme
	public class XlsContentTheme : XlsContentBase {
		byte[] themeContent = new byte[0];
		readonly FutureRecordHeader recordHeader = new FutureRecordHeader();
		#region Properties
		public int ThemeVersion { get; set; }
		public override FutureRecordHeaderBase RecordHeader { get { return recordHeader; } }
		public byte[] ThemeContent {
			get { return themeContent; }
			set {
				if(value == null)
					themeContent = new byte[0];
				else
					themeContent = value;
			}
		}
		#endregion
		public override void Read(XlsReader reader, int size) {
			reader.ReadBytes(size);
		}
		public override void Write(BinaryWriter writer) {
			recordHeader.Write(writer);
			writer.Write((uint)ThemeVersion);
			writer.Write(themeContent);
		}
		public override int GetSize() {
			return 16 + themeContent.Length;
		}
	}
	#endregion
	#region XlsContentIndex
	public class XlsContentIndex : XlsContentBase {
		#region Fields
		protected const int VariablePartElementSize = 4;
		protected const int FixedPartSize = 16;
		int firstRowIndex;
		int lastRowIndex;
		long defaultColumnWidthOffset;
		readonly List<long> dbCellsPositions = new List<long>();
		#endregion
		#region Properties
		public int FirstRowIndex { get { return firstRowIndex; } set { firstRowIndex = value; } }
		public int LastRowIndex { get { return lastRowIndex; } set { lastRowIndex = value; } }
		public long DefaultColumnWidthOffset { get { return defaultColumnWidthOffset; } set { defaultColumnWidthOffset = value; } }
		public List<long> DbCellsPositions { get { return dbCellsPositions; } }
		#endregion
		public override void Read(XlsReader reader, int size) {
			reader.ReadInt32(); 
			this.firstRowIndex = reader.ReadInt32();
			this.lastRowIndex = reader.ReadInt32();
			this.defaultColumnWidthOffset = reader.ReadUInt32();
			int count = (size - FixedPartSize) / VariablePartElementSize;
			for (int i = 0; i < count; i++) {
				this.dbCellsPositions.Add(reader.ReadUInt32());
			}
		}
		public override void Write(BinaryWriter writer) {
			writer.Write(0);
			writer.Write(this.firstRowIndex);
			writer.Write(this.lastRowIndex);
			writer.Write((uint)this.defaultColumnWidthOffset);
			int count = this.dbCellsPositions.Count;
			for (int i = 0; i < count; i++) {
				writer.Write((uint)this.dbCellsPositions[i]);
			}
		}
		public override int GetSize() {
			return FixedPartSize + this.dbCellsPositions.Count * VariablePartElementSize;
		}
	}
	#endregion
	#region XlsContentGuts
	public class XlsContentGuts : XlsContentBase {
		int rowGutterMaxOutlineLevel;
		int columnGutterMaxOutlineLevel;
		#region Properties
		public int RowGutterMaxOutlineLevel {
			get { return rowGutterMaxOutlineLevel; }
			set {
				CheckValue(value, 0, 7, "RowGutterMaxOutlineLevel");
				rowGutterMaxOutlineLevel = value;
			}
		}
		public int ColumnGutterMaxOutlineLevel {
			get { return columnGutterMaxOutlineLevel; }
			set {
				CheckValue(value, 0, 7, "ColumnGutterMaxOutlineLevel");
				columnGutterMaxOutlineLevel = value;
			}
		}
		#endregion
		public override void Read(XlsReader reader, int size) {
			reader.ReadUInt16();
			reader.ReadUInt16();
			RowGutterMaxOutlineLevel = DecodeOutlineLevel(reader.ReadUInt16());
			ColumnGutterMaxOutlineLevel = DecodeOutlineLevel(reader.ReadUInt16());
		}
		public override void Write(BinaryWriter writer) {
			writer.Write((ushort)0);
			writer.Write((ushort)0);
			writer.Write((ushort)EncodeOutlineLevel(RowGutterMaxOutlineLevel));
			writer.Write((ushort)EncodeOutlineLevel(ColumnGutterMaxOutlineLevel));
		}
		public override int GetSize() {
			return 8;
		}
		int EncodeOutlineLevel(int value) {
			if (value == 0)
				return 0;
			return value + 1;
		}
		int DecodeOutlineLevel(int value) {
			if (value == 0)
				return 0;
			return value - 1;
		}
	}
	#endregion
	#region XlsContentDefaultRowHeight
	public class XlsContentDefaultRowHeight : XlsContentBase {
		#region Fields
		int defaultRowHeightInTwips;
		#endregion
		#region Properties
		public bool CustomHeight { get; set; }
		public bool ZeroHeightOnEmptyRows { get; set; }
		public bool ThickTopBorder { get; set; }
		public bool ThickBottomBorder { get; set; }
		public int DefaultRowHeightInTwips {
			get { return defaultRowHeightInTwips; }
			set {
				if (value > XlsDefs.MaxDefaultRowHeight)
					value = XlsDefs.MaxDefaultRowHeight;
				defaultRowHeightInTwips = value;
			}
		}
		#endregion
		public override void Read(XlsReader reader, int size) {
			short bitwiseField = reader.ReadInt16();
			CustomHeight = Convert.ToBoolean(bitwiseField & 0x01);
			ZeroHeightOnEmptyRows = Convert.ToBoolean(bitwiseField & 0x02);
			ThickTopBorder = Convert.ToBoolean(bitwiseField & 0x04);
			ThickBottomBorder = Convert.ToBoolean(bitwiseField & 0x08);
			DefaultRowHeightInTwips = reader.ReadInt16();
		}
		public override void Write(BinaryWriter writer) {
			short bitwiseField = 0;
			if (CustomHeight)
				bitwiseField |= 0x01;
			if (ZeroHeightOnEmptyRows)
				bitwiseField |= 0x02;
			if (ThickTopBorder)
				bitwiseField |= 0x04;
			if (ThickBottomBorder)
				bitwiseField |= 0x08;
			writer.Write(bitwiseField);
			writer.Write((short)DefaultRowHeightInTwips);
		}
		public override int GetSize() {
			return 4;
		}
	}
	#endregion
	#region XlsContentWsBool
	public class XlsContentWsBool : XlsContentBase {
		#region Properties
		public bool ShowPageBreaks { get; set; }
		public bool IsDialog { get; set; }
		public bool ApplyStyles { get; set; }
		public bool ShowRowSumsBelow { get; set; }
		public bool ShowColumnSumsRight { get; set; }
		public bool FitToPage { get; set; }
		public bool SynchronizeHorizontalScrolling { get; set; }
		public bool SynchronizeVerticalScrolling { get; set; }
		public bool TransitionFormulaEvaluation { get; set; }
		public bool TransitionFormulaEntry { get; set; }
		#endregion
		public override void Read(XlsReader reader, int size) {
			short bitwiseField = reader.ReadInt16();
			ShowPageBreaks = Convert.ToBoolean(bitwiseField & 0x0001);
			IsDialog = Convert.ToBoolean(bitwiseField & 0x0010);
			ApplyStyles = Convert.ToBoolean(bitwiseField & 0x0020);
			ShowRowSumsBelow = Convert.ToBoolean(bitwiseField & 0x0040);
			ShowColumnSumsRight = Convert.ToBoolean(bitwiseField & 0x0080);
			FitToPage = Convert.ToBoolean(bitwiseField & 0x0100);
			SynchronizeHorizontalScrolling = Convert.ToBoolean(bitwiseField & 0x1000);
			SynchronizeVerticalScrolling = Convert.ToBoolean(bitwiseField & 0x2000);
			TransitionFormulaEvaluation = Convert.ToBoolean(bitwiseField & 0x4000);
			TransitionFormulaEntry = Convert.ToBoolean(bitwiseField & 0x8000);
		}
		public override void Write(BinaryWriter writer) {
			int bitwiseField = 0;
			if (ShowPageBreaks)
				bitwiseField |= 0x0001;
			if (IsDialog)
				bitwiseField |= 0x0010;
			if (ApplyStyles)
				bitwiseField |= 0x0020;
			if (ShowRowSumsBelow)
				bitwiseField |= 0x0040;
			if (ShowColumnSumsRight)
				bitwiseField |= 0x0080;
			if (FitToPage)
				bitwiseField |= 0x0100;
			if (SynchronizeHorizontalScrolling)
				bitwiseField |= 0x1000;
			if (SynchronizeVerticalScrolling)
				bitwiseField |= 0x2000;
			if (TransitionFormulaEvaluation)
				bitwiseField |= 0x4000;
			if (TransitionFormulaEntry)
				bitwiseField |= 0x8000;
			writer.Write((short)bitwiseField);
		}
		public override int GetSize() {
			return 2;
		}
	}
	#endregion
	#region XlsContentPageSetup
	public class XlsContentPageSetup : XlsContentBase {
		#region Properties
		public PaperKind PaperKind { get; set; }
		public int Scale { get; set; }
		public int FirstPageNumber { get; set; }
		public int FitToWidth { get; set; }
		public int FitToHeight { get; set; }
		public XlPagePrintOrder PagePrintOrder { get; set; }
		public XlCommentsPrintMode CommentsPrintMode { get; set; }
		public XlErrorsPrintMode ErrorsPrintMode { get; set; }
		public XlPageOrientation PageOrientation { get; set; }
		public bool BlackAndWhite { get; set; }
		public bool Draft { get; set; }
		public bool UseFirstPageNumber { get; set; }
		public int HorizontalDpi { get; set; }
		public int VerticalDpi { get; set; }
		public double HeaderMargin { get; set; }
		public double FooterMargin { get; set; }
		public int Copies { get; set; }
		#endregion
		public override void Read(XlsReader reader, int size) {
			PaperKind = ConvertToPaperKind(reader.ReadInt16());
			Scale = reader.ReadInt16();
			FirstPageNumber = reader.ReadInt16();
			FitToWidth = reader.ReadInt16();
			FitToHeight = reader.ReadInt16();
			short bitwiseField = reader.ReadInt16();
			bool NoPls = Convert.ToBoolean(bitwiseField & 0x04);
			PagePrintOrder = (XlPagePrintOrder)(bitwiseField & 0x01);
			if (Convert.ToBoolean(bitwiseField & 0x02))
				PageOrientation = XlPageOrientation.Portrait;
			else
				PageOrientation = XlPageOrientation.Landscape;
			BlackAndWhite = Convert.ToBoolean(bitwiseField & 0x08);
			Draft = Convert.ToBoolean(bitwiseField & 0x10);
			bool printCellComments = Convert.ToBoolean(bitwiseField & 0x20);
			if (Convert.ToBoolean(bitwiseField & 0x40))
				PageOrientation = XlPageOrientation.Default;
			UseFirstPageNumber = Convert.ToBoolean(bitwiseField & 0x80);
			CommentsPrintMode = (XlCommentsPrintMode)((bitwiseField & 0x0200) >> 9);
			ErrorsPrintMode = (XlErrorsPrintMode)((bitwiseField & 0x0c00) >> 10);
			HorizontalDpi = reader.ReadInt16();
			VerticalDpi = reader.ReadInt16();
			double margin = reader.ReadDouble();
			if (margin < XlsDefs.MinMarginInInches || margin >= XlsDefs.MaxMarginInInches)
				margin = XlsDefs.DefaultHeaderFooterMargin;
			HeaderMargin = margin;
			margin = reader.ReadDouble();
			if (margin < XlsDefs.MinMarginInInches || margin >= XlsDefs.MaxMarginInInches)
				margin = XlsDefs.DefaultHeaderFooterMargin;
			FooterMargin = margin;
			if (!printCellComments)
				CommentsPrintMode = XlCommentsPrintMode.None;
			Copies = reader.ReadInt16();
			if (NoPls) {
				PaperKind = PaperKind.Letter;
				Scale = 100;
				HorizontalDpi = 600;
				VerticalDpi = 600;
				Copies = 1;
				PageOrientation = XlPageOrientation.Default;
			}
			if (HorizontalDpi < 1)
				HorizontalDpi = 600;
			if (VerticalDpi == 0)
				VerticalDpi = HorizontalDpi;
			if (VerticalDpi < 1)
				VerticalDpi = 600;
			if (Copies < 1)
				Copies = 1;
			if (!UseFirstPageNumber)
				FirstPageNumber = 1;
			if (FirstPageNumber < 1)
				FirstPageNumber = 1;
		}
		public override void Write(BinaryWriter writer) {
			writer.Write((short)PaperKind);
			writer.Write((short)Scale);
			writer.Write((short)FirstPageNumber);
			writer.Write((short)FitToWidth);
			writer.Write((short)FitToHeight);
			int bitwiseField = (short)PagePrintOrder;
			if (PageOrientation == XlPageOrientation.Portrait)
				bitwiseField |= 0x02;
			if (BlackAndWhite)
				bitwiseField |= 0x08;
			if (Draft)
				bitwiseField |= 0x10;
			if (CommentsPrintMode != XlCommentsPrintMode.None)
				bitwiseField |= 0x20;
			if (PageOrientation == XlPageOrientation.Default)
				bitwiseField |= 0x40;
			if (UseFirstPageNumber)
				bitwiseField |= 0x80;
			bitwiseField |= ((short)CommentsPrintMode << 9);
			bitwiseField |= ((short)ErrorsPrintMode << 10);
			writer.Write((short)bitwiseField);
			writer.Write((short)HorizontalDpi);
			writer.Write((short)VerticalDpi);
			writer.Write((double)HeaderMargin);
			writer.Write((double)FooterMargin);
			writer.Write((short)Copies);
		}
		public override int GetSize() {
			return 34;
		}
		PaperKind ConvertToPaperKind(int value) {
			if (value < 0 || value > (int)PaperKind.PrcEnvelopeNumber10Rotated)
				return PaperKind.Custom;
			else
				return (PaperKind)value;
		}
	}
	#endregion
	#region XlsContentHeaderFooter
	public class XlsContentHeaderFooter : XlsContentBase {
		#region Fields
		const int fixedPartSize = 38;
		FutureRecordHeader recordHeader = new FutureRecordHeader();
		Guid viewId = Guid.Empty;
		XLUnicodeString evenHeader = new XLUnicodeString();
		XLUnicodeString evenFooter = new XLUnicodeString();
		XLUnicodeString firstHeader = new XLUnicodeString();
		XLUnicodeString firstFooter = new XLUnicodeString();
		#endregion
		#region Properties
		public Guid ViewId {
			get { return viewId; }
			set {
				viewId = value;
			}
		}
		public bool AlignWithMargins { get; set; }
		public bool DifferentFirst { get; set; }
		public bool DifferentOddEven { get; set; }
		public bool ScaleWithDoc { get; set; }
		public string EvenHeader { get { return evenHeader.Value; } set { evenHeader.Value = value; } }
		public string EvenFooter { get { return evenFooter.Value; } set { evenFooter.Value = value; } }
		public string FirstHeader { get { return firstHeader.Value; } set { firstHeader.Value = value; } }
		public string FirstFooter { get { return firstFooter.Value; } set { firstFooter.Value = value; } }
		public override FutureRecordHeaderBase RecordHeader { get { return recordHeader; } }
		#endregion
		public override void Read(XlsReader reader, int size) {
			recordHeader = FutureRecordHeader.FromStream(reader);
			byte[] guidBytes = reader.ReadBytes(16);
			viewId = new Guid(guidBytes);
			ushort bitwiseField = reader.ReadUInt16();
			DifferentOddEven = Convert.ToBoolean(bitwiseField & 0x0001);
			DifferentFirst = Convert.ToBoolean(bitwiseField & 0x0002);
			ScaleWithDoc = Convert.ToBoolean(bitwiseField & 0x0004);
			AlignWithMargins = Convert.ToBoolean(bitwiseField & 0x0008);
			int cchHeaderEven = reader.ReadUInt16();
			int cchFooterEven = reader.ReadUInt16();
			int cchHeaderFirst = reader.ReadUInt16();
			int cchFooterFirst = reader.ReadUInt16();
			if (cchHeaderEven > 0)
				this.evenHeader = XLUnicodeString.FromStream(reader);
			if (cchFooterEven > 0)
				this.evenFooter = XLUnicodeString.FromStream(reader);
			if (cchHeaderFirst > 0)
				this.firstHeader = XLUnicodeString.FromStream(reader);
			if (cchFooterFirst > 0)
				this.firstFooter = XLUnicodeString.FromStream(reader);
			int bytesToRead = size - GetSize();
			if (bytesToRead > 0)
				reader.ReadBytes(bytesToRead);
		}
		public override void Write(BinaryWriter writer) {
			recordHeader.Write(writer);
			writer.Write(viewId.ToByteArray());
			ushort bitwiseField = 0;
			if (DifferentOddEven)
				bitwiseField |= 0x0001;
			if (DifferentFirst)
				bitwiseField |= 0x0002;
			if (ScaleWithDoc)
				bitwiseField |= 0x0004;
			if (AlignWithMargins)
				bitwiseField |= 0x0008;
			writer.Write(bitwiseField);
			int cchHeaderEven = evenHeader.Value.Length;
			int cchFooterEven = evenFooter.Value.Length;
			int cchHeaderFirst = firstHeader.Value.Length;
			int cchFooterFirst = firstFooter.Value.Length;
			writer.Write((ushort)cchHeaderEven);
			writer.Write((ushort)cchFooterEven);
			writer.Write((ushort)cchHeaderFirst);
			writer.Write((ushort)cchFooterFirst);
			if (cchHeaderEven > 0)
				this.evenHeader.Write(writer);
			if (cchFooterEven > 0)
				this.evenFooter.Write(writer);
			if (cchHeaderFirst > 0)
				this.firstHeader.Write(writer);
			if (cchFooterFirst > 0)
				this.firstFooter.Write(writer);
		}
		public override int GetSize() {
			int varPartSize = 0;
			if (evenHeader.Value.Length > 0)
				varPartSize += evenHeader.Length;
			if (evenFooter.Value.Length > 0)
				varPartSize += evenFooter.Length;
			if (firstHeader.Value.Length > 0)
				varPartSize += firstHeader.Length;
			if (firstFooter.Value.Length > 0)
				varPartSize += firstFooter.Length;
			return fixedPartSize + varPartSize;
		}
	}
	#endregion
	#region XlsContentPageBreaksBase (abstract)
	public abstract class XlsContentPageBreaksBase : XlsContentBase {
		readonly IXlPageBreaks breaks;
		protected XlsContentPageBreaksBase(IXlPageBreaks breaks) {
			this.breaks = breaks;
		}
		protected abstract int MaxCount { get; }
		protected abstract int EndValue { get; }
		public override void Read(XlsReader reader, int size) {
		}
		public override void Write(BinaryWriter writer) {
			int count = breaks.Count;
			if(count > MaxCount)
				count = MaxCount;
			writer.Write((ushort)count);
			for(int i = 0; i < count; i++) {
				writer.Write((ushort)breaks[i]);
				writer.Write((ushort)0);
				writer.Write((ushort)EndValue);
			}
		}
		public override int GetSize() {
			return breaks.Count * 6 + 2;
		}
	}
	#endregion
	#region XlsContentColumnPageBreaks
	public class XlsContentColumnPageBreaks : XlsContentPageBreaksBase {
		public XlsContentColumnPageBreaks(IXlPageBreaks breaks) 
			:base(breaks) {
		}
		protected override int MaxCount {
			get { return 255; }
		}
		protected override int EndValue {
			get { return 0xffff; }
		}
	}
	#endregion
	#region XlsContentRowPageBreaks
	public class XlsContentRowPageBreaks : XlsContentPageBreaksBase {
		public XlsContentRowPageBreaks(IXlPageBreaks breaks)
			: base(breaks) {
		}
		protected override int MaxCount {
			get { return 1026; }
		}
		protected override int EndValue {
			get { return 0xff; }
		}
	}
	#endregion
	#region XlsContentColumnInfo
	public class XlsContentColumnInfo : XlsContentBase {
		#region Fields
		int firstColumn;
		int lastColumn;
		int columnWidth;
		int outlineLevel;
		#endregion
		#region Properties
		public int FirstColumn {
			get { return firstColumn; }
			set {
				CheckValue(value, 0, XlsDefs.FullRangeColumnIndex, "FirstColumn");
				firstColumn = value;
			}
		}
		public int LastColumn {
			get { return lastColumn; }
			set {
				CheckValue(value, 0, XlsDefs.FullRangeColumnIndex, "LastColumn");
				lastColumn = value;
			}
		}
		public int ColumnWidth {
			get { return columnWidth; }
			set {
				CheckValue(value, 0, ushort.MaxValue, "ColumnWidth");
				columnWidth = value;
			}
		}
		public int FormatIndex { get; set; }
		public bool Hidden { get; set; }
		public bool CustomWidth { get; set; }
		public bool BestFit { get; set; }
		public bool ShowPhoneticInfo { get; set; }
		public int OutlineLevel {
			get { return outlineLevel; }
			set {
				CheckValue(value, 0, XlsDefs.MaxOutlineLevel, "OutlineLevel");
				outlineLevel = value;
			}
		}
		public bool Collapsed { get; set; }
		#endregion
		public override void Read(XlsReader reader, int size) {
			FirstColumn = reader.ReadUInt16();
			LastColumn = reader.ReadUInt16();
			ColumnWidth = reader.ReadUInt16();
			FormatIndex = reader.ReadUInt16();
			ushort bitwiseField = reader.ReadUInt16();
			Hidden = Convert.ToBoolean(bitwiseField & 0x0001);
			CustomWidth = Convert.ToBoolean(bitwiseField & 0x0002);
			BestFit = Convert.ToBoolean(bitwiseField & 0x0004);
			ShowPhoneticInfo = Convert.ToBoolean(bitwiseField & 0x0008);
			OutlineLevel = (bitwiseField & 0x0700) >> 8;
			Collapsed = Convert.ToBoolean(bitwiseField & 0x1000);
			int bytesToSkip = size - 10;
			if (bytesToSkip > 0)
				reader.Seek(bytesToSkip, SeekOrigin.Current);
		}
		public override void Write(BinaryWriter writer) {
			writer.Write((ushort)FirstColumn);
			writer.Write((ushort)LastColumn);
			writer.Write((ushort)ColumnWidth);
			writer.Write((ushort)FormatIndex);
			ushort bitwiseField = (ushort)((OutlineLevel & 0x07) << 8);
			if (Hidden)
				bitwiseField |= 0x0001;
			if (CustomWidth)
				bitwiseField |= 0x0002;
			if (BestFit)
				bitwiseField |= 0x0004;
			if (ShowPhoneticInfo)
				bitwiseField |= 0x0008;
			if (Collapsed)
				bitwiseField |= 0x1000;
			writer.Write(bitwiseField);
			writer.Write((ushort)0); 
		}
		public override int GetSize() {
			return 12;
		}
	}
	#endregion
	#region XlsContentDimensions
	public class XlsContentDimensions : XlsContentBase {
		#region Properties
		public int FirstRowIndex { get; set; }
		public int LastRowIndex { get; set; }
		public int FirstColumnIndex { get; set; }
		public int LastColumnIndex { get; set; }
		#endregion
		public override void Read(XlsReader reader, int size) {
			FirstRowIndex = reader.ReadInt32() + 1;
			LastRowIndex = reader.ReadInt32();
			FirstColumnIndex = reader.ReadInt16() + 1;
			LastColumnIndex = reader.ReadInt16();
			int bytesToRead = size - 12;
			if (bytesToRead > 0)
				reader.ReadBytes(bytesToRead);
		}
		public override void Write(BinaryWriter writer) {
			writer.Write(FirstRowIndex - 1);
			writer.Write(LastRowIndex);
			writer.Write((short)(FirstColumnIndex - 1));
			writer.Write((short)LastColumnIndex);
			writer.Write((short)0);
		}
		public override int GetSize() {
			return 14;
		}
	}
	#endregion
	#region XlsContentWorksheetWindow
	public class XlsContentWorksheetWindow : XlsContentBase {
		#region Fields
		int topRowIndex;
		int leftColumnIndex;
		int gridlinesColorIndex;
		int zoomScalePageBreakPreview;
		int zoomScaleNormalView;
		#endregion
		#region Properties
		public bool ShowFormulas { get; set; }
		public bool ShowGridlines { get; set; }
		public bool ShowRowColumnHeadings { get; set; }
		public bool Frozen { get; set; }
		public bool ShowZeroValues { get; set; }
		public bool GridlinesInDefaultColor { get { return gridlinesColorIndex == XlsPalette.DefaultForegroundColorIndex; } }
		public bool RightToLeft { get; set; }
		public bool ShowOutlineSymbols { get; set; }
		public bool FrozenWithoutPaneSplit { get; set; }
		public bool SheetTabIsSelected { get; set; }
		public bool CurrentlyDisplayed { get; set; }
		public bool InPageBreakPreview { get; set; }
		public int TopRowIndex {
			get { return topRowIndex; }
			set {
				CheckValue(value, 0, ushort.MaxValue, "TopRowIndex");
				topRowIndex = value;
			}
		}
		public int LeftColumnIndex {
			get { return leftColumnIndex; }
			set {
				CheckValue(value, 0, byte.MaxValue, "LeftColumnIndex");
				leftColumnIndex = value;
			}
		}
		public int GridlinesColorIndex {
			get { return gridlinesColorIndex; }
			set {
				CheckValue(value, 0, XlsPalette.DefaultForegroundColorIndex, "GridlinesColorIndex");
				gridlinesColorIndex = value;
			}
		}
		public int ZoomScalePageBreakPreview {
			get { return zoomScalePageBreakPreview; }
			set {
				CheckValue(value, 0, ushort.MaxValue, "ZoomScalePageBreakPreview");
				zoomScalePageBreakPreview = value;
			}
		}
		public int ZoomScaleNormalView {
			get { return zoomScaleNormalView; }
			set {
				CheckValue(value, 0, ushort.MaxValue, "ZoomScaleNormalView");
				zoomScaleNormalView = value;
			}
		}
		#endregion
		public override void Read(XlsReader reader, int size) {
			ushort bitwiseField = reader.ReadUInt16();
			ShowFormulas = Convert.ToBoolean(bitwiseField & 0x0001);
			ShowGridlines = Convert.ToBoolean(bitwiseField & 0x0002);
			ShowRowColumnHeadings = Convert.ToBoolean(bitwiseField & 0x0004);
			Frozen = Convert.ToBoolean(bitwiseField & 0x0008);
			ShowZeroValues = Convert.ToBoolean(bitwiseField & 0x0010);
			RightToLeft = Convert.ToBoolean(bitwiseField & 0x0040);
			ShowOutlineSymbols = Convert.ToBoolean(bitwiseField & 0x0080);
			FrozenWithoutPaneSplit = Convert.ToBoolean(bitwiseField & 0x0100);
			SheetTabIsSelected = Convert.ToBoolean(bitwiseField & 0x0200);
			CurrentlyDisplayed = Convert.ToBoolean(bitwiseField & 0x0400);
			InPageBreakPreview = Convert.ToBoolean(bitwiseField & 0x0800);
			TopRowIndex = reader.ReadUInt16();
			LeftColumnIndex = reader.ReadUInt16();
			GridlinesColorIndex = reader.ReadUInt16();
			if ((bitwiseField & 0x0020) != 0)
				GridlinesColorIndex = XlsPalette.DefaultForegroundColorIndex;
			reader.ReadUInt16(); 
			if (size == 10)
				return;
			this.zoomScalePageBreakPreview = reader.ReadUInt16();
			this.zoomScaleNormalView = reader.ReadUInt16();
			reader.ReadUInt16(); 
			reader.ReadUInt16(); 
		}
		public override void Write(BinaryWriter writer) {
			ushort bitwiseField = 0;
			if (ShowFormulas)
				bitwiseField |= 0x0001;
			if (ShowGridlines)
				bitwiseField |= 0x0002;
			if (ShowRowColumnHeadings)
				bitwiseField |= 0x0004;
			if (Frozen)
				bitwiseField |= 0x0008;
			if (ShowZeroValues)
				bitwiseField |= 0x0010;
			if (GridlinesInDefaultColor)
				bitwiseField |= 0x0020;
			if (RightToLeft)
				bitwiseField |= 0x0040;
			if (ShowOutlineSymbols)
				bitwiseField |= 0x0080;
			if (FrozenWithoutPaneSplit)
				bitwiseField |= 0x0100;
			if (SheetTabIsSelected)
				bitwiseField |= 0x0200;
			if (CurrentlyDisplayed)
				bitwiseField |= 0x0400;
			if (InPageBreakPreview)
				bitwiseField |= 0x0800;
			writer.Write(bitwiseField);
			writer.Write((ushort)TopRowIndex);
			writer.Write((ushort)LeftColumnIndex);
			writer.Write((ushort)GridlinesColorIndex);
			writer.Write((ushort)0);
			writer.Write((ushort)ZoomScalePageBreakPreview);
			writer.Write((ushort)ZoomScaleNormalView);
			writer.Write((ushort)0);
			writer.Write((ushort)0);
		}
		public override int GetSize() {
			return 18;
		}
	}
	#endregion
	#region XlsContentPane
	public class XlsContentPane : XlsContentBase {
		#region Properties
		public int XPos { get; set; }
		public int YPos { get; set; }
		public int TopRow { get; set; }
		public int LeftColumn { get; set; }
		public byte ActivePane { get; set; }
		#endregion
		public override void Read(XlsReader reader, int size) {
			XPos = reader.ReadUInt16();
			YPos = reader.ReadUInt16();
			TopRow = reader.ReadUInt16();
			LeftColumn = reader.ReadUInt16() & 0x0ff;
			ActivePane = reader.ReadByte();
			reader.ReadByte(); 
		}
		public override void Write(BinaryWriter writer) {
			writer.Write((ushort)XPos);
			writer.Write((ushort)YPos);
			writer.Write((ushort)TopRow);
			writer.Write((ushort)LeftColumn);
			writer.Write(ActivePane);
			writer.Write((byte)0); 
		}
		public override int GetSize() {
			return 10;
		}
	}
	#endregion
	#region XlsContentRow
	public class XlsContentRow : XlsContentBase {
		#region Fields
		const int minHeightInTwips = 0; 
		const int maxHeightInTwips = 8192; 
		int firstColumnIndex;
		int lastColumnIndex;
		int heightInTwips = 0xff;
		#endregion
		#region Properties
		public int HeightInTwips {
			get { return heightInTwips; }
			set {
				heightInTwips = value;
				if (heightInTwips < minHeightInTwips)
					heightInTwips = minHeightInTwips;
				if (heightInTwips > maxHeightInTwips)
					heightInTwips = maxHeightInTwips;
			}
		}
		public int Index { get; set; }
		public int FirstColumnIndex {
			get { return firstColumnIndex; }
			set {
				CheckValue(value, 0, 255, "FirstColumnIndex");
				firstColumnIndex = value;
			}
		}
		public int LastColumnIndex {
			get { return lastColumnIndex; }
			set {
				CheckValue(value, 0, 256, "LastColumnIndex");
				lastColumnIndex = value;
			}
		}
		public int OutlineLevel { get; set; }
		public bool IsCollapsed { get; set; }
		public bool IsHidden { get; set; }
		public bool IsCustomHeight { get; set; }
		public bool HasFormatting { get; set; }
		public int FormatIndex { get; set; }
		public bool HasThickBorder { get; set; }
		public bool HasMediumBorder { get; set; }
		public bool HasPhoneticGuide { get; set; }
		#endregion
		public override void Read(XlsReader reader, int size) {
			Index = reader.ReadUInt16();
			int firstColIndex = reader.ReadUInt16();
			int lastColIndex = reader.ReadUInt16();
			if (firstColIndex != lastColIndex) {
				FirstColumnIndex = firstColIndex;
				LastColumnIndex = lastColIndex;
			}
			this.heightInTwips = reader.ReadUInt16() & 0x7fff; 
			if (this.heightInTwips > maxHeightInTwips)
				this.heightInTwips = maxHeightInTwips;
			reader.Seek(4, SeekOrigin.Current);
			byte bitwiseFiels = reader.ReadByte();
			OutlineLevel = bitwiseFiels & 0x07;
			IsCollapsed = Convert.ToBoolean(bitwiseFiels & 0x10);
			IsHidden = Convert.ToBoolean(bitwiseFiels & 0x20);
			IsCustomHeight = Convert.ToBoolean(bitwiseFiels & 0x40);
			HasFormatting = Convert.ToBoolean(bitwiseFiels & 0x80);
			reader.ReadByte();
			ushort bitwiseField2 = reader.ReadUInt16();
			FormatIndex = bitwiseField2 & 0x0fff;
			HasThickBorder = Convert.ToBoolean(bitwiseField2 & 0x1000);
			HasMediumBorder = Convert.ToBoolean(bitwiseField2 & 0x2000);
			HasPhoneticGuide = Convert.ToBoolean(bitwiseField2 & 0x4000);
		}
		public override void Write(BinaryWriter writer) {
			writer.Write((ushort)(Index));
			writer.Write((ushort)(FirstColumnIndex));
			writer.Write((ushort)(LastColumnIndex));
			writer.Write((ushort)HeightInTwips);
			writer.BaseStream.Seek(4, SeekOrigin.Current);
			byte bitwiseField = (byte)OutlineLevel;
			if (IsCollapsed)
				bitwiseField |= 0x10;
			if (IsHidden)
				bitwiseField |= 0x20;
			if (IsCustomHeight)
				bitwiseField |= 0x40;
			if (HasFormatting)
				bitwiseField |= 0x80;
			writer.Write(bitwiseField);
			writer.Write((byte)1);
			ushort bitwiseField2 = (ushort)(FormatIndex & 0x0fff);
			if (HasThickBorder)
				bitwiseField2 |= 0x1000;
			if (HasMediumBorder)
				bitwiseField2 |= 0x2000;
			if (HasPhoneticGuide)
				bitwiseField2 |= 0x4000;
			writer.Write(bitwiseField2);
		}
		public override int GetSize() {
			return 16;
		}
	}
	#endregion
	#region XlsContentDbCell
	public class XlsContentDbCell : XlsContentBase {
		const int fixedPartSize = 4;
		const int variablePartItemSize = 2;
		readonly List<int> streamOffsets = new List<int>();
		public long FirstRowOffset { get; set; }
		public List<int> StreamOffsets { get { return streamOffsets; } }
		public override void Read(XlsReader reader, int size) {
			streamOffsets.Clear();
			FirstRowOffset = reader.ReadUInt32();
			int count = (size - fixedPartSize) / variablePartItemSize;
			for (int i = 0; i < count; i++)
				streamOffsets.Add(reader.ReadUInt16());
		}
		public override void Write(BinaryWriter writer) {
			writer.Write((uint)FirstRowOffset);
			int count = streamOffsets.Count;
			for (int i = 0; i < count; i++)
				writer.Write((ushort)streamOffsets[i]);
		}
		public override int GetSize() {
			return fixedPartSize + variablePartItemSize * streamOffsets.Count;
		}
	}
	#endregion
	#region XlsContentCellBase (abstract class)
	public abstract class XlsContentCellBase : XlsContentBase {
		#region Properties
		public int RowIndex { get; set; }
		public int ColumnIndex { get; set; }
		public int FormatIndex { get; set; }
		#endregion
		public override void Read(XlsReader reader, int size) {
			RowIndex = reader.ReadUInt16();
			ColumnIndex = reader.ReadUInt16();
			FormatIndex = reader.ReadUInt16();
		}
		public override void Write(BinaryWriter writer) {
			writer.Write((ushort)RowIndex);
			writer.Write((ushort)ColumnIndex);
			writer.Write((ushort)FormatIndex);
		}
		public override int GetSize() {
			return 6;
		}
	}
	#endregion
	#region XlsContentBlank
	public class XlsContentBlank : XlsContentCellBase {
	}
	#endregion
	#region XlsContentMulBlank
	public class XlsContentMulBlank : XlsContentBase {
		const short fixedPartSize = 6;
		const short varPartItemSize = 2;
		readonly List<int> formatIndices = new List<int>();
		#region Properties
		public int RowIndex { get; set; }
		public int FirstColumnIndex { get; set; }
		public int LastColumnIndex { get { return FirstColumnIndex + formatIndices.Count - 1; } }
		public IList<int> FormatIndices { get { return formatIndices; } }
		#endregion
		public override void Read(XlsReader reader, int size) {
			RowIndex = reader.ReadUInt16();
			FirstColumnIndex = reader.ReadUInt16();
			formatIndices.Clear();
			int count = (size - fixedPartSize) / varPartItemSize;
			for (int i = 0; i < count; i++)
				formatIndices.Add(reader.ReadUInt16());
			reader.ReadUInt16(); 
		}
		public override void Write(BinaryWriter writer) {
			writer.Write((ushort)RowIndex);
			writer.Write((ushort)FirstColumnIndex);
			int count = formatIndices.Count;
			for (int i = 0; i < count; i++)
				writer.Write((ushort)formatIndices[i]);
			writer.Write((ushort)LastColumnIndex);
		}
		public override int GetSize() {
			return fixedPartSize + formatIndices.Count * varPartItemSize;
		}
	}
	#endregion
	#region XlsContentBoolErr
	public class XlsContentBoolErr : XlsContentCellBase {
		#region Properties
		public byte Value { get; set; }
		public bool IsError { get; set; }
		#endregion
		public override void Read(XlsReader reader, int size) {
			base.Read(reader, size);
			Value = reader.ReadByte();
			IsError = reader.ReadBoolean();
			int bytesToRead = size - GetSize();
			if (bytesToRead > 0)
				reader.ReadBytes(bytesToRead);
		}
		public override void Write(BinaryWriter writer) {
			base.Write(writer);
			writer.Write(Value);
			writer.Write(IsError);
		}
		public override int GetSize() {
			return base.GetSize() + 2;
		}
	}
	#endregion
	#region XlsContentNumber
	public class XlsContentNumber : XlsContentCellBase {
		public double Value { get; set; }
		public override void Read(XlsReader reader, int size) {
			base.Read(reader, size);
			Value = reader.ReadDouble();
			int bytesToRead = size - GetSize();
			if (bytesToRead > 0)
				reader.ReadBytes(bytesToRead);
		}
		public override void Write(BinaryWriter writer) {
			base.Write(writer);
			writer.Write(Value);
		}
		public override int GetSize() {
			return base.GetSize() + 8;
		}
	}
	#endregion
	#region XlsContentLabel
	public class XlsContentLabel : XlsContentCellBase {
		XLUnicodeString stringValue = new XLUnicodeString();
		public string Value {
			get { return stringValue.Value; }
			set {
				if (!string.IsNullOrEmpty(value) && value.Length > 255)
					throw new ArgumentException("String value length exceed 255 characters");
				stringValue.Value = value;
			}
		}
		public override void Read(XlsReader reader, int size) {
			base.Read(reader, size);
			stringValue = XLUnicodeString.FromStream(reader);
			int bytesToRead = size - GetSize();
			if (bytesToRead > 0)
				reader.ReadBytes(bytesToRead);
		}
		public override void Write(BinaryWriter writer) {
			base.Write(writer);
			stringValue.Write(writer);
		}
		public override int GetSize() {
			return base.GetSize() + stringValue.Length;
		}
	}
	#endregion
	#region XlsContentLabelSst
	public class XlsContentLabelSst : XlsContentCellBase {
		public int StringIndex { get; set; }
		public override void Read(XlsReader reader, int size) {
			base.Read(reader, size);
			StringIndex = reader.ReadInt32();
		}
		public override void Write(BinaryWriter writer) {
			base.Write(writer);
			writer.Write(StringIndex);
		}
		public override int GetSize() {
			return base.GetSize() + 4;
		}
	}
	#endregion
	#region XlsContentRk
	public class XlsContentRk : XlsContentCellBase {
		public double Value { get; set; }
		public override void Read(XlsReader reader, int size) {
			RowIndex = reader.ReadUInt16();
			ColumnIndex = reader.ReadUInt16();
			XlsRkRec rec = XlsRkRec.Read(reader);
			FormatIndex = rec.FormatIndex;
			Value = rec.Rk.Value;
		}
		public override void Write(BinaryWriter writer) {
			writer.Write((ushort)RowIndex);
			writer.Write((ushort)ColumnIndex);
			XlsRkRec rec = new XlsRkRec();
			rec.FormatIndex = FormatIndex;
			rec.Rk.Value = Value;
			rec.Write(writer);
		}
		public override int GetSize() {
			return 10;
		}
	}
	#endregion
	#region XlsContentMulRk
	public class XlsContentMulRk : XlsContentBase {
		#region Fields
		const int fixedPartSize = 6;
		const int variablePartElementSize = 6;
		readonly List<XlsRkRec> rkRecords = new List<XlsRkRec>();
		#endregion
		#region Properties
		public int RowIndex { get; set; }
		public int FirstColumnIndex { get; set; }
		public int LastColumnIndex { get { return FirstColumnIndex + RkRecords.Count - 1; } }
		public List<XlsRkRec> RkRecords { get { return rkRecords; } }
		#endregion
		public override void Read(XlsReader reader, int size) {
			RowIndex = reader.ReadUInt16();
			FirstColumnIndex = reader.ReadUInt16();
			RkRecords.Clear();
			int count = (size - fixedPartSize) / variablePartElementSize;
			for (int i = 0; i < count; i++)
				RkRecords.Add(XlsRkRec.Read(reader));
			reader.ReadUInt16(); 
		}
		public override void Write(BinaryWriter writer) {
			writer.Write((ushort)RowIndex);
			writer.Write((ushort)FirstColumnIndex);
			int count = RkRecords.Count;
			for (int i = 0; i < count; i++)
				RkRecords[i].Write(writer);
			writer.Write((ushort)LastColumnIndex);
		}
		public override int GetSize() {
			return fixedPartSize + RkRecords.Count * variablePartElementSize;
		}
	}
	#endregion
	#region XlsContentFormula
	public class XlsContentFormula : XlsContentCellBase {
		#region Fields
		const short formulaValueSize = 8;
		const short flagsSize = 2;
		const short chnSize = 4;
		XlsFormulaValue formulaValue = new XlsFormulaValue();
		byte[] formulaBytes = new byte[] { 0x00, 0x00 };
		#endregion
		#region Properties
		public XlsFormulaValue Value { get { return formulaValue; } }
		public bool AlwaysCalc { get; set; }
		public bool HasFillAlignment { get; set; }
		public bool PartOfSharedFormula { get; set; }
		public bool ClearErrors { get; set; }
		public byte[] FormulaBytes {
			get { return formulaBytes; }
			set {
				if (value == null || value.Length == 0) {
					this.formulaBytes = new byte[] { 0x00, 0x00 };
				}
				else {
					if (value.Length < 2)
						throw new ArgumentException("value must be at least 2 bytes long");
					this.formulaBytes = value;
				}
			}
		}
		#endregion
		public override void Read(XlsReader reader, int size) {
			base.Read(reader, size);
			this.formulaValue.Read(reader);
			ushort bitwiseField = reader.ReadUInt16();
			AlwaysCalc = (bitwiseField & 0x01) != 0;
			HasFillAlignment = (bitwiseField & 0x04) != 0;
			PartOfSharedFormula = (bitwiseField & 0x08) != 0;
			ClearErrors = (bitwiseField & 0x20) != 0;
			reader.ReadInt32(); 
			int formulaBytesCount = size - GetFixedPartSize();
			formulaBytes = reader.ReadBytes(formulaBytesCount);
		}
		public override void Write(BinaryWriter writer) {
			base.Write(writer);
			this.formulaValue.Write(writer);
			ushort bitwiseField = 0;
			if (AlwaysCalc)
				bitwiseField |= 0x01;
			if (HasFillAlignment)
				bitwiseField |= 0x04;
			if (PartOfSharedFormula)
				bitwiseField |= 0x08;
			if (ClearErrors)
				bitwiseField |= 0x20;
			writer.Write(bitwiseField);
			writer.Write((Int32)0); 
			writer.Write(formulaBytes);
		}
		public override int GetSize() {
			return GetFixedPartSize() + formulaBytes.Length;
		}
		int GetFixedPartSize() {
			return base.GetSize() + formulaValueSize + flagsSize + chnSize;
		}
	}
	#endregion
	#region XlsContentSharedFormula
	public class XlsContentSharedFormula : XlsContentBase {
		#region Fields
		const int fixedPartSize = 8;
		XlsRefU range = new XlsRefU();
		byte[] formulaBytes = new byte[] { 0x00, 0x00 };
		#endregion
		#region Properties
		public XlsRefU Range {
			get { return range; }
			set {
				if (value == null)
					value = new XlsRefU();
				range = value;
			}
		}
		public byte UseCount { get; set; }
		public byte[] FormulaBytes {
			get { return formulaBytes; }
			set {
				if (value == null || value.Length == 0) {
					this.formulaBytes = new byte[] { 0x00, 0x00 };
				}
				else {
					if (value.Length < 2)
						throw new ArgumentException("value must be at least 2 bytes long");
					this.formulaBytes = value;
				}
			}
		}
		#endregion
		public override void Read(XlsReader reader, int size) {
			this.range = XlsRefU.FromStream(reader);
			reader.ReadByte(); 
			UseCount = reader.ReadByte();
			int formulaBytesCount = size - fixedPartSize;
			formulaBytes = reader.ReadBytes(formulaBytesCount);
		}
		public override void Write(BinaryWriter writer) {
			this.range.Write(writer);
			writer.Write((byte)0); 
			writer.Write((byte)UseCount);
			writer.Write(formulaBytes);
		}
		public override int GetSize() {
			return fixedPartSize + formulaBytes.Length;
		}
	}
	#endregion
	#region XlsContentMergeCells
	public class XlsContentMergeCells : XlsContentBase {
		readonly List<XlsRef8> mergeCells = new List<XlsRef8>();
		public IList<XlsRef8> MergeCells { get { return mergeCells; } }
		public override void Read(XlsReader reader, int size) {
			mergeCells.Clear();
			int count = reader.ReadUInt16();
			for (int i = 0; i < count; i++)
				mergeCells.Add(XlsRef8.FromStream(reader));
		}
		public override void Write(BinaryWriter writer) {
			int count = Math.Min(mergeCells.Count, XlsDefs.MaxMergeCellCount);
			writer.Write((ushort)count);
			for (int i = 0; i < count; i++)
				mergeCells[i].Write(writer);
		}
		public override int GetSize() {
			int count = Math.Min(mergeCells.Count, XlsDefs.MaxMergeCellCount);
			return count * 8 + 2;
		}
	}
	#endregion
	#region XlsContentDVal
	public class XlsContentDVal : XlsContentBase {
		#region Fields
		int xLeft;
		int yTop;
		int objId = -1;
		int recordCount;
		#endregion
		#region Properties
		public bool InputWindowClosed { get; set; }
		public int XLeft {
			get { return xLeft; }
			set {
				CheckValue(value, 0, ushort.MaxValue, "XLeft");
				xLeft = value;
			}
		}
		public int YTop {
			get { return yTop; }
			set {
				CheckValue(value, 0, ushort.MaxValue, "YTop");
				yTop = value;
			}
		}
		public int ObjId {
			get { return objId; }
			set {
				if (value != -1)
					CheckValue(value, 1, short.MaxValue, "ObjId");
				objId = value;
			}
		}
		public int RecordCount {
			get { return recordCount; }
			set {
				CheckValue(value, 0, XlsDefs.MaxDataValidationRecordCount, "RecordCount");
				recordCount = value;
			}
		}
		#endregion
		public override void Read(XlsReader reader, int size) {
			ushort bitwiseField = reader.ReadUInt16();
			InputWindowClosed = (bitwiseField & 0x0001) != 0;
			xLeft = reader.ReadInt32();
			yTop = reader.ReadInt32();
			objId = reader.ReadInt32();
			recordCount = reader.ReadInt32();
		}
		public override void Write(BinaryWriter writer) {
			ushort bitwiseField = 0;
			if (InputWindowClosed)
				bitwiseField |= 0x0001;
			writer.Write(bitwiseField);
			writer.Write(xLeft);
			writer.Write(yTop);
			writer.Write(objId);
			writer.Write(recordCount);
		}
		public override int GetSize() {
			return 18;
		}
	}
	#endregion
	#region XlsContentDv
	public class XlsContentDv : XlsContentBase {
		#region Fields
		static readonly char[] nullChar = new char[] { '\0' };
		const string nullString = "\0";
		XLUnicodeString promptTitle = new XLUnicodeString() { Value = nullString };
		XLUnicodeString errorTitle = new XLUnicodeString() { Value = nullString };
		XLUnicodeString prompt = new XLUnicodeString() { Value = nullString };
		XLUnicodeString error = new XLUnicodeString() { Value = nullString };
		byte[] formula1Bytes = new byte[0];
		byte[] formula2Bytes = new byte[0];
		readonly List<XlsRef8> ranges = new List<XlsRef8>();
		#endregion
		#region Properties
		public XlDataValidationType ValidationType { get; set; }
		public XlDataValidationErrorStyle ErrorStyle { get; set; }
		public XlDataValidationImeMode ImeMode { get; set; }
		public bool StringLookup { get; set; }
		public bool AllowBlank { get; set; }
		public bool SuppressCombo { get; set; }
		public bool ShowInputMessage { get; set; }
		public bool ShowErrorMessage { get; set; }
		public XlDataValidationOperator ValidationOperator { get; set; }
		public string PromptTitle {
			get { return promptTitle.Value.TrimEnd(nullChar); }
			set {
				if (string.IsNullOrEmpty(value))
					promptTitle.Value = nullString;
				else {
					CheckLength(value, XlsDefs.MaxDataValidationTitleLength, "PromptTitle");
					promptTitle.Value = value;
				}
			}
		}
		public string ErrorTitle {
			get { return errorTitle.Value.TrimEnd(nullChar); }
			set {
				if (string.IsNullOrEmpty(value))
					errorTitle.Value = nullString;
				else {
					CheckLength(value, XlsDefs.MaxDataValidationTitleLength, "ErrorTitle");
					errorTitle.Value = value;
				}
			}
		}
		public string Prompt {
			get { return prompt.Value.TrimEnd(nullChar); }
			set {
				if (string.IsNullOrEmpty(value))
					prompt.Value = nullString;
				else {
					CheckLength(value, XlsDefs.MaxDataValidationPromptLength, "Prompt");
					prompt.Value = value;
				}
			}
		}
		public string Error {
			get { return error.Value.TrimEnd(nullChar); }
			set {
				if (string.IsNullOrEmpty(value))
					error.Value = nullString;
				else {
					CheckLength(value, XlsDefs.MaxDataValidationErrorLength, "Error");
					error.Value = value;
				}
			}
		}
		public byte[] Formula1Bytes {
			get { return formula1Bytes; }
			set {
				if (value == null || value.Length < 2)
					formula1Bytes = new byte[0];
				else {
					int size = BitConverter.ToUInt16(value, 0);
					this.formula1Bytes = new byte[size];
					Array.Copy(value, 2, this.formula1Bytes, 0, size);
				}
			}
		}
		public byte[] Formula2Bytes {
			get { return formula2Bytes; }
			set {
				if (value == null || value.Length < 2)
					formula2Bytes = new byte[0];
				else {
					int size = BitConverter.ToUInt16(value, 0);
					this.formula2Bytes = new byte[size];
					Array.Copy(value, 2, this.formula2Bytes, 0, size);
				}
			}
		}
		public IList<XlsRef8> Ranges { get { return ranges; } }
		#endregion
		public override void Read(XlsReader reader, int size) {
			uint bitwiseField = reader.ReadUInt32();
			ValidationType = (XlDataValidationType)(bitwiseField & 0x000f);
			ErrorStyle = (XlDataValidationErrorStyle)((bitwiseField & 0x0070) >> 4);
			StringLookup = (bitwiseField & 0x0080) != 0;
			AllowBlank = (bitwiseField & 0x0100) != 0;
			SuppressCombo = (bitwiseField & 0x0200) != 0;
			ImeMode = (XlDataValidationImeMode)((bitwiseField & 0x03fc00) >> 10);
			ShowInputMessage = (bitwiseField & 0x040000) != 0;
			ShowErrorMessage = (bitwiseField & 0x080000) != 0;
			ValidationOperator = (XlDataValidationOperator)((bitwiseField & 0x0f00000) >> 20);
			promptTitle = XLUnicodeString.FromStream(reader);
			errorTitle = XLUnicodeString.FromStream(reader);
			prompt = XLUnicodeString.FromStream(reader);
			error = XLUnicodeString.FromStream(reader);
			int formulaBytesCount = reader.ReadUInt16();
			reader.ReadUInt16(); 
			if (formulaBytesCount > 0)
				formula1Bytes = reader.ReadBytes(formulaBytesCount);
			else
				formula1Bytes = new byte[0];
			formulaBytesCount = reader.ReadUInt16();
			reader.ReadUInt16(); 
			if (formulaBytesCount > 0)
				formula2Bytes = reader.ReadBytes(formulaBytesCount);
			else
				formula2Bytes = new byte[0];
			int count = reader.ReadUInt16();
			for (int i = 0; i < count; i++)
				Ranges.Add(XlsRef8.FromStream(reader));
		}
		public override void Write(BinaryWriter writer) {
			uint bitwiseField = (uint)ValidationType;
			bitwiseField |= ((uint)ErrorStyle) << 4;
			if (StringLookup)
				bitwiseField |= 0x0080;
			if (AllowBlank)
				bitwiseField |= 0x0100;
			if (SuppressCombo)
				bitwiseField |= 0x0200;
			bitwiseField |= ((uint)ImeMode) << 10;
			if (ShowInputMessage)
				bitwiseField |= 0x040000;
			if (ShowErrorMessage)
				bitwiseField |= 0x080000;
			bitwiseField |= ((uint)ValidationOperator) << 20;
			writer.Write(bitwiseField);
			promptTitle.Write(writer);
			errorTitle.Write(writer);
			prompt.Write(writer);
			error.Write(writer);
			int formulaBytesCount = formula1Bytes.Length;
			writer.Write(formulaBytesCount);
			writer.Write(formula1Bytes);
			formulaBytesCount = formula2Bytes.Length;
			writer.Write(formulaBytesCount);
			writer.Write(formula2Bytes);
			int count = Ranges.Count;
			writer.Write((ushort)count);
			for (int i = 0; i < count; i++)
				Ranges[i].Write(writer);
		}
		public override int GetSize() {
			return 14 + promptTitle.Length + errorTitle.Length + prompt.Length + error.Length + formula1Bytes.Length + formula2Bytes.Length + Ranges.Count * 8;
		}
	}
	#endregion
	#region XlsContentHyperlink
	public class XlsContentHyperlink : XlsContentBase {
		#region Fields
		const int fixedPartSize = 24;
		XlsRef8 range = new XlsRef8();
		Guid classId = XlsHyperlinkObject.CLSID_StdHyperlink;
		XlsHyperlinkObject hyperlink = new XlsHyperlinkObject();
		#endregion
		#region Properties
		public XlsRef8 Range {
			get { return range; }
			set {
				if (value == null)
					value = new XlsRef8();
				range = value;
			}
		}
		public Guid ClassId {
			get { return classId; }
			set {
				Guard.ArgumentNotNull(value, "ClassId");
				classId = value;
			}
		}
		public bool HasMoniker { get { return hyperlink.HasMoniker; } set { hyperlink.HasMoniker = value; } }
		public bool IsAbsolute { get { return hyperlink.IsAbsolute; } set { hyperlink.IsAbsolute = value; } }
		public bool SiteGaveDisplayName { get { return hyperlink.SiteGaveDisplayName; } set { hyperlink.SiteGaveDisplayName = value; } }
		public bool HasLocationString { get { return hyperlink.HasLocationString; } set { hyperlink.HasLocationString = value; } }
		public bool HasDisplayName { get { return hyperlink.HasDisplayName; } set { hyperlink.HasDisplayName = value; } }
		public bool HasGUID { get { return hyperlink.HasGUID; } set { hyperlink.HasGUID = value; } }
		public bool HasCreationTime { get { return hyperlink.HasCreationTime; } set { hyperlink.HasCreationTime = value; } }
		public bool HasFrameName { get { return hyperlink.HasFrameName; } set { hyperlink.HasFrameName = value; } }
		public bool IsMonkerSavedAsString { get { return hyperlink.IsMonkerSavedAsString; } set { hyperlink.IsMonkerSavedAsString = value; } }
		public bool IsAbsoluteFromRelative { get { return hyperlink.IsAbsoluteFromRelative; } set { hyperlink.IsAbsoluteFromRelative = value; } }
		public string DisplayName { get { return hyperlink.DisplayName; } set { hyperlink.DisplayName = value; } }
		public string FrameName { get { return hyperlink.FrameName; } set { hyperlink.FrameName = value; } }
		public string Moniker { get { return hyperlink.Moniker; } set { hyperlink.Moniker = value; } }
		public XlsHyperlinkMonikerBase OleMoniker { get { return hyperlink.OleMoniker; } set { hyperlink.OleMoniker = value; } }
		public string Location { get { return hyperlink.Location; } set { hyperlink.Location = value; } }
		public Guid OptionalGUID { get { return hyperlink.OptionalGUID; } set { hyperlink.OptionalGUID = value; } }
		public Int64 CreationTime { get { return hyperlink.CreationTime; } set { hyperlink.CreationTime = value; } }
		#endregion
		public override void Read(XlsReader reader, int size) {
			this.range = XlsRef8.FromStream(reader);
			this.classId = new Guid(reader.ReadBytes(16));
			if (this.classId != XlsHyperlinkObject.CLSID_StdHyperlink)
				throw new Exception("Invalid XLS file: Wrong hyperlink class id");
			this.hyperlink = XlsHyperlinkObject.FromStream(reader);
		}
		public override void Write(BinaryWriter writer) {
			this.range.Write(writer);
			writer.Write(this.classId.ToByteArray());
			this.hyperlink.Write(writer);
		}
		public override int GetSize() {
			return fixedPartSize + hyperlink.GetSize();
		}
	}
	#endregion
	#region XlsContentHyperlinkTooltip
	public class XlsContentHyperlinkTooltip : XlsContentBase {
		FutureRecordHeaderRefNoFlags header = new FutureRecordHeaderRefNoFlags() { RecordTypeId = 0x0800 };
		NullTerminatedUnicodeString tooltip = new NullTerminatedUnicodeString();
		#region Properties
		public XlsRef8 Range { get { return header.Range; } set { header.Range = value; } }
		public string Tooltip { get { return tooltip.Value; } set { tooltip.Value = value; } }
		public override FutureRecordHeaderBase RecordHeader { get { return header; } }
		#endregion
		public override void Read(XlsReader reader, int size) {
			header = FutureRecordHeaderRefNoFlags.FromStream(reader);
			tooltip = NullTerminatedUnicodeString.FromStream(reader);
		}
		public override void Write(BinaryWriter writer) {
			header.Write(writer);
			tooltip.Write(writer);
		}
		public override int GetSize() {
			return header.GetSize() + tooltip.Length;
		}
	}
	#endregion
	#region XlsContentCondFmt
	public class XlsContentCondFmt : XlsContentBase {
		#region Fields
		const int fixedPartSize = 14;
		int recordCount;
		int id;
		XlsRef8 boundRange = new XlsRef8();
		readonly List<XlsRef8> ranges = new List<XlsRef8>();
		#endregion
		#region Properties
		public int RecordCount {
			get { return recordCount; }
			set {
				CheckRecordCount(value);
				recordCount = value;
			}
		}
		public bool ToughRecalc { get; set; }
		public int Id {
			get { return id; }
			set {
				CheckValue(value, 0, 32767, "Id");
				id = value;
			}
		}
		public XlsRef8 BoundRange {
			get { return boundRange; }
			set {
				if (value != null)
					boundRange = value;
				else
					boundRange = new XlsRef8();
			}
		}
		public List<XlsRef8> Ranges { get { return ranges; } }
		#endregion
		public override void Read(XlsReader reader, int size) {
			this.recordCount = reader.ReadUInt16();
			ushort bitwiseField = reader.ReadUInt16();
			ToughRecalc = Convert.ToBoolean(bitwiseField & 0x0001);
			this.id = (bitwiseField & 0xfffe) >> 1;
			this.boundRange = XlsRef8.FromStream(reader);
			;
			int count = reader.ReadUInt16();
			for (int i = 0; i < count; i++)
				Ranges.Add(XlsRef8.FromStream(reader));
		}
		public override void Write(BinaryWriter writer) {
			writer.Write((ushort)RecordCount);
			ushort bitwiseField = (ushort)(Id << 1);
			if (ToughRecalc)
				bitwiseField |= 0x0001;
			writer.Write(bitwiseField);
			boundRange.Write(writer);
			int count = Ranges.Count;
			writer.Write((ushort)count);
			for (int i = 0; i < count; i++)
				Ranges[i].Write(writer);
		}
		public override int GetSize() {
			return fixedPartSize + Ranges.Count * 8;
		}
		protected virtual void CheckRecordCount(int value) {
			CheckValue(value, 1, 3, "RecordCount");
		}
	}
	#endregion
	#region XlsContentCondFmt12
	public class XlsContentCondFmt12 : XlsContentCondFmt {
		FutureRecordHeaderRefU header = new FutureRecordHeaderRefU() { RecordTypeId = XlsRecordType.CondFmt12 };
		public override FutureRecordHeaderBase RecordHeader { get { return header; } }
		public override void Read(XlsReader reader, int size) {
			this.header = FutureRecordHeaderRefU.FromStream(reader);
			base.Read(reader, size);
		}
		public override void Write(BinaryWriter writer) {
			this.header.RangeOfCells = true;
			this.header.Range = BoundRange;
			this.header.Write(writer);
			base.Write(writer);
		}
		public override int GetSize() {
			return base.GetSize() + this.header.GetSize();
		}
		protected override void CheckRecordCount(int value) {
			CheckValue(value, 1, ushort.MaxValue, "RecordCount");
		}
	}
	#endregion
	#region XlsContentCFBase
	public abstract class XlsContentCFBase : XlsContentBase {
		#region Fields
		const int fixedPartSize = 6;
		byte[] firstFormulaBytes = new byte[0];
		byte[] secondFormulaBytes = new byte[0];
		#endregion
		#region Properties
		public XlCondFmtType RuleType { get; set; }
		public XlCondFmtOperator Operator { get; set; }
		public byte[] FirstFormulaBytes {
			get { return firstFormulaBytes; }
			set {
				if (value == null || value.Length < 2)
					firstFormulaBytes = new byte[0];
				else {
					int size = BitConverter.ToUInt16(value, 0);
					this.firstFormulaBytes = new byte[size];
					Array.Copy(value, 2, this.firstFormulaBytes, 0, size);
				}
			}
		}
		public byte[] SecondFormulaBytes {
			get { return secondFormulaBytes; }
			set {
				if (value == null || value.Length < 2)
					secondFormulaBytes = new byte[0];
				else {
					int size = BitConverter.ToUInt16(value, 0);
					this.secondFormulaBytes = new byte[size];
					Array.Copy(value, 2, this.secondFormulaBytes, 0, size);
				}
			}
		}
		#endregion
		public override void Read(XlsReader reader, int size) {
		}
		public override void Write(BinaryWriter writer) {
			writer.Write(XlsCondFmtHelper.RuleTypeToCode(RuleType));
			writer.Write(XlsCondFmtHelper.OperatorToCode(Operator));
			ushort firstFormulaSize = (ushort)firstFormulaBytes.Length;
			writer.Write(firstFormulaSize);
			ushort secondFormulaSize = (ushort)GetSecondFormulaSize();
			writer.Write(secondFormulaSize);
			DifferentialFormat.Write(writer);
			if (firstFormulaSize > 0)
				writer.Write(firstFormulaBytes);
			if (secondFormulaSize > 0)
				writer.Write(secondFormulaBytes);
		}
		public override int GetSize() {
			return fixedPartSize + DifferentialFormat.GetSize() + firstFormulaBytes.Length + GetSecondFormulaSize();
		}
		protected abstract XlsDxfN DifferentialFormat { get; }
		int GetSecondFormulaSize() {
			if (RuleType != XlCondFmtType.CellIs)
				return 0;
			if (Operator != XlCondFmtOperator.Between && Operator != XlCondFmtOperator.NotBetween)
				return 0;
			return secondFormulaBytes.Length;
		}
	}
	#endregion
	#region XlsContentCF
	public class XlsContentCF : XlsContentCFBase {
		#region Fields
		XlsDxfN format = new XlsDxfN();
		#endregion
		#region Properties
		public XlsDxfN Format { get { return format; } }
		#endregion
		protected override XlsDxfN DifferentialFormat { get { return format; } }
	}
	#endregion
	#region XlsContentCF12
	public class XlsContentCF12 : XlsContentCFBase, IXlsCondFmtWithRuleTemplate {
		#region Fields
		FutureRecordHeader header = new FutureRecordHeader() { RecordTypeId = XlsRecordType.CF12 };
		XlsDxfN12 format = new XlsDxfN12();
		byte[] activeFormulaBytes = new byte[0];
		int priority;
		int stdDev;
		XlsCondFmtDatabarParams dataBarParams = new XlsCondFmtDatabarParams();
		XlsCondFmtFilterParams filterParams = new XlsCondFmtFilterParams();
		XlsCondFmtIconSetParams iconSetParams = new XlsCondFmtIconSetParams();
		XlsCondFmtColorScaleParams colorScaleParams = new XlsCondFmtColorScaleParams();
		#endregion
		#region Properties
		public XlsDxfN12 Format { get { return format; } }
		public byte[] ActiveFormulaBytes {
			get { return activeFormulaBytes; }
			set {
				if (value == null || value.Length < 2)
					activeFormulaBytes = new byte[0];
				else {
					int size = BitConverter.ToUInt16(value, 0);
					this.activeFormulaBytes = new byte[size];
					Array.Copy(value, 2, this.activeFormulaBytes, 0, size);
				}
			}
		}
		public bool StopIfTrue { get; set; }
		public int Priority {
			get { return priority; }
			set {
				CheckValue(value, 0, ushort.MaxValue, "Priority");
				priority = value;
			}
		}
		public XlsCFRuleTemplate RuleTemplate { get; set; }
		public bool FilterTop { get; set; }
		public bool FilterPercent { get; set; }
		public int FilterValue { get; set; }
		public XlCondFmtSpecificTextType TextRule { get; set; }
		public int StdDev {
			get { return stdDev; }
			set {
				CheckValue(value, 0, 3, "StdDev");
				stdDev = value;
			}
		}
		public XlsCondFmtDatabarParams DataBarParams { get { return dataBarParams; } }
		public XlsCondFmtFilterParams FilterParams { get { return filterParams; } }
		public XlsCondFmtIconSetParams IconSetParams { get { return iconSetParams; } }
		public XlsCondFmtColorScaleParams ColorScaleParams { get { return colorScaleParams; } }
		public override FutureRecordHeaderBase RecordHeader { get { return header; } }
		#endregion
		public XlsContentCF12()
			: base() {
			RuleTemplate = XlsCFRuleTemplate.CellValue;
		}
		public override void Write(BinaryWriter writer) {
			this.header.RangeOfCells = false;
			this.header.Write(writer);
			base.Write(writer);
			ushort activeFormulaSize = (ushort)activeFormulaBytes.Length;
			writer.Write(activeFormulaSize);
			if (activeFormulaSize > 0)
				writer.Write(activeFormulaBytes);
			byte bitwiseField = 0;
			if (StopIfTrue)
				bitwiseField |= 0x02;
			writer.Write(bitwiseField);
			writer.Write((ushort)Priority);
			writer.Write((ushort)RuleTemplate);
			XlsCondFmtHelper.WriteTemplateParams(writer, this);
			if (RuleType == XlCondFmtType.DataBar)
				this.dataBarParams.Write(writer);
			else if (RuleType == XlCondFmtType.Top10)
				this.filterParams.Write(writer);
			else if (RuleType == XlCondFmtType.IconSet)
				this.iconSetParams.Write(writer);
			else if (RuleType == XlCondFmtType.ColorScale)
				this.colorScaleParams.Write(writer);
		}
		public override int GetSize() {
			int result = 12 + base.GetSize() + 2 + activeFormulaBytes.Length + 5 + 17;
			if (RuleType == XlCondFmtType.DataBar)
				result += this.dataBarParams.GetSize();
			else if (RuleType == XlCondFmtType.Top10)
				result += this.filterParams.GetSize();
			else if (RuleType == XlCondFmtType.IconSet)
				result += this.iconSetParams.GetSize();
			else if (RuleType == XlCondFmtType.ColorScale)
				result += this.colorScaleParams.GetSize();
			return result;
		}
		protected override XlsDxfN DifferentialFormat { get { return format; } }
	}
	#endregion
	#region XlsContentCFEx
	public class XlsContentCFEx : XlsContentBase, IXlsCondFmtWithRuleTemplate {
		#region Fields
		FutureRecordHeaderRefU header = new FutureRecordHeaderRefU() { RecordTypeId = XlsRecordType.CFEx };
		int id;
		XlsDxfN12 format = new XlsDxfN12();
		#endregion
		#region Properties
		public XlsRef8 Range { get { return header.Range; } set { header.Range = value; } }
		public bool IsCF12 { get; set; }
		public int Id {
			get { return id; }
			set {
				CheckValue(value, 0, 32767, "Id");
				id = value;
			}
		}
		public int CFIndex { get; set; }
		public XlCondFmtOperator Operator { get; set; }
		public XlsCFRuleTemplate RuleTemplate { get; set; }
		public int Priority { get; set; }
		public bool IsActive { get; set; }
		public bool StopIfTrue { get; set; }
		public bool HasFormat { get; set; }
		public XlsDxfN12 Format { get { return format; } }
		public bool FilterTop { get; set; }
		public bool FilterPercent { get; set; }
		public int FilterValue { get; set; }
		public XlCondFmtSpecificTextType TextRule { get; set; }
		public int StdDev { get; set; }
		public override FutureRecordHeaderBase RecordHeader { get { return header; } }
		#endregion
		public override void Read(XlsReader reader, int size) {
		}
		public override void Write(BinaryWriter writer) {
			this.header.RangeOfCells = true;
			this.header.Write(writer);
			writer.Write((int)(IsCF12 ? 1 : 0));
			writer.Write((ushort)id);
			if (!IsCF12) {
				writer.Write((ushort)CFIndex);
				writer.Write(XlsCondFmtHelper.OperatorToCode(Operator));
				writer.Write((byte)RuleTemplate);
				writer.Write((ushort)Priority);
				byte bitwiseField = 0;
				if (IsActive)
					bitwiseField |= 0x01;
				if (StopIfTrue)
					bitwiseField |= 0x02;
				writer.Write(bitwiseField);
				writer.Write((byte)(HasFormat ? 1 : 0));
				if (HasFormat)
					Format.Write(writer);
				XlsCondFmtHelper.WriteTemplateParams(writer, this);
			}
		}
		public override int GetSize() {
			int result = 18;
			if (!IsCF12) {
				result += 25;
				if (HasFormat)
					result += this.format.GetSize();
			}
			return result;
		}
	}
	#endregion
	#region XlsFeatureType
	public enum XlsFeatureType {
		Protection = 0x0002,
		IgnoredErrors = 0x0003,
		SmartTag = 0x0004,
		List = 0x0005
	}
	#endregion
	#region XlsContentFeatBase
	public abstract class XlsContentFeatBase : XlsContentBase {
		public XlsFeatureType FeatureType { get; set; }
		public override void Read(XlsReader reader, int size) {
		}
	}
	#endregion
	#region XlsContentFeatHdr
	public class XlsContentFeatHdr : XlsContentFeatBase {
		FutureRecordHeader header = new FutureRecordHeader() { RecordTypeId = XlsRecordType.FeatHdr };
		byte[] data = null;
		public override FutureRecordHeaderBase RecordHeader { get { return header; } }
		public byte[] Data { get { return data; } set { data = value; } }
		public override void Write(BinaryWriter writer) {
			header.Write(writer);
			writer.Write((ushort)FeatureType);
			writer.Write((byte)1); 
			if (Data != null) {
				writer.Write((int)-1);
				writer.Write(Data);
			}
			else
				writer.Write((int)0);
		}
		public override int GetSize() {
			int result = header.GetSize() + 7;
			if (Data != null)
				result += Data.Length;
			return result;
		}
	}
	#endregion
	#region XlsContentFeat
	public abstract class XlsContentFeat : XlsContentFeatBase {
		#region Fields
		FutureRecordHeader header = new FutureRecordHeader() { RecordTypeId = XlsRecordType.Feat };
		readonly List<XlsRef8> refs = new List<XlsRef8>();
		#endregion
		#region Properties
		public List<XlsRef8> Refs { get { return refs; } }
		public override FutureRecordHeaderBase RecordHeader { get { return header; } }
		#endregion
		public override void Write(BinaryWriter writer) {
			header.Write(writer);
			writer.Write((ushort)FeatureType);
			writer.Write((byte)0); 
			writer.Write((int)0); 
			int count = Refs.Count;
			writer.Write((ushort)count);
			writer.Write((int)(FeatureType == XlsFeatureType.IgnoredErrors ? 4 : 0));
			writer.Write((ushort)0); 
			for (int i = 0; i < count; i++)
				Refs[i].Write(writer);
		}
		public override int GetSize() {
			return header.GetSize() + 15 + Refs.Count * 8;
		}
	}
	#endregion
	#region XlsContentFeatIgnoredErrors
	public class XlsContentFeatIgnoredErrors : XlsContentFeat {
		public XlsContentFeatIgnoredErrors()
			: base() {
			FeatureType = XlsFeatureType.IgnoredErrors;
		}
		public bool CalculationErrors { get; set; }
		public bool EmptyCellRef { get; set; }
		public bool NumberStoredAsText { get; set; }
		public bool InconsistRange { get; set; }
		public bool InconsistFormula { get; set; }
		public bool TextDateInsuff { get; set; }
		public bool UnprotectedFormula { get; set; }
		public bool DataValidation { get; set; }
		public override void Write(BinaryWriter writer) {
			base.Write(writer);
			uint bitwiseField = 0;
			if (CalculationErrors)
				bitwiseField |= 0x0001;
			if (EmptyCellRef)
				bitwiseField |= 0x0002;
			if (NumberStoredAsText)
				bitwiseField |= 0x0004;
			if (InconsistRange)
				bitwiseField |= 0x0008;
			if (InconsistFormula)
				bitwiseField |= 0x0010;
			if (TextDateInsuff)
				bitwiseField |= 0x0020;
			if (UnprotectedFormula)
				bitwiseField |= 0x0040;
			if (DataValidation)
				bitwiseField |= 0x0080;
			writer.Write(bitwiseField);
		}
		public override int GetSize() {
			return base.GetSize() + 4;
		}
	}
	#endregion
}
