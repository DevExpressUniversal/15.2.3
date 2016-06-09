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
using System.Xml;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Export.OpenXml;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Office;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Import.OpenXml {
	#region ParagraphPropertiesBaseDestination (abstract class)
	public abstract class ParagraphPropertiesBaseDestination : ElementDestination {
		readonly IParagraphProperties paragraphProperties;
		readonly TabFormattingInfo tabs;
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		protected static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("rPr", OnRunProperties);
			result.Add("spacing", OnSpacing);
			result.Add("ind", OnIndents);
			result.Add("suppressAutoHyphens", OnSuppressHyphenation);
			result.Add("suppressLineNumbers", OnSuppressLineNumbers);
			result.Add("supressLineNumbers", OnSuppressLineNumbers); 
			result.Add("contextualSpacing", OnContextualSpacing);
			result.Add("pageBreakBefore", OnPageBreakBefore);
			result.Add("keepNext", OnKeepWithNext);
			result.Add("keepLines", OnKeepLinesTogether);
			result.Add("widowControl", OnWidowOrphanControl);
			result.Add("jc", OnAlignment);
			result.Add("tabs", OnTabs);
			result.Add("outlineLvl", OnOutlineLevel);
			result.Add("shd", OnBackground);
			result.Add("framePr", OnFrameProperties);
			result.Add("pBdr", OnParagraphBorders);
			return result;
		}
		protected ParagraphPropertiesBaseDestination(WordProcessingMLBaseImporter importer, IParagraphProperties paragraphProperties, TabFormattingInfo tabs)
			: base(importer) {
			Guard.ArgumentNotNull(paragraphProperties, "paragraphProperties");
			Guard.ArgumentNotNull(tabs, "tabs");
			this.paragraphProperties = paragraphProperties;
			this.tabs = tabs;
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		protected internal IParagraphProperties ParagraphFormatting { get { return paragraphProperties; } }
		public abstract int NumberingId { get; set; }
		public abstract int ListLevelIndex { get; set; }
		static ParagraphPropertiesBaseDestination GetThis(WordProcessingMLBaseImporter importer) {
			return (ParagraphPropertiesBaseDestination)importer.PeekDestination();
		}
		static IParagraphProperties GetParagraphProperties(WordProcessingMLBaseImporter importer) {
			return GetThis(importer).paragraphProperties;
		}
		static TabFormattingInfo GetTabs(WordProcessingMLBaseImporter importer) {
			return GetThis(importer).tabs;
		}
		static Destination OnRunProperties(WordProcessingMLBaseImporter importer, XmlReader reader) {
			importer.Position.ParagraphMarkCharacterFormatting.ReplaceInfo(importer.DocumentModel.Cache.CharacterFormattingInfoCache.DefaultItem, new CharacterFormattingOptions(CharacterFormattingOptions.Mask.UseNone));
			return new ParagraphMarkRunPropertiesDestination(importer, importer.Position.ParagraphMarkCharacterFormatting);
		}
		static Destination OnSpacing(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new ParagraphSpacingDestination(importer, GetParagraphProperties(importer));
		}
		static Destination OnIndents(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new ParagraphIndentsDestination(importer, GetParagraphProperties(importer));
		}
		static Destination OnSuppressHyphenation(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new SuppressHyphenationDestination(importer, GetParagraphProperties(importer));
		}
		static Destination OnSuppressLineNumbers(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new SuppressLineNumbersDestination(importer, GetParagraphProperties(importer));
		}
		static Destination OnContextualSpacing(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new ContextualSpacingDestination(importer, GetParagraphProperties(importer));
		}
		static Destination OnPageBreakBefore(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new PageBreakBeforeDestination(importer, GetParagraphProperties(importer));
		}
		static Destination OnKeepWithNext(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new KeepWithNextDestination(importer, GetParagraphProperties(importer));
		}
		static Destination OnKeepLinesTogether(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new KeepLinesTogetherDestination(importer, GetParagraphProperties(importer));
		}
		static Destination OnWidowOrphanControl(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new WidowOrphanControlDestination(importer, GetParagraphProperties(importer));
		}
		static Destination OnAlignment(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new ParagraphAlignmentDestination(importer, GetParagraphProperties(importer));
		}
		static Destination OnTabs(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TabsDestination(importer, GetTabs(importer));
		}
		static Destination OnOutlineLevel(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new OutlineLevelDestination(importer, GetParagraphProperties(importer));
		}
		static Destination OnBackground(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new ParagraphBackgroundDestination(importer, GetParagraphProperties(importer));
		}
		static Destination OnFrameProperties(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new FramePropertiesDestination(importer);
		}
		static Destination OnParagraphBorders(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new ParagraphBordersDestination(importer, GetParagraphProperties(importer));
		}
	}
	#endregion
	#region ParagraphPropertiesDestination
	public class ParagraphPropertiesDestination : ParagraphPropertiesBaseDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static new ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = ParagraphPropertiesBaseDestination.CreateElementHandlerTable();
			result.Add("pStyle", OnStyle);
			result.Add("sectPr", OnSection);
			result.Add("numPr", OnNumbering);
			return result;
		}
		readonly ParagraphDestination paragraphDestination;
		public ParagraphPropertiesDestination(WordProcessingMLBaseImporter importer, ParagraphDestination paragraphDestination, ParagraphFormattingBase paragraphFormatting, TabFormattingInfo tabs)
			: base(importer, paragraphFormatting, tabs) {
			Guard.ArgumentNotNull(paragraphDestination, "paragraphDestination");
			this.paragraphDestination = paragraphDestination;
			importer.Position.ParagraphMarkCharacterFormatting.ReplaceInfo(importer.DocumentModel.Cache.CharacterFormattingInfoCache.DefaultItem, new CharacterFormattingOptions(CharacterFormattingOptions.Mask.UseNone));
			importer.Position.ParagraphMarkCharacterStyleIndex = 0;
			importer.Position.ParagraphStyleIndex = 0;
			tabs.Clear();
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public override int ListLevelIndex { get { return paragraphDestination.ListLevelIndex; } set { paragraphDestination.ListLevelIndex = value; } }
		public override int NumberingId { get { return paragraphDestination.NumberingId; } set { paragraphDestination.NumberingId = value; } }
		protected internal virtual SectionDestinationBase CreateSectionDestination() {
			return new InnerSectionDestination(Importer);
		}
		public override void ProcessElementOpen(XmlReader reader) {
			ParagraphFormattingBase paragraphFormatting = (ParagraphFormattingBase)ParagraphFormatting;
			paragraphFormatting.BeginUpdate();
		}
		public override void ProcessElementClose(XmlReader reader) {
			ParagraphFormattingBase paragraphFormatting = (ParagraphFormattingBase)ParagraphFormatting;
			paragraphFormatting.EndUpdate();
		}
		static ParagraphPropertiesDestination GetThis(WordProcessingMLBaseImporter importer) {
			return (ParagraphPropertiesDestination)importer.PeekDestination();
		}
		protected static Destination OnStyle(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new ParagraphStyleReferenceDestination(importer);
		}
		protected static Destination OnSection(WordProcessingMLBaseImporter importer, XmlReader reader) {
			ParagraphPropertiesDestination thisDestination = GetThis(importer);
			thisDestination.paragraphDestination.ShouldInsertSection = importer.DocumentModel.DocumentCapabilities.SectionsAllowed && !importer.InsideTable;
			return thisDestination.CreateSectionDestination();
		}
		static Destination OnNumbering(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new ParagraphNumberingReferenceDestination(importer, GetThis(importer));
		}
	}
	#endregion
	#region ParagraphMarkRunPropertiesDestination
	public class ParagraphMarkRunPropertiesDestination : RunPropertiesBaseDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static new ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = RunPropertiesBaseDestination.CreateElementHandlerTable();
			result.Add("rStyle", OnStyle);
			return result;
		}
		public ParagraphMarkRunPropertiesDestination(WordProcessingMLBaseImporter importer, CharacterFormattingBase characterFormatting)
			: base(importer, characterFormatting) {
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
			return new ParagraphMarkRunStyleReferenceDestination(importer);
		}
	}
	#endregion
	#region ParagraphMarkRunStyleReferenceDestination
	public class ParagraphMarkRunStyleReferenceDestination : RunStyleReferenceBaseDestination {
		public ParagraphMarkRunStyleReferenceDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override void AssignCharacterStyleIndex(int value) {
			Importer.Position.ParagraphMarkCharacterStyleIndex = value;
		}
	}
	#endregion
	#region ParagraphStyleReferenceBaseDestination
	public abstract class ParagraphStyleReferenceBaseDestination : LeafElementDestination {
		protected ParagraphStyleReferenceBaseDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string value = reader.GetAttribute("val", Importer.WordProcessingNamespaceConst);
			if (!String.IsNullOrEmpty(value)) {
				int styleIndex = LookupStyleIndex(value);
				if (styleIndex >= 0)
					AssignParagraphStyleIndex(styleIndex);
			}
		}
		int LookupStyleIndex(string value) {
			return Importer.LookupParagraphStyleIndex(value);
		}
		protected internal abstract void AssignParagraphStyleIndex(int value);
	}
	#endregion
	#region ParagraphStyleReferenceDestination
	public class ParagraphStyleReferenceDestination : ParagraphStyleReferenceBaseDestination {
		public ParagraphStyleReferenceDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override void AssignParagraphStyleIndex(int value) {
			Importer.Position.ParagraphStyleIndex = value;
		}
	}
	#endregion
	public class ParagraphNumberingReferenceDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("ilvl", OnLevel);
			result.Add("numId", OnNumberingId);
			return result;
		}
		readonly ParagraphPropertiesBaseDestination paragraphPropertiesDestination;
		public ParagraphNumberingReferenceDestination(WordProcessingMLBaseImporter importer, ParagraphPropertiesBaseDestination paragraphPropertiesDestination)
			: base(importer) {
			Guard.ArgumentNotNull(paragraphPropertiesDestination, "paragraphPropertiesDestination");
			this.paragraphPropertiesDestination = paragraphPropertiesDestination;
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public int ListLevelIndex { get { return paragraphPropertiesDestination.ListLevelIndex; } set { paragraphPropertiesDestination.ListLevelIndex = value; } }
		public int NumberingId { get { return paragraphPropertiesDestination.NumberingId; } set { paragraphPropertiesDestination.NumberingId = value; } }
		static ParagraphNumberingReferenceDestination GetThis(WordProcessingMLBaseImporter importer) {
			return (ParagraphNumberingReferenceDestination)importer.PeekDestination();
		}
		protected static Destination OnLevel(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new ParagraphNumberingReferenceLevelDestination(importer, GetThis(importer));
		}
		protected static Destination OnNumberingId(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new ParagraphNumberingReferenceNumberingIdDestination(importer, GetThis(importer));
		}
	}
	public class ParagraphNumberingReferenceLevelDestination : LeafElementDestination {
		readonly ParagraphNumberingReferenceDestination parentDestination;
		public ParagraphNumberingReferenceLevelDestination(WordProcessingMLBaseImporter importer, ParagraphNumberingReferenceDestination parentDestination)
			: base(importer) {
			Guard.ArgumentNotNull(parentDestination, "parentDestination");
			this.parentDestination = parentDestination;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			parentDestination.ListLevelIndex = Math.Max(0, Math.Min(8, Importer.GetWpSTIntegerValue(reader, "val", -1)));
		}
	}
	public class ParagraphNumberingReferenceNumberingIdDestination : LeafElementDestination {
		readonly ParagraphNumberingReferenceDestination parentDestination;
		public ParagraphNumberingReferenceNumberingIdDestination(WordProcessingMLBaseImporter importer, ParagraphNumberingReferenceDestination parentDestination)
			: base(importer) {
			Guard.ArgumentNotNull(parentDestination, "parentDestination");
			this.parentDestination = parentDestination;
		}
		public override void ProcessElementOpen(XmlReader reader) {						
			int numberingId = Importer.GetWpSTIntegerValue(reader, "val", Int32.MinValue);;
			if (numberingId == 0)
				numberingId = ((IConvertToInt<NumberingListIndex>)NumberingListIndex.NoNumberingList).ToInt();
			parentDestination.NumberingId = numberingId;
		}
	}
	#region ParagraphFormattingLeafElementDestination (abstract class)
	public abstract class ParagraphFormattingLeafElementDestination : LeafElementDestination {
		readonly IParagraphProperties paragraphProperties;
		protected ParagraphFormattingLeafElementDestination(WordProcessingMLBaseImporter importer, IParagraphProperties paragraphProperties)
			: base(importer) {
			Guard.ArgumentNotNull(paragraphProperties, "paragraphProperties");
			this.paragraphProperties = paragraphProperties;
		}
		public IParagraphProperties ParagraphProperties { get { return paragraphProperties; } }
	}
	#endregion
	#region ParagraphSpacingDestination
	public class ParagraphSpacingDestination : ParagraphFormattingLeafElementDestination {
		public ParagraphSpacingDestination(WordProcessingMLBaseImporter importer, IParagraphProperties paragraphProperties)
			: base(importer, paragraphProperties) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			int spacingAfter = Importer.GetWpSTIntegerValue(reader, "after", Int32.MinValue);
			if (spacingAfter != Int32.MinValue)
				ParagraphProperties.SpacingAfter = UnitConverter.TwipsToModelUnits(spacingAfter);
			int spacingBefore = Importer.GetWpSTIntegerValue(reader, "before", Int32.MinValue);
			if (spacingBefore != Int32.MinValue)
				ParagraphProperties.SpacingBefore = UnitConverter.TwipsToModelUnits(spacingBefore);
			ParagraphProperties.BeforeAutoSpacing = Importer.GetWpSTOnOffValue(reader, Importer.GetWordProcessingMLValue(new WordProcessingMLValue("beforeAutospacing", "before-autospacing")), false);
			ParagraphProperties.AfterAutoSpacing = Importer.GetWpSTOnOffValue(reader, Importer.GetWordProcessingMLValue(new WordProcessingMLValue("afterAutospacing", "after-autospacing")), false);
			int lineSpacing = Importer.GetWpSTIntegerValue(reader, "line", Int32.MinValue);
			if (lineSpacing != Int32.MinValue && lineSpacing > 0) {
				WordProcessingMLValue attribute = new WordProcessingMLValue("lineRule", "line-rule");
				string lineSpacingRule = reader.GetAttribute(Importer.GetWordProcessingMLValue(attribute), Importer.WordProcessingNamespaceConst);
				ApplyLineSpacingValue(lineSpacing, lineSpacingRule);
			}
			else if(lineSpacing != Int32.MinValue)
				ParagraphProperties.LineSpacingType = ParagraphLineSpacing.Single;
		}
		protected internal virtual void ApplyLineSpacingValue(int lineSpacing, string lineSpacingRule) {
			switch (lineSpacingRule) {
				case "at-least":
				case "atLeast":
					ParagraphProperties.LineSpacingType = ParagraphLineSpacing.AtLeast;
					ParagraphProperties.LineSpacing = UnitConverter.TwipsToModelUnits(lineSpacing);
					break;
				case "exact":
					ParagraphProperties.LineSpacingType = ParagraphLineSpacing.Exactly;
					ParagraphProperties.LineSpacing = UnitConverter.TwipsToModelUnits(lineSpacing);
					break;
				default:
					if (lineSpacing == 240)
						ParagraphProperties.LineSpacingType = ParagraphLineSpacing.Single;
					else if (lineSpacing == 360)
						ParagraphProperties.LineSpacingType = ParagraphLineSpacing.Sesquialteral;
					else if (lineSpacing == 480)
						ParagraphProperties.LineSpacingType = ParagraphLineSpacing.Double;
					else {
						ParagraphProperties.LineSpacingType = ParagraphLineSpacing.Multiple;
						ParagraphProperties.LineSpacing = lineSpacing / 240.0f;
					}
					break;
			}
		}
	}
	#endregion
	#region ParagraphIndentsDestination
	public class ParagraphIndentsDestination : ParagraphFormattingLeafElementDestination {
		public ParagraphIndentsDestination(WordProcessingMLBaseImporter importer, IParagraphProperties paragraphProperties)
			: base(importer, paragraphProperties) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			int left = Importer.GetWpSTIntegerValue(reader, "left", Int32.MinValue);
			if (left != Int32.MinValue)
				ParagraphProperties.LeftIndent = UnitConverter.TwipsToModelUnits(left);
			int right = Importer.GetWpSTIntegerValue(reader, "right", Int32.MinValue);
			if (right != Int32.MinValue)
				ParagraphProperties.RightIndent = UnitConverter.TwipsToModelUnits(right);
			WordProcessingMLValue firstLineAttributeName = new WordProcessingMLValue("firstLine", "first-line");
			int firstLine = Importer.GetWpSTIntegerValue(reader, Importer.GetWordProcessingMLValue(firstLineAttributeName), Int32.MinValue);
			if (firstLine != Int32.MinValue) {
				if (firstLine > 0)
					ParagraphProperties.FirstLineIndentType = ParagraphFirstLineIndent.Indented;
				else if (firstLine < 0)
					ParagraphProperties.FirstLineIndentType = ParagraphFirstLineIndent.Hanging;
				else
					ParagraphProperties.FirstLineIndentType = ParagraphFirstLineIndent.None;
				ParagraphProperties.FirstLineIndent = UnitConverter.TwipsToModelUnits(Math.Abs(firstLine));
			}
			int hanging = Importer.GetWpSTIntegerValue(reader, "hanging", Int32.MinValue);
			if (hanging != Int32.MinValue) {
				ParagraphProperties.FirstLineIndentType = ParagraphFirstLineIndent.Hanging;
				ParagraphProperties.FirstLineIndent = UnitConverter.TwipsToModelUnits(hanging);
			}
		}
	}
	#endregion
	#region ParagraphAlignmentDestination
	public class ParagraphAlignmentDestination : ParagraphFormattingLeafElementDestination {
		public ParagraphAlignmentDestination(WordProcessingMLBaseImporter importer, IParagraphProperties paragraphProperties)
			: base(importer, paragraphProperties) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string value = reader.GetAttribute("val", Importer.WordProcessingNamespaceConst);
			if (!String.IsNullOrEmpty(value))
				ParagraphProperties.Alignment = Importer.GetWpEnumValueCore(value, OpenXmlExporter.paragraphAlignmentTable, ParagraphAlignment.Left);
		}
	}
	#endregion
	#region SuppressHyphenationDestination
	public class SuppressHyphenationDestination : ParagraphFormattingLeafElementDestination {
		public SuppressHyphenationDestination(WordProcessingMLBaseImporter importer, IParagraphProperties paragraphProperties)
			: base(importer, paragraphProperties) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			ParagraphProperties.SuppressHyphenation = Importer.GetWpSTOnOffValue(reader, "val");
		}
	}
	#endregion
	#region SuppressLineNumbersDestination
	public class SuppressLineNumbersDestination : ParagraphFormattingLeafElementDestination {
		public SuppressLineNumbersDestination(WordProcessingMLBaseImporter importer, IParagraphProperties paragraphProperties)
			: base(importer, paragraphProperties) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			ParagraphProperties.SuppressLineNumbers = Importer.GetWpSTOnOffValue(reader, "val");
		}
	}
	#endregion
	#region ContextualSpacingDestination
	public class ContextualSpacingDestination : ParagraphFormattingLeafElementDestination {
		public ContextualSpacingDestination(WordProcessingMLBaseImporter importer, IParagraphProperties paragraphProperties)
			: base(importer, paragraphProperties) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			ParagraphProperties.ContextualSpacing = Importer.GetWpSTOnOffValue(reader, "val");
		}
	}
	#endregion
	#region PageBreakBeforeDestination
	public class PageBreakBeforeDestination : ParagraphFormattingLeafElementDestination {
		public PageBreakBeforeDestination(WordProcessingMLBaseImporter importer, IParagraphProperties paragraphProperties)
			: base(importer, paragraphProperties) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			ParagraphProperties.PageBreakBefore = Importer.GetWpSTOnOffValue(reader, "val");
		}
	}
	#endregion
	#region KeepWithNextDestination
	public class KeepWithNextDestination : ParagraphFormattingLeafElementDestination {
		public KeepWithNextDestination(WordProcessingMLBaseImporter importer, IParagraphProperties paragraphProperties)
			: base(importer, paragraphProperties) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			ParagraphProperties.KeepWithNext = Importer.GetWpSTOnOffValue(reader, "val");
		}
	}
	#endregion
	#region KeepLinesTogetherDestination
	public class KeepLinesTogetherDestination : ParagraphFormattingLeafElementDestination {
		public KeepLinesTogetherDestination(WordProcessingMLBaseImporter importer, IParagraphProperties paragraphProperties)
			: base(importer, paragraphProperties) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			ParagraphProperties.KeepLinesTogether = Importer.GetWpSTOnOffValue(reader, "val");
		}
	}
	#endregion
	#region WidowOrphanControlDestination
	public class WidowOrphanControlDestination : ParagraphFormattingLeafElementDestination {
		public WidowOrphanControlDestination(WordProcessingMLBaseImporter importer, IParagraphProperties paragraphProperties)
			: base(importer, paragraphProperties) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			ParagraphProperties.WidowOrphanControl = Importer.GetWpSTOnOffValue(reader, "val");
		}
	}
	#endregion
	#region OutlineLevelDestination
	public class OutlineLevelDestination : ParagraphFormattingLeafElementDestination {
		public OutlineLevelDestination(WordProcessingMLBaseImporter importer, IParagraphProperties paragraphProperties)
			: base(importer, paragraphProperties) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			int level = Importer.GetWpSTIntegerValue(reader, "val", 9);
			if (level < 0 || level >= 9)
				level = 0;
			else
				level++;
			ParagraphProperties.OutlineLevel = level;
		}
	}
	#endregion
	#region ParagraphBackgroundDestination
	public class ParagraphBackgroundDestination : ParagraphFormattingLeafElementDestination {
		public ParagraphBackgroundDestination(WordProcessingMLBaseImporter importer, IParagraphProperties paragraphProperties)
			: base(importer, paragraphProperties) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			ShadingPattern pattern = Importer.GetWpEnumValue<ShadingPattern>(reader, "val", OpenXmlExporter.shadingPatternTable, ShadingPattern.Clear);
			Color fill = Importer.GetWpSTColorValue(reader, "fill", DXColor.Transparent);
			Color patternColor = Importer.GetWpSTColorValue(reader, "color", DXColor.Transparent);
			Color actualColor = ShadingHelper.GetActualBackColor(fill, patternColor, pattern);
			if(actualColor != DXColor.Empty)
				ParagraphProperties.BackColor = actualColor;
#if THEMES_EDIT
			Shading shading = ParagraphProperties.Shading;
			shading.ShadingPattern = Importer.GetWpEnumValue<ShadingPattern>(reader, "val", OpenXmlExporter.shadingPatternTable, ShadingPattern.Clear);
			OpenXmlImportHelper helper = new OpenXmlImportHelper();
			shading.ColorInfo = helper.SaveColorModelInfo(Importer, reader, "color");
			shading.FillInfo = helper.SaveFillInfo(Importer, reader);
#endif
		}
	}
	#endregion
	#region FramePropertiesDestination
	public class FramePropertiesDestination : LeafElementDestination {
		readonly ParagraphFrameFormattingBase frameProperties;
		public FramePropertiesDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
				importer.Position.ParagraphFrameFormatting.ReplaceInfo(importer.DocumentModel.Cache.ParagraphFrameFormattingInfoCache.DefaultItem, new ParagraphFrameFormattingOptions(ParagraphFrameFormattingOptions.Mask.UseNone));
				frameProperties = importer.Position.ParagraphFrameFormatting;
		}
		public ParagraphFrameFormattingBase FrameProperties { get { return frameProperties; } }
		public override void ProcessElementOpen(XmlReader reader) {
			int width = Importer.GetWpSTIntegerValue(reader, "w", int.MinValue);
			if (width != int.MinValue)
				FrameProperties.Width = width;
			int height = Importer.GetWpSTIntegerValue(reader, "h", int.MinValue);
			if (height != int.MinValue) {
				FrameProperties.Height = height;
				FrameProperties.HorizontalRule = ParagraphFrameHorizontalRule.AtLeast;
			}
			string horizontalRule = reader.GetAttribute("hRule", Importer.WordProcessingNamespaceConst);
			ApplyHorizontalRuleValue(horizontalRule);
			int verticalPadding = Importer.GetWpSTIntegerValue(reader, "vSpace", int.MinValue);
			if (verticalPadding != int.MinValue)
				FrameProperties.VerticalPadding = verticalPadding;
			int horizontalPadding = Importer.GetWpSTIntegerValue(reader, "hSpace", int.MinValue);
			if (horizontalPadding != int.MinValue)
				FrameProperties.HorizontalPadding = horizontalPadding;
			string wrapType = reader.GetAttribute("wrap", Importer.WordProcessingNamespaceConst);
			ApplyWrapTypeValue(wrapType);
			string verticalPositionType = reader.GetAttribute("vAnchor", Importer.WordProcessingNamespaceConst);
			ApplyVerticalPositionTypeValue(verticalPositionType);
			string horizontalPositionType = reader.GetAttribute("hAnchor", Importer.WordProcessingNamespaceConst);
			ApplyHorizontalPositionTypeValue(horizontalPositionType);
			int x = Importer.GetWpSTIntegerValue(reader, "x", int.MinValue);
			if (x != int.MinValue)
				FrameProperties.X = x;
			string horizontalPositionAlignment = reader.GetAttribute("xAlign", Importer.WordProcessingNamespaceConst);
			ApplyHorizontalPositionAlignmentValue(horizontalPositionAlignment);
			int y = Importer.GetWpSTIntegerValue(reader, "y", int.MinValue);
			if (y != int.MinValue)
				FrameProperties.Y = y;
			string verticalPositionAlignment = reader.GetAttribute("yAlign", Importer.WordProcessingNamespaceConst);
			ApplyVerticalPositionAlignmentValue(verticalPositionAlignment);
		}
		protected internal virtual void ApplyHorizontalRuleValue(string horizontalRule) {
			switch (horizontalRule) {
				case "auto":
					FrameProperties.HorizontalRule = ParagraphFrameHorizontalRule.Auto;
					break;
				case "atLeast":
					FrameProperties.HorizontalRule = ParagraphFrameHorizontalRule.AtLeast;
					break;
				case "exact":
					FrameProperties.HorizontalRule = ParagraphFrameHorizontalRule.Exact;
					break;
			}
		}
		protected internal virtual void ApplyWrapTypeValue(string wrapType) {
			switch (wrapType) {
				case "auto":
					FrameProperties.TextWrapType = ParagraphFrameTextWrapType.Auto;
					break;
				case "around":
					FrameProperties.TextWrapType = ParagraphFrameTextWrapType.Around;
					break;
				case "none":
					FrameProperties.TextWrapType = ParagraphFrameTextWrapType.None;
					break;
				case "notBeside":
					FrameProperties.TextWrapType = ParagraphFrameTextWrapType.NotBeside;
					break;
				case "through":
					FrameProperties.TextWrapType = ParagraphFrameTextWrapType.Through;
					break;
				case "tight":
					FrameProperties.TextWrapType = ParagraphFrameTextWrapType.Tight;
					break;
			}
		}
		protected internal virtual void ApplyVerticalPositionTypeValue(string verticalPositionType) {
			switch (verticalPositionType) {
				case "margin":
					FrameProperties.VerticalPositionType = ParagraphFrameVerticalPositionType.Margin;
					break;
				case "page":
					FrameProperties.VerticalPositionType = ParagraphFrameVerticalPositionType.Page;
					break;
				case "text":
					FrameProperties.VerticalPositionType = ParagraphFrameVerticalPositionType.Paragraph;
					break;
			}
		}
		protected internal virtual void ApplyHorizontalPositionTypeValue(string horizontalPositionType) {
			switch (horizontalPositionType) {
				case "margin":
					FrameProperties.HorizontalPositionType = ParagraphFrameHorizontalPositionType.Margin;
					break;
				case "page":
					FrameProperties.HorizontalPositionType = ParagraphFrameHorizontalPositionType.Page;
					break;
				case "text":
					FrameProperties.HorizontalPositionType = ParagraphFrameHorizontalPositionType.Column;
					break;
			}
		}
		protected internal virtual void ApplyHorizontalPositionAlignmentValue(string horizontalPositionAlignment) {
			switch (horizontalPositionAlignment) {
				case "center":
					FrameProperties.HorizontalPositionAlignment = ParagraphFrameHorizontalPositionAlignment.Center;
					break;
				case "inside":
					FrameProperties.HorizontalPositionAlignment = ParagraphFrameHorizontalPositionAlignment.Inside;
					break;
				case "left":
					FrameProperties.HorizontalPositionAlignment = ParagraphFrameHorizontalPositionAlignment.Left;
					break;
				case "outside":
					FrameProperties.HorizontalPositionAlignment = ParagraphFrameHorizontalPositionAlignment.Outside;
					break;
				case "right":
					FrameProperties.HorizontalPositionAlignment = ParagraphFrameHorizontalPositionAlignment.Right;
					break;
			}
		}
		protected internal virtual void ApplyVerticalPositionAlignmentValue(string verticalPositionAlignment) {
			switch (verticalPositionAlignment) {
				case "bottom":
					FrameProperties.VerticalPositionAlignment = ParagraphFrameVerticalPositionAlignment.Bottom;
					break;
				case "center":
					FrameProperties.VerticalPositionAlignment = ParagraphFrameVerticalPositionAlignment.Center;
					break;
				case "inline":
					FrameProperties.VerticalPositionAlignment = ParagraphFrameVerticalPositionAlignment.Inline;
					break;
				case "inside":
					FrameProperties.VerticalPositionAlignment = ParagraphFrameVerticalPositionAlignment.Inside;
					break;
				case "outside":
					FrameProperties.VerticalPositionAlignment = ParagraphFrameVerticalPositionAlignment.Outside;
					break;
				case "top":
					FrameProperties.VerticalPositionAlignment = ParagraphFrameVerticalPositionAlignment.Top;
					break;
			}
		}
	}
	#endregion
	#region ParagraphBordersDestination
	public class ParagraphBordersDestination : ParagraphFormattingLeafElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("top", OnTopBorder);
			result.Add("left", OnLeftBorder);
			result.Add("bottom", OnBottomBorder);
			result.Add("right", OnRightBorder);
			return result;
		}
		public ParagraphBordersDestination(WordProcessingMLBaseImporter importer, IParagraphProperties paragraphProperties)
			: base(importer, paragraphProperties) {
			paragraphProperties.TopBorder = new BorderInfo();
			paragraphProperties.LeftBorder = new BorderInfo();
			paragraphProperties.BottomBorder = new BorderInfo();
			paragraphProperties.RightBorder = new BorderInfo();
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static ParagraphBordersDestination GetThis(WordProcessingMLBaseImporter importer) {
			return (ParagraphBordersDestination)importer.PeekDestination();
		}
		static IParagraphProperties GetParagraphProperties(WordProcessingMLBaseImporter importer) {
			return GetThis(importer).ParagraphProperties;
		}
		static Destination OnTopBorder(WordProcessingMLBaseImporter importer, XmlReader reader) {
			IParagraphProperties paragraphProperties = GetParagraphProperties(importer);
			return new ParagraphBorderDestination(importer, paragraphProperties.TopBorder);
		}
		static Destination OnLeftBorder(WordProcessingMLBaseImporter importer, XmlReader reader) {
			IParagraphProperties paragraphProperties = GetParagraphProperties(importer);
			return new ParagraphBorderDestination(importer, paragraphProperties.LeftBorder);
		}
		static Destination OnBottomBorder(WordProcessingMLBaseImporter importer, XmlReader reader) {
			IParagraphProperties paragraphProperties = GetParagraphProperties(importer);
			return new ParagraphBorderDestination(importer, paragraphProperties.BottomBorder);
		}
		static Destination OnRightBorder(WordProcessingMLBaseImporter importer, XmlReader reader) {
			IParagraphProperties paragraphProperties = GetParagraphProperties(importer);
			return new ParagraphBorderDestination(importer, paragraphProperties.RightBorder);
		}
	}
	#endregion
	#region ParagraphBorderDestination
	public class ParagraphBorderDestination : LeafElementDestination {
		static Dictionary<BorderLineStyle, string> borderStyleTable = CreateBorderStyleTable();
		static Dictionary<BorderLineStyle, string> CreateBorderStyleTable() {
			Dictionary<BorderLineStyle, string> result = new Dictionary<BorderLineStyle, string>();
			result.Add(BorderLineStyle.DashDotStroked, "dashDotStroked");
			result.Add(BorderLineStyle.Dashed, "dashed");
			result.Add(BorderLineStyle.DashSmallGap, "dashSmallGap");
			result.Add(BorderLineStyle.DotDash, "dotDash");
			result.Add(BorderLineStyle.DotDotDash, "dotDotDash");
			result.Add(BorderLineStyle.Dotted, "dotted");
			result.Add(BorderLineStyle.Double, "double");
			result.Add(BorderLineStyle.DoubleWave, "doubleWave");
			result.Add(BorderLineStyle.Inset, "inset");
			result.Add(BorderLineStyle.Disabled, "disabled");
			result.Add(BorderLineStyle.None, "none");
			result.Add(BorderLineStyle.Nil, "nil");
			result.Add(BorderLineStyle.Outset, "outset");
			result.Add(BorderLineStyle.Single, "single");
			result.Add(BorderLineStyle.Thick, "thick");
			result.Add(BorderLineStyle.ThickThinLargeGap, "thickThinLargeGap");
			result.Add(BorderLineStyle.ThickThinMediumGap, "thickThinMediumGap");
			result.Add(BorderLineStyle.ThickThinSmallGap, "thickThinSmallGap");
			result.Add(BorderLineStyle.ThinThickLargeGap, "thinThickLargeGap");
			result.Add(BorderLineStyle.ThinThickMediumGap, "thinThickMediumGap");
			result.Add(BorderLineStyle.ThinThickSmallGap, "thinThickSmallGap");
			result.Add(BorderLineStyle.ThinThickThinLargeGap, "thinThickThinLargeGap");
			result.Add(BorderLineStyle.ThinThickThinMediumGap, "thinThickThinMediumGap");
			result.Add(BorderLineStyle.ThinThickThinSmallGap, "thinThickThinSmallGap");
			result.Add(BorderLineStyle.ThreeDEmboss, "threeDEmboss");
			result.Add(BorderLineStyle.ThreeDEngrave, "threeDEngrave");
			result.Add(BorderLineStyle.Triple, "triple");
			result.Add(BorderLineStyle.Wave, "wave");
			result.Add(BorderLineStyle.Apples, "apples");
			result.Add(BorderLineStyle.ArchedScallops, "archedScallops");
			result.Add(BorderLineStyle.BabyPacifier, "babyPacifier");
			result.Add(BorderLineStyle.BabyRattle, "babyRattle");
			result.Add(BorderLineStyle.Balloons3Colors, "balloons3Colors");
			result.Add(BorderLineStyle.BalloonsHotAir, "balloonsHotAir");
			result.Add(BorderLineStyle.BasicBlackDashes, "basicBlackDashes");
			result.Add(BorderLineStyle.BasicBlackDots, "basicBlackDots");
			result.Add(BorderLineStyle.BasicBlackSquares, "basicBlackSquares");
			result.Add(BorderLineStyle.BasicThinLines, "basicThinLines");
			result.Add(BorderLineStyle.BasicWhiteDashes, "basicWhiteDashes");
			result.Add(BorderLineStyle.BasicWhiteDots, "basicWhiteDots");
			result.Add(BorderLineStyle.BasicWhiteSquares, "basicWhiteSquares");
			result.Add(BorderLineStyle.BasicWideInline, "basicWideInline");
			result.Add(BorderLineStyle.BasicWideMidline, "basicWideMidline");
			result.Add(BorderLineStyle.BasicWideOutline, "basicWideOutline");
			result.Add(BorderLineStyle.Bats, "bats");
			result.Add(BorderLineStyle.Birds, "birds");
			result.Add(BorderLineStyle.BirdsFlight, "birdsFlight");
			result.Add(BorderLineStyle.Cabins, "cabins");
			result.Add(BorderLineStyle.CakeSlice, "cakeSlice");
			result.Add(BorderLineStyle.CandyCorn, "candyCorn");
			result.Add(BorderLineStyle.CelticKnotwork, "celticKnotwork");
			result.Add(BorderLineStyle.CertificateBanner, "certificateBanner");
			result.Add(BorderLineStyle.ChainLink, "chainLink");
			result.Add(BorderLineStyle.ChampagneBottle, "champagneBottle");
			result.Add(BorderLineStyle.CheckedBarBlack, "checkedBarBlack");
			result.Add(BorderLineStyle.CheckedBarColor, "checkedBarColor");
			result.Add(BorderLineStyle.Checkered, "checkered");
			result.Add(BorderLineStyle.ChristmasTree, "christmasTree");
			result.Add(BorderLineStyle.CirclesLines, "circlesLines");
			result.Add(BorderLineStyle.CirclesRectangles, "circlesRectangles");
			result.Add(BorderLineStyle.ClassicalWave, "classicalWave");
			result.Add(BorderLineStyle.Clocks, "clocks");
			result.Add(BorderLineStyle.Compass, "compass");
			result.Add(BorderLineStyle.Confetti, "confetti");
			result.Add(BorderLineStyle.ConfettiGrays, "confettiGrays");
			result.Add(BorderLineStyle.ConfettiOutline, "confettiOutline");
			result.Add(BorderLineStyle.ConfettiStreamers, "confettiStreamers");
			result.Add(BorderLineStyle.ConfettiWhite, "confettiWhite");
			result.Add(BorderLineStyle.CornerTriangles, "cornerTriangles");
			result.Add(BorderLineStyle.CouponCutoutDashes, "couponCutoutDashes");
			result.Add(BorderLineStyle.CouponCutoutDots, "couponCutoutDots");
			result.Add(BorderLineStyle.CrazyMaze, "crazyMaze");
			result.Add(BorderLineStyle.CreaturesButterfly, "creaturesButterfly");
			result.Add(BorderLineStyle.CreaturesFish, "creaturesFish");
			result.Add(BorderLineStyle.CreaturesInsects, "creaturesInsects");
			result.Add(BorderLineStyle.CreaturesLadyBug, "creaturesLadyBug");
			result.Add(BorderLineStyle.CrossStitch, "crossStitch");
			result.Add(BorderLineStyle.Cup, "cup");
			result.Add(BorderLineStyle.DecoArch, "decoArch");
			result.Add(BorderLineStyle.DecoArchColor, "decoArchColor");
			result.Add(BorderLineStyle.DecoBlocks, "decoBlocks");
			result.Add(BorderLineStyle.DiamondsGray, "diamondsGray");
			result.Add(BorderLineStyle.DoubleD, "doubleD");
			result.Add(BorderLineStyle.DoubleDiamonds, "doubleDiamonds");
			result.Add(BorderLineStyle.Earth1, "earth1");
			result.Add(BorderLineStyle.Earth2, "earth2");
			result.Add(BorderLineStyle.EclipsingSquares1, "eclipsingSquares1");
			result.Add(BorderLineStyle.EclipsingSquares2, "eclipsingSquares2");
			result.Add(BorderLineStyle.EggsBlack, "eggsBlack");
			result.Add(BorderLineStyle.Fans, "fans");
			result.Add(BorderLineStyle.Film, "film");
			result.Add(BorderLineStyle.Firecrackers, "firecrackers");
			result.Add(BorderLineStyle.FlowersBlockPrint, "flowersBlockPrint");
			result.Add(BorderLineStyle.FlowersDaisies, "flowersDaisies");
			result.Add(BorderLineStyle.FlowersModern1, "flowersModern1");
			result.Add(BorderLineStyle.FlowersModern2, "flowersModern2");
			result.Add(BorderLineStyle.FlowersPansy, "flowersPansy");
			result.Add(BorderLineStyle.FlowersRedRose, "flowersRedRose");
			result.Add(BorderLineStyle.FlowersRoses, "flowersRoses");
			result.Add(BorderLineStyle.FlowersTeacup, "flowersTeacup");
			result.Add(BorderLineStyle.FlowersTiny, "flowersTiny");
			result.Add(BorderLineStyle.Gems, "gems");
			result.Add(BorderLineStyle.GingerbreadMan, "gingerbreadMan");
			result.Add(BorderLineStyle.Gradient, "gradient");
			result.Add(BorderLineStyle.Handmade1, "handmade1");
			result.Add(BorderLineStyle.Handmade2, "handmade2");
			result.Add(BorderLineStyle.HeartBalloon, "heartBalloon");
			result.Add(BorderLineStyle.HeartGray, "heartGray");
			result.Add(BorderLineStyle.Hearts, "hearts");
			result.Add(BorderLineStyle.HeebieJeebies, "heebieJeebies");
			result.Add(BorderLineStyle.Holly, "holly");
			result.Add(BorderLineStyle.HouseFunky, "houseFunky");
			result.Add(BorderLineStyle.Hypnotic, "hypnotic");
			result.Add(BorderLineStyle.IceCreamCones, "iceCreamCones");
			result.Add(BorderLineStyle.LightBulb, "lightBulb");
			result.Add(BorderLineStyle.Lightning1, "lightning1");
			result.Add(BorderLineStyle.Lightning2, "lightning2");
			result.Add(BorderLineStyle.MapleLeaf, "mapleLeaf");
			result.Add(BorderLineStyle.MapleMuffins, "mapleMuffins");
			result.Add(BorderLineStyle.MapPins, "mapPins");
			result.Add(BorderLineStyle.Marquee, "marquee");
			result.Add(BorderLineStyle.MarqueeToothed, "marqueeToothed");
			result.Add(BorderLineStyle.Moons, "moons");
			result.Add(BorderLineStyle.Mosaic, "mosaic");
			result.Add(BorderLineStyle.MusicNotes, "musicNotes");
			result.Add(BorderLineStyle.Northwest, "northwest");
			result.Add(BorderLineStyle.Ovals, "ovals");
			result.Add(BorderLineStyle.Packages, "packages");
			result.Add(BorderLineStyle.PalmsBlack, "palmsBlack");
			result.Add(BorderLineStyle.PalmsColor, "palmsColor");
			result.Add(BorderLineStyle.PaperClips, "paperClips");
			result.Add(BorderLineStyle.Papyrus, "papyrus");
			result.Add(BorderLineStyle.PartyFavor, "partyFavor");
			result.Add(BorderLineStyle.PartyGlass, "partyGlass");
			result.Add(BorderLineStyle.Pencils, "pencils");
			result.Add(BorderLineStyle.People, "people");
			result.Add(BorderLineStyle.PeopleHats, "peopleHats");
			result.Add(BorderLineStyle.PeopleWaving, "peopleWaving");
			result.Add(BorderLineStyle.Poinsettias, "poinsettias");
			result.Add(BorderLineStyle.PostageStamp, "postageStamp");
			result.Add(BorderLineStyle.Pumpkin1, "pumpkin1");
			result.Add(BorderLineStyle.PushPinNote1, "pushPinNote1");
			result.Add(BorderLineStyle.PushPinNote2, "pushPinNote2");
			result.Add(BorderLineStyle.Pyramids, "pyramids");
			result.Add(BorderLineStyle.PyramidsAbove, "pyramidsAbove");
			result.Add(BorderLineStyle.Quadrants, "quadrants");
			result.Add(BorderLineStyle.Rings, "rings");
			result.Add(BorderLineStyle.Safari, "safari");
			result.Add(BorderLineStyle.Sawtooth, "sawtooth");
			result.Add(BorderLineStyle.SawtoothGray, "sawtoothGray");
			result.Add(BorderLineStyle.ScaredCat, "scaredCat");
			result.Add(BorderLineStyle.Seattle, "seattle");
			result.Add(BorderLineStyle.ShadowedSquares, "shadowedSquares");
			result.Add(BorderLineStyle.SharksTeeth, "sharksTeeth");
			result.Add(BorderLineStyle.ShorebirdTracks, "shorebirdTracks");
			result.Add(BorderLineStyle.Skyrocket, "skyrocket");
			result.Add(BorderLineStyle.SnowflakeFancy, "snowflakeFancy");
			result.Add(BorderLineStyle.Snowflakes, "snowflakes");
			result.Add(BorderLineStyle.Sombrero, "sombrero");
			result.Add(BorderLineStyle.Southwest, "southwest");
			result.Add(BorderLineStyle.Stars, "stars");
			result.Add(BorderLineStyle.Stars3d, "stars3d");
			result.Add(BorderLineStyle.StarsBlack, "starsBlack");
			result.Add(BorderLineStyle.StarsShadowed, "starsShadowed");
			result.Add(BorderLineStyle.StarsTop, "starsTop");
			result.Add(BorderLineStyle.Sun, "sun");
			result.Add(BorderLineStyle.Swirligig, "swirligig");
			result.Add(BorderLineStyle.TornPaper, "tornPaper");
			result.Add(BorderLineStyle.TornPaperBlack, "tornPaperBlack");
			result.Add(BorderLineStyle.Trees, "trees");
			result.Add(BorderLineStyle.TriangleParty, "triangleParty");
			result.Add(BorderLineStyle.Triangles, "triangles");
			result.Add(BorderLineStyle.Tribal1, "tribal1");
			result.Add(BorderLineStyle.Tribal2, "tribal2");
			result.Add(BorderLineStyle.Tribal3, "tribal3");
			result.Add(BorderLineStyle.Tribal4, "tribal4");
			result.Add(BorderLineStyle.Tribal5, "tribal5");
			result.Add(BorderLineStyle.Tribal6, "tribal6");
			result.Add(BorderLineStyle.TwistedLines1, "twistedLines1");
			result.Add(BorderLineStyle.TwistedLines2, "twistedLines2");
			result.Add(BorderLineStyle.Vine, "vine");
			result.Add(BorderLineStyle.Waveline, "waveline");
			result.Add(BorderLineStyle.WeavingAngles, "weavingAngles");
			result.Add(BorderLineStyle.WeavingBraid, "weavingBraid");
			result.Add(BorderLineStyle.WeavingRibbon, "weavingRibbon");
			result.Add(BorderLineStyle.WeavingStrips, "weavingStrips");
			result.Add(BorderLineStyle.WhiteFlowers, "whiteFlowers");
			result.Add(BorderLineStyle.Woodwork, "woodwork");
			result.Add(BorderLineStyle.XIllusions, "xIllusions");
			result.Add(BorderLineStyle.ZanyTriangles, "zanyTriangles");
			result.Add(BorderLineStyle.ZigZag, "zigZag");
			result.Add(BorderLineStyle.ZigZagStitch, "zigZagStitch");
			return result;
		}
		readonly BorderInfo border;
		public ParagraphBorderDestination(WordProcessingMLBaseImporter importer, BorderInfo border)
			: base(importer) {
			Guard.ArgumentNotNull(border, "border");
			this.border = border;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			BorderLineStyle borderLineStyle = Importer.GetWpEnumValue<BorderLineStyle>(reader, "val", borderStyleTable, BorderLineStyle.None);
			Color color = Importer.GetWpSTColorValue(reader, "color");
			bool frame = Importer.GetWpSTOnOffValue(reader, "frame", false);
			bool shadow = Importer.GetWpSTOnOffValue(reader, "shadow", false);
			bool isDefaultValue = borderLineStyle == BorderLineStyle.None && Object.Equals(color, DXColor.Empty) &&
				frame == false && shadow == false;
			if (!isDefaultValue) {
				border.Style = borderLineStyle;
				border.Color = color;
				border.Frame = frame;
				border.Shadow = shadow;
			}
			int value = Importer.GetWpSTIntegerValue(reader, "space");
			if (value != Int32.MinValue)
				border.Offset = UnitConverter.PointsToModelUnits(value);
			value = Importer.GetWpSTIntegerValue(reader, "sz");
			if (value != Int32.MinValue)
				border.Width = (int)UnitConverter.PointsToModelUnitsF(value * 0.125f);
#if THEMES_EDIT
			OpenXmlImportHelper helper = new OpenXmlImportHelper();
			border.ColorModelInfo = helper.SaveColorModelInfo(Importer, reader, "color");
#endif
		}
	}
	#endregion
	#region TabsDestination
	public class TabsDestination : ElementDestination {
		readonly TabFormattingInfo tabs;
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		protected static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("tab", OnTab);
			return result;
		}
		public TabsDestination(WordProcessingMLBaseImporter importer, TabFormattingInfo tabs)
			: base(importer) {
			Guard.ArgumentNotNull(tabs, "tabs");
			this.tabs = tabs;
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static TabFormattingInfo GetTabs(WordProcessingMLBaseImporter importer) {
			TabsDestination thisObject = (TabsDestination)importer.PeekDestination();
			return thisObject.tabs;
		}
		static Destination OnTab(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TabDestination(importer, GetTabs(importer));
		}
	}
	#endregion
	#region TabsLeafElementDestination (abstract class)
	public abstract class TabsLeafElementDestination : LeafElementDestination {
		readonly TabFormattingInfo tabs;
		protected TabsLeafElementDestination(WordProcessingMLBaseImporter importer, TabFormattingInfo tabs)
			: base(importer) {
			Guard.ArgumentNotNull(tabs, "tabs");
			this.tabs = tabs;
		}
		public TabFormattingInfo Tabs { get { return tabs; } }
	}
	#endregion
	#region TabDestination
	public class TabDestination : TabsLeafElementDestination {
		public TabDestination(WordProcessingMLBaseImporter importer, TabFormattingInfo tabs)
			: base(importer, tabs) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			int pos = Importer.GetWpSTIntegerValue(reader, "pos", Int32.MinValue);
			if (pos == Int32.MinValue)
				return;
			TabLeaderType leader = Importer.GetWpEnumValue(reader, "leader", OpenXmlExporter.tabLeaderTable, TabLeaderType.None);
			string value = reader.GetAttribute("val", Importer.WordProcessingNamespaceConst);
			TabAlignmentType align;
			if (String.IsNullOrEmpty(value))
				align = TabAlignmentType.Left;
			else
				align = Importer.GetWpEnumValueCore(value, OpenXmlExporter.tabAlignmentTable, TabAlignmentType.Left);
			Tabs.Add(new TabInfo(UnitConverter.TwipsToModelUnits(pos), align, leader, (value == "clear")));
		}
	}
	#endregion
	#region InnerSectionDestination
	public class InnerSectionDestination : SectionDestination {
		public InnerSectionDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
	}
	#endregion
}
