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
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Spreadsheet.Forms;
using DevExpress.Office.Forms;
using DevExpress.XtraSpreadsheet;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Forms;
using DevExpress.XtraSpreadsheet.Services;
using DevExpress.Xpf.Spreadsheet.Services;
#if !SL
using PlatformDialogResult = System.Windows.Forms.DialogResult;
using DevExpress.Xpf.Spreadsheet.Localization;
using System.ComponentModel;
using DevExpress.Xpf.Spreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Internal;
#endif
namespace DevExpress.Xpf.Spreadsheet {
	partial class SpreadsheetControl {
		#region ISpreadsheetControl.Show(*)Message
		PlatformDialogResult ISpreadsheetControl.ShowMessage(string message, string title, System.Windows.Forms.MessageBoxIcon icon) {
			return this.ShowMessage(message, title, icon);
		}
		PlatformDialogResult ShowMessage(string message, string title, System.Windows.Forms.MessageBoxIcon icon) {
			IMessageBoxService service = GetService<IMessageBoxService>();
			if (service != null)
				return service.ShowMessage(message, title, icon);
			else {
#if SL
			DXDialog dialog = new DXDialog();
			dialog.Title = XpfRichEditLocalizer.GetString(RichEditControlStringId.Msg_Warning);
			dialog.Buttons = DialogButtons.Ok;
			dialog.Content = message;
			dialog.IsSizable = false;
			dialog.Padding = new Thickness(20);
			dialog.ShowDialog();
#else
				DXMessageBox.Show(message, title, MessageBoxButton.OK, (MessageBoxImage)icon);
#endif
				return PlatformDialogResult.OK;
			}
		}
		PlatformDialogResult ISpreadsheetControl.ShowWarningMessage(string message) {
			return ShowWarningMessage(message);
		}
		PlatformDialogResult ShowWarningMessage(string message) {
			return this.ShowMessage(message, System.Windows.Forms.Application.ProductName, System.Windows.Forms.MessageBoxIcon.Warning);
		}
		PlatformDialogResult ISpreadsheetControl.ShowDataValidationDialog(string message, string title, DataValidationErrorStyle errorStyle) {
			IMessageBoxService service = GetService<IMessageBoxService>();
			if (service != null)
				return service.ShowDataValidationDialog(message, title, (DevExpress.Spreadsheet.DataValidationErrorStyle)errorStyle);
			else {
				if (errorStyle == DataValidationErrorStyle.Stop) {
					MessageBoxResult result = DXMessageBox.Show(message, title, MessageBoxButton.OKCancel, MessageBoxImage.Stop);
					return result == MessageBoxResult.OK ? PlatformDialogResult.No : (PlatformDialogResult)result;
				}
				if (errorStyle == DataValidationErrorStyle.Warning)
					return (PlatformDialogResult)DXMessageBox.Show(message, title, MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
				return (PlatformDialogResult)DXMessageBox.Show(message, title, MessageBoxButton.OKCancel, MessageBoxImage.Information);
			}
		}
		bool ISpreadsheetControl.ShowOkCancelMessage(string message) {
			IMessageBoxService service = GetService<IMessageBoxService>();
			if (service != null)
				return service.ShowOkCancelMessage(message);
			else
				return DXMessageBox.Show(message, System.Windows.Forms.Application.ProductName, MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK;
		}
		bool ISpreadsheetControl.ShowYesNoMessage(string message) {
			IMessageBoxService service = GetService<IMessageBoxService>();
			if (service != null)
				return service.ShowYesNoMessage(message);
			else
				return DXMessageBox.Show(message, System.Windows.Forms.Application.ProductName, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes;
		}
		PlatformDialogResult ISpreadsheetControl.ShowYesNoCancelMessage(string message) {
			IMessageBoxService service = GetService<IMessageBoxService>();
			if (service != null)
				return service.ShowYesNoCancelMessage(message);
			else {
				DXMessageBox.Show(message, System.Windows.Forms.Application.ProductName, MessageBoxButton.YesNoCancel, MessageBoxImage.Information);
				return PlatformDialogResult.Yes;
			}
		}
		#endregion
		#region ShowDialog
		void ShowDXDialog(UserControl form, DialogClosedDelegate onDialogClosed, string title, Func<bool> onOkClickDelegate = null) {
			CustomDXDialog dialog = new CustomDXDialog(title, DialogButtons.OkCancel);
			SetupDXDialog(dialog, form, onDialogClosed, onOkClickDelegate);
			dialog.ShowDialog();
		}
		void SetupDXDialog(CustomDXDialog dialog, UserControl form, DialogClosedDelegate onDialogClosed, Func<bool> onOkClickDelegate = null) {
			SetupDXWindow(dialog, SizeToContent.WidthAndHeight, WindowStartupLocation.CenterOwner, ResizeMode.NoResize);
			dialog.Content = form;
			if (onDialogClosed != null) {
				dialog.Closed += (s, e) => {
					onDialogClosed.Invoke(dialog.DialogResult);
				};
			}
			if (onOkClickDelegate != null) {
				dialog.OkButtonClick += (s, e) => {
					e.Cancel = !onOkClickDelegate.Invoke();
				};
			}
		}
		internal void ShowDXWindow(DXWindow window, string title, SizeToContent sizeToContent, WindowStartupLocation startupLocation, ResizeMode resizeMode) {
			SetupDXWindow(window, sizeToContent, startupLocation, resizeMode);
			window.Title = title;
			window.Show();
		}
		void SetupDXWindow(DXWindow window, SizeToContent sizeToContent, WindowStartupLocation startupLocation, ResizeMode resizeMode) {
			window.SizeToContent = sizeToContent;
			window.ResizeMode = resizeMode;
			window.WindowStartupLocation = startupLocation;
			TrySetDialogOwnerWindow(window);
			window.ShowIcon = false;
			ApplyDialogThemeName(window); 
			DocumentModel.ClearCopiedRange();
		}
		void ApplyDialogThemeName(DXWindow window) {
			string themeName = this.GetValue(ThemeManager.ThemeNameProperty) as string;
			if (!String.IsNullOrEmpty(themeName))
				window.SetValue(ThemeManager.ThemeNameProperty, themeName);
		}
		void TrySetDialogOwnerWindow(DXWindow window) {
			Window owner = LayoutHelper.FindParentObject<Window>(this);
			if (owner != null)
				window.Owner = owner;
		}
#if !SL
		void OnDialogClosed(object sender, RoutedEventArgs e) {
			FloatingContainer container = sender as FloatingContainer;
			if (container == null)
				return;
			try {
				container.Hidden -= OnDialogClosed;
			}
			catch { }
		}
#endif
		#endregion
		#region ISpreadsheetControl.Show(*)Form implementation
		void ISpreadsheetControl.ShowPasteSpecialForm(PasteSpecialInfo properties, ShowPasteSpecialFormCallback callback, object callbackData) {
			PasteSpecialFormControllerParameters controllerParameters = new PasteSpecialFormControllerParameters(this, properties);
			PasteSpecialFormControl form = new PasteSpecialFormControl(controllerParameters);
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value)
					callback(form.GetSourcePasteSpecialInfo(), callbackData);
			};
			ShowDXDialog(form, onFormClosed, XpfSpreadsheetLocalizer.GetString(SpreadsheetControlStringId.Caption_PasteSpecialFormTitle));
		}
		void ISpreadsheetControl.ShowPasteSpecialLocalForm(ModelPasteSpecialOptions properties, ShowPasteSpecialFormLocalCallback callback, object callbackData) {
			PasteSpecialLocalFormControllerParameters controllerParameters = new PasteSpecialLocalFormControllerParameters(this, properties);
			PasteSpecialLocalControl form = new PasteSpecialLocalControl(controllerParameters);
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value)
					callback(form.GetSourcePasteSpecialOptions());
			};
			ShowDXDialog(form, onFormClosed, XpfSpreadsheetLocalizer.GetString(SpreadsheetControlStringId.Caption_PasteSpecialFormTitle));
		}
		void ISpreadsheetControl.ShowUnhideSheetForm(UnhideSheetViewModel viewModel) {
			ShowUnhideSheetForm(viewModel);
		}
		void ShowUnhideSheetForm(UnhideSheetViewModel viewModel) {
			UnhideSheetControl unhideSheetControl = new UnhideSheetControl(viewModel);
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value)
					viewModel.ApplyChanges();
			};
			ShowDXDialog(unhideSheetControl, onFormClosed, XpfSpreadsheetLocalizer.GetString(SpreadsheetControlStringId.Caption_UnhideSheetFormTitle));
		}
		void ISpreadsheetControl.ShowRenameSheetForm(RenameSheetViewModel viewModel) {
			ShowRenameSheetForm(viewModel);
		}
		void ShowRenameSheetForm(RenameSheetViewModel viewModel) {
			RenameSheetControl renameControl = new RenameSheetControl(viewModel);
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value)
					viewModel.ApplyChanges();
			};
			Func<bool> okClick = new Func<bool>(() => {
				string errorMessage = viewModel.ValidateSheetName();
				if (!String.IsNullOrEmpty(errorMessage)) {
					DXMessageBox.Show(errorMessage);
					return false;
				}
				return true;
			});
			ShowDXDialog(renameControl, onFormClosed, XpfSpreadsheetLocalizer.GetString(SpreadsheetControlStringId.Caption_RenameSheetFormTitle), okClick);
		}
		void ISpreadsheetControl.ShowHyperlinkForm(IHyperlinkViewInfo hyperlink, ShowHyperlinkFormCallback callback) {
			HyperlinkFormControllerParameters controllerParameters = new HyperlinkFormControllerParameters(this, hyperlink);
			DevExpress.XtraSpreadsheet.Model.HyperlinkFormShowingEventArgs args = new DevExpress.XtraSpreadsheet.Model.HyperlinkFormShowingEventArgs(controllerParameters);
			HyperlinkControl hyperlinkControl = new HyperlinkControl(controllerParameters);
			DialogClosedDelegate onFormClosed;
			if (!args.Handled) {
				onFormClosed = (dialogResult) => {
					if (dialogResult.HasValue && dialogResult.Value)
						callback(hyperlink);
				};
				ShowDXDialog(hyperlinkControl, onFormClosed, XpfSpreadsheetLocalizer.GetString(SpreadsheetControlStringId.Caption_HyperlinkFormTitle));
			}
			else if (args.DialogResult == PlatformDialogResult.OK) {
				callback(controllerParameters.HyperlinkInfo);
			}
		}
		void ISpreadsheetControl.ShowFormatCellsForm(FormatCellsFormProperties properties, FormatCellsFormInitialTabPage initialTabPage, ShowFormatCellsFormCallback callback) {
			ShowFormatCellsForm(properties, initialTabPage, callback);
		}
		void ShowFormatCellsForm(FormatCellsFormProperties properties, FormatCellsFormInitialTabPage initialTabPage, ShowFormatCellsFormCallback callback) {
			FormatCellsModel model = new FormatCellsModel(this, properties, initialTabPage);
			FormatCellsViewModel viewModel = new FormatCellsViewModel(model);
			DevExpress.Xpf.Spreadsheet.Forms.FormatCellsControl control = new Forms.FormatCellsControl(viewModel);
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value) {
					callback(properties);
				}
			};
			Func<bool> okClick = new Func<bool>(() => { return viewModel.CheckNumberFormatIsValid(); });
			ShowDXDialog(control, onFormClosed, XpfSpreadsheetLocalizer.GetString(SpreadsheetControlStringId.Caption_FormatCellsFormTitle), okClick);
		}
		void ISpreadsheetControl.ShowMoveOrCopySheetForm(MoveOrCopySheetViewModel viewModel) {
			MoveOrCopySheetControl control = new MoveOrCopySheetControl(viewModel);
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value)
					viewModel.ApplyChanges();
			};
			ShowDXDialog(control, onFormClosed, XpfSpreadsheetLocalizer.GetString(SpreadsheetControlStringId.Caption_MoveOrCopySheetFormTitle));
		}
		void ISpreadsheetControl.ShowTableInsertForm(InsertTableViewModel viewModel) {
			InsertTableControl control = new InsertTableControl(viewModel);
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value) {
					viewModel.ApplyChanges();
				}
			};
			Func<bool> okClick = new Func<bool>(() => { return viewModel.Validate(); });
			ShowDXDialog(control, onFormClosed, viewModel.FormText, okClick);
		}
		void ISpreadsheetControl.ShowConditionalFormattingTop10RuleForm(ConditionalFormattingTopBottomRuleViewModel viewModel) {
			ShowConditionalFormattingTop10RuleForm(viewModel);
		}
		void ShowConditionalFormattingTop10RuleForm(ConditionalFormattingTopBottomRuleViewModel viewModel) {
			ConditionalFormattingTopBottomRuleControl control = new ConditionalFormattingTopBottomRuleControl(viewModel);
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value)
					viewModel.ApplyChanges();
			};
			ShowDXDialog(control, onFormClosed, viewModel.FormText);
		}
		void ISpreadsheetControl.ShowConditionalFormattingAverageRuleForm(ConditionalFormattingAverageRuleViewModel viewModel) {
			ConditionalFormattingAverageRuleControl control = new ConditionalFormattingAverageRuleControl(viewModel);
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value)
					viewModel.ApplyChanges();
			};
			ShowDXDialog(control, onFormClosed, viewModel.FormText);
		}
		void ISpreadsheetControl.ShowConditionalFormattingExpressionRuleForm(ConditionalFormattingHighlightCellsRuleViewModel viewModel) {
			ConditionalFormattingHighlightCellsRuleControl control = new ConditionalFormattingHighlightCellsRuleControl(viewModel);
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value)
					viewModel.ApplyChanges();
			};
			ShowDXDialog(control, onFormClosed, viewModel.FormText);
		}
		void ISpreadsheetControl.ShowConditionalFormattingTextRuleForm(ConditionalFormattingTextRuleViewModel viewModel) {
			ConditionalFormattingHighlightCellsRuleControl control = new ConditionalFormattingHighlightCellsRuleControl(viewModel);
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value)
					viewModel.ApplyChanges();
			};
			ShowDXDialog(control, onFormClosed, viewModel.FormText);
		}
		void ISpreadsheetControl.ShowConditionalFormattingDuplicateValuesRuleForm(ConditionalFormattingDuplicateValuesRuleViewModel viewModel) {
			ConditionalFormattingDuplicateValuesRuleControl control = new ConditionalFormattingDuplicateValuesRuleControl(viewModel);
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value)
					viewModel.ApplyChanges();
			};
			ShowDXDialog(control, onFormClosed, viewModel.FormText);
		}
		void ISpreadsheetControl.ShowConditionalFormattingDateOccurringRuleForm(ConditionalFormattingDateOccurringRuleViewModel viewModel) {
			ConditionalFormattingDateOccurringRuleControl control = new ConditionalFormattingDateOccurringRuleControl(viewModel);
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value)
					viewModel.ApplyChanges();
			};
			ShowDXDialog(control, onFormClosed, viewModel.FormText);
		}
		void ISpreadsheetControl.ShowConditionalFormattingBetweenRuleForm(ConditionalFormattingBetweenRuleViewModel viewModel) {
			ConditionalFormattingBetweenRuleControl control = new ConditionalFormattingBetweenRuleControl(viewModel);
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value)
					viewModel.ApplyChanges();
			};
			ShowDXDialog(control, onFormClosed, viewModel.FormText);
		}
		void ISpreadsheetControl.ShowChangeChartTypeForm(ChangeChartTypeViewModel viewModel) {
			ShowChangeChartTypeForm(viewModel);
		}
		void ShowChangeChartTypeForm(ChangeChartTypeViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowDataMemberEditorForm(DataMemberEditorViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowSelectDataMemberForm(SelectDataMemberViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowFilterEditorForm(FilterEditorViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowGroupEditorForm(GroupEditorViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowAddDataSourceForm(Action<object> callback) {
		}
		void ISpreadsheetControl.ShowSelectDataSourceForm(SelectDataSourceViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowManageQueriesForm() {
		}
		void ISpreadsheetControl.ShowManageRelationsForm() {
		}
		void ISpreadsheetControl.ShowManageDataSourcesForm(ManageDataSourcesViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowGroupUngroupForm(GroupViewModel viewModel) {
			ShowGroupUngroupForm(viewModel);
		}
		void ShowGroupUngroupForm(GroupViewModel viewModel) {
			GroupUngroupControl control = new GroupUngroupControl(viewModel);
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value)
					viewModel.ApplyChanges();
			};
			ShowDXDialog(control, onFormClosed, viewModel.FormCaption);
		}
		void ISpreadsheetControl.ShowOutlineSettingsForm(OutlineSettingsViewModel viewModel) {
			ShowOutlineSettingsForm(viewModel);
		}
		void ShowOutlineSettingsForm(OutlineSettingsViewModel viewModel) {
			OutlineSettingsControl outlineSettingControl = new OutlineSettingsControl(viewModel);
			OutlineDXDialog dialog = new OutlineDXDialog(XpfSpreadsheetLocalizer.GetString(SpreadsheetControlStringId.Caption_OutlineSettingFormTitle), viewModel);
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value)
					viewModel.ApplyChanges();
			};
			SetupDXDialog(dialog, outlineSettingControl, onFormClosed);
			dialog.ShowDialog();
		}
		void ISpreadsheetControl.ShowOutlineSubtotalForm(OutlineSubtotalViewModel viewModel) {
			ShowOutlineSubtotalForm(viewModel);
		}
		private void ShowOutlineSubtotalForm(OutlineSubtotalViewModel viewModel) {
			OutlineSubtotalControl control = new OutlineSubtotalControl(viewModel);
			OutlineSubtotalDXDialog dialog = new OutlineSubtotalDXDialog(XpfSpreadsheetLocalizer.GetString(SpreadsheetControlStringId.Caption_OutlineSubtotalFormSubtotal), viewModel);
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value)
					viewModel.ApplyChanges();
			};
			SetupDXDialog(dialog, control, onFormClosed);
			dialog.ShowDialog();
		}
		void ISpreadsheetControl.ShowGroupRangeEditorForm(GroupRangeEditorViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowMailMergePreviewForm() {
		}
		void ISpreadsheetControl.ShowChangeChartTitleForm(ChangeChartTitleViewModel viewModel) {
			this.ShowChangeChartTitleForm(viewModel);
		}
		void ShowChangeChartTitleForm(ChangeChartTitleViewModel viewModel) {
			ChangeChartItemTitleControl control = new ChangeChartItemTitleControl(viewModel);
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value)
					viewModel.ApplyChanges();
			};
			ShowDXDialog(control, onFormClosed, XpfSpreadsheetLocalizer.GetString(SpreadsheetControlStringId.Caption_ChartTitleFormTitle));
		}
		void ISpreadsheetControl.ShowChangeChartHorizontalAxisTitleForm(ChangeChartHorizontalAxisTitleViewModel viewModel) {
			this.ShowChangeChartHorizontalAxisTitleForm(viewModel);
		}
		void ShowChangeChartHorizontalAxisTitleForm(ChangeChartHorizontalAxisTitleViewModel viewModel) {
			ChangeChartItemTitleControl control = new ChangeChartItemTitleControl(viewModel);
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value)
					viewModel.ApplyChanges();
			};
			ShowDXDialog(control, onFormClosed, XpfSpreadsheetLocalizer.GetString(SpreadsheetControlStringId.Caption_ChartHorizontalAxisTitleFormTitle));
		}
		void ISpreadsheetControl.ShowChangeChartVerticalAxisTitleForm(ChangeChartVerticalAxisTitleViewModel viewModel) {
			this.ShowChangeChartVerticalAxisTitleForm(viewModel);
		}
		void ShowChangeChartVerticalAxisTitleForm(ChangeChartVerticalAxisTitleViewModel viewModel) {
			ChangeChartItemTitleControl control = new ChangeChartItemTitleControl(viewModel);
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value)
					viewModel.ApplyChanges();
			};
			ShowDXDialog(control, onFormClosed, XpfSpreadsheetLocalizer.GetString(SpreadsheetControlStringId.Caption_ChartVerticalAxisTitleFormTitle));
		}
		void ISpreadsheetControl.ShowChartSelectDataForm(ChartSelectDataViewModel viewModel) {
			ChartSelectDataControl control = new ChartSelectDataControl(viewModel);
			DialogClosedDelegate onFormClosed = (dialogResult) => { viewModel.RestoreActiveSheet(); };
			Func<bool> okClick = new Func<bool>(() => { return viewModel.ApplyChanges(); });
			ShowDXDialog(control, onFormClosed, XpfSpreadsheetLocalizer.GetString(SpreadsheetControlStringId.Caption_ChartSelectDataFormTitle), okClick);
		}
		void ISpreadsheetControl.ShowProtectSheetForm(ProtectSheetViewModel viewModel) {
			this.ShowProtectSheetForm(viewModel);
		}
		void ShowProtectSheetForm(ProtectSheetViewModel viewModel) {
			ProtectSheetControl control = new ProtectSheetControl(viewModel);
			Func<bool> okClick = new Func<bool>(() => {
				bool result = false;
				AskPasswordConfirmation(viewModel.Password, () => { result = true; });
				return result;
			});
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value)
					viewModel.ApplyChanges();
			};
			ShowDXDialog(control, onFormClosed, XpfSpreadsheetLocalizer.GetString(SpreadsheetControlStringId.Caption_ProtectSheetFormTitle), okClick);
		}
		void ISpreadsheetControl.ShowProtectWorkbookForm(ProtectWorkbookViewModel viewModel) {
			this.ShowProtectWorkbookForm(viewModel);
		}
		void ShowProtectWorkbookForm(ProtectWorkbookViewModel viewModel) {
			ProtectWorkbookControl control = new ProtectWorkbookControl(viewModel);
			Func<bool> okClick = new Func<bool>(() => {
				bool result = false;
				AskPasswordConfirmation(viewModel.Password, () => { result = true; });
				return result;
			});
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value)
					viewModel.ApplyChanges();
			};
			ShowDXDialog(control, onFormClosed, XpfSpreadsheetLocalizer.GetString(SpreadsheetControlStringId.Caption_ProtectWorkbookFormTitle), okClick);
		}
		void ISpreadsheetControl.ShowUnprotectSheetForm(UnprotectSheetViewModel viewModel) {
			ShowUnprotectSheetForm(viewModel);
		}
		void ShowUnprotectSheetForm(UnprotectSheetViewModel viewModel) {
			UnprotectSheetControl control = new UnprotectSheetControl(viewModel);
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value)
					viewModel.ApplyChanges();
			};
			ShowDXDialog(control, onFormClosed, XpfSpreadsheetLocalizer.GetString(SpreadsheetControlStringId.Caption_UnprotectSheetFormTitle));
		}
		void ISpreadsheetControl.ShowUnprotectWorkbookForm(UnprotectWorkbookViewModel viewModel) {
			ShowUnprotectWorkbookForm(viewModel);
		}
		void ShowUnprotectWorkbookForm(UnprotectWorkbookViewModel viewModel) {
			UnprotectWorkbookControl control = new UnprotectWorkbookControl(viewModel);
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value)
					viewModel.ApplyChanges();
			};
			ShowDXDialog(control, onFormClosed, XpfSpreadsheetLocalizer.GetString(SpreadsheetControlStringId.Caption_UnprotectWorkbookFormTitle));
		}
		void ISpreadsheetControl.ShowUnprotectRangeForm(UnprotectRangeViewModel viewModel) {
			ShowUnprotectRangeForm(viewModel);
		}
		void ShowUnprotectRangeForm(UnprotectRangeViewModel viewModel) {
			UnprotectRangeControl control = new UnprotectRangeControl(viewModel);
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value)
					viewModel.ApplyChanges();
			};
			ShowDXDialog(control, onFormClosed, XpfSpreadsheetLocalizer.GetString(SpreadsheetControlStringId.Caption_UnprotectRangeFormTitle));
		}
		void AskPasswordConfirmation(string password, Action applyChanges) {
			if (String.IsNullOrEmpty(password)) {
				applyChanges();
				return;
			}
			ConfirmPasswordViewModel viewModel = new ConfirmPasswordViewModel();
			ConfirmPasswordControl control = new ConfirmPasswordControl(viewModel);
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value) {
					if (viewModel.Password == password)
						applyChanges();
					else
						this.ShowWarningMessage(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_PasswordNotConfirmed));
				}
			};
			ShowDXDialog(control, onFormClosed, XpfSpreadsheetLocalizer.GetString(SpreadsheetControlStringId.Caption_ConfirmPasswordFormTitle));
		}
		void ISpreadsheetControl.ShowInsertFunctionForm(InsertFunctionViewModel viewModel) {
			this.ShowInsertFunctionForm(viewModel);
		}
		void ShowInsertFunctionForm(InsertFunctionViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowFunctionArgumentsForm(FunctionArgumentsViewModel viewModel) {
			this.ShowFunctionArgumentsForm(viewModel);
		}
		void ShowFunctionArgumentsForm(FunctionArgumentsViewModel viewModel) {
		}
		void ISpreadsheetControl.ShowInsertSymbolForm(InsertSymbolViewModel viewModel) {
			this.ShowInsertSymbolForm(viewModel);
		}
		void ShowInsertSymbolForm(InsertSymbolViewModel viewModel) {
			DevExpress.Xpf.Office.UI.CharacterMapControl control = new DevExpress.Xpf.Office.UI.CharacterMapControl() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = System.Windows.VerticalAlignment.Stretch };
			control.Margin = new Thickness(12, 12, 12, 12);
			control.Dispatcher.BeginInvoke(new Action(() => control.FontName = viewModel.FontName));
			control.CharDoubleClick += (s, e) => {
				DevExpress.Xpf.Office.UI.CharacterMapControl map = (DevExpress.Xpf.Office.UI.CharacterMapControl)s;
				OnCharacterMapControlCharClick(viewModel, map.Selection.ToString(), map.FontName);
			};
			control.ServiceProvider = this;
			control.UpdateLayout();
			CustomDXDialog dialog = new CustomDXDialog(XpfSpreadsheetLocalizer.GetString(SpreadsheetControlStringId.Caption_InsertSymbolFromSelectionFormTitle), DialogButtons.OkCancel);
			SetupDXDialog(dialog, control, null);
			dialog.ShowDialog();
		}
		void OnCharacterMapControlCharClick(InsertSymbolViewModel viewModel, string text, string fontName) {
			if (String.IsNullOrEmpty(text) || String.IsNullOrEmpty(fontName))
				return;
			viewModel.UnicodeChar = text[0];
			viewModel.FontName = fontName;
			viewModel.ApplyChanges();
		}
		void ISpreadsheetControl.ShowFindReplaceForm(FindReplaceViewModel viewModel) {
			this.ShowFindReplaceForm(viewModel);
		}
		void ShowFindReplaceForm(FindReplaceViewModel viewModel) {
			FindReplaceControl control = new FindReplaceControl(viewModel);
			FindReplaceDXDialog dialog = new FindReplaceDXDialog(XpfSpreadsheetLocalizer.GetString(SpreadsheetControlStringId.Caption_FindReplaceFormTitle), viewModel);
			SetupDXDialog(dialog, control, null);
			dialog.FindButtonClick += (s, e) => { viewModel.FindNext(); };
			dialog.ReplaceButtonClick += (s, e) => { viewModel.ReplaceNext(); };
			dialog.ReplaceAllButtonClick += (s, e) => { viewModel.ReplaceAll(); };
			dialog.ShowDialog();
		}
		void ISpreadsheetControl.ShowDefineNameForm(DefineNameViewModel viewModel) {
			ShowDefineNameForm(viewModel, null);
		}
		protected internal void ShowDefineNameForm(DefineNameViewModel viewModel, EventHandler formClosedCallback) {
			DefineNameControl control = new DefineNameControl(viewModel);
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value) {
					viewModel.ApplyChanges();
				}
			};
			Func<bool> okClick = new Func<bool>(() => { return viewModel.Validate(); });
			CustomDXDialog dialog = new CustomDXDialog(viewModel.FormText, DialogButtons.OkCancel);
			SetupDXDialog(dialog, control, onFormClosed, okClick);
			if (formClosedCallback != null)
				dialog.Closed += formClosedCallback;
			dialog.ShowDialog();
		}
		void ISpreadsheetControl.ShowNameManagerForm(NameManagerViewModel viewModel) {
			ShowNameManagerForm(viewModel);
		}
		void ShowNameManagerForm(NameManagerViewModel viewModel) {
			NameManagerControl control = new NameManagerControl(viewModel);
			NameManagerDXDialog dialog = new NameManagerDXDialog(XpfSpreadsheetLocalizer.GetString(SpreadsheetControlStringId.Caption_NameManagerFormTitle));
			viewModel.SubscribeDefinedNameEvents();
			SetupDXDialog(dialog, control,
				delegate(bool? dialogResult) {
					viewModel.UnsubscribeDefinedNameEvents();
				});
			dialog.ShowDialog();
		}
		void ISpreadsheetControl.ShowCreateDefinedNamesFromSelectionForm(CreateDefinedNamesFromSelectionViewModel viewModel) {
			ShowCreateDefinedNamesFromSelectionForm(viewModel);
		}
		void ShowCreateDefinedNamesFromSelectionForm(CreateDefinedNamesFromSelectionViewModel viewModel) {
			CreateDefinedNamesFromSelectionControl control = new CreateDefinedNamesFromSelectionControl(viewModel);
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value)
					viewModel.ApplyChanges();
			};
			ShowDXDialog(control, onFormClosed, XpfSpreadsheetLocalizer.GetString(SpreadsheetControlStringId.Caption_CreateDefinedNamesFromSelectionFormTitle));
		}
		void ISpreadsheetControl.ShowProtectedRangeForm(ProtectedRangeViewModel viewModel) {
			ShowProtectedRangeForm(viewModel);
		}
		void ShowProtectedRangeForm(ProtectedRangeViewModel viewModel) {
			ShowProtectedRangeForm(viewModel, null);
		}
		protected internal void ShowProtectedRangeForm(ProtectedRangeViewModel viewModel, EventHandler formClosedCallback) {
			ProtectRangeControl control = new ProtectRangeControl(viewModel);
			Func<bool> okClick = new Func<bool>(() => {
				if (!viewModel.Validate())
					return false;
				if (viewModel.HasPassword)
					return true;
				bool result = false;
				AskPasswordConfirmation(viewModel.Password, () => { result = true; });
				return result;
			});
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value)
					viewModel.ApplyChanges();
			};
			CustomDXDialog dialog = new CustomDXDialog(viewModel.FormText, DialogButtons.OkCancel);
			SetupDXDialog(dialog, control, onFormClosed, okClick);
			if (formClosedCallback != null)
				dialog.Closed += formClosedCallback;
			dialog.ShowDialog();
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
			ProtectedRangeManagerControl control = new ProtectedRangeManagerControl(viewModel);
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value)
					viewModel.ApplyChanges();
			};
			ShowDXDialog(control, onFormClosed, XpfSpreadsheetLocalizer.GetString(SpreadsheetControlStringId.Caption_ProtectedRangeManagerFormTitle));
		}
		void ISpreadsheetControl.ShowRowHeightForm(RowHeightViewModel viewModel) {
			ShowRowHeightForm(viewModel);
		}
		void ShowRowHeightForm(RowHeightViewModel viewModel) {
			RowHeightControl control = new RowHeightControl(viewModel);
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value) {
					viewModel.ApplyChanges();
				}
			};
			Func<bool> okClick = new Func<bool>(() => { return viewModel.Validate(); });
			ShowDXDialog(control, onFormClosed, XpfSpreadsheetLocalizer.GetString(SpreadsheetControlStringId.Caption_RowHeightFormTitle), okClick);
		}
		void ISpreadsheetControl.ShowColumnWidthForm(ColumnWidthViewModel viewModel) {
			ShowColumnWidthForm(viewModel);
		}
		void ShowColumnWidthForm(ColumnWidthViewModel viewModel) {
			ColumnWidthControl control = new ColumnWidthControl(viewModel);
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value) {
					viewModel.ApplyChanges();
				}
			};
			Func<bool> okClick = new Func<bool>(() => { return viewModel.Validate(); });
			ShowDXDialog(control, onFormClosed, XpfSpreadsheetLocalizer.GetString(SpreadsheetControlStringId.Caption_ColumnWidthFormTitle), okClick);
		}
		void ISpreadsheetControl.ShowDefaultColumnWidthForm(DefaultColumnWidthViewModel viewModel) {
			ShowDefaultColumnWidthForm(viewModel);
		}
		void ShowDefaultColumnWidthForm(DefaultColumnWidthViewModel viewModel) {
			DefaultColumnWidthControl control = new DefaultColumnWidthControl(viewModel);
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value) {
					viewModel.ApplyChanges();
				}
			};
			Func<bool> okClick = new Func<bool>(() => { return viewModel.Validate(); });
			ShowDXDialog(control, onFormClosed, XpfSpreadsheetLocalizer.GetString(SpreadsheetControlStringId.Caption_DefaultColumnWidthFormTitle), okClick);
		}
		void ISpreadsheetControl.ShowDocumentPropertiesForm(DocumentPropertiesViewModel viewModel) {
			ShowDocumentPropertiesForm(viewModel);
		}
		void ShowDocumentPropertiesForm(DocumentPropertiesViewModel viewModel) {
			DocumentPropertiesControl control = new DocumentPropertiesControl(viewModel);
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value) {
					viewModel.ApplyChanges();
				}
			};
			Func<bool> okClick = new Func<bool>(() => { return viewModel.Validate(); });
			string title = XpfSpreadsheetLocalizer.GetString(SpreadsheetControlStringId.Caption_DocumentPropertiesFormTitle);
			if (!String.IsNullOrEmpty(viewModel.FileName))
				title = viewModel.FileName + " " + title;
			ShowDXDialog(control, onFormClosed, title, okClick);
		}
		void ISpreadsheetControl.ShowAutoFilterForm(AutoFilterViewModel viewModel) {
			ShowAutoFilterForm(viewModel);
		}
		void ShowAutoFilterForm(AutoFilterViewModel viewModel) {
			SpreadsheetContextMenuService service = GetService<ISpreadsheetContextMenuService>() as SpreadsheetContextMenuService;
			if (service == null)
				return;
			service.ShowAutoFilterContextMenu(this);
		}
		void ISpreadsheetControl.ShowGenericFilterForm(GenericFilterViewModel viewModel) {
			ShowGenericFilterForm(viewModel);
		}
		void ShowGenericFilterForm(GenericFilterViewModel viewModel) {
			GenericFilterControl control = new GenericFilterControl(viewModel);
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value) {
					viewModel.ApplyChanges();
				}
			};
			Func<bool> okClick = new Func<bool>(() => { return viewModel.Validate(); });
			ShowDXDialog(control, onFormClosed, XpfSpreadsheetLocalizer.GetString(SpreadsheetControlStringId.Caption_GenericFilterFormTitle), okClick);
		}
		void ISpreadsheetControl.ShowTop10FilterForm(Top10FilterViewModel viewModel) {
			ShowTop10FilterForm(viewModel);
		}
		void ShowTop10FilterForm(Top10FilterViewModel viewModel) {
			Top10FilterControl control = new Top10FilterControl(viewModel);
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value) {
					viewModel.ApplyChanges();
				}
			};
			Func<bool> okClick = new Func<bool>(() => { return viewModel.Validate(); });
			ShowDXDialog(control, onFormClosed, XpfSpreadsheetLocalizer.GetString(SpreadsheetControlStringId.Caption_Top10FilterFormTitle), okClick);
		}
		void ISpreadsheetControl.ShowSimpleFilterForm(SimpleFilterViewModel viewModel) {
			ShowSimpleFilterForm(viewModel);
		}
		void ShowSimpleFilterForm(SimpleFilterViewModel viewModel) {
			SimpleFilterControl control = new SimpleFilterControl(viewModel);
			SimpleFilterDXDialog dialog = new SimpleFilterDXDialog(XpfSpreadsheetLocalizer.GetString(SpreadsheetControlStringId.Caption_SimpleFilterFormTitle), viewModel, control);
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value)
					viewModel.ApplyChanges();
			};
			Func<bool> okClick = new Func<bool>(() => {
				control.UpdateViewModelState();
				bool isValid = viewModel.Validate();
				if (!isValid)
					this.ShowWarningMessage(XpfSpreadsheetLocalizer.GetString(SpreadsheetControlStringId.Msg_ErrorEmptySimpleFilter));
				return isValid;
			});
			SetupDXDialog(dialog, control, onFormClosed, okClick);
			dialog.CheckAllButtonClick += (s, e) => { control.CheckedTree(control.Nodes, true); };
			dialog.UncheckAllButtonClick += (s, e) => { control.CheckedTree(control.Nodes, false); };
			dialog.ShowDialog();
		}
		void ISpreadsheetControl.ShowDataValidationForm(DataValidationViewModel viewModel) {
			DataValidationControl control = new DataValidationControl(viewModel);
			DataValidationDXDialog dialog = new DataValidationDXDialog(XpfSpreadsheetLocalizer.GetString(SpreadsheetControlStringId.Caption_DataValidationFormTitle), viewModel);
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value) {
					viewModel.ApplyChanges();
				}
			};
			Func<bool> okClick = new Func<bool>(() => { return viewModel.Validate(); });
			SetupDXDialog(dialog, control, onFormClosed, okClick);
			dialog.OnClearAllButtonClick += (s, e) => { viewModel.ResetToDefaults(); };
			dialog.ShowDialog();
		}
		void ISpreadsheetControl.ShowPageSetupForm(PageSetupViewModel viewModel, PageSetupFormInitialTabPage initialTabPage) {
			PageSetupControl control = new PageSetupControl(viewModel);
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value) {
					viewModel.ApplyChanges();
				}
			};
			Func<bool> okClick = new Func<bool>(() => { return control.ValidatePageSetupChanges(); });
			ShowDXDialog(control, onFormClosed, XpfSpreadsheetLocalizer.GetString(SpreadsheetControlStringId.Caption_PageSetupFormTitle), okClick);
		}
		void ISpreadsheetControl.ShowHeaderFooterForm(HeaderFooterViewModel viewModel) {
			ShowHeaderFooterForm(viewModel);
		}
		protected internal void ShowHeaderFooterForm(HeaderFooterViewModel viewModel) {
			HeaderFooterControl control = new HeaderFooterControl(viewModel);
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value) {
					viewModel.IsUpdateAllowed = dialogResult.Value;
					viewModel.ApplyChanges();
				}
			};
			Func<bool> okClick = new Func<bool>(() => { return viewModel.ValidateHeaderFooter(); });
			ShowDXDialog(control, onFormClosed, XpfSpreadsheetLocalizer.GetString(SpreadsheetControlStringId.Caption_HeaderFooterFormTitle), okClick);
		}
		void ISpreadsheetControl.ShowInsertPivotTableForm(InsertPivotTableViewModel viewModel) {
			InsertPivotTableControl control = new InsertPivotTableControl(viewModel);
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value) {
					viewModel.ApplyChanges();
				}
			};
			Func<bool> okClick = new Func<bool>(() => { return viewModel.Validate(); });
			ShowDXDialog(control, onFormClosed, XpfSpreadsheetLocalizer.GetString(SpreadsheetControlStringId.Caption_InsertPivotTableFormTitle), okClick);
		}
		void ISpreadsheetControl.ShowOptionsPivotTableForm(OptionsPivotTableViewModel viewModel) {
			OptionsPivotTableControl control = new OptionsPivotTableControl(viewModel);
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value) {
					viewModel.ApplyChanges();
				}
			};
			Func<bool> okClick = new Func<bool>(() => { return viewModel.Validate(); });
			ShowDXDialog(control, onFormClosed, XpfSpreadsheetLocalizer.GetString(SpreadsheetControlStringId.Caption_OptionsPivotTableFormTitle), okClick);
		}
		void ISpreadsheetControl.ShowMovePivotTableForm(MovePivotTableViewModel viewModel) {
			MovePivotTableControl control = new MovePivotTableControl(viewModel);
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value) {
					viewModel.ApplyChanges();
				}
			};
			Func<bool> okClick = new Func<bool>(() => { return viewModel.Validate(); });
			ShowDXDialog(control, onFormClosed, XpfSpreadsheetLocalizer.GetString(SpreadsheetControlStringId.Caption_MovePivotTableFormTitle), okClick);
		}
		void ISpreadsheetControl.ShowChangeDataSourcePivotTableForm(ChangeDataSourcePivotTableViewModel viewModel) {
			ChangeDataSourcePivotTableControl control = new ChangeDataSourcePivotTableControl(viewModel);
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value) {
					viewModel.ApplyChanges();
				}
			};
			Func<bool> okClick = new Func<bool>(() => { return viewModel.Validate(); });
			ShowDXDialog(control, onFormClosed, XpfSpreadsheetLocalizer.GetString(SpreadsheetControlStringId.Caption_ChangeDataSourcePivotTableFormTitle), okClick);
		}
		void ISpreadsheetControl.ShowFieldSettingsPivotTableForm(FieldSettingsPivotTableViewModel viewModel) {
			FieldSettingsPivotTableControl control = new FieldSettingsPivotTableControl(viewModel);
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value) {
					viewModel.ApplyChanges();
				}
			};
			Func<bool> okClick = new Func<bool>(() => {
				viewModel.SetSubtotal(control.CastToListSelectedFunctions());
				return viewModel.Validate();
			});
			ShowDXDialog(control, onFormClosed, XpfSpreadsheetLocalizer.GetString(SpreadsheetControlStringId.Caption_FieldSettingsPivotTableFormTitle), okClick);
		}
		void ISpreadsheetControl.ShowDataFieldSettingsPivotTableForm(DataFieldSettingsPivotTableViewModel viewModel) {
			DataFieldSettingsPivotTableControl control = new DataFieldSettingsPivotTableControl(viewModel);
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value) {
					viewModel.ApplyChanges();
				}
			};
			Func<bool> okClick = new Func<bool>(() => {
				return viewModel.Validate();
			});
			ShowDXDialog(control, onFormClosed, XpfSpreadsheetLocalizer.GetString(SpreadsheetControlStringId.Caption_DataFieldSettingsPivotTableFormTitle), okClick);
		}
		void ISpreadsheetControl.ShowPivotTableFieldsFilterItemsForm(PivotTableFieldsFilterItemsViewModel viewModel) {
			PivotTableFieldsFilterItemsControl control = new PivotTableFieldsFilterItemsControl(viewModel);
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value) {
					viewModel.ApplyChanges();
				}
			};
			Func<bool> okClick = new Func<bool>(() => {
				control.UpdateViewModel();
				bool isValid = viewModel.Validate();
				if (!isValid)
					this.ShowWarningMessage(XpfSpreadsheetLocalizer.GetString(SpreadsheetControlStringId.Msg_ErrorEmptySimpleFilter));
				return isValid;
			});
			ShowDXDialog(control, onFormClosed, XpfSpreadsheetLocalizer.GetString(SpreadsheetControlStringId.Caption_PivotTableFieldFilterItemsFormTitle), okClick);
		}
		IPivotTableFieldsPanel ISpreadsheetControl.CreatePivotTableFieldsPanel() {
			return new FieldsPanelPivotTableControl(this);
		}
		void ISpreadsheetControl.ShowPivotTableAutoFilterForm() {
			SpreadsheetContextMenuService service = GetService<ISpreadsheetContextMenuService>() as SpreadsheetContextMenuService;
			if (service == null)
				return;
			service.ShowPivotTableAutoFilterContextMenu(this);
		}
		void ISpreadsheetControl.ShowPivotTableValueFilterForm(PivotTableValueFiltersViewModel viewModel) {
			ShowPivotTableValueFilterForm(viewModel);
		}
		void ShowPivotTableValueFilterForm(PivotTableValueFiltersViewModel viewModel) {
			PivotTableValueFilterControl control = new PivotTableValueFilterControl(viewModel);
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value) {
					viewModel.ApplyChanges();
				}
			};
			Func<bool> okClick = new Func<bool>(() => { return viewModel.Validate(); });
			string title = XpfSpreadsheetLocalizer.GetString(SpreadsheetControlStringId.Caption_PivotTableValueFilterFormTitle) + " (" + viewModel.FieldName + ")";
			ShowDXDialog(control, onFormClosed, title, okClick);
		}
		void ISpreadsheetControl.ShowPivotTableTop10FilterForm(PivotTableTop10FiltersViewModel viewModel) {
			ShowPivotTableTop10FilterForm(viewModel);
		}
		void ShowPivotTableTop10FilterForm(PivotTableTop10FiltersViewModel viewModel) {
			PivotTableTop10FilterControl control = new PivotTableTop10FilterControl(viewModel);
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value) {
					viewModel.ApplyChanges();
				}
			};
			Func<bool> okClick = new Func<bool>(() => { return viewModel.Validate(); });
			string title = XpfSpreadsheetLocalizer.GetString(SpreadsheetControlStringId.Caption_PivotTableTop10FilterFormTitle) + " (" + viewModel.FieldName + ")";
			ShowDXDialog(control, onFormClosed, title, okClick);
		}
		void ISpreadsheetControl.ShowPivotTableDateFilterForm(PivotTableDateFiltersViewModel viewModel) {
			ShowPivotTableDateFilterForm(viewModel);
		}
		void ShowPivotTableDateFilterForm(PivotTableDateFiltersViewModel viewModel) {
			PivotTableDateFilterControl control = new PivotTableDateFilterControl(viewModel);
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value) {
					viewModel.ApplyChanges();
				}
			};
			Func<bool> okClick = new Func<bool>(() => {
				control.SetViewModelStringValues();
				return viewModel.Validate(); 
			});
			string title = XpfSpreadsheetLocalizer.GetString(SpreadsheetControlStringId.Caption_PivotTableDateFilterFormTitle) + " (" + viewModel.FieldName + ")";
			ShowDXDialog(control, onFormClosed, title, okClick);
		}
		void ISpreadsheetControl.ShowPivotTableLabelFilterForm(PivotTableLabelFiltersViewModel viewModel) {
			ShowPivotTableLabelFilterForm(viewModel);
		}
		void ShowPivotTableLabelFilterForm(PivotTableLabelFiltersViewModel viewModel) {
			PivotTableLabelFilterControl control = new PivotTableLabelFilterControl(viewModel);
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value) {
					viewModel.ApplyChanges();
				}
			};
			Func<bool> okClick = new Func<bool>(() => { return viewModel.Validate(); });
			string title = XpfSpreadsheetLocalizer.GetString(SpreadsheetControlStringId.Caption_PivotTableLabelFilterFormTitle) + " (" + viewModel.FieldName + ")";
			ShowDXDialog(control, onFormClosed, title, okClick);
		}
		void ISpreadsheetControl.ShowPivotTableShowValuesAsForm(PivotTableShowValuesAsViewModel viewModel) {
			ShowPivotTableShowValuesAsForm(viewModel);
		}
		void ShowPivotTableShowValuesAsForm(PivotTableShowValuesAsViewModel viewModel) {
			PivotTableShowValuesAsControl control = new PivotTableShowValuesAsControl(viewModel);
			DialogClosedDelegate onFormClosed = (dialogResult) => {
				if (dialogResult.HasValue && dialogResult.Value) {
					viewModel.ApplyChanges();
				}
			};
			Func<bool> okClick = new Func<bool>(() => { return true; });
			string title = XpfSpreadsheetLocalizer.GetString(SpreadsheetControlStringId.Caption_PivotTableShowValuesAsFormTitle) + " (" + viewModel.DataFieldName + ")";
			ShowDXDialog(control, onFormClosed, title, okClick);
		}
		#endregion
	}
}
namespace DevExpress.Xpf.Spreadsheet.Internal {
	public class CustomDXDialog : DXDialog {
		public CustomDXDialog(string title)
			: base(title) {
		}
		public CustomDXDialog(string title, DialogButtons dialogButtons)
			: base(title, dialogButtons) {
		}
		CancelEventHandler onOkButtonClick;
		public event CancelEventHandler OkButtonClick { add { onOkButtonClick += value; } remove { onOkButtonClick -= value; } }
		bool RaiseOnButtonClick() {
			if (onOkButtonClick != null) {
				CancelEventArgs args = new CancelEventArgs();
				onOkButtonClick(this, args);
				return !args.Cancel;
			}
			else
				return true;
		}
		protected override void OnButtonClick(bool? result, MessageBoxResult messageBoxResult) {
			if (messageBoxResult == MessageBoxResult.OK && OkButton != null) {
				OkButton.Focus();
				if (!RaiseOnButtonClick())
					return;
			}
			base.OnButtonClick(result, messageBoxResult);
		}
	}
}
