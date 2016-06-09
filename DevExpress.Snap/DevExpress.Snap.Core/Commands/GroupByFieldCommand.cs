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
using DevExpress.Snap.Core.Native.Data;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.Snap.Core.Native.Templates;
using DevExpress.Snap.Core.Native;
using DevExpress.Snap.Core.Fields;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.Data.Browsing.Design;
using DevExpress.Snap.Core.Native.Data.Implementations;
using DevExpress.Data;
using DevExpress.Snap.Core.Native.Services;
namespace DevExpress.Snap.Core.Commands {
	public abstract class InsertGroupTemplateCommandBase : EditListFromInnerFieldCommandBase {
		protected InsertGroupTemplateCommandBase(IRichEditControl control)
			: base(control) {
		}
		protected FieldPathInfo FieldPathInfo { get; set; }
		protected virtual void BeginUpdate(InstructionController controller) {
			PrepareFieldPathInfo(controller);
		}
		protected void PrepareFieldPathInfo(InstructionController controller) {
			string argument = controller.GetArgumentAsString(0);
			FieldPathInfo = FieldPathService.FromString(argument);
			FieldPathInfo.DataMemberInfo.EnsureItemsNotEmpty();
		}
		protected virtual void EndUpdate(InstructionController controller) {
			controller.SetArgument(0, FieldPathService.GetStringPath(FieldPathInfo));
		}
		protected virtual void ModifyFieldGroupProperties<T>(ModifyGroupPropertiesAction<T> modifyAction, T modifyInfo) {
			ProcessGroups(FieldPathInfo.DataMemberInfo.LastItem, modifyAction, modifyInfo);
		}
		int GetLevel(Field field) {
			if (field.Parent == null) {
				SnapPieceTable peaceTable = ActivePieceTable as SnapPieceTable;
				DocumentModelPosition targetPosition = DevExpress.XtraRichEdit.Utils.PositionConverter.ToDocumentModelPosition(peaceTable, DocumentModel.Selection.End);
				TableCell targetCell = peaceTable.Paragraphs[targetPosition.ParagraphIndex].GetCell();
				Table targetTable = targetCell != null ? targetCell.Table : null;
				int level = 0;
				if (targetTable != null) {
					level += peaceTable.DocumentModel.GetDefaultListStyleNameLevel(targetTable.TableStyle.StyleName);
				}
				return level;
			}
			return field.GetLevel();
		}
		protected int GetGroupLevel(InstructionController controller) {
			string argument = controller.GetArgumentAsString(0);
			FieldPathInfo info = FieldPathService.FromString(argument);
			info.DataMemberInfo.EnsureItemsNotEmpty();
			int level = 1;
			if (info.DataMemberInfo.LastItem.Groups != null)
				info.DataMemberInfo.LastItem.Groups.ForEach(p => { level += Convert.ToInt32(p.HasGroupTemplates); });
			return level;
		}
		protected virtual GroupModifyInfo InsertGroupTemplate(InstructionController controller, GroupModifyType modifyType) {
			string newSwitch = controller.GetNonUsedSwitch();
			DocumentModel groupTemplate = CreateGroupTemplate(controller, modifyType);
			controller.SetSwitch(newSwitch, groupTemplate);
			return new GroupModifyInfo(newSwitch, modifyType, groupTemplate);
		}
		protected virtual DocumentModel CreateGroupTemplate(InstructionController controller, GroupModifyType modifyType) {
			int listLevel = GetLevel(controller.Field);
			int groupLevel = GetGroupLevel(controller);
			return GetGroupTemplateCore(controller, GetBaseStyleName(modifyType), listLevel, groupLevel, modifyType);
		}
		string GetBaseStyleName(GroupModifyType modifyType) {
			switch (modifyType) {
				case GroupModifyType.InsertHeader: return SnapDocumentModel.DefaultGroupHeaderStyleName;
				case GroupModifyType.InsertFooter: return SnapDocumentModel.DefaultGroupFooterStyleName;
				default: return String.Empty;
			}
		}
		protected virtual DocumentModel GetGroupTemplateCore(InstructionController controller, string baseStyleName, int listLevel, int level, GroupModifyType modifyType) {
			DocumentModel result = DocumentModel.CreateNew();
			result.IntermediateModel = true;
			result.BeginSetContent();
			try {
				SetGroupTemplateContent((SnapPieceTable)result.MainPieceTable, controller, baseStyleName, listLevel, level, modifyType);
			}
			finally {
				result.EndSetContent(DocumentModelChangeType.LoadNewDocument, false, null);
			}
			return result;
		}
		protected internal override bool ProcessGroupProperties(GroupProperties properties) {
			return ProcessGroupProperties(properties, null, GroupModifyInfo.Empty);
		}
		protected virtual bool ProcessGroupProperties<T>(GroupProperties properties, ModifyGroupPropertiesAction<T> modifyAction, T modifyInfo) {
			int count = properties.GroupFieldInfos.Count;
			for (int i = 0; i < count; i++) {
				GroupFieldInfo groupInfo = properties.GroupFieldInfos[i];
				if (groupInfo.FieldName == DataFieldName) {
					if (modifyAction != null)
						modifyAction(properties, groupInfo, modifyInfo);
					return true;
				}
			}
			return false;
		}
		protected virtual void SetGroupTemplateContent(SnapPieceTable targetPieceTable, InstructionController instructionController, string baseStyleName, int listLevel, int level, GroupModifyType modifyType) {
			if (targetPieceTable == null)
				return;
			Selection selection = DocumentModel.Selection;
			TemplateController templateController = new TemplateController((SnapPieceTable)selection.PieceTable);
			DocumentLogInterval interval = instructionController.Instructions.GetSwitchArgumentDocumentInterval(TemplatedFieldBase.TemplateSwitch);
			Table table;
			TemplateContentType type = (interval != null) ? templateController.GetTemplateType(interval, out table)
				: TemplateContentType.NoTemplate;
			TemplateWriter templateWriter = new TemplateWriter(targetPieceTable, new InputPosition(targetPieceTable));
			StartTableIfRequired(type, templateWriter);
			SetGroupTemplateContentCore(templateWriter, modifyType);
			templateWriter.WriteParagraphStart(baseStyleName + level);
			if (type == TemplateContentType.Table)
				ProcessTableTemplate(templateWriter, targetPieceTable, instructionController, baseStyleName, listLevel, level);
		}
		void ProcessTableTemplate(TemplateWriter templateWriter, SnapPieceTable targetPieceTable, InstructionController instructionController, string baseStyleName, int listLevel, int level) {
			int totalWidth = 0;
			ActiveView.EnsureFormattingCompleteForSelection();
			Table sourceTable = null;
			TableLayoutType initialTableLayoutType = TableLayoutType.Autofit;
			if (ActiveView.CaretPosition.Update(XtraRichEdit.Layout.DocumentLayoutDetailsLevel.TableCell)) {
				if (InnerControl.ActiveView.CaretPosition.LayoutPosition.TableCell != null)
					sourceTable = InnerControl.ActiveView.CaretPosition.LayoutPosition.TableCell.Cell.Table;
				else {
					TableCell cell = new SnapObjectModelController((SnapPieceTable)DocumentModel.Selection.PieceTable).FindTableCellBySeparator();
					if (cell != null)
						sourceTable = cell.Table;
				}
				if (sourceTable != null) {
					initialTableLayoutType = sourceTable.TableLayout;
					SnapToggleTableFixedColumnWidthCommand command = new SnapToggleTableFixedColumnWidthCommand(Control);
					command.ExecuteCore();
					instructionController.BeforeUpdateFields += new EventHandler(controller_BeforeUpdateFields);
					TableCellCollection cells = sourceTable.Rows.First.Cells;
					for (int i = cells.Count - 1; i >= 0; i--)
						totalWidth += cells[i].PreferredWidth.Value;
				}
			}
			string styleName = StyleHelper.GetGroupStyleName(baseStyleName, listLevel, level, targetPieceTable.DocumentModel); 
			TableCellStyle style = targetPieceTable.DocumentModel.TableCellStyles.GetStyleByName(styleName);
			templateWriter.EndTableCell(totalWidth, style);
			templateWriter.EndTableRow();
			Table resultTable = templateWriter.EndTable(sourceTable != null && sourceTable.TableProperties.UsePreferredWidth ? sourceTable.TableProperties.PreferredWidth.Info : null);
			if (sourceTable != null) {
				DocumentModelCopyCommand.CopyStyles(resultTable.DocumentModel, sourceTable.DocumentModel);
				string tableStyleName = sourceTable.DocumentModel.TableStyles[sourceTable.StyleIndex].StyleName;
				resultTable.StyleIndex = resultTable.DocumentModel.TableStyles.GetStyleIndexByName(tableStyleName);
				if (sourceTable.TableLayout != initialTableLayoutType)
					sourceTable.TableLayout = initialTableLayoutType;
				if (resultTable.TableLayout != initialTableLayoutType)
					resultTable.TableLayout = initialTableLayoutType;
			}
		}
		void StartTableIfRequired(TemplateContentType type, TemplateWriter templateWriter) {
			if (type == TemplateContentType.Table) {
				templateWriter.BeginTable();
				templateWriter.BeginTableRow();
				templateWriter.BeginTableCell();
			}
		}
		protected virtual void SetGroupTemplateContentCore(TemplateWriter templateWriter, GroupModifyType modifyType) {
		}
		void controller_BeforeUpdateFields(object sender, EventArgs e) {
			((SnapPieceTable)Control.InnerControl.DocumentModel.ActivePieceTable).UpdateTemplate(false);
		}
		protected abstract GroupModifyType GroupType { get; }
		protected abstract void ProcessGroups<T>(FieldDataMemberInfoItem item, ModifyGroupPropertiesAction<T> modifyAction, T modifyInfo);
	}
	public abstract class GroupByFieldCommandBase : InsertGroupTemplateCommandBase {
		public SummaryItemType SummaryType { get; protected set; }
		public string SummaryDataFieldName { get; protected set; }
		protected GroupByFieldCommandBase(IRichEditControl control)
			: base(control) {
				SummaryType = SummaryItemType.Count;
		}
		protected override bool IsChecked() {
			return IsGrouped();
		}
		protected internal override void PrepareCommandInfo() {
			base.PrepareCommandInfo();
			SummaryDataFieldName = DataFieldName;
		}
		protected override void ProcessGroups<T>(FieldDataMemberInfoItem item, ModifyGroupPropertiesAction<T> modifyAction, T modifyInfo) {
			if (ShouldAddNewGroup(item, modifyAction, modifyInfo))
				AddNewGroup(item, modifyAction, modifyInfo);
		}
		bool ShouldAddNewGroup<T>(FieldDataMemberInfoItem item, ModifyGroupPropertiesAction<T> modifyAction, T modifyInfo) {
			if (!item.HasGroups)
				return true;
			int count = item.Groups.Count;
			for (int i = 0; i < count; i++)
				if (ProcessGroupProperties(item.Groups[i], modifyAction, modifyInfo)) {
					item.MoveToGrouped(i);
					return false;
				}
			return true;
		}
		protected internal override void UpdateFieldCode(InstructionController controller) {
			if (!IsGrouped())
				Group(controller);
			else
				Ungroup(controller);
		}
		protected virtual void Group(InstructionController controller) {
			BeginUpdate(controller);
			try {
				if (GroupType != GroupModifyType.Both) {
					GroupModifyInfo modifyInfo = InsertGroupTemplate(controller, GroupType);
					ModifyFieldGroupProperties(UpdateGroupInfo, modifyInfo);
				}
				else {
					GroupModifyInfo modifyInfo = InsertGroupTemplate(controller, GroupModifyType.InsertHeader);
					ModifyFieldGroupProperties(UpdateGroupInfo, modifyInfo);
					modifyInfo = InsertGroupTemplate(controller, GroupModifyType.InsertFooter);
					ModifyFieldGroupProperties(UpdateGroupInfo, modifyInfo);
				}
			}
			finally {
				EndUpdate(controller);
			}
		}
		protected void Ungroup(InstructionController controller) {
			string argument = controller.GetArgumentAsString(0);
			FieldPathInfo info = FieldPathService.FromString(argument);
			ProcessGroups(controller, info.DataMemberInfo.LastItem);
			controller.SetArgument(0, FieldPathService.GetStringPath(info));
		}
		void ProcessGroups(InstructionController controller, FieldDataMemberInfoItem item) {
			int count = item.Groups.Count;
			for (int i = 0; i < count; i++) {
				GroupProperties groupProperties = item.Groups[i];
				if (ProcessGroupProperties(groupProperties)) {
					RemoveTemplateHeaderAndFooter(controller, groupProperties);
					if (!groupProperties.HasGroupTemplates)
						item.RemoveGroup(groupProperties);
					return;
				}
			}
		}
		protected override void UpdateGroupInfo(GroupProperties groupProperties, GroupFieldInfo groupFieldInfo, GroupModifyInfo modifyInfo) {
			if (groupFieldInfo.SortOrder == Data.ColumnSortOrder.None)
				groupFieldInfo.SortOrder = Data.ColumnSortOrder.Ascending;
			if (modifyInfo.ModifyType == GroupModifyType.InsertHeader)
				groupProperties.TemplateHeaderSwitch = modifyInfo.NewSwitch;
			if (modifyInfo.ModifyType == GroupModifyType.InsertFooter)
				groupProperties.TemplateFooterSwitch = modifyInfo.NewSwitch;
		}
		protected override void SetGroupTemplateContentCore(TemplateWriter templateWriter, GroupModifyType modifyType) {
			templateWriter.WriteText(GetDisplayDataFieldName() + ": ");
			templateWriter.BeginField();
			templateWriter.WriteText(String.Format("{0} {1}", DataFieldType, DataFieldName));
			templateWriter.EndField();
		}
		protected virtual void SetGroupSummaryTemplateContentCore(TemplateWriter templateWriter) {
			templateWriter.WriteText(SNTextField.GetDisplaySummaryString(GetSummaryDisplayDataFieldName(), SummaryType));
			templateWriter.BeginField();
			string fieldCode = GetFieldCodeCore();
			templateWriter.WriteText(fieldCode);
			templateWriter.EndField();
			templateWriter.WriteText(" ");
		}		
		protected virtual string GetSummaryDisplayDataFieldName() {
			return GetDisplayDataFieldName();
		}
		protected virtual string GetFieldCodeCore() {
			return SNTextField.GetSummaryFieldCode(SummaryDataFieldName, "group", SummaryType);
		}
		protected abstract void RemoveTemplateHeaderAndFooter(InstructionController controller, GroupProperties groupProperties);
	}
	public abstract class InsertRemoveGroupHeaderFooterCommandBase : GroupByFieldCommandBase {
		DesignBinding designBinding;
		SnapFieldInfo fieldInfo;
		protected InsertRemoveGroupHeaderFooterCommandBase(IRichEditControl control)
			: base(control) {
		}
		protected IDataSourceDispatcher DataSourceDispacher { get { return DocumentModel.DataSourceDispatcher; } }
		protected override bool IsEnabled() {
			if (EditedFieldInfo == null)
				return false;
			return IsCorrectBookmarkType();
		}
		protected bool IsCorrectBookmarkType() {
			return SnapObjectModelController.IsBookmarkCorrespondsToGroup(Bookmark);
		}
		protected internal override void PrepareCommandInfo() {
			if (EditedFieldInfo == null || !IsCorrectBookmarkType())
				return;
			GroupProperties groupProperties = EditedFieldInfo.ParsedInfo.GetGroupProperties(DocumentModel, Bookmark.TemplateInterval.TemplateInfo.FirstGroupIndex);
			DataFieldName = groupProperties.GroupFieldInfos[0].FieldName;
			DataFieldType = SNTextField.FieldType;
			fieldInfo = new SnapFieldInfo(EditedFieldInfo.PieceTable, EditedFieldInfo.Field);
			designBinding = FieldsHelper.GetFieldDesignBinding(DataSourceDispatcher, fieldInfo, DataFieldName);
			if (designBinding == null)
				return;
			SummaryDataFieldName = DataFieldName;
		}
		protected override DesignBinding GetDesignBinding() {
			return designBinding;
		}
	}
	public abstract class InsertGroupHeaderFooterCommandBase : InsertRemoveGroupHeaderFooterCommandBase {
		protected InsertGroupHeaderFooterCommandBase(IRichEditControl control)
			: base(control) {
		}
		protected override bool IsChecked() {
			return false;
		}
		protected override bool IsEnabled() {
			return base.IsEnabled() && !IsGrouped();
		}
		protected internal override void UpdateFieldCode(InstructionController controller) {
			Group(controller);
		}
		protected override void RemoveTemplateHeaderAndFooter(InstructionController controller, GroupProperties groupProperties) {
		}
	}
	public abstract class RemoveGroupHeaderFooterCommandBase : InsertRemoveGroupHeaderFooterCommandBase {
		protected RemoveGroupHeaderFooterCommandBase(IRichEditControl control)
			: base(control) {
		}
		protected override bool IsChecked() {
			return false;
		}
		protected override bool IsEnabled() {
			return base.IsEnabled() && IsGrouped();
		}
		protected internal override void UpdateFieldCode(InstructionController controller) {
			Ungroup(controller);
		}
	}
	[CommandLocalization(Localization.SnapStringId.InsertGroupHeaderCommand_MenuCaption, Localization.SnapStringId.InsertGroupHeaderCommand_Description)]
	public class InsertGroupHeaderCommand : InsertGroupHeaderFooterCommandBase {
		public InsertGroupHeaderCommand(IRichEditControl control)
			: base(control) {
		}
		protected override GroupModifyType GroupType { get { return GroupModifyType.InsertHeader; } }
		public override string ImageName { get { return "InsertGroupHeader"; } }
		protected override bool CalcHasGroupTemplates(GroupProperties groupProperties) {
			return !String.IsNullOrEmpty(groupProperties.TemplateHeaderSwitch);
		}
	}
	[CommandLocalization(Localization.SnapStringId.InsertGroupFooterCommand_MenuCaption, Localization.SnapStringId.InsertGroupFooterCommand_Description)]
	public class InsertGroupFooterCommand : InsertGroupHeaderFooterCommandBase {
		public InsertGroupFooterCommand(IRichEditControl control)
			: base(control) {
		}
		protected override GroupModifyType GroupType { get { return GroupModifyType.InsertFooter; } }
		public override string ImageName { get { return "InsertGroupFooter"; } }
		protected override bool CalcHasGroupTemplates(GroupProperties groupProperties) {
			return !String.IsNullOrEmpty(groupProperties.TemplateFooterSwitch);
		}
		protected override void SetGroupTemplateContentCore(TemplateWriter templateWriter, GroupModifyType modifyType) {
			SetGroupSummaryTemplateContentCore(templateWriter);
		}
	}
	[CommandLocalization(Localization.SnapStringId.RemoveGroupHeaderCommand_MenuCaption, Localization.SnapStringId.RemoveGroupHeaderCommand_Description)]
	public class RemoveGroupHeaderCommand : RemoveGroupHeaderFooterCommandBase {
		public RemoveGroupHeaderCommand(IRichEditControl control)
			: base(control) {
		}
		protected override GroupModifyType GroupType { get { return GroupModifyType.InsertHeader; } }
		public override string ImageName { get { return "RemoveGroupHeader"; } }
		protected override bool CalcHasGroupTemplates(GroupProperties groupProperties) {
			return !String.IsNullOrEmpty(groupProperties.TemplateHeaderSwitch);
		}
		protected override void RemoveTemplateHeaderAndFooter(InstructionController controller, GroupProperties groupProperties) {
			string headerSwitch = groupProperties.TemplateHeaderSwitch;
			if (!String.IsNullOrEmpty(headerSwitch))
				controller.RemoveSwitch(headerSwitch);
			groupProperties.TemplateHeaderSwitch = String.Empty;
		}
	}
	[CommandLocalization(Localization.SnapStringId.RemoveGroupFooterCommand_MenuCaption, Localization.SnapStringId.RemoveGroupFooterCommand_Description)]
	public class RemoveGroupFooterCommand : RemoveGroupHeaderFooterCommandBase {
		public RemoveGroupFooterCommand(IRichEditControl control)
			: base(control) {
		}
		protected override GroupModifyType GroupType { get { return GroupModifyType.InsertFooter; } }
		public override string ImageName { get { return "RemoveGroupFooter"; } }
		protected override bool CalcHasGroupTemplates(GroupProperties groupProperties) {
			return !String.IsNullOrEmpty(groupProperties.TemplateFooterSwitch);
		}
		protected override void RemoveTemplateHeaderAndFooter(InstructionController controller, GroupProperties groupProperties) {
			string footerSwitch = groupProperties.TemplateFooterSwitch;
			if (!String.IsNullOrEmpty(footerSwitch))
				controller.RemoveSwitch(footerSwitch);
			groupProperties.TemplateFooterSwitch = String.Empty;
		}
	}
	[CommandLocalization(Localization.SnapStringId.GroupByFieldCommand_MenuCaption, Localization.SnapStringId.GroupByFieldCommand_Description)]
	public class GroupByFieldCommand : GroupByFieldCommandBase {
		public GroupByFieldCommand(IRichEditControl control)
			: base(control) {
		}
		protected override GroupModifyType GroupType { get { return GroupModifyType.Both; } }
		public override string ImageName { get { return "GroupBy"; } }
		protected override void RemoveTemplateHeaderAndFooter(InstructionController controller, GroupProperties groupProperties) {
			string headerSwitch = groupProperties.TemplateHeaderSwitch;
			if (!String.IsNullOrEmpty(headerSwitch))
				controller.RemoveSwitch(headerSwitch);
			groupProperties.TemplateHeaderSwitch = String.Empty;
			string footerSwitch = groupProperties.TemplateFooterSwitch;
			if (!String.IsNullOrEmpty(footerSwitch))
				controller.RemoveSwitch(footerSwitch);
			groupProperties.TemplateFooterSwitch = String.Empty;
		}
		protected override bool CalcHasGroupTemplates(GroupProperties groupProperties) {
			return !String.IsNullOrEmpty(groupProperties.TemplateHeaderSwitch) ||
				!String.IsNullOrEmpty(groupProperties.TemplateFooterSwitch);
		}
		protected override void SetGroupTemplateContentCore(TemplateWriter templateWriter, EditListFromInnerFieldCommandBase.GroupModifyType modifyType) {
			if (modifyType != GroupModifyType.InsertFooter)
				base.SetGroupTemplateContentCore(templateWriter, modifyType);
			else
				SetGroupSummaryTemplateContentCore(templateWriter);
		}
	}
	[CommandLocalization(Localization.SnapStringId.GroupHeaderCommand_MenuCaption, Localization.SnapStringId.GroupHeaderCommand_Description)]
	public class GroupHeaderCommand : DropDownCommandBase {
		public GroupHeaderCommand(IRichEditControl control)
			: base(control) {
		}
		public override string ImageName { get { return "GroupHeader"; } }
		public override RichEditCommandId Id { get { return SnapCommandId.GroupHeader; } }
		public override RichEditCommandId[] GetChildCommandIds() {
			return new RichEditCommandId[] { SnapCommandId.InsertGroupHeader, SnapCommandId.RemoveGroupHeader };
		}
	}
	[CommandLocalization(Localization.SnapStringId.GroupFooterCommand_MenuCaption, Localization.SnapStringId.GroupFooterCommand_Description)]
	public class GroupFooterCommand : DropDownCommandBase {
		public GroupFooterCommand(IRichEditControl control)
			: base(control) {
		}
		public override string ImageName { get { return "GroupFooter"; } }
		public override RichEditCommandId Id { get { return SnapCommandId.GroupFooter; } }
		public override RichEditCommandId[] GetChildCommandIds() {
			return new RichEditCommandId[] { SnapCommandId.InsertGroupFooter, SnapCommandId.RemoveGroupFooter };
		}
	}
}
