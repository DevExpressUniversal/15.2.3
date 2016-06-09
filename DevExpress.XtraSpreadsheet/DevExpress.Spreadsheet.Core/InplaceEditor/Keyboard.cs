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
using DevExpress.Utils.Commands;
using DevExpress.Utils.KeyboardHandler;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Keyboard;
using DevExpress.XtraSpreadsheet.Utils;
using System.Windows.Forms;
using DevExpress.Compatibility.System.Windows.Forms;
namespace DevExpress.XtraSpreadsheet.Internal {
	#region CellInplaceEditorKeyboardHandler
	public class CellInplaceEditorKeyboardHandler : CommandBasedKeyboardHandler<SpreadsheetCommandId> {
		readonly List<Keys> forceHandleKeyAgainShortcuts = new List<Keys>();
		public virtual InnerSpreadsheetControl Control { get { return (InnerSpreadsheetControl)Context; } set { Context = value; } }
		public List<Keys> ForceHandleKeyAgainShortcuts { get { return forceHandleKeyAgainShortcuts; } }
		public override Command CreateHandlerCommand(SpreadsheetCommandId id) {
			return Control.Owner.CreateCommand(id);
		}
		protected override IKeyHashProvider CreateKeyHashProviderFromContext() {
			return new SpreadsheetKeyHashProvider();
		}
		protected override void ValidateHandlerId(SpreadsheetCommandId commandId) {
			if (commandId == SpreadsheetCommandId.None)
				Exceptions.ThrowArgumentException("commandId", commandId);
		}
		protected override void ExecuteCommandCore(Command command, Keys keyData) {
			base.ExecuteCommandCore(command, keyData);
			InplaceEndEditCommand endEditCommand = command as InplaceEndEditCommand;
			if (endEditCommand != null) {
				this.handleKeyAgain = (Control.InplaceEditor.Mode == CellEditorMode.Enter) && endEditCommand.CommitSuccessfull;
				if (!this.handleKeyAgain && ForceHandleKeyAgainShortcuts.Contains(keyData))
					this.handleKeyAgain = endEditCommand.CommitSuccessfull;
			}
		}
		bool handleKeyAgain;
		SpreadsheetCursor previousCursor;
		public override bool HandleKey(Keys keyData) {
			this.handleKeyAgain = false;
			if (base.HandleKey(keyData)) {
				if (handleKeyAgain) { 
					Control.KeyboardHandler.Context = Control;
					Control.KeyboardHandler.HandleKey(keyData);
				}
				return true;
			}
			TryChangeCursor();
			return false;
		}
		protected internal virtual void TryChangeCursor() {
			ISpreadsheetControl control = Control.Owner;
			if (!(ShouldChangeCursor(SpreadsheetCursors.Hand) && control.IsHyperlinkActive()))
				return;
			previousCursor = new SpreadsheetCursor(GetCursor());
			SetCursor(SpreadsheetCursors.Hand);
		}
		protected internal virtual bool ShouldChangeCursor(SpreadsheetCursor cursor) {
			if (GetCursor() == cursor.Cursor)
				return false;
			return previousCursor == null || cursor.Cursor != previousCursor.Cursor;
		}
		public override bool HandleKeyUp(Keys keys) {
			bool result = base.HandleKeyUp(keys);
			RestorePreviousCursor();
			return result;
		}
		protected internal virtual void RestorePreviousCursor() {
			if (previousCursor != null) {
				SetCursor(previousCursor);
				previousCursor = null;
			}
		}
		void SetCursor(SpreadsheetCursor cursor) {
			Control.Owner.Cursor = cursor.Cursor;
		}
		Cursor GetCursor() {
			return Control.Owner.Cursor;
		}
		public void ReplaceAllCommandIdsWith(SpreadsheetCommandId commandId) {
			long[] keys = new long[KeyHandlerIdTable.Keys.Count];
			KeyHandlerIdTable.Keys.CopyTo(keys, 0);
			foreach (long key in keys)
				KeyHandlerIdTable[key] = commandId;
		}
		public void UpdateForceHandleKeyAgainShortcuts() {
			this.ForceHandleKeyAgainShortcuts.Clear();
			foreach (long key in KeyHandlerIdTable.Keys)
				this.ForceHandleKeyAgainShortcuts.Add(KeyDataToKeys(key));
		}
	}
	#endregion
	public partial class InnerCellInplaceEditor {
		public virtual bool OnKeyDown(KeyEventArgs e) {
			if (e.KeyCode == Keys.F4) {
#if !SL
				if (e.Modifiers == Keys.None)
#endif
					ModifyReferenceType();
				return true;
			}
			if (e.KeyCode == Keys.A) {
#if !SL
				if (e.Modifiers == Keys.Control)
#endif
					this.editor.SetSelection(0, this.editor.Text.Length);
			}
			return Control.OnKeyDown(e);
		}
		public virtual bool OnKeyUp(KeyEventArgs e) {
			return Control.OnKeyUp(e);
		}
		public virtual bool OnKeyPress(KeyPressEventArgs e) {
			return Control.OnKeyPress(e);
		}
	}
}
