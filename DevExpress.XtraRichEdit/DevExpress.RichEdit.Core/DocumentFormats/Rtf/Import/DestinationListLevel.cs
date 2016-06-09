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
using System.Text;
using DevExpress.Office;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Office.NumberConverters;
namespace DevExpress.XtraRichEdit.Import.Rtf {
	#region RtfListLevel
	public class RtfListLevel : ListLevel {
		string text = String.Empty;
		string numbers = String.Empty;
		public RtfListLevel(DocumentModel documentModel)
			: base(documentModel) {
			ListLevelProperties.Start = 1;
			ListLevelProperties.Separator = Characters.TabMark;
			ListLevelProperties.DisplayFormatString = "{0}.";
			ListLevelProperties.RelativeRestartLevel = 0;
		}
		public string Text { get { return text; } set { text = value; } }
		public string Number { get { return numbers; } set { numbers = value; } }
		public string CreatDisplayFormatString() {
			List<int> placeholderIndices = CreatePlaceholderIndices();
			if (Text.Length == 0)
				return String.Empty;
			else
				return StringHelper.RemoveSpecialSymbols(CreateDisplayFormatStringCore(placeholderIndices));
		}
		protected internal virtual List<int> CreatePlaceholderIndices() {
			int count = Math.Min(9, Number.Length);
			List<int> result = new List<int>(count + 2);
			result.Add(0);
			for (int i = 0; i < count; i++)
				if ((int)Number[i] <= Text.Length)
					result.Add((int)Number[i]);
			result.Add(Text.Length);
			return result;
		}
		protected internal virtual string CreateDisplayFormatStringCore(List<int> placeholderIndices) {
			return ListLevelDisplayTextHelper.CreateDisplayFormatStringCore(placeholderIndices, Text);
		}
	}
	#endregion
	#region ListLevelDestination
	public class ListLevelDestination : DestinationBase {
		#region Fields
		internal static readonly List<NumberingFormat> numberingFormats = CreateNumberingFormatList();
		RtfListLevel level;
		#endregion
		#region CreateKeywordTable
		static KeywordTranslatorTable keywordHT = CreateKeywordTable();
		static KeywordTranslatorTable CreateKeywordTable() {
			KeywordTranslatorTable table = new KeywordTranslatorTable();
			table.Add("levelstartat", OnListLevelStartAtKeyword);
			table.Add("lvltentative", OnListLevelTentativeKeyword);
			table.Add("levelnfc", OnListLevelNumberingFormatKeyword);
			table.Add("leveljc", OnListLevelAlignmentKeyword);
			table.Add("levelnfcn", OnListLevelNumberingFormatKeyword);
			table.Add("leveljcn", OnListLevelAlignmentKeyword);
			table.Add("levelold", OnListLevelOldKeyword);
			table.Add("levelprev", OnListLevelPrevKeyword);
			table.Add("levelprevspace", OnListLevelPrevspaceKeyword);
			table.Add("levelindent", OnListLevelIndentKeyword);
			table.Add("levelspace", OnListLevelSpaceKeyword);
			table.Add("leveltext", OnListLevelTextKeyword);
			table.Add("levelnumbers", OnListLevelNumbersKeyword);
			table.Add("levelfollow", OnListLevelFollowKeyword);
			table.Add("levellegal", OnListLevelLegalKeyword);
			table.Add("levelnorestart", OnListLevelNoRestartKeyword);
			table.Add("levelpicture", OnListLevelPictureKeyword);
			table.Add("levelpicturenosize", OnListLevelPictureNoSizeKeyword);
			table.Add("s", OnParagraphStyleKeyword);
			DefaultDestination.AddParagraphPropertiesKeywords(table);
			DefaultDestination.AddCharacterPropertiesKeywords(table);
			return table;
		}
		#endregion
		public ListLevelDestination(RtfImporter rtfImporter)
			: base(rtfImporter) {
			this.level = new RtfListLevel(rtfImporter.DocumentModel);
		}
		#region Properties
		protected override ControlCharTranslatorTable ControlCharHT { get { return null; } }
		protected override KeywordTranslatorTable KeywordHT { get { return keywordHT; } }
		public RtfListLevel Level { get { return level; } set { level = value; } }
		#endregion
		#region CreateNumberingFormatList
		static List<NumberingFormat> CreateNumberingFormatList() {
			List<NumberingFormat> result = new List<NumberingFormat>();
			result.Add(NumberingFormat.Decimal);
			result.Add(NumberingFormat.UpperRoman);
			result.Add(NumberingFormat.LowerRoman);
			result.Add(NumberingFormat.UpperLetter);
			result.Add(NumberingFormat.LowerLetter);
			result.Add(NumberingFormat.Ordinal);
			result.Add(NumberingFormat.CardinalText);
			result.Add(NumberingFormat.OrdinalText);
			result.Add(NumberingFormat.None); 
			result.Add(NumberingFormat.None);
			result.Add(NumberingFormat.None); 
			result.Add(NumberingFormat.None);
			result.Add(NumberingFormat.AIUEOHiragana);
			result.Add(NumberingFormat.Iroha);
			result.Add(NumberingFormat.None);
			result.Add(NumberingFormat.None);
			result.Add(NumberingFormat.None);
			result.Add(NumberingFormat.None);
			result.Add(NumberingFormat.DecimalEnclosedCircle);
			result.Add(NumberingFormat.None);
			result.Add(NumberingFormat.AIUEOFullWidthHiragana);
			result.Add(NumberingFormat.IrohaFullWidth);
			result.Add(NumberingFormat.DecimalZero);
			result.Add(NumberingFormat.Bullet);
			result.Add(NumberingFormat.Ganada);
			result.Add(NumberingFormat.Chosung);
			result.Add(NumberingFormat.None);
			result.Add(NumberingFormat.None);
			result.Add(NumberingFormat.None);
			result.Add(NumberingFormat.None);
			result.Add(NumberingFormat.None);
			result.Add(NumberingFormat.None);
			result.Add(NumberingFormat.None);
			result.Add(NumberingFormat.None);
			result.Add(NumberingFormat.None);
			result.Add(NumberingFormat.None);
			result.Add(NumberingFormat.None);
			result.Add(NumberingFormat.None);
			result.Add(NumberingFormat.KoreanCounting);
			result.Add(NumberingFormat.KoreanDigital);
			result.Add(NumberingFormat.KoreanDigital2);
			result.Add(NumberingFormat.KoreanLegal);
			result.Add(NumberingFormat.Hebrew1);
			result.Add(NumberingFormat.ArabicAlpha);
			result.Add(NumberingFormat.Hebrew2);
			result.Add(NumberingFormat.ArabicAbjad);
			result.Add(NumberingFormat.HindiVowels);
			result.Add(NumberingFormat.HindiConsonants);
			result.Add(NumberingFormat.HindiNumbers);
			result.Add(NumberingFormat.HindiDescriptive);
			result.Add(NumberingFormat.ThaiLetters);
			result.Add(NumberingFormat.ThaiNumbers);
			result.Add(NumberingFormat.ThaiDescriptive);
			result.Add(NumberingFormat.VietnameseDescriptive);
			result.Add(NumberingFormat.NumberInDash);
			result.Add(NumberingFormat.RussianLower);
			result.Add(NumberingFormat.RussianUpper);
			result.Add(NumberingFormat.None);
			result.Add(NumberingFormat.None);
			result.Add(NumberingFormat.None);
			result.Add(NumberingFormat.None);
			result.Add(NumberingFormat.None);
			result.Add(NumberingFormat.None);
			result.Add(NumberingFormat.None);
			result.Add(NumberingFormat.None);
			result.Add(NumberingFormat.None);
			return result;
		}
		#endregion
		public override void NestedGroupFinished(DestinationBase nestedDestination) {
			TryToHandleFinishOfListLevelTextDestination(nestedDestination);
			TryToHandleFinishOfListLevelNumbersDestination(nestedDestination);
		}
		public override void BeforePopRtfState() {
			ApplyListLevelParagraphProperties();
			ApplyListLevelCharacterProperties();
		}
		protected internal virtual void TryToHandleFinishOfListLevelTextDestination(DestinationBase nestedDestination) {
			ListLevelDestination currentDestination = (ListLevelDestination)Importer.Destination;
			ListLevelTextDestination destination = nestedDestination as ListLevelTextDestination;
			if (destination != null) {
				currentDestination.Level.Text = destination.Value;
				currentDestination.Level.ListLevelProperties.TemplateCode = destination.LevelTemplateId;
			}
		}
		protected internal virtual void TryToHandleFinishOfListLevelNumbersDestination(DestinationBase nestedDestination) {
			ListLevelDestination currentDestination = (ListLevelDestination)Importer.Destination;
			ListLevelNumbersDestination destination = nestedDestination as ListLevelNumbersDestination;
			if (destination != null)
				currentDestination.Level.Number = destination.Value;			
		}
		protected internal virtual void ApplyListLevelParagraphProperties() {
			ListLevelDestination destination = (ListLevelDestination)Importer.Destination;
			ParagraphFormattingBase formatting = Importer.DocumentModel.DefaultParagraphProperties.Info;
			MergedParagraphProperties parentParagraphProperties = new MergedParagraphProperties(formatting.Info, formatting.Options);
			RtfParagraphFormattingInfo formattingInfo = Importer.Position.ParagraphFormattingInfo;
			Importer.ApplyLineSpacing(formattingInfo);
			Importer.ApplyParagraphProperties(destination.Level.ParagraphProperties, formattingInfo, parentParagraphProperties);
		}
		protected internal virtual void ApplyListLevelCharacterProperties() {
			ListLevelDestination destination = (ListLevelDestination)Importer.Destination;
			destination.Level.CharacterProperties.CopyFrom(Importer.Position.CharacterFormatting);
		}
		#region Keyword handlers
		static void OnListLevelStartAtKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			ListLevelDestination destination = (ListLevelDestination)importer.Destination;
			if (hasParameter)
				destination.Level.ListLevelProperties.Start = parameterValue;
		}
		static void OnListLevelTentativeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
		}
		static void OnListLevelNumberingFormatKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			ListLevelDestination destination = (ListLevelDestination)importer.Destination;
			if (hasParameter) {
				if (parameterValue >= 0 && parameterValue < numberingFormats.Count)
					destination.Level.ListLevelProperties.Format = numberingFormats[parameterValue];
			}
		}
		static void OnListLevelAlignmentKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			ListLevelDestination destination = (ListLevelDestination)importer.Destination;
			if (hasParameter)
				destination.Level.ListLevelProperties.Alignment = (ListNumberAlignment)parameterValue;
		}
		static void OnListLevelOldKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter || parameterValue != 0) {
				ListLevelDestination destination = (ListLevelDestination)importer.Destination;
				destination.Level.ListLevelProperties.Legacy = true;
			}
		}
		static void OnListLevelPrevKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
		}
		static void OnListLevelPrevspaceKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
		}
		static void OnListLevelIndentKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (hasParameter) {
				ListLevelDestination destination = (ListLevelDestination)importer.Destination;
				destination.Level.ListLevelProperties.LegacyIndent = importer.UnitConverter.TwipsToModelUnits(parameterValue);
			}
		}
		static void OnListLevelSpaceKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (hasParameter) {
				ListLevelDestination destination = (ListLevelDestination)importer.Destination;
				destination.Level.ListLevelProperties.LegacySpace = importer.UnitConverter.TwipsToModelUnits(parameterValue);
			}
		}
		static void OnListLevelTextKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Destination = new ListLevelTextDestination(importer);
		}
		static void OnListLevelNumbersKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Destination = new ListLevelNumbersDestination(importer);
		}
		static void OnListLevelFollowKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			ListLevelDestination destination = (ListLevelDestination)importer.Destination;
			if (hasParameter) {
				if (parameterValue == 0)
					destination.Level.ListLevelProperties.Separator = Characters.TabMark;
				else {
					if (parameterValue == 1)
						destination.Level.ListLevelProperties.Separator = ' ';
					else
						destination.Level.ListLevelProperties.Separator = '\u0000';
				}
			}
		}
		static void OnListLevelLegalKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			ListLevelDestination destination = (ListLevelDestination)importer.Destination;
			if (hasParameter)
				destination.Level.ListLevelProperties.ConvertPreviousLevelNumberingToDecimal = (parameterValue != 0);
		}
		static void OnListLevelNoRestartKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			ListLevelDestination destination = (ListLevelDestination)importer.Destination;
			if (hasParameter)
				destination.Level.ListLevelProperties.SuppressRestart = (parameterValue != 0);
		}
		static void OnListLevelPictureKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
		}
		static void OnListLevelPictureNoSizeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			ListLevelDestination destination = (ListLevelDestination)importer.Destination;
			destination.Level.ListLevelProperties.SuppressBulletResize = true;
		}
		static void OnParagraphStyleKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			ListLevelDestination destination = (ListLevelDestination)importer.Destination;
			if (importer.ParagraphStyleCollectionIndex.ContainsKey(parameterValue))
				destination.Level.ParagraphStyleIndex = importer.ParagraphStyleCollectionIndex[parameterValue];
		}
		#endregion
		protected override void ProcessControlCharCore(char ch) {
		}
		protected override bool ProcessKeywordCore(string keyword, int parameterValue, bool hasParameter) {
			TranslateKeywordHandler translator;
			if (KeywordHT.TryGetValue(keyword, out translator)) {
				translator(Importer, parameterValue, hasParameter);
				return true;
			}
			return false;
		}
		protected override void ProcessCharCore(char ch) {
		}
		protected override DestinationBase CreateClone() {
			ListLevelDestination clone = new ListLevelDestination(Importer);
			clone.level = this.level;
			return clone;
		}
	}
	#endregion
}
