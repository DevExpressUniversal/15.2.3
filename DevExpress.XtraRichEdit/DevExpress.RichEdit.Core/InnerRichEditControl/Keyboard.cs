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
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Keyboard;
using System.Windows.Forms;
using DevExpress.Compatibility.System.Windows.Forms;
namespace DevExpress.XtraRichEdit.Internal {
	public partial class InnerRichEditControl {
		Stack<KeyboardHandler> keyboardHandlers;
		protected internal KeyboardHandler KeyboardHandler { get { return keyboardHandlers.Peek(); } }
		protected internal Stack<KeyboardHandler> KeyboardHandlers { get { return keyboardHandlers; } }
#region InitializeKeyboardHandlers
		protected internal virtual void InitializeKeyboardHandlers() {
			this.keyboardHandlers = new Stack<KeyboardHandler>();
			NormalKeyboardHandler defaultKeyboardHandler = CreateDefaultKeyboardHandler();
			InitializeKeyboardHandlerDefaults(defaultKeyboardHandler);
			SetNewKeyboardHandler(defaultKeyboardHandler);
		}
		#endregion
		#region InitializeKeyboardHandlerDefaults
		protected internal virtual void InitializeKeyboardHandlerDefaults(NormalKeyboardHandler keyboardHandler) {
			InitializeDefaultViewKeyboardHandlers(keyboardHandler, new RichEditKeyHashProvider(RichEditViewType.Simple));
			InitializeDefaultViewKeyboardHandlers(keyboardHandler, new RichEditKeyHashProvider(RichEditViewType.Draft));
			InitializeDefaultViewKeyboardHandlers(keyboardHandler, new RichEditKeyHashProvider(RichEditViewType.PrintLayout));
		}
		public bool GetShortcut(RichEditCommandId commandId, out Keys result) {
			result = Keys.None;
			NormalKeyboardHandler normalKeyboardHandler = KeyboardHandler as NormalKeyboardHandler;
			if(normalKeyboardHandler == null)
				return false;
			result = normalKeyboardHandler.GetKeys(commandId);
			return true;
		}
		public Keys GetShortcut(RichEditCommandId commandId, RichEditViewType viewType) {
			NormalKeyboardHandler normalKeyboardHandler = KeyboardHandler as NormalKeyboardHandler;
			if(normalKeyboardHandler == null)
				return Keys.None;
			return normalKeyboardHandler.GetKeys(commandId, viewType);
		}
		protected internal virtual void InitializeDefaultViewKeyboardHandlers(NormalKeyboardHandler keyboardHandler, IKeyHashProvider provider) {
			keyboardHandler.RegisterKeyHandler(provider, Keys.D8, Keys.Control | Keys.Shift, RichEditCommandId.ToggleShowWhitespace);
			keyboardHandler.RegisterKeyHandler(provider, Keys.F8, Keys.None, RichEditCommandId.UpdateFields);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Left, Keys.None, RichEditCommandId.PreviousCharacter);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Right, Keys.None, RichEditCommandId.NextCharacter);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Left, Keys.Shift, RichEditCommandId.ExtendPreviousCharacter);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Right, Keys.Shift, RichEditCommandId.ExtendNextCharacter);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Left, Keys.Control, RichEditCommandId.PreviousWord);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Right, Keys.Control, RichEditCommandId.NextWord);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Left, Keys.Control | Keys.Shift, RichEditCommandId.ExtendPreviousWord);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Right, Keys.Control | Keys.Shift, RichEditCommandId.ExtendNextWord);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Home, Keys.None, RichEditCommandId.StartOfLine);
			keyboardHandler.RegisterKeyHandler(provider, Keys.End, Keys.None, RichEditCommandId.EndOfLine);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Home, Keys.Shift, RichEditCommandId.ExtendStartOfLine);
			keyboardHandler.RegisterKeyHandler(provider, Keys.End, Keys.Shift, RichEditCommandId.ExtendEndOfLine);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Up, Keys.None, RichEditCommandId.PreviousLine);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Down, Keys.None, RichEditCommandId.NextLine);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Up, Keys.Shift, RichEditCommandId.ExtendPreviousLine);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Down, Keys.Shift, RichEditCommandId.ExtendNextLine);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Up, Keys.Control, RichEditCommandId.PreviousParagraph);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Down, Keys.Control, RichEditCommandId.NextParagraph);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Up, Keys.Control | Keys.Shift, RichEditCommandId.ExtendPreviousParagraph);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Down, Keys.Control | Keys.Shift, RichEditCommandId.ExtendNextParagraph);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageUp, Keys.Control, RichEditCommandId.PreviousPage);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageDown, Keys.Control, RichEditCommandId.NextPage);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageUp, Keys.Control | Keys.Shift, RichEditCommandId.ExtendPreviousPage);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageDown, Keys.Control | Keys.Shift, RichEditCommandId.ExtendNextPage);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageUp, Keys.None, RichEditCommandId.PreviousScreen);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageDown, Keys.None, RichEditCommandId.NextScreen);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageUp, Keys.Shift, RichEditCommandId.ExtendPreviousScreen);
			keyboardHandler.RegisterKeyHandler(provider, Keys.PageDown, Keys.Shift, RichEditCommandId.ExtendNextScreen);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Home, Keys.Control, RichEditCommandId.StartOfDocument);
			keyboardHandler.RegisterKeyHandler(provider, Keys.End, Keys.Control, RichEditCommandId.EndOfDocument);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Home, Keys.Control | Keys.Shift, RichEditCommandId.ExtendStartOfDocument);
			keyboardHandler.RegisterKeyHandler(provider, Keys.End, Keys.Control | Keys.Shift, RichEditCommandId.ExtendEndOfDocument);
			keyboardHandler.RegisterKeyHandler(provider, Keys.A, Keys.Control, RichEditCommandId.SelectAll);
			keyboardHandler.RegisterKeyHandler(provider, Keys.NumPad5, Keys.Control, RichEditCommandId.SelectAll);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Insert, Keys.None, RichEditCommandId.ToggleOvertype);
