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
using DevExpress.Utils.Commands;
using DevExpress.Office.History;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Commands.Internal;
namespace DevExpress.XtraRichEdit.Commands {
	#region ReplaceAllCommandBase (abstract class)
	public abstract class ReplaceAllCommandBase : SearchCommandBase {
		protected ReplaceAllCommandBase(IRichEditControl control)
			: base(control) {
		}
		protected override SearchAction Action { get { return SearchAction.ReplaceAll; } }
		protected internal override void PopulateCommands(CommandCollection commands) {
			commands.Add(new BeginReplaceAllCoreCommand(Control));
			commands.Add(CreateReplaceAllCommand());
		}
		protected internal abstract ReplaceAllCoreCommand CreateReplaceAllCommand();
	}
	#endregion
	#region ReplaceAllForwardCommand
	public class ReplaceAllForwardCommand : ReplaceAllCommandBase {
		public ReplaceAllForwardCommand(IRichEditControl control)
			: base(control) {
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ReplaceAllForward; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ReplaceAllForwardDescription; } }
		public override RichEditCommandId Id { get { return RichEditCommandId.ReplaceAllForward; } }
		protected override Direction Direction { get { return Direction.Forward; } }
		protected internal override ReplaceAllCoreCommand CreateReplaceAllCommand() {
			return new ReplaceAllForwardCoreCommand(Control);
		}
	}
	#endregion
	#region ReplaceAllBackwardCommand
	public class ReplaceAllBackwardCommand : ReplaceAllCommandBase {
		public ReplaceAllBackwardCommand(IRichEditControl control)
			: base(control) {
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ReplaceAllBackward; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ReplaceAllBackwardDescription; } }
		protected override Direction Direction { get { return Direction.Backward; } }
		protected internal override ReplaceAllCoreCommand CreateReplaceAllCommand() {
			return new ReplaceAllBackwardCoreCommand(Control);
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Commands.Internal {
	#region BeginReplaceAllCoreCommand
	public class BeginReplaceAllCoreCommand : RichEditSelectionCommand {
		public BeginReplaceAllCoreCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		protected SearchContext SearchContext { get { return DocumentModel.SearchContext; } }
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		protected internal override bool TryToKeepCaretX { get { return true; } }
		protected internal override bool TreatStartPositionAsCurrent { get { return true; } }
		protected internal override bool ExtendSelection { get { return true; } }
		protected internal override DocumentLayoutDetailsLevel UpdateCaretPositionBeforeChangeSelectionDetailsLevel { get { return DocumentLayoutDetailsLevel.None; } }
		#endregion
		protected internal override bool CanChangePosition(DocumentModelPosition pos) {
			return true;
		}
		protected internal override DocumentLogPosition ChangePosition(DocumentModelPosition pos) {
			return pos.LogPosition;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = SearchContext.StartOfSearch;
			state.Visible = true;
		}
		protected internal override void ChangeSelection(Selection selection) {
			selection.Start = selection.End;
			base.ChangeSelection(selection);
		}
	}
	#endregion
	#region ReplaceAllCoreCommand (abstract class)
	public abstract class ReplaceAllCoreCommand : RichEditMenuItemSimpleCommand {
		readonly ReplaceCoreCommand innerReplaceCommand;
		protected ReplaceAllCoreCommand(IRichEditControl control)
			: base(control) {
			this.innerReplaceCommand = CreateReplaceCommand();
		}
		#region Properties
		protected SearchParameters SearchParameters { get { return DocumentModel.SearchParameters; } }
		protected SearchContext SearchContext { get { return DocumentModel.SearchContext; } }
		protected ReplaceCoreCommand InnerReplaceCommand { get { return innerReplaceCommand; } }
		#endregion
		protected virtual ReplaceCoreCommand CreateReplaceCommand() {
			return new ReplaceCoreCommand(Control);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = IsContentEditable;
			state.Visible = true;
		}
		protected internal override void ExecuteCore() {
			if (String.IsNullOrEmpty(SearchParameters.SearchString))
				return;
			using (HistoryTransaction transaction = new HistoryTransaction(DocumentModel.History)) {
				for (; ; ) {
					DoReplaceAll();
					SearchCompleteEventArgs e = SearchContext.CreateEventArgs(SearchParameters.SearchString, SearchParameters.ReplaceString);
					InnerControl.RaiseSearchComplete(e);
					if (!e.Continue || e.EntireDocument)
						break;
				}
			}
		}
		protected internal virtual void OnAfterExecute() {
			SearchCompleteEventArgs e = SearchContext.CreateEventArgs(SearchParameters.SearchString, SearchParameters.ReplaceString);
			InnerControl.RaiseSearchComplete(e);
		}
		protected internal virtual void DoReplaceAll() {
			Control.BeginUpdate();
			try {
				DocumentModel.BeginUpdate();
				try {
					TextSearchProvider searchProvider = CreateTextSearchProvider();
					while (true) {
						RunInfo result = searchProvider.FindNextPosition();
						if (result == null)
							break;
						if (result.Start == result.End)
							continue;
						DocumentLogPosition start = result.Start.LogPosition;
						int length = result.End.LogPosition - start;
						if (CanEdit(start, length))
							InnerReplaceCommand.ReplaceText(start, length);
					}
				}
				finally {
					DocumentModel.EndUpdate();
				}
			}
			finally {
				Control.EndUpdate();
			}
		}
		protected virtual bool CanEdit(DocumentLogPosition start, int length) {
			if (!DocumentModel.IsDocumentProtectionEnabled)
				return true;
			return ActivePieceTable.CanEditRange(start, length);
		}
		protected abstract TextSearchProvider CreateTextSearchProvider();
	}
	#endregion
	#region ReplaceAllForwardCoreCommand
	public class ReplaceAllForwardCoreCommand : ReplaceAllCoreCommand {
		public ReplaceAllForwardCoreCommand(IRichEditControl control)
			: base(control) {
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		public override RichEditCommandId Id { get { return RichEditCommandId.ReplaceAllForward; } }
		protected override TextSearchProvider CreateTextSearchProvider() {
			return new TextSearchForwardProvider(ActivePieceTable, SearchParameters, SearchContext);
		}
	}
	#endregion
	#region ReplaceAllBackwardCoreCommand
	public class ReplaceAllBackwardCoreCommand : ReplaceAllCoreCommand {
		public ReplaceAllBackwardCoreCommand(IRichEditControl control)
			: base(control) {
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		protected override TextSearchProvider CreateTextSearchProvider() {
			return new TextSearchBackwardProvider(ActivePieceTable, SearchParameters, SearchContext);
		}
	}
	#endregion
}
