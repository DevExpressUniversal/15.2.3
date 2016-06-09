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

using DevExpress.Compatibility.System.Drawing;
using DevExpress.Data.Utils;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Export.OpenXml;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml;
using System.Globalization;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Office.Model;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Import.OpenXml {
	#region WordProcessingMLBaseImporter (abstract class)
	public abstract class WordProcessingMLBaseImporter : RichEditDestinationAndXmlBasedImporter {
		#region Fields
		readonly OpenXmlStyleInfoCollection paragraphStyleInfos;
		readonly OpenXmlStyleInfoCollection characterStyleInfos;
		readonly Stack<OpenXmlStyleInfoCollection> tableStyleInfosStack;
		readonly Stack<OpenXmlStyleInfoCollection> tableCellStyleInfosStack;
		readonly OpenXmlAbstractNumberingInfoCollection abstractListInfos;
		readonly OpenXmlNumberingListInfoCollection listInfos;
		readonly OpenXmlStyleInfoCollection numberingStyleInfos;
		LockAspectRatioTable lockAspectRatioTable = new LockAspectRatioTable();
		int tableDepth;
		int paraId = Int32.MinValue;
		public const string W15Prefix = "w15";
		public const string W15Namespace = "http://schemas.microsoft.com/office/word/2012/wordml";
		public const string W14Prefix = "w14";
		public const string W14Namespace = "http://schemas.microsoft.com/office/word/2010/wordml";
		#endregion
		protected WordProcessingMLBaseImporter(DocumentModel documentModel, XmlBasedDocumentImporterOptions options)
			: base(documentModel, options) {
			this.paragraphStyleInfos = new OpenXmlStyleInfoCollection();
			this.characterStyleInfos = new OpenXmlStyleInfoCollection();
			this.abstractListInfos = new OpenXmlAbstractNumberingInfoCollection();
			this.listInfos = new OpenXmlNumberingListInfoCollection();
			tableStyleInfosStack = new Stack<OpenXmlStyleInfoCollection>();
			tableStyleInfosStack.Push(new OpenXmlStyleInfoCollection());
			tableCellStyleInfosStack = new Stack<OpenXmlStyleInfoCollection>();
			tableCellStyleInfosStack.Push(new OpenXmlStyleInfoCollection());
			this.numberingStyleInfos = new OpenXmlStyleInfoCollection();
		}
		#region Properties
		public OpenXmlStyleInfoCollection ParagraphStyleInfos { get { return paragraphStyleInfos; } }
		public OpenXmlStyleInfoCollection CharacterStyleInfos { get { return characterStyleInfos; } }
		public Stack<OpenXmlStyleInfoCollection> TableStyleInfosStack { get { return tableStyleInfosStack; } }
		public OpenXmlStyleInfoCollection TableStyleInfos { get { return tableStyleInfosStack.Peek(); } }
		public Stack<OpenXmlStyleInfoCollection> TableCellStyleInfosStack { get { return tableCellStyleInfosStack; } }
		public OpenXmlStyleInfoCollection TableCellStyleInfos { get { return tableCellStyleInfosStack.Peek(); } }
		public OpenXmlStyleInfoCollection NumberingStyleInfos { get { return numberingStyleInfos; } }
		public OpenXmlAbstractNumberingInfoCollection AbstractListInfos { get { return abstractListInfos; } }
		public OpenXmlNumberingListInfoCollection ListInfos { get { return listInfos; } }
		public LockAspectRatioTable LockAspectRatioTable { get { return lockAspectRatioTable; } }
		public int ParaId { get { return paraId; } set { paraId = value; } }
		public abstract string WordProcessingNamespaceConst { get; }
		public abstract string W14NamespaceConst { get; }
		public abstract string W15NamespaceConst { get; }
		public abstract string OfficeNamespace { get; }
		public abstract string DrawingMLNamespaceConst { get; }
		public bool InsideTable { get { return tableDepth > 0; } }		
		#endregion
		#region Conversion and Parsing utilities
		public override string ReadAttribute(XmlReader reader, string attributeName) {
			string result = ReadAttribute(reader, attributeName, WordProcessingNamespaceConst);
			if (String.IsNullOrEmpty(result))
				result = reader.GetAttribute(attributeName);
			return result;
		}
		public override string ReadAttribute(XmlReader reader, string attributeName, string ns) {
			return reader.GetAttribute(attributeName, ns);
		}
		protected internal Color GetWpSTColorValue(XmlReader reader, string attributeName) {
			return GetWpSTColorValue(reader, attributeName, DXColor.Empty);
		}
		protected internal Color GetWpSTColorValue(XmlReader reader, string attributeName, Color defaultValue) {
			string value = reader.GetAttribute(attributeName, WordProcessingNamespaceConst);
			if (!String.IsNullOrEmpty(value))
				return ParseColor(value, defaultValue);
			else
				return defaultValue;
		}
		protected internal Color GetColorValue(XmlReader reader, string attributeName) {
			return GetColorValue(reader, attributeName, DXColor.Empty);
		}
		protected internal Color GetPresetColorValue(XmlReader reader, string attributeName) {
			return GetPresetColorValue(reader, attributeName, DXColor.Empty);
		}
		protected internal Color GetMSWordColorValue(XmlReader reader, string attributeName) {
			string value = reader.GetAttribute(attributeName);
			if (String.IsNullOrEmpty(value))
				return DXColor.Empty;
			value = value.Replace("#", "");
			int indexStart = value.IndexOf('[');
			int indexEnd = value.IndexOf(']');
			if (indexStart >= 0 && indexEnd >= 0 && indexStart < indexEnd)
				value = value.Remove(indexStart, indexEnd - indexStart + 1).Trim();
			WordProcessingMLValue mLValue = new WordProcessingMLValue(value);
			foreach (Color key in WordProcessingMLBaseExporter.predefinedBackgroundColors.Keys) {
				if (mLValue.OpenXmlValue == WordProcessingMLBaseExporter.predefinedBackgroundColors[key].OpenXmlValue)
					return key;
			}
			if (value.Length >= 6)
				value = value.Substring(0, 6);
			else
				if (value.Length == 3)
					value = String.Format("{0}{0}{1}{1}{2}{2}", value[0], value[1], value[2]);
				else
					value = new String('0', 6 - value.Length) + value;				
			return ParseColor(value, DXColor.Empty);
		}
		protected internal Color GetColorValue(XmlReader reader, string attributeName, Color defaultValue) {
			string value = reader.GetAttribute(attributeName);
			if (!String.IsNullOrEmpty(value))
				return ParseColor(value, defaultValue);
			else
				return defaultValue;
		}
		protected internal Color GetPresetColorValue(XmlReader reader, string attributeName, Color defaultValue) {
			string value = reader.GetAttribute(attributeName);
			if (!String.IsNullOrEmpty(value))
				return ParsePresetColor(value, defaultValue);
			else
				return defaultValue;
		}
		protected internal T GetWpEnumValue<T>(XmlReader reader, string attributeName, Dictionary<T, WordProcessingMLValue> table, T defaultValue) where T : struct {
			string value = reader.GetAttribute(attributeName, WordProcessingNamespaceConst);
			if (String.IsNullOrEmpty(value))
				return defaultValue;
			return GetWpEnumValueCore(value, table, defaultValue);
		}
		protected internal T GetEnumValue<T>(XmlReader reader, string attributeName, Dictionary<T, WordProcessingMLValue> table, T defaultValue) where T : struct {
			string value = reader.GetAttribute(attributeName, null);
			if (String.IsNullOrEmpty(value))
				return defaultValue;
			return GetWpEnumValueCore(value, table, defaultValue);
		}
		protected internal T GetWpEnumValueCore<T>(string value, Dictionary<T, WordProcessingMLValue> table, T defaultValue) where T : struct {
			foreach (T key in table.Keys) {
				WordProcessingMLValue valueString = table[key];
				if (value == valueString.OpenXmlValue || value == valueString.WordMLValue)
					return key;
			}
			return defaultValue;
		}
		protected internal virtual Color ParseColor(string value, Color defaultValue) {
			if (value == "auto")
				return defaultValue;
			Color result = MarkupLanguageColorParser.ParseColor(value);
			return result == DXColor.Empty ? defaultValue : result;
		}
		protected internal virtual Color ParsePresetColor(string value, Color defaultValue) {			
			Color result;
			if (WordProcessingMLBaseExporter.presetColors.TryGetValue(value, out result))
				return result; 
			else
				return defaultValue;
		}
		protected internal virtual int ConvertToInt(string value) {
			return GetIntegerValue(value, NumberStyles.HexNumber, Int32.MinValue);
		}
		#endregion
		protected internal ThemeColorIndex GetThemeColorIndex(XmlReader reader, string attributeName) {
			string name = ReadAttribute(reader, attributeName);
			if (String.IsNullOrEmpty(name))
				return ThemeColorIndex.None;
			return GetWpEnumValue<ThemeColorIndex>(reader, name, OpenXmlExporter.themeColorIndexTable, ThemeColorIndex.None);
		}
	   protected internal LangInfo ReadLanguage(XmlReader reader) {
		   string value = reader.GetAttribute("val", WordProcessingNamespaceConst);
		   CultureInfo latin = null;
		   int myInt;
		   try {
			   if (!String.IsNullOrEmpty(value)) {
				   if (int.TryParse(value, out myInt))
					   latin = CultureInfoHelper.CreateCultureInfo(myInt);
				   else
					   latin = new CultureInfo(value);
			   }
		   }
		   catch { }
		   value = reader.GetAttribute("bidi", WordProcessingNamespaceConst);
		   CultureInfo bidi = null;
		   try {
			   if (!String.IsNullOrEmpty(value)) {
				   if (int.TryParse(value, out myInt))
					   bidi = CultureInfoHelper.CreateCultureInfo(myInt);
				   else
					   bidi = new CultureInfo(value);
			   }
		   }
		   catch { }
		   value = reader.GetAttribute("eastAsia", WordProcessingNamespaceConst);
		   CultureInfo eastAsia = null;
		   try {
			   if (!String.IsNullOrEmpty(value)) {
				   if (int.TryParse(value, out myInt))
					   eastAsia = CultureInfoHelper.CreateCultureInfo(myInt);
				   else
					   eastAsia = new CultureInfo(value);
			   }
		   }
		   catch { }
		   return new LangInfo(latin, bidi, eastAsia);
	   }
		public override bool ReadToRootElement(XmlReader reader, string name) {
			return ReadToRootElement(reader, name, WordProcessingNamespaceConst);
		}
		protected internal virtual bool ReadToRootW15Element(XmlReader reader, string name) {
			return ReadToRootElement(reader, name, W15NamespaceConst);
		}
		protected internal virtual bool ReadToRootAElement(XmlReader reader, string name) {
			return ReadToRootElement(reader, name, DrawingMLNamespaceConst);
		} 
		#region Styles
		protected internal virtual int LookupParagraphStyleIndex(string styleId) {
			return LookupStyleIndexCore(styleId, ParagraphStyleInfos);
		}
		protected internal virtual int LookupCharacterStyleIndex(string styleId) {
			return LookupStyleIndexCore(styleId, CharacterStyleInfos);
		}
		protected internal virtual int LookupTableStyleIndex(string styleId) {
			return LookupStyleIndexCore(styleId, TableStyleInfos);
		}
		protected internal virtual int LookupTableCellStyleIndex(string styleId) {
			return LookupStyleIndexCore(styleId, TableCellStyleInfos);
		}
		protected internal virtual int LookupStyleIndexCore(string styleId, OpenXmlStyleInfoCollection styleInfos) {
			OpenXmlStyleInfo styleInfo = styleInfos.LookupStyleById(styleId);
			if (styleInfo != null)
				return styleInfo.StyleIndex;
			else
				return -1;
		}
		protected internal virtual void CreateStylesHierarchy() {
			CreateStylesHierarchyCore(DocumentModel.CharacterStyles, CharacterStyleInfos);
			CreateStylesHierarchyCore(DocumentModel.ParagraphStyles, ParagraphStyleInfos);
			CreateStylesHierarchyCore(DocumentModel.TableStyles, TableStyleInfos);
			CreateStylesHierarchyCore(DocumentModel.TableCellStyles, TableCellStyleInfos);
		}
		protected internal virtual void CreateStylesHierarchyCore<T>(StyleCollectionBase<T> styles, OpenXmlStyleInfoCollection styleInfos) where T : StyleBase<T> {
			int count = styleInfos.Count;
			for (int i = 0; i < count; i++) {
				OpenXmlStyleInfo styleInfo = styleInfos[i];
				OpenXmlStyleInfo parentStyleInfo = styleInfos.LookupStyleById(styleInfo.ParentStyleId);
				if (parentStyleInfo != null)
					styles[styleInfo.StyleIndex].Parent = styles[parentStyleInfo.StyleIndex];
			}
		}
		protected internal virtual void LinkStyles() {
			CharacterStyleCollection characterStyles = DocumentModel.CharacterStyles;
			ParagraphStyleCollection paragraphStyles = DocumentModel.ParagraphStyles;
			int count = CharacterStyleInfos.Count;
			for (int i = 0; i < count; i++) {
				OpenXmlStyleInfo styleInfo = CharacterStyleInfos[i];
				OpenXmlStyleInfo linkedStyleInfo = ParagraphStyleInfos.LookupStyleById(styleInfo.LinkedStyleId);
				if (linkedStyleInfo != null)
					DocumentModel.StyleLinkManager.CreateLink(paragraphStyles[linkedStyleInfo.StyleIndex], characterStyles[styleInfo.StyleIndex]);
			}
			count = ParagraphStyleInfos.Count;
			for (int i = 0; i < count; i++) {
				OpenXmlStyleInfo styleInfo = ParagraphStyleInfos[i];
				OpenXmlStyleInfo nextStyleInfo = ParagraphStyleInfos.LookupStyleById(styleInfo.NextStyleId);
				if (nextStyleInfo != null)
					paragraphStyles[styleInfo.StyleIndex].NextParagraphStyle = paragraphStyles[nextStyleInfo.StyleIndex];
			}
		}
		#endregion
		#region Numbering
		protected internal virtual void CreateNumberingLists() {
			int count = ListInfos.Count;
			for (int i = 0; i < count; i++) {
				OpenXmlNumberingListInfo listInfo = ListInfos[i];
				OpenXmlAbstractNumberingInfo abstractNumberingInfo = AbstractListInfos.FindById(listInfo.AbstractNumberingListId);
				if (abstractNumberingInfo != null)
					AppendNumberingList(listInfo, abstractNumberingInfo.AbstractNumberingIndex);
			}
		}
		protected internal virtual void AppendNumberingList(OpenXmlNumberingListInfo listInfo, int abstractNumberingListIndex) {
			NumberingList list = new NumberingList(DocumentModel, new AbstractNumberingListIndex(abstractNumberingListIndex));
			int count = listInfo.LevelOverrides.Count;
			for (int i = 0; i < count; i++) {
				OpenXmlListLevelOverride listLevelOverride = listInfo.LevelOverrides[i];
				list.Levels[listLevelOverride.LevelIndex] = listLevelOverride.GetOverrideListLevelCore((NumberingListReferenceLevel)list.Levels[listLevelOverride.LevelIndex]);
			}
			listInfo.ListIndex = new NumberingListIndex(DocumentModel.NumberingLists.Count);
			DocumentModel.AddNumberingListUsingHistory(list);
		}
		#endregion
		protected internal virtual void LinkParagraphStylesWithNumberingLists() {
			paragraphStyleInfos.ForEach(LinkParagraphStyleWithNumberingLists);
		}
		protected internal virtual void LinkNumberingListStyles() {
			foreach (OpenXmlStyleInfo styleInfo in this.numberingStyleInfos) {
				OpenXmlNumberingListInfo listInfo = ListInfos.FindById(styleInfo.NumberingId);
				if (listInfo == null)
					continue;
				NumberingListStyle style = DocumentModel.NumberingListStyles[styleInfo.StyleIndex];
				style.SetNumberingListIndex(listInfo.ListIndex);
				DocumentModel.NumberingLists[listInfo.ListIndex].AbstractNumberingList.SetStyleLinkIndex(styleInfo.StyleIndex);
			}
		}
		protected internal virtual void LinkParagraphStyleWithNumberingLists(OpenXmlStyleInfo styleInfo) {
			if (styleInfo.StyleIndex < 0)
				return;
			ParagraphStyle style = DocumentModel.ParagraphStyles[styleInfo.StyleIndex];
			if (styleInfo.NumberingId >= 0) {
				OpenXmlNumberingListInfo listInfo = ListInfos.FindById(styleInfo.NumberingId);
				if (listInfo == null)
					return;
				style.SetNumberingListIndex(listInfo.ListIndex);
			}
			style.SetNumberingListLevelIndex(styleInfo.ListLevelIndex);
		}
		public void LockAspectRatioTableAddValue(string key, string value) {
			if (!String.IsNullOrEmpty(key))
				if (value == "t" || value == "f")
					LockAspectRatioTable["#" + key] = value;
		}
		public bool LockAspectRatioTableGetValue(string key, out bool useLockAspectRatio) {
			if (LockAspectRatioTable.ContainsKey(key)) {
				useLockAspectRatio = true;
				return GetBoolValue(LockAspectRatioTable[key]);
			}
			else {
				useLockAspectRatio = false;
				return false;
			}
		}
		bool GetBoolValue(string value) {
			return value == "t";
		}
		protected internal abstract string GetWordProcessingMLValue(WordProcessingMLValue value);
		protected internal abstract SectionTextDirectionDestination CreateOpenXmlSectionTextDirectionDestination();
		protected internal abstract SectionTextDirectionDestination CreateWordMLSectionTextDirectionDestination();
		protected internal abstract ParagraphPropertiesBaseDestination CreateStyleParagraphPropertiesDestination(StyleDestinationBase styleDestination, ParagraphFormattingBase paragraphFormatting, TabFormattingInfo tabs);
		protected internal abstract int RegisterFootNote(FootNote note, string id);
		protected internal abstract int RegisterEndNote(EndNote note, string id);
		protected internal virtual Destination CreateBookmarkStartElementDestination(XmlReader reader) {
			return new BookmarkStartElementDestination(this);
		}
		protected internal virtual Destination CreateBookmarkEndElementDestination(XmlReader reader) {
			return new BookmarkEndElementDestination(this);
		}
		protected internal virtual Destination CreateCustomRunDestination(XmlReader reader) {
			return new CustomRunDestination(this);
		}
		protected internal virtual Destination CreateDataContainerRunDestination(XmlReader reader) {
			return new DataContainerRunDestination(this);
		}
		protected internal virtual Destination CreateVersionDestination(XmlReader reader) {
			return new DocumentVersionDestination(this);
		}
		protected internal virtual RunDestination CreateRunDestination() {
			return new RunDestination(this);
		}
		protected internal virtual ParagraphDestination CreateParagraphDestination() {
			return new ParagraphDestination(this);
		}
		public void BeginTable() {
			tableDepth++;
		}
		public void EndTable() {
			tableDepth--;
		}
	}
	#endregion
}
