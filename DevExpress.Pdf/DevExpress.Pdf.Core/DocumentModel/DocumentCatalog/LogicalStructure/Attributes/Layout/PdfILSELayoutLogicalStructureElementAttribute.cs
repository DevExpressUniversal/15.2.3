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
	[PdfDefaultField(PdfILSELayoutLogicalStructureElementAttributeTextDecorationType.None)]
	public enum PdfILSELayoutLogicalStructureElementAttributeTextDecorationType { None, Underline, Overline, LineThrough }
	[PdfDefaultField(PdfILSELayoutLogicalStructureElementAttributeRubyAlign.Distribute)]
	public enum PdfILSELayoutLogicalStructureElementAttributeRubyAlign { Start, Center, End, Justify, Distribute }
	[PdfDefaultField(PdfILSELayoutLogicalStructureElementAttributeRubyPosition.Before)]
	public enum PdfILSELayoutLogicalStructureElementAttributeRubyPosition { Before, After, Warichu, Inline }
	public class PdfILSELayoutLogicalStructureElementAttribute : PdfLayoutLogicalStructureElementAttribute {
		const string baselineShiftKey = "BaselineShift";
		const string lineHeightKey = "LineHeight";
		const string textDecorationColorKey = "TextDecorationColor";
		const string textDecorationThicknessKey = "TextDecorationThickness";
		const string textDecorationTypeKey = "TextDecorationType";
		const string rubyAlignKey = "RubyAlign";
		const string rubyPositionKey = "RubyPosition";
		const string glyphOrientationVerticalKey = "GlyphOrientationVertical";
		internal static string[] Keys = new string[] { baselineShiftKey, lineHeightKey, textDecorationColorKey, textDecorationThicknessKey, textDecorationTypeKey, rubyAlignKey, rubyPositionKey, glyphOrientationVerticalKey };
		readonly double baselineShift = 0.0;
		readonly double? lineHeight = null;
		readonly PdfColor textDecorationColor;
		readonly double? textDecorationThickness;
		readonly PdfILSELayoutLogicalStructureElementAttributeTextDecorationType textDecorationType = PdfILSELayoutLogicalStructureElementAttributeTextDecorationType.None;
		readonly PdfILSELayoutLogicalStructureElementAttributeRubyAlign rubyAlign = PdfILSELayoutLogicalStructureElementAttributeRubyAlign.Distribute;
		readonly PdfILSELayoutLogicalStructureElementAttributeRubyPosition rubyPosition = PdfILSELayoutLogicalStructureElementAttributeRubyPosition.Before;
		readonly object glyphOrientationVertical = null;
		public double BaselineShift { get { return baselineShift; } }
		public double? LineHeight { get { return lineHeight; } }
		public object TextDecorationColor { get { return textDecorationColor; } }
		public double? TextDecorationThickness { get { return textDecorationThickness; } }
		public PdfILSELayoutLogicalStructureElementAttributeTextDecorationType TextDecorationType { get { return textDecorationType; } }
		public PdfILSELayoutLogicalStructureElementAttributeRubyAlign RubyAlign { get { return rubyAlign; } }
		public PdfILSELayoutLogicalStructureElementAttributeRubyPosition RubyPosition { get { return rubyPosition; } }
		public object GlyphOrientationVertical { get { return glyphOrientationVertical; } }
		internal PdfILSELayoutLogicalStructureElementAttribute(PdfReaderDictionary dictionary) : base(dictionary) {
			object value;
			baselineShift = dictionary.GetNumber(baselineShiftKey) ?? 0.0;
			lineHeight = dictionary.GetNumber(lineHeightKey);
			textDecorationColor = ConvertToColor(dictionary.GetDoubleArray(textDecorationColorKey));
			textDecorationThickness = dictionary.GetNumber(textDecorationThicknessKey);
			if (textDecorationThickness.HasValue && textDecorationThickness.Value < 0)
				PdfDocumentReader.ThrowIncorrectDataException();
			textDecorationType = PdfEnumToStringConverter.Parse<PdfILSELayoutLogicalStructureElementAttributeTextDecorationType>(dictionary.GetName(textDecorationTypeKey));
			rubyAlign = PdfEnumToStringConverter.Parse<PdfILSELayoutLogicalStructureElementAttributeRubyAlign>(dictionary.GetName(rubyAlignKey));
			rubyPosition = PdfEnumToStringConverter.Parse<PdfILSELayoutLogicalStructureElementAttributeRubyPosition>(dictionary.GetName(rubyPositionKey));
			if (dictionary.TryGetValue(glyphOrientationVerticalKey, out value))
				PdfDocumentReader.ThrowIncorrectDataException();
		}
		protected override PdfWriterDictionary CreateDictionary(PdfObjectCollection collection) {
			PdfWriterDictionary dictionary = base.CreateDictionary(collection);
			dictionary.Add(baselineShiftKey, baselineShift);
			if (lineHeight.HasValue)
				dictionary.Add(lineHeightKey, lineHeight.Value);
			dictionary.Add(textDecorationColorKey, textDecorationColor);
			if (textDecorationThickness.HasValue)
				dictionary.Add(textDecorationThicknessKey, textDecorationThickness);
			dictionary.AddEnumName(textDecorationTypeKey, textDecorationType);
			dictionary.AddEnumName(rubyAlignKey, rubyAlign);
			dictionary.AddEnumName(rubyPositionKey, rubyPosition);
			return dictionary;
		}
	}
}
