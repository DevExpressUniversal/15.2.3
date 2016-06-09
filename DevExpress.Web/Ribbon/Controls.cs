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
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.Localization;
namespace DevExpress.Web.Internal {
	public class RibbonControl : ASPxInternalWebControl {
		internal const string TabControlID = "TC";
		const string TabsContainerID = "RTC";
		const string GroupCollapsePopupID = "GPC";
		const string TabMinimizePopupID = "MPC";
		const string TabChangedClientEventHandler = "ASPxClientRibbon.onTabChanged";
		const string TabClickClientEventHandler = "ASPxClientRibbon.onTabClick";
		const string InactiveTabName = "DXR_INACTIVE";
		public const string FileTabName = "DXR_FILE";
		public RibbonControl(ASPxRibbon ribbon) {
			Ribbon = ribbon;
		}
		public ASPxRibbon Ribbon { get; private set; }
		protected internal RibbonTabControl TabControl { get; set; }
		protected ASPxPopupControl GroupCollapsePopup { get; private set; }
		protected ASPxPopupControl TabMinimizePopup { get; private set; }
		protected WebControl TabsContainer { get; set; }
		protected List<Tab> ContextTabs = new List<Tab>();
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			CreateTabControl();
			CreateTabsContainer();
			CreateTabs();
			if(!DesignMode) {
				CreateGroupPopupControl();
				CreateMinimizePopupControl();
				CreateItemsExternalControls();
			}
		}
		protected void CreateItemsExternalControls() {
			foreach(var item in Ribbon.AllTabs.SelectMany(t => t.Groups).SelectMany(g => g.Items)) {
				var extControl = RibbonItemControlFactory.CreateItemExternalControl(Ribbon, item);
				if(extControl != null)
					Controls.Add(extControl);
			}
		}
		protected virtual void CreateTabControl() {
			TabControl = CreateTabControlCore();
			Controls.Add(TabControl);
		}
		protected RibbonTabControl CreateTabControlCore() {
			RibbonTabControl tc = new RibbonTabControl(Ribbon);
			tc.ID = TabControlID;
			tc.Width = Unit.Percentage(100);
			tc.ClientSideEvents.ActiveTabChanged = TabChangedClientEventHandler;
			tc.ClientSideEvents.TabClick = TabClickClientEventHandler;
			tc.Visible = Ribbon.ShowTabs;
			tc.EncodeHtml = Ribbon.EncodeHtml;
			tc.EnableTabScrolling = true;
			tc.TabAlign = TabAlign.Justify;
			if(Ribbon.AllowMinimize) {
				RibbonMinimizeButtonTemplate minimizeBtn = new RibbonMinimizeButtonTemplate(Ribbon);
				tc.SpaceAfterTabsTemplate = minimizeBtn;
			}
			return tc;
		}
		protected virtual void CreateTabsContainer() {
			TabsContainer = CreateTabsContainerCore();
			Controls.Add(TabsContainer);
		}
		protected WebControl CreateTabsContainerCore() {
			var tc = RenderUtils.CreateDiv();
			tc.ID = TabsContainerID;
			return tc;
		}
		void CreateTabs() {
			if(Ribbon.ShowFileTab)
				CreateFileTab();
			ContextTabs = new List<Tab>();
			foreach(RibbonTab tab in Ribbon.AllTabs)
				CreateTab(tab);
			if(!DesignMode)
				CreateInactiveTab();
			int tabIndex = Ribbon.ActiveTabIndex;
			var activeTab = tabIndex < 0 || tabIndex >= Ribbon.AllTabs.Count ? null : Ribbon.AllTabs[tabIndex];
			if(activeTab == null || (activeTab.IsContext && !activeTab.ContextTabCategory.ClientVisible))
				tabIndex = 0;
			if(Ribbon.AllTabs.Count(t => t.Visible == true) > 0)
				tabIndex = activeTab.Visible ? tabIndex : Ribbon.AllTabs.Find(t => t.Visible == true).Index;
			Ribbon.ActiveTabIndex = tabIndex;
			TabControl.ActiveTabIndex = Ribbon.ShowFileTab ? (Ribbon.ActiveTabIndex + 1) : Ribbon.ActiveTabIndex;
		}
		Tab CreateTab(RibbonTab tab) {
			var tcTab = new Tab();
			bool tabVisible = tab.GetVisible();
			tcTab.Text = tab.GetText();
			tcTab.Visible = tabVisible;
			tcTab.TabStyle.Assign(tab.TabStyle);
			tcTab.ActiveTabStyle.Assign(tab.ActiveTabStyle);
			if(tab.IsContext) {
				tcTab.TabTemplate = new RibbonContextTabTemplate(tab, tab.ContextTabCategory.Color);
				tcTab.ActiveTabTemplate = new RibbonContextTabTemplate(tab, tab.ContextTabCategory.Color);
				tcTab.ClientVisible = tab.ContextTabCategory.ClientVisible;
				tcTab.Visible = tabVisible ? tab.ContextTabCategory.Visible : false;
				ContextTabs.Add(tcTab);
			}
			TabControl.Tabs.Add(tcTab);
			if(tabVisible) {
				var tabContentControl = DesignMode ? new RibbonTabContentControlDesignTime(Ribbon, tab) : new RibbonTabContentControl(Ribbon, tab);
				tabContentControl.ID = RibbonHelper.GetRibbonTabID(tab);
				TabsContainer.Controls.Add(tabContentControl);
			}
			return tcTab;
		}
		void CreateFileTab() {
			var tab = new Tab();
			tab.Name = FileTabName;
			tab.Text = Ribbon.GetFileTabText();
			if(Ribbon.FileTabTemplate != null)
				tab.TabTemplate = Ribbon.FileTabTemplate;
			tab.TabStyle.CopyFrom(Ribbon.Styles.GetFileTabStyle());
			TabControl.Tabs.Add(tab);
		}
		void CreateInactiveTab() {
			Tab inactiveTab = new Tab();
			inactiveTab.TabStyle.CssClass = RibbonStyles.EmptyTabCssClass;
			inactiveTab.Text = " ";
			inactiveTab.Name = InactiveTabName;
			TabControl.Tabs.Add(inactiveTab);
		}
		void CreateGroupPopupControl() {
			GroupCollapsePopup = new ASPxPopupControl();
			GroupCollapsePopup.ID = GroupCollapsePopupID;
			GroupCollapsePopup.PopupAnimationType = AnimationType.Fade;
			GroupCollapsePopup.ShowCloseButton = false;
			GroupCollapsePopup.PopupVerticalAlign = PopupVerticalAlign.Below;
			GroupCollapsePopup.PopupHorizontalAlign = PopupHorizontalAlign.LeftSides;
			GroupCollapsePopup.ShowHeader = false;
			GroupCollapsePopup.ShowShadow = false;
			GroupCollapsePopup.CloseAction = CloseAction.None;
			Controls.Add(GroupCollapsePopup);
		}
		void CreateMinimizePopupControl() {
			TabMinimizePopup = new ASPxPopupControl();
			TabMinimizePopup.ID = TabMinimizePopupID;
			TabMinimizePopup.PopupAnimationType = AnimationType.Fade;
			TabMinimizePopup.ShowCloseButton = false;
			TabMinimizePopup.PopupVerticalAlign = PopupVerticalAlign.Below;
			TabMinimizePopup.PopupHorizontalAlign = PopupHorizontalAlign.LeftSides;
			TabMinimizePopup.PopupAlignCorrection = PopupAlignCorrection.Disabled;
			TabMinimizePopup.ShowHeader = false;
			TabMinimizePopup.ShowShadow = false;
			TabMinimizePopup.CloseAction = CloseAction.None;
			Controls.Add(TabMinimizePopup);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			RenderUtils.AssignAttributes(Ribbon, this);
			RenderUtils.SetVisibility(this, Ribbon.IsClientVisible(), true);
			Ribbon.GetControlStyle().AssignToControl(this, true);
			TabControl.Styles.Assign(Ribbon.StylesTabControl);
			foreach(Tab tab in ContextTabs) {
				tab.TabStyle.CssClass = RenderUtils.CombineCssClasses(tab.TabStyle.CssClass, RibbonStyles.ContextTabCssClass);
				tab.ActiveTabStyle.CssClass = RenderUtils.CombineCssClasses(tab.ActiveTabStyle.CssClass, RibbonStyles.ContextTabCssClass);
			}
			if(Ribbon.ShowFileTab)
				RenderUtils.AppendDefaultDXClassName(TabControl, RibbonStyles.TabControlHasFileTabCssClass);
			if(Ribbon.OneLineMode)
				RenderUtils.AppendDefaultDXClassName(this, RibbonStyles.OneLineModeCssClass);
			if(Ribbon.HasContextTabs())
				RenderUtils.AppendDefaultDXClassName(this, RibbonStyles.HasContextTabsCssClass);
			if(Ribbon.Width.IsEmpty)
				Width = Unit.Percentage(100);
			if(!DesignMode) {
				PrepareGroupPopupControl();
				PrepareMinimizePopupControl();
			}
			if(!Ribbon.ShowTabs)
				RenderUtils.AppendDefaultDXClassName(this, RibbonStyles.TabsHiddenCssClass);
			if(!Ribbon.ShowGroupLabels)
				RenderUtils.AppendDefaultDXClassName(this, RibbonStyles.GroupLabelsHiddenCssClass);
			RenderUtils.AppendDefaultDXClassName(this, RibbonStyles.SystemCssClassName);
		}
		void PrepareGroupPopupControl() {
			GroupCollapsePopup.ControlStyle.CopyFrom(Ribbon.Styles.GetGroupPopupStyle());
			GroupCollapsePopup.ContentStyle.CssClass = RenderUtils.CombineCssClasses(GroupCollapsePopup.ContentStyle.CssClass, RibbonStyles.GroupPopupWindowCssClass);
			GroupCollapsePopup.Width = 0;
			GroupCollapsePopup.Height = 0;
		}
		void PrepareMinimizePopupControl() {
			TabMinimizePopup.ControlStyle.CopyFrom(Ribbon.Styles.GetMinimizePopupStyle());
			TabMinimizePopup.ContentStyle.CssClass = RenderUtils.CombineCssClasses(TabMinimizePopup.ContentStyle.CssClass, RibbonStyles.MinimizePopupWindowCssClass);
			TabMinimizePopup.Width = 0;
			TabMinimizePopup.Height = 0;
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			TabControl = null;
		}
		protected override bool HasRootTag() { return true; }
		protected override System.Web.UI.HtmlTextWriterTag TagKey { get { return System.Web.UI.HtmlTextWriterTag.Div; } }
	}
	public class RibbonControlDesignTime : RibbonControl {
		public RibbonControlDesignTime(ASPxRibbon ribbon)
			:base(ribbon) { }
		protected override HtmlTextWriterTag TagKey { get { return HtmlTextWriterTag.Table; } }
		protected override void CreateTabControl() {
			TabControl = CreateTabControlCore();
			var tcRow = RenderUtils.CreateTableRow();
			tcRow.Height = Unit.Pixel(1);
			var tcCell = RenderUtils.CreateTableCell();
			tcCell.Controls.Add(TabControl);
			tcRow.Controls.Add(tcCell);
			Controls.Add(tcRow);
		}
		protected override void CreateTabsContainer() {
			TabsContainer = CreateTabsContainerCore();
			var tcRow = RenderUtils.CreateTableRow();
			tcRow.Height = Unit.Percentage(100);
			var tcCell = RenderUtils.CreateTableCell();
			Controls.Add(tcRow);
			tcRow.Controls.Add(tcCell);
			TabsContainer.Height = Unit.Percentage(100);
			tcCell.Controls.Add(TabsContainer);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			Attributes["cellpadding"] = "0";
			Attributes["cellspacing"] = "0";
		}
	}
	[ToolboxItem(false)]
	public class RibbonTabControl : ASPxTabControl {
		public RibbonTabControl(ASPxRibbon ribbon)
			:base(ribbon) {
			Ribbon = ribbon;
		}
		protected ASPxRibbon Ribbon { get; private set; }
		protected internal override AppearanceStyleBase GetTabSeparatorStyle(int index) {
			var style = base.GetTabSeparatorStyle(index);
			if(Ribbon.ShowFileTab && index == 0)
				style.CssClass = RenderUtils.CombineCssClasses(style.CssClass, RibbonStyles.TabControlFileTabSpacingCssClass);
			return style;
		}
		protected internal override Unit GetTabSpacing(int index) {
			if(Ribbon.ShowFileTab && index == 0 && !Ribbon.Styles.FileTab.Spacing.IsEmpty)
				return Ribbon.Styles.FileTab.Spacing;
			return base.GetTabSpacing(index);
		}
		protected override bool HasPressedScripts() {
			return base.HasPressedScripts() || Ribbon.ShowFileTab;
		}
		protected internal override bool UseClientSideVisibility(TabBase tab) {
			return !(Ribbon.ShowFileTab && tab.Index == 0) && base.UseClientSideVisibility(tab);
		}
		protected override TabControlLite CreateTabControl() {
			return DesignMode ? new RibbonTabControlLiteDesignMode(this) : ((TabControlLite)new RibbonTabControlLite(this));
		}
		protected override void AddPressedItems(StateScriptRenderHelper helper) { 
			base.AddPressedItems(helper);
			if(Ribbon.ShowFileTab) {
				var fileTab = Tabs.FindByName(RibbonControl.FileTabName);
				if(fileTab != null) {
					helper.AddStyles(GetTabStyles(GetTabPressedCssStyle(fileTab, false)), GetTabElementID(fileTab, false), TabIdPostfixes,
						new object[] { }, new string[] { }, GetTabEnabled(fileTab));
				}
			}
		}
		protected internal AppearanceStyle GetTabPressedCssStyle(TabBase tab, bool isActive) {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(GetTabPressedStyle(tab, isActive));
			style.Paddings.CopyFrom(GetTabSelectedPaddings(tab, isActive, style));
			return style;
		}
		protected internal AppearanceStyleBase GetTabPressedStyle(TabBase tab, bool isActive) {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyBordersFrom(GetTabStyleInternal(tab, isActive));
			style.CopyFrom(Ribbon.Styles.GetFileTabPressedStyle());
			return style;
		}
	}
	public class RibbonTabControlLite : TabControlLite {
		public RibbonTabControlLite(ASPxTabControlBase tabControl)
			: base(tabControl) {
		}
		protected override TabStripWrapperControlLite CreateTabStripWrapper() {
			return (TabStripWrapperControlLite)new RibbonTabStripWrapperControlLite(TabControl);
		}
	}
	public class RibbonTabControlLiteDesignMode : TabControlLiteDesignMode {
		public RibbonTabControlLiteDesignMode(ASPxTabControlBase tabControl)
			: base(tabControl) {
		}
		protected override TabStripWrapperControlLite CreateTabStripWrapper() {
			return (TabStripWrapperControlLiteDesignMode)new RibbonTabStripWrapperControlLiteDesignMode(TabControl);
		}
	}
	public class RibbonTabStripWrapperControlLite : TabStripWrapperControlLite {
		public RibbonTabStripWrapperControlLite(ASPxTabControlBase tabControl)
			: base(tabControl) {
		}
		protected override TabStripIndentControlLite GetCreateTabStripIndent(ASPxTabControlBase tabControl) {
			return (TabStripIndentControlLite)new RibbonTabStripIndentControlLite(tabControl);
		}
	}
	public class RibbonTabStripWrapperControlLiteDesignMode : TabStripWrapperControlLiteDesignMode {
		public RibbonTabStripWrapperControlLiteDesignMode(ASPxTabControlBase tabControl)
			: base(tabControl) {
		}
		protected override TabStripIndentControlLite GetCreateTabStripIndent(ASPxTabControlBase tabControl) {
			return (TabStripIndentControlLite)new RibbonTabStripIndentControlLite(tabControl);
		}
	}
	public class RibbonTabStripIndentControlLite : TabStripIndentControlLite {
		public RibbonTabStripIndentControlLite(ASPxTabControlBase tabControl)
			: base(tabControl) { }
		protected override bool DoesTemplateWithScrollingEnabled() {
			return true;
		}
	}
	public class RibbonMinimizeButtonTemplate : ASPxInternalWebControl, ITemplate {
		internal const string ButtonID = "MinBtn";
		internal const string ImageID = "I";
		public RibbonMinimizeButtonTemplate(ASPxRibbon ribbon) {
			Ribbon = ribbon;
		}
		public ASPxRibbon Ribbon { get; private set; }
		public Image Image { get; private set; }
		public WebControl Button { get; private set; }
		public void InstantiateIn(Control container) {
			Button = RenderUtils.CreateDiv();
			Button.ID = ButtonID;
			var style = Ribbon.Styles.GetMinimizeButtonStyle();
			Ribbon.MergeDisableStyle(style, Ribbon.Styles.GetMinimizeButtonDisabledStyle());
			style.AssignToControl(Button);
			container.Controls.Add(Button);
			Image = RenderUtils.CreateImage();
			Image.ID = ButtonID + ImageID;
			Ribbon.Images.GetMinimizeButtonImageProperties().AssignToControl(Image, Ribbon.DesignMode, !Ribbon.IsEnabled());
			Button.Controls.Add(Image);
		}
	}
	public class RibbonContextTabTemplate : ASPxInternalWebControl, ITemplate {
		protected RibbonTab Tab { get; private set; }
		protected System.Drawing.Color Color { get; private set; }
		protected HyperLink Link { get; private set; }
		protected WebControl ColorHeader { get; private set; }
		protected WebControl TextContainer { get; private set; }
		protected WebControl ColorBody { get; private set; }
		protected LiteralControl TextControl { get; private set; }
		public RibbonContextTabTemplate(RibbonTab tab, System.Drawing.Color color) {
			Tab = tab;
			Color = color;
		}
		public void InstantiateIn(Control container) {
			ColorHeader = RenderUtils.CreateDiv();
			ColorHeader.CssClass = RibbonStyles.ContextTabColorCssClass;
			ColorHeader.BackColor = Color;
			container.Controls.Add(ColorHeader);
			Link = new HyperLink();
			container.Controls.Add(Link);
			ColorBody = RenderUtils.CreateWebControl(HtmlTextWriterTag.Span);
			ColorBody.BackColor = Color;
			ColorBody.CssClass = RibbonStyles.ContextTabBodyColorCssClass;
			Link.Controls.Add(ColorBody);
			TextContainer = RenderUtils.CreateWebControl(HtmlTextWriterTag.Span);
			Link.Controls.Add(TextContainer);
			Link.CssClass = RibbonTabControlStyles.TabItemLinkCssClassName;
			RenderUtils.SetVerticalAlignClass(TextContainer, VerticalAlign.Middle);
			TextControl = RenderUtils.CreateLiteralControl(Tab.Text);
			TextContainer.Controls.Add(TextControl);
		}
		protected override void PrepareControlHierarchy() {
			Link.CssClass = RibbonTabControlStyles.TabItemLinkCssClassName;
			RenderUtils.SetVerticalAlignClass(TextContainer, VerticalAlign.Middle);
		}
	}
	public class RibbonTabContentControl : ASPxInternalWebControl {
		public RibbonTabContentControl(ASPxRibbon ribbon, RibbonTab tab) {
			RibbonTab = tab;
			Ribbon = ribbon;
		}
		protected RibbonTab RibbonTab { get; private set; }
		protected ASPxRibbon Ribbon { get; private set; }
		protected WebControl ListControl { get; set; }
		protected WebControl Container { get; private set; }
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			Container = RenderUtils.CreateDiv();
			Controls.Add(Container);
			CreateListControl();
			var groupsCount = RibbonTab.Groups.GetVisibleItemCount();
			for(int i = 0; i < groupsCount; i++)
				CreateRibbonGroupControl(RibbonTab.Groups.GetVisibleItem(i), true);
			Container.Controls.Add(RenderUtils.CreateClearElement());
		}
		protected virtual void CreateListControl() {
			ListControl = RenderUtils.CreateList(ListType.Bulleted);
			Container.Controls.Add(ListControl);
		}
		protected virtual void CreateRibbonGroupControl(RibbonGroup group, bool createSeparator) {
			ListControl.Controls.Add(new RibbonGroupControl(Ribbon, group));
			if(createSeparator)
				ListControl.Controls.Add(new RibbonGroupSeparatorControl(Ribbon, RibbonTab));
		}
		protected override void PrepareControlHierarchy() {
			Container.CssClass = RibbonStyles.TabWrapperCssClass;
			var style = Ribbon.Styles.GetTabContentStyle();
			style.AssignToControl(this);
			if(!style.Height.IsEmpty)
				Height = style.Height;
			ListControl.CssClass = RibbonStyles.GroupListCssClass;
		}
		protected override bool HasRootTag() { return true; }
		protected override System.Web.UI.HtmlTextWriterTag TagKey { get { return System.Web.UI.HtmlTextWriterTag.Div; } }
	}
	public class RibbonTabContentControlDesignTime : RibbonTabContentControl {
		public RibbonTabContentControlDesignTime(ASPxRibbon ribbon, RibbonTab tab)
			:base(ribbon, tab) { }
		protected TableRow ListTableRow { get; set; }
		protected override void CreateControlHierarchy() {
			if(Ribbon.AllowMinimize && Ribbon.Minimized)
				return;
			base.CreateControlHierarchy();
		}
		protected override void CreateListControl() {
			ListControl = RenderUtils.CreateTable();
			Container.Controls.Add(ListControl);
			ListTableRow = RenderUtils.CreateTableRow();
			ListControl.Controls.Add(ListTableRow);
		}
		protected override void CreateRibbonGroupControl(RibbonGroup group, bool createSeparator) {
			var groupCell = RenderUtils.CreateTableCell();
			groupCell.Controls.Add(new RibbonGroupControl(Ribbon, group));
			ListTableRow.Controls.Add(groupCell);
			if(createSeparator) {
				var groupSepCell = RenderUtils.CreateTableCell();
				groupSepCell.Controls.Add(new RibbonGroupSeparatorControl(Ribbon, RibbonTab));
				ListTableRow.Controls.Add(groupSepCell);
			}
		}
		protected override void PrepareControlHierarchy() {
			if(Ribbon.AllowMinimize && Ribbon.Minimized)
				return;
			base.PrepareControlHierarchy();
			ListTableRow.Height = Unit.Percentage(100);
			if(RibbonTab.GetVisibleIndex() == Ribbon.ActiveTabIndex && !RibbonTab.IsContext)
				Style.Add("display", "block");
			Container.Width = Unit.Percentage(100);
		}
	}
	public class RibbonGroupControl : ASPxInternalWebControl {
		internal const string DialogBoxLauncherImageID = "I";
		public RibbonGroupControl(ASPxRibbon ribbon, RibbonGroup group) {
			Group = group;
			Ribbon = ribbon;
		}
		protected RibbonGroup Group { get; private set; }
		protected ASPxRibbon Ribbon { get; private set; }
		protected WebControl LabelControl { get; private set; }
		protected WebControl DialogBoxLauncher { get; private set; }
		protected RibbonGroupContainerControl ContainerControl { get; private set; }
		protected RibbonGroupExpandButtonControl ExpandButton { get; private set; }
		protected Image DialogBoxLauncherImage { get; private set; }
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			ID = RibbonHelper.GetGroupID(Group);
			ContainerControl = DesignMode ? new RibbonGroupContainerControlDesignTime(Ribbon, Group) : new RibbonGroupContainerControl(Ribbon, Group);
			Controls.Add(ContainerControl);
			Controls.Add(RenderUtils.CreateClearElement());
			LabelControl = RenderUtils.CreateDiv();
			Controls.Add(LabelControl);
			if(Group.ShowDialogBoxLauncher) {
				DialogBoxLauncher = RenderUtils.CreateWebControl(System.Web.UI.HtmlTextWriterTag.Span);
				DialogBoxLauncher.ID = RibbonHelper.GetGroupDialogBoxLauncherID(Group);
				DialogBoxLauncherImage = RenderUtils.CreateImage();
				DialogBoxLauncherImage.ID = DialogBoxLauncher.ID + DialogBoxLauncherImageID;
				DialogBoxLauncher.Controls.Add(DialogBoxLauncherImage);
				LabelControl.Controls.Add(DialogBoxLauncher);
			}
			LabelControl.Controls.Add(RenderUtils.CreateLiteralControl(Ribbon.HtmlEncode(Group.GetText())));
			LabelControl.Visible = Ribbon.ShowGroupLabels;
			if(!DesignMode && !Ribbon.OneLineMode) {
				ExpandButton = new RibbonGroupExpandButtonControl(Ribbon, Group);
				Controls.Add(ExpandButton);
			}
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			Ribbon.Styles.GetGroupStyle().AssignToControl(this);
			ContainerControl.CssClass = RibbonStyles.GroupContentCssClass;
			var labelStyle = Ribbon.Styles.GetGroupLabelStyle();
			labelStyle.AssignToControl(LabelControl);
			if(!labelStyle.Height.IsEmpty)
				LabelControl.Height = labelStyle.Height;
			if(DialogBoxLauncher != null) {
				var dialogBoxLauncherStyle = Ribbon.Styles.GetGroupDialogBoxLauncherStyle();
				dialogBoxLauncherStyle.AssignToControl(DialogBoxLauncher);
				Ribbon.Images.GetDialogBoxLauncherImageProperties().AssignToControl(DialogBoxLauncherImage, Ribbon.DesignMode, !IsEnabled());
			}
		}
		protected override bool HasRootTag() { return true; }
		protected override System.Web.UI.HtmlTextWriterTag TagKey { get { return DesignMode ? System.Web.UI.HtmlTextWriterTag.Div : System.Web.UI.HtmlTextWriterTag.Li; } }
	}
	public class RibbonGroupSeparatorControl : ASPxInternalWebControl {
		public RibbonGroupSeparatorControl(ASPxRibbon ribbon, RibbonTab tab) {
			RibbonTab = tab;
			Ribbon = ribbon;
		}
		protected RibbonTab RibbonTab { get; private set; }
		protected ASPxRibbon Ribbon { get; private set; }
		protected override bool HasRootTag() { return true; }
		protected override System.Web.UI.HtmlTextWriterTag TagKey { get { return DesignMode ? System.Web.UI.HtmlTextWriterTag.Div : System.Web.UI.HtmlTextWriterTag.Li; } }
		protected WebControl InnerControl { get; private set; }
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			InnerControl = RenderUtils.CreateWebControl(System.Web.UI.HtmlTextWriterTag.B);
			Controls.Add(InnerControl);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			var style = Ribbon.Styles.GetGroupSeparatorStyle();
			style.AssignToControl(this);
			if(!style.Width.IsEmpty)
				InnerControl.Width = style.Width;
		}
	}
	public class RibbonGroupContainerControl : ASPxInternalWebControl {
		public RibbonGroupContainerControl(ASPxRibbon ribbon, RibbonGroup group) {
			Group = group;
			Ribbon = ribbon;
		}
		protected RibbonGroup Group { get; private set; }
		protected ASPxRibbon Ribbon { get; private set; }
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			List<RibbonItemsBlockControl> blocks = RibbonItemControlFactory.CreateItemBlocks(Ribbon, Group.Items, DesignMode);
			InstantiateItemBlocks(blocks);
			if(Ribbon.OneLineMode) {
				Controls.Add(new RibbonOneLineModeGroupExpandButtonControl(Ribbon, Group));
			}
			Controls.Add(RenderUtils.CreateClearElement());
		}
		protected virtual void InstantiateItemBlocks(List<RibbonItemsBlockControl> blocks) {
			foreach(var block in blocks) {
				Controls.Add(block);
			}
		}
		protected override bool HasRootTag() { return true; }
		protected override System.Web.UI.HtmlTextWriterTag TagKey { get { return System.Web.UI.HtmlTextWriterTag.Div; } }
	}
	public class RibbonGroupContainerControlDesignTime : RibbonGroupContainerControl {
		public RibbonGroupContainerControlDesignTime(ASPxRibbon ribbon, RibbonGroup group)
			:base(ribbon, group) { }
		protected override void InstantiateItemBlocks(List<RibbonItemsBlockControl> blocks) {
			var dtTable = RenderUtils.CreateTable();
			Controls.Add(dtTable);
			var dtRow = RenderUtils.CreateTableRow();
			dtTable.Controls.Add(dtRow);
			foreach(var block in blocks) {
				var dtCell = RenderUtils.CreateTableCell();
				dtRow.Controls.Add(dtCell);
				dtCell.Controls.Add(block);
				dtCell.VerticalAlign = VerticalAlign.Top;
			}
		}
	}
	public class RibbonGroupExpandButtonControl : ASPxInternalWebControl {
		public RibbonGroupExpandButtonControl(ASPxRibbon ribbon, RibbonGroup group) {
			Group = group;
			Ribbon = ribbon;
		}
		protected RibbonGroup Group { get; private set; }
		protected WebControl LabelContentControl { get; private set; }
		protected WebControl LabelTextControl { get; private set; }
		protected WebControl PopOut { get; private set; }
		protected ASPxRibbon Ribbon { get; private set; }
		protected Image Image { get; private set; }
		protected Image PopOutImage { get; private set; }
		protected override void CreateControlHierarchy() {
			ID = RibbonHelper.GetGroupExpandButtonID(Group);
			Image = RenderUtils.CreateImage();
			Image.ID = RibbonHelper.GetGroupExpandButtonID(Group) + "I";
			Controls.Add(Image);
			Controls.Add(RenderUtils.CreateBr());
			LabelContentControl = RenderUtils.CreateWebControl(System.Web.UI.HtmlTextWriterTag.Span);
			Controls.Add(LabelContentControl);
			var text = Ribbon.HtmlEncode(Group.GetText());
			LabelTextControl = RenderUtils.CreateWebControl(System.Web.UI.HtmlTextWriterTag.Span);
			LabelTextControl.Controls.Add(RenderUtils.CreateLiteralControl(text));
			LabelContentControl.Controls.Add(LabelTextControl);
			if(!string.IsNullOrEmpty(text) && !text.Contains(" "))
				LabelContentControl.Controls.Add(RenderUtils.CreateBr());
			PopOut = RenderUtils.CreateWebControl(System.Web.UI.HtmlTextWriterTag.Span);
			PopOutImage = RenderUtils.CreateImage();
			PopOut.Controls.Add(PopOutImage);
			LabelContentControl.Controls.Add(PopOut);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			var style = Ribbon.Styles.GetGroupExpandButtonStyle();
			style.AssignToControl(this);
			if(!style.Width.IsEmpty) {
				Width = style.Width;
				RenderUtils.AppendDefaultDXClassName(this, RibbonStyles.ItemHasWidth);
			}
			Group.GetImage().AssignToControl(Image, DesignMode);
			RenderUtils.AppendDefaultDXClassName(Image, RibbonStyles.Image32CssClass);
			LabelContentControl.CssClass = RibbonStyles.LabelContentCssClass;
			LabelTextControl.CssClass = RibbonStyles.LabelTextCssClass;
			PopOut.CssClass = RibbonStyles.DropDownButtonPopOutCssClass;
			Ribbon.Images.GetPopOutImageProperties().AssignToControl(PopOutImage, DesignMode);
			RenderUtils.SetVerticalAlignClass(PopOutImage, VerticalAlign.Middle);
		}
		protected override bool HasRootTag() { return true; }
		protected override System.Web.UI.HtmlTextWriterTag TagKey { get { return System.Web.UI.HtmlTextWriterTag.Div; } }
	}
	public class RibbonOneLineModeGroupExpandButtonControl : ASPxInternalWebControl {
		protected WebControl LabelControl { get; private set; }
		protected WebControl LabelTextControl { get; private set; }
		protected Image Image { get; private set; }
		protected WebControl PopOut { get; private set; }
		protected Image PopOutImage { get; private set; }
		protected ASPxRibbon Ribbon { get; private set; }
		protected RibbonGroup Group { get; private set; }
		public RibbonOneLineModeGroupExpandButtonControl(ASPxRibbon ribbon, RibbonGroup group) {
			Ribbon = ribbon;
			Group = group;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			ID = RibbonHelper.GetOneLineModeGroupExpandButtonID(Group);
			Image = RenderUtils.CreateImage();
			Image.ID = RibbonHelper.GetOneLineModeGroupExpandButtonImageID(Group);
			Controls.Add(Image);
			var text = Ribbon.HtmlEncode(Group.GetText());
			if(!string.IsNullOrEmpty(text) && Group.OneLineModeSettings.ShowExpandButtonText) {
				LabelControl = RenderUtils.CreateWebControl(System.Web.UI.HtmlTextWriterTag.Span);
				Controls.Add(LabelControl);
				LabelControl.Controls.Add(RenderUtils.CreateLiteralControl(text));
			}
			PopOut = RenderUtils.CreateWebControl(System.Web.UI.HtmlTextWriterTag.Span);
			Controls.Add(PopOut);
			PopOutImage = RenderUtils.CreateImage();
			PopOutImage.ID = RibbonHelper.GetOneLineModeGroupExpandButtonPopOutImageID(Group);
			PopOut.Controls.Add(PopOutImage);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			Group.OneLineModeSettings.Image.AssignToControl(Image, DesignMode, !IsEnabled());
			RenderUtils.AppendDefaultDXClassName(Image, RibbonStyles.Image16CssClass);
			RenderUtils.SetVerticalAlignClass(Image, VerticalAlign.Middle);
			Image.Visible = !Group.OneLineModeSettings.Image.IsEmpty;
			if(!string.IsNullOrEmpty(Group.GetText()) && Group.OneLineModeSettings.ShowExpandButtonText) {
				RenderUtils.SetVerticalAlignClass(LabelControl, VerticalAlign.Middle);
				RenderUtils.AppendDefaultDXClassName(LabelControl, RibbonStyles.LabelCssClass);
			}
			PopOut.CssClass = RibbonStyles.DropDownButtonPopOutCssClass;
			RenderUtils.SetVerticalAlignClass(PopOut, VerticalAlign.Middle);
			Ribbon.Images.GetPopOutImageProperties().AssignToControl(PopOutImage, DesignMode);
			var style = Ribbon.Styles.GetOneLineModeGroupExpandButtonStyle();
			style.AssignToControl(this);
		}
		protected override bool HasRootTag() { return true; }
		protected override System.Web.UI.HtmlTextWriterTag TagKey { get { return System.Web.UI.HtmlTextWriterTag.Span; } }
	}
	public class RibbonItemSeparatorControl : ASPxInternalWebControl {
		protected WebControl InnerControl { get; private set; }
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			InnerControl = RenderUtils.CreateWebControl(System.Web.UI.HtmlTextWriterTag.B);
			Controls.Add(InnerControl);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			CssClass = RibbonStyles.ItemSeparatorCssClass;
		}
		protected override bool HasRootTag() { return true; }
		protected override System.Web.UI.HtmlTextWriterTag TagKey { get { return System.Web.UI.HtmlTextWriterTag.Div; } }
	}
	public class RibbonItemsBlockControl : ASPxInternalWebControl {
		public RibbonItemsBlockControl(ASPxRibbon ribbon, RibbonBlockType blockType) {
			BlockType = blockType;
			BlockItems = new List<RibbonItemBase>();
			Ribbon = ribbon;
		}
		protected RibbonItemBase FirstItem { get; private set; }
		protected ASPxRibbon Ribbon { get; private set; }
		protected internal RibbonBlockType BlockType { get; private set; }
		protected internal List<RibbonItemBase> BlockItems { get; private set; }
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			foreach(var item in BlockItems) {
				if(item.BeginGroup && item.GetVisibleIndex() != 0)
					CreateItemSeparator();
				CreateItemControl(item);
			}
		}
		protected virtual void CreateItemSeparator() {
			Controls.Add(new RibbonItemSeparatorControl());
		}
		protected virtual void CreateItemControl(RibbonItemBase item) {
			Controls.Add(CreateItemControlCore(item));
			if(!item.OneLineMode && (BlockType == RibbonBlockType.HorizontalItems || (BlockType == RibbonBlockType.LargeItems && !DesignMode)))
				Controls.Add(RenderUtils.CreateClearElement());
		}
		protected RibbonItemControlBase CreateItemControlCore(RibbonItemBase item) {
			var itemControl = RibbonItemControlFactory.CreateItemControl(Ribbon, item);
			itemControl.ID = RibbonHelper.GetItemID(item.Group, item);
			return itemControl;
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			CssClass = RenderUtils.CombineCssClasses(RibbonStyles.BlockCssClass, GetBlockClass());
		}
		protected string GetBlockClass() {
			switch(BlockType) {
				case RibbonBlockType.HorizontalItems:
					return RibbonStyles.BlockHorizontalItemsCssClass;
				case RibbonBlockType.LargeItems:
					return RibbonStyles.BlockLargeItemsCssClass;
				case RibbonBlockType.SeparateItems:
					return RibbonStyles.BlockSeparateItemsCssClass;
				default:
					return RibbonStyles.BlockRegularItemsCssClass;
			}
		}
		protected override bool HasRootTag() { return true; }
		protected override System.Web.UI.HtmlTextWriterTag TagKey { get { return System.Web.UI.HtmlTextWriterTag.Div; } }
	}
	public class RibbonItemsBlockControlDesignTime : RibbonItemsBlockControl {
		public RibbonItemsBlockControlDesignTime(ASPxRibbon ribbon, RibbonBlockType blockType)
			:base(ribbon, blockType) { }
		protected TableRow BlockRow { get; set; }
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
		}
		protected override void CreateItemSeparator() {
			if(BlockType == RibbonBlockType.RegularItems)
				base.CreateItemSeparator();
		}
		protected override void CreateItemControl(RibbonItemBase item) {
			if(BlockType == RibbonBlockType.RegularItems && BlockRow == null) {
				BlockRow = RenderUtils.CreateTableRow();
				Controls.Add(BlockRow);
			} else if(BlockType == RibbonBlockType.LargeItems && BlockRow == null) {
				BlockRow = RenderUtils.CreateTableRow();
				Controls.Add(BlockRow);
			}
			else if(BlockType == RibbonBlockType.HorizontalItems) {
				BlockRow = RenderUtils.CreateTableRow();
				Controls.Add(BlockRow);
			}
			if(BlockType == RibbonBlockType.SeparateItems) {
				BlockRow = RenderUtils.CreateTableRow();
				Controls.Add(BlockRow);
			}
			var cell = RenderUtils.CreateTableCell();
			BlockRow.Controls.Add(cell);
			if(BlockType == RibbonBlockType.LargeItems)
				cell.Width = Unit.Pixel(50);
			cell.Controls.Add(CreateItemControlCore(item));
			cell.Controls.Add(RenderUtils.CreateClearElement());
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			Attributes.Add("cellspacing", "0");
			Attributes.Add("cellpadding", "0");
		}
		protected override HtmlTextWriterTag TagKey {
			get { return BlockType == RibbonBlockType.RegularItems ? HtmlTextWriterTag.Span : HtmlTextWriterTag.Table; }
		}
	}
	public abstract class RibbonItemControlBase : ASPxInternalWebControl {
		public RibbonItemControlBase(ASPxRibbon ribbon, RibbonItemBase item) {
			Item = item;
			Ribbon = ribbon;
		}
		protected RibbonItemBase Item { get; private set; }
		protected ASPxRibbon Ribbon { get; private set; }
		protected WebControl LabelControl { get; private set; }
		protected WebControl LabelContentControl { get; private set; }
		protected WebControl LabelTextControl { get; private set; }
		protected override void CreateControlHierarchy() {
			LabelControl = RenderUtils.CreateWebControl(System.Web.UI.HtmlTextWriterTag.Span);
			LabelContentControl = RenderUtils.CreateWebControl(HtmlTextWriterTag.Span);
			LabelControl.Controls.Add(LabelContentControl);
			var text = Ribbon.HtmlEncode(Item.GetText());
			if(IsLabelVisible() && !string.IsNullOrEmpty(text)) {
				LabelTextControl = RenderUtils.CreateWebControl(GetLabelTextControlTag());
				LabelContentControl.Controls.Add(LabelTextControl);
				text = DesignMode && text.Length > 10 ? (text.Substring(0, 7) + "...") : text;
				LabelTextControl.Controls.Add(RenderUtils.CreateLiteralControl(text));
			}
			if(!PlaceLabelAfterControl())
				Controls.Add(LabelControl);
			CreateInnerControl();
			if(PlaceLabelAfterControl()) {
				if(SeparateLabelByBr())
					Controls.Add(RenderUtils.CreateBr());
				Controls.Add(LabelControl);
			}
			LabelControl.Visible = IsLabelVisible();
		}
		protected virtual bool SeparateLabelByBr() {
			return Item.GetSize() == RibbonItemSize.Large && !Item.OneLineMode;
		}
		protected virtual void CreateInnerControl() { }
		protected override void ClearControlFields() {
			LabelControl = null;
		}
		protected virtual HtmlTextWriterTag GetLabelTextControlTag() {
			return HtmlTextWriterTag.Span;
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			var style = Ribbon.Styles.GetItemStyle(Item);
			Ribbon.MergeDisableStyle(style, Ribbon.Styles.GetItemDisabledStyle());
			style.AssignToControl(this, AttributesRange.Cell | AttributesRange.Common | AttributesRange.Margins | AttributesRange.Paddings);
			if(!style.Width.IsEmpty) {
				Width = style.Width;
				RenderUtils.AppendDefaultDXClassName(this, RibbonStyles.ItemHasWidth);
			}
			if(!string.IsNullOrEmpty(Item.GetToolTip()))
				ToolTip = Item.GetToolTip();
			if(IsLabelVisible()) {
				RenderUtils.AppendDefaultDXClassName(LabelControl, RibbonStyles.LabelCssClass);
				if(Item.GetSize() == RibbonItemSize.Small)
					RenderUtils.SetVerticalAlignClass(LabelControl, VerticalAlign.Middle);
				if(LabelTextControl != null) {
					style.AssignToControl(this, AttributesRange.Font);
					LabelTextControl.CssClass = RibbonStyles.LabelTextCssClass;
				}
			}
			LabelContentControl.CssClass = RibbonStyles.LabelContentCssClass;
		}
		protected override bool HasRootTag() { return true; }
		protected override System.Web.UI.HtmlTextWriterTag TagKey { get { return System.Web.UI.HtmlTextWriterTag.A; } }
		protected virtual bool IsLabelVisible() {
			return !string.IsNullOrEmpty(Item.GetText());
		}
		protected virtual bool PlaceLabelAfterControl() {
			return true;
		}
	}
	public class RibbonButtonControl : RibbonItemControlBase {
		public RibbonButtonControl(ASPxRibbon ribbon, RibbonButtonItem item) : this(ribbon, item, item.GetSmallImage(), item.GetLargeImage(), 
			item.GetNavigateUrl(), item.GetTarget()) { }
		public RibbonButtonControl(ASPxRibbon ribbon, RibbonItemBase item, ItemImagePropertiesBase smallImage, ItemImagePropertiesBase largeImage, 
			string navigateUrl, string target)
			: base(ribbon, item) {
				SmallImage = smallImage;
				LargeImage = largeImage;
				NavigateUrl = navigateUrl;
				Target = target;
		}
		protected ItemImagePropertiesBase SmallImage { get; private set; }
		protected ItemImagePropertiesBase LargeImage { get; private set; }
		protected string NavigateUrl { get; private set; }
		protected string Target { get; private set; }
		protected Image Image32by32 { get; private set; }
		protected Image Image16by16 { get; private set; }
		protected override void CreateInnerControl() {
			if(Item.GetSize() == RibbonItemSize.Large && !Item.OneLineMode) {
				Image32by32 = RenderUtils.CreateImage();
				Image32by32.ID = RibbonHelper.GetItemImageID(Item, RibbonItemSize.Large);
				AddImage(Image32by32);
			}
			Image16by16 = RenderUtils.CreateImage();
			Image16by16.ID = RibbonHelper.GetItemImageID(Item, RibbonItemSize.Small);
			AddImage(Image16by16);
		}
		protected virtual void AddImage(Image image) {
			Controls.Add(image);
		}
		protected override bool SeparateLabelByBr() {
			return base.SeparateLabelByBr() && !LargeImage.IsEmpty && !Item.OneLineMode;
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			var img32Properties = LargeImage;
			if(Item.GetSize() == RibbonItemSize.Large && !Item.OneLineMode) {
				img32Properties.AssignToControl(Image32by32, DesignMode, !IsEnabled());
				RenderUtils.AppendDefaultDXClassName(Image32by32, RibbonStyles.Image32CssClass);
				Image32by32.Visible = !img32Properties.IsEmpty;
			}
			SmallImage.AssignToControl(Image16by16, DesignMode, !IsEnabled());
			RenderUtils.AppendDefaultDXClassName(Image16by16, RibbonStyles.Image16CssClass);
			RenderUtils.SetVerticalAlignClass(Image16by16, VerticalAlign.Middle);
			Image16by16.Visible = !SmallImage.IsEmpty;
			if(IsLinkAttributesRequired())
				AddLinkAttributes();
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			Image32by32 = null;
			Image16by16 = null;
		}
		protected virtual void AddLinkAttributes() {
			var attributes = GetLinkAttributesOwner().Attributes;
			attributes.Add("href", ResolveUrl(NavigateUrl));
			var targetAttribute = Target;
			if(!string.IsNullOrEmpty(targetAttribute))
				attributes.Add("target", targetAttribute);
		}
		protected virtual bool IsLinkAttributesRequired() {
			return !string.IsNullOrEmpty(NavigateUrl);
		}
		protected virtual WebControl GetLinkAttributesOwner() {
			return this;
		}
		protected override bool HasRootTag() { return true; }
		protected override System.Web.UI.HtmlTextWriterTag TagKey { get { return System.Web.UI.HtmlTextWriterTag.A; } }
	}
	public abstract class RibbonEditItemControlBase : RibbonItemControlBase {
		public RibbonEditItemControlBase(ASPxRibbon ribbon, RibbonEditItemBase item)
			:base(ribbon, item) { }
		protected abstract ASPxEditBase CreateEditor();
		protected abstract PropertiesBase GetEditorProperties();
		protected abstract string GetEditorID();
		protected abstract void SetValueChangedClientSideEvent();
		protected abstract object GetEditorValue();
		protected ASPxEditBase Editor { get; private set; }
		protected WebControl EditorContainer { get; private set; }
		protected new RibbonEditItemBase Item { get { return (RibbonEditItemBase)base.Item; } }
		protected override void CreateInnerControl() {
			EditorContainer = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			Editor = CreateEditor();
			Editor.ID = GetEditorID();
			Editor.Value = Item.Value;
			Editor.Properties.Assign(GetEditorProperties());
			Editor.EncodeHtml = Ribbon.EncodeHtml;
			Editor.CustomJSProperties += (s, e) => RibbonHelper.AddItemControlCustomJSProperty(Item, e);
			Editor.ToolTip = Item.GetToolTip();
			EditorContainer.Controls.Add(Editor);
			Controls.Add(EditorContainer);
			Ribbon.EditorItems[Item] = Editor;
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			RenderUtils.AppendDefaultDXClassName(this, RibbonStyles.EditorItemCssClass);
			Editor.ParentStyles = Ribbon.StylesEditors;
			RenderUtils.SetVerticalAlignClass(EditorContainer, VerticalAlign.Middle);
			SetValueChangedClientSideEvent();
		}
		protected override bool PlaceLabelAfterControl() {
			return false;
		}
		protected override bool IsLabelVisible() {
			return base.IsLabelVisible() && !(Item is RibbonCheckBoxItem);
		}
		protected override bool HasRootTag() { return true; }
		protected override System.Web.UI.HtmlTextWriterTag TagKey { get { return System.Web.UI.HtmlTextWriterTag.Div; } }
		protected static void SetValueChangedClientSideEvent(EditClientSideEvents clientSideEvents, string valueChangedClientEventHandler) {
			if(string.IsNullOrWhiteSpace(clientSideEvents.ValueChanged)) {
				clientSideEvents.ValueChanged = valueChangedClientEventHandler;
				return;
			}
			clientSideEvents.ValueChanged = string.Format(
				@"function(s, e) {{ 
                    ({0})(s, e); 
                    ({1})(s, e); 
                }}", 
				clientSideEvents.ValueChanged, valueChangedClientEventHandler
			);
		}
	}
	public class RibbonComboBoxItemControl : RibbonEditItemControlBase {
		const string ValueChangedClientEventHandler = "ASPxClientRibbon.onComboBoxValueChanged";
		public RibbonComboBoxItemControl(ASPxRibbon ribbon, RibbonComboBoxItem item)
			:base(ribbon, item) { }
		protected new RibbonComboBoxItem Item { get { return (RibbonComboBoxItem)base.Item; } }
		protected new ASPxComboBox Editor { get { return (ASPxComboBox)base.Editor; } }
		ComboBoxProperties Properties { get { return Item.GetComboBoxProperties(); } }
		protected override ASPxEditBase CreateEditor() {
			return new ASPxComboBox();
		}
		protected override void CreateInnerControl() {
			base.CreateInnerControl();
			foreach (ListEditItem item in Item.Items) {
				if(item.Selected)
					Editor.Items[item.Index].Selected = true;
			}
			Dictionary<int, string> htmlTextItemsDictionary = Item.GetHtmlTextItemsDictionary();
			if(htmlTextItemsDictionary != null && htmlTextItemsDictionary.Count > 0) {
				Editor.EncodeHtml = false;
				foreach(ListEditItem item in Editor.Items) {
					if(htmlTextItemsDictionary.ContainsKey(item.Index))
						item.Text = htmlTextItemsDictionary[item.Index];
				}
			}
			if(!string.IsNullOrEmpty(Properties.DataSourceID))
				Editor.DataSourceID = Properties.DataSourceID;
			if(Properties.DataSource != null) {
				Editor.DataSource = Properties.DataSource;
				Editor.DataBind();
			}
		}
		protected override PropertiesBase GetEditorProperties() {
			return Properties;
		}
		protected override string GetEditorID() {
			return RibbonHelper.GetComboBoxID(Item);
		}
		protected override void SetValueChangedClientSideEvent() {
			SetValueChangedClientSideEvent(Editor.ClientSideEvents, ValueChangedClientEventHandler);
		}
		protected override object GetEditorValue() {
			return Item.Value;
		}
	}
	public class RibbonDateEditItemControl : RibbonEditItemControlBase {
		const string ValueChangedClientEventHandler = "ASPxClientRibbon.onDateEditValueChanged";
		public RibbonDateEditItemControl(ASPxRibbon ribbon, RibbonDateEditItem item)
			:base(ribbon, item) { }
		protected new RibbonDateEditItem Item { get { return (RibbonDateEditItem)base.Item; } }
		protected new ASPxDateEdit Editor { get { return (ASPxDateEdit)base.Editor; } }
		protected override ASPxEditBase CreateEditor() {
			return new ASPxDateEdit();
		}
		protected override PropertiesBase GetEditorProperties() {
			return Item.PropertiesDateEdit;
		}
		protected override string GetEditorID() {
			return RibbonHelper.GetDateEditID(Item);
		}
		protected override void SetValueChangedClientSideEvent() {
			SetValueChangedClientSideEvent(Editor.ClientSideEvents, ValueChangedClientEventHandler);
		}
		protected override object GetEditorValue() {
			return Item.Value;
		}
	}
	public class RibbonSpinEditItemControl : RibbonEditItemControlBase {
		const string ValueChangedClientEventHandler = "ASPxClientRibbon.onSpinEditValueChanged";
		public RibbonSpinEditItemControl(ASPxRibbon ribbon, RibbonSpinEditItem item)
			:base(ribbon, item) { }
		protected new RibbonSpinEditItem Item { get { return (RibbonSpinEditItem)base.Item; } }
		protected new ASPxSpinEdit Editor { get { return (ASPxSpinEdit)base.Editor; } }
		protected override ASPxEditBase CreateEditor() {
			return new ASPxSpinEdit();
		}
		protected override PropertiesBase GetEditorProperties() {
			return Item.PropertiesSpinEdit;
		}
		protected override string GetEditorID() {
			return RibbonHelper.GetSpinEditID(Item);
		}
		protected override void SetValueChangedClientSideEvent() {
			SetValueChangedClientSideEvent(Editor.ClientSideEvents, ValueChangedClientEventHandler);
		}
		protected override object GetEditorValue() {
			return null;
		}
	}
	public class RibbonTextBoxItemControl : RibbonEditItemControlBase {
		const string ValueChangedClientEventHandler = "ASPxClientRibbon.onTextBoxValueChanged";
		public RibbonTextBoxItemControl(ASPxRibbon ribbon, RibbonTextBoxItem item)
			:base(ribbon, item) { }
		protected new RibbonTextBoxItem Item { get { return (RibbonTextBoxItem)base.Item; } }
		protected new ASPxTextBox Editor { get { return (ASPxTextBox)base.Editor; } }
		protected override ASPxEditBase CreateEditor() {
			return new ASPxTextBox();
		}
		protected override PropertiesBase GetEditorProperties() {
			return Item.PropertiesTextBox;
		}
		protected override string GetEditorID() {
			return RibbonHelper.GetTextBoxID(Item);
		}
		protected override void SetValueChangedClientSideEvent() {
			SetValueChangedClientSideEvent(Editor.ClientSideEvents, ValueChangedClientEventHandler);
		}
		protected override object GetEditorValue() {
			return Item.Value;
		}
	}
	public class RibbonCheckBoxItemControl : RibbonEditItemControlBase {
		const string ValueChangedClientEventHandler = "ASPxClientRibbon.onCheckBoxCheckedChanged";
		public RibbonCheckBoxItemControl(ASPxRibbon ribbon, RibbonCheckBoxItem item)
			:base(ribbon, item) { }
		protected new RibbonCheckBoxItem Item { get { return (RibbonCheckBoxItem)base.Item; } }
		protected new ASPxCheckBox Editor { get { return (ASPxCheckBox)base.Editor; } }
		protected override object GetEditorValue() {
			return Item.Value;
		}
		protected override string GetEditorID() {
			return RibbonHelper.GetCheckBoxID(Item);
		}
		protected override void SetValueChangedClientSideEvent() {
			SetValueChangedClientSideEvent(Editor.ClientSideEvents, ValueChangedClientEventHandler);
		}
		protected override PropertiesBase GetEditorProperties() {
			return Item.PropertiesCheckBox;
		}
		protected override ASPxEditBase CreateEditor() {
			return new ASPxCheckBox() { Text = Item.GetText() };
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			RenderUtils.AppendDefaultDXClassName(this, RibbonStyles.CheckBoxItemCssClass);
		}
	}
	public class RibbonToggleButtonItemControl : RibbonButtonControl {
		public RibbonToggleButtonItemControl(ASPxRibbon ribbon, RibbonToggleButtonItem item)
			: base(ribbon, item, item.GetSmallImage(), item.GetLargeImage(), item.GetNavigateUrl(), item.GetTarget()) { }
	}
	public class RibbonTemplateItemControl : RibbonItemControlBase {
		public RibbonTemplateItemControl(ASPxRibbon ribbon, RibbonTemplateItem item)
			:base(ribbon, item) { }
		protected new RibbonTemplateItem Item { get { return (RibbonTemplateItem)base.Item; } }
		protected override void CreateInnerControl() {
			if(Item.Template != null) {
				RibbonItemTemplateContainer container = new RibbonItemTemplateContainer(Item);
				Item.Template.InstantiateIn(container);
				container.AddToHierarchy(this, RibbonHelper.GetItemTemplateContainerID(Item));
			}
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			RenderUtils.AppendDefaultDXClassName(this, RibbonStyles.TemplateItemCssClass);
		}
		protected override HtmlTextWriterTag TagKey { get { return HtmlTextWriterTag.Div; } }
	}
	public class RibbonDropDownButtonControl : RibbonButtonControl {
		public RibbonDropDownButtonControl(ASPxRibbon ribbon, RibbonItemBase item, bool dropDownMode, ItemImagePropertiesBase smallImage, ItemImagePropertiesBase largeImage,
			string navigateUrl, string target)
			: base(ribbon, item, smallImage, largeImage, navigateUrl, target) {
			DropDownMode = dropDownMode;
		}
		public RibbonDropDownButtonControl(ASPxRibbon ribbon, RibbonButtonItem item, bool dropDownMode)
			: this(ribbon, item, dropDownMode, item.GetSmallImage(), item.GetLargeImage(), item.GetNavigateUrl(), item.GetTarget()) {
		}
		protected bool DropDownMode { get; private set; }
		protected WebControl PopOut { get; private set; }
		protected Image PopOutImage { get; private set; }
		protected WebControl ImageContainer { get; private set; }
		protected override void AddImage(Image image) {
			ImageContainer.Controls.Add(image);
		}
		protected override void CreateInnerControl() {
			ImageContainer = RenderUtils.CreateWebControl(HtmlTextWriterTag.A);
			base.CreateInnerControl();
			Controls.Add(ImageContainer);
			PopOut = RenderUtils.CreateWebControl(System.Web.UI.HtmlTextWriterTag.Span);
			var text = Ribbon.HtmlEncode(Item.GetText());
			if(!DesignMode && Item.GetSize() == RibbonItemSize.Large && !string.IsNullOrEmpty(text) && !text.Contains(" ") && !Item.OneLineMode)
				LabelContentControl.Controls.Add(RenderUtils.CreateBr());
			LabelContentControl.Controls.Add(PopOut);
			PopOutImage = RenderUtils.CreateImage();
			PopOutImage.ID = RibbonHelper.GetItemPopOutImageID(Item);
			PopOut.Controls.Add(PopOutImage);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			ImageContainer.CssClass = RibbonStyles.DropDownButtonImageContainer;
			if(Item.GetSize() != RibbonItemSize.Large || Item.OneLineMode)
				RenderUtils.SetVerticalAlignClass(ImageContainer, VerticalAlign.Middle);
			if(DropDownMode)
				RenderUtils.AppendDefaultDXClassName(this, RibbonStyles.DropDownButtonMode);
			if(!DesignMode)
				PopOut.CssClass = RibbonStyles.DropDownButtonPopOutCssClass;
			RenderUtils.SetVerticalAlignClass(PopOut, VerticalAlign.Middle);
			Ribbon.Images.GetPopOutImageProperties().AssignToControl(PopOutImage, DesignMode);
			if(LabelTextControl != null)
				LabelTextControl.ForeColor = ForeColor;
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			PopOut = null;
			ImageContainer = null;
		}
		protected override void AddLinkAttributes() {
			base.AddLinkAttributes();
			if(Item.GetSize() == RibbonItemSize.Small || Item.OneLineMode) {
				LabelTextControl.Attributes.Add("href", GetLinkAttributesOwner().Attributes["href"]);
				LabelTextControl.Attributes.Add("target", GetLinkAttributesOwner().Attributes["target"]);
			}
		}
		protected override bool IsLinkAttributesRequired() {
			return base.IsLinkAttributesRequired() && DropDownMode;
		}
		protected override WebControl GetLinkAttributesOwner() {
			return ImageContainer;
		}
		protected override bool IsLabelVisible() {
			return true;
		}
		protected override HtmlTextWriterTag GetLabelTextControlTag() {
			return HtmlTextWriterTag.A;
		}
		protected override System.Web.UI.HtmlTextWriterTag TagKey { get { return System.Web.UI.HtmlTextWriterTag.Span; } }
	}
	public class RibbonDropDownToggleButtonControl : RibbonDropDownButtonControl {
		public RibbonDropDownToggleButtonControl(ASPxRibbon ribbon, RibbonButtonItem item)
			: base(ribbon, item, true) { }
	}
	public class RibbonColorButtonItemControl : RibbonDropDownButtonControl {
		const string ColorDivID = "_CD";
		public RibbonColorButtonItemControl(ASPxRibbon ribbon, RibbonColorButtonItemBase item)
			: base(ribbon, item, true) { }
		protected new RibbonColorButtonItemBase Item { get { return (RibbonColorButtonItemBase)base.Item; } }
		protected WebControl ColorDiv { get; private set; }
		protected override void CreateInnerControl() {
			base.CreateInnerControl();
			ColorDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Span);
			ColorDiv.ID = RibbonHelper.GetColorIndicatorID(Item);
			ImageContainer.Controls.Add(ColorDiv);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			RenderUtils.AppendDefaultDXClassName(this, RibbonStyles.ColorButtonCssClass);
			ColorDiv.CssClass = RibbonStyles.ColorDivCssClass;
			if(!this.Image16by16.Visible)
				RenderUtils.AppendDefaultDXClassName(ImageContainer, RibbonStyles.ColorButtonImageContainer16NoImageCssClass);
			if(!Item.OneLineMode && Item.GetSize() == RibbonItemSize.Large && !this.Image32by32.Visible)
				RenderUtils.AppendDefaultDXClassName(ImageContainer, RibbonStyles.ColorButtonImageContainer32NoImageCssClass);
			if(DesignMode)
				ColorDiv.BackColor = Item.Color.IsEmpty ? System.Drawing.Color.Gray : Item.Color;
		}
	}
	public class RibbonGalleryDropDownControl : RibbonDropDownButtonControl {
		public RibbonGalleryDropDownControl(ASPxRibbon ribbon, RibbonButtonItem item)
			: base(ribbon, item, false) { }
		public RibbonGalleryDropDownControl(ASPxRibbon ribbon, RibbonItemBase item, ItemImagePropertiesBase smallImage, ItemImagePropertiesBase largeImage,
			string navigateUrl, string target)
			: base(ribbon, item, false, smallImage, largeImage, navigateUrl, target) { }
	}
	public class RibbonGalleryBarControl : RibbonItemControlBase {
		protected WebControl BarGalleryContainer { get; private set; }
		public RibbonGalleryBarControl(ASPxRibbon ribbon, RibbonGalleryBarItem item)
			: base(ribbon, item) {
		}
		protected new RibbonGalleryBarItem Item { get { return (RibbonGalleryBarItem)base.Item; } }
		protected override void CreateInnerControl() {
			var buttonsControl = new RibbonGalleryBarButtonsControl(Item);
			Controls.Add(buttonsControl);
			var galleryProperties = new RibbonGalleryInfo(Item);
			galleryProperties.ShowGroupCaption = false;
			RibbonGalleryControl ribbonGallery = !DesignMode ? new RibbonGalleryControl(galleryProperties, RibbonHelper.GetGalleryBarItemID)
				: new RibbonGalleryControlDesignTime(galleryProperties, RibbonHelper.GetGalleryBarItemID);
			BarGalleryContainer = RenderUtils.CreateDiv();
			BarGalleryContainer.Controls.Add(ribbonGallery);
			Controls.Add(BarGalleryContainer);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			CssClass = RenderUtils.CombineCssClasses(CssClass, RibbonStyles.GalleryBarItemCssClass);
			BarGalleryContainer.CssClass = RibbonStyles.GalleryBarContainerCssClass;
		}
		protected override System.Web.UI.HtmlTextWriterTag TagKey { get { return System.Web.UI.HtmlTextWriterTag.Div; } }
	}
	public class RibbonGalleryBarButtonsControl : ASPxInternalWebControl {
		protected WebControl ButtonsContainer { get; private set; }
		protected RibbonGalleryBarItem GalleryBar { get; private set; }
		protected Image UpImage { get; private set; }
		protected Image DownImage { get; private set; }
		protected Image PopOutImage { get; private set; }
		protected ASPxRibbon Ribbon { get { return GalleryBar.Ribbon; } }
		public RibbonGalleryBarButtonsControl(RibbonGalleryBarItem galleryBar) { 
			GalleryBar = galleryBar; 
		}
		protected override void CreateControlHierarchy() {
			ButtonsContainer = RenderUtils.CreateDiv();
			var upButton = RenderUtils.CreateDiv();
			upButton.ID = RibbonHelper.GetGalleryUpButtonID(GalleryBar);
			UpImage = RenderUtils.CreateImage();
			UpImage.ID = RibbonHelper.GetGalleryUpButtonImageID(GalleryBar);
			upButton.Controls.Add(UpImage);
			var downButton = RenderUtils.CreateDiv();
			downButton.ID = RibbonHelper.GetGalleryDownButtonID(GalleryBar);
			DownImage = RenderUtils.CreateImage();
			DownImage.ID = RibbonHelper.GetGalleryDownButtonImageID(GalleryBar);
			downButton.Controls.Add(DownImage);
			var popoutButton = RenderUtils.CreateDiv();
			popoutButton.ID = RibbonHelper.GetGalleryPopOutButtonID(GalleryBar);
			PopOutImage = RenderUtils.CreateImage();
			PopOutImage.ID = RibbonHelper.GetGalleryPopOutButtonImageID(GalleryBar);
			popoutButton.Controls.Add(PopOutImage);
			ButtonsContainer.Controls.Add(upButton);
			ButtonsContainer.Controls.Add(downButton);
			ButtonsContainer.Controls.Add(popoutButton);
			Controls.Add(ButtonsContainer);
		}
		protected override void PrepareControlHierarchy() {
			ButtonsContainer.CssClass = RibbonStyles.GalleryBarButtonsCssClass;
			Ribbon.Images.GetGalleryUpButtonImageProperties().AssignToControl(UpImage, DesignMode, !IsEnabled());
			Ribbon.Images.GetGalleryDownButtonImageProperties().AssignToControl(DownImage, DesignMode, !IsEnabled());
			Ribbon.Images.GetGalleryPopOutButtonImageProperties().AssignToControl(PopOutImage, DesignMode, !IsEnabled());
		}
	}
	public class RibbonGalleryControlDesignTime: RibbonGalleryControl {
		public RibbonGalleryControlDesignTime(RibbonGalleryInfo galleryProperties, Func<RibbonGalleryItem, string> itemIDGetter) 
			: base(galleryProperties, itemIDGetter) {
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			int index = 0;
			var table = RenderUtils.CreateTable();
			var row = RenderUtils.CreateTableRow();
			foreach(RibbonGalleryGroup group in GalleryProperties.Groups) {
				foreach(RibbonGalleryItem galleryItem in group.Items) {
					if(!galleryItem.Visible)
						continue;
					var cell = RenderUtils.CreateTableCell();
					cell.Controls.Add(new RibbonGalleryItemControl(galleryItem, ItemIDGetter));
					row.Controls.Add(cell);
					index++;
					if(index >= GalleryProperties.MaxColumnCount) {
						index = 0;
						table.Controls.Add(row);
						row = RenderUtils.CreateTableRow();
					}
				}
			}
			table.Controls.Add(row);
			MainDiv.Controls.Add(table);
		}
	}
	public class RibbonGalleryControl : ASPxInternalWebControl {
		public RibbonGalleryControl(RibbonGalleryInfo galleryProperties, Func<RibbonGalleryItem, string> itemIDGetter) {
			ItemIDGetter = itemIDGetter;
			GalleryProperties = galleryProperties;
		}
		protected WebControl MainDiv { get; private set; }
		protected RibbonGalleryInfo GalleryProperties { get; private set; }
		protected List<WebControl> Groups { get; private set; }
		protected Func<RibbonGalleryItem, string> ItemIDGetter { get; private set; }
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			MainDiv = RenderUtils.CreateDiv();
			Controls.Add(MainDiv);
			Groups = new List<WebControl>();
			if(DesignMode)
				return;
			foreach(RibbonGalleryGroup group in GalleryProperties.Groups) {
				if(GalleryProperties.ShowGroupCaption) {
					var groupDiv = RenderUtils.CreateDiv();
					var text = group.Owner.Ribbon.HtmlEncode(group.Text);
					groupDiv.Controls.Add(RenderUtils.CreateLiteralControl(text));
					Groups.Add(groupDiv);
					MainDiv.Controls.Add(groupDiv);
				}
				foreach(RibbonGalleryItem galleryItem in group.Items) {
					if(!galleryItem.Visible)
						continue;
					MainDiv.Controls.Add(new RibbonGalleryItemControl(galleryItem, ItemIDGetter));
				}
			}
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			MainDiv.CssClass = RibbonStyles.GalleryMainDivCssClass;
			if(GalleryProperties.ShowText) {
				switch(GalleryProperties.ImagePosition) {
					case ImagePosition.Top:
						MainDiv.CssClass = RenderUtils.CombineCssClasses(MainDiv.CssClass, RibbonStyles.GalleryItemImagePositionTopCssClass);
						break;
					case ImagePosition.Bottom:
						MainDiv.CssClass = RenderUtils.CombineCssClasses(MainDiv.CssClass, RibbonStyles.GalleryItemImagePositionBottomCssClass);
						break;
					case ImagePosition.Right:
						MainDiv.CssClass = RenderUtils.CombineCssClasses(MainDiv.CssClass, RibbonStyles.GalleryItemImagePositionRightCssClass);
						break;
					case ImagePosition.Left:
						MainDiv.CssClass = RenderUtils.CombineCssClasses(MainDiv.CssClass, RibbonStyles.GalleryItemImagePositionLeftCssClass);
						break;
				}
			} else
				MainDiv.CssClass = RenderUtils.CombineCssClasses(MainDiv.CssClass, RibbonStyles.GalleryItemNoTextCssClass);
			if(!DesignMode)
				foreach(var group in Groups) {
					group.CssClass = RibbonStyles.GalleryGroupCssClass;
				}
		}
	}
	public class RibbonGalleryItemControl : ASPxInternalWebControl {
		private Func<RibbonGalleryItem, string> itemIDGetter;
		public RibbonGalleryItemControl(RibbonGalleryItem item, Func<RibbonGalleryItem, string> itemIDGetter) { 
			GalleryItem = item;
			this.itemIDGetter = itemIDGetter;
		}
		protected Image Image { get; private set; }
		protected WebControl ItemContentDiv { get; private set; }
		protected WebControl LiteralControl { get; private set; }
		protected RibbonGalleryItem GalleryItem { get; private set; }
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			ID = itemIDGetter(GalleryItem);
			ItemContentDiv = RenderUtils.CreateDiv();
			Image = RenderUtils.CreateImage();
			if(GalleryItem.Group.Gallery.ShowText) {
				var imagePosition = GalleryItem.Group.Gallery.ImagePosition;
				var text = GalleryItem.Group.Owner.Ribbon.HtmlEncode(GalleryItem.Text);
				switch(imagePosition) {
					case ImagePosition.Left:
					case ImagePosition.Top:
						ItemContentDiv.Controls.Add(Image);
						LiteralControl = RenderUtils.CreateWebControl(System.Web.UI.HtmlTextWriterTag.Div);
						LiteralControl.Controls.Add(RenderUtils.CreateLiteralControl(text));
						ItemContentDiv.Controls.Add(LiteralControl);
						break;
					case ImagePosition.Right:
					case ImagePosition.Bottom:
						LiteralControl = RenderUtils.CreateWebControl(System.Web.UI.HtmlTextWriterTag.Div);
						LiteralControl.Controls.Add(RenderUtils.CreateLiteralControl(text));
						ItemContentDiv.Controls.Add(LiteralControl);
						ItemContentDiv.Controls.Add(Image);
						break;				  
				}
			} else {
				ItemContentDiv.Controls.Add(Image);
			}
			Controls.Add(ItemContentDiv);
			if(GalleryItem.Group.Gallery.ShowText) { 
			}
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			GalleryItem.Image.AssignToControl(Image, DesignMode, !IsEnabled());
			Image.Style.Add(HtmlTextWriterStyle.Width, GalleryItem.Group.Gallery.ImageWidth.ToString());
			Image.Style.Add(HtmlTextWriterStyle.Height, GalleryItem.Group.Gallery.ImageHeight.ToString());
			CssClass = RibbonStyles.GalleryItemCssClass;
			ItemContentDiv.CssClass = RibbonStyles.GalleryItemContentCssClass;
			if(GalleryItem.Group.Gallery.ShowText) {
				LiteralControl.CssClass = RibbonStyles.GalleryItemTextCssClass;
				if (!GalleryItem.Group.Gallery.MaxTextWidth.IsEmpty)
					LiteralControl.Style.Add(HtmlTextWriterStyle.Width, GalleryItem.Group.Gallery.MaxTextWidth.ToString());
			}
			if(!string.IsNullOrEmpty(GalleryItem.ToolTip))
				ToolTip = GalleryItem.ToolTip;
		}
		protected override System.Web.UI.HtmlTextWriterTag TagKey { get { return System.Web.UI.HtmlTextWriterTag.Div; } }
		protected override bool HasRootTag() { return true; }
	}
	public class RibbonColorButtonExternalControl : ASPxInternalWebControl {
		const string ColorTableID = "CT";
		const string ColorChangedClientEventHandler = "ASPxClientRibbon.onColorTableColorChanged";
		const string ColorNestedControlShouldBeClosedEventHandler = "ASPxClientRibbon.onCNCShouldBeClosed";
		const string ColorNestedControlCustomColorTableUpdatedEventHandler = "ASPxClientRibbon.OnCNCCustomColorTableUpdated";
		const string PopupCloseUpClientEventHandler = "ASPxClientRibbon.onItemPopupCloseUp";
		const string PopupPopUpClientEventHandler = "ASPxClientRibbon.onColorButtonPopupPopUp";
		public RibbonColorButtonExternalControl(ASPxRibbon ribbon, RibbonColorButtonItemBase item) {
			Ribbon = ribbon;
			Item = item;
		}
		protected ASPxRibbon Ribbon { get; private set; }
		protected RibbonColorButtonItemBase Item { get; private set; }
		protected ASPxPopupControl PopupControl { get; private set; }
		protected ColorNestedControl ColorNestedControl { get; private set; }
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			PopupControl = new ASPxPopupControl();
			PopupControl.ID = RibbonHelper.GetDropDownPopupID(Item);
			PopupControl.PopupAnimationType = AnimationType.Auto;
			PopupControl.ShowCloseButton = false;
			PopupControl.PopupVerticalAlign = PopupVerticalAlign.Below;
			PopupControl.PopupVerticalOffset = -1;
			PopupControl.PopupHorizontalAlign = PopupHorizontalAlign.LeftSides;
			PopupControl.ShowHeader = false;
			PopupControl.ShowShadow = false;
			PopupControl.Width = 0;
			PopupControl.Height = 0;
			PopupControl.Border.Assign(Border.NullBorder);
			PopupControl.CustomJSProperties += (s, e) => RibbonHelper.AddItemControlCustomJSProperty(Item, e);
			Controls.Add(PopupControl);
			ColorNestedControl = new ColorNestedControl(Item.ColorNestedControlProperties);
			ClientIDHelper.EnableClientIDGeneration(ColorNestedControl);
			ColorNestedControl.ID = ColorTableID;
			ColorNestedControl.ClientSideEvents.ShouldBeClosed = ColorNestedControlShouldBeClosedEventHandler;
			ColorNestedControl.ClientSideEvents.CustomColorTableUpdated = ColorNestedControlCustomColorTableUpdatedEventHandler;
			PopupControl.Controls.Add(ColorNestedControl);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			PopupControl.ContentStyle.CopyFrom(Ribbon.Styles.GetItemDropDownContentStyle());
			PopupControl.ClientSideEvents.PopUp = PopupPopUpClientEventHandler;
			PopupControl.ClientSideEvents.CloseUp = PopupCloseUpClientEventHandler;
			ColorNestedControl.ColorTableStyle.CopyFrom(Ribbon.StylesEditors.ColorTable);
			ColorNestedControl.ColorTableCellStyle.CopyFrom(Ribbon.StylesEditors.ColorTableCell);
			ColorNestedControl.ColorPickerStyle.CopyFrom(Ribbon.StylesEditors.ColorPicker);
			ColorNestedControl.CustomJSProperties += (s, e) => RibbonHelper.AddItemControlCustomJSProperty(Item, e);
			ColorNestedControl.ClientSideEvents.ColorChanged = ColorChangedClientEventHandler;
		}
	}
	public class RibbonDropDownButtonExternalControl : ASPxInternalWebControl {
		const string MenuItemClickClientEventHandler = "ASPxClientRibbon.onMenuItemClick";
		const string MenuCloseUpClientEventHandler = "ASPxClientRibbon.onMenuCloseUp";
		const string MenuPopUpClientEventHandler = "ASPxClientRibbon.onMenuPopUp";
		public RibbonDropDownButtonExternalControl(ASPxRibbon ribbon, RibbonDropDownButtonItem item) {
			Item = item;
			Ribbon = ribbon;
		}
		protected RibbonDropDownButtonItem Item { get; set; }
		protected ASPxPopupMenu PopupMenu { get; private set; }
		protected ASPxRibbon Ribbon { get; private set; }
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			PopupMenu = new ASPxPopupMenu();
			PopupMenu.ID = RibbonHelper.GetDropDownMenuID(Item);
			PopupMenu.EncodeHtml = Item.PopupMenuEncodeHtml.HasValue ? Item.PopupMenuEncodeHtml.Value : Ribbon.EncodeHtml;
			PopupMenu.PopupHorizontalAlign = Ribbon.SettingsPopupMenu.PopupHorizontalAlign;
			PopupMenu.PopupVerticalAlign = Ribbon.SettingsPopupMenu.PopupVerticalAlign;
			PopupMenu.CloseAction = Ribbon.SettingsPopupMenu.CloseAction;
			PopupMenu.EnableScrolling = Ribbon.SettingsPopupMenu.EnableScrolling;
			PopupMenu.CustomJSProperties += (s, e) => RibbonHelper.AddItemControlCustomJSProperty(Item, e);
			PopupMenu.PopupVerticalOffset = -1;
			Controls.Add(PopupMenu);
			CreateMenuItems(PopupMenu.Items, Item.GetItems(), PopupMenu.ID);			
		}
		void CreateMenuItems(DevExpress.Web.MenuItemCollection menuCollection, RibbonDropDownButtonCollection ddCollection, string parentGroupName) {
			int index = 0;
			foreach(RibbonDropDownButtonItem ddItem in ddCollection) {
				var menuItem = new MenuItem();
				menuItem.TextTemplate = ddItem.TextTemplate;
				menuItem.Template = ddItem.Template;
				menuItem.Text = ddItem.GetText();
				menuItem.DropDownMode = ddItem.GetDropDownMode();
				menuItem.BeginGroup = ddItem.BeginGroup;
				menuItem.ToolTip = ddItem.GetToolTip();
				menuItem.NavigateUrl = ddItem.NavigateUrl;
				menuItem.Visible = ddItem.GetVisible();
				menuItem.ItemStyle.CopyFrom(ddItem.ItemStyle);
				if(ddItem.GetSize() == RibbonItemSize.Large && !ddItem.OneLineMode)
					menuItem.Image.CopyFrom(ddItem.GetLargeImage());
				else
					menuItem.Image.CopyFrom(ddItem.GetSmallImage());
				if(ddItem is RibbonDropDownToggleButtonItem) {
					menuItem.Checked = ((RibbonDropDownToggleButtonItem)ddItem).Checked;
					menuItem.GroupName = parentGroupName + index;
				}
				menuCollection.Add(menuItem);
				if(ddItem.GetItems().GetVisibleItemCount() > 0)
					CreateMenuItems(menuItem.Items, ddItem.GetItems(), parentGroupName + index);
				index++;
			}
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			PopupMenu.ClientSideEvents.ItemClick = MenuItemClickClientEventHandler;
			PopupMenu.ClientSideEvents.CloseUp = MenuCloseUpClientEventHandler;
			PopupMenu.ClientSideEvents.PopUp = MenuPopUpClientEventHandler;
			Ribbon.StylesPopupMenu.Style.MergeWith(Ribbon.Styles.GetItemDropDownContentStyle());
			PopupMenu.ParentStyles = Ribbon.StylesPopupMenu;
		}
	}
	public class RibbonPopupGalleryExternalControl : ASPxInternalWebControl {
		const string CloseUpClientEventHandler = "ASPxClientRibbon.onPopupGalleryCloseUp";
		const string PopUpClientEventHandler = "ASPxClientRibbon.onPopupGalleryPopUp";
		public RibbonPopupGalleryExternalControl(ASPxRibbon ribbon, RibbonItemBase item, RibbonGalleryInfo galleryProperties) {
			Item = item;
			Ribbon = ribbon;
			GalleryProperties = galleryProperties;
		}
		protected ASPxRibbon Ribbon { get; private set; }
		protected RibbonItemBase Item { get; private set; }
		protected RibbonGalleryInfo GalleryProperties { get; private set; }
		private ASPxPopupControl PopupControl { get; set; }
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			PopupControl = new ASPxPopupControl();
			PopupControl.ID = RibbonHelper.GetPopupGalleryID(Item);
			PopupControl.ShowHeader = false;
			PopupControl.ShowFooter = true;
			PopupControl.FooterText = "";
			PopupControl.ShowSizeGrip = ShowSizeGrip.True;
			PopupControl.AllowResize = true;
			PopupControl.ScrollBars = ScrollBars.Vertical; 
			PopupControl.PopupAlignCorrection = PopupAlignCorrection.Disabled;
			PopupControl.ContentStyle.Paddings.Padding = 0;
			PopupControl.PopupAnimationType = AnimationType.Fade;
			PopupControl.CustomJSProperties += (s, e) => RibbonHelper.AddItemControlCustomJSProperty(Item, e);
			PopupControl.Controls.Add(new RibbonGalleryControl(GalleryProperties, RibbonHelper.GetGalleryDropDownItemID));
			Controls.Add(PopupControl);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			PopupControl.ClientSideEvents.PopUp = PopUpClientEventHandler;
			PopupControl.ClientSideEvents.CloseUp = CloseUpClientEventHandler;
		}
	}
	public static class RibbonItemControlFactory {
		const int MaximumExtendedBlockSize = 3;
		const int MaximumSeparatedBlockSize = 1;
		static bool ItemHaveSameBlock(RibbonItemsBlockControl block, RibbonItemBase item) {
			if(block.BlockType == RibbonBlockType.RegularItems)
				return true;
			if(block.BlockItems.Any() && block.BlockItems.Last().GetSubGroupName() != item.GetSubGroupName())
				return false;
			if(block.BlockType == RibbonBlockType.SeparateItems && block.BlockItems.Count >= MaximumSeparatedBlockSize)
				return false;
			if(block.BlockItems.Count >= MaximumExtendedBlockSize)
				return false;
			if(block.BlockType == RibbonBlockType.LargeItems && item.BeginGroup)
				return false;
			return true;
		}
		public static List<RibbonItemsBlockControl> CreateItemBlocks(ASPxRibbon ribbon, IEnumerable<RibbonItemBase> items) {
			return CreateItemBlocks(ribbon, items, false);
		}
		public static List<RibbonItemsBlockControl> CreateItemBlocks(ASPxRibbon ribbon, IEnumerable<RibbonItemBase> items, bool designMode) {
			List<RibbonItemsBlockControl> blocks = new List<RibbonItemsBlockControl>();
			foreach(var item in items) {
				if(!item.GetVisible()) continue;
				RibbonBlockType blockType = GetRibbonBlockType(item);
				if(item.OneLineMode || !blocks.Any() || blocks.Last().BlockType != blockType || !ItemHaveSameBlock(blocks.Last(), item))
					blocks.Add(designMode ? new RibbonItemsBlockControlDesignTime(ribbon, blockType) : new RibbonItemsBlockControl(ribbon, blockType));
				blocks.Last().BlockItems.Add(item);
			}
			return blocks;
		}
		public static RibbonBlockType GetRibbonBlockType(RibbonItemBase item) {
			if(item.Ribbon != null && item.OneLineMode)
				return RibbonBlockType.HorizontalItems;
			if(item is RibbonComboBoxItem)
				return string.IsNullOrEmpty(item.GetText()) ? RibbonBlockType.RegularItems : RibbonBlockType.HorizontalItems;
			if(item is RibbonCheckBoxItem)
				return RibbonBlockType.HorizontalItems;
			if(item is RibbonGalleryBarItem)
				return RibbonBlockType.SeparateItems;
			if(item.GetSize() == RibbonItemSize.Large)
				return RibbonBlockType.LargeItems;
			else if(string.IsNullOrEmpty(item.GetText()))
				return RibbonBlockType.RegularItems;
			else
				return RibbonBlockType.HorizontalItems;
		}
		public static RibbonItemControlBase CreateItemControl(ASPxRibbon ribbon, RibbonItemBase item) {
			if(item is RibbonColorButtonItemBase)
				return new RibbonColorButtonItemControl(ribbon, (RibbonColorButtonItemBase)item);
			if(item is RibbonDropDownToggleButtonItem)
				return new RibbonDropDownToggleButtonControl(ribbon, (RibbonButtonItem)item);
			if(item is RibbonDropDownButtonItem)
				return new RibbonDropDownButtonControl(ribbon, (RibbonButtonItem)item, ((RibbonDropDownButtonItem)item).GetDropDownMode());
			if(item is RibbonGalleryDropDownItem)
				return new RibbonGalleryDropDownControl(ribbon, (RibbonButtonItem)item);
			if(item is RibbonGalleryBarItem && item.IsButtonMode())
				return new RibbonGalleryDropDownControl(ribbon, item, ((RibbonGalleryBarItem)item).GetSmallImage(), new RibbonItemImageProperties(item), "", "");
			if(item is RibbonGalleryBarItem)
				return new RibbonGalleryBarControl(ribbon, (RibbonGalleryBarItem)item);
			if(item is RibbonButtonItem)
				return new RibbonButtonControl(ribbon, (RibbonButtonItem)item);
			if(item is RibbonComboBoxItem)
				return new RibbonComboBoxItemControl(ribbon, (RibbonComboBoxItem)item);
			if(item is RibbonDateEditItem)
				return new RibbonDateEditItemControl(ribbon, (RibbonDateEditItem)item);
			if(item is RibbonSpinEditItem)
				return new RibbonSpinEditItemControl(ribbon, (RibbonSpinEditItem)item);
			if(item is RibbonTextBoxItem)
				return new RibbonTextBoxItemControl(ribbon, (RibbonTextBoxItem)item);
			if(item is RibbonCheckBoxItem)
				return new RibbonCheckBoxItemControl(ribbon, (RibbonCheckBoxItem)item);
			if(item is RibbonToggleButtonItem)
				return new RibbonToggleButtonItemControl(ribbon, (RibbonToggleButtonItem)item);
			if(item is RibbonTemplateItem)
				return new RibbonTemplateItemControl(ribbon, (RibbonTemplateItem)item);
			throw new NotImplementedException();
		}
		public static WebControl CreateItemExternalControl(ASPxRibbon ribbon, RibbonItemBase item) {
			if(item is RibbonColorButtonItemBase)
				return new RibbonColorButtonExternalControl(ribbon, (RibbonColorButtonItemBase)item);
			if(item is RibbonDropDownButtonItem)
				return new RibbonDropDownButtonExternalControl(ribbon, (RibbonDropDownButtonItem)item);
			if(item is RibbonGalleryDropDownItem)
				return new RibbonPopupGalleryExternalControl(ribbon, item, new RibbonGalleryInfo((RibbonGalleryDropDownItem)item));
			if(item is RibbonGalleryBarItem && item.OneLineMode)
				return new RibbonPopupGalleryExternalControl(ribbon, item, new RibbonGalleryInfo((RibbonGalleryBarItem)item));
			if(item is RibbonGalleryBarItem)
				return new RibbonPopupGalleryExternalControl(ribbon, item, new RibbonGalleryInfo((RibbonGalleryBarItem)item));
			return null;
		}
	}
}
