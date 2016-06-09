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
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Commands.Internal {
	#region ChangeActivePieceTableCommand
	public class ChangeActivePieceTableCommand : RichEditSelectionCommand {
		readonly PieceTable newActivePieceTable;
		readonly Section section;
		readonly int preferredPageIndex;
		protected ChangeActivePieceTableCommand(IRichEditControl control) : base(control) {
		}
		public ChangeActivePieceTableCommand(IRichEditControl control, PieceTable newActivePieceTable, Section section, int preferredPageIndex)
			: base(control) {
			Guard.ArgumentNotNull(newActivePieceTable, "newActivePieceTable");
			this.newActivePieceTable = newActivePieceTable;
			Debug.Assert(Object.ReferenceEquals(newActivePieceTable.DocumentModel, DocumentModel));
			if (!newActivePieceTable.IsMain)
				this.section = section;
			this.preferredPageIndex = preferredPageIndex;
		}
		#region Properties
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		protected internal override bool TryToKeepCaretX { get { return false; } }
		protected internal override bool TreatStartPositionAsCurrent { get { return false; } }
		protected internal override bool ExtendSelection { get { return false; } }
		protected internal override DocumentLayoutDetailsLevel UpdateCaretPositionBeforeChangeSelectionDetailsLevel { get { return DocumentLayoutDetailsLevel.PageArea; } }
		#endregion
		protected internal override bool PerformChangeSelection() {
			ActivatePieceTable(newActivePieceTable, section);
			return true; 
		}
		protected internal virtual void ActivatePieceTable(PieceTable newPieceTable, Section section) {
			DocumentModel.SetActivePieceTable(newPieceTable, section);
			if (newPieceTable.IsMain) {
				if (CaretPosition.LayoutPosition.IsValid(DocumentLayoutDetailsLevel.Page)) {
					DocumentModelPosition pos = CaretPosition.LayoutPosition.Page.GetFirstPosition(DocumentModel.MainPieceTable);
					SetSelection(pos.LogPosition, 0);
				}
			}
			else
				DocumentModel.Selection.SetStartCell(DocumentModel.Selection.Start);
		}
		protected internal virtual void SetSelection(DocumentLogPosition start, int length) {
			DocumentModel.BeginUpdate();
			try {
				DocumentModel.Selection.Start = start;
				DocumentModel.Selection.End = start + length;
				DocumentModel.Selection.SetStartCell(DocumentModel.Selection.Start);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal override void AfterUpdate() {
			base.AfterUpdate();
			ApplyLayoutPreferredPageIndex(preferredPageIndex);
		}
		protected internal override bool CanChangePosition(DocumentModelPosition pos) {
			return true;
		}
		protected internal override DocumentLogPosition ChangePosition(DocumentModelPosition pos) {
			return pos.LogPosition;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Visible = true;
			state.Checked = true;
			if (newActivePieceTable.IsHeaderFooter) {
				state.Enabled = ActiveViewType == RichEditViewType.PrintLayout;
				ApplyCommandRestrictionOnEditableControl(state, Options.DocumentCapabilities.HeadersFooters, state.Enabled);
				ApplyCommandRestrictionOnEditableControl(state, Options.DocumentCapabilities.Sections, state.Enabled);
			}
			else if (newActivePieceTable.IsTextBox) {
				state.Enabled = true;
				ApplyCommandRestrictionOnEditableControl(state, Options.DocumentCapabilities.FloatingObjects, state.Enabled);
			}
		}
	}
	#endregion
	#region SelectUpperLevelObjectCommand
	public class SelectUpperLevelObjectCommand : ChangeActivePieceTableCommand {
		public SelectUpperLevelObjectCommand(IRichEditControl control)
			: base(control) {
		}
		public override RichEditCommandId Id { get { return RichEditCommandId.SelectUpperLevelObject; } }
		protected internal override bool PerformChangeSelection() {
			if (ActivePieceTable.IsTextBox) {
				TextBoxContentType textBoxPieceTable = ActivePieceTable.ContentType as TextBoxContentType;
				PieceTable anchorPieceTable = textBoxPieceTable.AnchorRun.PieceTable;
				Section section = null;
				if (anchorPieceTable.IsHeaderFooter)
					section = GetCurrentSectionFromCaretLayoutPosition();
				ActivatePieceTable(anchorPieceTable, section);
				SetSelection(anchorPieceTable.GetRunLogPosition(textBoxPieceTable.AnchorRun), 1);
				return true;
			}
			else {
				if (!ActivePieceTable.IsMain) {
					if (ResetCurrentObjectSelection())
						return true;
					ActivatePieceTable(DocumentModel.MainPieceTable, null);
					return true;
				}
				else
					return ResetCurrentObjectSelection();
			}
		}
		protected internal virtual bool ResetCurrentObjectSelection() {
			if (!CanResetCurrentObjectSelection())
				return false;
			SetSelection(DocumentModel.Selection.NormalizedStart, 0);
			return true;
		}
		protected internal virtual bool CanResetCurrentObjectSelection() {
			if (DocumentModel.Selection.Length != 1)
				return false;
			RunInfo runInfo = ActivePieceTable.FindRunInfo(DocumentModel.Selection.NormalizedStart, 1);
			TextRunBase run = ActivePieceTable.Runs[runInfo.Start.RunIndex];
			if (!(run is FloatingObjectAnchorRun || run is InlinePictureRun))
				return false;
			return true;
		}
		protected internal Section GetCurrentSectionFromCaretLayoutPosition() {
			UpdateCaretPosition();
			if (!CaretPosition.LayoutPosition.IsValid(DocumentLayoutDetailsLevel.PageArea))
				return null;
			return CaretPosition.LayoutPosition.PageArea.Section;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Visible = true;
			if (DocumentModel.Selection.Length == 1) {
				RunInfo runInfo = ActivePieceTable.FindRunInfo(DocumentModel.Selection.NormalizedStart, 1);
				TextRunBase run = ActivePieceTable.Runs[runInfo.Start.RunIndex];
				state.Enabled = !ActivePieceTable.IsMain || (run is FloatingObjectAnchorRun || run is InlinePictureRun);
			}
			else
				state.Enabled = !ActivePieceTable.IsMain;
			state.Checked = true;
		}
	}
	#endregion
}
