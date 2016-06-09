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
using DevExpress.Services;
using DevExpress.Utils.KeyboardHandler;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Keyboard;
using System.Windows.Forms;
using DevExpress.Compatibility.System.Windows.Forms;
namespace DevExpress.XtraSpreadsheet.Internal {
	public partial class InnerSpreadsheetControl {
		Stack<KeyboardHandler> keyboardHandlers;
		PictureKeyboardHandler pictureKeyboardHandler;
		public KeyboardHandler KeyboardHandler {
			get {
				if (keyboardHandlers == null) return null;
				return keyboardHandlers.Peek();
			}
		}
		protected internal PictureKeyboardHandler PictureKeyboardHandler { get { return pictureKeyboardHandler; } }
		protected internal Stack<KeyboardHandler> KeyboardHandlers { get { return keyboardHandlers; } }
		#region InitializeKeyboardHandlers
		protected internal virtual void InitializeKeyboardHandlers() {
			this.keyboardHandlers = new Stack<KeyboardHandler>();
			NormalKeyboardHandler defaultKeyboardHandler = CreateDefaultKeyboardHandler();
			InitializeKeyboardHandlerDefaults(defaultKeyboardHandler);
			SetNewKeyboardHandler(defaultKeyboardHandler);
			this.pictureKeyboardHandler = CreatePictureKeyboardHandler();
			InitializePictureKeyboardHandler(pictureKeyboardHandler);
		}
		#endregion
		#region InitializeKeyboardHandlerDefaults
		protected internal virtual void InitializeKeyboardHandlerDefaults(NormalKeyboardHandler keyboardHandler) {
			InitializeDefaultViewKeyboardHandlers(keyboardHandler, new SpreadsheetKeyHashProvider());
		}
		protected internal virtual void InitializeDefaultViewKeyboardHandlers(NormalKeyboardHandler keyboardHandler, IKeyHashProvider provider) {
			AppendFileKeyboardShortcuts(keyboardHandler, provider);
			AppendEditKeyboardShortcuts(keyboardHandler, provider);
			AppendFormatKeyboardShortcuts(keyboardHandler, provider);
			AppendFindReplaceKeyboardShortcuts(keyboardHandler, provider);
			AppendSelectionKeyboardShortcuts(keyboardHandler, provider);
			AppendClipboardKeyboardShortcuts(keyboardHandler, provider);
			AppendArrayFormulaKeyboardShortcuts(keyboardHandler, provider);
			AppendHyperlinkKeyboardShortcuts(keyboardHandler, provider);
			AppendCollapseOrExpandFormulaBarKeyboardShortcuts(keyboardHandler, provider);
			AppendTableKeyboardShortcuts(keyboardHandler, provider);
			AppendAdditionalKeyboardShortcuts(keyboardHandler, provider);
			AppendOutlineGroupKeyboardShortcuts(keyboardHandler, provider);
			AppendSortAndFilterKeyboardShortcuts(keyboardHandler, provider);
			AppendCommentKeyboardShortcuts(keyboardHandler, provider);
		}
		protected internal virtual void AppendFileKeyboardShortcuts(NormalKeyboardHandler keyboardHandler, IKeyHashProvider provider) {
			keyboardHandler.RegisterKeyHandler(provider, Keys.N, Keys.Control, SpreadsheetCommandId.FileNew);
			keyboardHandler.RegisterKeyHandler(provider, Keys.O, Keys.Control, SpreadsheetCommandId.FileOpen);
#if !SL
			keyboardHandler.RegisterKeyHandler(provider, Keys.S, Keys.Control, SpreadsheetCommandId.FileSave);
#endif
			keyboardHandler.RegisterKeyHandler(provider, Keys.F12, Keys.None, SpreadsheetCommandId.FileSaveAs);
			keyboardHandler.RegisterKeyHandler(provider, Keys.P, Keys.Control, SpreadsheetCommandId.FilePrint);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Z, Keys.Control, SpreadsheetCommandId.FileUndo);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Y, Keys.Control, SpreadsheetCommandId.FileRedo);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Back, Keys.Alt, SpreadsheetCommandId.FileUndo);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Back, Keys.Alt | Keys.Shift, SpreadsheetCommandId.FileRedo);
		}
		protected internal virtual void AppendEditKeyboardShortcuts(NormalKeyboardHandler keyboardHandler, IKeyHashProvider provider) {
			keyboardHandler.RegisterKeyHandler(provider, Keys.F2, Keys.None, SpreadsheetCommandId.InplaceBeginEdit);
			keyboardHandler.RegisterKeyHandler(provider, Keys.F3, Keys.Shift, SpreadsheetCommandId.InsertFunction);
#if !SL
			keyboardHandler.RegisterKeyHandler(provider, Keys.Oemplus, Keys.Alt, SpreadsheetCommandId.FunctionsInsertSum);
#endif
		}
		protected internal virtual void AppendFormatKeyboardShortcuts(NormalKeyboardHandler keyboardHandler, IKeyHashProvider provider) {
			keyboardHandler.RegisterKeyHandler(provider, Keys.B, Keys.Control, SpreadsheetCommandId.FormatFontBold);
			keyboardHandler.RegisterKeyHandler(provider, Keys.D2, Keys.Control, SpreadsheetCommandId.FormatFontBold);
			keyboardHandler.RegisterKeyHandler(provider, Keys.I, Keys.Control, SpreadsheetCommandId.FormatFontItalic);
			keyboardHandler.RegisterKeyHandler(provider, Keys.D3, Keys.Control, SpreadsheetCommandId.FormatFontItalic);
			keyboardHandler.RegisterKeyHandler(provider, Keys.U, Keys.Control, SpreadsheetCommandId.FormatFontUnderline);
			keyboardHandler.RegisterKeyHandler(provider, Keys.D4, Keys.Control, SpreadsheetCommandId.FormatFontUnderline);
			keyboardHandler.RegisterKeyHandler(provider, Keys.D5, Keys.Control, SpreadsheetCommandId.FormatFontStrikeout);
			keyboardHandler.RegisterKeyHandler(provider, Keys.D9, Keys.Control, SpreadsheetCommandId.HideRows);
			keyboardHandler.RegisterKeyHandler(provider, Keys.D0, Keys.Control, SpreadsheetCommandId.HideColumns);
			keyboardHandler.RegisterKeyHandler(provider, Keys.D1, Keys.Control | Keys.Shift, SpreadsheetCommandId.FormatNumberPredefined4);
			keyboardHandler.RegisterKeyHandler(provider, Keys.D2, Keys.Control | Keys.Shift, SpreadsheetCommandId.FormatNumberPredefined18);
			keyboardHandler.RegisterKeyHandler(provider, Keys.D3, Keys.Control | Keys.Shift, SpreadsheetCommandId.FormatNumberPredefined15);
			keyboardHandler.RegisterKeyHandler(provider, Keys.D4, Keys.Control | Keys.Shift, SpreadsheetCommandId.FormatNumberPredefined8);
			keyboardHandler.RegisterKeyHandler(provider, Keys.D5, Keys.Control | Keys.Shift, SpreadsheetCommandId.FormatNumberPercent);
			keyboardHandler.RegisterKeyHandler(provider, Keys.D6, Keys.Control | Keys.Shift, SpreadsheetCommandId.FormatNumberScientific);
			keyboardHandler.RegisterKeyHandler(provider, Keys.D7, Keys.Control | Keys.Shift, SpreadsheetCommandId.FormatOutsideBorders);
			keyboardHandler.RegisterKeyHandler(provider, Keys.D9, Keys.Control | Keys.Shift, SpreadsheetCommandId.UnhideRows);
			keyboardHandler.RegisterKeyHandler(provider, Keys.D0, Keys.Control | Keys.Shift, SpreadsheetCommandId.UnhideColumns);
#if !SL
			keyboardHandler.RegisterKeyHandler(provider, Keys.OemMinus, Keys.Control | Keys.Shift, SpreadsheetCommandId.FormatNoBorders);
#endif
			keyboardHandler.RegisterKeyHandler(provider, Keys.Delete, Keys.None, SpreadsheetCommandId.FormatClearContents);
			keyboardHandler.RegisterKeyHandler(provider, Keys.F11, Keys.Shift, SpreadsheetCommandId.InsertSheet);
			keyboardHandler.RegisterKeyHandler(provider, Keys.D, Keys.Control, SpreadsheetCommandId.EditingFillDown);
			keyboardHandler.RegisterKeyHandler(provider, Keys.R, Keys.Control, SpreadsheetCommandId.EditingFillRight);
			keyboardHandler.RegisterKeyHandler(provider, Keys.D1, Keys.Control, SpreadsheetCommandId.FormatCellsNumber);
			keyboardHandler.RegisterKeyHandler(provider, Keys.F, Keys.Control | Keys.Shift, SpreadsheetCommandId.FormatCellsFont);
		}
		protected internal virtual void AppendFindReplaceKeyboardShortcuts(NormalKeyboardHandler keyboardHandler, IKeyHashProvider provider) {
			keyboardHandler.RegisterKeyHandler(provider, Keys.F, Keys.Control, SpreadsheetCommandId.EditingFind);
			keyboardHandler.RegisterKeyHandler(provider, Keys.H, Keys.Control, SpreadsheetCommandId.EditingReplace);
		}
		protected internal virtual void AppendSelectionKeyboardShortcuts(CommandBasedKeyboardHandler<SpreadsheetCommandId> keyboardHandler, IKeyHashProvider provider) {
			keyboardHandler.RegisterKeyHandler(provider, Keys.Space, Keys.Shift, SpreadsheetCommandId.SelectActiveRow); 
			keyboardHandler.RegisterKeyHandler(provider, Keys.A, Keys.Control, SpreadsheetCommandId.SelectAll);
			AppendSelectionKeyboardShortcutsSharedWithInplaceEditor(keyboardHandler, provider);
		}
		protected internal virtual void AppendSelectionKeyboardShortcutsSharedWithInplaceEditor(CommandBasedKeyboardHandler<SpreadsheetCommandId> keyboardHandler, IKeyHashProvider provider) {
			keyboardHandler.RegisterKeyHandler(provider, Keys.Enter, Keys.None, SpreadsheetCommandId.SelectionMoveActiveCellOnEnterPress);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Enter, Keys.Shift, SpreadsheetCommandId.SelectionMoveActiveCellOnEnterPressReverse);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Left, Keys.None, SpreadsheetCommandId.SelectionMoveLeft);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Right, Keys.None, SpreadsheetCommandId.SelectionMoveRight);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Up, Keys.None, SpreadsheetCommandId.SelectionMoveUp);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Down, Keys.None, SpreadsheetCommandId.SelectionMoveDown);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Left, Keys.Control, SpreadsheetCommandId.SelectionMoveLeftToDataEdge);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Right, Keys.Control, SpreadsheetCommandId.SelectionMoveRightToDataEdge);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Up, Keys.Control, SpreadsheetCommandId.SelectionMoveUpToDataEdge);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Down, Keys.Control, SpreadsheetCommandId.SelectionMoveDownToDataEdge);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Home, Keys.None, SpreadsheetCommandId.SelectionMoveToLeftColumn);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Home, Keys.Control, SpreadsheetCommandId.SelectionMoveToTopLeftCell);
			keyboardHandler.RegisterKeyHandler(provider, Keys.End, Keys.Control, SpreadsheetCommandId.SelectionMoveToLastUsedCell);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Tab, Keys.None, SpreadsheetCommandId.SelectionMoveActiveCellRight);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Tab, Keys.Shift, SpreadsheetCommandId.SelectionMoveActiveCellLeft);
