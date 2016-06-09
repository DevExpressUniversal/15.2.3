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
using System.Collections.Generic;
using System.Windows.Forms;
namespace DevExpress.XtraBars.Docking2010.DragEngine {
	public class BaseUIView : BaseLayoutElementHost, IUIView {
		IDictionary<object, IUIServiceListener> defaultListenersCore;
		IDictionary<object, IUIServiceListener> customListenersCore;
		internal int doubleClickFloatingCore;
		protected BaseUIView(ILayoutElementFactory factory, ILayoutElementBehavior behavior)
			: base(factory, behavior) {
		}
		protected override void OnCreate() {
			defaultListenersCore = new Dictionary<object, IUIServiceListener>();
			customListenersCore = new Dictionary<object, IUIServiceListener>();
			base.OnCreate();
		}
		protected override void OnDispose() {
			Ref.DisposeItemsAndClear(ref subscriptions);
			Ref.Clear(ref defaultListenersCore);
			Ref.Clear(ref customListenersCore);
			adapterCore = null;
			base.OnDispose();
		}
		internal IUIViewAdapter adapterCore;
		public IUIViewAdapter Adapter {
			get { return adapterCore; }
		}
		public bool IsDoubleClickFloating {
			get { return doubleClickFloatingCore > 0; }
		}
		#region IView
		IntPtr? zHandleCore;
		public IntPtr ZHandle {
			get {
				if(!zHandleCore.HasValue)
					zHandleCore = CalcZHandle();
				return zHandleCore.Value;
			}
		}
		public void InvalidateZHandle() {
			zHandleCore = null;
		}
		protected virtual IntPtr CalcZHandle() {
			return IntPtr.Zero;
		}
		protected internal virtual bool DoValidate() {
			return true;
		}
		protected internal virtual bool CanSuspendDocking(ILayoutElement dragItem) {
			return false;
		}
		protected internal virtual bool CanSuspendFloating(ILayoutElement dragItem) {
			return false;
		}
		protected internal virtual bool CanSuspendTabMouseActivation(ILayoutElement dragItem) {
			return false;
		}
		protected internal virtual bool CanSuspendResizing(ILayoutElement dragItem) {
			return false;
		}
		protected internal virtual bool CanSuspendBehindDragging(ILayoutElement dragItem) {
			return false;
		}
		protected internal virtual bool CheckReordering(Point point) {
			return true;
		}
		public void OnKeyDown(Keys key) {
			Adapter.ProcessKey(this, KeyEventType.KeyDown, key);
		}
		public void OnKeyUp(Keys key) {
			Adapter.ProcessKey(this, KeyEventType.KeyUp, key);
		}
		public void OnMouseDown(MouseEventArgs ea) {
			Adapter.ProcessMouseEvent(this, MouseEventType.MouseDown, ea);
		}
		public void OnMouseUp(MouseEventArgs ea) {
			Adapter.ProcessMouseEvent(this, MouseEventType.MouseUp, ea);
		}
		public void OnMouseMove(MouseEventArgs ea) {
			Adapter.ProcessMouseEvent(this, MouseEventType.MouseMove, ea);
		}
		public void OnMouseWheel(MouseEventArgs ea) {
			Adapter.ProcessMouseEvent(this, MouseEventType.MouseWheel, ea);
		}
		public void OnMouseLeave() {
			Adapter.ProcessMouseEvent(this, MouseEventType.MouseLeave, null);
		}
		public bool OnFlick(Point point, DevExpress.Utils.Gesture.FlickGestureArgs args) {
			return Adapter.ProcessFlickEvent(this, point, args);
		}
		public void OnGesture(GestureID gid, DevExpress.Utils.Gesture.GestureArgs args, object[] parameters) {
			Adapter.ProcessGesture(this, gid, args, parameters);
		}
		#endregion
		#region IUIServiceProvider
		public void RegisterUIServiceListener(IUIServiceListener listener) {
			if(listener != null) {
				defaultListenersCore[listener.Key] = listener;
				listener.ServiceProvider = this;
			}
		}
		public ServiceListener GetUIServiceListener<ServiceListener>(object key)
			where ServiceListener : class, IUIServiceListener {
			return GetUIServiceListenerCore<ServiceListener>(key);
		}
		public ServiceListener GetUIServiceListener<ServiceListener>()
			where ServiceListener : class, IUIServiceListener {
			return GetUIServiceListenerCore<ServiceListener>(typeof(ServiceListener));
		}
		ServiceListener GetUIServiceListenerCore<ServiceListener>(object key)
			where ServiceListener : class, IUIServiceListener {
			ServiceListener listener = null;
			if(CanUseCustomServiceListener(key)) {
				IUIServiceListener customListener = null;
				if(!customListenersCore.TryGetValue(key, out customListener)) {
					customListener = GetCustomUIServiceListener<ServiceListener>(key);
					customListener.ServiceProvider = this;
					if(customListener is ServiceListener)
						customListenersCore.Add(key, customListener);
				}
				listener = customListener as ServiceListener;
			}
			return listener ?? GetDefaultUIServiceListener<ServiceListener>(key);
		}
		protected internal void ResetListeners() {
			defaultListenersCore.Clear();
			customListenersCore.Clear();
		}
		protected virtual bool CanUseCustomServiceListener(object key) { return false; }
		protected virtual ServiceListener GetCustomUIServiceListener<ServiceListener>(object key)
			where ServiceListener : class, IUIServiceListener {
			return null;
		}
		protected virtual ServiceListener GetDefaultUIServiceListener<ServiceListener>(object key)
			where ServiceListener : class, IUIServiceListener {
			IUIServiceListener listener;
			return (ServiceListener)(defaultListenersCore.TryGetValue(key, out listener) ? listener : null);
		}
		#endregion
		#region Subscriptions
		protected enum SubscriptionType : int {
			Mouse = 0x01, Keyboard = 0x02
		}
		protected object GetSubscriptionKey(Control control, SubscriptionType type) {
			return control.GetHashCode() ^ (int)type;
		}
		IDictionary<object, IDisposable> subscriptions;
		protected void SubscribeCore(object element, IDisposable subscriber) {
			if(element == null) return;
			if(subscriptions == null)
				subscriptions = new Dictionary<object, IDisposable>();
			IDisposable existingSubscriber;
			if(subscriptions.TryGetValue(element, out existingSubscriber)) {
				Ref.Dispose(ref existingSubscriber);
				subscriptions[element] = subscriber;
			}
			else subscriptions.Add(element, subscriber);
		}
		protected void UnSubscribeCore(object element) {
			if(element == null || subscriptions == null) return;
			IDisposable subscriber;
			if(subscriptions.TryGetValue(element, out subscriber)) {
				Ref.Dispose(ref subscriber);
				subscriptions.Remove(element);
			}
		}
		#endregion
	}
}
