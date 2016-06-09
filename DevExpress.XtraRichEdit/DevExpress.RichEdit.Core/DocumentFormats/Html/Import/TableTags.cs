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
using DevExpress.Data.Utils;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Compatibility.System.Drawing;
using Debug = System.Diagnostics.Debug;
#if !SL
using System.Drawing;
using System.Collections;
using DevExpress.Office.Utils;
using System.Diagnostics;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Import.Html {
	#region TableTag
	public class TableTag : TagBase {
		#region AttributeKeywordTranslatorTable
		static AttributeKeywordTranslatorTable attributeTable = AddAttributes();
		static AttributeKeywordTranslatorTable AddAttributes() {
			AttributeKeywordTranslatorTable table = CreateAttributeTable();
			table.Add(ConvertKeyToUpper("align"), AlignmentKeyword);
			table.Add(ConvertKeyToUpper("background"), BackgroundImageKeyword);
			table.Add(ConvertKeyToUpper("bgcolor"), BackgroundColorKeyword);
			table.Add(ConvertKeyToUpper("border"), BorderWidthKeyword);
			table.Add(ConvertKeyToUpper("bordercolor"), BorderColorKeyword);
			table.Add(ConvertKeyToUpper("cellpadding"), CellPaddingKeyword);
			table.Add(ConvertKeyToUpper("cellspacing"), CellSpacingKeyword);
			table.Add(ConvertKeyToUpper("cols"), ColumnsCountKeyword);
			table.Add(ConvertKeyToUpper("height"), TableHeightKeyword);
			table.Add(ConvertKeyToUpper("width"), TableWidthKeyword);
			return table;
		}
		private static void TableIdKeyword(HtmlImporter importer, string value, TagBase tag) {
		}
		#endregion
		#region Handlers
		static internal void AlignmentKeyword(HtmlImporter importer, string value, TagBase tag) {
			TableTag tableTag = (TableTag)tag;
			TableRowAlignment rowAlignment = TableRowAlignment.Left;
			if (ImportAlignment(value, ref rowAlignment))
				tableTag.TableProperties.TableAlignment = rowAlignment;
		}
		static internal void BackgroundImageKeyword(HtmlImporter importer, string value, TagBase tag) {
		}
		static internal void BackgroundColorKeyword(HtmlImporter importer, string value, TagBase tag) {
			TableTag tableTag = (TableTag)tag;
			ImportBackgroundColor(importer.DocumentModel.UnitConverter, value, tableTag.TableProperties);
		}
		static internal void BorderWidthKeyword(HtmlImporter importer, string value, TagBase tag) {
			TableTag tableTag = (TableTag)tag;
			ImportBordersWidths(importer.DocumentModel.UnitConverter, value, tableTag.tableProperties.BordersProperties);
			if (tableTag.tableProperties.BordersProperties.TopBorder.Width > 0) {
				tableTag.tableProperties.SetInnerBorders = true;
				if (!tableTag.tableProperties.BordersProperties.TopBorder.UseLineStyle) {
					tableTag.tableProperties.BordersProperties.TopBorder.LineStyle = BorderLineStyle.Single;
					tableTag.tableProperties.BordersProperties.LeftBorder.LineStyle = BorderLineStyle.Single;
					tableTag.tableProperties.BordersProperties.RightBorder.LineStyle = BorderLineStyle.Single;
					tableTag.tableProperties.BordersProperties.BottomBorder.LineStyle = BorderLineStyle.Single;
				}
			}
		}
		static internal void BorderColorKeyword(HtmlImporter importer, string value, TagBase tag) {
			TableTag tableTag = (TableTag)tag;
			ImportBordersColors(importer.DocumentModel.UnitConverter, value, tableTag.TableProperties.BordersProperties);
		}
		public static void ImportBackgroundColor(DocumentModelUnitConverter converter, string value, HtmlTableProperties tableProperties) {
			Color color = ImportColor(converter, value);
			if (color != DXColor.Empty)
				tableProperties.BackgroundColor = color;
		}
		static internal void CellPaddingKeyword(HtmlImporter importer, string value, TagBase tag) {
			TableTag tableTag = (TableTag)tag;
			WidthUnitInfo cellMarging = ConvertPixelsValueToWidthUnitInfo(importer.DocumentModel.UnitConverter, value);
			if (cellMarging.Type != WidthUnitType.Nil && cellMarging.Value >= 0)
				tableTag.TableProperties.CellMarging = cellMarging;
		}
		static internal void CellSpacingKeyword(HtmlImporter importer, string value, TagBase tag) {
			TableTag tableTag = (TableTag)tag;
			WidthUnitInfo cellSpacing = ConvertPixelsValueToWidthUnitInfo(importer.DocumentModel.UnitConverter, value);
			if (cellSpacing.Type != WidthUnitType.Nil && cellSpacing.Value >= 0)
				tableTag.TableProperties.CellSpacing = cellSpacing;
		}
		static internal void ColumnsCountKeyword(HtmlImporter importer, string value, TagBase tag) {
		}
		static internal void TableHeightKeyword(HtmlImporter importer, string value, TagBase tag) {
		}
		static internal void TableWidthKeyword(HtmlImporter importer, string value, TagBase tag) {
			TableTag tableTag = (TableTag)tag;
			WidthUnitInfo width = ConvertPixelsValueToWidthUnitInfo(importer.DocumentModel.UnitConverter, value);
			if (width.Type != WidthUnitType.Nil && width.Value >= 0)
				tableTag.TableProperties.Width = width;
		}
		#endregion
		#region Fields
		HtmlTableProperties tableProperties;
		bool tableNotCreated;
		#endregion
		public TableTag(HtmlImporter importer)
			: base(importer) {
			SetDefaultTableProperties();
			this.tableNotCreated = false;
		}
		public bool TablesDisabled { get { return !Importer.TablesImportHelper.TablesAllowed; } }
		void SetDefaultTableProperties() {
			this.tableProperties = new HtmlTableProperties();
			this.tableProperties.BordersProperties.TopBorder.Width = 0;
			this.tableProperties.BordersProperties.LeftBorder.Width = 0;
			this.tableProperties.BordersProperties.RightBorder.Width = 0;
			this.tableProperties.BordersProperties.BottomBorder.Width = 0;
			if (Importer.Options.DefaultTableCellSpacing > 0) {
				int defaultSpacing = Importer.DocumentModel.UnitConverter.TwipsToModelUnits(Importer.Options.DefaultTableCellSpacing);
				this.tableProperties.CellSpacing = new WidthUnitInfo(WidthUnitType.ModelUnits, defaultSpacing);
			}
			if (Importer.Options.DefaultTableCellMarging > 0) {
				int defaultMarging = Importer.DocumentModel.UnitConverter.TwipsToModelUnits(Importer.Options.DefaultTableCellMarging);
				this.tableProperties.CellMarging = new WidthUnitInfo(WidthUnitType.ModelUnits, defaultMarging);
			}
		}
		#region Properties
		protected internal override AttributeKeywordTranslatorTable AttributeTable { get { return attributeTable; } }
		public HtmlTableProperties TableProperties { get { return tableProperties; } }
		public bool TableNotCreated { get { return tableNotCreated; } set { tableNotCreated = value; } }
		#endregion
		#region ApplyTagProperties
		protected internal override void ApplyTagProperties() {
			Importer.Position.TableProperties.CopyFrom(TableProperties);
			WidthUnitInfo indent = new WidthUnitInfo(WidthUnitType.ModelUnits, Importer.Position.TableProperties.Indent.Value + Importer.Position.AdditionalIndent);
			Importer.Position.TableProperties.Indent = indent;
			TableProperties.ApplyPropertiesToCharacter(Importer.Position.CharacterFormatting);
		}
		#endregion
		protected internal override ParagraphFormattingOptions ApplyCssProperties() {
			ParagraphFormattingOptions options = base.ApplyCssProperties();
			int count = Importer.TagsStack.Count;
			if (count < 2)
				return options;
			OpenHtmlTag prevTag = Importer.TagsStack[count - 2];
			LevelTag levelTag = prevTag.Tag as LevelTag;
			if (levelTag != null) {
				int delta = options.UseLeftIndent ? Importer.Position.ParagraphFormatting.LeftIndent : 0;
				Importer.Position.TableProperties.Indent.Value = Importer.TagsStack[count - 1].OldPosition.ParagraphFormatting.LeftIndent + delta;
			}
			return options;
		}
		#region FunctionalTagProcess
		protected internal override void FunctionalTagProcess() {
			ParagraphFunctionalProcess();
		}
		#endregion
		protected internal override void OpenTagProcessCore() {
			if (TablesDisabled)
				return;
			if (ShouldIgnoreOpenTag()) {
				TableNotCreated = true;
				return;
			}
			ClearParentProperties();
			Table newTable = Importer.TablesImportHelper.CreateTable(ObtainParentCell());			
			Importer.Position.TableProperties.ApplyPropertiesToTable(newTable.TableProperties);
			if (Importer.IsEmptyLine) {
				Importer.SetAppendObjectProperty();
				ParagraphFunctionalProcess();
			}
		}
		bool ShouldIgnoreOpenTag() {
			int i = Importer.TagsStack.Count - 2;
			while (i >= 0) {
				TagBase tag = Importer.TagsStack[i].Tag;
				HtmlTagNameID tagName = tag.Name;
				if (tagName == HtmlTagNameID.TD || tagName == HtmlTagNameID.TR || tagName == HtmlTagNameID.TH)
					return false;
				if (tagName == HtmlTagNameID.Table)
					return true;
				i--;
			}
			return false;
		}
		void ClearParentProperties() {
			Importer.Position.ParagraphFormatting.ReplaceInfo(DocumentModel.Cache.ParagraphFormattingInfoCache.DefaultItem, new ParagraphFormattingOptions(ParagraphFormattingOptions.Mask.UseNone));
			Importer.Position.ParagraphTabs.Clear();
			Importer.Position.DefaultAlignment.ResetDefaultAlignment();
			Importer.Position.TableCellRowSpan = 1;
			if (Importer.Position.CharacterFormatting.Options.UseBackColor)
				Importer.Position.CharacterFormatting.BackColor = DXColor.Empty;
			Importer.Position.AdditionalIndent = 0;
		}
		protected internal override void BeforeDeleteTagFromStack(int indexOfDeletedTag) {
			bool isIgnored = ((TableTag)(Importer.TagsStack[indexOfDeletedTag].Tag)).TableNotCreated;
			if (TablesDisabled || isIgnored)
				return;
			Importer.TablesImportHelper.FinalizeTableCreation();
		}
		protected override bool ShouldAddParagraph() {
			return !Importer.IsEmptyParagraph;
		}
		#region DeleteOldOpenTag
		protected internal override void DeleteOldOpenTag() {
			if (Tag.ElementType == HtmlElementType.OpenTag) {
				return; 
			}
			int count = Importer.TagsStack.Count;
			for (int i = count - 1; i >= 0; i--) {
				TagBase tag = Importer.TagsStack[i].Tag;
				if (tag is TdTag || tag is ThTag)
					CloseUnclosedTableCellTag(i);
				else if (tag is TrTag)
					CloseUnclosedTableRowTag(i);
				else if (tag is TableTag)
					return;
				continue;
			}
		}
		void CloseUnclosedTableRowTag(int index) {
			TrTag tag = (TrTag)Importer.TagsStack[index].Tag;
			Importer.CloseUnClosedTag(tag, index);
		}
		void CloseUnclosedTableCellTag(int index) {
			TdTag tdTag = (TdTag)Importer.TagsStack[index].Tag;
			Importer.CloseUnClosedTag(tdTag, index);
		}
		#endregion
		internal TableCell ObtainParentCell() {
			TableCell result = null;
			if (Importer.TablesImportHelper.Table == null)
				return result;
			if (Importer.TablesImportHelper.Table.LastRow == null)
				return result;
			int i = Importer.TagsStack.Count - 1;
			while (i >= 0) {
				TagBase tag = Importer.TagsStack[i].Tag;
				if (tag is TdTag || tag is ThTag) {
					result = Importer.TablesImportHelper.Table.LastRow.LastCell;
					return result; 
				}
				TrTag trTag = tag as TrTag;
				if (trTag != null && Importer.TablesImportHelper.Table != null) {
					TdTag cellTag = new TdTag(Importer);
					OpenHtmlTag newtag = new OpenHtmlTag(cellTag, Importer.PieceTable);
					newtag.OldPosition.CopyFrom(Importer.Position);
					FunctionalTagProcess();
					Importer.TagsStack.Insert(i + 1, newtag);
					cellTag.OpenTagProcessCore();
					result = Importer.TablesImportHelper.Table.LastRow.LastCell;
					if (Importer.TablesImportHelper.Table.LastRow.Cells.Count == 1) {
						cellTag.StartParagraphIndex = trTag.StartParagraphIndex;
						result.StartParagraphIndex = cellTag.StartParagraphIndex;
					}
					return result;
				}
				i--;
			}
			return result;
		}
	}
	#endregion
	#region TableCellTag (abstract class)
	public abstract class TableCellTag : TagBase {
		ParagraphIndex startParagraphIndex;
		protected TableCellTag(HtmlImporter importer)
			: base(importer) {
		}
		public ParagraphIndex StartParagraphIndex { get { return startParagraphIndex; } set { startParagraphIndex = value; } }
		public bool TablesDisabled { get { return !Importer.TablesImportHelper.TablesAllowed || !Importer.TablesImportHelper.IsInTable; } }
		protected abstract TableCell CreateTableCell();
		protected internal override void OpenTagProcessCore() {
			if (TablesDisabled)
				return;
			TableCell cell = CreateTableCell();
			this.startParagraphIndex = Importer.TablesImportHelper.FindStartParagraphIndexForCell(Importer.Position.ParagraphIndex);
			cell.StartParagraphIndex = StartParagraphIndex;
		}
		protected internal override void BeforeDeleteTagFromStack(int indexOfDeletedTag) {
			if (TablesDisabled)
				return;
			TableCellTag openTag = (TableCellTag)Importer.TagsStack[indexOfDeletedTag].Tag;
			ParagraphIndex startParagraphIndex = GetStartParagraphIndex(openTag);
			ParagraphIndex endParagraphIndex = GetEndParagraphIndex(openTag, startParagraphIndex);
			Importer.TablesImportHelper.InitializeTableCell(Importer.TablesImportHelper.Table.LastRow.LastCell, startParagraphIndex, endParagraphIndex);
		}
		protected ParagraphIndex GetStartParagraphIndex(TableCellTag openTag) {
			return openTag.StartParagraphIndex;
		}
		protected ParagraphIndex GetEndParagraphIndex(TableCellTag openTag, ParagraphIndex startParagraphIndex) {
			if (Importer.IsEmptyParagraph)
				return GetEndParagraphIndexEmptyLastParagraphCase(openTag, startParagraphIndex);
			else
				return GetEndParagraphIndexNonEmptyLastParagraphCase();
		}
		ParagraphIndex GetEndParagraphIndexNonEmptyLastParagraphCase() {
			return Importer.Position.ParagraphIndex;
		}
		ParagraphIndex GetEndParagraphIndexEmptyLastParagraphCase(TableCellTag openTag, ParagraphIndex startParagraphIndex) {
			ParagraphIndex endParagraphIndex;
			int index = Importer.PieceTable.Paragraphs.Count - 2;
			if (startParagraphIndex == new ParagraphIndex(index))
				endParagraphIndex = GetEndParagraphIndexForEmptyCell(startParagraphIndex);
			else
				endParagraphIndex = GetEndParagraphIndexForCellWithContent(openTag);
			return endParagraphIndex;
		}
		protected virtual ParagraphIndex GetEndParagraphIndexForCellWithContent(TableCellTag openTag) {
			ParagraphIndex endParagraphIndex;
			ParagraphIndex paragraphIndex = Importer.Position.ParagraphIndex;
			if (this.DocumentModel.ActivePieceTable.Paragraphs[paragraphIndex].IsEmpty) {
				endParagraphIndex = paragraphIndex - 1; 
			}
			else { 
				endParagraphIndex = paragraphIndex;
				Importer.SetAppendObjectProperty();
			}
			return endParagraphIndex;
		}
		ParagraphIndex GetEndParagraphIndexForEmptyCell(ParagraphIndex startParagraphIndex) {
			ParagraphIndex endParagraphIndex;
			Importer.SetAppendObjectProperty();
			endParagraphIndex = startParagraphIndex;
			return endParagraphIndex;
		}
	}
	#endregion
	#region TdTag
	public class TdTag : TableCellTag, ICellPropertiesOwner {
		#region AttributeKeywordTranslatorTable
		static AttributeKeywordTranslatorTable attributeTable = AddAttributes();
		static AttributeKeywordTranslatorTable AddAttributes() {
			AttributeKeywordTranslatorTable table = CreateAttributeTable();
			table.Add(ConvertKeyToUpper("headers"), HeadersKeyword);
			table.Add(ConvertKeyToUpper("scope"), ScopeKeyword);
			table.Add(ConvertKeyToUpper("abbr"), AbbrKeyword);
			table.Add(ConvertKeyToUpper("axis"), AxisKeyword);
			table.Add(ConvertKeyToUpper("rowspan"), RowspanKeyword);
			table.Add(ConvertKeyToUpper("colspan"), ColspanKeyword);
			table.Add(ConvertKeyToUpper("nowrap"), NowrapKeyword);
			table.Add(ConvertKeyToUpper("width"), WidthKeyword);
			table.Add(ConvertKeyToUpper("height"), HeightKeyword);
			table.Add(ConvertKeyToUpper("align"), AlignmentKeyword);
			table.Add(ConvertKeyToUpper("valign"), VerticalAlignmentKeyword);
			table.Add(ConvertKeyToUpper("bordercolor"), BorderColorKeyword);
			table.Add(ConvertKeyToUpper("bgcolor"), BackgroundColorKeyword);
			return table;
		}
		#endregion
		#region Handlers
		static internal void HeadersKeyword(HtmlImporter importer, string value, TagBase tag) {
		}
		static internal void ScopeKeyword(HtmlImporter importer, string value, TagBase tag) {
		}
		static internal void AbbrKeyword(HtmlImporter importer, string value, TagBase tag) {
		}
		static internal void AxisKeyword(HtmlImporter importer, string value, TagBase tag) {
		}
		static internal void RowspanKeyword(HtmlImporter importer, string value, TagBase tag) {
			int rowsSpanned;
			if (Int32.TryParse(value, out rowsSpanned)&& rowsSpanned > 1)
				importer.Position.TableCellRowSpan = rowsSpanned;
		}
		static internal void ColspanKeyword(HtmlImporter importer, string value, TagBase tag) {
			TdTag tdTag = (TdTag)tag;
			int columns;
			if (Int32.TryParse(value, out columns) && columns > 1)
				tdTag.CellProperties.ColumnSpan = columns;
		}
		static internal void NowrapKeyword(HtmlImporter importer, string value, TagBase tag) {
			TdTag tdTag = (TdTag)tag;
			tdTag.CellProperties.NoWrap = true;
		}
		static internal void WidthKeyword(HtmlImporter importer, string value, TagBase tag) {
			TdTag tdTag = (TdTag)tag;
			WidthUnitInfo width = ConvertPixelsValueToWidthUnitInfo(importer.DocumentModel.UnitConverter, value);
			if (width.Type != WidthUnitType.Nil) {
				PreferredWidth cellWidth = tdTag.CellProperties.PreferredWidth;
				cellWidth.Value = width.Value;
				cellWidth.Type = width.Type;
			}
		}
		static TrTag FindRowTag(HtmlImporter importer) {
			List<OpenHtmlTag> tagStack = importer.TagsStack;
			int start = tagStack.Count - 2;
			for (int i = start; i > 0; i--) {
				TagBase tag = tagStack[i].Tag;
				TrTag trTag = tag as TrTag;
				if (trTag != null)
					return trTag;
			}
			return null;
		}
		static internal void HeightKeyword(HtmlImporter importer, string value, TagBase tag) {
			TrTag trTag = FindRowTag(importer);
			if (trTag == null)
				return;
			int result;
			LengthValueParser parsedValue = new LengthValueParser(value + "px", importer.DocumentModel.ScreenDpi);
			if (parsedValue.IsDigit && !parsedValue.IsRelativeUnit) {
				DocumentModelUnitConverter converter = importer.DocumentModel.UnitConverter;
				result = Math.Max(0, (int)Math.Round(converter.PointsToModelUnitsF(parsedValue.PointsValue)));
				HeightUnitInfo height = new HeightUnitInfo();
				height.Type = HeightUnitType.Minimum;
				height.Value = result;
				trTag.RowProperties.Height = height;
				importer.Position.RowProperties.Height = height;
			}
		}
		static internal void AlignmentKeyword(HtmlImporter importer, string value, TagBase tag) {
			TdTag tdTag = (TdTag)tag;
			ParagraphAlignment targetValue = ParagraphAlignment.Left;
			if (ReadParagraphAlignment(value, ref targetValue))
				tdTag.Alignment.AlignmentValue = targetValue;
		}
		static internal void VerticalAlignmentKeyword(HtmlImporter importer, string value, TagBase tag) {
			TdTag tdTag = (TdTag)tag;
			tdTag.CellProperties.VerticalAlignment = ReadVerticalAlignment(value);
		}
		static internal void BorderColorKeyword(HtmlImporter importer, string value, TagBase tag) {
			TdTag tdTag = (TdTag)tag;
			SetColorToAllBorders(MarkupLanguageColorParser.ParseColor(value), tdTag.CellProperties);
		}
		public static void SetColorToAllBorders(Color color, TableCellProperties cellProperties) {
			TableCellBorders borders = cellProperties.Borders;
			borders.TopBorder.Color = color;
			borders.BottomBorder.Color = color;
			borders.RightBorder.Color = color;
			borders.LeftBorder.Color = color;
		}
		public static void ImportBorderWidth(DocumentModelUnitConverter unitConverter, string value, HtmlBorderProperty bottomBorder) {
			WidthUnitInfo width = new WidthUnitInfo();
			width = ImportBorderWidthCore(unitConverter, value);
			if (width.Type != WidthUnitType.Nil)
				bottomBorder.Width = width.Value;
		}
		public static void ImportBorderColor(DocumentModelUnitConverter converter, string value, HtmlBorderProperty border) {
			if (String.IsNullOrEmpty(value))
				return;
			Color color = ImportColor(converter, value);
			if (color != DXColor.Empty)
				border.Color = color;
		}
		static internal void BackgroundColorKeyword(HtmlImporter importer, string value, TagBase tag) {
			TdTag tdTag = (TdTag)tag;
			tdTag.CellProperties.BackgroundColor = MarkupLanguageColorParser.ParseColor(value);
		}
		static internal void BackgroundImageKeyword(HtmlImporter importer, string value, TagBase tag) {
		}
		#endregion
		#region Fields
		TableCellProperties cellProperties;
		int tablesCountOnOpen;
		int tableCellRowSpan; 
		bool tdTagWithoutOpenedTable;
		HtmlParagraphAlignment alignment;
		#endregion
		public TdTag(HtmlImporter importer)
			: base(importer) {
			cellProperties = new TableCellProperties(this.Importer.PieceTable, this);
			this.tableCellRowSpan = 1;
			tdTagWithoutOpenedTable = false;
			alignment = new HtmlParagraphAlignment();
		}
		#region Properties
		protected internal override AttributeKeywordTranslatorTable AttributeTable { get { return attributeTable; } }
		public TableCellProperties CellProperties { get { return cellProperties; } }
		public HtmlParagraphAlignment Alignment { get { return alignment; } set { alignment = value; } }
		public bool TdTagWithoutOpenedTable { get { return tdTagWithoutOpenedTable; } set { tdTagWithoutOpenedTable = value; } }
		#endregion
		#region ApplyTagProperties
		protected internal override void ApplyTagProperties() {
			Importer.Position.CellProperties.CopyFrom(CellProperties);
			if (Alignment.UseAlignment) {
				Importer.Position.DefaultAlignment.AlignmentValue = Alignment.AlignmentValue;
				Importer.Position.ParagraphFormatting.Alignment = Alignment.AlignmentValue;
			}
		}
		#endregion
		protected internal override ParagraphFormattingOptions ApplyCssProperties() {
			return base.ApplyCssProperties();
		}
		#region FunctionalTagProcess
		protected internal override void FunctionalTagProcess() {
			ParagraphFunctionalProcess();
		}
		#endregion
		protected internal override void OpenTagProcessCore() {
			if (TablesDisabled) {
				TdTagWithoutOpenedTable = true;
				return;
			}
			base.OpenTagProcessCore();
			this.tablesCountOnOpen = Importer.PieceTable.Tables.Count;
			this.tableCellRowSpan = Importer.Position.TableCellRowSpan;
			Importer.IsEmptyParagraph = true;
		}
		protected override TableCell CreateTableCell() {
			HtmlTablesImportHelper helper = Importer.TablesImportHelper;
			if (helper.Table.LastRow == null)
				helper.CreateNewRow();
			TableCell cell = new TableCell(helper.Table.LastRow);
			cell.Row.Cells.AddInternal(cell);
			ApplyAllCellProperties(cell);
			ApplyHeightToTableRow(cell.Row, Importer.Position.TableCellRowSpan);
			if (helper.Table.Rows.Count == helper.TableInfo.TableCaption.Count + 1) {
				helper.AddCellToSpanCollection(Importer.Position.TableCellRowSpan, Importer.Position.CellProperties.ColumnSpan);
				helper.TableInfo.CaptionColSpan += Importer.Position.CellProperties.ColumnSpan;
			}
			else if(helper.TableInfo.ColumnIndex + Importer.Position.CellProperties.ColumnSpan > helper.TableInfo.CellsRowSpanCollection.Count) {
				helper.ExpandSpanCollection(Importer.Position.TableCellRowSpan, Importer.Position.CellProperties.ColumnSpan);
			}
			return cell;
		}
		void ApplyHeightToTableRow(TableRow tableRow, int rowSpan) {
			TrTag trTag = TdTag.FindRowTag(Importer);
			if (trTag == null || rowSpan > 1)
				return;
			Importer.Position.RowProperties.ApplyPropertiesToRow(tableRow.Properties);
		}
		protected internal override void BeforeDeleteTagFromStack(int indexOfDeletedTag) {
			if (TablesDisabled || Importer.TablesImportHelper.Table.LastRow == null)
				return;
			TdTag openTag = (TdTag)Importer.TagsStack[indexOfDeletedTag].Tag;
			if (openTag.tdTagWithoutOpenedTable)
				return;
			base.BeforeDeleteTagFromStack(indexOfDeletedTag);
			Importer.TablesImportHelper.UpdateFirstRowSpanCollection(openTag.tableCellRowSpan, openTag.CellProperties.ColumnSpan);
			int count = Importer.TagsStack.Count;
			for(int i = count - 1; i> indexOfDeletedTag; i--)
				Importer.CloseUnClosedTag(Importer.TagsStack[i].Tag, i);
		}
		protected internal virtual void ApplyAllCellProperties(TableCell cell) {
			cell.Properties.BeginInit();
			try {
				cell.Properties.CopyFrom(Importer.Position.CellProperties);
				if (Importer.Position.TableCellRowSpan > 1)
					cell.Properties.VerticalMerging = MergingState.Restart;
				if (!cell.Properties.UseBackgroundColor) { 
					Importer.Position.TableProperties.ApplyBackgroundColorToCell(cell.Properties);
					Importer.Position.RowProperties.ApplyBackgroundColorToCell(cell.Properties);
				}
				if (!cell.Properties.UseVerticalAlignment) {
					Importer.Position.RowProperties.ApplyVerticalAlignmentToCell(cell.Properties);
				}
				if (!cell.Properties.UseVerticalAlignment)
					cell.Properties.VerticalAlignment = VerticalAlignment.Center;
				if (!cell.Properties.UsePreferredWidth)
					cell.Properties.PreferredWidth.Type = WidthUnitType.Auto;
			}
			finally {
				cell.Properties.EndInit();
			}
		}
		#region Find end paragraphIndex to innitialize cell
		protected override ParagraphIndex GetEndParagraphIndexForCellWithContent(TableCellTag openTag) {
			TdTag tdOpenTag = openTag as TdTag;
			int tablesCountOnClose = Importer.PieceTable.Tables.Count;
			if (tdOpenTag != null && tdOpenTag.tablesCountOnOpen < tablesCountOnClose)
				return GetParagraphIndexForCellContainsNestedTable();
			return base.GetEndParagraphIndexForCellWithContent(openTag);
		}
		ParagraphIndex GetParagraphIndexForCellContainsNestedTable() {
			ParagraphIndex endParagraphIndex;
			endParagraphIndex = Importer.Position.ParagraphIndex - 1;
			if (endParagraphIndex < ParagraphIndex.Zero || Importer.PieceTable.Paragraphs[endParagraphIndex].IsInCell()) {
				endParagraphIndex++;
				Importer.SetAppendObjectProperty();
			}
			return endParagraphIndex;
		}
		#endregion
		#region DeleteOldOpenTag
		protected internal override void DeleteOldOpenTag() {
			if (this.Tag.ElementType == HtmlElementType.CloseTag) {
				for (int i = Importer.TagsStack.Count - 1; i >= 0; i--) {
					TagBase tag = Importer.TagsStack[i].Tag;
					if (tag is TableTag) {
						Importer.OpenTagIsFoundAndRemoved(Importer.TagsStack[i].Tag);
						return;
					}
					if (tag is TrTag)
						return;
					if (tag is TdTag || tag is ThTag) {
						return;
					}
				}
				return;
			}
			for (int i = Importer.TagsStack.Count - 1; i >= 0; i--) {
				TagBase tag = Importer.TagsStack[i].Tag;
				if (tag is TableTag)
					return; 
				if (tag is TrTag)
					return; 
				if (tag is TdTag || tag is ThTag) {
					TdTag tdTag = (TdTag)Importer.TagsStack[i].Tag;
					Importer.CloseUnClosedTag(tdTag, i);
					return;
				}
			}
			return;
		}
		protected internal override int GetStartIndexAllowedSearchScope() {
			int count = Importer.TagsStack.Count;
			for (int i = count - 1; i >= 0; i--) {
				TagBase tag = Importer.TagsStack[i].Tag;
				if (tag is TrTag)
					return i;
			}
			return base.GetStartIndexAllowedSearchScope();
		}
		#endregion
		#region ICellPropertiesOwner Members
		IndexChangedHistoryItemCore<DocumentModelChangeActions> ICellPropertiesOwner.CreateCellPropertiesChangedHistoryItem(TableCellProperties properties) {
			Debug.Assert(properties == CellProperties);
			return new IndexChangedHistoryItem<DocumentModelChangeActions>(properties.PieceTable, properties);
		}
		#endregion
	}
	#endregion
	#region TrTag
	public class TrTag : TagBase {
		#region AttributeKeywordTranslatorTable
		static AttributeKeywordTranslatorTable attributeTable = AddAttributes();
		static AttributeKeywordTranslatorTable AddAttributes() {
			AttributeKeywordTranslatorTable table = CreateAttributeTable();
			table.Add(ConvertKeyToUpper("align"), AlignmentKeyword);
			table.Add(ConvertKeyToUpper("bgcolor"), BackgroundCellColorKeyword);
			table.Add(ConvertKeyToUpper("bordercolor"), BorderColorKeyword);
			table.Add(ConvertKeyToUpper("valign"), VerticalAlignmentKeyword);
			table.Add(ConvertKeyToUpper("height"), HeightKeyword);
			return table;
		}
		#endregion
		#region Handlers
		static internal void AlignmentKeyword(HtmlImporter importer, string value, TagBase tag) {
			TrTag trTag = (TrTag)tag;
			ParagraphAlignment resultAlignment = ParagraphAlignment.Left;
			if (ReadParagraphAlignment(value, ref resultAlignment))
				trTag.Alignment.AlignmentValue = resultAlignment;
		}
		static internal void BackgroundCellColorKeyword(HtmlImporter importer, string value, TagBase tag) {
			TrTag trTag = (TrTag)tag;
			trTag.RowProperties.BackgroundColor = MarkupLanguageColorParser.ParseColor(value);
		}
		static internal void VerticalAlignmentKeyword(HtmlImporter importer, string value, TagBase tag) {
			TrTag tdTag = (TrTag)tag;
			tdTag.RowProperties.VerticalAlignment = ReadVerticalAlignment(value);
		}
		static internal void BorderColorKeyword(HtmlImporter importer, string value, TagBase tag) {
		}
		static internal void HeightKeyword(HtmlImporter importer, string value, TagBase tag) {
			TrTag trTag = (TrTag)tag;
			if (String.IsNullOrEmpty(value))
				return;
			string valueType = value + "px";
			LengthValueParser par = new LengthValueParser(valueType, importer.DocumentModel.ScreenDpi);
			if (!par.IsDigit || par.PointsValue <= 0)
				return;
			HeightUnitInfo newHeight = new HeightUnitInfo();
			newHeight.Value = (int)Math.Round(tag.DocumentModel.UnitConverter.PointsToModelUnitsF(par.PointsValue));
			newHeight.Type = HeightUnitType.Minimum;
			HeightUnitInfo height = trTag.RowProperties.Height;
			if (height.Value < newHeight.Value)
				trTag.RowProperties.Height = newHeight;
		}
		#endregion
		#region Fields
		HtmlTableRowProperties rowProperties;
		ParagraphIndex startParagraphIndex;
		HtmlParagraphAlignment alignment;
		#endregion
		public TrTag(HtmlImporter importer)
			: base(importer) {
			this.rowProperties = new HtmlTableRowProperties();
			this.startParagraphIndex = ParagraphIndex.Zero;
			this.alignment = new HtmlParagraphAlignment();
		}
		#region Properties
		protected internal override AttributeKeywordTranslatorTable AttributeTable { get { return attributeTable; } }
		public HtmlTableRowProperties RowProperties { get { return rowProperties; } }
		public ParagraphIndex StartParagraphIndex { get { return startParagraphIndex; } set { startParagraphIndex = value; } }
		public HtmlParagraphAlignment Alignment { get { return alignment; } set { alignment = value; } }
		public bool TablesDisabled {
			get {
				return !Importer.TablesImportHelper.TablesAllowed || !Importer.TablesImportHelper.IsInTable;
			}
		}
		#endregion
		#region ApplyTagProperties
		protected internal override void ApplyTagProperties() {
			Importer.Position.RowProperties.CopyFrom(RowProperties);
			if (Alignment.UseAlignment) {
				Importer.Position.DefaultAlignment.AlignmentValue = Alignment.AlignmentValue;
				Importer.Position.ParagraphFormatting.Alignment = Alignment.AlignmentValue;
			}
			RowProperties.ApplyPropertiesToCharacter(Importer.Position.CharacterFormatting);
		}
		#endregion
		#region FunctionalTagProcess
		protected internal override void FunctionalTagProcess() {
			ParagraphFunctionalProcess();
		}
		#endregion
		#region DeleteOldOpenTag
		protected internal override void DeleteOldOpenTag() {
			if (this.Tag.ElementType == HtmlElementType.CloseTag)
				return;
			int count = Importer.TagsStack.Count;
			for (int i = count - 1; i >= 0; i--) {
				TagBase tag = Importer.TagsStack[i].Tag;
				if (tag is TableTag)
					return; 
				if (tag is ThTag || tag is TdTag) {
					TdTag tdTag = (TdTag)Importer.TagsStack[i].Tag;
					Importer.CloseUnClosedTag(tdTag, i);
					continue;
				}
				if (tag is TrTag) {
					TrTag trTag = (TrTag)Importer.TagsStack[i].Tag;
					Importer.CloseUnClosedTag(trTag, i);
					return;
				}
			}
			return;
		}
		#endregion
		protected internal override void OpenTagProcessCore() {
			if (TablesDisabled)
				return;
			StartParagraphIndex = Importer.Position.ParagraphIndex;
			TableRow row = Importer.TablesImportHelper.CreateNewRowOrGetLastEmpty();
			Importer.Position.RowProperties.ApplyPropertiesToRow(row.Properties);
		}
		protected internal override void BeforeDeleteTagFromStack(int indexOfDeletedTag) {
			if (TablesDisabled)
				return;
			TableRow lastRow = Importer.TablesImportHelper.Table.LastRow;
			if (lastRow == null)
				return;
			TrTag trTag = ((TrTag)Importer.TagsStack[indexOfDeletedTag].Tag);
			Importer.Position.RowProperties.CopyFrom(trTag.RowProperties);
			CreateCellWhenAfterOpenTrTagWasContent(trTag, lastRow);
			AdaptParagraphMarkHeightToRowHeight();
		}
		void CreateCellWhenAfterOpenTrTagWasContent(TrTag trTag, TableRow lastRow) {
			if (!Importer.IsEmptyParagraph && lastRow.Cells.Count == 0) {
				TableCell cell = new TableCell(lastRow);
				cell.Row.Cells.AddInternal(cell);
				ParagraphIndex startParagraphIndex = Importer.TablesImportHelper.FindStartParagraphIndexForCell(Importer.Position.ParagraphIndex);
				Importer.TablesImportHelper.InitializeTableCell(Importer.TablesImportHelper.Table.LastRow.LastCell, startParagraphIndex, Importer.Position.ParagraphIndex);
			}
		}
		void AdaptParagraphMarkHeightToRowHeight() {
			TableRow row = Importer.TablesImportHelper.Table.LastRow;
			if (row.Properties.Height.Type != HeightUnitType.Minimum)
				return;
			int sizeInPoints = Math.Max((int)DocumentModel.UnitConverter.ModelUnitsToPointsFRound(row.Properties.Height.Value), 1);
			TableCellCollection cells = row.Cells;
			int cellsCount = cells.Count;
			for (int cellId = 0; cellId < cellsCount; cellId++) {
				TableCell cell = cells[cellId];
				Paragraph paragraph = Importer.DocumentModel.MainPieceTable.Paragraphs[cell.StartParagraphIndex];
				if (cell.StartParagraphIndex == cell.EndParagraphIndex && paragraph.FirstRunIndex == paragraph.LastRunIndex) {
					TextRunBase textRun = Importer.DocumentModel.MainPieceTable.Runs[paragraph.FirstRunIndex];
					textRun.DoubleFontSize = Math.Min(textRun.DoubleFontSize, sizeInPoints * 2);
				}
			}
		}
	}
	#endregion
	#region ThTag
	public class ThTag : TdTag {
		#region AttributeKeywordTranslatorTable
		static AttributeKeywordTranslatorTable attributeTable = AddAttributes();
		static AttributeKeywordTranslatorTable AddAttributes() {
			AttributeKeywordTranslatorTable table = CreateAttributeTable();
			table.Add(ConvertKeyToUpper("headers"), HeadersKeyword);
			table.Add(ConvertKeyToUpper("scope"), ScopeKeyword);
			table.Add(ConvertKeyToUpper("abbr"), AbbrKeyword);
			table.Add(ConvertKeyToUpper("axis"), AxisKeyword);
			table.Add(ConvertKeyToUpper("rowspan"), RowspanKeyword);
			table.Add(ConvertKeyToUpper("colspan"), ColspanKeyword);
			table.Add(ConvertKeyToUpper("nowrap"), NowrapKeyword);
			table.Add(ConvertKeyToUpper("width"), WidthKeyword);
			table.Add(ConvertKeyToUpper("height"), HeightKeyword);
			table.Add(ConvertKeyToUpper("align"), AlignmentKeyword);
			table.Add(ConvertKeyToUpper("valign"), VerticalAlignmentKeyword);
			table.Add(ConvertKeyToUpper("bordercolor"), BorderColorKeyword);
			table.Add(ConvertKeyToUpper("bgcolor"), BackgroundColorKeyword);
			table.Add(ConvertKeyToUpper("background"), BackgroundImageKeyword);
			return table;
		}
		#endregion
		public ThTag(HtmlImporter importer)
			: base(importer) {
		}
		#region Properties
		protected internal override AttributeKeywordTranslatorTable AttributeTable { get { return attributeTable; } }
		#endregion
		#region ApplyTagProperties
		protected internal override void ApplyTagProperties() {
			base.ApplyTagProperties();
			Importer.Position.CharacterFormatting.FontBold = true;
			Importer.Position.ParagraphFormatting.Alignment = ParagraphAlignment.Center;
			Importer.Position.DefaultAlignment.AlignmentValue = ParagraphAlignment.Center;
		}
		#endregion
	}
	#endregion
	#region CaptionTag
	public class CaptionTag : TableCellTag {
		#region AttributeKeywordTranslatorTable
		static AttributeKeywordTranslatorTable attributeTable = AddAttributes();
		static AttributeKeywordTranslatorTable AddAttributes() {
			AttributeKeywordTranslatorTable table = CreateAttributeTable();
			table.Add(ConvertKeyToUpper("align"), AlignmentKeyword);
			return table;
		}
		#endregion
		#region Handlers
		static internal void AlignmentKeyword(HtmlImporter importer, string value, TagBase tag) { return; } 
		#endregion
		#region Fields
		#endregion
		public CaptionTag(HtmlImporter importer) : base(importer) { }
		#region Properties
		protected internal override AttributeKeywordTranslatorTable AttributeTable { get { return attributeTable; } }
		#endregion
		protected internal override void ApplyTagProperties() {
			Importer.Position.CharacterFormatting.FontBold = true;
			Importer.Position.ParagraphFormatting.Alignment = ParagraphAlignment.Center;			
		}
		protected override TableCell CreateTableCell() {
			HtmlTablesImportHelper helper = Importer.TablesImportHelper;
			TableRow row = helper.CreateNewRowOrGetLastEmpty();
			Importer.Position.RowProperties.ApplyPropertiesToRow(row.Properties);
			row.Cells.AddInternal(new TableCell(row));
			TableCell cell = row.LastCell;
			TableCellBorders borders = cell.Properties.Borders;
			cell.Properties.BeginInit();
			borders.TopBorder.Style = BorderLineStyle.None;
			borders.BottomBorder.Style = BorderLineStyle.None;
			borders.LeftBorder.Style = BorderLineStyle.None;
			borders.RightBorder.Style = BorderLineStyle.None;
			cell.Properties.EndInit();
			if (helper.TableInfo.CaptionColSpan > 0)
				cell.ColumnSpan = helper.TableInfo.CaptionColSpan;
			helper.TableInfo.TableCaption.Add(cell);
			return cell;
		}
		#region FunctionalTagProcess
		protected internal override void FunctionalTagProcess() {
			ParagraphFunctionalProcess();
		}
		#endregion
	}
	#endregion
	#region TBodyTag
	public class TBodyTag : TagBase {
		public TBodyTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyTagProperties() {
		}
	}
	#endregion
	#region THeadTag
	public class THeadTag : TagBase {
		public THeadTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyTagProperties() {
		}
	}
	#endregion
	#region TFootTag
	public class TFootTag : TagBase {
		public TFootTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyTagProperties() {
		}
	}
	#endregion
}
