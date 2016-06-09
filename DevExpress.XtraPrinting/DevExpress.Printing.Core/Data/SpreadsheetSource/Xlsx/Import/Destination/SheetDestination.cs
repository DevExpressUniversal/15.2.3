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
using DevExpress.XtraExport.Xlsx;
using System;
using System.Xml;
namespace DevExpress.SpreadsheetSource.Xlsx.Import {
	#region SheetsDestination
	public class SheetsDestination : ElementDestination<XlsxSpreadsheetSourceImporter> {
		static readonly ElementHandlerTable<XlsxSpreadsheetSourceImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<XlsxSpreadsheetSourceImporter> CreateElementHandlerTable() {
			ElementHandlerTable<XlsxSpreadsheetSourceImporter> result = new ElementHandlerTable<XlsxSpreadsheetSourceImporter>();
			result.Add("sheet", OnSheet);
			return result;
		}
		public SheetsDestination(XlsxSpreadsheetSourceImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable<XlsxSpreadsheetSourceImporter> ElementHandlerTable { get { return handlerTable; } }
		static Destination OnSheet(XlsxSpreadsheetSourceImporter importer, XmlReader reader) {
			return new SheetDestination(importer);
		}
		public override void ProcessElementClose(XmlReader reader) {
			if (Importer.Source.Worksheets.Count == 0)
				Importer.ThrowInvalidFile("Sheets count is zero");
		}
	}
	#endregion
	#region SheetDestination
	public class SheetDestination : LeafElementDestination<XlsxSpreadsheetSourceImporter> {
		public SheetDestination(XlsxSpreadsheetSourceImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string relationId = Importer.ReadAttribute(reader, "id", XlsxPackageBuilder.RelsNamespace);
			XlSheetVisibleState visibility = Importer.GetWpEnumValue<XlSheetVisibleState>(reader, "state", XlsxDataAwareExporter.VisibilityTypeTable, XlSheetVisibleState.Visible);
			string name = Importer.ReadAttribute(reader, "name");
			if (!String.IsNullOrEmpty(name))
				Importer.Source.InnerWorksheets.Add(new XlsxWorksheet(name, visibility, relationId));
		}
	}
	#endregion
}