#if !SL
			keyboardHandler.RegisterKeyHandler(provider, Keys.OemPeriod, Keys.Control, SpreadsheetCommandId.SelectionMoveActiveCellToNextCorner);
#endif
			keyboardHandler.RegisterKeyHandler(provider, Keys.Left, Keys.Control | Keys.Alt, SpreadsheetCommandId.SelectionPreviousRange);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Right, Keys.Control | Keys.Alt, SpreadsheetCommandId.SelectionNextRange);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Back, Keys.Shift, SpreadsheetCommandId.SelectActiveCell);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Space, Keys.Control, SpreadsheetCommandId.SelectActiveColumn);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Space, Keys.Control | Keys.Shift, SpreadsheetCommandId.SelectAll);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Left, Keys.Shift, SpreadsheetCommandId.SelectionExpandLeft);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Right, Keys.Shift, SpreadsheetCommandId.SelectionExpandRight);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Up, Keys.Shift, SpreadsheetCommandId.SelectionExpandUp);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Down, Keys.Shift, SpreadsheetCommandId.SelectionExpandDown);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Left, Keys.Control | Keys.Shift, SpreadsheetCommandId.SelectionExpandLeftToDataEdge);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Right, Keys.Control | Keys.Shift, SpreadsheetCommandId.SelectionExpandRightToDataEdge);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Up, Keys.Control | Keys.Shift, SpreadsheetCommandId.SelectionExpandUpToDataEdge);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Down, Keys.Control | Keys.Shift, SpreadsheetCommandId.SelectionExpandDownToDataEdge);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Home, Keys.Shift, SpreadsheetCommandId.SelectionExpandToLeftColumn);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Home, Keys.Control | Keys.Shift, SpreadsheetCommandId.SelectionExpandToTopLeftCell);
			keyboardHandler.RegisterKeyHandler(provider, Keys.End, Keys.Control | Keys.Shift, SpreadsheetCommandId.SelectionExpandToLastUsedCell);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageDown, Keys.None, SpreadsheetCommandId.SelectionMovePageDown);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageUp, Keys.None, SpreadsheetCommandId.SelectionMovePageUp);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageDown, Keys.Shift, SpreadsheetCommandId.SelectionExpandPageDown);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageUp, Keys.Shift, SpreadsheetCommandId.SelectionExpandPageUp);
		}
		protected internal virtual void AppendClipboardKeyboardShortcuts(NormalKeyboardHandler keyboardHandler, IKeyHashProvider provider) {
			keyboardHandler.RegisterKeyHandler(provider, Keys.C, Keys.Control, SpreadsheetCommandId.CopySelection);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Insert, Keys.Control, SpreadsheetCommandId.CopySelection);
			keyboardHandler.RegisterKeyHandler(provider, Keys.V, Keys.Control, SpreadsheetCommandId.PasteSelection);
			keyboardHandler.RegisterKeyHandler(provider, Keys.V, Keys.Control | Keys.Alt, SpreadsheetCommandId.ShowPasteSpecialForm);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Insert, Keys.Shift, SpreadsheetCommandId.PasteSelection);
			keyboardHandler.RegisterKeyHandler(provider, Keys.X, Keys.Control, SpreadsheetCommandId.CutSelection);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Delete, Keys.Shift, SpreadsheetCommandId.CutSelection);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Escape, Keys.None, SpreadsheetCommandId.ClearCopiedRange);
		}
		protected internal virtual void AppendHyperlinkKeyboardShortcuts(NormalKeyboardHandler keyboardHandler, IKeyHashProvider provider) {
			keyboardHandler.RegisterKeyHandler(provider, Keys.Control, Keys.K, SpreadsheetCommandId.InsertHyperlinkContextMenuItem);
		}
		protected internal virtual void AppendCollapseOrExpandFormulaBarKeyboardShortcuts(NormalKeyboardHandler keyboardHandler, IKeyHashProvider provider) {
			keyboardHandler.RegisterKeyHandler(provider, Keys.Control, Keys.Shift | Keys.U, SpreadsheetCommandId.CollapseOrExpandFormulaBar);
		}
		protected internal virtual void AppendTableKeyboardShortcuts(NormalKeyboardHandler keyboardHandler, IKeyHashProvider provider) {
			keyboardHandler.RegisterKeyHandler(provider, Keys.Control, Keys.T, SpreadsheetCommandId.InsertTable);
		}
		protected internal virtual void AppendAdditionalKeyboardShortcuts(NormalKeyboardHandler keyboardHandler, IKeyHashProvider provider) {
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageDown, Keys.Control, SpreadsheetCommandId.MoveToNextSheet);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageUp, Keys.Control, SpreadsheetCommandId.MoveToPreviousSheet);
		}
		protected internal virtual void AppendOutlineGroupKeyboardShortcuts(NormalKeyboardHandler keyboardHandler, IKeyHashProvider provider) {
			keyboardHandler.RegisterKeyHandler(provider, Keys.Shift, Keys.Alt | Keys.Right, SpreadsheetCommandId.GroupOutline);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Shift, Keys.Alt | Keys.Left, SpreadsheetCommandId.UngroupOutline);
		}
		protected internal virtual void AppendSortAndFilterKeyboardShortcuts(NormalKeyboardHandler keyboardHandler, IKeyHashProvider provider) {
			keyboardHandler.RegisterKeyHandler(provider, Keys.L, Keys.Shift | Keys.Control, SpreadsheetCommandId.DataFilterToggle);
			keyboardHandler.RegisterKeyHandler(provider, Keys.L, Keys.Alt | Keys.Control, SpreadsheetCommandId.DataFilterReApply);
		}
		protected internal virtual void AppendCommentKeyboardShortcuts(NormalKeyboardHandler keyboardHandler, IKeyHashProvider provider) {
			keyboardHandler.RegisterKeyHandler(provider, Keys.F2, Keys.Shift, SpreadsheetCommandId.ReviewInsertComment);
		}
		protected internal virtual void AppendArrayFormulaKeyboardShortcuts(NormalKeyboardHandler keyboardHandler, IKeyHashProvider provider) {
			keyboardHandler.RegisterKeyHandler(provider, Keys.F9, Keys.Alt | Keys.Control, SpreadsheetCommandId.FormulasCalculateFull);
			keyboardHandler.RegisterKeyHandler(provider, Keys.F9, Keys.Alt | Keys.Control | Keys.Shift, SpreadsheetCommandId.FormulasCalculateFullRebuild);
			keyboardHandler.RegisterKeyHandler(provider, Keys.F9, Keys.None, SpreadsheetCommandId.FormulasCalculateNow);
			keyboardHandler.RegisterKeyHandler(provider, Keys.F9, Keys.Shift, SpreadsheetCommandId.FormulasCalculateSheet);
			keyboardHandler.RegisterKeyHandler(provider, Keys.F3, Keys.Control, SpreadsheetCommandId.FormulasShowNameManager);
			keyboardHandler.RegisterKeyHandler(provider, Keys.F3, Keys.Control | Keys.Shift, SpreadsheetCommandId.FormulasCreateDefinedNamesFromSelection);
#if !SL
			keyboardHandler.RegisterKeyHandler(provider, Keys.Oemtilde, Keys.Control, SpreadsheetCommandId.ViewShowFormulas);
#endif
		}
		#endregion
		#region InitializePictureKeyboardHandler
		protected internal virtual void InitializePictureKeyboardHandler(PictureKeyboardHandler keyboardHandler) {
			InitializePictureViewKeyboardHandlers(keyboardHandler, new SpreadsheetKeyHashProvider());
		}
		protected internal virtual void InitializePictureViewKeyboardHandlers(PictureKeyboardHandler keyboardHandler, IKeyHashProvider provider) {
			AppendFileKeyboardShortcuts(keyboardHandler, provider);
			AppendClipboardKeyboardShortcuts(keyboardHandler, provider);
			AppendPictureFormatKeyboardShortcuts(keyboardHandler, provider);
		}
		protected internal virtual void AppendPictureFormatKeyboardShortcuts(NormalKeyboardHandler keyboardHandler, IKeyHashProvider provider) {
			keyboardHandler.RegisterKeyHandler(provider, Keys.Delete, Keys.None, SpreadsheetCommandId.FormatClearAll);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Back, Keys.None, SpreadsheetCommandId.FormatClearAll);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Left, Keys.None, SpreadsheetCommandId.ShapeMoveLeft);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Right, Keys.None, SpreadsheetCommandId.ShapeMoveRight);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Up, Keys.None, SpreadsheetCommandId.ShapeMoveUp);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Down, Keys.None, SpreadsheetCommandId.ShapeMoveDown);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Left, Keys.Alt, SpreadsheetCommandId.ShapeRotateLeft);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Left, Keys.Alt | Keys.Control, SpreadsheetCommandId.ShapeRotateLeftByDegree);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Right, Keys.Alt, SpreadsheetCommandId.ShapeRotateRight);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Right, Keys.Alt | Keys.Control, SpreadsheetCommandId.ShapeRotateRightByDegree);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Right, Keys.Shift, SpreadsheetCommandId.ShapeEnlargeWidth);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Up, Keys.Shift, SpreadsheetCommandId.ShapeEnlargeHeight);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Right, Keys.Shift | Keys.Control, SpreadsheetCommandId.ShapeBitEnlargeWidth);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Up, Keys.Shift | Keys.Control, SpreadsheetCommandId.ShapeBitEnlargeHeight);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Left, Keys.Shift, SpreadsheetCommandId.ShapeReduceWidth);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Down, Keys.Shift, SpreadsheetCommandId.ShapeReduceHeight);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Left, Keys.Shift | Keys.Control, SpreadsheetCommandId.ShapeBitReduceWidth);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Down, Keys.Shift | Keys.Control, SpreadsheetCommandId.ShapeBitReduceHeight);
			keyboardHandler.RegisterKeyHandler(provider, Keys.A, Keys.Control, SpreadsheetCommandId.ShapeSelectAll);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Tab, Keys.None, SpreadsheetCommandId.ShapeSelectNext);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Tab, Keys.Shift, SpreadsheetCommandId.ShapeSelectPrevious);
		}
		#endregion
		protected internal virtual NormalKeyboardHandler CreateDefaultKeyboardHandler() {
			return new NormalKeyboardHandler();
		}
		protected internal virtual PictureKeyboardHandler CreatePictureKeyboardHandler() {
			return new PictureKeyboardHandler();
		}
		protected internal virtual void SetNewKeyboardHandler(KeyboardHandler keyboardHandler) {
			keyboardHandlers.Push(keyboardHandler);
		}
		protected internal virtual void RestoreKeyboardHandler() {
			keyboardHandlers.Pop();
		}
		public virtual bool OnKeyDown(KeyEventArgs e) {
			if (IsDisposed)
				return false;
			IKeyboardHandlerService svc = GetService<IKeyboardHandlerService>();
			if (svc != null) {
				svc.OnKeyDown(e);
				return true;
			}
			else
				return false;
		}
		public virtual bool OnKeyUp(KeyEventArgs e) {
			if (IsDisposed)
				return false;
			IKeyboardHandlerService svc = GetService<IKeyboardHandlerService>();
			if (svc != null) {
				svc.OnKeyUp(e);
				return true;
			}
			else
				return false;
		}
		public virtual bool OnKeyPress(KeyPressEventArgs e) {
			if (IsDisposed)
				return false;
			IKeyboardHandlerService svc = GetService<IKeyboardHandlerService>();
			if (svc != null) {
				svc.OnKeyPress(e);
				return true;
			}
			else
				return false;
		}
		public void AssignShortcutKeyToCommand(Keys key, Keys modifier, SpreadsheetCommandId commandId) {
			AssignShortcutKeyToCommand(key, modifier, commandId, SpreadsheetViewType.Normal);
		}
		public void AssignShortcutKeyToCommand(Keys key, Keys modifier, SpreadsheetCommandId commandId, SpreadsheetViewType viewType) {
			NormalKeyboardHandler handler = KeyboardHandler as NormalKeyboardHandler;
			if (handler == null)
				return;
			SpreadsheetKeyHashProvider provider = new SpreadsheetKeyHashProvider();
			handler.UnregisterKeyHandler(provider, key, modifier);
			handler.RegisterKeyHandler(provider, key, modifier, commandId);
		}
		public void RemoveShortcutKey(Keys key, Keys modifier) {
			RemoveShortcutKey(key, modifier, SpreadsheetViewType.Normal);
		}
		public void RemoveShortcutKey(Keys key, Keys modifier, SpreadsheetViewType viewType) {
			NormalKeyboardHandler handler = KeyboardHandler as NormalKeyboardHandler;
			if (handler == null)
				return;
			handler.UnregisterKeyHandler(new SpreadsheetKeyHashProvider(), key, modifier);
		}
	}
}
