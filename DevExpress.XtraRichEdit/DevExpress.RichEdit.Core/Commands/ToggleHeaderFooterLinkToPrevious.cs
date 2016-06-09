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
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Layout;
using System.ComponentModel;
namespace DevExpress.XtraRichEdit.Commands {
	public class ToggleHeaderFooterLinkToPreviousCommand : HeaderFooterRelatedMultiCommandBase {
		public ToggleHeaderFooterLinkToPreviousCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleHeaderFooterLinkToPreviousCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleHeaderFooterLinkToPrevious; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleHeaderFooterLinkToPreviousCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleHeaderFooterLinkToPreviousDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleHeaderFooterLinkToPreviousCommandImageName")]
#endif
		public override string ImageName { get { return "LinkToPrevious"; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleHeaderFooterLinkToPreviousCommandId")]
#endif
public override RichEditCommandId Id { get { return RichEditCommandId.ToggleHeaderFooterLinkToPrevious; } }
		protected internal override void ForceExecuteCore(ICommandUIState state) {
			ToggleHeaderFooterLinkToPreviousCoreCommand command = new ToggleHeaderFooterLinkToPreviousCoreCommand(Control);
			command.Execute();
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			if (state.Enabled) {
				ToggleHeaderFooterLinkToPreviousCoreCommand command = new ToggleHeaderFooterLinkToPreviousCoreCommand(Control);
				ICommandUIState newState = command.CreateDefaultCommandUIState();
				command.UpdateUIState(newState);
				state.Enabled = newState.Enabled;
				state.Checked = newState.Checked;
			}
		}
	}
}
namespace DevExpress.XtraRichEdit.Commands.Internal {
	public class ToggleHeaderFooterLinkToPreviousCoreCommand : InsertObjectCommandBase, IPieceTableProvider {
		SectionHeaderFooterBase newActiveHeaderFooter;
		Section section;
		public ToggleHeaderFooterLinkToPreviousCoreCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		protected internal override DocumentLayoutDetailsLevel UpdateCaretPositionBeforeChangeSelectionDetailsLevel { get { return DocumentLayoutDetailsLevel.Page; } }
		#endregion
		protected internal override void ModifyModel() {
			SectionHeaderFooterBase headerFooter = ActivePieceTable.ContentType as SectionHeaderFooterBase;
			if (headerFooter == null)
				return;
			this.section = GetCurrentSectionFromCaretLayoutPosition();
			if (section == null)
				return;
			SectionHeadersFootersBase container = headerFooter.GetContainer(section);
			if (container.IsLinkedToPrevious(headerFooter.Type))
				container.UnlinkFromPrevious(headerFooter.Type);
			else
				container.LinkToPrevious(headerFooter.Type);
			newActiveHeaderFooter = container.GetObjectCore(headerFooter.Type);
			PieceTable actualActiveHeaderFooterPieceTable = newActiveHeaderFooter != null ? newActiveHeaderFooter.PieceTable : DocumentModel.MainPieceTable;
			ChangeActivePieceTableCommand command = new ChangeActivePieceTableCommand(Control, actualActiveHeaderFooterPieceTable, section, -1);
			command.ActivatePieceTable(actualActiveHeaderFooterPieceTable, section);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			SectionHeaderFooterBase headerFooter = ActivePieceTable.ContentType as SectionHeaderFooterBase;
			if (headerFooter == null) {
				state.Enabled = false;
				return;
			}
			Section section = GetCurrentSectionFromCaretLayoutPosition();
			if (section == null)
				return;
			SectionHeadersFootersBase container = headerFooter.GetContainer(section);
			if (state.Enabled) {
				state.Enabled = container.CanLinkToPrevious(headerFooter.Type);
				if (state.Enabled)
					state.Checked = container.IsLinkedToPrevious(headerFooter.Type);
			}
		}
		#region IPieceTableProvider Members
		PieceTable IPieceTableProvider.PieceTable { get { return newActiveHeaderFooter != null ? newActiveHeaderFooter.PieceTable : DocumentModel.MainPieceTable; } }
		Section IPieceTableProvider.Section { get { return newActiveHeaderFooter != null ? section : null; } }
		int IPieceTableProvider.PreferredPageIndex { get { return -1; } }
		#endregion
		protected internal Section GetCurrentSectionFromCaretLayoutPosition() {
			UpdateCaretPosition();
			if (!CaretPosition.LayoutPosition.IsValid(DocumentLayoutDetailsLevel.PageArea))
				return null;
			return CaretPosition.LayoutPosition.PageArea.Section;
		}
	}
}
