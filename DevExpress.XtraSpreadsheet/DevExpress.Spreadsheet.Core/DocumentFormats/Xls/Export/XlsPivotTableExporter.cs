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

#define EXPORT_STYLE_EXTENSIONS
#define EXPORT_XF_EXTENSIONS
using System;
using System.IO;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Import.Xls;
using DevExpress.XtraExport.Xls;
using DevExpress.XtraSpreadsheet.Internal;
#if !SL
using System.Drawing;
using DevExpress.Office.Services;
using DevExpress.XtraSpreadsheet.Localization;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Export.Xls {
	public class XlsPivotTableExporter : XlsExporterBase {
		#region Fields
		readonly PivotTable pivotTable;
		readonly Worksheet sheet;
		int iCache;
		#endregion
		#region Properties
		PivotTable PivotTable { get { return pivotTable; } }
		Worksheet Sheet { get { return sheet; } }
		#endregion
		public XlsPivotTableExporter(BinaryWriter writer, PivotTable pivotTable, ExportXlsStyleSheet exportStyleSheet, Worksheet sheet, int iCache)
			: base(writer, sheet.Workbook, exportStyleSheet) {
			this.pivotTable = pivotTable;
			this.sheet = sheet;
			this.iCache = iCache;
		}
		public override void WriteContent() {
			WritePivotCore();
			WritePivotFutureOptional();
		}
		#region ABNF -> PIVOTCORE
		protected void WritePivotCore() {
			WritePivotView();
			WritePivotFields();
			WritePivotAxis();
			WritePivotPageAxis();
			WritePivotViewDataItems();
			WritePivotLines();
			WritePivotExtension();
		}
		#endregion
		#region ABNF -> PIVOTFRT //TODO
		protected void WritePivotFutureOptional() {
			WritePivotFutureOptional9();
			if (PivotTable.Fields.Count != 0)
				WritePivotAdditionalInfo();
		}
		#endregion
		#region ABNF -> PIVOTCORE -> SxView //AutoFormatId - ? 
		protected void WritePivotView() {
			XlsCommandPivotView command = new XlsCommandPivotView();
			command.Range = new CellRangeInfo(PivotTable.Range.TopLeft, PivotTable.Range.BottomRight);
			PivotTableLocation location = PivotTable.Location;
			command.FirstDataColumnIndex = location.FirstDataColumn + command.Range.First.Column;
			command.FirstDataRowIndex = location.FirstDataRow + command.Range.First.Row;
			command.FirstRowIndex = location.FirstHeaderRow + command.Range.First.Row;
			command.DefaultDataFieldAxis = pivotTable.DataOnRows ? PivotTableAxis.Row : PivotTableAxis.Column;
			command.NumberOfFields = pivotTable.Fields.Count;
			command.NumberOfRowAxisFields = pivotTable.RowFields.Count;
			command.NumberOfColumnAxisFields = pivotTable.ColumnFields.Count;
			command.NumberOfPageFields = pivotTable.PageFields.Count;
			command.NumberOfDataFields = pivotTable.DataFields.Count;
			command.RowCount = pivotTable.RowItems.Count;
			command.ColumnCount = pivotTable.ColumnItems.Count;
			command.TableName = PivotTable.Name;
			command.DataFieldPosition = PivotTable.DataPosition;
			command.HasRowsGrandTotal = PivotTable.ColumnGrandTotals;
			command.HasColumnsGrandTotal = PivotTable.RowGrandTotals;
			command.HasAutoFormat = PivotTable.UseAutoFormatting;
			command.HasNumberAutoFormat = PivotTable.ApplyNumberFormats;
			command.HasFontAutoFormat = PivotTable.ApplyFontFormats;
			command.HasAlignmentAutoFormat = PivotTable.ApplyAlignmentFormats;
			command.HasBorderAutoFormat = PivotTable.ApplyBorderFormats;
			command.HasPatternAutoFormat = PivotTable.ApplyPatternFormats;
			command.HasWidthHeightAutoFormat = PivotTable.ApplyWidthHeightFormats;
			command.AutoFormat = PivotTable.AutoFormatId > 0 ? PivotTable.AutoFormatId : 1; 
			command.DataName = PivotTable.DataCaption;
			command.CacheIndex = iCache;
			command.Write(StreamWriter);
		}
		#endregion
		#region ABNF -> PIVOTCORE -> PIVOTVD
		protected void WritePivotFields() {
			int index = 0;
			foreach (PivotField pivotField in PivotTable.Fields) {
				WritePivotField(pivotField);
				foreach (PivotItem pivotItem in pivotField.Items) 
					WritePivotItem(pivotItem);
				WritePivotFieldExt(pivotField, index++);
			}
		}
		#endregion
		#region ABNF -> PIVOTCORE -> PIVOTVD -> Sxvd
		protected void WritePivotField(PivotField pivotField) {
			XlsCommandPivotField command = new XlsCommandPivotField();
			command.FieldAxis = pivotField.IsDataField ? ((PivotTableAxis)((short)pivotField.Axis | 8)) : pivotField.Axis;
			command.Subtotal = (int)pivotField.Subtotal;
			int tmpSubtotal = (int)pivotField.Subtotal;
			for (int i = Enum.GetNames(typeof(PivotFieldItemType)).Length - 1; i >= 0; i--) {
				tmpSubtotal >>= 1;
				if ((tmpSubtotal & 1) != 0)
					command.CountSubtotal++;
			}
			command.CountItem = pivotField.Items.Count;
			command.FieldName = pivotField.Name;
			command.Write(StreamWriter);
		}
		#endregion
		#region ABNF -> PIVOTCORE -> PIVOTVD -> SXVI
		protected void WritePivotItem(PivotItem pivotItem) {
			XlsCommandPivotItem command = new XlsCommandPivotItem();
			command.PivotItemType = pivotItem.ItemType;
			command.IsHidden = pivotItem.IsHidden;
			command.IsHideDetails = pivotItem.HideDetails;
			command.IsCalculatedItems = pivotItem.CalculatedMember;
			command.IsMissing = pivotItem.HasMissingValue;
				command.IndexCacheItem = pivotItem.ItemIndex; 
			command.ItemName = pivotItem.ItemUserCaption;
			command.Write(StreamWriter);
		}
		#endregion
		#region ABNF -> PIVOTCORE -> PIVOTVD ->SXVDEx   // ApplyNumberFormat and  DataItemAutoSort ?
		protected void WritePivotFieldExt(PivotField pivotField, int indexField) {
			XlsCommandPivotFieldExt command = new XlsCommandPivotFieldExt();
			command.ShowAllItems = pivotField.ShowItemsWithNoData;
			command.DragToRow = pivotField.DragToRow;
			command.DragToColumn = pivotField.DragToCol;
			command.DragToPage = pivotField.DragToPage;
			command.DragToHide = pivotField.DragOff;
			command.NotDragToData = !pivotField.DragToData;
			command.ServerBased = pivotField.ServerField;
			command.AscendSort = pivotField.SortType == PivotTableSortTypeField.Ascending;
			command.AutoSort = pivotField.SortType != PivotTableSortTypeField.Manual;
			command.AutoShow = pivotField.AutoShow;
			command.TopAutoShow = pivotField.TopAutoShow;
			if (PivotTable.Cache != null && PivotTable.Cache.CacheFields.Count > indexField)
				command.CalculatedField = !String.IsNullOrEmpty(PivotTable.Cache.CacheFields[indexField].Formula);
			command.PageBreaksBetweenItems = pivotField.InsertPageBreak;
			command.HideNewItems = pivotField.HideNewItems;
			command.Outline = pivotField.Outline;
			command.InsertBlankRow = pivotField.InsertBlankRow;
			command.SubtotalAtTop = pivotField.SubtotalTop;
			command.NumberAutoPivotItems = pivotField.ItemPageCount;
			if (!command.AutoSort)
				command.DataItemAutoSort = -1;
			if (pivotField.AutoShow && pivotField.RankBy.HasValue)
				command.RanksAutoShow = pivotField.RankBy.Value;
			else
				command.RanksAutoShow = -1;
			command.SubName = pivotField.SubtotalCaption;
			command.NumberFormatId = ExportStyleSheet.GetNumberFormatId(pivotField.NumberFormatIndex);
			command.Write(StreamWriter);
		}
		#endregion
		#region ABNF -> PIVOTCORE -> 2PIVOTIVD -> SxIvd *Continue
		protected void WritePivotAxis() {
			if (PivotTable.RowItems.Count != 0)  
				WriteRowColumnAxis(PivotTable.RowFields);
			if (pivotTable.ColumnItems.Count != 0) 
				WriteRowColumnAxis(PivotTable.ColumnFields);
		}
		void WriteRowColumnAxis(PivotTableColumnRowFieldIndices listField) {
			XlsCommandPivotAxis firstCommand = new XlsCommandPivotAxis();
			foreach (PivotFieldReference elementRef in listField) {
				XlsPivotAxisReWriter element = new XlsPivotAxisReWriter();
				element.Value = elementRef.FieldIndex;
				firstCommand.Axis.Add(element);
			}
			XlsCommandContinue continueCommand = new XlsCommandContinue();
			using (XlsChunkWriter writer = new XlsChunkWriter(StreamWriter, firstCommand, continueCommand)) {
				int count = firstCommand.Axis.Count;
				for (int i = 0; i < count; i++) {
					if (writer != null)
						writer.BeginRecord(count << 1);
					firstCommand.Axis[i].Write(writer);
				}
			}
		}
		#endregion
		#region ABNF -> PIVOTCORE -> PIVOTPI -> SXPI *Countinue // DropDownId = 0 because we do not write Obj
		protected void WritePivotPageAxis() {
			PivotPageFieldCollection pageList = PivotTable.PageFields;
			XlsCommandPivotPageAxis firstCommand = new XlsCommandPivotPageAxis();
			foreach (PivotPageField pPageField in pageList) {
				XlsPivotPageAxisReWriter intCommand = new XlsPivotPageAxisReWriter();
				intCommand.DropDownId = 0; 
				intCommand.PivotFieldIndex = pPageField.FieldIndex;
				intCommand.PivotItemIndex = pPageField.ItemIndex>= 0 ? pPageField.ItemIndex : 0x7FFD;
				firstCommand.PageAxis.Add(intCommand);
			}
			XlsCommandContinue continueCommand = new XlsCommandContinue();
			using (XlsChunkWriter writer = new XlsChunkWriter(StreamWriter, firstCommand, continueCommand)) {
				int count = firstCommand.PageAxis.Count;
				for (int i = 0; i < count; i++) {
					if (writer != null)
						writer.BeginRecord(count << 1);
					firstCommand.PageAxis[i].Write(writer);
				}
			}
		}
		#endregion
		#region ABNF -> PIVOTCORE -> SXDI
		protected void WritePivotViewDataItems() {
			foreach (PivotDataField dataField in pivotTable.DataFields)
				WritePivotViewDataItem(dataField);
		}
		void WritePivotViewDataItem(PivotDataField dataField) {
			XlsCommandPivotViewDataItem command = new XlsCommandPivotViewDataItem();
			command.PivotFieldIndex = dataField.FieldIndex;
			command.DataItemName = dataField.Name;
			command.DataItemFunction = dataField.Subtotal;
			if (XlsCommandPivotViewDataItem.listRevertPivotShowDataAs.ContainsKey(dataField.ShowDataAs))
				command.ShowDataAs = dataField.ShowDataAs;
			else {
				DocumentModel.LogMessage(LogCategory.Warning, "A PivotTable in this workbook contains data represented using the 'Show Values As' feature. These custom outputs will not be saved, and will be replaced by the original values from the data source.");
				command.ShowDataAs = PivotShowDataAs.Normal;
			}
			command.BaseField = dataField.BaseField;
			if (PivotTable.Fields[dataField.FieldIndex].Items.Count >= dataField.BaseItem)
				command.BaseItem = dataField.BaseItem;
			else
				command.BaseItem = 0x7FFB;
			command.ItemFormat = ExportStyleSheet.GetNumberFormatId(dataField.NumberFormatIndex);
			command.Write(StreamWriter);
		}
		#endregion
		#region ABNF -> PIVOTCORE -> PIVOTLI(SXLI *Continue)
		protected void WritePivotLines() {
			if (pivotTable.RowItems.Count > 0 | pivotTable.ColumnItems.Count > 0) {
				List<IPivotLayoutItem> list = new List<IPivotLayoutItem>();
				list.AddRange(pivotTable.RowItems);
				PreparationListPivotLineItems(list, pivotTable.RowFields.Count, pivotTable.DataOnRows);
				list = new List<IPivotLayoutItem>();
				list.AddRange(pivotTable.ColumnItems);
				PreparationListPivotLineItems(list, pivotTable.ColumnFields.Count, !pivotTable.DataOnRows);
			}
		}
		int[] CleanLineEntry(int[] arrayItem, int size, int startIndex, bool dataOnRows) {
			int value = dataOnRows ? 0x0000 : 0x7FFF;
			int[] lineEntry = arrayItem != null ? arrayItem : new int[size];
			for (int index = lineEntry.Length - 1; index >= startIndex; index--)
				lineEntry[index] = value;
			return lineEntry;
		}
		void PreparationListPivotLineItems<T>(List<T> rowColumnItems, int maxRowColumn, bool dataOnRows) where T : IPivotLayoutItem {
			List<XlsPivotLineItem> list = new List<XlsPivotLineItem>();
			int[] lineEntry = new int[maxRowColumn];
			foreach (IPivotLayoutItem item in rowColumnItems) {
				XlsPivotLineItem element = new XlsPivotLineItem();
				element.ItemType = item.Type;
				element.IsGrand = element.ItemType == PivotFieldItemType.Grand;
				element.IsSubtotal = element.ItemType >= PivotFieldItemType.DefaultValue;
				int countLineEntry = item.RepeatedItemsCount + item.PivotFieldItemIndices.Length;
				if (element.ItemType != PivotFieldItemType.Blank && pivotTable.DataFields.Count > 1 && dataOnRows)
					element.IsMultiDataOnAxis = true;
				if (element.IsGrand) {
					element.PivotItemIndexCount = 0;
					element.CountLineEntry = 1;
					element.IsMultiDataName = element.IsMultiDataOnAxis;
				}
				else {
					element.PivotItemIndexCount = item.RepeatedItemsCount;
					element.CountLineEntry = countLineEntry;
					if(element.IsMultiDataOnAxis && element.IsSubtotal)
						element.IsMultiDataName = true;
				}
				if (PivotTable.DataFields.Count > 0 && !element.IsGrand)
					element.ItemData = item.DataFieldIndex;
				lineEntry = CleanLineEntry(lineEntry, maxRowColumn, item.RepeatedItemsCount, dataOnRows);
				if (!dataOnRows && maxRowColumn > countLineEntry)
					element.IsEmptyItem = true;
				int itemIndex = 0;
				for (int index = item.RepeatedItemsCount; index < countLineEntry; index++)
					lineEntry[index] = item.PivotFieldItemIndices[itemIndex++];
				element.LineEntry.AddRange(lineEntry);
				list.Add(element);
			}
			WritePivotLineItem(list);
		}
		void WritePivotLineItem(List<XlsPivotLineItem> list) {
			using (XlsChunkWriter writer = new XlsChunkWriter(StreamWriter, new XlsCommandPivotLines(), new XlsCommandContinue())) {
				int count = list.Count;
				for (int i = 0; i < count; i++) 
					list[i].Write(writer);
			}
		}
		#endregion
		#region ABNF -> PIVOTCORE -> PIVOTEX
		protected void WritePivotExtension() {
			WritePivotViewProperties();
			PivotSelection pSelection = Sheet.PivotSelection;
			if (pSelection != null && pSelection.HasInitSelection) {
				WritePivotSelection(pSelection);
				PivotArea pivotArea = pSelection.PivotArea;
				WritePivotRule(pivotArea);
			}
			foreach (PivotFormat element in PivotTable.Formats) {
				WritePivotFormats(element);
			}
		}
		#endregion
		#region ABNF -> PIVOTCORE -> PIVOTEX -> SXEx //incomprehensible option
		protected void WritePivotViewProperties() {
			XlsCommandPivotAdditionalProperties command = new XlsCommandPivotAdditionalProperties();
			command.NumberSxFormat = PivotTable.Formats.Count;
			command.ErrorMessage = PivotTable.ErrorCaption;
			command.DisplayNull = PivotTable.MissingCaption;
			command.Tag = PivotTable.Tag;
			command.NumberSxSelect = Sheet.PivotSelection.HasInitSelection ? 1 : 0;
			command.NumberRowPage = PivotTable.Location.RowPageCount;
			command.NumberColumnPage = PivotTable.Location.ColumnPageCount;
			command.IsAcrossPageLay = PivotTable.PageOverThenDown;
			command.WrapPage = pivotTable.PageWrap; 
			command.IsEnableWizard = pivotTable.EnableWizard;
			command.IsEnableDrillDown = pivotTable.EnableDrill;
			command.IsEnableFieldDialog = pivotTable.EnableFieldProperties;
			command.IsPreserveFormatting = pivotTable.PreserveFormatting;
			command.IsMergeLabels = pivotTable.MergeItem;
			command.IsDisplayErrorString = pivotTable.ShowError;
			command.IsDisplayNullString = pivotTable.ShowMissing;
			command.IsSubtotalHiddenPageItems = pivotTable.SubtotalHiddenItems;
			command.PageFieldStyle = pivotTable.PageStyle;
			command.TableStyle = pivotTable.PivotTableStyle;
			command.VacateStyle = pivotTable.VacatedStyle;
			command.Write(StreamWriter);
		}
		#endregion
		#region ABNF -> PIVOTCORE -> PIVOTEX -> PIVOTSELECT -> SxSelect
		protected void WritePivotSelection(PivotSelection pSelection) {
			XlsCommandPivotSelection command = new XlsCommandPivotSelection();
			command.ActivePane = pSelection.Pane;
			command.SelectionAxis = pSelection.Axis;
			command.Dimension = pSelection.Dimension;
			command.StartLines = pSelection.Start;
			command.CountSelection = pSelection.CountSelection;
			command.Maximum = pSelection.Maximum;
			command.Minimum = pSelection.Minimum;
			command.ActiveRow = pSelection.ActiveRow;
			command.ActiveColumn = pSelection.ActiveColumn;
			command.PreviousRow = pSelection.PreviousRow;
			command.PreviousColumn = pSelection.PreviousColumn;
			command.CountClick = pSelection.CountClick;
			command.IsLabelOnly = pSelection.IsLabel;
			command.IsDataOnly = pSelection.IsDataSelection;
			command.IsToggleDataHeader = pSelection.IsShowHeader;
			command.IsExtendable = pSelection.IsExtendable;
			command.Write(StreamWriter);
		}
		#endregion
		#region ABNF -> PIVOTCORE -> PIVOTEX -> PIVOTSELECT -> PIVOTRULE(SxRule)
		protected void WritePivotRule(PivotArea pivotArea) {
			XlsCommandPivotRule command = new XlsCommandPivotRule();
			if (pivotArea.FieldPosition.HasValue)
				command.FieldPosition = pivotArea.FieldPosition.Value;
			if (pivotArea.References.Count > 0)
				command.FieldIndex = 0xFF;
			else
				command.FieldIndex = pivotArea.Field.HasValue ? pivotArea.Field.Value : 0xFF;
			command.Axis = pivotArea.Axis;
			command.TypeRule = pivotArea.Type;
			if (pivotArea.Range != null)
				command.IsAreaIncludedRule = true;
			command.IsDataOnly = pivotArea.IsDataOnly;
			command.IsLabelOnly = pivotArea.IsLabelOnly;
			command.IsGrandRow = pivotArea.IsGrandRow;
			command.IsGrandColumn = pivotArea.IsGrandColumn;
			command.IsCacheBased = pivotArea.IsCacheIndex;
			if (pivotArea.IsGrandColumn)
				command.IsGrandColumnCreated = true;
			if (pivotArea.IsGrandRow)
				command.IsGrandRowCreated = true;
			command.CountFilter = pivotArea.References.Count;
			command.Range = pivotArea.Range;
			command.Write(StreamWriter);
			foreach (PivotAreaReference reference in pivotArea.References) {
				WritePivotFilter(reference);
				WritePivotItemReferences(reference.SharedItemsIndex);
			}
		}
		#endregion
		#region ABNF -> PIVOTCORE -> PIVOTEX -> PIVOTSELECT -> PIVOTRULE -> PRFILTER(SxFilt)
		protected void WritePivotFilter(PivotAreaReference reference) {
			XlsCommandPivotFilter command = new XlsCommandPivotFilter();
			if (reference.Field.HasValue) {
				command.FieldIndex = (int)reference.Field.Value;
				if (command.FieldIndex == PivotTable.ValuesFieldFakeIndex)
					command.Axis = PivotTable.DataOnRows ? PivotTableAxis.Row : PivotTableAxis.Column;
				else 
					command.Axis = PivotTable.Fields[command.FieldIndex].Axis;
				if (reference.IsByPosition) {
					int result = 0;
					switch(command.Axis) {
						case PivotTableAxis.Row:
							result = PivotTable.RowFields.GetIndexElementByFieldIndex(command.FieldIndex);
							break;
						case PivotTableAxis.Column:
							result = PivotTable.ColumnFields.GetIndexElementByFieldIndex(command.FieldIndex);
							break;
						case PivotTableAxis.Page:
							result = PivotTable.PageFields.GetIndexElementByFieldIndex(command.FieldIndex);
							break;
						case PivotTableAxis.Value:
							result = PivotTable.DataFields.GetIndexElementByFieldIndex(command.FieldIndex);
							break;
					}
					if (result != -1)
						command.PivotFieldAxis = result;
				}
			}
			if (reference.Subtotal == PivotFieldItemType.Blank)
				command.FilterSubtotal = PivotFieldItemType.Data;
			else
				command.FilterSubtotal = reference.Subtotal;
			command.IsSelected = reference.IsSelected;
			command.CountItem = reference.SharedItemsIndex.Count;
			command.Write(StreamWriter);
		}
		#endregion
		#region ABNF -> PIVOTCORE -> PIVOTEX -> PIVOTSELECT -> PIVOTRULE -> PRFILTER(SxItm *Continue)
		protected void WritePivotItemReferences(List<long> itemsIndex) {
			XlsCommandPivotItemReferences firstCommand = new XlsCommandPivotItemReferences();
			foreach (long index in itemsIndex) { 
				XlsPivotFilterItemReWriter element = new XlsPivotFilterItemReWriter();
				element.Value = (int)index;
				firstCommand.FilterItems.Add(element);
			}
			XlsCommandContinue continueCommand = new XlsCommandContinue();
			using (XlsChunkWriter writer = new XlsChunkWriter(StreamWriter, firstCommand, continueCommand)) {
				int count = firstCommand.FilterItems.Count;
				for (int i = 0; i < count; i++) {
					if (writer != null)
						writer.BeginRecord(count << 1);
					firstCommand.FilterItems[i].Write(writer);
				}
			}
		}
		#endregion
		#region ABNF -> PIVOTCORE -> PIVOTEX -> PIVOTFORMAT
		protected void WritePivotFormats(PivotFormat pivotFormat) {
			DifferentialFormat dxf = (DifferentialFormat)pivotFormat.Info;
			DxfN12ListInfo dxfN12 = DxfN12ListInfo.FromDifferentialFormat(dxf);
			WritePvotFormat(pivotFormat, dxfN12.GetSize());
			WritePivotRule(pivotFormat.PivotArea); 
			if (pivotFormat.FormatAction != FormatAction.Blank)
				WriteDifferentialFormatting(dxfN12);
		}
		#endregion
		#region ABNF -> PIVOTCORE -> PIVOTEX -> PIVOTFORMAT -> SxFormat
		void WritePvotFormat(PivotFormat element, short size) {
			XlsCommandPivotFormat command = new XlsCommandPivotFormat();
			command.IsFormatApply = element.FormatAction != FormatAction.Blank;
			if (!command.IsFormatApply)
				command.DifferentialFormatLength = 0;
			else
				command.DifferentialFormatLength = size; 
			command.Write(StreamWriter);  
		}
		#endregion
		#region ABNF -> PIVOTCORE -> PIVOTEX -> PIVOTFORMAT -> SxDXF
		void WriteDifferentialFormatting(DxfN12ListInfo dxfN12) {
			XlsCommandPivotDifferentialFormat command = new XlsCommandPivotDifferentialFormat();
			command.DifferentialFormat = dxfN12;
			command.Write(StreamWriter);  
		}
		#endregion
		#region ABNF -> PIVOTFRT -> PIVOTFRT9 //TODO
		protected void WritePivotFutureOptional9() {
			PivotViewExtTag();
			if(PivotTable.Hierarchies.Count > 0)
				WritePivotViewEx();
			WritePivotViewExt9();
		}
		#endregion
		#region ABNF -> PIVOTFRT -> PIVOTFRT9(QsiSXTag)
		protected void PivotViewExtTag() {
			XlsCommandPivotViewExtTag command = new XlsCommandPivotViewExtTag();
			command.IsPivotTable = true;
			if (PivotTable.CreatedVersion > 2 || PivotTable.Cache.CreatedVersion > 2) {
				command.IsEnableRefresh = false;
				command.IsInvalid = true;
			}
			else {
				command.IsEnableRefresh = PivotTable.Cache.EnableRefresh;
				command.IsInvalid = PivotTable.Cache.Invalid;
			}
			command.IsOlapPivotTable = false;
			command.IsDisableNoData = !PivotTable.ShowDropZones;
			command.IsHideTotAnnotation = PivotTable.AsteriskTotals;
			command.IsIncludeEmptyRow = PivotTable.ShowEmptyRow;
			command.IsIncludeEmptyColumn = PivotTable.ShowEmptyColumn;
			command.LowRecalculateVersion = PivotTable.MinRefreshableVersion;
			command.VersionLastUpdated = PivotTable.UpdatedVersion;
			command.Name = PivotTable.Name;
			command.Write(StreamWriter);
		}
		#endregion
		#region ABNF -> PIVOTFRT -> PIVOTFRT9 -> PIVOTVIEWEX //TODO
		protected void WritePivotViewEx() {
			WritePivotViewExt();
		}
		#endregion
		#region ABNF -> PIVOTFRT -> PIVOTFRT9 -> PIVOTVIEWEX -> SxViewEx
		protected void WritePivotViewExt() {
			XlsCommandPivotViewExt command = new XlsCommandPivotViewExt();
			command.CountHierarchyRecords = PivotTable.Hierarchies.Count;
			command.CountExtPageAxisRecords = PivotTable.PageFields.Count;
			command.CountExtPivotFieldRecords = 0; 
			command.Write(StreamWriter);
		}
		#endregion
		#region ABNF -> PIVOTFRT -> PIVOTFRT9 -> PIVOTVIEWEX -> PIVOTTH(SXTH *ContinueFrt) TODO
		#endregion
		#region ABNF -> PIVOTFRT -> PIVOTFRT9(SXViewEx9)
		protected void WritePivotViewExt9() {
			XlsCommandPivotViewExt9 command = new XlsCommandPivotViewExt9();
			command.IsPrintTitles = PivotTable.FieldPrintTitles;
			command.IsLineMode = PivotTable.Outline;
			command.IsPrintItemTitles = PivotTable.ItemPrintTitles;
			command.AutoFormat = PivotTable.AutoFormatId > 0 ? PivotTable.AutoFormatId : 1; 
			command.GrandName = PivotTable.GrandTotalCaption;
			command.IsSupport = PivotTable.CreatedVersion > 2;
			command.Write(StreamWriter);
		}
		#endregion
		#region ABNF -> PIVOTFRT -> PIVOTADDL  //TODO
		protected void WritePivotAdditionalInfo() {
			WriteAddlCommand(WritePivotAddlId(PivotTable.Name, new XlsPivotAddlViewId()));
			WriteSxAddlVersion10();
			if (PivotTable.CreatedVersion > 2 || (PivotTable.Cache != null && PivotTable.Cache.CreatedVersion > 2)) {
				WriteSxAddlVersion12();
				if (PivotTable.PageFields.Count > 0)
					WritePivotAddlViewCalcMember();
				if (PivotTable.Hierarchies.Count > 0)
					WritePivotAddlHierarchies();
				if (PivotTable.Fields.Count > 0)
					WritePivotAddlFields();
			}
			WriteSxAddlTableStyleClient(PivotTable.StyleInfo);
			WritePivotAddlVersionUpdate(0x02, new XlsPivotAddlViewVersionUpdate());
			WritePivotAddlFilters12(PivotTable.Filters);
			WritePivotAddlVersionUpdate(0xFF, new XlsPivotAddlViewVersionUpdate());
			WriteAddlCommand(new XlsPivotAddlViewEnd());
		}
		void WriteAddlCommand(IXlsPivotAddl entity) {
			XlsCommandPivotAddl command = new XlsCommandPivotAddl();
			command.Data = entity;
			command.Write(StreamWriter);
		}
		IXlsPivotAddl WritePivotAddlId(String name, XlsPivotAddlStringBase command) {
			command.TotalCount = String.IsNullOrEmpty(name) ? 0 : name.Length;
			command.Value = name;
			return command;
		}
		void WritePivotAddlVersionUpdate(byte version, XlsPivotAddlVersionUpdateBase command) {
			command.Version = version;
			WriteAddlCommand(command);
		}
		void WriteSxAddlVersion10() {
			XlsPivotAddlViewVersion10 command = new XlsPivotAddlViewVersion10();
			if (PivotTable.Cache != null)
				command.Version = PivotTable.Cache.CreatedVersion;
			else
				command.Version = PivotTable.CreatedVersion;
			command.DisplayImmediateItems = PivotTable.ItemPrintTitles;
			command.EnableDataEditing = PivotTable.EditData;
			command.DisableFieldList = PivotTable.DisableFieldList;
			command.NotViewCalculatedMembers = !PivotTable.ShowCalcMbrs;
			command.NotVisualTotals = !PivotTable.VisualTotals;
			command.PageMultipleItemLabel = PivotTable.ShowMultipleLabel;
			command.TensorFillColorValue = false; 
			command.HideDropDownData = !PivotTable.ShowDataDropDown;
			WriteAddlCommand(command);
		}
		void WriteSxAddlVersion12() {
			XlsPivotAddlViewVersion12 command = new XlsPivotAddlViewVersion12();
			command.DefaultCompact = PivotTable.Compact;
			command.DefaultOutline = PivotTable.Outline;
			command.OutlineData = PivotTable.OutlineData;
			command.CompactData = PivotTable.CompactData;
			command.NewDropZones = !PivotTable.GridDropZones;
			command.Published = PivotTable.Published;
			command.TurnOffImmersive = !PivotTable.Immersive;
			command.SingleFilterPerField = !PivotTable.MultipleFieldFilters;
			command.NonDefaultSortInFieldList = PivotTable.FieldListSortAscending;
			command.DontUseCustomLists = !PivotTable.CustomListSort;
			command.HideDrillIndicators = !PivotTable.ShowDrill;
			command.PrintDrillIndicators = PivotTable.PrintDrill;
			command.MemPropsInTips = PivotTable.ShowMemberPropertyTips;
			command.NoPivotTips = !PivotTable.ShowDataTips;
			if (PivotTable.Indent != 1)
				command.IndentInc = PivotTable.Indent;
			command.NoHeaders = !PivotTable.ShowHeaders;
			WriteAddlCommand(command);
		}
		#region ------========TODO========------
		void WritePivotAddlViewCalcMember() { }
		void WritePivotAddlHierarchies() { }
		#endregion
		void WritePivotAddlFields() {
			int indexCache = 0;
			foreach (PivotField field in PivotTable.Fields) {
				if (!field.ShowDropDowns)
					WritePivotAddlFieldVer10(field);
				if (PivotTable.Cache != null) {
					WritePivotAddlFieldVer12(field, PivotTable.Cache.CacheFields[indexCache]);
					indexCache++;
				}
			}
		}
		void WritePivotAddlFieldVer10(PivotField field) {
			WriteAddlCommand(WritePivotAddlId(field.Name, new XlsPivotAddlFieldId()));
			XlsPivotAddlFieldVersion10 command = new XlsPivotAddlFieldVersion10();
			command.HideDropDown = !field.ShowDropDowns;
			WriteAddlCommand(command);
			WriteAddlCommand(new XlsPivotAddlFieldEnd());
		}
		void WritePivotAddlFieldVer12(PivotField field, IPivotCacheField cacheField) {
			WriteAddlCommand(WritePivotAddlId(String.IsNullOrEmpty(field.Name) ? cacheField.Name : field.Name, new XlsPivotAddlField12Id()));
			WritePivotAddlField12Version12(field);
			WritePivotAddlVersionUpdate(0x02, new XlsPivotAddlField12VersionUpdate());
			WritePivotAddlVersionUpdate(0xFF, new XlsPivotAddlField12VersionUpdate());
			WriteAddlCommand(new XlsPivotAddlField12End());
		}
		void WritePivotAddlField12Version12(PivotField field) {
			XlsPivotAddlField12Version12 command = new XlsPivotAddlField12Version12();
			command.HiddenLevel = field.HiddenLevel;
			command.UseMemPropCaption = field.ShowPropAsCaption;
			command.Compact = field.Compact;
			command.NotAutoSortDeffered = field.NonAutoSortDefault;
			command.FilterInclusive = !field.IncludeNewItemsInFilter;
			WriteAddlCommand(command);
		}
		void WriteSxAddlTableStyleClient(PivotTableStyleInfo styleInfo) {
			XlsPivotAddlViewTableStyleClient command = new XlsPivotAddlViewTableStyleClient();
			if (styleInfo.HasShowLastColumn)
				command.LastColumn = styleInfo.ShowLastColumn;
			command.RowStrips = styleInfo.ShowRowStripes;
			command.ColumnStrips = styleInfo.ShowColumnStripes;
			command.RowHeaders = styleInfo.ShowRowHeaders;
			command.ColumnHeaders = styleInfo.ShowColumnHeaders;
			if (styleInfo.Style != null || !styleInfo.StyleName.Equals(TableStyleName.DefaultStyleName.Name))
				command.Name = styleInfo.StyleName;
			WriteAddlCommand(command);
		}
		void WritePivotAddlFilter12Id(int count, XlsPivotAddlIntBase command) {
			command.Value = count;
			WriteAddlCommand(command);
		}
		void WritePivotAddlFilters12(PivotFilterCollection filters) {
			if (filters.Count > 0) {
				WritePivotAddlFilter12Id(filters.Count, new XlsPivotAddlFilters12Id());
				foreach (PivotFilter filter in filters)
					WritePivotAddlFilter12(filter);
				WriteAddlCommand(new XlsPivotAddlFilters12End());
			}
		}
		void WritePivotAddlFilter12Filter(PivotFilter filter) {
			XlsPivotAddlFilter12Filter command = new XlsPivotAddlFilter12Filter();
			command.FieldIndex = filter.FieldIndex;
			command.FilterType = filter.FilterType;
			if (filter.FilterType < PivotFilterType.CaptionEqual || filter.FilterType > PivotFilterType.CaptionNotBetween)
				command.MemberPropertyIndex = -1;
			else if (filter.MemberPropertyFieldId.HasValue)
				command.MemberPropertyIndex = filter.MemberPropertyFieldId.Value;
			if (filter.MeasureFieldIndex.HasValue)
				command.DataItemIndex = filter.MeasureFieldIndex.Value;
			if (filter.MeasureIndex.HasValue)
				command.HierarchyIndex = filter.MeasureIndex.Value;
			else
				command.HierarchyIndex = -1;
			WriteAddlCommand(command);
		}
		void WritePivotAddlFilter12XlsFilter(PivotFilter filter){
			XlsPivotAddlFilter12XlsFilter command = new XlsPivotAddlFilter12XlsFilter();
			AutoFilterColumnCollection listCriteria = filter.AutoFilter.FilterColumns;
			bool stringValue = false;
			foreach (AutoFilterColumn autoFilter in listCriteria) {
				if (filter.FilterType > PivotFilterType.Unknown && filter.FilterType < PivotFilterType.CaptionEqual) {
					command.FilterType = filter.FilterType;
					command.Top10 = new XlsPivotFilterTop10();
					command.Top10.Top10Type = (XlsPivotFilterTop10Type)((int)autoFilter.Top10FilterType + 1);
					command.Top10.IsTop = autoFilter.FilterByTopOrder;
					command.Top10.Value = autoFilter.TopOrBottomDoubleValue;
					command.Top10.Value = autoFilter.FilterDoubleValue;
					command.NumberOfCriteria = 1;
				}
				else {
					command.Criteria = new XlsPivotFilterCriteria();
					if(autoFilter.CustomFilters.Count > 0) {
						command.Criteria.JoinType = XlsPivotOperatorJoinType.None;
						command.NumberOfCriteria = autoFilter.CustomFilters.Count;
						stringValue = !String.IsNullOrEmpty(autoFilter.CustomFilters[0].Value);
						command.FilterType = filter.FilterType;
						FillOperator(command.Criteria.First, autoFilter.CustomFilters[0], stringValue);
						if(autoFilter.CustomFilters.Count > 1){
							command.Criteria.JoinType = autoFilter.CustomFilters.CriterionAnd ? XlsPivotOperatorJoinType.And : XlsPivotOperatorJoinType.Or;
							FillOperator(command.Criteria.Second, autoFilter.CustomFilters[1], autoFilter.CustomFilters[0].NumericValue.IsEmpty);
						}
					}
					else if (autoFilter.FilterCriteria.Filters.Count > 0) {
						stringValue = true;
						command.Criteria.JoinType = XlsPivotOperatorJoinType.None;
						command.NumberOfCriteria = autoFilter.FilterCriteria.Filters.Count;
						if (autoFilter.FilterCriteria.Filters.Count > 1)
							command.Criteria.JoinType = autoFilter.CustomFilters.CriterionAnd ? XlsPivotOperatorJoinType.And : XlsPivotOperatorJoinType.Or;
					}
				}
			}
			WriteAddlCommand(command);
			if (stringValue)
				WritePivotAddlFilter12XlsFilterValue(listCriteria);
		}
		void WritePivotAddlFilter12XlsFilterValue(AutoFilterColumnCollection listCriteria) {
			foreach(AutoFilterColumn autoFilter in listCriteria)
				if (autoFilter.CustomFilters.Count > 0) {
					WriteAddlCommand(WritePivotAddlId(autoFilter.CustomFilters[0].Value, new XlsPivotAddlFilter12XlsFilterValue1()));
					if (autoFilter.CustomFilters.Count > 1)
						WriteAddlCommand(WritePivotAddlId(autoFilter.CustomFilters[1].Value, new XlsPivotAddlFilter12XlsFilterValue2()));
				}
				else if (autoFilter.FilterCriteria.Filters.Count > 0) {
					WriteAddlCommand(WritePivotAddlId(autoFilter.FilterCriteria.Filters[0], new XlsPivotAddlFilter12XlsFilterValue1()));
					if (autoFilter.FilterCriteria.Filters.Count > 1)
						WriteAddlCommand(WritePivotAddlId(autoFilter.FilterCriteria.Filters[1], new XlsPivotAddlFilter12XlsFilterValue2()));
				}
		}
		void FillOperator(XlsPivotDataOperator dataOperator, CustomFilter filter, bool isValue) {
			if (!isValue) {
				dataOperator.Value = filter.NumericValue.NumericValue;
				dataOperator.ComparisonType = XlsPivotDataComparisonType.Numeric;
			}
			else {
				dataOperator.ComparisonType = XlsPivotDataComparisonType.String;
				dataOperator.IsSimpleComparison = CustomFilter.ContainsWildcardCharacters(filter.Value);
			}
			dataOperator.ComparisonOperation = (XlsPivotDataComparisonOperation)((int)filter.FilterOperator);
		}
		void WritePivotAddlFilter12(PivotFilter filter) {
			WritePivotAddlFilter12Id(filter.PivotFilterId, new XlsPivotAddlFilter12Id());
			WritePivotAddlFilter12Filter(filter);
			if (!String.IsNullOrEmpty(filter.Name))
				WriteAddlCommand(WritePivotAddlId(filter.Name, new XlsPivotAddlFilter12Caption()));
			if (!String.IsNullOrEmpty(filter.Description))
				WriteAddlCommand(WritePivotAddlId(filter.Description, new XlsPivotAddlFilter12FilterDesc()));
			if (!String.IsNullOrEmpty(filter.LabelPivot))
				WriteAddlCommand(WritePivotAddlId(filter.LabelPivot, new XlsPivotAddlFilter12FilterValue1()));
			if (!String.IsNullOrEmpty(filter.LabelPivotFilter))
				WriteAddlCommand(WritePivotAddlId(filter.LabelPivotFilter, new XlsPivotAddlFilter12FilterValue2()));
			WritePivotAddlFilter12XlsFilter(filter);
			WriteAddlCommand(new XlsPivotAddlFilter12End());
		}
		#endregion
	}
}
