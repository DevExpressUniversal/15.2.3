#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Validation.AllContextsView;
using DevExpress.ExpressApp.Validation.DiagnosticViews;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Persistent.Validation;
namespace DevExpress.ExpressApp.Validation.Win {
	public class ValidationResultsShowingController : WindowController {
		private int lastShownExceptionHash = 0;
		private void ShowValidationError(DevExpress.Persistent.Validation.ValidationCompletedEventArgs e) {
			DetailView view = Application.CreateDetailView(Application.CreateObjectSpace(typeof(ValidationResults)), "RuleSetValidationResult_ByTarget_Error_DetailView", true,
				new ValidationResults(e.Result, Application.Model));
			string dialogImageName = "State_Validation_Invalid_48x48";
			if(e.Result.ValidationOutcome == ValidationOutcome.Warning) {
				dialogImageName = "State_Validation_Warning_48x48";
				e.Exception = new ValidationException(e.Result);
			}
			StaticImage validationDialogImage = view.FindItem("ValidationResultsImage") as StaticImage;
			if(validationDialogImage != null) {
				validationDialogImage.ImageName = dialogImageName;
			}
			if(!string.IsNullOrEmpty(e.Exception.MessageHeader)) {
				ViewItem messageHeader = view.FindItem("ValidationResultsText");
				if(messageHeader != null && messageHeader is StaticText) {
					((StaticText)messageHeader).Text = e.Exception.MessageHeader;
				}
			}
			ShowViewParameters parameters = new ShowViewParameters(view);
			parameters.TargetWindow = TargetWindow.NewModalWindow;
			parameters.Context = TemplateContext.PopupWindow;
			DialogController dialogController = new DialogController();
			dialogController.CancelAction.Caption = CaptionHelper.GetLocalizedText("DialogButtons", "Close", "Close");
			dialogController.CancelAction.ToolTip = dialogController.CancelAction.Caption;
			dialogController.CancelAction.ActionMeaning |= ActionMeaning.Cancel;
			dialogController.AcceptAction.Active.SetItemValue("Showing validation results", e.Result.ValidationOutcome == ValidationOutcome.Warning);
			dialogController.AcceptAction.Caption = CaptionHelper.GetLocalizedText("DialogButtons", "Ignore", "Ignore");
			dialogController.AcceptAction.Execute += (s, a) => {
				e.Handled = true;
				e.Exception = null;
			};
			dialogController.SaveOnAccept = false;
			parameters.Controllers.Add(dialogController);
			Application.ShowViewStrategy.ShowView(parameters, new ShowViewSource(Frame, null));
		}
		private void RuleSet_ValidationCompleted(object sender, DevExpress.Persistent.Validation.ValidationCompletedEventArgs e) {
			lastShownExceptionHash = 0;
			if(e.Result.ValidationOutcome < ValidationOutcome.Information) return;
			ValidationException exception = null;
			ValidationException currentException = e.Exception as ValidationException;
			while(currentException != null) {
				exception = currentException;
				currentException = currentException.InnerException as ValidationException;
			}
			bool isWarning = e.Result.ValidationOutcome == ValidationOutcome.Warning;
			bool showDialog = exception != null || isWarning;
			e.Handled = e.Handled || !showDialog;
			if(!e.Handled) {
				ShowValidationError(e);
				lastShownExceptionHash = e.Exception != null ? e.Exception.GetHashCode() : 0;
			}
		}
		private void ValidationResultsShowingController_CustomHandleException(object sender, CustomHandleExceptionEventArgs e) {
			ValidationException exception = e.Exception as ValidationException;
			Exception currentException = e.Exception;
			while(exception == null && currentException.InnerException != null) {
				currentException = currentException.InnerException;
				if(currentException is ValidationException) {
					exception = currentException as ValidationException;
				}
			}
			if(exception != null && !e.Handled) {
				if(lastShownExceptionHash != exception.GetHashCode()) {
					ShowValidationError(new ValidationCompletedEventArgs(exception, null));
				}
				e.Handled = true;
			}
			lastShownExceptionHash = 0;
		}
		protected override void OnActivated() {
			base.OnActivated();
			DevExpress.Persistent.Validation.Validator.RuleSet.ValidationCompleted += new EventHandler<DevExpress.Persistent.Validation.ValidationCompletedEventArgs>(RuleSet_ValidationCompleted);
			((WinApplication)Application).CustomHandleException += new EventHandler<CustomHandleExceptionEventArgs>(ValidationResultsShowingController_CustomHandleException);
		}
		protected override void OnDeactivated() {
			DevExpress.Persistent.Validation.Validator.RuleSet.ValidationCompleted -= new EventHandler<DevExpress.Persistent.Validation.ValidationCompletedEventArgs>(RuleSet_ValidationCompleted);
			((WinApplication)Application).CustomHandleException -= new EventHandler<CustomHandleExceptionEventArgs>(ValidationResultsShowingController_CustomHandleException);
			base.OnDeactivated();
		}
		public ValidationResultsShowingController() {
			TargetWindowType = WindowType.Main;
		}
	}
	public class SuppressToolBar : ViewController {
		private List<Type> targetTypes = new List<Type>();		
		protected override void OnViewChanging(View view) {
			base.OnViewChanging(view);
			Boolean isActive = false;
			if(view is ObjectView) {
				isActive =
					targetTypes.Contains(((ObjectView)view).ObjectTypeInfo.Type)
					||
					(((ObjectView)view).ObjectTypeInfo.FindAttribute<SuppressToolBarAttribute>() != null);
				if(!isActive) {
					foreach(Type currentType in targetTypes) {
						if(currentType.IsInterface) {
							if(currentType.IsAssignableFrom(((ObjectView)view).ObjectTypeInfo.Type)) {
								isActive = true;
								break;
							}
						}
					}
				}
			}
			Active.SetItemValue("SupressToolbar", isActive);
		}
		protected override void OnActivated() {
			base.OnActivated();
			View.ControlsCreated += new EventHandler(View_ControlsCreated);
		}
		protected override void OnDeactivated() {
			if(Frame.Template is ISupportStoreSettings) {
				((ISupportStoreSettings)Frame.Template).SettingsReloaded -= new EventHandler(SuppressToolBar_SettingsReloaded);
			}
			base.OnDeactivated();
		}
		void View_ControlsCreated(object sender, EventArgs e) {
			View.ControlsCreated -= new EventHandler(View_ControlsCreated);
			if(Frame != null) {
				if(Frame.Template is ISupportActionsToolbarVisibility) {
					((ISupportActionsToolbarVisibility)(Frame.Template)).SetVisible(false);
				}
				if(Frame.Template is ISupportStoreSettings) {
					((ISupportStoreSettings)Frame.Template).SettingsReloaded -= new EventHandler(SuppressToolBar_SettingsReloaded);
					((ISupportStoreSettings)Frame.Template).SettingsReloaded += new EventHandler(SuppressToolBar_SettingsReloaded);
				}
			}
		}
		void SuppressToolBar_SettingsReloaded(object sender, EventArgs e) {
			if(sender is ISupportActionsToolbarVisibility) {
				((ISupportActionsToolbarVisibility)sender).SetVisible(false);
			}
		}
		public SuppressToolBar() : base() {
			targetTypes.Add(typeof(RuleSetValidationResultItem));
			targetTypes.Add(typeof(IRuleBaseProperties));
		}
	}
	public class CustomizeErrorMessageColumnController : ViewController {
		private void SetupGridView(GridListEditor gridListEditor) {
			if((gridListEditor.GridView != null) && (gridListEditor.DataSource != null)) {
				gridListEditor.GridView.OptionsView.ShowIndicator = false;
				ErrorMessages errorMessages = new ErrorMessages();
				List<object> targets = new List<object>();
				foreach(object obj in gridListEditor.List) {
					var ruleItem = ((DisplayableValidationResultItem)obj).Rule;
					string messageNodeName = "ValidationErrorMessage";
					if(ruleItem.Properties.ResultType == ValidationResultType.Warning) {
						messageNodeName = "ValidationWarningMessage";
					}
					else if(ruleItem.Properties.ResultType == ValidationResultType.Information) { 
						messageNodeName = "ValidationInformationMessage";
					}
					errorMessages.AddMessage("ErrorMessage", obj, CaptionHelper.GetLocalizedText("Messages", messageNodeName), ImageLoader.Instance.GetImageInfo(ruleItem.Properties.ResultType.ToString()));
				}
				IXafGridView xafGridView = gridListEditor.GridView as IXafGridView;
				if(xafGridView != null) {
					xafGridView.ErrorMessages = errorMessages;
				}
			}
		}
		private void Editor_ControlsCreated(object sender, EventArgs e) {
			SetupGridView((GridListEditor)sender);
		}
		private void Editor_DataSourceChanged(object sender, EventArgs e) {
			SetupGridView((GridListEditor)sender);
		}
		public CustomizeErrorMessageColumnController()
			: base() {
			this.TargetObjectType = typeof(DisplayableValidationResultItem);
			this.TargetViewType = ViewType.ListView;
			this.TargetViewId = "RuleSetValidationResultItem_ByTarget_ListView";
			this.TargetViewNesting = Nesting.Nested;
		}
		protected override void OnActivated() {
			base.OnActivated();
			Guard.TypeArgumentIs(typeof(GridListEditor), ((ListView)View).Editor.GetType(), "View.Editor");
			((ListView)View).Editor.ControlsCreated += new EventHandler(Editor_ControlsCreated);
			((ListView)View).Editor.DataSourceChanged += new EventHandler(Editor_DataSourceChanged);
		}
		protected override void OnDeactivated() {
			((ListView)View).Editor.ControlsCreated -= new EventHandler(Editor_ControlsCreated);
			((ListView)View).Editor.DataSourceChanged -= new EventHandler(Editor_DataSourceChanged);
			base.OnDeactivated();
		}
	}
}
