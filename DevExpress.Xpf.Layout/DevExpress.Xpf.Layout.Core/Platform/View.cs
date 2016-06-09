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
using System.Windows.Input;
using DevExpress.Xpf.Layout.Core.Base;
namespace DevExpress.Xpf.Layout.Core.Platform {
	public class BaseView : BaseLayoutElementHost, IView {
		IDictionary<object, IUIServiceListener> defaultListenersCore;
		IDictionary<object, IUIServiceListener> customListenersCore;
		protected BaseView(ILayoutElementFactory factory, ILayoutElementBehavior behavior)
			: base(factory, behavior) {
		}
		protected override void OnCreate() {
			defaultListenersCore = new Dictionary<object, IUIServiceListener>();
			customListenersCore = new Dictionary<object, IUIServiceListener>();
			base.OnCreate();
		}
		protected override void OnDispose() {
			Ref.Clear(ref defaultListenersCore);
			Ref.Clear(ref customListenersCore);
			adapterCore = null;
			base.OnDispose();
		}
		internal IViewAdapter adapterCore;
		public IViewAdapter Adapter {
			get { return adapterCore; }
		}
		#region IView
		int? zOrderCore;
		public int ZOrder {
			get {
				if(!zOrderCore.HasValue)
					zOrderCore = CalcZOrder();
				return zOrderCore.Value;
			}
		}
		public void InvalidateZOrder() {
			zOrderCore = null;
		}
		protected virtual int CalcZOrder() {
			return 0;
		}
		protected internal virtual bool CanSuspendDocking(ILayoutElement dragItem) {
			return false;
		}
		protected internal virtual bool CanSuspendFloating(ILayoutElement dragItem) {
			return false;
		}
		protected internal virtual bool CanSuspendClientDragging(ILayoutElement dragItem) {
			return false;
		}
		protected internal virtual bool CanSuspendResizing(ILayoutElement dragItem) {
			return false;
		}
		protected internal virtual bool CanSuspendReordering(ILayoutElement dragItem) {
			return false;
		}
		protected internal virtual bool CanSuspendFloatingMoving(ILayoutElement dragItem) {
			return false;
		}
		protected internal virtual bool CanSuspendBehindDragging(ILayoutElement dragItem) {
			return false;
		}
		protected internal virtual bool CheckReordering(System.Windows.Point point) {
			return true;
		}
		public void OnKeyDown(Key key) {
			Adapter.ProcessKey(this, KeyEventType.KeyDown, key);
		}
		public void OnKeyUp(Key key) {
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
		public void OnMouseLeave() {
			Adapter.ProcessMouseEvent(this, MouseEventType.MouseLeave, null);
		}
		protected virtual bool CanHandleMouseDownCore() {
			return Adapter.DragService.DragItem != null;
		}
		public bool CanHandleMouseDown() {
			return CanHandleMouseDownCore();
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
			ServiceListener listener=null;
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
	}
	public class ViewCollection : BaseChangeableList<IView> {
		IViewAdapter ownerCore;
		public ViewCollection(IViewAdapter owner) {
			ownerCore = owner;
		}
		protected override void OnDispose() {
			ownerCore = null;
			IView[] elements = ToArray();
			for(int i = 0; i < elements.Length; i++) {
				IView view = elements[i];
				view.Disposed -= OnLayoutItemDisposed;
				Ref.Dispose(ref view);
			}
			base.OnDispose();
		}
		protected override void OnElementAdded(IView element) {
			base.OnElementAdded(element);
			AffinityHelper.SetAffinity(ownerCore, element);
			element.Disposed += OnLayoutItemDisposed;
		}
		protected override void OnElementRemoved(IView element) {
			element.Disposed -= OnLayoutItemDisposed;
			AffinityHelper.SetAffinity(null, element);
			base.OnElementRemoved(element);
		}
		void OnLayoutItemDisposed(object sender, EventArgs ea) {
			RaiseCollectionChanged(
					new CollectionChangedEventArgs<IView>(sender as IView, CollectionChangedType.ElementDisposed)
				);
			if(List != null) Remove(sender as IView);
		}
		#region internal classes
		static class AffinityHelper {
			public static void SetAffinity(IViewAdapter adapter, IView element) {
				if(AffinityHelperException.Assert(element)) {
					((BaseView)element).adapterCore = adapter;
				}
			}
		}
		#endregion internal classes
	}
}
