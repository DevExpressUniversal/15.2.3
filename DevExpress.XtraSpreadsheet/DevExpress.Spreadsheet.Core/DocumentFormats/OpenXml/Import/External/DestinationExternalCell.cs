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
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model.External;
using DevExpress.XtraSpreadsheet.Model;
using System.Globalization;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region ExternalCellDestination
	public class ExternalCellDestination : CellDestinationBase<ExternalCell> {
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("v", OnCellValue);
			return result;
		}
		CellDataType cellDataType;
		public ExternalCellDestination(SpreadsheetMLBaseImporter importer, ExternalCellCollection cells)
			: base(importer, cells) {
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		protected override void ProcessElementOpenCore(XmlReader reader) {
			cellDataType = Importer.GetWpEnumValue<CellDataType>(reader, "t", CellDataTypeTable, CellDataType.Number);
		}
		public override void ProcessElementClose(XmlReader reader) {
			if (string.IsNullOrEmpty(Value)) {
				MoveLastColumnIndex();
				return;
			}
			Cell = CreateCell();
			if (Cell == null)
				return;
			switch (cellDataType) {
				case CellDataType.Bool:
					AssignBooleanValue(Importer.DocumentModel.DataContext);
					break;
				case CellDataType.Error:
					AssignErrorValue();
					break;
				case CellDataType.InlineString:
				case CellDataType.FormulaString:
					AssignInlineStringValue();
					break;
				case CellDataType.Number:
					AssignNumericValue(Importer.DocumentModel.DataContext);
					break;
				case CellDataType.SharedString:
				default:
					Exceptions.ThrowInternalException();
					break;
			}
		}
		void AssignInlineStringValue() {
			Cell.AssignValue(Value);
		}
	}
	#endregion
}
