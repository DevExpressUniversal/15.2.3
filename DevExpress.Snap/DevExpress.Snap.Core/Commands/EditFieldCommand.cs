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
using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit;
using DevExpress.Utils;
using DevExpress.Snap.Core.Native;
using DevExpress.Snap.Core.Native.Data;
using DevExpress.Data;
using DevExpress.Snap.Core.Fields;
using DevExpress.Data.Browsing.Design;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.Utils.Commands;
using System.Collections.Generic;
using DevExpress.Office.Utils;
using DevExpress.Snap.Core.Native.Services;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.Snap.Core.Commands {
	#region DropDownCommandBase
	public abstract class DropDownCommandBase : SnapMenuItemSimpleCommand {
		protected DropDownCommandBase(IRichEditControl control) : base(control) {
		}
		public abstract RichEditCommandId[] GetChildCommandIds();
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = IsEnabled();
			state.Checked = IsChecked();
		}		
		protected virtual bool IsEnabled() {
			RichEditCommandId[] childCommandIds = GetChildCommandIds();
			if (childCommandIds == null)
				return false;
			foreach (RichEditCommandId id in childCommandIds) {
				Command command = Control.CreateCommand(id);
				if (command == null)
					continue;
				if (command.CanExecute())
					return true;
			}
			return false;
		}
		protected virtual bool IsChecked() {
			return false;
		}
		public override void Execute() {
		}
		protected internal override void ExecuteCore() {
		}		
	}
	#endregion
	public abstract class EditFieldCommandBase<TField, TFieldInfo> : SnapMenuItemSimpleCommand
		where TField : CalculatedFieldBase
		where TFieldInfo : SnapFieldInfoBase<TField> {
		TFieldInfo editedFieldInfo;
		protected EditFieldCommandBase(IRichEditControl control)
			: base(control) {
		}
		public TFieldInfo EditedFieldInfo { get { return editedFieldInfo; } protected set { editedFieldInfo = value; } }
		protected virtual bool TryToKeepFieldSelection { get { return false; } }
		protected override void UpdateUIStateCore(ICommandUIState state) {
			editedFieldInfo = GetEditedField();
			PrepareCommandInfo();
			UpdateUIStateCoreInternal(state);
		}
		protected virtual void UpdateUIStateCoreInternal(ICommandUIState state) {
			ApplyCommandsRestriction(state, CharacterFormatting);
			if (state.Enabled)
				state.Enabled = IsEnabled();
			state.Checked = IsChecked();
		}
		protected virtual bool IsChecked() {
			return false;
		}
		protected virtual bool IsEnabled() {
			return EditedFieldInfo != null;
		}
		protected abstract TFieldInfo GetEditedField();
		protected internal virtual void PrepareCommandInfo() {
		}
		bool IsEditedFieldWholeSelected() {
			if (DocumentModel.Selection.PieceTable != EditedFieldInfo.PieceTable)
				return false;
			RunInfo runInfo = EditedFieldInfo.PieceTable.FindRunInfo(DocumentModel.Selection.NormalizedStart, DocumentModel.Selection.NormalizedEnd - DocumentModel.Selection.NormalizedStart);
			return DocumentModel.SelectionInfo.IsWholeFieldSelected(EditedFieldInfo.PieceTable, EditedFieldInfo.Field, runInfo.Start.RunIndex, runInfo.End.RunIndex);
		}
		protected internal override void ExecuteCore() {
			DocumentModel.BeginUpdate();
			try {
				bool isWholeFieldSelected = IsEditedFieldWholeSelected();
				ExcecuteCoreInternal();
				if (TryToKeepFieldSelection && isWholeFieldSelected) {
					SelectWholeListGroupCommand command = new SelectWholeListGroupCommand(Control);
					command.Field = EditedFieldInfo.Field;
					command.PerformChangeSelection();
				}
				EnsureSelectionValid();
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected virtual void ExcecuteCoreInternal() {
			SnapPieceTable pieceTable = EditedFieldInfo.PieceTable;
			using (InstructionController controller = pieceTable.CreateFieldInstructionController(EditedFieldInfo.Field)) {
				if (controller != null)
					UpdateFieldCode(controller);
			}
		}
		void EnsureSelectionValid() {
			if (IsCaretPositionInvalid()) {
				DocumentLogPosition logPosition = GetNewCaretPosition();
				DocumentModel.Selection.Start = logPosition;
				DocumentModel.Selection.End = logPosition;
			}
		}
		protected internal virtual void UpdateFieldCode(InstructionController controller) {
		}
		bool IsCaretPositionInvalid() {
			Selection selection = DocumentModel.Selection;
			return selection.Length == 0 && selection.NormalizedStart == GetFieldResultEndRunPosition();
		}
		DocumentLogPosition GetFieldResultEndRunPosition() {
			return EditedFieldInfo.PieceTable.GetRunLogPosition(EditedFieldInfo.Field.LastRunIndex);
		}
		protected virtual DocumentLogPosition GetNewCaretPosition() {
			return EditedFieldInfo.PieceTable.GetRunLogPosition(EditedFieldInfo.Field.Result.Start);
		}
	}	
	public class ListFieldSelectionController {		
	SnapBookmark bookmark;
	SnapListFieldInfo listFieldInfo;
	SnapDocumentModel documentModel;
	public ListFieldSelectionController(SnapDocumentModel documentModel) {
		Guard.ArgumentNotNull(documentModel, "documentModel");
		this.documentModel = documentModel;
	}
	SnapPieceTable PieceTable { get { return listFieldInfo.PieceTable; } }
	SnapDocumentModel DocumentModel { get { return documentModel; } }
	SnapListFieldInfo ListFieldInfo { get { return listFieldInfo; } }
	public SnapBookmark FindBookmark() {
		Selection selection = DocumentModel.Selection;
		SnapPieceTable pieceTable = (SnapPieceTable)selection.PieceTable;
		SnapBookmarkController controller = new SnapBookmarkController(pieceTable);
		bookmark = controller.FindInnermostTemplateBookmarkByPosition(selection.NormalizedStart);
		return bookmark;
	}
	public SnapListFieldInfo FindListField() {
		if (bookmark == null && FindBookmark() == null)
			return null;
		listFieldInfo = FindListFieldCore();
		return listFieldInfo;
	}
	SnapListFieldInfo FindListFieldCore() {
		Selection selection = DocumentModel.Selection;
		SnapPieceTable pieceTable = (SnapPieceTable)selection.PieceTable;
		RunInfo selectionRunInfo = selection.Interval;
		SnapListFieldInfo field = FindSNListByRunIndex(pieceTable, selectionRunInfo.NormalizedStart.RunIndex);
		if (field == null)
			return null;
		if (selection.Length == 0)
			return field;
		DocumentModelPosition selectionEnd = selectionRunInfo.NormalizedEnd;
		if (field.Field.ContainsRun(selectionEnd.RunIndex) || (selectionEnd.RunIndex == field.Field.LastRunIndex + 1 && selectionEnd.RunOffset == 0))
			return field;
		else
			return null;
	}
	public static SnapListFieldInfo FindSNListByRunIndex(SnapPieceTable pieceTable, RunIndex runIndex) {
		SnapFieldCalculatorService calculator = new SnapFieldCalculatorService();
		Field result = pieceTable.FindFieldByRunIndex(runIndex);
		while (result != null) {
			SNListField snListField = calculator.ParseField(pieceTable, result) as SNListField;
			if (snListField != null)
				return new SnapListFieldInfo(pieceTable, result, snListField);
			result = result.Parent;
		}
		return null;
	}   
	public SnapFieldInfo FindDataField() {
		SnapFieldInfo selectedField = FieldsHelper.GetSelectedField(DocumentModel);
		if (selectedField != null && IsDataField(selectedField))
			return selectedField;
		if (listFieldInfo == null && FindListField() == null)
			return null;			
		Selection selection = DocumentModel.Selection;
		SnapPieceTable pieceTable = (SnapPieceTable)selection.PieceTable;
		if (selection.Length == 0)
			return FindDataFieldByCaretPosition(pieceTable, selection.Interval.NormalizedStart);
		else
			return FindDataFieldBySelectionIntervals(pieceTable, selection);
	}
	private SnapFieldInfo FindDataFieldByCaretPosition(SnapPieceTable pieceTable, DocumentModelPosition caretPosition) {
		if (bookmark == null)
			return null;
		ParagraphIndex paragraphIndex = caretPosition.ParagraphIndex;
		Paragraph paragraph = pieceTable.Paragraphs[paragraphIndex];
		TableCell paragraphCell = paragraph.GetCell();
		if (paragraphCell == null)
			return null;
		SnapTemplateInfo templateInfo = bookmark.TemplateInterval.TemplateInfo;
		SnapTemplateIntervalType templateType = templateInfo.TemplateType;
		switch(templateType) {
			case SnapTemplateIntervalType.DataRow:
				return FindDataFieldByCaretPositionInDataRow(pieceTable, caretPosition, paragraphCell);
			case SnapTemplateIntervalType.ListHeader:
			case SnapTemplateIntervalType.ListFooter:
				return FindDataFieldByCaretPositionInListHeaderOrFooter(pieceTable, caretPosition, paragraphCell);			
			case SnapTemplateIntervalType.GroupHeader:
			case SnapTemplateIntervalType.GroupFooter:
				return FindDataFieldByCaretPositionInGroupHeaderOrFooter(pieceTable, caretPosition, paragraphCell, templateInfo);
			default:
				return null;
		}
	}
	SnapFieldInfo FindSingleSNFieldInCell(SnapPieceTable pieceTable, TableCell cell) {
		while (cell != null && IsCellInsideList(pieceTable, cell)) {
			SnapFieldInfo field = GetSingleSNFieldInCell(pieceTable, cell);
			if (field != null)
				return field;
			cell = cell.Table.ParentCell;
		}
		return null;
	}
	SnapFieldInfo FindDataFieldByCaretPositionInDataRow(SnapPieceTable pieceTable, DocumentModelPosition caretPosition, TableCell caretCell) {
		return FindSingleSNFieldInCell(pieceTable, caretCell);
	}
	SnapFieldInfo FindDataFieldByCaretPositionInGroupHeaderOrFooter(SnapPieceTable pieceTable, DocumentModelPosition caretPosition, TableCell caretCell, SnapTemplateInfo templateInfo) {
		GroupProperties groupProperties = ListFieldInfo.ParsedInfo.GetGroupProperties(DocumentModel, templateInfo.FirstGroupIndex);
		if (groupProperties == null)
			return null;
		if (groupProperties.GroupFieldInfos.Count != 1)
			return null;
		List<Field> fields = pieceTable.GetFieldsInsideInterval(bookmark.Interval.NormalizedStart.RunIndex, bookmark.Interval.NormalizedEnd.RunIndex);
		if (fields == null || fields.Count == 0)
			return null;
		string expectedDataFieldName = groupProperties.GroupFieldInfos[0].FieldName;
		foreach (Field field in fields) {
			SnapFieldInfo fieldInfo = new SnapFieldInfo(pieceTable, field);
			MergefieldField mergeField = fieldInfo.ParsedInfo as MergefieldField;
			if (mergeField != null && String.CompareOrdinal(mergeField.DataFieldName, expectedDataFieldName) == 0)
				return fieldInfo;
		}
		return null;
	}
	SnapFieldInfo FindDataFieldByCaretPositionInListHeaderOrFooter(SnapPieceTable pieceTable, DocumentModelPosition caretPosition, TableCell caretCell) {
		TableCell cell = FindTopmostTableCellInList(pieceTable, caretCell);
		int leftColumnIndex = TableCommandsHelper.GetStartColumnIndex(cell);
		int rightColumnIndex = TableCommandsHelper.GetEndColumnIndex(cell);
		List<TableCell> tableCells = FindCellsByColumnIndexInDataRowBookmark(cell.Table, leftColumnIndex, rightColumnIndex);
		int count = tableCells.Count;
		if (count < 1)
			return null;
		SnapFieldInfo result = null;
		for (int i = 0; i < count; i++) {
			SnapFieldInfo fieldInfo = GetSingleSNFieldInCell(pieceTable, tableCells[i]);
			if (fieldInfo == null)
				continue;
			if (result != null) {
				if (!SameDataFieldName(result, fieldInfo))
					return null;
			}
			else
				result = fieldInfo;
		}
		return result;
	}
	private List<TableCell> FindCellsByColumnIndexInDataRowBookmark(Table table, int leftColumnIndex, int rightColumnIndex) {
		TableRowCollection rows = table.Rows;
		int count = rows.Count;
		int i = 0;
		SnapBookmark targetBookmark = null;
		for (; i < count; i++) {
			targetBookmark = TableRowHelper.FindTemplateBookmark(rows[i]);
			if (targetBookmark == null || targetBookmark.TemplateInterval.TemplateInfo.TemplateType != SnapTemplateIntervalType.DataRow)
				continue;
			else
				break;
		}
		List<TableCell> result = new List<TableCell>();
		for (; i < count; i++) {
			SnapBookmark bookmark = TableRowHelper.FindTemplateBookmark(rows[i]);
			if (bookmark != targetBookmark)
				break;
			TableCell cell = GetCellByStartAndEndColumnIndex(rows[i], leftColumnIndex, rightColumnIndex);
			if (cell != null && (cell.VerticalMerging == MergingState.None || cell.VerticalMerging == MergingState.Restart))
				result.Add(cell);
		}
		return result;
	}
	TableCell GetCellByStartAndEndColumnIndex(TableRow tableRow, int leftColumnIndex, int rightColumnIndex) {
		int columnIndex = tableRow.GridBefore;
		TableCellCollection cells = tableRow.Cells;
		int count = cells.Count;
		for (int i = 0; i < count; i++) {
			TableCell cell = cells[i];
			int columnSpan = cell.ColumnSpan;
			if (columnIndex == leftColumnIndex) {
				if (columnIndex + columnSpan - 1 == rightColumnIndex)
					return cell;
				else
					return null;
			}
			if (columnIndex > leftColumnIndex)
				return null;
			columnIndex += columnSpan;
		}
		return null;
	}
	private bool IsSingleGroupField(SnapFieldInfo field, SnapTemplateInfo templateInfo) {
		Guard.ArgumentNotNull(field, "field");
		MergefieldField mergeField = field.ParsedInfo as MergefieldField;
		Debug.Assert(mergeField != null);
		string dataFieldName = mergeField.DataFieldName;
		GroupProperties groupProperties = ListFieldInfo.ParsedInfo.GetGroupProperties(DocumentModel, templateInfo.FirstGroupIndex);
		if (groupProperties == null)
			return false;
		if (groupProperties.GroupFieldInfos.Count != 1)
			return false;
		return String.CompareOrdinal(dataFieldName, groupProperties.GroupFieldInfos[0].FieldName) == 0;
	}
	private SnapFieldInfo FindDataFieldBySelectionIntervals(SnapPieceTable pieceTable, Selection selection) {
		SnapFieldInfo startInfo = FindDataFieldByCaretPosition(pieceTable, selection.Interval.NormalizedStart);
		SnapFieldInfo endInfo = FindDataFieldByCaretPosition(pieceTable, selection.Interval.NormalizedEnd);
		if (Object.ReferenceEquals(startInfo, null) && Object.ReferenceEquals(endInfo, null))
			return null;
		if (Object.ReferenceEquals(startInfo, null))
			return endInfo;
		if (Object.ReferenceEquals(endInfo, null) || Object.ReferenceEquals(startInfo.Field, endInfo.Field))
			return startInfo;
		return null;
	}		
	SnapFieldInfo GetSingleSNFieldInCell(SnapPieceTable pieceTable, TableCell cell) {
		RunIndex firstRunIndex = pieceTable.Paragraphs[cell.StartParagraphIndex].FirstRunIndex;
		RunIndex lastRunIndex = pieceTable.Paragraphs[cell.EndParagraphIndex].LastRunIndex;
		List<Field> fields = pieceTable.GetFieldsInsideInterval(firstRunIndex, lastRunIndex);
		int count = fields.Count;
		if (count == 0)
			return null;
		SnapFieldInfo result = null;
		for (int i = 0; i < count; i++) {
			SnapFieldInfo fieldInfo = new SnapFieldInfo(pieceTable, fields[i]);
			if (!IsDataField(fieldInfo))
				continue;
			if (result != null) {
				if (!SameDataFieldName(fieldInfo, result))
					return null;
			}
			else
				result = fieldInfo;
		}
		return result;
	}
	bool IsDataField(SnapFieldInfo field) {
		return field.ParsedInfo is MergefieldField;
	}
	bool SameDataFieldName(SnapFieldInfo field1, SnapFieldInfo field2) {
		MergefieldField snMergeField1 = field1.ParsedInfo as MergefieldField;
		MergefieldField snMergeField2 = field2.ParsedInfo as MergefieldField;
		Debug.Assert(snMergeField1 != null && snMergeField2 != null);
		string dataFieldName1 = snMergeField1.DataFieldName;
		string dataFieldName2 = snMergeField2.DataFieldName;
		return String.CompareOrdinal(dataFieldName1, dataFieldName2) == 0;
	}
	bool IsCellInsideList(SnapPieceTable pieceTable, TableCell cell) {
		if (ListFieldInfo == null)
			return false;
		RunIndex firstRunIndex = pieceTable.Paragraphs[cell.StartParagraphIndex].FirstRunIndex;
		if (!ListFieldInfo.Field.ContainsRun(firstRunIndex))
			return false;
		RunIndex lastRunIndex = pieceTable.Paragraphs[cell.EndParagraphIndex].LastRunIndex;
		return ListFieldInfo.Field.ContainsRun(lastRunIndex);
	}
	protected TableCell FindTopmostTableCellInList(SnapPieceTable pieceTable, TableCell cell) {			
		while (cell != null) {
			TableCell parentCell = cell.Table.ParentCell;
			if (parentCell == null)
				return cell;
			ParagraphIndex startParagraphIndex = parentCell.StartParagraphIndex;
			RunIndex startRunIndex = pieceTable.Paragraphs[startParagraphIndex].FirstRunIndex;
			if (startRunIndex <= ListFieldInfo.Field.FirstRunIndex)
				return cell;
			cell = parentCell;	
		}
		return null;
	}	
}
	public abstract class EditListCommandBase : EditFieldCommandBase<SNListField, SnapListFieldInfo> {		
	SnapBookmark bookmark;
	IFieldPathService fieldPathService;
	ListFieldSelectionController listFieldSelectionController;
	protected new SnapDocumentModel DocumentModel { get { return (SnapDocumentModel)base.DocumentModel; } }
	protected EditListCommandBase(IRichEditControl control)
		: base(control) {
	}		
	protected IFieldPathService FieldPathService {
		get {
			if (fieldPathService == null) {
				IFieldDataAccessService service = DocumentModel.GetService<IFieldDataAccessService>();
				if (service != null)
					fieldPathService = service.FieldPathService;
			}
			return fieldPathService;
		}
	}
	protected SnapBookmark Bookmark {
		get { return bookmark; }
	}
	protected override SnapListFieldInfo GetEditedField() {
		listFieldSelectionController = new ListFieldSelectionController(DocumentModel);
		bookmark = listFieldSelectionController.FindBookmark();
		return listFieldSelectionController.FindListField();
	}
	protected SnapFieldInfo FindDataField() {
		if (EditedFieldInfo == null || bookmark == null)
			return null;
		return listFieldSelectionController.FindDataField();
	}
	protected SNDataInfo GetDataInfo() {
		return GetDataInfo(false);
	}
	protected SNDataInfo GetDataInfo(bool createNewSummary) {
		SnapFieldInfo fieldInfo = FindDataField();
		if (fieldInfo == null)
			return null;
		DesignBinding binding = FieldsHelper.GetFieldDesignBinding(DataSourceDispatcher, fieldInfo);
		if (binding == null || (binding.DataSource == null && String.IsNullOrEmpty(binding.DataMember)))
			return null;
		string fieldName = binding.DataMember;
		Field parent = fieldInfo.Field.Parent;
		string dataMember = string.Empty;
		if (parent != null) {
			SnapFieldInfo parentFieldInfo = new SnapFieldInfo(EditedFieldInfo.PieceTable, parent);
			DesignBinding parentDesignBinding = FieldsHelper.GetFieldDesignBinding(DataSourceDispatcher, parentFieldInfo);
			dataMember = parentDesignBinding.DataMember;
			if (String.CompareOrdinal(dataMember, binding.DataMember) == 0)
				fieldName = String.Empty;
			else
				fieldName = !string.IsNullOrEmpty(dataMember) ? binding.DataMember.Substring(dataMember.Length + 1) : binding.DataMember;
		}
		string displayName = FieldsHelper.GetFieldDisplayName(DataSourceDispatcher, this.EditedFieldInfo, fieldName, fieldName);
		if(createNewSummary) {  
			SNTextField textField = fieldInfo.ParsedInfo as SNTextField;
			if(textField != null && textField.DataFieldName.Contains("\\"))
				return new SNDataInfo(binding.DataSource, string.Join(".", new[] { dataMember, fieldName }), new[] { dataMember, textField.DataFieldName }, displayName);
		}
		return new SNDataInfo(binding.DataSource, new [] { dataMember, fieldName }, displayName);
	}
}
	public abstract class EditListFromInnerFieldCommandBase : EditListCommandBase {
		#region inner classes
		protected enum GroupModifyType {
			None, 
			InsertHeader,
			InsertFooter,
			Both,
			InsertGroupSeparator
		}
		protected class GroupModifyInfo {
			static readonly GroupModifyInfo empty = new GroupModifyInfo();
			public static GroupModifyInfo Empty { get { return empty; } }
			readonly string newSwitch;
			readonly GroupModifyType modifyType;
			readonly DocumentModel groupTemplate;
			GroupModifyInfo() { }
			public GroupModifyInfo(string newSwitch, GroupModifyType modifyType, DocumentModel groupTemplate) {
				Guard.ArgumentIsNotNullOrEmpty(newSwitch, "newSwitch");
				Guard.ArgumentNotNull(groupTemplate, "groupTemplate");
				this.newSwitch = newSwitch;
				this.modifyType = modifyType;
				this.groupTemplate = groupTemplate;
			}
			public string NewSwitch { get { return newSwitch; } }
			public GroupModifyType ModifyType { get { return modifyType; } }
			public DocumentModel GroupTemplate { get { return groupTemplate; } }
		}
		#endregion
		GroupProperties currentGroupProperties;
		GroupFieldInfo currentGroupFieldInfo;
		SnapFieldInfo dataField;
		protected EditListFromInnerFieldCommandBase(IRichEditControl control)
			: base(control) {
		}
		public string DataFieldName { get; protected set; }
		public string DataFieldType { get; protected set; }
		protected internal override void PrepareCommandInfo() {
			base.PrepareCommandInfo();
			this.dataField = FindDataField();
			if (dataField == null)
				return;
			InstructionController controller = dataField.PieceTable.CreateFieldInstructionController(dataField.Field);
			DataFieldName = GetDataFieldName(controller);
			DataFieldType = GetDataFieldType(controller);			
		}
		protected string GetDisplayDataFieldName() {
			SnapFieldInfo snapFieldInfo = FindDataField();
			if (snapFieldInfo == null)
				return string.Empty;
			return FieldsHelper.GetFieldDisplayName(DataSourceDispatcher, this.EditedFieldInfo, DataFieldName, DataFieldName);
		}
		protected virtual string GetDataFieldName(InstructionController controller) {
			if (controller == null)
				return String.Empty;
			return controller.GetArgumentAsString(0);
		}
		protected virtual string GetDataFieldType(InstructionController controller) {
			return controller == null ? SNTextField.FieldType : controller.GetFieldType();
		}
		protected override bool IsEnabled() {
			if (!base.IsEnabled() || String.IsNullOrEmpty(DataFieldName))
				return false;
			IDataAccessService service = DocumentModel.GetService<IDataAccessService>();
			if (service == null)
				return false;
			SNDataInfo dataInfo = GetDataInfo();
			if (dataInfo == null)
				return false;
			DataMemberInfo dataMemberInfo = DataMemberInfo.Create(dataInfo.DataPaths);
			return service.AllowGroupAndSort(dataInfo.Source, dataMemberInfo.ParentDataMemberInfo.DataMember, dataMemberInfo.ColumnName);
		}
		protected virtual DesignBinding GetDesignBinding() {
			SnapFieldInfo fieldInfo = FindDataField();
			return fieldInfo != null ? FieldsHelper.GetFieldDesignBinding(DataSourceDispatcher, fieldInfo) : null;
		}
		protected bool HasGroupTemplates { get; private set; }
		protected internal override void UpdateFieldCode(InstructionController controller) {
			string argument = controller.GetArgumentAsString(0);
			FieldPathInfo info = FieldPathService.FromString(argument);
			info.DataMemberInfo.EnsureItemsNotEmpty();
			ProcessGroups(info.DataMemberInfo.LastItem);
			controller.SetArgument(0, FieldPathService.GetStringPath(info));
		}
		protected internal virtual void ProcessGroups(FieldDataMemberInfoItem item) {
			if (ShouldAddNewGroup(item))
				AddNewGroup(item, UpdateGroupInfo, GroupModifyInfo.Empty);
		}
		bool ShouldAddNewGroup(FieldDataMemberInfoItem item) {
			if (!item.HasGroups)
				return true;
			int count = item.Groups.Count;
			for (int i = 0; i < count; i++) {
				if (ProcessGroupProperties(item.Groups[i])) {
					if (item.Groups[i].GroupFieldInfos.Count == 0)
						item.Groups.RemoveAt(i);
					return false;
				}
			}
			return true;
		}
		protected internal virtual bool ProcessGroupProperties(GroupProperties properties) {
			int count = properties.GroupFieldInfos.Count;
			for (int i = 0; i < count; i++) {
				GroupFieldInfo groupInfo = properties.GroupFieldInfos[i];
				if (groupInfo.FieldName == DataFieldName) {
					UpdateGroupInfo(properties, groupInfo, GroupModifyInfo.Empty);
					if (groupInfo.SortOrder == ColumnSortOrder.None)
						properties.GroupFieldInfos.RemoveAt(i);
					return true;
				}
			}
			return false;
		}
		protected delegate void ModifyGroupPropertiesAction<T>(GroupProperties groupProperties, GroupFieldInfo groupFieldInfo, T modifyInfo);
		protected virtual void AddNewGroup<T>(FieldDataMemberInfoItem item, ModifyGroupPropertiesAction<T> modifyAction, T modifyInfo) {
			GroupFieldInfo groupInfo = new GroupFieldInfo(DataFieldName);
			GroupProperties groupProperties = new GroupProperties();
			groupProperties.GroupFieldInfos.Add(groupInfo);
			modifyAction(groupProperties, groupInfo, modifyInfo);
			if (groupProperties.HasGroupTemplates)
				item.InsertGroup(groupProperties);
			else
				item.AddGroup(groupProperties);
		}
		protected virtual void UpdateGroupInfo(GroupProperties groupProperties, GroupFieldInfo groupFieldInfo, GroupModifyInfo modifyInfo) {
		}
		protected FieldPathDataMemberInfo GetCorrespondingDataMemberInfo() {
			if (EditedFieldInfo == null)
				return null;
			SnapPieceTable pieceTable = EditedFieldInfo.PieceTable;
			InstructionController controller = pieceTable.CreateFieldInstructionController(EditedFieldInfo.Field);
			if (controller == null || FieldPathService == null)
				return null;
			return FieldPathService.FromString(controller.GetArgumentAsString(0)).DataMemberInfo;
		}
		protected GroupProperties GetCorrespondingGroupProperties() {
			if (currentGroupProperties == null)
				GetCorrespondingGroupInfo();
			return currentGroupProperties;
		}
		protected internal GroupFieldInfo GetCorrespondingGroupFieldInfo() {
			if (currentGroupFieldInfo == null)
				GetCorrespondingGroupInfo();
			return currentGroupFieldInfo;
		}
		protected void GetCorrespondingGroupInfo() {
			if (String.IsNullOrEmpty(DataFieldName))
				return;
			FieldPathDataMemberInfo dataMemberInfo = GetCorrespondingDataMemberInfo();
			if (dataMemberInfo == null || dataMemberInfo.IsEmpty || !dataMemberInfo.LastItem.HasGroups)
				return;
			foreach (GroupProperties groupProperties in dataMemberInfo.LastItem.Groups)
				foreach (GroupFieldInfo groupFieldInfo in groupProperties.GroupFieldInfos)
					if (groupFieldInfo.FieldName == DataFieldName) {
						HasGroupTemplates = CalcHasGroupTemplates(groupProperties);
						currentGroupFieldInfo = groupFieldInfo;
						currentGroupProperties = groupProperties;
					}
		}
		protected bool IsGrouped() {
			return GetCorrespondingGroupFieldInfo() != null && HasGroupTemplates;
		}
		protected virtual bool CalcHasGroupTemplates(GroupProperties groupProperties) {
			return groupProperties.HasGroupTemplates;
		}
	}
}
