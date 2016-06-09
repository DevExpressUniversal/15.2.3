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
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
namespace DevExpress.XtraTests {
	[CLSCompliant(false)]
	public struct KeyAction {
		public bool alt;
		public bool ctrl;
		public bool shift;
		public bool isChar;
		public bool isSealedKey;
		public bool sealedKeyState;
		public uint vCode;
		public char character;
	}
	[CLSCompliant(false)]
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
					if (!TestImports.IsSealedKey(key)) {
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
			keyAction.alt = Alt;
			keyAction.ctrl = Ctrl;
			keyAction.shift = Shift;
			keyAction.isSealedKey = false;
			keyAction.sealedKeyState = false;
			return keyAction;
		}
		protected KeyAction CreateKeyAction(Keys key) {
			KeyAction keyAction = new KeyAction();
			keyAction.isChar = false;
			keyAction.vCode = TestImports.GetKeyValue(key);
			keyAction.character = '\0';
			return FillModifierState(keyAction);
		}
		protected KeyAction CreateKeyAction(char key) {
			KeyAction keyAction = new KeyAction();
			keyAction.isChar = true;
			keyAction.vCode = key.ToString().ToUpper()[0];
			keyAction.character = key;
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
			InputKeyTemplate ikt = 0;
			if(Enum.TryParse<InputKeyTemplate>(tabKey, true, out ikt)) {
				key = (Keys)ikt;
				keys = keys.Remove(0, closedBracket + 1);
			} else {
				foreach(Keys i in Enum.GetValues(typeof(Keys))) {
					if(i.ToString().ToUpper() == tabKey) {
						key = i;
						keys = keys.Remove(0, closedBracket + 1);
						return key;
					}
				}
			}
			return key;
		}
		protected void UpdateModifiersState(Keys key, bool state) {
			switch (key) {
				case Keys.Alt:
					Alt = state;
					break;
				case Keys.Control:
					Ctrl = state;
					break;
				case Keys.Shift:
					Shift = state;
					break;
			}
		}
		protected virtual void InputKey(Keys key, ArrayList sealedKeyPressed) {
			if (TestImports.IsSealedKey(key) && sealedKeyPressed.IndexOf(key) < 0) {
				PressedSealedKey(key, ref sealedKeyPressed);
			} else {
				KeyActions.Add(CreateKeyAction(key));
			}
		}
		protected void PressedSealedKey(Keys key, ref ArrayList sealedKeyPressed) {
			UpdateModifiersState(key, true);
			KeyAction keyAction = CreateKeyAction(key);
			sealedKeyPressed.Add(key);
			keyAction.isSealedKey = true;
			keyAction.sealedKeyState = true;
			KeyActions.Add(keyAction);
		}
		protected void UnPressedSealedKey(Keys key) {
			UpdateModifiersState(key, false);
			KeyAction keyAction = CreateKeyAction(key);
			keyAction.isSealedKey = true;
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
 	[System.Security.SecuritySafeCritical]
		protected void ParseCharKey(char ch, ref ArrayList sealedKeyPressed) {
			const int VKeyCodeMask = 255;
			const int ShiftModifierMask = 256;
			const int CtrlModifierMask = 512;
			const int AltModifierMask = 2048;
			uint code = TestImports.VkKeyScan(ch);
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
}
