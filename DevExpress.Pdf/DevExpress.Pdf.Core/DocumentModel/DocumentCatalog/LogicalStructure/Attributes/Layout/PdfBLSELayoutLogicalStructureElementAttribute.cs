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

using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	[PdfDefaultField(PdfBLSELayoutLogicalStructureElementAttributeTextAlign.Start), PdfSupportUndefinedValue]
	public enum PdfBLSELayoutLogicalStructureElementAttributeTextAlign { Start, Center, End, Justify }
	[PdfDefaultField(PdfBLSELayoutLogicalStructureElementAttributeTableCellBlockAlign.Before)]
	public enum PdfBLSELayoutLogicalStructureElementAttributeTableCellBlockAlign { Before, Middle, After, Justify }
	[PdfDefaultField(PdfBLSELayoutLogicalStructureElementAttributeTableCellInlineAlign.Start)]
	public enum PdfBLSELayoutLogicalStructureElementAttributeTableCellInlineAlign { Start, Center, End }
	public class PdfBLSELayoutLogicalStructureElementAttribute : PdfLayoutLogicalStructureElementAttribute {
		const string spaceBeforeKey = "SpaceBefore";
		const string spaceAfterKey = "SpaceAfter";
		const string startIndentKey = "StartIndent";
		const string endIndentKey = "EndIndent";
		const string textIndentKey = "TextIndent";
		const string textAlignKey = "TextAlign";
		const string bBoxKey = "BBox";
		const string widthKey = "Width";
		const string heightKey = "Height";
		const string blockAlignKey = "BlockAlign";
		const string inlineAlignKey = "InlineAlign";
		const string tBorderStyleKey = "TBorderStyle";
		const string tPaddingKey = "TPadding";
		internal static string[] Keys = new string[] { spaceBeforeKey, spaceAfterKey, startIndentKey, endIndentKey, textIndentKey, textAlignKey, 
													   bBoxKey, widthKey, heightKey, blockAlignKey, inlineAlignKey, tBorderStyleKey, tPaddingKey };
		static double? GetElementSize(object value) {
			if (value == null)
				return null;
			if (value is double)
				return (double)value;
			if (value is int)
				return (double)(int)value;
			PdfName name = value as PdfName;
			if (name == null || name.Name != "Auto")
				PdfDocumentReader.ThrowIncorrectDataException();
			return null;
		}
		readonly double spaceBefore = 0.0;
		readonly double spaceAfter = 0.0;
		readonly double startIndent = 0.0;
		readonly double endIndent = 0.0;
		readonly double textIndent = 0.0;
		readonly PdfBLSELayoutLogicalStructureElementAttributeTextAlign textAlign;
		readonly PdfRectangle bBox = null;
		readonly double? width = null;
		readonly double? height = null;
		readonly PdfBLSELayoutLogicalStructureElementAttributeTableCellBlockAlign tableCellBlockAlign;
		readonly PdfBLSELayoutLogicalStructureElementAttributeTableCellInlineAlign tableCellInlineAlign;
		public double SpaceBefore { get { return spaceBefore; } }
		public double SpaceAfter { get { return spaceAfter; } }
		public double StartIndent { get { return startIndent; } }
		public double EndIndent { get { return endIndent; } }
		public double TextIndent { get { return textIndent; } }
		public PdfBLSELayoutLogicalStructureElementAttributeTextAlign TextAlign { get { return textAlign; } }
		public PdfRectangle BBox { get { return bBox; } }
		public double? Width { get { return width; } }
		public double? Height { get { return height; } }
		public PdfBLSELayoutLogicalStructureElementAttributeTableCellBlockAlign TableCellBlockAlign { get { return tableCellBlockAlign; } }
		public PdfBLSELayoutLogicalStructureElementAttributeTableCellInlineAlign InlineAlign { get { return tableCellInlineAlign; } }
		internal PdfBLSELayoutLogicalStructureElementAttribute(PdfReaderDictionary dictionary) : base(dictionary) {
			object value;
			spaceBefore = dictionary.GetNumber(spaceBeforeKey) ?? 0.0;
			spaceAfter = dictionary.GetNumber(spaceAfterKey) ?? 0.0;
			startIndent = dictionary.GetNumber(startIndentKey) ?? 0.0;
			endIndent = dictionary.GetNumber(endIndentKey) ?? 0.0;
			textIndent = dictionary.GetNumber(textIndentKey) ?? 0.0;
			textAlign = PdfEnumToStringConverter.Parse<PdfBLSELayoutLogicalStructureElementAttributeTextAlign>(dictionary.GetName(textAlignKey));
			bBox = dictionary.GetRectangle(bBoxKey);
			if (dictionary.TryGetValue(widthKey, out value)) 
				width = GetElementSize(value);
			if (dictionary.TryGetValue(heightKey, out value)) 
				height = GetElementSize(value);
			tableCellBlockAlign = PdfEnumToStringConverter.Parse<PdfBLSELayoutLogicalStructureElementAttributeTableCellBlockAlign>(dictionary.GetName(blockAlignKey));
			tableCellInlineAlign = PdfEnumToStringConverter.Parse<PdfBLSELayoutLogicalStructureElementAttributeTableCellInlineAlign>(dictionary.GetName(inlineAlignKey));
		}
		protected override PdfWriterDictionary CreateDictionary(PdfObjectCollection collection) {
			PdfWriterDictionary dictionary = base.CreateDictionary(collection);
			dictionary.Add(spaceBeforeKey, spaceBefore);
			dictionary.Add(spaceAfterKey, spaceAfter, 0.0);
			dictionary.Add(startIndentKey, startIndent, 0.0);
			dictionary.Add(endIndentKey, endIndent, 0.0);
			dictionary.Add(textIndentKey, textIndent, 0.0);
			dictionary.AddName(textAlignKey, PdfEnumToStringConverter.Convert(textAlign));
			dictionary.Add(bBoxKey, bBox);
			dictionary.AddIfPresent(widthKey, width.HasValue ? (object)width.Value : null);
			dictionary.AddIfPresent(heightKey, height.HasValue ? (object)height.Value : null);
			dictionary.AddEnumName(blockAlignKey, tableCellBlockAlign);
			dictionary.AddEnumName(inlineAlignKey, tableCellInlineAlign);
			return dictionary;
		}
	}
}
