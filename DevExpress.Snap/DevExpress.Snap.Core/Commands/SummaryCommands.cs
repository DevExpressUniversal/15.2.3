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
using System.Text;
using DevExpress.XtraRichEdit;
using DevExpress.Snap.Core.Native;
using DevExpress.Snap.Core.Native.Templates;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Snap.Core.Fields;
using DevExpress.Data.Browsing.Design;
using DevExpress.Snap.Core.Native.Data;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.Data;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.Data.Browsing;
namespace DevExpress.Snap.Core.Commands {
	[CommandLocalization(Localization.SnapStringId.SummaryCommand_MenuCaption, Localization.SnapStringId.SummaryCommand_Description)]
	public class SummaryCommand : DropDownCommandBase {
		public SummaryCommand(IRichEditControl control)
			: base(control) {
		}
		public override string ImageName {
			get {
				return "Summary";
			}
		}
		public override RichEditCommandId[] GetChildCommandIds() {
			return new RichEditCommandId[] {
				SnapCommandId.SummaryCount,
				SnapCommandId.SummarySum,
				SnapCommandId.SummaryAverage,
				SnapCommandId.SummaryMin,
				SnapCommandId.SummaryMax
			};
		}
	}
	public abstract class SumByFieldCommandBase : EditListFromInnerFieldCommandBase {
		protected SumByFieldCommandBase(IRichEditControl control)
			: base(control) {
		}
		SNTextField SNTextField { get; set; }
		protected override bool IsEnabled() {
			SnapFieldInfo snapFieldInfo = FindDataField();
			if (snapFieldInfo == null)
				return false;
			SnapFieldCalculatorService calculator = new SnapFieldCalculatorService();
			SNMergeFieldBase parsedInfo = calculator.ParseField(snapFieldInfo.PieceTable, snapFieldInfo.Field) as SNMergeFieldBase;
			if(parsedInfo == null || parsedInfo is SNSparklineField)
				return false;
			IDataAccessService service = DocumentModel.GetService<IDataAccessService>();
			if (service == null)
				return false;
			SNDataInfo dataInfo = GetDataInfo();
			if (dataInfo == null)
				return false;
			DataMemberInfo dataMemberInfo = DataMemberInfo.Create(dataInfo.DataPaths);
			return service.AllowSum(dataInfo.Source, dataMemberInfo.ParentDataMemberInfo.DataMember, dataMemberInfo.ColumnName, SummaryItemType);
		}
		protected internal override void PrepareCommandInfo() {
			base.PrepareCommandInfo();
			SnapFieldInfo snapFieldInfo = FindDataField();
			if (snapFieldInfo == null)
				return;
			SnapFieldCalculatorService calculator = new SnapFieldCalculatorService();
			SNTextField parsedInfo = calculator.ParseField(snapFieldInfo.PieceTable, snapFieldInfo.Field) as SNTextField;
			SNTextField = parsedInfo;			
		}
		protected override bool IsChecked() {
			return SNTextField != null && SNTextField.SummaryRunning != SummaryRunning.None && SNTextField.SummaryFunc == SummaryItemType;
		}
		protected abstract SummaryItemType SummaryItemType { get; }
		protected internal override void ExecuteCore() {
			if (SNTextField != null && SNTextField.SummaryRunning != SummaryRunning.None)
				ChangeExistingSummary();
			else
				CreateNewSummary();
		}
		void ChangeExistingSummary() {
			SnapFieldInfo fieldInfo = FindDataField();
			DocumentModel.BeginUpdate();
			try {
				using (InstructionController controller = fieldInfo.PieceTable.CreateFieldInstructionController(fieldInfo.Field)) {
					controller.SetSwitch(SNTextField.SummaryFuncSwitch, FieldsHelper.GetSummaryItemTypeDisplayString(SummaryItemType));
				}
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		void CreateNewSummary() {
			SNDataInfo dataInfo = GetDataInfo(true);
			if (dataInfo == null)
				return;
			SNListField snListField = EditedFieldInfo.ParsedInfo;
			String frameworkStringFormat = SNTextField != null ? SNTextField.FrameworkStringFormat : "{Null}";
			if(string.IsNullOrEmpty(frameworkStringFormat) && SummaryItemType == Data.SummaryItemType.Average)
				frameworkStringFormat = "0.000";
			SnapBookmark foundBookmark = snListField.GetGroupBookmarkByGroupIndex(DocumentModel, EditedFieldInfo.Field, 0, GroupBookmarkKind.GroupHeaderOrGroupFooter);
			if (foundBookmark != null) {
				SnapBookmark footerBookmark = snListField.GetGroupBookmarkByGroupIndex(DocumentModel, EditedFieldInfo.Field, 0, GroupBookmarkKind.GroupFooter);
				if (footerBookmark == null) {
					FieldPathInfo info = FieldPathService.FromString(snListField.DataSourceName);
					string groupFieldName = info.DataMemberInfo.Items[0].Groups[0].GroupFieldInfos[0].FieldName;
					InsertGroupFooterCommand2 command = new InsertGroupFooterCommand2(this.Control, EditedFieldInfo, groupFieldName, dataInfo.DisplayName, dataInfo.DisplayName, SummaryItemType, frameworkStringFormat);
					command.ExecuteCore();
				} else {
					UpdateTemplate(dataInfo, footerBookmark, "group", frameworkStringFormat);
				}
			}
			else {
				DocumentModel.BeginUpdate();
				try {
					new InsertListFooterCommand(this.Control).Execute();
					SnapBookmark lastContentBookmark = snListField.GetLastContentBookmark(EditedFieldInfo.PieceTable, EditedFieldInfo.Field);
					if (lastContentBookmark == null)
						return;
					UpdateTemplate(dataInfo, lastContentBookmark, "report", frameworkStringFormat);
				}
				finally {
					DocumentModel.EndUpdate();
				}
			}
		}
		void UpdateTemplate(SNDataInfo dataInfo, SnapBookmark footerBookmark, string running, string stringFormat) {
			Paragraph paragraph = ActivePieceTable.FindParagraph(footerBookmark.Start);
			TableCell tableCell = paragraph.GetCell();
			bool tabledtemplate = false;
			if (tableCell != null) 
				tabledtemplate = EditedFieldInfo.Field.Code.Start < ActivePieceTable.Paragraphs[tableCell.StartParagraphIndex].GetFirstRunIndex();
			if (tabledtemplate) {
				TableRow lastTableRow = tableCell.Table.LastRow;
				DocumentModel.BeginUpdate();
				try {
					if (lastTableRow.Cells.Count > 1)
						PrepareSummaryRow(lastTableRow);
					SummaryTemplateUpdater rowTemplateSummaryUpdater = new SummaryTemplateUpdater(tableCell, dataInfo, SummaryItemType, running, stringFormat);
					rowTemplateSummaryUpdater.UpdateTemplates();
				} finally {
					DocumentModel.EndUpdate();
				}
			} else {
				DocumentModel.BeginUpdate();
				try {
					string fieldCode = SNTextField.GetSummaryFieldCode(dataInfo.DataPaths[dataInfo.EscapedDataPaths.Length - 1], running, SummaryItemType) + " ";
					ActivePieceTable.InsertText(footerBookmark.Start, fieldCode);
					Field field = ActivePieceTable.CreateField(footerBookmark.Start, fieldCode.Length - 1);
					ActivePieceTable.FieldUpdater.UpdateFieldAndNestedFields(field);
					ActivePieceTable.InsertText(footerBookmark.Start, SNTextField.GetDisplaySummaryString(dataInfo.DisplayName, SummaryItemType));
				} finally {
					DocumentModel.EndUpdate();
				}
			}
		}
		void PrepareSummaryRow(TableRow lastTableRow) {
			SnapObjectModelController controller = new SnapObjectModelController((SnapPieceTable)ActivePieceTable);
			DocumentModel.Selection.Start = controller.FindCellStartLogPosition(lastTableRow.FirstCell);
			DocumentModel.Selection.End = DocumentModel.Selection.Start;
			new InsertTableRowBelowCommand(this.Control).Execute();
			lastTableRow = lastTableRow.Table.LastRow;
			SelectTableRow(lastTableRow);
			new MergeTableCellsCommand(this.Control).PerformModifyModel();
		}
		void SelectTableRow(TableRow row) {
			SelectTableRowCommand selectRowCommand = new SelectTableRowCommand(this.Control);
			selectRowCommand.CanCalculateExecutionParameters = false;
			selectRowCommand.ShouldEnsureCaretVisibleVerticallyAfterUpdate = false;
			selectRowCommand.Rows = row.Table.Rows;
			selectRowCommand.StartRowIndex = row.IndexInTable;
			selectRowCommand.EndRowIndex = row.IndexInTable;
			selectRowCommand.PerformChangeSelection();
		}
	}
	[CommandLocalization(Localization.SnapStringId.SummarySumCommand_MenuCaption, Localization.SnapStringId.SummarySumCommand_Description)]
	public class SummarySumCommand : SumByFieldCommandBase {
		public SummarySumCommand(IRichEditControl control)
			: base(control) {
		}
		protected override SummaryItemType SummaryItemType {
			get { return SummaryItemType.Sum; }
		}
	}
	[CommandLocalization(Localization.SnapStringId.SummaryCountCommand_MenuCaption, Localization.SnapStringId.SummaryCountCommand_Description)]
	public class SummaryCountCommand : SumByFieldCommandBase {
		public SummaryCountCommand(IRichEditControl control)
			: base(control) {
		}
		protected override SummaryItemType SummaryItemType {
			get { return SummaryItemType.Count; }
		}
	}
	[CommandLocalization(Localization.SnapStringId.SummaryAverageCommand_MenuCaption, Localization.SnapStringId.SummaryAverageCommand_Description)]
	public class SummaryAverageCommand : SumByFieldCommandBase {
		public SummaryAverageCommand(IRichEditControl control)
			: base(control) {
		}
		protected override SummaryItemType SummaryItemType {
			get { return SummaryItemType.Average; }
		}
	}
	[CommandLocalization(Localization.SnapStringId.SummaryMinCommand_MenuCaption, Localization.SnapStringId.SummaryMinCommand_Description)]
	public class SummaryMinCommand : SumByFieldCommandBase {
		public SummaryMinCommand(IRichEditControl control)
			: base(control) {
		}
		protected override SummaryItemType SummaryItemType {
			get { return SummaryItemType.Min; }
		}
	}
	[CommandLocalization(Localization.SnapStringId.SummaryMaxCommand_MenuCaption, Localization.SnapStringId.SummaryMaxCommand_Description)]
	public class SummaryMaxCommand : SumByFieldCommandBase {
		public SummaryMaxCommand(IRichEditControl control)
			: base(control) {
		}
		protected override SummaryItemType SummaryItemType {
			get { return SummaryItemType.Max; }
		}
	}
	public class InsertGroupFooterCommand2 : InsertGroupFooterCommand {
		readonly string summaryDisplayDataFieldName;
		readonly string frameworkFormatString;
		public InsertGroupFooterCommand2(IRichEditControl richEditControl, SnapListFieldInfo snapListFieldInfo, string dataFieldName, string summaryDataFieldName, string summaryDisplayDataFieldName, SummaryItemType summaryItemType, string frameworkFormatString)
			: base(richEditControl) {
			this.EditedFieldInfo = snapListFieldInfo;
			SummaryType = summaryItemType;
			DataFieldName = dataFieldName;
			DataFieldType = SNTextField.FieldType;
			this.SummaryDataFieldName = summaryDataFieldName;
			this.summaryDisplayDataFieldName = summaryDisplayDataFieldName;
			this.frameworkFormatString = frameworkFormatString;
		}
		protected override string GetSummaryDisplayDataFieldName() {
			return summaryDisplayDataFieldName;
		}
		protected override string GetFieldCodeCore() {
			StringBuilder sb = new StringBuilder(base.GetFieldCodeCore());
			sb.AppendFormat(" \\{0} \"{1}\"", SNTextField.FrameworkStringFormatSwitch, frameworkFormatString);
			return sb.ToString();
		}
	}
}
