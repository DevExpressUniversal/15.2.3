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
using System.Reflection;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Mdi;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars.Docking2010.Base;
using DevExpress.XtraBars.Docking2010.Customization;
using DevExpress.XtraBars.Docking2010.DragEngine;
using DevExpress.XtraBars.Docking2010.Dragging;
using DevExpress.XtraBars.Docking2010.Views;
namespace DevExpress.XtraBars.Docking2010 {
	enum ContainerType { MdiParent, ClientControl, ContainerControl }
	[Designer("DevExpress.XtraBars.Design.DocumentManagerDesigner, " + AssemblyInfo.SRAssemblyBarsDesign, typeof(IDesigner)),
	DesignerCategory("Component"),
 DXToolboxItem(true), ToolboxBitmap(typeof(ToolboxIcons.ToolboxIconsRootNS), "Docking2010.DocumentManager"),
	Description("Provides centralized control over the multiple document interface."),
	SerializationOrder(Order = 2),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabNavigation)]
	public class DocumentManager : BaseComponent, IUIElement, IMdiClientSubclasserOwner, IViewRegistrator, IMdiClientListener,
		IDocumentsHostOwner,
		IClientControlListener,
		IBarAndDockingControllerClient, IToolTipControlClient, IProcessRunningListener, IBarMouseActivateClient, ILogicalOwner {
		public static void About() {
			DevExpress.Utils.About.AboutHelper.Show(DevExpress.Utils.About.ProductKind.DXperienceWin, new DevExpress.Utils.About.ProductStringInfoWin(DevExpress.Utils.About.ProductInfoHelper.WinXtraBars));
		}
		public DocumentManager()
			: base(null) {
		}
		public DocumentManager(IContainer container)
			: base(container) {
		}
		DocumentSelectorBootStrapper documentSelectorBootStrapperCore;
		protected override void OnCreate() {
			this.registrators = new Dictionary<ViewType, BaseRegistrator.Create>();
			this.viewCollectionCore = CreateViewCollection();
			RegisterViews();
			GetBarAndDockingController().AddClient(this);
			activationInfoRef = new SharedRef<IActivationInfo>(CreateActivationInfo());
			thumbnailManagerRef = new SharedRef<IThumbnailManager>(CreateThumbnailManager());
			HitTestEnabled = true;
			ComponentLocator.RegisterFindRoutine(FromCurrentControAndForm);
		}
		protected override void LockComponentBeforeDisposing() {
			HitTestEnabled = false;
			RemoveClientFromToolTipController();
			this.toolTipControllerCore = null;
			GetBarAndDockingController().RemoveClient(this);
			this.barAndDockingControllerCore = null;
			if(strategyCore != null)
				Strategy.UnSubscribe();
			if(ActivationInfo != null) {
				ActivationInfo.Detach(this);
				ActivationInfo.Detach(View);
				ActivationInfo.Detach(DockManager);
			}
		}
		protected override void OnDispose() {
			Depopulate();
			DestroyDocumentSelector();
			ReleaseDockManager();
			Ref.Dispose(ref notificationSourceCore);
			Ref.Dispose(ref adornerCore);
			Ref.Dispose(ref snapAdornerCore);
			Ref.Dispose(ref uiViewCore);
			Ref.Dispose(ref uiViewAdapterRef);
			Ref.Dispose(ref activationInfoRef);
			Ref.Dispose(ref thumbnailManagerRef);
			Ref.Dispose(ref strategyCore);
			Ref.Dispose(ref viewCore);
			Ref.Dispose(ref viewCollectionCore);
			Ref.Dispose(ref documentsHostContextRef);
			registrators.Clear();
			containerControlCore = null;
			mdiParentCore = null;
			ResetClientControlCore();
			base.OnDispose();
		}
		IDXMenuManager menuManagerCore;
#if !SL
	[DevExpressXtraBarsLocalizedDescription("DocumentManagerMenuManager")]
#endif
		[Category("BarManager"), DefaultValue(null)]
		public IDXMenuManager MenuManager {
			get { return menuManagerCore; }
			set { menuManagerCore = value; }
		}
		BarAndDockingController barAndDockingControllerCore;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DocumentManagerBarAndDockingController"),
#endif
 Category("Appearance"), DefaultValue(null)]
		public virtual BarAndDockingController BarAndDockingController {
			get { return barAndDockingControllerCore; }
			set {
				if(BarAndDockingController == value) return;
				GetBarAndDockingController().RemoveClient(this);
				this.barAndDockingControllerCore = value;
				GetBarAndDockingController().AddClient(this);
				OnBarAndDockingControllerChanged(value);
			}
		}
		ToolTipController toolTipControllerCore;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DocumentManagerToolTipController"),
#endif
 Category("Appearance"), DefaultValue(null)]
		public virtual ToolTipController ToolTipController {
			get { return toolTipControllerCore; }
			set {
				if(ToolTipController == value) return;
				RemoveClientFromToolTipController();
				this.toolTipControllerCore = value;
				AddClientToToolTipController();
			}
		}
		void RemoveClientFromToolTipController() {
			Control client = (strategyCore != null) ? Strategy.GetOwnerControl() : null;
			if(client != null)
				GetToolTipController().RemoveClientControl(client);
		}
		void AddClientToToolTipController() {
			Control client = (strategyCore != null) ? Strategy.GetOwnerControl() : null;
			AddMdiClientToToolTipControllerCore(client);
		}
		void AddMdiClientToToolTipControllerCore(Control client) {
			if(client != null)
				GetToolTipController().AddClientControl(client, this);
		}
		#region IToolTipControlClient Members
		bool IToolTipControlClient.ShowToolTips {
			get { return ShowToolTips != DefaultBoolean.False && !DesignMode; }
		}
		ToolTipControlInfo IToolTipControlClient.GetObjectInfo(Point point) {
			return GetObjectInfo(point);
		}
		protected virtual ToolTipControlInfo GetObjectInfo(Point point) {
			if(IsDisposing || View == null) return null;
			BaseViewHitInfo hitInfo = CalcHitInfo(point);
			if(hitInfo != null && !hitInfo.IsEmpty)
				return View.GetToolTipControlInfo(hitInfo);
			return null;
		}
		DefaultBoolean showTooltipsCore = DefaultBoolean.Default;
#if !SL
	[DevExpressXtraBarsLocalizedDescription("DocumentManagerShowToolTips")]
#endif
		[DefaultValue(DefaultBoolean.Default), Category("Appearance")]
		public DefaultBoolean ShowToolTips {
			get { return showTooltipsCore; }
			set { showTooltipsCore = value; }
		}
		#endregion
		#region IBarAndDockingControllerClient Members
		void IBarAndDockingControllerClient.OnControllerChanged(BarAndDockingController controller) {
			OnBarAndDockingControllerChanged(controller);
		}
		void IBarAndDockingControllerClient.OnDisposed(BarAndDockingController controller) {
			barAndDockingControllerCore = null;
		}
		protected virtual void OnBarAndDockingControllerChanged(BarAndDockingController controller) {
			if(View != null)
				View.UpdateStyle();
			if(!IsUpdateLayoutRestricted(false)) {
				var hostWindow = DocumentsHostContext.GetForm(this) as FloatDocumentsHostWindow;
				if(hostWindow != null)
					hostWindow.UpdateStyle();
			}
			LayoutChanged();
		}
		#endregion IBarAndDockingControllerClient Members
		#region IBarMouseActivateClient Members
		void IBarMouseActivateClient.OnBarMouseActivate(BarManager barManager, ref Message msg) {
			if(IsUpdateLayoutRestricted(false)) return;
			if(View != null && View.ActiveFloatDocument != null)
				msg.Result = new IntPtr(3 );
			else {
				IDocumentsHostWindow hostWindow = DocumentsHostContext.GetDocumentsHostWindow(this);
				if(hostWindow != null && object.ReferenceEquals(hostWindow, Form.ActiveForm))
					msg.Result = new IntPtr(3 );
				if(DockManager != null) {
					var activeForm = Form.ActiveForm as Docking.FloatForm;
					if(DockManager.ActivePanel != null && !DockManager.ActivePanel.IsMdiDocument)
						activeForm = GetActiveFloatForm(DockManager.ActivePanel, activeForm);
					if(activeForm != null && activeForm.Visible)
						msg.Result = new IntPtr(3 );
				}
			}
		}
		Docking.FloatForm GetActiveFloatForm(Docking.DockPanel dockPanel, Docking.FloatForm form) {
			if(dockPanel == null) return form;
			return dockPanel.FloatForm ?? GetActiveFloatForm(dockPanel.ParentPanel, form);
		}
		#endregion
		object imagesCore;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DocumentManagerImages"),
#endif
 Category("Appearance")]
		[DefaultValue(null), TypeConverter(typeof(DevExpress.Utils.Design.ImageCollectionImagesConverter))]
		public virtual object Images {
			get { return imagesCore; }
			set {
				if(Images == value)
					return;
				imagesCore = value;
				LayoutChanged();
			}
		}
		DefaultBoolean rightToLeftLayoutCore = DefaultBoolean.Default;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DocumentManagerRightToLeftLayout"),
