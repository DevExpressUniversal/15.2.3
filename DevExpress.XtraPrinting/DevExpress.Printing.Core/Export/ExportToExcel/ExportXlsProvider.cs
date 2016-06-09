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
using System.Text;
using System.IO;
using System.Drawing;
using DevExpress.XtraPrinting;
using System.Drawing.Printing;
using DevExpress.XtraPrinting.Native;
using DevExpress.Data.Export;
using System.Globalization;
using DevExpress.XtraPrinting.Export;
using System.Drawing.Drawing2D;
using DevExpress.XtraReports.UI;
using DevExpress.XtraPrinting.Export.XLS;
namespace DevExpress.XtraExport {
	public interface IExportXlsProvider : IExportProvider {
		void SetCellImage(TwoCellAnchorInfo twoCellAnchorInfo, Image image, SizeF innerSize, PaddingInfo padding, string hyperlink);
		void SetCellShape(TwoCellAnchorInfo twoCellAnchorInfo, Color lineColor, LineDirection lineDirection, DashStyle lineStyle, float lineWidth, string hyperlink);
		void SetCellUrlAndAnchor(int col, int row, ITableCell tableCell);
		void SetGroupIndex(int row, int index);
		void SetAdditionalSettings(bool fitToPageWidth, bool fitToPageHeight, DevExpress.Utils.DefaultBoolean rightToLeft);
	}
	[CLSCompliant(false), Obsolete("")]
	public class ExportXlsProvider : ExportXlsProviderInternal {
		public ExportXlsProvider(string fileName)
			: base(fileName) {
		}
		public ExportXlsProvider(Stream stream, string sheetName, WorkbookColorPaletteCompliance workbookColorPaletteCompliance)
			: base(stream, sheetName, workbookColorPaletteCompliance) {
		}
		protected ExportXlsProvider(Stream stream)
			: base(stream) {
		}
	}
	[CLSCompliant(false)]
	public class ExportXlsProviderInternal : ExportExcelProvider, IExportXlsProvider {
		internal static bool HasUrlScheme(string url) {
			return PrintingSettings.AllowCustomUrlScheme 
				? System.Text.RegularExpressions.Regex.IsMatch(url, @"^[a-zA-Z]([a-zA-Z0-9+\-\.])*:")
				: url.StartsWith("http") || url.StartsWith("mailto") || url.StartsWith("file:///");
		} 
		#region fields
		Dictionary<ushort, Color> colorTable = XlsConsts.CreateDefaultColorTable();
		MsoCrc32Compute crc32 = new MsoCrc32Compute();
		int[] palette = new int[56];
		int usedColors;
		XlsRecordList fonts = new XlsRecordList(XlsConsts.Font);
		XlsRecordList formats = new XlsRecordList(XlsConsts.Format);
		XlsRecordList styles = new XlsRecordList(XlsConsts.XF);
		XlsRecordList stylesExt = new XlsRecordList(XlsConsts.XFEXT);
		XlsRecordList stylesChecksum = new XlsRecordList(XlsConsts.XFCRC);
		XlsCellData cells = new XlsCellData();
		DynamicMergeRectBuffer unionCells = new DynamicMergeRectBuffer();
		XlsRecordList colStyles = new XlsRecordList(XlsConsts.COLINFO);
		XlsRecordList rowStyles = new XlsRecordList(XlsConsts.Row);
		int maxCol = -1;
		int maxRow = -1;
		int unionCellsCount;
		int unionCellsCapacity;
		XlsWorkBookWriter workBoolWriter = new XlsWorkBookWriter();
		XlsStringTable sst = new XlsStringTable();
		XlsStream stream;
		WorkbookColorPaletteCompliance workbookColorPaletteCompliance;
		MarginsF margins;
		bool landscape;
		short paperKind;
		bool fitToPageWidth;
		DevExpress.Utils.DefaultBoolean rightToLeft;
		byte[] unicodeSheetNameBytes;
		bool visibleGrid;
		protected string invalidCellDimension = "Invalid cell dimension";
		XlsPictureCollection pictures = new XlsPictureCollection();
		XlsPictureWriter picturesWriter = new XlsPictureWriter();
		List<XlsHyperlink> hyperlinks = new List<XlsHyperlink>();
		#endregion
		public ExportXlsProviderInternal(string fileName)
			: base(fileName) {
			this.stream = CreateXlsStream(new FileStream(fileName, FileMode.Create, FileAccess.Write));
			Initialize();
		}
		public ExportXlsProviderInternal(Stream stream, string sheetName, WorkbookColorPaletteCompliance workbookColorPaletteCompliance)
			: base(stream, sheetName, workbookColorPaletteCompliance) {
			this.stream = CreateXlsStream(stream);
			this.workbookColorPaletteCompliance = workbookColorPaletteCompliance;
			Initialize();
		}
		protected ExportXlsProviderInternal(Stream stream)
			: this(stream, string.Empty, WorkbookColorPaletteCompliance.ReducePaletteForExactColors) {
		}
		protected internal XlsPictureCollection Pictures { get { return pictures; } }
		protected internal XlsCellData Cells { get { return cells; } }
		#region tests
#if DEBUGTEST
		internal DynamicMergeRectBuffer Test_UnionCells {
			get { return unionCells; }
		}
		internal IList<XlsHyperlink> Test_Hyperlinks {
			get { return hyperlinks; }
		}
#endif
		#endregion
		protected override void InitializeSheetName(string sheetName, string defaultSheetName) {
			this.unicodeSheetNameBytes = System.Text.Encoding.Unicode.GetBytes(string.IsNullOrEmpty(sheetName) ? defaultSheetName : sheetName);
		}
		void Initialize() {
			usedColors = palette.Length - 1;
			MoveXlsPalette();
		}
		void SetCellStringInternal(int col, int row, string str) {
			str = ValidateString(str);
			if(str.Length > 0) {
				if(str.Length <= XlsConsts.MaxLenShortStringW)
					cells.SetCellDataString(col, row, str);
				else
					cells.SetCellDataSSTString(col, row, sst.Add(str));
			}
		}
		void MoveXlsPalette() {
			for(int i = 0; i < XlsConsts.SizeOfPalette_ / 4; i++)
				palette[i] = XlsConsts.Palette_[i];
		}
		string ValidateString(string str) {
			string result = str;
			int i = 0;
			while(i < result.Length) {
				if(result[i] == '\x000D')
					result = result.Remove(i, 1);
				else
					i++;
			}
			return result;
		}
		byte[] StringToByteArray(string str) {
			byte[] result = new byte[str.Length * 2];
			int index = 0;
			for(int i = 0; i < str.Length; i++) {
				byte[] ch = BitConverter.GetBytes(str[i]);
				result[index] = ch[0];
				result[index + 1] = ch[1];
				index += 2;
			}
			return result;
		}
		void SetHyperLink(short col, short row, string url) {
			hyperlinks.Add(new XlsHyperlink(col, row, url));
		}
		void SetPictureUnion(int col, int row, int width, int height) {
			foreach(SheetPicture pic in cells.Pictures) {
				if(pic == null)
					continue;
				if(pic.Col1 == col && pic.Row1 == row) {
					pic.Col2 = (ushort)(pic.Col1 + width - 1);
					pic.Row2 = (ushort)(pic.Row1 + height - 1);
				}
			}
		}
		ExportCacheCellBorderStyle GetBorderStyle(ExportCacheCellStyle style, int index) {
			switch(index) {
				case 0:
					return style.LeftBorder;
				case 1:
					return style.RightBorder;
				case 2:
					return style.TopBorder;
				default:
					return style.BottomBorder;
			}
		}
		byte GetColorShift(int index) {
			switch(index) {
				case 0:
					return 0;
				case 1:
					return 7;
				case 2:
					return 16;
				default:
					return 23;
			}
		}
		byte GetBrushStyle(BrushStyle bs) {
			switch(bs) {
				case BrushStyle.Clear:
					return 0;
				default:
					return 1;
			}
		}
		int GetPackedFillStyle(byte style, Color fgColor, Color bkColor) {
			int result = 0;
			int fgColor_, bkColor_;
			fgColor_ = xlsGetColorIndex(fgColor, ColorItemType.BrushBKColor);
			bkColor_ = 0x41;
			ushort loWord = 0;
			ushort hiWord = 0;
			if(fgColor_ != 0x40)
				loWord = (ushort)(style << 10);
			hiWord = (ushort)(((bkColor_ & 0x7F) << 7) | (fgColor_ & 0x7F));
			result = hiWord;
			result <<= 16;
			result += loWord;
			return result;
		}
		ushort Trim(double value) {
			return value > ushort.MaxValue ? ushort.MaxValue : (ushort)value;
		}
		DynamicByteBuffer WordArrayToDynamicByteBuffer(ushort[] data) {
			DynamicByteBuffer result = new DynamicByteBuffer();
			result.Alloc(data.Length * 2);
			for(int i = 0; i < data.Length; i++)
				result.SetElements(i * 2, BitConverter.GetBytes(data[i]), 2);
			return result;
		}
		protected XlsStream CreateXlsStream(Stream stream) {
			return new XlsStream(stream);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				pictures.Clear();
				pictures = null;
				picturesWriter.Dispose();
				if(!IsStreamMode)
					this.stream.Close();
			}
			base.Dispose(disposing);
		}
		protected virtual int CalculateStoredSize() {
			int result = XlsConsts.DefaultDataSize;
			result += styles.GetFullSize();
			result += stylesChecksum.GetFullSize();
			result += stylesExt.GetFullSize();
			result += fonts.GetFullSize();
			result += formats.GetFullSize();
			result += this.unicodeSheetNameBytes.Length;
			result += sst.PackedSize;
			result += colStyles.GetFullSize();
			result += rowStyles.GetFullSize();
			if(workbookColorPaletteCompliance == WorkbookColorPaletteCompliance.ReducePaletteForExactColors) 
				result += XlsConsts.SizeOfPalette_ + 6;
			result += CalculatePicturesStoredSize();
			result += cells.FullSize;
			if(unionCellsCount > 0) {
				int size = unionCellsCount << 3;
				result += size + (int)Math.Ceiling(((double)size / 0x2000)) * 6;
			}
			foreach(XlsHyperlink hyperlink in hyperlinks)
				result += hyperlink.GetSize();
			return result;
		}
		protected int CalculatePicturesStoredSize() {
			if(!picturesWriter.IsReady)
				return 0;
			int result = 0;
			Stream innerStream = this.stream.InnerStream;
			long pos = innerStream.Position;
			CalculationXlsStream calcStream = new CalculationXlsStream(innerStream);
			try {
				picturesWriter.WriteMsoDrawingGroup(calcStream);
				picturesWriter.WriteMsoDrawing(calcStream);
				result = calcStream.CalculatedSize;
			} finally {
				innerStream.Seek(pos, SeekOrigin.Begin);
				calcStream.Close();
			}
			return result;
		}
		protected ushort xlsGetColorIndex(Color color, ColorItemType itemType) {
			if(workbookColorPaletteCompliance == WorkbookColorPaletteCompliance.AdjustColorsToDefaultPalette) {
				ushort nearest = GetExactColorIndex(color);
				return nearest != 0 ? nearest : GetNearestColorIndex(color);
			} else {
				ushort result = 0;
				int winColor = ColorTranslator.ToWin32(color);
				switch(itemType) {
					case ColorItemType.FontColor:
						if(winColor == 0)
							result = 0x7FFF;
						break;
				}
				if(result > 0)
					return result;
				for(int i = 55; i >= 0; i--) {
					if(palette[i] == winColor) {
						if(i <= usedColors) {
							if(i != usedColors) {
								int c = palette[usedColors];
								palette[usedColors] = palette[i];
								palette[i] = c;
							}
							result = (ushort)(usedColors + 8);
							usedColors--;
						} else
							result = (ushort)(i + 8);
						return result;
					}
				}
				if(usedColors >= 0) {
					palette[usedColors] = winColor;
					result = (ushort)(usedColors + 8);
					usedColors--;
				} else 
					result = 0x01;
				return result;
			}
		}
		ushort GetExactColorIndex(Color color) {
			foreach(KeyValuePair<ushort, Color> pair in colorTable) {
				if(color.R == pair.Value.R && color.G == pair.Value.G && color.B == pair.Value.B) {
					return pair.Key;
				}
			}
			return 0;
		}
		ushort GetNearestColorIndex(Color color) {
			List<Tuple<double, ushort>> items = new List<Tuple<double, ushort>>();
			foreach(KeyValuePair<ushort, Color> pair in colorTable) {
				if(IsCompatibleColors(pair.Value, color)) {
					double d = GetColorDistance(color, pair.Value, 3.0);
					items.Add(Tuple.Create(d, pair.Key));
				}
			}
			items.Sort();
			const int limit = 5;
			if(items.Count > limit)
				items.RemoveRange(limit, items.Count - limit);
			ushort nearest = 0;
				double distance = double.MaxValue;
				foreach(Tuple<double, ushort> item in items) {
					if(nearest == 0) {
						nearest = item.Item2;
						distance = ColorComparer.BrightnessDifference(color, colorTable[item.Item2]);
					} else {
						double d = ColorComparer.BrightnessDifference(color, colorTable[item.Item2]);
						if(d < distance) {
							nearest = item.Item2;
							distance = d;
						}
					}
				}
		   return nearest;
		}
		bool IsCompatibleColors(Color tableColor, Color color) {
			return IsGrayShade(tableColor, 0) == IsGrayShade(color, 3);
		}
		bool IsGrayShade(Color color, int deviation) {
			return Math.Abs(color.R - color.G) > deviation || Math.Abs(color.R - color.B) > deviation || Math.Abs(color.G - color.B) > deviation ? false : true;
		}
		double GetColorDistance(Color x, Color y, double rgbWeight) {
			double hsbD = ColorComparer.HSBDifference(x, y);
			double rgbD = ColorComparer.RGBDifference(x, y) * rgbWeight;
			return hsbD + rgbD;
		}
		protected bool xlsCheckPos(int col, int row) {
			if(maxCol < 0 || maxRow < 0)
				throw new ExportCacheException(invalidCellDimension);
			return (col < maxCol) && (row < maxRow) && (col >= 0) && (row >= 0);
		}
		protected void xlsCreateStyles() {
			if(cells.CellsList != null) {
				for(int i = 0; i < cells.CellsList.Length; i++) {
					if(cells.CellsList[i].XF >= XlsConsts.CountOfXFStyles &&
						(cells.CellsList[i].RecType & XlsConsts.MergeState) != XlsConsts.MergeState) {
						int styleIndex = cells.CellsList[i].XF - XlsConsts.CountOfXFStyles;
						ushort initialRecType = cells.CellsList[i].RecType;
						cells.CellsList[i].XF = (ushort)XlsRegisterStyle(StyleCache[styleIndex], ref cells.CellsList[i].RecType);
						StyleCache.MarkStyleAsExported(styleIndex, cells.CellsList[i].XF, initialRecType);
					}
					OnProviderProgress(i * 80 / cells.CellsList.Length);
				}
				AddStandardFormats();
			}
			OnProviderProgress(80);
		}
		protected int XlsRegisterFont(ExportCacheCellStyle style) {
			int size = (style.TextFont.Name.Length << 1) + 16;
			string name = style.TextFont.Name;
			DynamicByteBuffer font = new DynamicByteBuffer();
			font.Alloc(size + 6);
			font.SetElements(0, BitConverter.GetBytes((ushort)size));
			font.SetElements(2, BitConverter.GetBytes((ushort)((int)style.TextFont.Size * 20)));
			ushort a = 0;
			ushort b = 0;
			if(style.TextFont.Italic)
				a = 0x02;
			if(style.TextFont.Strikeout)
				b = 0x08;
			font.SetElements(4, BitConverter.GetBytes(a | b));
			font.SetElements(6, BitConverter.GetBytes(
				xlsGetColorIndex(style.TextColor, ColorItemType.FontColor)));
			a = (ushort)(style.TextFont.Bold ? 0x2BC : 0x190);
			font.SetElements(8, BitConverter.GetBytes(a));
			font.SetElement(12, Convert.ToByte(style.TextFont.Underline));
			font.SetElement(14, style.TextFont.GdiCharSet);
			font.SetElement(16, (byte)name.Length);
			font.SetElement(17, (byte)1);
			font.SetElements(18, StringToByteArray(name));
			font.SetElements(size + 2, BitConverter.GetBytes((int)0));
			return fonts.AddUniqueFontData(font) + 6;
		}
		void AddStandardFormats() {
			FormatStringToExcelNumberFormatConverter converter = new FormatStringToExcelNumberFormatConverter();
			CultureInfo culture = CultureInfo.CurrentCulture;
			string format = converter.ConvertNumeric("c0", culture).FormatString;
			AddFormat(format, 5);
			format = converter.ConvertNumeric("c2", culture).FormatString;
			AddFormat(format, 7);
		}
		void AddFormat(string formatString, int id) {
			int size = 2 * formatString.Length + 5; 
			formats.AddData(CreateFormatData(formatString, id), size);
		}
		protected override int RegisterFormat(string formatString) {
			int id = formats.Count + XlsxHelper.PredefinedNumFmtCount + 1;
			DynamicByteBuffer format = CreateFormatData(formatString, id);
			return formats.AddUniqueFormatData(format) + XlsxHelper.PredefinedNumFmtCount + 1;
		}		
		DynamicByteBuffer CreateFormatData(string formatString, int id) {
			DynamicByteBuffer format = new DynamicByteBuffer();
			int size = 2 * formatString.Length + 5;
			format.Alloc(size + 2);
			format.SetElements(0, BitConverter.GetBytes((ushort)size));
			format.SetElements(2, BitConverter.GetBytes(id));
			format.SetElements(4, BitConverter.GetBytes(formatString.Length));
			format.SetElement(6, (byte)1);
			byte[] result = Encoding.Unicode.GetBytes(formatString.ToCharArray());
			format.SetElements(7, result);
			return format;
		}
		protected int XlsRegisterStyle(ExportCacheCellStyle style, ref ushort type) {
			ushort initialType = type;
			ushort preparedCellType = XlsCellData.PrepareCellStyle(ref type);
			if(style.PreparedCellType != XlsConsts.GeneralFormat)
				preparedCellType = (ushort)style.PreparedCellType;
			if(style.WasExportedWithType(initialType))
				return style.GetExportResult(initialType);
			DynamicByteBuffer XFExt = new DynamicByteBuffer();
			XFExt.Alloc(42);
			XFExt.SetElements(0, XlsConsts.XFExt_);
			DynamicByteBuffer XF = new DynamicByteBuffer();
			XF.Alloc(22);
			XF.SetElements(0, XlsConsts.XF_, 15 * 24 + 2, 22);
			XF.SetElements(2, BitConverter.GetBytes(
				(ushort)(XlsRegisterFont(style)) & 0xFFFF));
			XF.SetElements(4, BitConverter.GetBytes((ushort)GetFormatId(style)));
			ushort xfStyleState = 0x0400 | 0x0800 | 0x1000 | 0x2000 | 0x4000 | 0x8000;
			ushort temp = (ushort)(BitConverter.ToUInt16(XF.GetElements(10, 2), 0) | xfStyleState);
			XF.SetElements(10, BitConverter.GetBytes(temp));
			byte alignment = 1;
			if(style.TextAlignment == StringAlignment.Center)
				alignment = 2;
			else if(style.TextAlignment == StringAlignment.Far)
				alignment = 3;
			if(style.LineAlignment == StringAlignment.Center)
				alignment += 16;
			else if(style.LineAlignment == StringAlignment.Far)
				alignment += 32;
			if(style.WrapText)
				alignment |= XlsConsts.WrapText;
			XF.SetElements(8, BitConverter.GetBytes((ushort)(alignment)));
			temp = BitConverter.ToUInt16(XF.GetElements(10, 2), 0);
			temp |= (ushort)(Convert.ToByte(false) << 5);
			if(style.WrapText) {
				temp |= XlsConsts.ShrinkToFit;
			}
			if(style.RightToLeft)
				temp |= XlsConsts.ReadingOrder_RightToLeft;
			XF.SetElements(10, BitConverter.GetBytes(temp));
			byte[] leftRightBorders = new byte[4] { 0, 2, 1, 3 };
			ExcelBorderLineStyle borderStyle = 0;
			for(int i = 0; i < 4; i++) {
				ExportCacheCellBorderStyle border = GetBorderStyle(style, i);
				if(!border.IsDefault && border.Width > 0) {
					borderStyle = ExcelHelper.ConvertBorderStyle(border.Width, border.BorderDashStyle);
					temp = BitConverter.ToUInt16(XF.GetElements(12, 2), 0);
					temp |= (ushort)((ushort)borderStyle << (4 * i));
					XF.SetElements(12, BitConverter.GetBytes(temp));
					int temp2 = BitConverter.ToInt32(XF.GetElements(14, 4), 0);
					temp2 |= (xlsGetColorIndex(border.Color_,
						ColorItemType.BorderColor) << GetColorShift(i));
					XF.SetElements(14, BitConverter.GetBytes(temp2));
				}
			}
				XF.SetElements(18, BitConverter.GetBytes(
					GetPackedFillStyle(GetBrushStyle(style.BrushStyle_),
					style.BkColor,
					style.FgColor)));
			XF.SetElement(19, (byte)(XF.GetElement(19) | 0x02)); 
			XFExt.SetElements(30, new byte[] { style.BkColor.R, style.BkColor.G, style.BkColor.B });
			return styles.AddUniqueStyleData(XF, styles, XFExt, stylesExt) + XlsConsts.CountOfXFStyles;
		}
		protected void xlsWriteBuf(byte[] recData) {
			stream.Write(recData, 0, recData.Length);
		}
		void xlsCalculateChecksum() {
			if(styles.Count == 0) return;
			for(int i = 0; i < 21; i++) {
				crc32.Add(XlsConsts.XF_, i * 24 + 4, 20);
			}
			for(int i = 0; i < styles.Count; i++) {
				crc32.Add(styles[i].Data.Data, 2, 20);
			}
			stylesChecksum.AddStyleChecksum(XlsConsts.CountOfXFStyles + styles.Count, crc32.CrcValue);
		}
		protected void xlsWriteHeader() {
			xlsCreateStyles();
			xlsCalculateChecksum();
			picturesWriter.CreateObjectHierarchy(Pictures, cells.Pictures);
			workBoolWriter.CreateOleStream(CalculateStoredSize(), stream);
			int pos = (int)stream.Position;
			byte[] bof = (byte[])XlsConsts.BOF.Clone();
			bof[6] = 0x05;
			xlsWriteBuf(bof);
			xlsWriteBuf(XlsConsts.TabID);
			xlsWriteBuf(XlsConsts.WINDOW1);
			for(int i = 0; i <= 4; i++)
				xlsWriteBuf(XlsConsts.Font_);
			fonts.SaveToStream(stream);
			formats.SaveToStream(stream);
			xlsWriteBuf(XlsConsts.XF_);
			styles.SaveToStream(stream);
			stylesChecksum.SaveToStream(stream);
			stylesExt.SaveToStream(stream);
			if(workbookColorPaletteCompliance == WorkbookColorPaletteCompliance.ReducePaletteForExactColors) {
				stream.Write(BitConverter.GetBytes(XlsConsts.Palette), 0, 2);
				ushort sizeOfPalette = (ushort)(palette.Length * 4);
				stream.Write(BitConverter.GetBytes((ushort)(sizeOfPalette + 2)), 0, 2);
				stream.Write(BitConverter.GetBytes((ushort)56), 0, 2);
				for(int i = 0; i < 56; i++)
					stream.Write(BitConverter.GetBytes(palette[i]), 0, 4);
			}
			stream.Write(XlsConsts.STYLE, 0, XlsConsts.SizeOfSTYLE);
			int sheetPos = (int)stream.Position + 4;
			stream.Write(BitConverter.GetBytes(XlsConsts.BoundSheet), 0, 2);
			stream.Write(BitConverter.GetBytes((ushort)(this.unicodeSheetNameBytes.Length + 8)), 0, 2);
			stream.Write(BitConverter.GetBytes((ushort)0), 0, 2);
			stream.Write(BitConverter.GetBytes((ushort)0), 0, 2);
			stream.Write(BitConverter.GetBytes((ushort)0), 0, 2);
			stream.Write(BitConverter.GetBytes((byte)this.unicodeSheetNameBytes.Length / 2), 0, 1);
			stream.Write(BitConverter.GetBytes((byte)1), 0, 1);
			for(int i = 0; i < this.unicodeSheetNameBytes.Length; i++)
				stream.Write(BitConverter.GetBytes((byte)this.unicodeSheetNameBytes[i]), 0, 1);
			picturesWriter.WriteMsoDrawingGroup(stream);
			sst.SaveToStream(stream, -1);
			xlsWriteBuf(XlsConsts.SupBook);
			xlsWriteBuf(XlsConsts.ExternSheet);
			xlsWriteBuf(XlsConsts.EOF);
			stream.Seek(sheetPos, SeekOrigin.Begin);
			sheetPos = (int)stream.Length - pos;
			stream.Write(BitConverter.GetBytes(sheetPos), 0, 4);
			stream.Seek(0, SeekOrigin.End);
		}
		void WriteMargin(byte marginValue, float value) {
			DynamicByteBuffer margin = new DynamicByteBuffer((byte[])XlsConsts.Margin.Clone());
			margin.SetElement(0, marginValue);
			margin.SetElements(4, BitConverter.GetBytes(Math.Round((double)value, 4)));
			xlsWriteBuf(margin.Data);
		}
		void WriteWorkspaceSettings() {
			DynamicByteBuffer wsbool = new DynamicByteBuffer((byte[])XlsConsts.WSBOOL.Clone());
			if(fitToPageWidth)
				wsbool.SetElement(5, 0x05);
			xlsWriteBuf(wsbool.Data);
		}
		void WriteSetup() {
			DynamicByteBuffer setup = new DynamicByteBuffer((byte[])XlsConsts.SETUP.Clone());
			setup.SetElements(4, BitConverter.GetBytes(paperKind));
			if(landscape)
				setup.SetElement(14, 0x00);
			xlsWriteBuf(setup.Data);
		}
		protected void xlsWriteWorkBook() {
			byte[] bof = (byte[])XlsConsts.BOF.Clone();
			bof[6] = 0x10;
			xlsWriteBuf(bof);
			WriteMargin(0x26, margins.Left);
			WriteMargin(0x27, margins.Right);
			WriteMargin(0x28, margins.Top);
			WriteMargin(0x29, margins.Bottom);
			WriteWorkspaceSettings();
			WriteSetup();
			DynamicByteBuffer dimension = new DynamicByteBuffer((byte[])XlsConsts.Dimension.Clone());
			dimension.SetElements(2 * 4, BitConverter.GetBytes((int)(maxRow)));
			dimension.SetElements(7 * 2, BitConverter.GetBytes((ushort)(maxCol)));
			xlsWriteBuf(dimension.Data);
			DynamicByteBuffer window2 = new DynamicByteBuffer((byte[])XlsConsts.WINDOW2.Clone());
			window2.SetElements(2 * 2, BitConverter.GetBytes((ushort)(visibleGrid ? 0x6B6 : 0x6B4)));
			if(rightToLeft == Utils.DefaultBoolean.True) {
				byte rtlByte = window2.GetElement(4);
				rtlByte |= 0x40;
				window2.SetElement(4, rtlByte);
			}
			picturesWriter.WriteMsoDrawing(stream);
			xlsWriteBuf(window2.Data);
			colStyles.SaveToStream(stream);
			rowStyles.SaveToStream(stream);
			cells.SaveToStream(stream);
			if(unionCellsCount > 0) {
				ushort C = (ushort)Math.Min(unionCellsCount, 1024);
				ushort size = (ushort)((C << 3) + 2);
				stream.Write(BitConverter.GetBytes(XlsConsts.MergeCells), 0, 2);
				stream.Write(BitConverter.GetBytes(size), 0, 2);
				stream.Write(BitConverter.GetBytes(C), 0, 2);
				for(int i = 1; i <= unionCellsCount; i++) {
					unionCells.GetElement(i - 1).WriteToStream(stream);
					if((i % 1024) == 0 && i != unionCellsCount) {
						C = (ushort)Math.Min(unionCellsCount - i, 1024);
						size = (ushort)((C << 3) + 2);
						stream.Write(BitConverter.GetBytes(XlsConsts.MergeCells), 0, 2);
						stream.Write(BitConverter.GetBytes(size), 0, 2);
						stream.Write(BitConverter.GetBytes(C), 0, 2);
					}
				}
			}
			foreach(XlsHyperlink hyperlink in hyperlinks)
				hyperlink.WriteToStream(stream);
			xlsWriteBuf(XlsConsts.EOF);
		}
		protected bool PlaceParsedString(int col, int row, string text) {
			return true;
		}
		protected override ExportExcelProvider CreateInstance(string fileName) {
			return new ExportXlsProviderInternal(fileName);
		}
		protected override ExportExcelProvider CreateInstance(Stream stream) {
			return new ExportXlsProviderInternal(stream);
		}
		protected override ExportStyleManagerBase CreateExportStyleManager(string fileName, Stream stream) {
			return new ExportStyleManager(fileName, stream);
		}
		#region IExportXlsProvider implementation
		void IExportXlsProvider.SetCellShape(TwoCellAnchorInfo twoCellAnchorInfo, Color lineColor, LineDirection lineDirection, DashStyle lineStyle, float lineWidth, string hyperLink) {
		}
		void IExportXlsProvider.SetCellImage(TwoCellAnchorInfo twoCellAnchorInfo, Image image, SizeF innerSize, PaddingInfo padding, string hyperlink) {
			SheetPicture pic = SheetPicture.CreateInstance(twoCellAnchorInfo.StartCell.X, twoCellAnchorInfo.StartCell.Y,
				Pictures.GetByImage(image), twoCellAnchorInfo.StartCellOffset.X, twoCellAnchorInfo.StartCellOffset.Y, hyperlink);
			cells.SetCellDataImage(pic);
		}
		void IExportXlsProvider.SetCellUrlAndAnchor(int col, int row, ITableCell tableCell) {
			if(!string.IsNullOrEmpty(tableCell.Url) && ExportXlsProviderInternal.HasUrlScheme(tableCell.Url))
				SetHyperLink((short)col, (short)row, tableCell.Url);
		}
		void IExportXlsProvider.SetGroupIndex(int row, int index) {
			;
		}
		void IExportXlsProvider.SetAdditionalSettings(bool fitToPageWidth, bool fitToPageHeight, DevExpress.Utils.DefaultBoolean rightToLeft) {
			this.fitToPageWidth = fitToPageWidth;
			this.rightToLeft = rightToLeft;
		}
		#endregion
		#region IExportProvider implementation
		void IExportProvider.Commit() {
			try {
				OnProviderProgress(0);
				xlsWriteHeader();
				OnProviderProgress(90);
				xlsWriteWorkBook();
				OnProviderProgress(100);
			} finally {
				if(IsStreamMode)
					stream.Flush();
				else
					stream.Close();
				ImageHelper.Reset();
			}
		}
		int IExportProvider.RegisterStyle(ExportCacheCellStyle style) {
			return StyleCache.RegisterStyle(style);
		}
		void IExportProvider.SetDefaultStyle(ExportCacheCellStyle style) {
			StyleCache.DefaultStyle = style;
		}
		void IExportProvider.SetStyle(ExportCacheCellStyle style) {
			for(int i = 0; i <= maxCol; i++)
				for(int j = 0; j < maxRow; j++)
					((IExportProvider)this).SetCellStyle(i, j, style);
		}
		void IExportProvider.SetStyle(int styleIndex) {
			for(int i = 0; i <= maxCol; i++)
				for(int j = 0; j < maxRow; j++)
					((IExportProvider)this).SetCellStyle(i, j, styleIndex);
		}
		void IExportProvider.SetCellStyle(int col, int row, int styleIndex) {
			if(xlsCheckPos(col, row))
				cells.GetCell(col, row).XF = (ushort)(styleIndex + XlsConsts.CountOfXFStyles);
		}
		void IExportProvider.SetCellStyle(int col, int row, ExportCacheCellStyle style) {
			if(xlsCheckPos(col, row))
				((IExportProvider)this).SetCellStyle(col, row, ((IExportProvider)this).RegisterStyle(style));
		}
		void IExportProvider.SetCellStyle(int col, int row, int exampleCol, int exampleRow) {
			if(xlsCheckPos(exampleCol, exampleRow))
				cells.GetCell(col, row).XF = cells.GetCell(exampleCol, exampleRow).XF;
		}
		void IExportProvider.SetCellUnion(int col, int row, int width, int height) {
			if(!xlsCheckPos(col, row))
				return;
			width = Math.Min(width, maxCol - col);
			height = Math.Min(height, maxRow - row);
			if(width == 1 && height == 1) return;
			if(unionCellsCount == unionCellsCapacity) {
				unionCellsCapacity = ((unionCellsCapacity >> 1) + 1) << 2;
				unionCells.Realloc(unionCellsCapacity);
			}
			MergeRect rect = unionCells.GetElement(unionCellsCount);
			rect.Top = (ushort)row;
			rect.Bottom = (ushort)(row + height - 1);
			rect.Left = (ushort)col;
			rect.Right = (ushort)(col + width - 1);
			unionCells.SetElement(unionCellsCount, (MergeRect)rect);
			unionCellsCount++;
			for(int i = col; i < col + width; i++)
				for(int j = row; j < row + height; j++)
					if(i != col || j != row)
						((IExportProvider)this).SetCellStyle(i, j, col, row);
			SetPictureUnion(col, row, width, height);
		}
		void IExportProvider.SetCellStyleAndUnion(int col, int row, int width, int height, int styleIndex) {
			((IExportProvider)this).SetCellStyle(col, row, styleIndex);
			((IExportProvider)this).SetCellUnion(col, row, width, height);
		}
		void IExportProvider.SetCellStyleAndUnion(int col, int row, int width, int height, ExportCacheCellStyle style) {
			((IExportProvider)this).SetCellStyleAndUnion(col, row, width, height, ((IExportProvider)this).RegisterStyle(style));
		}
		void IExportProvider.SetPageSettings(MarginsF margins, PaperKind paperKind, bool landscape) {
			this.margins = margins;
			this.landscape = landscape;
			this.paperKind = (short)paperKind;
		}
		void IExportProvider.SetRange(int width, int height, bool isVisible) {
			maxCol = Math.Min(width, XlsConsts.MaxColumn + 1);
			maxRow = Math.Min(height, XlsConsts.MaxRow + 1);
			colStyles.Capacity = maxCol;
			rowStyles.Capacity = maxRow;
			visibleGrid = isVisible;
			cells.SetRange(maxCol, maxRow);
			ExportCacheCellStyle defaultStyle = StyleCache.DefaultStyle;
			int defaultBorderWidth = 0;
			if(isVisible)
				defaultBorderWidth = 1;
			defaultStyle.LeftBorder.Width = defaultBorderWidth;
			defaultStyle.TopBorder.Width = defaultBorderWidth;
			defaultStyle.RightBorder.Width = defaultBorderWidth;
			defaultStyle.BottomBorder.Width = defaultBorderWidth;
			((IExportProvider)this).SetDefaultStyle(defaultStyle);
		}
		void IExportProvider.SetColumnWidth(int col, int width) {
			if(col > XlsConsts.MaxColumn)
				return;
			int colRecSize = 12;
			ushort[] colInfo = new ushort[(colRecSize + 2 + 1) / 2];
			colInfo[0] = (ushort)colRecSize;
			colInfo[1] = (ushort)col;
			colInfo[2] = (ushort)col;
			colInfo[3] = Trim(Math.Round(width * 36.6)); ;
			colInfo[4] = (ushort)0x000F;
			colStyles.AddData(WordArrayToDynamicByteBuffer(colInfo), colRecSize);
		}
		void IExportProvider.SetRowHeight(int row, int height) {
			if(row > XlsConsts.MaxRow)
				return;
			int rowRecSize = 16;
			ushort[] rowInfo = new ushort[(rowRecSize + 2) / 2];
			rowInfo[0] = (ushort)rowRecSize;
			rowInfo[1] = (ushort)row;
			rowInfo[3] = (ushort)0x0100;
			rowInfo[4] = Trim(Math.Round(height * 20 / 1.325));
			rowInfo[7] = (ushort)0x01C0;
			rowInfo[8] = (ushort)0x0F;
			rowStyles.AddData(WordArrayToDynamicByteBuffer(rowInfo), rowRecSize);
		}
		void IExportProvider.SetCellData(int col, int row, object data) {
			if(!xlsCheckPos(col, row)) return;
			if(data is System.Boolean)
				cells.SetCellDataBoolean(col, row, (bool)data);
			else if(ExportUtils.IsIntegerValue(data))
				cells.SetCellDataInteger(col, row, Convert.ToInt64(data));
			else if(ExportUtils.IsDoubleValue(data))
				cells.SetCellDataDouble(col, row, Convert.ToDouble(data));
			else if(data is System.String)
				SetCellStringInternal(col, row, (string)data);
			else if(data is Image)
				((IExportXlsProvider)this).SetCellImage(new TwoCellAnchorInfo(new Point(col, row), new PointF(), new Point(), new PointF()), (Image)data, SizeF.Empty, PaddingInfo.Empty, string.Empty);
			else if(data is System.DateTime) {
				DateTime value = (DateTime)data;
				if(value.Millisecond > 0)
					value = value.AddMilliseconds(-value.Millisecond);
				cells.SetCellDataDateTime(col, row, value);
			} else if(data is System.TimeSpan)
				cells.SetCellDataTimeSpan(col, row, (System.TimeSpan)data);
			else
				((IExportProvider)this).SetCellData(col, row, Convert.ToString(data));
		}
		void IExportProvider.SetCellString(int col, int row, string str) {
			if(xlsCheckPos(col, row))
				SetCellStringInternal(col, row, str);
		}
		ExportCacheCellStyle IExportProvider.GetStyle(int styleIndex) {
			return StyleCache[styleIndex];
		}
		ExportCacheCellStyle IExportProvider.GetCellStyle(int col, int row) {
			if(xlsCheckPos(col, row)) {
				ushort XF = cells.GetCell(col, row).XF;
				if(XF > XlsConsts.CountOfXFStyles)
					return StyleCache[XF - XlsConsts.CountOfXFStyles];
				else
					return StyleCache[0];
			} else
				return new ExportCacheCellStyle();
		}
		ExportCacheCellStyle IExportProvider.GetDefaultStyle() {
			return StyleCache.DefaultStyle;
		}
		int IExportProvider.GetColumnWidth(int col) {
			return 0;
		}
		int IExportProvider.GetRowHeight(int row) {
			return 0;
		}
		IExportProvider IExportProvider.Clone(string fileName, Stream stream) {
			return (IExportProvider)CloneCore(fileName, stream);
		}
		#endregion
	}
	[CLSCompliant(false)]
	public class CalculationXlsStream : XlsStream {
		int calculatedSize = 0;
		public CalculationXlsStream(Stream stream)
			: base(stream) {
		}
		public int CalculatedSize { get { return calculatedSize; } }
		protected internal override void WriteCore(byte[] buffer, int offset, int count) {
			calculatedSize += count;
		}
		public override void Flush() {
		}
		public override void Close() {
		}
	}
	[CLSCompliant(false)]
	public struct MergeRect {
		public ushort Top, Bottom, Left, Right;
		public const int SizeOf = 4 * 2;
		public void WriteToStream(XlsStream stream) {
			stream.Write(Top);
			stream.Write(Bottom);
			stream.Write(Left);
			stream.Write(Right);
		}
	}
	public enum ColorItemType {
		FontColor,
		BrushBKColor,
		BrushFGColor,
		BorderColor
	}
	[CLSCompliant(false)]
	public class XlsHyperlink {
		short col;
		short row;
		string url;
		public XlsHyperlink(short col, short row, string url) {
			this.col = col;
			this.row = row;
			this.url = url;
		}
		public short Column { get { return col; } }
		public short Row { get { return row; } }
		public string Url { get { return url; } }
		public int GetSize() {
			const short hyperlinkHeaderSize = 26;
			return hyperlinkHeaderSize + (XlsConsts.SizeOfHyperlinkData + Url.Length) * 2;
		}
		public void WriteToStream(XlsStream stream) {
			System.Text.UnicodeEncoding encoding = new System.Text.UnicodeEncoding();
			short zeroWord = 0;
			byte shortSize = 2;
			byte fourByteSize = 4;
			stream.WriteHeader(XlsConsts.BIFFRecId_Hlink, GetSize() - 4);
			stream.Write(BitConverter.GetBytes(Row), 0, shortSize);
			stream.Write(BitConverter.GetBytes(Row), 0, shortSize);
			stream.Write(BitConverter.GetBytes(Column), 0, shortSize);
			stream.Write(BitConverter.GetBytes(Column), 0, shortSize);
			stream.Write(XlsConsts.HyperlinkData1, 0, XlsConsts.SizeOfHyperlinkData);
			stream.Write(BitConverter.GetBytes(0x2), 0, fourByteSize);
			stream.Write(BitConverter.GetBytes(0x3), 0, fourByteSize);
			stream.Write(XlsConsts.HyperlinkData2, 0, XlsConsts.SizeOfHyperlinkData);
			short length = (short)(Url.Length * 2);
			stream.Write(BitConverter.GetBytes(length + 2), 0, shortSize);
			stream.Write(BitConverter.GetBytes(zeroWord), 0, shortSize);
			stream.Write(encoding.GetBytes(Url), 0, length);
			stream.Write(BitConverter.GetBytes(zeroWord), 0, shortSize);
		}
	}
	static class ColorComparer {
		public static double RGBDifference(Color x, Color y) {
			double deltaR = ((double)x.R - (double)y.R) / 255;
			double deltaG = ((double)x.G - (double)y.G) / 255;
			double deltaB = ((double)x.B - (double)y.B) / 255;
			return Math.Sqrt(deltaR * deltaR + deltaG * deltaG + deltaB * deltaB);
		}
		public static double HSBDifference(Color x, Color y) {
			double deltaH = Math.Abs(x.GetHue() - y.GetHue());
			if(deltaH > 180.0)
				deltaH = 360.0 - deltaH;
			deltaH /= 57.3;
			double deltaB = Math.Abs(x.GetBrightness() - y.GetBrightness()) * 3.0;
			double deltaS = Math.Abs(x.GetSaturation() - y.GetSaturation()) * 1.5;
			return deltaB + deltaH + deltaS;
		}
		public static double BrightnessDifference(Color x, Color y) {
			double xBrightness = GetBrightness(x);
			double yBrightness = GetBrightness(y);
			return Math.Abs(xBrightness - yBrightness);
		}
		static double GetBrightness(Color color) {
			return ((color.R * 299) + (color.G * 587) + (color.B * 114)) / 1000; 
		}
	}
}
