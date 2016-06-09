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

using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxSpreadsheet.Localization;
namespace DevExpress.Web.ASPxSpreadsheet.Internal.Forms {
	public class UnhideSheetDialog : SpreadsheetDialogWithRoundPanel {
		protected const string UnhideSheetListBoxId = "lbUnhideSheet";
		protected ASPxListBox UnhideSheet { get; private set; }
		protected ASPxLabel Caption { get; private set; }
		protected override string GetDialogCssClassName() {
			return SpreadsheetStyles.UnhideSheetDialogCssClass;
		}
		protected override string GetContentTableID() {
			return "dxUnhideSheetForm";
		}
		protected override string GetRoundPanelID() {
			return "rpUnhideSheet";
		}
		protected override void PopulateRoundPanelContent(HtmlTable container) {
			HtmlTableRow tableRow = new HtmlTableRow();
			container.Rows.Add(tableRow);
			HtmlTableCell tableCell = DialogUtils.CreateInputCell();
			tableRow.Cells.Add(tableCell);
			Caption = new ASPxLabel() { ID = "lblChartChangeTitle", AssociatedControlID = UnhideSheetListBoxId };
			tableCell.Controls.Add(Caption);
			UnhideSheet = new ASPxListBox();
			tableCell.Controls.Add(UnhideSheet);
			UnhideSheet.ID = UnhideSheetListBoxId;
			UnhideSheet.ClientInstanceName = GetControlClientInstanceName("_dxlbUnhideSheet");
			UnhideSheet.Width = Unit.Percentage(100);
			UnhideSheet.SelectionMode = ListEditSelectionMode.Single;
			UnhideSheet.ValueType = typeof(System.String);
			UnhideSheet.SelectedIndex = 0;
			UnhideSheet.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.Text;
			UnhideSheet.ValidationSettings.ErrorTextPosition = ErrorTextPosition.Bottom;
			UnhideSheet.ValidationSettings.SetFocusOnError = true;
			UnhideSheet.ValidationSettings.ValidateOnLeave = false;
			UnhideSheet.ValidationSettings.ValidationGroup = "_dxSpUnhideSheet";
			UnhideSheet.ValidationSettings.ErrorFrameStyle.Font.Size = FontUnit.Parse("10px");
		}
		protected override void ApplyLocalization() {
			base.ApplyLocalization();
			Caption.Text = ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.UnhideSheet_Caption) + DialogUtils.LabelEndMark;
			UnhideSheet.ValidationSettings.RequiredField.ErrorText = ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.RequiredFieldError);
		}
		protected override ASPxEditBase[] GetChildDxEdits() {
			return new ASPxEditBase[] { UnhideSheet };
		}
	}
}
