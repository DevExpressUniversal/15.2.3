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
using System.Xml;
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Export.OpenXml;
using DevExpress.Office.Drawing;
using DevExpress.Office.Import.OpenXml;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region CharacterPropertiesDestination
	public class DrawingTextCharacterPropertiesDestination : DrawingFillDestinationBase {
		#region Static Members
		#region Handler Table
		static readonly ElementHandlerTable<DestinationAndXmlBasedImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<DestinationAndXmlBasedImporter> CreateElementHandlerTable() {
			ElementHandlerTable<DestinationAndXmlBasedImporter> result = new ElementHandlerTable<DestinationAndXmlBasedImporter>();
			result.Add("cs", OnComplexScript);
			result.Add("ea", OnEastAsian);
			result.Add("effectDag", OnEffectDAG);
			result.Add("effectLst", OnEffectList);
			result.Add("highlight", OnHighlightColor);
			result.Add("latin", OnLatin);
			result.Add("ln", OnOutline);
			result.Add("sym", OnSymbol);
			result.Add("uFill", OnUnderlineFill);
			result.Add("uFillTx", OnUnderlineFillFollowsText);
			result.Add("uLn", OnStrokeUnderline);
			result.Add("uLnTx", OnUnderlineFollowsText);
			AddFillHandlers(result);
			return result;
		}
		#endregion
		static DrawingTextCharacterPropertiesDestination GetThis(DestinationAndXmlBasedImporter importer) {
			return (DrawingTextCharacterPropertiesDestination)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnComplexScript(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new DrawingTextFontDestination(importer, GetThis(importer).properties.ComplexScript);
		}
		static Destination OnEastAsian(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new DrawingTextFontDestination(importer, GetThis(importer).properties.EastAsian);
		}
		static Destination OnEffectDAG(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new DrawingEffectsDAGDestination(importer, GetThis(importer).properties.Effects);
		}
		static Destination OnEffectList(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new DrawingEffectsListDestination(importer, GetThis(importer).properties.Effects);
		}
		static Destination OnHighlightColor(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new DrawingColorDestination(importer, GetThis(importer).properties.Highlight);
		}
		static Destination OnLatin(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new DrawingTextFontDestination(importer, GetThis(importer).properties.Latin);
		}
		static Destination OnOutline(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new OutlineDestination(importer, GetThis(importer).properties.Outline);
		}
		static Destination OnSymbol(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new DrawingTextFontDestination(importer, GetThis(importer).properties.Symbol);
		}
		static Destination OnUnderlineFill(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new DrawingTextCharacterPropertiesUnderlineFill(importer, GetThis(importer).properties);
		}
		static Destination OnUnderlineFillFollowsText(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			DrawingTextCharacterPropertiesDestination destination = GetThis(importer);
			destination.properties.UnderlineFill = DrawingUnderlineFill.FollowsText;
			return new EmptyDestination<DestinationAndXmlBasedImporter>(importer);
		}
		static Destination OnStrokeUnderline(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			DrawingTextCharacterPropertiesDestination destination = GetThis(importer);
			Outline outline = new Outline(importer.ActualDocumentModel);
			destination.properties.StrokeUnderline = outline;
			return new OutlineDestination(importer, outline);
		}
		static Destination OnUnderlineFollowsText(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			DrawingTextCharacterPropertiesDestination destination = GetThis(importer);
			destination.properties.StrokeUnderline = DrawingStrokeUnderline.FollowsText;
			return new EmptyDestination<DestinationAndXmlBasedImporter>(importer);
		}
		#endregion
		#endregion
		readonly DrawingTextCharacterProperties properties;
		public DrawingTextCharacterPropertiesDestination(DestinationAndXmlBasedImporter importer, DrawingTextCharacterProperties properties)
			: base(importer) {
			this.properties = properties;
		}
		protected override ElementHandlerTable<DestinationAndXmlBasedImporter> ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
			properties.BeginUpdate();
			DrawingTextCharacterInfo defaultInfo = DrawingTextCharacterInfo.DefaultInfo;
			string alternateLanguage = Importer.ReadAttribute(reader, "altLang");
			if (!String.IsNullOrEmpty(alternateLanguage))
				properties.AlternateLanguage = new CultureInfo(alternateLanguage);
			string language = Importer.ReadAttribute(reader, "lang");
			if (!String.IsNullOrEmpty(language))
				properties.Language = new CultureInfo(language);
			bool? bold = Importer.GetWpSTOnOffNullValue(reader, "b");
			if (bold.HasValue)
				properties.Bold = bold.Value;
			int? baseline = Importer.GetIntegerNullableValue(reader, "baseline");
			if (baseline.HasValue)
				properties.Baseline = baseline.Value;
			string bookmark = Importer.ReadAttribute(reader, "bmk");
			if (!String.IsNullOrEmpty(bookmark))
				properties.Bookmark = bookmark;
			DrawingTextCapsType? caps = Importer.GetWpEnumOnOffNullValue(reader, "cap", OpenXmlExporter.CapsTypeTable);
			if (caps.HasValue)
				properties.Caps = caps.Value;
			properties.Dirty = Importer.GetOnOffValue(reader, "dirty", defaultInfo.Dirty);
			properties.SpellingError = Importer.GetOnOffValue(reader, "err", defaultInfo.SpellingError);
			bool? italic = Importer.GetWpSTOnOffNullValue(reader, "i");
			if (italic.HasValue)
				properties.Italic = italic.Value;
			int? kerning = Importer.GetIntegerNullableValue(reader, "kern");
			if (kerning.HasValue)
				properties.Kerning = kerning.Value;
			bool? kumimoji = Importer.GetWpSTOnOffNullValue(reader, "kumimoji");
			if (kumimoji.HasValue)
				properties.Kumimoji = kumimoji.Value;
			bool? noProofing = Importer.GetWpSTOnOffNullValue(reader, "noProof");
			if (noProofing.HasValue)
				properties.NoProofing = noProofing.Value;
			bool? normalizeHeight = Importer.GetWpSTOnOffNullValue(reader, "normalizeH");
			if (normalizeHeight.HasValue)
				properties.NormalizeHeight = normalizeHeight.Value;
			properties.SmartTagClean = Importer.GetOnOffValue(reader, "smtClean", defaultInfo.SmartTagClean);
			properties.SmartTagId = Importer.GetIntegerValue(reader, "smtId", defaultInfo.SmartTagId);
			int? spacing = Importer.GetIntegerNullableValue(reader, "spc");
			if (spacing.HasValue)
				properties.Spacing = spacing.Value;
			DrawingTextStrikeType? strikethrough = Importer.GetWpEnumOnOffNullValue(reader, "strike", OpenXmlExporter.StrikeTypeTable);
			if (strikethrough.HasValue)
				properties.Strikethrough = strikethrough.Value;
			int? fontSize = Importer.GetIntegerNullableValue(reader, "sz");
			if (fontSize.HasValue)
				properties.FontSize = fontSize.Value;
			DrawingTextUnderlineType? underline = Importer.GetWpEnumOnOffNullValue(reader, "u", OpenXmlExporter.UnderlineTypeTable);
			if (underline.HasValue)
				properties.Underline = underline.Value;
		}
		public override void ProcessElementClose(XmlReader reader) {
			if (Fill != null)
				properties.Fill = Fill;
			properties.EndUpdate();
		}
	}
	#endregion
	#region DrawingTextCharacterPropertiesUnderlineFill
	public class DrawingTextCharacterPropertiesUnderlineFill : DrawingFillDestinationBase {
		#region Handler Table
		static readonly ElementHandlerTable<DestinationAndXmlBasedImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<DestinationAndXmlBasedImporter> CreateElementHandlerTable() {
			ElementHandlerTable<DestinationAndXmlBasedImporter> result = new ElementHandlerTable<DestinationAndXmlBasedImporter>();
			AddFillHandlers(result);
			return result;
		}
		#endregion
		readonly DrawingTextCharacterProperties properties;
		public DrawingTextCharacterPropertiesUnderlineFill(DestinationAndXmlBasedImporter importer, DrawingTextCharacterProperties properties)
			: base(importer) {
			this.properties = properties;
		}
		protected override ElementHandlerTable<DestinationAndXmlBasedImporter> ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementClose(XmlReader reader) {
			if (Fill != null)
				properties.UnderlineFill = Fill as IUnderlineFill;
		}
	}
	#endregion
}
