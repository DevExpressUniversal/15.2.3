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
	#region ExternalRowDestination
	public class ExternalRowDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("cell", OnExternalCell);
			return result;
		}
		static ExternalRowDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (ExternalRowDestination)importer.PeekDestination();
		}
		ExternalRowCollection rows;
		ExternalRow row;
		public ExternalRowDestination(SpreadsheetMLBaseImporter importer, ExternalRowCollection rows)
			: base(importer) {
			Guard.ArgumentNotNull(rows, "rows");
			this.rows = rows;
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public ExternalRowCollection Rows { get { return rows; } }
		public ExternalRow Row { get { return row; } }
		public override void ProcessElementOpen(XmlReader reader) {
			int index = Importer.GetWpSTIntegerValue(reader, "r", 1) - 1;
			if (Rows.Contains(index))
				Importer.ThrowInvalidFile();
			this.row = Rows.GetRow(index);
		}
		static Destination OnExternalCell(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ExternalCellDestination(importer, GetThis(importer).Row.Cells);
		}
	}
	#endregion
}
