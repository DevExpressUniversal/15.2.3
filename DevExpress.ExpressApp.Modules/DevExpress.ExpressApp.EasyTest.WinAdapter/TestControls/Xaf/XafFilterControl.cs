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
using System.Reflection;
using DevExpress.Utils.Menu;
using DevExpress.ExpressApp.DC;
using System.Collections.Generic;
using DevExpress.ExpressApp.Editors;
using DevExpress.EasyTest.Framework;
using System.Text.RegularExpressions;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Filtering;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Utils;
using DevExpress.XtraFilterEditor;
using DevExpress.XtraEditors;
namespace DevExpress.ExpressApp.EasyTest.WinAdapter.TestControls.Xaf {
	public class PopupCriteriaEditTestControl : IControlAct, IControlReadOnlyText {
		private PopupCriteriaEdit control;
		public PopupCriteriaEditTestControl(PopupCriteriaEdit control) {
			this.control = control;
		}
		#region IControlAct Members
		public void Act(string value) {
			control.Focus();
			control.GetType().GetMethod("OnClickButton", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(control, new object[] { new EditorButtonObjectInfoArgs(control.Properties.Buttons[0], control.Properties.Appearance) });
		}
		#endregion
		#region IControlReadOnlyText Members
		public string Text {
			get { return control.Text; }
		}
		#endregion
	}
	public class FilterEditorControlAdapter : TestControlTextValidated<FilterEditorControl>, IControlAct {
		public FilterEditorControlAdapter(FilterEditorControl control)
			: base(control) {
		}
		private object GetNonPublicPropertyValue(object control, string propertyName, object[] param) {
			PropertyInfo property = control.GetType().BaseType.GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Instance);
			if(property == null) {
				property = control.GetType().GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Instance);
			}
			return property.GetValue(control, param);
		}
		private object GetNonPublicPropertyValue(string propertyName, object[] param) {
			return GetNonPublicPropertyValue(GetFilterControl(control), propertyName, param);
		}
		private void InvokeNonPublicMethod(object control, string methodName, object[] param) {
			MethodInfo method = control.GetType().BaseType.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
			if(method == null) {
				method = control.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
			}
			method.Invoke(control, param);
		}
		private void InvokeNonPublicMethod(string methodName, object[] param) {
			InvokeNonPublicMethod(GetFilterControl(control), methodName, param);
		}
		private int ConvertIndexFromString(string nodeIndex) {
			int index;
			if(!int.TryParse(nodeIndex, out index)) {
				throw new AdapterOperationException("'" + nodeIndex + "' is not valid integer value.");
			}
			return index;
		}
		private void SetFocusedNode(int nodeIndex, int elementIndex) {
			ClauseNode conditionNode = GetNodeByIndex<ClauseNode>(nodeIndex);
			FilterControlFocusInfo info = new FilterControlFocusInfo(conditionNode, elementIndex);
			PropertyInfo propertyInfo = GetFilterControl(control).GetType().GetProperty("FocusInfo", BindingFlags.NonPublic | BindingFlags.Instance);
			propertyInfo.SetValue(GetFilterControl(control), info, null);
		}
		private GroupType ConvertGroupTypeFromString(string str) {
			return (GroupType)Enum.Parse(typeof(GroupType), str, true);
		}
		private FilterControl GetFilterControl(FilterEditorControl control) {
			return (FilterControl)control.GetType().GetProperty("Tree", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(control, null);
		}
		private GroupNode GetRootNode() {
			MemberInfo[] rootNodeProperty = GetFilterControl(control).GetType().GetMember("RootNode", BindingFlags.NonPublic | BindingFlags.Instance);
			GroupNode rootNode = ((GroupNode)((PropertyInfo)(rootNodeProperty.GetValue(0))).GetValue(GetFilterControl(control), null));
			return rootNode;
		}
		private void EnumerateNodes(GroupNode rootNode, ref List<INode> nodes) {
			nodes.Add(rootNode);
			foreach(INode subNode in rootNode.SubNodes) {
				if(subNode is GroupNode) {
					EnumerateNodes(subNode as GroupNode, ref nodes);
				}
				else {
					nodes.Add(subNode);
				}
			}
		}
		private T GetNodeByIndex<T>(GroupNode parentNode, int nodeIndex) where T : INode {
			List<INode> nodes = new List<INode>();
			EnumerateNodes(parentNode, ref nodes);
			if(nodeIndex < nodes.Count) {
				return (T)nodes[nodeIndex];
			}
			else {
				throw new AdapterOperationException(nodeIndex + " is too large. There are only " + nodes.Count + " nodes.");
			}
		}
		private T GetNodeByIndex<T>(int nodeIndex) where T : INode {
			return GetNodeByIndex<T>(GetRootNode(), nodeIndex);
		}
		private void AddCondition(int groupNodeIndex) {
			GetNodeByIndex<GroupNode>(groupNodeIndex).AddElement();
		}
		private void RemoveCriteria() {
			control.FilterString = "";
		}
		private void AddGroup(int groupNodeIndex) {
			InvokeNonPublicMethod("AddGroup", new object[] { GetNodeByIndex<GroupNode>(groupNodeIndex) });
		}
		private void ChangeGroupType(int groupNodeIndex, GroupType newType) {
			GetNodeByIndex<GroupNode>(groupNodeIndex).NodeType = newType;
		}
		private void ChangeFieldName(int nodeIndex, string fieldName) {
			SetFocusedNode(nodeIndex,  0);
			fieldName = fieldName.Replace(" ", "");
			InvokeNonPublicMethod("MenuItemPropertyClick", new object[] { fieldName });
		}
		public virtual void Act(string parameterValue) {
			if(control.Enabled) {
				if(string.Compare(parameterValue, "Text", true) == 0) {
					control.ActiveView = FilterEditorActiveView.Text;
					return;
				}
				else if(string.Compare(parameterValue, "Visual", true) == 0) {
					control.ActiveView = FilterEditorActiveView.Visual;
					return;
				}
				else {
					string[] param = parameterValue.Split('|');
					List<string> newParam = new List<string>(param);
					newParam.RemoveAt(0);
					if(param[0] == "AddCondition") {
						AddCondition(ConvertIndexFromString(param[1]));
						return;
					}
					if(param[0] == "AddGroup") {
						AddGroup(ConvertIndexFromString(param[1]));
						return;
					}
					if(param[0] == "ChangeGroupType") {
						ChangeGroupType(ConvertIndexFromString(param[1]), ConvertGroupTypeFromString(param[2]));
						return;
					}
					if(param[0] == "ChangeValue") {
						ChangeValue(param[1], param[2]);
						return;
					}
					if(param[0] == "ChangeFieldName") {
						ChangeFieldName(ConvertIndexFromString(param[1]), param[2]);
						return;
					}
					if(param[0] == "ChangeOperation") {
						ChangeOperation(param[1], param[2]);
						return;
					}
					if(param[0] == "RemoveCriteria") {
						RemoveCriteria();
						return;
					}
					throw new WarningException("The '" + param[0] + "' command is not supported. Use only 'Text', 'Visual', 'AddCondition', 'ChangeFieldName', 'ChangeValue', 'ChangeOperation' commands.");
				}
			}
			else {
				throw new AdapterOperationException(String.Format("The '{0}' control is disabled", TestControl.Name));
			}
		}
		public void ChangeOperation(string nodeIndex, string operationName) {
			GroupNode rootNode = GetRootNode();
			SetFocusedNode(ConvertIndexFromString(nodeIndex), 1);
			DXMenuItem newOperationItem = null;
			foreach(DXMenuItem item in control.FilterViewInfo.GetClauseMenu().Items) {
				if(item.Caption == operationName) {
					newOperationItem = item;
					break;
				}
			}
			if(newOperationItem == null) {
				throw new AdapterOperationException("Unable to change operation to '" + operationName + "'. There are no such menu item.");
			}
			else {
				InvokeNonPublicMethod(newOperationItem, "OnClick", new object[] { });
			}
		}
		protected internal void ShowValueEditorForCurrentNode(GroupNode rootNode, int nodeIndex) {
			ClauseNode node = GetNodeByIndex(rootNode, nodeIndex) as ClauseNode;
			if(node != null) {
				while((node.AdditionalOperands[0] is OperandParameter) || (node.AdditionalOperands[0] is OperandProperty)) {
					node.ChangeElement(ElementType.FieldAction);
				}
				ShowValueEditorForCurrentNodeCore(node);
			}
		}
		protected internal void ShowParameterEditorForCurrentNode(GroupNode rootNode, int nodeIndex) {
			ClauseNode node = GetNodeByIndex(rootNode, nodeIndex) as ClauseNode;
			if(node != null) {
				while(!(node.AdditionalOperands[0] is OperandParameter)) {
					node.ChangeElement(ElementType.FieldAction);
				}
				ShowValueEditorForCurrentNodeCore(node);
			}
		}
		void ShowValueEditorForCurrentNodeCore(Node node) {
			int valueIndex = -1;
			foreach(NodeEditableElement element in node.Elements) {
				if(element.IsValueElement) {
					valueIndex = element.Index;
					break;
				}
			}
			if(valueIndex > -1) {
				SetFocusedNode(node.Index, valueIndex);
				InvokeNonPublicMethod("CreateActiveEditor", new object[] { });
			}
		}
		private INode GetNodeByIndex(GroupNode rootNode, int nodeIndex) {
			List<INode> nodes = new List<INode>();
			EnumerateNodes(rootNode, ref nodes);
			if(nodeIndex < nodes.Count) {
				return nodes[nodeIndex];
			}
			else {
				throw new AdapterOperationException(nodeIndex + " is too large. There are only " + nodes.Count + " nodes.");
			}
		}
		public void ChangeValue(string nodeIndex, string param) {
			GroupNode rootNode = GetRootNode();
			ShowValueEditorForCurrentNode(rootNode, int.Parse(nodeIndex));
			ITestControl testControl = TestControlFactoryWin.Instance.CreateControl(control.ActiveEditor);
			testControl.FindInterface<IControlText>().Text = param;
			control.FindForm().SelectNextControl(control, true, false, true, true); 
		}
		protected override void InternalSetText(string text) {
			control.FilterString = text;
		}
		protected override string GetText() {
			return (string)GetNonPublicPropertyValue(control, "EditorText", new object[0]);
		}
	}
}
