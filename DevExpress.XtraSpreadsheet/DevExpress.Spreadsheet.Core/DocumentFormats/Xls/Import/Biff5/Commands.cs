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
using System.Reflection;
using DevExpress.Export.Xl;
using DevExpress.XtraExport.Xls;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Internal;
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region XlsBIFF5CommandBase (abstract)
	public abstract class XlsBIFF5CommandBase : XlsCommandBase {
		protected override void WriteCore(BinaryWriter writer) {
		}
		protected string ReadString(XlsReader reader, XlsContentBuilder contentBuilder) {
			int length = reader.ReadByte();
			byte[] buffer = reader.ReadBytes(length);
			return contentBuilder.Options.ActualEncoding.GetString(buffer, 0, length);
		}
	}
	#endregion
	#region XlsBIFF5CommandStringValueBase (abstract)
	public abstract class XlsBIFF5CommandStringValueBase : XlsBIFF5CommandBase {
		public string Value { get; private set; }
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			if (Size > 0)
				Value = ReadString(reader, contentBuilder);
			else
				Value = string.Empty;
		}
	}
	#endregion
	#region XlsBIFF5CommandCellBase (abstract)
	public abstract class XlsBIFF5CommandCellBase : XlsBIFF5CommandBase {
		#region Properties
		public int RowIndex { get; private set; }
		public int ColumnIndex { get; private set; }
		public int FormatIndex { get; private set; }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			RowIndex = reader.ReadUInt16();
			ColumnIndex = reader.ReadUInt16();
			FormatIndex = reader.ReadUInt16();
		}
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			ICell cell = contentBuilder.CurrentSheet.Rows[RowIndex].Cells[ColumnIndex];
			DocumentModel workbook = contentBuilder.DocumentModel;
			bool suppressCellValueAssignment = workbook.SuppressCellValueAssignment;
			workbook.SuppressCellValueAssignment = false;
			AssignCellValueCore(cell, contentBuilder);
			workbook.SuppressCellValueAssignment = suppressCellValueAssignment;
			if (FormatIndex > workbook.StyleSheet.DefaultCellFormatIndex)
				cell.SetCellFormatIndex(contentBuilder.StyleSheet.GetCellFormatIndex(FormatIndex));
		}
		protected abstract void AssignCellValueCore(ICell cell, XlsContentBuilder contentBuilder);
		protected string ReadString2(XlsReader reader, XlsContentBuilder contentBuilder) {
			int length = reader.ReadByte();
			reader.ReadByte();
			byte[] buffer = reader.ReadBytes(length);
			return contentBuilder.Options.ActualEncoding.GetString(buffer, 0, length);
		}
	}
	#endregion
	#region XlsBIFF5CommandBoundSheet
	public class XlsBIFF5CommandBoundSheet : XlsBIFF5CommandBase {
		#region Properties
		public int StartPosition { get; private set; }
		public SheetVisibleState VisibleState { get; private set; }
		public SheetType Type { get; private set; }
		public string Name { get; private set; }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			StartPosition = reader.ReadNotCryptedInt32();
			VisibleState = (SheetVisibleState)(reader.ReadByte() & 0x03);
			Type = (SheetType)reader.ReadByte();
			Name = ReadString(reader, contentBuilder);
		}
		public override void Execute(XlsContentBuilder contentBuilder) {
			contentBuilder.StyleSheet.RegisterFormats();
			contentBuilder.RegisterSheetInfo(Type, Name, VisibleState);
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsBIFF5CommandFont
	public class XlsBIFF5CommandFont : XlsBIFF5CommandBase {
		#region Fields
		const double fontCoeff = 20.0;
		const short defaultNormal = 400;
		const short defaultBoldness = 700;
		short boldness = defaultNormal;
		#endregion
		#region Properties
		public double FontSize { get; private set; }
		public bool Bold { get { return boldness >= defaultBoldness; } }
		public bool Italic { get; private set; }
		public bool StrikeThrough { get; private set; }
		public bool Outline { get; private set; }
		public bool Shadow { get; private set; }
		public bool Condense { get; private set; }
		public bool Extend { get; private set; }
		public XlScriptType Script { get; private set; }
		public XlUnderlineType Underline { get; private set; }
		public int FontFamily { get; private set; }
		public int Charset { get; private set; }
		public string FontName { get; private set; }
		public int ColorIndex { get; private set; }
		public short Boldness { get { return boldness; } }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			FontSize = reader.ReadInt16() / fontCoeff;
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
			FontName = ReadString(reader, contentBuilder);
		}
		protected override void ApplyWorkbookGlobalsContent(XlsContentBuilder contentBuilder) {
			XlsFontInfo info = new XlsFontInfo();
			info.Bold = Bold;
			info.Boldness = Boldness;
			info.Italic = Italic;
			info.Outline = Outline;
			info.Shadow = Shadow;
			info.StrikeThrough = StrikeThrough;
			info.Charset = Charset;
			info.FontFamily = FontFamily;
			info.Name = FontName;
			info.Size = FontSize;
			info.FontColorIndex = ColorIndex;
			info.FontColor.ColorIndex = contentBuilder.StyleSheet.GetPaletteColorIndex(ColorIndex, true);
			info.SchemeStyle = XlFontSchemeStyles.None;
			info.Script = Script;
			info.Underline = Underline;
			contentBuilder.StyleSheet.Fonts.Add(info);
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsBIFF5CommandNumberFormat
	public class XlsBIFF5CommandNumberFormat : XlsBIFF5CommandBase {
		#region Properties
		public int FormatId { get; private set; }
		public string FormatCode { get; private set; }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			FormatId = reader.ReadUInt16();
			FormatCode = ReadString(reader, contentBuilder);
		}
		protected override void ApplyWorkbookGlobalsContent(XlsContentBuilder contentBuilder) {
			string formatCode = FormatCode;
			if (string.Equals(formatCode, "GENERAL", StringComparison.OrdinalIgnoreCase))
				formatCode = string.Empty;
			contentBuilder.StyleSheet.RegisterNumberFormat(FormatId, formatCode);
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsBIFF5CommandXF
	public class XlsBIFF5CommandXF : XlsBIFF5CommandBase {
		#region Properties
		public bool IsStyleFormat { get; private set; }
		public int FontId { get; private set; }
		public int NumberFormatId { get; private set; }
		public bool IsLocked { get; private set; }
		public bool IsHidden { get; private set; }
		public bool QuotePrefix { get; private set; }
		public int StyleId { get; private set; }
		public XlHorizontalAlignment HorizontalAlignment { get; private set; }
		public bool WrapText { get; private set; }
		public XlVerticalAlignment VerticalAlignment { get; private set; }
		public int TextRotation { get; private set; }
		public bool ApplyNumberFormat { get; private set; }
		public bool ApplyFont { get; private set; }
		public bool ApplyAlignment { get; private set; }
		public bool ApplyBorder { get; private set; }
		public bool ApplyFill { get; private set; }
		public bool ApplyProtection { get; private set; }
		public XlBorderLineStyle BorderLeftLineStyle { get; private set; }
		public XlBorderLineStyle BorderRightLineStyle { get; private set; }
		public XlBorderLineStyle BorderTopLineStyle { get; private set; }
		public XlBorderLineStyle BorderBottomLineStyle { get; private set; }
		public int BorderLeftColorIndex { get; private set; }
		public int BorderRightColorIndex { get; private set; }
		public int BorderTopColorIndex { get; private set; }
		public int BorderBottomColorIndex { get; private set; }
		public XlPatternType FillPatternType { get; private set; }
		public int FillForeColorIndex { get; private set; }
		public int FillBackColorIndex { get; private set; }
		public bool PivotButton { get; private set; }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			int fontId = reader.ReadInt16();
			if (fontId == XlsDefs.UnusedFontIndex)
				fontId = 0;
			else if (fontId > XlsDefs.UnusedFontIndex)
				fontId--;
			FontId = fontId;
			NumberFormatId = reader.ReadInt16();
			int bitwiseField = reader.ReadInt16();
			IsLocked = Convert.ToBoolean(bitwiseField & 0x0001);
			IsHidden = Convert.ToBoolean(bitwiseField & 0x0002);
			IsStyleFormat = Convert.ToBoolean(bitwiseField & 0x0004);
			if (!IsStyleFormat) {
				QuotePrefix = Convert.ToBoolean(bitwiseField & 0x0008);
				StyleId = (bitwiseField & 0xfff0) >> 4;
			}
			bitwiseField = reader.ReadUInt16();
			HorizontalAlignment = (XlHorizontalAlignment)(bitwiseField & 0x0007);
			WrapText = Convert.ToBoolean(bitwiseField & 0x0008);
			VerticalAlignment = (XlVerticalAlignment)((bitwiseField & 0x0070) >> 4);
			int rotation = (bitwiseField & 0x0300) >> 8;
			if (rotation == 1)
				TextRotation = 0xff;
			else if (rotation == 2)
				TextRotation = 0x5a;
			else if (rotation == 3)
				TextRotation = 0xb4;
			else
				TextRotation = 0;
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
			FillForeColorIndex = bitwiseField & 0x007f;
			FillBackColorIndex = (bitwiseField & 0x1f80) >> 7;
			if (!IsStyleFormat)
				PivotButton = Convert.ToBoolean(bitwiseField & 0x2000);
			bitwiseField = reader.ReadUInt16();
			FillPatternType = (XlPatternType)(bitwiseField & 0x0000003f);
			BorderBottomLineStyle = (XlBorderLineStyle)((bitwiseField & 0x01c0) >> 6);
			BorderBottomColorIndex = (bitwiseField & 0x0000fe00) >> 9;
			bitwiseField = reader.ReadUInt16();
			BorderTopLineStyle = (XlBorderLineStyle)(bitwiseField & 0x0007);
			BorderLeftLineStyle = (XlBorderLineStyle)((bitwiseField & 0x0038) >> 3);
			BorderRightLineStyle = (XlBorderLineStyle)((bitwiseField & 0x01c0) >> 6);
			BorderTopColorIndex = (bitwiseField & 0x0000fe00) >> 9;
			bitwiseField = reader.ReadInt16();
			BorderLeftColorIndex = bitwiseField & 0x007f;
			BorderRightColorIndex = (bitwiseField & 0x3f80) >> 7;
		}
		protected override void ApplyWorkbookGlobalsContent(XlsContentBuilder contentBuilder) {
			XlsImportStyleSheet styleSheet = contentBuilder.StyleSheet;
			XlsExtendedFormatInfo info = new XlsExtendedFormatInfo();
			info.FontId = FontId;
			info.NumberFormatId = NumberFormatId;
			info.StyleXFIndex = StyleId;
			info.IsStyleFormat = IsStyleFormat;
			info.QuotePrefix = QuotePrefix;
			info.IsLocked = IsLocked;
			info.IsHidden = IsHidden;
			info.HorizontalAlignment = HorizontalAlignment;
			info.VerticalAlignment = VerticalAlignment;
			info.WrapText = WrapText;
			info.TextRotation = TextRotation;
			info.ApplyNumberFormat = ApplyNumberFormat;
			info.ApplyFont = ApplyFont;
			info.ApplyAlignment = ApplyAlignment;
			info.ApplyBorder = ApplyBorder;
			info.ApplyFill = ApplyFill;
			info.ApplyProtection = ApplyProtection;
			int colorIndex = styleSheet.GetBorderColorIndex(BorderLeftColorIndex);
			info.LeftBorderColor.ColorIndex = colorIndex;
			info.LeftBorderLineStyle = colorIndex == 0 ? XlBorderLineStyle.None : BorderLeftLineStyle;
			colorIndex = styleSheet.GetBorderColorIndex(BorderRightColorIndex);
			info.RightBorderColor.ColorIndex = colorIndex;
			info.RightBorderLineStyle = colorIndex == 0 ? XlBorderLineStyle.None : BorderRightLineStyle;
			colorIndex = styleSheet.GetBorderColorIndex(BorderTopColorIndex);
			info.TopBorderColor.ColorIndex = colorIndex;
			info.TopBorderLineStyle = colorIndex == 0 ? XlBorderLineStyle.None : BorderTopLineStyle;
			colorIndex = styleSheet.GetBorderColorIndex(BorderBottomColorIndex);
			info.BottomBorderColor.ColorIndex = colorIndex;
			info.BottomBorderLineStyle = colorIndex == 0 ? XlBorderLineStyle.None : BorderBottomLineStyle;
			info.FillPatternType = FillPatternType;
			info.ForegroundColor.ColorIndex = styleSheet.GetPaletteColorIndex(FillForeColorIndex, true);
			info.BackgroundColor.ColorIndex = styleSheet.GetPaletteColorIndex(FillBackColorIndex, false);
			info.PivotButton = PivotButton;
			styleSheet.ExtendedFormats.Add(info);
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsBIFF5CommandStyle
	public class XlsBIFF5CommandStyle : XlsBIFF5CommandBase {
		#region Properties
		public string StyleName { get; private set; }
		public int BuiltInId { get; private set; }
		public int OutlineLevel { get; private set; }
		public int StyleFormatId { get; private set; }
		public bool IsBuiltIn { get; private set; }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			ushort bitwiseField = reader.ReadUInt16();
			StyleFormatId = bitwiseField & 0x0fff;
			IsBuiltIn = Convert.ToBoolean(bitwiseField & 0x8000);
			if (!IsBuiltIn) {
				BuiltInId = Int32.MinValue;
				OutlineLevel = Int32.MinValue;
				StyleName = ReadString(reader, contentBuilder);
			}
			else {
				BuiltInId = reader.ReadByte();
				OutlineLevel = reader.ReadByte() + 1;
				if (BuiltInId != 0x01 && BuiltInId != 0x02) {
					OutlineLevel = Int32.MinValue;
					StyleName = BuiltInCellStyleCalculator.CalculateName(BuiltInId);
				}
				else
					StyleName = OutlineCellStyle.CalculateName(BuiltInId == 0x01, OutlineLevel);
			}
		}
		protected override void ApplyWorkbookGlobalsContent(XlsContentBuilder contentBuilder) {
			XlsStyleInfo info = new XlsStyleInfo();
			info.StyleXFIndex = StyleFormatId;
			info.IsBuiltIn = IsBuiltIn;
			info.BuiltInId = BuiltInId;
			info.OutlineLevel = OutlineLevel;
			info.Name = StyleName;
			contentBuilder.StyleSheet.AddStyle(info);
			contentBuilder.UseXFExt = true;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsBIFF5CommandPageHeader
	public class XlsBIFF5CommandPageHeader : XlsBIFF5CommandStringValueBase {
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			UpdateHeaderFooter(contentBuilder.CurrentSheet.Properties.HeaderFooter);
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			if (contentBuilder.CurrentChart != null)
				UpdateHeaderFooter(contentBuilder.CurrentChart.PrintSettings.HeaderFooter);
		}
		void UpdateHeaderFooter(HeaderFooterOptions headerFooter) {
			headerFooter.BeginUpdate();
			try {
				headerFooter.OddHeader = Value;
			}
			finally {
				headerFooter.EndUpdate();
			}
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsBIFF5CommandPageFooter
	public class XlsBIFF5CommandPageFooter : XlsBIFF5CommandStringValueBase {
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			UpdateHeaderFooter(contentBuilder.CurrentSheet.Properties.HeaderFooter);
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			if (contentBuilder.CurrentChart != null)
				UpdateHeaderFooter(contentBuilder.CurrentChart.PrintSettings.HeaderFooter);
		}
		void UpdateHeaderFooter(HeaderFooterOptions headerFooter) {
			headerFooter.BeginUpdate();
			try {
				headerFooter.OddFooter = Value;
			}
			finally {
				headerFooter.EndUpdate();
			}
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsBIFF5CommandLabel
	public class XlsBIFF5CommandLabel : XlsBIFF5CommandCellBase {
		#region Properties
		public string Value { get; private set; }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			base.ReadCore(reader, contentBuilder);
			Value = ReadString2(reader, contentBuilder);
			int bytesToRead = Size - Value.Length - 8;
			if (bytesToRead > 0)
				reader.ReadBytes(bytesToRead);
		}
		protected override void AssignCellValueCore(ICell cell, XlsContentBuilder contentBuilder) {
			DocumentModel workbook = contentBuilder.DocumentModel;
			SharedStringIndex index = workbook.SharedStringTable.RegisterString(Value);
			VariantValue value = new VariantValue();
			value.SetSharedString(workbook.SharedStringTable, index);
			cell.AssignValueCore(value);
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			if (contentBuilder.CurrentChart == null)
				return;
			List<XlsChartCachedValue> cache = contentBuilder.GetCurrentDataCache(ColumnIndex);
			if (cache != null) {
				XlsChartCachedValue cachedValue = new XlsChartCachedValue(RowIndex, Value);
				cache.Add(cachedValue);
			}
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsBIFF5FormatRun
	public class XlsBIFF5FormatRun {
		public int CharIndex { get; private set; }
		public int FontIndex { get; private set; }
		public static XlsBIFF5FormatRun FromStream(XlsReader reader) {
			XlsBIFF5FormatRun result = new XlsBIFF5FormatRun();
			result.Read(reader);
			return result;
		}
		protected void Read(XlsReader reader) {
			CharIndex = reader.ReadByte();
			FontIndex = reader.ReadByte();
		}
	}
	#endregion
	#region XlsBIFF5CommandRichString
	public class XlsBIFF5CommandRichString : XlsBIFF5CommandCellBase {
		readonly List<XlsBIFF5FormatRun> formatRuns = new List<XlsBIFF5FormatRun>();
		#region Properties
		public string Value { get; private set; }
		public List<XlsBIFF5FormatRun> FormatRuns { get { return formatRuns; } }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			base.ReadCore(reader, contentBuilder);
			Value = ReadString2(reader, contentBuilder);
			int count = reader.ReadByte();
			for (int i = 0; i < count; i++)
				FormatRuns.Add(XlsBIFF5FormatRun.FromStream(reader));
		}
		protected override void AssignCellValueCore(ICell cell, XlsContentBuilder contentBuilder) {
			DocumentModel workbook = contentBuilder.DocumentModel;
			SharedStringTable sharedStringTable = workbook.SharedStringTable;
			FormattedStringItem richTextItem = new FormattedStringItem(workbook);
			ExtractRuns(richTextItem, contentBuilder);
			sharedStringTable.Add(richTextItem);
			SharedStringIndex index = new SharedStringIndex(sharedStringTable.Count - 1);
			VariantValue value = new VariantValue();
			value.SetSharedString(sharedStringTable, index);
			cell.AssignValueCore(value);
		}
		void ExtractRuns(FormattedStringItem richTextItem, XlsContentBuilder contentBuilder) {
			int lastCharIndex = 0;
			int lastFontIndex = 0;
			string str = Value;
			int length = str.Length;
			for (int i = 0; i < FormatRuns.Count; i++) {
				XlsBIFF5FormatRun formatRun = FormatRuns[i];
				if (formatRun.CharIndex >= length) break; 
				int runLength = formatRun.CharIndex - lastCharIndex;
				if (runLength > 0) {
					AddFormattedStringItemPart(richTextItem, str.Substring(lastCharIndex, runLength),
						contentBuilder.StyleSheet.GetFontInfoIndex(lastFontIndex));
				}
				lastCharIndex = formatRun.CharIndex;
				lastFontIndex = formatRun.FontIndex;
				if (lastFontIndex == XlsDefs.UnusedFontIndex)
					contentBuilder.ThrowInvalidFile("Invalid font index in rich shared string");
				else if (lastFontIndex > XlsDefs.UnusedFontIndex)
					lastFontIndex--;
			}
			if ((length - lastCharIndex) > 0) {
				AddFormattedStringItemPart(richTextItem,
					str.Substring(lastCharIndex, length - lastCharIndex),
					contentBuilder.StyleSheet.GetFontInfoIndex(lastFontIndex));
			}
		}
		void AddFormattedStringItemPart(FormattedStringItem richTextItem, string partContent, int fontIndex) {
			FormattedStringItemPart item = richTextItem.AddNewFormattedStringItemPart();
			item.Content = partContent;
			RunFontInfo newFontInfo = new RunFontInfo();
			newFontInfo.CopyFrom(richTextItem.DocumentModel.Cache.FontInfoCache[fontIndex]);
			item.ReplaceInfo(newFontInfo, DocumentModelChangeActions.None);
		}
		public override IXlsCommand GetInstance() {
			FormatRuns.Clear();
			return this;
		}
	}
	#endregion
	#region XlsBIFF5CommandString
	public class XlsBIFF5CommandString : XlsBIFF5CommandBase {
		public string Value { get; private set; }
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			int length = reader.ReadByte();
			reader.ReadByte();
			byte[] buffer = reader.ReadBytes(length);
			Value = contentBuilder.Options.ActualEncoding.GetString(buffer, 0, length);
		}
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			if (contentBuilder.FormulaBuilder != null)
				contentBuilder.FormulaBuilder.Build(Value);
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
}
