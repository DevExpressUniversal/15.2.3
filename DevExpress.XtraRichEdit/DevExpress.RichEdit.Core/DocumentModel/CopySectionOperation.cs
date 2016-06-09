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
using DevExpress.Utils;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model.History;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Tables.Native;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Model {
	#region ParagraphNumerationCopyOptions
	public enum ParagraphNumerationCopyOptions {
		CopyAlways,
		CopyIfWholeSelected
	}
	#endregion
	#region CharacterFormattingCopyOptions
	public enum FormattingCopyOptions {
		KeepSourceFormatting,
		UseDestinationStyles
	}
	#endregion
	#region DefaultPropertiesCopyOptions
	public enum DefaultPropertiesCopyOptions {
		Always,
		Never
	}
	#endregion
	#region TableCopyHelper
	public class TableCopyHelper {
		#region Fields
		Stack<TableCopyInfo> tableCopyStack;
		readonly DocumentModelCopyManager owner;
		Dictionary<TableCellProperties, TableCellProperties> copiedProperties;
		ParagraphIndex targetStartParagraphIndex;
		bool suppressCopyTables;
		int copyFromNestedLevel = -1;
		#endregion
		public TableCopyHelper(DocumentModelCopyManager owner) {
			this.owner = owner;
			this.targetStartParagraphIndex = owner.TargetPosition.Clone().ParagraphIndex;
			tableCopyStack = new Stack<TableCopyInfo>();
			tableCopyStack.Push(new TableCopyInfo());
			this.copiedProperties = new Dictionary<TableCellProperties, TableCellProperties>();
		}
		#region Properties
		TableCopyInfo TableCopyState { get { return tableCopyStack.Peek(); } }
		public bool SuppressCopyTables { get { return suppressCopyTables; } set { suppressCopyTables = value; } }
		public Table LastSourceTable { get { return TableCopyState.LastSourceTable; } set { TableCopyState.LastSourceTable = value; } }
		public TableRow LastSourceRow { get { return TableCopyState.LastSourceRow; } set { TableCopyState.LastSourceRow = value; } }
		public Table LastTargetTable { get { return TableCopyState.LastTargetTable; } set { TableCopyState.LastTargetTable = value; } }
		public TableCell LastSourceCell { get { return TableCopyState.LastSourceCell; } set { TableCopyState.LastSourceCell = value; } }
		public PieceTable SourcePieceTable { get { return owner.SourcePieceTable; } }
		public PieceTable TargetPieceTable { get { return owner.TargetPieceTable; } }
		public DocumentModel SourceModel { get { return owner.SourcePieceTable.DocumentModel; } }
		public DocumentModel TargetModel { get { return owner.TargetPieceTable.DocumentModel; } }
		public ParagraphIndex TargetStartParagraphIndex { get { return targetStartParagraphIndex; } set { targetStartParagraphIndex = value; } }
		internal int CopyFromNestedLevel { get { return copyFromNestedLevel; } set { copyFromNestedLevel = value; } }
		#endregion
		#region TableCopyInfo
		class TableCopyInfo {
			Table lastSourceTable = null;
			Table lastTargetTable = null;
			TableRow lastSourceRow = null;
			TableCell lastSourceCell = null;
			public Table LastSourceTable { get { return lastSourceTable; } set { lastSourceTable = value; } }
			public TableRow LastSourceRow { get { return lastSourceRow; } set { lastSourceRow = value; } }
			public Table LastTargetTable { get { return lastTargetTable; } set { lastTargetTable = value; } }
			public TableCell LastSourceCell { get { return lastSourceCell; } set { lastSourceCell = value; } }
		}
		#endregion
		Table CreateTable(PieceTable pieceTable, TableCell targetParentCell, TableCell sourceCell) {
			Table result = pieceTable.CreateTableCore(targetParentCell);
			LastSourceTable = sourceCell.Table;
			result.CopyProperties(sourceCell.Table);
			return result;
		}
		void CreateRow(Table table, TableRowProperties sourceRowProperties, TableProperties sourceTablePropertiesException) {
			TableRow lastTargetRow = table.PieceTable.CreateTableRowCore(table);
			lastTargetRow.Properties.CopyFrom(sourceRowProperties);
			lastTargetRow.TablePropertiesException.CopyFrom(sourceTablePropertiesException);
		}
		TableCell CreateCell(ParagraphIndex startCellParagraphIndex, ParagraphIndex endParagraphIndex, TableCell sourceCell) {
			TableCell lastTargetCell = TargetPieceTable.CreateTableCellCore(LastTargetTable.LastRow, startCellParagraphIndex, endParagraphIndex);
			lastTargetCell.CopyProperties(sourceCell);
			return lastTargetCell;
		}
		bool IsNewRow(TableRow tableRow) {
			return !Object.ReferenceEquals(tableRow, LastSourceRow);
		}
		bool IsNewTable(Table table) {
			return !Object.ReferenceEquals(table, LastSourceTable);
		}
		public void CopyTables(RunInfo sourceRunInfo) {
			if (!TargetModel.DocumentCapabilities.TablesAllowed || !TargetModel.DocumentCapabilities.ParagraphsAllowed)
				return;
			ParagraphIndex start = sourceRunInfo.NormalizedStart.ParagraphIndex;
			ParagraphIndex end = sourceRunInfo.NormalizedEnd.ParagraphIndex;
			int paragraphIndexOffset = sourceRunInfo.NormalizedStart.ParagraphIndex - TargetStartParagraphIndex;
			TableCellsManager.TableCellNode root = SourcePieceTable.TableCellsManager.GetCellSubTree(start, end, CopyFromNestedLevel);
			TableCell targetCell = GetTargetCell(root, paragraphIndexOffset);
			if (targetCell != null)
				CopyFromNestedLevel = targetCell.Table.NestedLevel + 1;
			ProcessTableCore(root, targetCell, paragraphIndexOffset, sourceRunInfo);
		}
		protected internal virtual void ProcessTableCore(TableCellsManager.TableCellNode node, TableCell targetParentCell, int paragraphIndexOffset, RunInfo runInfo) {
			if (node == null || node.ChildNodes == null || node.ChildNodes.Count == 0)
				return;
			if (targetParentCell != null)
				tableCopyStack.Push(new TableCopyInfo());
			try {
				for (int i = 0; i < node.ChildNodes.Count; i++) {
					if (node.ChildNodes[i].Cell == null) {
						ProcessTableCore(node.ChildNodes[i], targetParentCell, paragraphIndexOffset, runInfo); 
						continue;
					}
					if (!CopyCellAllowed(node.ChildNodes[i].Cell, runInfo)) {
						ProcessTableCore(node.ChildNodes[i], targetParentCell, paragraphIndexOffset, runInfo);
						return;
					}
					if (IsNewTable(node.ChildNodes[i].Cell.Table))
						LastTargetTable = CreateTable(TargetPieceTable, targetParentCell, node.ChildNodes[i].Cell);
					TableCell sourceCell = node.ChildNodes[i].Cell;
					if (IsNewRow(sourceCell.Row)) {
						LastSourceRow = sourceCell.Row;
						CreateRow(LastTargetTable, LastSourceRow.Properties, LastSourceRow.TablePropertiesException);
					}
					ParagraphIndex targetStart = sourceCell.StartParagraphIndex - paragraphIndexOffset;
					ParagraphIndex targetEnd = sourceCell.EndParagraphIndex - paragraphIndexOffset;
					TableCell newTargetCell = CreateCell(targetStart, targetEnd, sourceCell);
					ProcessTableCore(node.ChildNodes[i], newTargetCell, paragraphIndexOffset, runInfo);
				}
			}
			finally {
				if (targetParentCell != null)
					FinalizeNestedTableCreation();
			}
		}
		protected internal bool IsOneCellCopying(TableCellsManager.TableCellNode root) {
			return root != null && root.ChildNodes.Count == 1;
		}
		protected internal TableCell GetTargetCell(TableCellsManager.TableCellNode root, int paragraphIndexOffset) {
			if (root == null || root.ChildNodes == null || root.ChildNodes.First == null || root.ChildNodes.First.Cell == null)
				return null;
			ParagraphIndex targetStart = root.ChildNodes.First.Cell.StartParagraphIndex - paragraphIndexOffset;
			if (targetStart < ParagraphIndex.Zero)
				targetStart = ParagraphIndex.Zero;
			if (targetStart >= TargetPieceTable.Paragraphs.Last.Index)
				return null;
			return TargetPieceTable.Paragraphs[targetStart].GetCell();
		}
		void FinalizeNestedTableCreation() {
			tableCopyStack.Pop();
		}
		protected internal virtual bool CopyCellAllowed(TableCell cell, RunInfo info) {
			if (info == null)
				return true;
			ParagraphCollection paragraphs = owner.SourcePieceTable.Paragraphs;
			RunIndex startParagraphFirstRunIndex = paragraphs[cell.StartParagraphIndex].FirstRunIndex;
			RunIndex endParagraphLastRunIndex = paragraphs[cell.EndParagraphIndex].LastRunIndex;
			if (info.Start.RunIndex <= startParagraphFirstRunIndex && info.End.RunIndex >= endParagraphLastRunIndex)
				return true;
			return false;
		}
	}
	#endregion
	public struct RunCharacterFormatting {
		readonly int characterStyleIndex;
		readonly int characterPropertiesIndex;
		readonly int paragraphStyleIndex;
		public RunCharacterFormatting(int characterStyleIndex, int characterPropertiesIndex, int paragraphStyleIndex) {
			this.characterStyleIndex = characterStyleIndex;
			this.characterPropertiesIndex = characterPropertiesIndex;
			this.paragraphStyleIndex = paragraphStyleIndex;
		}
		public int CharacterStyleIndex { get { return characterStyleIndex; } }
		public int CharacterPropertiesIndex { get { return characterPropertiesIndex; } }
		public int ParagraphStyleIndex { get { return paragraphStyleIndex; } }
	}
	public struct RunParagraphFormatting {
		readonly int paragraphStyleIndex;
		readonly int paragraphPropertiesIndex;
		public RunParagraphFormatting(int paragraphStyleIndex, int paragraphPropertiesIndex) {
			this.paragraphStyleIndex = paragraphStyleIndex;
			this.paragraphPropertiesIndex = paragraphPropertiesIndex;
		}
		public int ParagraphStyleIndex { get { return paragraphStyleIndex; } }
		public int ParagraphPropertiesIndex { get { return paragraphPropertiesIndex; } }
	}
	#region DocumentModelCopyManager
	public class DocumentModelCopyManager {
		#region Fields
		readonly PieceTable sourcePieceTable;
		readonly PieceTable targetPieceTable;
		readonly DocumentModelPosition targetPosition;
		readonly ParagraphNumerationCopyOptions paragraphNumerationCopyOptions;
		readonly FormattingCopyOptions formattingCopyOptions;
		TableCopyHelper tableCopyHelper;
		bool paragraphWasInsertedBeforeTable;
		bool isInserted;
		Dictionary<RunCharacterFormatting, RunCharacterFormatting> mapSourceToTargetCharacterFormatting;
		Dictionary<RunParagraphFormatting, RunParagraphFormatting> mapSourceToTargetParagraphFormatting;
		#endregion
		public DocumentModelCopyManager(PieceTable sourcePieceTable, PieceTable targetPieceTable, ParagraphNumerationCopyOptions paragraphNumerationCopyOptions) : this(sourcePieceTable, targetPieceTable, paragraphNumerationCopyOptions, FormattingCopyOptions.UseDestinationStyles) {
		}
		public DocumentModelCopyManager(PieceTable sourcePieceTable, PieceTable targetPieceTable, ParagraphNumerationCopyOptions paragraphNumerationCopyOptions, FormattingCopyOptions formattingCopyOptions) {
			Guard.ArgumentNotNull(sourcePieceTable, "sourcePieceTable");
			Guard.ArgumentNotNull(targetPieceTable, "targetPieceTable");
			this.sourcePieceTable = sourcePieceTable;
			this.targetPieceTable = targetPieceTable;
			this.targetPosition = new DocumentModelPosition(targetPieceTable);
			this.paragraphNumerationCopyOptions = paragraphNumerationCopyOptions;
			this.formattingCopyOptions = formattingCopyOptions;
			this.tableCopyHelper = new TableCopyHelper(this);
			this.mapSourceToTargetCharacterFormatting = new Dictionary<RunCharacterFormatting, RunCharacterFormatting>();
			this.mapSourceToTargetParagraphFormatting = new Dictionary<RunParagraphFormatting, RunParagraphFormatting>();
		}
		#region Properties
		public PieceTable SourcePieceTable { get { return sourcePieceTable; } }
		public PieceTable TargetPieceTable { get { return targetPieceTable; } }
		public DocumentModel SourceModel { get { return sourcePieceTable.DocumentModel; } }
		public DocumentModel TargetModel { get { return targetPieceTable.DocumentModel; } }
		public DocumentModelPosition TargetPosition { get { return targetPosition; } }
		public ParagraphNumerationCopyOptions ParagraphNumerationCopyOptions { get { return paragraphNumerationCopyOptions; } }
		public FormattingCopyOptions FormattingCopyOptions { get { return formattingCopyOptions; } } 
		public bool ParagraphWasInsertedBeforeTable { get { return paragraphWasInsertedBeforeTable; } set { paragraphWasInsertedBeforeTable = value; } }
		public TableCopyHelper TableCopyHelper { get { return tableCopyHelper; } set { tableCopyHelper = value; } }
		public bool IsInserted { get { return isInserted; } set { isInserted = value; } }
		#endregion
		public void OnTargetRunInserted(TextRunBase sourceRun, TextRunBase targetRun) {
			int runLength = sourceRun.Length;
			targetPosition.RunStartLogPosition += runLength;
			targetPosition.LogPosition += runLength;
			targetPosition.RunIndex++;
			if (targetPieceTable == TargetModel.MainPieceTable)
				Debug.Assert(targetPosition.RunIndex == PositionConverter.ToDocumentModelPosition(TargetModel.MainPieceTable, targetPosition.LogPosition).RunIndex);
			if(formattingCopyOptions == FormattingCopyOptions.KeepSourceFormatting)
				ApplySourceCharacterFormatting(sourceRun, targetRun);
		}
		public void OnTargetParagraphInserted(Paragraph sourceParagraph, Paragraph targetParagraph, TextRunBase sourceRun, TextRunBase targetRun) {
			targetPosition.ParagraphIndex++;			
			OnTargetRunInserted(sourceRun, targetRun);
			if (formattingCopyOptions == FormattingCopyOptions.KeepSourceFormatting)
				ApplySourceParagraphFormatting(sourceParagraph, targetParagraph);
		}
		public void OnTargetSectionInserted(Section sourceSection, Section targetSection) {
			RunIndex targetSectionLastRunIndex = TargetPieceTable.Paragraphs[targetSection.LastParagraphIndex].LastRunIndex;
			RunIndex sourceSectionLastRunIndex = SourcePieceTable.Paragraphs[sourceSection.LastParagraphIndex].LastRunIndex;
			targetPosition.ParagraphIndex++;
			ParagraphRun targetRun = (ParagraphRun)TargetPieceTable.Runs[targetSectionLastRunIndex];
			ParagraphRun sourceRun = (ParagraphRun)SourcePieceTable.Runs[sourceSectionLastRunIndex];
			OnTargetRunInserted(sourceRun, targetRun);
		}
		public virtual void CopyAdditionalInfo(bool copyBetweenInternalModels) { 
		}
		void ApplySourceCharacterFormatting(TextRunBase sourceRun, TextRunBase targetRun) {
			RunCharacterFormatting targetFormatting;
			RunCharacterFormatting sourceFormatting = new RunCharacterFormatting(sourceRun.CharacterProperties.Index, sourceRun.CharacterStyleIndex, sourceRun.Paragraph.ParagraphStyleIndex);
			if (mapSourceToTargetCharacterFormatting.TryGetValue(sourceFormatting, out targetFormatting)) {
				targetRun.CharacterStyleIndex = targetFormatting.CharacterStyleIndex;
				targetRun.CharacterProperties.ChangeIndexCore(targetFormatting.CharacterPropertiesIndex, DocumentModelChangeActions.None);
			}
			else {
				targetRun.CharacterProperties.BeginUpdate();
				try {
					if (sourceRun.FontName != targetRun.FontName)
						targetRun.FontName = sourceRun.FontName;
					if (sourceRun.DoubleFontSize != targetRun.DoubleFontSize)
						targetRun.DoubleFontSize = sourceRun.DoubleFontSize;
					if (sourceRun.FontBold != targetRun.FontBold)
						targetRun.FontBold = sourceRun.FontBold;
					if (sourceRun.FontItalic != targetRun.FontItalic)
						targetRun.FontItalic = sourceRun.FontItalic;
					if (sourceRun.FontStrikeoutType != targetRun.FontStrikeoutType)
						targetRun.FontStrikeoutType = sourceRun.FontStrikeoutType;
					if (sourceRun.FontUnderlineType != targetRun.FontUnderlineType)
						targetRun.FontUnderlineType = sourceRun.FontUnderlineType;
					if (sourceRun.AllCaps != targetRun.AllCaps)
						targetRun.AllCaps = sourceRun.AllCaps;
					if (sourceRun.UnderlineWordsOnly != targetRun.UnderlineWordsOnly)
						targetRun.UnderlineWordsOnly = sourceRun.UnderlineWordsOnly;
					if (sourceRun.StrikeoutWordsOnly != targetRun.StrikeoutWordsOnly)
						targetRun.StrikeoutWordsOnly = sourceRun.StrikeoutWordsOnly;
					if (sourceRun.ForeColor != targetRun.ForeColor)
						targetRun.ForeColor = sourceRun.ForeColor;
#if THEMES_EDIT
					if (!sourceRun.ForeColorInfo.Equals(targetRun.ForeColorInfo))
						targetRun.ForeColorInfo = sourceRun.ForeColorInfo;
#endif
					if (sourceRun.BackColor != targetRun.BackColor)
						targetRun.BackColor = sourceRun.BackColor;
#if THEMES_EDIT
					if (!sourceRun.BackColorInfo.Equals(targetRun.BackColorInfo))
						targetRun.BackColorInfo = sourceRun.BackColorInfo;
					if (!sourceRun.Shading.Equals(targetRun.Shading))
						targetRun.Shading = sourceRun.Shading;
#endif
					if (sourceRun.UnderlineColor != targetRun.UnderlineColor)
						targetRun.UnderlineColor = sourceRun.UnderlineColor;
#if THEMES_EDIT
					if (!sourceRun.UnderlineColorInfo.Equals(targetRun.UnderlineColorInfo))
						targetRun.UnderlineColorInfo = sourceRun.UnderlineColorInfo;
#endif
					if (sourceRun.StrikeoutColor != targetRun.StrikeoutColor)
						targetRun.StrikeoutColor = sourceRun.StrikeoutColor;
#if THEMES_EDIT
					if (!sourceRun.StrikeoutColorInfo.Equals(targetRun.StrikeoutColorInfo))
						targetRun.StrikeoutColorInfo = sourceRun.StrikeoutColorInfo;
#endif
					if (sourceRun.Script != targetRun.Script)
						targetRun.Script = sourceRun.Script;
					if (sourceRun.Hidden != targetRun.Hidden)
						targetRun.Hidden = sourceRun.Hidden;
				}
				finally {
					targetRun.CharacterProperties.EndUpdate();
				}
				mapSourceToTargetCharacterFormatting.Add(sourceFormatting, new RunCharacterFormatting(targetRun.CharacterStyleIndex, targetRun.CharacterProperties.Index, targetRun.Paragraph.ParagraphStyleIndex));
			}
		}
		void ApplySourceParagraphFormatting(Paragraph sourceParagraph, Paragraph targetParagraph) {
			RunParagraphFormatting targetFormatting;
			RunParagraphFormatting sourceFormatting = new RunParagraphFormatting(sourceParagraph.ParagraphProperties.Index, sourceParagraph.ParagraphStyleIndex);
			if (mapSourceToTargetParagraphFormatting.TryGetValue(sourceFormatting, out targetFormatting)) {
				targetParagraph.ParagraphStyleIndex = targetFormatting.ParagraphStyleIndex;
				targetParagraph.ParagraphProperties.ChangeIndexCore(targetFormatting.ParagraphPropertiesIndex, DocumentModelChangeActions.None);
			}
			else {
				targetParagraph.ParagraphProperties.BeginUpdate();
				try {
					if (sourceParagraph.Alignment != targetParagraph.Alignment)
						targetParagraph.Alignment = sourceParagraph.Alignment;
					if (sourceParagraph.LeftIndent != targetParagraph.LeftIndent)
						targetParagraph.LeftIndent = sourceParagraph.LeftIndent;
					if (sourceParagraph.RightIndent != targetParagraph.RightIndent)
						targetParagraph.RightIndent = sourceParagraph.RightIndent;
					if (sourceParagraph.SpacingBefore != targetParagraph.SpacingBefore)
						targetParagraph.SpacingBefore = sourceParagraph.SpacingBefore;
					if (sourceParagraph.SpacingAfter != targetParagraph.SpacingAfter)
						targetParagraph.SpacingAfter = sourceParagraph.SpacingAfter;
					if (sourceParagraph.LineSpacingType != targetParagraph.LineSpacingType)
						targetParagraph.LineSpacingType = sourceParagraph.LineSpacingType;
					if (sourceParagraph.LineSpacing != targetParagraph.LineSpacing)
						targetParagraph.LineSpacing = sourceParagraph.LineSpacing;
					if (sourceParagraph.FirstLineIndentType != targetParagraph.FirstLineIndentType)
						targetParagraph.FirstLineIndentType = sourceParagraph.FirstLineIndentType;
					if (sourceParagraph.FirstLineIndent != targetParagraph.FirstLineIndent)
						targetParagraph.FirstLineIndent = sourceParagraph.FirstLineIndent;
					if (sourceParagraph.SuppressHyphenation != targetParagraph.SuppressHyphenation)
						targetParagraph.SuppressHyphenation = sourceParagraph.SuppressHyphenation;
					if (sourceParagraph.SuppressLineNumbers != targetParagraph.SuppressLineNumbers)
						targetParagraph.SuppressLineNumbers = sourceParagraph.SuppressLineNumbers;
					if (sourceParagraph.ContextualSpacing != targetParagraph.ContextualSpacing)
						targetParagraph.ContextualSpacing = sourceParagraph.ContextualSpacing;
					if (sourceParagraph.PageBreakBefore != targetParagraph.PageBreakBefore)
						targetParagraph.PageBreakBefore = sourceParagraph.PageBreakBefore;
					if (sourceParagraph.BeforeAutoSpacing != targetParagraph.BeforeAutoSpacing)
						targetParagraph.BeforeAutoSpacing = sourceParagraph.BeforeAutoSpacing;
					if (sourceParagraph.AfterAutoSpacing != targetParagraph.AfterAutoSpacing)
						targetParagraph.AfterAutoSpacing = sourceParagraph.AfterAutoSpacing;
					if (sourceParagraph.KeepWithNext != targetParagraph.KeepWithNext)
						targetParagraph.KeepWithNext = sourceParagraph.KeepWithNext;
					if (sourceParagraph.KeepLinesTogether != targetParagraph.KeepLinesTogether)
						targetParagraph.KeepLinesTogether = sourceParagraph.KeepLinesTogether;
					if (sourceParagraph.WidowOrphanControl != targetParagraph.WidowOrphanControl)
						targetParagraph.WidowOrphanControl = sourceParagraph.WidowOrphanControl;
					if (sourceParagraph.OutlineLevel != targetParagraph.OutlineLevel)
						targetParagraph.OutlineLevel = sourceParagraph.OutlineLevel;
					if (sourceParagraph.BackColor != targetParagraph.BackColor)
						targetParagraph.BackColor = sourceParagraph.BackColor;
				}
				finally {
					targetParagraph.ParagraphProperties.EndUpdate();
				}
				mapSourceToTargetParagraphFormatting.Add(sourceFormatting, new RunParagraphFormatting(targetParagraph.ParagraphStyleIndex, targetParagraph.ParagraphProperties.Index));
			}
		}
	}
	#endregion
	#region CopySectionOperation
	public class CopySectionOperation : SelectionBasedOperation {
		#region Fields
		readonly DocumentModelCopyManager copyManager;
		int transactionItemCountBeforeExecute;
		UpdateFieldOperationType updateFieldOperationType = UpdateFieldOperationType.Copy;
		bool shouldCopyBookmarks;
		bool fixLastParagraph;
		bool isMergingTableCell;
		bool suppressParentFieldsUpdate;
		bool allowCopyWholeFieldResult;
		bool suppresFieldsUpdate;
		bool suppressJoinTables;
		List<CopyFieldsOperation> unfinishedCopyOperations;
		#endregion
		public CopySectionOperation(DocumentModelCopyManager copyManager)
			: base(GetSourcePieceTable(copyManager)) {
			this.copyManager = copyManager;
			this.shouldCopyBookmarks = true;
		}
		static PieceTable GetSourcePieceTable(DocumentModelCopyManager copyManager) {
			Guard.ArgumentNotNull(copyManager, "copyManager");
			return copyManager.SourcePieceTable;
		}
		#region Properties
		public DocumentModelCopyManager CopyManager { get { return copyManager; } }
		public DocumentModel TargetModel { get { return copyManager.TargetModel; } }
		public DocumentModel SourceModel { get { return copyManager.SourceModel; } }
		public PieceTable TargetPieceTable { get { return copyManager.TargetPieceTable; } }
		public PieceTable SourcePieceTable { get { return copyManager.SourcePieceTable; } }
		public DocumentModelPosition TargetPosition { get { return copyManager.TargetPosition; } }
		public UpdateFieldOperationType UpdateFieldOperationType { get { return updateFieldOperationType; } set { updateFieldOperationType = value; } }
		protected internal bool AffectsMainPieceTable { get { return Object.ReferenceEquals(SourcePieceTable, SourceModel.MainPieceTable); } }
		public bool ShouldCopyBookmarks { get { return shouldCopyBookmarks; } set { shouldCopyBookmarks = value; } }
		public bool FixLastParagraph { get { return fixLastParagraph; } set { fixLastParagraph = value; } }
		protected internal bool IsMergingTableCell { get { return isMergingTableCell; } set { isMergingTableCell = value; } }
		public bool SuppressParentFieldsUpdate { get { return suppressParentFieldsUpdate; } set { suppressParentFieldsUpdate = value; } }
		public bool SuppressFieldsUpdate { get { return suppresFieldsUpdate; } set { suppresFieldsUpdate = value; } }		
		public bool AllowCopyWholeFieldResult { get { return allowCopyWholeFieldResult; } set { allowCopyWholeFieldResult = value; } }
		public bool SuppressJoinTables { get { return suppressJoinTables; } set { suppressJoinTables = value; } }
		public bool SuppressCopySectionProperties { get; set; }
		public bool RemoveLeadingPageBreak { get; set; } 
		#endregion
		public override bool ExecuteCore(RunInfo runInfo, bool documentLastParagraphSelected) {
			SourceModel.BeginUpdate();
			try {
				CalculateDocumentVariableEventRouter eventRouter = new CalculateDocumentVariableEventRouter(SourceModel);
				TargetModel.BeginUpdate();
				try {
					if (SourceAndTargetModelAreDifferent() && !SourceModel.IntermediateModel)
						TargetModel.CalculateDocumentVariable += eventRouter.OnCalculateDocumentVariable;
					CopyFieldsOperation operation = new CopyFieldsOperation(SourcePieceTable, TargetPieceTable);
					operation.AllowCopyWholeFieldResult = AllowCopyWholeFieldResult;
					operation.SuppressFieldsUpdate = SuppressFieldsUpdate;
					operation.UpdateFieldOperationType = UpdateFieldOperationType;
					operation.RecalculateRunInfo(runInfo);
					DocumentModelPosition positionToInsert = copyManager.TargetPosition.Clone();
					bool result = base.ExecuteCore(runInfo, documentLastParagraphSelected);
					if (copyManager.ParagraphWasInsertedBeforeTable || IsMergingTableCell) {
						positionToInsert.LogPosition++;
						positionToInsert.RunIndex++;
					}
					operation.Execute(runInfo, positionToInsert.RunIndex);
					if (ShouldCopyBookmarks) {
						CopyBookmarksOperation bookmarksCopier = CreateBookmarkCopyOperation();
						bookmarksCopier.CopyBookmarksToTargetModel(runInfo, positionToInsert);
						AfterBookmarkCopied(operation);
					}
					else {
						if (unfinishedCopyOperations == null)
							unfinishedCopyOperations = new List<CopyFieldsOperation>();
						unfinishedCopyOperations.Add(operation);
					}
					if (FixLastParagraph)
						TargetPieceTable.FixLastParagraph();
					if(RemoveLeadingPageBreak)
						TargetPieceTable.Paragraphs.First.PageBreakBefore = false;
					return result;
				}
				finally {
					TargetModel.EndUpdate();
					if (SourceAndTargetModelAreDifferent() && !SourceModel.IntermediateModel)
						TargetModel.CalculateDocumentVariable -= eventRouter.OnCalculateDocumentVariable;
				}
			}
			finally {
				SourceModel.EndUpdate();
			}
		}
		public void AfterBookmarkCopied(CopyFieldsOperation operation) {			
			operation.UpdateCopiedFields();
		}
		public void AfterBookmarkCopied() {
			if (unfinishedCopyOperations == null)
				return;
			int count = unfinishedCopyOperations.Count;
			for (int i = 0; i < count; i++) {
				unfinishedCopyOperations[i].UpdateCopiedFields();
			}
			unfinishedCopyOperations = null;
		}
		bool SourceAndTargetModelAreDifferent() {
			return !Object.ReferenceEquals(CopyManager.SourceModel, copyManager.TargetModel);
		}
		protected virtual CopyBookmarksOperation CreateBookmarkCopyOperation() {
			return new CopyBookmarksOperation(CopyManager);
		}
		protected internal override bool ShouldProcessContentInSameParent(RunInfo info) {
			if (!AffectsMainPieceTable)
				return true; 
			SectionIndex startSectionIndex = SourcePieceTable.LookupSectionIndexByParagraphIndex(info.Start.ParagraphIndex);
			SectionIndex endSectionIndex = SourcePieceTable.LookupSectionIndexByParagraphIndex(info.End.ParagraphIndex);
			Section section = SourceModel.Sections[startSectionIndex];
			Paragraph startParargraph = SourcePieceTable.Paragraphs[section.FirstParagraphIndex];
			Paragraph endParagraph = SourcePieceTable.Paragraphs[section.LastParagraphIndex];
			return startSectionIndex == endSectionIndex &&
				(info.Start.RunIndex != startParargraph.FirstRunIndex || info.End.RunIndex != endParagraph.LastRunIndex);
		}
		protected internal override bool ShouldProcessRunParent(RunInfo info) {
			if (!AffectsMainPieceTable)
				return false; 
			DocumentModelPosition end = info.End;
			SectionIndex sectionIndex = SourcePieceTable.LookupSectionIndexByParagraphIndex(info.Start.ParagraphIndex);
			Section section = SourceModel.Sections[sectionIndex];
			TextRunBase endRun = SourcePieceTable.Runs[end.RunIndex];
			RunIndex lastSectionRun = SourcePieceTable.Paragraphs[section.LastParagraphIndex].LastRunIndex;
			return (end.RunIndex == lastSectionRun && endRun.GetType() == typeof(SectionRun));
		}
		protected internal override void ProcessRunParent(RunInfo info, bool documentLastParagraphSelected) {
			if (AffectsMainPieceTable) {
				SectionIndex sectionIndex = SourcePieceTable.LookupSectionIndexByParagraphIndex(info.Start.ParagraphIndex);
				Section sourceSection = SourceModel.Sections[sectionIndex];
				Section targetSection = CreateSectionCopy(sourceSection);
				RunInfo selectionContentInfo = SourcePieceTable.ObtainAffectedRunInfo(info.Start.LogPosition, info.End.LogPosition - info.Start.LogPosition);
				ProcessContentInsideParent(selectionContentInfo, true, documentLastParagraphSelected);
				copyManager.OnTargetSectionInserted(sourceSection,targetSection);
			}
			else
				ProcessContentInsideParent(info, true, documentLastParagraphSelected);
		}
		protected internal override bool ProcessContentSameParent(RunInfo info, bool allowMergeWithNextParagraph, bool documentLastParagraphSelected) {
			if (!SourceModel.FieldResultModel && TryCopyLastSection(info, documentLastParagraphSelected)) {
				ProcessContentInsideParent(info, true, false);
				return true;
			}
			return base.ProcessContentSameParent(info, allowMergeWithNextParagraph, documentLastParagraphSelected);
		}
		protected internal override void ProcessContentInsideParent(RunInfo info, bool allowMergeWithNextParagraph, bool documentLastParagraphSelected) {
			CopyManager.TableCopyHelper.TargetStartParagraphIndex = copyManager.TargetPosition.ParagraphIndex;
			CopyParagraphOperation paragraphOperation = new CopyParagraphOperation(CopyManager);
			paragraphOperation.IsMergingTableCell = IsMergingTableCell;
			paragraphOperation.ExecuteCore(info, false);
			CopyManager.TableCopyHelper.CopyTables(info);
			if (!SuppressJoinTables)
				JoinTables(info);
		}
		protected internal virtual void JoinTables(RunInfo runInfo) {			
			TableCell sourceFirstCell = SourcePieceTable.Paragraphs[runInfo.NormalizedStart.ParagraphIndex].GetCell();
			if (sourceFirstCell == null)
				return;
			ParagraphIndex targetStartParagraphIndex = CopyManager.TableCopyHelper.TargetStartParagraphIndex;
			if (targetStartParagraphIndex == ParagraphIndex.Zero)
				return;
			TableCell previousTargetCell = TargetPieceTable.Paragraphs[targetStartParagraphIndex - 1].GetCell();
			if (previousTargetCell == null)
				return;
			Table previousTargetTable = previousTargetCell.Table;
			TableCell cell = TargetPieceTable.Paragraphs[previousTargetTable.LastRow.LastCell.EndParagraphIndex + 1].GetCell();
			if (cell == null || previousTargetTable.NestedLevel != cell.Table.NestedLevel)
				return;
			TargetPieceTable.JoinTables(previousTargetTable, cell.Table);
		}
		protected internal override int ProcessHead(RunInfo info, bool documentLastParagraphSelected) {
			SectionIndex startSectionIndex = SourcePieceTable.LookupSectionIndexByParagraphIndex(info.Start.ParagraphIndex);
			SectionIndex endSectionIndex = SourcePieceTable.LookupSectionIndexByParagraphIndex(info.End.ParagraphIndex);
			Section section = SourceModel.Sections[startSectionIndex];
			int sectionCount = endSectionIndex - startSectionIndex + 1;
			if (SourcePieceTable.Paragraphs[section.FirstParagraphIndex].FirstRunIndex == info.Start.RunIndex)
				return sectionCount;
			else {
				ProcessSectionHeadCore(info, startSectionIndex, documentLastParagraphSelected);
				return sectionCount - 1;
			}
		}
		private void ProcessSectionHeadCore(RunInfo info, SectionIndex startSectionIndex, bool documentLastParagraphSelected) {
			Section section = SourceModel.Sections[startSectionIndex];
			Paragraph endParagraph = SourcePieceTable.Paragraphs[section.LastParagraphIndex];
			RunInfo selectionContentInfo = SourcePieceTable.ObtainAffectedRunInfo(info.Start.LogPosition, endParagraph.EndLogPosition - info.Start.LogPosition + 1);
			ProcessRunParent(selectionContentInfo, documentLastParagraphSelected);
		}
		protected internal override bool ProcessMiddle(RunInfo info, int count, bool documentLastParagraphSelected) {
			SectionIndex endSectionIndex = SourcePieceTable.LookupSectionIndexByParagraphIndex(info.End.ParagraphIndex);
			SectionIndex startSectionIndex = endSectionIndex - count + 1;
			RunIndex lastRunIndex = SourcePieceTable.Paragraphs[SourceModel.Sections[endSectionIndex].LastParagraphIndex].LastRunIndex;
			if (info.End.RunIndex == lastRunIndex && lastRunIndex != new RunIndex(SourcePieceTable.Runs.Count - 1)) {
				ProcessSections(startSectionIndex, count);
				return false;
			}
			else {
				ProcessSections(startSectionIndex, count - 1);
				return true;
			}
		}
		protected internal override int ProcessTail(RunInfo info, bool documentLastParagraphSelected) {
			SectionIndex sectionIndex = SourcePieceTable.LookupSectionIndexByParagraphIndex(info.End.ParagraphIndex);
			DocumentLogPosition startLogPosition = SourcePieceTable.Paragraphs[SourceModel.Sections[sectionIndex].FirstParagraphIndex].LogPosition;
			TryCopyLastSection(info, documentLastParagraphSelected);
			RunInfo selectionContentInfo = SourcePieceTable.ObtainAffectedRunInfo(startLogPosition, info.End.LogPosition - startLogPosition + 1);
			ProcessContentInsideParent(selectionContentInfo, true, false);
			return 0;
		}
		bool TryCopyLastSection(RunInfo info, bool documentLastParagraphSelected) {
			if (!AffectsMainPieceTable)
				return false; 
			RunIndex lastRunIndex = new RunIndex(PieceTable.Runs.Count - 1);
			bool isLastParagraphMarkSelected = info.End.RunIndex == lastRunIndex || documentLastParagraphSelected;
			if (!isLastParagraphMarkSelected || !IsTargetSectionLast())
				return false;
			Section sourceSection = SourceModel.Sections.Last;
			Section targetSection = TargetModel.Sections.Last;
			if (!ShouldCopySection(sourceSection, targetSection))
				return false;
			targetSection.CopyFromCore(sourceSection);
			targetSection.Headers.CopyFrom(sourceSection);
			targetSection.Footers.CopyFrom(sourceSection);
			return true;
		}
		bool IsTargetSectionLast() {
			SectionIndex targetSectionIndex = TargetPieceTable.LookupSectionIndexByParagraphIndex(TargetPosition.ParagraphIndex);
			SectionIndex lastSectionIndex = new SectionIndex(TargetModel.Sections.Count - 1);
			return (targetSectionIndex == lastSectionIndex);
		}
		bool ShouldCopySection(Section sourceSection, Section targetSection) {			
			return !SuppressCopySectionProperties && (targetSection.DocumentModel.ModelForExport || IsEmptySection(targetSection));
		}
		private bool IsEmptySection(Section targetSection) {
			if (targetSection.HasNonEmptyHeadersOrFooters)
				return false;
			ParagraphIndex firstParagraphIndex = targetSection.FirstParagraphIndex;
			if (firstParagraphIndex != targetSection.LastParagraphIndex)
				return false;
			return targetSection.DocumentModel.MainPieceTable.Paragraphs[firstParagraphIndex].IsEmpty;
		}
		protected internal virtual void ProcessSections(SectionIndex startSectionIndex, int sectionCount) {
			SectionIndex endSectionIndex = startSectionIndex + sectionCount;
			SectionCollection sections = SourceModel.Sections;
			for (SectionIndex i = startSectionIndex; i < endSectionIndex; i++) {
				Section sourceSection = sections[i];
				Section targetSection = CreateSectionCopy(sourceSection);
				ProcessSectionsCore(i);
				copyManager.OnTargetSectionInserted(sourceSection, targetSection);
			}
		}
		protected internal virtual void ProcessSectionsCore(SectionIndex index) {
			Section section = SourceModel.Sections[index];
			DocumentLogPosition startLogPosition = SourcePieceTable.Paragraphs[section.FirstParagraphIndex].LogPosition;
			DocumentLogPosition endLogPosition = SourcePieceTable.Paragraphs[section.LastParagraphIndex].EndLogPosition;
			if (endLogPosition != startLogPosition) {
				RunInfo selectionContentInfo = SourcePieceTable.ObtainAffectedRunInfo(startLogPosition, endLogPosition - startLogPosition);
				ProcessContentInsideParent(selectionContentInfo, true, false);
			}
		}
		protected internal override void BeforeExecute() {
			DocumentModel.SuppressPerformLayout++;
			CompositeHistoryItem transaction = SourceModel.History.Transaction;
			transactionItemCountBeforeExecute = transaction.Items.Count;
		}
		protected internal override void AfterExecute() {
			CompositeHistoryItem transaction = SourceModel.History.Transaction;
			for (int i = transaction.Items.Count - 1; i >= transactionItemCountBeforeExecute; i--) {
				HistoryItem item = transaction.Items[i];
				if (Object.ReferenceEquals(item.DocumentModelPart, SourcePieceTable)) {
					item.Undo();
					transaction.Items.RemoveAt(i);
				}
			}
			DocumentModel.SuppressPerformLayout--;
		}
		protected internal virtual Section CreateSectionCopy(Section sourceSection) {
			return sourceSection.Copy(CopyManager);
		}
	}
	#endregion
	#region BookmarkCopyOperationBase<T> (abstract class)
	public abstract class BookmarkCopyOperationBase<T> where T : BookmarkBase {
		public abstract List<T> GetEntireBookmarks(PieceTable sourcePieceTable, DocumentLogPosition start, int length);
		public abstract void InsertBookmark(PieceTable targetPieceTable, DocumentLogPosition start, int length, T bookmark, int positionOffset);
	}
	#endregion
	#region BookmarkCopyOperation
	public class BookmarkCopyOperation : BookmarkCopyOperationBase<Bookmark> {
		bool forceUpdateInterval;
		public override List<Bookmark> GetEntireBookmarks(PieceTable sourcePieceTable, DocumentLogPosition start, int length) {
			return sourcePieceTable.GetEntireBookmarks(start, length);
		}
		public bool ForceUpdateInterval { get { return forceUpdateInterval; } set { forceUpdateInterval = value; } }
		public override void InsertBookmark(PieceTable targetPieceTable, DocumentLogPosition start, int length, Bookmark bookmark, int positionOffset) {
			targetPieceTable.CreateBookmarkCore(start, length, bookmark.Name, this.forceUpdateInterval);
		}
	}
	#endregion
	#region RangePermissionCopyOperation
	public class RangePermissionCopyOperation : BookmarkCopyOperationBase<RangePermission> {
		public override List<RangePermission> GetEntireBookmarks(PieceTable sourcePieceTable, DocumentLogPosition start, int length) {
			return sourcePieceTable.GetEntireRangePermissions(start, length);
		}
		public override void InsertBookmark(PieceTable targetPieceTable, DocumentLogPosition start, int length, RangePermission bookmark, int positionOffset) {
			targetPieceTable.ApplyDocumentPermission(start, start + length, bookmark.Properties.Info);
		}
	}
	#endregion
	#region CommentCopyOperation
	public class CommentCopyOperation : BookmarkCopyOperationBase<Comment> {
		public override List<Comment> GetEntireBookmarks(PieceTable sourcePieceTable, DocumentLogPosition start, int length) {
			return sourcePieceTable.GetEntireComments(start, length);
		}
		public override void InsertBookmark(PieceTable targetPieceTable, DocumentLogPosition start, int length, Comment comment, int positionOffset) {
			CommentContentType contentCopy = GetContentCopy(targetPieceTable.DocumentModel, comment.Content);
			targetPieceTable.CreateCommentCore(start, length, comment.Name, comment.Author,  comment.Date, comment.ParentComment, contentCopy);
		}
		CommentContentType GetContentCopy(DocumentModel targetModel, CommentContentType sourceContent) {
			PieceTable sourcePieceTable = sourceContent.PieceTable;
			CommentContentType result = new CommentContentType(targetModel);
			DocumentModelCopyManager copyManager = new DocumentModelCopyManager(sourcePieceTable, result.PieceTable, ParagraphNumerationCopyOptions.CopyAlways);
			CopySectionOperation operation = sourceContent.DocumentModel.CreateCopySectionOperation(copyManager);
			operation.FixLastParagraph = true;
			operation.Execute(sourcePieceTable.DocumentStartLogPosition, sourcePieceTable.DocumentEndLogPosition - sourcePieceTable.DocumentStartLogPosition + 1, false);
			return result;
		}
	}
	#endregion
	public abstract class FieldsOperation {
		protected internal static bool IsEntireFieldAffected(RunInfo runInfo, Field field) {
			return runInfo.NormalizedStart.RunIndex <= field.Code.Start && runInfo.NormalizedEnd.RunIndex >= field.Result.End;
		}
		protected internal static bool IsFieldCodeTextAffectedOnly(RunInfo runInfo, Field field) {
			return runInfo.NormalizedStart.RunIndex > field.Code.Start && runInfo.NormalizedEnd.RunIndex < field.Code.End;
		}
		protected internal static bool IsFieldResultTextAffectedOnly(RunInfo runInfo, Field field) {
			return runInfo.NormalizedStart.RunIndex >= field.Result.Start && runInfo.NormalizedEnd.RunIndex < field.Result.End;
		}
		protected internal static bool IsEntireFieldResultAffectd(RunInfo runInfo, Field field) {
			if (field.IsCodeView)
				return false;
			RunIndex start = field.Result.Start;
			RunIndex end = field.Result.End - 1;
			if (start > end)
				return false;
			return runInfo.NormalizedStart.RunIndex == start && runInfo.NormalizedEnd.RunIndex == end;
		}
		protected internal static void EnsureIntervalContainsField(RunInfo interval, Field field) {
			DocumentModelPosition intervalStart = interval.Start;
			DocumentModelPosition newStart = DocumentModelPosition.FromRunStart(intervalStart.PieceTable, field.FirstRunIndex);
			if (newStart.LogPosition < intervalStart.LogPosition)
				intervalStart.CopyFrom(newStart);
			DocumentModelPosition intervalEnd = interval.End;
			DocumentModelPosition newEnd = DocumentModelPosition.FromRunStart(intervalEnd.PieceTable, field.LastRunIndex);
			if (newEnd.LogPosition > intervalEnd.LogPosition)
				intervalEnd.CopyFrom(newEnd);
		}
	}
	#region CopyFieldsOperation
	public class CopyFieldsOperation : FieldsOperation {
		class UpdateFieldsParameters {
			public UpdateFieldsParameters(IList<Field> fields, IList<Field> fieldsToDelete, Field parent) {
				Fields = fields;
				FieldsToDelete = fieldsToDelete;
				Parent = parent;
			}
			public IList<Field> Fields { get; private set; }
			public IList<Field> FieldsToDelete { get; private set; }
			public Field Parent { get; private set; }
		}
		#region Fields
		readonly PieceTable sourcePieceTable;
		readonly PieceTable targetPieceTable;		
		bool allowCopyWholeFieldResult;
		bool suppressFieldsUpdate;
		UpdateFieldsParameters updateFieldsParameters;
		UpdateFieldOperationType updateFieldOperationType = UpdateFieldOperationType.Copy;
		#endregion
		public CopyFieldsOperation(PieceTable sourcePieceTable, PieceTable targetPieceTable) {
			Guard.ArgumentNotNull(sourcePieceTable, "sourcePieceTable");
			Guard.ArgumentNotNull(targetPieceTable, "targetPieceTable");
			this.sourcePieceTable = sourcePieceTable;
			this.targetPieceTable = targetPieceTable;
		}
		#region Properties
		public DocumentModel TargetModel { get { return targetPieceTable.DocumentModel; } }
		public DocumentModel SourceModel { get { return sourcePieceTable.DocumentModel; } }
		public PieceTable TargetPieceTable { get { return targetPieceTable; } }
		public PieceTable SourcePieceTable { get { return sourcePieceTable; } }
		public UpdateFieldOperationType UpdateFieldOperationType { get { return updateFieldOperationType; } set { updateFieldOperationType = value; } }
		public bool AllowCopyWholeFieldResult { get { return allowCopyWholeFieldResult; } set { allowCopyWholeFieldResult = value; } }
		public bool SuppressFieldsUpdate { get { return suppressFieldsUpdate; } set { suppressFieldsUpdate = value; } }
		#endregion
		public void RecalculateRunInfo(RunInfo runInfo) {
			Field rigthCopiedField = SourcePieceTable.FindFieldByRunIndex(runInfo.End.RunIndex);
			if (rigthCopiedField != null)
				ChangeRunInfo(runInfo, rigthCopiedField);
			Field leftCopiedField = SourcePieceTable.FindFieldByRunIndex(runInfo.Start.RunIndex);
			if (leftCopiedField != null)
				ChangeRunInfo(runInfo, leftCopiedField);
		}
		void ChangeRunInfo(RunInfo runInfo, Field field) {
			if (ShouldExtendInterval(runInfo, field))
				EnsureIntervalContainsField(runInfo, field);
			if (field.Parent != null)
				ChangeRunInfo(runInfo, field.Parent);
		}
		protected virtual bool ShouldExtendInterval(RunInfo runInfo, Field field) {
			if (IsEntireFieldAffected(runInfo, field) || IsFieldCodeTextAffectedOnly(runInfo, field))
				return false;
			if (IsFieldResultTextAffectedOnly(runInfo, field))
				return AllowCopyWholeFieldResult ? false : IsEntireFieldResultAffectd(runInfo, field);
			return true;
		}
		public void Execute(RunInfo runInfo, RunIndex runIndex) {
			updateFieldsParameters = null;
			IList<Field> sourceFields = SourcePieceTable.GetEntireFieldsFromInterval(runInfo.Start.RunIndex, runInfo.End.RunIndex);
			if (sourceFields.Count == 0) {
				return;			   
			}
			int offset = runIndex - runInfo.Start.RunIndex;
			IList<Field> fields = CalculateCopingFields(sourceFields, offset);
			int parentFieldIndex = TargetPieceTable.FindFieldIndexByRunIndex(runIndex);
			CalculateFieldsHierarchy(sourceFields, fields, parentFieldIndex);
			int indexToInsert = GetIndexToInsert(parentFieldIndex, runIndex);
			int count = fields.Count;
			List<Field> fieldsToDelete = new List<Field>();
			Field parent = parentFieldIndex >= 0 ? TargetPieceTable.Fields[parentFieldIndex] : null;
			bool hyperlinksAllowed = TargetModel.DocumentCapabilities.HyperlinksAllowed;
			for (int i = 0; i < count; i++) {
				fields[i].Index = indexToInsert;
				TargetPieceTable.AddFieldToTable(fields[i], indexToInsert);
				HyperlinkInfo hyperlinkInfo;
				if (SourcePieceTable.HyperlinkInfos.TryGetHyperlinkInfo(sourceFields[i].Index, out hyperlinkInfo)) {
					if (hyperlinksAllowed)
						TargetPieceTable.InsertHyperlinkInfo(indexToInsert, hyperlinkInfo.Clone());
					else
						fieldsToDelete.Add(fields[i]);
				}
				indexToInsert++;
			}
			updateFieldsParameters = new UpdateFieldsParameters(fields, fieldsToDelete, parent);
		}
		public void UpdateCopiedFields() {
			if (updateFieldsParameters == null)
				return;
			IList<Field> fieldsToDelete = updateFieldsParameters.FieldsToDelete;
			IList<Field> fields = updateFieldsParameters.Fields;
			for (int i = fieldsToDelete.Count - 1; i >= 0; i--) {
				Field field = fieldsToDelete[i];
				fields.Remove(field);
				TargetPieceTable.DeleteFieldWithoutResult(field);
			}
			if (!SuppressFieldsUpdate)
				TargetPieceTable.FieldUpdater.UpdateFields(fields, updateFieldsParameters.Parent, UpdateFieldOperationType);
		}
		int GetIndexToInsert(int parentFieldIndex, RunIndex runIndex) {
			if (parentFieldIndex < 0)
				return ~parentFieldIndex;
			FieldCollection fields = TargetPieceTable.Fields;
			int result = parentFieldIndex;
			for (int i = parentFieldIndex - 1; i >= 0; i--) {
				if (runIndex > fields[i].LastRunIndex)
					return result;
				result = i;
			}
			return result;
		}
		List<Field> CalculateCopingFields(IList<Field> originFields, int runOffset) {
			List<Field> result = new List<Field>();
			int count = originFields.Count;
			for (int i = 0; i < count; i++) {
				Field newField = originFields[i].CloneToNewPieceTable(this.TargetPieceTable);
				newField.Code.ShiftRunIndex(runOffset);
				newField.Result.ShiftRunIndex(runOffset);
				result.Add(newField);
			}
			return result;
		}
		void CalculateFieldsHierarchy(IList<Field> originFields, IList<Field> newFields, int parentFieldIndex) {
			Field parentField = parentFieldIndex >= 0 ? TargetPieceTable.Fields[parentFieldIndex] : null;
			int count = originFields.Count;
			for (int i = 0; i < count; i++) {
				Field originParent = originFields[i].Parent;
				if (originParent != null && originFields.Contains(originParent)) {
					int parentIndex = originFields.IndexOf(originParent);
					newFields[i].Parent = newFields[parentIndex];
				}
				else
					newFields[i].Parent = parentField;
			}
		}
	}
	#endregion
	public class CopyBookmarksOperation : CopySectionOperation {
		DocumentModelPosition targetBookmarksPosition;
		public CopyBookmarksOperation(DocumentModelCopyManager copyManager)
			: base(copyManager) {
		}
		public DocumentModelPosition TargetBookmarksPosition { get { return targetBookmarksPosition; } set { targetBookmarksPosition = value; } }
		public override bool ExecuteCore(RunInfo info, bool documentLastParagraphSelected) {
			CopyBookmarksToTargetModel(info, TargetBookmarksPosition);
			return true;
		}
		public virtual void CopyBookmarksToTargetModel(RunInfo runInfo, DocumentModelPosition positionToInsert) {
			SourceModel.BeginUpdate();
			try {
				TargetModel.BeginUpdate();
				try {
					CopyBookmarksToTargetModelCore(runInfo, positionToInsert);
				}
				finally {
					TargetModel.EndUpdate();
				}
			}
			finally {
				SourceModel.EndUpdate();
			}
		}
		protected virtual void CopyBookmarksToTargetModelCore(RunInfo runInfo, DocumentModelPosition positionToInsert) {
			bool bookmarksAllowed = TargetModel.DocumentCapabilities.BookmarksAllowed;
			CopyBookmarksToTargetModelCore(runInfo, positionToInsert.LogPosition, bookmarksAllowed, new BookmarkCopyOperation() { ForceUpdateInterval = true });
			CopyBookmarksToTargetModelCore(runInfo, positionToInsert.LogPosition, bookmarksAllowed, new RangePermissionCopyOperation());
			CopyBookmarksToTargetModelCore(runInfo, positionToInsert.LogPosition, bookmarksAllowed, new CommentCopyOperation());
		}
		protected void CopyBookmarksToTargetModelCore<T>(RunInfo runInfo, DocumentLogPosition positionToInsert, bool isAllowed, BookmarkCopyOperationBase<T> bookmarkCopyOperation) where T : BookmarkBase {
			if (!isAllowed)
				return;
			DocumentLogPosition startPos = runInfo.Start.LogPosition;
			int length = runInfo.End.LogPosition - startPos + 1;
			List<T> bookmarks = bookmarkCopyOperation.GetEntireBookmarks(SourcePieceTable, startPos, length);
			if (bookmarks.Count == 0)
				return;
			int offset = positionToInsert - startPos;
			int count = bookmarks.Count;
			for (int i = 0; i < count; i++)
				bookmarkCopyOperation.InsertBookmark(TargetPieceTable, bookmarks[i].Start + offset, bookmarks[i].Length, bookmarks[i], offset);
		}
	}
}
