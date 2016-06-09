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
using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Snap.Core.Native.Templates;
using DevExpress.Snap.Core.Native.Data;
using DevExpress.Snap.Core.Native.Data.Implementations;
using DevExpress.Snap.Core.Native;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Office;
using DevExpress.Snap.Core.Services;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.Snap.Core.Fields {
	public class SNListField : TemplatedFieldBase, IDataFieldNameOwner {
		internal const int DefaultEditorRowLimit = 20;		
		public static readonly string FieldType = "SNLIST";
		public static readonly string ListHeaderTemplateSwitch = "listheader";
		public static readonly string ListFooterTemplateSwitch = "listfooter";
		public static readonly string SeparatorTemplateSwitch = "separator";
		public static readonly string KeepLastSeparatorSwitch = "keeplastseparator";
		public static readonly string KeepLastSeparatorSwitchWithPrefix = "\\" + KeepLastSeparatorSwitch;
		public static readonly string EditorRowLimitSwitch = "erl";
		public static readonly string EditorRowLimitSwitchWithPrefix = "\\" + EditorRowLimitSwitch;
		public static readonly string NameSwitch = "name";
		public static readonly string NameSwitchWithPrefix = "\\" + NameSwitch;
		Dictionary<string, TemplateInfo> templateInfos = new Dictionary<string,TemplateInfo>();
		private int progressIndicatorToken;
		private ISnapProgressIndicationService progressIndicatorService = null;
		public static CalculatedFieldBase Create() {
			return new SNListField();
		}		
		public override void Initialize(PieceTable pieceTable, InstructionCollection switches) {
			base.Initialize(pieceTable,switches);			
			ListHeaderTemplateInterval = switches.GetSwitchArgumentDocumentInterval(ListHeaderTemplateSwitch);
			ListFooterTemplateInterval = switches.GetSwitchArgumentDocumentInterval(ListFooterTemplateSwitch);
			SeparatorTemplateInterval = switches.GetSwitchArgumentDocumentInterval(SeparatorTemplateSwitch);
			KeepLastSeparator = switches.GetBool(KeepLastSeparatorSwitch);			
			int? editorRowCountSwitchValue = switches.GetNullableInt(EditorRowLimitSwitch);			
			if (editorRowCountSwitchValue.HasValue)
				EditorRowLimit = editorRowCountSwitchValue.Value > 0 ? editorRowCountSwitchValue.Value : 0;
			else
				EditorRowLimit = DefaultEditorRowLimit;
			Name = switches.GetString(NameSwitch);
		}
		public DocumentLogInterval ListHeaderTemplateInterval { get; set; }
		public DocumentLogInterval ListFooterTemplateInterval { get; set; }
		public DocumentLogInterval SeparatorTemplateInterval { get; set; }
		public bool KeepLastSeparator { get; set; }
		public string Name { get; set; }
		public int EditorRowLimit { get; set; }
		protected override string FieldTypeName { get { return FieldType; } }
		TemplateInfo GetTemplateInfoBySwitch(TemplateController templateController, string templateSwitch, SnapTemplateIntervalType intervalType) {
			TemplateInfo result;
			if (templateInfos.TryGetValue(templateSwitch, out result))
				return result;
			result = templateController.GetTemplateInfo(Switches.GetSwitchArgumentDocumentInterval(templateSwitch), intervalType);
			if (intervalType == SnapTemplateIntervalType.GroupSeparator)
				result.SeparateAsPageBreakBefore = IsPageBreakSeparator(result);
			templateInfos.Add(templateSwitch, result);
			return result;
		}
		string GetRelativePath(FieldPathDataMemberInfo path) {
			String pathToList = String.Empty;
			if (path.Items.Count > 0) {
				for (int i = 0; i < path.Items.Count; i++)
					pathToList = PathHelper.Join(pathToList, path.Items[i].FieldName);
			}
			return pathToList;
		}
		protected void ProcessItems(SnapDocumentModel documentModel, IFieldContextService fieldContextService, FieldDataValueDescriptor fieldDataValueDescriptor, int maxRecordCount, string[] fieldNames, Action<IDataEnumerator> itemsAction) {
			IFieldContext parentDataContext = fieldDataValueDescriptor.ParentDataContext;
			ICalculationContext calculationContext = fieldContextService.BeginCalculation(documentModel.DataSourceDispatcher);
			if (!fieldDataValueDescriptor.RelativePath.IsEmpty && fieldDataValueDescriptor.RelativePath.Items.Count > 0) {
				string relativePath = GetRelativePath(fieldDataValueDescriptor.RelativePath);
				string[] joinedFieldNames = new string[fieldNames.Length + 1];
				Array.Copy(fieldNames, joinedFieldNames, fieldNames.Length);
				joinedFieldNames[fieldNames.Length] = relativePath;
				calculationContext.FieldNames = joinedFieldNames;
			}
			else 
				calculationContext.FieldNames = fieldNames;
			progressIndicatorService = documentModel.GetService(typeof(ISnapProgressIndicationService)) as ISnapProgressIndicationService;
			if(progressIndicatorService != null)
				progressIndicatorToken = progressIndicatorService.Begin("SNListField.GetResultItems", 0, Math.Max(fieldNames.Length, maxRecordCount), 0);
			try {
				using (IDataEnumerator enumerator = calculationContext.GetChildDataEnumerator(parentDataContext, fieldDataValueDescriptor.RelativePath)) {
					itemsAction(enumerator);
				}
			}
			finally {
				fieldContextService.EndCalculation(calculationContext);
				if(progressIndicatorService != null) {
					progressIndicatorService.End(this.progressIndicatorToken);
					progressIndicatorService = null;
				}
			}
		}
		public SnapBookmark GetGroupBookmarkByGroupIndex(SnapDocumentModel documentModel, Field field, int groupIndex, GroupBookmarkKind groupBookmarkKind) {
			IFieldPathService fieldPathService = ((IFieldDataAccessService)documentModel.GetService(typeof(IFieldDataAccessService))).FieldPathService;
			FieldPathInfo info = fieldPathService.FromString(DataSourceName);
			string templateHeaderSwitch = string.Empty;
			string templateFooterSwitch = string.Empty;
			if (info.DataMemberInfo.Items.Count > 0 && info.DataMemberInfo.Items[0].Groups != null) {
				templateHeaderSwitch = info.DataMemberInfo.Items[0].Groups[groupIndex].TemplateHeaderSwitch;
				templateFooterSwitch = info.DataMemberInfo.Items[0].Groups[groupIndex].TemplateFooterSwitch;
			}
			SnapBookmark bookmark = GetFirstContentBookmark(documentModel.MainPieceTable, field);
			SnapBookmark foundBookmark = null;
			DocumentModelPosition selectionStartlPosition = PositionConverter.ToDocumentModelPosition(documentModel.MainPieceTable, documentModel.Selection.Start);
			int delta = int.MaxValue;
			while (bookmark != null) {
				string templateSwitch = SnapBookmarksHelper.GetTemplateSwitch(this, bookmark);
				if ((templateSwitch == templateHeaderSwitch && (groupBookmarkKind & GroupBookmarkKind.GroupHeader) != 0) || (templateSwitch == templateFooterSwitch && (groupBookmarkKind & GroupBookmarkKind.GroupFooter) != 0)) {
					if (delta > Math.Abs(selectionStartlPosition.LogPosition - bookmark.End)) {
						delta = Math.Abs(selectionStartlPosition.LogPosition - bookmark.End);
						foundBookmark = bookmark;
					}
				}
				SnapObjectModelController controller = new SnapObjectModelController(bookmark.PieceTable);
				bookmark = controller.GetNextBookmark(bookmark);
			}
			return foundBookmark;
		}
		public SnapBookmark GetGroupBookmarkByGroupIndex(SnapDocumentModel documentModel, Field field, int groupIndex) {
			return GetGroupBookmarkByGroupIndex(documentModel, field, groupIndex, GroupBookmarkKind.GroupHeaderOrGroupFooter);
		}
		public int GetGroupIndex(SnapDocumentModel documentModel, int firstGroupIndex) {
			IFieldPathService fieldPathService = ((IFieldDataAccessService)documentModel.GetService(typeof(IFieldDataAccessService))).FieldPathService;
			FieldPathInfo info = fieldPathService.FromString(DataSourceName);
			return GetGroupIndexCore(info, firstGroupIndex);
		}
		public GroupProperties GetGroupProperties(SnapDocumentModel documentModel, int firstGroupIndex) {
			IFieldPathService fieldPathService = ((IFieldDataAccessService)documentModel.GetService(typeof(IFieldDataAccessService))).FieldPathService;
			FieldPathInfo info = fieldPathService.FromString(DataSourceName);
			int index = GetGroupIndexCore(info, firstGroupIndex);
			if (index < 0 || index >= info.DataMemberInfo.Items[0].Groups.Count)
				return null;
			return info.DataMemberInfo.Items[0].Groups[index];
		}
		protected int GetGroupIndexCore(FieldPathInfo info, int firstGroupIndex) {
			int totalIndex = 0;
			for (int i = 0; i < info.DataMemberInfo.Items[0].Groups.Count; i++) {
				GroupProperties groupProperties = info.DataMemberInfo.Items[0].Groups[i];
				if (groupProperties.HasGroupTemplates) {
					totalIndex += groupProperties.GroupFieldInfos.Count;
					if (totalIndex > firstGroupIndex)
						return i;
				}
			}
			return 0;
		}
		public virtual SnapBookmark GetFirstContentBookmark(SnapPieceTable pieceTable, Field field) {
			RunIndex firstRecordStart = GetFirstRecordStart(pieceTable, field);
			if (firstRecordStart == RunIndex.DontCare)
				return null;
			if (firstRecordStart >= field.Result.End)
				return null;
			SnapBookmarkController controller = new SnapBookmarkController(pieceTable);
			SnapBookmark bookmark = controller.FindInnermostTemplateBookmarkByPosition(firstRecordStart);
			System.Diagnostics.Debug.Assert(bookmark != null);
			DocumentModelPosition position = PositionConverter.ToDocumentModelPosition(pieceTable, bookmark.NormalizedStart);
			if (position.RunIndex < field.Result.Start || position.RunIndex > field.Result.End)
				return null;
			return bookmark;
		}
		RunIndex GetFirstRecordStart(SnapPieceTable snapPieceTable, Field field) {
			RunIndex firstRecordStart = field.Result.Start;
			if (snapPieceTable.Runs[firstRecordStart] is ParagraphRun) {
				if (snapPieceTable.Runs[firstRecordStart].Hidden == false)
					return RunIndex.DontCare;
				firstRecordStart++;
			}
			return firstRecordStart;
		}
		public virtual SnapBookmark GetLastContentBookmark(SnapPieceTable pieceTable, Field field) {
			RunIndex firstRecordStart = GetFirstRecordStart(pieceTable, field);
			if (firstRecordStart == RunIndex.DontCare)
				return null;
			RunIndex lastRecordEnd = field.Result.End - 1;
			if (lastRecordEnd < firstRecordStart)
				return null;
			SnapBookmarkController controller = new SnapBookmarkController(pieceTable);
			SnapBookmark bookmark = controller.FindInnermostTemplateBookmarkByPosition(lastRecordEnd);
			System.Diagnostics.Debug.Assert(bookmark != null);
			DocumentModelPosition position = PositionConverter.ToDocumentModelPosition(pieceTable, bookmark.NormalizedStart);
			if (position.RunIndex < field.Result.Start || position.RunIndex > field.Result.End)
				return null;
			return bookmark;
		}
		protected override ResultItemInfoCollection GetResultItems(SnapPieceTable sourcePieceTable, IFieldDataAccessService fieldDataAccessService, FieldDataValueDescriptor fieldDataValueDescriptor, bool modelForExport, Field documentField) {
			int maxRecordCount = modelForExport ? Int32.MaxValue : this.EditorRowLimit;
			if (maxRecordCount <= 0)
				maxRecordCount = Int32.MaxValue;
			ResultItemInfoCollection resultItems = new ResultItemInfoCollection();
			string[] fieldNames = GetFieldsWithParent(sourcePieceTable, documentField);
			ProcessItems(sourcePieceTable.DocumentModel, fieldDataAccessService.FieldContextService, fieldDataValueDescriptor, maxRecordCount, fieldNames, (enumerator) => ProcessItemsCore(sourcePieceTable, fieldDataValueDescriptor, enumerator, resultItems, maxRecordCount));
			return resultItems;
		}
		string[] GetFieldsWithParent(PieceTable pieceTable, Field parent) {
			List<string> result = new List<string>();
			pieceTable.Fields.ForEach(field => {
				if (field.Parent != parent)
					if (field.Parent == null || !field.Parent.DisableUpdate || field.Parent.Parent != parent)
						return;
				if (parent != null && field.FirstRunIndex >= parent.Code.End)
					return;
				SnapFieldCalculatorService calculator = new SnapFieldCalculatorService();
				SNMergeFieldBase snList = calculator.ParseField(pieceTable, field) as SNMergeFieldBase;
				if (snList != null)
					result.Add(snList.DataFieldName);
			}
			);
			return result.ToArray();
		}
		protected virtual void ProcessItemsCore(SnapPieceTable sourcePieceTable, FieldDataValueDescriptor fieldDataValueDescriptor, IDataEnumerator enumerator, ResultItemInfoCollection resultItems, int maxRecordCount) {
			if (enumerator == null)
				return;
			TemplateController templateController = new TemplateController(sourcePieceTable);
			TemplateInfo itemTemplateInfo = templateController.GetTemplateInfo(TemplateInterval, SnapTemplateIntervalType.DataRow);
			TemplateInfo separatorTemplateInfo = templateController.GetTemplateInfo(SeparatorTemplateInterval, SnapTemplateIntervalType.Separator);
			separatorTemplateInfo.SeparateAsPageBreakBefore = ShouldUsePageBreakBefore(separatorTemplateInfo, itemTemplateInfo);
			List<GroupFooterInfo> groupFooters = new List<GroupFooterInfo>();
			Stack<ItemsIntervalInfo> groupIntervalInfos = new Stack<ItemsIntervalInfo>();
			groupIntervalInfos.Push(new ItemsIntervalInfo(null, -1, resultItems.Count, 0));
			int recordCount = 0;
			IFieldContext lastRecordFieldContext = null;
			int groupIndex = -1;
			while (enumerator.MoveNext() && recordCount < maxRecordCount) {
				if (recordCount == 0) {
					IFieldContext listHeaderFieldContext = enumerator.CreateFieldContext();
					resultItems.AddTemplateIfExists(templateController.GetTemplateInfo(ListHeaderTemplateInterval, SnapTemplateIntervalType.ListHeader), listHeaderFieldContext, groupIntervalInfos.Peek());
				}
				List<GroupProperties> groupPropertiesList = enumerator.GetChangedGroupsWithTemplates();
				int count = groupPropertiesList != null ? groupPropertiesList.Count : 0;
				if (recordCount > 0) {
					for (int i = count - 1; i >= 0; i--) {
						if (groupPropertiesList[i].HasGroupTemplates) {
							ItemsIntervalInfo groupIntervalInfo = groupIntervalInfos.Pop();
							AddClosedGroupFootersAndSeparators(resultItems, templateController, groupFooters, groupIntervalInfo, groupPropertiesList[i]);
							groupIntervalInfo.EndGroupIndex = resultItems.Count - 1;
						}
					}
				}
				for (int i = 0; i < count; i++) {
					groupIndex = GetGroupIndex(groupPropertiesList[i], fieldDataValueDescriptor.RelativePath.Items);
					if (groupPropertiesList[i].HasGroupTemplates)
						groupIntervalInfos.Push(new ItemsIntervalInfo(groupPropertiesList[i], groupIndex, resultItems.Count, groupPropertiesList[i].GroupFieldInfos.Count));
					if (groupPropertiesList[i].HasTemplateHeader) {
						TemplateInfo headerTemplateInfo = GetTemplateInfoBySwitch(templateController, groupPropertiesList[i].TemplateHeaderSwitch, SnapTemplateIntervalType.GroupHeader);
						IFieldContext headerFieldContext = enumerator.CreateFieldContext();
						resultItems.AddTemplateIfExists(headerTemplateInfo, headerFieldContext, groupIntervalInfos.Peek());
					}
					if (groupPropertiesList[i].HasTemplateFooter)
						groupFooters.Add(new GroupFooterInfo(groupPropertiesList[i], enumerator.CreateFieldContext()));
					if (groupPropertiesList[i].HasTemplateSeparator)
						groupFooters.Add(new GroupFooterInfo(groupPropertiesList[i], enumerator.CreateFieldContext()));
				}
				IFieldContext relatedFieldContext = enumerator.CreateFieldContext();
				resultItems.AddTemplateIfExists(itemTemplateInfo, relatedFieldContext, groupIntervalInfos.Peek());
				resultItems.AddSeparatorIfExists(separatorTemplateInfo, relatedFieldContext, groupIntervalInfos.Peek());
				lastRecordFieldContext = relatedFieldContext;
				count = groupFooters.Count;
				for (int i = 0; i < count; i++)
					groupFooters[i].FieldContext = enumerator.CreateFieldContext();
				recordCount++;
				if (progressIndicatorService != null)
					progressIndicatorService.SetProgress(this.progressIndicatorToken, recordCount);
			}
			if (!KeepLastSeparator)
				resultItems.RemoveLastSeparatorIfExists();
			else if (separatorTemplateInfo.SeparateAsPageBreakBefore) {
				resultItems.RemoveLastSeparatorIfExists();
				resultItems.AddTemplateIfExists(separatorTemplateInfo, lastRecordFieldContext);
			}
			for (int i = groupFooters.Count - 1; i >= 0; i--) {
				GroupProperties properties = groupFooters[i].Properties;
				if (!properties.HasTemplateFooter)
					continue;
				if (i > 0 && properties.HasTemplateSeparator && properties == groupFooters[i - 1].Properties)
					continue;
				TemplateInfo footerTemplateInfo = GetTemplateInfoBySwitch(templateController, properties.TemplateFooterSwitch, SnapTemplateIntervalType.GroupFooter);
				IFieldContext footerFieldContext = groupFooters[i].FieldContext;
				ItemsIntervalInfo groupIntervalInfo = GetGroupIntervalInfo(groupIntervalInfos, properties, resultItems.Count);
				resultItems.AddTemplateIfExists(footerTemplateInfo, footerFieldContext, groupIntervalInfo);
			}
			if (recordCount == 0) {
				IDataControllerListFieldContext listContext = enumerator.GetListFieldContext();
				AddProxyResult(resultItems, templateController.GetTemplateInfo(ListHeaderTemplateInterval, SnapTemplateIntervalType.ListHeader), fieldDataValueDescriptor, sourcePieceTable.DocumentModel, groupIntervalInfos.Peek(), listContext);
				if (!sourcePieceTable.DocumentModel.ModelForExport) {
					AddProxyResult(resultItems, itemTemplateInfo, fieldDataValueDescriptor, sourcePieceTable.DocumentModel, groupIntervalInfos.Peek(), listContext);
					AddProxyResult(resultItems, separatorTemplateInfo, fieldDataValueDescriptor, sourcePieceTable.DocumentModel, groupIntervalInfos.Peek(), listContext);
				}
				AddProxyResult(resultItems, templateController.GetTemplateInfo(ListFooterTemplateInterval, SnapTemplateIntervalType.ListFooter), fieldDataValueDescriptor, sourcePieceTable.DocumentModel, groupIntervalInfos.Peek(), listContext);
			}
			else
				resultItems.AddTemplateIfExists(templateController.GetTemplateInfo(ListFooterTemplateInterval, SnapTemplateIntervalType.ListFooter), lastRecordFieldContext, groupIntervalInfos.Peek());
			if (resultItems.Count == 0)
				return;
			while (groupIntervalInfos.Count > 0) {
				ItemsIntervalInfo info = groupIntervalInfos.Pop();
				info.EndGroupIndex = resultItems.Count - 1;
			}
		}
		bool ShouldUsePageBreakBefore(TemplateInfo separatorTemplateInfo, TemplateInfo itemTemplateInfo) {
			if (!IsPageBreakSeparator(separatorTemplateInfo))
				return false;
			if (itemTemplateInfo.TemplateContentType == TemplateContentType.NoTemplate)
				return false;
			RunInfo runInfo = itemTemplateInfo.PieceTable.FindRunInfo(itemTemplateInfo.ActualInterval.Start, itemTemplateInfo.ActualInterval.Length);
			ParagraphRun lastRun = itemTemplateInfo.PieceTable.Runs[runInfo.End.RunIndex] as ParagraphRun;
			return lastRun != null;
		}
		bool IsPageBreakSeparator(TemplateInfo separatorTemplateInfo) {
			if (separatorTemplateInfo.TemplateContentType == TemplateContentType.NoTemplate)
				return false;
			if (separatorTemplateInfo.ActualInterval.Length != 1)
				return false;
			RunInfo runInfo = separatorTemplateInfo.PieceTable.FindRunInfo(separatorTemplateInfo.ActualInterval.Start, separatorTemplateInfo.ActualInterval.Length);
			if (runInfo == null)
				return false;
			TextRunBase firstRun = separatorTemplateInfo.PieceTable.Runs[runInfo.Start.RunIndex];
			if (firstRun.GetType() != typeof(TextRun) || runInfo.Start.RunOffset >= firstRun.Length)
				return false;
			string text = firstRun.GetTextFast(separatorTemplateInfo.PieceTable.TextBuffer);
			return text[runInfo.Start.RunOffset] == Characters.PageBreak;
		}
		int GetGroupIndex(GroupProperties groupProperties, List<FieldDataMemberInfoItem> list) {
			int count = list.Count;
			if (count < 1)
				return -1;
			if (!list[0].HasGroups)
				return -1;
			int groupPropertiesIndex = list[0].Groups.IndexOf(groupProperties);
			if (groupPropertiesIndex < 0)
				return -1;
			int result = 0;
			for (int i = 0; i < groupPropertiesIndex; i++) {
				result += list[0].Groups[i].GroupFieldInfos.Count;
			}
			return result;
		}
		ItemsIntervalInfo GetGroupIntervalInfo(Stack<ItemsIntervalInfo> groupIntervalInfos, GroupProperties groupProperties, int endGroupIndex) {
			while (true) {
				ItemsIntervalInfo result = groupIntervalInfos.Pop();
				result.EndGroupIndex = endGroupIndex;
				if (result.GroupProperties == groupProperties)
					return result;
			}
		}
		void AddClosedGroupFootersAndSeparators(ResultItemInfoCollection resultItems, TemplateController templateController, List<GroupFooterInfo>groupFooters, ItemsIntervalInfo groupIntervalInfo, GroupProperties groupProperties) {			 
			int openGroupPropertiesIndex = groupFooters.Count - 1;
			if (openGroupPropertiesIndex < 0 || groupFooters[openGroupPropertiesIndex].Properties != groupProperties)
				return;
			Debug.Assert(groupProperties.HasTemplateFooter || groupProperties.HasTemplateSeparator);
			if (groupProperties.HasTemplateFooter)
				AddClosedTemplate(openGroupPropertiesIndex, resultItems, templateController, groupProperties.TemplateFooterSwitch, SnapTemplateIntervalType.GroupFooter, groupFooters, groupIntervalInfo);
			openGroupPropertiesIndex = groupFooters.Count - 1;
			if (openGroupPropertiesIndex < 0 || groupFooters[openGroupPropertiesIndex].Properties != groupProperties)
				return;
			Debug.Assert(groupProperties.HasTemplateSeparator);
			if (groupProperties.HasTemplateSeparator)
				AddClosedTemplate(openGroupPropertiesIndex, resultItems, templateController, groupProperties.TemplateSeparatorSwitch, SnapTemplateIntervalType.GroupSeparator, groupFooters, groupIntervalInfo);
		}
		void AddClosedTemplate(int index, ResultItemInfoCollection resultItems, TemplateController templateController, string templateSwitch, SnapTemplateIntervalType templateType, List<GroupFooterInfo> groupFooters, ItemsIntervalInfo groupIntervalInfo) {
			TemplateInfo templateInfo = GetTemplateInfoBySwitch(templateController, templateSwitch, templateType);
			IFieldContext fieldContext = groupFooters[index].FieldContext;
			if(templateType != SnapTemplateIntervalType.GroupSeparator)
				resultItems.AddTemplateIfExists(templateInfo, fieldContext, groupIntervalInfo);
			else
				resultItems.AddSeparatorIfExists(templateInfo, fieldContext, groupIntervalInfo);
			groupFooters.RemoveAt(index);
		}
		void AddProxyResult(ResultItemInfoCollection resultItems, TemplateInfo itemTemplateInfo, FieldDataValueDescriptor fieldDataValueDescriptor, IServiceProvider serviceProvider, ItemsIntervalInfo intervalInfo, IDataControllerListFieldContext listContext) {
			IFieldContext parentDataContext = fieldDataValueDescriptor.ParentDataContext;
			FieldPathDataMemberInfo relativePath = fieldDataValueDescriptor.RelativePath;
			if (parentDataContext != null || relativePath != null) {
				string fieldPath = String.Empty;
				if(relativePath != null && relativePath.LastItem != null)
					fieldPath = relativePath.LastItem.FieldName;
				resultItems.AddTemplateIfExists(itemTemplateInfo, new ProxyFieldContext(fieldPath, listContext), intervalInfo);
			}
			else
				resultItems.AddTemplateIfExists(itemTemplateInfo, new EmptyFieldContext(), intervalInfo);
		}
		#region IDataFieldNameOwner Members
		string IDataFieldNameOwner.DataFieldName {
			get { return DataSourceName; }
		}
		#endregion
		public override void OnRemoveTemplateSwitches(PieceTable pieceTable, InstructionController instructionController, List<string> templateSwitches) {
			base.OnRemoveTemplateSwitches(pieceTable, instructionController, templateSwitches);
			if (templateSwitches.Count == 0)
				return;
			FieldPathInfo fieldPathInfo = ProcessGroups(pieceTable, group => OnRemoveTemplateSwitch(group, templateSwitches));
			if (fieldPathInfo != null) {
				IFieldDataAccessService dataAccessService = pieceTable.DocumentModel.GetService<IFieldDataAccessService>();
				if (dataAccessService == null)
					return;
				IFieldPathService fieldPathService = dataAccessService.FieldPathService;
				instructionController.SetArgument(0, fieldPathService.GetStringPath(fieldPathInfo));
			}
		}
		protected virtual bool OnRemoveTemplateSwitch(GroupProperties group, List<string> templateSwitches) {
			if (!group.HasGroupTemplates)
				return false;
			bool templateRemoved = false;
			if (templateSwitches.Contains(group.TemplateHeaderSwitch)) {
				group.TemplateHeaderSwitch = String.Empty;
				templateRemoved = true;
			}
			if (templateSwitches.Contains(group.TemplateFooterSwitch)) {
				group.TemplateFooterSwitch = String.Empty;
				templateRemoved = true;
			}
			return templateRemoved;
		}
		protected virtual FieldPathInfo ProcessGroups(PieceTable pieceTable, Func<GroupProperties, bool> groupAction) {
			bool processed = false;
			IFieldDataAccessService dataAccessService = pieceTable.DocumentModel.GetService<IFieldDataAccessService>();
			if (dataAccessService == null)
				return null;
			IFieldPathService fieldPathService = dataAccessService.FieldPathService;
			FieldPathInfo fieldPathInfo = fieldPathService.FromString(DataSourceName);
			List<FieldDataMemberInfoItem> items = fieldPathInfo.DataMemberInfo.Items;
			int count = items.Count;
			for (int i = 0; i < count; i++) {
				FieldDataMemberInfoItem item = items[i];
				if (!item.HasGroups)
					continue;
				List<GroupProperties> groups = item.Groups;
				int groupCount = groups.Count;
				for (int j = 0; j < groupCount; j++) {
					GroupProperties group = groups[j];
					processed |= groupAction(group);
				}
			}
			return processed ? fieldPathInfo : null;
		}
		public override List<TemplateFieldInterval> GetTemplateIntervals(PieceTable pieceTable) {
			List<TemplateFieldInterval> result = new List<TemplateFieldInterval>();
			AddTemplateIntervalIfExists(result, TemplateSwitch);
			ProcessGroups(pieceTable, group => AppendGroupTemplates(result, group));
			AddTemplateIntervalIfExists(result, ListHeaderTemplateSwitch);
			AddTemplateIntervalIfExists(result, ListFooterTemplateSwitch);
			AddTemplateIntervalIfExists(result, SeparatorTemplateSwitch);
			return result;
		}
		bool AppendGroupTemplates(List<TemplateFieldInterval> target, GroupProperties group) {
			AddTemplateIntervalIfExists(target, group.TemplateFooterSwitch);
			AddTemplateIntervalIfExists(target, group.TemplateHeaderSwitch);
			AddTemplateIntervalIfExists(target, group.TemplateSeparatorSwitch);
			return true;
		}
		public override bool IsSwitchWithArgument(string fieldSpecificSwitch) {
			string switchName = fieldSpecificSwitch.ToLower();
			return switchName != KeepLastSeparatorSwitchWithPrefix;
		}
		public override bool IsSwitchArgumentField(string fieldSpecificSwitch) {
			string switchName = fieldSpecificSwitch.ToLower();
			return switchName != NameSwitchWithPrefix && switchName != EditorRowLimitSwitchWithPrefix;
		}
	}
	public class GroupFooterInfo {
		public GroupProperties Properties { get; set; }
		public IFieldContext FieldContext { get; set; }
		public GroupFooterInfo(GroupProperties properties, IFieldContext fieldContext) {
			Properties = properties;
			FieldContext = fieldContext;
		}
	}
	public class ItemsIntervalInfo {
		int endGroupIndex;
		public ItemsIntervalInfo(GroupProperties groupProperties, int groupPropertiesIndex, int startGroupIndex, int fieldInGroupCount) {
			StartGroupIndex = startGroupIndex;
			GroupProperties = groupProperties;
			GroupPropertiesIndex = groupPropertiesIndex;
			FieldInGroupCount = fieldInGroupCount;
		}
		public int StartGroupIndex { get; private set; }
		public int GroupPropertiesIndex { get; private set; }
		public int FieldInGroupCount { get; private set; }
		public GroupProperties GroupProperties { get; private set; }
		public int EndGroupIndex {
			get { return endGroupIndex; }
			set {
				if (value < StartGroupIndex)
					Exceptions.ThrowInternalException();
				endGroupIndex = value; 
			}
		}
	}
	public class ResultItemInfo {
		public ResultItemInfo(TemplateInfo templateInfo, IFieldContext dataContext, ItemsIntervalInfo intervalInfo, bool pageBreakBefore) {
			TemplateInfo = templateInfo;
			DataContext = dataContext;
			IntervalInfo = intervalInfo;
			PageBreakBefore = pageBreakBefore;
		}
		public TemplateInfo TemplateInfo { get; private set; }
		public IFieldContext DataContext { get; private set; }
		public bool PageBreakBefore { get; private set; }
		public ItemsIntervalInfo IntervalInfo { get; set; }		
	}
	public class ResultItemInfoCollection  {
		bool pageBreakBeforeNextTemplate;
		bool separatorLastItem;
		List<ResultItemInfo> innerList;
		public ResultItemInfoCollection() {
			this.innerList = new List<ResultItemInfo>();
		}
		public int Count { get { return innerList.Count; } }
		public ResultItemInfo this[int index] { get { return innerList[index]; } }
		public void AddTemplateIfExists(TemplateInfo templateInfo, IFieldContext dataContext) {
			AddTemplateIfExists(templateInfo, dataContext, null);
		}
		public bool AddTemplateIfExists(TemplateInfo templateInfo, IFieldContext dataContext, ItemsIntervalInfo intervalInfo) {
			if (templateInfo.TemplateContentType == TemplateContentType.NoTemplate)
				return false;
			innerList.Add(new ResultItemInfo(templateInfo, dataContext, intervalInfo, pageBreakBeforeNextTemplate));
			separatorLastItem = false;
			pageBreakBeforeNextTemplate = false;
			return true;
		}
		public void AddSeparatorIfExists(TemplateInfo separatorTemplateInfo, IFieldContext relatedFieldContext, ItemsIntervalInfo intervalInfo) {
			if (!separatorTemplateInfo.SeparateAsPageBreakBefore) {
				if(AddTemplateIfExists(separatorTemplateInfo, relatedFieldContext, intervalInfo))
					separatorLastItem = true;				
			}
			else
				pageBreakBeforeNextTemplate = true;
		}
		public void RemoveLastSeparatorIfExists() {
			if (pageBreakBeforeNextTemplate)
				pageBreakBeforeNextTemplate = false;
			else
				if (separatorLastItem)
					innerList.RemoveAt(Count - 1);				
		}
	}
	[Flags]
	public enum GroupBookmarkKind { GroupHeader = 1, GroupFooter = 2, GroupHeaderOrGroupFooter = 3 }
}
