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
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Export.OpenXml;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Office;
using DevExpress.Office.NumberConverters;
namespace DevExpress.XtraRichEdit.Import.OpenXml {
	#region NumberingsDestination
	public class NumberingsDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("abstractNum", OnAbstractNumbering);
			result.Add("num", OnNumbering);
			return result;
		}
		public NumberingsDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		protected static Destination OnAbstractNumbering(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new AbstractNumberingListDestination(importer);
		}
		protected static Destination OnNumbering(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new NumberingListDestination(importer);
		}
	}
	#endregion
	#region AbstractNumberingListDestination
	public class AbstractNumberingListDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("lvl", OnLevel);
			result.Add("multiLevelType", OnMultilevelType);
			result.Add("name", OnName);
			result.Add("nsid", OnUniqueId);
			result.Add("numStyleLink", OnNumberingStyleLink);
			result.Add("styleLink", OnStyleLink);
			result.Add("tmpl", OnTemplate);
			return result;
		}
		readonly AbstractNumberingList list;
		string id;
		protected internal AbstractNumberingList List { get { return list; } }
		protected internal string Id { get { return id; } set { id = value; } }
		public AbstractNumberingListDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
			this.list = new AbstractNumberingList(importer.DocumentModel);
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		protected static AbstractNumberingListDestination GetThis(WordProcessingMLBaseImporter importer) {
			return (AbstractNumberingListDestination)importer.PeekDestination();
		}
		public override void ProcessElementOpen(XmlReader reader) {
			this.id = reader.GetAttribute("abstractNumId", Importer.WordProcessingNamespaceConst);
		}
		public override void ProcessElementClose(XmlReader reader) {
			if (String.IsNullOrEmpty(id))
				return;
			AbstractNumberingListCollection lists = Importer.DocumentModel.AbstractNumberingLists;
			int index = lists.Count;
			lists.Add(list);
			OpenXmlAbstractNumberingInfo info = new OpenXmlAbstractNumberingInfo();
			info.AbstractNumberingListId = id;
			info.AbstractNumberingIndex = index;
			Importer.AbstractListInfos.Add(info);
		}
		static Destination OnLevel(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new NumberingLevelDestination(importer, GetThis(importer).list);
		}
		protected static Destination OnMultilevelType(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new NumberingListMultiLevelTypeDestination(importer, GetThis(importer).List);
		}
		protected static Destination OnName(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new NumberingListNameDestination(importer, GetThis(importer).list);
		}
		protected static Destination OnUniqueId(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new AbstractNumberingListUniqueIdDestination(importer, GetThis(importer).list);
		}
		protected static Destination OnNumberingStyleLink(WordProcessingMLBaseImporter importer, XmlReader reader) {			
			return new NumberingListNumStyleLinkDestination(importer, GetThis(importer).list);
		}
		protected static Destination OnStyleLink(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return null;
		}
		protected static Destination OnTemplate(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return null;
		}
	}
	#endregion
	#region NumberingListDestination
	public class NumberingListDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("abstractNumId", OnAbstractNumberingId);
			result.Add("lvlOverride", OnLevelOverride);
			return result;
		}
		readonly OpenXmlNumberingListInfo listInfo;
		public NumberingListDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
			this.listInfo = new OpenXmlNumberingListInfo();
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static NumberingListDestination GetThis(WordProcessingMLBaseImporter importer) {
			return (NumberingListDestination)importer.PeekDestination();
		}
		public override void ProcessElementOpen(XmlReader reader) {
			listInfo.Id = Importer.GetWpSTIntegerValue(reader, "numId", Int32.MinValue);
		}
		public override void ProcessElementClose(XmlReader reader) {
			if (String.IsNullOrEmpty(listInfo.AbstractNumberingListId) || listInfo.Id == Int32.MinValue)
				return;
			Importer.ListInfos.Add(listInfo);
		}
		static Destination OnAbstractNumberingId(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new AbstractNumberingListReferenceDestination(importer, GetThis(importer).listInfo);
		}
		static Destination OnLevelOverride(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new ListLevelOverrideDestination(importer, GetThis(importer).listInfo);
		}
	}
	#endregion
	#region AbstractNumberingLeafElementDestination (abstract class)
	public abstract class AbstractNumberingLeafElementDestination : LeafElementDestination {
		readonly AbstractNumberingList list;
		protected AbstractNumberingLeafElementDestination(WordProcessingMLBaseImporter importer, AbstractNumberingList list)
			: base(importer) {
			Guard.ArgumentNotNull(list, "list");
			this.list = list;
		}
		public AbstractNumberingList List { get { return list; } }
	}
	#endregion
	#region NumberingListNameDestination
	public class NumberingListNameDestination : AbstractNumberingLeafElementDestination {
		public NumberingListNameDestination(WordProcessingMLBaseImporter importer, AbstractNumberingList list)
			: base(importer, list) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
		}
	}
	#endregion
	#region AbstractNumberingListIdDestination
	public class AbstractNumberingListUniqueIdDestination : AbstractNumberingLeafElementDestination {
		public AbstractNumberingListUniqueIdDestination(WordProcessingMLBaseImporter importer, AbstractNumberingList list)
			: base(importer, list) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Int32 value = Importer.GetWpSTIntegerValue(reader, "val", NumberStyles.HexNumber, Int32.MinValue);
			if (value != Int32.MinValue) {
				List.SetId(Math.Abs((int)value));
			}
		}
	}
	#endregion
	#region NumberingListMultiLevelTypeDestination
	public class NumberingListMultiLevelTypeDestination : AbstractNumberingLeafElementDestination {
		public NumberingListMultiLevelTypeDestination(WordProcessingMLBaseImporter importer, AbstractNumberingList list)
			: base(importer, list) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			NumberingType listType = Importer.GetWpEnumValue(reader, "val", OpenXmlExporter.numberingListTypeTable, NumberingType.Simple);
			if (listType != NumberingType.MultiLevel)
				NumberingListHelper.SetHybridListType(List);
		}
	}
	#endregion
	#region NumberingListNumStyleLinkDestination
	public class NumberingListNumStyleLinkDestination : AbstractNumberingLeafElementDestination {
		public NumberingListNumStyleLinkDestination(WordProcessingMLBaseImporter importer, AbstractNumberingList list)
			: base(importer, list) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string styleId = reader.GetAttribute("val", Importer.WordProcessingNamespaceConst);
			OpenXmlStyleInfo styleInfo = Importer.NumberingStyleInfos.LookupStyleById(styleId);  
			if (styleInfo != null)
				List.SetNumberingStyleReferenceIndex(styleInfo.StyleIndex);
		}
	}
	#endregion
	#region AbstractNumberingListReferenceDestination
	public class AbstractNumberingListReferenceDestination : LeafElementDestination {
		readonly OpenXmlNumberingListInfo listInfo;
		public AbstractNumberingListReferenceDestination(WordProcessingMLBaseImporter importer, OpenXmlNumberingListInfo listInfo)
			: base(importer) {
			Guard.ArgumentNotNull(listInfo, "listInfo");
			this.listInfo = listInfo;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			listInfo.AbstractNumberingListId = reader.GetAttribute("val", Importer.WordProcessingNamespaceConst);
		}
	}
	#endregion
	#region ListLevelOverrideDestination
	public class ListLevelOverrideDestination : LeafElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("lvl", OnLevelOverride);
			result.Add("startOverride", OnLevelStartOverride);
			return result;
		}
		readonly OpenXmlNumberingListInfo listInfo;
		readonly OpenXmlListLevelOverride levelOverride;
		bool overrideRead;
		public ListLevelOverrideDestination(WordProcessingMLBaseImporter importer, OpenXmlNumberingListInfo listInfo)
			: base(importer) {
			Guard.ArgumentNotNull(listInfo, "listInfo");
			this.listInfo = listInfo;
			this.levelOverride = new OpenXmlListLevelOverride(importer.DocumentModel);
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
			levelOverride.LevelIndex = Importer.GetWpSTIntegerValue(reader, "ilvl", Int32.MinValue);
		}
		static ListLevelOverrideDestination GetThis(WordProcessingMLBaseImporter importer) {
			return (ListLevelOverrideDestination)importer.PeekDestination();
		}
		public override void ProcessElementClose(XmlReader reader) {
			if (levelOverride.LevelIndex < 0 || levelOverride.LevelIndex >= 9 || !overrideRead)
				return;
			listInfo.LevelOverrides.Add(levelOverride);
		}
		static Destination OnLevelOverride(WordProcessingMLBaseImporter importer, XmlReader reader) {
			ListLevelOverrideDestination thisObject = GetThis(importer);
			thisObject.overrideRead = true;
			return new ListLevelOverrideLevelDestination(importer, thisObject.levelOverride);
		}
		static Destination OnLevelStartOverride(WordProcessingMLBaseImporter importer, XmlReader reader) {
			ListLevelOverrideDestination thisObject = GetThis(importer);
			thisObject.overrideRead = true;
			return new ListLevelOverrideStartDestination(importer, thisObject.levelOverride);
		}
	}
	#endregion
	#region NumberingLevelBaseDestination (abstract class)
	public abstract class NumberingLevelBaseDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		protected static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("isLgl", OnLegalNumbering);
			result.Add("lvlJc", OnTextAlignment);
			result.Add("lvlRestart", OnRestart);
			result.Add("lvlText", OnText);
			result.Add("start", OnStart);
			result.Add("suff", OnSuffix);
			result.Add("numFmt", OnNumberingFormat);
			result.Add("pPr", OnParagraphProperties);
			result.Add("rPr", OnRunProperties);			
			result.Add("legacy", OnLegacy);
			return result;
		}
		protected NumberingLevelBaseDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		protected internal abstract IListLevel Level { get; }
		protected internal abstract int LevelIndex { get; }
		protected static NumberingLevelBaseDestination GetThis(WordProcessingMLBaseImporter importer) {
			return (NumberingLevelBaseDestination)importer.PeekDestination();
		}
		protected static Destination OnTextAlignment(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new ListLevelTextAlignmentDestination(importer, GetThis(importer).Level);
		}
		protected static Destination OnRestart(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new ListLevelRestartDestination(importer, GetThis(importer).Level, GetThis(importer).LevelIndex);
		}
		protected static Destination OnText(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new ListLevelFormatStringDestination(importer, GetThis(importer).Level);
		}
		protected static Destination OnStart(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new ListLevelStartDestination(importer, GetThis(importer).Level);
		}
		protected static Destination OnSuffix(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new ListLevelSuffixDestination(importer, GetThis(importer).Level);
		}
		protected static Destination OnNumberingFormat(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new ListLevelNumberingFormatDestination(importer, GetThis(importer).Level);
		}
		protected static Destination OnParagraphProperties(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new ListLevelParagraphPropertiesDestination(importer, GetThis(importer).Level, new TabFormattingInfo());
		}
		protected static Destination OnRunProperties(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new ListLevelRunPropertiesDestination(importer, GetThis(importer).Level);
		}
		protected static Destination OnLegalNumbering(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new ListLevelLegalNumberingDestination(importer, GetThis(importer).Level);
		}
		protected static Destination OnLegacy(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new ListLevelLegacyDestination(importer, GetThis(importer).Level);
		}
	}
	#endregion
	#region NumberingLevelDestination
	public class NumberingLevelDestination : NumberingLevelBaseDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		protected static new ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = NumberingLevelBaseDestination.CreateElementHandlerTable();
			result.Add("pStyle", OnParagraphStyleReference);
			return result;
		}
		readonly AbstractNumberingList list;
		int levelIndex;
		ListLevel level;
		public NumberingLevelDestination(WordProcessingMLBaseImporter importer, AbstractNumberingList list)
			: base(importer) {
			Guard.ArgumentNotNull(list, "list");
			this.list = list;
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		protected internal override IListLevel Level { get { return level; } }
		protected internal override int LevelIndex { get { return levelIndex; } }
		public override void ProcessElementOpen(XmlReader reader) {
			this.levelIndex = Importer.GetWpSTIntegerValue(reader, "ilvl", 0);
			this.level = list.CreateLevel(levelIndex);
			this.level.ListLevelProperties.TemplateCode = Importer.GetWpSTIntegerValue(reader, "tplc", NumberStyles.HexNumber,0);
		}
		public override void ProcessElementClose(XmlReader reader) {
			if (levelIndex < 0 || levelIndex >= 9)
				return;
			list.Levels[levelIndex] = level;
		}
		protected static Destination OnParagraphStyleReference(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new ListLevelParagraphStyleReferenceDestination(importer, (ListLevel)GetThis(importer).Level);		}
	}
	#endregion
	#region ListLevelOverrideLevelDestination
	public class ListLevelOverrideLevelDestination : NumberingLevelBaseDestination {
		readonly OpenXmlListLevelOverride levelOverride;
		readonly IListLevel level;
		public ListLevelOverrideLevelDestination(WordProcessingMLBaseImporter importer, OpenXmlListLevelOverride levelOverride)
			: base(importer) {
			Guard.ArgumentNotNull(levelOverride, "levelOverride");
			this.levelOverride = levelOverride;
			this.level = levelOverride.GetOverrideListLevel();
		}
		protected internal override IListLevel Level { get { return level; } }
		protected internal override int LevelIndex { get { return levelOverride.LevelIndex; } }
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			this.level.ListLevelProperties.TemplateCode = Importer.GetWpSTIntegerValue(reader, "tplc", NumberStyles.HexNumber, 0);
		}
	}
	#endregion
	#region ListLevelOverrideStartDestination
	public class ListLevelOverrideStartDestination : LeafElementDestination {
		OpenXmlListLevelOverride levelOverride;
		public ListLevelOverrideStartDestination(WordProcessingMLBaseImporter importer, OpenXmlListLevelOverride levelOverride)
			: base(importer) {
			Guard.ArgumentNotNull(levelOverride, "levelOverride");
			this.levelOverride = levelOverride;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			levelOverride.NewStart = Importer.GetWpSTIntegerValue(reader, "val", 0);
		}
	}
	#endregion
	#region ListLevelElementDestination (abstract class)
	public abstract class ListLevelElementDestination : LeafElementDestination {
		readonly IListLevel level;
		protected ListLevelElementDestination(WordProcessingMLBaseImporter importer, IListLevel level)
			: base(importer) {
			Guard.ArgumentNotNull(level, "level");
			this.level = level;
		}
		public IListLevel Level { get { return level; } }
	}
	#endregion
	#region ListLevelTextAlignmentDestination
	public class ListLevelTextAlignmentDestination : ListLevelElementDestination {
		public ListLevelTextAlignmentDestination(WordProcessingMLBaseImporter importer, IListLevel level)
			: base(importer, level) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Level.ListLevelProperties.Alignment = Importer.GetWpEnumValue(reader, "val", OpenXmlExporter.listNumberAlignmentTable, ListNumberAlignment.Left);
		}
	}
	#endregion
	#region ListLevelStartDestination
	public class ListLevelStartDestination : ListLevelElementDestination {
		public ListLevelStartDestination(WordProcessingMLBaseImporter importer, IListLevel level)
			: base(importer, level) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Level.ListLevelProperties.Start = Math.Max(0, Importer.GetWpSTIntegerValue(reader, "val"));
		}
	}
	#endregion
	#region ListLevelLegacyDestination
	public class ListLevelLegacyDestination : ListLevelElementDestination {
		public ListLevelLegacyDestination(WordProcessingMLBaseImporter importer, IListLevel level)
			: base(importer, level) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Level.ListLevelProperties.Legacy = true;
			Level.ListLevelProperties.LegacySpace = UnitConverter.TwipsToModelUnits(Importer.GetWpSTIntegerValue(reader, "legacySpace", 0));
			Level.ListLevelProperties.LegacyIndent = UnitConverter.TwipsToModelUnits(Importer.GetWpSTIntegerValue(reader, "legacyIndent", 0));
		}
	}
	#endregion
	#region ListLevelFormatStringDestination
	public class ListLevelFormatStringDestination : ListLevelElementDestination {
		public ListLevelFormatStringDestination(WordProcessingMLBaseImporter importer, IListLevel level)
			: base(importer, level) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string formatString = reader.GetAttribute("val", Importer.WordProcessingNamespaceConst);
			if (formatString == null)
				formatString = String.Empty;
			if (!DocumentFormatsHelper.ShouldInsertBulletedNumbering(DocumentModel))
				formatString = ReplaceStringFormatInBulletNumbering(formatString);
			if (DocumentFormatsHelper.NeedReplaceSimpleToBulletNumbering(DocumentModel)) {
				formatString = ReplaceStringFormatInSimpleNumbering();
				Level.CharacterProperties.FontName = "Symbol";
			}
			Level.ListLevelProperties.DisplayFormatString = ConvertFormatString(formatString);   
		}
		string ReplaceStringFormatInBulletNumbering(string value) {
			string result = ConvertFormatString(value);
			if (result == value)
				return @"%1.";
			return value;
		}
		string ReplaceStringFormatInSimpleNumbering() {
			return Characters.MiddleDot.ToString();
		}
		string pattern = "%(?<number>\\d)";
		string ConvertFormatString(string value) {
			Regex regex = new Regex(pattern);
			string result = value.Replace("{", "{{").Replace("}", "}}");
			return regex.Replace(result, ConvertMatch);
		}
		protected internal virtual string ConvertMatch(Match match) {
			string value = match.Groups["number"].Value;
			int number;
			if (Int32.TryParse(value, out number) && number > 0)
				value = (number - 1).ToString();
			return "{" + value + "}";
		}
	}
	#endregion
	#region ListLevelRestartDestination
	public class ListLevelRestartDestination : ListLevelElementDestination {
		readonly int levelIndex;
		public ListLevelRestartDestination(WordProcessingMLBaseImporter importer, IListLevel level, int levelIndex)
			: base(importer, level) {
			this.levelIndex = levelIndex;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			int index = Math.Min(9, Math.Max(1, Importer.GetWpSTIntegerValue(reader, "val"))) - 1;
			Level.ListLevelProperties.RelativeRestartLevel = levelIndex - index;
		}
	}
	#endregion
	#region ListLevelSuffixDestination
	public class ListLevelSuffixDestination : ListLevelElementDestination {
		public ListLevelSuffixDestination(WordProcessingMLBaseImporter importer, IListLevel level)
			: base(importer, level) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Level.ListLevelProperties.Separator = Importer.GetWpEnumValue(reader, "val", OpenXmlExporter.listNumberSeparatorTable, Characters.TabMark);
		}
	}
	#endregion
	#region ListLevelNumberingFormatDestination
	public class ListLevelNumberingFormatDestination : ListLevelElementDestination {
		public ListLevelNumberingFormatDestination(WordProcessingMLBaseImporter importer, IListLevel level)
			: base(importer, level) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			NumberingFormat format = Importer.GetWpEnumValue(reader, "val", OpenXmlExporter.pageNumberingFormatTable, NumberingFormat.Decimal);
			if (format == NumberingFormat.Bullet && DocumentFormatsHelper.NeedReplaceBulletedLevelsToDecimal(DocumentModel))
				format = (DocumentFormatsHelper.ShouldInsertSimpleNumbering(DocumentModel)) ? NumberingFormat.Decimal : NumberingFormat.None;
			else if (format == NumberingFormat.Decimal && DocumentFormatsHelper.NeedReplaceSimpleToBulletNumbering(DocumentModel))
				format = (DocumentFormatsHelper.ShouldInsertBulletedNumbering(DocumentModel)) ? NumberingFormat.Bullet : NumberingFormat.None;
			Level.ListLevelProperties.Format = format;
		}
	}
	#endregion
	#region ListLevelLegalNumberingDestination
	public class ListLevelLegalNumberingDestination : ListLevelElementDestination {
		public ListLevelLegalNumberingDestination(WordProcessingMLBaseImporter importer, IListLevel level)
			: base(importer, level) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Level.ListLevelProperties.ConvertPreviousLevelNumberingToDecimal = Importer.GetWpSTOnOffValue(reader, "val");
		}
	}
	#endregion
	#region ListLevelParagraphStyleReferenceDestination
	public class ListLevelParagraphStyleReferenceDestination : ListLevelElementDestination {
		public ListLevelParagraphStyleReferenceDestination(WordProcessingMLBaseImporter importer, ListLevel level)
			: base(importer, level) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			int styleIndex = Importer.LookupParagraphStyleIndex(reader.GetAttribute("val", Importer.WordProcessingNamespaceConst));
			if (styleIndex >= 0)
				((ListLevel)Level).ParagraphStyleIndex = styleIndex;
		}
	}
	#endregion
	#region ListLevelRunPropertiesDestination
	public class ListLevelRunPropertiesDestination : RunPropertiesBaseDestination {
		public ListLevelRunPropertiesDestination(WordProcessingMLBaseImporter importer, IListLevel level)
			: base(importer, level) {
		}
	}
	#endregion
	#region ListLevelParagraphPropertiesDestination
	public class ListLevelParagraphPropertiesDestination : ParagraphPropertiesBaseDestination {
		public ListLevelParagraphPropertiesDestination(WordProcessingMLBaseImporter importer, IListLevel level, TabFormattingInfo tabs)
			: base(importer, level, tabs) {
		}
		public override int NumberingId { get { return -1; } set { } }
		public override int ListLevelIndex { get { return -1; } set { } }
	}
	#endregion
	#region OpenXmlAbstractNumberingInfo
	public class OpenXmlAbstractNumberingInfo {
		string abstractNumberingListId;
		int abstractNumberingIndex;
		NumberingType type;
		public string AbstractNumberingListId { get { return abstractNumberingListId; } set { abstractNumberingListId = value; } }
		public int AbstractNumberingIndex { get { return abstractNumberingIndex; } set { abstractNumberingIndex = value; } }
		public NumberingType ListType { get { return type; } set { type = value; } }
	}
	#endregion
	#region OpenXmlAbstractNumberingInfoCollection
	public class OpenXmlAbstractNumberingInfoCollection : List<OpenXmlAbstractNumberingInfo> {
		public OpenXmlAbstractNumberingInfo FindById(string id) {
			int count = Count;
			for (int i = 0; i < count; i++)
				if (this[i].AbstractNumberingListId == id)
					return this[i];
			return null;
		}
	}
	#endregion
	#region OpenXmlNumberingListInfo
	public class OpenXmlNumberingListInfo {
		int id;
		string abstractNumberingListId;
		OpenXmlListLevelOverrideCollection levelOverrides;
		NumberingListIndex listIndex;		
		public OpenXmlNumberingListInfo() {
			this.levelOverrides = new OpenXmlListLevelOverrideCollection();
		}
		public int Id { get { return id; } set { id = value; } }
		public string AbstractNumberingListId { get { return abstractNumberingListId; } set { abstractNumberingListId = value; } }
		public NumberingListIndex ListIndex { get { return listIndex; } set { listIndex = value; } }
		public OpenXmlListLevelOverrideCollection LevelOverrides { get { return levelOverrides; } }		
	}
	#endregion
	#region OpenXmlNumberingListInfoCollection
	public class OpenXmlNumberingListInfoCollection : List<OpenXmlNumberingListInfo> {
		public OpenXmlNumberingListInfo FindById(int id) {
			int count = Count;
			for (int i = 0; i < count; i++)
				if (this[i].Id == id)
					return this[i];
			return null;
		}
	}
	#endregion
	#region OpenXmlListLevelOverride
	public class OpenXmlListLevelOverride {
		int levelIndex;
		DocumentModel documentModel;		
		OverrideListLevel level;
		bool overrideStart;
		int newStart;
		public OpenXmlListLevelOverride(DocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.documentModel = documentModel;
		}
		public int LevelIndex { get { return levelIndex; } set { levelIndex = value; } }
		public int NewStart { get { return newStart; }
			set {
				newStart = value;
				overrideStart = true;
				if (level != null)
					ApplyStartOverride(level);
			}
		}
		public OverrideListLevel GetOverrideListLevel() {
			if (level == null) {
				level = new OverrideListLevel(documentModel);
				ApplyStartOverride(level);
			}
			return level;
		}
		protected virtual void ApplyStartOverride(OverrideListLevel level) {
			Guard.ArgumentNotNull(level, "level");
			if (overrideStart) {
				level.SetOverrideStart(true);
				level.ListLevelProperties.Start = newStart;
			}
		}
		protected internal virtual IOverrideListLevel GetOverrideListLevelCore(NumberingListReferenceLevel originalLevel) {
			if (level != null)
				return level;
			else if (overrideStart) {
				originalLevel.SetOverrideStart(true);
				originalLevel.NewStart = newStart;
			}
			return originalLevel;
		}
	}
	#endregion
	#region OpenXmlListLevelOverrideCollection
	public class OpenXmlListLevelOverrideCollection : List<OpenXmlListLevelOverride> {
	}
	#endregion
}
