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
using System.Windows.Input;
using DevExpress.Utils;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.XtraPrinting;
using DevExpress.Mvvm;
namespace DevExpress.Xpf.Printing {
#if SL
	public enum MouseButton { Left, Middle, Right };
#endif
	public abstract class InputController {
		readonly Dictionary<InputShortcut, ICommand> shortcuts;
		readonly List<ModifierKeys> pressedModifiers;
		Key pressedKey;
		MouseInputAction currentMouseAction;
		MouseWheelScrollingDirection scrollingDirection;
		IPreviewModel model;
		public InputController() {
			pressedKey = Key.None;
			pressedModifiers = new List<ModifierKeys>();
			currentMouseAction = MouseInputAction.None;
			scrollingDirection = MouseWheelScrollingDirection.None;
			shortcuts = new Dictionary<InputShortcut, ICommand>();
		}
		public IPreviewModel Model {
			get { return model; }
			set {
				if(model == value)
					return;
				model = value;
				if(model != null) {
					shortcuts.Clear();
					CreateShortcuts();
				}
			}
		}
		public static bool AreModifiersPressed {
			get { return !ModifierKeysHelper.NoModifiers(Keyboard.Modifiers); }
		}
		public void HandleKeyDown(Key key) {
			if(key == Key.None)
				return;
			if(!IsModifierKey(key)) {
#if !SL
				key = Convert(key);
#endif
				pressedKey = key;
				FillPressedModifiers();
				ExecuteShortcut(new KeyShortcut(pressedModifiers.ToArray(), pressedKey));
			}
		}
		public void HandleMouseDown(MouseButton mouseButton) {
			switch(mouseButton) {
				case MouseButton.Left:
					currentMouseAction = MouseInputAction.LeftClick;
					break;
				case MouseButton.Middle:
					currentMouseAction = MouseInputAction.MiddleClick;
					break;
				case MouseButton.Right:
					currentMouseAction = MouseInputAction.RightClick;
					break;
				default:
					break;
			}
			FillPressedModifiers();
			ExecuteShortcut(new MouseShortcut(pressedModifiers.ToArray(), currentMouseAction, MouseWheelScrollingDirection.None));
		}
		public void HandleMouseWheel(int delta) {
			scrollingDirection = MouseWheelScrollingDirection.None;
			if(delta > 0)
				scrollingDirection = MouseWheelScrollingDirection.Up;
			if(delta < 0)
				scrollingDirection = MouseWheelScrollingDirection.Down;
			FillPressedModifiers();
			ExecuteShortcut(new MouseShortcut(pressedModifiers.ToArray(), MouseInputAction.WheelClick, scrollingDirection));
		}
#if !SL
		static Key Convert(Key key) {
			if(key == Key.Prior)
				return Key.PageUp;
			if(key == Key.Next)
				return Key.PageDown;
			return key;
		}
#endif
		static bool IsModifierKey(Key key) {
			switch(key) {
#if !SL
				case (Key.LeftCtrl):
				case (Key.RightCtrl):
				case (Key.LeftAlt):
				case (Key.RightAlt):
				case (Key.LeftShift):
				case (Key.RightShift):
				case (Key.LWin):
				case (Key.RWin):
#else
				case (Key.Ctrl):
				case (Key.Shift):
				case (Key.Alt):
#endif
					return true;
				default:
					return false;
			}
		}
		protected abstract void CreateShortcuts();
		protected internal void TryAddShortcutForPSCommand(InputShortcut shortcut, PrintingSystemCommand psCommand) {
			PreviewModelBase previewModel = Model as PreviewModelBase;
			if(previewModel == null)
				return;
			DelegateCommand<object> command;
			if(previewModel.Commands.TryGetValue(psCommand, out command))
				AddShortcut(shortcut, command);
		}
		protected void AddShortcut(InputShortcut shortcut, ICommand command) {
			Guard.ArgumentNotNull(shortcut, "shortcut");
			Guard.ArgumentNotNull(command, "command");
			if(shortcuts.ContainsKey(shortcut))
				throw new ArgumentException("This shortcut has already been added");
			shortcuts.Add(shortcut, command);
		}
		void FillPressedModifiers() {
			pressedModifiers.Clear();
			ModifierKeys keyboardModifiers = Keyboard.Modifiers;
			if(!AreModifiersPressed)
				return;
			if(ModifierKeysHelper.IsAltPressed(keyboardModifiers))
				pressedModifiers.Add(ModifierKeys.Alt);
			if(ModifierKeysHelper.IsCtrlPressed(keyboardModifiers))
				pressedModifiers.Add(ModifierKeys.Control);
			if(ModifierKeysHelper.IsShiftPressed(keyboardModifiers))
				pressedModifiers.Add(ModifierKeys.Shift);
#if !SL
			if(ModifierKeysHelper.IsWinPressed(keyboardModifiers))
				pressedModifiers.Add(ModifierKeys.Windows);
#endif
		}
		void ExecuteShortcut(InputShortcut shortcut) {
			ICommand command;
			if(shortcuts.TryGetValue(shortcut, out command) && command.CanExecute(null))
				command.Execute(null);
		}
	}
}
