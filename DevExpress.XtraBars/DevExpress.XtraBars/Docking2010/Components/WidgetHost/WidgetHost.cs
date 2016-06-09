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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.Mdi;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
namespace DevExpress.XtraBars.Docking2010.Views.Widget {
	[ToolboxItem(false), DesignTimeVisible(false)]
	public class WidgetsHost : XtraPanel, IDocumentsHost, ISupportXtraAnimation {
		IDocumentsHostOwner ownerCore;
		DocumentContainer activeContainerCore;
		internal bool LayoutSuspend { get; set; }
		public WidgetsHost(IDocumentsHostOwner owner) {
			ownerCore = owner;
			base.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
			base.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			base.SetStyle(ControlStyles.ResizeRedraw, true);
			base.SetStyle(ControlStyles.UserMouse, true);
			base.SetStyle(ControlStyles.Selectable, false);
			Dock = DockStyle.Fill;
			if(View.LayoutMode == LayoutMode.StackLayout || View.LayoutMode == LayoutMode.FlowLayout)
				AutoScroll = true;
			else
				AutoScroll = false;
			Handler = CreateHandler();
			AlwaysScrollActiveControlIntoView = false;
			VScrollBar.ValueChanged += OnScroll;
			HScrollBar.ValueChanged += OnScroll;
		}
		protected virtual IWidgetsHostHandler CreateHandler() {
			return new WidgetsHostHandler(this);
		}
		internal WidgetContainer MaximizedContainer { get; set; }
		public void LockUpdateLayout() {
			if(LayoutSuspend) return;
			LayoutSuspend = true;
			SuspendLayout();
		}
		bool isDisposingCore;
		protected sealed override void Dispose(bool disposing) {
			if(disposing) {
				if(!isDisposingCore) {
					isDisposingCore = true;
					ownerCore = null;
					MaximizedContainer = null;
					Handler = null;
					VScrollBar.ValueChanged -= OnScroll;
					HScrollBar.ValueChanged -= OnScroll;
				}
			}
			base.Dispose(disposing);
			DisposeUpdateTimer();
		}
		public void UpdateHandler() {
			Handler.UpdateDragService();
			if(View != null) {
				if(View.LayoutMode == LayoutMode.TableLayout)
					AutoScroll = false;
				else
					AutoScroll = true;
			}
		}
		public IWidgetsHostHandler Handler { get; internal set; }
		public WidgetView View { get { return Owner != null ? (Owner as DocumentManager).View as WidgetView : null; } }
		public void UnlockUpdateLayout() {
			if(Handler == null || Handler.DragMode) return;
			LayoutSuspend = false;
			ResumeLayout();
			LayoutChanged();
		}
		public void LayoutChanged() {
			UpdateLayout();
			if(LayoutSuspend) return;
			OnLayout(new LayoutEventArgs(this, String.Empty));
		}
		public void UpdateLayout() {
			if(View != null && View.ViewInfo != null)
				View.ViewInfo.SetDirty();
			if(LayoutSuspend) return;
			Owner.CalculateNC(ClientRectangle);
		}
		protected override void WndProc(ref Message m) {
			switch(m.Msg) {
				case MSG.WM_ERASEBKGND:
					DoEraseBackground(ref m);
					break;
				case MSG.WM_PRINT:
					DoPrint(ref m);
					break;
				case MSG.WM_PAINT:
					DoPaint(ref m);
					break;
				case DevExpress.Utils.Mdi.WinAPI.WM_PARENTNOTIFY:
					DoParentNotify(ref m);
					break;
				case DevExpress.Utils.Drawing.Helpers.MSG.WM_NCHITTEST:
					DoNCHitTest(ref m);
					break;
				case MSG.WM_KILLFOCUS:
					ProcessFocus(ref m, false);
					break;
				case MSG.WM_SETFOCUS:
					ProcessFocus(ref m, true);
					break;
				case DevExpress.Utils.Drawing.Helpers.MSG.WM_MOUSEHWHEEL:
					if(MaximizedContainer != null)
						m.Result = IntPtr.Zero;
					break;
				default:
					base.WndProc(ref m);
					break;
			}
			XtraBars.CodedUISupport.CodedUIMessagesHandler.ProcessCodedUIMessage(ref m, this);
		}
		protected virtual void ProcessFocus(ref Message m, bool setFocus) {
			if(Owner != null)
				Owner.Invalidate();
			m.Result = IntPtr.Zero;
		}
		#region DocumentContainerCollection
		protected override Control.ControlCollection CreateControlsInstance() {
			return new WidgetContainerCollection(this);
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new WidgetContainerCollection Controls {
			get { return base.Controls as WidgetContainerCollection; }
		}
		[Designer("", "")]
		public class WidgetContainerCollection : ControlCollection {
			IDocumentsHost documentHostCore;
			public IDocumentsHost DocumentHost {
				get { return documentHostCore; }
			}
			public WidgetContainerCollection(IDocumentsHost owner)
				: base(owner as Control) {
				documentHostCore = owner;
			}
			public override void Add(Control value) {
				DocumentContainer container = value as DocumentContainer;
				if(container != null) {
					base.Add(value);
					DocumentHost.OnControlAdded(value);
				}
			}
			public override void Remove(Control value) {
				DocumentContainer container = value as DocumentContainer;
				if(container != null) {
					var widgetContainer = container as WidgetContainer;
					if(widgetContainer != null && container.IsMaximized && widgetContainer.Document != null) {
						(DocumentHost as WidgetsHost).MaximizedContainer = null;
						widgetContainer.SetMaximized(false, false);
					}
					base.Remove(value);
					DocumentHost.OnControlRemoved(value);
				}
			}
			public DocumentContainer[] ToArray() {
				DocumentContainer[] containers = new DocumentContainer[Count];
				CopyTo(containers, 0);
				return containers;
			}
		}
		#endregion
		protected virtual void DoEraseBackground(ref Message m) {
			m.Result = new IntPtr(1);
		}
		void DoPrint(ref Message m) {
			try {
				using(Graphics g = Graphics.FromHdc(m.WParam))
					OnPaint(g);
			}
			finally { m.Result = IntPtr.Zero; }
		}
		protected virtual void DoPaint(ref Message m) {
			NativeMethods.PAINTSTRUCT ps = new NativeMethods.PAINTSTRUCT();
			try {
				IntPtr dc = NativeMethods.BeginPaint(Handle, ref ps);
				using(Graphics g = Graphics.FromHdc(dc))
					OnPaint(g);
			}
			finally {
				NativeMethods.EndPaint(Handle, ref ps);
				m.Result = IntPtr.Zero;
			}
		}
		protected virtual void DoNCHitTest(ref Message m) {
			if(ActiveContainer != null && ActiveContainer.IsMaximized)
				m.Result = new IntPtr(NativeMethods.HT.HTTRANSPARENT);
			else
				base.WndProc(ref m);
		}
		protected virtual void DoParentNotify(ref Message m) {
			if(WinAPIHelper.GetInt(m.WParam) == DevExpress.Utils.Drawing.Helpers.MSG.WM_LBUTTONDOWN) {
				Point pt = WinAPIHelper.GetPoint(m.LParam);
				if(!IsHandleCreated) return;
				DocumentContainer child;
				if(ParentNotifyHelper.ShouldSelectChild(this, pt, out child)) {
					UpdateLayout();
					Point childClientPoint = WinAPIHelper.Translate(Handle, child.Handle, pt);
					bool selectImmediately = false;
					if(!child.NCHitTest(childClientPoint)) {
						if(child.Document.Control is UserControl) return;
					}
					else selectImmediately = true;
					if(child != ActiveContainer || !child.ContainsFocus || !ControlStylesHelper.IsSelectableRecursive(child.Document.Control)) {
						ContainerControl parentContainer = Parent as ContainerControl;
						if(parentContainer != null) {
							if(parentContainer.ActiveControl != child) {
								if(selectImmediately)
									Select(child, parentContainer);
								else child.BeginInvoke(
										new Action<DocumentContainer, ContainerControl>(Select),
										new object[] { child, parentContainer });
							}
						}
					}
				}
			}
			base.WndProc(ref m);
		}
		Timer UpdateSizeTimer;
		const int DefaultSizeUpdateInterval = 400;
		protected override void OnResize(EventArgs eventargs) {
			base.OnResize(eventargs);
			if(!IsHandleCreated) return;
			if(Size.Width <= 0 || Size.Height <= 0) return;
			ResizeLayout();
		}
		void ResizeLayout() {
			if(View == null) return;
			if(firstPaint) {
				if(View.IsDesignMode() || View.AllowStartupAnimation == DefaultBoolean.False)
					LayoutChanged();
				else
					ResetUpdateTimer();
				return;
			}
			if((View.AllowResizeAnimation != DefaultBoolean.True) || (MaximizedContainer != null)) {
				LayoutChanged();
				return;
			}
			ResetUpdateTimer();
		}
		protected internal void OnUpdateSizeTimerTick(object sender, EventArgs e) {
			DisposeUpdateTimer();
			UpdateLayoutWithAnimation();
		}
		protected internal void UpdateLayoutWithAnimation() {
			if(View == null || View.StackGroups == null) return;
			if(!IsHandleCreated) return;
			if(Size.Width <= 0 || Size.Height <= 0) return;
			if(View.IsDisposing) return;
			LockUpdateLayout();
			Owner.CalculateNC(ClientRectangle);
			if(View.LayoutMode == LayoutMode.StackLayout)
				foreach(var item in View.StackGroups) {
					if(item.Info != null)
						item.Info.AddAnimation();
				}
			else
				foreach(var item in View.TableGroup.Items) {
					var containerControl = item.ContainerControl as WidgetContainer;
					if(containerControl != null)
						(containerControl).AddBoundsAnimation(containerControl.Bounds, item.Info.Bounds, false);
				}
			UnlockUpdateLayout();
		}
		void CreateUpdateTimer() {
			UpdateSizeTimer = new Timer();
			UpdateSizeTimer.Interval = DefaultSizeUpdateInterval;
			UpdateSizeTimer.Tick += OnUpdateSizeTimerTick;
			UpdateSizeTimer.Start();
		}
		void ResetUpdateTimer() {
			LockUpdateLayout();
			if(UpdateSizeTimer != null) {
				UpdateSizeTimer.Stop();
				UpdateSizeTimer.Start();
			}
			else {
				CreateUpdateTimer();
			}
		}
		void DisposeUpdateTimer() {
			if(UpdateSizeTimer == null) return;
			UpdateSizeTimer.Tick -= OnUpdateSizeTimerTick;
			UpdateSizeTimer.Stop();
			UpdateSizeTimer = null;
		}
		WidgetsLayoutEngine layoutEngineCore;
		public override System.Windows.Forms.Layout.LayoutEngine LayoutEngine {
			get {
				if(layoutEngineCore == null)
					layoutEngineCore = new WidgetsLayoutEngine();
				return layoutEngineCore;
			}
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			if(Owner != null)
				Owner.HandleCreated();
			Handler.UpdateDragService();
			LayoutChanged();
			ResizeLayout();
		}
		protected override void OnHandleDestroyed(EventArgs e) {
			base.OnHandleDestroyed(e);
			if(Owner != null)
				Owner.HandleDestroyed();
		}
		void IDocumentsHost.OnControlAdded(Control control) {
			OnControlAddedCore(control);
		}
		void IDocumentsHost.OnControlRemoved(Control control) {
			OnControlRemovedCore(control);
		}
		void OnControlAddedCore(Control control) {
			WidgetContainer container = control as WidgetContainer;
			if(Owner != null)
				Owner.ControlAdded(control);
			if(container != null && container.Manager == null) {
				container.SetManager(ownerCore as DocumentManager);
			}
			UpdateExistAnimation();
		}
		public void UpdateExistAnimation() {
			int animationCount = DevExpress.Utils.Drawing.Animation.XtraAnimator.Current.Animations.GetAnimationsCountByObject(this);
			if(animationCount > 0) {
				UpdateLayoutWithAnimation();
			}
			else {
				LayoutChanged();
			}
		}
		void OnControlRemovedCore(Control control) {
			if(Owner != null)
				Owner.ControlRemoved(control);
			if(ActiveContainer == control) {
				activeContainerCore = null;
				activeDocumentCore = null;
			}
			if(control == MaximizedContainer) {
				MaximizedContainer = null;
			}
		}
		BaseDocument activeDocumentCore;
		public BaseDocument ActiveDocument {
			get { return activeDocumentCore; }
		}
		public DocumentContainer ActiveContainer {
			get { return activeContainerCore; }
		}
		public bool ActivatePreviousDocument(BaseDocument document, BaseView view) {
			return false;
		}
		public void SetActiveContainer() {
			DocumentContainer documentContainer = ActiveContainer;
			IContainerControl parentContainer = Parent as IContainerControl;
			if(parentContainer != null)
				documentContainer = DocumentContainer.FromControl(parentContainer.ActiveControl);
			SetActiveContainer(documentContainer);
		}
		int lockSetActiveContainer = 0;
		void Select(DocumentContainer child, ContainerControl parentContainer) {
			lockSetActiveContainer++;
			try {
				if(parentContainer.Validate())
					SelectNextControlWithinTheDocumentContainerCore(child);
			}
			finally { lockSetActiveContainer--; }
			if(IsChild(parentContainer.ActiveControl, child))
				SetActiveContainer(child);
		}
		bool IsChild(Control control, DocumentContainer container) {
			while(control != null) {
				if(control == container)
					return true;
				control = control.Parent;
			}
			return false;
		}
		void SelectNextControlWithinTheDocumentContainerCore(DocumentContainer documentContainer) {
			Control content = ContainerControlManagementStrategy.GetContent(documentContainer);
			if(content != null && !ControlStylesHelper.IsSelectableRecursive(content))
				documentContainer.Select();
			else
				DocumentManager.SelectNextControl(ContainerControlManagementStrategy.CheckNestedContent(content));
		}
		public void SetActiveContainer(DocumentContainer documentContainer) {
			if(this.FindForm() == null) return;
			if(ActiveContainer == documentContainer) {
				SelectNextControlWithinTheDocumentContainer(documentContainer);
				return;
			}
			if(lockSetActiveContainer > 0) return;
			lockSetActiveContainer++;
			try {
				if(activeContainerCore != null && documentContainer != null && activeContainerCore == MaximizedContainer) {
					NativeMethods.SendMessage(activeContainerCore.Handle, MSG.WM_SYSCOMMAND, new IntPtr(NativeMethods.SC.SC_RESTORE), IntPtr.Zero);
					NativeMethods.SendMessage(documentContainer.Handle, MSG.WM_SYSCOMMAND, new IntPtr(NativeMethods.SC.SC_MAXIMIZE), IntPtr.Zero);
					documentContainer.BringToFront();
				}
				activeContainerCore = documentContainer;
				if(ActiveContainer != null)
					activeDocumentCore = ActiveContainer.Document;
				else
					activeDocumentCore = null;
				RaiseDocumentContainerActivate();
			}
			finally { lockSetActiveContainer--; }
		}
		void SelectNextControlWithinTheDocumentContainer(DocumentContainer documentContainer) {
			if(documentContainer != null && !Views.DocumentsHostContext.ContainsFocus(documentContainer))
				SelectNextControlWithinTheDocumentContainerCore(documentContainer);
		}
		object documentContainerActivate = new object();
		public event EventHandler DocumentContainerActivate {
			add { Events.AddHandler(documentContainerActivate, value); }
			remove { Events.RemoveHandler(documentContainerActivate, value); }
		}
		protected void RaiseDocumentContainerActivate() {
			EventHandler handler = (EventHandler)Events[documentContainerActivate];
			if(handler != null)
				handler(this, EventArgs.Empty);
		}
		public DocumentContainer[] Containers {
			get { return Controls.ToArray(); }
		}
		public IDocumentsHostOwner Owner {
			get { return ownerCore; }
		}
		bool firstPaint = true;
		protected virtual void OnPaint(Graphics g) {
			if(Owner != null) {
				if(firstPaint) {
					OnLoaded();
					firstPaint = false;
				}
				Owner.Paint(g);
			}
		}
		protected virtual void OnLoaded() {
		}
		protected virtual void OnScroll(object sender, EventArgs e) {
			UpdateLayout();
			Invalidate();
		}
		#region ISupportXtraAnimation Members
		public bool CanAnimate {
			get { return true; }
		}
		public Control OwnerControl {
			get { return this; }
		}
		#endregion
		Control FindFocusedChild(Control control) {
			if(control == null) return null;
			if(control.HasChildren) {
				for(int n = 0; n < control.Controls.Count; n++) {
					Control fc = FindFocusedChild(control.Controls[n]);
					if(fc != null) return fc;
				}
			}
			return control.Focused ? control : null;
		}
	}
}
