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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.ButtonPanel;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraTab.Buttons;
using DevExpress.XtraTab.Registrator;
namespace DevExpress.XtraTab.ViewInfo {
	public enum XtraTabHitTest { None, PageHeader, PageHeaderButtons, PageClient }
	public class BaseTabHitInfo {
		IXtraTabPage fPage;
		WeakReference fPageWeak;
		protected XtraTabHitTest fHitTest;
		Point hitPoint;
		protected internal bool fInControlBox = false;
		public BaseTabHitInfo() {
			Clear();
		}
		public virtual void Clear() {
			this.hitPoint = new Point(-10000, -10000);
			this.fHitTest = XtraTabHitTest.None;
			this.fPage = null;
			this.fPageWeak = null;
		}
		public bool IsValid { get { return HitPoint.X != -10000; } }
		public Point HitPoint { get { return hitPoint; } set { hitPoint = value; } }
		public IXtraTabPage Page {
			get {
				if(fPageWeak != null)
					return (IXtraTabPage)fPageWeak.Target;
				return fPage;
			}
			set {
				fPage = value;
				fPageWeak = null;
			}
		}
		internal void MakePageReferenceWeak() {
			fPageWeak = new WeakReference(this.Page);
			fPage = null;
		}
		public XtraTabHitTest HitTest { get { return fHitTest; } set { fHitTest = value; } }
		public bool InPageControlBox {
			get { return IsValid && Page != null && fInControlBox; }
		}
	}
	public class PageEventArgs : EventArgs {
		IXtraTabPage page;
		public PageEventArgs(IXtraTabPage page) {
			this.page = page;
		}
		public IXtraTabPage Page { get { return page; } }
	}
	public class ClosePageButtonEventArgs : PageEventArgs {
		IXtraTabPage prevPage;
		public ClosePageButtonEventArgs(IXtraTabPage prevPage, IXtraTabPage page)
			: base(page) {
			this.prevPage = prevPage;
		}
		public IXtraTabPage PrevPage { get { return prevPage; } }
	}
	public class CustomHeaderButtonEventArgs : EventArgs {
		IXtraTabPage page;
		CustomHeaderButton button;
		public CustomHeaderButtonEventArgs(CustomHeaderButton button, IXtraTabPage page) {
			this.page = page;
			this.button = button;
		}
		public CustomHeaderButton Button { get { return button; } }
		public IXtraTabPage ActivePage { get { return page; } }
	}
	public class HeaderButtonEventArgs : EventArgs {
		IXtraTabPage page;
		TabButtons button;
		public HeaderButtonEventArgs(TabButtons button, IXtraTabPage page) {
			this.page = page;
			this.button = button;
		}
		public TabButtons Button { get { return button; } }
		public IXtraTabPage ActivePage { get { return page; } }
		bool handled = false;
		public bool Handled {
			get { return handled; }
			set { handled = value; }
		}
	}
	public delegate void PageEventHandler(
		object sender, PageEventArgs e
	);
	public delegate void HeaderButtonEventHandler(
		object sender, HeaderButtonEventArgs e
	);
	public delegate void CustomHeaderButtonEventHandler(
		object sender, CustomHeaderButtonEventArgs e
	);
	public class ViewInfoTabPageChangedEventArgs : EventArgs {
		IXtraTabPage prevPage, page;
		public ViewInfoTabPageChangedEventArgs(IXtraTabPage prevPage, IXtraTabPage page) {
			this.prevPage = prevPage;
			this.page = page;
		}
		public IXtraTabPage PrevPage { get { return prevPage; } }
		public IXtraTabPage Page { get { return page; } }
	}
	public class ViewInfoTabPageChangingEventArgs : ViewInfoTabPageChangedEventArgs {
		bool cancel = false;
		public ViewInfoTabPageChangingEventArgs(IXtraTabPage prevPage, IXtraTabPage page) : base(prevPage, page) { }
		public bool Cancel { get { return cancel; } set { cancel = value; } }
	}
	public delegate void ViewInfoTabPageChangingEventHandler(object sender, ViewInfoTabPageChangingEventArgs e);
	public delegate void ViewInfoTabPageChangedEventHandler(object sender, ViewInfoTabPageChangedEventArgs e);
	public class XtraTabHitInfo : BaseTabHitInfo {
		public new XtraTabPage Page { get { return base.Page as XtraTabPage; } set { base.Page = value; } }
	}
	public class BaseTabControlViewInfo : IDisposable {
		public event ViewInfoTabPageChangedEventHandler SelectedPageChanged, HotTrackedPageChanged;
		public event ViewInfoTabPageChangingEventHandler SelectedPageChanging;
		public event EventHandler PageClientBoundsChanged, CloseButtonClick;
		public event PageEventHandler TabMiddleClick;
		public event HeaderButtonEventHandler HeaderButtonClick;
		public event CustomHeaderButtonEventHandler CustomHeaderButtonClick;
		IXtraTab tabControl;
		BorderPainter pageClientBorderPainter, headerBorderPainter, headerRowBorderPainter, tabControlBorderPainter;
		bool allowFillPageClientBackground;
		AppearanceObject paintAppearance;
		int firstVisiblePageIndex = 0;
		internal int lockUpdate = 0;
		IXtraTabPage hotTrackedTabPage, selectedTabPage;
		BaseClosePageButtonHelper closeButtonHelperCore;
		protected Rectangle fBounds, fClient, fPageBounds, fPageClientBounds, fPrevPageClientBounds;
		BaseTabHeaderViewInfo headerInfo;
		GraphicsInfo graphicsInfo;
		Hashtable defaultAppearances;
		bool drawPaneWhenEmptyCore;
		public BaseTabControlViewInfo(IXtraTab tabControl) {
			this.defaultAppearances = null;
			this.paintAppearance = new AppearanceObject();
			this.fPrevPageClientBounds = Rectangle.Empty;
			this.allowFillPageClientBackground = true;
			this.selectedTabPage = null;
			this.hotTrackedTabPage = null;
			this.graphicsInfo = new GraphicsInfo();
			this.tabControl = tabControl;
			this.headerInfo = TabControl.View.CreateHeaderViewInfo(this);
			CreatePainters();
			this.closeButtonHelperCore = CreateCloseButtonHelper();
			this.drawPaneWhenEmptyCore = true;
			Clear();
		}
		protected virtual void RegisterDefaultAppearances(Hashtable appearances) {
			View.RegisterDefaultAppearances(TabControl, appearances);
		}
		public void ResetDefaultAppearances() {
			ColoredTabElementsCache.Reset();
			this.defaultAppearances = null;
		}
		protected Hashtable DefaultAppearances {
			get {
				if(defaultAppearances == null) {
					defaultAppearances = new Hashtable();
					RegisterDefaultAppearances(defaultAppearances);
				}
				return defaultAppearances;
			}
		}
		public bool IsRightToLeftLocation {
			get { return HeaderInfo.IsRightToLeftLocation; }
		}
		protected virtual BaseClosePageButtonHelper CreateCloseButtonHelper() {
			return new BaseClosePageButtonHelper(this);
		}
		protected internal BaseClosePageButtonHelper CloseButtonHelper {
			get { return closeButtonHelperCore; }
		}
		public void BeginUpdate() { this.lockUpdate++; }
		public void EndUpdate() {
			if(--this.lockUpdate == 0)
				LayoutChanged();
		}
		protected internal virtual Size CalcCloseButtonSize(Graphics g) {
			Size size = Size.Empty;
			try {
				CloseButtonHelper.ButtonInfoArgs.Graphics = g;
				size = CloseButtonHelper.ButtonPainter.CalcObjectMinBounds(CloseButtonHelper.ButtonInfoArgs).Size;
				size.Width = Math.Max(14, size.Width);
				size.Height = Math.Max(14, size.Height);
			}
			finally { CloseButtonHelper.ButtonInfoArgs.Graphics = null; }
			return size;
		}
		protected internal virtual Point CalcCloseButtonOffset() {
			Point offset = new Point(0, 0);
			if(CloseButtonHelper.ButtonPainter is SkinTabButtonPainter) {
				offset = ((SkinTabButtonPainter)CloseButtonHelper.ButtonPainter).GetOffset(CloseButtonHelper.ButtonInfoArgs);
			}
			return offset;
		}
		public bool DrawPaneWhenEmpty {
			get { return drawPaneWhenEmptyCore; }
			set { drawPaneWhenEmptyCore = value; }
		}
		public bool IsLockUpdate { get { return lockUpdate != 0; } }
		public virtual bool IsAllowPageCustomBackColor { get { return true; } }
		public virtual AppearanceDefault GetDefaultAppearance(object key) {
			AppearanceDefault res = DefaultAppearances[key] as AppearanceDefault;
			if(res == null) return AppearanceDefault.Control;
			return res;
		}
		public virtual AppearanceDefault GetPageHeaderAppearanceByState(ObjectState state) {
			TabPageAppearance key = TabPageAppearance.PageHeader;
			if((state & ObjectState.Selected) != 0) {
				key = TabPageAppearance.PageHeaderActive;
				if(AllowInactiveState && !IsActive) {
					if(DefaultAppearances.ContainsKey(TabPageAppearance.PageHeaderTabInactive))
						key = TabPageAppearance.PageHeaderTabInactive;
				}
			}
			else {
				switch(state) {
					case ObjectState.Disabled: key = TabPageAppearance.PageHeaderDisabled; break;
					case ObjectState.Hot: key = TabPageAppearance.PageHeaderHotTracked; break;
					case ObjectState.Pressed: key = TabPageAppearance.PageHeaderPressed; break;
				}
			}
			return GetDefaultAppearance(key);
		}
		protected virtual void CreatePainters() {
			this.pageClientBorderPainter = CreatePageClientBorderPainter();
			this.headerBorderPainter = TabControl.View.CreateHeaderBorderPainter();
			this.headerRowBorderPainter = TabControl.View.CreateHeaderRowBorderPainter();
			this.tabControlBorderPainter = CreateTabControlBorderPainter();
		}
		protected virtual BorderPainter CreateTabControlBorderPainter() {
			if(Properties.BorderStyle == BorderStyles.Default) return View.CreateTabControlBorderPainter();
			return BorderHelper.GetPainter(Properties.BorderStyle);
		}
		protected virtual BorderPainter CreatePageClientBorderPainter() {
			if(Properties.BorderStylePage == BorderStyles.Default) return View.CreatePageClientBorderPainter();
			return BorderHelper.GetPainter(Properties.BorderStylePage);
		}
		protected BaseViewInfoRegistrator View { get { return TabControl.View; } }
		protected virtual GraphicsInfo GraphicsInfo { get { return graphicsInfo; } }
		public virtual CustomHeaderButtonCollection CustomHeaderButtons {
			get { return PropertiesEx.CustomHeaderButtons; }
		}
		public virtual TabMiddleClickFiringMode TabMiddleClickFiringMode {
			get { return PropertiesEx.TabMiddleClickFiringMode; }
		}
		public virtual TabButtonShowMode HeaderButtonsShowMode {
			get { return Properties.HeaderButtonsShowMode; }
		}
		public virtual TabButtons HeaderButtons {
			get { return Properties == null ? TabButtons.Default : CalcTabButtons(Properties.ClosePageButtonShowMode); }
		}
		public virtual DefaultBoolean ShowClosePageButton {
			get { return Properties == null ? DefaultBoolean.Default : CalcShowClosePageButton(Properties.ClosePageButtonShowMode); }
		}
		public virtual DefaultBoolean ShowPinPageButton {
			get { return Properties == null ? DefaultBoolean.Default : CalcShowPinPageButton(Properties.PinPageButtonShowMode); }
		}
		public virtual DefaultBoolean ShowCloseButtonForInactivePages {
			get { return Properties == null ? DefaultBoolean.Default : CalcShowCloseButtonForInactivePages(Properties.ClosePageButtonShowMode); }
		}
		public virtual DefaultBoolean ShowPinButtonForInactivePages {
			get { return Properties == null ? DefaultBoolean.Default : CalcShowPinButtonForInactivePages(Properties.PinPageButtonShowMode); }
		}
		public virtual DefaultBoolean DisableDrawCloseButtonForInactivePages {
			get { return Properties == null ? DefaultBoolean.Default : CalcDisableDrawCloseButtonForInactivePages(Properties.ClosePageButtonShowMode); }
		}
		public virtual DefaultBoolean DisableDrawPinButtonForInactivePages {
			get { return Properties == null ? DefaultBoolean.Default : CalcDisableDrawPinButtonForInactivePages(Properties.PinPageButtonShowMode); }
		}
		public virtual DefaultBoolean DrawCloseButtonForInactivePage {
			get { return Properties == null ? DefaultBoolean.Default : CalcShowCloseButtonForInactivePages(Properties.ClosePageButtonShowMode); }
		}
		public virtual DefaultBoolean DrawPinButtonForInactivePage {
			get { return Properties == null ? DefaultBoolean.Default : CalcShowPinButtonForInactivePages(Properties.PinPageButtonShowMode); }
		}
		public virtual DefaultBoolean AllowGlyphSkinning {
			get { return Properties == null ? DefaultBoolean.Default : Properties.AllowGlyphSkinning; }
		}
		public virtual TabButtons CalcTabButtons(ClosePageButtonShowMode mode) {
			TabButtons buttons = Properties.HeaderButtons;
			switch(mode) {
				case ClosePageButtonShowMode.Default:
					break;
				case ClosePageButtonShowMode.InTabControlHeader:
				case ClosePageButtonShowMode.InActiveTabPageAndTabControlHeader:
				case ClosePageButtonShowMode.InAllTabPagesAndTabControlHeader:
					if((buttons & TabButtons.Close) == 0) buttons |= TabButtons.Close;
					break;
				case ClosePageButtonShowMode.InActiveTabPageHeader:
				case ClosePageButtonShowMode.InActiveTabPageHeaderAndOnMouseHover:
				case ClosePageButtonShowMode.InAllTabPageHeaders:
					if((buttons & TabButtons.Close) == TabButtons.Close) buttons &= ~TabButtons.Close;
					break;
			}
			bool fCloseButtonPresent = ((buttons & TabButtons.Close) == TabButtons.Close);
			if(HeaderInfo != null && SelectedTabPageViewInfo != null) {
				if(fCloseButtonPresent && !HeaderInfo.AllowClosePageButton(SelectedTabPageViewInfo)) {
					buttons &= ~TabButtons.Close;
				}
			}
			return buttons;
		}
		protected virtual DefaultBoolean CalcShowClosePageButton(ClosePageButtonShowMode mode) {
			DefaultBoolean result = DefaultBoolean.Default;
			switch(mode) {
				case ClosePageButtonShowMode.Default:
					break;
				case ClosePageButtonShowMode.InTabControlHeader:
					result = DefaultBoolean.False;
					break;
				case ClosePageButtonShowMode.InActiveTabPageAndTabControlHeader:
				case ClosePageButtonShowMode.InActiveTabPageHeaderAndOnMouseHover:
				case ClosePageButtonShowMode.InActiveTabPageHeader:
				case ClosePageButtonShowMode.InAllTabPageHeaders:
				case ClosePageButtonShowMode.InAllTabPagesAndTabControlHeader:
					result = DefaultBoolean.True;
					break;
			}
			return result;
		}
		protected virtual DefaultBoolean CalcShowPinPageButton(PinPageButtonShowMode mode) {
			DefaultBoolean result = DefaultBoolean.Default;
			switch(mode) {
				case PinPageButtonShowMode.Default:
					break;
				case PinPageButtonShowMode.InActiveTabPageHeaderAndOnMouseHover:
				case PinPageButtonShowMode.InActiveTabPageHeader:
				case PinPageButtonShowMode.InAllTabPageHeaders:
					result = DefaultBoolean.True;
					break;
			}
			return result;
		}
		protected virtual DefaultBoolean CalcDisableDrawCloseButtonForInactivePages(ClosePageButtonShowMode mode) {
			DefaultBoolean result = DefaultBoolean.Default;
			switch(mode) {
				case ClosePageButtonShowMode.InActiveTabPageHeaderAndOnMouseHover:
					result = DefaultBoolean.True;
					break;
				default:
					break;
			}
			return result;
		}
		protected virtual DefaultBoolean CalcDisableDrawPinButtonForInactivePages(PinPageButtonShowMode mode) {
			DefaultBoolean result = DefaultBoolean.Default;
			switch(mode) {
				case PinPageButtonShowMode.InActiveTabPageHeaderAndOnMouseHover:
					result = DefaultBoolean.True;
					break;
				default:
					break;
			}
			return result;
		}
		protected virtual DefaultBoolean CalcShowCloseButtonForInactivePages(ClosePageButtonShowMode mode) {
			DefaultBoolean result = DefaultBoolean.Default;
			switch(mode) {
				case ClosePageButtonShowMode.Default:
				case ClosePageButtonShowMode.InTabControlHeader:
					break;
				case ClosePageButtonShowMode.InActiveTabPageAndTabControlHeader:
				case ClosePageButtonShowMode.InActiveTabPageHeader:
					result = DefaultBoolean.False;
					break;
				case ClosePageButtonShowMode.InAllTabPageHeaders:
				case ClosePageButtonShowMode.InAllTabPagesAndTabControlHeader:
				case ClosePageButtonShowMode.InActiveTabPageHeaderAndOnMouseHover:
					result = DefaultBoolean.True;
					break;
			}
			return result;
		}
		protected virtual DefaultBoolean CalcShowPinButtonForInactivePages(PinPageButtonShowMode mode) {
			DefaultBoolean result = DefaultBoolean.Default;
			switch(mode) {
				case PinPageButtonShowMode.Default:
					break;
				case PinPageButtonShowMode.InActiveTabPageHeader:
					result = DefaultBoolean.False;
					break;
				case PinPageButtonShowMode.InAllTabPageHeaders:
				case PinPageButtonShowMode.InActiveTabPageHeaderAndOnMouseHover:
					result = DefaultBoolean.True;
					break;
			}
			return result;
		}
		public virtual IXtraTabProperties Properties {
			get {
				IXtraTabProperties res = TabControl as IXtraTabProperties;
				if(res == null) res = DefaultTabProperties.Default;
				return res;
			}
		}
		public virtual IXtraTabPropertiesEx PropertiesEx {
			get {
				IXtraTabPropertiesEx res = TabControl as IXtraTabPropertiesEx;
				if(res == null) res = DefaultTabProperties.Default;
				return res;
			}
		}
		public bool FireTabMiddleClickOnMouseDown {
			get {
				TabMiddleClickFiringMode mode = TabMiddleClickFiringMode;
				if(mode == TabMiddleClickFiringMode.Default)
					mode = CheckDefaultTabMiddleClickFiringMode();
				return mode == TabMiddleClickFiringMode.MouseDown;
			}
		}
		public bool FireTabMiddleClickOnMouseUp {
			get {
				TabMiddleClickFiringMode mode = TabMiddleClickFiringMode;
				if(mode == TabMiddleClickFiringMode.Default)
					mode = CheckDefaultTabMiddleClickFiringMode();
				return mode == TabMiddleClickFiringMode.MouseUp;
			}
		}
		public void SetDefaultTabMiddleClickFiringMode(TabMiddleClickFiringMode defaultMode) {
			defaultTabMiddleClickFiringModeCore = defaultMode;
		}
		TabMiddleClickFiringMode defaultTabMiddleClickFiringModeCore = TabMiddleClickFiringMode.None;
		protected virtual TabMiddleClickFiringMode CheckDefaultTabMiddleClickFiringMode() {
			return defaultTabMiddleClickFiringModeCore;
		}
		public virtual AppearanceObject PaintAppearance { get { return paintAppearance; } }
		public BaseTabHitInfo CreateHitInfo() { return TabControl.CreateHitInfo(); }
		public virtual void Invalidate() {
			TabControl.Invalidate(Rectangle.Empty);
			if(SelectedTabPage != null) SelectedTabPage.Invalidate();
		}
		public virtual void Invalidate(Rectangle rect) {
			TabControl.Invalidate(rect);
		}
		public virtual void OnPageChanged(IXtraTabPage page) {
			if(SelectedTabPage == null) {
				if(CanSelectPage(page)) SetSelectedTabPageCore(page);
			}
			else {
				if(SelectedTabPage == page && !CanSelectPage(page)) {
					SetSelectedTabPageCore(GetSelectablePage());
				}
			}
		}
		public virtual bool ShowToolTips { get { return Properties == null || Properties.ShowToolTips != DefaultBoolean.False; } }
		public virtual ToolTipControlInfo GetToolTipInfo(Point point) {
			if(!ShowToolTips) return null;
			return HeaderInfo.GetToolTipInfo(point);
		}
		public virtual IXtraTabPage GetSelectablePage() {
			for(int n = 0; n < TabControl.PageCount; n++) {
				IXtraTabPage page = TabControl.GetTabPage(n);
				if(CanSelectPage(page)) return page;
			}
			return null;
		}
		public virtual void Resize() {
			LayoutChanged();
			CheckFirstPageIndex();
		}
		public virtual void CheckFirstPageIndex() {
			if(IsLockUpdate) return;
			if(FirstVisiblePageIndex > 0) {
				FirstVisiblePageIndex = HeaderInfo.CalcResizeFirstVisibleIndex();
			}
		}
		public virtual void LayoutChanged() {
			if(IsLockUpdate) return;
			CalcViewInfo(TabControl.Bounds, null);
			Invalidate();
		}
		protected virtual void UpdatePaintAppearance() {
			AppearanceHelper.Combine(PaintAppearance, new AppearanceObject[] { Properties.Appearance }, GetDefaultAppearance(TabPageAppearance.TabControl));
		}
		public virtual TabPageImagePosition HeaderImagePosition { get { return Properties == null ? TabPageImagePosition.Near : Properties.PageImagePosition; } }
		public virtual bool ShowHeaderFocus {
			get {
				if(Properties == null || Properties.ShowHeaderFocus == DefaultBoolean.Default) return HeaderInfo.DefaultShowHeaderFocus;
				return Properties.ShowHeaderFocus == DefaultBoolean.True;
			}
		}
		public virtual bool IsTabHeaderFocused {
			get {
				if(!ShowHeaderFocus) return false;
				XtraTabControl xt = TabControl.OwnerControl as XtraTabControl;
				if(xt != null) return xt.Focused;
				return false;
			}
		}
		protected virtual bool IsNeedLayoutOnHotTrack { get { return false; } }
		public virtual BaseTabHitInfo CalcHitInfo(Point pt) {
			BaseTabHitInfo hitInfo = CreateHitInfo();
			hitInfo.HitPoint = pt;
			if(IsShowHeader && HeaderInfo.Bounds.Contains(pt)) {
				if(HeaderInfo.ButtonsBounds.Contains(pt)) {
					hitInfo.HitTest = XtraTabHitTest.PageHeaderButtons;
					return hitInfo;
				}
				BaseTabPageViewInfo pInfo = HeaderInfo.FindPage(pt);
				if(pInfo != null) {
					hitInfo.HitTest = XtraTabHitTest.PageHeader;
					hitInfo.Page = pInfo.Page;
					hitInfo.fInControlBox = pInfo.ButtonsPanel.Bounds.Contains(pt);
				}
				return hitInfo;
			}
			if(PageClientBounds.Contains(pt)) {
				hitInfo.HitTest = XtraTabHitTest.PageClient;
			}
			return hitInfo;
		}
		protected Control OwnerControl { get { return TabControl != null ? TabControl.OwnerControl : null; } }
		public virtual bool IsDesignMode {
			get {
				ISite site = OwnerControl == null ? null : OwnerControl.Site;
				if(site != null) return site.DesignMode;
				return false;
			}
		}
		public virtual BorderPainter TabControlBorderPainter { get { return tabControlBorderPainter; } }
		public virtual BorderPainter PageClientBorderPainter { get { return pageClientBorderPainter; } }
		public virtual BorderPainter HeaderBorderPainter { get { return headerBorderPainter; } }
		public virtual BorderPainter HeaderRowBorderPainter { get { return headerRowBorderPainter; } }
		public int FirstVisiblePageIndex {
			get { return IsMultiLine ? 0 : firstVisiblePageIndex; }
			set {
				int max = HeaderInfo.CalcMaxFirstVisibleIndex();
				if(value > max) value = max;
				if(IsMultiLine || value < 0) value = 0;
				if(FirstVisiblePageIndex == value) return;
				firstVisiblePageIndex = value;
				LayoutChanged();
			}
		}
		public int VisiblePagesCount {
			get {
				int res = 0;
				for(int n = 0; n < TabControl.PageCount; n++) {
					IXtraTabPage page = TabControl.GetTabPage(n);
					if(page.PageVisible || IsDesignMode) res++;
				}
				return res;
			}
		}
		public virtual bool IsAllowMultiLine { get { return true; } }
		public virtual bool AllowInactiveState { get { return false; } }
		public virtual bool IsActive { get { return false; } }
		public virtual TabHeaderLocation CheckHeaderLocation(TabHeaderLocation location) {
			return location;
		}
		public virtual BaseTabHeaderViewInfo HeaderInfo {
			get { return headerInfo; }
		}
		public virtual void Clear() {
			this.fPageClientBounds = this.fPageBounds = this.fBounds = this.fClient = Rectangle.Empty;
		}
		public virtual Rectangle PageBounds { get { return fPageBounds; } }
		public virtual Rectangle PageClientBounds { get { return fPageClientBounds; } }
		public virtual Rectangle PrevPageClientBounds { get { return fPrevPageClientBounds; } }
		public virtual Rectangle Bounds { get { return fBounds; } }
		public virtual Rectangle Client { get { return fClient; } }
		public virtual IXtraTab TabControl { get { return tabControl; } }
		bool isDisposingCore;
		public bool IsDisposing {
			get { return isDisposingCore; }
		}
		public virtual void Dispose() {
			if(!isDisposingCore) {
				isDisposingCore = true;
				HeaderInfo.Dispose();
			}
		}
		public virtual bool FillPageClient { get { return allowFillPageClientBackground; } set { allowFillPageClientBackground = value; } }
		public virtual void CalcViewInfo(Rectangle bounds, Graphics g) {
			CreatePainters();
			this.fBounds = bounds;
			GraphicsInfo.AddGraphics(g);
			try {
				this.fClient = CalcClientBounds(GraphicsInfo.Graphics, Bounds);
				UpdatePaintAppearance();
				HeaderInfo.CalcViewInfo(GraphicsInfo.Graphics, Client, TabControl.HeaderLocation);
				this.fPageBounds = CalcPageBounds();
				this.fPageClientBounds = CalcPageClientBounds();
			}
			finally {
				GraphicsInfo.ReleaseGraphics();
			}
			if(PrevPageClientBounds != PageClientBounds) {
				this.fPrevPageClientBounds = PageClientBounds;
				if(!PageClientBounds.IsEmpty)
					OnPageClientBoundsChanged();
			}
		}
		protected virtual Rectangle CalcClientBounds(Graphics g, Rectangle bounds) {
			return TabControlBorderPainter.GetObjectClientRectangle(new TabBorderObjectInfoArgs(this, null, null, bounds));
		}
		protected virtual void OnPageClientBoundsChanged() {
			if(PageClientBoundsChanged != null) PageClientBoundsChanged(this, EventArgs.Empty);
		}
		public virtual bool IsHeaderAutoFill {
			get {
				if(Properties == null || Properties.HeaderAutoFill == DefaultBoolean.Default) return HeaderInfo.GetDefaultAutoFill(IsMultiLine);
				return Properties.HeaderAutoFill == DefaultBoolean.True;
			}
		}
		public virtual bool IsMultiLine {
			get {
				if(!IsAllowMultiLine) return false;
				if(Properties == null && Properties.MultiLine == DefaultBoolean.Default) return HeaderInfo.DefaultMultiLine;
				return Properties.MultiLine == DefaultBoolean.True;
			}
		}
		public virtual bool IsShowHeader { get { return (Properties == null || Properties.ShowTabHeader != DefaultBoolean.False); } }
		protected virtual bool AllowHotTracking { get { return true && (Properties == null || Properties.AllowHotTrack != DefaultBoolean.False); } }
		protected internal virtual ObjectState OnHeaderButtonCalcState(TabButtonInfo button) {
			ObjectState res = ObjectState.Normal;
			switch(button.ButtonType) {
				case TabButtonType.Prev: if(FirstVisiblePageIndex - HeaderInfo.GetIndexLastPinnedTab() <= 0) res = ObjectState.Disabled; break;
				case TabButtonType.Next: if(FirstVisiblePageIndex >= HeaderInfo.CalcMaxFirstVisibleIndex()) res = ObjectState.Disabled; break;
				case TabButtonType.Close: break;
				case TabButtonType.User:
					if(!button.Button.Enabled)
						res = ObjectState.Disabled;
					break;
			}
			return res;
		}
		protected internal virtual void OnHeaderButtonClick(TabButtonInfo button) {
			switch(button.ButtonType) {
				case TabButtonType.Prev:
					if(!RaiseHeaderButtonClick(TabButtons.Prev))
						FirstVisiblePageIndex--;
					break;
				case TabButtonType.Next:
					if(!RaiseHeaderButtonClick(TabButtons.Next)) {
						if(FirstVisiblePageIndex < HeaderInfo.GetIndexLastPinnedTab())
							firstVisiblePageIndex = HeaderInfo.GetIndexLastPinnedTab();
						FirstVisiblePageIndex++;
					}
					break;
				case TabButtonType.Close:
					if(SelectedTabPage != null && SelectedTabPage.PageEnabled) {
						if(!RaiseHeaderButtonClick(TabButtons.Close))
							OnHeaderCloseButtonClick(new ClosePageButtonEventArgs(null, SelectedTabPage));
					}
					break;
				case TabButtonType.User:
					RaiseCustomHeaderButtonClick(button.Button as CustomHeaderButton);
					break;
			}
		}
		protected internal virtual void OnTabMiddleClick(PageEventArgs ea) {
			if(TabMiddleClick != null) TabMiddleClick(this, ea);
		}
		protected internal virtual void OnPageCloseButtonClick(ClosePageButtonEventArgs ea) {
			if(CloseButtonClick != null) CloseButtonClick(this, ea);
		}
		protected virtual void OnHeaderCloseButtonClick(ClosePageButtonEventArgs ea) {
			if(CloseButtonClick != null) CloseButtonClick(this, ea);
		}
		protected bool RaiseHeaderButtonClick(TabButtons button) {
			HeaderButtonEventArgs ea = new HeaderButtonEventArgs(button, SelectedTabPage);
			OnHeaderButtonClick(ea);
			return ea.Handled;
		}
		protected void RaiseCustomHeaderButtonClick(CustomHeaderButton button) {
			OnCustomHeaderButtonClick(new CustomHeaderButtonEventArgs(button, SelectedTabPage));
		}
		protected virtual void OnHeaderButtonClick(HeaderButtonEventArgs ea) {
			if(HeaderButtonClick != null) HeaderButtonClick(this, ea);
		}
		protected virtual void OnCustomHeaderButtonClick(CustomHeaderButtonEventArgs ea) {
			if(CustomHeaderButtonClick != null) CustomHeaderButtonClick(this, ea);
		}
		protected virtual Rectangle CalcPageBounds() {
			Rectangle r = Client;
			if(!IsShowHeader) return r;
			if(HeaderInfo.IsSideLocation) {
				r.Width -= HeaderInfo.Bounds.Width;
				if(HeaderInfo.IsLeftLocation) r.X += HeaderInfo.Bounds.Width;
			}
			else {
				r.Height -= HeaderInfo.Bounds.Height;
				if(HeaderInfo.IsTopLocation) r.Y += HeaderInfo.Bounds.Height;
			}
			return r;
		}
		protected virtual Rectangle CalcPageClientBounds() {
			return PageClientBorderPainter.GetObjectClientRectangle(new TabBorderObjectInfoArgs(this, null, null, PageBounds));
		}
		public virtual void SetSelectedTabPageCore(IXtraTabPage page) {
			if(!CanSelectPage(page)) page = null;
			IXtraTabPage prevPage = this.selectedTabPage;
			this.selectedTabPage = page;
			if(prevPage != page) OnSelectedPageChanged(prevPage);
		}
		public virtual void MakePageVisible(IXtraTabPage page) {
			if(IsMultiLine || IsHeaderAutoFill || page == null || HeaderInfo.AllPages[page] == null) return;
			HeaderInfo.MakePageVisible(page);
		}
		public virtual BaseTabPageViewInfo SelectedTabPageViewInfo {
			get {
				return HeaderInfo.AllPages[SelectedTabPage];
			}
		}
		protected virtual void SelectNextPageCore(int current, int delta, bool delayedIncrement) {
			if(current == -1 || TabControl.PageCount < 2) return;
			int dest = current;
			while(true) {
				dest = dest + (delayedIncrement ? 0 : delta);
				delayedIncrement = false;
				if(dest >= TabControl.PageCount) {
					dest = -1;
					delta = 1;
					continue;
				}
				if(dest < 0) {
					dest = TabControl.PageCount;
					delta = -1;
					continue;
				}
				if(CanSelectPage(TabControl.GetTabPage(dest))) {
					SelectedTabPageIndex = dest;
					break;
				}
				if(dest == current) break;
			}
		}
		public virtual IXtraTabPage FindMnemonic(char charCode) {
			for(int n = 0; n < TabControl.PageCount; n++) {
				IXtraTabPage page = TabControl.GetTabPage(n);
				if(!CanSelectPage(page)) continue;
				if(Control.IsMnemonic(charCode, page.Text)) return page;
			}
			return null;
		}
		public virtual void SelectNextPage(int delta) {
			SelectNextPageCore(SelectedTabPageIndex, delta, false);
		}
		public virtual void SelectFirstPage() {
			SelectNextPageCore(0, 1, true);
		}
		public virtual void SelectLastPage() {
			SelectNextPageCore(TabControl.PageCount - 1, -1, true);
		}
		public virtual int SelectedTabPageIndex {
			get {
				if(SelectedTabPage == null) return -1;
				for(int n = TabControl.PageCount - 1; n >= 0; n--) {
					IXtraTabPage page = TabControl.GetTabPage(n);
					if(page == SelectedTabPage) return n;
				}
				return -1;
			}
			set {
				int cur = SelectedTabPageIndex;
				value = Math.Min(value, TabControl.PageCount - 1);
				if(value < 0) value = 0;
				if(cur == value) return;
				if(value == -1) return;
				SelectedTabPage = TabControl.PageCount == 0 ? null : TabControl.GetTabPage(value);
			}
		}
		public virtual IXtraTabPage SelectedTabPage {
			get { return selectedTabPage; }
			set {
				if(value != null && !CanSelectPage(value)) return;
				if(SelectedTabPage == value) return;
				IXtraTabPage prev = SelectedTabPage;
				if(!OnSelectedPageChanging(prev, value) || IsDisposing) return;
				selectedTabPage = value;
				LayoutChanged();
				MakePageVisible(value);
				OnSelectedPageChanged(prev);
			}
		}
		public virtual IXtraTabPage HotTrackedTabPage {
			get { return hotTrackedTabPage; }
			set {
				if(!AllowHotTracking || !CanHotTrackPage(value)) value = null;
				if(HotTrackedTabPage == value) return;
				IXtraTabPage prev = HotTrackedTabPage;
				hotTrackedTabPage = value;
				OnHotTrackedPageChanged(prev);
			}
		}
		protected virtual void OnHotTrackedPageChanged(IXtraTabPage prevPage) {
			if(IsNeedLayoutOnHotTrack) {
				LayoutChanged();
			}
			else {
				if(HeaderInfo.UpdatePageStates()) {
					Invalidate(HeaderInfo.Bounds);
					Invalidate(PageBounds);
				}
			}
			if(HotTrackedPageChanged != null) HotTrackedPageChanged(TabControl, new ViewInfoTabPageChangedEventArgs(prevPage, HotTrackedTabPage));
		}
		protected virtual void OnCloseButtonStateChanged(IXtraTabPage page, IXtraTabPage prevPage) {
			if(prevPage != null) {
				BaseTabPageViewInfo prevViewInfo = HeaderInfo.AllPages[prevPage];
				if(prevViewInfo != null) Invalidate(prevViewInfo.ControlBox);
			}
			if(page != null) {
				BaseTabPageViewInfo pageViewInfo = HeaderInfo.AllPages[page];
				if(pageViewInfo != null) Invalidate(pageViewInfo.ControlBox);
			}
		}
		protected virtual void OnSelectedPageChanged(IXtraTabPage prevPage) {
			if(SelectedPageChanged != null) SelectedPageChanged(TabControl, new ViewInfoTabPageChangedEventArgs(prevPage, SelectedTabPage));
		}
		protected virtual bool OnSelectedPageChanging(IXtraTabPage oldPage, IXtraTabPage newPage) {
			ViewInfoTabPageChangingEventArgs e = new ViewInfoTabPageChangingEventArgs(oldPage, newPage);
			if(SelectedPageChanging != null) SelectedPageChanging(TabControl, e);
			return !e.Cancel;
		}
		protected virtual bool CanSelectPage(IXtraTabPage page) {
			return page != null && (IsDesignMode || (page.PageEnabled && page.PageVisible));
		}
		protected virtual bool CanHotTrackPage(IXtraTabPage page) {
			return page != null && page.PageEnabled && page.PageVisible;
		}
		public virtual void OnPageAdded(IXtraTabPage page) {
			if(SelectedTabPage == null)
				SelectedTabPage = page;
			else
				LayoutChanged();
			CheckFirstPageIndex();
		}
		public virtual void OnPageRemoved(IXtraTabPage page) {
			if(SelectedTabPage == page) {
				SetSelectedTabPageCore(GetSelectablePage());
			}
			LayoutChanged();
			CheckFirstPageIndex();
		}
	}
	public enum IndentType { Left, Right, Top, Bottom, BetweenImageText, CloseButton };
	public class BaseTabRowViewInfo : IDisposable {
		PageViewInfoCollection pages;
		BaseTabHeaderViewInfo header;
		Rectangle bounds, client;
		internal Size buttonsSize;
		public BaseTabRowViewInfo(BaseTabHeaderViewInfo header) {
			this.header = header;
			this.pages = new PageViewInfoCollection();
			this.client = this.bounds = Rectangle.Empty;
		}
		public virtual void Dispose() {
			Clear();
		}
		public virtual void Clear() {
			this.client = this.bounds = Rectangle.Empty;
			this.pages.Clear();
		}
		public Rectangle Bounds { get { return bounds; } set { bounds = value; } }
		public Rectangle Client { get { return client; } set { client = value; } }
		public BaseTabHeaderViewInfo Header { get { return header; } }
		protected bool UseReversedHitTest { get { return Header.UseReversedHitTest; } }
		public virtual PageViewInfoCollection Pages { get { return pages; } }
		public virtual BaseTabPageViewInfo FindPage(Point pt) {
			bool reversed = UseReversedHitTest;
			for(int n = reversed ? Pages.Count - 1 : 0; reversed ? n >= 0 : n < Pages.Count; n += reversed ? -1 : 1) {
				BaseTabPageViewInfo page = Pages[n];
				if(page.AllowDraw && page.Bounds.Contains(pt)) {
					if(page.PageRegion != null) {
						if(!page.PageRegion.IsVisible(pt)) continue;
					}
					if(page.ClipRegion != null) {
						if(page.ClipRegion.IsVisible(pt)) return page;
					}
					return page;
				}
			}
			return null;
		}
		public BaseTabPageViewInfo GetLastVisiblePageHeader() {
			if(Client.IsEmpty) return null;
			for(int n = Pages.Count - 1; n >= 0; n--) {
				BaseTabPageViewInfo page = Pages[n];
				if(!page.Bounds.IsEmpty && Client.IntersectsWith(page.Bounds)) return page;
			}
			return null;
		}
		public int GetUnusedSize() {
			if(Pages.Count == 0) return Header.GetSizeWidth(Client.Size);
			BaseTabPageViewInfo page = Pages[Pages.Count - 1];
			return Math.Max(0, Header.GetRectFar(Client) - Header.GetRectFar(page.Bounds));
		}
		internal Size GetPagesSize() {
			Size res = Size.Empty;
			foreach(BaseTabPageViewInfo page in Pages) {
				res.Width += page.Bounds.Width;
				res.Height += page.Bounds.Height;
			}
			return res;
		}
		public virtual BaseTabPageViewInfo SelectedPage {
			get {
				foreach(BaseTabPageViewInfo pInfo in Pages) {
					if((pInfo.PageState & ObjectState.Selected) != 0) return pInfo;
				}
				return null;
			}
		}
	}
	public class BaseTabRowViewInfoCollection : CollectionBase, System.Collections.Generic.IEnumerable<BaseTabRowViewInfo> {
		public virtual BaseTabRowViewInfo LastRow { get { return Count == 0 ? null : this[Count - 1]; } }
		public virtual BaseTabRowViewInfo this[int index] { get { return List[index] as BaseTabRowViewInfo; } }
		public virtual void Add(BaseTabRowViewInfo rowInfo) {
			List.Add(rowInfo);
		}
		public virtual void MakeSelectedRowLast() {
			int selIndex = -1;
			for(int n = Count - 1; n >= 0; n--) {
				if(this[n].SelectedPage != null) {
					selIndex = n;
					break;
				}
			}
			if(selIndex == Count - 1 || selIndex < 0) return;
			InnerList.Add(this[selIndex]);
			InnerList.RemoveAt(selIndex);
		}
		public int IndexOf(BaseTabRowViewInfo rowInfo) {
			if(!List.Contains(rowInfo)) return -1;
			return List.IndexOf(rowInfo);
		}
		System.Collections.Generic.IEnumerator<BaseTabRowViewInfo> System.Collections.Generic.IEnumerable<BaseTabRowViewInfo>.GetEnumerator() {
			foreach(BaseTabRowViewInfo rowInfo in List)
				yield return rowInfo;
		}
	}
	public class BaseClosePageButtonHelper {
		ObjectPainter closeButtonPainter = null;
		EditorButtonObjectInfoArgs closeButtonInfoArgs = null;
		BaseTabControlViewInfo viewInfoCore = null;
		public BaseClosePageButtonHelper(BaseTabControlViewInfo viewInfo) {
			this.viewInfoCore = viewInfo;
			this.closeButtonPainter = CreateButtonPainter();
			this.closeButtonInfoArgs = CreateButtonInfoArgs();
		}
		public BaseTabControlViewInfo ViewInfo { get { return viewInfoCore; } }
		public ObjectPainter ButtonPainter { get { return closeButtonPainter; } }
		public EditorButtonObjectInfoArgs ButtonInfoArgs { get { return closeButtonInfoArgs; } }
		protected virtual ObjectPainter CreateButtonPainter() {
			return ViewInfo.TabControl.View.CreateClosePageButtonPainter(ViewInfo.TabControl);
		}
		protected virtual EditorButtonObjectInfoArgs CreateButtonInfoArgs() {
			return new EditorButtonObjectInfoArgs(new EditorButton(ButtonPredefines.Close), null);
		}
	}
	public class BaseTabHeaderViewInfo : IDisposable {
		Rectangle bounds, client;
		BaseTabRowViewInfoCollection rows;
		PageViewInfoCollection allPages, visiblePages;
		BaseTabControlViewInfo viewInfo;
		TabHeaderLocation headerLocation;
		TabOrientation realPageOrientation;
		Region headerRegion;
		GraphicsInfo graphicsInfo;
		AppearanceObject paintAppearance;
		AppearanceObject paintAppearanceCustomHeaderButton;
		AppearanceObject paintAppearanceCustomHeaderButtonHot;
		TabButtonsPanel buttons;
		bool fillTransparentBackground;
		public BaseTabHeaderViewInfo(BaseTabControlViewInfo viewInfo) {
			this.fillTransparentBackground = false;
			this.paintAppearance = new FrozenAppearance();
			this.paintAppearanceCustomHeaderButton = new FrozenAppearance();
			this.paintAppearanceCustomHeaderButtonHot = new FrozenAppearance();
			this.graphicsInfo = new GraphicsInfo();
			this.rows = new BaseTabRowViewInfoCollection();
			this.allPages = new PageViewInfoCollection();
			this.visiblePages = new PageViewInfoCollection();
			this.viewInfo = viewInfo;
			this.headerLocation = TabHeaderLocation.Top;
			this.buttons = CreateHeaderButtons();
			Clear();
		}
		public bool FillTransparentBackground { get { return fillTransparentBackground; } set { fillTransparentBackground = value; } }
		public virtual bool UseReversePainter { get { return false; } }
		public virtual bool DrawSelectedPageLast { get { return false; } }
		protected virtual GraphicsInfo GraphicsInfo { get { return graphicsInfo; } }
		public virtual void Dispose() {
			this.pageStateStorageCore.Clear();
			this.buttons.Dispose();
			Clear();
		}
		protected internal virtual bool UseReversedHitTest { get { return false; } }
		protected internal virtual EditorButtonPainter OnHeaderButtonGetPainter(TabButtonInfo button) {
			return EditorButtonHelper.GetPainter(BorderStyles.Style3D);
		}
		protected internal virtual ToolTipControlInfo GetToolTipInfo(Point point) {
			BaseTabHitInfo hit = ViewInfo.CalcHitInfo(point);
			if(hit.Page != null) {
				string toolTipStr = hit.Page.Tooltip;
				object hitObj = hit.Page;
				if(hit.InPageControlBox) {
					hitObj = hit.Page.GetHashCode(); 
					BaseTabPageViewInfo pInfo = ViewInfo.HeaderInfo.FindPage(point);
					return pInfo.ButtonsPanel.GetObjectInfo(point);
				}
				if(!string.IsNullOrEmpty(toolTipStr) || (hit.Page.SuperTip != null && !hit.Page.SuperTip.IsEmpty)) {
					ToolTipControlInfo info = new ToolTipControlInfo(hitObj, toolTipStr, hit.Page.TooltipTitle, hit.Page.TooltipIconType);
					info.SuperTip = hit.Page.SuperTip;
					return info;
				}
			}
			return Buttons.GetToolTipInfo(point);
		}
		protected virtual TabButtonsPanel CreateHeaderButtons() {
			return new TabButtonsPanel(ViewInfo);
		}
		protected internal virtual int CalcResizeFirstVisibleIndex() {
			int res = ViewInfo.FirstVisiblePageIndex;
			if(Rows.LastRow == null) return res;
			int unused = Rows.LastRow.GetUnusedSize() - GetSizeWidth(ButtonsBounds.Size);
			if(unused < 1) return res;
			res = res > AllPages.Count ? AllPages.Count : res;
			for(int n = res - 1; n >= 0; n--) {
				BaseTabPageViewInfo page = AllPages[n];
				unused -= GetSizeWidth(page.Bounds.Size);
				if(unused > 0) res = n;
			}
			return res;
		}
		protected internal virtual int CalcMaxFirstVisibleIndex() {
			if(ViewInfo.IsMultiLine) return 0;
			if(ViewInfo.IsHeaderAutoFill) return 0;
			BaseTabPageViewInfo info = VisiblePages.Count > 0 ? VisiblePages[VisiblePages.Count - 1] : null;
			if(info == null) return 0;
			if(IsPageFullyVisible(info.Page) && ViewInfo.FirstVisiblePageIndex + VisiblePages.Count >= AllPages.Count) return ViewInfo.FirstVisiblePageIndex;
			return ViewInfo.VisiblePagesCount - 1;
		}
		public virtual bool GetDefaultAutoFill(bool isMultiLine) {
			return isMultiLine;
		}
		public virtual bool DefaultMultiLine { get { return false; } }
		public virtual bool DefaultShowHeaderFocus { get { return true; } }
		public virtual TabButtons DefaultHeaderButtons { get { return TabButtons.Next | TabButtons.Prev; } }
		public virtual TabButtons GetHeaderButtons() {
			TabButtons res = ViewInfo.HeaderButtons;
			if((res & TabButtons.Default) != 0) res = DefaultHeaderButtons | (res & (~TabButtons.Default));
			if(ViewInfo.IsMultiLine || ViewInfo.IsHeaderAutoFill) res &= ~(TabButtons.Next | TabButtons.Prev);
			return res;
		}
		protected virtual bool CanShowButtons {
			get {
				if(ViewInfo.VisiblePagesCount < 1) return false;
				return true;
			}
		}
		protected virtual bool VisiblePagesExists {
			get { return ViewInfo.VisiblePagesCount > 0; }
		}
		protected virtual DefaultBoolean DefaultShowCloseButtonForInactivePage { get { return DefaultBoolean.False; } }
		protected virtual DefaultBoolean DefaultDisableDrawCloseButtonForInactivePage { get { return DefaultBoolean.False; } }
		protected virtual DefaultBoolean DefaultShowClosePageButton { get { return DefaultBoolean.False; } }
		protected virtual DefaultBoolean DefaultAllowClosePageButton { get { return DefaultBoolean.True; } }
		protected virtual DefaultBoolean DefaultShowPinButtonForInactivePage { get { return DefaultBoolean.False; } }
		protected virtual DefaultBoolean DefaultDisableDrawPinButtonForInactivePage { get { return DefaultBoolean.False; } }
		protected virtual DefaultBoolean DefaultShowPinPageButton { get { return DefaultBoolean.False; } }
		protected virtual DefaultBoolean DefaultAllowPinPageButton { get { return DefaultBoolean.True; } }
		protected virtual DefaultBoolean DefaultAllowGlyphSkinning { get { return DefaultBoolean.False; } }
		protected virtual DefaultBoolean DefaultAllowGlyphSkinningForPage { get { return DefaultBoolean.False; } }
		protected internal virtual bool GetAllowGlyphSkinning() {
			DefaultBoolean allow = ViewInfo.AllowGlyphSkinning;
			if(allow == DefaultBoolean.Default) allow = DefaultAllowGlyphSkinning;
			return (allow == DefaultBoolean.True);
		}
		protected internal virtual bool GetAllowGlyphSkinning(BaseTabPageViewInfo info) {
			DefaultBoolean allow = info.Page.AllowGlyphSkinning;
			if(allow == DefaultBoolean.Default) allow = DefaultAllowGlyphSkinningForPage;
			return allow == DefaultBoolean.True;
		}
		protected internal virtual bool CanShowPageCloseButtons() {
			DefaultBoolean show = ViewInfo.ShowClosePageButton;
			if(show == DefaultBoolean.Default) show = DefaultShowClosePageButton;
			return VisiblePagesExists && (show == DefaultBoolean.True);
		}
		protected internal virtual bool CanShowPagePinButtons() {
			DefaultBoolean show = ViewInfo.ShowPinPageButton;
			if(show == DefaultBoolean.Default) show = DefaultShowPinPageButton;
			return VisiblePagesExists && (show == DefaultBoolean.True);
		}
		protected internal virtual bool CanShowCloseButtonForPage(BaseTabPageViewInfo info) {
			DefaultBoolean show = ViewInfo.ShowCloseButtonForInactivePages;
			if(show == DefaultBoolean.Default) show = DefaultShowCloseButtonForInactivePage;
			return AllowClosePageButton(info) && (info.IsActiveState || (show == DefaultBoolean.True));
		}
		protected internal virtual bool CanShowPinButtonForPage(BaseTabPageViewInfo info) {
			DefaultBoolean show = ViewInfo.ShowPinButtonForInactivePages;
			if(show == DefaultBoolean.Default) show = DefaultShowPinButtonForInactivePage;
			return AllowPinPageButton(info) && (info.IsActiveState || (show == DefaultBoolean.True));
		}
		protected virtual bool CanDisableDrawCloseButtonForInactivePage(BaseTabPageViewInfo info) {
			DefaultBoolean disable = ViewInfo.DisableDrawCloseButtonForInactivePages;
			if(disable == DefaultBoolean.Default) disable = DefaultDisableDrawCloseButtonForInactivePage;
			return !info.IsActiveState && (disable == DefaultBoolean.True);
		}
		protected virtual bool CanDisableDrawPinButtonForInactivePage(BaseTabPageViewInfo info) {
			DefaultBoolean disable = ViewInfo.DisableDrawPinButtonForInactivePages;
			if(disable == DefaultBoolean.Default) disable = DefaultDisableDrawPinButtonForInactivePage;
			return !info.IsActiveState && (disable == DefaultBoolean.True);
		}
		protected internal virtual bool AllowClosePageButton(BaseTabPageViewInfo info) {
			DefaultBoolean show = info.Page.ShowCloseButton;
			if(show == DefaultBoolean.Default) show = DefaultAllowClosePageButton;
			return show == DefaultBoolean.True;
		}
		protected internal virtual bool AllowPinPageButton(BaseTabPageViewInfo info) {
			DefaultBoolean show = DefaultBoolean.Default;
			if(info.Page is IXtraTabPageExt)
				show = ((IXtraTabPageExt)info.Page).ShowPinButton;
			if(show == DefaultBoolean.Default) show = DefaultAllowClosePageButton;
			return show == DefaultBoolean.True;
		}
		protected virtual TabButtonShowMode DefaultButtonShowMode { get { return TabButtonShowMode.WhenNeeded; } }
		protected virtual bool CalcCanShowCustomHeaderButtons() {
			return ViewInfo.CustomHeaderButtons.Count > 0;
		}
		protected virtual TabButtons CalcCanShowButtons() {
			TabButtons res = TabButtons.None;
			if(!CanShowButtons) return res;
			TabButtonShowMode showMode = ViewInfo.HeaderButtonsShowMode;
			if(showMode == TabButtonShowMode.Default) showMode = DefaultButtonShowMode;
			switch(showMode) {
				case TabButtonShowMode.Never: return res;
				case TabButtonShowMode.Always: return TabButtons.Next | TabButtons.Prev | TabButtons.Close;
			}
			if((ViewInfo.HeaderButtons & TabButtons.Close) != 0) res |= TabButtons.Close;
			BaseTabRowViewInfo row = Rows.Count == 1 ? Rows[0] : null;
			if(row == null) return res;
			if(ViewInfo.FirstVisiblePageIndex > 0) {
				res |= TabButtons.Prev | TabButtons.Next;
				return res;
			}
			Size size = CalcRowPagesSize(row);
			Size buttonSize = CalcButtonsSize(res);
			int width = GetRectSize(new Rectangle(Point.Empty, size)) + (IsSideLocation ? buttonSize.Height : buttonSize.Width); 
			if(width <= GetRectSize(row.Client)) return res;
			res |= TabButtons.Prev | TabButtons.Next;
			return res;
		}
		public virtual TabButtonsPanel Buttons { get { return buttons; } }
		public virtual BaseTabRowViewInfoCollection Rows { get { return rows; } }
		public virtual PageViewInfoCollection AllPages { get { return allPages; } }
		public virtual PageViewInfoCollection VisiblePages { get { return visiblePages; } }
		public virtual Rectangle Bounds { get { return bounds; } }
		public virtual Rectangle Client { get { return client; } }
		public virtual Rectangle ButtonsBounds { get { return Buttons.Bounds; } }
		public virtual Region HeaderRegion {
			get { return headerRegion; }
			set {
				if(HeaderRegion == value) return;
				if(headerRegion != null) headerRegion.Dispose();
				headerRegion = value;
			}
		}
		public virtual void MakePageVisible(IXtraTabPage page) {
			BaseTabPageViewInfo info = VisiblePages[page];
			if(info == null) {
				ViewInfo.FirstVisiblePageIndex = AllPages.IndexOf(page);
				return;
			}
			else {
				if(Client.IsEmpty) return;
				MakePageVisibleCore(info);
			}
		}
		protected virtual void MakePageVisibleCore(BaseTabPageViewInfo info) {
			if(GetRectNear(info.Bounds) < GetRectNear(Client) ||
				GetRectFar(info.Bounds) < GetRectNear(Client)) {
				ViewInfo.FirstVisiblePageIndex = AllPages.IndexOf(info);
				return;
			}
			IXtraTabPage page = info.Page;
			while(true) {
				info = VisiblePages[page];
				if(ViewInfo.FirstVisiblePageIndex >= AllPages.IndexOf(info)) break;
				if(IsPageFullyVisible(page)) break;
				int prev = ViewInfo.FirstVisiblePageIndex;
				if(prev == ++ViewInfo.FirstVisiblePageIndex) break;
			}
		}
		protected virtual bool IsPageFullyVisible(IXtraTabPage page) {
			int far = ButtonsBounds.IsEmpty ? GetRectFar(Client) : GetRectNear(ButtonsBounds);
			int near = GetRectNear(Client);
			BaseTabPageViewInfo info = VisiblePages[page];
			if(info != null && info.AllowDraw) {
				if(IsRightToLeftLocation) {
					near = ButtonsBounds.IsEmpty ? GetRectNear(Client) : GetRectFar(ButtonsBounds);
					far = GetRectFar(Client);
				}
				return (GetRectNear(info.Bounds) >= near && GetRectFar(info.Bounds) < far);
			}
			return false;
		}
		internal Size SetSizeWidth(Size size, int width) { return SetSizeWidth(size, width, !IsSideLocation); }
		internal Size SetSizeHeight(Size size, int height) { return SetSizeWidth(size, height, IsSideLocation); }
		Size SetSizeWidth(Size size, int width, bool horz) { return new Size(horz ? width : size.Width, horz ? size.Height : width); }
		Point SetPointStart(Point p, int start) { return SetPointStart(p, start, !IsSideLocation); }
		Point SetPointStart(Point p, int start, bool horz) { return new Point(horz ? start : p.X, horz ? p.Y : start); }
		int GetRectNear(Rectangle rect) { return GetRectNear(rect, !IsSideLocation); }
		internal int GetRectFar(Rectangle rect) { return GetRectFar(rect, !IsSideLocation); }
		int GetRectSize(Rectangle rect) { return GetRectSize(rect, !IsSideLocation); }
		internal int GetSizeWidth(Size size) { return GetRectSize(new Rectangle(Point.Empty, size), !IsSideLocation); }
		internal int GetSizeHeight(Size size) { return GetRectSize(new Rectangle(Point.Empty, size), IsSideLocation); }
		int GetRectNear(Rectangle rect, bool horz) { return horz ? rect.X : rect.Y; }
		int GetRectFar(Rectangle rect, bool horz) { return horz ? rect.Right : rect.Bottom; }
		int GetRectSize(Rectangle rect, bool horz) { return horz ? rect.Width : rect.Height; }
		public virtual void Clear() {
			this.client = this.bounds = Rectangle.Empty;
			this.realPageOrientation = TabOrientation.Horizontal;
			HeaderRegion = null;
			AllPages.Clear();
			VisiblePages.Clear();
			Rows.Clear();
			ResetCache();
		}
		public virtual BaseTabPageViewInfo FindPage(Point pt) {
			BaseTabPageViewInfo selPage = ViewInfo.SelectedTabPageViewInfo;
			if(VisiblePages.IndexOf(selPage) == -1 || (selPage != null && !selPage.AllowDraw)) selPage = null;
			if(selPage != null && selPage.Bounds.Contains(pt) && (selPage.PageRegion == null || selPage.PageRegion.IsVisible(pt))) return selPage;
			for(int i = Rows.Count - 1; i >= 0; i--) {
				BaseTabPageViewInfo page = Rows[i].FindPage(pt);
				if(page != null) return page;
			}
			return null;
		}
		public bool IsBottomLocation { get { return HeaderLocation == TabHeaderLocation.Bottom; } }
		public bool IsRightLocation { get { return HeaderLocation == TabHeaderLocation.Right; } }
		public bool IsLeftLocation { get { return HeaderLocation == TabHeaderLocation.Left; } }
		public bool IsTopLocation { get { return HeaderLocation == TabHeaderLocation.Top; } }
		public virtual TabHeaderLocation HeaderLocation { get { return headerLocation; } }
		public virtual TabOrientation RealPageOrientation { get { return realPageOrientation; } }
		public virtual BaseTabControlViewInfo ViewInfo { get { return viewInfo; } }
		public virtual IXtraTabProperties Properties { get { return ViewInfo.Properties; } }
		public virtual IXtraTab TabControl { get { return ViewInfo.TabControl; } }
		public virtual bool IsSideLocation {
			get {
				return (HeaderLocation == TabHeaderLocation.Left || HeaderLocation == TabHeaderLocation.Right);
			}
		}
		protected virtual TabOrientation CalcOrientation() {
			TabOrientation res = TabControl.HeaderOrientation;
			if(res == TabOrientation.Default)
				res = DefaultOrientation;
			return res;
		}
		protected internal bool IsStandardOrientation { get { return RealPageOrientation == DefaultOrientation; } }
		protected virtual TabOrientation DefaultOrientation {
			get { return IsSideLocation ? TabOrientation.Vertical : TabOrientation.Horizontal; ; }
		}
		protected virtual TabHeaderLocation CalcHeaderLocation(TabHeaderLocation hLocation) {
			return hLocation;
		}
		public virtual void CalcViewInfo(Graphics g, Rectangle r, TabHeaderLocation hLocation) {
			GraphicsInfo.AddGraphics(g);
			try {
				ResetCache();
				allowResetCache = false;
				this.headerLocation = CalcHeaderLocation(hLocation);
				this.realPageOrientation = CalcOrientation();
				foreach(BaseTabPageViewInfo info in AllPages) {
					if((info.Page.TabControl != null) && info.Page.PageVisible)
						info.SaveState(GetPageStateStorage(info.Page));
					else
						ResetPageStateStorage(info.Page);
					info.Dispose();
				}
				this.AllPages.Clear();
				this.VisiblePages.Clear();
				this.Rows.Clear();
				CreatePages();
				UpdatePageStates();
				UpdatePaintAppearance();
				if(!ViewInfo.IsShowHeader) return;
				CalcRowsViewInfo(r);
				CalcButtons(r);
			}
			finally {
				GraphicsInfo.ReleaseGraphics();
				allowResetCache = true;
			}
		}
		protected virtual Size CalcButtonsSize() {
			return CalcButtonsSize(CalcCanShowButtons());
		}
		protected virtual Size CalcButtonsSize(TabButtons btns) {
			return (btns != TabButtons.None || CalcCanShowCustomHeaderButtons()) ?
				Buttons.CalcSize(GraphicsInfo.Graphics) : Size.Empty;
		}
		protected virtual int GetButtonsAdditionalOffsetInMultiRow() { return 0; }
		protected internal bool IsRightToLeftLocation { get { return IsRightToLeftLayout && (IsTopLocation || IsBottomLocation); } }
		protected virtual void CalcButtons(Rectangle bounds) {
			TabButtons btns = CalcCanShowButtons();
			Size oldButtonsSize = CalcButtonsSize(btns);
			Buttons.CreateButtons(btns, ViewInfo.CustomHeaderButtons);
			Size buttonsSize = Size.Empty;
			BaseTabRowViewInfo lastRow = Rows.Count > 0 ? Rows[Rows.Count - 1] : null;
			if(lastRow != null) buttonsSize = CalcButtonsSize();
			Rectangle rect = Rectangle.Empty, client;
			if(!buttonsSize.IsEmpty) {
				client = rect = lastRow.Bounds;
				if(Rows.Count > 1) {
					int offset = GetSizeHeight(buttonsSize) - GetSizeHeight(lastRow.Bounds.Size);
					if(offset > 0) {
						offset += GetButtonsAdditionalOffsetInMultiRow();
						if(IsSideLocation)
							client.Width = buttonsSize.Width;
						else client.Height = buttonsSize.Height;
						if(IsTopLocation)
							client.Y -= offset;
						if(IsLeftLocation)
							client.X -= offset;
						rect = client;
					}
				}
				rect.Size = SetSizeWidth(rect.Size, GetSizeWidth(buttonsSize));
				if(!IsRightToLeftLocation)
					rect.Location = SetPointStart(rect.Location, GetButtonsRectFar(client) - GetRectSize(rect));
			}
			if(Rows.Count == 0) {
				buttonsSize = CalcButtonsSize();
				rect = GetButtonBounds(bounds); ;
				rect.Size = SetSizeWidth(rect.Size, GetSizeWidth(buttonsSize));
				rect.Height = buttonsSize.Height;
				if(!IsRightToLeftLocation)
					rect.Location = SetPointStart(rect.Location, GetButtonsRectFar(bounds) - GetRectSize(rect));
			}
			Buttons.Bounds = rect;
			Buttons.CalcViewInfo(GraphicsInfo.Graphics);
			if(!rect.IsEmpty) ButtonsUpdateRowPages();
			if(oldButtonsSize != buttonsSize || AreRowsButtonsSizeDifferent(buttonsSize)) {
				int rowCount = Rows.Count;
				this.Rows.Clear();
				CalcRowsViewInfo(bounds, buttonsSize);
				lastRow = Rows.Count > 0 ? Rows[Rows.Count - 1] : null;
				bool lastRowGrown = (lastRow != null) && (IsSideLocation ?
						(lastRow.Bounds.Width != rect.Width) :
						(lastRow.Bounds.Height != rect.Height));
				if(rowCount != Rows.Count || lastRowGrown)
					CalcButtons(bounds);
			}
		}
		protected virtual bool AreRowsButtonsSizeDifferent(Size buttonsSize) {
			return System.Linq.Enumerable.Any(Rows, r => r.buttonsSize != buttonsSize);
		}
		protected virtual void CalcButtons(BaseTabPageViewInfo info, Rectangle bounds) {
			if(info.ButtonsPanel != null) {
				ConvertHeaderLocation(info);
				info.ButtonsPanel.ViewInfo.SetDirty();
				info.ButtonsPanel.ViewInfo.Calc(GraphicsInfo.Graphics, bounds);
			}
		}
		void ConvertHeaderLocation(BaseTabPageViewInfo info) {
			info.ButtonsPanel.BeginUpdate();
			if(RealPageOrientation != TabOrientation.Vertical) {
				info.ButtonsPanel.Orientation = Orientation.Horizontal;
				if(HeaderLocation == TabHeaderLocation.Left)
					info.ButtonsPanel.ContentAlignment = ContentAlignment.TopLeft;
				else
					info.ButtonsPanel.ContentAlignment = ContentAlignment.MiddleRight;
			}
			else {
				if(HeaderLocation == TabHeaderLocation.Right || HeaderLocation == TabHeaderLocation.Bottom)
					info.ButtonsPanel.ContentAlignment = ContentAlignment.BottomCenter;
				else
					info.ButtonsPanel.ContentAlignment = ContentAlignment.TopLeft;
				info.ButtonsPanel.Orientation = Orientation.Vertical;
			}
			info.ButtonsPanel.CancelUpdate();
		}
		bool IsRightToLeftLayout { get { return TabControl != null && TabControl.RightToLeftLayout; } }
		Rectangle GetButtonBounds(Rectangle rect) {
			Rectangle result;
			Size buttonsSize = CalcButtonsSize();
			Rectangle r = rect;
			if(bounds == ButtonsBounds) return bounds;
			if(IsBottomLocation) {
				if(IsRightToLeftLayout)
					result = new Rectangle(r.X, r.Y + r.Height - buttonsSize.Height, bounds.Width, buttonsSize.Height);
				else
					result = new Rectangle(r.X + r.Width - buttonsSize.Width, r.Y + r.Height - buttonsSize.Height, bounds.Width, buttonsSize.Height);
			}
			else if(IsRightLocation)
				result = new Rectangle(r.X + r.Width - buttonsSize.Width, r.Y + r.Height - buttonsSize.Height, buttonsSize.Width, bounds.Height);
			else if(IsTopLocation) {
				if(IsRightToLeftLayout)
					result = new Rectangle(r.X, r.Y, bounds.Width, buttonsSize.Height);
				else
					result = new Rectangle(r.X + r.Width - buttonsSize.Width, r.Y, bounds.Width, buttonsSize.Height);
			}
			else
				result = new Rectangle(r.X, r.Y + r.Height - buttonsSize.Height, buttonsSize.Width, bounds.Height);
			return CalcRowBoundsByClient(null, result);
		}
		protected int GetButtonsRectFar(Rectangle client) {
			return GetRectFar(client) - GetButtonsIndent();
		}
		protected internal virtual int GetButtonsIndent() {
			return 0;
		}
		protected virtual void ButtonsUpdateRowPages() {
		}
		public virtual void UpdatePaintAppearance() {
			AppearanceHelper.Combine(PaintAppearance,
				new AppearanceObject[] { Properties.AppearancePage.Header }, ViewInfo.GetDefaultAppearance(TabPageAppearance.PageHeader));
			AppearanceHelper.Combine(paintAppearanceCustomHeaderButton,
				new AppearanceObject[] { }, ViewInfo.GetDefaultAppearance(TabPageAppearance.TabHeaderButton));
			AppearanceHelper.Combine(paintAppearanceCustomHeaderButtonHot,
				new AppearanceObject[] { }, ViewInfo.GetDefaultAppearance(TabPageAppearance.TabHeaderButtonHot));
		}
		public AppearanceObject PaintAppearance {
			get { return paintAppearance; }
		}
		public AppearanceObject PaintAppearanceCustomHeaderButton {
			get { return paintAppearanceCustomHeaderButton; }
		}
		public AppearanceObject PaintAppearanceCustomHeaderButtonHot {
			get { return paintAppearanceCustomHeaderButtonHot; }
		}
		public virtual AppearanceObject HeaderButtonPaintAppearance {
			get { return PaintAppearance; }
		}
		public virtual AppearanceObject CustomHeaderButtonPaintAppearance {
			get { return PaintAppearanceCustomHeaderButton; }
		}
		public virtual AppearanceObject CustomHeaderButtonPaintAppearanceHot {
			get { return PaintAppearanceCustomHeaderButtonHot; }
		}
		protected virtual int BorderToTabHeadersIndent {
			get { return 2; }
		}
		protected virtual Point CalcStartPoint(Rectangle p) {
			int borderToTabHeadersIndent = BorderToTabHeadersIndent;
			if(IsSideLocation) {
				p.Y += borderToTabHeadersIndent;
				if(HeaderLocation == TabHeaderLocation.Left)
					p.X += borderToTabHeadersIndent;
			}
			else {
				if(IsRightToLeftLocation) {
					p.X = p.Right - borderToTabHeadersIndent;
				} else
					p.X += borderToTabHeadersIndent;
				if(HeaderLocation == TabHeaderLocation.Top)
					p.Y += borderToTabHeadersIndent;
			}
			return p.Location;
		}
		protected Point CalcStartPoint(Point p) {
			int borderToTabHeadersIndent =  BorderToTabHeadersIndent;
			if(IsSideLocation) {
				p.Y += borderToTabHeadersIndent;
				if(HeaderLocation == TabHeaderLocation.Left)
					p.X += borderToTabHeadersIndent;
			}
			else {
				p.X += borderToTabHeadersIndent;
				if(HeaderLocation == TabHeaderLocation.Top)
					p.Y += borderToTabHeadersIndent;
			}
			return p;
		}
		protected virtual Size UpdateHeaderBoundsSize(Size size) {
			if(IsSideLocation) {
				size.Width += 2;
			}
			else {
				size.Height += 2;
			}
			return size;
		}
		protected virtual void CalcRowsViewInfo(Rectangle rect) {
			CalcRowsViewInfo(rect, CalcButtonsSize());
		}
		protected virtual void CalcRowsViewInfo(Rectangle rect, Size buttonsSize) {
			Rectangle hClient = ViewInfo.HeaderBorderPainter.GetObjectClientRectangle(new TabBorderObjectInfoArgs(ViewInfo, null, null, rect)), r;
			CalcPagesSize();
			CalcUpdatePagesSize(AllPages);
			CalcPagesSize();
			BaseTabRowViewInfo rowInfo = null;
			r = CalcRowClientBounds(null, hClient);
			if(IsSideLocation)
				r.Height -= (CalcMaxPageExtent() + buttonsSize.Height);
			else {
				r.Width -= (CalcMaxPageExtent() + buttonsSize.Width);
				if(IsRightToLeftLocation) {
					r.X += buttonsSize.Width;
				}
			}
			Point startPoint = CalcStartPoint(r);
			for(int n = 0; n < VisiblePages.Count; n++) {
				BaseTabPageViewInfo info = VisiblePages[n];
				CalcPageViewInfo(rowInfo, info, ref startPoint);
				if(rowInfo != null && rowInfo.Pages.Count > 0 && ViewInfo.IsMultiLine && (!r.Contains(startPoint) || !r.Contains(new Point(info.Bounds.Right - 1, info.Bounds.Bottom - 1)))) {
					rowInfo = null;
					startPoint = CalcStartPoint(r);
					n--;
					continue;
				}
				if(rowInfo == null) {
					rowInfo = new BaseTabRowViewInfo(this);
					Rows.Add(rowInfo);
				}
				info.Row = rowInfo;
				rowInfo.Pages.Add(info);
			}
			if(ViewInfo.IsHeaderAutoFill) {
				Rearrange(r.Size);
				Rearrange(r.Size);
			}
			CalcRows(hClient, buttonsSize);
		}
		protected virtual void Rearrange(Size rowsSize) {
			if(Rows.Count < 2) return;
			int pageCount = VisiblePages.Count;
			if(pageCount < 3) return;
			int pagePerRow = Math.Max(2, pageCount / Rows.Count);
			if(pagePerRow > 1) {
				int mod = pageCount % Rows.Count;
				if(mod > 1 && pagePerRow / mod > 1) {
					pagePerRow++;
				}
			}
			ArrayList list = new ArrayList();
			int rowsCount = Rows.Count;
			for(int n = 0; n < Rows.Count; n++) {
				BaseTabRowViewInfo row = Rows[n];
				while(list.Count > 0) {
					BaseTabPageViewInfo info = list[list.Count - 1] as BaseTabPageViewInfo;
					info.Row = row;
					row.Pages.InternalInsert(0, info);
					list.RemoveAt(list.Count - 1);
				}
				if(n == Rows.Count - 1) {
					if(row.Pages.Count - pagePerRow < 2) {
						if(row.Pages.Count == 1) continue;
						if(IsAllPagesFit(row, rowsSize)) continue;
					}
					Rows.Add(new BaseTabRowViewInfo(this));
				}
				while((!IsAllPagesFit(row, rowsSize) || row.Pages.Count > pagePerRow) && row.Pages.Count > 1) {
					list.Insert(0, row.Pages[row.Pages.Count - 1]);
					row.Pages.InternalRemoveAt(row.Pages.Count - 1);
				}
			}
		}
		bool IsAllPagesFit(BaseTabRowViewInfo row, Size rowsMaxSize) {
			Size rowSize = row.GetPagesSize();
			if(IsSideLocation) return rowSize.Height <= rowsMaxSize.Height;
			return rowSize.Width <= rowsMaxSize.Width;
		}
		protected virtual void CalcRows(Rectangle rect, Size buttonsSize) {
			if(Rows.Count > 1) {
				Rows.MakeSelectedRowLast();
			}
			Rectangle r = rect;
			for(int n = 0; n < Rows.Count; n++) {
				BaseTabRowViewInfo row = Rows[n];
				row.buttonsSize = buttonsSize;
				CalcPagesViewInfo(row, r, buttonsSize);
				r = CalcNextRowMaxRect(row, r);
			}
			UpdatePagesBounds();
			UpdateClipBounds();
			CalcHeaderBounds(rect);
			if(ViewInfo.IsMultiLine && ViewInfo.IsShowHeader) {
				if(UpdateFillPageBounds())
					UpdateClipBounds();
			}
		}
		protected virtual bool UpdateFillPageBounds() {
			bool changed = false;
			for(int n = 0; n < Rows.Count; n++) {
				BaseTabRowViewInfo row = Rows[n];
				int headerBounds = 0;
				if(IsSideLocation) {
					headerBounds = IsLeftLocation ? Bounds.Right : Bounds.X;
				}
				else {
					headerBounds = IsTopLocation ? Bounds.Bottom : Bounds.Top;
				}
				foreach(BaseTabPageViewInfo page in row.Pages) {
					changed |= UpdateFillPageBounds(row, page, headerBounds);
				}
			}
			return changed;
		}
		protected virtual bool UpdateFillPageBounds(BaseTabRowViewInfo row, BaseTabPageViewInfo page, int headerBounds) { return false; }
		protected virtual void CalcHeaderBounds(Rectangle hMaxClient) {
			Rectangle r = hMaxClient;
			if(Rows.Count > 0) {
				BaseTabRowViewInfo first = Rows[0], last = Rows[Rows.Count - 1];
				if(IsSideLocation) {
					if(HeaderLocation == TabHeaderLocation.Right) {
						r.X = last.Bounds.X;
						r.Width = hMaxClient.Right - r.X;
					}
					else {
						r.Width = last.Bounds.Right - r.X;
					}
				}
				else {
					if(HeaderLocation == TabHeaderLocation.Bottom) {
						r.Y = last.Bounds.Y;
						r.Height = hMaxClient.Bottom - r.Y;
					}
					else {
						r.Height = last.Bounds.Bottom - r.Y;
					}
				}
			}
			else {
				if(ViewInfo.DrawPaneWhenEmpty) {
					this.client = Rectangle.Empty;
					this.bounds = Rectangle.Empty;
				}
				else {
					this.client = hMaxClient;
					this.bounds = hMaxClient;
				}
				Size size = CalcButtonsSize();
				if(size != Size.Empty) {
					this.client = new Rectangle(GetButtonBounds(hMaxClient).Location, size);
					this.bounds = ViewInfo.HeaderBorderPainter.CalcBoundsByClientRectangle(new TabBorderObjectInfoArgs(ViewInfo, null, null, this.client));
				}
				return;
			}
			this.client = r;
			this.bounds = ViewInfo.HeaderBorderPainter.CalcBoundsByClientRectangle(new TabBorderObjectInfoArgs(ViewInfo, null, null, r)); ;
		}
		protected virtual Rectangle CalcNextRowMaxRect(BaseTabRowViewInfo row, Rectangle headerClient) {
			Rectangle r = headerClient;
			if(IsSideLocation) {
				if(HeaderLocation == TabHeaderLocation.Right) {
					r.Width = row.Bounds.X - r.X;
				}
				else {
					r.X = row.Bounds.Right;
					r.Width = headerClient.Right - r.X;
				}
			}
			else {
				if(HeaderLocation == TabHeaderLocation.Bottom) {
					r.Height = row.Bounds.Y - r.Y;
				}
				else {
					r.Y = row.Bounds.Bottom;
					r.Height = headerClient.Bottom - r.Y;
				}
			}
			return r;
		}
		protected virtual Rectangle CalcRowClientBounds(BaseTabRowViewInfo row, Rectangle headerClient) {
			return ViewInfo.HeaderRowBorderPainter.GetObjectClientRectangle(new TabBorderObjectInfoArgs(ViewInfo, null, null, headerClient));
		}
		protected virtual Rectangle CalcRowBoundsByClient(BaseTabRowViewInfo row, Rectangle rowClient) {
			return ViewInfo.HeaderRowBorderPainter.CalcBoundsByClientRectangle(new TabBorderObjectInfoArgs(ViewInfo, null, null, rowClient));
		}
		protected virtual void CalcPagesViewInfo(BaseTabRowViewInfo row, Rectangle headerClient, Size buttonsSize) {
			Rectangle rowClient = CalcRowClientBounds(row, headerClient);
			Rectangle r = rowClient;
			Size maxPage = CalcUpdatePagesSize(Rows.Count == 1 ? AllPages : row.Pages);
			row.Bounds = row.Client = Rectangle.Empty;
			if(maxPage.IsEmpty) return;
			maxPage = UpdateHeaderBoundsSize(maxPage);
			int clientOffset = 0;
			bool canIncreaseRowSize = Rows.Count < 2;
			if(IsSideLocation) {
				if(canIncreaseRowSize) {
					r.Width = Math.Max(maxPage.Width, buttonsSize.Width);
					if(maxPage.Width < buttonsSize.Width && HeaderLocation == TabHeaderLocation.Left)
						clientOffset = buttonsSize.Width - maxPage.Width;
				}
				else r.Width = maxPage.Width;
				if(HeaderLocation == TabHeaderLocation.Right)
					r.X = rowClient.Right - r.Width;
			}
			else {
				if(canIncreaseRowSize) {
					r.Height = Math.Max(maxPage.Height, buttonsSize.Height);
					if(maxPage.Height < buttonsSize.Height && HeaderLocation == TabHeaderLocation.Top)
						clientOffset = buttonsSize.Height - maxPage.Height;
				}
				else r.Height = maxPage.Height;
				if(HeaderLocation == TabHeaderLocation.Bottom)
					r.Y = rowClient.Bottom - r.Height;
			}
			row.Client = r;
			row.Bounds = CalcRowBoundsByClient(row, r);
			CalcRowAutoFill(row, buttonsSize);
			Point startPoint = CalcStartPoint(r);
			if(canIncreaseRowSize && clientOffset > 0) {
				if(IsLeftLocation)
					startPoint.X += clientOffset;
				if(IsTopLocation)
					startPoint.Y += clientOffset;
				Buttons.Bounds = new Rectangle(Buttons.Bounds.Location, buttonsSize);
				Buttons.IsDirty = true;
			}
			foreach(BaseTabPageViewInfo info in row.Pages) {
				CalcPageViewInfo(row, info, ref startPoint);
			}
		}
		protected virtual Size CalcRowPagesSize(BaseTabRowViewInfo row) {
			Size size = Size.Empty;
			foreach(BaseTabPageViewInfo info in row.Pages) {
				size.Width += info.Bounds.Width;
				size.Height += info.Bounds.Height;
			}
			return size;
		}
		protected virtual void CalcRowAutoFill(BaseTabRowViewInfo row, Size buttonsSize) {
			if(!ViewInfo.IsHeaderAutoFill) return;
			Size max = row.Client.Size;
			max.Width -= CalcMaxPageExtent();
			max.Height -= CalcMaxPageExtent();
			Size size = CalcRowPagesSize(row); ;
			int count = row.Pages.Count;
			int d = 0, total, cd, pos = 0;
			total = GetSizeWidth(max) - GetSizeWidth(size) - GetSizeWidth(buttonsSize);
			if(total < 1 || count == 0) return;
			d = total / count;
			for(int n = 0; n < row.Pages.Count; n++) {
				BaseTabPageViewInfo info = row.Pages[n];
				bool isLastPage = n == row.Pages.Count - 1;
				if(isLastPage) cd = total - pos;
				else cd = d;
				Rectangle r = info.Bounds;
				if(IsSideLocation) {
					r.Height += cd;
				}
				else {
					r.Width += cd;
				}
				info.Bounds = r;
				pos += cd;
			}
		}
		protected virtual Rectangle GetPageClipRectangle(BaseTabPageViewInfo info) {
			if((info.PageState & ObjectState.Selected) != 0) return info.Bounds;
			Rectangle r = info.Bounds;
			foreach(BaseTabRowViewInfo rowInfo in Rows) {
				foreach(BaseTabPageViewInfo vi in rowInfo.Pages) {
					if(info == vi) continue;
					if((vi.PageState & ObjectState.Selected) != 0) {
						if(r.IntersectsWith(vi.Bounds)) {
							info.ClipRegion = new Region(info.Bounds);
							info.ClipRegion.Exclude(vi.Bounds);
							return r;
						}
					}
				}
			}
			return r;
		}
		public virtual bool ExcludeFromClipping(BaseTabPageViewInfo info) {
			if((info.PageState & ObjectState.Selected) == 0) return false;
			return true;
		}
		protected virtual void UpdateClipBounds() {
			foreach(BaseTabRowViewInfo rowInfo in Rows) {
				foreach(BaseTabPageViewInfo info in rowInfo.Pages) {
					info.Clip = GetPageClipRectangle(info);
				}
			}
		}
		protected virtual void UpdatePagesBounds() {
			foreach(BaseTabRowViewInfo rowInfo in Rows) {
				foreach(BaseTabPageViewInfo info in rowInfo.Pages) {
					UpdatePageBounds(info);
				}
			}
		}
		protected virtual int CalcMaxPageExtent() {
			return 4;
		}
		protected virtual int CalcSideGrowSize() {
			return 2;
		}
		protected virtual int CalcUpDownGrowSize() {
			return 2;
		}
		protected virtual int CalcBorderSideSize() {
			return 2;
		}
		protected virtual void UpdatePageBounds(BaseTabPageViewInfo info) {
			if((info.PageState & ObjectState.Selected) == 0) return;
			Rectangle r = info.Bounds;
			Rectangle controlBoxRect = info.ControlBox;
			int sideGrow = CalcSideGrowSize();
			int growUpDown = CalcUpDownGrowSize();
			int border = CalcBorderSideSize();
			if(IsSideLocation) {
				r.Height += sideGrow * 2; r.Y -= sideGrow;
				r.Width += growUpDown + border;
				if(HeaderLocation == TabHeaderLocation.Left) {
					r.X -= growUpDown;
					growUpDown *= -1;
				}
				else {
					r.X -= border;
					growUpDown = -border;
				}
				info.Text = info.OffsetRect(info.Text, growUpDown, 0);
				info.Image = info.OffsetRect(info.Image, growUpDown, 0);
				if(!controlBoxRect.IsEmpty)
					controlBoxRect = info.OffsetRect(controlBoxRect, growUpDown, 0);
			}
			else {
				r.Width += sideGrow * 2; r.X -= sideGrow;
				r.Height += growUpDown + border;
				if(HeaderLocation == TabHeaderLocation.Top) {
					r.Y -= growUpDown;
					growUpDown *= -1;
				}
				else {
					r.Y -= border;
					growUpDown = -border;
				}
				info.Image = info.OffsetRect(info.Image, 0, growUpDown);
				info.Text = info.OffsetRect(info.Text, 0, growUpDown);
				if(!controlBoxRect.IsEmpty)
					controlBoxRect = info.OffsetRect(controlBoxRect, 0, growUpDown);
			}
			if(CanShowCloseButtonForPage(info) || CanShowPinButtonForPage(info)) {
				info.ButtonsPanel.ViewInfo.SetDirty();
				info.ButtonsPanel.ViewInfo.Calc(GraphicsInfo.Graphics, controlBoxRect);
			}
			info.Bounds = r;
		}
		protected virtual Rectangle CalcPageFocusBounds(BaseTabPageViewInfo info, Rectangle contentBounds) {
			if(!ViewInfo.ShowHeaderFocus) return contentBounds;
			contentBounds.Inflate(1, 1);
			return contentBounds;
		}
		protected virtual void CalcPageViewInfo(BaseTabRowViewInfo row, BaseTabPageViewInfo info, ref Point topLeft) {
			Rectangle r = new Rectangle(topLeft, info.Bounds.Size);
			if(IsRightToLeftLocation) r.X -= r.Width;
			info.Bounds = r;
			Rectangle content = info.Bounds;
			content.Width -= CalcPageIndent(info, IndentType.Right);
			int n = CalcPageIndent(info, IndentType.Left);
			content.X += n;
			content.Width -= n;
			content.Height -= CalcPageIndent(info, IndentType.Bottom);
			n = CalcPageIndent(info, IndentType.Top);
			content.Y += n;
			content.Height -= n;
			Rectangle contentWithoutButton = content;
			if((CanShowPageCloseButtons() && CanShowCloseButtonForPage(info)) || (CanShowPagePinButtons() && CanShowPinButtonForPage(info))) {
				ConvertHeaderLocation(info);
				Rectangle controlBox = CalcControlBoxRect(info, content, ref contentWithoutButton);
				if(!controlBox.IsEmpty && !contentWithoutButton.IsEmpty && IsRightToLeftLocation) 
					SwapRectangles(ref controlBox, ref contentWithoutButton);
				CalcButtons(info, controlBox);
				info.DisableDrawCloseButton = CanDisableDrawCloseButtonForInactivePage(info);
				info.DisableDrawPinButton = CanDisableDrawPinButtonForInactivePage(info);
			}
			info.Content = contentWithoutButton;
			info.Focus = CalcPageFocusBounds(info, info.Content);
			info.Image = info.Text = Rectangle.Empty;
			if(RealPageOrientation == TabOrientation.Horizontal)
				CalcHPageViewInfo(info);
			else
				CalcVPageViewInfo(info);
			NextStartPoint(info, ref topLeft);
		}
		static int ControlBoxMinSize = 14;
		protected Rectangle CalcControlBoxRect(BaseTabPageViewInfo info, Rectangle contentAll, ref Rectangle contentWithoutButton) {
			Size controlBoxSize = info.ButtonsPanel.ViewInfo.CalcMinSize(GraphicsInfo.Graphics);
			Rectangle closeBtn = Rectangle.Empty;
			if(controlBoxSize.IsEmpty) return closeBtn;
			int indent = CalcPageIndent(info, IndentType.CloseButton);
			Point offset = CalcClosePageButtonOffset(info);
			if(RealPageOrientation == TabOrientation.Horizontal) {
				CheckAndSetRightBthPlacement(contentAll, controlBoxSize, offset, indent, ref closeBtn, ref contentWithoutButton);
				CheckAndSetLeftBthPlacement(contentAll, controlBoxSize, offset, indent, ref closeBtn, ref contentWithoutButton);
			}
			else {
				CheckAndSetTopBthPlacement(contentAll, controlBoxSize, offset, indent, ref closeBtn, ref contentWithoutButton);
				CheckAndSetBottomBthPlacement(contentAll, controlBoxSize, offset, indent, ref closeBtn, ref contentWithoutButton);
			}
			return closeBtn;
		}
		protected Point CalcClosePageButtonOffset(BaseTabPageViewInfo info) {
			Point result = new Point(0, 0);
			if((IsShowPageImage(info) || IsShowPageText(info)) && (CanShowPageCloseButtons() || CanShowPagePinButtons())) {
				result = CalcCloseButtonOffset();
			}
			return result;
		}
		protected bool CheckAndSetRightBthPlacement(Rectangle content, Size btnSize, Point offset, int indent, ref Rectangle closeBtn, ref Rectangle contentWithoutButton) {
			bool checkRes = (HeaderLocation == TabHeaderLocation.Top) || (HeaderLocation == TabHeaderLocation.Right) || (HeaderLocation == TabHeaderLocation.Bottom);
			if(checkRes) {
				int top = ((content.Top * 2) + (content.Height - btnSize.Height)) / 2;
				closeBtn = new Rectangle(content.Right - btnSize.Width + offset.X, top + offset.Y, btnSize.Width, btnSize.Height);
				contentWithoutButton = new Rectangle(content.Left, content.Top, content.Width - btnSize.Width - indent, content.Height);
			}
			return checkRes;
		}
		protected bool CheckAndSetLeftBthPlacement(Rectangle content, Size btnSize, Point offset, int indent, ref Rectangle closeBtn, ref Rectangle contentWithoutButton) {
			bool checkRes = (HeaderLocation == TabHeaderLocation.Left);
			if(checkRes) {
				int top = ((content.Top * 2) + (content.Height - btnSize.Height)) / 2;
				closeBtn = new Rectangle(content.Left - offset.X, top + offset.Y, btnSize.Width, btnSize.Height);
				contentWithoutButton = new Rectangle(content.Left + btnSize.Width + indent, content.Top, content.Width - btnSize.Width + indent, content.Height);
			}
			return checkRes;
		}
		protected bool CheckAndSetTopBthPlacement(Rectangle content, Size btnSize, Point offset, int indent, ref Rectangle closeBtn, ref Rectangle contentWithoutButton) {
			bool checkRes = (HeaderLocation == TabHeaderLocation.Top) || (HeaderLocation == TabHeaderLocation.Left);
			if(checkRes) {
				int leftDelta = (content.Width - btnSize.Width) / 2;
				closeBtn = new Rectangle(content.Left + leftDelta + offset.Y, content.Top - offset.X, btnSize.Width, btnSize.Height);
				contentWithoutButton = new Rectangle(content.Left, content.Top + btnSize.Height + indent, content.Width, content.Height - btnSize.Height - indent);
			}
			return checkRes;
		}
		protected bool CheckAndSetBottomBthPlacement(Rectangle content, Size btnSize, Point offset, int indent, ref Rectangle closeBtn, ref Rectangle contentWithoutButton) {
			bool checkRes = (HeaderLocation == TabHeaderLocation.Right) || (HeaderLocation == TabHeaderLocation.Bottom);
			if(checkRes) {
				int leftDelta = (content.Width - btnSize.Width) / 2;
				closeBtn = new Rectangle(content.Right - btnSize.Width - leftDelta - offset.Y, content.Bottom - btnSize.Height + offset.X, btnSize.Width, btnSize.Height);
				contentWithoutButton = new Rectangle(content.Left, content.Top, content.Width, content.Height - btnSize.Height - indent);
			}
			return checkRes;
		}
		protected virtual void NextStartPoint(BaseTabPageViewInfo info, ref Point topLeft) {
			if(IsSideLocation) {
				topLeft.Y += info.Bounds.Height;
				topLeft.Y += info.Separator.Height;
			}
			else {
				if(IsRightToLeftLocation) {
					topLeft.X -= info.Bounds.Width;
				} else
					topLeft.X += info.Bounds.Width;
				topLeft.Y += info.Separator.Width; 
			}
		}
		protected virtual bool IsShowPageText(BaseTabPageViewInfo info) {
			if(info.Page.Text == null || info.Page.Text == "") return false;
			if(IsShowPageImage(info) && ViewInfo.HeaderImagePosition == TabPageImagePosition.Center) return false;
			return true;
		}
		protected virtual TabPageImagePosition GetPageImagePosition(BaseTabPageViewInfo info) {
			bool showImage = IsShowPageImage(info), showText = IsShowPageText(info);
			if(!showImage) return TabPageImagePosition.None;
			if(!showText) return TabPageImagePosition.Center;
			TabPageImagePosition res = ViewInfo.HeaderImagePosition;
			if(res == TabPageImagePosition.Center) return res;
			if(RealPageOrientation == TabOrientation.Vertical && (IsLeftLocation || IsTopLocation)) {
				if(res == TabPageImagePosition.Near) res = TabPageImagePosition.Far;
				else
					if(res == TabPageImagePosition.Far) res = TabPageImagePosition.Near;
			}
			return res; ;
		}
		protected virtual bool IsShowPageImage(BaseTabPageViewInfo info) {
			if(info.ImageSize.IsEmpty || ViewInfo.HeaderImagePosition == TabPageImagePosition.None) return false;
			return true;
		}
		void SetAppearance(Hashtable hash, AppearanceObject app) {
			hash[AppearanceHelper.GetFontHashCode(app.GetFont())] = app;
		}
		Hashtable hash;
		protected void CalcAppearances(BaseTabPageViewInfo info) {
			if(hash == null) hash = new Hashtable();
			Hashtable coreHash = new Hashtable();
			SetAppearance(coreHash, info.GetPageAppearance(ObjectState.Normal, DefaultBoolean.False));
			SetAppearance(coreHash, info.GetPageAppearance(ObjectState.Hot, DefaultBoolean.False));
			SetAppearance(coreHash, info.GetPageAppearance(ObjectState.Disabled, DefaultBoolean.False));
			SetAppearance(coreHash, info.GetPageAppearance(ObjectState.Selected, DefaultBoolean.False));
			SetAppearance(coreHash, info.GetPageAppearance(ObjectState.Selected, DefaultBoolean.True));
			hash.Add(info, coreHash);
		}
		protected virtual Size CalcPageTextSize(Graphics g, BaseTabPageViewInfo info) {
			Size realRes = Size.Empty;
			if(!IsShowPageText(info)) return realRes;
			Size[] res = new Size[4];
			if(hash == null || hash[info] == null) {
				CalcAppearances(info);
			}
			int max = 0;
			foreach(AppearanceObject app in ((Hashtable)hash[info]).Values) {
				res[max++] = CalcPageTextSize(g, info, app, info.Page.Text);
			}
			for(int n = 0; n < max; n++) {
				realRes.Width = Math.Max(res[n].Width, realRes.Width);
				realRes.Height = Math.Max(res[n].Height, realRes.Height);
			}
			return realRes;
		}
		protected virtual Size CalcPageImageSize(Graphics g, BaseTabPageViewInfo info) {
			if(!IsShowPageImage(info)) return Size.Empty;
			return info.ImageSizeWithPadding;
		}
		protected virtual Size CalcPageTextSize(Graphics g, BaseTabPageViewInfo info, AppearanceObject appearance, string pageText) {
			if(pageText == null || pageText.Length == 0) pageText = "Wg";
			SizeF size;
			using(StringFormat strFormat = appearance.GetStringFormat().Clone() as StringFormat) {
				if(appearance.TextOptions.HotkeyPrefix == HKeyPrefix.Default && info.UseHotkeyPrefixDrawModeOverride)
					strFormat.HotkeyPrefix = info.HotkeyPrefixDrawModeOverride;
				size = MeasureString(g, appearance, pageText, strFormat);
			}
			Size res = new Size((int)Math.Ceiling(size.Width), (int)Math.Ceiling(size.Height));
			if(RealPageOrientation == TabOrientation.Vertical) return new Size(res.Height, res.Width);
			return res;
		}
		SizeF MeasureString(Graphics g, AppearanceObject appearance, string pageText, StringFormat strFormat) {
			return (RealPageOrientation == TabOrientation.Horizontal) ? appearance.CalcTextSize(g, strFormat, pageText, 0) :
			g.MeasureString(pageText, appearance.GetFont(), 0, strFormat);
		}
		Hashtable pageStateStorageCore = new Hashtable();
		protected Hashtable GetPageStateStorage(IXtraTabPage page) {
			Hashtable storage = pageStateStorageCore[page] as Hashtable;
			if(storage == null) {
				storage = new Hashtable();
				pageStateStorageCore.Add(page, storage);
			}
			return storage;
		}
		protected void ResetPageStateStorage(IXtraTabPage page) {
			pageStateStorageCore.Remove(page);
		}
		protected virtual void CreatePages() {
			ArrayList list = new ArrayList();
			for(int n = 0; n < TabControl.PageCount; n++) {
				IXtraTabPage page = TabControl.GetTabPage(n);
				if(!page.PageVisible && !ViewInfo.IsDesignMode) continue;
				BaseTabPageViewInfo pageInfo = CreatePage(page);
				pageInfo.ApplyState(GetPageStateStorage(page));
				AllPages.Add(pageInfo);
				pageInfo.UpdatePaintAppearance();
				if(page is IXtraTabPageExt && ((IXtraTabPageExt)page).Pinned) {
					pageInfo.VisibleIndex = GetIndexLastPinnedTab();
					VisiblePages.InternalInsert(pageInfo.VisibleIndex, pageInfo);
				}
				else if(AllPages.Count > ViewInfo.FirstVisiblePageIndex) {
					pageInfo.VisibleIndex = VisiblePages.Add(pageInfo);
				}
			}
		}
		protected internal int GetIndexLastPinnedTab() {
			int result = 0;
			foreach(BaseTabPageViewInfo info in VisiblePages) {
				if(info.Page is IXtraTabPageExt && ((IXtraTabPageExt)info.Page).Pinned)
					result++;
			}
			return result;
		}
		protected virtual BaseTabPageViewInfo CreatePage(IXtraTabPage page) {
			return new BaseTabPageViewInfo(page);
		}
		protected virtual void CalcPagesSize() {
			ResetCache();
			allowResetCache = false;
			BaseTabPageViewInfo[] pageInfos = AllPages.ToArray();
			foreach(BaseTabPageViewInfo info in pageInfos) {
				info.Bounds = new Rectangle(Point.Empty, CalcPageSize(info));
			}
			allowResetCache = true;
		}
		protected virtual Size CalcUpdatePagesSize(PageViewInfoCollection pages) {
			Size maxSize = Size.Empty, maxBoundsSize = Size.Empty;
			foreach(BaseTabPageViewInfo info in pages) {
				Size size = GetPageClientSize(info, info.Bounds.Size);
				maxSize.Width = Math.Max(maxSize.Width, size.Width);
				maxSize.Height = Math.Max(maxSize.Height, size.Height);
			}
			int w, h;
			foreach(BaseTabPageViewInfo info in pages) {
				w = maxSize.Width;
				h = maxSize.Height;
				Size size = GetPageClientSize(info, info.Bounds.Size);
				if(RealPageOrientation == TabOrientation.Horizontal) {
					if(!IsSideLocation) w = size.Width;
				}
				else {
					if(IsSideLocation) h = size.Height;
				}
				info.Bounds = new Rectangle(Point.Empty, GetPageBoundsByClientSize(info, new Size(w, h)));
				maxBoundsSize.Width = Math.Max(maxBoundsSize.Width, info.Bounds.Width);
				maxBoundsSize.Height = Math.Max(maxBoundsSize.Height, info.Bounds.Height);
			}
			return maxBoundsSize;
		}
		protected virtual int CalcPageIndent(BaseTabPageViewInfo info, IndentType indent) {
			return (RealPageOrientation == TabOrientation.Horizontal ? CalcHPageIndent(info, indent) : CalcVPageIndent(info, indent));
		}
		protected virtual int CalcHPageIndent(BaseTabPageViewInfo info, IndentType indent) {
			switch(indent) {
				case IndentType.BetweenImageText:
					if(IsShowPageImage(info) && IsShowPageText(info)) return 3;
					break;
				case IndentType.CloseButton:
					if((IsShowPageImage(info) || IsShowPageText(info)) && (CanShowPageCloseButtons() || CanShowPagePinButtons())) return 4;
					break;
				case IndentType.Left: return 5;
				case IndentType.Right: return 4;
				case IndentType.Top: return 3;
				case IndentType.Bottom: return 2;
			}
			return 0;
		}
		protected virtual int CalcVPageIndent(BaseTabPageViewInfo info, IndentType indent) {
			switch(indent) {
				case IndentType.BetweenImageText:
					if(IsShowPageImage(info) && IsShowPageText(info)) return 3;
					break;
				case IndentType.CloseButton:
					if((IsShowPageImage(info) || IsShowPageText(info)) && (CanShowPageCloseButtons() || CanShowPagePinButtons())) return 4;
					break;
				case IndentType.Top: return 4;
				case IndentType.Bottom: return 3;
				case IndentType.Left: return 3;
				case IndentType.Right: return 2;
			}
			return 0;
		}
		public virtual Size CalcMinPageSize() {
			return new Size(4, 4);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool allowResetCache = true;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void ResetCache() {
			if(!allowResetCache)
				hash = new Hashtable();
		}
		public Size CalcPageSize(BaseTabPageViewInfo info) {
			ResetCache();
			return GetPageBoundsByClientSize(info, CalcPageClientSize(info));
		}
		protected virtual Size GetPageClientSize(BaseTabPageViewInfo info, Size bounds) {
			Size size = bounds;
			size.Width -= (CalcPageIndent(info, IndentType.Left) + CalcPageIndent(info, IndentType.Right));
			size.Height -= (CalcPageIndent(info, IndentType.Top) + CalcPageIndent(info, IndentType.Bottom));
			return size;
		}
		protected virtual Size GetPageBoundsByClientSize(BaseTabPageViewInfo info, Size client) {
			Size size = client;
			size.Width += (CalcPageIndent(info, IndentType.Left) + CalcPageIndent(info, IndentType.Right));
			size.Height += (CalcPageIndent(info, IndentType.Top) + CalcPageIndent(info, IndentType.Bottom));
			return size;
		}
		protected virtual int GetUserPageSize(BaseTabPageViewInfo info) {
			if(info.Page is IXtraTabPageExt) {
				int maxTabPageWidth = GetMaxTabPageWidth(info);
				if(maxTabPageWidth != 0) return maxTabPageWidth;
			}
			if(info.Page.TabPageWidth != 0) return info.Page.TabPageWidth;
			if(Properties.TabPageWidth != 0) return Properties.TabPageWidth;
			return 0;
		}
		int GetMaxTabPageWidth(BaseTabPageViewInfo info) {
			IXtraTabPageExt page = info.Page as IXtraTabPageExt;
			int result = 0;
			if(RealPageOrientation == TabOrientation.Horizontal) {
				if(info.Bounds.Width > page.MaxTabPageWidth && page.MaxTabPageWidth != 0)
					result = page.MaxTabPageWidth;
			}
			else {
				if(info.Bounds.Height > page.MaxTabPageWidth && page.MaxTabPageWidth != 0)
					result = page.MaxTabPageWidth;
			}
			return result;
		}
		protected virtual Size CalcPageClientSize(BaseTabPageViewInfo info) {
			Size size = Size.Empty;
			GraphicsInfo.AddGraphics(null);
			try {
				UpdatePaintAppearance();
				if(RealPageOrientation == TabOrientation.Horizontal) {
					size = CalcHPageSize(info);
					if(GetUserPageSize(info) != 0) size.Width = GetUserPageSize(info);
				}
				else {
					size = CalcVPageSize(info);
					if(GetUserPageSize(info) != 0) size.Height = GetUserPageSize(info);
				}
			}
			finally {
				GraphicsInfo.ReleaseGraphics();
			}
			Size min = CalcMinPageSize();
			size.Width = Math.Max(size.Width, min.Width);
			size.Height = Math.Max(size.Height, min.Height);
			return size;
		}
		protected virtual Size CalcCloseButtonSize(Graphics g) {
			return ViewInfo.CalcCloseButtonSize(g);
		}
		protected virtual Point CalcCloseButtonOffset() {
			return ViewInfo.CalcCloseButtonOffset();
		}
		protected virtual Size CalcHPageSize(BaseTabPageViewInfo info) {
			Size size = CalcPageTextSize(GraphicsInfo.Graphics, info);
			Size image = CalcPageImageSize(GraphicsInfo.Graphics, info);
			size.Height = Math.Max(size.Height, image.Height);
			size.Width += image.Width;
			size.Width += CalcPageIndent(info, IndentType.BetweenImageText);
			if((CanShowPageCloseButtons() && CanShowCloseButtonForPage(info)) || (CanShowPagePinButtons() && CanShowPinButtonForPage(info))) {
				ConvertHeaderLocation(info);
				Size controlBoxSize = info.ButtonsPanel.ViewInfo.CalcMinSize(GraphicsInfo.Graphics); 
				if(controlBoxSize.Width != 0) {
					controlBoxSize.Width = Math.Max(ControlBoxMinSize, controlBoxSize.Width);
					size.Width += (controlBoxSize.Width) + CalcPageIndent(info, IndentType.CloseButton);
				}
			}
			return size;
		}
		int textToImageIndent = 4;
		protected virtual Size CalcVPageSize(BaseTabPageViewInfo info) {
			Size size = CalcPageTextSize(GraphicsInfo.Graphics, info);
			Size image = CalcPageImageSize(GraphicsInfo.Graphics, info);
			Size closeButtonSize = CalcCloseButtonSize(GraphicsInfo.Graphics);
			size.Width = Math.Max(size.Width, image.Width);
			size.Height += image.Height + textToImageIndent;
			size.Height += CalcPageIndent(info, IndentType.BetweenImageText);
			if((CanShowPageCloseButtons() && CanShowCloseButtonForPage(info)) || (CanShowPagePinButtons() && CanShowPinButtonForPage(info))) {
				ConvertHeaderLocation(info);
				Size controlBoxSize = info.ButtonsPanel.ViewInfo.CalcMinSize(GraphicsInfo.Graphics);
				controlBoxSize.Height = Math.Max(ControlBoxMinSize, controlBoxSize.Height);
				size.Height += (controlBoxSize.Height) + CalcPageIndent(info, IndentType.CloseButton);
			}
			return size;
		}
		protected virtual void CalcHPageViewInfo(BaseTabPageViewInfo info) {
			int near, width;
			near = info.Content.Left;
			width = info.Content.Width;
			Rectangle img = Rectangle.Empty;
			if(IsShowPageImage(info)) {
				Size i = info.ImageSizeWithPadding;
				if(i.Width > info.Content.Width)
					i = Size.Empty;
				if(!i.IsEmpty) {
					img.Size = i;
					TabPageImagePosition pos = GetPageImagePosition(info);
					img.Location = new Point(near, info.Content.Y);
					switch(pos) {
						case TabPageImagePosition.Center:
							img.X += (info.Content.Width - img.Width) / 2;
							break;
						case TabPageImagePosition.Near:
							near += img.Width;
							break;
						case TabPageImagePosition.Far:
							img.X = info.Content.Right - img.Width - 1; break;
					}
					img.Y += (info.Content.Height - img.Height) / 2;
					width -= img.Width;
				}
			}
			near += CalcHPageIndent(info, IndentType.BetweenImageText);
			width -= CalcHPageIndent(info, IndentType.BetweenImageText);
			Rectangle text = Rectangle.Empty;
			if(IsShowPageText(info)) {
				text.Size = CalcPageTextSize(GraphicsInfo.Graphics, info);
				if(info.Page is IXtraTabPageExt)
					info.IsTextTrimming = text.Size.Width > width;
				text.X = near;
				text.Width = width; 
				text.Y = info.Content.Y + (info.Content.Height - text.Height) / 2;
			}
			if(!text.IsEmpty && !img.IsEmpty && IsRightToLeftLocation)
				SwapRectangles(ref text, ref img);
			info.Text = text;
			info.Image = img;
		}
		static void SwapRectangles(ref Rectangle first, ref Rectangle second) {
			if(first.Left < second.Left) {
				int rightMost = second.Right;
				second = new Rectangle(first.Left, second.Top, second.Width, second.Height);
				first = new Rectangle(rightMost - first.Left, first.Top, first.Width, first.Height);
			}
			else {
				int leftMost = second.Left;
				second = new Rectangle(first.Right - second.Width, second.Top, second.Width, second.Height);
				first = new Rectangle(leftMost, first.Top, first.Width, first.Height);
			}
		}
		protected virtual void CalcVPageViewInfo(BaseTabPageViewInfo info) {
			int near, width;
			Rectangle r;
			r = Rectangle.Empty;
			near = info.Content.Top;
			width = info.Content.Height;
			if(IsShowPageImage(info)) {
				Size i = info.ImageSizeWithPadding;
				if(i.Height > info.Content.Height)
					i = Size.Empty;
				if(!i.IsEmpty) {
					r.Size = i;
					TabPageImagePosition pos = GetPageImagePosition(info);
					r.Location = new Point(info.Content.X, near);
					switch(pos) {
						case TabPageImagePosition.Center:
							r.Y += (info.Content.Height - r.Height) / 2;
							near += (r.Height + textToImageIndent);
							break;
						case TabPageImagePosition.Near:
							near += (r.Height + textToImageIndent);
							break;
						case TabPageImagePosition.Far:
							r.Y = info.Content.Bottom - r.Height;
							break;
					}
					r.X += (info.Content.Width - r.Width) / 2;
					info.Image = r;
					width -= r.Height;
					width -= textToImageIndent;
				}
			}
			near += CalcVPageIndent(info, IndentType.BetweenImageText);
			width -= CalcVPageIndent(info, IndentType.BetweenImageText);
			if(IsShowPageText(info)) {
				r.Size = CalcPageTextSize(GraphicsInfo.Graphics, info);
				if(info.Page is IXtraTabPageExt)
					info.IsTextTrimming = r.Height > width;
				r.Y = near;
				r.Height = width;
				r.X = info.Content.X + (info.Content.Width - r.Width) / 2;
				info.Text = r;
			}
		}
		public virtual bool UpdatePageStates() {
			bool anyChanges = false;
			foreach(BaseTabPageViewInfo info in AllPages) {
				ObjectState newState = CalcPageState(info.Page);
				if(info.PageState != newState) {
					info.PageState = newState;
					info.UpdatePaintAppearance();
					anyChanges = true;
				}
			}
			return anyChanges;
		}
		public virtual ObjectState CalcPageState(IXtraTabPage page) {
			ObjectState state = ObjectState.Normal;
			if(!page.PageEnabled) {
				state = ObjectState.Disabled;
				if(!ViewInfo.IsDesignMode) return state;
			}
			if(page == ViewInfo.HotTrackedTabPage && !ViewInfo.IsDesignMode) {
				state = ObjectState.Hot;
			}
			if(page == ViewInfo.SelectedTabPage)
				state |= ObjectState.Selected;
			return state;
		}
		public static int CorrectPos(bool isRightToLeftLayout, Rectangle pageBounds, Rectangle header, bool horz) {
			return isRightToLeftLayout ?
				CorrectPosRTL(pageBounds, header, horz) :
				CorrectPos(pageBounds, header, horz);
		}
		public static bool HitTestHeader(Rectangle header, int pos, bool horz) {
			int near = horz ? header.Left : header.Top;
			int far = horz ? header.Right : header.Bottom;
			return (near <= pos) && (far >= pos);
		}
		static int CorrectPosRTL(Rectangle pageBounds, Rectangle header, bool horz) {
			if(horz)
				return Math.Min(pageBounds.Right - header.Width, Math.Max(pageBounds.Left, header.Left - header.Width));
			else
				return Math.Max(pageBounds.Top, Math.Min(pageBounds.Bottom - header.Height, header.Bottom));
		}
		static int CorrectPos(Rectangle pageBounds, Rectangle header, bool horz) {
			if(horz)
				return Math.Max(pageBounds.Left, Math.Min(pageBounds.Right - header.Width, header.Left + header.Width));
			else
				return Math.Max(pageBounds.Top, Math.Min(pageBounds.Bottom - header.Height, header.Bottom));
		}
	}
	public class PageViewInfoCollection : CollectionBase {
		public int IndexOf(BaseTabPageViewInfo pageInfo) { return List.IndexOf(pageInfo); }
		public int IndexOf(IXtraTabPage page) {
			BaseTabPageViewInfo pageInfo = this[page];
			return pageInfo == null ? -1 : IndexOf(pageInfo);
		}
		public virtual BaseTabPageViewInfo this[int index] { get { return (BaseTabPageViewInfo)List[index]; } }
		public virtual BaseTabPageViewInfo this[IXtraTabPage page] {
			get {
				for(int n = Count - 1; n >= 0; n--) {
					BaseTabPageViewInfo vi = this[n];
					if(vi.Page == page) return vi;
				}
				return null;
			}
		}
		public BaseTabPageViewInfo FirstPage { get { return Count == 0 ? null : this[0]; } }
		public BaseTabPageViewInfo LastPage { get { return Count == 0 ? null : this[Count - 1]; } }
		public virtual int Add(BaseTabPageViewInfo pageInfo) {
			return List.Add(pageInfo);
		}
		internal void InternalInsert(int index, BaseTabPageViewInfo pageInfo) {
			InnerList.Insert(index, pageInfo);
		}
		internal void InternalRemoveAt(int index) {
			InnerList.RemoveAt(index);
		}
		protected override void OnRemoveComplete(int position, object val) {
			base.OnRemoveComplete(position, val);
			BaseTabPageViewInfo page = val as BaseTabPageViewInfo;
			if(page != null) page.Clear();
		}
		protected override void OnClear() {
			for(int n = Count - 1; n >= 0; n--) {
				this[n].Clear();
			}
			InnerList.Clear();
		}
		protected internal BaseTabPageViewInfo[] ToArray() {
			BaseTabPageViewInfo[] result = new BaseTabPageViewInfo[InnerList.Count];
			InnerList.CopyTo(result, 0);
			return result;
		}
	}
	public class BaseTabPageViewInfo : IDisposable, IButtonsPanelOwner {
		bool allowDraw;
		Region pageRegion, clipRegion;
		Rectangle bounds, image, text, focus, content, separator, clip;
		ObjectState pageState;
		AppearanceObject paintAppearance, paintAppearanceClient;
		IXtraTabPage page;
		BaseTabRowViewInfo row;
		int visibleIndex;
		BaseButtonsPanel buttonsPanelCore;
		public BaseTabPageViewInfo(IXtraTabPage page) {
			this.visibleIndex = -1;
			this.allowDraw = true;
			this.row = null;
			this.page = page;
			this.pageRegion = null;
			this.paintAppearance = new FrozenAppearance();
			this.paintAppearanceClient = new FrozenAppearance();
			this.buttonsPanelCore = CreateButtonsPanel();
			InitButtonsPanel();
			SubscribeButtonsPanel();
			Clear();
		}
		protected void InitButtonsPanel() {
			ButtonsPanel.BeginUpdate();
			ButtonsPanel.ButtonInterval = 2;
			var headerInfo = ViewInfo.HeaderInfo;
			var closeButton = new TabCloseButton();
			var pinButton = new TabPinButton();
			closeButton.Visible = headerInfo.CanShowCloseButtonForPage(this) && headerInfo.CanShowPageCloseButtons();
			closeButton.Enabled = Page.PageEnabled;
			ButtonsPanel.Buttons.AddButtonInternal(closeButton);
			if(Page is IXtraTabPageExt) {
				pinButton.Visible = ((IXtraTabPageExt)Page).UsePinnedTab && headerInfo.CanShowPinButtonForPage(this) && headerInfo.CanShowPagePinButtons();
				pinButton.Checked = !((IXtraTabPageExt)Page).Pinned;
				pinButton.Enabled = Page.PageEnabled;
				ButtonsPanel.Buttons.AddButtonInternal(pinButton);
			}
			ButtonsPanel.CancelUpdate();
		}
		protected virtual void SubscribeButtonsPanel() {
			ButtonsPanel.ButtonClick += OnDefaultButtonClick;
			ButtonsPanel.ButtonChecked += OnDefaultButtonClick;
			ButtonsPanel.ButtonUnchecked += OnDefaultButtonClick;
		}
		protected virtual void UnsubscribeButtonsPanel() {
			ButtonsPanel.ButtonClick -= OnDefaultButtonClick;
			ButtonsPanel.ButtonChecked -= OnDefaultButtonClick;
			ButtonsPanel.ButtonUnchecked -= OnDefaultButtonClick;
		}
		void OnDefaultButtonClick(object sender, DevExpress.XtraBars.Docking2010.BaseButtonEventArgs e) {
			IBaseButton button = e.Button;
			if(!(button is DefaultButton)) return;
			if(button is TabPinButton)
				OnClickPinButton();
			if(button is TabCloseButton)
				OnClickCloseButton();
		}
		protected virtual void OnClickPinButton() {
			if(Page is IXtraTabPageExt)
				((IXtraTabPageExt)Page).Pinned = !((IXtraTabPageExt)Page).Pinned;
		}
		protected virtual void OnClickCloseButton() {
			if(Page != null && Page.PageEnabled)
				ViewInfo.OnPageCloseButtonClick(new ClosePageButtonEventArgs(Page, Page));
		}
		protected virtual BaseButtonsPanel CreateButtonsPanel() {
			return new BaseButtonsPanelWithState(this);
		}
		public virtual void Dispose() {
			Clear();
			this.paintAppearance.Dispose();
			this.paintAppearanceClient.Dispose();
			if(buttonsPanelCore != null) {
				UnsubscribeButtonsPanel();
				ButtonsPanel.Dispose();
				buttonsPanelCore = null;
			}
			GC.SuppressFinalize(this);
		}
		public bool AllowDraw {
			get { return allowDraw; }
			set { allowDraw = value; }
		}
		public BaseTabRowViewInfo Row {
			get { return row; }
			set { row = value; }
		}
		public int VisibleIndex {
			get { return Row == null ? visibleIndex : Row.Pages.IndexOf(this); }
			set { visibleIndex = value; }
		}
		public AppearanceObject PaintAppearanceClient {
			get { return paintAppearanceClient; }
			set { paintAppearanceClient = value; }
		}
		public AppearanceObject PaintAppearance {
			get { return paintAppearance; }
			set {
				if(PaintAppearance != null)
					paintAppearance.Dispose();
				paintAppearance = value;
			}
		}
		public virtual Size ImageSize {
			get {
				if(Page.Image != null) return Page.Image.Size;
				if(Page.ImageIndex > -1 && Page.ImageIndex < ImageCollection.GetImageListImageCount(Page.TabControl.Images))
					return ImageCollection.GetImageListSize(Page.TabControl.Images);
				return Size.Empty;
			}
		}
		public virtual Size ImageSizeWithPadding {
			get {
				Size imgSize = ImageSize;
				if(!imgSize.IsEmpty) {
					imgSize = new Size(
							imgSize.Width + Page.ImagePadding.Horizontal,
							imgSize.Height + Page.ImagePadding.Vertical
						);
				}
				return imgSize;
			}
		}
		public bool IsEmptyImagePadding {
			get {
				return (Page.ImagePadding.Left == 0) && (page.ImagePadding.Top == 0) &&
					(Page.ImagePadding.Right == 0) && (page.ImagePadding.Bottom == 0);
			}
		}
		public virtual void Clear() {
			PageRegion = null;
			ClipRegion = null;
			Focus = Clip = Separator = Content = Image = Text = Bounds = Rectangle.Empty;
			this.pageState = ObjectState.Normal;
			drawControlBox = false;
		}
		public IButtonsPanel ButtonsPanel { get { return buttonsPanelCore; } }
		protected internal BaseViewInfoRegistrator View { get { return TabControl == null ? null : TabControl.View; } }
		protected PageAppearance DefaultPageAppearance { get { return Properties.AppearancePage; } }
		protected IXtraTab TabControl { get { return Page == null ? null : Page.TabControl; } }
		public BaseTabControlViewInfo ViewInfo { get { return TabControl == null ? null : TabControl.ViewInfo; } }
		protected IXtraTabProperties Properties { get { return ViewInfo == null ? null : ViewInfo.Properties; } }
		public virtual IXtraTabPage Page { get { return page; } }
		public virtual Rectangle Bounds {
			get { return bounds; }
			set { bounds = value; }
		}
		bool drawControlBox = false;
		public bool DisableDrawCloseButton {
			get { return drawControlBox; }
			set { drawControlBox = value; }
		}
		bool drawPinButton = false;
		public bool DisableDrawPinButton {
			get { return drawPinButton; }
			set { drawPinButton = value; }
		}
		public virtual Rectangle ControlBox {
			get { return ButtonsPanel.Bounds.Width <= 0 || ButtonsPanel.Bounds.Height <= 0 ? Rectangle.Empty : ButtonsPanel.Bounds; }
		}
		public virtual Rectangle Focus {
			get { return focus; }
			set { focus = value; }
		}
		public virtual Rectangle Clip {
			get { return clip; }
			set {
				if(value == Clip) return;
				clip = value;
				UpdateClipRegion();
			}
		}
		public virtual Rectangle Separator {
			get { return separator; }
			set { separator = value; }
		}
		public virtual void UpdatePaintAppearance() {
			PaintAppearance = GetPageAppearance(PageState);
			AppearanceHelper.Combine(PaintAppearanceClient,
				new AppearanceObject[] { PageAppearance.PageClient, DefaultPageAppearance == null ? null : DefaultPageAppearance.PageClient },
				ViewInfo.GetDefaultAppearance(TabPageAppearance.PageClient));
		}
		protected PageAppearance PageAppearance {
			get {
				if(Page.Appearance == null) return ViewInfo.Properties.AppearancePage;
				return Page.Appearance;
			}
		}
		protected internal AppearanceObject GetPageAppearance(ObjectState state, DefaultBoolean isActive) {
			AppearanceObject res = new FrozenAppearance();
			AppearanceObject main = GetPageAppearance(PageAppearance, state, isActive), defAppearance = null;
			AppearanceDefault def = (View == null) ? AppearanceDefault.Control : ViewInfo.GetPageHeaderAppearanceByState(state);
			def = UpdatePageDefaultAppearance(def);
			defAppearance = (DefaultPageAppearance == null) ? null : GetPageAppearance(DefaultPageAppearance, state, isActive);
			AppearanceHelper.Combine(res, new AppearanceObject[] { main, defAppearance }, def);
			return res;
		}
		protected internal AppearanceObject GetPageAppearance(ObjectState state) {
			return GetPageAppearance(state, DefaultBoolean.Default);
		}
		protected virtual AppearanceDefault UpdatePageDefaultAppearance(AppearanceDefault defaultAppearance) { return defaultAppearance; }
		protected virtual AppearanceObject GetPageAppearance(PageAppearance app, ObjectState state, DefaultBoolean isActive) {
			bool selected = (state & ObjectState.Selected) != 0;
			ObjectState temp = state & (~ObjectState.Selected);
			AppearanceObject res = null;
			switch(temp) {
				case ObjectState.Disabled: res = app.HeaderDisabled; break;
				case ObjectState.Hot: res = app.HeaderHotTracked; break;
			}
			if(!selected) return res == null ? app.Header : res;
			AppearanceObject comb = new AppearanceObject();
			AppearanceHelper.Combine(comb, new AppearanceObject[] { app.HeaderActive, app.Header, res });
			return comb;
		}
		protected virtual AppearanceObject GetPageAppearance(PageAppearance app, ObjectState state) {
			return GetPageAppearance(app, state, DefaultBoolean.Default);
		}
		public virtual Rectangle Content {
			get { return content; }
			set { content = value; }
		}
		public virtual Rectangle Image {
			get { return image; }
			set { image = value; }
		}
		public virtual Rectangle Text {
			get { return text; }
			set { text = value; }
		}
		public bool IsActiveState {
			get {
				bool result = false;
				if(ViewInfo != null && ViewInfo.SelectedTabPage != null)
					result = ViewInfo.SelectedTabPage == Page;
				return result || (PageState & ObjectState.Selected) != 0;
			}
		}
		public bool IsHotState {
			get { return (PageState & ObjectState.Hot) != 0; }
		}
		public virtual ObjectState PageState {
			get { return pageState; }
			set { pageState = value; }
		}
		public virtual Region PageRegion {
			get { return pageRegion; }
			set {
				if(PageRegion == value) return;
				if(pageRegion != null) pageRegion.Dispose();
				this.pageRegion = value;
			}
		}
		public virtual Region ClipRegion {
			get { return clipRegion; }
			set {
				if(ClipRegion == value) return;
				if(clipRegion != null) clipRegion.Dispose();
				this.clipRegion = value;
			}
		}
		protected virtual void UpdateClipRegion() {
		}
		public virtual Rectangle OffsetRect(Rectangle r, int x, int y) {
			r.Offset(x, y);
			return r;
		}
		public bool UseHotkeyPrefixDrawModeOverride {
			get { return Page is IXtraTabPageExt; }
		}
		public System.Drawing.Text.HotkeyPrefix HotkeyPrefixDrawModeOverride {
			get {
				IXtraTabPageExt extPage = Page as IXtraTabPageExt;
				if(extPage != null)
					return extPage.HotkeyPrefixOverride;
				else
					return System.Drawing.Text.HotkeyPrefix.Show;
			}
		}
		bool isTextTrimmingCore;
		public bool IsTextTrimming {
			get { return isTextTrimmingCore; }
			internal set { isTextTrimmingCore = value; }
		}
		#region IButtonsPanelOwner Members
		object IButtonsPanelOwner.Images {
			get { return null; }
		}
		ObjectPainter IButtonsPanelOwner.GetPainter() {
			return TabControl.View.CreateControlBoxPainter(TabControl) as BaseButtonsPanelPainter;
		}
		bool IButtonsPanelOwner.IsSelected {
			get {
				bool result = false;
				if(ViewInfo != null && ViewInfo.SelectedTabPage != null) {
					result = ViewInfo.SelectedTabPage == Page;
					result &= ViewInfo.IsActive;
				}
				return result && (PageState & ObjectState.Selected) != 0;
			}
		}
		void IButtonsPanelOwner.Invalidate() {
			if(TabControl != null && ButtonsPanel != null && ButtonsPanel.ViewInfo != null)
				TabControl.Invalidate(ButtonsPanel.ViewInfo.Bounds);
		}
		object IButtonsPanelOwner.ButtonBackgroundImages {
			get { return null; }
		}
		DevExpress.XtraEditors.ButtonsPanelControl.ButtonsPanelControlAppearance IButtonsPanelOwner.AppearanceButton {
			get { return null; }
		}
		bool IButtonsPanelOwner.Enabled {
			get { return true; }
		}
		bool IButtonsPanelOwner.AllowHtmlDraw {
			get { return false; }
		}
		bool IButtonsPanelOwner.AllowGlyphSkinning {
			get { return false; }
		}
		#endregion
		protected internal virtual void SaveState(Hashtable storage) {
			IButtonsPanelHandlerWithState handlerWithState = GetHandlerWithState();
			if(handlerWithState != null) handlerWithState.SaveState(storage);
		}
		protected internal virtual void ApplyState(Hashtable storage) {
			IButtonsPanelHandlerWithState handlerWithState = GetHandlerWithState();
			if(handlerWithState != null) handlerWithState.ApplyState(storage);
		}
		protected IButtonsPanelHandlerWithState GetHandlerWithState() {
			return (ButtonsPanel != null) ? ButtonsPanel.Handler as IButtonsPanelHandlerWithState : null;
		}
	}
	public class TabBorderObjectInfoArgs : BorderObjectInfoArgs {
		BaseTabControlViewInfo viewInfo;
		public TabBorderObjectInfoArgs(BaseTabControlViewInfo viewInfo, GraphicsCache cache, AppearanceObject appearance, Rectangle bounds)
			: this(viewInfo, cache, appearance, bounds, ObjectState.Normal) {
		}
		public TabBorderObjectInfoArgs(BaseTabControlViewInfo viewInfo, GraphicsCache cache, AppearanceObject appearance, Rectangle bounds, ObjectState state)
			: base(cache, bounds, appearance, state) {
			this.viewInfo = viewInfo;
		}
		public BaseTabControlViewInfo ViewInfo { get { return viewInfo; } }
	}
}
