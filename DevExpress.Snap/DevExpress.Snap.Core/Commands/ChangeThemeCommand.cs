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
using System.Linq;
using System.Text;
using DevExpress.XtraRichEdit;
using DevExpress.Utils.Commands;
using DevExpress.Snap.Core.Native.Services;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.Snap.Core.Native;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.Utils;
namespace DevExpress.Snap.Core.Commands {
	[CommandLocalization(Localization.SnapStringId.ChangeThemeCommand_MenuCaption, Localization.SnapStringId.ChangeThemeCommand_Description)]
	public class ChangeThemeCommand : SnapMenuItemSimpleCommand {
		DefaultValueBasedCommandUIState<string> uiState;
		public ChangeThemeCommand(IRichEditControl control)
			: base(control) {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandsRestriction(state, CharacterFormatting);
		}
		public override void ForceExecute(ICommandUIState state) {
			uiState = state as DefaultValueBasedCommandUIState<string>;
			base.ForceExecute(state);
		}
		protected internal override void ExecuteCore() {
			if (uiState == null || uiState.Value == null)
				return;
			this.DocumentModel.ApplyTheme(uiState.Value);
		}
		protected internal virtual DocumentModelPosition CalculateStartPosition(SelectionItem selection, bool allowSelectionExpanding) {
			DocumentModelPosition result;
			if (selection.Start < selection.End)
				result = selection.Interval.Start;
			else if (selection.Start > selection.End)
				result = selection.Interval.End;
			else {
				DocumentModelPosition start = selection.Interval.Start;
				if (allowSelectionExpanding) {
					WordsDocumentModelIterator iterator = new WordsDocumentModelIterator(ActivePieceTable);
					if (!iterator.IsInsideWord(start) || iterator.IsNewElement(start))
						result = start;
					else
						result = iterator.MoveBack(start);
				}
				else
					result = start;
			}
			return result;
		}
		protected internal virtual DocumentModelPosition CalculateEndPosition(SelectionItem selection, bool allowSelectionExpanding) {
			DocumentModelPosition result;
			if (selection.Start < selection.End)
				result = selection.Interval.End;
			else if (selection.Start > selection.End)
				result = selection.Interval.Start;
			else {
				if (allowSelectionExpanding) {
					WordsDocumentModelIterator iterator = new WordsDocumentModelIterator(ActivePieceTable);
					if (!iterator.IsInsideWord(selection.Interval.End) || iterator.IsNewElement(selection.Interval.End))
						result = selection.Interval.End;
					else
						result = iterator.MoveForward(selection.Interval.End);
				}
				else
					result = selection.Interval.Start;
			}
			return result;
		}
	}
	public class ChangeThemeCoreCommand : RichEditMenuItemSimpleCommand {
		readonly Theme theme;
		public ChangeThemeCoreCommand(ISnapControl control, Theme theme)
			: base(control) {
			Guard.ArgumentNotNull(theme, "theme");
			this.theme = theme;
		}
		protected internal new SnapDocumentModel DocumentModel { get { return (SnapDocumentModel)base.DocumentModel; } }
		public override XtraRichEditStringId DescriptionStringId {
			get { throw new NotImplementedException(); }
		}
		public override XtraRichEditStringId MenuCaptionStringId {
			get { throw new NotImplementedException(); }
		}
		public override string Description { get { return theme.Name; } }
		public override string MenuCaption { get { return theme.Name; } }
		protected internal override void ExecuteCore() {
			if (!Object.ReferenceEquals(theme, DocumentModel.ActiveTheme))
				DocumentModel.ApplyTheme(theme);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, DocumentModel.DocumentCapabilities.TableStyle);
			ApplyCommandRestrictionOnEditableControl(state, DocumentModel.DocumentCapabilities.TableCellStyle);
		}
	}
}
