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
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Internal;
using System.Xml;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region SheetsDestination
	public class SheetsDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("sheet", OnSheet);
			return result;
		}
		public SheetsDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		static Destination OnSheet(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new SheetDestination(importer);
		}
		public override void ProcessElementClose(XmlReader reader) {
			if (Importer.DocumentModel.Sheets.Count == 0)
				Importer.ThrowInvalidFile("Sheets count is zero");
		}
	}
	#endregion
	#region SheetDestination
	public class SheetDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		public SheetDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string relationId = Importer.ReadRelationAttribute(reader, "id");
			OpenXmlImporter importer = (OpenXmlImporter)Importer;
			int sheetId = Importer.GetWpSTIntegerValue(reader, "sheetId");
			SheetVisibleState visibility = Importer.GetWpEnumValue<SheetVisibleState>(reader, "state", OpenXmlImporter.visibilityTypeTable, SheetVisibleState.Visible);
			string name = Importer.ReadAttribute(reader, "name");
			if (!String.IsNullOrEmpty(name)) {
				Worksheet sheet = Importer.DocumentModel.CreateWorksheet(name);
				sheet.VisibleState = visibility;
				Importer.DocumentModel.Sheets.Add(sheet);
				importer.SheetIdsTable.Add(sheetId, sheet);
			}
			importer.SheetsRelationIds.Add(relationId);
		}
	}
	#endregion
}