#if !SL
			keyboardHandler.RegisterKeyHandler(provider, Keys.Clear, Keys.Control, RichEditCommandId.SelectAll);
#endif
			keyboardHandler.RegisterKeyHandler(provider, Keys.Z, Keys.Control, RichEditCommandId.Undo);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Y, Keys.Control, RichEditCommandId.Redo);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Back, Keys.Alt, RichEditCommandId.Undo);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Back, Keys.Alt | Keys.Shift, RichEditCommandId.Redo);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Enter, Keys.None, RichEditCommandId.EnterKey);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Enter, Keys.Shift, RichEditCommandId.InsertLineBreak);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Tab, Keys.None, RichEditCommandId.TabKey);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Tab, Keys.Shift, RichEditCommandId.ShiftTabKey);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Tab, Keys.Control, RichEditCommandId.InsertTab);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Enter, Keys.Control, RichEditCommandId.InsertPageBreak);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Space, Keys.Control, RichEditCommandId.ClearFormatting);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Space, Keys.Shift | Keys.Control, RichEditCommandId.InsertNonBreakingSpace);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Enter, Keys.Control | Keys.Shift, RichEditCommandId.InsertColumnBreak);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Subtract, Keys.Control, RichEditCommandId.InsertEnDash);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Subtract, Keys.Control | Keys.Alt, RichEditCommandId.InsertEmDash);
			keyboardHandler.RegisterKeyHandler(provider, Keys.C, Keys.Control | Keys.Alt, RichEditCommandId.InsertCopyrightSymbol);
			keyboardHandler.RegisterKeyHandler(provider, Keys.R, Keys.Control | Keys.Alt, RichEditCommandId.InsertRegisteredTrademarkSymbol);
			keyboardHandler.RegisterKeyHandler(provider, Keys.T, Keys.Control | Keys.Alt, RichEditCommandId.InsertTrademarkSymbol);
