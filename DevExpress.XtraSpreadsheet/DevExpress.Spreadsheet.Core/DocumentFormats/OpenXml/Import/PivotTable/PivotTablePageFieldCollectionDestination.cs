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

using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using System.Xml;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region PivotTablePageFieldCollectionDestination
	public class PivotTablePageFieldCollectionDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		readonly PivotTable pivotTable;
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("pageField", OnPageField);
			return result;
		}
		public PivotTablePageFieldCollectionDestination(SpreadsheetMLBaseImporter importer, PivotTable pivotTable)
			: base(importer) {
			this.pivotTable = pivotTable;
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		static PivotTablePageFieldCollectionDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotTablePageFieldCollectionDestination)importer.PeekDestination();
		}
		static Destination OnPageField(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTablePageFieldCollectionDestination self = GetThis(importer);
			return new PivotTablePageFieldDestination(importer, self.pivotTable);
		}
	}
	#endregion
	#region PivotTablePageFieldDestination
	public class PivotTablePageFieldDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		readonly PivotTable pivotTable;
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			return result;
		}
		public PivotTablePageFieldDestination(SpreadsheetMLBaseImporter importer, PivotTable pivotTable)
			: base(importer) {
			this.pivotTable = pivotTable;
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			PivotPageField pivotPageFild = new PivotPageField(pivotTable, Importer.GetWpSTIntegerValue(reader, "fld"));
			pivotTable.PageFields.AddCore(pivotPageFild);
			pivotPageFild.SetItemIndexCore(Importer.GetIntegerValue(reader, "item", -1));
			pivotPageFild.SetHierarchyIndexCore(Importer.GetWpSTIntegerValue(reader, "hier"));
			pivotPageFild.SetHierarchyUniqueNameCore(Importer.GetWpSTXString(reader, "name"));
			pivotPageFild.SetHierarchyDisplayNameCore(Importer.GetWpSTXString(reader, "cap"));
		}
	}
	#endregion
}
