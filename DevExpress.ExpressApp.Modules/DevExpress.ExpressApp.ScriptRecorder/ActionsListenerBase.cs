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
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
namespace DevExpress.ExpressApp.ScriptRecorder {
	public class ScriptRecorderActionsListenerBase : ViewListener<List<ActionBase>> {
		private bool actionExecuting = false;
		private ActionBase currentActionExecuting;
		public const string ProcessRecordPrefix = "*ProcessRecord";
		public ScriptRecorderActionsListenerBase(List<ActionBase> actions) {
			obj = actions;
			RegisterControl(actions);
		}
		private void Action_Executed(object sender, ActionBaseEventArgs e) {
			actionExecuting = false;
			if(currentActionExecuting == e.Action) {
				currentActionExecuting = null;
			}
		}
		private void Action_Executing(object sender, CancelEventArgs e) {
			WriteActionExecuting((ActionBase)sender);
		}
		private void CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e) {
			WriteActionExecuting((ActionBase)sender);
			actionExecuting = false; 
			currentActionExecuting = null; 
		}
		private void CheckActiveTab(ActionBase action) {
			if(action.Controller != null && action.Controller.Frame != null && action.Controller.Frame.View != null) {
				ControlNameHelper.Instance.GetFullViewName(action.Controller.Frame.View);
			}
		}
		private string GetSingleChoiceActionFullName(ChoiceActionItem item) {
			string result = item.Caption;
			if(item.ParentItem != null) {
				result = GetSingleChoiceActionFullName(item.ParentItem) + "." + result;
			}
			return result;
		}
		private string GetPropertyName(IMemberInfo memberInfo) {
			return memberInfo.BindingName.Trim('!').Split('.')[0];
		}
		private IModelColumn FindByPropertyName(IModelListView model, string propertyName) {
			IModelColumn columnInfo = null;
			foreach(IModelColumn colInfo in model.Columns) {
				if(colInfo.PropertyName == propertyName) {
					columnInfo = colInfo;
					break;
				}
			}
			return columnInfo;
		}
		private bool IsLoggedActionExecuting(ActionBase action) {
			bool result = true;
			if(!(action is PopupWindowShowAction)) {
				if(currentActionExecuting == null) {
					currentActionExecuting = action;
				}
				else {
					result = false;
				}
			}
			return result;
		}
		protected Dictionary<string, object> GetParameters(ListEditor editor) {
			Dictionary<string, object> result = new Dictionary<string, object>();
			ITypeInfo objectTypeInfo = XafTypesInfo.Instance.FindTypeInfo(editor.FocusedObject.GetType());
			IMemberInfo memberInfo = null;
			IModelColumn columnInfo = null;
			string propertyName = null;
			if(objectTypeInfo.DefaultMember != null) {
				memberInfo = objectTypeInfo.DefaultMember;
				columnInfo = FindByPropertyName(editor.Model, GetPropertyName(memberInfo));
			}
			if(columnInfo != null && columnInfo.Index != -1) {
				propertyName = columnInfo.Caption;
			}
			else {
				IList<IModelColumn> visibleColumns = editor.Model.Columns.GetVisibleColumns();
				if(visibleColumns.Count < 1) {
					return result;
				}
				columnInfo = visibleColumns[0];
				if(columnInfo.ModelMember != null) {
					string property = columnInfo.ModelMember.Name;
					IMemberInfo displayMember = new ObjectEditorHelperBase(columnInfo.ModelMember.MemberInfo.MemberTypeInfo, columnInfo).DisplayMember;
					if(displayMember != null) {
						property += "." + displayMember.Name;
					}
					memberInfo = objectTypeInfo.FindMember(property);
				}
				if(memberInfo == null) {
					return result;
				}
				columnInfo = FindByPropertyName(editor.Model, GetPropertyName(memberInfo));
				if(columnInfo == null) {
					return result;
				}
				propertyName = columnInfo.Caption;
			}
			result.Add(propertyName, memberInfo.GetValue(editor.FocusedObject));
			return result;
		}
		protected string GetParametersToString(Dictionary<string, object> param) {
			string result = null;
			foreach(string propertyName in param.Keys) {
				object value = null;
				param.TryGetValue(propertyName, out value);
				result += String.Format(Environment.NewLine + " {0} = {1}", propertyName, value);
			}
			return result;
		}
		protected void WriteProcessRecord(ListView view) {
			if(view.Editor.FocusedObject == null) { 
				return;
			}
			string viewFullName = ControlNameHelper.Instance.GetFullViewName(view);
			if(viewFullName != null) {
				viewFullName = " " + viewFullName;
			}
			Dictionary<string, object> propertyNameAndValue = GetParameters(view.Editor);
			WriteMessage(ProcessRecordPrefix + viewFullName + GetParametersToString(propertyNameAndValue));
		}
		protected void WriteActionExecuting(ActionBase action) {
			if(!IsLoggedActionExecuting(action)) {
				return;
			}
			actionExecuting = true;
			if(action.Controller is ListViewProcessCurrentObjectController) {
				if(action.SelectionContext is ListView) {
					WriteProcessRecord((ListView)action.SelectionContext);
				}
			}
			else {
				CheckActiveTab(action); 
				if(action.SelectionDependencyType != SelectionDependencyType.Independent && action.SelectionContext is ListView) {
					WriteSelectRecord(action, true, 0);
				}
				if(action is SingleChoiceAction) {
					WriteActionExecute((SingleChoiceAction)action);
				}
				else {
					if(action is ParametrizedAction) {
						WriteComplexAction(action, ConvertValueToString(((ParametrizedAction)action).Value));
					}
					else {
						WriteMessage("*Action " + GetActionFullName(action));
					}
				}
				WriteConfirmationMessage(action);
			}
		}
		protected void WriteSelectRecord(ActionBase action, bool isCheckRowsValue, int visibleColumnIndex) {
			ListEditor editor = ((ListView)action.SelectionContext).Editor;
			WriteSelectRecord(editor, (ObjectView)action.SelectionContext, isCheckRowsValue, visibleColumnIndex);
		}
		protected void WriteActionExecute(SingleChoiceAction action) {
			if(action.SelectedItem != null) {
				if(action.Items.Count > 1 || GetActionFullName(action) == "Navigation") {
					if(action.SelectedItem.Items.Count == 0 || action.SelectedItem.Data is ViewShortcut) {
						WriteComplexAction(action, GetSingleChoiceActionFullName(action.SelectedItem));
					}
				}
				else {
					WriteMessage("*Action " + GetActionFullName(action));
				}
			}
		}
		protected virtual void WriteComplexAction(ActionBase action, string value) {
			WriteMessage(String.Format("*Action {0}({1})", GetActionFullName(action), value));
		}
		protected void WriteConfirmationMessage(ActionBase action) {
			if(!string.IsNullOrEmpty(action.ConfirmationMessage)) {
				WriteConfirmationMessage("Yes");
			}
		}
		protected void WriteConfirmationMessage(string dialogResult) {
			if(actionExecuting) {
				WriteMessage("*HandleDialog");
				WriteMessage(" Respond = " + dialogResult);
				actionExecuting = false;
			}
		}
		public override void RegisterControl(List<ActionBase> actions) {
			foreach(ActionBase action in actions) {
				if(action is PopupWindowShowAction) {
					((PopupWindowShowAction)action).CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(CustomizePopupWindowParams);
				}
				else {
					action.Executing += new CancelEventHandler(Action_Executing);
					action.Executed += new EventHandler<ActionBaseEventArgs>(Action_Executed);
				}
			}
		}
		public override void UnRegisterControl(List<ActionBase> actions) {
			foreach(ActionBase action in actions) {
				if(action is PopupWindowShowAction) {
					((PopupWindowShowAction)action).CustomizePopupWindowParams -= new CustomizePopupWindowParamsEventHandler(CustomizePopupWindowParams);
				}
				action.Executing -= new CancelEventHandler(Action_Executing);
				action.Executed -= new EventHandler<ActionBaseEventArgs>(Action_Executed);
			}
		}
		public override string EmptyValue {
			get { return ""; }
		}
		public IEnumerable<ActionBase> Actions {
			get { return obj; }
		}
		public virtual string GetActionFullName(ActionBase action) {
			string result = action.Caption;
			if(action.Controller != null && action.Controller.Frame != null && action.Controller.Frame.View != null) {
				result = ControlNameHelper.Instance.GetFullName(action.Controller.Frame.View, action.Caption, action);
			}
			return result;
		}
	}
}
