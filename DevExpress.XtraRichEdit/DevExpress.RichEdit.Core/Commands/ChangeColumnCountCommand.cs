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
using System.ComponentModel;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit;
namespace DevExpress.XtraRichEdit.Commands {
	#region SetSectionColumnCountCommandBase (abstract class)
	public abstract class SetSectionColumnCountCommandBase : SelectionBasedPropertyChangeCommandBase {
		protected SetSectionColumnCountCommandBase(IRichEditControl control)
			: base(control) {
		}
		protected internal abstract int ActualColumnCount { get; }
		protected internal override DocumentModelChangeActions ChangeProperty(DocumentModelPosition start, DocumentModelPosition end, ICommandUIState state) {
			return DocumentModelChangeActions.None;
		}
		protected internal override void ModifyDocumentModelCore(ICommandUIState state) {
			DocumentModel documentModel = DocumentModel;
			Section section = documentModel.GetActiveSectionBySelectionEnd();
			if (section == null)
				return;
			SectionColumns columns = section.Columns;
			if (ActualColumnCount != columns.ColumnCount) {
				columns.ColumnCount = ActualColumnCount;
				columns.EqualWidthColumns = true;
				ParagraphIndex startParagraphIndex = section.FirstParagraphIndex;
				ParagraphIndex endParagraphIndex = section.LastParagraphIndex;
				PieceTable pieceTable = documentModel.MainPieceTable;
				ParagraphCollection paragraphs = pieceTable.Paragraphs;
				pieceTable.ApplyChanges(DocumentModelChangeType.ModifySection, paragraphs[startParagraphIndex].FirstRunIndex, paragraphs[endParagraphIndex].LastRunIndex);
			}
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			Section section = DocumentModel.GetActiveSectionBySelectionEnd();
			SectionColumns columns = section.Columns;
			state.Enabled = IsContentEditable && section != null && DocumentModel.CanEditSection(section);
			state.Checked = columns.EqualWidthColumns && columns.ColumnCount == ActualColumnCount;
			state.Visible = true;
			ApplyCommandRestrictionOnEditableControl(state, DocumentModel.DocumentCapabilities.Sections, state.Enabled);
			if (section != null)
				UpdateUIStateCore(state, section);
		}
		protected internal virtual void UpdateUIStateCore(ICommandUIState state, Section section) {
		}
	}
	#endregion
	#region ChangeColumnCountCommand
	public class ChangeColumnCountCommand : SetSectionColumnCountCommandBase {
		int columnCount;
		public ChangeColumnCountCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public int ColumnCount { get { return columnCount; } set { columnCount = value; } }
		protected internal override int ActualColumnCount { get { return ColumnCount; } }
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ChangeColumnCount; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ChangeColumnCountDescription; } }
		#endregion
		public override void ForceExecute(ICommandUIState state) {
			IValueBasedCommandUIState<int> valueBasedState = state as IValueBasedCommandUIState<int>;
			if (valueBasedState != null)
				ColumnCount = valueBasedState.Value;
			base.ForceExecute(state);
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			return new DefaultValueBasedCommandUIState<int>();
		}
		protected internal override void UpdateUIStateCore(ICommandUIState state, Section section) {
			base.UpdateUIStateCore(state, section);
			IValueBasedCommandUIState<int> valueBasedState = state as IValueBasedCommandUIState<int>;
			if (valueBasedState != null)
				valueBasedState.Value = section.Columns.ColumnCount;
		}
	}
	#endregion
	#region SetSectionOneColumnCommand
	public class SetSectionOneColumnCommand : SetSectionColumnCountCommandBase {
		public SetSectionOneColumnCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetSectionOneColumnCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_SetSectionOneColumn; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetSectionOneColumnCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_SetSectionOneColumnDescription; } }
		protected internal override int ActualColumnCount { get { return 1; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetSectionOneColumnCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.SetSectionOneColumn; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetSectionOneColumnCommandImageName")]
#endif
		public override string ImageName { get { return "ColumnsOne"; } }
	}
	#endregion
	#region SetSectionTwoColumnsCommand
	public class SetSectionTwoColumnsCommand : SetSectionColumnCountCommandBase {
		public SetSectionTwoColumnsCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetSectionTwoColumnsCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_SetSectionTwoColumns; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetSectionTwoColumnsCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_SetSectionTwoColumnsDescription; } }
		protected internal override int ActualColumnCount { get { return 2; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetSectionTwoColumnsCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.SetSectionTwoColumns; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetSectionTwoColumnsCommandImageName")]
#endif
		public override string ImageName { get { return "ColumnsTwo"; } }
	}
	#endregion
	#region SetSectionThreeColumnsCommand
	public class SetSectionThreeColumnsCommand : SetSectionColumnCountCommandBase {
		public SetSectionThreeColumnsCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetSectionThreeColumnsCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_SetSectionThreeColumns; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetSectionThreeColumnsCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_SetSectionThreeColumnsDescription; } }
		protected internal override int ActualColumnCount { get { return 3; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetSectionThreeColumnsCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.SetSectionThreeColumns; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetSectionThreeColumnsCommandImageName")]
#endif
		public override string ImageName { get { return "ColumnsThree"; } }
	}
	#endregion
	#region SetSectionColumnsPlaceholderCommand
	public class SetSectionColumnsPlaceholderCommand : SetSectionColumnCountCommandBase, IPlaceholderCommand {
		public SetSectionColumnsPlaceholderCommand(IRichEditControl control)
			: base(control) {
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_SetSectionColumns; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_SetSectionColumnsDescription; } }
		protected internal override int ActualColumnCount { get { return -1; } }
		public override RichEditCommandId Id { get { return RichEditCommandId.SetSectionColumnsPlaceholder; } }
		public override string ImageName { get { return "ColumnsTwo"; } }
		protected internal override void ModifyDocumentModelCore(ICommandUIState state) {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			if (!Control.InnerControl.DocumentModel.ActivePieceTable.CanContainCompositeContent())
				state.Enabled = false;
		}
	}
	#endregion
}
