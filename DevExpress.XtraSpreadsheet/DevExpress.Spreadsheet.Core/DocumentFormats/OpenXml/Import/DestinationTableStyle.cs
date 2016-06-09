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
using System.Xml;
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region TableStylesDestination
	public class TableStylesDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static Members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("tableStyle", OnTableStyle);
			return result;
		}
		static Destination OnTableStyle(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new TableStyleDestination(importer);
		}
		#endregion
		TableStyleName defaultTableStyleName;
		TableStyleName defaultPivotStyleName;
		public TableStylesDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		DocumentModel DocumentModel { get { return Importer.DocumentModel; } }
		public override void ProcessElementOpen(XmlReader reader) {
			defaultTableStyleName = ReadDefaultStyleName(reader, true);
			defaultPivotStyleName = ReadDefaultStyleName(reader, false);
		}
		TableStyleName ReadDefaultStyleName(XmlReader reader, bool isDefaultTableStyle) {
			string value = Importer.ReadAttribute(reader, isDefaultTableStyle ? "defaultTableStyle" : "defaultPivotStyle");
			if (String.IsNullOrEmpty(value))
				return TableStyleName.DefaultStyleName;
			return isDefaultTableStyle ? TableStyleName.CreateTableName(value) : TableStyleName.CreatePivotName(value);
		}
		public override void ProcessElementClose(XmlReader reader) {
			TableStyleCollection tableStyles = DocumentModel.StyleSheet.TableStyles;
			if (defaultTableStyleName.IsPredefined)
				tableStyles.Add(TableStyle.CreateTablePredefinedStyle(DocumentModel, defaultTableStyleName));
			tableStyles.SetDefaultTableStyleName(defaultTableStyleName.Name);
			if (defaultPivotStyleName.IsPredefined)
				tableStyles.Add(TableStyle.CreatePivotPredefinedStyle(DocumentModel, defaultPivotStyleName));
			tableStyles.SetDefaultPivotStyleName(defaultPivotStyleName.Name);
		}
	}
	#endregion
	#region TableStyleDestination
	public class TableStyleDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static Members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("tableStyleElement", OnTableStyleElement);
			return result;
		}
		static TableStyleDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (TableStyleDestination)importer.PeekDestination();
		}
		static Destination OnTableStyleElement(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new TableStyleElementDestination(importer, GetThis(importer).Info);
		}
		#endregion
		readonly ImportExportTableStyleInfo info;
		public TableStyleDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
			info = new ImportExportTableStyleInfo();
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		protected internal ImportExportTableStyleInfo Info { get { return info; } }
		public override void ProcessElementOpen(XmlReader reader) {
			info.Name = Importer.ReadAttribute(reader, "name");
			info.IsTable = Importer.GetWpSTOnOffValue(reader, "table", true);
			info.IsPivot = Importer.GetWpSTOnOffValue(reader, "pivot", true);
		}
		public override void ProcessElementClose(XmlReader reader) {
			info.RegisterStyle(Importer.DocumentModel, Importer.StyleSheet.DifferentialFormatTable);
		}
	}
	#endregion
	#region TableStyleElementDestination
	public class TableStyleElementDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		#region Static Members
		static Dictionary<string, int> importTableStyleElementTypeTable = GetImportTableStyleElementTypeTable();
		static Dictionary<string, int> GetImportTableStyleElementTypeTable() {
			Dictionary<string, int> result = new Dictionary<string, int>();
			result.Add("wholeTable", TableStyle.WholeTableIndex);
			result.Add("headerRow", TableStyle.HeaderRowIndex);
			result.Add("totalRow", TableStyle.TotalRowIndex);
			result.Add("firstColumn", TableStyle.FirstColumnIndex);
			result.Add("lastColumn", TableStyle.LastColumnIndex);
			result.Add("firstRowStripe", TableStyle.FirstRowStripeIndex);
			result.Add("secondRowStripe", TableStyle.SecondRowStripeIndex);
			result.Add("firstColumnStripe", TableStyle.FirstColumnStripeIndex);
			result.Add("secondColumnStripe", TableStyle.SecondColumnStripeIndex);
			result.Add("firstHeaderCell", TableStyle.FirstHeaderCellIndex);
			result.Add("lastHeaderCell", TableStyle.LastHeaderCellIndex);
			result.Add("firstTotalCell", TableStyle.FirstTotalCellIndex);
			result.Add("lastTotalCell", TableStyle.LastTotalCellIndex);
			result.Add("firstSubtotalColumn", TableStyle.FirstSubtotalColumnIndex);
			result.Add("secondSubtotalColumn", TableStyle.SecondSubtotalColumnIndex);
			result.Add("thirdSubtotalColumn", TableStyle.ThirdSubtotalColumnIndex);
			result.Add("firstSubtotalRow", TableStyle.FirstSubtotalRowIndex);
			result.Add("secondSubtotalRow", TableStyle.SecondSubtotalRowIndex);
			result.Add("thirdSubtotalRow", TableStyle.ThirdSubtotalRowIndex);
			result.Add("blankRow", TableStyle.BlankRowIndex);
			result.Add("firstColumnSubheading", TableStyle.FirstColumnSubheadingIndex);
			result.Add("secondColumnSubheading", TableStyle.SecondColumnSubheadingIndex);
			result.Add("thirdColumnSubheading", TableStyle.ThirdColumnSubheadingIndex);
			result.Add("firstRowSubheading", TableStyle.FirstRowSubheadingIndex);
			result.Add("secondRowSubheading", TableStyle.SecondRowSubheadingIndex);
			result.Add("thirdRowSubheading", TableStyle.ThirdRowSubheadingIndex);
			result.Add("pageFieldLabels", TableStyle.PageFieldLabelsIndex);
			result.Add("pageFieldValues", TableStyle.PageFieldValuesIndex);
			return result;
		}
		#endregion
		ImportExportTableStyleInfo info;
		public TableStyleElementDestination(SpreadsheetMLBaseImporter importer, ImportExportTableStyleInfo info)
			: base(importer) {
			this.info = info;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string type = Importer.ReadAttribute(reader, "type");
			if (String.IsNullOrEmpty(type) || !importTableStyleElementTypeTable.ContainsKey(type))
				Importer.ThrowInvalidFile();
			int elementIndex = importTableStyleElementTypeTable[type];
			int? dxfId = Importer.GetIntegerNullableValue(reader, "dxfId");
			int? stripeSize = Importer.GetIntegerNullableValue(reader, "size");
			info.RegisterStyleElementFormat(elementIndex, dxfId, stripeSize);
		}
	}
	#endregion
}
