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
using System.Text;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Spreadsheet;
using DevExpress.XtraSpreadsheet.Localization;
namespace DevExpress.XtraSpreadsheet.Commands {
	public class PasteFieldListInfoCommand : PasteCommandBase {
		PasteFieldListInfoHelper[] fields;
		public CellRangeBase Range { get; set; }
		public PasteFieldListInfoCommand(ISpreadsheetControl control)
			: base(control) {
			Range = null;
			fields = null;
		}
		public PasteFieldListInfoCommand(ISpreadsheetControl control, CellRangeBase range, PasteFieldListInfoHelper[] fields)
			: base(control) {
			Range = range;
			this.fields = fields;
		}
		public override DocumentFormat Format { get { return DocumentFormat.Undefined; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_None; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_None; } }
		bool PrepareFields() {
			if (fields != null && fields.Length > 0)
				return true;
			object data = PasteSource.GetData("DevExpress.XtraSpreadsheet.Commands.PasteFieldListInfoHelper[]", true);
			fields = data as PasteFieldListInfoHelper[];
			return fields != null && fields.Length != 0;
		}
		protected internal override void PerformModifyModel() {
			if (!PrepareFields())
				return;
			MailMergeOptions options = new MailMergeOptions(DocumentModel);
			string rangeDataMember = string.Empty;
			CellPosition cell = Range.TopLeft;
			foreach (CellRange mergedCell in ActiveSheet.MergedCells.GetEVERYMergedRangeSLOWEnumerable()) {
				if (mergedCell.ContainsCell(cell.Column, cell.Row)) {
					cell = mergedCell.TopLeft;
				}
			}
			if (options.DetailRange != null) {
				if (options.DetailRange.Worksheet != DocumentModel.ActiveSheet)
					return;
				rangeDataMember = options.GetFullDataMemberFromPosition(cell);
			}
			if (string.IsNullOrEmpty(rangeDataMember))
				rangeDataMember = DocumentModel.MailMergeDataMember;
			if (string.IsNullOrEmpty(rangeDataMember))
				rangeDataMember = string.Empty;
			IEnumerable<PasteFieldListInfoHelper> insertableFields = GetInfoNames(rangeDataMember, fields);
			string functionFieldName = FormulaCalculator.GetLocalizedFunctionName("FIELD", DocumentModel.DataContext);
			string functionFieldPictureName = FormulaCalculator.GetLocalizedFunctionName("FIELDPICTURE", DocumentModel.DataContext);
			StringBuilder formula = new StringBuilder();
			foreach (PasteFieldListInfoHelper info in insertableFields) {
				formula.Append(formula.Length == 0 ? "=" : "&\" \"&");
				if (info.IsFieldPicture) {
					AppendFieldPictureFunction(functionFieldPictureName, info.FieldValue, formula);
				}
				else {
					AppendFieldFunction(functionFieldName, info.FieldValue, formula);
				}
			}
			CellContentSnapshot snapshot = new CellContentSnapshot(ActiveSheet[cell]);
			ActiveSheet[cell].SetFormulaBody(formula.ToString());
			DocumentModel.RaiseCellValueChangedProgrammatically(snapshot);
			ActiveSheet.Selection.SetSelection(cell);
		}
		void AppendFieldPictureFunction(string functionFieldPictureName, string fieldValue, StringBuilder formula) {
			formula.Append(functionFieldPictureName);
			formula.Append("(\"");
			formula.Append(fieldValue);
			formula.Append("\"");
			formula.Append(DocumentModel.DataContext.GetListSeparator());
			formula.Append("\"Range\"");
			formula.Append(DocumentModel.DataContext.GetListSeparator());
			formula.Append(Range);
			formula.Append(")");
		}
		void AppendFieldFunction(string functionFieldName, string field, StringBuilder formula) {
			formula.Append(functionFieldName);
			formula.Append("(\"");
			formula.Append(field);
			formula.Append("\")");
		}
		protected internal override bool ChangeSelection() {
			return false;
		}
		public static string GetFilteredFieldName(string[] dataMember, string field) {
			StringBuilder infoName = new StringBuilder();
			string[] dataPaths = field.Split('.');
			if (dataMember.Length >= dataPaths.Length || dataMember.Length == 0)
				infoName.Append(field);
			else {
				int i = 0;
				while (i < dataMember.Length) {
					if (dataMember[i] == dataPaths[i])
						i++;
					else
						break;
				}
				for (; i < dataPaths.Length; i++) {
					infoName.Append(dataPaths[i]);
					if (i < dataPaths.Length - 1)
						infoName.Append('.');
				}
			}
			return infoName.ToString();
		}
		PasteFieldListInfoHelper[] GetInfoNames(string defaultMember, PasteFieldListInfoHelper[] fieldNames) {
			List<PasteFieldListInfoHelper> result = new List<PasteFieldListInfoHelper>();
			if (fieldNames.Length != 0) {
				string[] dataMember = defaultMember.Split('.');
				foreach (PasteFieldListInfoHelper field in fieldNames) {
					result.Add(new PasteFieldListInfoHelper(field.IsFieldPicture, GetFilteredFieldName(dataMember, field.FieldValue)));
				}
			}
			return result.ToArray();
		}
		protected internal override bool IsDataAvailable() {
			if (fields != null && fields.Length > 0)
				return true;
			object data = PasteSource.GetData("DevExpress.XtraSpreadsheet.Commands.PasteFieldListInfoHelper[]", true);
			PasteFieldListInfoHelper[] testFields = data as PasteFieldListInfoHelper[];
			return testFields != null && testFields.Length != 0 && Range != null;
		}
	}
	#region PasteFieldListInfoCommand
	public class PasteFieldListInfoHelper {
		#region Properties
		public bool IsFieldPicture { get; private set; }
		public string FieldValue { get; private set; }
		#endregion
		public PasteFieldListInfoHelper(bool isFieldPicture, string fieldValue) {
			IsFieldPicture = isFieldPicture;
			FieldValue = fieldValue;
		}
	}
	#endregion
}
