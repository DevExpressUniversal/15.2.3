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
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Validation;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.ExpressApp.Web.Layout;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.ExpressApp.Web.Utils;
using DevExpress.Persistent.Validation;
using DevExpress.Web;
namespace DevExpress.ExpressApp.Validation.Web {
	public class InplaceEditorsValidationControllerWeb : InplaceEditorsValidationControllerBase {
		private string FormatOperand(Type memberType, object operandValue) {
			if(memberType == typeof(DateTime) || memberType == typeof(string)) {
				return string.Format(@"""{0}""", operandValue);
			}
			else if(memberType == typeof(bool)) {
				return ClientSideEventsHelper.ToJSBoolean((bool)operandValue);
			}
			else {
				return string.Format(@"{0}", operandValue);
			}
		}
		private string DefineErrorImageUrls(string template) {
			StringBuilder stringBuilder = new StringBuilder();			
			stringBuilder.AppendFormat(template, GetErrorImageName(ValidationResultType.Error), GetErrorImageInfo(ValidationResultType.Error).ImageUrl);
			stringBuilder.AppendFormat(template, GetErrorImageName(ValidationResultType.Warning), GetErrorImageInfo(ValidationResultType.Warning).ImageUrl);
			stringBuilder.AppendFormat(template, GetErrorImageName(ValidationResultType.Information), GetErrorImageInfo(ValidationResultType.Information).ImageUrl);
			return stringBuilder.ToString();
		}
		private string GetErrorMessage(string messageTemplate, RuleBase rule, object targetObject) {
			return HttpUtility.JavaScriptStringEncode(RuleBase.GetValidationResultMessage(messageTemplate, rule, targetObject));
		}
		private IList<object> GetValidatorParams(IRule rule, string errorMessage) {			
			string validationImageName = GetErrorImageName(rule.Properties.ResultType);
			string invertResult = ClientSideEventsHelper.ToJSBoolean(rule.Properties.InvertResult);
			string skipNullOrEmptyValues = ClientSideEventsHelper.ToJSBoolean(rule.Properties.SkipNullOrEmptyValues);			
			return new List<object> { errorMessage, validationImageName, invertResult, skipNullOrEmptyValues};
		}
		private string GetRequiredFieldScript(RuleRequiredField rule, object targetObject) {
			string errorMessage = GetErrorMessage(rule.Properties.MessageTemplateMustNotBeEmpty, rule, targetObject);
			return string.Format(@"clientSideValidator.AddValidator(new RuleRequiredFieldValidator(""{0}"", ""{1}"", {2}, {3}));", GetValidatorParams(rule, errorMessage).ToArray());
		}
		private string GetRegularExpressionScript(RuleRegularExpression rule, object targetObject) {
			string errorMessage = GetErrorMessage(rule.Properties.MessageTemplateMustMatchPattern, rule, targetObject);			
			string pattern = HttpUtility.JavaScriptStringEncode(rule.Properties.Pattern);
			IList<object> parameters = GetValidatorParams(rule, errorMessage);
			parameters.Add(pattern);
			return string.Format(@"clientSideValidator.AddValidator(new RuleRegularExpressionValidator(""{0}"", ""{1}"", {2}, {3}, ""{4}""));", parameters.ToArray());
		}
		private string GetRangeInitializationScript(Type memberType, object minimumValue, object maximumValue) {
			string operandsInitializationScript = "";
			if(memberType == typeof(DateTime)) {
				operandsInitializationScript = string.Format(@"var leftOperand = editorValue ? editorValue.getTime() : 0;
                                                               var minimumDate = new Date(""{0}"");
                                                               var maximumDate = new Date(""{1}"");
                                                               var minimumValue = minimumDate.getTime();
                                                               var maximumValue = maximumDate.getTime();", minimumValue, maximumValue);
			}
			else {
				operandsInitializationScript = string.Format(@"var leftOperand = editorValue;
                                                               var minimumValue = {0};
                                                               var maximumValue = {1};", FormatOperand(memberType, minimumValue), FormatOperand(memberType, maximumValue));
			}
			return operandsInitializationScript;
		}
		private string GetValueRangeScript(RuleRange rule, Type memberType, object targetObject) {
			string errorMessage = GetErrorMessage(rule.Properties.MessageTemplateMustBeInRange, rule, targetObject);
			string operandsInitializationScript = GetRangeInitializationScript(memberType, rule.Properties.MinimumValue, rule.Properties.MaximumValue);
			return operandsInitializationScript + string.Format(@"clientSideValidator.AddValidator(new RuleRangeValidator(""{0}"", ""{1}"", {2}, {3}, minimumValue, maximumValue));", GetValidatorParams(rule, errorMessage).ToArray());
		}
		private string GetStringComparisonScript(RuleStringComparison rule, object targetObject) {			
			string errorMessage = "";
			string operatorName = "";
			switch(rule.Properties.OperatorType) {
				case StringComparisonType.Contains:					
					errorMessage = GetErrorMessage(rule.Properties.MessageTemplateMustContain, rule, targetObject);
					operatorName = "StringContains";
					break;
				case StringComparisonType.EndsWith:					
					errorMessage = GetErrorMessage(rule.Properties.MessageTemplateMustEndWith, rule, targetObject);
					operatorName = "StringEndsWith";
					break;
				case StringComparisonType.Equals:					
					errorMessage = GetErrorMessage(rule.Properties.MessageTemplateMustBeEqual, rule, targetObject);
					operatorName = "StringEquals";
					break;
				case StringComparisonType.NotEquals:					
					errorMessage = GetErrorMessage(rule.Properties.MessageTemplateMustNotBeEqual, rule, targetObject);
					operatorName = "StringNotEquals";
					break;
				case StringComparisonType.StartsWith:					
					errorMessage = GetErrorMessage(rule.Properties.MessageTemplateMustBeginWith, rule, targetObject);
					operatorName = "StringStartsWith";
					break;
			}			
			IList<object> parameters = GetValidatorParams(rule, errorMessage);
			parameters.Add(rule.Properties.OperandValue);
			parameters.Add(operatorName);
			parameters.Add(ClientSideEventsHelper.ToJSBoolean(rule.Properties.IgnoreCase));
			return string.Format(@"clientSideValidator.AddValidator(new RuleStringComparisonValidator(""{0}"", ""{1}"", {2}, {3}, ""{4}"", {5}, {6}));", parameters.ToArray());
		}
		private string GetOperandsInitializationScript(Type memberType) {
			string operandsInitializationString = "";
			if(memberType == typeof(DateTime)) {
				operandsInitializationString = @"var leftOperand = editorValue != null ? editorValue.getTime() : 0;
                                                 var rightOperandDate = new Date(operandValue);
                                                 var rightOperand = rightOperandDate.getTime();";
			}
			else {
				operandsInitializationString = @"var leftOperand = editorValue;
                                                 var rightOperand = operandValue;";			
			}
			return operandsInitializationString;
		}
		private string GetValueComparisonScript(RuleValueComparison rule, Type memberType, object targetObject) {			
			string operandsInitializationScript = GetOperandsInitializationScript(memberType);
			string errorMessage = "";
			string comparsionOperator = "";			
			switch(rule.Properties.OperatorType) {
				case ValueComparisonType.Equals: comparsionOperator = "==";
					errorMessage = GetErrorMessage(rule.Properties.MessageTemplateMustBeEqualToOperand, rule, targetObject);
					break;
				case ValueComparisonType.GreaterThan: comparsionOperator = ">";
					errorMessage = GetErrorMessage(rule.Properties.MessageTemplateMustBeGreaterThanOperand, rule, targetObject);
					break;
				case ValueComparisonType.GreaterThanOrEqual: comparsionOperator = ">=";
					errorMessage = GetErrorMessage(rule.Properties.MessageTemplateMustBeGreaterThanOrEqualToOperand, rule, targetObject);
					break;
				case ValueComparisonType.LessThan: comparsionOperator = "<";
					errorMessage = GetErrorMessage(rule.Properties.MessageTemplateMustBeLessThanOperand, rule, targetObject);
					break;
				case ValueComparisonType.LessThanOrEqual: comparsionOperator = "<=";
					errorMessage = GetErrorMessage(rule.Properties.MessageTemplateMustBeLessThanOrEqualToOperand, rule, targetObject);
					break;
				case ValueComparisonType.NotEquals: comparsionOperator = "!=";
					errorMessage = GetErrorMessage(rule.Properties.MessageTemplateMustNotBeEqualToOperand, rule, targetObject);
					break;
			}
			IList<object> parameters = GetValidatorParams(rule, errorMessage);
			parameters.Add(FormatOperand(memberType, rule.Properties.RightOperand));
			parameters.Add(string.Format(@"function(value, operandValue) {{ 
                                               {0}
                                               return leftOperand {1} rightOperand; 
                                          }}", operandsInitializationScript, comparsionOperator));
			return string.Format(@"clientSideValidator.AddValidator(new RuleValueComparisonValidator(""{0}"", ""{1}"", {2}, {3}, {4}, {5}));", parameters.ToArray());			
		}
		private string GetRuleValidationScript(IRule rule, object currentObject, Type memberType) {
			string result = "";
			Type ruleType = rule.GetType();			
			if(typeof(RuleRequiredField).IsAssignableFrom(ruleType)) {
				result = GetRequiredFieldScript((RuleRequiredField)rule, currentObject);
			}
			else if(typeof(RuleRegularExpression).IsAssignableFrom(ruleType)) {
				result = GetRegularExpressionScript((RuleRegularExpression)rule, currentObject);
			}
			else if(typeof(RuleStringComparison).IsAssignableFrom(ruleType)) {
				result = GetStringComparisonScript((RuleStringComparison)rule, currentObject);
			}
			else if(typeof(RuleValueComparison).IsAssignableFrom(ruleType)) {
				RuleValueComparison ruleValueComparison = (RuleValueComparison)rule;
				if(ruleValueComparison.Properties.RightOperandExpression == null && ruleValueComparison.Properties.TargetCollectionOwnerType == null) {
					result = GetValueComparisonScript(ruleValueComparison, memberType, currentObject);
				}
			}
			else if(typeof(RuleRange).IsAssignableFrom(ruleType)) {
				RuleRange ruleRange = (RuleRange)rule;
				if(ruleRange.Properties.MinimumValueExpression == null && ruleRange.Properties.MaximumValueExpression == null && ruleRange.Properties.TargetCollectionOwnerType == null) {
					result = GetValueRangeScript((RuleRange)rule, memberType, currentObject);
				}
			}
			return result;
		}
		protected override void ApplyListEditorValidationSettings(ListEditor listEditor) {
			if(listEditor is ASPxGridListEditor) {
				ASPxGridListEditor gridListEditor = (ASPxGridListEditor)listEditor;
				gridListEditor.Grid.SettingsEditing.BatchEditSettings.AllowValidationOnEndEdit = true;
				string allColumnsValidatingScript = "var columnsValidationHandlers = { };";
				foreach(GridViewDataColumn column in gridListEditor.Grid.DataColumns) {
					if(gridListEditor.IsBatchMode && ASPxGridListEditor.UseASPxGridViewDataSpecificColumns) {
						IList<IRule> rulesList = GetRuleListForMember(View.ObjectTypeInfo.Type, column.FieldName, true);
						if(rulesList != null) {
							string allRulesValidatingScript = "";
							foreach(IRule rule in rulesList) {
								allRulesValidatingScript += GetRuleValidationScript(rule, null, View.ObjectTypeInfo.FindMember(column.FieldName).MemberType);
							}
							if(!string.IsNullOrEmpty(allRulesValidatingScript)) {
								allColumnsValidatingScript += string.Format("columnsValidationHandlers[\"{0}\"] = function() {{ {1} }};\r\n", column.FieldName, allRulesValidatingScript);
							}
						}
					}
					else if(column.EditItemTemplate is EditModeDataItemTemplate) {
						EditModeDataItemTemplate editItemTemplate = (EditModeDataItemTemplate)column.EditItemTemplate;
						if(editItemTemplate.PropertyEditor is ASPxPropertyEditor) {
							ASPxPropertyEditor propertyEditor = (ASPxPropertyEditor)editItemTemplate.PropertyEditor;
							propertyEditor.ControlCreated += PropertyEditor_ControlCreated;
						}
					}
				}
				if(!string.IsNullOrEmpty(allColumnsValidatingScript)) {
					string imageUrlsDefinition = DefineErrorImageUrls(@"clientSideValidator.SetErrorImageUrl(""{0}"", ""{1}"");");
					ClientSideEventsHelper.AssignClientHandlerSafe(gridListEditor.Grid, "BatchEditRowValidating", string.Format(@"function(s, e) {{    
                                                                                                 {0};
                                                                                                 for(key in columnsValidationHandlers) {{
                                                                                                     var column = s.GetColumnByField(key);
                                                                                                     if(column) {{
                                                                                                         var columnIndex = column.index;
                                                                                                         var editorValue = e.validationInfo[columnIndex].value;
                                                                                                         var isValid = e.validationInfo[columnIndex].isValid;
                                                                                                         var errorText = e.validationInfo[columnIndex].errorText;        
                                                                                                         var clientSideValidator = new ClientSideValidator(editorValue, errorText);  
                                                                                                         {1}
                                                                                                         columnsValidationHandlers[key]();
                                                                                                         clientSideValidator.Validate();
                                                                                                         isValid = isValid && clientSideValidator.GetIsValid();
                                                                                                         e.validationInfo[columnIndex].isValid = isValid;
                                                                                                         e.validationInfo[columnIndex].errorText =  clientSideValidator.GetErrorMessage();
                                                                                                         if(!isValid) {{
                                                                                                             var errorImageUrl = clientSideValidator.GetErrorImageUrl();
                                                                                                             SetCellErrorImage(s, e.visibleIndex, columnIndex, ""dxGridView_gvCellError_{2}"", errorImageUrl); 
                                                                                                         }}
                                                                                                     }}
                                                                                                 }}
                                                                                             }}", allColumnsValidatingScript, imageUrlsDefinition, BaseXafPage.CurrentTheme), "GridBatchEditRowValidatingHandler");
				}
			}
		}
		protected override void ApplyPropertyEditorValidationSettings(PropertyEditor propertyEditor, IList<IRule> rulesList) {
			if(propertyEditor != null && propertyEditor is ASPxPropertyEditor && rulesList.Count > 0) {
				ASPxPropertyEditor aspxPropertyEditor = (ASPxPropertyEditor)propertyEditor;
				if(aspxPropertyEditor.Editor is ASPxEdit) {
					ASPxEdit editor = (ASPxEdit)aspxPropertyEditor.Editor;
					string allRulesValidatingScript = "";
					string validationOnInitScript = "";
					if(View is ListView) {
						validationOnInitScript = "function(s, e) { s.SetIsValid(true); }";
						editor.ValidationSettings.ErrorTextPosition = ErrorTextPosition.Right;
						editor.ValidationSettings.Display = Display.Static;
					}
					else {
						validationOnInitScript = string.Format(@"function(s, e) {{ SetEditorIsValid(s, ""{0}""); }}", GetErrorImageInfo(ValidationResultType.Error).ImageUrl);
						editor.ValidationSettings.ErrorTextPosition = ErrorTextPosition.Left;
					}
					ClientSideEventsHelper.AssignClientHandlerSafe(editor, "Init", validationOnInitScript, "Validation_ScriptOnInit");
					aspxPropertyEditor.UseEditorErrorCell = true;
					ClientSideEventsHelper.AssignClientHandlerSafe(editor, "LostFocus", "function(s, e) { s.Validate(); }", "Validation_ScriptOnLoad");
					editor.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.ImageWithTooltip;
					foreach(IRule rule in rulesList) {
						allRulesValidatingScript += GetRuleValidationScript(rule, aspxPropertyEditor.CurrentObject, aspxPropertyEditor.MemberInfo.MemberType);
					}
					string imageUrlsDefinition = DefineErrorImageUrls(@"clientSideValidator.SetErrorImageUrl(""{0}"", ""{1}"");");
					string validationEventHandlerScript = string.Format(@"function(s, e) {{
                                                                             var editorValue = e.value;
                                                                             var errorText = e.isValid ? """" : e.errorText;
                                                                             var clientSideValidator = new ClientSideValidator(editorValue, errorText);
                                                                             {0}
                                                                             {1}
                                                                             clientSideValidator.Validate();
                                                                             var isValid = clientSideValidator.GetIsValid();
                                                                             e.isValid = e.isValid && isValid;
                                                                             if(!e.isValid) {{                                                                                 
                                                                                 e.errorText = clientSideValidator.GetErrorMessage();
                                                                                 var errorImageUrl = clientSideValidator.GetErrorImageUrl();
                                                                                 SetEditorErrorImage(s, errorImageUrl);
                                                                             }} 
                                                                             ApplyValidationCssClass(s, e.isValid);
                                                                         }}", imageUrlsDefinition, allRulesValidatingScript);
					ClientSideEventsHelper.AssignClientHandlerSafe(editor, "Validation", validationEventHandlerScript, "ValidationScript");
				}
			}
		}
#if DebugTest
		public void DebugTest_ApplyPropertyEditorValidationSettings(ASPxPropertyEditor propertyEditor, IList<IRule> rulesList) {
			ApplyPropertyEditorValidationSettings(propertyEditor, rulesList);
		}
		public void DebugTest_ApplyListEditorValidationSettings(ListEditor listEditor) {
			ApplyListEditorValidationSettings(listEditor);
		}
		public string DebugTest_GetRequiredFieldScript(RuleRequiredField rule, object targetObject) {
			return GetRequiredFieldScript(rule, targetObject);
		}
		public string DebugTest_GetRegularExpressionScript(RuleRegularExpression rule, object targetObject) {
			return GetRegularExpressionScript(rule, targetObject);
		}
		public string DebugTest_GetValueRangeScript(RuleRange rule, Type memberType, object targetObject) {
			return GetValueRangeScript(rule, memberType, targetObject);
		}
		public string DebugTest_GetStringComparisonScript(RuleStringComparison rule, object targetObject) {
			return GetStringComparisonScript(rule, targetObject);
		}
		public string DebugTest_GetValueComparisonScript(RuleValueComparison rule, Type memberType, object targetObject) {
			return GetValueComparisonScript(rule, memberType, targetObject);
		}
		public string DebugTest_GetOperandsInitializationScript(Type memberType, object rightOperand) {
			return GetOperandsInitializationScript(memberType);
		}
		public string DebugTest_GetRangeInitializationScript(Type memberType, object minimumValue, object maximumValue) {
			return GetRangeInitializationScript(memberType, minimumValue, maximumValue);
		}
#endif
	}
}
