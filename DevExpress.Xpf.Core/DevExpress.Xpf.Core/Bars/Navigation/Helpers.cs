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
using DevExpress.Xpf.Bars.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Internal;
using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
namespace DevExpress.Xpf.Bars {
	public static class FocusObserver {
		static bool IsAttached { get; set; }
		static readonly object lock_ = new object();
		[ThreadStatic]
		static WeakReference oldWR;
		[ThreadStatic]
		static WeakReference currentWR;
		[ThreadStatic]
		static WeakReference savedWR;
		public static IInputElement SavedFocus {
			get { return (IInputElement)savedWR.With(x => x.Target); }
			private set { savedWR = new WeakReference(value); }
		}
		public static IInputElement CurrentFocus {
			get { return (IInputElement)currentWR.With(x => x.Target); }
			private set { currentWR = new WeakReference(value); }
		}
		public static IInputElement PreviousFocus {
			get { return (IInputElement)oldWR.With(x => x.Target); }
			private set { oldWR = new WeakReference(value); }
		}
		static bool isInitialized = false;
		public static void Initialize() {
			if (isInitialized)
				return;
			lock (lock_)
			{
				if (isInitialized)
					return;
				isInitialized = true;
				CurrentFocus = Keyboard.FocusedElement;
				EventManager.RegisterClassHandler(typeof(UIElement), Keyboard.GotKeyboardFocusEvent, new KeyboardFocusChangedEventHandler(OnKeyboardFocusChanged));
				EventManager.RegisterClassHandler(typeof(UIElement), Keyboard.LostKeyboardFocusEvent, new KeyboardFocusChangedEventHandler(OnKeyboardFocusChanged));
			}
		}
		static FocusObserver() {
			Initialize();
		}
		static void OnKeyboardFocusChanged(object sender, KeyboardFocusChangedEventArgs e) {
			if (e.OldFocus == CurrentFocus)
				AddRecord(e.NewFocus);
		}
		public static event ValueChangedEventHandler<IInputElement> FocusChanged;
		static void RaiseFocusChanged(IInputElement oldElement, IInputElement newElement) {
			if (FocusChanged == null)
				return;
			FocusChanged(null, new ValueChangedEventArgs<IInputElement>(oldElement, newElement));
		}
#if DEBUGTEST
		static bool? trackFocus;
		static bool TrackFocus { get { return trackFocus ?? (trackFocus = String.Equals(Environment.GetEnvironmentVariable("TrackFocus", EnvironmentVariableTarget.Machine), "1")).Value; } }
#endif
		static void AddRecord(IInputElement focusedElement) {
#if DEBUGTEST
			if (TrackFocus)
				System.Diagnostics.Debug.WriteLine("{0} ({1})", focusedElement, focusedElement.Return(x => x.GetHashCode(), () => -1));
#endif
			PreviousFocus = CurrentFocus;
			CurrentFocus = focusedElement;
			if (PreviousFocus != CurrentFocus)
				RaiseFocusChanged(PreviousFocus, CurrentFocus);
		}
		public static bool SaveFocus(bool current) {
			if (SavedFocus != null)
				return false;
			SavedFocus = current ? CurrentFocus : PreviousFocus;
			return SavedFocus != null;
		}
		static DispatcherOperation restoreOperation = null;
		public static bool RestoreFocus(bool restoreAsync, Func<bool> condition = null) {
			if (restoreAsync && SavedFocus is DispatcherObject) {
				restoreOperation = (SavedFocus as DispatcherObject)
					.Dispatcher
					.BeginInvoke(new Action(() => { RestoreFocus(false, condition); restoreOperation = null; }), DispatcherPriority.Background);
				return true;
			}
			try {
				if (SavedFocus == null)
					return false;
				if (condition == null || condition())
					return SavedFocus.Focus();
				else
					return false;
			} finally {
				Reset();
			}
		}
		public static void CancelRestore() {
			restoreOperation.Do(x => x.Abort());
			restoreOperation = null;
		}
		public static void Reset() {
			SavedFocus = null;
		}
	}
	public static class MenuModeHelper {
		public static readonly RoutedEvent EnterMenuModeEvent = EventManager.RegisterRoutedEvent("EnterMenuMode", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MenuModeHelper));
		public static readonly RoutedEvent PreviewEnterMenuModeEvent = EventManager.RegisterRoutedEvent("PreviewEnterMenuMode", RoutingStrategy.Tunnel, typeof(RoutedEventHandler), typeof(MenuModeHelper));
		static Delegate onEnterMenuModeDelegate = null;
		static WeakList<Func<object, EventArgs, bool>> enterMenuModeWeakList = new WeakList<Func<object, EventArgs, bool>>();
		static Action<DependencyObject, bool> enableKeyboardCues;
		static MenuModeHelper() {
			PropertyInfo info = typeof(KeyboardNavigation).GetProperty("Current", BindingFlags.NonPublic | BindingFlags.Static);
			KeyboardNavigation current = (KeyboardNavigation)info.GetValue(null, null);
			EventInfo eventInfo = typeof(KeyboardNavigation).GetEvent("EnterMenuMode", BindingFlags.NonPublic | BindingFlags.Instance);
			MethodInfo mi = eventInfo.GetAddMethod(true);
			Type tDelegate = eventInfo.EventHandlerType;
			if (onEnterMenuModeDelegate == null)
				onEnterMenuModeDelegate = Delegate.CreateDelegate(tDelegate, typeof(MenuModeHelper).GetMethod("OnEnterMenuMode", BindingFlags.Static | BindingFlags.NonPublic));
			mi.Invoke(current, new object[] { onEnterMenuModeDelegate });
		}
		public static event Func<object, EventArgs, bool> EnterMenuMode {
			add { enterMenuModeWeakList.Add(value); }
			remove { enterMenuModeWeakList.Remove(value); }
		}
		public static void EnableKeyboardCues(DependencyObject element, bool enable) {
			(enableKeyboardCues ?? (enableKeyboardCues = ReflectionHelper.CreateInstanceMethodHandler<KeyboardNavigation, Action<DependencyObject, bool>>(null, "EnableKeyboardCues", BindingFlags.NonPublic | BindingFlags.Static)))(element, enable);
		}
		static bool OnEnterMenuMode(object sender, EventArgs e) {
			if (RaiseRoutedEvents())
				return true;
			try {
				enterMenuModeWeakList.LockReferences();   
				foreach (var value in enterMenuModeWeakList.ToList()) {
					if (value == null)
						continue;
					if (value(sender, e))
						return true;
				}
				return false;
			} finally {
				enterMenuModeWeakList.UnlockReferences();
			}
		}
		static bool RaiseRoutedEvents() {
			var focused = Keyboard.FocusedElement;
			if (focused == null)
				return false;
			var previewArgs = new RoutedEventArgs(PreviewEnterMenuModeEvent, focused);
			if (RaiseEvent(focused, previewArgs))
				return true;
			var args = new RoutedEventArgs(EnterMenuModeEvent, focused);
			if (RaiseEvent(focused, args))
				return true;
			return false;
		}
		static bool RaiseEvent(IInputElement target, RoutedEventArgs args) {
			var uie = target as UIElement;
			if (uie != null)
				uie.RaiseEvent(args);
			var ce = target as ContentElement;
			if (ce != null)
				ce.RaiseEvent(args);
			return args.Handled;
		}
	}
}
