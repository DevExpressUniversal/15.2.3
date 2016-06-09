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
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.Drawing;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.Model.History;
using DevExpress.Office.NumberConverters;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System;
using Debug = System.Diagnostics.Debug;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Model {
	#region Paragraph
	public class Paragraph : IParagraphPropertiesContainer, IParagraphProperties, IObtainAffectedRangeListener, IHasRelativeIndex<Paragraph> {
		#region Fields
#if UseOldIndicies
		RunIndex firstRunIndex = new RunIndex(-1);
		RunIndex lastRunIndex = new RunIndex(-1);
		DocumentLogPosition logPosition = new DocumentLogPosition(-1);
#endif
		int length = -1;
		int paragraphStyleIndex;
		readonly ParagraphBoxCollection boxCollection;
		readonly PieceTable pieceTable;
		readonly ParagraphProperties paragraphProperties;
		FrameProperties frameProperties;
		readonly TabProperties tabs;
		NumberingListIndex numberingListIndex = NumberingListIndex.ListIndexNotSetted;
		int listLevelIndex = 0;
		int mergedParagraphFormattingCacheIndex = -1;
		#endregion
		public Paragraph(PieceTable pieceTable) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
			this.boxCollection = new ParagraphBoxCollection();
			this.paragraphProperties = new ParagraphProperties(this);
			this.tabs = new TabProperties(this);
		}
		#region Properties
		internal int InnerMergedParagraphFormattingCacheIndex { get { return mergedParagraphFormattingCacheIndex; } set { mergedParagraphFormattingCacheIndex = value; } }
		protected internal int MergedParagraphFormattingCacheIndex {
			get {
				if (mergedParagraphFormattingCacheIndex < 0) {
					mergedParagraphFormattingCacheIndex = DocumentModel.Cache.MergedParagraphFormattingInfoCache.GetItemIndex(GetMergedParagraphProperties().Info);
				}
				return mergedParagraphFormattingCacheIndex;
			}
		}
		internal ParagraphFormattingInfo MergedParagraphFormatting { get { return DocumentModel.Cache.MergedParagraphFormattingInfoCache[MergedParagraphFormattingCacheIndex]; } }
		internal bool IsLast { get { return Index == new ParagraphIndex(PieceTable.Paragraphs.Count - 1); } }
		public ParagraphBoxCollection BoxCollection { get { return boxCollection; } }
		#region Index
		public ParagraphIndex Index {
			get {
				return GetParagraphIndex();
			}
		}
		public bool IsEmpty { get { return Length <= 1; } }
		#endregion
		public ParagraphProperties ParagraphProperties { get { return paragraphProperties; } }
		public FrameProperties FrameProperties { get { return frameProperties; } set { frameProperties = value; } }
		#region FirstRunIndex
		public RunIndex FirstRunIndex {
#if UseOldIndicies
			get { return firstRunIndex; }
			set {
				if (value < new RunIndex(0) || value > lastRunIndex)
					Exceptions.ThrowArgumentException("FirstRunIndex", value);
				if (firstRunIndex == value)
					return;
				firstRunIndex = value;
			}
#else
			get { return GetFirstRunIndex(); }
#endif
		}
		#endregion
		#region LastRunIndex
		public RunIndex LastRunIndex {
#if UseOldIndicies
			get { return lastRunIndex; }
			set {
				if (value < new RunIndex(0) || firstRunIndex > value)
					Exceptions.ThrowArgumentException("LastRunIndex", value);
				if (lastRunIndex == value)
					return;
				lastRunIndex = value;
			}
#else
			get { return GetLastRunIndex(); }
#endif            
		}
		#endregion
		#region ParagraphStyleIndex
		public int ParagraphStyleIndex {
			get { return paragraphStyleIndex; }
			set {
				Guard.ArgumentNonNegative(value, "value");
				if (paragraphStyleIndex == value)
					return;
				DocumentModel.BeginUpdate();
				try {
					ChangeParagraphStyleIndexHistoryItem item = new ChangeParagraphStyleIndexHistoryItem(PieceTable, this.Index, paragraphStyleIndex, value);
					DocumentModel.History.Add(item);
					item.Execute();
				}
				finally {
					DocumentModel.EndUpdate();
				}
			}
		}
		#endregion
		public NumberingListIndex NumberingListIndex {
			get { return GetOwnNumberingListIndex(); }
			set {
				if (NumberingListIndex == value)
					return;
				DocumentModel.BeginUpdate();
				try {
					PieceTable.AddParagraphToList(Index, value, GetListLevelIndex());
				}
				finally {
					DocumentModel.EndUpdate();
				}
			}
		}
		#region LogPosition
		public DocumentLogPosition LogPosition {
#if UseOldIndicies
			get { return logPosition; }
			set {
				if (value < DocumentLogPosition.Zero)
					Exceptions.ThrowArgumentException("LogPosition", value);
				if (logPosition == value)
					return;
				logPosition = value;
			}
#else
			get { return GetLogPosition(); }
#endif
		}
		#endregion
		#region Length
		public int Length {
			get { return length; }
			set {
				if (value <= 0)
					Exceptions.ThrowArgumentException("Length", value);
				if (length == value)
					return;
				length = value;
			}
		}
		#endregion
		#region EndLogPosition
		public DocumentLogPosition EndLogPosition {
			get { return LogPosition + Length - 1; }
		}
		#endregion
		public ParagraphStyle ParagraphStyle { get { return DocumentModel.ParagraphStyles[ParagraphStyleIndex]; } }
		public DocumentModel DocumentModel { get { return pieceTable.DocumentModel; } }
		public PieceTable PieceTable { get { return pieceTable; } }
		public TabProperties Tabs { get { return tabs; } }
		#region Alignment
		public ParagraphAlignment Alignment {
			get { return MergedParagraphFormatting.Alignment; }
			set { ParagraphProperties.Alignment = value; }
		}
		#endregion
		#region LeftIndent
		public int LeftIndent {
			get { return MergedParagraphFormatting.LeftIndent; }
			set { ParagraphProperties.LeftIndent = value; }
		}
		#endregion
		#region RightIndent
		public int RightIndent {
			get { return MergedParagraphFormatting.RightIndent; }
			set { ParagraphProperties.RightIndent = value; }
		}
		#endregion
		#region SpacingBefore
		public int SpacingBefore {
			get { return MergedParagraphFormatting.SpacingBefore; }
			set { ParagraphProperties.SpacingBefore = value; }
		}
		#endregion
		#region SpacingAfter
		public int SpacingAfter {
			get { return MergedParagraphFormatting.SpacingAfter; }
			set { ParagraphProperties.SpacingAfter = value; }
		}
		#endregion
		#region ContextualSpacingBefore
		public int ContextualSpacingBefore {
			get {
				if (Index <= ParagraphIndex.Zero)
					return SpacingBefore;
				if (this.ContextualSpacing && PieceTable.Paragraphs[Index - 1].ParagraphStyleIndex == this.ParagraphStyleIndex)
					return 0;
				else
					return SpacingBefore;
			}
		}
		#endregion
		#region ContextualSpacingAfter
		public int ContextualSpacingAfter {
			get {
				if (Index + 1 >= new ParagraphIndex(PieceTable.Paragraphs.Count))
					return SpacingAfter;
				if (this.ContextualSpacing && PieceTable.Paragraphs[Index + 1].ParagraphStyleIndex == this.ParagraphStyleIndex)
					return 0;
				else
					return SpacingAfter;
			}
		}
		#endregion
		#region LineSpacingType
		public ParagraphLineSpacing LineSpacingType {
			get { return MergedParagraphFormatting.LineSpacingType; }
			set { ParagraphProperties.LineSpacingType = value; }
		}
		#endregion
		#region LineSpacing
		public float LineSpacing {
			get { return MergedParagraphFormatting.LineSpacing; }
			set { ParagraphProperties.LineSpacing = value; }
		}
		#endregion
		#region FirstLineIndentType
		public ParagraphFirstLineIndent FirstLineIndentType {
			get { return MergedParagraphFormatting.FirstLineIndentType; }
			set { ParagraphProperties.FirstLineIndentType = value; }
		}
		#endregion
		#region FirstLineIndent
		public int FirstLineIndent {
			get { return MergedParagraphFormatting.FirstLineIndent; }
			set { ParagraphProperties.FirstLineIndent = value; }
		}
		#endregion
		#region SuppressHyphenation
		public bool SuppressHyphenation {
			get { return MergedParagraphFormatting.SuppressHyphenation; }
			set { ParagraphProperties.SuppressHyphenation = value; }
		}
		#endregion
		#region SuppressLineNumbers
		public bool SuppressLineNumbers {
			get { return MergedParagraphFormatting.SuppressLineNumbers; }
			set { ParagraphProperties.SuppressLineNumbers = value; }
		}
		#endregion
		#region ContextualSpacing
		public bool ContextualSpacing {
			get { return MergedParagraphFormatting.ContextualSpacing; }
			set { ParagraphProperties.ContextualSpacing = value; }
		}
		#endregion
		#region PageBreakBefore
		public bool PageBreakBefore {
			get { return MergedParagraphFormatting.PageBreakBefore; }
			set { ParagraphProperties.PageBreakBefore = value; }
		}
		#endregion
		#region BeforeAutoSpacing
		public bool BeforeAutoSpacing {
			get { return MergedParagraphFormatting.BeforeAutoSpacing; }
			set { ParagraphProperties.BeforeAutoSpacing = value; }
		}
		#endregion
		#region AfterAutoSpacing
		public bool AfterAutoSpacing {
			get { return MergedParagraphFormatting.AfterAutoSpacing; }
			set { ParagraphProperties.AfterAutoSpacing = value; }
		}
		#endregion
		#region KeepWithNext
		public bool KeepWithNext {
			get { return MergedParagraphFormatting.KeepWithNext; }
			set { ParagraphProperties.KeepWithNext = value; }
		}
		#endregion
		#region KeepLinesTogether
		public bool KeepLinesTogether {
			get { return MergedParagraphFormatting.KeepLinesTogether; }
			set { ParagraphProperties.KeepLinesTogether = value; }
		}
		#endregion
		#region WidowOrphanControl
		public bool WidowOrphanControl {
			get { return MergedParagraphFormatting.WidowOrphanControl; }
			set { ParagraphProperties.WidowOrphanControl = value; }
		}
		#endregion
		#region OutlineLevel
		public int OutlineLevel {
			get { return MergedParagraphFormatting.OutlineLevel; }
			set { ParagraphProperties.OutlineLevel = value; }
		}
		#endregion
		#region BackColor
		public Color BackColor {
			get { return MergedParagraphFormatting.BackColor; }
			set { ParagraphProperties.BackColor = value; }
		}
		#endregion
#if THEMES_EDIT
		#region Shading
		public Shading Shading {
			get { return MergedParagraphFormatting.Shading; }
			set { ParagraphProperties.Shading = value; }
		}
		#endregion
#endif
		#region TopBorder
		public BorderInfo TopBorder {
			get { return MergedParagraphFormatting.TopBorder; }
			set { ParagraphProperties.TopBorder = value; }
		}
		#endregion
		#region BottomBorder
		public BorderInfo BottomBorder {
			get { return MergedParagraphFormatting.BottomBorder; }
			set { ParagraphProperties.BottomBorder = value; }
		}
		#endregion
		#region LeftBorder
		public BorderInfo LeftBorder {
			get { return MergedParagraphFormatting.LeftBorder; }
			set { ParagraphProperties.LeftBorder = value; }
		}
		#endregion
		#region RightBorder
		public BorderInfo RightBorder {
			get { return MergedParagraphFormatting.RightBorder; }
			set { ParagraphProperties.RightBorder = value; }
		}
		#endregion
		#endregion
		public TableCell GetCell() {
			return PieceTable.TableCellsManager.GetCell(this);
		}
		public bool IsInCell() {
			return PieceTable.TableCellsManager.IsInCell(this);
		}
		internal void SetLength(int newLength) {
			this.length = newLength;
		}
#if UseOldIndicies
		internal void SetLastRunIndex(RunIndex newLastRunIndex) {
			this.lastRunIndex = newLastRunIndex;
		}
#endif
		internal void SetParagraphStyleIndexCore(int newStyleIndex) {
			this.paragraphStyleIndex = newStyleIndex;
			ResetCachedIndices(ResetFormattingCacheType.All);
#if UseOldIndicies
			if (FirstRunIndex >= RunIndex.Zero)
#else
			if(parent != null)
#endif
				PieceTable.ApplyChangesCore(ParagraphFormattingChangeActionsCalculator.CalculateChangeActions(ParagraphFormattingChangeType.ParagraphStyle), FirstRunIndex, LastRunIndex);
		}
		public TabFormattingInfo GetTabs() {
			return TabFormattingInfo.Merge(Tabs.GetTabs(), ParagraphStyle.GetTabs());
		}
		public TabFormattingInfo GetOwnTabs() {
			return Tabs.GetTabs();
		}
		public void SetOwnTabs(TabFormattingInfo tabs) {
			Tabs.SetTabs(tabs);
		}
		protected internal virtual Paragraph Copy(DocumentModelCopyManager copyManager) {
			Debug.Assert(this.DocumentModel == copyManager.SourceModel);
			PieceTable targetPieceTable = copyManager.TargetPieceTable;
			DocumentModelPosition targetPosition = copyManager.TargetPosition;
			if (targetPieceTable.DocumentModel.DocumentCapabilities.ParagraphsAllowed)
				targetPieceTable.InsertParagraph(targetPosition.LogPosition);
			Paragraph resultParagraph = targetPieceTable.Paragraphs[targetPosition.ParagraphIndex];
			CopyFrom(copyManager.TargetModel, resultParagraph);
			return resultParagraph;
		}
		protected internal virtual void CopyFrom(DocumentModel documentModel, Paragraph resultParagraph) {
			DocumentCapabilitiesOptions options = resultParagraph.DocumentModel.DocumentCapabilities;
			if (options.ParagraphFormattingAllowed)
				resultParagraph.ParagraphProperties.CopyFrom(this.ParagraphProperties.Info);
			if (options.ParagraphTabsAllowed)
				resultParagraph.Tabs.CopyFrom(this.Tabs.Info);
			if (options.ParagraphStyleAllowed)
				resultParagraph.ParagraphStyleIndex = ParagraphStyle.Copy(documentModel);
			if (IsInNonStyleList() || GetOwnNumberingListIndex() == NumberingListIndex.NoNumberingList) {
				CopyNumberingListProperties(resultParagraph);
			}
			else if (resultParagraph.PieceTable.PrecalculatedNumberingListTexts != null && GetOwnNumberingListIndex() == NumberingListIndex.ListIndexNotSetted && ParagraphStyle.GetNumberingListIndex() >= NumberingListIndex.MinValue) {
				resultParagraph.PieceTable.PrecalculatedNumberingListTexts.Add(resultParagraph, GetNumberingListText());
			}
			if (this.FrameProperties != null) {
				resultParagraph.FrameProperties = new FrameProperties(resultParagraph);
				resultParagraph.FrameProperties.CopyFrom(this.FrameProperties.Info);
			}
		}
		protected internal void CopyNumberingListProperties(Paragraph targetParagraph) {
			NumberingOptions numbering = targetParagraph.DocumentModel.DocumentCapabilities.Numbering;
			if (!numbering.BulletedAllowed || !numbering.SimpleAllowed || !numbering.MultiLevelAllowed)
				return;
			if (targetParagraph.IsInNonStyleList())
				targetParagraph.PieceTable.RemoveNumberingFromParagraph(targetParagraph);
			if (this.GetOwnNumberingListIndex() == NumberingListIndex.NoNumberingList) {
				targetParagraph.PieceTable.AddNumberingListToParagraph(targetParagraph, GetOwnNumberingListIndex(), GetListLevelIndex());
				return;
			}
			NumberingListIndex targetNumberingListIndex = DocumentModel.GetNumberingListIndex(targetParagraph.DocumentModel, this.GetOwnNumberingListIndex());
			targetParagraph.PieceTable.AddNumberingListToParagraph(targetParagraph, targetNumberingListIndex, GetListLevelIndex());
			if (targetParagraph.PieceTable.PrecalculatedNumberingListTexts != null)
				targetParagraph.PieceTable.PrecalculatedNumberingListTexts.Add(targetParagraph, GetNumberingListText());
		}
		protected internal virtual void CreateNumberingList(DocumentModel target, int numberingListId) {
			AbstractNumberingListIndex abstractNumberingListIndex = DocumentModel.GetAbstractNumberingListIndex(target, this.GetNumberingListIndex());
			NumberingList numberingList = new NumberingList(target, abstractNumberingListIndex);
			numberingList.CopyFrom(DocumentModel.NumberingLists[this.GetNumberingListIndex()]);
			numberingList.SetId(numberingListId);
			target.AddNumberingListUsingHistory(numberingList);
		}
		protected internal virtual bool ShouldExportNumbering() {
			NumberingOptions options = DocumentModel.DocumentCapabilities.Numbering;
			NumberingListIndex listIndex = GetNumberingListIndex();
			NumberingType numberingType;
			if (listIndex == NumberingListIndex.NoNumberingList) {
				NumberingListIndex styleNumbering = ParagraphStyle.GetNumberingListIndex();
				if (styleNumbering >= NumberingListIndex.MinValue)
					numberingType = NumberingListHelper.GetListType(DocumentModel.NumberingLists[styleNumbering]);
				else
					return false;
			}
			else
				numberingType = NumberingListHelper.GetListType(DocumentModel.NumberingLists[listIndex]);
			switch (numberingType) {
				case NumberingType.Bullet:
					return options.BulletedAllowed;
				case NumberingType.Simple:
					return options.SimpleAllowed;
				default:
					return options.MultiLevelAllowed;
			}
		}
		public MergedCharacterProperties GetMergedCharacterProperties() {
			return GetMergedCharacterProperties(false, null);
		}
		public virtual MergedCharacterProperties GetMergedCharacterProperties(bool useSpecialTableStyle, TableStyle tableStyle) {
			CharacterPropertiesMerger merger = new CharacterPropertiesMerger(ParagraphStyle.GetMergedCharacterProperties());
			TableCell cell = GetCell();
			if (cell != null) {
				merger.Merge(cell.GetMergedCharacterProperties());
				if (!useSpecialTableStyle) {
					merger.Merge(cell.Table.GetMergedCharacterProperties(cell));
				}
				else if(tableStyle != null) {
					merger.Merge(cell.Table.GetMergedCharacterProperties(cell, tableStyle));
				}
			}
			merger.Merge(DocumentModel.DefaultCharacterProperties);
			return merger.MergedProperties;
		}
		int GetTableStyleCharacterPropertiesIndex() {
			TableCell cell = GetCell();
			if (cell != null)
				return cell.Table.TableStyle.CharacterProperties.Index;
			else
				return -1;
		}
		internal bool TryUseMergedCharacterCachedResult(ParagraphMergedCharacterPropertiesCachedResult cachedResult) {
			return TryUseMergedCharacterCachedResultCore(cachedResult, GetCell());
		}
		bool TryUseMergedCharacterCachedResultCore(ParagraphMergedCharacterPropertiesCachedResult cachedResult, TableCell cell) {
			if (cachedResult.ParagraphStyleIndex == this.ParagraphStyleIndex && cachedResult.TableCell == cell)
				return true;
			else {
				cachedResult.ParagraphStyleIndex = this.ParagraphStyleIndex;
				cachedResult.TableCell = cell;
				return false;
			}
		}
		public virtual MergedCharacterProperties GetMergedCharacterProperties(ParagraphMergedCharacterPropertiesCachedResult cachedResult) {			
			if (TryUseMergedCharacterCachedResultCore(cachedResult, GetCell()))
				return cachedResult.MergedCharacterProperties;
			MergedCharacterProperties result = GetMergedCharacterProperties();
			cachedResult.MergedCharacterProperties = result;
			return result;
		}
		public virtual MergedParagraphProperties GetMergedParagraphProperties() {
			ParagraphPropertiesMerger merger = new ParagraphPropertiesMerger(ParagraphProperties);
			merger.Merge(GetParentMergedParagraphProperties());
			return merger.MergedProperties;
		}
		protected internal MergedParagraphProperties GetParentMergedParagraphProperties() {
			return GetParentMergedWithTableStyleParagraphProperties(false, null);
		}
		protected internal MergedParagraphProperties GetParentMergedWithTableStyleParagraphProperties(bool useSpecialTableStyle, TableStyle tableStyle) {
			ParagraphPropertiesMerger merger;
			ParagraphProperties ownListLevelParagraphProperties = GetOwnListLevelParagraphProperties();
			if (ownListLevelParagraphProperties != null) {
				Debug.Assert(IsInNonStyleList());
				merger = new ParagraphPropertiesMerger(ownListLevelParagraphProperties);
				merger.Merge(ParagraphStyle.GetMergedParagraphProperties());
			}
			else {
				merger = new ParagraphPropertiesMerger(ParagraphStyle.GetMergedParagraphProperties());
				ParagraphProperties listLevelParagraphProperties = GetListLevelParagraphProperties();
				if (listLevelParagraphProperties != null)
					merger.Merge(listLevelParagraphProperties);
			}
			TableCell cell = GetCell();
			if (cell != null) {
				merger.Merge(cell.GetMergedParagraphProperties());
				if (!useSpecialTableStyle)
					merger.Merge(cell.Table.GetMergedParagraphProperties(cell));
				else if (tableStyle != null)
					merger.Merge(cell.Table.GetMergedParagraphProperties(tableStyle, cell));
			}
			merger.Merge(DocumentModel.DefaultParagraphProperties);
			return merger.MergedProperties;
		}
		public virtual MergedFrameProperties GetMergedFrameProperties() {
			if (FrameProperties == null)
				return null;
			FramePropertiesMerger merger = new FramePropertiesMerger(FrameProperties);
			merger.Merge(ParagraphStyle.GetMergedFrameProperties());
			return merger.MergedProperties;
		}
		int GetOwnListLevelParagraphPropertiesIndex() {
			ParagraphProperties ownListLevelParagraphProperties = GetOwnListLevelParagraphProperties();
			if (ownListLevelParagraphProperties != null)
				return ownListLevelParagraphProperties.Index;
			else
				return -1;
		}
		int GetTableStyleIndex() {
			TableCell cell = GetCell();
			if (cell != null)
				return cell.Table.StyleIndex;
			else
				return -1;
		}
		internal bool TryUseParentMergedCachedResult(ParagraphMergedParagraphPropertiesCachedResult cachedResult, int tableStyleIndex) {
			return TryUseParentMergedCachedResultCore(cachedResult, GetOwnListLevelParagraphPropertiesIndex(), tableStyleIndex);
		}
		bool TryUseParentMergedCachedResultCore(ParagraphMergedParagraphPropertiesCachedResult cachedResult, int ownListLevelParagraphPropertiesIndex, int tableStyleParagraphPropertiesIndex) {
			bool shouldUseExistingResult =
				cachedResult.ParagraphPropertiesIndex < 0 &&
				cachedResult.ParagraphStyleIndex == this.ParagraphStyleIndex &&
				cachedResult.OwnListLevelParagraphPropertiesIndex == ownListLevelParagraphPropertiesIndex &&
				cachedResult.TableStyleParagraphPropertiesIndex == tableStyleParagraphPropertiesIndex;
			if (shouldUseExistingResult)
				return true;
			cachedResult.ParagraphPropertiesIndex = -1;
			cachedResult.ParagraphStyleIndex = this.ParagraphStyleIndex;
			cachedResult.OwnListLevelParagraphPropertiesIndex = ownListLevelParagraphPropertiesIndex;
			cachedResult.TableStyleParagraphPropertiesIndex = tableStyleParagraphPropertiesIndex;
			return false;
		}
		protected internal virtual ParagraphProperties GetOwnListLevelParagraphProperties() {
			if (!IsInList() || !IsInNonStyleList())
				return null;
			return GetListLevelParagraphProperties();
		}
		protected internal virtual ParagraphProperties GetListLevelParagraphProperties() {
			if (!IsInList())
				return null;
			NumberingList numberingList = DocumentModel.NumberingLists[GetNumberingListIndex()];
			IListLevel level = numberingList.Levels[GetListLevelIndex()];
			return level.ParagraphProperties;
		}
#if UseOldIndicies
		internal void ShiftRunIndex(int offset) {
			RunIndex newFirstRunIndex = firstRunIndex + offset;
			if (newFirstRunIndex < new RunIndex(0))
				Exceptions.ThrowArgumentException("offset", offset);
			this.firstRunIndex = newFirstRunIndex;
			this.lastRunIndex += offset;
		}
#endif
		public void InheritStyleAndFormattingFrom(Paragraph paragraph) {
			if (paragraph == null) {
				Exceptions.ThrowArgumentException("paragraph", paragraph);
				return;
			}
			this.ParagraphStyleIndex = paragraph.ParagraphStyleIndex;
			ParagraphProperties.CopyFrom(paragraph.ParagraphProperties);
			InnerMergedParagraphFormattingCacheIndex = paragraph.InnerMergedParagraphFormattingCacheIndex;
			Tabs.SetTabs(paragraph.GetOwnTabs());
		}
		internal void InheritStyleAndFormattingFromCore(Paragraph paragraph) {
			if (paragraph == null) {
				Exceptions.ThrowArgumentException("paragraph", paragraph);
				return;
			}
			this.ParagraphStyleIndex = paragraph.ParagraphStyleIndex;
			ParagraphProperties.CopyFromCore(paragraph.ParagraphProperties);
			InnerMergedParagraphFormattingCacheIndex = paragraph.InnerMergedParagraphFormattingCacheIndex;
			Tabs.CopyFromCore(paragraph.Tabs);
		}
		#region IParagraphPropertiesContainer Members
		IndexChangedHistoryItemCore<DocumentModelChangeActions> IParagraphPropertiesContainer.CreateParagraphPropertiesChangedHistoryItem() {
			if (Index < new ParagraphIndex(0))
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_InvalidSetParagraphProperties);
			return new ParagraphParagraphPropertiesChangedHistoryItem(PieceTable, Index);
		}
		void IParagraphPropertiesContainer.OnParagraphPropertiesChanged() {
			ResetOwnCachedIndices();
		}
		#endregion
		#region IObtainAffectedRangeListener Members
		public void NotifyObtainAffectedRange(ObtainAffectedRangeEventArgs args) {
			OnParagraphPropertiesObtainAffectedRange(this, args);
		}
		#endregion
		protected internal virtual void OnParagraphPropertiesObtainAffectedRange(object sender, ObtainAffectedRangeEventArgs e) {
			e.Start = FirstRunIndex;
			e.End = LastRunIndex;
		}
		protected internal virtual RunIndex GetRunIndex(TextRunBase textRunBase) {
			RunIndex startIndex = FirstRunIndex;
			RunIndex endIndex = LastRunIndex;
			TextRunCollection runs = PieceTable.Runs;
			for (RunIndex index = endIndex; index >= startIndex; index--) {
				if (runs[index] == textRunBase)
					return index;
			}
			return new RunIndex(-1);
		}
		protected internal virtual bool IsInList() {
			NumberingListIndex numListIndex = GetNumberingListIndex();
			if(numListIndex < NumberingListIndex.MinValue)
				return false;
			NumberingList numList = DocumentModel.NumberingLists[numListIndex];
			return numList.Levels[this.GetListLevelIndex()].ListLevelProperties.Format != NumberingFormat.None; 
		}
		protected internal virtual bool IsInNonStyleList() {
			return numberingListIndex >= NumberingListIndex.MinValue;
		}
		public virtual NumberingListIndex GetOwnNumberingListIndex() {
			return numberingListIndex;
		}
		protected internal virtual string GetNumberingListText() {
			return GetNumberingListText(PieceTable.GetRangeListCounters(this));
		}
		protected internal virtual string GetNumberingListText(int[] counters) {
			if(PieceTable.PrecalculatedNumberingListTexts != null) 
				if(PieceTable.PrecalculatedNumberingListTexts.ContainsKey(this))
					return PieceTable.PrecalculatedNumberingListTexts[this];
			ListLevelCollection<IOverrideListLevel> levels = DocumentModel.NumberingLists[this.GetNumberingListIndex()].Levels;
			string formatstring = levels[this.GetListLevelIndex()].ListLevelProperties.DisplayFormatString;
			return Format(formatstring, counters, levels);
		}
		protected internal virtual string GetListLevelSeparator() {
			ListLevelCollection<IOverrideListLevel> levels = DocumentModel.NumberingLists[this.GetNumberingListIndex()].Levels;
			char separator = levels[this.GetListLevelIndex()].ListLevelProperties.Separator;
			if (separator != '\x0')
				return new string(separator, 1);
			else
				return String.Empty;
		}
		protected internal virtual MergedCharacterProperties GetNumerationCharacterProperties() {
			IListLevel listLevel = DocumentModel.NumberingLists[GetNumberingListIndex()].Levels[GetListLevelIndex()];
				CharacterPropertiesMerger merger = new CharacterPropertiesMerger(listLevel.CharacterProperties);
				MergedCharacterProperties paragraphMarkMergedProperties = PieceTable.Runs[LastRunIndex].GetMergedCharacterProperties();
				paragraphMarkMergedProperties.Options.UseFontUnderlineType = false;
				merger.Merge(paragraphMarkMergedProperties);
				return merger.MergedProperties;
		}
		protected internal virtual int GetNumerationFontCacheIndex() {
			MergedCharacterProperties characterProperties = GetNumerationCharacterProperties();
			CharacterFormattingInfo info = characterProperties.Info;
			return DocumentModel.FontCache.CalcFontIndex(info.FontName, info.DoubleFontSize, info.FontBold, info.FontItalic, info.Script, false, false);
		}
		protected internal virtual FontInfo GetNumerationFontInfo() {
			int fontCachIndex = GetNumerationFontCacheIndex();
			return DocumentModel.FontCache[fontCachIndex];
		}
		internal static string Format(string formatString, int[] args, IReadOnlyIListLevelCollection levels) {
			object[] objArgs = new object[args.Length];
			for (int i = 0; i < args.Length; i++) {
				OrdinalBasedNumberConverter converter = OrdinalBasedNumberConverter.CreateConverter(levels[i].ListLevelProperties.Format, LanguageId.English);
				objArgs[i] = converter.ConvertNumber(args[i]);
			}
			try {
				return String.Format(formatString, objArgs);
			}
			catch {
				try {
					return (string)objArgs[0];
				}
				catch {
					return String.Empty;
				}
			}
		}
		protected internal virtual NumberingListIndex GetNumberingListIndex() {
			if (numberingListIndex >= NumberingListIndex.MinValue || numberingListIndex == NumberingListIndex.NoNumberingList)
				return numberingListIndex;
			else
				return ParagraphStyle.GetNumberingListIndex();
		}
		protected internal virtual int GetListLevelIndex() {
			if (GetOwnNumberingListIndex() >= NumberingListIndex.MinValue)
				return listLevelIndex;
			else {
				return ParagraphStyle.GetListLevelIndex();
			}
		}
		public virtual int GetOwnListLevelIndex() {
			return listLevelIndex;
		}
		protected internal virtual void SetNumberingListIndex(NumberingListIndex numberingListIndex) {
			this.numberingListIndex = numberingListIndex;
			ResetCachedIndices(ResetFormattingCacheType.All);
#if UseOldIndicies
			if (FirstRunIndex >= RunIndex.Zero)
#else
			if(parent != null)
#endif
				PieceTable.ApplyChangesCore(ParagraphFormattingChangeActionsCalculator.CalculateChangeActions(ParagraphFormattingChangeType.NumberingListIndex), FirstRunIndex, RunIndex.MaxValue);
		}
		protected internal virtual void SetListLevelIndex(int listLevelIndex) {
			this.listLevelIndex = listLevelIndex;
			ResetCachedIndices(ResetFormattingCacheType.All);
			if (DocumentModel.IsUpdateLocked && FirstRunIndex >= RunIndex.Zero)
				PieceTable.ApplyChangesCore(ParagraphFormattingChangeActionsCalculator.CalculateChangeActions(ParagraphFormattingChangeType.NumberingListIndex), FirstRunIndex, RunIndex.MaxValue);
		}
		protected internal virtual void ResetNumberingListIndex(NumberingListIndex index) {
			SetNumberingListIndex(index);
		}
		protected internal virtual void ResetListLevelIndex() {
			SetListLevelIndex(-1);
		}
		protected virtual void ResetOwnCachedIndices() {
			InnerMergedParagraphFormattingCacheIndex = -1;
		}
		protected internal virtual void ResetCachedIndices(ResetFormattingCacheType resetFromattingCacheType) {
			if((resetFromattingCacheType & ResetFormattingCacheType.Paragraph) != 0)
				ResetOwnCachedIndices();
			BoxCollection.InvalidateBoxes();
#if UseOldIndicies
			if (FirstRunIndex < RunIndex.MinValue)
				return;
#else
			if (parent == null)
				return;
#endif
			if ((resetFromattingCacheType & ResetFormattingCacheType.Character) != 0) {
				RunIndex lastRunIndex = LastRunIndex;
				for (RunIndex index = FirstRunIndex; index <= lastRunIndex; index++)
					PieceTable.Runs[index].ResetCachedIndices(resetFromattingCacheType);
			}
		}   
		public virtual bool IsHidden() {
			TextRunCollection runs = PieceTable.Runs;
			RunIndex lastRunIndex = LastRunIndex;
			for (RunIndex i = FirstRunIndex; i <= lastRunIndex; i++) {
				if (!runs[i].Hidden)
					return false;
			}
			return true;
		}
		public int CalculateHashCode() {
			int formattingHash = this.ParagraphProperties.Index;
			int styleHash = this.ParagraphStyleIndex << 4;
			int listHash = ((IConvertToInt<NumberingListIndex>)this.numberingListIndex).ToInt() << 8;
			int listLevelHash = this.listLevelIndex << 12;
			int tabsHash = this.Tabs.Index << 16;
			int tableHash = 0;
			TableCell cell = GetCell();
			if (cell != null)
				tableHash = cell.Table.Index << 20;
			int result = formattingHash ^ styleHash ^ listHash ^ listLevelHash ^ tabsHash ^ tableHash;
			int runFormattingHash = 0;
			StringBuilder text = new StringBuilder(Length);
			for (RunIndex i = FirstRunIndex; i < LastRunIndex; i++) {
				text.Append(PieceTable.GetRunPlainText(i));
				TextRunBase run = PieceTable.Runs[i];
				runFormattingHash ^= ((run.CharacterStyleIndex << 28) ^ (run.CharacterProperties.Index << 12) ^ (i - FirstRunIndex));
			}
			result ^= (runFormattingHash ^ text.ToString().GetHashCode());
			return result;
		}
		protected internal virtual void ResetRunsCharacterFormatting() {
			MergedCharacterProperties styleProperties = DocumentModel.ParagraphStyles[paragraphStyleIndex].GetMergedCharacterProperties();
			RunIndex startIndex = FirstRunIndex;
			RunIndex endIndex = LastRunIndex;
			TextRunCollection runs = PieceTable.Runs;
			for (RunIndex index = startIndex; index <= endIndex; index++) {
				TextRunBase run = runs[index];
				CharacterProperties properties = run.CharacterProperties;
				ResetCharacterFormattingProperties(properties, styleProperties.Options.Clone());
			}
		}
		protected internal virtual void ResetCharacterFormattingProperties(CharacterProperties properties, CharacterFormattingOptions styleFormattingOptions) {
			if (properties.UseAllCaps && styleFormattingOptions.UseAllCaps)
				properties.ResetUse(CharacterFormattingOptions.Mask.UseAllCaps);
			if (properties.UseHidden && styleFormattingOptions.UseHidden)
				properties.ResetUse(CharacterFormattingOptions.Mask.UseHidden);
			if (properties.UseFontBold && styleFormattingOptions.UseFontBold)
				properties.ResetUse(CharacterFormattingOptions.Mask.UseFontBold);
			if (properties.UseFontItalic && styleFormattingOptions.UseFontItalic)
				properties.ResetUse(CharacterFormattingOptions.Mask.UseFontItalic);
			if (properties.UseFontName && styleFormattingOptions.UseFontName)
				properties.ResetUse(CharacterFormattingOptions.Mask.UseFontName);
			if (properties.UseDoubleFontSize && styleFormattingOptions.UseDoubleFontSize)
				properties.ResetUse(CharacterFormattingOptions.Mask.UseDoubleFontSize);
			if (properties.UseFontUnderlineType && styleFormattingOptions.UseFontUnderlineType)
				properties.ResetUse(CharacterFormattingOptions.Mask.UseFontUnderlineType);
			if (properties.UseForeColor && styleFormattingOptions.UseForeColor)
				properties.ResetUse(CharacterFormattingOptions.Mask.UseForeColor);
			if (properties.UseBackColor && styleFormattingOptions.UseBackColor)
				properties.ResetUse(CharacterFormattingOptions.Mask.UseBackColor);
			if (properties.UseScript && styleFormattingOptions.UseScript)
				properties.ResetUse(CharacterFormattingOptions.Mask.UseScript);
			if (properties.UseStrikeoutColor && styleFormattingOptions.UseStrikeoutColor)
				properties.ResetUse(CharacterFormattingOptions.Mask.UseStrikeoutColor);
			if (properties.UseFontStrikeoutType && styleFormattingOptions.UseFontStrikeoutType)
				properties.ResetUse(CharacterFormattingOptions.Mask.UseFontStrikeoutType);
			if (properties.UseStrikeoutWordsOnly && styleFormattingOptions.UseStrikeoutWordsOnly)
				properties.ResetUse(CharacterFormattingOptions.Mask.UseStrikeoutWordsOnly);
			if (properties.UseUnderlineColor && styleFormattingOptions.UseUnderlineColor)
				properties.ResetUse(CharacterFormattingOptions.Mask.UseUnderlineColor);
			if (properties.UseUnderlineWordsOnly && styleFormattingOptions.UseUnderlineWordsOnly)
				properties.ResetUse(CharacterFormattingOptions.Mask.UseUnderlineWordsOnly);
		}
		int relativeIndex;
		IndexedTreeNodeLeafLevel<Paragraph> parent;
		IndexedTreeNodeLeafLevel<Paragraph> IHasRelativeIndex<Paragraph>.Parent { get { return parent; } set { parent = value; } }
		void IHasRelativeIndex<Paragraph>.ShiftRelativeIndex(int delta) {
			relativeIndex += delta;
		}
		void IHasRelativeIndex<Paragraph>.ShiftRelativePosition(int deltaLength, int deltaFirstRunIndex, int deltaLastRunIndex) {
			relativeFirstRunIndex += deltaFirstRunIndex;
			relativeLastRunIndex += deltaLastRunIndex;
			relativeLogPosition += deltaLength;
			if (relativeFirstRunIndex < 0)
				parent.ShiftRelativeFirstRunIndex(relativeFirstRunIndex);
#if DEBUGTEST || DEBUG
			if (relativeFirstRunIndex < 0)
				Exceptions.ThrowInternalException();
#endif        
		}
		void IHasRelativeIndex<Paragraph>.SetRelativeIndex(int relativeIndex) {
			this.relativeIndex = relativeIndex;
		}
		ParagraphIndex GetParagraphIndex() {
			if (parent == null)
				Exceptions.ThrowInternalException();
			int result = parent.FirstRelativeIndex + relativeIndex;
			IndexedTreeNodeMiddleLevel<Paragraph> parentLevel = parent.Parent;
			while (parentLevel != null) {
				result += parentLevel.FirstRelativeIndex;
				parentLevel = parentLevel.Parent;
			}
			return new ParagraphIndex(result);
		}
		int relativeFirstRunIndex;
		int IHasRelativeIndex<Paragraph>.GetRelativeFirstRunIndex() {
			return relativeFirstRunIndex;
		}
		internal RunIndex GetFirstRunIndex() {
			if (parent == null)
				Exceptions.ThrowInternalException();
			int result = parent.RelativeFirstRunIndex + relativeFirstRunIndex;
			IndexedTreeNodeMiddleLevel<Paragraph> parentLevel = parent.Parent;
			while (parentLevel != null) {
				result += parentLevel.RelativeFirstRunIndex;
				parentLevel = parentLevel.Parent;
			}
			return new RunIndex(result);
		}
		internal void SetRelativeFirstRunIndex(RunIndex runIndex) {
			if (parent == null)
				Exceptions.ThrowInternalException();
			if(runIndex < RunIndex.Zero)
				Exceptions.ThrowArgumentException("runIndex", runIndex);			
			relativeFirstRunIndex = ((IConvertToInt<RunIndex>)runIndex).ToInt() - parent.RelativeFirstRunIndex;
			IndexedTreeNodeMiddleLevel<Paragraph> parentLevel = parent.Parent;
			while (parentLevel != null) {
				relativeFirstRunIndex -= parentLevel.RelativeFirstRunIndex;
				parentLevel = parentLevel.Parent;
			}
			parent.EnsureValidRelativeFirstRunIndex(this, relativeFirstRunIndex);
#if DEBUGTEST || DEBUG
			if (relativeFirstRunIndex < 0)
				Exceptions.ThrowInternalException();
#endif
		}
		int relativeLastRunIndex;
		int IHasRelativeIndex<Paragraph>.GetRelativeLastRunIndex() {			
			return relativeLastRunIndex;
		}
		internal RunIndex GetLastRunIndex() {
			if (parent == null)
				Exceptions.ThrowInternalException();
			int result = parent.RelativeLastRunIndex + relativeLastRunIndex;
			IndexedTreeNodeMiddleLevel<Paragraph> parentLevel = parent.Parent;
			while (parentLevel != null) {
				result += parentLevel.RelativeLastRunIndex;
				parentLevel = parentLevel.Parent;
			}
			return new RunIndex(result);
		}
		internal void SetRelativeLastRunIndexWithoutCheck(RunIndex runIndex) {
			if (parent == null)
				Exceptions.ThrowInternalException();
			relativeLastRunIndex = ((IConvertToInt<RunIndex>)runIndex).ToInt() - parent.RelativeLastRunIndex;
			IndexedTreeNodeMiddleLevel<Paragraph> parentLevel = parent.Parent;
			while (parentLevel != null) {
				relativeLastRunIndex -= parentLevel.RelativeLastRunIndex;
				parentLevel = parentLevel.Parent;
			}
		}
		internal void SetRelativeLastRunIndex(RunIndex runIndex) {
			if (parent == null)
				Exceptions.ThrowInternalException();
			if (runIndex < RunIndex.Zero)
				Exceptions.ThrowArgumentException("runIndex", runIndex);
			relativeLastRunIndex = ((IConvertToInt<RunIndex>)runIndex).ToInt() - parent.RelativeLastRunIndex;
			IndexedTreeNodeMiddleLevel<Paragraph> parentLevel = parent.Parent;
			while (parentLevel != null) {
				relativeLastRunIndex -= parentLevel.RelativeLastRunIndex;
				parentLevel = parentLevel.Parent;
			}
		}
		int relativeLogPosition;
		int IHasRelativeIndex<Paragraph>.GetRelativeLogPosition() {
			return relativeLogPosition;
		}
		internal DocumentLogPosition GetLogPosition() {
			if (parent == null)
				return new DocumentLogPosition(-1);
			int result = parent.RelativeLogPosition + relativeLogPosition;
			IndexedTreeNodeMiddleLevel<Paragraph> parentLevel = parent.Parent;
			while (parentLevel != null) {
				result += parentLevel.RelativeLogPosition;
				parentLevel = parentLevel.Parent;
			}
			return new DocumentLogPosition(result);
		}
		internal void SetRelativeLogPosition(DocumentLogPosition logPosition) {
			if (parent == null)
				Exceptions.ThrowInternalException();
			if (logPosition < DocumentLogPosition.Zero)
				Exceptions.ThrowArgumentException("logPosition", logPosition);
			relativeLogPosition = ((IConvertToInt<DocumentLogPosition>)logPosition).ToInt() - parent.RelativeLogPosition;
			IndexedTreeNodeMiddleLevel<Paragraph> parentLevel = parent.Parent;
			while (parentLevel != null) {
				relativeLogPosition -= parentLevel.RelativeLogPosition;
				parentLevel = parentLevel.Parent;
			}
			parent.EnsureValidRelativeLogPosition(this, relativeLogPosition);
		}
		internal void AfterRemove(RunIndex firstRunIndex, RunIndex lastRunIndex, DocumentLogPosition logPosition) {
			this.relativeFirstRunIndex = ((IConvertToInt<RunIndex>)firstRunIndex).ToInt();
			this.relativeLastRunIndex = ((IConvertToInt<RunIndex>)lastRunIndex).ToInt();
			this.relativeLogPosition = ((IConvertToInt<DocumentLogPosition>)logPosition).ToInt();
		}
		internal void AfterUndoRemove() {
			SetRelativeFirstRunIndex(new RunIndex(relativeFirstRunIndex));
			SetRelativeLastRunIndexWithoutCheck(new RunIndex(relativeLastRunIndex));
			SetRelativeLogPosition(new DocumentLogPosition(relativeLogPosition));
		}
		internal void ShiftLogPosition(int deltaLength) {
			((IHasRelativeIndex<Paragraph>)this).ShiftRelativePosition(deltaLength, 0, 0);
			parent.EnsureValidRelativeLogPosition(this, relativeLogPosition);
		}
		public AbstractNumberingList GetAbstractNumberingList() {
			NumberingListIndex listIndex = GetNumberingListIndex();
			if (listIndex < NumberingListIndex.MinValue)
				return null;
			return DocumentModel.NumberingLists[listIndex].AbstractNumberingList;
		}
		public AbstractNumberingListIndex GetAbstractNumberingListIndex() {
			NumberingListIndex listIndex = GetNumberingListIndex();
			if (listIndex < NumberingListIndex.MinValue)
				return new AbstractNumberingListIndex(-1);
			return DocumentModel.NumberingLists[listIndex].AbstractNumberingListIndex;
		}
	}
	#endregion
	#region ParagraphCollection
	public class ParagraphCollection : ParagraphIndexedTree {
		public void OnBeginMultipleRunSplit() {			
		}
		public void OnEndMultipleRunSplit() {
			if (Root != null)
				Root.OnEndMultipleRunSplit();
		}
	}
	#endregion
	#region ParagraphList
	public class ParagraphList : List<Paragraph, ParagraphIndex> {
	}
	#endregion
	#region ParagraphIndex
	[Serializable, StructLayout(LayoutKind.Sequential), ComVisible(false)]
	public struct ParagraphIndex : IConvertToInt<ParagraphIndex>, IComparable<ParagraphIndex> {
		readonly int m_value;
		public static ParagraphIndex MinValue = new ParagraphIndex(0);
		public static readonly ParagraphIndex Zero = new ParagraphIndex(0);
		public static ParagraphIndex MaxValue = new ParagraphIndex(int.MaxValue);
		[DebuggerStepThrough]
		public ParagraphIndex(int value) {
			m_value = value;
		}
		public override bool Equals(object obj) {
			return ((obj is ParagraphIndex) && (this.m_value == ((ParagraphIndex)obj).m_value));
		}
		public override int GetHashCode() {
			return m_value.GetHashCode();
		}
		public override string ToString() {
			return m_value.ToString();
		}
		public static ParagraphIndex operator +(ParagraphIndex index, int value) {
			return new ParagraphIndex(index.m_value + value);
		}
		public static int operator -(ParagraphIndex index1, ParagraphIndex index2) {
			return index1.m_value - index2.m_value;
		}
		public static ParagraphIndex operator -(ParagraphIndex index, int value) {
			return new ParagraphIndex(index.m_value - value);
		}
		public static ParagraphIndex operator ++(ParagraphIndex index) {
			return new ParagraphIndex(index.m_value + 1);
		}
		public static ParagraphIndex operator --(ParagraphIndex index) {
			return new ParagraphIndex(index.m_value - 1);
		}
		public static bool operator <(ParagraphIndex index1, ParagraphIndex index2) {
			return index1.m_value < index2.m_value;
		}
		public static bool operator <=(ParagraphIndex index1, ParagraphIndex index2) {
			return index1.m_value <= index2.m_value;
		}
		public static bool operator >(ParagraphIndex index1, ParagraphIndex index2) {
			return index1.m_value > index2.m_value;
		}
		public static bool operator >=(ParagraphIndex index1, ParagraphIndex index2) {
			return index1.m_value >= index2.m_value;
		}
		public static bool operator ==(ParagraphIndex index1, ParagraphIndex index2) {
			return index1.m_value == index2.m_value;
		}
		public static bool operator !=(ParagraphIndex index1, ParagraphIndex index2) {
			return index1.m_value != index2.m_value;
		}
		#region IConvertToInt<ParagraphIndex> Members
		[DebuggerStepThrough]
		int IConvertToInt<ParagraphIndex>.ToInt() {
			return m_value;
		}
		[DebuggerStepThrough]
		ParagraphIndex IConvertToInt<ParagraphIndex>.FromInt(int value) {
			return new ParagraphIndex(value);
		}
		#endregion
		#region IComparable<ParagraphIndex> Members
		public int CompareTo(ParagraphIndex other) {
			if (m_value < other.m_value)
				return -1;
			if (m_value > other.m_value)
				return 1;
			else
				return 0;
		}
		#endregion
	}
	#endregion
	#region ParagraphAndLogPositionComparable
	public class ParagraphAndLogPositionComparable : IComparable<Paragraph> {
		readonly DocumentLogPosition logPosition;
		public ParagraphAndLogPositionComparable(DocumentLogPosition logPosition) {
			this.logPosition = logPosition;
		}
		public DocumentLogPosition LogPosition { get { return logPosition; } }
		#region IComparable<Paragraph> Members
		public int CompareTo(Paragraph paragraph) {
			DocumentLogPosition paragraphLogPosition = paragraph.LogPosition;
			if (logPosition < paragraphLogPosition)
				return 1;
			else if (logPosition > paragraphLogPosition) {
				if (logPosition < paragraphLogPosition + paragraph.Length)
					return 0;
				else
					return -1;
			}
			else
				return 0;
		}
		#endregion
	}
	#endregion
}
