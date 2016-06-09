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
using System.Drawing;
using DevExpress.Office;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Data.Utils;
using DevExpress.Compatibility.System.Drawing;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Import.Html {
	#region BoldTag
	public class BoldTag : TagBase {
		public BoldTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyTagProperties() {
			Importer.Position.CharacterFormatting.FontBold = true;
		}
	}
	#endregion
	#region ItalicTag
	public class ItalicTag : TagBase {
		public ItalicTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyTagProperties() {
			Importer.Position.CharacterFormatting.FontItalic = true;
		}
	}
	#endregion
	#region UnderlineTag
	public class UnderlineTag : TagBase {
		public UnderlineTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyTagProperties() {
			Importer.Position.CharacterFormatting.FontUnderlineType = UnderlineType.Single;
		}
	}
	#endregion
	#region StrongTag
	public class StrongTag : TagBase {
		public StrongTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyTagProperties() {
			Importer.Position.CharacterFormatting.FontBold = true;
		}
	}
	#endregion
	#region BigTag
	public class BigTag : HtmlTag {
		public BigTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyTagProperties() {
			Importer.Position.CharacterFormatting.DoubleFontSize = Importer.HtmlFontSize.GetLargerDoubleFontSize();
		}
	}
	#endregion
	#region SmallTag
	public class SmallTag : TagBase {
		public SmallTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyTagProperties() {
			Importer.Position.CharacterFormatting.DoubleFontSize = Importer.HtmlFontSize.GetSmallerDoubleFontSize();
		}
	}
	#endregion
	#region FontTag
	public class FontTag : TagBase {
		#region Field
		int doubleFontSize;
		bool useFontSize;
		string fontName;
		bool useFontName;
		Color fontColor;
		bool useFontColor;
		#endregion
		protected internal override AttributeKeywordTranslatorTable AttributeTable { get { return attributeTable; } }
		static AttributeKeywordTranslatorTable attributeTable = AddAttributes();
		static AttributeKeywordTranslatorTable AddAttributes() {
			AttributeKeywordTranslatorTable table = CreateAttributeTable();
			table.Add(ConvertKeyToUpper("size"), FontSizeKeyword);
			table.Add(ConvertKeyToUpper("color"), FontColorKeyword);
			table.Add(ConvertKeyToUpper("face"), FontFaceKeyword);
			return table;
		}		
		static internal void FontSizeKeyword(HtmlImporter importer, string value, TagBase tag) {
			FontTag fontTag = (FontTag)tag;
			try {
				if (value.StartsWith("+")) {
					int count = Convert.ToInt32(value.Substring(1));
					for (int i = 0; i < count; i++)
						fontTag.DoubleFontSize = importer.HtmlFontSize.GetLargerDoubleFontSize();
				}
				else if (value.StartsWith("-")) {
					int count = Convert.ToInt32(value.Substring(1));
					for (int i = 0; i < count; i++)
						fontTag.DoubleFontSize = importer.HtmlFontSize.GetSmallerDoubleFontSize();
				}
				else
					fontTag.DoubleFontSize = importer.HtmlFontSize.GetDoubleFontSize(Convert.ToInt32(value));
			}
			catch {
			}
		}
		static internal void FontColorKeyword(HtmlImporter importer, string value, TagBase tag) {
			FontTag fontTag = (FontTag)tag;
			fontTag.FontColor = MarkupLanguageColorParser.ParseColor(value);
		}
		static internal void FontFaceKeyword(HtmlImporter importer, string value, TagBase tag) {
			FontTag fontTag = (FontTag)tag;
			if (String.IsNullOrEmpty(value)) {
				fontTag.FontName = value;
				return;
			}
			string[] names = value.Split(',');
			int count = names.Length;
			for (int i = 0; i < count; i++) {
				string name = names[i].Trim();
				if (!String.IsNullOrEmpty(name)) {
					fontTag.FontName = name;
					break;
				}
			}
		}
		public FontTag(HtmlImporter importer)
			: base(importer) {
			CharacterFormattingBase formatting = Importer.Position.CharacterFormatting;
			if(formatting.Options.UseDoubleFontSize)
				DoubleFontSize = formatting.DoubleFontSize;
			if (formatting.Options.UseFontName)
				FontName = formatting.FontName;
			if (formatting.Options.UseForeColor)
				FontColor = formatting.ForeColor;
		}
		protected int DoubleFontSize { get { return doubleFontSize; } set { doubleFontSize = value; useFontSize = true; } }
		protected string FontName { get { return fontName; } set { fontName = value; useFontName = true; } }
		protected Color FontColor { get { return fontColor; } set { fontColor = value; useFontColor = true; } }
		protected internal override void ApplyTagProperties() {
			CharacterFormattingBase formatting = Importer.Position.CharacterFormatting;
			if(useFontSize)
				formatting.DoubleFontSize = DoubleFontSize;
			if(useFontName)
				formatting.FontName = FontName;
			if(useFontColor)
				formatting.ForeColor = FontColor;
		}
	}
	#endregion
	#region EmphasizedTag
	public class EmphasizedTag : TagBase {
		public EmphasizedTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyTagProperties() {
			Importer.Position.CharacterFormatting.FontItalic = true;
		}
	}
	#endregion
	#region SubScriptTag
	public class SubScriptTag : TagBase {
		public SubScriptTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyTagProperties() {
			Importer.Position.CharacterFormatting.Script = CharacterFormattingScript.Subscript;
		}
	}
	#endregion
	#region SuperScriptTag
	public class SuperScriptTag : TagBase {
		public SuperScriptTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyTagProperties() {
			Importer.Position.CharacterFormatting.Script = CharacterFormattingScript.Superscript;
		}
	}
	#endregion
	#region StrikeoutTag
	public class StrikeoutTag : TagBase {
		public StrikeoutTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyTagProperties() {
			Importer.Position.CharacterFormatting.FontStrikeoutType = StrikeoutType.Single;
		}
	}
	#endregion
	#region CenterTag
	public class CenterTag : TagBase {
		public CenterTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyTagProperties() {
			Importer.Position.DefaultAlignment.AlignmentValue = ParagraphAlignment.Center;
			Importer.Position.ParagraphFormatting.Alignment = ParagraphAlignment.Center;
		}
		protected internal override void FunctionalTagProcess() {
			ParagraphFunctionalProcess();
		}
	}
	#endregion
	#region CodeTag
	public class CodeTag : HtmlTag {
		public CodeTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyTagProperties() {
			Importer.Position.CharacterFormatting.DoubleFontSize = 20;
			Importer.Position.CharacterFormatting.FontName = "Courier New";
		}
	}
	#endregion
	#region BlockquoteTag
	public class BlockquoteTag : TagBase {
		int indent;
		public BlockquoteTag(HtmlImporter importer)
			: base(importer) {
			this.indent = importer.UnitConverter.PixelsToModelUnits(40, 96);
		}
		protected internal override void ApplyTagProperties() {
			Importer.Position.AdditionalIndent += indent;
		}
		protected internal override void FunctionalTagProcess() {
			ParagraphFunctionalProcess();
		}
	}
	#endregion
	#region XmpTag
	public class XmpTag : TagBase {
		public XmpTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyTagProperties() {
		}
	}
	#endregion
	#region WbrTag
	public class WbrTag : TagBase {
		public WbrTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyTagProperties() {
		}
	}
	#endregion
	#region VarTag
	public class VarTag : TagBase {
		public VarTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyTagProperties() {
		}
	}
	#endregion
	#region TtTag
	public class TtTag : TagBase {
		public TtTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyTagProperties() {
		}
	}
	#endregion
	#region SampTag
	public class SampTag : TagBase {
		public SampTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyTagProperties() {
		}
	}
	#endregion
	#region QTag
	public class QTag : TagBase {
		public QTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyTagProperties() {
		}
		protected internal override void OpenTagProcessCore() {
			this.Importer.PieceTable.InsertText(this.Importer.Position, "\"", false);
		}
		protected internal override void BeforeDeleteTagFromStack(int indexOfDeletedTag) {
			this.Importer.PieceTable.InsertText(this.Importer.Position, "\"", false);
			base.BeforeDeleteTagFromStack(indexOfDeletedTag);
		}
	}
	#endregion
	#region NobrTag
	public class NobrTag : TagBase {
		public NobrTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyTagProperties() {
		}
	}
	#endregion
	#region KbdTag
	public class KbdTag : TagBase {
		public KbdTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyTagProperties() {
		}
	}
	#endregion
	#region InsTag
	public class InsTag : TagBase {
		public InsTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyTagProperties() {
		}
	}
	#endregion
	#region DfnTag
	public class DfnTag : TagBase {
		public DfnTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyTagProperties() {
		}
	}
	#endregion
	#region DelTag
	public class DelTag : TagBase {
		public DelTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyTagProperties() {
		}
	}
	#endregion
	#region CiteTag
	public class CiteTag : TagBase {
		public CiteTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyTagProperties() {
		}
	}
	#endregion
	#region BdoTag
	public class BdoTag : TagBase {
		public BdoTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyTagProperties() {
		}
	}
	#endregion
	#region BaseFontTag
	public class BaseFontTag : FontTag {
		public BaseFontTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyTagProperties() {
			base.ApplyTagProperties();
		}
	}
	#endregion
	#region AddressTag
	public class AddressTag : TagBase {
		public AddressTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyTagProperties() {
		}
	}
	#endregion
	#region AcronymTag
	public class AcronymTag : TagBase {
		public AcronymTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyTagProperties() {
		}
	}
	#endregion
	#region AbberTag
	public class AbberTag : TagBase {
		public AbberTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyTagProperties() {
		}
	}
	#endregion
}
