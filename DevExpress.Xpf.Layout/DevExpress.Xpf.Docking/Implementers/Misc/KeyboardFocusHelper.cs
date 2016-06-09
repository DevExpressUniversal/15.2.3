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

using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Docking.Platform;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
#if SILVERLIGHT
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Docking {
	static class KeyboardFocusHelper {
		public static void RestoreKeyboardFocus(FrameworkElement element) {
			var scope = FocusManager.GetIsFocusScope(element) ? element : FocusManager.GetFocusScope(element);
			var focused = FocusManager.GetFocusedElement(scope);
			if(focused != null) {
				FocusElement(focused as UIElement);
			}
		}
		public static void FocusElement(UIElement element, bool ignoreCurrentFocusState = false) {
			if(element != null && (ignoreCurrentFocusState || !IsKeyboardFocusWithin(element))) {
				DependencyObject elementToFocus = GetElementToFocus(element);
				if(elementToFocus != null) {
					if(InvokePostFocusRequired(elementToFocus)) {
						InvokeHelper.BeginInvoke(elementToFocus, new Action<DependencyObject>(PostFocus),
							InvokeHelper.Priority.Loaded, elementToFocus);
					} else
						PostFocus(elementToFocus);
				}
			}
		}
		static DependencyObject GetElementToFocus(DependencyObject element) {
			var scope = FocusManager.GetIsFocusScope(element) ? element : FocusManager.GetFocusScope(element);
			var focusedElement = FocusManager.GetFocusedElement(scope) as DependencyObject;
			if(focusedElement != null)
				if(LayoutHelper.IsChildElement(element, focusedElement))
					return focusedElement;
			if(element is IInputElement)
				return element;
			return null;
		}
		static bool InvokePostFocusRequired(DependencyObject dObj) {
			if(!WindowHelper.IsXBAP)
				return InvokePostFocusRequiredCore(dObj);
			return LayoutHelper.FindParentObject<Page>(dObj) == null;
		}
		static bool InvokePostFocusRequiredCore(DependencyObject dObj) {
			return PresentationSource.FromDependencyObject(dObj) == null;
		}
		static void PostFocus(DependencyObject elementToFocus) {
			elementToFocus = LayoutItemsHelper.FindElementInVisualTree(elementToFocus, CanFocusSkipCallback, CanFocusResultCallback);
			if(elementToFocus != null)
				PostFocusCore(elementToFocus);
		}
		static bool CanFocusSkipCallback(DependencyObject d) {
			return d is Bars.BarItemLinkInfo || d is ToolBar || (d is UIElement && !((UIElement)d).IsVisible) || d is Ribbon.IRibbonStatusBarControl || d is Ribbon.IRibbonControl;
		}
		static bool CanFocusResultCallback(DependencyObject e) {
			if(e is Bars.BarManager)
				return false;
			var inputElement = e as IInputElement;
			if(inputElement != null)
				return inputElement.Focusable;
			return false;
		}
		static void PostFocusCore(DependencyObject elementToFocus) {
			UIElement uiElement = elementToFocus as UIElement;
			IInputElement inputElement = elementToFocus as IInputElement;
			bool success = uiElement.Return(x => x.Focus(), () => false);
			if(!success)
				inputElement.If(x => !x.IsKeyboardFocusWithin).Do(x => Keyboard.Focus(x));
		}
		public static void SetFocus(UIElement element) {
			element.Focus();
		}
		public static bool Focusable(UIElement element) {
			return element.Focusable;
		}
		public static bool IsKeyboardFocused(UIElement element) {
			return element.IsKeyboardFocused;
		}
		public static bool IsKeyboardFocusWithin(UIElement element) {
			return element.IsKeyboardFocusWithin;
		}
		public static IInputElement Convert(UIElement element) {
			return element;
		}
		public static DependencyObject FocusedElement {
			get { return Keyboard.FocusedElement as DependencyObject; }
		}
	}
}
