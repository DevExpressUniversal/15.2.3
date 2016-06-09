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
using System.Windows;
using System.Collections.Generic;
namespace DevExpress.Xpf.Docking {
	abstract class BaseWeakEventManager<T> : WeakEventManager where T : WeakEventManager, new() {
		protected static T GetManager() {
			Type managerType = typeof(T);
			var manager = (T)WeakEventManager.GetCurrentManager(managerType);
			if(manager == null) {
				manager = new T();
				WeakEventManager.SetCurrentManager(managerType, manager);
			}
			return (T)manager;
		}
	}
	class ThemeChangedEventManager : BaseWeakEventManager<ThemeChangedEventManager> {
		#region static
		public static void AddListener(UIElement source, IWeakEventListener listener) {
			GetManager().ProtectedAddListener(source, listener);
		}
		public static void RemoveListener(UIElement source, IWeakEventListener listener) {
			GetManager().ProtectedRemoveListener(source, listener);
		}
		#endregion static
		public ThemeChangedEventManager() { }
		protected override void StartListening(object source) {
			if(source is DependencyObject)
				Core.ThemeManager.AddThemeChangedHandler((DependencyObject)source, ThemeManager_ThemeChanged);
		}
		protected override void StopListening(object source) {
			if(source is DependencyObject)
				Core.ThemeManager.RemoveThemeChangedHandler((DependencyObject)source, ThemeManager_ThemeChanged);
		}
		void ThemeManager_ThemeChanged(DependencyObject sender, Core.ThemeChangedRoutedEventArgs e) {
			DockLayoutManager manager = sender as DockLayoutManager;
			if(manager != null) manager.OnThemeChanged();
		}
	}
	class OwnerWindowClosedEventManager : BaseWeakEventManager<OwnerWindowClosedEventManager> {
		#region static
		public static void AddListener(Window w, IWeakEventListener listener) {
			GetManager().ProtectedAddListener(w, listener);
		}
		public static void RemoveListener(Window w, IWeakEventListener listener) {
			GetManager().ProtectedRemoveListener(w, listener);
		}
		#endregion static
		public OwnerWindowClosedEventManager() { }
		protected override void StartListening(object source) {
			((Window)source).Closed += DeliverEvent;
		}
		protected override void StopListening(object source) {
			((Window)source).Closed -= DeliverEvent;
		}
	}
	class OwnerWindowBoundsChangedEventManager : BaseWeakEventManager<OwnerWindowBoundsChangedEventManager> {
		#region static
		public static void AddListener(Window w, IWeakEventListener listener) {
			GetManager().ProtectedAddListener(w, listener);
		}
		public static void RemoveListener(Window w, IWeakEventListener listener) {
			GetManager().ProtectedRemoveListener(w, listener);
		}
		#endregion static
		public OwnerWindowBoundsChangedEventManager() { }
		protected override void StartListening(object source) {
			((Window)source).SizeChanged += DeliverEvent;
			((Window)source).LocationChanged += DeliverEvent;
		}
		protected override void StopListening(object source) {
			((Window)source).SizeChanged -= DeliverEvent;
			((Window)source).LocationChanged -= DeliverEvent;
		}
	}
	class VisualRootPreviewKeyDownEventManager : BaseWeakEventManager<VisualRootPreviewKeyDownEventManager> {
		#region static
		public static void AddListener(UIElement root, IWeakEventListener listener) {
			GetManager().ProtectedAddListener(root, listener);
		}
		public static void RemoveListener(UIElement root, IWeakEventListener listener) {
			GetManager().ProtectedRemoveListener(root, listener);
		}
		#endregion static
		public VisualRootPreviewKeyDownEventManager() { }
		protected override void StartListening(object source) {
			((UIElement)source).PreviewKeyDown += DeliverEvent;
		}
		protected override void StopListening(object source) {
			((UIElement)source).PreviewKeyDown -= DeliverEvent;
		}
	}
	class VisualRootPreviewKeyUpEventManager : BaseWeakEventManager<VisualRootPreviewKeyUpEventManager> {
		#region static
		public static void AddListener(UIElement root, IWeakEventListener listener) {
			GetManager().ProtectedAddListener(root, listener);
		}
		public static void RemoveListener(UIElement root, IWeakEventListener listener) {
			GetManager().ProtectedRemoveListener(root, listener);
		}
		#endregion static
		public VisualRootPreviewKeyUpEventManager() { }
		protected override void StartListening(object source) {
			((UIElement)source).PreviewKeyUp += DeliverEvent;
		}
		protected override void StopListening(object source) {
			((UIElement)source).PreviewKeyUp -= DeliverEvent;
		}
	}
	class BaseLayoutItemWeakEventManager : BaseWeakEventManager<BaseLayoutItemWeakEventManager> {
		#region static
		public static void AddListener(UIElement root, IWeakEventListener listener) {
			GetManager().ProtectedAddListener(root, listener);
		}
		public static void RemoveListener(UIElement root, IWeakEventListener listener) {
			GetManager().ProtectedRemoveListener(root, listener);
		}
		#endregion static
		protected override void StartListening(object source) {
			BaseLayoutItem item = source as BaseLayoutItem;
			if(item != null) {
				item.VisualChanged += DeliverEvent;
			}
		}
		protected override void StopListening(object source) {
			BaseLayoutItem item = source as BaseLayoutItem;
			if(item != null) {
				item.VisualChanged -= DeliverEvent;
			}
		}
	}
}
