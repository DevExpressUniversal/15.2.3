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
using System.Xml;
using DevExpress.Utils;
using DevExpress.Office;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model.External;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region ExternalWorksheetsDestination
	public class ExternalWorksheetsDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("sheetData", OnExternalWorksheet);
			return result;
		}
		static ExternalWorksheetsDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (ExternalWorksheetsDestination)importer.PeekDestination();
		}
		readonly ExternalWorksheetCollection sheets;
		public ExternalWorksheetsDestination(SpreadsheetMLBaseImporter importer, ExternalWorksheetCollection sheets)
			: base(importer) {
			Guard.ArgumentNotNull(sheets, "sheets");
			this.sheets = sheets;
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public ExternalWorksheetCollection Sheets { get { return sheets; } }
		static Destination OnExternalWorksheet(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ExternalWorksheetsDestination thisImporter = GetThis(importer);
			return new ExternalWorksheetDestination(importer, thisImporter.Sheets);
		}
	}
	#endregion
	#region ExternalWorksheetDestination
	public class ExternalWorksheetDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("row", OnExternalRow);
			return result;
		}
		static ExternalWorksheetDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (ExternalWorksheetDestination)importer.PeekDestination();
		}
		ExternalWorksheet sheet;
		readonly ExternalWorksheetCollection sheets;
		public ExternalWorksheetDestination(SpreadsheetMLBaseImporter importer, ExternalWorksheetCollection sheets)
			: base(importer) {
			Guard.ArgumentNotNull(sheets, "sheets");
			this.sheets = sheets;
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public ExternalWorksheet Sheet { get { return sheet; } }
		public override void ProcessElementOpen(XmlReader reader) {
			int id = Importer.GetIntegerValue(reader, "sheetId", Int32.MinValue);
			if (id == Int32.MinValue)
				Importer.ThrowInvalidFile();
			this.sheet = sheets[id];
			if (sheet == null)
				Importer.ThrowInvalidFile();
			bool refreshError = Importer.GetWpSTOnOffValue(reader, "refreshError", false);
			if (refreshError)
				sheet.RefreshFailed = true;
		}
		static Destination OnExternalRow(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ExternalRowDestination(importer, GetThis(importer).Sheet.Rows);
		}
	}
	#endregion
	#region ExternalSheetNamesDestination
	public class ExternalSheetNamesDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("sheetName", OnExternalSheetName);
			return result;
		}
		static ExternalSheetNamesDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (ExternalSheetNamesDestination)importer.PeekDestination();
		}
		readonly ExternalWorksheetCollection sheets;
		public ExternalSheetNamesDestination(SpreadsheetMLBaseImporter importer, ExternalWorksheetCollection sheets)
			: base(importer) {
			Guard.ArgumentNotNull(sheets, "sheets");
			this.sheets = sheets;
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		static Destination OnExternalSheetName(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ExternalSheetNameDestination(importer, GetThis(importer).sheets);
		}
	}
	#endregion
	#region ExternalSheetNameDestination
	public class ExternalSheetNameDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		ExternalWorksheetCollection sheets;
		public ExternalSheetNameDestination(SpreadsheetMLBaseImporter importer, ExternalWorksheetCollection sheets)
			: base(importer) {
			Guard.ArgumentNotNull(sheets, "sheets");
			this.sheets = sheets;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string name = Importer.ReadAttribute(reader, "val");
			if (String.IsNullOrEmpty(name))
				Importer.ThrowInvalidFile();
			sheets.Add(new ExternalWorksheet(sheets.Workbook, name));
		}
	}
	#endregion
}
