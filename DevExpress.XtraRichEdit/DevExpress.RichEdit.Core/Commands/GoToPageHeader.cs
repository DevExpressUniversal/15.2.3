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
using System.ComponentModel;
namespace DevExpress.XtraRichEdit.Commands {
	#region GotoPageHeaderFooterCommand<T> (abstract class)
	public abstract class GoToPageHeaderFooterCommand<T> : TransactedInsertObjectCommand where T : SectionHeaderFooterBase {
		protected GoToPageHeaderFooterCommand(IRichEditControl control)
			: base(control) {
		}
		protected internal override RichEditCommand InsertObjectCommand { get { return Commands.Count > 0 ? (RichEditCommand)Commands[0] : null; } }
		public override string MenuCaption { get { return XtraRichEditLocalizer.GetString(MenuCaptionStringId); } }
		public override string Description { get { return XtraRichEditLocalizer.GetString(DescriptionStringId); } }
		protected internal override void CreateCommands() {
			T correspondingHeaderFooter = GetCorrespondingHeaderFooter();
			if (correspondingHeaderFooter == null) {
				InsertPageHeaderFooterCoreCommand<T> command = (InsertPageHeaderFooterCoreCommand<T>)CreateInsertObjectCommand();
				if (command != null) {
					Commands.Add(command);
					Commands.Add(new MakeHeaderFooterActiveCommand(Control, command));
				}
			}
			else {
				IPieceTableProvider provider = new ExplicitPieceTableProvider(correspondingHeaderFooter.PieceTable, DocumentModel.GetActiveSection(), -1);
				Commands.Add(new MakeHeaderFooterActiveCommand(Control, provider));
			}
		}
		public override void UpdateUIState(ICommandUIState state) {
			base.UpdateUIState(state);
			ApplyCommandRestrictionOnEditableControl(state, Options.DocumentCapabilities.HeadersFooters, state.Enabled);
			if (state.Enabled)
				state.Enabled = ActivePieceTable.IsHeaderFooter &&
					ActiveViewType == RichEditViewType.PrintLayout &&
					CanGoFromCurrentSelection();
		}
		protected internal virtual bool CanGoFromCurrentSelection() {
			return !(ActivePieceTable.ContentType is T);
		}
		protected internal abstract T GetCorrespondingHeaderFooter();
	}
	#endregion
	#region GoToPageHeaderCommand
	public class GoToPageHeaderCommand : GoToPageHeaderFooterCommand<SectionHeader> {
		public GoToPageHeaderCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("GoToPageHeaderCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_GoToPageHeader; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("GoToPageHeaderCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_GoToPageHeaderDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("GoToPageHeaderCommandImageName")]
#endif
		public override string ImageName { get { return "GoToHeader"; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("GoToPageHeaderCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.GoToPageHeader; } }
		protected internal override RichEditCommand CreateInsertObjectCommand() {
			SectionFooter footer = ActivePieceTable.ContentType as SectionFooter;
			if (footer != null)
				return new InsertPageHeaderCoreCommand(Control, footer.Type);
			else
				return null;
		}
		protected internal override SectionHeader GetCorrespondingHeaderFooter() {
			SectionFooter footer = ActivePieceTable.ContentType as SectionFooter;
			if (footer != null) {
				Section section = DocumentModel.GetActiveSectionBySelectionEnd();
				return section.GetCorrespondingHeader(footer);
			}
			else
				return null;
		}
	}
	#endregion
}
