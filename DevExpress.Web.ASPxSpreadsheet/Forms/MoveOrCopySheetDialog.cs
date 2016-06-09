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

using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.Web.Internal;
using DevExpress.Web.ASPxSpreadsheet.Localization;
namespace DevExpress.Web.ASPxSpreadsheet.Internal.Forms {
	public class MoveOrCopySheetDialog : SpreadsheetDialogBase {
		protected const string ContentTableID = "dxMoveOrCopySheetForm";
		protected const string MoveSheetsCaptionID = "lbMoveSheetsCaption",
							   BeforeSheetListBoxID = "lbxBeforeSheet",
							   BeforeSheetListBoxClientName = "_lbxBeforeSheet",
							   CreateCopyCheckBoxID = "cbCreateCopy",
							   CreateCopyCheckBoxClientName = "_cbCreateCopy";
		protected ASPxLabel MoveSheetsCaption { get; private set; }
		protected ASPxListBox BeforeSheet { get; private set; }
		protected ASPxCheckBox CreateCopy { get; private set; }
		protected override string GetDialogCssClassName() {
			return SpreadsheetStyles.MoveOrCopySheetDialogCssClass;
		}
		protected override string GetContentTableID() {
			return ContentTableID;
		}
		protected override void PopulateContentArea(Control container) {
			InstantiateEditors();
			HtmlTable table = new HtmlTable();
			table.Attributes.Add("class", SpreadsheetStyles.DialogContentTableCssClass);
			container.Controls.Add(table);
			HtmlTableRow tableRow = new HtmlTableRow();
			table.Rows.Add(tableRow);
			HtmlTableCell tableCell = DialogUtils.CreateCaptionCell();
			tableRow.Cells.Add(tableCell);
			tableCell.Controls.Add(MoveSheetsCaption);
			CreateSeparatorRow(tableCell);
			tableRow = new HtmlTableRow();
			table.Rows.Add(tableRow);
			tableCell = DialogUtils.CreateInputCell();
			tableRow.Cells.Add(tableCell);
			tableCell.Controls.Add(BeforeSheet);
			CreateSeparatorRow(tableCell);
			tableRow = new HtmlTableRow();
			table.Rows.Add(tableRow);
			tableCell = DialogUtils.CreateInputCell();
			tableRow.Cells.Add(tableCell);
			tableCell.Controls.Add(CreateCopy);
		}
		protected void CreateSeparatorRow(HtmlTableCell container) {
			WebControl webControl = RenderUtils.CreateDiv("dxssDlgCaptionIndent");
			container.Controls.Add(webControl);
		}
		protected void InstantiateEditors() {
			MoveSheetsCaption = new ASPxLabel() {
				ID = MoveSheetsCaptionID,
				AssociatedControlID = BeforeSheetListBoxID
			};
			BeforeSheet = new ASPxListBox() {
				ID = BeforeSheetListBoxID,
				ClientInstanceName = GetControlClientInstanceName(BeforeSheetListBoxClientName),
				Width = Unit.Percentage(100),
				Height = Unit.Pixel(200)
			};
			InitializeSheetNames();
			CreateCopy = new ASPxCheckBox() {
				ID = CreateCopyCheckBoxID,
				ClientInstanceName = GetControlClientInstanceName(CreateCopyCheckBoxClientName)
			};
		}
		protected void InitializeSheetNames() {
			var webSpreadsheetControl = Spreadsheet.GetCurrentWorkSessions().WebSpreadsheetControl;
			var innerControl = webSpreadsheetControl.InnerControl;
			MoveOrCopySheetCommand command = innerControl.CreateCommand(SpreadsheetCommandId.MoveOrCopySheet) as MoveOrCopySheetCommand;
			List<string> sheetNames = command.GetSheetNames();
			BeforeSheet.DataSource = sheetNames;
			BeforeSheet.DataBind();
		}
		protected override void ApplyLocalization() {
			base.ApplyLocalization();
			MoveSheetsCaption.Text = ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.MoveOrCopySheet_MoveSheetsCaption) + DialogUtils.LabelEndMark;
			CreateCopy.Text = ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.MoveOrCopySheet_CreateCopy);
		}
		protected override ASPxEditBase[] GetChildDxEdits() {
			return new ASPxEditBase[] { BeforeSheet, CreateCopy };
		}
	}
}
