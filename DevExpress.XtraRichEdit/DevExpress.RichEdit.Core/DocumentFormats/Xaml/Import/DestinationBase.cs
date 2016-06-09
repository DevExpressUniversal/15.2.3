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
using System.Drawing;
using System.Xml;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Office;
using DevExpress.Compatibility.System.Drawing;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Import.Xaml {
	public delegate Destination ElementHandler(XamlImporter importer, XmlReader reader);
	#region ElementHandlerTable
	public class ElementHandlerTable : Dictionary<string, ElementHandler> {
		static readonly ElementHandlerTable empty = new ElementHandlerTable();
		public static ElementHandlerTable Empty { get { return empty; } }
	}
	#endregion
	#region ElementDestination (abstract class)
	public abstract class ElementDestination : Destination {
		protected ElementDestination(XamlImporter importer)
			: base(importer) {
		}
		internal virtual new XamlImporter Importer { get { return (XamlImporter)base.Importer; } }
		protected internal DocumentModel DocumentModel { get { return Importer.DocumentModel; } }
		protected internal PieceTable PieceTable { get { return Importer.PieceTable; } }
		protected internal abstract ElementHandlerTable ElementHandlerTable { get; }
		protected internal DocumentModelUnitConverter UnitConverter { get { return Importer.UnitConverter; } }
		public override void ProcessElementOpen(XmlReader reader) {
		}
		public override void ProcessElementClose(XmlReader reader) {
		}
		public override bool ProcessText(XmlReader reader) {
			return true;
		}
		protected override Destination ProcessCurrentElement(XmlReader reader) {
			string localName = reader.LocalName;
			ElementHandler handler;
			if (ElementHandlerTable.TryGetValue(localName, out handler))
				return handler(Importer, reader);
			else
				return null;
		}
	}
	#endregion
	#region TextElementDestination (abstract class)
	public abstract class TextElementDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("Run", OnRun);
			result.Add("Span", OnSpan);
			result.Add("Bold", OnBold);
			result.Add("Italic", OnItalic);
			result.Add("Underline", OnUnderline);
			result.Add("Hyperlink", OnHyperlink);
			result.Add("InlineUIContainer", OnInlineUIContainer);
			result.Add("Figure", OnFigure);
			result.Add("Floater", OnFloater);
			result.Add("LineBreak", OnLineBreak);
			return result;
		}
		protected static ElementHandlerTable CreateInlineElementHandlerTable() {
			return CreateElementHandlerTable();
		}
		CharacterFormattingBase prevCharacterFormatting;
		int prevCharacterStyleIndex;
		protected TextElementDestination(XamlImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
			ImportInputPosition position = Importer.Position;
			this.prevCharacterFormatting = position.CharacterFormatting.Clone();
			this.prevCharacterStyleIndex = position.CharacterStyleIndex;
			ApplyAttributes(reader);
		}
		public override void ProcessElementClose(XmlReader reader) {
			if (prevCharacterFormatting != null) {
				ImportInputPosition position = Importer.Position;
				position.CharacterFormatting.CopyFrom(prevCharacterFormatting);
				position.CharacterStyleIndex = prevCharacterStyleIndex;
			}
		}
		protected internal virtual void ApplyAttributes(XmlReader reader) {
			CharacterFormattingBase formatting = Importer.Position.CharacterFormatting;
			formatting.BeginUpdate();
			try {
				string value;
				value = reader.GetAttribute("FontFamily");
				if (!String.IsNullOrEmpty(value) && StringExtensions.CompareInvariantCultureIgnoreCase(value, "Portable User Interface") != 0)
					formatting.FontName = value;
				int fontSize = Importer.GetWpSTIntegerValue(reader, "FontSize", 0);
				if (fontSize > 0)
					formatting.DoubleFontSize = fontSize*2;
				DefaultBoolean bold = Importer.GetWpEnumValue(reader, "FontWeight", XamlImporter.fontWeightTable, DefaultBoolean.Default);
				if (bold != DefaultBoolean.Default)
					formatting.FontBold = (bold == DefaultBoolean.True);
				DefaultBoolean italic = Importer.GetWpEnumValue(reader, "FontStyle", XamlImporter.fontStyleTable, DefaultBoolean.Default);
				if (italic != DefaultBoolean.Default)
					formatting.FontItalic = (italic == DefaultBoolean.True);
				Color color = Importer.GetBrushColorValue(reader, "Foreground");
				if (!DXColor.IsTransparentOrEmpty(color))
					formatting.ForeColor = color;
				color = Importer.GetBrushColorValue(reader, "Background");
				if (!DXColor.IsTransparentOrEmpty(color))
					formatting.BackColor = color;
			}
			finally {
				formatting.EndUpdate();
			}
		}
		protected internal virtual void ApplyTextDecorations(XmlReader reader) {
			string value = Importer.ReadAttribute(reader, "TextDecorations");
			if (String.IsNullOrEmpty(value))
				return;
			string[] decorations = value.Split(',');
			int count = decorations.Length;
			for (int i = 0; i < count; i++) {
				switch (decorations[i].Trim()) {
					case "Underline":
						Importer.Position.CharacterFormatting.FontUnderlineType = UnderlineType.Single;
						break;
					case "Baseline":
						Importer.Position.CharacterFormatting.FontUnderlineType = UnderlineType.Single;
						break;
					case "Strikethrough":
						Importer.Position.CharacterFormatting.FontStrikeoutType = StrikeoutType.Single;
						break;
				}
			}
		}
		static Destination OnRun(XamlImporter importer, XmlReader reader) {
			return new RunDestination(importer);
		}
		static Destination OnSpan(XamlImporter importer, XmlReader reader) {
			return new SpanDestination(importer);
		}
		static Destination OnBold(XamlImporter importer, XmlReader reader) {
			return new BoldDestination(importer);
		}
		static Destination OnItalic(XamlImporter importer, XmlReader reader) {
			return new ItalicDestination(importer);
		}
		static Destination OnUnderline(XamlImporter importer, XmlReader reader) {
			return new UnderlineDestination(importer);
		}
		static Destination OnHyperlink(XamlImporter importer, XmlReader reader) {
			return new HyperlinkDestination(importer);
		}
		static Destination OnInlineUIContainer(XamlImporter importer, XmlReader reader) {
			return new InlineUIContainerDestination(importer);
		}
		static Destination OnFigure(XamlImporter importer, XmlReader reader) {
			return new FigureDestination(importer);
		}
		static Destination OnFloater(XamlImporter importer, XmlReader reader) {
			return new FloaterDestination(importer);
		}
		static Destination OnLineBreak(XamlImporter importer, XmlReader reader) {
			return new LineBreakDestination(importer);
		}
	}
	#endregion
	#region InlineElementDestination (abstract class)
	public abstract class InlineElementDestination : TextElementDestination {
		protected InlineElementDestination(XamlImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			ApplyTextDecorations(reader);
			string value = Importer.ReadAttribute(reader, "BaselineAlignment");
			if (!String.IsNullOrEmpty(value)) {
				CharacterFormattingBase formatting = Importer.Position.CharacterFormatting;
				switch (value) {
					case "Superscript":
						formatting.Script = CharacterFormattingScript.Superscript;
						break;
					case "Subscript":
						formatting.Script = CharacterFormattingScript.Subscript;
						break;
					default:
						formatting.Script = CharacterFormattingScript.Normal;
						break;
				}
			}
		}
	}
	#endregion
	#region InlineLeafElementDestination (abstract class)
	public abstract class InlineLeafElementDestination : InlineElementDestination {
		protected InlineLeafElementDestination(XamlImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return ElementHandlerTable.Empty; } }
	}
	#endregion
	#region BlockElementDestination (abstract class)
	public abstract class BlockElementDestination : TextElementDestination {
		ParagraphFormattingBase prevParagraphFormatting;
		int prevParagraphStyleIndex;
		protected BlockElementDestination(XamlImporter importer)
			: base(importer) {
		}
		protected static ElementHandlerTable CreateBlockElementHandlerTable() {
			ElementHandlerTable result = CreateInlineElementHandlerTable();
			result.Add("Paragraph", OnParagraph);
			result.Add("Section", OnSection);
			result.Add("BlockUIContainer", OnBlockUIContainer);
			result.Add("Table", OnTable);
			result.Add("List", OnList);
			return result;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			if (Importer.GetWpSTOnOffValue(reader, "BreakColumnBefore", false))
				Importer.PieceTable.InsertTextCore(Importer.Position, new String(Characters.ColumnBreak, 1));
			if (Importer.GetWpSTOnOffValue(reader, "BreakPageBefore", false))
				Importer.PieceTable.InsertTextCore(Importer.Position, new String(Characters.PageBreak, 1));
			ImportInputPosition position = Importer.Position;
			ParagraphFormattingBase formatting = position.ParagraphFormatting;
			this.prevParagraphFormatting = formatting.Clone();
			this.prevParagraphStyleIndex = position.ParagraphStyleIndex;
			formatting.BeginUpdate();
			try {
				formatting.Alignment = Importer.GetWpEnumValue(reader, "TextAlignment", XamlImporter.textAlignmentTable, ParagraphAlignment.Left);
				formatting.SuppressHyphenation = !Importer.GetWpSTOnOffValue(reader, "IsHyphenationEnabled");
				MarginsInfo margins = Importer.ReadThickness(reader, "Margin");
				MarginsInfo paddings = Importer.ReadThickness(reader, "Padding");
				formatting.LeftIndent = margins.Left + paddings.Left;
				formatting.RightIndent = margins.Right + paddings.Right;
				formatting.SpacingBefore = margins.Top + paddings.Top;
				formatting.SpacingAfter = margins.Bottom + paddings.Bottom;
				string lineSpacingString = Importer.ReadAttribute(reader, "LineHeight");
				if (!String.IsNullOrEmpty(lineSpacingString) && lineSpacingString != "Auto") {
					int lineSpacing = Importer.ParseMetricIntegerToModelUnits(lineSpacingString);
					if (lineSpacing > 0) {
						switch (Importer.ReadAttribute(reader, "LineStackingStrategy")) {
							default:
							case "MaxHeight":
								formatting.LineSpacingType = ParagraphLineSpacing.AtLeast;
								break;
							case "BlockLineHeight":
								formatting.LineSpacingType = ParagraphLineSpacing.Exactly;
								break;
						}
						formatting.LineSpacing = lineSpacing;
					}
				}
				else
					formatting.LineSpacingType = ParagraphLineSpacing.Single;
			}
			finally {
				formatting.EndUpdate();
			}
		}
		public override void ProcessElementClose(XmlReader reader) {
			base.ProcessElementClose(reader);
			if (prevParagraphFormatting != null) {
				ImportInputPosition position = Importer.Position;
				position.ParagraphFormatting.CopyFrom(prevParagraphFormatting);
				position.ParagraphStyleIndex = prevParagraphStyleIndex;
			}
		}
		static Destination OnParagraph(XamlImporter importer, XmlReader reader) {
			return new ParagraphDestination(importer);
		}
		public static Destination OnSection(XamlImporter importer, XmlReader reader) {
			return new SectionDestination(importer);
		}
		public static Destination OnBlockUIContainer(XamlImporter importer, XmlReader reader) {
			return new BlockUIContainerDestination(importer);
		}
		public static Destination OnTable(XamlImporter importer, XmlReader reader) {
			if (importer.DocumentModel.DocumentCapabilities.TablesAllowed)
				return new TableDestination(importer);
			return new TableDisabledDestination(importer);
		}
		public static Destination OnList(XamlImporter importer, XmlReader reader) {
			return new ListDestination(importer);
		}
	}
	#endregion
}
