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
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Native;
using DevExpress.XtraRichEdit.Model.History;
using DevExpress.XtraRichEdit.Commands.Internal;
using System.Globalization;
namespace DevExpress.XtraRichEdit.Commands {
	#region ReplaceCommandBase (abstract class)
	public abstract class ReplaceCommandBase : SearchCommandBase {
		protected ReplaceCommandBase(IRichEditControl control)
			: base(control) {
		}
		protected override SearchAction Action { get { return SearchAction.Replace; } }
		protected internal override void PopulateCommands(CommandCollection commands) {
			commands.Add(new ReplaceCoreCommand(Control));
			commands.Add(CreateFindCommand());
		}
		protected internal abstract FindAndSelectCoreCommandBase CreateFindCommand();
	}
	#endregion
	#region ReplaceForwardCommand
	public class ReplaceForwardCommand : ReplaceCommandBase {
		public ReplaceForwardCommand(IRichEditControl control)
			: base(control) {
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ReplaceForward; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ReplaceForwardDescription; } }
		public override RichEditCommandId Id { get { return RichEditCommandId.ReplaceForward; } }
		protected override Direction Direction { get { return Direction.Forward; } }
		protected internal override FindAndSelectCoreCommandBase CreateFindCommand() {
			return new FindAndSelectForwardCoreCommand(Control);
		}
	}
	#endregion
	#region ReplaceBackwardCommand
	public class ReplaceBackwardCommand : ReplaceCommandBase {
		public ReplaceBackwardCommand(IRichEditControl control)
			: base(control) {
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ReplaceBackward; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ReplaceForwardDescription; } }
		protected override Direction Direction { get { return Direction.Backward; } }
		protected internal override FindAndSelectCoreCommandBase CreateFindCommand() {
			return new FindAndSelectBackwardCoreCommand(Control);
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Commands.Internal {
	#region RichEditMenuItemSimpleCommand
	public class ReplaceTextCommand : RichEditMenuItemSimpleCommand {
		int length = -1;
		string text = String.Empty;
		DocumentLogPosition pos = new DocumentLogPosition(-1);
		public ReplaceTextCommand(IRichEditControl control, DocumentLogPosition pos, int length, string text)
			: base(control) {
			this.pos = pos;
			this.length = length;
			this.text = text;
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ReplaceText; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ReplaceTextDescription; } }
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = IsContentEditable && ActivePieceTable.CanEditRange(pos, length);
			state.Visible = true;
		}
		protected internal override void ExecuteCore() {
			ActivePieceTable.ReplaceTextAndInheritFormatting(pos, length, text);
		}
	}
	#endregion
	#region ReplaceCoreCommand
	public class ReplaceCoreCommand : RichEditMenuItemSimpleCommand {
		public ReplaceCoreCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		protected SearchParameters SearchParameters { get { return DocumentModel.SearchParameters; } }
		protected SearchContext SearchContext { get { return DocumentModel.SearchContext; } }
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		#endregion
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = IsContentEditable && CanReplaceSelectedText();
			state.Visible = true;
		}
		protected internal override void ExecuteCore() {
			CheckExecutedAtUIThread();
			Selection selection = DocumentModel.Selection;
			ReplaceText(selection.NormalizedStart, selection.Length);
			ActiveView.EnsureCaretVisible();
		}
		protected internal virtual bool CanReplaceSelectedText() {
			if (DocumentModel.Selection.Length == 0 || String.IsNullOrEmpty(SearchParameters.SearchString))
				return false;
			if (!CanEditSelection())
				return false;
			if (!SearchContext.StartOfSearch)
				return true;
			return (!SearchParameters.UseRegularExpression && CompareStringWithSelectedText(SearchParameters.SearchString));
		}
		bool CompareStringWithSelectedText(string text) {
			RunInfo selection = DocumentModel.Selection.Interval;
			int length = text.Length;
			BufferedDocumentCharacterIteratorForward iterator = new BufferedDocumentCharacterIteratorForward(selection.NormalizedStart);
			iterator.AppendBuffer(length);
			if (!iterator.IsCharacterExist(length - 1) || iterator.GetPosition(length) != selection.NormalizedEnd)
				return false;
			for (int offset = 0; offset < length; offset++) {
				if (!iterator.Compare(offset, text[offset], true))
					return false;
			}
			return true;
		}
		protected internal virtual void ReplaceText(DocumentLogPosition startPos, int length) {
			string replacement = SearchParameters.ReplaceString;
			if (SearchParameters.UseRegularExpression && SearchContext.MatchInfo != null)
				replacement = SearchContext.MatchInfo.Match.Result(replacement);
			ReplaceInnerCommand command = Control.CreateCommand(RichEditCommandId.InnerReplace) as ReplaceInnerCommand;
			command.StartPosition = startPos;
			command.Length = length;
			command.Replacement = replacement;
			command.Execute();
			SearchContext.ReplaceCount++;
		}
	}
	#endregion
}
