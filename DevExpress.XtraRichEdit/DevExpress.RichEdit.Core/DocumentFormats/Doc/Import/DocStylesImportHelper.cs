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
using System.Text;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
namespace DevExpress.XtraRichEdit.Import.Doc {
	public class DocStylesImportHelper {
		#region Fields
		const int defaultFontSize = 10;
		string defaultStyleName;
		DocContentBuilder contentBuilder;
		DocumentModel documentModel;
		Dictionary<int, string> styleMapping;
		#endregion
		public DocStylesImportHelper(DocContentBuilder contentBuilder, DocumentModel documentModel) {
			this.contentBuilder = contentBuilder;
			this.documentModel = documentModel;
			this.styleMapping = new Dictionary<int, string>();
		}
		#region Properties
		protected DocContentBuilder ContentBuilder { get { return contentBuilder; } }
		protected DocumentModel DocumentModel { get { return documentModel; } }
		protected Dictionary<int, string> StyleMapping { get { return styleMapping; } }
		protected String DefaultParagraphStyleName { get { return defaultStyleName; } }
		#endregion
		public int GetStyleIndex(int docStyleIndex, StyleType styleType) {
			return Math.Max(0, GetStyleIndexCore(docStyleIndex, styleType));
		}
		public int GetNumberingStyleIndex(int docStyleIndex) {
			return GetStyleIndexCore(docStyleIndex, StyleType.NumberingListStyle);
		}
		int GetStyleIndexCore(int docStyleIndex, StyleType styleType) {
			string styleName;
			if (!StyleMapping.TryGetValue(docStyleIndex, out styleName))
				return styleType != StyleType.NumberingListStyle ? 0 : -1;
			switch (styleType) {
				case StyleType.ParagraphStyle: return DocumentModel.ParagraphStyles.GetStyleIndexByName(styleName);
				case StyleType.CharacterStyle: return DocumentModel.CharacterStyles.GetStyleIndexByName(styleName);
				case StyleType.TableStyle: return DocumentModel.TableStyles.GetStyleIndexByName(styleName);
				case StyleType.NumberingListStyle: return DocumentModel.NumberingListStyles.GetStyleIndexByName(styleName);
				default: return 0;
			}
		}
		public void SetDocunentDefaults() {
			CharacterProperties characterProperties = DocumentModel.DefaultCharacterProperties;
			characterProperties.BeginUpdate();
			characterProperties.FontName = ContentBuilder.FontManager.GetFontName(ContentBuilder.StyleSheet.StylesInformation.DefaultASCIIFont);
			characterProperties.EndUpdate();
		}
		public void InitializeStyles() {
			StyleDescriptionCollection styles = ContentBuilder.StyleSheet.Styles;
			GenerateStyleMapping();
			DeleteStyleLinks();
			IList<StyleDescriptionBase> sortedStyles = Algorithms.TopologicalSort<StyleDescriptionBase>(styles, new StyleDescriptionTopologicalComparer());
			int count = sortedStyles.Count;
			for (int i = 0; i < count; i++) {
				StyleDescriptionBase style = sortedStyles[i];
				if (style != null)
					RegisterStyle(style);
			}
			UpdateDefaultFontSize();
			SetNextParagraphStyles();
			CreateStyleLinks();
		}
		void GenerateStyleMapping() {
			this.styleMapping = new Dictionary<int, string>();
			StyleDescriptionCollection styles = ContentBuilder.StyleSheet.Styles;
			if (styles.Count > 0 && styles[0] is ParagraphStyleDescription)
				defaultStyleName = styles[0].StyleName;
			for (int i = 0; i < styles.Count; i++) {
				if (styles[i] != null)
					StyleMapping.Add(styles[i].StyleIndex, styles[i].StyleName);
			}
		}
		void DeleteStyleLinks() {
			int count = DocumentModel.CharacterStyles.Count;
			for (int i = 0; i < count; i++) {
				if (DocumentModel.CharacterStyles[i].HasLinkedStyle)
					DocumentModel.StyleLinkManager.DeleteLink(DocumentModel.CharacterStyles[i]);
			}
		}
		void RegisterStyle(StyleDescriptionBase styleDescription) {
			switch (styleDescription.StyleType) {
				case StyleType.ParagraphStyle:
					AddParagraphStyle((ParagraphStyleDescription)styleDescription);
					break;
				case StyleType.CharacterStyle:
					AddCharacterStyle((CharacterStyleDescription)styleDescription);
					break;
				case StyleType.TableStyle:
					AddTableStyle((TableStyleDescription)styleDescription);
					break;
				case StyleType.NumberingListStyle:
					AddListStyle((ListStyleDescription)styleDescription);
					break;
			}
		}
		void AddParagraphStyle(ParagraphStyleDescription styleDescription) {
			ParagraphStyle paragraphStyle = DocumentModel.ParagraphStyles.GetStyleByName(styleDescription.StyleName);
			if (paragraphStyle == null) {
				paragraphStyle = new ParagraphStyle(DocumentModel);
				paragraphStyle.StyleName = styleDescription.StyleName;
				paragraphStyle.HiddenCore = styleDescription.Hidden;
				DocumentModel.ParagraphStyles.Add(paragraphStyle);
			}
			if (styleDescription.BaseStyleIndex != StyleDescriptionBase.EmptyStyleIdentifier) {
				ParagraphStyle parent = DocumentModel.ParagraphStyles.GetStyleByName(StyleMapping[styleDescription.BaseStyleIndex]);
				paragraphStyle.Parent = parent;
			}
			paragraphStyle.Primary = styleDescription.QFormat;
			SetParagraphStyleProperties(paragraphStyle, styleDescription);
		}
		void SetParagraphStyleProperties(ParagraphStyle style, ParagraphStyleDescription description) {
			DocPropertyContainer container = DocCommandHelper.Traverse(description.ParagraphUPX, ContentBuilder.Factory, ContentBuilder.DataReader);
			DocCommandHelper.Traverse(description.CharacterUPX, container, ContentBuilder.DataReader);
			SetParagraphStyleOutlineLevel(description, container);
			ContentBuilder.FontManager.SetFontName(container);
			CharacterFormattingInfo characterFormattingInfo = GetParagraphStyleFormattingInfo(description.BaseStyleIndex, container);
			if (container.CharacterInfo != null)
				style.CharacterProperties.CopyFrom(new MergedCharacterProperties(characterFormattingInfo, container.CharacterInfo.FormattingOptions));
			ParagraphInfo paragraphInfo = container.ParagraphInfo;
			if (paragraphInfo != null) {
				style.ParagraphProperties.CopyFrom(new MergedParagraphProperties(paragraphInfo.FormattingInfo, paragraphInfo.FormattingOptions));
				style.Tabs.SetTabs(paragraphInfo.Tabs);
				ProcessParagraphListInfoIndex(style, paragraphInfo);
			}
		}
		void SetParagraphStyleOutlineLevel(ParagraphStyleDescription description, DocPropertyContainer propertyContainer) {
			ParagraphInfo info = propertyContainer.ParagraphInfo;
			if (info != null && description.StyleIndex >= 1 && description.StyleIndex <= 9) {
				info.FormattingInfo.OutlineLevel = description.StyleIndex;
				info.FormattingOptions.UseOutlineLevel = true;
			}
		}
		void ProcessParagraphListInfoIndex(ParagraphStyle style, ParagraphInfo info) {
			int listInfoIndex = info.ListInfoIndex - 1; 
			if (listInfoIndex < -1 || listInfoIndex >= DocumentModel.NumberingLists.Count)
				return;
			if (listInfoIndex == -1)
				style.SetNumberingListIndex(NumberingListIndex.NoNumberingList);
			else
				style.SetNumberingListIndex(new NumberingListIndex(listInfoIndex));
			style.SetNumberingListLevelIndex(info.ListLevel);
		}
		void AddCharacterStyle(CharacterStyleDescription styleDescription) {
			CharacterStyle characterStyle = DocumentModel.CharacterStyles.GetStyleByName(styleDescription.StyleName);
			if (characterStyle == null) {
				characterStyle = new CharacterStyle(DocumentModel);
				characterStyle.StyleName = styleDescription.StyleName;
				characterStyle.HiddenCore = styleDescription.Hidden;				
				DocumentModel.CharacterStyles.Add(characterStyle);
			}
			if (styleDescription.BaseStyleIndex != StyleDescriptionBase.EmptyStyleIdentifier) {
				CharacterStyle parent = DocumentModel.CharacterStyles.GetStyleByName(StyleMapping[styleDescription.BaseStyleIndex]);
				characterStyle.Parent = parent;
			}
			characterStyle.Primary = styleDescription.QFormat;
			SetCharacterStyleProperties(characterStyle, styleDescription);
		}
		void SetCharacterStyleProperties(CharacterStyle style, CharacterStyleDescription description) {
			DocPropertyContainer container = DocCommandHelper.Traverse(description.CharacterUPX, ContentBuilder.Factory, ContentBuilder.DataReader);
			ContentBuilder.FontManager.SetFontName(container);
			CharacterFormattingInfo formattingInfo = GetCharacterStyleFormattingInfo(description.BaseStyleIndex, container);
			if (container.CharacterInfo != null)
				style.CharacterProperties.CopyFrom(new MergedCharacterProperties(formattingInfo, container.CharacterInfo.FormattingOptions));
		}
		void AddTableStyle(TableStyleDescription styleDescription) {
			TableStyle tableStyle = DocumentModel.TableStyles.GetStyleByName(styleDescription.StyleName);
			if (tableStyle == null) {
				tableStyle = new TableStyle(DocumentModel);
				tableStyle.StyleName = styleDescription.StyleName;
				tableStyle.HiddenCore = styleDescription.Hidden;
				DocumentModel.TableStyles.Add(tableStyle);
			}
			if (styleDescription.BaseStyleIndex != StyleDescriptionBase.EmptyStyleIdentifier) {
				TableStyle parent = DocumentModel.TableStyles.GetStyleByName(StyleMapping[styleDescription.BaseStyleIndex]);
				tableStyle.Parent = parent;
			}
			tableStyle.Primary = styleDescription.QFormat;
			SetTableStyleProperties(tableStyle, styleDescription);
		}
		void SetTableStyleProperties(TableStyle style, TableStyleDescription description) {
			DocPropertyContainer container = DocCommandHelper.Traverse(description.CharacterUPX, ContentBuilder.Factory, ContentBuilder.DataReader);
			DocCommandHelper.Traverse(description.ParagraphUPX, container, ContentBuilder.DataReader);
			DocCommandHelper.Traverse(description.TableUPX, container, ContentBuilder.DataReader);
			ContentBuilder.FontManager.SetFontName(container);
			CharacterFormattingInfo characterFormattingInfo = GetTableStyleFormattingInfo(description.BaseStyleIndex, container);
			if (container.CharacterInfo != null)
				style.CharacterProperties.CopyFrom(new MergedCharacterProperties(characterFormattingInfo, container.CharacterInfo.FormattingOptions));
			ParagraphInfo paragraphInfo = container.ParagraphInfo;
			if (paragraphInfo != null) {
				style.ParagraphProperties.CopyFrom(new MergedParagraphProperties(paragraphInfo.FormattingInfo, paragraphInfo.FormattingOptions));
				style.Tabs.SetTabs(paragraphInfo.Tabs);
			}
			if (container.TableInfo != null)
				style.TableProperties.CopyFrom(container.TableInfo.TableProperties);
			if (container.TableRowInfo != null)
				style.TableRowProperties.CopyFrom(container.TableRowInfo.TableRowProperties);
			if (container.TableCellInfo != null)
				style.TableCellProperties.CopyFrom(container.TableCellInfo.TableCellProperties);
		}
		void AddListStyle(ListStyleDescription styleDescription) {
			NumberingListStyle listStyle = DocumentModel.NumberingListStyles.GetStyleByName(styleDescription.StyleName);
			DocPropertyContainer container = DocCommandHelper.Traverse(styleDescription.ParagraphUPX, ContentBuilder.Factory, ContentBuilder.DataReader);
			DocCommandHelper.Traverse(styleDescription.ParagraphUPX, container, ContentBuilder.DataReader);
			if (container.ParagraphInfo == null)
				return;
			int index = container.ParagraphInfo.ListInfoIndex;
			if (index <= 0)
				return;
			index--;
			if (index > DocumentModel.NumberingLists.Count - 1)
				return;
			if (listStyle == null) {
				listStyle = new NumberingListStyle(documentModel, styleDescription.StyleName);
				listStyle.HiddenCore = styleDescription.Hidden;
				DocumentModel.NumberingListStyles.Add(listStyle);
			}
			listStyle.SetNumberingListIndex(new NumberingListIndex(index));
			listStyle.Primary = styleDescription.QFormat;			
		}
		void SetNextParagraphStyles() {
			StyleDescriptionCollection styles = ContentBuilder.StyleSheet.Styles;
			int count = styles.Count;
			for (int i = 0; i < count; i++) {
				StyleDescriptionBase style = styles[i];
				if (style is ParagraphStyleDescription && style.NextStyleIndex != StyleDescriptionBase.EmptyStyleIdentifier) {
					ParagraphStyle currentStyle = DocumentModel.ParagraphStyles.GetStyleByName(style.StyleName);
					string nextStyleName;
					if (StyleMapping.TryGetValue(style.NextStyleIndex, out nextStyleName)) {
						ParagraphStyle nextStyle = DocumentModel.ParagraphStyles.GetStyleByName(nextStyleName);
						currentStyle.NextParagraphStyle = nextStyle;
					}
				}
			}
		}
		void CreateStyleLinks() {
			StyleDescriptionCollection styles = ContentBuilder.StyleSheet.Styles;
			int count = styles.Count;
			for (int i = 0; i < count; i++) {
				ParagraphStyle paragraphStyle = null;
				CharacterStyle characterStyle = null;
				StyleDescriptionBase current = styles[i];
				if (current is ParagraphStyleDescription && current.LinkedStyleIndex != 0) {
					paragraphStyle = DocumentModel.ParagraphStyles.GetStyleByName(current.StyleName);
					string characterStyleName;
					if (StyleMapping.TryGetValue(current.LinkedStyleIndex, out characterStyleName))
						characterStyle = DocumentModel.CharacterStyles.GetStyleByName(characterStyleName);
				}
				else if (current is CharacterStyleDescription && current.LinkedStyleIndex != 0) {
					characterStyle = DocumentModel.CharacterStyles.GetStyleByName(current.StyleName);
					string paragraphStyleName;
					if (StyleMapping.TryGetValue(current.LinkedStyleIndex, out paragraphStyleName))
						paragraphStyle = DocumentModel.ParagraphStyles.GetStyleByName(paragraphStyleName);
				}
				if (paragraphStyle != null && characterStyle != null)
					DocumentModel.StyleLinkManager.CreateLink(paragraphStyle, characterStyle);
			}
		}
		void UpdateDefaultFontSize() {
			if (String.IsNullOrEmpty(DefaultParagraphStyleName))
				return;
			ParagraphStyle defaultStyle = DocumentModel.ParagraphStyles.GetStyleByName(DefaultParagraphStyleName);
			if (!defaultStyle.CharacterProperties.UseDoubleFontSize )
				defaultStyle.CharacterProperties.DoubleFontSize = defaultFontSize*2;
		}
		public CharacterFormattingInfo GetCharacterFormattingInfo(DocCharacterFormattingInfo info) {
			int characterStyleIndex = GetStyleIndex(info.StyleIndex, StyleType.CharacterStyle);
			CharacterFormattingInfo formattingInfo = GetFormattingInfoCore(characterStyleIndex);
			return DocCharacterFormattingHelper.GetMergedCharacterFormattingInfo(info, formattingInfo);
		}
		CharacterFormattingInfo GetCharacterStyleFormattingInfo(short baseStyleIndex, DocPropertyContainer propertyContainer) {
			int parentStyleIndex = GetStyleIndex(baseStyleIndex, StyleType.CharacterStyle);
			CharacterFormattingInfo formattingInfo = GetFormattingInfoCore(parentStyleIndex);
			if (propertyContainer.CharacterInfo == null)
				return formattingInfo;
			return DocCharacterFormattingHelper.GetMergedCharacterFormattingInfo(propertyContainer.CharacterInfo.FormattingInfo, formattingInfo);
		}
		CharacterFormattingInfo GetFormattingInfoCore(int characterStyleIndex) {
			if (characterStyleIndex >= 0) {
				CharacterStyle style = DocumentModel.CharacterStyles[characterStyleIndex];
				return style.CharacterProperties.Info.Info.Clone();
			}
			else
				return DocumentModel.Cache.CharacterFormattingCache[CharacterFormattingInfoCache.DefaultItemIndex].Info.Clone();
		}
		CharacterFormattingInfo GetParagraphStyleFormattingInfo(short baseStyleIndex, DocPropertyContainer propertyContainer) {
			CharacterFormattingInfo formattingInfo;
			int parentStyleIndex = GetStyleIndex(baseStyleIndex, StyleType.ParagraphStyle);
			if (baseStyleIndex != StyleDescriptionBase.EmptyStyleIdentifier) {
				ParagraphStyle parent = DocumentModel.ParagraphStyles[parentStyleIndex];
				formattingInfo = parent.CharacterProperties.Info.Info.Clone();
			}
			else
				formattingInfo = DocumentModel.Cache.CharacterFormattingCache[CharacterFormattingInfoCache.DefaultItemIndex].Info.Clone();
			if (propertyContainer.CharacterInfo == null)
				return formattingInfo;
			return DocCharacterFormattingHelper.GetMergedCharacterFormattingInfo(propertyContainer.CharacterInfo.FormattingInfo, formattingInfo);
		}
		CharacterFormattingInfo GetTableStyleFormattingInfo(short baseStyleIndex, DocPropertyContainer propertyContainer) {
			CharacterFormattingInfo formattingInfo;
			if (baseStyleIndex != StyleDescriptionBase.EmptyStyleIdentifier) {
				TableStyle parent = DocumentModel.TableStyles[GetStyleIndex(baseStyleIndex, StyleType.TableStyle)];
				formattingInfo = parent.CharacterProperties.Info.Info.Clone();
			}
			else
				formattingInfo = DocumentModel.Cache.CharacterFormattingCache[CharacterFormattingInfoCache.DefaultItemIndex].Info.Clone();
			if (propertyContainer.CharacterInfo == null)
				return formattingInfo;
			return DocCharacterFormattingHelper.GetMergedCharacterFormattingInfo(propertyContainer.CharacterInfo.FormattingInfo, formattingInfo);
		}
	}
}
