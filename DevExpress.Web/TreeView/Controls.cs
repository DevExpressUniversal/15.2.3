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
using System.Collections.Generic;
using System.Text;
using DevExpress.Web;
using System.Web.UI.WebControls;
using DevExpress.Web.Internal;
using System.Web.UI;
using DevExpress.Web.Internal.InternalCheckBox;
namespace DevExpress.Web.Internal {
	public abstract class TreeViewControlBase : ASPxInternalWebControl {
		ASPxTreeView treeView = null;
		public TreeViewControlBase(ASPxTreeView treeView) {
			this.treeView = treeView;
		}
		protected ASPxTreeView TreeView { get { return this.treeView; } }
	}
	public class TreeViewControl : TreeViewControlBase {
		const string
			ContentDivID = "CD",
			SampleExpandButtonID = "SEB",
			SampleCollapseButtonID = "SCB",
			SampleNodeLoadingPanelID = "SNLP",
			OperaRtlFixCssClassName = "OperaRtlFix";
		WebControl controlDiv = null;
		WebControl contentDiv = null;
		TreeViewNodesListControl rootNodesContainer = null;
		System.Web.UI.WebControls.Image sampleExpandButton = null;
		System.Web.UI.WebControls.Image sampleCollapseButton = null;
		System.Web.UI.WebControls.Image sampleNodeLoadingPanel = null;
		public TreeViewControl(ASPxTreeView treeView)
			: base(treeView) {
		}
		protected internal TreeViewNodesListControl RootNodesContainer {
			get { return this.rootNodesContainer; }
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			this.controlDiv = null;
			this.contentDiv = null;
			this.rootNodesContainer = null;
			this.sampleCollapseButton = null;
			this.sampleExpandButton = null;
			this.sampleNodeLoadingPanel = null;
		}
		protected override void CreateControlHierarchy() {
			this.controlDiv = RenderUtils.CreateDiv();
			Controls.Add(controlDiv);
			this.contentDiv = RenderUtils.CreateDiv();
			this.contentDiv.ID = ContentDivID;
			this.controlDiv.Controls.Add(this.contentDiv);
			if (!TreeView.IsVirtualMode() && TreeView.Nodes.GetVisibleItemCount() == 0)
				return;
			this.rootNodesContainer = new TreeViewNodesListControl(TreeView, TreeView.CallbackNodes ?? TreeView.Nodes, true);
			this.contentDiv.Controls.Add(this.rootNodesContainer);
			if (!DesignMode && TreeView.ShowExpandButtons) {
				CreateSampleButtons();
				if (TreeView.SettingsLoadingPanel.Mode == TreeViewLoadingPanelMode.ShowNearNode &&
					TreeView.IsCallBacksEnabled()) {
					CreateSampleNodeLoadingPanel();
				}
			}
			Controls.Add(RenderUtils.CreateClearElement());
		}
		protected void CreateSampleNodeLoadingPanel() {
			this.sampleNodeLoadingPanel = RenderUtils.CreateImage();
			this.sampleNodeLoadingPanel.ID = SampleNodeLoadingPanelID;
			this.controlDiv.Controls.Add(this.sampleNodeLoadingPanel);
		}
		protected void CreateSampleButtons() {
			this.sampleCollapseButton = RenderUtils.CreateImage();
			this.sampleCollapseButton.ID = SampleCollapseButtonID;
			this.controlDiv.Controls.Add(this.sampleCollapseButton);
			this.sampleExpandButton = RenderUtils.CreateImage();
			this.sampleExpandButton.ID = SampleExpandButtonID;
			this.controlDiv.Controls.Add(this.sampleExpandButton);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			RenderUtils.AssignAttributes(TreeView, this.controlDiv);
			RenderUtils.SetVisibility(this.controlDiv, TreeView.IsClientVisible(), true);
			AppearanceStyleBase controlStyle = TreeView.GetControlStyle();
			if (!TreeView.Enabled)
				TreeView.MergeWithDisabledStyle<AppearanceStyleBase>(controlStyle);
			controlStyle.AssignToControl(this.controlDiv);
			if (TreeView.IsRightToLeft() && Browser.IsOpera)
				RenderUtils.AppendDefaultDXClassName(this.controlDiv, OperaRtlFixCssClassName);
			PrepareSampleButton(this.sampleExpandButton, TreeView.Images.EffectiveExpandButtonImageName);
			PrepareSampleButton(this.sampleCollapseButton, TreeView.Images.EffectiveCollapseButtonImageName);
			PrepareSampleLoadingPanel(this.sampleNodeLoadingPanel, TreeViewImages.NodeLoadingPanelImageName);
		}
		protected void PrepareSampleButton(System.Web.UI.WebControls.Image button, string imageName) {
			if (button == null)
				return;
			PrepareSampleImageControl(button, imageName);
			TreeView.GetExpandButtonStyle().AssignToControl(button);
			RenderUtils.SetVisibility(button, false, true);
		}
		protected void PrepareSampleLoadingPanel(System.Web.UI.WebControls.Image panel, string imageName) {
			if(panel == null)
				return;
			PrepareSampleImageControl(panel, imageName);
			RenderUtils.SetVisibility(panel, false, false, true);
			RenderUtils.SetStyleStringAttribute(panel, "poision", "absolute");
		}
		protected void PrepareSampleImageControl(System.Web.UI.WebControls.Image image, string imageName) {
			if (image == null)
				return;
			ImageProperties imageProperties = TreeView.Images.GetImageProperties(Page, imageName);
			imageProperties.AssignToControl(image, DesignMode);
		}
	}
	public class TreeViewNodesListControl : TreeViewControlBase {
		TreeViewNodeCollection nodes = null;
		WebControl listControl = null;
		bool expanded = false;
		public TreeViewNodesListControl(ASPxTreeView treeView, TreeViewNodeCollection nodes,
			bool display)
			: base(treeView) {
			this.nodes = nodes;
			this.expanded = display;
		}
		protected TreeViewNodeCollection Nodes { get { return this.nodes; } }
		protected bool Display { get { return this.expanded; } }
		protected override void ClearControlFields() {
			this.listControl = null;
			base.ClearControlFields();
		}
		protected override void CreateControlHierarchy() {
			TreeViewNodeControl nodeControl = null;
			List<bool> requireRenderIDList = null;			
			this.listControl = RenderUtils.CreateList(ListType.Bulleted);
			List<TreeViewNode> visibleNodes = GetVisibleNodes(out requireRenderIDList);
			for (int i = 0; i < visibleNodes.Count; i++) {
				nodeControl = new TreeViewNodeControl(TreeView, visibleNodes[i],
					i == visibleNodes.Count - 1, requireRenderIDList[i]);
				this.listControl.Controls.Add(nodeControl);
			}
			Controls.Add(this.listControl);
		}
		protected List<TreeViewNode> GetVisibleNodes(out List<bool> requireRenderIDList) {
			List<TreeViewNode> visibleNodes = new List<TreeViewNode>();
			requireRenderIDList = new List<bool>();
			for(int i = 0 ; i< Nodes.Count; i++) {
				if (Nodes[i].Visible) {
					visibleNodes.Add(Nodes[i]);
					requireRenderIDList.Add(i > 0 && !Nodes[i - 1].Visible);
				}
			}
			return visibleNodes;
		}
		protected override void PrepareControlHierarchy() {
			if (Browser.IsOpera && TreeView.IsRightToLeft())
				RenderUtils.SetStyleStringAttribute(this.listControl, "overflow-x", "hidden");
			if (!Display)
				RenderUtils.SetVisibility(this.listControl, false, true);
		}
	}
	public class TreeViewNodeControl : TreeViewControlBase, IInternalCheckBoxOwner {
		const string
			ClearElementCssClassName = "dxtv-clr",
			GifFileExtension = ".gif",
			CssFileExtension = ".css";
		bool last = false;
		bool requireRenderID = false;
		TreeViewNode node = null;
		WebControl listItem = null;
		WebControl elbowSpan = null;
		WebControl textSpan = null;
		InternalCheckboxControl checkBox = null;
		WebControl contentControl = null;
		HyperLink hyperLink = null;
		System.Web.UI.WebControls.Image expandButton = null;
		System.Web.UI.WebControls.Image image = null;
		ItemImageProperties imageProperties = null;
		protected TreeViewNode Node { get { return this.node; } }
		protected bool Last { get { return this.last; } }
		protected bool RequireRenderID { get { return this.requireRenderID; } }
		protected bool IsSubnode { get { return Node.Depth != 0; } }
		protected bool IsExpandableVirtualNode { get { return TreeView.IsVirtualMode() && !Node.IsLeaf; } }
		protected bool HasNodeTemplate { get { return TreeView.GetNodeTemplate(Node) != null; } }
		protected bool HasNodeTextTemplate { get { return TreeView.GetNodeTextTemplate(Node) != null; } }
		protected bool EnabledOnServer { get { return TreeView.Enabled && Node.Enabled; } }
		protected bool IsNodeWithCurrentPath { get { return Node == TreeView.NodeWithCurrentPath; } }
		protected bool RenderContentControlAsLink {
			get {
				return TreeView.NodeLinkMode == ItemLinkMode.ContentBounds &&
					!HasNodeTemplate && !HasNodeTextTemplate && !IsNodeWithCurrentPath &&
					!(string.IsNullOrEmpty(Node.NavigateUrl) && !TreeView.IsAccessibilityCompliantRender());
			}
		}
		protected ItemImageProperties ImageProperties {
			get {
				if (this.imageProperties == null)
					this.imageProperties = TreeView.GetRenderingNodeImageProperties(Node);
				return this.imageProperties;
			}
		}
		public TreeViewNodeControl(ASPxTreeView treeView, TreeViewNode node, bool last, bool requireRenderID)
			: base(treeView) {
			this.node = node;
			this.last = last;
			this.requireRenderID = requireRenderID;
		}
		public bool ClientEnabled {
			get { return EnabledOnServer; }
		}
		public CheckState CheckState {
			get { return Node.CheckState; }
		}
		InternalCheckBoxImageProperties IInternalCheckBoxOwner.GetCurrentCheckableImage() {
			return TreeView.GetCheckBoxImage(CheckState);
		}
		AppearanceStyleBase IInternalCheckBoxOwner.InternalCheckBoxStyle {
			get { return TreeView.GetNodeCheckBoxStyle(Node); }
		}
		string IInternalCheckBoxOwner.GetCheckBoxInputID() {
			return string.Empty;
		}
		bool IInternalCheckBoxOwner.IsInputElementRequired {
			get { return true; }
		}
		Dictionary<string, string> IInternalCheckBoxOwner.AccessibilityCheckBoxAttributes {
			get { return null; }
		}
		protected override void ClearControlFields() {
			this.expandButton = null;
			this.image = null;
			this.hyperLink = null;
			this.contentControl = null;
			this.checkBox = null;
			this.textSpan = null;
			this.elbowSpan = null;
			this.listItem = null;
		}
		protected void SetDesignModeBackgroundImage(WebControl control, string imageName) {
			string imageUrl = ImagesBase.GetSpriteLevelImageResource(TreeView.Page, TreeView, ASPxWebControl.WebImagesResourcePath, imageName + ".gif");
			RenderUtils.SetStyleStringAttribute(control, "background-image",
				ResourceManager.ResolveClientUrl(control, imageUrl));
		}
		protected WebControl GetClearElement() {
			WebControl clearElement = RenderUtils.CreateWebControl(HtmlTextWriterTag.B);
			RenderUtils.AppendDefaultDXClassName(clearElement, ClearElementCssClassName);
			return clearElement;
		}
		protected override void CreateControlHierarchy() {
			this.listItem = RenderUtils.CreateListItem();
			CreateElbowSpan();
			CreateContentControl();
			if(DesignMode)
				this.listItem.Controls.Add(GetClearElement());
			CreateSubnodes();
			Controls.Add(this.listItem);
		}
		protected void CreateContentControl() {
			this.contentControl = RenderContentControlAsLink ?
				RenderUtils.CreateHyperLink(true, true) : RenderUtils.CreateDiv();
			if (HasNodeTemplate) {
				TreeView.CreateTemplate(TreeView.GetNodeTemplate(Node), new TreeViewNodeTemplateContainer(Node),
					TreeView.GetNodeTemplateContainerID(Node), this.contentControl);
			} else
				CreateContent();
			if (RequireRenderID)
				this.contentControl.ID = Node.GetID();
			this.listItem.Controls.Add(this.contentControl);
			if(DesignMode && this.contentControl != null)
				this.contentControl.Controls.Add(RenderUtils.CreateWebControl(HtmlTextWriterTag.Span));
		}
		protected void CreateContent() {
			CreateCheckBox();
			if (TreeView.NodeLinkMode == ItemLinkMode.TextAndImage &&
				!HasNodeTextTemplate && EnabledOnServer) {
				CreateHyperLink();
			}
			if (TreeView.NodeImagePosition == TreeViewNodeImagePosition.Left) {
				CreateImage();
			}
			CreateText();
			if (TreeView.NodeImagePosition == TreeViewNodeImagePosition.Right) {
				CreateImage();
			}
		}
		protected void CreateText() {
			if (HasNodeTextTemplate)
				CreateTextTemplate();
			else if (TreeView.NodeLinkMode == ItemLinkMode.TextOnly && EnabledOnServer)
				CreateHyperLink();
			else
				CreateTextSpan();
		}
		protected void CreateTextTemplate() {
			WebControl templateParent = this.contentControl;
			if (TreeView.NodeImagePosition == TreeViewNodeImagePosition.Right) {
				templateParent = RenderUtils.CreateDiv();
				this.contentControl.Controls.Add(templateParent);
				RenderUtils.AlignBlockLevelElement(templateParent, HorizontalAlign.Left);
			}
			TreeView.CreateTemplate(TreeView.GetNodeTextTemplate(Node),
				new TreeViewNodeTemplateContainer(Node), TreeView.GetNodeTextTemplateContainerID(Node),
				templateParent);
		}
		protected void CreateHyperLink() {
			this.hyperLink = RenderUtils.CreateHyperLink();
			this.contentControl.Controls.Add(this.hyperLink);
		}
		protected void CreateCheckBox() {
			if (!TreeView.AllowCheckNodes || !Node.AllowCheck)
				return;
			this.checkBox = new InternalCheckboxControl(this);
			this.contentControl.Controls.Add(this.checkBox);
		}
		protected void CreateElbowSpan() {
			this.elbowSpan = RenderUtils.CreateWebControl(HtmlTextWriterTag.Span);
			if((IsExpandableVirtualNode || Node.Nodes.GetVisibleItemCount() > 0) &&
				TreeView.ShowExpandButtons) {
				this.expandButton = RenderUtils.CreateImage();
				this.elbowSpan.Controls.Add(this.expandButton);
			}
			this.listItem.Controls.Add(this.elbowSpan);
		}
		protected void CreateImage() {
			if (ImageProperties.IsEmpty)
				return;
			this.image = RenderUtils.CreateImage();
			WebControl parent = TreeView.NodeLinkMode == ItemLinkMode.TextAndImage && EnabledOnServer
				&& !HasNodeTextTemplate ? this.hyperLink : this.contentControl;
			parent.Controls.Add(this.image);
		}
		protected void CreateTextSpan() {
			this.textSpan = RenderUtils.CreateWebControl(HtmlTextWriterTag.Span);
			string strNodeText = RenderUtils.ProtectTextWhitespaces(TreeView.HtmlEncode(Node.GetRenderingText()));
			LiteralControl nodeText = RenderUtils.CreateLiteralControl(strNodeText);
			this.textSpan.Controls.Add(nodeText);
			WebControl parent = TreeView.NodeLinkMode == ItemLinkMode.ContentBounds || !EnabledOnServer ?
				this.contentControl : this.hyperLink;
			parent.Controls.Add(this.textSpan);
		}
		protected void CreateSubnodes() {
			if (!IsExpandableVirtualNode && Node.Nodes.GetVisibleItemCount() == 0)
				return;
			bool loadableOnDemand = !Node.Expanded && (TreeView.IsCallBacksEnabled() ||
				TreeView.AutoPostBack || TreeView.IsVirtualMode());
			WebControl clearElement = GetClearElement();
			this.listItem.Controls.Add(clearElement);
			if (loadableOnDemand)
				return;
			TreeViewNodesListControl subnodesList = new TreeViewNodesListControl(TreeView,
				Node.Nodes, Node.Expanded);
			this.listItem.Controls.Add(subnodesList);
		}
		protected override void PrepareControlHierarchy() {
			PrepareListItem();
			PrepareElbowSpan();
			PrepareContentControl();
			PrepareCheckBox();
			PrepareHyperLink();
			PrepareImage();
			PrepareTextSpan();
		}
		protected void PrepareContentControl() {
			AppearanceStyle style = HasNodeTemplate ? TreeView.GetNodeTemplateStyle() :
				TreeView.GetNodeStyle(Node);
			if (!Node.Enabled && TreeView.Enabled)
				TreeView.MergeWithDisabledStyle<AppearanceStyle>(style);
			if (RenderContentControlAsLink) {
				RenderUtils.PrepareHyperLink(this.contentControl as HyperLink, string.Empty,
					Node.GetRenderingNavigateUrl(), Node.GetRenderingTarget(), Node.ToolTip, EnabledOnServer);
				RenderUtils.PrepareHyperLinkForAccessibility(this.contentControl as HyperLink,
					EnabledOnServer, TreeView.IsAccessibilityCompliantRender(true), EnabledOnServer);
			} else if(this.hyperLink == null)
				this.contentControl.ToolTip = Node.ToolTip; 
			style.AssignToControl(this.contentControl);
			if (!style.Paddings.IsEmpty)
				RenderUtils.SetPaddings(this.contentControl, style.Paddings);
		}
		protected void PrepareHyperLink() {
			if (this.hyperLink == null)
				return;
			string text = TreeView.NodeLinkMode == ItemLinkMode.TextOnly ?
				RenderUtils.ProtectTextWhitespaces(TreeView.HtmlEncode(Node.GetRenderingText())) : string.Empty;
			RenderUtils.PrepareHyperLink(this.hyperLink, text, IsNodeWithCurrentPath ?
				string.Empty : Node.GetRenderingNavigateUrl(), Node.GetRenderingTarget(), 
				Node.ToolTip, EnabledOnServer);
			RenderUtils.PrepareHyperLinkForAccessibility(this.hyperLink, EnabledOnServer,
				TreeView.IsAccessibilityCompliantRender(true), EnabledOnServer);
			if (TreeView.NodeLinkMode == ItemLinkMode.TextOnly) {
				AppearanceStyle textStyle = TreeView.GetNodeTextStyle(Node);
				textStyle.AssignToControl(this.hyperLink);
				if (!textStyle.Paddings.IsEmpty)
					RenderUtils.SetPaddings(this.hyperLink, textStyle.Paddings);
			}
			AppearanceStyle nodeLinkStyle = TreeView.GetNodeLinkStyle(Node);
			nodeLinkStyle.AssignToHyperLink(this.hyperLink, false);
			if (TreeView.NodeLinkMode == ItemLinkMode.TextAndImage)
				RenderUtils.AlignBlockLevelElement(this.hyperLink, HorizontalAlign.Left);
		}
		protected void PrepareCheckBox() {
			if (this.checkBox == null)
				return;
			if(DesignMode)
				RenderUtils.SetMargins(this.checkBox.MainElement, 6, 4, 0, 0);
			RenderUtils.SetVerticalAlignClass(this.checkBox.MainElement, TreeView.GetNodeStyle(Node).VerticalAlign);
		}
		protected void PrepareImage() {
			if (this.image == null)
				return;
			ImageProperties.AssignToControl(this.image, DesignMode);
			AppearanceStyle imageStyle = TreeView.GetNodeImageStyle(Node);
			imageStyle.AssignToControl(this.image);
			Paddings margins = new Paddings();
			if (!imageStyle.Paddings.IsEmpty)
				margins.CopyFrom(imageStyle.Paddings);
			Unit spacing = TreeView.GetNodeImageStyle(Node).ImageSpacing;
			if(!spacing.IsEmpty) {
				if(TreeView.NodeImagePosition == TreeViewNodeImagePosition.Left && TreeView.IsRightToLeft() ||
					TreeView.NodeImagePosition == TreeViewNodeImagePosition.Right && !TreeView.IsRightToLeft())
					margins.PaddingLeft = spacing;
				else
					margins.PaddingRight = spacing;
			}
			if(!margins.IsEmpty)
				RenderUtils.SetMargins(this.image, margins);
			RenderUtils.SetVerticalAlignClass(this.image, TreeView.GetNodeStyle(Node).VerticalAlign);
		}
		protected void PrepareTextSpan() {
			if (textSpan == null)
				return;
			AppearanceStyle textStyle = TreeView.GetNodeTextStyle(Node);
			textStyle.AssignToControl(this.textSpan);
			if (!textStyle.Paddings.IsEmpty)
				RenderUtils.SetPaddings(this.textSpan, textStyle.Paddings);
			RenderUtils.SetVerticalAlignClass(this.textSpan, TreeView.GetNodeStyle(Node).VerticalAlign);
		}
		protected void PrepareElbowSpan() {
			TreeView.GetElbowStyle().AssignToControl(this.elbowSpan);
			if (DesignMode && TreeView.ShowTreeLines)
				SetDesignModeBackgroundImage(this.elbowSpan, TreeView.Images.EffectiveElbowImageName);
			PrepareExpandButton();
		}
		protected void PrepareListItem() {
			if (!Last && TreeView.ShowTreeLines) {
				TreeView.GetLineStyle().AssignToControl(this.listItem);
				if (DesignMode)
					SetDesignModeBackgroundImage(this.listItem, TreeViewImages.LineImageName);
			}
			if (IsSubnode)
				TreeView.GetSubnodeStyle().AssignToControl(this.listItem);
		}
		protected void PrepareExpandButton() {
			if (this.expandButton == null)
				return;
			string imageName = Node.Expanded ? TreeView.Images.EffectiveCollapseButtonImageName :
				TreeView.Images.EffectiveExpandButtonImageName;
			ImageProperties imageProperties = TreeView.Images.GetImageProperties(Page, imageName);
			imageProperties.AssignToControl(this.expandButton, DesignMode);
			AppearanceStyle buttonStyle = TreeView.GetExpandButtonStyle();
			if (!Node.Enabled && TreeView.Enabled)
				TreeView.MergeWithDisabledStyle<AppearanceStyle>(buttonStyle);
			buttonStyle.AssignToControl(this.expandButton);
		}
	}
}
