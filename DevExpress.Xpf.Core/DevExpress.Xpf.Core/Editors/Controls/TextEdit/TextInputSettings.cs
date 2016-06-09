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

using System.Windows.Input;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Editors {
	public class TextInputSettings : TextInputSettingsBase {
		protected internal override void InsertText(string value) {
			base.InsertText(value);
			EditBox.SelectedText = value;
		}
		protected internal override bool NeedsKey(Key key, ModifierKeys modifiers) {
			if (key == Key.PageDown || key == Key.PageUp)
				return false;
			if (key == Key.Left || key == Key.Home)
				return NeedsNavigateKeyLeftRight(key, modifiers, () => EditBox.NeedsKey(key, modifiers));
			if (key == Key.Right || key == Key.End)
				return NeedsNavigateKeyLeftRight(key, modifiers, () => EditBox.NeedsKey(key, modifiers));
			if (key == Key.Up || key == Key.Down)
				return NeedsNavigateUpDown(key, modifiers, () => EditBox.NeedsKey(key, modifiers));
			return EditBox.NeedsKey(key, modifiers);
		}
		protected internal override void Cut() {
			if (CanCut())
				EditBox.Cut();
		}
		protected internal override void Undo() {
			if (CanUndo())
				EditBox.Undo();
		}
		protected internal override bool CanUndo() {
			return EditBox.CanUndo;
		}
		protected internal override void Paste() {
			if (CanPaste())
				EditBox.Paste();
		}
		protected internal override void Delete() {
			if (CanDelete())
				EditBox.SelectedText = string.Empty;
		}
		protected internal override void PerformLostFocus() {
			base.PerformLostFocus();
			EditStrategy.UpdateDisplayText();
		}
		protected internal override void PerformGotFocus() {
			base.PerformGotFocus();
			EditStrategy.UpdateDisplayText();
		}
		protected internal override bool GetAcceptsReturn() {
			return OwnerEdit.AcceptsReturn;
		}
		protected internal override void UpdateIme() {
			base.UpdateIme();
			OwnerEdit.Do(x => EditBox.IsImeEnabled(true));
		}
		public TextInputSettings(TextEditBase editor) : base(editor) {
		}
	}
}
