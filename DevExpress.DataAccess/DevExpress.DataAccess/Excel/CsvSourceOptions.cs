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
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using DevExpress.DataAccess.Native;
using DevExpress.Export.Xl;
using DevExpress.SpreadsheetSource;
using DevExpress.SpreadsheetSource.Csv;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.ComponentModel.Design;
using DevExpress.Compatibility.System.Text;
using DevExpress.Utils;
namespace DevExpress.DataAccess.Excel {
	public class CsvSourceOptions : ExcelSourceOptionsBase {
		public class EncodingConverter : TypeConverter {
			static readonly StandardValuesCollection standardValuesCollection;
			static EncodingConverter() {
				var encodingInfos = DXEncoding.GetEncodings().OrderBy(e => e.DisplayName).ToArray();
				List<Encoding> values = new List<Encoding>(encodingInfos.Length);
				values.AddRange(encodingInfos.Select(t => t.GetEncoding()));
				standardValuesCollection = new StandardValuesCollection(values);
			}
			public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
				return standardValuesCollection;
			}
			public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
				return true;
			}
			public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
				return true;
			}
			public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
				if (sourceType == typeof(string))
					return true;
				return base.CanConvertFrom(context, sourceType);
			}
			public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
				if (destinationType == typeof(String) && value != null)
					return ((Encoding)value).EncodingName;
				return base.ConvertTo(context, culture, value, destinationType);
			}
			public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
				if (value == null || value.GetType() != typeof(String))
					return base.ConvertFrom(context, culture, value);
				EncodingInfo[] encodingInfos = DXEncoding.GetEncodings();
				int count = encodingInfos.Length;
				for (int i = 0; i < count; i++) {
					Encoding encoding = encodingInfos[i].GetEncoding();
					if (string.Compare(encoding.EncodingName, (string)value, true) == 0)
						return encoding;
				}
				return DXEncoding.Default;
			}
		}
		const string xml_CellRange = "CellRange";
		const string xml_CultureName = "CultureName";
		const string xml_DetectEncoding = "DetectEncoding";
		const string xml_DetectNewlineType = "DetectNewlineType";
		const string xml_DetectValueSeparator = "DetectValueSeparator";
		const string xml_EncodingName = "EncodingName";
		const string xml_NewlineType = "NewlineType";
		const string xml_TextQualifier = "TextQualifier";
		const string xml_TrimBlanks = "TrimBlanks";
		const string xml_ValueSeparator = "ValueSeparator";
		public CsvSourceOptions() {
			ValueSeparator = ',';
			NewlineType = CsvNewlineType.CRLF;
			DetectEncoding = true;
			TextQualifier = '"';
			TrimBlanks = true;
			Culture = CultureInfo.InvariantCulture;
		}
		CsvSourceOptions(CsvSourceOptions other) : base(other) {
			CellRange = other.CellRange;
			Culture = other.Culture;
			DetectEncoding = other.DetectEncoding;
			DetectNewlineType = other.DetectNewlineType;
			DetectValueSeparator = other.DetectValueSeparator;
			Encoding = other.Encoding;
			NewlineType = other.NewlineType;
			TextQualifier = other.TextQualifier;
			TrimBlanks = other.TrimBlanks;
			ValueSeparator = other.ValueSeparator;
		}
		[DefaultValue(null)]
		public string CellRange { get; set; }
		[DefaultValue(null)]
		public CultureInfo Culture { get; set; }
		[DefaultValue(true)]
		public bool DetectEncoding { get; set; }
		[DefaultValue(false)]
		public bool DetectNewlineType { get; set; }
		[DefaultValue(false)]
		public bool DetectValueSeparator { get; set; }
		[DefaultValue(null)]
		[TypeConverter(typeof(EncodingConverter))]
		public Encoding Encoding { get; set; }
		[DefaultValue(CsvNewlineType.CRLF)]
		public CsvNewlineType NewlineType { get; set; }
		[DefaultValue('"')]
		public char TextQualifier { get; set; }
		[DefaultValue(true)]
		public bool TrimBlanks { get; set; }
		[DefaultValue(',')]
		public char ValueSeparator { get; set; }
		protected internal override ExcelSourceOptionsBase Clone() {
			return new CsvSourceOptions(this);
		}
		protected internal override SpreadsheetSourceOptions GetSpreadsheetSourceOptions() {
			return new CsvSpreadsheetSourceOptions {
				Culture = Culture, DetectEncoding = DetectEncoding,
				DetectNewlineType = DetectNewlineType, DetectValueSeparator = DetectValueSeparator, Encoding = Encoding,
				NewlineType = (XtraExport.Csv.CsvNewlineType)NewlineType, TextQualifier = TextQualifier, TrimBlanks = TrimBlanks, ValueSeparator = ValueSeparator
			};
		}
		protected internal override ISpreadsheetDataReader CreateReader(ISpreadsheetSource source) {
			if(!string.IsNullOrEmpty(CellRange))
				return source.GetDataReader(source.Worksheets[0], XlCellRange.Parse(CellRange));
			return source.GetDataReader(source.Worksheets[0]);
		}
		bool Equals(CsvSourceOptions other) {
			return CellRange == other.CellRange
				&& Equals(Culture, other.Culture)
				&& DetectEncoding == other.DetectEncoding
				&& DetectNewlineType == other.DetectNewlineType
				&& DetectValueSeparator == other.DetectValueSeparator
				&& Equals(Encoding, other.Encoding)
				&& NewlineType == other.NewlineType
				&& TextQualifier == other.TextQualifier
				&& TrimBlanks == other.TrimBlanks
				&& ValueSeparator == other.ValueSeparator;
		}
		protected internal override void SaveToXml(XElement options) {
			base.SaveToXml(options);
			if(!string.IsNullOrEmpty(CellRange))
				options.Add(new XAttribute(xml_CellRange, CellRange));
			options.Add(new XAttribute(xml_DetectEncoding, DetectEncoding));
			options.Add(new XAttribute(xml_DetectNewlineType, DetectNewlineType));
			options.Add(new XAttribute(xml_DetectValueSeparator, DetectValueSeparator));
			options.Add(new XAttribute(xml_NewlineType, NewlineType));
			options.Add(new XAttribute(xml_TrimBlanks, TrimBlanks));
			options.Add(new XAttribute(xml_TextQualifier, TextQualifier));
			options.Add(new XAttribute(xml_ValueSeparator, ValueSeparator));
			if (Encoding != null)
				options.Add(new XAttribute(xml_EncodingName, Encoding.WebName));
			if(Culture != null)
				options.Add(new XAttribute(xml_CultureName, Culture.Name));
		}
		protected internal override void LoadFromXml(XElement options) {
			base.LoadFromXml(options);
			CellRange = options.GetAttributeValue(xml_CellRange);
			DetectEncoding = Convert.ToBoolean(options.GetAttributeValue(xml_DetectEncoding));
			DetectNewlineType = Convert.ToBoolean(options.GetAttributeValue(xml_DetectNewlineType));
			DetectValueSeparator = Convert.ToBoolean(options.GetAttributeValue(xml_DetectValueSeparator));
			NewlineType = (CsvNewlineType)Enum.Parse(typeof(CsvNewlineType), options.GetAttributeValue(xml_NewlineType));
			TrimBlanks = Convert.ToBoolean(options.GetAttributeValue(xml_TrimBlanks));
			TextQualifier = Convert.ToChar(options.GetAttributeValue(xml_TextQualifier));
			ValueSeparator = Convert.ToChar(options.GetAttributeValue(xml_ValueSeparator));
			var encodingName = options.GetAttributeValue(xml_EncodingName);
			if(!string.IsNullOrEmpty(encodingName))
				Encoding = Encoding.GetEncoding(encodingName);
			var cultureName = options.GetAttributeValue(xml_CultureName);
			if(!string.IsNullOrEmpty(cultureName))
				Culture = new CultureInfo(cultureName);
		}
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			var other = obj as CsvSourceOptions;
			return other != null && Equals(other);
		}
		public override int GetHashCode() {
			return 0;
		}
	}
}
