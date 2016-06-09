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
using System.Text;
using System.Windows.Forms;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.ExpressApp.Win.Editors.Grid;
using DevExpress.Persistent.Validation;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
namespace DevExpress.ExpressApp.Validation.Win {
	public class InplaceEditorsValidationControllerWin : InplaceEditorsValidationControllerBase {
		private ErrorMessages listEditorInplaceErrorMessages = new ErrorMessages();
		private ErrorMessages currentViewErrorMessages;
		private DXValidationProvider validationProvider;
		private Dictionary<Type, DXValidationRuleAdapterBase> validationRuleAdapters = new Dictionary<Type, DXValidationRuleAdapterBase>();
		private void Control_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
			validationProvider.Validate(sender as Control);
		}
		private void Control_InvalidValue(object sender, XtraEditors.Controls.InvalidValueExceptionEventArgs e) {
			e.ExceptionMode = XtraEditors.Controls.ExceptionMode.DisplayError;
		}
		private void RegisterValidationRuleAdapters() {
			RegisterValidationRuleAdapter(new DXValidationRuleRequiredAdapter(), typeof(RuleRequiredField));
			RegisterValidationRuleAdapter(new DXValidationRuleRangeAdapter(), typeof(RuleRange));
			RegisterValidationRuleAdapter(new DXValidationRuleValueComparisonAdapter(), typeof(RuleValueComparison));
			RegisterValidationRuleAdapter(new DXValidationRuleStringComparisonAdapter(), typeof(RuleStringComparison));
			RegisterValidationRuleAdapter(new DXRuleRegularExpressionAdapter(), typeof(RuleRegularExpression));
		}
		private void ErrorMessages_MessagesChanged(object sender, EventArgs e) {
			listEditorInplaceErrorMessages.Clear();
		}
		private ComplexValidationRule CreateComplexRule(object currentObject, IList<IRule> rulesList) {
			ComplexValidationRule complexRule = new ComplexValidationRule();
			foreach(IRule rule in rulesList) {
				DXValidationRuleAdapterBase ruleAdapter = null;
				if(validationRuleAdapters.TryGetValue(rule.GetType(), out ruleAdapter) && ruleAdapter != null) {
					ValidationRule dxValidationRule = ruleAdapter.GetDXValidationRule(rule, currentObject);
					if(dxValidationRule != null) {
						complexRule.Rules.Add(dxValidationRule);
					}
				}
			}
			return complexRule;
		}
		private void GridView_ValidatingEditor(object sender, XtraEditors.Controls.BaseContainerValidateEditorEventArgs e) {
			GridView gridView = (GridView)sender;
			ListView listView = (ListView)View;
			GridListEditor listEditor = (GridListEditor)listView.Editor;
			object currentObject = listEditor.FocusedObject;
			if(currentObject != null) {
				ProcessGridViewValidatingEditor(gridView, currentObject, e.Value);
			}
		}
		protected static ImageInfo GetErrorImageInfo(ErrorType errorType) {
			return GetErrorImageInfo(DXValidationRuleAdapterBase.GetXafValidationResultByErrorType(errorType));
		}
		protected void ProcessGridViewValidatingEditor(GridView gridView, object targetObject, object editorValue) {
			GridColumn focusedColumn = gridView.FocusedColumn;
			if(focusedColumn != null) {
				string propertyName = focusedColumn.FieldName;
				IList<IRule> rulesList = GetRuleListForMember(targetObject.GetType(), propertyName, true);
				if(rulesList != null) {
					currentViewErrorMessages.RemoveMessages(propertyName, targetObject, false);
					listEditorInplaceErrorMessages.RemoveMessages(propertyName, targetObject);
					ComplexValidationRule complexRule = CreateComplexRule(targetObject, rulesList);
					if(!complexRule.Validate(null, editorValue)) {
						listEditorInplaceErrorMessages.AddMessage(propertyName, targetObject, complexRule.ErrorText, GetErrorImageInfo(complexRule.ErrorType));
					}
				}
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			RegisterValidationRuleAdapters();
			currentViewErrorMessages = View.ErrorMessages;
		}
		protected override void ApplyPropertyEditorValidationSettings(PropertyEditor propertyEditor, IList<IRule> rulesList) {
			if(propertyEditor is DXPropertyEditor) {
				DXPropertyEditor dxPropertyEditor = (DXPropertyEditor)propertyEditor;
				dxPropertyEditor.Control.InvalidValue += Control_InvalidValue;
				dxPropertyEditor.Control.Validating += Control_Validating;
				ComplexValidationRule complexRule = CreateComplexRule(dxPropertyEditor.CurrentObject, rulesList);
				validationProvider.SetValidationRule(dxPropertyEditor.Control, complexRule);
			}
		}
		protected override void ApplyListEditorValidationSettings(ListEditor listEditor) {
			if(listEditor is GridListEditor) {
				GridListEditor gridListEditor = (GridListEditor)listEditor;
				gridListEditor.GetColumnError += gridListEditor_GetColumnError;
				if(gridListEditor.GridView != null) {
					gridListEditor.GridView.ValidatingEditor += GridView_ValidatingEditor;
					if(currentViewErrorMessages != null) {
						currentViewErrorMessages.MessagesChanged += ErrorMessages_MessagesChanged;
					}
				}
			}
		}
		private void gridListEditor_GetColumnError(object sender, GetColumnErrorEventArgs e) {
			ErrorMessage message = listEditorInplaceErrorMessages.GetMessage(e.PropertyName, e.TargetObject);
			if(message != null) {
				e.Handled = true;
				e.Message = message.Message;
				e.ErrorType = XtraGridUtils.GetErrorTypeByName(message.Icon.ImageName);
			}
		}		
		public void RegisterValidationRuleAdapter(DXValidationRuleAdapterBase ruleAdapter, Type xafValidationRuleType) {
			if(!validationRuleAdapters.ContainsKey(xafValidationRuleType)) {
				validationRuleAdapters.Add(xafValidationRuleType, ruleAdapter);
			}
			else {
				validationRuleAdapters[xafValidationRuleType] = ruleAdapter;
			}
		}
		static InplaceEditorsValidationControllerWin() {
			DXErrorProvider.GetErrorIcon += DXErrorProvider_GetErrorIcon;
		}
		static void DXErrorProvider_GetErrorIcon(GetErrorIconEventArgs e) {
			ValidationResultType xafResultType = DXValidationRuleAdapterBase.GetXafValidationResultByErrorType(e.ErrorType);
			ImageInfo errorImageInfo = GetErrorImageInfo(xafResultType);
			if(!errorImageInfo.IsEmpty) {
				e.ErrorIcon = errorImageInfo.Image;
			}			
		}
		public InplaceEditorsValidationControllerWin()
			: base() {
			validationProvider = new DXValidationProvider();
			validationProvider.ValidationMode = ValidationMode.Manual;			
		}
#if DebugTest
		public void DebugTest_ProcessGridViewValidatingEditor(GridView gridView, object targetObject, object editorValue) {
			ProcessGridViewValidatingEditor(gridView, targetObject, editorValue);
		}
		public void DebugTest_RegisterValidationRuleAdapters() {
			RegisterValidationRuleAdapters();
		}
		public void DebugTest_SubscribeErrorMessagesChangedEvent() {
			currentViewErrorMessages.MessagesChanged += ErrorMessages_MessagesChanged;
		}
		public ErrorMessages DebugTest_ListEditorErrorMessages {
			get { return listEditorInplaceErrorMessages; }
		}
		public ErrorMessages DebugTest_CurrentViewErrorMessages {
			get { return currentViewErrorMessages; }
			set { currentViewErrorMessages = value; }
		}
#endif
	}
}
