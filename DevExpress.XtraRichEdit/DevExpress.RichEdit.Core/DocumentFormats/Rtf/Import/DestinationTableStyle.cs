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
namespace DevExpress.XtraRichEdit.Import.Rtf {
	#region DestinationTableStyle
	public class DestinationTableStyle : DestinationPieceTable {
		KeywordTranslatorTable keywordHT = CreateKeywordTable();
		static KeywordTranslatorTable CreateKeywordTable() {
			KeywordTranslatorTable table = new KeywordTranslatorTable();
			table.Add("sbasedon", OnParentStyleIndex);
			table.Add("sqformat", OnStyleQFormatKeyword);
			table.Add("tscfirstrow", OnConditionalStyleFirstRow);
			table.Add("tsclastrow", OnConditionalStyleLastRow);
			table.Add("tscfirstcol", OnConditionalStyleFirstColumn);
			table.Add("tsclastcol", OnConditionalStyleLastColumn);
			table.Add("tscbandhorzodd", OnConditionalStyleOddRowBanding);
			table.Add("tscbandhorzeven", OnConditionalStyleEvenRowBanding);
			table.Add("tscbandvertodd", OnConditionalStyleOddColumnBanding);
			table.Add("tscbandverteven", OnConditionalStyleEvenColumnBanding);
			table.Add("tscnwcell", OnConditionalStyleTopLeftCell);
			table.Add("tscnecell", OnConditionalStyleTopRightCell);
			table.Add("tscswcell", OnConditionalStyleBottomLeftCell);
			table.Add("tscsecell", OnConditionalStyleBottomRightCell);
			AddCharacterPropertiesKeywords(table);
			AddParagraphPropertiesKeywords(table);
			AppendTablePropertiesKeywords(table);
			return table;
		}
		static void OnStyleQFormatKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			GetThis(importer).QFormat = true;
		}
		static void OnParentStyleIndex(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Position.RtfFormattingInfo.ParentStyleIndex = parameterValue;
		}
		static void OnConditionalStyleFirstRow(RtfImporter importer, int parameterValue, bool hasParameter) {
			GetThis(importer).conditionalTableStyleFormattingType = ConditionalTableStyleFormattingTypes.FirstRow;
		}
		static void OnConditionalStyleLastRow(RtfImporter importer, int parameterValue, bool hasParameter) {
			GetThis(importer).conditionalTableStyleFormattingType = ConditionalTableStyleFormattingTypes.LastRow;
		}
		static void OnConditionalStyleFirstColumn(RtfImporter importer, int parameterValue, bool hasParameter) {
			GetThis(importer).conditionalTableStyleFormattingType = ConditionalTableStyleFormattingTypes.FirstColumn;
		}
		static void OnConditionalStyleLastColumn(RtfImporter importer, int parameterValue, bool hasParameter) {
			GetThis(importer).conditionalTableStyleFormattingType = ConditionalTableStyleFormattingTypes.LastColumn;
		}
		static void OnConditionalStyleOddRowBanding(RtfImporter importer, int parameterValue, bool hasParameter) {
			GetThis(importer).conditionalTableStyleFormattingType = ConditionalTableStyleFormattingTypes.OddRowBanding;
		}
		static void OnConditionalStyleEvenRowBanding(RtfImporter importer, int parameterValue, bool hasParameter) {
			GetThis(importer).conditionalTableStyleFormattingType = ConditionalTableStyleFormattingTypes.EvenRowBanding;
		}
		static void OnConditionalStyleOddColumnBanding(RtfImporter importer, int parameterValue, bool hasParameter) {
			GetThis(importer).conditionalTableStyleFormattingType = ConditionalTableStyleFormattingTypes.OddColumnBanding;
		}
		static void OnConditionalStyleEvenColumnBanding(RtfImporter importer, int parameterValue, bool hasParameter) {
			GetThis(importer).conditionalTableStyleFormattingType = ConditionalTableStyleFormattingTypes.EvenColumnBanding;
		}
		static void OnConditionalStyleTopLeftCell(RtfImporter importer, int parameterValue, bool hasParameter) {
			GetThis(importer).conditionalTableStyleFormattingType = ConditionalTableStyleFormattingTypes.TopLeftCell;
		}
		static void OnConditionalStyleTopRightCell(RtfImporter importer, int parameterValue, bool hasParameter) {
			GetThis(importer).conditionalTableStyleFormattingType = ConditionalTableStyleFormattingTypes.TopRightCell;
		}
		static void OnConditionalStyleBottomLeftCell(RtfImporter importer, int parameterValue, bool hasParameter) {
			GetThis(importer).conditionalTableStyleFormattingType = ConditionalTableStyleFormattingTypes.BottomLeftCell;
		}
		static void OnConditionalStyleBottomRightCell(RtfImporter importer, int parameterValue, bool hasParameter) {
			GetThis(importer).conditionalTableStyleFormattingType = ConditionalTableStyleFormattingTypes.BottomRightCell;
		}
		static DestinationTableStyle GetThis(RtfImporter rtfImporter) {
			return (DestinationTableStyle)rtfImporter.Destination;
		}
		string styleName;
		int rtfStyleIndex;
		ConditionalTableStyleFormattingTypes conditionalTableStyleFormattingType = ConditionalTableStyleFormattingTypes.WholeTable;
		public DestinationTableStyle(RtfImporter importer, int styleIndex)
			: base(importer, importer.PieceTable) {
			this.styleName = String.Empty;
			this.rtfStyleIndex = styleIndex;
			SetCharacterFormattingOptions(CharacterFormattingOptions.Mask.UseNone);
		}
		protected override ControlCharTranslatorTable ControlCharHT { get { return null; } }
		protected override KeywordTranslatorTable KeywordHT { get { return keywordHT; } }
		protected internal override bool CanAppendText { get { return false; } }
		bool QFormat { get; set; }
		public override void BeforePopRtfState() {
			string name = styleName.Trim();
			ITableStyle style = null;
			int parentStyleIndex = Importer.Position.RtfFormattingInfo.ParentStyleIndex;
			bool isConditionalStyle = this.conditionalTableStyleFormattingType != ConditionalTableStyleFormattingTypes.WholeTable;
			if (isConditionalStyle)
				style = AddConditionalStyle();
			else {
				SetCharacterFormattingOptions(CharacterFormattingOptions.Mask.UseAll);
				if (!Importer.TableStyleCollectionIndex.ContainsKey(rtfStyleIndex))
					style = GetTableStyleByName(name);
			}
			if (style == null)
				return;
			TableStyle tableStyle = style as TableStyle;
			if (tableStyle != null)
				tableStyle.Primary = QFormat;
			if (name != TableStyleCollection.DefaultTableStyleName) {
				TableStyle parentTableStyle;
				if (!isConditionalStyle) {
					parentTableStyle = DocumentModel.TableStyles[Importer.GetTableStyleIndex(parentStyleIndex)];
				}
				else {
					parentTableStyle = style.Parent;
					if(parentTableStyle == null)
						parentTableStyle = DocumentModel.TableStyles[Importer.GetTableStyleIndex(-1)];
				}
				MergedParagraphProperties parentParagraphProperties;
				MergedTableProperties parentTableProperties;
				MergedTableRowProperties parentTableRowProperties;
				MergedTableCellProperties parentTableCellProperties;
				if (isConditionalStyle) {
					style.CharacterProperties.CopyFrom(Importer.Position.CharacterFormatting);
					parentParagraphProperties = parentTableStyle.GetMergedParagraphProperties(conditionalTableStyleFormattingType);
					parentTableRowProperties = parentTableStyle.GetMergedTableRowProperties(conditionalTableStyleFormattingType);
					parentTableCellProperties = parentTableStyle.GetMergedTableCellProperties(conditionalTableStyleFormattingType);
				}
				else {
					MergedCharacterProperties parentCharacterProperties = Importer.GetStyleMergedCharacterProperties(parentTableStyle.GetMergedCharacterProperties());
					Importer.ApplyCharacterProperties(style.CharacterProperties, Importer.Position.CharacterFormatting.Info, parentCharacterProperties);
					parentParagraphProperties = Importer.GetStyleMergedParagraphProperties(parentTableStyle.GetMergedParagraphProperties());
					parentTableRowProperties = Importer.GetStyleMergedTableRowProperties(parentTableStyle.GetMergedTableRowProperties());
					parentTableCellProperties = Importer.GetStyleMergedTableCellProperties(parentTableStyle.GetMergedTableCellProperties());
				}
				TablePropertiesMerger merger = new TablePropertiesMerger(parentTableStyle.GetMergedTableProperties());
				merger.Merge(GetDefaultRtfProperties());
				parentTableProperties = merger.MergedProperties;
				RtfParagraphFormattingInfo formattingInfo = Importer.Position.ParagraphFormattingInfo;
				Importer.ApplyLineSpacing(formattingInfo);
				Importer.ApplyParagraphProperties(style.ParagraphProperties, formattingInfo, parentParagraphProperties);
				Importer.ApplyTableProperties(Importer.TableReader.TableProperties, parentTableProperties);
				style.TableProperties.CopyFrom(Importer.TableReader.TableProperties);
				Importer.ApplyTableRowProperties(Importer.TableReader.RowProperties, parentTableRowProperties);
				style.TableRowProperties.CopyFrom(Importer.TableReader.RowProperties);
				Importer.ApplyTableCellProperties(Importer.TableReader.CellProperties, parentTableCellProperties);
				style.TableCellProperties.CopyFrom(Importer.TableReader.CellProperties);
			}
			if (!isConditionalStyle && Importer.TableStyleCollectionIndex.ContainsKey(parentStyleIndex))
				tableStyle.Parent = DocumentModel.TableStyles[Importer.GetTableStyleIndex(parentStyleIndex)];
		}
		void SetCharacterFormattingOptions(CharacterFormattingOptions.Mask mask) {
			CharacterFormattingOptions options = new CharacterFormattingOptions(mask);
			Importer.Position.CharacterFormatting.ReplaceInfo(Importer.Position.CharacterFormatting.Info.Clone(), options);
		}
		TableProperties GetDefaultRtfProperties() {
			return DocumentModel.DefaultTableProperties;
		}
		protected virtual TableStyle GetTableStyleByName(string styleName) {
			DocumentModel documentModel = Importer.DocumentModel;
			TableStyleCollection tableStyles = documentModel.TableStyles;
			int styleIndex = tableStyles.GetStyleIndexByName(styleName);
			if (styleIndex >= 0) {
				Importer.TableStyleCollectionIndex[rtfStyleIndex] = styleIndex;
				return tableStyles[styleIndex];
			}
			TableStyle result = new TableStyle(documentModel);
			result.StyleName = styleName;
			int index = tableStyles.Add(result);
			Importer.TableStyleCollectionIndex.Add(rtfStyleIndex, index);
			return result;
		}
		TableConditionalStyle AddConditionalStyle() {
			if (!Importer.TableStyleCollectionIndex.ContainsKey(rtfStyleIndex))
				return null;
			DocumentModel documentModel = Importer.DocumentModel;
			TableStyle mainStyle = documentModel.TableStyles[Importer.GetTableStyleIndex(rtfStyleIndex)];
			TableConditionalStyle tableConditionalStyle = mainStyle.ConditionalStyleProperties.GetStyleSafe(conditionalTableStyleFormattingType);
			return tableConditionalStyle;
		}
		protected override DestinationBase CreateClone() {
			return new DestinationTableStyle(Importer, Importer.Position.CharacterStyleIndex);
		}
		protected override void ProcessCharCore(char ch) {
			if (ch != ';')
				styleName += ch;
		}
		public override void FinalizePieceTableCreation() {
		}
	}
	#endregion
}