#if (!SL)
			keyboardHandler.RegisterKeyHandler(provider, Keys.OemPeriod, Keys.Control | Keys.Alt, RichEditCommandId.InsertEllipsis);
#endif
			keyboardHandler.RegisterKeyHandler(provider, Keys.B, Keys.Control, RichEditCommandId.ToggleFontBold);
			keyboardHandler.RegisterKeyHandler(provider, Keys.I, Keys.Control, RichEditCommandId.ToggleFontItalic);
			keyboardHandler.RegisterKeyHandler(provider, Keys.U, Keys.Control, RichEditCommandId.ToggleFontUnderline);
			keyboardHandler.RegisterKeyHandler(provider, Keys.D, Keys.Control | Keys.Shift, RichEditCommandId.ToggleFontDoubleUnderline);
			keyboardHandler.RegisterKeyHandler(provider, Keys.H, Keys.Control | Keys.Shift, RichEditCommandId.ToggleHiddenText);
#if (!SL)
			keyboardHandler.RegisterKeyHandler(provider, Keys.OemPeriod, Keys.Control | Keys.Shift, RichEditCommandId.IncreaseFontSize);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Oemcomma, Keys.Control | Keys.Shift, RichEditCommandId.DecreaseFontSize);
			keyboardHandler.RegisterKeyHandler(provider, Keys.OemCloseBrackets, Keys.Control, RichEditCommandId.IncrementFontSize);
			keyboardHandler.RegisterKeyHandler(provider, Keys.OemOpenBrackets, Keys.Control, RichEditCommandId.DecrementFontSize);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Oemplus, Keys.Control | Keys.Shift, RichEditCommandId.ToggleFontSuperscript);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Oemplus, Keys.Control, RichEditCommandId.ToggleFontSubscript);
#endif
			keyboardHandler.RegisterKeyHandler(provider, Keys.D, Keys.Control, RichEditCommandId.ShowFontForm);
#if !SL
			keyboardHandler.RegisterKeyHandler(provider, Keys.S, Keys.Control, RichEditCommandId.FileSave);
