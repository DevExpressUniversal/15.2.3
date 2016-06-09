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
	#region ExternalWorkbookDestination
	public class ExternalWorkbookDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("sheetNames", OnExternalSheetNames);
			result.Add("sheetDataSet", OnExternalWorksheets);
			result.Add("definedNames", OnExternalDefinedNames);
			return result;
		}
		static ExternalWorkbookDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (ExternalWorkbookDestination)importer.PeekDestination();
		}
		ExternalWorkbook workbook;
		public ExternalWorkbookDestination(SpreadsheetMLBaseImporter importer, ExternalWorkbook workbook)
			: base(importer) {
			Guard.ArgumentNotNull(workbook, "workbook");
			this.workbook = workbook;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public ExternalWorkbook Workbook { get { return workbook; } }
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			Importer.CurrentExternalFileRelationId = Importer.ReadRelationAttribute(reader, "id");
		}
		static Destination OnExternalSheetNames(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ExternalSheetNamesDestination(importer, GetThis(importer).Workbook.Sheets);
		}
		static Destination OnExternalWorksheets(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ExternalWorkbookDestination thisImporter = GetThis(importer);
			return new ExternalWorksheetsDestination(importer, thisImporter.Workbook.Sheets);
		}
		static Destination OnExternalDefinedNames(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ExternalWorkbookDestination thisImporter = GetThis(importer);
			return new ExternalDefinedNamesDestination(importer, thisImporter.Workbook);
		}
	}
	#endregion
}
