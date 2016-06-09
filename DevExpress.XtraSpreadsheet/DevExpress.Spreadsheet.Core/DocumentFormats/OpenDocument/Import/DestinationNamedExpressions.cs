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
using System.Xml;
namespace DevExpress.XtraSpreadsheet.Import.OpenDocument {
	#region NamedExpressiosnDestination
	public class NamedExpressionsDestination : ElementDestination {
		#region Static
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("named-range", OnNamedRange);
			result.Add("named-expression", OnNamedExpression);
			return result;
		}
		static NamedExpressionsDestination GetThis(OpenDocumentWorkbookImporter importer) {
			return (NamedExpressionsDestination)importer.PeekDestination();
		}
		#endregion
		DefinedNameCollection definedNames;
		int scopedsheetId;
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public NamedExpressionsDestination(OpenDocumentWorkbookImporter importer)
			: base(importer) {
			this.definedNames = importer.DocumentModel.DefinedNames;
			this.scopedsheetId = -1;
		}
		public NamedExpressionsDestination(Worksheet sheet, OpenDocumentWorkbookImporter importer)
			: base(importer) {
			this.definedNames = sheet.DefinedNames;
			this.scopedsheetId = sheet.SheetId;
		}
		#region Handlers
		static Destination OnNamedRange(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			NamedExpressionsDestination destination = GetThis(importer);
			return new NamedRangeDestination(destination.definedNames, destination.scopedsheetId, importer);
		}
		static Destination OnNamedExpression(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			NamedExpressionsDestination destination = GetThis(importer);
			return new NamedExpressionDestination(destination.definedNames, destination.scopedsheetId, importer);
		}
		#endregion
	}
	#endregion
	#region Base
	public abstract class DefinedNameDestinationBase : LeafElementDestination {
		DefinedNameCollection definedNames;
		int scopedsheetId;
		protected DefinedNameDestinationBase(DefinedNameCollection definedNames, int scopedsheetId, OpenDocumentWorkbookImporter importer)
			: base(importer) {
			this.definedNames = definedNames;
			this.scopedsheetId = scopedsheetId;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			CellRange baseCell = Importer.GetCellRangeByCellRangeAddress(reader.GetAttribute("table:base-cell-address"));
			string reference = Importer.NormalizeOdsFormula(Getreference(reader), baseCell.Worksheet.Name);
			DefinedName defName = new DefinedName(Importer.DocumentModel, reader.GetAttribute("table:name"), reference, scopedsheetId);
			definedNames.AddWithoutHistoryAndNotifications(defName);
		}
		protected abstract string Getreference(XmlReader reader);
	}
	#endregion
	#region NamedRangeDestination
	public class NamedRangeDestination : DefinedNameDestinationBase {
		public NamedRangeDestination(DefinedNameCollection definedNames, int scopedsheetId, OpenDocumentWorkbookImporter importer)
			: base(definedNames, scopedsheetId, importer) {
		}
		protected override string Getreference(XmlReader reader) {
			return reader.GetAttribute("table:cell-range-address");
		}
	}
	#endregion
	#region NamedExpressionDestination
	public class NamedExpressionDestination : DefinedNameDestinationBase {
		public NamedExpressionDestination(DefinedNameCollection definedNames, int scopedsheetId, OpenDocumentWorkbookImporter importer)
			: base(definedNames, scopedsheetId, importer) {
		}
		protected override string Getreference(XmlReader reader) {
			return reader.GetAttribute("table:expression");
		}
	}
	#endregion
}
#endif
