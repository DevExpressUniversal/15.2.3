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
using System.ComponentModel;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.TypeConverters;
namespace DevExpress.XtraPivotGrid {
	public abstract class FormatRuleSettings : IPivotGridViewInfoDataOwner {
		internal static string GetSerializationName(FormatRuleSettings sett) {
			return sett.GetType().Name;
		}
		internal static FormatRuleSettings Create(string name) {
			return Activator.CreateInstance(Type.GetType("DevExpress.XtraPivotGrid." + name)) as FormatRuleSettings;
		}
		internal static string GetDisplayText(Type type) {
			if(type == typeof(FormatRuleFieldIntersectionSettings))
				return "Format cells by Row and Column field";
			if(type == typeof(FormatRuleTotalTypeSettings))
				return "Format cells by Row and Column value type";
			return "Format cells";
		}
		PivotGridFormatRule rule;
		internal PivotGridFormatRule Rule {
			get { return rule; }
			set { rule = value; }
		}
		protected PivotGridViewInfoData Data { get { return rule == null ? null : rule.Data; } }
		protected void ItemChanged() {
			if(Rule != null)
				Rule.ItemChanged();
		}
		protected internal abstract bool IsValid();
		protected internal abstract bool CanApplyToCell(PivotGridCellItem item);
		PivotGridViewInfoData IPivotGridViewInfoDataOwner.DataViewInfo {
			get { return Data; }
		}
	}
	public class FormatRuleFieldIntersectionSettings : FormatRuleSettings {
		PivotGridField row, column;
		internal FormatRuleFieldIntersectionSettings(PivotGridCellItem item) {
			Column = (PivotGridField)item.Data.GetField(item.ColumnField);
			Row = (PivotGridField)item.Data.GetField(item.RowField);
		}
		public FormatRuleFieldIntersectionSettings() {
		}
		[
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFormatRule.Row"),
		DefaultValue(null), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		TypeConverter("DevExpress.XtraPivotGrid.TypeConverters.FieldReferenceConverter, " + AssemblyInfo.SRAssemblyPivotGrid),
		PivotAreaProperty(PivotArea.RowArea),
		Category("Intersection"),
		RefreshProperties(RefreshProperties.All)
		]
		public PivotGridField Row {
			get { return row; }
			set {
				row = value;
				ItemChanged();
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public string RowName {
			get {
				if(Row != null)
					return Row.Name;
				return string.Empty;
			}
			set {
				if(value != null && value != string.Empty && Data != null) {
					Row = Data.Fields.GetFieldByName(value);
				}
			}
		}
		[
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFormatRule.Column"),
		DefaultValue(null), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		TypeConverter("DevExpress.XtraPivotGrid.TypeConverters.FieldReferenceConverter, " + AssemblyInfo.SRAssemblyPivotGrid),
		PivotAreaProperty(PivotArea.ColumnArea),
		Category("Intersection"),
		RefreshProperties(RefreshProperties.All)
		]
		public PivotGridField Column {
			get { return column; }
			set {
				column = value;
				ItemChanged();
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public string ColumnName {
			get {
				if(Column != null)
					return Column.Name;
				return string.Empty;
			}
			set {
				if(value != null && value != string.Empty && Data != null) {
					Column = Data.Fields.GetFieldByName(value);
				}
			}
		}
		protected internal override bool CanApplyToCell(PivotGridCellItem item) {
			return Data != null && item.RowField == Data.GetFieldItem(Row) && item.ColumnField == Data.GetFieldItem(Column);
		}
		protected internal override bool IsValid() {
			return true;
		}
		public override string ToString() {
			return string.Format("Format cells where Column field is {0} and Row field is {1}", Column != null ? ColumnName : "Grand Total", Row != null ? RowName : "Grand Total");
		}
	}
	public class FormatRuleTotalTypeSettings : FormatRuleSettings {
		bool applytoCell = true, applytoTotalCell = true, applytoGrandTotalCell = true, applytoCustomTotalCell = true;
		[
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFormatRule.ApplyToCell"),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty(), DefaultValue(true),
		Category("Intersection"),
		RefreshProperties(RefreshProperties.All)
		]
		public bool ApplyToCell {
			get { return applytoCell; }
			set {
				if(ApplyToCell == value)
					return;
				applytoCell = value;
				ItemChanged();
			}
		}
		[
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFormatRule.ApplyToTotalCell"),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty(), DefaultValue(true),
		Category("Intersection"),
		RefreshProperties(RefreshProperties.All)
		]
		public bool ApplyToTotalCell {
			get { return applytoTotalCell; }
			set {
				if(ApplyToTotalCell == value)
					return;
				applytoTotalCell = value;
				ItemChanged();
			}
		}
		[
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFormatRule.ApplyToGrandTotalCell"),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty(),
		DefaultValue(true),
		Category("Intersection"),
		RefreshProperties(RefreshProperties.All)
		]
		public bool ApplyToGrandTotalCell {
			get { return applytoGrandTotalCell; }
			set {
				if(ApplyToGrandTotalCell == value)
					return;
				applytoGrandTotalCell = value;
				ItemChanged();
			}
		}
		[
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFormatRule.ApplyToCustomTotalCell"),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty(),
		DefaultValue(true),
		Category("Intersection"),
		RefreshProperties(RefreshProperties.All)
		]
		public bool ApplyToCustomTotalCell {
			get { return applytoCustomTotalCell; }
			set {
				if(ApplyToCustomTotalCell == value)
					return;
				applytoCustomTotalCell = value;
				ItemChanged();
			}
		}
		protected internal override bool CanApplyToCell(PivotGridCellItem item) {
			switch(item.GetCellType()) {
				case PivotGridCellType.Cell:
					return ApplyToCell;
				case PivotGridCellType.Total:
					return ApplyToTotalCell;
				case PivotGridCellType.GrandTotal:
					return ApplyToGrandTotalCell;
				case PivotGridCellType.CustomTotal:
					return ApplyToCustomTotalCell;
				default:
					return true;
			}
		}
		protected internal override bool IsValid() {
			return ApplyToCell || ApplyToTotalCell || ApplyToGrandTotalCell || ApplyToCustomTotalCell;
		}
		public override string ToString() {
			List<string> types = new List<string>();
			if(applytoCell)
				types.Add("Cell");
			if(applytoTotalCell)
				types.Add("Total");
			if(applytoCustomTotalCell)
				types.Add("Custom Total");
			if(applytoGrandTotalCell)
				types.Add("Grand Total");
			if(types.Count == 0)
				return "No Formatting";
			if(types.Count == 4)
				return "Format all cells";
			return string.Format("Format {0}", string.Join(", ", types.ToArray()));
		}
	}
}
