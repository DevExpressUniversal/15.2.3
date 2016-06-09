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
using System.Globalization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Office;
using DevExpress.Office.NumberConverters;
using DevExpress.Utils;
namespace DevExpress.XtraRichEdit.Import.Html {
	#region NumberingListTagBase (abstract class)
	public abstract class NumberingListTagBase : TagBase {
		protected NumberingListTagBase(HtmlImporter importer)
			: base(importer) {
		}
		protected static string[] BulletTypes = { "disc", "circle", "square" };
		protected static HtmlListLevelProperties SetBulletNumberingFormat(string value, HtmlListLevelProperties listLevelProperties) {
			switch (value.ToLower(CultureInfo.InvariantCulture)) {
				case "disc":
					listLevelProperties.Format = NumberingFormat.Bullet;
					listLevelProperties.BulletFontName = "Symbol";
					break;
				case "circle":
					listLevelProperties.Format = NumberingFormat.Bullet;
					listLevelProperties.BulletFontName = "Courier New";
					break;
				case "square":
					listLevelProperties.Format = NumberingFormat.Bullet;
					listLevelProperties.BulletFontName = "Wingdings";
					break;
			}
			return listLevelProperties;
		}
		protected static HtmlListLevelProperties SetNumberigFormat(string value, HtmlListLevelProperties listLevelProperties) {
			switch (value) {
				case "A":
					listLevelProperties.Format = NumberingFormat.UpperLetter;
					break;
				case "a":
					listLevelProperties.Format = NumberingFormat.LowerLetter;
					break;
				case "I":
					listLevelProperties.Format = NumberingFormat.UpperRoman;
					break;
				case "i":
					listLevelProperties.Format = NumberingFormat.LowerRoman;
					break;
				case "1":
					listLevelProperties.Format = NumberingFormat.Decimal;
					break;
			}
			return listLevelProperties;
		}
		#region Properties
		protected internal virtual HtmlListLevelProperties ListLevelProperties { get { return new HtmlListLevelProperties(); } }
		#endregion
		protected internal override void ApplyTagProperties() {
			if (DocumentFormatsHelper.ShouldInsertNumbering(DocumentModel))
				Importer.Position.ListLevelProperties.CopyFrom(ListLevelProperties.MergeWith(Importer.Position.ListLevelProperties));
		}
		protected internal ParagraphFormattingOptions ApplyLeftIndent() {
			Importer.Position.ParagraphFormatting.FirstLineIndentType = ParagraphFirstLineIndent.Hanging;
			Importer.Position.ParagraphFormatting.FirstLineIndent = DocumentModel.DocumentProperties.DefaultTabWidth / 2;
			Importer.Position.ParagraphFormatting.LeftIndent += DocumentModel.DocumentProperties.DefaultTabWidth;
			ParagraphFormattingOptions options = new ParagraphFormattingOptions(ParagraphFormattingOptions.Mask.UseFirstLineIndent | ParagraphFormattingOptions.Mask.UseLeftIndent);
			return options;
		}
		protected internal override void FunctionalTagProcess() {
			if (Importer.DocumentModel.ActivePieceTable.Paragraphs[Importer.Position.ParagraphIndex].IsInList())
				Importer.IsEmptyParagraph = false;
			ParagraphFunctionalProcess();
		}
		protected internal override void OpenTagProcessCore() {
			if (!DocumentFormatsHelper.ShouldInsertNumbering(DocumentModel))
				return;
			if (Importer.Position.LevelIndex < 0)
				CreateNewAbstractNumberingList();
			ListLevel level = new ListLevel(DocumentModel);
			Importer.Position.LevelIndex = (DocumentFormatsHelper.ShouldInsertMultiLevelNumbering(DocumentModel)) ? Importer.Position.LevelIndex + 1 : 0;
			level.CopyFrom(DocumentModel.AbstractNumberingLists.Last.Levels[Importer.Position.LevelIndex]);
			level = Importer.Position.ListLevelProperties.ApplayPropertiesToListLevel(level);
			DocumentModel.AbstractNumberingLists.Last.Levels[Importer.Position.LevelIndex] = level;
		}
		protected internal abstract void CreateNewAbstractNumberingList();
		protected internal void CreateNewBulletAbstractNumberingList() {
			DefaultNumberingListHelper.InsertDefaultBulletNumberingList(DocumentModel, DocumentModel.UnitConverter, DocumentModel.DocumentProperties.DefaultTabWidth);
		}
		protected internal void CreateNewSimpleAbstractNumberingList() {
			DefaultNumberingListHelper.InsertDefaultSimpleNumberingList(DocumentModel, DocumentModel.UnitConverter, DocumentModel.DocumentProperties.DefaultTabWidth);
		}
		protected internal override void DeleteOldOpenTag() {
			int count = Importer.TagsStack.Count;
			for (int i = count - 1; i >= 0; i--) {
				TagBase tag = Importer.TagsStack[i].Tag;
				if (tag.Name == HtmlTagNameID.NumberingList || tag.Name == HtmlTagNameID.BulletList)
					return;
				if (Importer.TagsStack[i].Tag.Name == HtmlTagNameID.LI) {
					Importer.CloseUnClosedTag(new LevelTag(Importer), i);
					return;
				}
			}
		}
	}
	#endregion
	#region NumberingListTag
	public class NumberingListTag : NumberingListTagBase {
		HtmlListLevelProperties listLevelProperties;
		static AttributeKeywordTranslatorTable attributeTable = AddAttributes();
		static AttributeKeywordTranslatorTable AddAttributes() {
			AttributeKeywordTranslatorTable table = CreateAttributeTable();
			table.Add(ConvertKeyToUpper("type"), TypeAttributeKeyword);
			table.Add(ConvertKeyToUpper("start"), StartAttributeKeyword);
			return table;
		}
		static internal void TypeAttributeKeyword(HtmlImporter importer, string value, TagBase tag) {
			NumberingListTag numberingListTag = (NumberingListTag)tag;
			numberingListTag.listLevelProperties.CopyFrom(SetNumberigFormat(value, numberingListTag.listLevelProperties));
		}
		static internal void StartAttributeKeyword(HtmlImporter importer, string value, TagBase tag) {
			NumberingListTag numberingListTag = (NumberingListTag)tag;
			numberingListTag.listLevelProperties.Start = Convert.ToInt32(value);
		}
		HtmlListLevelProperties oldListLevelProperties;
		public NumberingListTag(HtmlImporter importer)
			: base(importer) {
			this.listLevelProperties = new HtmlListLevelProperties();
		}
		protected internal override void CreateNewAbstractNumberingList() {
			CreateNewSimpleAbstractNumberingList();
		}
		#region Properties
		protected internal override AttributeKeywordTranslatorTable AttributeTable { get { return attributeTable; } }
		protected internal override HtmlListLevelProperties ListLevelProperties { get { return listLevelProperties; } }
		#endregion
		protected internal override ParagraphFormattingOptions ApplyCssProperties() {
			ParagraphFormattingOptions options = new ParagraphFormattingOptions();
			if(DocumentFormatsHelper.ShouldInsertNumbering(DocumentModel))
				options.Value |= base.ApplyCssProperties().Value;
			if (DocumentFormatsHelper.ShouldInsertMultiLevelNumbering(DocumentModel))
				options.Value |= ApplyLeftIndent().Value;
			return options;
		}
		protected internal override void DeleteOldOpenTag() {
			if (this.Tag.ElementType == HtmlElementType.OpenTag)
				return;
			base.DeleteOldOpenTag();
		}
		protected internal override void ApplyProperties() {
			this.oldListLevelProperties = new HtmlListLevelProperties();
			this.oldListLevelProperties.CopyFrom(Importer.Position.ListLevelProperties);
			Importer.Position.ListLevelProperties.CopyFrom(new HtmlListLevelProperties());
			base.ApplyProperties();
		}
		protected internal override void BeforeDeleteTagFromStack(int indexOfDeletedTag) {
			NumberingListTag tag = Importer.TagsStack[indexOfDeletedTag].Tag as NumberingListTag;
			if (tag != null)
				Importer.Position.ListLevelProperties.CopyFrom(tag.oldListLevelProperties);
			base.BeforeDeleteTagFromStack(indexOfDeletedTag);
		}
	}
	#endregion
	#region BulletListTag
	public class BulletListTag : NumberingListTagBase {
		HtmlListLevelProperties listLevelProperties;
		static AttributeKeywordTranslatorTable attributeTable = AddAttributes();
		static AttributeKeywordTranslatorTable AddAttributes() {
			AttributeKeywordTranslatorTable table = CreateAttributeTable();
			table.Add(ConvertKeyToUpper("type"), TypeAttributeKeyword);
			return table;
		}
		static internal void TypeAttributeKeyword(HtmlImporter importer, string value, TagBase tag) {
			BulletListTag bulletListTag = (BulletListTag)tag;
			bulletListTag.listLevelProperties.CopyFrom(SetBulletNumberingFormat(value, bulletListTag.listLevelProperties));
		}
		protected internal override void CreateNewAbstractNumberingList() {
			CreateNewBulletAbstractNumberingList();
		}
		protected internal override HtmlListLevelProperties ListLevelProperties { get { return listLevelProperties; } }
		protected internal override AttributeKeywordTranslatorTable AttributeTable { get { return attributeTable; } }
		public BulletListTag(HtmlImporter importer)
			: base(importer) {
			this.listLevelProperties = new HtmlListLevelProperties();
		}
		protected internal override void ApplyTagProperties() {
			if (DocumentFormatsHelper.NeedReplaceBulletedLevelsToDecimal(DocumentModel)) {
				listLevelProperties.BulletFontName = DocumentModel.DefaultCharacterProperties.FontName;
				listLevelProperties.Format = NumberingFormat.Decimal;
			}
			else if (DocumentFormatsHelper.ShouldInsertBulletedNumbering(DocumentModel)) {
				if (listLevelProperties.Format != NumberingFormat.Bullet) {
					listLevelProperties.BulletFontName = "Symbol";
					listLevelProperties.Format = NumberingFormat.Bullet;
				}
			}
			else
				listLevelProperties.Format = NumberingFormat.None;
			base.ApplyTagProperties();
		}
		protected internal override ParagraphFormattingOptions ApplyCssProperties() {
			ParagraphFormattingOptions options = new ParagraphFormattingOptions();
			if (DocumentFormatsHelper.ShouldInsertNumbering(DocumentModel))
				options.Value |= base.ApplyCssProperties().Value;
			if (DocumentFormatsHelper.ShouldInsertMultiLevelNumbering(DocumentModel))
				options.Value |= ApplyLeftIndent().Value;
			return options;
		}
		protected internal override void DeleteOldOpenTag() {
			if (this.Tag.ElementType == HtmlElementType.OpenTag)
				return;
			base.DeleteOldOpenTag();
		}
	}
	#endregion
	#region LevelTag
	public class LevelTag : NumberingListTagBase {
		const int DefaultSpacing = 0;
		HtmlListLevelProperties listLevelProperties;
		static AttributeKeywordTranslatorTable attributeTable = AddAttributes();
		static AttributeKeywordTranslatorTable AddAttributes() {
			AttributeKeywordTranslatorTable table = CreateAttributeTable();
			table.Add(ConvertKeyToUpper("type"), TypeAttributeKeyword);
			table.Add(ConvertKeyToUpper("value"), ValueAttributeKeyword);
			return table;
		}
		static internal void TypeAttributeKeyword(HtmlImporter importer, string value, TagBase tag) {
			LevelTag levelTag = (LevelTag)tag;
			levelTag.listLevelProperties.CopyFrom(SetNumberigFormat(value, levelTag.listLevelProperties));
			levelTag.listLevelProperties.CopyFrom(SetBulletNumberingFormat(value, levelTag.listLevelProperties));
		}
		static internal void ValueAttributeKeyword(HtmlImporter importer, string value, TagBase tag) {
			LevelTag levelTag = (LevelTag)tag;
			try {
				levelTag.listLevelProperties.Start = Convert.ToInt32(value);
			} catch {
			}
		}
		protected internal override AttributeKeywordTranslatorTable AttributeTable { get { return attributeTable; } }
		public LevelTag(HtmlImporter importer)
			: base(importer) {
			this.listLevelProperties = new HtmlListLevelProperties();
		}
		protected internal override void FindKeywordInAttributeTable() {
			if (Importer.Position.LevelIndex >= 0 && Tag.ElementType == HtmlElementType.OpenTag && Importer.Position.ListLevelProperties.Format == NumberingFormat.Bullet) {
				Importer.Position.ListLevelProperties.UseBulletFontName = false;
				SetBulletNumberingFormat(BulletTypes[Importer.Position.LevelIndex % BulletTypes.Length], listLevelProperties);
			}
			base.FindKeywordInAttributeTable();
		}
		protected internal override void ApplyTagProperties() {
			ApplyAutoSpacing();
			Importer.Position.ListLevelProperties.CopyFrom(Importer.Position.ListLevelProperties.MergeWith(listLevelProperties));
		}
		void ApplyAutoSpacing() {
			ParagraphFormattingBase formatting = Importer.Position.ParagraphFormatting;
			formatting.SpacingAfter = DefaultSpacing;
			formatting.SpacingBefore = DefaultSpacing;
		}
		protected internal override ParagraphFormattingOptions ApplyCssProperties() {
			ParagraphFormattingOptions options = new ParagraphFormattingOptions();
			if (DocumentFormatsHelper.ShouldInsertMultiLevelNumbering(DocumentModel)) {
				int oldLeftIndentFromParentTags = Importer.Position.ParagraphFormatting.LeftIndent;
				options.Value |= base.ApplyCssProperties().Value;
				if ( Importer.Position.LevelIndex >= 0 && options.UseLeftIndent) {
					Importer.Position.ParagraphFormatting.FirstLineIndentType = ParagraphFirstLineIndent.Hanging;
					Importer.Position.ParagraphFormatting.FirstLineIndent = DocumentModel.DocumentProperties.DefaultTabWidth / 2;
					Importer.Position.ParagraphFormatting.LeftIndent = oldLeftIndentFromParentTags + Importer.Position.ParagraphFormatting.LeftIndent;
				}
			}
			return options;
		}
		protected internal override void FunctionalTagProcess() {
			if (Importer.IsEmptyListItem) {
				Importer.IsEmptyListItem = false;
				Importer.IsEmptyParagraph = false;
			}
			ParagraphFunctionalProcess();
		}
		protected internal override int GetStartIndexAllowedSearchScope() {
			int count = Importer.TagsStack.Count;
			for (int i = count - 1; i >= 0; i--) {
				TagBase tag = Importer.TagsStack[i].Tag;
				if (tag is NumberingListTag || tag is BulletListTag)
					return i;
			}
			return base.GetStartIndexAllowedSearchScope();
		}
		protected internal override void CreateNewAbstractNumberingList() {
			if (Importer.Position.ListLevelProperties.Format == NumberingFormat.Bullet)
				CreateNewBulletAbstractNumberingList();
			else
				CreateNewSimpleAbstractNumberingList();
		}
		protected internal override void OpenTagProcessCore() {
			if (!DocumentModel.DocumentCapabilities.ParagraphsAllowed || !DocumentFormatsHelper.ShouldInsertNumbering(DocumentModel))
				return;
			Paragraph paragraph = DocumentModel.MainPieceTable.Paragraphs[Importer.Position.ParagraphIndex];
			if (Importer.Position.LevelIndex < 0)
				Importer.AppendText(DocumentModel.MainPieceTable, Characters.Bullet.ToString() + "  ");
			else {
				IListLevel abstractLastLevel = DocumentModel.AbstractNumberingLists.Last.Levels[Importer.Position.LevelIndex];
				ListLevel level = new ListLevel(DocumentModel);
				level.ListLevelProperties.CopyFrom(abstractLastLevel.ListLevelProperties);
				level.CharacterProperties.BeginInit();
				level.CharacterProperties.CopyFrom(abstractLastLevel.CharacterProperties);
				level.CharacterProperties.EndInit();
				if (Importer.Position.ListLevelProperties.UseFormat)
					level.ListLevelProperties.Format = Importer.Position.ListLevelProperties.Format;
				if (Importer.Position.ListLevelProperties.UseStart)
					level.ListLevelProperties.Start = Importer.Position.ListLevelProperties.Start;
				if (!level.ListLevelProperties.Info.Equals(abstractLastLevel.ListLevelProperties.Info)) {
					if (IsPreviousCreatedListUsedBefore())
						CreateNewAbstractNumberingList();
					SetDisplayFormatString(level);
					DocumentModel.AbstractNumberingLists.Last.Levels[Importer.Position.LevelIndex] = level;
				}
				try {
					SetDisplayFormatString(abstractLastLevel);
					int levelIndex = Importer.Position.LevelIndex;
					if (!DocumentFormatsHelper.ShouldInsertMultiLevelNumbering(DocumentModel)) {
						levelIndex = 0;
						Importer.Position.LevelIndex = 0;
					}
					NumberingListIndex numberingListIndex = new NumberingListIndex(DocumentModel.NumberingLists.Count - 1);
					DocumentModel.MainPieceTable.AddNumberingListToParagraph(paragraph, numberingListIndex, levelIndex);
					MarkNumberingListUsed(numberingListIndex);
					Importer.SetAppendObjectProperty(); 
					Importer.IsEmptyListItem = true;
				} catch {
				}			   
			}
		}		
		bool IsPreviousCreatedListUsedBefore() {
			return Importer.UsedNumberingLists.Contains(new NumberingListIndex(DocumentModel.NumberingLists.Count - 1));
		}
		void MarkNumberingListUsed(NumberingListIndex index) {
			if (!Importer.UsedNumberingLists.Contains(index))
				Importer.UsedNumberingLists.Add(index);
		}
		protected internal void SetDisplayFormatString(IListLevel level) {
			if (DocumentFormatsHelper.NeedReplaceSimpleToBulletNumbering(DocumentModel)) {
				ReplaceSimpleNumberingToBulleted(level);
				return;
			}
			CorrectLevelDisplayFormatString(level);
		}
		protected internal virtual void ReplaceSimpleNumberingToBulleted(IListLevel level) {
#if !SL
			level.CharacterProperties.FontName = "Symbol";
#endif
			SetDotCharacterCore(level);
		}
		protected internal virtual void CorrectLevelDisplayFormatString(IListLevel level) {
			if (Importer.Position.ListLevelProperties.Format != NumberingFormat.Bullet)
				level.ListLevelProperties.DisplayFormatString = String.Format("{{{0}}}.", Importer.Position.LevelIndex);
			else
				SetDotCharacter(level);
		}
		void SetDotCharacter(IListLevel level) {
			level.CharacterProperties.FontName = Importer.Position.ListLevelProperties.BulletFontName;
			SetDotCharacterCore(level);
		}
		void SetDotCharacterCore(IListLevel level) {
			if (level.CharacterProperties.FontName.ToUpper(CultureInfo.InvariantCulture) == "COURIER NEW")
				level.ListLevelProperties.DisplayFormatString = "\u006F";
			else if (level.CharacterProperties.FontName.ToUpper(CultureInfo.InvariantCulture) == "WINGDINGS")
				level.ListLevelProperties.DisplayFormatString = "\u00A7";
			else {
#if SL
				level.ListLevelProperties.DisplayFormatString = "\u2022";
#else
				level.ListLevelProperties.DisplayFormatString = Characters.MiddleDot.ToString();
#endif
			}
		}
	}
	#endregion
	#region DtTag
	public class DtTag : TagBase {
		public DtTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyTagProperties() {
		}
	}
	#endregion
	#region DlTag
	public class DlTag : TagBase {
		public DlTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyTagProperties() {
		}
	}
	#endregion
	#region DdTag
	public class DdTag : TagBase {
		public DdTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyTagProperties() {
		}
	}
	#endregion
}
