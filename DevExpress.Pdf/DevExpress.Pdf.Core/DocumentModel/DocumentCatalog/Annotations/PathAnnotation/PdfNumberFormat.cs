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
	[PdfDefaultField(PdfNumberFormatDisplayFormat.ShowAsDecimal)]
	public enum PdfNumberFormatDisplayFormat { 
		[PdfFieldName("D")]
		ShowAsDecimal,
		[PdfFieldName("F")]
		ShowAsFraction,
		[PdfFieldName("R")]
		Round,
		[PdfFieldName("T")]
		Truncate
	}
	[PdfDefaultField(PdfNumberFormatLabelPosition.Suffix)]
	public enum PdfNumberFormatLabelPosition {
		[PdfFieldName("S")]
		Suffix,
		[PdfFieldName("P")]
		Prefix
	}
	public class PdfNumberFormat : PdfObject {
		const string labelDictionaryKey = "U";
		const string conversionFactorDictionaryKey = "C";
		const string displayFormatDictionaryKey = "F";
		const string precisionDictionaryKey = "D";
		const string truncateLowOrderZerosDictionaryKey = "FD";
		const string digitalGroupingDelimiterDictionaryKey = "RT";
		const string decimalDelimiterDictionaryKey = "RD";
		const string prefixDictionaryKey = "PS";
		const string suffixDictionaryKey = "SS";
		const string labelPositionDictionaryKey = "O";
		const int defaultPrecision = 100;
		const int defaultDenominator = 16;
		const string defaultDigitalGroupingDelimiter = ",";
		const string defaultDecimalDelimiter = ".";
		const string defaultPrefixSuffix = " ";
		readonly string label;
		readonly double conversionFactor;
		readonly PdfNumberFormatDisplayFormat displayFormat;
		readonly int precision = defaultPrecision;
		readonly int denominator = defaultDenominator;
		readonly bool truncateLowOrderZeros;
		readonly string digitalGroupingDelimiter;
		readonly string decimalDelimiter;
		readonly string prefix;
		readonly string suffix;
		readonly PdfNumberFormatLabelPosition labelPosition;
		public string Label { get { return label; } }
		public double ConversionFactor { get { return conversionFactor; } }
		public PdfNumberFormatDisplayFormat DisplayFormat { get { return displayFormat; } }
		public int Precision { get { return precision; } }
		public int Denominator { get { return denominator; } }
		public bool TruncateLowOrderZeros { get { return truncateLowOrderZeros; } }
		public string DigitalGroupingDelimiter { get { return digitalGroupingDelimiter; } }
		public string DecimalDelimiter { get { return decimalDelimiter; } }
		public string Prefix { get { return prefix; } }
		public string Suffix { get { return suffix; } }
		public PdfNumberFormatLabelPosition LabelPosition { get { return labelPosition; } }
		internal PdfNumberFormat(PdfReaderDictionary dictionary) : base(dictionary.Number) {
			string type = dictionary.GetName(PdfDictionary.DictionaryTypeKey);
			label = dictionary.GetString(labelDictionaryKey);
			double? conversionFactorValue = dictionary.GetNumber(conversionFactorDictionaryKey);
			if ((type != null && type != "NumberFormat") || label == null || !conversionFactorValue.HasValue)
				PdfDocumentReader.ThrowIncorrectDataException();
			conversionFactor = conversionFactorValue.Value;
			displayFormat = dictionary.GetEnumName<PdfNumberFormatDisplayFormat>(displayFormatDictionaryKey);
			switch (displayFormat) {
				case PdfNumberFormatDisplayFormat.ShowAsDecimal:
					precision = dictionary.GetInteger(precisionDictionaryKey) ?? defaultPrecision;
					if (precision % 10 != 0)
						PdfDocumentReader.ThrowIncorrectDataException();
					break;
				case PdfNumberFormatDisplayFormat.ShowAsFraction:
					denominator = dictionary.GetInteger(precisionDictionaryKey) ?? defaultDenominator;
					if (denominator < 0)
						PdfDocumentReader.ThrowIncorrectDataException();
					break;
			}
			truncateLowOrderZeros = dictionary.GetBoolean(truncateLowOrderZerosDictionaryKey) ?? false;
			digitalGroupingDelimiter = dictionary.GetString(digitalGroupingDelimiterDictionaryKey) ?? defaultDigitalGroupingDelimiter;
			decimalDelimiter = dictionary.GetString(decimalDelimiterDictionaryKey) ?? defaultDecimalDelimiter;
			prefix = dictionary.GetString(prefixDictionaryKey) ?? defaultPrefixSuffix;
			suffix = dictionary.GetString(suffixDictionaryKey) ?? defaultPrefixSuffix;
			labelPosition = dictionary.GetEnumName<PdfNumberFormatLabelPosition>(labelPositionDictionaryKey);
		}
		protected internal override object ToWritableObject(PdfObjectCollection collection) {
			PdfWriterDictionary dictionary = new PdfWriterDictionary(collection);
			dictionary.Add(labelDictionaryKey, label);
			dictionary.Add(conversionFactorDictionaryKey, conversionFactor);
			dictionary.AddEnumName<PdfNumberFormatDisplayFormat>(displayFormatDictionaryKey, displayFormat);
			switch (displayFormat) {
				case PdfNumberFormatDisplayFormat.ShowAsDecimal:
					dictionary.Add(precisionDictionaryKey, precision, defaultPrecision);
					break;
				case PdfNumberFormatDisplayFormat.ShowAsFraction:
					dictionary.Add(precisionDictionaryKey, denominator, defaultDenominator);
					break;
			}
			dictionary.Add(truncateLowOrderZerosDictionaryKey, truncateLowOrderZeros, false);
			dictionary.Add(digitalGroupingDelimiterDictionaryKey, digitalGroupingDelimiter, defaultDigitalGroupingDelimiter);
			dictionary.Add(decimalDelimiterDictionaryKey, decimalDelimiter, defaultDecimalDelimiter);
			dictionary.Add(prefixDictionaryKey, prefix, defaultPrefixSuffix);
			dictionary.Add(suffixDictionaryKey, suffix, defaultPrefixSuffix);
			dictionary.AddEnumName<PdfNumberFormatLabelPosition>(labelPositionDictionaryKey, labelPosition);
			return dictionary;
		}
	}
}
