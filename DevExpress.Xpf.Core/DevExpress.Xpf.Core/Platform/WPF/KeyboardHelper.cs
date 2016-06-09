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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
#if SILVERLIGHT
using DevExpress.Data;
using PlatformIndependentKeyEventArgs = DevExpress.Data.KeyEventArgs;
#else
using System.Windows.Forms;
using PlatformIndependentKeyEventArgs = System.Windows.Forms.KeyEventArgs;
#endif
namespace DevExpress.Xpf.Utils {
	public static class KeyboardHelper {
		public static bool IsAltKey(Key key) {
			return KeyMapper.KeyToKeysValue(key) == (int)Keys.Alt;
		}
		public static bool IsControlKey(Key key) {
			return KeyMapper.KeyToKeysValue(key) == (int)Keys.Control;
		}
		public static bool IsShiftKey(Key key) {
			return KeyMapper.KeyToKeysValue(key) == (int)Keys.Shift;
		}
		public static bool IsAltPressed {
			get { return IsAltModifier(Keyboard.Modifiers); }
		}
		public static bool IsControlPressed {
			get { return IsControlModifier(Keyboard.Modifiers); }
		}
		public static bool IsShiftPressed {
			get { return IsShiftModifier(Keyboard.Modifiers); }
		}
		public static bool IsAltModifier(ModifierKeys modifiers) {
			return (modifiers & ModifierKeys.Alt) != 0;
		}
		public static bool IsControlModifier(ModifierKeys modifiers) {
#if SILVERLIGHT
			return (modifiers & (ModifierKeys.Control | ModifierKeys.Apple)) != 0;
#else
			return (modifiers & ModifierKeys.Control) != 0;
#endif
		}
		public static bool IsShiftModifier(ModifierKeys modifiers) {
			return (modifiers & ModifierKeys.Shift) != 0;
		}
		public static DependencyObject FocusedElement {
			get {
#if SILVERLIGHT
				return FocusManager.GetFocusedElement() as DependencyObject;
#else
				return Keyboard.FocusedElement as DependencyObject;
#endif                
			}
		}
#if SILVERLIGHT
		static string Buffer = "";
		public static void Add(char c) {
			lock (Buffer) {
				Buffer += c;
			}
		}
		public static string Read {
			get {
				lock (Buffer) {
					string result = Buffer;
					Buffer = string.Empty;
					return result;
				}
			}
		}
		public static void Focus(IInputElement element) { }
		public static bool Focus(UIElement element) {
			return (element is Control) && (element as Control).Focus();
		}
		public static int KeySystemCodeEnter { get { return DevExpress.Data.Keys.Enter.Value; } }
		public static int KeySystemCodeEscape { get { return DevExpress.Data.Keys.Escape.Value; } }
#else
		public static bool Focus(UIElement element) {
			return element.Focus();
		}
		public static void Focus(IInputElement element) {
			Keyboard.Focus(element);
		}
		public static int KeySystemCodeEnter { get { return (int)System.Windows.Forms.Keys.Enter; } }
		public static int KeySystemCodeEscape { get { return (int)System.Windows.Forms.Keys.Escape; } }
		public static int KeySystemCodeLineFeed { get { return (int)System.Windows.Forms.Keys.LineFeed; } }
#endif
	}
	public static class KeyMapper {
		static Dictionary<Key, Keys> keyTable;
		static KeyMapper() {
			InitKeyTable();
		}
		static void InitKeyTable() {
			keyTable = new Dictionary<Key, Keys>();
			keyTable[Key.None] = Keys.None;
			keyTable[Key.Back] = Keys.Back;
			keyTable[Key.Tab] = Keys.Tab;
			keyTable[Key.Enter] = Keys.Enter;
			keyTable[Key.Escape] = Keys.Escape;
			keyTable[Key.Space] = Keys.Space;
			keyTable[Key.PageUp] = Keys.PageUp;
			keyTable[Key.PageDown] = Keys.PageDown;
			keyTable[Key.End] = Keys.End;
			keyTable[Key.Home] = Keys.Home;
			keyTable[Key.Left] = Keys.Left;
			keyTable[Key.Up] = Keys.Up;
			keyTable[Key.Right] = Keys.Right;
			keyTable[Key.Down] = Keys.Down;
			keyTable[Key.Insert] = Keys.Insert;
			keyTable[Key.Delete] = Keys.Delete;
			keyTable[Key.D0] = Keys.D0;
			keyTable[Key.D1] = Keys.D1;
			keyTable[Key.D2] = Keys.D2;
			keyTable[Key.D3] = Keys.D3;
			keyTable[Key.D4] = Keys.D4;
			keyTable[Key.D5] = Keys.D5;
			keyTable[Key.D6] = Keys.D6;
			keyTable[Key.D7] = Keys.D7;
			keyTable[Key.D8] = Keys.D8;
			keyTable[Key.D9] = Keys.D9;
			keyTable[Key.A] = Keys.A;
			keyTable[Key.B] = Keys.B;
			keyTable[Key.C] = Keys.C;
			keyTable[Key.D] = Keys.D;
			keyTable[Key.E] = Keys.E;
			keyTable[Key.F] = Keys.F;
			keyTable[Key.G] = Keys.G;
			keyTable[Key.H] = Keys.H;
			keyTable[Key.I] = Keys.I;
			keyTable[Key.J] = Keys.J;
			keyTable[Key.K] = Keys.K;
			keyTable[Key.L] = Keys.L;
			keyTable[Key.M] = Keys.M;
			keyTable[Key.N] = Keys.N;
			keyTable[Key.O] = Keys.O;
			keyTable[Key.P] = Keys.P;
			keyTable[Key.Q] = Keys.Q;
			keyTable[Key.R] = Keys.R;
			keyTable[Key.S] = Keys.S;
			keyTable[Key.T] = Keys.T;
			keyTable[Key.U] = Keys.U;
			keyTable[Key.V] = Keys.V;
			keyTable[Key.W] = Keys.W;
			keyTable[Key.X] = Keys.X;
			keyTable[Key.Y] = Keys.Y;
			keyTable[Key.Z] = Keys.Z;
			keyTable[Key.F1] = Keys.F1;
			keyTable[Key.F2] = Keys.F2;
			keyTable[Key.F3] = Keys.F3;
			keyTable[Key.F4] = Keys.F4;
			keyTable[Key.F5] = Keys.F5;
			keyTable[Key.F6] = Keys.F6;
			keyTable[Key.F7] = Keys.F7;
			keyTable[Key.F8] = Keys.F8;
			keyTable[Key.F9] = Keys.F9;
			keyTable[Key.F10] = Keys.F10;
			keyTable[Key.F11] = Keys.F11;
			keyTable[Key.F12] = Keys.F12;
			keyTable[Key.NumPad0] = Keys.NumPad0;
			keyTable[Key.NumPad1] = Keys.NumPad1;
			keyTable[Key.NumPad2] = Keys.NumPad2;
			keyTable[Key.NumPad3] = Keys.NumPad3;
			keyTable[Key.NumPad4] = Keys.NumPad4;
			keyTable[Key.NumPad5] = Keys.NumPad5;
			keyTable[Key.NumPad6] = Keys.NumPad6;
			keyTable[Key.NumPad7] = Keys.NumPad7;
			keyTable[Key.NumPad8] = Keys.NumPad8;
			keyTable[Key.NumPad9] = Keys.NumPad9;
			keyTable[Key.Multiply] = Keys.Multiply;
			keyTable[Key.Add] = Keys.Add;
			keyTable[Key.Subtract] = Keys.Subtract;
			keyTable[Key.Decimal] = Keys.Decimal;
			keyTable[Key.Divide] = Keys.Divide;
			keyTable[Key.CapsLock] = Keys.CapsLock;
#if SILVERLIGHT
			keyTable[Key.Shift] = Keys.Shift;
			keyTable[Key.Ctrl] = Keys.Control;
			keyTable[Key.Alt] = Keys.Alt;
			keyTable[Key.Unknown] = Keys.Unknown;
#else
			keyTable[Key.LeftShift] = Keys.Shift;
			keyTable[Key.RightShift] = Keys.Shift;
			keyTable[Key.LeftCtrl] = Keys.Control;
			keyTable[Key.RightCtrl] = Keys.Control;
			keyTable[Key.LeftAlt] = Keys.Alt;
			keyTable[Key.RightAlt] = Keys.Alt;
#endif
		}
		public static int KeyToKeysValue(Key k) {
			Keys res;
			if (!keyTable.TryGetValue(k, out res)) {
				return (int)k << 8;
			}
			else {
				return GetValue(res);
			}
		}
		internal static int GetValue(Keys key) {
#if SILVERLIGHT
			return key.Value;
#else
			return (int)key;
#endif
		}
	}
	public static class KeyEventArgsExtensions {
		public static PlatformIndependentKeyEventArgs ToPlatformIndependent(this System.Windows.Input.KeyEventArgs e) {
			int keyValue = KeyMapper.KeyToKeysValue(e.Key) | GetModKeys();
#if SILVERLIGHT
			return new PlatformIndependentKeyEventArgs(new Keys(keyValue));
#else
			return new PlatformIndependentKeyEventArgs((Keys)keyValue);
#endif
		}
		static int GetModKeys() {
			int res = 0;
			if (KeyboardHelper.IsShiftPressed) res |= KeyMapper.GetValue(Keys.Shift);
			if (KeyboardHelper.IsControlPressed) res |= KeyMapper.GetValue(Keys.Control);
			if (KeyboardHelper.IsAltPressed) res |= KeyMapper.GetValue(Keys.Alt);
			return res;
		}
	}
}
