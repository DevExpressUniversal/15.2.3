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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraEditors.Helpers;
using DevExpress.XtraPivotGrid.Helpers;
namespace DevExpress.XtraPivotGrid {
	public class PivotGridAppearancesBase : BaseAppearanceCollection {
		protected const string FieldHeaderAppearanceName = "FieldHeader";
		protected const string CellAppearanceName = "Cell";
		protected const string TotalCellAppearanceName = "TotalCell";
		protected const string GrandTotalCellAppearanceName = "GrandTotalCell";
		protected const string CustomTotalCellAppearanceName = "CustomTotalCell";
		protected const string LinesAppearanceName = "Lines";
		protected const string FilterSeparatorAppearanceName = "FilterSeparator";
		protected const string FieldValueAppearanceName = "FieldValue";
		protected const string FieldValueTotalAppearanceName = "FieldValueTotal";
		protected const string FieldValueGrandTotalAppearanceName = "FieldValueGrandTotal";
		protected const string HeaderGroupLineAppearanceName = "HeaderGroupLine";
		AppearanceObject fieldHeader, cell, totalCell, grandTotalCell, customTotalCell,
			fieldValue, fieldValueTotal, fieldValueGrandTotal, lines, filterSeparator,
			headerGroupLine;
		protected override void CreateAppearances() {
			base.CreateAppearances();
			this.fieldHeader = CreateAppearance(FieldHeaderAppearanceName);
			this.cell = CreateAppearance(CellAppearanceName);
			this.totalCell = CreateAppearance(TotalCellAppearanceName);
			this.grandTotalCell = CreateAppearance(GrandTotalCellAppearanceName);
			this.customTotalCell = CreateAppearance(CustomTotalCellAppearanceName);
			this.fieldValue = CreateAppearance(FieldValueAppearanceName);
			this.fieldValueTotal = CreateAppearance(FieldValueTotalAppearanceName);
			this.fieldValueGrandTotal = CreateAppearance(FieldValueGrandTotalAppearanceName);
			this.lines = CreateAppearance(LinesAppearanceName);
			this.filterSeparator = CreateAppearance(FilterSeparatorAppearanceName);
			this.headerGroupLine = CreateAppearance(HeaderGroupLineAppearanceName);
		}
		readonly PivotGridViewInfoData data;
		public PivotGridAppearancesBase(PivotGridViewInfoData data) {
			this.data = data;
		}
#if !SL
	[DevExpressXtraPivotGridLocalizedDescription("PivotGridAppearancesBaseIsLoading")]
#endif
		public override bool IsLoading { get { return data != null ? data.IsLoading : false; } }
		bool ShouldSerializeCell() { return Cell.ShouldSerialize(); }
		void ResetCell() { Cell.Reset(); }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridAppearancesBaseCell"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridAppearancesBase.Cell")
		]
		public AppearanceObject Cell { get { return cell; } }
		bool ShouldSerializeFieldHeader() { return FieldHeader.ShouldSerialize(); }
		void ResetFieldHeader() { FieldHeader.Reset(); }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridAppearancesBaseFieldHeader"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridAppearancesBase.FieldHeader")
		]
		public AppearanceObject FieldHeader { get { return fieldHeader; } }
		bool ShouldSerializeTotalCell() { return TotalCell.ShouldSerialize(); }
		void ResetTotalCell() { TotalCell.Reset(); }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridAppearancesBaseTotalCell"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridAppearancesBase.TotalCell")
		]
		public AppearanceObject TotalCell { get { return totalCell; } }
		bool ShouldSerializeGrandTotalCell() { return GrandTotalCell.ShouldSerialize(); }
		void ResetGrandTotalCell() { GrandTotalCell.Reset(); }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridAppearancesBaseGrandTotalCell"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridAppearancesBase.GrandTotalCell")
		]
		public AppearanceObject GrandTotalCell { get { return grandTotalCell; } }
		bool ShouldSerializeCustomTotalCell() { return CustomTotalCell.ShouldSerialize(); }
		void ResetCustomTotalCell() { CustomTotalCell.Reset(); }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridAppearancesBaseCustomTotalCell"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridAppearancesBase.CustomTotalCell")
		]
		public AppearanceObject CustomTotalCell { get { return customTotalCell; } }
		bool ShouldSerializeFieldValue() { return FieldValue.ShouldSerialize(); }
		void ResetFieldValue() { FieldValue.Reset(); }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridAppearancesBaseFieldValue"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridAppearancesBase.FieldValue")
		]
		public AppearanceObject FieldValue { get { return fieldValue; } }
		bool ShouldSerializeFieldValueTotal() { return FieldValueTotal.ShouldSerialize(); }
		void ResetFieldValueTotal() { FieldValueTotal.Reset(); }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridAppearancesBaseFieldValueTotal"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridAppearancesBase.FieldValueTotal")
		]
		public AppearanceObject FieldValueTotal { get { return fieldValueTotal; } }
		bool ShouldSerializeFieldValueGrandTotal() { return FieldValueGrandTotal.ShouldSerialize(); }
		void ResetFieldValueGrandTotal() { FieldValueGrandTotal.Reset(); }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridAppearancesBaseFieldValueGrandTotal"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridAppearancesBase.FieldValueGrandTotal")
		]
		public AppearanceObject FieldValueGrandTotal { get { return fieldValueGrandTotal; } }
		bool ShouldSerializeLines() { return Lines.ShouldSerialize(); }
		void ResetLines() { Lines.Reset(); }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridAppearancesBaseLines"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridAppearancesBase.Lines")
		]
		public AppearanceObject Lines { get { return lines; } }
		bool ShouldSerializeFilterSeparator() { return FilterSeparator.ShouldSerialize(); }
		void ResetFilterSeparator() { FilterSeparator.Reset(); }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridAppearancesBaseFilterSeparator"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridAppearancesBase.FilterSeparator")
		]
		public virtual AppearanceObject FilterSeparator { get { return filterSeparator; } }
		bool ShouldSerializeHeaderGroupLine() { return HeaderGroupLine.ShouldSerialize(); }
		void ResetHeaderGroupLine() { HeaderGroupLine.Reset(); }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridAppearancesBaseHeaderGroupLine"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridAppearancesBase.HeaderGroupLine")
		]
		public virtual AppearanceObject HeaderGroupLine { get { return headerGroupLine; } }
		internal AppearanceObject GetActualFieldValueAppearance(PivotGridValueType valueType, PivotFieldItem field) {
			switch(valueType) {
				case PivotGridValueType.CustomTotal:
				case PivotGridValueType.Total:
					AppearanceObject totalAppearance = field == null ? FieldValueTotal : new AppearanceObject(field.Appearance.ValueTotal, FieldValueTotal);
					return new AppearanceObject(totalAppearance, GetActualFieldValueAppearance(PivotGridValueType.Value, field));
				case PivotGridValueType.GrandTotal:
					AppearanceObject grandTotalAppearance = field == null ? FieldValueGrandTotal : new AppearanceObject(field.Appearance.ValueGrandTotal, FieldValueGrandTotal);
					return new AppearanceObject(grandTotalAppearance, GetActualFieldValueAppearance(PivotGridValueType.Total, field));
				case PivotGridValueType.Value:
					return field == null ? FieldValue : new AppearanceObject(field.Appearance.Value, FieldValue);
				default:
					throw new ArgumentOutOfRangeException("valueType");
			}
		}
		internal AppearanceObject GetActualFieldAppearance(PivotFieldItem field) {
			return field == null || field.Appearance == null ? FieldHeader : new AppearanceObject(field.Appearance.Header, FieldHeader);
		}
		internal AppearanceObject GetActualCellAppearance(PivotGridCellItem cellItem) {
			AppearanceObject appearance;
			if(cellItem.IsGrandTotalAppearance)
				appearance = GetGrandTotalCellAppearance();
			else
				if(cellItem.IsTotalAppearance)
					appearance = GetTotalCellAppearance();
				else
					appearance = Cell;
			if(cellItem.IsCustomTotalAppearance) {
				AppearanceObject defaultAppearance = CustomTotalCell;
				appearance = new AppearanceObject();
				PivotGridCustomTotal customTotal = (PivotGridCustomTotal)cellItem.CustomTotal;
				AppearanceHelper.Combine(appearance, new AppearanceObject[] { customTotal.Appearance, defaultAppearance });
			}
			PivotFieldItem field = cellItem.DataField as PivotFieldItem;
			if(field != null) {
				List<AppearanceObject> fieldAppearances = new List<AppearanceObject>();
				if(cellItem.IsGrandTotalAppearance)
					fieldAppearances.Add(field.Appearance.CellGrandTotal);
				else if(cellItem.IsTotalAppearance)
					fieldAppearances.Add(field.Appearance.CellTotal);
				else
					fieldAppearances.Add(field.Appearance.Cell);
				fieldAppearances.Add(appearance);
				appearance = new AppearanceObject();
				AppearanceHelper.Combine(appearance, fieldAppearances.ToArray());
			}
			PivotGridStyleFormatCondition formatAppearance = data.FormatConditions.GetStyleFormatByValue(cellItem);
			if(formatAppearance != null) {
				AppearanceObject oldAppearance = appearance;
				appearance = new AppearanceObject();
				AppearanceHelper.Combine(appearance, new AppearanceObject[] { formatAppearance.Appearance, oldAppearance });
			}
			return appearance;
		}
		internal AppearanceObject GetCellAppearanceWithCustomCellAppearance(PivotGridCellItem cellItem, Rectangle? bounds) { 
			AppearanceObject cellAppearance = new AppearanceObject();
			cellAppearance.Assign(GetActualCellAppearance(cellItem));
			foreach(PivotGridFormatRule rule in data.FormatRules)
				if(cellItem.DataField == data.GetFieldItem(rule.Measure)) {
					IFormatRuleAppearance appearanceRule = rule.Rule as IFormatRuleAppearance;
					if(appearanceRule != null && rule.CheckValue(cellItem)) {
						AppearanceObject appearance = appearanceRule.QueryAppearance(new FormatRuleAppearanceArgs(rule.ValueProvider, cellItem.Value));
						cellAppearance.Combine(appearance);
					}
				}
			GetCustomCellAppearance(ref cellAppearance, cellItem, bounds);
			return cellAppearance;
		}
		internal AppearanceObject GetCustomCellAppearance(ref AppearanceObject cellAppearance, PivotGridCellItem cellItem, Rectangle? bounds) {
			if(cellAppearance.HAlignment == HorzAlignment.Default)
				cellAppearance.TextOptions.HAlignment = HorzAlignment.Far;
			data.CustomAppearance(ref cellAppearance, cellItem, bounds);
			return cellAppearance;
		}
		internal AppearanceObject GetTotalCellAppearance() {
			AppearanceObject appearance = new AppearanceObject();
			AppearanceHelper.Combine(appearance, new AppearanceObject[] { TotalCell, Cell });
			return appearance;
		}
		internal AppearanceObject GetGrandTotalCellAppearance() {
			AppearanceObject appearance = new AppearanceObject();
			AppearanceHelper.Combine(appearance, new AppearanceObject[] { GrandTotalCell, TotalCell, Cell });
			return appearance;
		}
	}
	public class PivotGridAppearances : PivotGridAppearancesBase {
		protected const string FocusedCellAppearanceName = "FocusedCell";
		protected const string SelectedCellAppearanceName = "SelectedCell";
		protected const string HeaderAreaAppearanceName = "HeaderArea";
		protected const string ColumnHeaderAreaAppearanceName = "ColumnHeaderArea";
		protected const string RowHeaderAreaAppearanceName = "RowHeaderArea";
		protected const string FilterHeaderAreaAppearanceName = "FilterHeaderArea";
		protected const string DataHeaderAreaAppearanceName = "DataHeaderArea";
		protected const string EmptyAppearanceName = "Empty";
		protected const string ExpandButtonAppearanceName = "ExpandButton";
		protected const string HeaderFilterButtonAppearanceName = "HeaderFilterButton";
		protected const string HeaderFilterButtonActiveAppearanceName = "HeaderFilterButtonActive";
		protected const string PrefilterPanelName = "FilterPanel";
		AppearanceObject focusedCell, selectedCell, headerArea, columnHeaderArea,
			rowHeaderArea, filterHeaderArea, dataHeaderArea, empty, drillDownButton,
			headerFilterButton, headerFilterButtonActive, prefilterPanel;
		Image sortByColumnIndicatorImage;
		Color fieldValueTopBorderColor, fieldValueLeftRightBorderColor;
		int fieldValueTopMarginCorrection;
		protected override void CreateAppearances() {
			base.CreateAppearances();
			this.focusedCell = CreateAppearance(FocusedCellAppearanceName);
			this.selectedCell = CreateAppearance(SelectedCellAppearanceName);
			this.headerArea = CreateAppearance(HeaderAreaAppearanceName);
			this.columnHeaderArea = CreateAppearance(ColumnHeaderAreaAppearanceName);
			this.rowHeaderArea = CreateAppearance(RowHeaderAreaAppearanceName);
			this.filterHeaderArea = CreateAppearance(FilterHeaderAreaAppearanceName);
			this.dataHeaderArea = CreateAppearance(DataHeaderAreaAppearanceName);
			this.empty = CreateAppearance(EmptyAppearanceName);
			this.drillDownButton = CreateAppearance(ExpandButtonAppearanceName);
			this.headerFilterButton = CreateAppearance(HeaderFilterButtonAppearanceName);
			this.headerFilterButtonActive = CreateAppearance(HeaderFilterButtonActiveAppearanceName);
			this.prefilterPanel = CreateAppearance(PrefilterPanelName);
			this.sortByColumnIndicatorImage = null;
		}
		public PivotGridAppearances(PivotGridViewInfoData data)
			: base(data) { }
		bool ShouldSerializeFocusedCell() { return FocusedCell.ShouldSerialize(); }
		void ResetFocusedCell() { FocusedCell.Reset(); }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridAppearancesFocusedCell"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridAppearances.FocusedCell")
		]
		public AppearanceObject FocusedCell { get { return focusedCell; } }
		bool ShouldSerializeSelectedCell() { return SelectedCell.ShouldSerialize(); }
		void ResetSelectedCell() { SelectedCell.Reset(); }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridAppearancesSelectedCell"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridAppearances.SelectedCell")
		]
		public AppearanceObject SelectedCell { get { return selectedCell; } }
		bool ShouldSerializeHeaderArea() { return HeaderArea.ShouldSerialize(); }
		void ResetHeaderArea() { HeaderArea.Reset(); }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridAppearancesHeaderArea"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridAppearances.HeaderArea")
		]
		public AppearanceObject HeaderArea { get { return headerArea; } }
		bool ShouldSerializeColumnHeaderArea() { return ColumnHeaderArea.ShouldSerialize(); }
		void ResetColumnHeaderArea() { ColumnHeaderArea.Reset(); }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridAppearancesColumnHeaderArea"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridAppearances.ColumnHeaderArea")
		]
		public AppearanceObject ColumnHeaderArea { get { return columnHeaderArea; } }
		bool ShouldSerializeRowHeaderArea() { return RowHeaderArea.ShouldSerialize(); }
		void ResetRowHeaderArea() { RowHeaderArea.Reset(); }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridAppearancesRowHeaderArea"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridAppearances.RowHeaderArea")
		]
		public AppearanceObject RowHeaderArea { get { return rowHeaderArea; } }
		bool ShouldSerializeFilterHeaderArea() { return FilterHeaderArea.ShouldSerialize(); }
		void ResetFilterHeaderArea() { FilterHeaderArea.Reset(); }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridAppearancesFilterHeaderArea"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridAppearances.FilterHeaderArea")
		]
		public AppearanceObject FilterHeaderArea { get { return filterHeaderArea; } }
		bool ShouldSerializeDataHeaderArea() { return DataHeaderArea.ShouldSerialize(); }
		void ResetDataHeaderArea() { DataHeaderArea.Reset(); }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridAppearancesDataHeaderArea"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridAppearances.DataHeaderArea")
		]
		public AppearanceObject DataHeaderArea { get { return dataHeaderArea; } }
		bool ShouldSerializeEmpty() { return Empty.ShouldSerialize(); }
		void ResetEmpty() { Empty.Reset(); }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridAppearancesEmpty"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridAppearances.Empty")
		]
		public AppearanceObject Empty { get { return empty; } }
		bool ShouldSerializeExpandButton() { return ExpandButton.ShouldSerialize(); }
		void ResetExpandButton() { ExpandButton.Reset(); }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridAppearancesExpandButton"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridAppearances.ExpandButton")
		]
		public AppearanceObject ExpandButton { get { return drillDownButton; } }
		bool ShouldSerializeHeaderFilterButton() { return HeaderFilterButton.ShouldSerialize(); }
		void ResetHeaderFilterButton() { HeaderFilterButton.Reset(); }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridAppearancesHeaderFilterButton"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridAppearances.HeaderFilterButton")
		]
		public AppearanceObject HeaderFilterButton { get { return headerFilterButton; } }
		bool ShouldSerializeHeaderFilterButtonActive() { return HeaderFilterButtonActive.ShouldSerialize(); }
		void ResetHeaderFilterButtonActive() { HeaderFilterButtonActive.Reset(); }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridAppearancesHeaderFilterButtonActive"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridAppearances.HeaderFilterButtonActive")
		]
		public AppearanceObject HeaderFilterButtonActive { get { return headerFilterButtonActive; } }
		bool ShouldSerializePrefilterPanel() { return PrefilterPanel.ShouldSerialize(); }
		void ResetPrefilterPanel() { PrefilterPanel.Reset(); }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridAppearancesPrefilterPanel"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridAppearances.PrefilterPanel")
		]
		public AppearanceObject PrefilterPanel { get { return prefilterPanel; } }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridAppearancesSortByColumnIndicatorImage"),
