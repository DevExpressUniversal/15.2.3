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
using System.Windows.Forms;
using DevExpress.Utils.Commands;
using DevExpress.Compatibility.System.Windows.Forms;
namespace DevExpress.Utils.KeyboardHandler {
	#region KeyState
	public enum KeyState {
		ShiftKey = 4,
		CtrlKey = 8,
		AltKey = 32
	}
	#endregion
	#region KeyboardHandler (abstract class)
	public abstract class KeyboardHandler {
		object context;
		public object Context { get { return context; } set { context = value; } }
		public virtual bool IsValidChar(char c) {
			return c >= 0x20;
		}
		public virtual bool HandleKey(Keys keyData) {
			return false;
		}
		public virtual bool HandleKeyPress(char character, Keys modifier) {
			return false;
		}
		public virtual bool HandleKeyUp(Keys keys) {
			return false;
		}
#if DXPORTABLE
		public static bool IsShiftPressed { get { return false; } }
		public static bool IsControlPressed { get { return false; } }
		public static bool IsAltPressed { get { return false; } }
		public static KeyState KeyState { get { return (KeyState)0; } }
#elif !SILVERLIGHT
		public static bool IsShiftPressed { get { return (System.Windows.Forms.Control.ModifierKeys & Keys.Shift) != 0; } }
		public static bool IsControlPressed { get { return (System.Windows.Forms.Control.ModifierKeys & Keys.Control) != 0; } }
		public static bool IsAltPressed { get { return (System.Windows.Forms.Control.ModifierKeys & Keys.Alt) != 0; } }
		public static KeyState KeyState {
			get {
				Keys keys = System.Windows.Forms.Control.ModifierKeys;
				KeyState keyState = (KeyState)0;
				if ((keys & Keys.Shift) != 0)
					keyState |= KeyState.ShiftKey;
				if ((keys & Keys.Control) != 0)
					keyState |= KeyState.CtrlKey;
				if ((keys & Keys.Alt) != 0)
					keyState |= KeyState.AltKey;
				return keyState;
			}
		}
#else
		public static bool IsShiftPressed { get { return KeyboardHelper.IsShiftPressed; } }
		public static bool IsControlPressed { get { return KeyboardHelper.IsControlPressed; } }
		public static bool IsAltPressed { get { return KeyboardHelper.IsAltPressed; } }
		public static KeyState KeyState {
			get {
				KeyState keyState = (KeyState)0;
				if(IsShiftPressed)
					keyState |= KeyState.ShiftKey;
				if(IsControlPressed)
					keyState |= KeyState.CtrlKey;
				if(IsAltPressed)
					keyState |= KeyState.AltKey;
				return keyState;
			}
		}
#endif
			public static Keys GetModifierKeys() {
			Keys result = Keys.None;
			if (IsAltPressed)
				result |= Keys.Alt;
			if (IsControlPressed)
				result |= Keys.Control;
			if (IsShiftPressed)
				result |= Keys.Shift;
			return result;
		}
	}
	#endregion
	#region EmptyKeyboardHandler
	public class EmptyKeyboardHandler : KeyboardHandler {
	}
	#endregion
	#region IKeyHashProvider
	public interface IKeyHashProvider {
		Int64 CreateHash(Int64 keyData);
	}
	#endregion
	#region CommandBasedKeyboardHandler<T> (abstract class)
	public abstract class CommandBasedKeyboardHandler<T> : KeyboardHandler {
		#region Fields
		Dictionary<Int64, T> keyHandlerIdTable = new Dictionary<Int64, T>();
		#endregion
		#region Properties
		public Dictionary<Int64, T> KeyHandlerIdTable { get { return keyHandlerIdTable; } }
		#endregion
		public override bool HandleKey(Keys keyData) {
			Command command = GetKeyHandler(keyData);
			if (command != null) {
#if SILVERLIGHT
				if (command.InnerShouldBeExecutedOnKeyUpInSilverlightEnvironment)
					return false;
#endif
				return ExecuteCommand(command, keyData);
			}
			else
				return false;
		}
#if SILVERLIGHT
		public override bool HandleKeyUp(Keys keyData) {
			Command command = GetKeyHandler(keyData);
			if (command != null) {
				if (!command.InnerShouldBeExecutedOnKeyUpInSilverlightEnvironment)
					return false;
				return ExecuteCommand(command, keyData);
			}
			else
				return false;
		}
#endif
		public override bool HandleKeyPress(char character, Keys modifier) {
			Command command = GetKeyHandler(character, modifier);
			return ExecuteCommand(command, modifier);
		}
		protected bool ExecuteCommand(Command command, Keys keyData) {
			if (command == null)
				return false;
			ExecuteCommandCore(command, keyData);
			return true;
		}
		protected virtual void ExecuteCommandCore(Command command, Keys keyData) {
			command.CommandSourceType = CommandSourceType.Keyboard;
			command.Execute();
		}
		public virtual void RegisterKeyHandler(IKeyHashProvider provider, Keys key, Keys modifier, T handlerId) {
			Int64 keyData = KeysToInt64KeyData(key | modifier);
			RegisterKeyHandlerCore(provider, keyData, handlerId);
		}
		protected internal virtual void RegisterKeyHandlerCore(IKeyHashProvider provider, Int64 keyData, T handlerId) {
			ValidateHandlerId(handlerId);
			Int64 hash = provider.CreateHash(keyData);
			keyHandlerIdTable.Add(hash, handlerId);
		}
		public virtual void UnregisterKeyHandler(IKeyHashProvider provider, Keys key, Keys modifier) {
			Int64 keyData = KeysToInt64KeyData(key | modifier);
			UnregisterKeyHandlerCore(provider, keyData);
		}
		protected internal virtual void UnregisterKeyHandlerCore(IKeyHashProvider provider, Int64 keyData) {
			Int64 hash = provider.CreateHash(keyData);
			if (keyHandlerIdTable.ContainsKey(hash))
				keyHandlerIdTable.Remove(hash);
		}
		public virtual T GetKeyHandlerId(Int64 keyData) {
			IKeyHashProvider provider = CreateKeyHashProviderFromContext();
			if (provider == null)
				return default(T);
			Int64 hash = provider.CreateHash(keyData);
			T result;
			if (keyHandlerIdTable.TryGetValue(hash, out result))
				return result;
			else
				return default(T);
		}
		public virtual Command GetKeyHandler(Keys keyData) {
			return GetKeyHandlerCore(KeysToInt64KeyData(keyData));
		}
		public virtual Command GetKeyHandler(char key, Keys modifier) {
			return GetKeyHandlerCore(CharToInt64KeyData(key, modifier));
		}
		protected internal virtual Command GetKeyHandlerCore(Int64 keyData) {
			T handlerId = GetKeyHandlerId(keyData);
			if (!Object.Equals(handlerId, default(T)))
				return CreateHandlerCommand(handlerId);
			return null;
		}
		public static Int64 KeysToInt64KeyData(Keys keys) {
			return ((Int64)keys) << 32; 
		}
		public static Keys KeyDataToKeys(Int64 keyData) {
			return (Keys)(keyData >> 32);
		}
		public static Int64 CharToInt64KeyData(char value, Keys modifier) {
			return (Int64)((int)modifier | (int)Char.ToUpper(value));
		}
		public virtual Keys GetKeys(T handlerId) {
			foreach (Int64 keyData in KeyHandlerIdTable.Keys) {
				T id = KeyHandlerIdTable[keyData];
				if (Object.Equals(handlerId, id))
					return KeyDataToKeys(keyData);
			}
			return Keys.None;
		}
		protected abstract void ValidateHandlerId(T handlerId);
		public abstract Command CreateHandlerCommand(T handlerId);
		protected abstract IKeyHashProvider CreateKeyHashProviderFromContext();
	}
	#endregion
}
