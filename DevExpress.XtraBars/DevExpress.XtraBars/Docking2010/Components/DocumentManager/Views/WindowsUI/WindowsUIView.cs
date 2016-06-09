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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Serializing;
using DevExpress.XtraBars.Docking2010.Base;
using DevExpress.XtraBars.Docking2010.Customization;
namespace DevExpress.XtraBars.Docking2010.Views.WindowsUI {
	public enum ContextualZoomLevel {
		Overview,
		Normal,
		Detail
	}
	public class WindowsUIView : BaseView, ISearchProvider {
		IBaseTileProperties tilePropertiesCore;
		IPageProperties pagePropertiesCore;
		ISplitGroupProperties splitGroupPropertiesCore;
		ISlideGroupProperties slideGroupPropertiesCore;
		IPageGroupProperties pageGroupPropertiesCore;
		ITabbedGroupProperties tabbedGroupPropertiesCore;
		ITileContainerProperties tileContainerPropertiesCore;
		ISearchPanelProperties searchPanelPropertiesCore;
		IFlyoutProperties flyoutPropertiesCore;
		IContentContainerProperties detailContainerPropertiesCore;
		IContentContainer activeContentContainerCore;
		IContentContainer activeFlyoutContainerCore;
		BaseTileCollection tilesCore;
		ContentContainerCollection contentContainersCore;
		ContentContainerActionCollection contentContainerActionsCore;
		NavigationAdornerBootStrapper navigationBarBootStrapperCore;
		FlyoutAdornerBootStrapper flyoutBootStrapperCore;
		SearchPanelBootStraper searchPanelBootStrapperCore;
		public WindowsUIView() { }
		public WindowsUIView(IContainer container)
			: base(container) {
		}
		protected override void OnCreate() {
			tilesCore = CreateTileCollection();
			Tiles.CollectionChanged += OnTileCollectionChanged;
			tilePropertiesCore = CreateTileProperties();
			TileProperties.Changed += OnTilePropertiesChanged;
			pagePropertiesCore = CreatePageProperties();
			PageProperties.Changed += OnPagePropertiesChanged;
			splitGroupPropertiesCore = CreateSplitGroupProperties();
			SplitGroupProperties.Changed += OnSplitGroupPropertiesChanged;
			slideGroupPropertiesCore = CreateSlideGroupProperties();
			SplitGroupProperties.Changed += OnSlideGroupPropertiesChanged;
			pageGroupPropertiesCore = CreatePageGroupProperties();
			PageGroupProperties.Changed += OnPageGroupPropertiesChanged;
			tabbedGroupPropertiesCore = CreateTabbedGroupProperties();
			TabbedGroupProperties.Changed += OnTabbedGroupPropertiesChanged;
			flyoutPropertiesCore = CreateFlyoutProperties();
			FlyoutProperties.Changed += OnFlyoutPropertiesChanged;
			tileContainerPropertiesCore = CreateTileContainerProperties();
			TileContainerProperties.Changed += OnTileContainerPropertiesChanged;
			contentContainersCore = CreateContentContainers();
			ContentContainers.CollectionChanged += OnContentContainersCollectionChanged;
			contentContainerActionsCore = CreateActions();
			ContentContainerActions.CollectionChanged += OnActionsCollectionChanged;
			overviewContainerPropertiesCore = CreateOverviewContainerProperties();
			OverviewContainerProperties.Changed += OnOverviewContainerPropertiesChanged;
			detailContainerPropertiesCore = CreateDetailContainerProperties();
			DetailContainerProperties.Changed += OnDetailContainerPropertiesChanged;
			splashScreenPropertiesCore = CreateSplashScreenProperties();
			searchPanelPropertiesCore = CreateSearchPanelPropertie();
			base.OnCreate();
		}
		protected override void LockComponentBeforeDisposing() {
			base.LockComponentBeforeDisposing();
			DestroyAsyncAnimations();
			Tiles.CollectionChanged -= OnTileCollectionChanged;
			ContentContainerActions.CollectionChanged -= OnActionsCollectionChanged;
			ContentContainers.CollectionChanged -= OnContentContainersCollectionChanged;
			TileProperties.Changed -= OnTilePropertiesChanged;
			PageProperties.Changed -= OnPagePropertiesChanged;
			SplitGroupProperties.Changed -= OnSplitGroupPropertiesChanged;
			SlideGroupProperties.Changed -= OnSlideGroupPropertiesChanged;
			PageGroupProperties.Changed -= OnPageGroupPropertiesChanged;
			TabbedGroupProperties.Changed -= OnTabbedGroupPropertiesChanged;
			TileContainerProperties.Changed -= OnTileContainerPropertiesChanged;
			OverviewContainerProperties.Changed -= OnOverviewContainerPropertiesChanged;
			DetailContainerProperties.Changed -= OnDetailContainerPropertiesChanged;
		}
		protected override void OnDispose() {
			Unload();
			ColoredElementsCache.Reset();
			Ref.Dispose(ref tilesCore);
			Ref.Dispose(ref tilePropertiesCore);
			Ref.Dispose(ref pagePropertiesCore);
			Ref.Dispose(ref splitGroupPropertiesCore);
			Ref.Dispose(ref slideGroupPropertiesCore);
			Ref.Dispose(ref pageGroupPropertiesCore);
			Ref.Dispose(ref tabbedGroupPropertiesCore);
			Ref.Dispose(ref tileContainerPropertiesCore);
			Ref.Dispose(ref flyoutPropertiesCore);
			Ref.Dispose(ref contentContainersCore);
			Ref.Dispose(ref contentContainerActionsCore);
			Ref.Dispose(ref navigationBarBootStrapperCore);
			Ref.Dispose(ref searchPanelBootStrapperCore);
			Ref.Dispose(ref searchPanelPropertiesCore);
			Ref.Dispose(ref flyoutBootStrapperCore);
			Ref.Dispose(ref overviewContainerPropertiesCore);
			Ref.Dispose(ref detailContainerPropertiesCore);
			Ref.Dispose(ref splashScreenPropertiesCore);
			activeContentContainerCore = null;
			base.OnDispose();
		}
		internal AdornerElementInfo navigationBarInfo;
		protected internal void CreateNavigationAdorner() {
			var info = Customization.NavigationAdornerElementInfoArgs.EnsureInfoArgs(ref navigationBarInfo, this);
			navigationBarBootStrapperCore = new NavigationAdornerBootStrapper(this, info);
		}
		protected internal void DestroyNavigationAdorner() {
			Ref.Dispose(ref navigationBarBootStrapperCore);
		}
		internal int navigationAdornerCounter;
		protected internal bool IsInNavigationAdorner {
			get { return navigationAdornerCounter > 0; }
		}
		protected internal bool ForceShowNavigationAdorner;
		protected internal void ShowNavigationAdorner() {
			if(navigationBarBootStrapperCore != null) {
				if(!navigationBarBootStrapperCore.isShown)
					navigationBarBootStrapperCore.Show();
				else UpdateNavigationBars();
			}
		}
		public void ShowSearchPanel() {
			if(searchPanelBootStrapperCore == null || searchPanelBootStrapperCore.IsShown) return;
			if(!RaiseSearchPanelShowing(null, false)) return;
			searchPanelBootStrapperCore.Show();
		}
		public void HideSearchPanel() { CancelSearchPanelAdorner(); }
		protected internal void HideNavigationAdorner() {
			if(navigationBarBootStrapperCore != null && navigationBarBootStrapperCore.isShown)
				navigationBarBootStrapperCore.Hide();
		}
		protected override void UpdateStyleCore() {
			base.UpdateStyleCore();
			UpdateNavigationBars();
			UpdateSearchPanelAdorner();
		}
		public void UpdateDocumentActions() {
			var contentContainer = ActiveContentContainer as IContentContainerInternal;
			if(contentContainer != null) contentContainer.UpdateDocumentActions();
		}
		public void UpdateNavigationBars() {
			if(navigationBarBootStrapperCore != null && navigationBarBootStrapperCore.isShown)
				if(navigationBarInfo.InfoArgs != null)
					navigationBarBootStrapperCore.Update();
		}
		protected internal virtual bool CanActivateNavigationAdornerOnKeyDown(Control target) {
			if(target is DevExpress.XtraEditors.PictureEdit)
				return true;
			if(target is TextBoxBase || target is DevExpress.XtraEditors.BaseEdit)
				return false;
			if(target is ButtonBase || target is DevExpress.XtraEditors.BaseButton)
				return false;
			return true;
		}
		protected internal override bool CanShowOverlapWarning() {
			return base.CanShowOverlapWarning() && Tiles.Count == 0 && ContentContainers.Count == 0;
		}
		readonly static string[] ignoreNavigationAdornerActivation = new string[] { 
			"DevExpress.XtraScheduler.SchedulerControl"
		};
		protected internal virtual bool CanActivateNavigationAdornerOnRightClick(Point screenPoint, Control target, DevExpress.Utils.DXMouseEventArgs args, out bool handled) {
			handled = false;
			if(ActiveContentContainer == null)
				return false;
			if(ActiveFlyoutContainer != null) return false;
			if(target == Manager.GetOwnerControl()) {
				Point clientPoint = Manager.ScreenToClient(screenPoint);
				var hitInfo = Manager.CalcHitInfo(clientPoint);
				if(hitInfo == null || hitInfo.IsEmpty || hitInfo.Info == null) return true;
				if(hitInfo.Info.InMenuBounds || hitInfo.Info.InControlBox) return false;
				if(hitInfo.Info.InHandlerBounds) {
					IInteractiveElementInfo interactiveInfo = Dragging.InfoHelper.GetInteractiveElementInfo(hitInfo.Info.Element);
					if(interactiveInfo != null) {
						interactiveInfo.ProcessMouseDown(args);
						handled = true;
					}
				}
			}
			else {
				if(target is TextBoxBase || target is DevExpress.XtraEditors.BaseEdit)
					return false;
				if(target is ButtonBase || target is DevExpress.XtraEditors.BaseButton)
					return false;
				if(target.ContextMenu != null || target.ContextMenuStrip != null)
					return false;
				Type targetType = target.GetType();
				while(targetType != typeof(Control)) {
					if(Array.IndexOf(ignoreNavigationAdornerActivation, targetType.FullName) != -1)
						return false;
					targetType = targetType.BaseType;
				}
				BarManager barManager = BarManager.FindManager(target) ??
					BarManager.FindManager(target.FindForm());
				if(barManager != null && barManager.GetPopupContextMenu(target) != null)
					return false;
				int openedFormsCount = Application.OpenForms.Count;
				handled = true;
				System.Reflection.MethodInfo mi = typeof(Control).GetMethod("OnMouseDown",
					System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
				using(var context = DevExpress.Utils.Menu.DXMenuShowTracker.Track(Manager.GetMenuManager())) {
					if(mi != null)
						mi.Invoke(target, new object[] { args });
					if(context.Shown)
						return false;
					DevExpress.Utils.Drawing.Helpers.NativeMethods.SendMessage(target.Handle, DevExpress.Utils.Drawing.Helpers.MSG.WM_CONTEXTMENU,
						target.Handle, GetLParam(screenPoint));
					if(context.Shown)
						return false;
				}
				if(openedFormsCount < Application.OpenForms.Count)
					return false;
				handled = args.Handled;
			}
			return !args.Handled || ForceShowNavigationAdorner;
		}
		protected IntPtr GetLParam(Point screenPoint) {
			return new IntPtr(screenPoint.Y * 0x10000 + screenPoint.X);
		}
		protected internal virtual bool CanCancelNavigationAdornerOnRightClick(Point screenPoint, Control target, DevExpress.Utils.DXMouseEventArgs args) {
			Point clientPoint = Manager.ScreenToClient(screenPoint);
			var hitInfo = Manager.CalcHitInfo(clientPoint);
			if(hitInfo == null || hitInfo.IsEmpty || hitInfo.Info == null) return true;
			if(hitInfo.Info.InMenuBounds || hitInfo.Info.InControlBox) return false;
			if(hitInfo.Info.InHandlerBounds) {
				IInteractiveElementInfo interactiveInfo = Dragging.InfoHelper.GetInteractiveElementInfo(hitInfo.Info.Element);
				if(interactiveInfo != null)
					interactiveInfo.ProcessMouseDown(args);
			}
			return !args.Handled;
		}
		internal AdornerElementInfo flyoutInfo;
		protected internal void CreateFlyoutAdorner() {
			FlyoutAdornerElementInfoArgs info = FlyoutAdornerElementInfoArgs.EnsureInfoArgs(ref flyoutInfo, this);
			flyoutBootStrapperCore = new FlyoutAdornerBootStrapper(this, info);
		}
		protected internal void DestroyFlyoutAdorner() {
			Ref.Dispose<FlyoutAdornerBootStrapper>(ref flyoutBootStrapperCore);
		}
		protected internal void ShowFlyoutAdorner() {
			if(flyoutBootStrapperCore != null) {
				if(!flyoutBootStrapperCore.isShown) {
					flyoutBootStrapperCore.Show();
				}
				else {
					UpdateFlyoutAdorner();
				}
			}
		}
		protected internal DialogResult GetFlyoutAdornerResult() {
			if((flyoutInfo != null) && flyoutBootStrapperCore != null)
				return flyoutBootStrapperCore.GetDialogResult();
			return DialogResult.None;
		}
		protected internal void HideFlyoutAdorner() {
			if((flyoutInfo != null) && flyoutBootStrapperCore.isShown) {
				flyoutBootStrapperCore.HideAdorner();
			}
		}
		protected internal void UpdateFlyoutAdorner() {
			if(((flyoutBootStrapperCore != null) && flyoutBootStrapperCore.isShown) && (flyoutInfo.InfoArgs != null)) {
				flyoutBootStrapperCore.Update();
			}
		}
		protected internal void CreateSearchPanelAdorner() {
			var info = new Customization.SearchPanelAdornerElementInfoArgs(this);
			searchPanelBootStrapperCore = new SearchPanelBootStraper(info);
		}
		protected internal void CancelSearchPanelAdorner() {
			if(searchPanelBootStrapperCore != null && searchPanelBootStrapperCore.IsShown)
				searchPanelBootStrapperCore.Cancel();
		}
		protected internal void DestroySearchPanelAdorner() {
			Ref.Dispose(ref searchPanelBootStrapperCore);
		}
		protected internal void UpdateSearchPanelAdorner() {
			if(searchPanelBootStrapperCore != null)
				searchPanelBootStrapperCore.Update();
		}
		protected override void OnManagerChanged() {
			DestroyNavigationAdorner();
			DestroyFlyoutAdorner();
			DestroySearchPanelAdorner();
			base.OnManagerChanged();
			foreach(IContentContainerInternal container in ContentContainers)
				container.SetManager(Manager);
			foreach(BaseTile tile in Tiles)
				tile.SetManager(Manager);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override FloatingDocumentContainer FloatingDocumentContainer {
			get { return base.FloatingDocumentContainer; }
			set { base.FloatingDocumentContainer = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DevExpress.Utils.DefaultBoolean UseDocumentSelector {
			get { return base.UseDocumentSelector; }
			set { base.UseDocumentSelector = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DevExpress.Utils.DefaultBoolean AllowHotkeyNavigation {
			get { return base.AllowHotkeyNavigation; }
			set { base.AllowHotkeyNavigation = value; }
		}
		protected internal override bool CanUseDocumentSelector() {
			return false;
		}
		public sealed override ViewType Type {
			get { return ViewType.WindowsUI; }
		}
		protected sealed internal override Type GetUIElementKey() {
			return typeof(WindowsUIView);
		}
		protected sealed internal override bool AllowMdiLayout {
			get { return false; }
		}
		protected sealed internal override bool AllowMdiSystemMenu {
			get { return false; }
		}
		[Browsable(false)]
		public new IWindowsUIViewController Controller {
			get { return base.Controller as IWindowsUIViewController; }
		}
		protected override IBaseViewController CreateController() {
			return new WindowsUIViewController(this);
		}
		protected override IBaseDocumentProperties CreateDocumentProperties() {
			return new DocumentProperties();
		}
		DevExpress.Utils.DefaultBoolean addTileWhenCreatingDocumentCore = DevExpress.Utils.DefaultBoolean.Default;
#if !SL
	[DevExpressXtraBarsLocalizedDescription("WindowsUIViewAddTileWhenCreatingDocument")]
#endif
		[XtraSerializableProperty, DefaultValue(DevExpress.Utils.DefaultBoolean.Default), Category("Behavior")]
		public DevExpress.Utils.DefaultBoolean AddTileWhenCreatingDocument {
			get { return addTileWhenCreatingDocumentCore; }
			set { SetValue(ref addTileWhenCreatingDocumentCore, value); }
		}
		protected internal virtual bool CanAddTileWhenCreatingDocument() {
			return AddTileWhenCreatingDocument != DevExpress.Utils.DefaultBoolean.False;
		}
		string captionCore;
#if !SL
	[DevExpressXtraBarsLocalizedDescription("WindowsUIViewCaption")]
#endif
		[XtraSerializableProperty, DefaultValue(null), Category("Appearance"), Localizable(true)]
		public string Caption {
			get { return captionCore; }
			set { SetValue(ref captionCore, value); }
		}
		DevExpress.Utils.DefaultBoolean allowCaptionDragMoveCore = DevExpress.Utils.DefaultBoolean.Default;
#if !SL
	[DevExpressXtraBarsLocalizedDescription("WindowsUIViewAllowCaptionDragMove")]
#endif
		[XtraSerializableProperty, DefaultValue(DevExpress.Utils.DefaultBoolean.Default), Category("Behavior")]
		public DevExpress.Utils.DefaultBoolean AllowCaptionDragMove {
			get { return allowCaptionDragMoveCore; }
			set {
				if(AllowCaptionDragMove == value) return;
				allowCaptionDragMoveCore = value;
				if(!CanCaptionDragMove())
					Ref.Dispose(ref dragMoveHelper);
			}
		}
		object actionButtonBackgroundImagesCore;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("WindowsUIViewActionButtonBackgroundImages"),
#endif
	  DefaultValue(null), Category("Appearance"), TypeConverter(typeof(DevExpress.Utils.Design.ImageCollectionImagesConverter))]
		public object ActionButtonBackgroundImages {
			get { return actionButtonBackgroundImagesCore; }
			set {
				if(ActionButtonBackgroundImages == value) return;
				actionButtonBackgroundImagesCore = value;
				OnButtonBackgroundImagesChanged();
			}
		}
		protected void OnButtonBackgroundImagesChanged() {
			ColoredElementsCache.Reset();
			if(Manager != null)
				Manager.LayoutChanged();
		}
		protected internal virtual bool CanCaptionDragMove() {
			return AllowCaptionDragMove == DevExpress.Utils.DefaultBoolean.True && !IsDesignMode();
		}
		DevExpress.Utils.DefaultBoolean useSplashScreenCore = DevExpress.Utils.DefaultBoolean.Default;
#if !SL
	[DevExpressXtraBarsLocalizedDescription("WindowsUIViewUseSplashScreen")]
#endif
		[Category("Behavior"), DefaultValue(DevExpress.Utils.DefaultBoolean.Default)]
		public virtual DevExpress.Utils.DefaultBoolean UseSplashScreen {
			get { return useSplashScreenCore; }
			set {
				if(UseSplashScreen == value) return;
				useSplashScreenCore = value;
				if(!CanUseSplashScreen())
					Ref.Dispose(ref splashScreenAdorner);
			}
		}
		protected internal virtual bool CanUseSplashScreen() {
			return UseSplashScreen != DevExpress.Utils.DefaultBoolean.False && !IsDesignMode()
				&& !System.Diagnostics.Debugger.IsAttached && !IsAboutOpen();
		}
		protected internal static bool IsAboutOpen() {
			bool isAboutOpen = false;
			return isAboutOpen;
		}
		DevExpress.Utils.DefaultBoolean useTransitionAnimationCore = DevExpress.Utils.DefaultBoolean.Default;
#if !SL
	[DevExpressXtraBarsLocalizedDescription("WindowsUIViewUseTransitionAnimation")]
#endif
		[Category("Behavior"), DefaultValue(DevExpress.Utils.DefaultBoolean.Default)]
		public virtual DevExpress.Utils.DefaultBoolean UseTransitionAnimation {
			get { return useTransitionAnimationCore; }
			set { useTransitionAnimationCore = value; }
		}
		protected internal virtual bool CanUseTransitionAnimation() {
			return UseTransitionAnimation != DevExpress.Utils.DefaultBoolean.False && !IsDesignMode();
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("WindowsUIViewContentContainers")]
#endif
		[Category("Layout"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Browsable(false), XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public ContentContainerCollection ContentContainers {
			get { return contentContainersCore; }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("WindowsUIViewContentContainerActions")]
#endif
		[Category("Behavior"), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ContentContainerActionCollection ContentContainerActions {
			get { return contentContainerActionsCore; }
		}
		protected virtual ContentContainerActionCollection CreateActions() {
			return new ContentContainerActionCollection(this);
		}
		void OnActionsCollectionChanged(CollectionChangedEventArgs<IContentContainerAction> ea) {
			LayoutChanged();
		}
		IOverviewContainerProperties overviewContainerPropertiesCore;
#if !SL
	[DevExpressXtraBarsLocalizedDescription("WindowsUIViewOverviewContainerProperties")]
#endif
		[Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public IOverviewContainerProperties OverviewContainerProperties {
			get { return overviewContainerPropertiesCore; }
		}
		bool ShouldSerializeOverviewContainerProperties() {
			return OverviewContainerProperties != null && OverviewContainerProperties.ShouldSerialize();
		}
		void ResetOverviewContainerProperties() {
			OverviewContainerProperties.Reset();
		}
		protected virtual IOverviewContainerProperties CreateOverviewContainerProperties() {
			return new OverviewContainerProperties();
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("WindowsUIViewDetailContainerProperties")]
#endif
		[Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public IContentContainerProperties DetailContainerProperties {
			get { return detailContainerPropertiesCore; }
		}
		bool ShouldSerializeDetailContainerProperties() {
			return DetailContainerProperties != null && DetailContainerProperties.ShouldSerialize();
		}
		void ResetDetailContainerProperties() {
			DetailContainerProperties.Reset();
		}
		protected virtual IContentContainerProperties CreateDetailContainerProperties() {
			return new DetailContainerProperties();
		}
		ISplashScreenProperties splashScreenPropertiesCore;
#if !SL
	[DevExpressXtraBarsLocalizedDescription("WindowsUIViewSplashScreenProperties")]
#endif
		[Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public ISplashScreenProperties SplashScreenProperties {
			get { return splashScreenPropertiesCore; }
		}
		bool ShouldSerializeSplashScreenProperties() {
			return SplashScreenProperties != null && SplashScreenProperties.ShouldSerialize();
		}
		void ResetSplashScreenProperties() {
			SplashScreenProperties.Reset();
		}
		protected virtual ISplashScreenProperties CreateSplashScreenProperties() {
			return new SplashScreenProperties();
		}
		[Category("Layout"), XtraSerializableProperty(XtraSerializationVisibility.Content), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("WindowsUIViewFlyoutProperties"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public IFlyoutProperties FlyoutProperties {
			get { return flyoutPropertiesCore; }
		}
		bool ShouldSerializeFlyoutProperties() {
			return ((FlyoutProperties != null) && FlyoutProperties.ShouldSerialize());
		}
		void ResetFlyoutProperties() {
			FlyoutProperties.Reset();
		}
		protected internal override DevExpress.Utils.ToolTipControlInfo GetToolTipControlInfo(BaseViewHitInfo hitInfo) {
			if(hitInfo.HitElement is PageGroupInfo) {
				return (hitInfo.HitElement as IDocumentSelectorInfo<PageGroup>).GetToolTipControlInfo(hitInfo);
			}
			return base.GetToolTipControlInfo(hitInfo);
		}
		#region Appearances
		protected override BaseViewAppearanceCollection CreateAppearanceCollection() {
			return new WindowsUIViewAppearanceCollection(this);
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("WindowsUIViewAppearanceCaption"),
#endif
 Category("Appearance"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DevExpress.Utils.AppearanceObject AppearanceCaption {
			get { return ((WindowsUIViewAppearanceCollection)AppearanceCollection).Caption; }
		}
		bool ShouldSerializeAppearanceCaption() {
			return !IsDisposing && AppearanceCollection != null && AppearanceCaption.ShouldSerialize();
		}
		void ResetAppearanceCaption() {
			AppearanceCaption.Reset();
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("WindowsUIViewAppearanceSplashScreen"),
#endif
 Category("Appearance"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DevExpress.Utils.AppearanceObject AppearanceSplashScreen {
			get { return ((WindowsUIViewAppearanceCollection)AppearanceCollection).SplashScreen; }
		}
		bool ShouldSerializeAppearanceSplashScreen() {
			return AppearanceCollection != null && AppearanceSplashScreen.ShouldSerialize();
		}
		void ResetAppearanceSplashScreen() {
			AppearanceSplashScreen.Reset();
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("WindowsUIViewAppearanceActionsBar"),
#endif
 Category("Appearance"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DevExpress.Utils.AppearanceObject AppearanceActionsBar {
			get { return ((WindowsUIViewAppearanceCollection)AppearanceCollection).ActionsBar; }
		}
		bool ShouldSerializeAppearanceActionsBar() {
			return AppearanceCollection != null && AppearanceActionsBar.ShouldSerialize();
		}
		void ResetAppearanceActionsBar() {
			AppearanceActionsBar.Reset();
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("WindowsUIViewAppearanceActionsBarButton"),
#endif
 Category("Appearance"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DevExpress.Utils.AppearanceObject AppearanceActionsBarButton {
			get { return ((WindowsUIViewAppearanceCollection)AppearanceCollection).ActionsBarButton; }
		}
		bool ShouldSerializeAppearanceActionsBarButton() {
			return !IsDisposing && AppearanceCollection != null && AppearanceActionsBarButton.ShouldSerialize();
		}
		void ResetAppearanceActionsBarButton() {
			AppearanceActionsBarButton.Reset();
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("WindowsUIViewAppearanceSearchPanel"),
#endif
 Category("Appearance"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DevExpress.Utils.AppearanceObject AppearanceSearchPanel {
			get { return ((WindowsUIViewAppearanceCollection)AppearanceCollection).SearchPanel; }
		}
		bool ShouldSerializeAppearanceSearchPanel() {
			return !IsDisposing && AppearanceCollection != null && AppearanceSearchPanel.ShouldSerialize();
		}
		void ResetAppearanceSearchPanel() {
			AppearanceSearchPanel.Reset();
		}
		#endregion Appearances
		#region Nested Properties
#if !SL
	[DevExpressXtraBarsLocalizedDescription("WindowsUIViewTileProperties")]
#endif
		[Category("Layout"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public IBaseTileProperties TileProperties {
			get { return tilePropertiesCore; }
		}
		bool ShouldSerializeTileProperties() {
			return TileProperties != null && TileProperties.ShouldSerialize();
		}
		void ResetTileProperties() {
			TileProperties.Reset();
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("WindowsUIViewPageProperties")]
#endif
		[Category("Layout"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public IPageProperties PageProperties {
			get { return pagePropertiesCore; }
		}
		bool ShouldSerializePageProperties() {
			return PageProperties != null && PageProperties.ShouldSerialize();
		}
		void ResetPageProperties() {
			PageProperties.Reset();
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("WindowsUIViewSplitGroupProperties")]
#endif
		[Category("Layout"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public ISplitGroupProperties SplitGroupProperties {
			get { return splitGroupPropertiesCore; }
		}
		bool ShouldSerializeSplitGroupProperties() {
			return SplitGroupProperties != null && SplitGroupProperties.ShouldSerialize();
		}
		void ResetSplitGroupProperties() {
			SplitGroupProperties.Reset();
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("WindowsUIViewSlideGroupProperties")]
#endif
		[Category("Layout"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public ISlideGroupProperties SlideGroupProperties {
			get { return slideGroupPropertiesCore; }
		}
		bool ShouldSerializeSlideGroupProperties() {
			return SlideGroupProperties != null && SlideGroupProperties.ShouldSerialize();
		}
		void ResetSlideGroupProperties() {
			SlideGroupProperties.Reset();
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("WindowsUIViewPageGroupProperties")]
#endif
		[Category("Layout"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public IPageGroupProperties PageGroupProperties {
			get { return pageGroupPropertiesCore; }
		}
		bool ShouldSerializePageGroupProperties() {
			return PageGroupProperties != null && PageGroupProperties.ShouldSerialize();
		}
		void ResetPageGroupProperties() {
			PageGroupProperties.Reset();
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("WindowsUIViewTabbedGroupProperties")]
#endif
		[Category("Layout"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public ITabbedGroupProperties TabbedGroupProperties {
			get { return tabbedGroupPropertiesCore; }
		}
		bool ShouldSerializeTabbedGroupProperties() {
			return TabbedGroupProperties != null && TabbedGroupProperties.ShouldSerialize();
		}
		void ResetTabbedGroupProperties() {
			TabbedGroupProperties.Reset();
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("WindowsUIViewTileContainerProperties")]
#endif
		[Category("Layout"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public ITileContainerProperties TileContainerProperties {
			get { return tileContainerPropertiesCore; }
		}
		bool ShouldSerializeTileContainerProperties() {
			return TileContainerProperties != null && TileContainerProperties.ShouldSerialize();
		}
		void ResetTileContainerProperties() {
			TileContainerProperties.Reset();
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("WindowsUIViewSearchPanelProperties")]
#endif
		[Category("Layout"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public ISearchPanelProperties SearchPanelProperties {
			get { return searchPanelPropertiesCore; }
		}
		bool ShouldSerializeSearchPanelProperties() {
			return SearchPanelProperties != null && SearchPanelProperties.ShouldSerialize();
		}
		void ResetSearchPanelProperties() {
			SearchPanelProperties.Reset();
		}
		protected virtual IBaseTileProperties CreateTileProperties() {
			return new BaseTileProperties();
		}
		protected virtual IPageProperties CreatePageProperties() {
			return new PageProperties();
		}
		protected virtual ISplitGroupProperties CreateSplitGroupProperties() {
			return new SplitGroupProperties();
		}
		protected virtual ISlideGroupProperties CreateSlideGroupProperties() {
			return new SlideGroupProperties();
		}
		protected virtual IPageGroupProperties CreatePageGroupProperties() {
			return new PageGroupProperties();
		}
		protected virtual ITabbedGroupProperties CreateTabbedGroupProperties() {
			return new TabbedGroupProperties();
		}
		protected virtual ITileContainerProperties CreateTileContainerProperties() {
			return new TileContainerProperties();
		}
		protected virtual IFlyoutProperties CreateFlyoutProperties() {
			return new FlyoutProperties();
		}
		protected virtual ISearchPanelProperties CreateSearchPanelPropertie() {
			return new SearchPanelProperties();
		}
		void OnTilePropertiesChanged(object sender, EventArgs e) {
			OnContentContainerPropertiesChanged<TileContainer>();
		}
		void OnPagePropertiesChanged(object sender, EventArgs e) {
			OnContentContainerPropertiesChanged<Page>();
		}
		void OnSplitGroupPropertiesChanged(object sender, EventArgs e) {
			OnContentContainerPropertiesChanged<SplitGroup>();
		}
		void OnSlideGroupPropertiesChanged(object sender, EventArgs e) {
			OnContentContainerPropertiesChanged<SlideGroup>();
		}
		void OnPageGroupPropertiesChanged(object sender, EventArgs e) {
			OnContentContainerPropertiesChanged<PageGroup>();
		}
		void OnTabbedGroupPropertiesChanged(object sender, EventArgs e) {
			OnContentContainerPropertiesChanged<PageGroup>();
		}
		void OnTileContainerPropertiesChanged(object sender, EventArgs e) {
			OnContentContainerPropertiesChanged<TileContainer>();
		}
		void OnOverviewContainerPropertiesChanged(object sender, EventArgs e) {
			OnContentContainerPropertiesChanged<OverviewContainer>();
		}
		void OnDetailContainerPropertiesChanged(object sender, EventArgs e) {
			OnContentContainerPropertiesChanged<DetailContainer>();
		}
		void OnFlyoutPropertiesChanged(object sender, EventArgs e) {
			OnContentContainerPropertiesChanged<Flyout>();
		}
		void OnContentContainerPropertiesChanged<T>() where T : IContentContainer {
			if(ActiveContentContainer is T) {
				using(LockPainting()) {
					RequestInvokePatchActiveChild();
				}
			}
		}
		#endregion Nested Properties
		#region XtraSerializable
		IDisposable contentContainersLockObj;
		List<SerializableObjectInfo> ObjectInfoCollection = new List<SerializableObjectInfo>();
		List<IContentContainer> containersToRemove;
		protected override void BeginRestoreLayout() {
			base.BeginRestoreLayout();
			contentContainersLockObj = CleanUpContentContainers();
			containersToRemove = new List<IContentContainer>();
		}
		protected override void EndRestoreLayout() {
			RestoreContentContainers();
			Ref.Dispose(ref contentContainersLockObj);
			containersToRemove.Clear();
			ObjectInfoCollection.Clear();
			base.EndRestoreLayout();
		}
		protected IDisposable CleanUpContentContainers() {
			IDisposable result = ContentContainers.LockCollectionChanged();
			IContentContainer[] contentContainers = ContentContainers.CleanUp();
			for(int i = 0; i < contentContainers.Length; i++) {
				IContentContainer contentContainer = contentContainers[i];
				ClearContentContainer(contentContainer);
				UnRegisterContentContainer(contentContainer);
			}
			return result;
		}
		protected void ClearContentContainer(IContentContainer contentContainer) {
			if(contentContainer is DocumentGroup) ((DocumentGroup)contentContainer).Items.CleanUp();
			if(contentContainer is TileContainer) ((TileContainer)contentContainer).Items.CleanUp();
			if(contentContainer is Page) ((Page)contentContainer).Document = null;
			if(contentContainer is Flyout) ((Flyout)contentContainer).Document = null;
		}
		protected void RestoreContentContainers() {
			var itemsAndNames = new Dictionary<string, SerializableObjectInfo>();
			foreach(SerializableObjectInfo component in Items)
				itemsAndNames.Add(component.Name, component);
			List<IContentContainer> containers = new List<IContentContainer>();
			List<Tile> tiles = new List<Tile>();
			Tiles.Clear();
			foreach(SerializableObjectInfo info in Items) {
				SerializableTileInfo tileInfo = info as SerializableTileInfo;
				if(tileInfo != null) {
					Tile tile = (Tile)info.Source;
					tile.Name = info.Name;
					tile.Document = (Document)TryGetValue(tileInfo.Document, itemsAndNames);
					tile.ActivationTarget = (IContentContainer)TryGetValue(tileInfo.ActivateTarget, itemsAndNames);
					tile.Group = tileInfo.Group;
					tiles.Add(tile);
				}
				SerializableBaseContentContainerInfo containerInfo = info as SerializableBaseContentContainerInfo;
				if(containerInfo != null) {
					IContentContainer container = (IContentContainer)info.Source;
					container.Name = info.Name;
					containers.Add(container);
				}
			}
			DeleteDuplicatingDocumentsFromTiles(tiles, tiles.Count);
			Tiles.AddRange(tiles.ToArray());
			foreach(IContentContainer container in containers) {
				RestoreContentContainerItems(container, itemsAndNames);
			}
			foreach(IContentContainer containerToRemove in containersToRemove) {
				containers.Remove(containerToRemove);
				DestroyEmptyContentContainer(containerToRemove);
			}
			ContentContainers.AddRange(containers);
			if(!RestoreActiveContentContainer(containers))
				EnsureActiveContentContainer();
			RestoreSelectedDocuments(itemsAndNames);
		}
		void RestoreSelectedDocuments(Dictionary<string, SerializableObjectInfo> itemsAndNames) {
			foreach(IContentContainer container in ContentContainers) {
				IDocumentSelector selector = container as IDocumentSelector;
				if(selector != null) {
					string selectedDocument = ((SerializableDocumentGroupInfo)Infos[container]).SelectedDocument;
					SerializableObjectInfo documentInfo;
					if(selectedDocument != null && itemsAndNames.TryGetValue(selectedDocument, out documentInfo))
						selector.SetSelected((Document)documentInfo.Source);
				}
			}
		}
		bool RestoreActiveContentContainer(List<IContentContainer> containers) {
			foreach(IContentContainer container in containers) {
				SerializableBaseContentContainerInfo contentContainerInfo = (SerializableBaseContentContainerInfo)Infos[container];
				if(contentContainerInfo != null) {
					if(contentContainerInfo.IsActive) {
						SetActiveContentContainerCore(container);
						return true;
					}
				}
			}
			activeContentContainerCore = null;
			return false;
		}
		void RestoreContentContainerItems(IContentContainer item, IDictionary<string, SerializableObjectInfo> itemsAndNames) {
			SerializableBaseContentContainerInfo info = (SerializableBaseContentContainerInfo)itemsAndNames[item.Name];
			string[] documents = ParseString(((SerializableBaseContentContainerInfo)itemsAndNames[item.Name]).Items);
			SerializableObjectInfo objectInfo;
			if(info.Parent != null) {
				SerializableObjectInfo parentInfo;
				if(itemsAndNames.TryGetValue(info.Parent, out parentInfo))
					item.Parent = (IContentContainer)parentInfo.Source;
			}
			string[] length = item is DocumentGroup ? ParseString(((SerializableDocumentGroupInfo)info).DocumentLength) : new string[] { };
			string[] id = item is TileContainer ? ParseString(((SerializableTileContainerInfo)info).TileId) : new string[] { };
			int count = 0;
			for(int i = 0; i < documents.Length; i++) {
				if(itemsAndNames.TryGetValue(documents[i], out objectInfo)) {
					count++;
					if(item is DocumentGroup) {
						((DocumentGroup)item).Items.Add((Document)objectInfo.Source);
						((DocumentGroup)item).SetLength((Document)objectInfo.Source, Int32.Parse(length[i]));
					}
					if(item is TileContainer) {
						((TileContainer)item).Items.Add((Tile)objectInfo.Source);
						((TileContainer)item).SetID((Tile)objectInfo.Source, Int32.Parse(id[i]));
					}
					if(item is Page) ((Page)item).Document = ((Document)objectInfo.Source);
					if(item is Flyout) ((Flyout)item).Document = ((Document)objectInfo.Source);
				}
			}
			if(count == 0 && item.Properties.ActualDestroyOnRemovingChildren && !((IDesignTimeSupport)this).IsSerializing)
				containersToRemove.Add(item);
		}
		void DestroyEmptyContentContainer(IContentContainer container) {
			container.Parent = null;
			container.Dispose();
		}
		protected string[] ParseString(string str) {
			return str != null ? str.Split(',') : new string[] { };
		}
		protected object TryGetValue(string document, IDictionary<string, SerializableObjectInfo> collection) {
			SerializableObjectInfo objectInfo;
			if(document != null && collection.TryGetValue(document, out objectInfo))
				return objectInfo.Source;
			return null;
		}
		protected void DeleteDuplicatingDocumentsFromTiles(List<Tile> tiles, int length) {
			for(int i = 0; i < length - 1; i++) {
				for(int j = i + 1; j < length; j++) {
					if(tiles[i].Document == tiles[j].Document) {
						if(ObjectInfoCollection.Contains(Infos[tiles[i]])) tiles[j].Document = null;
						else tiles[i].Document = null;
					}
				}
			}
		}
		protected override void PrepareSerializableObjectInfos() {
			base.PrepareSerializableObjectInfos();
			PrepareSerializableContentContainersInfos();
			PrepareSerializableTileInfos();
		}
		private void PrepareSerializableTileInfos() {
			for(int i = 0; i < Tiles.Count; i++) {
				Tile tile = (Tile)Tiles[i];
				SerializableObjectInfo info = new SerializableTileInfo(tile);
				RegisterSerializableObjectinfo(tile, info);
				info.SetName(tile.Name);
			}
		}
		protected virtual void PrepareSerializableContentContainersInfos() {
			IContentContainer[] contentContainerGroups = ContentContainers.ToArray();
			IContentContainer contentContainer;
			SerializableObjectInfo info = null;
			for(int i = 0; i < contentContainerGroups.Length; i++) {
				contentContainer = contentContainerGroups[i];
				if(contentContainer is DocumentGroup)
					info = new SerializableDocumentGroupInfo((DocumentGroup)contentContainer);
				if(contentContainer is TileContainer)
					info = new SerializableTileContainerInfo((TileContainer)contentContainer);
				if(contentContainer is Page)
					info = new SerializablePageInfo((Page)contentContainer);
				if(contentContainer is Flyout)
					info = new SerializableFlyoutInfo((Flyout)contentContainer);
				info.SetName(contentContainer.Name);
				RegisterSerializableObjectinfo(contentContainer, info);
			}
		}
		protected override BaseSerializableDocumentInfo CreateSerializableDocumentInfo(BaseDocument document) {
			return new SerializableDocumentInfo(document as Document);
		}
		protected override object XtraFindItemsItem(XtraItemEventArgs e) {
			SerializableObjectInfo objectInfo = base.XtraFindItemsItem(e) as SerializableObjectInfo;
			if(objectInfo != null)
				ObjectInfoCollection.Add(objectInfo);
			return objectInfo;
		}
		protected override object XtraCreateItemsItem(XtraItemEventArgs e) {
			SerializableObjectInfo objectInfo = base.XtraCreateItemsItem(e) as SerializableObjectInfo;
			if(objectInfo != null)
				ObjectInfoCollection.Add(objectInfo);
			return objectInfo;
		}
		protected override SerializableObjectInfo CreateSerializableObjectInfo(string typeName) {
			switch(typeName) {
				case "SplitGroup":
					DocumentGroup splitGroup = CreateSplitGroup();
					return RegisterDocumentGroupObjectInfo(splitGroup); ;
				case "SlideGroup":
					DocumentGroup slideGroup = CreateSlideGroup();
					return RegisterDocumentGroupObjectInfo(slideGroup);
				case "PageGroup":
					DocumentGroup pageGroup = CreatePageGroup();
					return RegisterDocumentGroupObjectInfo(pageGroup);
				case "TabbedGroup":
					DocumentGroup tabbedGroup = CreateTabbedGroup();
					return RegisterDocumentGroupObjectInfo(tabbedGroup);
				case "Page":
					Page page = CreatePage();
					return RegisterPageObjectInfo(page);
				case "Flyout":
					Flyout flyout = CreateFlyout();
					return RegisterFlyoutObjectInfo(flyout);
				case "TileContainer":
					TileContainer tileContainer = CreateTileContainer();
					return RegisterTileContainerObjectInfo(tileContainer);
				case "Tile":
					Tile tile = CreateTile(null);
					return RegisterTileObjectInfo(tile);
			}
			return base.CreateSerializableObjectInfo(typeName);
		}
		protected SerializableObjectInfo RegisterTileObjectInfo(Tile tile) {
			tile.Properties.EnsureParentProperties(TileProperties);
			SerializableTileInfo tileInfo = new SerializableTileInfo(tile);
			RegisterSerializableObjectinfo(tile, tileInfo);
			return tileInfo;
		}
		protected SerializableObjectInfo RegisterTileContainerObjectInfo(TileContainer tileContainer) {
			((IContentContainerInternal)tileContainer).IsLoaded = true;
			((IContentContainerInternal)tileContainer).SetManager(Manager);
			((IContentContainerInternal)tileContainer).EnsureParentProperties(this);
			SerializableTileContainerInfo tileContainerInfo = new SerializableTileContainerInfo(tileContainer);
			RegisterSerializableObjectinfo(tileContainer, tileContainerInfo);
			return tileContainerInfo;
		}
		protected SerializablePageInfo RegisterPageObjectInfo(Page page) {
			((IContentContainerInternal)page).IsLoaded = true;
			((IContentContainerInternal)page).SetManager(Manager);
			((IContentContainerInternal)page).EnsureParentProperties(this);
			SerializablePageInfo pageInfo = new SerializablePageInfo(page);
			RegisterSerializableObjectinfo(page, pageInfo);
			return pageInfo;
		}
		protected SerializableFlyoutInfo RegisterFlyoutObjectInfo(Flyout flyout) {
			((IContentContainerInternal)flyout).IsLoaded = true;
			((IContentContainerInternal)flyout).SetManager(Manager);
			((IContentContainerInternal)flyout).EnsureParentProperties(this);
			SerializableFlyoutInfo flyoutInfo = new SerializableFlyoutInfo(flyout);
			RegisterSerializableObjectinfo(flyout, flyoutInfo);
			return flyoutInfo;
		}
		protected SerializableDocumentGroupInfo RegisterDocumentGroupObjectInfo(DocumentGroup group) {
			((IContentContainerInternal)group).IsLoaded = true;
			((IContentContainerInternal)group).SetManager(Manager);
			((IContentContainerInternal)group).EnsureParentProperties(this);
			SerializableDocumentGroupInfo result = new SerializableDocumentGroupInfo(group);
			RegisterSerializableObjectinfo(group, result);
			return result;
		}
		#region Serializable Object Infos
		protected class SerializableBaseContentContainerInfo : SerializableObjectInfo {
			IContentContainerDefaultProperties propertiesCore;
			string type;
			public SerializableBaseContentContainerInfo(object source)
				: base(source) {
			}
			protected override string GetTypeNameCore() {
				return type;
			}
			public string Type { get { return type; } protected set { type = value; } }
			[XtraSerializableProperty]
			public string Items { get; set; }
			[XtraSerializableProperty]
			public string Parent { get; set; }
			[XtraSerializableProperty]
			public bool IsActive { get; set; }
			[XtraSerializableProperty(XtraSerializationVisibility.Content)]
			public IContentContainerDefaultProperties Properties {
				get { return propertiesCore; }
				set { propertiesCore = value; }
			}
		}
		protected class SerializableDocumentGroupInfo : SerializableBaseContentContainerInfo {
			string length;
			string selectedDocument;
			public SerializableDocumentGroupInfo(DocumentGroup group)
				: base(group) {
				Properties = group.Properties;
				if(group.Parent != null) Parent = group.Parent.Name;
				Type = group.GetType().Name;
				foreach(Document doc in group.Items) {
					Items += doc.GetName() + ",";
					length += group.GetLength(doc) + ",";
				}
				if(group.Items.Count != 0) {
					Items = Items.Remove(Items.Length - 1, 1);
					length = length.Remove(length.Length - 1, 1);
				}
				IsActive = group.IsActive;
				IDocumentSelector selector = group as IDocumentSelector;
				if(selector != null && selector.SelectedDocument != null)
					selectedDocument = selector.SelectedDocument.ControlName;
			}
			[XtraSerializableProperty]
			public string SelectedDocument {
				get { return selectedDocument; }
				set { selectedDocument = value; }
			}
			[XtraSerializableProperty]
			public string DocumentLength {
				get { return length; }
				set { length = value; }
			}
		}
		protected class SerializableDocumentInfo : BaseSerializableDocumentInfo {
			string type;
			public SerializableDocumentInfo(Document document)
				: base(document) {
				type = document.GetType().Name;
			}
			protected override string GetTypeNameCore() {
				return type;
			}
		}
		protected class SerializableTileInfo : SerializableObjectInfo {
			IBaseTileDefaultProperties propertiesCore;
			string type;
			string group;
			string document;
			string activateTarget;
			public SerializableTileInfo(Tile tile)
				: base(tile) {
				propertiesCore = tile.Properties;
				type = tile.GetType().Name;
				group = tile.Group;
				if(tile.Document != null)
					document = tile.Document.GetName();
				activateTarget = tile.ActivationTarget != null ? tile.ActivationTarget.Name : null;
			}
			[XtraSerializableProperty]
			public string Document {
				get { return document; }
				set { document = value; }
			}
			[XtraSerializableProperty]
			public string ActivateTarget {
				get { return activateTarget; }
				set { activateTarget = value; }
			}
			[XtraSerializableProperty]
			public string Group {
				get { return group; }
				set { group = value; }
			}
			[XtraSerializableProperty(XtraSerializationVisibility.Content)]
			public IBaseTileDefaultProperties Properties {
				get { return propertiesCore; }
			}
			protected override string GetTypeNameCore() {
				return type;
			}
		}
		protected class SerializableTileContainerInfo : SerializableBaseContentContainerInfo {
			string id;
			public SerializableTileContainerInfo(TileContainer tileContainer)
				: base(tileContainer) {
				Properties = tileContainer.Properties;
				if(tileContainer.Parent != null) Parent = tileContainer.Parent.Name;
				Type = tileContainer.GetType().Name;
				foreach(BaseTile tile in tileContainer.Items) {
					Items += tile.Name + ",";
					id += tileContainer.GetID(tile) + ",";
				}
				if(tileContainer.Items.Count != 0) {
					Items = Items.Remove(Items.Length - 1, 1);
					id = id.Remove(id.Length - 1, 1);
				}
				IsActive = tileContainer.IsActive;
			}
			[XtraSerializableProperty]
			public string TileId {
				get { return id; }
				set { id = value; }
			}
		}
		protected class SerializablePageInfo : SerializableBaseContentContainerInfo {
			public SerializablePageInfo(Page page)
				: base(page) {
				Properties = page.Properties;
				if(page.Parent != null) Parent = page.Parent.Name;
				Type = page.GetType().Name;
				if(page.Document != null) Items = page.Document.GetName();
				IsActive = page.IsActive;
			}
		}
		protected class SerializableFlyoutInfo : SerializableBaseContentContainerInfo {
			public SerializableFlyoutInfo(Flyout flyout)
				: base(flyout) {
				Properties = flyout.Properties;
				if(flyout.Parent != null) Parent = flyout.Parent.Name;
				Type = flyout.GetType().Name;
				if(flyout.Document != null) Items = flyout.Document.GetName();
				IsActive = flyout.IsActive;
			}
		}
		#endregion Serializable Object Infos
		#endregion XtraSerializable
		#region Tiles
#if !SL
	[DevExpressXtraBarsLocalizedDescription("WindowsUIViewTiles")]
#endif
		[Browsable(false), Category("Layout"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public BaseTileCollection Tiles {
			get { return tilesCore; }
		}
		protected virtual BaseTileCollection CreateTileCollection() {
			return new BaseTileCollection(this);
		}
		void OnTileCollectionChanged(CollectionChangedEventArgs<BaseTile> ea) {
			BaseTile tile = ea.Element;
			if(ea.ChangedType == CollectionChangedType.ElementAdded) {
				OnTileAdded(tile);
				RaiseTileAdded(tile);
			}
			if(ea.ChangedType == CollectionChangedType.ElementRemoved) {
				OnTileRemoved(tile);
				RaiseTileRemoved(tile);
			}
			LayoutChanged();
		}
		protected virtual void OnTileAdded(BaseTile tile) {
			tile.Properties.EnsureParentProperties(TileProperties);
			tile.SetManager(Manager);
			InvalidateUIView();
		}
		protected virtual void OnTileRemoved(BaseTile tile) {
			tile.SetManager(null);
			InvalidateUIView();
		}
		#endregion Tiles
		#region ContentContainers
		protected virtual ContentContainerCollection CreateContentContainers() {
			return new ContentContainerCollection(this);
		}
		[Browsable(false)]
		public IContentContainer ActiveContentContainer {
			get { return activeContentContainerCore; }
		}
		[Browsable(false)]
		public ContextualZoomLevel ZoomLevel {
			get {
				IContentContainerInternal container = ActiveContentContainer as IContentContainerInternal;
				return (container != null) ? container.ZoomLevel : ContextualZoomLevel.Normal;
			}
		}
		void OnContentContainersCollectionChanged(CollectionChangedEventArgs<IContentContainer> ea) {
			IContentContainer contentContainer = ea.Element;
			CheckActiveContentContainer(ea);
			if(ea.ChangedType == CollectionChangedType.ElementAdded) {
				AddToContainer(contentContainer);
				OnContentContainerAdded(contentContainer);
				RaiseContentContainerAdded(contentContainer);
			}
			if(ea.ChangedType == CollectionChangedType.ElementRemoved) {
				OnContentContainerRemoved(contentContainer);
				RemoveFromContainer(contentContainer);
				RaiseContentContainerRemoved(contentContainer);
			}
			SetLayoutModified();
			LayoutChanged();
		}
		void CheckActiveContentContainer(CollectionChangedEventArgs<IContentContainer> ea) {
			if(ea.ChangedType == CollectionChangedType.ElementDisposed || ea.ChangedType == CollectionChangedType.ElementRemoved) {
				if(ActiveContentContainer == ea.Element)
					SetActiveContentContainerCore(null);
			}
			if(IsInitialized && ea.ChangedType == CollectionChangedType.ElementAdded) {
				if(ActiveContentContainer == null)
					SetActiveContentContainerCore(ea.Element);
			}
		}
		internal int contentContainerContextActionBarActivating = 0;
		protected internal bool SetActiveContentContainerCore(IContentContainer contentContainer) {
			if(ActiveContentContainer == contentContainer && !IsDeserializing) return false;
			if(patchChildrenInProgress > 0)
				retryPatchActiveChildrenFlag = true;
			HideNavigationAdorner();
			CancelSearchPanelAdorner();
			if(contentContainer is Flyout) {
				return SetActiveFlyoutContainerCore(contentContainer);
			}
			StartNavigation(ActiveContentContainer ?? ActiveFlyoutContainer, contentContainer);
			if(ActiveContentContainer != null)
				OnContentContainerDeactivated(ActiveContentContainer);
			if(ActiveFlyoutContainer != null) {
				OnContentContainerDeactivated(ActiveFlyoutContainer);
				activeFlyoutContainerCore = null;
			}
			activeContentContainerCore = contentContainer;
			if(ActiveContentContainer != null) {
				OnContentContainerActivated(ActiveContentContainer);
				if(ActiveContentContainer.Properties.CanShowContextActionBarOnActivating) {
					var container = contentContainer as IContentContainerInternal;
					if(container != null)
						container.RequestContextActionBarActivation();
				}
			}
			InvalidateUIView();
			LayoutChanged();
			return activeContentContainerCore == contentContainer;
		}
		internal int flyoutDeactivation;
		protected internal bool IsFlyoutDeactivation {
			get { return flyoutDeactivation > 0; }
		}
		protected internal bool SetActiveFlyoutContainerCore(IContentContainer contentContainer) {
			Flyout flyoutContainer = contentContainer as Flyout;
			if(flyoutContainer != null && flyoutContainer.Action == null && flyoutContainer.Document == null) return false;
			if(ActiveFlyoutContainer == contentContainer && !IsDeserializing) return false;
			HideNavigationAdorner();
			CancelSearchPanelAdorner();
			if(ActiveFlyoutContainer != null)
				DeactivateFlyoutContainerCore(ActiveFlyoutContainer as Flyout);
			if(flyoutContainer != null) {
				StartNavigation(ActiveContentContainer, flyoutContainer);
				ActivateFlyoutContainerCore(flyoutContainer);
			}
			InvalidateUIView();
			LayoutChanged();
			return activeFlyoutContainerCore == flyoutContainer;
		}
		FlyoutDialogForm flyoutDialogForm;
		public DialogResult ShowFlyoutDialog(Flyout flyout) {
			try {
				AdornerElementInfo elementInfo = null;
				FlyoutAdornerElementInfoArgs.EnsureInfoArgs(ref elementInfo, this);
				flyoutDialogForm = CreateFlyoutDialogForm(elementInfo, flyout);
				HideNavigationAdorner();
				CancelSearchPanelAdorner();
				return flyoutDialogForm.ShowDialog();
			}
			finally {
				flyoutDialogForm = null;
			}
		}
		protected virtual FlyoutDialogForm CreateFlyoutDialogForm(AdornerElementInfo elementInfo, Flyout flyout) {
			FlyoutDialogForm flyoutDialogForm = new FlyoutDialogForm(elementInfo, flyout);
			Control ownerControl = Manager.GetOwnerControl();
			var ownerForm = ownerControl.FindForm();
			flyoutDialogForm.Owner = ownerForm;
			if(ownerForm != null) {
				flyoutDialogForm.RightToLeft = ownerForm.RightToLeft;
				flyoutDialogForm.RightToLeftLayout = ownerForm.RightToLeftLayout;
				flyoutDialogForm.TopMost = ownerForm.TopMost;
			}
			flyoutDialogForm.Location = ownerControl.PointToScreen(Point.Empty);
			flyoutDialogForm.Size = Bounds.Size;
			return flyoutDialogForm;
		}
		[Browsable(false)]
		public IContentContainer ActiveFlyoutContainer {
			get { return activeFlyoutContainerCore; }
		}
		protected void ActivateFlyoutContainerCore(IContentContainer contentContainer) {
			if(RaiseFlyoutShowing()) {
				activeFlyoutContainerCore = contentContainer;
				if(ActiveFlyoutContainer != null) {
					OnContentContainerActivated(ActiveFlyoutContainer);
				}
			}
		}
		protected void DeactivateFlyoutContainerCore(Flyout activeFlyout) {
			if(RaiseFlyoutHiding() && ActiveFlyoutContainer != null) {
				if(ActiveFlyoutContainer != null)
					StartNavigation(ActiveFlyoutContainer, activeFlyout ?? ActiveContentContainer);
				OnContentContainerDeactivated(ActiveFlyoutContainer);
				Manager.InvokePatchActiveChildren();
				activeFlyoutContainerCore = null;
			}
		}
		public void HideFlyout() {
			SetActiveFlyoutContainerCore(null);
		}
		public void HideFlyout(DialogResult result) {
			if(ActiveFlyoutContainer != null) {
				flyoutBootStrapperCore.SetDialogResult(result);
				HideFlyout();
			}
			if(flyoutDialogForm != null)
				flyoutDialogForm.DialogResult = result;
		}
		public void ActivateDocument(BaseDocument document) {
			if(Controller != null)
				Controller.Activate(document);
		}
		public void ActivateTile(BaseTile tile) {
			if(Controller != null)
				Controller.Activate(tile);
		}
		public void ActivateContainer(IContentContainer container) {
			if(Controller != null)
				Controller.Activate(container);
		}
		Document documentForDelayedActivation;
		protected internal void ActivateDocument(Document document) {
			using(BatchUpdate.Enter(this, true)) {
				if(IsInitialized && ActiveDocument != document)
					this.documentForDelayedActivation = document;
				SetActiveDocumentCore(document);
			}
		}
		IEnumerable<IDocumentAction> currentlyUsedDocumentActions;
		protected internal IEnumerable<IDocumentAction> GetCurrentlyUsedDocumentActions() {
			return currentlyUsedDocumentActions;
		}
		protected internal IDocumentActionsArgs GetDocumentActions(IContentContainer container) {
			return GetDocumentActions(container, ActiveDocument as Document);
		}
		protected internal IDocumentActionsArgs GetDocumentActions(IContentContainer container, Document document) {
			QueryDocumentActionsEventArgs e = new QueryDocumentActionsEventArgs(this, container, document);
			RaiseQueryDocumentActions(e);
			ISupportDocumentActions supportDocumentActions = document.Control as ISupportDocumentActions;
			if(supportDocumentActions != null)
				supportDocumentActions.OnQueryDocumentActions(e);
			this.currentlyUsedDocumentActions = e.DocumentActions;
			return e;
		}
		protected internal void NotifyNavigatedTo() {
			INavigationContext context;
			if(NavigationContext.TryGetValue(this, out context)) {
				NavigationEventArgs e = new NavigationEventArgs(this, null, context.Target, context.Source, context.Tag);
				e.Parameter = context.Parameter;
				RaiseNavigatedTo(e);
			}
		}
		protected internal void NotifyNavigatedTo(Document document) {
			INavigationContext context;
			if(document != null && NavigationContext.TryGetValue(this, out context)) {
				NavigationEventArgs e = new NavigationEventArgs(this, document, context.Target, context.Source, context.Tag);
				e.Parameter = context.Parameter;
				RaiseNavigatedTo(e);
				ISupportNavigation supportNavigation = document.Control as ISupportNavigation;
				if(supportNavigation != null)
					supportNavigation.OnNavigatedTo(new NavigationArgs(e));
			}
		}
		protected internal void NotifyNavigatedFrom() {
			INavigationContext context;
			if(NavigationContext.TryGetValue(this, out context)) {
				NavigationEventArgs e = new NavigationEventArgs(this, null, context.Target, context.Source, context.Tag);
				RaiseNavigatedFrom(e);
				context.Parameter = e.Parameter;
			}
		}
		protected internal void NotifyNavigatedFrom(Document document) {
			INavigationContext context;
			if(document != null && NavigationContext.TryGetValue(this, out context)) {
				NavigationEventArgs e = new NavigationEventArgs(this, document, context.Target, context.Source, context.Tag);
				ISupportNavigation supportNavigation = document.Control as ISupportNavigation;
				if(supportNavigation != null)
					supportNavigation.OnNavigatedFrom(new NavigationArgs(e));
				RaiseNavigatedFrom(e);
				context.Parameter = e.Parameter;
			}
		}
		protected virtual void OnContentContainerAdded(IContentContainer contentContainer) {
			((IContentContainerInternal)contentContainer).IsLoaded = true;
			((IContentContainerInternal)contentContainer).EnsureParentProperties(this);
			((IContentContainerInternal)contentContainer).SetManager(Manager);
			InvalidateUIView();
		}
		protected virtual void OnContentContainerRemoved(IContentContainer contentContainer) {
			((IContentContainerInternal)contentContainer).SetManager(null);
			((IContentContainerInternal)contentContainer).IsLoaded = false;
			((IContentContainerInternal)contentContainer).ContainerRemoved();
			InvalidateUIView();
		}
		protected virtual void OnContentContainerActivated(IContentContainer contentContainer) {
			((IContentContainerInternal)contentContainer).Activate(this);
			RegisterContentContainer(contentContainer);
			RaiseContentContainerActivated(contentContainer);
		}
		protected virtual void OnContentContainerDeactivated(IContentContainer contentContainer) {
			RaiseContentContainerDeactivated(contentContainer);
			UnRegisterContentContainer(contentContainer);
			((IContentContainerInternal)contentContainer).Deactivate();
		}
		protected internal void RegisterContentContainer(IContentContainer contentContainer) {
			if(ViewInfo != null)
				((WindowsUIViewInfo)ViewInfo).RegisterContentContainer(contentContainer);
		}
		protected internal void UnRegisterContentContainer(IContentContainer contentContainer) {
			if(ViewInfo != null)
				((WindowsUIViewInfo)ViewInfo).UnRegisterContentContainer(contentContainer);
		}
		#endregion ContentContainers
		protected override void OnDocumentAdded(BaseDocument document) {
			base.OnDocumentAdded(document);
			if(!IsInitializing && !IsDeserializing && ensureContentContainerInProgress == 0 && CanAddTileWhenCreatingDocument())
				Controller.AddTile(document as Document);
		}
		protected override void OnDocumentRemoved(BaseDocument document) {
			if(!IsInitializing && !IsDeserializing && ensureContentContainerInProgress == 0) {
				Controller.RemoveTile(document as Document);
				DeactivateDocumentContainer(document);
			}
			base.OnDocumentRemoved(document);
		}
		protected void DeactivateDocumentContainer(BaseDocument document) {
			IDocumentContainer documentContainer = ActiveContentContainer as IDocumentContainer;
			if(documentContainer != null && documentContainer.Document == document) {
				IContentContainer parent = documentContainer.Parent;
				if(parent != null && !parent.IsDisposing)
					SetActiveContentContainerCore(parent);
				else SetActiveContentContainerCore(null);
			}
		}
		protected override void OnLoaded() {
			base.OnLoaded();
			CreateFlyoutAdorner();
			CreateNavigationAdorner();
			CreateSearchPanelAdorner();
			foreach(BaseTile tile in Tiles)
				Tiles.EnsureRegistered(tile);
		}
		protected override void OnUnloaded() {
			DestroyNavigationContext();
			DestroyFlyoutAdorner();
			DestroyNavigationAdorner();
			DestroySearchPanelAdorner();
			if(ActiveContentContainer != null)
				OnContentContainerDeactivated(ActiveContentContainer);
			Ref.Dispose(ref dragMoveHelper);
			this.activeContentContainerCore = null;
			base.OnUnloaded();
		}
		protected internal override void OnDepopulating() {
			if(ActiveContentContainer != null && ActiveContentContainer.IsAutoCreated)
				SetActiveContentContainerCore(null);
			base.OnDepopulating();
		}
		IDisposable dragMoveHelper;
		protected override void OnShown() {
			base.OnShown();
			EnsureActiveContentContainer();
			EnsureCaptionDragMove();
		}
		protected override void OnInitialized() {
			if(IsLoading && CanUseSplashScreen()) {
				UpdateStyleCore();
				splashScreenAdorner = ShowSplashScreen();
			}
			base.OnInitialized();
		}
		protected override void OnBeforeFirstDraw() {
			using(LockComponentChangeNotifications()) {
				EnsureActiveContentContainer();
			}
			base.OnBeforeFirstDraw();
			Ref.Dispose(ref splashScreenAdorner);
		}
		AsyncAdornerElementInfo splashScreenInfo;
		DevExpress.Utils.Animation.IAsyncAdorner splashScreenAdorner;
		protected internal DevExpress.Utils.Animation.IAsyncAdorner ShowSplashScreen() {
			SplashScreenAdornerInfoArgs args = SplashScreenAdornerInfoArgs.EnsureInfoArgs(ref splashScreenInfo, this);
			IntPtr hWnd = DevExpress.Utils.Drawing.Helpers.NativeMethods.GetDesktopWindow();
			return AsyncAdornerBootStrapper.Show(hWnd, splashScreenInfo);
		}
		AsyncAdornerElementInfo transitionScreenInfo;
		[Browsable(false)]
		public bool IsTransitionAnimationInProgress {
			get {
				if(transitionScreenInfo != null && transitionScreenInfo.InfoArgs != null) {
					var args = transitionScreenInfo.InfoArgs as TransitionScreenAdornerInfoArgs;
					return (args.TransitionAnimator) != null && args.TransitionAnimator.AnimationInProgress;
				}
				return false;
			}
		}
		protected internal void StartTransitionAnimation(Document from, Document to, AnimationParameters parameters) {
			if(!CanUseTransitionAnimation() || (from == null) || (to == null)) return;
			if(from == to || !from.IsControlLoaded || !to.IsControlLoaded) return;
			TransitionScreenAdornerInfoArgs args = TransitionScreenAdornerInfoArgs.EnsureInfoArgs(ref transitionScreenInfo, this, from, to);
			args.Parameters = parameters;
			if(args.TransitionAnimator == null || !args.TransitionAnimator.AnimationInProgress) {
				IntPtr hWnd = Manager.GetOwnerControlHandle();
				AsyncAdornerBootStrapper.Show(hWnd, transitionScreenInfo);
			}
		}
		protected override void DestroyAsyncAnimations() {
			base.DestroyAsyncAnimations();
			if(transitionScreenInfo != null && transitionScreenInfo.InfoArgs != null)
				transitionScreenInfo.InfoArgs.Destroy();
			if(splashScreenInfo != null && splashScreenInfo.InfoArgs != null)
				splashScreenInfo.InfoArgs.Destroy();
		}
		int ensureContentContainerInProgress = 0;
		void EnsureActiveContentContainer() {
			if(ActiveContentContainer != null) return;
			IContentContainer startupContainer = RaiseQueryStartupContentContainer();
			if(startupContainer == null) {
				foreach(IContentContainer container in ContentContainers) {
					if(container.Parent != null || (container is Flyout && IsDesignMode())) continue;
					startupContainer = container;
					break;
				}
			}
			else {
				using(var enumerator = startupContainer.GetEnumerator()) {
					using(BatchUpdate.Enter(this, true)) {
						ensureContentContainerInProgress++;
						try {
							Document[] documents;
							HashSet<Document> documentsHash = new HashSet<Document>();
							while(enumerator.MoveNext()) {
								ContentContainers.Add(enumerator.Current);
								IContentContainerInternal contentContainer = enumerator.Current as IContentContainerInternal;
								if(contentContainer != null) {
									documents = contentContainer.GetDocuments();
									for(int i = 0; i < documents.Length; i++)
										documentsHash.Add(documents[i]);
								}
							}
							documents = new Document[documentsHash.Count];
							documentsHash.CopyTo(documents);
							Documents.AddRange(documents);
						}
						finally { ensureContentContainerInProgress--; }
					}
				}
			}
			SetActiveContentContainerCore(startupContainer);
		}
		void EnsureCaptionDragMove() {
			if(CanCaptionDragMove())
				dragMoveHelper = new DragMoveHelper(Manager, DragMoveHitTest);
		}
		static bool DragMoveHitTest(BaseViewHitInfo hitInfo) {
			var headerInfo = hitInfo.HitElement as Docking2010.Views.WindowsUI.IContentContainerHeaderInfo;
			if(headerInfo != null)
				return headerInfo.DragMoveHitTest(hitInfo.HitPoint);
			var containerInfo = hitInfo.HitElement as Docking2010.Views.WindowsUI.IContentContainerInfo;
			if(containerInfo != null)
				return containerInfo.DragMoveHitTest(hitInfo.HitPoint);
			return false;
		}
		protected internal override bool IgnoreActiveFormOnActivation {
			get { return true; }
		}
		protected internal override bool ShouldRetryPatchActiveChildren(Size size) {
			return base.ShouldRetryPatchActiveChildren(size) || retryPatchActiveChildrenFlag;
		}
		internal bool retryPatchActiveChildrenFlag;
		protected internal override void PatchBeforeActivateChild(Control activatedChild, Point offset) { }
		protected internal override void PatchActiveChildren(Point offset) {
			retryPatchActiveChildrenFlag = false;
			IContentContainerInternal activeContainer = ActiveContentContainer as IContentContainerInternal;
			if(activeContainer != null)
				activeContainer.PatchChildren(Manager.Bounds, true);
			IContentContainerInternal activeFlyout = this.ActiveFlyoutContainer as IContentContainerInternal;
			if(activeFlyout != null)
				activeFlyout.PatchChildren(Manager.Bounds, true);
			if(documentForDelayedActivation != null && documentForDelayedActivation.IsControlLoaded) {
				var activeChild = Manager.GetActiveChild();
				var child = Manager.GetChild(documentForDelayedActivation);
				if(!object.ReferenceEquals(activeChild, child))
					Manager.Activate(child);
				this.documentForDelayedActivation = null;
			}
			DestroyNavigationContext();
		}
		#region Events
		static readonly object tileAdded = new object();
		static readonly object tileRemoved = new object();
		static readonly object tileClick = new object();
		static readonly object tilePress = new object();
		static readonly object tileCheckedChanged = new object();
		static readonly object queryStartupContentContainer = new object();
		static readonly object contentContainerAdded = new object();
		static readonly object contentContainerRemoved = new object();
		static readonly object contentContainerActivated = new object();
		static readonly object contentContainerDeactivated = new object();
		static readonly object contentContainerActionCustomization = new object();
		static readonly object contentContainerHeaderClick = new object();
		static readonly object navigationBarsShowing = new object();
		static readonly object navigationBarsShown = new object();
		static readonly object navigationBarsHidden = new object();
		static readonly object navigationBarsButtonClick = new object();
		static readonly object backButtonClick = new object();
		static readonly object queryDocumentActions = new object();
		static readonly object navigatedTo = new object();
		static readonly object navigatedFrom = new object();
		static readonly object flyoutShowing = new object();
		static readonly object flyoutShown = new object();
		static readonly object flyoutHiding = new object();
		static readonly object flyoutHidden = new object();
		static readonly object searchPanelShowing = new object();
		static readonly object searchPanelShown = new object();
		static readonly object searchPanelHidden = new object();
		static readonly object queryFlyoutActionsControlEvent = new object();
		static readonly object hierarchyChanged = new object();
		static readonly object customDrawBackButton = new object();
		static readonly object customizeSearchItems = new object();
		static readonly object searchItemClickCore = new object();
		[Category(DevExpress.XtraEditors.CategoryName.Layout)]
		public event TileEventHandler TileAdded {
			add { Events.AddHandler(tileAdded, value); }
			remove { Events.RemoveHandler(tileAdded, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.Layout)]
		public event TileEventHandler TileRemoved {
			add { Events.AddHandler(tileRemoved, value); }
			remove { Events.RemoveHandler(tileRemoved, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public event TileClickEventHandler TileClick {
			add { Events.AddHandler(tileClick, value); }
			remove { Events.RemoveHandler(tileClick, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public event TileEventHandler TilePress {
			add { Events.AddHandler(tilePress, value); }
			remove { Events.RemoveHandler(tilePress, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public event TileEventHandler TileCheckedChanged {
			add { Events.AddHandler(tileCheckedChanged, value); }
			remove { Events.RemoveHandler(tileCheckedChanged, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.Layout)]
		public event QueryContentContainerEventHandler QueryStartupContentContainer {
			add { Events.AddHandler(queryStartupContentContainer, value); }
			remove { Events.RemoveHandler(queryStartupContentContainer, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.Layout)]
		public event ContentContainerEventHandler ContentContainerAdded {
			add { Events.AddHandler(contentContainerAdded, value); }
			remove { Events.RemoveHandler(contentContainerAdded, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.Layout)]
		public event ContentContainerEventHandler ContentContainerRemoved {
			add { Events.AddHandler(contentContainerRemoved, value); }
			remove { Events.RemoveHandler(contentContainerRemoved, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.Layout)]
		public event EventHandler HierarchyChanged {
			add { Events.AddHandler(hierarchyChanged, value); }
			remove { Events.RemoveHandler(hierarchyChanged, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.Events)]
		public event ContentContainerEventHandler ContentContainerActivated {
			add { Events.AddHandler(contentContainerActivated, value); }
			remove { Events.RemoveHandler(contentContainerActivated, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.Events)]
		public event ContentContainerEventHandler ContentContainerDeactivated {
			add { Events.AddHandler(contentContainerDeactivated, value); }
			remove { Events.RemoveHandler(contentContainerDeactivated, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public event NavigationBarsCancelEventHandler NavigationBarsShowing {
			add { Events.AddHandler(navigationBarsShowing, value); }
			remove { Events.RemoveHandler(navigationBarsShowing, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public event EventHandler NavigationBarsShown {
			add { Events.AddHandler(navigationBarsShown, value); }
			remove { Events.RemoveHandler(navigationBarsShown, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public event EventHandler NavigationBarsHidden {
			add { Events.AddHandler(navigationBarsHidden, value); }
			remove { Events.RemoveHandler(navigationBarsHidden, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public event NavigationBarsButtonClickEventHandler NavigationBarsButtonClick {
			add { Events.AddHandler(navigationBarsButtonClick, value); }
			remove { Events.RemoveHandler(navigationBarsButtonClick, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public event BackButtonClickEventHandler BackButtonClick {
			add { Events.AddHandler(backButtonClick, value); }
			remove { Events.RemoveHandler(backButtonClick, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.Appearance)]
		public event ContentContainerActionCustomizationEventHandler ContentContainerActionCustomization {
			add { this.Events.AddHandler(contentContainerActionCustomization, value); }
			remove { this.Events.RemoveHandler(contentContainerActionCustomization, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.Layout)]
		public event DocumentHeaderClickEventHandler ContentContainerHeaderClick {
			add { this.Events.AddHandler(contentContainerHeaderClick, value); }
			remove { this.Events.RemoveHandler(contentContainerHeaderClick, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.Layout)]
		public event QueryDocumentActionsEventHandler QueryDocumentActions {
			add { this.Events.AddHandler(queryDocumentActions, value); }
			remove { this.Events.RemoveHandler(queryDocumentActions, value); }
		}
		[Category(DevExpress.XtraEditors.CategoryName.Appearance)]
		public event CustomDrawBackButtonEventHandler CustomDrawBackButton {
			add { this.Events.AddHandler(customDrawBackButton, value); }
			remove { this.Events.RemoveHandler(customDrawBackButton, value); }
		}
		[Category("Navigation")]
		public event NavigationEventHandler NavigatedTo {
			add { Events.AddHandler(navigatedTo, value); }
			remove { Events.RemoveHandler(navigatedTo, value); }
		}
		[Category("Navigation")]
		public event NavigationEventHandler NavigatedFrom {
			add { Events.AddHandler(navigatedFrom, value); }
			remove { Events.RemoveHandler(navigatedFrom, value); }
		}
		[Category("Behavior")]
		public event SearchPanelCancelEventHandler SearchPanelShowing {
			add { Events.AddHandler(searchPanelShowing, value); }
			remove { base.Events.RemoveHandler(searchPanelShowing, value); }
		}
		[Category("Behavior")]
		public event SearchPanelEventHandler SearchPanelShown {
			add { base.Events.AddHandler(searchPanelShown, value); }
			remove { base.Events.RemoveHandler(searchPanelShown, value); }
		}
		[Category("Behavior")]
		public event SearchPanelEventHandler SearchPanelHidden {
			add { base.Events.AddHandler(searchPanelHidden, value); }
			remove { base.Events.RemoveHandler(searchPanelHidden, value); }
		}
		[Category("Behavior")]
		public event FlyoutCancelEventHandler FlyoutShowing {
			add { Events.AddHandler(flyoutShowing, value); }
			remove { base.Events.RemoveHandler(flyoutShowing, value); }
		}
		[Category("Behavior")]
		public event EventHandler FlyoutShown {
			add { base.Events.AddHandler(flyoutShown, value); }
			remove { base.Events.RemoveHandler(flyoutShown, value); }
		}
		[Category("Behavior")]
		public event FlyoutCancelEventHandler FlyoutHiding {
			add { base.Events.AddHandler(flyoutHiding, value); }
			remove { base.Events.RemoveHandler(flyoutHiding, value); }
		}
		[Category("Behavior")]
		public event FlyoutResultEventHandler FlyoutHidden {
			add { base.Events.AddHandler(flyoutHidden, value); }
			remove { base.Events.RemoveHandler(flyoutHidden, value); }
		}
		[Category("Behavior")]
		public event QueryFlyoutActionsControlEventHandler QueryFlyoutActionsControl {
			add { Events.AddHandler(queryFlyoutActionsControlEvent, value); }
			remove { Events.RemoveHandler(queryFlyoutActionsControlEvent, value); }
		}
		[Category("Behavior")]
		public event CustomizeSearchItemsEventHandler CustomizeSearchItems {
			add { Events.AddHandler(customizeSearchItems, value); }
			remove { Events.RemoveHandler(customizeSearchItems, value); }
		}
		[Category("Behavior")]
		public event SearchItemClickEventHandler SearchItemClick {
			add { Events.AddHandler(searchItemClickCore, value); }
			remove { Events.RemoveHandler(searchItemClickCore, value); }
		}
		protected void RaiseTileAdded(BaseTile tile) {
			TileEventHandler handler = (TileEventHandler)Events[tileAdded];
			if(handler != null)
				handler(this, new TileEventArgs(tile));
		}
		protected void RaiseTileRemoved(BaseTile tile) {
			TileEventHandler handler = (TileEventHandler)Events[tileRemoved];
			if(handler != null)
				handler(this, new TileEventArgs(tile));
		}
		protected internal bool RaiseTileClick(BaseTile tile) {
			TileClickEventHandler handler = (TileClickEventHandler)Events[tileClick];
			TileClickEventArgs e = new TileClickEventArgs(tile);
			if(handler != null)
				handler(this, e);
			return !e.Handled;
		}
		protected internal void RaiseTilePress(BaseTile tile) {
			TileEventHandler handler = (TileEventHandler)Events[tilePress];
			TileEventArgs e = new TileEventArgs(tile);
			if(handler != null)
				handler(this, e);
		}
		protected internal void RaiseTileCheckedChanged(BaseTile tile) {
			TileEventHandler handler = (TileEventHandler)Events[tileCheckedChanged];
			if(handler != null)
				handler(this, new TileEventArgs(tile));
		}
		protected IContentContainer RaiseQueryStartupContentContainer() {
			QueryContentContainerEventHandler handler = (QueryContentContainerEventHandler)Events[queryStartupContentContainer];
			QueryContentContainerEventArgs ea = new QueryContentContainerEventArgs();
			if(handler != null)
				handler(this, ea);
			return ea.ContentContainer;
		}
		protected void RaiseContentContainerAdded(IContentContainer contentContainer) {
			ContentContainerEventHandler handler = (ContentContainerEventHandler)Events[contentContainerAdded];
			if(handler != null)
				handler(this, new ContentContainerEventArgs(contentContainer));
		}
		protected void RaiseContentContainerRemoved(IContentContainer contentContainer) {
			ContentContainerEventHandler handler = (ContentContainerEventHandler)Events[contentContainerRemoved];
			if(handler != null)
				handler(this, new ContentContainerEventArgs(contentContainer));
		}
		protected internal void RaiseHierarchyChanged() {
			EventHandler handler = (EventHandler)Events[hierarchyChanged];
			if(handler != null)
				handler(this, EventArgs.Empty);
		}
		protected void RaiseContentContainerActivated(IContentContainer contentContainer) {
			ContentContainerEventHandler handler = (ContentContainerEventHandler)Events[contentContainerActivated];
			if(handler != null)
				handler(this, new ContentContainerEventArgs(contentContainer));
		}
		protected void RaiseContentContainerDeactivated(IContentContainer contentContainer) {
			ContentContainerEventHandler handler = (ContentContainerEventHandler)Events[contentContainerDeactivated];
			if(handler != null)
				handler(this, new ContentContainerEventArgs(contentContainer));
		}
		protected internal void RaiseContentContainerActionCustomization(ContentContainerActionCustomizationEventArgs args) {
			ContentContainerActionCustomizationEventHandler handler = (ContentContainerActionCustomizationEventHandler)Events[contentContainerActionCustomization];
			if(handler != null)
				handler(this, args);
		}
		protected internal void RaiseContentContainerHeaderClick(DocumentGroup group, DocumentHeaderClickEventArgs args) {
			DocumentHeaderClickEventHandler handler = (DocumentHeaderClickEventHandler)Events[contentContainerHeaderClick];
			if(handler != null)
				handler(group, args);
		}
		protected internal bool RaiseNavigationBarsShowing(Control source, EventArgs ea, bool cancel) {
			NavigationBarsCancelEventHandler handler = (NavigationBarsCancelEventHandler)Events[navigationBarsShowing];
			var e = new NavigationBarsCancelEventArgs(ActiveContentContainer, source, ea, cancel);
			if(handler != null)
				handler(this, e);
			return !e.Cancel;
		}
		protected internal bool RaiseNavigationBarsButtonClick(DevExpress.XtraEditors.ButtonPanel.BaseButton button) {
			if(button == null) return true;
			NavigationBarsButtonClickEventArgs e = new NavigationBarsButtonClickEventArgs(button);
			NavigationBarsButtonClickEventHandler handler = (NavigationBarsButtonClickEventHandler)Events[navigationBarsButtonClick];
			if(handler != null)
				handler(this, e);
			return !e.Handled;
		}
		protected internal bool RaiseBackButtonClick() {
			BackButtonClickEventArgs e = new BackButtonClickEventArgs();
			BackButtonClickEventHandler handler = (BackButtonClickEventHandler)Events[backButtonClick];
			if(handler != null)
				handler(this, e);
			return !e.Handled;
		}
		protected internal void RaiseNavigationBarsShown() {
			EventHandler handler = (EventHandler)Events[navigationBarsShown];
			if(handler != null)
				handler(this, EventArgs.Empty);
		}
		protected internal void RaiseNavigationBarsHidden() {
			EventHandler handler = (EventHandler)Events[navigationBarsHidden];
			if(handler != null)
				handler(this, EventArgs.Empty);
		}
		protected internal void RaiseQueryDocumentActions(QueryDocumentActionsEventArgs e) {
			QueryDocumentActionsEventHandler handler = (QueryDocumentActionsEventHandler)Events[queryDocumentActions];
			if(handler != null)
				handler(this, e);
		}
		protected internal void RaiseNavigatedTo(NavigationEventArgs e) {
			NavigationEventHandler handler = (NavigationEventHandler)Events[navigatedTo];
			if(handler != null)
				handler(this, e);
		}
		protected internal void RaiseNavigatedFrom(NavigationEventArgs e) {
			NavigationEventHandler handler = (NavigationEventHandler)Events[navigatedFrom];
			if(handler != null)
				handler(this, e);
		}
		protected internal bool RaiseFlyoutShowing() {
			FlyoutCancelEventHandler handler = (FlyoutCancelEventHandler)Events[flyoutShowing];
			FlyoutCancelEventArgs e = new FlyoutCancelEventArgs();
			if(handler != null) {
				handler(this, e);
			}
			return !e.Cancel;
		}
		protected internal void RaiseFlyoutShown() {
			EventHandler handler = (EventHandler)Events[flyoutShown];
			if(handler != null) {
				handler(this, EventArgs.Empty);
			}
		}
		protected internal bool RaiseFlyoutHiding() {
			FlyoutCancelEventHandler handler = (FlyoutCancelEventHandler)Events[flyoutHiding];
			FlyoutCancelEventArgs e = new FlyoutResultCancelEventArgs(flyoutBootStrapperCore.GetDialogResult());
			if(handler != null) {
				handler(this, e);
			}
			return !e.Cancel;
		}
		protected internal void RaiseFlyoutHidden(DialogResult result) {
			FlyoutResultEventHandler handler = (FlyoutResultEventHandler)Events[flyoutHidden];
			FlyoutResultEventArgs e = new FlyoutResultEventArgs(result);
			if(handler != null) {
				handler(this, e);
			}
		}
		protected internal void RaiseQueryFlyoutActionsControl(QueryFlyoutActionsControlArgs e) {
			QueryFlyoutActionsControlEventHandler handler = (QueryFlyoutActionsControlEventHandler)Events[queryFlyoutActionsControlEvent];
			if(handler != null)
				handler(this, e);
		}
		protected internal bool RaiseSearchPanelShowing(Control source, bool cancel) {
			SearchPanelCancelEventArgs e = new SearchPanelCancelEventArgs(source, cancel);
			SearchPanelCancelEventHandler handler = (SearchPanelCancelEventHandler)Events[searchPanelShowing];
			if(handler != null)
				handler(this, e);
			return !e.Cancel;
		}
		protected internal void RaiseSearchPanelShown() {
			SearchPanelEventHandler handler = (SearchPanelEventHandler)Events[searchPanelShown];
			SearchPanelEventArgs e = new SearchPanelEventArgs();
			if(handler != null)
				handler(this, e);
		}
		protected internal void RaiseSearchPanelHidden() {
			SearchPanelEventHandler handler = (SearchPanelEventHandler)Events[searchPanelHidden];
			SearchPanelEventArgs e = new SearchPanelEventArgs();
			if(handler != null)
				handler(this, e);
		}
		protected internal bool RaiseCustomDrawBackButton(DevExpress.Utils.Drawing.GraphicsCache cache, XtraEditors.ButtonPanel.BaseButtonInfo info, XtraEditors.ButtonPanel.BaseButtonPainter painter, DevExpress.Utils.AppearanceObject appearance) {
			CustomDrawBackButtonEventHandler handler = (CustomDrawBackButtonEventHandler)Events[customDrawBackButton];
			CustomDrawBackButtonEventArgs ea = new CustomDrawBackButtonEventArgs(cache, info, painter, appearance);
			if(handler != null)
				handler(this, ea);
			return ea.Handled;
		}
		protected void RaiseCustomizeSearchItems(CustomizeSearchItemsEventArgs e) {
			CustomizeSearchItemsEventHandler handler = (CustomizeSearchItemsEventHandler)Events[customizeSearchItems];
			if(handler == null) return;
			handler(this, e);
		}
		protected void RaiseSearchItemClick(SearchItemClickEventArgs e) {
			SearchItemClickEventHandler handler = Events[searchItemClickCore] as SearchItemClickEventHandler;
			if(handler != null)
				handler(this, e);
		}
		#endregion
		protected internal override void RegisterListeners(DragEngine.BaseUIView uiView) {
			uiView.RegisterUIServiceListener(new Dragging.DocumentManagerUIViewRegularDragListener());
			uiView.RegisterUIServiceListener(new Dragging.WindowsUI.DocumentManagerUIViewUIInteractionListener());
			uiView.RegisterUIServiceListener(new Dragging.WindowsUI.DocumentManagerUIViewResizingListener());
			uiView.RegisterUIServiceListener(new Dragging.WindowsUI.DocumentManagerUIViewScrollingListener());
		}
		protected override void OnShowingDockGuidesCore(DockGuidesConfiguration configuration, BaseDocument document, BaseViewHitInfo hitInfo) {
			base.OnShowingDockGuidesCore(configuration, document, hitInfo);
			if(ActiveContentContainer == null || hitInfo.IsEmpty) return;
		}
		#region Elements
		protected internal virtual Tile CreateTile(Document document) {
			return new Tile(TileProperties, document);
		}
		protected internal virtual TileGroup CreateTileGroup(BaseTile[] tiles) {
			return new TileGroup(TileProperties, tiles);
		}
		protected internal virtual Page CreatePage() {
			return new Page(PageProperties);
		}
		protected internal virtual Flyout CreateFlyout() {
			return new Flyout(FlyoutProperties);
		}
		protected internal virtual SplitGroup CreateSplitGroup() {
			return new SplitGroup(SplitGroupProperties);
		}
		protected internal virtual SlideGroup CreateSlideGroup() {
			return new SlideGroup(SlideGroupProperties);
		}
		protected internal virtual PageGroup CreatePageGroup() {
			return new PageGroup(PageGroupProperties);
		}
		protected internal virtual TabbedGroup CreateTabbedGroup() {
			return new TabbedGroup(TabbedGroupProperties);
		}
		protected internal virtual TileContainer CreateTileContainer() {
			return new TileContainer(TileContainerProperties);
		}
		#endregion Elements
		#region NavigationContext
		INavigationContext navigationContext;
		internal WeakReference navigationTagReference;
		protected internal void SetNavigationTag(object tag) {
			navigationTagReference = new WeakReference(tag);
		}
		protected internal void StartNavigation(IContentContainer source, IContentContainer target) {
			Ref.Dispose(ref navigationContext);
			if(target != null) {
				object tag = navigationTagReference != null ? navigationTagReference.Target : null;
				navigationContext = new NavigationContext(this, source, target) { Tag = tag };
			}
		}
		protected internal void StartItemsNavigation(IDocumentSelector selector) {
			if(navigationContext == null)
				navigationContext = new NavigationContext(this, selector, selector);
			NotifyNavigatedFrom(selector.SelectedDocument);
		}
		protected internal void DestroyNavigationContext() {
			Ref.Dispose(ref navigationContext);
			this.navigationTagReference = null;
		}
		internal class NavigationContext : INavigationContext {
			static IDictionary<WindowsUIView, INavigationContext> contexts =
			   new Dictionary<WindowsUIView, INavigationContext>();
			public NavigationContext(WindowsUIView view, IContentContainer source, IContentContainer target) {
				contexts.Add(view, this);
				this.View = view;
				this.Source = source;
				this.Target = target;
			}
			public WindowsUIView View { get; private set; }
			public IContentContainer Source { get; private set; }
			public IContentContainer Target { get; private set; }
			public object Tag { get; set; }
			public static bool TryGetValue(WindowsUIView view, out INavigationContext context) {
				return contexts.TryGetValue(view, out context);
			}
			public void Dispose() {
				contexts.Remove(View);
				Parameter = null;
				View = null;
				Source = null;
				Target = null;
				Tag = null;
			}
			public object Parameter { get; set; }
		}
		#endregion NavigationContext
		#region ISearchProvider Members
		ISearchContainer ISearchProvider.SearchActiveContainer { get { return ActiveContentContainer as ISearchContainer; } }
		bool CheckIsChild(IContentContainer container) {
			if(container == null) return true;
			ISearchContainer activeContainer = ActiveContentContainer as ISearchContainer;
			if(activeContainer == null || activeContainer.SearchChildList == null) return false;
			if(container == ActiveContentContainer) return true;
			foreach(IContentContainer child in activeContainer.SearchChildList)
				if(child == container) return true;
			return false;
		}
		IEnumerable<ISearchContainer> ISearchProvider.SearchOtherList {
			get {
				if(ContentContainers == null || ContentContainers.Count == 0) return null;
				List<ISearchContainer> searchContainers = new List<ISearchContainer>();
				foreach(IContentContainer container in ContentContainers)
					if(!CheckIsChild(container))
						searchContainers.Add(container as ISearchContainer);
				return searchContainers;
			}
		}
		void ISearchProvider.Activate(ISearchObjectContext context) {
			SearchItemClickEventArgs args = new SearchItemClickEventArgs(context);
			RaiseSearchItemClick(args);
			if(args.Cancel) return;
			if(context.SourceContainer != null)
				Controller.Activate(context.SourceContainer);
			BaseComponent component = context.Source;
			if(component == null) return;
			if(component is BaseTile)
				Controller.Activate((BaseTile)component);
			if(component is BaseDocument)
				Controller.Activate((BaseDocument)component);
		}
		void ISearchProvider.CustomizeSearchItems(CustomizeSearchItemsEventArgs args) {
			if(args == null) return;
			RaiseCustomizeSearchItems(args);
		}
		bool ISearchProvider.CanShowSearchPanel { get { return ActiveContentContainer != null && !(ActiveContentContainer is Flyout) && ActiveFlyoutContainer == null; } }
		#endregion
	}
	[Obsolete(WindowsUIViewObsoleteText.SRObsoleteMetroUIView), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class MetroUIView : WindowsUIView {
		public MetroUIView() { }
		public MetroUIView(IContainer container)
			: base(container) {
		}
	}
	internal class WindowsUIViewObsoleteText {
		internal const string SRObsoleteMetroUIView = "You should use the WindowsUIView class";
	}
}