#endif
 DefaultValue(null),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridAppearances.SortByColumnIndicatorImage"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public Image SortByColumnIndicatorImage {
			get { return sortByColumnIndicatorImage; }
			set { sortByColumnIndicatorImage = value; }
		}
		protected internal Color FieldValueTopBorderColor {
			get { return fieldValueTopBorderColor; }
			set { fieldValueTopBorderColor = value; }
		}
		protected internal Color FieldValueLeftRightBorderColor {
			get { return fieldValueLeftRightBorderColor; }
			set { fieldValueLeftRightBorderColor = value; }
		}
		protected internal int FieldValueTopMarginCorrection {
			get { return fieldValueTopMarginCorrection; }
			set { fieldValueTopMarginCorrection = value; }
		}
		public virtual AppearanceDefaultInfo[] GetAppearanceDefaultInfo(UserLookAndFeel lf) {
			ArrayList list = new ArrayList();
			list.AddRange(GetEmptyAppearanceDefaultInfo());
			switch(lf.ActiveStyle) {
				case ActiveLookAndFeelStyle.Skin:
					list.AddRange(GetSkinAppearanceDefaultInfo(lf));
					break;
				case ActiveLookAndFeelStyle.Office2003:
					list.AddRange(GetOffice2003AppearanceDefaultInfo());
					break;
				case ActiveLookAndFeelStyle.WindowsXP:
					list.AddRange(GetWindowsXPAppearanceDefaultInfo());
					break;
				default:
					list.AddRange(GetFlatAppearanceDefaultInfo());
					break;
			}
			return (AppearanceDefaultInfo[])list.ToArray(typeof(AppearanceDefaultInfo));
		}
		public virtual AppearanceDefaultInfo[] GetEmptyAppearanceDefaultInfo() {
			return new AppearanceDefaultInfo[] { 
				new AppearanceDefaultInfo(GrandTotalCellAppearanceName, new AppearanceDefault(Color.Empty, Color.Empty)),
				new AppearanceDefaultInfo(FieldValueTotalAppearanceName, new AppearanceDefault(Color.Empty, Color.Empty)),
				new AppearanceDefaultInfo(FieldValueGrandTotalAppearanceName, new AppearanceDefault(Color.Empty, Color.Empty)),
				new AppearanceDefaultInfo(ColumnHeaderAreaAppearanceName, new AppearanceDefault(Color.Empty, Color.Empty)),
				new AppearanceDefaultInfo(RowHeaderAreaAppearanceName, new AppearanceDefault(Color.Empty, Color.Empty)),
				new AppearanceDefaultInfo(FilterHeaderAreaAppearanceName, new AppearanceDefault(Color.Empty, Color.Empty)),
				new AppearanceDefaultInfo(DataHeaderAreaAppearanceName, new AppearanceDefault(Color.Empty, Color.Empty)),
				new AppearanceDefaultInfo(PrefilterPanelName, new AppearanceDefault(Color.Empty, Color.Empty)),
			};
		}
		public virtual AppearanceDefaultInfo[] GetFlatAppearanceDefaultInfo() {
			return new AppearanceDefaultInfo[] {
				new AppearanceDefaultInfo(CellAppearanceName, new AppearanceDefault(SystemColors.WindowText, SystemColors.Window)),
				new AppearanceDefaultInfo(FocusedCellAppearanceName, new AppearanceDefault(SystemColors.WindowText, SystemColors.Window, HorzAlignment.Default, VertAlignment.Default)),
				new AppearanceDefaultInfo(SelectedCellAppearanceName, new AppearanceDefault(SystemColors.HighlightText, SystemColors.Highlight)),
				new AppearanceDefaultInfo(TotalCellAppearanceName, new AppearanceDefault(SystemColors.InfoText, SystemColors.Info)),
				new AppearanceDefaultInfo(CustomTotalCellAppearanceName, new AppearanceDefault(SystemColors.WindowText, Color.LightGray)),
				new AppearanceDefaultInfo(FieldValueAppearanceName, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control)),
				new AppearanceDefaultInfo(FieldHeaderAppearanceName, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control)),
				new AppearanceDefaultInfo(HeaderAreaAppearanceName, new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDark)),
				new AppearanceDefaultInfo(LinesAppearanceName, new AppearanceDefault(SystemColors.ControlDark, SystemColors.ControlDark)),
				new AppearanceDefaultInfo(FilterSeparatorAppearanceName, new AppearanceDefault(SystemColors.ControlDark, SystemColors.ControlDark)),
				new AppearanceDefaultInfo(HeaderGroupLineAppearanceName, new AppearanceDefault(SystemColors.ControlDark, SystemColors.ControlDarkDark)),
				new AppearanceDefaultInfo(EmptyAppearanceName, new AppearanceDefault(SystemColors.WindowText, SystemColors.Window)),
				new AppearanceDefaultInfo(ExpandButtonAppearanceName, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Default, VertAlignment.Center)),
				new AppearanceDefaultInfo(HeaderFilterButtonAppearanceName, new AppearanceDefault(SystemColors.ControlDark, SystemColors.Control, HorzAlignment.Default, VertAlignment.Center)),
				new AppearanceDefaultInfo(HeaderFilterButtonActiveAppearanceName, new AppearanceDefault(Color.Blue, SystemColors.Control, Color.Empty, SystemColors.ControlLightLight, HorzAlignment.Default, VertAlignment.Center)),
				new AppearanceDefaultInfo(PrefilterPanelName, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control)),
			};
		}
		public virtual AppearanceDefaultInfo[] GetWindowsXPAppearanceDefaultInfo() {
			return GetFlatAppearanceDefaultInfo();
		}
		public virtual AppearanceDefaultInfo[] GetOffice2003AppearanceDefaultInfo() {
			AppearanceDefaultInfo[] appearances = GetFlatAppearanceDefaultInfo();
			for(int i = 0; i < appearances.Length; i++) {
				if(appearances[i].Name == HeaderAreaAppearanceName) {
					appearances[i].DefaultAppearance = Office2003Colors.Default[Office2003GridAppearance.GroupPanel].Clone() as AppearanceDefault;
					break;
				}
				if(appearances[i].Name == PrefilterPanelName) {
					appearances[i].DefaultAppearance = Office2003Colors.Default[Office2003GridAppearance.FooterPanel].Clone() as AppearanceDefault;
					break;
				}
			}
			return appearances;
		}
		public virtual AppearanceDefaultInfo[] GetSkinAppearanceDefaultInfo(UserLookAndFeel lf) {
			return new AppearanceDefaultInfo[] {
				new AppearanceDefaultInfo(CellAppearanceName, UpdateAppearance(lf, GridSkins.SkinGridRow, new AppearanceDefault(SystemColors.WindowText, SystemColors.Window))),
 				new AppearanceDefaultInfo(FocusedCellAppearanceName, UpdateAppearance(lf, GridSkins.SkinGridRow, new AppearanceDefault(SystemColors.WindowText, SystemColors.Window))),
				new AppearanceDefaultInfo(SelectedCellAppearanceName, UpdateSystemColors(lf, new AppearanceDefault(SystemColors.HighlightText, SystemColors.Highlight))),
				new AppearanceDefaultInfo(TotalCellAppearanceName, UpdateTotalAppearance(lf, new string[] { GridSkins.SkinGridRow, GridSkins.SkinGridEvenRow }, new AppearanceDefault(SystemColors.InfoText, SystemColors.Info))),
				new AppearanceDefaultInfo(CustomTotalCellAppearanceName, UpdateTotalAppearance(lf, new string[] { GridSkins.SkinGridRow, GridSkins.SkinGridOddRow }, new AppearanceDefault(SystemColors.WindowText, Color.LightGray))),
				new FieldValueAppearanceDefaultInfo(lf, FieldValueAppearanceName, UpdateAppearance(lf, GridSkins.SkinHeader, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control))),
				new AppearanceDefaultInfo(FieldHeaderAppearanceName, UpdateAppearance(lf, GridSkins.SkinHeader, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control))),
				new AppearanceDefaultInfo(HeaderAreaAppearanceName, UpdateAppearance(lf, GridSkins.SkinGridGroupPanel, new AppearanceDefault(SystemColors.ControlText, SystemColors.ControlDark, Color.Empty, SystemColors.ControlLightLight, HorzAlignment.Near, VertAlignment.Center))),
				new AppearanceDefaultInfo(LinesAppearanceName, UpdateAppearance(lf, GridSkins.SkinGridLine, new AppearanceDefault(SystemColors.ControlDark, SystemColors.ControlDark))),
				new AppearanceDefaultInfo(FilterSeparatorAppearanceName, UpdateFilterSeparatorAppearance(lf, GridSkins.SkinGridGroupPanel, new AppearanceDefault(SystemColors.ControlDark, SystemColors.ControlDark))),
				new AppearanceDefaultInfo(HeaderGroupLineAppearanceName, UpdateAppearance(lf, GridSkins.SkinGridFixedLine, new AppearanceDefault(SystemColors.ControlDark, SystemColors.ControlDarkDark))),
				new AppearanceDefaultInfo(FilterHeaderAreaAppearanceName, UpdateHeaderAreaAppearance(lf, GridSkins.SkinGridGroupPanel, new AppearanceDefault(Color.Empty, Color.Empty))),
				new AppearanceDefaultInfo(EmptyAppearanceName, UpdateAppearance(lf, GridSkins.SkinGridRow, new AppearanceDefault(SystemColors.WindowText, SystemColors.Window))),
				new AppearanceDefaultInfo(ExpandButtonAppearanceName, UpdateAppearance(lf, GridSkins.SkinGridLine, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, HorzAlignment.Default, VertAlignment.Center))),
				new AppearanceDefaultInfo(HeaderFilterButtonAppearanceName, UpdateAppearance(lf, GridSkins.SkinGridLine, new AppearanceDefault(Color.Blue, SystemColors.Control, Color.Empty, SystemColors.ControlLightLight, HorzAlignment.Default, VertAlignment.Center))),
				new AppearanceDefaultInfo(HeaderFilterButtonActiveAppearanceName, UpdateAppearance(lf, GridSkins.SkinGridLine, new AppearanceDefault(Color.Blue, SystemColors.Control, SystemColors.ControlLightLight))),
				new AppearanceDefaultInfo(PrefilterPanelName, UpdateAppearance(lf, GridSkins.SkinFooterPanel, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, SystemColors.ControlDark))),
			};
		}
		protected AppearanceDefault UpdateTotalAppearance(UserLookAndFeel lf, string[] elementName, AppearanceDefault info) {
			for(int i = 0; i < elementName.Length; i++)
				info = UpdateTotalAppearance(lf, elementName[i], info);
			return info;
		}
		protected AppearanceDefault UpdateAppearance(UserLookAndFeel lf, string elementName, AppearanceDefault info) {
			SkinElement element = GridSkins.GetSkin(lf)[elementName];
			UpdateAppearanceCore(element, info);
			return info;
		}
		void UpdateAppearanceCore(SkinElement element, AppearanceDefault info) {
			if(element.Color.GetBackColor() != Color.Empty) {
				info.BackColor = element.Color.GetBackColor();
				info.BackColor2 = element.Color.GetBackColor2();
				info.GradientMode = element.Color.GradientMode;
			}
			if(element.Color.FontBold) {
				info.Font = new Font(info.Font == null ? AppearanceObject.DefaultFont : info.Font, FontStyle.Bold);
			}
			if(element.Color.GetForeColor() != Color.Empty) {
				info.ForeColor = element.Color.GetForeColor();
			}
		}
		protected AppearanceDefault UpdateTotalAppearance(UserLookAndFeel lf, string elementName, AppearanceDefault info) {
			SkinElement element = GridSkins.GetSkin(lf)[elementName];
			UpdateAppearanceCore(element, info);
			Color backColor = GetTotalBackColor(element.Color);
			if(backColor != Color.Empty)
				info.BackColor = info.BackColor2 = backColor;
			return info;
		}
		Color GetTotalBackColor(SkinColor skinColor) {
			return skinColor.BackColor2 != Color.Empty ? skinColor.BackColor2 : skinColor.BackColor;
		}
		protected AppearanceDefault UpdateHeaderAreaAppearance(UserLookAndFeel lf, string elementName, AppearanceDefault info) {
			SkinElement element = GridSkins.GetSkin(lf)[elementName];
			if(element.Color.GetBackColor2() != Color.Empty) {
				info.BackColor = element.Color.GetBackColor2();
				info.BackColor2 = element.Color.GetBackColor2();
				info.GradientMode = element.Color.GradientMode;
			}
			if(element.Color.FontBold) {
				info.Font = new Font(info.Font == null ? AppearanceObject.DefaultFont : info.Font, FontStyle.Bold);
			}
			if(element.Color.GetForeColor() != Color.Empty) {
				info.ForeColor = element.Color.GetForeColor();
			}
			return info;
		}
		protected AppearanceDefault UpdateFilterSeparatorAppearance(UserLookAndFeel lf, string elementName, AppearanceDefault info) {
			SkinElement element = GridSkins.GetSkin(lf)[elementName];
			if(element.Border.All != Color.Empty) {
				info.BackColor = element.Border.All;
				info.BackColor2 = element.Border.All;
				info.GradientMode = element.Color.GradientMode;
			}
			if(element.Color.FontBold) {
				info.Font = new Font(info.Font == null ? AppearanceObject.DefaultFont : info.Font, FontStyle.Bold);
			}
			if(element.Color.GetForeColor() != Color.Empty) {
				info.ForeColor = element.Color.GetForeColor();
			}
			return info;
		}
		protected AppearanceDefault UpdateSystemColors(UserLookAndFeel lf, AppearanceDefault info) {
			info.ForeColor = CommonSkins.GetSkin(lf).TranslateColor(info.ForeColor);
			info.BackColor = CommonSkins.GetSkin(lf).TranslateColor(info.BackColor);
			return info;
		}
		protected internal void LoadFieldValueExtraProperties(AppearanceDefaultInfo[] DefaultAppearance) {
			FieldValueTopBorderColor = Color.Empty;
			FieldValueLeftRightBorderColor = Color.Empty;
			foreach(AppearanceDefaultInfo appearanceDI in DefaultAppearance) {
				if(appearanceDI.Name == FieldValueAppearanceName) {
					FieldValueAppearanceDefaultInfo pivotAppearanceDI = appearanceDI as FieldValueAppearanceDefaultInfo;
					if(pivotAppearanceDI == null)
						continue;
					FieldValueTopBorderColor = pivotAppearanceDI.HeaderTopBorder;
					FieldValueLeftRightBorderColor = pivotAppearanceDI.HeaderLeftRightBorder;
					FieldValueTopMarginCorrection = pivotAppearanceDI.HeaderTopMarginCorrection;
				}
			}
		}
	}
	public class FieldValueAppearanceDefaultInfo : AppearanceDefaultInfo {
		const string HeaderTopBorderColorName = "HeaderTopBorder";
		const string HeaderLeftRightBorderColorName = "HeaderLeftRightBorder";
		const string HeaderTopMarginCorrectionName = "HeaderTopMarginCorrection";
		Color headerTopBorder;
		Color headerLeftRightBorder;
		int headerTopMarginCorrection;
		public FieldValueAppearanceDefaultInfo(UserLookAndFeel lf, string name, AppearanceDefault defaultAppearance)
			: base(name, defaultAppearance) {
			Skin skin = GridSkins.GetSkin(lf);
			if(skin.Colors.Contains(HeaderTopBorderColorName)) {
				HeaderTopBorder = skin.Colors[HeaderTopBorderColorName];
			}
			if(skin.Colors.Contains(HeaderLeftRightBorderColorName)) {
				HeaderLeftRightBorder = skin.Colors[HeaderLeftRightBorderColorName];
			}
			headerTopMarginCorrection = skin.Properties.GetInteger(HeaderTopMarginCorrectionName);
		}
		public Color HeaderTopBorder {
			get { return headerTopBorder; }
			set { headerTopBorder = value; }
		}
		public Color HeaderLeftRightBorder {
			get { return headerLeftRightBorder; }
			set { headerLeftRightBorder = value; }
		}
		public int HeaderTopMarginCorrection {
			get { return headerTopMarginCorrection; }
			set { headerTopMarginCorrection = value; }
		}
	}
	public class PivotGridAppearancesPrint : PivotGridAppearancesBase {
		public PivotGridAppearancesPrint(PivotGridViewInfoData data)
			: base(data) { }
		public virtual AppearanceDefaultInfo[] GetAppearanceDefaultInfo() {
			return new AppearanceDefaultInfo[] {
				new AppearanceDefaultInfo(FieldHeaderAppearanceName, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control)),
				new AppearanceDefaultInfo(CellAppearanceName, new AppearanceDefault(SystemColors.WindowText, SystemColors.Window)),
				new AppearanceDefaultInfo(TotalCellAppearanceName, new AppearanceDefault(Color.Empty, Color.Empty)),
				new AppearanceDefaultInfo(CustomTotalCellAppearanceName, new AppearanceDefault(SystemColors.WindowText, Color.LightGray)),
				new AppearanceDefaultInfo(GrandTotalCellAppearanceName, new AppearanceDefault(Color.Empty, Color.Empty)),
				new AppearanceDefaultInfo(FieldValueAppearanceName, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control)),
				new AppearanceDefaultInfo(FieldValueTotalAppearanceName, new AppearanceDefault(Color.Empty, Color.Empty)),
				new AppearanceDefaultInfo(FieldValueGrandTotalAppearanceName, new AppearanceDefault(Color.Empty, Color.Empty)),
				new AppearanceDefaultInfo(LinesAppearanceName, new AppearanceDefault(SystemColors.ControlDark, SystemColors.ControlDark)),
				new AppearanceDefaultInfo(FilterSeparatorAppearanceName, new AppearanceDefault(SystemColors.ControlDark, SystemColors.ControlDark))
			};
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override AppearanceObject FilterSeparator { get { return base.FilterSeparator; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override AppearanceObject HeaderGroupLine { get { return base.HeaderGroupLine; } }
	}
	public class PivotGridFieldAppearances : BaseAppearanceCollection {
		protected const string ValueAppearanceName = "Value";
		protected const string ValueTotalAppearanceName = "ValueTotal";
		protected const string ValueGrandTotalAppearanceName = "ValueGrandTotal";
		protected const string HeaderAppearanceName = "Header";
		protected const string CellAppearanceName = "Cell";
		protected const string CellTotalAppearanceName = "CellTotal";
		protected const string CellGrandTotalAppearanceName = "CellGrandTotal";
		AppearanceObject value, valueTotal, header, cell, cellTotal, cellGrandTotal, valueGrandTotal;
		protected override void CreateAppearances() {
			this.value = CreateAppearance(ValueAppearanceName);
			this.header = CreateAppearance(HeaderAppearanceName);
			this.valueTotal = CreateAppearance(ValueTotalAppearanceName);
			this.valueGrandTotal = CreateAppearance(ValueGrandTotalAppearanceName);
			this.cell = CreateAppearance(CellAppearanceName);
			this.cellTotal = CreateAppearance(CellTotalAppearanceName);
			this.cellGrandTotal = CreateAppearance(CellGrandTotalAppearanceName);
		}
		readonly PivotGridField field;
		public PivotGridFieldAppearances(PivotGridField field) {
			this.field = field;
		}
#if !SL
	[DevExpressXtraPivotGridLocalizedDescription("PivotGridFieldAppearancesIsLoading")]
#endif
		public override bool IsLoading { get { return field != null ? field.IsLoading : false; } }
		bool ShouldSerializeValue() { return Value.ShouldSerialize(); }
		void ResetValue() { Value.Reset(); }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridFieldAppearancesValue"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject Value { get { return value; } }
		bool ShouldSerializeHeader() { return Header.ShouldSerialize(); }
		void ResetHeader() { Header.Reset(); }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridFieldAppearancesHeader"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject Header { get { return header; } }
		bool ShouldSerializeValueTotal() { return ValueTotal.ShouldSerialize(); }
		void ResetValueTotal() { ValueTotal.Reset(); }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridFieldAppearancesValueTotal"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject ValueTotal { get { return valueTotal; } }
		bool ShouldSerializeValueGrandTotal() { return ValueGrandTotal.ShouldSerialize(); }
		void ResetValueGrandTotal() { ValueGrandTotal.Reset(); }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridFieldAppearancesValueGrandTotal"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject ValueGrandTotal { get { return valueGrandTotal; } }
		bool ShouldSerializeCell() { return Cell.ShouldSerialize(); }
		void ResetCell() { Cell.Reset(); }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridFieldAppearancesCell"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject Cell { get { return cell; } }
		bool ShouldSerializeCellTotal() { return CellTotal.ShouldSerialize(); }
		void ResetCellTotal() { CellTotal.Reset(); }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridFieldAppearancesCellTotal"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject CellTotal { get { return cellTotal; } }
		bool ShouldSerializeCellGrandTotal() { return CellGrandTotal.ShouldSerialize(); }
		void ResetCellGrandTotal() { CellGrandTotal.Reset(); }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridFieldAppearancesCellGrandTotal"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject CellGrandTotal { get { return cellGrandTotal; } }
	}
}
