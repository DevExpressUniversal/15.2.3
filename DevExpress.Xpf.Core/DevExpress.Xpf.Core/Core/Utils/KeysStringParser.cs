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
using System.Collections;
using System.Text.RegularExpressions;
using System;
using DevExpress.Utils;
#if SL
using DevExpress.Data;
using DevExpress.Xpf.Collections;
#else
using System.Windows.Forms;
#endif
namespace DevExpress.Xpf.Utils {
	public class KeyAction {
		public bool Alt { get; set; }
		public bool Ctrl { get; set; }
		public bool Shift { get; set; }
		public bool IsChar { get; set; }
		public bool IsSealedKey { get; set; }
		public bool SealedKeyState { get; set; }
		public int VCode { get; set; }
		public int Character { get; set; }
		public Keys Key {
			get {
#if SL
				return new Keys(VCode);
#else
				return (Keys)VCode;
#endif
			}
		}
		public char Char { get { return (char)Character; } }
	}
	public class KeysStringParser {
		string keys;
		bool alt, ctrl, shift;
		List<KeyAction> keyActions;
		public KeysStringParser(string keys) {
			this.keys = keys;
			this.alt = false;
			this.ctrl = false;
			this.shift = false;
			this.keyActions = new List<KeyAction>();
		}
		protected bool Alt { get { return alt; } set { alt = value; } }
		protected bool Ctrl { get { return ctrl; } set { ctrl = value; } }
		protected bool Shift { get { return shift; } set { shift = value; } }
		protected string KeysString { get { return keys; } set { keys = value; } }
		protected List<KeyAction> KeyActions { get { return keyActions; } }
		public List<KeyAction> Parse() {
			Parse(KeysString);
			return KeyActions;
		}
		protected void Parse(string keys) {
			Keys key;
			ArrayList sealedKeyPressed = new ArrayList();
			Regex regexEvaluator = new Regex(@"^\((.+?)\)");
			while (keys != string.Empty) {
				if (HasKey(ref keys, out key)) {
					InputKey(key, sealedKeyPressed);
					if (!Helper.IsSealedKey(key)) {
						UnPressedSealedKeys(ref sealedKeyPressed);
					}
				}  else {
					if (sealedKeyPressed.Count != 0 && regexEvaluator.IsMatch(keys)) {
						Parse(regexEvaluator.Match(keys).Result("$1"));
						keys = regexEvaluator.Replace(keys, "");
					} else {
						ParseCharKey(keys[0], ref sealedKeyPressed);
						keys = keys.Remove(0, 1);
					}
					UnPressedSealedKeys(ref sealedKeyPressed);
				}
			}
			UnPressedSealedKeys(ref sealedKeyPressed);
		}
		protected KeyAction FillModifierState(KeyAction keyAction) {
			keyAction.Alt = Alt;
			keyAction.Ctrl = Ctrl;
			keyAction.Shift = Shift;
			keyAction.IsSealedKey = false;
			keyAction.SealedKeyState = false;
			return keyAction;
		}
		protected KeyAction CreateKeyAction(Keys key) {
			KeyAction keyAction = new KeyAction();
			keyAction.IsChar = false;
			keyAction.VCode = Helper.GetKeyValue(key);
			keyAction.Character = '\0';
			return FillModifierState(keyAction);
		}
		protected KeyAction CreateKeyAction(char key) {
			KeyAction keyAction = new KeyAction();
			keyAction.IsChar = true;
			keyAction.VCode = key.ToString().ToUpper()[0];
			keyAction.Character = key;
			return FillModifierState(keyAction);
		}
		protected bool HasKey(ref string keys, out Keys key) {
			key = Keys.None;
			if (keys == string.Empty) return false;
			key = GetNonEclosedKey(ref keys);
			if (key != Keys.None) return true;
			if (GetEnclosedChar(ref keys)) return false;
			key = GetEnclosedKey(ref keys);
			if (key != Keys.None) return true;
			return false;
		}
		protected Keys GetNonEclosedKey(ref string keys) {
			Keys key = Keys.None;
			switch (keys[0]) {
				case '+':
					key = Keys.Shift;
					break;
				case '^':
					key = Keys.Control;
					break;
				case '%':
					key = Keys.Alt;
					break;
				case '~':
					key = Keys.Enter;
					break;
			}
			if (key != Keys.None) {
				keys = keys.Remove(0, 1);
			}
			return key;
		}
		protected bool GetEnclosedChar(ref string keys) {
			Regex regexEvaluator = new Regex(@"^{([\+\[\]\^%~{}])}");
			if (regexEvaluator.IsMatch(keys)) {
				keys = regexEvaluator.Replace(keys, "$1");
				return true;
			}
			return false;
		}
		protected Keys GetEnclosedKey(ref string keys) {
			Keys key = Keys.None;
			if (keys[0] != '[' && keys[0] != '{') return key;
			char expCloseSymbol = keys[0] == '[' ? ']' : '}';
			int closedBracket = keys.IndexOf(expCloseSymbol);
			int openBracket = keys.IndexOf(keys[0], 1);
			if ((closedBracket < 0) || ((openBracket > 0) && (closedBracket > openBracket)))
				return key;
			string tabKey = keys.Substring(1, closedBracket - 1);
			if (tabKey == string.Empty) return key;
			tabKey = tabKey.ToUpper();
			try {
				key = (Keys)Enum.Parse(typeof(InputKeyTemplate), tabKey, true);
				keys = keys.Remove(0, closedBracket + 1);
			} catch {
				foreach (Keys i in EnumExtensions.GetValues(typeof(Keys))) {
					if (i.ToString().ToUpper() == tabKey) {
						key = i;
						keys = keys.Remove(0, closedBracket + 1);
						return key;
					}
				}
			}
			return key;
		}
		protected void UpdateModifiersState(Keys key, bool state) {
			if(key == Keys.Alt) {
				Alt = state;
				return;
			}
			if(key == Keys.Control) {
				Ctrl = state;
				return;
			}
			if(key == Keys.Shift) {
				Shift = state;
				return;
			}
		}
		protected virtual void InputKey(Keys key, ArrayList sealedKeyPressed) {
			if (Helper.IsSealedKey(key) && sealedKeyPressed.IndexOf(key) < 0) {
				PressedSealedKey(key, ref sealedKeyPressed);
			} else {
				KeyActions.Add(CreateKeyAction(key));
			}
		}
		protected void PressedSealedKey(Keys key, ref ArrayList sealedKeyPressed) {
			UpdateModifiersState(key, true);
			KeyAction keyAction = CreateKeyAction(key);
			sealedKeyPressed.Add(key);
			keyAction.IsSealedKey = true;
			keyAction.SealedKeyState = true;
			KeyActions.Add(keyAction);
		}
		protected void UnPressedSealedKey(Keys key) {
			UpdateModifiersState(key, false);
			KeyAction keyAction = CreateKeyAction(key);
			keyAction.IsSealedKey = true;
			KeyActions.Add(keyAction);
		}
		protected void UnPressedSealedKey(Keys key, ref ArrayList sealedKeyPressed) {
			UnPressedSealedKey(key);
			int indexOfKey = sealedKeyPressed.IndexOf(key);
			while (indexOfKey > -1) {
				sealedKeyPressed.RemoveAt(indexOfKey);
				indexOfKey = sealedKeyPressed.IndexOf(key);
			}
		}
		protected void UnPressedSealedKeys(ref ArrayList sealedKeyPressed) {
			foreach (Keys key in sealedKeyPressed) {
				UnPressedSealedKey(key);
			}
			sealedKeyPressed.Clear();
		}
		protected void ParseCharKey(char ch, ref ArrayList sealedKeyPressed) {
			const int VKeyCodeMask = 255;
			const int ShiftModifierMask = 256;
			const int CtrlModifierMask = 512;
			const int AltModifierMask = 2048;
			uint code = Helper.VkKeyScan(ch);
			List<Keys> sealedKeys = new List<Keys>();
			if (!Alt && ((code & AltModifierMask) != 0)) sealedKeys.Add(Keys.Alt);
			if (!Ctrl && ((code & CtrlModifierMask) != 0)) sealedKeys.Add(Keys.Control);
			if (!Shift && ((code & ShiftModifierMask) != 0)) sealedKeys.Add(Keys.Shift);
			foreach (Keys key in sealedKeys) {
				PressedSealedKey(key, ref sealedKeyPressed);
			}
			KeyActions.Add(CreateKeyAction((char)(code & VKeyCodeMask)));
			foreach (Keys key in sealedKeys) {
				UnPressedSealedKey(key, ref sealedKeyPressed);
			}
		}
	}
	class Helper {
		internal static uint VkKeyScan(char ch) {
			return ch;
		}
		internal static bool IsSealedKey(Keys key) {
			throw new NotImplementedException();
		}
		internal static int GetKeyValue(Keys key) {
			throw new NotImplementedException();
		}
	}
	class InputKeyTemplate {
	}
}
