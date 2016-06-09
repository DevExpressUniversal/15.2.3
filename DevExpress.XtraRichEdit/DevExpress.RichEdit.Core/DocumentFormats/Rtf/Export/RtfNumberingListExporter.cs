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
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Import.Rtf;
using DevExpress.Office.NumberConverters;
namespace DevExpress.XtraRichEdit.Export.Rtf {
	#region RtfNumberingListExporter
	public class RtfNumberingListExporter {
		#region Fields
		readonly RtfContentExporter rtfExporter;
		readonly RtfBuilder rtfBuilder;
		string text = String.Empty;
		string numder = String.Empty;
		readonly RtfCharacterPropertiesExporter characterPropertiesExporter;
		readonly RtfParagraphPropertiesExporter paragraphPropertiesExporter;
		int textLength;
		#endregion
		public RtfNumberingListExporter(RtfContentExporter rtfExporter) {
			Guard.ArgumentNotNull(rtfExporter, "rtfExporter");
			this.rtfExporter = rtfExporter;
			this.rtfBuilder = rtfExporter.CreateRtfBuilder();
			this.characterPropertiesExporter = new RtfCharacterPropertiesExporter(rtfExporter.DocumentModel, rtfExporter.RtfExportHelper, rtfBuilder, rtfExporter.Options);
			this.paragraphPropertiesExporter = new RtfParagraphPropertiesExporter(rtfExporter.DocumentModel, rtfExporter.RtfExportHelper, rtfBuilder);
		}
		#region Properties
		public string Text { get { return text; } set { text = value; } }
		public string Number { get { return numder; } set { numder = value; } }
		protected internal RtfBuilder RtfBuilder { get { return rtfBuilder; } }
		public int TextLength { get { return textLength; } set { textLength = value; } }
		protected internal RtfContentExporter RtfExporter { get { return rtfExporter; } }
		#endregion
		public virtual void Export(NumberingListCollection numberingLists, NumberingListIndex startIndex, int count) {
			if (numberingLists.Count <= 0)
				return;
			AbstractNumberingListCollection abstractNumberingLists = GetAbstractNumberingLists(numberingLists, startIndex, count);
			ExportNumberingListTable(abstractNumberingLists);
			ExportListOverrideTable(numberingLists, startIndex, count);
		}
		AbstractNumberingListCollection GetAbstractNumberingLists(NumberingListCollection numberingLists, NumberingListIndex startIndex, int count) {
			AbstractNumberingListCollection result = new AbstractNumberingListCollection();
			NumberingListIndex lastIndex = startIndex + (count - 1);
			for (NumberingListIndex i = startIndex; i <= lastIndex; i++) {
				AbstractNumberingList list = numberingLists[i].AbstractNumberingList;
				if (!result.Contains(list))
					result.Add(list);
			}
			return result;
		}
		public virtual void ExportNumberingListTable(AbstractNumberingListCollection abstractNumberingLists) {
			if (abstractNumberingLists.Count > 0) {
				rtfBuilder.Clear();
				ExportAbstractNumberingLists(abstractNumberingLists);
			}
		}
		protected internal virtual void ExportAbstractNumberingLists(AbstractNumberingListCollection abstractNumberingLists) {
			AbstractNumberingListIndex count = new AbstractNumberingListIndex(abstractNumberingLists.Count);
			for (AbstractNumberingListIndex i = AbstractNumberingListIndex.MinValue; i < count; i++) {
				rtfBuilder.Clear();
				ExportAbstractNumberingList(abstractNumberingLists[i]);
			}
		}
		protected internal virtual void ExportAbstractNumberingList(AbstractNumberingList list) {
			if (list.Id == -2) 
				return;
			rtfBuilder.OpenGroup();
			rtfBuilder.WriteCommand(RtfExportSR.NumberingList);
			rtfBuilder.WriteCommand(RtfExportSR.NumberingListTemplateId, -1);
			if (NumberingListHelper.IsHybridList(list))
				rtfBuilder.WriteCommand(RtfExportSR.NumberingListHybrid);
			ExportListLevels(list.Levels);
			rtfBuilder.OpenGroup();
			rtfBuilder.WriteCommand(RtfExportSR.NumberingListName);
			rtfBuilder.CloseGroup();
			rtfBuilder.WriteCommand(RtfExportSR.NumberingListId, list.Id);
			if (list.StyleLinkIndex >= 0) {
				rtfBuilder.OpenGroup();
				rtfBuilder.WriteCommand(RtfExportSR.NumberingListStyleName, list.StyleLink.StyleName);
				rtfBuilder.CloseGroup();
			}
			else if (list.NumberingStyleReferenceIndex >= 0) {
				rtfBuilder.WriteCommand(RtfExportSR.NumberingListStyleId, list.NumberingStyleReference.NumberingList.AbstractNumberingList.Id);
			}
			rtfBuilder.CloseGroup();
			if (!rtfExporter.RtfExportHelper.ListCollection.ContainsKey(list.Id))
				rtfExporter.RtfExportHelper.ListCollection.Add(list.Id, rtfBuilder.RtfContent.ToString());
		}
		protected internal virtual void ExportListLevels(ListLevelCollection<ListLevel> listLevelCollection) {
			int count = listLevelCollection.Count;
			for (int i = 0; i < count; i++)
				ExportListLevel(listLevelCollection[i]);
		}
		public virtual void ExportListOverrideTable(NumberingListCollection numberingLists, NumberingListIndex startIndex, int count) {
			if (numberingLists.Count <= 0)
				return;
			NumberingListIndex lastIndex = startIndex + (count - 1);
			for (NumberingListIndex i = startIndex; i <= lastIndex; i++) {
				rtfBuilder.Clear();
				ExportListOverride(numberingLists[i]);
			}
		}
		public virtual void ExportListOverrideTable(NumberingListCollection numberingLists) {
			if (numberingLists.Count <= 0)
				return;
			NumberingListIndex count = new NumberingListIndex(numberingLists.Count);
			for (NumberingListIndex i = new NumberingListIndex(0); i < count; i++) {
				rtfBuilder.Clear();
				ExportListOverride(numberingLists[i]);
			}
		}
		protected internal virtual void ExportListOverride(NumberingList numberingList) {
			rtfBuilder.OpenGroup();
			rtfBuilder.WriteCommand(RtfExportSR.ListOverride);
			rtfBuilder.WriteCommand(RtfExportSR.NumberingListId, numberingList.AbstractNumberingList.Id);
			int listOverrideCount = GetListOverrideCount(numberingList);
			rtfBuilder.WriteCommand(RtfExportSR.ListOverrideCount, listOverrideCount);
			WriteListOverrideId(numberingList);
			ExportListOverrideLevels(numberingList);
			rtfBuilder.CloseGroup();
			int index = rtfExporter.RtfExportHelper.ListOverrideCollection.Count;			
			rtfExporter.RtfExportHelper.ListOverrideCollection.Add(rtfBuilder.RtfContent.ToString());
			if (!rtfExporter.RtfExportHelper.ListOverrideCollectionIndex.ContainsKey(numberingList.Id))
				rtfExporter.RtfExportHelper.ListOverrideCollectionIndex.Add(numberingList.Id, index);
		}
		protected internal virtual void WriteListOverrideId(NumberingList numberingList) {
			rtfBuilder.WriteCommand(RtfExportSR.ListIndex, numberingList.Id);
		}
		int GetListOverrideCount(NumberingList numberingList) {
			ListLevelCollection<IOverrideListLevel> levels = numberingList.Levels;
			int count = levels.Count;
			int result = 0;
			for (int i = 0; i < count; i++)
				if (IsOverrideLevel(levels[i]))
					result++;
			return result;
		}
		bool IsOverrideLevel(IOverrideListLevel listLevel) {
			if (listLevel is OverrideListLevel)
				return true;
			return listLevel.OverrideStart;
		}
		void ExportListOverrideLevels(NumberingList numberingList) {
			ListLevelCollection<IOverrideListLevel> levels = numberingList.Levels;
			int count = levels.Count;
			for (int i = 0; i < count; i++)
				if (IsOverrideLevel(levels[i]))
					ExportListOverrideLevel(levels[i]);
		}
		void ExportListOverrideLevel(IOverrideListLevel level) {
			rtfBuilder.OpenGroup();
			rtfBuilder.WriteCommand(RtfExportSR.ListOverrideLevel);
			if (level.OverrideStart)
				rtfBuilder.WriteCommand(RtfExportSR.ListOverrideStart);
			OverrideListLevel overrideLevel = level as OverrideListLevel;
			if (overrideLevel != null) {
				rtfBuilder.WriteCommand(RtfExportSR.ListOverrideFormat);
				ExportListLevel(level);
			}
			else {
				rtfBuilder.WriteCommand(RtfExportSR.ListLevelStart, level.NewStart);
			}
			rtfBuilder.CloseGroup();
		}
		protected internal virtual void ExportListLevel(IListLevel listLevel) {
			int numberingListFormat = GetNumberingListFormat(listLevel.ListLevelProperties.Format);
			rtfBuilder.OpenGroup();
			rtfBuilder.WriteCommand(RtfExportSR.ListLevel);
			rtfBuilder.WriteCommand(RtfExportSR.ListLevelNumberingFormat, numberingListFormat);
			rtfBuilder.WriteCommand(RtfExportSR.ListLevelNumberingFormatN, numberingListFormat);
			rtfBuilder.WriteCommand(RtfExportSR.ListLevelAlignment, (int)listLevel.ListLevelProperties.Alignment);
			rtfBuilder.WriteCommand(RtfExportSR.ListLevelAlignmentN, (int)listLevel.ListLevelProperties.Alignment);
			rtfBuilder.WriteCommand(RtfExportSR.ListLevelFollow, GetListLevelSeparator(listLevel.ListLevelProperties.Separator));			
			rtfBuilder.WriteCommand(RtfExportSR.ListLevelStart, listLevel.ListLevelProperties.Start);
			if (listLevel.ListLevelProperties.Legacy) {
				rtfBuilder.WriteCommand(RtfExportSR.ListLevelLegacy);
				rtfBuilder.WriteCommand(RtfExportSR.ListLevelLegacySpace,  listLevel.DocumentModel.UnitConverter.ModelUnitsToTwips(listLevel.ListLevelProperties.LegacySpace));
				rtfBuilder.WriteCommand(RtfExportSR.ListLevelLegacyIndent, listLevel.DocumentModel.UnitConverter.ModelUnitsToTwips(listLevel.ListLevelProperties.LegacyIndent));
			}
			ExportListLevelTextAndNumber(listLevel.ListLevelProperties.DisplayFormatString, listLevel.ListLevelProperties.TemplateCode);
			if (listLevel.ListLevelProperties.ConvertPreviousLevelNumberingToDecimal)
				rtfBuilder.WriteCommand(RtfExportSR.ListLevelLegal, 1);
			else
				rtfBuilder.WriteCommand(RtfExportSR.ListLevelLegal, 0);
			if (listLevel.ListLevelProperties.SuppressRestart)
				rtfBuilder.WriteCommand(RtfExportSR.ListLevelNoRestart, 1);
			else
				rtfBuilder.WriteCommand(RtfExportSR.ListLevelNoRestart, 0);
			ExportListLevelCharacterAndParagraphProperties(listLevel);
			rtfBuilder.CloseGroup();
		}
		protected internal int GetNumberingListFormat(NumberingFormat numberingFormat) {
			int result = ListLevelDestination.numberingFormats.IndexOf(numberingFormat);
			return result >= 0 ? result : 0;
		}
		protected internal virtual int GetListLevelSeparator(char separator) {
			switch (separator) {
				case Characters.TabMark:
					return 0;
				case ' ':
					return 1;
				default:
					return 2;
			}
		}
		protected internal virtual void ExportListLevelTextAndNumber(string displayFormatString, int levelTemplateId) {
			Text = String.Empty; ;
			Number = String.Empty;
			GetTextAndNumber(displayFormatString);
			rtfBuilder.OpenGroup();
			rtfBuilder.WriteCommand(RtfExportSR.ListLevelText);			
			if(levelTemplateId != 0)
				rtfBuilder.WriteCommand(RtfExportSR.LevelTemplateId, levelTemplateId);
			rtfBuilder.WriteTextDirect(String.Format(@"\'{0:x2}", TextLength));
			rtfBuilder.WriteTextDirect(Text, true);
			rtfBuilder.WriteChar(';');
			rtfBuilder.CloseGroup();
			rtfBuilder.OpenGroup();
			rtfBuilder.WriteCommand(RtfExportSR.ListLevelNumbers, Number + ";");
			rtfBuilder.CloseGroup();
		}
		protected internal void GetTextAndNumber(string displayFormatString) {
			TextLength = 0;
			int count = displayFormatString.Length;
			int i = 0;
			while (i < count) {
				char ch = displayFormatString[i];
				if (!RtfBuilder.IsSpecialSymbol(ch))
					i = AddChar(ch, i);
				else
					i = AddLevelNumber(displayFormatString, i);
			}
		}
		protected internal int AddLevelNumber(string displayFormatString, int i) {
			if (DoubleBrackets(displayFormatString, i)) {
				i = AddEscapedChar(displayFormatString[i], i);
				return i + 1;
			}
			if (displayFormatString[i] == '\\')
				return AddEscapedChar(displayFormatString[i], i);
			return AddLevelNumberCore(displayFormatString, i);
		}
		protected internal int AddLevelNumberCore(string displayFormatString, int i) {
			string value = String.Empty;
			i++;
			value = displayFormatString.Substring(i, displayFormatString.IndexOf('}', i) - i);
			Text += @"\'" + String.Format("{0:x2}", int.Parse(value));
			TextLength += value.Length;
			Number += @"\'" + String.Format("{0:x2}", TextLength);
			return i + value.Length + 1;
		}
		protected internal static bool DoubleBrackets(string displayFormatString, int i) {
			return (displayFormatString[i] == '}' && displayFormatString[i + 1] == '}') ||
					(displayFormatString[i] == '{' && displayFormatString[i + 1] == '{');
		}
		protected internal int AddChar(char ch, int i) {
			Text += ch;
			TextLength++;
			return i + 1;
		}
		protected internal int AddEscapedChar(char ch, int i) {
			string str = @"\'" + RtfBuilder.byteToHexString[(int)ch % 256];
			Text += str;
			TextLength++;
			return i + 1;
		}
		void ExportListLevelCharacterAndParagraphProperties(IListLevel listLevel) {
			CharacterPropertiesMerger merger = new CharacterPropertiesMerger(listLevel.CharacterProperties);
			merger.Merge(rtfExporter.DocumentModel.DefaultCharacterProperties);
			merger.MergedProperties.Options.UseDoubleFontSize = listLevel.CharacterProperties.UseDoubleFontSize;
			merger.MergedProperties.Options.UseFontName = listLevel.CharacterProperties.UseFontName;
			characterPropertiesExporter.ExportCharacterProperties(merger.MergedProperties, true, true, true);
			ListLevel abstractListLevel = listLevel as ListLevel;
			if (abstractListLevel != null)
				ExportAbstractListLevelParagraphStyleIndex(abstractListLevel);			
			ParagraphPropertiesMerger paragraphMerger = new ParagraphPropertiesMerger(listLevel.ParagraphProperties);
			paragraphMerger.Merge(rtfExporter.DocumentModel.DefaultParagraphProperties);			
			paragraphPropertiesExporter.WriteParagraphIndents(paragraphMerger.MergedProperties);
		}
		void ExportAbstractListLevelParagraphStyleIndex(ListLevel abstractListLevel) {
			ParagraphStyleCollection paragraphStyles = rtfExporter.DocumentModel.ParagraphStyles;
			int paragraphStyleIndex = abstractListLevel.ParagraphStyleIndex;
			if (paragraphStyleIndex < 0 || paragraphStyleIndex >= paragraphStyles.Count)
				return;
			rtfExporter.WriteParagraphStyle(rtfBuilder, paragraphStyles[abstractListLevel.ParagraphStyleIndex]);
		}
	}
	#endregion
}
