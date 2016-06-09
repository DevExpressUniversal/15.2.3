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
using System.Collections;
using System.Collections.Generic;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.SystemModule;
namespace DevExpress.ExpressApp.Validation.AllContextsView {
	public class AllContextsValidatingEventArgs : EventArgs {
		private ArrayList targetObjects;
		public AllContextsValidatingEventArgs(ArrayList targetObjects) {
			this.targetObjects = targetObjects;
		}
		public ArrayList TargetObjects {
			get { return targetObjects; }
		}
	}
	public class CustomValidateAllContextsEventArgs : EventArgs {
		public CustomValidateAllContextsEventArgs(SimpleActionExecuteEventArgs args) {
			this.Args = args;
		}
		public RuleSetValidationResult ValidationResult { get; set; }
		public SimpleActionExecuteEventArgs Args { get; private set; }
	}
	public class ShowAllContextsController : ViewController {
		public const string ValidationResults_Error_DetailView = "ValidationResults_Error_DetailView";
		private SimpleAction action;
		private bool isResultsHighlighted;
		private void selector_SelectorCustomGetAggregatedObjectsToValidate(object sender, CustomGetAggregatedObjectsToValidateEventArgs args) {
			if(CustomGetAggregatedObjectsToValidate != null) {
				CustomGetAggregatedObjectsToValidate(this, args);
			}
		}
		private void selector_SelectorNeedToValidateObject(object sender, NeedToValidateObjectEventArgs args) {
			if(NeedToValidateObject != null) {
				NeedToValidateObject(this, args);
			}
		}
		private RuleSetValidationResult GetValidationResult(SimpleActionExecuteEventArgs e) {
			if(CustomValidateAllContexts != null) {
				CustomValidateAllContextsEventArgs customValidateArgs = new CustomValidateAllContextsEventArgs(e);
				CustomValidateAllContexts(this, customValidateArgs);
				return customValidateArgs.ValidationResult ?? new RuleSetValidationResult();
			}
			ArrayList objectsToValidate = new ArrayList(e.SelectedObjects);
			using(AllContextsTargetObjectSelector targetsSelector = new AllContextsTargetObjectSelector()) {
				SubscribeSelectorEvents(targetsSelector);
				foreach(object currentObject in e.SelectedObjects) {
					ArrayList additionalObjects = targetsSelector.GetObjectsToValidate(this.View.ObjectSpace, currentObject);
					foreach(object additionalObj in additionalObjects) {
						if(!objectsToValidate.Contains(additionalObj)) {
							objectsToValidate.Add(additionalObj);
						}
					}
				}
			}
			AllContextsValidatingEventArgs args = new AllContextsValidatingEventArgs(objectsToValidate);
			if(Validating != null) {
				Validating(this, args);
			}
			return Validator.RuleSet.ValidateAllTargets(ObjectSpace, args.TargetObjects);
		}
		private void action_Execute(object sender, SimpleActionExecuteEventArgs e) {
			RuleSetValidationResult ruleSetValidationResult = GetValidationResult(e);
			ValidationResults dialogObject = new ValidationResults(ruleSetValidationResult, Application.Model);
			bool isValid = ruleSetValidationResult.ValidationOutcome <= ValidationOutcome.Valid;
			string viewId = isValid ? "ValidationResults_Passed_DetailView" : "ValidationResults_Error_DetailView";
			DetailView view = Application.CreateDetailView(Application.CreateObjectSpace(typeof(ValidationResults)), viewId, true, dialogObject);
			view.Closing += new EventHandler(view_Closing);
			ViewItem messageHeader = view.FindItem("ValidationResultsText");
			if(messageHeader != null && messageHeader is StaticText) {
				string headerText = isValid ? ValidationExceptionLocalizer.GetExceptionMessage(ValidationExceptionId.ValidationSucceededMessageHeader)
						: ValidationExceptionLocalizer.GetExceptionMessage(ValidationExceptionId.AllContextsErrorMessageHeader);
				((StaticText)messageHeader).Text = headerText;
			}
			e.ShowViewParameters.CreatedView = view;
			e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
			e.ShowViewParameters.Context = TemplateContext.PopupWindow;
			DialogController dialogController = Application.CreateController<DialogController>();
			dialogController.CancelAction.Active.SetItemValue("Showing validation results", false);
			dialogController.AcceptAction.Caption = CaptionHelper.GetLocalizedText("DialogButtons", "Close", "Close");
			dialogController.AcceptAction.ToolTip = dialogController.AcceptAction.Caption;
			dialogController.Accepting += new EventHandler<DialogControllerAcceptingEventArgs>(dialogController_Accepting);
			e.ShowViewParameters.Controllers.Add(dialogController);
			e.ShowViewParameters.Controllers.Add(new EditModeController());
			isResultsHighlighted = false;
		}
		private void DetailView_ViewEditModeChanged(object sender, EventArgs e) {
			UpdateActionState();
		}
		private void view_Closing(object sender, System.EventArgs e) {
			if(!isResultsHighlighted) {
				ObjectView view = sender as ObjectView;
				view.Closing -= new EventHandler(view_Closing);
				HighlightResults((ValidationResults)view.CurrentObject);
			}
		}
		private void HighlightResults(ValidationResults results) {
			List<ResultsHighlightController> resultsHighlightControllers = new List<ResultsHighlightController>();
			resultsHighlightControllers.Add(Frame.GetController<ResultsHighlightController>());
			if(View is DetailView) {
				foreach(ListPropertyEditor listPropertyEditor in ((DetailView)View).GetItems<ListPropertyEditor>()) {
					if(listPropertyEditor.Frame != null) {
						ResultsHighlightController nestedController = listPropertyEditor.Frame.GetController<ResultsHighlightController>();
						if(nestedController != null) {
							resultsHighlightControllers.Add(nestedController);
						}
					}
				}
			}
			foreach(ResultsHighlightController resultsHighlightController in resultsHighlightControllers) {
				if(results.HighlightErrors) {
					resultsHighlightController.HighlightResults(results.RuleSetValidationResult);
				}
				else {
					resultsHighlightController.ClearHighlighting();
				}
			}
			resultsHighlightControllers.Clear();
			isResultsHighlighted = true;
		}
		private void dialogController_Accepting(object sender, DialogControllerAcceptingEventArgs e) {
			DialogController dialogController = (DialogController)sender;
			dialogController.Accepting -= new EventHandler<DialogControllerAcceptingEventArgs>(dialogController_Accepting);
			HighlightResults((ValidationResults)((ObjectView)dialogController.Window.View).CurrentObject);
		}
		private void SubscribeSelectorEvents(ValidationTargetObjectSelector selector) {
			selector.CustomNeedToValidateObject += new EventHandler<NeedToValidateObjectEventArgs>(selector_SelectorNeedToValidateObject);
			selector.CustomGetAggregatedObjectsToValidate += new EventHandler<CustomGetAggregatedObjectsToValidateEventArgs>(selector_SelectorCustomGetAggregatedObjectsToValidate);
		}
		protected override void OnActivated() {
			base.OnActivated();
			if(View is DetailView) {
				((DetailView)View).ViewEditModeChanged += new EventHandler<EventArgs>(DetailView_ViewEditModeChanged);
			}
			UpdateActionState();
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			if(View is DetailView) {
				((DetailView)View).ViewEditModeChanged -= new EventHandler<EventArgs>(DetailView_ViewEditModeChanged);
			}
		}
		protected virtual void UpdateActionState() {
			action.Active.SetItemValue("EditMode", ((View is DetailView) && ((DetailView)View).ViewEditMode == ViewEditMode.Edit));
			action.Active.SetItemValue("IsRoot", View.IsRoot);
		}
		public ShowAllContextsController() {
			action = new SimpleAction(this, "ShowAllContexts", PredefinedCategory.Edit.ToString());
			action.Caption = "Validate";
			action.ToolTip = "Validate selected object(s) in all presented contexts";
			action.ImageName = "Action_Validation_Validate";
			action.SelectionDependencyType = SelectionDependencyType.RequireMultipleObjects;
			action.Execute += new SimpleActionExecuteEventHandler(action_Execute);
			RegisterActions(action);
		}
		public SimpleAction Action {
			get { return action; }
		}
		public event EventHandler<AllContextsValidatingEventArgs> Validating;
		public event EventHandler<CustomGetAggregatedObjectsToValidateEventArgs> CustomGetAggregatedObjectsToValidate;
		public event EventHandler<NeedToValidateObjectEventArgs> NeedToValidateObject;
		public event EventHandler<CustomValidateAllContextsEventArgs> CustomValidateAllContexts;
	}
	internal class EditModeController : ViewController {
		protected override void OnActivated() {
			base.OnActivated();
			DetailView view = (DetailView)View;
			foreach(PropertyEditor editor in view.GetItems<PropertyEditor>()) {
				ISupportViewEditMode support = editor as ISupportViewEditMode;
				if(support != null) {
					support.ViewEditMode = ViewEditMode.Edit;
				}
			}
		}
		public EditModeController()
			: base() {
			TargetViewType = ViewType.DetailView;
		}
	}
	public class PreventValidationDetailsAccessController : ViewController {
		ListViewProcessCurrentObjectController processObjectController;
		ValidationModule validationModule;
		private void processObjectController_CustomProcessSelectedItem(object sender, CustomProcessListViewSelectedItemEventArgs e) {
			bool isValidationDetailsAccessAllowed = validationModule != null ? validationModule.AllowValidationDetailsAccess : false;
			if(!isValidationDetailsAccessAllowed) {
				e.Handled = true;
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			if(validationModule == null) {
				validationModule = (ValidationModule)Application.Modules.FindModule(typeof(ValidationModule));
			}
			processObjectController = Frame.GetController<ListViewProcessCurrentObjectController>();
			if(processObjectController != null) {
				processObjectController.CustomProcessSelectedItem += new EventHandler<CustomProcessListViewSelectedItemEventArgs>(processObjectController_CustomProcessSelectedItem);
			}
		}
		protected override void OnDeactivated() {
			if(processObjectController != null) {
				processObjectController.CustomProcessSelectedItem -= new EventHandler<CustomProcessListViewSelectedItemEventArgs>(processObjectController_CustomProcessSelectedItem);
			}
			processObjectController = null;
			base.OnDeactivated();
		}
		public PreventValidationDetailsAccessController() {
			TargetObjectType = typeof(DevExpress.ExpressApp.Validation.AllContextsView.DisplayableValidationResultItem);
			TargetViewType = ViewType.ListView;
		}
	}
}
