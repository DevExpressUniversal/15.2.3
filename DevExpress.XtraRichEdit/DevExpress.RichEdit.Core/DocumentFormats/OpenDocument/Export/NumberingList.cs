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
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Export.OpenDocument {
	public enum NumberingListTagType { ListOpen, ListClose, ListItemOpen, ListItemClose };
	#region NumberingListTag (abstract class)
	public abstract class NumberingListTag {
		readonly int listIndex;
		readonly int levelIndex;
		protected NumberingListTag(int listIndex, int levelIndex) {
			this.listIndex = listIndex;
			this.levelIndex = levelIndex;
		}
		public abstract NumberingListTagType Type { get; }
		public int ListIndex { get { return listIndex; } }
		public int LevelIndex { get { return levelIndex; } }
		public abstract void Export(ExportHelper writeHelper);
		public bool HasSameList(NumberingListTag tag) {
			return tag.ListIndex == ListIndex && tag.LevelIndex == LevelIndex;
		}
	}
	#endregion
	#region NumberingListOpenTag
	public class NumberingListOpenTag : NumberingListTag {
		bool rootList;
		string id;
		string parentId;
		public NumberingListOpenTag(int listIndex, int levelIndex)
			: base(listIndex, levelIndex) {
		}
		public override NumberingListTagType Type { get { return NumberingListTagType.ListOpen; } }
		public bool RootList { get { return rootList; } set { rootList = value; } }
		public string Id { get { return id; } set { id = value; } }
		public string ParentId { get { return parentId; } set { parentId = value; } }
		public override void Export(ExportHelper writeHelper) {
			writeHelper.WriteListStart();
				string styleName = NameResolver.CalculateNumberingListStyleName(ListIndex);
				writeHelper.WriteListAttributes(styleName, Id, ParentId);
			writeHelper.WriteTextStringAttribute("continue-numbering", "true"); 
		}
	}
	#endregion
	#region NumberingListCloseTag
	public class NumberingListCloseTag : NumberingListTag {
		public NumberingListCloseTag(int listIndex, int levelIndex)
			: base(listIndex, levelIndex) {
		}
		public override NumberingListTagType Type { get { return NumberingListTagType.ListClose; } }
		public override void Export(ExportHelper writeHelper) {
			writeHelper.WriteEndElement();
		}
	}
	#endregion
	#region NumberingListItemOpenTag
	public class NumberingListItemOpenTag : NumberingListTag {
		public NumberingListItemOpenTag(int listIndex, int levelIndex)
			: base(listIndex, levelIndex) {
		}
		public override NumberingListTagType Type { get { return NumberingListTagType.ListItemOpen; } }
		public override void Export(ExportHelper writeHelper) {
			writeHelper.WriteListItemStart();
		}
	}
	#endregion
	#region NumberingListItemCloseTag
	public class NumberingListItemCloseTag : NumberingListTag {
		public NumberingListItemCloseTag(int listIndex, int levelIndex)
			: base(listIndex, levelIndex) {
		}
		public override NumberingListTagType Type { get { return NumberingListTagType.ListItemClose; } }
		public override void Export(ExportHelper writeHelper) {
			writeHelper.WriteEndElement();
		}
	}
	#endregion
	#region NumberingListItem
	public class NumberingListItem {
		Paragraph paragraph;
		List<NumberingListTag> tagsToOpen;
		List<NumberingListTag> tagsToClose;
		bool isStartOfList;
		string listId;
		string parentListId;
		public NumberingListItem(Paragraph paragraph) {
			Guard.ArgumentNotNull(paragraph, "paragraph");
			this.paragraph = paragraph;
			tagsToOpen = new List<NumberingListTag>();
			tagsToClose = new List<NumberingListTag>();
		}
		public Paragraph Paragraph { get { return paragraph; } }
		public List<NumberingListTag> TagsToOpen { get { return tagsToOpen; } }
		public List<NumberingListTag> TagsToClose { get { return tagsToClose; } }
		public bool IsStartOfList { get { return isStartOfList; } set { isStartOfList = value; } }
		public string ListId { get { return listId; } set { listId = value; } }
		public string ParentListId { get { return parentListId; } set { parentListId = value; } }
		public virtual void Export(ExportHelper writeHelper) {
			if (IsStartOfList)
				UpdateRootItemTag();
			ExportTags(TagsToClose, writeHelper);
			ExportTags(TagsToOpen, writeHelper);
		}
		protected internal void UpdateRootItemTag() {
			int count = TagsToOpen.Count;
			for (int i = 0; i < count; i++) {
				NumberingListTag openList = TagsToOpen[i];
				if (openList.Type == NumberingListTagType.ListOpen) {
					NumberingListOpenTag listTag = (NumberingListOpenTag)openList;
					listTag.RootList = true;
					listTag.Id = ListId;
					listTag.ParentId = ParentListId;
					break;
				}
			}
		}
		protected internal virtual void ExportTags(List<NumberingListTag> tags, ExportHelper writeHelper) {
			for (int i = 0; i < tags.Count; i++) {
				tags[i].Export(writeHelper);
			}
		}
	}
	#endregion
	#region NumberingListRegistrator
	public class NumberingListRegistrator {
		Dictionary<Paragraph, NumberingListItem> listItems;
		Stack<NumberingListTag> listTagsStack;
		Dictionary<int, string> activeLists;
		Dictionary<Section, NumberingListItem> finalizeListItems;
		Dictionary<PieceTable, NumberingListItem> finalizeHeaderFooterListItems;
		int currentListIndex = -1;
		bool needCloseRootListOnNextProcessListItem;
		public NumberingListRegistrator() {
			listItems = new Dictionary<Paragraph, NumberingListItem>();
			listTagsStack = new Stack<NumberingListTag>();
			activeLists = new Dictionary<int, string>();
			finalizeListItems = new Dictionary<Section, NumberingListItem>();
			finalizeHeaderFooterListItems = new Dictionary<PieceTable, NumberingListItem>();
		}
		public Dictionary<Paragraph, NumberingListItem> ListItems { get { return listItems; } }
		public Stack<NumberingListTag> ListTagsStack { get { return listTagsStack; } }
		public Dictionary<Section, NumberingListItem> FinalizeSectionListItems { get { return finalizeListItems; } }
		public Dictionary<PieceTable, NumberingListItem> FinalizeHeaderFooterListItems { get { return finalizeHeaderFooterListItems; } }
		protected Dictionary<int, string> ActiveLists { get { return activeLists; } }
		public bool ReadyForRegister { get { return true; } } 
		internal bool NeedCloseRootListOnNextProcessListItem { get { return needCloseRootListOnNextProcessListItem; } set { needCloseRootListOnNextProcessListItem = value; } }
		public void RegisterParagraph(Paragraph paragraph) {
			int listIndex = GetParagraphNumberingListIndex(paragraph);
			if (listIndex < 0 && ListItems.Count == 0)
				return;
			NumberingListItem listItem = CreateListItem(paragraph);
			RegisterListItem(listItem);
			listItem.IsStartOfList = CalculateIsStartOfList(listIndex, CheckParagraphIsFirstInTableCell(paragraph));
			if (listItem.IsStartOfList) {
				if (!IsListActive(listIndex)) {
					RegisterActiveListId(listIndex, listItem);
					listItem.ListId = GetActiveListId(listIndex);
				} else {
					listItem.ListId =  GenerateListName(listItem);
					listItem.ParentListId = GetActiveListId(listIndex);
				}
			}
			ProcessListItem(listItem);
		}
		bool CheckParagraphIsFirstInTableCell(Paragraph paragraph) {
			bool result = paragraph.GetCell() != null && paragraph.GetCell().StartParagraphIndex == paragraph.Index;
			return result;
		}
		public void RegisterParagraphAfterClosedList(Section section, Paragraph paragraph) {
			int listIndex = GetParagraphNumberingListIndex(paragraph);
			if (currentListIndex != listIndex) {
				NumberingListItem listItem = CreateListItem(paragraph);
				RegisterListItem(listItem);
				currentListIndex = listIndex;
				this.finalizeListItems[section] = null;
				CloseExistingList(listItem);
			}
		}
		public virtual void RegisterTableCellEnd() {
			NeedCloseRootListOnNextProcessListItem = true;
		}
		protected internal string GetActiveListId(int listIndex) {
			return ActiveLists[listIndex];
		}
		protected internal virtual bool IsListActive(int listIndex) { 
			return ActiveLists.ContainsKey(listIndex);
		}
		protected internal virtual void RegisterActiveListId(int listIndex, NumberingListItem listItem) {
			if (!IsListActive(listIndex))
				ActiveLists[listIndex] = GenerateListName(listItem);
		}
		private string GenerateListName(NumberingListItem listItem) {
			return NameResolver.CalculateNumberingListId(listItem.GetHashCode());
		}
		protected internal virtual NumberingListTag CreateNumberingListTag(NumberingListTagType type, int listIndex, int levelIndex) {
			switch (type) {
				case NumberingListTagType.ListOpen:
					return new NumberingListOpenTag(listIndex, levelIndex);
				case NumberingListTagType.ListClose:
					return new NumberingListCloseTag(listIndex, levelIndex);
				case NumberingListTagType.ListItemOpen:
					return new NumberingListItemOpenTag(listIndex, levelIndex);
				case NumberingListTagType.ListItemClose:
					return new NumberingListItemCloseTag(listIndex, levelIndex);
			}
			return null;
		}
		protected internal virtual bool CalculateIsStartOfList(int listIndex, bool isFirstParagraphInTableCell) {
			if (isFirstParagraphInTableCell) {
				currentListIndex = listIndex;
				return true;
			}
			if (currentListIndex != listIndex) {
				currentListIndex = listIndex;
				if (isFirstParagraphInTableCell)
					return true;
				return currentListIndex >= 0;
			}
			return false;
		}
		protected internal virtual void RegisterListItem(NumberingListItem item) {
			ListItems.Add(item.Paragraph, item);
		}
		protected internal virtual NumberingListItem CreateListItem(Paragraph paragraph) {
			return new NumberingListItem(paragraph);
		}
		protected internal virtual bool CanRegisterNewList(int listIndex) {
			return listIndex >= 0;
		}
		protected internal virtual void ProcessListItem(NumberingListItem item) {
			int listIndex = GetParagraphNumberingListIndex(item.Paragraph);
			int levelIndex = GetParagraphLevelIndex(item.Paragraph);
			if (NeedCloseRootListOnNextProcessListItem) {
				CloseExistingList(item);
				NeedCloseRootListOnNextProcessListItem = false;
			}
			if (ListTagsStack.Count == 0) {
				if (CanRegisterNewList(listIndex))
					RegisterNewList(item, levelIndex + 1, listIndex, 0);
				return;
			}
			NumberingListTag current = ListTagsStack.Peek();
			int currentListIndex = current.ListIndex;
			int currentLevelIndex = current.LevelIndex;
			if (currentListIndex != listIndex) {
				CloseExistingList(item);
				if (CanRegisterNewList(listIndex))
					RegisterNewList(item, levelIndex + 1, listIndex, 0);
				return;
			}
			if (currentLevelIndex < levelIndex) {
				if (CanRegisterNewList(listIndex))
					RegisterNewList(item, levelIndex - currentLevelIndex, listIndex, currentLevelIndex + 1);
				return;
			}
			if (currentLevelIndex > levelIndex) {
				CloseExistingSubLevels(item, levelIndex);
				ReplaceListLevelItem(item);
				return;
			}
			ReplaceListLevelItem(item);
		}
		protected internal virtual void CloseExistingSubLevels(NumberingListItem item, int levelIndex) {
			while(ListTagsStack.Count > 0) {
				NumberingListTag tag = ListTagsStack.Peek();
				if (tag.LevelIndex <= levelIndex)
					break;
				NumberingListTag tagToClose = CreateCloseTag(tag);
				item.TagsToClose.Add(tagToClose);
				ListTagsStack.Pop();
		   }
		}
		protected internal virtual void CloseExistingList(NumberingListItem item) {
			while (ListTagsStack.Count > 0) {
				NumberingListTag tagToOpen = ListTagsStack.Pop();
				NumberingListTag tagToClose = CreateCloseTag(tagToOpen);
				item.TagsToClose.Add(tagToClose);
			}
		}
		protected internal virtual void RegisterNewList(NumberingListItem item, int count, int listIndex, int startLevelIndex) {
			Debug.Assert(listIndex >= 0);
			for (int i = 0; i < count; i++) {
				NumberingListTag openListTag = CreateNumberingListTag(NumberingListTagType.ListOpen, listIndex, startLevelIndex + i);
				NumberingListTag openListItem = CreateNumberingListTag(NumberingListTagType.ListItemOpen, listIndex, startLevelIndex + i);
				item.TagsToOpen.Add(openListTag);
				item.TagsToOpen.Add(openListItem);
				ListTagsStack.Push(openListTag);
				ListTagsStack.Push(openListItem);
			}
		}
		protected internal NumberingListTag CreateCloseTag(NumberingListTag openTag) {
			NumberingListTagType type = CalculateCloseTagType(openTag.Type);
			return CreateNumberingListTag(type, openTag.ListIndex, openTag.LevelIndex);
		}
		protected internal NumberingListTagType CalculateCloseTagType(NumberingListTagType openTagType) {
			if (openTagType == NumberingListTagType.ListOpen)
				return NumberingListTagType.ListClose;
			return NumberingListTagType.ListItemClose;
		}
		protected internal NumberingListTag CreateNumberingListTag(NumberingListTagType type, NumberingListItem item) {
			int listIndex = GetParagraphNumberingListIndex(item.Paragraph);
			int levelIndex = GetParagraphLevelIndex(item.Paragraph);
			return CreateNumberingListTag(type, listIndex, levelIndex);
		}
		protected internal virtual void ReplaceListLevelItem(NumberingListItem item) {
			NumberingListTag openTag = CreateNumberingListTag(NumberingListTagType.ListItemOpen, item);
			item.TagsToOpen.Add(openTag);
			while (true) {
				if (ListTagsStack.Count == 0)
					return;
				NumberingListTag tag = ListTagsStack.Peek();
				int listIndex = GetParagraphNumberingListIndex(item.Paragraph);
				Debug.Assert(tag.ListIndex == listIndex);
				int levelIndex = GetParagraphLevelIndex(item.Paragraph);
				Debug.Assert(tag.LevelIndex == levelIndex);
				if (!ShouldReplaceListItem(tag, listIndex, levelIndex))
					break;
				PerformReplaceListLevelItem(item, listIndex, levelIndex);
			}
			ListTagsStack.Push(openTag);
		}
		protected internal virtual void PerformReplaceListLevelItem(NumberingListItem item, int listIndex, int levelIndex) {
			NumberingListTag closeTag = CreateNumberingListTag(NumberingListTagType.ListItemClose, listIndex, levelIndex);
			item.TagsToClose.Add(closeTag);
			ListTagsStack.Pop();
		}
		protected virtual bool ShouldReplaceListItem(NumberingListTag tag, int listIndex, int levelIndex) {
			return tag.Type == NumberingListTagType.ListItemOpen && tag.ListIndex == listIndex && tag.LevelIndex == levelIndex;
		}
		protected internal int GetParagraphLevelIndex(Paragraph paragraph) {
			return paragraph.GetOwnListLevelIndex();
		}
		protected internal int GetParagraphNumberingListIndex(Paragraph paragraph) {
			return ((IConvertToInt<NumberingListIndex>)paragraph.GetOwnNumberingListIndex()).ToInt();
		}
		protected internal virtual void RegisterSection(Section section) {
			FinalizeSectionListItems.Add(section, null);
		}
		public virtual void FinalizeSection(Section section) {
			Paragraph lastParagraph = section.DocumentModel.MainPieceTable.Paragraphs[section.LastParagraphIndex];
			NumberingListItem numberingListItem = GetNumberingListItem(lastParagraph);
			FinalizeSectionListItems[section] = numberingListItem;
		}
		protected internal virtual NumberingListItem GetNumberingListItem(Paragraph paragraph) {
			NumberingListItem result = new NumberingListItem(paragraph);
			CloseExistingList(result);
			return result;
		}
		protected internal virtual void FinalizeHeaderFooter(PieceTable pieceTable) {
			if (FinalizeHeaderFooterListItems.ContainsKey(pieceTable))
				return;
			Paragraph lastParagraph = pieceTable.Paragraphs.Last;
			NumberingListItem numberingListItem = GetNumberingListItem(lastParagraph);
			FinalizeHeaderFooterListItems.Add(pieceTable, numberingListItem);
		}
		protected internal bool IsRegistered(Paragraph paragraph) {
			return ListItems.ContainsKey(paragraph);
		}
		public void CloseNumberingListBeforeTableStart() {
			NeedCloseRootListOnNextProcessListItem = true;
		}
	}
	#endregion
	#region NumberingListExporter
	public class NumberingListExporter {
		NumberingListRegistrator registrator;
		ExportHelper writeHelper;
		public NumberingListExporter(NumberingListRegistrator registrator, ExportHelper writeHelper) { 
			Guard.ArgumentNotNull(writeHelper, "writeHelper");
			this.writeHelper = writeHelper;
			Guard.ArgumentNotNull(registrator, "registrator");
			this.registrator = registrator;
		}
		protected internal ExportHelper WriteHelper { get { return writeHelper; } }
		protected internal NumberingListRegistrator Registrator { get { return registrator; } }
		public bool ReadyForProcess { get { return Registrator.ListItems.Count > 0; } }
		public ParagraphIndex ExportParagraph(Paragraph paragraph) {
			if (Registrator.IsRegistered(paragraph))
				ExportNumberedParagraph(paragraph);
			return paragraph.Index;
		}
		protected internal virtual void ExportNumberedParagraph(Paragraph paragraph) {
			NumberingListItem item = Registrator.ListItems[paragraph];
			item.Export(WriteHelper);
		}
		protected internal virtual void EndSectionExport(Section section, ExportHelper writeHelper) {
			NumberingListItem item = Registrator.FinalizeSectionListItems[section];
			if (item != null) item.Export(writeHelper);
		}
		protected internal virtual void EndHeaderFooterExport(PieceTable pieceTable, ExportHelper writeHelper) {
			NumberingListItem item = Registrator.FinalizeHeaderFooterListItems[pieceTable];
			if (item != null) item.Export(writeHelper);
		}
		public virtual void ExportTableStart(Table table) {
			ParagraphIndex index = table.FirstRow.FirstCell.StartParagraphIndex;
			Paragraph paragraph = table.PieceTable.Paragraphs[index];
			if (Registrator.IsRegistered(paragraph)) {
				NumberingListItem item = Registrator.ListItems[paragraph];
				item.ExportTags(item.TagsToClose, WriteHelper);
				item.TagsToClose.Clear();
			}
		}
		public virtual void ExportCellEnd(TableCell cell) {
			ParagraphIndex index = cell.EndParagraphIndex + 1;
			Paragraph nextParagraph = cell.PieceTable.Paragraphs[index];
			if(Registrator.IsRegistered(nextParagraph)){
				NumberingListItem item = Registrator.ListItems[nextParagraph];
				item.ExportTags(item.TagsToClose, WriteHelper);
				item.TagsToClose.Clear();
			}
		}
		public void BeforeExportCoveredCells(TableCell cell) {
			Paragraph next = cell.PieceTable.Paragraphs[cell.EndParagraphIndex + 1];
			if (!Registrator.IsRegistered(next))
				return;
			NumberingListItem item = Registrator.ListItems[next];
			int previouslyOpenTagsCount = item.TagsToClose.Count;
			for (int i = 0; i < previouslyOpenTagsCount; i++) {
				item.TagsToClose[i].Export(writeHelper);
			}
			item.TagsToClose.Clear();
		}
	}
	#endregion
}
