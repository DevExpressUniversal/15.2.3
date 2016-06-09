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
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Office;
using System.Globalization;
namespace DevExpress.XtraRichEdit.Import.Html {
	#region LineBreakTag
	public class LineBreakTag : TagBase {
		public LineBreakTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override bool CanAppendToTagStack { get { return false; } }
		protected internal override void EmptyTagProcess() {
			Importer.CloseProcess(this);
		}
		protected internal override AttributeKeywordTranslatorTable AttributeTable { get { return base.AttributeTable; } }
		protected internal override void ApplyTagProperties() {
		}
		protected internal override void FunctionalTagProcess() {
			switch(Importer.Options.LineBreakSubstitute) {
				case LineBreakSubstitute.Space:
					Importer.AppendText(new String(Characters.Space, 1));
					break;
				case LineBreakSubstitute.Paragraph:
					ParagraphFunctionalProcess();
					break;
				default:
			Importer.AppendText(new String(Characters.LineBreak, 1));
					break;
			}
			Importer.IsEmptyLine = true;
		}
	}
	#endregion
	#region DivisionTag
	public class DivisionTag : ParagraphTagBase {
		public DivisionTag(HtmlImporter importer)
			: base(importer) {
		}
		protected override ParagraphAlignment GetActualAlignmentValue() {
			if (Alignment.AlignmentValue == ParagraphAlignment.Justify)
				return Importer.Position.DefaultAlignment.AlignmentValue;
			return base.GetActualAlignmentValue();
		}
		protected internal override ParagraphFormattingOptions ApplyCssProperties() {
			ParagraphFormattingOptions options = base.ApplyCssProperties();
			return options;
		}
	}
	#endregion
	#region HeadingTag
	public class HeadingTag : ParagraphTagBase {
		readonly int fontSize;
		readonly int outlineLevel;
		public HeadingTag(HtmlImporter importer, int fontSize, int outlineLevel)
			: base(importer) {
			this.fontSize = fontSize;
			this.outlineLevel = outlineLevel;
		}
		protected internal override void ApplyTagProperties() {
			ParagraphFormattingBase paragraphFormatting = Importer.Position.ParagraphFormatting;
			CharacterFormattingBase characterFormatting = Importer.Position.CharacterFormatting;
			if (Alignment.UseAlignment) {
				Importer.Position.DefaultAlignment.AlignmentValue = Alignment.AlignmentValue;
				paragraphFormatting.Alignment = Alignment.AlignmentValue;
			}
			characterFormatting.DoubleFontSize = Importer.HtmlFontSize.GetDoubleFontSize(fontSize);
			characterFormatting.FontBold = true;
			paragraphFormatting.OutlineLevel = outlineLevel;
			ResetParagraphFormattingProperties(paragraphFormatting);
		}
		void ResetParagraphFormattingProperties(ParagraphFormattingBase paragraphFormatting) {
			if (paragraphFormatting.Options.UseSpacingAfter)
				paragraphFormatting.SpacingAfter = DefaultSpacing;
			if (paragraphFormatting.Options.UseSpacingBefore)
				paragraphFormatting.SpacingBefore = DefaultSpacing;
			if (paragraphFormatting.Options.UseLeftIndent)
				paragraphFormatting.LeftIndent = 0;
		}
	}
	#endregion
	#region MarqueeTag
	public class MarqueeTag : TagBase {
		public MarqueeTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyTagProperties() {
		}
	}
	#endregion
	#region ParagraphTagBase
	public class ParagraphTagBase : TagBase {
		static AttributeKeywordTranslatorTable attributeTable = AddAttributes();
		static AttributeKeywordTranslatorTable AddAttributes() {
			AttributeKeywordTranslatorTable table = CreateAttributeTable();
			table.Add(ConvertKeyToUpper("align"), ParagraphAlignmentKeyword);
			return table;
		}
		static internal void ParagraphAlignmentKeyword(HtmlImporter importer, string value, TagBase tag) {
			ParagraphTagBase paragraphTag = (ParagraphTagBase)tag;
			ParagraphAlignment resultAlignment = ParagraphAlignment.Left;
			if (ReadParagraphAlignment(value, ref resultAlignment))
				paragraphTag.Alignment.AlignmentValue = resultAlignment;
		}
		#region Fields
		const int DefaultSpacingInPoints = 12;
		readonly int defaultSpacing;
		HtmlParagraphAlignment alignment;
		#endregion
		public ParagraphTagBase(HtmlImporter importer)
			: base(importer) {
			this.defaultSpacing = importer.UnitConverter.PointsToModelUnits(DefaultSpacingInPoints);
			InitializeAlignment();
		}
		protected internal HtmlParagraphAlignment Alignment { get { return alignment; } }
		protected internal override AttributeKeywordTranslatorTable AttributeTable { get { return attributeTable; } }
		protected int DefaultSpacing { get { return defaultSpacing; } }
		void InitializeAlignment() {
			this.alignment = new HtmlParagraphAlignment();
			if (Importer.Position.DefaultAlignment.UseAlignment)
				this.alignment.AlignmentValue = Importer.Position.DefaultAlignment.AlignmentValue;
		}
		protected internal override void ApplyTagProperties() {
			if (Alignment.UseAlignment)
				Importer.Position.ParagraphFormatting.Alignment = GetActualAlignmentValue();
			Importer.Position.ParagraphFormatting.OutlineLevel = 0;
		}
		protected virtual ParagraphAlignment GetActualAlignmentValue() {
			return Alignment.AlignmentValue;
		}
		protected internal override ParagraphFormattingOptions ApplyCssProperties() {
			ParagraphFormattingOptions options = base.ApplyCssProperties();			
			int count = Importer.TagsStack.Count;
			if (count < 2)
				return options;			
			OpenHtmlTag prevTag = Importer.TagsStack[count - 2];
			LevelTag levelTag = prevTag.Tag as LevelTag;
			if (levelTag != null) {
				int delta = options.UseLeftIndent ? Importer.Position.ParagraphFormatting.LeftIndent : 0;
				Importer.Position.ParagraphFormatting.LeftIndent = Importer.TagsStack[count - 1].OldPosition.ParagraphFormatting.LeftIndent + delta;
			}
			return options;
		}
		protected internal override void FunctionalTagProcess() {
			if (!String.IsNullOrEmpty(this.Id)) {
				Importer.ProcessBookmarkStart(this.Id);
				Importer.ProcessBookmarkEnd();
			}
			ParagraphFunctionalProcess();
		}
	}
	#endregion
	#region ParagraphTag
	public class ParagraphTag : ParagraphTagBase {
		public ParagraphTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyTagProperties() {
			ParagraphFormattingBase formatting = Importer.Position.ParagraphFormatting;
			if (Alignment.UseAlignment)
				formatting.Alignment = Alignment.AlignmentValue;
			formatting.SpacingAfter = DefaultSpacing;
			formatting.SpacingBefore = DefaultSpacing;
		}
		protected internal override void FunctionalTagProcess() {
			if (!Importer.IsEmptyParagraph)
				ApplyAutoSpacing(Importer.Position.ParagraphIndex);
			base.FunctionalTagProcess();
		}
		protected internal override void OpenTagProcessCore() {
			Importer.LastOpenParagraphTagIndex = Importer.TagsStack.Count - 1;
		}
		protected internal virtual void ApplyAutoSpacing(ParagraphIndex paragraphIndex) {
			ParagraphProperties paragraphProperties = Importer.PieceTable.Paragraphs[paragraphIndex].ParagraphProperties;
			if (!paragraphProperties.UseSpacingAfter)
				paragraphProperties.SpacingAfter = DefaultSpacing;
			if (!paragraphProperties.UseSpacingBefore)
				paragraphProperties.SpacingBefore = DefaultSpacing;
		}
	}
	#endregion
	#region PreformattedTag
	public class PreformattedTag : TagBase {
		public PreformattedTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyTagProperties() {
			if(!Importer.Position.CharacterFormatting.Options.UseDoubleFontSize)
				Importer.Position.CharacterFormatting.DoubleFontSize = 20;
			Importer.Position.CharacterFormatting.FontName = "Courier New";
			TabFormattingInfo tabs = new TabFormattingInfo();
			DocumentModelUnitConverter converter = Importer.UnitConverter;
			tabs.Add(new TabInfo(converter.TwipsToModelUnits(916)));
			tabs.Add(new TabInfo(converter.TwipsToModelUnits(1832)));
			tabs.Add(new TabInfo(converter.TwipsToModelUnits(2748)));
			tabs.Add(new TabInfo(converter.TwipsToModelUnits(3664)));
			tabs.Add(new TabInfo(converter.TwipsToModelUnits(4580)));
			tabs.Add(new TabInfo(converter.TwipsToModelUnits(5496)));
			tabs.Add(new TabInfo(converter.TwipsToModelUnits(6412)));
			tabs.Add(new TabInfo(converter.TwipsToModelUnits(7328)));
			tabs.Add(new TabInfo(converter.TwipsToModelUnits(8244)));
			tabs.Add(new TabInfo(converter.TwipsToModelUnits(9160)));
			tabs.Add(new TabInfo(converter.TwipsToModelUnits(10076)));
			tabs.Add(new TabInfo(converter.TwipsToModelUnits(11908)));
			tabs.Add(new TabInfo(converter.TwipsToModelUnits(12824)));
			tabs.Add(new TabInfo(converter.TwipsToModelUnits(13740)));
			tabs.Add(new TabInfo(converter.TwipsToModelUnits(14656)));
			Importer.Position.ParagraphTabs.AddRange(tabs);
		}
		protected internal override void FunctionalTagProcess() {
			ParagraphFunctionalProcess();
		}
		protected internal override void BeforeDeleteTagFromStack(int indexOfDeletedTag) {
			base.BeforeDeleteTagFromStack(indexOfDeletedTag);
			Importer.Position.ParagraphTabs.Clear();
		}
	}
	#endregion
	#region SpanTag
	public class SpanTag : TagBase {
		public SpanTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyTagProperties() {
		}
	}
	#endregion
	#region HrTag
	public class HrTag : TagBase {
		public HrTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override bool CanAppendToTagStack { get { return false; } }
		protected internal override bool ApplyStylesToInnerHtml { get { return false; } }
		protected internal override void ApplyProperties() {
		}
		protected internal override ParagraphFormattingOptions ApplyCssProperties() {
			return new ParagraphFormattingOptions(ParagraphFormattingOptions.Mask.UseNone);
		}
		protected internal override void ApplyTagProperties() {
		}
		protected internal override void EmptyTagProcess() {
			Importer.CloseProcess(this);
		}
	}
	#endregion
}
