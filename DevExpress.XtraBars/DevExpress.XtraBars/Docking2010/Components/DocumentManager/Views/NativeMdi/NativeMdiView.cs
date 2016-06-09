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
namespace DevExpress.XtraBars.Docking2010.Views.NativeMdi {
	public class NativeMdiView : BaseView {
		public NativeMdiView() { }
		public NativeMdiView(IContainer container)
			: base(container) {
		}
		protected override void OnCreate() {
			allowDocumentCaptionColorBlendingCore = true;
			locations = new Dictionary<IntPtr, Point>();
			base.OnCreate();
		}
		public sealed override ViewType Type {
			get { return ViewType.NativeMdi; }
		}
		protected sealed internal override Type GetUIElementKey() {
			return typeof(NativeMdiView);
		}
		protected sealed internal override bool AllowMdiLayout {
			get { return true; }
		}
		protected sealed internal override bool AllowMdiSystemMenu {
			get { return true; }
		}
		protected internal override bool CanProcessSysCommand(IntPtr hWnd, int cmd) {
			return (cmd != DevExpress.Utils.Drawing.Helpers.NativeMethods.SC.SC_CLOSE) && base.CanProcessSysCommand(hWnd, cmd);
		}
		protected internal override bool OnSCClose(IntPtr hWnd) {
			locations.Remove(hWnd);
			return base.OnSCClose(hWnd);
		}
		protected internal override bool OnSCMaximize(IntPtr hWnd) {
			if(Documents.GetDocument(hWnd) != null) {
				maximizedCore = true;
				OnMdiMaximize();
				return true;
			}
			return false;
		}
		protected internal override bool OnSCRestore(IntPtr hWnd) {
			if(Documents.GetDocument(hWnd) != null) {
				maximizedCore = false;
				OnMdiRestore();
				return true;
			}
			return false;
		}
		bool maximizedCore;
		[Browsable(false), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("NativeMdiViewMaximized")
#else
	Description("")
#endif
]
		public bool Maximized {
			get { return maximizedCore; }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("NativeMdiViewDocumentProperties")]
#endif
		[Category("Layout"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public new IDocumentProperties DocumentProperties {
			get { return base.DocumentProperties as IDocumentProperties; }
		}
		#region Appearance
		protected override BaseViewAppearanceCollection CreateAppearanceCollection() {
			return new NativeMdiViewAppearanceCollection(this);
		}
		bool allowDocumentCaptionColorBlendingCore;
#if !SL
	[DevExpressXtraBarsLocalizedDescription("NativeMdiViewAllowDocumentCaptionColorBlending")]
#endif
		[Category("Appearance"), DefaultValue(true)]
		[XtraSerializableProperty]
		public bool AllowDocumentCaptionColorBlending {
			get { return allowDocumentCaptionColorBlendingCore; }
			set { SetValue(ref allowDocumentCaptionColorBlendingCore, value); }
		}
		protected override bool CanBlendCaptionColor() {
			return AllowDocumentCaptionColorBlending;
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("NativeMdiViewAppearanceDocumentCaption"),
#endif
 Category("Appearance"),
	   DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DevExpress.Utils.AppearanceObject AppearanceDocumentCaption {
			get { return ((NativeMdiViewAppearanceCollection)AppearanceCollection).DocumentCaption; }
		}
		bool ShouldSerializeAppearanceDocumentCaption() {
			return !IsDisposing && AppearanceCollection != null && AppearanceDocumentCaption.ShouldSerialize();
		}
		void ResetAppearanceDocumentCaption() {
			AppearanceDocumentCaption.Reset();
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("NativeMdiViewAppearanceActiveDocumentCaption"),
#endif
 Category("Appearance"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DevExpress.Utils.AppearanceObject AppearanceActiveDocumentCaption {
			get { return ((NativeMdiViewAppearanceCollection)AppearanceCollection).ActiveDocumentCaption; }
		}
		bool ShouldSerializeAppearanceActiveDocumentCaption() {
			return !IsDisposing && AppearanceCollection != null && AppearanceActiveDocumentCaption.ShouldSerialize();
		}
		void ResetAppearanceActiveDocumentCaption() {
			AppearanceActiveDocumentCaption.Reset();
		}
		#endregion
		protected virtual void OnMdiRestore() {
			if(Manager.ActivationInfo != null)
				Manager.ActivationInfo.OnRestore();
			Array.ForEach(GetChildren(), RestoreLocation);
		}
		protected virtual void OnMdiMaximize() {
			Array.ForEach(GetChildren(), StoreLocation);
			if(Manager.ActivationInfo != null)
				Manager.ActivationInfo.OnMaximize();
		}
		[Browsable(false)]
		public new INativeMdiViewController Controller {
			get { return base.Controller as INativeMdiViewController; }
		}
		protected override IBaseDocumentProperties CreateDocumentProperties() {
			return new DocumentProperties();
		}
		protected override IBaseViewController CreateController() {
			return new NativeMdiViewController(this);
		}
		protected override BaseDocumentCollection CreateDocumentCollection() {
			return new NativeMdiViewDocumentCollection(this);
		}
		protected internal override bool CanUseLoadingIndicator() {
			return UseLoadingIndicator == DevExpress.Utils.DefaultBoolean.True && (Site == null || !Site.DesignMode);
		}
		protected override void OnShown() {
			base.OnShown();
			if(Documents.Exists((document) => !document.IsControlLoaded))
				Manager.InvokePatchActiveChildren();
			if(IsCascadeRequested) 
				Controller.Cascade();
			cascadeRequested = 0;
		}
		protected override void UpdateStyleCore() {
			base.UpdateStyleCore();
			foreach(var item in Documents) 
				item.UpdateStyle();
		}
		protected internal override void PatchActiveChildren(Point offset) {
			var ownerControl = Manager.GetOwnerControl();
			for(int i = 0; i < Documents.Count; i++) {
				Document document = Documents[i] as Document;
				bool fUpdateNC = false;
				bool prev = document.IsControlLoadedByQueryControl;
				if(document.EnsureIsBoundToControl(this) && prev != document.IsControlLoadedByQueryControl)
					fUpdateNC = true;
				if(document.IsNonMdi && document.IsMaximized) {
					if(!ownerControl.ClientSize.IsEmpty) {
						fUpdateNC |= DevExpress.Utils.Mdi.MdiChildHelper.MaximizeControl(document.ContentContainerHandle,
							document.Bounds.Size, ownerControl.ClientSize);
					}
				}
				if(fUpdateNC)
					DevExpress.Utils.Mdi.MdiClientSubclasser.ProcessNC(document.ContentContainerHandle);
			}
		}
		protected internal override void PatchBeforeActivateChild(Control activatedChild, Point offset) { }
		protected Form[] GetChildren() {
			List<Form> children = new List<Form>();
			for(int i = 0; i < Documents.Count; i++)
				if(Documents[i].IsControlLoaded)
					children.Add(Documents[i].Form);
			return children.ToArray();
		}
		internal IDictionary<IntPtr, Point> locations;
		void StoreLocation(Form child) {
			if(!locations.ContainsKey(child.Handle)) {
				if(child.WindowState == FormWindowState.Normal)
					locations[child.Handle] = child.Location;
				else locations[child.Handle] = child.RestoreBounds.Location;
			}
		}
		void RestoreLocation(Form child) {
			Point location;
			if(locations.TryGetValue(child.Handle, out location)) {
				locations.Remove(child.Handle);
				if(child.WindowState == FormWindowState.Normal)
					child.Location = location;
			}
		}
		protected internal override void OnDepopulated() {
			base.OnDepopulated();
			maximizedCore = false;
			locations.Clear();
		}
		int cascadeRequested;
		protected bool IsCascadeRequested {
			get { return cascadeRequested > 0; }
		}
		protected internal override void OnPopulated() {
			base.OnPopulated();
			if(!Bounds.IsEmpty && !IsShownRequested)
				Controller.Cascade();
			else
				cascadeRequested++;
		}
		public bool HasIconicChildren() {
			return Array.Exists(GetChildren(), IsMinimized);
		}
		public bool AllChildrenAreIcons() {
			return Array.TrueForAll(GetChildren(), IsMinimized);
		}
		static bool IsMaximized(Form child) {
			return child.WindowState == FormWindowState.Maximized;
		}
		static bool IsMinimized(Form child) {
			return child.WindowState == FormWindowState.Minimized;
		}
		protected override void OnShowingDockGuidesCore(Customization.DockGuidesConfiguration configuration, BaseDocument document, BaseViewHitInfo hitInfo) {
			configuration.Disable(Customization.DockHint.Center);
			configuration.Disable(Customization.DockHint.CenterLeft);
			configuration.Disable(Customization.DockHint.CenterRight);
			configuration.Disable(Customization.DockHint.CenterTop);
			configuration.Disable(Customization.DockHint.CenterBottom);
		}
		#region XtraSerializable
		protected override BaseSerializableDocumentInfo CreateSerializableDocumentInfo(BaseDocument document) {
			return new SerializableDocumentInfo(document as Document);
		}
		protected class SerializableDocumentInfo : BaseSerializableDocumentInfo {
			public SerializableDocumentInfo(Document document)
				: base(document) {
				bool containsMaximizedForm = false;
				Control client = document.Manager.GetOwnerControl();
				foreach(Document item in document.Manager.View.Documents) {
					if(item.IsControlLoaded) {
						if(item.Form.WindowState == FormWindowState.Maximized)
							containsMaximizedForm = true;
					}
				}
				if(document.IsControlLoaded) {
					Location = document.Form.Location;
					Index = client.Controls.IndexOf(document.Form);
				}
				Bounds = document.Bounds;
				if(containsMaximizedForm) {
					IntPtr handle = document.Manager.GetChild(document).Handle;
					Location = ((NativeMdiView)document.Manager.View).locations[handle];
				}
			}
			[XtraSerializableProperty]
			public int Index { get; set; }
			[XtraSerializableProperty]
			public Rectangle Bounds { get; set; }
		}
		protected override void EndRestoreLayout() {
			RestoreMdiChildren();
			base.EndRestoreLayout();
		}
		void RestoreMdiChildren() {
			List<SerializableDocumentInfo> itemList = new List<SerializableDocumentInfo>();
			foreach(SerializableObjectInfo info in Items) {
				SerializableDocumentInfo dockInfo = info as SerializableDocumentInfo;
				if(dockInfo != null) {
					Point location;
					itemList.Add(dockInfo);
					Document document = dockInfo.Source as Document;
					if(document.IsControlLoaded) {
						document.RestoreBounds(new Rectangle(dockInfo.Location, dockInfo.Size));
						if(locations.TryGetValue(document.Form.Handle, out location)) {
							locations.Remove(document.Form.Handle);
							locations.Add(document.Form.Handle, document.Form.Location);
						}
					}
					document.Bounds = dockInfo.Bounds;
				}
				itemList.Sort(Compare);
				foreach(SerializableDocumentInfo documentInfo in itemList) {
					Document document = documentInfo.Source as Document;
					if(document.IsControlLoaded)
						document.Form.Activate();
				}
			}
		}
		static int Compare(SerializableDocumentInfo x, SerializableDocumentInfo y) {
			if(x == y) return 0;
			if(x == null || y == null)
				return 0;
			return y.Index.CompareTo(x.Index);
		}
		#endregion XtraSerializable
		protected internal override void RegisterListeners(DragEngine.BaseUIView uiView) {
			uiView.RegisterUIServiceListener(new Dragging.DocumentManagerUIViewRegularDragListener());
			uiView.RegisterUIServiceListener(new Dragging.DocumentManagerUIViewDockingListener());
			uiView.RegisterUIServiceListener(new Dragging.DocumentManagerUIViewFloatingDragListener());
			uiView.RegisterUIServiceListener(new Dragging.DocumentManagerUIViewUIInteractionListener());
		}
		protected override DevExpress.Utils.AppearanceObject GetActiveDocumentCaptionAppearance() {
			return AppearanceActiveDocumentCaption;
		}
		protected override DevExpress.Utils.AppearanceObject GetDocumentCaptionAppearance() {
			return AppearanceDocumentCaption;
		}
	}
	public class NativeMdiViewDocumentCollection : BaseDocumentCollection {
		WM_SYSCOMMAND_Interceptor interceptor;
		public NativeMdiViewDocumentCollection(NativeMdiView owner)
			: base(owner) {
			interceptor = new WM_SYSCOMMAND_Interceptor(owner);
		}
		protected override void OnDispose() {
			interceptor.ReleaseHandle();
			base.OnDispose();
		}
		protected override void OnElementRemoved(BaseDocument element) {
			Form form = element.Form ?? FindControl(element) as Form;
			if(form != null)
				interceptor.Release(form);
			base.OnElementRemoved(element);
		}
		protected override void SubscribeFormEvents(Form mdiChild) {
			if(mdiChild != null) {
				mdiChild.Activated += OnMDIChildActivated;
				mdiChild.Deactivate += OnMDIChildDeactivated;
				mdiChild.FormClosed += OnMdiChildClosed;
				mdiChild.Disposed += OnMdiChildDisposed;
			}
		}
		protected override void UnsubscribeFormEvents(Form mdiChild) {
			if(mdiChild != null) {
				mdiChild.Activated -= OnMDIChildActivated;
				mdiChild.Deactivate -= OnMDIChildDeactivated;
				mdiChild.FormClosed -= OnMdiChildClosed;
				mdiChild.Disposed -= OnMdiChildDisposed;
			}
		}
		protected override bool PostFilterMessageCore(int Msg, Control wnd, IntPtr HWnd, IntPtr WParam, IntPtr LParam) {
			if(interceptor.Handle != HWnd) CheckFocus(Msg, wnd, WParam);
			return (interceptor.Handle != HWnd) && CheckClosing(Msg, HWnd, WParam);
		}
		void OnMDIChildActivated(object sender, EventArgs e) {
			Form mdiChild = sender as Form;
			if(mdiChild != null && mdiChild.IsHandleCreated)
				interceptor.AssignHandle(mdiChild.Handle);
		}
		void OnMDIChildDeactivated(object sender, EventArgs e) {
			interceptor.ReleaseHandle();
		}
		void OnMdiChildClosed(object sender, FormClosedEventArgs e) {
			Form mdiChild = sender as Form;
			interceptor.Release(mdiChild);
			DisposeDocument(mdiChild, true);
		}
		void OnMdiChildDisposed(object sender, EventArgs e) {
			DisposeDocument(sender as Form, false);
		}
	}
}
