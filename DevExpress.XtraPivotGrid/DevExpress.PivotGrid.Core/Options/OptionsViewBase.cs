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
using System.ComponentModel;
using DevExpress.WebUtils;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Design;
using DevExpress.Utils.Controls;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Data.Utils;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraPivotGrid {
	[TypeConverter(typeof(EnumTypeConverter))]
	[ResourceFinder(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile)]
	public enum PivotTotalsLocation { Near, Far }
	[TypeConverter(typeof(EnumTypeConverter))]
	[ResourceFinder(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile)]
	public enum PivotRowTotalsLocation { Near, Far, Tree }
	public static class TotalsLocationHelper {
		public static PivotTotalsLocation ToTotalsLocation(PivotRowTotalsLocation value, PivotTotalsLocation defaultValue) {
			switch(value) {
				case PivotRowTotalsLocation.Near:
					return PivotTotalsLocation.Near;
				case PivotRowTotalsLocation.Far:
					return PivotTotalsLocation.Far;
				default:
					return defaultValue;
			}
		}
		public static PivotRowTotalsLocation ToRowTotalsLocation(PivotTotalsLocation value) {
			switch(value) {
				case PivotTotalsLocation.Near:
					return PivotRowTotalsLocation.Near;
				case PivotTotalsLocation.Far:
					return PivotRowTotalsLocation.Far;
				default:
					throw new ArgumentException("value");
			}
		}
	}
	public class PivotGridOptionsViewBase : PivotGridOptionsBase {
		public const int DefaultRowTreeOffset = 21;
		internal const string TreeLikeNoTotalsExceptionText = "The ShowRowTotals option must be enabled when the 'Tree' row area mode is used.";
		const int DefaultHeaderOffset = 3;
		int headerWidthOffset, headerHeightOffset;
		bool showColumnTotals;
		bool showRowTotals;
		bool showColumnGrandTotals;
		bool showRowGrandTotals;
		PivotTotalsLocation columnTotalsLocation;
		PivotRowTotalsLocation rowTotalsLocation;
		bool showTotalsForSingleValues;
		bool showCustomTotalsForSingleValues;
		bool showGrandTotalsForSingleValues;
		bool showVertLines;
		bool showHorzLines;
		bool[] showHeaders;
		bool drawFocusedCellRect;
		bool showFilterSeparatorBar;
		bool showColumnGrandTotalHeader;
		bool showRowGrandTotalHeader;
		int rowTreeWidth, rowTreeOffset;
		bool groupFieldsInCustomizationWindow;
		public PivotGridOptionsViewBase(EventHandler optionsChanged)
			: this(optionsChanged, null, string.Empty) {
		}
		public PivotGridOptionsViewBase(EventHandler optionsChanged, IViewBagOwner viewBagOwner, string objectPath)
			: base(optionsChanged, viewBagOwner, objectPath) {
			this.showColumnTotals = false;
			this.showRowTotals = false;
			this.showColumnGrandTotals = false;
			this.showRowGrandTotals = false;
			this.columnTotalsLocation = PivotTotalsLocation.Far;
			this.rowTotalsLocation = PivotRowTotalsLocation.Far;
			this.showTotalsForSingleValues = false;
			this.showCustomTotalsForSingleValues = false;
			this.showGrandTotalsForSingleValues = false;
			this.showHeaders = new bool[Helpers.GetEnumValues(typeof(PivotArea)).Length];
			for(int i = 0; i < this.showHeaders.Length; i++)
				this.showHeaders[i] = true;
			this.showHorzLines = true;
			this.showVertLines = true;
			this.headerWidthOffset = headerHeightOffset = DefaultHeaderOffset;
			this.drawFocusedCellRect = true;
			this.showFilterSeparatorBar = true;
			this.showColumnGrandTotalHeader = true;
			this.showRowGrandTotalHeader = true;
			this.rowTreeWidth = PivotGridFieldBase.DefaultWidth;
			this.rowTreeOffset = DefaultRowTreeOffset;
			this.groupFieldsInCustomizationWindow = true;
		}
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridOptionsView.GroupFieldsInCustomizationWindow")]
		[DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true)]
		[TypeConverter(typeof(BooleanTypeConverter))]
		public bool GroupFieldsInCustomizationWindow {
			get { return groupFieldsInCustomizationWindow; }
			set {
				if(value == GroupFieldsInCustomizationWindow) return;
				groupFieldsInCustomizationWindow = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsViewBaseHeaderWidthOffset"),
#endif
		DefaultValue(DefaultHeaderOffset), XtraSerializableProperty(), NotifyParentProperty(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.HeaderWidthOffset")]
		public int HeaderWidthOffset {
			get { return headerWidthOffset; }
			set {
				if(value == HeaderWidthOffset) return;
				headerWidthOffset = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsViewBaseHeaderHeightOffset"),
#endif
		DefaultValue(DefaultHeaderOffset), XtraSerializableProperty(), NotifyParentProperty(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.HeaderHeightOffset")]
		public int HeaderHeightOffset {
			get { return headerHeightOffset; }
			set {
				if(value == HeaderHeightOffset) return;
				headerHeightOffset = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsViewBaseShowColumnTotals"),
#endif
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.ShowColumnTotals"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool ShowColumnTotals {
			get { return showColumnTotals; }
			set {
				if(ShowColumnTotals == value) return;
				showColumnTotals = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsViewBaseShowRowTotals"),
#endif
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.ShowRowTotals"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool ShowRowTotals {
			get { return showRowTotals; }
			set {
				if(ShowRowTotals == value) return;
				bool oldValue = showRowTotals;
				showRowTotals = value;
				try {
					OnOptionsChanged();
				} catch {
					showRowTotals = oldValue;
					throw;
				}
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsViewBaseShowColumnGrandTotals"),
#endif
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.ShowColumnGrandTotals"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool ShowColumnGrandTotals {
			get { return showColumnGrandTotals; }
			set {
				if(ShowColumnGrandTotals == value) return;
				showColumnGrandTotals = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsViewBaseShowRowGrandTotals"),
#endif
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.ShowRowGrandTotals"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool ShowRowGrandTotals {
			get { return showRowGrandTotals; }
			set {
				if(ShowRowGrandTotals == value) return;
				showRowGrandTotals = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsViewBaseColumnTotalsLocation"),
#endif
		DefaultValue(PivotTotalsLocation.Far), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile,
			"DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.ColumnTotalsLocation")
		]
		public PivotTotalsLocation ColumnTotalsLocation {
			get { return columnTotalsLocation; }
			set {
				if(columnTotalsLocation == value) return;
				columnTotalsLocation = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsViewBaseRowTotalsLocation"),
#endif
		DefaultValue(PivotRowTotalsLocation.Far), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile,
			"DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.RowTotalsLocation")
		]
		public PivotRowTotalsLocation RowTotalsLocation {
			get { return rowTotalsLocation; }
			set {
				if(rowTotalsLocation == value) return;
				PivotRowTotalsLocation oldValue = rowTotalsLocation;
				rowTotalsLocation = value;
				try {
					OnOptionsChanged();
				} catch {
					rowTotalsLocation = oldValue;
					throw;
				}
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsViewBaseShowTotalsForSingleValues"),
#endif
		DefaultValue(false), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.ShowTotalsForSingleValues"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool ShowTotalsForSingleValues {
			get { return showTotalsForSingleValues; }
			set {
				if(value == ShowTotalsForSingleValues) return;
				bool oldValue = showTotalsForSingleValues;
				showTotalsForSingleValues = value;
				try {
					OnOptionsChanged();
				} catch {
					showTotalsForSingleValues = oldValue;
					throw;
				}
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsViewBaseShowCustomTotalsForSingleValues"),
#endif
		DefaultValue(false), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.ShowCustomTotalsForSingleValues"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool ShowCustomTotalsForSingleValues {
			get { return showCustomTotalsForSingleValues; }
			set {
				if(value == ShowCustomTotalsForSingleValues) return;
				showCustomTotalsForSingleValues = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsViewBaseShowGrandTotalsForSingleValues"),
#endif
		DefaultValue(false), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.ShowGrandTotalsForSingleValues"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool ShowGrandTotalsForSingleValues {
			get { return showGrandTotalsForSingleValues; }
			set {
				if(value == ShowGrandTotalsForSingleValues) return;
				showGrandTotalsForSingleValues = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsViewBaseShowColumnGrandTotalHeader"),
#endif
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.ShowColumnGrandTotalHeader"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool ShowColumnGrandTotalHeader {
			get { return showColumnGrandTotalHeader; }
			set {
				if(value == ShowColumnGrandTotalHeader) return;
				showColumnGrandTotalHeader = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsViewBaseShowRowGrandTotalHeader"),
#endif
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.ShowRowGrandTotalHeader"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool ShowRowGrandTotalHeader {
			get { return showRowGrandTotalHeader; }
			set {
				if(value == ShowRowGrandTotalHeader) return;
				showRowGrandTotalHeader = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsViewBaseShowHorzLines"),
#endif
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.ShowHorzLines"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool ShowHorzLines {
			get { return showHorzLines; }
			set {
				if(value == ShowHorzLines) return;
				showHorzLines = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsViewBaseShowVertLines"),
#endif
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.ShowVertLines"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool ShowVertLines {
			get { return showVertLines; }
			set {
				if(value == ShowVertLines) return;
				showVertLines = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsViewBaseShowDataHeaders"),
#endif
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.ShowDataHeaders"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool ShowDataHeaders {
			get { return GetShowHeaders(PivotArea.DataArea); }
			set { SetShowHeaders(PivotArea.DataArea, value); }
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsViewBaseShowFilterHeaders"),
#endif
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.ShowFilterHeaders"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool ShowFilterHeaders {
			get { return GetShowHeaders(PivotArea.FilterArea); }
			set { SetShowHeaders(PivotArea.FilterArea, value); }
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsViewBaseShowColumnHeaders"),
#endif
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.ShowColumnHeaders"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool ShowColumnHeaders {
			get { return GetShowHeaders(PivotArea.ColumnArea); }
			set { SetShowHeaders(PivotArea.ColumnArea, value); }
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsViewBaseShowRowHeaders"),
#endif
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.ShowRowHeaders"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool ShowRowHeaders {
			get { return GetShowHeaders(PivotArea.RowArea); }
			set { SetShowHeaders(PivotArea.RowArea, value); }
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsViewBaseDrawFocusedCellRect"),
#endif
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.DrawFocusedCellRect"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool DrawFocusedCellRect {
			get { return drawFocusedCellRect; }
			set {
				if(value == DrawFocusedCellRect) return;
				drawFocusedCellRect = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsViewBaseShowFilterSeparatorBar"),
#endif
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsViewBase.ShowFilterSeparatorBar"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool ShowFilterSeparatorBar {
			get { return showFilterSeparatorBar; }
			set {
				if(value == ShowFilterSeparatorBar) return;
				showFilterSeparatorBar = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsViewBaseRowTreeWidth"),
#endif
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile,
			"DevExpress.XtraPivotGrid.PivotGridOptionsViewBase.RowTreeWidth"), NotifyParentProperty(true),
		DefaultValue(PivotGridFieldBase.DefaultWidth), XtraSerializableProperty()
		]
		public virtual int RowTreeWidth {
			get { return rowTreeWidth; }
			set {
				if(rowTreeWidth == value) return;
				rowTreeWidth = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsViewBaseRowTreeOffset"),
#endif
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile,
			"DevExpress.XtraPivotGrid.PivotGridOptionsViewBase.RowTreeOffset"), NotifyParentProperty(true),
		DefaultValue(PivotGridOptionsViewBase.DefaultRowTreeOffset), XtraSerializableProperty()
		]
		public virtual int RowTreeOffset {
			get { return rowTreeOffset; }
			set {
				if(rowTreeOffset == value) return;
				rowTreeOffset = value;
				OnOptionsChanged();
			}
		}
		public bool GetShowHeaders(PivotArea area) {
			return this.showHeaders[(int)area];
		}
		protected void SetShowHeaders(PivotArea area, bool value) {
			if(value == GetShowHeaders(area)) return;
			this.showHeaders[(int)area] = value;
			OnOptionsChanged();
		}
		string GetShowHeadersViewBagName(PivotArea area) {
			switch(area) {
				case PivotArea.ColumnArea: return "ShowColumnHeaders";
				case PivotArea.RowArea: return "ShowRowHeaders";
				case PivotArea.DataArea: return "ShowDataHeaders";
				case PivotArea.FilterArea: return "ShowFilterHeaders";
			}
			return string.Empty;
		}
		public void ShowAllTotals() {
			ShowAllTotals(true);
		}
		public void HideAllTotals() {
			ShowAllTotals(false);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsTotalsFar(bool isColumn, PivotGridValueType valueType) {
			if(isColumn)
				return ColumnTotalsLocation == PivotTotalsLocation.Far;
			else {
				if(RowTotalsLocation == PivotRowTotalsLocation.Tree)
					return valueType == PivotGridValueType.GrandTotal;
				else
					return RowTotalsLocation == PivotRowTotalsLocation.Far;
			}
		}
		public void SetBothTotalsLocation(PivotTotalsLocation value) {
			ColumnTotalsLocation = value;
			RowTotalsLocation = (PivotRowTotalsLocation)value;
		}
		protected void ShowAllTotals(bool show) {
			BeginUpdate();
			ShowColumnTotals = show;
			ShowColumnGrandTotals = show;
			ShowRowTotals = show;
			ShowRowGrandTotals = show;
			EndUpdate();
		}
		public override void Reset() {
			BeginUpdate();
			ShowAllTotals(true);
			ColumnTotalsLocation = PivotTotalsLocation.Far;
			RowTotalsLocation = PivotRowTotalsLocation.Far;
			ShowTotalsForSingleValues = false;
			ShowCustomTotalsForSingleValues = false;
			ShowGrandTotalsForSingleValues = false;
			for(int i = 0; i < this.showHeaders.Length; i++)
				showHeaders[i] = true;
			ShowHorzLines = true;
			ShowVertLines = true;
			RowTreeWidth = PivotGridFieldBase.DefaultWidth;
			RowTreeOffset = DefaultRowTreeOffset;
			EndUpdate();
		}
		internal void Validate() {
			ValidateRowTotalsLocation();
		}
		void ValidateRowTotalsLocation() {
			if(RowTotalsLocation != PivotRowTotalsLocation.Tree) return;
			if(!ShowRowTotals)
				throw new ArgumentException(TreeLikeNoTotalsExceptionText);
		}
	}
}
