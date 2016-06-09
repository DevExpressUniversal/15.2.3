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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Mdi;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraTab;
using DevExpress.XtraTab.Drawing;
using DevExpress.XtraTab.Registrator;
using DevExpress.XtraTab.ViewInfo;
namespace DevExpress.XtraTabbedMdi {
	[Designer("DevExpress.XtraBars.Design.XtraTabbedMdiManagerDesigner, " + AssemblyInfo.SRAssemblyBarsDesign, typeof(IDesigner)),
	DesignerCategory("Component"),
 DXToolboxItem(true),
	Description("Provides centralized control over the multiple document interface (MDI) child forms that are parented to the form."),
	ToolboxTabName(AssemblyInfo.DXTabNavigation),
	ToolboxBitmap(typeof(XtraBars.ToolboxIcons.ToolboxIconsRootNS), "XtraTabbedMdiManager")
	]
	public class XtraTabbedMdiManager : System.ComponentModel.Component, ISupportInitialize, IXtraTab, IXtraTabProperties, IBarAndDockingControllerClient, IToolTipControlClient, IMdiClientSubclasserOwner, IXtraTabPropertiesEx,
		DevExpress.Utils.MVVM.Services.IDocumentAdapterFactory {
		BarAndDockingController controller;
		Form _mdiParent, _subscribedMdiParent;
		IMdiClientSubclasser _encapsulator;
		int initialize = 0;
		bool invalidated = false;
		SetNextMdiChildMode mdiNextChildMode = SetNextMdiChildMode.Default;
		ClosePageButtonShowMode closePageButtonShowModeCore;
		PinPageButtonShowMode pinPageButtonShowModeCore;
		DefaultBoolean allowGlyphSkinningCore;
		bool populating;
		Rectangle bounds, clientBounds;
		Rectangle cachedViewInfoPageClientBounds = Rectangle.Empty;
		BaseViewInfoRegistrator view;
		BaseTabControlViewInfo viewInfo;
		BaseTabPainter painter;
		BaseTabHandler handler;
		XtraMdiTabPageCollection pages;
		[Category(XtraTabbedMdiCategoryName.Events)]
		public event EventHandler SelectedPageChanged;
		[Category(XtraTabbedMdiCategoryName.Events)]
		public event EventHandler MouseEnter;
		[Category(XtraTabbedMdiCategoryName.Events)]
		public event EventHandler MouseLeave;
		[Category(XtraTabbedMdiCategoryName.Events)]
		public event MouseEventHandler MouseDown;
		[Category(XtraTabbedMdiCategoryName.Events)]
		public event MouseEventHandler MouseUp;
		[Category(XtraTabbedMdiCategoryName.Events)]
		public event MouseEventHandler MouseMove;
		[Category(XtraTabbedMdiCategoryName.Events)]
		public event MdiTabPageEventHandler PageAdded;
		[Category(XtraTabbedMdiCategoryName.Events)]
		public event MdiTabPageEventHandler PageRemoved;
		[Category(XtraTabbedMdiCategoryName.Events)]
		protected internal event MdiTabPageCancelEventHandler PageClosing;
		[Category(XtraTabbedMdiCategoryName.Events)]
		public event SetNextMdiChildEventHandler SetNextMdiChild;
		AppearanceObject appearance;
		PageAppearance appearancePage;
		BorderStyles borderStyle = BorderStyles.Default, borderStylePage = BorderStyles.Default;
		DefaultBoolean showHeaderFocus, headerAutoFill,  showToolTips, multiLine;
		TabButtonShowMode headerButtonsShowMode;
		TabButtons headerButtons;
		TabPageImagePosition pageImagePosition;
		object images;
		TabHeaderLocation headerLocation;
		TabOrientation headerOrientation;
		DefaultBoolean allowDragDrop;
		Rectangle dragStartBox = Rectangle.Empty;
		DragDropDrawTabControl dragDropPainterOverride = null;
		DefaultBoolean floatOnDoubleClick = DefaultBoolean.Default;
		DefaultBoolean floatOnDrag = DefaultBoolean.Default;
		int maxTabPageWidthCore;
		int tabPageWidthCore;
		Timer dropTimerCore;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("XtraTabbedMdiManagerMdiParent"),
#endif
 Category(XtraTabbedMdiCategoryName.Behavior)]
		public Form MdiParent {
			get { return _mdiParent; }
			set {
				if(ReferenceEquals(_mdiParent, value))
					return;
				_mdiParent = value;
				ReInit();
			}
		}
		protected IMdiClientSubclasser MdiClientEncapsulator {
			get {
				return _encapsulator;
			}
		}
		public XtraTabbedMdiManager(System.ComponentModel.IContainer container)
			: this() {
			container.Add(this);
		}
		public XtraTabbedMdiManager() {
			this.controller = null;
			this.floatFormsCore = CreateFloatFormsCollection();
			this.closePageButtonShowModeCore = ClosePageButtonShowMode.Default;
			this.pinPageButtonShowModeCore = PinPageButtonShowMode.Default;
			this.headerButtonsShowMode = TabButtonShowMode.Default;
			this.headerButtons = TabButtons.Default;
			this.appearance = new AppearanceObject("Appearance");
			this.appearance.Changed += new EventHandler(OnAppearanceChanged);
			this.appearancePage = new PageAppearance();
			this.appearancePage.Changed += new EventHandler(OnAppearanceChanged);
			this.showHeaderFocus = DefaultBoolean.Default;
			this.showToolTips = DefaultBoolean.Default;
			this.multiLine = DefaultBoolean.Default;
			this.headerAutoFill = DefaultBoolean.Default;
			this.pageImagePosition = TabPageImagePosition.Near;
			this.images = null;
			this.headerLocation = TabHeaderLocation.Top;
			this.headerOrientation = TabOrientation.Default;
			this.allowDragDrop = DefaultBoolean.Default;
			this.allowGlyphSkinningCore = DefaultBoolean.Default;
			this.populating = false;
			this.pages = new XtraMdiTabPageCollection();
			this.bounds = Rectangle.Empty;
			GetControllerInternal().AddClient(this);
			((IXtraTab)this).LookAndFeel.StyleChanged += OnLookAndFeelStyleChanged;
			CreateView();
			tabPageWidthCore = 0;
			maxTabPageWidthCore = 0;
		}
		void ISupportInitialize.BeginInit() { ++initialize; }
		void ISupportInitialize.EndInit() {
			--initialize;
			ReInit();
		}
		protected bool IsInitialize { get { return initialize != 0; } }
		protected virtual bool InTheAir {
			get {
				if(IsInitialize)
					return false;
				if(ViewInfo == null)
					return false;
				if(MdiClientEncapsulator == null)
					return false;
				if(!MdiClientEncapsulator.ClientWindow.IsHandleCreated)
					return false;
				if(MdiParent == null)
					return false;
				return true;
			}
		}
		protected virtual bool GetDrawEmptyPageClient() {
			return ViewInfo.VisiblePagesCount == 0;
		}
		protected virtual void MultipleMdiClientConditionProcessor() {
			try {
				throw new InvalidOperationException("//TODO multiple MdiClients");	
			}
			catch { }
		}
		protected virtual void ReInit() {
			if(MdiClientEncapsulator != null)
				UncapsulateClient();
			Depopulate();
			if(MdiParent != null)
				MdiParent.IsMdiContainer = true;
			if(IsInitialize)
				return;
			if(_subscribedMdiParent != null) {
				if(!ReferenceEquals(_subscribedMdiParent, MdiParent) && _subscribedMdiParent.ActiveMdiChild != null)
					_subscribedMdiParent.LayoutMdi(MdiLayout.Cascade);
				_subscribedMdiParent.MdiChildActivate -= new EventHandler(OnMdiParent_ChildActivate);
				_subscribedMdiParent.Invalidate();
				_subscribedMdiParent = null;
			}
			if(MdiParent == null)
				return;
			_subscribedMdiParent = MdiParent;
			_subscribedMdiParent.MdiChildActivate += new EventHandler(OnMdiParent_ChildActivate);
			MdiClient client = MdiClientSubclasser.GetMdiClient(MdiParent);
			if(client == null)
				throw new InvalidOperationException("//TODO can't find MdiClient");	
			this.clientBounds = client.Bounds;
			this.bounds = new Rectangle(clientBounds.X + 1, clientBounds.Y + 1, clientBounds.Width - 2, clientBounds.Height - 2);
			Populate(MdiParent.MdiChildren);
			EncapsulateClient(client);
			LayoutChanged();
			PatchActiveChild();
		}
		void Depopulate() {
			foreach(XtraMdiTabPage page in new ArrayList(Pages))
				UnregisterMDIChild(page.MdiChild);
			Pages.Clear();
		}
		void Populate(ICollection controls) {
			populating = true;
			try {
				ViewInfo.SetSelectedTabPageCore(null);
				foreach(Control c in controls) {
					RegisterMDIChild(c as Form);
				}
				ViewInfo.SelectedTabPage = Pages[this.MdiParent.ActiveMdiChild];
			}
			finally {
				populating = false;
			}
		}
		protected virtual IMdiClientSubclasser CreateMdiClientSubclasser(MdiClient client) {
			return new XtraMdiClientSubclasser(client, this);
		}
		protected virtual void EncapsulateClient(MdiClient client) {
			if(MdiClientEncapsulator != null) {
				if(ReferenceEquals(client, MdiClientEncapsulator.ClientWindow))
					return;
				throw new NotImplementedException("//TODO MdiClient changing not supported");	
			}
			if(client == null)
				return;
			_encapsulator = CreateMdiClientSubclasser(client);
			client.ControlAdded += new ControlEventHandler(OnMdiClient_ControlAdded);
			client.ControlRemoved += new ControlEventHandler(OnMdiClient_ControlRemoved);
			client.MouseEnter += new EventHandler(OnMdiClient_MouseEnter);
			client.MouseLeave += new EventHandler(OnMdiClient_MouseLeave);
			client.MouseMove += new MouseEventHandler(OnMdiClient_MouseMove);
			client.MouseDown += new MouseEventHandler(OnMdiClient_MouseDown);
			client.MouseUp += new MouseEventHandler(OnMdiClient_MouseUp);
			client.MouseClick += new MouseEventHandler(OnMdiClient_MouseClick);
			if(IsManagerHandlesDragDrop) {
				client.AllowDrop = true;
				client.GiveFeedback += new GiveFeedbackEventHandler(OnMdiClient_GiveFeedback);
				client.DragDrop += new DragEventHandler(OnMdiClient_DragDrop);
				client.DragEnter += new DragEventHandler(OnMdiClient_DragEnter);
				client.DragLeave += new EventHandler(OnMdiClient_DragLeave);
				client.DragOver += new DragEventHandler(OnMdiClient_DragOver);
				client.QueryContinueDrag += new QueryContinueDragEventHandler(OnMdiClient_QueryContinueDrag);
			}
			AddMdiClientToToolTipController(ToolTipController);
		}
		protected virtual void UncapsulateClient() {
			if(MdiClientEncapsulator == null)
				return;
			MdiClient client = MdiClientEncapsulator.ClientWindow;
			RemoveMdiClientFromToolTipController(ToolTipController);
			MdiClientEncapsulator.Dispose();
			_encapsulator = null;
			client.ControlAdded -= new ControlEventHandler(OnMdiClient_ControlAdded);
			client.ControlRemoved -= new ControlEventHandler(OnMdiClient_ControlRemoved);
			client.MouseEnter -= new EventHandler(OnMdiClient_MouseEnter);
			client.MouseLeave -= new EventHandler(OnMdiClient_MouseLeave);
			client.MouseMove -= new MouseEventHandler(OnMdiClient_MouseMove);
			client.MouseDown -= new MouseEventHandler(OnMdiClient_MouseDown);
			client.MouseUp -= new MouseEventHandler(OnMdiClient_MouseUp);
			client.MouseClick -= new MouseEventHandler(OnMdiClient_MouseClick);
			client.GiveFeedback -= new GiveFeedbackEventHandler(OnMdiClient_GiveFeedback);
			client.DragDrop -= new DragEventHandler(OnMdiClient_DragDrop);
			client.DragEnter -= new DragEventHandler(OnMdiClient_DragEnter);
			client.DragLeave -= new EventHandler(OnMdiClient_DragLeave);
			client.DragOver -= new DragEventHandler(OnMdiClient_DragOver);
			client.QueryContinueDrag -= new QueryContinueDragEventHandler(OnMdiClient_QueryContinueDrag);
			if(client.IsHandleCreated)
				MdiClientSubclasser.ProcessNC(client.Handle);
		}
		protected virtual void OnMdiParent_ChildActivate(object sender, EventArgs e) {
			InvokePatchActiveChild();
			Form activeChild = this.MdiParent.ActiveMdiChild;
			if(activeChild != null) {
				if(activeChild.ActiveControl == null || !activeChild.ActiveControl.Visible)
					activeChild.SelectNextControl(activeChild, true, false, true, false);
			}
			if(IsUpdateLocked)
				ViewInfo.SetSelectedTabPageCore(Pages[activeChild]);
			else ViewInfo.SelectedTabPage = Pages[activeChild];
		}
		void InvokePatchActiveChild() {
			if(MdiParent.IsHandleCreated)
				MdiParent.BeginInvoke(new MethodInvoker(AfterChildActivation));
		}
		protected virtual void OnMdiClient_ControlAdded(object sender, ControlEventArgs e) {
			RegisterMDIChild(e.Control as Form);
		}
		protected virtual void OnMdiClient_ControlRemoved(object sender, ControlEventArgs e) {
			UnregisterMDIChild(e.Control as Form);
		}
		protected virtual DXMouseEventArgs CreateDXMouseEventArgs(MouseEventArgs prototype) {
			Point clientLocation = ViewInfo.PageClientBounds.Location;
			return new DXMouseEventArgs(prototype.Button, prototype.Clicks, prototype.X + clientLocation.X, prototype.Y + clientLocation.Y, prototype.Delta);
		}
		void OnMdiClient_MouseEnter(object sender, EventArgs e) {
			OnMouseEnter(e);
		}
		void OnMdiClient_MouseLeave(object sender, EventArgs e) {
			OnMouseLeave(e);
		}
		void OnMdiClient_MouseDown(object sender, MouseEventArgs e) {
			OnMouseDown(CreateDXMouseEventArgs(e));
		}
		void OnMdiClient_MouseUp(object sender, MouseEventArgs e) {
			OnMouseUp(CreateDXMouseEventArgs(e));
		}
		void OnMdiClient_MouseMove(object sender, MouseEventArgs e) {
			OnMouseMove(CreateDXMouseEventArgs(e));
		}
		bool fWaitForSecondClick = false;
		MouseEventArgs lastClickArgs;
		long lastClickTime;
		const long tickLength = 0x2710L;
		void OnMdiClient_MouseClick(object sender, MouseEventArgs e) {
			if(e.Button != MouseButtons.Left) return;
			if(fWaitForSecondClick) {
				if(IsDoubleClick(e)) {
					OnDoubleClick(CreateDXMouseEventArgs(e));
					fWaitForSecondClick = false;
				}
				else SaveFirstClickInfo(e);
			}
			else SaveFirstClickInfo(e);
		}
		protected Point PointToClient(Point point) {
			if(MdiClientEncapsulator == null)
				return Point.Empty;
			Point clientCoord = MdiClientEncapsulator.ClientWindow.PointToClient(point);
			Point clientLocation = ViewInfo.PageClientBounds.Location;
			return new Point(clientCoord.X + clientLocation.X, clientCoord.Y + clientLocation.Y);
		}
		protected Point PointToScreen(Point point) {
			if(MdiClientEncapsulator == null) return Point.Empty;
			Point screenPoint = MdiClientEncapsulator.ClientWindow.PointToScreen(point);
			Point clientLocation = ViewInfo.PageClientBounds.Location;
			return new Point(screenPoint.X - clientLocation.X, screenPoint.Y - clientLocation.Y);
		}
		protected virtual void OnDoubleClick(DXMouseEventArgs e) {
			BaseTabHitInfo clickInfo = ViewInfo.CalcHitInfo(e.Location);
			if(clickInfo.HitTest == XtraTabHitTest.PageHeader) {
				if(CanFloatOnDoubleClick()) {
					XtraMdiTabPage page = (XtraMdiTabPage)clickInfo.Page;
					if(Float(page, PointToScreen(e.Location))) {
						using(FloatForms.Lock()) {
							RaiseEndFloating(page.MdiChild, false);
						}
					}
				}
			}
		}
		bool CanFloatOnDoubleClick() {
			return FloatOnDoubleClick == DefaultBoolean.True && !DesignMode;
		}
		bool CanFloatOnDrag() {
			return FloatOnDrag == DefaultBoolean.True && !DesignMode;
		}
		bool IsDoubleClick(MouseEventArgs e) {
			return CheckLocation(e.Location) && CheckTime(DateTime.Now.Ticks);
		}
		bool CheckLocation(Point point) {
			bool conditionX = Math.Abs(point.X - lastClickArgs.X) <= SystemInformation.DoubleClickSize.Width;
			bool conditionY = Math.Abs(point.Y - lastClickArgs.Y) <= SystemInformation.DoubleClickSize.Height;
			return conditionX && conditionY;
		}
		bool CheckTime(long time) {
			return ((int)(Math.Abs(lastClickTime - time) / tickLength)) <= SystemInformation.DoubleClickTime;
		}
		void SaveFirstClickInfo(MouseEventArgs e) {
			lastClickArgs = e;
			lastClickTime = System.DateTime.Now.Ticks;
			fWaitForSecondClick = true;
		}
		protected virtual void OnMouseEnter(EventArgs e) {
			if(MouseEnter != null)
				MouseEnter(this, e);
		}
		protected virtual void OnMouseLeave(EventArgs e) {
			if(MouseLeave != null)
				MouseLeave(this, e);
			Handler.ProcessEvent(EventType.MouseLeave, e);
			if(IsManagerHandlesDragDrop) {
				this.dragStartBox = Rectangle.Empty;
			}
		}
		Point dragStartOffset;
		protected virtual void OnMouseDown(DXMouseEventArgs e) {
			if(MouseDown != null)
				MouseDown(this, e);
			if(!e.Handled) {
				e.Handled = Handler.ProcessEvent(EventType.MouseDown, e) == EventResult.Handled;
				if(IsManagerHandlesDragDrop && e.Button == MouseButtons.Left && Control.MouseButtons == MouseButtons.Left) {
					if(SelectedPage != null) {
						BaseTabHitInfo hitInfo = CalcHitInfo(e.Location);
						if(hitInfo.IsValid && hitInfo.HitTest == XtraTabHitTest.PageHeader && ReferenceEquals(SelectedPage, hitInfo.Page) && !hitInfo.InPageControlBox) {
							Rectangle dragPage = ViewInfo.HeaderInfo.VisiblePages[SelectedPage].Bounds;
							Point clientLocation = ViewInfo.PageClientBounds.Location;
							dragStartOffset = new Point(e.X - clientLocation.X - dragPage.X, e.Y - clientLocation.Y - dragPage.Y);
							Size dragSize = SystemInformation.DragSize;
							this.dragStartBox = new Rectangle(new Point(e.X - dragSize.Width / 2, e.Y - dragSize.Height / 2), dragSize);
							e.Handled = true;
						}
					}
				}
			}
		}
		protected virtual void OnMouseUp(DXMouseEventArgs e) {
			if(MouseUp != null)
				MouseUp(this, e);
			if(!e.Handled) {
				e.Handled = Handler.ProcessEvent(EventType.MouseUp, e) == EventResult.Handled;
			}
			if(IsManagerHandlesDragDrop) {
				this.dragStartBox = Rectangle.Empty;
			}
		}
		protected virtual void OnMouseMove(MouseEventArgs e) {
			if(MouseMove != null)
				MouseMove(this, e);
			Handler.ProcessEvent(EventType.MouseMove, e);
			if(IsManagerHandlesDragDrop) {
				if(!(this.dragStartBox.IsEmpty || this.dragStartBox.Contains(e.X, e.Y))) {
					this.dragStartBox = Rectangle.Empty;
					DoDragDrop();
				}
			}
		}
		protected virtual void OnPageAdded(MdiTabPageEventArgs e) {
			if(IsInitialize)
				return;
			if(e.Page != null)
				e.Page.OnAddedToManager();
			if(PageAdded != null)
				PageAdded(this, e);
			if(!InTheAir)
				return;
			if(ViewInfo != null) {
				ViewInfo.OnPageAdded(e.Page);
			}
			ReflectPageClientSizeChangedIfNeeded();
		}
		protected virtual void OnPageRemoved(MdiTabPageEventArgs e) {
			if(IsInitialize)
				return;
			e.Page.OnRemovedFromManager();
			if(PageRemoved != null)
				PageRemoved(this, e);
			if(!InTheAir)
				return;
			if(ViewInfo != null) {
				ViewInfo.OnPageRemoved(e.Page);
			}
			ReflectPageClientSizeChangedIfNeeded();
		}
		public virtual void LayoutChanged() {
			if(!InTheAir)
				return;
			ViewInfo.LayoutChanged();
			ReflectPageClientSizeChangedIfNeeded();
		}
		bool CanUseFormIconAsPageImage() {
			return UseFormIconAsPageImage == DefaultBoolean.True;
		}
		protected virtual XtraMdiTabPage CreateNewPage(Form child) {
			return new XtraMdiTabPage(this, child, CanUseFormIconAsPageImage());
		}
		protected virtual void PatchMaximized(Form form) {
			if(form == null)
				return;
			if(form.WindowState != FormWindowState.Normal)
				form.WindowState = FormWindowState.Normal;
		}
		internal void SortPinnedItems() {
			if(Pages != null || Pages.Count > 1) {
				XtraMdiTabPage savedSelectedPage = SelectedPage;
				int i = 0;
				foreach(XtraMdiTabPage item in GetPinnedPages()) {
					Pages.Remove(item);
					Pages.Insert(i, item);
					i++;
				}
			}
			LayoutChanged();
		}
		List<XtraMdiTabPage> GetPinnedPages() {
			List<XtraMdiTabPage> result = new List<XtraMdiTabPage>();
			foreach(XtraMdiTabPage document in Pages) {
				if(document.Pinned)
					result.Add(document);
			}
			return result;
		}
		protected virtual void RegisterMDIChild(Form child) {
			if(child == null)
				return;
			PatchBeforeRegister(child);
			XtraMdiTabPage newPage;
			if(!PageAssociation.TryGetValue(child, out newPage)) {
				newPage = CreateNewPage(child);
			}
			else newPage.tabControl = this;
			AddPage(newPage);
			child.SizeChanged += new EventHandler(MDIChild_SizeChanged);
			OnPageAdded(new MdiTabPageEventArgs(newPage));
		}
		void AddPage(XtraMdiTabPage newPage) {
			if(IsCancelFloating && Pages.IsValid(floatFormCancelPageIndex)) {
				Pages.Insert(floatFormCancelPageIndex, newPage);
				return;
			}
			if(IsDocking && Pages.IsValid(floatFormDockingPageIndex)) {
				Pages.Insert(floatFormDockingPageIndex, newPage);
				return;
			}
			Pages.Add(newPage);
		}
		protected virtual void MDIChild_SizeChanged(object sender, EventArgs e) {
			Form frm = sender as Form;
			if(frm == null)
				return;
			if(frm.WindowState == FormWindowState.Normal)
				return;
			frm.BeginInvoke(new EventHandler(DeferredMaximizedPatcher));
		}
		protected virtual void DeferredMaximizedPatcher(object sender, EventArgs e) {
			if(!InTheAir)
				return;
			PatchActiveChild();
		}
		protected virtual void UnregisterMDIChild(Form child) {
			if(child == null)
				return;
			XtraMdiTabPage page = Pages[child];
			if(page == null)
				return;
			child.SizeChanged -= new EventHandler(MDIChild_SizeChanged);
			Pages.Remove(page);
			OnPageRemoved(new MdiTabPageEventArgs(page));
			if(!this.Disposing && !child.Disposing) {
				PatchAfterUnregister(child);
			}
			if(child.Disposing) {
				((IDisposable)page).Dispose();
			}
		}
		protected virtual void PatchBeforeRegister(Form child) {
			MdiChildHelper.PatchBeforeRegister(child);
		}
		protected virtual void PatchAfterUnregister(Form child) {
			MdiChildHelper.PatchAfterUnregister(child);
		}
		void AfterChildActivation() {
			if(!InTheAir)
				return;
			PatchActiveChild();
		}
		protected virtual void PatchActiveChild() {
			Form child = MdiParent.ActiveMdiChild;
			if(child != null) {
				PatchMaximized(child);
				if(child.Dock != DockStyle.Fill) {
					Size newChildSize = ViewInfo.PageClientBounds.Size;
					Point newChildLocation = new Point(0, 0);
					Rectangle newChildBounds = new Rectangle(newChildLocation, newChildSize);
					if(child.Bounds.IntersectsWith(newChildBounds)) {
						child.Bounds = newChildBounds;
					}
					else {
						Point invisibleOrigin = new Point(-newChildSize.Width, -newChildSize.Height);
						Rectangle invisibleBounds = new Rectangle(invisibleOrigin, newChildSize);
						child.Bounds = invisibleBounds;
					}
					child.Dock = DockStyle.Fill;
				}
			}
			foreach(XtraMdiTabPage passivePage in Pages) {
				Form passive = passivePage.MdiChild;
				if(passive != null && !ReferenceEquals(passive, child)) {
					if(passive.Dock == DockStyle.Fill) {
						passive.SuspendLayout();
						try {
							Rectangle remembered = passive.Bounds;
							passive.Dock = DockStyle.None;
							passive.Bounds = remembered;
						}
						finally {
							passive.ResumeLayout();
						}
					}
					PatchMaximized(passive);
				}
			}
		}
		protected virtual void PatchBeforeActiveChild(Form activatedForm) {
			Rectangle r = new Rectangle(Point.Empty, ViewInfo.PageClientBounds.Size);
			MdiChildHelper.PatchBeforeActivateChild(activatedForm, MdiParent, r);
		}
		protected virtual Rectangle CalculateNC(Rectangle newBounds) {
			Rectangle rebasedNewBounds = new Rectangle(0, 0, newBounds.Width, newBounds.Height);
			SetBounds(rebasedNewBounds);
			cachedViewInfoPageClientBounds = ViewInfo.PageClientBounds;
			Rectangle clientBounds = cachedViewInfoPageClientBounds;
			clientBounds.Offset(newBounds.Location);
			if(GetDrawEmptyPageClient()) {
				clientBounds.Size = Size.Empty;
			}
			return clientBounds;
		}
		protected virtual void DrawNC() {
			if(!InTheAir) return;
			((XtraMdiClientSubclasser)MdiClientEncapsulator).DrawNC(clientBounds);
		}
		int lockUpdate;
		protected bool IsUpdateLocked {
			get { return lockUpdate > 0; }
		}
		public void BeginUpdate() {
			lockUpdate++;
			if(ViewInfo != null)
				ViewInfo.BeginUpdate();
		}
		public void EndUpdate() {
			if(--lockUpdate == 0)
				OnUnlockUpdate();
		}
		protected void OnUnlockUpdate() {
			if(ViewInfo != null) {
				ViewInfo.EndUpdate();
				ViewInfo.FirstVisiblePageIndex = Pages.IndexOf(SelectedPage);
				ViewInfo.CheckFirstPageIndex();
			}
			ProcessNC();
		}
		protected virtual void ProcessNC() {
			if(IsUpdateLocked) return;
			MdiClientEncapsulator.ProcessNC();
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("XtraTabbedMdiManagerController"),
#endif
 Category(XtraTabbedMdiCategoryName.Appearance), DefaultValue(null)]
		public virtual BarAndDockingController Controller {
			get { return controller; }
			set {
				if(Controller == value) return;
				GetControllerInternal().RemoveClient(this);
				this.controller = value;
				GetControllerInternal().AddClient(this);
				OnController_Changed(this);
			}
		}
		bool IXtraTab.RightToLeftLayout { get { return IsRightToLeftLayout(); } }
		UserLookAndFeel IXtraTab.LookAndFeel { get { return GetControllerInternal().LookAndFeel; } }
		BarAndDockingController GetControllerInternal() { return controller != null ? controller : BarAndDockingController.Default; }
		int IXtraTab.PageCount { get { return Pages.Count; } }
		IXtraTabPage IXtraTab.GetTabPage(int index) { return Pages[index]; }
		BaseViewInfoRegistrator IXtraTab.View { get { return View; } }
		Rectangle IXtraTab.Bounds { get { return Bounds; } }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("XtraTabbedMdiManagerHeaderLocation"),
#endif
 Category(XtraTabbedMdiCategoryName.Appearance), DefaultValue(TabHeaderLocation.Top)]
		public virtual TabHeaderLocation HeaderLocation {
			get { return headerLocation; }
			set {
				if(HeaderLocation == value) return;
				headerLocation = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("XtraTabbedMdiManagerMaxTabPageWidth"),
#endif
 Category(CategoryName.Appearance), DefaultValue(0)]
		public virtual int MaxTabPageWidth {
			get { return maxTabPageWidthCore; }
			set {
				if(MaxTabPageWidth == value) return;
				maxTabPageWidthCore = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("XtraTabbedMdiManagerHeaderOrientation"),
#endif
 Category(XtraTabbedMdiCategoryName.Appearance), DefaultValue(TabOrientation.Default)]
		public virtual TabOrientation HeaderOrientation {
			get { return headerOrientation; }
			set {
				if(HeaderOrientation == value) return;
				headerOrientation = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("XtraTabbedMdiManagerImages"),
#endif
 Category(XtraTabbedMdiCategoryName.Appearance),
		DefaultValue(null),
		TypeConverter(typeof(DevExpress.Utils.Design.ImageCollectionImagesConverter)),
		]
		public virtual object Images {
			get {
				return this.images;
			}
			set {
				if(Images == value)
					return;
				this.images = value;
				LayoutChanged();
			}
		}
		void IXtraTab.OnPageChanged(IXtraTabPage page) {
			if(ViewInfo != null) ViewInfo.OnPageChanged(page);
			LayoutChanged();
		}
		BaseTabHitInfo IXtraTab.CreateHitInfo() { return new BaseTabHitInfo(); }
		BaseTabControlViewInfo IXtraTab.ViewInfo { get { return ViewInfo; } }
		Control IXtraTab.OwnerControl { get { return (MdiClientEncapsulator != null) ? this.MdiClientEncapsulator.ClientWindow : null; } }
		protected virtual void InvalidateNC() {
			if(!InTheAir)
				return;
			invalidated = true;
			this.MdiClientEncapsulator.ClientWindow.BeginInvoke(new EventHandler(UpdateNCIfNeeded));
		}
		private void UpdateNCIfNeeded(object sender, EventArgs e) {
			if(!InTheAir)
				return;
			if(invalidated) {
				invalidated = false;
				this.DrawNC();
			}
		}
		public virtual void Invalidate() {
			if(!InTheAir)
				return;
			InvalidateNC();
		}
		public virtual void Invalidate(Rectangle rect) {
			Invalidate();
		}
		protected virtual void ReflectPageClientSizeChangedIfNeeded() {
			if(!InTheAir || IsUpdateLocked)
				return;
			if(ViewInfo.PageClientBounds != cachedViewInfoPageClientBounds
				|| ViewInfo.PageClientBounds.Size != MdiClientEncapsulator.ClientWindow.ClientRectangle.Size)
				ProcessNC();
		}
		protected virtual void DrawNC(DXPaintEventArgs e) {
			if(e.ClipRectangle.Height <= 0 || e.ClipRectangle.Width <= 0)
				return;
			ViewInfo.FillPageClient = GetDrawEmptyPageClient();
			using(GraphicsCache cache1 = new GraphicsCache(e)) {
				if(!ViewInfo.FillPageClient)
					cache1.ClipInfo.ExcludeClip(ViewInfo.PageClientBounds);
				BaseTabControlViewInfo paintViewInfo = this.ViewInfo;
				BaseTabPainter painter = this.Painter;
				if(this.dragDropPainterOverride != null) {
					paintViewInfo = this.dragDropPainterOverride.ViewInfo;
					painter = this.dragDropPainterOverride.View.CreatePainter(this.dragDropPainterOverride);
				}
				using(XtraBufferedGraphics bg = XtraBufferedGraphicsManager.Current.Allocate(cache1.Graphics, e.ClipRectangle)) {
					using(GraphicsCache cache2 = new GraphicsCache(bg.Graphics)) {
						TabDrawArgs drawArgs = new TabDrawArgs(cache2, paintViewInfo, Bounds);
						DrawTabBackground(drawArgs);
						painter.Draw(drawArgs);
					}
					bg.Render();
				}
			}
		}
		protected virtual void DrawTabBackground(TabDrawArgs drawArgs) {
			IXtraTab tab = this as IXtraTab;
			Color parentBackColor = (tab.OwnerControl != null && tab.OwnerControl.Parent != null) ? tab.OwnerControl.Parent.BackColor :
				LookAndFeelHelper.GetSystemColor(tab.LookAndFeel, SystemColors.Control);
			if(MdiParent.RightToLeftLayout)
				clientBounds.X--;
			drawArgs.Cache.FillRectangle(drawArgs.Cache.GetSolidBrush(parentBackColor), clientBounds);
		}
		protected virtual internal BaseTabControlViewInfo ViewInfo { get { return viewInfo; } }
		protected virtual internal BaseTabPainter Painter { get { return painter; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public virtual BaseTabHandler Handler { get { return handler; } }
		bool _disposing = false;
		protected bool Disposing {
			get { return this._disposing; }
		}
		protected override void Dispose(bool disposing) {
			this._disposing = true;
			if(disposing) {
				UnCaptureFormMoving();
				if(dropTimerCore != null) {
					dropTimerCore.Tick -= OnDropTimerTick;
					dropTimerCore.Dispose();
					dropTimerCore = null;
				}
				if(FloatForms != null) {
					FloatForms.Dispose();
					floatFormsCore = null;
				}
				if(Pages != null)
					Pages.Dispose();
				((IXtraTab)this).LookAndFeel.StyleChanged -= OnLookAndFeelStyleChanged;
				GetControllerInternal().RemoveClient(this);
				if(this.viewInfo != null)
					this.viewInfo.Dispose();
				this.viewInfo = null;
				this.MdiParent = null;
				DestroyDragFrame();
				DestroyDropHint();
			}
			base.Dispose(disposing);
		}
		protected virtual void ClearPages() {
			foreach(XtraMdiTabPage page in Pages) {
				page.ClearImageHeleper();
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public virtual XtraMdiTabPage SelectedPage {
			get { return (XtraMdiTabPage)ViewInfo.SelectedTabPage; }
			set {
				ViewInfo.SelectedTabPage = value;
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public virtual Rectangle Bounds {
			get { return bounds; }
		}
		protected virtual void SetBounds(Rectangle newValue) {
			if(clientBounds == newValue) return;
			clientBounds = newValue;
			bounds = new Rectangle(clientBounds.X + 1, clientBounds.Y + 1, clientBounds.Width - 2, clientBounds.Height - 2);
			ViewInfo.Resize();
		}
		protected virtual void CheckInfo() {
			if(View != GetView()) CreateView();
		}
		protected virtual BaseViewInfoRegistrator GetView() {
			return DevExpress.XtraTab.Registrator.PaintStyleCollection.DefaultPaintStyles.GetView(GetControllerInternal().LookAndFeel, GetTabPaintStyleName(GetControllerInternal().GetPaintStyleName()));
		}
		protected virtual string GetTabPaintStyleName(string barPaintStyleName) {
			switch(barPaintStyleName) {
				case "Default":
				case "Office2003": return "Office2003";
				case "Office2000": return "Standard";
				case "OfficeXP": return "Flat";
				case "WindowsXP": return "WindowsXP";
				case "Skin": return "Skin";
			}
			return BaseViewInfoRegistrator.DefaultViewName;
		}
		protected virtual void CreateView() {
			this.view = GetView();
			IXtraTabPage prevPage = null;
			if(ViewInfo != null) {
				prevPage = ViewInfo.SelectedTabPage;
				ViewInfo.SelectedPageChanged -= OnSelectedPageChanged;
				ViewInfo.TabMiddleClick -= OnTabMiddleClick;
				ViewInfo.CloseButtonClick -= OnCloseButtonClick;
				ViewInfo.Dispose();
			}
			this.viewInfo = View.CreateViewInfo(this);
			this.painter = View.CreatePainter(this);
			this.handler = View.CreateHandler(this);
			if(ViewInfo != null) {
				if(prevPage != null) ViewInfo.SetSelectedTabPageCore(prevPage);
				ViewInfo.DrawPaneWhenEmpty = false;
				ViewInfo.SetDefaultTabMiddleClickFiringMode(TabMiddleClickFiringMode.MouseDown);
				ViewInfo.SelectedPageChanged += OnSelectedPageChanged;
				ViewInfo.TabMiddleClick += OnTabMiddleClick;
				ViewInfo.CloseButtonClick += OnCloseButtonClick;
			}
		}
		void OnLookAndFeelStyleChanged(object sender, EventArgs e) {
			if(ViewInfo != null) ViewInfo.ResetDefaultAppearances();
			LayoutChanged();
		}
		protected virtual void OnSelectedPageChanged(object sender, ViewInfoTabPageChangedEventArgs e) {
			if(populating)
				return;
			if(e.Page != null) {
				XtraMdiTabPage page = (XtraMdiTabPage)e.Page;
				if(!ReferenceEquals(page.MdiChild, MdiParent.ActiveMdiChild)) {
					PatchBeforeActiveChild(page.MdiChild);
					page.MdiChild.Select();
				}
			}
			if(SelectedPageChanged != null)
				SelectedPageChanged(this, EventArgs.Empty);
		}
		protected virtual void OnTabMiddleClick(object sender, PageEventArgs e) {
			Form activeForm = MdiParent.ActiveMdiChild;
			XtraMdiTabPage page = null;
			if(e != null && e.Page is XtraMdiTabPage) {
				page = e.Page as XtraMdiTabPage;
				activeForm = page.MdiChild;
			}
			CloseForm(page, activeForm);
		}
		protected virtual void OnCloseButtonClick(object sender, EventArgs e) {
			Form activeForm = MdiParent.ActiveMdiChild;
			XtraMdiTabPage page = null;
			ClosePageButtonEventArgs ea = e as ClosePageButtonEventArgs;
			if(ea != null && ea.Page is XtraMdiTabPage) {
				page = ea.Page as XtraMdiTabPage;
				activeForm = page.MdiChild;
			}
			CloseForm(page, activeForm);
		}
		void CloseForm(XtraMdiTabPage page, Form activeForm) {
			if(activeForm == null || ControlState.IsCreatingHandle(activeForm)) return;
			if(!RaisePageClosing(page)) return;
			CloseForm(activeForm);
		}
		bool RaisePageClosing(XtraMdiTabPage page) {
			MdiTabPageCancelEventArgs ea = new MdiTabPageCancelEventArgs(page);
			MdiTabPageCancelEventHandler handler = PageClosing;
			if(handler != null) handler(this, ea);
			return !ea.Cancel;
		}
		void CloseForm(Form activeForm) {
			Control.ControlCollection collection = MdiClientEncapsulator.ClientWindow.Controls;
			if(collection.Count > 1 && ReferenceEquals(activeForm, collection[0])) {
				PatchBeforeActiveChild(collection[1] as Form);
			}
			ViewInfo.BeginUpdate();
			activeForm.Close();
			ViewInfo.FirstVisiblePageIndex = Pages.IndexOf(SelectedPage);
			ViewInfo.EndUpdate();
			ReflectPageClientSizeChangedIfNeeded();
			ViewInfo.CheckFirstPageIndex();
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public virtual XtraMdiTabPageCollection Pages { get { return pages; } }
		protected virtual BaseViewInfoRegistrator View { get { return view; } }
		#region IXtraTabProperties
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("XtraTabbedMdiManagerTabPageWidth"),
#endif
 Category(CategoryName.Appearance), DefaultValue(0)]
		public virtual int TabPageWidth {
			get { return tabPageWidthCore; }
			set {
				if(TabPageWidth == value) return;
				tabPageWidthCore = value;
				LayoutChanged();
			}
		}
		DefaultBoolean IXtraTabProperties.AllowHotTrack { get { return DefaultBoolean.True; } }
		bool ShouldSerializeAppearance() { return Appearance.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("XtraTabbedMdiManagerAppearance"),
#endif
 Category(XtraTabbedMdiCategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Appearance {
			get { return appearance; }
		}
		bool ShouldSerializeAppearancePage() { return AppearancePage.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("XtraTabbedMdiManagerAppearancePage"),
#endif
 Category(XtraTabbedMdiCategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PageAppearance AppearancePage {
			get { return appearancePage; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("XtraTabbedMdiManagerBorderStyle"),
#endif
 Category(XtraTabbedMdiCategoryName.Appearance), DefaultValue(BorderStyles.Default)]
		public virtual BorderStyles BorderStyle {
			get { return borderStyle; }
			set {
				if(BorderStyle == value) return;
				borderStyle = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("XtraTabbedMdiManagerBorderStylePage"),
#endif
 Category(XtraTabbedMdiCategoryName.Appearance), DefaultValue(BorderStyles.Default)]
		public virtual BorderStyles BorderStylePage {
			get { return borderStylePage; }
			set {
				if(BorderStylePage == value) return;
				borderStylePage = value;
				LayoutChanged();
			}
		}
		[Category(XtraTabbedMdiCategoryName.Appearance), DefaultValue(DefaultBoolean.Default)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual DefaultBoolean HeaderAutoFill {
			get { return headerAutoFill; }
			set {
				if(HeaderAutoFill == value) return;
				headerAutoFill = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("XtraTabbedMdiManagerHeaderButtons"),
#endif
 Category(XtraTabbedMdiCategoryName.Behavior), DefaultValue(TabButtons.Default),
		Editor(typeof(DevExpress.Utils.Editors.AttributesEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public virtual TabButtons HeaderButtons {
			get { return headerButtons; }
			set {
				if(HeaderButtons == value) return;
				headerButtons = value;
				LayoutChanged();
			}
		}
		TabButtons IXtraTabProperties.HeaderButtons {
			get { return CalcTabButtons(ClosePageButtonShowMode); }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("XtraTabbedMdiManagerHeaderButtonsShowMode"),
#endif
 Category(XtraTabbedMdiCategoryName.Behavior), DefaultValue(TabButtonShowMode.Default)]
		public TabButtonShowMode HeaderButtonsShowMode {
			get { return headerButtonsShowMode; }
			set {
				if(HeaderButtonsShowMode == value) return;
				headerButtonsShowMode = value;
				LayoutChanged();
			}
		}
		[Category(XtraTabbedMdiCategoryName.Appearance), DefaultValue(DefaultBoolean.Default)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual DefaultBoolean MultiLine {
			get { return multiLine; }
			set {
				if(!IsInitialize) {
					if(!ViewInfo.IsAllowMultiLine && value == DefaultBoolean.True)
						value = DefaultBoolean.Default;
				}
				if(MultiLine == value)
					return;
				multiLine = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("XtraTabbedMdiManagerPageImagePosition"),
#endif
 Category(XtraTabbedMdiCategoryName.Appearance), DefaultValue(TabPageImagePosition.Near)]
		public virtual TabPageImagePosition PageImagePosition {
			get { return pageImagePosition; }
			set {
				if(PageImagePosition == value) return;
				pageImagePosition = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("XtraTabbedMdiManagerShowHeaderFocus"),
#endif
 Category(XtraTabbedMdiCategoryName.Appearance), DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean ShowHeaderFocus {
			get { return showHeaderFocus; }
			set {
				if(ShowHeaderFocus == value) return;
				showHeaderFocus = value;
				LayoutChanged();
			}
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("XtraTabbedMdiManagerClosePageButtonShowMode")]
#endif
		[Category(XtraTabbedMdiCategoryName.Behavior), DefaultValue(ClosePageButtonShowMode.Default)]
		public virtual ClosePageButtonShowMode ClosePageButtonShowMode {
			get { return closePageButtonShowModeCore; }
			set {
				if(ClosePageButtonShowMode == value) return;
				closePageButtonShowModeCore = value;
				LayoutChanged();
			}
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("XtraTabbedMdiManagerPinPageButtonShowMode")]
#endif
		[Category(XtraTabbedMdiCategoryName.Behavior), DefaultValue(PinPageButtonShowMode.Default)]
		public virtual PinPageButtonShowMode PinPageButtonShowMode {
			get { return pinPageButtonShowModeCore; }
			set {
				if(PinPageButtonShowMode == value) return;
				pinPageButtonShowModeCore = value;
				LayoutChanged();
			}
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("XtraTabbedMdiManagerAllowGlyphSkinning")]
#endif
		[DefaultValue(DefaultBoolean.Default)]
		public virtual DefaultBoolean AllowGlyphSkinning {
			get { return allowGlyphSkinningCore; }
			set {
				if(AllowGlyphSkinning == value) return;
				allowGlyphSkinningCore = value;
				LayoutChanged();
			}
		}
		protected virtual bool CanShowCloseButtonForMdiPageAsForm(XtraMdiTabPage page) {
			return page != null && page.MdiChild.ControlBox == false;
		}
		protected virtual TabButtons CalcTabButtons(ClosePageButtonShowMode mode) {
			TabButtons buttons = this.HeaderButtons;
			if((buttons & TabButtons.Default) == TabButtons.Default)
				buttons = TabButtons.Close | TabButtons.Prev | TabButtons.Next;
			return buttons;
		}
		DefaultBoolean IXtraTabProperties.ShowTabHeader { get { return DefaultBoolean.True; } }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("XtraTabbedMdiManagerShowToolTips"),
#endif
 Category(XtraTabbedMdiCategoryName.Appearance), DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean ShowToolTips {
			get { return showToolTips; }
			set { showToolTips = value; }
		}
		#endregion
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("XtraTabbedMdiManagerSetNextMdiChildMode"),
#endif
 Category(XtraTabbedMdiCategoryName.Behavior), DefaultValue(SetNextMdiChildMode.Default)]
		public virtual SetNextMdiChildMode SetNextMdiChildMode {
			get { return this.mdiNextChildMode; }
			set { this.mdiNextChildMode = value; }
		}
		XtraMdiTabPage documentStartSelectorPage;
		bool CanUseDocumentSelector() {
			return UseDocumentSelector == DefaultBoolean.True;
		}
		void IMdiClientSubclasserOwner.HandleCreated() {
		}
		void IMdiClientSubclasserOwner.HandleDestroyed() {
		}
		void IMdiClientSubclasserOwner.OnSetNextMdiChild(SetNextMdiChildEventArgs e) {
			OnSetNextMdiChildCore(e);
		}
		void IMdiClientSubclasserOwner.OnContextMenu() {
			OnContextMenuCore();
		}
		Rectangle IMdiClientSubclasserOwner.CalculateNC(Rectangle bounds) {
			return CalculateNC(bounds);
		}
		void IMdiClientSubclasserOwner.InvalidateNC() {
			Invalidate();
		}
		void IMdiClientSubclasserOwner.Paint(Graphics g) {
			throw new NotImplementedException("IMdiClientSubclasserOwner.Paint");
		}
		void IMdiClientSubclasserOwner.EraseBackground(Graphics g) {
			throw new NotImplementedException("IMdiClientSubclasserOwner.EraseBackground");
		}
		void IMdiClientSubclasserOwner.DrawNC(DXPaintEventArgs e) {
			DrawNC(e);
		}
		bool IMdiClientSubclasserOwner.AllowMdiLayout {
			get { return false; }
		}
		bool IMdiClientSubclasserOwner.AllowMdiSystemMenu {
			get { return false; }
		}
		protected virtual void OnContextMenuCore() {
		}
		protected virtual void OnSetNextMdiChildCore(SetNextMdiChildEventArgs e) {
			if(SetNextMdiChild != null)
				SetNextMdiChild(this, e);
			if(e.Handled)
				return;
			if(CanUseDocumentSelector()) {
				e.Handled = true;
				XtraMdiTabPage nextPage = SelectedPage;
				if(documentStartSelectorPage == nextPage)
					documentStartSelectorPage = null;
				if(ShowDocumentSelector(documentStartSelectorPage, ref nextPage)) {
					documentStartSelectorPage = SelectedPage;
					ViewInfo.SelectedTabPage = nextPage;
					return;
				}
			}
			switch(SetNextMdiChildMode) {
				case SetNextMdiChildMode.Default:
				case SetNextMdiChildMode.TabControl:
					e.Handled = true;
					ViewInfo.SelectNextPage(e.ForwardNavigation ? 1 : -1);
					break;
				case SetNextMdiChildMode.Windows:
					break;
				default:
					throw new NotImplementedException(SetNextMdiChildMode.ToString());	
			}
		}
		public virtual BaseTabHitInfo CalcHitInfo(Point point) {
			return ViewInfo.CalcHitInfo(point);
		}
		protected virtual void OnAppearanceChanged(object sender, EventArgs e) {
			LayoutChanged();
		}
		FloatFormCollection floatFormsCore;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public FloatFormCollection FloatForms {
			get { return floatFormsCore; }
		}
		protected Timer DropTimer {
			get {
				if(dropTimerCore == null) {
					dropTimerCore = new Timer();
					dropTimerCore.Tick += OnDropTimerTick;
				}
				return dropTimerCore;
			}
		}
		const int defaultDockDelay = 100;
		int dockDelayCore = defaultDockDelay;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("XtraTabbedMdiManagerFloatMDIChildDockDelay"),
#endif
		Category(XtraTabbedMdiCategoryName.Behavior), DefaultValue(defaultDockDelay)]
		public int FloatMDIChildDockDelay {
			get { return dockDelayCore; }
			set { dockDelayCore = Math.Min(5000, Math.Max(value, 100)); }
		}
		DefaultBoolean showDropHintCore = DefaultBoolean.Default;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("XtraTabbedMdiManagerShowFloatingDropHint"),
#endif
	   Category(XtraTabbedMdiCategoryName.Behavior), DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean ShowFloatingDropHint {
			get { return showDropHintCore; }
			set { showDropHintCore = value; }
		}
		DefaultBoolean useDocumentSelectorCore = DefaultBoolean.Default;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("XtraTabbedMdiManagerUseDocumentSelector"),
#endif
	   Category(XtraTabbedMdiCategoryName.Behavior), DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean UseDocumentSelector {
			get { return useDocumentSelectorCore; }
			set { useDocumentSelectorCore = value; }
		}
		DefaultBoolean useFormIconAsPageImageCore = DefaultBoolean.Default;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("XtraTabbedMdiManagerUseFormIconAsPageImage"),
#endif
	   Category(XtraTabbedMdiCategoryName.Behavior), DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean UseFormIconAsPageImage {
			get { return useFormIconAsPageImageCore; }
			set { useFormIconAsPageImageCore = value; }
		}
		float previewPageZoomRatioCore = 0.5f;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("XtraTabbedMdiManagerPreviewPageZoomRatio"),
#endif
	   Category(XtraTabbedMdiCategoryName.Behavior), DefaultValue(0.5f)]
		public float PreviewPageZoomRatio {
			get { return previewPageZoomRatioCore; }
			set { previewPageZoomRatioCore = (float)Math.Min(1.0f, Math.Max(value, 0.25)); }
		}
		FloatPageDragMode floatPageDragModeCore = FloatPageDragMode.Default;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("XtraTabbedMdiManagerFloatPageDragMode"),
#endif
	  Category(XtraTabbedMdiCategoryName.Behavior), DefaultValue(FloatPageDragMode.Default)]
		public FloatPageDragMode FloatPageDragMode {
			get { return floatPageDragModeCore; }
			set {
				if(floatPageDragModeCore == value) return;
				floatPageDragModeCore = value;
				OnFloatPageDragModeChanged();
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public Form ActiveFloatForm {
			get {
				foreach(Form frm in FloatForms) {
					if(frm.ContainsFocus) return frm;
				}
				return null;
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("XtraTabbedMdiManagerFloatOnDoubleClick"),
#endif
		Category(XtraTabbedMdiCategoryName.Behavior), DefaultValue(DefaultBoolean.Default)]
		public virtual DefaultBoolean FloatOnDoubleClick {
			get { return floatOnDoubleClick; }
			set { floatOnDoubleClick = value; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("XtraTabbedMdiManagerFloatOnDrag"),
#endif
		Category(XtraTabbedMdiCategoryName.Behavior), DefaultValue(DefaultBoolean.Default)]
		public virtual DefaultBoolean FloatOnDrag {
			get { return floatOnDrag; }
			set { floatOnDrag = value; }
		}
		protected virtual void OnFloatPageDragModeChanged() {
		}
		protected virtual FloatFormCollection CreateFloatFormsCollection() {
			return new FloatFormCollection(this);
		}
		public event FloatingCancelEventHandler BeginFloating;
		public event FloatingEventHandler Floating;
		public event FloatingEventHandler EndFloating;
		public event FloatingCancelEventHandler BeginDocking;
		public event FloatingEventHandler EndDocking;
		public event EventHandler FloatMDIChildActivated;
		public event EventHandler FloatMDIChildDeactivated;
		public event FloatMDIChildDraggingEventHandler FloatMDIChildDragging;
		protected internal virtual void OnFloatFormActivated(Form form) {
			RaiseFloatMDIChildActivated(form);
		}
		protected internal virtual void OnFloatFormDeactivated(Form form) {
			RaiseFloatMDIChildDeactivated(form);
		}
		protected internal virtual void OnFloatFormLocationChanged(Form form) {
			FloatMDIChildDraggingEventArgs ea = RaiseFloatMDIChildDragging(form, Cursor.Position);
			bool dockingStarted = false;
			foreach(XtraTabbedMdiManager manager in ea.Targets) {
				if(manager.CanDock(form, ea.ScreenPoint)) {
					if(manager.PerformDocking(form, this, floatForm == null)) {
						manager.UpdateDropHint(ea.ScreenPoint);
						dockingStarted = true;
						break;
					}
				}
			}
			if(!dockingStarted) {
				foreach(XtraTabbedMdiManager manager in ea.Targets)
					manager.DestroyDropHint();
			}
		}
		internal bool IsDragFullWindows {
			get { return SystemInformation.DragFullWindows; }
		}
		bool IsDelayedDropAllowed(bool overDropHint) {
			if(overDropHint)
				return Control.MouseButtons == MouseButtons.None;
			return !IsDragFullWindows || (Control.MouseButtons == MouseButtons.Left);
		}
		void OnDropTimerTick(object sender, EventArgs e) {
			DropTimer.Stop();
			if(delayedDrop != null) {
				if(IsDelayedDropAllowed(IsFloatingDropHintVisible)) {
					delayedDrop();
					DestroyDropHint();
				}
				else {
					if(floatForm == null) {
						DropTimer.Interval = defaultDockDelay;
						DropTimer.Start();
						return;
					}
				}
				delayedDrop = null;
			}
		}
		bool CanDock(Form form, Point screenPoint) {
			if(MdiParent == null || !MdiParent.Visible) return false;
			Point point = PointToClient(screenPoint);
			bool inEmpyMDIManager = (Pages.Count == 0) && ViewInfo.Bounds.Contains(point);
			bool inHeader = ViewInfo.HeaderInfo.Bounds.Contains(point);
			if(delayedDrop != null) {
				DropTimer.Stop();
				delayedDrop = null;
			}
			return inHeader || inEmpyMDIManager;
		}
		delegate void DelayedDrop();
		[ThreadStatic]
		static DelayedDrop delayedDrop;
		[ThreadStatic]
		static bool delayedDropDockOnly;
		bool PerformDocking(Form form, XtraTabbedMdiManager source, bool formMoving) {
			if(RaiseBeginDocking(form)) {
				delayedDrop = delegate() {
					if(IsDocking) return;
					docking++;
					DockCore(form, source);
					if(!formMoving && !delayedDropDockOnly) MdiParent.BeginInvoke(
							 new Action<object>(delegate(object sender) {
							floatForm = form;
							DoDragDrop();
						}), form
						 );
					RaiseEndDocking(form);
					delayedDropDockOnly = false;
					docking--;
				};
				DropTimer.Interval = FloatMDIChildDockDelay;
				DropTimer.Start();
				ShowDropHint();
				return true;
			}
			return false;
		}
		void DockCore(Form form, XtraTabbedMdiManager source) {
			using(var context = new XtraBars.Docking2010.ChildDocumentManagerBeginInvokeContext(form)) {
				form.Visible = false;
				source.UnCaptureFormMoving();
				source.FloatForms.Remove(form);
				context.RequestPatchActiveChildren();
				form.MdiParent = MdiParent;
				PageAssociation.Remove(form);
				source.DoFloatingEndDragging(false);
				source.RaiseFloatMDIChildDeactivated(form);
				source.floatFormDragStarted = false;
				form.Visible = true;
			}
		}
		int floatFormCancelPageIndex = -1;
		internal int floatFormDockingPageIndex = -1;
		public bool Float(XtraMdiTabPage page, Point point) {
			if(page == null) return false;
			if(Pages.IndexOf(page) == -1) return false;
			Form form = page.MdiChild;
			using(FloatForms.Lock()) {
				if(RaiseBeginFloating(form)) {
					floatFormCancelPageIndex = Pages.IndexOf(page);
					form.Visible = false;
					form.MdiParent = null;
					form.Owner = MdiParent;
					form.Location = point;
					PageAssociation.Add(form, page);
					FloatForms.Add(form);
					RaiseFloating(form);
					if(!(UseDragPreview() && lockStartDragging > 0))
						form.Visible = true;
					return true;
				}
			}
			return false;
		}
		public bool Dock(Form floatForm, XtraTabbedMdiManager source) {
			if(floatForm == null || source == null) return false;
			if(floatForm.MdiParent == MdiParent) return false;
			using(FloatForms.Lock()) {
				if(RaiseBeginDocking(floatForm)) {
					DockCore(floatForm, source);
					RaiseEndDocking(floatForm);
					return true;
				}
			}
			return false;
		}
		void RaiseFloatMDIChildActivated(Form form) {
			if(FloatMDIChildActivated != null)
				FloatMDIChildActivated(form, EventArgs.Empty);
		}
		void RaiseFloatMDIChildDeactivated(Form form) {
			if(FloatMDIChildDeactivated != null)
				FloatMDIChildDeactivated(form, EventArgs.Empty);
		}
		bool RaiseBeginFloating(Form form) {
			FloatingCancelEventArgs ea = new FloatingCancelEventArgs(form, MdiParent, Pages.Count <= 1);
			if(BeginFloating != null)
				BeginFloating(this, ea);
			return !ea.Cancel;
		}
		FloatMDIChildDraggingEventArgs RaiseFloatMDIChildDragging(Form form, Point point) {
			FloatMDIChildDraggingEventArgs ea = new FloatMDIChildDraggingEventArgs(form, MdiParent, point);
			ea.Targets.Add(this);
			if(FloatMDIChildDragging != null)
				FloatMDIChildDragging(this, ea);
			return ea;
		}
		bool RaiseBeginDocking(Form form) {
			FloatingCancelEventArgs ea = new FloatingCancelEventArgs(form, MdiParent);
			if(BeginDocking != null)
				BeginDocking(this, ea);
			return !ea.Cancel;
		}
		void RaiseFloating(Form form) {
			if(Floating != null)
				Floating(this, new FloatingEventArgs(form, MdiParent));
		}
		void RaiseEndFloating(Form form, bool isDocking) {
			if(EndFloating != null)
				EndFloating(this, new EndFloatingEventArgs(form, MdiParent, isDocking));
		}
		void RaiseEndDocking(Form form) {
			if(EndDocking != null)
				EndDocking(this, new FloatingEventArgs(form, MdiParent));
		}
		void IBarAndDockingControllerClient.OnDisposed(BarAndDockingController controller) {
			if(Disposing) return;
			Controller = null;
		}
		void IBarAndDockingControllerClient.OnControllerChanged(BarAndDockingController controller) {
			OnController_Changed(controller);
		}
		protected virtual void OnController_Changed(object sender) {
			CheckInfo();
			LayoutChanged();
		}
		public static XtraTabbedMdiManager GetXtraTabbedMdiManager(Form mdiParent) {
			if(mdiParent == null) return null;
			MdiClient client = MdiClientSubclasser.GetMdiClient(mdiParent);
			if(client == null)
				return null;
			XtraMdiClientSubclasser subclasser = MdiClientSubclasser.FromMdiClient(client) as XtraMdiClientSubclasser;
			return (subclasser != null) ? subclasser.Manager : null;
		}
		Point IXtraTab.ScreenPointToControl(Point point) {
			return PointToClient(point);
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("XtraTabbedMdiManagerAllowDragDrop"),
#endif
 Category(XtraTabbedMdiCategoryName.Behavior), DefaultValue(DefaultBoolean.Default)]
		public virtual DefaultBoolean AllowDragDrop {
			get { return this.allowDragDrop; }
			set {
				if(AllowDragDrop == value)
					return;
				this.allowDragDrop = value;
				ReInit();
			}
		}
		protected virtual bool IsManagerHandlesDragDrop {
			get { return AllowDragDrop != DefaultBoolean.False; }
		}
		void OnMdiClient_GiveFeedback(object sender, GiveFeedbackEventArgs e) {
			if(!IsManagerHandlesDragDrop)
				return;
			if(e.UseDefaultCursors && e.Effect == DragDropEffects.Scroll) {
				e.UseDefaultCursors = false;
				Cursor.Current = Cursors.Default;
			}
		}
		IXtraTabPage ExtractPage(DragEventArgs e) {
			if(e.Data == null)
				return null;
			IXtraTabPage page = e.Data.GetData(typeof(IXtraTabPage)) as IXtraTabPage;
			if(page == null)
				return null;
			if(!ReferenceEquals(this, page.TabControl))
				return null;
			return page;
		}
		static int GetRangeFromPointToTab(Point point, BaseTabPageViewInfo pageInfo) {
			Rectangle workBounds = pageInfo.Bounds;
			workBounds.Offset(-point.X, -point.Y);
			if(workBounds.Top < 0)
				workBounds = new Rectangle(new Point(workBounds.Left, workBounds.Height - workBounds.Top), workBounds.Size);
			if(workBounds.Right < 0)
				workBounds = new Rectangle(new Point(-workBounds.Right, workBounds.Top), workBounds.Size);
			if(workBounds.Left <= 0 && workBounds.Bottom <= 0) {
				return 0;
			}
			else if(workBounds.Left <= 0) {
				return workBounds.Bottom;
			}
			else if(workBounds.Bottom <= 0) {
				return workBounds.Left;
			}
			else {
				return workBounds.Left + workBounds.Right;	
			}
		}
		int FindNearestTab(Point point, int defaultValue) {
			BaseTabPageViewInfo bestFoundPage = null;
			int bestFoundRange = int.MaxValue;
			foreach(BaseTabPageViewInfo pageViewInfo in ViewInfo.HeaderInfo.VisiblePages) {
				int range = GetRangeFromPointToTab(point, pageViewInfo);
				if(range <= bestFoundRange) {
					bestFoundRange = range;
					bestFoundPage = pageViewInfo;
				}
			}
			if(bestFoundPage == null)
				return defaultValue;
			int result = ViewInfo.HeaderInfo.AllPages.IndexOf(bestFoundPage);
			if(result >= 0)
				return result;
			return defaultValue;
		}
		int ExtractPosition(DragEventArgs e, int defaultValue) {
			Point localPoint = PointToClient(new Point(e.X, e.Y));
			BaseTabHitInfo hitInfo = CalcHitInfo(localPoint);
			switch(hitInfo.HitTest) {
				default:
					return defaultValue;
				case XtraTabHitTest.None:
					return FindNearestTab(localPoint, defaultValue);
				case XtraTabHitTest.PageHeader:
					return this.Pages.IndexOf(hitInfo.Page);
			}
		}
		void OnMdiClient_DragDrop(object sender, DragEventArgs e) {
			if(!IsManagerHandlesDragDrop)
				return;
			DragDropPaintCancelOverride();
			if(e.Effect != DragDropEffects.Scroll)
				return;
			IXtraTabPage page = ExtractPage(e);
			if(page == null)
				return;
			int oldPosition = Pages.IndexOf(page);
			int newPosition = ExtractPosition(e, oldPosition);
			if(oldPosition != newPosition) {
				this.Pages.Remove(page);
				this.Pages.Insert(newPosition, page);
				this.LayoutChanged();
			}
		}
		void OnMdiClient_DragEnter(object sender, DragEventArgs e) {
		}
		bool floatFormDragStarted = false;
		Form floatForm = null;
		void OnMdiClient_DragLeave(object sender, EventArgs e) {
			if(!IsManagerHandlesDragDrop)
				return;
			DragDropPaintCancelOverride();
			if(IsInDoDragDrop && dragLMousePressed && CanFloatOnDrag() && SelectedPage != null) {
				floatForm = SelectedPage.MdiChild;
				DoFloatingStartDragging();
			}
		}
		void OnMdiClient_DragOver(object sender, DragEventArgs e) {
			if(!IsManagerHandlesDragDrop)
				return;
			if((e.AllowedEffect & DragDropEffects.Scroll) != DragDropEffects.Scroll)
				return;
			IXtraTabPage page = ExtractPage(e);
			if(page == null) return;
			int oldPosition = Pages.IndexOf(page);
			int newPosition = ExtractPosition(e, -1);
			if(newPosition < 0) {
				e.Effect = DragDropEffects.None;
				DragDropPaintCancelOverride();
			}
			else {
				e.Effect = DragDropEffects.Scroll;
				DragDropPaintOverride(oldPosition, newPosition);
			}
		}
		bool dragLMousePressed;
		void OnMdiClient_QueryContinueDrag(object sender, QueryContinueDragEventArgs e) {
			if(!IsManagerHandlesDragDrop)
				return;
			dragLMousePressed = (e.KeyState == 1);
			Point pt = PointToClient(Cursor.Position);
			if(CanFloatOnDrag() && ViewInfo.PageClientBounds.Contains(pt)) {
				e.Action = DragAction.Cancel;
			}
			if(floatFormDragStarted) {
				e.Action = DragAction.Cancel;
			}
		}
		int doDragDropCounter;
		protected bool IsInDoDragDrop {
			get { return doDragDropCounter > 0; }
		}
		protected virtual void DoDragDrop() {
			if(!InTheAir)
				return;
			XtraMdiTabPage page = SelectedPage;
			if(page == null)
				return;
			IDataObject data = new XtraTabbedMdiDataObject(page);
			doDragDropCounter++;
			MdiClientEncapsulator.ClientWindow.DoDragDrop(data, DragDropEffects.Scroll);
			doDragDropCounter--;
			if(floatFormDragStarted) {
				floatFormDragStarted = false;
				CaptureFormMoving();
			}
		}
		Form capturedForm = null;
		void CaptureFormMoving() {
			MdiClientEncapsulator.ClientWindow.Capture = true;
			if(capturedForm != floatForm) {
				capturedForm = floatForm;
				MdiClientEncapsulator.ClientWindow.MouseMove += OnFloatFormMove;
				MdiClientEncapsulator.ClientWindow.MouseUp += OnFloatFormDrop;
			}
		}
		void UnCaptureFormMoving() {
			if(MdiClientEncapsulator != null) {
				MdiClientEncapsulator.ClientWindow.MouseMove -= OnFloatFormMove;
				MdiClientEncapsulator.ClientWindow.MouseUp -= OnFloatFormDrop;
				MdiClientEncapsulator.ClientWindow.Capture = false;
			}
			capturedForm = null;
		}
		void OnFloatFormDrop(object sender, MouseEventArgs e) {
			if(IsDocking) return;
			docking++;
			using(FloatForms.Lock()) {
				UnCaptureFormMoving();
				if(IsFloatingDropHintVisible) {
					if(RaiseBeginDocking(floatForm)) {
						delayedDrop = null;
						DockCore(floatForm, this);
						RaiseEndDocking(floatForm);
					}
				}
				else {
					Form childForm = floatForm;
					bool isDocking = IsPreviewDragging && delayedDrop != null;
					if(IsDocking)
						delayedDropDockOnly = true;
					DoFloatingEndDragging(true);
					RaiseEndFloating(childForm, isDocking);
				}
			}
			docking--;
		}
		void OnFloatFormMove(object sender, MouseEventArgs e) {
			if(floatForm.MdiParent != null) {
				CancelFloating(false);
				return;
			}
			DoFloatingDragging(new Point(e.X - dragStartOffset.X, e.Y - dragStartOffset.Y));
		}
		protected bool IsCancelFloating {
			get { return cancelFloating > 0; }
		}
		protected bool IsDocking {
			get { return docking > 0; }
		}
		int docking = 0;
		int cancelFloating = 0;
		protected internal void CancelFloating(bool restoreDock) {
			if(IsCancelFloating) return;
			cancelFloating++;
			if(IsDragFullWindows)
				DropTimer.Stop();
			UnCaptureFormMoving();
			if(restoreDock)
				DockCore(floatForm, this);
			else {
				DestroyDropHint();
			}
			cancelFloating--;
		}
		DropHint dropHint;
		bool IsFloatingDropHintVisible {
			get { return dropHint != null; }
		}
		void ShowDropHint() {
			if(ShowFloatingDropHint == DefaultBoolean.False) return;
			if(dropHint == null) {
				dropHint = CreateDropHint();
				dropHint.Location = PointToScreen(Point.Empty);
				dropHint.Size = ViewInfo.Bounds.Size;
				dropHint.Show();
			}
		}
		void UpdateDropHint(Point screenPoint) {
			if(dropHint != null)
				dropHint.Docking(PointToClient(screenPoint));
		}
		void DestroyDropHint() {
			if(dropHint != null) {
				dropHint.Hide();
				dropHint.Dispose();
				dropHint = null;
			}
		}
		protected virtual DropHint CreateDropHint() {
			return new DropHint(this);
		}
		DragFrame dragFrame;
		bool UseDragPreview() {
			return FloatPageDragMode != FloatPageDragMode.FullWindow;
		}
		protected internal bool IsPreviewDragging {
			get { return dragFrame != null; }
		}
		int lockStartDragging = 0;
		void DoFloatingStartDragging() {
			if(lockStartDragging > 0) return;
			lockStartDragging++;
			Point cursorPosition = Cursor.Position;
			floatFormDragStarted = Float(SelectedPage, cursorPosition);
			if(floatFormDragStarted) {
				if(UseDragPreview()) {
					dragFrame = CreateDragFrame();
					dragFrame.Location = PointToScreen(cursorPosition);
					dragFrame.Size = GetPreviewSize(ViewInfo.Client.Size);
					dragFrame.Target = floatForm;
					floatForm.Hide();
					ReparentWindow(dragFrame);
					dragFrame.Show();
					DoFocusWindow(dragFrame);
					dragFrame.Focus();
				}
			}
			else floatForm = null;
			lockStartDragging--;
		}
		internal void ReparentWindow(Control dragFrame) {
			if(dragFrame.Handle == IntPtr.Zero) return;
			WinPopupController popupController = new WinPopupController();
			popupController.ReparentWindow(dragFrame, MdiParent.Handle);
		}
		internal void DoFocusWindow(Control dragFrame) {
			if(MdiParent == null) return;
			XtraForm owner = MdiParent as XtraForm;
			if(owner != null) owner.SuspendRedraw();
			try {
				dragFrame.Focus();
				EmulateFormFocus(MdiParent);
			}
			finally {
				if(owner != null) owner.ResumeRedraw();
			}
		}
		static void EmulateFormFocus(Form form) {
			EmulateFormFocus(form.Handle, !IsRibbonFormGlass(form));
		}
		static void EmulateFormFocus(IntPtr formHandle, bool invalidateTitleBar) {
			Utils.Drawing.Helpers.NativeMethods.SendMessage(formHandle, 0x86, invalidateTitleBar ? 1 : 0, (IntPtr)(-1)); 
			Utils.Drawing.Helpers.NativeMethods.RedrawWindow(formHandle, IntPtr.Zero, IntPtr.Zero, 0x401);
		}
		static bool IsRibbonFormGlass(Form form) {
			XtraBars.Ribbon.RibbonForm ribbonForm = form as XtraBars.Ribbon.RibbonForm;
			return ribbonForm != null && ribbonForm.IsShouldUseGlassForm();
		}
		int floatingDragging = 0;
		protected internal bool IsFloatingDragging {
			get { return floatingDragging > 0; }
		}
		void DoFloatingDragging(Point location) {
			if(IsFloatingDragging) return;
			floatingDragging++;
			Point screenPoint = PointToScreen(location);
			if(IsPreviewDragging) {
				dragFrame.Location = screenPoint;
				OnFloatFormLocationChanged(floatForm);
			}
			else floatForm.Location = screenPoint;
			floatingDragging--;
		}
		void DoFloatingEndDragging(bool syncLocation) {
			if(IsPreviewDragging) {
				using(FloatForms.Lock()) {
					dragFrame.Hide();
					if(syncLocation)
						floatForm.Location = dragFrame.Location;
					if(delayedDrop == null) {
						MdiParent.Focus();
						floatForm.Show();
					}
					DestroyDragFrame();
				}
			}
			DestroyDropHint();
			floatForm = null;
		}
		protected virtual DragFrame CreateDragFrame() {
			return new DragFrame(this);
		}
		protected void DestroyDragFrame() {
			if(dragFrame != null) {
				dragFrame.Dispose();
				dragFrame = null;
			}
		}
		Size GetPreviewSize(Size mdiParentSize) {
			return new Size(
					Round(mdiParentSize.Width * PreviewPageZoomRatio),
					Round(mdiParentSize.Height * PreviewPageZoomRatio)
				);
		}
		protected virtual DocumentSelector CreateDocumentSelector(XtraMdiTabPage startPage, XtraMdiTabPage nextPage) {
			return new DocumentSelector(this, startPage, nextPage);
		}
		protected bool ShowDocumentSelector(XtraMdiTabPage startPage, ref XtraMdiTabPage nextPage) {
			using(DocumentSelector documentSelector = CreateDocumentSelector(startPage, nextPage)) {
				if(documentSelector.ShowDialog() == DialogResult.OK) {
					nextPage = documentSelector.NextPage;
					return true;
				}
			}
			return false;
		}
		public event CustomDocumentSelectorItemEventHandler CustomDocumentSelectorItem;
		protected internal void RaiseCustomDocumentSelectorItem(DocumentSelectorItem item) {
			if(CustomDocumentSelectorItem != null)
				CustomDocumentSelectorItem(this, new CustomDocumentSelectorItemEventArgs(item));
		}
		public event CustomDocumentSelectorSettingsEventHandler CustomDocumentSelectorSettings;
		protected internal void RaiseDocumentSelectorSettings(DocumentSelector selector) {
			if(CustomDocumentSelectorSettings != null)
				CustomDocumentSelectorSettings(this, new CustomDocumentSelectorSettingsEventArgs(selector));
		}
		static int Round(float value) {
			return value > 0 ? (int)(value + 0.5f) : (int)(value - 0.5f);
		}
		protected virtual void DragDropPaintCancelOverride() {
			IDisposable disposableDragDropPainterOverride = dragDropPainterOverride as IDisposable;
			if(disposableDragDropPainterOverride != null)
				disposableDragDropPainterOverride.Dispose();
			dragDropPainterOverride = null;
			Invalidate();
		}
		protected virtual void DragDropPaintOverride(int oldPosition, int newPosition) {
			if(dragDropPainterOverride == null) {
				dragDropPainterOverride = new DragDropDrawTabControl(this, oldPosition, newPosition);
				Invalidate();
			}
			else if(dragDropPainterOverride.UpdatePositions(oldPosition, newPosition)) {
				Invalidate();
			}
		}
		void RemoveMdiClientFromToolTipController(ToolTipController controller) {
			MdiClient client = MdiClientEncapsulator.ClientWindow;
			if(client != null)
				controller.RemoveClientControl(client);
		}
		void AddMdiClientToToolTipController(ToolTipController controller) {
			MdiClient client = MdiClientEncapsulator.ClientWindow;
			if(client != null)
				controller.AddClientControl(client, this);
		}
		ToolTipController toolTipController;
		[Category("Appearance"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("XtraTabbedMdiManagerToolTipController"),
#endif
 DefaultValue((string)null)]
		public virtual ToolTipController ToolTipController {
			get { return toolTipController != null ? toolTipController : ToolTipController.DefaultController; }
			set {
				if(ToolTipController == value) return;
				if(ToolTipController != null)
					RemoveMdiClientFromToolTipController(ToolTipController);
				toolTipController = value;
				if(ToolTipController != null) {
					RemoveMdiClientFromToolTipController(ToolTipController.DefaultController);
					AddMdiClientToToolTipController(ToolTipController);
				}
				else AddMdiClientToToolTipController(ToolTipController.DefaultController);
			}
		}
		#region IToolTipControlClient Members
		ToolTipControlInfo GetToolTipControlInfo(IXtraTabPage page, Point point) {
			ToolTipControlInfo info = new ToolTipControlInfo();
			if(page == null) return info;
			XtraMdiTabPage mdiPage = page as XtraMdiTabPage;
			if(mdiPage != null) {
				info.SuperTip = mdiPage.SuperTip;
				info.ToolTipType = ToolTipType.SuperTip;
			}
			info.Title = page.TooltipTitle;
			info.IconType = page.TooltipIconType;
			info.Text = page.Tooltip;
			info.Object = page;
			info.ToolTipPosition = MdiClientEncapsulator.ClientWindow.PointToScreen(point);
			return info;
		}
		ToolTipControlInfo IToolTipControlClient.GetObjectInfo(Point point) {
			Point clientLocation = ViewInfo.PageClientBounds.Location;
			Point pt = new Point(point.X + clientLocation.X, point.Y + clientLocation.Y);
			if(ViewInfo == null) return new ToolTipControlInfo();
			if(!ViewInfo.HeaderInfo.Bounds.Contains(pt)) return new ToolTipControlInfo();
			BaseTabPageViewInfo pageInfo = ViewInfo.HeaderInfo.FindPage(pt);
			if(pageInfo == null) return new ToolTipControlInfo();
			return GetToolTipControlInfo(pageInfo.Page, point);
		}
		bool IToolTipControlClient.ShowToolTips {
			get { return ShowToolTips != DefaultBoolean.False && !DesignMode; }
		}
		#endregion
		#region IXtraTabPropertiesEx Members
		DevExpress.XtraTab.Buttons.CustomHeaderButtonCollection tabButtonsCore = new DevExpress.XtraTab.Buttons.CustomHeaderButtonCollection();
		DevExpress.XtraTab.Buttons.CustomHeaderButtonCollection IXtraTabPropertiesEx.CustomHeaderButtons {
			get { return tabButtonsCore; }
		}
		DevExpress.XtraTab.TabMiddleClickFiringMode IXtraTabPropertiesEx.TabMiddleClickFiringMode {
			get { return (TabMiddleClickFiringMode)((int)closeTabOnMiddleClickCore); }
		}
		CloseTabOnMiddleClick closeTabOnMiddleClickCore;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("XtraTabbedMdiManagerCloseTabOnMiddleClick"),
#endif
 Category(XtraTabbedMdiCategoryName.Behavior),
	 DefaultValue(CloseTabOnMiddleClick.Default)]
		public CloseTabOnMiddleClick CloseTabOnMiddleClick {
			get { return closeTabOnMiddleClickCore; }
			set {
				if(CloseTabOnMiddleClick == value) return;
				closeTabOnMiddleClickCore = value;
				LayoutChanged();
			}
		}
		#endregion
		DefaultBoolean rightToLeftLayoutCore = DefaultBoolean.Default;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("XtraTabbedMdiManagerRightToLeftLayout"),
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
				if(!InTheAir) return XtraEditors.WindowsFormsSettings.GetIsRightToLeft(MdiParent);
			}
			return RightToLeftLayout == DefaultBoolean.True;
		}
		#region MVVM
		DevExpress.Utils.MVVM.Services.IDocumentAdapter DevExpress.Utils.MVVM.Services.IDocumentAdapterFactory.Create() {
			return new XtraBars.MVVM.Services.XtraMdiTabPageAdapter(this);
		}
		#endregion MVVM
	}
	public class XtraMdiClientSubclasser : MdiClientSubclasser {
		public XtraMdiClientSubclasser(MdiClient subclassedMdiClient, XtraTabbedMdiManager owner)
			: base(subclassedMdiClient, owner) {
		}
		public XtraTabbedMdiManager Manager {
			get { return Owner as XtraTabbedMdiManager; }
		}
	}
}
namespace DevExpress.XtraBars.MVVM.Services {
	using System.Windows.Forms;
	using DevExpress.Utils.MVVM.Services;
	using DevExpress.XtraTabbedMdi;
	class XtraMdiTabPageAdapter : IDocumentAdapter {
		XtraMdiTabPage mdiTabPage;
		XtraTabbedMdiManager manager;
		public XtraMdiTabPageAdapter(XtraTabbedMdiManager manager) {
			this.manager = manager;
			manager.PageRemoved += manager_PageRemoved;
			manager.PageClosing += manager_PageClosing;
		}
		public void Dispose() {
			var control = GetControl(mdiTabPage);
			if(control != null)
				control.TextChanged -= control_TextChanged;
			manager.PageClosing -= manager_PageClosing;
			manager.PageRemoved -= manager_PageRemoved;
			mdiTabPage = null;
		}
		void manager_PageClosing(object sender, MdiTabPageCancelEventArgs e) {
			if(e.Page == mdiTabPage)
				RaiseClosing(e);
		}
		void manager_PageRemoved(object sender, MdiTabPageEventArgs e) {
			if(e.Page == mdiTabPage) {
				RaiseClosed(e);
				Dispose();
			}
		}
		void control_TextChanged(object sender, EventArgs e) {
			mdiTabPage.Text = ((Control)sender).Text;
		}
		void RaiseClosed(MdiTabPageEventArgs e) {
			if(Closed != null) Closed(manager, e);
		}
		void RaiseClosing(MdiTabPageCancelEventArgs e) {
			var ea = new CancelEventArgs(e.Cancel);
			if(Closing != null) Closing(manager, ea);
			e.Cancel = ea.Cancel;
		}
		public void Show(Control control) {
			var page = System.Linq.Enumerable.FirstOrDefault(manager.Pages, p => GetControl(p) == control);
			if(page == null) {
				XtraForm mdiChild = new XtraForm();
				mdiChild.Text = control.Text;
				mdiChild.MdiParent = manager.MdiParent;
				mdiChild.Show();
				mdiTabPage = manager.Pages[mdiChild];
				mdiTabPage.Text = control.Text;
				control.TextChanged += control_TextChanged;
			}
			if(mdiTabPage != null) {
				if(!mdiTabPage.MdiChild.Controls.Contains(control)) {
					control.Dock = DockStyle.Fill;
					mdiTabPage.MdiChild.Controls.Add(control);
				}
			}
			manager.SelectedPage = mdiTabPage;
		}
		public void Close(Control control, bool force = true) {
			if(force)
				manager.PageClosing -= manager_PageClosing;
			if(control != null)
				control.TextChanged -= control_TextChanged;
			((IDisposable)mdiTabPage).Dispose();
		}
		public event EventHandler Closed;
		public event CancelEventHandler Closing;
		static Control GetControl(XtraMdiTabPage page) {
			return page != null && page.MdiChild.Controls.Count > 0 ? page.MdiChild.Controls[0] : null;
		}
	}
}
