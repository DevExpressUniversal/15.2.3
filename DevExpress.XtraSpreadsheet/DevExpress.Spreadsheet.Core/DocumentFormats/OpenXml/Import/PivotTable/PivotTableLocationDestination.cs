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
using System;
using System.Collections.Generic;
using System.Xml;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region PivotTableLocationDestination
	public class PivotTableLocationDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		readonly PivotTableLocation pivotTableLocation;
		readonly Worksheet sheet;
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			return result;
		}
		#endregion
		public PivotTableLocationDestination(SpreadsheetMLBaseImporter importer, PivotTableLocation location, Worksheet sheet)
			: base(importer) {
			this.pivotTableLocation = location;
			this.sheet = sheet;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public PivotTableLocation PivotTableLocation { get { return pivotTableLocation; } }
		public Worksheet Worksheet { get { return sheet; } }
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			ReaderAttributePivotTableLocation(reader);
		}
		void ReaderAttributePivotTableLocation(XmlReader reader) {
			CellRange range = Importer.ReadCellRange(reader, "ref", Worksheet);
			if (range == null)
				Importer.ThrowInvalidFile();
			PivotTableLocation.SetRangeCore(range);
			int? requiredValue = Importer.GetIntegerNullableValue(reader, "firstHeaderRow");
			if (!requiredValue.HasValue)
				Importer.ThrowInvalidFile();
			PivotTableLocation.FirstHeaderRow = requiredValue.Value;
			requiredValue = Importer.GetIntegerNullableValue(reader, "firstDataRow");
			if (!requiredValue.HasValue)
				Importer.ThrowInvalidFile();
			PivotTableLocation.FirstDataRow = requiredValue.Value;
			requiredValue = Importer.GetIntegerNullableValue(reader, "firstDataCol");
			if (!requiredValue.HasValue)
				Importer.ThrowInvalidFile();
			PivotTableLocation.FirstDataColumn = requiredValue.Value;
			int optionalValue = Importer.GetWpSTIntegerValue(reader, "colPageCount", 0);
			PivotTableLocation.ColumnPageCount = optionalValue;
			optionalValue = Importer.GetWpSTIntegerValue(reader, "rowPageCount", 0);
			PivotTableLocation.RowPageCount = optionalValue;
		}
	}
	#endregion
}
