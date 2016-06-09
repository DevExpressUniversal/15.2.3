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
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Services;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Commands;
namespace DevExpress.XtraSpreadsheet.Forms {
	#region RenameSheetViewModel
	public class RenameSheetViewModel : ViewModelBase {
		#region Fields
		readonly ISpreadsheetControl control;
		readonly Worksheet sheet;
		string sheetName;
		#endregion
		public RenameSheetViewModel(ISpreadsheetControl control, Worksheet sheet) {
			Guard.ArgumentNotNull(control, "control");
			Guard.ArgumentNotNull(sheet, "sheet");
			this.control = control;
			this.sheet = sheet;
			this.sheetName = sheet.Name;
		}
		#region SheetName
		public string SheetName {
			get { return sheetName; }
			set {
				if (SheetName == value)
					return;
				this.sheetName = value;
				OnPropertyChanged("SheetName");
			}
		}
		#endregion
		IWorksheetNameCreationService GetService() {
			return control.GetService(typeof(IWorksheetNameCreationService)) as IWorksheetNameCreationService;
		}
		public string ValidateSheetName() {
			IWorksheetNameCreationService service = GetService();
			if (service == null)
				return String.Empty;
			string[] existingSheetNames = sheet.Workbook.Sheets.GetSheetNames();
			WorksheetNameError error = service.VerifyName(SheetName, existingSheetNames);
			if (error == WorksheetNameError.None || SheetName == sheet.Name)
				return String.Empty;
			if (error == WorksheetNameError.Duplicate)
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorDuplicateSheetName);
			if (error == WorksheetNameError.Blank)
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorBlankSheetName);
			if (error == WorksheetNameError.StartOrEndWithSingleQuote)
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorSheetNameStartOrEndWithSingleQuote);
			if (error == WorksheetNameError.ContainsProhibitedCharacters)
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorSheetNameContainsNotAllowedCharacters);
			if (error == WorksheetNameError.ExceedAllowedLength)
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorSheetNameExceedAllowedLength);
			return String.Empty;
		}
		public List<char> GetRestrictedCharacters() {
			List<char> result = new List<char>();
			IWorksheetNameCreationService service = GetService();
			if (service == null)
				return result;
			result.AddRange(service.RestrictedCharacters);
			return result;
		}
		public void ApplyChanges() {
			RenameSheetCommand command = new RenameSheetCommand(control);
			command.ShowRenameSheetFormCallback(sheet, SheetName);
		}
	}
	#endregion
}
