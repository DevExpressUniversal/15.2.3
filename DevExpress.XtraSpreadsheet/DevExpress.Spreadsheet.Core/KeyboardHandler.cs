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
using System.Reflection;
using DevExpress.Utils.Commands;
using DevExpress.Utils.KeyboardHandler;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Services;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Compatibility.System.Windows.Forms;
#if !SL
using System.Windows.Forms;
#else
using DevExpress.Data;
using System.Windows.Input;
#endif
namespace DevExpress.XtraSpreadsheet.Keyboard {
	#region SpreadsheetKeyHashProvider
	public class SpreadsheetKeyHashProvider : IKeyHashProvider {
		public SpreadsheetKeyHashProvider() {
		}
		#region IKeyHashProvider Members
		public Int64 CreateHash(Int64 keyData) {
			Int64 result = keyData; 
			return result;
		}
		#endregion
	}
	#endregion
	#region NormalKeyboardHandler
	public class NormalKeyboardHandler : CommandBasedKeyboardHandler<SpreadsheetCommandId> {
		public virtual InnerSpreadsheetControl Control { get { return (InnerSpreadsheetControl)Context; } set { Context = value; } }
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
		public override bool HandleKeyPress(char character, Keys modifier) {
			if (base.HandleKeyPress(character, modifier))
				return true;
			Comment selectedComment = GetSelectedComment();
			if (selectedComment != null)
				Control.ActivateCommentInplaceEditor(selectedComment.Reference);
			else
				Control.OpenInplaceEditor(character.ToString());
			return true;
		}
		Comment GetSelectedComment() {
			Worksheet activeSheet = Control.DocumentModel.ActiveSheet;
			SheetViewSelection selection = activeSheet.Selection;
			if (!selection.IsCommentSelected)
				return null;
			return activeSheet.Comments[selection.SelectedCommentIndex];
		}
		SpreadsheetCursor previousCursor;
		public override bool HandleKey(Keys keyData) {
			if (base.HandleKey(keyData))
				return true;
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
	}
	#endregion
	#region PictureKeyboardHandler
	public class PictureKeyboardHandler : NormalKeyboardHandler {
	}
	#endregion
	#region CommentKeyboardHandler
	public class CommentKeyboardHandler : NormalKeyboardHandler {
	}
	#endregion
	#region DataValidationKeyboardHandler
	public class DataValidationKeyboardHandler : NormalKeyboardHandler {
	}
	#endregion
}
