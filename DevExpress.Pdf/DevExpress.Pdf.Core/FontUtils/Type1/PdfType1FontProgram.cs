#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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
using System.Text;
using System.Globalization;
using System.Collections.Generic;
namespace DevExpress.Pdf.Native {
	public enum PdfType1FontPaintType { Invalid = -1, Filled = 0, Stroked = 2 }
	public enum PdfType1FontType { Invalid = -1, Type1 = 1 }
	public enum PdfType1FontWMode { Horizontal = 0, Vertical = 1 }
	public class PdfType1FontProgram {
		internal const string SerializationPattern = "/{0} {1} def\n";
		const string fontInfoDictionaryKey = "FontInfo";
		const string fontNameDictionaryKey = "FontName";
		const string encodingDictionaryKey = "Encoding";
		const string paintTypeDictionaryKey = "PaintType";
		const string fontTypeDictionaryKey = "FontType";
		const string fontMatrixDictionaryKey = "FontMatrix";
		const string fontBBoxDictionaryKey = "FontBBox";
		const string uniqueIDDictionaryKey = "UniqueID";
		const string metricsDictionaryKey = "Metrics";
		const string strokeWidthDictionaryKey = "StrokeWidth";
		const string wModeDictionaryKey = "WMode";
		public static PdfType1FontProgram Create(IType1Font font) {
			try {
				byte[] fontFileData = font.FontFileData;
				int plainTextLength = font.PlainTextLength;
				PdfPostScriptStack stack = new PdfPostScriptInterpreter().Execute(PdfPostScriptFileParser.Parse(fontFileData, font.PlainTextLength));
				if (stack.Count > 0) {
					PdfPostScriptDictionary fontProgramDictionary = stack.Peek() as PdfPostScriptDictionary;
					if (fontProgramDictionary != null)
						return new PdfType1FontProgram(font, fontProgramDictionary);
				}
			}
			catch {
			}
			return null;
		}
		static int ToInt32(object value) {
			if (!(value is int))
				PdfDocumentReader.ThrowIncorrectDataException();
			return (int)value;
		}
		static string ToName(object value) {
			PdfName name = value as PdfName;
			if (name == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			return name.Name;
		}
		static IList<object> ToList(object value) {
			IList<object> list = value as IList<object>;
			if (list == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			return list;
		}
		readonly IType1Font font;
		readonly PdfType1FontInfo fontInfo;
		readonly string fontName;
		readonly List<string> encoding;
		readonly PdfType1FontPaintType paintType = PdfType1FontPaintType.Invalid;
		readonly PdfType1FontType fontType = PdfType1FontType.Invalid;
		readonly PdfTransformationMatrix fontMatrix;
		readonly int uniqueID;
		readonly PdfPostScriptDictionary metrics;
		readonly double strokeWidth;
		readonly PdfType1FontWMode wMode;
		PdfRectangle fontBBox;
		bool shouldWrite;
		public PdfType1FontInfo FontInfo { get { return fontInfo; } }
		public string FontName { get { return fontName; } }
		public IList<string> Encoding { get { return encoding; } }
		public PdfType1FontPaintType PaintType { get { return paintType; } }
		public PdfType1FontType FontType { get { return fontType; } }
		public PdfTransformationMatrix FontMatrix { get { return fontMatrix; } }
		public int UniqueID { get { return uniqueID; } }
		public PdfPostScriptDictionary Metrics { get { return metrics; } }
		public double StrokeWidth { get { return strokeWidth; } }
		public PdfType1FontWMode WMode { get { return wMode; } }
		public PdfRectangle FontBBox { get { return fontBBox; } }
		PdfType1FontProgram(IType1Font font, PdfPostScriptDictionary dictionary) {
			this.font = font;
			shouldWrite = dictionary.DuplicatedKeys.Contains(encodingDictionaryKey);
			foreach (KeyValuePair<string, object> pair in dictionary)
				switch (pair.Key) {
					case fontInfoDictionaryKey:
						PdfPostScriptDictionary fontInfoDictionary = pair.Value as PdfPostScriptDictionary;
						if (fontInfoDictionary == null)
							PdfDocumentReader.ThrowIncorrectDataException();
						fontInfo = new PdfType1FontInfo(fontInfoDictionary);
						break;
					case fontNameDictionaryKey:
						fontName = ToName(pair.Value);
						break;
					case encodingDictionaryKey:
						object[] encodingArray = pair.Value as object[];
						if (encodingArray == null)
							PdfDocumentReader.ThrowIncorrectDataException();
						encoding = new List<string>(encodingArray.Length);
						foreach (object encodingValue in encodingArray)
							encoding.Add(encodingValue == null ? PdfGlyphNames._notdef : ToName(encodingValue));
						break;
					case paintTypeDictionaryKey:
						paintType = (PdfType1FontPaintType)ToInt32(pair.Value);
						break;
					case fontTypeDictionaryKey:
						fontType = (PdfType1FontType)ToInt32(pair.Value);
						break;
					case fontMatrixDictionaryKey:
						fontMatrix = new PdfTransformationMatrix(ToList(pair.Value));
						break;
					case fontBBoxDictionaryKey:
						fontBBox = new PdfRectangle(ToList(pair.Value));
						break;
					case uniqueIDDictionaryKey:
						uniqueID = ToInt32(pair.Value);
						break;
					case metricsDictionaryKey:
						metrics = pair.Value as PdfPostScriptDictionary;
						if (metrics == null || metrics.Count != 0)
							PdfDocumentReader.ThrowIncorrectDataException();
						break;
					case strokeWidthDictionaryKey:
						strokeWidth = PdfDocumentReader.ConvertToDouble(pair.Value);
						break;
					case wModeDictionaryKey:
						wMode = (PdfType1FontWMode)ToInt32(pair.Value);
						break;
					default:
						PdfDocumentReader.ThrowIncorrectDataException();
					break;
				}
			if (encoding == null || (paintType != PdfType1FontPaintType.Filled && paintType != PdfType1FontPaintType.Stroked) || 
				fontType != PdfType1FontType.Type1 || fontMatrix == null || fontBBox == null || (wMode != PdfType1FontWMode.Horizontal && wMode != PdfType1FontWMode.Vertical))
					PdfDocumentReader.ThrowIncorrectDataException();
		}
		public string ToPostScript() {
			StringBuilder sb = new StringBuilder();
			sb.Append(String.Format("%!FontType1-1.0: {0} {1}\n", fontName, fontInfo == null ? "001.001" : fontInfo.Version));
			sb.Append("11 dict begin\n");
			if (fontInfo != null)
				sb.Append(String.Format("/{0} {1} readonly def\n", fontInfoDictionaryKey, fontInfo.Serialize()));
			sb.Append(String.Format("/{0} /{1} def\n", fontNameDictionaryKey, fontName));
			sb.Append(String.Format("/{0} 256 array 0 1 255 {{1 index exch /.notdef put}} for\n", encodingDictionaryKey));
			int encodingLength = encoding.Count;
			for (int i = 0; i < encodingLength; i++) {
				string glyphName = encoding[i];
				if (glyphName != PdfGlyphNames._notdef)
					sb.Append(String.Format("dup {0} /{1} put\n", i, glyphName));
			}
			sb.Append("readonly def\n");
			sb.Append(String.Format(SerializationPattern, paintTypeDictionaryKey, (int)paintType));
			sb.Append(String.Format(SerializationPattern, fontTypeDictionaryKey, (int)fontType));
			CultureInfo cultureInfo = CultureInfo.InvariantCulture;
			sb.Append(String.Format(cultureInfo, "/{0} [{1} {2} {3} {4} {5} {6}] readonly def\n", fontMatrixDictionaryKey, fontMatrix.A, fontMatrix.B, fontMatrix.C, fontMatrix.D, fontMatrix.E, fontMatrix.F));
			sb.Append(String.Format(cultureInfo, "/{0} {{{1} {2} {3} {4}}} readonly def\n", fontBBoxDictionaryKey, fontBBox.Left, fontBBox.Bottom, fontBBox.Right, fontBBox.Top));
			if (uniqueID != 0)
				sb.Append(String.Format(SerializationPattern, uniqueIDDictionaryKey, uniqueID));
			if (metrics != null) {
				sb.Append(String.Format("/{0} 1 dict dup begin\n", metricsDictionaryKey));
				sb.Append("end def\n");
			}
			if (strokeWidth != 0)
				sb.Append(String.Format(SerializationPattern, strokeWidthDictionaryKey, strokeWidth));
			if (wMode != PdfType1FontWMode.Horizontal)
				sb.Append(String.Format(SerializationPattern, wModeDictionaryKey, (int)wMode));
			sb.Append("currentdict end\n");
			sb.Append("currentfile eexec\n");
			return sb.ToString();
		}
		public bool Validate() {
			PdfType1Font type1Font = font as PdfType1Font;
			if (type1Font != null) {
				PdfSimpleFontEncoding fontEncoding = type1Font.Encoding;
				foreach (KeyValuePair<int, string> pair in fontEncoding.Differences)
					if (encoding.IndexOf(pair.Value) != pair.Key) {
						encoding.Clear();
						for (int i = 0; i < 256; i++)
							encoding.Add(fontEncoding.GetGlyphName((byte)i));
						shouldWrite = true;
						break;
					}
			}
			if (fontBBox.Left == 0 && fontBBox.Right == 0 && fontBBox.Bottom == 0 && fontBBox.Top == 0) {
				fontBBox = font.FontDescriptor.FontBBox;
				shouldWrite = true;
			}
			return shouldWrite;
		}
	}
}
