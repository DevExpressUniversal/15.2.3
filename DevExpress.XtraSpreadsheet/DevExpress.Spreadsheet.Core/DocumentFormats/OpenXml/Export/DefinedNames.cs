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
using System.Diagnostics;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		#region Defined Names
		protected internal virtual void GenerateDefinedNames() {
			GenerateDefinedNamesCore(Workbook, ExportDefinedNameCore);
		}
		protected internal virtual void GenerateDefinedNamesCore(IModelWorkbook workbook, Action<DefinedNameBase, IWorksheet> action) {
			if (!HasDefinedNames(workbook))
				return;
			WriteShStartElement("definedNames");
			try {
				ExportDefinedNames(workbook.DefinedNames, null, action);
				foreach (IWorksheet sheet in workbook.GetSheets())
					ExportDefinedNames(sheet.DefinedNames, sheet, action);
			}
			finally {
				WriteShEndElement();
			}
		}
		bool HasDefinedNames(IModelWorkbook workbook) {
			int count = workbook.DefinedNames.Count;
			if (count > 0)
				return true;
			foreach (IWorksheet sheet in workbook.GetSheets()) {
				count += sheet.DefinedNames.Count;
				if (count > 0)
					return true;
			}
			return false;
		}
		void ExportDefinedNames(DefinedNameCollectionBase collection, IWorksheet scopeSheet, Action<DefinedNameBase, IWorksheet> action) {
			if (collection == null || collection.Count <= 0)
				return;
			foreach (DefinedNameBase definedName in collection)
				action(definedName, scopeSheet);
		}
		void ExportDefinedNameCore(DefinedNameBase item, IWorksheet scopeSheet) {
			DefinedName definedName = item as DefinedName;
			if(!ShouldExportDefinedName(definedName))
				return;
			WriteShStartElement("definedName");
			try {
				WriteShStringValue("name", EncodeXmlChars(item.Name));
				if (scopeSheet != null) {
					int sheetIndex = Workbook.Sheets.GetIndexById(scopeSheet.SheetId);
					WriteShStringValue("localSheetId", sheetIndex.ToString());
				}
				if (definedName != null) {
					if(definedName.IsHidden)
						WriteShStringValue("hidden", "1");
					if(definedName.IsMacro) {
						WriteShStringValue("xlm", "1");
						if(definedName.IsXlmMacro)
							WriteShStringValue("function", "1");
						if(definedName.IsVbaMacro)
							WriteShStringValue("vbProcedure", "1");
						if(definedName.FunctionGroupId > 0)
							WriteShIntValue("functionGroupId", definedName.FunctionGroupId);
					}
					if(!String.IsNullOrEmpty(definedName.Comment))
						WriteShStringValue("comment", EncodeXmlChars(definedName.Comment));
				}
				string reference = item.GetReference(0, 0);
				Debug.Assert(!reference.StartsWith("=", StringComparison.Ordinal));
				WriteShString(reference);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual bool ShouldExportDefinedName(DefinedName definedName) {
			if(definedName == null)
				return true;
			return !definedName.IsMacro;
		}
		#endregion
	}
}
