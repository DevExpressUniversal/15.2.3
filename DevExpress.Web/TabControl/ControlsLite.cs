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
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Internal {
	[ToolboxItem(false)]
	public class TCControlBase : ASPxInternalWebControl {
		private ASPxTabControlBase fTabControl = null;
		public ASPxTabControlBase TabControl {
			get { return fTabControl; }
		}
		public TCControlBase(ASPxTabControlBase tabControl) {
			fTabControl = tabControl;
		}
		protected bool IsRightToLeft {
			get { return (TabControl as ISkinOwner).IsRightToLeft(); }
		}
		protected bool IsLeftRightTabPosition() {
			return TabControl.TabPosition == TabPosition.Left || TabControl.TabPosition == TabPosition.Right;
		}
		protected bool IsTopBottomTabPosition() {
			return TabControl.TabPosition == TabPosition.Top || TabControl.TabPosition == TabPosition.Bottom;
		}
		protected bool IsScrollingEnabled() {
			return IsTopBottomTabPosition() && TabControl.EnableTabScrolling;
		}
		protected bool IsMultiRow() {
			if(IsScrollingEnabled() || !IsTopBottomTabPosition())
				return false;
			foreach(TabBase tab in TabControl.TabItems) {
				if(tab.NewLine)
					return true;
			}
			return false;
		}
		protected bool IsTabStartsNewLine(TabBase tab) {
			bool first = TabControl.TabItems.GetVisibleTabItem(0) == tab;
			if(tab.NewLine)
				return tab.NewLine && !first && !IsScrollingEnabled() && IsTopBottomTabPosition();
			int i = tab.Index - 1;
			while(i >= 0) {
				if(TabControl.TabItems[i].Visible)
					return false;
				if(TabControl.TabItems[i].NewLine && !TabControl.TabItems[i].Visible)
					return true;
				i--;
			}
			return false;
		}
		protected void CreateTabTemplateContainer(Control control, TabBase tab, bool isActive, ITemplate template, string containerID) {
			TabControlTemplateContainerBase container = TabControl.CreateTemplateContainer(tab, isActive);
			CreateTemplateContainer(control, template, container, containerID);
		}
		protected void CreateTemplateContainer(Control control, ITemplate template, TemplateContainerBase container, string containerID) {
			template.InstantiateIn(container);
			container.AddToHierarchy(control, containerID);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if(CanPrepareTable())
				RenderUtils.ApplyCellPaddingAndSpacing(GetTableElement());
		}
		protected virtual bool CanPrepareTable() {
			return false;
		}
		protected virtual WebControl GetTableElement() {
			return this;
		}
	}
	public class TabControlLite : TCControlBase {
		TabStripWrapperControlLite strip = null;
		protected WebControl MainCell { get; set; }
		protected WebControl MainRow { get; set; }
		public TabControlLite(ASPxTabControlBase tabControl)
			: base(tabControl) {
		}
		protected override bool HasRootTag() { return true; }
		protected override HtmlTextWriterTag TagKey {
			get {
				return HtmlTextWriterTag.Div;
			}
		}
		protected override void ClearControlFields() {
			this.strip = null;
		}
		public TabItemControlLite GetTabItemElement(int tabIndex) {
			return TabStripWrapper.TabStripControl.Controls[3 * tabIndex] as TabItemControlLite;
		}
		protected TabStripWrapperControlLite TabStripWrapper {
			get { return strip; }
			set { strip = value; }
		}
		protected override void CreateControlHierarchy() {
			TabStripWrapper = CreateTabStripWrapper();
			Controls.Add(TabStripWrapper);
		}
		protected void CreateMainRow() {
			MainRow = RenderUtils.CreateWebControl(HtmlTextWriterTag.Tr);
			Controls.Add(MainRow);
		}
		protected WebControl CreateMainCell() {
			WebControl mainCell = RenderUtils.CreateWebControl(HtmlTextWriterTag.Td);
			MainRow.Controls.Add(mainCell);
			return mainCell;
		}
		protected virtual TabStripWrapperControlLite CreateTabStripWrapper() {
			return new TabStripWrapperControlLite(TabControl);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			RenderUtils.AssignAttributes(TabControl, this);
			TabControl.GetControlStyle().AssignToControl(this, AttributesRange.All, true);
			RenderUtils.SetVisibility(this, TabControl.IsClientVisible(), true);
			string cssClass = TabControlStyles.SystemCssClassName;
			if(!DesignMode)
				cssClass += " " + TabControlStyles.InitializationCssClassName;
			cssClass += " " + TabControlStyles.GetTabPositionCssMarker(TabControl.GetEffectiveTabPosition());
			if(IsRightToLeft)
				cssClass += " " + TabControlStyles.RightToLeftCssMarker;
			if(TabControl.TabSpacing.Value == 0 && !TabControl.TabSpacing.IsEmpty)
				cssClass += " " + TabControlStyles.NoTabSpacingCssMarker;
			if(IsMultiRow())
				cssClass += " " + TabControlStyles.MultiRowModeCssMarker;
			RenderUtils.AppendDefaultDXClassName(this, cssClass);
		}
	}
	public class TabControlLiteDesignMode : TabControlLite {
		protected WebControl TabStripCell { get; private set; }
		public TabControlLiteDesignMode(ASPxTabControlBase tabControl)
			: base(tabControl) { }
		protected override HtmlTextWriterTag TagKey {
			get { return HtmlTextWriterTag.Table; }
		}
		protected override TabStripWrapperControlLite CreateTabStripWrapper() {
			return new TabStripWrapperControlLiteDesignMode(TabControl);
		}
		protected override void CreateControlHierarchy() {
			TabStripWrapper = CreateTabStripWrapper();
			WebControl tableRow = RenderUtils.CreateWebControl(HtmlTextWriterTag.Tr);
			Controls.Add(tableRow);
			TabStripCell = RenderUtils.CreateWebControl(HtmlTextWriterTag.Td);
			tableRow.Controls.Add(TabStripCell);
			TabStripCell.Controls.Add(TabStripWrapper);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			TabStripCell.CssClass = this.CssClass;	  
		}
		protected override bool CanPrepareTable() {
			return true;
		}
	}
	public class TabStripWrapperControlLite : TCControlBase {
		protected WebControl VisibleScrollArea { get; private set; }
		public TabStripWrapperControlLite(ASPxTabControlBase tabControl)
			: base(tabControl) {
		}
		protected override bool HasRootTag() { return true; }
		protected override HtmlTextWriterTag TagKey {
			get { return HtmlTextWriterTag.Ul; }
		}
		protected internal TabStripControlLite TabStripControl {
			get {
				if(!IsScrollingEnabled())
					return Controls[1] as TabStripControlLite;
				return VisibleScrollArea.Controls[0] as TabStripControlLite;
			}
		}
		protected override void ClearControlFields() {
			VisibleScrollArea = null;
		}
		protected override void CreateControlHierarchy() {
			AddChild(GetCreateTabStripIndent(TabControl));
			if(IsScrollingEnabled()) {
				if(TabControl.TabAlign != TabAlign.Left) {
					AddChild(new ScrollButtonControlLite(TabControl, IsRightToLeft ? ASPxTabControlBase.ScrollDirectionRight : ASPxTabControlBase.ScrollDirectionLeft));
					if(TabControl.TabAlign == TabAlign.Right) {
						AddChild(new ScrollButtonSpaceControlLite(TabControl));
						AddChild(new ScrollButtonControlLite(TabControl, IsRightToLeft ? ASPxTabControlBase.ScrollDirectionLeft : ASPxTabControlBase.ScrollDirectionRight));
					}
					AddChild(new ScrollButtonIndentControlLite(TabControl));
				}
				VisibleScrollArea = CreateVisibleScrollArea();
				AddChild(VisibleScrollArea);
				VisibleScrollArea.Controls.Add(CreateTabStripControl());
				if(TabControl.TabAlign != TabAlign.Right) {
					AddChild(new ScrollButtonIndentControlLite(TabControl));
					if(TabControl.TabAlign == TabAlign.Left) {
						AddChild(new ScrollButtonControlLite(TabControl, IsRightToLeft ? ASPxTabControlBase.ScrollDirectionRight : ASPxTabControlBase.ScrollDirectionLeft));
						AddChild(new ScrollButtonSpaceControlLite(TabControl));
					}
					AddChild(new ScrollButtonControlLite(TabControl, IsRightToLeft ? ASPxTabControlBase.ScrollDirectionLeft : ASPxTabControlBase.ScrollDirectionRight));
				}
			} else
				AddChild(CreateTabStripControl());
			AddChild(GetCreateTabStripIndent(TabControl));
		}
		protected virtual TabStripIndentControlLite GetCreateTabStripIndent(ASPxTabControlBase tabControl) {
			return new TabStripIndentControlLite(tabControl);
		} 
		protected virtual void AddChild(WebControl child) {
			Controls.Add(child);
		}
		protected virtual WebControl CreateVisibleScrollArea() {
			return RenderUtils.CreateWebControl(HtmlTextWriterTag.Li);
		}
		protected virtual TabStripControlLite CreateTabStripControl() {
			return new TabStripControlLite(TabControl);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if(IsScrollingEnabled()) {
				RenderUtils.AppendDefaultDXClassName(this, TabControlStyles.TabStripWrapperCssClassName);
				VisibleScrollArea.ID = TabControl.GetScrollVisibleAreaID();
			} else
				RenderUtils.AppendDefaultDXClassName(this, TabControlStyles.TabStripCssClassName);
			RenderUtils.AppendDefaultDXClassName(this, TabControlStyles.TabStripContainerCssMarker);
			RenderUtils.SetPaddings(this, TabControl.GetTabStripPaddings());
		}
	}
	public class TabStripWrapperControlLiteDesignMode : TabStripWrapperControlLite {
		protected override HtmlTextWriterTag TagKey {
			get { return HtmlTextWriterTag.Table; }
		}
		public TabStripWrapperControlLiteDesignMode(ASPxTabControlBase tabControl)
			: base(tabControl) {
		}
		protected override WebControl CreateVisibleScrollArea() {
			return RenderUtils.CreateWebControl(HtmlTextWriterTag.Td);
		}
		protected override TabStripControlLite CreateTabStripControl() {
			return new TabStripControlLiteDesignMode(TabControl);
		}
		protected override void AddChild(WebControl child) {
			if(IsTopBottomTabPosition()) {
				WebControl tableRow = Controls.Count == 0 ? null : Controls[0] as WebControl;
				if(tableRow == null) {
					tableRow = RenderUtils.CreateWebControl(HtmlTextWriterTag.Tr);
					Controls.Add(tableRow);
				}
				tableRow.Controls.Add(child);
			} else {
				WebControl tableRow = RenderUtils.CreateWebControl(HtmlTextWriterTag.Tr);
				Controls.Add(tableRow);
				tableRow.Controls.Add(child);
			}
		}
		protected override bool CanPrepareTable() {
			return true;
		}
	}
	public abstract class TabStripControlLiteBase : TCControlBase {
		List<WebControl> tabItemControls = null;
		public TabStripControlLiteBase(ASPxTabControlBase tabControl)
			: base(tabControl) {
		}
		protected List<WebControl> TabItemControls {
			get { return tabItemControls; }
		}
		protected override HtmlTextWriterTag TagKey {
			get { return HtmlTextWriterTag.Ul; }
		}
		protected void CreateTabItemControl(TabBase tab, bool isActive) {
			if(tabItemControls == null)
				tabItemControls = new List<WebControl>();
			WebControl tabItemControl = new TabItemControlLite(tab, isActive);
			tabItemControls.Add(tabItemControl);
			AddChild(tabItemControl);
		}
		protected virtual void AddChild(WebControl child) {
			Controls.Add(child);
		}
	}
	public class TabStripControlLite : TabStripControlLiteBase {
		public TabStripControlLite(ASPxTabControlBase tabControl)
			: base(tabControl) {
		}
		protected override bool HasRootTag() { return IsScrollingEnabled(); }
		protected WebControl ScrollFiller {
			get {
				if(IsScrollingEnabled())
					return Controls[Controls.Count - 1] as WebControl;
				return null;
			}
		}
		protected override void CreateControlHierarchy() {
			int count = TabControl.TabItems.GetVisibleTabItemCount();
			for(int i = 0; i < count; i++) {
				TabBase tab = TabControl.TabItems.GetVisibleTabItem(i);
				bool tabStartsNewLine = IsTabStartsNewLine(tab);
				if(tabStartsNewLine)
					AddChild(new LineBrakeControlLite());
				if(i > 0 && !tabStartsNewLine)
					AddChild(new TabItemSpacerControlLite(TabControl.TabItems.GetVisibleTabItem(i - 1)));
				CreateTabItemControl(tab, false);
				CreateTabItemControl(tab, true);
			}
			if(IsScrollingEnabled())
				CreateScrollFiller();
		}
		protected virtual void CreateScrollFiller() {
			AddChild(RenderUtils.CreateWebControl(HtmlTextWriterTag.Li));
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if(IsScrollingEnabled())
				PrepareScrollFiller();
			for(int i = 0; i < TabControl.TabItems.GetVisibleTabItemCount(); i++) {
				TabBase tab = TabControl.TabItems.GetVisibleTabItem(i);
				RenderUtils.SetVisibility(GetTabItemControl(i, false), !tab.IsActive && (tab.ClientVisible || DesignMode), TabControl.UseClientSideVisibility(tab) || !(tab.ClientVisible || DesignMode));
				RenderUtils.SetVisibility(GetTabItemControl(i, true), tab.IsActive && tab.ClientEnabled, TabControl.UseClientSideVisibility(tab));
			}
		}
		protected virtual void PrepareScrollFiller() {
			RenderUtils.AppendDefaultDXClassName(this, TabControlStyles.TabStripCssClassName);
			ScrollFiller.CssClass = TabControlStyles.TabItemSpacerCssClassName;
			ScrollFiller.Height = TabControl.TabStyle.Height;
			TabControl.GetTabSeparatorStyle(-1).AssignToControl(ScrollFiller);
		}
		protected WebControl GetTabItemControl(int index, bool isActive) {
			if(isActive)
				return TabItemControls[2 * index + 1];
			return TabItemControls[2 * index];
		}
	}
	public class TabStripControlLiteDesignMode : TabStripControlLite {
		protected WebControl Table { get; private set; }
		protected override HtmlTextWriterTag TagKey {
			get { return HtmlTextWriterTag.Td; }
		}
		public TabStripControlLiteDesignMode(ASPxTabControlBase tabControl)
			: base(tabControl) {
		}
		protected override bool HasRootTag() {
			return true;
		}
		protected override void AddChild(WebControl child) {
			if(Table == null) {
				Table = RenderUtils.CreateWebControl(HtmlTextWriterTag.Table);
				Controls.Add(Table);
			}
			if(IsTopBottomTabPosition()) {
				int rowCount = Table.Controls.Count;
				WebControl tableRow = rowCount == 0 ? null : Table.Controls[rowCount - 1] as WebControl;
				bool addNewRow = child is LineBrakeControlLite;
				if(tableRow == null || addNewRow) {
					tableRow = RenderUtils.CreateWebControl(HtmlTextWriterTag.Tr);
					Table.Controls.Add(tableRow);
					if(addNewRow) {
						WebControl tableCell = RenderUtils.CreateWebControl(HtmlTextWriterTag.Td);
						tableRow.Controls.Add(tableCell);
						tableRow = RenderUtils.CreateWebControl(HtmlTextWriterTag.Tr);
						Table.Controls.Add(tableRow);
					}
				}
				if(!addNewRow)
					tableRow.Controls.Add(child);
			}
			else {
				WebControl tableRow = RenderUtils.CreateWebControl(HtmlTextWriterTag.Tr);
				Table.Controls.Add(tableRow);
				tableRow.Controls.Add(child);
			}
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			switch(TabControl.TabPosition) {
				case TabPosition.Top:
					Style["vertical-align"] = "bottom";
					break;
				case TabPosition.Bottom:
					Style["vertical-align"] = "top";
					break;
			}
		}
		protected override void CreateScrollFiller() {
		}
		protected override void PrepareScrollFiller() {
		}
		protected override bool CanPrepareTable() {
			return true;
		}
		protected override WebControl GetTableElement() {
			return Table;
		}
	}
	public class TabStripIndentControlLite : TCControlBase {
		public TabStripIndentControlLite(ASPxTabControlBase tabControl)
			: base(tabControl) { }
		protected override bool HasRootTag() { return true; }
		protected override HtmlTextWriterTag TagKey {
			get { return DesignMode ? HtmlTextWriterTag.Td : HtmlTextWriterTag.Li; }
		}
		protected bool IsLeftIndent {
			get { return Parent.Controls[0] == this; }
		}
		protected ITemplate SpaceTemplate {
			get {
				if(IsLeftIndent)
					return TabControl.SpaceBeforeTabsTemplate;
				return TabControl.SpaceAfterTabsTemplate;
			}
		}
		protected bool HasTemplate {
			get {
				return SpaceTemplate != null && DoesTemplateWithScrollingEnabled() &&
					   (TabControl.TabAlign != TabAlign.Left && IsLeftIndent || TabControl.TabAlign != TabAlign.Right && !IsLeftIndent);
			}
		}
		protected virtual bool DoesTemplateWithScrollingEnabled() {
			return !TabControl.EnableTabScrolling && TabControl.TabAlign != TabAlign.Justify;
		}
		protected override void CreateControlHierarchy() {
			if(HasTemplate) {
				TabSpaceTemplatePosition templatePosition =
					IsLeftIndent ? TabSpaceTemplatePosition.After : TabSpaceTemplatePosition.Before;
				TabsSpaceTemplateContainer container = new TabsSpaceTemplateContainer(TabControl, templatePosition);
				CreateTemplateContainer(this, SpaceTemplate, container, TabControl.GetTabsSpaceTemplateID(templatePosition));
			}
			else if(!DesignMode)
				Controls.Add(RenderUtils.CreateLiteralControl("&nbsp;"));
		}
		protected override void PrepareControlHierarchy() {
			Height = TabControl.TabStyle.Height;
			RenderUtils.AppendDefaultDXClassName(this, IsLeftIndent ?
				TabControlStyles.TabStripLeftIndentCssClassName : TabControlStyles.TabStripRightIndentCssClassName);
			TabControl.GetTabSeparatorStyle(-1).AssignToControl(this);
			if(HasTemplate) {
				if(IsLeftIndent)
					TabControl.SpaceBeforeTabsTemplateStyle.AssignToControl(this, true);
				else
					TabControl.SpaceAfterTabsTemplateStyle.AssignToControl(this, true);
				RenderUtils.AppendDefaultDXClassName(this, TabControlStyles.TabStripIndentTemplateCssClassName);
			}
			if(IsTopBottomTabPosition() && !HasTemplate)
				Width = IsLeftIndent ? TabControl.Paddings.GetPaddingLeft() : TabControl.Paddings.GetPaddingRight();
			else
				Height = IsLeftIndent ? TabControl.Paddings.GetPaddingTop() : TabControl.Paddings.GetPaddingBottom();
		}
	}
	public class TabItemContentControl : TCControlBase {
		protected TabBase Tab { get; private set; }
		protected bool IsActive { get; private set; }
		protected HyperLink Link { get; private set; }
		protected Image Image { get; private set; }
		protected WebControl TextContainer { get; private set; }
		protected LiteralControl TextControl { get; private set; }
		protected bool HasTextTemplate {
			get { return TabControl.GetTabTextTemplate(Tab, IsActive) != null; }
		}
		public TabItemContentControl(TabBase tab, bool isActive)
			: base(tab.TabControl) {
			Tab = tab;
			IsActive = isActive;
		}
		protected override void CreateControlHierarchy() {
			Link = new HyperLink();
			Controls.Add(Link);
			if(HasImage()) {
				Image = RenderUtils.CreateImage();
				Link.Controls.Add(Image);
			}
			if(HasTextTemplate) {
				CreateTabTemplateContainer(Link, Tab, IsActive, TabControl.GetTabTextTemplate(Tab, IsActive),
					TabControl.GetTabTextTemplateContainerID(Tab, IsActive));
			}
			else {
				TextContainer = RenderUtils.CreateWebControl(HtmlTextWriterTag.Span);
				Link.Controls.Add(TextContainer);
				TextControl = RenderUtils.CreateLiteralControl();
				TextContainer.Controls.Add(TextControl);
			}
		}
		protected override void PrepareControlHierarchy() {
			if(Link != null)
				PrepareHyperLinkControl();
			if(TextControl != null)
				PrepareTextControl();
			if(Image != null)
				PrepareImageControl();
		}
		protected internal void PrepareHyperLinkControl() {
			string url = IsActive ? string.Empty : TabControl.GetTabNavigateUrl(Tab);
			string target = IsActive ? string.Empty : TabControl.GetTabTarget(Tab);
			RenderUtils.PrepareHyperLink(Link, string.Empty, url, target, Tab.ToolTip, TabControl.GetTabEnabled(Tab));
			RenderUtils.PrepareHyperLinkForAccessibility(Link, TabControl.GetTabEnabled(Tab), TabControl.IsAccessibilityCompliantRender(true), true);
			AppearanceStyleBase linkStyle = TabControl.GetTabLinkStyle(Tab, IsActive);
			linkStyle.AssignToHyperLink(Link);
			if (Link.ForeColor.IsEmpty)
				Link.ForeColor = TabControl.ForeColor;
			if(linkStyle.IsFontSizeRelative())
				linkStyle.AssignToControl(Link, AttributesRange.Font);
			RenderUtils.SetPaddings(Link, TabControl.GetTabContentPaddings(Tab, IsActive));
			RenderUtils.AppendDefaultDXClassName(Link, TabControlStyles.TabItemLinkCssClassName);
		}
		protected internal void PrepareImageControl() {
			Image.Style[IsRightToLeft ? HtmlTextWriterStyle.MarginLeft : HtmlTextWriterStyle.MarginRight] =
				TabControl.GetTabImageSpacing(Tab, IsActive).ToString();
			TabControl.GetTabImage(Tab, IsActive).AssignToControl(Image, DesignMode, !TabControl.GetTabEnabled(Tab));
			RenderUtils.AppendDefaultDXClassName(Image, TabControlStyles.TabItemImageCssClassName);
			RenderUtils.SetVerticalAlignClass(Image, TabControl.GetTabStyle(Tab, IsActive).VerticalAlign);
		}
		protected internal void PrepareTextControl() {
			TextControl.Text = RenderUtils.ProtectTextWhitespaces(TabControl.HtmlEncode(TabControl.GetTabText(Tab)));
			if(TextContainer != null) {
				RenderUtils.SetVerticalAlignClass(TextContainer, TabControl.GetTabStyle(Tab, IsActive).VerticalAlign);
				RenderUtils.SetWrap(TextContainer, TabControl.GetTabStyle(Tab, IsActive).Wrap);
			}
		}
		protected virtual bool HasImage() {
			return !TabControl.GetTabImage(Tab, IsActive).IsEmpty;
		}
	}
	public class TabItemControlLite : TCControlBase {
		TabBase tab;
		bool isActive;
		public TabItemControlLite(TabBase tab, bool isActive)
			: base(tab.TabControl) {
			this.tab = tab;
			this.isActive = isActive;
		}
		protected override bool HasRootTag() { return true; }
		protected override HtmlTextWriterTag TagKey {
			get { return DesignMode ? HtmlTextWriterTag.Td : HtmlTextWriterTag.Li; }
		}
		protected TabBase Tab {
			get { return this.tab; }
		}
		protected bool IsActive {
			get { return this.isActive; }
		}
		protected bool First {
			get {
				return TabControl.TabItems.GetVisibleTabItem(0) == Tab;
			}
		}
		protected bool HasTemplate {
			get { return TabControl.GetTabTemplate(Tab, IsActive) != null; }
		}
		protected bool StartsNewLine() {
			return IsTabStartsNewLine(Tab) || (IsMultiRow() && First);
		}
		protected override void CreateControlHierarchy() {
			if(DesignMode)
				ID = TabControl.GetTabElementID(Tab, IsActive);
			if(HasTemplate) {
				CreateTabTemplateContainer(this, Tab, IsActive, TabControl.GetTabTemplate(Tab, IsActive), 
					TabControl.GetTabTemplateContainerID(Tab, IsActive));
			}
			else
				Controls.Add(new TabItemContentControl(Tab, IsActive));
		}
		protected override void PrepareControlHierarchy() {
			TabStyle tabStyle = TabControl.GetTabStyle(Tab, IsActive);
			tabStyle.AssignToControl(this, AttributesRange.Common | AttributesRange.Cell);
			if(!TabControl.GetTabEnabled(Tab))
				tabStyle.AssignToControl(this, AttributesRange.Font);
			RenderUtils.ResetWrap(this);
			bool widthIsPrimary = IsTopBottomTabPosition();
			Unit height = TabControl.GetTabSize(Tab, IsActive, !widthIsPrimary);
			Unit width = TabControl.GetTabSize(Tab, IsActive, widthIsPrimary);
			if(TabControl.TabAlign == TabAlign.Justify || IsMultiRow()) {
				if(widthIsPrimary) {
					RenderUtils.SetStyleStringAttribute(this, "min-width", width.ToString());
					Height = height;
				}
				else {
					RenderUtils.SetStyleStringAttribute(this, "min-height", height.ToString());
					Width = width;
				}
			}
			else {
				Height = height;
				Width = width;
			}
			if(TabControl.HasTabCellOnClick(Tab, IsActive)) { 
				string eventProperty = (TabControl.GetActivateTabPageAction() == ActivateTabPageAction.Click) ? "onclick" : "onmouseover";
				RenderUtils.SetStringAttribute(this, eventProperty, TabControl.GetTabCellOnClick(Tab, IsActive));
			}
			ToolTip = Tab.ToolTip;
			if(!TabControl.GetTabEnabled(Tab))
				RenderUtils.AppendDefaultDXClassName(this, TabControl.Styles.GetDefaultDisabledStyle().CssClass);
			if(StartsNewLine())
				RenderUtils.AppendDefaultDXClassName(this, TabControlStyles.TabStartsNewLineCssMarker);
			if(First)
				RenderUtils.AppendDefaultDXClassName(this, TabControlStyles.FirstTabCssMarker);
		}
	}
	public class LineBrakeControlLite : ASPxInternalWebControl {
		public LineBrakeControlLite() { }
		protected override bool HasRootTag() { return true; }
		protected override HtmlTextWriterTag TagKey {
			get { return DesignMode ? HtmlTextWriterTag.Td : HtmlTextWriterTag.Li; }
		}
		protected override void PrepareControlHierarchy() {
			CssClass = TabControlStyles.LineBrakeCssClassName;
		}
	}
	public class TabItemSpacerControlLite : TCControlBase {
		protected TabBase Tab { get; private set; }
		public TabItemSpacerControlLite(ASPxTabControlBase tabControl)
			: base(tabControl) {
		}
		public TabItemSpacerControlLite(TabBase tab)
			: this(tab.TabControl) {
			Tab = tab;
		}
		protected override bool HasRootTag() { return true; }
		protected override HtmlTextWriterTag TagKey {
			get { return DesignMode ? HtmlTextWriterTag.Td : HtmlTextWriterTag.Li; }
		}
		protected bool Last {
			get { return TabControl.TabItems[TabControl.TabItems.Count - 1] == Tab; }
		}
		protected override void CreateControlHierarchy() {
			Visible = !Last;
			if(!DesignMode)
				Controls.Add(RenderUtils.CreateLiteralControl("&nbsp;"));
		}
		protected override void PrepareControlHierarchy() {
			Height = TabControl.TabStyle.Height;
			RenderUtils.AppendDefaultDXClassName(this, TabControlStyles.TabItemSpacerCssClassName);
			var style = TabControl.GetTabSeparatorStyle(Tab.GetVisibleIndex());
			style.AssignToControl(this);
			if(IsTopBottomTabPosition())
				Width = TabControl.GetTabSpacing(Tab.GetVisibleIndex());
			else
				Height = TabControl.GetTabSpacing(Tab.GetVisibleIndex());
		}
	}
	public class ScrollButtonControlLite : TCControlBase {
		string scrollDirection;
		public ScrollButtonControlLite(ASPxTabControlBase tabControl, string scrollDirection)
			: base(tabControl) {
			this.scrollDirection = scrollDirection;
		}
		protected override bool HasRootTag() { return true; }
		protected override HtmlTextWriterTag TagKey {
			get { return DesignMode ? HtmlTextWriterTag.Td : HtmlTextWriterTag.Li; }
		}
		protected DivButtonControl Button {
			get { return Controls[0] as DivButtonControl; }
		}
		protected override void CreateControlHierarchy() {
			Controls.Add(new DivButtonControl());
			Button.ButtonID = TabControl.GetScrollButtonID(scrollDirection);
			Button.ButtonImageID = TabControl.GetScrollButtonImageID(scrollDirection);
			Button.ButtonStyle = TabControl.GetScrollButtonStyle();
			Button.ButtonPaddings = TabControl.GetScrollButtonStyle().Paddings;
			if(scrollDirection == ASPxTabControlBase.ScrollDirectionRight)
				Button.ButtonImage = TabControl.GetScrollRightImage();
			else
				Button.ButtonImage = TabControl.GetScrollLeftImage();
		}
		protected override void PrepareControlHierarchy() {
			TabControl.GetScrollButtonCellStyle().AssignToControl(this);
			Height = TabControl.TabStyle.Height;
		}
	}
	public class ScrollButtonIndentControlLite : TCControlBase {
		public ScrollButtonIndentControlLite(ASPxTabControlBase tabControl)
			: base(tabControl) {
		}
		protected override bool HasRootTag() { return true; }
		protected override HtmlTextWriterTag TagKey {
			get { return DesignMode ? HtmlTextWriterTag.Td : HtmlTextWriterTag.Li; }
		}
		protected override void PrepareControlHierarchy() {
			TabControl.GetScrollButtonIndentStyle().AssignToControl(this);
			Height = TabControl.TabStyle.Height;
			Width = TabControl.ScrollButtonsIndent;
		}
	}
	public class ScrollButtonSpaceControlLite : TCControlBase {
		public ScrollButtonSpaceControlLite(ASPxTabControlBase tabControl)
			: base(tabControl) {
		}
		protected override bool HasRootTag() { return true; }
		protected override HtmlTextWriterTag TagKey {
			get { return DesignMode ? HtmlTextWriterTag.Td : HtmlTextWriterTag.Li; }
		}
		protected override void PrepareControlHierarchy() {
			TabControl.GetScrollButtonSeparatorStyle().AssignToControl(this);
			Height = TabControl.TabStyle.Height;
			Width = TabControl.ScrollButtonSpacing;
		}
	}
	public class PageControlLite : TabControlLite {
		protected WebControl ContentsContainer { get; set; }
		protected WebControl TabStripHolder { get; set; }
		public PageControlLite(ASPxPageControl pageControl)
			: base(pageControl) {
		}
		protected ASPxPageControl PageControl {
			get { return TabControl as ASPxPageControl; }
		}
		protected bool IsTabsBeforeContent {
			get {
				return PageControl.TabPosition == TabPosition.Top || PageControl.TabPosition == TabPosition.Left;
			}
		}
		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender(e);
			for(int i = 0; i < PageControl.TabPages.GetVisibleTabPageCount(); i++) {
				TabPage page = PageControl.TabPages.GetVisibleTabPage(i);
				ContentControlLite pageContent = GetContentElement(i);
				if(pageContent == null)
					continue;
				RenderUtils.SetVisibility(pageContent, page.IsActive, TabControl.UseClientSideVisibility(page));
				pageContent.ContentControl.Visible = !TabControl.IsLoadTabByCallbackInternal() || page.IsActive;
			}
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			ContentsContainer = null;
		}
		protected override void CreateControlHierarchy() {
			if(IsTabsBeforeContent) {
				CreateTabs();
				CreateContent();
			}
			else {
				CreateContent();
				CreateTabs();
			}
		}
		protected void CreateTabs() {
			if(PageControl.ShowTabs) {
				TabStripWrapper = CreateTabStripWrapper();
				WebControl parentControl = this;
				parentControl.Controls.Add(TabStripWrapper);
			}
		}
		protected void CreateContent() {
			ContentsContainer = CreateContentContainer();
			CreateContentInternal();
		}
		protected void CreateContentInternal() {
			for(int i = 0; i < PageControl.TabPages.GetVisibleTabPageCount(); i++)
				ContentsContainer.Controls.Add(PageControl.CreateContentControl(i));
		}
		protected WebControl CreateContentContainer() {
			WebControl control = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			WebControl parentControl = this;
			parentControl.Controls.Add(control);
			return control;
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if(!PageControl.ShowTabs)
				RenderUtils.AppendDefaultDXClassName(this, TabControlStyles.NoTabsCssMarker);
			if(TabStripHolder != null)
				RenderUtils.AppendDefaultDXClassName(TabStripHolder, TabControlStyles.TabStripHolderCssClassName);
			PrepareContentsContainer();
			for(int i = 0; i < PageControl.TabPages.GetVisibleTabPageCount(); i++) {
				TabPage page = PageControl.TabPages.GetVisibleTabPage(i);
				ContentControlLite pageContent = GetContentElement(i);
				RenderUtils.SetVisibility(pageContent, page.IsActive, TabControl.UseClientSideVisibility(page));
				pageContent.ContentControl.Visible = !TabControl.IsLoadTabByCallbackInternal() || page.IsActive;
			}
		}
		protected void PrepareContentsContainer() {
			PageControl.GetContentStyle().AssignToControl(ContentsContainer);
			PageControl.GetContentPaddings().AssignToControl(ContentsContainer);
		}
		protected ContentControlLite GetContentElement(int index) {
			return ContentsContainer.Controls[index] as ContentControlLite;
		}
	}
	public class PageControlLiteDesignMode : PageControlLite {
		protected WebControl TabStripCell { get; private set; }
		protected TabPosition TabPosition {
			get { return TabControl.TabPosition; }
		}
		protected override HtmlTextWriterTag TagKey {
			get { return HtmlTextWriterTag.Table; }
		}
		public PageControlLiteDesignMode(ASPxPageControl pageControl)
			: base(pageControl) {
		}
		protected override void CreateControlHierarchy() {
			TabStripCell = RenderUtils.CreateWebControl(HtmlTextWriterTag.Td);
			ContentsContainer = RenderUtils.CreateWebControl(HtmlTextWriterTag.Td);
			if(IsTopBottomTabPosition()) {
				WebControl tableRow = RenderUtils.CreateWebControl(HtmlTextWriterTag.Tr);
				Controls.Add(tableRow);
				tableRow.Controls.Add(TabPosition == TabPosition.Top ? TabStripCell : ContentsContainer);
				tableRow = RenderUtils.CreateWebControl(HtmlTextWriterTag.Tr);
				Controls.Add(tableRow);
				tableRow.Controls.Add(TabPosition == TabPosition.Top ? ContentsContainer : TabStripCell);
			} else {
				WebControl tableRow = RenderUtils.CreateWebControl(HtmlTextWriterTag.Tr);
				Controls.Add(tableRow);
				tableRow.Controls.Add(TabPosition == TabPosition.Left ? TabStripCell : ContentsContainer);
				tableRow.Controls.Add(TabPosition == TabPosition.Left ? ContentsContainer : TabStripCell);
			}
			if(PageControl.ShowTabs) {
				TabStripWrapper = CreateTabStripWrapper();
				TabStripCell.Controls.Add(TabStripWrapper);
			}
			CreateContentInternal();
		}
		protected override TabStripWrapperControlLite CreateTabStripWrapper() {
			return new TabStripWrapperControlLiteDesignMode(TabControl);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if(IsLeftRightTabPosition())
				TabStripCell.Style["vertical-align"] = "top";
			TabStripCell.CssClass = this.CssClass;	  
			(ContentsContainer.Parent as WebControl).CssClass = this.CssClass;
		}
		protected override bool CanPrepareTable() {
			return true;
		}
	}
	public class ContentControlLite : TCControlBase {
		TabPage tabPage;
		WebControl ContentHolder { get; set; }
		public ContentControlLite(TabPage tabPage)
			: base(tabPage.TabControl) {
			this.tabPage = tabPage;
		}
		public TabPage TabPage { get { return tabPage; } }
		protected override bool HasRootTag() { return true; }
		protected override HtmlTextWriterTag TagKey {
			get { return HtmlTextWriterTag.Div; }
		}
		protected ASPxPageControl PageControl {
			get { return TabControl as ASPxPageControl; }
		}
		protected internal ContentControl ContentControl {
			get {
				return ContentHolder.Controls[0] as ContentControl;
			}
		}
		protected override void CreateControlHierarchy() {
			if(DesignMode)
				ID = PageControl.GetContentDivID(this.tabPage);
			ContentControl content = TabPage.ContentControl;
			content.ID = PageControl.GetContentControlID(TabPage);
			ContentHolder = RenderUtils.CreateDiv();
			ContentHolder.Controls.Add(content);
			Controls.Add(ContentHolder);
		}
	}
}
