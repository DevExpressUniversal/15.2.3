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

using DevExpress.XtraRichEdit;
using DevExpress.Snap.Core.Fields;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.Snap.Core.Native;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.Snap.Core.Native.Templates;
namespace DevExpress.Snap.Core.Commands {
	public abstract class InsertDataRowSeparatorCommandBase : InsertTemplateCommandBase {
		protected InsertDataRowSeparatorCommandBase(IRichEditControl control)
			: base(control) {
		}
		protected override string TemplateSwitch { get { return SNListField.SeparatorTemplateSwitch; } }
		protected override bool IsEnabled() {
			return base.IsEnabled() && Bookmark != null;
		}
		protected override bool IsChecked() {
			return base.IsTemplateExists() && CompareWithTemplate();
		}
		protected virtual bool CompareWithTemplate() {
			if (EditedFieldInfo.ParsedInfo == null || EditedFieldInfo.ParsedInfo.SeparatorTemplateInterval == null)
				return false;
			DocumentLogInterval actualInterval = new TemplateController((SnapPieceTable)ActivePieceTable).GetActualTemplateInterval(EditedFieldInfo.ParsedInfo.SeparatorTemplateInterval);
			return CompareWithTemplateCore(actualInterval, new SeparatorTemplateComparer((SnapPieceTable)ActivePieceTable));
		}
		protected virtual bool CompareWithTemplateCore(DocumentLogInterval separatorInterval, SeparatorTemplateComparer comparer) {
			return false;
		}
	}
	public abstract class InsertSectionBreakDataRowSeparatorCommandBase : InsertDataRowSeparatorCommandBase {
		protected InsertSectionBreakDataRowSeparatorCommandBase(IRichEditControl control)
			: base(control) {
		}
		public abstract SectionStartType StartType { get; }
		protected override bool CompareWithTemplateCore(DocumentLogInterval separatorInterval, SeparatorTemplateComparer comparer) {
			return comparer.IsSectionBreak(separatorInterval, StartType);
		}
	}
	[CommandLocalization(Localization.SnapStringId.InsertNoneDataRowSeparatorCommand_MenuCaption, Localization.SnapStringId.InsertNoneDataRowSeparatorCommand_Description)]
	public class InsertNoneDataRowSeparatorCommand : InsertDataRowSeparatorCommandBase {
		public InsertNoneDataRowSeparatorCommand(IRichEditControl control)
			: base(control) {
		}
		public override RichEditCommandId Id { get { return SnapCommandId.InsertNoneDataRowSeparator; } }
		public override string ImageName { get { return "SeparatorListNone"; } }
		protected override ITemplateModifier CreateTemplateModifier() {
			return new InsertNoneSeparatorCommandInternal();
		}
		protected override bool IsChecked() {
			if (EditedFieldInfo == null || EditedFieldInfo.ParsedInfo == null)
				return false;
			InstructionController controller = EditedFieldInfo.CreateFieldInstructionController();
			if (controller == null)
				return false;
			return !controller.Instructions.GetBool(TemplateSwitch);
		}
	}
	[CommandLocalization(Localization.SnapStringId.InsertPageBreakDataRowSeparatorCommand_MenuCaption, Localization.SnapStringId.InsertPageBreakDataRowSeparatorCommand_Description)]
	public class InsertPageBreakDataRowSeparatorCommand : InsertDataRowSeparatorCommandBase {
		public InsertPageBreakDataRowSeparatorCommand(IRichEditControl control)
			: base(control) {
		}
		public override RichEditCommandId Id { get { return SnapCommandId.InsertPageBreakGroupSeparator; } }
		public override string ImageName { get { return "SeparatorPageBreakList"; } }
		protected override ITemplateModifier CreateTemplateModifier() {
			return new InsertPageBreakSeparatorCommandInternal(DocumentModel);
		}
		protected override bool CompareWithTemplateCore(DocumentLogInterval separatorInterval, SeparatorTemplateComparer comparer) {
			return comparer.IsPageBreak(separatorInterval);
		}
	}
	[CommandLocalization(Localization.SnapStringId.InsertEmptyParagraphDataRowSeparatorCommand_MenuCaption, Localization.SnapStringId.InsertEmptyParagraphDataRowSeparatorCommand_Description)]
	public class InsertEmptyParagraphDataRowSeparatorCommand : InsertDataRowSeparatorCommandBase {
		public InsertEmptyParagraphDataRowSeparatorCommand(IRichEditControl control)
			: base(control) {
		}
		public override RichEditCommandId Id { get { return SnapCommandId.InsertEmptyParagraphDataRowSeparator; } }
		public override string ImageName { get { return "EmptyParagraphSeparatorList"; } }
		protected override ITemplateModifier CreateTemplateModifier() {
			return new InsertEmptyParagraphSeparatorCommandInternal(DocumentModel);
		}
		protected override bool CompareWithTemplateCore(DocumentLogInterval separatorInterval, SeparatorTemplateComparer comparer) {
			return comparer.IsEmptyParagraph(separatorInterval);
		}
	}
	[CommandLocalization(Localization.SnapStringId.InsertEmptyRowDataRowSeparatorCommand_MenuCaption, Localization.SnapStringId.InsertEmptyRowDataRowSeparatorCommand_Description)]
	public class InsertEmptyRowDataRowSeparatorCommand : InsertDataRowSeparatorCommandBase {
		public InsertEmptyRowDataRowSeparatorCommand(IRichEditControl control)
			: base(control) {
		}
		public override RichEditCommandId Id { get { return SnapCommandId.InsertEmptyRowDataRowSeparator; } }
		public override string ImageName { get { return "EmptyTableRowSeparatorList"; } }
		protected override ITemplateModifier CreateTemplateModifier() {
			return new InsertEmptyRowSeparatorCommandInternal(DocumentModel);
		}
		protected override void UpdateUIStateCore(Utils.Commands.ICommandUIState state) {
			base.UpdateUIStateCore(state);
			state.Visible = state.Enabled;
		}
		protected override bool IsEnabled() {
			if (!base.IsEnabled())
				return false;
			if (TableCommandsHelper.CheckForTablesIncluded(DocumentModel))
				return true;
			if (Bookmark.TemplateInterval.TemplateInfo.TemplateType == SnapTemplateIntervalType.Separator)
				return new SnapObjectModelController((SnapPieceTable)ActivePieceTable).FindTableCellBySeparator() != null;
			return false;
		}
		protected override bool CompareWithTemplateCore(DocumentLogInterval separatorInterval, SeparatorTemplateComparer comparer) {
			return comparer.IsEmptyRow(separatorInterval);
		}
	}
	[CommandLocalization(Localization.SnapStringId.InsertSectionBreakNextPageDataRowSeparatorCommand_MenuCaption, Localization.SnapStringId.InsertSectionBreakNextPageDataRowSeparatorCommand_Description)]
	public class InsertSectionBreakNextPageDataRowSeparatorCommand : InsertSectionBreakDataRowSeparatorCommandBase {
		public InsertSectionBreakNextPageDataRowSeparatorCommand(IRichEditControl control)
			: base(control) {
		}
		public override RichEditCommandId Id { get { return SnapCommandId.InsertSectionBreakNextPageDataRowSeparator; } }
		public override string ImageName { get { return "SectionBreaksList_NextPage"; } }
		public override SectionStartType StartType { get { return SectionStartType.NextPage; } }
		protected override ITemplateModifier CreateTemplateModifier() {
			return new InsertSectionBreakNextPageSeparatorCommandInternal(DocumentModel);
		}
	}
	[CommandLocalization(Localization.SnapStringId.InsertSectionBreakEvenPageDataRowSeparatorCommand_MenuCaption, Localization.SnapStringId.InsertSectionBreakEvenPageDataRowSeparatorCommand_Description)]
	public class InsertSectionBreakEvenPageDataRowSeparatorCommand : InsertSectionBreakDataRowSeparatorCommandBase {
		public InsertSectionBreakEvenPageDataRowSeparatorCommand(IRichEditControl control)
			: base(control) {
		}
		public override RichEditCommandId Id { get { return SnapCommandId.InsertSectionBreakEvenPageDataRowSeparator; } }
		public override string ImageName { get { return "SectionBreaksList_EvenPage"; } }
		public override SectionStartType StartType { get { return SectionStartType.EvenPage; } }
		protected override ITemplateModifier CreateTemplateModifier() {
			return new InsertSectionBreakEvenPageSeparatorCommandInternal(DocumentModel);
		}
	}
	[CommandLocalization(Localization.SnapStringId.InsertSectionBreakOddPageDataRowSeparatorCommand_MenuCaption, Localization.SnapStringId.InsertSectionBreakOddPageDataRowSeparatorCommand_Description)]
	public class InsertSectionBreakOddPageDataRowSeparatorCommand : InsertSectionBreakDataRowSeparatorCommandBase {
		public InsertSectionBreakOddPageDataRowSeparatorCommand(IRichEditControl control)
			: base(control) {
		}
		public override RichEditCommandId Id { get { return SnapCommandId.InsertSectionBreakOddPageDataRowSeparator; } }
		public override string ImageName { get { return "SectionBreaksList_OddPage"; } }
		public override SectionStartType StartType { get { return SectionStartType.OddPage; } }
		protected override ITemplateModifier CreateTemplateModifier() {
			return new InsertSectionBreakOddPageSeparatorCommandInternal(DocumentModel);
		}
	}
	public class SeparatorTemplateComparer {
		readonly SnapObjectModelController controller;
		public SeparatorTemplateComparer(SnapPieceTable pieceTable) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			controller = new SnapObjectModelController(pieceTable);
		}
		protected SnapObjectModelController Controller { get { return controller; } }
		public bool IsPageBreak(DocumentLogInterval interval) {
			return interval.Length == 1 && controller.GetRunPlainText(interval.Start)[0] == Characters.PageBreak;
		}
		public bool IsEmptyParagraph(DocumentLogInterval interval) {
			if (interval.Length != 1)
				return false;
			ParagraphRun paragraphRun = Controller.FindRunByLogPosition(interval.Start) as ParagraphRun;
			return paragraphRun != null && !paragraphRun.Paragraph.IsInCell();
		}
		public bool IsEmptyRow(DocumentLogInterval interval) {
			if (interval.Length != 1)
				return false;
			ParagraphRun paragraphRun = Controller.FindRunByLogPosition(interval.Start) as ParagraphRun;
			if (paragraphRun == null)
				return false;
			Paragraph paragraph = paragraphRun.Paragraph;
			ParagraphIndex index = paragraph.Index;
			TableCell cell = paragraph.GetCell();
			return cell != null && cell.StartParagraphIndex == index && cell.EndParagraphIndex == index;
		}
		public bool IsSectionBreak(DocumentLogInterval interval, SectionStartType startType) {
			if (interval.Length != 2)
				return false;
			DocumentLogPosition pos = interval.Start;
			SectionRun sectionRun = Controller.FindRunByLogPosition(pos) as SectionRun;
			if (sectionRun == null || Controller.GetSectionByRun(sectionRun).GeneralSettings.StartType != startType)
				return false;
			return Controller.FindRunByLogPosition(pos + 1) is ParagraphRun;
		}
	}
	public class SeparatorTemplateExecutor {
		public void InsertPageBreakSeparator(PieceTable target) {
			string pageBreak = new string(Characters.PageBreak, 1);
			target.InsertText(DocumentLogPosition.Zero, pageBreak);
		}
		public void InsertEmptyParagraphSeparator(PieceTable target) {
			target.InsertParagraph(DocumentLogPosition.Zero);
			target.InsertParagraph(DocumentLogPosition.Zero);
		}
		public void InsertSectionSeparator(PieceTable pieceTable, SectionStartType startType) {
			pieceTable.InsertParagraph(DocumentLogPosition.Zero);
			pieceTable.DocumentModel.InsertSection(DocumentLogPosition.Zero);
			pieceTable.DocumentModel.Sections[new SectionIndex(0)].GeneralSettings.StartType = startType;
			pieceTable.InsertParagraph(DocumentLogPosition.Zero);
		}
	}
}
