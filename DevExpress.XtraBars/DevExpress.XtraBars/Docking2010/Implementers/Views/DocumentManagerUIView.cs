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
using System.Collections.Generic;
using DevExpress.XtraTabbedMdi;
using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraBars.Docking2010.DragEngine;
namespace DevExpress.XtraBars.Docking2010.Dragging {
	public class DocumentManagerUIView : DragEngine.BaseUIView {
		DocumentManager managerCore;
		Customization.DragFrame dragFrameCore;
		public DocumentManagerUIView(DocumentManager manager)
			: base(null, null) {
			managerCore = manager;
			RootUIElement = Manager.GetOwnerControl();
			SubscribeMouseEvents(RootUIElement);
			RegisterListeners();
		}
		protected override void OnDispose() {
			Ref.Dispose(ref dragFrameCore);
			UnSubscribeMouseEvents(RootUIElement);
			ReleaseCaptureCore();
			RootUIElement = null;
			managerCore = null;
			base.OnDispose();
		}
		protected virtual void RegisterListeners() {
			RegisterListeners(Manager.View);
		}
		protected internal void RegisterListeners(Views.BaseView view) {
			ResetListeners();
			if(view != null) 
				view.RegisterListeners(this);
		}
		public DocumentManager Manager {
			get { return managerCore; }
		}
		public override object RootKey {
			get { return managerCore; }
		}
		protected Control RootUIElement; 
		protected override ILayoutElementFactory ResolveDefaultFactory() {
			return new DocumentManagerElementFactory();
		}
		protected override ILayoutElementBehavior GetElementBehaviorCore(ILayoutElement element) {
			return ((IDocumentLayoutElement)element).GetBehavior();
		}
		protected virtual void SubscribeMouseEvents(Control element) {
			SubscribeCore(GetSubscriptionKey(element, SubscriptionType.Mouse),
				new MouseEventSubscriber<Control>(element, this));
		}
		protected virtual void UnSubscribeMouseEvents(Control element) {
			UnSubscribeCore(GetSubscriptionKey(element, SubscriptionType.Mouse));
		}
		protected void UnsubscribeKeyboardEvent() {
			if(KeyboardFocusHolder != null) {
				UnSubscribeCore(GetSubscriptionKey(KeyboardFocusHolder, SubscriptionType.Keyboard));
				KeyboardFocusHolder = null;
			}
		}
		Control KeyboardFocusHolder;
		protected void SubscribeKeyboardEvent() {
			Control ctrl = Manager.GetActiveChild();
			if(KeyboardFocusHolder == ctrl) return;
			if(KeyboardFocusHolder != null)
				UnSubscribeCore(KeyboardFocusHolder);
			KeyboardFocusHolder = ctrl;
			if(KeyboardFocusHolder != null) {
				SubscribeCore(GetSubscriptionKey(KeyboardFocusHolder, SubscriptionType.Keyboard),
					new KeyboardEventSubscriber(KeyboardFocusHolder, this));
			}
		}
		protected override void SetCaptureCore() {
			if(!RootUIElement.Capture)
				RootUIElement.Capture = true;
			SubscribeKeyboardEvent();
		}
		protected override void ReleaseCaptureCore() {
			UnsubscribeKeyboardEvent();
			if(RootUIElement != null)
				RootUIElement.Capture = false;
		}
		public override Point ScreenToClient(Point screenPoint) {
			return RootUIElement.PointToClient(screenPoint);
		}
		public override Point ClientToScreen(Point clientPoint) {
			return RootUIElement.PointToScreen(clientPoint);
		}
		protected override IntPtr CalcZHandle() {
			Form form = Views.DocumentsHostContext.GetForm(Manager);
			if(form != null && form.IsMdiChild)
				form = form.MdiParent;
			if(form != null && !form.TopLevel) {
				if(form.Parent != null)
					form = form.ParentForm;
			}
			if(form != null && form.IsHandleCreated) {
				if(IsChildForm(form.Handle))
					return ZOrderHelper.GetRoot(form.Handle);
				return form.Handle;
			}
			return IntPtr.Zero;
		}
		bool IsChildForm(IntPtr handle) {
			int windowStyles = DevExpress.Utils.Drawing.Helpers.NativeMethods.GetWindowLong(handle, BarNativeMethods.GWL_STYLE);
			return (windowStyles & BarNativeMethods.WS_CHILD) != 0;
		}
		protected internal override bool CanSuspendFloating(ILayoutElement dragItem) {
			Views.IBaseDocumentInfo info = InfoHelper.GetBaseDocumentInfo(dragItem);
			if(info == null || info.BaseDocument == null || !info.BaseDocument.IsEnabled) 
				return true;
			return !Manager.View.OnBeginFloating(info.BaseDocument, IsDoubleClickFloating ?
				Views.FloatingReason.DoubleClick : Views.FloatingReason.Dragging);
		}
		public void BeginDocking(Point screenPoint, ILayoutElement dragItem) {
			Views.IBaseDocumentInfo info = InfoHelper.GetBaseDocumentInfo(dragItem);
			if(info != null && info.BaseDocument != null) {
				if(dragFrameCore == null) {
					dragFrameCore = new Customization.DragFrame();
					dragFrameCore.Create(Manager.GetContainer());
				}
				dragFrameCore.SkinProvider = Manager.LookAndFeel;
				dragFrameCore.Target = info.BaseDocument.Form;
				dragFrameCore.Show(GetDragFramePos());
			}
		}
		public void Docking(Point screenPoint) {
			if(dragFrameCore != null)
				dragFrameCore.Show(GetDragFramePos());
		}
		public void EndDocking() {
			if(dragFrameCore != null) {
				dragFrameCore.Target = null;
				dragFrameCore.Hide();
			}
		}
		static Point GetDragFramePos() {
			Point mousePos = Control.MousePosition;
			mousePos.Offset(-15, -7);
			return mousePos;
		}
		protected internal override bool CanSuspendResizing(ILayoutElement dragItem) {
			Views.IBaseSplitterInfo info = InfoHelper.GetBaseSplitterInfo(dragItem);
			if(info == null || info.Owner == null)
				return true;
			return !Manager.View.RaiseBeginSizing(info);
		}
		protected internal override bool CanSuspendTabMouseActivation(ILayoutElement dragItem) {
			Views.BaseDocument document = null;
			if(dragItem is IDocumentLayoutElement) {
				Views.IBaseDocumentInfo info = InfoHelper.GetBaseDocumentInfo(dragItem);
				document = info != null ? info.BaseDocument : null;
			}
			return document!= null ? Manager.View.RaiseTabMouseActivating(document) : false;
		}
		protected internal override bool DoValidate() {
			if(Manager == null || Manager.View == null || Manager.View.ActiveDocument == null) return true;
			return Manager.View.ActiveDocument.DoValidate();
		}
		#region UIView Comparer
		public static IComparer<IUIView> DefaultComparer {
			get { return UIViewComparer.Default; }
		}
		class UIViewComparer : IComparer<IUIView> {
			internal static IComparer<IUIView> Default = new UIViewComparer();
			UIViewComparer() { }
			int IComparer<IUIView>.Compare(IUIView x, IUIView y) {
				if(object.Equals(x, y)) return 0;
				if(x is DocumentsHostWindowUIView && y is DocumentManagerUIView) return 1;
				if(y is DocumentsHostWindowUIView && x is DocumentManagerUIView) return -1;
				return 0;
			}
		}
		#endregion UIView Comparer
	}
}
