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
using System.Collections.Generic;
using System.Xml;
using DevExpress.XtraRichEdit.Export.OpenXml;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Office;
using System.Globalization;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Compatibility.System.Drawing;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Import.OpenXml {
	#region RunPropertiesBaseDestination (abstract class)
	public abstract class RunPropertiesBaseDestination : ElementDestination {
		readonly ICharacterProperties characterProperties;
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		protected static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("b", OnBold);
			result.Add("i", OnItalic);
			result.Add("caps", OnCaps);
			result.Add("vanish", OnHiddenText);
			result.Add("color", OnForeColor);
			result.Add("highlight", OnBackColor);
			result.Add("shd", OnShading);
			result.Add("strike", OnSingleStrikeThrough);
			result.Add("dstrike", OnDoubleStrikeThrough);
			result.Add("u", OnUnderline);
			result.Add("sz", OnFontSize);
			result.Add("vertAlign", OnScript);
			result.Add("rFonts", OnFontName);
			result.Add("lang", OnLanguage);
			result.Add("noProof", OnNoProof);
			return result;
		}
		protected RunPropertiesBaseDestination(WordProcessingMLBaseImporter importer, ICharacterProperties characterProperties)
			: base(importer) {
			Guard.ArgumentNotNull(characterProperties, "characterProperties");
			this.characterProperties = characterProperties;
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		protected internal ICharacterProperties CharacterProperties { get { return characterProperties; } }
		static ICharacterProperties GetCharacterProperties(WordProcessingMLBaseImporter importer) {
			RunPropertiesBaseDestination thisObject = (RunPropertiesBaseDestination)importer.PeekDestination();
			return thisObject.characterProperties;
		}
		static Destination OnBold(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new BoldDestination(importer, GetCharacterProperties(importer));
		}
		static Destination OnItalic(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new ItalicDestination(importer, GetCharacterProperties(importer));
		}
		static Destination OnCaps(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new AllCapsDestination(importer, GetCharacterProperties(importer));
		}
		static Destination OnHiddenText(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new HiddenTextDestination(importer, GetCharacterProperties(importer));
		}
		static Destination OnUnderline(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new UnderlineDestination(importer, GetCharacterProperties(importer));
		}
		static Destination OnSingleStrikeThrough(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new SingleStrikeThroughDestination(importer, GetCharacterProperties(importer));
		}
		static Destination OnDoubleStrikeThrough(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new DoubleStrikeThroughDestination(importer, GetCharacterProperties(importer));
		}
		static Destination OnForeColor(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new ForeColorDestination(importer, GetCharacterProperties(importer));
		}
		static Destination OnBackColor(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new BackColorDestination(importer, GetCharacterProperties(importer));
		}
		static Destination OnShading(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new ShadingDestination(importer, GetCharacterProperties(importer));
		}
		static Destination OnFontSize(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new FontSizeDestination(importer, GetCharacterProperties(importer));
		}
		static Destination OnScript(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new FontScriptDestination(importer, GetCharacterProperties(importer));
		}
		static Destination OnFontName(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new FontNameDestination(importer, GetCharacterProperties(importer));
		}
		static Destination OnLanguage(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new LanguageDestination(importer, GetCharacterProperties(importer));
		}
		static Destination OnNoProof(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new NoProofDestination(importer, GetCharacterProperties(importer));
		}
	}
	#endregion
	#region RunPropertiesDestination
	public class RunPropertiesDestination : RunPropertiesBaseDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static new ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = RunPropertiesBaseDestination.CreateElementHandlerTable();
			result.Add("rStyle", OnStyle);
			return result;
		}
		public RunPropertiesDestination(WordProcessingMLBaseImporter importer, CharacterFormattingBase characterFormatting)
			: base(importer, characterFormatting) {
			importer.Position.CharacterStyleIndex = 0;
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
			CharacterFormattingBase characterFormatting = (CharacterFormattingBase)CharacterProperties;
			characterFormatting.BeginUpdate();
		}
		public override void ProcessElementClose(XmlReader reader) {
			CharacterFormattingBase characterFormatting = (CharacterFormattingBase)CharacterProperties;
			characterFormatting.EndUpdate();
		}
		static Destination OnStyle(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new RunStyleReferenceDestination(importer);
		}
	}
	#endregion
	#region CharacterFormattingLeafElementDestination (abstract class)
	public abstract class CharacterFormattingLeafElementDestination : LeafElementDestination {
		readonly ICharacterProperties characterProperties;
		protected CharacterFormattingLeafElementDestination(WordProcessingMLBaseImporter importer, ICharacterProperties characterProperties)
			: base(importer) {
			Guard.ArgumentNotNull(characterProperties, "characterProperties");
			this.characterProperties = characterProperties;
		}
		public ICharacterProperties CharacterProperties { get { return characterProperties; } }
	}
	#endregion
	#region BoldDestination
	public class BoldDestination : CharacterFormattingLeafElementDestination {
		public BoldDestination(WordProcessingMLBaseImporter importer, ICharacterProperties characterProperties)
			: base(importer, characterProperties) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			CharacterProperties.FontBold = Importer.GetWpSTOnOffValue(reader, "val");
		}
	}
	#endregion
	#region ItalicDestination
	public class ItalicDestination : CharacterFormattingLeafElementDestination {
		public ItalicDestination(WordProcessingMLBaseImporter importer, ICharacterProperties characterProperties)
			: base(importer, characterProperties) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			CharacterProperties.FontItalic = Importer.GetWpSTOnOffValue(reader, "val");
		}
	}
	#endregion
	#region AllCapsDestination
	public class AllCapsDestination : CharacterFormattingLeafElementDestination {
		public AllCapsDestination(WordProcessingMLBaseImporter importer, ICharacterProperties characterProperties)
			: base(importer, characterProperties) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			CharacterProperties.AllCaps = Importer.GetWpSTOnOffValue(reader, "val");
		}
	}
	#endregion
	#region HiddenTextDestination
	public class HiddenTextDestination : CharacterFormattingLeafElementDestination {
		public HiddenTextDestination(WordProcessingMLBaseImporter importer, ICharacterProperties characterProperties)
			: base(importer, characterProperties) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			CharacterProperties.Hidden = Importer.GetWpSTOnOffValue(reader, "val");
		}
	}
	#endregion
	#region UnderlineDestination
	public class UnderlineDestination : CharacterFormattingLeafElementDestination {
		public UnderlineDestination(WordProcessingMLBaseImporter importer, ICharacterProperties characterProperties)
			: base(importer, characterProperties) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			ImportUnderlineType(reader);
			ImportUnderlineColor(reader);
		}
		protected internal void ImportUnderlineType(XmlReader reader) {
			string value = reader.GetAttribute("val", Importer.WordProcessingNamespaceConst);
			if (!String.IsNullOrEmpty(value)) {
				if (value == "words") {
					CharacterProperties.FontUnderlineType = UnderlineType.Single;
					CharacterProperties.UnderlineWordsOnly = true;
				}
				else
					CharacterProperties.FontUnderlineType = Importer.GetWpEnumValueCore(value, OpenXmlExporter.underlineTable, UnderlineType.Single);
			}
			else
				CharacterProperties.FontUnderlineType = UnderlineType.None;
		}
		private void ImportUnderlineColor(XmlReader reader) {
			Color color = Importer.GetWpSTColorValue(reader, "color");
			if (color != DXColor.Empty) {
				CharacterProperties.UnderlineColor = color;
			}
#if THEMES_EDIT            
			OpenXmlImportHelper helper = new OpenXmlImportHelper();
			CharacterProperties.UnderlineColorInfo = helper.SaveColorModelInfo(Importer, reader, "color");
#endif
		}
	}
	#endregion
	#region SingleStrikeThroughDestination
	public class SingleStrikeThroughDestination : CharacterFormattingLeafElementDestination {
		public SingleStrikeThroughDestination(WordProcessingMLBaseImporter importer, ICharacterProperties characterProperties)
			: base(importer, characterProperties) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			bool value = Importer.GetWpSTOnOffValue(reader, "val");
			if (value)
				CharacterProperties.FontStrikeoutType = StrikeoutType.Single;
			else
				CharacterProperties.FontStrikeoutType = StrikeoutType.None;
		}
	}
	#endregion
	#region DoubleStrikeThroughDestination
	public class DoubleStrikeThroughDestination : CharacterFormattingLeafElementDestination {
		public DoubleStrikeThroughDestination(WordProcessingMLBaseImporter importer, ICharacterProperties characterProperties)
			: base(importer, characterProperties) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			bool value = Importer.GetWpSTOnOffValue(reader, "val");
			if (value)
				CharacterProperties.FontStrikeoutType = StrikeoutType.Double;
			else
				CharacterProperties.FontStrikeoutType = StrikeoutType.None;
		}
	}
	#endregion
	#region ForeColorDestination
	public class ForeColorDestination : CharacterFormattingLeafElementDestination {
		public ForeColorDestination(WordProcessingMLBaseImporter importer, ICharacterProperties characterProperties)
			: base(importer, characterProperties) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string value = Importer.ReadAttribute(reader, "val"); 
			if (value == "auto") {
				CharacterProperties.ForeColor = DXColor.Empty;
			}
			else {
				Color color = Importer.GetWpSTColorValue(reader, "val");
				if (color != DXColor.Empty) {
					CharacterProperties.ForeColor = color;
				}
			}
#if THEMES_EDIT
			OpenXmlImportHelper helper = new OpenXmlImportHelper();
			CharacterProperties.ForeColorInfo = helper.SaveColorModelInfo(Importer, reader, "val");
#endif
		}
	}
	#endregion
	#region BackColorDestination
	public class BackColorDestination : CharacterFormattingLeafElementDestination {
		public BackColorDestination(WordProcessingMLBaseImporter importer, ICharacterProperties characterProperties)
			: base(importer, characterProperties) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string value = reader.GetAttribute("val", Importer.WordProcessingNamespaceConst);
			if (!String.IsNullOrEmpty(value)) {
				Color color = Importer.GetWpEnumValueCore(value, OpenXmlExporter.predefinedBackgroundColors, DXColor.Empty);
				if (color != DXColor.Empty) {
					CharacterProperties.BackColor = color;
				}
			}
#if THEMES_EDIT
			OpenXmlImportHelper helper = new OpenXmlImportHelper();
			CharacterProperties.BackColorInfo = helper.SaveColorModelInfo(Importer, reader, "val");
#endif
		}
	}
	#endregion
	#region ShadingDestination
	public class ShadingDestination : CharacterFormattingLeafElementDestination {
		public ShadingDestination(WordProcessingMLBaseImporter importer, ICharacterProperties characterProperties)
			: base(importer, characterProperties) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Color color = Importer.GetWpSTColorValue(reader, "fill");
			if (color != DXColor.Empty) {
				string value = reader.GetAttribute("val", Importer.WordProcessingNamespaceConst);
				if (String.IsNullOrEmpty(value) || value == "nil" || value == "clear") {
					if (DXColor.IsTransparentOrEmpty(CharacterProperties.BackColor))
						CharacterProperties.BackColor = color;
				}
			}
#if THEMES_EDIT
			Shading shading = CharacterProperties.Shading;
			shading.ShadingPattern = Importer.GetWpEnumValue<ShadingPattern>(reader, "val", OpenXmlExporter.shadingPatternTable, ShadingPattern.Clear);
			OpenXmlImportHelper helper = new OpenXmlImportHelper();
			shading.ColorInfo = helper.SaveColorModelInfo(Importer, reader, "color");
			shading.FillInfo = helper.SaveFillInfo(Importer, reader);
#endif
		}
	}
	#endregion
	#region FontSizeDestination
	public class FontSizeDestination : CharacterFormattingLeafElementDestination {
		public FontSizeDestination(WordProcessingMLBaseImporter importer, ICharacterProperties characterProperties)
			: base(importer, characterProperties) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			int value = Importer.GetWpSTIntegerValue(reader, "val", -1);
			if (value > 0)
				CharacterProperties.DoubleFontSize = Math.Max(1, value);
		}
	}
	#endregion
	#region FontScriptDestination
	public class FontScriptDestination : CharacterFormattingLeafElementDestination {
		public FontScriptDestination(WordProcessingMLBaseImporter importer, ICharacterProperties characterProperties)
			: base(importer, characterProperties) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string value = reader.GetAttribute("val", Importer.WordProcessingNamespaceConst);
			switch (value) {
				case "baseline":
					CharacterProperties.Script = CharacterFormattingScript.Normal;
					break;
				case "subscript":
					CharacterProperties.Script = CharacterFormattingScript.Subscript;
					break;
				case "superscript":
					CharacterProperties.Script = CharacterFormattingScript.Superscript;
					break;
			}
		}
	}
	#endregion
	#region FontNameDestination
	public class FontNameDestination : CharacterFormattingLeafElementDestination {
		public FontNameDestination(WordProcessingMLBaseImporter importer, ICharacterProperties characterProperties)
			: base(importer, characterProperties) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string fontName = ReadFontName(reader);
			if (!String.IsNullOrEmpty(fontName))
				CharacterProperties.FontName = fontName;
#if THEMES_EDIT
			RichEditFontInfo fontInfo = CharacterProperties.FontInfo;
			fontInfo.AsciiFontName = reader.GetAttribute("ascii", Importer.WordProcessingNamespaceConst);
			fontInfo.CsFontName = reader.GetAttribute("cs", Importer.WordProcessingNamespaceConst);
			fontInfo.EastAsiaFontName = reader.GetAttribute("eastAsia", Importer.WordProcessingNamespaceConst);
			fontInfo.HAnsiFontName = reader.GetAttribute("hAnsi", Importer.WordProcessingNamespaceConst);
			fontInfo.AsciiThemeFont = Importer.GetWpEnumValue<ThemeFontType>(reader, "asciiTheme", OpenXmlExporter.themeFontTypeTable, ThemeFontType.None);
			fontInfo.CsThemeFont = Importer.GetWpEnumValue<ThemeFontType>(reader, "cs", OpenXmlExporter.themeFontTypeTable, ThemeFontType.None);
			fontInfo.EastAsiaThemeFont = Importer.GetWpEnumValue<ThemeFontType>(reader, "eastAsia", OpenXmlExporter.themeFontTypeTable, ThemeFontType.None);
			fontInfo.HAnsiThemeFont = Importer.GetWpEnumValue<ThemeFontType>(reader, "hAnsi", OpenXmlExporter.themeFontTypeTable, ThemeFontType.None); ;
			fontInfo.HintFont = Importer.GetWpEnumValue<FontTypeHint>(reader, "hint", OpenXmlExporter.fontTypeHintTable, FontTypeHint.None);
#endif
		}
		protected virtual string ReadFontName(XmlReader reader) {
			string value = reader.GetAttribute("ascii", Importer.WordProcessingNamespaceConst);
			if (!String.IsNullOrEmpty(value))
				return value;
			WordProcessingMLValue attribute = new WordProcessingMLValue("hAnsi", "h-ansi");
			value = reader.GetAttribute(Importer.GetWordProcessingMLValue(attribute), Importer.WordProcessingNamespaceConst);
			if (!String.IsNullOrEmpty(value))
				return value;
			return String.Empty;
		}
	}
	#endregion
	#region DefaultFontNameDestination
	public class DefaultFontNameDestination : FontNameDestination {
		public DefaultFontNameDestination(WordProcessingMLBaseImporter importer, ICharacterProperties characterProperties)
			: base(importer, characterProperties) {
		}
		protected override string ReadFontName(XmlReader reader) {
			string value = reader.GetAttribute("ascii", Importer.WordProcessingNamespaceConst);
			if (!String.IsNullOrEmpty(value))
				return value;
			WordProcessingMLValue attribute = new WordProcessingMLValue("hAnsi", "h-ansi");
			value = reader.GetAttribute(Importer.GetWordProcessingMLValue(attribute), Importer.WordProcessingNamespaceConst);
			if (!String.IsNullOrEmpty(value))
				return value;
			value = reader.GetAttribute("cs", Importer.WordProcessingNamespaceConst);
			if (!String.IsNullOrEmpty(value))
				return value;
			value = reader.GetAttribute("eastAsia", Importer.WordProcessingNamespaceConst);
			if (!String.IsNullOrEmpty(value))
				return value;
			return RichEditControlCompatibility.DefaultFontName;
		}
	}
	#endregion
	#region LanguageDestination
	public class LanguageDestination : CharacterFormattingLeafElementDestination {
		public LanguageDestination(WordProcessingMLBaseImporter importer, ICharacterProperties characterProperties)
			: base(importer, characterProperties) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			LangInfo language = Importer.ReadLanguage(reader);
			CharacterProperties.LangInfo = language;
		}
	}
	#endregion
	#region NoProofDestination
	public class NoProofDestination : CharacterFormattingLeafElementDestination {
		public NoProofDestination(WordProcessingMLBaseImporter importer, ICharacterProperties characterProperties)
			: base(importer, characterProperties) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			CharacterProperties.NoProof = Importer.GetWpSTOnOffValue(reader, "val");
		}
	}
	#endregion
	#region RunStyleReferenceBaseDestination (abstract class)
	public abstract class RunStyleReferenceBaseDestination : LeafElementDestination {
		protected RunStyleReferenceBaseDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string value = reader.GetAttribute("val", Importer.WordProcessingNamespaceConst);
			if (!String.IsNullOrEmpty(value)) {
				int styleIndex = LookupStyleIndex(value);
				if (styleIndex >= 0)
					AssignCharacterStyleIndex(styleIndex);
			}
		}
		int LookupStyleIndex(string value) {
			return Importer.LookupCharacterStyleIndex(value);
		}
		protected internal abstract void AssignCharacterStyleIndex(int value);
	}
	#endregion
	#region RunStyleReferenceDestination
	public class RunStyleReferenceDestination : RunStyleReferenceBaseDestination {
		public RunStyleReferenceDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override void AssignCharacterStyleIndex(int value) {
			if (Importer.DocumentModel.DocumentCapabilities.CharacterStyleAllowed)
				Importer.Position.CharacterStyleIndex = value;
		}
	}
	#endregion
}
