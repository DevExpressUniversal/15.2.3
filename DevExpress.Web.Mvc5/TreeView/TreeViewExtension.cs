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
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
namespace DevExpress.Web.Mvc {
	using DevExpress.Web.Internal;
	using DevExpress.Web;
	using DevExpress.Web.Mvc.Internal;
	public delegate void TreeViewVirtualModeCreateChildrenMethod(TreeViewVirtualModeCreateChildrenEventArgs args);
	public class TreeViewExtension: ExtensionBase {
		public TreeViewExtension(TreeViewSettings settings)
			: base(settings) {
		}
		public TreeViewExtension(TreeViewSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		protected internal new MVCxTreeView Control {
			get { return (MVCxTreeView)base.Control; }
		}
		protected internal new TreeViewSettings Settings {
			get { return (TreeViewSettings)base.Settings; }
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			Control.AllowCheckNodes = Settings.AllowCheckNodes;
			Control.AllowSelectNode = Settings.AllowSelectNode;
			Control.CallbackRouteValues = Settings.CallbackRouteValues;
			Control.ClientSideEvents.Assign(Settings.ClientSideEvents);
			Control.ClientVisible = Settings.ClientVisible;
			Control.CheckNodesRecursive = Settings.CheckNodesRecursive;
			Control.EnableAnimation = Settings.EnableAnimation;
			Control.EnableClientSideAPI = Settings.EnableClientSideAPI;
			Control.EnableHotTrack = Settings.EnableHotTrack;
			Control.Images.CopyFrom(Settings.Images);
			Control.ImageUrlField = Settings.ImageUrlField;
			Control.NameField = Settings.NameField;
			Control.NavigateUrlField = Settings.NavigateUrlField;
			Control.NavigateUrlFormatString = Settings.NavigateUrlFormatString;
			Control.NodeImagePosition = Settings.NodeImagePosition;
			Control.Nodes.Assign(Settings.Nodes);
			Control.NodeLinkMode = Settings.NodeLinkMode;
			Control.SaveStateToCookies = Settings.SaveStateToCookies;
			Control.SaveStateToCookiesID = Settings.SaveStateToCookiesID;
			Control.SettingsLoadingPanel.Assign(Settings.SettingsLoadingPanel);
			Control.ShowExpandButtons = Settings.ShowExpandButtons;
			Control.ShowTreeLines = Settings.ShowTreeLines;
			Control.Styles.CopyFrom(Settings.Styles);
			Control.SyncSelectionMode = Settings.SyncSelectionMode;
			Control.Target = Settings.Target;
			Control.TextField = Settings.TextField;
			Control.TextFormatString = Settings.TextFormatString;
			Control.ToolTipField = Settings.ToolTipField;
			Control.AccessibilityCompliant = Settings.AccessibilityCompliant;
			Control.RightToLeft = Settings.RightToLeft;
			Control.BeforeGetCallbackResult += Settings.BeforeGetCallbackResult;
			Control.ClientLayout += Settings.ClientLayout;
			Control.CustomJSProperties += Settings.CustomJSProperties;
			Control.NodeDataBound += Settings.NodeDataBound;
			Control.DataBinding += Settings.DataBinding;
			Control.DataBound += Settings.DataBound;
			Control.VirtualModeCreateChildren += Settings.VirtualModeCreateChildrenInternal;
		}
		protected override void AssignRenderProperties() {
			base.AssignRenderProperties();
			Control.NodeTemplate = ContentControlTemplate<TreeViewNodeTemplateContainer>.Create(
				Settings.NodeTemplateContent, Settings.NodeTemplateContentMethod, typeof(TreeViewNodeTemplateContainer));
			Control.NodeTextTemplate = ContentControlTemplate<TreeViewNodeTemplateContainer>.Create(
				Settings.NodeTextTemplateContent, Settings.NodeTextTemplateContentMethod, typeof(TreeViewNodeTemplateContainer));
			if(!IsCallback() || IsExpandAllCommand()) {
				for(int i = 0; i < Control.Nodes.Count; i++) {
					MVCxTreeViewNode sourceNode = (i < Settings.Nodes.Count) ? Settings.Nodes[i] : null;
					AssignNodeTemplates(sourceNode, Control.Nodes[i]);
				}
			}
		}
		protected void AssignNodeTemplates(MVCxTreeViewNode sourceNode, TreeViewNode destinationNode) {
			if(sourceNode == null) return;
			destinationNode.Template = ContentControlTemplate<TreeViewNodeTemplateContainer>.Create(
				sourceNode.TemplateContent, sourceNode.TemplateContentMethod, typeof(TreeViewNodeTemplateContainer));
			destinationNode.TextTemplate = ContentControlTemplate<TreeViewNodeTemplateContainer>.Create(
				sourceNode.TextTemplateContent, sourceNode.TextTemplateContentMethod, typeof(TreeViewNodeTemplateContainer));
			for(int i = 0; i < destinationNode.Nodes.Count; i++) {
				MVCxTreeViewNode sourceSubNode = (sourceNode != null && i < sourceNode.Nodes.Count) ? sourceNode.Nodes[i] : null;
				AssignNodeTemplates(sourceSubNode, destinationNode.Nodes[i]);
			}
		}
		protected void AssignExpandedNodeTemplates(string expandedNodeId) {
			if(!String.IsNullOrEmpty(expandedNodeId)) {
				MVCxTreeViewNode expandedNode = Control.FindNodeByID(expandedNodeId);
				int[] pathIndices = Array.ConvertAll<string, int>(
					expandedNode.GetIndexPath().Split('_'),
					delegate(string str) { return int.Parse(str); }
				);
				MVCxTreeViewNode sourceExpandedNode = Settings.Nodes[pathIndices[0]];
				if(sourceExpandedNode == null)
					return;
				for(int i = 1; i < pathIndices.Length; i++) {
					sourceExpandedNode = sourceExpandedNode.Nodes[pathIndices[i]];
				}
				for(int i = 0; i < expandedNode.Nodes.Count; i++) {
					AssignNodeTemplates(sourceExpandedNode.Nodes[i], expandedNode.Nodes[i]);
				}
			}
		}
		public TreeViewExtension Bind(object dataObject) {
			BindInternal(dataObject);
			return this;
		}
		public TreeViewExtension BindToVirtualData(TreeViewVirtualModeCreateChildrenMethod method) {
			if(method != null)
				Control.VirtualModeCreateChildren += (sender, e) => {
					method(e);
				};
			return this;
		}
		public TreeViewExtension BindToSiteMap(string fileName) {
			return BindToSiteMap(fileName, true);
		}
		public TreeViewExtension BindToSiteMap(string fileName, bool showStartingNode) {
			BindToSiteMapInternal(fileName, showStartingNode);
			return this;
		}
		public TreeViewExtension BindToXML(string fileName) {
			return BindToXML(fileName, string.Empty, string.Empty);
		}
		public TreeViewExtension BindToXML(string fileName, string xPath) {
			return BindToXML(fileName, xPath, string.Empty);
		}
		public TreeViewExtension BindToXML(string fileName, string xPath, string transformFileName) {
			BindToXMLInternal(fileName, xPath, transformFileName);
			return this;
		}
		protected internal override void PrepareControl() {
			base.PrepareControl();
			Control.EnsureClientStateLoaded();
		}
		protected override Control GetCallbackResultControl() {
			return Control.GetCallbackResultControl();
		}
		protected override void RenderCallbackResult() {
			if(!Control.Bound && !Control.IsVirtualMode()) 
				AssignExpandedNodeTemplates(Control.NodeExpandedOnCallbackID);
			base.RenderCallbackResult();
		}
		bool IsExpandAllCommand() {
			return MvcUtils.CallbackArgument.Contains("EA");
		}
		public static TreeViewState GetState(string name) {
			return MVCxTreeView.GetState(name);
		}
		protected override DevExpress.Web.ASPxWebControl CreateControl() {
			return new MVCxTreeView();
		}
	}
}
