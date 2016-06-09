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
using System.Text;
using System.Xml;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Export.OpenDocument;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Office.NumberConverters;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Import.OpenDocument {
	#region OpenDocumentAbstractNumberingInfo
	public class OpenDocumentListInfo {
		string listStyleName;
		int abstractNumberingIndex;
		public OpenDocumentListInfo(string listStyleName, int abstractNumberingIndex) {
			this.listStyleName = listStyleName;
			this.abstractNumberingIndex = abstractNumberingIndex;
		}
		public string ListStyleName { get { return listStyleName; } }
		public int AbstractNumberingIndex { get { return abstractNumberingIndex; } set { abstractNumberingIndex = value; } }
	}
	#endregion
	#region OpenDocumentAbstractNumberingInfoCollection
	public class OpenDocumentListInfoCollection : List<OpenDocumentListInfo> {
		public OpenDocumentListInfo FindByStyleName(string styleName) {
			int count = Count;
			for (int i = 0; i < count; i++)
				if (this[i].ListStyleName == styleName)
					return this[i];
			return null;
		}
	}
	#endregion
	public class NumberingListReference {
		int listIndex;
		int levelIndex;
		List<AbstractNumberingListIndex> usedIndexes;
		List<NumberingListIndex> openedLists;
		bool suppressInsertPageBreak = true;
		public NumberingListReference(int listIndex, int levelIndex) {
			this.listIndex = listIndex;
			this.levelIndex = levelIndex;
			this.usedIndexes = new List<AbstractNumberingListIndex>();
			this.openedLists = new List<NumberingListIndex>();
		}
		public int ListIndex { get { return listIndex; } }
		public int LevelIndex { get { return levelIndex; } }
		public bool IsListActive { get { return listIndex >= 0 && levelIndex >= 0; } }
		public bool CanAddNumberingListToParagraph { get; set; }
		protected List<AbstractNumberingListIndex> UsedAbstractListIndexes { get { return usedIndexes; } }
		protected List<NumberingListIndex> OpenedLists { get { return openedLists; } }
		public bool SuppressInsertPageBreak { get { return suppressInsertPageBreak; } set { suppressInsertPageBreak = value; } }
		public void OpenList(int listIndex) {
			this.listIndex = listIndex;
			this.levelIndex = 0;
			this.suppressInsertPageBreak = true;
			CanAddNumberingListToParagraph = true;
			NumberingListIndex index = new NumberingListIndex(listIndex);
			if (!OpenedLists.Contains(index))
				OpenedLists.Add(index);
		}
		public void CloseList() {
			NumberingListIndex index = new NumberingListIndex(listIndex);
			if (OpenedLists.Contains(index))
				OpenedLists.Remove(index);
			listIndex = -1;
			levelIndex = -1;
			this.suppressInsertPageBreak = false;
			CanAddNumberingListToParagraph = true;
		}
		public void IncreaseLevel() {
			levelIndex++;
			CanAddNumberingListToParagraph = true;
		}
		public void DecreaseLevel() {
			levelIndex--;
			CanAddNumberingListToParagraph = true;
		}
		public bool IsListOpened(int listIndex) {
			return OpenedLists.Contains(new NumberingListIndex(listIndex));
		}
		public bool IsAbstractListUsed(AbstractNumberingListIndex listIndex) {
			return UsedAbstractListIndexes.Contains(listIndex);
		}
		internal void UseAbstractList(AbstractNumberingListIndex index) {
			UsedAbstractListIndexes.Add(index);
		}
	}
	public class ListStyleDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("list-level-style-number", OnListLevelStyleNumber);
			result.Add("list-level-style-bullet", OnListLevelStyleBullet);
			return result;
		}
		string styleName;
		AbstractNumberingList list;
		public ListStyleDestination(OpenDocumentTextImporter importer)
			: base(importer) {
			this.list = new AbstractNumberingList(importer.DocumentModel);
		}
		protected internal AbstractNumberingList List { get { return list; } }
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static ListStyleDestination GetThis(OpenDocumentTextImporter importer) {
			return (ListStyleDestination)importer.PeekDestination();
		}
		public override void ProcessElementOpen(XmlReader reader) {
			this.styleName = ImportHelper.GetStyleStringAttribute(reader, "name");
			Debug.Assert(!string.IsNullOrEmpty(styleName));
			int listIndex = -1;
			OpenDocumentListInfo listInfo = Importer.ListInfos.FindByStyleName(styleName);
			if (listInfo != null)
				listIndex = listInfo.AbstractNumberingIndex;
			else {
				listIndex = CreateAbstractNumberingList();
				listInfo = CreateListInfo(styleName, listIndex);
				Importer.ListInfos.Add(listInfo);
			}
			Debug.Assert(listIndex >= 0);
			this.list = DocumentModel.AbstractNumberingLists[new AbstractNumberingListIndex(listIndex)];
		}
		public override void ProcessElementClose(XmlReader reader) {
		}
		protected internal int CreateAbstractNumberingList() {
			AbstractNumberingList abstractNumberingList = new AbstractNumberingList(DocumentModel);
			AbstractNumberingListCollection lists = DocumentModel.AbstractNumberingLists;
			lists.Add(abstractNumberingList);
			return lists.Count - 1;
		}
		protected OpenDocumentListInfo CreateListInfo(string styleName, int listIndex) {
			return new OpenDocumentListInfo(styleName, listIndex);
		}
		protected static Destination OnListLevelStyleNumber(OpenDocumentTextImporter importer, XmlReader reader) {
			return new ListLevelStyleNumberDestination(importer, GetThis(importer).list);
		}
		protected static Destination OnListLevelStyleBullet(OpenDocumentTextImporter importer, XmlReader reader) {
			return new ListLevelStyleBulletDestination(importer, GetThis(importer).list);
		}
	}
	public abstract class ListLevelStyleBaseDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("list-level-properties", OnListLevelProperties);
			result.Add("text-properties", OnListLevelTextProperties);
			return result;
		}
		readonly AbstractNumberingList list;
		ListLevel level;
		int levelIndex = -1;
		string styleName;
		string numPrefix;
		string numSuffix;
		protected ListLevelStyleBaseDestination(OpenDocumentTextImporter importer, AbstractNumberingList list)
			: base(importer) {
			Guard.ArgumentNotNull(list, "list");
			this.list = list;
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public ListLevel Level { get { return level; } }
		protected string NumPrefix { get { return numPrefix; } set { numPrefix = value; } }
		protected string NumSuffix { get { return numSuffix; } set { numSuffix = value; } }
		public int LevelIndex { get { return levelIndex; } }
		static ListLevelStyleBaseDestination GetThis(OpenDocumentTextImporter importer) {
			return (ListLevelStyleBaseDestination)importer.PeekDestination();
		}
		public override void ProcessElementOpen(XmlReader reader) {
			int levelNo = Math.Min(9, ImportHelper.GetTextIntegerAttribute(reader, "level", 1));
			this.levelIndex = levelNo - 1;
			this.level = GetListLevel(LevelIndex);
			this.numPrefix = ImportHelper.GetStyleStringAttribute(reader, "num-prefix");
			this.numSuffix = ImportHelper.GetStyleStringAttribute(reader, "num-suffix");
			ApplyListLevelDisplayFormat(reader);
			this.styleName = ImportHelper.GetTextStringAttribute(reader, "style-name");
			if (!string.IsNullOrEmpty(styleName))
				ApplyListLevelAutoStyle(styleName);
		}
		protected internal virtual void ApplyListLevelDisplayFormat(XmlReader reader) {
			string format = AddSuffixPrefixToDisplayFormat(CreateListLevelDisplayFormat(LevelIndex));
			Level.ListLevelProperties.DisplayFormatString = format;
		}
		protected string CreateListLevelDisplayFormat(int levelIndex) {
			return String.Format("{{{0}}}", levelIndex);
		}
		protected string AddSuffixPrefixToDisplayFormat(string levelFormat) {
			return String.Format("{0}{1}{2}", NumPrefix, levelFormat, NumSuffix);
		}
		protected internal virtual void ApplyListLevelAutoStyle(string styleName) {
			CharacterStyle style = Importer.DocumentModel.CharacterStyles.GetStyleByName(styleName);
			if (style != null)
				Level.CharacterProperties.CopyFrom(style.CharacterProperties);
		}
		protected internal ListLevel GetListLevel(int levelIndex) {
			ListLevel result = list.Levels[levelIndex];
			return result != null ? result : list.CreateLevel(levelIndex);
		}
		public override void ProcessElementClose(XmlReader reader) {
			list.Levels[LevelIndex] = Level;
		}
		protected static Destination OnListLevelProperties(OpenDocumentTextImporter importer, XmlReader reader) {
			return new ListLevelPropertiesDestination(importer, GetThis(importer).Level);
		}
		protected static Destination OnListLevelTextProperties(OpenDocumentTextImporter importer, XmlReader reader) {
			return new ListLevelTextPropertiesDestination(importer, GetThis(importer).Level);
		}
	}
	public class ListLevelStyleNumberDestination : ListLevelStyleBaseDestination {
		public ListLevelStyleNumberDestination(OpenDocumentTextImporter importer, AbstractNumberingList list)
			: base(importer, list) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			if (DocumentFormatsHelper.NeedReplaceSimpleToBulletNumbering(DocumentModel)) {
				Level.ListLevelProperties.Format = NumberingFormat.Bullet;
				return;
			}
			int startValue = ImportHelper.GetTextIntegerAttribute(reader, "start-value", 1);
			Level.ListLevelProperties.Start = startValue;
			string numFormat = ImportHelper.GetStyleStringAttribute(reader, "num-format");
			Level.ListLevelProperties.Format = OpenDocumentHelper.NumberingFormatTable.GetEnumValue(numFormat, NumberingFormat.Decimal, true);
		}
		protected internal override void ApplyListLevelDisplayFormat(XmlReader reader) {
			if (DocumentFormatsHelper.NeedReplaceSimpleToBulletNumbering(DocumentModel)) {
				base.NumSuffix = String.Empty;
				base.ApplyListLevelDisplayFormat(reader);
				return;
			}
			int displayLevelCount = -1;
			string displayLevels = ImportHelper.GetTextStringAttribute(reader, "display-levels");
			if (!string.IsNullOrEmpty(displayLevels) && Int32.TryParse(displayLevels, out displayLevelCount)) {
				Level.ListLevelProperties.DisplayFormatString = CalculateLevelsDisplayFormat(displayLevelCount);
				return;
			}
			base.ApplyListLevelDisplayFormat(reader);
		}
		protected internal string CalculateLevelsDisplayFormat(int odtDisplayLevelCount) {
			int modelDisplayLevelCount = Math.Min(9, odtDisplayLevelCount);
			string result = string.Empty;
			for (int i = 0; i < modelDisplayLevelCount; i++) {
				if (!string.IsNullOrEmpty(result))
					result += ".";
				result += CreateListLevelDisplayFormat(i);
			}
			return AddSuffixPrefixToDisplayFormat(result);
		}
	}
	public class ListLevelStyleBulletDestination : ListLevelStyleBaseDestination {
		Char bulletChar = Characters.Bullet;
		public ListLevelStyleBulletDestination(OpenDocumentTextImporter importer, AbstractNumberingList list)
			: base(importer, list) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			DocumentModel documentModel = Importer.DocumentModel;
			if (!DocumentFormatsHelper.ShouldInsertBulletedNumbering(documentModel)) {
				Level.ListLevelProperties.Format = NumberingFormat.Decimal;
				Level.CharacterProperties.FontName = documentModel.DefaultCharacterProperties.FontName;
				return;
			}
			Level.ListLevelProperties.Format = NumberingFormat.Bullet;
			this.bulletChar = Convert.ToChar(ImportHelper.GetTextStringAttribute(reader, "bullet-char"));
			Level.ListLevelProperties.DisplayFormatString = Convert.ToString(bulletChar);
		}
	}
	public abstract class ListLevelBasePropertiesDestination : ElementDestination {
		readonly IListLevel level;
		protected ListLevelBasePropertiesDestination(OpenDocumentTextImporter importer, IListLevel level)
			: base(importer) {
			this.level = level;
		}
		public IListLevel Level { get { return level; } }
		internal static void ApplyListLevelTextAlignment(XmlReader reader, IListLevel level) {
			string alignment = ImportHelper.GetFoStringAttribute(reader, "text-align");
			if (!String.IsNullOrEmpty(alignment))
				level.ListLevelProperties.Alignment = OpenDocumentHelper.ListNumberAlignmentTable.GetEnumValue(alignment, ListNumberAlignment.Left);
		}
	}
	public class ListLevelPropertiesDestination : ListLevelBasePropertiesDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("list-level-label-alignment", OnListLevelLabelAlignment);
			return result;
		}
		public ListLevelPropertiesDestination(OpenDocumentTextImporter importer, IListLevel level)
			: base(importer, level) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			ValueInfo startIndent = ImportHelper.GetTextAttributeInfo(reader, "space-before");
			if (!startIndent.IsEmpty) {
				Level.FirstLineIndentType = ParagraphFirstLineIndent.Indented;
				Level.FirstLineIndent = Math.Abs(GetIntegerDocumentValue(startIndent));
				if (GetIntegerDocumentValue(startIndent) < 0)
					Level.FirstLineIndentType = ParagraphFirstLineIndent.Hanging;
			}
			ApplyListLevelTextAlignment(reader, Level);
		}
		static ListLevelBasePropertiesDestination GetThis(OpenDocumentTextImporter importer) {
			return (ListLevelBasePropertiesDestination)importer.PeekDestination();
		}
		protected static Destination OnListLevelLabelAlignment(OpenDocumentTextImporter importer, XmlReader reader) {
			return new ListLevelLabelAlignmentDestination(importer, GetThis(importer).Level);
		}
	}
	public class ListLevelTextPropertiesDestination : ListLevelBasePropertiesDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			return result;
		}
		public ListLevelTextPropertiesDestination(OpenDocumentTextImporter importer, IListLevel level)
			: base(importer, level) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			ApplyListLevelTextAlignment(reader, Level); 
			string fontName = ImportHelper.GetStyleStringAttribute(reader, "font-name");
			if (!string.IsNullOrEmpty(fontName))
				Level.CharacterProperties.FontName = fontName;
		}
	}
	public class ListLevelLabelAlignmentDestination : LeafElementDestination {
		IListLevel level;
		public ListLevelLabelAlignmentDestination(OpenDocumentTextImporter importer, IListLevel level)
			: base(importer) {
			Guard.ArgumentNotNull(level, "level");
			this.level = level;
		}
		public IListLevel Level { get { return level; } }
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			ApplyTextAlignment(reader);
			ApplyIndention(reader);
			ApplyLineSpacing(reader);
			ApplySeparator(reader);
		}
		protected virtual void ApplyTextAlignment(XmlReader reader) {
			ListLevelBasePropertiesDestination.ApplyListLevelTextAlignment(reader, Level);
		}
		protected virtual void ApplyIndention(XmlReader reader) {
			ValueInfo indent = ImportHelper.GetFoAttributeInfo(reader, "text-indent");
			if (!indent.IsEmpty)
				SetFirstLineIndent(indent);
			ValueInfo marginLeft = ImportHelper.GetFoAttributeInfo(reader, "margin-left");
			if (!marginLeft.IsEmpty)
				Level.LeftIndent = GetIntegerDocumentValue(marginLeft);
			ValueInfo marginRight = ImportHelper.GetFoAttributeInfo(reader, "margin-right");
			if (!marginRight.IsEmpty)
				Level.RightIndent = GetIntegerDocumentValue(marginRight);
			ValueInfo marginTop = ImportHelper.GetFoAttributeInfo(reader, "margin-top");
			if (!marginTop.IsEmpty)
				Level.SpacingBefore = GetIntegerDocumentValue(marginTop);
			ValueInfo marginBottom = ImportHelper.GetFoAttributeInfo(reader, "margin-bottom");
			if (!marginBottom.IsEmpty)
				Level.SpacingAfter = GetIntegerDocumentValue(marginBottom);
		}
		protected virtual void ApplySeparator(XmlReader reader) {
			string labelSeparator = ImportHelper.GetTextStringAttribute(reader, "label-followed-by");
			Level.ListLevelProperties.Separator = OpenDocumentHelper.ListNumberSeparatorTable.GetEnumValue(labelSeparator, Characters.TabMark);
		}
		protected virtual void ApplyLineSpacing(XmlReader reader) {
			ParagraphLineSpacing type = ParagraphLineSpacing.Single;
			float lineSpacing = 0;
			string atLeast = ImportHelper.GetStyleStringAttribute(reader, "line-height-at-least");
			if (!String.IsNullOrEmpty(atLeast))
				ImportHelper.ConvertLineSpacingFromString(atLeast, Importer.UnitsConverter, out type, out lineSpacing);
			else {
				string lineHeight = ImportHelper.GetFoStringAttribute(reader, "line-height");
				if (lineHeight.ToLower(CultureInfo.InvariantCulture) == "normal") {
				}
				if (lineHeight.Contains("%")) {
					ValueInfo percentHeight = StringValueParser.TryParse(lineHeight);
					if (percentHeight.Unit == "%") {
						lineSpacing = percentHeight.Value / 100f;
						type = ParagraphLineSpacing.Multiple;
					}
				}
				else if (!String.IsNullOrEmpty(lineHeight))
					ImportHelper.ConvertLineSpacingFromString(lineHeight, Importer.UnitsConverter, out type, out lineSpacing);
			}
			Level.LineSpacingType = type;
			Level.LineSpacing = lineSpacing;
		}
		private void SetFirstLineIndent(ValueInfo indent) {
			int value = GetIntegerDocumentValue(indent);
			if (value < 0)
				Level.FirstLineIndentType = ParagraphFirstLineIndent.Hanging;
			if (value > 0)
				Level.FirstLineIndentType = ParagraphFirstLineIndent.Indented;
			Level.FirstLineIndent = Math.Abs(value);
		}
	}
	public class ListDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("list-item", OnListItem);
			result.Add("list-header", OnListHeader);
			return result;
		}
		string styleName;
		bool listOpened;
		public ListDestination(OpenDocumentTextImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			this.styleName = ImportHelper.GetTextStringAttribute(reader, "style-name");
			OpenDocumentListInfo listInfo = Importer.ListInfos.FindByStyleName(styleName);
			if (listInfo == null) {
				listOpened = false;
				return;
			}			
			bool continueNumbering = ImportHelper.GetTextBoolAttribute(reader, "continue-numbering", false);
			NumberingListReference listReference = Importer.InputPosition.CurrentListReference;
			AbstractNumberingListIndex index = new AbstractNumberingListIndex(listInfo.AbstractNumberingIndex);
			if (listReference.IsAbstractListUsed(index) && !continueNumbering) {
				listInfo.AbstractNumberingIndex = AddNewAbstractNumberingListClone(index);
			}
			CreateNumberingList(listInfo.AbstractNumberingIndex);
			int listIndex = DocumentModel.NumberingLists.Count - 1;
			Debug.Assert(listIndex >= 0);
			listReference.OpenList(listIndex);
			listOpened = true;
		}
		protected internal int AddNewAbstractNumberingListClone(AbstractNumberingListIndex index) {
			AbstractNumberingListCollection lists = DocumentModel.AbstractNumberingLists;
			AbstractNumberingList clone = lists[index].Clone();
			lists.Add(clone);
			return lists.Count - 1;
		}
		public override void ProcessElementClose(XmlReader reader) {
			if (!listOpened)
				return;
			Importer.InputPosition.CurrentListReference.CloseList();
		}
		protected void CreateNumberingList(int abstractNumberingListIndex) {
			AbstractNumberingListIndex index = new AbstractNumberingListIndex(abstractNumberingListIndex);
			NumberingList newList = new NumberingList(DocumentModel, index);
			DocumentModel.AddNumberingListUsingHistory(newList);
			Importer.InputPosition.CurrentListReference.UseAbstractList(index);
		}
		protected static Destination OnListItem(OpenDocumentTextImporter importer, XmlReader reader) {
			return new ListItemDestination(importer);
		}
		static Destination OnListHeader(OpenDocumentTextImporter importer, XmlReader reader) {
			return new ListHeaderDestination(importer);
		}
	}
	public class ListItemDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("p", OnParagraph);
			result.Add("h", OnHeading);
			result.Add("list", OnListLevel);
			return result;
		}
		public ListItemDestination(OpenDocumentTextImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
		}
		public override void ProcessElementClose(XmlReader reader) {
			base.ProcessElementClose(reader);
			Importer.InputPosition.CurrentListReference.CanAddNumberingListToParagraph = true;
		}
		static Destination OnParagraph(OpenDocumentTextImporter importer, XmlReader reader) {
			importer.InputPosition.RegisterParagraphForTableCellDestination();
			return new ParagraphDestination(importer);
		}
		static Destination OnHeading(OpenDocumentTextImporter importer, XmlReader reader) {
			importer.InputPosition.RegisterParagraphForTableCellDestination();
			return new HeadingDestination(importer);
		}
		protected static Destination OnListLevel(OpenDocumentTextImporter importer, XmlReader reader) {
			return new ListLevelDestination(importer);
		}
	}
	public class ListHeaderDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("p", OnParagraph);
			return result;
		}
		public ListHeaderDestination(OpenDocumentTextImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			Importer.InputPosition.CurrentListReference.CanAddNumberingListToParagraph = false;
		}
		public override void ProcessElementClose(XmlReader reader) {
			base.ProcessElementClose(reader);
			Importer.InputPosition.CurrentListReference.CanAddNumberingListToParagraph = true;
		}
		static Destination OnParagraph(OpenDocumentTextImporter importer, XmlReader reader) {
			importer.InputPosition.RegisterParagraphForTableCellDestination();
			return new ParagraphDestination(importer);
		}
	}
	public class ListLevelDestination : LeafElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("list-item", OnListItem);
			return result;
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public ListLevelDestination(OpenDocumentTextImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			Importer.InputPosition.CurrentListReference.IncreaseLevel();
		}
		public override void ProcessElementClose(XmlReader reader) {
			base.ProcessElementClose(reader);
			Importer.InputPosition.CurrentListReference.DecreaseLevel();
		}
		protected static Destination OnListItem(OpenDocumentTextImporter importer, XmlReader reader) {
			return new ListItemDestination(importer);
		}
	}
}
