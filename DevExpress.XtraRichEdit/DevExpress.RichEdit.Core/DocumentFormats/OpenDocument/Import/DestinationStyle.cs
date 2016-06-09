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
using System.Xml;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Export.OpenDocument;
using DevExpress.XtraRichEdit.Import.Html;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Compatibility.System.Drawing;
using Debug = System.Diagnostics.Debug;
#if !SL
using System.Drawing;
using System.Text.RegularExpressions;
using System.Diagnostics;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Import.OpenDocument {
	#region StyleDestinationBase ( abstract class )
	public abstract class StyleDestinationBase : ElementDestination, ICellPropertiesOwner {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("paragraph-properties", OnParagraphProperties);
			result.Add("text-properties", OnTextProperties);
			result.Add("section-properties", OnSectionProperties);
			result.Add("table-properties", OnTableProperties);
			result.Add("table-cell-properties", OnTableCellProperties);
			result.Add("table-column-properties", OnTableColumnProperties);
			result.Add("table-row-properties", OnTableRowProperties);
			result.Add("graphic-properties", OnFrameProperties);
			return result;
		}
		#region Fields
		TabFormattingInfo tabs;
		ParagraphBreaksInfo paragraphBreaks;
		SectionStyleInfo sectionStyleInfo;
		FloatingObjectImportInfo floatingObjectImportInfo;
		string styleName;
		string family;
		string parentName;
		string nextStyleName;
		string masterPageName;
		CharacterFormattingBase characterFormatting;
		ParagraphFormattingBase paragraphFormatting;
		TablePropertiesInfo tableProperties; 
		TableCellProperties tableCellProperties;
		TableColumnWidthInfo tableColumnWidths;
		TableRowInfo tableRowProperties;
		#endregion
		protected StyleDestinationBase(OpenDocumentTextImporter importer)
			: base(importer) {
			this.styleName = String.Empty;
			this.nextStyleName = String.Empty;
			this.family = String.Empty;
			this.parentName = String.Empty;
		}
		#region Properties
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public CharacterFormattingBase CharacterFormatting {
			get {
				if (characterFormatting == null)
					this.characterFormatting = new CharacterFormattingBase(DocumentModel.MainPieceTable, DocumentModel, CharacterFormattingInfoCache.DefaultItemIndex, CharacterFormattingOptionsCache.EmptyCharacterFormattingOptionIndex);
				return characterFormatting;
			}
		}
		public ParagraphFormattingBase ParagraphFormatting {
			get {
				if (paragraphFormatting == null)
					this.paragraphFormatting = new ParagraphFormattingBase(DocumentModel.MainPieceTable, DocumentModel, ParagraphFormattingInfoCache.DefaultItemIndex, ParagraphFormattingOptionsCache.EmptyParagraphFormattingOptionIndex);
				return paragraphFormatting;
			}
		}
		public TabFormattingInfo Tabs {
			get {
				if (tabs == null)
					this.tabs = new TabFormattingInfo();
				return tabs;
			}
		}
		internal string StyleName { get { return styleName; } set { styleName = value; } }
		internal string NextParagraphStyleName { get { return nextStyleName; } set { nextStyleName = value; } }
		internal string Family { get { return family; } set { family = value; } }
		internal string ParentName { get { return parentName; } }
		internal string MasterPageName { get { return masterPageName; } }
		public ParagraphBreaksInfo ParagraphBreaks {
			get {
				if (paragraphBreaks == null)
					this.paragraphBreaks = new ParagraphBreaksInfo();
				return paragraphBreaks;
			}
		}
		public SectionStyleInfo SectionStyleInfo {
			get {
				if (sectionStyleInfo == null)
					this.sectionStyleInfo = new SectionStyleInfo();
				return sectionStyleInfo;
			}
			internal set { sectionStyleInfo = value; }
		}
		public TablePropertiesInfo TableProperties {
			get {
				if (tableProperties == null) {
					this.tableProperties = new TablePropertiesInfo(DocumentModel.MainPieceTable);
					this.tableProperties.Reset();
				}
				return tableProperties;
			}
		}
		public TableColumnWidthInfo TableColumnWidths {
			get {
				if (tableColumnWidths == null)
					this.tableColumnWidths = new TableColumnWidthInfo(0, false);
				return tableColumnWidths;
			}
		}
		public TableRowInfo TableRowProperties {
			get {
				if (tableRowProperties == null)
					this.tableRowProperties = new TableRowInfo(Importer.PieceTable);
				return tableRowProperties;
			}
		}
		public TableCellProperties TableCellProperties {
			get {
				if (tableCellProperties == null) {
					this.tableCellProperties = new TableCellProperties(DocumentModel.MainPieceTable, this);
					this.tableCellProperties.Reset();
				}
				return tableCellProperties;
			}
		}
		public FloatingObjectImportInfo FloatingObjectImportInfo {
			get {
				if (floatingObjectImportInfo == null)
					this.floatingObjectImportInfo = new FloatingObjectImportInfo(DocumentModel.MainPieceTable);
				return floatingObjectImportInfo;
			}
		}
		#endregion
		static StyleDestinationBase GetThis(OpenDocumentTextImporter importer) {
			return (StyleDestinationBase)importer.PeekDestination();
		}
		public override void ProcessElementOpen(XmlReader reader) {
			this.styleName = ImportHelper.GetStyleStringAttribute(reader, "name");
			this.family = ImportHelper.GetStyleStringAttribute(reader, "family").ToLower(CultureInfo.InvariantCulture);
			this.parentName = ImportHelper.GetStyleStringAttribute(reader, "parent-style-name");
			this.masterPageName = ImportHelper.GetStyleStringAttribute(reader, "master-page-name");
			this.nextStyleName = ImportHelper.GetStyleStringAttribute(reader, "next-style-name");
		}
		public override void ProcessElementClose(XmlReader reader) {
			switch (Family) {
				case "text":
					ImportCharacterStyle();
					break;
				case "paragraph":
					ImportParagraphStyle();
					break;
				case "section":
					ImportSectionStyle();
					break;
				case "table":
					ImportTableStyle();
					break;
				case "table-column":
					ImportTableColumnStyle();
					break;
				case "table-cell":
					ImportTableCellStyle();
					break;
				case "table-row":
					ImportTableRowStyle();
					break;
				case "graphic":
					ImportFrameStyle();
					break;
			}
		}
		protected static Destination OnParagraphProperties(OpenDocumentTextImporter importer, XmlReader reader) {
			ParagraphFormattingBase paragraphFormatting = GetThis(importer).ParagraphFormatting;
			paragraphFormatting.ReplaceInfo(importer.DocumentModel.Cache.ParagraphFormattingInfoCache.DefaultItem, new ParagraphFormattingOptions(ParagraphFormattingOptions.Mask.UseNone)); 
			return new ParagraphPropertiesDestination(importer, paragraphFormatting, GetThis(importer).Tabs, GetThis(importer).ParagraphBreaks);
		}
		protected static Destination OnTextProperties(OpenDocumentTextImporter importer, XmlReader reader) {
			CharacterFormattingBase characterFormatting = GetThis(importer).CharacterFormatting;
			characterFormatting.ReplaceInfo(importer.DocumentModel.Cache.CharacterFormattingInfoCache.DefaultItem, new CharacterFormattingOptions(CharacterFormattingOptions.Mask.UseNone));
			return new TextPropertiesDestination(importer, characterFormatting);
		}
		protected static Destination OnFrameProperties(OpenDocumentTextImporter importer, XmlReader reader) {
			FloatingObjectImportInfo floatingObjectImportInfo = GetThis(importer).FloatingObjectImportInfo;
			return new FramePropertiesDestination(importer, floatingObjectImportInfo);
		}
		protected static Destination OnSectionProperties(OpenDocumentTextImporter importer, XmlReader reader) {
			SectionStyleInfo info = GetThis(importer).SectionStyleInfo;
			return new SectionPropertiesDestination(importer, info);
		}
		#region Table Properties
		protected static Destination OnTableProperties(OpenDocumentTextImporter importer, XmlReader reader) {
			StyleDestinationBase thisDestination = GetThis(importer);
			TablePropertiesInfo info = thisDestination.TableProperties;
			info.MasterPageName = thisDestination.MasterPageName;
			return new TablePropertiesDestination(importer, info);
		}
		protected static Destination OnTableColumnProperties(OpenDocumentTextImporter importer, XmlReader reader) {
			return new TableColumnPropertiesDestination(importer, GetThis(importer).TableColumnWidths);
		}
		protected static Destination OnTableRowProperties(OpenDocumentTextImporter importer, XmlReader reader) {
			return new TableRowPropertiesDestination(importer, GetThis(importer).TableRowProperties);
		}
		protected static Destination OnTableCellProperties(OpenDocumentTextImporter importer, XmlReader reader) {
			return new TableCellPropertiesDestination(importer, GetThis(importer).TableCellProperties);
		}
		#endregion
		protected internal abstract void ImportParagraphStyle();
		protected internal abstract void ImportCharacterStyle();
		protected internal virtual void ImportStyleCore<T>(Dictionary<string, T> styles, T item) {
			if (!styles.ContainsKey(StyleName))
				styles.Add(StyleName, item);
		}
		protected internal virtual void ImportSectionStyle() {
			ImportStyleCore(Importer.SectionAutoStyles, SectionStyleInfo);
		}
		protected internal virtual void ImportTableStyle() {
			ImportStyleCore(Importer.TableAutoStyles, TableProperties);
		}
		protected internal virtual void ImportTableColumnStyle() {
			ImportStyleCore(Importer.TableColumnAutoStyles, TableColumnWidths);
		}
		protected internal virtual void ImportTableCellStyle() {
			ImportStyleCore(Importer.TableCellsAutoStyles, TableCellProperties);
		}
		protected internal virtual void ImportTableRowStyle() {
			ImportStyleCore(Importer.TableRowsAutoStyles, TableRowProperties);
		}
		protected internal virtual void ImportFrameStyle() {
			ImportStyleCore(Importer.FrameAutoStyles, FloatingObjectImportInfo);
		}
		protected internal virtual void ApplyParagraphStyleProperties(ParagraphStyle style, string newStyleName, ParagraphStyle parent, ParagraphStyle nextParagraphStyle) {
			if (parent != null)
				style.Parent = parent;
			if (characterFormatting != null)
				style.CharacterProperties.CopyFrom(characterFormatting);
			if (paragraphFormatting != null)
				style.ParagraphProperties.CopyFrom(paragraphFormatting);
			if (tabs != null)
				style.Tabs.SetTabs(tabs);
			if (nextParagraphStyle != null) {
				style.NextParagraphStyle = nextParagraphStyle;
			}
		}
		protected internal virtual void ApplyCharacterStyleProperties(CharacterStyle style, string newStyleName, CharacterStyle parent) {
			if (parent != null)
				style.Parent = parent;
			if (characterFormatting != null)
				style.CharacterProperties.CopyFrom(characterFormatting);
		}
		#region ICellPropertiesOwner Members
		IndexChangedHistoryItemCore<DocumentModelChangeActions> ICellPropertiesOwner.CreateCellPropertiesChangedHistoryItem(TableCellProperties properties) {
			Debug.Assert(properties == TableCellProperties);
			return new IndexChangedHistoryItem<DocumentModelChangeActions>(properties.PieceTable, properties);
		}
		#endregion
	}
	#endregion
	#region StyleDestination
	public class StyleDestination : StyleDestinationBase {
		public StyleDestination(OpenDocumentTextImporter importer)
			: base(importer) {
		}
		#region Create Parent Styles
		protected internal virtual int CreateParentCharacterStyle(string parent) {
			Debug.Assert(!String.IsNullOrEmpty(parent));
			int parentStyleIndex = DocumentModel.CharacterStyles.GetStyleIndexByName(parent);
			if (parentStyleIndex < 0) {
				CharacterStyle style = new CharacterStyle(DocumentModel);
				style.StyleName = parent;
				parentStyleIndex = DocumentModel.CharacterStyles.Add(style);
			}
			return parentStyleIndex;
		}
		protected internal virtual int ObtainParentParagraphStyle(string parentStyleName) {
			if (!string.IsNullOrEmpty(parentStyleName))
				return ObtainParagraphStyle(parentStyleName);
			return GetDefaultParentStyleIndex();
		}
		protected internal virtual int ObtainParentCharacterStyle(string parentStyleName) {
			if (!string.IsNullOrEmpty(parentStyleName))
				return ObtainCharacterStyle(parentStyleName);
			return GetDefaultCharacterStyleIndex();
		}
		protected internal virtual int GetDefaultParentStyleIndex() {
			return DocumentModel.ParagraphStyles.IndexOf(GetDefaultParagraphParentStyle());
		}
		protected internal virtual int GetDefaultCharacterStyleIndex() {
			return DocumentModel.CharacterStyles.IndexOf(GetDefaultCharacterParentStyle());
		}
		protected internal virtual int ObtainParagraphStyle(string styleName) {
			Debug.Assert(!String.IsNullOrEmpty(styleName));
			int parentStyleIndex = DocumentModel.ParagraphStyles.GetStyleIndexByName(styleName);
			if (parentStyleIndex < 0) {
				ParagraphStyle style = new ParagraphStyle(DocumentModel);
				style.StyleName = styleName;
				parentStyleIndex = DocumentModel.ParagraphStyles.Add(style);
			}
			return parentStyleIndex;
		}
		protected internal virtual int ObtainCharacterStyle(string styleName) {
			Debug.Assert(!String.IsNullOrEmpty(styleName));
			int parentStyleIndex = DocumentModel.CharacterStyles.GetStyleIndexByName(styleName);
			if (parentStyleIndex < 0) {
				CharacterStyle style = new CharacterStyle(DocumentModel);
				style.StyleName = styleName;
				parentStyleIndex = DocumentModel.CharacterStyles.Add(style);
			}
			return parentStyleIndex;
		}
		#endregion
		#region ImportParagraphStyle
		protected internal override void ImportParagraphStyle() {
			int styleIndex = ObtainParagraphStyle(StyleName);
			int parentStyleIndex = -1;
			if (styleIndex != GetDefaultParentStyleIndex())
				parentStyleIndex = ObtainParentParagraphStyle(ParentName);
			int nextParagraphStyleIndex = GetNextStyleIndex(styleIndex);
			ImportParagraphParentStyle(styleIndex, parentStyleIndex, nextParagraphStyleIndex);
		}
		int GetNextStyleIndex(int styleIndex) {
			if (!string.IsNullOrEmpty(NextParagraphStyleName))
				return ObtainParagraphStyle(NextParagraphStyleName);
			return styleIndex;
		}
		protected ParagraphStyle GetParagraphStyleByIndex(int styleIndex) {
			int count = DocumentModel.ParagraphStyles.Count;
			return styleIndex >= 0 && styleIndex < count ? DocumentModel.ParagraphStyles[styleIndex] : null;
		}
		protected CharacterStyle GetCharacterStyleByIndex(int styleIndex) {
			int count = DocumentModel.CharacterStyles.Count;
			return styleIndex >= 0 && styleIndex < count ? DocumentModel.CharacterStyles[styleIndex] : null;
		}
		protected internal virtual ParagraphStyle GetDefaultParagraphParentStyle() {
			return DocumentModel.ParagraphStyles.DefaultItem;
		}
		protected internal virtual CharacterStyle GetDefaultCharacterParentStyle() {
			return DocumentModel.CharacterStyles.DefaultItem;
		}
		protected internal virtual void ImportParagraphParentStyle(int styleIndex, int parentStyleIndex, int nextParagraphStyleIndex) {
			ParagraphStyle style = GetParagraphStyleByIndex(styleIndex);
			Debug.Assert(style != null);
			ParagraphStyle parent = null;
			if (style != GetDefaultParagraphParentStyle()) {
				parent = GetParagraphStyleByIndex(parentStyleIndex);
				Debug.Assert(parent != null);
				Debug.Assert(style.StyleName != parent.StyleName);
			}
			ParagraphStyle nextParagraphStyle = (styleIndex == nextParagraphStyleIndex) ? null : GetParagraphStyleByIndex(nextParagraphStyleIndex);
			ApplyParagraphStyleProperties(style, style.StyleName, parent, nextParagraphStyle);  
		}
		#endregion
		#region ImportCharacterStyle
		protected internal override void ImportCharacterStyle() {
			int styleIndex = ObtainCharacterStyle(StyleName);
			int characterStyleIndex = -1;
			if (styleIndex != GetDefaultCharacterStyleIndex())
				characterStyleIndex = ObtainParentCharacterStyle(ParentName);
			ImportCharacterParentStyle(styleIndex, characterStyleIndex);
		}
		protected internal virtual void ImportCharacterParentStyle(int styleIndex, int parentStyleIndex) {
			CharacterStyle style = GetCharacterStyleByIndex(styleIndex);
			Debug.Assert(style != null);
			CharacterStyle parent = null;
			if (style != GetDefaultCharacterParentStyle()) {
				parent = GetCharacterStyleByIndex(parentStyleIndex);
				Debug.Assert(parent != null);
				Debug.Assert(style.StyleName != parent.StyleName);
			}
			ApplyCharacterStyleProperties(style, style.StyleName, parent);  
		}
		#endregion
	}
	#endregion
	#region DefaultStyleDestination
	public partial class DefaultStyleDestination : StyleDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		public DefaultStyleDestination(OpenDocumentTextImporter importer)
			: base(importer) {
		}
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("paragraph-properties", OnParagraphProperties);
			result.Add("text-properties", OnTextProperties);
			return result;
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		protected internal override void ImportParagraphStyle() {
			ApplyDefaultTabs();
			ApplyDefaultProperties();
		}
		protected virtual void ApplyDefaultProperties() {
			ApplyDefaultCharacterProperties();
			ApplyDefaultParagraphProperties();
		}
		protected internal virtual void ApplyDefaultCharacterProperties() {
			CharacterPropertiesMerger merger = new CharacterPropertiesMerger(new MergedCharacterProperties(CharacterFormatting.Info, CharacterFormatting.Options));
			CharacterProperties properties = Importer.DocumentModel.DefaultCharacterProperties;
			merger.Merge(properties);
			properties.CopyFrom(merger.MergedProperties);
		}
		protected internal virtual void ApplyDefaultParagraphProperties() {
			ParagraphPropertiesMerger merger = new ParagraphPropertiesMerger(new MergedParagraphProperties(ParagraphFormatting.Info, ParagraphFormatting.Options));
			ParagraphProperties properties = Importer.DocumentModel.DefaultParagraphProperties;
			merger.Merge(properties);
			properties.CopyFrom(merger.MergedProperties);
		}
		protected internal virtual void ApplyDefaultTabs() {
		}
		protected internal override void ImportCharacterStyle() {
			ApplyDefaultCharacterProperties();
		}
	}
	#endregion
	#region ParagraphPropertiesDestination
	public class ParagraphPropertiesDestination : ElementDestination {
		readonly ParagraphFormattingBase paragraphProperties;
		readonly TabFormattingInfo tabs;
		readonly ParagraphBreaksInfo breakInfo;
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("tab-stops", OnTabStops);
			return result;
		}
		public ParagraphPropertiesDestination(OpenDocumentTextImporter importer, ParagraphFormattingBase paragraphFormatting, TabFormattingInfo tabs, ParagraphBreaksInfo breakInfo)
			: base(importer) {
			Guard.ArgumentNotNull(paragraphFormatting, "paragraphFormatting");
			Guard.ArgumentNotNull(tabs, "tabs");
			Guard.ArgumentNotNull(breakInfo, "breakInfo");
			this.paragraphProperties = paragraphFormatting;
			this.tabs = tabs;
			this.breakInfo = breakInfo;
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		protected internal ParagraphFormattingBase ParagraphFormatting { get { return paragraphProperties; } }
		public ParagraphBreaksInfo BreakInfo { get { return breakInfo; } }
		public TabFormattingInfo Tabs { get { return tabs; } }
		static ParagraphPropertiesDestination GetThis(OpenDocumentTextImporter importer) {
			return (ParagraphPropertiesDestination)importer.PeekDestination();
		}
		public override void ProcessElementOpen(XmlReader reader) {
			ParagraphFormatting.BeginUpdate();
			string alignment = ImportHelper.GetFoStringAttribute(reader, "text-align");
			if (!String.IsNullOrEmpty(alignment)) {
				ParagraphFormatting.Alignment = (ParagraphAlignment)OpenDocumentHelper.ParagraphAlignmentTable.GetEnumValue(alignment, OfficeParagraphAlignment.Left);
			}
			ImportFirstLineIndent(reader);
			ValueInfo leftIndent = ImportHelper.GetFoAttributeInfo(reader, "margin-left");
			ValueInfo rightIndent = ImportHelper.GetFoAttributeInfo(reader, "margin-right");
			ValueInfo spacingBefore = ImportHelper.GetFoAttributeInfo(reader, "margin-top");
			ValueInfo spacingAfter = ImportHelper.GetFoAttributeInfo(reader, "margin-bottom");
			if (!leftIndent.IsEmpty)
				ParagraphFormatting.LeftIndent = GetIntegerDocumentValue(leftIndent);
			if (!rightIndent.IsEmpty)
				ParagraphFormatting.RightIndent = GetIntegerDocumentValue(rightIndent);
			if (!spacingBefore.IsEmpty)
				ParagraphFormatting.SpacingBefore = GetIntegerDocumentValue(spacingBefore);
			if (!spacingAfter.IsEmpty)
				ParagraphFormatting.SpacingAfter = GetIntegerDocumentValue(spacingAfter);
			ImportStyleParagraphLineSpacing(reader);
			ImportBreakType(reader);
			ImportLineNumbering(reader);
			ImportKeepWithNext(reader);
			ImportKeepLinesTogether(reader);
			ImportWidowOrphanControl(reader);
			ImportOutlineLevel(reader);
			ImportBackgroundColor(reader);
		}
		protected internal virtual void ImportLineNumbering(XmlReader reader) {
			string value = ImportHelper.GetTextStringAttribute(reader, "number-lines");
			if (value == "false")
				ParagraphFormatting.SuppressLineNumbers = true;
		}
		protected internal virtual void ImportKeepWithNext(XmlReader reader) {
			string value = ImportHelper.GetFoStringAttribute(reader, "keep-with-next");
			if (value == "always")
				ParagraphFormatting.KeepWithNext = true;
		}
		protected internal virtual void ImportKeepLinesTogether(XmlReader reader) {
			string value = ImportHelper.GetFoStringAttribute(reader, "keep-together");
			if (value == "always")
				ParagraphFormatting.KeepLinesTogether = true;
		}
		protected internal virtual void ImportWidowOrphanControl(XmlReader reader) {
			int widows = ImportHelper.GetFoIntegerAttribute(reader, "widows", -1);
			int orphans = ImportHelper.GetFoIntegerAttribute(reader, "orphans", -1);
			if (widows != -1 || orphans != 1)
				ParagraphFormatting.WidowOrphanControl = true;
		}
		protected internal virtual void ImportOutlineLevel(XmlReader reader) {
			int outlineLevel = ImportHelper.GetStyleIntegerAttribute(reader, "default-outline-level", -1);
			if (outlineLevel >= 0)
				ParagraphFormatting.OutlineLevel = outlineLevel;
		}
		protected internal virtual void ImportBackgroundColor(XmlReader reader) {
			string backColor = ImportHelper.GetFoStringAttribute(reader, "background-color");
			if (!String.IsNullOrEmpty(backColor)) {
				Color backgroundColor = ImportHelper.ConvertStringToColor(backColor);
				if (backgroundColor != DXColor.Transparent) 
					ParagraphFormatting.BackColor = backgroundColor;
			}
		}
		protected internal virtual void ImportBreakType(XmlReader reader) {
			ImportHelper.ImportBreakType(reader, BreakInfo, ParagraphFormatting);
		}
		public override void ProcessElementClose(XmlReader reader) {
			ParagraphFormatting.BeginUpdate();
		}
		static Destination OnTabStops(OpenDocumentTextImporter importer, XmlReader reader) {
			TabFormattingInfo tabs = GetThis(importer).Tabs;
			ParagraphFormattingBase paragraphFormatting = GetThis(importer).ParagraphFormatting;
			return new TabStopsDestination(importer, tabs, paragraphFormatting.LeftIndent);
		}
		#region ImportStyleParagraphLineSpacing
		protected internal virtual void ImportStyleParagraphLineSpacing(XmlReader reader) {
			ParagraphLineSpacing type = ParagraphLineSpacing.Single;
			float lineSpacing = 0;
			string atLeast = ImportHelper.GetStyleStringAttribute(reader, "line-height-at-least");
			if (!String.IsNullOrEmpty(atLeast)) {
				ImportHelper.ConvertLineSpacingFromString(atLeast, Importer.UnitsConverter, out type, out lineSpacing);
				ParagraphFormatting.LineSpacingType = type;
				ParagraphFormatting.LineSpacing = lineSpacing;
			}
			else {
				string lineHeight = ImportHelper.GetFoStringAttribute(reader, "line-height");
				if (!String.IsNullOrEmpty(lineHeight)) {
					if (lineHeight.ToLower(CultureInfo.InvariantCulture) == "normal")
						return;
					ImportHelper.ConvertLineSpacingFromString(lineHeight, Importer.UnitsConverter, out type, out lineSpacing);
					ParagraphFormatting.LineSpacingType = type;
					ParagraphFormatting.LineSpacing = lineSpacing;
				}
			}
		}
		#endregion
		protected internal virtual void ImportFirstLineIndent(XmlReader reader) {
			ValueInfo value = ImportHelper.GetFoAttributeInfo(reader, "text-indent");
			if (value.IsEmpty)
				return;
			int indent = GetIntegerDocumentValue(value);
			ParagraphFormatting.FirstLineIndentType = GetFirstLineIndentType(indent);
			ParagraphFormatting.FirstLineIndent = Math.Abs(indent);
		}
		#region GetFirstLineIndentType
		protected internal ParagraphFirstLineIndent GetFirstLineIndentType(int firstLine) {
			if (firstLine != Int32.MinValue) {
				if (firstLine < 0)
					return ParagraphFirstLineIndent.Hanging;
				else if (firstLine > 0)
					return ParagraphFirstLineIndent.Indented;
				else
					return ParagraphFirstLineIndent.None;
			}
			return ParagraphFirstLineIndent.None;
		}
		#endregion
	}
	#endregion
	#region FramePropertiesDestination
	public class FramePropertiesDestination : LeafElementDestination {
		readonly FloatingObjectImportInfo floatingObjectImportInfo;
		public FramePropertiesDestination(OpenDocumentTextImporter importer, FloatingObjectImportInfo floatingObjectImportInfo)
			: base(importer) {
			this.floatingObjectImportInfo = floatingObjectImportInfo;
		}
		FloatingObjectProperties FloatingObject { get { return floatingObjectImportInfo.FloatingObjectProperties; } }
		Shape Shape { get { return floatingObjectImportInfo.Shape; } }
		TextBoxProperties TextBoxProperties { get { return floatingObjectImportInfo.TextBoxProperties; } }
		void ImportVerticalPositionAlignment(XmlReader reader) {
			string vPosAlignment = ImportHelper.GetStyleStringAttribute(reader, "vertical-pos");   
			if (!String.IsNullOrEmpty(vPosAlignment)) {
				FloatingObjectVerticalPositionAlignment verticalPositionAlignment;
				if(ImportHelper.VerticalPositionAlignmentTable.TryGetValue(vPosAlignment, out verticalPositionAlignment))
					FloatingObject.VerticalPositionAlignment = verticalPositionAlignment;
			}
		}
		void ImportHorizontalPositionAlignment(XmlReader reader) {
			string hPosAlignment = ImportHelper.GetStyleStringAttribute(reader, "horizontal-pos");
			if (!String.IsNullOrEmpty(hPosAlignment)) {
				FloatingObjectHorizontalPositionAlignment horizontalPositionAlignment;
				if(ImportHelper.HorizontalPositionAlignmentTable.TryGetValue(hPosAlignment, out horizontalPositionAlignment))
					FloatingObject.HorizontalPositionAlignment = horizontalPositionAlignment;
			}
		}
		void ImportVerticalPositionType(XmlReader reader) {
			string vPosType = ImportHelper.GetStyleStringAttribute(reader, "vertical-rel");
			if (!String.IsNullOrEmpty(vPosType)) {
				FloatingObjectVerticalPositionType verticalPositionType;
				if(ImportHelper.VerticalPositionTypeTable.TryGetValue(vPosType, out verticalPositionType))
					FloatingObject.VerticalPositionType = verticalPositionType;
			}
		}
		void ImportHorizontalPositionType(XmlReader reader) {
			string hPosType = ImportHelper.GetStyleStringAttribute(reader, "horizontal-rel");
			if (!String.IsNullOrEmpty(hPosType)) {
				FloatingObjectHorizontalPositionType horizontalPositionType;
				if(ImportHelper.HorizontalPositionTypeTable.TryGetValue(hPosType, out horizontalPositionType))
					FloatingObject.HorizontalPositionType = horizontalPositionType;
			}
		}
		void ImportWrapSideAndWrapType(XmlReader reader) {
			string wrap = ImportHelper.GetStyleStringAttribute(reader, "wrap");
			if (!String.IsNullOrEmpty(wrap)) {
				FloatingObjectTextWrapSide textWrapSide;
				FloatingObjectTextWrapType textWrapType;
				if (ImportHelper.TextWrapSideTable.TryGetValue(wrap, out textWrapSide))
					FloatingObject.TextWrapSide = textWrapSide;
				if (ImportHelper.TextWrapTypeTable.TryGetValue(wrap, out textWrapType)) {
					if (textWrapType == FloatingObjectTextWrapType.Through)
						FloatingObject.TextWrapType = FloatingObjectTextWrapType.None;
					else
						FloatingObject.TextWrapType = textWrapType;
				}
			}
		}
		void ImportLeftDistance(XmlReader reader) {
			ValueInfo leftDistance = ImportHelper.GetFoAttributeInfo(reader, "margin-left");
			if (!leftDistance.IsEmpty)
				FloatingObject.LeftDistance = GetIntegerDocumentValue(leftDistance);
		}
		void ImportRightDistance(XmlReader reader) {
			ValueInfo rightDistance = ImportHelper.GetFoAttributeInfo(reader, "margin-right");
			if (!rightDistance.IsEmpty)
				FloatingObject.RightDistance = GetIntegerDocumentValue(rightDistance);
		}
		void ImportBottomDistance(XmlReader reader) {
			ValueInfo bottomDistance = ImportHelper.GetFoAttributeInfo(reader, "margin-bottom");
			if (!bottomDistance.IsEmpty)
				FloatingObject.BottomDistance = GetIntegerDocumentValue(bottomDistance);
		}
		void ImportTopDistance(XmlReader reader) {
			ValueInfo topDistance = ImportHelper.GetFoAttributeInfo(reader, "margin-top");
			if (!topDistance.IsEmpty)
				FloatingObject.TopDistance = GetIntegerDocumentValue(topDistance);
		}
		void ImportPaddings(XmlReader reader) {
			ValueInfo value;
			value = ImportHelper.GetFoAttributeInfo(reader, "padding-top");
			if (!value.IsEmpty)
				TextBoxProperties.TopMargin = GetIntegerDocumentValue(value);
			value = ImportHelper.GetFoAttributeInfo(reader, "padding-bottom");
			if (!value.IsEmpty)
				TextBoxProperties.BottomMargin = GetIntegerDocumentValue(value);
			value = ImportHelper.GetFoAttributeInfo(reader, "padding-left");
			if (!value.IsEmpty)
				TextBoxProperties.LeftMargin = GetIntegerDocumentValue(value);
			value = ImportHelper.GetFoAttributeInfo(reader, "padding-right");
			if (!value.IsEmpty)
				TextBoxProperties.RightMargin = GetIntegerDocumentValue(value);
		}
		void ImportResizeShapeToFitText(XmlReader reader) {
			string value = ImportHelper.GetDrawStringAttribute(reader, "auto-grow-height");
			if (!String.IsNullOrEmpty(value))
				TextBoxProperties.ResizeShapeToFitText = (value == "true");
		}
		void ImportShapeProperties(XmlReader reader) {
			string fillColor = ImportHelper.GetDrawStringAttribute(reader, "fill-color");
			if (!String.IsNullOrEmpty(fillColor))
				Shape.FillColor = ImportHelper.ConvertStringToColor(fillColor);
			ValueInfo value = ImportHelper.GetSvgAttributeInfo(reader, "stroke-width");
			if (!value.IsEmpty) {
				Shape.OutlineWidth = GetIntegerDocumentValue(value);
				string strokeColor = ImportHelper.GetSvgStringAttribute(reader, "stroke-color");
				if (!String.IsNullOrEmpty(strokeColor))
					Shape.OutlineColor = ImportHelper.ConvertStringToColor(strokeColor);
				else
					Shape.OutlineColor = DXColor.Black;
			}
			string borderValue = ImportHelper.GetFoStringAttribute(reader, "border");
			if (!String.IsNullOrEmpty(borderValue)) {
				HtmlBorderProperty border = new HtmlBorderProperty();
				CssProperties cssProperties = new CssProperties(Importer.PieceTable);
				List<string> propertiesValue = new List<string>();
				propertiesValue.Add(borderValue);
				CssParser.CssBorderKeywordCore(cssProperties, propertiesValue, border);
				if (border.UseWidth && border.Width > 0) {
					Shape.OutlineWidth = border.Width;
					if (border.UseColor)
						Shape.OutlineColor = border.Color;
					else {
						if (border.LineStyle != BorderLineStyle.Nil && border.LineStyle != BorderLineStyle.None)
							Shape.OutlineColor = DXColor.Black;
						else
							Shape.OutlineColor = DXColor.Empty;
					}
				}
			}
		}
		void ImportIsBehindDoc(XmlReader reader) {
			string value = ImportHelper.GetStyleStringAttribute(reader, "run-through");
			if (!String.IsNullOrEmpty(value)) 
				FloatingObject.IsBehindDoc = (value.Equals("background") ? true : false);
		}
		protected internal virtual void ImportBackgroundColor(XmlReader reader) {
			string backColor = ImportHelper.GetFoStringAttribute(reader, "background-color");
			if (!String.IsNullOrEmpty(backColor)) {
				Color backgroundColor = ImportHelper.ConvertStringToColor(backColor);
				if (backgroundColor != DXColor.Transparent) 
					floatingObjectImportInfo.Shape.FillColor = backgroundColor;
			}
		}
		public override void ProcessElementOpen(XmlReader reader) {
			ImportHorizontalPositionAlignment(reader);
			ImportVerticalPositionAlignment(reader);
			ImportHorizontalPositionType(reader);
			ImportVerticalPositionType(reader);
			ImportIsBehindDoc(reader);
			ImportWrapSideAndWrapType(reader);
			ImportLeftDistance(reader);
			ImportRightDistance(reader);
			ImportBottomDistance(reader);
			ImportTopDistance(reader);
			ImportPaddings(reader);
			ImportResizeShapeToFitText(reader);
			ImportShapeProperties(reader);
			ImportBackgroundColor(reader);
		}
	}
	#endregion
	#region TextPropertiesDestination
	public class TextPropertiesDestination : LeafElementDestination {
		readonly CharacterFormattingBase characterProperties;
		string textUnderLineColor = String.Empty;
		public TextPropertiesDestination(OpenDocumentTextImporter importer, CharacterFormattingBase characterProperties)
			: base(importer) {
			Guard.ArgumentNotNull(characterProperties, "characterProperties");
			this.characterProperties = characterProperties;
		}
		protected internal CharacterFormattingBase CharacterProperties { get { return characterProperties; } }
		public override void ProcessElementOpen(XmlReader reader) {
			characterProperties.BeginUpdate();
			ImportFontSize(reader);
			ImportFontItalic(reader);
			ImportFontBold(reader);
			ImportFontName(reader);
			ImportColors(reader);
			ImportHidden(reader);
			ImportTextUnderlineProperties(reader);
			ImportTextStrikeOutProperties(reader);
			ImportTextScriptProperties(reader);
		}
		private void ImportHidden(XmlReader reader) {
			string hidden = ImportHelper.GetTextStringAttribute(reader, "display");
			if (!String.IsNullOrEmpty(hidden))
				CharacterProperties.Hidden = (hidden.ToLower(CultureInfo.InvariantCulture) == "none");
		}
		private void ImportColors(XmlReader reader) {
			string color = ImportHelper.GetFoStringAttribute(reader, "color");
			string backColor = ImportHelper.GetFoStringAttribute(reader, "background-color");
			if (!String.IsNullOrEmpty(color)) {
				Color fontColor = ImportHelper.ConvertStringToColor(color);
				if (fontColor != DXColor.Transparent) 
					CharacterProperties.ForeColor = fontColor;
			}
			if (!String.IsNullOrEmpty(backColor)) {
				Color backgroundColor = ImportHelper.ConvertStringToColor(backColor);
				if (backgroundColor != DXColor.Transparent) 
					CharacterProperties.BackColor = backgroundColor;
			}
		}
		private void ImportFontBold(XmlReader reader) {
			string fontBold = ImportHelper.GetFoStringAttribute(reader, "font-weight");
			if (!String.IsNullOrEmpty(fontBold))
				CharacterProperties.FontBold = OpenDocumentHelper.FontBoldTable.GetEnumValue(fontBold, false);
		}
		private void ImportFontSize(XmlReader reader) {
			ValueInfo fontSize = ImportHelper.GetFoAttributeInfo(reader, "font-size");
			if (fontSize.Unit == "%" && fontSize.Value > 0) {
				int defaultFontSize = Importer.DocumentModel.CharacterStyles.DefaultItem.CharacterProperties.DoubleFontSize;
				CharacterProperties.DoubleFontSize = Math.Max((int)Math.Round(UnitsConverter.ValueUnitToModelUnitsF(fontSize) * defaultFontSize), 1);
			}
			else if (fontSize.Value > 0)
				CharacterProperties.DoubleFontSize = Math.Max((int)Math.Round(fontSize.Value*2), 1);
		}
		private void ImportFontItalic(XmlReader reader) {
			string fontItalic = ImportHelper.GetFoStringAttribute(reader, "font-style");
			if (!String.IsNullOrEmpty(fontItalic))
				CharacterProperties.FontItalic = OpenDocumentHelper.FontStyleTable.GetEnumValue(fontItalic, false);
		}
		private void ImportFontName(XmlReader reader) {
			string fontName = ImportHelper.GetStyleStringAttribute(reader, "font-name");
			if (String.IsNullOrEmpty(fontName))
				return;
			if (Importer.FontTable.ContainsKey(fontName))
				CharacterProperties.FontName = Importer.FontTable[fontName];
			else
				CharacterProperties.FontName = fontName;
		}
		public override void ProcessElementClose(XmlReader reader) {
			if (textUnderLineColor == "font-color")
				CharacterProperties.UnderlineColor = CharacterProperties.ForeColor;
			characterProperties.EndUpdate();
		}
		protected internal virtual void ImportTextUnderlineProperties(XmlReader reader) {
			ImportTextUnderlineType(reader);
			ImportTextUnderlineColor(reader);
			ImportTextUnderlineMode(reader);
		}
		protected internal virtual void ImportTextUnderlineMode(XmlReader reader) {
			string underlineMode = ImportHelper.GetStyleStringAttribute(reader, "text-underline-mode");
			if (!String.IsNullOrEmpty(underlineMode)) {
				CharacterProperties.UnderlineWordsOnly = (underlineMode.ToLower(CultureInfo.InvariantCulture) == "skip-white-space");
			}
		}
		protected internal virtual void ImportStyleTextStrikeOutType(XmlReader reader) {
			string strikeoutType = ImportHelper.GetStyleStringAttribute(reader, "text-line-through-type").ToLower(CultureInfo.InvariantCulture);
			string textLineThroughStyle = ImportHelper.GetStyleStringAttribute(reader, "text-line-through-style").ToLower(CultureInfo.InvariantCulture);
			if (!String.IsNullOrEmpty(textLineThroughStyle))
				CharacterProperties.FontStrikeoutType = ImportHelper.GetFontStrikeoutType(strikeoutType, textLineThroughStyle);
			else if (!String.IsNullOrEmpty(strikeoutType))
				CharacterProperties.FontStrikeoutType = ImportHelper.FontStrikeoutThinTypeTable.GetEnumValue(strikeoutType);
		}
		protected internal virtual void ImportTextUnderlineColor(XmlReader reader) {
			this.textUnderLineColor = ImportHelper.GetStyleStringAttribute(reader, "text-underline-color");
			if (!String.IsNullOrEmpty(this.textUnderLineColor) && this.textUnderLineColor != "font-color") {
				CharacterProperties.UnderlineColor = ImportHelper.ConvertStringToColor(textUnderLineColor);
			}
		}
		protected internal virtual void ImportTextUnderlineType(XmlReader reader) {
			string underlineWidth = ImportHelper.GetStyleStringAttribute(reader, "text-underline-width");
			string underlineType = ImportHelper.GetStyleStringAttribute(reader, "text-underline-type");
			string underlineStyle = ImportHelper.GetStyleStringAttribute(reader, "text-underline-style").ToLower(CultureInfo.InvariantCulture);
			bool bold = OpenDocumentHelper.FontUnderLineWidthTable.GetEnumValue(underlineWidth, false);
			if (String.IsNullOrEmpty(underlineStyle) && String.IsNullOrEmpty(underlineType) && String.IsNullOrEmpty(underlineWidth))
				return;
			CharacterProperties.FontUnderlineType = ImportHelper.GetUnderlineType(underlineStyle, bold, underlineType);
		}
		protected internal virtual void ImportTextScriptProperties(XmlReader reader) {
			string textPosition = ImportHelper.GetStyleStringAttribute(reader, "text-position");
			if (!String.IsNullOrEmpty(textPosition)) {
				int spacePos = textPosition.IndexOf(' ');
				if (spacePos > 0)
					textPosition = textPosition.Substring(0, spacePos);
				CharacterProperties.Script = OpenDocumentHelper.CharacterScriptTable.GetEnumValue(textPosition, CharacterFormattingScript.Normal);
			}
		}
		protected internal virtual void ImportTextStrikeOutProperties(XmlReader reader) {
			ImportStyleTextStrikeOutType(reader);
			string strikeoutColor = ImportHelper.GetStyleStringAttribute(reader, "text-line-through-color");
			if (!String.IsNullOrEmpty(strikeoutColor)) {
				CharacterProperties.StrikeoutColor = ImportHelper.ConvertStringToColor(strikeoutColor);
			}
			string strikeoutWordsOnly = ImportHelper.GetStyleStringAttribute(reader, "text-line-through-mode");
			if (!String.IsNullOrEmpty(strikeoutWordsOnly)) {
				CharacterProperties.StrikeoutWordsOnly = (strikeoutWordsOnly.ToLower(CultureInfo.InvariantCulture) == "skip-white-space");
			}
		}
	}
	#endregion
	#region SectionPropertiesDestination
	public class SectionPropertiesDestination : ElementDestination {
		readonly SectionStyleInfo sectionStyleInfo;
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("columns", OnColumns);
			return result;
		}
		public SectionPropertiesDestination(OpenDocumentTextImporter importer, SectionStyleInfo styleInfo)
			: base(importer) {
			Guard.ArgumentNotNull(styleInfo, "styleInfo");
			this.sectionStyleInfo = styleInfo;
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public SectionStyleInfo SectionStyleInfo { get { return sectionStyleInfo; } }
		static SectionPropertiesDestination GetThis(OpenDocumentTextImporter importer) {
			return (SectionPropertiesDestination)importer.PeekDestination();
		}
		static Destination OnColumns(OpenDocumentTextImporter importer, XmlReader reader) {
			ColumnStyles columnStyles = GetThis(importer).SectionStyleInfo.Columns;
			return new ColumnsStyleDestination(importer, columnStyles);
		}
		public override void ProcessElementOpen(XmlReader reader) {
			ValueInfo leftMargin = ImportHelper.GetFoAttributeInfo(reader, "margin-left");
			ValueInfo rightMargin = ImportHelper.GetFoAttributeInfo(reader, "margin-right");
			SectionStyleInfo.LeftMargin = GetIntegerDocumentValue(leftMargin);
			SectionStyleInfo.RightMargin = GetIntegerDocumentValue(rightMargin);
		}
	}
	#endregion
	#region TablePropertiesDestination
	public class TablePropertiesDestination : LeafElementDestination {
		readonly TablePropertiesInfo tableInfo;
		public TablePropertiesDestination(OpenDocumentTextImporter importer, TablePropertiesInfo tableInfo)
			: base(importer) {
			Guard.ArgumentNotNull(tableInfo, "TablePropertiesInfo");
			this.tableInfo = tableInfo;
		}
		public TableProperties TableProperties { get { return tableInfo.TableProperties; } }
		public ParagraphBreaksInfo Breaks { get { return tableInfo.Breaks; } }
		protected TablePropertiesInfo TableInfo { get { return tableInfo; } }
		public override void ProcessElementOpen(XmlReader reader) {
			TableProperties.BeginUpdate();
			ImportLeftIndent(reader);
			ImportWidth(reader);
			ImportAlign(reader);
			ImportVerticalMargins(reader);
			ImportTableLayout(reader);
		}
		protected internal virtual void ImportWidth(XmlReader reader) {
			ValueInfo readValue = ImportHelper.GetStyleAttributeInfo(reader,  "width");
			if (!readValue.IsEmpty) {
				int convertedValue = GetPositiveIntegerDocumentValue(readValue);
				if (IsSupposedAutoPreferedWidth(convertedValue)) {
					TableProperties.PreferredWidth.Value = 0;
					TableProperties.PreferredWidth.Type = WidthUnitType.Auto;
				}
				else {
					TableProperties.PreferredWidth.Value = convertedValue;
					TableProperties.PreferredWidth.Type = WidthUnitType.ModelUnits;
				}
			}
		}
		bool IsSupposedAutoPreferedWidth(int readValue) {
			return readValue == 0;
		}
		protected internal virtual void ImportLeftIndent(XmlReader reader) {
			ValueInfo readValue = ImportHelper.GetFoAttributeInfo(reader, "margin-left");
			if (!readValue.IsEmpty) {
				TableProperties.TableIndent.Value = GetPositiveIntegerDocumentValue(readValue);
				TableProperties.TableIndent.Type = WidthUnitType.ModelUnits;
			}
		}
		protected internal virtual void ImportAlign(XmlReader reader) {
		   string value = ImportHelper.GetTableStringAttribute(reader, "align");
		   OpenDocumentTableAlignment align = OpenDocumentHelper.TableRowAlignmentTable.GetEnumValue(value, OpenDocumentTableAlignment.Left);
		   if (align != OpenDocumentTableAlignment.Left)
			   TableInfo.RowAlignment = (TableRowAlignment) align;
		}
		#region Background color
		protected internal virtual void ImportBackgroundColor(XmlReader reader) {
			string backColor = ImportHelper.GetFoStringAttribute(reader, "background-color");
			if (!String.IsNullOrEmpty(backColor)) {
			}
		}
		void ImportVerticalMargins(XmlReader reader) {
			ValueInfo marginTop = ImportHelper.GetFoAttributeInfo(reader, "margin-top");
			if (!marginTop.IsEmpty && marginTop.Unit != "%")
				TableProperties.FloatingPosition.TopFromText = GetIntegerDocumentValue(marginTop);
			ValueInfo marginBottom = ImportHelper.GetFoAttributeInfo(reader, "margin-bottom");
			if (!marginBottom.IsEmpty && marginTop.Unit != "%")
				TableProperties.FloatingPosition.BottomFromText = GetIntegerDocumentValue(marginBottom);
		}
		protected internal virtual void ImportTableLayout(XmlReader reader) {
			TableProperties.TableLayout = TableLayoutType.Autofit;
		}
		protected internal virtual void ImportTableLook(XmlReader reader) {
		}
		#endregion
		public override void ProcessElementClose(XmlReader reader) {
			TableProperties.EndUpdate();
		}
	}
	#endregion
	#region TableColumnPropertiesDestination
	public class TableColumnPropertiesDestination : LeafElementDestination {
		readonly TableColumnWidthInfo tableColumnWidth;
		public TableColumnPropertiesDestination(OpenDocumentTextImporter importer, TableColumnWidthInfo tableColumnWidth)
			: base(importer) {
			this.tableColumnWidth = tableColumnWidth;
		}
		public TableColumnWidthInfo TableColumnWidhts { get { return tableColumnWidth; } }
		public override void ProcessElementOpen(XmlReader reader) {
			ValueInfo columnWidth = ImportHelper.GetStyleAttributeInfo(reader, "column-width");
			int width = GetIntegerDocumentValue(columnWidth);
			if (width != 0)
				tableColumnWidth.Width = width;
			tableColumnWidth.UseOptimalColumnWidth = ImportHelper.GetStyleBoolAttribute(reader, "use-optimal-column-width", true);
		}
	}
	#endregion
	#region TableRowPropertiesDestination
	public class TableRowPropertiesDestination : LeafElementDestination {
		readonly TableRowInfo properties;
		public TableRowPropertiesDestination(OpenDocumentTextImporter importer, TableRowInfo properties)
			: base(importer) {
			this.properties = properties;
		}
		TableRowInfo TableRowProperties { get { return properties; } }
		public override void ProcessElementOpen(XmlReader reader) {
			ImportRowHeight(reader);
			ImportCantSplit(reader);
		}
		protected internal virtual void ImportRowHeight(XmlReader reader) {
			ValueInfo rowsHeight = ImportHelper.GetStyleAttributeInfo(reader, "row-height");
			ValueInfo minHeight = ImportHelper.GetStyleAttributeInfo(reader, "min-row-height");
			HeightUnitInfo height = new HeightUnitInfo();
			if (!rowsHeight.IsEmpty) {				
				height.Value = GetIntegerDocumentValue(rowsHeight);
				if (height.Value != 0)
					height.Type = HeightUnitType.Exact;
				TableRowProperties.Properties.Height.CopyFrom(height);
			}
			else if (!minHeight.IsEmpty)   {									 
				height.Value = GetIntegerDocumentValue(minHeight);
				if (height.Value != 0)
					height.Type = HeightUnitType.Minimum;
				TableRowProperties.Properties.Height.CopyFrom(height);
			}
		}
		protected internal virtual void ImportCantSplit(XmlReader reader) {
			string value = ImportHelper.GetFoStringAttribute(reader, "keep-together");
			if (!String.IsNullOrEmpty(value) && value == "always")
				TableRowProperties.Properties.CantSplit = true;
		}
	}
	#endregion
	#region TableCellPropertiesDestination
	public class TableCellPropertiesDestination : LeafElementDestination {
		readonly TableCellProperties properties;
		public TableCellPropertiesDestination(OpenDocumentTextImporter importer, TableCellProperties properties)
			: base(importer) {
			Guard.ArgumentNotNull(properties, "TableCellProperties");
			this.properties = properties;
		}
		public TableCellProperties Properties { get { return properties; } }
		public override void ProcessElementOpen(XmlReader reader) {
			Properties.BeginUpdate();
			ImportBorderProperties(reader);			
			ImportWrap(reader);
			ImportPaddingProperty(reader);
			ImportBackgroundColor(reader);
			ImportWritingMode(reader);
			ImportVerticalAlignment(reader);
		}
		protected internal virtual void ImportWrap(XmlReader reader) {
			string value = ImportHelper.GetFoStringAttribute(reader, "wrap-option");
			if (!string.IsNullOrEmpty(value) && value == "no-wrap")
				Properties.NoWrap = true;
		}
		public override void ProcessElementClose(XmlReader reader) {
			Properties.EndUpdate();
		}
		#region Borders
		void ImportBorderProperties(XmlReader reader) {
			string border = ImportHelper.GetFoStringAttribute(reader, "border");
			if (!String.IsNullOrEmpty(border))
				ImportSameBorders(border);
			ImportCurrentBorder(reader, Properties.Borders.LeftBorder, "border-left");
			ImportCurrentBorder(reader, Properties.Borders.RightBorder, "border-right");
			ImportCurrentBorder(reader, Properties.Borders.TopBorder, "border-top");
			ImportCurrentBorder(reader, Properties.Borders.BottomBorder, "border-bottom");
			string bordersLineWidth = ImportHelper.GetStyleStringAttribute(reader, "border-line-width");
			if (!String.IsNullOrEmpty(bordersLineWidth))
				ImportSameBordersLineStyle(bordersLineWidth);
			ImportBorderLineStyle(reader, "border-line-width-left", Properties.Borders.LeftBorder);
			ImportBorderLineStyle(reader, "border-line-width-right", Properties.Borders.RightBorder);
			ImportBorderLineStyle(reader, "border-line-width-top", Properties.Borders.TopBorder);
			ImportBorderLineStyle(reader, "border-line-width-bottom", Properties.Borders.BottomBorder);
		}
		void ImportCurrentBorder(XmlReader reader, BorderBase border, string attributeName) {
			string content = ImportHelper.GetFoStringAttribute(reader, attributeName);
			if (String.IsNullOrEmpty(content))
				return;
			BorderInfo info = ImportHelper.ParseBordersContent(content, UnitsConverter);
			border.CopyFrom(info);
		}
		void ImportSameBorders(string border) {
			BorderInfo info = ImportHelper.ParseBordersContent(border, UnitsConverter);
			if (info != null) {
				Properties.Borders.LeftBorder.CopyFrom(info);
				Properties.Borders.RightBorder.CopyFrom(info);
				Properties.Borders.TopBorder.CopyFrom(info);
				Properties.Borders.BottomBorder.CopyFrom(info);
			}
		}
		void ImportBorderLineStyle(XmlReader reader, string attributeName, BorderBase border) {
			string leftBorderLineWidth = ImportHelper.GetStyleStringAttribute(reader, attributeName);
			if (!String.IsNullOrEmpty(leftBorderLineWidth)) {
				BorderLineStyle lineStyle = ImportHelper.GetBorderLineStyle(leftBorderLineWidth, UnitsConverter);
				border.Style = lineStyle;
			}
		}
		void ImportSameBordersLineStyle(string bordersLineWidth) {
			BorderLineStyle lineStyle = ImportHelper.GetBorderLineStyle(bordersLineWidth, UnitsConverter);
			if (Properties.Borders.LeftBorder.Style != BorderLineStyle.None)
				Properties.Borders.LeftBorder.Style = lineStyle;
			if (Properties.Borders.RightBorder.Style != BorderLineStyle.None)
				Properties.Borders.RightBorder.Style = lineStyle;
			if (Properties.Borders.TopBorder.Style != BorderLineStyle.None)
				Properties.Borders.TopBorder.Style = lineStyle;
			if (Properties.Borders.BottomBorder.Style != BorderLineStyle.None)
				Properties.Borders.BottomBorder.Style = lineStyle;
		}
		#endregion
		#region Padding
		protected internal virtual void ImportPaddingProperty(XmlReader reader) {
			string padding = ImportHelper.GetFoStringAttribute(reader, "padding");
			if (!String.IsNullOrEmpty(padding))
				ImportSameCellMargins(padding);
			else {
				ImportCurrentCellMargin(reader, Properties.CellMargins.Left, "padding-left");
				ImportCurrentCellMargin(reader, Properties.CellMargins.Right, "padding-right");
				ImportCurrentCellMargin(reader, Properties.CellMargins.Top, "padding-top");
				ImportCurrentCellMargin(reader, Properties.CellMargins.Bottom, "padding-bottom");
			}
		}
		protected internal virtual void ImportCurrentCellMargin(XmlReader reader, WidthUnit marginUnit, string attributeName) {
			string content = ImportHelper.GetFoStringAttribute(reader, attributeName);
			if (String.IsNullOrEmpty(content))
				return;
			ValueInfo valueInfo = ImportHelper.GetFoAttributeInfo(reader, attributeName);
			if (valueInfo != ValueInfo.Empty){
				marginUnit.Value = GetIntegerDocumentValue(valueInfo);
				marginUnit.Type = WidthUnitType.ModelUnits;
			}
		}
		protected internal virtual void ImportSameCellMargins(string padding) {
			if (String.IsNullOrEmpty(padding))
				return;
			ValueInfo valueInfo = StringValueParser.TryParse(padding);
			if (valueInfo != ValueInfo.Empty) {
				int docValue = GetIntegerDocumentValue(valueInfo);
				Properties.CellMargins.Left.Value = docValue;
				Properties.CellMargins.Right.Value = docValue;
				Properties.CellMargins.Top.Value = docValue;
				Properties.CellMargins.Bottom.Value = docValue;
				Properties.CellMargins.Left.Type = WidthUnitType.ModelUnits;
				Properties.CellMargins.Right.Type = WidthUnitType.ModelUnits;
				Properties.CellMargins.Top.Type = WidthUnitType.ModelUnits;
				Properties.CellMargins.Bottom.Type = WidthUnitType.ModelUnits;
			}
		}
		#endregion
		#region Background color
		protected internal virtual void ImportBackgroundColor(XmlReader reader) {
			string backColor = ImportHelper.GetFoStringAttribute(reader, "background-color");
			if (!String.IsNullOrEmpty(backColor)) {
				Color backgroundColor = ImportHelper.ConvertStringToColor(backColor);
				if (backgroundColor != DXColor.Empty)
					Properties.BackgroundColor = backgroundColor;
			}
		}
		#endregion
		protected internal virtual void ImportWritingMode(XmlReader reader) {
			string value = ImportHelper.GetStyleStringAttribute(reader, "writing-mode");
			TextDirection direction = OpenDocumentHelper.TextDirectionTable.GetEnumValue(value, TextDirection.LeftToRightTopToBottom);
			if (direction != TextDirection.LeftToRightTopToBottom)
				Properties.TextDirection = direction;
		}
		protected internal virtual void ImportVerticalAlignment(XmlReader reader) {
			string value = ImportHelper.GetStyleStringAttribute(reader, "vertical-align");
			OpenDocumentVerticalAlignment align = OpenDocumentHelper.VerticalAlignmentTable.GetEnumValue(value, OpenDocumentVerticalAlignment.Top);
			if (align != OpenDocumentVerticalAlignment.Top)
				Properties.VerticalAlignment = (VerticalAlignment)align;
		}
	}
	#endregion
	#region TabStopsDestination
	public class TabStopsDestination : ElementDestination {
		readonly TabFormattingInfo tabs;
		readonly int leftIndent;
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("tab-stop", OnTabStop);
			return result;
		}
		public TabStopsDestination(OpenDocumentTextImporter importer, TabFormattingInfo tabs, int leftIndent)
			: base(importer) {
			Guard.ArgumentNotNull(tabs, "tabs");
			this.tabs = tabs;
			this.leftIndent = leftIndent;
		}
		static TabStopsDestination GetThis(OpenDocumentTextImporter importer) {
			return (TabStopsDestination)importer.PeekDestination();
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static TabFormattingInfo GetTabs(OpenDocumentTextImporter importer) {
			return GetThis(importer).tabs;
		}
		static int GetLeftIndent(OpenDocumentTextImporter importer) {
			return GetThis(importer).leftIndent;
		}
		static Destination OnTabStop(OpenDocumentTextImporter importer, XmlReader reader) {
			return new TabStopDestination(importer, GetTabs(importer), GetLeftIndent(importer));
		}
	}
	#endregion
	#region TabStopDestination
	public class TabStopDestination : LeafElementDestination {
		readonly TabFormattingInfo tabs;
		readonly int leftIndent;
		public TabStopDestination(OpenDocumentTextImporter importer, TabFormattingInfo tabs, int leftIndent)
			: base(importer) {
			this.tabs = tabs;
			this.leftIndent = leftIndent;
		}
		int position;
		TabLeaderType leader;
		TabAlignmentType alignment;
		public TabFormattingInfo Tabs { get { return tabs; } }
		public override void ProcessElementOpen(XmlReader reader) {
			ValueInfo valueInfo = ImportHelper.GetStyleAttributeInfo(reader, "position");
			this.position = (int)UnitsConverter.ValueUnitToModelUnitsF(valueInfo) + this.leftIndent;
			this.leader = OpenDocumentHelper.GetEnumValue(reader, "leader-text", OpenDocumentHelper.StyleNamespace, OpenDocumentHelper.TabLeaderTable, TabLeaderType.None);
			this.alignment = OpenDocumentHelper.GetEnumValue(reader, "type", OpenDocumentHelper.StyleNamespace, OpenDocumentHelper.TabAlignmentTypeTable, TabAlignmentType.Left);
			string characterRepresentaton = ImportHelper.GetStyleStringAttribute(reader, "char");
			if (this.alignment == TabAlignmentType.Decimal && characterRepresentaton != ",")
				this.alignment = TabAlignmentType.Left;
		}
		public override void ProcessElementClose(XmlReader reader) {
			Tabs.Add(new TabInfo(this.position, this.alignment, this.leader));
		}
	}
	#endregion
	#region LineNumberingConfigDestination
	public class LineNumberingConfigDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			return result;
		}
		public LineNumberingConfigDestination(OpenDocumentTextImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
			string enableNumberLines = ImportHelper.GetTextStringAttribute(reader, "number-lines");
			Importer.LineNumbering.ShowLineNumbering = (enableNumberLines == "false") ? false : true;
			const int defaultOoIncrement = 5;
			ValueInfo increment = ImportHelper.GetTextAttributeInfo(reader, "increment");
			Importer.LineNumbering.Step = (increment == ValueInfo.Empty) ? defaultOoIncrement : (int)increment.Value;
			ValueInfo offset = ImportHelper.GetTextAttributeInfo(reader, "offset");
			if (offset == ValueInfo.Empty || offset.Value < 0) {
				offset = new ValueInfo(0f, "cm");
				Importer.LineNumbering.ShowLineNumbering = false;
			}
			Importer.LineNumbering.Distance = (int)UnitsConverter.ValueUnitToModelUnitsF(offset); 
			Importer.LineNumbering.StartingLineNumber = 1;
			Importer.LineNumbering.NumberingRestartType = LineNumberingRestart.Continuous;
		}
	}
	#endregion
}
