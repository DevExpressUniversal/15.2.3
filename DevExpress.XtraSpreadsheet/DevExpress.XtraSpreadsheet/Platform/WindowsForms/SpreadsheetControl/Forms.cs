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
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.DataAccess;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.EntityFramework;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.UI.EntityFramework;
using DevExpress.DataAccess.UI.Sql;
using DevExpress.DataAccess.UI.Wizard;
using DevExpress.DataAccess.UI.Wizard.Clients;
using DevExpress.DataAccess.Wizard;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.DataAccess.Wizard.Native;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.Entity.ProjectModel;
using DevExpress.Office.Forms;
using DevExpress.Utils.Internal;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Native;
using DevExpress.XtraRichEdit.Forms;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Forms;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Services;
using HyperlinkForm = DevExpress.XtraSpreadsheet.Forms.HyperlinkForm;
using HyperlinkFormControllerParameters = DevExpress.XtraSpreadsheet.Forms.HyperlinkFormControllerParameters;
using InsertTableForm = DevExpress.XtraSpreadsheet.Forms.InsertTableForm;
using PageSetupForm = DevExpress.XtraSpreadsheet.Forms.PageSetupForm;
using PageSetupFormInitialTabPage = DevExpress.XtraSpreadsheet.Forms.PageSetupFormInitialTabPage;
using PasteSpecialForm = DevExpress.XtraSpreadsheet.Forms.PasteSpecialForm;
using PasteSpecialFormControllerParameters = DevExpress.XtraSpreadsheet.Forms.PasteSpecialFormControllerParameters;
using PasteSpecialInfo = DevExpress.XtraSpreadsheet.Forms.PasteSpecialInfo;
using DevExpress.XtraSpreadsheet.Internal;
namespace DevExpress.XtraSpreadsheet {
	partial class SpreadsheetControl {
		bool isMessageBoxShown;
		bool IsMessageBoxShown { get { return isMessageBoxShown; } }
		#region ISpreadsheetControl.Show(*)Message implementation
		DialogResult ISpreadsheetControl.ShowWarningMessage(string message) {
			return ShowMessage(message, Application.ProductName, MessageBoxIcon.Warning);
		}
		DialogResult ISpreadsheetControl.ShowMessage(string message, string title, MessageBoxIcon icon) {
			return ShowMessage(message, title, icon);
		}
		DialogResult ShowMessage(string message, string title, MessageBoxIcon icon) {
			if (IsDisposed)
				return DialogResult.None;
			this.isMessageBoxShown = true;
			try {
				return ShowMessageCore(message, title, icon);
			}
			finally {
				this.isMessageBoxShown = false;
			}
		}
		DialogResult ShowMessageCore(string message, string title, MessageBoxIcon icon) {
			IMessageBoxService service = GetService<IMessageBoxService>();
			if (service != null)
				return service.ShowMessage(message, title, icon);
			return XtraMessageBox.Show(LookAndFeel, this, message, title, MessageBoxButtons.OK, icon);
		}
		DialogResult ISpreadsheetControl.ShowDataValidationDialog(string message, string title, DataValidationErrorStyle errorStyle) {
			if (IsDisposed)
				return DialogResult.None;
			this.isMessageBoxShown = true;
			try {
				return ShowDataValidationDialogCore(message, title, errorStyle);
			}
			finally {
				this.isMessageBoxShown = false;
			}
		}
		DialogResult ShowDataValidationDialogCore(string message, string title, DataValidationErrorStyle errorStyle) {
			IMessageBoxService service = GetService<IMessageBoxService>();
			if (service != null)
				return service.ShowDataValidationDialog(message, title, (DevExpress.Spreadsheet.DataValidationErrorStyle)errorStyle);
			MessageBoxButtons buttons;
			MessageBoxIcon icon;
			if (errorStyle == DataValidationErrorStyle.Stop) {
				buttons = MessageBoxButtons.RetryCancel;
				icon = MessageBoxIcon.Error;
			}
			else if (errorStyle == DataValidationErrorStyle.Warning) {
				buttons = MessageBoxButtons.YesNoCancel;
				icon = MessageBoxIcon.Warning;
			}
			else {
				buttons = MessageBoxButtons.OKCancel;
				icon = MessageBoxIcon.Information;
			}
			return XtraMessageBox.Show(lookAndFeel, this, message, title, buttons, icon);
		}
		DialogResult ISpreadsheetControl.ShowYesNoCancelMessage(string message) {
			if (IsDisposed)
				return DialogResult.None;
			this.isMessageBoxShown = true;
			try {
				return ShowYesNoCancelMessageCore(message);
			}
			finally {
				this.isMessageBoxShown = false;
			}
		}
		DialogResult ShowYesNoCancelMessageCore(string message) {
			IMessageBoxService service = GetService<IMessageBoxService>();
			if (service != null)
				return service.ShowYesNoCancelMessage(message);
			return XtraMessageBox.Show(LookAndFeel, this, message, Application.ProductName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
		}
		bool ISpreadsheetControl.ShowOkCancelMessage(string message) {
			if (IsDisposed)
				return false;
			this.isMessageBoxShown = true;
			try {
				return ShowOkCancelMessageCore(message);
			}
			finally {
				this.isMessageBoxShown = false;
			}
		}
		bool ShowOkCancelMessageCore(string message) {
			IMessageBoxService service = GetService<IMessageBoxService>();
			if (service != null)
				return service.ShowOkCancelMessage(message);
			return DialogResult.OK == XtraMessageBox.Show(LookAndFeel, this, message, Application.ProductName, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
		}
		bool ISpreadsheetControl.ShowYesNoMessage(string message) {
			if (IsDisposed)
				return false;
			this.isMessageBoxShown = true;
			try {
				return ShowYesNoMessageCore(message);
			}
			finally {
				this.isMessageBoxShown = false;
			}
		}
		bool ShowYesNoMessageCore(string message) {
			IMessageBoxService service = GetService<IMessageBoxService>();
			if (service != null)
				return service.ShowYesNoMessage(message);
			return DialogResult.OK == XtraMessageBox.Show(LookAndFeel, this, message, Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
		}
		#endregion
		#region ShowModalForm
		DialogResult ShowModalForm(XtraForm form) {
			return ShowModalForm(form, this, true);
		}
		protected internal DialogResult ShowModalForm(XtraForm form, IWin32Window owner) {
			return ShowModalForm(form, owner, true);
		}
		protected internal DialogResult ShowModalForm(XtraForm form, IWin32Window owner, bool clearCopiedRange) {
			MenuManagerUtils.SetMenuManager(form.Controls, this.MenuManager);
			if (clearCopiedRange)
				DocumentModel.ClearCopiedRange();
			form.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			return FormTouchUIAdapter.ShowDialog(form, owner);
		}
		#endregion
		#region ShowModelessForm
		internal void ShowModelessForm(XtraForm form, Action action) {
			ShowModelessForm(form, action, true);
		}
		void ShowModelessForm(XtraForm form, Action action, bool clearCopiedRange) {
			form.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			MenuManagerUtils.SetMenuManager(form.Controls, this.MenuManager);
			form.Closed += (o, e) => {
				if (form.IsDisposed)
					return;
				if (form.DialogResult == DialogResult.OK && action != null)
					action();
				form.Dispose();
			};
			if (clearCopiedRange)
				DocumentModel.ClearCopiedRange();
			CorrectModelessFormStartPosition(form, this);
			FormTouchUIAdapter.Show(form, this);
		}
		static void CorrectModelessFormStartPosition(Form form, Control parent) {
			if (form.StartPosition == FormStartPosition.CenterParent) {
				form.StartPosition = FormStartPosition.Manual;
				Point pointToScreen = parent.PointToScreen(new Point(0, 0));
				form.Location = new Point(pointToScreen.X + (parent.Width - form.Width) / 2, pointToScreen.Y + (parent.Height - form.Height) / 2);
			}
		}
		#endregion
		#region ISpreadsheetControl.Show(*)Form implementations
		void ISpreadsheetControl.ShowFormatCellsForm(FormatCellsFormProperties properties, FormatCellsFormInitialTabPage initialTabPage, ShowFormatCellsFormCallback callback) {
			ShowFormatCellsForm(properties, initialTabPage, callback);
		}
		void ShowFormatCellsForm(FormatCellsFormProperties properties, FormatCellsFormInitialTabPage initialTabPage, ShowFormatCellsFormCallback callback) {
			FormatCellsModel model = new FormatCellsModel(this, properties, initialTabPage);
			FormatCellsViewModel viewModel = new FormatCellsViewModel(model);
			using (FormatCellsForm form = new FormatCellsForm(viewModel)) {
				DialogResult result = ShowModalForm(form);
				if (result == DialogResult.OK)
					callback(properties);
			}
		}
		void ISpreadsheetControl.ShowHyperlinkForm(IHyperlinkViewInfo hyperlink, ShowHyperlinkFormCallback callback) {
			ShowHyperlinkForm(hyperlink, callback);
		}
		void ShowHyperlinkForm(IHyperlinkViewInfo hyperlink, ShowHyperlinkFormCallback callback) {
			HyperlinkFormControllerParameters controllerParameters = new HyperlinkFormControllerParameters(this, hyperlink);
			HyperlinkFormShowingEventArgs args = new HyperlinkFormShowingEventArgs(controllerParameters);
			RaiseHyperlinkFormShowing(args);
			if (!args.Handled) {
				using (HyperlinkForm form = new HyperlinkForm(controllerParameters)) {
					DialogResult result = ShowModalForm(form);
					if (result == DialogResult.OK)
						callback(hyperlink);
				}
			}
			else {
				DialogResult result = args.DialogResult;
				if (result == DialogResult.OK)
					callback(controllerParameters.HyperlinkInfo);
			}
		}
		void ISpreadsheetControl.ShowRenameSheetForm(RenameSheetViewModel viewModel) {
			ShowRenameSheetForm(viewModel);
		}
		void ShowRenameSheetForm(RenameSheetViewModel viewModel) {
			using (RenameSheetForm form = new RenameSheetForm(viewModel)) {
				DialogResult result = ShowModalForm(form, this, false);
				if (result == DialogResult.OK)
					viewModel.ApplyChanges();
			}
		}
		void ISpreadsheetControl.ShowDataMemberEditorForm(DataMemberEditorViewModel viewModel) {
			ShowDataMemberEditorForm(viewModel);
		}
		void ShowDataMemberEditorForm(DataMemberEditorViewModel viewModel) {
			using (DataMemberEditorForm form = new DataMemberEditorForm(viewModel, this)) {
				DialogResult result = ShowModalForm(form);
				if (result == DialogResult.OK)
					viewModel.ApplyChanges();
			}
		}
		void ISpreadsheetControl.ShowSelectDataMemberForm(SelectDataMemberViewModel viewModel) {
			ShowSelectDataMemberForm(viewModel);
		}
		void ShowSelectDataMemberForm(SelectDataMemberViewModel viewModel) {
			using (SelectDataMemberForm form = new SelectDataMemberForm(viewModel, this)) {
				DialogResult result = ShowModalForm(form);
				if (result == DialogResult.OK)
					viewModel.ApplyChanges();
			}
		}
		void ISpreadsheetControl.ShowFilterEditorForm(FilterEditorViewModel viewModel) {
			ShowFilterEditorForm(viewModel);
		}
		void ShowFilterEditorForm(FilterEditorViewModel viewModel) {
			using (FilterEditorForm form = new FilterEditorForm(viewModel, this)) {
				DialogResult result = ShowModalForm(form);
				if (result == DialogResult.OK)
					viewModel.ApplyChanges();
			}
		}
		void ISpreadsheetControl.ShowGroupEditorForm(GroupEditorViewModel viewModel) {
			ShowGroupEditorForm(viewModel);
		}
		void ShowGroupEditorForm(GroupEditorViewModel viewModel) {
			using (GroupEditorForm form = new GroupEditorForm(viewModel, this)) {
				DialogResult result = ShowModalForm(form);
				if (result == DialogResult.OK)
					viewModel.ApplyChanges();
			}
		}
		void ISpreadsheetControl.ShowOutlineSubtotalForm(OutlineSubtotalViewModel viewModel) {
			ShowOutlineSubtotalForm(viewModel);
		}
		void ShowOutlineSubtotalForm(OutlineSubtotalViewModel viewModel) {
			using (OutlineSubtotalForm form = new OutlineSubtotalForm(viewModel)) {
				DialogResult result = ShowModalForm(form);
				if (result == DialogResult.OK)
					viewModel.ApplyChanges();
			}
		}
		void ISpreadsheetControl.ShowOutlineSettingsForm(OutlineSettingsViewModel viewModel) {
			ShowOutlineSettingsForm(viewModel);
		}
		void ShowOutlineSettingsForm(OutlineSettingsViewModel viewModel) {
			using (OutlineSettingsForm form = new OutlineSettingsForm(viewModel)) {
				DialogResult result = ShowModalForm(form);
				if (result == DialogResult.OK)
					viewModel.ApplyChanges();
			}
		}
		void ISpreadsheetControl.ShowGroupUngroupForm(GroupViewModel viewModel) {
			ShowGroupUngroupForm(viewModel);
		}
		void ShowGroupUngroupForm(GroupViewModel viewModel) {
			using (GroupUngroupForm form = new GroupUngroupForm(viewModel)) {
				DialogResult result = ShowModalForm(form);
				if (result == DialogResult.OK)
					viewModel.ApplyChanges();
			}
		}
		void ISpreadsheetControl.ShowGroupRangeEditorForm(GroupRangeEditorViewModel viewModel) {
			ShowGroupRangeEditorForm(viewModel);
		}
		void ShowGroupRangeEditorForm(GroupRangeEditorViewModel viewModel) {
			using (GroupRangeEditorForm form = new GroupRangeEditorForm(viewModel)) {
				DialogResult result = ShowModalForm(form);
				if (result == DialogResult.OK)
					viewModel.ApplyChanges();
			}
		}
		void ISpreadsheetControl.ShowMailMergePreviewForm() {
			ShowMailMergePreviewForm();
		}
		void ShowMailMergePreviewForm() {
			MailMergePreviewForm form = new MailMergePreviewForm(this);
			ShowModelessForm(form, null);
			form.FormClosing += mailMergePreviewForm_FormClosing;
		}
		void mailMergePreviewForm_FormClosing(object sender, FormClosingEventArgs e) {
			MailMergePreviewForm form = (sender as MailMergePreviewForm);
			if (form != null)
				form.Dispose();
		}
		void ISpreadsheetControl.ShowConditionalFormattingTop10RuleForm(ConditionalFormattingTopBottomRuleViewModel viewModel) {
			ShowConditionalFormattingTop10RuleForm(viewModel);
		}
		void ShowConditionalFormattingTop10RuleForm(ConditionalFormattingTopBottomRuleViewModel viewModel) {
			using (ConditionalFormattingTopBottomRuleForm form = new ConditionalFormattingTopBottomRuleForm(viewModel)) {
				ShowModalForm(form);
			}
		}
		void ISpreadsheetControl.ShowConditionalFormattingExpressionRuleForm(ConditionalFormattingHighlightCellsRuleViewModel viewModel) {
			ShowConditionalFormattingExpressionRuleForm(viewModel);
		}
		void ShowConditionalFormattingExpressionRuleForm(ConditionalFormattingHighlightCellsRuleViewModel viewModel) {
			ConditionalFormattingHighlightCellsRuleForm form = new ConditionalFormattingHighlightCellsRuleForm(viewModel);
			ShowModelessForm(form, null);
		}
		void ISpreadsheetControl.ShowConditionalFormattingTextRuleForm(ConditionalFormattingTextRuleViewModel viewModel) {
			ShowConditionalFormattingTextRuleForm(viewModel);
		}
		void ShowConditionalFormattingTextRuleForm(ConditionalFormattingTextRuleViewModel viewModel) {
			ConditionalFormattingHighlightCellsRuleForm form = new ConditionalFormattingHighlightCellsRuleForm(viewModel);
			ShowModelessForm(form, null);
		}
		void ISpreadsheetControl.ShowConditionalFormattingBetweenRuleForm(ConditionalFormattingBetweenRuleViewModel viewModel) {
			ShowConditionalFormattingBetweenRuleForm(viewModel);
		}
		void ShowConditionalFormattingBetweenRuleForm(ConditionalFormattingBetweenRuleViewModel viewModel) {
			ConditionalFormattingBetweenRuleForm form = new ConditionalFormattingBetweenRuleForm(viewModel);
			ShowModelessForm(form, null);
		}
		void ISpreadsheetControl.ShowConditionalFormattingDuplicateValuesRuleForm(ConditionalFormattingDuplicateValuesRuleViewModel viewModel) {
			ShowConditionalFormattingDuplicateValuesRuleForm(viewModel);
		}
		void ShowConditionalFormattingDuplicateValuesRuleForm(ConditionalFormattingDuplicateValuesRuleViewModel viewModel) {
			using (ConditionalFormattingDuplicateValuesRuleForm form = new ConditionalFormattingDuplicateValuesRuleForm(viewModel)) {
				ShowModalForm(form);
			}
		}
		void ISpreadsheetControl.ShowConditionalFormattingDateOccurringRuleForm(ConditionalFormattingDateOccurringRuleViewModel viewModel) {
			ShowConditionalFormattingDateOccurringRuleForm(viewModel);
		}
		void ShowConditionalFormattingDateOccurringRuleForm(ConditionalFormattingDateOccurringRuleViewModel viewModel) {
			using (ConditionalFormattingDateOccurringRuleForm form = new ConditionalFormattingDateOccurringRuleForm(viewModel)) {
				ShowModalForm(form);
			}
		}
		void ISpreadsheetControl.ShowConditionalFormattingAverageRuleForm(ConditionalFormattingAverageRuleViewModel viewModel) {
			ShowConditionalFormattingAverageRuleForm(viewModel);
		}
		void ShowConditionalFormattingAverageRuleForm(ConditionalFormattingAverageRuleViewModel viewModel) {
			using (ConditionalFormattingAverageRuleForm form = new ConditionalFormattingAverageRuleForm(viewModel)) {
				ShowModalForm(form);
			}
		}
		void ISpreadsheetControl.ShowUnhideSheetForm(UnhideSheetViewModel viewModel) {
			ShowUnhideSheetForm(viewModel);
		}
		void ShowUnhideSheetForm(UnhideSheetViewModel viewModel) {
			using (UnhideSheetForm form = new UnhideSheetForm(viewModel)) {
				ShowModalForm(form);
			}
		}
		void ISpreadsheetControl.ShowPasteSpecialLocalForm(ModelPasteSpecialOptions properties, ShowPasteSpecialFormLocalCallback callback, object callbackData) {
			PasteSpecialLocalFormControllerParameters controllerParameters = new PasteSpecialLocalFormControllerParameters(this, properties);
			using (PasteSpecialLocalForm form = new PasteSpecialLocalForm(controllerParameters)) {
				DialogResult result = ShowModalForm(form);
				if (result == DialogResult.OK)
					callback(form.Controller.SourcePasteSpecialOptions);
			}
		}
		void ISpreadsheetControl.ShowPasteSpecialForm(PasteSpecialInfo properties, ShowPasteSpecialFormCallback callback, object callbackData) {
			this.ShowPasteSpecialForm(properties, callback, callbackData);
		}
		void ShowPasteSpecialForm(PasteSpecialInfo properties, ShowPasteSpecialFormCallback callback, object callbackData) {
			PasteSpecialFormControllerParameters controllerParameters = new PasteSpecialFormControllerParameters(this, properties);
			using (PasteSpecialForm form = new PasteSpecialForm(controllerParameters)) {
				DialogResult result = ShowModalForm(form);
				if (result == DialogResult.OK)
					callback(form.Controller.SourcePasteSpecialInfo, callbackData);
			}
		}
		void ISpreadsheetControl.ShowMoveOrCopySheetForm(MoveOrCopySheetViewModel viewModel) {
			ShowMoveOrCopySheetForm(viewModel);
		}
		void ShowMoveOrCopySheetForm(MoveOrCopySheetViewModel viewModel) {
			using (MoveOrCopySheetForm form = new MoveOrCopySheetForm(viewModel)) {
				DialogResult result = ShowModalForm(form);
				if (result == DialogResult.OK)
					viewModel.ApplyChanges();
			}
		}
		void ISpreadsheetControl.ShowTableInsertForm(InsertTableViewModel viewModel) {
			InsertTableForm form = new InsertTableForm(viewModel);
			ShowModelessForm(form, () => { viewModel.ApplyChanges(); });
		}
		void ISpreadsheetControl.ShowChangeChartTypeForm(ChangeChartTypeViewModel viewModel) {
			this.ShowChangeChartTypeForm(viewModel);
		}
		void ShowChangeChartTypeForm(ChangeChartTypeViewModel viewModel) {
			using (ChangeChartTypeForm form = new ChangeChartTypeForm(viewModel)) {
				ShowModalForm(form);
			}
		}
		void ISpreadsheetControl.ShowChangeChartTitleForm(ChangeChartTitleViewModel viewModel) {
			this.ShowChangeChartTitleForm(viewModel);
		}
		void ShowChangeChartTitleForm(ChangeChartTitleViewModel viewModel) {
			using (ChangeChartTitleForm form = new ChangeChartTitleForm(viewModel)) {
				ShowModalForm(form);
			}
		}
		void ISpreadsheetControl.ShowChangeChartHorizontalAxisTitleForm(ChangeChartHorizontalAxisTitleViewModel viewModel) {
			this.ShowChangeChartHorizontalAxisTitleForm(viewModel);
		}
		void ShowChangeChartHorizontalAxisTitleForm(ChangeChartHorizontalAxisTitleViewModel viewModel) {
			using (ChangeChartHorizontalAxisTitleForm form = new ChangeChartHorizontalAxisTitleForm(viewModel)) {
				ShowModalForm(form);
			}
		}
		void ISpreadsheetControl.ShowChangeChartVerticalAxisTitleForm(ChangeChartVerticalAxisTitleViewModel viewModel) {
			this.ShowChangeChartVerticalAxisTitleForm(viewModel);
		}
		void ShowChangeChartVerticalAxisTitleForm(ChangeChartVerticalAxisTitleViewModel viewModel) {
			using (ChangeChartVerticalAxisTitleForm form = new ChangeChartVerticalAxisTitleForm(viewModel)) {
				ShowModalForm(form);
			}
		}
		void ISpreadsheetControl.ShowChartSelectDataForm(ChartSelectDataViewModel viewModel) {
			ChartSelectDataForm form = new ChartSelectDataForm(viewModel);
			ShowModelessForm(form, null);
		}
		void ISpreadsheetControl.ShowProtectSheetForm(ProtectSheetViewModel viewModel) {
			this.ShowProtectSheetForm(viewModel);
		}
		void ShowProtectSheetForm(ProtectSheetViewModel viewModel) {
			using (ProtectSheetForm form = new ProtectSheetForm(viewModel)) {
				ShowModalForm(form);
			}
		}
		void ISpreadsheetControl.ShowProtectWorkbookForm(ProtectWorkbookViewModel viewModel) {
			this.ShowProtectWorkbookForm(viewModel);
		}
		void ShowProtectWorkbookForm(ProtectWorkbookViewModel viewModel) {
			using (ProtectWorkbookForm form = new ProtectWorkbookForm(viewModel)) {
				ShowModalForm(form);
			}
		}
		void ISpreadsheetControl.ShowUnprotectSheetForm(UnprotectSheetViewModel viewModel) {
			ShowUnprotectSheetForm(viewModel);
		}
		void ShowUnprotectSheetForm(UnprotectSheetViewModel viewModel) {
			using (UnprotectSheetForm form = new UnprotectSheetForm(viewModel)) {
				ShowModalForm(form);
			}
		}
		void ISpreadsheetControl.ShowUnprotectWorkbookForm(UnprotectWorkbookViewModel viewModel) {
			ShowUnprotectWorkbookForm(viewModel);
		}
		void ShowUnprotectWorkbookForm(UnprotectWorkbookViewModel viewModel) {
			using (UnprotectWorkbookForm form = new UnprotectWorkbookForm(viewModel)) {
				ShowModalForm(form);
			}
		}
		void ISpreadsheetControl.ShowUnprotectRangeForm(UnprotectRangeViewModel viewModel) {
			ShowUnprotectRangeForm(viewModel);
		}
		void ShowUnprotectRangeForm(UnprotectRangeViewModel viewModel) {
			using (UnprotectRangeForm form = new UnprotectRangeForm(viewModel)) {
				ShowModalForm(form);
			}
		}
		void ISpreadsheetControl.ShowInsertFunctionForm(InsertFunctionViewModel viewModel) {
			ShowInsertFunctionForm(viewModel);
		}
		void ShowInsertFunctionForm(InsertFunctionViewModel viewModel) {
			using (InsertFunctionForm form = new InsertFunctionForm(viewModel)) {
				DialogResult result = ShowModalForm(form);
				if (result == DialogResult.OK)
					viewModel.ApplyChanges();
				else
					viewModel.DiscardChanges();
			}
		}
		void ISpreadsheetControl.ShowInsertSymbolForm(InsertSymbolViewModel viewModel) {
			ShowInsertSymbolForm(viewModel);
		}
		void ShowInsertSymbolForm(InsertSymbolViewModel viewModel) {
			viewModel.ModelessBehavior = true;
			SymbolForm form = new SymbolForm(viewModel);
			ShowModelessForm(form, null);
		}
		void ISpreadsheetControl.ShowFindReplaceForm(FindReplaceViewModel viewModel) {
			ShowFindReplaceForm(viewModel);
		}
		void ShowFindReplaceForm(FindReplaceViewModel viewModel) {
			FindReplaceForm form = new FindReplaceForm(viewModel);
			ShowModelessForm(form, null, false);
		}
		void ISpreadsheetControl.ShowFunctionArgumentsForm(FunctionArgumentsViewModel viewModel) {
			ShowFunctionArgumentsForm(viewModel);
		}
		void ShowFunctionArgumentsForm(FunctionArgumentsViewModel viewModel) {
			FunctionArgumentsForm form = new FunctionArgumentsForm(viewModel);
			ShowModelessForm(form, null);
		}
		void ISpreadsheetControl.ShowDefineNameForm(DefineNameViewModel viewModel) {
			ShowDefineNameForm(viewModel);
		}
		protected internal DefineNameForm ShowDefineNameForm(DefineNameViewModel viewModel) {
			DefineNameForm form = new DefineNameForm(viewModel);
			ShowModelessForm(form, null);
			return form;
		}
		void ISpreadsheetControl.ShowNameManagerForm(NameManagerViewModel viewModel) {
			ShowNameManagerForm(viewModel);
		}
		void ShowNameManagerForm(NameManagerViewModel viewModel) {
			NameManagerForm form = new NameManagerForm(viewModel);
			ShowModelessForm(form, null);
		}
		void ISpreadsheetControl.ShowCreateDefinedNamesFromSelectionForm(CreateDefinedNamesFromSelectionViewModel viewModel) {
			ShowCreateDefinedNamesFromSelectionForm(viewModel);
		}
		void ShowCreateDefinedNamesFromSelectionForm(CreateDefinedNamesFromSelectionViewModel viewModel) {
			using (CreateDefinedNamesFromSelectionForm form = new CreateDefinedNamesFromSelectionForm(viewModel)) {
				ShowModalForm(form);
			}
		}
		void ISpreadsheetControl.ShowProtectedRangeForm(ProtectedRangeViewModel viewModel) {
			ShowProtectedRangeForm(viewModel);
		}
		protected internal ProtectRangeForm ShowProtectedRangeForm(ProtectedRangeViewModel viewModel) {
			ProtectRangeForm form = new ProtectRangeForm(viewModel);
			ShowModelessForm(form, null);
			return form;
		}
		void ISpreadsheetControl.ShowProtectedRangePermissionsForm(ProtectedRangePermissionsViewModel viewModel) {
			ShowProtectedRangePermissionsForm(viewModel);
		}
		void ShowProtectedRangePermissionsForm(ProtectedRangePermissionsViewModel viewModel) {
			ProtectedRangeSecurityInformation securityInformation = new ProtectedRangeSecurityInformation();
			securityInformation.SecurityDescriptor = viewModel.SecurityDescriptor;
			securityInformation.ObjectName = viewModel.Name;
			EditSecurityForm form = new EditSecurityForm();
			form.ShowDialog(this, securityInformation);
			viewModel.SecurityDescriptor = securityInformation.SecurityDescriptor;
		}
		void ISpreadsheetControl.ShowProtectedRangeManagerForm(ProtectedRangeManagerViewModel viewModel) {
			ShowProtectedRangeManagerForm(viewModel);
		}
		void ShowProtectedRangeManagerForm(ProtectedRangeManagerViewModel viewModel) {
			ProtectedRangeManagerForm form = new ProtectedRangeManagerForm(viewModel);
			ShowModelessForm(form, null);
		}
		void ISpreadsheetControl.ShowRowHeightForm(RowHeightViewModel viewModel) {
			ShowRowHeightForm(viewModel);
		}
		void ShowRowHeightForm(RowHeightViewModel viewModel) {
			using (RowHeightForm form = new RowHeightForm(viewModel)) {
				ShowModalForm(form);
			}
		}
		void ISpreadsheetControl.ShowColumnWidthForm(ColumnWidthViewModel viewModel) {
			ShowColumnWidthForm(viewModel);
		}
		void ShowColumnWidthForm(ColumnWidthViewModel viewModel) {
			using (ColumnWidthForm form = new ColumnWidthForm(viewModel)) {
				ShowModalForm(form);
			}
		}
		void ISpreadsheetControl.ShowDefaultColumnWidthForm(DefaultColumnWidthViewModel viewModel) {
			ShowDefaultColumnWidthForm(viewModel);
		}
		void ShowDefaultColumnWidthForm(DefaultColumnWidthViewModel viewModel) {
			using (DefaultColumnWidthForm form = new DefaultColumnWidthForm(viewModel)) {
				ShowModalForm(form);
			}
		}
		void ISpreadsheetControl.ShowDocumentPropertiesForm(DocumentPropertiesViewModel viewModel) {
			ShowDocumentPropertiesForm(viewModel);
		}
		void ShowDocumentPropertiesForm(DocumentPropertiesViewModel viewModel) {
			using (DocumentPropertiesForm form = new DocumentPropertiesForm(viewModel)) {
				ShowModalForm(form);
			}
		}
		void ISpreadsheetControl.ShowAutoFilterForm(AutoFilterViewModel viewModel) {
			ShowAutoFilterForm(viewModel);
		}
		void ShowAutoFilterForm(AutoFilterViewModel viewModel) {
			this.ShowAutoFilterPopupMenu(viewModel);
		}
		void ISpreadsheetControl.ShowGenericFilterForm(GenericFilterViewModel viewModel) {
			this.ShowGenericFilterForm(viewModel);
		}
		void ShowGenericFilterForm(GenericFilterViewModel viewModel) {
			using (GenericFilterForm form = new GenericFilterForm(viewModel)) {
				ShowModalForm(form);
			}
		}
		void ISpreadsheetControl.ShowTop10FilterForm(Top10FilterViewModel viewModel) {
			this.ShowTop10FilterForm(viewModel);
		}
		void ShowTop10FilterForm(Top10FilterViewModel viewModel) {
			using (Top10FilterForm form = new Top10FilterForm(viewModel)) {
				ShowModalForm(form);
			}
		}
		void ISpreadsheetControl.ShowSimpleFilterForm(SimpleFilterViewModel viewModel) {
			this.ShowSimpleFilterForm(viewModel);
		}
		void ShowSimpleFilterForm(SimpleFilterViewModel viewModel) {
			using (SimpleFilterForm form = new SimpleFilterForm(viewModel)) {
				ShowModalForm(form);
			}
		}
		void ISpreadsheetControl.ShowPageSetupForm(PageSetupViewModel viewModel, PageSetupFormInitialTabPage initialTabPage) {
			ShowPageSetupForm(viewModel, initialTabPage);
		}
		void ShowPageSetupForm(PageSetupViewModel viewModel, PageSetupFormInitialTabPage initialTabPage) {
			using (PageSetupForm form = new PageSetupForm(viewModel)) {
				ShowModalForm(form);
			}
		}
		void ISpreadsheetControl.ShowHeaderFooterForm(HeaderFooterViewModel viewModel) {
			ShowHeaderFooterForm(viewModel);
		}
		protected internal void ShowHeaderFooterForm(HeaderFooterViewModel viewModel) {
			using (HeaderFooterForm form = new HeaderFooterForm(viewModel)) {
				ShowModalForm(form);
			}
		}
		void ISpreadsheetControl.ShowDataValidationForm(DataValidationViewModel viewModel) {
			DataValidationForm form = new DataValidationForm(viewModel);
			ShowModelessForm(form, () => { viewModel.ApplyChanges(); });
		}
		void ISpreadsheetControl.ShowInsertPivotTableForm(InsertPivotTableViewModel viewModel) {
			ShowInsertPivotTableForm(viewModel);
		}
		void ShowInsertPivotTableForm(InsertPivotTableViewModel viewModel) {
			InsertPivotTableForm form = new InsertPivotTableForm(viewModel);
			ShowModelessForm(form, null);
		}
		void ISpreadsheetControl.ShowOptionsPivotTableForm(OptionsPivotTableViewModel viewModel) {
			ShowOptionsPivotTableForm(viewModel);
		}
		void ShowOptionsPivotTableForm(OptionsPivotTableViewModel viewModel) {
			using (OptionsPivotTableForm form = new OptionsPivotTableForm(viewModel)) {
				ShowModalForm(form);
			}
		}
		void ISpreadsheetControl.ShowMovePivotTableForm(MovePivotTableViewModel viewModel) {
			ShowMovePivotTableForm(viewModel);
		}
		void ShowMovePivotTableForm(MovePivotTableViewModel viewModel) {
			MovePivotTableForm form = new MovePivotTableForm(viewModel);
			ShowModelessForm(form, null);
		}
		void ISpreadsheetControl.ShowChangeDataSourcePivotTableForm(ChangeDataSourcePivotTableViewModel viewModel) {
			ShowChangeDataSourcePivotTableForm(viewModel);
		}
		void ShowChangeDataSourcePivotTableForm(ChangeDataSourcePivotTableViewModel viewModel) {
			ChangeDataSourcePivotTableForm form = new ChangeDataSourcePivotTableForm(viewModel);
			ShowModelessForm(form, null);
		}
		void ISpreadsheetControl.ShowFieldSettingsPivotTableForm(FieldSettingsPivotTableViewModel viewModel) {
			ShowFieldSettingsPivotTableForm(viewModel);
		}
		void ShowFieldSettingsPivotTableForm(FieldSettingsPivotTableViewModel viewModel) {
			using (FieldSettingsPivotTableForm form = new FieldSettingsPivotTableForm(viewModel)) {
				ShowModalForm(form);
			}
		}
		void ISpreadsheetControl.ShowDataFieldSettingsPivotTableForm(DataFieldSettingsPivotTableViewModel viewModel) {
			ShowDataFieldSettingsPivotTableForm(viewModel);
		}
		void ShowDataFieldSettingsPivotTableForm(DataFieldSettingsPivotTableViewModel viewModel) {
			using (DataFieldSettingsPivotTableForm form = new DataFieldSettingsPivotTableForm(viewModel)) {
				ShowModalForm(form);
			}
		}
		void ISpreadsheetControl.ShowPivotTableFieldsFilterItemsForm(PivotTableFieldsFilterItemsViewModel viewModel) {
			this.ShowPivotTableFieldsFilterItemsForm(viewModel);
		}
		void ShowPivotTableFieldsFilterItemsForm(PivotTableFieldsFilterItemsViewModel viewModel) {
			using (PivotTableFieldsFilterItemsForm form = new PivotTableFieldsFilterItemsForm(viewModel)) {
				ShowModalForm(form);
			}
		}
		void ISpreadsheetControl.ShowPivotTableAutoFilterForm() {
			ShowPivotTableAutoFilterForm();
		}
		void ShowPivotTableAutoFilterForm() {
			this.ShowPivotTableAutoFilterPopupMenu();
		}
		void ISpreadsheetControl.ShowPivotTableValueFilterForm(PivotTableValueFiltersViewModel viewModel) {
			this.ShowPivotTableValueFilterForm(viewModel);
		}
		void ShowPivotTableValueFilterForm(PivotTableValueFiltersViewModel viewModel) {
			using (PivotTableValueFilterForm form = new PivotTableValueFilterForm(viewModel)) {
				ShowModalForm(form);
			}
		}
		void ISpreadsheetControl.ShowPivotTableTop10FilterForm(PivotTableTop10FiltersViewModel viewModel) {
			this.ShowPivotTableTop10FilterForm(viewModel);
		}
		void ShowPivotTableTop10FilterForm(PivotTableTop10FiltersViewModel viewModel) {
			using (PivotTableTop10FilterForm form = new PivotTableTop10FilterForm(viewModel)) {
				ShowModalForm(form);
			}
		}
		void ISpreadsheetControl.ShowPivotTableDateFilterForm(PivotTableDateFiltersViewModel viewModel) {
			this.ShowPivotTableDateFilterForm(viewModel);
		}
		void ShowPivotTableDateFilterForm(PivotTableDateFiltersViewModel viewModel) {
			using (PivotTableDateFilterForm form = new PivotTableDateFilterForm(viewModel)) {
				ShowModalForm(form);
			}
		}
		void ISpreadsheetControl.ShowPivotTableLabelFilterForm(PivotTableLabelFiltersViewModel viewModel) {
			this.ShowPivotTableLabelFilterForm(viewModel);
		}
		void ShowPivotTableLabelFilterForm(PivotTableLabelFiltersViewModel viewModel) {
			using (PivotTableLabelFilterForm form = new PivotTableLabelFilterForm(viewModel)) {
				ShowModalForm(form);
			}
		}
		void ISpreadsheetControl.ShowPivotTableShowValuesAsForm(PivotTableShowValuesAsViewModel viewModel) {
			this.ShowPivotTableShowValuesAsForm(viewModel);
		}
		void ShowPivotTableShowValuesAsForm(PivotTableShowValuesAsViewModel viewModel) {
			using (PivotTableShowValuesAsForm form = new PivotTableShowValuesAsForm(viewModel)) {
				ShowModalForm(form);
			}
		}
		void ISpreadsheetControl.ShowAddDataSourceForm(Action<object> callback) {
			this.ShowAddDataSourceForm(callback);
		}
		void ShowAddDataSourceForm(Action<object> callback) {
			DataSourceWizardRunner<DataSourceModel> runner = new DataSourceWizardRunner<DataSourceModel>(this.LookAndFeel, this);
			ConnectionStorageService connectionStorage = new ConnectionStorageService();
			RuntimeConnectionStringsProvider connectionStrings = new RuntimeConnectionStringsProvider();
			IParameterService parameterService = DocumentModel.MailMergeParameters.GetParameterService();
			ISolutionTypesProvider solutionTypesProvider = EntityServiceHelper.GetRuntimeSolutionProvider(Assembly.GetEntryAssembly());
			SqlWizardOptions sqlWizardOptions = CreateSqlWizardOptions();
			DataSourceWizardClientUI client = new DataSourceWizardClientUI(
				connectionStorage,
				parameterService,
				solutionTypesProvider,
				connectionStrings,
				new DBSchemaProvider(),
				new DataSourceNameCreationService(),
				null,
				OperationMode.DataOnly,
				sqlWizardOptions,
				null
				);
			client.DataSourceTypes = GetDataSourceTypes();
			if (runner.Run(client)) {
				IDataComponent component = new DataComponentCreator().CreateDataComponent(runner.WizardModel);
				DocumentModel.MailMergeParameters.UpdateFromParameterService(parameterService); 
				callback(component);
			}
		}
		DataSourceTypes GetDataSourceTypes() {
			DataSourceTypes result = new DataSourceTypes();
			result.Clear();
			if ((Options.DataSourceWizard.DataSourceTypes & Office.Options.DataSourceTypes.Object) == Office.Options.DataSourceTypes.Object)
				result.Add(DataSourceType.Object);
			if ((Options.DataSourceWizard.DataSourceTypes & Office.Options.DataSourceTypes.Excel) == Office.Options.DataSourceTypes.Excel)
				result.Add(DataSourceType.Excel);
			if ((Options.DataSourceWizard.DataSourceTypes & Office.Options.DataSourceTypes.EntityFramework) == Office.Options.DataSourceTypes.EntityFramework)
				result.Add(DataSourceType.Entity);
			if ((Options.DataSourceWizard.DataSourceTypes & Office.Options.DataSourceTypes.Sql) == Office.Options.DataSourceTypes.Sql)
				result.Add(DataSourceType.Xpo);
			return result;
		}
		SqlWizardOptions CreateSqlWizardOptions() {
			SqlWizardSettings sqlWizardSettings = new SqlWizardSettings();
			sqlWizardSettings.AlwaysSaveCredentials = Options.DataSourceWizard.AlwaysSaveCredentials;
			sqlWizardSettings.DisableNewConnections = Options.DataSourceWizard.DisableNewConnections;
			sqlWizardSettings.EnableCustomSql = Options.DataSourceWizard.EnableCustomSql;
			sqlWizardSettings.QueryBuilderLight = Options.DataSourceWizard.QueryBuilderLight;
			return sqlWizardSettings.ToSqlWizardOptions();
		}
		class DataSourceNameCreationService : IDataSourceNameCreationService { 
			public string CreateName() {
				return "Data Source";
			}
			public bool ValidateName(string name) {
				return true;
			}
		}
		void ISpreadsheetControl.ShowSelectDataSourceForm(SelectDataSourceViewModel viewModel) {
			this.ShowSelectDataSourceForm(viewModel);
		}
		void ShowSelectDataSourceForm(SelectDataSourceViewModel viewModel) {
			using (SelectDataSourceForm form = new SelectDataSourceForm(viewModel)) {
				ShowModalForm(form);
			}
		}
		void ISpreadsheetControl.ShowManageQueriesForm() {
			this.ShowManageQueriesForm();
		}
		void ShowManageQueriesForm() {
			SqlDataSource sqlDataSource = this.DocumentModel.MailMergeDataSource as SqlDataSource;
			if (sqlDataSource == null)
				return;
			List<SqlQuery> oldValues = new List<SqlQuery>();
			oldValues.AddRange(sqlDataSource.Queries);
			IDBSchemaProvider schemaProvider = new DBSchemaProvider();
			IParameterService parameterService = DocumentModel.MailMergeParameters.GetParameterService();
			SqlWizardOptions sqlWizardOptions = CreateSqlWizardOptions();
			if (sqlDataSource.ManageQueries(new EditQueryContext { LookAndFeel = LookAndFeel, Owner = this, DBSchemaProvider = schemaProvider, ParameterService = parameterService, Options = sqlWizardOptions })) {
				MailMergeManageQueriesCommand command = new MailMergeManageQueriesCommand(this);
				DocumentModel.MailMergeParameters.UpdateFromParameterService(parameterService); 
				command.ApplyChanges(oldValues);
			}
		}
		void ISpreadsheetControl.ShowManageRelationsForm() {
			this.ShowManageRelationsForm();
		}
		void ShowManageRelationsForm() {
			SqlDataSource sqlDataSource = this.DocumentModel.MailMergeDataSource as SqlDataSource;
			if (sqlDataSource == null)
				return;
			List<MasterDetailInfo> oldValues = new List<MasterDetailInfo>();
			oldValues.AddRange(sqlDataSource.Relations);
			if (sqlDataSource.ManageRelations(new ManageRelationsContext{ LookAndFeel = LookAndFeel, Owner = this })) {
				MailMergeManageRelationsCommand command = new MailMergeManageRelationsCommand(this);
				command.ApplyChanges(oldValues);
			}
		}
		void ISpreadsheetControl.ShowManageDataSourcesForm(ManageDataSourcesViewModel viewModel) {
			this.ShowManageDataSourcesForm(viewModel);
		}
		void ShowManageDataSourcesForm(ManageDataSourcesViewModel viewModel) {
			using (ManageDataSourcesForm form = new ManageDataSourcesForm(viewModel)) {
				ShowModalForm(form);
			}
		}
		IPivotTableFieldsPanel ISpreadsheetControl.CreatePivotTableFieldsPanel() {
			return new FieldsPanelPivotTableForm(this);
		}
		#endregion
	}
}
