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
using System.Collections.ObjectModel;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
namespace DevExpress.Persistent.Validation {
	public enum ValidationState {
		[ImageName("State_Validation_Valid")]
		Valid,
		[ImageName("State_Validation_Invalid")]
		Invalid,
		[ImageName("State_Validation_Skipped")]
		Skipped
	}
	public enum ValidationOutcome {
		[ImageName("State_Validation_Skipped")]
		Skipped,
		[ImageName("State_Validation_Valid")]
		Valid,
		[ImageName("State_Validation_Information")]
		Information,
		[ImageName("State_Validation_Warning")]
		Warning,
		[ImageName("State_Validation_Invalid")]
		Error
	}
	public class RuleValidationResult {
		private IRule rule;
		private string errorMessage;
		private ValidationOutcome validationOutcome;
		private static ValidationOutcome GetValidationOutcome(ValidationState ruleValidationResult, ValidationResultType ruleValidationResultType) {
			if(ruleValidationResult == ValidationState.Invalid) {
				switch(ruleValidationResultType) {
					case ValidationResultType.Error:
						return ValidationOutcome.Error;
					case ValidationResultType.Warning:
						return ValidationOutcome.Warning;
					case ValidationResultType.Information:
						return ValidationOutcome.Information;
				}
			}
			return ruleValidationResult == ValidationState.Skipped ? ValidationOutcome.Skipped : ValidationOutcome.Valid;
		}
		private RuleValidationResult(IRule rule, object target, ValidationOutcome outcome, string formattedErrorMessage) {
			this.rule = rule;
			this.validationOutcome = outcome;
			this.errorMessage = formattedErrorMessage;
		}
		public RuleValidationResult(IRule rule, object target, ValidationState validationResult, string formattedErrorMessage)
			: this(rule, target, GetValidationOutcome(validationResult, rule.Properties.ResultType), formattedErrorMessage) {
		}
		public IRule Rule {
			get { return rule; }
		}
		public ValidationState State {
			get {
				if(ValidationOutcome > ValidationOutcome.Warning) {
					return ValidationState.Invalid;
				}
				return ValidationOutcome == ValidationOutcome.Skipped ? ValidationState.Skipped : ValidationState.Valid;
			}
		}
		public string ErrorMessage {
			get { return errorMessage; }
		}
		public ValidationOutcome ValidationOutcome {
			get { return validationOutcome; }
		}
	}
	public interface IRule {
		RuleValidationResult Validate(object target);
		ReadOnlyCollection<string> UsedProperties { get; }
		string Id { get; }
		IRuleBaseProperties Properties { get; }
	}
	public interface IRuleSource {
		ICollection<IRule> CreateRules();
		string Name { get; }
	}
	public interface IEmptyCheckable {
		bool IsEmpty { get; }
	}
	public interface ISupportClientValidation {
		event EventHandler<ClientValidateObjectEventArgs> ClientValidateObject;
	}
}
