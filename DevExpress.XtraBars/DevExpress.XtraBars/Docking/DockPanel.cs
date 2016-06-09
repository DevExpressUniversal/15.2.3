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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Gesture;
using DevExpress.Utils.Serializing;
using DevExpress.XtraBars.Docking.Helpers;
using DevExpress.XtraBars.Docking.Paint;
using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraEditors.ButtonPanel;
using DevExpress.Utils.Design;
using DevExpress.Skins;
namespace DevExpress.XtraBars.Docking {
	internal interface IDockPanelInfo {
		string PanelName { get; }
		bool DockedAsMdiDocument { get; }
	}
	public enum DockPanelState { Regular, Sizing, Docking }
	public enum AutoHiddenPanelShowMode { Default, MouseHover, MouseClick }
	enum AutoHideMoveState { Collapsed, Expanding, Expanded, Collapsing }
#if DEBUGTEST
	[System.Diagnostics.DebuggerDisplay("Name={Name}, Text={Text}")]
#endif
	[ProhibitUsingAsDockingContainer]
	[Designer("DevExpress.XtraBars.Docking.Design.DockPanelDesigner, " + AssemblyInfo.SRAssemblyBarsDesign, typeof(IDesigner)),
	DevExpress.Utils.Design.SmartTagSupport(typeof(DockPanelDesignTimeBoundsProvider), DevExpress.Utils.Design.SmartTagSupportAttribute.SmartTagCreationMode.Auto),
	DevExpress.Utils.Design.SmartTagAction(typeof(DockPanelDesignTimeActionsProvider), "DockToLeft", "Dock to Left", SmartTagActionType.CloseAfterExecute),
	DevExpress.Utils.Design.SmartTagAction(typeof(DockPanelDesignTimeActionsProvider), "DockToRight", "Dock to Right", SmartTagActionType.CloseAfterExecute),
	DevExpress.Utils.Design.SmartTagAction(typeof(DockPanelDesignTimeActionsProvider), "DockToTop", "Dock to Top", SmartTagActionType.CloseAfterExecute),
	DevExpress.Utils.Design.SmartTagAction(typeof(DockPanelDesignTimeActionsProvider), "DockToBottom", "Dock to Bottom", SmartTagActionType.CloseAfterExecute),
	DevExpress.Utils.Design.SmartTagAction(typeof(DockPanelDesignTimeActionsProvider), "Float", "Float", SmartTagActionType.CloseAfterExecute),
	DevExpress.Utils.Design.SmartTagAction(typeof(DockPanelDesignTimeActionsProvider), "DockedAsTabbedDocument", "Dock as Tabbed Document", SmartTagActionType.CloseAfterExecute),
	DevExpress.Utils.Design.SmartTagAction(typeof(DockPanelDesignTimeActionsProvider), "AddCustomHeaderButtons", "Add Custom Header Buttons", SmartTagActionType.CloseAfterExecute)]
	public class DockPanel : ZIndexControl, IToolTipControlClient, IFlickGestureClient, IButtonsPanelOwner, IButtonsPanelGlyphSkinningOwner, ILookAndFeelProvider, IDockPanelInfo, DevExpress.Utils.Menu.IDXMenuManagerProvider {
		DockLayout dockLayout;
		DockManager dockManager;
		DockPanelMouseHandler mouseHandler;
		string tabText, hint;
		int setBoundsCounter, lockActivateCounter;
		AppearanceObject appearance;
		DockPanelOptions options;
		Guid id;
		Point clickPointCore = Point.Empty;
		ButtonsPanel buttonsPanelCore;
		ButtonCollection customHeaderButtonsCore;
		string headerCore;
		string footerCore;
		bool allowBorderColorBlendingCore;
		protected internal bool CanRaiseAutoHideMoveEvents {
			get { return Visibility == DockVisibility.AutoHide || (RootPanel != null && RootPanel.Visibility == DockVisibility.AutoHide); }
		}
		internal bool RaiseAutoHideMoveEvents(AutoHideMoveState state) {
			if(!CanRaiseAutoHideMoveEvents) return true;
			if(state == AutoHideMoveState.Expanded)
				RaiseExpanded(new DockPanelEventArgs(this));
			if(state == AutoHideMoveState.Expanding)
				return RaiseExpanding(new DockPanelCancelEventArgs(this));
			if(state == AutoHideMoveState.Collapsed)
				RaiseCollapsed(new DockPanelEventArgs(this));
			if(state == AutoHideMoveState.Collapsing)
				RaiseCollapsing(new DockPanelEventArgs(this));
			return true;
		}
		public DockPanel() : this(false, DockingStyle.Float, null) { }
		protected internal DockPanel(bool createControlContainer, DockingStyle dock, DockManager dockManager) {
			this.tabText = string.Empty;
			this.hint = string.Empty;
			this.mouseHandler = CreateMouseHandler();
			this.dockLayout = new DockLayout(dock, this);
			this.appearance = new AppearanceObject();
			this.appearance.Changed += new EventHandler(Appearance_Changed);
			this.options = CreateOptions();
			this.options.Changed += Options_Changed;
			this.id = Guid.NewGuid();
			this.setBoundsCounter = this.lockActivateCounter = 0;
			this.buttonsPanelCore = CreateButtonsPanel();
			this.buttonsPanelCore.ButtonInterval = 2;
			this.customHeaderButtonsCore = new ButtonCollection(ButtonsPanel);
			ButtonsPanel.Buttons.AddRange(new IButton[] { new CloseButton(), new PinButton(), new MaximizeButton() });
			SubscribeEmbeddedButtonsPanel();
			base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			if(createControlContainer) {
				CreateControlContainer();
			}
			SetDockManager(dockManager);
			this.SizeChanged +=DockPanel_SizeChanged;
			footerCore = string.Empty;
			headerCore = string.Empty;
			allowBorderColorBlendingCore = false;
		}
		protected override void DisposeCore() {
			Docking2010.Ref.Dispose(ref notificationSourceCore);
			if(DockManager != null) {
				if(DockManager.DocumentManager != null)
					DockManager.DocumentManager.CheckDocumentSelectorVisibility();
			}
			DockLayout.Dispose();
			SetDockManager(null);
			if(FloatForm != null && !FloatForm.IsDisposed)
				FloatForm.Dispose();
			this.SizeChanged -= DockPanel_SizeChanged;
			this.options.Changed -= Options_Changed;
			this.appearance.Changed -= Appearance_Changed;
			UnsubscribeEmbeddedButtonsPanel();
			this.customHeaderButtonsCore = null;
			this.appearance.Dispose();
			this.buttonsPanelCore = null;
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			EnsureLayeredWindowNotificationSource();
		}
		protected override void OnHandleDestroyed(EventArgs e) {
			base.OnHandleDestroyed(e);
			DestroyLayeredWindowNotificationSource();
		}
		DevExpress.Utils.Internal.ILayeredWindowNotificationSource notificationSourceCore;
		void EnsureLayeredWindowNotificationSource() {
			if(notificationSourceCore == null)
				notificationSourceCore = DevExpress.Utils.Internal.LayeredWindowNotificationSource.Register(Handle, true);
		}
		void DestroyLayeredWindowNotificationSource() {
			Docking2010.Ref.Dispose(ref notificationSourceCore);
		}
		protected override int GetZIndex() {
			var autoHideContainer = Parent as AutoHideContainer;
			if(autoHideContainer == null && Parent is AutoHideControl)
				autoHideContainer = (Parent as AutoHideControl).SavedParent as AutoHideContainer;
			if(autoHideContainer != null && autoHideContainer.AutoHideInfo != null)
				return autoHideContainer.AutoHideInfo.IndexOf(DockLayout);
			return base.GetZIndex();
		}
		void DockPanel_SizeChanged(object sender, EventArgs e) {
			if(DockManager != null && DockManager.needUpdateControlsZOrder) {
				DockManager.UpdateControlsZOrder(); 
				DockManager.needUpdateControlsZOrder = false;
			}
		}
		UserLookAndFeel ILookAndFeelProvider.LookAndFeel {
			get { return (DockManager != null) ? DockManager.LookAndFeel : null; }
		}
		protected virtual ButtonsPanel CreateButtonsPanel() {
			return new ButtonsPanel(this);
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("DockPanelBindingContext")]
#endif
		public override BindingContext BindingContext {
			get {
				if(this.DockManager == null || DockManager.Form == null) return base.BindingContext;
				try {
					return DockManager.Form.BindingContext;
				}
				catch {
					return null;
				}
			}
		}
		protected virtual DockPanelMouseHandler CreateMouseHandler() {
			return new DockPanelMouseHandler(this);
		}
		protected virtual void CreateControlContainer() {
			this.Controls.Add(new ControlContainer());
		}
		protected virtual DockPanelOptions CreateOptions() {
			return new DockPanelOptions(this);
		}
		protected internal virtual void OnAddIntoAutoHideContainer() {
			LockActivate();
			try {
				ControlVisible = false;
			}
			finally {
				UnlockActivate();
			}
		}
		protected override void OnControlAdded(ControlEventArgs e) {
			base.OnControlAdded(e);
			if(!(e.Control is DockPanel)) {
				if(ControlContainer != null) {
					if(e.Control != ControlContainer) {
						e.Control.Parent = ControlContainer;
					}
				}
				else if(Count > 0)
					e.Control.Parent = this[0];
			}
		}
		protected override void OnControlRemoved(ControlEventArgs e) {
			base.OnControlRemoved(e);
			DockPanel panel = e.Control as DockPanel;
			if(panel != null)
				panel.UpdateControlVisible(true); 
		}
		protected override void OnCreateControl() {
			base.OnCreateControl();
			if(!IsDisposing && DockManager != null && !DockManager.Disposing) DockLayout.CalcViewInfo(); 
		}
		protected override void OnPaint(PaintEventArgs e) {
			if(DockLayout != null && DockLayout.DockManager != null && (DockLayout.DockManager.PaintingSuspended && Dock != DockingStyle.Float)) return;
			if(!IsDeserializing) {
				if(AllowBorderColorBlending) {
					using(SkinElementCustomColorizer colorizer = new SkinElementCustomColorizer(Appearance.BorderColor)) {
						DockLayout.Draw(e, Painter);
					}			
				}
				else
					DockLayout.Draw(e, Painter);
			}
			base.OnPaint(e);
		}
		protected internal Point ClickPoint { get { return clickPointCore; } }
#if DEBUGTEST
		internal void SendMessage(int eventId, EventArgs e) {
			switch(eventId) {
				case MouseEvents.WM_LBUTTONDOWN: OnMouseDown(e as MouseEventArgs); break;
				case MouseEvents.WM_LBUTTONUP: OnMouseUp(e as MouseEventArgs); break;
				case MouseEvents.WM_MOUSEMOVE: OnMouseMove(e as MouseEventArgs); break;
				case MouseEvents.WM_MOUSELEAVE: OnMouseLeave(e as EventArgs); break;
			}
		}
#endif
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			Point point = new Point(e.X, e.Y);
			if(GetHitInfo(point).HitTest == HitTest.Tab) clickPointCore = Point.Empty;
			else clickPointCore = new Point(e.X, e.Y);
			MouseHandler.MouseDown(e);
			if(ButtonsPanel != null)
				ButtonsPanel.Handler.OnMouseDown(e);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			MouseHandler.MouseMove(e);
			if(ButtonsPanel != null)
				ButtonsPanel.Handler.OnMouseMove(e);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			if(ButtonsPanel != null) {
				if(DesignMode) {
					MouseEventArgs es = new MouseEventArgs(MouseButtons.Left, e.Clicks, e.X, e.Y, e.Delta);
					ButtonsPanel.Handler.OnMouseUp(es);
				}
				else
					ButtonsPanel.Handler.OnMouseUp(e);
			}
			MouseHandler.MouseUp(e);
		}
		protected override void OnDoubleClick(EventArgs e) {
			base.OnDoubleClick(e);
			MouseHandler.DoubleClick();
		}
		protected override void OnMouseEnter(EventArgs e) {
			ToolTipController.AddClientControl(this);
			base.OnMouseEnter(e);
			MouseHandler.MouseEnter(PointToClient(Cursor.Position));
		}
		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			MouseHandler.MouseLeave();
			ToolTipController.RemoveClientControl(this);
			if(ButtonsPanel != null)
				ButtonsPanel.Handler.OnMouseLeave();
		}
		protected override void OnDragOver(DragEventArgs drgevent) {
			base.OnDragOver(drgevent);
			HitInfo hi = GetHitInfo(PointToClient(new Point(drgevent.X, drgevent.Y)));
			if(hi.HitTest == HitTest.Tab && hi.Tab != null)
				ActiveChild = hi.Tab;
			if(hi.HitTest == HitTest.Client && ActiveChild != null)
				ActiveChild.RaiseBaseDragOver(drgevent);
		}
		protected override void OnDragDrop(DragEventArgs drgevent) {
			base.OnDragDrop(drgevent);
			HitInfo hi = GetHitInfo(PointToClient(new Point(drgevent.X, drgevent.Y)));
			if(hi.HitTest == HitTest.Client && ActiveChild != null)
				ActiveChild.RaiseBaseDragDrop(drgevent);
		}
		internal void RaiseBaseDragOver(DragEventArgs drgevent) {
			base.OnDragOver(drgevent);
		}
		ObjectPainter IButtonsPanelOwner.GetPainter() {
			return Painter.ButtonsPanelPainter;
		}
		bool IButtonsPanelOwner.IsSelected {
			get {
				bool result = CanDrawCaptionActive;
				if(DockManager != null && DockManager.ActivePanel != null) {
					for(Control control = DockManager.ActivePanel; control != null; control = control.Parent) {
						if(control == this && DockManager.ActivePanel.ContainsFocus)
							result = true;
					}
				}
				if(this.Dock == DockingStyle.Float && DockManager != null)
					result = FloatForm == Form.ActiveForm && DockManager.CanActivateFloatForm;
				if(!(Painter is DockElementsSkinPainter) && this.Dock == DockingStyle.Float)
					result = true;
				return result;
			}
		}
		void OnEmbeddedButtonChecked(object sender, ButtonEventArgs e) {
			if(e.Button is DefaultButton) return;
			ButtonEventHandler handler = GetButtonEventHandler(customButtonChecked);
			if(handler != null) handler(this, e);
		}
		void OnEmbeddedButtonUnchecked(object sender, ButtonEventArgs e) {
			if(e.Button is DefaultButton) return;
			ButtonEventHandler handler = GetButtonEventHandler(customButtonUnchecked);
			if(handler != null) handler(this, e);
		}
		void OnEmbeddedButtonClick(object sender, ButtonEventArgs e) {
			if(e.Button is DefaultButton) return;
			ButtonEventHandler handler = GetButtonEventHandler(customButtonClick);
			if(handler != null) handler(this, e);
		}
		protected ButtonEventHandler GetButtonEventHandler(object key) {
			ButtonEventHandler result = (ButtonEventHandler)this.Events[key];
			if(result == null && ActiveChild != null && ActiveChild.IsTab)
				result = (ButtonEventHandler)ActiveChild.Events[customButtonClick];
			return result;
		}
		internal void RaiseBaseDragDrop(DragEventArgs drgevent) {
			base.OnDragDrop(drgevent);
		}
		ToolTipController ToolTipController {
			get {
				if(DockManager != null && DockManager.ToolTipController != null) return DockManager.ToolTipController;
				return ToolTipController.DefaultController;
			}
		}
		bool IToolTipControlClient.ShowToolTips {
			get {
				if(DesignMode) return false;
				return (State == DockPanelState.Regular);
			}
		}
		ToolTipControlInfo IToolTipControlClient.GetObjectInfo(Point point) {
			HitInfo hi = DockLayout.GetHitInfo(point);
			switch(hi.HitTest) {
				case HitTest.Caption:
					return new ToolTipControlInfo(this, Tabbed && ActiveChild != null ? ActiveChild.Hint : Hint);
				case HitTest.Tab:
					return new ToolTipControlInfo(hi.Tab, hi.Tab.Hint);
				case HitTest.EmbeddedButtonPanel:
					if(ButtonsPanel != null && ButtonsPanel.ShowToolTips)
						return ButtonsPanel.GetObjectInfo(point);
					break;
			}
			return null;
		}
		protected void SubscribeEmbeddedButtonsPanel() {
			ButtonsPanel.Changed += EmbeddedButtonPanelChanged;
			ButtonsPanel.ButtonChecked += OnEmbeddedButtonChecked;
			ButtonsPanel.ButtonUnchecked += OnEmbeddedButtonUnchecked;
			ButtonsPanel.ButtonClick += OnEmbeddedButtonClick;
			ButtonsPanel.ButtonClick += OnDefaultButtonClick;
			ButtonsPanel.ButtonChecked += OnDefaultButtonClick;
			ButtonsPanel.ButtonUnchecked += OnDefaultButtonClick;
			CustomHeaderButtons.CollectionChanged += ButtonsPanel.OnButtonsCollectionChanged;
		}
		protected void UnsubscribeEmbeddedButtonsPanel() {
			CustomHeaderButtons.CollectionChanged -= ButtonsPanel.OnButtonsCollectionChanged;
			ButtonsPanel.Changed -= EmbeddedButtonPanelChanged;
			ButtonsPanel.ButtonChecked -= OnEmbeddedButtonChecked;
			ButtonsPanel.ButtonUnchecked -= OnEmbeddedButtonUnchecked;
			ButtonsPanel.ButtonClick -= OnEmbeddedButtonClick;
			ButtonsPanel.ButtonClick -= OnDefaultButtonClick;
			ButtonsPanel.ButtonChecked -= OnDefaultButtonClick;
			ButtonsPanel.ButtonUnchecked -= OnDefaultButtonClick;
		}
		protected override void OnLostFocus(EventArgs e) {
			base.OnLostFocus(e);
			if(!MouseHandler.IsFocusLocked && !ContainsFocus)
				MouseHandler.Reset();
		}
		protected override void OnLostCapture() {
			if(DesignMode) return;
			ResetDragging(false);
		}
		protected internal bool IsFocusLocked { get { return DesignMode; } }
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			InternalKeyDown(e);
		}
		protected override void OnKeyUp(KeyEventArgs e) {
			base.OnKeyUp(e);
			InternalKeyUp(e);
		}
		void OnDefaultButtonClick(object sender, ButtonEventArgs e) {
			IButton button = e.Button;
			if(!(button is DefaultButton)) return;
			if(button is PinButton)
				OnClickAutoHideButton();
			if(button is CloseButton)
				OnClickCloseButton();
			if(button is MaximizeButton)
				OnClickMaximizeButton();
		}
		protected virtual void OnClickMaximizeButton() {
			if(Dock == DockingStyle.Float) {
				FloatForm.CheckMaximizedBounds();
				FloatForm.WindowState = FloatForm.WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;
			}
			else {
				if(ParentPanel.ActiveChild == this)
					ParentPanel.ActiveChild = null;
				else
					ParentPanel.ActiveChild = this;
			}
		}
		protected virtual void OnClickAutoHideButton() {
			DockLayout dockLayout = DockLayout;
			if(dockLayout.IsTab && dockLayout.LayoutParent.AutoHide)
				dockLayout = dockLayout.LayoutParent;
			DockManager.UseLockWindowUpdate = true;
			dockLayout.AutoHide = !dockLayout.AutoHide;
			DockManager.UseLockWindowUpdate = false;
		}
		protected bool CanCloseActiveFloatTabOnly() {
			return DockManager.DockingOptions.CloseActiveFloatTabOnly && RootPanel.FloatForm != null;
		}
		protected bool CanCloseActiveTabOnly() {
			return DockManager.DockingOptions.CloseActiveTabOnly && RootPanel.FloatForm == null;
		}
		protected virtual void OnClickCloseButton() {
			if(Tabbed && (CanCloseActiveFloatTabOnly() || CanCloseActiveTabOnly())) {
				bool hideImmediately = (ParentAutoHideControl != null);
				DockLayoutManager layoutManager = DockManager.LayoutManager;
				DockPanel child = ActiveChild;
				child.Close();
				if(hideImmediately && child.ParentPanel == null)
					layoutManager.HideImmediately();
			}
			else
				Close();
		}
		protected internal bool InternalKeyDown(KeyEventArgs e) {
			bool result = false;
			if(e.KeyCode == Keys.Escape && !e.Handled) {
				result = (State != ResetDragging(true));
			}
			CheckModifierKeys(e.KeyCode);
			return result;
		}
		protected virtual DockPanelState ResetDragging(bool reparentDragOperation) {
			DockPanelState state = State;
			MouseHandler.Reset();
			if(reparentDragOperation && FloatForm == null && DockManager != null && (DockManager.DocumentManager != null && DockManager.DocumentManager.IsStrategyValid))
				DockManager.DocumentManager.UIViewAdapter.DragService.ReparentDragOperation();
			if(!Options.AllowFloating && state != DockPanelState.Regular) {
				if(state == DockPanelState.Docking && IsMdiDocument)
					return state;
				Restore();
			}
			return state;
		}
		protected internal void InternalKeyUp(KeyEventArgs e) {
			CheckModifierKeys(e.KeyCode);
		}
		void CheckModifierKeys(Keys key) {
			if(key == Keys.ControlKey)
				MouseHandler.ModifierKey(key);
		}
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			if(Parent == null || IsSetBoundsInDocumentContainer) return;
			DockLayout.Panel_OnResize();
		}
		void Appearance_Changed(object sender, EventArgs e) {
			if(ControlContainer == null) return;
			ControlContainer.LayoutChanged();
			if(DockManager != null)
				ControlContainer_InitAppearance(ControlContainer.ViewAppearance);
		}
		void Options_Changed(object sender, BaseOptionChangedEventArgs e) {
			DockLayout.LayoutChanged();
			if(IsMdiDocument && DockManager != null && DockManager.DocumentManager != null) {
				var document = DockManager.DocumentManager.GetDocument(this);
				if(document != null)
					document.InitializePropertiesFromDockPanel(this);
			}
		}
		internal int lockButtonsChanged;
		void EmbeddedButtonPanelChanged(object sender, EventArgs e) {
			if(lockButtonsChanged > 0) return;
			if(this.DockLayout != null)
				this.DockLayout.LayoutChanged();
		}
		int dockingInitialization;
		protected internal bool IsDockingInitialization {
			get { return dockingInitialization > 0; }
		}
		protected internal void InitDocking(Point screenPoint) {
			Docking.HitInfo hi = GetHitInfo(PointToClient(screenPoint));
			dockingInitialization++;
			MouseHandler.SetState(Docking.DockPanelState.Docking, hi);
			dockingInitialization--;
		}
		internal Docking2010.DocumentContainer parentContainer;
		protected internal void SetParent(Control parent) {
			if(Visibility == DockVisibility.AutoHide && parent == null) return;
			CheckMDIClient();
			bool containsFocus = ((DockManager == null || DockManager.Form == null) ? false : DockManager.FormContainsFocus);
			if(!IsDockingInitialization) {
				Docking2010.DocumentContainer container = Parent as Docking2010.DocumentContainer;
				if((parent == null || parent is AutoHideContainer) && container != null)
					parentContainer = container;
				Parent = parent;
			}
			if(containsFocus && DockLayout != null && !DockLayout.Disposing) {
				if(DockManager.ActivePanel != null)
					DockManager.ActivePanel.OnActivate();
				else
					DockManager.Form.Focus(); 
			}
		}
		internal void CheckMDIClient() {
			if(DockManager == null || !ContainsFocus || !DockManager.IsOwnerFormMDIContainer) return;
			Form ownerForm = DockManager.OwnerForm;
			for(int i = 0; i < ownerForm.Controls.Count; i++) {
				MdiClient mdiCLient = ownerForm.Controls[i] as System.Windows.Forms.MdiClient;
				if(mdiCLient != null) {
					mdiCLient.Select();
					break;
				}
			}
		}
		internal void SwapContent(DockPanel panel) {
			if(Parent == panel) {
				panel.SwapContent(this);
				return;
			}
			LockSetBounds();
			panel.LockSetBounds();
			try {
				SwapContentCore(panel);
			}
			finally {
				UnlockSetBounds();
				panel.UnlockSetBounds();
			}
		}
		protected virtual void SwapContentCore(DockPanel panel) {
			DockLayout tmpLayout = panel.DockLayout;
			int tmpZIndex = ZIndex;
			Point tmpFloatLocation = FloatLocation;
			DockStyle tmpDock = ControlDock;
			panel.DockLayout = DockLayout;
			panel.SetParent(Parent);
			panel.ControlDock = tmpDock;
			ControlDock = DockStyle.None;
			DockLayout = tmpLayout;
			DockLayout.FloatLocation = tmpFloatLocation;
			SetParent(panel);
			panel.ZIndex = tmpZIndex;
			panel.UpdateControlVisible(panel.Visible);
			UpdateControlVisible(Visible);
		}
		protected internal virtual void UpdateControlVisible(bool value) {
			if(!Visible)
				ControlVisible = false;
			else {
				if(IsDisposing && value && !ControlVisible)
					return;
				ControlVisible = value;
			}
		}
		protected internal void UpdateChildPanelsVisibiblity() {
			if(IsDeserializing && Visibility != DockVisibility.AutoHide) return;
			UpdateChildPanelsVisibiblityCore();
		}
		protected virtual void UpdateChildPanelsVisibiblityCore() {
			for(int i = 0; i < Count; i++) {
				this[i].UpdateControlVisible(Tabbed ? i == ActiveChildIndex : true);
			}
		}
		protected internal virtual DockPanel GetPanelAtPos(Point ptScreen) {
			if(Tabbed && HasChildren) {
				if(ActiveChild != null && ActiveChild.ClientRectangle.Contains(ActiveChild.PointToClient(ptScreen)))
					return ActiveChild.GetPanelAtPos(ptScreen);
			}
			else {
				for(int i = 0; i < Count; i++) {
					DockPanel panel = this[i];
					if(panel.ClientRectangle.Contains(panel.PointToClient(ptScreen)))
						return panel.GetPanelAtPos(ptScreen);
				}
			}
			return this;
		}
		protected internal virtual void ControlContainer_InitAppearance(AppearanceObject controlContainerAppearance) {
			DockLayout.InitControlContainerAppearance(controlContainerAppearance, Appearance);
		}
		protected internal virtual void ControlContainer_Activate() {
			if(IsDeserializing || ParentAutoHideContainer != null || IsActivateLocked) return;
			if(IsTab && ParentPanel != null && (ParentPanel.ActiveChild != this || ParentPanel.IsActivateLocked)) return;
			DockManager.ActivePanel = this;
		}
		protected internal virtual void ControlContainer_Disposing() {
			if(!IsDisposing)
				Dispose();
		}
		public override HitInfo GetHitInfo(Point ptClient) { return DockLayout.GetHitInfo(ptClient); }
		protected internal DockLayout DockLayout {
			get { return dockLayout; }
			set {
				dockLayout = value;
				CheckOptions();
			}
		}
		protected bool IsDeserializing {
			get {
				if(DockManager == null) return true;
				return DockManager.IsDeserializing;
			}
		}
		public bool HasAsParent(DockPanel panel) {
			if(panel == null) return false;
			return DockLayout.HasAsParent(panel.DockLayout);
		}
		public bool HasAsChild(DockPanel panel) {
			if(panel == null) return false;
			return panel.HasAsParent(this);
		}
		public virtual void Close() {
			DockPanelCancelEventArgs e = new DockPanelCancelEventArgs(this);
			RaiseClosingPanel(e);
			if(e.Cancel) return;
			if(DockManager != null) {
				if(DockManager.DocumentManager != null)
					DockManager.DocumentManager.CheckDocumentSelectorVisibility();
				var dockController = DockManager.DockController as Controller.DockController;
				if(dockController != null)
					dockController.RegisterSavedDock(this);
			}
			HideCore();
			RaiseClosedPanel();
		}
		public new void Hide() {
			if(RootPanel.Visibility == DockVisibility.AutoHide) {
				if(DockManager != null && RootPanel == DockManager.ActivePanel)
					DockManager.ActivePanel = null;
			}
			else {
				HideCore();
			}
		}
		void HideCore() {
			DockPanel parentPanel = Parent as DockPanel;
			if(parentPanel != null) this.DockLayout.OriginalSize = parentPanel.OriginalSize;
			Visibility = DockVisibility.Hidden;
			if(Visibility == DockVisibility.Hidden && ControlContainer != null && DockManager != null) {
				DockManager.ResetFormActiveControl(ControlContainer);
			}
			if(notificationSourceCore != null && IsHandleCreated)
				notificationSourceCore.NotifyHidden(Handle);
		}
		public virtual void ShowSliding() {
			if(DockManager == null) return;
			if(ParentPanel != null)
				ParentPanel.ShowSlidingCore(DockLayout);
			else ShowSlidingCore(DockLayout);
		}
		protected void ShowSlidingCore(DockLayout layout) {
			if(ParentAutoHideContainer != null) {
				DockManager.LayoutManager.HideImmediately();
				ParentAutoHideContainer.BeginShowSliding(layout);
			}
		}
		public virtual void HideSliding() {
			if(DockManager == null) return;
			DockManager.HideSliding(this);
		}
		public virtual void HideImmediately() {
			if(DockManager == null) return;
			if(RootPanel.Visibility == DockVisibility.AutoHide) {
				DockManager.LayoutManager.HideImmediately();
				return;
			}
			Hide();
		}
		public new void Show() {
			if(RootPanel.Visibility != DockVisibility.AutoHide)
				RootPanel.Visibility = DockVisibility.Visible;
			if(DockManager != null)
				DockManager.ActivePanel = this;
			if(ParentPanel != null)
				ParentPanel.ActiveChild = this;
		}
		public DockPanel AddPanel() {
			if(IsMdiDocument) {
				DockManager.SuspendRedrawForm();
				DockPanel panel = DockManager.AddPanel(DevExpress.XtraBars.Docking.DockingStyle.Bottom);
				panel.Register(DockManager);
				panel.DockAsMdiDocument(this);
				DockManager.ResumeRedrawForm();
				return panel;
			}
			DockLayout dockLayout = DockLayout.AddLayout();
			return (dockLayout == null ? null : dockLayout.Panel);
		}
		public void RemovePanel(DockPanel panel) {
			if(panel == null || panel.Parent != this) return;
			panel.Dispose();
		}
		public void DockAsTab(DockPanel panel) {
			DockAsTab(panel, LayoutConsts.InvalidIndex);
		}
		public void DockAsTab(DockPanel panel, int index) {
			if(panel == null) return;
			if(panel.IsMdiDocument) {
				DockAsMdiDocument(panel);
				return;
			}
			DockLayout.DockAsTab(panel.DockLayout, index);
		}
		public void DockTo(DockPanel panel) {
			int index = (panel == null ? 0 : LayoutConsts.InvalidIndex);
			DockTo(panel, index);
		}
		public void DockTo(DockPanel panel, int index) {
			if(panel != null && panel.IsMdiDocument) {
				DockAsMdiDocument(panel);
				return;
			}
			DockToCore(panel == null ? null : panel.DockLayout, DockingStyle.Fill, index);
		}
		public void DockTo(DockPanel panel, DockingStyle dock, int index) {
			DockToCore(panel == null ? null : panel.DockLayout, dock, index);
		}
		public void DockTo(DockingStyle dock) {
			DockTo(dock, LayoutConsts.InvalidIndex);
		}
		internal void DockToObs(DockManager dockManager, DockingStyle dock) {
			DockToObs(dockManager, dock, LayoutConsts.InvalidIndex);
		}
		internal void DockToObs(DockManager dockManager, DockingStyle dock, int index) {
			DockToInternal(dockManager, dock, index);
		}
		[Obsolete()]
		public void DockTo(DockManager dockManager, DockingStyle dock) {
			DockToObs(dockManager, dock);
		}
		[Obsolete()]
		public void DockTo(DockManager dockManager, DockingStyle dock, int index) {
			DockToObs(dockManager, dock, index);
		}
		public void DockTo(DockPanel panel, DockingStyle dock) {
			if(dock == DockingStyle.Fill) {
				DockAsTab(panel);
				return;
			}
			DockLayout.DockTo(panel.DockLayout, dock);
		}
		protected bool AllowUnregisterChildPanels(DockManager dockManager) {
			return dockManager != null && dockManager.LayoutManager != DockLayout.Manager;
		}
		protected void UnregisterChildPanels(DockLayout currentLayout, DockManager newDM) {
			if(currentLayout.Panel != null && currentLayout.Panel.DockManager != newDM) currentLayout.Panel.Unregister();
			foreach(DockLayout tempLayout in currentLayout) {
				UnregisterChildPanels(tempLayout, newDM);
			}
		}
		protected void RegisterChildPanels(DockLayout currentLayout, DockManager newDM) {
			if(currentLayout.Panel != null) currentLayout.Panel.Register(newDM);
			foreach(DockLayout tempLayout in currentLayout) {
				RegisterChildPanels(tempLayout, newDM);
			}
		}
		protected void DockToInternal(DockManager dockManager, DockingStyle dock, int index) {
			DockLayoutManager layoutManager = (dockManager == null ? DockLayout.Manager : dockManager.LayoutManager);
			bool shouldProcessChildren = AllowUnregisterChildPanels(dockManager);
			if(layoutManager == null && DockLayout != null && DockLayout.Visible)
				layoutManager = (DockManager != null ? DockManager.LayoutManager : null);
			DockToCore(layoutManager, dock, index);
			if(shouldProcessChildren) {
				UnregisterChildPanels(DockLayout, dockManager);
				RegisterChildPanels(DockLayout, dockManager);
			}
		}
		public void DockTo(DockingStyle dock, int index) {
			DockToInternal(null, dock, index);
		}
		protected virtual void DockToCore(BaseLayoutInfo info, DockingStyle dock, int index) {
			if(info == null)
				MakeFloat();
			else {
				DockLayout.DockTo(info, dock, index);
				CheckOptions();
			}
		}
		Point GetFloatLocation() {
			if(DockLayout.FloatLocation == Point.Empty) return PointToScreen(Point.Empty);
			return DockLayout.FloatLocation;
		}
		[Browsable(false)]
		public override Size MaximumSize {
			get { return Size.Empty; }
			set { }
		}
		[Browsable(false)]
		public override Size MinimumSize {
			get { return Size.Empty; }
			set { }
		}
		public void MakeFloat() {
			MakeFloat(FloatForm == null ? GetFloatLocation() : FloatForm.Location);
		}
		public void MakeFloat(Point ptScreen) {
			DockLayout.MakeFloat(ptScreen);
			if(FloatForm != null)
				FloatForm.Location = DockLayout.Location;
			CheckOptions();
		}
		public virtual void Restore() {
			if(DockedAsTabbedDocument) {
				var view = DockManager.DocumentManager.View;
				var document = view.Documents.FindFirst(doc => doc.GetDockPanel() == this);
				if(document != null) {
					DockManager.DocumentManager.RemoveDocumentFromHost(document);
					document.Dispose();
				}
			}
			DockLayout.Restore();
		}
		protected internal virtual void MakeFloatCore() {
			if(FloatForm != null || DockLayout.IsDockingAsTab || Parent is Docking2010.DocumentContainer) return;
			FloatForm floatForm = EnsureFloatForm();
			DockManager.MakeFloatFormVisible(floatForm);
		}
		protected internal FloatForm EnsureFloatForm(bool isMdiChild) {
			if(isMdiChild) {
				try {
					forceMdiChild++;
					return EnsureFloatForm();
				}
				finally { forceMdiChild--; }
			}
			else return EnsureFloatForm();
		}
		protected FloatForm EnsureFloatForm() {
			if(FloatForm != null) return FloatForm;
			FloatForm floatForm = DockManager.CreateFloatForm();
			floatForm.FloatLayout = DockLayout;
			if(forceMdiChild == 0)
				floatForm.ClientSize = (DockLayout == null ? DockConsts.DefaultFloatFormSize : DockLayout.FloatSize);
			else
				floatForm.ClientSize = Size;
			floatForm.Controls.Add(this);
			floatForm.Location = DockLayout.Location;
			if(DockManager.OwnerForm != null)
				DockManager.OwnerForm.AddOwnedForm(floatForm);
			if(DockManager.OwnerForm == null && DockManager.Form != null && DockManager.IsDesignMode)
				DevExpress.Utils.Drawing.Helpers.NativeMethods.SetWindowLong(floatForm.Handle, BarNativeMethods.GWL_HWNDPARENT, DockManager.Form.Handle);
			ControlDock = DockStyle.Fill;
			AdjustControlContainerBoundsOnMakeFloat();
			return floatForm;
		}
		protected internal virtual void UpdateFloatFormVisibility() {
			if(FloatForm == null) return;
			if(Visibility == DockVisibility.Visible)
				FloatForm.Show();
			else
				FloatForm.Hide();
		}
		void AdjustControlContainerBoundsOnMakeFloat() {
			if(ControlContainer == null || DockLayout == null) return;
			DockLayout.LockAdjustBounds();
			try {
				ControlContainer.Bounds = DockLayout.GetClientBoundsOnFloating();
			}
			finally {
				DockLayout.UnlockAdjustBounds();
			}
		}
		void CheckOptions() {
			Options.SetOptionByDockingStyle(Dock);
		}
		protected internal virtual void OnDeserialize(BaseLayoutInfo info) {
			AssignDockManager(info);
			if(Visibility == DockVisibility.Visible) {
				if(!IsMdiDocument)
					ControlDock = DockLayoutUtils.ConvertToDockStyle(Dock, info);
			}
			else {
				if(SavedParent == null)
					DockLayout.SavedInfo.SavedParent = info;
			}
			if(DockLayout.Parent != null && DockLayout.Parent is DockLayoutManager || DockLayout.Parent == null) {
				if(originalSizeCore != Size.Empty)
					DockLayout.OriginalSize = originalSizeCore;
				else
					DockLayout.OriginalSize = this.Size;
			}
			else DockLayout.OriginalSize = this.Size;
			if(Controls.Count < 2) return;
			for(int i = 0; i < Controls.Count; i++) {
				DockPanel tempPanel = (DockPanel)Controls[i];
				DockLayout.AddLayoutCore(tempPanel.DockLayout);
			}
			DockLayout.CalcViewInfo();
			for(int i = 0; i < Controls.Count; i++)
				((DockPanel)Controls[i]).OnDeserialize(DockLayout);
			UpdateChildPanelsVisibiblityCore();
			DockLayout.OnDeserialized();
			DockLayout.CalcViewInfo();
			if(DockLayout != null && DockLayout.Count > 0) {
				AutoScroll = false;
			}
		}
		protected internal virtual void BeforeUpdateLayoutOnResize() {
			if(!IsDockManagerFormParent) return;
			DockManager.BeginFormLayoutUpdate();
		}
		protected internal virtual void AfterUpdateLayoutOnResize() {
			if(!IsDockManagerFormParent) return;
			DockManager.EndFormLayoutUpdate();
		}
		bool IsDockManagerFormParent {
			get {
				if(DockManager == null || DockManager.Form == null) return false;
				return (DockManager.Form == Parent);
			}
		}
		internal void AssignDockManager(BaseLayoutInfo info) {
			if(info is DockLayoutManager)
				SetDockManager(((DockLayoutManager)info).DockManager);
			else
				SetDockManager(((DockLayout)info).Panel.DockManager);
		}
		internal void UndockOnRestoreLayout() {
			if(DockManager != null && DockManager.ActivePanel == this)
				DockManager.ActivePanel = null;
			Control parent = Parent;
			Parent = null;
			FloatForm ff = parent as FloatForm;
			if(ff != null) {
				ff.Controls.Clear();
				Form[] formArray = (Form[])ff.OwnedForms;
				for(int i = formArray.Length - 1; i >= 0; i--) {
					if(formArray[i] != null) formArray[i].Owner = null;
				}
				ff.Dispose();
			}
			Docking2010.DocumentContainer dc = parent as Docking2010.DocumentContainer ?? parentContainer;
			if(dc != null) {
				dc.Controls.Clear();
				dc.Dispose();
			}
			DockLayout.UndockOnRestoreLayout();
			UpdateControlVisible(true);
		}
		void UndockChildrenOnRestoreLayout() {
			for(int i = Count - 1; i > -1; i--)
				this[i].UndockOnRestoreLayout();
		}
		internal void DestoryOnRestoreLayout() {
			UndockChildrenOnRestoreLayout();
			UndockOnRestoreLayout();
			Unregister();
		}
		protected internal void FireChanged() {
			if(DockManager != null)
				DockManager.FireChanged();
		}
		void LockSetBounds() { setBoundsCounter++; }
		void UnlockSetBounds() { setBoundsCounter--; }
		bool IsSetBoundsLocked { get { return (setBoundsCounter != 0); } }
		void LockActivate() { lockActivateCounter++; }
		void UnlockActivate() { lockActivateCounter--; }
		bool IsActivateLocked { get { return (lockActivateCounter != 0); } }
		internal bool IsParentAutoScaling {
			get {
				if(!IsHandleCreated) return false;
				return !DockLayout.IsAdjustBoundsLocked;
			}
		}
		bool isAutoScaling = false;
		protected bool IsAutoScaling { get { return isAutoScaling; } }
		protected override void ScaleCore(float dx, float dy) {
			this.isAutoScaling = true;
			try {
				base.ScaleCore(dx, dy);
			}
			finally {
				this.isAutoScaling = false;
			}
		}
		internal bool allowChangeBounds = false;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static bool OptimizeBoundsCalculation = false;
		internal bool AllowChangeBounds {
			get {
				if(Parent == null) return true;
				if(!Visible) return true;
				Form parentForm = FindForm();
				if(parentForm != null && parentForm.WindowState == FormWindowState.Minimized) return false;
				if(DockManager != null && !OptimizeBoundsCalculation) return true;
				return allowChangeBounds && DockLayout.IsAdjustBoundsLocked;
			}
		}
		protected internal static bool IsActive(DockPanel panel) {
			if(panel == null || panel.IsMdiDocument)
				return false;
			return GetActiveDockPanel() == panel;
		}
		protected internal static bool IsActive() {
			DockPanel panel = GetActiveDockPanel();
			return (panel != null) && !panel.IsMdiDocument;
		}
		protected internal static DockPanel GetActiveDockPanel() {
			IntPtr focus = BarNativeMethods.GetFocus();
			Control focusedControl = Docking2010.WinAPIHelper.FindControl(focus);
			while(focusedControl != null) {
				if(focusedControl is DockPanel)
					break;
				if(focusedControl is FloatForm) {
					focusedControl = ((FloatForm)focusedControl).FloatLayout.Panel;
					break;
				}
				focusedControl = focusedControl.Parent;
			}
			return focusedControl as DockPanel;
		}
		public new Size Size {
			get { return base.Size; }
			set { allowChangeBounds = true; base.Size = value; allowChangeBounds = false; }
		}
		public new Point Location {
			get { return base.Location; }
			set { allowChangeBounds = true; base.Location = value; allowChangeBounds = false; }
		}
		public new Rectangle Bounds {
			get { return base.Bounds; }
			set {
				allowChangeBounds = true; base.Bounds = value; allowChangeBounds = false;
			}
		}
		internal int setBoundsInDocumentContainer = 0;
		protected bool IsSetBoundsInDocumentContainer {
			get { return setBoundsInDocumentContainer > 0; }
		}
		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {
			if(Parent is DocumentContainer || IsSetBoundsInDocumentContainer) {
				base.SetBoundsCore(x, y, width, height, specified);
				return;
			}
			if(DockLayout != null && DockLayout.Parent != null && !IsDeserializing && !IsDisposing && !IsAutoScaling && ParentAutoHideControl == null && ParentAutoHideContainer == null && DockLayout != null && !DockLayout.IsAdjustBoundsLocked && specified != BoundsSpecified.None) {
				Rectangle bounds = DockLayout.GetCorrectBounds(new Rectangle(x, y, width, height));
				x = bounds.Left;
				y = bounds.Top;
				width = bounds.Width;
				height = bounds.Height;
				if(DockLayout.Manager != null)
					DockLayout.Manager.CheckDecreaseSize();
			}
			if(DockManager == null || AllowChangeBounds)
				base.SetBoundsCore(x, y, width, height, specified);
			if(!IsDeserializing) OriginalSize = DockLayout.OriginalSize;
		}
		protected override void OnTextChanged(EventArgs e) {
			OnPanelTextChanged();
			base.OnTextChanged(e);
		}
		void CheckValidationCancelled() {
			if(DockManager != null && DockManager.IsOwnerFormMDIContainer) {
				PropertyInfo pi = typeof(ContainerControl).GetProperty("ValidationCancelled", BindingFlags.Instance | BindingFlags.NonPublic);
				if(pi != null && (bool)pi.GetValue(this, null))
					pi.SetValue(this, false, null);
			}
		}
		protected internal bool IsChanged { get; set; }
		protected virtual void OnDockedAsTabbedDocumentChanged(BaseOptionChangedEventArgs args) {
			if(!IsDeserializing) {
				bool value = (bool)args.NewValue;
				if(value) DockAsTabbedDocument();
				else UndockFromTabbedDocument();
			}
		}
		void DockAsTabbedDocument() {
			DockAsMdiDocument();
		}
		void UndockFromTabbedDocument() {
			var view = DockManager.DocumentManager.View;
			var document = view.Documents.FindFirst(doc => doc.GetDockPanel() == this);
			if(document != null)
				view.Controller.Float(document);
		}
		protected internal virtual void OnActivate() {
			CheckValidationCancelled(); 
			IsChanged = false;
			if(IsTab)
				DockLayout.LayoutParent.ActiveChild = DockLayout;
			DockLayout.OnActivate();
			if(ControlContainer != null)
				ControlContainer.Activate();
			if(!ContainsFocus)
				Focus();
		}
		protected internal virtual void OnDeactivate(DockPanel newActivate) {
			if(IsDisposing) return;
			DockLayout.RefreshCaption();
			if(newActivate == null && ContainsFocus && DockManager.OwnerForm != null) {
				DockManager.ResetFocusedControl();
				DockManager.FocusDockFillControl();
			}
		}
		protected internal virtual bool ValidateOnSetInactive() {
			return Validate() || AllowFocusChangeOnValidation();
		}
		bool AllowFocusChangeOnValidation() {
			return (AutoValidate != System.Windows.Forms.AutoValidate.EnablePreventFocusChange);
		}
		int leaveCounter = 999999;
		protected override void OnLeave(EventArgs e) {
			if(DockManager != null && !Visible && leaveCounter-- <= 0) {
				MethodInfo mi = typeof(ContainerControl).GetMethod("ResetActiveAndFocusedControlsRecursive", BindingFlags.Instance | BindingFlags.NonPublic);
				if(mi != null) mi.Invoke(DockManager.Form, new object[] { });
			}
			if(DockManager != null && DockManager.ActivePanel == this)
				DockManager.ActivePanel = null;
			base.OnLeave(e);
		}
		void OnPanelTextChanged() { DockLayout.UpdateView(); }
		protected override Size DefaultSize { get { return DockConsts.DefaultDockPanelSize; } }
		DevExpress.Utils.Menu.IDXMenuManager DevExpress.Utils.Menu.IDXMenuManagerProvider.MenuManager {
			get { return (dockManager != null) ? dockManager.MenuManager : null; }
		}
		[Browsable(false)]
		public DockManager DockManager { get { return dockManager; } }
		[Browsable(false)]
		public void Register(DockManager value) {
			SetDockManager(value);
		}
		protected void Unregister() {
			SetDockManager(null);
			if(FloatForm != null) FloatForm.Hide();
		}
		protected virtual void SetDockManager(DockManager value) {
			if(DockManager == value) return;
			defaultBackColor = null;
			if(DockManager != null) {
				DockManager.LookAndFeel.StyleChanged -= new EventHandler(LookAndFeelStyleChanged);
				DockManager.UnregisterPanel(this);
			}
			bool backColorChanged = (dockManager == null && value != null);
			this.dockManager = value;
			if(DockManager != null) {
				DockManager.RegisterPanel(this);
				DockManager.LookAndFeel.StyleChanged += new EventHandler(LookAndFeelStyleChanged);
			}
			if(backColorChanged)
				OnBackColorChanged(EventArgs.Empty);
		}
		void LookAndFeelStyleChanged(object sender, EventArgs e) {
			defaultBackColor = null;
			if(ButtonsPanel != null && ButtonsPanel.Buttons != null)
				ButtonsPanel.UpdateStyle();
			OnBackColorChanged(EventArgs.Empty);
		}
		internal bool IsMouseOnTopEdge {
			get {
				if(!IsMouseInside) return false;
				Point ptMouse = PointToClient(MousePosition);
				return (ptMouse.Y < Painter.GetCaptionHeight(DockLayout.CaptionAppearance, DockLayout.Float));
			}
		}
		[Browsable(false), XtraSerializableProperty()]
		public int Count { get { return (DockLayout == null ? 0 : DockLayout.Count); } }
