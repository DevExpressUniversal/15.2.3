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

using DevExpress.Office;
using DevExpress.Utils.Zip;
using DevExpress.XtraSpreadsheet.Import.OpenXml;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Model.History;
using System;
using System.Collections.Generic;
using System.Xml;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		protected internal void GeneratePivotTableRelations() {
			PivotTableCollection tables = ActiveSheet.PivotTables;
			for (int i = 0; i < tables.Count; ++i) {
				PivotTable table = tables[i];
				++this.pivotTableCounter;
				if (CheckExternalWorksheetSource(table.Cache))
					continue;
				string tableFileName = string.Format("pivotTable{0}.xml", this.pivotTableCounter);
				OpenXmlRelationCollection relations = SheetRelationsTable[ActiveSheet.Name];
				string id = GenerateIdByCollection(relations);
				relations.Add(new OpenXmlRelation(id, "../pivotTables/" + tableFileName, RelsPivotTablesNamepace));
				PivotTablesPathsTable.Add(table, tableFileName);
				PivotTablesRelationId.Add(table, id);
			}
		}
		protected internal virtual void AddPivotTablesPackageContent() {
			int pivotCachesCounter = 0;
			foreach (KeyValuePair<PivotTable, string> pair in PivotTablesPathsTable) {
				PivotTable table = pair.Key;
				string tableFileName = pair.Value;
				string tablePath = @"xl\pivotTables\" + tableFileName;
				string tableRelationPath = @"xl\pivotTables\_rels\" + tableFileName + ".rels";
				SetActivePivotTable(table);
				Builder.OverriddenContentTypes.Add("/xl/pivotTables/" + tableFileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.pivotTable+xml");
				AddPackageContent(tablePath, ExportPivotTablesContent());
				int cacheIndex = Workbook.PivotCaches.IndexOf(table.Cache);
				++pivotCachesCounter;
				string cacheFileName = PivotCachesPathsTable[cacheIndex];
				string id = string.Format("rId{0}", pivotCachesCounter);
				currentExternalReferenceRelation = new OpenXmlRelation(id, "../pivotCache/" + cacheFileName, RelsPivotCacheDefinitionNamepace);
				AddPackageContent(tableRelationPath, ExportExternalFilePathRelation());
			}
		}
		#region Export PivotTable
		protected internal virtual CompressedStream ExportPivotTablesContent() {
			return CreateXmlContent(GeneratePivotTablesXmlContent);
		}
		protected internal virtual void GeneratePivotTablesXmlContent(XmlWriter writer) {
			DocumentContentWriter = writer;
			GeneratePivotTableContent();
		}
		protected internal virtual void GeneratePivotTableContent() {
			PivotTable pivotTable = ActivePivotTable;
			WriteShStartElement("pivotTableDefinition");
			try {
				#region Attribute
				WriteStringAttr(null, "xmlns", null, SpreadsheetNamespaceConst);
				WriteStringValue("name", pivotTable.Name);
				int cacheIndex = Workbook.PivotCaches.IndexOf(pivotTable.Cache);
				WriteIntValue("cacheId", cacheIndex);
				WriteBoolValue("dataOnRows", pivotTable.DataOnRows, false);
				WriteIntValue("dataPosition", pivotTable.DataPosition, pivotTable.DataPosition != -1);
				WriteBoolValue("applyNumberFormats", pivotTable.ApplyNumberFormats);
				WriteBoolValue("applyBorderFormats", pivotTable.ApplyBorderFormats);
				WriteBoolValue("applyFontFormats", pivotTable.ApplyFontFormats);
				WriteBoolValue("applyPatternFormats", pivotTable.ApplyPatternFormats);
				WriteBoolValue("applyAlignmentFormats", pivotTable.ApplyAlignmentFormats);
				WriteBoolValue("applyWidthHeightFormats", pivotTable.ApplyWidthHeightFormats);
				WriteStringValue("dataCaption", pivotTable.DataCaption, !String.IsNullOrEmpty(pivotTable.DataCaption));
				WriteStringValue("grandTotalCaption", pivotTable.GrandTotalCaption, !String.IsNullOrEmpty(pivotTable.GrandTotalCaption));
				WriteStringValue("errorCaption", pivotTable.ErrorCaption, !String.IsNullOrEmpty(pivotTable.ErrorCaption));
				WriteBoolValue("showError", pivotTable.ShowError, false);
				WriteStringValue("missingCaption", pivotTable.MissingCaption, !String.IsNullOrEmpty(pivotTable.MissingCaption));
				WriteBoolValue("showMissing", pivotTable.ShowMissing, true);
				WriteStringValue("pageStyle", pivotTable.PageStyle, !String.IsNullOrEmpty(pivotTable.PageStyle));
				WriteStringValue("pivotTableStyle", pivotTable.PivotTableStyle, !String.IsNullOrEmpty(pivotTable.PivotTableStyle));
				WriteStringValue("vacatedStyle", pivotTable.VacatedStyle, !String.IsNullOrEmpty(pivotTable.VacatedStyle));
				WriteStringValue("tag", pivotTable.Tag, !String.IsNullOrEmpty(pivotTable.Tag));
				WriteIntValue("updatedVersion", pivotTable.UpdatedVersion, 0);
				WriteIntValue("minRefreshableVersion", pivotTable.MinRefreshableVersion, 0);
				WriteBoolValue("asteriskTotals", pivotTable.AsteriskTotals, false);
				WriteBoolValue("showItems", pivotTable.ShowItems, true);
				WriteBoolValue("editData", pivotTable.EditData, false);
				WriteBoolValue("disableFieldList", pivotTable.DisableFieldList, false);
				WriteBoolValue("showCalcMbrs", pivotTable.ShowCalcMbrs, true);
				WriteBoolValue("visualTotals", pivotTable.VisualTotals, true);
				WriteBoolValue("showMultipleLabel", pivotTable.ShowMultipleLabel, true);
				WriteBoolValue("showDataDropDown", pivotTable.ShowDataDropDown, true);
				WriteBoolValue("showDrill", pivotTable.ShowDrill, true);
				WriteBoolValue("printDrill", pivotTable.PrintDrill, false);
				WriteBoolValue("showMemberPropertyTips", pivotTable.ShowMemberPropertyTips, true);
				WriteBoolValue("showDataTips", pivotTable.ShowDataTips, true);
				WriteBoolValue("enableWizard", pivotTable.EnableWizard, true);
				WriteBoolValue("enableDrill", pivotTable.EnableDrill, true);
				WriteBoolValue("enableFieldProperties", pivotTable.EnableFieldProperties, true);
				WriteBoolValue("preserveFormatting", pivotTable.PreserveFormatting, true);
				WriteBoolValue("useAutoFormatting", pivotTable.UseAutoFormatting, false);
				WriteIntValue("pageWrap", pivotTable.PageWrap, 0);
				WriteBoolValue("pageOverThenDown", pivotTable.PageOverThenDown, false);
				WriteBoolValue("subtotalHiddenItems", pivotTable.SubtotalHiddenItems, false);
				WriteBoolValue("rowGrandTotals", pivotTable.ColumnGrandTotals, true);
				WriteBoolValue("colGrandTotals", pivotTable.RowGrandTotals, true);
				WriteBoolValue("fieldPrintTitles", pivotTable.FieldPrintTitles, false);
				WriteBoolValue("itemPrintTitles", pivotTable.ItemPrintTitles, false);
				WriteBoolValue("mergeItem", pivotTable.MergeItem, false);
				WriteBoolValue("showDropZones", pivotTable.ShowDropZones, true);
				WriteIntValue("createdVersion", pivotTable.CreatedVersion, 0);
				int indent = pivotTable.Indent - 1;
				if (indent < 0)
					indent += 128;
				WriteIntValue("indent", indent, 1);
				WriteBoolValue("showEmptyRow", pivotTable.ShowEmptyRow, false);
				WriteBoolValue("showEmptyCol", pivotTable.ShowEmptyColumn, false);
				WriteBoolValue("showHeaders", pivotTable.ShowHeaders, true);
				WriteBoolValue("compact", pivotTable.Compact, true);
				WriteBoolValue("outline", pivotTable.Outline, false);
				WriteBoolValue("outlineData", pivotTable.OutlineData, false);
				WriteBoolValue("compactData", pivotTable.CompactData, true);
				WriteBoolValue("published", pivotTable.Published, false);
				WriteBoolValue("gridDropZones", pivotTable.GridDropZones, false);
				WriteBoolValue("immersive", pivotTable.Immersive, true);
				WriteBoolValue("multipleFieldFilters", pivotTable.MultipleFieldFilters, true);
				WriteIntValue("chartFormat", pivotTable.ChartFormat, 0);
				WriteStringValue("rowHeaderCaption", pivotTable.RowHeaderCaption, !String.IsNullOrEmpty(pivotTable.RowHeaderCaption));
				WriteStringValue("colHeaderCaption", pivotTable.ColHeaderCaption, !String.IsNullOrEmpty(pivotTable.ColHeaderCaption));
				WriteBoolValue("fieldListSortAscending", pivotTable.FieldListSortAscending, false);
				WriteBoolValue("mdxSubqueries", pivotTable.MdxSubqueries, false);
				WriteBoolValue("customListSort", pivotTable.CustomListSort, true);
				#endregion
				GeneratePivotTableLocationContent(pivotTable.Location);
				GeneratePivotFieldCollectionContent(pivotTable.Fields);
				GeneratePivotTableRowFieldsContent(pivotTable.RowFields);
				GeneratePivotTableRowItemsContent(pivotTable.RowItems);
				GeneratePivotTableColumnFieldsContent(pivotTable.ColumnFields);
				GeneratePivotTableColumnItemsContent(pivotTable.ColumnItems);
				GeneratePivotTablePageFieldsContent(pivotTable.PageFields);
				GeneratePivotTableDataFieldsContent(pivotTable.DataFields);
				GeneratePivotTableFormatsContent(pivotTable.Formats);
				GeneratePivotTableChartFormatsContent(pivotTable.ChartFormats);
				GeneratePivotTablePivotHierarchiesContent(pivotTable.Hierarchies);
				GeneratePivotTableStyleInfoContent(pivotTable.StyleInfo);
				GeneratePivotTableFiltersContent(pivotTable.Filters);
				GeneratePivotTableRowHierarchiesUsageContent(pivotTable.RowHierarchiesUsage);
				GeneratePivotTableColHierarchiesUsageContent(pivotTable.ColHierarchiesUsage);
				GeneratePivotTableExtLstContent(pivotTable);
			}
			finally {
				WriteShEndElement();
			}
		}
		#endregion
		#region Export PivotTableLocation
		protected internal virtual void GeneratePivotTableLocationContent(PivotTableLocation location) {
			WriteShStartElement("location");
			try {
				WriteStRef(location.Range, "ref");
				WriteIntValue("firstHeaderRow", location.FirstHeaderRow);
				WriteIntValue("firstDataRow", location.FirstDataRow);
				WriteIntValue("firstDataCol", location.FirstDataColumn);
				WriteIntValue("rowPageCount", location.RowPageCount, 0);
				WriteIntValue("colPageCount", location.ColumnPageCount, 0);
			}
			finally {
				WriteShEndElement();
			}
		}
		#endregion
		#region Export GeneratePivotFieldCollectionContent
		protected internal virtual void GeneratePivotFieldCollectionContent(PivotFieldCollection fields) {
			if (fields.Count > 0) {
				WriteShStartElement("pivotFields");
				try {
					WriteIntValue("count", fields.Count);
					foreach (PivotField record in fields)
						GeneratePivotFieldContent(record);
				}
				finally {
					WriteShEndElement();
				}
			}
		}
		#endregion
		#region Export PivotField
		protected internal virtual void GeneratePivotFieldContent(PivotField field) {
			WriteShStartElement("pivotField");
			try {
				WriteStringValue("name", field.Name, !string.IsNullOrEmpty(field.Name));
				WriteEnumValue("axis", field.Axis, PivotTablePivotFieldDestination.pivotTableAxisTable, PivotTableAxis.None);
				WriteBoolValue("dataField", field.IsDataField, false);
				WriteStringValue("subtotalCaption", field.SubtotalCaption, !String.IsNullOrEmpty(field.SubtotalCaption));
				WriteBoolValue("showDropDowns", field.ShowDropDowns, true);
				WriteBoolValue("hiddenLevel", field.HiddenLevel, false);
				WriteStringValue("uniqueMemberProperty", field.UniqueMemberProperty, !String.IsNullOrEmpty(field.UniqueMemberProperty));
				WriteBoolValue("compact", field.Compact, true);
				WriteBoolValue("allDrilled", field.AllDrilled, false);
				WriteIntValue("numFmtId", ExportStyleSheet.GetNumberFormatId(field.NumberFormatIndex), field.NumberFormatIndex > 0);
				WriteBoolValue("outline", field.Outline, true);
				WriteBoolValue("subtotalTop", field.SubtotalTop, true);
				WriteBoolValue("dragToRow", field.DragToRow, true);
				WriteBoolValue("dragToCol", field.DragToCol, true);
				WriteBoolValue("multipleItemSelectionAllowed", field.MultipleItemSelectionAllowed, false);
				WriteBoolValue("dragToPage", field.DragToPage, true);
				WriteBoolValue("dragToData", field.DragToData, true);
				WriteBoolValue("dragOff", field.DragOff, true);
				WriteBoolValue("showAll", field.ShowItemsWithNoData, true);
				WriteBoolValue("insertBlankRow", field.InsertBlankRow, false);
				WriteBoolValue("serverField", field.ServerField, false);
				WriteBoolValue("insertPageBreak", field.InsertPageBreak, false);
				WriteBoolValue("autoShow", field.AutoShow, false);
				WriteBoolValue("topAutoShow", field.TopAutoShow, true);
				WriteBoolValue("hideNewItems", field.HideNewItems, false);
				WriteBoolValue("measureFilter", field.MeasureFilter, false);
				WriteBoolValue("includeNewItemsInFilter", field.IncludeNewItemsInFilter, false);
				WriteIntValue("itemPageCount", field.ItemPageCount, 10);
				WriteEnumValue("sortType", field.SortType, PivotTablePivotFieldDestination.pivotTableFieldSortTypeTable, PivotTableSortTypeField.Manual);
				if (field.HasDataSourceSort)
					WriteBoolValue("dataSourceSort", field.DataSourceSort);
				WriteBoolValue("nonAutoSortDefault", field.NonAutoSortDefault, false);
				if (field.RankBy.HasValue &&  field.RankBy.Value > -1)
					WriteIntValue("rankBy", field.RankBy.Value); 
				WriteBoolValue("defaultSubtotal", field.DefaultSubtotal, true);
				WriteBoolValue("sumSubtotal", field.SumSubtotal, false);
				WriteBoolValue("countASubtotal", field.CountASubtotal, false);
				WriteBoolValue("avgSubtotal", field.AvgSubtotal, false);
				WriteBoolValue("maxSubtotal", field.MaxSubtotal, false);
				WriteBoolValue("minSubtotal", field.MinSubtotal, false);
				WriteBoolValue("productSubtotal", field.ProductSubtotal, false);
				WriteBoolValue("countSubtotal", field.CountSubtotal, false);
				WriteBoolValue("stdDevSubtotal", field.StdDevSubtotal, false);
				WriteBoolValue("stdDevPSubtotal", field.StdDevPSubtotal, false);
				WriteBoolValue("varSubtotal", field.VarSubtotal, false);
				WriteBoolValue("varPSubtotal", field.VarPSubtotal, false);
				WriteBoolValue("showPropCell", field.ShowPropCell, false);
				WriteBoolValue("showPropTip", field.ShowPropTip, false);
				WriteBoolValue("showPropAsCaption", field.ShowPropAsCaption, false);
				WriteBoolValue("defaultAttributeDrillState", field.DefaultAttributeDrillState, false);
				GeneratePivotItemCollectionContent(field.Items);
				if (field.SortType != PivotTableSortTypeField.Manual)
					GeneratePivotAutoSortScopeContent(field.PivotArea);
				if (field.FillDownLabels)
					GeneratePivotFieldExtContent(field);
			}
			finally {
				WriteShEndElement();
			}
		}
		#endregion
		#region Export PivotItemCollection
		protected internal virtual void GeneratePivotItemCollectionContent(PivotItemCollection items) {
			if (items.Count > 0) {
				WriteShStartElement("items");
				try {
					WriteIntValue("count", items.Count);
					foreach (PivotItem item in items)
						GeneratePivotItemContent(item);
				}
				finally {
					WriteShEndElement();
				}
			}
		}
		#endregion
		#region Export PivotItem
		protected internal virtual void GeneratePivotItemContent(PivotItem item) {
			WriteShStartElement("item");
			try {
				WriteStringValue("n", item.ItemUserCaption, !string.IsNullOrEmpty(item.ItemUserCaption));
				WriteEnumValue("t", item.ItemType, PivotTablePivotItemCollectionDestination.pivotTableItemTypeTable, PivotFieldItemType.Data);
				WriteBoolValue("h", item.IsHidden, false);
				WriteBoolValue("s", item.HasCharacterValue, false);
				WriteBoolValue("sd", !item.HideDetails, true);
				WriteBoolValue("f", item.CalculatedMember, false);
				WriteBoolValue("m", item.HasMissingValue, false);
				WriteBoolValue("c", item.ChildItems, false);
				if (item.ItemType == PivotFieldItemType.Data)
					WriteIntValue("x", item.ItemIndex); 
				WriteBoolValue("d", item.HasExpandedView, false);
				WriteBoolValue("e", item.DrillAcross, true);
			}
			finally {
				WriteShEndElement();
			}
		}
		#endregion
		#region Export PivotAutoSortScope
		protected internal virtual void GeneratePivotAutoSortScopeContent(PivotArea area) {
			if (!area.IsDefault) {
				WriteShStartElement("autoSortScope");
				try {
					GeneratePivotAreaContent(area);
				}
				finally {
					WriteShEndElement();
				}
			}
		}
		#endregion
		#region Export PivotAreas
		protected internal virtual void GeneratePivotTablePivotAreasContent(PivotAreaCollection pivotAreas) {
			if (pivotAreas.Count > 0) {
				WriteShStartElement("pivotAreas");
				try {
					WriteIntValue("count", pivotAreas.Count);
					foreach (PivotArea pArea in pivotAreas)
						GeneratePivotAreaContent(pArea);
				}
				finally {
					WriteShEndElement();
				}
			}
		}
		#endregion
		#region Export PivotArea
		protected internal virtual void GeneratePivotAreaContent(PivotArea area) {
			WriteShStartElement("pivotArea");
			try {
				if (area.Field.HasValue)
					WriteIntValue("field", area.Field.Value);
				WriteEnumValue("type", area.Type, PivotTablePivotAreaDestination.pivotTableAreaTypeTable, PivotAreaType.Normal);
				WriteBoolValue("dataOnly", area.IsDataOnly, true);
				WriteBoolValue("labelOnly", area.IsLabelOnly, false);
				WriteBoolValue("grandRow", area.IsGrandRow, false);
				WriteBoolValue("grandCol", area.IsGrandColumn, false);
				WriteBoolValue("cacheIndex", area.IsCacheIndex, false);
				WriteBoolValue("outline", area.IsOutline, true);
				if (area.Range != null)
					WriteStRef(area.Range, "offset");
				WriteBoolValue("collapsedLevelsAreSubtotals", area.IsCollapsedLevelsAreSubtotals, false);
				WriteEnumValue("axis", area.Axis, PivotTablePivotFieldDestination.pivotTableAxisTable, PivotTableAxis.None);
				if (area.FieldPosition != null)
					WriteIntValue("fieldPosition", area.FieldPosition.Value);
				GeneratPivotAreaReferenceCollection(area.References);
			}
			finally {
				WriteShEndElement();
			}
		}
		#endregion
		#region Export GeneratPivotAreaReferenceCollection
		protected internal virtual void GeneratPivotAreaReferenceCollection(PivotAreaReferenceCollection references) {
			if (references.Count > 0) {
				WriteShStartElement("references");
				try {
					WriteIntValue("count", references.Count);
					foreach (PivotAreaReference record in references)
						GeneratePivotAreaReference(record);
				}
				finally {
					WriteShEndElement();
				}
			}
		}
		#endregion
		#region Export GeneratePivotAreaReference
		protected internal virtual void GeneratePivotAreaReference(PivotAreaReference reference) {
			WriteShStartElement("reference");
			try {
				if (reference.Field.HasValue) {
					if(reference.Field.Value == -2)
						WriteLongValue("field", 4294967294);
					else
						WriteLongValue("field", reference.Field.Value);
				}
				WriteIntValue("count", reference.SharedItemsIndex.Count);
				WriteBoolValue("selected", reference.IsSelected, true);
				WriteBoolValue("byPosition", reference.IsByPosition, false);
				WriteBoolValue("relative", reference.IsRelative, false);
				WriteBoolValue("defaultSubtotal", reference.DefaultSubtotal, false);
				WriteBoolValue("sumSubtotal", reference.SumSubtotal, false);
				WriteBoolValue("countASubtotal", reference.CountASubtotal, false);
				WriteBoolValue("avgSubtotal", reference.AvgSubtotal, false);
				WriteBoolValue("maxSubtotal", reference.MaxSubtotal, false);
				WriteBoolValue("minSubtotal", reference.MinSubtotal, false);
				WriteBoolValue("productSubtotal", reference.ProductSubtotal, false);
				WriteBoolValue("countSubtotal", reference.CountSubtotal, false);
				WriteBoolValue("stdDevSubtotal", reference.StdDevSubtotal, false);
				WriteBoolValue("stdDevPSubtotal", reference.StdDevPSubtotal, false);
				WriteBoolValue("varSubtotal", reference.VarSubtotal, false);
				WriteBoolValue("varPSubtotal", reference.VarPSubtotal, false);
				foreach (long index in reference.SharedItemsIndex) {
					WriteShStartElement("x");
					try {
						WriteLongValue("v", index);
					}
					finally {
						WriteShEndElement();
					}
				}
			}
			finally {
				WriteShEndElement();
			}
		}
		#endregion
		#region Export PivotTableStyleInfo
		protected internal virtual void GeneratePivotTableStyleInfoContent(PivotTableStyleInfo styleInfo) {
			WriteShStartElement("pivotTableStyleInfo");
			try {
				WriteStringValue("name", styleInfo.StyleName, styleInfo.StyleName != PivotTableStyleInfo.DefaultStyleName); 
				WriteBoolValue("showRowHeaders", styleInfo.ShowRowHeaders);
				WriteBoolValue("showColHeaders", styleInfo.ShowColumnHeaders);
				WriteBoolValue("showRowStripes", styleInfo.ShowRowStripes);
				WriteBoolValue("showColStripes", styleInfo.ShowColumnStripes);
				if (styleInfo.HasShowLastColumn)
					WriteBoolValue("showLastColumn", styleInfo.ShowLastColumn);
			} finally {
				WriteShEndElement();
			}
		}
		#endregion
		#region Export PivotTableFilters
		protected internal virtual void GeneratePivotTableFiltersContent(PivotFilterCollection filters) {
			if (filters.Count > 0) {
				WriteShStartElement("filters");
				try {
					WriteIntValue("count", filters.Count);
					foreach (PivotFilter filter in filters) {
						WriteShStartElement("filter");
						try {
							WriteIntValue("fld", filter.FieldIndex);
							if (filter.MemberPropertyFieldId != null)
								WriteIntValue("mpFld", filter.MemberPropertyFieldId.Value);
							WriteEnumValue("type", filter.FilterType, PivotFilterDestination.pivotTablePivotFilterTypeTable);
							WriteIntValue("evalOrder", filter.EvalOrder, 0);
							WriteIntValue("id", filter.PivotFilterId);
							if (filter.MeasureIndex != null)
								WriteIntValue("iMeasureHier", filter.MeasureIndex.Value);
							if (filter.MeasureFieldIndex != null)
								WriteIntValue("iMeasureFld", filter.MeasureFieldIndex.Value);
							WriteStringValue("name", filter.Name, !String.IsNullOrEmpty(filter.Name));
							WriteStringValue("description", filter.Description, !String.IsNullOrEmpty(filter.Description));
							WriteStringValue("stringValue1", filter.LabelPivot, !String.IsNullOrEmpty(filter.LabelPivot));
							WriteStringValue("stringValue2", filter.LabelPivotFilter, !String.IsNullOrEmpty(filter.LabelPivotFilter));
							GenerateAutoFilterContentCore(filter.AutoFilter);
						}
						finally {
							WriteShEndElement();
						}
					}
				}
				finally {
					WriteShEndElement();
				}
			}
		}
		#endregion
		#region Export PivotTableFormats
		protected internal virtual void GeneratePivotTableFormatsContent(PivotFormatCollection formats) {
			if (formats.Count > 0) {
				WriteShStartElement("formats");
				try {
					WriteIntValue("count", formats.Count);
					foreach (PivotFormat format in formats) {
						WriteShStartElement("format");
						try {
							WriteEnumValue("action", format.FormatAction, PivotTablePivotFormatDestination.pivotTableFormatActionTable, FormatAction.Formatting);
							WriteDifferentialFormatId("dxfId", format.Index);
							GeneratePivotAreaContent(format.PivotArea);
						}
						finally {
							WriteShEndElement();
						}
					}
				}
				finally {
					WriteShEndElement();
				}
			}
		}
		#endregion
		#region Export PivotTableDataFields
		protected internal virtual void GeneratePivotTableDataFieldsContent(PivotDataFieldCollection dataFields) {
			if (dataFields != null)
				if (dataFields.Count > 0) {
					WriteShStartElement("dataFields");
					try {
						WriteIntValue("count", dataFields.Count);
						foreach (PivotDataField dataField in dataFields) {
							WriteShStartElement("dataField");
							try {
								WriteStringValue("name", dataField.Name, !String.IsNullOrEmpty(dataField.Name));
								WriteIntValue("fld", dataField.FieldIndex);
								WriteEnumValue("subtotal", dataField.Subtotal, PivotTableDataFieldDestination.pivotDataConsolidateFunctionTable, PivotDataConsolidateFunction.Sum);
								if (dataField.ShowDataAs <= PivotShowDataAs.Index)
									WriteEnumValue("showDataAs", dataField.ShowDataAs, PivotTableDataFieldDestination.pivotShowDataAsTable, PivotShowDataAs.Normal);
								WriteIntValue("baseField", dataField.BaseField, -1);
								WriteIntValue("baseItem", dataField.BaseItem, 1048832);
								WriteIntValue("numFmtId", ExportStyleSheet.GetNumberFormatId(dataField.NumberFormatIndex), 0);
								if (dataField.ShowDataAs > PivotShowDataAs.Index)
									GeneratePivotTableDataFieldsExtContent(dataField);
							}
							finally {
								WriteShEndElement();
							}
						}
					}
					finally {
						WriteShEndElement();
					}
				}
		}
		#endregion
		#region Export PivotTableDataFieldsExt
		protected internal virtual void GeneratePivotTableDataFieldsExtContent(PivotDataField dataField) {
			WriteShStartElement("extLst");
			try {
				WriteShStartElement("ext");
				try {
					WriteStringAttr("xmlns", "x14", null, NamespaceXmlnsX14);
					WriteStringValue("uri", PivotDataFieldsExtUri);
					WriteStartElement("x14", "dataField", null);
					try {
						WriteEnumValue("pivotShowAs", dataField.ShowDataAs, PivotTableDataFieldDestination.pivotShowDataAsTable, PivotShowDataAs.Normal);
					}
					finally {
						WriteShEndElement();
					}
				}
				finally {
					WriteShEndElement();
				}
			}
			finally {
				WriteShEndElement();
			}
		}
		#endregion
		#region Export PivotTablePageFields
		protected internal virtual void GeneratePivotTablePageFieldsContent(PivotPageFieldCollection pageFields) {
			if (pageFields.Count > 0) {
				WriteShStartElement("pageFields");
				try {
					WriteIntValue("count", pageFields.Count);
					foreach (PivotPageField pageField in pageFields) {
						WriteShStartElement("pageField");
						try {
							WriteIntValue("fld", pageField.FieldIndex);
							WriteIntValue("item", pageField.ItemIndex, -1);
							WriteIntValue("hier", pageField.HierarchyIndex);
							WriteStringValue("name", pageField.HierarchyUniqueName, !string.IsNullOrEmpty(pageField.HierarchyUniqueName));
							WriteStringValue("cap", pageField.HierarchyDisplayName, !string.IsNullOrEmpty(pageField.HierarchyDisplayName));
						}
						finally {
							WriteShEndElement();
						}
					}
				}
				finally {
					WriteShEndElement();
				}
			}
		}
		#endregion
		#region Export PivotFieldExt
		protected internal virtual void GeneratePivotFieldExtContent(PivotField field) {
			WriteShStartElement("extLst");
			try {
				WriteShStartElement("ext");
				try {
					WriteStringAttr("xmlns", "x14", null, NamespaceXmlnsX14);
					WriteStringValue("uri", PivotFieldExtUri);
					WriteStartElement("x14", "pivotField", null);
					try {
						WriteBoolValue("fillDownLabels", field.FillDownLabels);
					}
					finally {
						WriteShEndElement();
					}
				}
				finally {
					WriteShEndElement();
				}
			}
			finally {
				WriteShEndElement();
			}
		}
		#endregion
		#region Export ConditionalFormats
		protected internal virtual void GeneratePivotTableConditionalFormatsContent(PivotConditionalFormatCollection conditionalFormats) {
			if (conditionalFormats.Count > 0) { 
				WriteShStartElement("conditionalFormats");
				try {
					WriteIntValue("count", conditionalFormats.Count);
					foreach (PivotConditionalFormat cFormat in conditionalFormats) { 
						WriteShStartElement("conditionalFormat");
						try {
							WriteEnumValue("scope", cFormat.Scope,
									PivotTableConditionalFormatDestination.pivotTableConditionalFormatScopeTable, ConditionalFormatScope.Selection);
							WriteEnumValue("type", cFormat.Type,
									PivotTableConditionalFormatDestination.pivotTableConditionalFormatTypeTable, ConditionalFormatType.None);
							WriteIntValue("priority", cFormat.Priority);
							GeneratePivotTablePivotAreasContent(cFormat.PivotAreas);
						}
						finally {
							WriteShEndElement();
						}
					}
				}
				finally {
					WriteShEndElement();
				}
			}
		}
		#endregion
		#region Export ChartFormats
		protected internal virtual void GeneratePivotTableChartFormatsContent(PivotChartFormatsCollection chartFormats) {
			if (chartFormats.Count > 0) { 
				WriteShStartElement("chartFormats");
				try {
					WriteIntValue("count", chartFormats.Count);
					foreach (PivotChartFormat chartFormat in chartFormats) {
						WriteShStartElement("chartFormat");
						try {
							WriteIntValue("chart", chartFormat.ChartIndex);
							WriteIntValue("format", chartFormat.PivotFormatId);
							WriteBoolValue("series", chartFormat.SeriesFormat, false);
							GeneratePivotAreaContent(chartFormat.PivotArea);
						}
						finally {
							WriteShEndElement();
						}
					}
				}
				finally {
					WriteShEndElement();
				}
			}
		}
		#endregion
		#region Export PivotHierarchies
		protected internal virtual void GeneratePivotTablePivotHierarchiesContent(PivotHierarchyCollection hierarchies) {
			if(hierarchies.Count > 0){
				WriteShStartElement("pivotHierarchies");
				try {
					WriteIntValue("count", hierarchies.Count);
					foreach (PivotHierarchy hierarchy in hierarchies) {
						WriteShStartElement("pivotHierarchy");
						try {
							WriteBoolValue("outline", hierarchy.Outline, false);
							WriteBoolValue("multipleItemSelectionAllowed", hierarchy.MultipleItemSelectionAllowed, false);
							WriteBoolValue("subtotalTop", hierarchy.SubtotalTop, false);
							WriteBoolValue("showInFieldList", hierarchy.ShowInFieldList, true);
							WriteBoolValue("dragToRow", hierarchy.DragToRow, true);
							WriteBoolValue("dragToCol", hierarchy.DragToColumn, true);
							WriteBoolValue("dragToPage", hierarchy.DragToPage, true);
							WriteBoolValue("dragToData", hierarchy.DragToData, false);
							WriteBoolValue("dragOff", hierarchy.DragOff, true);
							WriteBoolValue("includeNewItemsInFilter", hierarchy.IncludeNewItemsInFilter, false);
							WriteStringValue("caption", hierarchy.Caption, !String.IsNullOrEmpty(hierarchy.Caption));
							#region Members
							if (hierarchy.Members.Count > 0) {
								WriteShStartElement("members");
								try {
									WriteIntValue("count", hierarchy.Members.Count);
									if(hierarchy.LevelMembers.HasValue)
										WriteIntValue("level", hierarchy.LevelMembers.Value);
									foreach (string memberName in hierarchy.Members) { 
										WriteShStartElement("member");
										try {
											WriteStringValue("name", memberName); 
										}
										finally {
											WriteShEndElement();
										}
									}
								}
								finally {
									WriteShEndElement();
								}
							}
							#endregion
							GeneratePivotTablePivotHierarchiesMPSContent(hierarchy.MemberProperties);
						}
						finally {
							WriteShEndElement();
						}
					}
				}
				finally {
					WriteShEndElement();
				}
			}
		}
		protected internal virtual void GeneratePivotTablePivotHierarchiesMPSContent(MemberPropertiesCollection memberProperties) {
			if (memberProperties.Count > 0) { 
				WriteShStartElement("mps");
				try {
					WriteIntValue("count", memberProperties.Count);
					foreach(MemberProperty mp in memberProperties){
						WriteShStartElement("mp");
						try {
							WriteStringValue("name", mp.Name, !String.IsNullOrEmpty(mp.Name));
							WriteBoolValue("showCell", mp.ShowCell, false);
							WriteBoolValue("showTip", mp.ShowTip, false);
							WriteBoolValue("showAsCaption", mp.ShowAsCaption, false);
							if(mp.NameLen.HasValue)
								WriteIntValue("nameLen", (int)mp.NameLen.Value);
							if (mp.PropertyNameCharacterIndex.HasValue)
								WriteIntValue("pPos", (int)mp.PropertyNameCharacterIndex.Value);
							if (mp.PropertyNameLength.HasValue)
								WriteIntValue("pLen", (int)mp.PropertyNameLength.Value);
							if (mp.LevelIndex.HasValue)
								WriteIntValue("level", (int)mp.LevelIndex.Value);
							WriteIntValue("field", (int)mp.FieldIndex);
						}
						finally {
							WriteShEndElement();
						}
					}
				}
				finally {
					WriteShEndElement();
				}
			}
		}
		#endregion 
		#region Export Col/Row HierarchiesUsage
		protected internal virtual void GeneratePivotTableColHierarchiesUsageContent(UndoableCollection<int> hierarchiesUsage) {
			GeneratePivotTableHierarchiesUsageContent(hierarchiesUsage, "col");
		}
		protected internal virtual void GeneratePivotTableRowHierarchiesUsageContent(UndoableCollection<int> hierarchiesUsage) {
			GeneratePivotTableHierarchiesUsageContent(hierarchiesUsage, "row");
		}
		private void GeneratePivotTableHierarchiesUsageContent(UndoableCollection<int> hierarchiesUsage, string prefixTag) {
			if (hierarchiesUsage.Count > 0) {
				WriteShStartElement(prefixTag + "HierarchiesUsage");
				try {
					WriteIntValue("count", hierarchiesUsage.Count);
					foreach (int element in hierarchiesUsage) {
						WriteShStartElement(prefixTag + "HierarchyUsage");
						try {
							WriteIntValue("hierarchyUsage", element);
						}
						finally {
							WriteShEndElement();
						}
					}
				}
				finally {
					WriteShEndElement();
				}
			}
		}
		#endregion 
		#region Export Column/Row Fields
		protected internal virtual void  GeneratePivotTableRowFieldsContent(PivotTableColumnRowFieldIndices rowFields) {
			GeneratePivotTableColumnRowFieldsContent(rowFields, "row");
		}
		protected internal virtual void  GeneratePivotTableColumnFieldsContent(PivotTableColumnRowFieldIndices colFields) {
			GeneratePivotTableColumnRowFieldsContent(colFields, "col");
		}
		private void GeneratePivotTableColumnRowFieldsContent(PivotTableColumnRowFieldIndices fields, string prefixTag) {
			if (fields.Count > 0) {
				WriteShStartElement(prefixTag + "Fields");
				try {
					WriteIntValue("count", fields.Count);
					foreach (int fieldIndex in fields.GetIndicesEnumerable()) {
						WriteShStartElement("field");
						try {
							WriteIntValue("x", fieldIndex);
						}
						finally {
							WriteShEndElement();
						}
					}
				}
				finally {
					WriteShEndElement();
				}
			}
		}
		#endregion
		#region Export Column/Row Items
		protected internal virtual void GeneratePivotTableRowItemsContent(PivotLayoutItems rowItems) {
			if (rowItems.Count > 0) {
				WriteShStartElement("rowItems");
				try {
					WriteIntValue("count", rowItems.Count);
					foreach (IPivotLayoutItem element in rowItems) {
						GeneratePivotTableColumnRowItemsContent(element);
					}
				}
				finally {
					WriteShEndElement();
				}
			}
		}
		protected internal virtual void GeneratePivotTableColumnItemsContent(PivotLayoutItems columnItems) {
			if (columnItems.Count > 0) {
				WriteShStartElement("colItems");
				try {
					WriteIntValue("count", columnItems.Count);
					foreach (IPivotLayoutItem element in columnItems) {
						GeneratePivotTableColumnRowItemsContent(element);
					}
				}
				finally {
					WriteShEndElement();
				}
			}
		}
		protected internal virtual void GeneratePivotTableColumnRowItemsContent(IPivotLayoutItem item) {
			WriteShStartElement("i");
			try {
				WriteEnumValue("t", item.Type, PivotTablePivotItemDestination.pivotTableItemTypeTable, PivotFieldItemType.Data);
				WriteIntValue("r", item.RepeatedItemsCount, 0);
				WriteIntValue("i", item.DataFieldIndex, 0);
				foreach (int mpi in item.PivotFieldItemIndices) {
					WriteShStartElement("x");
					try {
						WriteIntValue("v", mpi, mpi != 0);
					}
					finally {
						WriteShEndElement();
					}
				}
			}
			finally {
				WriteShEndElement();
			}
		}
		#endregion
		#region Export ExtList PivotTable
		bool ShouldExportPivotTableExtList(PivotTable pivotTable) {
			return !pivotTable.ShowValuesRow || !string.IsNullOrEmpty(pivotTable.AltText) || !string.IsNullOrEmpty(pivotTable.AltTextSummary);
		}
		protected internal virtual void GeneratePivotTableExtLstContent(PivotTable pivotTable) {
			if (ShouldExportPivotTableExtList(pivotTable)) {
				WriteShStartElement("extLst");
				try {
					WriteShStartElement("ext");
					try {
						WriteStringAttr("xmlns", "x14", null, NamespaceXmlnsX14);
						WriteStringValue("uri", PivotTableExtUri);
						WriteStartElement("x14", "pivotTableDefinition", null);
						try {
							WriteStringAttr("xmlns", "xm", null, PivotTableNamespaceXmlns);
							WriteBoolValue("hideValuesRow", !pivotTable.ShowValuesRow, false);
							if (!string.IsNullOrEmpty(pivotTable.AltText))
								WriteStringValue("altText", pivotTable.AltText);
							if (!string.IsNullOrEmpty(pivotTable.AltTextSummary))
								WriteStringValue("altTextSummary", pivotTable.AltTextSummary);
						}
						finally {
							WriteShEndElement();
						}
					}
					finally {
						WriteShEndElement();
					}
				}
				finally {
					WriteShEndElement();
				}
			}
		}
		#endregion
	}
}
