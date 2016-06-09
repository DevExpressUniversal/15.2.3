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

#if OPENDOCUMENT
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Services;
using System.Xml;
namespace DevExpress.XtraSpreadsheet.Import.OpenDocument {
	public class TableDestination : ElementDestination {
		#region Static
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("named-expressions", OnNamedExpressions);
			result.Add("table-column", OnColumn);
			result.Add("table-column-group", OnColumnGroup);
			result.Add("table-row", OnRow);
			result.Add("table-row-group", OnRowGroup);
			return result;
		}
		#endregion
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public TableDestination(OpenDocumentWorkbookImporter importer)
			: base(importer) {
			importer.ColumnOutlineLevel = 0;
			importer.RowOutlineLevel = 0;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string worksheetName = Importer.ReadAttribute(reader, "table:name");
			Worksheet worksheet = Importer.CreateWorksheet(worksheetName);
			Importer.DocumentModel.Sheets.Add(worksheet);
			string styleName = Importer.ReadAttribute(reader, "table:style-name");
			Importer.ApplyTableFormatOnTable(worksheet, styleName);
		}
		#region Handlers
		static Destination OnNamedExpressions(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			return new NamedExpressionsDestination(importer.CurrentSheet, importer);
		}
		static Destination OnColumn(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			return new ColumnDestination(importer);
		}
		static Destination OnColumnGroup(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			return new TableColumnGroupDestination(importer);
		}
		static Destination OnRow(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			return new RowDestination(importer);
		}
		static Destination OnRowGroup(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			return new TableRowGroupDestination(importer);
		}
		#endregion
	}
}
#endif
