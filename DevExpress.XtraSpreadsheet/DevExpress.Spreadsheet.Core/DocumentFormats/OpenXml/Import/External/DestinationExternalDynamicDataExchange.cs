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
using DevExpress.XtraSpreadsheet.Model.External;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.Utils;
using System.Xml;
using DevExpress.Office;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Export.OpenXml;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region ExternalDdeConnectionDestination
	public class ExternalDdeConnectionDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("ddeItems", OnExternalDdeItemCollection);
			return result;
		}
		static ExternalDdeConnectionDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (ExternalDdeConnectionDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly ExternalLink link;
		#endregion
		public ExternalDdeConnectionDestination(SpreadsheetMLBaseImporter importer, ExternalLink link)
			: base(importer) {
			Guard.ArgumentNotNull(link, "link");
			this.link = link;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			string ddeService = Importer.ReadAttribute(reader, "ddeService");
			string ddeTopic = Importer.ReadAttribute(reader, "ddeTopic");
			if (String.IsNullOrEmpty(ddeService) || String.IsNullOrEmpty(ddeTopic))
				Importer.ThrowInvalidFile();
			DdeExternalWorkbook workbook = link.Workbook as DdeExternalWorkbook;
			workbook.DdeServiceName = ddeService;
			workbook.DdeServerTopic = ddeTopic;
		}
		static Destination OnExternalDdeItemCollection(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ExternalDdeItemCollectionDestination(importer, GetThis(importer).link.Workbook as DdeExternalWorkbook);
		}
	}
	#endregion
	#region ExternalDdeItemCollectionDestination
	public class ExternalDdeItemCollectionDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("ddeItem", OnExternalDdeItemDefinition);
			return result;
		}
		static ExternalDdeItemCollectionDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (ExternalDdeItemCollectionDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly DdeExternalWorkbook ddeWorkbook;
		#endregion
		public ExternalDdeItemCollectionDestination(SpreadsheetMLBaseImporter importer, DdeExternalWorkbook ddeWorkbook)
			: base(importer) {
			Guard.ArgumentNotNull(ddeWorkbook, "ddeWorkbook");
			this.ddeWorkbook = ddeWorkbook;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		static Destination OnExternalDdeItemDefinition(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ExternalDdeItemDefinitionDestination(importer, GetThis(importer).ddeWorkbook);
		}
	}
	#endregion
	#region ExternalDdeItemDefinitionDestination
	public class ExternalDdeItemDefinitionDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("values", OnExternalDdeLinkValues);
			return result;
		}
		static ExternalDdeItemDefinitionDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (ExternalDdeItemDefinitionDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly ExternalDdeItemDefinition ddeItem;
		readonly DdeExternalWorkbook ddeWorkbook;
		#endregion
		public ExternalDdeItemDefinitionDestination(SpreadsheetMLBaseImporter importer, DdeExternalWorkbook ddeWorkbook)
			: base(importer) {
			Guard.ArgumentNotNull(ddeWorkbook, "ddeWorkbook");
			this.ddeWorkbook = ddeWorkbook;
			this.ddeItem = new ExternalDdeItemDefinition();
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			string ddeName = Importer.ReadAttribute(reader, "name");
			ddeItem.DdeName = String.IsNullOrEmpty(ddeName) ? "0" : ddeName;
			ddeItem.IsUsesOLE = Importer.GetOnOffValue(reader, "ole", false);
			ddeItem.Advise = Importer.GetOnOffValue(reader, "advise", false);
			ddeItem.IsDataImage = Importer.GetOnOffValue(reader, "preferPic", false);
		}
		public override void ProcessElementClose(XmlReader reader) {
			DdeExternalWorksheet sheet = new DdeExternalWorksheet(ddeWorkbook, ddeItem.DdeName);
			sheet.IsUsesOLE = ddeItem.IsUsesOLE;
			sheet.Advise = ddeItem.Advise;
			sheet.IsDataImage = ddeItem.IsDataImage;
			ddeWorkbook.Sheets.Add(sheet);
			int columns = ddeItem.DdeLinkValues.Columns;
			sheet.RowCount = ddeItem.DdeLinkValues.Rows;
			sheet.ColumnCount = columns;
			int count = ddeItem.DdeLinkValues.Count;
			if(count > 0) {
				for(int i = 0; i < count; i++) {
					Model.VariantValue cellValue = ddeItem.DdeLinkValues[i].GetVariantValue(Importer.DocumentModel.DataContext);
					if(cellValue != Model.VariantValue.Empty) {
						int rowIndex = i / columns;
						int columnIndex = i % columns;
						ExternalCell cell = sheet.Rows[rowIndex].Cells[columnIndex];
						cell.Value = cellValue;
					}
				}
			}
		}
		static Destination OnExternalDdeLinkValues(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ExternalDdeLinkValuesDestination(importer, GetThis(importer).ddeItem.DdeLinkValues);
		}
	}
	#endregion
	#region ExternalDdeLinkValuesDestination
	public class ExternalDdeLinkValuesDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("value", OnExternalDdeLinkValue);
			return result;
		}
		static ExternalDdeLinkValuesDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (ExternalDdeLinkValuesDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly ExternalDdeLinkValueCollection ddeLinkValues;
		#endregion
		public ExternalDdeLinkValuesDestination(SpreadsheetMLBaseImporter importer, ExternalDdeLinkValueCollection ddeLinkValues)
			: base(importer) {
			Guard.ArgumentNotNull(ddeLinkValues, "ddeLinkValues");
			this.ddeLinkValues = ddeLinkValues;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			ddeLinkValues.Columns = Importer.GetIntegerValue(reader, "cols", 1);
			ddeLinkValues.Rows = Importer.GetIntegerValue(reader, "rows", 1);
		}
		static Destination OnExternalDdeLinkValue(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ExternalDdeLinkValueDestination(importer, GetThis(importer).ddeLinkValues);
		}
	}
	#endregion
	#region ExternalDdeLinkValueDestination
	public class ExternalDdeLinkValueDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("val", OnExternalDdeLinkValueCore);
			return result;
		}
		static Dictionary<DdeValueType, string> ddeValueTypeTable = OpenXmlExporter.DdeValueTypeTable;
		static ExternalDdeLinkValueDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (ExternalDdeLinkValueDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly ExternalDdeLinkValueCollection ddeLinkValues;
		readonly ExternalDdeLinkValue ddeLinkValue;
		#endregion
		public ExternalDdeLinkValueDestination(SpreadsheetMLBaseImporter importer, ExternalDdeLinkValueCollection ddeLinkValues)
			: base(importer) {
			Guard.ArgumentNotNull(ddeLinkValues, "ddeLinkValues");
			this.ddeLinkValues = ddeLinkValues;
			this.ddeLinkValue = new ExternalDdeLinkValue();
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			ddeLinkValue.Type = Importer.GetWpEnumValue<DdeValueType>(reader, "t", ddeValueTypeTable, DdeValueType.RealNumber);
			ddeLinkValues.Add(ddeLinkValue);
		}
		static Destination OnExternalDdeLinkValueCore(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ExternalDdeLinkValueCoreDestination(importer, GetThis(importer).ddeLinkValue);
		}
	}
	#endregion
	#region ExternalDdeLinkValueCoreDestination
	public class ExternalDdeLinkValueCoreDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		#region Fields
		readonly ExternalDdeLinkValue ddeLinkValue;
		#endregion
		public ExternalDdeLinkValueCoreDestination(SpreadsheetMLBaseImporter importer, ExternalDdeLinkValue ddeLinkValue)
			: base(importer) {
			Guard.ArgumentNotNull(ddeLinkValue, "ddeLinkValue");
			this.ddeLinkValue = ddeLinkValue;
		}
		public override bool ProcessText(XmlReader reader) {
			string value = reader.Value;
			if (String.IsNullOrEmpty(value))
				Importer.ThrowInvalidFile();
			ddeLinkValue.Value = value;
			return true;
		}
	}
	#endregion
}
