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
using DevExpress.Snap.Core.Native;
using DevExpress.Snap.Core.Native.Data;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.Utils.Commands;
using DevExpress.Snap.Core.Native.Templates;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.Snap.Core.Commands {
	public abstract class InsertGroupSeparatorCommandBase : InsertGroupTemplateCommandBase {
		protected InsertGroupSeparatorCommandBase(IRichEditControl control)
			: base(control) {
		}
		protected override GroupModifyType GroupType { get { return GroupModifyType.InsertGroupSeparator; } }
		protected override void UpdateUIStateCoreInternal(ICommandUIState state) {
			state.Enabled = IsEnabled();
			if (state.Enabled)
				state.Checked = IsChecked();
		}
		protected override bool IsEnabled() {
			if (EditedFieldInfo == null)
				return false;
			return SnapObjectModelController.IsBookmarkCorrespondsToGroup(Bookmark);
		}
		protected override bool IsChecked() {
			InstructionController controller = EditedFieldInfo.CreateFieldInstructionController();
			if (controller == null)
				return false;
			PrepareFieldPathInfo(controller);
			GroupProperties properties = GetEditedGroupProperties();
			return CheckGroupSeparatorTemplate(controller, properties);
		}
		protected virtual bool CheckGroupSeparatorTemplate(InstructionController controller, GroupProperties properties) {
			if (properties == null || !properties.HasTemplateSeparator)
				return false;
			DocumentLogInterval interval = controller.Instructions.GetSwitchArgumentDocumentInterval(properties.TemplateSeparatorSwitch);
			if (interval == null)
				return false;
			SnapPieceTable pieceTable = (SnapPieceTable)ActivePieceTable;
			DocumentLogInterval actualInterval = new TemplateController(pieceTable).GetActualTemplateInterval(interval);
			return CompareWithTemplate(actualInterval, new SeparatorTemplateComparer(pieceTable));
		}
		protected virtual bool CompareWithTemplate(DocumentLogInterval interval, SeparatorTemplateComparer comparer) {
			return false;
		}
		protected internal override void UpdateFieldCode(InstructionController controller) {
			BeginUpdate(controller);
			try {
				UpdateFieldCodeCore(controller);
			}
			finally {
				EndUpdate(controller);
			}
		}
		protected internal virtual void UpdateFieldCodeCore(InstructionController controller) {
			DocumentModel template = CreateGroupTemplate(controller, GroupType);
			ProcessGroups(controller, template);
		}
		void ProcessGroups(InstructionController controller, DocumentModel groupTemplate) {
			GroupProperties properties = GetEditedGroupProperties();
			SetGroupSeparator(controller, properties, groupTemplate);
		}
		GroupProperties GetEditedGroupProperties() {
			FieldDataMemberInfoItem item = FieldPathInfo.DataMemberInfo.LastItem;
			string templateSwitch = SnapBookmarksHelper.GetTemplateSwitch(EditedFieldInfo.ParsedInfo, Bookmark);
#if DEBUGTEST || DEBUG
			Debug.Assert(!String.IsNullOrEmpty(templateSwitch));
#endif
			int count = item.Groups.Count;
			for (int i = 0; i < count; i++) {
				GroupProperties properties = item.Groups[i];
				if (properties.TemplateHeaderSwitch == templateSwitch || properties.TemplateFooterSwitch == templateSwitch || properties.TemplateSeparatorSwitch == templateSwitch) {
#if DEBUGTEST || DEBUG
					Debug.Assert(i <= item.CurrentGroupIndex);
#endif
					return properties;
				}
			}
#if DEBUGTEST || DEBUG
			Exceptions.ThrowInternalException();
#endif
			return null;
		}
		protected virtual void SetGroupSeparator(InstructionController controller, GroupProperties properties, DocumentModel groupTemplate) {
			if (String.IsNullOrEmpty(properties.TemplateSeparatorSwitch))
				properties.TemplateSeparatorSwitch = controller.GetNonUsedSwitch();
			controller.SetSwitch(properties.TemplateSeparatorSwitch, groupTemplate);
		}
		protected override void ProcessGroups<T>(FieldDataMemberInfoItem item, ModifyGroupPropertiesAction<T> modifyAction, T modifyInfo) { }
	}
	public abstract class InsertSectionBreakGroupSeparatorCommandBase : InsertGroupSeparatorCommandBase {
		protected InsertSectionBreakGroupSeparatorCommandBase(IRichEditControl control)
			: base(control) {
		}
		protected internal abstract SectionStartType StartType { get; }
		protected override void SetGroupTemplateContent(SnapPieceTable targetPieceTable, InstructionController controller, string baseStyleName, int listLevel, int level, GroupModifyType modifyType) {
			new SeparatorTemplateExecutor().InsertSectionSeparator(targetPieceTable, StartType);
		}
		protected override bool CompareWithTemplate(DocumentLogInterval interval, SeparatorTemplateComparer comparer) {
			return comparer.IsSectionBreak(interval, StartType);
		}
	}
	[CommandLocalization(Localization.SnapStringId.InsertNoneGroupSeparatorCommand_MenuCaption, Localization.SnapStringId.InsertNoneGroupSeparatorCommand_Description)]
	public class InsertNoneGroupSeparatorCommand : InsertGroupSeparatorCommandBase {
		public InsertNoneGroupSeparatorCommand(IRichEditControl control)
			: base(control) {
		}
		public override RichEditCommandId Id { get { return SnapCommandId.InsertNoneGroupSeparator; } }
		public override string ImageName { get { return "None"; } }
		protected override bool CheckGroupSeparatorTemplate(InstructionController controller, GroupProperties properties) {
			if (properties == null)
				return false;
			return (!properties.HasTemplateSeparator);
		}
		protected override void SetGroupSeparator(InstructionController controller, GroupProperties properties, DocumentModel groupTemplate) {
			if (string.IsNullOrEmpty(properties.TemplateSeparatorSwitch))
				return;
			controller.RemoveSwitch(properties.TemplateSeparatorSwitch);
			properties.TemplateSeparatorSwitch = String.Empty;
		}
	}
	[CommandLocalization(Localization.SnapStringId.InsertPageBreakGroupSeparatorCommand_MenuCaption, Localization.SnapStringId.InsertPageBreakGroupSeparatorCommand_Description)]
	public class InsertPageBreakGroupSeparatorCommand : InsertGroupSeparatorCommandBase {
		public InsertPageBreakGroupSeparatorCommand(IRichEditControl control)
			: base(control) {
		}
		public override RichEditCommandId Id { get { return SnapCommandId.InsertPageBreakGroupSeparator; } }
		public override string ImageName { get { return "InsertPageBreak"; } }
		protected override void SetGroupTemplateContent(SnapPieceTable pieceTable, InstructionController controller, string baseStyleName, int listLevel, int level, GroupModifyType modifyType) {
			new SeparatorTemplateExecutor().InsertPageBreakSeparator(pieceTable);
		}
		protected override bool CompareWithTemplate(DocumentLogInterval interval, SeparatorTemplateComparer comparer) {
			return comparer.IsPageBreak(interval);
		}
	}
	[CommandLocalization(Localization.SnapStringId.InsertEmptyParagraphGroupSeparatorCommand_MenuCaption, Localization.SnapStringId.InsertEmptyParagraphGroupSeparatorCommand_Description)]
	public class InsertEmptyParagraphGroupSeparatorCommand : InsertGroupSeparatorCommandBase {
		public InsertEmptyParagraphGroupSeparatorCommand(IRichEditControl control)
			: base(control) {
		}
		public override RichEditCommandId Id { get { return SnapCommandId.InsertEmptyParagraphGroupSeparator; } }
		public override string ImageName { get { return "EmptyParagraphSeparator"; } }
		protected override void SetGroupTemplateContent(SnapPieceTable pieceTable, InstructionController controller, string baseStyleName, int listLevel, int level, GroupModifyType modifyType) {
			new SeparatorTemplateExecutor().InsertEmptyParagraphSeparator(pieceTable);
		}
		protected override bool CompareWithTemplate(DocumentLogInterval interval, SeparatorTemplateComparer comparer) {
			return comparer.IsEmptyParagraph(interval);
		}
	}
	[CommandLocalization(Localization.SnapStringId.InsertEmptyRowGroupSeparatorCommand_MenuCaption, Localization.SnapStringId.InsertEmptyRowGroupSeparatorCommand_Description)]
	public class InsertEmptyRowGroupSeparatorCommand : InsertGroupSeparatorCommandBase {
		public InsertEmptyRowGroupSeparatorCommand(IRichEditControl control)
			: base(control) {
		}
		public override RichEditCommandId Id { get { return SnapCommandId.InsertEmptyRowGroupSeparator; } }
		public override string ImageName { get { return "EmptyTableRowSeparator"; } }
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			state.Visible = state.Enabled;
		}
		protected override bool IsEnabled() {
			if (!base.IsEnabled())
				return false;
			if (TableCommandsHelper.CheckForTablesIncluded(DocumentModel))
				return true;
			if (Bookmark.TemplateInterval.TemplateInfo.TemplateType == SnapTemplateIntervalType.GroupSeparator)
				return new SnapObjectModelController((SnapPieceTable)ActivePieceTable).FindTableCellBySeparator() != null;
			return false;
		}
		protected override bool CompareWithTemplate(DocumentLogInterval interval, SeparatorTemplateComparer comparer) {
			return comparer.IsEmptyRow(interval);
		}
	}
	[CommandLocalization(Localization.SnapStringId.InsertSectionBreakNextPageGroupSeparatorCommand_MenuCaption, Localization.SnapStringId.InsertSectionBreakNextPageGroupSeparatorCommand_Description)]
	public class InsertSectionBreakNextPageGroupSeparatorCommand : InsertSectionBreakGroupSeparatorCommandBase {
		public InsertSectionBreakNextPageGroupSeparatorCommand(IRichEditControl control)
			: base(control) {
		}
		public override RichEditCommandId Id { get { return SnapCommandId.InsertSectionBreakNextPageGroupSeparator; } }
		public override string ImageName { get { return "InsertSectionBreakNextPage"; } }
		protected internal override SectionStartType StartType { get { return SectionStartType.NextPage; } }
	}
	[CommandLocalization(Localization.SnapStringId.InsertSectionBreakEvenPageGroupSeparatorCommand_MenuCaption, Localization.SnapStringId.InsertSectionBreakEvenPageGroupSeparatorCommand_Description)]
	public class InsertSectionBreakEvenPageGroupSeparatorCommand : InsertSectionBreakGroupSeparatorCommandBase {
		public InsertSectionBreakEvenPageGroupSeparatorCommand(IRichEditControl control)
			: base(control) {
		}
		public override RichEditCommandId Id { get { return SnapCommandId.InsertSectionBreakEvenPageGroupSeparator; } }
		public override string ImageName { get { return "InsertSectionBreakEvenPage"; } }
		protected internal override SectionStartType StartType { get { return SectionStartType.EvenPage; } }
	}
	[CommandLocalization(Localization.SnapStringId.InsertSectionBreakOddPageGroupSeparatorCommand_MenuCaption, Localization.SnapStringId.InsertSectionBreakOddPageGroupSeparatorCommand_Description)]
	public class InsertSectionBreakOddPageGroupSeparatorCommand : InsertSectionBreakGroupSeparatorCommandBase {
		public InsertSectionBreakOddPageGroupSeparatorCommand(IRichEditControl control)
			: base(control) {
		}
		public override RichEditCommandId Id { get { return SnapCommandId.InsertSectionBreakOddPageGroupSeparator; } }
		public override string ImageName { get { return "InsertSectionBreakOddPage"; } }
		protected internal override SectionStartType StartType { get { return SectionStartType.OddPage; } }
	}
}
