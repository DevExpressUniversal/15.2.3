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
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.Gesture;
using DevExpress.Utils.Mdi;
namespace DevExpress.XtraBars.Docking2010 {
	[ToolboxItem(false), DesignTimeVisible(false)]
	public class DocumentsHost : Control, IDocumentsHost, IFlickGestureClient, IFlickGestureProvider, IGestureClient, IGestureProvider {
		IDocumentsHostOwner ownerCore;
		public DocumentsHost(IDocumentsHostOwner owner) {
			ownerCore = owner;
			base.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
			base.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			base.SetStyle(ControlStyles.ResizeRedraw, true);
			base.SetStyle(ControlStyles.UserMouse, true);
			base.SetStyle(ControlStyles.Selectable, false);
			Dock = DockStyle.Fill;
		}
		protected override CreateParams CreateParams {
			get {
				CreateParams createParams = base.CreateParams;
				createParams.ExStyle |= 0x00010000;
				return createParams;
			}
		}
		bool isDisposingCore;
		public bool IsDisposing {
			get { return isDisposingCore; }
		}
		protected sealed override void Dispose(bool disposing) {
			if(disposing) {
				if(!isDisposingCore) {
					isDisposingCore = true;
					OnDispose();
					ownerCore = null;
				}
			}
			base.Dispose(disposing);
		}
		protected virtual void OnDispose() {
			activeContainerCore = null;
			activeDocumentCore = null;
		}
		public IDocumentsHostOwner Owner {
			get { return ownerCore; }
		}
		DocumentContainer activeContainerCore;
		public DocumentContainer ActiveContainer {
			get { return activeContainerCore; }
		}
		public Views.BaseDocument ActiveDocument {
			get { return activeDocumentCore; }
		}
		public DocumentContainer[] Containers {
			get { return Controls.ToArray(); }
		}
		object documentContainerActivate = new object();
		public event EventHandler DocumentContainerActivate {
			add { Events.AddHandler(documentContainerActivate, value); }
			remove { Events.RemoveHandler(documentContainerActivate, value); }
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			if(Owner != null)
				Owner.HandleCreated();
		}
		protected override void OnHandleDestroyed(EventArgs e) {
			base.OnHandleDestroyed(e);
			if(Owner != null)
				Owner.HandleDestroyed();
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
			UpdateActiveDocument();
		}
		void UpdateActiveDocument() {
			ContainerControl parentContainer = Parent as ContainerControl;
			if(parentContainer != null && parentContainer.ActiveControl != null) {
				var activeContainer = DocumentContainer.FromControl(parentContainer.ActiveControl);
				if(activeContainer != null && activeContainer.Parent == this) {
					activeContainerCore = activeContainer;
					var document = activeContainer.Document;
					activeDocumentCore = document;
					if(document != null && document.Manager != null) {
						var view = document.Manager.View;
						if(view != null) {
							using(view.LockPainting()) {
								view.SetActiveDocumentCore(document);
								view.FocusEnter();
							}
						}
					}
				}
			}
		}
		bool visible;
		protected override void OnVisibleChanged(EventArgs e) {
			base.OnVisibleChanged(e);
			if(Owner != null && visible != Visible)
				Owner.VisibleChanged(Visible);
			visible = Visible;
		}
		protected override void OnParentBackColorChanged(EventArgs e) {
			base.OnParentBackColorChanged(e);
			Owner.ParentBackColorChanged();
		}
		#region DocumentContainerCollection
		protected override Control.ControlCollection CreateControlsInstance() {
			return new DocumentContainerCollection(this);
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new DocumentContainerCollection Controls {
			get { return base.Controls as DocumentContainerCollection; }
		}
		[Designer("", "")]
		public class DocumentContainerCollection : ControlCollection {
			IDocumentsHost documentHostCore;
			public IDocumentsHost DocumentHost {
				get { return documentHostCore; }
			}
			public DocumentContainerCollection(IDocumentsHost owner)
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
		#endregion DocumentContainerCollection
		#region IDocumentsHost Members
		void IDocumentsHost.OnControlAdded(Control control) {
			if(Owner != null)
				Owner.ControlAdded(control);
		}
		void IDocumentsHost.OnControlRemoved(Control control) {
			if(Owner != null)
				Owner.ControlRemoved(control);
			if(ActiveContainer == control) {
				activeContainerCore = null;
				activeDocumentCore = null;
			}
		}
		void IDocumentsHost.UpdateLayout() {
			ContainerControlManagementStrategy.ProcessNC(Handle);
		}
		#endregion
		public static IDocumentsHost GetDocumentsHost(ContainerControl container) {
			if(container == null)
				return null;
			return GetDocumentsHostCore(container);
		}
		internal static IDocumentsHost GetDocumentsHostCore(Control candidate) {
			IDocumentsHost host = candidate as IDocumentsHost;
			if(host != null)
				return host;
			if(candidate == null)
				return null;
			foreach(Control childControl in candidate.Controls) {
				host = GetDocumentsHostCore(childControl);
				if(host != null)
					return host;
			}
			return null;
		}
		protected override void WndProc(ref Message m) {
			if(FlickGestureHelper.WndProc(ref m)) return;
			if(GestureHelper.WndProc(ref m)) return;
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
				case WinAPI.WM_PARENTNOTIFY:
					DoParentNotify(ref m);
					break;
				case MSG.WM_KILLFOCUS:
					ProcessFocus(ref m, false);
					break;
				case MSG.WM_SETFOCUS:
					ProcessFocus(ref m, true);
					break;
				case MSG.WM_NCCALCSIZE:
					DoNCCalcSize(ref m);
					break;
				default:
					base.WndProc(ref m);
					break;
			}
			XtraBars.CodedUISupport.CodedUIMessagesHandler.ProcessCodedUIMessage(ref m, this);
		}
		private void DoPrint(ref Message m) {
			try {
				using(Graphics g = Graphics.FromHdc(m.WParam))
					OnPaint(g);
			}
			finally { m.Result = IntPtr.Zero; }
		}
		protected virtual void ProcessFocus(ref Message m, bool setFocus) {
			if(Owner != null)
				Owner.Invalidate();
			m.Result = IntPtr.Zero;
		}
		bool IDocumentsHost.ActivatePreviousDocument(Views.BaseDocument document, Views.BaseView view) {
			if(document != null) {
				if(view != null && !view.IsDisposing) {
					var prevActivatedDocument = System.Linq.Enumerable.FirstOrDefault(view.Manager.ActivationInfo.DocumentActivationList,
						(doc) => doc.IsVisible && !doc.IsContainer && doc.IsControlLoaded && doc.CanActivate());
					if(prevActivatedDocument != null) {
						DocumentContainer container = prevActivatedDocument.Control.Parent as DocumentContainer;
						if(container != null) {
							container.Select();
							return true;
						}
					}
				}
				else activeDocumentCore = null;
			}
			return false;
		}
		protected virtual void DoParentNotify(ref Message m) {
			if(WinAPIHelper.GetInt(m.WParam) == MSG.WM_LBUTTONDOWN) {
				Point pt = WinAPIHelper.GetPoint(m.LParam);
				if(!IsHandleCreated) return;
				DocumentContainer child;
				if(ParentNotifyHelper.ShouldSelectChild(this, pt, out child)) {
					Point childClientPoint = WinAPIHelper.Translate(Handle, child.Handle, pt);
					bool selectImmediately = false;
					if(!child.NCHitTest(childClientPoint)) {
						if(child.Document.Control is UserControl) return;
					}
					else selectImmediately = true;
					if(child != ActiveContainer || !ControlStylesHelper.IsSelectableRecursive(child.Document.Control)) {
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
		void Select(DocumentContainer child, ContainerControl parentContainer) {
			lockSetActiveContainer++;
			try {
				if(parentContainer.Validate())
					SelectNextControlWithinTheDocumentContainerCore(child);
			}
			finally { lockSetActiveContainer--; }
			if(IsChild(parentContainer.ActiveControl, child))
				SetActiveContainerCore(child);
		}
		bool IsChild(Control control, DocumentContainer container) {
			return Views.DocumentsHostContext.IsChild(control, container);
		}
		protected virtual void DoEraseBackground(ref Message m) {
			m.Result = new IntPtr(1);
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
		protected virtual void DoNCCalcSize(ref Message m) {
			if(m.WParam == IntPtr.Zero) {
				WinAPI.RECT nccsRect = (WinAPI.RECT)m.GetLParam(typeof(WinAPI.RECT));
				Rectangle patchedRectangle = Owner.CalculateNC(nccsRect.ToRectangle());
				nccsRect.RestoreFromRectangle(patchedRectangle);
				BarNativeMethods.StructureToPtr(nccsRect, m.LParam, false);
				m.Result = IntPtr.Zero;
			}
			else {
				WinAPI.NCCALCSIZE_PARAMS nccsParams = (WinAPI.NCCALCSIZE_PARAMS)m.GetLParam(typeof(WinAPI.NCCALCSIZE_PARAMS));
				Rectangle bounds = nccsParams.rgrcProposed.ToRectangle();
				Rectangle patchedRectangle = Owner.CalculateNC(bounds);
				nccsParams.rgrcProposed.RestoreFromRectangle(patchedRectangle);
				BarNativeMethods.StructureToPtr(nccsParams, m.LParam, false);
				m.Result = IntPtr.Zero;
			}
		}
		int lockSetActiveContainer = 0;
		void IDocumentsHost.SetActiveContainer() {
			SetActiveContainerCore();
		}
		protected internal void SetActiveContainerCore() {
			DocumentContainer documentContainer = ActiveContainer;
			IContainerControl parentContainer = Parent as IContainerControl;
			if(parentContainer != null)
				documentContainer = DocumentContainer.FromControl(parentContainer.ActiveControl);
			SetActiveContainerCore(documentContainer);
		}
		Views.BaseDocument activeDocumentCore;
		void IDocumentsHost.SetActiveContainer(DocumentContainer documentContainer) {
			SetActiveContainerCore(documentContainer);
		}
		protected internal void SetActiveContainerCore(DocumentContainer documentContainer) {
			if(ActiveContainer == documentContainer) {
				SelectNextControlWithinTheDocumentContainer(documentContainer);
				return;
			}
			if(lockSetActiveContainer > 0) return;
			lockSetActiveContainer++;
			try {
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
		void SelectNextControlWithinTheDocumentContainerCore(DocumentContainer documentContainer) {
			Control content = ContainerControlManagementStrategy.GetContent(documentContainer);
			if(content != null && !ControlStylesHelper.IsSelectableRecursive(content))
				documentContainer.Select();
			else
				DocumentManager.SelectNextControl(ContainerControlManagementStrategy.CheckNestedContent(content));
		}
		protected void RaiseDocumentContainerActivate() {
			EventHandler handler = (EventHandler)Events[documentContainerActivate];
			if(handler != null)
				handler(this, EventArgs.Empty);
		}
		public new void OnMouseWheel(MouseEventArgs e) {
			base.OnMouseWheel(e);
		}
		static object gesture = new object();
		static object flick = new object();
		#region IFlickGestureClient Members
		public event FlickGestureHandler Flick {
			add { Events.AddHandler(flick, value); }
			remove { Events.RemoveHandler(flick, value); }
		}
		FlickGestureHelper flickGestureHelper;
		FlickGestureHelper FlickGestureHelper {
			get {
				if(flickGestureHelper == null)
					flickGestureHelper = new FlickGestureHelper(this);
				return flickGestureHelper;
			}
		}
		IntPtr IFlickGestureClient.Handle {
			get { return IsHandleCreated ? Handle : IntPtr.Zero; }
		}
		bool IFlickGestureClient.OnFlick(Point point, FlickGestureArgs args) {
			return RaiseFlickEvent(point, args);
		}
		bool RaiseFlickEvent(Point point, FlickGestureArgs args) {
			FlickGestureHandler handler = Events[flick] as FlickGestureHandler;
			if(handler != null)
				return handler(point, args);
			return false;
		}
		#endregion
		#region IGestureClient Members
		public event GestureHandler Gesture {
			add { Events.AddHandler(gesture, value); }
			remove { Events.RemoveHandler(gesture, value); }
		}
		GestureHelper gestureHelper;
		GestureHelper GestureHelper {
			get {
				if(gestureHelper == null) {
					gestureHelper = new GestureHelper(this);
					gestureHelper.PanWithGutter = false;
				}
				return gestureHelper;
			}
		}
		IntPtr IGestureClient.Handle {
			get { return IsHandleCreated ? Handle : IntPtr.Zero; }
		}
		IntPtr IGestureClient.OverPanWindowHandle {
			get { return GestureHelper.FindOverpanWindow(this); }
		}
		GestureAllowArgs[] IGestureClient.CheckAllowGestures(Point point) {
			GestureAllowArgs[] allowArgs = new GestureAllowArgs[] { GestureAllowArgs.Pan };
			object[] parameters = new object[] { point, allowArgs };
			RaiseGesture(new GestureArgs(), GestureID.QueryAllowGesture, parameters);
			return FilterAllowedGesture(parameters[1] as GestureAllowArgs[]);
		}
		GestureAllowArgs[] FilterAllowedGesture(GestureAllowArgs[] allowArgs) {
			if(allowArgs.Length < 2) return allowArgs;
			System.Collections.Generic.List<GestureAllowArgs> list = new System.Collections.Generic.List<GestureAllowArgs>(allowArgs);
			list.Sort(new GestureAllowArgsGIDComparer());
			for(int i = 0; i < list.Count - 1; i++) {
				if(list[i].GID == list[i + 1].GID) {
					list.Remove(list[i]);
				}
			}
			return list.ToArray();
		}
		class GestureAllowArgsGIDComparer : System.Collections.Generic.IComparer<GestureAllowArgs> {
			public int Compare(GestureAllowArgs x, GestureAllowArgs y) {
				if(x == y) return 0;
				return x.GID.CompareTo(y.GID);
			}
		}
		void IGestureClient.OnEnd(GestureArgs info) { }
		void IGestureClient.OnBegin(GestureArgs args) {
			RaiseGesture(args, GestureID.Begin);
		}
		void IGestureClient.OnPan(GestureArgs args, Point delta, ref Point overPan) {
			object[] parameters = new object[] { delta, overPan };
			RaiseGesture(args, GestureID.Pan, parameters);
			overPan = (Point)parameters[1];
		}
		void IGestureClient.OnPressAndTap(GestureArgs args) {
			RaiseGesture(args, GestureID.PressAndTap);
		}
		void IGestureClient.OnRotate(GestureArgs args, Point center, double degreeDelta) {
			object[] parameters = new object[] { center, degreeDelta };
			RaiseGesture(args, GestureID.Rotate, parameters);
		}
		void IGestureClient.OnTwoFingerTap(GestureArgs args) {
			RaiseGesture(args, GestureID.TwoFingerTap);
		}
		void IGestureClient.OnZoom(GestureArgs args, Point center, double zoomDelta) {
			object[] parameters = new object[] { center, zoomDelta };
			RaiseGesture(args, GestureID.Zoom, parameters);
		}
		void RaiseGesture(GestureArgs args, GestureID gid) {
			RaiseGesture(args, gid, new object[] { });
		}
		void RaiseGesture(GestureArgs args, GestureID gid, object[] parameters) {
			GestureHandler handler = Events[gesture] as GestureHandler;
			if(handler != null)
				handler(gid, args, parameters);
		}
		#endregion
	}
}
