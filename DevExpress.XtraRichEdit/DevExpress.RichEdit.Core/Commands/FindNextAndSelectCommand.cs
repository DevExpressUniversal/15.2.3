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
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Native;
using DevExpress.XtraRichEdit.Commands.Internal;
namespace DevExpress.XtraRichEdit.Commands {
	#region SearchCommandBase (abstract class)
	public abstract class SearchCommandBase : MultiCommand {
		protected SearchCommandBase(IRichEditControl control)
			: base(control) {
		}
		protected internal override MultiCommandExecutionMode ExecutionMode { get { return MultiCommandExecutionMode.ExecuteAllAvailable; } }
		protected internal override MultiCommandUpdateUIStateMode UpdateUIStateMode { get { return MultiCommandUpdateUIStateMode.EnableIfAnyAvailable; } }
		protected SearchContext SearchContext { get { return DocumentModel.SearchContext; } }
		protected abstract SearchAction Action { get; }
		protected abstract Direction Direction { get; }
		public override void ForceExecute(ICommandUIState state) {
			SearchContext.BeginSearch(Action, Direction);
			try {
				base.ForceExecute(state);
			}
			catch (IncorrectRegularExpressionException e) {
				Control.ShowWarningMessage(e.Message);
			}
			finally {
				SearchContext.EndSearch();
			}
		}
		protected internal override void CreateCommands() {
			PopulateCommands(Commands);
			Commands.Add(CreateEndSearchCommand());
		}
		protected internal virtual EndSearchCoreCommand CreateEndSearchCommand() {
			return new EndSearchCoreCommand(Control);
		}
		protected internal abstract void PopulateCommands(CommandCollection commands);
	}
	#endregion
	#region FindNextAndSelectCommandBase
	public abstract class FindNextAndSelectCommandBase : SearchCommandBase {
		protected FindNextAndSelectCommandBase(IRichEditControl control)
			: base(control) {
		}
		protected override SearchAction Action { get { return SearchAction.Find; } }
		protected internal override void PopulateCommands(CommandCollection commands) {
			commands.Add(CreateFindCommand());
		}
		protected internal abstract FindAndSelectCoreCommandBase CreateFindCommand();
	}
	#endregion
	#region FindAndSelectForwardCommand
	public class FindAndSelectForwardCommand : FindNextAndSelectCommandBase {
		public FindAndSelectForwardCommand(IRichEditControl control)
			: base(control) {
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_FindAndSelectForward; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_FindAndSelectForwardDescription; } }
		public override RichEditCommandId Id { get { return RichEditCommandId.FindForward; } }
		protected override Direction Direction { get { return Direction.Forward; } }
		protected internal override FindAndSelectCoreCommandBase CreateFindCommand() {
			return new FindAndSelectForwardCoreCommand(Control);
		}
	}
	#endregion
	#region FindAndSelectBackwardCommand
	public class FindAndSelectBackwardCommand : FindNextAndSelectCommandBase {
		public FindAndSelectBackwardCommand(IRichEditControl control)
			: base(control) {
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_FindAndSelectBackward; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_FindAndSelectBackwardDescription; } }
		public override RichEditCommandId Id { get { return RichEditCommandId.FindBackward; } }
		protected override Direction Direction { get { return Direction.Backward; } }
		protected internal override FindAndSelectCoreCommandBase CreateFindCommand() {
			return new FindAndSelectBackwardCoreCommand(Control);
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Commands.Internal {
	#region EndSearchCoreCommand
	public class EndSearchCoreCommand : RichEditSelectionCommand {
		public EndSearchCoreCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		protected SearchContext SearchContext { get { return DocumentModel.SearchContext; } }
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		protected internal override bool TryToKeepCaretX { get { return false; } }
		protected internal override bool TreatStartPositionAsCurrent { get { return true; } }
		protected internal override bool ExtendSelection { get { return true; } }
		protected internal override Layout.DocumentLayoutDetailsLevel UpdateCaretPositionBeforeChangeSelectionDetailsLevel { get { return Layout.DocumentLayoutDetailsLevel.None; } }
		#endregion
		protected internal override bool CanChangePosition(DocumentModelPosition pos) {
			return true;
		}
		protected internal override DocumentLogPosition ChangePosition(DocumentModelPosition pos) {
			return pos.LogPosition;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = SearchContext.EndOfSearch;
			state.Visible = true;
		}
		protected internal override void ChangeSelection(Selection selection) {
			selection.Start = SearchContext.StartSelectionAnchor.Position.LogPosition;
			selection.End = SearchContext.EndSelectionAnchor.Position.LogPosition;
			base.ChangeSelection(selection);
		}
	}
	#endregion
	#region FindAndSelectCoreCommandBase (abstract class)
	public abstract class FindAndSelectCoreCommandBase : RichEditSelectionCommand {
		protected FindAndSelectCoreCommandBase(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		protected internal override bool TryToKeepCaretX { get { return false; } }
		protected internal override bool TreatStartPositionAsCurrent { get { return true; } }
		protected internal override bool ExtendSelection { get { return true; } }
		protected internal override DocumentLayoutDetailsLevel UpdateCaretPositionBeforeChangeSelectionDetailsLevel { get { return DocumentLayoutDetailsLevel.None; } }
		protected SearchParameters SearchParameters { get { return DocumentModel.SearchParameters; } }
		protected SearchContext SearchContext { get { return DocumentModel.SearchContext; } }
		#endregion
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Checked = false;
			state.Enabled = !String.IsNullOrEmpty(DocumentModel.SearchParameters.SearchString);
			state.Visible = true;
		}
		protected internal override void ChangeSelection(Selection selection) {
			if (String.IsNullOrEmpty(SearchParameters.SearchString))
				return;
			for (; ; ) {
				RunInfo result = FindNextPosition();
				if (result != null) {
					selection.Start = result.Start.LogPosition;
					selection.End = result.End.LogPosition;
					break;
				}
				else {
					SearchCompleteEventArgs e = SearchContext.CreateEventArgs(SearchParameters.SearchString, SearchParameters.ReplaceString);
					InnerControl.RaiseSearchComplete(e);
					if (!e.EntireDocument && e.Continue)
						continue;
					else
						break;
				}
			}
			base.ChangeSelection(selection);
		}
		protected internal virtual RunInfo FindNextPosition() {
			TextSearchProvider searchProvider = CreateTextSearchProvider();
			return searchProvider.FindNextPosition();
		}
		protected internal override DocumentLogPosition ChangePosition(DocumentModelPosition pos) {
			return pos.LogPosition;
		}
		protected internal override bool CanChangePosition(DocumentModelPosition pos) {
			return true;
		}
		protected abstract TextSearchProvider CreateTextSearchProvider();
	}
	#endregion
	#region FindAndSelectForwardCoreCommand
	public class FindAndSelectForwardCoreCommand : FindAndSelectCoreCommandBase {
		public FindAndSelectForwardCoreCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		#endregion
		protected override TextSearchProvider CreateTextSearchProvider() {
			return new TextSearchForwardProvider(ActivePieceTable, SearchParameters, SearchContext);
		}
	}
	#endregion
	#region FindAndSelectBackwardCoreCommand
	public class FindAndSelectBackwardCoreCommand : FindAndSelectCoreCommandBase {
		public FindAndSelectBackwardCoreCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		#endregion
		protected override TextSearchProvider CreateTextSearchProvider() {
			return new TextSearchBackwardProvider(ActivePieceTable, SearchParameters, SearchContext);
		}
	}
	#endregion
}
