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
using System.Globalization;
using System.Text;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Spreadsheet.Functions;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.API.Native.Implementation;
using Model = DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Localization;
using System.ComponentModel;
using DevExpress.Compatibility.System;
namespace DevExpress.Spreadsheet.Formulas {
	#region SheetReferenceType
	public enum SheetReferenceType {
		Simple, 
		CurrentWorksheet, 
		CurrentWorkbook, 
		External, 
		Invalid 
	}
	#endregion
	#region SheetReference
	public class SheetReference : ICloneable<SheetReference>, ICloneable, ISupportsCopyFrom<SheetReference> {
		#region Fields
		readonly IWorkbook workbook;
		string startSheetName = string.Empty;
		string endSheetName = string.Empty;
		string externalLinkName = string.Empty;
		SheetReferenceType type;
		#endregion
		public SheetReference(IWorkbook workbook) {
			this.workbook = workbook;
			this.type = SheetReferenceType.Simple;
		}
		public SheetReference(IWorkbook workbook, string sheetName)
			: this(workbook, sheetName, string.Empty) {
		}
		public SheetReference(IWorkbook workbook, string startSheetName, string endSheetName)
			: this(workbook) {
			this.startSheetName = startSheetName;
			this.endSheetName = endSheetName;
		}
		internal SheetReference(IWorkbook workbook, Model.SheetDefinition sheetDefinition)
			: this(workbook) {
			Guard.ArgumentNotNull(sheetDefinition, "sheetDefinition");
			InitFromModelSheetDefinition(sheetDefinition);
		}
		#region Properties
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("SheetReferenceStartSheetName")]
#endif
		public string StartSheetName { get { return startSheetName; } set { startSheetName = value; } }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("SheetReferenceEndSheetName")]
#endif
		public string EndSheetName { get { return endSheetName; } set { endSheetName = value; } }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("SheetReferenceType")]
#endif
		public SheetReferenceType Type { get { return type; } set { type = value; } }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("SheetReferenceExternalLinkName")]
#endif
		public string ExternalLinkName { get { return externalLinkName; } set { externalLinkName = value; } }
		Model.DocumentModel ModelWorkbook { get { return workbook.Model.DocumentModel; } }
		#endregion
		internal void BuildString(StringBuilder stringBuilder, Model.WorkbookDataContext context) {
			Model.SheetDefinition sheetDefinition = ToSheetDefinition();
			sheetDefinition.BuildExpressionString(stringBuilder, context);
		}
		void InitFromModelSheetDefinition(Model.SheetDefinition sheetDefinition) {
			this.type = SheetReferenceType.Simple;
			this.startSheetName = sheetDefinition.SheetNameStart;
			this.endSheetName = sheetDefinition.SheetNameEnd;
			if (!sheetDefinition.ValidReference)
				this.type = SheetReferenceType.Invalid;
			else if (sheetDefinition.IsCurrentSheetReference)
				this.type = SheetReferenceType.CurrentWorksheet;
			else if (sheetDefinition.IsCurrentWorkbookReference)
				this.type = SheetReferenceType.CurrentWorkbook;
			else if (sheetDefinition.IsExternalReference) {
				this.type = SheetReferenceType.External;
				this.externalLinkName = LookupExternalReferenceName(sheetDefinition.ExternalReferenceIndex);
			}
		}
		string LookupExternalReferenceName(int index) {
			if (index <= 0)
				return string.Empty;
			DevExpress.XtraSpreadsheet.Model.DocumentModel modelWorkbook = ModelWorkbook;
			DevExpress.XtraSpreadsheet.Model.External.ExternalLink externalLink = modelWorkbook.ExternalLinks[index - 1];
			if (externalLink == null)
				return string.Empty;
			return externalLink.Workbook.FilePath;
		}
		int LookupExternalReferenceIndex() {
			if (!string.IsNullOrEmpty(externalLinkName)) {
				int index = ModelWorkbook.ExternalLinks.IndexOf(externalLinkName);
				if (index >= 0)
					return index + 1;
			}
			return -1;
		}
		#region ICloneable<SheetReference> Members
		public SheetReference Clone() {
			SheetReference result = new SheetReference(workbook);
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ICloneable Members
		object ICloneable.Clone() {
			return Clone();
		}
		#endregion
		#region ISupportsCopyFrom<SheetReference> Members
		public void CopyFrom(SheetReference value) {
			this.type = value.type;
			this.startSheetName = value.startSheetName;
			this.endSheetName = value.endSheetName;
			this.externalLinkName = value.externalLinkName;
		}
		#endregion
		internal Model.SheetDefinition ToSheetDefinition() {
			Model.SheetDefinition sheetDefinition = new Model.SheetDefinition();
			switch (type) {
				case SheetReferenceType.Simple:
					sheetDefinition.SheetNameStart = startSheetName;
					sheetDefinition.SheetNameEnd = endSheetName;
					break;
				case SheetReferenceType.CurrentWorksheet:
					sheetDefinition.ExternalReferenceIndex = -1;
					break;
				case SheetReferenceType.CurrentWorkbook:
					sheetDefinition.ExternalReferenceIndex = 0;
					break;
				case SheetReferenceType.External:
					sheetDefinition.ExternalReferenceIndex = LookupExternalReferenceIndex();
					sheetDefinition.SheetNameStart = startSheetName;
					sheetDefinition.SheetNameEnd = endSheetName;
					break;
				case SheetReferenceType.Invalid:
					sheetDefinition.ValidReference = false;
					break;
				default:
					throw new ArgumentException("Invalid SheetReferenceType: " + type.ToString());
			}
			return sheetDefinition;
		}
	}
	#endregion
}
