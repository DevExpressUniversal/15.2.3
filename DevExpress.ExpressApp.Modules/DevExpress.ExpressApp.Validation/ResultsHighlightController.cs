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
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Validation;
using System.Collections.ObjectModel;
namespace DevExpress.ExpressApp.Validation {
	public class ResultsHighlightController : ViewController {
		private void RuleSet_ValidationCompleted(object sender, ValidationCompletedEventArgs e) {
			RuleSetValidationCompleted(e);
		}
		protected virtual void RuleSetValidationCompleted(ValidationCompletedEventArgs e) {
			if(e.ObjectSpace == ObjectSpace) {
				ProcessRuleSetValidationResult(e.Result);
			}
		}
		protected virtual void ProcessRuleSetValidationResult(RuleSetValidationResult ruleSetValidationResult) {
			if(ruleSetValidationResult.ValidationOutcome < ValidationOutcome.Information) {
				ClearHighlighting();
			}
			else {
				HighlightResults(ruleSetValidationResult);
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			Validator.RuleSet.ValidationCompleted += new EventHandler<ValidationCompletedEventArgs>(RuleSet_ValidationCompleted);
		}
		protected override void OnDeactivated() {
			Validator.RuleSet.ValidationCompleted -= new EventHandler<ValidationCompletedEventArgs>(RuleSet_ValidationCompleted);
			base.OnDeactivated();
		}
		public void ClearHighlighting() {
			if(View != null && View.ErrorMessages != null) {
				View.ErrorMessages.Clear();
			}
		}
		public void HighlightResults(RuleSetValidationResult result) {
			if(View == null || View.IsDisposed) {
				return;
			}
			ErrorMessages errorMessages = View.ErrorMessages;
			errorMessages.LockEvents();
			try {
				errorMessages.Clear();
				List<RuleSetValidationResultItem> resultItems = new List<RuleSetValidationResultItem>();
				foreach(RuleSetValidationResultItem resultItem in result.Results) {
					resultItems.Clear();
					if(resultItem is RuleSetValidationResultItemAggregate) {
						string propertyName = ((IRuleCollectionPropertyProperties)resultItem.Rule.Properties).TargetCollectionPropertyName;
						errorMessages.AddMessage(propertyName, resultItem.Target, resultItem.ErrorMessage, ImageLoader.Instance.GetImageInfo(resultItem.Rule.Properties.ResultType.ToString()));
						resultItems.AddRange(((RuleSetValidationResultItemAggregate)resultItem).AggregatedResults);
					}
					else {
						resultItems.Add(resultItem);
					}
					foreach(RuleSetValidationResultItem innerResultItem in resultItems) {
						if(innerResultItem.ValidationOutcome > ValidationOutcome.Valid) {
							foreach(string propertyName in innerResultItem.Rule.UsedProperties) {
								errorMessages.AddMessage(propertyName, innerResultItem.Target, innerResultItem.ErrorMessage, ImageLoader.Instance.GetImageInfo(innerResultItem.Rule.Properties.ResultType.ToString()));
							}
						}
					}
				}
			}
			finally {
				errorMessages.UnlockEvents();
			}
		}
	}
	public class GridClientValidationController : ViewController<ListView> {
		ISupportClientValidation editor;
		protected override void OnActivated() {
			base.OnActivated();
			if(View.IsControlCreated) {
				SubscribeValidationEvent();
			}
			else {
				View.ControlsCreated += View_ControlsCreated;
			}
		}
		private void View_ControlsCreated(object sender, EventArgs e) {
			SubscribeValidationEvent();
		}
		private void editor_ClientValidateObject(object sender, ClientValidateObjectEventArgs e) {
			var result = Validator.RuleSet.ValidateTarget(e.ObjectSpace, e.Object, "Save");
			if(result.State == ValidationState.Invalid) {
				e.Valid = false;
				e.ErrorText = result.GetFormattedErrorMessage();
				foreach(var resultItem in result.Results) {
					foreach(string propertyName in resultItem.Rule.UsedProperties) {
						e.PropertyErrors[propertyName] = resultItem.ErrorMessage;
					}
				}
			}
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			UnsubscribeValidationEvent();
		}
		private void SubscribeValidationEvent() {
			editor = View.Editor as ISupportClientValidation;
			if(editor != null) {
				editor.ClientValidateObject += editor_ClientValidateObject;
			}
		}
		private void UnsubscribeValidationEvent() {
			if(editor != null) {
				editor.ClientValidateObject -= editor_ClientValidateObject;
			}
		}
	}
}
