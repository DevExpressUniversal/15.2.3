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
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Export.Rtf {
	#region RtfStyleExporter
	public class RtfStyleExporter {
		#region Fields
		readonly IRtfExportHelper rtfExportHelper;
		readonly RtfBuilder rtfBuilder;
		readonly RtfCharacterPropertiesExporter characterPropertiesExporter;
		readonly RtfParagraphPropertiesExporter paragraphPropertiesExporter;
		RtfTableStyleTablePropertiesExporter tablePropertiesExporter;
		RtfTableRowPropertiesExporter tableRowPropertiesExporter;
		RtfTableStyleTableCellPropertiesExporter tableCellPropertiesExporter;
		readonly DocumentModel documentModel;
		#endregion
		public RtfStyleExporter(DocumentModel documentModel, RtfBuilder rtfBuilder, IRtfExportHelper rtfExportHelper, RtfDocumentExporterOptions options) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			Guard.ArgumentNotNull(rtfExportHelper, "rtfExportHelper");
			Guard.ArgumentNotNull(options, "options");
			this.documentModel = documentModel;
			this.rtfExportHelper = rtfExportHelper;
			this.rtfBuilder = rtfBuilder;
			this.characterPropertiesExporter = new RtfCharacterPropertiesExporter(documentModel, RtfExportHelper, rtfBuilder, options);
			this.paragraphPropertiesExporter = new RtfParagraphPropertiesExporter(documentModel, RtfExportHelper, rtfBuilder);
		}
		#region Properties
		protected internal RtfBuilder RtfBuilder { get { return rtfBuilder; } }
		protected internal IRtfExportHelper RtfExportHelper { get { return rtfExportHelper; } }
		protected internal RtfCharacterPropertiesExporter CharacterExporter { get { return characterPropertiesExporter; } }
		protected DocumentModel DocumentModel { get { return documentModel; } }
		#endregion
		public virtual void ExportStyleSheet(ParagraphStyleCollection paragraphStyles, CharacterStyleCollection characterStyles, TableStyleCollection tableStyles) {
			if (paragraphStyles.Count > 0)
				ExportParagraphStyles(paragraphStyles);
			if (characterStyles.Count > 0)
				ExportCharacterStyles(characterStyles);
			if (tableStyles.Count > 0)
				ExportTableStyles(tableStyles);
		}
		public virtual void ExportParagraphStyles(ParagraphStyleCollection paragraphStyles) {
			rtfBuilder.Clear();
			IList<ParagraphStyle> styles = Algorithms.TopologicalSort<ParagraphStyle>(paragraphStyles, new StyleTopologicalComparer<ParagraphStyle>());
			List<ParagraphStyle> stylesToWrite = new List<ParagraphStyle>(styles.Count);
			int count = styles.Count;
			for (int i = 0; i < count; i++) {
				ParagraphStyle style = styles[i];
				if (!style.Deleted && !RtfExportHelper.ParagraphStylesCollectionIndex.ContainsKey(style.StyleName)) {
					stylesToWrite.Add(style);
					int styleIndex = GetNextFreeStyleIndex();
					RtfExportHelper.ParagraphStylesCollectionIndex.Add(style.StyleName, styleIndex);
				}
			}
			count = stylesToWrite.Count;
			for (int i = 0; i < count; i++)
				ExportParagraphStyle(stylesToWrite[i], i);
			RtfExportHelper.StylesCollection.Add(rtfBuilder.RtfContent.ToString());
		}
		protected internal virtual void ExportParagraphStyle(ParagraphStyle style, int i) {
			int styleIndex = ObtainParagraphStyleIndex(style);
			if (styleIndex < 0)
				return;
			rtfBuilder.OpenGroup();
			if (i > 0) {
				rtfBuilder.WriteCommand(RtfExportSR.ParagraphStyle, styleIndex);
				int parentStyleIndex = ObtainParagraphStyleIndex(style.Parent);
				if (parentStyleIndex >= 0)
					rtfBuilder.WriteCommand(RtfExportSR.ParentStyle, parentStyleIndex);
			}
			if (style.HasLinkedStyle) {
				int linkedStyleIndex = ObtainCharacterStyleIndex(style.LinkedStyle);
				if (linkedStyleIndex >= 0)
					rtfBuilder.WriteCommand(RtfExportSR.LinkedStyle, linkedStyleIndex);
			}
			if (style.NextParagraphStyle != null) {
				int nextStyleIndex = ObtainParagraphStyleIndex(style.NextParagraphStyle);
				if (nextStyleIndex >= 0)
					rtfBuilder.WriteCommand(RtfExportSR.NextStyle, nextStyleIndex);
			}
			if (style.Primary)
				rtfBuilder.WriteCommand(RtfExportSR.QuickFormatStyle);
			ExportParagraphProperties(style.ParagraphProperties, style.GetMergedParagraphProperties());
			ExportCharacterProperties(style.GetMergedCharacterProperties());
			if (style.GetNumberingListIndex() >= new NumberingListIndex(0)) {
				RtfBuilder.WriteCommand(RtfExportSR.ListIndex, GetListId(style.GetNumberingListIndex()));
				int listLevelIndex = style.GetListLevelIndex();
				if (listLevelIndex > 0) {
					RtfBuilder.WriteCommand(RtfExportSR.LevelIndex, listLevelIndex);
				}
			}
			WriteStyleName(style.StyleName);
			rtfBuilder.CloseGroup();
		}
		protected virtual int GetListId(NumberingListIndex index) {
			return documentModel.NumberingLists[index].Id;
		}
		public virtual void ExportCharacterStyles(CharacterStyleCollection characterStyles) {
			rtfBuilder.Clear();
			int count = characterStyles.Count;
			for (int i = 0; i < count; i++)
				ExportCharacterStyle(characterStyles[i]);
			RtfExportHelper.StylesCollection.Add(rtfBuilder.RtfContent.ToString());
		}
		public virtual void ExportTableStyles(TableStyleCollection tableStyles) {
			rtfBuilder.Clear();
			tablePropertiesExporter = new RtfTableStyleTablePropertiesExporter(documentModel, rtfExportHelper, rtfBuilder);
			tableRowPropertiesExporter = new RtfTableRowPropertiesExporter(documentModel, rtfExportHelper, rtfBuilder);
			tableCellPropertiesExporter = new RtfTableStyleTableCellPropertiesExporter(documentModel, rtfExportHelper, rtfBuilder);
			int count = tableStyles.Count;
			for (int i = 0; i < count; i++)
				ExportTableStyle(tableStyles[i]);
			RtfExportHelper.StylesCollection.Add(rtfBuilder.RtfContent.ToString());
		}
		protected internal virtual void ExportCharacterStyle(CharacterStyle style) {
			if (style.Deleted)
				return;
			if (RtfExportHelper.CharacterStylesCollectionIndex.ContainsKey(style.StyleName))
				return;
			int styleIndex = GetNextFreeStyleIndex();
			RtfExportHelper.CharacterStylesCollectionIndex.Add(style.StyleName, styleIndex);
			rtfBuilder.OpenGroup();
			rtfBuilder.WriteCommand(RtfExportSR.CharacterStyle, styleIndex);
			int parentStyleIndex = ObtainCharacterStyleIndex(style.Parent);
			if (parentStyleIndex >= 0)
				rtfBuilder.WriteCommand(RtfExportSR.ParentStyle, parentStyleIndex);
			if (style.HasLinkedStyle) {
				int linkedStyleIndex = ObtainParagraphStyleIndex(style.LinkedStyle);
				if (linkedStyleIndex >= 0)
					rtfBuilder.WriteCommand(RtfExportSR.LinkedStyle, linkedStyleIndex);
			}
			if (style.Primary)
				rtfBuilder.WriteCommand(RtfExportSR.QuickFormatStyle);
			ExportCharacterProperties(style.GetMergedCharacterProperties());
			WriteStyleName(style.StyleName);
			rtfBuilder.CloseGroup();
		}
		protected internal virtual void ExportTableStyle(TableStyle style) {
			if (style.Deleted)
				return;
			if (RtfExportHelper.TableStylesCollectionIndex.ContainsKey(style.StyleName))
				return;
			int styleIndex = GetNextFreeStyleIndex();
			RtfExportHelper.TableStylesCollectionIndex.Add(style.StyleName, styleIndex);
			rtfBuilder.OpenGroup();
			rtfBuilder.WriteCommand(RtfExportSR.TableStyle, styleIndex);
			rtfBuilder.WriteCommand(RtfExportSR.TableStyleResetTableProperties);
			int parentStyleIndex = ObtainTableStyleIndex(style.Parent);
			if (parentStyleIndex >= 0)
				rtfBuilder.WriteCommand(RtfExportSR.ParentStyle, parentStyleIndex);
			if (style.Primary)
				rtfBuilder.WriteCommand(RtfExportSR.QuickFormatStyle);
			ExportCharacterProperties(style.GetMergedCharacterProperties());
			ExportParagraphProperties(style.ParagraphProperties, style.GetMergedParagraphProperties());
			ExportTableProperties(style.TableProperties, style.GetMergedTableProperties(), style.HasRowBandingStyleProperties, style.HasColumnBandingStyleProperties);
			ExportTableRowProperties(style.TableRowProperties, style.GetMergedTableRowProperties());
			ExportTableCellProperties(style.TableCellProperties, style.GetMergedTableCellProperties());
			WriteStyleName(style.StyleName);
			rtfBuilder.CloseGroup();
			if (style.HasConditionalStyleProperties) {
				foreach (TableConditionalStyle conditionalStyle in style.ConditionalStyleProperties.Items.Values) {
					ExportTableConditionalStyle(conditionalStyle, style.StyleName, styleIndex);
				}
			}
		}
		void ExportTableConditionalStyle(TableConditionalStyle conditionalStyle, string StyleName, int styleIndex) {
			if (conditionalStyle == null)
				return;
			rtfBuilder.OpenGroup();
			rtfBuilder.WriteCommand(RtfExportSR.TableStyle, styleIndex);
			rtfBuilder.WriteCommand(RtfExportSR.TableStyleResetTableProperties);
			ExportCharacterProperties(conditionalStyle.GetMergedCharacterProperties());
			ExportParagraphProperties(conditionalStyle.ParagraphProperties, conditionalStyle.GetMergedParagraphProperties());
			ExportTableRowProperties(conditionalStyle.TableRowProperties, conditionalStyle.GetMergedTableRowProperties());
			ExportTableCellProperties(conditionalStyle.TableCellProperties, conditionalStyle.GetMergedTableCellProperties());
			WriteConditionalStyleType(conditionalStyle.ConditionType);
			WriteStyleName(StyleName);
			rtfBuilder.CloseGroup();
		}
		protected virtual void WriteStyleName(string name) {
			int count = name.Length;
			for (int i = 0; i < count; i++) {
				rtfBuilder.WriteChar(name[i]);
			}
			rtfBuilder.WriteChar(';');
		}
		protected void WriteConditionalStyleType(ConditionalTableStyleFormattingTypes conditionType) {
			string keyword;
			if (RtfContentExporter.ConditionalStylesTypes.TryGetValue(conditionType, out keyword))
				rtfBuilder.WriteCommand(keyword);
		}
		protected internal void ExportCharacterProperties(MergedCharacterProperties characterProperties) {
			CharacterPropertiesMerger merger = new CharacterPropertiesMerger(characterProperties);
			merger.Merge(documentModel.DefaultCharacterProperties);
			CharacterExporter.ExportCharacterProperties(merger.MergedProperties, true, false, false);
		}
		protected internal void ExportParagraphProperties(ParagraphProperties paragraphProperties, MergedParagraphProperties mergedParagraphProperties) {
			ParagraphPropertiesMerger merger = new ParagraphPropertiesMerger(mergedParagraphProperties);
			merger.Merge(documentModel.DefaultParagraphProperties);
			ParagraphFormattingInfo info = merger.MergedProperties.Info;
			if (info.TopBorder.Style != BorderLineStyle.None)
				paragraphPropertiesExporter.WriteParagraphBorder(info.TopBorder, RtfExportSR.TopParagraphBorder);
			if (info.LeftBorder.Style != BorderLineStyle.None)
				paragraphPropertiesExporter.WriteParagraphBorder(info.LeftBorder, RtfExportSR.LeftParagraphBorder);
			if (info.BottomBorder.Style != BorderLineStyle.None)
				paragraphPropertiesExporter.WriteParagraphBorder(info.BottomBorder, RtfExportSR.BottomParagraphBorder);
			if (info.RightBorder.Style != BorderLineStyle.None)
				paragraphPropertiesExporter.WriteParagraphBorder(info.RightBorder, RtfExportSR.RightParagraphBorder);
			paragraphPropertiesExporter.WriteParagraphAlignment(info.Alignment);
			paragraphPropertiesExporter.WriteParagraphIndents(mergedParagraphProperties);
			paragraphPropertiesExporter.WriteParagraphSuppressHyphenation(info.SuppressHyphenation);
			paragraphPropertiesExporter.WriteParagraphSuppressLineNumbers(info.SuppressLineNumbers);
			paragraphPropertiesExporter.WriteParagraphContextualSpacing(info.ContextualSpacing);
			paragraphPropertiesExporter.WriteParagraphPageBreakBefore(info.PageBreakBefore);
			paragraphPropertiesExporter.WriteParagraphOutlineLevel(info.OutlineLevel);
			paragraphPropertiesExporter.WriteParagraphBackColor(info.BackColor);
			paragraphPropertiesExporter.WriteParagraphLineSpacing(info.LineSpacingType, info.LineSpacing);
			paragraphPropertiesExporter.WriteParagraphSpacingBefore(paragraphProperties.SpacingBefore);
			paragraphPropertiesExporter.WriteParagraphSpacingAfter(paragraphProperties.SpacingAfter);
		}
		protected internal void ExportTableProperties(TableProperties tableProperties, MergedTableProperties mergedTableProperties, bool exportRowProperties, bool exportColProperties) {
			TablePropertiesMerger merger = new TablePropertiesMerger(mergedTableProperties);
			merger.Merge(DocumentModel.DefaultTableProperties);
			tablePropertiesExporter.ExportTableProperties(merger.MergedProperties, exportRowProperties, exportColProperties);
		}
		protected internal void ExportTableRowProperties(TableRowProperties tableRowProperties, MergedTableRowProperties mergedTableRowProperties) {
			TableRowPropertiesMerger merger = new TableRowPropertiesMerger(mergedTableRowProperties);
			merger.Merge(documentModel.DefaultTableRowProperties);
			CombinedTableRowPropertiesInfo prop = merger.MergedProperties.Info;
			tableRowPropertiesExporter.WriteRowAlignment(prop.GeneralSettings.TableRowAlignment);
			tableRowPropertiesExporter.WriteRowHeight(prop.Height);
			tableRowPropertiesExporter.WriteRowHeader(prop.GeneralSettings.Header);
			tableRowPropertiesExporter.WriteRowCantSplit(tableRowProperties.CantSplit);
			tableRowPropertiesExporter.WriteWidthBefore(prop.WidthBefore);
			tableRowPropertiesExporter.WriteWidthAfter(prop.WidthAfter);
			tableRowPropertiesExporter.WriteRowCellSpacing(prop.CellSpacing);
		}
		protected internal void ExportTableCellProperties(TableCellProperties tableCellProperties, MergedTableCellProperties mergedTableCellProperties) {
			TableCellPropertiesMerger merger = new TableCellPropertiesMerger(mergedTableCellProperties);
			merger.Merge(documentModel.DefaultTableCellProperties);
			CombinedCellPropertiesInfo info = merger.MergedProperties.Info;
			tableCellPropertiesExporter.WriteCellMerging(tableCellProperties.VerticalMerging);
			tableCellPropertiesExporter.WriteCellVerticalAlignment(info.GeneralSettings.VerticalAlignment);
			tableCellPropertiesExporter.WriteCellBackgroundColor(info.GeneralSettings.BackgroundColor);
			tableCellPropertiesExporter.WriteCellForegroundColor(info.GeneralSettings.ForegroundColor);
			tableCellPropertiesExporter.WriteCellShading(info.GeneralSettings.ShadingPattern);
			tableCellPropertiesExporter.WriteCellBasicBorders(info.Borders.TopBorder, info.Borders.LeftBorder, info.Borders.RightBorder, info.Borders.BottomBorder);
			tableCellPropertiesExporter.WriteCellTextDirection(info.GeneralSettings.TextDirection);
			tableCellPropertiesExporter.WriteCellFitText(info.GeneralSettings.FitText);
			tableCellPropertiesExporter.WriteCellNoWrap(info.GeneralSettings.NoWrap);
			tableCellPropertiesExporter.WriteCellHideCellMark(info.GeneralSettings.HideCellMark);
			tableCellPropertiesExporter.WriteCellPreferredWidth(info.PreferredWidth);
			tableCellPropertiesExporter.WriteCellMargings(info.CellMargins.Top, info.CellMargins.Left, info.CellMargins.Right, info.CellMargins.Bottom);
		}
		protected internal int GetNextFreeStyleIndex() {
			int result = 0;
			while (RtfExportHelper.CharacterStylesCollectionIndex.ContainsValue(result)
				|| RtfExportHelper.ParagraphStylesCollectionIndex.ContainsValue(result)
				|| RtfExportHelper.TableStylesCollectionIndex.ContainsValue(result))
				result++;
			return result;
		}
		protected internal int ObtainParagraphStyleIndex(ParagraphStyle style) {
			return ObtainStyleIndex(style, RtfExportHelper.ParagraphStylesCollectionIndex);
		}
		protected internal int ObtainCharacterStyleIndex(CharacterStyle style) {
			return ObtainStyleIndex(style, RtfExportHelper.CharacterStylesCollectionIndex);
		}
		protected internal int ObtainTableStyleIndex(TableStyle style) {
			return ObtainStyleIndex(style, RtfExportHelper.TableStylesCollectionIndex);
		}
		protected internal int ObtainStyleIndex(IStyle style, Dictionary<string, int> collection) {
			if (style == null)
				return -1;
			int result;
			if (collection.TryGetValue(style.StyleName, out result))
				return result;
			else
				return -1;
		}
	}
	#endregion
}