#endif
 Category("Layout")]
		[DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean RightToLeftLayout {
			get { return rightToLeftLayoutCore; }
			set {
				if(RightToLeftLayout == value) return;
				rightToLeftLayoutCore = value;
				LayoutChanged();
			} 
		}
		protected internal bool IsRightToLeftLayout() {
			if(RightToLeftLayout == DefaultBoolean.Default) {
				if(IsStrategyValid) {
					var control = Strategy.GetOwnerControl();
					return DevExpress.XtraEditors.WindowsFormsSettings.GetIsRightToLeft(control);
				}
			}
			return RightToLeftLayout == DefaultBoolean.True;
		}
		internal bool IsFormRightToLeftLayout() {
			return (MdiParent != null) && MdiParent.RightToLeftLayout;
		}
		protected internal UserLookAndFeel LookAndFeel {
			get { return GetBarAndDockingController().LookAndFeel; }
		}
		protected internal string PaintStyleName {
			get { return GetBarAndDockingController().GetPaintStyleName(); }
		}
		protected internal BarAndDockingController GetBarAndDockingController() {
			return barAndDockingControllerCore ?? BarAndDockingController.Default;
		}
		protected internal ToolTipController GetToolTipController() {
			return toolTipControllerCore ?? ToolTipController.DefaultController;
		}
		protected internal IDXMenuManager GetMenuManager() {
			return menuManagerCore ?? MenuManagerHelper.GetMenuManager(LookAndFeel, Strategy.Container);
		}
		protected internal BarManager GetBarManager() {
			BarManager manager = MenuManager as BarManager;
			if(manager == null) {
				Ribbon.RibbonControl ribbon = MenuManager as Ribbon.RibbonControl;
				manager = ribbon != null ? ribbon.Manager : null;
			}
			return manager;
		}
		Control clientControlCore;
		[DefaultValue(null), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("DocumentManagerClientControl"),
#endif
 Category("Behavior")]
		public Control ClientControl {
			[System.Diagnostics.DebuggerStepThrough]
			get { return clientControlCore; }
			set {
				if(value is Form || value is Docking.DockPanel) return;
				if(clientControlCore == value || !CanChangeStrategy()) return;
				if(value == null & !ShouldResetParentContainer(ContainerType.ClientControl)) {
					throw new NullParentContainerException();
				}
				if(strategyCore != null)
					Strategy.UnSubscribe();
				using(LockViewPainting()) {
					if(!IsInitializing) {
						if(!IsDisposing)
							Depopulate();
						Destroy(MdiParent);
						Destroy(ContainerControl);
						DestroyClientControl();
					}
					Ref.Dispose(ref strategyCore);
					mdiParentCore = null;
					containerControlCore = null;
					ResetClientControlCore();
					if(value != null)
						clientControls.Add(value, this);
					clientControlCore = value;
					OnEndChangeStrategy();
					if(!IsInitializing) {
						Initialize(null);
						EnsureMainView();
						if(View.CanUseDocumentSelector())
							CreateDocumentSelector();
					}
				}
				LayoutChanged();
			}
		}
		ContainerControl containerControlCore;
		[DefaultValue(null), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("DocumentManagerContainerControl"),
#endif
 Category("Behavior")]
		public ContainerControl ContainerControl {
			[System.Diagnostics.DebuggerStepThrough]
			get { return containerControlCore; }
			set {
				if(Docking.ProhibitUsingAsDockingContainerAttribute.IsDefined(value)) return;
				if(containerControlCore == value || !CanChangeStrategy()) return;
				if(value == null & !ShouldResetParentContainer(ContainerType.ContainerControl)) {
					throw new NullParentContainerException();
				}
				if(strategyCore != null)
					Strategy.UnSubscribe();
				using(LockViewPainting()) {
					if(!IsInitializing) {
						if(!IsDisposing)
							Depopulate();
						Destroy(ContainerControl);
						Destroy(MdiParent);
						DestroyClientControl();
					}
					Ref.Dispose(ref strategyCore);
					mdiParentCore = null;
					ResetClientControlCore();
					containerControlCore = value;
					OnEndChangeStrategy();
					Strategy.Subscribe(ContainerControl);
					if(!IsInitializing) {
						if(ContainerControl != null) {
							Initialize(ContainerControl);
							EnsureMainView();
							IDocumentsHost host = DocumentsHost.GetDocumentsHost(ContainerControl);
							if(host != null)
								Populate(host.Containers);
							if(View.CanUseDocumentSelector())
								CreateDocumentSelector();
						}
					}
				}
				LayoutChanged();
			}
		}
		Form mdiParentCore;
		[DefaultValue(null), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("DocumentManagerMdiParent"),
#endif
 Category("Behavior")]
		public Form MdiParent {
			[System.Diagnostics.DebuggerStepThrough]
			get { return mdiParentCore; }
			set {
				if(mdiParentCore == value || !CanChangeStrategy()) return;
				if(value == null & !ShouldResetParentContainer(ContainerType.MdiParent)) {
					throw new NullParentContainerException();
				}
				if(strategyCore != null)
					Strategy.UnSubscribe();
				using(LockViewPainting()) {
					if(!IsInitializing) {
						if(!IsDisposing)
							Depopulate();
						Destroy(ContainerControl);
						Destroy(MdiParent);
						DestroyClientControl();
					}
					Ref.Dispose(ref strategyCore);
					containerControlCore = null;
					ResetClientControlCore();
					mdiParentCore = value;
					OnEndChangeStrategy();
					Strategy.Subscribe(MdiParent);
					if(!IsInitializing) {
						if(MdiParent != null) {
							Initialize(MdiParent);
							EnsureMainView();
							Populate(MdiParent.MdiChildren);
							if(View.CanUseDocumentSelector())
								CreateDocumentSelector();
						}
					}
				}
				LayoutChanged();
			}
		}
		protected internal IDisposable LockViewPainting() {
			return (View != null) ? View.LockPainting() : null;
		}
		bool ShouldResetParentContainer(ContainerType containerType) {
			switch(containerType) {
				case ContainerType.MdiParent:
					return ContainerControl != null || ClientControl != null;
				case ContainerType.ContainerControl:
					return MdiParent != null || ClientControl != null;
				case ContainerType.ClientControl:
					return ContainerControl != null || MdiParent != null;
			}
			return true;
		}
		void ResetClientControlCore() {
			if(ClientControl != null)
				clientControls.Remove(ClientControl);
			clientControlCore = null;
		}
		BaseView viewCore;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DocumentManagerView"),
#endif
 Category("Layout")]
		public BaseView View {
			[System.Diagnostics.DebuggerStepThrough]
			get { return viewCore; }
			set {
				if(viewCore == value) return;
				using(new BaseView.ChangeContext(View, value)) {
					if(View != null) {
						if(UIView != null)
							UIView.ResetListeners();
						View.Disposed -= OnViewDisposed;
						View.Unload();
						if(ActivationInfo != null)
							ActivationInfo.Detach(View);
					}
					OnViewChanging();
					viewCore = value;
					if(viewCore != null) {
						View.Disposed += OnViewDisposed;
						ViewCollection.Add(View);
						ActivationInfo.Attach(View);
						View.Load();
						if(UIView != null)
							UIView.RegisterListeners(View);
					}
					OnViewChanged();
				}
			}
		}
		BaseViewCollection viewCollectionCore;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DocumentManagerViewCollection"),
