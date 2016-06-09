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
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxSpreadsheet.Internal.Forms {
	public class FindAndReplaceDialog : SpreadsheetDialogWithRoundPanel {
		protected const string FindWhatTextBoxId = "txbFindWhat",
							   SearchParamsComboBoxId = "cbxSearchParams",
							   LookinComboBoxId = "cbxLookin",
							   SearchResultListBoxId = "lbxSearchResults";
		protected ASPxTextBox FindWhat { get; private set; }
		protected ASPxLabel FindWhatCaption { get; private set; }
		protected ASPxLabel CaptionSearchParams { get; private set; }
		protected ASPxComboBox SearchParams { get; private set; }
		protected ASPxCheckBox TextCase { get; private set; }
		protected ASPxLabel CaptionLookin { get; private set; }
		protected ASPxComboBox Lookin { get; private set; }
		protected ASPxCheckBox MatchContent { get; private set; }
		protected ASPxLabel CaptionSearchResults { get; private set; }
		protected ASPxListBox SearchResults { get; private set; }
		protected override string GetDialogCssClassName() {
			return "dxssDlgFindAndReplaceForm";
		}
		protected override string GetContentTableID() {
			return "dxFindAndReplaceForm";
		}
		protected override string GetRoundPanelID() {
			return "rpFindAndReplace";
		}
		protected override void PopulateRoundPanelContent(HtmlTable container) {
			#region FindWhatSection
			HtmlTableRow tableRow = new HtmlTableRow();
			container.Rows.Add(tableRow);
			HtmlTableCell tableCell = new HtmlTableCell();
			tableCell.Attributes.Add("class", SpreadsheetStyles.DialogCaptionCellCssClass);
			tableRow.Cells.Add(tableCell);
			FindWhatCaption = new ASPxLabel() { ID = "lblFind", AssociatedControlID = FindWhatTextBoxId };
			tableCell.Controls.Add(FindWhatCaption);
			tableCell = new HtmlTableCell();
			tableCell.Attributes.Add("class", SpreadsheetStyles.DialogInputCellCssClass);
			tableCell.ColSpan = 2;
			tableRow.Cells.Add(tableCell);
			FindWhat = new ASPxTextBox();
			tableCell.Controls.Add(FindWhat);
			FindWhat.ID = FindWhatTextBoxId;
			FindWhat.ClientInstanceName = GetControlClientInstanceName("_dxTxbFindWhat");
			FindWhat.Width = Unit.Percentage(100);
			FindWhat.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.Text;
			FindWhat.ValidationSettings.ErrorTextPosition = ErrorTextPosition.Bottom;
			FindWhat.ValidationSettings.SetFocusOnError = true;
			FindWhat.ValidationSettings.ValidateOnLeave = false;
			FindWhat.ValidationSettings.RequiredField.IsRequired = true;
			FindWhat.ValidationSettings.ValidationGroup = "_dxTxbFindGroup";
			FindWhat.ValidationSettings.ErrorFrameStyle.Font.Size = FontUnit.Parse("10px");
			#endregion
			#region SearchParamsSection
			tableRow = new HtmlTableRow();
			container.Rows.Add(tableRow);
			tableCell = new HtmlTableCell();
			tableCell.Attributes.Add("class", SpreadsheetStyles.DialogSeparatorCellCssClass);
			tableCell.ColSpan = 3;
			tableRow.Cells.Add(tableCell);
			tableRow = new HtmlTableRow();
			container.Rows.Add(tableRow);
			tableCell = new HtmlTableCell();
			tableCell.Attributes.Add("class", SpreadsheetStyles.DialogCaptionCellCssClass);
			tableRow.Cells.Add(tableCell);
			CaptionSearchParams = new ASPxLabel() { ID = "lblSearchParams", AssociatedControlID = SearchParamsComboBoxId };
			tableCell.Controls.Add(CaptionSearchParams);
			tableCell = new HtmlTableCell();
			tableCell.Attributes.Add("class", "dxssDlgComboCell");
			tableRow.Cells.Add(tableCell);
			SearchParams = new ASPxComboBox();
			tableCell.Controls.Add(SearchParams);
			SearchParams.ID = SearchParamsComboBoxId;
			SearchParams.ClientInstanceName = GetControlClientInstanceName("_dxCbxSearchParams");
			SearchParams.Width = Unit.Percentage(100);
			SearchParams.Items.Add(ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.FindAndReplace_Search_ByRows), "By Rows");
			SearchParams.Items.Add(ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.FindAndReplace_Search_ByColumns), "By Columns");
			SearchParams.Items[0].Selected = true;
			tableCell = new HtmlTableCell();
			tableCell.Attributes.Add("class", SpreadsheetStyles.DialogInputCellCssClass);
			tableRow.Cells.Add(tableCell);
			TextCase = new ASPxCheckBox();
			tableCell.Controls.Add(TextCase);
			TextCase.ID = "cbTextCase";
			TextCase.ClientInstanceName = GetControlClientInstanceName("_dxCbTextCase");
			#endregion
			#region LookinSection
			tableRow = new HtmlTableRow();
			container.Rows.Add(tableRow);
			tableCell = new HtmlTableCell();
			tableCell.Attributes.Add("class", SpreadsheetStyles.DialogSeparatorCellCssClass);
			tableCell.ColSpan = 3;
			tableRow.Cells.Add(tableCell);
			tableRow = new HtmlTableRow();
			container.Rows.Add(tableRow);
			tableCell = new HtmlTableCell();
			tableCell.Attributes.Add("class", SpreadsheetStyles.DialogCaptionCellCssClass);
			tableRow.Cells.Add(tableCell);
			CaptionLookin = new ASPxLabel() { ID = "lblLookin", AssociatedControlID = LookinComboBoxId };
			tableCell.Controls.Add(CaptionLookin);
			tableCell = new HtmlTableCell();
			tableCell.Attributes.Add("class", "dxssDlgComboCell");
			tableRow.Cells.Add(tableCell);
			Lookin = new ASPxComboBox();
			tableCell.Controls.Add(Lookin);
			Lookin.ID = LookinComboBoxId;
			Lookin.ClientInstanceName = GetControlClientInstanceName("_dxCbxLookin");
			Lookin.Width = Unit.Percentage(100);
			Lookin.Items.Add(ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.FindAndReplace_LookIn_Formulas), "Formulas");
			Lookin.Items.Add(ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.FindAndReplace_LookIn_Values), "Values");
			Lookin.Items[0].Selected = true;
			tableCell = new HtmlTableCell();
			tableCell.Attributes.Add("class", SpreadsheetStyles.DialogInputCellCssClass);
			tableRow.Cells.Add(tableCell);
			MatchContent = new ASPxCheckBox();
			tableCell.Controls.Add(MatchContent);
			MatchContent.ID = "cbMatchContent";
			MatchContent.ClientInstanceName = GetControlClientInstanceName("_dxCbMatchContent");
			#endregion
			#region SearchResultSection
			tableRow = new HtmlTableRow();
			container.Rows.Add(tableRow);
			tableCell = new HtmlTableCell();
			tableCell.Attributes.Add("class", SpreadsheetStyles.DialogSeparatorCellCssClass);
			tableCell.ColSpan = 3;
			tableRow.Cells.Add(tableCell);
			tableRow = new HtmlTableRow();
			container.Rows.Add(tableRow);
			tableCell = new HtmlTableCell();
			tableCell.Attributes.Add("class", "dxssDlgSearchArea");
			tableCell.ColSpan = 3;
			tableRow.Cells.Add(tableCell);
			CaptionSearchResults = new ASPxLabel() { ID = "lblSearchResult", AssociatedControlID = SearchResultListBoxId };
			tableCell.Controls.Add(CaptionSearchResults);
			WebControl separatorDiv = RenderUtils.CreateDiv("dxssDlgCaptionIndent");
			tableCell.Controls.Add(separatorDiv);
			SearchResults = new ASPxListBox();
			tableCell.Controls.Add(SearchResults);
			SearchResults.ID = SearchResultListBoxId;
			SearchResults.ClientInstanceName = GetControlClientInstanceName("_dxLbxSearchResults");
			SearchResults.Width = Unit.Percentage(100);
			SearchResults.Height = Unit.Pixel(120);
			SearchResults.ValueType = typeof(System.String);
			((System.Web.UI.IAttributeAccessor)(SearchResults)).SetAttribute("TextFormatString", "{0}|{1}");
			SearchResults.ValidationSettings.CausesValidation = false;
			SearchResults.ClientSideEvents.ItemDoubleClick = "ASPx.SSSearchItemClicked";
			SearchResults.ClientSideEvents.KeyDown = "ASPx.SSSearchResultKeyDown";
			SearchResults.ClientSideEvents.SelectedIndexChanged = "ASPx.SSSearchResultScrollToItem";
			SearchResults.Columns.Add("fCell", ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.FindAndReplace_SearchResults_CellHeader), Unit.Pixel(30));
			SearchResults.Columns.Add("fValue", ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.FindAndReplace_SearchResults_ValueHeader), Unit.Percentage(100));
			#endregion
		}
		protected override string GetDefaultSubmitButtonCaption() {
			return ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.ButtonFindAll);
		}
		protected override void PrepareChildControls() {
			base.PrepareChildControls();
			RoundPanel.Style.Add("width", "480px"); 
			SearchParams.Width = Unit.Pixel(120);
			Lookin.Width = Unit.Pixel(120);
		}
		protected override void ApplyLocalization() {
			base.ApplyLocalization();
			FindWhatCaption.Text = ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.FindAndReplace_FindWhat) + DialogUtils.LabelEndMark;
			TextCase.Text = ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.FindAndReplace_MatchCase);
			CaptionSearchParams.Text = ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.FindAndReplace_Search) + DialogUtils.LabelEndMark;
			CaptionLookin.Text = ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.FindAndReplace_LookIn) + DialogUtils.LabelEndMark;
			MatchContent.Text = ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.FindAndReplace_MatchCellContent);
			CaptionSearchResults.Text = ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.FindAndReplace_SearchResults) + DialogUtils.LabelEndMark;
			FindWhat.ValidationSettings.RequiredField.ErrorText = ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.RequiredFieldError);
		}
		protected override ASPxEditBase[] GetChildDxEdits() {
			return new ASPxEditBase[] { FindWhat, SearchParams, TextCase, Lookin, MatchContent, SearchResults };
		}
	}
}
