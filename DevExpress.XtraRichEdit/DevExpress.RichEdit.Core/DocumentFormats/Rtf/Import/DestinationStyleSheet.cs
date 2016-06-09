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
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Utils;
using System.Collections;
namespace DevExpress.XtraRichEdit.Import.Rtf {
	#region StyleSheetDestination
	public class StyleSheetDestination : DestinationPieceTable {
		KeywordTranslatorTable keywordHT = CreateKeywordTable();
		string styleName;		
		static KeywordTranslatorTable CreateKeywordTable() {
			KeywordTranslatorTable table = new KeywordTranslatorTable();
			table.Add("s", OnParagraphStyle);
			table.Add("sqformat", OnStyleQFormatKeyword);
			table.Add("sbasedon", OnParentStyleIndex);
			table.Add("cs", OnCharacterStyle);
			table.Add("ts", OnTableStyle);
			table.Add("slink", OnStyleLinkKeyword);
			table.Add("snext", OnNextStyleIndex);
			table.Add("ls", OnStyleListOverride);
			table.Add("ilvl", OnStyleListLevel);
			AddParagraphPropertiesKeywords(table);
			AddCharacterPropertiesKeywords(table);
			return table;
		}
		static void OnStyleQFormatKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			((StyleSheetDestination)importer.Destination).QFormat = true;
		}
		static void OnParagraphStyle(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.ParagraphFormattingInfo.StyleIndex = parameterValue;
		}
		static void OnParentStyleIndex(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.ParagraphFormattingInfo.ParentStyleIndex = parameterValue;
		}
		static void OnStyleLinkKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.ParagraphFormattingInfo.StyleLink = parameterValue;
		}
		static void OnNextStyleIndex(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.ParagraphFormattingInfo.NextStyle = parameterValue;
		}
		static void OnCharacterStyle(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Destination = new DestinationCharacterStyle(importer, parameterValue);
		}
		static void OnTableStyle(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Destination = new DestinationTableStyle(importer, parameterValue);
		}
		internal static void OnStyleListOverride(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.ParagraphFormattingInfo.NumberingListIndex = new NumberingListIndex(parameterValue);
		}
		internal static void OnStyleListLevel(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.ParagraphFormattingInfo.ListLevelIndex = parameterValue;
		}
		public StyleSheetDestination(RtfImporter importer)
			: base(importer, importer.PieceTable) {
			this.styleName = String.Empty;
		}
		bool QFormat { get; set; }
		protected internal override bool CanAppendText { get { return false; } }
		public override void NestedGroupFinished(DestinationBase nestedDestination) {
			StyleSheetDestination styleSheetDestination = nestedDestination as StyleSheetDestination;
			if (styleSheetDestination != null) {
				RtfParagraphFormattingInfo paragraphFormattingInfo = Importer.Position.ParagraphFormattingInfo;
				ParagraphStyle style = GetParagraphStyle(paragraphFormattingInfo);
				string name = GetPrimaryStyleName(styleSheetDestination.styleName);
				if (!String.IsNullOrEmpty(name))
					style.StyleName = name;
				if (paragraphFormattingInfo.StyleLink >= 0 && !Importer.LinkParagraphStyleIndexToCharacterStyleIndex.ContainsKey(paragraphFormattingInfo.StyleIndex))
					Importer.LinkParagraphStyleIndexToCharacterStyleIndex.Add(Importer.Position.ParagraphFormattingInfo.StyleIndex, paragraphFormattingInfo.StyleLink);
				if (paragraphFormattingInfo.NextStyle >= 0)
					Importer.NextParagraphStyleIndexTable[paragraphFormattingInfo.StyleIndex] = paragraphFormattingInfo.NextStyle;
				MergedCharacterProperties parentCharacterProperties = Importer.GetStyleMergedParagraphCharacterProperties(-1, paragraphFormattingInfo.ParentStyleIndex);
				Importer.ApplyParagraphProperties(style.ParagraphProperties, paragraphFormattingInfo.ParentStyleIndex, paragraphFormattingInfo);
				Importer.ApplyCharacterProperties(style.CharacterProperties, Importer.Position.CharacterFormatting.Info, parentCharacterProperties);
				if (paragraphFormattingInfo.NumberingListIndex >= new NumberingListIndex(0)) {
					if (!Importer.ParagraphStyleListOverrideIndexMap.ContainsKey(style)) {
						int index = ((IConvertToInt<NumberingListIndex>)paragraphFormattingInfo.NumberingListIndex).ToInt();
						int levelIndex = paragraphFormattingInfo.ListLevelIndex;
						Importer.ParagraphStyleListOverrideIndexMap.Add(style, new RtfNumberingListInfo(index, levelIndex));
					}
				}
				style.Primary = styleSheetDestination.QFormat;
				QFormat = false;
				AddParagraphStyleCollectionIndex(paragraphFormattingInfo, style);
			}
		}
		public static string GetPrimaryStyleName(string styleName) {
			string[] names = styleName.Split(',');
			int count = names.Length;
			for (int i = 0; i < count; i++) {
				string name = names[i].Trim();
				if (!String.IsNullOrEmpty(name))
					return name.Trim();
			}
			return String.Empty;
		}
		void AddParagraphStyleCollectionIndex(RtfParagraphFormattingInfo paragraphFormattingInfo, ParagraphStyle style) {
			if (!Importer.ParagraphStyleCollectionIndex.ContainsKey(paragraphFormattingInfo.StyleIndex)) {
				Importer.DocumentModel.ParagraphStyles.Add(style);
				if (Importer.ParagraphStyleCollectionIndex.ContainsKey(paragraphFormattingInfo.ParentStyleIndex)) {
					int paragraphStyleIndex = Importer.ParagraphStyleCollectionIndex[paragraphFormattingInfo.ParentStyleIndex];
					style.Parent = Importer.DocumentModel.ParagraphStyles[paragraphStyleIndex];
				}
				Importer.ParagraphStyleCollectionIndex.Add(paragraphFormattingInfo.StyleIndex, Importer.DocumentModel.ParagraphStyles.Count - 1);
			}
		}
		ParagraphStyle GetParagraphStyle(RtfParagraphFormattingInfo paragraphFormattingInfo) {
			if (paragraphFormattingInfo.StyleIndex == 0)
				return Importer.DocumentModel.ParagraphStyles[0];
			return new ParagraphStyle(Importer.DocumentModel);
		}
		protected override KeywordTranslatorTable KeywordHT { get { return keywordHT; } }
		protected override ControlCharTranslatorTable ControlCharHT { get { return null; } }
		protected override DestinationBase CreateClone() {
			StyleSheetDestination result = new StyleSheetDestination(Importer);
			result.styleName = styleName;
			return result;
		}
		protected override void ProcessCharCore(char ch) {
			if (ch != ';')
				this.styleName += ch;
		}
		public override void FinalizePieceTableCreation() {
		}
	}
	#endregion
}
