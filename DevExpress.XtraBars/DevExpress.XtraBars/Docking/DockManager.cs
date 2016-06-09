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
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Serializing;
using DevExpress.XtraBars.Docking.Helpers;
using DevExpress.XtraBars.Docking.Paint;
using DevExpress.XtraBars.Docking2010.Customization;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
namespace DevExpress.XtraBars.Docking {
	[Designer("DevExpress.XtraBars.Docking.Design.DockManagerDesigner, " + AssemblyInfo.SRAssemblyBarsDesign, typeof(System.ComponentModel.Design.IDesigner)),
   DesignerSerializer("DevExpress.XtraBars.Docking.Design.DockManagerSerializer, " + AssemblyInfo.SRAssemblyBarsDesign, "System.ComponentModel.Design.Serialization.CodeDomSerializer, System.Design"),
	Description("Allows you to create dock windows."),
	ToolboxTabName(AssemblyInfo.DXTabNavigation), DXToolboxItem(true), SerializationOrder(Order = 1)
]
	[ToolboxBitmap(typeof(ToolboxIcons.ToolboxIconsRootNS), "DockManager")]
	public class DockManager : Component, ISupportInitialize, IXtraSerializable, IXtraSerializableLayout, IBarAndDockingControllerClient, ISupportXtraSerializer, IStyleDockZone, ILogicalOwner,
		DevExpress.Utils.MVVM.Services.IDocumentAdapterFactory {
		public static void About() {
			DevExpress.Utils.About.AboutHelper.Show(DevExpress.Utils.About.ProductKind.DXperienceWin, new DevExpress.Utils.About.ProductStringInfoWin(DevExpress.Utils.About.ProductInfoHelper.WinXtraBars));
		}
		AutoHiddenPanelShowMode autoHiddenPanelShowModeCore;
		bool validateFloatFormChildrenOnDeactivateCore = true;
		bool validateDockPanelOnDeactivateCore = true;
		bool validateFormChildrenOnDeactivateCore = true;
		ContainerControl form;
		DockLayoutManager layoutManager;
		BarAndDockingController controller;
		ToolTipController toolTipController;
		bool disposing, loadFired, firstInit;
		int dockModeVS2005FadeSpeedCore = 15000;
		int dockModeVS2005FadeFramesCountCore = 10;
		int initializeCounter, deserializeCounter, lockRaiseRegisterPanelCounter, lockRedrawFormCounter, lockUpdateCounter;
		DockPanelCollection rootPanels, hiddenPanels;
		ReadOnlyPanelCollection panels;
		object images;
		DockingOptions dockingOptions;
		LayoutSerializationOptions serializationOptions;
		DockPanel activePanel;
		IDesignerHost designerHost;
		StringUniqueCollection topZIndexControls;
		string layoutVersion = "";
		static DockManagerCollection dockManagers;
		protected int autoHideSpeedCore = 1;
		DockMode dragVisualizationStyleCore = DockMode.VS2005;
		VS2005StyleDockingVisualizer panelDockZoneVisualizerCore;
		VS2005StyleDockingVisualizer globalDockZoneVisualizerCore;
		static DockManager() {
			dockManagers = new DockManagerCollection();
			DevExpress.Utils.Design.DXAssemblyResolver.Init();
			ComponentLocator.RegisterFindRoutine(FromControl);
		}
		public DockManager() : this((ContainerControl)null) { }
		public DockManager(IContainer container)
			: this((ContainerControl)null) {
			if(container != null) {
				container.Add(this);
			}
		}
		public DockManager(ContainerControl form) {
			DockManagers.RegisterDockManager(this);
			this.layoutManager = CreateLayoutManager();
			this.dockControllerCore = CreateDockController();
			this.rootPanels = new DockPanelCollection();
			this.rootPanels.Changed += new CollectionChangeEventHandler(RootPanels_Changed);
			this.hiddenPanels = new DockPanelCollection();
			this.hiddenPanels.Changed += new CollectionChangeEventHandler(HiddenPanels_Changed);
			this.panels = new ReadOnlyPanelCollection();
			this.topZIndexControls = CreateTopZIndexControls();
			this.controller = null;
			CurrentController.AddClient(this);
			this.disposing = false;
			this.loadFired = false;
			this.firstInit = true;
			this.initializeCounter = this.deserializeCounter = this.lockRaiseRegisterPanelCounter =
				this.lockRedrawFormCounter = this.lockUpdateCounter = 0;
			this.images = null;
			this.designerHost = null;
			this.serializationOptions = CreateSerializationOptions();
			this.dockingOptions = CreateDockingOptions();
			this.dockingOptions.Changed += new BaseOptionChangedEventHandler(DockingOptions_Changed);
			this.activePanel = null;
			this.toolTipController = null;
			Form = form;
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				DockManagers.UnregisterDockManager(this);
				this.disposing = disposing;
				this.rootPanels.Changed -= new CollectionChangeEventHandler(RootPanels_Changed);
				this.hiddenPanels.Changed -= new CollectionChangeEventHandler(HiddenPanels_Changed);
				this.dockingOptions.Changed -= new BaseOptionChangedEventHandler(DockingOptions_Changed);
				HookDesignerHost(false);
				Clear();
				LayoutManager.Dispose();
				Controller = null;
				ToolTipController = null;
				Form = null;
				dockControllerCore = null;
				Docking2010.Ref.Dispose(ref panelDockZoneVisualizerCore);
				Docking2010.Ref.Dispose(ref globalDockZoneVisualizerCore);
				Docking2010.Ref.Dispose(ref sizingAdornerCore);
			}
			base.Dispose(disposing);
		}
		public void CreateVs2005DockStyleVisualizers() {
			panelDockZoneVisualizerCore = CreateVisualizer(VisualizerRole.PanelVisualizer);
			globalDockZoneVisualizerCore = CreateVisualizer(VisualizerRole.RootLayoutVisualizer);
		}
		SizingAdorer sizingAdornerCore;
		protected internal SizingAdorer SizingAdorner {
			get {
				if(Form == null) return null;
				if(sizingAdornerCore == null) {
					if(SupportVS2010DockingStyle())
						sizingAdornerCore = new SizingAdorer(Form);
				}
				return sizingAdornerCore;
			}
		}
		protected internal VS2005StyleDockingVisualizer PanelDockZoneVisualizer {
			get {
				if(panelDockZoneVisualizerCore == null)
					panelDockZoneVisualizerCore = CreateVisualizer(VisualizerRole.PanelVisualizer);
				return panelDockZoneVisualizerCore;
			}
		}
		protected virtual VS2005StyleDockingVisualizer CreateVisualizer(VisualizerRole role) {
			if(SupportVS2010DockingStyle())
				return new VS2010StyleDockZoneVisualizer(
					role, DockModeVS2005FadeSpeed, DockModeVS2005FadeFramesCount, this);
			return new VS2005StyleDockingVisualizer(role, DockModeVS2005FadeSpeed, DockModeVS2005FadeFramesCount);
		}
		protected internal VS2005StyleDockingVisualizer GlobalDockZoneVisualizer {
			get {
				if(globalDockZoneVisualizerCore == null)
					globalDockZoneVisualizerCore = CreateVisualizer(VisualizerRole.RootLayoutVisualizer);
				return globalDockZoneVisualizerCore;
			}
		}
		protected internal bool ContainsHotHint() {
			bool result = false;
			DevExpress.XtraBars.Docking2010.Customization.VS2010StyleDockZoneVisualizer visualizer;
			visualizer = GlobalDockZoneVisualizer as DevExpress.XtraBars.Docking2010.Customization.VS2010StyleDockZoneVisualizer;
			result |= CheckVisualizer(visualizer);
			visualizer = PanelDockZoneVisualizer as DevExpress.XtraBars.Docking2010.Customization.VS2010StyleDockZoneVisualizer;
			result |= CheckVisualizer(visualizer);
			return result;
		}
		bool CheckVisualizer(DevExpress.XtraBars.Docking2010.Customization.VS2010StyleDockZoneVisualizer visualizer) {
			if(visualizer == null) return false;
			return visualizer.IsHot;
		}
		protected virtual DockLayoutManager CreateLayoutManager() { return new DockLayoutManager(this); }
		protected virtual LayoutSerializationOptions CreateSerializationOptions() { return new LayoutSerializationOptions(); }
		protected virtual DockingOptions CreateDockingOptions() { return new DockingOptions(); }
		bool allowCreateAuoHideContainerCore = true;
		internal bool IsCreatingAutohideControl { get { return allowCreateAuoHideContainerCore; } set { allowCreateAuoHideContainerCore = value; } }
		internal DockPanel CreateDockPanel(bool createControlContainer, DockingStyle dock) {
			DockPanel result = null;
			if(!createControlContainer)
				lockRaiseRegisterPanelCounter++;
			try {
				result = CreateDockPanel(dock, createControlContainer);
			}
			finally {
				if(!createControlContainer)
					lockRaiseRegisterPanelCounter--;
			}
			AddToContainer(result, CreateDockPanelName(!createControlContainer));
			result.Text = GetDockPanelText(result);
			if(result.ControlContainer != null)
				AddToContainer(result.ControlContainer, string.Format(DockConsts.ContainerControlFormatString, result.Name));
			return result;
		}
		protected virtual DockPanel CreateDockPanel(DockingStyle dock, bool createControlContainer) {
			return new DockPanel(createControlContainer, dock, this);
		}
		protected virtual string GetDockPanelText(DockPanel panel) {
			if(panel.Name != string.Empty)
				return panel.Name;
			return DockConsts.DefaultDockPanelText;
		}
		string CreateDockPanelName(bool isPanelContainer) {
			if(Container == null || !isPanelContainer) return null;
			int number = 1;
			string result = string.Empty;
			do {
				result = string.Format(DockConsts.DockPanelContainerNameFormatString, number++);
			} while(Container.Components[result] != null);
			return result;
		}
		protected internal static DockManager FromControl(Control control) {
			return DockManagers.FromControl(control);
		}
		protected internal AutoHideContainer InternalCreateAutoHideContainer(DockStyle dock) {
			AutoHideContainer result = CreateAutoHideContainerCore();
			result.Dock = dock;
			result.Manager = LayoutManager;
			AddToContainer(result, string.Format(DockConsts.AutoHideContainerNameFormatString, dock));
			return result;
		}
		protected virtual AutoHideContainer CreateAutoHideContainerCore() { return new AutoHideContainer(); }
		void AddToContainer(Control control, string name) {
			if(Container == null) return;
			Container.Add(control);
			if(name != null && control.Site != null) {
				try {
					control.Site.Name = name;
				}
				catch { }
			}
		}
		internal bool canImmediateRepaint = true; 
		internal Docking2010.DocumentManager documentManagerCore;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Docking2010.DocumentManager DocumentManager {
			get {
				if(documentManagerCore == null)
					InitializeDocumentManager();
				return documentManagerCore;
			}
		}
		void InitializeDocumentManager() {
			if(Disposing) return;
			Docking2010.DocumentManager manager =
				Docking2010.DocumentManager.FromContainer(Container, Form) ??
				Docking2010.DocumentManager.FromControl(Form);
			if(manager != null) {
				if(manager.GetContainer() == Form) {
					documentManagerCore = manager;
					manager.dockManagerCore = this;
					if(!(panelDockZoneVisualizerCore is Docking2010.Customization.VS2010StyleDockZoneVisualizer))
						Docking2010.Ref.Dispose(ref panelDockZoneVisualizerCore);
					if(!(globalDockZoneVisualizerCore is Docking2010.Customization.VS2010StyleDockZoneVisualizer))
						Docking2010.Ref.Dispose(ref globalDockZoneVisualizerCore);
				}
			}
		}
		protected internal void ReleaseDocumentManager() {
			documentManagerCore = null;
			Docking2010.Ref.Dispose(ref panelDockZoneVisualizerCore);
			Docking2010.Ref.Dispose(ref globalDockZoneVisualizerCore);
			Docking2010.Ref.Dispose(ref sizingAdornerCore);
		}
		protected internal bool SupportVS2010DockingStyle() {
			return DocumentManager != null && DocumentManager.IsStrategyValid;
		}
		protected internal virtual FloatForm CreateFloatForm() {
			return new FloatForm()
			{
				RightToLeft = WindowsFormsSettings.GetRightToLeft(Form)
			};
		}
		protected virtual StringUniqueCollection CreateTopZIndexControls() {
			StringUniqueCollection result = new StringUniqueCollection();
			result.Add(typeof(BarDockControl).FullName);
			result.Add(typeof(StandaloneBarDockControl).FullName);
			result.Add(typeof(StatusBar).FullName);
			result.Add(typeof(MenuStrip).FullName);
			result.Add(typeof(StatusStrip).FullName);
			result.Add(typeof(RibbonStatusBar).FullName);
			result.Add(typeof(RibbonControl).FullName);
			result.Add(typeof(Navigation.OfficeNavigationBar).FullName);
			result.Add(typeof(Navigation.TileNavPane).FullName);
			return result;
		}
		internal void RegisterPanel(DockPanel panel) {
			if(Panels.Contains(panel)) return;
			Panels.Add(panel);
			UpdateRootPanels();
			UpdateHiddenPanels();
			RaiseRegisterDockPanel(panel);
		}
		internal void UnregisterPanel(DockPanel panel) {
			Panels.Remove(panel);
			if(Disposing) return;
			UpdateRootPanels();
			UpdateHiddenPanels();
			if(panel == ActivePanel)
				ActivePanel = null;
			RaiseUnregisterDockPanel(panel);
		}
		internal AppearanceObject GetMaxHeightHideBarAppearance(TabsPosition position) {
			FrozenAppearance hideBarButtonActive = new FrozenAppearance();
			FrozenAppearance hideBarButton = new FrozenAppearance();
			CurrentController.PaintStyle.ElementsParameters.InitHidePanelButtonActiveAppearance(hideBarButtonActive, CurrentController.AppearancesDocking.HidePanelButtonActive, position);
			CurrentController.PaintStyle.ElementsParameters.InitHidePanelButtonActiveAppearance(hideBarButton, CurrentController.AppearancesDocking.HidePanelButton, position);
			AppearanceObject maxHeightAppearance = Painter.GetMaxHeightAppearance(hideBarButtonActive, hideBarButton);
			if(maxHeightAppearance == hideBarButtonActive) {
				hideBarButton.Dispose();
				return hideBarButtonActive;
			}
			hideBarButtonActive.Dispose();
			return hideBarButton;
		}
		protected internal bool IsDesignMode { get { return DesignMode || ((Form != null && Form.Site != null) ? Form.Site.DesignMode : false); } }
		internal void SetDesignTimeActiveChild(DockPanel container, DockPanel child, bool canFireChanged) {
			if(!DesignMode || !child.CanActivate) return;
			container.ActiveChild = child;
			if(canFireChanged && container.ActiveChild == child)
				FireChanged();
		}
		protected internal void FireChanged() {
			if(!DesignMode || IsDeserializing) return;
			IComponentChangeService srv = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(srv != null) {
				srv.OnComponentChanged(this, null, null, null);
			}
		}
		void IncreaseInitializeCounter() { this.initializeCounter++; }
		void DecreaseInitializeCounter() { this.initializeCounter--; }
		public virtual void BeginInit() {
			IncreaseInitializeCounter();
			if(FirstInit) {
				TopZIndexControls.Clear();
				this.firstInit = false;
			}
		}
		protected void DockAsTabbedDocument() {
			foreach(DockPanel panel in RootPanels) {
				if(panel.DockedAsTabbedDocument && !panel.IsMdiDocument)
					DocumentManager.View.Controller.Dock(panel);
			}
		}
		public virtual void EndInit() {
			DecreaseInitializeCounter();
			HookDesignerHost(true);
		}
		public virtual void BeginUpdate() {
			this.lockUpdateCounter++;
			LayoutManager.BeginUpdate();
			BeginFormLayoutUpdate();
		}
		public virtual void EndUpdate() {
			this.lockUpdateCounter--;
			LayoutManager.EndUpdate();
			EndFormLayoutUpdate();
			UpdateFloatFormsVisibility();
		}
		protected bool IsLayoutLocked { get { return this.lockUpdateCounter != 0; } }
		protected internal virtual void MakeFloatFormVisible(FloatForm floatForm) {
			if(IsLayoutLocked) return;
			CheckFloatFormLocation(floatForm);
			if(floatForm != null && floatForm.FloatLayout != null && floatForm.FloatLayout.Panel != null && floatForm.FloatLayout.Panel.MouseHandler != null && floatForm.FloatLayout.Panel.MouseHandler.State != DockPanelState.Regular) {
				DockPanel activeChild = floatForm.FloatLayout.Panel.ActiveChild;
				if(activeChild != null && activeChild.MouseHandler != null)
					activeChild.MouseHandler.BeginLockFocus();
				floatForm.ActiveControl = floatForm.FloatLayout.Panel.ActiveControl;
				floatForm.Show();
				if(activeChild != null && activeChild.MouseHandler != null)
					activeChild.MouseHandler.EndLockFocus();
			}
			else {
#if DEBUGTEST
				floatForm.Show();
#else
				if(Form != null && Form.IsHandleCreated && !IsDeserializing && !IsDesignMode) Form.BeginInvoke(new ShowFormInvoker(InvokeShowForm), new object[] { floatForm });
				else floatForm.Show();
#endif
			}
		}
		bool NeedPanelToMoveHorizontally(Rectangle intersect, Rectangle virtualScreen, FloatForm panel) {
			if(intersect.IsEmpty) return !(virtualScreen.X <= panel.Bounds.X && panel.Bounds.Right <= virtualScreen.Right);
			return intersect.Width < panel.Width;
		}
		bool NeedPanelToMoveVertically(Rectangle intersect, Rectangle virtualScreen, FloatForm panel) {
			if(intersect.IsEmpty) return !(virtualScreen.Y <= panel.Bounds.Y && panel.Bounds.Bottom <= virtualScreen.Bottom);
			return intersect.Height < panel.Height;
		}
		protected void CheckFloatFormLocation(FloatForm floatForm) {
			Rectangle formBounds = floatForm.Bounds;
			Rectangle screenWorkingArea = GetScreenWorkingArea(formBounds);
			Rectangle virtualScreen = GetVirtualScreen(formBounds);
			Rectangle Intersect = Rectangle.Intersect(virtualScreen, formBounds);
			if(IsDeserializing && XtraSerializableScreenConfiguration != null) {
				bool hasConflicts = System.Linq.Enumerable.Any(XtraSerializableScreenConfiguration, r => r.IntersectsWith(formBounds));
				if(!hasConflicts)
					return;
			}
			if(NeedPanelToMoveHorizontally(Intersect, virtualScreen, floatForm)) {
				if(floatForm.Left < virtualScreen.X) floatForm.Location = new Point(screenWorkingArea.X, floatForm.Location.Y);
				else if(floatForm.Right > virtualScreen.X) floatForm.Location = new Point(screenWorkingArea.Right - floatForm.Width, floatForm.Location.Y);
			}
			if(NeedPanelToMoveVertically(Intersect, virtualScreen, floatForm)) {
				if(floatForm.Top < virtualScreen.Y) floatForm.Location = new Point(floatForm.Location.X, screenWorkingArea.Y);
				else if(floatForm.Top > virtualScreen.Y) floatForm.Location = new Point(floatForm.Location.X, screenWorkingArea.Bottom - floatForm.Height);
			}
		}
		protected virtual Rectangle GetScreenWorkingArea(Rectangle screenRect) {
			if(SystemInformation.MonitorCount > 1)
				return Screen.FromRectangle(screenRect).WorkingArea;
			return SystemInformation.WorkingArea;
		}
		protected virtual Rectangle GetVirtualScreen(Rectangle screenRect) {
			if(SystemInformation.MonitorCount > 1)
				return Screen.FromRectangle(screenRect).Bounds;
			return SystemInformation.VirtualScreen;
		}
		delegate void ShowFormInvoker(Form control);
		void InvokeShowForm(Form control) {
			FloatForm form = control as FloatForm;
			if(form != null && form.FloatLayout.Panel.IsMdiDocument) return;
			if(!control.IsDisposed && !control.Visible) control.Show();
		}
		protected virtual void UpdateFloatFormsVisibility() {
			if(IsLayoutLocked) return;
			RootPanels.UpdateFloatFormsVisibility();
		}
		public virtual void ForceInitialize() {
			OnLoaded();
		}
		protected virtual void HookDesignerHost(bool addHandler) {
			if(!DesignMode) return;
			if(addHandler && this.designerHost != null) return;
			this.designerHost = GetService(typeof(IDesignerHost)) as IDesignerHost;
			if(designerHost != null) {
				if(addHandler) {
					this.designerHost.TransactionClosed += new DesignerTransactionCloseEventHandler(OnDesignerHost_TransactionClosed);
					this.designerHost.LoadComplete += new EventHandler(OnDesignerHost_LoadComplete);
					this.designerHost.Activated += new EventHandler(OnDesignerHost_Activated);
					this.designerHost.Deactivated += new EventHandler(OnDesignerHost_Deactivated);
				}
				else {
					this.designerHost.TransactionClosed -= new DesignerTransactionCloseEventHandler(OnDesignerHost_TransactionClosed);
					this.designerHost.LoadComplete -= new EventHandler(OnDesignerHost_LoadComplete);
					this.designerHost.Activated -= new EventHandler(OnDesignerHost_Activated);
					this.designerHost.Deactivated -= new EventHandler(OnDesignerHost_Deactivated);
				}
			}
		}
		protected virtual void OnDesignerHost_TransactionClosed(object sender, DesignerTransactionCloseEventArgs e) {
			if(!IsDesignerHostLoading && !this.designerHost.InTransaction)
				OnLoaded();
		}
		protected virtual void OnDesignerHost_LoadComplete(object sender, EventArgs e) {
			if(Disposing) return;
			OnLoaded();
		}
		void OnDesignerHost_Activated(object sender, EventArgs e) {
			ShowFloatForms(true);
		}
		void OnDesignerHost_Deactivated(object sender, EventArgs e) {
			ShowFloatForms(false);
		}
		void ShowFloatForms(bool value) {
			if(!IsInitialized || !DesignMode || Disposing) return;
			for(int i = 0; i < RootPanels.Count; i++) {
				if(RootPanels[i].FloatForm != null)
					RootPanels[i].FloatForm.Visible = value;
			}
		}
		internal void FocusDockFillControl() {
			foreach(Control ctrl in Form.Controls) {
				if(ctrl.Dock == DockStyle.Fill) {
					ctrl.Select();
					if(ctrl.ContainsFocus) return;
				}
			}
			ResetFormActiveControl(OwnerForm.ActiveControl);
		}
		internal void ResetFormActiveControl(Control ctrl) {
			Form ownerForm = OwnerForm;
			DevExpress.XtraEditors.Container.ContainerHelper.UpdateActiveControl(ctrl, ownerForm);
			DevExpress.XtraEditors.Container.ContainerHelper.UpdateUnvalidatedControl(ctrl, ownerForm, null);
		}
		internal void ResetFocusedControl() {
			DevExpress.XtraEditors.Container.ContainerHelper.ClearUnvalidatedControl(Form.ActiveControl, Form);
			DevExpress.XtraEditors.Container.ContainerHelper.UpdateActiveControl(Form.ActiveControl, Form);
			Form.ActiveControl = null;
		}
		protected virtual void BeforeSerialize(IDesignerSerializationManager manager) {
			LayoutManager.BeforeSerialize(manager);
		}
		bool suspendOnLoad = false;
		internal void SuspendOnLoad() {
			this.suspendOnLoad = true;
		}
		internal void ResumeOnLoad() {
			this.suspendOnLoad = false;
			OnLoaded();
		}
		bool isNotificationsEnabled;
		bool isFormLayoutSuspended = false;
		protected internal virtual void SafeOnLoaded() {
			if(designerHost != null && designerHost.Loading) return;
			if(IsInitialized || this.suspendOnLoad) return;
			isNotificationsEnabled = DevExpress.XtraBars.Utils.BarManagerDockingHelper.EnableComponentNotifications(false, Form);
			if(Form != null && !IsDesignMode) {
				Form.SuspendLayout();
				isFormLayoutSuspended = true;
			}
			DeserializeLayoutManager();
			loadFired = true;
		}
		protected internal virtual void UnsafeOnLoaded() {
			LayoutChanged();
			RaiseLoad();
			DevExpress.XtraBars.Utils.BarManagerDockingHelper.ResumeComponentNotifications(isNotificationsEnabled, Form);
			if(Form != null && !IsDesignMode) {
				UpdateControlsZOrder(); 
				Form.ResumeLayout();
				isFormLayoutSuspended = false;
			}
		}
		protected internal virtual void OnLoaded() {
			SafeOnLoaded();
			UnsafeOnLoaded();
		}
		protected void LockDeserialize() { deserializeCounter++; }
		protected void UnlockDeserialize() { deserializeCounter--; }
		[Browsable(false)]
		public bool IsDeserializing { get { return deserializeCounter != 0; } }
		protected virtual void DeserializeLayoutManager() {
			if(IsDeserializing) return;
			LockDeserialize();
			try {
				LayoutManager.Deserialize();
			}
			finally {
				UnlockDeserialize();
			}
			DockAsTabbedDocument();
			CheckActivePanel();
			EnsureChildrenCreated();
		}
		void EnsureChildrenCreated() {
			foreach(DockPanel panel in Panels)
				panel.EnsureChildrenCreated();
		}
		protected virtual void CheckActivePanel() {
			if(activePanelID != LayoutConsts.InvalidIndex)
				ActivePanel = Panels[activePanelID];
			this.activePanelID = LayoutConsts.InvalidIndex;
		}
		void RootPanels_Changed(object sender, CollectionChangeEventArgs e) {
			if(!IsInitialized) return;
		}
		void HiddenPanels_Changed(object sender, CollectionChangeEventArgs e) {
		}
		void DockingOptions_Changed(object sender, BaseOptionChangedEventArgs e) {
			LayoutManager.OnDockingOptionsChanged(e);
		}
		[Browsable(false)]
		public int Count { get { return Panels.Count; } }
#if !SL
	[DevExpressXtraBarsLocalizedDescription("DockManagerItem")]
#endif
		public DockPanel this[int index] { get { return Panels[index]; } }
#if !SL
	[DevExpressXtraBarsLocalizedDescription("DockManagerItem")]
#endif
		public DockPanel this[string name] { get { return Panels[name]; } }
		public DockPanel AddPanel(DockingStyle dock) {
			Point startLocation = Point.Empty;
			if(Form != null)
				startLocation = Screen.GetWorkingArea(Form).Location;
			else startLocation = Screen.PrimaryScreen.WorkingArea.Location;
			return AddPanelCore(dock, startLocation);
		}
		public DockPanel AddPanel(Point floatLocation) {
			return AddPanelCore(DockingStyle.Float, floatLocation);
		}
		protected virtual DockPanel AddPanelCore(DockingStyle dock, Point floatLocation) {
			if(Form == null) {
				if(DesignMode)
					MessageBox.Show(DockConsts.DockManagerFormIsNullMessageString, DockConsts.DockManagerFormIsNullCaptionString,
						MessageBoxButtons.OK, MessageBoxIcon.Error);
				return null;
			}
			if(Form.Handle == IntPtr.Zero) Form.CreateControl();
			DockLayout dockLayout = LayoutManager.AddLayout(dock, floatLocation) as DockLayout;
			return (dockLayout == null ? null : dockLayout.Panel);
		}
		public void AddPanel(DockingStyle dock, DockPanel panel) {
			if(panel == null) return;
			panel.Register(this);
			panel.DockTo(dock);
		}
		public void RemovePanel(DockPanel panel) {
			if(panel != null)
				panel.Dispose();
		}
		public void Clear() {
			int watchDog = 1000;
			while(HiddenPanels.Count > 0 && watchDog > 0) { HiddenPanels[0].Dispose(); watchDog--; }
			watchDog = 1000;
			while(AutoHideContainers.Count > 0 && watchDog > 0) { AutoHideContainers[0].Dispose(); watchDog--; };
			RootPanels.Clear();
		}
		public DockPanel GetDockPanelAtPos(Point ptScreen) {
			return LayoutManager.GetDockPanelAtPos(ptScreen);
		}
		internal DockPanel GetDockPanelAtPosBut(Point ptScreen, DockPanel dpanel) {
			return LayoutManager.GetDockPanelAtPosBut(ptScreen, dpanel);
		}
		protected internal virtual void UpdateRootPanels() {
			if(IsDeserializing) return;
			RootPanels.Update(LayoutManager.GetRootPanelsArray());
		}
		protected internal virtual void UpdateHiddenPanels() {
			if(IsDeserializing) return;
			HiddenPanels.Update(LayoutManager.GetHiddenPanelsArray());
		}
		Control refreshControl = null;
		protected Form MdiParent {
			get {
				Form frm = Form as Form;
				if(frm != null) {
					if(frm.IsMdiContainer) return frm;
					return frm.MdiParent;
				}
				return null;
			}
		}
		internal void OnStartDock(DockPanel source, Control dest, DockingStyle dock) {
			refreshControl = dest.Parent == null ? dest : dest.Parent;
			if(source.FloatForm != null) {
				Form activeMdiChild = null;
				if(MdiParent != null)
					activeMdiChild = MdiParent.ActiveMdiChild;
				if(!source.IsDockingInitialization)
					source.FloatForm.Visible = false;
				if(activeMdiChild != null)
					activeMdiChild.BringToFront();
			}
			SuspendRedraw(refreshControl);
		}
		internal void OnEndDock() {
			ResumeRedraw(refreshControl);
			refreshControl = null;
		}
		protected internal void SuspendRedrawForm() {
			if(Form == null || Form.Visible == false) return;
			if(lockRedrawFormCounter++ == 0)
				SuspendRedraw(Form);
		}
		protected internal void ResumeRedrawForm() {
			if(lockRedrawFormCounter == 0) return;
			if(--lockRedrawFormCounter != 0) return;
			if(Disposing) return;
			ResumeRedraw(Form);
			if(Form != null && Form.IsHandleCreated) {
				DevExpress.Skins.XtraForm.FormClientPainter.InvalidateNC(Form);
			}
		}
		protected virtual bool AllowSyspendRedraw {
			get {
				if(Form != null) {
					Form formCore = this.Form as System.Windows.Forms.Form;
					if(formCore != null) {
						return !formCore.AllowTransparency;
					}
				}
				return true;
			}
		}
		bool useLockWindowUpdate = false;
		protected internal bool UseLockWindowUpdate {
			get { return useLockWindowUpdate; }
			set { useLockWindowUpdate = value; }
		}
		protected virtual void SuspendRedraw(Control control) {
			if(control == null || !IsInitialized) return;
			if(!AllowSyspendRedraw) return;
			SetRedrawNew(control, false);
		}
		protected virtual void ResumeRedraw(Control control) {
			if(control == null || !IsInitialized) return;
			if(!AllowSyspendRedraw) return;
			SetRedrawNew(control, true);
			if(control.IsHandleCreated) control.Refresh();
		}
		protected virtual void SetRedrawNew(Control control, bool value) {
			PaintingSuspended = !value;
		}
		private bool paintSuspended = false;
		internal bool PaintingSuspended {
			get { return paintSuspended; }
			set { paintSuspended = value; }
		}
		bool windowUpdateLocked = false;
		protected void LockWindowUpdate(Control control, bool shouldLock) {
			if(shouldLock && windowUpdateLocked) BarNativeMethods.LockWindowUpdate(IntPtr.Zero);
			if(!shouldLock && !windowUpdateLocked) return;
			int result = BarNativeMethods.LockWindowUpdate(shouldLock ? control.Handle : IntPtr.Zero);
			windowUpdateLocked = (result != 0 && shouldLock);
		}
		void SetRedraw(Control control, bool value) {
			if(control == null || control.IsDisposed || Form == null || Form.Disposing || !control.IsHandleCreated || !control.Visible) return;
			if(useLockWindowUpdate) {
				LockWindowUpdate(control, !value);
				return;
			}
			const int WM_SETREDRAW = 0x000B;
			BarNativeMethods.SendMessage(control.Handle, WM_SETREDRAW, (uint)(value ? 1 : 0), 0);
		}
		protected internal DockLayoutManager LayoutManager { get { return layoutManager; } }
		protected internal bool Disposing { get { return disposing; } }
		string IXtraSerializableLayout.LayoutVersion {
			get { return this.LayoutVersion; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockManagerLayoutVersion"),
#endif
 DefaultValue(""), Category("Data")]
		public virtual string LayoutVersion {
			get { return layoutVersion; }
			set {
				if(value == null) value = "";
				layoutVersion = value;
			}
		}
		[ DefaultValue(AutoHiddenPanelShowMode.Default)]
		public virtual AutoHiddenPanelShowMode AutoHiddenPanelShowMode {
			get { return autoHiddenPanelShowModeCore; }
			set {
				if(autoHiddenPanelShowModeCore == value) return;
				autoHiddenPanelShowModeCore = value;
				layoutManager.AutoHideMoveHelper.AutoHiddenPanelShow(AutoHiddenPanelShowMode);
			}
		}
		protected internal bool CanAutoHideByClick {
			get { return AutoHiddenPanelShowMode == AutoHiddenPanelShowMode.MouseClick; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockManagerAutoHideSpeed"),
#endif
 DefaultValue(1)]
		public virtual int AutoHideSpeed {
			get { return autoHideSpeedCore; }
			set {
				if(value < 1) value = 1;
				autoHideSpeedCore = value;
			}
		}
		bool allowGlyphSkinning;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockManagerAllowGlyphSkinning"),
#endif
 DefaultValue(false),
		Category(DockConsts.AppearanceCategory), XtraSerializableProperty]
		public bool AllowGlyphSkinning {
			get { return allowGlyphSkinning; }
			set {
				if(allowGlyphSkinning == value)
					return;
				allowGlyphSkinning = value;
				if(IsInitialized) LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockManagerForm"),
#endif
 DefaultValue(null), Category(DockConsts.LayoutCategory)]
		public ContainerControl Form {
			get { return form; }
			set {
				if(Form == value) return;
				Docking2010.Ref.Dispose(ref sizingAdornerCore);
				if(value != null) {
					DockPanel panel = value as DockPanel;
					if(panel != null) {
						if(panel.DockManager == this)
							return;
					}
					else {
						if(ProhibitUsingAsDockingContainerAttribute.IsDefined(value))
							return;
					}
				}
				if(IsDesignMode && IsInitialized && !Disposing) {
					string caption = DockManagerLocalizer.Active.GetLocalizedString(DockManagerStringId.MessageFormPropertyChangedCaption);
					string text = DockManagerLocalizer.Active.GetLocalizedString(DockManagerStringId.MessageFormPropertyChangedText);
					DialogResult res = XtraMessageBox.Show(this.LookAndFeel, text, caption, MessageBoxButtons.OKCancel);
					if(DialogResult.Cancel == res)
						return;
				}
				if(Form != null)
					UnsubscribeFormEvents();
				this.form = value;
				if(!IsInitializing) this.loadFired = true;
				if(IsInitialized) Clear();
				if(Form != null) {
					SubscribeFormEvents();
					HookDesignerHost(true);
				}
				if(IsInitialized) LayoutChanged();
			}
		}
		void SubscribeFormEvents() {
			Form.ParentChanged += new EventHandler(FormParentChanged);
			Form.SizeChanged += new EventHandler(FormSizeChanged);
			Form.ControlAdded += new ControlEventHandler(Form_ControlAdded);
			Form.ControlRemoved += new ControlEventHandler(Form_ControlRemoved);
			Form.Disposed += new EventHandler(FormDisposed);
			Form.HandleCreated += new EventHandler(FormHandleCreated);
			Form_Activated += new EventHandler(FormActivated);
			Form_Deactivate += new EventHandler(FormDeactivate);
			Form.Layout += new LayoutEventHandler(Form_Layout);
			Form.RightToLeftChanged += Form_RightToLeftChanged;
		}
		void UnsubscribeFormEvents() {
			Form.RightToLeftChanged -= Form_RightToLeftChanged;
			Form.ParentChanged -= new EventHandler(FormParentChanged);
			Form.SizeChanged -= new EventHandler(FormSizeChanged);
			Form.ControlAdded -= new ControlEventHandler(Form_ControlAdded);
			Form.ControlRemoved -= new ControlEventHandler(Form_ControlRemoved);
			Form.Disposed -= new EventHandler(FormDisposed);
			Form.HandleCreated -= new EventHandler(FormHandleCreated);
			Form_Activated -= new EventHandler(FormActivated);
			Form_Deactivate -= new EventHandler(FormDeactivate);
			Form.Layout -= new LayoutEventHandler(Form_Layout);
		}
		void Form_RightToLeftChanged(object sender, EventArgs e) {
			if(sizingAdornerCore != null)
				sizingAdornerCore.ResetIsRightToLeft();
			if(panelDockZoneVisualizerCore != null)
				Docking2010.Ref.Dispose(ref panelDockZoneVisualizerCore);
			if(globalDockZoneVisualizerCore != null)
				Docking2010.Ref.Dispose(ref globalDockZoneVisualizerCore);
		}
		void Form_Layout(object sender, LayoutEventArgs e) {
			if(Form != null && !Form.IsHandleCreated) return;
			if(!IsInitialized || isFormLayoutSuspended) return;
			if(OwnerForm != null && OwnerForm.WindowState == FormWindowState.Minimized) return;
			if(e.AffectedProperty == "Bounds" && ContainsTopZIndexControl(e.AffectedComponent as Control))
				LayoutChanged();
		}
		void Form_ControlRemoved(object sender, ControlEventArgs e) {
			if(!(e.Control is DockPanel))
				documentManagerCore = null;
			if(ContainsTopZIndexControl(e.Control)) needUpdateControlsZOrder = true;
		}
		internal bool needUpdateControlsZOrder = false;
		void Form_ControlAdded(object sender, ControlEventArgs e) {
			if(!(e.Control is DockPanel))
				documentManagerCore = null;
			if(ContainsTopZIndexControl(e.Control)) needUpdateControlsZOrder = true;
		}
		protected internal Form OwnerForm {
			get {
				if(Form == null) return null;
				return Form.FindForm();
			}
		}
		internal bool IsOwnerFormActive {
			get {
				if(OwnerForm != null)
					return OwnerForm == System.Windows.Forms.Form.ActiveForm;
				return true;
			}
		}
		internal bool IsOwnerFormMDIContainer {
			get {
				if(OwnerForm == null) return false;
				return OwnerForm.IsMdiContainer;
			}
		}
		internal bool IsOwnerFormMDIChild {
			get {
				if(OwnerForm == null) return false;
				return OwnerForm.IsMdiChild && OwnerForm.MdiParent != null;
			}
		}
		protected internal Control FindFocusedControl() {
			Form ownerForm = OwnerForm;
			if(ownerForm == null) return null;
			foreach(Control control in ownerForm.Controls) {
				if(control.ContainsFocus) return control;
			}
			return null;
		}
		event EventHandler Form_Activated {
			add {
				if(Form is Form) ((Form)Form).Activated += value;
				else Form.Enter += value;
			}
			remove {
				if(Form is Form) ((Form)Form).Activated -= value;
				else Form.Enter -= value;
			}
		}
		event EventHandler Form_Deactivate {
			add {
				if(Form is Form) ((Form)Form).Deactivate += value;
				else Form.Leave += value;
			}
			remove {
				if(Form is Form) ((Form)Form).Deactivate -= value;
				else Form.Leave -= value;
			}
		}
		protected internal bool FormContainsFocus {
			get {
				Form tempForm = Form as Form;
				if(tempForm != null) {
					if(!tempForm.IsMdiChild) return (System.Windows.Forms.Form.ActiveForm == Form);
					else {
						return tempForm.MdiParent.ActiveMdiChild == tempForm;
					}
				}
				else
					return Form.ContainsFocus;
			}
		}
		void FormParentChanged(object sender, EventArgs e) {
			documentManagerCore = null;
			HookDesignerHost(true);
		}
		void FormSizeChanged(object sender, EventArgs e) {
			if(Form != null && !Form.IsHandleCreated) return;
			if(OwnerForm != null && OwnerForm.WindowState == FormWindowState.Minimized) return;
			if(!IsInitialized) return;
			if(isFormLayoutSuspended) return;
			LayoutManager.FormSizeChanged();
		}
		void FormDisposed(object sender, EventArgs e) {
			documentManagerCore = null;
			Dispose();
		}
		void FormHandleCreated(object sender, EventArgs e) {
			if(IsDesignMode && !IsInitialized) {
				if(designerHost != null && designerHost.Loading || this.suspendOnLoad) return;
				SafeOnLoaded();
				UnsafeOnLoaded();
				return;
			}
			Form.BeginInvoke(new MethodInvoker(UnsafeOnLoaded));
			SafeOnLoaded();
		}
		protected virtual DockPanel GetDockPanelByFocusedControl() {
			foreach(DockPanel dp in Panels) if(dp.ContainsFocus) return dp;
			return null;
		}
		void FormActivated(object sender, EventArgs e) {
			CheckFocusedPanel();
		}
		protected internal bool wasDeactivated = false;
		void FormDeactivate(object sender, EventArgs e) {
			if(ActivePanel != null) {
				wasDeactivated = true;
				ActivePanel.DockLayout.RefreshCaption();
				wasDeactivated = false;
			}
		}
		protected internal void InvokeCheckFocusedPanel() {
			if(Form != null && Form.IsHandleCreated)
				Form.BeginInvoke(new MethodInvoker(CheckFocusedPanel));
		}
		void CheckFocusedPanel() {
			if(ActivePanel == null) return;
			DockPanel focusCandidate = null;
			if(IsFloatPanel(ActivePanel))
				focusCandidate = GetDockPanelByFocusedControl();
			ActivePanel = focusCandidate ?? ActivePanel;
			ActivePanel.DockLayout.RefreshCaption();
		}
		bool IsFloatPanel(DockPanel panel) {
			if(panel.Dock == DockingStyle.Float) return true;
			DockPanel parentPanel = panel.ParentPanel;
			while(parentPanel != null) {
				if(parentPanel.Dock == DockingStyle.Float)
					return true;
				parentPanel = parentPanel.ParentPanel;
			}
			return false;
		}
		AutoHiddenPanelCaptionShowMode ahCaptionShowModeCore = AutoHiddenPanelCaptionShowMode.ShowForAllPanels;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockManagerImages"),
#endif
 DefaultValue(AutoHiddenPanelCaptionShowMode.ShowForAllPanels),
		Category(DockConsts.AppearanceCategory), XtraSerializableProperty]
		public AutoHiddenPanelCaptionShowMode AutoHiddenPanelCaptionShowMode {
			get { return ahCaptionShowModeCore; }
			set {
				if(ahCaptionShowModeCore == value) return;
				ahCaptionShowModeCore = value;
				LayoutChanged();
				FireChanged();
			}
		}
		protected internal virtual bool CanCollapseAutoHideCaptions {
			get { return AutoHiddenPanelCaptionShowMode == AutoHiddenPanelCaptionShowMode.ShowForActivePanel; }
		}
		protected internal virtual bool CanPerformAutoHideMoving {
			get {
				if(DesignMode) return IsInitialized;
				if(!IsInitialized) return false;
				Form activeForm = System.Windows.Forms.Form.ActiveForm;
				if(activeForm == null || OwnerForm == null) return false; 
				if(activeForm == OwnerForm) return true;
				if(activeForm == OwnerForm.MdiParent ||
					activeForm == OwnerForm.ParentForm) return true;
				return (activeForm.Owner == Form);
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockManagerImages"),
#endif
 DefaultValue(null), Category(DockConsts.AppearanceCategory), TypeConverter(typeof(DevExpress.Utils.Design.ImageCollectionImagesConverter))]
		public object Images {
			get { return images; }
			set {
				if(Images == value) return;
				images = value;
				ViewChanged();
			}
		}
		protected internal bool IsInitializing { get { return (initializeCounter != 0); } }
#if DEBUGTEST
		[Browsable(false)]
		public virtual bool IsInitialized {
			get {
				if(IsInitializing) return false;
				return loadFired;
			}
		}
#else
		[Browsable(false)]
		public bool IsInitialized {
			get {
				if(IsInitializing) return false;
				return loadFired;
			}
		}
#endif
		protected internal BarAndDockingController CurrentController { get { return (Controller == null || Controller.Disposing ? BarAndDockingController.Default : Controller); } }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockManagerController"),
#endif
 DefaultValue(null), Category(DockConsts.AppearanceCategory)]
		public BarAndDockingController Controller {
			get { return controller; }
			set {
				if(GetBarAndDockingController() == value) return;
				GetBarAndDockingController().RemoveClient(this);
				this.controller = value;
				if(!Disposing) {
					GetBarAndDockingController().AddClient(this);
					LayoutChanged();
				}
			}
		}
		BarAndDockingController GetBarAndDockingController() {
			return controller != null ? controller : BarAndDockingController.Default;
		}
		void IBarAndDockingControllerClient.OnDisposed(BarAndDockingController controller) {
			if(Disposing) return;
			Controller = null;
		}
		void IBarAndDockingControllerClient.OnControllerChanged(BarAndDockingController controller) {
			OnControllerChanged(controller);
		}
		protected virtual void OnControllerChanged(object sender) {
			if(Disposing) return;
			ViewChanged();
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockManagerValidateFloatFormChildrenOnDeactivate"),
#endif
 DefaultValue(true)]
		public virtual bool ValidateFloatFormChildrenOnDeactivate {
			get { return validateFloatFormChildrenOnDeactivateCore; }
			set { validateFloatFormChildrenOnDeactivateCore = value; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockManagerValidateDockPanelOnDeactivate"),
#endif
 DefaultValue(true)]
		public virtual bool ValidateDockPanelOnDeactivate {
			get { return validateDockPanelOnDeactivateCore; }
			set { validateDockPanelOnDeactivateCore = value; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockManagerValidateFormChildrenOnDeactivate"),
#endif
 DefaultValue(true)]
		public virtual bool ValidateFormChildrenOnDeactivate {
			get { return validateFormChildrenOnDeactivateCore; }
			set { validateFormChildrenOnDeactivateCore = value; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockManagerToolTipController"),
#endif
 DefaultValue(null), Category(DockConsts.AppearanceCategory)]
		public virtual ToolTipController ToolTipController {
			get { return toolTipController; }
			set {
				if(ToolTipController == value) return;
				if(ToolTipController != null) ToolTipController.Disposed -= new EventHandler(ToolTipControllerDisposed);
				toolTipController = value;
				if(ToolTipController != null) ToolTipController.Disposed += new EventHandler(ToolTipControllerDisposed);
			}
		}
		void ToolTipControllerDisposed(object sender, EventArgs e) {
			ToolTipController = null;
		}
		void ViewChanged() {
			LayoutManager.ViewChanged();
			LayoutChanged();
		}
		int GetTopZIndexControlCount() {
			int result = 0;
			foreach(Control control in Form.Controls) {
				if(ContainsTopZIndexControl(control))
					result++;
			}
			return result;
		}
		bool ContainsTopZIndexControl(Control control) {
			if(control == null) return false;
			return TopZIndexControls.Contains(control.GetType().FullName);
		}
		protected internal virtual void HideSliding(DockPanel panel) {
			if(!IsInitialized || panel.RootPanel.ParentAutoHideControl == null) return;
			if(ActivePanel != null && (panel == ActivePanel || panel.HasAsParent(ActivePanel)))
				ActivePanel = null;
			LayoutManager.HideSliding();
		}
		bool IsDesignerHostLoading { get { return (designerHost != null && designerHost.Loading); } }
		protected internal virtual void UpdateControlsZOrder() {
			if(IsDesignerHostLoading) return;
			BeginFormLayoutUpdate();
			try {
				int topZIndexControlCount = GetTopZIndexControlCount();
				DockLayoutUtils.UpdateRootDockPanelsZOrder(LayoutManager, Form.Controls.Count, AutoHideContainers.Count + topZIndexControlCount);
				for(int i = 0; i < AutoHideContainers.Count; i++) DockLayoutUtils.UpdateAutoHideContainersZOrder(AutoHideContainers, Form.Controls.Count - 1 - topZIndexControlCount);
				int counter = 0;
				for(int i = 0; i < Form.Controls.Count; i++) {
					Control layoutControl = Form.Controls[i];
					if(layoutControl is ZIndexControl) continue;
					if(ContainsTopZIndexControl(layoutControl)) continue;
					Form.Controls.SetChildIndex(layoutControl, counter++);
				}
				for(int i = 0; i < AutoHideContainers.Count; i++) DockLayoutUtils.UpdateAutoHideContainersZOrder(AutoHideContainers, Form.Controls.Count - 1 - topZIndexControlCount);
			}
			finally {
				EndFormLayoutUpdate();
			}
		}
		internal void BeginFormLayoutUpdate() {
			SuspendRedrawForm();
			Form.SuspendLayout();
		}
		internal void EndFormLayoutUpdate() {
			Form.ResumeLayout();
			ResumeRedrawForm();
		}
		protected internal Rectangle GetScreenClientBounds() {
			var clientBounds = GetClientBoundsCore(Form.ClientRectangle, ContainsTopZIndexControl);
			return new Rectangle(Form.PointToScreen(clientBounds.Location), clientBounds.Size);
		}
		protected internal Rectangle GetScreenDockingBounds() {
			var dockingBounds = GetClientBoundsCore(Form.ClientRectangle, IsSideControl);
			return new Rectangle(Form.PointToScreen(dockingBounds.Location), dockingBounds.Size);
		}
		protected internal Rectangle GetClientBounds() {
			return GetClientBoundsCore(Form.ClientRectangle, ContainsTopZIndexControl);
		}
		protected internal Rectangle GetDockingBounds() {
			return GetClientBoundsCore(Form.ClientRectangle, IsSideControl);
		}
		protected internal Rectangle GetClientBounds(Rectangle clientBounds) {
			return GetClientBoundsCore(clientBounds, ContainsTopZIndexControl);
		}
		protected internal Rectangle GetDockingBounds(Rectangle clientBounds) {
			return GetClientBoundsCore(clientBounds, IsSideControl);
		}
		Rectangle GetClientBoundsCore(Rectangle clientBounds, Predicate<Control> filter) {
			foreach(Control control in Form.Controls) {
				if(!filter(control) || !control.Visible) continue; 
				DockingStyle dock = (DockingStyle)control.Dock;
				if(!LayoutRectangle.GetIsVertical(dock) && !LayoutRectangle.GetIsHorizontal(dock)) continue;
				clientBounds = (new LayoutRectangle(clientBounds, dock)).RemoveSize(control.Size);
			}
			return clientBounds;
		}
		protected bool IsSideControl(Control ctrl) {
			return ctrl is AutoHideContainer || ContainsTopZIndexControl(ctrl);
		}
		#region IXtraSerializable
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void SaveToXml(string xmlFile) {
			SaveLayoutToXml(xmlFile);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RestoreFromXml(string xmlFile) {
			RestoreLayoutFromXml(xmlFile);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void SaveToStream(System.IO.Stream stream) {
			SaveLayoutToStream(stream);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RestoreFromStream(System.IO.Stream stream) {
			RestoreLayoutFromStream(stream);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool SaveToRegistry(string path) {
			return SaveLayoutToRegistry(path);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RestoreFromRegistry(string path) {
			RestoreLayoutFromRegistry(path);
		}
		public virtual void SaveLayoutToXml(string xmlFile) {
			SaveLayoutCore(new XmlXtraSerializer(), xmlFile);
		}
		public virtual void RestoreLayoutFromXml(string xmlFile) {
			RestoreLayoutCore(new XmlXtraSerializer(), xmlFile);
		}
		public virtual void SaveLayoutToStream(System.IO.Stream stream) {
			SaveLayoutCore(new XmlXtraSerializer(), stream);
		}
		public virtual void RestoreLayoutFromStream(System.IO.Stream stream) {
			RestoreLayoutCore(new XmlXtraSerializer(), stream);
		}
		void ISupportXtraSerializer.SaveLayoutToRegistry(string path) { SaveLayoutCore(new RegistryXtraSerializer(), path); }
		public virtual bool SaveLayoutToRegistry(string path) {
			return SaveLayoutCore(new RegistryXtraSerializer(), path);
		}
		public virtual void RestoreLayoutFromRegistry(string path) {
			RestoreLayoutCore(new RegistryXtraSerializer(), path);
		}
		protected virtual bool SaveLayoutCore(XtraSerializer serializer, object path) {
			System.IO.Stream stream = path as System.IO.Stream;
			if(stream != null)
				return serializer.SerializeObject(this, stream, this.GetType().Name);
			else
				return serializer.SerializeObject(this, path.ToString(), this.GetType().Name);
		}
		protected virtual void RestoreLayoutCore(XtraSerializer serializer, object path) {
			if(Form == null) return;
			System.IO.Stream stream = path as System.IO.Stream;
			if(stream != null)
				serializer.DeserializeObject(this, stream, this.GetType().Name);
			else
				serializer.DeserializeObject(this, path.ToString(), this.GetType().Name);
		}
		bool serializationInProgressCore = false;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool SerializationInProgress { get { return serializationInProgressCore || IsDeserializing; } }
		SortedList deserializedPanels = null, deserializedAutoHideContainers = null;
		int activePanelID = LayoutConsts.InvalidIndex;
		protected SortedList DeserializedPanels { get { return deserializedPanels; } }
		protected SortedList DeserializedAutoHideContainers { get { return deserializedAutoHideContainers; } }
		string oldLayoutVersion;
		IDisposable lockPaintingObj;
		void IXtraSerializable.OnStartDeserializing(LayoutAllowEventArgs e) {
			oldLayoutVersion = LayoutVersion;
			RaiseBeforeLoadLayout(e);
			if(!e.Allow) return;
			if(!IsInitialized) DeserializeLayoutManager();
			if(DocumentManager != null)
				lockPaintingObj = DocumentManager.View.LockPainting();
			LockDeserialize();
			LayoutManager.HideImmediately();
			this.screenConfigurationCore = new List<Rectangle>();
			this.deserializedPanels = new SortedList();
			this.deserializedAutoHideContainers = new SortedList();
		}
		protected void CheckPanelsCB56273() {
			foreach(DockPanel panel in Panels) {
				if(panel.Visibility == DockVisibility.AutoHide) {
					panel.Dock = panel.XtraAutoHideContainerDock;
				}
			}
		}
		void IXtraSerializable.OnEndDeserializing(string restoredVersion) {
			CheckScreenConfiguration();
			RestoreAutoHideContainers();
			RestoreDockPanels();
			CheckPanelsCB56273();
			UnlockDeserialize();
			this.deserializedPanels = this.deserializedAutoHideContainers = null;
			this.loadFired = false;
			try {
				OnLoaded();
				FormSizeChanged(this, EventArgs.Empty);
			}
			finally {
				if(restoredVersion != oldLayoutVersion) RaiseLayoutUpgrade(new LayoutUpgradeEventArgs(restoredVersion));
				Docking2010.Ref.Dispose(ref lockPaintingObj);
			}
			this.screenConfigurationCore = null;
		}
		void IXtraSerializable.OnStartSerializing() {
			OnStartSerializing();
		}
		void IXtraSerializable.OnEndSerializing() {
			OnEndSerialising();
		}
		protected virtual void OnEndSerialising() {
			serializationInProgressCore = false;
			screenConfigurationCore = null;
		}
		protected virtual void OnStartSerializing() {
			serializationInProgressCore = true;
			screenConfigurationCore = GetScreenConfiguration();
		}
		Rectangle[] GetScreenConfiguration() {
			return System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Select(Screen.AllScreens, s => s.Bounds));
		}
		void CheckScreenConfiguration() {
			if(screenConfigurationCore == null)
				return;
			if(screenConfigurationCore.Count == 0) {
				screenConfigurationCore = null;
				return;
			}
			var currentScreens = GetScreenConfiguration();
			for(int i = 0; i < currentScreens.Length; i++)
				screenConfigurationCore.Remove(currentScreens[i]);
		}
		protected virtual object XtraFindPanelsItem(XtraItemEventArgs e) {
			Guid id = DockLayoutUtils.ConvertToGuid(e.Item.ChildProperties["ID"].Value.ToString());
			if(id.Equals(Guid.Empty)) return null;
			DockPanel result = Panels[id];
			if(result != null)
				result.UndockOnRestoreLayout();
			return result;
		}
		protected virtual object XtraCreatePanelsItem(XtraItemEventArgs e) {
			DockingStyle dock = DockLayoutUtils.ConvertToDockingStyle(e.Item.ChildProperties["Dock"].Value.ToString());
			DockPanel result = CreateDockPanel(dock, e.Item.ChildProperties["Count"].Value.ToString() == "0");
			if(IsDesignMode && Site != null && Site.Container != null)
				Site.Container.Add(result);
			return result;
		}
		protected virtual void XtraSetIndexPanelsItem(XtraSetItemIndexEventArgs e) {
			DockPanel panel = e.Item.Value as DockPanel;
			DeserializedPanels[Convert.ToInt32(e.Item.ChildProperties["XtraID"].Value)] = new XtraDeserializeInfo(e);
			panel.Parent = null;
		}
		protected virtual void XtraClearAutoHideContainers(XtraItemEventArgs e) {
			for(int i = 0; i < AutoHideContainers.Count; i++) {
				while(AutoHideContainers[i].Controls.Count > 0) {
					DockPanel panel = (DockPanel)AutoHideContainers[i].Controls[0];
					panel.UndockOnRestoreLayout();
				}
			}
		}
		protected virtual object XtraFindAutoHideContainersItem(XtraItemEventArgs e) {
			return AutoHideContainers[(DockingStyle)DockLayoutUtils.ConvertToDockStyle(e.Item.ChildProperties["Dock"].Value.ToString())];
		}
		protected virtual object XtraCreateAutoHideContainersItem(XtraItemEventArgs e) {
			AutoHideContainer result = CreateAutoHideContainerCore();
			if(IsDesignMode && Site != null && Site.Container != null)
				Site.Container.Add(result);
			return result;
		}
		protected virtual void XtraSetIndexAutoHideContainersItem(XtraSetItemIndexEventArgs e) {
			DeserializedAutoHideContainers[Convert.ToInt32(e.Item.ChildProperties["XtraZIndex"].Value)] = e.Item.Value;
		}
		protected virtual void RestoreAutoHideContainers() {
			for(int i = AutoHideContainers.Count - 1; i > -1; i--) {
				AutoHideContainer container = AutoHideContainers[i];
				if(!DeserializedAutoHideContainers.ContainsValue(container))
					LayoutManager.DestroyAutoHideContainer(container);
			}
			AutoHideContainer[] containers = new AutoHideContainer[DeserializedAutoHideContainers.Count];
			for(int i = 0; i < DeserializedAutoHideContainers.Count; i++)
				containers[i] = (AutoHideContainer)DeserializedAutoHideContainers.GetByIndex(i);
			AutoHideContainers.SetRange(containers);
			int topIndexCount = GetTopZIndexControlCount();
			for(int i = 0; i < AutoHideContainers.Count; i++)
				RestoreParentOnDeserialize(Form, AutoHideContainers[i], topIndexCount);
		}
		protected virtual void RestoreDockPanels() {
			if(DeserializedPanels == null) return;
			ReadOnlyPanelCollection collection = new ReadOnlyPanelCollection();
			XtraDeserializeInfoCollection rootInfoCollection = new XtraDeserializeInfoCollection();
			XtraDeserializeInfoCollection mdiContainerInfoCollection = new XtraDeserializeInfoCollection();
			XtraDeserializeInfoCollection hiddenInfoCollection = new XtraDeserializeInfoCollection();
			XtraDeserializeAutoHideInfoCollection autoHideInfoCollection = new XtraDeserializeAutoHideInfoCollection();
			XtraDeserializeChildInfoCollection childInfoCollection = new XtraDeserializeChildInfoCollection();
			for(int panelID = 0; panelID < DeserializedPanels.Count; panelID++) {
				XtraDeserializeInfo info = (XtraDeserializeInfo)DeserializedPanels.GetByIndex(panelID);
				if(info.Visibility != DockVisibility.Hidden) {
					switch(info.ParentID) {
						case DockConsts.RootParentID: rootInfoCollection.Add(info); break;
						case DockConsts.AutoHideParentID: autoHideInfoCollection.Add(info); break;
						default: childInfoCollection.Add(info); break;
					}
					if(info.IsMdiDocument)
						mdiContainerInfoCollection.Add(info);
				}
				else
					hiddenInfoCollection.Add(info);
				collection.Add(info.Panel);
			}
			DestroyUselessPanelsOnDeserialize(collection);
			Panels.Assign(collection);
			RestoreRootPanels(rootInfoCollection);
			RestoreHiddenPanels(hiddenInfoCollection);
			RestoreAutoHidePanels(autoHideInfoCollection);
			RestoreChildPanels(childInfoCollection);
			RestoreMdiContainerPanels(mdiContainerInfoCollection);
		}
		protected virtual void RestoreRootPanels(XtraDeserializeInfoCollection rootInfoCollection) {
			rootInfoCollection.Sort();
			RootPanels.Assign(rootInfoCollection);
			RestoreDockPanelsCore(rootInfoCollection, Form, null, AutoHideContainers.Count + GetTopZIndexControlCount());
		}
		protected virtual void RestoreMdiContainerPanels(XtraDeserializeInfoCollection mdiContainerInfoCollection) {
			if(DocumentManager == null) return;
			foreach(XtraDeserializeInfo info in mdiContainerInfoCollection) {
				DockPanel panel = info.Panel;
				if(Panels.Contains(panel)) {
					panel.DockManager.BeginFormLayoutUpdate();
					panel.DockLayout.Dock = DockingStyle.Float;
					DocumentManager.View.Controller.Dock(panel);
					panel.DockManager.EndFormLayoutUpdate();
				}
			}
		}
		protected virtual void RestoreHiddenPanels(XtraDeserializeInfoCollection hiddenInfoCollection) {
			HiddenPanels.Assign(hiddenInfoCollection);
			RestoreDockPanelsCore(hiddenInfoCollection, Form, null, 0);
		}
		protected virtual void RestoreChildPanels(XtraDeserializeChildInfoCollection childInfoCollection) {
			foreach(int parentID in childInfoCollection.Keys) {
				XtraDeserializeInfoCollection childCollection = childInfoCollection[parentID];
				XtraDeserializeInfo parentInfo = (XtraDeserializeInfo)DeserializedPanels[parentID];
				RestoreDockPanelsCore(childCollection, parentInfo.Panel, parentInfo, 0);
			}
		}
		protected virtual void RestoreAutoHidePanels(XtraDeserializeAutoHideInfoCollection autoHideInfoCollection) {
			foreach(DockingStyle dock in autoHideInfoCollection.Keys) {
				XtraDeserializeInfoCollection childCollection = autoHideInfoCollection[dock];
				RestoreDockPanelsCore(childCollection, AutoHideContainers[dock], null, 0);
			}
		}
		protected virtual void RestoreDockPanelsCore(XtraDeserializeInfoCollection childCollection, Control parent, XtraDeserializeInfo parentInfo, int topIndexCount) {
			if(childCollection.Count == 0) return;
			childCollection.Sort();
			parent.SuspendLayout();
			try {
				for(int i = 0; i < childCollection.Count; i++) {
					RestoreParentOnDeserialize(childCollection[i], parent, topIndexCount);
					RestoreSavedParentOnDeserialize(childCollection[i]);
				}
				if(parentInfo != null && parentInfo.ActiveChildID != -1)
					parentInfo.Panel.ActiveChild = ((XtraDeserializeInfo)DeserializedPanels[parentInfo.ActiveChildID]).Panel;
			}
			finally {
				parent.ResumeLayout();
			}
		}
		void RestoreParentOnDeserialize(XtraDeserializeInfo info, Control parent, int topIndexCount) {
			if(info.Panel.Dock == DockingStyle.Float) return;
			if(info.Visibility == DockVisibility.Hidden) return;
			RestoreParentOnDeserialize(parent, info.Panel, topIndexCount);
			if(info.Panel.Visibility != DockVisibility.AutoHide)
				info.Panel.UpdateControlVisible(info.Panel.Visible);
		}
		void RestoreParentOnDeserialize(Control parent, Control control, int topIndexCount) {
			parent.Controls.Add(control);
			if(topIndexCount > 0)
				parent.Controls.SetChildIndex(control, parent.Controls.Count - topIndexCount - 1);
		}
		void RestoreSavedParentOnDeserialize(XtraDeserializeInfo info) {
			if(info.SavedParentID == LayoutConsts.InvalidIndex) {
				if(info.Panel.Dock == DockingStyle.Float) {
					SavedDockPanelInfo savedInfo = info.Panel.DockLayout.GetSavedInfo();
					savedInfo.SavedParent = null;
					info.Panel.DockLayout.SetSavedInfo(savedInfo);
				}
				if(info.Panel.SavedParent != null) info.Panel.SavedParent = null;
				return;
			}
			info.Panel.SavedParent = (DeserializedPanels[info.SavedParentID] as XtraDeserializeInfo).Panel;
		}
		protected virtual void DestroyUselessPanelsOnDeserialize(ReadOnlyPanelCollection collection) {
			for(int i = ExistingPanelsCount - 1; i > -1; i--) {
				DockPanel panel = GetExistingPanel(i);
				if(collection.Contains(panel)) continue;
				panel.DestoryOnRestoreLayout();
				i = Math.Min(i, Count);
			}
		}
		int ExistingPanelsCount { get { return (IsInitialized ? Count : RootPanels.Count); } }
		DockPanel GetExistingPanel(int index) {
			if(IsInitialized) return this[index];
			return RootPanels[index];
		}
		#endregion IXtraSerializable
		protected static DockManagerCollection DockManagers { get { return dockManagers; } }
		protected bool FirstInit { get { return firstInit; } }
		protected internal DockElementsPainter Painter { get { return CurrentController.PaintStyle.ElementsPainter; } }
		protected internal virtual bool CanActivateFloatForm { get { return !DesignMode; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DockPanelCollection RootPanels { get { return rootPanels; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DockPanelCollection HiddenPanels { get { return hiddenPanels; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(true, true, true)]
		public AutoHideContainerCollection AutoHideContainers { get { return LayoutManager.AutoHideContainers; } }
		bool ShouldSerializeDockingOptions() { return DockingOptions.ShouldSerializeCore(); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockManagerDockingOptions"),
#endif
 Category(DockConsts.BehaviorCategory), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public DockingOptions DockingOptions { get { return dockingOptions; } }
		bool ShouldSerializeSerializationOptions() { return SerializationOptions.ShouldSerializeCore(); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockManagerSerializationOptions"),
#endif
 Category(DockConsts.BehaviorCategory), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LayoutSerializationOptions SerializationOptions { get { return serializationOptions; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DockPanel ActivePanel {
			get { return activePanel; }
			set {
				DockPanel oldActive = ActivePanel;
				if(Form == null || oldActive == value || DesignMode) return;
				if(ValidateDockPanelOnDeactivate) {
					if(ActivePanel != null && !ActivePanel.ValidateOnSetInactive()) return;
				}
				if(ValidateFormChildrenOnDeactivate) {
					if(CanValidateFormChildren(oldActive, value) && !ValidateFormChildren()) return;
				}
				if(value != null && !value.CanActivate) return;
				activePanel = value;
				OnActivePanelChanged(value, oldActive);
			}
		}
		protected virtual bool CanValidateFormChildren(DockPanel oldActivePanel, DockPanel newActivePanel) {
			if(newActivePanel == null || Disposing) return false;
			bool canValidate = oldActivePanel == null;
			DockPanel activePanel = null;
			if(Form != null)
				activePanel = Form.ActiveControl as DockPanel;
			if(activePanel != null && IsParentPanel(newActivePanel, ActivePanel))
				canValidate = false;
			return canValidate;
		}
		bool IsParentPanel(DockPanel panel, DockPanel parentPanel) {
			if(panel == parentPanel) return true;
			if(panel == null) return false;
			return IsParentPanel(panel.ParentPanel, parentPanel);
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), XtraSerializableProperty()]
		public int ActivePanelID {
			get { return (ActivePanel == null ? LayoutConsts.InvalidIndex : ActivePanel.XtraID); }
			set { this.activePanelID = value; }
		}
		IList<Rectangle> screenConfigurationCore;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[XtraSerializableProperty(XtraSerializationVisibility.SimpleCollection, true)]
		public IEnumerable<Rectangle> XtraSerializableScreenConfiguration {
			get { return screenConfigurationCore; }
		}
		void XtraCreateXtraSerializableScreenConfigurationItem(XtraItemEventArgs args) {
			if(screenConfigurationCore != null && args.Item != null)
				screenConfigurationCore.Add((Rectangle)args.Item.ValueToObject(typeof(Rectangle)));
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockManagerTopZIndexControls"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.SimpleCollection, false, true, true, 1),
		Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design", typeof(System.Drawing.Design.UITypeEditor)), Category(DockConsts.LayoutCategory)]
		public StringUniqueCollection TopZIndexControls { get { return topZIndexControls; } }
		protected virtual object XtraFindTopZIndexControlsItem(XtraItemEventArgs e) {
			return e.Item.Value as string;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XtraSerializableProperty(true, true, true)]
		public ReadOnlyPanelCollection Panels { get { return panels; } }
		protected internal bool suspendRaiseActivePanelChanged = false;
		protected virtual void OnActivePanelChanged(DockPanel newPanel, DockPanel oldPanel) {
			if(oldPanel != null) {
#if DXWhidbey
				oldPanel.DockLayout.RefreshCaption();
#else
				oldPanel.OnDeactivate(newPanel);
#endif
			}
			if(newPanel != null)
				newPanel.OnActivate();
			if(!suspendRaiseActivePanelChanged)
				RaiseActivePanelChanged(new ActivePanelChangedEventArgs(newPanel, oldPanel));
		}
		bool ValidateFormChildren() {
			var form = (Form != null) ? Form.FindForm() : null;
			return (form == null) || form.ValidateChildren() || AllowFocusChangeOnValidation(form);
		}
		bool AllowFocusChangeOnValidation(Form form) {
			return (form.AutoValidate != System.Windows.Forms.AutoValidate.EnablePreventFocusChange);
		}
		void LayoutChanged() { LayoutManager.LayoutChanged(); }
		#region Events
		static readonly object load = new object();
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockManagerLoad"),
#endif
 Category(DockConsts.DockingCategory)]
		public event EventHandler Load {
			add { this.Events.AddHandler(load, value); }
			remove { this.Events.RemoveHandler(load, value); }
		}
		protected virtual void RaiseLoad() {
			EventHandler handler = (EventHandler)this.Events[load];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		static readonly object layoutUpgrade = new object();
		private static readonly object beforeLoadLayout = new object();
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockManagerBeforeLoadLayout"),
#endif
 Category(DockConsts.DockingCategory)]
		public event LayoutAllowEventHandler BeforeLoadLayout {
			add { this.Events.AddHandler(beforeLoadLayout, value); }
			remove { this.Events.RemoveHandler(beforeLoadLayout, value); }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockManagerLayoutUpgrade"),
#endif
 Category(DockConsts.DockingCategory)]
		public event LayoutUpgradeEventHandler LayoutUpgrade {
			add { this.Events.AddHandler(layoutUpgrade, value); }
			remove { this.Events.RemoveHandler(layoutUpgrade, value); }
		}
		protected internal virtual void RaiseBeforeLoadLayout(LayoutAllowEventArgs e) {
			LayoutAllowEventHandler handler = (LayoutAllowEventHandler)this.Events[beforeLoadLayout];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseLayoutUpgrade(LayoutUpgradeEventArgs e) {
			LayoutUpgradeEventHandler handler = (LayoutUpgradeEventHandler)this.Events[layoutUpgrade];
			if(handler != null) handler(this, e);
		}
		static readonly object activePanelChanged = new object();
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockManagerActivePanelChanged"),
#endif
 Category(DockConsts.DockingCategory)]
		public event ActivePanelChangedEventHandler ActivePanelChanged {
			add { this.Events.AddHandler(activePanelChanged, value); }
			remove { this.Events.RemoveHandler(activePanelChanged, value); }
		}
		protected virtual void RaiseActivePanelChanged(ActivePanelChangedEventArgs e) {
			ActivePanelChangedEventHandler handler = (ActivePanelChangedEventHandler)this.Events[activePanelChanged];
			if(handler != null) handler(this, e);
		}
		static readonly object registerDockPanel = new object();
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockManagerRegisterDockPanel"),
#endif
 Category(DockConsts.DockingCategory)]
		public event DockPanelEventHandler RegisterDockPanel {
			add { this.Events.AddHandler(registerDockPanel, value); }
			remove { this.Events.RemoveHandler(registerDockPanel, value); }
		}
		protected internal virtual void RaiseRegisterDockPanel(DockPanel panel) {
			if(lockRaiseRegisterPanelCounter != 0) return;
			DockPanelEventHandler handler = (DockPanelEventHandler)this.Events[registerDockPanel];
			if(handler != null) handler(this, new DockPanelEventArgs(panel));
		}
		static readonly object unregisterDockPanel = new object();
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockManagerUnregisterDockPanel"),
#endif
 Category(DockConsts.DockingCategory)]
		public event DockPanelEventHandler UnregisterDockPanel {
			add { this.Events.AddHandler(unregisterDockPanel, value); }
			remove { this.Events.RemoveHandler(unregisterDockPanel, value); }
		}
		protected virtual void RaiseUnregisterDockPanel(DockPanel panel) {
			DockPanelEventHandler handler = (DockPanelEventHandler)this.Events[unregisterDockPanel];
			if(handler != null) handler(this, new DockPanelEventArgs(panel));
		}
		static readonly object closingPanel = new object();
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockManagerClosingPanel"),
#endif
 Category(DockConsts.DockingCategory)]
		public event DockPanelCancelEventHandler ClosingPanel {
			add { this.Events.AddHandler(closingPanel, value); }
			remove { this.Events.RemoveHandler(closingPanel, value); }
		}
		protected internal virtual void RaiseClosingPanel(DockPanelCancelEventArgs e) {
			DockPanelCancelEventHandler handler = (DockPanelCancelEventHandler)this.Events[closingPanel];
			if(handler != null) handler(this, e);
		}
		static readonly object closedPanel = new object();
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockManagerClosedPanel"),
#endif
 Category(DockConsts.DockingCategory)]
		public event DockPanelEventHandler ClosedPanel {
			add { this.Events.AddHandler(closedPanel, value); }
			remove { this.Events.RemoveHandler(closedPanel, value); }
		}
		protected internal virtual void RaiseClosedPanel(DockPanelEventArgs e) {
			DockPanelEventHandler handler = (DockPanelEventHandler)this.Events[closedPanel];
			if(handler != null) handler(this, e);
		}
		static readonly object visibilityChanged = new object();
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockManagerVisibilityChanged"),
#endif
 Category(DockConsts.DockingCategory)]
		public event VisibilityChangedEventHandler VisibilityChanged {
			add { this.Events.AddHandler(visibilityChanged, value); }
			remove { this.Events.RemoveHandler(visibilityChanged, value); }
		}
		protected internal virtual void RaiseVisibilityChanged(VisibilityChangedEventArgs e) {
			VisibilityChangedEventHandler handler = (VisibilityChangedEventHandler)this.Events[visibilityChanged];
			if(handler != null) handler(this, e);
		}
		static readonly object tabbedChanged = new object();
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockManagerTabbedChanged"),
#endif
 Category(DockConsts.DockingCategory)]
		public event DockPanelEventHandler TabbedChanged {
			add { this.Events.AddHandler(tabbedChanged, value); }
			remove { this.Events.RemoveHandler(tabbedChanged, value); }
		}
		protected internal virtual void RaiseTabbedChanged(DockPanelEventArgs e) {
			DockPanelEventHandler handler = (DockPanelEventHandler)this.Events[tabbedChanged];
			if(handler != null) handler(this, e);
		}
		static readonly object tabsScrollChanged = new object();
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockManagerTabsScrollChanged"),
#endif
 Category(DockConsts.DockingCategory)]
		public event DockPanelEventHandler TabsScrollChanged {
			add { this.Events.AddHandler(tabsScrollChanged, value); }
			remove { this.Events.RemoveHandler(tabsScrollChanged, value); }
		}
		protected internal virtual void RaiseTabsScrollChanged(DockPanelEventArgs e) {
			DockPanelEventHandler handler = (DockPanelEventHandler)this.Events[tabsScrollChanged];
			if(handler != null) handler(this, e);
		}
		static readonly object tabsPositionChanged = new object();
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockManagerTabsPositionChanged"),
#endif
 Category(DockConsts.DockingCategory)]
		public event TabsPositionChangedEventHandler TabsPositionChanged {
			add { this.Events.AddHandler(tabsPositionChanged, value); }
			remove { this.Events.RemoveHandler(tabsPositionChanged, value); }
		}
		protected internal virtual void RaiseTabsPositionChanged(TabsPositionChangedEventArgs e) {
			TabsPositionChangedEventHandler handler = (TabsPositionChangedEventHandler)this.Events[tabsPositionChanged];
			if(handler != null) handler(this, e);
		}
		static readonly object activeChildChanged = new object();
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockManagerActiveChildChanged"),
#endif
 Category(DockConsts.DockingCategory)]
		public event DockPanelEventHandler ActiveChildChanged {
			add { this.Events.AddHandler(activeChildChanged, value); }
			remove { this.Events.RemoveHandler(activeChildChanged, value); }
		}
		protected internal virtual void RaiseActiveChildChanged(DockPanelEventArgs e) {
			DockPanelEventHandler handler = (DockPanelEventHandler)this.Events[activeChildChanged];
			if(handler != null) handler(this, e);
		}
		static readonly object startDocking = new object();
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockManagerStartDocking"),
#endif
 Category(DockConsts.DockingCategory)]
		public event DockPanelCancelEventHandler StartDocking {
			add { this.Events.AddHandler(startDocking, value); }
			remove { this.Events.RemoveHandler(startDocking, value); }
		}
		protected internal virtual void RaiseStartDocking(DockPanelCancelEventArgs e) {
			DockPanelCancelEventHandler handler = (DockPanelCancelEventHandler)this.Events[startDocking];
			if(handler != null) handler(this, e);
			if(!e.Cancel)
				suspendRaiseActivePanelChanged = true;
		}
		static readonly object docking = new object();
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockManagerDocking"),
#endif
 Category(DockConsts.DockingCategory)]
		public event DockingEventHandler Docking {
			add { this.Events.AddHandler(docking, value); }
			remove { this.Events.RemoveHandler(docking, value); }
		}
		protected internal virtual void RaiseDocking(DockingEventArgs e) {
			DockingEventHandler handler = (DockingEventHandler)this.Events[docking];
			if(handler != null) handler(this, e);
		}
		static readonly object endDocking = new object();
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockManagerEndDocking"),
#endif
 Category(DockConsts.DockingCategory)]
		public event EndDockingEventHandler EndDocking {
			add { this.Events.AddHandler(endDocking, value); }
			remove { this.Events.RemoveHandler(endDocking, value); }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockManagerDockMode"),
#endif
 DefaultValue(DockMode.VS2005)]
		public DockMode DockMode {
			get { return dragVisualizationStyleCore; }
			set { dragVisualizationStyleCore = value; FormSizeChanged(this, EventArgs.Empty); }
		}
		static readonly object expandingCore = new object();
		public event DockPanelCancelEventHandler Expanding {
			add { this.Events.AddHandler(expandingCore, value); }
			remove { this.Events.RemoveHandler(expandingCore, value); }
		}
		protected internal virtual bool RaiseExpanding(DockPanelCancelEventArgs e) {
			DockPanelCancelEventHandler handler = (DockPanelCancelEventHandler)this.Events[expandingCore];
			if(handler != null) handler(this, e);
			return !e.Cancel;
		}
		static readonly object expandedCore = new object();
		public event DockPanelEventHandler Expanded {
			add { this.Events.AddHandler(expandedCore, value); }
			remove { this.Events.RemoveHandler(expandedCore, value); }
		}
		protected internal virtual void RaiseExpanded(DockPanelEventArgs e) {
			DockPanelEventHandler handler = (DockPanelEventHandler)this.Events[expandedCore];
			if(handler != null) handler(this, e);
		}
		static readonly object collapsingCore = new object();
		public event DockPanelEventHandler Collapsing {
			add { this.Events.AddHandler(collapsingCore, value); }
			remove { this.Events.RemoveHandler(collapsingCore, value); }
		}
		protected internal virtual void RaiseCollapsing(DockPanelEventArgs e) {
			DockPanelEventHandler handler = (DockPanelEventHandler)this.Events[collapsingCore];
			if(handler != null) handler(this, e);
		}
		static readonly object collapsedCore = new object();
		public event DockPanelEventHandler Collapsed {
			add { this.Events.AddHandler(collapsedCore, value); }
			remove { this.Events.RemoveHandler(collapsedCore, value); }
		}
		protected internal virtual void RaiseCollapsed(DockPanelEventArgs e) {
			DockPanelEventHandler handler = (DockPanelEventHandler)this.Events[collapsedCore];
			if(handler != null) handler(this, e);
		}
		static readonly object createResizeZoneCore = new object();
		public event CreateResizeZoneEventHandler CreateResizeZone {
			add { Events.AddHandler(createResizeZoneCore, value); }
			remove { Events.RemoveHandler(createResizeZoneCore, value); }
		}
		protected internal virtual bool RaiseCreateResizeZone(CreateResizeZoneEventArgs e) {
			CreateResizeZoneEventHandler handler = (CreateResizeZoneEventHandler)Events[createResizeZoneCore];
			if(handler != null) handler(this, e);
			return !e.Cancel;
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockManagerDockModeVS2005FadeSpeed"),
#endif
			 DefaultValue(15000)]
		public int DockModeVS2005FadeSpeed {
			get { return dockModeVS2005FadeSpeedCore; }
			set {
				dockModeVS2005FadeSpeedCore = value;
				globalDockZoneVisualizerCore = null;
				panelDockZoneVisualizerCore = null;
				FireChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockManagerDockModeVS2005FadeFramesCount"),
#endif
 DefaultValue(10)]
		public int DockModeVS2005FadeFramesCount {
			get { return dockModeVS2005FadeFramesCountCore; }
			set {
				dockModeVS2005FadeFramesCountCore = value;
				globalDockZoneVisualizerCore = null;
				panelDockZoneVisualizerCore = null;
				FireChanged();
			}
		}
		protected internal virtual void RaiseEndDocking(EndDockingEventArgs e) {
			EndDockingEventHandler handler = (EndDockingEventHandler)this.Events[endDocking];
			if(handler != null) handler(this, e);
			suspendRaiseActivePanelChanged = false;
		}
		static readonly object startSizing = new object();
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockManagerStartSizing"),
#endif
 Category(DockConsts.DockingCategory)]
		public event StartSizingEventHandler StartSizing {
			add { this.Events.AddHandler(startSizing, value); }
			remove { this.Events.RemoveHandler(startSizing, value); }
		}
		protected internal virtual void RaiseStartSizing(StartSizingEventArgs e) {
			StartSizingEventHandler handler = (StartSizingEventHandler)this.Events[startSizing];
			if(handler != null) handler(this, e);
		}
		static readonly object sizing = new object();
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockManagerSizing"),
#endif
 Category(DockConsts.DockingCategory)]
		public event SizingEventHandler Sizing {
			add { this.Events.AddHandler(sizing, value); }
			remove { this.Events.RemoveHandler(sizing, value); }
		}
		protected internal virtual void RaiseSizing(SizingEventArgs e) {
			SizingEventHandler handler = (SizingEventHandler)this.Events[sizing];
			if(handler != null) handler(this, e);
		}
		static readonly object endSizing = new object();
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockManagerEndSizing"),
#endif
 Category(DockConsts.DockingCategory)]
		public event EndSizingEventHandler EndSizing {
			add { this.Events.AddHandler(endSizing, value); }
			remove { this.Events.RemoveHandler(endSizing, value); }
		}
		protected internal virtual void RaiseEndSizing(EndSizingEventArgs e) {
			EndSizingEventHandler handler = (EndSizingEventHandler)this.Events[endSizing];
			if(handler != null) handler(this, e);
		}
		static readonly object createAutoHideContainer = new object();
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockManagerCreateAutoHideContainer"),
#endif
 Category(DockConsts.DockingCategory)]
		public event AutoHideContainerEventHandler CreateAutoHideContainer {
			add { this.Events.AddHandler(createAutoHideContainer, value); }
			remove { this.Events.RemoveHandler(createAutoHideContainer, value); }
		}
		protected internal virtual void RaiseCreateAutoHideContainer(AutoHideContainerEventArgs e) {
			AutoHideContainerEventHandler handler = (AutoHideContainerEventHandler)this.Events[createAutoHideContainer];
			if(handler != null) handler(this, e);
		}
		static readonly object destroyAutoHideContainer = new object();
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockManagerDestroyAutoHideContainer"),
#endif
 Category(DockConsts.DockingCategory)]
		public event AutoHideContainerEventHandler DestroyAutoHideContainer {
			add { this.Events.AddHandler(destroyAutoHideContainer, value); }
			remove { this.Events.RemoveHandler(destroyAutoHideContainer, value); }
		}
		protected internal virtual void RaiseDestroyAutoHideContainer(AutoHideContainerEventArgs e) {
			AutoHideContainerEventHandler handler = (AutoHideContainerEventHandler)this.Events[destroyAutoHideContainer];
			if(handler != null) handler(this, e);
		}
		#endregion Events
		#region IDXMenuManager
		IDXMenuManager menuManagerCore;
#if !SL
	[DevExpressXtraBarsLocalizedDescription("DockManagerMenuManager")]
#endif
		[Category(DockConsts.AppearanceCategory), DefaultValue(null)]
		public IDXMenuManager MenuManager {
			get { return menuManagerCore; }
			set { menuManagerCore = value; }
		}
		Controller.IDockController dockControllerCore;
		[Browsable(false)]
		public Controller.IDockController DockController {
			get { return dockControllerCore; }
		}
		protected virtual Controller.IDockController CreateDockController() {
			return new Controller.DockController(this);
		}
		protected internal UserLookAndFeel LookAndFeel {
			get { return GetBarAndDockingController().LookAndFeel; }
		}
		protected internal IDXMenuManager GetMenuManager() {
			return menuManagerCore ?? MenuManagerHelper.GetMenuManager(LookAndFeel);
		}
		static readonly object popupMenuShowing = new object();
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockManagerPopupMenuShowing"),
#endif
 Category(DockConsts.DockingCategory)]
		public event PopupMenuShowingEventHandler PopupMenuShowing {
			add { Events.AddHandler(popupMenuShowing, value); }
			remove { Events.RemoveHandler(popupMenuShowing, value); }
		}
		protected internal virtual bool CanShowContextMenu(DockControllerMenu menu, Point point) {
			return !RaisePopupMenuShowing(menu, point);
		}
		protected internal bool RaisePopupMenuShowing(DockControllerMenu menu, Point point) {
			PopupMenuShowingEventArgs e = new PopupMenuShowingEventArgs(menu, point);
			PopupMenuShowingEventHandler handler = (PopupMenuShowingEventHandler)Events[popupMenuShowing];
			if(handler != null)
				handler(this, e);
			return e.Cancel;
		}
		#endregion IDXMenuManager
		#region DockGuides
		static readonly object showingDockGuides = new object();
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockManagerShowingDockGuides"),
#endif
 Category(DockConsts.DockingCategory)]
		public event ShowingDockGuidesEventHandler ShowingDockGuides {
			add { Events.AddHandler(showingDockGuides, value); }
			remove { Events.RemoveHandler(showingDockGuides, value); }
		}
		protected internal void RaiseShowingDockGuides(DockPanel panel, DockPanel targetPanel, DockGuidesConfiguration configuration) {
			ShowingDockGuidesEventHandler handler = (ShowingDockGuidesEventHandler)Events[showingDockGuides];
			if(handler != null)
				handler(this, new ShowingDockGuidesEventArgs(panel, targetPanel, configuration));
		}
		#endregion DockGuides
		protected internal bool CanDockPanelInCaptionRegion() {
			DefaultBoolean allow = DockingOptions.DockPanelInCaptionRegion;
			if(allow == DefaultBoolean.Default)
				return SupportVS2010DockingStyle();
			return allow == DefaultBoolean.True;
		}
		#region IVS2010StyleInfo Members
		Control IStyleDockZone.Owner {
			get { return this.Form; }
		}
		UserLookAndFeel IStyleDockZone.ElementsLookAndFeel {
			get { return this.LookAndFeel; }
		}
		#endregion
		#region ILogicalOwner Members
		public System.Collections.Generic.IEnumerable<Component> GetLogicalChildren() {
			foreach(DockPanel panel in Panels) {
				yield return panel;
			}
		}
		#endregion
		#region MVVM
		DevExpress.Utils.MVVM.Services.IDocumentAdapter DevExpress.Utils.MVVM.Services.IDocumentAdapterFactory.Create() {
			return new MVVM.Services.DockPanelAdapter(this);
		}
		#endregion MVVM
	}
}
namespace DevExpress.XtraBars.MVVM.Services {
	using System.Linq;
	using DevExpress.Utils.MVVM.Services;
	using DevExpress.XtraBars.Docking;
	class DockPanelAdapter : IDocumentAdapter {
		DockManager hostManager;
		DockPanel hostPanel;
		bool addAsFloat;
		public DockPanelAdapter(DockManager manager, bool addAsFloat = false) {
			this.hostManager = manager;
			this.addAsFloat = addAsFloat;
			hostManager.ClosedPanel += manager_ClosedPanel;
			hostManager.ClosingPanel += manager_ClosingPanel;
		}
		public void Dispose() {
			var control = GetControl(hostPanel);
			if(control != null)
				control.TextChanged -= control_TextChanged;
			hostManager.ClosedPanel -= manager_ClosedPanel;
			hostManager.ClosingPanel -= manager_ClosingPanel;
			hostPanel = null;
		}
		void manager_ClosingPanel(object sender, DockPanelCancelEventArgs e) {
			if(e.Panel == hostPanel)
				RaiseClosing(e);
		}
		void manager_ClosedPanel(object sender, DockPanelEventArgs e) {
			if(e.Panel == hostPanel) {
				RaiseClosed(e);
				e.Panel.Dispose();
				Dispose();
			}
		}
		void control_TextChanged(object sender, EventArgs e) {
			hostPanel.Text = ((Control)sender).Text;
		}
		void RaiseClosed(DockPanelEventArgs e) {
			if(Closed != null) Closed(hostManager, e);
		}
		void RaiseClosing(DockPanelCancelEventArgs e) {
			var args = new CancelEventArgs(e.Cancel);
			if(Closing != null) Closing(hostManager, args);
			e.Cancel = args.Cancel;
		}
		public event EventHandler Closed;
		public event CancelEventHandler Closing;
		public void Show(Control control) {
			var panel = hostManager.Panels.FirstOrDefault(p => GetControl(p) == control);
			if(panel == null) {
				hostPanel = hostManager.AddPanel(addAsFloat ? DockingStyle.Float : DockingStyle.Left);
				hostPanel.Text = control.Text;
				control.TextChanged += control_TextChanged;
			}
			if(hostPanel != null) {
				if(!hostPanel.ControlContainer.Controls.Contains(control)) {
					control.Dock = DockStyle.Fill;
					hostPanel.ControlContainer.Controls.Add(control);
				}
			}
			hostManager.ActivePanel = hostPanel;
		}
		public void Close(Control control, bool force = true) {
			if(force)
				hostManager.ClosingPanel -= manager_ClosingPanel;
			if(control != null)
				control.TextChanged -= control_TextChanged;
			hostPanel.Dispose();
		}
		static Control GetControl(DockPanel panel) {
			return (panel != null && panel.ControlContainer != null)
				&& (panel.ControlContainer.Controls.Count > 0) ? panel.ControlContainer.Controls[0] : null;
		}
	}
}
