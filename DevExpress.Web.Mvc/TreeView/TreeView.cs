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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.UI;
namespace DevExpress.Web.Mvc {
	using DevExpress.Web.Internal;
	using DevExpress.Web;
	using DevExpress.Web.Mvc.Internal;
	[ToolboxItem(false)]
	public class MVCxTreeView : ASPxTreeView {
		protected const string NodesInfoKey = "nodesInfo";
		public MVCxTreeView()
			: base() {
		}
		public object CallbackRouteValues { get; set; }
		public new TreeViewImages Images {
			get { return base.Images; }
		}
		public new TreeViewStyles Styles {
			get { return base.Styles; }
		}
		protected internal new TreeViewDataMediator DataMediator { get { return base.DataMediator; } }
		public override bool IsCallback {
			get { return MvcUtils.CallbackName == ID; }
		}
		protected internal override bool IsCallBacksEnabled() {
			return CallbackRouteValues != null;
		}
		protected override string GetCallbackResultHtml() {
			return Utils.CallbackHtmlContentPlaceholder;
		}
		protected internal new Control GetCallbackResultControl() {
			if(!string.IsNullOrEmpty(NodeCheckedRecursiveOnCallbackID))
				return null; 
			return base.GetCallbackResultControl();
		}
		protected internal new string NodeExpandedOnCallbackID {
			get { return base.NodeExpandedOnCallbackID; }
			set { base.NodeExpandedOnCallbackID = value; }
		}
		protected internal new MVCxTreeViewNode FindNodeByID(string id) {
			return (MVCxTreeViewNode)base.FindNodeByID(id);
		}
		protected override void GetCreateClientObjectScript(System.Text.StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(CallbackRouteValues != null)
				stb.Append(localVarName + ".callbackUrl=\"" + Utils.GetUrl(CallbackRouteValues) + "\";\n");
		}
		protected override Hashtable GetClientObjectState() {
			Hashtable result = new Hashtable();
			result.Add(NodesInfoKey, TreeViewState.GetSerializedNodesInfo(Nodes));
			return result;
		}
		protected override string GetClientObjectClassName() {
			return "MVCxClientTreeView";
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(MVCxTreeView), Utils.UtilsScriptResourceName);
			RegisterIncludeScript(typeof(MVCxTreeView), Utils.TreeViewScriptResourceName);
		}
		public override bool IsLoading() {
			return false;
		}
		protected internal new void EnsureClientStateLoaded() {
			base.EnsureClientStateLoaded();
		}
		protected override bool NeedLoadClientState() {
			return !IsCallback;
		}
		protected override object GetExpandNodeOnCallbackResult() {
			object[] result = (object[])base.GetExpandNodeOnCallbackResult();
			int oldResultLength = result.Length;
			Array.Resize(ref result, oldResultLength + 1);
			TreeViewState state = GetState(ID);
			TreeViewNodeState expandedNodeState = state.FindNodeStateByID(NodeExpandedOnCallbackID);
			if(expandedNodeState != null) { 
				IEnumerable<TreeViewNodeState> callbackNodesState = new TreeViewNodeState[CallbackNodes.Count];
				for(int i = 0; i < CallbackNodes.Count; i++) {
					var node = new TreeViewNodeState();
					((TreeViewNodeState[])callbackNodesState)[i] = node;
					node.Index = CallbackNodes[i].Index;
					node.Name = CallbackNodes[i].Name;
					node.Text = CallbackNodes[i].Text;
				}
				expandedNodeState.Nodes = callbackNodesState;
			}
			result[oldResultLength] = state.CreateSerializedNodesInfo();
			return result;
		}
		internal static TreeViewState GetState(string name) {
			Hashtable clientObjectState = LoadClientObjectState(HttpContext.Current.Request.Params, name);
			if(clientObjectState == null) return null;
			string nodesInfo = GetClientObjectStateValue<string>(clientObjectState, NodesInfoKey);
			ArrayList nodesState = GetClientObjectStateValue<ArrayList>(clientObjectState, NodesStateKey);
			return TreeViewState.Load(nodesInfo, nodesState);
		}
	}
}
