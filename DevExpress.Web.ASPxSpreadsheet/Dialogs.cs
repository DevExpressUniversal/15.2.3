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
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxSpreadsheet.Internal;
using DevExpress.Web.ASPxSpreadsheet.Localization;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxSpreadsheet {
	[ToolboxItem(false)]
	public class SpreadsheetUserControl : UserControl, IDialogFormElementRequiresLoad {
		private ASPxSpreadsheet spreadsheet = null;
		private ASPxSpreadsheet FindParentSpreadsheet() {
			Control curControl = Parent;
			while(curControl != null) {
				if(curControl is ASPxSpreadsheet)
					return curControl as ASPxSpreadsheet;
				curControl = curControl.Parent;
			}
			return null;
		}
		protected ASPxSpreadsheet Spreadsheet {
			get {
				if(spreadsheet == null)
					spreadsheet = FindParentSpreadsheet();
				return spreadsheet;
			}
		}
		protected override void OnInit(EventArgs e) {
			ClientIDHelper.UpdateClientIDMode(this);
			base.OnInit(e);
		}
		protected override void OnLoad(System.EventArgs e) {
			base.OnLoad(e);
			PrepareChildControls();
		}
		protected virtual ASPxButton[] GetChildDxButtons() {
			return new ASPxButton[] { };
		}
		protected virtual ASPxEditBase[] GetChildDxEdits() {
			return new ASPxEditBase[] { };
		}
		protected virtual ASPxRoundPanel GetChildDxSpreadsheetPanel() {
			return null;
		}
		protected virtual ASPxRoundPanel[] GetChildDxSpreadsheetPanels() {
			List<ASPxRoundPanel> roundPanels = new List<ASPxRoundPanel>();
			ASPxRoundPanel firstPanel = GetChildDxSpreadsheetPanel();
			if(firstPanel != null)
				roundPanels.Add(firstPanel);
			return roundPanels.ToArray();
		}
		protected virtual void PrepareChildControls() {
			EnsureChildControls();
			ApplyLocalization();
			ASPxEditBase[] dxEdits = GetChildDxEdits();
			foreach(ASPxEditBase edit in dxEdits) {
				edit.ParentStyles = Spreadsheet.StylesEditors;
			}
			ASPxButton[] dxButtons = GetChildDxButtons();
			foreach(ASPxButton btn in dxButtons) {
				btn.ParentStyles = Spreadsheet.StylesButton;
			}
		}
		protected virtual void ApplyLocalization() { }
		protected void AddTemplateToControl(Control destinationContainer, ITemplate template) {
			if(template == null || destinationContainer == null)
				return;
			template.InstantiateIn(destinationContainer);
		}
		void IDialogFormElementRequiresLoad.ForceInit() {
			FrameworkInitialize();
		}
		void IDialogFormElementRequiresLoad.ForceLoad() {
			OnLoad(EventArgs.Empty);
		}
		public override string ClientID {
			get { return ClientIDHelper.GenerateClientID(this, base.ClientID); }
		}
	}
	[ToolboxItem(false)]
	public abstract class SpreadsheetDialogBase : SpreadsheetUserControl {
		private const string InitSubmitButtonEventHandler = "ASPx.SpreadsheetDialog.SubmitButtonInit",
							 InitCancelButtonEventHandler = "ASPx.SpreadsheetDialog.CancelButtonInit";
		protected const string MailPanelID = "MainPanel";
		protected ASPxPanel MainPanel { get; private set; }
		protected HtmlTable ContenTable { get; private set; }
		protected HtmlTableCell ContenCell { get; private set; }
		protected HtmlTableCell FooterCell { get; private set; }
		protected ASPxButton SubmitButton { get; private set; }
		protected ASPxButton CancelButton { get; private set; }
		protected override void CreateChildControls() {
			base.CreateChildControls();
			MainPanel = new ASPxPanel() { ID = MailPanelID, DefaultButton = GetDefaultSubmitButtonID() };
			Controls.Add(MainPanel);
			ContenTable = DialogUtils.CreateTable(GetDialogCssClassName());
			ContenTable.ID = GetContentTableID();
			MainPanel.Controls.Add(ContenTable);
			HtmlTableRow tableRow = new HtmlTableRow();
			ContenTable.Rows.Add(tableRow);
			ContenCell = new HtmlTableCell();
			PopulateContentArea(ContenCell);
			tableRow.Cells.Add(ContenCell);
			tableRow = new HtmlTableRow();
			ContenTable.Rows.Add(tableRow);
			FooterCell = new HtmlTableCell();
			tableRow.Cells.Add(FooterCell);
			PopulateFooterArea(FooterCell);
		}
		protected override void PrepareChildControls() {
			base.PrepareChildControls();
			ContenCell.Attributes.Add("class", SpreadsheetStyles.DialogContentCellCssClass);
			FooterCell.Attributes.Add("class", SpreadsheetStyles.DialogFooterCellCssClass);
		}
		protected abstract string GetDialogCssClassName();
		protected abstract string GetContentTableID();
		protected abstract void PopulateContentArea(Control container);
		#region DialogFooterArea
		protected virtual void PopulateFooterArea(Control container) {
			SubmitButton = InitializeSubmitButton();
			container.Controls.Add(SubmitButton);
			InitializeMiddleButtons(container);
			CancelButton = InitializeCancelButton();
			container.Controls.Add(CancelButton);
		}
		protected virtual void InitializeMiddleButtons(Control container) {
		}
		protected virtual ASPxButton InitializeSubmitButton() {
			ASPxButton submitButton = new ASPxButton();
			submitButton.ID = GetDefaultSubmitButtonID();
			submitButton.AutoPostBack = false;
			submitButton.CssClass = SpreadsheetStyles.DialogFooterButtonCssClass;
			submitButton.CausesValidation = false;
			submitButton.ClientInstanceName = GetControlClientInstanceName("_dxeBtnOk");
			submitButton.ClientSideEvents.Init = GetDefaultSubmitButtonInitEventHandler();
			return submitButton;
		}
		protected virtual ASPxButton InitializeCancelButton() {
			ASPxButton cancelButton = new ASPxButton();
			cancelButton.ID = GetDefaultCancelButtonID();
			cancelButton.AutoPostBack = false;
			cancelButton.CssClass = SpreadsheetStyles.DialogFooterButtonCssClass;
			cancelButton.CausesValidation = false;
			cancelButton.ClientSideEvents.Init = GetDefaultCancelButtonInitEventHandler();
			return cancelButton;
		}
		protected virtual string GetDefaultSubmitButtonCaption() {
			return ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.ButtonOK);
		}
		protected virtual string GetDefaultCancelButtonCaption() {
			return ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.ButtonCancel);
		}
		protected virtual string GetDefaultSubmitButtonInitEventHandler() {
			return InitSubmitButtonEventHandler;
		}
		protected virtual string GetDefaultCancelButtonInitEventHandler() {
			return InitCancelButtonEventHandler;
		}
		protected virtual string GetDefaultSubmitButtonID() {
			return "btnOk";
		}
		protected virtual string GetDefaultCancelButtonID() {
			return "btnCancel";
		}
		#endregion
		protected override void ApplyLocalization() {
			base.ApplyLocalization();
			CancelButton.Text = GetDefaultCancelButtonCaption();
			SubmitButton.Text = GetDefaultSubmitButtonCaption();
		}
		protected string GetControlClientInstanceName(string name) {
			return this.ClientID + name;
		}
		protected override ASPxButton[] GetChildDxButtons() {
			return new ASPxButton[] { SubmitButton, CancelButton };
		}
		protected virtual ITemplate GetTopAreaTemplate() {
			return null;
		}
		protected virtual ITemplate GetBottomAreaTemplate() {
			return null;
		}
	}
	public abstract class SpreadsheetDialogWithRoundPanel : SpreadsheetDialogBase {
		protected ASPxRoundPanel RoundPanel { get; private set; }
		protected HtmlTable RoundPanelContent { get; private set; }
		protected override void PopulateContentArea(Control container) {
			CreateRoundPanel(container);
		}
		protected virtual void CreateRoundPanel(Control container) {
			RoundPanel = new ASPxRoundPanel() { ID = GetRoundPanelID(), ShowHeader = false, Width = Unit.Percentage(100) };
			RoundPanelContent = DialogUtils.CreateTable(SpreadsheetStyles.DialogRoundPanelContentCssClass);
			PopulateRoundPanelContent(RoundPanelContent);
			RoundPanel.Controls.Add(RoundPanelContent);
			container.Controls.Add(RoundPanel);
		}
		protected abstract void PopulateRoundPanelContent(HtmlTable container);
		protected abstract string GetRoundPanelID();
		protected override ASPxRoundPanel GetChildDxSpreadsheetPanel() {
			return RoundPanel;
		}
	}
	public abstract class SpreadsheetDialogWithChoice : SpreadsheetDialogWithRoundPanel {
		protected HtmlTableCell ChoiceContainer { get; private set; }
		protected HtmlTable RadioButtonTable { get; private set; }
		protected ASPxRadioButton FirstChoice { get; private set; }
		protected ASPxRadioButton SecondChoice { get; private set; }
		protected virtual bool IsDialogContainsChoiceSection {
			get { return false; }
		}
		protected override void PopulateContentArea(Control container) {
			if(IsDialogContainsChoiceSection) {
				HtmlTable innerContentTable = DialogUtils.CreateTable();
				container.Controls.Add(innerContentTable);
				HtmlTableRow tableRow = new HtmlTableRow();
				innerContentTable.Rows.Add(tableRow);
				ChoiceContainer = new HtmlTableCell();
				PopulateChoiceContainer(ChoiceContainer);
				tableRow.Cells.Add(ChoiceContainer);
				tableRow = new HtmlTableRow();
				innerContentTable.Rows.Add(tableRow);
				HtmlTableCell tableCell = new HtmlTableCell();
				CreateRoundPanel(tableCell);
				tableRow.Cells.Add(tableCell);
			} else {
				base.PopulateContentArea(container);
			}
		}
		protected override void PrepareChildControls() {
			base.PrepareChildControls();
			if(IsDialogContainsChoiceSection) {
				ChoiceContainer.Attributes.Add("class", SpreadsheetStyles.DialogRadioButtonCssClass);
			}
		}
		protected virtual void PopulateChoiceContainer(Control container) {
			RadioButtonTable = DialogUtils.CreateTable();
			container.Controls.Add(RadioButtonTable);
			HtmlTableRow tableRow = new HtmlTableRow();
			RadioButtonTable.Rows.Add(tableRow);
			HtmlTableCell tableCell = new HtmlTableCell();
			FirstChoice = CreateFirstChoiceElement();
			tableCell.Controls.Add(FirstChoice);
			tableRow.Cells.Add(tableCell);
			tableCell = new HtmlTableCell();
			tableCell.Controls.Add(new LiteralControl("&nbsp"));
			tableCell.Attributes.Add("class", SpreadsheetStyles.DialogSeparatorCellCssClass);
			tableRow.Cells.Add(tableCell);
			tableCell = new HtmlTableCell();
			SecondChoice = CreateSecondChoiceElement();
			tableCell.Controls.Add(SecondChoice);
			tableRow.Cells.Add(tableCell);
		}
		protected abstract ASPxRadioButton CreateSecondChoiceElement();
		protected abstract ASPxRadioButton CreateFirstChoiceElement();
		protected override ASPxEditBase[] GetChildDxEdits() {
			if(IsDialogContainsChoiceSection) {
				ASPxEditBase[] baseList = base.GetChildDxEdits();
				List<ASPxEditBase> baseCollection = new List<ASPxEditBase>(baseList);
				baseCollection.Add(FirstChoice);
				baseCollection.Add(SecondChoice);
				return baseCollection.ToArray();
			}
			return base.GetChildDxEdits();
		}
	}
	[ToolboxItem(false)]
	public class SpreadsheetFileManager : ASPxFileManager {
		protected internal new const string ScriptName = ASPxSpreadsheet.SpreadsheetFileManagerScriptResourceName;
		protected internal bool IsSpreadsheetCallback { get; set; }
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(SpreadsheetFileManager), ScriptName);
		}
		protected override StylesBase CreateStyles() {
			return new SpreadsheetFileManagerStyles(this);
		}
		protected override string GetClientObjectClassName() {
			return "ASPx.SpreadsheetFileManager";
		}
		protected internal object GetCallbackResult(string callbackArgs) {
			RaisePostBackEventCore(callbackArgs);
			return GetCallbackResult();
		}
		protected override bool IsNeedResetToInitalFolder() {
			return false;
		}
		protected override ASPxUploadControl CreateUploadControl(ASPxWebControl owner) {
			return new SpreadsheetFileManagerUploadControl(owner, this);
		}
		protected override bool IsNeedToAddCallbackCommandResult() {
			return base.IsNeedToAddCallbackCommandResult() || IsSpreadsheetCallback;
		}
	}
	[ToolboxItem(false)]
	public class SpreadsheetFolderManager : ASPxFileManager {
		protected internal new const string ScriptName = ASPxSpreadsheet.SpreadsheetFolderManagerScriptResourceName;
		protected internal bool IsSpreadsheetCallback { get; set; }
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(SpreadsheetFolderManager), ScriptName);
		}
		protected override StylesBase CreateStyles() {
			return new SpreadsheetFileManagerStyles(this);
		}
		protected override string GetClientObjectClassName() {
			return "ASPx.SpreadsheetFolderManager";
		}
		protected internal object GetCallbackResult(string callbackArgs) {
			RaisePostBackEventCore(callbackArgs);
			return GetCallbackResult();
		}
		protected override bool IsNeedResetToInitalFolder() {
			return false;
		}
		protected override bool IsNeedToAddCallbackCommandResult() {
			return base.IsNeedToAddCallbackCommandResult() || IsSpreadsheetCallback;
		}
	}
	[ToolboxItem(false)]
	public sealed class SpreadsheetUploadControl : ASPxUploadControl {
		internal const string ScriptName = ASPxSpreadsheet.SpreadsheetUploadControlScriptResourceName;
		public SpreadsheetUploadControl()
			: base() {
			this.OwnerControl = FindParentSpreassheetControl();
		}
		protected override string GetClientObjectClassName() {
			return "ASPx.SpreadsheetUploadControl";
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(SpreadsheetUploadControl), ScriptName);
		}
		protected override void OnInit(System.EventArgs e) {
			ASPxSpreadsheet editor = FindParentSpreassheetControl();
			if(editor != null)
				ValidationSettings.AllowedFileExtensions = new string[] { ".bmp", ".dib", ".jpe", ".jpeg", ".jpg", ".gif", ".png" };
			base.OnInit(e);
		}
		private ASPxSpreadsheet FindParentSpreassheetControl() {
			Control curControl = Parent;
			while(curControl != null) {
				if(curControl is ASPxSpreadsheet)
					return curControl as ASPxSpreadsheet;
				curControl = curControl.Parent;
			}
			return null;
		}
	}
	[ToolboxItem(false)]
	public sealed class SpreadsheetFileManagerUploadControl : FileManagerUploadControl {
		internal const string UploadControlScriptName = ASPxSpreadsheet.SpreadsheetFileManagerUploadControlScriptResourceName;
		public SpreadsheetFileManagerUploadControl(ASPxWebControl owner, ASPxFileManager fileManager) : base(owner, fileManager) { }
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(SpreadsheetFileManagerUploadControl), UploadControlScriptName);
		}
		protected override string GetClientObjectClassName() {
			return "ASPx.SpreadsheetFileManagerUploadControl";
		}
	}
}
