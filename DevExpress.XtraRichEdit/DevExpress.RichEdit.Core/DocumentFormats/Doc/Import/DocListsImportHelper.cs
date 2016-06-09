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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Import.Doc {
	public class DocListsImportHelper {
		#region Fields
		DocContentBuilder contentBuilder;
		DocumentModel documentModel;
		Dictionary<int, AbstractNumberingListIndex> abstractListMapping;
		#endregion
		public DocListsImportHelper(DocContentBuilder contentBuilder, DocumentModel documentModel) {
			this.contentBuilder = contentBuilder;
			this.documentModel = documentModel;
			this.abstractListMapping = new Dictionary<int, AbstractNumberingListIndex>();
		}
		#region Properties
		protected DocContentBuilder ContentBuilder { get { return this.contentBuilder; } }
		protected DocumentModel DocumentModel { get { return this.documentModel; } }
		protected Dictionary<int, AbstractNumberingListIndex> AbstractListMapping { get { return this.abstractListMapping; } }
		#endregion
		public void InitializeAbstractLists() {
			this.abstractListMapping = new Dictionary<int, AbstractNumberingListIndex>();
			List<DocListData> listData = ContentBuilder.ListInfo.ListData;
			int count = listData.Count;
			for (int i = 0; i < count; i++) {
				CreateAbstractList(listData[i]);
			}
		}
		public void InitializeLists() {
			DocListOverrideFormatInformation listOverrideInfo = ContentBuilder.ListOverrideInfo;
			Debug.Assert(listOverrideInfo.FormatOverride.Count == listOverrideInfo.FormatOverrideData.Count);
			int count = listOverrideInfo.FormatOverride.Count;
			for (int i = 0; i < count; i++) {
				CreateList(listOverrideInfo.FormatOverride[i], listOverrideInfo.FormatOverrideData[i]);
			}
		}
		void CreateAbstractList(DocListData currentListData) {
			if (this.abstractListMapping.ContainsKey(currentListData.ListFormatting.ListIdentifier))
				return;
			AbstractNumberingList abstractList = new AbstractNumberingList(DocumentModel);
			abstractList.SetId(currentListData.ListFormatting.ListIdentifier);
			this.abstractListMapping.Add(currentListData.ListFormatting.ListIdentifier, new AbstractNumberingListIndex(DocumentModel.AbstractNumberingLists.Count));
			DocumentModel.AddAbstractNumberingListUsingHistory(abstractList);
			List<DocListLevel> levelsFormatting = currentListData.LevelsFormatting;
			int count = levelsFormatting.Count;
			for (int i = 0; i < count; i++) {
				InitializeListLevel(abstractList.Levels[i], levelsFormatting[i]);
			}
		}
		void InitializeListLevel(ListLevel listLevel, DocListLevel docListLevel) {
			SetListLevelFormatting(listLevel, docListLevel);
			SetListLevelProperties(listLevel, docListLevel);
		}
		void SetListLevelFormatting(ListLevel listLevel, DocListLevel docListLevel) {
			DocPropertyContainer container = DocCommandHelper.Traverse(docListLevel.CharacterUPX, ContentBuilder.Factory, ContentBuilder.DataReader);
			DocCommandHelper.Traverse(docListLevel.ParagraphUPX, container, ContentBuilder.DataReader);
			ContentBuilder.FontManager.SetFontName(container);
			if (container.CharacterInfo != null) {
				CharacterFormattingInfo defaultFormattingInfo = DocumentModel.Cache.CharacterFormattingCache[CharacterFormattingInfoCache.DefaultItemIndex].Info.Clone();
				CharacterFormattingInfo characterFormattingInfo = DocCharacterFormattingHelper.GetMergedCharacterFormattingInfo(container.CharacterInfo.FormattingInfo, defaultFormattingInfo);
				listLevel.CharacterProperties.BeginInit();
				listLevel.CharacterProperties.CopyFrom(new MergedCharacterProperties(characterFormattingInfo, container.CharacterInfo.FormattingOptions));
				listLevel.CharacterProperties.EndInit();
				listLevel.ListLevelProperties.SuppressBulletResize = container.CharacterInfo.PictureBulletInformation.SuppressBulletResize;
			}
			if (container.ParagraphInfo != null)
				listLevel.ParagraphProperties.CopyFrom(new MergedParagraphProperties(container.ParagraphInfo.FormattingInfo, container.ParagraphInfo.FormattingOptions));
		}
		void SetListLevelProperties(ListLevel listLevel, DocListLevel docListLevel) {
			ListLevelProperties listLevelProperties = listLevel.ListLevelProperties;
			DocListLevelProperties docListLevelProperties = docListLevel.ListLevelProperties;
			listLevelProperties.Alignment = docListLevelProperties.Alignment;
			listLevelProperties.ConvertPreviousLevelNumberingToDecimal = docListLevelProperties.ConvertPreviousLevelNumberingToDecimal;
			listLevelProperties.DisplayFormatString = docListLevel.GetDisplayFormatString();
			listLevelProperties.Format = docListLevelProperties.NumberingFormat;
			listLevelProperties.Legacy = docListLevelProperties.Legacy;
			listLevelProperties.LegacyIndent = docListLevelProperties.LegacyIndent;
			listLevelProperties.LegacySpace = docListLevelProperties.LegacySpace;
			listLevelProperties.RelativeRestartLevel = docListLevelProperties.RelativeRestartLevel;
			listLevelProperties.Separator = docListLevelProperties.Separator;
			listLevelProperties.Start = docListLevelProperties.Start;
			listLevelProperties.SuppressRestart = docListLevelProperties.SuppressRestart;
			 if (docListLevelProperties.BulletedList) {
				 listLevelProperties.Format = Office.NumberConverters.NumberingFormat.Bullet;
				 listLevelProperties.TemplateCode = NumberingListHelper.GenerateNewTemplateCode(documentModel);
			 }
		}
		void CreateList(DocListOverrideFormat listOverrideFormat, DocListOverrideLevelInformation listOverrideLevelInformation) {
			AbstractNumberingListIndex index;
			if (!AbstractListMapping.TryGetValue(listOverrideFormat.ListIdentifier, out index))
				return;
			NumberingList list = new NumberingList(DocumentModel, index);
			DocumentModel.AddNumberingListUsingHistory(list);
			int count = listOverrideFormat.LevelsCount;
			for (int i = 0; i < count; i++) {
				DocListOverrideLevelFormat levelFormat = listOverrideLevelInformation.LevelFormatOverrideData[i];
				int overriddenLevel = levelFormat.OverriddenLevel;
				if (levelFormat.OverrideFormatting) {
					OverrideListLevel overrideLevel = new OverrideListLevel(DocumentModel);
					InitializeListLevel(overrideLevel, levelFormat.OverrideLevelFormatting);
					list.Levels[overriddenLevel] = overrideLevel;
					overrideLevel.SetOverrideStart(levelFormat.OverrideStart);
				}
				else if (levelFormat.OverrideStart) {
					list.Levels[overriddenLevel].SetOverrideStart(true);
					list.Levels[overriddenLevel].NewStart = levelFormat.StartAt;
				}
			}
		}
		public void LinkNumberingListStyles(DocStylesImportHelper helper) {
			List<ListStylesRecordItem> styles = ContentBuilder.DocFileRecords.ListStyles;
			int count = styles.Count;
			AbstractNumberingListCollection abstractLists = DocumentModel.AbstractNumberingLists;
			int abstractListsCount = abstractLists.Count;
			int numberingListStylesCount = DocumentModel.NumberingListStyles.Count;
			for (int i = 0; i < count; i++) {
				ListStylesRecordItem item = styles[i];
				if(item.ListIndex < 0 || item.ListIndex >= abstractListsCount)
					continue;
				AbstractNumberingListIndex listIndex = new AbstractNumberingListIndex(item.ListIndex);
				AbstractNumberingList list = abstractLists[listIndex];
				int styleIndex = helper.GetNumberingStyleIndex(item.StyleIndex);
				if (styleIndex < 0 || styleIndex >= numberingListStylesCount)
					continue;
				if (item.StyleDefinition)
					list.SetStyleLinkIndex(styleIndex);
				else
					list.SetNumberingStyleReferenceIndex(styleIndex);					
			}
		}
	}
}