#endif
			keyboardHandler.RegisterKeyHandler(provider, Keys.F12, Keys.None, RichEditCommandId.FileSaveAs);
			keyboardHandler.RegisterKeyHandler(provider, Keys.P, Keys.Control, RichEditCommandId.Print);
			keyboardHandler.RegisterKeyHandler(provider, Keys.K, Keys.Control, RichEditCommandId.ShowHyperlinkForm);
			keyboardHandler.RegisterKeyHandler(provider, Keys.O, Keys.Control, RichEditCommandId.FileOpen);
			keyboardHandler.RegisterKeyHandler(provider, Keys.N, Keys.Control, RichEditCommandId.FileNew);
			keyboardHandler.RegisterKeyHandler(provider, Keys.L, Keys.Control, RichEditCommandId.ToggleParagraphAlignmentLeft);
			keyboardHandler.RegisterKeyHandler(provider, Keys.E, Keys.Control, RichEditCommandId.ToggleParagraphAlignmentCenter);
			keyboardHandler.RegisterKeyHandler(provider, Keys.R, Keys.Control, RichEditCommandId.ToggleParagraphAlignmentRight);
			keyboardHandler.RegisterKeyHandler(provider, Keys.J, Keys.Control, RichEditCommandId.ToggleParagraphAlignmentJustify);
			keyboardHandler.RegisterKeyHandler(provider, Keys.D1, Keys.Control, RichEditCommandId.SetSingleParagraphSpacing);
			keyboardHandler.RegisterKeyHandler(provider, Keys.D2, Keys.Control, RichEditCommandId.SetDoubleParagraphSpacing);
			keyboardHandler.RegisterKeyHandler(provider, Keys.D5, Keys.Control, RichEditCommandId.SetSesquialteralParagraphSpacing);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Delete, Keys.None, RichEditCommandId.Delete);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Back, Keys.None, RichEditCommandId.BackSpaceKey);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Back, Keys.Shift, RichEditCommandId.BackSpaceKey);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Delete, Keys.Control, RichEditCommandId.DeleteWord);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Back, Keys.Control, RichEditCommandId.DeleteWordBack);
			keyboardHandler.RegisterKeyHandler(provider, Keys.C, Keys.Control, RichEditCommandId.CopySelection);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Insert, Keys.Control, RichEditCommandId.CopySelection);
			keyboardHandler.RegisterKeyHandler(provider, Keys.V, Keys.Control, RichEditCommandId.PasteSelection);
			keyboardHandler.RegisterKeyHandler(provider, Keys.V, Keys.Control | Keys.Alt, RichEditCommandId.ShowPasteSpecialForm);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Insert, Keys.Shift, RichEditCommandId.PasteSelection);
			keyboardHandler.RegisterKeyHandler(provider, Keys.I, Keys.Alt, RichEditCommandId.IncrementNumerationFromParagraph);
			keyboardHandler.RegisterKeyHandler(provider, Keys.I, Keys.Alt | Keys.Control, RichEditCommandId.DecrementNumerationFromParagraph);
			keyboardHandler.RegisterKeyHandler(provider, Keys.X, Keys.Control, RichEditCommandId.CutSelection);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Delete, Keys.Shift, RichEditCommandId.CutSelection);
			keyboardHandler.RegisterKeyHandler(provider, Keys.F9, Keys.None, RichEditCommandId.UpdateFields);
			keyboardHandler.RegisterKeyHandler(provider, Keys.F9, Keys.Control, RichEditCommandId.CreateField);
			keyboardHandler.RegisterKeyHandler(provider, Keys.F11, Keys.Control, RichEditCommandId.LockFieldCommand);
			keyboardHandler.RegisterKeyHandler(provider, Keys.F11, Keys.Control | Keys.Shift, RichEditCommandId.UnlockFieldCommand);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Left, Keys.Alt | Keys.Shift, RichEditCommandId.DecrementParagraphOutlineLevel);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Right, Keys.Alt | Keys.Shift, RichEditCommandId.IncrementParagraphOutlineLevel);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Escape, Keys.None, RichEditCommandId.SelectUpperLevelObject);
#if !SL
			keyboardHandler.RegisterKeyHandler(provider, Keys.F3, Keys.None, RichEditCommandId.FindNext);
			keyboardHandler.RegisterKeyHandler(provider, Keys.F3, Keys.Shift, RichEditCommandId.FindPrev);
			keyboardHandler.RegisterKeyHandler(provider, Keys.F, Keys.Control, RichEditCommandId.Find);
			keyboardHandler.RegisterKeyHandler(provider, Keys.H, Keys.Control, RichEditCommandId.Replace);
#if DEBUG
#endif
#endif
			keyboardHandler.RegisterKeyHandler(provider, Keys.U, Keys.Control | Keys.Shift, RichEditCommandId.CollapseOrExpandFormulaBar);
			keyboardHandler.RegisterKeyHandler(provider, Keys.F4, Keys.Shift, RichEditCommandId.ChangeCase);
		}
		#endregion
		protected internal virtual NormalKeyboardHandler CreateDefaultKeyboardHandler() {
			return new NormalKeyboardHandler();
		}
		protected internal virtual void SetNewKeyboardHandler(KeyboardHandler keyboardHandler) {
			FlushPendingTextInput();
			keyboardHandlers.Push(keyboardHandler);
		}
		protected internal virtual void RestoreKeyboardHandler() {
			keyboardHandlers.Pop();
		}
		protected internal virtual void FlushPendingTextInput() {
			if (keyboardHandlers == null || KeyboardHandlers.Count <= 0)
				return;
			NormalKeyboardHandler handler = KeyboardHandler as NormalKeyboardHandler;
			if (handler != null)
				handler.FlushPendingTextInput();
		}
		protected internal virtual bool OnKeyDown(KeyEventArgs e) {
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
		protected internal virtual bool OnKeyUp(KeyEventArgs e) {
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
		protected internal virtual bool OnKeyPress(KeyPressEventArgs e) {
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
	}
}
