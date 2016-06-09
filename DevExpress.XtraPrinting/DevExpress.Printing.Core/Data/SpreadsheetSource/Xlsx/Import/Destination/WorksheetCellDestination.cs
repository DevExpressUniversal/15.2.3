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

using DevExpress.Export.Xl;
using DevExpress.Office;
using DevExpress.SpreadsheetSource.Implementation;
using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
namespace DevExpress.SpreadsheetSource.Xlsx.Import {
	#region WorksheetCellDestination
	public class WorksheetCellDestination : ElementDestination<XlsxSpreadsheetSourceImporter> {
		#region CellDataType
		enum CellDataType {
			Bool,
			Error,
			InlineString,
			Number,
			SharedString,
			FormulaString
		}
		static Dictionary<string, CellDataType> cellDataTypeTable = CreateCellDataTypeTable();
		static Dictionary<string, CellDataType> CreateCellDataTypeTable() {
			Dictionary<string, CellDataType> result = new Dictionary<string, CellDataType>();
			result.Add("b", CellDataType.Bool);
			result.Add("e", CellDataType.Error);
			result.Add("inlineStr", CellDataType.InlineString);
			result.Add("n", CellDataType.Number);
			result.Add("s", CellDataType.SharedString);
			result.Add("str", CellDataType.FormulaString);
			return result;
		}
		#endregion
		#region Handler Table
		static readonly ElementHandlerTable<XlsxSpreadsheetSourceImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<XlsxSpreadsheetSourceImporter> CreateElementHandlerTable() {
			ElementHandlerTable<XlsxSpreadsheetSourceImporter> result = new ElementHandlerTable<XlsxSpreadsheetSourceImporter>();
			result.Add("is", OnInlineString);
			result.Add("v", OnValue);
			return result;
		}
		#endregion
		[ThreadStatic]
		static WorksheetCellDestination instance;
		CellDataType cellDataType;
		int formatIndex;
		public WorksheetCellDestination(XlsxSpreadsheetSourceImporter importer)
			: base(importer) {
			Reset();
		}
		protected internal override ElementHandlerTable<XlsxSpreadsheetSourceImporter> ElementHandlerTable { get { return handlerTable; } }
		public string Value { get; set; }
		static WorksheetCellDestination GetThis(XlsxSpreadsheetSourceImporter importer) {
			return (WorksheetCellDestination)importer.PeekDestination();
		}
		static Destination OnInlineString(XlsxSpreadsheetSourceImporter importer, XmlReader reader) {
			return new InlineStringDestination(importer, GetThis(importer));
		}
		static Destination OnValue(XlsxSpreadsheetSourceImporter importer, XmlReader reader) {
			return CellValueDestination.GetInstance(importer, GetThis(importer));
		}
		public override void ProcessElementOpen(XmlReader reader) {
			this.cellDataType = Importer.GetWpEnumValue<CellDataType>(reader, "t", cellDataTypeTable, CellDataType.Number);
			this.formatIndex = Importer.GetWpSTIntegerValue(reader, "s", Int32.MinValue);
		}
		public override void ProcessElementClose(XmlReader reader) {
			if (String.IsNullOrEmpty(Value))
				return;
			int index = Importer.GetCurrentCellIndex();
			XlsxSourceDataReader dataReader = Importer.Source.DataReader;
			if (!dataReader.CanAddCell(index))
				return;
			dataReader.AddCell(index, GetVariantValue(index), formatIndex);
		}
		XlVariantValue GetVariantValue(int columnIndex) {
			switch (cellDataType) {
				case CellDataType.Bool:
					return GetBooleanValue();
				case CellDataType.Error:
					return GetErrorValue();
				case CellDataType.InlineString:
					return GetInlineStringValue();
				case CellDataType.Number:
					return GetNumericValue(columnIndex);
				case CellDataType.SharedString:
					return GetSharedStringValue();
				case CellDataType.FormulaString:
					return GetFormulaStringValue();
				default:
					throw new InvalidOperationException("Invalid cell data type");
			}
		}
		XlVariantValue GetBooleanValue() {
			return Importer.ConvertToBool(Value);
		}
		XlVariantValue GetErrorValue() {
			IXlCellError error;
			if (XlCellErrorFactory.TryCreateErrorByInvariantName(Value, out error))
				return error.Value;
			return XlVariantValue.Empty;
		}
		XlVariantValue GetInlineStringValue() {
			return GetStringValue();
		}
		XlVariantValue GetNumericValue(int columnIndex) {
			double doubleValue;
			if (Double.TryParse(Value, NumberStyles.Float, CultureInfo.InvariantCulture, out doubleValue))
				return Importer.Source.DataReader.GetDateTimeOrNumericValue(doubleValue, formatIndex, columnIndex);
			return XlVariantValue.Empty;
		}
		XlVariantValue GetSharedStringValue() {
			XlVariantValue result = new XlVariantValue();
			int index;
			if (!Int32.TryParse(Value, out index))
				result.TextValue = String.Empty;
			else
				result.TextValue = Importer.Source.SharedStrings[index];
			return result;
		}
		XlVariantValue GetFormulaStringValue() {
			return GetStringValue();
		}
		XlVariantValue GetStringValue() {
			XlVariantValue result = new XlVariantValue();
			result.TextValue = Importer.DecodeXmlChars(Value);
			return result;
		}
		public static WorksheetCellDestination GetInstance(XlsxSpreadsheetSourceImporter importer) {
			if (instance == null || instance.Importer != importer)
				instance = new WorksheetCellDestination(importer);
			else {
				instance.Reset();
			}
			return instance;
		}
		void Reset() {
			cellDataType = CellDataType.Number;
			formatIndex = 0;
			Value = null;
		}
		public static void ClearInstance() {
			instance = null;
			CellValueDestination.ClearInstance();
		}
	}
	#endregion
	#region CellValueDestination
	public class CellValueDestination : LeafElementDestination<XlsxSpreadsheetSourceImporter> {
		#region Fields
		WorksheetCellDestination cellDestination;
		[ThreadStatic]
		static CellValueDestination instance;
		#endregion
		public CellValueDestination(XlsxSpreadsheetSourceImporter importer, WorksheetCellDestination cellDestination)
			: base(importer) {
			Guard.ArgumentNotNull(cellDestination, "cellDestination");
			this.cellDestination = cellDestination;
		}
		public override bool ProcessText(XmlReader reader) {
			cellDestination.Value = reader.Value;
			return true;
		}
		public static CellValueDestination GetInstance(XlsxSpreadsheetSourceImporter importer, WorksheetCellDestination cellDestination) {
			if (instance == null || instance.Importer != importer)
				instance = new CellValueDestination(importer, cellDestination);
			else
				instance.cellDestination = cellDestination;
			return instance;
		}
		public static void ClearInstance() {
			if (instance != null)
				instance.cellDestination = null;
			instance = null;
		}
	}
	#endregion
	#region InlineStringDestination
	public class InlineStringDestination : SharedStringDestination {
		readonly WorksheetCellDestination cellDestination;
		public InlineStringDestination(XlsxSpreadsheetSourceImporter importer, WorksheetCellDestination cellDestination)
			: base(importer) {
			Guard.ArgumentNotNull(cellDestination, "cellDestination");
			this.cellDestination = cellDestination;
		}
		protected override void ApplyText(string item) {
			cellDestination.Value = item;
		}
	}
	#endregion
}
