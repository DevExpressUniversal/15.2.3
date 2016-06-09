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
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Model;
using System.Collections.Generic;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraRichEdit.SpellChecker;
using System.ComponentModel;
using DevExpress.XtraRichEdit.Model.History;
using DevExpress.XtraRichEdit.Layout;
namespace DevExpress.XtraRichEdit.Commands {
	#region ShowHyperlinkFormCommand
	public class ShowHyperlinkFormCommand : MultiCommand {
		public ShowHyperlinkFormCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		protected internal override MultiCommandExecutionMode ExecutionMode { get { return MultiCommandExecutionMode.ExecuteAllAvailable; } }
		protected internal override MultiCommandUpdateUIStateMode UpdateUIStateMode { get { return MultiCommandUpdateUIStateMode.EnableIfAllAvailable; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowHyperlinkFormCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.ShowHyperlinkForm; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowHyperlinkFormCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ShowHyperlinkForm; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowHyperlinkFormCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ShowHyperlinkFormDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowHyperlinkFormCommandImageName")]
#endif
		public override string ImageName { get { return "Hyperlink"; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowHyperlinkFormCommandShowsModalDialog")]
#endif
		public override bool ShowsModalDialog { get { return true; } }
		#endregion
		protected internal override void CreateCommands() {
			Commands.Add(new SelectTextForHyperlinkCommand(Control));
			Commands.Add(new ShowHyperlinkFormCoreCommand(Control));
		}
	}
	#endregion
	#region SelectTextForHyperlinkCommand
	public class SelectTextForHyperlinkCommand : RichEditSelectionCommand {
		public SelectTextForHyperlinkCommand(IRichEditControl control)
			: base(control) {
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		protected internal override bool TryToKeepCaretX { get { return false; } }
		protected internal override bool TreatStartPositionAsCurrent { get { return false; } }
		protected internal override bool ExtendSelection { get { return true; } }
		protected internal override DocumentLayoutDetailsLevel UpdateCaretPositionBeforeChangeSelectionDetailsLevel { get { return DocumentLayoutDetailsLevel.None; } }
		protected internal override void ChangeSelectionStart(Selection selection, DocumentLogPosition logPosition) {
			SpellCheckerWordIterator iterator = new SpellCheckerWordIterator(ActivePieceTable);
			selection.Start = iterator.MoveToWordStart(selection.Interval.NormalizedStart, false).LogPosition;
		}
		protected internal override bool PerformChangeSelection() {
			if (DocumentModel.Selection.Length > 0)
				return false;
			Field field = ActivePieceTable.FindFieldByRunIndex(DocumentModel.Selection.Interval.Start.RunIndex);
			if (field != null && ActivePieceTable.IsHyperlinkField(field))
				return false;
			return base.PerformChangeSelection();
		}
		protected internal override bool CanChangePosition(DocumentModelPosition pos) {
			return true;
		}
		protected internal override DocumentLogPosition ChangePosition(DocumentModelPosition pos) {
			SpellCheckerWordIterator iterator = new SpellCheckerWordIterator(ActivePieceTable);
			DocumentModelPosition result = pos.Clone();
			iterator.MoveToWordEndCore(result);
			return result.LogPosition;
		}
	}
	#endregion
	#region ShowHyperlinkFormCoreCommand
	public class ShowHyperlinkFormCoreCommand : RichEditSelectionCommand {
		Field hyperlinkField;
		bool shouldChangeSelection;
		public ShowHyperlinkFormCoreCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		protected internal override bool TryToKeepCaretX { get { return false; } }
		protected internal override bool TreatStartPositionAsCurrent { get { return false; } }
		protected internal override bool ExtendSelection { get { return false; } }
		protected internal override DocumentLayoutDetailsLevel UpdateCaretPositionBeforeChangeSelectionDetailsLevel { get { return DocumentLayoutDetailsLevel.None; } }
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		#endregion
		protected internal override void ExecuteCore() {
			List<Field> fields = GetSelectedHyperlinkFields();
			if (fields.Count == 0)
				ShowInsertHyperlinkForm();
			else {
				hyperlinkField = fields[0];
				ShowEditHyperlinkForm();
			}
			base.ExecuteCore();
		}
		protected internal virtual void ShowInsertHyperlinkForm() {
			Selection selection = DocumentModel.Selection;
			string title = XtraRichEditLocalizer.GetString(XtraRichEditStringId.HyperlinkForm_InsertHyperlinkTitle);
			RunInfo runInfo = selection.Interval.Clone();
			Control.ShowHyperlinkForm(new HyperlinkInfo(), runInfo, title, CreateHyperlink);
		}
		protected internal virtual void ShowEditHyperlinkForm() {
			HyperlinkInfo hyperlinkInfo = ActivePieceTable.HyperlinkInfos[hyperlinkField.Index].Clone();
			RunInfo runInfo = new RunInfo(ActivePieceTable);
			DocumentModelPosition.SetRunStart(runInfo.Start, hyperlinkField.Result.Start);
			DocumentModelPosition.SetRunStart(runInfo.End, hyperlinkField.Result.End);
			string title = XtraRichEditLocalizer.GetString(XtraRichEditStringId.HyperlinkForm_EditHyperlinkTitle);
			if (hyperlinkField.IsCodeView)
				ActivePieceTable.ToggleFieldCodes(hyperlinkField);
			Control.ShowHyperlinkForm(hyperlinkInfo, runInfo, title, ChangeHyperlink);
		}
		protected List<Field> GetSelectedHyperlinkFields() {
			RunInfo interval = DocumentModel.Selection.Interval;
			DocumentModelPosition start = interval.NormalizedStart;
			DocumentModelPosition end = interval.NormalizedEnd;
			if (end > start)
				end = DocumentModelPosition.MoveBackward(end);
			return ActivePieceTable.GetHyperlinkFields(start.RunIndex, end.RunIndex);
		}
		protected internal virtual void CreateHyperlink(HyperlinkInfo hyperlinkInfo, TextToDisplaySource source, RunInfo runInfo, string text) {
			if (source == TextToDisplaySource.ExistingText) {
				DocumentLogPosition start = runInfo.NormalizedStart.LogPosition;
				int length = runInfo.NormalizedEnd.LogPosition - start;
				CreateHyperlinkCommand cmd = new CreateHyperlinkCommand(Control, hyperlinkInfo, start, length);
				cmd.Execute();
			}
			else {
				InsertHyperlinkCommand cmd = new InsertHyperlinkCommand(Control);
				cmd.Text = text;
				cmd.HyperlinkInfo = hyperlinkInfo;
				cmd.Execute();
			}
			shouldChangeSelection = true;
		}
		protected internal virtual void ChangeHyperlink(HyperlinkInfo hyperlinkInfo, TextToDisplaySource source, RunInfo runInfo, string text) {
			if (source == TextToDisplaySource.NewText)
				ActivePieceTable.ModifyHyperlinkResult(hyperlinkField, text);
			ActivePieceTable.ModifyHyperlinkCode(hyperlinkField, hyperlinkInfo);
			shouldChangeSelection = true;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			ApplyCommandRestriction(state);
			List<Field> hyperlinks = GetSelectedHyperlinkFields();
			bool canShowHyperlinkForm = (hyperlinks.Count == 0) || (hyperlinks.Count == 1 && (!hyperlinks[0].HideByParent || hyperlinks[0].Parent == null));
			state.Enabled = state.Enabled  && !DocumentModel.Selection.IsMultiSelection && IsTableCorrectlySelected() && canShowHyperlinkForm;
		}
		protected void ApplyCommandRestriction(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, Options.DocumentCapabilities.Hyperlinks);
			ApplyDocumentProtectionToSelectedCharacters(state);
		}
		protected internal virtual bool IsTableCorrectlySelected() {
			Selection selection = DocumentModel.Selection;
			TableCell startCell = ActivePieceTable.FindParagraph(selection.NormalizedStart).GetCell();
			TableCell endCell = ActivePieceTable.FindParagraph(selection.NormalizedVirtualEnd).GetCell();
			if (startCell == null && endCell == null)
				return true;
			bool isNotSelectedCellMark = endCell == null ? false : ActivePieceTable.Paragraphs[endCell.EndParagraphIndex].EndLogPosition + 1 != selection.NormalizedEnd;
			if (startCell == endCell && isNotSelectedCellMark)
				return true;
			if (startCell != null && endCell == null)
				return true;
			if (startCell != null && endCell != null && startCell.Table.NestedLevel == endCell.Table.NestedLevel + 1 && isNotSelectedCellMark)
				return true;
			return false;
		}
		protected internal override bool PerformChangeSelection() {
			if (shouldChangeSelection)
				return base.PerformChangeSelection();
			else
				return false;
		}
		protected internal override bool CanChangePosition(DocumentModelPosition pos) {
			return true;
		}
		DocumentLogPosition GetFieldEndPostition(Field field) {
			return DocumentModelPosition.FromRunEnd(ActivePieceTable, field.Result.End).LogPosition;
		}
		protected internal override DocumentLogPosition ChangePosition(DocumentModelPosition pos) {
			if (hyperlinkField != null)
				return GetFieldEndPostition(hyperlinkField);
			else {
				Field field = ActivePieceTable.FindFieldByRunIndex(pos.RunIndex);
				if (field != null)
					return GetFieldEndPostition(field);
			}
			return pos.LogPosition;
		}
	}
	#endregion
	#region EditHyperlinkCommand
	public class EditHyperlinkCommand : ShowHyperlinkFormCommand {
		public EditHyperlinkCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("EditHyperlinkCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_EditHyperlink; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("EditHyperlinkCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_EditHyperlinkDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("EditHyperlinkCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.EditHyperlink; } }
	}
	#endregion
	#region CreateHyperlinkContextMenuItemCommand
	public class CreateHyperlinkContextMenuItemCommand : ShowHyperlinkFormCommand {
		public CreateHyperlinkContextMenuItemCommand(IRichEditControl control)
			: base(control) {
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_Hyperlink; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_HyperlinkDescription; } }
		public override RichEditCommandId Id { get { return RichEditCommandId.CreateHyperlink; } }
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
		}
	}
	#endregion
}
