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
using DevExpress.Office.Model;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Office.NumberConverters;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Import.Rtf {
	#region TextBeforeDestination
	public class TextBeforeDestination : StringValueDestination {
		public TextBeforeDestination(RtfImporter importer)			
			: base(importer) {			
		}
		protected internal override StringValueDestination CreateEmptyClone() {
			return new TextBeforeDestination(Importer);
		}
	}
	#endregion
	#region TextAfterDestination
	public class TextAfterDestination : StringValueDestination {
		public TextAfterDestination(RtfImporter importer)
			: base(importer) {
		}
		protected internal override StringValueDestination CreateEmptyClone() {
			return new TextAfterDestination(Importer);
		}
	}
	#endregion
	public abstract class DestinationOldParagraphNumberingBase : DestinationBase {
		#region AppendParagraphNumberingDescKeywords
		protected static void AppendParagraphNumberingDescKeywords(KeywordTranslatorTable table) {
			table.Add("pncard", OnCardinalKeyword);
			table.Add("pndec", OnDecimalKeyword);
			table.Add("pnucltr", OnUpperCaseAlphabeticalKeyword);
			table.Add("pnucrm", OnUpperCaseRomanKeyword);
			table.Add("pnlcltr", OnLowerCaseAlphabeticalKeyword);
			table.Add("pnlcrm", OnLowerCaseRomanKeyword);
			table.Add("pnord", OnOrdinalKeyword);
			table.Add("pnordt", OnOrdinalTextKeyword);
			table.Add("pncnum", OnNumberingInCircleKeyword);
			table.Add("pnuldash", OnDashedUndrelineKeyword);
			table.Add("pnuldashd", OnDashDottedUnderlineKeyword);
			table.Add("pnuldashdd", OnDashDotDottedUnderlineKeyword);
			table.Add("pnulhair", OnHairlineUnderlineKeyword);
			table.Add("pnulth", OnThickUnderlineKeyword);
			table.Add("pnulwave", OnWaveUnderlineKeyword);
			table.Add("pnul", OnContinuousUnderlineKeyword);
			table.Add("pnuld", OnDottedUnderlineKeyword);
			table.Add("pnuldb", OnDoubleUnderlineKeyword);
			table.Add("pnulnone", OnNoneUnderlineKeyword);
			table.Add("pnulw", OnWordUnderlineKeyword);
			table.Add("pnf", OnFontNumberKeyword);
			table.Add("pnfs", OnFontSizeKeyword);
			table.Add("pnb", OnFontBoldKeyword);
			table.Add("pni", OnItalicKeyword);
			table.Add("pncaps", OnAllCapsKeyword);
			table.Add("pnscaps", OnSmallCapsKeyword);
			table.Add("pnstrike", OnStrikeKeyword);
			table.Add("pncf", OnForegroundColorKeyword);
			table.Add("pnindent", OnIndentKeyword);
			table.Add("pnsp", OnSpaceKeyword);
			table.Add("pnprev", OnUsePrevKeyword);
			table.Add("pnstart", OnStartAtKeyword);
			table.Add("pnhang", OnHangingIndentKeyword);
			table.Add("pnrestart", OnRestartOnSectionBreakKeyword);
			table.Add("pnqc", OnCenterAlignmentKeyword);
			table.Add("pnql", OnLeftAlignmentKeyword);
			table.Add("pnqr", OnRightAlignmentKeyword);
			table.Add("pntxtb", OnTextBeforeKeyword);
			table.Add("pntxta", OnTextAfterKeyword);
		}
		#endregion
		#region processing control char and keyword
		static void SetNumberingListFormat(RtfImporter importer, NumberingFormat format) {
			importer.Position.OldListLevelInfo.ListLevelProperties.Format = format;
		}
		static void SetUnderlineType(RtfImporter importer, UnderlineType underlineType) {
			importer.Position.CharacterFormatting.FontUnderlineType = underlineType;
		}
		static void OnCardinalKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			SetNumberingListFormat(importer, NumberingFormat.CardinalText);
		}
		static void OnDecimalKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			SetNumberingListFormat(importer, NumberingFormat.Decimal);
		}
		static void OnUpperCaseAlphabeticalKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			SetNumberingListFormat(importer, NumberingFormat.UpperLetter);
		}
		static void OnUpperCaseRomanKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			SetNumberingListFormat(importer, NumberingFormat.UpperRoman);
		}
		static void OnLowerCaseAlphabeticalKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			SetNumberingListFormat(importer, NumberingFormat.LowerLetter);
		}
		static void OnLowerCaseRomanKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			SetNumberingListFormat(importer, NumberingFormat.LowerRoman);
		}
		static void OnOrdinalKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			SetNumberingListFormat(importer, NumberingFormat.Ordinal);
		}
		static void OnOrdinalTextKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			SetNumberingListFormat(importer, NumberingFormat.OrdinalText);
		}
		static void OnNumberingInCircleKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			SetNumberingListFormat(importer, NumberingFormat.DecimalEnclosedCircle);
		}
		static void OnDashedUndrelineKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			SetUnderlineType(importer, UnderlineType.Dashed);
		}
		static void OnDashDottedUnderlineKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			SetUnderlineType(importer, UnderlineType.DashDotted);
		}
		static void OnDashDotDottedUnderlineKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			SetUnderlineType(importer, UnderlineType.DashDotDotted);
		}
		static void OnHairlineUnderlineKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			SetUnderlineType(importer, UnderlineType.ThickSingle);
		}
		static void OnThickUnderlineKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			SetUnderlineType(importer, UnderlineType.ThickSingle);
		}
		static void OnWaveUnderlineKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			SetUnderlineType(importer, UnderlineType.Wave);
		}		
		static void OnDottedUnderlineKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			SetUnderlineType(importer, UnderlineType.Dotted);
		}
		static void OnDoubleUnderlineKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			SetUnderlineType(importer, UnderlineType.Double);
		}
		static void OnNoneUnderlineKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			SetUnderlineType(importer, UnderlineType.None);
		}
		static void OnContinuousUnderlineKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.CharacterFormatting.UnderlineWordsOnly = false;
		}
		static void OnWordUnderlineKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.CharacterFormatting.UnderlineWordsOnly = true;
		}
		static void OnFontNumberKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
		   if (!hasParameter)
				parameterValue = importer.DocumentProperties.DefaultFontNumber;
			RtfDocumentProperties props = importer.DocumentProperties;
			importer.SetFont(props.Fonts.GetRtfFontInfoById(parameterValue));
		}
		static void OnFontSizeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				parameterValue = 24;
			importer.Position.CharacterFormatting.DoubleFontSize = Math.Max(PredefinedFontSizeCollection.MinFontSize, parameterValue );
		}
		static void OnFontBoldKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			bool val = hasParameter ? parameterValue != 0 : true;
			importer.Position.CharacterFormatting.FontBold = val;
		}
		static void OnItalicKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			bool val = hasParameter ? parameterValue != 0 : true;
			importer.Position.CharacterFormatting.FontItalic = val;
		}
		static void OnAllCapsKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			bool val = hasParameter ? parameterValue != 0 : true;
			importer.Position.CharacterFormatting.AllCaps = val;
		}
		static void OnSmallCapsKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			bool val = hasParameter ? parameterValue != 0 : true;
			importer.Position.CharacterFormatting.AllCaps = !val;
		}
		static void OnStrikeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			bool val = hasParameter ? parameterValue != 0 : true;
			importer.Position.CharacterFormatting.FontStrikeoutType = val ? StrikeoutType.Single : StrikeoutType.None;
		}		
		static void OnForegroundColorKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				parameterValue = 0;
			RtfDocumentProperties props = importer.DocumentProperties;
			Color foreColor = props.Colors.GetRtfColorById(parameterValue);
			importer.Position.CharacterFormatting.ForeColor = foreColor;
		}
		static void OnIndentKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			RtfParagraphFormattingInfo info = importer.Position.ParagraphFormattingInfo;
			if (parameterValue < 0) {
				info.FirstLineIndent = importer.UnitConverter.TwipsToModelUnits(-parameterValue);
				info.FirstLineIndentType = ParagraphFirstLineIndent.Hanging;
			}
			else {
				info.FirstLineIndent = importer.UnitConverter.TwipsToModelUnits(parameterValue);
				info.FirstLineIndentType = ParagraphFirstLineIndent.Indented;
			}
		}
		static void OnSpaceKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
		}
		static void OnUsePrevKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.OldListLevelInfo.IncludeInformationFromPreviousLevel = true;			
		}
		static void OnStartAtKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.OldListLevelInfo.ListLevelProperties.Start = parameterValue;
		}
		static void OnHangingIndentKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.ParagraphFormattingInfo.FirstLineIndentType = ParagraphFirstLineIndent.Hanging;
		}
		static void OnRestartOnSectionBreakKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
		}
		static void OnCenterAlignmentKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.OldListLevelInfo.ListLevelProperties.Alignment = ListNumberAlignment.Center;
		}
		static void OnLeftAlignmentKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.OldListLevelInfo.ListLevelProperties.Alignment = ListNumberAlignment.Left;
		}
		static void OnRightAlignmentKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.OldListLevelInfo.ListLevelProperties.Alignment = ListNumberAlignment.Right;
		}
		static void OnTextBeforeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Destination = new TextBeforeDestination(importer);
		}
		static void OnTextAfterKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Destination = new TextAfterDestination(importer);
		}
		#endregion        
		string textBefore;
		string textAfter;
		protected DestinationOldParagraphNumberingBase(RtfImporter importer)
			: base(importer) {
		}
		protected override ControlCharTranslatorTable ControlCharHT { get { return null; } }
		protected virtual string TextBefore { get { return textBefore; } set { textBefore = value; } }
		protected virtual string TextAfter { get { return textAfter; } set { textAfter = value; } }
		public override void NestedGroupFinished(DestinationBase nestedDestination) {
			base.NestedGroupFinished(nestedDestination);
			TextBeforeDestination textBeforeDestination = nestedDestination as TextBeforeDestination;
			if (textBeforeDestination != null) {
				TextBefore = textBeforeDestination.Value;
				return;
			}
			TextAfterDestination textAfterDestination = nestedDestination as TextAfterDestination;
			if (textAfterDestination != null) {
				TextAfter = textAfterDestination.Value;
				return;
			}
		}
		public override void BeforePopRtfState() {
			base.BeforePopRtfState();
			Importer.Position.OldListLevelInfo.TextAfter = TextAfter;
			Importer.Position.OldListLevelInfo.TextBefore = TextBefore;
		}
	}
	public class DestinationOldSectionNumberingLevel : DestinationOldParagraphNumberingBase {
		#region CreateKeywordTable
		static KeywordTranslatorTable keywordHT = CreateKeywordTable();
		static KeywordTranslatorTable CreateKeywordTable() {
			KeywordTranslatorTable table = new KeywordTranslatorTable();
			AppendParagraphNumberingDescKeywords(table);
			return table;
		}
		#endregion
		readonly int levelNumber;
		protected override KeywordTranslatorTable KeywordHT { get { return keywordHT; } }
		public DestinationOldSectionNumberingLevel(RtfImporter importer, int levelNumber)
			: base(importer) {
			importer.Position.OldListLevelInfo = new RtfOldListLevelInfo(importer.DocumentModel);
			this.levelNumber = levelNumber;
		}
		protected int LevelNumber { get { return levelNumber; } }
		protected override DestinationBase CreateClone() {
			DestinationOldSectionNumberingLevel result = new DestinationOldSectionNumberingLevel(Importer, LevelNumber);
			return result;
		}
		public override void BeforePopRtfState() {
			base.BeforePopRtfState();			
			Importer.Position.OldListLevelInfoCollection[LevelNumber].CopyFrom(CreateRtfOldListLevelInfo());
		}
		protected virtual RtfOldListLevelInfo CreateRtfOldListLevelInfo() {
			RtfOldListLevelInfo result = new RtfOldListLevelInfo(Importer.DocumentModel);
			result.CopyFrom(Importer.Position.OldListLevelInfo);
			result.CharacterProperties.CopyFrom(Importer.Position.CharacterFormatting);
			result.Indent = Importer.Position.ParagraphFormattingInfo.FirstLineIndent;
			return result;
		}
	}
	public class DestinationOldParagraphNumbering : DestinationOldParagraphNumberingBase {
		#region CreateKeywordTable
		static KeywordTranslatorTable keywordHT = CreateKeywordTable();
		static KeywordTranslatorTable CreateKeywordTable() {
			KeywordTranslatorTable table = new KeywordTranslatorTable();
			AppendParagraphNumberingDescKeywords(table);
			table.Add("pnlvl", OnParagraphLevelKeyword);
			table.Add("pnlvlblt", OnBulletedParagraphKeyword);
			table.Add("pnlvlbody", OnSimpleNumberingKeyword);
			table.Add("pnlvlcont", OnSkipNumberingKeyword);			
			table.Add("ilvl", OnListLevelKeyword);
			table.Add("ls", OnListOverrideKeyword);
			return table;
		}
		#endregion
		#region processing control char and keyword
		static void OnParagraphLevelKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (hasParameter && parameterValue == 10) {
				OnSimpleNumberingKeyword(importer, 0, false);
				return;
			}
			((DestinationOldParagraphNumbering)importer.Destination).OldLevelNumber = parameterValue;
			((DestinationOldParagraphNumbering)importer.Destination).SimpleList = false;			
			importer.Position.OldListLevelInfo.CopyFrom(importer.Position.OldListLevelInfoCollection[parameterValue]);
		}
		static void OnBulletedParagraphKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.OldListLevelInfo.ListLevelProperties.Format = NumberingFormat.Bullet;
		}
		static void OnSimpleNumberingKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			((DestinationOldParagraphNumbering)importer.Destination).SimpleList = true;
		}
		static void OnSkipNumberingKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.OldListLevelInfo.SkipNumbering = true;
		}
		internal static void OnListOverrideKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			NumberingListIndex index;
			if (importer.ListOverrideIndexToNumberingListIndexMap.TryGetValue(parameterValue, out index)) 
				importer.Position.ParagraphFormattingInfo.NumberingListIndex = index;
			((DestinationOldParagraphNumbering)importer.Destination).ExplicitNumberingListIndex = true;
		}
		internal static void OnListLevelKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.ParagraphFormattingInfo.ListLevelIndex = parameterValue;
			((DestinationOldParagraphNumbering)importer.Destination).ExplicitListLevelIndex = true;
		}		
		#endregion
		int oldLevelNumber;
		NumberingListIndex simpleListIndex;
		NumberingListIndex multiLevelListIndex;
		int listLevelIndex;
		bool isOldNumberingListCreated;
		bool skipNumbering;
		bool simpleList;
		protected override KeywordTranslatorTable KeywordHT { get { return keywordHT; } }
		public DestinationOldParagraphNumbering(RtfImporter importer)
			: base(importer) {
			this.oldLevelNumber = -1;
			if(importer.Position.OldListLevelInfo == null)
				importer.Position.OldListLevelInfo = new RtfOldListLevelInfo(importer.DocumentModel);
			if(importer.Position != null)
				importer.Position.CharacterFormatting.ResetUse(CharacterFormattingOptions.Mask.UseAll);
			this.multiLevelListIndex = NumberingListIndex.ListIndexNotSetted;
			this.simpleListIndex = NumberingListIndex.ListIndexNotSetted;
		}
		protected int OldLevelNumber { get { return oldLevelNumber; } set { oldLevelNumber = value; } }
		protected NumberingListIndex MultiLevelListIndex { get { return multiLevelListIndex; } set { multiLevelListIndex = value; } }
		protected NumberingListIndex SimpleListIndex { get { return simpleListIndex; } set { simpleListIndex = value; } }
		protected bool IsOldNumberingListCreated { get { return isOldNumberingListCreated; } set { isOldNumberingListCreated = value; } }
		protected int ListLevelIndex { get { return listLevelIndex; } set { listLevelIndex = value; } }
		protected bool SkipNumbering { get { return skipNumbering; } }
		protected bool SimpleList { get { return simpleList; } set { simpleList = value; } }
		protected bool ExplicitNumberingListIndex { get; set; }
		protected bool ExplicitListLevelIndex { get; set; }
		protected NumberingListIndex NumberingListIndex { get; set; }
		protected override DestinationBase CreateClone() {
			DestinationOldParagraphNumbering result = new DestinationOldParagraphNumbering(Importer);
			return result;
		}
		public override void BeforePopRtfState() {
			base.BeforePopRtfState();
			if (ShouldCreateNewList()) {
				CreateNewList();
			}
			else {
				skipNumbering = IsSkipNumbering();
				SimpleListIndex = Importer.Position.CurrentOldSimpleListIndex;
				MultiLevelListIndex = Importer.Position.CurrentOldMultiLevelListIndex;
			}
			if (ExplicitNumberingListIndex && ExplicitListLevelIndex) {
				ListLevelIndex = Importer.Position.ParagraphFormattingInfo.ListLevelIndex;
				NumberingListIndex = Importer.Position.ParagraphFormattingInfo.NumberingListIndex;
			}
			else
				ListLevelIndex = OldLevelNumber >= 0 ? OldLevelNumber - 1 : 0;
		}
		public override void AfterPopRtfState() {
			base.AfterPopRtfState();
			Importer.Position.ParagraphFormattingInfo.ListLevelIndex = ListLevelIndex;
			NumberingListIndex actualNumberingListIndex =  IsSimpleList() ? SimpleListIndex : MultiLevelListIndex;
			Importer.Position.CurrentOldListSkipNumbering = SkipNumbering;
			if (IsOldNumberingListCreated) {
				Importer.Position.CurrentOldMultiLevelListIndex = MultiLevelListIndex;
				Importer.Position.CurrentOldSimpleListIndex = SimpleListIndex;
				Importer.Position.CurrentOldSimpleList = SimpleList;
			}
			if (ExplicitListLevelIndex && ExplicitNumberingListIndex) {
				Importer.Position.ParagraphFormattingInfo.NumberingListIndex = NumberingListIndex;
				Importer.Position.ParagraphFormattingInfo.ListLevelIndex = ListLevelIndex;
			}
			else
				Importer.Position.ParagraphFormattingInfo.NumberingListIndex = !SkipNumbering ? actualNumberingListIndex : NumberingListIndex.NoNumberingList;
		}
		protected virtual void CreateNewList() {
			RtfOldListLevelInfo oldListLevelInfo = Importer.Position.OldListLevelInfo;
			if (SimpleList) {
				CreateSimpleNumberingListLevels();
				SimpleListIndex = new NumberingListIndex(Importer.DocumentModel.NumberingLists.Count - 1);				
				MultiLevelListIndex = Importer.Position.CurrentOldMultiLevelListIndex;
			}
			else if (oldListLevelInfo.ListLevelProperties.Format == NumberingFormat.Bullet) {
				CreateBulletedListLevels();
				MultiLevelListIndex = new NumberingListIndex(Importer.DocumentModel.NumberingLists.Count - 1);
				SimpleListIndex = Importer.Position.CurrentOldSimpleListIndex;
			}
			else {
				CreateMultilevelListLevels();
				MultiLevelListIndex = new NumberingListIndex(Importer.DocumentModel.NumberingLists.Count - 1);
				SimpleListIndex = Importer.Position.CurrentOldSimpleListIndex;
			}
		}
		private void CreateMultilevelListLevels() {
			DocumentModel documentModel = Importer.DocumentModel;
			AbstractNumberingList abstractNumberingList = new AbstractNumberingList(documentModel);
			documentModel.AddAbstractNumberingListUsingHistory(abstractNumberingList);
			for (int i = 0; i < abstractNumberingList.Levels.Count; i++) {
				RtfOldListLevelInfo levelInfo = Importer.Position.OldListLevelInfoCollection[i + 1];
				ListLevel level = new ListLevel(documentModel);
				abstractNumberingList.Levels[i] = level;
				int firstLineIndent = levelInfo.Indent;
				SetFirstLineIndent(abstractNumberingList.Levels[i], firstLineIndent);
				level.CharacterProperties.BeginInit();
				level.CharacterProperties.CopyFrom(Importer.Position.CharacterFormatting);
				level.CharacterProperties.EndInit();
				string formatString = String.Format("{0}{{{1}}}{2}", levelInfo.TextBefore, i, levelInfo.TextAfter);
				if (i > 0 && levelInfo.IncludeInformationFromPreviousLevel)
					formatString = abstractNumberingList.Levels[i - 1].ListLevelProperties.DisplayFormatString + formatString;
				SetDisplayFormatString(level, formatString);
				SetLegacyProperties(level, 0, 0);
			}
			isOldNumberingListCreated = true;
			documentModel.AddNumberingListUsingHistory(new NumberingList(documentModel, new AbstractNumberingListIndex(documentModel.AbstractNumberingLists.Count - 1)));
		}
		private void CreateBulletedListLevels() {
			DocumentModel documentModel = Importer.DocumentModel;
			int levelOffset = documentModel.UnitConverter.DocumentsToModelUnits(150);
			AbstractNumberingList abstractNumberingList = new AbstractNumberingList(documentModel);
			documentModel.AddAbstractNumberingListUsingHistory(abstractNumberingList);
			for (int i = 0; i < abstractNumberingList.Levels.Count; i++) {
				ListLevel level = new ListLevel(documentModel);
				abstractNumberingList.Levels[i] = level;
				level.CharacterProperties.BeginInit();
				level.CharacterProperties.CopyFrom(Importer.Position.CharacterFormatting);
				level.CharacterProperties.EndInit();
				int firstLineIndent = levelOffset * i + Importer.Position.ParagraphFormattingInfo.FirstLineIndent;
				SetFirstLineIndent(level, firstLineIndent);
				SetDisplayFormatString(level, String.Format("{0}{1}", Importer.Position.OldListLevelInfo.TextBefore, Importer.Position.OldListLevelInfo.TextAfter));
				SetTemplateCode(level, NumberingListHelper.GenerateNewTemplateCode(documentModel));
				SetLegacyProperties(level, 0, 0);
			}
			isOldNumberingListCreated = true;
			documentModel.AddNumberingListUsingHistory(new NumberingList(documentModel, new AbstractNumberingListIndex(documentModel.AbstractNumberingLists.Count - 1)));
		}
		private void CreateSimpleNumberingListLevels() {
			DocumentModel documentModel = Importer.DocumentModel;
			NumberingListIndex existingNumberingListIndex;
			if (ShouldCreateNewAbstractSimpleList(out existingNumberingListIndex)) {
				AbstractNumberingList abstractNumberingList = new AbstractNumberingList(documentModel);
				documentModel.AddAbstractNumberingListUsingHistory(abstractNumberingList);
				for (int i = 0; i < abstractNumberingList.Levels.Count; i++) {
					ListLevel level = new ListLevel(documentModel);
					abstractNumberingList.Levels[i] = level;
					level.CharacterProperties.BeginInit();
					level.CharacterProperties.CopyFrom(Importer.Position.CharacterFormatting);
					level.CharacterProperties.EndInit();
					int firstLineIndent = Importer.Position.ParagraphFormattingInfo.FirstLineIndent + 150 * i;
					SetFirstLineIndent(level, firstLineIndent);
					SetDisplayFormatString(level, String.Format("{0}{{{1}}}{2}", Importer.Position.OldListLevelInfo.TextBefore, i, Importer.Position.OldListLevelInfo.TextAfter));
					SetTemplateCode(level, NumberingListHelper.GenerateNewTemplateCode(documentModel));
					SetLegacyProperties(level, 0, 0);
					int start = Importer.Position.OldListLevelInfo.ListLevelProperties.Start;
					if(start > 0)
						level.ListLevelProperties.Start = start;
					NumberingFormat numberingFormat = Importer.Position.OldListLevelInfo.ListLevelProperties.Format;
					if(numberingFormat != NumberingFormat.Decimal)
						level.ListLevelProperties.Format = numberingFormat;
				}
				isOldNumberingListCreated = true;
				documentModel.AddNumberingListUsingHistory(new NumberingList(documentModel, new AbstractNumberingListIndex(documentModel.AbstractNumberingLists.Count - 1)));
				Importer.NumberingListToOldListLevelInfoMap.Add(new NumberingListIndex(documentModel.NumberingLists.Count - 1), Importer.Position.OldListLevelInfo.Clone());
			}
			else {
				AbstractNumberingListIndex abstractNumberingListIndex = documentModel.NumberingLists[existingNumberingListIndex].AbstractNumberingListIndex;
				NumberingList prevNumberingList = documentModel.NumberingLists.Last;
				NumberingList newList = new NumberingList(documentModel, abstractNumberingListIndex);
				OverrideListLevel level = new OverrideListLevel(documentModel);
				int start = Importer.Position.OldListLevelInfo.ListLevelProperties.Start;
				if (start >= 0)
					level.ListLevelProperties.Start = start;
				NumberingFormat numberingFormat = Importer.Position.OldListLevelInfo.ListLevelProperties.Format;
				if(numberingFormat != NumberingFormat.Decimal)
					level.ListLevelProperties.Format = numberingFormat;
				if(prevNumberingList != null && prevNumberingList.Levels[0].ListLevelProperties.Format != level.ListLevelProperties.Format)
					level.SetOverrideStart(true);
				level.CharacterProperties.BeginInit();
				level.CharacterProperties.CopyFrom(Importer.Position.CharacterFormatting);
				level.CharacterProperties.EndInit();
				int firstLineIndent = Importer.Position.ParagraphFormattingInfo.FirstLineIndent;
				SetFirstLineIndent(level, firstLineIndent);
				SetDisplayFormatString(level, String.Format("{0}{{{1}}}{2}", Importer.Position.OldListLevelInfo.TextBefore, 0, Importer.Position.OldListLevelInfo.TextAfter));
				SetTemplateCode(level, NumberingListHelper.GenerateNewTemplateCode(documentModel));
				SetLegacyProperties(level, 0, 0);
				newList.Levels[0] = level;
				documentModel.AddNumberingListUsingHistory(newList);
				Importer.NumberingListToOldListLevelInfoMap.Add(new NumberingListIndex(documentModel.NumberingLists.Count - 1), Importer.Position.OldListLevelInfo.Clone());
			}
		}
		protected virtual bool ShouldCreateNewAbstractSimpleList(out NumberingListIndex existingNumberingListIndex) {
			if (Importer.Position.CurrentOldSimpleList && Importer.Position.CurrentOldSimpleListIndex >= NumberingListIndex.MinValue) {
				RtfOldListLevelInfo oldListLevelInfo;
				existingNumberingListIndex = Importer.Position.CurrentOldSimpleListIndex;
				if (!Importer.NumberingListToOldListLevelInfoMap.TryGetValue(existingNumberingListIndex, out oldListLevelInfo))
					return true;
				return !AreSameInfo(oldListLevelInfo, Importer.Position.OldListLevelInfo);
			}
			existingNumberingListIndex = NumberingListIndex.ListIndexNotSetted;
			ParagraphIndex prevParagraphIndex = Importer.Position.ParagraphIndex - 1;
			for(; prevParagraphIndex >= ParagraphIndex.Zero; prevParagraphIndex--) {
				Paragraph prevParagraph = Importer.Position.PieceTable.Paragraphs[prevParagraphIndex];
				if (!prevParagraph.IsInList()) {
					if(prevParagraph.NumberingListIndex == NumberingListIndex.NoNumberingList)
						continue;
					return true;
				}
				NumberingListIndex numberingListIndex = prevParagraph.NumberingListIndex;
				RtfOldListLevelInfo prevOldListLevelInfo;
				if (!Importer.NumberingListToOldListLevelInfoMap.TryGetValue(numberingListIndex, out prevOldListLevelInfo))
					return true;
				if (!AreSameInfo(prevOldListLevelInfo, Importer.Position.OldListLevelInfo))
					return true;
				existingNumberingListIndex = numberingListIndex;
				return false;
			}
			return true;
		}
		protected internal void SetLegacyProperties(IListLevel level, int legacyIndent, int legacySpace) {
			level.ListLevelProperties.BeginInit();
			try {
				level.ListLevelProperties.Legacy = true;
				level.ListLevelProperties.LegacySpace = legacyIndent;
				level.ListLevelProperties.LegacyIndent = legacySpace;
			}
			finally {
				level.ListLevelProperties.EndInit();
			}
		}
		protected internal void SetDisplayFormatString(IListLevel level, string displayFormatString) {
			level.ListLevelProperties.BeginInit();
			try {
				level.ListLevelProperties.DisplayFormatString = displayFormatString;
			}
			finally {
				level.ListLevelProperties.EndInit();
			}
		}
		protected internal void SetFirstLineIndent(IListLevel level, int lineIndent) {
			level.ParagraphProperties.BeginInit();
			try {
				level.ParagraphProperties.LeftIndent = lineIndent;
			}
			finally {
				level.ParagraphProperties.EndInit();
			}
		}
		protected internal void SetTemplateCode(IListLevel level, int templateCode) {
			level.ListLevelProperties.BeginInit();
			try {
				level.ListLevelProperties.TemplateCode = templateCode;
			}
			finally {
				level.ListLevelProperties.EndInit();
			}
		}
		protected virtual bool ShouldCreateNewList() {
			if (IsNewListLevelInfoPresent())
				return false;
			if(IsMultilevelList())
				return (!SectionMultiLevelListCreated());
			if (IsSimpleList())
				return true;
			if (IsSkipNumbering())
				return false;
			return true;
		}
		protected virtual bool IsNewListLevelInfoPresent() {
			if (ExplicitListLevelIndex && ExplicitNumberingListIndex)
				return true;
			if (IsSimpleList()) {
				if (SimpleListIndex < NumberingListIndex.MinValue)
					return false;
				RtfOldListLevelInfo existingListLevelInfo;
				if(!Importer.NumberingListToOldListLevelInfoMap.TryGetValue(simpleListIndex, out existingListLevelInfo))
					return false;
				return AreSameInfo(existingListLevelInfo, Importer.Position.OldListLevelInfo);
			}
			else
				return MultiLevelListIndex >= NumberingListIndex.MinValue;
		}
		private bool AreSameInfo(RtfOldListLevelInfo existingListLevelInfo, RtfOldListLevelInfo rtfOldListLevelInfo) {
			return existingListLevelInfo.TextAfter == rtfOldListLevelInfo.TextAfter && existingListLevelInfo.TextBefore == rtfOldListLevelInfo.TextBefore;
		}
		protected virtual bool IsSimpleList() {
			return SimpleList;
		}
		protected virtual bool IsMultilevelList() {
			return (OldLevelNumber >= 0) && !SimpleList;
		}
		protected virtual bool SectionMultiLevelListCreated() {
			return Importer.Position.CurrentOldMultiLevelListIndex >= NumberingListIndex.MinValue;
		}
		protected virtual bool IsSkipNumbering() {
			return Importer.Position.OldListLevelInfo.SkipNumbering;
		}
	}
}