#if !SL
	[DevExpressXtraBarsLocalizedDescription("DockPanelItem")]
#endif
		public DockPanel this[int index] { get { return dockLayout[index].Panel; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int Index { get { return DockLayout.Index; } set { DockLayout.Index = value; } }
		[Browsable(false), DefaultValue(null)]
		public DockPanel SavedParent {
			get { return DockLayout.GetSavedInfo().SavedParent; }
			set {
				SavedDockPanelInfo savedInfo = DockLayout.GetSavedInfo();
				savedInfo.SavedParent = value;
				DockLayout.SetSavedInfo(savedInfo);
			}
		}
		[Browsable(false), DefaultValue(DockingStyle.Float), XtraSerializableProperty()]
		public DockingStyle SavedDock {
			get { return DockLayout.GetSavedInfo().SavedDock; }
			set {
				if(value == DockingStyle.Float && Visibility == DockVisibility.Visible) return;
				SavedDockPanelInfo savedInfo = DockLayout.GetSavedInfo();
				savedInfo.SavedDock = value;
				DockLayout.SetSavedInfo(savedInfo);
			}
		}
		[Browsable(false), DefaultValue(LayoutConsts.InvalidIndex), XtraSerializableProperty()]
		public int SavedIndex {
			get { return DockLayout.GetSavedInfo().SavedIndex; }
			set {
				if(value == LayoutConsts.InvalidIndex) return;
				SavedDockPanelInfo savedInfo = DockLayout.GetSavedInfo();
				savedInfo.SavedIndex = value;
				DockLayout.SetSavedInfo(savedInfo);
			}
		}
		[Browsable(false), DefaultValue(false), XtraSerializableProperty()]
		public bool SavedTabbed {
			get { return DockLayout.GetSavedInfo().SavedTabbed; }
			set {
				if(!value) return;
				SavedDockPanelInfo savedInfo = DockLayout.GetSavedInfo();
				savedInfo.SavedTabbed = value;
				DockLayout.SetSavedInfo(savedInfo);
			}
		}
		[Browsable(false), DefaultValue(false), XtraSerializableProperty()]
		public bool SavedMdiDocument {
			get { return DockLayout.GetSavedInfo().SavedMdiDocument; }
			set {
				if(!value) return;
				SavedDockPanelInfo savedInfo = DockLayout.GetSavedInfo();
				savedInfo.SavedMdiDocument = value;
				DockLayout.SetSavedInfo(savedInfo);
			}
		}
		[Browsable(false)]
		public virtual bool CanActivate { get { return DockLayout.CanActivate; } }
		[Browsable(false)]
		public DockPanel ParentPanel { get { return (Parent as DockPanel); } }
		protected internal AutoHideControl ParentAutoHideControl { get { return (Parent as AutoHideControl); } }
		[Browsable(false)]
		public AutoHideContainer ParentAutoHideContainer { get { return (Parent as AutoHideContainer); } }
		[Browsable(false)]
		public DockPanel RootPanel { get { return (DockLayout.RootLayout.Panel); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int ActiveChildIndex { get { return DockLayout.ActiveChildIndex; } set { DockLayout.ActiveChildIndex = value; } }
		[Browsable(false), DefaultValue(null)]
		public DockPanel ActiveChild { get { return DockLayout.ActiveChild == null ? null : DockLayout.ActiveChild.Panel; } set { DockLayout.ActiveChild = (value == null ? null : value.DockLayout); } }
		[Browsable(false)]
		public FloatForm FloatForm { get { return (Parent as FloatForm); } }
		bool ShouldSerializeFloatSize() { return FloatSize != DockConsts.DefaultFloatFormSize; }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockPanelFloatSize"),
#endif
 XtraSerializableProperty(), Category(DockConsts.AppearanceCategory)]
		public Size FloatSize {
			get { return DockLayout.FloatSize; }
			set {
				DockLayout.FloatSize = value;
				if(CanChangeFloatFormLayout)
					FloatForm.ClientSize = FloatSize;
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockPanelFloatVertical"),
#endif
 DefaultValue(LayoutConsts.DefaultFloatVertical), XtraSerializableProperty(), Category(DockConsts.AppearanceCategory)]
		public bool FloatVertical { get { return DockLayout.FloatVertical; } set { DockLayout.FloatVertical = value; } }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockPanelDockVertical"),
#endif
 DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(), Category(DockConsts.AppearanceCategory)]
		public DefaultBoolean DockVertical { get { return DockLayout.DockVertical; } set { DockLayout.DockVertical = value; } }
		bool ShouldSerializeFloatLocation() { return (FloatLocation != Point.Empty); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockPanelFloatLocation"),
#endif
 XtraSerializableProperty(), Category(DockConsts.AppearanceCategory)]
		public Point FloatLocation {
			get {
				if(Dock != DockingStyle.Float) return Point.Empty;
				return DockLayout.Location;
			}
			set {
				DockLayout.Location = value;
				if(CanChangeFloatFormLayout)
					FloatForm.Location = FloatLocation;
			}
		}
		bool CanChangeFloatFormLayout { get { return (FloatForm != null && DockManager != null && DockManager.IsInitialized); } }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockPanelDock"),
#endif
 XtraSerializableProperty(), Category(DockConsts.AppearanceCategory)]
		public new DockingStyle Dock {
			get { return DockLayout.Dock; }
			set {
				DockLayout.Dock = value;
				UpdateDockAsTabbedDocument(value);
				CheckOptions();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockPanelHeader"),
#endif
 XtraSerializableProperty(), Category("Document Selector"),
		DefaultValue(""), Localizable(true)]
		public string Header {
			get { return headerCore; }
			set { headerCore = value; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockPanelFooter"),
#endif
 XtraSerializableProperty(), Category("Document Selector"),
		DefaultValue(""), Localizable(true)]
		public string Footer {
			get { return footerCore; }
			set { footerCore = value; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockPanelAllowBorderColorBlending"),
#endif
 XtraSerializableProperty(), Category(DockConsts.AppearanceCategory),
		DefaultValue(false), Localizable(true)]
		public bool AllowBorderColorBlending {
			get { return allowBorderColorBlendingCore; }
			set { allowBorderColorBlendingCore = value; }
		}
		[Browsable(false)]
		public int DockLevel { get { return DockLayout.Level; } }
		int lockVisibleChangedCounter = 0;
		internal void SuspendControlVisibleChanged() { this.lockVisibleChangedCounter++; }
		internal void ResumeControlVisibleChanged() {
			this.lockVisibleChangedCounter--;
			if(!IsControlVisibleChangedLocked && !ActivateOnContentFocusWhenAutoHidden)
				base.OnVisibleChanged(EventArgs.Empty);
		}
		bool IsControlVisibleChangedLocked {
			get {
				if(lockVisibleChangedCounter != 0) return true;
				if(IsDeserializing) return true;
				if(DesignMode && (Capture || (ParentPanel != null && ParentPanel.Capture))) return true; 
				return false;
			}
		}
		protected override void OnVisibleChanged(EventArgs e) {
			if(IsControlVisibleChangedLocked) return;
			base.OnVisibleChanged(e);
		}
		protected virtual void OnVisibleChanged() {
			ControlVisible = this.Visible;
		}
		protected internal DockPanelMouseHandler MouseHandler { get { return mouseHandler; } }
		[DefaultValue(null), Browsable(false)]
		public ControlContainer ControlContainer {
			get {
				if(Controls.Count > 0)
					return Controls[0] as ControlContainer;
				return null;
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockPanelTabbed"),
#endif
 DefaultValue(false), XtraSerializableProperty(), Category(DockConsts.AppearanceCategory)]
		public bool Tabbed { get { return DockLayout.Tabbed; } set { DockLayout.Tabbed = value; } }
		[Browsable(false)]
		public bool IsTab { get { return DockLayout.IsTab; } }
		string IDockPanelInfo.PanelName {
			get { return Name; }
		}
		internal bool dockedAsTabbedDocumentCore;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockPanelDockedAsTabbedDocument"),
#endif
 DefaultValue(false), XtraSerializableProperty(), Category(DockConsts.BehaviorCategory)]
		public bool DockedAsTabbedDocument {
			get { return dockedAsTabbedDocumentCore; }
			set {
				if(!SetDockedAsTabbedDocument(value)) return;
				OnDockedAsTabbedDocumentChanged(new BaseOptionChangedEventArgs("DockedAsTabbedDocument", !value, value));
			}
		}
		bool IDockPanelInfo.DockedAsMdiDocument {
			get { return DockedAsTabbedDocument && IsMdiDocument; }
		}
		void UpdateDockAsTabbedDocument(DockingStyle value) {
			if(invokeUpdateDockAsTabbedDocument && value != DockingStyle.Float) {
				dockedAsTabbedDocumentCore = false;
				OnDockedAsTabbedDocumentChanged(new BaseOptionChangedEventArgs("DockedAsTabbedDocument", true, false));
				invokeUpdateDockAsTabbedDocument = false;
			}
		}
		bool invokeUpdateDockAsTabbedDocument = false;
		bool SetDockedAsTabbedDocument(bool value) {
			invokeUpdateDockAsTabbedDocument = false;
			if(dockedAsTabbedDocumentCore == value || !CanUseDockedTabbedDocument()) return false;
			if(!Options.AllowFloating && !value && IsDeserializing)
				invokeUpdateDockAsTabbedDocument = true;
			if((value && Options.AllowDockAsTabbedDocument) || (Options.AllowFloating && !value)) {
				dockedAsTabbedDocumentCore = value;
				return true;
			}
			return false;
		}
		bool CanUseDockedTabbedDocument() {
			if(this.IsDeserializing) return true;
			if((DockManager == null) || (DockManager.DocumentManager == null)) return false;
			if(!DockManager.DocumentManager.IsStrategyValid) return false;
			if(DockManager.DocumentManager.View.IsDesignMode() && DockManager.DocumentManager.ContainerControl == null)
				return false;
			return DockManager.DocumentManager.View is Docking2010.Views.Tabbed.TabbedView;
		}
		int forceMdiChild = 0;
		[Browsable(false)]
		public bool IsMdiDocument {
			get { return (forceMdiChild > 0) || ((FloatForm != null) && FloatForm.IsMdiChild) || (Parent is Docking2010.DocumentContainer); }
		}
		protected bool DockAsMdiDocument(DockPanel destPanel) {
			if(DockManager == null || DockManager.DocumentManager == null) return false;
			if(DockManager.DocumentManager.View == null) return false;
			if(!Options.AllowDockAsTabbedDocument) return false;
			if(IsMdiDocument) return false;
			DockLayout layout = DockLayout;
			layout.BeginDockingAsTab();
			try {
				if(DockLayout.Dock == DockingStyle.Float && DockLayout.Parent == null)
					DockLayout.DockTo(null, DockingStyle.Float, -1);
				if(parentContainer != null && !parentContainer.IsDisposed) {
					using(Docking2010.BatchUpdate.Enter(DockManager.DocumentManager)) {
						using(DockManager.DocumentManager.View.LockPainting()) {
							SetParent(parentContainer);
							base.Dock = DockStyle.Fill;
							this.dockedAsTabbedDocumentCore = true;
							this.parentContainer = null;
							DockManager.DocumentManager.View.RequestInvokePatchActiveChild();
							return true;
						}
					}
				}
				DockManager.BeginUpdate();
				if(Visibility == DockVisibility.AutoHide)
					Visibility = DockVisibility.Visible;
				var saveAllowFloating = Options.AllowFloating;
				Options.AllowFloating = true;
				if(DockLayout.Dock != DockingStyle.Float)
					DockLayout.DockTo(null, DockingStyle.Float, -1);
				Options.AllowFloating = saveAllowFloating;
				DockManager.EndUpdate();
				if(destPanel != null && destPanel.IsMdiDocument && DockManager.DocumentManager.View is Docking2010.Views.Tabbed.TabbedView) {
					var tabbedView = DockManager.DocumentManager.View as Docking2010.Views.Tabbed.TabbedView;
					Control dockPanelControl = destPanel.FloatForm as Control ?? destPanel as Control;
					var document = DockManager.DocumentManager.GetDocument(dockPanelControl) as Docking2010.Views.Tabbed.Document;
					if(document != null) {
						var documentGroup = tabbedView.DocumentGroups.FindFirst(
							(Docking2010.Views.Tabbed.DocumentGroup doucmentGroup) => doucmentGroup.Contains(dockPanelControl));
						if(documentGroup != null)
							return tabbedView.Controller.Dock(this, documentGroup, documentGroup.Items.IndexOf(document) + 1);
					}
				}
				return DockManager.DocumentManager.View.Controller.Dock(this);
			}
			finally {
				parentContainer = null;
				layout.EndDockingAsTab();
			}
		}
		public bool DockAsMdiDocument() {
			return DockAsMdiDocument(null);
		}
		public bool DockAsMdiDocument(bool activate) {
			ActivateWhenDockingAsMdiDocument = activate;
			try { return DockAsMdiDocument(null); }
			finally { ActivateWhenDockingAsMdiDocument = true; }
		}
		internal bool ActivateWhenDockingAsMdiDocument = true;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockPanelTabsPosition"),
#endif
 DefaultValue(DockConsts.DefaultTabsPosition), XtraSerializableProperty(), Category(DockConsts.AppearanceCategory)]
		public TabsPosition TabsPosition { get { return DockLayout.TabsPosition; } set { DockLayout.TabsPosition = value; } }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockPanelTabsScroll"),
#endif
 DefaultValue(DockConsts.DefaultTabsScroll), XtraSerializableProperty(), Category(DockConsts.AppearanceCategory)]
		public bool TabsScroll { get { return DockLayout.TabsScroll; } set { DockLayout.TabsScroll = value; } }
		[Browsable(false)]
		public DockPanelState State { get { return MouseHandler.State; } }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockPanelImageIndex"),
#endif
 XtraSerializableProperty(), Category(DockConsts.AppearanceCategory),
		Editor(typeof(DevExpress.Utils.Design.ImageIndexesEditor), typeof(UITypeEditor)),
		DevExpress.Utils.ImageList("Images"), DevExpress.Utils.Design.SmartTagProperty("ImageIndex", "Appearance", SmartTagActionType.RefreshAfterExecute)]
		public int ImageIndex { get { return DockLayout.ImageIndex; } set { DockLayout.ImageIndex = value; } }
		bool ShouldSerializeImageIndex() {
			return DockLayout != null && DockLayout.ShouldSerializeImageIndex();
		}
		void ResetImageIndex() {
			if(DockLayout != null)
				DockLayout.ResetImageIndex();
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object Images {
			get {
				if(DockManager == null) return null;
				return DockManager.Images;
			}
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("DockPanelImage")]
#endif
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category(DockConsts.AppearanceCategory), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor)), DevExpress.Utils.Design.SmartTagProperty("Image", "Appearance", SmartTagActionType.RefreshAfterExecute)]
		public Image Image { get { return DockLayout.Image; } set { DockLayout.Image = value; } }
		bool ShouldSerializeImage() {
			return DockLayout != null && DockLayout.ShouldSerializeImage();
		}
		void ResetImage() {
			if(DockLayout != null)
				DockLayout.ResetImage();
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockPanelImageUri"),
#endif
 Category("Appearance"), DefaultValue(null)]
		[TypeConverter(typeof(ExpandableObjectConverter)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), SmartTagProperty("ImageUri","Appearance", SmartTagActionType.RefreshAfterExecute)]
		public DxImageUri ImageUri {
			get { return DockLayout.ImageUri; }
			set { DockLayout.ImageUri = value; }
		}
		bool ShouldSerializeImageUri() {
			return DockLayout != null && DockLayout.ShouldSerializeImageUri();
		}
		void ResetImageUri() {
			if(DockLayout != null)
				DockLayout.ResetImageUri();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		new public bool Visible {
			get { return DockLayout.Visible; }
			set {
				if(Visible == value) return;
				Visibility = (value ? DockVisibility.Visible : DockVisibility.Hidden);
				if(Visible == value)
					OnVisibleChanged();
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new event EventHandler VisibleChanged {
			add { base.VisibleChanged += value; }
			remove { base.VisibleChanged -= value; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockPanelVisibility"),
#endif
 DefaultValue(LayoutConsts.DefaultVisibility), XtraSerializableProperty(), Category(DockConsts.AppearanceCategory), DevExpress.Utils.Design.SmartTagProperty("Visibility", "Appearance")]
		public DockVisibility Visibility { get { return DockLayout.Visibility; } set { DockLayout.Visibility = value; } }
		void XtraDeserializeTabText(XtraEventArgs e) {
			if(DockManager == null) Text = e.Info.Value as String;
			else {
				if(DockManager.SerializationOptions.RestoreDockPanelsText) TabText = e.Info.Value as String;
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockPanelTabText"),
#endif
 DefaultValue(""), XtraSerializableProperty(), Category(DockConsts.AppearanceCategory), Localizable(true)]
		public virtual string TabText {
			get { return tabText; }
			set {
				if(value == null) value = string.Empty;
				if(TabText == value) return;
				tabText = value;
				OnPanelTextChanged();
			}
		}
		DefaultBoolean allowGlyphSkinning = DefaultBoolean.Default;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockPanelAllowGlyphSkinning"),
#endif
 XtraSerializableProperty(), DefaultValue(DefaultBoolean.Default), Category(DockConsts.AppearanceCategory)]
		public DefaultBoolean AllowGlyphSkinning {
			get { return allowGlyphSkinning; }
			set {
				if(AllowGlyphSkinning == value)
					return;
				allowGlyphSkinning = value;
				if(IsHandleCreated)
					Invalidate();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockPanelAllowCustomHeaderButtonsGlyphSkinning"),
#endif
 XtraSerializableProperty(), DefaultValue(DefaultBoolean.Default), Category(DockConsts.AppearanceCategory)]
		public DefaultBoolean AllowCustomHeaderButtonsGlyphSkinning {
			get { return DockLayout.AllowCustomHeaderButtonsGlyphSkinning; }
			set { DockLayout.AllowCustomHeaderButtonsGlyphSkinning = value; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockPanelHint"),
#endif
 DefaultValue(""), XtraSerializableProperty(), Category(DockConsts.AppearanceCategory), Localizable(true)]
		public virtual string Hint {
			get { return hint; }
			set {
				if(value == null) value = string.Empty;
				hint = value;
			}
		}
		void XtraDeserializeText(XtraEventArgs e) {
			if(DockManager == null) Text = e.Info.Value as String;
			else {
				if(DockManager.SerializationOptions.RestoreDockPanelsText) Text = e.Info.Value as String;
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockPanelText"),
#endif
 XtraSerializableProperty(), DevExpress.Utils.Design.SmartTagProperty("Text", "Appearance", SmartTagActionType.RefreshAfterExecute)]
		public override string Text { get { return base.Text; } set { base.Text = value; } }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockPanelBackColor"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color BackColor {
			get { return GetColor(Appearance.BackColor, DefaultPanelBackColor); }
			set { Appearance.BackColor = value; }
		}
		new void ResetBackColor() { Appearance.BackColor = Color.Empty; }
		bool ShouldSerializeBackColor() { return Appearance.BackColor != Color.Empty; }
		static Color GetColor(Color value, Color defaultColor) {
			return value == Color.Empty || value == Color.Transparent ? defaultColor : value;
		}
		Color? defaultBackColor;
		protected Color DefaultPanelBackColor {
			get {
				if(!defaultBackColor.HasValue) {
					if(DockManager != null && !IsDisposing) {
						if(ControlContainer != null) {
							ControlContainer_InitAppearance(ControlContainer.ViewAppearance);
							defaultBackColor = ControlContainer.ViewAppearance.BackColor;
						}
						else {
							defaultBackColor = LookAndFeelHelper.GetSystemColor(Painter.LookAndFeel, SystemColors.Control);
						}
					}
					else defaultBackColor = SystemColors.Control;
				}
				return defaultBackColor.Value;
			}
		}
		protected override void OnParentChanged(EventArgs e) {
			defaultBackColor = null;
			if(notificationSourceCore != null && IsHandleCreated)
				notificationSourceCore.NotityReparented(Handle);
			base.OnParentChanged(e);
		}
		protected override void OnBackColorChanged(EventArgs e) {
			defaultBackColor = null;
			base.OnBackColorChanged(e);
		}
		void ResetAppearance() { Appearance.Reset(); }
		bool ShouldSerializeAppearance() { return Appearance.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockPanelAppearance"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category(DockConsts.AppearanceCategory)]
		public AppearanceObject Appearance { get { return appearance; } }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockPanelOptions"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content), Category(DockConsts.DockingCategory)]
		public DockPanelOptions Options { get { return options; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), XtraSerializableProperty()]
		public int XtraParentID {
			get {
				int id = GetXtraParentId();
				if(IsMdiDocument)
					id += DockConsts.MdiContainerParentID;
				return id;
			}
		}
		int GetXtraParentId() {
			if(ParentAutoHideContainer != null || (Parent is AutoHideControl))
				return DockConsts.AutoHideParentID;
			return DockManager.Panels.IndexOf(ParentPanel);
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), XtraSerializableProperty()]
		public int XtraSavedParentID {
			get {
				if(SavedParent == null || DockManager == null || SavedParent.DockManager == null) return LayoutConsts.InvalidIndex;
				return SavedParent.XtraID;
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), XtraSerializableProperty()]
		public DockingStyle XtraAutoHideContainerDock {
			get {
				if(ParentAutoHideContainer != null) return (DockingStyle)ParentAutoHideContainer.Dock;
				if(ParentAutoHideControl != null) return DockLayoutUtils.ConvertToDockingStyle(ParentAutoHideControl.Position);
				return DockingStyle.Float;
			}
		}
		[Browsable(false), XtraSerializableProperty()]
		public Guid ID { get { return id; } set { id = value; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), XtraSerializableProperty()]
		public int XtraID { get { return DockManager.Panels.IndexOf(this); } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), XtraSerializableProperty()]
		public int XtraActiveChildID { get { return (ActiveChild == null ? -1 : ActiveChild.XtraID); } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), XtraSerializableProperty()]
		public Rectangle XtraBounds { get { return IsTab ? new Rectangle(Point.Empty, DockConsts.DefaultDockPanelSize) : Bounds; } set { Bounds = value; } }
		Size originalSizeCore = Size.Empty;
		[Browsable(false), XtraSerializableProperty()]
		public Size OriginalSize { get { return originalSizeCore; } set { originalSizeCore = value; } }
		protected internal bool ControlVisible { get { return base.Visible; } set { base.Visible = value; } }
		protected internal DockStyle ControlDock {
			get { return base.Dock; }
			set {
				DockLayout.LockAdjustBounds();
				if(ParentAutoHideControl != null)
					value = (DockStyle)DockLayoutUtils.ConvertToOppositeDockingStyle(ParentAutoHideControl.Position);
				try {
					base.Dock = value;
				}
				finally {
					DockLayout.UnlockAdjustBounds();
				}
			}
		}
		protected internal virtual bool CanDrawCaptionActive {
			get {
				if(DockManager == null) return false;
				if(RootPanel.FloatForm != null) {
					if(RootPanel.FloatForm != System.Windows.Forms.Form.ActiveForm)
						return false;
				}
				else if(!DockManager.FormContainsFocus) return false;
				if(DockManager.wasDeactivated) return false;
				if(DockManager.ActivePanel == this) return true;
				return (DockManager.ActivePanel != null && DockManager.ActivePanel.IsTab && DockManager.ActivePanel.ParentPanel == this);
			}
		}
		protected internal DockElementsPainter Painter { get { return DockManager.Painter; } }
		static readonly object visibilityChanged = new object();
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockPanelVisibilityChanged"),
#endif
 Category(DockConsts.PropertyChangedCategory)]
		public event VisibilityChangedEventHandler VisibilityChanged {
			add { this.Events.AddHandler(visibilityChanged, value); }
			remove { this.Events.RemoveHandler(visibilityChanged, value); }
		}
		protected internal virtual void RaiseVisibilityChanged(VisibilityChangedEventArgs e) {
			VisibilityChangedEventHandler handler = (VisibilityChangedEventHandler)this.Events[visibilityChanged];
			if(handler != null) handler(this, e);
			if(DockManager != null)
				DockManager.RaiseVisibilityChanged(e);
		}
		static readonly object tabbedChanged = new object();
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockPanelTabbedChanged"),
#endif
 Category(DockConsts.PropertyChangedCategory)]
		public event DockPanelEventHandler TabbedChanged {
			add { this.Events.AddHandler(tabbedChanged, value); }
			remove { this.Events.RemoveHandler(tabbedChanged, value); }
		}
		protected internal virtual void RaiseTabbedChanged() {
			DockPanelEventHandler handler = (DockPanelEventHandler)this.Events[tabbedChanged];
			DockPanelEventArgs e = new DockPanelEventArgs(this);
			if(handler != null) handler(this, e);
			if(DockManager != null)
				DockManager.RaiseTabbedChanged(e);
		}
		static readonly object tabsScrollChanged = new object();
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockPanelTabsScrollChanged"),
#endif
 Category(DockConsts.PropertyChangedCategory)]
		public event DockPanelEventHandler TabsScrollChanged {
			add { this.Events.AddHandler(tabsScrollChanged, value); }
			remove { this.Events.RemoveHandler(tabsScrollChanged, value); }
		}
		protected internal virtual void RaiseTabsScrollChanged() {
			DockPanelEventHandler handler = (DockPanelEventHandler)this.Events[tabsScrollChanged];
			DockPanelEventArgs e = new DockPanelEventArgs(this);
			if(handler != null) handler(this, e);
			if(DockManager != null)
				DockManager.RaiseTabsScrollChanged(e);
		}
		static readonly object tabsPositionChanged = new object();
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockPanelTabsPositionChanged"),
#endif
 Category(DockConsts.PropertyChangedCategory)]
		public event TabsPositionChangedEventHandler TabsPositionChanged {
			add { this.Events.AddHandler(tabsPositionChanged, value); }
			remove { this.Events.RemoveHandler(tabsPositionChanged, value); }
		}
		protected internal virtual void RaiseTabsPositionChanged(TabsPosition oldValue) {
			TabsPositionChangedEventArgs e = new TabsPositionChangedEventArgs(this, oldValue);
			TabsPositionChangedEventHandler handler = (TabsPositionChangedEventHandler)this.Events[tabsPositionChanged];
			if(handler != null) handler(this, e);
			if(DockManager != null)
				DockManager.RaiseTabsPositionChanged(e);
		}
		static readonly object activeChildChanged = new object();
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockPanelActiveChildChanged"),
#endif
 Category(DockConsts.PropertyChangedCategory)]
		public event DockPanelEventHandler ActiveChildChanged {
			add { this.Events.AddHandler(activeChildChanged, value); }
			remove { this.Events.RemoveHandler(activeChildChanged, value); }
		}
		protected internal virtual void RaiseActiveChildChanged() {
			DockPanelEventArgs e = new DockPanelEventArgs(this);
			DockPanelEventHandler handler = (DockPanelEventHandler)this.Events[activeChildChanged];
			if(handler != null) handler(this, e);
			if(DockManager != null)
				DockManager.RaiseActiveChildChanged(e);
		}
		static readonly object closingPanel = new object();
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockPanelClosingPanel"),
#endif
 Category(DockConsts.BehaviorCategory)]
		public event DockPanelCancelEventHandler ClosingPanel {
			add { this.Events.AddHandler(closingPanel, value); }
			remove { this.Events.RemoveHandler(closingPanel, value); }
		}
		protected internal virtual void RaiseClosingPanel(DockPanelCancelEventArgs e) {
			DockPanelCancelEventHandler handler = (DockPanelCancelEventHandler)this.Events[closingPanel];
			if(handler != null) handler(this, e);
			if(!e.Cancel && DockManager != null)
				DockManager.RaiseClosingPanel(e);
		}
		static readonly object customButtonClick = new object();
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockPanelCustomButtonClick"),
#endif
 Category(DockConsts.BehaviorCategory)]
		public event ButtonEventHandler CustomButtonClick {
			add { this.Events.AddHandler(customButtonClick, value); }
			remove { this.Events.RemoveHandler(customButtonClick, value); }
		}
		static readonly object customButtonUnchecked = new object();
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockPanelCustomButtonUnchecked"),
#endif
 Category(DockConsts.BehaviorCategory)]
		public event ButtonEventHandler CustomButtonUnchecked {
			add { this.Events.AddHandler(customButtonUnchecked, value); }
			remove { this.Events.RemoveHandler(customButtonUnchecked, value); }
		}
		static readonly object customButtonChecked = new object();
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockPanelCustomButtonChecked"),
#endif
 Category(DockConsts.BehaviorCategory)]
		public event ButtonEventHandler CustomButtonChecked {
			add { this.Events.AddHandler(customButtonChecked, value); }
			remove { this.Events.RemoveHandler(customButtonChecked, value); }
		}
		static readonly object closedPanel = new object();
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockPanelClosedPanel"),
#endif
 Category(DockConsts.BehaviorCategory)]
		public event DockPanelEventHandler ClosedPanel {
			add { this.Events.AddHandler(closedPanel, value); }
			remove { this.Events.RemoveHandler(closedPanel, value); }
		}
		protected internal ButtonsPanel ButtonsPanel {
			get { return buttonsPanelCore; }
		}
		static readonly object expandingCore = new object();
		[ Category(DockConsts.BehaviorCategory)]
		public event DockPanelCancelEventHandler Expanding {
			add { this.Events.AddHandler(expandingCore, value); }
			remove { this.Events.RemoveHandler(expandingCore, value); }
		}
		protected virtual bool RaiseExpanding(DockPanelCancelEventArgs e) {
			DockPanelCancelEventHandler handler = (DockPanelCancelEventHandler)this.Events[expandingCore];
			if(handler != null) handler(this, e);
			if(DockManager != null) DockManager.RaiseExpanding(e);
			return !e.Cancel;
		}
		static readonly object expandedCore = new object();
		[ Category(DockConsts.BehaviorCategory)]
		public event DockPanelEventHandler Expanded {
			add { this.Events.AddHandler(expandedCore, value); }
			remove { this.Events.RemoveHandler(expandedCore, value); }
		}
		protected virtual void RaiseExpanded(DockPanelEventArgs e) {
			DockPanelEventHandler handler = (DockPanelEventHandler)this.Events[expandedCore];
			if(handler != null) handler(this, e);
			if(DockManager != null) DockManager.RaiseExpanded(e);
		}
		static readonly object collapsingCore = new object();
		[ Category(DockConsts.BehaviorCategory)]
		public event DockPanelEventHandler Collapsing {
			add { this.Events.AddHandler(collapsingCore, value); }
			remove { this.Events.RemoveHandler(collapsingCore, value); }
		}
		protected virtual void RaiseCollapsing(DockPanelEventArgs e) {
			DockPanelEventHandler handler = (DockPanelEventHandler)this.Events[collapsingCore];
			if(handler != null) handler(this, e);
			if(DockManager != null) DockManager.RaiseCollapsing(e);
		}
		static readonly object collapsedCore = new object();
		[ Category(DockConsts.BehaviorCategory)]
		public event DockPanelEventHandler Collapsed {
			add { this.Events.AddHandler(collapsedCore, value); }
			remove { this.Events.RemoveHandler(collapsedCore, value); }
		}
		protected virtual void RaiseCollapsed(DockPanelEventArgs e) {
			DockPanelEventHandler handler = (DockPanelEventHandler)this.Events[collapsedCore];
			if(handler != null) handler(this, e);
			if(DockManager != null) DockManager.RaiseCollapsed(e);
		}
		static readonly object createResizeZoneCore = new object();
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockPanelCreateResizeZone"),
#endif
 Category(DockConsts.BehaviorCategory)]
		public event CreateResizeZoneEventHandler CreateResizeZone {
			add { this.Events.AddHandler(createResizeZoneCore, value); }
			remove { this.Events.RemoveHandler(createResizeZoneCore, value);}
		}
		protected internal virtual bool RaiseCreateResizeZone(CreateResizeZoneEventArgs e) {
			CreateResizeZoneEventHandler handler = (CreateResizeZoneEventHandler)Events[createResizeZoneCore];
			if(handler != null) handler(this, e);
			if(DockManager != null) DockManager.RaiseCreateResizeZone(e);
			return !e.Cancel;
		}
		[ListBindable(false), TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Editor("DevExpress.XtraBars.Docking.Design.CustomHeaderButtonCollectionEditor, " + AssemblyInfo.SRAssemblyBarsDesign,
			typeof(System.Drawing.Design.UITypeEditor)), Category("Custom Header Buttons"), Localizable(true)]
		public ButtonCollection CustomHeaderButtons {
			get { return customHeaderButtonsCore; }
		}
		bool ShouldSerializeCustomHeaderButtons() {
			return (customHeaderButtonsCore != null) && customHeaderButtonsCore.Count > 0;
		}
		protected internal virtual void RaiseClosedPanel() {
			DockPanelEventArgs e = new DockPanelEventArgs(this);
			DockPanelEventHandler handler = (DockPanelEventHandler)this.Events[closedPanel];
			if(handler != null) handler(this, e);
			if(DockManager != null)
				DockManager.RaiseClosedPanel(e);
		}
		internal void EnsureChildrenCreated() {
			if(!IsControlVisibleChangedLocked && Visible)
				OnVisibleChanged(EventArgs.Empty);
		}
		#region IFlickGestureClient Members
		FlickGestureHelper gestureHelper;
		FlickGestureHelper GestureHelper {
			get {
				if(gestureHelper == null) gestureHelper = new FlickGestureHelper(this);
				return gestureHelper;
			}
		}
		protected override void WndProc(ref Message m) {
			if(GestureHelper.WndProc(ref m)) return;
			base.WndProc(ref m);
		}
		IntPtr IFlickGestureClient.Handle {
			get { return IsHandleCreated ? Handle : IntPtr.Zero; }
		}
		Point IFlickGestureClient.PointToClient(Point p) {
			return PointToClient(p);
		}
		bool IFlickGestureClient.OnFlick(Point point, FlickGestureArgs args) {
			if(Dock != DockingStyle.Float || IsMdiDocument) return false;
			HitInfo hi = GetHitInfo(point);
			if(hi.HitTest == HitTest.Caption) {
				switch(args.Direction) {
					case FlickDirection.Up:
						DockTo(DockingStyle.Top);
						break;
					case FlickDirection.Left:
						DockTo(DockingStyle.Left);
						break;
					case FlickDirection.Right:
						DockTo(DockingStyle.Right);
						break;
					case FlickDirection.Down:
						DockTo(DockingStyle.Bottom);
						break;
				}
				return true;
			}
			return false;
		}
		#endregion
		internal int GetResizeZoneWidth() {
			return DesignMode ? DockConsts.DesignerResizeZoneWidth : DockConsts.ResizeZoneWidth;
		}
		#region IButtonsPanelOwner Members
		bool IButtonsPanelOwner.AllowHtmlDraw {
			get { return false; }
		}
		bool IButtonsPanelOwner.AllowGlyphSkinning {
			get { return GetAllowCustomHeaderButtonsGlyphSkinning(); }
		}
		Color IButtonsPanelGlyphSkinningOwner.GetGlyphSkinningColor(BaseButtonInfo info) {
			return Painter.ButtonPainter.GetGlyphSkinningColor(info, this);
		}
		internal AppearanceObject GetCaptionAppearance() {
			return (IsTab) ? DockLayout.LayoutParent.CaptionAppearance : DockLayout.CaptionAppearance;
		}
		XtraEditors.ButtonsPanelControl.ButtonsPanelControlAppearance IButtonsPanelOwner.AppearanceButton {
			get { return null; }
		}
		object IButtonsPanelOwner.ButtonBackgroundImages {
			get { return null; }
		}
		void IButtonsPanelOwner.Invalidate() {
			if(ButtonsPanel != null && ButtonsPanel.ViewInfo != null)
				Invalidate(ButtonsPanel.ViewInfo.Bounds);
		}
		#endregion
		internal bool GetAllowGlyphSkinning() {
			DefaultBoolean allow = AllowGlyphSkinning;
			if(allow == DefaultBoolean.Default && DockManager != null)
				return DockManager.AllowGlyphSkinning;
			return allow == DefaultBoolean.True;
		}
		internal bool GetAllowCustomHeaderButtonsGlyphSkinning() {
			DefaultBoolean allow = AllowCustomHeaderButtonsGlyphSkinning;
			if(ActiveChild != null)
				allow = ActiveChild.AllowCustomHeaderButtonsGlyphSkinning;
			if(allow == DefaultBoolean.Default && DockManager != null)
				return DockManager.AllowGlyphSkinning;
			return allow == DefaultBoolean.True;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool ActivateOnContentFocusWhenAutoHidden { get; set; }
	}
	[ToolboxItem(false),
	Designer("DevExpress.XtraBars.Docking.Design.ContainerControlDesigner, " + AssemblyInfo.SRAssemblyBarsDesign, typeof(IDesigner))]
	public class ControlContainer : Panel, ITransparentBackgroundManager {
		AppearanceObject viewAppearance;
		public ControlContainer() {
			TabStop = false;
			this.viewAppearance = new FrozenAppearance();
#if DXWhidbey
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
#else
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlConstants.DoubleBuffer, true);
#endif
		}
		protected override void Dispose(bool disposing) {
			if(disposing && !IsDisposed && Panel != null) {
				Panel.ControlContainer_Disposing();
				ViewAppearance.Dispose();
			}
			base.Dispose(disposing);
		}
		protected override void OnPaint(PaintEventArgs e) {
			if(Panel == null || Panel.DockManager == null || Panel.DockManager.IsDeserializing) return;
			if(Panel.DockManager.PaintingSuspended) return;
			using(GraphicsCache cache = new GraphicsCache(e.Graphics)) {
				cache.PaintArgs = new DXPaintEventArgs(e);
				try {
					Panel.DockManager.Painter.WindowPainter.DrawControlContainerClientArea(new DrawArgs(cache, ClientRectangle, ViewAppearance));
				}
				finally {
					cache.PaintArgs = null;
				}
			}
		}
		#region ITransparentBackgroundManager Members
		Color ITransparentBackgroundManager.GetForeColor(object childObject) {
			return GetForeColorCore(childObject as Control);
		}
		Color ITransparentBackgroundManager.GetForeColor(Control childControl) {
			return GetForeColorCore(childControl);
		}
		Color GetForeColorCore(Control control) {
			return ViewAppearance.ForeColor;
		}
		#endregion
		protected internal virtual void LayoutChanged() {
			Invalidate();
		}
		protected override void OnCreateControl() {
			base.OnCreateControl();
			LayoutChanged();
			if(!DesignMode) {
				OnVisibleChanged(EventArgs.Empty);
			}
		}
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			Invalidate();
		}
		bool IsParentAutoScaling {
			get {
				if(Panel == null) return false;
				return Panel.IsParentAutoScaling;
			}
		}
		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {
			if(IsParentAutoScaling) return;
			base.SetBoundsCore(x, y, width, height, specified);
		}
		protected override void OnResize(EventArgs e) {
			LayoutChanged();
			base.OnResize(e);
		}
		protected override void OnControlAdded(ControlEventArgs e) {
			if(e.Control is ControlContainer || e.Control is DockPanel || e.Control is AutoHideContainer || e.Control is AutoHideControl)
				throw new WarningException(string.Format(DockConsts.InvalidControlContainerChildFormatString, e.Control.GetType().FullName));
			base.OnControlAdded(e);
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			if((e.Button & MouseButtons.Left) != 0) {
				if(Panel != null && !Panel.IsMdiDocument)
					OnActivate();
			}
			base.OnMouseDown(e);
		}
		protected override bool ProcessKeyPreview(ref Message m) {
			if(Panel.RootPanel.Visibility != DockVisibility.AutoHide && DockLayoutUtils.IsEscapeDownMessage(ref m)) {
				if(Panel.InternalKeyDown(new KeyEventArgs(Keys.Escape)))
					return true;
			}
			return base.ProcessKeyPreview(ref m);
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			Panel.InternalKeyDown(e);
		}
		protected override void OnKeyUp(KeyEventArgs e) {
			base.OnKeyUp(e);
			Panel.InternalKeyUp(e);
		}
		protected bool AllowActivate {
			get {
				if(Panel != null) {
					if(Panel.DockManager != null) {
						if(Panel.DockManager.IsCreatingAutohideControl || Panel.Visibility != DockVisibility.AutoHide) {
							return true;
						}
						else return false;
					}
				}
				return true;
			}
		}
		protected override void OnEnter(EventArgs e) {
			if(AllowActivate && !Panel.IsMdiDocument)
				OnActivate();
			base.OnEnter(e);
		}
		protected virtual void OnActivate() {
			Panel.ControlContainer_Activate();
		}
		protected internal virtual void Activate() {
			if(ContainsFocus) return;
			if(Controls.Count == 0) Focus();
			else SelectNextControl(null, true, true, true, false);
		}
		protected override void WndProc(ref Message msg) {
			base.WndProc(ref msg);
			CodedUISupport.CodedUIMessagesHandler.ProcessCodedUIMessage(ref msg, this);
		}
		protected internal AppearanceObject ViewAppearance { get { return viewAppearance; } }
		[Browsable(false)]
		public DockPanel Panel { get { return (Parent as DockPanel); } }
	}
}
