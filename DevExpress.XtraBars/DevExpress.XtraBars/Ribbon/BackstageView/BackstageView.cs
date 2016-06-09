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
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.Extensions;
using DevExpress.XtraBars.MessageFilter;
using DevExpress.XtraBars.Ribbon.Drawing;
using DevExpress.XtraBars.Ribbon.Handler;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using DevExpress.XtraEditors;
using DevExpress.XtraBars.Ribbon.Accessible;
using DevExpress.XtraBars.Ribbon.BackstageView.Accessible;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars.Docking2010;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Utils.Text;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Controls;
using DevExpress.XtraEditors.Drawing;
namespace DevExpress.XtraBars.Ribbon {
	public interface IBackstageViewControl {
		void OnBeforeShowing();
		void OnShowing();
		void OnHiding();
		void OnHided();
		bool WindowsUIStyleActive { get; }
	}
	public interface ISizableControl {
		Control Owner { get; }
		Form OwnerForm { get; }
		void OnBeginSizing();
		void OnEndSizing();
		bool CanUpdateBounds(int width, int height);
		bool CanUpdateSize(MouseEventArgs e);
	}
	public enum BackstageViewPaintStyle { Default, Skinned, Flat }
	[DXToolboxItem(true),
	Description("Allows you to emulate a Backstage View found in Microsoft Office 2010 products."),
	ToolboxTabName(AssemblyInfo.DXTabNavigation),
	Designer("DevExpress.XtraBars.Ribbon.Design.BackstageViewControlDesigner, " + AssemblyInfo.SRAssemblyBarsDesign, typeof(IDesigner)), ToolboxBitmap(typeof(ToolboxIcons.ToolboxIconsRootNS), "BackstageViewControl")]
	public class BackstageViewControl : Control, IBarAndDockingControllerClient, IToolTipControlClient, ISupportXtraAnimation, IXtraAnimationListener, IBackstageViewControl, IBarManagerListener, IKeyTipsOwnerControl, ISizableControl, ISupportInitialize {
		private static readonly object selectedTabChanged = new object();
		private static readonly object highlightedItemChanged = new object();
		private static readonly object itemPressed = new object();
		private static readonly object itemClick = new object();
		private static readonly object customDrawItem = new object();
		private static readonly object backButtonClick = new object();
		private static readonly object shown = new object();
		private static readonly object showing = new object();
		private static readonly object hiding = new object();
		private static readonly object hidden = new object();
		internal readonly object AnimationId = new object();
		BackstageViewControlItemCollecton items;
		BackstageViewInfo viewInfo;
		BackstageViewPainter painter;
		BackstageViewControlHandler handler;
		BarAndDockingController controller;
		int leftPaneMinWidth;
		int leftPaneMaxWidth;
		bool allowGlyphSkinning;
		BackstageViewStyle style;
		BackstageViewStyle realStyle;
		BackstageViewShowRibbonItems backstageViewShowRibbonItems;
		object images;
		ItemLocation glyphLocation;
		ItemHorizontalAlignment glyphHorizontalAlignment, captionHorizontalAlignment;
		ItemVerticalAlignment glyphVerticalAlignment, captionVerticalAlignment;
		int glyphToCaptionIndent;
		Padding itemsContentPadding;
		bool showImage;
		Image image;
		RibbonControl ribbon;
		RibbonControlColorScheme colorScheme;
		BackstageViewOffice2013StyleOptions office2013StyleOptions;
		static readonly Size DefaultControlSize = new Size(240, 150);
		AccessibleBackstageView accessibleBackstageView = null;
		BackstageViewBaseKeyTipManager keyTipManager;
		AppearanceObject appearance, parentAppearance;
		BackstageViewPaintStyle paintStyle;
		bool isInitialize;
		bool needRaiseSelectedTabChanged;
		BackstageViewScrollController scrollController;
		int scrollerPosition;
		public BackstageViewControl() {
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			this.leftPaneMinWidth = -1;
			this.leftPaneMaxWidth = -1;
			this.glyphHorizontalAlignment = ItemHorizontalAlignment.Default;
			this.glyphVerticalAlignment = ItemVerticalAlignment.Default;
			this.captionHorizontalAlignment = ItemHorizontalAlignment.Default;
			this.captionVerticalAlignment = ItemVerticalAlignment.Default;
			this.glyphToCaptionIndent = -1;
			this.allowGlyphSkinning = false;
			this.showImage = true;
			this.image = null;
			this.style = this.realStyle = BackstageViewStyle.Default;
			this.backstageViewShowRibbonItems = BackstageViewShowRibbonItems.Default;
			ToolTipController.DefaultController.AddClientControl(this);
			this.ribbon = null;
			this.colorScheme = RibbonControlColorScheme.Yellow;
			this.office2013StyleOptions = CreateOffice2013StyleOptions();
			this.GetController().AddClient(this);
			this.shouldShowKeyTips = false;
			this.appearance = CreateAppearance();
			this.parentAppearance = CreateAppearance();
			this.paintStyle = BackstageViewPaintStyle.Default;
			this.itemsContentPadding = new Padding(0);
			this.isInitialize = false;
			this.needRaiseSelectedTabChanged = false;
			this.scrollController = CreateScrollController();
			this.scrollController.AddControls(this);
			this.scrollController.VScrollValueChanged += OnVScrollValueChanged;
		}
		protected virtual void OnVScrollValueChanged(object sender, EventArgs e) {
			ScrollerPosition = scrollController.VScrollPosition;
		}
		protected internal BackstageViewScrollController ScrollController { get { return scrollController; } }
		protected internal int ScrollerPosition {
			get { return scrollerPosition; }
			set {
				if(ScrollerPosition == value) return;
				scrollerPosition = ConstrainPosition(value);
				OnScrollerPositionChanged();
			}
		}
		protected int ConstrainPosition(int value) {
			if(value < 0)
				value = 0;
			int height = Math.Max(0, ScrollController.VScrollMaximum - ScrollController.VScrollLargeChange);
			if(value > height)
				value = height;
			return value;
		}
		protected virtual void OnScrollerPositionChanged() {
			Refresh();
			if(ScrollerPosition == 0 && SelectedTab != null) SelectedTab.DoRefreshContent();
		}
		[Browsable(false)]
		public override Color BackColor { get { return base.BackColor; } set { base.BackColor = value; } }
		protected virtual AppearanceObject CreateAppearance() {
			AppearanceObject res = new AppearanceObject();
			res.Changed += new EventHandler(OnAppearanceChanged);
			return res;
		}
		protected virtual void OnAppearanceChanged(object sender, EventArgs e) {
			OnPropertiesChanged();
		}
		internal void ResetAppearance() { Appearance.Reset(); }
		internal bool ShouldSerializeAppearance() { return Appearance != null && Appearance.Options != AppearanceOptions.Empty; }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BackstageViewControlAppearance"),
#endif
 Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject Appearance {
			get { return appearance; }
		}
		internal void ResetParentAppearance() { ParentAppearance.Reset(); }
		internal bool ShouldSerializeParentAppearance() { return ParentAppearance != null && ParentAppearance.Options != AppearanceOptions.Empty; }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public virtual AppearanceObject ParentAppearance {
			get {
				return GetParentAppearance();
			}
		}
		protected AppearanceObject GetParentAppearance() {
			if(Parent is BackstageViewClientControl && GetPaintStyle() == BackstageViewPaintStyle.Flat) {
				BackstageViewClientControl client = Parent as BackstageViewClientControl;
				BackstageViewControl bsv = client.Parent as BackstageViewControl;
				if(bsv.SelectedTab != null) {
					BackstageViewItemBaseViewInfo vi = bsv.SelectedTab.GetViewInfo();
					if(bsv.SelectedTab.ContentControl == client && vi != null) {
						return bsv.SelectedTab.GetViewInfo().GetAppearance(ObjectState.Pressed);
					}
				}
			}
			return parentAppearance;
		}
		protected internal BackstageViewControl ParentBackstageView {
			get { return GetParentBackstageView(); }
		}
		protected internal Form ParentForm {
			get {
				if(ParentBackstageView == null) return Parent as Form;
				else return ParentBackstageView.ParentForm;
			}
		}
		protected BackstageViewControl GetParentBackstageView() {
			if(Parent is BackstageViewClientControl) {
				BackstageViewClientControl client = Parent as BackstageViewClientControl;
				return client.Parent as BackstageViewControl;
			}
			return null;
		}
		protected internal bool IsParentBackstageView {
			get {
				if(ParentBackstageView == null) return true;
				return false;
			}
		}
		protected bool UseParentStyle {
			get {
				if(ParentBackstageView == null) return false;
				if(PaintStyle == BackstageViewPaintStyle.Default) return true;
				return false;
			}
		}
		protected internal BackstageViewPaintStyle GetPaintStyle() {
			if(UseParentStyle) return ParentBackstageView.GetPaintStyle();
			return PaintStyle;
		}
		protected virtual BackstageViewOffice2013StyleOptions CreateOffice2013StyleOptions() {
			return new BackstageViewOffice2013StyleOptions(this);
		}
		protected virtual BackstageViewScrollController CreateScrollController() {
			return new BackstageViewScrollController(this);
		}
		[Browsable(false)]
		public AccessibleBackstageView AccessibleBackstageView {
			get {
				if(accessibleBackstageView == null) accessibleBackstageView = CreateAccessibleBackstageView();
				return accessibleBackstageView;
			}
		}
		protected virtual AccessibleBackstageView CreateAccessibleBackstageView() { return new AccessibleBackstageView(this); }
		protected override AccessibleObject CreateAccessibilityInstance() { return AccessibleBackstageView.Accessible; }
		protected override Size DefaultSize {
			get { return DefaultControlSize; }
		}
		BackstageViewDesignTimeManager disignTimeManagerCore = null;
		protected internal BackstageViewDesignTimeManager DesignTimeManager {
			get {
				if(disignTimeManagerCore == null) {
					disignTimeManagerCore = CreateDesignTimeManager();
				}
				return disignTimeManagerCore;
			}
		}
		protected virtual BackstageViewDesignTimeManager CreateDesignTimeManager() {
			return new BackstageViewDesignTimeManager(this, Site);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public BackstageViewDesignTimeManager GetDesignTimeManager() {
			return DesignTimeManager;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Size DefaultSizeCore {
			get { return DefaultSize; }
		}
		protected virtual void OnBeforeShowing() {
			UpdateViewCore();
			RaiseShowing();
		}
		protected virtual void OnShowing() {
			OnAddListener();
			ViewInfo.OnShowing();
			if(SelectedTab != null) SelectedTab.OnShowing();
			RaiseShown();
			if(!ShouldStartTransitionAnimation)
				return;
			CheckRightToLeft();
			if(IsRightToLeft)
				ViewInfo.AddTransitionAnimation(BackstageViewTransitionAnimationInfo.Direction.RightToLeft);
			else
				ViewInfo.AddTransitionAnimation(BackstageViewTransitionAnimationInfo.Direction.LeftToRight);
		}
		protected virtual void OnHidding() {
			OnRemoveListener();
			KeyTipManager.OnHidding();
			if(!Visible) return;
			ViewInfo.OnHidding();
			if(SelectedTab != null) SelectedTab.OnHidding();
			RaiseHiding();
			if(!ShouldStartTransitionAnimation)
				return;
			if(IsRightToLeft)
				ViewInfo.AddTransitionAnimation(BackstageViewTransitionAnimationInfo.Direction.LeftToRight);
			else
				ViewInfo.AddTransitionAnimation(BackstageViewTransitionAnimationInfo.Direction.RightToLeft);
			KeyTipManager.HideKeyTips();
		}
		protected virtual void OnAddListener() {
			if(Ribbon == null || Ribbon.Manager == null)
				return;
			Ribbon.Manager.AddListener(this);
		}
		protected virtual void OnRemoveListener() {
			if(Ribbon == null || Ribbon.Manager == null)
				return;
			Ribbon.Manager.RemoveListener(this);
		}
		[Browsable(false)]
		public virtual BackstageViewBaseKeyTipManager KeyTipManager {
			get {
				if(keyTipManager == null) keyTipManager = CreateKeyTipManager();
				return keyTipManager;
			}
		}
		protected virtual BackstageViewBaseKeyTipManager CreateKeyTipManager() { return new BackstageViewBaseKeyTipManager(this); }
		protected internal virtual void ShowKeyTips() {
			ViewInfo.CalcViewInfo(ClientRectangle);
			FillKeyTipItems();
			KeyTipManager.ShowKeyTips();
			ShouldShowKeyTips = true;
		}
		protected internal virtual void HideKeyTips() {
			KeyTipManager.HideKeyTips();
			KeyTipManager.ClearItems();
			ShouldShowKeyTips = false;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void FillKeyTipItems() {
			KeyTipManager.ClearItems();
			KeyTipManager.AddItems(Items);
		}
		protected void ActivateRibbonKeyTips() {
			Ribbon.ActivateKeyboardNavigation();
			Ribbon.ActivateKeyTips();
			ShouldShowRibbonKeyTips = false;
		}
		protected virtual void OnHided() {
			ViewInfo.OnHided();
			RaiseHidden();
			if(Ribbon != null) {
				Ribbon.RaiseAfterApplicationButtonContentControlHidden();
				if(ShouldShowRibbonKeyTips)
					ActivateRibbonKeyTips();
			}
			RefreshRibbon();
		}
		protected virtual void RefreshRibbon() {
			if(Ribbon != null) Ribbon.Invalidate();
		}
		#region IShowHidePreparingSupports
		void IBackstageViewControl.OnBeforeShowing() {
			OnBeforeShowing();
		}
		void IBackstageViewControl.OnShowing() {
			OnShowing();
		}
		void IBackstageViewControl.OnHiding() {
			OnHidding();
		}
		void IBackstageViewControl.OnHided() {
			OnHided();
		}
		bool IBackstageViewControl.WindowsUIStyleActive { get { return WindowsUIStyleActiveCore; } }
		#endregion
		protected virtual bool WindowsUIStyleActiveCore {
			get {
				if(DesignMode || RealStyle != BackstageViewStyle.Office2013 || !ViewInfo.IsActive) return false;
				return IsSkinCompatibleWithRibbonItems;
			}
		}
		protected internal bool IsSkinCompatibleWithRibbonItems {
			get { return ViewInfo.UseRibbonItemsWithBackstageView; }
		}
		protected internal virtual bool ShouldStartTransitionAnimation {
			get { return RealStyle == BackstageViewStyle.Office2013; }
		}
		protected override bool ProcessDialogKey(Keys keyData) {
			if(keyData == Keys.Up || keyData == Keys.Down || keyData == Keys.Tab)
				return true;
			return base.ProcessDialogKey(keyData);
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(disposing) {
				OnRemoveListener();
				foreach(BackstageViewItemBase item in Items)
					item.Dispose();
				this.GetController().RemoveClient(this);
				ToolTipController = null;
				ToolTipController.DefaultController.RemoveClientControl(this);
				if(ScrollController != null) {
					ScrollController.VScrollValueChanged -= OnVScrollValueChanged;
					ScrollController.RemoveControls(this);
					ScrollController.Dispose();
				}
				this.scrollController = null;
			}
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BackstageViewControlSelectedTabChanged")]
#endif
		public event BackstageViewItemEventHandler SelectedTabChanged {
			add { Events.AddHandler(selectedTabChanged, value); }
			remove { Events.RemoveHandler(selectedTabChanged, value); }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BackstageViewControlItemClick")]
#endif
		public event BackstageViewItemEventHandler ItemClick {
			add { Events.AddHandler(itemClick, value); }
			remove { Events.RemoveHandler(itemClick, value); }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BackstageViewControlHighlightedItemChanged")]
#endif
		public event BackstageViewItemEventHandler HighlightedItemChanged {
			add { Events.AddHandler(highlightedItemChanged, value); }
			remove { Events.RemoveHandler(highlightedItemChanged, value); }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BackstageViewControlItemPressed")]
#endif
		public event BackstageViewItemEventHandler ItemPressed {
			add { Events.AddHandler(itemPressed, value); }
			remove { Events.RemoveHandler(itemPressed, value); }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BackstageViewControlCustomDrawItem")]
#endif
		public event BackstageViewItemCustomDrawEventHandler CustomDrawItem {
			add { Events.AddHandler(customDrawItem, value); }
			remove { Events.RemoveHandler(customDrawItem, value); }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BackstageViewControlBackButtonClick")]
#endif
		public event EventHandler BackButtonClick {
			add { Events.AddHandler(backButtonClick, value); }
			remove { Events.RemoveHandler(backButtonClick, value); }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BackstageViewControlShowing")]
#endif
		public event EventHandler Showing {
			add { Events.AddHandler(showing, value); }
			remove { Events.RemoveHandler(showing, value); }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BackstageViewControlShown")]
#endif
		public event EventHandler Shown {
			add { Events.AddHandler(shown, value); }
			remove { Events.RemoveHandler(shown, value); }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BackstageViewControlHiding")]
#endif
		public event EventHandler Hiding {
			add { Events.AddHandler(hiding, value); }
			remove { Events.RemoveHandler(hiding, value); }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BackstageViewControlHidden")]
#endif
		public event EventHandler Hidden {
			add { Events.AddHandler(hidden, value); }
			remove { Events.RemoveHandler(hidden, value); }
		}
		protected internal void RaiseItemClick(BackstageViewItem item) {
			BackstageViewItemEventHandler handler = Events[itemClick] as BackstageViewItemEventHandler;
			if(handler != null)
				handler(this, new BackstageViewItemEventArgs(item));
		}
		protected internal virtual void RaiseHighlightedItemChanged(BackstageViewItem item) {
			BackstageViewItemEventHandler handler = Events[highlightedItemChanged] as BackstageViewItemEventHandler;
			if(handler != null)
				handler(this, new BackstageViewItemEventArgs(item));
		}
		protected internal virtual void RaisePressedItemChanged(BackstageViewItem item) {
			BackstageViewItemEventHandler handler = Events[itemPressed] as BackstageViewItemEventHandler;
			if(handler != null)
				handler(this, new BackstageViewItemEventArgs(item));
		}
		protected internal virtual void RaiseItemCustomDraw(BackstageViewItemCustomDrawEventArgs e) {
			BackstageViewItemCustomDrawEventHandler handler = Events[customDrawItem] as BackstageViewItemCustomDrawEventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected internal void RaiseSelectedTabChanged() {
			BackstageViewItemEventHandler handler = Events[selectedTabChanged] as BackstageViewItemEventHandler;
			if(handler != null)
				handler(this, new BackstageViewItemEventArgs(SelectedTab));
		}
		protected internal virtual void RaiseBackButtonClick() {
			EventHandler handler = Events[backButtonClick] as EventHandler;
			if(handler != null)
				handler(this, EventArgs.Empty);
		}
		protected internal virtual void RaiseShowing() {
			EventHandler handler = Events[showing] as EventHandler;
			if(handler != null)
				handler(this, EventArgs.Empty);
		}
		protected internal virtual void RaiseShown() {
			EventHandler handler = Events[shown] as EventHandler;
			if(handler != null)
				handler(this, EventArgs.Empty);
		}
		protected internal virtual void RaiseHiding() {
			EventHandler handler = Events[hiding] as EventHandler;
			if(handler != null)
				handler(this, EventArgs.Empty);
		}
		protected internal virtual void RaiseHidden() {
			EventHandler handler = Events[hidden] as EventHandler;
			if(handler != null)
				handler(this, EventArgs.Empty);
		}
		int updateCount = 0;
		public void BeginUpdate() {
			this.updateCount++;
		}
		public void CancelUpdate() {
			if(this.updateCount > 0)
				this.updateCount--;
		}
		public void EndUpdate() {
			CancelUpdate();
			if(this.updateCount == 0)
				OnPropertiesChanged();
		}
		protected internal RibbonControl GetRibbon() {
			if(Ribbon != null)
				return ribbon;
			RibbonApplicationButtonContainerControl container = Parent as RibbonApplicationButtonContainerControl;
			return container == null ? null : container.Ribbon;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsInUpdate { get { return this.updateCount > 0; } }
		ToolTipController toolTipController;
		[Category("Appearance"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonControlToolTipController"),
#endif
 DefaultValue((string)null)]
		public virtual ToolTipController ToolTipController {
			get { return toolTipController; }
			set {
				if(ToolTipController == value) return;
				if(ToolTipController != null)
					ToolTipController.RemoveClientControl(this);
				toolTipController = value;
				if(ToolTipController != null) {
					ToolTipController.DefaultController.RemoveClientControl(this);
					ToolTipController.AddClientControl(this);
				}
				else ToolTipController.DefaultController.AddClientControl(this);
			}
		}
		bool shouldShowKeyTips;
		protected internal bool ShouldShowKeyTips {
			get {
				if(ParentBackstageView == null) {
					if(Ribbon == null || !Ribbon.AllowKeyTips)
						return false;
				}
				else if(ParentBackstageView.Ribbon == null || !ParentBackstageView.Ribbon.AllowKeyTips)
					return false;
				return shouldShowKeyTips;
			}
			set {
				shouldShowKeyTips = value;
			}
		}
		protected internal bool ShouldShowRibbonKeyTips { get; set; }
		internal bool DesignModeCore { get { return DesignMode; } }
		[DefaultValue(-1), Category("Behavior")]
		public int SelectedTabIndex {
			get { return SelectedTab == null ? -1 : Items.IndexOf(SelectedTab); }
			set {
				if(SelectedTabIndex == value)
					return;
				if(value < -1)
					value = -1;
				if(value > Items.LastTabIndex)
					value = Items.LastTabIndex;
				SelectedTab = Items[value] as BackstageViewTabItem;
			}
		}
		BackstageViewTabItem selectedTab;
		public BackstageViewTabItem SelectedTab {
			get { return selectedTab; }
			set {
				if(SelectedTab == value)
					return;
				BackstageViewTabItem prev = SelectedTab;
				selectedTab = value;
				OnSelectedTabChanged(prev);
			}
		}
		protected virtual void OnSelectedTabChanged(BackstageViewTabItem item) {
			if(item != null) {
				item.ContentControl.ActiveControl = null;
				item.UpdateControlProperties();
				item.HideChildKeyTips();
				item.RaiseSelectedChanged();
			}
			if(SelectedTab != null) {
				SelectedTab.UpdateControlProperties();
				SelectedTab.RaiseSelectedChanged();
			}
			RaiseSelectedTabChanged();
			OnPropertiesChanged();
			if(isInitialize) needRaiseSelectedTabChanged = true;
			var childId = AccessibleBackstageView.GetTabItemId(SelectedTab);
			AccessibilityNotifyClients(AccessibleEvents.Selection, childId);
			AccessibilityNotifyClients(AccessibleEvents.Focus, childId);
		}
		protected internal virtual void OnButtonItemClick(BackstageViewButtonItem buttonItem) {
			if(buttonItem == null) return;
			var childId = AccessibleBackstageView.GetButtonItemId(buttonItem);
			AccessibilityNotifyClients(AccessibleEvents.Focus, childId);
		}
		protected internal virtual void OnBackButtonClick() {
			RaiseBackButtonClick();
			if(ShouldCloseRibbonControlOnBackButtonClick) CloseApplicationMenuCore();
		}
		protected internal virtual void CloseApplicationMenuCore() {
			RibbonControl ribbon = GetRibbon();
			if(ribbon != null) ribbon.HideApplicationButtonContentControl();
		}
		protected virtual bool ShouldCloseRibbonControlOnBackButtonClick {
			get { return true; }
		}
		[DefaultValue(true), Category("Appearance")]
		public bool ShowImage {
			get { return showImage; }
			set {
				if(ShowImage == value)
					return;
				showImage = value;
				OnPropertiesChanged();
			}
		}
		[DefaultValue(null), Category("Appearance"), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public Image Image {
			get { return image; }
			set {
				if(Image == value)
					return;
				image = value;
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BackstageViewControlAllowGlyphSkinning"),
#endif
 Category("Appearance"), DefaultValue(false)]
		public bool AllowGlyphSkinning {
			get { return allowGlyphSkinning; }
			set {
				if(AllowGlyphSkinning == value)
					return;
				allowGlyphSkinning = value;
				OnPropertiesChanged();
			}
		}
		[DefaultValue(-1), Category("Appearance")]
		public int GlyphToCaptionIndent {
			get { return glyphToCaptionIndent; }
			set {
				if(GlyphToCaptionIndent == value)
					return;
				glyphToCaptionIndent = value;
				OnPropertiesChanged();
			}
		}
		[DefaultValue(typeof(Padding), "0, 0, 0, 0"), Category("Appearance")]
		public Padding ItemsContentPadding {
			get { return itemsContentPadding; }
			set {
				if(ItemsContentPadding == value)
					return;
				itemsContentPadding = value;
				OnPropertiesChanged();
			}
		}
		[DefaultValue(ItemHorizontalAlignment.Default), Category("Appearance")]
		public ItemHorizontalAlignment GlyphHorizontalAlignment {
			get { return glyphHorizontalAlignment; }
			set {
				if(GlyphHorizontalAlignment == value)
					return;
				glyphHorizontalAlignment = value;
				OnPropertiesChanged();
			}
		}
		[DefaultValue(ItemHorizontalAlignment.Default), Category("Appearance")]
		public ItemHorizontalAlignment CaptionHorizontalAlignment {
			get { return captionHorizontalAlignment; }
			set {
				if(CaptionHorizontalAlignment == value)
					return;
				captionHorizontalAlignment = value;
				OnPropertiesChanged();
			}
		}
		[DefaultValue(ItemVerticalAlignment.Default), Category("Appearance")]
		public ItemVerticalAlignment GlyphVerticalAlignment {
			get { return glyphVerticalAlignment; }
			set {
				if(GlyphVerticalAlignment == value)
					return;
				glyphVerticalAlignment = value;
				OnPropertiesChanged();
			}
		}
		[DefaultValue(ItemVerticalAlignment.Default), Category("Appearance")]
		public ItemVerticalAlignment CaptionVerticalAlignment {
			get { return captionVerticalAlignment; }
			set {
				if(CaptionVerticalAlignment == value)
					return;
				captionVerticalAlignment = value;
				OnPropertiesChanged();
			}
		}
		[DefaultValue(ItemLocation.Default)]
		public ItemLocation GlyphLocation {
			get { return glyphLocation; }
			set {
				if(GlyphLocation == value)
					return;
				glyphLocation = value;
				OnPropertiesChanged();
			}
		}
		protected override void OnRightToLeftChanged(EventArgs e) {
			base.OnRightToLeftChanged(e);
			CheckRightToLeft();
		}
		bool isRightToLeft = false;
		protected internal void CheckRightToLeft() {
			if(UpdateRightToLeft()) OnPropertiesChanged();
		}
		protected internal bool UpdateRightToLeft() {
			bool rightToLeft = false;
			if(Ribbon != null) rightToLeft = WindowsFormsSettings.GetIsRightToLeft(Ribbon);
			else if(ParentBackstageView != null) rightToLeft = WindowsFormsSettings.GetIsRightToLeft(ParentBackstageView);
			else if(ParentForm != null) rightToLeft = WindowsFormsSettings.GetIsRightToLeft(ParentForm);
			else rightToLeft = WindowsFormsSettings.GetIsRightToLeft(this);
			if(rightToLeft == isRightToLeft) return false;
			this.isRightToLeft = rightToLeft;
			return true;
		}
		internal bool IsRightToLeft { get { return isRightToLeft; } }
		internal void FireBackstageViewChanged(Component component) {
			IComponentChangeService srv = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(srv != null) {
				srv.OnComponentChanged(component, null, null, null);
			}
		}
		protected virtual void OnPropertiesChanged() {
			ViewInfo.SetAppearanceDirty();
			Refresh();
		}
		protected internal virtual void OnPropertiesChangedCore() {
			OnPropertiesChanged();
		}
		[DefaultValue(null)]
		public RibbonControl Ribbon {
			get { return ribbon; }
			set {
				if(Ribbon == value)
					return;
				RibbonControl prev = Ribbon;
				this.ribbon = value;
				OnRibbonChanged(prev);
			}
		}
		protected virtual void OnRibbonChanged(RibbonControl prev) {
			if(prev != null) {
				prev.ColorSchemeChanged -= OnRibbonColorSchemeChanged;
				prev.RibbonStyleChanged -= OnRibbonStyleChanged;
			}
			if(Ribbon != null) {
				Ribbon.ColorSchemeChanged += OnRibbonColorSchemeChanged;
				Ribbon.RibbonStyleChanged += OnRibbonStyleChanged;
			}
			UpdateViewCore();
			OnPropertiesChanged();
		}
		void OnRibbonStyleChanged(object sender, EventArgs e) {
			if(ShouldUseDefferedStyleChanging) {
				OnHidding();
				ViewInfo.ShouldUsePostponedStyleChanging = true;
				return;
			}
			UpdateViewCore();
			OnPropertiesChanged();
		}
		protected virtual bool ShouldUseDefferedStyleChanging {
			get {
				if(DesignMode) return false;
				return RealStyle == BackstageViewStyle.Office2013 && Visible;
			}
		}
		void OnRibbonColorSchemeChanged(object sender, EventArgs e) {
			OnPropertiesChanged();
		}
		public RibbonControlColorScheme ColorScheme {
			get { return Ribbon != null ? Ribbon.ColorScheme : colorScheme; }
			set {
				if(ColorScheme == value)
					return;
				colorScheme = value;
				OnPropertiesChanged();
			}
		}
		[DefaultValue(-1)]
		public int LeftPaneMinWidth {
			get {
				return leftPaneMinWidth;
			}
			set {
				if(LeftPaneMinWidth == value)
					return;
				if(LeftPaneMaxWidth > 0)
					value = Math.Min(LeftPaneMaxWidth, value);
				leftPaneMinWidth = value;
				OnPropertiesChanged();
			}
		}
		[DefaultValue(-1)]
		public int LeftPaneMaxWidth {
			get { return leftPaneMaxWidth; }
			set {
				if(LeftPaneMaxWidth == value)
					return;
				if(value > -1)
					value = Math.Max(value, LeftPaneMinWidth);
				leftPaneMaxWidth = value;
				OnPropertiesChanged();
			}
		}
		public override void Refresh() {
			if(IsInUpdate)
				return;
			ViewInfo.IsReady = false;
			base.Refresh();
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public BackstageViewControlItemCollecton Items {
			get {
				if(items == null)
					items = CreateItems();
				return items;
			}
		}
		protected virtual BackstageViewControlItemCollecton CreateItems() {
			return new BackstageViewControlItemCollecton(this);
		}
		protected internal BackstageViewControlHandler Handler {
			get {
				if(handler == null)
					handler = CreateHandler();
				return handler;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public BackstageViewControlHandler GetHandler() {
			return Handler;
		}
		protected virtual BackstageViewControlHandler CreateHandler() {
			if(ShouldUseOffice2013ControlStyle)
				return new Office2013BackstageViewControlHandler(this);
			return new BackstageViewControlHandler(this);
		}
		protected internal BackstageViewInfo ViewInfo {
			get {
				if(viewInfo == null)
					viewInfo = CreateViewInfo();
				return viewInfo;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public BackstageViewInfo GetViewInfo() {
			return ViewInfo;
		}
		protected virtual BackstageViewInfo CreateViewInfo() {
			if(ShouldUseOffice2013ControlStyle)
				return new Office2013BackstageViewInfo(this);
			return new BackstageViewInfo(this);
		}
		protected internal BackstageViewPainter Painter {
			get {
				if(painter == null)
					painter = CreatePainter();
				return painter;
			}
		}
		protected virtual BackstageViewPainter CreatePainter() {
			if(ShouldUseOffice2013ControlStyle)
				return new Office2013BackstageViewPainter();
			return new BackstageViewPainter();
		}
		protected internal virtual bool ShouldUseOffice2013ControlStyle {
			get {
				if(Ribbon != null) {
					Form ownerForm = GetOwnerForm();
					if(ownerForm != null && ownerForm.IsMdiChild) return false;
					return Ribbon.RibbonStyle == RibbonControlStyle.Office2013 || Ribbon.IsOfficeTablet;
				}
				return Style == BackstageViewStyle.Office2013 || Style == BackstageViewStyle.Office2013;
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BackstageViewControlPaintStyle"),
#endif
 Category("Appearance"), DefaultValue(BackstageViewPaintStyle.Default)]
		public BackstageViewPaintStyle PaintStyle {
			get {
				return paintStyle;
			}
			set {
				if(PaintStyle == value) return;
				paintStyle = value;
				OnPaintStyleChanged();
			}
		}
		void OnPaintStyleChanged() {
			OnPropertiesChanged();
			foreach(BackstageViewItemBase item in Items) {
				if(item.HasClientBackstageView) ((BackstageViewTabItem)item).RefreshContentControl();
			}
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			CheckRightToLeft();
			ViewInfo.SetAppearanceDirty();
		}
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			if(IsInUpdate)
				return;
			ViewInfo.IsReady = false;
			CheckViewInfo();
			CheckScrollPos();
			Refresh();
		}
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			CheckViewInfo();
			using(GraphicsCache cache = new GraphicsCache(e)) {
				Painter.Draw(cache, ViewInfo);
			}
		}
		protected virtual void CheckViewInfo() {
			if(!ViewInfo.IsReady) {
				ViewInfo.CalcViewInfo(ClientRectangle);
				UpdateScrollers();
			}
		}
		protected bool IsVScrollVisible { get { return ViewInfo.IsVScrollVisible; } }
		protected virtual void CheckScrollPos() {
			if(ScrollerPosition != 0 && !IsVScrollVisible) ScrollerPosition = 0;
		}
		protected virtual void UpdateScrollers() {
			if(DesignMode) ScrollController.VScroll.Enabled = false;
			else ScrollController.VScroll.Enabled = true;
			ScrollController.IsRightToLeft = IsRightToLeft;
			ScrollController.VScrollVisible = ViewInfo.IsVScrollVisible;
			ScrollController.ClientRect = ViewInfo.RightPaneBounds;
			ScrollController.VScroll.LookAndFeel.ParentLookAndFeel = GetController().LookAndFeel;
			if(ScrollController.VScrollVisible) ScrollController.VScrollArgs = ViewInfo.CalcVScrollArgs();
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BackstageViewControlStyle"),
#endif
 Category("Appearance"), DefaultValue(BackstageViewStyle.Default)]
		public BackstageViewStyle Style {
			get { return style; }
			set {
				if(style == value) return;
				style = value;
				OnViewStyleChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BackstageViewControlOffice2013StyleOptions"),
#endif
 Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public BackstageViewOffice2013StyleOptions Office2013StyleOptions {
			get { return office2013StyleOptions; }
			set {
				if(Office2013StyleOptions == value)
					return;
				office2013StyleOptions = value;
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BackstageViewControlBackstageViewShowRibbonItems"),
#endif
 Category("Appearance"), DefaultValue(BackstageViewShowRibbonItems.Default), System.ComponentModel.Editor(typeof(DevExpress.Utils.Editors.AttributesEditor), typeof(UITypeEditor))]
		public BackstageViewShowRibbonItems BackstageViewShowRibbonItems {
			get { return backstageViewShowRibbonItems; }
			set {
				if(backstageViewShowRibbonItems == value) return;
				backstageViewShowRibbonItems = value;
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BackstageViewControlImages"),
#endif
 DefaultValue(null), TypeConverter(typeof(DevExpress.Utils.Design.ImageCollectionImagesConverter))]
		public virtual object Images {
			get { return images; }
			set { images = value; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BackstageViewControlController"),
#endif
 DefaultValue(null)]
		public virtual BarAndDockingController Controller {
			get { return controller; }
			set {
				if(Controller == value) return;
				GetControllerInternal().RemoveClient(this);
				this.controller = value;
				GetControllerInternal().AddClient(this);
				OnControllerChanged();
			}
		}
		protected virtual void OnControllerChanged() {
			if(!this.controllerDisposing)
				Refresh();
		}
		protected virtual void OnViewStyleChanged() {
			if(Ribbon != null)
				Ribbon.ResetApplicationButtonContentControlCache();
			UpdateViewCore();
			OnPropertiesChanged();
		}
		protected internal BackstageViewStyle RealStyle {
			get { return realStyle; }
		}
		protected internal virtual void UpdateViewCore() {
			UpdatePainter();
			UpdateViewInfo();
			UpdateHandler();
			CheckRightToLeft();
			CheckViewInfo();
			UpdateRealStyle();
		}
		protected virtual void UpdatePainter() {
			painter = CreatePainter();
		}
		protected virtual void UpdateViewInfo() {
			viewInfo = CreateViewInfo();
		}
		protected virtual void UpdateHandler() {
			handler = CreateHandler();
		}
		protected virtual void UpdateRealStyle() {
			realStyle = ShouldUseOffice2013ControlStyle ? BackstageViewStyle.Office2013 : BackstageViewStyle.Default;
		}
		BarAndDockingController GetControllerInternal() {
			return controller != null ? controller : BarAndDockingController.Default;
		}
		public virtual BarAndDockingController GetController() {
			if(Controller != null && !Controller.Disposing)
				return Controller;
			if(Ribbon != null)
				return Ribbon.GetController();
			return BarAndDockingController.Default;
		}
		#region IBarAndDockingControllerClient Members
		void IBarAndDockingControllerClient.OnControllerChanged(BarAndDockingController controller) {
			ViewInfo.SetAppearanceDirty();
			Refresh();
		}
		bool controllerDisposing = false;
		void IBarAndDockingControllerClient.OnDisposed(BarAndDockingController controller) {
			this.controllerDisposing = true;
			try {
				Controller = null;
			}
			finally {
				this.controllerDisposing = false;
			}
		}
		#endregion
		protected internal virtual void OnItemChanged(BackstageViewItemBase item) {
			Refresh();
		}
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
			bool res = base.ProcessCmdKey(ref msg, keyData);
			if(res)
				return res;
			Handler.ProcessCmdKey(ref msg, keyData);
			return res;
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			if(e.Handled) return;
			Handler.OnKeyDown(e);
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(e);
			try {
				base.OnMouseDown(ee);
				if(ee.Handled) return;
				Handler.OnMouseDown(ee);
				if(Handler.ViewInfo.PressedItem is BackstageViewButtonItemViewInfo)
					OnButtonItemClick(ViewInfo.PressedItem.Item as BackstageViewButtonItem);
			}
			finally {
				ee.Sync();
			}
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(e);
			try {
				base.OnMouseUp(ee);
				if(ee.Handled) return;
				Handler.OnMouseUp(ee);
			}
			finally {
				ee.Sync();
			}
		}
		protected override void OnMouseDoubleClick(MouseEventArgs e) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(e);
			try {
				base.OnMouseDoubleClick(ee);
				if(ee.Handled) return;
				Handler.OnMouseDoubleClick(ee);
			}
			finally {
				ee.Sync();
			}
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			CheckViewInfo();
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(e);
			try {
				base.OnMouseMove(ee);
				if(ee.Handled) return;
				Handler.OnMouseMove(ee);
			}
			finally {
				ee.Sync();
			}
		}
		protected override void OnMouseLeave(EventArgs e) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(this, e);
			try {
				base.OnMouseLeave(ee);
				if(ee.Handled) return;
				Handler.OnMouseLeave(ee);
			}
			finally {
				ee.Sync();
			}
		}
		protected override void OnMouseEnter(EventArgs e) {
			CheckViewInfo();
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(this, e);
			try {
				base.OnMouseEnter(ee);
				if(ee.Handled) return;
				Handler.OnMouseEnter(ee);
			}
			finally {
				ee.Sync();
			}
		}
		protected override void OnDragEnter(DragEventArgs e) {
			if(!DesignMode)
				base.OnDragEnter(e);
			else
				Handler.OnDragEnter(e);
		}
		protected override void OnDragLeave(EventArgs e) {
			if(!DesignMode)
				base.OnDragLeave(e);
			else
				Handler.OnDragLeave(e);
		}
		protected override void OnDragOver(DragEventArgs e) {
			if(!DesignMode)
				base.OnDragOver(e);
			else
				Handler.OnDragOver(e);
		}
		protected override void OnDragDrop(DragEventArgs e) {
			if(!DesignMode)
				base.OnDragDrop(e);
			else
				Handler.OnDragDrop(e);
		}
		protected override void OnGiveFeedback(GiveFeedbackEventArgs e) {
			if(!DesignMode)
				base.OnGiveFeedback(e);
			else
				Handler.OnGiveFeedback(e);
		}
		protected override void OnQueryContinueDrag(QueryContinueDragEventArgs e) {
			if(!DesignMode)
				base.OnQueryContinueDrag(e);
			else
				Handler.OnQueryContinueDrag(e);
		}
		protected override void OnMouseWheel(MouseEventArgs e) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(this, e);
			try {
				Handler.OnMouseWheel(ee);
			}
			finally {
				ee.Sync();
			}
		}
		protected internal void ForceCreateHandle() {
			if(IsHandleCreated) return;
			CreateHandle();
		}
		protected internal Form GetOwnerForm() {
			if(Ribbon == null) return FindForm();
			return Ribbon.FindForm();
		}
		#region IToolTipControlClient Members
		ToolTipControlInfo IToolTipControlClient.GetObjectInfo(Point point) {
			return ViewInfo.GetToolTipInfo(point);
		}
		bool IToolTipControlClient.ShowToolTips {
			get { return true; }
		}
		#endregion
		#region ISupportXtraAnimation Members
		bool ISupportXtraAnimation.CanAnimate {
			get { return true; }
		}
		Control ISupportXtraAnimation.OwnerControl {
			get { return this; }
		}
		#endregion
		#region IXtraAnimationListener Members
		void IXtraAnimationListener.OnAnimation(BaseAnimationInfo info) {
			ViewInfo.OnAnimation(info);
		}
		void IXtraAnimationListener.OnEndAnimation(BaseAnimationInfo info) {
			ViewInfo.OnEndAnimation(info);
			if(ShouldShowKeyTips) {
				ShowKeyTips();
			}
		}
		#endregion
		#region IPrefilterMessageListener Members
		BarManagerHookResult IBarManagerListener.PreFilterMessage(object owner, int Msg, Control wnd, IntPtr HWnd, IntPtr WParam, IntPtr LParam) {
			if(!AllowProcessMessage()) return BarManagerHookResult.NotProcessed;
			bool isProcessed = ViewInfo.PrefilterMessageController.PreFilterMessage(Msg, HWnd, WParam, LParam, owner, wnd);
			return isProcessed ? BarManagerHookResult.ProcessedExit : BarManagerHookResult.NotProcessed;
		}
		BarManagerHookResult IBarManagerListener.InternalPreFilterMessage(object owner, int Msg, Control wnd, IntPtr HWnd, IntPtr WParam, IntPtr LParam) {
			if(!AllowProcessMessage()) return BarManagerHookResult.NotProcessed;
			bool isProcessed = ViewInfo.PrefilterMessageController.InternalPreFilterMessage(Msg, HWnd, WParam, LParam, owner, wnd);
			return isProcessed ? BarManagerHookResult.ProcessedExit : BarManagerHookResult.NotProcessed;
		}
		#endregion
		protected virtual bool AllowProcessMessage() {
			if(ViewInfo == null || DesignMode) return false;
			return ViewInfo.PrefilterMessageController.AllowProcessMessage;
		}
		protected internal virtual void OnItemRemoved(BackstageViewItemBase item) {
			Refresh();
		}
		protected override void WndProc(ref Message msg) {
			bool handled = false;
			switch(msg.Msg) {
				case MSG.WM_NCHITTEST:
					handled = OnWmNcHitTest(ref msg);
					break;
			}
			if(handled)
				return;
			base.WndProc(ref msg);
			CodedUISupport.CodedUIMessagesHandler.ProcessCodedUIMessage(ref msg, this);
		}
		protected virtual bool OnWmNcHitTest(ref Message msg) {
			return ViewInfo.OnWmNcHitTest(ref msg);
		}
		protected internal bool AllowMouseClick(Control control, Point mousePosition) {
			return ViewInfo.AllowMouseClick(control, mousePosition);
		}
		#region ISizableControl
		void ISizableControl.OnBeginSizing() {
			OnBeginSizingCore();
		}
		void ISizableControl.OnEndSizing() {
			OnEndSizingCore();
		}
		Control ISizableControl.Owner { get { return this; } }
		Form ISizableControl.OwnerForm { get { return this.Parent as Form; } }
		bool ISizableControl.CanUpdateSize(MouseEventArgs e) {
			return CanUpdateSizeCore(e);
		}
		bool ISizableControl.CanUpdateBounds(int width, int height) {
			return CanUpdateBoundsCore(width, height);
		}
		#endregion
		protected internal virtual void OnBeginSizingCore() {
			ViewInfo.OnBeginSizingCore();
		}
		protected internal virtual void OnEndSizingCore() {
			ViewInfo.OnEndSizingCore();
			Invalidate();
		}
		protected internal virtual bool CanUpdateBoundsCore(int width, int height) {
			return ViewInfo.CanUpdateBoundsCore(width, height);
		}
		protected internal virtual bool CanUpdateSizeCore(MouseEventArgs e) {
			return ViewInfo.CanUpdateSizeCore(e);
		}
		protected internal virtual void HidePopups() {
			HideKeyTips();
		}
		void IKeyTipsOwnerControl.ShowKeyTips() {
			ShowKeyTips();
		}
		bool IKeyTipsOwnerControl.ShoulShowKeyTips {
			get { return ShouldShowKeyTips; }
			set { ShouldShowKeyTips = value; }
		}
		#region ISupportInitialize
		void ISupportInitialize.BeginInit() {
			isInitialize = true;
		}
		void ISupportInitialize.EndInit() {
			isInitialize = false;
			if(needRaiseSelectedTabChanged)
				RaiseSelectedTabChanged();
			if(GetPaintStyle() == BackstageViewPaintStyle.Flat) {
				ViewInfo.SetAppearanceDirty();
			}
		}
		#endregion
	}
	public interface IBackstageViewPrefilterMessageController : IDisposable {
		bool AllowProcessMessage { get; }
		bool InternalPreFilterMessage(int Msg, IntPtr HWnd, IntPtr WParam, IntPtr LParam, object owner, Control wnd);
		bool PreFilterMessage(int Msg, IntPtr HWnd, IntPtr WParam, IntPtr LParam, object owner, Control wnd);
	}
	public class BackstageViewPrefilterMessageController : IBackstageViewPrefilterMessageController {
		BackstageViewControl backstageView;
		const int WM_MINCLICK = 0x201, WM_MAXCLICK = 0x208, WM_MINNCCLICK = 0xA1, WM_MAXNCCLICK = 0x0a9;
		public BackstageViewPrefilterMessageController(BackstageViewControl backstageView) {
			this.backstageView = backstageView;
		}
		public virtual bool AllowProcessMessage {
			get { return true; }
		}
		public virtual bool InternalPreFilterMessage(int Msg, IntPtr HWnd, IntPtr WParam, IntPtr LParam, object owner, Control wnd) {
			if(Msg == MSG.WM_LBUTTONUP || Msg == MSG.WM_LBUTTONDOWN) {
				backstageView.HideKeyTips();
				return false;
			}
			RibbonBarManager manager = owner as RibbonBarManager;
			Control control = wnd;
			if((Msg >= WM_MINCLICK && Msg <= WM_MAXCLICK) || (Msg >= WM_MINNCCLICK && Msg <= WM_MAXNCCLICK)) {
				if(manager.SelectionInfo.internalFocusLock == 0 && !manager.IsDragging) {
					backstageView.HideKeyTips();
					return false;
				}
			}
			if(Msg == MSG.WM_ACTIVATEAPP) {
				backstageView.HideKeyTips();
				return false;
			}
			return false;
		}
		public virtual bool PreFilterMessage(int Msg, IntPtr HWnd, IntPtr WParam, IntPtr LParam, object owner, Control wnd) {
			if(Msg == MSG.WM_SYSKEYUP) {
				Keys keyData = ((Keys)WParam.ToInt32());
				if(keyData == Keys.Menu) {
					TryActivateKeyTipMode();
					return true;
				}
				if(backstageView.KeyTipManager.Show) {
					backstageView.HideKeyTips();
					return true;
				}
			}
			if(Msg == MSG.WM_CHAR) {
				if(backstageView.ShouldShowKeyTips) {
					char ch = (char)WParam.ToInt32();
					if(backstageView.KeyTipManager.Show && (char.IsLetter(ch) || char.IsNumber(ch))) {
						backstageView.KeyTipManager.AddChar(ch);
					}
				}
				return true;
			}
			if(Msg == MSG.WM_KEYDOWN) {
				Keys keyDataSecond = ((Keys)WParam.ToInt32());
				if(keyDataSecond == Keys.Escape) {
					if(backstageView.ShouldShowKeyTips) {
						backstageView.ShouldShowRibbonKeyTips = true;
					}
					((RibbonHandler)backstageView.Ribbon.Handler).ShowApplicationButtonPopup();
					backstageView.HideKeyTips();
					return true;
				}
			}
			return false;
		}
		protected internal virtual void TryActivateKeyTipMode() {
			backstageView.ShouldShowKeyTips = !backstageView.ShouldShowKeyTips;
			if(backstageView.ViewInfo.ShouldActivateKeyTips)
				backstageView.ShowKeyTips();
			else backstageView.HideKeyTips();
		}
		#region IDisposable
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		protected virtual void Dispose(bool disposing) { }
	}
	public class Office2013StyleBackstageViewPrefilterMessageController : BackstageViewPrefilterMessageController {
		BackstageViewControl backstageView;
		BackstageViewSizingManager sizingManager;
		AeroSnapShortcutManager shortcutManager;
		public Office2013StyleBackstageViewPrefilterMessageController(BackstageViewControl backstageView)
			: base(backstageView) {
			this.backstageView = backstageView;
			this.sizingManager = new BackstageViewSizingManager(backstageView);
			this.shortcutManager = new AeroSnapShortcutManager(backstageView);
		}
		public override bool AllowProcessMessage {
			get { return Form.ActiveForm is RibbonOffice2013BackstageViewContainerControl.ContentPopupForm; }
		}
		public override bool InternalPreFilterMessage(int Msg, IntPtr HWnd, IntPtr WParam, IntPtr LParam, object owner, Control wnd) {
			bool res = base.InternalPreFilterMessage(Msg, HWnd, WParam, LParam, owner, wnd);
			ShortcutManager.ResetResult();
			switch(Msg) {
				case MSG.WM_MOVE:
					OnWmMove(Msg, WParam);
					break;
				case MSG.WM_LBUTTONDOWN:
					res = OnWmLButtonDown(WParam, LParam);
					break;
				case MSG.WM_LBUTTONUP:
					res = OnWmLButtonUp(WParam, LParam);
					break;
				case MSG.WM_MOUSEMOVE:
					res = OnWmMouseMove(WParam, LParam);
					break;
				case MSG.WM_LBUTTONDBLCLK:
					res = OnWmLButtonDblClk(WParam, LParam);
					break;
			}
			return res;
		}
		public override bool PreFilterMessage(int Msg, IntPtr HWnd, IntPtr WParam, IntPtr LParam, object owner, Control wnd) {
			bool res = base.PreFilterMessage(Msg, HWnd, WParam, LParam, owner, wnd);
			ShortcutManager.ResetResult();
			switch(Msg) {
				case MSG.WM_KEYDOWN:
					res = OnWmKeyDown(WParam);
					break;
				case MSG.WM_KEYUP:
					res = OnWmKeyUp(WParam);
					break;
			}
			return res;
		}
		protected virtual bool OnWmLButtonDblClk(IntPtr WParam, IntPtr LParam) {
			DXMouseEventArgs e = CreateDXMouseEventArgs(MouseButtons.Left, LParam);
			if(BackstageView.ViewInfo.CanDoFullScreen(e.Location))
				ShortcutManager.OnMouseDblClk();
			return ShortcutManager.GetResult();
		}
		protected virtual void OnWmMove(int Msg, IntPtr WParam) {
			if(BackstageView.Ribbon == null) return;
			RibbonForm form = BackstageView.Ribbon.ViewInfo.Form;
			if(form == null) return;
			if(form.WindowState == FormWindowState.Maximized && !form.LockSizing && !BackstageView.ViewInfo.SizingMode) {
				LockRibbonSizing(form);
			}
			if(form.WindowState == FormWindowState.Maximized && form.LockSizing) {
				UnlockRibbonSizing(form);
			}
		}
		protected void LockRibbonSizing(RibbonForm form) {
			if(BackstageView.Ribbon == null) return;
			Rectangle workingArea = Screen.GetWorkingArea(BackstageView.Ribbon);
			if(form.Size.Width >= workingArea.Width || form.Size.Height >= workingArea.Height)
				return;
			Form popupForm = BackstageView.Parent as Form;
			if(popupForm == null)
				return;
			popupForm.Region = BackstageView.Region = null;
			form.LockSizingCore();
			form.SuspendLayout();
			var opacity = form.Opacity;
			try {
				form.Opacity = 0;
				form.Refresh();
			}
			finally {
				form.Opacity = opacity;
			}
			ShortcutManager.SyncFormState();
		}
		protected void UnlockRibbonSizing(RibbonForm form) {
			form.ResumeLayout(true);
			form.UnlockSizingCore();
		}
		protected virtual bool OnWmLButtonDown(IntPtr WParam, IntPtr LParam) {
			DXMouseEventArgs e = CreateDXMouseEventArgs(MouseButtons.Left, LParam);
			SizingManager.OnMouseDown(e);
			return SizingManager.IsActive;
		}
		protected virtual bool OnWmLButtonUp(IntPtr WParam, IntPtr LParam) {
			DXMouseEventArgs e = CreateDXMouseEventArgs(MouseButtons.Left, LParam);
			SizingManager.OnMouseUp(e);
			return SizingManager.IsActive;
		}
		protected virtual bool OnWmMouseMove(IntPtr WParam, IntPtr LParam) {
			DXMouseEventArgs e = CreateDXMouseEventArgs(MouseButtons.Left, LParam);
			SizingManager.OnMouseMove(e);
			return SizingManager.IsActive;
		}
		protected DXMouseEventArgs CreateDXMouseEventArgs(MouseButtons mb, IntPtr param) {
			Point pt = backstageView.PointToClient(param.PointFromLParam());
			return new DXMouseEventArgs(mb, 1, pt.X, pt.Y, 0);
		}
		protected virtual bool OnWmKeyDown(IntPtr WParam) {
			Keys key = (Keys)WinAPIHelper.GetInt(WParam);
			ShortcutManager.OnKeyDown(key);
			return ShortcutManager.GetResult();
		}
		protected virtual bool OnWmKeyUp(IntPtr WParam) {
			Keys key = (Keys)WinAPIHelper.GetInt(WParam);
			ShortcutManager.OnKeyUp(key);
			return ShortcutManager.GetResult();
		}
		protected override void Dispose(bool disposing) {
			this.backstageView = null;
		}
		public BackstageViewSizingManager SizingManager { get { return sizingManager; } }
		public BackstageViewControl BackstageView { get { return backstageView; } }
		public AeroSnapShortcutManager ShortcutManager { get { return shortcutManager; } }
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class BackstageViewOffice2013StyleOptions {
		Color headerBackColor;
		int leftPaneContentVerticalOffset, rightPaneContentVerticalOffset;
		BackstageViewControl backstageView;
		public BackstageViewOffice2013StyleOptions(BackstageViewControl backstageView) {
			this.headerBackColor = Color.Empty;
			this.leftPaneContentVerticalOffset = LeftPaneContentVerticalOffsetDefault;
			this.rightPaneContentVerticalOffset = RightPaneContentVerticalOffsetDefault;
			this.backstageView = backstageView;
		}
		[ DefaultValue(LeftPaneContentVerticalOffsetDefault)]
		public int LeftPaneContentVerticalOffset {
			get { return leftPaneContentVerticalOffset; }
			set {
				if(LeftPaneContentVerticalOffset == value)
					return;
				if(value < 0)
					value = 0;
				leftPaneContentVerticalOffset = value;
				OnPropertiesChanged();
			}
		}
		[ DefaultValue(RightPaneContentVerticalOffsetDefault)]
		public int RightPaneContentVerticalOffset {
			get { return rightPaneContentVerticalOffset; }
			set {
				if(RightPaneContentVerticalOffset == value)
					return;
				if(value < 0)
					value = 0;
				rightPaneContentVerticalOffset = value;
				OnPropertiesChanged();
			}
		}
		public Color HeaderBackColor {
			get { return headerBackColor; }
			set {
				if(HeaderBackColor == value)
					return;
				headerBackColor = value;
				OnPropertiesChanged();
			}
		}
		protected virtual bool ShouldSerializeHeaderBackColor() { return HeaderBackColor != Color.Empty; }
		protected virtual void ResetHeaderBackColor() { HeaderBackColor = Color.Empty; }
		protected virtual void OnPropertiesChanged() {
			BackstageView.OnPropertiesChangedCore();
		}
		public const int LeftPaneContentVerticalOffsetDefault = 70;
		public const int RightPaneContentVerticalOffsetDefault = 62;
		protected BackstageViewControl BackstageView { get { return backstageView; } }
	}
	public enum BackstageViewStyle {
		Default,
		Office2010,
		Office2013
	}
	[Flags]
	public enum BackstageViewShowRibbonItems {
		None = 0,
		Default = 1,
		FormButtons = 2,
		Title = 4,
		PageHeaderItems = 8,
		All = FormButtons | Title | PageHeaderItems
	}
	[ToolboxItem(false), DesignTimeVisible(false)]
	public class BackstageViewItemBase : Component, ICloneable, ISupportRibbonKeyTip {
		BackstageViewControl control;
		string name;
		bool visible;
		int firstIndex;
		string userKeyTip;
		string itemKeyTip;
		AppearanceObject appearance, appearanceHover, appearanceDisabled;
		public BackstageViewItemBase() {
			this.visible = true;
			firstIndex = 0;
			userKeyTip = itemKeyTip = string.Empty;
			this.appearance = CreateAppearance();
			this.appearanceHover = CreateAppearance();
			this.appearanceDisabled = CreateAppearance();
		}
		protected virtual AppearanceObject CreateAppearance() {
			AppearanceObject res = new AppearanceObject();
			res.Changed += new EventHandler(OnAppearanceChanged);
			return res;
		}
		protected virtual void OnAppearanceChanged(object sender, EventArgs e) {
			OnItemChanged();
		}
		internal void ResetAppearance() { Appearance.Reset(); }
		internal bool ShouldSerializeAppearance() { return Appearance != null && Appearance.Options != AppearanceOptions.Empty; }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BackstageViewItemBaseAppearance"),
#endif
 Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject Appearance {
			get { return appearance; }
		}
		internal void ResetAppearanceHover() { AppearanceHover.Reset(); }
		internal bool ShouldSerializeAppearanceHover() { return AppearanceHover != null && AppearanceHover.Options != AppearanceOptions.Empty; }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BackstageViewItemBaseAppearance"),
#endif
 Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject AppearanceHover {
			get { return appearanceHover; }
		}
		internal void ResetAppearanceDisabled() { AppearanceDisabled.Reset(); }
		internal bool ShouldSerializeAppearanceDisabled() { return AppearanceDisabled != null && AppearanceDisabled.Options != AppearanceOptions.Empty; }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BackstageViewItemBaseAppearance"),
#endif
 Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject AppearanceDisabled {
			get { return appearanceDisabled; }
		}
		internal virtual bool IsTabItem { get { return false; } }
		internal virtual bool IsSeparator { get { return false; } }
		[Browsable(false)]
		public string Name {
			get {
				if(Site == null)
					return name;
				return Site.Name;
			}
			set {
				if(value == null)
					value = string.Empty;
				name = value;
			}
		}
		[Browsable(false)]
		public BackstageViewControl Control { get { return control; } }
		internal void SetControlCore(BackstageViewControl control) {
			this.control = control;
		}
		internal virtual void SetControl(BackstageViewControl control) {
			SetControlCore(control);
			OnItemChanged();
		}
		protected virtual void OnItemChanged() {
			if(Control != null)
				Control.OnItemChanged(this);
		}
		public object Clone() {
			BackstageViewItemBase item = CloneItemCore();
			item.Assign(this);
			return item;
		}
		protected virtual BackstageViewItemBase CloneItemCore() { return new BackstageViewItemBase(); }
		public virtual void Assign(BackstageViewItemBase item) {
			this.visible = item.visible;
		}
		[ DefaultValue(true), Category("Behavior")]
		public virtual bool Visible {
			get { return visible; }
			set {
				if(Visible == value) return;
				visible = value;
				OnVisibleChanged();
			}
		}
		protected internal bool IsVisible { get { return Visible || DesignMode; } }
		protected virtual void OnVisibleChanged() {
			OnItemChanged();
		}
		protected internal virtual BackstageViewItemBaseViewInfo GetViewInfo() {
			foreach(BackstageViewItemBaseViewInfo info in Control.ViewInfo.Items) {
				if(info.Item == this) return info;
			}
			return null;
		}
		protected virtual string ItemCaptionCore { get { return string.Empty; } }
		protected virtual bool IsCommandItemCore { get { return true; } }
		#region ISupportRibbonKeyTip
		string ISupportRibbonKeyTip.ItemCaption { get { return ItemCaptionCore; } }
		string ISupportRibbonKeyTip.ItemKeyTip { get { return itemKeyTip; } set { itemKeyTip = value; } }
		string ISupportRibbonKeyTip.ItemUserKeyTip {
			get { return userKeyTip; }
			set {
				if(value == null)
					value = string.Empty;
				else
					value = value.ToUpper();
				if(((ISupportRibbonKeyTip)this).ItemUserKeyTip == value)
					return;
				userKeyTip = value;
			}
		}
		protected virtual bool KeyTipEnabledCore { get { return false; } }
		bool ISupportRibbonKeyTip.KeyTipEnabled {
			get { return KeyTipEnabledCore; }
		}
		Point ISupportRibbonKeyTip.ShowPoint { get { return GetPoint(); } }
		protected virtual Point GetPoint() { return new Point(); }
		ContentAlignment ISupportRibbonKeyTip.Alignment {
			get { return ContentAlignment.MiddleCenter; }
		}
		int ISupportRibbonKeyTip.FirstIndex { get { return firstIndex; } set { firstIndex = value; } }
		void ISupportRibbonKeyTip.Click() {
			KeyTipItemClick();
			Control.KeyTipManager.HideKeyTips();
		}
		protected virtual void KeyTipItemClick() {
		}
		bool ISupportRibbonKeyTip.HasDropDownButton { get { return HasDropDownButtonCore; } }
		protected virtual bool HasDropDownButtonCore { get { return false; } }
		bool ISupportRibbonKeyTip.KeyTipVisible { get { return KeyTipVisible; } }
		protected virtual bool KeyTipVisible {
			get {
				if(GetViewInfo() == null || Control == null)
					return false;
				return Control.ClientRectangle.Contains(GetViewInfo().Bounds);
			}
		}
		bool ISupportRibbonKeyTip.IsCommandItem { get { return IsCommandItemCore; } }
		#endregion
		protected internal virtual bool HasClientBackstageView {
			get { return false; }
		}
		protected internal virtual List<BackstageViewControl> GetClientBackstageViewControls() {
			return new List<BackstageViewControl>();
		}
	}
	public class BackstageViewItemSeparator : BackstageViewItemBase {
		protected override BackstageViewItemBase CloneItemCore() {
			return new BackstageViewItemSeparator();
		}
		internal override bool IsSeparator { get { return true; } }
		protected override bool KeyTipVisible {
			get { return false; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public override AppearanceObject AppearanceDisabled { get { return base.AppearanceDisabled; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public override AppearanceObject AppearanceHover { get { return base.AppearanceHover; } }
	}
	[
	Designer("DevExpress.XtraBars.Ribbon.Design.BackstageViewItemDesigner, " + AssemblyInfo.SRAssemblyBarsDesign, typeof(IDesigner)),
	SmartTagAction(typeof(BackstageViewItemDesignTimeActionsProvider), "AddCommand", "Add Command"),
	SmartTagAction(typeof(BackstageViewItemDesignTimeActionsProvider), "AddTab", "Add Tab"),
	SmartTagAction(typeof(BackstageViewItemDesignTimeActionsProvider), "AddSeparator", "Add Separator"),
	]
	public class BackstageViewItem : BackstageViewItemBase {
		private static readonly object itemHover = new object();
		private static readonly object itemPressed = new object();
		string caption;
		Image glyph, glyphDisabled, glyphHover, glyphPressed;
		int imageIndex, imageIndexDisabled, imageIndexHover, imageIndexPressed;
		SuperToolTip superTip;
		bool enabled;
		bool allowHtmlString;
		DefaultBoolean allowGlyphSkinning;
		ItemLocation glyphLocation;
		ItemHorizontalAlignment glyphHorizontalAlignment, captionHorizontalAlignment;
		ItemVerticalAlignment glyphVerticalAlignment, captionVerticalAlignment;
		object tag;
		public BackstageViewItem() {
			this.enabled = true;
			this.glyphLocation = ItemLocation.Default;
			this.glyphHorizontalAlignment = ItemHorizontalAlignment.Default;
			this.glyphVerticalAlignment = ItemVerticalAlignment.Default;
			this.captionHorizontalAlignment = ItemHorizontalAlignment.Default;
			this.captionVerticalAlignment = ItemVerticalAlignment.Default;
			this.imageIndex = -1;
			this.imageIndexDisabled = -1;
			this.imageIndexHover = -1;
			this.imageIndexPressed = -1;
			this.allowGlyphSkinning = DefaultBoolean.Default;
		}
		[
		Editor("DevExpress.XtraEditors.Design.ToolTipContainerUITypeEditor, " + AssemblyInfo.SRAssemblyEditorsDesign, typeof(System.Drawing.Design.UITypeEditor)), Category("Appearance"),
		Localizable(true), SmartTagProperty("Super Tip", "Appearance")
		]
		public virtual SuperToolTip SuperTip {
			get { return superTip; }
			set { superTip = value; }
		}
		[ DefaultValue(null), Category("Data"),
	   Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor)), TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter))]
		public object Tag {
			get { return tag; }
			set { tag = value; }
		}
		protected virtual bool ShouldSerializeSuperTip() { return SuperTip != null && !SuperTip.IsEmpty; }
		public virtual void ResetSuperTip() { SuperTip = null; }
		[DefaultValue(ItemHorizontalAlignment.Default), Category("Appearance")]
		public ItemHorizontalAlignment GlyphHorizontalAlignment {
			get { return glyphHorizontalAlignment; }
			set {
				if(GlyphHorizontalAlignment == value)
					return;
				glyphHorizontalAlignment = value;
				OnItemChanged();
			}
		}
		[DefaultValue(ItemHorizontalAlignment.Default), Category("Appearance"), SmartTagProperty("Caption Horizontal Alignment", "Appearance")]
		public ItemHorizontalAlignment CaptionHorizontalAlignment {
			get { return captionHorizontalAlignment; }
			set {
				if(CaptionHorizontalAlignment == value)
					return;
				captionHorizontalAlignment = value;
				OnItemChanged();
			}
		}
		[DefaultValue(ItemVerticalAlignment.Default), Category("Appearance")]
		public ItemVerticalAlignment GlyphVerticalAlignment {
			get { return glyphVerticalAlignment; }
			set {
				if(GlyphVerticalAlignment == value)
					return;
				glyphVerticalAlignment = value;
				OnItemChanged();
			}
		}
		[DefaultValue(ItemVerticalAlignment.Default), Category("Appearance"), SmartTagProperty("Caption Vertical Alignment", "Appearance")]
		public ItemVerticalAlignment CaptionVerticalAlignment {
			get { return captionVerticalAlignment; }
			set {
				if(CaptionVerticalAlignment == value)
					return;
				captionVerticalAlignment = value;
				OnItemChanged();
			}
		}
		[DefaultValue(false), Category("Appearance")]
		public bool AllowHtmlString {
			get { return allowHtmlString; }
			set {
				if(AllowHtmlString == value)
					return;
				allowHtmlString = value;
				OnItemChanged();
			}
		}
		[DefaultValue(ItemLocation.Default), Category("Appearance")]
		public ItemLocation GlyphLocation {
			get { return glyphLocation; }
			set {
				if(GlyphLocation == value)
					return;
				glyphLocation = value;
				OnItemChanged();
			}
		}
		[Browsable(false)]
		public object Images { get { return Control == null ? null : Control.Images; } }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BackstageViewItemImageIndex"),
#endif
 DefaultValue(-1), Category("Appearance"), ImageList("Images"),
		Editor(typeof(ImageIndexesEditor), typeof(UITypeEditor)), SmartTagProperty("Image Index", "Image", 1, SmartTagActionType.RefreshAfterExecute)
		]
		public virtual int ImageIndex {
			get { return imageIndex; }
			set {
				if(value == ImageIndex)
					return;
				imageIndex = value;
				OnItemChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BackstageViewItemImageIndexHover"),
#endif
 DefaultValue(-1), Category("Appearance"), Editor(typeof(DevExpress.Utils.Design.ImageIndexesEditor), typeof(UITypeEditor)), DevExpress.Utils.ImageList("Images")]
		public virtual int ImageIndexHover {
			get { return imageIndexHover; }
			set {
				if(ImageIndexHover == value)
					return;
				imageIndexHover = value;
				OnItemChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BackstageViewItemImageIndexPressed"),
#endif
 DefaultValue(-1), Category("Appearance"), Editor(typeof(DevExpress.Utils.Design.ImageIndexesEditor), typeof(UITypeEditor)), DevExpress.Utils.ImageList("Images")]
		public virtual int ImageIndexPressed {
			get { return imageIndexPressed; }
			set {
				if(ImageIndexPressed == value)
					return;
				imageIndexPressed = value;
				OnItemChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BackstageViewItemImageIndexDisabled"),
#endif
 DefaultValue(-1), Category("Appearance"), Editor(typeof(DevExpress.Utils.Design.ImageIndexesEditor), typeof(UITypeEditor)), DevExpress.Utils.ImageList("Images")]
		public virtual int ImageIndexDisabled {
			get { return imageIndexDisabled; }
			set {
				if(ImageIndexDisabled == value)
					return;
				imageIndexDisabled = value;
				OnItemChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BackstageViewItemEnabled"),
#endif
 DefaultValue(true), Category("Behavior")]
		public virtual bool Enabled {
			get { return enabled; }
			set {
				if(Enabled == value) return;
				enabled = value;
				OnEnabledChanged();
			}
		}
		protected virtual void OnEnabledChanged() {
			OnItemChanged();
		}
		[ DefaultValue(null), Category("Appearance"), SmartTagProperty("Glyph", "Image", 0, SmartTagActionType.RefreshAfterExecute), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public virtual Image Glyph {
			get { return glyph; }
			set {
				if(value == Glyph) return;
				glyph = value;
				OnItemChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BackstageViewItemGlyphHover"),
#endif
 DefaultValue(null), Category("Appearance"), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public virtual Image GlyphHover {
			get { return glyphHover; }
			set {
				if(GlyphHover == value) return;
				glyphHover = value;
				OnItemChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BackstageViewItemGlyphPressed"),
#endif
 DefaultValue(null), Category("Appearance"), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public virtual Image GlyphPressed {
			get { return glyphPressed; }
			set {
				if(GlyphPressed == value) return;
				glyphPressed = value;
				OnItemChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BackstageViewItemGlyphDisabled"),
#endif
 DefaultValue(null), Category("Appearance"), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public virtual Image GlyphDisabled {
			get { return glyphDisabled; }
			set {
				if(GlyphDisabled == value) return;
				glyphDisabled = value;
				OnItemChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BackstageViewItemAllowGlyphSkinning"),
#endif
 Category("Appearance"), DefaultValue(DefaultBoolean.Default), SmartTagProperty("Allow Glyph Skinning", "Image", 120)]
		public DefaultBoolean AllowGlyphSkinning {
			get { return allowGlyphSkinning; }
			set {
				if(AllowGlyphSkinning == value)
					return;
				allowGlyphSkinning = value;
				OnItemChanged();
			}
		}
		protected internal virtual bool GetAllowGlyphSkinning() {
			if(AllowGlyphSkinning == DefaultBoolean.Default)
				return Control == null ? false : Control.AllowGlyphSkinning;
			return AllowGlyphSkinning == DefaultBoolean.True;
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BackstageViewItemCaption"),
#endif
 Category("Appearance"), Localizable(true),
		Editor(ControlConstants.MultilineStringEditor, typeof(UITypeEditor)), SmartTagProperty("Caption", "Appearance", SmartTagActionType.RefreshAfterExecute)
		]
		public virtual string Caption {
			get { return caption; }
			set {
				if(value == null) value = string.Empty;
				if(Caption == value) return;
				caption = value;
				OnItemChanged();
			}
		}
		protected internal virtual bool ShouldSerializeCaption() {
			return !String.IsNullOrEmpty(Caption);
		}
		protected internal virtual void ResetCaption() {
			Caption = String.Empty;
		}
		protected override BackstageViewItemBase CloneItemCore() {
			return new BackstageViewItemBase();
		}
		public override void Assign(BackstageViewItemBase itemBase) {
			base.Assign(itemBase);
			BackstageViewItem item = itemBase as BackstageViewItem;
			if(item == null)
				return;
			this.allowHtmlString = item.AllowHtmlString;
			this.caption = item.Caption;
			this.captionHorizontalAlignment = item.CaptionHorizontalAlignment;
			this.captionVerticalAlignment = item.CaptionVerticalAlignment;
			this.enabled = item.Enabled;
			this.glyph = item.Glyph;
			this.glyphDisabled = item.GlyphDisabled;
			this.glyphHover = item.GlyphHover;
			this.glyphPressed = item.GlyphPressed;
			this.glyphHorizontalAlignment = item.GlyphHorizontalAlignment;
			this.glyphVerticalAlignment = item.GlyphVerticalAlignment;
			this.glyphLocation = item.GlyphLocation;
			this.imageIndex = item.ImageIndex;
			this.imageIndexHover = item.ImageIndexHover;
			this.imageIndexPressed = item.ImageIndexPressed;
			this.imageIndexDisabled = item.ImageIndexDisabled;
			this.Name = item.Name;
			this.superTip = item.SuperTip == null ? null : (SuperToolTip)item.SuperTip.Clone();
			this.Events.AddHandler(itemHover, item.Events[itemHover]);
			this.Events.AddHandler(itemPressed, item.Events[itemPressed]);
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BackstageViewItemItemHover")]
#endif
		public event BackstageViewItemEventHandler ItemHover {
			add { Events.AddHandler(itemHover, value); }
			remove { Events.RemoveHandler(itemHover, value); }
		}
		protected internal void RaiseItemHover() {
			BackstageViewItemEventHandler handler = Events[itemHover] as BackstageViewItemEventHandler;
			if(handler != null)
				handler(this, new BackstageViewItemEventArgs(this));
			Control.RaiseHighlightedItemChanged(this);
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BackstageViewItemItemPressed")]
#endif
		public event BackstageViewItemEventHandler ItemPressed {
			add { Events.AddHandler(itemPressed, value); }
			remove { Events.RemoveHandler(itemPressed, value); }
		}
		protected internal void RaiseItemPressed() {
			BackstageViewItemEventHandler handler = Events[itemPressed] as BackstageViewItemEventHandler;
			if(handler != null)
				handler(this, new BackstageViewItemEventArgs(this));
			Control.RaisePressedItemChanged(this);
		}
		protected override string ItemCaptionCore { get { return Caption; } }
		protected override bool KeyTipEnabledCore { get { return Enabled; } }
		static readonly int KeyTipHorisontalIndent = 20;
		static readonly int KeyTipVerticalIndent = 10;
		protected override Point GetPoint() {
			Point pt = Point.Empty;
			pt.Y = KeyTipItemBounds.Y + KeyTipItemBounds.Height / 2 - KeyTipVerticalIndent;
			pt.X = KeyTipItemBounds.X + KeyTipHorisontalIndent;
			pt = Control.PointToScreen(pt);
			return pt;
		}
		protected virtual Rectangle KeyTipItemBounds {
			get { return GetViewInfo().Bounds; }
		}
		[ DefaultValue("")]
		public virtual string KeyTip {
			get { return (this as ISupportRibbonKeyTip).ItemUserKeyTip; }
			set {
				if((this as ISupportRibbonKeyTip).ItemUserKeyTip == value) return;
				(this as ISupportRibbonKeyTip).ItemUserKeyTip = value;
			}
		}
	}
	[ToolboxItem(false), DesignTimeVisible(false), DefaultEvent("ItemClick"), SmartTagSupport(typeof(BackstageViewButtonItemDesignTimeBoundsProvider), SmartTagSupportAttribute.SmartTagCreationMode.Auto)]
	public class BackstageViewButtonItem : BackstageViewItem, DevExpress.Utils.MVVM.ISupportCommandBinding {
		private static readonly object itemClick = new object();
		AppearanceObject appearancePressed;
		bool closeBackstageViewOnClick = true;
		public BackstageViewButtonItem() {
			this.appearancePressed = CreateAppearance();
		}
		internal void ResetAppearancePressed() { AppearancePressed.Reset(); }
		internal bool ShouldSerializeAppearancePressed() { return AppearancePressed != null && AppearancePressed.Options != AppearanceOptions.Empty; }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BackstageViewButtonItemAppearancePressed"),
#endif
 Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject AppearancePressed {
			get { return appearancePressed; }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BackstageViewButtonItemItemClick")]
#endif
		public event BackstageViewItemEventHandler ItemClick {
			add { Events.AddHandler(itemClick, value); }
			remove { Events.RemoveHandler(itemClick, value); }
		}
		protected internal void RaiseItemClick() {
			BackstageViewItemEventHandler handler = Events[itemClick] as BackstageViewItemEventHandler;
			if(handler != null)
				handler(this, new BackstageViewItemEventArgs(this));
			if(Control != null) {
				Control.RaiseItemClick(this);
				if(Control.GetRibbon() != null && CloseBackstageViewOnClick)
					Control.GetRibbon().HideApplicationButtonContentControl();
			}
			Execute();
		}
		protected virtual void Execute() {
		}
		protected override BackstageViewItemBase CloneItemCore() {
			return new BackstageViewButtonItem();
		}
		[DefaultValue(true), Category("Behavior")]
		public bool CloseBackstageViewOnClick {
			get { return closeBackstageViewOnClick; }
			set { closeBackstageViewOnClick = value; }
		}
		public override void Assign(BackstageViewItemBase item) {
			base.Assign(item);
			BackstageViewButtonItem button = item as BackstageViewButtonItem;
			this.closeBackstageViewOnClick = button.closeBackstageViewOnClick;
			if(button != null)
				this.Events.AddHandler(itemClick, button.Events[itemClick]);
		}
		protected override void KeyTipItemClick() {
			base.KeyTipItemClick();
			RaiseItemClick();
		}
		#region Commands
		public IDisposable BindCommand(object command, Func<object> queryCommandParameter = null) {
			return DevExpress.Utils.MVVM.CommandHelper.Bind(this,
				(buttonItem, execute) => buttonItem.ItemClick += (s, e) => execute(),
				(buttonItem, canExecute) => buttonItem.Enabled = canExecute(),
				command, queryCommandParameter);
		}
		public IDisposable BindCommand(System.Linq.Expressions.Expression<Action> commandSelector, object source, Func<object> queryCommandParameter = null) {
			return DevExpress.Utils.MVVM.CommandHelper.Bind(this,
				(buttonItem, execute) => buttonItem.ItemClick += (s, e) => execute(),
				(buttonItem, canExecute) => buttonItem.Enabled = canExecute(),
				commandSelector, source, queryCommandParameter);
		}
		public IDisposable BindCommand<T>(System.Linq.Expressions.Expression<Action<T>> commandSelector, object source, Func<T> queryCommandParameter = null) {
			return DevExpress.Utils.MVVM.CommandHelper.Bind(this,
				(buttonItem, execute) => buttonItem.ItemClick += (s, e) => execute(),
				(buttonItem, canExecute) => buttonItem.Enabled = canExecute(),
				commandSelector, source, () => (queryCommandParameter != null) ? queryCommandParameter() : default(T));
		}
		#endregion Commands
	}
	[
	ToolboxItem(false), DesignTimeVisible(false), DefaultEvent("SelectedChanged"),
	SmartTagSupport(typeof(BackstageViewTabItemDesignTimeBoundsProvider), SmartTagSupportAttribute.SmartTagCreationMode.Auto),
		SmartTagAction(typeof(BackstageViewTabItemDesignTimeActionsProvider), "AddChildBackstageView", "Add Child BackstageView"),
	SmartTagAction(typeof(BackstageViewTabItemDesignTimeActionsProvider), "AddRecentItemControl", "Add RecentItemControl")
	]
	public class BackstageViewTabItem : BackstageViewItem {
		private static readonly object selectedChanged = new object();
		BackstageViewClientControl contentControl;
		AppearanceObject appearanceSelected;
		public BackstageViewTabItem() {
			this.appearanceSelected = CreateAppearance();
			this.contentControl = new BackstageViewClientControl();
		}
		internal void ResetAppearanceSelected() { AppearanceSelected.Reset(); }
		internal bool ShouldSerializeAppearanceSelected() { return AppearanceSelected != null && AppearanceSelected.Options != AppearanceOptions.Empty; }
		[ Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject AppearanceSelected {
			get { return appearanceSelected; }
		}
		internal override bool IsTabItem { get { return true; } }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BackstageViewTabItemSelected"),
#endif
 SmartTagProperty("Selected", "Appearance", 5)]
		public bool Selected {
			get { return Control == null ? false : Control.SelectedTab == this; }
			set {
				if(Control == null)
					return;
				if(value == false && Control.SelectedTab == this)
					Control.SelectedTab = null;
				else if(value)
					Control.SelectedTab = this;
			}
		}
		public BackstageViewClientControl ContentControl {
			get { return contentControl; }
			set {
				if(ContentControl == value)
					return;
				Control prevControl = ContentControl;
				contentControl = value;
				OnContentControlChanged(prevControl);
			}
		}
		protected virtual void OnContentControlChanged(Control prevControl) {
			if(Control == null)
				return;
			if(prevControl != null)
				Control.Controls.Remove(prevControl);
			if(ContentControl != null)
				Control.Controls.Add(ContentControl);
			OnItemChanged();
			UpdateControlProperties();
		}
		protected internal virtual void UpdateControlProperties() {
			if(ContentControl == null)
				return;
			if(Control != null) {
				if(Control.ViewInfo.IsReady) {
					ContentControl.Bounds = Control.ViewInfo.RightPaneBounds;
				}
				ContentControl.Visible = Control.SelectedTab == this;
			}
			ContentControl.Enabled = Enabled;
			ContentControl.UpdateProperties();
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BackstageViewTabItemSelectedChanged")]
#endif
		public event BackstageViewItemEventHandler SelectedChanged {
			add { Events.AddHandler(selectedChanged, value); }
			remove { Events.RemoveHandler(selectedChanged, value); }
		}
		protected internal void RaiseSelectedChanged() {
			BackstageViewItemEventHandler handler = Events[selectedChanged] as BackstageViewItemEventHandler;
			if(handler != null)
				handler(this, new BackstageViewItemEventArgs(this));
		}
		internal override void SetControl(BackstageViewControl control) {
			SetControlCore(control);
			if(Control != null) {
				Control.Controls.Remove(ContentControl);
			}
			if(control != null) {
				control.Controls.Add(ContentControl);
				UpdateControlProperties();
			}
			OnItemChanged();
		}
		protected override void OnVisibleChanged() {
			base.OnVisibleChanged();
			if(!IsVisible && Control != null && Control.SelectedTab == this)
				Control.SelectedTab = null;
		}
		protected override void OnEnabledChanged() {
			base.OnEnabledChanged();
			ContentControl.Enabled = Enabled;
		}
		protected internal virtual void OnShowing() {
			if(ContentControl != null) {
				ContentControl.Visible = true;
				ContentControl.UpdateProperties();
			}
		}
		protected internal virtual void OnHidding() {
			HideChildKeyTips();
		}
		protected internal virtual void HideChildKeyTips() {
			if(HasClientBackstageView) {
				BackstageViewControl bsv = GetClientBackstageViewControls()[0];
				if(bsv != null && bsv.ViewInfo.ShouldActivateKeyTips) {
					bsv.HideKeyTips();
				}
			}
		}
		protected override BackstageViewItemBase CloneItemCore() {
			return new BackstageViewTabItem();
		}
		public override void Assign(BackstageViewItemBase item) {
			base.Assign(item);
			BackstageViewTabItem tab = item as BackstageViewTabItem;
			if(tab != null)
				this.Events.AddHandler(selectedChanged, tab.Events[selectedChanged]);
		}
		protected override void KeyTipItemClick() {
			base.KeyTipItemClick();
			Selected = true;
			if(HasClientBackstageView) {
				BackstageViewControl bsv = GetClientBackstageViewControls()[0];
					if(bsv != null) {
						bsv.ShouldShowKeyTips = true;
						if(bsv.ViewInfo.ShouldActivateKeyTips) 
							bsv.ShowKeyTips();
						bsv.Focus();
					}
			}
		}
		protected override void OnAppearanceChanged(object sender, EventArgs e) {
			base.OnAppearanceChanged(sender, e);
			RefreshContentControl();
		}
		protected internal void RefreshContentControl() {
			foreach(Control control in ContentControl.Controls) {
				if(control is BackstageViewControl) {
					((BackstageViewControl)control).OnPropertiesChangedCore();
				}
			}
		}
		protected internal new BackstageViewTabItemViewInfo GetViewInfo() {
			return (BackstageViewTabItemViewInfo)base.GetViewInfo();
		}
		protected internal override bool HasClientBackstageView {
			get {
				if(ContentControl == null) return false;
				if(GetClientBackstageViewControls().Count == 0) return false;
				return true;
			}
		}
		protected internal override List<BackstageViewControl> GetClientBackstageViewControls() {
			List<BackstageViewControl> res = new List<BackstageViewControl>();
			foreach(Control bsv in ContentControl.Controls) {
				if(bsv is BackstageViewControl) res.Add((BackstageViewControl)bsv);
			}
			return res;
		}
		protected internal bool HasChildRecentItemControl {
			get {
				if(ContentControl == null) return false;
				return ContentControl.Controls.OfType<RecentItemControl>().Count() > 0;
			}
		}
		protected internal void DoRefreshContent() {
			if(ContentControl != null) ContentControl.Refresh();
		}
	}
	public class BackstageViewControlItemCollecton : CollectionBase, IEnumerable<BackstageViewItemBase> {
		BackstageViewControl control;
		public BackstageViewControlItemCollecton(BackstageViewControl control) {
			this.control = control;
		}
		public BackstageViewControl Control { get { return control; } }
		public int Add(BackstageViewItemBase item) { return List.Add(item); }
		public void Insert(int index, BackstageViewItemBase item) { List.Insert(index, item); }
		public void Remove(BackstageViewItemBase item) { List.Remove(item); }
		public int IndexOf(BackstageViewItemBase item) { return List.IndexOf(item); }
		public bool Contains(BackstageViewItemBase item) { return List.Contains(item); }
		public BackstageViewItemBase this[int index] { get { return (BackstageViewItemBase)List[index]; } set { List[index] = value; } }
		public int TabCount {
			get {
				int count = 0;
				for(int i = 0; i < Count; i++) {
					if(this[i].IsTabItem)
						count++;
				}
				return count;
			}
		}
		public int FirstTabIndex {
			get {
				for(int i = 0; i < Count; i++) {
					if(this[i].IsTabItem)
						return i;
				}
				return -1;
			}
		}
		public int LastTabIndex {
			get {
				for(int i = Count - 1; i >= 0; i--) {
					if(this[i].IsTabItem)
						return i;
				}
				return -1;
			}
		}
		protected override void OnInsertComplete(int index, object value) {
			base.OnInsertComplete(index, value);
			((BackstageViewItemBase)value).SetControl(Control);
		}
		protected override void OnRemoveComplete(int index, object value) {
			base.OnRemoveComplete(index, value);
			if(Control.SelectedTab == value)
				Control.SelectedTab = null;
			((BackstageViewItemBase)value).SetControl(null);
			Control.OnItemRemoved((BackstageViewItemBase)value);
		}
		protected override void OnSetComplete(int index, object oldValue, object newValue) {
			base.OnSetComplete(index, oldValue, newValue);
			if(Control.SelectedTab == oldValue)
				Control.SelectedTab = null;
			((BackstageViewItemBase)oldValue).SetControl(null);
			((BackstageViewItemBase)newValue).SetControl(Control);
		}
		IEnumerator<BackstageViewItemBase> IEnumerable<BackstageViewItemBase>.GetEnumerator() {
			foreach(BackstageViewItemBase item in InnerList)
				yield return item;
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return InnerList.GetEnumerator();
		}
	}
	public delegate void BackstageViewItemEventHandler(object sender, BackstageViewItemEventArgs e);
	public class BackstageViewItemEventArgs : EventArgs {
		BackstageViewItem item;
		public BackstageViewItemEventArgs(BackstageViewItem item) {
			this.item = item;
		}
		public BackstageViewItem Item { get { return item; } }
	}
	public delegate void BackstageViewItemCustomDrawEventHandler(object sender, BackstageViewItemCustomDrawEventArgs e);
	public class BackstageViewItemCustomDrawEventArgs : EventArgs {
		GraphicsCache cache;
		BackstageViewItemBaseViewInfo itemInfo;
		public BackstageViewItemCustomDrawEventArgs(BackstageViewItemBaseViewInfo itemInfo, GraphicsCache cache) {
			this.itemInfo = itemInfo;
			this.cache = cache;
		}
		public GraphicsCache Cache { get { return cache; } }
		public Graphics Graphics { get { return Cache.Graphics; } }
		public BackstageViewItemBaseViewInfo ItemInfo { get { return itemInfo; } }
		public BackstageViewItemViewInfo BackstageViewItemInfo { get { return ItemInfo as BackstageViewItemViewInfo; } }
		BackstageViewButtonItemViewInfo ButtonItemInfo { get { return ItemInfo as BackstageViewButtonItemViewInfo; } }
		BackstageViewTabItemViewInfo TabItemInfo { get { return ItemInfo as BackstageViewTabItemViewInfo; } }
		BackstageViewItemSeparatorViewInfo SeparatorItemInfo { get { return ItemInfo as BackstageViewItemSeparatorViewInfo; } }
		public Rectangle Bounds { get { return ItemInfo.Bounds; } }
		public Rectangle TextBounds {
			get {
				return BackstageViewItemInfo != null ? BackstageViewItemInfo.CaptionBounds : Rectangle.Empty;
			}
		}
		public Rectangle GlyphBounds {
			get {
				return BackstageViewItemInfo != null ? BackstageViewItemInfo.GlyphBounds : Rectangle.Empty;
			}
		}
		public Rectangle ContentBounds {
			get {
				return BackstageViewItemInfo != null ? BackstageViewItemInfo.ContentBounds : Rectangle.Empty;
			}
		}
	}
	[Docking(DockingBehavior.Never), Designer("DevExpress.XtraBars.Ribbon.Design.BackstageViewControlClientDesigner, " + AssemblyInfo.SRAssemblyBarsDesign, typeof(IDesigner)), ToolboxItem(false)]
	public class BackstageViewClientControl : XtraUserControl, ITransparentBackgroundManager {
		IntPtr hRegion;
		public BackstageViewClientControl() {
			this.hRegion = IntPtr.Zero;
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
		}
		AccessibleBackstageViewClientControl accessibleBackstageViewClientControl = null;
		[Browsable(false)]
		public AccessibleBackstageViewClientControl AccessibleBackstageViewClientControl {
			get {
				if(accessibleBackstageViewClientControl == null) accessibleBackstageViewClientControl = CreateAccessibleBackstageViewClientControl();
				return accessibleBackstageViewClientControl;
			}
		}
		protected virtual AccessibleBackstageViewClientControl CreateAccessibleBackstageViewClientControl() { return new AccessibleBackstageViewClientControl(this); }
		protected override AccessibleObject CreateAccessibilityInstance() { return AccessibleBackstageViewClientControl.Accessible; }
		protected override bool ProcessTabKey(bool forward) {
			return base.SelectNextControl(ActiveControl, forward, true, true, true);
		}
		[Browsable(false)]
		public new Size Size {
			get { return base.Size; }
			set { base.Size = value; }
		}
		[Browsable(false)]
		public new Point Location {
			get { return base.Location; }
			set { base.Location = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DockStyle Dock {
			get { return base.Dock; }
			set { base.Dock = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool Visible {
			get { return base.Visible; }
			set { base.Visible = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool Enabled {
			get { return base.Enabled; }
			set { base.Enabled = value; }
		}
		protected bool ShouldExpandContent() {
			if(BackstageView == null || DesignMode) return false;
			return BackstageView.ViewInfo.IsVScrollVisible;
		}
		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {
			BackstageViewControl owner = Parent as BackstageViewControl;
			if(owner != null) {
				bool expandContent = ShouldExpandContent();
				x = owner.ViewInfo.RightPaneContentBounds.X;
				y = owner.ViewInfo.RightPaneContentBounds.Y;
				if(expandContent && owner.ScrollerPosition != 0) {
					y -= owner.ScrollerPosition;
				}
				width = owner.ViewInfo.RightPaneContentBounds.Width;
				if(expandContent) {
					height = Math.Max(0, owner.ViewInfo.ItemBottomLine - y);
				}
				else {
					height = owner.ViewInfo.RightPaneContentBounds.Height;
				}
			}
			base.SetBoundsCore(x, y, width, height, specified);
			SetClipRegion();
		}
		protected internal virtual void UpdateProperties() {
			SetClipRegion();
		}
		[System.Security.SecuritySafeCritical]
		protected virtual void SetClipRegion() {
			if(!IsHandleCreated) return;
			IntPtr oldRgn = hRegion;
			this.hRegion = CalcClipRegion();
			if(this.hRegion != IntPtr.Zero) {
				NativeMethods.SetWindowRgn(Handle, this.hRegion, false);
			}
			else {
				NativeMethods.SetWindowRgn(Handle, IntPtr.Zero, false);
			}
			if(oldRgn != IntPtr.Zero) NativeMethods.DeleteObject(oldRgn);
		}
		protected IntPtr CalcClipRegion() {
			if(!ShouldExpandContent()) return IntPtr.Zero;
			int yPt = BackstageView.ViewInfo.RightPaneContentBounds.Y;
			int height = Math.Max(0, BackstageView.Height - yPt - 1);
			if(height == 0) return IntPtr.Zero;
			int overpan = Math.Max(0, yPt - Location.Y);
			return NativeMethods.CreateRectRgn(0, overpan, Width, height + overpan);
		}
		public Color GetBackgroundColor() {
			if(BackstageView != null) {
				Color c = BackstageView.ViewInfo.GetBackgroundInfo().Element.Color.BackColor;
				if(c.IsSystemColor)
					c = RibbonSkins.GetSkin(BackstageView.ViewInfo.Provider).GetSystemColor(c);
				return c;
			}
			return BackColor;
		}
		protected override void OnPaint(PaintEventArgs e) {
			DevExpress.Utils.Mdi.ControlState.CheckPaintError(this);
			base.OnPaint(e);
			using(GraphicsCache cache = new GraphicsCache(e)) {
				Color c = GetBackgroundColor();
				if(IsPaintingSuspended) {
					if(!IsEqualsToBackgroundColor(c))
						e.Graphics.FillRectangle(cache.GetSolidBrush(c), ClientRectangle);
					return;
				}
				if(BackstageView != null) {
					e.Graphics.FillRectangle(cache.GetSolidBrush(c), ClientRectangle);
					BackstageViewPainter.DrawBackstageViewImage(cache, this, BackstageView);
				}
				if(Controls.Count == 0 && DesignMode) DrawDTHint(cache);
			}
		}
		protected virtual void DrawDTHint(GraphicsCache cache) {
			AppearanceObject app = AppearanceObject.ControlAppearance;
			app.TextOptions.HAlignment = HorzAlignment.Center;
			if(BackstageView != null && BackstageView.GetPaintStyle() == BackstageViewPaintStyle.Flat) {
				app.TextOptions.WordWrap = WordWrap.Wrap;
				app.DrawString(cache, "Drop controls here, or to speed up the form load, provide the content for the tab dynamically, using the BackstageViewControl.SelectedTabChanged event", ClientRectangle);
			}
			else app.DrawString(cache, "Drop controls here", ClientRectangle);
		}
		protected virtual bool IsPaintingSuspended {
			get {
				if(BackstageView == null)
					return false;
				BackstageViewInfo vi = BackstageView.ViewInfo;
				return (vi.SizingMode && !HasImage) || vi.IsAnimationActive;
			}
		}
		protected bool HasImage {
			get {
				if(BackstageView == null)
					return false;
				BackstageViewInfo vi = BackstageView.ViewInfo;
				return BackstageView.BackgroundImage != null || BackstageView.Image != null || vi.HasSkinBackgroundImage;
			}
		}
		protected bool IsEqualsToBackgroundColor(Color color) {
			if(this.BackColor == color || color.A == 0) return true;
			return false;
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(this.hRegion != IntPtr.Zero) {
					NativeMethods.DeleteObject(this.hRegion);
					this.hRegion = IntPtr.Zero;
				}
			}
			base.Dispose(disposing);
		}
		protected BackstageViewControl BackstageView { get { return Parent as BackstageViewControl; } }
		#region ITransparentBackgroundManager Members
		Color GetTransparentForeColor() {
			if(BackstageView != null) {
				Color res = BackstageView.ViewInfo.GetBackgroundInfo().Element.Color.ForeColor;
				if(res.IsSystemColor)
					res = RibbonSkins.GetSkin(BackstageView.ViewInfo.Provider).GetSystemColor(res);
				return res;
			}
			return SystemColors.ControlText;
		}
		Color ITransparentBackgroundManager.GetForeColor(object childObject) {
			return GetTransparentForeColor();
		}
		Color ITransparentBackgroundManager.GetForeColor(Control childControl) {
			return GetTransparentForeColor();
		}
		#endregion
	}
	public class BackstageViewBaseKeyTipManager : BarsKeyTipManagerBase {
		BackstageViewControl backstageViewControl;
		public BackstageViewBaseKeyTipManager(BackstageViewControl backstageViewControl) {
			this.backstageViewControl = backstageViewControl;
		}
		public void AddItems(BackstageViewControlItemCollecton items) {
			foreach(ISupportRibbonKeyTip item in items) {
				if(item.KeyTipVisible) {
					Items.Add(item);
				}
			}
		}
		public override void HideKeyTips() {
			base.HideKeyTips();
			backstageViewControl.ShouldShowKeyTips = false;
		}
		public override void ShowKeyTips() {
			if(RibbonKeyTipManager != null) {
				RibbonKeyTipManager.IsSuspended = true;
			}
			GenerateKeyTips();
			base.ShowKeyTips();
		}
		protected internal virtual void OnHidding() {
			if(RibbonKeyTipManager == null) return;
			RibbonKeyTipManager.IsSuspended = false;
		}
		protected RibbonKeyTipManager RibbonKeyTipManager {
			get {
				if(BackstageView.Ribbon == null)
					return null;
				return BackstageView.Ribbon.KeyTipManager;
			}
		}
		public BackstageViewControl BackstageView { get { return backstageViewControl; } }
	}
	public class BackstageViewScrollController : IDisposable {
		BackstageViewControl owner;
		DevExpress.XtraEditors.VScrollBar vScroll;
		bool vScrollVisible;
		Rectangle clientRect, vscrollRect;
		bool isRightToLeft;
		public BackstageViewScrollController(BackstageViewControl owner) {
			this.owner = owner;
			this.clientRect = this.vscrollRect = Rectangle.Empty;
			this.vScroll = CreateVScroll();
			this.VScroll.Visible = false;
			this.VScroll.SmallChange = 1;
			this.VScroll.LookAndFeel.ParentLookAndFeel = owner.GetController().LookAndFeel;
			this.isRightToLeft = false;
			ScrollBarBase.ApplyUIMode(VScroll);
		}
		protected virtual DevExpress.XtraEditors.VScrollBar CreateVScroll() { return new DevExpress.XtraEditors.VScrollBar(); }
		public virtual void AddControls(Control container) {
			if(container == null) return;
			container.Controls.Add(VScroll);
		}
		public virtual void RemoveControls(Control container) {
			if(container == null) return;
			container.Controls.Remove(VScroll);
		}
		public virtual int VScrollWidth {
			get { return VScroll.GetDefaultVerticalScrollBarWidth(); }
		}
		public int VScrollPosition { get { return VScroll.Value; } }
		public DevExpress.XtraEditors.VScrollBar VScroll { get { return vScroll; } }
		public ScrollArgs VScrollArgs {
			get { return new ScrollArgs(VScroll); }
			set { value.AssignTo(VScroll); }
		}
		public Rectangle VScrollRect { get { return vscrollRect; } }
		public int VScrollMaximum { get { return VScroll.Maximum; } }
		public int VScrollLargeChange { get { return VScroll.LargeChange; } }
		public bool VScrollVisible {
			get { return vScrollVisible; }
			set {
				if(VScrollVisible == value) UpdateVisiblity();
				else {
					vScrollVisible = value;
					LayoutChanged();
				}
			}
		}
		public bool IsRightToLeft {
			get {
				return isRightToLeft;
			}
			set {
				if(isRightToLeft == value) return;
				isRightToLeft = value;
				LayoutChanged();
			}
		}
		public Rectangle ClientRect {
			get { return clientRect; }
			set {
				if(ClientRect == value) return;
				clientRect = value;
				LayoutChanged();
			}
		}
		protected virtual void CalcRects() {
			this.vscrollRect = Rectangle.Empty;
			Rectangle r = Rectangle.Empty;
			if(VScrollVisible) {
				if(!IsRightToLeft) {
					r.Location = new Point(ClientRect.Right - VScrollWidth, ClientRect.Y);
				}
				else {
					r.Location = new Point(ClientRect.X, ClientRect.Y);
				}
				r.Size = new Size(VScrollWidth, ClientRect.Height);
				vscrollRect = r;
			}
		}
		public void UpdateVisiblity() {
			VScroll.SetVisibility(vScrollVisible && !ClientRect.IsEmpty);
			VScroll.Bounds = VScrollRect;
		}
		int lockLayout = 0;
		public virtual void LayoutChanged() {
			if(lockLayout != 0) return;
			lockLayout++;
			try {
				CalcRects();
				UpdateVisiblity();
				if(ClientRect.IsEmpty) VScroll.SetVisibility(false);
			}
			finally {
				lockLayout--;
			}
		}
		public event EventHandler VScrollValueChanged {
			add { VScroll.ValueChanged += value; }
			remove { VScroll.ValueChanged -= value; }
		}
		internal void OnAction(ScrollNotifyAction action) {
			VScroll.OnAction(action);
		}
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				if(VScroll != null) VScroll.Dispose();
			}
		}
	}
}