#endif
 Category("Layout")]
		public BaseViewCollection ViewCollection {
			get { return viewCollectionCore; }
		}
		protected virtual BaseViewCollection CreateViewCollection() {
			return new BaseViewCollection(this);
		}
		protected virtual IActivationInfo CreateActivationInfo() {
			return new ActivationInfo(this);
		}
		#region DragEngine
		SharedRef<IUIViewAdapter> uiViewAdapterRef;
		IUIView uiViewCore;
		protected internal IUIViewAdapter UIViewAdapter {
			get { return uiViewAdapterRef != null ? uiViewAdapterRef.Target : null; }
		}
		protected internal DocumentManagerUIView UIView {
			get { return uiViewCore as DocumentManagerUIView; }
		}
		protected virtual IUIViewAdapter CreateViewAdapter() {
			return new UIViewAdapter(DocumentManagerUIView.DefaultComparer);
		}
		protected virtual IUIView CreateDocumentManagerUIView() {
			return new DocumentManagerUIView(this);
		}
		protected virtual IUIView CreateFloatFormUIView(BaseDocument document) {
			return new FloatFormUIView(document);
		}
		protected virtual IUIView CreateFloatPanelUIView(BaseDocument document) {
			return new FloatPanelUIView(document);
		}
		bool CanChangeStrategy() {
			if(DesignMode && !IsInitializing && !IsUpdateLocked) {
				if(mdiParentCore == null && containerControlCore == null && clientControlCore == null) return true;
				if(View != null && View.IsEmpty) return true;
				DialogResult result = MessageBox.Show("Changing this property will destroy the current layout. Do you want to proceed?", "Change Strategy", MessageBoxButtons.YesNo);
				return result != DialogResult.No;
			}
			return true;
		}
		void OnEndChangeStrategy() {
			if(DesignMode && !IsInitializing && View != null) {
				View.Controller.CloseAll();
			}
		}
		protected internal void RegisterFloatPanel(BaseDocument document) {
			IUIView panelView = CreateFloatPanelUIView(document);
			UIViewAdapter.Views.Add(panelView);
		}
		protected internal void UnregisterFloatPanel(Control control) {
			if(UIViewAdapter == null) return;
			IUIView panelView = UIViewAdapter.GetView(control);
			UIViewAdapter.Views.Remove(panelView);
			Ref.Dispose(ref panelView);
		}
		protected internal void RegisterFloatDocument(BaseDocument document) {
			if(UIViewAdapter != null || IsOwnerControlHandleCreated)
				RegisterFloatDocumentCore(document);
			else QueueFloatDocumentRegistration(document);
		}
		protected internal void UnregisterFloatDocument(Control control) {
			if(UIViewAdapter == null) return;
			IUIView floatView = UIViewAdapter.GetView(control);
			UIViewAdapter.Views.Remove(floatView);
			Ref.Dispose(ref floatView);
		}
		Queue<BaseDocument> registrationQueue;
		void QueueFloatDocumentRegistration(BaseDocument document) {
			if(registrationQueue == null)
				registrationQueue = new Queue<BaseDocument>();
			registrationQueue.Enqueue(document);
		}
		void PerformFloatDocumentsRegistration() {
			if(registrationQueue == null) return;
			BaseDocument[] documents = registrationQueue.ToArray();
			Array.ForEach(documents, RegisterFloatDocumentCore);
			registrationQueue.Clear();
			registrationQueue = null;
		}
		void RegisterFloatDocumentCore(BaseDocument document) {
			IUIView floatView = CreateFloatFormUIView(document);
			UIViewAdapter.Views.Add(floatView);
		}
		int floatingCounter = 0;
		protected internal void BeginFloating() {
			floatingCounter++;
		}
		protected internal void EndFloating() {
			floatingCounter--;
		}
		protected internal bool IsFloating {
			get { return floatingCounter > 0; }
		}
		int dockingCounter = 0;
		protected internal void BeginDocking() {
			dockingCounter++;
		}
		protected internal void EndDocking() {
			dockingCounter--;
		}
		protected internal bool IsDocking {
			get { return dockingCounter > 0; }
		}
		#region IUIElement
		IUIElement IUIElement.Scope { get { return null; } }
		UIChildren uiChildren = new UIChildren();
		UIChildren IUIElement.Children {
			get { return uiChildren; }
		}
		#endregion IUIElement
		#endregion DragEngine
		#region IClientControlListener
		void IClientControlListener.HandleCreated() {
			OnHandleCreatedCore();
		}
		void IClientControlListener.HandleDestroyed() {
			OnHandleDestroyedCore();
		}
		void IClientControlListener.VisibleChanged(bool visible) {
			if(!DesignMode) 
				OnVisibleChanged(visible);
		}
		void IClientControlListener.Resize(Rectangle bounds) {
			if(!CanUpdateLayout()) return;
			CalculateNCCore(bounds);
		}
		void IClientControlListener.OnDisposed() {
			if(IsDisposing) return;
			using(View.FloatPanels.LockFloatPanelRegistration()) {
				DepopulateFloatPanels();
			}
			HitTestEnabled = false;
			Strategy.UnSubscribe();
			Destroy(Strategy.Container, false);
			Ref.Dispose(ref strategyCore);
			ResetClientControlCore();
			ResetInitialization();
			if(IsClientControlDeletedInDesignMode())
				ShowNullParentContainerWarning();
		}
		bool IsClientControlDeletedInDesignMode() {
			IDesignerHost service = GetDesignerHostService();
			return (service != null) && !service.Loading;
		}
		static void ShowNullParentContainerWarning() {
			string caption = Docking.DockManagerLocalizer.GetString(
				Docking.DockManagerStringId.MessageFormPropertyChangedCaption);
			string message = DocumentManagerLocalizer.GetString(
				DocumentManagerStringId.NullParentContainerExceptionMessage);
			DevExpress.XtraEditors.XtraMessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
		}
		#endregion IClientControlListener
		#region IDocumentsHostOwner
		void IDocumentsHostOwner.HandleCreated() {
			OnHandleCreatedCore();
		}
		void IDocumentsHostOwner.HandleDestroyed() {
			OnHandleDestroyedCore();
		}
		void IDocumentsHostOwner.VisibleChanged(bool visible) {
			OnVisibleChanged(visible);
		}
		Rectangle IDocumentsHostOwner.CalculateNC(Rectangle bounds) {
			if(!CanUpdateLayout()) return bounds;
			return CalculateNCCore(bounds);
		}
		void IDocumentsHostOwner.Invalidate() {
		}
		void IDocumentsHostOwner.Paint(Graphics g) {
			PaintCore(g, Bounds);
		}
		void IDocumentsHostOwner.ControlAdded(Control control) {
			if(control is DocumentContainer) return;
			OnControlAdded(control);
		}
		int documentHostControlRemoved = 0;
		protected internal bool InDocumentsHostControlRemoved {
			get { return documentHostControlRemoved > 0; }
		}
		void IDocumentsHostOwner.ControlRemoved(Control control) {
			DocumentContainer documentContainer = control as DocumentContainer;
			if(documentContainer != null) {
				if(documentContainer.IsDisposing) return;
				if(documentContainer.Document.IsDockPanel)
					control = documentContainer.Document.Form;
				else control = documentContainer.Document.Control;
			}
			documentHostControlRemoved++;
			OnControlRemoved(control);
			documentHostControlRemoved--;
		}
		void IDocumentsHostOwner.ParentBackColorChanged() {
			if(View != null && View.ViewInfo != null)
				View.ViewInfo.SetAppearanceDirty();
		}
		#endregion
		#region IMdiClientSubclasserOwner Members
		void IMdiClientSubclasserOwner.HandleCreated() {
			OnHandleCreatedCore();
		}
		void IMdiClientSubclasserOwner.HandleDestroyed() {
			OnHandleDestroyedCore();
		}
		void IMdiClientSubclasserOwner.OnSetNextMdiChild(DevExpress.XtraTabbedMdi.SetNextMdiChildEventArgs e) {
			if(!CanUpdateLayout()) return;
			OnSetNextMdiChildCore(e);
		}
		void IMdiClientSubclasserOwner.OnContextMenu() {
			if(!CanUpdateLayout()) return;
		}
		Rectangle IMdiClientSubclasserOwner.CalculateNC(Rectangle bounds) {
			if(!CanUpdateLayout()) return bounds;
			return CalculateNCCore(bounds);
		}
		void IMdiClientSubclasserOwner.InvalidateNC() {
		}
		void IMdiClientSubclasserOwner.Paint(Graphics g) {
			DevExpress.Skins.SkinElementPainter.CorrectByRTL = IsFormRightToLeftLayout();
			Rectangle bounds = Bounds;
			if(DevExpress.Skins.SkinElementPainter.CorrectByRTL) 
				bounds.X--;
			PaintCore(g, bounds);
			DevExpress.Skins.SkinElementPainter.CorrectByRTL = false;
		}
		void IMdiClientSubclasserOwner.EraseBackground(Graphics g) {
			EraseBackgroundCore(g);
		}
		void IMdiClientSubclasserOwner.DrawNC(DXPaintEventArgs e) {
		}
		bool IMdiClientSubclasserOwner.AllowMdiLayout {
			get { return (View != null) && View.AllowMdiLayout; }
		}
		bool IMdiClientSubclasserOwner.AllowMdiSystemMenu {
			get { return (View != null) && View.AllowMdiSystemMenu; }
		}
		IDocumentManagementStrategy strategyCore;
		protected IDocumentManagementStrategy Strategy {
			[System.Diagnostics.DebuggerStepThrough]
			get {
				if(strategyCore == null) {
					if(!IsDisposing)
						EnsureStrategy();
				}
				return strategyCore;
			}
		}
		protected void EnsureStrategy() {
			if(MdiParent != null)
				strategyCore = new MDIClientManagementStrategy(this);
			if(ContainerControl != null)
				strategyCore = new ContainerControlManagementStrategy(this);
			if(strategyCore == null)
				strategyCore = new NoDocumentsStrategy(this);
		}
		protected bool CanUpdateLayout() {
			if(IsUpdateLayoutRestricted(false)) return false;
			return Strategy.CanUpdateLayout();
		}
		protected bool CanUpdateLayout(bool ignoreLockUpdateLayout) {
			if(IsUpdateLayoutRestricted(ignoreLockUpdateLayout)) return false;
			return Strategy.CanUpdateLayout();
		}
		internal int lockUpdateLayout = 0;
		protected bool IsUpdateLayoutRestricted(bool ignoreLockUpdateLayout) {
			return IsInitializing || IsDisposing || (strategyCore == null) || (!ignoreLockUpdateLayout && lockUpdateLayout > 0);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Rectangle Bounds {
			get { return boundsCore; }
		}
		protected internal bool HitTestEnabled;
		protected internal bool IsStrategyValid {
			get { return !IsDisposing && Strategy.IsValid; }
		}
		protected internal bool IsMdiStrategyInUse {
			get { return strategyCore is MDIClientManagementStrategy; }
		}
		protected internal bool IsContainerControlStrategyInUse {
			get { return strategyCore is ContainerControlManagementStrategy; }
		}
		protected internal bool IsNoDocumentsStrategyInUse {
			get { return strategyCore is NoDocumentsStrategy; }
		}
		protected internal ContainerControl GetContainer() {
			return Strategy.Container;
		}
		protected internal Control GetActiveChild() {
			return Strategy.GetActiveChild();
		}
		protected internal void Activate(BaseDocument document) {
			if(IsUpdateLayoutRestricted(false)) return;
			Control child = Strategy.GetChild(document);
			if(child != null) Strategy.Activate(child);
		}
		protected internal void Activate(Control child) {
			if(!CanUpdateLayout()) return;
			Strategy.Activate(child);
			Strategy.Invalidate();
		}
		protected internal void PostFocus(Control child) {
			if(!CanUpdateLayout()) return;
			Strategy.PostFocus(child);
		}
		protected internal IntPtr GetOwnerControlHandle() {
			return Strategy.GetOwnerControlHandle();
		}
		protected internal bool IsOwnerControlHandleCreated {
			get { return Strategy.IsOwnerControlHandleCreated; }
		}
		protected internal Control GetOwnerControl() {
			if(!CanUpdateLayout(true)) return null;
			return Strategy.GetOwnerControl();
		}
		protected internal bool GetOwnerControlEmpty() {
			return Strategy.IsOwnerControlEmpty;
		}
		public void Invalidate() {
			if(!CanUpdateLayout()) return;
			Strategy.Invalidate();
		}
		public void Invalidate(Rectangle rect) {
			if(!CanUpdateLayout()) return;
			Strategy.Invalidate(rect);
		}
		public void Update() {
			if(!CanUpdateLayout()) return;
			Strategy.Update();
		}
#if DEBUGTEST
		internal SharedRef<Views.DocumentsHostContext> documentsHostContextRef;
#else
		SharedRef<Views.DocumentsHostContext> documentsHostContextRef;
#endif
		internal Views.DocumentsHostContext EnsureDocumentsHostContext() {
			if(documentsHostContextRef == null) {
				Views.DocumentsHostContext context = new Views.DocumentsHostContext(this);
				documentsHostContextRef = new SharedRef<Views.DocumentsHostContext>(context);
			}
			return documentsHostContextRef.Target;
		}
		internal Views.DocumentsHostContext GetDocumentsHostContext() {
			return documentsHostContextRef != null ? documentsHostContextRef.Target : null;
		}
		protected internal void MakeChild(DocumentManager parentManager) {
			HitTestEnabled = true;
			if(parentManager == this) return;
			if(UIViewAdapter != null && UIView != null)
				UIViewAdapter.Views.Remove(UIView);
			parentManager.uiViewAdapterRef.Share(ref uiViewAdapterRef);
			parentManager.documentsHostContextRef.Share(ref documentsHostContextRef);
			parentManager.activationInfoRef.Share(ref activationInfoRef);
			parentManager.thumbnailManagerRef.Share(ref thumbnailManagerRef);
			if(UIViewAdapter != null && UIView != null) {
				UIViewAdapter.Views.Add(UIView);
				UIView.Invalidate();
			}
		}
		protected internal void MoveFloatFormsTo(DocumentManager parentManager) {
			if(parentManager == this || parentManager == null || parentManager.IsDisposing) return;
			using(BatchUpdate.Enter(View, true)) {
				using(BatchUpdate.Enter(parentManager.View, true)) {
					BaseDocument[] documents = View.FloatDocuments.CleanUp();
					using(var container = new DisposableObjectsContainer()) {
						for(int i = 0; i < documents.Length; i++) {
							container.Register(documents[i].TrackContainerChange(parentManager.View.Container));
							documents[i].Form.Owner = Views.DocumentsHostContext.GetForm(parentManager);
						}
						parentManager.View.FloatDocuments.AddRange(documents);
					}
				}
			}
		}
		protected internal void EnsureDocument(BaseDocument document) {
			Strategy.Ensure(document);
		}
		protected internal void AddDocumentToHost(BaseDocument document) {
			Strategy.AddDocumentToHost(document);
		}
		protected internal void RemoveDocumentFromHost(BaseDocument document) {
			LayeredWindowsNotityReparented(document.Control);
			Strategy.RemoveDocumentFromHost(document);
		}
		protected internal void DockFloatForm(BaseDocument document, Action<Form> patchAction) {
			LayeredWindowsNotityReparented(document.Control);
			LayeredWindowsUnregisterNotificationSourceForFloatView(document.Form);
			Strategy.DockFloatForm(document, patchAction);
		}
		protected internal void DockFloatForm(BaseDocument document) {
			LayeredWindowsNotityReparented(document.Control);
			LayeredWindowsUnregisterNotificationSourceForFloatView(document.Form);
			Strategy.DockFloatForm(document);
		}
		protected internal void DockFloatForm(BaseDocument document, Docking.DockPanel panel) {
			LayeredWindowsNotityReparented(document.Control);
			LayeredWindowsUnregisterNotificationSourceForFloatView(document.Form);
			Strategy.DockFloatForm(document, panel);
		}
		protected internal Control GetChild(BaseDocument document) {
			return Strategy.GetChild(document);
		}
		protected internal bool CheckFocus(int msg, BaseView view, Control control, IntPtr wParam) {
			return Strategy.CheckFocus(msg, view, control, wParam);
		}
		protected internal void PatchControlBeforeAdd(Control control) {
			Strategy.PatchControlBeforeAdd(control);
		}
		protected internal void PatchControlAfterRemove(Control control) {
			Strategy.PatchControlAfterRemove(control);
		}
		protected internal void PatchDocumentBeforeFloat(BaseDocument document) {
			Strategy.PatchDocumentBeforeFloat(document);
		}
		protected internal void LayeredWindowsRegisterNotificationSourceForFloatView(Form form) { 
			if(notificationSourceCore != null && (form != null) && form.IsHandleCreated)
				DevExpress.Utils.Internal.LayeredWindowNotificationSource.Register(form.Handle, notificationSourceCore);
		}
		protected internal void LayeredWindowsUnregisterNotificationSourceForFloatView(Form form) {
			if(notificationSourceCore != null && (form != null) && form.IsHandleCreated)
				DevExpress.Utils.Internal.LayeredWindowNotificationSource.Unregister(form.Handle, notificationSourceCore);
		}
		protected internal void LayeredWindowsNotifyHidden(Control control) {
			if(notificationSourceCore != null && (control != null) && control.IsHandleCreated)
				notificationSourceCore.NotifyHidden(control.Handle);
		}
		protected internal void LayeredWindowsNotityReparented(Control control) {
			if(notificationSourceCore != null && (control != null) && control.IsHandleCreated)
				notificationSourceCore.NotityReparented(control.Handle);
		}
		internal Docking.DockManager dockManagerCore;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Docking.DockManager DockManager {
			[System.Diagnostics.DebuggerStepThrough]
			get {
				if(dockManagerCore == null)
					InitializeDockManager();
				return dockManagerCore;
			}
		}
		protected void InitializeDockManager() {
			if(Container == null || Container.Components == null) return;
			foreach(IComponent component in Container.Components) {
				Docking.DockManager manager = component as Docking.DockManager;
				if(manager != null && manager.Form == Strategy.Container) {
					manager.ReleaseDocumentManager();
					manager.documentManagerCore = this;
					dockManagerCore = manager;
					return;
				}
			}
		}
		protected void ReleaseDockManager() {
			if(dockManagerCore == null) return;
			dockManagerCore.ReleaseDocumentManager();
			dockManagerCore = null;
		}
		Adorner adornerCore;
		protected internal Adorner Adorner {
			get { return adornerCore; }
		}
		SnapAdorner snapAdornerCore;
		protected internal SnapAdorner SnapAdorner {
			get { return snapAdornerCore; }
		}
		protected virtual Adorner CreateAdorner() {
			return new Adorner(Strategy.GetOwnerControl());
		}
		protected virtual SnapAdorner CreateSnapAdorner() {
			return new SnapAdorner(Strategy.GetOwnerControl());
		}
		#endregion
		protected internal void InvokePatchActiveChildren() {
			if(IsDisposing || IsNoDocumentsStrategyInUse) return;
			if(!CanUpdateLayout() || View == null || View.IsDisposing) return;
			if(!View.IsPaintingLocked)
				InvokePatchActiveChildren(Strategy.Container);
			else View.RequestInvokePatchActiveChild();
		}
		static object syncObj = new object();
		protected internal void InvokePatchActiveChildren(Control control) {
			if(IsDisposing || IsNoDocumentsStrategyInUse) return;
			lock(syncObj) {
				if(!control.IsHandleCreated) return;
				if(lockPatchActiveChildren == 0) {
					if(lockPatchActiveChildrenInChildManager == 0) {
						lockPatchActiveChildren++;
						control.BeginInvoke(new MethodInvoker(PatchActiveChildren));
					}
				}
			}
		}
		bool patchActiveChildrenRequeryFlag;
		int patchActiveChildrenRequeryCount = 0;
		protected internal void SetPatchActiveChildrenRequeryFlag() {
			patchActiveChildrenRequeryFlag = true;
		}
		protected internal void PatchActiveChildrenRequery() {
			if(patchActiveChildrenRequeryFlag) {
				patchActiveChildrenRequeryFlag = false;
				if(patchActiveChildrenRequeryCount > 0) {
					patchActiveChildrenRequeryCount = 0;
					InvokePatchActiveChildren();
				}
			}
		}
		internal int lockPatchActiveChildrenInChildManager = 0;
		int lockPatchActiveChildren = 0;
		protected void PatchActiveChildren() {
			lock(syncObj) {
				try {
					if(!CanUpdateLayout() || View == null || View.IsDisposing) return;
					if(patchActiveChildrenRequeryFlag) {
						patchActiveChildrenRequeryCount++;
						return;
					}
					Size size = View.Bounds.Size;
					PatchChildrenCore();
					if(View.ShouldRetryPatchActiveChildren(size))
						PatchChildrenCore();
				}
				finally { lockPatchActiveChildren--; }
			}
		}
		protected void PatchChildrenCore() {
			View.patchChildrenInProgress++;
			if(View.EnsureLayoutCalculated(null)) {
				Point offsetNC = GetOffsetNC();
				View.PatchActiveChildren(offsetNC);
			}
			View.patchChildrenInProgress--;
		}
		protected internal void PatchBeforeActivateChild(Control activatedChild) {
			if(!CanUpdateLayout() || View == null) return;
			if(View.EnsureLayoutCalculated(null)) {
				Point offsetNC = GetOffsetNC();
				View.PatchBeforeActivateChild(activatedChild, offsetNC);
			}
		}
		Rectangle boundsCore;
		Rectangle prevBounds;
		protected virtual Rectangle CalculateNCCore(Rectangle bounds) {
			if(CanUpdateThumbnails()) {
				ThumbnailManager.UpdateThumbnails();
				return bounds;
			}
			if(View != null && View.HasNCElements)
				bounds = View.CalcNCBounds(bounds);
			if(!prevBounds.IsEmpty && prevBounds != bounds)
				OnResize(bounds);
			prevBounds = bounds;
			boundsCore = new Rectangle(Point.Empty, bounds.Size);
			if(View != null && !bounds.IsEmpty) {
				using(BatchUpdate.Enter(this, true))
					View.Resize(Bounds);
			}
			return bounds;
		}
		bool CanUpdateThumbnails() {
			Form form = DocumentsHostContext.GetForm(this);
			return form != null && form.IsHandleCreated && IsInitialized && DevExpress.Utils.Drawing.Helpers.NativeMethods.IsIconic(form.Handle);
		}
		protected virtual void OnSetNextMdiChildCore(DevExpress.XtraTabbedMdi.SetNextMdiChildEventArgs e) {
			if(View != null && View.SetNextDocument(e.ForwardNavigation)) {
				e.Handled = true;
			}
		}
		protected virtual void PaintCore(Graphics g, Rectangle bounds) {
			using(GraphicsCache cache = new GraphicsCache(g)) {
				if(View != null)
					View.Draw(cache, bounds);
				else DrawEmpty(cache, bounds);
			}
		}
		protected virtual void EraseBackgroundCore(Graphics g) {
		}
		protected virtual void DrawEmpty(GraphicsCache cache, Rectangle bounds) {
			if(IsDisposing) return;
			cache.FillRectangle(GetParentBackColor(), bounds);
		}
		protected internal Color GetParentBackColor() {
			Control client = Strategy.GetOwnerControl();
			return (client != null && client.Parent != null) ? client.Parent.BackColor :
				LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.Control);
		}
		protected internal Color GetParentForeColor() {
			Control client = Strategy.GetOwnerControl();
			return (client != null && client.Parent != null) ? client.Parent.ForeColor :
				LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.ControlText);
		}
		protected internal Image GetParentBackgroundImage() {
			Control client = Strategy.GetOwnerControl();
			return (client != null && client.Parent != null) ? client.Parent.BackgroundImage : GetSkinBackgroundImage();
		}
		protected internal ImageLayout GetParentBackgroundImageLayout() {
			Control client = Strategy.GetOwnerControl();
			return (client != null && client.Parent != null) ? GetBackgroundImageLayout(client.Parent) : ImageLayout.None;
		}
		Image GetSkinBackgroundImage() {
			if(PaintStyleName != "Skin") return null;
			DevExpress.Skins.SkinElement skinElement = GetFormSkin();
			if(skinElement != null && skinElement.Image != null)
				return skinElement.Image.Image;
			return null;
		}
		ImageLayout GetBackgroundImageLayout(Control parent) {
			if(parent.BackgroundImage != null)
				return parent.BackgroundImageLayout;
			if(GetSkinBackgroundImage() != null)
				return GetSkinBackgroundImageLayout();
			return ImageLayout.None;
		}
		ImageLayout GetSkinBackgroundImageLayout() {
			if(PaintStyleName != "Skin") return ImageLayout.None;
			DevExpress.Skins.SkinElement skinElement = GetFormSkin();
			if(skinElement != null && skinElement.Image != null) {
				DevExpress.Skins.SkinImageStretch stretch = skinElement.Image.Stretch;
				switch(stretch) {
					case DevExpress.Skins.SkinImageStretch.NoResize:
						return ImageLayout.None;
					case DevExpress.Skins.SkinImageStretch.Stretch:
						return ImageLayout.Stretch;
					case DevExpress.Skins.SkinImageStretch.Tile:
						return ImageLayout.Tile;
				}
			}
			return ImageLayout.None;
		}
		DevExpress.Skins.SkinElement GetFormSkin() {
			return DevExpress.Skins.CommonSkins.GetSkin(LookAndFeel)[DevExpress.Skins.CommonSkins.SkinForm];
		}
		void OnViewDisposed(object sender, EventArgs e) {
			if(ActivationInfo != null)
				ActivationInfo.Detach(View);
			BaseView newView = ViewCollection.Count > 0 ? ViewCollection[0] : null;
			if(IsDisposing)
				newView = null;
			View = newView;
		}
		protected virtual void OnViewChanging() {
			if(View != null) RaiseViewChanging(View);
		}
		protected virtual void OnViewChanged() {
			if(View != null) RaiseViewChanged(View);
			if(!IsDisposing && !IsInitializing) {
				PopulateDocuments();
				UpdateDocumentSelector();
			}
		}
		void PopulateDocuments() {
			if(ContainerControl != null) {
				IDocumentsHost host = DocumentsHost.GetDocumentsHost(ContainerControl);
				if(host != null) {
					DocumentContainer[] containers = host.Containers;
					for(int i = 0; i < containers.Length; i++) {
						if(containers[i].Document.IsDockPanel) {
							Docking.DockPanel panel = containers[i].Document.GetDockPanel();
							if(panel != null)
								panel.Restore();
						}
					}
					containers = Array.FindAll(host.Containers, c => c.Document.IsControlLoaded);
					Control[] children = new Control[containers.Length];
					for(int i = 0; i < children.Length; i++) 
						children[i] = containers[i].Document.Control;
					Populate(children);
				}
			}
			if(MdiParent != null) {
				Form[] containers = new Form[MdiParent.MdiChildren.Length];
				MdiParent.MdiChildren.CopyTo(containers, 0);
				for(int i = 0; i < containers.Length; i++) {
					if(containers[i] is Docking.FloatForm) {
						Docking.DockPanel panel = ((Docking.FloatForm)containers[i]).FloatLayout.Panel;
						if(panel != null) panel.Restore();
					}
				}
				Populate(MdiParent.MdiChildren);
			}
		}
		void UpdateDocumentSelector() {
			DestroyDocumentSelector();
			if(View != null && View.CanUseDocumentSelector())
				CreateDocumentSelector();
		}
		protected virtual void OnResize(Rectangle bounds) {
			if(UIView != null) UIView.Invalidate();
			InvokePatchActiveChildren(Strategy.Container);
		}
		protected override void OnInitialized() {
			EnsureMainView();
			if(Strategy.OnInitialized()) {
				Initialize(Strategy.Container);
				if(View != null && View.CanUseDocumentSelector())
					CreateDocumentSelector();
			}
			else {
				if(DesignMode) {
					IDesignerHost service = GetDesignerHostService();
					if(service != null && service.Loading) {
						service.LoadComplete += service_LoadComplete;
						isInitializationDelayedWhileDesignerHostLoading = true;
					}
				}
			}
			UpdateScaleFactor();
		}
		static PropertyInfo autoScaleFactorInfo;
		protected void UpdateScaleFactor() {
			if(Strategy == null || Strategy.Container == null || !IsOwnerControlHandleCreated) return;
			if(autoScaleFactorInfo == null)
				autoScaleFactorInfo = typeof(ContainerControl).GetProperty("AutoScaleFactor", BindingFlags.NonPublic | BindingFlags.Instance);
			scaleFactorCore = (SizeF)autoScaleFactorInfo.GetValue(Strategy.Container, null);
		}
		SizeF? scaleFactorCore;
		protected internal SizeF ScaleFactor {
			get {
				if(!scaleFactorCore.HasValue)
					UpdateScaleFactor();
				return scaleFactorCore.GetValueOrDefault(new SizeF(1.0f, 1.0f)); 
			}
		}
		bool isInitializationDelayedWhileDesignerHostLoading;
		void service_LoadComplete(object sender, EventArgs e) {
			((IDesignerHost)sender).LoadComplete -= service_LoadComplete;
			if(isInitializationDelayedWhileDesignerHostLoading) {
				OnInitialized();
				isInitializationDelayedWhileDesignerHostLoading = false;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void ForceInitialize() {
			HitTestEnabled = true;
			InitializeCore();
			if(View != null) {
				var control = GetOwnerControl() as Control;
				if(control != null)
					View.Resize(control.Bounds);
			}
		}
		DevExpress.Utils.Internal.ILayeredWindowNotificationSource notificationSourceCore;
		void EnsureLayeredWindowNotificationSource() {
			if(notificationSourceCore == null)
				notificationSourceCore = DevExpress.Utils.Internal.LayeredWindowNotificationSource.Register(Strategy.GetOwnerControlHandle());
		}
		void DestroyLayeredWindowNotificationSource() {
			Ref.Dispose(ref notificationSourceCore);
		}
		protected virtual void OnHandleCreatedCore() {
			if(IsRecreatingHandle) {
				if(adornerCore == null)
					adornerCore = CreateAdorner();
				if(snapAdornerCore == null)
					snapAdornerCore = CreateSnapAdorner();
			}
			else Ensure(Strategy.Container);
			EnsureLayeredWindowNotificationSource();
		}
		protected virtual void OnHandleDestroyedCore() {
			DestroyLayeredWindowNotificationSource();
			if(!IsRecreatingHandle)
				Destroy(Strategy.Container, true);
			else {
				Ref.Dispose(ref adornerCore);
				Ref.Dispose(ref snapAdornerCore);
			}
		}
		protected bool IsRecreatingHandle {
			get { return Strategy.IsOwnerControlRecreatingHandle; }
		}
		protected virtual void Ensure(ContainerControl container) {
			if(Strategy.IsOwnerControlInvalid) {
				Control client = Strategy.GetOwnerControl();
				if(client != null) {
					Strategy.Ensure(client);
					adornerCore = CreateAdorner();
					snapAdornerCore = CreateSnapAdorner();
					EnsureDragEngineInitialized();
					AddMdiClientToToolTipControllerCore(client);
					if(View != null && View.CanUseDocumentSelector())
						CreateDocumentSelector();
					Strategy.IsOwnerControlInvalid = false;
				}
			}
			else EnsureDragEngineInitialized();
		}
		protected virtual void DestroyClientControl() {
			if(ClientControl == null) return;
			DestroyCore(null, false);
		}
		protected virtual void Destroy(ContainerControl container) {
			Destroy(container, false);
		}
		protected virtual void Destroy(ContainerControl container, bool preserveSubclasser) {
			if(container == null) return;
			if(preserveSubclasser && lockPatchActiveChildren > 0)
				lockPatchActiveChildren = 0;
			DestroyCore(container, preserveSubclasser);
		}
		void DestroyCore(ContainerControl container, bool preserveSubclasser) {
			ActivationInfo.Detach(DockManager);
			DepopulateFloatPanels();
			RemoveClientFromToolTipController();
			DestroyDocumentSelector();
			Ref.Dispose(ref adornerCore);
			Ref.Dispose(ref snapAdornerCore);
			Strategy.Destroy(container, preserveSubclasser);
			Ref.Dispose(ref uiViewCore);
			Ref.Dispose(ref uiViewAdapterRef);
		}
		internal AdornerElementInfo documentSelectorInfo;
		#region IDebuggingProcess
		ProcessExecuting processExecutingCore = null;
		event ProcessExecuting IProcessRunningListener.ProcessExecuting {
			add { this.Events.AddHandler(processExecutingCore, value); }
			remove { this.Events.RemoveHandler(processExecutingCore, value); }
		}
		protected internal virtual void RaiseProcessRunning(ProcessRunningEventArgs e) {
			ProcessExecuting handler = (ProcessExecuting)this.Events[processExecutingCore];
			if(handler != null)
				handler(this, e);
		}
		bool isRunningCore;
		bool IProcessRunningListener.IsRunning {
			get {
				RaiseProcessRunning(new ProcessRunningEventArgs(this));
				return isRunningCore;
			}
			set { isRunningCore = value; }
		}
		object IServiceProvider.GetService(Type serviceType) {
			return GetService(serviceType);
		}
		#endregion IDebuggingProcess
		protected virtual void Initialize(ContainerControl container) {
			Control client = Strategy.Initialize(container);
			prevBounds = client.Bounds;
			adornerCore = CreateAdorner();
			snapAdornerCore = CreateSnapAdorner();
			if(client.IsHandleCreated && client.Visible || IsNoDocumentsStrategyInUse)
				EnsureDragEngineInitialized();
			AddClientToToolTipController();
			ActivationInfo.Attach(DockManager);
		}
		[Browsable(false)]
		public bool IsDocumentSelectorVisible {
			get { return documentSelectorBootStrapperCore != null && documentSelectorBootStrapperCore.isShown; }
		}
		protected internal void CheckDocumentSelectorVisibility() {
			if(documentSelectorBootStrapperCore != null && documentSelectorBootStrapperCore.isShown)
				documentSelectorBootStrapperCore.Cancel();
		}
		protected internal void CreateDocumentSelector() {
			DestroyDocumentSelector();
			var info = Customization.DocumentSelectorAdornerElementInfoArgs.EnsureInfoArgs(ref documentSelectorInfo, this);
			documentSelectorBootStrapperCore = new DocumentSelectorBootStrapper(info);
		}
		protected internal void DestroyDocumentSelector() {
			Ref.Dispose(ref documentSelectorBootStrapperCore);
		}
		protected override void OnLayoutChanged() {
			if(strategyCore != null && Strategy.CanUpdateLayout())
				Strategy.UpdateLayout();
			if(UIView != null)
				UIView.Invalidate();
			Invalidate();
		}
		protected void EnsureMainView() {
			if(IsDisposing) return;
			if(strategyCore == null)
				EnsureStrategy();
			if(!IsNoDocumentsStrategyInUse) {
				if(View == null) {
					BaseView defaultView = CreateDefaultView();
					if(Container != null)
						Container.Add(defaultView);
					View = defaultView;
				}
			}
			else {
				BaseView noDocumentsView = ViewCollection.FindFirst(view => view.Type == ViewType.NoDocuments);
				if(noDocumentsView == null) {
					noDocumentsView = CreateNoDocumentsView();
					if(Container != null)
						Container.Add(noDocumentsView);
				}
				View = noDocumentsView;
			}
		}
		protected virtual BaseView CreateDefaultView() {
			return CreateView(ViewType.Tabbed);
		}
		protected virtual BaseView CreateNoDocumentsView() {
			return CreateView(ViewType.NoDocuments);
		}
		public virtual BaseView CreateView(ViewType type) {
			switch(type) {
				case ViewType.Tabbed: return new Views.Tabbed.TabbedView(Container);
				case ViewType.NativeMdi: return new Views.NativeMdi.NativeMdiView(Container);
				case ViewType.WindowsUI: return new Views.WindowsUI.WindowsUIView(Container);
				case ViewType.Widget: return new Views.Widget.WidgetView(Container);
				case ViewType.NoDocuments: return new Views.NoDocuments.NoDocumentsView(Container);
			}
			throw new NotSupportedException(type.ToString());
		}
		protected internal void Populate(Control[] children) {
			if(View == null) return;
			using(BatchUpdate.Enter(this)) {
				using(View.LockPainting()) {
					BaseDocument[] documents = View.Documents.ToArray();
					foreach(BaseDocument document in documents)
						document.EnsureDeferredLoadControlOnPopulate(View);
					foreach(Control child in children)
						OnControlAdded(child);
					View.OnPopulated();
				}
			}
		}
		int depopulating = 0;
		protected internal bool IsDepopulating {
			get { return depopulating > 0; }
		}
		protected internal void Depopulate() {
			if(View == null || View.Documents == null) return;
			using(BatchUpdate.Enter(this, true)) {
				depopulating++;
				View.OnDepopulating();
				try {
					BaseDocument[] documents = View.Documents.ToArray();
					foreach(BaseDocument document in documents) {
						document.ReleaseDeferredLoadControlOnDepopulate(View);
						if(!document.IsDesignModeContent)
							OnControlRemoved(document.Control);
					}
				}
				finally {
					depopulating--;
				}
				DepopulateFloatPanels();
				View.OnDepopulated();
			}
		}
		void DepopulateFloatPanels() {
			if(View == null || View.FloatPanels == null) return;
			BaseDocument[] floatPanels = View.FloatPanels.CleanUp();
			for(int i = 0; i < floatPanels.Length; i++) {
				floatPanels[i].LockFormDisposing();
				floatPanels[i].Dispose();
				floatPanels[i].UnLockFormDisposing();
			}
		}
		#region IViewRegistrator
		IDictionary<ViewType, BaseRegistrator.Create> registrators;
		BaseViewPainter IViewRegistrator.CreatePainter(BaseView view) {
			BaseRegistrator.Create registrator;
			if(registrators.TryGetValue(view.Type, out registrator))
				return registrator(view).CreatePainter();
			return null;
		}
		BaseViewInfo IViewRegistrator.CreateViewInfo(BaseView view) {
			BaseRegistrator.Create registrator;
			if(registrators.TryGetValue(view.Type, out registrator))
				return registrator(view).CreateViewInfo();
			return null;
		}
		BaseDocument IViewRegistrator.CreateDocument(BaseView view, Control control) {
			BaseRegistrator.Create registrator;
			if(registrators.TryGetValue(view.Type, out registrator))
				return registrator(view).CreateDocument(control);
			return null;
		}
		BaseViewHitInfo IViewRegistrator.CreateHitInfo(BaseView view) {
			BaseRegistrator.Create registrator;
			if(registrators.TryGetValue(view.Type, out registrator))
				return registrator(view).CreateHitInfo();
			return null;
		}
		DocumentContainer IViewRegistrator.CreateDocumentContainer(BaseDocument document) {
			BaseRegistrator.Create registrator;
			if(registrators.TryGetValue(View.Type, out registrator))
				return registrator(View).CreateDocumentContainer(document);
			return null;
		}
		Control IViewRegistrator.CreateDocumentsHost(IDocumentsHostOwner owner) {
			BaseRegistrator.Create registrator;
			if(registrators.TryGetValue(View.Type, out registrator))
				return registrator(View).CreateDocumentsHost(owner);
			return null;
		}
		protected virtual void RegisterViews() {
			RegisterView<TabbedViewRegistrator>(ViewType.Tabbed);
			RegisterView<NativeMdiViewRegistrator>(ViewType.NativeMdi);
			RegisterView<NoDocumentsViewRegistrator>(ViewType.NoDocuments);
			RegisterView<WindowsUIViewRegistrator>(ViewType.WindowsUI);
			RegisterView<WidgetViewRegistrator>(ViewType.Widget);
		}
		protected void RegisterView<T>(ViewType viewType) where T : BaseRegistrator, new() {
			registrators[viewType] = BaseRegistrator.Register<T>();
		}
		#endregion IViewRegistrator
		#region IMdiClientListener Members
		int mdiClientControlAdded = 0;
		protected internal bool InMdiClientControlAdded {
			get { return mdiClientControlAdded > 0; }
		}
#pragma warning disable 0618
		void IMdiClientListener.OnControlAdded(Control control) {
			if(control is FloatDocumentForm) return;
			DevExpress.Utils.Drawing.Animation.XtraAnimator.Current.DisableBitmapAnimation = true;
			mdiClientControlAdded++;
			OnControlAdded(control);
			mdiClientControlAdded--;
		}
#pragma warning restore 0618
		void IMdiClientListener.OnControlRemoved(Control control) {
			FloatDocumentForm floatDocumentForm = control as FloatDocumentForm;
			if(floatDocumentForm != null) {
				if(floatDocumentForm.IsDisposing) return;
				control = floatDocumentForm.Document.Control;
			}
			OnControlRemoved(control);
		}
		void IMdiClientListener.OnDisposed() {
			if(IsDisposing) return;
			HitTestEnabled = false;
			Strategy.UnSubscribe();
			Destroy(Strategy.Container, false);
			Ref.Dispose(ref strategyCore);
			ResetInitialization();
		}
		void IMdiClientListener.OnVisibleChanged(bool visible) {
			OnVisibleChanged(visible);
		}
		protected virtual void OnControlAdded(Control control) {
			if(IsDisposing || control == null || control.IsDisposed) return;
			using(BatchUpdate.Enter(this)) {
				if(View != null)
					View.AddDocument(control);
			}
		}
		protected virtual void OnControlRemoved(Control control) {
			if(IsDisposing || control == null || control.IsDisposed) return;
			using(BatchUpdate.Enter(this)) {
				if(control.Disposing)
					CancelDragOperation();
				if(View != null)
					View.RemoveDocument(control);
			}
		}
		protected virtual void OnVisibleChanged(bool visible) {
			if(visible)
				Ensure(Strategy.Container);
			else {
				Ref.Dispose(ref uiViewCore);
				Ref.Dispose(ref uiViewAdapterRef);
			}
			if(View != null) View.OnVisibleChanged(visible);
		}
		protected internal void CancelDragOperation() {
			if(UIViewAdapter != null && UIViewAdapter.DragService != null)
				UIViewAdapter.DragService.CancelDragOperation();
		}
		protected void EnsureDragEngineInitialized() {
			if(uiViewAdapterRef == null)
				uiViewAdapterRef = new SharedRef<IUIViewAdapter>(CreateViewAdapter());
			if(UIView == null) {
				uiViewCore = CreateDocumentManagerUIView();
				UIViewAdapter.Views.Add(UIView);
			}
			PerformFloatDocumentsRegistration();
		}
		#endregion
		#region IClientRectTranslator Members
		protected virtual DXMouseEventArgs CreateDXMouseEventArgs(MouseEventArgs e) {
			Point offset = GetOffsetNC();
			return new DXMouseEventArgs(e.Button, e.Clicks, e.X - offset.X, e.Y - offset.Y, e.Delta);
		}
		protected virtual internal Rectangle GetDockingRect() {
			Rectangle dockingRect = Bounds;
			if(DockManager != null) {
				ContainerControl container = Strategy.Container ?? DockManager.Form;
				dockingRect = DockManager.GetDockingBounds(container.ClientRectangle);
				Control client = Strategy.GetOwnerControl();
				Point offset = client.Bounds.Location;
				dockingRect.Offset(-offset.X, -offset.Y);
			}
			return dockingRect;
		}
		protected virtual internal Point GetOffsetNC() {
			return Point.Empty;
		}
		#endregion
		#region public API
		public virtual Point ClientToScreen(Point point) {
			Point offset = GetOffsetNC();
			Point clientPoint = new Point(point.X + offset.X, point.Y + offset.Y);
			Control host = Strategy.GetOwnerControl();
			return host.PointToScreen(clientPoint);
		}
		public virtual Point ScreenToClient(Point screenPoint) {
			Point offset = GetOffsetNC();
			Point clientPoint = new Point(screenPoint.X - offset.X, screenPoint.Y - offset.Y);
			Control host = Strategy.GetOwnerControl();
			return host.PointToClient(clientPoint);
		}
		public BaseViewHitInfo CalcHitInfo(Point point) {
			BaseViewHitInfo result = View.CreateHitInfo();
			if(!IsDisposing && UIView != null) {
				LayoutElementHitInfo hitInfo = UIViewAdapter.CalcHitInfo(UIView, point);
				if(result != null)
					result.CheckAndSetElement(hitInfo);
			}
			return result;
		}
		public BaseDocument GetDocument(Control control) {
			BaseDocument result = null;
			if(control != null) {
				if(!View.Documents.TryGetValue(control, out result)) {
					View.FloatDocuments.TryGetValue(control, out result);
				}
			}
			return result;
		}
		static IDictionary<Control, DocumentManager> clientControls = new Dictionary<Control, DocumentManager>();
		public static DocumentManager FromContainer(IContainer container, Control containerControl) {
			if(container == null || containerControl == null)
				return null;
			foreach(IComponent component in container.Components) {
				DocumentManager manager = component as DocumentManager;
				if((manager != null) && !manager.IsDisposing && manager.GetContainer() == containerControl)
					return manager;
			}
			return null;
		}
		public static DocumentManager FromControl(Control control) {
			DocumentManager manager = null;
			while(control != null) {
				manager = FromCurrentControl(control);
				if(manager != null) return manager;
				control = control.Parent;
			}
			return null;
		}
		static DocumentManager FromCurrentControAndForm(Control control) {
			return FromCurrentControl(control, false);
		}
		public static DocumentManager FromCurrentControl(Control control, bool useParentForm = true) {
			DocumentManager manager = null;
			if(control == null) return null;
			if(control is DocumentContainer)
				return ((DocumentContainer)control).Manager;
			if(control is IDocumentsHost)
				return ((IDocumentsHost)control).Owner as DocumentManager;
			if(control is BaseFloatDocumentForm)
				return ((BaseFloatDocumentForm)control).Manager;
			if(control is Form) {
				manager = FromForm((Form)control, useParentForm);
				if(manager != null) return manager;
			}
			if(manager == null && control is ContainerControl) {
				manager = FromContainerControl((ContainerControl)control);
				if(manager != null) return manager;
			}
			if(clientControls.TryGetValue(control, out manager))
				return manager;
			return manager;
		}
		static DocumentManager FromForm(Form form, bool useParentForm) {
			if(form.Container != null) {
				foreach(IComponent component in form.Container.Components) {
					DocumentManager noDocumentsManager = component as DocumentManager;
					if(noDocumentsManager != null && noDocumentsManager.IsNoDocumentsStrategyInUse)
						return noDocumentsManager;
				}
			}
			Form mdiParent = null;
			if(useParentForm)
				mdiParent = (form.MdiParent ?? form.Owner) ?? form;
			else
				mdiParent = form;
			MdiClient mdiClient = DevExpress.Utils.Mdi.MdiClientSubclasserService.GetMdiClient(mdiParent);
			if(mdiClient != null) {
				DevExpress.Utils.Mdi.MdiClientSubclasser subclasser =
					DevExpress.Utils.Mdi.MdiClientSubclasser.FromMdiClient(mdiClient);
				if(subclasser != null)
					return subclasser.Owner as DocumentManager;
			}
			return null;
		}
		static DocumentManager FromContainerControl(ContainerControl control) {
			IDocumentsHost host = DocumentsHost.GetDocumentsHost((ContainerControl)control);
			if(host != null)
				return host.Owner as DocumentManager;
			foreach(Control clientControl in control.Controls) {
				DocumentManager manager;
				if(clientControls.TryGetValue(clientControl, out manager))
					return manager;
			}
			return null;
		}
		#endregion public API
		#region Activation
		DocumentActivationScope documentActivationScopeCore = DocumentActivationScope.Default;
#if !SL
	[DevExpressXtraBarsLocalizedDescription("DocumentManagerDocumentActivationScope")]
#endif
		[DefaultValue(DocumentActivationScope.Default), Category("Behavior")]
		public DocumentActivationScope DocumentActivationScope {
			get { return documentActivationScopeCore; }
			set { documentActivationScopeCore = value; }
		}
		protected internal bool IsShareActivationScope(DocumentManager targetManager) {
			if(DocumentActivationScope == DocumentActivationScope.Default)
				return DocumentsHostContext.IsParented(targetManager);
			return DocumentActivationScope != DocumentActivationScope.DocumentsHost;
		}
		RibbonAndBarsMergeStyle ribbonAndBarsMergeStyleCore = RibbonAndBarsMergeStyle.Default;
#if !SL
	[DevExpressXtraBarsLocalizedDescription("DocumentManagerRibbonAndBarsMergeStyle")]
#endif
		[DefaultValue(RibbonAndBarsMergeStyle.Default), Category("Behavior")]
		public RibbonAndBarsMergeStyle RibbonAndBarsMergeStyle {
			get { return ribbonAndBarsMergeStyleCore; }
			set { ribbonAndBarsMergeStyleCore = value; }
		}
		protected internal bool CanMergeOnDocumentActivate() {
			return !IsDisposing && CanMergeOnDocumentActivateCore();
		}
		bool CanMergeOnDocumentActivateCore() {
			if(RibbonAndBarsMergeStyle == RibbonAndBarsMergeStyle.Always)
				return true;
			if(RibbonAndBarsMergeStyle == RibbonAndBarsMergeStyle.WhenNotFloating)
				return ActivationInfo != null && ActivationInfo.ActiveDocument != null && !ActivationInfo.ActiveDocument.IsFloating;
			return false;
		}
		SharedRef<IActivationInfo> activationInfoRef;
		protected internal IActivationInfo ActivationInfo {
			get { return (activationInfoRef != null) ? activationInfoRef.Target : null; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DocumentManagerDocumentActivate"),
#endif
 Category("Behavior")]
		public event DocumentEventHandler DocumentActivate {
			add { if(ActivationInfo != null) ActivationInfo.ActiveDocumentChanged += value; }
			remove { if(ActivationInfo != null) ActivationInfo.ActiveDocumentChanged -= value; }
		}
		#endregion Activation
		#region Tab Thumbnails
		DefaultBoolean showThumbnailsInTaskBarCore = DefaultBoolean.Default;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DocumentManagerShowThumbnailsInTaskBar"),
#endif
 Category("Behavior")]
		[DefaultValue(DefaultBoolean.Default), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public DefaultBoolean ShowThumbnailsInTaskBar {
			get { return AllowShowThumbnailsInTaskBar ? DefaultBoolean.False : showThumbnailsInTaskBarCore; }
			set {
				if(!IsInitializing && AllowShowThumbnailsInTaskBar) return;
				SetValue(ref showThumbnailsInTaskBarCore, value, OnShowTabThumbnailsInTaskBarChanged);
			}
		}
		bool AllowShowThumbnailsInTaskBar { get { return (View == null || !View.AllowShowThumbnailsInTaskBar); } }
		int maxThumbnailCountCore = 5;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DocumentManagerMaxThumbnailCount"),
#endif
 Category("Behavior")]
		[DefaultValue(5), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public int MaxThumbnailCount {
			get { return maxThumbnailCountCore; }
			set { SetValue(ref maxThumbnailCountCore, value, OnMaxTabThumbnailCountChanged); }
		}
		SharedRef<IThumbnailManager> thumbnailManagerRef;
		protected IThumbnailManager ThumbnailManager {
			get { return (thumbnailManagerRef != null) ? thumbnailManagerRef.Target as IThumbnailManager : null; }
		}
		protected virtual IThumbnailManager CreateThumbnailManager() {
			return new ThumbnailManager(this, ActivationInfo as IThumbnailManagerClient);
		}
		void OnMaxTabThumbnailCountChanged() {
			UpdateTaskbarThumbnails();
		}
		void OnShowTabThumbnailsInTaskBarChanged() {
			UpdateTaskbarThumbnails();
		}
		protected internal virtual bool CanUseTaskbarThumbnails() {
			var form = DocumentsHostContext.GetForm(this);
			return !IsDisposing && !DesignMode && ShowThumbnailsInTaskBar == DevExpress.Utils.DefaultBoolean.True && form != null && form.ShowInTaskbar;
		}
		protected internal void UpdateTaskbarThumbnails() {
			if(ThumbnailManager != null)
				ThumbnailManager.UpdateThumbnails();
		}
		#endregion
		#region Events
		static readonly object viewChanging = new object();
		static readonly object viewChanged = new object();
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DocumentManagerViewChanging"),
#endif
 Category("Behavior")]
		public event ViewEventHandler ViewChanging {
			add { Events.AddHandler(viewChanging, value); }
			remove { Events.RemoveHandler(viewChanging, value); }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DocumentManagerViewChanged"),
#endif
 Category("Behavior")]
		public event ViewEventHandler ViewChanged {
			add { Events.AddHandler(viewChanged, value); }
			remove { Events.RemoveHandler(viewChanged, value); }
		}
		void RaiseViewChanging(BaseView view) {
			ViewEventHandler handler = Events[viewChanging] as ViewEventHandler;
			if(handler != null) handler(this, new ViewEventArgs(view));
		}
		void RaiseViewChanged(BaseView view) {
			ViewEventHandler handler = Events[viewChanged] as ViewEventHandler;
			if(handler != null) handler(this, new ViewEventArgs(view));
		}
		#endregion Events
		IEnumerable<Component> ILogicalOwner.GetLogicalChildren() {
			foreach(BaseView view in ViewCollection) {
				yield return view;
			}
		}
		#region Content Selection
		WeakReference queuedControlSelection;
		internal void QueueSelectNextControl(Control content) {
			GetRootManager(this).QueueSelectNextControlCore(content);
		}
		void QueueSelectNextControlCore(Control content) {
			if(queuedControlSelection == null) {
				if(content != null) {
					queuedControlSelection = new WeakReference(content);
					System.Threading.SynchronizationContext.Current.Post(
						(s) => SelectNextControl(), null);
				}
			}
			else queuedControlSelection.Target = content;
		}
		void SelectNextControl() {
			if(queuedControlSelection != null) {
				Control content = queuedControlSelection.Target as Control;
				if(content != null)
					SelectNextControl(content);
				queuedControlSelection = null;
			}
		}
		static DocumentManager GetRootManager(DocumentManager manager) {
			var container = manager.GetContainer();
			while(container != null) {
				var candidate = FromControl(container.Parent);
				if(candidate == null || candidate == manager)
					break;
				manager = candidate;
				container = candidate.GetContainer();
			}
			return manager;
		}
		internal static void SelectNextControl(Control content) {
			if(content.Controls.Count == 0 || !content.SelectNextControl(content, true, false, true, false))
				content.Select();
		}
		#endregion Content Selection
	}
	public class ProcessRunningEventArgs : EventArgs {
		IProcessRunningListener processRunningCore;
		public ProcessRunningEventArgs(IProcessRunningListener processRunning) {
			processRunningCore = processRunning;
		}
		public IProcessRunningListener ProcessRunning { get { return processRunningCore; } }
	}
	public delegate void ProcessExecuting(object sender, ProcessRunningEventArgs e);
	public interface IProcessRunningListener : IServiceProvider {
		bool IsRunning { get; set; }
		event ProcessExecuting ProcessExecuting;
	}
	public class SkipOnDocumentCreation : Attribute {
		public static string Name {
			get { return "DevExpress.XtraBars.Docking2010.SkipOnDocumentCreation"; }
		}
	}
}
