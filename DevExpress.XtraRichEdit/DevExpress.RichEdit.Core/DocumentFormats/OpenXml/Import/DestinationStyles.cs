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
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Export.OpenXml;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Import.OpenXml {
	#region StylesDestination
	public class StylesDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("docDefaults", OnDocumentDefaults);
			result.Add("style", OnStyle);
			return result;
		}
		public StylesDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static Destination OnDocumentDefaults(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new DocumentDefaultsDestination(importer);
		}
		static Destination OnStyle(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new StyleDestination(importer);
		}
	}
	#endregion
	#region StyleDestinationBase
	public abstract class StyleDestinationBase : ElementDestination, ICellPropertiesOwner {
		protected static void FillElementHandlerTable(ElementHandlerTable result) {			
			result.Add("pPr", OnParagraphProperties);
			result.Add("rPr", OnRunProperties);
			result.Add("tblPr", OnTableProperties);
			result.Add("trPr", OnTableRowProperties);
			result.Add("tcPr", OnTableCellProperties);			
		}
		readonly CharacterFormattingBase characterFormatting;
		readonly ParagraphFormattingBase paragraphFormatting;
		readonly TabFormattingInfo tabs;
		readonly TableProperties tableProperties;
		readonly TableRowProperties tableRowProperties;
		readonly TableCellProperties tableCellProperties;
		int numberingId = -1;
		int listLevelIndex;
		protected StyleDestinationBase(WordProcessingMLBaseImporter importer)
			: base(importer) {
			PieceTable mainPieceTable = DocumentModel.MainPieceTable;
			this.characterFormatting = new CharacterFormattingBase(mainPieceTable, DocumentModel, CharacterFormattingInfoCache.DefaultItemIndex, CharacterFormattingOptionsCache.EmptyCharacterFormattingOptionIndex);
			this.paragraphFormatting = new ParagraphFormattingBase(mainPieceTable, DocumentModel, ParagraphFormattingInfoCache.DefaultItemIndex, ParagraphFormattingOptionsCache.EmptyParagraphFormattingOptionIndex);
			this.tableProperties = new TableProperties(mainPieceTable);
			this.tableRowProperties = new TableRowProperties(mainPieceTable);
			this.tableCellProperties = new TableCellProperties(mainPieceTable, this);
			this.tabs = new TabFormattingInfo();
		}
		protected CharacterFormattingBase CharacterFormatting { get { return characterFormatting; } }
		protected ParagraphFormattingBase ParagraphFormatting { get { return paragraphFormatting; } }
		protected TabFormattingInfo Tabs { get { return tabs; } }
		protected TableProperties TableProperties { get { return tableProperties; } }
		protected TableRowProperties TableRowProperties { get { return tableRowProperties; } }
		protected TableCellProperties TableCellProperties { get { return tableCellProperties; } }
		public int NumberingId { get { return numberingId; } set { numberingId = value; } }
		public int ListLevelIndex { get { return listLevelIndex; } set { listLevelIndex = value; } }
		public override void ProcessElementOpen(XmlReader reader) {
			characterFormatting.BeginUpdate();
			paragraphFormatting.BeginUpdate();
			tableProperties.BeginUpdate();
			tableRowProperties.BeginUpdate();
			tableCellProperties.BeginUpdate();
		}
		public override void ProcessElementClose(XmlReader reader) {
			characterFormatting.EndUpdate();
			paragraphFormatting.EndUpdate();
			tableProperties.EndUpdate();
			tableRowProperties.EndUpdate();
			tableCellProperties.EndUpdate();			
		}
		static StyleDestinationBase GetThis(WordProcessingMLBaseImporter importer) {
			return (StyleDestinationBase)importer.PeekDestination();
		}
		static Destination OnParagraphProperties(WordProcessingMLBaseImporter importer, XmlReader reader) {
			ParagraphFormattingBase paragraphFormatting = GetThis(importer).paragraphFormatting;
			paragraphFormatting.ReplaceInfo(importer.DocumentModel.Cache.ParagraphFormattingInfoCache.DefaultItem, new ParagraphFormattingOptions(ParagraphFormattingOptions.Mask.UseNone));
			return importer.CreateStyleParagraphPropertiesDestination(GetThis(importer), paragraphFormatting, GetThis(importer).tabs);
		}
		static Destination OnRunProperties(WordProcessingMLBaseImporter importer, XmlReader reader) {
			CharacterFormattingBase characterFormatting = GetThis(importer).characterFormatting;
			characterFormatting.ReplaceInfo(importer.DocumentModel.Cache.CharacterFormattingInfoCache.DefaultItem, new CharacterFormattingOptions(CharacterFormattingOptions.Mask.UseNone));
			return new StyleRunPropertiesDestination(importer, characterFormatting);
		}
		static Destination OnTableProperties(WordProcessingMLBaseImporter importer, XmlReader reader) {
			TableProperties tableProperties = GetThis(importer).tableProperties;
			tableProperties.ReplaceInfo(importer.DocumentModel.Cache.TablePropertiesOptionsCache.DefaultItem, DocumentModelChangeActions.None);
			return new StyleTablePropertiesDestination(importer, tableProperties);
		}
		static Destination OnTableRowProperties(WordProcessingMLBaseImporter importer, XmlReader reader) {
			TableRowProperties tableRowProperties = GetThis(importer).tableRowProperties;
			tableRowProperties.ReplaceInfo(importer.DocumentModel.Cache.TableRowPropertiesOptionsCache.DefaultItem, DocumentModelChangeActions.None);
			return new StyleTableRowPropertiesDestination(importer, tableRowProperties);
		}
		static Destination OnTableCellProperties(WordProcessingMLBaseImporter importer, XmlReader reader) {
			TableCellProperties tableCellProperties = GetThis(importer).tableCellProperties;
			tableCellProperties.ReplaceInfo(importer.DocumentModel.Cache.TableCellPropertiesOptionsCache.DefaultItem, DocumentModelChangeActions.None);
			return new StyleTableCellPropertiesDestination(importer, tableCellProperties);
		}
		#region ICellPropertiesOwner Members
		IndexChangedHistoryItemCore<DocumentModelChangeActions> ICellPropertiesOwner.CreateCellPropertiesChangedHistoryItem(TableCellProperties properties) {
			Debug.Assert(properties == tableCellProperties);
			return new IndexChangedHistoryItem<DocumentModelChangeActions>(properties.PieceTable, properties);
		}
		#endregion
	}
	#endregion
	#region StyleDestination
	public class StyleDestination : StyleDestinationBase {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			FillElementHandlerTable(result);
			result.Add("basedOn", OnParentStyleId);
			result.Add("hidden", OnHidden);
			result.Add("qFormat", OnQFormat);
			result.Add("semiHidden", OnSemiHidden);
			result.Add("name", OnName);
			result.Add("link", OnLinkedStyleId);
			result.Add("next", OnNextStyle);
			result.Add("tblStylePr", OnStyleConditionalTableFormatting);
			return result;
		}
		const string NormalTableStyleName = "Normal Table";
		string id;
		string styleType;
		string parentId;
		string linkedStyleId;
		string styleName;
		bool semiHidden;
		bool hidden;
		bool isDefaultStyle;
		bool qFormat;
		string nextStyleId;
		List<OpenXmlStyleConditionalTableFormattingInfo> conditionalTableFormattingInfoList;
		public StyleDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
			this.id = String.Empty;
			this.parentId = String.Empty;
			this.linkedStyleId = String.Empty;
			this.styleType = String.Empty;
			this.styleName = String.Empty;
			this.nextStyleId = String.Empty;
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public string ParentId { get { return parentId; } set { parentId = value; } }
		public string LinkedStyleId { get { return linkedStyleId; } set { linkedStyleId = value; } }
		public string StyleName { get { return styleName; } set { styleName = value; } }
		public bool SemiHidden { get { return semiHidden; } set { semiHidden = value; } }
		public bool Hidden { get { return hidden; } set { hidden = value; } }
		public bool IsDefaultStyle { get { return isDefaultStyle; } set { isDefaultStyle = value; } }
		public string NextStyleId { get { return nextStyleId; } set { nextStyleId = value; } }
		public bool QFormat { get { return qFormat; } set { qFormat = value; } }
		static StyleDestination GetThis(WordProcessingMLBaseImporter importer) {
			return (StyleDestination)importer.PeekDestination();
		}
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			this.id = reader.GetAttribute("styleId", Importer.WordProcessingNamespaceConst);
			this.styleType = reader.GetAttribute("type", Importer.WordProcessingNamespaceConst);
			this.isDefaultStyle = Importer.GetWpSTOnOffValue(reader, "default", false);
		}
		public override void ProcessElementClose(XmlReader reader) {
			base.ProcessElementClose(reader);			
			if (String.IsNullOrEmpty(id))
				return;
			if (String.IsNullOrEmpty(styleType))
				styleType = "paragraph";
			if (styleType == "paragraph")
				ImportParagraphStyle();
			else if (styleType == "character")
				ImportCharacterStyle();
			else if (styleType == "table")
				ImportTableStyle();
			else if (styleType == "tableCell")
				ImportTableCellStyle();
			else if (styleType == "numbering")
				ImportNumberingStyle();
		}
		static Destination OnParentStyleId(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new StyleParentIdDestination(importer, GetThis(importer));
		}
		static Destination OnHidden(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new StyleHiddenDestination(importer, GetThis(importer));
		}
		static Destination OnQFormat(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new StyleQFormatDestination(importer, GetThis(importer));
		}
		static Destination OnSemiHidden(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new StyleSemiHiddenDestination(importer, GetThis(importer));
		}
		static Destination OnName(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new StyleNameDestination(importer, GetThis(importer));
		}
		static Destination OnLinkedStyleId(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new LinkedStyleIdDestination(importer, GetThis(importer));
		}
		static Destination OnNextStyle(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new NextStyleDestination(importer, GetThis(importer));
		}
		static Destination OnStyleConditionalTableFormatting(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new StyleConditionalTableFormatting(importer, GetThis(importer));
		}		
		protected internal virtual void ImportParagraphStyle() {
			int styleIndex = ImportParagraphStyleCore();
			Importer.ParagraphStyleInfos.Add(ImportStyleCore(styleIndex));
		}
		protected internal virtual void ImportCharacterStyle() {
			int styleIndex = ImportCharacterStyleCore();
			Importer.CharacterStyleInfos.Add(ImportStyleCore(styleIndex));
		}
		protected internal virtual void ImportTableStyle() {
			int styleIndex = ImportTableStyleCore();
			Importer.TableStyleInfos.Add(ImportStyleCore(styleIndex));
		}
		protected internal virtual void ImportTableCellStyle() {
			int styleIndex = ImportTableCellStyleCore();
			Importer.TableCellStyleInfos.Add(ImportStyleCore(styleIndex));
		}
		protected internal virtual void ImportNumberingStyle() {
			int styleIndex = ImportNumberingStyleCore();
			Importer.NumberingStyleInfos.Add(ImportStyleCore(styleIndex));
		}
		protected internal virtual int ImportParagraphStyleCore() {
			if (IsDefaultStyle) {
				ApplyParagraphStyleProperties(DocumentModel.ParagraphStyles.DefaultItem);
				return 0;
			}
			else {
				int styleIndex = DocumentModel.ParagraphStyles.GetStyleIndexByName(StyleName);
				if (styleIndex >= 0) {
					ApplyParagraphStyleProperties(DocumentModel.ParagraphStyles[styleIndex]);
					return styleIndex;
				}
				else {
					ParagraphStyle style = CreateParagraphStyle();
					return DocumentModel.ParagraphStyles.Add(style);
				}
			}
		}
		protected internal virtual int ImportCharacterStyleCore() {
			if (IsDefaultStyle) {
				ApplyCharacterStyleProperties(DocumentModel.CharacterStyles.DefaultItem);
				return 0;
			}
			else {
				int styleIndex = DocumentModel.CharacterStyles.GetStyleIndexByName(StyleName);
				if (styleIndex >= 0) {
					ApplyCharacterStyleProperties(DocumentModel.CharacterStyles[styleIndex]);
					return styleIndex;
				}
				else {
					CharacterStyle style = CreateCharacterStyle();
					return DocumentModel.CharacterStyles.Add(style);
				}
			}
		}
		protected internal virtual int ImportTableStyleCore() {
			if (IsDefaultStyle) {
				ApplyTableStyleProperties(DocumentModel.TableStyles.DefaultItem);
				return 0;
			}
			else {
				int styleIndex = DocumentModel.TableStyles.GetStyleIndexByName(styleName);
				if (styleIndex >= 0) {
					ApplyTableStyleProperties(DocumentModel.TableStyles[styleIndex]);
					return styleIndex;
				}
				else {
					TableStyle style = CreateTableStyle();
					return DocumentModel.TableStyles.Add(style);
				}
			}
		}
		protected internal virtual int ImportTableCellStyleCore() {
			if (IsDefaultStyle) {
				ApplyTableCellStyleProperties(DocumentModel.TableCellStyles.DefaultItem);
				return 0;
			}
			else {
				int styleIndex = DocumentModel.TableCellStyles.GetStyleIndexByName(styleName);
				if (styleIndex >= 0) {
					ApplyTableCellStyleProperties(DocumentModel.TableCellStyles[styleIndex]);
					return styleIndex;
				}
				else {
					TableCellStyle style = CreateTableCellStyle();
					return DocumentModel.TableCellStyles.Add(style);
				}
			}
		}
		protected internal virtual int ImportNumberingStyleCore() {
			int styleIndex = DocumentModel.NumberingListStyles.GetStyleIndexByName(StyleName);
			if (styleIndex >= 0)
				return styleIndex;
			NumberingListStyle style = CreateNumberingListStyle();
			return DocumentModel.NumberingListStyles.Add(style);
		}
		protected internal virtual OpenXmlStyleInfo ImportStyleCore(int styleIndex) {
			OpenXmlStyleInfo styleInfo = new OpenXmlStyleInfo();
			styleInfo.StyleId = id;
			styleInfo.ParentStyleId = parentId;
			styleInfo.LinkedStyleId = linkedStyleId;
			styleInfo.NextStyleId = nextStyleId;
			styleInfo.StyleIndex = styleIndex;
			styleInfo.NumberingId = NumberingId;
			styleInfo.ListLevelIndex = ListLevelIndex;
			return styleInfo;
		}
		protected internal virtual ParagraphStyle CreateParagraphStyle() {
			ParagraphStyle style = new ParagraphStyle(DocumentModel);
			ApplyParagraphStyleProperties(style);
			return style;
		}
		protected internal virtual void ApplyParagraphStyleProperties(ParagraphStyle style) {
			style.StyleName = styleName;
			style.SemihiddenCore = semiHidden;
			style.HiddenCore = hidden;
			style.Primary = qFormat;
			style.CharacterProperties.CopyFrom(CharacterFormatting);
			style.ParagraphProperties.CopyFrom(ParagraphFormatting);
			if (Importer.Position.ParagraphFrameFormatting.Options.GetHashCode() != 0) {
				if (style.FrameProperties == null)
					style.FrameProperties = new FrameProperties(style);
				style.FrameProperties.CopyFrom(Importer.Position.ParagraphFrameFormatting);
				Importer.Position.ParagraphFrameFormatting.ReplaceInfo(Importer.DocumentModel.Cache.ParagraphFrameFormattingInfoCache.DefaultItem, new ParagraphFrameFormattingOptions(ParagraphFrameFormattingOptions.Mask.UseNone));
			}
			style.Tabs.SetTabs(Tabs);
			if (new NumberingListIndex(NumberingId) == NumberingListIndex.NoNumberingList)
				style.SetNumberingListIndex(NumberingListIndex.NoNumberingList);
		}
		protected internal virtual CharacterStyle CreateCharacterStyle() {
			CharacterStyle style = new CharacterStyle(DocumentModel);
			ApplyCharacterStyleProperties(style);
			return style;
		}
		protected internal virtual void ApplyCharacterStyleProperties(CharacterStyle style) {
			style.StyleName = styleName;
			style.SemihiddenCore = semiHidden;			
			style.HiddenCore = hidden;
			style.Primary = qFormat;
			style.CharacterProperties.CopyFrom(CharacterFormatting);
		}
		protected internal virtual TableStyle CreateTableStyle() {
			TableStyle style = new TableStyle(DocumentModel);
			ApplyTableStyleProperties(style);
			return style;
		}
		protected internal virtual TableCellStyle CreateTableCellStyle() {
			TableCellStyle style = new TableCellStyle(DocumentModel);
			ApplyTableCellStyleProperties(style);
			return style;
		}
		protected internal virtual NumberingListStyle CreateNumberingListStyle() {
			return new NumberingListStyle(DocumentModel, styleName);
		}
		protected internal virtual void ApplyTableStyleProperties(TableStyle style) {
			style.StyleName = styleName;
			style.SemihiddenCore = semiHidden;
			style.HiddenCore = hidden;
			style.Primary = qFormat;
			if (!String.IsNullOrEmpty(ParentId)) {
				int index = Importer.LookupTableStyleIndex(ParentId);
				TableStyle parentStyle = GetParentTableStyle(index);
				if(parentStyle != null)
					style.Parent = parentStyle;
			}
			bool isNormalTableStyle = String.Equals(style.LocalizedStyleName, NormalTableStyleName);
			if((isNormalTableStyle
					&& (TableProperties.CellMargins.UseLeftMargin
						|| TableProperties.CellMargins.UseRightMargin
						|| TableProperties.CellMargins.UseBottomMargin
						|| TableProperties.CellMargins.UseTopMargin))
				|| !isNormalTableStyle)
				style.TableProperties.CopyFrom(TableProperties);
			style.TableRowProperties.CopyFrom(TableRowProperties);
			style.TableCellProperties.CopyFrom(TableCellProperties);
			style.CharacterProperties.CopyFrom(CharacterFormatting);
			style.ParagraphProperties.CopyFrom(ParagraphFormatting);
			style.Tabs.SetTabs(Tabs);
			if (conditionalTableFormattingInfoList != null) {
				int count = conditionalTableFormattingInfoList.Count;
				for(int i = 0; i < count; i++)
					ApplyConditionalTableFormattingInfo(style, conditionalTableFormattingInfoList[i]);
			}
		}
		protected internal virtual TableStyle GetParentTableStyle(int index) {
			if (index >= 0 && index < DocumentModel.TableStyles.Count)
				return DocumentModel.TableStyles[index];
			return null;
		}
		protected internal virtual void ApplyTableCellStyleProperties(TableCellStyle style) {
			style.StyleName = styleName;
			style.SemihiddenCore = semiHidden;
			style.HiddenCore = hidden;
			style.Primary = qFormat;
			if (!String.IsNullOrEmpty(ParentId)) {
				int index = Importer.LookupTableCellStyleIndex(ParentId);
				TableCellStyle parentStyle = GetParentTableCellStyle(index);
				if(parentStyle != null)
					style.Parent = parentStyle;
			}
			style.TableCellProperties.CopyFrom(TableCellProperties);
			style.CharacterProperties.CopyFrom(CharacterFormatting);
			style.ParagraphProperties.CopyFrom(ParagraphFormatting);
			style.Tabs.SetTabs(Tabs);
		}
		protected internal virtual TableCellStyle GetParentTableCellStyle(int index) {
			if (index >= 0 && index < DocumentModel.TableCellStyles.Count)
				return DocumentModel.TableCellStyles[index];
			return null;
		}
		protected virtual void ApplyConditionalTableFormattingInfo(TableStyle style, OpenXmlStyleConditionalTableFormattingInfo info) {
			TableConditionalStyle conditionalStyle = style.ConditionalStyleProperties.GetStyleSafe(info.ConditionType);
			conditionalStyle.TableCellProperties.CopyFrom(info.TableCellProperties);
			conditionalStyle.TableRowProperties.CopyFrom(info.TableRowProperties);
			conditionalStyle.TableProperties.CopyFrom(info.TableProperties);
			conditionalStyle.CharacterProperties.CopyFrom(info.CharacterFormatting);
			conditionalStyle.ParagraphProperties.CopyFrom(info.ParagraphFormatting);
			conditionalStyle.Tabs.CopyFrom(info.Tabs);
		}
		protected internal virtual void AddStyleConditionalTableFormatting(OpenXmlStyleConditionalTableFormattingInfo info) {
			if (conditionalTableFormattingInfoList == null)
				conditionalTableFormattingInfoList = new List<OpenXmlStyleConditionalTableFormattingInfo>();
			conditionalTableFormattingInfoList.Add(info);
		}
	}
	#endregion
	#region DocumentDefaultsDestination
	public class DocumentDefaultsDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("pPrDefault", OnDefaultParagraphProperties);
			result.Add("rPrDefault", OnDefaultRunProperties);
			return result;
		}
		public DocumentDefaultsDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static Destination OnDefaultParagraphProperties(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new DefaultParagraphPropertiesDestination(importer);
		}
		static Destination OnDefaultRunProperties(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new DefaultRunPropertiesDestination(importer);
		}
	}
	#endregion
	#region DefaultRunPropertiesDestination
	public class DefaultRunPropertiesDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("rPr", OnRunProperties);
			return result;
		}
		public DefaultRunPropertiesDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			CharacterProperties properties = DocumentModel.DefaultCharacterProperties;
			properties.BeginUpdate();
			try {
				properties.FontName = "Times New Roman";
				properties.DoubleFontSize = 20;
			}
			finally {
				properties.EndUpdate();
			}
		}
		static Destination OnRunProperties(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new InnerDefaultRunPropertiesDestination(importer, importer.DocumentModel.DefaultCharacterProperties);
		}
	}
	#endregion
	#region InnerDefaultRunPropertiesDestination
	public class InnerDefaultRunPropertiesDestination : RunPropertiesBaseDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static new ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = RunPropertiesBaseDestination.CreateElementHandlerTable();
			result.Remove("rFonts");
			result.Add("rFonts", OnFontName);
			return result;
		}
		public InnerDefaultRunPropertiesDestination(WordProcessingMLBaseImporter importer, CharacterProperties characterProperties)
			: base(importer, characterProperties) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
			CharacterProperties characterFormatting = (CharacterProperties)CharacterProperties;
			characterFormatting.BeginUpdate();
		}
		public override void ProcessElementClose(XmlReader reader) {
			CharacterProperties characterFormatting = (CharacterProperties)CharacterProperties;
			characterFormatting.EndUpdate();
		}
		static ICharacterProperties GetCharacterProperties(WordProcessingMLBaseImporter importer) {
			RunPropertiesBaseDestination thisObject = (RunPropertiesBaseDestination)importer.PeekDestination();
			return thisObject.CharacterProperties;
		}
		static Destination OnFontName(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new DefaultFontNameDestination(importer, GetCharacterProperties(importer));
		}
	}
	#endregion
	#region DefaultParagraphPropertiesDestination
	public class DefaultParagraphPropertiesDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("pPr", OnParagraphProperties);
			return result;
		}
		public DefaultParagraphPropertiesDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static Destination OnParagraphProperties(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new InnerDefaultParagraphPropertiesDestination(importer, importer.DocumentModel.DefaultParagraphProperties, new TabFormattingInfo());
		}
	}
	#endregion
	#region InnerDefaultParagraphPropertiesDestination
	public class InnerDefaultParagraphPropertiesDestination : ParagraphPropertiesBaseDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static new ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = ParagraphPropertiesBaseDestination.CreateElementHandlerTable();
			return result;
		}
		public InnerDefaultParagraphPropertiesDestination(WordProcessingMLBaseImporter importer, ParagraphProperties paragraphProperties, TabFormattingInfo tabs)
			: base(importer, paragraphProperties, tabs) {
			tabs.Clear();
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public override int NumberingId { get { return -1; } set { } }
		public override int ListLevelIndex { get { return -1; } set { } }
		public override void ProcessElementOpen(XmlReader reader) {
			ParagraphProperties paragraphProperties = (ParagraphProperties)ParagraphFormatting;
			paragraphProperties.BeginUpdate();
		}
		public override void ProcessElementClose(XmlReader reader) {
			ParagraphProperties paragraphProperties = (ParagraphProperties)ParagraphFormatting;
			paragraphProperties.EndUpdate();
		}
	}
	#endregion
	#region StyleParagraphPropertiesDestination
	public class StyleParagraphPropertiesDestination : ParagraphPropertiesBaseDestination {
		readonly StyleDestinationBase styleDestination;
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static new ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = ParagraphPropertiesBaseDestination.CreateElementHandlerTable();
			result.Add("numPr", OnNumbering);
			return result;
		}
		public StyleParagraphPropertiesDestination(WordProcessingMLBaseImporter importer, StyleDestinationBase styleDestination, ParagraphFormattingBase paragraphFormatting, TabFormattingInfo tabs)
			: base(importer, paragraphFormatting, tabs) {
			Guard.ArgumentNotNull(styleDestination, "styleDestination");
			this.styleDestination = styleDestination;
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public override int NumberingId { get { return styleDestination.NumberingId; } set { styleDestination.NumberingId = value; } }
		public override int ListLevelIndex { get { return styleDestination.ListLevelIndex; } set { styleDestination.ListLevelIndex = value; } }
		static StyleParagraphPropertiesDestination GetThis(WordProcessingMLBaseImporter importer) {
			return (StyleParagraphPropertiesDestination)importer.PeekDestination();
		}
		public override void ProcessElementOpen(XmlReader reader) {
			ParagraphFormattingBase paragraphFormatting = (ParagraphFormattingBase)ParagraphFormatting;
			paragraphFormatting.BeginUpdate();
		}
		public override void ProcessElementClose(XmlReader reader) {
			ParagraphFormattingBase paragraphFormatting = (ParagraphFormattingBase)ParagraphFormatting;
			paragraphFormatting.EndUpdate();
		}
		static Destination OnNumbering(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new ParagraphNumberingReferenceDestination(importer, GetThis(importer));
		}
	}
	#endregion
	#region StyleRunPropertiesDestination
	public class StyleRunPropertiesDestination : RunPropertiesBaseDestination {
		public StyleRunPropertiesDestination(WordProcessingMLBaseImporter importer, CharacterFormattingBase characterFormatting)
			: base(importer, characterFormatting) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			CharacterFormattingBase characterFormatting = (CharacterFormattingBase)CharacterProperties;
			characterFormatting.BeginUpdate();
		}
		public override void ProcessElementClose(XmlReader reader) {
			CharacterFormattingBase characterFormatting = (CharacterFormattingBase)CharacterProperties;
			characterFormatting.EndUpdate();
		}
	}
	#endregion
	#region StyleTablePropertiesDestination
	public class StyleTablePropertiesDestination : TablePropertiesDestinationCore {
		public StyleTablePropertiesDestination(WordProcessingMLBaseImporter importer, TableProperties tableProperties)
			: base(importer, tableProperties) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Properties.BeginUpdate();
		}
		public override void ProcessElementClose(XmlReader reader) {
			Properties.EndUpdate();
		}
	}
	#endregion
	#region StyleTableRowPropertiesDestination
	public class StyleTableRowPropertiesDestination : TableRowPropertiesDestination {
		public StyleTableRowPropertiesDestination(WordProcessingMLBaseImporter importer, TableRowProperties tableRowProperties)
			: base(importer, tableRowProperties) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Properties.BeginUpdate();
		}
		public override void ProcessElementClose(XmlReader reader) {
			Properties.EndUpdate();
		}
	}
	#endregion
	#region StyleTableCellPropertiesDestination
	public class StyleTableCellPropertiesDestination : TableCellPropertiesDestinationCore {
		public StyleTableCellPropertiesDestination(WordProcessingMLBaseImporter importer, TableCellProperties tableCellProperties)
			: base(importer, tableCellProperties) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Properties.BeginUpdate();
		}
		public override void ProcessElementClose(XmlReader reader) {
			Properties.EndUpdate();
		}
	}
	#endregion
	#region StyleLeafElementDestination (abstract class)
	public abstract class StyleLeafElementDestination : LeafElementDestination {
		readonly StyleDestination styleDestination;
		protected StyleLeafElementDestination(WordProcessingMLBaseImporter importer, StyleDestination styleDestination)
			: base(importer) {
			Guard.ArgumentNotNull(styleDestination, "styleDestination");
			this.styleDestination = styleDestination;
		}
		public StyleDestination StyleDestination { get { return styleDestination; } }
	}
	#endregion
	#region StyleParentIdDestination
	public class StyleParentIdDestination : StyleLeafElementDestination {
		public StyleParentIdDestination(WordProcessingMLBaseImporter importer, StyleDestination styleDestination)
			: base(importer, styleDestination) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			StyleDestination.ParentId = reader.GetAttribute("val", Importer.WordProcessingNamespaceConst);
		}
	}
	#endregion
	#region StyleHiddenDestination
	public class StyleHiddenDestination : StyleLeafElementDestination {
		public StyleHiddenDestination(WordProcessingMLBaseImporter importer, StyleDestination styleDestination)
			: base(importer, styleDestination) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			StyleDestination.Hidden = Importer.GetWpSTOnOffValue(reader, "val");
		}
	}
	#endregion
	#region StyleSemiHiddenDestination
	public class StyleSemiHiddenDestination : StyleLeafElementDestination {
		public StyleSemiHiddenDestination(WordProcessingMLBaseImporter importer, StyleDestination styleDestination)
			: base(importer, styleDestination) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			StyleDestination.SemiHidden = Importer.GetWpSTOnOffValue(reader, "val");
		}
	}
	#endregion
	#region StyleNameDestination
	public class StyleNameDestination : StyleLeafElementDestination {
		public StyleNameDestination(WordProcessingMLBaseImporter importer, StyleDestination styleDestination)
			: base(importer, styleDestination) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			StyleDestination.StyleName = reader.GetAttribute("val", Importer.WordProcessingNamespaceConst);
		}
	}
	#endregion
	#region StyleQFormatDestination
	public class StyleQFormatDestination : StyleLeafElementDestination {
		public StyleQFormatDestination(WordProcessingMLBaseImporter importer, StyleDestination styleDestination)
			: base(importer, styleDestination) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			StyleDestination.QFormat = Importer.GetWpSTOnOffValue(reader, "val", true);
		}
	}
	#endregion
	#region LinkedStyleIdDestination
	public class LinkedStyleIdDestination : StyleLeafElementDestination {
		public LinkedStyleIdDestination(WordProcessingMLBaseImporter importer, StyleDestination styleDestination)
			: base(importer, styleDestination) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			StyleDestination.LinkedStyleId = reader.GetAttribute("val", Importer.WordProcessingNamespaceConst);
		}
	}
	#endregion   
	#region NextStyleDestination
	public class NextStyleDestination : StyleLeafElementDestination {
		public NextStyleDestination(WordProcessingMLBaseImporter importer, StyleDestination styleDestination)
			: base(importer, styleDestination) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			StyleDestination.NextStyleId = reader.GetAttribute("val", Importer.WordProcessingNamespaceConst);
		}
	}
	#endregion
	#region StyleConditionalTableFormatting
	public class StyleConditionalTableFormatting : StyleDestinationBase {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static readonly Dictionary<string, ConditionalTableStyleFormattingTypes> condtionTypesTable = CreateCondtionTypesTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			FillElementHandlerTable(result);		   
			return result;
		}
		static Dictionary<string, ConditionalTableStyleFormattingTypes> CreateCondtionTypesTable() {
			Dictionary<string, ConditionalTableStyleFormattingTypes> result = new Dictionary<string, ConditionalTableStyleFormattingTypes>();
			result.Add("band1Horz", ConditionalTableStyleFormattingTypes.OddRowBanding);
			result.Add("band1Vert", ConditionalTableStyleFormattingTypes.OddColumnBanding);
			result.Add("band2Horz", ConditionalTableStyleFormattingTypes.EvenRowBanding);
			result.Add("band2Vert", ConditionalTableStyleFormattingTypes.EvenColumnBanding);
			result.Add("firstCol", ConditionalTableStyleFormattingTypes.FirstColumn);
			result.Add("firstRow", ConditionalTableStyleFormattingTypes.FirstRow);
			result.Add("lastCol", ConditionalTableStyleFormattingTypes.LastColumn);
			result.Add("lastRow", ConditionalTableStyleFormattingTypes.LastRow);
			result.Add("neCell", ConditionalTableStyleFormattingTypes.TopRightCell);
			result.Add("nwCell", ConditionalTableStyleFormattingTypes.TopLeftCell);
			result.Add("seCell", ConditionalTableStyleFormattingTypes.BottomRightCell);
			result.Add("swCell", ConditionalTableStyleFormattingTypes.BottomLeftCell);
			result.Add("wholeTable", ConditionalTableStyleFormattingTypes.WholeTable);
			return result;
		}
		ConditionalTableStyleFormattingTypes conditionType = ConditionalTableStyleFormattingTypes.WholeTable;
		StyleDestination styleDestination;
		public StyleConditionalTableFormatting(WordProcessingMLBaseImporter importer, StyleDestination styleDestination)
			: base(importer) {
			Guard.ArgumentNotNull(styleDestination, "styleDestination");
			this.styleDestination = styleDestination;
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			string value = reader.GetAttribute("type", Importer.WordProcessingNamespaceConst);
			if (!String.IsNullOrEmpty(value))
				this.conditionType = Importer.GetWpEnumValueCore(value, OpenXmlExporter.conditionalTableStyleFormattingTypesTable, ConditionalTableStyleFormattingTypes.WholeTable);
			else
				this.conditionType = ConditionalTableStyleFormattingTypes.WholeTable;
		}
		public override void ProcessElementClose(XmlReader reader) {
			base.ProcessElementClose(reader);
			OpenXmlStyleConditionalTableFormattingInfo result = new OpenXmlStyleConditionalTableFormattingInfo();
			result.ConditionType = conditionType;
			result.CharacterFormatting = CharacterFormatting;
			result.ParagraphFormatting = ParagraphFormatting;
			result.TableProperties = TableProperties;
			result.TableCellProperties = TableCellProperties;
			result.TableRowProperties = TableRowProperties;
			result.Tabs = Tabs;
			styleDestination.AddStyleConditionalTableFormatting(result);
		}
	}
	#endregion
	#region OpenXmlStyleInfo
	public class OpenXmlStyleInfo {
		#region Fields
		string styleId;
		string parentStyleId;
		int styleIndex;
		string linkedStyleId;
		string nextStyleId;
		int numberingId = -1;
		int listLevelIndex;
		#endregion
		public OpenXmlStyleInfo() {
		}
		#region Properties
		public string StyleId { get { return styleId; } set { styleId = value; } }
		public int StyleIndex { get { return styleIndex; } set { styleIndex = value; } }
		public string LinkedStyleId { get { return linkedStyleId; } set { linkedStyleId = value; } }
		public string NextStyleId { get { return nextStyleId; } set { nextStyleId = value; } }
		public string ParentStyleId { get { return parentStyleId; } set { parentStyleId = value; } }
		public int NumberingId { get { return numberingId; } set { numberingId = value; } }
		public int ListLevelIndex { get { return listLevelIndex; } set { listLevelIndex = value; } }
		#endregion
	}
	#endregion
	public class OpenXmlStyleConditionalTableFormattingInfo {
		public ConditionalTableStyleFormattingTypes ConditionType { get; set; }
		public CharacterFormattingBase CharacterFormatting { get; set; }
		public ParagraphFormattingBase ParagraphFormatting { get; set; }
		public TabFormattingInfo Tabs { get; set; }
		public TableProperties TableProperties { get; set; }
		public TableRowProperties TableRowProperties { get; set; }
		public TableCellProperties TableCellProperties { get; set; }
	}
	#region OpenXmlStyleInfoCollection
	public class OpenXmlStyleInfoCollection : List<OpenXmlStyleInfo> {
		public OpenXmlStyleInfo LookupStyleById(string id) {
			if (String.IsNullOrEmpty(id))
				return null;
			int count = Count;
			for (int i = 0; i < count; i++)
				if (this[i].StyleId == id)
					return this[i];
			return null;
		}
	}
	#endregion
}
