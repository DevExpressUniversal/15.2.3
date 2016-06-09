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
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Services;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Compatibility.System.Windows.Forms;
#if !SL
using System.Windows.Forms;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Services.Implementation;
#else
using DevExpress.Data;
using System.Windows.Input;
#endif
namespace DevExpress.XtraRichEdit.Keyboard {
	#region RichEditKeyHashProvider
	public class RichEditKeyHashProvider : IKeyHashProvider {
		#region Fields
		RichEditViewType viewType;
		#endregion
		public RichEditKeyHashProvider(RichEditViewType viewType) {
			this.viewType = viewType;
		}
		#region Properties
		public RichEditViewType ViewType { get { return viewType; } set { viewType = value; } }
		#endregion
		#region IKeyHashProvider Members
		public Int64 CreateHash(Int64 keyData) {
			Int64 result = keyData; 
			result |= (((Int64)viewType) << (32 + 22));
			return result;
		}
		#endregion
		public static RichEditViewType GetViewTypeFromHash(Int64 keyData) {
			var res = keyData >> (32 + 22);
			return (RichEditViewType)res;
		}
		public static Keys DeleteViewTypeInfoFromKeys(Keys keys) {
			Int64 res = (Int64)keys;
			res &= 0x3FFFFF;
			return (Keys)res;
		}
	}
	#endregion
	#region NormalKeyboardHandler
	public class NormalKeyboardHandler : CommandBasedKeyboardHandler<RichEditCommandId> {
		int beginProcessMultipleKeyPressCount;
		public virtual RichEditView View { get { return (RichEditView)Context; } set { Context = value; } }
		public override Command CreateHandlerCommand(RichEditCommandId id) {
			return View.Control.InnerControl.CreateCommand(id);
		}
		protected override IKeyHashProvider CreateKeyHashProviderFromContext() {
			return new RichEditKeyHashProvider(View.Type);
		}
		protected override void ValidateHandlerId(RichEditCommandId commandId) {
			if (commandId == RichEditCommandId.None)
				Exceptions.ThrowArgumentException("commandId", commandId);
		}
		public override bool HandleKeyPress(char character, Keys modifier) {
			if (base.HandleKeyPress(character, modifier))
				return true;
			{
				text += character;
				FlushPendingTextInputIfNeeded();
				return true;
			}
		}
		protected void FlushPendingTextInputIfNeeded() {
			TimeSpan span = DateTime.Now - lastFlushTime;
			if (span > TimeSpan.FromMilliseconds(PendingTextFlushMilliseconds) && beginProcessMultipleKeyPressCount == 0)
				FlushPendingTextInput();
		}
		public virtual void BeginProcessMultipleKeyPress() {
			beginProcessMultipleKeyPressCount++;
		}
		public virtual void EndProcessMultipleKeyPress() {
			if(beginProcessMultipleKeyPressCount > 0)
				beginProcessMultipleKeyPressCount--;
			if (beginProcessMultipleKeyPressCount == 0)
				FlushPendingTextInputIfNeeded();
		}
		public const int PendingTextFlushMilliseconds = 100;
		public override Command GetKeyHandler(Keys keyData) {
			Command command = base.GetKeyHandler(keyData);
			if (command != null)
				View.Control.ForceFlushPendingTextInput();
			return command;
		}
		string text = String.Empty;
		DateTime lastFlushTime = DateTime.Now;
		protected internal virtual void FlushPendingTextInput() {
			this.lastFlushTime = DateTime.Now;
			if (String.IsNullOrEmpty(text))
				return;
			string pendingInput = this.text;
			this.text = String.Empty;
			PerformFlushPendingTextInput(pendingInput);
			this.lastFlushTime = DateTime.Now;
		}
		protected internal virtual void PerformFlushPendingTextInput(string pendingInput) {
			if (pendingInput.Length == 1)
				FlushPendingTextInputCore(pendingInput);
			else
				FlushPendingTextInputByParts(pendingInput);
		}
		protected internal virtual void FlushPendingTextInputCore(string pendingInput) {
			IInsertTextCommand command = CreateFlushPendingTextCommand();
			if (command != null) {
				command.Text = pendingInput;
				command.CommandSourceType = CommandSourceType.Keyboard;
				command.Execute();
			}
		}
		private IInsertTextCommand CreateFlushPendingTextCommand() {
			IRichEditCommandFactoryService service = View.Control.GetService(typeof(IRichEditCommandFactoryService)) as IRichEditCommandFactoryService;
			if (service == null)
				return null;
			if (View.Control.Overtype)
				return  (IInsertTextCommand)(service.CreateCommand(RichEditCommandId.OvertypeText));
			else
				return  (IInsertTextCommand)(service.CreateCommand(RichEditCommandId.InsertText));
		}
		protected internal virtual void FlushPendingTextInputByParts(string pendingInput) {
			int startIndex = 0;
			int count = pendingInput.Length;
			for (int i = 0; i < count; i++) {
				if (IsSeparator(pendingInput[i])) {
					string part = pendingInput.Substring(startIndex, i - startIndex + 1);
					startIndex = i + 1;
					FlushPendingTextInputCore(part);
				}
			}
			if (startIndex < count)
				FlushPendingTextInputCore(pendingInput.Substring(startIndex));
		}
		protected internal virtual bool IsSeparator(char ch) {
			return Char.IsWhiteSpace(ch) || Char.IsSeparator(ch) || Char.IsPunctuation(ch);
		}
		RichEditCursor previousCursor;
		public override bool HandleKey(Keys keyData) {
			if (base.HandleKey(keyData))
				return true;
			TryChangeCursor();
			return false;
		}
		protected internal virtual void TryChangeCursor() {
			IRichEditControl control = View.Control;
			if (!(ShouldChangeCursor(RichEditCursors.Hand) && control.IsHyperlinkActive()))
				return;
			previousCursor = new RichEditCursor(GetCursor());
			SetCursor(RichEditCursors.Hand);
		}
		protected internal virtual bool ShouldChangeCursor(RichEditCursor cursor) {
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
		void SetCursor(RichEditCursor cursor) {
			View.Control.Cursor = cursor.Cursor;
		}
		Cursor GetCursor() {
			return View.Control.Cursor;
		}
		public virtual Keys GetKeys(RichEditCommandId commandId, RichEditViewType viewType) {
			foreach(Int64 keyData in KeyHandlerIdTable.Keys) {
				RichEditViewType viewTypeFromHash = RichEditKeyHashProvider.GetViewTypeFromHash(keyData);
				if(viewTypeFromHash != viewType)
					continue;
				RichEditCommandId id = KeyHandlerIdTable[keyData];
				if(Object.Equals(commandId, id)) {
					Keys res = KeyDataToKeys(keyData);
					res = RichEditKeyHashProvider.DeleteViewTypeInfoFromKeys(res);
					return res;
				}
			}
			return Keys.None;
		}
	}
	#endregion
}
