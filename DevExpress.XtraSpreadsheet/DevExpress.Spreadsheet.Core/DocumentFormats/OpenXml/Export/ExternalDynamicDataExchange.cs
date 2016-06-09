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
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Model.External;
using DevExpress.Office.Utils;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		#region Translation tables
		internal static readonly Dictionary<DdeValueType, string> DdeValueTypeTable = CreateDdeValueTypeTable();
		static Dictionary<DdeValueType, string> CreateDdeValueTypeTable() {
			Dictionary<DdeValueType, string> result = new Dictionary<DdeValueType, string>();
			result.Add(DdeValueType.Boolean, "b");
			result.Add(DdeValueType.Error, "e");
			result.Add(DdeValueType.Nil, "nil");
			result.Add(DdeValueType.RealNumber, "n");
			result.Add(DdeValueType.String, "str");
			return result;
		}
		#endregion
		protected internal virtual bool ShouldExportExternalDdeConnection(DdeExternalWorkbook ddeWorkbook) {
			return ddeWorkbook != null && !String.IsNullOrEmpty(ddeWorkbook.DdeServiceName) 
				&& !String.IsNullOrEmpty(ddeWorkbook.DdeServerTopic) && !ddeWorkbook.IsOleLink;
		}
		protected internal virtual void GenerateExternalDdeConnectionContent(DdeExternalWorkbook ddeWorkbook) {
			if (!ShouldExportExternalDdeConnection(ddeWorkbook))
				return;
			WriteShStartElement("ddeLink");
			try {
				WriteShStringValue("ddeService", ddeWorkbook.DdeServiceName);
				WriteShStringValue("ddeTopic", ddeWorkbook.DdeServerTopic);
				GenerateExternalDdeItemsContent(ddeWorkbook);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual void GenerateExternalDdeItemsContent(DdeExternalWorkbook ddeWorkbook) {
			if (ddeWorkbook.SheetCount == 0)
				return;
			WriteShStartElement("ddeItems");
			try {
				ddeWorkbook.Sheets.ForEach(GenerateExternalDdeItemContent);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual void GenerateExternalDdeItemContent(ExternalWorksheet sheet) {
			DdeExternalWorksheet ddeItem = sheet as DdeExternalWorksheet;
			if (!ShouldExportExternalDdeItem(ddeItem))
				return;
			WriteShStartElement("ddeItem");
			try {
				GenerateExternalDdeItemAttributesContent(ddeItem);
				GenerateExternalDdeLinkValuesContent(ddeItem);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual bool ShouldExportExternalDdeItem(DdeExternalWorksheet ddeItem) {
			return !ddeItem.IsOleLink && (ShouldExportExternalDdeLinkValues(ddeItem) || ddeItem.Name != "0" ||
				ddeItem.IsUsesOLE || ddeItem.Advise || ddeItem.IsDataImage);
		}
		protected internal virtual void GenerateExternalDdeItemAttributesContent(DdeExternalWorksheet ddeItem) {
			if (ddeItem.Name != "0")
				WriteShStringValue("name", ddeItem.Name);
			if (ddeItem.IsUsesOLE)
				WriteBoolValue("ole", ddeItem.IsUsesOLE);
			if (ddeItem.Advise)
				WriteBoolValue("advise", ddeItem.Advise);
			if (ddeItem.IsDataImage)
				WriteBoolValue("preferPic", ddeItem.IsDataImage);
		}
		protected internal virtual void GenerateExternalDdeLinkValuesContent(DdeExternalWorksheet ddeItem) {
			if (!ShouldExportExternalDdeLinkValues(ddeItem))
				return;
			WriteShStartElement("values");
			try {
				GenerateExternalDdeLinkValuesAttributesContent(ddeItem);
				int columns = ddeItem.ColumnCount;
				int count = ddeItem.RowCount * columns;
				ExternalDdeLinkValue ddeLinkValue = new ExternalDdeLinkValue();
				for(int i = 0; i < count; i++) {
					int rowIndex = i / columns;
					int columnIndex = i % columns;
					ExternalCell cell = ddeItem.TryGetCell(columnIndex, rowIndex) as ExternalCell;
					if(cell != null)
						ddeLinkValue.SetVariantValue(cell.Value);
					else
						ddeLinkValue.SetVariantValue(Model.VariantValue.Empty);
					GenerateExternalDdeLinkValueContent(ddeLinkValue);
				}
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual bool ShouldExportExternalDdeLinkValues(DdeExternalWorksheet ddeItem) {
			return ddeItem.ValuesCount > 0;
		}
		protected internal virtual void GenerateExternalDdeLinkValuesAttributesContent(DdeExternalWorksheet ddeItem) {
			if (ddeItem.RowCount != 1)
				WriteShIntValue("rows", ddeItem.RowCount);
			if (ddeItem.ColumnCount != 1)
				WriteShIntValue("cols", ddeItem.ColumnCount);
		}
		protected internal virtual void GenerateExternalDdeLinkValueContent(ExternalDdeLinkValue ddeLinkValue) {
			WriteShStartElement("value");
			try {
				if (ddeLinkValue.Type != DdeValueType.RealNumber)
					WriteShStringValue("t", DdeValueTypeTable[ddeLinkValue.Type]);
				if (!String.IsNullOrEmpty(ddeLinkValue.Value))
					WriteShString("val", ddeLinkValue.Value, true);
			}
			finally {
				WriteShEndElement();
			}
		}
	}
}
