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
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Layout;
using System.ComponentModel;
namespace DevExpress.XtraRichEdit.Commands {
	#region HeaderFooterRelatedMultiCommandBase (abstract class)
	public abstract class HeaderFooterRelatedMultiCommandBase : TransactedMultiCommand {
		protected HeaderFooterRelatedMultiCommandBase(IRichEditControl edit)
			: base(edit) {
		}
		protected internal override MultiCommandExecutionMode ExecutionMode { get { return MultiCommandExecutionMode.ExecuteAllAvailable; } }
		protected internal override MultiCommandUpdateUIStateMode UpdateUIStateMode { get { return MultiCommandUpdateUIStateMode.EnableIfAllAvailable; } }
		protected internal virtual bool CanExecuteOnlyInMainPieceTable { get { return false; } }
		protected internal override void CreateCommands() {
		}
		protected internal virtual bool UpdateLayoutPositionToPageArea() {
			CaretPosition caretPosition = ActiveView.CaretPosition;
			caretPosition.Update(DocumentLayoutDetailsLevel.PageArea);
			return caretPosition.LayoutPosition.IsValid(DocumentLayoutDetailsLevel.PageArea);
		}
		protected internal virtual void GoToExistingHeaderFooter(IPieceTableProvider where) {
			MakeHeaderFooterActiveCommand command = new MakeHeaderFooterActiveCommand(Control, where);
			command.Execute();
		}
		protected internal virtual void GoToExistingHeaderFooter(SectionHeaderFooterBase headerFooter, int preferredPageIndex, Section section) {
			IPieceTableProvider provider = new ExplicitPieceTableProvider(headerFooter.PieceTable, section, preferredPageIndex);
			GoToExistingHeaderFooter(provider);
		}
		protected internal bool IsFirstSectionPage(DocumentLayoutPosition layoutPosition) {
			int currentPageIndex = layoutPosition.Page.PageIndex;
			if (currentPageIndex <= 0)
				return true;
			Page previousPage = ActiveView.DocumentLayout.Pages[currentPageIndex - 1];
			return !Object.ReferenceEquals(previousPage.Areas.First.Section, layoutPosition.PageArea.Section);
		}
		public override void UpdateUIState(ICommandUIState state) {
			UpdateUIStateCore(state);
			UpdateUIStateViaService(state);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = IsContentEditable;
			state.Visible = true;
			ApplyCommandRestrictionOnEditableControl(state, Options.DocumentCapabilities.HeadersFooters, state.Enabled);
			ApplyDocumentProtectionToSelectedSections(state);
			if (state.Enabled)
				state.Enabled = ActivePieceTable.IsMain == CanExecuteOnlyInMainPieceTable && ActiveViewType == RichEditViewType.PrintLayout;
		}
	}
	#endregion
	#region EditPageHeaderFooterCommand<T> (abstract class)
	public abstract class EditPageHeaderFooterCommand<T> : HeaderFooterRelatedMultiCommandBase where T : SectionHeaderFooterBase {
		bool forceCreateNewHeader;
		protected EditPageHeaderFooterCommand(IRichEditControl control)
			: base(control) {
		}
		protected internal override bool CanExecuteOnlyInMainPieceTable { get { return true; } }
		public bool ForceCreateNewHeader { get { return forceCreateNewHeader; } set { forceCreateNewHeader = value; } }
		protected internal override void ForceExecuteCore(ICommandUIState state) {
			if (!UpdateLayoutPositionToPageArea())
				return;
			DocumentLayoutPosition layoutPosition = ActiveView.CaretPosition.LayoutPosition;
			Section section = layoutPosition.PageArea.Section;
			if (!DocumentModel.CanEditSection(section))
				return;
			bool isFirstSectionPage = IsFirstSectionPage(layoutPosition);
			bool isEvenPage = layoutPosition.Page.IsEven;
			SectionHeadersFootersBase container = GetContainer(section);
			SectionHeaderFooterBase headerFooter = container.CalculateActualObjectCore(isFirstSectionPage, isEvenPage);
			if (headerFooter == null || ForceCreateNewHeader) {
				HeaderFooterType type = container.CalculateActualObjectType(isFirstSectionPage, isEvenPage);
				InsertPageHeaderFooterCoreCommand<T> command = CreateInsertObjectCommand(type);
				command.Execute();
				GoToExistingHeaderFooter(command);
			}
			else
				GoToExistingHeaderFooter(headerFooter, layoutPosition.Page.PageIndex, section);
		}
		protected internal abstract InsertPageHeaderFooterCoreCommand<T> CreateInsertObjectCommand(HeaderFooterType type);
		protected internal abstract SectionHeadersFootersBase GetContainer(Section section);
	}
	#endregion
	#region EditPageHeaderCommand
	public class EditPageHeaderCommand : EditPageHeaderFooterCommand<SectionHeader> {
		public EditPageHeaderCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("EditPageHeaderCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_EditPageHeader; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("EditPageHeaderCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_EditPageHeaderDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("EditPageHeaderCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.EditPageHeader; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("EditPageHeaderCommandImageName")]
#endif
		public override string ImageName { get { return "Header"; } }
		protected internal override InsertPageHeaderFooterCoreCommand<SectionHeader> CreateInsertObjectCommand(HeaderFooterType type) {
			return new InsertPageHeaderCoreCommand(Control, type);
		}
		protected internal override SectionHeadersFootersBase GetContainer(Section section) {
			return section.Headers;
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Commands.Internal {
	#region IPieceTableProvider
	public interface IPieceTableProvider {
		PieceTable PieceTable { get; }
		Section Section { get; }
		int PreferredPageIndex { get; }
	}
	#endregion
	#region ExplicitPieceTableProvider
	public class ExplicitPieceTableProvider : IPieceTableProvider {
		readonly PieceTable pieceTable;
		readonly Section section;
		readonly int preferredPageIndex;
		public ExplicitPieceTableProvider(PieceTable pieceTable, Section section, int preferredPageIndex) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
			this.section = section;
			this.preferredPageIndex = preferredPageIndex;
		}
		public PieceTable PieceTable { get { return pieceTable; } }
		public Section Section { get { return section; } }
		public int PreferredPageIndex { get { return preferredPageIndex; } }
	}
	#endregion
	public abstract class InsertPageHeaderFooterCoreCommandBase : InsertObjectCommandBase {
		protected InsertPageHeaderFooterCoreCommandBase(IRichEditControl control)
			: base(control) {
		}
		protected internal abstract SectionHeaderFooterBase ModifyModelCore(Section section);
	}
	#region InsertPageHeaderFooterCoreCommand<T> (abstract class)
	public abstract class InsertPageHeaderFooterCoreCommand<T> : InsertPageHeaderFooterCoreCommandBase, IPieceTableProvider where T : SectionHeaderFooterBase {
		readonly HeaderFooterType type;
		SectionHeaderFooterBase insertedHeaderFooter;
		Section section;
		protected InsertPageHeaderFooterCoreCommand(IRichEditControl control, HeaderFooterType type)
			: base(control) {
			this.type = type;
		}
		public HeaderFooterType Type { get { return type; } }
		protected internal override void ModifyModel() {
			ModifyModelCore(DocumentModel.GetActiveSectionBySelectionEnd());
		}
		protected internal override SectionHeaderFooterBase ModifyModelCore(Section section) {
			this.section = section;
			if (section != null)
				this.insertedHeaderFooter = CreateHeaderFooter(section);
			return insertedHeaderFooter;
		}
		#region IPieceTableProvider Members
		PieceTable IPieceTableProvider.PieceTable { get { return insertedHeaderFooter.PieceTable; } }
		Section IPieceTableProvider.Section { get { return section; } }
		int IPieceTableProvider.PreferredPageIndex { get { return -1; } }
		#endregion
		protected internal abstract SectionHeadersFootersBase GetHeaderFooterContainer(Section section);
		protected internal virtual SectionHeaderFooterBase CreateHeaderFooter(Section section) {
			SectionHeadersFootersBase container = GetHeaderFooterContainer(section);
			bool relinkNextSection = container.ShouldRelinkNextSection(Type);
			bool relinkPrevSection = container.ShouldRelinkPreviousSection(Type);
			container.Create(Type);
			if (relinkPrevSection)
				GetHeaderFooterContainer(section.GetPreviousSection()).LinkToNext(Type);
			if (relinkNextSection)
				GetHeaderFooterContainer(section.GetNextSection()).LinkToPrevious(Type);
			return container.GetObjectCore(Type);
		}
	}
	#endregion
	#region InsertPageHeaderCoreCommand
	public class InsertPageHeaderCoreCommand : InsertPageHeaderFooterCoreCommand<SectionHeader> {
		public InsertPageHeaderCoreCommand(IRichEditControl control, HeaderFooterType type)
			: base(control, type) {
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_EditPageHeader; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_EditPageHeaderDescription; } }
		public override string ImageName { get { return "Header"; } }
		protected internal override SectionHeadersFootersBase GetHeaderFooterContainer(Section section) {
			return section.Headers;
		}
	}
	#endregion
	#region MakeHeaderFooterActiveCommand
	public class MakeHeaderFooterActiveCommand : RichEditMenuItemSimpleCommand {
		readonly IPieceTableProvider pieceTableProvider;
		public MakeHeaderFooterActiveCommand(IRichEditControl control, IPieceTableProvider pieceTableProvider)
			: base(control) {
			Guard.ArgumentNotNull(pieceTableProvider, "pieceTableProvider");
			this.pieceTableProvider = pieceTableProvider;
		}
		#region Properties
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		#endregion
		protected internal override void ExecuteCore() {
			PieceTable pieceTable = pieceTableProvider.PieceTable;
			if (pieceTable != null) {
				ChangeActivePieceTableCommand command = new ChangeActivePieceTableCommand(Control, pieceTable, pieceTableProvider.Section, pieceTableProvider.PreferredPageIndex);
				command.Execute();
			}
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
		}
	}
	#endregion
}
