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

using System.Collections.Generic;
using System.Windows;
using DevExpress.Xpf.Layout.Core.Base;
namespace DevExpress.Xpf.Layout.Core {
	public class BaseLayoutElementHost : BaseObject, ILayoutElementHost {
		ILayoutElement layoutRootCore;
		ILayoutElementFactory defautFactoryCore;
		ILayoutElementBehavior defaultBehaviorCore;
		protected BaseLayoutElementHost()
			: this(null, null) {
		}
		protected BaseLayoutElementHost(ILayoutElementFactory factory, ILayoutElementBehavior behavior) {
			defautFactoryCore = factory;
			defaultBehaviorCore = behavior;
		}
		protected override void OnDispose() {
			defautFactoryCore = null;
			defaultBehaviorCore = null;
			Ref.Dispose(ref layoutRootCore);
			base.OnDispose();
		}
		public virtual HostType Type {
			get { return HostType.Layout; }
		}
		protected ILayoutElementFactory DefaultFactory {
			[System.Diagnostics.DebuggerStepThrough]
			get {
				if(defautFactoryCore == null)
					defautFactoryCore = ResolveDefaultFactory();
				return defautFactoryCore; 
			}
		}
		protected ILayoutElementBehavior DefaultBehavior {
			[System.Diagnostics.DebuggerStepThrough]
			get {
				if(defaultBehaviorCore == null)
					defaultBehaviorCore = ResolveDefaultBehavior();
				return defaultBehaviorCore; 
			}
		}
		protected virtual ILayoutElementFactory ResolveDefaultFactory() {
			return ServiceLocator<ILayoutElementFactory>.Resolve();
		}
		protected virtual ILayoutElementBehavior ResolveDefaultBehavior() {
			return ServiceLocator<ILayoutElementBehavior>.Resolve();
		}
		public ILayoutElement LayoutRoot {
			get { return layoutRootCore; }
		}
		public void EnsureLayoutRoot() {
			layoutRootCore = EnsureLayoutRootCore();
		}
		public void Invalidate() {
			Ref.Dispose(ref layoutRootCore);
		}
		public virtual object RootKey {
			get { return null; }
		}
		public ILayoutElement GetElement(object key) {
			if(LayoutRoot == null)
				EnsureLayoutRoot();
			return DefaultFactory.GetElement(key);
		}
		public ILayoutElement GetDragItem(ILayoutElement element) {
			return (element != null) ? GetDragItemCore(element) : null;
		}
		public ILayoutElementFactory GetLayoutElementFactory() {
			return GetLayoutElementFactoryCore() ?? DefaultFactory;
		}
		public ILayoutElementBehavior GetElementBehavior(ILayoutElement element) {
			return GetElementBehaviorCore(element) ?? DefaultBehavior;
		}
		protected virtual ILayoutElementFactory GetLayoutElementFactoryCore() {
			return null;
		}
		protected virtual ILayoutElementBehavior GetElementBehaviorCore(ILayoutElement element) { 
			return null; 
		}
		protected virtual ILayoutElement GetDragItemCore(ILayoutElement element) {
			return element;
		}
		protected virtual ILayoutElement EnsureLayoutRootCore() {
			return GetLayoutElementFactory().CreateLayoutHierarchy(RootKey);
		}
		public virtual bool IsActiveAndCanProcessEvent {
			get { return layoutRootCore != null && !IsDisposing; }
		}
		public virtual Point ClientToScreen(Point clientPoint) {
			return clientPoint;
		}
		public virtual Point ScreenToClient(Point screenPoint) {
			return screenPoint;
		}
		public void ReleaseCapture() {
			ReleaseCaptureCore();
		}
		public void SetCapture() {
			SetCaptureCore();
		}
		protected virtual void ReleaseCaptureCore() { }
		protected virtual void SetCaptureCore() { }
	}
	public abstract class UILayoutElementFactory : ILayoutElementFactory {
		readonly ElementHash Hash;
		public UILayoutElementFactory() {
			Hash = new ElementHash();
		}
		public ILayoutElement CreateLayoutHierarchy(object rootKey) {
			if(!(rootKey is IUIElement)) return null;
			return CreateUIHierarchy((IUIElement)rootKey, Hash);
		}
		public ILayoutElement GetElement(object key) {
			if(!(key is IUIElement)) return null;
			ILayoutElement element = null;
			return Hash.TryGetValue((IUIElement)key, out element) ? element : null;
		}
		ILayoutElement CreateUIHierarchy(IUIElement rootKey, ElementHash hash) {
			if(hash.Count > 0) hash.Clear();
			if(rootKey == null) return null;
			using(var enumerator = GetUIEnumerator(rootKey)) {
				while(enumerator.MoveNext()) {
					IUIElement uiKey = enumerator.Current;
					ILayoutElement element = CreateElement(uiKey);
					ILayoutElement elementParent = null;
					if((uiKey.Scope != null) && hash.TryGetValue(uiKey.Scope, out elementParent)) {
						BaseLayoutContainer container = elementParent as BaseLayoutContainer;
						if(container != null) container.AddInternal(element);
					}
					hash.Add(uiKey, element);
				}
			}
			return hash[rootKey];
		}
		protected abstract IEnumerator<IUIElement> GetUIEnumerator(IUIElement rootKey);
		protected abstract ILayoutElement CreateElement(IUIElement uiKey);
		#region InternalClasses
		class ElementHash {
			IDictionary<IUIElement, ILayoutElement> hash;
			public ElementHash() {
				hash = new Dictionary<IUIElement, ILayoutElement>();
			}
			public void Add(IUIElement key, ILayoutElement element) {
				hash.Add(key, element);
				element.Disposed += new Sink(key, hash).Disposed;
			}
			public bool TryGetValue(IUIElement key, out ILayoutElement element) {
				return hash.TryGetValue(key, out element);
			}
			public ILayoutElement this[IUIElement key] {
				get { return hash[key]; }
			}
			public int Count {
				get { return hash.Count; }
			}
			public void Clear() {
				ILayoutElement[] elements = new ILayoutElement[hash.Count];
				hash.Values.CopyTo(elements, 0);
				for(int i = 0; i < elements.Length; i++) 
					elements[i].Dispose();
			}
			class Sink {
				IUIElement Key;
				IDictionary<IUIElement, ILayoutElement> Hash;
				public Sink(IUIElement key, IDictionary<IUIElement, ILayoutElement> hash) {
					Key = key;
					Hash = hash;
				}
				public void Disposed(object sender, System.EventArgs e) {
					((ILayoutElement)sender).Disposed -= Disposed;
					Hash.Remove(Key);
					Hash = null;
					Key = null;
				}
			}
		}
		#endregion InternalClasses
	}
}
