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
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Localization;
using System.ComponentModel;
namespace DevExpress.XtraRichEdit.Commands {
	#region ToggleHeaderFooterCommandBase (abstract class)
	public abstract class ToggleHeaderFooterCommandBase : HeaderFooterRelatedMultiCommandBase {
		protected ToggleHeaderFooterCommandBase(IRichEditControl control)
			: base(control) {
		}
		protected internal override void ForceExecuteCore(ICommandUIState state) {
			if (!UpdateLayoutPositionToPageArea())
				return;
			DocumentLayoutPosition layoutPosition = ActiveView.CaretPosition.LayoutPosition;
			Section section = layoutPosition.PageArea.Section;
			SectionHeaderFooterBase headerFooter = (SectionHeaderFooterBase)layoutPosition.PageArea.PieceTable.ContentType;
			bool isFirstSectionPage = IsFirstSectionPage(layoutPosition);
			bool isEvenPage = layoutPosition.Page.IsEven;
			int preferredPageIndex = layoutPosition.Page.PageIndex;
			SectionHeadersFootersBase container = headerFooter.GetContainer(section);
			ChangeValue(section, preferredPageIndex);
			HeaderFooterType type = container.CalculateActualObjectType(isFirstSectionPage, isEvenPage);
			SectionHeaderFooterBase newHeaderFooter = container.GetObjectCore(type);
			if (newHeaderFooter == null)
				newHeaderFooter = CreateNewHeaderFooter(headerFooter, isFirstSectionPage, isEvenPage, container);
			GoToExistingHeaderFooter(newHeaderFooter, preferredPageIndex, section);
		}
		protected internal virtual void ChangeValue(Section section, int preferredPageIndex) {
			DocumentModel.BeginUpdate();
			try {
				SetValue(section, !GetValue(section));
				ChangeActivePieceTableCommand command = new ChangeActivePieceTableCommand(Control, DocumentModel.MainPieceTable, section, preferredPageIndex);
				command.ActivatePieceTable(DocumentModel.MainPieceTable, null);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		private SectionHeaderFooterBase CreateNewHeaderFooter(SectionHeaderFooterBase headerFooter, bool isFirstSectionPage, bool isEvenPage, SectionHeadersFootersBase container) {
			HeaderFooterType type = container.CalculateActualObjectType(isFirstSectionPage, isEvenPage);
			InsertPageHeaderFooterCoreCommandBase command;
			if (headerFooter is SectionHeader)
				command = new InsertPageHeaderCoreCommand(Control, type);
			else
				command = new InsertPageFooterCoreCommand(Control, type);
			command.Execute();
			IPieceTableProvider provider = (IPieceTableProvider)command;
			return (SectionHeaderFooterBase)provider.PieceTable.ContentType;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			CaretPosition caretPosition = ActiveView.CaretPosition;
			caretPosition.Update(DocumentLayoutDetailsLevel.PageArea);
			if (caretPosition.LayoutPosition.IsValid(DocumentLayoutDetailsLevel.PageArea)) {
				Section section = caretPosition.LayoutPosition.PageArea.Section;
				state.Checked = GetValue(section);
			}
		}
		protected internal abstract bool GetValue(Section section);
		protected internal abstract void SetValue(Section section, bool value);
	}
	#endregion
	#region ToggleDifferentFirstPageCommand
	public class ToggleDifferentFirstPageCommand : ToggleHeaderFooterCommandBase {
		public ToggleDifferentFirstPageCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleDifferentFirstPageCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleDifferentFirstPage; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleDifferentFirstPageCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleDifferentFirstPageDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleDifferentFirstPageCommandId")]
#endif
public override RichEditCommandId Id { get { return RichEditCommandId.ToggleDifferentFirstPage; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleDifferentFirstPageCommandImageName")]
#endif
		public override string ImageName { get { return "DifferentFirstPage"; } }
		#endregion
		protected internal override bool GetValue(Section section) {
			return section.GeneralSettings.DifferentFirstPage;
		}
		protected internal override void SetValue(Section section, bool value) {
			section.GeneralSettings.DifferentFirstPage = value;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			if (!Control.InnerControl.DocumentModel.ActivePieceTable.CanContainCompositeContent())
				state.Enabled = false;
		}
	}
	#endregion
}
