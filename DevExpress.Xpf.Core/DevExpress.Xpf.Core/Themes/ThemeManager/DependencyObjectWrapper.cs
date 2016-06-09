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

using System.Windows;
using System;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Core {
	public class DependencyObjectWrapper {
		readonly WeakReference wRef;
		public DependencyObjectWrapper(DependencyObject dObj) {
			wRef = new WeakReference(dObj);
		}
		public DependencyObject Object { get { return wRef.Target as DependencyObject; } }
		public ResourceDictionary Resources {
			get {
				if (Object is FrameworkElement)
					return ((FrameworkElement)Object).Resources;
				if (Object is FrameworkContentElement)
					return ((FrameworkContentElement)Object).Resources;
				throw new ArgumentException("Resources");
			}
		}
		public bool HasDefaultStyleKeyProperty { get { return (Object is FrameworkElement) || (Object is FrameworkContentElement); } }
		public bool IsLoaded {
			get {
				if (Object is FrameworkElement)
					return ((FrameworkElement)Object).IsLoaded;
				if (Object is FrameworkContentElement)
					return ((FrameworkContentElement)Object).IsLoaded;
				throw new ArgumentException("IsLoaded");
			}
		}
		public bool OverridesDefaultStyleKey {
			get {
				if (Object is FrameworkElement)
					return ((FrameworkElement)Object).OverridesDefaultStyle;
				if (Object is FrameworkContentElement)
					return ((FrameworkContentElement)Object).OverridesDefaultStyle;
				throw new ArgumentException("OverridesDefaultStyleKey");
			}
		}
		string ThemeName { get; set; }
		public object GetDefaultStyleKey() {
			if (Object is FrameworkElement)
				return ((FrameworkElement)Object).GetDefaultStyleKey();
			if (Object is FrameworkContentElement)
				return ((FrameworkContentElement)Object).GetDefaultStyleKey();
			throw new ArgumentException("DefaultStyleKey");
		}
		public void SetDefaultStyleKey(object value) {
			if (Object is FrameworkElement) {
				((FrameworkElement)Object).SetDefaultStyleKey(value);
				return;
			}
			if (Object is FrameworkContentElement) {
				((FrameworkContentElement)Object).SetDefaultStyleKey(value);
				return;
			}
			throw new ArgumentException("DefaultStyleKey");
		}
		public void AddHandler(RoutedEvent routedEvent, Delegate handler) {
			FrameworkElement uie = Object as FrameworkElement;
			if (uie != null) {
				uie.AddHandler(routedEvent, handler);
				return;
			}
			FrameworkContentElement fce = Object as FrameworkContentElement;
			if (fce != null) {
				fce.AddHandler(routedEvent, handler);
				return;
			}
			throw new ArgumentException("AddHandler");
		}
		public void RemoveHandler(RoutedEvent routedEvent, Delegate handler) {
			FrameworkElement uie = Object as FrameworkElement;
			if (uie != null) {
				uie.RemoveHandler(routedEvent, handler);
				return;
			}
			FrameworkContentElement fce = Object as FrameworkContentElement;
			if (fce != null) {
				fce.RemoveHandler(routedEvent, handler);
				return;
			}
			throw new ArgumentException("RemoveHandler");
		}
		public void RaiseEvent(RoutedEventArgs e) {
			FrameworkElement uie = Object as FrameworkElement;
			if (uie != null) {
				uie.RaiseEvent(e);
				return;
			}
			FrameworkContentElement fce = Object as FrameworkContentElement;
			if (fce != null) {
				fce.RaiseEvent(e);
				return;
			}
			throw new ArgumentException("RaiseEvent");
		}
		public bool InVisualTree(DependencyObject obj) {
			return LayoutHelper.GetParent(obj, true) != null;
		}
		public void ClearDefaultStyleKey() {
			if (Object is FrameworkElement) {
				((FrameworkElement)Object).ClearDefaultStyleKey();
				return;
			}
			if (Object is FrameworkContentElement) {
				((FrameworkContentElement)Object).ClearDefaultStyleKey();
				return;
			}
			throw new ArgumentException("ClearDefaultStyleKey");
		}
		public System.Windows.Data.BindingExpression GetBindingExpression(DependencyObject dObj, DependencyProperty dProp) {
			FrameworkElement uie = Object as FrameworkElement;
			if (uie != null) {
				return uie.GetBindingExpression(dProp);
			}
			FrameworkContentElement fce = Object as FrameworkContentElement;
			if (fce != null) {
				return fce.GetBindingExpression(dProp);
			}
			throw new ArgumentException("GetBindingExpression");
		}
	}
}
