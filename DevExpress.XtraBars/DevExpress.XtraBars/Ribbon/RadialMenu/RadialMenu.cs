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
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Text;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraEditors;
using System.Linq;
using System.Collections;
using DevExpress.Utils.Menu;
namespace DevExpress.XtraBars.Ribbon {
	[Designer("DevExpress.XtraBars.Design.RadialMenuDesigner, " + AssemblyInfo.SRAssemblyBarsDesignFull, typeof(IDesigner)),
	DXToolboxItem(true),
	ToolboxBitmap(typeof(ToolboxIcons.ToolboxIconsRootNS), "RadialMenu"),
	DesignerCategory("Component"),
	Description("A circular context menu."),
	ToolboxTabName(AssemblyInfo.DXTabNavigation),
	ProvideProperty("AutoSize", typeof(BarItem)),
	ProvideProperty("ItemAutoSize", typeof(BarLinkContainerItem)),
	]
	public class RadialMenu : PopupMenuBase, ICustomizationMenuFilterSupports, IExtenderProvider {
		internal static Color DefaultBackColor = Color.White;
		internal static Color DefaultBorderColor = Color.FromArgb(0xFF, 0xED, 0xDC, 0xEB);
		internal static Color DefaultMenuColor = Color.FromArgb(255, 165, 84, 160);
		internal static Color DefaultMenuHoverColor = Color.FromArgb(0x6D, 0x38, 0x6A);
		RadialMenuWindow window;
		RadialMenuState state;
		Image glyph;
		bool autoExpand, allowGlyphSkinning;
		Color backColor, menuColor, borderColor, menuHoverColor;
		public RadialMenu() {
			LinksHolderList = new Stack<BarLinksHolder>();
			backColor = Color.Empty;
			menuColor = Color.Empty;
			itemAutoSize = RadialMenuItemAutoSize.None;
			paintStyle = PaintStyle.Skin;
			CloseOnOuterMouseClick = true;
			CollapseOnOuterMouseClick = true;
			TextRenderingHint = TextRenderingHint.SystemDefault;
			properties = new Hashtable();
			this.autoExpand = this.allowGlyphSkinning = false;
		}
		public RadialMenu(IContainer container) : this() {
			container.Add(this);
			components = new Container();
		}
		public RadialMenu(BarManager manager) : this() {
			Manager = manager;
		}
		Hashtable properties;
		System.ComponentModel.Container components = null;
		bool IExtenderProvider.CanExtend(object target) {
			return target is BarLinkContainerItem || target is BarItem;
		}
		class Properties {
			public RadialMenuContainerItemAutoSize ItemAutoSize;
			public RadialMenuContainerItemAutoSize AutoSize;
			public Properties() {
				ItemAutoSize = RadialMenuContainerItemAutoSize.Default;
				AutoSize = RadialMenuContainerItemAutoSize.Default;
			}
		}
		Properties EnsurePropertiesExists(object key) {
			Properties prop = (Properties)properties[key];
			if(prop == null) {
				prop = new Properties();
				properties[key] = prop;
			}
			return prop;
		}
		public RadialMenuContainerItemAutoSize GetItemAutoSize(BarLinkContainerItem item) {
			return EnsurePropertiesExists(item).ItemAutoSize;
		}
		public void SetItemAutoSize(BarLinkContainerItem item, RadialMenuContainerItemAutoSize value) {
			EnsurePropertiesExists(item).ItemAutoSize = value;
			OnMenuChanged();
		}
		bool ShouldSerializeItemAutoSize(BarLinkContainerItem item) {
			return GetItemAutoSize(item) != RadialMenuContainerItemAutoSize.Default;
		}
		void ResetItemAutoSize(BarLinkContainerItem item) {
			SetItemAutoSize(item, RadialMenuContainerItemAutoSize.Default);
		}
		public RadialMenuContainerItemAutoSize GetAutoSize(BarItem item) {
			return EnsurePropertiesExists(item).AutoSize;
		}
		public void SetAutoSize(BarItem item, RadialMenuContainerItemAutoSize value) {
			EnsurePropertiesExists(item).AutoSize = value;
			OnMenuChanged();
		}
		bool ShouldSerializeAutoSize(BarItem item) {
			return GetAutoSize(item) != RadialMenuContainerItemAutoSize.Default;
		}
		void ResetAutoSize(BarItem item) {
			SetAutoSize(item, RadialMenuContainerItemAutoSize.Default);
		}
		[DefaultValue(TextRenderingHint.SystemDefault), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("RadialMenuTextRenderingHint"),
#endif
 DXCategory(CategoryName.Appearance)]
		public TextRenderingHint TextRenderingHint { get; set; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void HideInDesigner() {
			if(Manager == null || !Manager.IsDesignMode) return;
			MakeCollapsed(true);
		}
		protected override BarItemLinkCollection CreateItemLinks() {
			return new RadialMenuItemLinkCollection(this);
		}
		protected internal RadialMenuWindow Window {
			get {
				if(window == null)
					window = CreateLayeredWindow();
				return window;
			}
		}
		protected internal virtual bool IsCustomizationMode { get; set; }
		protected internal virtual bool IsDesignMode { get { return DesignMode; } }
		protected override void CustomizeCore() {
			IsCustomizationMode = true;
			State = RadialMenuState.Expanded;
			Rectangle rect = Manager.Form.RectangleToScreen(Manager.Form.ClientRectangle);
			ShowPopup(new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2));
		}
		protected internal virtual bool IsTransparentBackground { get { return BorderColor == Color.Transparent && BackColor == Color.Transparent; } }
		protected virtual RadialMenuWindow CreateLayeredWindow() {
			return new RadialMenuWindow(this);
		}
		public override void ShowPopup(Point point) {
			ShowPopup(point, AutoExpand);
		}
		public override void ShowPopup(BarManager manager, Point point, PopupControl parentPopup) {
			ShowPopup(point, AutoExpand);
		}
		public virtual void ShowPopup(Point point, bool expanded) {
			if(!CanShowRadialMenu) return;
			HidePopup();
			ResetActiveLinksHolder();
			Window.Init();
			if(!IsCustomizationMode)
				MakeCollapsed(false);
			Window.ShowPopup(point);
			OnPopup();
			if(expanded) Expand();
		}
		protected internal bool IsPopupActive {
			get { return Window != null && Window.Visible; }
		}
		protected bool CanShowRadialMenu {
			get { return !IsPopupActive; }
		}
		bool shouldForceClosing = false;
		public override void HidePopup() {
			if(!IsPopupActive) return;
			base.HidePopup();
			MakeCollapsed(true, true);
		}
		public void Collapse(bool animated) {
			MakeCollapsed(animated);
		}
		public void Collapse(bool animated, bool forceClosing) {
			MakeCollapsed(animated, forceClosing);
		}
		protected internal virtual void MakeCollapsed(bool animated) {
			MakeCollapsed(animated, false);
		}
		protected internal virtual void MakeCollapsed(bool animated, bool forceClosing) {
			this.shouldForceClosing = forceClosing;
			if(DesignMode && State == RadialMenuState.Collapsed)
				return;
			State = RadialMenuState.Collapsed;
			Window.MakeCollapsed(animated);
		}
		protected internal virtual void OnPopupClosed() {
			RaiseCloseUp();
		}
		protected void ResetActiveLinksHolder() {
			ActualLinksHolder = this;
		}
		protected internal RadialMenuState State {
			get { return state; }
			set {
				if(State == value)
					return;
				state = value;
				OnStateChanged();
			}
		}
		[DefaultValue(true), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("RadialMenuCloseOnOuterMouseClick"),
#endif
 DXCategory(CategoryName.Behavior)]
		public bool CloseOnOuterMouseClick { get; set; }
		[DefaultValue(true), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("RadialMenuCollapseOnOuterMouseClick"),
#endif
 DXCategory(CategoryName.Behavior)]
		public bool CollapseOnOuterMouseClick { get; set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override MenuDrawMode MenuDrawMode {
			get { return base.MenuDrawMode; }
			set { base.MenuDrawMode = value; }
		}
		protected virtual void OnStateChanged() {
		}
		protected internal override void OnMenuChanged() {
			base.OnMenuChanged();
			if(!Visible)
				return;
			OnMenuRadiusChanged();
			Window.UpdateMaximumSize();
			Window.Invalidate();
		}
		void ResetBackColor() { BackColor = Color.Empty; }
		bool ShouldSerializeBackColor() { return !BackColor.IsEmpty; }
		[Category("Appearance")]
		public Color BackColor {
			get { return backColor; }
			set {
				if(BackColor == value)
					return;
				backColor = value;
				OnMenuChanged();
			}
		}
		void ResetMenuColor() { MenuColor = Color.Empty; }
		bool ShouldSerializeMenuColor() { return !MenuColor.IsEmpty; }
		[Category("Appearance")]
		public Color MenuColor {
			get { return menuColor; }
			set {
				if(MenuColor == value)
					return;
				menuColor = value;
				OnMenuChanged();
			}
		}
		void ResetSubMenuHoverColor() { SubMenuHoverColor = Color.Empty; }
		bool ShouldSerializeSubMenuHoverColor() { return !SubMenuHoverColor.IsEmpty; }
		[Category("Appearance")]
		public Color SubMenuHoverColor {
			get { return menuHoverColor; }
			set {
				if(SubMenuHoverColor == value)
					return;
				menuHoverColor = value;
				OnMenuChanged();
			}
		}
		void ResetBorderColor() { BorderColor = Color.Empty; }
		bool ShouldSerializeBorderColor() { return !BorderColor.IsEmpty; }
		[Category("Appearance")]
		public Color BorderColor {
			get { return borderColor; }
			set {
				if(BorderColor == value)
					return;
				borderColor = value;
				OnMenuChanged();
			}
		}
		int menuRadius = 0;
		[DefaultValue(0), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("RadialMenuMenuRadius"),
#endif
 DXCategory(CategoryName.Layout)]
		public int MenuRadius {
			get { return menuRadius; }
			set {
				if(value < 0)
					throw new ArgumentOutOfRangeException("MenuRadius must be non-negative");
				if(value > MaxMenuRadius)
					throw new ArgumentOutOfRangeException("Menu Radius is too big");
				if(MenuRadius == value)
					return;
				menuRadius = value;
				OnMenuRadiusChanged();
			}
		}
		RadialMenuItemAutoSize itemAutoSize;
		[DefaultValue(typeof(RadialMenuItemAutoSize), "None"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("RadialMenuItemAutoSize"),
#endif
 DXCategory(CategoryName.Layout)]
		public RadialMenuItemAutoSize ItemAutoSize {
			get { return itemAutoSize; }
			set {
				if(itemAutoSize == value) return;
				itemAutoSize = value;
				OnMenuChanged();
			}
		}
		PaintStyle paintStyle;
		[DefaultValue(typeof(PaintStyle), "Skin"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("RadialMenuPaintStyle"),
#endif
 DXCategory(CategoryName.Layout)]
		public PaintStyle PaintStyle {
			get { return paintStyle; }
			set {
				if(paintStyle == value) return;
				paintStyle = value;
				OnMenuChanged();
			}
		}
		const int defaultInnerRadius = 50;
		int innerRadius = defaultInnerRadius;
		[DefaultValue(defaultInnerRadius), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("RadialMenuInnerRadius"),
#endif
 DXCategory(CategoryName.Layout)]
		public int InnerRadius {
			get { return innerRadius; }
			set {
				if(InnerRadius == value) return;
				if(value < 0)
					throw new ArgumentOutOfRangeException("InnerRadius must be non-negative");
				if(LockInitCore == 0 && value > MaxInnerRadius)
					throw new ArgumentOutOfRangeException(string.Format("InnerRadius is too big(min = 0, max = {0})", MaxInnerRadius));
				innerRadius = value;
				OnMenuChanged();
			}
		}
		protected internal int MaxInnerRadius {
			get { return Math.Max(Window.ViewInfo.CalcMenuRadius() - RadialMenuConstants.MenuBoundsWidth - RadialMenuConstants.MenuItemsToBoundsIndent, 0); }
		}
		protected internal int MaxMenuRadius {
			get {
				Size monitorSize = SystemInformation.PrimaryMonitorSize;
				return Math.Min(monitorSize.Width, monitorSize.Height) / 2;
			}
		}
		protected virtual void OnMenuRadiusChanged() {
			Window.UpdateViewInfo();
			Window.OnMenuRadiusChanged();
			if(InnerRadius > MaxInnerRadius) InnerRadius = MaxInnerRadius;
		}
		[DefaultValue(null), DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("RadialMenuGlyph"),
#endif
 Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public Image Glyph {
			get { return glyph; }
			set {
				if(Glyph == value)
					return;
				glyph = value;
				OnGlyphChanged();
			}
		}
		static Image defaultGlyph = null;
		public static Image DefaultGlyph {
			get { return defaultGlyph; }
			set {
				if(DefaultGlyph == value)
					return;
				defaultGlyph = value;
			}
		}
		[DefaultValue(false), DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("RadialMenuAllowGlyphSkinning")
#else
	Description("")
#endif
]
		public bool AllowGlyphSkinning {
			get { return allowGlyphSkinning; }
			set {
				if(AllowGlyphSkinning == value) return;
				allowGlyphSkinning = value;
				OnMenuChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RadialMenuAutoExpand"),
#endif
 DXCategory(CategoryName.Behavior), DefaultValue(false)]
		public bool AutoExpand {
			get { return autoExpand; }
			set {
				if(AutoExpand == value)
					return;
				autoExpand = value;
				OnMenuChanged();
			}
		}
		[Browsable(false)]
		public RadialMenuContainer CurrentContainer { get { return new RadialMenuContainer(ActualLinksHolder); } }
		public event RadialMenuCenterButtonClickEventHandler CenterButtonClick;
		protected virtual internal void RaiseCenterButtonClick(BarLinksHolder next) {
			if(CenterButtonClick == null) return;
			CenterButtonClick(this, new RadialMenuCenterButtonClickEventArgs(next));
		}
		protected virtual void OnGlyphChanged() {
			Window.UpdateSize(false);   
		}
		public void Expand() {
			MakeExpanded();
		}
		protected internal virtual void MakeExpanded() {
			if(State == RadialMenuState.Expanded)
				return;
			State = RadialMenuState.Expanded;
			Window.MakeExpanded();
		}
		[Browsable(false)]
		public override bool Visible { get { return Window.Visible; } }
		protected internal virtual void UpdateHoverInfo(Point point) {
			if(Window.Visible)
				Window.UpdateHoverInfo(point);
		}
		protected internal virtual void AppActiveStatusChanging(bool status) {
			if(!IsPopupActive || status) return;
			MakeCollapsed(true, true);
		}
		BarLinksHolder actualLinksHolder;
		protected internal virtual BarLinksHolder ActualLinksHolder {
			get {
				if(actualLinksHolder == null)
					actualLinksHolder = this;
				return actualLinksHolder; 
			}
			set {
				if(ActualLinksHolder == value)
					return;
				BarLinksHolder prev = ActualLinksHolder;
				actualLinksHolder = value;
				OnActualLinksHolderChanged(prev, ActualLinksHolder);
			}
		}
		public void SetLinksHolder(BarLinksHolder linksHolder) {
			ActualLinksHolder = linksHolder;
		}
		public override BarItemLink AddItem(BarItem item) {
			if(ActualLinksHolder is RadialMenu) return base.AddItem(item);
			return ActualLinksHolder.AddItem(item);
		}
		protected internal Stack<BarLinksHolder> LinksHolderList {
			get;
			set;
		}
		protected virtual void OnActualLinksHolderChanged(BarLinksHolder prev, BarLinksHolder next) {
			LinksHolderList.Push(prev);
			Window.OnActualLinksHolderChanged(prev, next);
		}
		protected internal void SetPrevHolder(BarLinksHolder holder) {
			BarLinksHolder prev = ActualLinksHolder;
			actualLinksHolder = holder;
			Window.OnActualLinksHolderChanged(prev, holder);
		}
		public override void RemoveLink(BarItemLink itemLink) {
			base.RemoveLink(itemLink);
			Window.UpdateViewInfo();
		}
		protected internal virtual BarItemLinkCollection ActualItemLinks {
			get { return ActualLinksHolder == null? ItemLinks : ActualLinksHolder.ItemLinks; } 
		}
		protected internal virtual void OnLinkChanged(BarItemLink barItemLink) {
			if(!Visible)
				return;
			OnMenuRadiusChanged();
			Window.UpdateMaximumSize();
			Window.Invalidate();
		}
		internal bool IsPointInEllipse(Point pt) {
			return IsPointInCircle(pt, Window.ViewInfo.MenuRadius);
		}
		internal bool IsPointInGlyphEllipse(Point pt) {
			return IsPointInCircle(pt, Window.ViewInfo.GlyphRadius);
		}
		bool IsPointInCircle(Point pt, int radius) {
			pt = new Point(pt.X - Window.ViewInfo.CenterPoint.X - Window.Location.X, pt.Y - Window.ViewInfo.CenterPoint.Y - Window.Location.Y);
			return Math.Sqrt(pt.X * pt.X + pt.Y * pt.Y) < radius;
		}
		protected internal virtual void ProcessMouseOuterClick(Control control, MouseInfoArgs mouseInfoArgs) {
			Point pt = mouseInfoArgs.Location;
			if(Manager.IsDesignMode || !Visible)
				return;
			if((State == RadialMenuState.Expanded && IsPointInEllipse(pt)) || (State == RadialMenuState.Collapsed && IsPointInGlyphEllipse(pt)))
				return;
			if(!ShouldPerformCollapsing) return;
			MakeCollapsed(true);
		}
		protected virtual bool ShouldPerformCollapsing {
			get {
				if(Manager != null && Manager.IsDesignMode) return true;
				return CollapseOnOuterMouseClick || CloseOnOuterMouseClick;
			}
		}
		protected internal virtual bool ShouldClose {
			get {
				if(Manager == null) return false;
				if(Manager.IsDesignMode || shouldForceClosing) return true;
				return CloseOnOuterMouseClick && State == RadialMenuState.Collapsed;
			}
		}
		protected internal virtual void OnHidingComplete() {
			this.shouldForceClosing = false;
		}
		public override void ClearLinks() {
			base.ClearLinks();
			if(Window != null)
				Window.ClearLinks();
		}
		public RadialMenuHitInfo CalcHitInfo(Point pt) {
			if(Window == null) {
				throw new InvalidOperationException("Radial Menu has not been created");
			}
			return Window.ViewInfo.CalcHitInfo(pt);
		}
		protected override bool IsItemAcceptableCore(DXMenuItem menuItem, int count) {
			if(count < 15) return true; 
			if(!menuItem.Enabled) return false;
			return menuItem.Priority != DXMenuItemPriority.Low;
		}
		#region Disposing
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(window != null) {
					window.ReleaseAnimations();
					window.DestroyHandle();
				}
				window = null;
			}
			base.Dispose(disposing);
		}
		#endregion
		static Type[] AllowedLinkTypes = new Type[] {
			typeof(BarButtonItemLink),
			typeof(BarCheckItemLink),
			typeof(BarSubItemLink),
			typeof(BarStaticItemLink),
			typeof(BarLinkContainerItemLink),
			typeof(BarLargeButtonItemLink)
		};
		#region ICustomizationMenuFilterSupports
		bool ICustomizationMenuFilterSupports.ShouldShowItem(Type type) {
			return AllowedLinkTypes.Contains(type);
		}
		#endregion
		protected internal BarManager GetManager() {
			if(Ribbon != null && Ribbon.Manager != null) return Ribbon.Manager;
			return Manager;
		}
	}
	public enum RadialMenuContainerItemAutoSize {
		Default,
		Spring,
		None
	}
	public enum RadialMenuItemAutoSize {
		Spring,
		None
	}
	public enum PaintStyle {
		Classic,
		Skin
	}
	public class RadialMenuAppearance : BaseOwnerAppearance {
		public RadialMenuAppearance(IAppearanceOwner owner) : base(owner) { }
		protected override void OnResetCore() {
		}
	}
	[System.ComponentModel.Design.Serialization.DesignerSerializer("DevExpress.XtraBars.Design.Serialization.RibbonItemLinksSerializer, " + AssemblyInfo.SRAssemblyBarsDesign, "System.ComponentModel.Design.Serialization.CodeDomSerializer, System.Design")]
	class RadialMenuItemLinkCollection : BarItemLinkCollection {
		RadialMenu menu;
		public RadialMenuItemLinkCollection(object owner)
			: base(owner) {
			this.menu = (RadialMenu)owner;
		}
		RibbonControl Ribbon { get { return menu.Ribbon; } }
		protected internal override LinksInfo LinksPersistInfo {
			get {
				if(Ribbon != null) return null;
				return base.LinksPersistInfo;
			}
			set {
				if(Ribbon != null) return;
				base.LinksPersistInfo = value;
			}
		}
	}
	public enum RadialMenuState { Collapsed, Expanded }
	public delegate void RadialMenuCenterButtonClickEventHandler(object sender, RadialMenuCenterButtonClickEventArgs e);
	public class RadialMenuCenterButtonClickEventArgs : EventArgs {
		public RadialMenuCenterButtonClickEventArgs(BarLinksHolder holder) {
			NextContainer = new RadialMenuContainer(holder);
		}
		public RadialMenuContainer NextContainer { get; private set; }
	}
	public class RadialMenuContainer {
		public RadialMenuContainer(BarLinksHolder holder) {
			IsRadialMenu = holder is RadialMenu;
			IsBarSubItem =  IsRadialMenu ? false : holder is BarSubItem;
			IsBarLinkContainerItem = IsBarSubItem ? false : holder is BarLinkContainerItem;
		}
		public bool IsRadialMenu { get; private set; }
		public bool IsBarSubItem { get; private set; }
		public bool IsBarLinkContainerItem { get; private set; }
	}
}
