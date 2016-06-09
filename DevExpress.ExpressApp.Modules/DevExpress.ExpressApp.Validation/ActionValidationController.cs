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
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Validation;
namespace DevExpress.ExpressApp.Validation {
	public class ErrorMessageFormatter {
		private Frame frame;
		private string actionName;
		public ErrorMessageFormatter(Frame frame, string actionName) {
			this.frame = frame;
			this.actionName = actionName;
		}
		public void CustomizeValidationException(ValidationCompletedEventArgs args) {
			string messageHeaderFormat = ValidationExceptionLocalizer.GetExceptionMessage(ValidationExceptionId.ActionErrorMessageHeaderFormat);
			args.Exception.MessageHeader = string.Format(messageHeaderFormat == null ? string.Empty : messageHeaderFormat, actionName);
			string objectMessageHeaderFormat = ValidationExceptionLocalizer.GetExceptionMessage(ValidationExceptionId.ActionErrorMessageObjectFormat);
			if(string.IsNullOrEmpty(objectMessageHeaderFormat)) {
				if(frame.View is ListView) {
					objectMessageHeaderFormat = "\r\n{0}:";
				}
			}
			args.Exception.ObjectHeaderFormat = objectMessageHeaderFormat;
		}
		public string FormatValidationErrorMessage(RuleSetValidationResult ruleSetResult) {
			List<string> items = new List<string>();
			string format = ValidationExceptionLocalizer.GetExceptionMessage(ValidationExceptionId.ActionErrorMessageHeaderFormat);
			items.Add(string.Format(format == null ? string.Empty : format, actionName));
			string objectMessageHeaderFormat = "";
			if(frame.View is ListView) {
				objectMessageHeaderFormat = "\r\n{0}:";
			}
			items.Add(ruleSetResult.GetFormattedErrorMessage(objectMessageHeaderFormat));
			return string.Join("\r\n", items.ToArray());
		}
	}
	public class ActionValidationController : Controller, IModelExtender {
		private void action_Executing(object sender, CancelEventArgs args) {
			if(Frame.View is ISelectionContext) {
				ActionBase action = (ActionBase)sender;
				string validationContexts = action.Model.GetValue<string>("ValidationContexts");
				if(!String.IsNullOrEmpty(validationContexts)) {
					Validate(action.Caption, ((ISelectionContext)Frame.View).SelectedObjects, validationContexts);
				}
			}
		}
		private IObjectSpace ObjectSpace {
			get {
				if((Frame != null) && (Frame.View is ObjectView)) {
					return ((ObjectView)Frame.View).ObjectSpace;
				}
				else {
					return null;
				}
			}
		}
		private bool Validate(string actionName, IEnumerable targets, string contextIDs) {
			return Validator.RuleSet.ValidateAll(ObjectSpace, targets, contextIDs, new ErrorMessageFormatter(Frame, actionName).CustomizeValidationException);
		}
		protected override void OnFrameAssigned() {
			foreach(Controller controller in Frame.Controllers) {
				foreach(ActionBase action in controller.Actions) {
					action.Executing += new CancelEventHandler(action_Executing);
				}
			}
		}
		protected override void Dispose(bool disposing) {
			try {
				if(disposing && Frame != null) {
					foreach(Controller controller in Frame.Controllers) {
						foreach(ActionBase action in controller.Actions) {
							action.Executing -= new CancelEventHandler(action_Executing);
						}
					}
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		#region IExtendModel Members
		void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
			extenders.Add<IModelAction, IModelActionValidationContexts>();
		}
		#endregion
	}
	public interface IModelActionValidationContexts {
		[TypeConverter(typeof(DefaultValidationContextsConverter))]
		[
#if !SL
	DevExpressExpressAppValidationLocalizedDescription("IModelActionValidationContextsValidationContexts"),
#endif
 Category("Behavior")]
		string ValidationContexts { get; set; }
	}
}
