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
using System.Linq;
using System.Text;
using DevExpress.Export.Xl;
using DevExpress.SpreadsheetSource;
using DevExpress.SpreadsheetSource.Implementation;
namespace DevExpress.SpreadsheetSource.Csv {
	using DevExpress.Utils;
	using DevExpress.XtraExport.Csv;
	#region CsvSourceSchemaItem
	public class CsvSourceSchemaItem {
		public CsvSourceSchemaItem(int index, XlVariantValueType valueType) {
			Guard.ArgumentNonNegative(index, "index");
			Index = index;
			ValueType = valueType;
		}
		public int Index { get; private set; }
		public XlVariantValueType ValueType { get; private set; }
	}
	#endregion
	#region CsvSourceSchema
	public class CsvSourceSchema {
		readonly Dictionary<int, XlVariantValueType> items = new Dictionary<int, XlVariantValueType>();
		public XlVariantValueType this[int index] {
			get {
				Guard.ArgumentNonNegative(index, "index");
				XlVariantValueType valueType;
				if(items.TryGetValue(index, out valueType))
					return valueType;
				return XlVariantValueType.None;
			}
		}
		public IEnumerable<CsvSourceSchemaItem> Items {
			get {
				var query = from item in items
							orderby item.Key
							select new CsvSourceSchemaItem(item.Key, item.Value);
				return query;
			}
		}
		public void Add(int index, XlVariantValueType valueType) {
			Guard.ArgumentNonNegative(index, "index");
			items[index] = valueType;
		}
		public void Clear() {
			items.Clear();
		}
	}
	#endregion
	#region ICsvSourceValueConverter
	public interface ICsvSourceValueConverter {
		object Convert(string text, int columnIndex);
	}
	#endregion
	#region CsvSpreadsheetSourceOptions
	public class CsvSpreadsheetSourceOptions : SpreadsheetSourceOptions {
		CultureInfo culture = CultureInfo.InvariantCulture;
		Encoding encoding = Encoding.UTF8;
		readonly CsvSourceSchema schema = new CsvSourceSchema();
		public CsvSpreadsheetSourceOptions()
			: base() {
			ValueSeparator = ',';
			TextQualifier = '"';
			NewlineType = CsvNewlineType.CrLf;
			TrimBlanks = true;
			DetectEncoding = true;
		}
		public char ValueSeparator { get; set; }
		public char TextQualifier { get; set; }
		public CsvNewlineType NewlineType { get; set; }
		public CultureInfo Culture {
			get { return culture; }
			set {
				if(value == null)
					value = CultureInfo.InvariantCulture;
				culture = value;
			}
		}
		public Encoding Encoding {
			get { return encoding; }
			set {
				if(value == null)
					value = Encoding.UTF8;
				encoding = value;
			}
		}
		public bool TrimBlanks { get; set; }
		public bool DetectEncoding { get; set; }
		public bool DetectNewlineType { get; set; }
		public bool DetectValueSeparator { get; set; }
		public CsvSourceSchema Schema { get { return schema; } }
		public ICsvSourceValueConverter ValueConverter { get; set; }
		public static CsvSpreadsheetSourceOptions ConvertToCsvOptions(ISpreadsheetSourceOptions other) {
			CsvSpreadsheetSourceOptions result = other as CsvSpreadsheetSourceOptions;
			if(result == null) {
				result = new CsvSpreadsheetSourceOptions();
				if(other != null) {
					result.Password = other.Password;
					result.SkipEmptyRows = other.SkipEmptyRows;
					result.SkipHiddenRows = other.SkipHiddenRows;
					result.SkipHiddenColumns = other.SkipHiddenColumns;
				}
			}
			return result;
		}
		public void AutoDetect(Stream stream) {
			CsvSpreadsheetSourceOptions options = Clone();
			options.DetectEncoding = true;
			options.DetectNewlineType = true;
			options.DetectValueSeparator = true;
			using(ISpreadsheetSource source = SpreadsheetSourceFactory.CreateSource(stream, SpreadsheetDocumentFormat.Csv, options)) {
				CsvSourceDataReader dataReader = source.GetDataReader(source.Worksheets[0]) as CsvSourceDataReader;
				this.Encoding = dataReader.ActualEncoding;
				this.NewlineType = dataReader.ActualNewlineType;
				this.ValueSeparator = dataReader.ActualValueSeparator;
			}
		}
		CsvSpreadsheetSourceOptions Clone() {
			CsvSpreadsheetSourceOptions result = new CsvSpreadsheetSourceOptions();
			result.Culture = Culture.Clone() as CultureInfo;
			result.DetectEncoding = DetectEncoding;
			result.DetectNewlineType = DetectNewlineType;
			result.DetectValueSeparator = DetectValueSeparator;
			result.Encoding = Encoding.Clone() as Encoding;
			result.NewlineType = NewlineType;
			result.Password = Password;
			IEnumerable<CsvSourceSchemaItem> schemaItems = Schema.Items;
			foreach(CsvSourceSchemaItem item in schemaItems)
				result.Schema.Add(item.Index, item.ValueType);
			result.SkipEmptyRows = SkipEmptyRows;
			result.SkipHiddenColumns = SkipHiddenColumns;
			result.SkipHiddenRows = SkipHiddenRows;
			result.TextQualifier = TextQualifier;
			result.TrimBlanks = TrimBlanks;
			result.ValueConverter = ValueConverter;
			result.ValueSeparator = ValueSeparator;
			return result;
		}
	}
	#endregion
}
