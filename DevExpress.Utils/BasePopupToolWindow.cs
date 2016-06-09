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
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.Helpers;
using DevExpress.Skins;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.Extensions;
using DevExpress.Utils.Win.Hook;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.Utils.Win {
	[ToolboxItem(false)]
	public abstract class BasePopupToolWindow : TopFormBase, IPopupControl, IMessageRedirector, ISupportsExternalPopupManagement, ISupportLookAndFeel, ITransparentBackgroundManager {
		Control content;
		Form ownerForm;
		BasePopupToolWindowHandler handler;
		ToolWindowHookPopup hookPopup;
		ToolWindowPainterBase painter;
		UserLookAndFeel lookAndFeel;
		public BasePopupToolWindow() {
			this.AllowDeepOwnerChecking = false;
			this.handler = CreateHandler();
			if(AutoInitialization) {
				ApplyOptionsCore();
			}
			if(KeepParentFormActive) {
				SetStyle(ControlStyles.Selectable, false);
			}
			this.lookAndFeel = new ControlUserLookAndFeel(this);
			this.lookAndFeel.StyleChanged += OnLookAndFeelChanged;
			EnsureContentControl();
		}
		protected UserLookAndFeel LookAndFeel { get { return lookAndFeel; } }
		protected ToolWindowPainterBase Painter {
			get {
				if(painter == null)
					painter = CreatePainter();
				return painter;
			}
		}
		protected virtual ToolWindowPainterBase CreatePainter() {
			return new ToolWindowPainter(this, this);
		}
		protected virtual ToolWindowHookPopup CreateHook() {
			return new ToolWindowHookPopup(this, MessageRedirector, PopupManager);
		}
		protected virtual void EnsureContentControl() {
			if(Content != null)
				return;
			this.content = CreateContentControl();
		}
		public void ApplyOptions() {
			ApplyOptionsCore();
		}
		double? preservedOpacity = null;
		protected internal virtual void PreserveOpacity() {
			this.preservedOpacity = Opacity;
		}
		protected internal virtual void RestoreOpacity() {
			if(this.preservedOpacity == null) return;
			double val = (double)this.preservedOpacity;
			if(Math.Abs(val - Opacity) > 0.001) {
				Opacity = val;
			}
		}
		protected internal double? PreservedOpacity {
			get { return preservedOpacity; }
		}
		protected virtual void ApplyOptionsCore() {
			SetSizeCore();
			EnsureContentControl();
		}
		protected virtual bool AutoInitialization { get { return true; } }
		protected virtual BasePopupToolWindowHandler CreateHandler() {
			return new BasePopupToolWindowHandler(this);
		}
		protected virtual PanelControl CreateContentPanel() {
			return new PanelControl();
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			OnAddContentControl();
			SetSkin();
		}
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			Painter.Draw(e);
		}
		protected virtual void SetSkin() {
			UpdateLookAndFeel();
		}
		void OwnerLookAndFeelStyleChanged(object sender, EventArgs e) {
			UpdateLookAndFeel();
		}
		protected virtual void UpdateLookAndFeel() {
			if(OwnerLookAndFeel == null)
				return;
			ISupportLookAndFeel laf = Content as ISupportLookAndFeel;
			if(laf == null)
				return;
			LookAndFeel.Assign(OwnerLookAndFeel);
			this.EnumAllChildren(control => {
				ISupportLookAndFeel claf = control as ISupportLookAndFeel;
				if(claf != null)
					claf.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			});
			LookAndFeel.OnStyleChanged();
		}
		protected virtual Form GetOwnerForm() {
			if(OwnerControl == null) return null;
			Form owner = FindOwnerForm();
			if(owner == null && AllowDeepOwnerChecking) {
				IntPtr handle = ProcessHelper.GetMainWindowHandle();
				owner = Form.FromHandle(handle) as Form;
			}
			return owner;
		}
		protected virtual Form FindOwnerForm() {
			Form owner = OwnerControl.FindForm();
			while(owner != null && !owner.TopLevel && owner.Parent != null) {
				owner = owner.Parent.FindForm();
			}
			return owner;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool AllowDeepOwnerChecking { get; set; }
		protected virtual void OnAddContentControl() {
			if(Content == null || Controls.Contains(Content))
				return;
			Controls.Add(Content);
			if(ShouldDockFillContent) 
				Content.Dock = DockStyle.Fill;
			if(ShouldCheckContentBounds) {
				UpdateContentBounds();
			}
		}
		protected void UpdateContentBounds() {
			if(Content.Parent != null) {
				Content.Size = Content.Parent.Size;
			}
			Content.Location = Point.Empty;
		}
		bool contentVisible = false;
		protected virtual void OnSaveContentState() {
			this.contentVisible = Content.Visible;
			if(!Content.Visible)
				Content.Visible = true;
		}
		protected virtual bool ShouldDockFillContent { get { return true; } }
		protected virtual bool ShouldCheckContentBounds { get { return false; } }
		protected virtual void OnRestoreContentState() {
			if(Content.Visible != this.contentVisible)
				Content.Visible = this.contentVisible;
		}
		public void ShowToolWindow() {
			ShowToolWindow(false);
		}
		bool shownImmediately;
		public virtual void ShowToolWindow(bool immediate) {
			if(DesignMode)
				return;
			if(IsParentFormRequired) {
				EnsureParentForm();
			}
			if(!CanDisplayFlyout(OwnerForm)) {
				OnCancelShowCore();
				return;
			}
			OnBeforeShowToolWindow();
			SubscribeOnParentEvents();
			OnAddContentControl();
			SetPaddingsCore();
			SetSizeCore();
			if(immediate) {
				OnStartAnimation(new PopupToolWindowAnimationEventArgs(true));
				Handler.OnImmediateShowToolWindow();
				OnEndAnimation(new PopupToolWindowAnimationEventArgs(true));
			}
			else {
				Handler.OnShowToolWindow();
			}
			this.shownImmediately = immediate;
			if(KeepParentFormActive) {
				PopupService.PopupShowing(this);
			}
		}
		protected virtual void EnsureParentForm() {
			this.ownerForm = GetOwnerForm();
			if(this.ownerForm == null) {
				throw new InvalidOperationException("Can't find the top form");
			}
		}
		protected internal virtual bool IsParentFormRequired { get { return true; } }
		protected internal virtual void DoShow() {
			Show();
		}
		protected virtual void OnBeforeShowToolWindow() {
			this.topControls = null;
		}
		protected virtual bool CanDisplayFlyout(Form form) {
			Control parent = GetOwnerControlParent();
			if(IsControlHidden(parent) || !form.IsHandleCreated) return false;
			Form activeForm = Form.ActiveForm;
			if(activeForm != null) {
				if(!object.ReferenceEquals(activeForm, form) && activeForm.Modal) {
					Form mdiParent = GetMdiParent();
					if(mdiParent == null || !object.ReferenceEquals(activeForm, mdiParent))
						return false;
				}
			}
			return true;
		}
		protected virtual void OnCancelShowCore() {
		}
		protected void EnsureHook() {
			if(this.hookPopup != null) return;
			this.hookPopup = CreateHook();
		}
		protected virtual void SetPaddingsCore() {
			if(!ShouldUseSkinPadding)
				return;
			SkinPaddingEdges paddingEdges = ContentPaddings;
			Padding = new Padding(paddingEdges.Left, paddingEdges.Top, paddingEdges.Right, paddingEdges.Bottom);
		}
		protected virtual void SetSizeCore() {
			Size = CalcFormSize();
		}
		protected SkinPaddingEdges ContentPaddings {
			get {
				if(Painter.GetBackgroundElement() == null) return new SkinPaddingEdges();
				return Painter.GetBackgroundElement().ContentMargins;
			}
		}
		protected ToolWindowOrientation FormOrientation {
			get {
				if(AnchorType == PopupToolWindowAnchor.Left || AnchorType == PopupToolWindowAnchor.Right)
					return ToolWindowOrientation.Vertical;
				return ToolWindowOrientation.Horizontal;
			}
		}
		protected internal virtual Size CalcFormSize() {
			Size res = FormSize;
			if(ShouldUseSkinPadding) {
				if(FormOrientation == ToolWindowOrientation.Horizontal)
					res.Height += (ContentPaddings.Top + ContentPaddings.Bottom);
				if(FormOrientation == ToolWindowOrientation.Vertical)
					res.Width += (ContentPaddings.Left + ContentPaddings.Right);
			}
			return res;
		}
		public void FocusControl(Control control) {
			PopupHookHelper.FocusFormControl(control, OwnerForm, PopupService);
		}
		public virtual bool Contains(Point point) {
			return this.Bounds.Contains(point);
		}
		public void HideToolWindow() {
			HideToolWindow(false);
		}
		public virtual void HideToolWindow(bool immediate) {
			if(DesignMode)
				return;
			RemoveAnimations();
			if(immediate) {
				OnStartAnimation(new PopupToolWindowAnimationEventArgs(false));
				Handler.OnImmediateHideToolWindow();
				OnEndAnimation(new PopupToolWindowAnimationEventArgs(false));
			}
			else {
				Handler.OnHideToolWindow();
			}
			if(KeepParentFormActive) {
				PopupService.PopupClosed(this);
				DestroyHookPopupCore();
			}
		}
		public void UpdateLocation() { UpdateLocation(IntPtr.Zero); }
		public void UpdateLocation(IntPtr handle) {
			if(CloseOnHidingOwner && handle != IntPtr.Zero) {
				Control control = Control.FromHandle(handle);
				if(control != null && IsControlHidden(control)) {
					ForceClosingCore(true);
					return;
				}
			}
			Handler.CheckToolWindowLocation();
		}
		#region Message Routing
		List<int> routedMessages = null;
		protected List<int> RoutedMessages {
			get {
				if(routedMessages == null) routedMessages = new List<int>();
				return routedMessages;
			}
		}
		public void RegisterRoutedMessage(int msg) {
			RoutedMessages.Add(msg);
		}
		protected virtual void ClearRoutedMessageList() {
			if(routedMessages == null) return;
			routedMessages.Clear();
			routedMessages = null;
		}
		#endregion
		protected internal void DoActivateInternal() {
			XtraForm xform = OwnerForm as XtraForm;
			if(xform == null || !xform.IsHandleCreated)
				return;
			XtraForm.SuppressDeactivation = true;
			xform.BeginInvoke((MethodInvoker)delegate { XtraForm.SuppressDeactivation = false; });
		}
		static IntPtr SC_CLOSE = new IntPtr(0xF060);
		protected internal virtual bool DoSysCommand(ref Message msg) {
			if(msg.WParam == SC_CLOSE) {
				return PostMessageToOwner(ref msg);
			}
			return false;
		}
		protected bool PostMessageToOwner(ref Message msg) {
			Control ownerControl = OwnerControl;
			if(ownerControl == null || !ownerControl.IsHandleCreated) {
				return false;
			}
			NativeMethods.PostMessage(ownerControl.Handle, msg.Msg, msg.WParam, msg.LParam);
			return true;
		}
		protected override void WndProc(ref Message msg) {
			bool handled = false;
			switch(msg.Msg) {
				case MSG.WM_MOUSEACTIVATE:
					DoActivateInternal();
					break;
				case MSG.WM_SYSCOMMAND:
					handled = DoSysCommand(ref msg);
					break;
			}
			if(handled) return;
			base.WndProc(ref msg);
		}
		#region IMessageRedirector
		bool IMessageRedirector.AllowRedirect(int msg) {
			return RoutedMessages.Contains(msg);
		}
		Control IMessageRedirector.MessageTarget {
			get { return MessageRoutingTarget ?? Content; }
		}
		#endregion
		#region ISupportsExternalPopupManagement
		void ISupportsExternalPopupManagement.Close() {
			ForceClosingCore(this.shownImmediately);
		}
		bool ISupportsExternalPopupManagement.AllowCloseOnMouseMove(Rectangle formBounds, Point pt) {
			return AllowCloseOnMouseMoveCore(formBounds, pt);
		}
		protected virtual bool AllowCloseOnMouseMoveCore(Rectangle formBounds, Point pt) {
			return false;
		}
		ICollection<IntPtr> topControls = null;
		ICollection<IntPtr> ISupportsExternalPopupManagement.TopControls {
			get {
				if(topControls == null) topControls = GetTopControls();
				return topControls;
			}
		}
		protected virtual ICollection<IntPtr> GetTopControls() {
			HashSet<IntPtr> hs = new HashSet<IntPtr>();
			hs.Add(ProcessHelper.GetMainWindowHandle());
			Control current = OwnerControl;
			while(current != null) {
				if(current.IsHandleCreated) hs.Add(current.Handle);
				current = current.Parent;
			}
			return hs;
		}
		#endregion
		protected void ForceClosingCore() { ForceClosingCore(false); }
		protected virtual void ForceClosingCore(bool immediate) {
			HideToolWindow(immediate);
		}
		#region Popup Hook
		static IPopupServiceControl popupService;
		public IPopupServiceControl PopupService {
			get {
				if(popupService == null) popupService = CreatePopupServiceController();
				return popupService;
			}
		}
		protected virtual WinPopupController CreatePopupServiceController() {
			return new WinPopupController();
		}
		#endregion
		#region ISupportLookAndFeel
		UserLookAndFeel ISupportLookAndFeel.LookAndFeel {
			get { return OwnerLookAndFeel; }
		}
		bool ISupportLookAndFeel.IgnoreChildren {
			get { return false; }
		}
		#endregion
		#region IPopupControl
		void IPopupControl.ClosePopup() {
		}
		bool IPopupControl.AllowMouseClick(Control control, Point mousePosition) {
			return true;
		}
		Control IPopupControl.PopupWindow { get { return this; } }
		bool IPopupControl.SuppressOutsideMouseClick { get { return true; } }
		#endregion
		#region StartAnimation Event
		static object StartAnimationKey = new object();
		public event PopupToolWindowAnimationEventHandler StartAnimation {
			add { Events.AddHandler(StartAnimationKey, value); }
			remove { Events.RemoveHandler(StartAnimationKey, value); }
		}
		protected internal virtual void OnStartAnimation(PopupToolWindowAnimationEventArgs e) {
			if(e.IsShowing) OnSaveContentState();
			PopupToolWindowAnimationEventHandler handler = (PopupToolWindowAnimationEventHandler)Events[StartAnimationKey];
			if(handler != null) handler(this, e);
		}
		#endregion
		#region EndAnimation  Event
		static object EndAnimationKey = new object();
		public event PopupToolWindowAnimationEventHandler EndAnimation {
			add { Events.AddHandler(EndAnimationKey, value); }
			remove { Events.RemoveHandler(EndAnimationKey, value); }
		}
		protected internal virtual void OnEndAnimation(PopupToolWindowAnimationEventArgs e) {
			if(e.IsShowing) EnsureHook();
			if(!e.IsShowing) {
				UnSubscribeOnParentEvents();
				OnRestoreContentState();
				Owner = null;
			}
			PopupToolWindowAnimationEventHandler handler = (PopupToolWindowAnimationEventHandler)Events[EndAnimationKey];
			if(handler != null) handler(this, e);
		}
		#endregion
		protected internal virtual bool AllowMessageRouting { get { return false; } }
		protected override bool IsTopMost { get { return false; } }
		protected internal UserLookAndFeel OwnerLookAndFeel {
			get {
				if(LookAndFeelProvider != null)
					return LookAndFeelProvider.LookAndFeel;
				ISupportLookAndFeel laf = OwnerControl as ISupportLookAndFeel;
				if(laf == null)
					return null;
				return laf.LookAndFeel;
			}
		}
		protected virtual void SubscribeOnParentEvents() {
			if(!IsParentFormRequired) return;
			OwnerForm.LocationChanged += OnFormOwnerLocationChanged;
			OwnerForm.SizeChanged += OnFormOwnerSizeChanged;
			OwnerForm.ControlRemoved += OnFormOwnerControlRemoved;
			OwnerControl.SizeChanged += OnOwnerControlSizeChanged;
			OwnerForm.VisibleChanged += OnFormOwnerVisibleChanged;
			OwnerForm.ParentChanged += OwnerFormParentChanged;
			SubscribeOnOwnerControlParentEvents();
			SubscribeOnMdiParentEvents();
			Control docContainer = GetDocumentContainer(); 
			if(docContainer != null) {
				docContainer.LocationChanged += OnDocumentContainerLocationChanged;
				docContainer.ParentChanged += OnDocumentContainerParentChanged;
			}
			if(OwnerLookAndFeel != null)
				OwnerLookAndFeel.StyleChanged += OwnerLookAndFeelStyleChanged;
		}
		protected virtual void SubscribeOnOwnerControlParentEvents() {
			Control parent = GetOwnerControlParent();
			if(parent != null) parent.LocationChanged += OnOwnerControlParentLocationChanged;
		}
		void OwnerFormParentChanged(object sender, EventArgs e) {
			SubscribeOnMdiParentEvents();
		}
		protected virtual void SubscribeOnMdiParentEvents() {
			Form mdiParent = GetMdiParent();
			if(mdiParent != null) {
				mdiParent.LocationChanged += OnMdiParentLocationChanged;
				mdiParent.MdiChildActivate += OnMdiParentMdiChildActivate;
			}
		}
		protected virtual void UnSubscribeOnMdiParentEvents() {
			Form mdiParent = GetMdiParent();
			if(mdiParent != null) {
				mdiParent.LocationChanged -= OnMdiParentLocationChanged;
				mdiParent.MdiChildActivate -= OnMdiParentMdiChildActivate;
			}
		}
		protected virtual void UnSubscribeOnParentEvents() {
			if(!IsParentFormRequired) return;
			if(OwnerForm != null) {
				OwnerForm.LocationChanged -= OnFormOwnerLocationChanged;
				OwnerForm.SizeChanged -= OnFormOwnerSizeChanged;
				OwnerForm.ControlRemoved -= OnFormOwnerControlRemoved;
				OwnerForm.VisibleChanged -= OnFormOwnerVisibleChanged;
				OwnerForm.ParentChanged -= OwnerFormParentChanged;
			}
			if(OwnerControl != null)
				OwnerControl.SizeChanged -= OnOwnerControlSizeChanged;
			UnsubscribeOnOwnerControlParentEvents();
			UnSubscribeOnMdiParentEvents();
			Control docContainer = GetDocumentContainer();
			if(docContainer != null) {
				docContainer.LocationChanged -= OnDocumentContainerLocationChanged;
				docContainer.ParentChanged -= OnDocumentContainerParentChanged;
			}
			if(OwnerLookAndFeel != null)
				OwnerLookAndFeel.StyleChanged -= OwnerLookAndFeelStyleChanged;
		}
		protected virtual void UnsubscribeOnOwnerControlParentEvents() {
			Control parent = GetOwnerControlParent();
			if(parent != null) parent.LocationChanged -= OnOwnerControlParentLocationChanged;
		}
		protected virtual void OnDocumentContainerParentChanged(object sender, EventArgs e) {
			if(GetOwnerForm() == null) return;
			ReassignOwnerForm();
		}
		protected virtual bool CloseOnHidingOwner { get { return true; } }
		protected virtual void OnOwnerControlParentLocationChanged(object sender, EventArgs e) {
			Control parent = GetOwnerControlParent();
			if(CloseOnHidingOwner && parent != null && IsControlHidden(parent)) ForceClosingCore(true);
		}
		protected virtual void OnFormOwnerLocationChanged(object sender, EventArgs e) {
			if(CloseOnHidingOwner && IsControlHidden(OwnerForm)) {
				HideToolWindow(true);
				return;
			}
			if(SyncLocationWithOwner) Handler.OnFormOwnerLocationChanged();
		}
		protected bool IsControlHidden(Control control) {
			if(control == null) return false;
			if(!control.Visible) return true;
			return control.Left == -control.Width && control.Top == -control.Height;
		}
		void OnDocumentContainerLocationChanged(object sender, EventArgs e) {
			if(SyncLocationWithOwner)
				Handler.OnFormOwnerLocationChanged();
		}
		void OnFormOwnerSizeChanged(object sender, EventArgs e) {
			if(SyncLocationWithOwner)
				Handler.OnFormOwnerSizeChanged();
		}
		void OnOwnerControlSizeChanged(object sender, EventArgs e) {
			if(SyncLocationWithOwner)
				Handler.OnOwnerControlSizeChanged();
		}
		protected virtual void OnFormOwnerVisibleChanged(object sender, EventArgs e) {
			if(!SyncLocationWithOwner) return;
			Form owner = (Form)sender;
			if(Visible != owner.Visible)
				Visible = owner.Visible;
		}
		void OnMdiParentLocationChanged(object sender, EventArgs e) {
			if(SyncLocationWithOwner)
				Handler.OnFormOwnerLocationChanged();
		}
		void OnMdiParentMdiChildActivate(object sender, EventArgs e) {
			OnMdiParentMdiChildActivateCore();
		}
		protected virtual void OnMdiParentMdiChildActivateCore() {
			if(OwnerForm == null || !OwnerForm.IsHandleCreated) return;
			Form activeChild = GetActiveMdiChild();
			if(activeChild == null || !activeChild.IsHandleCreated) return;
			Visible = object.ReferenceEquals(OwnerForm, activeChild);
		}
		void OnFormOwnerControlRemoved(object sender, ControlEventArgs e) {
			if(OwnerControl == null || !OwnerControl.IsHandleCreated)
				return;
			if(!SyncLocationWithOwner || !IsOwnerControlContainer(e.Control))
				return;
			ReassignOwnerForm();
		}
		protected virtual void ReassignOwnerForm() {
			OnOwnerFormChanging();
			OwnerControl.BeginInvoke(new MethodInvoker(OnOwnerFormChanged));
		}
		protected bool IsOwnerControlContainer(Control container) {
			Control current = OwnerControl;
			if(container == null || current == null) return false;
			while(current != null) {
				if(object.ReferenceEquals(current, container))
					return true;
				current = current.Parent;
			}
			return false;
		}
		protected virtual void OnOwnerFormChanging() {
			HideToolWindow(true);
		}
		protected virtual void OnOwnerFormChanged() {
		}
		string skinName = string.Empty;
		protected virtual void OnLookAndFeelChanged(object sender, EventArgs e) {
			OnBackColorChanged(EventArgs.Empty);
			if(!Size.IsEmpty) {
				InitLayout();
				UpdateChildrenLayout();
			}
			if(LookAndFeel != null) {
				if(!string.IsNullOrEmpty(this.skinName) && !string.Equals(this.skinName, LookAndFeel.ActiveSkinName, StringComparison.Ordinal)) {
					OnEffectiveLookAndFeelChanged();
				}
				if(!string.IsNullOrEmpty(LookAndFeel.ActiveSkinName)) {
					this.skinName = LookAndFeel.ActiveSkinName;
				}
			}
		}
		protected virtual void OnEffectiveLookAndFeelChanged() {
			SetSizeCore();
			SetPaddingsCore();
			UpdateLocation();
		}
		void UpdateChildrenLayout() {
			for(int n = 0; n < Controls.Count; n++) Controls[n].LayoutEngine.InitLayout(Controls[n], BoundsSpecified.All);
		}
		protected virtual void DestroyHookPopupCore() {
			if(HookPopup == null)
				return;
			HookPopup.Dispose();
			hookPopup = null;
		}
		protected virtual void RemoveAnimations() {
			XtraAnimator.Current.Animations.Remove(Handler.AnimationProvider);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				RemoveAnimations();
				UnSubscribeOnParentEvents();
				ClearRoutedMessageList();
				DestroyHookPopupCore();
				if(LookAndFeel != null) {
					LookAndFeel.StyleChanged -= new EventHandler(OnLookAndFeelChanged);
					LookAndFeel.Dispose();
					lookAndFeel = null;
				}
			}
			base.Dispose(disposing);
		}
		protected ISupportsExternalPopupManagement PopupManager { get { return this; } }
		protected IMessageRedirector MessageRedirector { get { return this; } }
		protected ToolWindowHookPopup HookPopup { get { return hookPopup; } }
		public Control Content { get { return content; } }
		protected internal Control GetDocumentContainer() {
			Control current = OwnerControl;
			while(current != null) {
				if(string.Equals(current.GetType().Name, "DocumentContainer", StringComparison.Ordinal))
					return current;
				current = current.Parent;
			}
			return null;
		}
		protected internal Form GetMdiParent() {
			return OwnerForm != null ? OwnerForm.MdiParent : null;
		}
		protected internal Form GetActiveMdiChild() {
			Form mdiParent = GetMdiParent();
			return mdiParent != null ? mdiParent.ActiveMdiChild : null;
		}
		protected internal Control GetOwnerControlParent() {
			return OwnerControl != null ? OwnerControl.Parent : null;
		}
		protected internal Form OwnerForm { get { return ownerForm; } }
		protected BasePopupToolWindowHandler Handler { get { return handler; } }
		protected internal virtual int AnimationLength { get { return 4000; } }
		protected internal abstract PopupToolWindowAnimation AnimationType { get; }
		protected internal abstract PopupToolWindowAnchor AnchorType { get; }
		protected internal abstract int HorzIndent { get; }
		protected internal abstract int VertIndent { get; }
		protected internal abstract bool KeepParentFormActive { get; }
		protected internal abstract Size FormSize { get; }
		protected internal abstract Control OwnerControl { get; }
		protected internal abstract ISupportLookAndFeel LookAndFeelProvider { get; }
		protected internal abstract Point FormLocation { get; }
		protected internal abstract bool SyncLocationWithOwner { get; }
		protected internal abstract Control CreateContentControl();
		protected internal abstract Control MessageRoutingTarget { get; }
		protected override bool ShowWithoutActivation { get { return true; } }
		protected internal virtual bool CloseOnOuterClick { get { return false; } }
		protected internal virtual bool ShouldUseSkinPadding { get { return true; } }
		#region ITransparentBackgroundManager
		Color ITransparentBackgroundManager.GetForeColor(Control childControl) {
			return GetForeColorLabel();
		}
		Color ITransparentBackgroundManager.GetForeColor(object childObject) {
			return GetForeColorLabel();
		}
		#endregion
		protected virtual Color GetForeColorLabel() {
			if(OwnerLookAndFeel == null || Painter == null)
				return ForeColor;
			SkinElement element = Painter.GetBackgroundElement();
			if(element == null)
				return ForeColor;
			Color color = element.Color.GetForeColor();
			return color.IsEmpty ? ForeColor : color;
		}
	}
	public abstract class ToolWindowPainterBase {
		BasePopupToolWindow owner;
		ISupportLookAndFeel lookAndFeel;
		public ToolWindowPainterBase(BasePopupToolWindow owner, ISupportLookAndFeel lookAndFeel) {
			this.owner = owner;
			this.lookAndFeel = lookAndFeel;
		}
		public virtual void Draw(PaintEventArgs e) {
			using(GraphicsCache cache = new GraphicsCache(e)) {
				Draw(cache);
			}
		}
		public virtual void Draw(GraphicsCache cache) {
			DrawBackground(cache);
			DrawBorders(cache);
		}
		protected virtual void DrawBackground(GraphicsCache cache) {
			SkinElementInfo elementInfo = new SkinElementInfo(GetBackgroundElement(), Bounds);
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, elementInfo);
		}
		protected virtual void DrawBorders(GraphicsCache cache) {
		}
		protected internal SkinElement GetBackgroundElement() {
			if(SkinProvider == null) return null;
			return GetBackgroundElementCore();
		}
		protected virtual SkinElement GetBackgroundElementCore() {
			return PdfViewerSkins.GetSkin(SkinProvider)[PdfViewerSkins.SearchPanel];
		}
		protected internal Rectangle Bounds {
			get { return new Rectangle(Point.Empty, ToolWindow.Size); }
		}
		protected ISkinProvider SkinProvider {
			get {
				if(LookAndFeel == null) return null;
				return LookAndFeel.LookAndFeel;
			}
		}
		protected ISupportLookAndFeel LookAndFeel { get { return lookAndFeel; } }
		protected BasePopupToolWindow ToolWindow { get { return owner as BasePopupToolWindow; } }
	}
	public class ToolWindowPainter : ToolWindowPainterBase {
		public ToolWindowPainter(BasePopupToolWindow toolWindow, ISupportLookAndFeel lookAndFeel)
			: base(toolWindow, lookAndFeel) {
		}
		protected override SkinElement GetBackgroundElementCore() {
			if(ToolWindow.ShouldUseSkinPadding) return base.GetBackgroundElementCore();
			return CommonSkins.GetSkin(SkinProvider)[CommonSkins.SkinGroupPanel];
		}
	}
	public class ToolWindowHookPopup : HookPopup {
		IMessageRedirector msgRedirector;
		ISupportsExternalPopupManagement target;
		public ToolWindowHookPopup(IPopupControl popup, IMessageRedirector msgRedirector, ISupportsExternalPopupManagement target) : base(popup) {
			this.target = target;
			this.msgRedirector = msgRedirector;
		}
		protected override bool PreFilterMessageCore(int Msg, IntPtr HWnd, IntPtr WParam) {
			if(ToolWindow.AllowMessageRouting) {
				RouteMessage(Msg, HWnd, WParam);
			}
			switch(Msg) {
				case MSG.WM_LBUTTONDOWN:
				case MSG.WM_NCLBUTTONDOWN:
					if(OnLButtonDown(HWnd, WParam)) return true;
					break;
				case MSG.WM_ACTIVATEAPP:
					if(OnWmActivateApp(WParam.ToInt32() != 0)) return true;
					break;
				case MSG.WM_MOUSEMOVE:
					if(OnWmMouseMove()) return true;
					break;
				case MSG.WM_MOVE:
				case MSG.WM_WINDOWPOSCHANGED:
				case MSG.WM_SIZE:
					OnWindowPosChangedCore(HWnd);
					break;
			}
			return base.PreFilterMessageCore(Msg, HWnd, WParam);
		}
		protected virtual void OnWindowPosChangedCore(IntPtr hwnd) {
			if(Target.TopControls.Contains(hwnd)) ToolWindow.UpdateLocation(hwnd);
		}
		protected virtual bool OnLButtonDown(IntPtr HWnd, IntPtr WParam) {
			ClosePopupIfNeeded();
			return false;
		}
		protected virtual bool OnWmActivateApp(bool activated) {
			if(activated) return false;
			ClosePopupIfNeeded();
			return false;
		}
		protected virtual bool OnWmMouseMove() {
			if(Target.AllowCloseOnMouseMove(ToolWindow.Bounds, Control.MousePosition)) {
				Target.Close();
				return true;
			}
			return false;
		}
		protected virtual bool ClosePopupIfNeeded() {
			if(!ToolWindow.CloseOnOuterClick || ToolWindow.Contains(Control.MousePosition))
				return false;
			Target.Close();
			return true;
		}
		protected virtual void RouteMessage(int Msg, IntPtr HWnd, IntPtr WParam) {
			if(processing || Form.ActiveForm != ToolWindow)
				return;
			Control target = MsgRedirector.MessageTarget;
			if(MsgRedirector.AllowRedirect(Msg) && target != null && target.IsHandleCreated)
				RouteMessageCore(target, Msg, WParam);
		}
		protected bool processing = false;
		protected virtual void RouteMessageCore(Control target, int Msg, IntPtr WParam) {
			processing = true;
			try {
				IntPtr lParam = Control.MousePosition.LParamFromPoint();
				NativeMethods.SendMessage(target.Handle, Msg, WParam, lParam);
			}
			finally {
				processing = false;
			}
		}
		public override void Dispose() {
			base.Dispose();
		}
		protected ISupportsExternalPopupManagement Target { get { return target; } }
		protected IMessageRedirector MsgRedirector { get { return msgRedirector; } }
		protected BasePopupToolWindow ToolWindow { get { return Popup as BasePopupToolWindow; } }
	}
	public class PopupToolWindowAnimationEventArgs : EventArgs {
		bool isShowing;
		public PopupToolWindowAnimationEventArgs(bool isShowing) {
			this.isShowing = isShowing;
		}
		public bool IsShowing { get { return isShowing; } }
	}
	public delegate void PopupToolWindowAnimationEventHandler(object sender, PopupToolWindowAnimationEventArgs e);
	public class BasePopupToolWindowHandler : IPopupToolWindowAnimationSupports {
		BasePopupToolWindow toolWindow;
		public BasePopupToolWindowHandler(BasePopupToolWindow toolWindow) {
			this.toolWindow = toolWindow;
		}
		PopupToolWindowAnimationProviderBase animationProvider;
		public virtual void OnShowToolWindow() {
			UpdateAnimationProvider();
			AnimationProvider.OnShowToolWindow();
		}
		public virtual void OnImmediateHideToolWindow() {
			ToolWindow.Hide();
			AnimationProvider.OnImmediateHideToolWindow();
		}
		public virtual void OnImmediateShowToolWindow() {
			ToolWindow.Owner = ToolWindow.OwnerForm;
			CheckToolWindowLocation();
			ToolWindow.DoShow();
			AnimationProvider.OnImmediateShowToolWindow();
		}
		public virtual void OnHideToolWindow() {
			AnimationProvider.OnHideToolWindow();
		}
		protected virtual void UpdateAnimationProvider() {
			this.animationProvider = null;
		}
		protected internal virtual void OnFormOwnerLocationChanged() {
			CheckToolWindowLocation();
		}
		protected internal virtual void OnFormOwnerSizeChanged() {
			CheckToolWindowLocation();
		}
		protected internal virtual void OnOwnerControlSizeChanged() {
			CheckToolWindowLocation();
		}
		protected internal virtual void CheckToolWindowLocation() {
			if(AnimationProvider == null)
				return;
			Point loc = AnimationProvider.CalcTargetFormLocation();
			if(ToolWindow.Location != loc)
				ToolWindow.Location = loc;
			Size size = AnimationProvider.CalcTargetFormSize();
			if(ToolWindow.Size != size)
				ToolWindow.Size = size;
		}
		#region IPopupToolWindowAnimationSupports
		BasePopupToolWindow IPopupToolWindowAnimationSupports.Form {
			get { return ToolWindow; }
		}
		PopupToolWindowAnchor IPopupToolWindowAnimationSupports.AnchorType {
			get { return ToolWindow.AnchorType; }
		}
		Form IPopupToolWindowAnimationSupports.OwnerForm {
			get { return ToolWindow.OwnerForm; }
		}
		Control IPopupToolWindowAnimationSupports.OwnerControl {
			get { return ToolWindow.OwnerControl; }
		}
		int IPopupToolWindowAnimationSupports.HorzIndent {
			get { return ToolWindow.HorzIndent; }
		}
		int IPopupToolWindowAnimationSupports.VertIndent {
			get { return ToolWindow.VertIndent; }
		}
		Size IPopupToolWindowAnimationSupports.FormSize {
			get { return ToolWindow.CalcFormSize(); }
		}
		Point IPopupToolWindowAnimationSupports.FormLocation {
			get { return ToolWindow.FormLocation; }
		}
		int IPopupToolWindowAnimationSupports.AnimationLength {
			get { return ToolWindow.AnimationLength; }
		}
		void IPopupToolWindowAnimationSupports.NotifyStartAnimation(PopupToolWindowAnimationEventArgs e) {
			ToolWindow.OnStartAnimation(e);
		}
		void IPopupToolWindowAnimationSupports.NotifyEndAnimation(PopupToolWindowAnimationEventArgs e) {
			ToolWindow.OnEndAnimation(e);
		}
		#endregion
		protected virtual PopupToolWindowAnimationProviderBase CreateAnimationProvider() {
			return PopupToolWindowAnimationProviderBase.Create(this, ToolWindow.AnimationType);
		}
		public BasePopupToolWindow ToolWindow { get { return toolWindow; } }
		public PopupToolWindowAnimationProviderBase AnimationProvider { 
			get { 
				if(animationProvider == null)
					animationProvider = CreateAnimationProvider();
				return animationProvider; 
			} 
		}
	}
	public interface IPopupToolWindowAnimationSupports {
		BasePopupToolWindow Form { get; }
		Form OwnerForm { get; }
		Control OwnerControl { get; }
		PopupToolWindowAnchor AnchorType { get; }
		int HorzIndent { get; }
		int VertIndent { get; }
		int AnimationLength { get; }
		Size FormSize { get; }
		Point FormLocation { get; }
		void NotifyStartAnimation(PopupToolWindowAnimationEventArgs e);
		void NotifyEndAnimation(PopupToolWindowAnimationEventArgs e);
	}
	public interface ISupportsExternalPopupManagement {
		void Close();
		ICollection<IntPtr> TopControls { get; }
		bool AllowCloseOnMouseMove(Rectangle formBounds, Point point);
	}
	public abstract class PopupToolWindowAnimationProviderBase : ISupportXtraAnimation, IXtraAnimationListener {
		object animationId = new object();
		IPopupToolWindowAnimationSupports info;
		public PopupToolWindowAnimationProviderBase(IPopupToolWindowAnimationSupports info) {
			this.info = info;
		}
		public static PopupToolWindowAnimationProviderBase Create(IPopupToolWindowAnimationSupports info, PopupToolWindowAnimation animationType) {
			if(animationType == PopupToolWindowAnimation.Fade)
				return new PopupToolWindowFadeAnimationProvider(info);
			if(animationType == PopupToolWindowAnimation.Slide) {
				switch(info.AnchorType) {
					case PopupToolWindowAnchor.TopRight:
					case PopupToolWindowAnchor.TopLeft:
					case PopupToolWindowAnchor.Top:
					case PopupToolWindowAnchor.Center:
					case PopupToolWindowAnchor.Manual:
						return new PopupToolWindowUpDownSlideAnimationProvider(info);
					case PopupToolWindowAnchor.Bottom:
						return new PopupToolWindowDownUpSlideAnimationProvider(info);
					case PopupToolWindowAnchor.Left:
						return new PopupToolWindowLeftRightSlideAnimationProvider(info);
					case PopupToolWindowAnchor.Right:
						return new PopupToolWindowRightLeftSlideAnimationProvider(info);
					default: throw new ArgumentException("AnchorType");
				}   
			}
			throw new ArgumentException("animationType");
		}
		protected virtual void OnStartAnimation() {
			Info.NotifyStartAnimation(new PopupToolWindowAnimationEventArgs(Showing));
			EnsureStopAnimation();
			XtraAnimator.Current.AddAnimation(AnimationInfo);
		}
		protected virtual void OnEndAnimation() {
			EnsureStopAnimation();
		}
		protected virtual void EnsureStopAnimation() {
			if(XtraAnimator.Current.Get(this, this.animationId) != null) XtraAnimator.Current.Animations.Remove(this, this.animationId);
		}
		#region ISupportXtraAnimation
		Control ISupportXtraAnimation.OwnerControl {
			get { return Info.Form; }
		}
		bool ISupportXtraAnimation.CanAnimate {
			get { return true; }
		}
		#endregion
		#region IXtraAnimationListener
		void IXtraAnimationListener.OnAnimation(BaseAnimationInfo info) {
			OnAnimationCore(info);
		}
		void IXtraAnimationListener.OnEndAnimation(BaseAnimationInfo info) {
			Info.NotifyEndAnimation(new PopupToolWindowAnimationEventArgs(Showing));
			if(!ToolWindow.IsDisposed) 
				OnEndAnimationCore(info);
		}
		#endregion
		protected virtual void OnAnimationCore(BaseAnimationInfo info) { }
		protected virtual void OnEndAnimationCore(BaseAnimationInfo info) { }
		bool showingStateCore = false;
		public void OnShowToolWindow() {
			showingStateCore = true;
			ToolWindow.Owner = Info.OwnerForm;
			OnShowToolWindowCore();
		}
		public void OnHideToolWindow() {
			showingStateCore = false;
			OnHideToolWindowCore();
		}
		public abstract void OnShowToolWindowCore();
		public abstract void OnHideToolWindowCore();
		protected internal abstract void OnImmediateHideToolWindow();
		protected internal abstract void OnImmediateShowToolWindow();
		protected abstract BaseAnimationInfo AnimationInfo { get; }
		public virtual Point CalcTargetFormLocation() {
			Rectangle ownerBounds = GetOwnerClientBounds();
			switch(Info.AnchorType) {
				case PopupToolWindowAnchor.TopRight:
					return new Point(ownerBounds.Right - ToolWindow.Width - Info.HorzIndent, ownerBounds.Top + Info.VertIndent);
				case PopupToolWindowAnchor.Top:
				case PopupToolWindowAnchor.Left:
					return new Point(ownerBounds.Left, ownerBounds.Top);
				case PopupToolWindowAnchor.TopLeft:
					return new Point(ownerBounds.Left + Info.HorzIndent, ownerBounds.Top + Info.VertIndent);
				case PopupToolWindowAnchor.Bottom:
					return new Point(ownerBounds.Left, ownerBounds.Bottom - ToolWindow.Height);
				case PopupToolWindowAnchor.Right:
					return new Point(ownerBounds.Right - ToolWindow.Width, ownerBounds.Top);
				case PopupToolWindowAnchor.Center:
					return new Point(ownerBounds.X + ownerBounds.Width / 2 - ToolWindow.Width / 2, ownerBounds.Y + ownerBounds.Height / 2 - ToolWindow.Height / 2);
				case PopupToolWindowAnchor.Manual:
					return new Point(ownerBounds.X + Info.FormLocation.X, ownerBounds.Y + Info.FormLocation.Y);
				default: throw new NotSupportedException("Invalid Anchor Type");
			}
		}
		public virtual Size CalcTargetFormSize() {
			Size res = Size.Empty;
			Rectangle ownerBounds = GetOwnerClientBounds();
			switch(Info.AnchorType) {
				case PopupToolWindowAnchor.TopLeft:
				case PopupToolWindowAnchor.TopRight:
				case PopupToolWindowAnchor.Center:
				case PopupToolWindowAnchor.Manual:
					res = Info.FormSize;
					break;
				case PopupToolWindowAnchor.Top:
				case PopupToolWindowAnchor.Bottom:
					res = new Size(ownerBounds.Width, Info.Form.Height);
					break;
				case PopupToolWindowAnchor.Left:
				case PopupToolWindowAnchor.Right:
					res = new Size(Info.Form.Width, ownerBounds.Height);
					break;
				default: throw new NotSupportedException("Invalid Anchor Type");
			}
			return res;
		}
		protected Rectangle GetOwnerBounds() {
			if(OwnerControl == null || !OwnerControl.IsHandleCreated) return Rectangle.Empty;
			if(OwnerControl is Form) {
				Form form = (Form)OwnerControl;
				if(form.IsMdiChild) {
					Point loc = OwnerControl.PointToScreen(Point.Empty);
					loc.X -= form.GetBorderSize();
					loc.Y -= form.GetCaptionHeight();
					return new Rectangle(loc, OwnerControl.Size);
				}
				return OwnerControl.Bounds;
			}
			return OwnerControl.RectangleToScreen(new Rectangle(Point.Empty, OwnerControl.Size));
		}
		protected Rectangle GetOwnerClientBounds() {
			Rectangle ownerBounds = GetOwnerBounds();
			if(ownerBounds.IsEmpty)
				return Rectangle.Empty;
			Size borderSize = Size.Empty;
			if(ToolWindow.IsParentFormRequired)
				borderSize = CalcBorders();
			ownerBounds.Inflate(-borderSize.Width, 0);
			ownerBounds.Y += borderSize.Height;
			ownerBounds.Height -= (borderSize.Height + borderSize.Width);
			return ownerBounds;
		}
		protected Size CalcBorders() {
			int width = Math.Max((OwnerControl.Width - OwnerControl.ClientRectangle.Width) / 2, 0);
			int height = Math.Max(OwnerControl.Height - OwnerControl.ClientRectangle.Height - width, 0);
			return new Size(width, height);
		}
		protected bool Showing { get { return showingStateCore; } }
		protected object AnimationId { get { return animationId; } }
		protected int AnimationLength { get { return Info.AnimationLength; } }
		protected BasePopupToolWindow ToolWindow { get { return Info.Form; } }
		protected Control OwnerControl { get { return Info.OwnerControl; } }
		protected IPopupToolWindowAnimationSupports Info { get { return info; } }
	}
	public class PopupToolWindowFadeAnimationProvider : PopupToolWindowAnimationProviderBase {
		public PopupToolWindowFadeAnimationProvider(IPopupToolWindowAnimationSupports info)
			: base(info) {
		}
		public override void OnShowToolWindowCore() {
			ToolWindow.Location = CalcTargetFormLocation();
			ToolWindow.Size = CalcTargetFormSize();
			ToolWindow.PreserveOpacity();
			ToolWindow.Opacity = 0.01;
			ToolWindow.DoShow();
			OnStartAnimation();
		}
		public override void OnHideToolWindowCore() {
			OnStartAnimation();
		}
		protected override void OnEndAnimationCore(BaseAnimationInfo info) {
			base.OnEndAnimationCore(info);
			ToolWindow.RestoreOpacity();
		}
		protected override BaseAnimationInfo AnimationInfo {
			get { return new PopupToolWindowFormOpacityAnimationInfo(this, AnimationId, ToolWindow, Showing, AnimationLength); }
		}
		protected internal override void OnImmediateHideToolWindow() {
		}
		protected internal override void OnImmediateShowToolWindow() {
		}
	}
	public abstract class PopupToolWindowSlideAnimationProviderBase : PopupToolWindowAnimationProviderBase {
		Point startPt, endPt;
		public PopupToolWindowSlideAnimationProviderBase(IPopupToolWindowAnimationSupports info)
			: base(info) {
		}
		public override void OnShowToolWindowCore() {
			OnStartSlideCore(true);
		}
		public override void OnHideToolWindowCore() {
			OnStartSlideCore(false);
		}
		protected internal override void OnImmediateHideToolWindow() {
			ToolWindow.Region = new Region(Rectangle.Empty);
		}
		protected internal override void OnImmediateShowToolWindow() {
		}
		protected virtual void OnStartSlideCore(bool down) {
			CalcCheckPoints();
			ToolWindow.Location = startPt;
			ToolWindow.Size = CalcTargetFormSize();
			OnStartAnimation();
			if(down) {
				ToolWindow.PreserveOpacity();
				ToolWindow.Opacity = 0.001;
			}
			ToolWindow.DoShow();
		}
		protected void CalcCheckPoints() {
			this.startPt = CalcStartPoint();
			this.endPt = CalcEndPoint();
		}
		protected abstract Point CalcStartPoint();
		protected abstract Point CalcEndPoint();
		protected override void OnEndAnimationCore(BaseAnimationInfo info) {
			FormSlideAnimationInfoBase animationInfo = (FormSlideAnimationInfoBase)info;
			if(!Showing) ToolWindow.Hide();
		}
		public Point StartPt { get { return startPt; } }
		public Point EndPt { get { return endPt; } }
	}
	public class PopupToolWindowUpDownSlideAnimationProvider : PopupToolWindowSlideAnimationProviderBase {
		public PopupToolWindowUpDownSlideAnimationProvider(IPopupToolWindowAnimationSupports info)
			: base(info) {
		}
		protected override Point CalcStartPoint() {
			Point pt = CalcTargetFormLocation();
			if(Showing) {
				pt.Y -= ToolWindow.Height;
			}
			return pt;
		}
		protected override Point CalcEndPoint() {
			Point pt = CalcTargetFormLocation();
			if(!Showing) {
				pt.Y -= ToolWindow.Height;
			}
			return pt;
		}
		protected override BaseAnimationInfo AnimationInfo {
			get { return new FormSlideUpDownAnimationInfo(this, AnimationId, ToolWindow, Showing, StartPt, EndPt, GetOwnerClientBounds(), AnimationLength); }
		}
	}
	public class PopupToolWindowDownUpSlideAnimationProvider : PopupToolWindowSlideAnimationProviderBase {
		public PopupToolWindowDownUpSlideAnimationProvider(IPopupToolWindowAnimationSupports info)
			: base(info) {
		}
		protected override Point CalcStartPoint() {
			Point pt = CalcTargetFormLocation();
			if(Showing) {
				pt.Y += ToolWindow.Height;
			}
			return pt;
		}
		protected override Point CalcEndPoint() {
			Point pt = CalcTargetFormLocation();
			if(!Showing) {
				pt.Y += ToolWindow.Height;
			}
			return pt;
		}
		protected override BaseAnimationInfo AnimationInfo {
			get { return new FormSlideDownUpAnimationInfo(this, AnimationId, ToolWindow, Showing, StartPt, EndPt, GetOwnerClientBounds(), AnimationLength); }
		}
	}
	public class PopupToolWindowLeftRightSlideAnimationProvider : PopupToolWindowSlideAnimationProviderBase {
		public PopupToolWindowLeftRightSlideAnimationProvider(IPopupToolWindowAnimationSupports info)
			: base(info) {
		}
		protected override Point CalcStartPoint() {
			Point pt = CalcTargetFormLocation();
			if(Showing) {
				pt.X -= ToolWindow.Width;
			}
			return pt;
		}
		protected override Point CalcEndPoint() {
			Point pt = CalcTargetFormLocation();
			if(!Showing) {
				pt.X -= ToolWindow.Width;
			}
			return pt;
		}
		protected override BaseAnimationInfo AnimationInfo {
			get { return new FormSlideLeftRightAnimationInfo(this, AnimationId, ToolWindow, Showing, StartPt, EndPt, GetOwnerClientBounds(), AnimationLength); }
		}
	}
	public class PopupToolWindowRightLeftSlideAnimationProvider : PopupToolWindowSlideAnimationProviderBase {
		public PopupToolWindowRightLeftSlideAnimationProvider(IPopupToolWindowAnimationSupports info)
			: base(info) {
		}
		protected override Point CalcStartPoint() {
			Point pt = CalcTargetFormLocation();
			if(Showing) {
				pt.X += ToolWindow.Width;
			}
			return pt;
		}
		protected override Point CalcEndPoint() {
			Point pt = CalcTargetFormLocation();
			if(!Showing) {
				pt.X += ToolWindow.Width;
			}
			return pt;
		}
		protected override BaseAnimationInfo AnimationInfo {
			get { return new FormSlideRightLeftAnimationInfo(this, AnimationId, ToolWindow, Showing, StartPt, EndPt, GetOwnerClientBounds(), AnimationLength); }
		}
	}
	public class PopupToolWindowFormOpacityAnimationInfo : FormOpacityAnimationInfo {
		public PopupToolWindowFormOpacityAnimationInfo(ISupportXtraAnimation obj, object animationId, BasePopupToolWindow frm, bool fadeIn, int lengthMsec)
			: base(obj, animationId, frm, fadeIn, lengthMsec) {
		}
		protected override void DoSetFadeInOpacity() {
			if(!Form.PreservedOpacity.HasValue) return;
			Form.Opacity = (float)Form.PreservedOpacity * ((float)CurrentFrame) / FrameCount;
		}
		protected override void DoSetFadeOutOpacity() {
			if(!Form.PreservedOpacity.HasValue) return;
			Form.Opacity = (float)Form.PreservedOpacity - (float)(CurrentFrame) / FrameCount;
		}
		public new BasePopupToolWindow Form { get { return base.Form as BasePopupToolWindow; } }
	}
	public abstract class FormSlideAnimationInfoBase : BaseAnimationInfo {
		BasePopupToolWindow form;
		bool forward;
		Point startPt, endPt;
		Rectangle ownerBounds;
		public FormSlideAnimationInfoBase(ISupportXtraAnimation obj, object animationId, BasePopupToolWindow form, bool forward, Point startPt, Point endPt, Rectangle ownerBounds, int lengthMsec)
			: base(obj, animationId, 1000 * 1000 / 50, (int)(lengthMsec * 0.001f * 50)) {
			this.form = form;
			this.forward = forward;
			this.startPt = startPt;
			this.endPt = endPt;
			this.ownerBounds = ownerBounds;
		}
		public override void FrameStep() {
			float current = ((float)CurrentFrame) / FrameCount;
			Form.Location = CalcLocation(current);
			Form.Region = GetRegion(current);
			if(Forward && Form.Opacity < 1) {
				Form.RestoreOpacity();
			}
			if(IsFinalFrame && Forward) {
				Form.Region = null;
			}
		}
		protected virtual Region GetRegion(float current) {
			Rectangle rect = CalcRegionRect(current);
			return new Region(rect);
		}
		protected abstract Point CalcLocation(float current);
		protected abstract Rectangle CalcRegionRect(float current);
		public BasePopupToolWindow Form { get { return form; } }
		public bool Forward { get { return forward; } }
		public Point StartPt { get { return startPt; } }
		public Point EndPt { get { return endPt; } }
		public Rectangle OwnerBounds { get { return ownerBounds; } }
	}
	public class FormSlideUpDownAnimationInfo : FormSlideAnimationInfoBase {
		public FormSlideUpDownAnimationInfo(ISupportXtraAnimation obj, object animationId, BasePopupToolWindow form, bool forward, Point startPt, Point endPt, Rectangle ownerBounds, int lengthMsec)
			: base(obj, animationId, form, forward, startPt, endPt, ownerBounds, lengthMsec) {
		}
		protected override Point CalcLocation(float current) {
			return new Point(StartPt.X, StartPt.Y + (int)((EndPt.Y - StartPt.Y) * current));
		}
		protected override Rectangle CalcRegionRect(float current) {
			Point pt = CalcLocation(current);
			Rectangle intersect = Rectangle.Intersect(OwnerBounds, new Rectangle(pt, Form.Size));
			return new Rectangle(0, Form.Height - intersect.Height, intersect.Width, intersect.Height);
		}
	}
	public class FormSlideDownUpAnimationInfo : FormSlideAnimationInfoBase {
		public FormSlideDownUpAnimationInfo(ISupportXtraAnimation obj, object animationId, BasePopupToolWindow form, bool forward, Point startPt, Point endPt, Rectangle ownerBounds, int lengthMsec)
			: base(obj, animationId, form, forward, startPt, endPt, ownerBounds, lengthMsec) {
		}
		protected override Point CalcLocation(float current) {
			return new Point(StartPt.X, StartPt.Y + (int)((EndPt.Y - StartPt.Y) * current));
		}
		protected override Rectangle CalcRegionRect(float current) {
			Point pt = CalcLocation(current);
			Rectangle intersect = Rectangle.Intersect(OwnerBounds, new Rectangle(pt, Form.Size));
			return new Rectangle(0, 0, intersect.Width, intersect.Height);
		}
	}
	public class FormSlideLeftRightAnimationInfo : FormSlideAnimationInfoBase {
		public FormSlideLeftRightAnimationInfo(ISupportXtraAnimation obj, object animationId, BasePopupToolWindow form, bool forward, Point startPt, Point endPt, Rectangle ownerBounds, int lengthMsec)
			: base(obj, animationId, form, forward, startPt, endPt, ownerBounds, lengthMsec) {
		}
		protected override Point CalcLocation(float current) {
			return new Point(StartPt.X + (int)((EndPt.X - StartPt.X) * current), StartPt.Y);
		}
		protected override Rectangle CalcRegionRect(float current) {
			Point pt = CalcLocation(current);
			Rectangle intersect = Rectangle.Intersect(OwnerBounds, new Rectangle(pt, Form.Size));
			return new Rectangle(Form.Width - intersect.Width, 0, intersect.Width, intersect.Height);
		}
	}
	public class FormSlideRightLeftAnimationInfo : FormSlideAnimationInfoBase {
		public FormSlideRightLeftAnimationInfo(ISupportXtraAnimation obj, object animationId, BasePopupToolWindow form, bool forward, Point startPt, Point endPt, Rectangle ownerBounds, int lengthMsec)
			: base(obj, animationId, form, forward, startPt, endPt, ownerBounds, lengthMsec) {
		}
		protected override Point CalcLocation(float current) {
			return new Point(StartPt.X + (int)((EndPt.X - StartPt.X) * current), StartPt.Y);
		}
		protected override Rectangle CalcRegionRect(float current) {
			Point pt = CalcLocation(current);
			Rectangle intersect = Rectangle.Intersect(OwnerBounds, new Rectangle(pt, Form.Size));
			return new Rectangle(0, 0, intersect.Width, intersect.Height);
		}
	}
	public enum ToolWindowOrientation { Horizontal, Vertical }
	public enum PopupToolWindowAnimation { Fade, Slide }
	public enum PopupToolWindowAnchor { TopRight, TopLeft, Top, Bottom, Left, Right, Center, Manual }
}
