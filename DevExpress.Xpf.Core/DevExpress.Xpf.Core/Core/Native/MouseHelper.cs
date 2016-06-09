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
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;
using System.Collections;
using System.Windows.Controls;
#if SILVERLIGHT
using DevExpress.Xpf.Core.Native;
#endif
namespace DevExpress.Utils {
#if SILVERLIGHT
	public enum MouseButtonState { Released = 0, Pressed = 1 }
#endif
	public static class MouseHelper {
		public static void SubscribeLeftButtonDoubleClick(Control element, EventHandler handler) {
#if SILVERLIGHT
			element.AddHandler(FrameworkElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(delegate(object sender, MouseButtonEventArgs e) {
				if(IsDoubleClick(e) && handler != null)
					handler(sender, EventArgs.Empty);
			}), true);
#else
			element.AddHandler(Control.MouseDoubleClickEvent, new MouseButtonEventHandler(delegate(object sender, MouseButtonEventArgs e) {
				if(e.LeftButton == MouseButtonState.Pressed && handler != null)
					handler(sender, EventArgs.Empty);
			}));
#endif
		}
#if SILVERLIGHT
		static int Timeout = 500;
		static object clicked = null;
		static List<WeakReference> ClickDownElements = new List<WeakReference>();
		static Point position;
		public static bool IsDoubleClick(MouseButtonEventArgs e) {
			if(clicked != null) {
				if(clicked != e) {
					clicked = null;
					return position.Equals(e.GetPosition(null));
				}
				return false;
			}
			clicked = e;
			position = e.GetPosition(null);
			ParameterizedThreadStart threadStart = new ParameterizedThreadStart(ResetThread);
			Thread thread = new Thread(threadStart);
			thread.Start();
			return false;
		}
		public static void StartClickDown(MouseButtonEventArgs e, FrameworkElement element, Type type) {
			IEnumerable list = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(null), element).Where(el => el.GetType() == type);
			foreach(UIElement uiElement in list)
				ClickDownElements.Add(new WeakReference(uiElement));
		}
		public static IEnumerable IsClick(MouseButtonEventArgs e, FrameworkElement element) {
			List<UIElement> list = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(null), element).ToList();
			foreach(WeakReference elem in ClickDownElements) {
				UIElement uiElement = (UIElement)elem.Target;
				if(list.Contains(uiElement))
					yield return uiElement;
			}
			ClickDownElements.Clear();
		}
		private static void ResetThread(object state) {
			Thread.Sleep(Timeout);
			clicked = null;
		}
#endif
#if DEBUGTEST && SL
		public static void ForseSetCaptured(UIElement element) {
			captured = element;
		}
#endif
#if !SILVERLIGHT
		public static IInputElement Captured { 
			get {
				return Mouse.Captured;
			}
		}
#else
		static UIElement captured = null;
		public static UIElement Captured {
			get {
				return captured;
			}
			private set {
				if(value == captured)
					return;
				captured = value;
			}
		}
#endif
		public static bool Capture(UIElement element) {
			bool isCaptured = false;
#if SILVERLIGHT
			if(element == null) return isCaptured;
			isCaptured = element.CaptureMouse();
			if(isCaptured)
				Captured = element;
#else
			isCaptured = Mouse.Capture(element);
#endif
			return isCaptured;
		}
		public static void ReleaseCapture(UIElement element) {
#if SILVERLIGHT
			element.ReleaseMouseCapture();
			if(Captured == element)
				Captured = null;
#else
			if(Mouse.Captured == element) 
				Mouse.Capture(null);
#endif
		}
		public static bool IsMouseLeftButtonPressed(MouseEventArgs e) {
#if SILVERLIGHT
			return e is MouseButtonEventArgs;
#else 
			return Mouse.LeftButton == MouseButtonState.Pressed;		
#endif
		}
		public static bool IsMouseLeftButtonReleased(MouseEventArgs e) {
#if SILVERLIGHT
			return true;
#else             
			return Mouse.LeftButton == MouseButtonState.Released;
#endif
		}
		public static bool IsMouseRightButtonPressed(MouseEventArgs e) {
#if SILVERLIGHT
			return false;
#else             
			return Mouse.RightButton == MouseButtonState.Pressed;
#endif
		}
		public static bool IsMouseButtonPressed(MouseButtonEventArgs e) {
#if SILVERLIGHT
			return e is MouseButtonEventArgs;
#else             
			return e.ButtonState == MouseButtonState.Pressed;
#endif
		}
		public static Point GetPosition(FrameworkElement relativeTo) {
#if SILVERLIGHT
			return new Point();
#else             
			return Mouse.GetPosition(relativeTo);
#endif
		}
	}
}
