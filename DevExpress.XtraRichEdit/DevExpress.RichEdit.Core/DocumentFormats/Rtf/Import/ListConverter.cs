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
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
namespace DevExpress.XtraRichEdit.Import.Rtf {
	#region RtfListConverter
	public class RtfListConverter {
		readonly Dictionary<RtfListId, int> styleCrossTable = new Dictionary<RtfListId, int>();
		readonly RtfImporter importer;
		public RtfListConverter(RtfImporter importer) {
			Guard.ArgumentNotNull(importer, "importer");
			this.importer = importer;
		}
		public RtfImporter Importer { get { return importer; } }
		public DocumentModel DocumentModel { get { return Importer.DocumentModel; } }
		public virtual void Convert(RtfListTable listTable, ListOverrideTable listOverrideTable) {
			CreateAbstractNumberingLists(listTable);
			LinkNumberingListStyles(listOverrideTable, listTable);
			FixBrokenListStyles();
			CreateNumberingListsCore(listOverrideTable, listTable);
		}
		void FixBrokenListStyles() {
			NumberingListStyleCollection styles = DocumentModel.NumberingListStyles;
			int count = styles.Count;
			for (int i = 0; i < count; i++) {
				NumberingListStyle style = styles[i];
				if (style.NumberingListIndex >= NumberingListIndex.MinValue)
					continue;
				AbstractNumberingListIndex abstractListIndex = FindAbstractNumberingListByStyle(i);
				if (abstractListIndex == AbstractNumberingListIndex.InvalidValue)
					continue;
				NumberingList list = new NumberingList(DocumentModel, abstractListIndex);
				DocumentModel.AddNumberingListUsingHistory(list);
				NumberingListIndex numberingListIndex = new NumberingListIndex(DocumentModel.NumberingLists.Count - 1);
				style.SetNumberingListIndex(numberingListIndex);
			}
		}
		AbstractNumberingListIndex FindAbstractNumberingListByStyle(int styleIndex) {
			AbstractNumberingListCollection lists = DocumentModel.AbstractNumberingLists;
			AbstractNumberingListIndex count = new AbstractNumberingListIndex(lists.Count);
			for (AbstractNumberingListIndex i = new AbstractNumberingListIndex(0); i < count; i++) {
				if (lists[i].StyleLinkIndex == styleIndex)
					return i;
			}
			return AbstractNumberingListIndex.InvalidValue;
		}
		void CreateNumberingListsCore(ListOverrideTable listOverrideTable, RtfListTable listTable) {
			int count = listOverrideTable.Count;
			AbstractNumberingListCollection abstractNumberingLists = DocumentModel.AbstractNumberingLists;			
			for (int i = 0; i < count; i++) {
				RtfNumberingListOverride rtfList = listOverrideTable[i];
				AbstractNumberingListIndex sourceListIndex = GetListIndex((int)rtfList.ListId, abstractNumberingLists);
				if (sourceListIndex < new AbstractNumberingListIndex(0))
					continue;
				AbstractNumberingList abstractList = DocumentModel.AbstractNumberingLists[sourceListIndex];
				if (abstractList.StyleLinkIndex >= 0)
					continue;
				NumberingList list = new NumberingList(DocumentModel, sourceListIndex);
				int overrideId = (int)rtfList.Id;
				DocumentModel.AddNumberingListUsingHistory(list);
				ConvertRtfOverrideToNumbering(list, rtfList);
				NumberingListIndex numberingListIndex = new NumberingListIndex(DocumentModel.NumberingLists.Count - 1);
				Importer.ListOverrideIndexToNumberingListIndexMap[overrideId] = numberingListIndex;
				list.SetId(DocumentModel.NumberingLists.Count);				
			}
		}
		void LinkNumberingListStyles(ListOverrideTable listOverrideTable, RtfListTable listTable) {
			int count = listOverrideTable.Count;
			AbstractNumberingListCollection abstractNumberingLists = DocumentModel.AbstractNumberingLists;
			for (int i = 0; i < count; i++) {
				RtfNumberingListOverride rtfList = listOverrideTable[i];
				AbstractNumberingListIndex sourceListIndex = GetListIndex((int)rtfList.ListId, abstractNumberingLists);
				if (sourceListIndex < new AbstractNumberingListIndex(0))
					continue;
				AbstractNumberingList abstractList = DocumentModel.AbstractNumberingLists[sourceListIndex];
				if (abstractList.StyleLinkIndex >= 0) {
					NumberingList list = new NumberingList(DocumentModel, sourceListIndex);
					DocumentModel.AddNumberingListUsingHistory(list);
					NumberingListIndex numberingListIndex = new NumberingListIndex(DocumentModel.NumberingLists.Count - 1);
					list.SetId(DocumentModel.NumberingLists.Count);
					int overrideId = (int)rtfList.Id;
					Importer.ListOverrideIndexToNumberingListIndexMap[overrideId] = numberingListIndex;
					DocumentModel.NumberingListStyles[abstractList.StyleLinkIndex].SetNumberingListIndex(numberingListIndex);
				}			   
			}
		}
		void CreateAbstractNumberingLists(RtfListTable listTable) {
			int count = listTable.Count;
			List<AbstractNumberingList> lists = new List<AbstractNumberingList>();
			for (int i = 0; i < count; i++) {
				RtfNumberingList rtfList = listTable[i];
				AbstractNumberingList list = CreateAbstractNumberingList(rtfList);
				lists.Add(list);
				if (!String.IsNullOrEmpty(rtfList.StyleName)) {
					if (DocumentModel.NumberingListStyles.GetStyleIndexByName(rtfList.StyleName) < 0) {
						NumberingListStyle style = new NumberingListStyle(DocumentModel, rtfList.StyleName);
						int styleIndex = DocumentModel.NumberingListStyles.Add(style);
						list.SetStyleLinkIndex(styleIndex);
						styleCrossTable.Add(rtfList.Id, styleIndex);
					}
				}
			}
			for (int i = 0; i < count; i++) {
				RtfNumberingList rtfList = listTable[i];
				AbstractNumberingList list = lists[i];
				if (rtfList.ParentStyleId != new RtfListId(0)) {
					int styleIndex = GetStyleIndex(rtfList.ParentStyleId);
					if (styleIndex >= 0)
						list.SetNumberingStyleReferenceIndex(styleIndex);
				}
			}
		}
		AbstractNumberingList CreateAbstractNumberingList(RtfNumberingList rtfList) {
			AbstractNumberingList list = new AbstractNumberingList(DocumentModel);
			ConvertRtfListToNumberingList(rtfList.Levels, list);
			if (IsHybridList(rtfList))
				NumberingListHelper.SetHybridListType(list);
			list.SetId((int)rtfList.Id);
			DocumentModel.AddAbstractNumberingListUsingHistory(list);
			return list;
		}
		protected virtual bool IsHybridList(RtfNumberingList rtfList) {
			if (rtfList.NumberingListType != RtfNumberingListType.Unknown)
				return true;
			List<RtfListLevel> levels = rtfList.Levels;
			int count = levels.Count;
			for (int i = 0; i < count; i++)
				if (levels[i].ListLevelProperties.TemplateCode != 0)
					return true;
			return false;
		}
		protected internal int GetStyleIndex(RtfListId rtfParentStyleId) {
			int styleIndex;
			if (styleCrossTable.TryGetValue(rtfParentStyleId, out styleIndex))
				return styleIndex;
			else
				return -1;
		}
		protected internal AbstractNumberingListIndex GetListIndex(int listId, AbstractNumberingListCollection lists) {
			AbstractNumberingListIndex count = new AbstractNumberingListIndex(lists.Count);
			for (AbstractNumberingListIndex i = new AbstractNumberingListIndex(0); i < count; i++) {
				if (lists[i].GetId() == listId)
					return i;
			}
			return new AbstractNumberingListIndex(-1);
		}
		public virtual void ConvertRtfOverrideToNumbering(NumberingList list, RtfNumberingListOverride rtfOverride) {
			int count = rtfOverride.Levels.Count;
			for (int i = 0; i < count; i++) {
				bool restart = rtfOverride.Levels[i].OverrideStartAt;
				bool reformat = rtfOverride.Levels[i].OverrideFormat;
				if (reformat) {
					OverrideListLevel level = new OverrideListLevel(DocumentModel);
					level.ListLevelProperties.BeginInit();
					try {
						level.ListLevelProperties.CopyFrom(list.Levels[i].ListLevelProperties);
						level.CharacterProperties.CopyFrom(rtfOverride.Levels[i].Level.CharacterProperties);
						level.ParagraphProperties.CopyFrom(rtfOverride.Levels[i].Level.ParagraphProperties);
						ConvertPropertyRtfToNumbering(rtfOverride.Levels[i].Level, level.ListLevelProperties, restart, reformat);
					}
					finally {
						level.ListLevelProperties.EndInit();
					}
					list.SetLevel(i, level);
					if (restart)
						level.SetOverrideStart(true);
				}
				else {
					if (restart) {
						NumberingListReferenceLevel referenceLevel = (NumberingListReferenceLevel)list.Levels[i];
						referenceLevel.SetOverrideStart(true);
						referenceLevel.NewStart = rtfOverride.Levels[i].StartAt;
					}
				}
			}
		}
		protected virtual void ConvertRtfListToNumberingList(List<RtfListLevel> rtfLevels, AbstractNumberingList list) {
			int count = rtfLevels.Count;
			for (int i = 0; i < count; i++) {
				ListLevel level = list.Levels[i];
				level.ParagraphProperties.CopyFrom(rtfLevels[i].ParagraphProperties);
				level.CharacterProperties.CopyFrom(rtfLevels[i].CharacterProperties);
				level.ParagraphStyleIndex = rtfLevels[i].ParagraphStyleIndex;
				ConvertPropertyRtfToNumbering(rtfLevels[i], level.ListLevelProperties, true, true);				
			}
		}
		private static void ConvertPropertyRtfToNumbering(RtfListLevel rtfLevel, IListLevelProperties level, bool restart, bool reformat) {
			ListLevelProperties levelProperties = rtfLevel.ListLevelProperties;
			if (restart == true)
				level.Start = levelProperties.Start;
			if (reformat == true) {
				level.Format = levelProperties.Format;
				level.Alignment = levelProperties.Alignment;
				level.SuppressBulletResize = levelProperties.SuppressBulletResize;
				level.SuppressRestart = levelProperties.SuppressRestart;
				level.Separator = levelProperties.Separator;
				level.ConvertPreviousLevelNumberingToDecimal = levelProperties.ConvertPreviousLevelNumberingToDecimal;
				level.DisplayFormatString = rtfLevel.CreatDisplayFormatString();
				level.TemplateCode = levelProperties.TemplateCode;
			}
			if (levelProperties.Legacy) {
				level.Legacy = levelProperties.Legacy;
				level.LegacySpace = levelProperties.LegacySpace;
				level.LegacyIndent = levelProperties.LegacyIndent;
			}
		}
	}
	#endregion
}
