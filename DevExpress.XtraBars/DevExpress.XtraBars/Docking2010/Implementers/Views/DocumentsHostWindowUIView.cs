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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraBars.Docking2010.DragEngine;
using DevExpress.XtraBars.Docking2010.Views;
namespace DevExpress.XtraBars.Docking2010.Dragging {
	class DocumentsHostWindowUIView : DragEngine.BaseUIView, ISnapHelperOwnerView {
		IDocumentsHostWindow window;
		IDocumentsHostWindowRoot root;
		public DocumentsHostWindowUIView(IDocumentsHostWindow window)
			: base(null, null) {
			this.window = window;
			RegisterListeners();
			SubscribeMouseEvents();
		}
		protected override void OnDispose() {
			UnSubscribeMouseEvents();
			UnSubscribeKeyboardEvents();
			this.root = null;
			this.window = null;
			base.OnDispose();
		}
		public override HostType Type {
			get { return HostType.Floating; }
		}
		DocumentManager ISnapHelperOwnerView.Manager {
			get { return window.DocumentManager; }
		}
		protected override ILayoutElementFactory ResolveDefaultFactory() {
			return new DocumentsHostWindowElementFactory();
		}
		public Form Form {
			get { return window as Form; }
		}
		public override object RootKey {
			get {
				if(root == null)
					this.root = CreateDocumentsHostWindowRoot();
				return root; 
			}
		}
		protected virtual IDocumentsHostWindowRoot CreateDocumentsHostWindowRoot() {
			return new DocumentsHostWindowRoot(window);
		}
		protected override ILayoutElementBehavior GetElementBehaviorCore(ILayoutElement element) {
			return ((IDocumentLayoutElement)element).GetBehavior();
		}
		public override Point ClientToScreen(Point clientPoint) {
			return Form.PointToScreen(clientPoint);
		}
		public override Point ScreenToClient(Point screenPoint) {
			return Form.PointToClient(screenPoint);
		}
		protected override IntPtr CalcZHandle() {
			return Form.IsHandleCreated ? Form.Handle : IntPtr.Zero;
		}
		int floatingMovingCounter = 0;
		protected internal void LockFloatLocation() {
			if(0 == floatingMovingCounter++)
				snapBounds = null;
		}
		Rectangle? snapBounds;
		protected internal void UnlockFloatLocation() {
			if(--floatingMovingCounter == 0) {
				if(snapBounds.HasValue) {
					Form.BeginInvoke(new Action<Rectangle>(SetFloatingBoundsCore), snapBounds.Value);
					snapBounds = null;
				}
			}
		}
		bool ISnapHelperOwnerView.CanEmulateSnapping {
			get { return true; }
		}
		public bool IsFloatLocationLocked {
			get { return floatingMovingCounter > 0; }
		}
		public void SetFloatLocation(Point point) {
			if(IsFloatLocationLocked) return;
			LockFloatLocation();
			SetFloatLocationCore(point);
			UnlockFloatLocation();
		}
		public void SetFloatingBounds(Rectangle bounds) {
			if(IsFloatLocationLocked) {
				snapBounds = bounds;
				return;
			}
			LockFloatLocation();
			SetFloatingBoundsCore(bounds);
			UnlockFloatLocation();
		}
		protected virtual void SetFloatLocationCore(Point point) {
			Form.Location = point;
		}
		protected virtual void SetFloatingBoundsCore(Rectangle bounds) {
			Form.Bounds = bounds;
		}
		protected virtual void RegisterListeners() {
			RegisterUIServiceListener(new DocumentHostWindowUIViewRegularDragListener());
			RegisterUIServiceListener(new BaseFloatFormUIViewFloatingMovingListener());
			RegisterUIServiceListener(new BaseFloatFormUIViewNonClientDragListener());
		}
		protected virtual void SubscribeMouseEvents() {
			SubscribeCore(GetSubscriptionKey(Form, SubscriptionType.Mouse),
				new MouseEventSubscriber<Form>(Form, this));
		}
		protected virtual void UnSubscribeMouseEvents() {
			UnSubscribeCore(GetSubscriptionKey(Form, SubscriptionType.Mouse));
		}
		protected virtual void SubscribeKeyboardEvents() {
			SubscribeCore(GetSubscriptionKey(Form, SubscriptionType.Keyboard),
				new KeyboardEventSubscriber(Form, this));
		}
		protected virtual void UnSubscribeKeyboardEvents() {
			UnSubscribeCore(GetSubscriptionKey(Form, SubscriptionType.Keyboard));
		}
		protected override void SetCaptureCore() {
			if(!Form.Capture) {
				Form.Capture = true;
				SubscribeKeyboardEvents();
			}
		}
		protected override void ReleaseCaptureCore() {
			if(Form.Capture) {
				UnSubscribeKeyboardEvents();
				Form.Capture = false;
			}
		}
		public void EndFloating(EndFloatingReason reason) {
			IBaseDocumentInfo info = InfoHelper.GetBaseDocumentInfo(Adapter.DragService.DragItem);
			if(info != null && info.BaseDocument != null)
				window.DocumentManager.View.OnEndFloating(info.BaseDocument, reason);
		}
		int externalDraggingCounter = 0;
		public bool InExternalDragging {
			get { return externalDraggingCounter > 0; }
		}
		public void BeginExternalDragging(Point screenPoint) {
			externalDraggingCounter++;
			LockFloatLocation();
			EnsureLayoutRoot();
			Adapter.DragService.DragOrigin = screenPoint;
			Adapter.DragService.DragItem = LayoutRoot;
			Adapter.DragService.SetState(OperationType.FloatingMoving);
		}
		public void ExternalDragging(Point screenPoint) {
			SnapHelper.TryRestoreBounds(LayoutRoot, screenPoint);
			Point clientPoint = ScreenToClient(screenPoint);
			OnMouseMove(GetArgs(MouseButtons.Left, clientPoint));
		}
		public void EndExternalDragging(Point screenPoint) {
			Point clientPoint = ScreenToClient(screenPoint);
			OnMouseUp(GetArgs(MouseButtons.Left, clientPoint));
			if(Adapter != null)
				Adapter.DragService.SetState(OperationType.Regular);
			CancelExternalDragging();
		}
		public void CancelExternalDragging() {
			UnlockFloatLocation();
			Ref.Dispose(ref root);
			Invalidate();
			externalDraggingCounter--;
		}
		static MouseEventArgs GetArgs(MouseButtons buttons, Point pt) {
			return new MouseEventArgs(buttons, 0, pt.X, pt.Y, 0);
		}
	}
}
