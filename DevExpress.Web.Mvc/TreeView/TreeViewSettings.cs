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
using System.ComponentModel;
using DevExpress.Utils;
namespace DevExpress.Web.Mvc {
	public class TreeViewSettings: SettingsBase {
		MVCxTreeViewNodeCollection nodes;
		TreeViewSettingsLoadingPanel settingsLoadingPanel;
		public TreeViewSettings() {
			this.nodes = new MVCxTreeViewNodeCollection();
			this.settingsLoadingPanel = new TreeViewSettingsLoadingPanel(null);
			EnableAnimation = true;
			EnableHotTrack = true;
			NodeImagePosition = TreeViewNodeImagePosition.Left;
			NodeLinkMode = ItemLinkMode.ContentBounds;
			ShowExpandButtons = true;
			ShowTreeLines = true;
			SyncSelectionMode = SyncSelectionMode.CurrentPathAndQuery;
		}
		public bool AccessibilityCompliant { get { return AccessibilityCompliantInternal; } set { AccessibilityCompliantInternal = value; } }
		public bool AllowCheckNodes { get; set; }
		public bool AllowSelectNode { get; set; }
		public object CallbackRouteValues { get; set; }
		public TreeViewClientSideEvents ClientSideEvents { get { return (TreeViewClientSideEvents)ClientSideEventsInternal; } }
		public bool ClientVisible { get { return ClientVisibleInternal; } set { ClientVisibleInternal = value; } }
		public bool CheckNodesRecursive { get; set; }
		public new AppearanceStyleBase ControlStyle { get { return (AppearanceStyleBase)base.ControlStyle; } }
		public bool EnableAnimation { get; set; }
		public bool EnableClientSideAPI { get { return EnableClientSideAPIInternal; } set { EnableClientSideAPIInternal = value; } }
		public bool EnableHotTrack { get; set; }
		public TreeViewImages Images { get { return (TreeViewImages)ImagesInternal; } }
		public string ImageUrlField { get; set; }
		public string NameField { get; set; }
		public string NavigateUrlField { get; set; }
		public string NavigateUrlFormatString { get; set; }
		public TreeViewNodeImagePosition NodeImagePosition { get; set; }
		public MVCxTreeViewNodeCollection Nodes { get { return nodes; } }
		public ItemLinkMode NodeLinkMode { get; set; }
		public DefaultBoolean RightToLeft { get { return RightToLeftInternal; } set { RightToLeftInternal = value; } }
		public bool SaveStateToCookies { get; set; }
		public string SaveStateToCookiesID { get; set; }
		public TreeViewSettingsLoadingPanel SettingsLoadingPanel { get { return settingsLoadingPanel; } }
		public bool ShowExpandButtons { get; set; }
		public bool ShowTreeLines { get; set; }
		public TreeViewStyles Styles { get { return (TreeViewStyles)StylesInternal; } }
		public SyncSelectionMode SyncSelectionMode { get; set; }
		public string Target { get; set; }
		public string TextField { get; set; }
		public string TextFormatString { get; set; }
		public string ToolTipField { get; set; }
		public EventHandler BeforeGetCallbackResult { get { return BeforeGetCallbackResultInternal; } set { BeforeGetCallbackResultInternal = value; } }
		public ASPxClientLayoutHandler ClientLayout { get { return ClientLayoutInternal; } set { ClientLayoutInternal = value; } }
		public CustomJSPropertiesEventHandler CustomJSProperties { get; set; }
		public TreeViewNodeEventHandler NodeDataBound { get; set; }
		public EventHandler DataBinding { get; set; }
		public EventHandler DataBound { get; set; }
		[Obsolete("Use the BindToVirtualData method instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public TreeViewVirtualModeCreateChildrenEventHandler VirtualModeCreateChildren { get { return VirtualModeCreateChildrenInternal; } set { VirtualModeCreateChildrenInternal = value; } }
		internal TreeViewVirtualModeCreateChildrenEventHandler VirtualModeCreateChildrenInternal { get; set; }
		protected internal string NodeTemplateContent { get; set; }
		protected internal Action<TreeViewNodeTemplateContainer> NodeTemplateContentMethod { get; set; }
		protected internal string NodeTextTemplateContent { get; set; }
		protected internal Action<TreeViewNodeTemplateContainer> NodeTextTemplateContentMethod { get; set; }
		public void SetNodeTemplateContent(Action<TreeViewNodeTemplateContainer> contentMethod) {
			NodeTemplateContentMethod = contentMethod;
		}
		public void SetNodeTemplateContent(string content) {
			NodeTemplateContent = content;
		}
		public void SetNodeTextTemplateContent(Action<TreeViewNodeTemplateContainer> contentMethod) {
			NodeTextTemplateContentMethod = contentMethod;
		}
		public void SetNodeTextTemplateContent(string content) {
			NodeTextTemplateContent = content;
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new TreeViewClientSideEvents();
		}
		protected override AppearanceStyleBase CreateControlStyle() {
			return new AppearanceStyleBase();
		}
		protected override ImagesBase CreateImages() {
			return new TreeViewImages(null);
		}
		protected override StylesBase CreateStyles() {
			return new TreeViewStyles(null);
		}
	}
}
