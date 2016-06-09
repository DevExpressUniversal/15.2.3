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
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using DevExpress.Utils.Commands;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Forms;
using DevExpress.XtraRichEdit.Utils;
using Debug = System.Diagnostics.Debug;
#if !SL
using System.Collections;
using System.Diagnostics;
#endif
namespace DevExpress.XtraRichEdit.Commands {
	#region ShowColumnsSetupFormCommand
	public class ShowColumnsSetupFormCommand : ChangeSectionFormattingCommandBase<ColumnsInfoUI> {
		public ShowColumnsSetupFormCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowColumnsSetupFormCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.ShowColumnsSetupForm; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowColumnsSetupFormCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ShowColumnsSetupForm; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowColumnsSetupFormCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ShowColumnsSetupFormDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowColumnsSetupFormCommandImageName")]
#endif
		public override string ImageName { get { return "Columns"; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowColumnsSetupFormCommandShowsModalDialog")]
#endif
		public override bool ShowsModalDialog { get { return true; } }
		#endregion
		public override void ForceExecute(ICommandUIState state) {
			CheckExecutedAtUIThread();
			NotifyBeginCommandExecution(state);
			try {
				IValueBasedCommandUIState<ColumnsInfoUI> valueBasedState = state as IValueBasedCommandUIState<ColumnsInfoUI>;
				ShowColumnsSetupForm(valueBasedState.Value, ShowColumnsSetupFormCallback, state);
			}
			finally {
				NotifyEndCommandExecution(state);
			}
		}
		protected internal virtual void ShowColumnsSetupFormCallback(ColumnsInfoUI properties, object callbackData) {
			IValueBasedCommandUIState<ColumnsInfoUI> valueBasedState = callbackData as IValueBasedCommandUIState<ColumnsInfoUI>;
			valueBasedState.Value = properties;
			base.ForceExecute(valueBasedState);
		}
		protected internal override void ModifyDocumentModelCore(ICommandUIState state) {
			IValueBasedCommandUIState<ColumnsInfoUI> valueBasedState = state as IValueBasedCommandUIState<ColumnsInfoUI>;
			if (valueBasedState.Value.ApplyType == SectionPropertiesApplyType.WholeDocument) {
				PieceTable pieceTable = ActivePieceTable;
				DocumentModelPosition start = DocumentModelPosition.FromParagraphStart(pieceTable, ParagraphIndex.Zero);
				DocumentModelPosition end = DocumentModelPosition.FromParagraphEnd(pieceTable, new ParagraphIndex(pieceTable.Paragraphs.Count - 1));
				DocumentModelChangeActions actions = ChangeProperty(start, end, state);
				pieceTable.ApplyChangesCore(actions, RunIndex.DontCare, RunIndex.DontCare);
			}
			else
				base.ModifyDocumentModelCore(state);
		}
		internal virtual void ShowColumnsSetupForm(ColumnsInfoUI properties, ShowColumnsSetupFormCallback callback, object callbackData) {
			Control.ShowColumnsSetupForm(properties, callback, callbackData);
		}
		protected internal override SectionPropertyModifier<ColumnsInfoUI> CreateModifier(ICommandUIState state) {
			IValueBasedCommandUIState<ColumnsInfoUI> valueBasedState = state as IValueBasedCommandUIState<ColumnsInfoUI>;
			return new SectionColumnsSetupModifier(valueBasedState.Value);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			CheckExecutedAtUIThread();
			base.UpdateUIStateCore(state);
			IValueBasedCommandUIState<ColumnsInfoUI> valueBasedState = state as IValueBasedCommandUIState<ColumnsInfoUI>;
			if (valueBasedState == null)
				return;
			valueBasedState.Value = GetCurrentPropertyValue();
		}
		ColumnsInfoUI GetCurrentPropertyValue() {
			MergedSectionPropertyModifier<ColumnsInfoUI> modifier = (MergedSectionPropertyModifier<ColumnsInfoUI>)CreateModifier(CreateDefaultCommandUIState());
			List<SelectionItem> items = DocumentModel.Selection.Items;
			int count = items.Count;
			ColumnsInfoUI result = null;
			Debug.Assert(count > 0);
			SectionIndex minSectionIndex = SectionIndex.MinValue;
			SectionIndex maxSectionIndex = SectionIndex.MinValue;
			for (int i = 0; i < count; i++) {
				SelectionItem item = items[i];
				DocumentModelPosition start = CalculateStartPosition(item, false);
				DocumentModelPosition end = CalculateEndPosition(item, false);
				ColumnsInfoUI properties = DocumentModel.ObtainMergedSectionsPropertyValue(start.LogPosition, Math.Max(1, end.LogPosition - start.LogPosition), modifier);
				if (result != null)
					modifier.Merge(result, properties);
				else
					result = properties;
				minSectionIndex = DocumentModel.FindSectionIndex(start.LogPosition);
				maxSectionIndex = DocumentModel.FindSectionIndex(end.LogPosition);
			}
			result.ApplyType = CalculateApplyType(minSectionIndex, maxSectionIndex);
			result.AvailableApplyType = CalculateAvailableApplyType(minSectionIndex, maxSectionIndex);
			return result;
		}
		SectionPropertiesApplyType CalculateApplyType(SectionIndex minSectionIndex, SectionIndex maxSectionIndex) {
			if (DocumentModel.Sections.Count == 1)
				return SectionPropertiesApplyType.WholeDocument;
			if (minSectionIndex == maxSectionIndex)
				return SectionPropertiesApplyType.CurrentSection;
			return SectionPropertiesApplyType.SelectedSections;
		}
		SectionPropertiesApplyType CalculateAvailableApplyType(SectionIndex minSectionIndex, SectionIndex maxSectionIndex) {
			if (DocumentModel.Sections.Count == 1)
				return SectionPropertiesApplyType.WholeDocument;
			if (minSectionIndex == maxSectionIndex)
				return SectionPropertiesApplyType.CurrentSection | SectionPropertiesApplyType.WholeDocument;
			return SectionPropertiesApplyType.SelectedSections | SectionPropertiesApplyType.WholeDocument;
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			DefaultValueBasedCommandUIState<ColumnsInfoUI> result = new DefaultValueBasedCommandUIState<ColumnsInfoUI>();
			return result;
		}
	}
	#endregion
}
