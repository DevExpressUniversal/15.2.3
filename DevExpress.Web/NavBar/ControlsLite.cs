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
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Utils;
namespace DevExpress.Web.Internal {
	[ToolboxItem(false)]
	public class NavBarControlBase : ASPxInternalWebControl {
		private ASPxNavBar fNavBar = null;
		public ASPxNavBar NavBar {
			get { return fNavBar; }
		}
		public NavBarControlBase(ASPxNavBar navBar) {
			fNavBar = navBar;
		}
		protected bool IsRightToLeft {
			get { return (NavBar as ISkinOwner).IsRightToLeft(); }
		}
		protected ImagePosition GetEffectiveItemImagePosition(NavBarGroup group) {
			ImagePosition result = group.ItemImagePosition;
			if(IsRightToLeft) {
				if(result == ImagePosition.Left)
					return ImagePosition.Right;
				if(result == ImagePosition.Right)
					return ImagePosition.Left;
			}
			return result;
		}
	}
	public class NavBarControlLite : NavBarControlBase {
		public NavBarControlLite(ASPxNavBar navBar)
			: base(navBar) {
		}
		protected override bool HasRootTag() { return true; }
		protected override HtmlTextWriterTag TagKey { get { return HtmlTextWriterTag.Ul; } }
		protected override void CreateControlHierarchy() {
			for(int i = 0; i < NavBar.Groups.GetVisibleItemCount(); i++)
				Controls.Add(new NavBarGroupControlLite(NavBar.Groups.GetVisibleItem(i)));
			Parent.Controls.Add(RenderUtils.CreateClearElement());
		}
		protected override void PrepareControlHierarchy() {
			RenderUtils.AssignAttributes(NavBar, this);
			RenderUtils.SetVisibility(this, NavBar.IsClientVisible(), true);
			Height = Unit.Empty;
			NavBar.GetControlStyle().AssignToControl(this, true);
			RenderUtils.AppendDefaultDXClassName(this, NavBarStyles.ControlSystemClassName);
			if(!NavBar.ShowGroupHeaders)
				RenderUtils.AppendDefaultDXClassName(this, NavBarStyles.NoHeadsCssMarker);
			if(IsRightToLeft)
				Style["float"] = "right";
		}
	}
	public class NavBarGroupControlLite : NavBarControlBase {
		private NavBarGroup group;
		public NavBarGroupControlLite(NavBarGroup group)
			: base(group.NavBar) {
			this.group = group;
		}
		protected override bool HasRootTag() { return true; }
		protected override HtmlTextWriterTag TagKey { get { return HtmlTextWriterTag.Li; } }
		protected NavBarGroup Group { get { return group; } }
		protected bool Last { get { return NavBar.Groups[NavBar.Groups.Count - 1] == Group; } }
		protected NavBarGroupHeaderControlLite ExpandedHeader {
			get { return Controls[0] as NavBarGroupHeaderControlLite; }
		}
		protected NavBarGroupHeaderControlLite CollapsedHeader {
			get { return Controls[1] as NavBarGroupHeaderControlLite; }
		}
		protected NavBarGroupContentControlLite ContentControl {
			get { return Controls[Controls.Count - 1] as NavBarGroupContentControlLite; }
		}
		protected bool HasGroupContent {
			get { return Group.HasContent; }
		}
		protected bool Disabled { get { return !Group.Enabled && NavBar.Enabled; } }
		protected override void CreateControlHierarchy() {
			if(NavBar.ShowGroupHeaders) {
				Controls.Add(new NavBarGroupHeaderControlLite(Group, true));
				Controls.Add(new NavBarGroupHeaderControlLite(Group, false));
			}
			if(HasGroupContent)
				Controls.Add(new NavBarGroupContentControlLite(Group));
		}
		protected override void PrepareControlHierarchy() {
			RenderUtils.AppendDefaultDXClassName(this, NavBarStyles.GroupCssClassName);
			if(NavBar.GetItemLinkMode(Group) == ItemLinkMode.TextOnly)
				RenderUtils.AppendDefaultDXClassName(this, NavBarStyles.TextOnlyGroupCssClassName);
			else if(NavBar.GetItemLinkMode(Group) == ItemLinkMode.TextAndImage)
				RenderUtils.AppendDefaultDXClassName(this, NavBarStyles.TextAndImageGroupCssClassName);
			if(Last)
				RenderUtils.AppendDefaultDXClassName(this, NavBarStyles.LastCssMarker);
			else
				RenderUtils.SetStyleUnitAttribute(this, "margin-bottom", NavBar.GetGroupSpacing());
			if(Disabled)
				RenderUtils.AppendDefaultDXClassName(this, NavBar.Styles.GetDefaultDisabledStyle().CssClass);
			if(NavBar.ShowGroupHeaders) {
				RenderUtils.SetVisibility(ExpandedHeader, Group.Expanded, NavBar.UseClientSideVisibility(Group));
				RenderUtils.SetVisibility(CollapsedHeader, !Group.Expanded, NavBar.UseClientSideVisibility(Group));
			}
			if(HasGroupContent)
				RenderUtils.SetVisibility(ContentControl, Group.Expanded, NavBar.UseClientSideVisibility(Group));
		}
	}
	public class NavBarGroupHeaderControlLite : NavBarControlBase {
		const string TextControlClassName = "dxnb-ghtext";
		private NavBarGroup group;
		private bool expanded;
		private Image expandButtonControl = null;
		private Image imageControl = null;
		private LiteralControl textControl = null;
		private HyperLink hyperLinkControl = null;
		WebControl textSpan = null;
		public NavBarGroupHeaderControlLite(NavBarGroup group, bool expanded)
			: base(group.NavBar) {
			this.group = group;
			this.expanded = expanded;
		}
		protected override bool HasRootTag() { return true; }
		protected override HtmlTextWriterTag TagKey { get { return HtmlTextWriterTag.Div; } }
		protected NavBarGroup Group { get { return group; } }
		protected bool Expanded { get { return expanded; } }
		protected Image ImageControl { get { return imageControl; } set { imageControl = value; } }
		protected HyperLink HyperLinkControl { get { return hyperLinkControl; } set { hyperLinkControl = value; } }
		protected Image ExpandButtonControl { get { return expandButtonControl; } set { expandButtonControl = value; } }
		protected LiteralControl TextControl { get { return textControl; } set { textControl = value; } }
		protected WebControl TextSpan { get { return textSpan; } set { textSpan = value; } }
		protected bool HasHeaderTemplate { get { return NavBar.GetGroupHeaderTemplate(Group, Expanded) != null; } }
		protected bool IsNavigateUrl { get { return NavBar.IsGroupNavigateUrl(Group); } }
		protected bool IsImageLinklable { get { return IsNavigateUrl || NavBar.IsAccessibilityCompliantRender(true); } }
		protected Unit HeaderImageSpacing { get { return NavBar.GetGroupHeaderImageSpacing(Group, Expanded); } }
		protected bool ExpandButtonOnLeft { get { return NavBar.GetExpandButtonPosition(Group) == (IsRightToLeft ? ExpandButtonPosition.Right : ExpandButtonPosition.Left); } }
		protected bool HasImage { get { return !NavBar.GetGroupHeaderImageProperties(Group, Expanded).IsEmpty; } }
		protected bool HasText { get { return !string.IsNullOrEmpty(Group.Text); } }
		protected override void ClearControlFields() {
			base.ClearControlFields();
			ImageControl = null;
			HyperLinkControl = null;
			ExpandButtonControl = null;
			TextControl = null;
		}
		protected override void CreateControlHierarchy() {
			CreateExpandButtonImage();
			if(HasHeaderTemplate) {
				NavBarGroupTemplateContainer container = new NavBarGroupTemplateContainer(Group);
				ITemplate template = NavBar.GetGroupHeaderTemplate(Group, Expanded);
				template.InstantiateIn(container);
				container.AddToHierarchy(this, NavBar.GetGroupHeaderTemplateContainerID(Group, Expanded));
			} else {
				CreateHyperLink();
				CreateImage();
				CreateText();
			}
		}
		protected override void PrepareControlHierarchy() {
			NavBar.GetGroupHeaderStyle(Group, Expanded).AssignToControl(this, true);
			RenderUtils.ResetWrap(this);
			if(NavBar.HasGroupHeaderCellOnClick(Group, Expanded)) {
				if(NavBar.ExpandGroupAction == ExpandGroupAction.MouseOver)
					RenderUtils.SetStringAttribute(this, "onmousemove", NavBar.GetGroupHeaderCellOnMouseMove(Group));
				else
					RenderUtils.SetStringAttribute(this, "onclick", NavBar.GetGroupHeaderCellOnClick(Group));
			}
			if(Group.ExpandButtonPosition == ExpandButtonPosition.Left)
				RenderUtils.AppendDefaultDXClassName(this, NavBarStyles.HeaderWithLeftExpandButtonCssClassName);
			ToolTip = NavBar.GetGroupContentToolTip(Group);
			PrepareText();
			PrepareExpandButtonImage();
			PrepareImage();
			PrepareHyperLink();
			var height = NavBar.GetGroupHeaderHeight(Group, Expanded);
			if(!height.IsEmpty)
				Height = height;
		}
		protected void CreateHyperLink() {
			if(IsNavigateUrl || NavBar.IsAccessibilityCompliantRender(true)) {
				HyperLinkControl = RenderUtils.CreateHyperLink();
				Controls.Add(HyperLinkControl);
			}
		}
		protected void CreateExpandButtonImage() {
			if(!NavBar.IsVisibleExpandButton(Group)) return;
			ExpandButtonControl = RenderUtils.CreateImage();
			if(DesignMode)
				ExpandButtonControl.ID = NavBar.GetGroupHeaderExpandButtonID(Group, Expanded);
			Controls.Add(ExpandButtonControl);
		}
		protected void CreateImage() {
			if(!HasImage) return;
			ImageControl = RenderUtils.CreateImage();
			if(IsImageLinklable)
				HyperLinkControl.Controls.Add(ImageControl);
			else
				Controls.Add(ImageControl);
		}
		protected void CreateText() {
			if(!HasText) return;
			WebControl parent = IsNavigateUrl ? (WebControl)HyperLinkControl : this;
			TextSpan = new WebControl(HtmlTextWriterTag.Span);
			parent.Controls.Add(TextSpan);
			TextControl = new LiteralControl();
			TextSpan.Controls.Add(TextControl);
		}
		protected void PrepareExpandButtonImage() {
			if(ExpandButtonControl == null) return;
			NavBar.GetHeaderButtonImageProperties(Group, Expanded).AssignToControl(ExpandButtonControl, DesignMode);
			RenderUtils.AppendDefaultDXClassName(ExpandButtonControl, ExpandButtonOnLeft ?
				NavBarStyles.ExpandButtonLeftCssClassName : NavBarStyles.ExpandButtonCssClassName);
			RenderUtils.SetStyleUnitAttribute(ExpandButtonControl, ExpandButtonOnLeft ? "margin-right" : "margin-left", HeaderImageSpacing);
		}
		protected void PrepareImage() {
			if(ImageControl == null) return;
			NavBar.GetGroupHeaderImageProperties(Group, Expanded).AssignToControl(ImageControl, DesignMode);
			RenderUtils.AppendDefaultDXClassName(ImageControl, NavBarStyles.ImageCssClassName);
			RenderUtils.SetStyleUnitAttribute(ImageControl, IsRightToLeft ? "margin-left" : "margin-right", HeaderImageSpacing);
			RenderUtils.SetVerticalAlignClass(ImageControl, NavBar.GetGroupHeaderStyle(Group, Expanded).VerticalAlign);
		}
		protected void PrepareHyperLink() {
			if(HyperLinkControl == null) return;
			RenderUtils.PrepareHyperLink(HyperLinkControl, string.Empty, NavBar.GetGroupNavigateUrl(Group),
			NavBar.GetGroupTarget(Group), Group.ToolTip, NavBar.GetGroupEnabled(Group));
			RenderUtils.PrepareHyperLinkForAccessibility(HyperLinkControl, NavBar.GetGroupEnabled(Group),
				NavBar.IsAccessibilityCompliantRender(true), TextControl != null);
			NavBar.GetGroupHeaderLinkStyle(Group, Expanded).AssignToHyperLink(HyperLinkControl);
			RenderUtils.SetVerticalAlignClass(HyperLinkControl, NavBar.GetGroupHeaderStyle(Group, Expanded).VerticalAlign);
		}
		protected void PrepareText() {
			if(TextSpan != null) {
				RenderUtils.AppendDefaultDXClassName(TextSpan, TextControlClassName);
				if(HyperLinkControl == null)
					RenderUtils.SetVerticalAlignClass(TextSpan, NavBar.GetGroupHeaderStyle(Group, Expanded).VerticalAlign);
				RenderUtils.SetWrap(TextSpan, NavBar.GetGroupHeaderStyle(Group, Expanded).Wrap);
			}
			if(TextControl != null) 
				TextControl.Text = RenderUtils.ProtectTextWhitespaces(NavBar.HtmlEncode(NavBar.GetGroupText(Group)));
		}
	}
	public class NavBarGroupContentControlLite : NavBarControlBase {
		private NavBarGroup group;
		public NavBarGroupContentControlLite(NavBarGroup group)
			: base(group.NavBar) {
			this.group = group;
			if(NavBar.IsCallback)
				ID = NavBar.GetGroupContentCellID(Group);
		}
		protected override bool HasRootTag() { return true; }
		protected override HtmlTextWriterTag TagKey {
			get {
				if(RenderAsList)
					return HtmlTextWriterTag.Ul;
				return HtmlTextWriterTag.Div;
			}
		}
		protected bool IsContentVisible {
			get { return !NavBar.IsCallBacksEnabled() || Group.Expanded || NavBar.IsCallback; }
		}
		protected bool RenderAsList { get { return IsContentVisible && !HasContentTemplate; } }
		protected NavBarGroup Group { get { return group; } }
		protected bool InBulletMode { get { return NavBar.IsBulletMode(Group) && !NavBar.IsLargeItems(Group); } }
		protected bool HasContentTemplate { get { return NavBar.GetGroupContentTemplate(Group) != null; } }
		protected override void CreateControlHierarchy() {
			if(HasContentTemplate) {
				NavBarGroupTemplateContainer container = new NavBarGroupTemplateContainer(Group);
				ITemplate template = NavBar.GetGroupContentTemplate(Group);
				template.InstantiateIn(container);
				container.AddToHierarchy(this, NavBar.GetGroupContentTemplateContainerID(Group));
			} else {
				for(int i = 0; i < Group.Items.GetVisibleItemCount(); i++)
					Controls.Add(new NavBarGroupItemControlLite(Group.Items.GetVisibleItem(i)));
			}
		}
		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender(e);
			CheckInnerControlsVisibility();
		}
		protected override void PrepareControlHierarchy() {
			NavBar.GetGroupContentStyle(Group).AssignToControl(this, true);
			if(RenderAsList) {
				RenderUtils.AppendDefaultDXClassName(this, NavBarStyles.GetGroupImagePositionCssMarker(GetEffectiveItemImagePosition(Group)));
				if(InBulletMode) {
					Style.Add(HtmlTextWriterStyle.ListStyleType, ASPxNavBar.GetItemBulletStyleAttribute(Group.ItemBulletStyle));
					RenderUtils.SetStyleStringAttribute(this, "list-style-position", "inside");
				}
			}
			CheckInnerControlsVisibility();
		}
		protected void CheckInnerControlsVisibility() {
			var visible = IsContentVisible;
			foreach(Control child in Controls)
				child.Visible = visible;
		}
	}
	public class NavBarGroupItemControlLite : NavBarControlBase {
		private NavBarItem item;
		private Image imageControl;
		private LiteralControl textControl;
		private WebControl textSpan;
		private HyperLink hyperLinkControl;
		public NavBarGroupItemControlLite(NavBarItem item)
			: base(item.NavBar) {
			this.item = item;
		}
		protected override bool HasRootTag() { return true; }
		protected override HtmlTextWriterTag TagKey { get { return HtmlTextWriterTag.Li; } }
		protected NavBarItem Item { get { return item; } }
		protected Image ImageControl { get { return imageControl; } set { imageControl = value; } }
		protected LiteralControl TextControl { get { return textControl; } set { textControl = value; } }
		protected WebControl TextSpan { get { return textSpan; } set { textSpan = value; } }
		protected HyperLink HyperLinkControl { get { return hyperLinkControl; } set { hyperLinkControl = value; } }
		protected bool Last { get { return Item.Group.Items[Item.Group.Items.Count - 1] == Item; } }
		protected bool HasItemTemplate { get { return NavBar.GetItemTemplate(Item) != null; } }
		protected bool HasItemTextTemplate { get { return NavBar.GetItemTextTemplate(Item) != null; } }
		protected bool IsImageBeforeText { get { return Item.Group.ItemImagePosition == ImagePosition.Left || Item.Group.ItemImagePosition == ImagePosition.Top; } }
		protected bool IsNavigateUrl { get { return NavBar.IsItemNavigateUrl(Item); } }
		protected bool IsItemClickable { get { return NavBar.HasItemElementOnClick(Item) && !HasItemTemplate; } }
		protected bool IsImageLinkable { get { return IsNavigateUrl && NavBar.GetItemLinkMode(Item) != ItemLinkMode.TextOnly || NavBar.IsAccessibilityCompliantRender(true); } }
		protected bool IsLinkBlock { get { return NavBar.IsItemLinkBlock(Item); } }
		protected bool HasImage { get { return !NavBar.GetItemImageProperties(Item).IsEmpty; } }
		protected bool IsItemEnabled { get { return Item.Enabled && Item.Group.Enabled; } }
		protected override void ClearControlFields() {
			ImageControl = null;
			TextControl = null;
			TextSpan = null;
			HyperLinkControl = null;
		}
		protected override void CreateControlHierarchy() {
			if(HasItemTemplate) {
				NavBarItemTemplateContainer container = new NavBarItemTemplateContainer(Item);
				ITemplate template = NavBar.GetItemTemplate(Item);
				template.InstantiateIn(container);
				container.AddToHierarchy(this, NavBar.GetItemTemplateContainerID(Item));
			} 
			else {
				if(!IsImageLinkable && IsImageBeforeText)
					CreateImage();
				CreateHyperLink();
				if(IsImageLinkable && IsImageBeforeText) 
					CreateImage();
				if(HasItemTextTemplate) {
					NavBarItemTemplateContainer container = new NavBarItemTemplateContainer(Item);
					ITemplate template = NavBar.GetItemTextTemplate(Item);
					template.InstantiateIn(container);
					container.AddToHierarchy(HyperLinkControl as Control ?? this, NavBar.GetItemTextTemplateContainerID(Item));
				} 
				else 
					CreateText();
				if(!IsImageBeforeText) 
					CreateImage();
			}
		}
		protected override void PrepareControlHierarchy() {
			NavBarItemStyle itemStyle = NavBar.GetItemStyle(Item);
			if(!HasItemTemplate) {
				itemStyle.AssignToControl(this, AttributesRange.All ^ AttributesRange.Font);
				RenderUtils.ResetWrap(this);
				if(HyperLinkControl == null || !IsItemEnabled)
					itemStyle.AssignToControl(this, AttributesRange.Font);
				if(!IsLinkBlock || !IsItemEnabled) {
					RenderUtils.SetPaddings(this, NavBar.GetItemContentPaddings(Item));
					RenderUtils.AppendDefaultDXClassName(this, NavBarStyles.LinkCssClassName);
				}
			}
			else
				RenderUtils.AppendDefaultDXClassName(this, NavBarStyles.ItemTemplateCssClassName);
			if(!IsItemEnabled)
				RenderUtils.AppendDefaultDXClassName(this, NavBar.Styles.GetDefaultItemDisabledStyle().CssClass);
			if(Last)
				RenderUtils.AppendDefaultDXClassName(this, NavBarStyles.LastCssMarker);
			else
				RenderUtils.SetStyleUnitAttribute(this, "margin-bottom", NavBar.GetItemSpacing(Item.Group));
			ToolTip = NavBar.GetItemContentToolTip(Item);
			if(IsItemClickable)
				RenderUtils.SetStringAttribute(this, "onclick", NavBar.GetItemElementOnClick(Item));
			if(TextSpan == null)
				RenderUtils.SetWrap(this, itemStyle.Wrap); 
			PrepareText();
			PrepareImage();
			PrepareHyperLink();
			var height = NavBar.GetItemHeight(Item);
			if(!height.IsEmpty)
				Height = height;
		}
		protected void CreateHyperLink() {
			bool needHyperLink = NavBar.IsAccessibilityCompliantRender(true) ||
				IsNavigateUrl && (!HasItemTextTemplate || HasImage);
			if(needHyperLink) {
				HyperLinkControl = RenderUtils.CreateHyperLink();
				Controls.Add(HyperLinkControl);
			}
		}
		protected void CreateImage() {
			if(NavBar.GetItemImageProperties(Item).IsEmpty || NavBar.IsBulletMode(Item.Group)) return;
			ImageControl = RenderUtils.CreateImage();
			if(IsImageLinkable)
				HyperLinkControl.Controls.Add(ImageControl);
			else
				Controls.Add(ImageControl);
		}
		protected void CreateText() {
			if(string.IsNullOrEmpty(Item.Text)) return;
			WebControl parent = IsNavigateUrl ? (WebControl)HyperLinkControl : this;
			if(Item.Group.ItemImagePosition == ImagePosition.Left || Item.Group.ItemImagePosition == ImagePosition.Right) {
				TextSpan = new WebControl(HtmlTextWriterTag.Span);
				parent.Controls.Add(TextSpan);
				parent = TextSpan;
			}
			TextControl = new LiteralControl();
			parent.Controls.Add(TextControl);
		}
		protected void PrepareText() {
			if(TextSpan != null) {
				NavBarItemStyle itemStyle = NavBar.GetItemStyle(Item);
				RenderUtils.SetVerticalAlignClass(TextSpan, itemStyle.VerticalAlign);
				RenderUtils.SetWrap(TextSpan, itemStyle.Wrap);
				if(NavBar.HasItemTextElementOnClick(Item))
					TextSpan.Attributes["onclick"] = NavBar.GetItemElementOnClick(Item);
			}
			if(TextControl != null) 
				TextControl.Text = RenderUtils.ProtectTextWhitespaces(NavBar.HtmlEncode(NavBar.GetItemText(item)));
		}
		protected void PrepareImage() {
			if(ImageControl == null) return;
			NavBar.GetItemImageProperties(Item).AssignToControl(ImageControl, DesignMode, !NavBar.GetItemEnabled(Item));
			RenderUtils.MergeImageWithItemToolTip(ImageControl, NavBar.GetItemImageToolTip(Item));
			RenderUtils.SetStyleUnitAttribute(ImageControl, GetImageMarginAttribute(), NavBar.GetItemImageSpacing(Item.Group));
			if(Item.Group.ItemImagePosition == ImagePosition.Left || Item.Group.ItemImagePosition == ImagePosition.Right)
				RenderUtils.SetVerticalAlignClass(ImageControl, NavBar.GetItemStyle(Item).VerticalAlign);
			RenderUtils.AppendDefaultDXClassName(ImageControl, NavBarStyles.ImageCssClassName);
			if(NavBar.HasItemImageElementOnClick(Item))
				ImageControl.Attributes["onclick"] = NavBar.GetItemElementOnClick(Item);
		}
		string GetImageMarginAttribute() {
			switch(Item.Group.ItemImagePosition) {
				case ImagePosition.Left:
					return "margin-right";
				case ImagePosition.Top:
					return "margin-bottom";
				case ImagePosition.Bottom:
					return "margin-top";
				default:
					return "margin-left";
			}
		}
		protected void PrepareHyperLink() {
			if(HyperLinkControl == null) return;
			NavBar.GetItemStyle(Item).AssignToHyperLink(HyperLinkControl);
			RenderUtils.ResetWrap(HyperLinkControl);
			RenderUtils.SetCursor(HyperLinkControl, "");
			if(IsLinkBlock) {
				RenderUtils.SetPaddings(HyperLinkControl, NavBar.GetItemContentPaddings(Item));
				RenderUtils.AppendDefaultDXClassName(HyperLinkControl, NavBarStyles.LinkCssClassName);
			}
			RenderUtils.PrepareHyperLink(HyperLinkControl, "", NavBar.GetItemNavigateUrl(Item),
				NavBar.GetItemTarget(Item), Item.ToolTip, NavBar.GetItemEnabled(Item));
			RenderUtils.PrepareHyperLinkForAccessibility(HyperLinkControl, NavBar.GetItemEnabled(Item),
				NavBar.IsAccessibilityCompliantRender(true), TextControl != null);
			NavBar.GetItemLinkStyle(Item).AssignToHyperLink(HyperLinkControl, false);
			if(NavBar.HasItemTextLinkOnClick(Item))
				HyperLinkControl.Attributes["onclick"] = NavBar.GetItemElementOnClick(Item);
			if(Item.Group.ItemImagePosition == ImagePosition.Right)
				HyperLinkControl.Width = Unit.Percentage(100);
		}
	}
}
