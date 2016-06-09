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
using System.ComponentModel;
using System.Collections.Generic;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Data.Browsing;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.Snap.Core.Native.Data;
using DevExpress.Snap.Core.Fields;
using DevExpress.Snap.Core.API;
using DevExpress.XtraReports.Native.Parameters;
using DevExpress.Snap.Core.Options;
using FieldPathService = DevExpress.Snap.Core.Native.Data.Implementations.FieldPathService;
namespace DevExpress.Snap.Core.Native.Templates {
	public abstract class InsertAction {
		readonly InsertAction parent;
		protected InsertAction(InsertAction parent) {
			this.parent = parent;
		}
		public InsertAction Parent { get { return parent; } }
		public override bool Equals(object obj) {
			InsertAction other = obj as InsertAction;
			if (Object.ReferenceEquals(other, null))
				return false;
			if (Object.ReferenceEquals(other.Parent, Parent))
				return true;
			if (Object.ReferenceEquals(Parent, null))
				return false;
			return Parent.Equals(other.Parent);
		}
		public override int GetHashCode() {
			if (parent == null)
				return 0;
			return parent.GetHashCode() + 1;
		}
		public abstract void WriteStartText(TemplateWriter templateWriter);
		public abstract void WriteEndText(TemplateWriter templateWriter);
		public virtual void BeforeWriteInnerAction(InsertAction innerAction, TemplateWriter templateWriter) {
		}
		public virtual void AfterWriteInnerAction(InsertAction innerAction, TemplateWriter templateWriter) {
		}
	}
	public class InsertActionTreeNode {
		readonly InsertAction action;
		readonly List<InsertActionTreeNode> childNodes;
		readonly InsertActionTreeNode parentNode;
		public InsertActionTreeNode(InsertAction action, InsertActionTreeNode parentNode) {
			this.action = action;
			this.childNodes = new List<InsertActionTreeNode>();
			this.parentNode = parentNode;
		}
		public InsertAction Action { get { return action; } }
		public List<InsertActionTreeNode> ChildNodes { get { return childNodes; } }
		public InsertActionTreeNode ParentNode { get { return parentNode; } }
	}
	public class InsertActionTreeBuilder {
		Dictionary<InsertAction, InsertActionTreeNode> mapActionToNode;
		public InsertActionTree Build(InsertAction rootAction, List<InsertAction> leafActions) {
			mapActionToNode = new Dictionary<InsertAction, InsertActionTreeNode>();
			foreach (InsertAction action in leafActions)
				GetNode(action);
			return new InsertActionTree(GetNode(rootAction));
		}
		InsertActionTreeNode GetNode(InsertAction action) {
			InsertActionTreeNode result;
			if (!mapActionToNode.TryGetValue(action, out result)) {
				InsertActionTreeNode parentNode = action.Parent != null ? GetNode(action.Parent) : null;
				result = new InsertActionTreeNode(action, parentNode);
				mapActionToNode.Add(action, result);
				if (parentNode != null)
					parentNode.ChildNodes.Add(result);
			}
			return result;
		}
	}
	public class InsertActionTree {
		readonly InsertActionTreeNode rootNode;
		public InsertActionTree(InsertActionTreeNode rootNode) {
			this.rootNode = rootNode;
		}
		public InsertActionTreeNode RootNode { get { return rootNode; } }
		public bool IsEmpty { get { return rootNode == null || rootNode.ChildNodes.Count == 0; } }
	}
	public class RootInsertAction : InsertAction {
		readonly List<InsertAction> leafActions;
		InsertActionTree insertActionTree;
		public RootInsertAction()
			: base(null) {
			this.leafActions = new List<InsertAction>();
		}
		private List<InsertAction> LeafActions { get { return leafActions; } }
		public InsertActionTree GetInsertActionTree() {
			if (insertActionTree == null) {
				InsertActionTreeBuilder builder = new InsertActionTreeBuilder();
				insertActionTree = builder.Build(this, LeafActions);
			}
			return insertActionTree;
		}
		public void AddLeafAction(InsertAction action) {
			leafActions.Add(action);
			insertActionTree = null;
		}
		public override bool Equals(object obj) {
			return Object.ReferenceEquals(this, obj);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override void WriteStartText(TemplateWriter templateWriter) {
		}
		public override void WriteEndText(TemplateWriter templateWriter) {
		}
	}
	public class InsertSNTextFieldAction : InsertAction {
		readonly string fieldName;
		public InsertSNTextFieldAction(string fieldName, InsertAction parent)
			: base(parent) {
			this.fieldName = fieldName;
		}
		public string FieldName { get { return fieldName; } }
		protected virtual string FieldText { get { return SNTextField.FieldType; } }
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			InsertSNTextFieldAction other = obj as InsertSNTextFieldAction;
			if (Object.ReferenceEquals(other, null))
				return false;
			return String.Compare(FieldName, other.FieldName, true) == 0;
		}
		public override int GetHashCode() {
			return fieldName.GetHashCode();
		}
		public override void WriteStartText(TemplateWriter templateWriter) {
			templateWriter.BeginField();
			templateWriter.WriteText(String.Format("{0} {1} \\{2} {3} \\{4}", FieldText, InstructionController.GetEscapedFieldName(fieldName), SNTextField.EmptyFieldDataAliasSwitch, SNTextField.AliasForEmptyFieldData, SNTextField.EnableEmptyFieldDataAliasSwitch));
		}
		public override void WriteEndText(TemplateWriter templateWriter) {
			templateWriter.EndField();
		}
	}
	public class InsertParameterAsSNTextFieldAction : InsertSNTextFieldAction {
		public InsertParameterAsSNTextFieldAction(string fieldName, InsertAction parent)
			: base(fieldName, parent) {
		}
		public override void WriteStartText(TemplateWriter templateWriter) {
			base.WriteStartText(templateWriter);
			templateWriter.WriteText(" \\" + SNTextField.ParameterSwitch);
		}
	}
	public class InsertSNImageFieldAction : InsertSNTextFieldAction {
		public InsertSNImageFieldAction(string fieldName, InsertAction parent)
			: base(fieldName, parent) {
		}
		protected override string FieldText { get { return SNImageField.FieldType; } }
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			return obj is InsertSNImageFieldAction;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
	public class InsertSNSparklineFieldAction : InsertAction {
		string dataSourceName;
		readonly string fieldName;
		Dictionary<string, string> instructions;
		public InsertSNSparklineFieldAction(string fieldName, string dataSourceName, InsertAction parent, Dictionary<string, string> instructions)
			: base(parent) {
			this.fieldName = fieldName;
			this.dataSourceName = dataSourceName;
			this.instructions = instructions;
		}
		public string FieldName { get { return fieldName; } }
		public string DataSourceName { get { return dataSourceName; } }
		public Dictionary<string, string> Instructions { get { return instructions; } }
		protected virtual string FieldText { get { return SNSparklineField.FieldType; } }
		public override void WriteStartText(TemplateWriter templateWriter) {
			templateWriter.BeginField();
			string result = String.Format("{0} {1} \\{2} {3} \\{4}", FieldText, InstructionController.GetEscapedArgument(FieldName), SNSparklineField.EmptyFieldDataAliasSwitch, SNSparklineField.AliasForEmptyFieldData, SNSparklineField.EnableEmptyFieldDataAliasSwitch);
			result = ApplyCurrentSparklineSettings(result);
			templateWriter.WriteText(result);
		}
		public override void WriteEndText(TemplateWriter templateWriter) {
			templateWriter.EndField();
		}
		string ApplyCurrentSparklineSettings(string startText) {
			if (!string.IsNullOrEmpty(dataSourceName))
				startText += " \\" + SNSparklineField.SnSparklineDataSourceNameSwitch + " " + GetQuotes(dataSourceName) + dataSourceName + GetQuotes(dataSourceName);
			if (Instructions == null)
				return startText;
			foreach (var item in Instructions) {
				startText += " \\" + item.Key + " " + item.Value;
			}
			return startText;
		}
		string GetQuotes(string dataSourceName) {
			if (string.IsNullOrEmpty(dataSourceName))
				return string.Empty;
			bool needQuotes = dataSourceName.Contains(" ");
			return (needQuotes ? "\"" : string.Empty);
		}
		public override int GetHashCode() {
			return FieldName.GetHashCode();
		}
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			InsertSNSparklineFieldAction other = obj as InsertSNSparklineFieldAction;
			if (Object.ReferenceEquals(other, null))
				return false;
			bool isFieldNamesEquals = String.Compare(FieldName, other.FieldName, true) == 0;
			bool isDataSourceNamesEquals = String.Compare(DataSourceName, other.DataSourceName, true) == 0;
			return isFieldNamesEquals && isDataSourceNamesEquals;
		}
	}
	public class InsertSNCheckBoxAction : InsertSNTextFieldAction {
		public InsertSNCheckBoxAction(string fieldName, InsertAction parent)
			: base(fieldName, parent) {
		}
		protected override string FieldText { get { return SNCheckBoxField.FieldType; } }
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			return obj is SNCheckBoxField;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
	public class InsertSNHyperlinkFieldAction : InsertSNTextFieldAction {
		public InsertSNHyperlinkFieldAction(string fieldName, InsertAction parent)
			: base(fieldName, parent) {
		}
		protected override string FieldText { get { return SNHyperlinkField.FieldType; } }
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			return obj is InsertSNHyperlinkFieldAction;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
	public abstract class InsertObjectActionBase : InsertAction {
		public static InsertObjectActionBase Create(Type dataSourceType, string dataSourceName, string dataMember, InsertAction parent, string listName) {
			if (ListTypeHelper.IsListType(dataSourceType))
				if (dataMember != null)
					return new InsertSNListAction(dataSourceName, dataMember, parent, listName);
				else
					return null;
			return new InsertDisplayObjectAction(dataSourceName, dataMember, parent);
		}
		readonly string dataSourceName;
		readonly string dataMember;
		protected InsertObjectActionBase(string dataSourceName, string dataMember, InsertAction parent)
			: base(parent) {
			this.dataSourceName = dataSourceName;
			this.dataMember = dataMember;
		}
		public string DataMember { get { return dataMember; } }
		public string DataSourceName { get { return dataSourceName; } }
		protected abstract string FieldName { get; }
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			InsertObjectActionBase other = obj as InsertObjectActionBase;
			if (Object.ReferenceEquals(other, null))
				return false;
			return String.Compare(DataSourceName, other.DataSourceName, true) == 0 && String.Compare(DataMember, other.DataMember, true) == 0;
		}
		public override int GetHashCode() {
			if (string.IsNullOrEmpty(DataMember))
				return 0;
			if (!string.IsNullOrEmpty(DataSourceName))
				return DataMember.GetHashCode() ^ DataSourceName.GetHashCode();
			return DataMember.GetHashCode();
		}
		public override void WriteStartText(TemplateWriter templateWriter) {
			templateWriter.BeginField();
			string dataReferenceString = GetDataReferenceString();
			templateWriter.WriteText(String.Format(@"{0} {1} \t ", FieldName, InstructionController.GetEscapedArgument(dataReferenceString)));
			templateWriter.BeginTemplate();
		}
		public override void WriteEndText(TemplateWriter templateWriter) {
			templateWriter.EndTemplate();
			BeforeEndField(templateWriter);
			templateWriter.EndField();
		}
		protected virtual void BeforeEndField(TemplateWriter templateWriter) {
		}
		public string GetDataReferenceString() {
			string result = GetDataReferenceStringCore();
			if (String.IsNullOrEmpty(result))
				return result;
			int index = result.IndexOf('[');
			if (index > 0)
				result = result.Substring(0, index);
			else
				if (index == 0)
					result = String.Empty;
			return result;
		}
		protected virtual string GetDataReferenceStringCore() {
			if (String.IsNullOrEmpty(DataSourceName))
				return DataMember;
			if (!String.IsNullOrEmpty(DataMember))
				return String.Format("/{0}.{1}", DataSourceName, DataMember);
			return String.Format("/{0}", DataSourceName);
		}
		public override void BeforeWriteInnerAction(InsertAction innerAction, TemplateWriter templateWriter) {
			templateWriter.WriteParagraphStart();
		}
	}
	public class InsertDisplayObjectAction : InsertObjectActionBase {
		public InsertDisplayObjectAction(string dataSourceName, string dataMember, InsertAction parent)
			: base(dataSourceName, dataMember, parent) {
		}
		protected override string FieldName { get { return "DisplayObject"; } }
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			return obj is InsertDisplayObjectAction;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
	public class InsertSNListAction : InsertObjectActionBase {
		string listName;
		public InsertSNListAction(string dataSourceName, string dataMember, InsertAction parent, string listName)
			: base(dataSourceName, dataMember, parent) {
			this.listName = listName;
		}
		protected override string FieldName { get { return SNListField.FieldType; } }
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			return obj is InsertSNListAction;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override void WriteStartText(TemplateWriter templateWriter) {
			base.WriteStartText(templateWriter);
			templateWriter.WriteParagraphStart();
			templateWriter.BeginTable();
			templateWriter.BeginTableRow();
		}
		protected override void BeforeEndField(TemplateWriter templateWriter) {
			templateWriter.WriteText(String.Format("{0} {1}", SNListField.NameSwitchWithPrefix, InstructionController.GetEscapedArgument(listName)));
		}
		public override void WriteEndText(TemplateWriter templateWriter) {
			templateWriter.EndTableRow(true);
			templateWriter.EndTable(SnapDocumentModel.DefaultListStyleName, GetListLevel(), true);
			base.WriteEndText(templateWriter);
		}
		int GetListLevel() {
			int result = 1;
			InsertAction action = Parent;
			while (action != null) {
				if (action is InsertSNListAction)
					result++;
				action = action.Parent;
			}
			return result;
		}
		public override void BeforeWriteInnerAction(InsertAction innerAction, TemplateWriter templateWriter) {
			bool changeStyleProperties = false;
			if (innerAction is InsertSNListAction) {
				templateWriter.EndTableRow(true);
				templateWriter.BeginTableRow();
				changeStyleProperties = true;
			}
			templateWriter.BeginTableCell(changeStyleProperties);
		}
		public override void AfterWriteInnerAction(InsertAction innerAction, TemplateWriter templateWriter) {
			base.AfterWriteInnerAction(innerAction, templateWriter);
			templateWriter.WriteParagraphStart();
			templateWriter.EndTableCell();
			if (innerAction is InsertSNListAction) {
				templateWriter.EndTableRow(true);
				templateWriter.BeginTableRow();
			}
		}
	}
	public class InsertRelationAction : InsertAction {
		readonly InsertAction innerAction;
		public InsertRelationAction(InsertAction innerAction)
			: base(innerAction.Parent) {
			this.innerAction = innerAction;
		}
		internal InsertAction InnerAction { get { return innerAction; } }
		public override void WriteStartText(TemplateWriter templateWriter) {
			InnerAction.WriteStartText(templateWriter);
		}
		public override void WriteEndText(TemplateWriter templateWriter) {
			InnerAction.WriteEndText(templateWriter);
		}
		public override void BeforeWriteInnerAction(InsertAction innerAction, TemplateWriter templateWriter) {
			InnerAction.BeforeWriteInnerAction(innerAction, templateWriter);
		}
		public override void AfterWriteInnerAction(InsertAction innerAction, TemplateWriter templateWriter) {
			InnerAction.AfterWriteInnerAction(innerAction, templateWriter);
		}
		public override bool Equals(object obj) {
			return InnerAction.Equals(obj);
		}
		public override int GetHashCode() {
			return InnerAction.GetHashCode();
		}
	}
	public class SparklineTemplateBuilder : TemplateBuilder {
		enum ContextType {
			None,
			Without, 
			Within 
		}
		enum ContextLevel {
			None, 
			Correct, 
			SameLevel, 
			Deeper 
		}
		enum DataSourceType {
			None,
			Default,
			Named
		}
		Dictionary<string, string> instructions;
		public SparklineTemplateBuilder(Dictionary<string, string> instructions) {
			this.instructions = instructions;
			contextLevel = ContextLevel.None;
			contextType = ContextType.None;
			dataSourceType = DataSourceType.None;
		}
		ContextLevel contextLevel;
		ContextType contextType;
		DataSourceType dataSourceType;
		bool isLastContextLevel = true;
		InsertAction sparklineParentAction = null;
		string columnName = string.Empty;
		DataMemberInfo baseDataMemberInfo = null;
		bool calculateRelationAction = false;
		SNDataInfo currentDataInfo = null;
		string customDataSourceName = string.Empty;
		protected override InsertAction GetInsertAction(SnapDocumentModel documentModel, List<string> listNames, string listName, DataContext dataContext, DataBrowser dataBrowser, object dataSource, DataMemberInfo dataMemberInfo, InsertAction parentAction, bool leafAction, List<string> insertPositionDataSources) {
			IDataSourceDispatcher dataSourceDispatcher = documentModel.DataSourceDispatcher;
			if (dataSourceType == DataSourceType.None)
				dataSourceType = object.ReferenceEquals(dataSourceDispatcher.DefaultDataSource, dataSource) ? DataSourceType.Default : DataSourceType.Named;
			if (dataSourceType == DataSourceType.Named && string.IsNullOrEmpty(customDataSourceName))
				customDataSourceName = dataSourceDispatcher.FindDataSourceName(dataSource);
			if (contextType == ContextType.None)
				contextType = insertPositionDataSources.Count != 0 ? ContextType.Within : ContextType.Without;
			if (contextLevel == ContextLevel.None) {
				string[] dataMemberParts = dataMemberInfo.DataMember.Split('.');
				if (dataMemberParts.Length != 0) {
					if (contextType == ContextType.Without) {
						int rightLevel = dataSourceType == DataSourceType.Default ? 1 : 2;
						contextLevel = dataMemberParts.Length == rightLevel ? ContextLevel.Correct : ContextLevel.Deeper;
					}
					else if (contextType == ContextType.Within) {
						if (dataMemberParts.Length >= 2) {
							int rightLevel = dataSourceType == DataSourceType.Default ? 2 : 3;
							contextLevel = dataMemberParts.Length == rightLevel ? ContextLevel.Correct : ContextLevel.Deeper;
						}
						else
							contextLevel = ContextLevel.SameLevel;
					}
				}
			}
			isLastContextLevel = contextType != ContextType.None && contextLevel == ContextLevel.Deeper && string.Equals(dataMemberInfo.ColumnName, dataMemberInfo.DataMember);
			return base.GetInsertAction(documentModel, listNames, listName, dataContext, dataBrowser, dataSource, dataMemberInfo, parentAction, leafAction, insertPositionDataSources);
		}
		protected override string GetColumnName(DataMemberInfo dataMemberInfo) {
			if (calculateRelationAction || contextLevel == ContextLevel.None)
				return base.GetColumnName(dataMemberInfo);
			return dataMemberInfo.ParentDataMemberInfo.ColumnName;
		}
		protected override bool ShouldCreateInsertAction(SnapDocumentModel documentModel, object dataSource, string parentDataMember) {
			if (contextLevel == ContextLevel.None)
				return false;
			bool shouldCreateParentAction = base.ShouldCreateInsertAction(documentModel, dataSource, parentDataMember);
			return shouldCreateParentAction && contextLevel == ContextLevel.Deeper && !isLastContextLevel;
		}
		protected override InsertAction CreateInsertAction(DataBrowser dataBrowser, DataMemberInfo dataMemberInfo, InsertAction parentAction) {
			if (calculateRelationAction || contextLevel == ContextLevel.None) {
				bool isRightContext = IsRelatedDataInfoInRightContext(dataMemberInfo.DataMember.Split('.'), baseDataMemberInfo.DataMember.Split('.'));
				InsertAction correctParentAction = sparklineParentAction != null && isRightContext ? sparklineParentAction : parentAction;
				if (!isRightContext && contextType == ContextType.Without && sparklineParentAction != null) {
					var rootActionForSparklineWithoutContext = RootActionForSparklineWithoutContext;
					correctParentAction = rootActionForSparklineWithoutContext == null ? parentAction : rootActionForSparklineWithoutContext;
				}
				return base.CreateInsertAction(dataBrowser, dataMemberInfo, correctParentAction);
			}
			string[] dataMembers = dataMemberInfo.ParentDataMemberInfo.DataMember.Split('.');
			string dataSourceName = dataMembers.Length != 0 ? dataMembers[dataMembers.Length - 1] : string.Empty;
			if (parentAction is RootInsertAction && dataSourceType == DataSourceType.Named && !string.IsNullOrEmpty(customDataSourceName) && dataMembers.Length <= 1) {
				if (!string.IsNullOrEmpty(dataSourceName))
					dataSourceName = string.Format(".{0}", dataSourceName);
				dataSourceName = string.Format("/{0}{1}", customDataSourceName, dataSourceName);
			}
			sparklineParentAction = parentAction;
			baseDataMemberInfo = dataMemberInfo;
			columnName = dataMemberInfo.ColumnName;
			return new InsertSNSparklineFieldAction(dataMemberInfo.ColumnName, dataSourceName, parentAction, instructions);
		}
		InsertAction RootActionForSparklineWithoutContext {
			get {
				if (contextType != ContextType.Without || sparklineParentAction == null)
					return null;
				InsertAction rootOutsideAction = sparklineParentAction.Parent == null ? null : sparklineParentAction;
				InsertAction parentTreeAction = sparklineParentAction.Parent;
				while (parentTreeAction != null && parentTreeAction.Parent != null) {
					rootOutsideAction = parentTreeAction;
					parentTreeAction = parentTreeAction.Parent;
				}
				return rootOutsideAction;
			}
		}
		protected override bool IsRootListInsertAction(DataBrowser dataBrowser) {
			var res = base.IsRootListInsertAction(dataBrowser);
			if (dataSourceType != DataSourceType.Named || contextType == ContextType.Within)
				return res;
			return !res && dataBrowser.Parent.Parent.Parent == null;
		}
		protected override string GetDataMember(DataMemberInfo dataMemberInfo) {
			var res = base.GetDataMember(dataMemberInfo);
			if (dataSourceType != DataSourceType.Named || contextType == ContextType.Within)
				return res;
			if (dataMemberInfo.ParentDataMemberInfo == null)
				return res;
			return dataMemberInfo.ParentDataMemberInfo.DataMember;
		}
		protected override void CalculateRelation(SNDataInfo dataInfo, SNDataInfo[] dataInfoArray, List<SNDataInfo> relatedDataInfos, SnapDocumentModel documentModel, List<string> listNames, RootInsertAction rootInsertAction, List<string> insertPositionDataSources) {
			bool shouldntCalcRelation = contextType != ContextType.None && contextLevel == ContextLevel.Correct;
			if (shouldntCalcRelation || contextLevel == ContextLevel.None)
				return;
			calculateRelationAction = true;
			currentDataInfo = dataInfo;
			base.CalculateRelation(dataInfo, dataInfoArray, relatedDataInfos, documentModel, listNames, rootInsertAction, insertPositionDataSources);
			currentDataInfo = null;
			calculateRelationAction = false;
		}
		protected override bool CanAddRelatedDataInfo(SNDataInfo relatedDataInfo, SNDataInfo[] dataInfos) {
			if (contextType == ContextType.Within && currentDataInfo != null && currentDataInfo.DataPaths.Length > 1) {
				return IsRelatedDataInfoInRightContext(relatedDataInfo.DataPaths, currentDataInfo.DataPaths);
			}
			return base.CanAddRelatedDataInfo(relatedDataInfo, dataInfos);
		}
		bool IsRelatedDataInfoInRightContext(string[] relatedDataPaths, string[] currentDataPaths) {
			if (currentDataPaths.Length == 0 || relatedDataPaths.Length == 0)
				return false;
			if (relatedDataPaths.Length != currentDataPaths.Length - 1)
				return false;
			if (relatedDataPaths.Length == 1 && currentDataPaths.Length == 2)
				return true;
			for (int i = 0; i < relatedDataPaths.Length - 1; i++) {
				if (!string.Equals(relatedDataPaths[i], currentDataPaths[i]))
					return false;
			}
			return true;
		}
	}
	public class TemplateBuilder {
		public bool InduceRelation { get; set; }
		public DocumentModel CreateTemplateFromDraggedDataInfo(SnapPieceTable sourcePieceTable, DocumentLogPosition position, SNDataInfo[] dataInfoArray) {
			if (dataInfoArray == null || dataInfoArray.Length == 0)
				return null;
			SnapBookmarkController bookmarkController = new SnapBookmarkController(sourcePieceTable);
			SnapBookmark bookmark = bookmarkController.FindInnermostTemplateBookmarkByPosition(position);
			SnapDocumentModel sourceModel = sourcePieceTable.DocumentModel;
			SnapDocumentModel documentModel = (SnapDocumentModel)sourceModel.CreateNew();
			documentModel.BeginSetContent();
			documentModel.IntermediateModel = true;
			documentModel.InheritDataServices(sourceModel);
			if (sourceModel.SnapMailMergeVisualOptions.DataSourceName != null)
				documentModel.SnapMailMergeVisualOptions.CopyFrom(sourceModel.SnapMailMergeVisualOptions);
			try {
				DocumentModelCopyCommand.ReplaceDefaultProperties(documentModel, sourceModel);
				DocumentModelCopyCommand.CopyStyles(documentModel, sourceModel);
				documentModel.FieldOptions.CopyFrom(sourceModel.FieldOptions);
				List<string> insertPositionDataSources = GetInsertPositionDataSources(sourcePieceTable, bookmark);
				InsertActionTree insertActionTree = CalculateInsertActionTree(sourceModel, dataInfoArray, insertPositionDataSources);
				TemplateWriter templateWriter = new TemplateWriter((SnapPieceTable)documentModel.MainPieceTable, CreateInputPosition(documentModel.MainPieceTable, sourcePieceTable, position));
				WriteTree(templateWriter, insertActionTree, insertPositionDataSources);
			}
			finally {
				documentModel.EndSetContent(DocumentModelChangeType.LoadNewDocument, false, null);
			}
			if (bookmark != null && bookmark.FieldContext != null)
				((SnapDocumentModel)documentModel).SetRootDataContext(bookmark.FieldContext);
			return documentModel;
		}
		protected internal virtual InputPosition CreateInputPosition(PieceTable targetPieceTable, PieceTable sourcePieceTable, DocumentLogPosition logPosition) {
			DocumentModelPosition currentFormattingPosition = PositionConverter.ToDocumentModelPosition(sourcePieceTable, logPosition);
			TextRunBase run = sourcePieceTable.Runs[currentFormattingPosition.RunIndex];
			InputPosition pos = new InputPosition(targetPieceTable);
			pos.CharacterStyleIndex = run.CharacterStyle.Copy(targetPieceTable.DocumentModel);
			pos.CharacterFormatting.CopyFrom(run.CharacterProperties.Info);
			return pos;
		}
		protected internal void WriteTree(TemplateWriter templateWriter, InsertActionTree tree, List<string> insertPositionDataSource) {
			InsertActionTreeNode rootNode = tree.RootNode;
			foreach (InsertActionTreeNode node in rootNode.ChildNodes) {
				InsertObjectActionBase insertObjectAction = node.Action as InsertObjectActionBase;
				if (insertObjectAction == null) {
					templateWriter.WriteTree(node);
					continue;
				}
				string dataReferenceString = insertObjectAction.GetDataReferenceString();
				int index = insertPositionDataSource.IndexOf(dataReferenceString);
				if (index < 0) {
					templateWriter.WriteTree(node);
					continue;
				}
				WriteChildNodesCore(templateWriter, node, insertPositionDataSource, index - 1);
			}
		}
		public void WriteChildNodesCore(TemplateWriter templateWriter, InsertActionTreeNode parentNode, List<string> insertPositionDataSource, int index) {
			foreach (InsertActionTreeNode node in parentNode.ChildNodes) {
				if (node.Action is InsertRelationAction)
					continue;
				InsertObjectActionBase insertObjectAction = node.Action as InsertObjectActionBase;
				if (insertObjectAction == null) {
					templateWriter.WriteTree(node);
					continue;
				}
				if (index < 0) {
					templateWriter.WriteTree(node);
					continue;
				}
				if (insertPositionDataSource[index] == insertObjectAction.GetDataReferenceString())
					WriteChildNodesCore(templateWriter, node, insertPositionDataSource, index - 1);
			}
		}
		public List<string> GetInsertPositionDataSources(SnapPieceTable sourcePieceTable, SnapBookmark bookmark) {
			List<string> result = new List<string>();
			if (bookmark == null)
				return result;
			Field field = sourcePieceTable.FindNonTemplateFieldByRunIndex(bookmark.Interval.NormalizedStart.RunIndex);
			SnapFieldCalculatorService parser = new SnapFieldCalculatorService();
			for (; field != null; field = field.Parent) {
				CalculatedFieldBase parsedInfo = parser.ParseField(sourcePieceTable, field);
				if (parsedInfo == null)
					continue;
				TemplatedFieldBase templatedField = parsedInfo as TemplatedFieldBase;
				if (templatedField == null)
					continue;
				string dataSourceName = templatedField.DataSourceName;
				int index = String.IsNullOrEmpty(dataSourceName) ? -1 : dataSourceName.IndexOf('[');
				if (index > 0)
					dataSourceName = dataSourceName.Substring(0, index);
				else
					if (index == 0)
						dataSourceName = String.Empty;
				result.Add(dataSourceName);
			}
			return result;
		}
		protected virtual InsertAction GetInsertAction(SnapDocumentModel documentModel, List<string> listNames, string listName, DataContext dataContext, DataBrowser dataBrowser, object dataSource, DataMemberInfo dataMemberInfo, InsertAction parentAction, bool leafAction, List<string> insertPositionDataSources) {
			string name = documentModel.GetListName(listNames, listName);
			if (dataBrowser.Parent == null || (IsRootListInsertAction(dataBrowser) && dataBrowser.DataSource is ITypedList && dataSource is IListSource)) {
				listNames.Add(name);
				string dataSourceName = documentModel.DataSourceDispatcher.FindDataSourceName(dataSource);
				return InsertDisplayObjectAction.Create(dataBrowser.DataSourceType, dataSourceName, GetDataMember(dataMemberInfo), parentAction, name);
			}
			string parentDataMember = FieldPathService.DecodePath(dataMemberInfo.ParentDataMemberInfo.DataMember);
			DataBrowser parentDataBrowser = dataContext.GetDataBrowser(dataSource, parentDataMember, true);
			if (ShouldCreateInsertAction(documentModel, dataSource, parentDataMember))
				parentAction = GetInsertAction(documentModel, listNames, listName, dataContext, parentDataBrowser, dataSource, dataMemberInfo.ParentDataMemberInfo, parentAction, false, insertPositionDataSources);
			if (!leafAction) {
				listNames.Add(name);
				return InsertDisplayObjectAction.Create(dataBrowser.DataSourceType, String.Empty, GetColumnName(dataMemberInfo), parentAction, name);
			}
			return CreateInsertAction(dataBrowser, dataMemberInfo, parentAction);
		}
		protected virtual bool ShouldCreateInsertAction(SnapDocumentModel documentModel, object dataSource, string parentDataMember) {
			SnapMailMergeVisualOptions options = documentModel.SnapMailMergeVisualOptions;
			bool correspondsDataSource = Object.ReferenceEquals(dataSource, options.DataSource);
			bool correspondsDataMember;
			if (string.IsNullOrEmpty(options.DataMember) && string.IsNullOrEmpty(parentDataMember))
				correspondsDataMember = true;
			else
				correspondsDataMember = string.Compare(options.DataMember, parentDataMember) == 0;
			return !(correspondsDataSource && correspondsDataMember);
		}
		protected virtual bool IsRootListInsertAction(DataBrowser dataBrowser) {
			return dataBrowser.Parent.Parent == null;
		}
		protected virtual string GetColumnName(DataMemberInfo dataMemberInfo) {
			return dataMemberInfo.ColumnName;
		}
		protected virtual string GetDataMember(DataMemberInfo dataMemberInfo) {
			return dataMemberInfo.DataMember;
		}
		protected virtual InsertAction CreateInsertAction(DataBrowser dataBrowser, DataMemberInfo dataMemberInfo, InsertAction parentAction) {
			if (TemplatedFieldTypeQualifier.IsImageField(dataBrowser))
				return new InsertSNImageFieldAction(GetColumnName(dataMemberInfo), parentAction);
			if (TemplatedFieldTypeQualifier.IsCheckBoxField(dataBrowser))
				return new InsertSNCheckBoxAction(GetColumnName(dataMemberInfo), parentAction);
			if (TemplatedFieldTypeQualifier.IsHyperlinkField(dataBrowser))
				return new InsertSNHyperlinkFieldAction(GetColumnName(dataMemberInfo), parentAction);
			return new InsertSNTextFieldAction(GetColumnName(dataMemberInfo), parentAction);
		}
		internal InsertActionTree CalculateInsertActionTree(SnapDocumentModel documentModel, SNDataInfo[] dataInfoArray, List<string> insertPositionDataSources) {
			RootInsertAction rootInsertAction = new RootInsertAction();
			List<string> listNames = new List<string>(documentModel.GetListNames());
			List<SNDataInfo> relatedDataInfos = new List<SNDataInfo>();
			foreach (SNDataInfo dataInfo in dataInfoArray) {
				InsertAction insertAction = CalculateInsertAction(documentModel, listNames, dataInfo, rootInsertAction, insertPositionDataSources);
				if (insertAction == null)
					continue;
				if (InduceRelation && dataInfo.RelatedData != null)
					CalculateRelation(dataInfo, dataInfoArray, relatedDataInfos, documentModel, listNames, rootInsertAction, insertPositionDataSources);
				rootInsertAction.AddLeafAction(insertAction);
			}
			return rootInsertAction.GetInsertActionTree();
		}
		protected virtual void CalculateRelation(SNDataInfo dataInfo, SNDataInfo[] dataInfoArray, List<SNDataInfo> relatedDataInfos, SnapDocumentModel documentModel, List<string> listNames, RootInsertAction rootInsertAction, List<string> insertPositionDataSources) {
			foreach (SNDataInfo relatedDataInfo in dataInfo.RelatedData) {
				if (relatedDataInfos.Contains(relatedDataInfo) || !CanAddRelatedDataInfo(relatedDataInfo, dataInfoArray))
					continue;
				DataMemberInfo dataMemberInfo = DataMemberInfo.Create(relatedDataInfo.EscapedDataPaths);
				if (!ShouldCreateInsertAction(documentModel, relatedDataInfo.Source, dataMemberInfo.ParentDataMemberInfo.DataMember))
					continue;
				InsertAction insertRelationAction = CalculateInsertAction(documentModel, listNames, relatedDataInfo, rootInsertAction, insertPositionDataSources);
				if (insertRelationAction != null)
					rootInsertAction.AddLeafAction(new InsertRelationAction(insertRelationAction));
				relatedDataInfos.Add(relatedDataInfo);
			}
		}
		InsertAction CalculateInsertAction(SnapDocumentModel documentModel, List<string> listNames, SNDataInfo dataInfo, InsertAction root, List<string> insertPositionDataSources) {
			object dataSource = dataInfo.Source;
			string dataMember = dataInfo.Member;
			IDataSourceDisplayNameProvider dataSourceDisplayNameProvider = documentModel.GetService<IDataSourceDisplayNameProvider>();
			IDataSourceDispatcher dataSourceDispatcher = documentModel.DataSourceDispatcher;
			DataSourceInfo dataSourceInfo = dataSourceDispatcher.GetInfo(dataInfo.Source);
			if (dataSource is ParametersDataSource) {
				DataMemberInfo dataMemberInfo = DataMemberInfo.Create(dataInfo.DataPaths);
				return new InsertParameterAsSNTextFieldAction(dataMemberInfo.ColumnName, root);
			}
			else {
				if (dataSourceInfo == null)
					return null;
				using (DataContext dataContext = new SnapDataContext(dataSourceInfo.CalculatedFields, documentModel.Parameters, dataSourceDisplayNameProvider)) {
					DataBrowser dataBrowser = dataContext.GetDataBrowser(dataSource, dataMember, true);
					if (dataBrowser == null)
						return null;
					string listName = string.Empty;
					if (object.ReferenceEquals(dataSourceDispatcher.DefaultDataSource, dataSource))
						listName = DesignBindingHelper.GetListName(dataContext, dataSource, dataMember);
					else
						listName = dataSourceDispatcher.FindDataSourceName(dataSource);
					return GetInsertAction(documentModel, listNames, listName, dataContext, dataBrowser, dataSource, DataMemberInfo.Create(dataInfo.EscapedDataPaths), root, true, insertPositionDataSources);
				}
			}
		}
		protected virtual bool CanAddRelatedDataInfo(SNDataInfo relatedDataInfo, SNDataInfo[] dataInfos) {
			return Array.TrueForAll(dataInfos, dataInfo =>
			{
				return !dataInfo.IsNeighbour(relatedDataInfo);
			});
		}
	}
}
